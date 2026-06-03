using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InternPortal.Data;
using InternPortal.Models;

namespace InternPortal.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalInterns = await _context.Interns.CountAsync();
        
        var today = DateTime.Today;
        ViewBag.PresentToday = await _context.Attendances
            .CountAsync(a => a.Date.Date == today && a.IsPresent);

        ViewBag.TotalMentors = await _context.Mentors.CountAsync();

        var totalAttendanceRecords = await _context.Attendances
            .CountAsync(a => a.Date.Date == today);
        ViewBag.AttendanceRate = totalAttendanceRecords > 0 
            ? (int)Math.Round((double)ViewBag.PresentToday / totalAttendanceRecords * 100) 
            : 0;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
