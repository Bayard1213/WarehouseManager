using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WarehouseManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "clients",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("clients_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documents_receipt",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_receipt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("documents_receipt_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "measures",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("measures_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("resources_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documents_shipment",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    date_shipment = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("documents_shipment_pkey", x => x.id);
                    table.ForeignKey(
                        name: "documents_shipment_client_id_fkey",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "clients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "balances",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    resource_id = table.Column<int>(type: "integer", nullable: false),
                    measure_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("balances_pkey", x => x.id);
                    table.ForeignKey(
                        name: "balances_measure_id_fkey",
                        column: x => x.measure_id,
                        principalSchema: "public",
                        principalTable: "measures",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "balances_resource_id_fkey",
                        column: x => x.resource_id,
                        principalSchema: "public",
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "receipt_resources",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    document_receipt_id = table.Column<int>(type: "integer", nullable: false),
                    resource_id = table.Column<int>(type: "integer", nullable: false),
                    measure_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("receipt_resources_pkey", x => x.id);
                    table.ForeignKey(
                        name: "receipt_resources_document_receipt_id_fkey",
                        column: x => x.document_receipt_id,
                        principalSchema: "public",
                        principalTable: "documents_receipt",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "receipt_resources_measure_id_fkey",
                        column: x => x.measure_id,
                        principalSchema: "public",
                        principalTable: "measures",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "receipt_resources_resource_id_fkey",
                        column: x => x.resource_id,
                        principalSchema: "public",
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "shipment_resources",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    document_shipment_id = table.Column<int>(type: "integer", nullable: false),
                    resource_id = table.Column<int>(type: "integer", nullable: false),
                    measure_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("shipment_resources_pkey", x => x.id);
                    table.ForeignKey(
                        name: "shipment_resources_document_shipment_id_fkey",
                        column: x => x.document_shipment_id,
                        principalSchema: "public",
                        principalTable: "documents_shipment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "shipment_resources_measure_id_fkey",
                        column: x => x.measure_id,
                        principalSchema: "public",
                        principalTable: "measures",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "shipment_resources_resource_id_fkey",
                        column: x => x.resource_id,
                        principalSchema: "public",
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_balances_measure_id",
                schema: "public",
                table: "balances",
                column: "measure_id");

            migrationBuilder.CreateIndex(
                name: "idx_balances_resource_id",
                schema: "public",
                table: "balances",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "unique_resource_measure",
                schema: "public",
                table: "balances",
                columns: new[] { "resource_id", "measure_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "clients_name_key",
                schema: "public",
                table: "clients",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "documents_receipt_number_key",
                schema: "public",
                table: "documents_receipt",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "documents_shipment_number_key",
                schema: "public",
                table: "documents_shipment",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_shipment_client_id",
                schema: "public",
                table: "documents_shipment",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "measures_name_key",
                schema: "public",
                table: "measures",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_receipt_resources_measure_id",
                schema: "public",
                table: "receipt_resources",
                column: "measure_id");

            migrationBuilder.CreateIndex(
                name: "idx_receipt_resources_resource_id",
                schema: "public",
                table: "receipt_resources",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_resources_document_receipt_id",
                schema: "public",
                table: "receipt_resources",
                column: "document_receipt_id");

            migrationBuilder.CreateIndex(
                name: "resources_name_key",
                schema: "public",
                table: "resources",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_shipment_resources_measure_id",
                schema: "public",
                table: "shipment_resources",
                column: "measure_id");

            migrationBuilder.CreateIndex(
                name: "idx_shipment_resources_resource_id",
                schema: "public",
                table: "shipment_resources",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_resources_document_shipment_id",
                schema: "public",
                table: "shipment_resources",
                column: "document_shipment_id");

            #region CREATE VIEW
            //v_balance
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW v_balance AS
            SELECT
                b.resource_id,
                r.name AS resource_name,
                r.status AS resource_status,
                b.measure_id,
                m.name AS measure_name,
                m.status AS measure_status,
                b.quantity AS balance_quantity
            FROM balances b
            JOIN resources r ON r.id = b.resource_id
            JOIN measures m ON m.id = b.measure_id;
            ");

            // v_receipt_document_resource
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW v_receipt_document_resource AS
            SELECT
                rr.id AS receipt_resource_id,
                rr.document_receipt_id,
                rr.resource_id,
                r.name AS resource_name,
                rr.measure_id,
                m.name AS measure_name,
                rr.quantity
            FROM
                receipt_resources rr
            JOIN resources r ON r.id = rr.resource_id
            JOIN measures m ON m.id = rr.measure_id;
            ");

            // v_shipment_document_resource
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW v_shipment_document_resource AS
            SELECT
                sr.id AS shipment_resource_id,
                sr.document_shipment_id,
                sr.resource_id,
                r.name AS resource_name,
                sr.measure_id,
                m.name AS measure_name,
                sr.quantity
            FROM
                shipment_resources sr
            JOIN resources r ON r.id = sr.resource_id
            JOIN measures m ON m.id = sr.measure_id;
            ");

            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balances",
                schema: "public");

            migrationBuilder.DropTable(
                name: "receipt_resources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "shipment_resources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "documents_receipt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "documents_shipment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "measures",
                schema: "public");

            migrationBuilder.DropTable(
                name: "resources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "public");

            migrationBuilder.Sql(@"DROP VIEW v_balance;");
            migrationBuilder.Sql(@"DROP VIEW v_shipment_document_resource;");
            migrationBuilder.Sql(@"DROP VIEW v_receipt_document_resource;");
        }
    }
}
