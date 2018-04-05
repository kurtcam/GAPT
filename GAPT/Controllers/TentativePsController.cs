using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class TentativePsController : Controller
    {
        private GaptDbContext _context;

        public TentativePsController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: TentativePs
        public ActionResult Index(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            var tp = _context.TentativePs.SingleOrDefault(m => m.Id == pr.TentativePsId);
            var years = _context.Years.Where(m => m.TentativePsId == tp.Id).OrderBy(m => m.YearNo).ToList();

            var viewModel = new TentativePsIndexViewModel
            {
                Proposal = proposal,
                Years = years
            };
            return View(viewModel);
        }
        

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (pr.TentativePsId == null)
            {
                TentativeP tp = new TentativeP();
                pr.TentativeP = tp;
                _context.TentativePs.Add(tp);
                _context.SaveChanges();
                return RedirectToAction("Index", "TentativePs", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Index", "TentativePs", new { id = proposal.Id });
            }
            
        }

        [HttpPost]
        public ActionResult DummySave(TentativePsIndexViewModel vm)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            var jump = Request["jump"];
            switch (jump)
            {
                
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Edit", "ProgrammeStudy", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "ExternalReview", new { id = proposal.Id });
                    }
                case "A":
                    {
                        // A pressed -> go to Section A
                        return RedirectToAction("Edit", "General", new { id = proposal.Id });
                    }
                case "B":
                    {
                        // B pressed -> go to Section B
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = proposal.Id });
                    }
                case "C":
                    {
                        // C pressed -> go to Section C
                        return RedirectToAction("Jump", "ExternalReview", new { id = proposal.Id });
                    }
                case "D":
                    {
                        // D pressed -> go to Section D
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = proposal.Id });
                    }
                case "E":
                    {
                        // E pressed -> go to Section E
                        return RedirectToAction("Jump", "Approval", new { id = proposal.Id });
                    }
                default:
                    {
                        return RedirectToAction("Index", "Proposal");
                    }
            }

        }
    }


}