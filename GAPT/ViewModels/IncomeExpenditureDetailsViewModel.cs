using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GAPT.Models;

namespace GAPT.ViewModels
{
    public class IncomeExpenditureDetailsViewModel
    {
        public Proposal Proposal { get; set; }
        public IEnumerable<StatementIE> Statements { get; set; }
    }
}