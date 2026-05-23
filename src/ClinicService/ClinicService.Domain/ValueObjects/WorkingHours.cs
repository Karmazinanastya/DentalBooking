using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.ValueObjects;

public sealed class WorkingHours : ValueObject
{
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    private WorkingHours(TimeOnly start, TimeOnly end)
    {
        Start = start;
        End = end;
    }

    public static Result<WorkingHours> Create(TimeOnly start, TimeOnly end)
    {
        if (start >= end)
            return Result.Failure<WorkingHours>(
                Error.Validation(nameof(WorkingHours), "Start time must be before end time."));

        return new WorkingHours(start, end);
    }

    public bool Contains(TimeOnly time) => time >= Start && time < End;

    public bool Overlaps(WorkingHours other) => Start < other.End && End > other.Start;

    public int DurationInMinutes => (int)(End - Start).TotalMinutes;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
