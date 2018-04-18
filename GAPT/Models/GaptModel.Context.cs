﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GaptDbContext : DbContext
    {
        public GaptDbContext()
            : base("name=GaptDbContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<Approval_Endorsement> Approval_Endorsement { get; set; }
        public virtual DbSet<Approval_Recommendation> Approval_Recommendation { get; set; }
        public virtual DbSet<Approval_Statement> Approval_Statement { get; set; }
        public virtual DbSet<CouncilDecision> CouncilDecisions { get; set; }
        public virtual DbSet<Demand> Demands { get; set; }
        public virtual DbSet<Department_General> Department_General { get; set; }
        public virtual DbSet<EndorsementCollab> EndorsementCollabs { get; set; }
        public virtual DbSet<ExternalReview> ExternalReviews { get; set; }
        public virtual DbSet<ExternalReview_Reviewer> ExternalReview_Reviewer { get; set; }
        public virtual DbSet<General> Generals { get; set; }
        public virtual DbSet<IncomeExpenditure> IncomeExpenditures { get; set; }
        public virtual DbSet<IncomeExpenditure_StatementIE> IncomeExpenditure_StatementIE { get; set; }
        public virtual DbSet<InPrincipal> InPrincipals { get; set; }
        public virtual DbSet<InPrincipal_Council> InPrincipal_Council { get; set; }
        public virtual DbSet<InPrincipal_Pvc> InPrincipal_Pvc { get; set; }
        public virtual DbSet<InPrincipal_Senate> InPrincipal_Senate { get; set; }
        public virtual DbSet<ProgrammeOfStudy> ProgrammeOfStudies { get; set; }
        public virtual DbSet<ProgrammeRationale> ProgrammeRationales { get; set; }
        public virtual DbSet<Proposal> Proposals { get; set; }
        public virtual DbSet<Proposer_General> Proposer_General { get; set; }
        public virtual DbSet<PvcApproval> PvcApprovals { get; set; }
        public virtual DbSet<Rationale> Rationales { get; set; }
        public virtual DbSet<RecommendationFic> RecommendationFics { get; set; }
        public virtual DbSet<Ref_Delivery> Ref_Delivery { get; set; }
        public virtual DbSet<Ref_Department> Ref_Department { get; set; }
        public virtual DbSet<Ref_Faculty> Ref_Faculty { get; set; }
        public virtual DbSet<Ref_Level> Ref_Level { get; set; }
        public virtual DbSet<Ref_Unit> Ref_Unit { get; set; }
        public virtual DbSet<Reviewer> Reviewers { get; set; }
        public virtual DbSet<SenateDecision> SenateDecisions { get; set; }
        public virtual DbSet<StatementIE> StatementIEs { get; set; }
        public virtual DbSet<StatementServ> StatementServs { get; set; }
        public virtual DbSet<TentativeP> TentativePs { get; set; }
        public virtual DbSet<Year> Years { get; set; }
        public virtual DbSet<Year_Unit> Year_Unit { get; set; }
    }
}
