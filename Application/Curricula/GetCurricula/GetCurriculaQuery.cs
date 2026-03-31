using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Curricula.GetCurricula
{
    [PublicAPI]
    public class GetCurriculaQuery : ICommand<List<GetCurriculaQuery.Result>>
    {
        [PublicAPI]
        public class Result
        {
            public int CurriculumCode { get; set; }
            public string Name { get; set; }
        }


        internal class Handler : CommandHandlerBase<GetCurriculaQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<Result>> Handle(GetCurriculaQuery request, CancellationToken cancellationToken)
            {
                return await Db.Curricula
                               .Select(_ => new Result
                                            {
                                                CurriculumCode = _.CurriculumCode,
                                                Name = _.Name
                                            })
                               .ToListAsync(cancellationToken);
            }
        }
    }
}