using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ApprovalController : Controller
    {
        private GaptDbContext _context;

        public ApprovalController()
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
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (proposal.ApprovalId == null)
            {
                Approval a = new Approval();
                proposal.Approval = a;
                _context.Approvals.Add(a);
                _context.SaveChanges();
                return RedirectToAction("Approval", "Proposal", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Approval", "Proposal", new { id = proposal.Id });
            }
        }
        
    }
}