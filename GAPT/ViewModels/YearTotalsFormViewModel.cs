using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class YearTotalsFormViewModel
    {
        public SortedDictionary<int, int> Pairs { get; set; }
        public Proposal Proposal { get; set; }
        public TentativeP TentativePs { get; set; }
    }
}