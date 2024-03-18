using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Data.Migrations
{
    /// <inheritdoc />
    public partial class Enable_Change_Tracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER DATABASE CustomerOnboarding
SET CHANGE_TRACKING = ON
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)", true);

            migrationBuilder.Sql(@"
ALTER TABLE Customers
ENABLE CHANGE_TRACKING
WITH (TRACK_COLUMNS_UPDATED = ON)", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE Customers
DISABLE CHANGE_TRACKING", true);

            migrationBuilder.Sql(@"
ALTER DATABASE CustomerOnboarding
SET CHANGE TRACKING = OFF", true);
        }
    }
}
