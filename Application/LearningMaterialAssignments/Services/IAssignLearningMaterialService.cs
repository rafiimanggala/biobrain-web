using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;


namespace Biobrain.Application.LearningMaterialAssignments.Services
{
    internal interface IAssignLearningMaterialService
    {
        Task<Result> Assign(Params request, bool forceCreateNew, CancellationToken ct);

        internal record Params
        {
            public Guid SchoolClassId { get; init; }
            public ImmutableList<Guid> StudentIds { get; init; }
            public ImmutableList<Guid> ContentTreeNodeIds { get; init; }
            public DateTime DueDateUtc { get; init; }
            public DateTime DueDateLocal { get; init; }
            public DateTime AssignedDateUtc { get; init; }
            public DateTime AssignedDateLocal { get; init; }
        }


        internal record Result
        {
            public ImmutableList<Row> Rows { get; set; }
            public ImmutableList<Guid> NotAssignedNodeIds { get; set; }

            internal record Row
            {
                public Guid UserId { get; init; }
                public Guid ContentTreeNodeId { get; init; }
                public Guid LearningMaterialUserAssignmentId { get; init; }
            }
        }
    }
}
