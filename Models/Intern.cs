using System.ComponentModel.DataAnnotations;

namespace InternPortal.Models;

public class Intern
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    public int MentorId { get; set; }

    public Mentor? Mentor { get; set; }

    public string Department { get; set; } = string.Empty;

    public string Branch { get; set; } = string.Empty;

    public int DurationMonths { get; set; }
}