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
    
    public partial class Reviewer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reviewer()
        {
            this.ExternalReview_Reviewer = new HashSet<ExternalReview_Reviewer>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Affiliation { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExternalReview_Reviewer> ExternalReview_Reviewer { get; set; }
    }
}
