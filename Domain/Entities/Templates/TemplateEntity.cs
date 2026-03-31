using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Quiz;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Templates
{
    public class TemplateEntity
    {
        public Guid TemplateId { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
    }
}