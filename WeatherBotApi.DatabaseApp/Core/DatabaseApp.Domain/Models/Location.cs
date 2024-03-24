namespace DatabaseApp.Domain.Models;

public record Location
{
    public string Value { get; private set; }

    public Location(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Location cannot be null or whitespace.", nameof(value));
        }

        Value = value;
    }

    public void Update(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            throw new ArgumentException("New location cannot be null or whitespace", nameof(newValue));
        }

        Value = newValue;
    }
}