using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ProgrammeStudyController : Controller
    {
        private GaptDbContext _context;

        public ProgrammeStudyController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: ProgrammeStudy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            ProgrammeRationale pr = null;
            if (proposal.ProgrammeRationaleId != null)
            {
                pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            }
            if (pr.PsId != null)
            {
                return HttpNotFound();
            }
            var viewModel = new ProgrammeStudyFormViewModel
            {
                Proposal = proposal,
            };
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ProgrammeStudyFormViewModel vm)
        {
            var programmeStudy = vm.ProgrammeStudy;
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            if (!ModelState.IsValid)
            {
                return View("Form", programmeStudy);
            }

            if (programmeStudy.Id == 0)
            {
                _context.ProgrammeOfStudies.Add(programmeStudy);
                var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.Id);
                pr.PsId = programmeStudy.Id;
                pr.ProgrammeOfStudy = programmeStudy;
            }
            else
            {
                var psInDb = _context.ProgrammeOfStudies.SingleOrDefault(m => m.Id == programmeStudy.Id);
                psInDb.KnowledgeUnderstanding = programmeStudy.KnowledgeUnderstanding;
                psInDb.IntellectualDevelopment = programmeStudy.IntellectualDevelopment;
                psInDb.OtherSkills = programmeStudy.OtherSkills;
            }

            _context.SaveChanges();
            var jump = Request["jump"];
            switch (jump)
            {
                case "0":
                    {
                        // Save pressed -> return form
                        return RedirectToAction("Edit", "ProgrammeStudy", new { id = proposal.Id });
                    }
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Edit", "Demand", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        //return RedirectToAction("Jump", "TentativePs", new { id = proposal.Id });
                        return RedirectToAction("Jump", "TentativePs", new { id = proposal.Id });
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

        public ActionResult Edit(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (proposal.ProgrammeRationaleId == null)
            {
                return HttpNotFound();
            }

            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            var programmeStudy = _context.ProgrammeOfStudies.SingleOrDefault(c => c.Id == pr.PsId);

            if (programmeStudy == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ProgrammeStudyFormViewModel
            {
                Proposal = proposal,
                ProgrammeStudy = programmeStudy,
            };
            return View("Form", viewModel);
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            var pr = _context.ProgrammeRationales.SingleOrDefault(c => c.Id == proposal.ProgrammeRationaleId);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (pr.PsId == null)
            {
                return RedirectToAction("New", "ProgrammeStudy", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Edit", "ProgrammeStudy", new { id = proposal.Id });
            }
        }
    }
}