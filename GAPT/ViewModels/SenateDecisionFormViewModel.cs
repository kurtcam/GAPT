using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class SenateDecisionFormViewModel
    {
        public Proposal Proposal { get; set; }
        public SenateDecision SenateDecision { get; set; }
    }
}