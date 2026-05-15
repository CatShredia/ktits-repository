using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ProductionSystem.Client.Models;

namespace ProductionSystem.Client.Views;

public partial class GanttChartControl : UserControl
{
    public static readonly StyledProperty<IEnumerable?> BarsProperty =
        AvaloniaProperty.Register<GanttChartControl, IEnumerable?>(nameof(Bars));

    public IEnumerable? Bars
    {
        get => GetValue(BarsProperty);
        set => SetValue(BarsProperty, value);
    }

    public GanttChartControl()
    {
        InitializeComponent();
        BarsProperty.Changed.AddClassHandler<GanttChartControl>((c, _) => c.RenderChart());
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RenderChart();
    }

    private void RenderChart()
    {
        ChartCanvas.Children.Clear();
        if (Bars is not IEnumerable enumerable)
            return;

        var bars = enumerable.OfType<GanttBarDto>().ToList();
        if (bars.Count == 0)
            return;

        var equipmentRows = bars
            .Select(b => b.EquipmentMarking ?? $"[фон] {b.EquipmentTypeName}")
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var maxEnd = bars.Max(b => b.EndMinutes);
        if (maxEnd <= 0) maxEnd = 1;

        const double rowHeight = 36;
        const double leftMargin = 180;
        const double topMargin = 10;
        const double scale = 2.0;

        ChartCanvas.Width = leftMargin + maxEnd * scale + 40;
        ChartCanvas.Height = topMargin + equipmentRows.Count * rowHeight + 20;

        for (var i = 0; i < equipmentRows.Count; i++)
        {
            var eq = equipmentRows[i];
            var y = topMargin + i * rowHeight;

            var label = new TextBlock
            {
                Text = eq,
                Width = leftMargin - 8,
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap,
            };
            ChartCanvas.Children.Add(label);
            Canvas.SetLeft(label, 4);
            Canvas.SetTop(label, y + 4);

            foreach (var bar in bars.Where(b =>
                         (b.EquipmentMarking ?? $"[фон] {b.EquipmentTypeName}") == eq))
            {
                var rect = new Rectangle
                {
                    Width = Math.Max(4, (bar.EndMinutes - bar.StartMinutes) * scale),
                    Height = rowHeight - 10,
                    Fill = bar.IsBackground ? new SolidColorBrush(Color.Parse("#9E9E9E")) : new SolidColorBrush(Color.Parse("#4A90D9")),
                    Stroke = Brushes.DarkBlue,
                    StrokeThickness = 0.5,
                };
                var tip = new ToolTip
                {
                    Content = $"{bar.ProductName}: {bar.OperationName} ({bar.StartMinutes}-{bar.EndMinutes} мин)",
                };
                ToolTip.SetTip(rect, tip.Content);
                ChartCanvas.Children.Add(rect);
                Canvas.SetLeft(rect, leftMargin + bar.StartMinutes * scale);
                Canvas.SetTop(rect, y + 4);
            }
        }
    }
}
