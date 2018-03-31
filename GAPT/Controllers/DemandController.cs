using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class DemandController : Controller
    {
        private GaptDbContext _context;

        public DemandController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Demand
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
            if (pr.DemandId != null) {
                return HttpNotFound();
            }
            var viewModel = new DemandFormViewModel
            {
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(DemandFormViewModel vm)
        {
            var demand = vm.Demand;
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            if (!ModelState.IsValid)
            {
                return View("Form", demand);
            }

            if (demand.Id == 0)
            {
                _context.Demands.Add(demand);
                var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.Id);
                pr.DemandId = demand.Id;
                pr.Demand = demand;
            }
            else
            {
                var demandInDb = _context.Demands.SingleOrDefault(m => m.Id == demand.Id);
                demandInDb.Description = demand.Description;
                demandInDb.Period = demand.Period;
            }

            _context.SaveChanges();
            var jump = Request["jump"];
            switch (jump)
            {
                case "0":
                    {
                        // Save pressed -> return form
                        return RedirectToAction("Edit", "Demand", new { id = proposal.Id });
                    }
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Edit", "Rationale", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "Demand", new { id = proposal.Id });
                    }
                case "A":
                    {
                        // A pressed -> go to Section A
                        return RedirectToAction("Edit", "General", new { id = proposal.Id });
                    }
                case "B":
                    {
                        // B pressed -> go to Section B
                        return RedirectToAction("Edit", "Rationale", new { id = proposal.Id });
                    }
                case "C":
                    {
                        // C pressed -> go to Section C
                        return RedirectToAction("Jump", "ExternalReview", new { id = proposal.Id });
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
            if (proposal == null) {
                return HttpNotFound();
            }
            if (proposal.ProgrammeRationaleId == null)
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var demand = _context.Demands.SingleOrDefault(c => c.Id == pr.DemandId);

            if (demand == null)
            {
                return HttpNotFound();
            }
            var viewModel = new DemandFormViewModel
            {
                Proposal = proposal,
                Demand = demand,
            };
            return View("Form", viewModel);
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (pr.PsId == null)
            {
                return RedirectToAction("New", "ProgrammeStudy", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Edit", "ProgrammeStudy", new { id = proposal.Id });
            }
        }
    }
}