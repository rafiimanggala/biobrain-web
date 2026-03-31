using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Entities.User;
using BiobrainWebAPI.Values;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.TrackSession
{
    public class TrackSessionService : ITrackSessionService
    {
        private readonly IDb _db;

        public TrackSessionService(IDb db) => _db = db;

        public async Task TrackSession(Guid userId, Guid? courseId, Guid? schoolId, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;

            var userSession = await _db.UserSessions
                .Include(x => x.Courses)
                .Include(x => x.Schools)
                .Where(x => x.UserId == userId)
                .Where(x => schoolId == null || x.Schools.Any(_ => _.SchoolId == schoolId))
                .OrderByDescending(x => x.LastTrack)
                .FirstOrDefaultAsync(cancellationToken);

            if (userSession == null || (date - userSession.LastTrack).TotalMinutes > AppSettings.MinutesForNewSession)
            {
                var session = new UserSessionEntity
                {
                    UserSessionId = new Guid(),
                    UserId = userId,
                    StartAt = date,
                    LastTrack = date,
                };

                if (courseId != null)
                    session.Courses = new List<UserSessionCourseEntity>
                    {
                        new() { CourseId = courseId.Value, UserSessionId = session.UserSessionId }
                    };

                if (schoolId != null)
                    session.Schools = new List<UserSessionSchoolEntity>
                    {
                        new() { SchoolId = schoolId.Value, UserSessionId = session.UserSessionId }
                    };

                await _db.AddAsync(session, cancellationToken);
            }
            else
            {
                userSession.LastTrack = date;

                if (courseId != null && userSession.Courses.All(_ => _.CourseId != courseId))
                    _db.UserSessionCourses.Add(new UserSessionCourseEntity
                        { CourseId = courseId.Value, UserSessionId = userSession.UserSessionId });

                if (schoolId != null && userSession.Schools.All(_ => _.SchoolId != schoolId))
                    _db.UserSessionSchools.Add(new UserSessionSchoolEntity
                        { SchoolId = schoolId.Value, UserSessionId = userSession.UserSessionId });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task PageView(Guid userId, string pagePath, Guid? courseId, Guid? schoolId, CancellationToken cancellationToken)
        {
            await _db.UserPageViews.AddAsync(new UserPageViewEntity
            {
                UserId = userId,
                PagePath = pagePath,
                CourseId = courseId,
                SchoolId = schoolId
            }, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}