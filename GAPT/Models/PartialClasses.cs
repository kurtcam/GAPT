using System;
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

    [MetadataType(typeof(Ref_DepartmentMetadata))]
    public partial class Ref_Department
    {
        public EndorsementCollab GetCollabEndorsement(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var eres = db.ExternalReview_Endorsement.Where(m => m.ExternalReviewId == proposal.ExternalReviewId).ToList();

            if (eres == null)
            {
                return null;
            }
            else
            {
                foreach (ExternalReview_Endorsement ere in eres) {
                    var endCollab = db.EndorsementCollabs.SingleOrDefault(m => m.Id == ere.EndorsementId && m.DepartmentId == Id);
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

            var erss = db.ExternalReview_Statement.Where(m => m.ExternalReviewId == proposal.ExternalReviewId).ToList();

            if (erss == null)
            {
                return null;
            }
            else
            {
                foreach (ExternalReview_Statement ers in erss)
                {
                    var stmServ = db.StatementServs.SingleOrDefault(m => m.Id == ers.StatementId && m.DepartmentId == Id);
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

    [MetadataType(typeof(StatementServMetadata))]
    public partial class StatementServ
    {
    }
}