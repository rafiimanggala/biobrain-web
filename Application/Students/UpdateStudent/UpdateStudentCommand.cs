using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.UpdateStudent
{
    [PublicAPI]
    public class UpdateStudentCommand : ICommand<UpdateStudentCommand.Result>
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int? CurriculumCode { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateStudentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateStudentCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateStudentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var student = _db.Students.Include(_ => _.Schools).GetSingle(StudentSpec.ById(request.StudentId));
                if (student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId) || user.IsSchoolTeacher(x.SchoolId))) return true;

                if (user.IsStudentAccountOwner(request.StudentId)) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateStudentCommand, Result>
        {
            private readonly IRefreshClaimsService _refreshClaimsService;

            public Handler(IDb db, IRefreshClaimsService refreshClaimsService) : base(db) => _refreshClaimsService = refreshClaimsService;

            public override async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
            {
                var student = await Db.Students.GetSingleAsync(StudentSpec.ById(request.StudentId), cancellationToken);
                student.FirstName = request.FirstName;
                student.LastName = request.LastName;
                student.Country = request.Country;
                student.State = request.State;
                student.CurriculumCode = request.CurriculumCode;

                await Db.SaveChangesAsync(cancellationToken);
                await _refreshClaimsService.RefreshClaims(student.StudentId);
                return new Result();
            }
        }
    }
}
