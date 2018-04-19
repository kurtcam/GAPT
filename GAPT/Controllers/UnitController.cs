using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;
namespace GAPT.Controllers
{
    public class UnitController : Controller
    {
        private GaptDbContext _context;

        public UnitController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        
        public ActionResult New(int yearRedirect)
        {
            var viewModel = GetViewModel(yearRedirect);
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(UnitFormViewModel vm)
        {
            var unit = vm.Unit;
            if (unit.Id != 0) {
                return HttpNotFound();
            }
            unit.Code += " *";
            if (!ModelState.IsValid)
            {
                var viewModel = GetViewModel(vm.YearRedirect);
                //viewModel.Unit.Code = unit.Code;
                //viewModel.Unit.Title = unit.Title;
                return View("Form", viewModel);
            }
            if (vm.SelectedDept != null)
            {
                string s = vm.SelectedDept;
                var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                unit.Ref_Department = department;
                unit.DepartmentId = department.Id;
            }
            _context.Ref_Unit.Add(unit);
            

            _context.SaveChanges();

            //check that user can go to yearredirect
            return RedirectToAction("Edit", "Year", new { id = vm.YearRedirect });

        }

        private UnitFormViewModel GetViewModel(int yearRedirect) {
            var departments = _context.Ref_Department.ToList().OrderBy(m => m.Name);
            List<SelectListItem> listSelectListDepts = new List<SelectListItem>();
            foreach (Ref_Department dept in departments)
            {
                SelectListItem selectList = new SelectListItem()
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString(),
                };
                listSelectListDepts.Add(selectList);
            }

            var viewModel = new UnitFormViewModel
            {
                YearRedirect = yearRedirect,
                Depts = listSelectListDepts,
                Unit = new Ref_Unit()
            };
            return viewModel;
        }
    }
}