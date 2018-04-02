using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class EndorsementStatementIndexViewModel
    {
        public Proposal Proposal { get; set; }
        public IEnumerable<Ref_Department> CollabDepts { get; set; }
        public IEnumerable<Ref_Department> ServDepts { get; set; }
    }
}