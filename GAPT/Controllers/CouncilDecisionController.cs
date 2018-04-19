using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class CouncilDecisionController : Controller
    {
        private GaptDbContext _context;

        public CouncilDecisionController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Details(int id)
        {
            var council = _context.CouncilDecisions.SingleOrDefault(m => m.Id == id);
            if (council == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ipc = _context.InPrincipal_Council.SingleOrDefault(m => m.CouncilDecisionId == council.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.InPrincipalId == ipc.InPrincipalId);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var viewModel = new CouncilDecisionFormViewModel
            {
                Proposal = proposal,
                CouncilDecision = council
            };
            return View("Form", viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CouncilDecisionFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var council = vm.CouncilDecision;
            if (!ModelState.IsValid)
            {
                //return form
                var viewModel = new CouncilDecisionFormViewModel
                {
                    Proposal = proposal,
                    CouncilDecision = council
                };
                return View("Form", viewModel);
            }
            if (council.Selection == true)
            {
                //if a is chosen
                
                var senate = proposal.GetSenateDecision();
                if (senate.SignedBy != null)
                {
                    if (senate.Selection == true)
                    {
                        //council also approved
                        proposal.InPrincipalApproved = true;
                    }
                    else
                    {
                        proposal.InPrincipalApproved = false;
                    }
                }
                
            }
            else
            {
                //if b is chosen
                proposal.InPrincipalApproved = false;

            }

            var councilInDb = _context.CouncilDecisions.SingleOrDefault(m => m.Id == council.Id);
            councilInDb.SignedBy = "DeanXYZ";
            councilInDb.SignedDate = DateTime.Now;
            councilInDb.Selection = council.Selection;

            _context.SaveChanges();
            return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });

        }
    }
}