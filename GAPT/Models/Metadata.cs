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

    public class ProgrammeRationaleMetadata
    {
    }

    public class RationaleMetadata
    {
    }

    public class DemandMetadata
    {
    }

    public class ProgrammeOfStudyMetadata
    {
    }

    public class TentativePMetadata
    {
    }

    public class YearMetadata
    {
    }

    public class ReviewerMetadata
    {
    }

    public class Ref_DepartmentMetadata
    {
    }

    public class Ref_UnitMetadata
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int DepartmentId { get; set; }
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

    public class StatementIEMetadata
    {
        [Required]
        public string Upload { get; set; }
    }

    public class StatementServMetadata
    {
        [Required]
        public string Reservations { get; set; }
    }


    
}