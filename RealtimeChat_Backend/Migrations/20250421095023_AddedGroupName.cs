﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "GroupChats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "GroupChats");
        }
    }
}
