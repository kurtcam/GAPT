using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class InPrincipleController : Controller
    {
        private GaptDbContext _context;

        public InPrincipleController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            if (proposal.InPrincipalId == null)
            {
                InPrincipal a = new InPrincipal();
                proposal.InPrincipal = a;
                _context.InPrincipals.Add(a);


                var pvc = new PvcApproval();
                pvc.Upload = "_";
                _context.PvcApprovals.Add(pvc);

                var ipp = new InPrincipal_Pvc();
                ipp.PvcApproval = pvc;
                ipp.PvcApprovalId = pvc.Id;
                ipp.InPrincipal = a;
                ipp.InPrincipalId = a.Id;

                _context.InPrincipal_Pvc.Add(ipp);
                _context.SaveChanges();
                return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });
            }
        }
    }
}