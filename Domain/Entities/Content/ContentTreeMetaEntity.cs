using System;
using Biobrain.Domain.Base;

namespace DataAccessLayer.WebAppEntities
{
	public class ContentTreeMetaEntity: IDeletedEntity
	{
		public Guid ContentTreeMetaId { get; set; }
		public Guid CourseId { get; set; }
		
		public string Name { get; set; }
		public long Depth { get; set; }
		public bool CouldAddEntry { get; set; }
		public bool CouldAddContent { get; set; }
		public bool CouldCopyIn { get; set; }
		public int StartIndex { get; set; }
		public bool AutoExpand { get; set; }
		public DateTime? DeletedAt { get; set; }
	}
}