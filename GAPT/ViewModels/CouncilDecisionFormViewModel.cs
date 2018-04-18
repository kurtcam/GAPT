using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class CouncilDecisionFormViewModel
    {
        public Proposal Proposal { get; set; }
        public CouncilDecision CouncilDecision { get; set; }
    }
}