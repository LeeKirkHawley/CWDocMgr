//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace CWDocMgr.Migrations
//{
//    /// <inheritdoc />
//    public partial class CreateDocumentsTable : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "Documents",
//                columns: table => new
//                {
//                    DocumentModelId = table.Column<int>(type: "INTEGER", nullable: false)
//                        .Annotation("Sqlite:Autoincrement", true),
//                    UserId = table.Column<string>(type: "TEXT", nullable: false),
//                    DocumentName = table.Column<string>(type: "TEXT", nullable: false),
//                    OriginalDocumentName = table.Column<string>(type: "TEXT", nullable: false),
//                    DocumentDate = table.Column<long>(type: "INTEGER", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Documents", x => x.DocumentModelId);
//                });
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "Documents");
//        }
//    }
//}
