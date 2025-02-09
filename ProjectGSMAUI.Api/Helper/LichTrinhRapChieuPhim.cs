using System;
using System.Collections.Generic;
using System.Linq;
using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Helper
{
    public class LichTrinhRapChieuPhim
    {
        public const int SoNgay = 7; // Số ngày trong tuần
        public const int SuatMoiNgay = 36; // Số suất chiếu mỗi ngày
        public const int TongSuat = SoNgay * SuatMoiNgay; // Tổng số suất chiếu = 252

        public List<LichChieu> TaoLichChieu(List<Phim> danhSachPhim, List<Phong> danhSachPhong, List<KhungGio> danhSachKhungGio, out List<Phim> phimDaCapNhat)
        {
            var lichChieu = new List<LichChieu>();
            var LichChieu = new LichChieu[SoNgay, SuatMoiNgay]; // Mảng 2 chiều cho lịch chiếu
            var soSuatMoiNgay = new int[SoNgay]; // Mảng để theo dõi số suất chiếu mỗi ngày
            var soSuatPhimMoiNgay = new Dictionary<int, int[]>(); // Dictionary để theo dõi số suất chiếu mỗi ngày cho từng phim
            var random = new Random();

            // Khởi tạo dictionary cho số suất chiếu mỗi ngày của mỗi phim
            foreach (var phim in danhSachPhim)
            {
                soSuatPhimMoiNgay[phim.Id] = new int[SoNgay];
            }

            // Kiểm tra và log danh sách phim, phòng và khung giờ
            Console.WriteLine($"Danh sách phim: {danhSachPhim.Count}, Danh sách phòng: {danhSachPhong.Count}, Danh sách khung giờ: {danhSachKhungGio.Count}");

            // Duyệt qua tất cả các phim
            foreach (var phim in danhSachPhim)
            {
                int soLuongSuatChieu = phim.SoSuatChieu ?? 0;

                // Kiểm tra và giới hạn số suất chiếu của phim không vượt quá TongSuat
                if (soLuongSuatChieu > TongSuat)
                {
                    Console.WriteLine($"Số suất chiếu của phim {phim.TenPhim} vượt quá giới hạn {TongSuat}! Đang điều chỉnh...");
                    soLuongSuatChieu = TongSuat;
                    phim.SoSuatChieu = TongSuat; // Cập nhật lại thông tin phim
                }

                int khoangCach = TongSuat / soLuongSuatChieu; // Khoảng cách giữa các suất chiếu
                int dem = 0; // Đếm số suất chiếu đã thêm
                int chiSo = random.Next(TongSuat); // Vị trí bắt đầu ngẫu nhiên
                int attempts = 0; // Đếm số lần thử
                int maxAttempts = TongSuat; // Giới hạn số lần thử

                // Kiểm tra và log giá trị của phim trước khi bắt đầu tạo lịch chiếu
                Console.WriteLine($"Đang tạo lịch chiếu cho phim: {phim.TenPhim}, Số suất chiếu: {soLuongSuatChieu}");

                while (dem < soLuongSuatChieu && attempts < maxAttempts)
                {
                    int ngay = chiSo / SuatMoiNgay;   // Tính ngày từ chỉ số
                    int suat = chiSo % SuatMoiNgay; // Tính suất chiếu trong ngày

                    // Kiểm tra trước khi thêm suất chiếu
                    Console.WriteLine($"Đang kiểm tra suất chiếu của phim {phim.TenPhim}, Ngày: {ngay}, Suất: {suat}, Số suất đã chiếu trong ngày: {soSuatPhimMoiNgay[phim.Id][ngay]}");

                    // Nếu vị trí trống và số suất chiếu trong ngày chưa vượt quá giới hạn, gán phim vào
                    if (LichChieu[ngay, suat] == null && soSuatMoiNgay[ngay] < SuatMoiNgay && soSuatPhimMoiNgay[phim.Id][ngay] < 5)
                    {
                        LichChieu[ngay, suat] = new LichChieu
                        {
                            MaPhim = phim.Id,
                            MaPhong = danhSachPhong[random.Next(danhSachPhong.Count)].Id,
                            GioChieu = danhSachKhungGio[random.Next(danhSachKhungGio.Count)].Id,
                            NgayChieu = DateOnly.FromDateTime(DateTime.Today.AddDays(ngay)),
                            GiaVe = 50000,
                            TinhTrang = true
                        };

                        lichChieu.Add(LichChieu[ngay, suat]);
                        soSuatMoiNgay[ngay]++;
                        soSuatPhimMoiNgay[phim.Id][ngay]++;
                        dem++;

                        // Xuất thông tin suất chiếu
                        Console.WriteLine($"Suất chiếu thứ {dem} của {phim.TenPhim}:");
                        Console.WriteLine($"Ngày: {ngay + 1}, Suất: {(suat + 1) % 6}, Phòng: {(suat + 1) / 6}");
                    }

                    // Di chuyển đến vị trí tiếp theo với khoảng cách `khoangCach`
                    chiSo = (chiSo + khoangCach) % TongSuat;
                    attempts++;
                }

                // Giảm số suất chiếu của phim sau khi lập lịch
                phim.SoSuatChieu -= dem;
                if (phim.SoSuatChieu < 0) phim.SoSuatChieu = 0; // Tránh giảm quá mức
                if (phim.SoSuatChieu <= 0)
                {
                    phim.TrangThai = 0; // Chuyển trạng thái phim thành hết phim nếu không còn suất chiếu
                }

                // Log trạng thái phim sau khi cập nhật
                Console.WriteLine($"Phim {phim.TenPhim} sau khi cập nhật: Số suất chiếu còn lại: {phim.SoSuatChieu}, Trạng thái: {phim.TrangThai}");
            }

            // Trả về danh sách phim đã cập nhật
            phimDaCapNhat = danhSachPhim;

            return lichChieu;
        }
    }
}