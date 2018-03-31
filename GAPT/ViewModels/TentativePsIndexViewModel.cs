using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class TentativePsIndexViewModel
    {
        public Proposal Proposal { get; set; }
        public IEnumerable<Year> Years { get; set; }
    }
}