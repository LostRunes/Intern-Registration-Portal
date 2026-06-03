using System;

namespace InternPortal.Models;

public class Attendance
{
    public int Id { get; set; }

    public int InternId { get; set; }

    public Intern? Intern { get; set; }

    public DateTime Date { get; set; }

    public bool IsPresent { get; set; }
}
