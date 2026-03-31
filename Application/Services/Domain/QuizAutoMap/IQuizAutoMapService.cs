using System.Threading.Tasks;
using System;

namespace Biobrain.Application.Services.Domain.QuizAutoMap
{
    public interface IQuizAutoMapService
    {
        Task MapQuiz(Guid quizId);
    }
}