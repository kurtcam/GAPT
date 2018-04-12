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
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (proposal.ApprovalId == null)
            {
                Approval a = new Approval();
                proposal.Approval = a;
                _context.Approvals.Add(a);
                _context.SaveChanges();
                return RedirectToAction("Index", "Approval", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Index", "Approval", new { id = proposal.Id });
            }
        }

        // GET: Approval
        public ActionResult Index(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var AllDeparments = _context.Department_General.Where(m => m.GeneralId == proposal.GeneralId).ToList();
            foreach (Department_General dg in AllDeparments)
            {
                if (dg.Type == 2)
                {
                    //collaborating

                    var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == dg.DepartmentId);
                    if (dept.GetCollabEndorsement(proposal.Id) == null)
                    {
                        var end = new EndorsementCollab();
                        end.Ref_Department = dept;
                        end.DepartmentId = dept.Id;
                        end.HeldDate = "01/01/0001";
                        _context.EndorsementCollabs.Add(end);

                        Approval_Endorsement obj = new Approval_Endorsement();
                        obj.Approval = proposal.Approval;
                        obj.ApprovalId = (int)proposal.ApprovalId;
                        obj.EndorsementCollab = end;
                        obj.EndorsementId = end.Id;
                        _context.Approval_Endorsement.Add(obj);
                        _context.SaveChanges();
                    }

                }
                else if (dg.Type == 3)
                {
                    //servicing

                    var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == dg.DepartmentId);
                    if (dept.GetServStatement(proposal.Id) == null)
                    {
                        var stm = new StatementServ();
                        stm.Ref_Department = dept;
                        stm.DepartmentId = dept.Id;
                        stm.Reservations = "_";
                        _context.StatementServs.Add(stm);

                        Approval_Statement obj = new Approval_Statement();
                        obj.Approval = proposal.Approval;
                        obj.ApprovalId = (int)proposal.ApprovalId;
                        obj.StatementServ = stm;
                        obj.StatementId = stm.Id;
                        _context.Approval_Statement.Add(obj);
                        _context.SaveChanges();
                    }
                }
            }

            var faculty = _context.Ref_Faculty.SingleOrDefault(m => m.Id == proposal.General.FacultyId);
            if (faculty == null) {
                return Content("No faculty yet");
            }
            if (faculty.GetRecommendation(proposal.Id) == null)
            {
                var rec = new RecommendationFic();
                rec.Ref_Faculty = faculty;
                rec.FacultyId = faculty.Id;
                rec.HeldDateA = "01/01/0001";
                rec.HeldDateB = "01/01/0001";
                _context.RecommendationFics.Add(rec);

                Approval_Recommendation obj = new Approval_Recommendation();
                obj.Approval = proposal.Approval;
                obj.ApprovalId = (int)proposal.ApprovalId;
                obj.RecommendationFic = rec;
                obj.RecommendationId = rec.Id;
                _context.Approval_Recommendation.Add(obj);
                _context.SaveChanges();
            }

            var collabDepts = new List<Ref_Department>();
            var servDepts = new List<Ref_Department>();
            foreach (Department_General dg in AllDeparments) {
                if (dg.Type == 2)
                {
                    //collaborating
                    var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == dg.DepartmentId);
                    collabDepts.Add(dept);
                }
                else if(dg.Type == 3)
                {
                    //servicing
                    var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == dg.DepartmentId);
                    servDepts.Add(dept);
                }
            }
            var viewModel = new ApprovalIndexViewModel
            {
                CollabDepts = collabDepts,
                ServDepts = servDepts,
                Proposal = proposal,
                Faculty = faculty
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DummySave(ExternalReviewIndexViewModel vm)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == vm.Proposal.Id);
            var jump = Request["jump"];
            switch (jump)
            {
                case "-1":
                    {
                        // Previous pressed -> return form
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = proposal.Id });
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