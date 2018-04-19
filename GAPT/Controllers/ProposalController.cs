using GAPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GAPT.ViewModels;
using System;

namespace GAPT.Controllers
{
    public class ProposalController : Controller
    {
        private GaptDbContext _context;

        public ProposalController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Proposal
        public ActionResult Index()
        {
            var proposals = _context.Proposals.ToList();

            return View(proposals);
        }

        public ActionResult New()
        {
            return RedirectToAction("New", "General");
        }

        public ActionResult Submit(int id) {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            string errorMessage = null;
            if (proposal == null)
            {
                return HttpNotFound();
            }

            if (proposal.GeneralId == null)
            {
                errorMessage = "Section A not filled in yet.";
            }
            else if (proposal.ProgrammeRationaleId == null)
            {
                errorMessage = "Section B not filled in yet.";
            }
            else if (proposal.ExternalReviewId == null)
            {
                errorMessage = "Section C not filled in yet.";
            }
            else if (proposal.IncomeExpenditureId == null)
            {
                errorMessage = "Section D not filled in yet.";
            }
            else
            {
                var general = proposal.General;
                var pr = proposal.ProgrammeRationale;
                var er = proposal.ExternalReview;
                var ie = proposal.IncomeExpenditure;
                if (general.GetErrorMessage() != null) {
                    errorMessage = general.GetErrorMessage();
                }
                else if (pr.GetErrorMessage() != null)
                {
                    errorMessage = pr.GetErrorMessage();
                }
                else if (er.GetErrorMessage() != null)
                {
                    errorMessage = er.GetErrorMessage();
                }
                else if (ie.GetErrorMessage() != null)
                {
                    errorMessage = ie.GetErrorMessage();
                }
            }

            if (errorMessage != null)
            {
                // an error is present
                var ie = _context.IncomeExpenditures.SingleOrDefault(m => m.Id == proposal.IncomeExpenditureId);
                var statements = ie.GetStatements();

                var viewModel = new IncomeExpenditureIndexViewModel
                {
                    Proposal = proposal,
                    Statements = statements,
                    ErrorMessage = errorMessage
                };
                return View("../IncomeExpenditure/Index", viewModel);
            }
            else {
                proposal.Submitted = true;
                _context.SaveChanges();
                return RedirectToAction("Index", "Proposal");
            }
            
        }

        public ActionResult Details(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal ==  null) {
                return HttpNotFound();
            }

            return View(proposal);
        }

        public ActionResult General(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }

            return View(proposal);
        }

        public ActionResult Rationale(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }

            return View(proposal);
        }

        public ActionResult Demand(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }

            return View(proposal);
        }

        public ActionResult ProgrammeStudy(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }

            return View(proposal);
        }

        public ActionResult TentativePs(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var viewModel = new TentativePsDetailsViewModel();
            var programmeRationale = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            if (programmeRationale == null)
            {
                viewModel.Proposal = proposal;
                viewModel.Dictionary = null;
                return View(viewModel);
            }

            var tentative = _context.TentativePs.SingleOrDefault(m => m.Id == programmeRationale.TentativePsId);
            if (tentative == null)
            {
                viewModel.Proposal = proposal;
                viewModel.Dictionary = null;
                return View(viewModel);
            }

            Dictionary<Year, List<Year_Unit>> dict = new Dictionary<Year, List<Year_Unit>>();
            var years = _context.Years.Where(m => m.TentativePsId == tentative.Id).ToList().OrderBy(m => m.YearNo);
            foreach (Year year in years) {
                var yearUnits = _context.Year_Unit.Where(m => m.YearId == year.Id).ToList();
                dict.Add(year, yearUnits);
                
            }
            viewModel = new TentativePsDetailsViewModel
            {
                Proposal = proposal,
                Dictionary = dict
            };
            return View(viewModel);
        }

        public ActionResult ExternalReview(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ExternalReviewDetailsViewModel();
            var externalReview = _context.ExternalReviews.SingleOrDefault(m => m.Id == proposal.ExternalReviewId);
            if (externalReview == null)
            {
                viewModel.Proposal = proposal;
                viewModel.Reviewers = null;
                return View(viewModel);
            }
            var reviewers = new List<Reviewer>();
            var errs = _context.ExternalReview_Reviewer.Where(m => m.ExternalReviewId == externalReview.Id).ToList();
            foreach (ExternalReview_Reviewer err in errs)
            {
                var reviewer = _context.Reviewers.SingleOrDefault(m => m.Id == err.ReviewerId);
                reviewers.Add(reviewer);

            }
            viewModel = new ExternalReviewDetailsViewModel
            {
                Proposal = proposal,
                Reviewers = reviewers
            };
            return View(viewModel);
        }

        public ActionResult IncomeExpenditure(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            var viewModel = new IncomeExpenditureDetailsViewModel();
            var incomeExpenditure = _context.IncomeExpenditures.SingleOrDefault(m => m.Id == proposal.IncomeExpenditureId);
            if (incomeExpenditure == null)
            {
                viewModel.Proposal = proposal;
                viewModel.Statements = null;
                return View(viewModel);
            }
            var statements = new List<StatementIE>();
            var ies = _context.IncomeExpenditure_StatementIE.Where(m => m.IncomeExpenditureId == incomeExpenditure.Id).ToList();
            foreach (IncomeExpenditure_StatementIE ie in ies)
            {
                var statement = _context.StatementIEs.SingleOrDefault(m => m.Id == ie.StatementId);
                statements.Add(statement);

            }
            viewModel = new IncomeExpenditureDetailsViewModel
            {
                Proposal = proposal,
                Statements = statements
            };
            return View(viewModel);
        }

        public ActionResult Approval(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted)
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
            if (faculty != null)
            {
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
            }
            

            var collabDepts = new List<Ref_Department>();
            var servDepts = new List<Ref_Department>();
            foreach (Department_General dg in AllDeparments)
            {
                if (dg.Type == 2)
                {
                    //collaborating
                    var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == dg.DepartmentId);
                    collabDepts.Add(dept);
                }
                else if (dg.Type == 3)
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

        public ActionResult InPrinciple(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }

            var ipp = _context.InPrincipal_Pvc.SingleOrDefault(m => m.InPrincipalId == proposal.InPrincipalId);
            var pvcApproval = ipp.PvcApproval;

            var ips = _context.InPrincipal_Senate.SingleOrDefault(m => m.InPrincipalId == proposal.InPrincipalId);
            var senateDecision = (ips == null) ? null : ips.SenateDecision;

            var ipc = _context.InPrincipal_Council.SingleOrDefault(m => m.InPrincipalId == proposal.InPrincipalId);
            var councilDecision = (ipc == null) ? null : ipc.CouncilDecision;

            var viewModel = new InPrincipleIndexViewModel
            {
                PvcApproval = pvcApproval,
                SenateDecision = senateDecision,
                CouncilDecision = councilDecision,
                Proposal = proposal
            };
            return View(viewModel);
        }
    }


}