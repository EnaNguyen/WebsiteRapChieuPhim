using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectGSMAUI.Api.Migrations
{
    /// <inheritdoc />
    public partial class DLSS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCombo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ghe",
                columns: table => new
                {
                    MaGhe = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    SoHang = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    SoCot = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ghe__3CD3C67B40EF14C7", x => x.MaGhe);
                });

            migrationBuilder.CreateTable(
                name: "GiamGia",
                columns: table => new
                {
                    MaGiamGia = table.Column<int>(type: "int", nullable: false),
                    TenGiamGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GiaTri = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NgayBatDau = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GiamGia__EF9458E404C081C8", x => x.MaGiamGia);
                });

            migrationBuilder.CreateTable(
                name: "KhungGio",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: true),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhungGio__3214EC2782A9AD02", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    TenPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuongGhe = table.Column<int>(type: "int", nullable: true),
                    LoaiPhong = table.Column<int>(type: "int", nullable: true),
                    TinhTrang = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phong__3214EC27E14F6440", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refreshtoken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    IDTaiKhoan = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    TenTaiKhoan = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "char(64)", unicode: false, fixedLength: true, maxLength: 64, nullable: true),
                    TenNguoiDung = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    VaiTro = table.Column<int>(type: "int", nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: true),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: true),
                    Hinh = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    CCCD = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaiKhoan__BC5F907CDEBE2E2D", x => x.IDTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "TheLoaiPhim",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenTheLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TheLoaiP__3214EC2721C910CA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    MaNhap = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    MaGiamGia = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coupon__3214EC27700E1307", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Coupon__MaGiamGi__534D60F1",
                        column: x => x.MaGiamGia,
                        principalTable: "GiamGia",
                        principalColumn: "MaGiamGia");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietCombos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietCombos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietCombos_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietCombos_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhim = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TheLoai = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ThoiLuong = table.Column<int>(type: "int", nullable: true),
                    DaoDien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GioiHanDoTuoi = table.Column<int>(type: "int", nullable: true),
                    NgayKhoiChieu = table.Column<DateOnly>(type: "date", nullable: true),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: true),
                    SoSuatChieu = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phim__3214EC27C9B2D53C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Phim__TheLoai__3B75D760",
                        column: x => x.TheLoai,
                        principalTable: "TheLoaiPhim",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaDatVe = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    TongTien = table.Column<int>(type: "int", nullable: true),
                    MaGiamGia = table.Column<int>(type: "int", nullable: true),
                    NgayXuat = table.Column<DateOnly>(type: "date", nullable: true),
                    TinhTrang = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__835ED13B5131F5A5", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK__HoaDon__MaGiamGi__571DF1D5",
                        column: x => x.MaGiamGia,
                        principalTable: "Coupon",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__HoaDon__MaKhachH__5629CD9C",
                        column: x => x.MaKhachHang,
                        principalTable: "TaiKhoan",
                        principalColumn: "IDTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "HinhAnh",
                columns: table => new
                {
                    ID = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    Phim = table.Column<int>(type: "int", nullable: true),
                    ImageData = table.Column<string>(type: "text", nullable: true),
                    Avatar = table.Column<bool>(type: "bit", nullable: false),
                    PhimNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HinhAnh__3214EC272D4B69CB", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HinhAnh_Phim_PhimNavigationId",
                        column: x => x.PhimNavigationId,
                        principalTable: "Phim",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LichChieu",
                columns: table => new
                {
                    MaLichChieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayChieu = table.Column<DateOnly>(type: "date", nullable: true),
                    GioChieu = table.Column<int>(type: "int", nullable: true),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    GiaVe = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TinhTrang = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LichChie__DC740197B3AF7358", x => x.MaLichChieu);
                    table.ForeignKey(
                        name: "FK__LichChieu__GioCh__440B1D61",
                        column: x => x.GioChieu,
                        principalTable: "KhungGio",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__LichChieu__MaPhi__4316F928",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__LichChieu__MaPho__4222D4EF",
                        column: x => x.MaPhong,
                        principalTable: "Phong",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    ID = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false),
                    Phim = table.Column<int>(type: "int", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Video__3214EC27FC0AF659", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Video__Phim__7C4F7684",
                        column: x => x.Phim,
                        principalTable: "Phim",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ve",
                columns: table => new
                {
                    MaVe = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaLichChieu = table.Column<int>(type: "int", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    TinhTrang = table.Column<int>(type: "int", nullable: true),
                    MaGhe = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaPhimNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ve__2725100F2802FE7C", x => x.MaVe);
                    table.ForeignKey(
                        name: "FK_Ve_Ghe",
                        column: x => x.MaGhe,
                        principalTable: "Ghe",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK_Ve_Phim_MaPhimNavigationId",
                        column: x => x.MaPhimNavigationId,
                        principalTable: "Phim",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Ve__MaLichChieu__49C3F6B7",
                        column: x => x.MaLichChieu,
                        principalTable: "LichChieu",
                        principalColumn: "MaLichChieu");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon",
                columns: table => new
                {
                    MaChiTietHoaDon = table.Column<int>(type: "int", nullable: false),
                    MaVe = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaHoaDon = table.Column<int>(type: "int", nullable: true),
                    Gia = table.Column<int>(type: "int", nullable: true),
                    GheMaGhe = table.Column<string>(type: "char(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietH__CFF2C426609D1D0B", x => x.MaChiTietHoaDon);
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDon_Ghe_GheMaGhe",
                        column: x => x.GheMaGhe,
                        principalTable: "Ghe",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK__ChiTietHo__MaHoa__5AEE82B9",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon");
                    table.ForeignKey(
                        name: "FK__ChiTietHo__MaVe__59FA5E80",
                        column: x => x.MaVe,
                        principalTable: "Ve",
                        principalColumn: "MaVe");
                });

            migrationBuilder.CreateTable(
                name: "DatVe",
                columns: table => new
                {
                    MaDatVe = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    MaVe = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgayDat = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__DatVe__MaKhachHa__4D94879B",
                        column: x => x.MaKhachHang,
                        principalTable: "TaiKhoan",
                        principalColumn: "IDTaiKhoan");
                    table.ForeignKey(
                        name: "FK__DatVe__MaVe__4E88ABD4",
                        column: x => x.MaVe,
                        principalTable: "Ve",
                        principalColumn: "MaVe");
                });

            migrationBuilder.InsertData(
                table: "Ghe",
                columns: new[] { "MaGhe", "SoCot", "SoHang" },
                values: new object[,]
                {
                    { "A1", 1, "A" },
                    { "A10", 10, "A" },
                    { "A11", 11, "A" },
                    { "A12", 12, "A" },
                    { "A13", 13, "A" },
                    { "A14", 14, "A" },
                    { "A15", 15, "A" },
                    { "A16", 16, "A" },
                    { "A2", 2, "A" },
                    { "A3", 3, "A" },
                    { "A4", 4, "A" },
                    { "A5", 5, "A" },
                    { "A6", 6, "A" },
                    { "A7", 7, "A" },
                    { "A8", 8, "A" },
                    { "A9", 9, "A" },
                    { "B1", 1, "B" },
                    { "B10", 10, "B" },
                    { "B11", 11, "B" },
                    { "B12", 12, "B" },
                    { "B13", 13, "B" },
                    { "B14", 14, "B" },
                    { "B15", 15, "B" },
                    { "B16", 16, "B" },
                    { "B2", 2, "B" },
                    { "B3", 3, "B" },
                    { "B4", 4, "B" },
                    { "B5", 5, "B" },
                    { "B6", 6, "B" },
                    { "B7", 7, "B" },
                    { "B8", 8, "B" },
                    { "B9", 9, "B" },
                    { "C1", 1, "C" },
                    { "C10", 10, "C" },
                    { "C11", 11, "C" },
                    { "C12", 12, "C" },
                    { "C13", 13, "C" },
                    { "C14", 14, "C" },
                    { "C15", 15, "C" },
                    { "C16", 16, "C" },
                    { "C2", 2, "C" },
                    { "C3", 3, "C" },
                    { "C4", 4, "C" },
                    { "C5", 5, "C" },
                    { "C6", 6, "C" },
                    { "C7", 7, "C" },
                    { "C8", 8, "C" },
                    { "C9", 9, "C" },
                    { "D1", 1, "D" },
                    { "D10", 10, "D" },
                    { "D11", 11, "D" },
                    { "D12", 12, "D" },
                    { "D13", 13, "D" },
                    { "D14", 14, "D" },
                    { "D15", 15, "D" },
                    { "D16", 16, "D" },
                    { "D2", 2, "D" },
                    { "D3", 3, "D" },
                    { "D4", 4, "D" },
                    { "D5", 5, "D" },
                    { "D6", 6, "D" },
                    { "D7", 7, "D" },
                    { "D8", 8, "D" },
                    { "D9", 9, "D" },
                    { "E1", 1, "E" },
                    { "E10", 10, "E" },
                    { "E11", 11, "E" },
                    { "E12", 12, "E" },
                    { "E13", 13, "E" },
                    { "E14", 14, "E" },
                    { "E15", 15, "E" },
                    { "E16", 16, "E" },
                    { "E2", 2, "E" },
                    { "E3", 3, "E" },
                    { "E4", 4, "E" },
                    { "E5", 5, "E" },
                    { "E6", 6, "E" },
                    { "E7", 7, "E" },
                    { "E8", 8, "E" },
                    { "E9", 9, "E" },
                    { "F1", 1, "F" },
                    { "F10", 10, "F" },
                    { "F11", 11, "F" },
                    { "F12", 12, "F" },
                    { "F13", 13, "F" },
                    { "F14", 14, "F" },
                    { "F15", 15, "F" },
                    { "F16", 16, "F" },
                    { "F2", 2, "F" },
                    { "F3", 3, "F" },
                    { "F4", 4, "F" },
                    { "F5", 5, "F" },
                    { "F6", 6, "F" },
                    { "F7", 7, "F" },
                    { "F8", 8, "F" },
                    { "F9", 9, "F" },
                    { "G1", 1, "G" },
                    { "G10", 10, "G" },
                    { "G11", 11, "G" },
                    { "G12", 12, "G" },
                    { "G13", 13, "G" },
                    { "G14", 14, "G" },
                    { "G15", 15, "G" },
                    { "G16", 16, "G" },
                    { "G2", 2, "G" },
                    { "G3", 3, "G" },
                    { "G4", 4, "G" },
                    { "G5", 5, "G" },
                    { "G6", 6, "G" },
                    { "G7", 7, "G" },
                    { "G8", 8, "G" },
                    { "G9", 9, "G" },
                    { "H1", 1, "H" },
                    { "H10", 10, "H" },
                    { "H11", 11, "H" },
                    { "H12", 12, "H" },
                    { "H13", 13, "H" },
                    { "H14", 14, "H" },
                    { "H15", 15, "H" },
                    { "H16", 16, "H" },
                    { "H2", 2, "H" },
                    { "H3", 3, "H" },
                    { "H4", 4, "H" },
                    { "H5", 5, "H" },
                    { "H6", 6, "H" },
                    { "H7", 7, "H" },
                    { "H8", 8, "H" },
                    { "H9", 9, "H" },
                    { "I1", 1, "I" },
                    { "I10", 10, "I" },
                    { "I11", 11, "I" },
                    { "I12", 12, "I" },
                    { "I13", 13, "I" },
                    { "I14", 14, "I" },
                    { "I15", 15, "I" },
                    { "I16", 16, "I" },
                    { "I2", 2, "I" },
                    { "I3", 3, "I" },
                    { "I4", 4, "I" },
                    { "I5", 5, "I" },
                    { "I6", 6, "I" },
                    { "I7", 7, "I" },
                    { "I8", 8, "I" },
                    { "I9", 9, "I" },
                    { "J1", 1, "J" },
                    { "J10", 10, "J" },
                    { "J11", 11, "J" },
                    { "J12", 12, "J" },
                    { "J13", 13, "J" },
                    { "J14", 14, "J" },
                    { "J15", 15, "J" },
                    { "J16", 16, "J" },
                    { "J2", 2, "J" },
                    { "J3", 3, "J" },
                    { "J4", 4, "J" },
                    { "J5", 5, "J" },
                    { "J6", 6, "J" },
                    { "J7", 7, "J" },
                    { "J8", 8, "J" },
                    { "J9", 9, "J" },
                    { "K1", 1, "K" },
                    { "K10", 10, "K" },
                    { "K11", 11, "K" },
                    { "K12", 12, "K" },
                    { "K13", 13, "K" },
                    { "K14", 14, "K" },
                    { "K15", 15, "K" },
                    { "K16", 16, "K" },
                    { "K2", 2, "K" },
                    { "K3", 3, "K" },
                    { "K4", 4, "K" },
                    { "K5", 5, "K" },
                    { "K6", 6, "K" },
                    { "K7", 7, "K" },
                    { "K8", 8, "K" },
                    { "K9", 9, "K" },
                    { "L1", 1, "L" },
                    { "L10", 10, "L" },
                    { "L11", 11, "L" },
                    { "L12", 12, "L" },
                    { "L13", 13, "L" },
                    { "L14", 14, "L" },
                    { "L15", 15, "L" },
                    { "L16", 16, "L" },
                    { "L2", 2, "L" },
                    { "L3", 3, "L" },
                    { "L4", 4, "L" },
                    { "L5", 5, "L" },
                    { "L6", 6, "L" },
                    { "L7", 7, "L" },
                    { "L8", 8, "L" },
                    { "L9", 9, "L" }
                });

            migrationBuilder.InsertData(
                table: "GiamGia",
                columns: new[] { "MaGiamGia", "GiaTri", "HinhAnh", "MoTa", "NgayBatDau", "NgayKetThuc", "SoLuong", "TenGiamGia" },
                values: new object[] { 1, 20, null, "abc", new DateOnly(2025, 1, 1), new DateOnly(2024, 6, 30), 10, "Khuyến mãi nửa đầu 2025" });

            migrationBuilder.InsertData(
                table: "KhungGio",
                columns: new[] { "ID", "GioBatDau", "GioKetThuc" },
                values: new object[,]
                {
                    { 1, new TimeOnly(9, 15, 0), new TimeOnly(11, 45, 0) },
                    { 2, new TimeOnly(12, 30, 0), new TimeOnly(14, 15, 0) },
                    { 3, new TimeOnly(15, 10, 0), new TimeOnly(17, 25, 0) },
                    { 4, new TimeOnly(18, 5, 0), new TimeOnly(20, 50, 0) },
                    { 5, new TimeOnly(21, 20, 0), new TimeOnly(23, 35, 0) },
                    { 6, new TimeOnly(0, 15, 0), new TimeOnly(2, 40, 0) }
                });

            migrationBuilder.InsertData(
                table: "Phong",
                columns: new[] { "ID", "LoaiPhong", "SoLuongGhe", "TenPhong", "TinhTrang" },
                values: new object[,]
                {
                    { 1, null, 192, "Phòng 1", 1 },
                    { 2, null, 192, "Phòng 2", 1 },
                    { 3, null, 192, "Phòng 3", 1 },
                    { 4, null, 192, "Phòng 4", 1 },
                    { 5, null, 192, "Phòng 5", 1 },
                    { 6, null, 192, "Phòng 6", 1 }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "IDTaiKhoan", "CCCD", "DiaChi", "DiemTichLuy", "Email", "GioiTinh", "Hinh", "MatKhau", "NgayDangKy", "NgaySinh", "SDT", "TenNguoiDung", "TenTaiKhoan", "TrangThai", "VaiTro" },
                values: new object[] { "AD001", "123456789012", "123 Đường ABC, Thành phố XYZ", 0, "nguyenquangquyX@gmail.com", true, null, "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92", new DateOnly(2025, 2, 14), new DateOnly(1999, 5, 19), "0973713274", "Quản trị viên", "Admin", 1, 2 });

            migrationBuilder.InsertData(
                table: "TheLoaiPhim",
                columns: new[] { "ID", "TenTheLoai" },
                values: new object[,]
                {
                    { "TLP001", "Hành Động" },
                    { "TLP002", "Lãng Mạn" },
                    { "TLP003", "Kinh Dị" },
                    { "TLP004", "Hoạt Hình" },
                    { "TLP005", "Khoa Học Viễn Tưởng" }
                });

            migrationBuilder.InsertData(
                table: "Coupon",
                columns: new[] { "ID", "MaGiamGia", "MaNhap", "TrangThai" },
                values: new object[,]
                {
                    { 1, 1, "COUPON001", true },
                    { 2, 1, "COUPON002", true },
                    { 3, 1, "COUPON003", false },
                    { 4, 1, "COUPON004", true },
                    { 5, 1, "COUPON005", false },
                    { 6, 1, "COUPON006", true },
                    { 7, 1, "COUPON007", false },
                    { 8, 1, "COUPON008", true },
                    { 9, 1, "COUPON009", true },
                    { 10, 1, "COUPON010", false }
                });

            migrationBuilder.InsertData(
                table: "Phim",
                columns: new[] { "Id", "DaoDien", "GioiHanDoTuoi", "MoTa", "NgayKetThuc", "NgayKhoiChieu", "SoSuatChieu", "TenPhim", "TheLoai", "ThoiLuong", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Director A", 13, "Action-packed movie about...", new DateOnly(2024, 2, 28), new DateOnly(2024, 1, 15), 50, "Action Movie 1", "TLP001", 120, 1 },
                    { 2, "Director B", 16, "A romantic story about...", new DateOnly(2024, 3, 15), new DateOnly(2024, 2, 1), 40, "Romantic Movie 1", "TLP002", 110, 1 },
                    { 3, "Director C", 18, "A terrifying horror film...", new DateOnly(2024, 3, 31), new DateOnly(2024, 3, 1), 35, "Horror Movie 1", "TLP003", 95, 1 },
                    { 4, "Director D", 0, "An animated adventure for...", new DateOnly(2024, 4, 15), new DateOnly(2024, 3, 20), 15, "Animated Movie 1", "TLP004", 90, 1 },
                    { 5, "Director E", 13, "A sci-fi epic about...", new DateOnly(2024, 4, 30), new DateOnly(2024, 4, 10), 22, "Sci-Fi Movie 1", "TLP005", 135, 1 }
                });

            migrationBuilder.InsertData(
                table: "LichChieu",
                columns: new[] { "MaLichChieu", "GiaVe", "GioChieu", "MaPhim", "MaPhong", "NgayChieu", "TinhTrang" },
                values: new object[,]
                {
                    { 1, 100000m, 1, 1, 1, new DateOnly(2025, 1, 8), true },
                    { 2, 100000m, 2, 2, 2, new DateOnly(2025, 1, 9), true },
                    { 3, 100000m, 3, 3, 3, new DateOnly(2025, 1, 10), true },
                    { 4, 100000m, 4, 4, 4, new DateOnly(2025, 1, 11), true },
                    { 5, 100000m, 5, 5, 5, new DateOnly(2025, 1, 12), true },
                    { 6, 100000m, 6, 1, 6, new DateOnly(2025, 1, 13), true },
                    { 7, 100000m, 1, 2, 1, new DateOnly(2025, 1, 14), true },
                    { 8, 100000m, 2, 3, 2, new DateOnly(2025, 1, 15), true },
                    { 9, 100000m, 3, 4, 3, new DateOnly(2025, 1, 16), true },
                    { 10, 100000m, 4, 5, 4, new DateOnly(2025, 1, 17), true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombos_ComboId",
                table: "ChiTietCombos",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombos_SanPhamId",
                table: "ChiTietCombos",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_GheMaGhe",
                table: "ChiTietHoaDon",
                column: "GheMaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaHoaDon",
                table: "ChiTietHoaDon",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaVe",
                table: "ChiTietHoaDon",
                column: "MaVe");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_MaGiamGia",
                table: "Coupon",
                column: "MaGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_DatVe_MaKhachHang",
                table: "DatVe",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DatVe_MaVe",
                table: "DatVe",
                column: "MaVe");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnh_PhimNavigationId",
                table: "HinhAnh",
                column: "PhimNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaGiamGia",
                table: "HoaDon",
                column: "MaGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKhachHang",
                table: "HoaDon",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_GioChieu",
                table: "LichChieu",
                column: "GioChieu");

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_MaPhim",
                table: "LichChieu",
                column: "MaPhim");

            migrationBuilder.CreateIndex(
                name: "IX_LichChieu_MaPhong",
                table: "LichChieu",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Phim_TheLoai",
                table: "Phim",
                column: "TheLoai");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_MaGhe",
                table: "Ve",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_MaLichChieu",
                table: "Ve",
                column: "MaLichChieu");

            migrationBuilder.CreateIndex(
                name: "IX_Ve_MaPhimNavigationId",
                table: "Ve",
                column: "MaPhimNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Video_Phim",
                table: "Video",
                column: "Phim");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietCombos");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDon");

            migrationBuilder.DropTable(
                name: "DatVe");

            migrationBuilder.DropTable(
                name: "HinhAnh");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "Ve");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "Ghe");

            migrationBuilder.DropTable(
                name: "LichChieu");

            migrationBuilder.DropTable(
                name: "GiamGia");

            migrationBuilder.DropTable(
                name: "KhungGio");

            migrationBuilder.DropTable(
                name: "Phim");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "TheLoaiPhim");
        }
    }
}
