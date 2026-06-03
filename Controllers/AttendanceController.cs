using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InternPortal.Data;
using InternPortal.Models;

namespace InternPortal.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            ViewBag.SelectedDate = targetDate.ToString("yyyy-MM-dd");

            var interns = await _context.Interns.ToListAsync();
            var attendances = await _context.Attendances
                .Where(a => a.Date.Date == targetDate.Date)
                .ToDictionaryAsync(a => a.InternId);

            var viewModel = interns.Select(i => new AttendanceRowViewModel
            {
                InternId = i.Id,
                InternName = i.Name,
                Department = i.Department,
                Branch = i.Branch,
                IsPresent = attendances.ContainsKey(i.Id) && attendances[i.Id].IsPresent
            }).ToList();

            return View(viewModel);
        }

        // POST: Attendance/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DateTime date, List<AttendanceRowViewModel> models)
        {
            foreach (var model in models)
            {
                var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.InternId == model.InternId && a.Date.Date == date.Date);

                if (attendance == null)
                {
                    _context.Attendances.Add(new Attendance
                    {
                        InternId = model.InternId,
                        Date = date.Date,
                        IsPresent = model.IsPresent
                    });
                }
                else
                {
                    attendance.IsPresent = model.IsPresent;
                    _context.Update(attendance);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Attendance successfully saved for " + date.ToString("yyyy-MM-dd");
            return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
        }
    }

    public class AttendanceRowViewModel
    {
        public int InternId { get; set; }
        public string InternName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
    }
}
