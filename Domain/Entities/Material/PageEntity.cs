using System;
using System.Collections.Generic;
using Biobrain.Domain.Entities.Content;

namespace Biobrain.Domain.Entities.Material
{
	public class PageEntity
	{
		public Guid PageId { get; set; }

		public Guid ContentTreeId { get; set; }
		public ContentTreeEntity ContentTreeNode { get; set; }

		public List<PageMaterialEntity> PageMaterials { get; set; }
	}
}