using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;

namespace ArqInf.Controllers
{
    public class StatisticsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> userManager;

        public StatisticsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index(string? id)
        {
            var usersContext = _context.Users;
            var assignmentContext = _context.UserAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
            var assignmentList = assignmentContext.Where(a => a.User.Id == id).Select(a => a.Assignment);
            int[] hoursMonth = new int[12];
            int[] assignmentsMonth = new int[12];
            double[] finishedAssign = new double[12];
            int[] auxAssign = new int[12];
            double[] hoursWorked = new double[12];

            for (int i = 1; i < 13; i++)
            {
                //assignmentsMonth[i-1] = assignmentContext.Where(a => a.Assignment.LimitDate.Month == i).Count();

                foreach (var item in assignmentList)
                {
                    if (item.LimitDate.Month == i && item.LimitDate.Year == DateTime.Now.Year)
                    {
                        hoursMonth[i-1] = ((int)(hoursMonth[i-1] + item.AssignedHours));
                        assignmentsMonth[i - 1] = assignmentsMonth[i - 1] + 1;
                        if (item.FinishDate.Year == DateTime.Now.Year)
                        {
                            auxAssign[i - 1] = auxAssign[i - 1] + 1;
                        }
                    }                
                }
                if (assignmentsMonth[i - 1] != 0)
                {
                    finishedAssign[i - 1] = Math.Round((auxAssign[i - 1]*1.0 / assignmentsMonth[i - 1]), 2)*100;
                }
                else
                {
                    finishedAssign[i - 1] = 0;
                }
                hoursWorked[i - 1] = Math.Round((hoursMonth[i - 1] / 160.0), 2) * 100;
            }
            
            return View(new TestModel()
            {
                hoursPerMonth = hoursMonth,
                assignmentsPerMonth = assignmentsMonth,
                finishedAssignments =  finishedAssign,
                hoursWorked = hoursWorked

            });
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult GlobalStatistics()
        {
            int[] projectsMonth = new int[12];
            int[] usersProject = new int[12];
            double[] moneySpent = new double[12];
            double[] assignCosts = new double[12];
            int[] doneAssignments = new int[12];
            int[] totalAssignments = new int[12];
            double[] percentageDone = new double[12];
            int[] projectsDone = new int[12];
            double[] percentageProjects = new double[12];
            for (int i = 1; i < 13; i++)
            {
                List<User> listUsers = new List<User>();
                var userAssignments = _context.UserAssignments.Include(a => a.User).Include(a => a.Assignment);
                foreach (var userAssign in userAssignments)
                {
                    if (!listUsers.Contains(userAssign.User) && (userAssign.Assignment.LimitDate.Month == i && userAssign.Assignment.LimitDate.Year == DateTime.Now.Year))
                    {
                        listUsers.Add(userAssign.User);
                    }
                }
                foreach (var item in _context.ProjectAssignments.Include(a => a.Project).Include(a => a.Assignment))
                {
                    if (item.Assignment.LimitDate.Month == i && item.Assignment.LimitDate.Year == DateTime.Now.Year)
                    {
                        if (item.Assignment.FinishDate.Year == DateTime.Now.Year)
                        {

                            doneAssignments[i - 1] = doneAssignments[i - 1] + 1;
                        }
                        totalAssignments[i - 1] = totalAssignments[i - 1] + 1;
                        assignCosts[i - 1] = (double)(assignCosts[i - 1] + item.Assignment.Budget);
                        
                    }
                    
                    //foreach (var item in _context.Project)
                    //{
                    //    List<User> listUsers = new List<User>();
                    //    var projectContext = _context.ProjectAssignments.Include(a => a.Project).Where(a => a.Project.Id == item.Id);
                    //    var assignmentList = projectContext.Include(a => a.Assignment).Where(a => a.Project.Id == item.Id).Select(a => a.Assignment);
                    //    foreach (var assignment in assignmentList)
                    //    {
                    //        if (assignment != null)
                    //        {
                    //            if (assignment.LimitDate.Month == i && assignment.LimitDate.Year == DateTime.Now.Year)
                    //            {
                    //                if (assignment.FinishDate.Year == DateTime.Now.Year)
                    //                {

                    //                    doneAssignments[i - 1] = doneAssignments[i - 1] + 1;
                    //                }
                    //                totalAssignments[i - 1] = totalAssignments[i - 1] + 1;
                    //                assignCosts[i - 1] = (double)(assignCosts[i - 1] + assignment.Budget);
                    //            }
                    //            var users = _context.UserAssignments.Include(a => a.User).Where(a => a.Assignment.Id == assignment.Id).Select(a => a.User);
                    //            foreach (var user in users)
                    //            {
                    //                if (!listUsers.Contains(user))
                    //                {
                    //                    listUsers.Add(user);
                    //                }
                    //            }
                    //        }

                    //    }
                    if (item.Project.LimitDate.Month == i && item.Project.LimitDate.Year == DateTime.Now.Year)
                    {
                        projectsMonth[i - 1] = projectsMonth[i - 1] + 1;
                        usersProject[i - 1] = listUsers.Count();
                        moneySpent[i - 1] = (double)(moneySpent[i - 1] + item.Project.Budget);
                        if (item.Project.FinishDate.Year == DateTime.Now.Year)
                        {
                            projectsDone[i - 1] = projectsDone[i - 1] + 1;
                        }
                    }   
                }
                if (totalAssignments[i - 1] != 0)
                {
                    percentageDone[i - 1] = Math.Round((doneAssignments[i - 1] * 1.0 / totalAssignments[i - 1]), 2) * 100;
                }
                else
                {
                    percentageDone[i - 1] = 0;
                }
                if (projectsMonth[i - 1] != 0)
                {
                    percentageProjects[i - 1] = Math.Round((projectsDone[i - 1] * 1.0 / projectsMonth[i - 1]), 2) * 100;
                }
                else
                {
                    percentageProjects[i - 1] = 0;
                }
            }

            return View(new TestModel()
            {
                projectsPerMonth = projectsMonth,
                usersInProject = usersProject,
                projectBudgets = moneySpent,
                AssignmentCosts = assignCosts,
                totalDoneAssignments = percentageDone,
                finishedProjects = percentageProjects
            });
        }


        public class TestModel
        {
            public int[] hoursPerMonth;
            public int[] assignmentsPerMonth;
            public double[] finishedAssignments;
            public double[] hoursWorked;
            public int[] projectsPerMonth;
            public int[] usersInProject;
            public double[] projectBudgets;
            public double[] AssignmentCosts;
            public double[] totalDoneAssignments;
            public double[] finishedProjects;
        }
    }


}
