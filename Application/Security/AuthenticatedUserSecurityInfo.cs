using System;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Domain.Constants;

namespace Biobrain.Application.Security
{
    internal class AuthenticatedUserSecurityInfo : IUserSecurityInfo
    {
        private readonly ISessionContext _sessionContext;

        public AuthenticatedUserSecurityInfo(ISessionContext sessionContext) => _sessionContext = sessionContext;

        public bool IsApplicationAdmin() => _sessionContext.IsUserInRole(Constant.Roles.SystemAdministrator);
        public bool IsStudent() => _sessionContext.IsUserInRole(Constant.Roles.Student);
        public bool IsSchoolAdmin(Guid schoolId) => _sessionContext.IsUserInRole(Constant.Roles.SchoolAdministrator) && _sessionContext.IsFromSchool(schoolId);
        public bool IsSchoolTeacher(Guid schoolId) => _sessionContext.IsUserInRole(Constant.Roles.Teacher) && _sessionContext.IsFromSchool(schoolId);
        public bool IsTeacher() => _sessionContext.IsUserInRole(Constant.Roles.Teacher);
        public bool IsSchoolStudent(Guid schoolId) => _sessionContext.IsUserInRole(Constant.Roles.Student) && _sessionContext.IsFromSchool(schoolId);

        public bool IsAccountOwner(Guid userId) => _sessionContext.GetUserId() == userId;
        public bool IsTeacherAccountOwner(Guid teacherId) => _sessionContext.IsUserInRole(Constant.Roles.Teacher) && IsAccountOwner(teacherId);
        public bool IsStudentAccountOwner(Guid studentId) => _sessionContext.IsUserInRole(Constant.Roles.Student) && IsAccountOwner(studentId);

        public bool HasAccessToSchool(Guid schoolId) => IsApplicationAdmin()|| IsSchoolAdmin(schoolId) || IsSchoolTeacher(schoolId) || IsSchoolStudent(schoolId);
    }
}