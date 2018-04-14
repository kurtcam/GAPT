using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class StatementServController : Controller
    {

        private GaptDbContext _context;

        public StatementServController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: StatementServ
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var stmServ = _context.StatementServs.SingleOrDefault(m => m.Id == id);
            if (stmServ == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ers = _context.Approval_Statement.SingleOrDefault(m => m.StatementId == stmServ.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.ApprovalId == ers.ApprovalId);
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (stmServ.SignedBy == null)
            {
                //return Form
                //stmServ.Reservations = null;
                var viewModel = new StatementServFormViewModel
                {
                    Proposal = proposal,
                    Statement = stmServ
                };
                return View("Form", viewModel);
            }
            else
            {
                if (stmServ.Selection == true)
                {
                    //return accept view
                    var viewModel = new StatementServFormViewModel
                    {
                        Proposal = proposal,
                        Statement = stmServ
                    };
                    return View("Accepted", viewModel);
                }
                else
                {
                    //return reject view
                    var viewModel = new StatementServFormViewModel
                    {
                        Proposal = proposal,
                        Statement = stmServ
                    };
                    return View("Rejected", viewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(StatementServFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            var statement = vm.Statement;
            if (statement.Selection == true)
            {
                //if a is chosen, set reservations to a default string
                statement.Reservations = "_";
            }
            if (!ModelState.IsValid)
            {
                return View("Form", statement);
            }

            var statementInDb = _context.StatementServs.SingleOrDefault(m => m.Id == statement.Id);
            statementInDb.Reservations = statement.Reservations;
            statementInDb.SignedBy = "DeanXYZ";
            statementInDb.SignedDate = DateTime.Now;
            statementInDb.Selection = statement.Selection;

            _context.SaveChanges();
            return RedirectToAction("Index", "Approval", new { id = proposal.Id });

        }
    }
}