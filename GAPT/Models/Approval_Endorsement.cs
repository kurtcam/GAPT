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
    
    public partial class Approval_Endorsement
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public int EndorsementId { get; set; }
    
        public virtual Approval Approval { get; set; }
        public virtual EndorsementCollab EndorsementCollab { get; set; }
    }
}