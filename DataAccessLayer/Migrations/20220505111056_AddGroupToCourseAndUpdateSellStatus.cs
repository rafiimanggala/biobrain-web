using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DataAccessLayer.Migrations
{
    public partial class AddGroupToCourseAndUpdateSellStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.InsertData("CourseTemplates",
            //    new string[] { "CourseTemplateId", "TemplateId", "CourseId" },
            //    new object[,]
            //    {
            //        { Guid.Parse("B211D4AA-A34A-46D4-B746-0A85E30E805F"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("AF5A7DB1-D7EE-4D4D-9D6A-D237886DDA75") },
            //        { Guid.Parse("06196C01-59E9-4B3B-9AB1-BE188E1D5176"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("AF5A7DB1-D7EE-4D4D-9D6A-D237886DDA75") },

            //    });

            ////Biology 10
            //migrationBuilder.UpdateData("Courses", "CourseId", "cb628740-a88c-4ccb-addc-32307dbb1659", new string[]{"IsForSell", "Group" }, new object[]{true, 2});
            ////Chemistry 10
            //migrationBuilder.UpdateData("Courses", "CourseId", "22644304-9de3-4493-88a2-c27fe004c35b", new string[]{"IsForSell", "Group" }, new object[]{true, 2});
            ////Physics 10
            //migrationBuilder.UpdateData("Courses", "CourseId", "AF5A7DB1-D7EE-4D4D-9D6A-D237886DDA75", new string[]{"IsForSell", "Group" }, new object[]{true, 2});
            ////Psychology 10
            //migrationBuilder.UpdateData("Courses", "CourseId", "1ebd2e42-2b21-404a-82e2-5190117ca5b6", new string[]{"IsForSell", "Group" }, new object[]{true, 2});
            ////Forensics 10
            //migrationBuilder.UpdateData("Courses", "CourseId", "9c79e64f-a8bd-47b0-8434-0738732243e8", new string[]{"IsForSell", "Group" }, new object[]{true, 2});

            ////Biology
            //migrationBuilder.UpdateData("Courses", "CourseId", "16d4754f-4219-4271-b6ce-ee563ee9c0a5", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "26ced180-9a6d-47a2-93d8-4e3731628d09", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "6553e323-828f-44e2-bfa1-38584cb056f0", "Group", 1);
            ////Chemistry
            //migrationBuilder.UpdateData("Courses", "CourseId", "8e698e20-7961-4a81-a62f-e8255f6cfa60", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "a00e14ab-adc8-40b4-ac03-6df1fc9a9b46", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "daf966ab-774e-4d52-a806-ff7241217a02", "Group", 1);
            ////Physics
            //migrationBuilder.UpdateData("Courses", "CourseId", "3e82da8e-5034-493d-a51b-87098bb35fbb", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "766ef33f-598f-4350-a691-2e27ca25b84d", "Group", 1);
            //migrationBuilder.UpdateData("Courses", "CourseId", "aa91b5fb-b93d-489f-901b-63c4519d63e0", "Group", 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "Courses");
        }
    }
}
