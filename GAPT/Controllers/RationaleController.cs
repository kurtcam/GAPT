using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class RationaleController : Controller
    {
        private GaptDbContext _context;

        public RationaleController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Rationale
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
            if (proposal.ProgrammeRationaleId != null) {
                return HttpNotFound();
            }
            var viewModel = new RationaleFormViewModel
            {
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(RationaleFormViewModel vm)
        {
            var rationale = vm.Rationale;
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (proposal.Submitted)
            {
                return Content("Proposal already submitted");
            }
            if (!ModelState.IsValid)
            {
                return View("Form", rationale);
            }

            if (rationale.Id == 0)
            {
                _context.Rationales.Add(rationale);
                ProgrammeRationale pr = new ProgrammeRationale();
                pr.RationaleId = rationale.Id;
                _context.ProgrammeRationales.Add(pr);
                proposal.ProgrammeRationaleId = pr.Id;
                proposal.ProgrammeRationale = pr;
            }
            else
            {
                var rationaleInDb = _context.Rationales.SingleOrDefault(m => m.Id == rationale.Id);
                rationaleInDb.Justification = rationale.Justification;
                rationaleInDb.Fit = rationale.Fit;
                rationaleInDb.Differences = rationale.Differences;
            }

            _context.SaveChanges();
            var jump = Request["jump"];
            switch (jump)
            {
                case "0":
                    {
                        // Save pressed -> return form
                        return RedirectToAction("Edit", "Rationale", new { id = proposal.Id });
                    }
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Edit", "General", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "Rationale", new { id = proposal.Id });
                    }
                case "A":
                    {
                        // A pressed -> go to Section A
                        return RedirectToAction("Edit", "General", new { id = proposal.Id });
                    }
                case "B":
                    {
                        // B pressed -> go to Section B
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = proposal.Id });
                    }
                case "C":
                    {
                        // C pressed -> go to Section C
                        return RedirectToAction("Jump", "ExternalReview", new { id = proposal.Id });
                    }
                case "D":
                    {
                        // D pressed -> go to Section D
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = proposal.Id });
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
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (proposal.ProgrammeRationaleId == null)
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var rationale = _context.Rationales.SingleOrDefault(c => c.Id == pr.RationaleId);
            if (rationale == null)
            {
                return HttpNotFound();
            }
            var viewModel = new RationaleFormViewModel
            {
                Proposal = proposal,
                Rationale = rationale,
            };
            return View("Form", viewModel);
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (pr.DemandId == null)
            {
                return RedirectToAction("New", "Demand", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Edit", "Demand", new { id = proposal.Id });
            }
        }
    }
}