namespace Biobrain.Domain.Entities.Course
{
    public class CurriculumEntity
    {
        public int CurriculumCode { get; set; }
        public string Name { get; set; }
        public bool IsGeneric { get; set; }
        public int OrderPriority { get; set; }
    }
}