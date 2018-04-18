using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class IncomeExpenditureController : Controller
    {
        private GaptDbContext _context;

        public IncomeExpenditureController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Index(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            var ie = _context.IncomeExpenditures.SingleOrDefault(m => m.Id == proposal.IncomeExpenditureId);
            var statements = ie.GetStatements();

            var viewModel = new IncomeExpenditureIndexViewModel
            {
                Proposal = proposal,
                Statements = statements
            };
            return View(viewModel);
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (proposal.IncomeExpenditureId == null)
            {
                IncomeExpenditure ie = new IncomeExpenditure();
                proposal.IncomeExpenditure = ie;
                _context.IncomeExpenditures.Add(ie);
                _context.SaveChanges();
                return RedirectToAction("Index", "IncomeExpenditure", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Index", "IncomeExpenditure", new { id = proposal.Id });
            }
        }

        [HttpPost]
        public ActionResult DummySave(ExternalReviewIndexViewModel vm)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (proposal.Submitted)
            {
                return Content("Proposal already submitted");
            }
            var jump = Request["jump"];
            switch (jump)
            {
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Jump", "ExternalReview", new { id = proposal.Id });
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
    }
}