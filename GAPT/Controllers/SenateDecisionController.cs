using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class SenateDecisionController : Controller
    {
        private GaptDbContext _context;

        public SenateDecisionController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Details(int id)
        {
            var senate = _context.SenateDecisions.SingleOrDefault(m => m.Id == id);
            if (senate == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ips = _context.InPrincipal_Senate.SingleOrDefault(m => m.SenateDecisionId == senate.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.InPrincipalId == ips.InPrincipalId);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var viewModel = new SenateDecisionFormViewModel
            {
                Proposal = proposal,
                SenateDecision = senate
            };
            return View("Form", viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(SenateDecisionFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var senate = vm.SenateDecision;
            if (!ModelState.IsValid) {
                //return form
                var viewModel = new SenateDecisionFormViewModel
                {
                    Proposal = proposal,
                    SenateDecision = senate
                };
                return View("Form", viewModel);
            }
            if (senate.Selection == true)
            {
                //if a is chosen
                var pvc = proposal.GetPvcApproval();
                if (pvc.CouncilRef == false)
                {
                    //not referred to council
                    proposal.InPrincipalApproved = true;
                }
                else
                {
                    var council = proposal.GetCouncilDecision();
                    if (council.SignedBy != null) {
                        if (council.Selection == true)
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
            }
            else
            {
                //if b is chosen
                proposal.InPrincipalApproved = false;
                
            }

            var senateInDb = _context.SenateDecisions.SingleOrDefault(m => m.Id == senate.Id);
            senateInDb.SignedBy = "DeanXYZ";
            senateInDb.SignedDate = DateTime.Now;
            senateInDb.Selection = senate.Selection;

            _context.SaveChanges();
            return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });

        }
    }
}