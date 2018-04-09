using GAPT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GAPT.ViewModels;
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
    }


}