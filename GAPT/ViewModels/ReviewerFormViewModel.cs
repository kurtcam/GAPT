using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class ReviewerFormViewModel
    {
        public Proposal Proposal { get; set; }
        public Reviewer Reviewer { get; set; }
    }
}