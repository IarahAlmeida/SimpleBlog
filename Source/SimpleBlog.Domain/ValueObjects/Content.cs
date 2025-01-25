using SimpleBlog.Domain.Common;

namespace SimpleBlog.Domain.ValueObjects;

public class Content : ValueObject
{
    public string Text { get; private set; }

    public Content(string text)
    {
        Text = text;
    }

    public static Result<Content> Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<Content>("Content cannot be empty");

        if (text.Length > 5000)
            return Result.Failure<Content>("Content cannot exceed 5000 characters");

        return Result.Success(new Content(text));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Text;
    }
}
