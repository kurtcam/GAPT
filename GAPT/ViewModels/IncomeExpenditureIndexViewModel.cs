using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class IncomeExpenditureIndexViewModel
    {
        public Proposal Proposal { get; set; }
        public IEnumerable<StatementIE> Statements { get; set; }
        public string ErrorMessage { get; set; }
    }
}