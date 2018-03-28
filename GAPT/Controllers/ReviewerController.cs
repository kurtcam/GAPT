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
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ReviewerFormViewModel{
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }

        [Route("Reviewer/Edit/{pid}/{rid}")]
        public ActionResult Edit(int pid, int rid)
        {

            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == pid);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var reviewer = _context.Reviewers.SingleOrDefault(m => m.Id == rid);
            if (reviewer == null)
            {
                return HttpNotFound();
            }

            //checks if there exists a relationship
            var reviewerIds = _context.Database.SqlQuery<int>("Select ReviewerId From dbo.ExternalReview_Reviewer Where ExternalReviewId = " + proposal.ExternalReviewId).ToList();
            if (reviewerIds.Contains(reviewer.Id))
            {
                var viewModel = new ReviewerFormViewModel
                {
                    Reviewer = reviewer,
                    Proposal = proposal,
                };
                return View("Form", viewModel);
            }
            else
            {
                return HttpNotFound();
            }

            
        }

        [Route("Reviewer/Delete/{pid}/{rid}")]
        public ActionResult Delete(int pid, int rid)
        {

            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == pid);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var reviewer = _context.Reviewers.SingleOrDefault(m => m.Id == rid);
            if (reviewer == null)
            {
                return HttpNotFound();
            }

            //checks if there exists a relationship
            var reviewerIds = _context.Database.SqlQuery<int>("Select ReviewerId From dbo.ExternalReview_Reviewer Where ExternalReviewId = " + proposal.ExternalReviewId).ToList();
            if (reviewerIds.Contains(reviewer.Id)) {
                var err = _context.ExternalReview_Reviewer.Single(m => m.ReviewerId == reviewer.Id);
                _context.ExternalReview_Reviewer.Remove(err);
                _context.Reviewers.Remove(reviewer);
                _context.SaveChanges();

                return RedirectToAction("Index", "ExternalReview", new { id = proposal.Id });
            }
            else{
                return HttpNotFound();
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ReviewerFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
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