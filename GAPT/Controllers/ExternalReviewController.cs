using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ExternalReviewController : Controller
    {
        private GaptDbContext _context;

        public ExternalReviewController()
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
            var er = _context.ExternalReviews.SingleOrDefault(m => m.Id == proposal.ExternalReviewId);
            var reviewers = er.GetReviewers();

            var viewModel = new ExternalReviewIndexViewModel
            {
                Proposal = proposal,
                Reviewers = reviewers
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
            if (proposal.ExternalReviewId == null)
            {
                ExternalReview er = new ExternalReview();
                proposal.ExternalReview = er;
                _context.ExternalReviews.Add(er);
                _context.SaveChanges();
                return RedirectToAction("Index", "ExternalReview", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Index", "ExternalReview", new { id = proposal.Id });
            }
        }

        [HttpPost]
        public ActionResult DummySave(ExternalReviewIndexViewModel vm)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            var jump = Request["jump"];
            switch (jump)
            {
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Jump", "TentativePs", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = proposal.Id });
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