using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SchoolClass;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Teachers.UpdateTeacherClasses
{
    [PublicAPI]
    public class UpdateTeacherClassesCommand : ICommand<UpdateTeacherClassesCommand.Result>
    {
        public Guid TeacherId { get; set; }
        public List<Guid> SchoolClassIds { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateTeacherClassesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
                RuleForEach(_ => _.SchoolClassIds).ExistsInTable(Db.SchoolClasses);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateTeacherClassesCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateTeacherClassesCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                
                var teacher = _db.Teachers.Include(x => x.Schools).GetSingle(TeacherSpec.ById(request.TeacherId));
                if (teacher.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId))) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateTeacherClassesCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateTeacherClassesCommand request, CancellationToken cancellationToken)
            {
                var teacher = await Db.Teachers.Include(x => x.Schools).Include(_ => _.Classes).GetSingleAsync(TeacherSpec.ById(request.TeacherId), cancellationToken);
                var schoolClasses = await Db.SchoolClasses.Where(SchoolClassSpec.ByIds(request.SchoolClassIds)).ToListAsync(cancellationToken);

                if (schoolClasses.Any(_ =>  !teacher.IsInSchool(_.SchoolId)))
                    throw new SchoolClassIsAssignedToAnotherSchool();
                
                var newClasses = request.SchoolClassIds.Select(schoolClassId => new SchoolClassTeacherEntity {TeacherId = request.TeacherId, SchoolClassId = schoolClassId});

                Db.SchoolClassTeachers.RemoveRange(teacher.Classes);
                await Db.SchoolClassTeachers.AddRangeAsync(newClasses, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }

        public class SchoolClassIsAssignedToAnotherSchool : BusinessLogicException
        {
            public override Guid ErrorCode => new("3E1287AD-142D-4284-B522-06B0FFF43B06");
        }
    }
}
