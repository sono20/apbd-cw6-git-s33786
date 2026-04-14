using System.ComponentModel.DataAnnotations;

namespace WebApplication1;

public class Reservation : IValidatableObject
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    [Required] public string OrganizerName { get; set; } = string.Empty;
    [Required] public string Topic { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.",
                new[] { nameof(EndTime) }
            );
        }
    }
}