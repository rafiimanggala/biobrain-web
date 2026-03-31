using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Subjects.GetSubjects
{
    [PublicAPI]
    public class GetSubjectsQuery : ICommand<List<GetSubjectsQuery.Result>>
    {
        [PublicAPI]
        public class Result
        {
            public int SubjectCode { get; set; }
            public string Name { get; set; }
        }


        internal class Handler : CommandHandlerBase<GetSubjectsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<Result>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
            {
                return await Db.Subjects
                               .Select(_ => new Result
                                            {
                                                SubjectCode = _.SubjectCode,
                                                Name = _.Name
                                            })
                               .ToListAsync(cancellationToken);
            }
        }
    }
}
