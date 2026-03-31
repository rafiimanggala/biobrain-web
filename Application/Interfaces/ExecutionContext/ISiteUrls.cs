using System;

namespace Biobrain.Application.Interfaces.ExecutionContext
{
    public interface ISiteUrls
    {
        Uri Login();
        Uri SetPassword(string login, string token);
        Uri SetPasswordAfterRegistration(string login, string token);
        Uri PerformQuiz(Guid quizStudentAssignmentId, Guid courseId);
        Uri PerformAssignedLearningMaterial(Guid learningMaterialUserAssignmentId);
    }
}