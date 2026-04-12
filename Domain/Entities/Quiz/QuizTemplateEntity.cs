using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Quiz
{
    public class QuizTemplateEntity : ICreatedEntity
    {
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public Guid CreatedByTeacherId { get; set; }
        public Guid CourseId { get; set; }
        public string ContentTreeNodeIdsJson { get; set; } // JSON array of Guid
        public int QuestionCount { get; set; }
        public bool HintsEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
