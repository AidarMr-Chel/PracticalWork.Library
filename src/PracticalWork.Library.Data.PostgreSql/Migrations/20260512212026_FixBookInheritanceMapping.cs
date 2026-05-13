using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticalWork.Library.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class FixBookInheritanceMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Books_BookId",
                table: "BookBorrows");

            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Readers_ReaderId",
                table: "BookBorrows");

            migrationBuilder.DropTable(
                name: "EducationalBooks");

            migrationBuilder.DropTable(
                name: "FictionBooks");

            migrationBuilder.DropTable(
                name: "ScientificBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Readers",
                table: "Readers");

            migrationBuilder.DropIndex(
                name: "IX_Readers_PhoneNumber",
                table: "Readers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookBorrows",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_BookId",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_ReaderId",
                table: "BookBorrows");

            migrationBuilder.RenameTable(
                name: "Readers",
                newName: "readers");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "books");

            migrationBuilder.RenameTable(
                name: "BookBorrows",
                newName: "book_borrows");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "readers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "readers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "readers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "readers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "books",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "books",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "CoverImagePath",
                table: "books",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "Authors",
                table: "books",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AddColumn<string>(
                name: "CategoriesOfFiction",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GradeLevel",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResearchField",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "book_type",
                table: "books",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "book_borrows",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<Guid>(
                name: "AbstractBookEntityId",
                table: "book_borrows",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReaderEntityId",
                table: "book_borrows",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_readers",
                table: "readers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_books",
                table: "books",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_book_borrows",
                table: "book_borrows",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "notification_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    borrow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_book_borrows_AbstractBookEntityId",
                table: "book_borrows",
                column: "AbstractBookEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_book_borrows_ReaderEntityId",
                table: "book_borrows",
                column: "ReaderEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_book_borrows_books_AbstractBookEntityId",
                table: "book_borrows",
                column: "AbstractBookEntityId",
                principalTable: "books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_book_borrows_readers_ReaderEntityId",
                table: "book_borrows",
                column: "ReaderEntityId",
                principalTable: "readers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_book_borrows_books_AbstractBookEntityId",
                table: "book_borrows");

            migrationBuilder.DropForeignKey(
                name: "FK_book_borrows_readers_ReaderEntityId",
                table: "book_borrows");

            migrationBuilder.DropTable(
                name: "notification_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_readers",
                table: "readers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_books",
                table: "books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_book_borrows",
                table: "book_borrows");

            migrationBuilder.DropIndex(
                name: "IX_book_borrows_AbstractBookEntityId",
                table: "book_borrows");

            migrationBuilder.DropIndex(
                name: "IX_book_borrows_ReaderEntityId",
                table: "book_borrows");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "readers");

            migrationBuilder.DropColumn(
                name: "CategoriesOfFiction",
                table: "books");

            migrationBuilder.DropColumn(
                name: "GradeLevel",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "books");

            migrationBuilder.DropColumn(
                name: "ResearchField",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "books");

            migrationBuilder.DropColumn(
                name: "book_type",
                table: "books");

            migrationBuilder.DropColumn(
                name: "AbstractBookEntityId",
                table: "book_borrows");

            migrationBuilder.DropColumn(
                name: "ReaderEntityId",
                table: "book_borrows");

            migrationBuilder.RenameTable(
                name: "readers",
                newName: "Readers");

            migrationBuilder.RenameTable(
                name: "books",
                newName: "Books");

            migrationBuilder.RenameTable(
                name: "book_borrows",
                newName: "BookBorrows");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Readers",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Readers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Readers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Books",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "CoverImagePath",
                table: "Books",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "Authors",
                table: "Books",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "BookBorrows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Readers",
                table: "Readers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookBorrows",
                table: "BookBorrows",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EducationalBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeLevel = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationalBooks_Books_Id",
                        column: x => x.Id,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FictionBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriesOfFiction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FictionBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FictionBooks_Books_Id",
                        column: x => x.Id,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScientificBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: true),
                    ResearchField = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScientificBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScientificBooks_Books_Id",
                        column: x => x.Id,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Readers_PhoneNumber",
                table: "Readers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_BookId",
                table: "BookBorrows",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_ReaderId",
                table: "BookBorrows",
                column: "ReaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Books_BookId",
                table: "BookBorrows",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Readers_ReaderId",
                table: "BookBorrows",
                column: "ReaderId",
                principalTable: "Readers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
