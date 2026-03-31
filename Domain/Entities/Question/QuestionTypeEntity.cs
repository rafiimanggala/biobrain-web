namespace Biobrain.Domain.Entities.Question
{
    public class QuestionTypeEntity
    {
        public long QuestionTypeCode { get; set; }

        public string Name { get; set; }

        public bool IsInUse { get; set; }
    }
}