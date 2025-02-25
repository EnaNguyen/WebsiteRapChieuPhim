using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Services;
using System.Text;

namespace ProjectGSMAUI.Api.Container
{
    public class GeminiServices : IGeminiServices
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiSettings _authSettings;

        public GeminiServices(ApplicationDbContext context, GeminiSettings authSettings)
        {
            _context = context;
            _authSettings = authSettings;
        }

        public async Task<APIResponse> TraLoi(string userInput)
        {
            APIResponse response1 = new APIResponse();
            try
            {
                string Openning = "Dưới đây là danh sách phim đang chiếu:\n";
                StringBuilder danhSachPhim = new StringBuilder("");
                var phimList = _context.Phims.ToList();
                if (phimList.Count == 0)
                {
                    danhSachPhim.Append("Hiện tại không có phim nào đang chiếu.");
                }
                else
                {
                    for (int i = 0; i < phimList.Count; i++)
                    {
                        danhSachPhim.AppendLine($"{i + 1}. Tên: {phimList[i].TenPhim}, Thể loại: {phimList[i].TheLoai}");
                    }
                }
                var GoogleAPIKey = _authSettings.Google.GoogleAPIKey;
                var GoogleAPIUrl = _authSettings.Google.GoogleAPIUrl;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = $"{Openning}\n{danhSachPhim}\n" +
                                           "Hãy đóng vai là một chuyên viên tư vấn phim, giúp tôi chọn phim phù hợp với câu hỏi sau:\n" +
                                           $"{userInput}.\n" 
                                         
                                }
                            }
                        }
                    }
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={GoogleAPIKey}", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);


                    string answer = responseObject?.candidates[0].content?.parts[0]?.text ?? "Xin lỗi, câu hỏi của bạn đã vi phạm chính sách của Google hoặc câu trở lời quá dài nên Rem không hiển thị cho bạn được";
                    response1.ResponseCode = 201;
                    response1.Result = answer.ToString();
                }
            }
            catch (Exception ex)
            {
                response1.ResponseCode = 400;
                response1.ErrorMessage = ex.Message;
            }
            return response1;
        }
    }
}
