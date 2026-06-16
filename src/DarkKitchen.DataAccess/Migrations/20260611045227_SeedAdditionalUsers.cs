using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdditionalUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { 2, "rodrigo.admin@darkkitchen.com", "Rodrigo", "Rey", "$2a$11$r.Zi1wj4HWTZaln2O7gf0eR8bDzJLx8ep5ZZmVLQK5dFAs/I5NBee", "099111333", "Admin" },
                    { 3, "roman.dispatcher@darkkitchen.com", "Roman", "Ferrero", "$2a$11$fl/TH4TtU.chzdOLfNUlnOm1gLgvDllQhxCf0zxzfO1WiVRIwyxEG", "099100200", "Dispatcher" },
                    { 4, "santiago.dispatcher@darkkitchen.com", "Santiago", "Pedetti", "$2a$11$u7j/7G/u3HPyBCh4jhUKD.DbaYI3Hn0jwAMJB7TxbYAbhNDYySuQS", "099300400", "Dispatcher" },
                    { 5, "maia@gmail.com", "Maia", "Terzaghi", "$2a$11$TAubO9G4nmnElrk1iubOJOhrLe8csJUvuhRk9HhioJ7jUPLVgKZAC", "099111222", "Client" },
                    { 6, "pilar@gmail.com", "Pilar", "Fraschini", "$2a$11$Xn6cqMRr/cTDgoidQnfRf.54veJk3Ofzs..gjPRFh4lRVkclc2LVq", "099333444", "Client" },
                    { 8, "francisco@gmail.com", "Francisco", "Suarez", "$2a$11$bT4q7wvuhLF6iQrOCQojr.eXUaLhZZy5C1SjeBs/GYQYmuovcYDsi", "099777888", "Client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
