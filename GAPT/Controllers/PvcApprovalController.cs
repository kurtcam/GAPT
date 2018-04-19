using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;
using System.IO;

namespace GAPT.Controllers
{
    public class PvcApprovalController : Controller
    {
        private GaptDbContext _context;

        public PvcApprovalController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Details(int id)
        {
            var pvc = _context.PvcApprovals.SingleOrDefault(m => m.Id == id);
            if (pvc == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ipp = _context.InPrincipal_Pvc.SingleOrDefault(m => m.PvcApprovalId == pvc.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.InPrincipalId == ipp.InPrincipalId);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var viewModel = new PvcApprovalFormViewModel
            {
                Proposal = proposal,
                PvcApproval = pvc
            };
            return View("Form", viewModel);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PvcApprovalFormViewModel vm, HttpPostedFileBase postedFile)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            var pvc = vm.PvcApproval;
            if (pvc.Selection == true)
            {
                //if a is chosen
                pvc.Upload = "_";

                if (pvc.CouncilRef == true)
                {
                    //refer to council
                    var council = new CouncilDecision();
                    _context.CouncilDecisions.Add(council);

                    var ipc = new InPrincipal_Council();
                    ipc.CouncilDecision = council;
                    ipc.CouncilDecisionId = council.Id;
                    ipc.InPrincipal = proposal.InPrincipal;
                    ipc.InPrincipalId = proposal.InPrincipal.Id;

                    _context.InPrincipal_Council.Add(ipc);
                }
                else
                {
                    pvc.CouncilRef = false;
                }

                //refer to senate
                var senate = new SenateDecision();
                _context.SenateDecisions.Add(senate);

                var ips = new InPrincipal_Senate();
                ips.SenateDecision = senate;
                ips.SenateDecisionId = senate.Id;
                ips.InPrincipal = proposal.InPrincipal;
                ips.InPrincipalId = proposal.InPrincipal.Id;

                _context.InPrincipal_Senate.Add(ips);
            }
            else
            {
                //if b is chosen
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName.Replace(' ', '_')));
                    pvc.Upload = postedFile.FileName.Replace(' ', '_');
                    pvc.CouncilRef = null;
                    proposal.InPrincipalApproved = false;
                }
                else {
                    //return form
                    var viewModel = new PvcApprovalFormViewModel
                    {
                        Proposal = proposal,
                        PvcApproval = pvc
                    };
                    return View("Form", viewModel);
                }
            }

            var pvcInDb = _context.PvcApprovals.SingleOrDefault(m => m.Id == pvc.Id);
            pvcInDb.Upload = pvc.Upload;
            pvcInDb.CouncilRef = pvc.CouncilRef;
            pvcInDb.SignedBy = "DeanXYZ";
            pvcInDb.SignedDate = DateTime.Now;
            pvcInDb.Selection = pvc.Selection;

            _context.SaveChanges();
            return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });

        }
    }
}