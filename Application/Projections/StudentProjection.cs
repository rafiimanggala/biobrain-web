using System;
using System.Linq.Expressions;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Domain.Entities.Student;


namespace Biobrain.Application.Projections
{
    public static class StudentProjection
    {
        public static Expression<Func<StudentEntity, QuizAnalyticOutput.Student>> ToQuizAnalyticStudent()
            => _ => new QuizAnalyticOutput.Student
                    {
                        StudentId = _.StudentId,
                        FirstName = _.FirstName,
                        LastName = _.LastName
                    };
    }
}
