using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.Student;
using BiobrainWebAPI.Values;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students;

internal interface IJoinStudentToSchoolClassWithAccessCodeService
{
    Task Perform(SchoolClassEntity schoolClass, StudentEntity student, CancellationToken ct);
}


internal sealed class JoinStudentToSchoolClassWithAccessCodeService(IDb db) : IJoinStudentToSchoolClassWithAccessCodeService
{
    /// <inheritdoc />
    public async Task Perform(SchoolClassEntity schoolClass, StudentEntity student, CancellationToken ct)
    {
        //if (Db.Students.CheckLicenseForSchool(schoolClass.School))
        //    throw new NotEnoughStudentsLicensesException();

        SchoolClassStudentEntity existingClass = student.SchoolClasses.FirstOrDefault(_ => _.SchoolClassId == schoolClass.SchoolClassId);
        if (existingClass != null)
            return;

        List<ScheduledPaymentEntity> studentAccessCodeSubscriptions =
            await Db.ScheduledPayment
                    .AsNoTracking()
                    .Include(x => x.ScheduledPaymentCourses)
                    .Where(x => x.UserId == student.StudentId &&
                                (
                                    x.Type == ScheduledPaymentType.AccessCode ||
                                    x.Type == ScheduledPaymentType.Recurring ||
                                    x.Type == ScheduledPaymentType.Voucher
                                ) &&
                                (
                                    x.Status == ScheduledPaymentStatus.Success ||
                                    x.Status == ScheduledPaymentStatus.StoppedByUser
                                ))
                    .ToListAsync(ct);

        if (!studentAccessCodeSubscriptions.Any(x => x.ScheduledPaymentCourses.Any(sp => sp.CourseId == schoolClass.CourseId)))
            throw new ValidationException(Errors.NoAccessCodeForClass);

        SchoolClassStudentEntity schoolClassStudents = new()
                                                       {
                                                           StudentId = student.StudentId,
                                                           SchoolClassId = schoolClass.SchoolClassId
                                                       };

        if (!await Db.SchoolStudents.AnyAsync(_ => _.StudentId == student.StudentId && _.SchoolId == schoolClass.SchoolId, ct))
            await Db.SchoolStudents.AddAsync(new SchoolStudentEntity { SchoolId = schoolClass.SchoolId, StudentId = student.StudentId }, ct);

        await Db.SchoolClassStudents.AddAsync(schoolClassStudents, ct);

        await Db.SaveChangesAsync(ct);
    }

    private IDb Db { get; } = db;
}
