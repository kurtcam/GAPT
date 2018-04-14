using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.ViewModels
{
    public class UnitFormViewModel
    {
        public Ref_Unit Unit { get; set; }
        public int YearRedirect { get; set; }
        public string SelectedDept { get; set; }
        public IEnumerable<SelectListItem> Depts { get; set; }
    }
}