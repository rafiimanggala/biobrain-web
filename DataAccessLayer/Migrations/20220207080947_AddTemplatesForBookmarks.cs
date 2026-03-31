using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DataAccessLayer.Migrations
{
    public partial class AddTemplatesForBookmarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.InsertData("Templates", new string[] { "TemplateId", "Type", "Value" },
            //    new object[,]
            //    {
            //        { Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), 2, "Unit {0:i} > AOS {1:i} > {2} > {3} > Level {4:i}" },
            //        { Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), 2, "{0} > {1} > Level {2:i}" }
            //    });


            //migrationBuilder.InsertData("CourseTemplates", new string[] { "CourseTemplateId", "TemplateId", "CourseId" },
            //    new object[,]
            //    {
            //        // Generic
		          //  { Guid.Parse("7E08E548-81C6-412B-8D08-E9755E4DB033"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("16d4754f-4219-4271-b6ce-ee563ee9c0a5") },
            //        { Guid.Parse("DBEEA03C-DD00-4115-A7A2-3E75E8D5340D"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60") },
            //        { Guid.Parse("B154D5E6-A884-4C55-8339-B0964C83E548"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0") },
            //        { Guid.Parse("99C233A4-AE39-4579-867F-156AC46D198A"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("1ebd2e42-2b21-404a-82e2-5190117ca5b6") },
            //        { Guid.Parse("D005C5CF-8360-4BCB-AEFA-8217C187387E"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("22644304-9de3-4493-88a2-c27fe004c35b") },
            //        { Guid.Parse("C02FAC4E-C3BA-44E4-99DC-4238E310EEF8"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("9c79e64f-a8bd-47b0-8434-0738732243e8") },
            //        { Guid.Parse("0964813E-1D32-49C0-9388-EB4A2AD50080"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("cb628740-a88c-4ccb-addc-32307dbb1659") },

            //        // VCE
            //        { Guid.Parse("89F10016-C67B-41D9-9CC1-B6518FF48043"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("6553e323-828f-44e2-bfa1-38584cb056f0") },
            //        { Guid.Parse("05A7BDDE-587F-40B3-943C-01D0B6160BAF"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("a00e14ab-adc8-40b4-ac03-6df1fc9a9b46") },
            //        { Guid.Parse("917F042D-F4CE-416E-876B-DD905086E728"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("766ef33f-598f-4350-a691-2e27ca25b84d") },
            //        { Guid.Parse("F0B31F05-CC87-4EEA-A802-4A15681E0F8C"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("26ced180-9a6d-47a2-93d8-4e3731628d09") },
            //        { Guid.Parse("82946D10-32C0-4ED4-929C-B57EF6BF496C"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("daf966ab-774e-4d52-a806-ff7241217a02") },
            //        { Guid.Parse("B2CF8D39-4202-4DF3-80E5-D2EAA44C8061"), Guid.Parse("D29036C3-4194-4CDA-BCD4-EF50180271FE"), Guid.Parse("3e82da8e-5034-493d-a51b-87098bb35fbb") },

            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
