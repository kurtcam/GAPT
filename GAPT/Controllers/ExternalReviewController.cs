﻿using GAPT.Models;
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
            if (proposal == null)
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
            if (proposal == null)
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
    }
}