using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class YearFormViewModel
    {
        public Proposal Proposal { get; set; }
        public Year Year { get; set; }
        public IEnumerable<Ref_Unit> AllUnits { get; set; }
        public IEnumerable<Year_Unit> SelectedUnits { get; set; }
        public IEnumerable<Ref_Unit> UnselectedUnits { get; set; }
    }
}