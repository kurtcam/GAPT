using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GAPT.Models
{

    public class ProposalMetadata
    {

    }

    public class GeneralMetadata
    {
        
    }

    public class Ref_DepartmentMetadata
    {

    }

    public class Ref_FacultyMetadata
    {

    }

    public class Department_GeneralMetadata
    {
        public int DepartmentId { get; set; }
        public int GeneralId { get; set; }
        public int Type { get; set; }

    }

    public class Year_UnitMetadata
    {
    }

    public class ExternalReviewMetadata
    {
    }

    public class IncomeExpenditureMetadata
    {
    }

    public class StatementServMetadata
    {
        [Required]
        public string Reservations { get; set; }
    }


    
}