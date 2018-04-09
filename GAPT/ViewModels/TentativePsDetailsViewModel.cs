using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class TentativePsDetailsViewModel
    {
        public Proposal Proposal { get; set; }
        public Dictionary<Year, List<Year_Unit>> Dictionary { get; set; }
    }
}