using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class EndorsementCollabController : Controller
    {

        private GaptDbContext _context;

        public EndorsementCollabController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: EndorsementCollab
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var endCollab = _context.EndorsementCollabs.SingleOrDefault(m => m.Id == id);
            if (endCollab == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ae = _context.Approval_Endorsement.SingleOrDefault(m => m.EndorsementId == endCollab.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.ApprovalId == ae.ApprovalId);
            
            if (endCollab.SignedBy == null)
            {
                //return Form

                //set datepicker start to today
                endCollab.HeldDate = DateTime.Today.ToString();
                var viewModel = new EndorsementCollabFormViewModel
                {
                    Proposal = proposal,
                    Endorsement = endCollab
                };
                return View("Form", viewModel);
            }
            else
            {
                if (endCollab.Selection == true)
                {
                    //return accept view
                    var viewModel = new EndorsementCollabFormViewModel
                    {
                        Proposal = proposal,
                        Endorsement = endCollab
                    };
                    return View("Accepted", viewModel);
                }
                else
                {
                    //return reject view
                    var viewModel = new EndorsementCollabFormViewModel
                    {
                        Proposal = proposal,
                        Endorsement = endCollab
                    };
                    return View("Rejected", viewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EndorsementCollabFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            var endorsement = vm.Endorsement;
            if (endorsement.Selection == true) {
                //if a is chosen, set helddate to a default date
                endorsement.HeldDate = "01/01/0001";
            }
            if (!ModelState.IsValid)
            {
                return View("Form", endorsement);
            }
            
            var endorsementInDb = _context.EndorsementCollabs.SingleOrDefault(m => m.Id == endorsement.Id);
            endorsementInDb.HeldDate = endorsement.HeldDate;
            endorsementInDb.SignedBy = "DeanXYZ";
            endorsementInDb.SignedDate = DateTime.Now;
            endorsementInDb.Selection = endorsement.Selection;

            _context.SaveChanges();
            return RedirectToAction("Index", "Approval", new { id = proposal.Id });

        }
    }
}