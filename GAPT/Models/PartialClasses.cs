﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GAPT.Models
{

    [MetadataType(typeof(ProposalMetadata))]
    public partial class Proposal
    {
        public General GetGeneral()
        {
            GaptDbContext db = new GaptDbContext();
            var general = db.Generals.SingleOrDefault(t => t.Id == GeneralId);
            return general;
        }

        public ProgrammeRationale GetProgrammeRationale()
        {
            GaptDbContext db = new GaptDbContext();
            var pr = db.ProgrammeRationales.SingleOrDefault(t => t.Id == ProgrammeRationaleId);
            return pr;
        }

    }

    [MetadataType(typeof(GeneralMetadata))]
    public partial class General
    {
        public Proposal GetProposal() {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.GeneralId == Id);
            return proposal;
        }


    }

    [MetadataType(typeof(Ref_FacultyMetadata))]
    public partial class Ref_Faculty
    {
        public RecommendationFic GetRecommendation(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var ars = db.Approval_Recommendation.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (ars == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Recommendation ar in ars)
                {
                    var recommendation = db.RecommendationFics.SingleOrDefault(m => m.Id == ar.RecommendationId && m.FacultyId == Id);
                    if (recommendation != null)
                    {
                        return recommendation;
                    }
                }
                return null;
            }
        }
    }

    [MetadataType(typeof(Ref_DepartmentMetadata))]
    public partial class Ref_Department
    {
        public EndorsementCollab GetCollabEndorsement(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var aes = db.Approval_Endorsement.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (aes == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Endorsement ae in aes) {
                    var endCollab = db.EndorsementCollabs.SingleOrDefault(m => m.Id == ae.EndorsementId && m.DepartmentId == Id);
                    if (endCollab != null) {
                        return endCollab;
                    }
                }
                return null;
            }
        }

        public StatementServ GetServStatement(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var apps = db.Approval_Statement.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (apps == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Statement app in apps)
                {
                    var stmServ = db.StatementServs.SingleOrDefault(m => m.Id == app.StatementId && m.DepartmentId == Id);
                    if (stmServ != null)
                    {
                        return stmServ;
                    }
                }
                return null;
            }
        }


    }

    [MetadataType(typeof(Department_GeneralMetadata))]
    public partial class Department_General
    {
        public Department_General(int DepartmentId, int GeneralId, int Type)
        {
            this.DepartmentId = DepartmentId;
            this.GeneralId = GeneralId;
            this.Type = Type;
        }

        public Department_General()
        {
        }


    }

    [MetadataType(typeof(Year_UnitMetadata))]
    public partial class Year_Unit
    {
        public Ref_Unit GetUnit() {
            GaptDbContext db = new GaptDbContext();
            var unit = db.Ref_Unit.SingleOrDefault(m => m.Id == UnitId);
            return unit;
        }
        public Year GetYear()
        {
            GaptDbContext db = new GaptDbContext();
            var year = db.Years.SingleOrDefault(m => m.Id == YearId);
            return year;
        }


    }

    [MetadataType(typeof(ExternalReviewMetadata))]
    public partial class ExternalReview
    {
        public List<Reviewer> GetReviewers()
        {
            GaptDbContext db = new GaptDbContext();
            var reviewerIds = db.Database.SqlQuery<int>("Select ReviewerId From dbo.ExternalReview_Reviewer Where ExternalReviewId = " + Id).ToList();
            List<Reviewer> reviewers = new List<Reviewer>();

            foreach (int id in reviewerIds) {
                var reviewer = db.Reviewers.SingleOrDefault(m => m.Id == id);
                reviewers.Add(reviewer);
            }
            var sorted = reviewers.OrderBy(m => m.Name).ToList();
            return sorted;
        }


    }

    [MetadataType(typeof(IncomeExpenditureMetadata))]
    public partial class IncomeExpenditure
    {
        public List<StatementIE> GetStatements()
        {
            GaptDbContext db = new GaptDbContext();
            var statementsIds = db.Database.SqlQuery<int>("Select StatementId From dbo.IncomeExpenditure_StatementIE Where IncomeExpenditureId = " + Id).ToList();
            List<StatementIE> statements = new List<StatementIE>();

            foreach (int id in statementsIds)
            {
                var statement = db.StatementIEs.SingleOrDefault(m => m.Id == id);
                statements.Add(statement);
            }
            return statements;
        }


    }

    [MetadataType(typeof(StatementServMetadata))]
    public partial class StatementServ
    {
    }
}