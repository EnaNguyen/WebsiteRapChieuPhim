
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Services;
using System.Diagnostics;
using System.Text;
using static System.Net.WebRequestMethods;
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
                string Openning = "Hãy xưng hô với tôi là Rem - Chủ Nhân để trả lời câu hỏi của tôi:";
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
                               new{  text = Openning + userInput
                                  /*  "\nYêu cầu đầu ra: Tôi sẽ đưa câu trả lời của bạn vào innerHTML , do đó, nếu cần xuống dòng hãy thêm <br>, nếu cần gắn link sản phẩm hãy gắn <button class='btn btn-primary'><a href=\"https://localhost:7265/Detail/Index/@maSanPham\">Xem Chi Tiết</a></button>." +
                                    "\n Hãy linh hoạt trong câu trả lời và trả lời ngắn gọn, nếu khách hàng hỏi câu hỏi bên ngoài thì vẫn trả lời và không cần đề cập đến các sản phẩm trong cửa hàng mà hãy trả lời như bình thường." +
                                    " Còn nếu câu hỏi về cần tư vấn sản phẩm thì hãy dựa vào dữ liệu tôi đưa ( khi cần ) để trả lời câu hỏi sau : " + userInput*/
                                }
                            },
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
