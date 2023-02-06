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
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;

        public ProjectController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var projectContext = _context.Project.Include(a => a.ProjectManager).Where(a => 0 == 0);
            return View(projectContext);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ProjectAssignments(int id)
        {
            var userAssignments = new Dictionary<int, string>();
            var userAssignContext = _context.UserAssignments.Include(a => a.Assignment).Include(a => a.User).Where(a => 0 == 0);
            foreach (var item in userAssignContext)
            {
                userAssignments.Add(item.Assignment.Id, item.User.Email);
            }
            ViewBag.UserAssignments = userAssignments;
            var assignmentContext = _context.ProjectAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
            var assignmentList = assignmentContext.Where(a => a.Project.Id == id).Select(a => a.Assignment);
            return View(assignmentList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectName,StartDate,LimitDate,Description,Budget")] Project project)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;

                if (DateTime.Compare(project.LimitDate, DateTime.Now) > 0 && DateTime.Compare(project.StartDate, DateTime.Now) >= 0)
                {
                    if (DateTime.Compare(project.LimitDate, project.StartDate) > 0)
                    {
                        project.ProjectManager = user;
                        _context.Add(project);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Data limite tem se ser maior que a de começo");
                    }
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Datas tem de ser maiores que a atual");
                }
                
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectName,StartDate,LimitDate,FinishDate,Description,Budget,MoneySpent")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (DateTime.Compare(project.LimitDate, DateTime.Now) > 0 && DateTime.Compare(project.StartDate, DateTime.Now) >= 0)
                {
                    if (DateTime.Compare(project.LimitDate, project.StartDate) > 0)
                    {
                        try
                        {
                            _context.Update(project);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ProjectExists(project.Id))
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
                        ModelState.AddModelError(string.Empty, "Data limite tem se ser maior que a de começo");
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Datas tem de ser maiores que a atual");
                }
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            var projectAssignments = _context.ProjectAssignments.Include(a => a.Project).Where(a => a.Project.Id == id);
            foreach (var item in projectAssignments)
            {
                if (item != null)
                {
                    if(item.Project.Id == id)
                    {
                        _context.ProjectAssignments.Remove(item);
                    }   
                }

            }
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAssignment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = _context.Project.FirstOrDefault(x => x.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            TempData["projectId"] = id;
            return View(_context.Assignment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("AddAssignment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignment(int id, string assignmentName)
        {
            if (ModelState.IsValid)
            {
                ProjectAssignments projectAssignments = new ProjectAssignments();
                Project project = _context.Project.FindAsync(id).Result;
                Assignment assignment = _context.Assignment.FirstOrDefault(x => x.AssignmentName == assignmentName);
                //var projectContext = _context.ProjectAssignments.Include(a => a.Project).Where(a => a.Project.Id == id);
                //var assignmentContext = _context.ProjectAssignments.Include(a => a.Assignment).Where(a => a.Assignment.AssignmentName == assignmentName);
                //projectAssignments.Project = projectContext.Select(a => a.Project).FirstOrDefault();;
                //projectAssignments.Assignment = assignmentContext.Select(a => a.Assignment).FirstOrDefault();
                bool assignmentAdded = false;
                foreach(var item in _context.ProjectAssignments.Include(a => a.Assignment).Where(a => a.Project.Id == id))
                {
                    if (item.Assignment != null)
                    {
                        if (item.Assignment.Id == assignment.Id)
                        {
                            assignmentAdded = true;
                        }
                    }
                    
                }
                if (!assignmentAdded)
                {
                    if (project.MoneySpent == null)
                    {
                        project.MoneySpent = 0.0;
                    }
                    if (_context.UserAssignments.FirstOrDefault(x => x.Assignment.Id == assignment.Id) != null)
                    {
                        if (project.Budget > (project.MoneySpent + assignment.Budget))
                        {
                            project.MoneySpent = Math.Round((double)(project.MoneySpent + assignment.Budget), 2, MidpointRounding.AwayFromZero);
                            projectAssignments.Project = project;
                            projectAssignments.Assignment = assignment;
                            await _context.ProjectAssignments.AddAsync(projectAssignments);
                            _context.Project.Update(project);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Dinheiro gasto não pode ultrapassar orçamento do projeto.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tarefa necessita de ser atribuida a um utilizador previamente.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tarefa já foi atribuida a este projeto.");
                }
            }
            TempData["projectId"] = id;
            return View(_context.Assignment.ToList());

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Finish(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Finish")]
        [ValidateAntiForgeryToken]
        public IActionResult Finish(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var d = _context.Project.Where(x => x.Id == id).Select(x => x.Id).Single();

                    var project = _context.Project.FirstOrDefault(p => p.Id == Convert.ToInt32(d));
                    bool projectDone = true;
                    foreach (var item in _context.ProjectAssignments.Include(a => a.Assignment).Where(a => 0 == 0))
                    {
                        if (item.Project.Id == project.Id)
                        {
                            if (item.Assignment.FinishDate.Year < 2022)
                            {
                                projectDone = false;
                            }
                        }
                    }
                    if (project.FinishDate.Year < 2022 && projectDone)
                    {
                        project.FinishDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else if(project.FinishDate.Year < 2022)
                    {
                        ModelState.AddModelError(string.Empty, "Existem ainda tarefas por terminar");
                    }
                    else 
                    {
                        ModelState.AddModelError(string.Empty, "Projeto já foi concluido.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(id))
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
