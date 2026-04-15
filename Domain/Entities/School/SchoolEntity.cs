using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SchoolClass;

namespace Biobrain.Domain.Entities.School
{
    public class SchoolEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; }
        public int TeachersLicensesNumber { get; set; }
        public int StudentsLicensesNumber { get; set; }
        public bool UseAccessCodes { get; set; }
        public Constant.SchoolStatus Status { get; set; }
        public DateTime? EndDateUtc { get; set; }

        public bool AiDisabled { get; set; }

        // SAML SSO Configuration
        public bool SsoEnabled { get; set; }
        public string SamlEntityId { get; set; }
        public string SamlMetadataUrl { get; set; }
        public string SamlCertificate { get; set; }
        public string SamlLoginUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public ICollection<SchoolClassEntity> Classes { get; set; }
        public ICollection<SchoolTeacherEntity> Teachers { get; set; }
        public ICollection<SchoolAdminEntity> SchoolAdmins { get; set; }
        public ICollection<SchoolStudentEntity> Students { get; set; }
        public ICollection<SchoolCourseEntity> Courses { get; set; }
    }
}
