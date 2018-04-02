using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class EndorsementStatementController : Controller
    {
        private GaptDbContext _context;

        public EndorsementStatementController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: EndorsementStatement
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

                        ExternalReview_Endorsement obj = new ExternalReview_Endorsement();
                        obj.ExternalReview = proposal.ExternalReview;
                        obj.ExternalReviewId = (int)proposal.ExternalReviewId;
                        obj.EndorsementCollab = end;
                        obj.EndorsementId = end.Id;
                        _context.ExternalReview_Endorsement.Add(obj);
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
                        stm.Reservations = " ";
                        _context.StatementServs.Add(stm);

                        ExternalReview_Statement obj = new ExternalReview_Statement();
                        obj.ExternalReview = proposal.ExternalReview;
                        obj.ExternalReviewId = (int)proposal.ExternalReviewId;
                        obj.StatementServ = stm;
                        obj.StatementId = stm.Id;
                        _context.ExternalReview_Statement.Add(obj);
                        _context.SaveChanges();
                    }
                }
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
            var viewModel = new EndorsementStatementIndexViewModel
            {
                CollabDepts = collabDepts,
                ServDepts = servDepts,
                Proposal = proposal
            };
            return View(viewModel);
        }
    }
}