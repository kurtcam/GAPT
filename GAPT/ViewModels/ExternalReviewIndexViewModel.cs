using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class ExternalReviewIndexViewModel
    {
        public Proposal Proposal { get; set; }
        public IEnumerable<Reviewer> Reviewers { get; set; }
    }
}