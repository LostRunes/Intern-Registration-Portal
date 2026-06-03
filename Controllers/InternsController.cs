using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternPortal.Data;
using InternPortal.Models;

namespace InternPortal.Controllers
{
    public class InternsController : Controller
    {
        private readonly AppDbContext _context;

        public InternsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Interns
        public async Task<IActionResult> Index(string searchString, int? mentorId, string department, string branch)
        {
            var internsQuery = _context.Interns.Include(i => i.Mentor).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                internsQuery = internsQuery.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            if (mentorId.HasValue)
            {
                internsQuery = internsQuery.Where(i => i.MentorId == mentorId.Value);
            }

            if (!string.IsNullOrEmpty(department))
            {
                internsQuery = internsQuery.Where(i => i.Department == department);
            }

            if (!string.IsNullOrEmpty(branch))
            {
                internsQuery = internsQuery.Where(i => i.Branch == branch);
            }

            ViewData["MentorId"] = new SelectList(_context.Mentors, "Id", "Name", mentorId);
            ViewData["Departments"] = new SelectList(await _context.Interns.Select(i => i.Department).Distinct().ToListAsync(), department);
            ViewData["Branches"] = new SelectList(await _context.Interns.Select(i => i.Branch).Distinct().ToListAsync(), branch);

            return View(await internsQuery.ToListAsync());
        }

        // GET: Interns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intern = await _context.Interns
                .Include(i => i.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intern == null)
            {
                return NotFound();
            }

            return View(intern);
        }

        // GET: Interns/Create
        public IActionResult Create()
        {
            ViewData["MentorId"] = new SelectList(_context.Mentors, "Id", "Name");
            return View();
        }

        // POST: Interns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Contact,MentorId,Department,Branch,DurationMonths")] Intern intern)
        {
            if (ModelState.IsValid)
            {
                _context.Add(intern);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MentorId"] = new SelectList(_context.Mentors, "Id", "Name", intern.MentorId);
            return View(intern);
        }

        // GET: Interns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intern = await _context.Interns.FindAsync(id);
            if (intern == null)
            {
                return NotFound();
            }
            ViewData["MentorId"] = new SelectList(_context.Mentors, "Id", "Name", intern.MentorId);
            return View(intern);
        }

        // POST: Interns/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Contact,MentorId,Department,Branch,DurationMonths")] Intern intern)
        {
            if (id != intern.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(intern);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InternExists(intern.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MentorId"] = new SelectList(_context.Mentors, "Id", "Name", intern.MentorId);
            return View(intern);
        }

        // GET: Interns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intern = await _context.Interns
                .Include(i => i.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intern == null)
            {
                return NotFound();
            }

            return View(intern);
        }

        // POST: Interns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var intern = await _context.Interns.FindAsync(id);
            if (intern != null)
            {
                _context.Interns.Remove(intern);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Interns/UploadCsv
        public IActionResult UploadCsv()
        {
            return View();
        }

        // POST: Interns/UploadCsv
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid CSV file.";
                return View();
            }

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(headerLine))
                {
                    TempData["ErrorMessage"] = "The uploaded file is empty.";
                    return View();
                }

                var headers = headerLine.Split(',').Select(h => h.Trim().ToUpper()).ToList();
                int nameIndex = headers.IndexOf("NAME");
                int contactIndex = headers.IndexOf("CONTACT");
                int deptIndex = headers.IndexOf("DEPARTMENT");
                int branchIndex = headers.IndexOf("BRANCH");
                int durationIndex = headers.IndexOf("DURATIONMONTHS");
                int mentorNameIndex = headers.IndexOf("MENTORNAME");
                int mentorIdIndex = headers.IndexOf("MENTORID");

                if (nameIndex == -1)
                {
                    TempData["ErrorMessage"] = "Required column 'Name' is missing in CSV header.";
                    return View();
                }

                var mentors = await _context.Mentors.ToListAsync();
                var defaultMentor = mentors.FirstOrDefault();
                if (defaultMentor == null)
                {
                    TempData["ErrorMessage"] = "No mentors found in the database. Please seed mentors first.";
                    return View();
                }

                var interns = new List<Intern>();
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',').Select(v => v.Trim()).ToArray();
                    if (values.Length <= nameIndex || string.IsNullOrEmpty(values[nameIndex]))
                    {
                        continue;
                    }

                    string name = values[nameIndex];
                    string contact = contactIndex != -1 && values.Length > contactIndex ? values[contactIndex] : "";
                    string dept = deptIndex != -1 && values.Length > deptIndex ? values[deptIndex] : "";
                    string branch = branchIndex != -1 && values.Length > branchIndex ? values[branchIndex] : "";
                    
                    int duration = 3;
                    if (durationIndex != -1 && values.Length > durationIndex)
                    {
                        int.TryParse(values[durationIndex], out duration);
                    }

                    int mentorId = defaultMentor.Id;
                    if (mentorIdIndex != -1 && values.Length > mentorIdIndex && int.TryParse(values[mentorIdIndex], out int mId))
                    {
                        if (mentors.Any(m => m.Id == mId))
                        {
                            mentorId = mId;
                        }
                    }
                    else if (mentorNameIndex != -1 && values.Length > mentorNameIndex && !string.IsNullOrEmpty(values[mentorNameIndex]))
                    {
                        var mName = values[mentorNameIndex];
                        var matchedMentor = mentors.FirstOrDefault(m => m.Name.Equals(mName, StringComparison.OrdinalIgnoreCase));
                        if (matchedMentor != null)
                        {
                            mentorId = matchedMentor.Id;
                        }
                    }

                    interns.Add(new Intern
                    {
                        Name = name,
                        Contact = contact,
                        Department = dept,
                        Branch = branch,
                        DurationMonths = duration,
                        MentorId = mentorId
                    });
                }

                if (interns.Any())
                {
                    _context.Interns.AddRange(interns);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Successfully imported {interns.Count} interns!";
                }
                else
                {
                    TempData["ErrorMessage"] = "No valid records were found in the CSV file.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to parse CSV file: {ex.Message}";
            }

            return View();
        }

        private bool InternExists(int id)
        {
            return _context.Interns.Any(e => e.Id == id);
        }
    }
}
