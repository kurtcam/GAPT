using GAPT.Models;
using System.Linq;
using System.Web.Mvc;

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
    }
}