using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GAPT.Models;
using System.Web.Mvc;

namespace GAPT.ViewModels
{
    public class GeneralFormViewModel
    {
        public General General { get; set; }
        public IEnumerable<Ref_Level> Levels { get; set; }
        public IEnumerable<Ref_Delivery> Deliveries { get; set; }
        public IEnumerable<Ref_Faculty> Faculties { get; set; }

        public IEnumerable<string> SelectedInitDepts { get; set; }
        public IEnumerable<SelectListItem> InitDepts { get; set; }

        public IEnumerable<string> SelectedServDepts { get; set; }
        public IEnumerable<SelectListItem> ServDepts { get; set; }

        public IEnumerable<string> SelectedCollabDepts { get; set; }
        public IEnumerable<SelectListItem> CollabDepts { get; set; }
    }
}