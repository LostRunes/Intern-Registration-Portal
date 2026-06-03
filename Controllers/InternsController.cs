using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private bool InternExists(int id)
        {
            return _context.Interns.Any(e => e.Id == id);
        }
    }
}
