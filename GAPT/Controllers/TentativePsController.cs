using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class TentativePsController : Controller
    {
        private GaptDbContext _context;

        public TentativePsController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: TentativePs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            ProgrammeRationale pr = null;
            if (proposal.ProgrammeRationaleId != null)
            {
                pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            }
            if (pr.TentativePsId != null)
            {
                return HttpNotFound();
            }
            
            var allUnits = _context.Database.SqlQuery<Ref_Unit>("Select * from Ref_Unit where DepartmentId In(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = " + proposal.GeneralId  + ")").ToList();
            //var departmentIds = _context.Database.SqlQuery<int>("Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = " + proposal.GeneralId).ToList();
            var viewModel = new TentativePsFormViewModel
            {
                Proposal = proposal,
                AllUnits = allUnits,
                TentativePs = new TentativeP(),

            };
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(TentativePsFormViewModel vm)
        {
            var tentative = vm.TentativePs;
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            if (!ModelState.IsValid)
            {
                return View("Form", tentative);
            }

            if (tentative.Id == 0)
            {
                _context.TentativePs.Add(tentative);
                var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.Id);
                pr.TentativePsId = tentative.Id;
                pr.TentativeP = tentative;
                var addedYears = new List<int>();

                foreach (string name in Request.Form.AllKeys)
                {
                    try
                    {
                        int n = Convert.ToInt32(name);
                        int yearNo = Convert.ToInt32(Request["year_" + name]);
                        if (!addedYears.Contains(yearNo))
                        {
                            addedYears.Add(yearNo);
                            Year year = new Year();
                            year.TentativePsId = tentative.Id;
                            year.TentativeP = tentative;
                            year.YearNo = yearNo;
                            year.TotalEcts = 0;
                            _context.Years.Add(year);
                            _context.SaveChanges();
                        }
                        Year yearInDb = _context.Years.SingleOrDefault(m => m.TentativePsId == tentative.Id && m.YearNo == yearNo);
                        Year_Unit yu = new Year_Unit();
                        yu.YearId = yearInDb.Id;
                        yu.Year = _context.Years.SingleOrDefault(m => m.Id == yearInDb.Id);
                        yu.UnitId = n;
                        yu.Coe = Convert.ToInt32(Request["coe_" + name]);
                        yu.Ects = Convert.ToInt32(Request["credits_" + name]);
                        yu.Period = Convert.ToInt32(Request["period_" + name]);
                        yu.Lecturer = Request["lecturer_" + name];
                        _context.Year_Unit.Add(yu);
                        _context.SaveChanges();
                        //return Content(name + " " + Request["lecturer_" + name] + " " + Request["credits_" + name]);
                    }
                    catch (Exception e)
                    {

                    }
                                       

                }
                
            }
            else
            {
                //save all yearno, ects pairs to dictionary
                SortedDictionary<int, int> pairs = new SortedDictionary<int, int>();
                string query = "Select YearNo From dbo.Year Where TentativePsId = " + tentative.Id;
                var yearNos = _context.Database.SqlQuery<int>(query).ToList();
                foreach (int yearNo in yearNos)
                {
                    string q = "Select TotalEcts From dbo.Year Where YearNo = " + yearNo + " and TentativePsId = " + tentative.Id;
                    int ects = _context.Database.SqlQuery<int>(q).Single();
                    pairs.Add(yearNo, ects);
                }

                // remove exsting Year_Units and Years where TentativePsId == tentative.Id
                string queryYu = "DELETE FROM dbo.Year_Unit WHERE YearId in (Select Id From dbo.Year Where TentativePsId = "+ tentative.Id+"); ";
                string queryY = "DELETE FROM dbo.Year Where TentativePsId = " + tentative.Id;
                _context.Database.ExecuteSqlCommand(queryYu);
                _context.Database.ExecuteSqlCommand(queryY);

                var addedYears = new List<int>();

                foreach (string name in Request.Form.AllKeys)
                {
                    try
                    {
                        int n = Convert.ToInt32(name);
                        int yearNo = Convert.ToInt32(Request["year_" + name]);
                        if (!addedYears.Contains(yearNo))
                        {
                            addedYears.Add(yearNo);
                            Year year = new Year();
                            year.TentativePsId = tentative.Id;
                            year.YearNo = yearNo;
                            if (pairs.ContainsKey(yearNo))
                            {
                                year.TotalEcts = pairs[yearNo];
                            }
                            else {
                                year.TotalEcts = 0;
                            }
                            
                            _context.Years.Add(year);
                            _context.SaveChanges();
                        }
                        Year yearInDb = _context.Years.SingleOrDefault(m => m.TentativePsId == tentative.Id && m.YearNo == yearNo);
                        Year_Unit yu = new Year_Unit();
                        yu.YearId = yearInDb.Id;
                        yu.Year = _context.Years.SingleOrDefault(m => m.Id == yearInDb.Id);
                        yu.UnitId = n;
                        yu.Coe = Convert.ToInt32(Request["coe_" + name]);
                        yu.Ects = Convert.ToInt32(Request["credits_" + name]);
                        yu.Period = Convert.ToInt32(Request["period_" + name]);
                        yu.Lecturer = Request["lecturer_" + name];
                        _context.Year_Unit.Add(yu);
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }


                }
            }

            _context.SaveChanges();
            var jump = Request["jump"];
            if (jump == "-1")
            {
                //go previous
                return RedirectToAction("Edit", "ProgrammeStudy", new { id = proposal.Id });
            }
            else if (jump == "0")
            {
                // Save pressed -> return form
                return RedirectToAction("Edit", "TentativePs", new { id = proposal.Id });
            }
            else if (jump == "1")
            {
                // Next pressed -> return next page
                return RedirectToAction("YearTotals", "TentativePs", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Index", "Proposal");
            }
            
            

        }

        public ActionResult Edit(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (proposal.ProgrammeRationaleId == null)
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var tentative = _context.TentativePs.SingleOrDefault(c => c.Id == pr.TentativePsId);
            if (tentative == null)
            {
                return HttpNotFound();
            }
            string selectedSubQuery = "SELECT UnitId FROM Year_Unit JOIN Year ON dbo.Year.Id = dbo.Year_Unit.YearId WHERE dbo.Year.TentativePsId = " + tentative.Id;
            //string selectedQuery = "SELECT * FROM Ref_Unit WHERE Id in (" + selectedSubQuery + ")";
            //var selectedUnits = _context.Database.SqlQuery<Ref_Unit>(selectedQuery).ToList();
            string selectedQuery = "SELECT * FROM Year_Unit WHERE UnitId in (" + selectedSubQuery + ") order by YearId";
            var selectedUnits = _context.Database.SqlQuery<Year_Unit>(selectedQuery).ToList();


            string unselectedQuery = "SELECT * FROM Ref_Unit WHERE Id not in (" + selectedSubQuery + ") AND DepartmentId IN(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = "+ proposal.GeneralId+")";
            var unselectedUnits = _context.Database.SqlQuery<Ref_Unit>(unselectedQuery).ToList();

            
            var viewModel = new TentativePsFormViewModel
            {
                Proposal = proposal,
                TentativePs = tentative,
                SelectedUnits = selectedUnits,
                UnselectedUnits = unselectedUnits
            };
            return View("Form", viewModel);
        }

        public ActionResult YearTotals(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var tentative = _context.TentativePs.SingleOrDefault(c => c.Id == pr.TentativePsId);

            
            if (tentative == null)
            {
                return HttpNotFound();
            }
            SortedDictionary<int, int> pairs = new SortedDictionary<int, int>();
            string query = "Select YearNo From dbo.Year Where TentativePsId = " + tentative.Id;
            var yearNos = _context.Database.SqlQuery<int>(query).ToList();
            foreach (int yearNo in yearNos) {
                string q = "Select TotalEcts From dbo.Year Where YearNo = " + yearNo+ " and TentativePsId = " + tentative.Id;
                int ects = _context.Database.SqlQuery<int>(q).Single();
                pairs.Add(yearNo, ects);
            }
           

            var viewModel = new YearTotalsFormViewModel
            {
                Pairs = pairs,
                Proposal = proposal,
                TentativePs = tentative,
            };
            return View("YearTotalsForm", viewModel);
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (pr.TentativePsId == null)
            {
                return RedirectToAction("New", "TentativePs", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Edit", "TentativePs", new { id = proposal.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCredits(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var tentative = _context.TentativePs.SingleOrDefault(c => c.Id == pr.TentativePsId);


            if (tentative == null)
            {
                return HttpNotFound();
            }
            foreach (string name in Request.Form.AllKeys)
            {
                try
                {
                    int yearNo = Convert.ToInt32(name);
                    int totalEcts = Convert.ToInt32(Request[name]);
                    var year = _context.Years.SingleOrDefault(m => m.TentativePsId == tentative.Id && m.YearNo == yearNo);
                    year.TotalEcts = totalEcts;
                    _context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
            var jump = Request["jump"];
            if (jump == "-1")
            {
                //go previous
                return RedirectToAction("Edit", "TentativePs", new { id = proposal.Id });
            }
            else if (jump == "0")
            {
                // Save pressed -> return form
                return RedirectToAction("YearTotals", "TentativePs", new { id = proposal.Id });
            }
            else if (jump == "1")
            {
                // Next pressed -> return next page
                return RedirectToAction("Index", "Proposal");
            }
            else
            {
                return RedirectToAction("Index", "Proposal");
            }
        }
    }

    
}