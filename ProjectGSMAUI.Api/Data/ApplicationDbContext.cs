using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        public virtual DbSet<Coupon> Coupons { get; set; }

        public virtual DbSet<Ghe> Ghes { get; set; }

        public virtual DbSet<GiamGia> GiamGia { get; set; }

        public virtual DbSet<HinhAnh> HinhAnhs { get; set; }

        public virtual DbSet<HoaDon> HoaDons { get; set; }

        public virtual DbSet<KhungGio> KhungGios { get; set; }

        public virtual DbSet<LichChieu> LichChieus { get; set; }

        public virtual DbSet<Phim> Phims { get; set; }

        public virtual DbSet<Phong> Phongs { get; set; }

        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

        public virtual DbSet<TheLoaiPhim> TheLoaiPhims { get; set; }

        public virtual DbSet<Ve> Ves { get; set; }

        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Combo> Combos { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<ChiTietCombo> ChiTietCombos { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public static List<Ghe> GetSeats()
        {
            var seats = new List<Ghe>();
            var rows = "ABCDEFGHIJKL".ToCharArray();
            var columns = 16;

            foreach (var row in rows)
            {
                for (int col = 1; col <= columns; col++)
                {
                    seats.Add(new Ghe { MaGhe = $"{row}{col}", SoHang = row.ToString(), SoCot = col });
                }
            }
            return seats;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.TokenID).IsRequired();
                entity.Property(e => e.refreshtoken).IsRequired();
            });
            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.HasKey(e => e.MaChiTietHoaDon).HasName("PK__ChiTietH__CFF2C426609D1D0B");

                entity.ToTable("ChiTietHoaDon");

                entity.Property(e => e.MaChiTietHoaDon).ValueGeneratedOnAdd();

                entity.HasOne(d => d.MaVeNavigation).WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaVe)
                    .HasConstraintName("FK__ChiTietHo__MaVe__59FA5E80");

                entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaHoaDon)
                    .HasConstraintName("FK__ChiTietHo__MaHoa__5AEE82B9");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Coupon__3214EC27700E1307");

                entity.ToTable("Coupon");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.MaNhap)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.MaGiamGia)
                    .HasConstraintName("FK__Coupon__MaGiamGi__534D60F1");
            });
            modelBuilder.Entity<Ghe>(entity =>
            {
                entity.HasKey(e => e.MaGhe).HasName("PK__Ghe__3CD3C67B40EF14C7");

                entity.ToTable("Ghe");

                entity.Property(e => e.MaGhe)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.SoHang)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<GiamGia>(entity =>
            {
                entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E404C081C8");

                entity.Property(e => e.MaGiamGia).ValueGeneratedNever();
                entity.Property(e => e.TenGiamGia).HasMaxLength(100);
            });

            modelBuilder.Entity<HinhAnh>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__HinhAnh__3214EC272D4B69CB");

                entity.ToTable("HinhAnh");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("ID");
                entity.Property(e => e.ImageData).HasColumnType("text");

            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B5131F5A5");

                entity.ToTable("HoaDon");

                entity.Property(e => e.MaHoaDon).ValueGeneratedNever();
                entity.Property(e => e.MaKhachHang)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();
              
                entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaGiamGia)
                    .HasConstraintName("FK__HoaDon__MaGiamGi__571DF1D5");

                entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKhachHang)
                    .HasConstraintName("FK__HoaDon__MaKhachH__5629CD9C");             
            });

            modelBuilder.Entity<KhungGio>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__KhungGio__3214EC2782A9AD02");

                entity.ToTable("KhungGio");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<LichChieu>(entity =>
            {
                entity.HasKey(e => e.MaLichChieu).HasName("PK__LichChie__DC740197B3AF7358");

                entity.ToTable("LichChieu");

                entity.Property(e => e.MaLichChieu).ValueGeneratedOnAdd();
                entity.Property(e => e.GiaVe).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.GioChieuNavigation).WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.GioChieu)
                    .HasConstraintName("FK__LichChieu__GioCh__440B1D61");

                entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.MaPhim)
                    .HasConstraintName("FK__LichChieu__MaPhi__4316F928");

                entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.MaPhong)
                    .HasConstraintName("FK__LichChieu__MaPho__4222D4EF");
            });


            modelBuilder.Entity<Phim>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Phim__3214EC27C9B2D53C");

                entity.ToTable("Phim");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DaoDien).HasMaxLength(50);
                entity.Property(e => e.MoTa).HasColumnType("text");
                entity.Property(e => e.TenPhim).HasMaxLength(100);

                entity.HasOne(d => d.TheLoaiNavigation).WithMany(p => p.Phims)
                    .HasForeignKey(d => d.TheLoai)
                    .HasConstraintName("FK__Phim__TheLoai__3B75D760");
            });

            modelBuilder.Entity<Phong>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Phong__3214EC27E14F6440");

                entity.ToTable("Phong");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.TenPhong).HasMaxLength(50);
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.IdtaiKhoan).HasName("PK__TaiKhoan__BC5F907CDEBE2E2D");

                entity.ToTable("TaiKhoan");

                entity.Property(e => e.IdtaiKhoan)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("IDTaiKhoan");
                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("CCCD");
                entity.Property(e => e.DiaChi).HasMaxLength(100);
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Hinh).HasColumnType("VARBINARY(MAX)");
                entity.Property(e => e.MatKhau)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Sdt)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("SDT");
                entity.Property(e => e.TenNguoiDung).HasMaxLength(50);
                entity.Property(e => e.TenTaiKhoan)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<TheLoaiPhim>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TheLoaiP__3214EC2721C910CA");

                entity.ToTable("TheLoaiPhim");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.TenTheLoai).HasMaxLength(50);
            });

            modelBuilder.Entity<Ve>(entity =>
            {
                entity.HasKey(e => e.MaVe).HasName("PK__Ve__2725100F2802FE7C");

                entity.ToTable("Ve");

                entity.Property(e => e.MaVe).ValueGeneratedOnAdd();
                entity.Property(e => e.MaGhe)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.Ves)
                    .HasForeignKey(d => d.MaGhe)
                    .HasConstraintName("FK_Ve_Ghe");

                entity.HasOne(d => d.MaLichChieuNavigation).WithMany(p => p.Ves)
                    .HasForeignKey(d => d.MaLichChieu)
                    .HasConstraintName("FK__Ve__MaLichChieu__49C3F6B7");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Video__3214EC27FC0AF659");

                entity.ToTable("Video");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("ID");
                entity.Property(e => e.Link).HasColumnType("text");

                entity.HasOne(d => d.PhimNavigation).WithMany(p => p.Videos)
                    .HasForeignKey(d => d.Phim)
                    .HasConstraintName("FK__Video__Phim__7C4F7684");
            });
            modelBuilder.Entity<ChiTietCombo>()
    .HasOne(ct => ct.Combo)
    .WithMany(c => c.ChiTietCombos)
    .HasForeignKey(ct => ct.ComboId);


            modelBuilder.Entity<ChiTietCombo>()
                .HasOne(ct => ct.SanPham)
                .WithMany(sp => sp.ChiTietCombos)
                .HasForeignKey(ct => ct.SanPhamId);
            modelBuilder.Entity<GiamGia>().HasData(
               new GiamGia
               {
                   MaGiamGia = 1,
                   TenGiamGia = "Khuyến mãi nửa đầu 2025",
                   GiaTri = 20,
                   MoTa = "abc",
                   NgayBatDau = DateOnly.FromDateTime(new DateTime(2025, 1, 1)),
                   NgayKetThuc = DateOnly.FromDateTime(new DateTime(2024, 6, 30)),
                   SoLuong = 10,
               }
           );
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    MaNhap = "COUPON001",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 2,
                    MaNhap = "COUPON002",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 3,
                    MaNhap = "COUPON003",
                    MaGiamGia = 1,
                    TrangThai = false
                },
                new Coupon
                {
                    Id = 4,
                    MaNhap = "COUPON004",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 5,
                    MaNhap = "COUPON005",
                    MaGiamGia = 1,
                    TrangThai = false
                },
                new Coupon
                {
                    Id = 6,
                    MaNhap = "COUPON006",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 7,
                    MaNhap = "COUPON007",
                    MaGiamGia = 1,
                    TrangThai = false
                },
                new Coupon
                {
                    Id = 8,
                    MaNhap = "COUPON008",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 9,
                    MaNhap = "COUPON009",
                    MaGiamGia = 1,
                    TrangThai = true
                },
                new Coupon
                {
                    Id = 10,
                    MaNhap = "COUPON010",
                    MaGiamGia = 1,
                    TrangThai = false
                }
            );
            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan
                {
                    IdtaiKhoan = "AD001",
                    TenTaiKhoan = "Admin",
                    MatKhau = "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92",
                    TenNguoiDung = "Quản trị viên",
                    Email = "nguyenquangquyX@gmail.com",
                    Sdt = "0973713274",
                    VaiTro = 2,
                    NgaySinh = DateOnly.FromDateTime(new DateTime(1999, 5, 19)),
                    NgayDangKy = DateOnly.FromDateTime(DateTime.Now),
                    TrangThai = 1,
                    DiemTichLuy = 0,
                    Hinh = null,
                    Cccd = "123456789012",
                    GioiTinh = true,
                    DiaChi = "123 Đường ABC, Thành phố XYZ"
                }
            );
            modelBuilder.Entity<TheLoaiPhim>().HasData(
                new TheLoaiPhim { Id = "TLP001", TenTheLoai = "Hành Động" },
                new TheLoaiPhim { Id = "TLP002", TenTheLoai = "Lãng Mạn" },
                new TheLoaiPhim { Id = "TLP003", TenTheLoai = "Kinh Dị" },
                new TheLoaiPhim { Id = "TLP004", TenTheLoai = "Hoạt Hình" },
                new TheLoaiPhim { Id = "TLP005", TenTheLoai = "Khoa Học Viễn Tưởng" }
            );
            modelBuilder.Entity<Phim>().HasData(
                new Phim
                {
                    Id = 1,
                    DaoDien = "Director A",
                    GioiHanDoTuoi = 13,
                    MoTa = "Action-packed movie about...",
                    NgayKetThuc = new DateOnly(2024, 2, 28),
                    NgayKhoiChieu = new DateOnly(2024, 1, 15),
                    SoSuatChieu = 50,
                    TenPhim = "Action Movie 1",
                    TheLoai = "TLP001", // Action
                    ThoiLuong = 120,
                    TrangThai = 1
                },
                new Phim
                {
                    Id = 2,
                    DaoDien = "Director B",
                    GioiHanDoTuoi = 16,
                    MoTa = "A romantic story about...",
                    NgayKetThuc = new DateOnly(2024, 3, 15),
                    NgayKhoiChieu = new DateOnly(2024, 2, 1),
                    SoSuatChieu = 40,
                    TenPhim = "Romantic Movie 1",
                    TheLoai = "TLP002", // Romantic
                    ThoiLuong = 110,
                    TrangThai = 1

                },
                new Phim
                {
                    Id = 3,
                    DaoDien = "Director C",
                    GioiHanDoTuoi = 18,
                    MoTa = "A terrifying horror film...",
                    NgayKetThuc = new DateOnly(2024, 3, 31),
                    NgayKhoiChieu = new DateOnly(2024, 3, 1),
                    SoSuatChieu = 35,
                    TenPhim = "Horror Movie 1",
                    TheLoai = "TLP003", // Horror
                    ThoiLuong = 95,
                    TrangThai = 1
                },
                new Phim
                {
                    Id = 4,
                    DaoDien = "Director D",
                    GioiHanDoTuoi = 0, // All ages
                    MoTa = "An animated adventure for...",
                    NgayKetThuc = new DateOnly(2024, 4, 15),
                    NgayKhoiChieu = new DateOnly(2024, 3, 20),
                    SoSuatChieu = 15,
                    TenPhim = "Animated Movie 1",
                    TheLoai = "TLP004", // Animated
                    ThoiLuong = 90,
                    TrangThai = 1
                },
                new Phim
                {
                    Id = 5,
                    DaoDien = "Director E",
                    GioiHanDoTuoi = 13,
                    MoTa = "A sci-fi epic about...",
                    NgayKetThuc = new DateOnly(2024, 4, 30),
                    NgayKhoiChieu = new DateOnly(2024, 4, 10),
                    SoSuatChieu = 22,
                    TenPhim = "Sci-Fi Movie 1",
                    TheLoai = "TLP005", // Sci-fi
                    ThoiLuong = 135,
                    TrangThai = 1
                }

            );

            modelBuilder.Entity<Phong>().HasData(
                new Phong { Id = 1, TenPhong = "Phòng 1", SoLuongGhe = 192, TinhTrang = 1 },
                new Phong { Id = 2, TenPhong = "Phòng 2", SoLuongGhe = 192, TinhTrang = 1 },
                new Phong { Id = 3, TenPhong = "Phòng 3", SoLuongGhe = 192, TinhTrang = 1 },
                new Phong { Id = 4, TenPhong = "Phòng 4", SoLuongGhe = 192, TinhTrang = 1 },
                new Phong { Id = 5, TenPhong = "Phòng 5", SoLuongGhe = 192, TinhTrang = 1 },
                new Phong { Id = 6, TenPhong = "Phòng 6", SoLuongGhe = 192, TinhTrang = 1 }

            );//16 cột
            modelBuilder.Entity<KhungGio>().HasData(
                new KhungGio { Id = 1, GioBatDau = new TimeOnly(9, 15), GioKetThuc = new TimeOnly(11, 45) },
                new KhungGio { Id = 2, GioBatDau = new TimeOnly(12, 30), GioKetThuc = new TimeOnly(14, 15) },
                new KhungGio { Id = 3, GioBatDau = new TimeOnly(15, 10), GioKetThuc = new TimeOnly(17, 25) },
                new KhungGio { Id = 4, GioBatDau = new TimeOnly(18, 5), GioKetThuc = new TimeOnly(20, 50) },
                new KhungGio { Id = 5, GioBatDau = new TimeOnly(21, 20), GioKetThuc = new TimeOnly(23, 35) },
                new KhungGio { Id = 6, GioBatDau = new TimeOnly(0, 15), GioKetThuc = new TimeOnly(2, 40) }
            );
            var seats = GetSeats();
            modelBuilder.Entity<Ghe>().HasData(seats);
            modelBuilder.Entity<LichChieu>().HasData(
                new LichChieu { MaLichChieu = 1, NgayChieu = new DateOnly(2025, 1, 8), GioChieu = 1, MaPhim = 1, MaPhong = 1, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 2, NgayChieu = new DateOnly(2025, 1, 9), GioChieu = 2, MaPhim = 2, MaPhong = 2, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 3, NgayChieu = new DateOnly(2025, 1, 10), GioChieu = 3, MaPhim = 3, MaPhong = 3, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 4, NgayChieu = new DateOnly(2025, 1, 11), GioChieu = 4, MaPhim = 4, MaPhong = 4, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 5, NgayChieu = new DateOnly(2025, 1, 12), GioChieu = 5, MaPhim = 5, MaPhong = 5, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 6, NgayChieu = new DateOnly(2025, 1, 13), GioChieu = 6, MaPhim = 1, MaPhong = 6, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 7, NgayChieu = new DateOnly(2025, 1, 14), GioChieu = 1, MaPhim = 2, MaPhong = 1, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 8, NgayChieu = new DateOnly(2025, 1, 15), GioChieu = 2, MaPhim = 3, MaPhong = 2, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 9, NgayChieu = new DateOnly(2025, 1, 16), GioChieu = 3, MaPhim = 4, MaPhong = 3, GiaVe = 100000, TinhTrang = true },
                new LichChieu { MaLichChieu = 10, NgayChieu = new DateOnly(2025, 1, 17), GioChieu = 4, MaPhim = 5, MaPhong = 4, GiaVe = 100000, TinhTrang = true }
            );
        }
    }
}