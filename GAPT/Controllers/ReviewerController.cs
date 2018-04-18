using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ReviewerController : Controller
    {
        private GaptDbContext _context;

        public ReviewerController()
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
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            var viewModel = new ReviewerFormViewModel{
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }
        
        public ActionResult Edit(int id)
        {
            var reviewer = _context.Reviewers.SingleOrDefault(m => m.Id == id);
            if (reviewer == null)
            {
                return HttpNotFound();
            }

            var err = _context.ExternalReview_Reviewer.SingleOrDefault(m => m.ReviewerId == id);

            var proposal = _context.Proposals.SingleOrDefault(m => m.ExternalReviewId == err.ExternalReviewId);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            var viewModel = new ReviewerFormViewModel
            {
                Reviewer = reviewer,
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }
        
        public ActionResult Delete(int id)
        {

            var reviewer = _context.Reviewers.SingleOrDefault(m => m.Id == id);
            if (reviewer == null)
            {
                return HttpNotFound();
            }

            var err = _context.ExternalReview_Reviewer.SingleOrDefault(m => m.ReviewerId == id);

            var proposal = _context.Proposals.SingleOrDefault(m => m.ExternalReviewId == err.ExternalReviewId);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            _context.ExternalReview_Reviewer.Remove(err);
            _context.Reviewers.Remove(reviewer);
            _context.SaveChanges();

            return RedirectToAction("Index", "ExternalReview", new { id = proposal.Id });
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ReviewerFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            var reviewer = vm.Reviewer;
            if (!ModelState.IsValid)
            {
                return View("Form", reviewer);
            }

            if (reviewer.Id == 0)
            {
                _context.Reviewers.Add(reviewer);
                ExternalReview_Reviewer err = new ExternalReview_Reviewer();
                err.Reviewer = reviewer;
                err.ExternalReview = proposal.ExternalReview;
                _context.ExternalReview_Reviewer.Add(err);
            }
            else
            {
                var reviewerInDb = _context.Reviewers.SingleOrDefault(m => m.Id == reviewer.Id);
                reviewerInDb.Name = reviewer.Name;
                reviewerInDb.Affiliation = reviewer.Affiliation;
                reviewerInDb.Address = reviewer.Address;
                reviewerInDb.Email = reviewer.Email;
                reviewerInDb.Telephone = reviewer.Telephone;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "ExternalReview", new { id = proposal.Id });

        }
    }
}