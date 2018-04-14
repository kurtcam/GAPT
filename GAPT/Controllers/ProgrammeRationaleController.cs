using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ProgrammeRationaleController : Controller
    {
        private GaptDbContext _context;

        public ProgrammeRationaleController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: ProgrammeRationale
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (proposal.ProgrammeRationaleId == null)
            {
                return RedirectToAction("New", "Rationale", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Edit", "Rationale", new { id = proposal.Id });
            }
        }
    }
}