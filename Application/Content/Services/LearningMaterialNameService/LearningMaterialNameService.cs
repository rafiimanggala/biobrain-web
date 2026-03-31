using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.Services
{
    internal class LearningMaterialNameService : ILearningMaterialNameService
    {
        private readonly IDb _db;

        public LearningMaterialNameService(IDb db) => _db = db;

        public async Task<Dictionary<Guid, (ContentTreeEntity node, string fullName)>> GetMaterialsById(IEnumerable<Guid> learningMaterialIds, CancellationToken cancellationToken)
        {
            var learningMaterials = await _db.ContentTree
                                             .Include(_ => _.ParentContentTree)
                                             .ThenInclude(_ => _.ParentContentTree)
                                             .ThenInclude(_ => _.ParentContentTree)
                                             .ThenInclude(_ => _.ParentContentTree)
                                             .ThenInclude(_ => _.ParentContentTree)
                                             .ThenInclude(_ => _.ParentContentTree)
                                             .Where(ContentTreeSpec.ByIds(learningMaterialIds))
                                             .ToListAsync(cancellationToken);

            return learningMaterials.ToDictionary(_ => _.NodeId, _ => (_, BuildName(_)));
        }

        private static string BuildName(ContentTreeEntity node) => node.ParentContentTree != null ? $"{BuildName(node.ParentContentTree)} &gt; {node.Name}" : node.Name;
    }
}