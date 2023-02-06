#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ArqInf.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;

        public AssignmentController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult Index()
        {
            var userAssignments = new Dictionary<int,string>();
            var userAssignContext = _context.UserAssignments.Include(a => a.Assignment).Include(a => a.User).Where(a => 0 == 0);
            foreach (var item in userAssignContext)
            {
                userAssignments.Add(item.Assignment.Id, item.User.Email);
            }
            ViewBag.UserAssignments = userAssignments;
            var assignmentContext = _context.Assignment.Include(a => a.Assigner).Where(a => 0 == 0);
            return View(assignmentContext);
        }

        // GET: Assignment
        public IActionResult MyAssignments(string id)
        {
            var assignmentContext = _context.UserAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
            var assignmentList = assignmentContext.Where(a => a.User.Id == id).Select(a => a.Assignment);
            return View(assignmentList);
        }


        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult UserAssignments(string id)
        {
            //assignmentContext.Where(a => a.Id == id);
            var assignmentContext = _context.UserAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
            var assignmentList = assignmentContext.Where(a => a.User.Id == id).Select(a => a.Assignment);
            return View(assignmentList);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(m => m.Id == id);
            var userAssignContext = _context.UserAssignments.Include(a => a.Assignment).Include(a => a.User).Where(a => 0 == 0);
            ViewBag.User = userAssignContext.FirstOrDefault(a => a.Id == assignment.Id).User.Email;
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssignmentName, LimitDate, AssignedHours,Description")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                if (DateTime.Compare(assignment.LimitDate, DateTime.Now) > 0)
                {
                    assignment.Assigner = user;
                    _context.Add(assignment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Data limite tem de ser maior que a atual");
                }   
            }
            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentName,FinishDate,Description")] Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (DateTime.Compare(assignment.LimitDate, DateTime.Now) > 0)
                {
                    try
                    {
                        _context.Update(assignment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AssignmentExists(assignment.Id))
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Data limite tem de ser maior que a atual");
                }
            }
            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignment.FindAsync(id);
            var userAssignments = _context.UserAssignments.FirstOrDefaultAsync(a => a.Assignment.Id == id).Result;
            var projectAssignments = _context.ProjectAssignments.Include(a => a.Assignment).Include(a => a.Project).Where(a => a.Assignment.Id == id);
            foreach (var item in projectAssignments)
            {
                if (item != null)
                {
                    if(item.Assignment.Id == id && item.Assignment.Budget > 0)
                    {
                        item.Project.MoneySpent = item.Project.MoneySpent - item.Assignment.Budget;
                        _context.ProjectAssignments.Remove(item);
                    }   
                }

            }
            if (userAssignments != null)
            {
                _context.UserAssignments.Remove(userAssignments);
            }

            _context.Assignment.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignment.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Finish(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("Finish")]
        [ValidateAntiForgeryToken]
        public IActionResult Finish(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var d = _context.Assignment.Where(x => x.Id == id).Select(x => x.Id).Single();

                    var assignment = _context.Assignment.FirstOrDefault(p => p.Id == Convert.ToInt32(d));
                    var associatedUser = _context.UserAssignments.FirstOrDefault(a => a.Assignment.Id == assignment.Id).User;
                    if (assignment.FinishDate.Year < 2022 && associatedUser == null)
                    {
                        assignment.FinishDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Não existe utilizador atribuido à tarefa");
                    }       
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(id))
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
            return View();
        }

    }
}
