using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.SqlServer.Server;
using System.Xml.Linq;

namespace ProjectGSMAUI.Api.Container
{
    public class PhimService : IPhimService
    {
        private readonly ApplicationDbContext _context;

        public PhimService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> CheckSuatChieu(int Id, CheckDate Data)
        {
            async Task AddSuatChieu(int IDPhim, DateOnly NgayBatDau, int SoNgayChieu, int SoSuatChieu)
            {
                int SoPhong = 6; // Giả sử có 6 phòng chiếu
                int SoCaChieu = 6; // Có 6 suất chiếu trong ngày
                int TongSuat = SoNgayChieu * SoCaChieu * SoPhong;

                if (SoSuatChieu > TongSuat)
                {
                    throw new Exception("Số lượng suất chiếu vượt quá giới hạn của rạp!");
                }

                int spacing = TongSuat / SoSuatChieu; // Khoảng cách giữa các suất chiếu
                int count = 0; // Đếm số suất chiếu đã thêm
                int index = 0; // Vị trí bắt đầu

                while (count < SoSuatChieu)
                {
                    int day = index / (SoCaChieu * SoPhong);   // Tính ngày từ chỉ số
                    int slot = (index % (SoCaChieu * SoPhong)); // Tính suất chiếu trong ngày
                    int Phong = (slot % SoPhong) + 1; //Tính phòng chiếu
                    int GioChieu = (slot / SoPhong) + 1; //Tính giờ chiếu

                    DateOnly NgayChieu = NgayBatDau.AddDays(day);

                    // Nếu vị trí trống, gán phim vào
                    var Checked = _context.LichChieus
                        .FirstOrDefault(g => g.NgayChieu == NgayChieu &&
                                             g.MaPhong == Phong &&
                                             g.GioChieu == GioChieu);

                    while (Checked != null)
                    {
                        index = (index + 1) % TongSuat; 
                        day = index / (SoCaChieu * SoPhong);   
                        slot = (index % (SoCaChieu * SoPhong)); 
                        Phong = (slot % SoPhong) + 1; 
                        GioChieu = (slot / SoPhong) + 1;
                        NgayChieu = NgayBatDau.AddDays(day);
                        Checked = _context.LichChieus
                            .FirstOrDefault(g => g.NgayChieu == NgayChieu &&
                                                 g.MaPhong == Phong &&
                                                 g.GioChieu == GioChieu);
                    }

                    _context.LichChieus.Add(new LichChieu
                    {
                        NgayChieu = NgayChieu,
                        GioChieu = GioChieu,
                        MaPhong = Phong,
                        MaPhim = IDPhim,
                        GiaVe = 100000,
                        TinhTrang = true,
                    });

                    await _context.SaveChangesAsync();
                    count++;

                    // Xuất thông tin suất chiếu (bạn có thể bỏ qua phần này nếu không cần)
                    Console.WriteLine($"Suất chiếu thứ {count} của phim ID {IDPhim}:");
                    Console.WriteLine($"Ngày: {NgayChieu}, Suất: {GioChieu}, Phòng: {Phong}");

                    // Di chuyển đến vị trí tiếp theo với khoảng cách `spacing`
                    index = (index + spacing) % TongSuat;
                }
            }

            APIResponse response = new APIResponse();

            try
            {
                int soNgayChieu = (Data.NgayKetThuc.Value.ToDateTime(TimeOnly.MinValue) -
                    Data.NgayKhoiChieu.Value.ToDateTime(TimeOnly.MinValue)).Days + 1; //+1 để tính luôn cả ngày cuối
                if (soNgayChieu <= 0)
                {
                    throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu!");
                }

                await AddSuatChieu(Id, Data.NgayKhoiChieu ?? DateOnly.FromDateTime(DateTime.Now), soNgayChieu, Data.SoSuatChieu);

                response.ResponseCode = 200;
                response.Result = "Cập nhật thành công!";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.Result = $"Lỗi: {ex.Message}";
            }
            return response;        
        }

        public async Task<APIResponse> Create(CreateMovie data)
        {
            APIResponse response = new APIResponse();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int MaxMaPhim = (_context.Phims.Max(g => (int?)g.Id) ?? 0) + 1;
                Phim phim = new Phim
                {
                    TenPhim = data.PhimDatas.TenPhim,
                    TheLoai = data.PhimDatas.TheLoai,
                    ThoiLuong = data.PhimDatas.ThoiLuong,
                    DaoDien = data.PhimDatas.DaoDien,
                    GioiHanDoTuoi = data.PhimDatas.GioiHanDoTuoi,
                    MoTa = data.PhimDatas.MoTa,
                    NgayKetThuc = data.PhimDatas.NgayKetThuc,
                    NgayKhoiChieu = data.PhimDatas.NgayKhoiChieu,
                    SoSuatChieu = data.PhimDatas.SoSuatChieu,
                    TrangThai = 1
                };
                _context.Phims.Add(phim);
                await _context.SaveChangesAsync(); 
                for(int i=0; i<data.HinhAnhs.Count;i++)
                {
                    if (!string.IsNullOrEmpty(data.HinhAnhs[i].ImageData))
                    {
                        byte[] imageBytes;
                        try
                        {
                            imageBytes = Convert.FromBase64String(data.HinhAnhs[i].ImageData);
                        }
                        catch (FormatException)
                        {
                            response.ResponseCode = 400;
                            response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                            await transaction.RollbackAsync();
                            return response;
                        }

                        string ID = Guid.NewGuid().ToString();

                        _context.HinhAnhs.Add(new HinhAnh { Id = ID, Phim = phim.Id, ImageData = imageBytes, Avatar = (i == 0) });
                    }
                }

                await _context.SaveChangesAsync();

                foreach (var video in data.Videos)
                {
                    string ID = Guid.NewGuid().ToString();

                    _context.Videos.Add(new Video { Id = ID, Phim = phim.Id, Link = video.Link });
                }

                await _context.SaveChangesAsync();

                CheckDate DuLieuLichChieu = new CheckDate
                {
                    NgayKetThuc = phim.NgayKetThuc,
                    NgayKhoiChieu = phim.NgayKhoiChieu,
                    SoSuatChieu = phim.SoSuatChieu ?? 1
                };

                var checkResponse = await CheckSuatChieu(phim.Id, DuLieuLichChieu);
                if (checkResponse.ResponseCode != 200)
                {
                    await transaction.RollbackAsync();
                    return checkResponse;
                }

                await transaction.CommitAsync(); 
                response.ResponseCode = 200;
                response.Result = "Cập nhật thành công!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.ResponseCode = 500;
                response.Result = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        public async Task<List<PhimView>> GetAll(string name)
        {
            var data = await _context.Phims
            .AsNoTracking()
            .Select(item => new PhimView
            {
                Id = item.Id,
                TenPhim = item.TenPhim,
                TheLoai = _context.TheLoaiPhims.Where(g=>g.Id.Trim()==item.TheLoai.Trim()).Select(h=>h.TenTheLoai).FirstOrDefault()??null,
                ThoiLuong = item.ThoiLuong,
                DaoDien = item.DaoDien,
                GioiHanDoTuoi = item.GioiHanDoTuoi,
                NgayKhoiChieu = item.NgayKhoiChieu,
                NgayKetThuc = item.NgayKetThuc,
                SoSuatChieu = item.SoSuatChieu,
                TrangThai = item.TrangThai,
                MoTa = item.MoTa,
                HinhAnh = _context.HinhAnhs
                    .Where(g => (g.Phim == item.Id&&g.Avatar==true))
                    .Select(g => g.ImageData)
                    .FirstOrDefault()??null,
                Video = _context.Videos
                .Where(v => v.Phim == item.Id)
                .Select(v => v.Link)
                .FirstOrDefault()??null,
            })
            .ToListAsync();
            return data;
        }

        public async Task<DetailMovie> GetByID(int id)
        {
            var data = await _context.Phims
                .AsNoTracking()
                .Where(item => item.Id == id) 
                .Select(item => new PhimView
                {
                    Id = item.Id,
                    TenPhim = item.TenPhim,
                    TheLoai = item.TheLoai,
                    ThoiLuong = item.ThoiLuong,
                    DaoDien = item.DaoDien,
                    GioiHanDoTuoi = item.GioiHanDoTuoi,
                    NgayKhoiChieu = item.NgayKhoiChieu,
                    NgayKetThuc = item.NgayKetThuc,
                    SoSuatChieu = item.SoSuatChieu,
                    TrangThai = item.TrangThai,
                    MoTa = item.MoTa,
                    HinhAnh = _context.HinhAnhs
                        .Where(g => g.Phim == item.Id)
                        .Select(g => g.ImageData)
                        .FirstOrDefault() 
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return null; 

            var Anh = await _context.HinhAnhs
                .AsNoTracking()
                .Where(g => g.Phim == id)
                .ToListAsync();
            var Video = await _context.Videos.AsNoTracking().Where(g => g.Phim == id).ToListAsync();
            var Movie = new DetailMovie
            {
                Movies = data,
                HinhAnhs = Anh,
                Videos = Video
            };           
            return Movie;
        }

        public Task<APIResponse> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse> Update(int Id, CreateMovie data)
        {
            APIResponse response = new APIResponse();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var phim = _context.Phims.Where(g => g.Id == Id).FirstOrDefault();
                phim.TenPhim = data.PhimDatas.TenPhim;
                phim.TheLoai = data.PhimDatas.TheLoai;
                phim.ThoiLuong = data.PhimDatas.ThoiLuong;
                phim.DaoDien = data.PhimDatas.DaoDien;
                phim.GioiHanDoTuoi = data.PhimDatas.GioiHanDoTuoi;
                phim.MoTa = data.PhimDatas.MoTa;
                phim.NgayKetThuc = data.PhimDatas.NgayKetThuc;
                phim.NgayKhoiChieu = data.PhimDatas.NgayKhoiChieu;
                phim.TrangThai = data.PhimDatas.TrangThai;
                _context.Phims.Update(phim);
                await _context.SaveChangesAsync();
                var OldImage = _context.HinhAnhs.Where(g => g.Phim == Id).ToList();
                _context.HinhAnhs.RemoveRange(OldImage);
                await _context.SaveChangesAsync();
                var OldVideo = _context.Videos.Where(g => g.Phim == Id).ToList();
                _context.Videos.RemoveRange(OldVideo);
                await _context.SaveChangesAsync();
                foreach (var image in data.HinhAnhs)
                {
                    if (!string.IsNullOrEmpty(image.ImageData))
                    {
                        byte[] imageBytes;
                        try
                        {
                            imageBytes = Convert.FromBase64String(image.ImageData);
                        }
                        catch (FormatException)
                        {
                            response.ResponseCode = 400;
                            response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                            await transaction.RollbackAsync();
                            return response;
                        }

                        string ID = Guid.NewGuid().ToString();
                        _context.HinhAnhs.Add(new HinhAnh { Id = ID, Phim = Id, ImageData = imageBytes });
                    }
                }
                foreach (var video in data.Videos)
                {
                    string ID = Guid.NewGuid().ToString();
                    _context.Videos.Add(new Video { Id = ID, Phim = Id, Link = video.Link });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 200;
                response.Result = "Cập nhật thành công!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.ResponseCode = 500;
                response.Result = $"Lỗi: {ex.Message}";
            }

            return response;
        }

    }
}