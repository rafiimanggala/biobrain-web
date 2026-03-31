using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Content;

namespace Biobrain.Application.Content.Services
{
    interface ILearningMaterialNameService
    {
        Task<Dictionary<Guid, (ContentTreeEntity node, string fullName)>> GetMaterialsById(IEnumerable<Guid> learningMaterialIds, CancellationToken cancellationToken);
    }
}