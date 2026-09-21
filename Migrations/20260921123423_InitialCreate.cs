using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoom.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Building = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Equipment = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    StudentNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudySessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    HostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Topic = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaxParticipants = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessions_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySessions_Student_HostId",
                        column: x => x.HostId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudySessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_StudySessions_StudySessionId",
                        column: x => x.StudySessionId,
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudySessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participation_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participation_StudySessions_StudySessionId",
                        column: x => x.StudySessionId,
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_StudentId",
                table: "Message",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_StudySessionId",
                table: "Message",
                column: "StudySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_StudentId",
                table: "Participation",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_StudySessionId",
                table: "Participation",
                column: "StudySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_HostId",
                table: "StudySessions",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_RoomId",
                table: "StudySessions",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Participation");

            migrationBuilder.DropTable(
                name: "StudySessions");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
