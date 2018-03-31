using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class YearController : Controller
    {
        private GaptDbContext _context;

        public YearController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ActionResult New(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            Year year = new Year();
            int? maxYearNo = _context.Years.Where(m => m.TentativePsId == pr.TentativePsId).Max(u => (int?)u.YearNo);
            int? yearNo; ;
            if (maxYearNo == null)
            {
                yearNo = 1;
            }
            else {
                yearNo = maxYearNo + 1;
            }
            
            year.YearNo = yearNo;

            var allUnits = _context.Database.SqlQuery<Ref_Unit>("Select * from Ref_Unit where DepartmentId In(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = " + proposal.GeneralId + ")").ToList();
            var viewModel = new YearFormViewModel
            {
                Proposal = proposal,
                Year = year,
                AllUnits = allUnits
            };
            return View("Form", viewModel);
        }

        [Route("Reviewer/Edit/{pid}/{yid}")]
        public ActionResult Edit(int pid, int yid)
        {

            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == pid);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var year = _context.Years.SingleOrDefault(m => m.Id == yid);
            if (year == null)
            {
                return HttpNotFound();
            }

            //checks if there exists a relationship
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            var yearIds = _context.Database.SqlQuery<int>("Select Id From dbo.Year Where TentativePsId = " + pr.TentativePsId).ToList();
            if (yearIds.Contains(year.Id))
            {
                string selectedQuery = "SELECT * FROM Year_Unit WHERE YearId = " + year.Id;
                var selectedUnits = _context.Database.SqlQuery<Year_Unit>(selectedQuery).ToList();
                string unselectedQuery = "SELECT * FROM Ref_Unit WHERE Id not in (SELECT UnitId FROM Year_Unit WHERE YearId = "+year.Id+") AND DepartmentId IN(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = "+ proposal.GeneralId+ ")";
                var unselectedUnits = _context.Database.SqlQuery<Ref_Unit>(unselectedQuery).ToList();
                var viewModel = new YearFormViewModel
                {
                    Year = year,
                    Proposal = proposal,
                    SelectedUnits = selectedUnits,
                    UnselectedUnits = unselectedUnits
                };
                return View("Form", viewModel);
            }
            else
            {
                return HttpNotFound();
            }


        }

        // GET: Year
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(YearFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            var year = vm.Year;
            if (!ModelState.IsValid)
            {
                return View("Form", year);
            }

            if (year.Id == 0)
            {
                _context.Years.Add(year);
                var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.Id);
                year.TentativeP = pr.TentativeP;
                year.TentativePsId = pr.TentativePsId;
                
            }
            else
            {
                var yearInDb = _context.Years.SingleOrDefault(m => m.Id == year.Id);
                yearInDb.TotalEcts = year.TotalEcts;
                // remove exsting Year_Units where TentativePsId == tentative.Id
                string queryYu = "DELETE FROM dbo.Year_Unit WHERE YearId = " + year.Id;
                _context.Database.ExecuteSqlCommand(queryYu);

                
            }
            foreach (string name in Request.Form.AllKeys)
            {

                try
                {
                    int unitId = Convert.ToInt32(name);
                    Year_Unit yu = new Year_Unit();
                    yu.YearId = year.Id;
                    yu.Year = _context.Years.SingleOrDefault(m => m.Id == year.Id);
                    yu.UnitId = unitId;
                    yu.Coe = Convert.ToInt32(Request["coe_" + name]);
                    if (Request["credits_" + name] != "")
                    {
                        yu.Ects = Convert.ToInt32(Request["credits_" + name]);
                    }
                    if (Request["lecturer_" + name] != "")
                    {
                        yu.Lecturer = Request["lecturer_" + name];
                    }
                    yu.Period = Convert.ToInt32(Request["period_" + name]);
                    if (proposal.GetGeneral().LevelId == 1)
                    {
                        // if UG, check if compensating was selected
                        if (Request["comp_" + name] == "on")
                        {
                            yu.Compensating = 1;
                            if (Request["reason_" + name] != "")
                            {
                                yu.CompensatingReason = Request["reason_" + name];
                            }
                        }
                        else
                        {
                            yu.Compensating = 0;
                        }
                    }
                    else
                    {
                        yu.Compensating = 0;
                    }
                    _context.Year_Unit.Add(yu);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "TentativePs", new { id = proposal.Id });

        }
    }
}