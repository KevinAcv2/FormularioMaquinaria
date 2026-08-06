using System.Text;
using System.Text.Json;

namespace Maquinarias.Services
{
    public class OcrService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OcrService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> LeerTextoAsync(IFormFile imagen)
        {
            var apiKey = _configuration["OcrSpace:ApiKey"]
                ?? throw new InvalidOperationException("La API Key 'OcrSpace:ApiKey' no está configurada en appsettings.json.");


            using var content =
                new MultipartFormDataContent();

            using var stream =
                imagen.OpenReadStream();

            content.Add(
                new StreamContent(stream),
                "file",
                imagen.FileName);

            content.Add(
                new StringContent(apiKey),
                "apikey");

            content.Add(
                new StringContent("spa"),
                "language");

            var response =
                await _httpClient.PostAsync(
                    "https://api.ocr.space/parse/image",
                    content);

            var json =
                await response.Content.ReadAsStringAsync();

            using JsonDocument doc =
                JsonDocument.Parse(json);

            var texto =
                doc.RootElement
                .GetProperty("ParsedResults")[0]
                .GetProperty("ParsedText")
                .GetString();

            return texto ?? "";
        }
    }
}
