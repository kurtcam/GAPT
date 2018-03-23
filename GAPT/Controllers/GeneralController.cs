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

        public ActionResult New() {
            var levels = _context.Ref_Level.ToList();
            var faculties = _context.Ref_Faculty.ToList();
            var deliveries = _context.Ref_Delivery.ToList();

            var departments = _context.Ref_Department.ToList();
            List<SelectListItem> listSelectListInitDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListServDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListCollabDepts = new List<SelectListItem>();

            foreach (Ref_Department dept in departments) {
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
            return View("Form", viewModel);
        }
    }
}