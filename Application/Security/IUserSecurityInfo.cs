using System;

namespace Biobrain.Application.Security
{
    internal interface IUserSecurityInfo
    {
        bool IsApplicationAdmin();
        bool IsSchoolAdmin(Guid schoolId);
        bool IsSchoolTeacher(Guid schoolId);
        bool IsTeacher();
        bool IsSchoolStudent(Guid schoolId);
        bool IsAccountOwner(Guid userId);
        bool IsTeacherAccountOwner(Guid teacherId);
        bool IsStudentAccountOwner(Guid studentId);
        bool HasAccessToSchool(Guid schoolId); // TODO: does it match the design?
        bool IsStudent();
    }
}