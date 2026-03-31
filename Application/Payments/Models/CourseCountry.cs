using System;
using System.Collections.Generic;

namespace Biobrain.Application.Payments.Models
{
	public class CourseCountry
	{
		public Guid CourseId { get; init; }
		public List<Country> Countries { get; init; }
	}
}