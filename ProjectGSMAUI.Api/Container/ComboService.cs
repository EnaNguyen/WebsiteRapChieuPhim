using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Container
{
    public class ComboService : ICombo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ComboService> _logger;

        public ComboService(ApplicationDbContext context, IMapper mapper, ILogger<ComboService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ Lấy tất cả Combo (Gồm luôn danh sách sản phẩm + tên sản phẩm)
        public async Task<List<ComboModal>> GetAll()
        {
            var combos = await _context.Combos
                .Include(c => c.ChiTietCombos)
                .ThenInclude(ct => ct.SanPham)
                .ToListAsync();

            var comboList = _mapper.Map<List<ComboModal>>(combos);

            return comboList;
        }

        // ✅ Lấy Combo theo ID (Gồm luôn danh sách sản phẩm + tên sản phẩm)
        public async Task<ComboModal> GetById(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.ChiTietCombos)
                .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
                return null;

            var comboModal = _mapper.Map<ComboModal>(combo);

            // ✅ Gán tên sản phẩm vào từng ChiTietCombo
            foreach (var ct in comboModal.ChiTietCombos)
            {
                var sanPham = await _context.SanPhams.FindAsync(ct.SanPhamId);
                ct.TenSanPham = sanPham?.TenSanPham ?? "Không xác định";
            }

            return comboModal;
        }

        // ✅ Tạo mới Combo
        public async Task<APIResponse> Create(ComboCreate comboModal)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var response = new APIResponse();
            try
            {
                Combo newCombo = new Combo()
                {
                    TenCombo = comboModal.TenCombo,
                    Gia = comboModal.Gia,
                    MoTa = comboModal.MoTa,
                    HinhAnh = comboModal.HinhAnh,
                };
                
                await _context.Combos.AddAsync(newCombo);
                await _context.SaveChangesAsync();

                foreach (var item in comboModal.ChiTietCombos)
                {
                    var sanPham = await _context.SanPhams.FindAsync(item.SanPhamId);
                    if (sanPham != null)  // Kiểm tra sản phẩm có tồn tại không
                    {
                        ChiTietCombo newCT = new ChiTietCombo()
                        {
                            ComboId = newCombo.Id,   // Gán ID của Combo mới
                            SanPhamId = item.SanPhamId,
                            SoLuong = item.SoLuong
                        };

                        await _context.ChiTietCombos.AddAsync(newCT);
                    }
                }
                await _context.SaveChangesAsync(); // Lưu thay đổi vào database
                await transaction.CommitAsync();
               
                response.ResponseCode = 201;
                response.Result = "Tạo combo thành công!";
                _logger.LogInformation("Created new combo with ID: {ComboId}", newCombo.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error creating combo");
            }

            return response;
        }

        // ✅ Cập nhật Combo
        public async Task<APIResponse> Update(int id, ComboCreate comboModal)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var response = new APIResponse();

            try
            {
                var existingCombo = await _context.Combos
                    .Include(c => c.ChiTietCombos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existingCombo == null)
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Combo không tồn tại!";
                    return response;
                }

                // Cập nhật thông tin Combo
                existingCombo.TenCombo = comboModal.TenCombo;
                existingCombo.Gia = comboModal.Gia;
                existingCombo.MoTa = comboModal.MoTa;
                existingCombo.HinhAnh = comboModal.HinhAnh;

                // Xóa ChiTietCombo cũ
                _context.ChiTietCombos.RemoveRange(existingCombo.ChiTietCombos);
                await _context.SaveChangesAsync(); // Cập nhật vào database để tránh lỗi khóa ngoại

                // Thêm ChiTietCombo mới
                foreach (var item in comboModal.ChiTietCombos)
                {
                    var sanPham = await _context.SanPhams.FindAsync(item.SanPhamId);
                    if (sanPham != null)
                    {
                        ChiTietCombo newCT = new ChiTietCombo()
                        {
                            ComboId = existingCombo.Id,
                            SanPhamId = item.SanPhamId,
                            SoLuong = item.SoLuong
                        };
                        await _context.ChiTietCombos.AddAsync(newCT);
                    }
                }

                await _context.SaveChangesAsync(); // Lưu thay đổi
                await transaction.CommitAsync();  // Commit transaction nếu mọi thứ thành công

                response.ResponseCode = 200;
                response.Result = "Cập nhật combo thành công!";
                _logger.LogInformation("Updated combo with ID: {ComboId}", existingCombo.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback nếu có lỗi
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error updating combo");
            }

            return response;
        }


        // ✅ Xóa Combo
        public async Task<APIResponse> Delete(int id)
        {
            var response = new APIResponse();
            try
            {
                var combo = await _context.Combos
                    .Include(c => c.ChiTietCombos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (combo == null)
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Combo không tồn tại!";
                    return response;
                }

                _context.ChiTietCombos.RemoveRange(combo.ChiTietCombos);
                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();

                response.ResponseCode = 200;
                response.Result = "Combo đã được xóa!";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error deleting combo");
            }

            return response;
        }
    }
}
