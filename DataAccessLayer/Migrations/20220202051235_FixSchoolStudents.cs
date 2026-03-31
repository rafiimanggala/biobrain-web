using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class FixSchoolStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

	        migrationBuilder.Sql("INSERT INTO \"SchoolStudents\" (\"SchoolId\", \"StudentId\", \"CreatedAt\") " +
                                 "  SELECT SC.\"SchoolId\", ST.\"StudentId\",'2022-01-13 10:08:42.096217' AS  \"CreatedAt\" " +
	                             "      FROM \"Students\" ST " +
                                 "      LEFT JOIN \"SchoolClassStudents\" SCS ON SCS.\"StudentId\" = ST.\"StudentId\" " +
                                 "      LEFT JOIN \"SchoolClasses\" SC ON SCS.\"SchoolClassId\" = SC.\"SchoolClassId\" " +
                                 "      WHERE SC.\"SchoolId\" IS NOT NULL" +
                                 "          AND SC.\"SchoolId\" NOT IN (SELECT SS.\"SchoolId\" FROM \"SchoolStudents\" SS WHERE ST.\"StudentId\" = SS.\"StudentId\")");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
