//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GAPT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class IncomeExpenditure_StatementIE
    {
        public int Id { get; set; }
        public int IncomeExpenditureId { get; set; }
        public int StatementId { get; set; }
    
        public virtual IncomeExpenditure IncomeExpenditure { get; set; }
        public virtual StatementIE StatementIE { get; set; }
    }
}
