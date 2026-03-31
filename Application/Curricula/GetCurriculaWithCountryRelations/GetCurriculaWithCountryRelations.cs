using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Curricula.GetCurriculaWithCountryRelations
{
    [PublicAPI]
    public class GetCurriculaWithCountryRelationsQuery : ICommand<List<GetCurriculaWithCountryRelationsQuery.Result>>
    {
        [PublicAPI]
        public class Result
        {
            public int CurriculumCode { get; set; }
            public int OrderPriority { get; set; }
            public string Name { get; set; }
            public List<CountryRelation> AvailableCountries { get; set; }
        }

        [PublicAPI]
        public class CountryRelation
        {
            public string Name { get; set; }
            public string CurriculumName { get; set; }
            public List<string> States { get; set; }
            public bool IsExclude { get; set; }
        }


        internal class Handler(IDb db) : CommandHandlerBase<GetCurriculaWithCountryRelationsQuery, List<Result>>(db)
        {
            public override Task<List<Result>> Handle(GetCurriculaWithCountryRelationsQuery request, CancellationToken cancellationToken)
            {
                return Db.Curricula
                    .Select(_ => new Result
                    {
                        CurriculumCode = _.CurriculumCode,
                        OrderPriority = _.OrderPriority,
                        Name = GetCurriculumDisplayName(_.CurriculumCode),
                        AvailableCountries = GetCountryRelations(_.CurriculumCode)
                    })
                    .ToListAsync(cancellationToken);
            }

            private static List<CountryRelation> GetCountryRelations(int curriculumCode)
            {
                return curriculumCode switch
                {
                    // Generic - null means all countries
                    0 =>
                    [
                        new()
                        {
                            Name = "Australia",
                            States = ["Victoria", "Northern Territory", "NSW", "South Australia"],
                            CurriculumName = "First year university / General interest"
                        },

                        new()
                        {
                            Name = "Australia",
                            States =
                            [
                                "Queensland", "Tasmania", "Western Australia", "Australian Capital Territory"
                            ],
                            CurriculumName = "Year 12 / First year university / General interest"
                        },

                        new() { Name = "Canada", CurriculumName = "Grade 12 / First year university / General interest", },
                        new() { Name = "China", CurriculumName = "First year university / General interest", },
                        new() { Name = "United States", CurriculumName = "Grade 12 / First year university / General interest", },
                        new() { Name = "", CurriculumName = "Grade 12 / First year university / General interest", }
                    ],
                    // VCE
                    1 => [new() { Name = "Australia", States = ["Victoria"] }, new() { Name = "China", }],
                    // SACE
                    2 => [new() { Name = "Australia", States = ["South Australia", "Northern Territory"] }],
                    // IB
                    3 =>
                    [
                        new() { Name = "", },
                        new() { Name = "Canada", CurriculumName = "International Baccalaureate (IB) & 10th Grade Science", },
                        new() { Name = "United States", CurriculumName = "International Baccalaureate (IB) & 10th Grade Science", }
                    ],
                    //return null; //"International Baccalaureate (IB) & 10th Grade Science"
                    // AP
                    4 => [new() { Name = "Australia", IsExclude = true }],
                    // HSC
                    6 => [new() { Name = "Australia", States = ["NSW"] }],
                    _ => []
                };
            }

            private static string GetCurriculumDisplayName(int curriculumCode)
            {
                return curriculumCode switch
                {
                    // Generic
                    0 => "Other",
                    // VCE
                    1 => "VCE & Year 10 Science",
                    // SACE
                    2 => "SACE & Year 10 Science",
                    // IB
                    3 => "International Baccalaureate (IB) & Year 10 Science",
                    // AP
                    4 => "Advanced Placement (AP)",
                    // HSC
                    6 => "HSC & Year 10 Science",
                    _ => ""
                };
            }
        }
    }
}