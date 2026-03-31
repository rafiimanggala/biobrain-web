using Biobrain.Domain.Base;


namespace Biobrain.Application.Specifications
{
    public static class DeletedSpec<T> where T:IDeletedEntity
    {
        public static Spec<T> NotDeleted() => new(_ => _.DeletedAt == null);
    }
}
