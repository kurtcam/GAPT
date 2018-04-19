using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class GeneralController : Controller
    {
        private GaptDbContext _context;

        public GeneralController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: General
        public ActionResult Index()
        {
            return View();
        }

        [Route("General/New")]
        public ActionResult New() {
            var viewModel = GetFormViewModel();
            return View("Form", viewModel);
        }

        private GeneralFormViewModel GetFormViewModel() {
            var levels = _context.Ref_Level.ToList();
            var faculties = _context.Ref_Faculty.ToList();
            var deliveries = _context.Ref_Delivery.ToList();

            var departments = _context.Ref_Department.ToList();
            List<SelectListItem> listSelectListInitDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListServDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListCollabDepts = new List<SelectListItem>();

            foreach (Ref_Department dept in departments)
            {
                SelectListItem selectList = new SelectListItem()
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString(),
                };
                listSelectListInitDepts.Add(selectList);
                listSelectListServDepts.Add(selectList);
                listSelectListCollabDepts.Add(selectList);
            }

            var viewModel = new GeneralFormViewModel
            {
                Levels = levels,
                Faculties = faculties,
                Deliveries = deliveries,
                InitDepts = listSelectListInitDepts,
                ServDepts = listSelectListServDepts,
                CollabDepts = listSelectListCollabDepts,
            };
            return viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(GeneralFormViewModel vm)
        {
            
            var general = vm.General;
            if (!ModelState.IsValid)
            {
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        return Content(error.ErrorMessage);
                //    }
                //}
                return View("Form", general);
            }

            if (general.Id == 0)
            {
                _context.Generals.Add(general);
                var proposal = new Proposal();
                proposal.GeneralId = general.Id;
                proposal.CreatedOn = DateTime.Now;
                proposal.CreatedBy = "Andrea Naudi";
                _context.Proposals.Add(proposal);
            }
            else
            {
                var proposal = _context.Proposals.SingleOrDefault(m => m.GeneralId == general.Id);
                if (proposal == null) {
                    return HttpNotFound();
                }
                if (proposal.Submitted) {
                    return Content("Proposal already submitted");
                }
                var generalInDb = _context.Generals.SingleOrDefault(m => m.Id == general.Id);
                generalInDb.Title = general.Title;
                generalInDb.LevelId = general.LevelId;
                generalInDb.Ref_Level = _context.Ref_Level.SingleOrDefault(m => m.Id == general.LevelId);
                generalInDb.AreasStudy = general.AreasStudy;
                generalInDb.FacultyId = general.FacultyId;
                generalInDb.Ref_Faculty = general.Ref_Faculty;
                generalInDb.DeliveryId = general.DeliveryId;
                generalInDb.Ref_Delivery = _context.Ref_Delivery.SingleOrDefault(m => m.Id == general.DeliveryId);
                generalInDb.DurationSemesters = general.DurationSemesters;
                generalInDb.FirstDateIntake = general.FirstDateIntake;
                generalInDb.ExpectedStudents = general.ExpectedStudents;
                generalInDb.MaxStudents = general.MaxStudents;
                generalInDb.CappingReason = general.CappingReason;

                //remove departments, to be added again later
                var toRemove = _context.Department_General.Where(m => m.GeneralId == general.Id);
                _context.Department_General.RemoveRange(toRemove);
            }

            _context.SaveChanges();
            if (vm.SelectedInitDepts != null)
            {
                foreach (string s in vm.SelectedInitDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 1);
                    _context.Department_General.Add(dg);
                    
                }
            }
            if (vm.SelectedCollabDepts != null)
            {
                foreach (string s in vm.SelectedCollabDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 2);
                    _context.Department_General.Add(dg);
                }
            }
            if (vm.SelectedServDepts != null)
            {
                foreach (string s in vm.SelectedServDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 3);
                    _context.Department_General.Add(dg);
                }
            }
            _context.SaveChanges();
            var jump = Request["jump"];
            var prop = general.GetProposal();
            switch (jump) {
                case "0": {
                        // Save pressed -> return form
                        return RedirectToAction("Edit", "General", new { id = general.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = prop.Id });
                    }
                case "A":
                    {
                        // A pressed -> go to Section A
                        return RedirectToAction("Edit", "General", new { id = general.Id });
                    }
                case "B":
                    {
                        // B pressed -> go to Section B
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = prop.Id });
                    }
                case "C":
                    {
                        // C pressed -> go to Section C
                        return RedirectToAction("Jump", "ExternalReview", new { id = prop.Id });
                    }
                case "D":
                    {
                        // D pressed -> go to Section D
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = prop.Id });
                    }
                default:
                    {
                        return RedirectToAction("Index", "Proposal");
                    }
            }
            
        }

        public ActionResult Edit(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            if (proposal == null || proposal.Submitted) {
                return HttpNotFound();
            }
            var general = _context.Generals.SingleOrDefault(c => c.Id == proposal.GeneralId);

            if (general == null)
            {
                return HttpNotFound();
            }

            var alldepts = _context.Ref_Department.ToList();
            List<SelectListItem> listSelectListDepts = new List<SelectListItem>();
            foreach (Ref_Department dept in alldepts) {
                SelectListItem selectList = new SelectListItem()
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString(),
                };
                listSelectListDepts.Add(selectList);
            }
            // I -> Initiating
            string queryI = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 1 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedInit = _context.Database.SqlQuery<Ref_Department>(queryI).ToList();

            // C -> Collaborating
            string queryC = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 2 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedCollab = _context.Database.SqlQuery<Ref_Department>(queryC).ToList();

            // S -> Servicing
            string queryS = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 3 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedServ = _context.Database.SqlQuery<Ref_Department>(queryS).ToList();

            List<string> selectedI = new List<string>();
            foreach (Ref_Department dept in selectedInit)
            {
                string s = dept.Name;
                selectedI.Add(s);
            }

            List<string> selectedC = new List<string>();
            foreach (Ref_Department dept in selectedCollab)
            {
                string s = dept.Name;
                selectedC.Add(s);
            }

            List<string> selectedS = new List<string>();
            foreach (Ref_Department dept in selectedServ)
            {
                string s = dept.Name;
                selectedS.Add(s);
            }

            var viewModel = GetFormViewModel();
            viewModel.General = general;
            viewModel.InitDepts = listSelectListDepts;
            viewModel.SelectedInitDepts = selectedI;
            viewModel.CollabDepts = listSelectListDepts;
            viewModel.SelectedCollabDepts = selectedC;
            viewModel.InitDepts = listSelectListDepts;
            viewModel.SelectedServDepts = selectedS;
            return View("Form", viewModel);
        }

    }
}