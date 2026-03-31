using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixInvalidCreatedAtDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const string replacement = "1970-01-01 00:00:00";

            migrationBuilder.Sql($@"
DO $$
DECLARE
    rec RECORD;
    -- Define a TRUE PL/pgSQL variable named 'replacement'
    replacement CONSTANT text := '{replacement}';
BEGIN
    FOR rec IN
        SELECT table_schema, table_name, data_type
        FROM information_schema.columns
        WHERE column_name = 'CreatedAt'
          AND table_schema = 'public'
          AND data_type LIKE 'timestamp%'
    LOOP
        EXECUTE format(
            -- %I → schema, %I → table, %L → quote-literal(replacement), %s → raw data_type
            'UPDATE %I.%I
             SET ""CreatedAt"" = %L::%s
             WHERE EXTRACT(YEAR FROM ""CreatedAt"") = 1;',
             rec.table_schema,
             rec.table_name,
             replacement,
             rec.data_type
        );
    END LOOP;
END
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // do nothing, there is no reason to restore date values which are unrepresentable in .NET
        }
    }
}
