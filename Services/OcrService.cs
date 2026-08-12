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
            if (imagen == null || imagen.Length == 0)
                return "";

            var apiKey = _configuration["OcrSpace:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "La API Key de OCR.space no está configurada.");
            }

            using var content = new MultipartFormDataContent();

            using var stream = imagen.OpenReadStream();

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

            content.Add(
                new StringContent("true"),
                "isOverlayRequired");

            var response = await _httpClient.PostAsync(
                "https://api.ocr.space/parse/image",
                content);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OCR.space respondió HTTP {(int)response.StatusCode}: {json}");
            }

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (root.TryGetProperty("IsErroredOnProcessing", out var error))
            {
                if (error.GetBoolean())
                {
                    return "";
                }
            }

            if (!root.TryGetProperty("ParsedResults", out var resultados))
                return "";

            if (resultados.GetArrayLength() == 0)
                return "";

            var texto = resultados[0]
                .GetProperty("ParsedText")
                .GetString();

            return texto ?? "";
        }
    }
}