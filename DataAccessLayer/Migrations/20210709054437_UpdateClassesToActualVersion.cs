using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateClassesToActualVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
Update public.""SchoolClasses"" AS SC
    SET ""CourseId"" = '26CED180-9A6D-47A2-93D8-4E3731628D09'
    WHERE SC.""Year"" = 12 AND SC.""CourseId"" = '6553E323-828F-44E2-BFA1-38584CB056F0';

Update public.""SchoolClasses"" AS SC
    SET ""CourseId"" = 'DAF966AB-774E-4D52-A806-FF7241217A02'
    WHERE SC.""Year"" = 12 AND SC.""CourseId"" = 'A00E14AB-ADC8-40B4-AC03-6DF1FC9A9B46';

Update public.""SchoolClasses"" AS SC
    SET ""CourseId"" = '3E82DA8E-5034-493D-A51B-87098BB35FBB'
    WHERE SC.""Year"" = 12 AND SC.""CourseId"" = '766EF33F-598F-4350-A691-2E27CA25B84D';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
