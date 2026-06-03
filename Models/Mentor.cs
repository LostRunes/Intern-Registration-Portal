using System.Collections.Generic;

namespace InternPortal.Models;

public class Mentor
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Intern> Interns { get; set; } = new List<Intern>();
}
