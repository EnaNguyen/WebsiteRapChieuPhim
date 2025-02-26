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

        // ✅ Lấy tất cả Combo (Gồm luôn danh sách sản phẩm)
        public async Task<List<ComboModal>> GetAll()
        {
            var combos = await _context.Combos
                .Include(c => c.ChiTietCombos)
                .ThenInclude(ct => ct.SanPham)
                .ToListAsync();

            return _mapper.Map<List<ComboModal>>(combos);
        }

        // ✅ Lấy Combo theo ID (Gồm luôn danh sách sản phẩm)
        public async Task<ComboModal> GetById(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.ChiTietCombos)
                .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(c => c.Id == id);

            return combo != null ? _mapper.Map<ComboModal>(combo) : null;
        }

        // ✅ Tạo mới Combo
        public async Task<APIResponse> Create(ComboModal comboModal)
        {
            var response = new APIResponse();
            try
            {
                var combo = _mapper.Map<Combo>(comboModal);
                await _context.Combos.AddAsync(combo);
                await _context.SaveChangesAsync();

                response.ResponseCode = 201;
                response.Result = "Tạo combo thành công!";
                _logger.LogInformation("Created new combo with ID: {ComboId}", combo.Id);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error creating combo");
            }

            return response;
        }

        // ✅ Cập nhật Combo
        public async Task<APIResponse> Update(int id, ComboModal comboModal)
        {
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

                // Cập nhật thông tin combo
                existingCombo.TenCombo = comboModal.TenCombo;
                existingCombo.Gia = comboModal.Gia;
                existingCombo.MoTa = comboModal.MoTa;
                existingCombo.HinhAnh = comboModal.HinhAnh;

                // Xóa các ChiTietCombo cũ (Cách 2)
                if (existingCombo.ChiTietCombos != null && existingCombo.ChiTietCombos.Any())
                {
                    foreach (var chiTiet in existingCombo.ChiTietCombos)
                    {
                        _context.Entry(chiTiet).State = EntityState.Deleted;
                    }
                }

                // Thêm danh sách ChiTietCombo mới
                existingCombo.ChiTietCombos = comboModal.ChiTietCombos
                    .Select(ct => new ChiTietCombo
                    {
                        SanPhamId = ct.SanPhamId,
                        SoLuong = ct.SoLuong
                    }).ToList();

                await _context.SaveChangesAsync();

                response.ResponseCode = 200;
                response.Result = "Cập nhật combo thành công!";
            }
            catch (Exception ex)
            {
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
