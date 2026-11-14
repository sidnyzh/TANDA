using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tanda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderStatusValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                    UPDATE [Orders]
                    SET [Status] = CASE [Status]
                        WHEN 0 THEN 1 -- Quoted
                        WHEN 1 THEN 2 -- Confirmed
                        WHEN 2 THEN 3 -- InProduction
                        WHEN 3 THEN 4 -- Ready  
                        WHEN 4 THEN 5 -- Delivered
                        WHEN 5 THEN 6 -- Cancelled
                        ELSE [Status]
                    END
                    WHERE [Status] BETWEEN 0 AND 5;
                    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                    UPDATE [Orders]
                    SET [Status] = CASE [Status]
                        WHEN 1 THEN 0
                        WHEN 2 THEN 1
                        WHEN 3 THEN 2
                        WHEN 4 THEN 3
                        WHEN 5 THEN 4
                        WHEN 6 THEN 5
                        ELSE [Status]
                    END
                    WHERE [Status] BETWEEN 1 AND 6;
                    """);
        }
    }
}
