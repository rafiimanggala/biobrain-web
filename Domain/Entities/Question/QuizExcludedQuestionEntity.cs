using System;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Domain.Entities.Question
{
	public class QuizExcludedQuestionEntity
    {
		public Guid QuizId { get; set; }
		public QuizEntity Quiz { get; set; }
		public Guid QuestionId { get; set; }
		public QuestionEntity Question { get; set; }
		public int Order { get; set; }
	}
}