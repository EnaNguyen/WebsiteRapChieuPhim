using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectGSMVC.Controllers
{
    public class GeminiTraLoiUserController : Controller
    {
        private readonly HttpClient _httpClient;

        public GeminiTraLoiUserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> GetGeminiResponse([FromBody] UserQuestionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserQuestion))
            {
                return Json(new { response = "Vui lòng nhập câu hỏi!" });
            }

            string apiUrl = $"https://localhost:7141/api/Gemini/TraLoi?question={model.UserQuestion}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize phản hồi từ API
                var apiResult = JsonSerializer.Deserialize<ApiResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Kiểm tra nếu `result` có dữ liệu hợp lệ
                string finalResponse = !string.IsNullOrWhiteSpace(apiResult?.Result)
                    ? apiResult.Result
                    : "Xin lỗi, tôi không thể tìm thấy câu trả lời phù hợp.";

                return Json(new { response = finalResponse });
            }
            else
            {
                return Json(new { response = "Lỗi khi gọi API!" });
            }
        }

        public class UserQuestionModel
        {
            public string UserQuestion { get; set; }
        }

        public class ApiResponse
        {
            public string Result { get; set; }
        }
    }
}
