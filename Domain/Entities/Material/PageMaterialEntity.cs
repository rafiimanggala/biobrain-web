using System;

namespace Biobrain.Domain.Entities.Material
{
	public class PageMaterialEntity
	{
		public Guid PageId { get; set; }
		public PageEntity Page { get; set; }
		public Guid MaterialId { get; set; }
		public MaterialEntity Material { get; set; }
		public int Order { get; set; }
	}
}