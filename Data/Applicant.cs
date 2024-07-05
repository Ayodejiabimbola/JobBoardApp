using JobBoard.Data.Enum;

namespace JobBoard.Data;
public class Applicant : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateTime DOB { get; set; }
    public Gender Gender { get; set; } = default!;
    public int StateId { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = default!;
    public string? CVPath { get; set; } 
    public string? CoverLetterPath { get; set; }
    public string UserId { get; internal set; } = default!;
}