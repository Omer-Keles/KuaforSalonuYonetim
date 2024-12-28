using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpMethod = System.Net.Http.HttpMethod;

public class HairstyleController : Controller
{
    private readonly Cloudinary _cloudinary;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public HairstyleController(Cloudinary cloudinary, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _cloudinary = cloudinary;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
        ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
        ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        return View(new HairstyleRequestModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(HairstyleRequestModel model)
    {
        ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
        ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
        ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        if (model.ImageFile == null || string.IsNullOrEmpty(model.TextPrompt))
        {
            ModelState.AddModelError("", "Please upload an image and enter a prompt.");
            return View(model);
        }

        // Step 1: Upload Image to Cloudinary
        string imageUrl = await UploadToCloudinary(model.ImageFile);

        if (string.IsNullOrEmpty(imageUrl))
        {
            ModelState.AddModelError("", "Failed to upload image.");
            return View(model);
        }

        // Step 2: Call LightX API
        string resultImageUrl = await CallLightXApi(imageUrl, model.TextPrompt);

        if (string.IsNullOrEmpty(resultImageUrl))
        {
            ViewBag.ErrorMessage = "Görsel oluşturma başarısız oldu. Lütfen tekrar deneyin.";
            return View(model);
        }

        model.ResultImageUrl = resultImageUrl;
        return View(model);
    }

    private async Task<string> UploadToCloudinary(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = _configuration["Cloudinary:Folder"]
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult?.SecureUrl?.ToString();
    }

    private async Task<string> CallLightXApi(string imageUrl, string textPrompt)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var apiKey = _configuration["LightX:ApiKey"];
        var payload = new
        {
            imageUrl,
            textPrompt
        };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.lightxeditor.com/external/api/v1/hairstyle"),
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("x-api-key", apiKey);

        try
        {
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda yanıtı logla
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CallLightXApi Error Response: {errorDetails}");
                return null;
            }

            var responseData = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseData);

            Console.WriteLine($"CallLightXApi API Response: {responseData}");

            // orderId kontrolü
            string orderId = result?.body?.orderId?.ToString();
            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("CallLightXApi: Missing orderId in response.");
                return null;
            }

            // Durum kontrolü yap
            return await CheckStatus(orderId);
        }
        catch (Exception ex)
        {
            // Beklenmedik bir hata durumunda detayları logla
            Console.WriteLine($"CallLightXApi Exception: {ex.Message}");
            return null;
        }
    }


    private async Task<string> CheckStatus(string orderId)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var apiKey = _configuration["LightX:ApiKey"];
        var payload = new { orderId };

        for (int i = 0; i < 5; i++) // En fazla 5 kez durum kontrolü yap
        {
            try
            {
                // Her döngüde yeni bir HttpRequestMessage oluştur
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api.lightxeditor.com/external/api/v1/order-status"),
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                };

                request.Headers.Add("x-api-key", apiKey);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    // Hata durumunda detayları logla
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"CheckStatus Error Response: {errorDetails}");
                    continue;
                }

                var responseData = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseData);

                Console.WriteLine($"CheckStatus API Response: {responseData}");

                if (result?.body?.status == "active")
                {
                    return result?.body?.output?.ToString();
                }

                // Eğer hala "init" durumundaysa 3 saniye bekle
                await Task.Delay(3000);
            }
            catch (Exception ex)
            {
                // Beklenmedik bir hata durumunda detayları logla
                Console.WriteLine($"CheckStatus Exception: {ex.Message}");
            }
        }

        return null; // Eğer sonuç alınamazsa null döner
    }


}
