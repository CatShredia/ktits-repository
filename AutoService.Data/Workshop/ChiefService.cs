using System.Data;
using System.Globalization;

namespace AutoService.Data.Workshop;

public sealed class ChiefService
{
    private readonly IDbContextFactory<AutoServiceDbContext> _factory;

    public ChiefService(IDbContextFactory<AutoServiceDbContext> factory) => _factory = factory;

    public async Task<IReadOnlyList<AnalyticsSection>> GetReportAsync(DateTime from, DateTime to)
    {
        var start = from.Date;
        var end = to.Date.AddDays(1);
        if (end <= start)
            throw new WorkshopException("Дата окончания должна быть не раньше даты начала.");

        await using var db = await _factory.CreateDbContextAsync();
        var orders = await ScalarAsync(db, """
            SELECT COUNT(*)::bigint
            FROM work_orders
            WHERE created_at >= @from AND created_at < @to
            """, start, end);
        var revenue = await ScalarAsync(db, """
            SELECT COALESCE(SUM(amount), 0)
            FROM payments
            WHERE status = 'Проведён' AND paid_at >= @from AND paid_at < @to
            """, start, end);
        var average = await ScalarAsync(db, """
            SELECT COALESCE(AVG(total), 0)
            FROM invoices
            WHERE created_at >= @from AND created_at < @to
            """, start, end);
        var servicesDone = await ScalarAsync(db, """
            SELECT COUNT(*)::bigint
            FROM work_order_services AS line
            JOIN work_orders AS work_order ON work_order.id = line.work_order_id
            WHERE work_order.created_at >= @from AND work_order.created_at < @to
            """, start, end);

        var workload = await RowsAsync(db, """
            SELECT person.full_name, COUNT(appointment.id)::bigint
            FROM mechanics AS mechanic
            JOIN users AS person ON person.id = mechanic.user_id
            LEFT JOIN appointments AS appointment ON appointment.mechanic_id = mechanic.id
                AND appointment.scheduled_at >= @from AND appointment.scheduled_at < @to
                AND appointment.status <> 'Отменена'
            GROUP BY mechanic.id, person.full_name
            ORDER BY person.full_name
            """, start, end, static row => new MetricRow { Title = row[0], Value = row[1] + " записей" });

        var popular = await RowsAsync(db, """
            SELECT service.name, COUNT(*)::bigint
            FROM work_order_services AS line
            JOIN services AS service ON service.id = line.service_id
            JOIN work_orders AS work_order ON work_order.id = line.work_order_id
            WHERE work_order.created_at >= @from AND work_order.created_at < @to
            GROUP BY service.id, service.name
            ORDER BY COUNT(*) DESC, service.name
            """, start, end, static row => new MetricRow { Title = row[0], Value = row[1] });

        var lowStock = await RowsAsync(db, """
            SELECT name || ' (' || sku || ')', quantity::text || ' при минимуме ' || min_quantity::text
            FROM parts
            WHERE quantity < min_quantity
            ORDER BY name
            """, null, null, static row => new MetricRow { Title = row[0], Value = row[1] });

        var ordersByMechanic = await RowsAsync(db, """
            SELECT person.full_name, COUNT(work_order.id)::bigint
            FROM mechanics AS mechanic
            JOIN users AS person ON person.id = mechanic.user_id
            LEFT JOIN work_orders AS work_order ON work_order.mechanic_id = mechanic.id
                AND work_order.created_at >= @from AND work_order.created_at < @to
            GROUP BY mechanic.id, person.full_name
            ORDER BY person.full_name
            """, start, end, static row => new MetricRow { Title = row[0], Value = row[1] });

        var expensiveServices = await RowsAsync(db, """
            SELECT name, base_price
            FROM services
            WHERE base_price > (SELECT AVG(base_price) FROM services)
            ORDER BY base_price DESC, name
            """, null, null, static row => new MetricRow { Title = row[0], Value = row[1] + " ₽" });

        var repeatClients = await RowsAsync(db, """
            SELECT client.full_name, (SELECT COUNT(*) FROM appointments AS visit WHERE visit.client_id = client.id)::bigint
            FROM clients AS client
            WHERE (SELECT COUNT(*) FROM appointments AS visit WHERE visit.client_id = client.id) > 1
            ORDER BY client.full_name
            """, null, null, static row => new MetricRow { Title = row[0], Value = row[1] + " обращений" });

        var busyMechanics = await RowsAsync(db, """
            SELECT person.full_name, COUNT(work_order.id)::bigint
            FROM mechanics AS mechanic
            JOIN users AS person ON person.id = mechanic.user_id
            LEFT JOIN work_orders AS work_order ON work_order.mechanic_id = mechanic.id
                AND work_order.created_at >= @from AND work_order.created_at < @to
            GROUP BY mechanic.id, person.full_name
            HAVING COUNT(work_order.id) > (
                SELECT AVG(order_count)
                FROM (
                    SELECT COUNT(other_order.id) AS order_count
                    FROM mechanics AS other_mechanic
                    LEFT JOIN work_orders AS other_order ON other_order.mechanic_id = other_mechanic.id
                        AND other_order.created_at >= @from AND other_order.created_at < @to
                    GROUP BY other_mechanic.id
                ) AS counts
            )
            ORDER BY COUNT(work_order.id) DESC, person.full_name
            """, start, end, static row => new MetricRow { Title = row[0], Value = row[1] + " заказов" });

        var revenueRollup = await RowsAsync(db, """
            SELECT
                CASE WHEN date_trunc('month', paid_at) IS NULL THEN 'Итого' ELSE to_char(date_trunc('month', paid_at), 'MM.YYYY') END,
                CASE
                    WHEN date_trunc('month', paid_at) IS NULL THEN 'все месяцы и способы'
                    WHEN method IS NULL THEN 'все способы за месяц'
                    ELSE method
                END,
                COALESCE(SUM(amount), 0)
            FROM payments
            WHERE status = 'Проведён' AND paid_at >= @from AND paid_at < @to
            GROUP BY ROLLUP(date_trunc('month', paid_at), method)
            """, start, end, static row => new MetricRow { Title = row[0] + " · " + row[1], Value = row[2] + " ₽" });

        var orderCube = await RowsAsync(db, """
            SELECT
                CASE WHEN person.full_name IS NULL THEN 'Все механики' ELSE person.full_name END,
                CASE WHEN work_order.status IS NULL THEN 'Все статусы' ELSE work_order.status END,
                COUNT(*)::bigint
            FROM work_orders AS work_order
            JOIN mechanics AS mechanic ON mechanic.id = work_order.mechanic_id
            JOIN users AS person ON person.id = mechanic.user_id
            WHERE work_order.created_at >= @from AND work_order.created_at < @to
            GROUP BY CUBE(person.full_name, work_order.status)
            """, start, end, static row => new MetricRow { Title = row[0] + " · " + row[1], Value = row[2] });

        return
        [
            Section("Сводка за период", "Число заказов, выручка по проведённым платежам, средняя сумма счёта и число услуг в заказ-нарядах.",
            [
                new MetricRow { Title = "Общее количество заказов", Value = orders },
                new MetricRow { Title = "Выручка", Value = Money(revenue) + " ₽" },
                new MetricRow { Title = "Средняя стоимость ремонта", Value = Money(average) + " ₽" },
                new MetricRow { Title = "Количество выполненных услуг", Value = servicesDone }
            ]),
            Section("Загруженность механиков", "Число неотменённых записей на обслуживание в выбранном периоде.", workload),
            Section("Наиболее востребованные услуги", "Сколько раз услуга попала в заказ-наряды периода.", popular),
            Section("Запчасти ниже минимального остатка", "Складской остаток меньше минимального. Период на этот список не влияет.", lowStock),
            Section("Количество заказов по каждому механику", "Заказ-наряды с датой создания в выбранном периоде.", ordersByMechanic),
            Section("Услуги дороже средней стоимости", "Вложенный запрос: базовая стоимость больше SELECT AVG(base_price) FROM services.", expensiveServices),
            Section("Клиенты с несколькими обращениями", "Вложенный запрос: у клиента больше одной записи на обслуживание.", repeatClients),
            Section("Механики с числом заказов выше среднего", "Вложенный запрос: заказов у механика больше, чем среднее число заказов по механикам за период.", busyMechanics),
            Section("Выручка, ROLLUP", "GROUP BY ROLLUP(месяц, способ оплаты) по проведённым платежам. «Все способы за месяц» и «Итого» — промежуточный и общий итоги.", revenueRollup),
            Section("Число заказов, CUBE", "GROUP BY CUBE(механик, статус). «Все механики» и «Все статусы» — итоги по каждому измерению и общий итог.", orderCube)
        ];
    }

    private static AnalyticsSection Section(string title, string explanation, IReadOnlyList<MetricRow> rows)
        => new()
        {
            Title = title,
            Explanation = explanation,
            Rows = rows.Count == 0 ? [new MetricRow { Title = "Нет данных" }] : rows
        };

    private static string Money(string value)
        => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)
            ? amount.ToString("0.00", CultureInfo.InvariantCulture)
            : value;

    private static async Task<string> ScalarAsync(AutoServiceDbContext db, string sql, DateTime from, DateTime to)
    {
        var rows = await ReadAsync(db, sql, from, to);
        return rows.Count == 0 ? "0" : rows[0][0];
    }

    private static async Task<IReadOnlyList<MetricRow>> RowsAsync(
        AutoServiceDbContext db,
        string sql,
        DateTime? from,
        DateTime? to,
        Func<string[], MetricRow> map)
    {
        var rows = await ReadAsync(db, sql, from, to);
        return rows.Select(map).ToList();
    }

    private static async Task<List<string[]>> ReadAsync(AutoServiceDbContext db, string sql, DateTime? from, DateTime? to)
    {
        var connection = db.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;
        if (openedHere)
            await connection.OpenAsync();
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            AddDate(command, "from", from);
            AddDate(command, "to", to);
            var rows = new List<string[]>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var values = new string[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        values[i] = "";
                        continue;
                    }

                    var value = reader.GetValue(i);
                    values[i] = value switch
                    {
                        decimal number => number.ToString("0.00", CultureInfo.InvariantCulture),
                        double number => number.ToString("0.00", CultureInfo.InvariantCulture),
                        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""
                    };
                }

                rows.Add(values);
            }

            return rows;
        }
        finally
        {
            if (openedHere)
                await connection.CloseAsync();
        }
    }

    private static void AddDate(System.Data.Common.DbCommand command, string name, DateTime? value)
    {
        if (value is not DateTime date)
            return;
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
        command.Parameters.Add(parameter);
    }
}
