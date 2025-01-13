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
                    Hinh = table.Column<string>(type: "text", nullable: true),
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
                    ID = table.Column<int>(type: "int", nullable: false),
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
                    ID = table.Column<int>(type: "int", nullable: false),
                    TenPhim = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TheLoai = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK__Phim__3214EC27C9B2D53C", x => x.ID);
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
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HinhAnh__3214EC272D4B69CB", x => x.ID);
                    table.ForeignKey(
                        name: "FK__HinhAnh__Phim__797309D9",
                        column: x => x.Phim,
                        principalTable: "Phim",
                        principalColumn: "ID");
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
                        principalColumn: "ID");
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
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Ve",
                columns: table => new
                {
                    MaVe = table.Column<int>(type: "int", nullable: false),
                    MaLichChieu = table.Column<int>(type: "int", nullable: true),
                    MaPhong = table.Column<int>(type: "int", nullable: true),
                    MaPhim = table.Column<int>(type: "int", nullable: true),
                    TinhTrang = table.Column<int>(type: "int", nullable: true),
                    MaGhe = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true)
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
                    MaGhe = table.Column<int>(type: "int", nullable: true),
                    MaHoaDon = table.Column<int>(type: "int", nullable: true),
                    Gia = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietH__CFF2C426609D1D0B", x => x.MaChiTietHoaDon);
                    table.ForeignKey(
                        name: "FK__ChiTietHo__MaGhe__59FA5E80",
                        column: x => x.MaGhe,
                        principalTable: "Ve",
                        principalColumn: "MaVe");
                    table.ForeignKey(
                        name: "FK__ChiTietHo__MaHoa__5AEE82B9",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon");
                });

            migrationBuilder.CreateTable(
                name: "DatVe",
                columns: table => new
                {
                    MaDatVe = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    MaVe = table.Column<int>(type: "int", nullable: true),
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
                table: "GiamGia",
                columns: new[] { "MaGiamGia", "GiaTri", "HinhAnh", "MoTa", "NgayBatDau", "NgayKetThuc", "SoLuong", "TenGiamGia" },
                values: new object[] { 1, 20, null, "GiamGiaNuaDauNam", new DateOnly(2025, 1, 1), new DateOnly(2024, 6, 30), 10, "Khuyến mãi nửa đầu 2025" });

            migrationBuilder.InsertData(
                table: "KhungGio",
                columns: new[] { "ID", "GioBatDau", "GioKetThuc" },
                values: new object[,]
                {
                    { 1, new TimeOnly(9, 0, 0), new TimeOnly(11, 0, 0) },
                    { 2, new TimeOnly(12, 0, 0), new TimeOnly(14, 0, 0) },
                    { 3, new TimeOnly(15, 0, 0), new TimeOnly(17, 0, 0) },
                    { 4, new TimeOnly(18, 0, 0), new TimeOnly(20, 0, 0) },
                    { 5, new TimeOnly(21, 0, 0), new TimeOnly(23, 0, 0) },
                    { 6, new TimeOnly(0, 0, 0), new TimeOnly(2, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Phong",
                columns: new[] { "ID", "LoaiPhong", "SoLuongGhe", "TenPhong", "TinhTrang" },
                values: new object[,]
                {
                    { 1, null, 112, "Phòng 1", 1 },
                    { 2, null, 112, "Phòng 2", 1 },
                    { 3, null, 112, "Phòng 3", 1 },
                    { 4, null, 112, "Phòng 4", 1 },
                    { 5, null, 112, "Phòng 5", 1 },
                    { 6, null, 112, "Phòng 6", 1 }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "IDTaiKhoan", "CCCD", "DiaChi", "DiemTichLuy", "Email", "GioiTinh", "Hinh", "MatKhau", "NgayDangKy", "NgaySinh", "SDT", "TenNguoiDung", "TenTaiKhoan", "TrangThai", "VaiTro" },
                values: new object[] { "TK001", "123456789012", "123 Đường ABC, Thành phố XYZ", 0, "nguyenquangquyX@gmail.com", true, null, "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92", new DateOnly(2025, 1, 9), new DateOnly(1999, 5, 19), "0973713274", "Quản trị viên", "Admin", 1, 2 });

            migrationBuilder.InsertData(
                table: "TheLoaiPhim",
                columns: new[] { "ID", "TenTheLoai" },
                values: new object[,]
                {
                    { 1, "Hành Động" },
                    { 2, "Lãng Mạn" },
                    { 3, "Kinh Dị" },
                    { 4, "Hoạt Hình" },
                    { 5, "Khoa Học Viễn Tưởng" }
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
                columns: new[] { "ID", "DaoDien", "GioiHanDoTuoi", "MoTa", "NgayKetThuc", "NgayKhoiChieu", "SoSuatChieu", "TenPhim", "TheLoai", "ThoiLuong", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Director A", 13, "Action-packed movie about...", new DateOnly(2024, 2, 28), new DateOnly(2024, 1, 15), 5, "Action Movie 1", 1, 120, 1 },
                    { 2, "Director B", 16, "A romantic story about...", new DateOnly(2024, 3, 15), new DateOnly(2024, 2, 1), 4, "Romantic Movie 1", 2, 110, 1 },
                    { 3, "Director C", 18, "A terrifying horror film...", new DateOnly(2024, 3, 31), new DateOnly(2024, 3, 1), 3, "Horror Movie 1", 3, 95, 1 },
                    { 4, "Director D", 0, "An animated adventure for...", new DateOnly(2024, 4, 15), new DateOnly(2024, 3, 20), 6, "Animated Movie 1", 4, 90, 1 },
                    { 5, "Director E", 13, "A sci-fi epic about...", new DateOnly(2024, 4, 30), new DateOnly(2024, 4, 10), 5, "Sci-Fi Movie 1", 5, 135, 1 }
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
                name: "IX_ChiTietHoaDon_MaGhe",
                table: "ChiTietHoaDon",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaHoaDon",
                table: "ChiTietHoaDon",
                column: "MaHoaDon");

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
                name: "IX_HinhAnh_Phim",
                table: "HinhAnh",
                column: "Phim");

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
