using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class InPrincipleIndexViewModel
    {
        public Proposal Proposal { get; set; }
        public PvcApproval PvcApproval { get; set; }
        public SenateDecision SenateDecision { get; set; }
        public CouncilDecision CouncilDecision { get; set; }
    }
}