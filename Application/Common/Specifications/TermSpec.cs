using Biobrain.Domain.Entities.Glossary;


namespace Biobrain.Application.Specifications
{
    public static class TermSpec
    {
        public static Spec<TermEntity> ForSubject(int subjectCode) => new(_ => _.SubjectCode == subjectCode);
    }
}
