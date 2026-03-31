using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.MaterialAssignments;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.Quiz;
using Biobrain.Domain.Entities.Student;
using Biobrain.Domain.Entities.Teacher;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Domain.Entities.SiteIdentity
{
    public class UserEntity : IdentityUser<Guid>, IDeletedEntity
    {
        public StudentEntity Student { get; set; }
        public TeacherEntity Teacher { get; set; }

        public ICollection<QuizStudentAssignmentEntity> AssignedQuizzes { get; set; }
        public ICollection<LearningMaterialUserAssignmentEntity> AssignedMaterials { get; set; }
        public ICollection<LearningMaterialAssignmentEntity> AssignedByUserMaterials { get; set; }
        public ICollection<ScheduledPaymentEntity> ScheduledPayments { get; set; }
        public ICollection<UserPaymentDetailsEntity> PaymentDetails { get; set; }

        public string GetFullName()
        {
            if (Teacher != null) return Teacher.GetFullName();
            if (Student != null) return Student.GetFullName();
            return UserName;
        }

        public string GetFirstName()
        {
            if (Teacher != null) return Teacher.FirstName;
            if (Student != null) return Student.FirstName;
            return UserName;
        }

        public int LoginCount { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
