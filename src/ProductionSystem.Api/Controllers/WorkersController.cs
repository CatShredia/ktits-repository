using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Director)]
public class WorkersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WorkerListItemDto>>> List(CancellationToken ct)
    {
        var workers = await db.Workers.AsNoTracking()
            .Include(w => w.WorkerOperations)
            .ThenInclude(wo => wo.Operation)
            .OrderBy(w => w.LastName)
            .ToListAsync(ct);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = workers.Select(w => new WorkerListItemDto
        {
            Id = w.Id,
            LastName = w.LastName,
            Age = AgeYears(w.BirthDate, today),
            OperationsCommaSeparated = string.Join(", ",
                w.WorkerOperations.Select(x => x.Operation.Name).OrderBy(n => n)),
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkerDetailDto>> Get(int id, CancellationToken ct)
    {
        var w = await db.Workers.AsNoTracking()
            .Include(x => x.WorkerOperations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (w is null)
            return NotFound();

        return Ok(new WorkerDetailDto
        {
            Id = w.Id,
            LastName = w.LastName,
            FirstMiddleName = w.FirstMiddleName,
            BirthDate = w.BirthDate,
            HomeAddress = w.HomeAddress,
            Education = w.Education,
            Qualification = w.Qualification,
            OperationIds = w.WorkerOperations.Select(o => o.OperationId).ToList(),
        });
    }

    [HttpPost]
    public async Task<ActionResult<WorkerDetailDto>> Create([FromBody] WorkerCreateUpdateRequest req, CancellationToken ct)
    {
        var worker = new Worker
        {
            LastName = req.LastName.Trim(),
            FirstMiddleName = req.FirstMiddleName.Trim(),
            BirthDate = req.BirthDate,
            HomeAddress = req.HomeAddress.Trim(),
            Education = req.Education.Trim(),
            Qualification = req.Qualification.Trim(),
        };
        db.Workers.Add(worker);
        await db.SaveChangesAsync(ct);

        await SyncOperations(worker.Id, req.OperationIds, ct);
        return CreatedAtAction(nameof(Get), new { id = worker.Id }, await BuildDetail(worker.Id, ct));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkerCreateUpdateRequest req, CancellationToken ct)
    {
        var worker = await db.Workers.Include(w => w.WorkerOperations).FirstOrDefaultAsync(w => w.Id == id, ct);
        if (worker is null)
            return NotFound();

        worker.LastName = req.LastName.Trim();
        worker.FirstMiddleName = req.FirstMiddleName.Trim();
        worker.BirthDate = req.BirthDate;
        worker.HomeAddress = req.HomeAddress.Trim();
        worker.Education = req.Education.Trim();
        worker.Qualification = req.Qualification.Trim();

        db.WorkerOperations.RemoveRange(worker.WorkerOperations);
        await db.SaveChangesAsync(ct);

        await SyncOperations(worker.Id, req.OperationIds, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var worker = await db.Workers.FirstOrDefaultAsync(w => w.Id == id, ct);
        if (worker is null)
            return NotFound();

        db.Workers.Remove(worker);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task SyncOperations(int workerId, List<int> operationIds, CancellationToken ct)
    {
        var ids = operationIds.Distinct().ToList();
        if (ids.Count == 0)
            return;

        var existing = await db.ProductionOperations.Where(o => ids.Contains(o.Id)).Select(o => o.Id).ToListAsync(ct);
        foreach (var opId in existing)
            db.WorkerOperations.Add(new WorkerOperation { WorkerId = workerId, OperationId = opId });

        await db.SaveChangesAsync(ct);
    }

    private async Task<WorkerDetailDto> BuildDetail(int id, CancellationToken ct)
    {
        var w = await db.Workers.AsNoTracking()
            .Include(x => x.WorkerOperations)
            .FirstAsync(x => x.Id == id, ct);

        return new WorkerDetailDto
        {
            Id = w.Id,
            LastName = w.LastName,
            FirstMiddleName = w.FirstMiddleName,
            BirthDate = w.BirthDate,
            HomeAddress = w.HomeAddress,
            Education = w.Education,
            Qualification = w.Qualification,
            OperationIds = w.WorkerOperations.Select(o => o.OperationId).ToList(),
        };
    }

    private static int AgeYears(DateOnly birth, DateOnly today)
    {
        var age = today.Year - birth.Year;
        if (today < birth.AddYears(age))
            age--;
        return Math.Max(0, age);
    }
}
