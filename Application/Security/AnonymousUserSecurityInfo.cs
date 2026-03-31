using System;

namespace Biobrain.Application.Security
{
    internal class AnonymousUserSecurityInfo : IUserSecurityInfo
    {
        public bool IsApplicationAdmin() => false;
        public bool IsSchoolAdmin(Guid schoolId) => false;
        public bool IsSchoolTeacher(Guid schoolId) => false;
        public bool IsTeacher() => false;
        public bool IsSchoolStudent(Guid schoolId) => false;

        public bool IsAccountOwner(Guid userId) => false;
        public bool IsTeacherAccountOwner(Guid teacherId) => false;
        public bool IsStudentAccountOwner(Guid studentId) => false;

        public bool HasAccessToSchool(Guid schoolId) => false;
        public bool IsStudent() => false;
    }
}