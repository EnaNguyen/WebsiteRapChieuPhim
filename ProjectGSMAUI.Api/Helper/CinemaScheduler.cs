using System;
using System.Collections.Generic;
using System.Linq;
using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Helper
{
    public class CinemaScheduler
    {
        public List<LichChieu> GenerateSchedule(List<Phim> films, List<Phong> rooms, List<KhungGio> timeSlots)
        {
            var schedule = new List<LichChieu>();
            int maxShowsPerDay = 36; // Tổng số suất chiếu tối đa mỗi ngày
            int maxShowsPerFilmPerDay = 5; // Số suất chiếu tối đa cho mỗi phim mỗi ngày
            int totalRooms = rooms.Count;
            int totalTimeSlots = timeSlots.Count;

            // Khởi tạo danh sách các khung giờ đã sử dụng
            var usedTimeSlots = new Dictionary<(int, int), bool>();

            foreach (var film in films.Where(f => f.TrangThai == 1))
            {
                int remainingShows = Math.Min(film.SoSuatChieu ?? 0, maxShowsPerFilmPerDay);
                int showsAssigned = 0;

                foreach (var timeSlot in timeSlots)
                {
                    if (remainingShows <= 0 || showsAssigned >= maxShowsPerFilmPerDay) break;

                    foreach (var room in rooms)
                    {
                        if (remainingShows <= 0 || showsAssigned >= maxShowsPerFilmPerDay) break;

                        // Kiểm tra xem khung giờ và phòng này đã được sử dụng chưa
                        if (!usedTimeSlots.ContainsKey((room.Id, timeSlot.Id)))
                        {
                            var showing = new LichChieu
                            {
                                MaPhim = film.Id,
                                MaPhong = room.Id,
                                GioChieu = timeSlot.Id,
                                NgayChieu = DateOnly.FromDateTime(DateTime.Today),
                                GiaVe = 50000,
                                TinhTrang = true
                            };

                            schedule.Add(showing);
                            showsAssigned++;
                            remainingShows--;

                            // Đánh dấu khung giờ và phòng này đã được sử dụng
                            usedTimeSlots[(room.Id, timeSlot.Id)] = true;

                            if (schedule.Count >= maxShowsPerDay) break;
                        }
                    }

                    if (schedule.Count >= maxShowsPerDay) break;
                }

                if (schedule.Count >= maxShowsPerDay) break;
            }

            return schedule;
        }
    }
}
