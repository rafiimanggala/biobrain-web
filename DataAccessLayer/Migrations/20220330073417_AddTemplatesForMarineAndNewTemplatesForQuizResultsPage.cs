using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddTemplatesForMarineAndNewTemplatesForQuizResultsPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.InsertData("Templates", new string[] { "TemplateId", "Type", "Value" },
            //    new object[,]
            //    {
            //        { Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), 3, "Unit {0:i}, AOS {1:i}\n{2}\n{3}\nLevel: {4:i}" },
            //        { Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), 3, "{0}\n{1}\nLevel: {2:i}" }
            //    });


            //migrationBuilder.InsertData("CourseTemplates", new string[] { "CourseTemplateId", "TemplateId", "CourseId" },
            //    new object[,]
            //    {
            //        // Generic
		          //  { Guid.Parse("AE2661C7-1EA7-4163-8C3B-6DBD977108FB"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("16d4754f-4219-4271-b6ce-ee563ee9c0a5") },
            //        { Guid.Parse("E472B8C5-C68B-4C09-A5F2-74821993EE55"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("8e698e20-7961-4a81-a62f-e8255f6cfa60") },
            //        { Guid.Parse("BBA429BB-A818-4233-B014-DA4AC8F33B6C"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("aa91b5fb-b93d-489f-901b-63c4519d63e0") },
            //        { Guid.Parse("197A02C1-6441-4713-91D9-6D72824E0E2C"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("1ebd2e42-2b21-404a-82e2-5190117ca5b6") },
            //        { Guid.Parse("9FB71019-D7B4-48C8-9553-694743D06AAB"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("22644304-9de3-4493-88a2-c27fe004c35b") },
            //        { Guid.Parse("1389DDBD-7F06-4116-9F98-3CAFCB8EC3C6"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("9c79e64f-a8bd-47b0-8434-0738732243e8") },
            //        { Guid.Parse("343B6EC8-C820-452F-A6A9-FBB07A54B1BF"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("cb628740-a88c-4ccb-addc-32307dbb1659") },
            //        // Marine
            //        { Guid.Parse("2E09C529-EFB5-4FE1-AEB9-2BC36E15B3CB"), Guid.Parse("83F2A7FA-A6C2-42C6-B1D4-E3685A1C8248"), Guid.Parse("B93D42C0-A3E7-4D70-A25F-07BA8719F753") },
            //        { Guid.Parse("4E597F5C-8C24-4280-83D9-CA8524D451C5"), Guid.Parse("65C1A940-B547-4782-8E21-B53DAD4E582D"), Guid.Parse("B93D42C0-A3E7-4D70-A25F-07BA8719F753") },

            //        // VCE
            //        { Guid.Parse("CCF477C0-44B5-4A39-AF4E-4E7F258DD5B9"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("6553e323-828f-44e2-bfa1-38584cb056f0") },
            //        { Guid.Parse("FC6CC87F-4A41-46A7-B191-28B572D2D69E"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("a00e14ab-adc8-40b4-ac03-6df1fc9a9b46") },
            //        { Guid.Parse("B0EF27A7-D830-44FD-91E1-D1CA927E4C7F"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("766ef33f-598f-4350-a691-2e27ca25b84d") },
            //        { Guid.Parse("2C5960A3-B824-4165-9EAF-6EDC45B98BAD"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("26ced180-9a6d-47a2-93d8-4e3731628d09") },
            //        { Guid.Parse("8F078960-FCAD-4F39-8295-D856CC40C22A"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("daf966ab-774e-4d52-a806-ff7241217a02") },
            //        { Guid.Parse("2BFCECEA-55C5-48A8-BCD1-8CE2932EA6CA"), Guid.Parse("0C3C8702-946D-4CCA-8239-D3D13681162E"), Guid.Parse("3e82da8e-5034-493d-a51b-87098bb35fbb") },

            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
