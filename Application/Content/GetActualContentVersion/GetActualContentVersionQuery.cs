using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AvailableCourses;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.SiteIdentity;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Content.GetActualContentVersion
{
    [PublicAPI]
    public sealed class GetActualContentVersionQuery : IQuery<List<GetActualContentVersionQuery.Result>>
    {
	    public Guid UserId { get; set; }

	    [PublicAPI]
        public record Result
        {
	        public Guid CourseId { get; set; }
	        public string CourseName { get; set; }
	        public long Version { get; init; }
            public string RelativeFileUrl { get; init; }
        }


        internal class Validator : ValidatorBase<GetActualContentVersionQuery>
        {
	        public Validator(IDb db) : base(db)
	        {
	        }
        }


        internal class PermissionCheck : PermissionCheckBase<GetActualContentVersionQuery>
        {
	        private readonly IDb db;

	        public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(GetActualContentVersionQuery request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudentAccountOwner(request.UserId) && !user.IsTeacherAccountOwner(request.UserId))
			        return false;

		        return true;
	        }
        }

        internal sealed class Handler : QueryHandlerBase<GetActualContentVersionQuery, List<Result>>
        {
	        private readonly UserManager<UserEntity> _userManager;
	        private readonly IAvailableCoursesService _availableCoursesService;

			public Handler(IDb db, UserManager<UserEntity> userManager, IAvailableCoursesService availableCoursesService) : base(db)
			{
				_userManager = userManager;
				_availableCoursesService = availableCoursesService;
			}

            public override async Task<List<Result>> Handle(GetActualContentVersionQuery request,
	            CancellationToken cancellationToken)
            {
	            var user = await Db.Users.Where(UserSpec.ById(request.UserId)).FirstOrDefaultAsync(cancellationToken);
	            var availableCourses = new List<CourseEntity>();
	            if (await _userManager.IsInRoleAsync(user, Constant.Roles.Student))
		            availableCourses =
			            await _availableCoursesService.GetAvailableStudentCourses(request.UserId, cancellationToken);

	            if (await _userManager.IsInRoleAsync(user, Constant.Roles.Teacher))
		            availableCourses =
			            await _availableCoursesService.GetAvailableTeacherCourses(request.UserId, cancellationToken);
                var availableCourseIds = availableCourses.Select(_ => _.CourseId);

				return await Db.ContentVersion
		            .AsNoTracking()
		            .Include(_ => _.Course)
		            .ThenInclude(_ => _.Subject)
		            .Include(_ => _.Course)
		            .ThenInclude(_ => _.Curriculum)
					.Where(_ => availableCourseIds.Contains(_.CourseId))
		            .OrderByDescending(x => x.Version)
		            .Select(_ => new Result
		            {
						CourseId = _.CourseId,
						CourseName = CourseHelper.GetCourseName(_.Course),
						Version = _.Version
		            })
		            .ToListAsync(cancellationToken);
            }
        }
    }
}
