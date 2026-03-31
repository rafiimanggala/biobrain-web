using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddContentTreeMetaForVCECurriculum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.InsertData("Courses", new[] { "CourseId", "SubjectCode", "CurriculumCode" }, new object[,]
	        {
		        {Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), 1, 1},
		        {Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), 2, 1},
		        {Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), 3, 1},
	        });

	        migrationBuilder.InsertData("ContentTreeMeta", new[] { "ContentTreeMetaId", "CourseId", "Name", "Depth", "CouldAddEntry", "CouldAddContent" }, new object[,]
			{
				{Guid.Parse("491FC325-F62E-4943-BD74-73EBAB0BFE1B"), Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "Unit", 0, true, false},
				{Guid.Parse("86A10A2B-3ACA-4940-A33D-4963307B8F3F"), Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "Area of Study", 1, true, false},
				{Guid.Parse("AAC810BE-C2DD-4AF0-9D9B-D22613357904"), Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "Key Knoledge", 2, true, false},
				{Guid.Parse("80297E95-C6EC-4D2B-8E71-9C999A1CAFDA"), Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "Topic", 3, true, false},
				{Guid.Parse("63FC8E2A-B9FD-49D7-A70C-CB8A23AD56BF"), Guid.Parse("6553E323-828F-44E2-BFA1-38584CB056F0"), "Level", 4, false, true},
				
				{Guid.Parse("AF4BDC3F-29C9-419D-80D1-69D681AC2059"), Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "Unit", 0, true, false},
				{Guid.Parse("1A7C08CD-C47F-4D5E-8B3B-2EF11DC37730"), Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "Area of Study", 1, true, false},
				{Guid.Parse("22CEA254-804B-48E9-9E6F-DF36678A5A11"), Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "Key Knoledge", 2, true, false},
				{Guid.Parse("E8753AA5-2939-48FD-AB9D-A186F211338F"), Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "Topic", 3, true, false},
				{Guid.Parse("F0CA0665-4DC1-4D79-BD45-1F5849E6FD40"), Guid.Parse("A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46"), "Level", 4, false, true},
				
				{Guid.Parse("C6CE6CAB-C2CD-4528-B465-2E4A1EEDFA45"), Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "Unit", 0, true, false},
				{Guid.Parse("FCABA401-0205-4DEF-A10C-1EC6404F236D"), Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "Area of Study", 1, true, false},
				{Guid.Parse("809BCA6E-1EB6-45BE-A4DC-1ED6621DFDB2"), Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "Key Knoledge", 2, true, false},
				{Guid.Parse("D17244DF-AB06-4443-ABBA-FE1A14BFF3A9"), Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "Topic", 3, true, false},
				{Guid.Parse("22BC3EBC-8B73-41D5-B7CB-97B2D7858C9A"), Guid.Parse("766EF33F-598F-4350-A691-2E27CA25B84D"), "Level", 4, false, true},
			});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
