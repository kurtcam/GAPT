using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class StatementIEController : Controller
    {
        private GaptDbContext _context;

        public StatementIEController()
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
            var viewModel = new StatementIEFormViewModel
            {
                Proposal = proposal,
                StatementIE = new StatementIE()
            };
            return View("Form", viewModel);
        }
        
        public ActionResult Edit(int id)
        {
            var statement = _context.StatementIEs.SingleOrDefault(m => m.Id == id);
            if (statement == null)
            {
                return HttpNotFound();
            }

            var ies = _context.IncomeExpenditure_StatementIE.SingleOrDefault(m => m.StatementId == id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.IncomeExpenditureId == ies.IncomeExpenditureId);
              
            var viewModel = new StatementIEFormViewModel
            {
                StatementIE = statement,
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }

        public ActionResult Delete(int id)
        {

            var statement = _context.StatementIEs.SingleOrDefault(m => m.Id == id);
            if (statement == null)
            {
                return HttpNotFound();
            }

            var ies = _context.IncomeExpenditure_StatementIE.SingleOrDefault(m => m.StatementId == id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.IncomeExpenditureId == ies.IncomeExpenditureId);


            _context.IncomeExpenditure_StatementIE.Remove(ies);
            _context.StatementIEs.Remove(statement);
            _context.SaveChanges();

            return RedirectToAction("Index", "IncomeExpenditure", new { id = proposal.Id });
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(StatementIEFormViewModel vm, HttpPostedFileBase postedFile)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            var statement = vm.StatementIE;
            if (!ModelState.IsValid)
            {
                return View("Form", statement);
            }
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName.Replace(' ', '_')));
            }
            if (statement.Id == 0)
            {
                if (postedFile != null)
                {
                    statement.Upload = postedFile.FileName.Replace(' ', '_');
                }

                _context.StatementIEs.Add(statement);
                IncomeExpenditure_StatementIE ies = new IncomeExpenditure_StatementIE();
                ies.StatementIE = statement;
                ies.IncomeExpenditure = proposal.IncomeExpenditure;
                _context.IncomeExpenditure_StatementIE.Add(ies);
            }
            else
            {
                var statementInDb = _context.StatementIEs.SingleOrDefault(m => m.Id == statement.Id);
                if (postedFile != null)
                {
                    statementInDb.Upload = postedFile.FileName.Replace(' ', '_');
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "IncomeExpenditure", new { id = proposal.Id });

        }
    }
}