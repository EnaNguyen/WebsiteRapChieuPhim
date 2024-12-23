﻿using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        public virtual DbSet<Coupon> Coupons { get; set; }

        public virtual DbSet<DatVe> DatVes { get; set; }

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
        public DbSet<Combo> Combos { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<ChiTietCombo> ChiTietCombos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.HasKey(e => e.MaChiTietHoaDon).HasName("PK__ChiTietH__CFF2C426609D1D0B");

                entity.ToTable("ChiTietHoaDon");

                entity.Property(e => e.MaChiTietHoaDon).ValueGeneratedNever();

                entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaGhe)
                    .HasConstraintName("FK__ChiTietHo__MaGhe__59FA5E80");

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

            modelBuilder.Entity<DatVe>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("DatVe");

                entity.Property(e => e.MaKhachHang)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.MaKhachHangNavigation).WithMany()
                    .HasForeignKey(d => d.MaKhachHang)
                    .HasConstraintName("FK__DatVe__MaKhachHa__4D94879B");

                entity.HasOne(d => d.MaVeNavigation).WithMany()
                    .HasForeignKey(d => d.MaVe)
                    .HasConstraintName("FK__DatVe__MaVe__4E88ABD4");
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
                entity.Property(e => e.Link).HasColumnType("text");

                entity.HasOne(d => d.PhimNavigation).WithMany(p => p.HinhAnhs)
                    .HasForeignKey(d => d.Phim)
                    .HasConstraintName("FK__HinhAnh__Phim__797309D9");
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

                entity.Property(e => e.MaLichChieu).ValueGeneratedNever();
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

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
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
                entity.Property(e => e.Hinh).HasColumnType("text");
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

                entity.Property(e => e.MaVe).ValueGeneratedNever();
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
        }
    }
}