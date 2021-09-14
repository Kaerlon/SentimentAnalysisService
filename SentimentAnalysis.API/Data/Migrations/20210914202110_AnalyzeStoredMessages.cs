using Microsoft.EntityFrameworkCore.Migrations;

namespace SentimentAnalysis.API.Data.Migrations
{
    public partial class AnalyzeStoredMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scores",
                table: "StoredMessages",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scores",
                table: "StoredMessages");
        }
    }
}
