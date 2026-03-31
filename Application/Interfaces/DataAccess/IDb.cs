using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
using DataAccessLayer.WebAppEntities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Biobrain.Application.Interfaces.DataAccess
{
    public interface IDb
    {
        DbSet<UserEntity> Users { get; set; }
        DbSet<UserRoleEntity> UserRoles { get; set; }
        DbSet<RoleEntity> Roles { get; set; }
        DbSet<RoleClaimEntity> RoleClaims { get; set; }
        DbSet<UserClaimEntity> UserClaims { get; }


        DbSet<SchoolEntity> Schools { get; }
        DbSet<SchoolStudentEntity> SchoolStudents { get; }
        DbSet<SchoolTeacherEntity> SchoolTeachers { get; }
        DbSet<SchoolAdminEntity> SchoolAdmins { get; }
        DbSet<SchoolClassEntity> SchoolClasses { get; }
        DbSet<SchoolCourseEntity> SchoolCourses { get; }
        DbSet<SchoolClassStudentEntity> SchoolClassStudents { get; }
        DbSet<SchoolClassTeacherEntity> SchoolClassTeachers { get; }
        DbSet<TeacherEntity> Teachers { get; }
        DbSet<StudentEntity> Students { get; }


        DbSet<CourseEntity> Courses { get; }
        DbSet<SubjectEntity> Subjects { get; set; }
        DbSet<CurriculumEntity> Curricula { get; set; }
        DbSet<StudentCurriculumSetEntity> StudentCurriculumSets { get; set; }
        DbSet<StudentCurriculumSetEntryEntity> StudentCurriculumSetEntries { get; set; }
        DbSet<StudentCurriculumSetCountryEntity> StudentCurriculumSetCountries { get; set; }


        DbSet<QuizEntity> Quizzes { get; }
        DbSet<QuizStreakEntity> QuizStreak { get; set; }
        DbSet<QuizAssignmentEntity> QuizAssignments { get; }
        DbSet<QuizStudentAssignmentEntity> QuizStudentAssignments { get; }
        DbSet<QuizResultEntity> QuizResults { get; }
        DbSet<QuizResultQuestionEntity> QuizResultQuestions { get; }
        DbSet<ExcludedQuestionEntity> ExcludedQuestions { get; }

        DbSet<ContentVersionEntity> ContentVersion { get; set; }
        DbSet<ContentTreeEntity> ContentTree { get; set; }
        DbSet<ContentTreeMetaEntity> ContentTreeMeta { get; set; }
        DbSet<IconEntity> Icons { get; set; }

        DbSet<PageEntity> Pages { get; set; }
        DbSet<MaterialEntity> Materials { get; set; }
        DbSet<PageMaterialEntity> PageMaterials { get; set; }
        DbSet<BookmarkEntity> Bookmarks { get; set; }

        DbSet<QuestionEntity> Questions { get; set; }
        DbSet<QuestionTypeEntity> QuestionTypes { get; set; }
        DbSet<QuizQuestionEntity> QuizQuestions { get; set; }
        DbSet<QuizExcludedQuestionEntity> QuizExcludedQuestions { get; set; }
        DbSet<QuizTemplateEntity> QuizTemplates { get; set; }
        DbSet<AnswerEntity> Answers { get; set; }

        DbSet<TermEntity> Terms { get; set; }

        
        DbSet<EmailMessageEntity> EmailMessageQueue { get; }

        DbSet<LearningMaterialAssignmentEntity> LearningMaterialAssignments { get; }
        DbSet<LearningMaterialUserAssignmentEntity> LearningMaterialUserAssignments { get; }

        DbSet<TemplateEntity> Templates { get; set; }
        DbSet<CourseTemplateEntity> CourseTemplates { get; set; }


        DbSet<LastPaidScheduledPaymentEntity> LastPaidScheduledPayment { get; set; }
        DbSet<LastPaymentReviewEntity> LastPaymentReview { get; set; }
        DbSet<PaymentEntity> Payment { get; set; }
        DbSet<ScheduledPaymentEntity> ScheduledPayment { get; set; }
        DbSet<ScheduledPaymentCourseEntity> ScheduledPaymentCourse { get; set; }
        DbSet<UserPaymentDetailsEntity> UserPaymentDetails { get; set; }
        DbSet<PromoCodeEntity> PromoCodes { get; set; }
        DbSet<UserPromoCodeEntity> UserPromoCodes { get; set; }
        DbSet<VoucherEntity> Vouchers { get; set; }
        DbSet<UserVoucherEntity> UserVouchers { get; set; }
        DbSet<UserVoucherTransactionEntity> UserVoucherTransactions { get; set; }

        DbSet<TempHistoryEntity> TempHistory { get; set; }
        DbSet<UserPageViewEntity> UserPageViews { get; set; }
        DbSet<UserLogEntity> UserLogs { get; set; }
        DbSet<UserSessionEntity> UserSessions { get; set; }
        DbSet<UserSessionCourseEntity> UserSessionCourses { get; set; }
        DbSet<UserSessionSchoolEntity> UserSessionSchools { get; set; }

        DbSet<AccessCodeBatchEntity> AccessCodeBatches { get; set; }
        DbSet<AccessCodeBatchCourseEntity> AccessCodeBatchCourses { get; set; }
        DbSet<AccessCodeEntity> AccessCodes { get; set; }
        DbSet<AccessCodeMilestoneEntity> AccessCodeMilestone { get; set; }

        DbSet<UserGuideContentTreeEntity> UserGuideContentTree { get; set; }
        DbSet<UserGuideArticleEntity> UserGuideArticles { get; set; }

        DbSet<WhatsNewEntity> WhatsNew { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);
        ValueTask<TEntity> FindAsync<TEntity>([CanBeNull]object[] keyValues, CancellationToken cancellationToken) where TEntity: class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry Update(object entity);
        EntityEntry Remove(object entity);
        void RemoveRange(IEnumerable<object> entity);
	}
}