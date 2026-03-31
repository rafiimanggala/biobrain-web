using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.AccessCodes;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.Glossary;
using Biobrain.Domain.Entities.History;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.MaterialAssignments;
using Biobrain.Domain.Entities.Notifications;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;
using Biobrain.Domain.Entities.Student;
using Biobrain.Domain.Entities.Teacher;
using Biobrain.Domain.Entities.Templates;
using Biobrain.Domain.Entities.User;
using Biobrain.Domain.Entities.UserGuide;
using Biobrain.Domain.Entities.Vouchers;
using Biobrain.Domain.Entities.WhatsNew;
using DataAccessLayer.Base;
using DataAccessLayer.WebAppEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer
{
    public class BiobrainWebContext : IdentityDbContext<UserEntity, 
                                                        RoleEntity, 
                                                        Guid,
                                                        UserClaimEntity,
                                                        UserRoleEntity,
                                                        UserLoginEntity,
                                                        RoleClaimEntity,
                                                        UserTokenEntity>,
                                      IDb
    {
        public virtual DbSet<SchoolEntity> Schools { get; set; }
        public virtual DbSet<SchoolStudentEntity> SchoolStudents { get; set; }
        public virtual DbSet<SchoolTeacherEntity> SchoolTeachers { get; set; }
        public virtual DbSet<SchoolAdminEntity> SchoolAdmins { get; set; }
        public virtual DbSet<SchoolClassEntity> SchoolClasses { get; set; }
        public virtual DbSet<SchoolCourseEntity> SchoolCourses { get; set; }
        public virtual DbSet<SchoolClassStudentEntity> SchoolClassStudents { get; set; }
        public virtual DbSet<TeacherEntity> Teachers { get; set; }
        public virtual DbSet<StudentEntity> Students { get; set; }
        public virtual DbSet<CourseEntity> Courses { get; set; }
        public virtual DbSet<SubjectEntity> Subjects { get; set; }
        public virtual DbSet<CurriculumEntity> Curricula { get; set; }
        public virtual DbSet<StudentCurriculumSetEntity> StudentCurriculumSets { get; set; }
        public virtual DbSet<StudentCurriculumSetEntryEntity> StudentCurriculumSetEntries { get; set; }
        public virtual DbSet<StudentCurriculumSetCountryEntity> StudentCurriculumSetCountries { get; set; }

        public virtual DbSet<ContentVersionEntity> ContentVersion { get; set; }
        public virtual DbSet<ContentTreeEntity> ContentTree { get; set; }
        public virtual DbSet<ContentTreeMetaEntity> ContentTreeMeta { get; set; }
        public virtual DbSet<IconEntity> Icons { get; set; }
        
        public virtual DbSet<PageEntity> Pages { get; set; }
        public virtual DbSet<MaterialEntity> Materials { get; set; }
        public virtual DbSet<PageMaterialEntity> PageMaterials { get; set; }
        public virtual DbSet<BookmarkEntity> Bookmarks { get; set; }

        public virtual DbSet<QuizEntity> Quizzes { get; set; }
        public virtual DbSet<QuizStreakEntity> QuizStreak { get; set; }
        public virtual DbSet<QuizAssignmentEntity> QuizAssignments { get; set; }
        public virtual DbSet<QuizStudentAssignmentEntity> QuizStudentAssignments { get; set; }
        public virtual DbSet<QuizResultEntity> QuizResults { get; set; }
        public virtual DbSet<QuizResultQuestionEntity> QuizResultQuestions { get; set; }
        public virtual DbSet<QuestionEntity> Questions { get; set; }
        public virtual DbSet<QuestionTypeEntity> QuestionTypes { get; set; }
        public virtual DbSet<QuizQuestionEntity> QuizQuestions { get; set; }
        public virtual DbSet<QuizExcludedQuestionEntity> QuizExcludedQuestions { get; set; }
        public virtual DbSet<QuizTemplateEntity> QuizTemplates { get; set; }
        public virtual DbSet<AnswerEntity> Answers { get; set; }
        public virtual DbSet<ExcludedQuestionEntity> ExcludedQuestions { get; set; }

        public virtual DbSet<TermEntity> Terms { get; set; }

        public virtual DbSet<SchoolClassTeacherEntity> SchoolClassTeachers { get; set; }

        public virtual DbSet<EmailMessageEntity> EmailMessageQueue { get; set; }
        public virtual DbSet<LearningMaterialAssignmentEntity> LearningMaterialAssignments { get; set; }
        public virtual DbSet<LearningMaterialUserAssignmentEntity> LearningMaterialUserAssignments { get; set; }

        
        public virtual DbSet<TemplateEntity> Templates { get; set; }
        public virtual DbSet<CourseTemplateEntity> CourseTemplates { get; set; }


        public virtual DbSet<LastPaidScheduledPaymentEntity> LastPaidScheduledPayment { get; set; }
        public virtual DbSet<LastPaymentReviewEntity> LastPaymentReview { get; set; }
        public virtual DbSet<PaymentEntity> Payment { get; set; }
        public virtual DbSet<ScheduledPaymentEntity> ScheduledPayment { get; set; }
        public virtual DbSet<ScheduledPaymentCourseEntity> ScheduledPaymentCourse { get; set; }
        public virtual DbSet<UserPaymentDetailsEntity> UserPaymentDetails { get; set; }
        public virtual DbSet<PromoCodeEntity> PromoCodes { get; set; }
        public virtual DbSet<UserPromoCodeEntity> UserPromoCodes { get; set; }
        public virtual DbSet<VoucherEntity> Vouchers { get; set; }
        public virtual DbSet<UserVoucherEntity> UserVouchers { get; set; }
        public virtual DbSet<UserVoucherTransactionEntity> UserVoucherTransactions { get; set; }

        public virtual DbSet<TempHistoryEntity> TempHistory { get; set; }
        public virtual DbSet<UserPageViewEntity> UserPageViews { get; set; }
        public virtual DbSet<UserLogEntity> UserLogs { get; set; }
        public virtual DbSet<UserSessionEntity> UserSessions { get; set; }
        public virtual DbSet<UserSessionCourseEntity> UserSessionCourses { get; set; }
        public virtual DbSet<UserSessionSchoolEntity> UserSessionSchools { get; set; }

        public virtual DbSet<AccessCodeBatchEntity> AccessCodeBatches { get; set; }
        public virtual DbSet<AccessCodeBatchCourseEntity> AccessCodeBatchCourses { get; set; }
        public virtual DbSet<AccessCodeEntity> AccessCodes { get; set; }
        public virtual DbSet<AccessCodeMilestoneEntity> AccessCodeMilestone { get; set; }

        public virtual DbSet<UserGuideContentTreeEntity> UserGuideContentTree { get; set; }
        public virtual DbSet<UserGuideArticleEntity> UserGuideArticles { get; set; }

        public virtual DbSet<WhatsNewEntity> WhatsNew { get; set; }


		static BiobrainWebContext()
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}

		public BiobrainWebContext(DbContextOptions<BiobrainWebContext> options)
			: base(options)
		{
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplyConfigurations();

            //TODO:
            //AddIsDeletedGlobalFilter(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTrackerHandler();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTrackerHandler();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ChangeTrackerHandler();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ChangeTrackerHandler();
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteByIdAsync<T>(Guid id)
        {
            var entity = await FindAsync(typeof(T), id);
            if (entity == null)
            {
                return false;
            }

            Entry(entity).State = EntityState.Deleted;
            return true;
        }

        private void AddIsDeletedGlobalFilter(ModelBuilder modelBuilder)
        {
            LambdaExpression GetQueryFilterExpression(Type type)
            {
                var paramExpr = Expression.Parameter(type);
                var propAccess = Expression.PropertyOrField(paramExpr, nameof(IDeletedEntity.DeletedAt));

                var constantExpression = Expression.Constant(null);
                var condition = Expression.Equal(propAccess, constantExpression);
                var predicate = Expression.Lambda(condition, paramExpr);

                return predicate;
            }

            var entityTypes = typeof(BiobrainWebContext).GetProperties()
                .Where(p => p.PropertyType.IsGenericType)
                .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments().First())
                .Where(t => t.GetInterfaces().Contains(typeof(IDeletedEntity)));

            foreach (var entityType in entityTypes)
            {
                modelBuilder
                    .Entity(entityType)
                    .HasQueryFilter(GetQueryFilterExpression(entityType));
            }
        }

        private void ChangeTrackerHandler()
        {
            AddCreatedAtTimestamps();
            AddUpdatedAtTimestamps();
            //TODO:
            //AddDeletedAtTimestamps();
        }

        private void AddCreatedAtTimestamps()
            => ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .Any(x =>
                {
                    if (x.Entity is ICreatedEntity cEntity)
                    {
                        cEntity.CreatedAt = DateTime.UtcNow;
                    }
                    if (x.Entity is IUpdatedEntity uEntity)
                    {
                        uEntity.UpdatedAt = DateTime.UtcNow;
                    }

                    return false;
                });

        private void AddUpdatedAtTimestamps()
	        => ChangeTracker.Entries()
		        .Where(x => x.State == EntityState.Modified)
		        .Any(x =>
		        {
			        if (x.Entity is IUpdatedEntity uEntity)
			        {
				        uEntity.UpdatedAt = DateTime.UtcNow;
			        }

			        return false;
		        });

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => await this.Database.BeginTransactionAsync(cancellationToken);
        public async Task CommitTransactionAsync() => await this.Database.CommitTransactionAsync();
        public async Task RollbackTransactionAsync() => await this.Database.RollbackTransactionAsync();

        private void AddDeletedAtTimestamps()
        {
            var deletedEntities = ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Deleted)
                .ToArray();
            for (var i = 0; i < deletedEntities.Length; i++)
            {
                var entity = deletedEntities[i];
                if (entity.Entity is IDeletedEntity dEntity)
                {
                    dEntity.DeletedAt = DateTime.UtcNow;
                    entity.State = EntityState.Modified;
                }
            }
        }
    }
}
