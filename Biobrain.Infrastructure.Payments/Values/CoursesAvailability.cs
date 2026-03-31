using System;
using System.Collections.Generic;
using Biobrain.Application.Payments.Models;

namespace Biobrain.Infrastructure.Payments.Values
{
	public static class CoursesAvailability
	{
        public static List<CurriculumProductModel> Products => new List<CurriculumProductModel>()
        {
			new CurriculumProductModel{CurriculumCode = 0, Subjects = new List<SubjectProductModel>
            {
				new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("16d4754f-4219-4271-b6ce-ee563ee9c0a5") }}},
				new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60") }}},
				new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0") }}},
            }},
			new CurriculumProductModel{CurriculumCode = 1, Subjects = new List<SubjectProductModel>
            {
				new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("6553e323-828f-44e2-bfa1-38584cb056f0"), CourseName = "Unit 1 & 2"}, new CourseProductModel{CourseId = Guid.Parse("26ced180-9a6d-47a2-93d8-4e3731628d09"), CourseName = "Unit 3 & 4" } }},
				new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("a00e14ab-adc8-40b4-ac03-6df1fc9a9b46"), CourseName = "Unit 1 & 2" }, new CourseProductModel{CourseId = Guid.Parse("daf966ab-774e-4d52-a806-ff7241217a02"), CourseName = "Unit 3 & 4" } }},
				new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("766ef33f-598f-4350-a691-2e27ca25b84d"), CourseName = "Unit 1 & 2" }, new CourseProductModel{CourseId = Guid.Parse("3e82da8e-5034-493d-a51b-87098bb35fbb"), CourseName = "Unit 3 & 4" } }},
            }},
			new CurriculumProductModel{CurriculumCode = 2, Subjects = new List<SubjectProductModel>
            {
				new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("2144de6d-175a-451d-a59c-f6b888de1c61"), CourseName = "Stage 1" }, new CourseProductModel{CourseId = Guid.Parse("9a9784e4-c602-41c3-9c19-b02c28a4ab5e"), CourseName = "Stage 2" } }},
				new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("7ae0382c-d9a2-4cc9-a894-c6a30fdc7b4b"), CourseName = "Stage 1" }, new CourseProductModel{CourseId = Guid.Parse("8a641da7-fe67-49b7-8c5f-ba3e43d7aaec"), CourseName = "Stage 2" } }},
				new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("2c47326c-a8b1-4236-abc0-dfbe8ff6a023"), CourseName = "Stage 1" }, new CourseProductModel{CourseId = Guid.Parse("66494d46-58bd-4385-897f-0aac25ad9812"), CourseName = "Stage 2" } }},
            }},
            new CurriculumProductModel{CurriculumCode = 3, Subjects = new List<SubjectProductModel>
            {
                new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("26efa509-4566-4086-8c84-25ec8db5f0d3"), CourseName = "IB DP (LE2024)" }, new CourseProductModel{CourseId = Guid.Parse("116D01CE-96ED-41BD-BF72-4799124817A1"), CourseName = "IB DP (FE2025)" } }},
                new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("7e35ef61-98db-4c5a-82b7-f1fd514629d5"), CourseName = "IB DP (LE2024)" }, new CourseProductModel{CourseId = Guid.Parse("39483A86-C89C-4F8F-AECE-C9D24F10A0F1"), CourseName = "IB DP (FE2025)" } }},
                new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0"), CourseName = "Senior Physics" } }},
            }},
            new CurriculumProductModel{CurriculumCode = 4, Subjects = new List<SubjectProductModel>
            {
                new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("d5bf4f71-dcf3-4347-88e9-6ec2a45ea82f"), CourseName = "AP Biology" } }},
                new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60"), CourseName = "Senior Chemistry" } }},
                new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0"), CourseName = "Senior Physics" } }},
            }},
            new CurriculumProductModel{CurriculumCode = 6, Subjects = new List<SubjectProductModel>
            {
                new SubjectProductModel{SubjectCode = 1, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("e2efecef-2519-4172-b549-8583503b6d6b"), CourseName = "Year 11" }, new CourseProductModel{CourseId = Guid.Parse("e9ebaa4b-f54d-4654-bfb5-17c8ad16dd76"), CourseName = "Year 12" } }},
                new SubjectProductModel{SubjectCode = 2, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60"), CourseName = "Senior Chemistry" } }},
                new SubjectProductModel{SubjectCode = 3, Courses = new List<CourseProductModel>{new CourseProductModel{CourseId = Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0"), CourseName = "Senior Physics" } }},
            }},
        };

        public static List<string> Year10CoursesCountries = new List<string> { "Australia", "United States of America", "Canada" };
        public static List<int> Year10CoursesCurricula = new List<int> { 0, 1, 2, 3 };

    }
}