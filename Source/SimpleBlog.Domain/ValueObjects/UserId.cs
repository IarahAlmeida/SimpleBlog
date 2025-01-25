using SimpleBlog.Domain.Common;

namespace SimpleBlog.Domain.ValueObjects;

public class UserId : ValueObject
{
    public Guid Id { get; private set; }

    protected UserId()
    { }

    public UserId(Guid id)
    {
        Id = id;
    }

    public static Result<UserId> Create(string? id = null)
    {
        if (id == null)
        {
            var newGuid = Guid.NewGuid();
            return Result.Success(new UserId(newGuid));
        }

        if (Guid.TryParse(id, out Guid guid))
        {
            return Result.Success(new UserId(guid));
        }

        return Result.Failure<UserId>("Invalid GUID format");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}
