using System;
using System.Collections.Generic;

namespace BiobrainWebAPI.Values
{
	public static class GenericCourses
	{
		public static List<Guid> Ids => new()
		{
			Guid.Parse("16D4754F-4219-4271-B6CE-EE563EE9C0A5"),
			Guid.Parse("AA91B5FB-B93D-489F-901B-63C4519D63E0"),
			Guid.Parse("8E698E20-7961-4A81-A62F-E8255F6CFA60"),
		};
	}
}