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
                    "La API Key de OCR.space no está configurada."
                );
            }

            using var formData = new MultipartFormDataContent();

            using var stream = imagen.OpenReadStream();

            var fileContent = new StreamContent(stream);

            formData.Add(
                fileContent,
                "file",
                imagen.FileName
            );

            formData.Add(
                new StringContent(apiKey),
                "apikey"
            );

            formData.Add(
                new StringContent("spa"),
                "language"
            );

            formData.Add(
                new StringContent("2"),
                "OCREngine"
            );

            var response = await _httpClient.PostAsync(
                "https://api.ocr.space/parse/image",
                formData
            );

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OCR.space respondió HTTP {(int)response.StatusCode}: {json}"
                );
            }

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            // Revisar si OCR.space reportó error
            if (root.TryGetProperty(
                "IsErroredOnProcessing",
                out var errorProperty))
            {
                if (errorProperty.GetBoolean())
                {
                    string mensaje = "Error desconocido de OCR.space";

                    if (root.TryGetProperty(
                        "ErrorMessage",
                        out var errorMessage))
                    {
                        mensaje = errorMessage.ToString();
                    }

                    throw new Exception(
                        $"OCR.space: {mensaje}"
                    );
                }
            }

            if (!root.TryGetProperty(
                "ParsedResults",
                out var resultados))
            {
                return "";
            }

            if (resultados.GetArrayLength() == 0)
            {
                return "";
            }

            var texto =
                resultados[0]
                    .GetProperty("ParsedText")
                    .GetString();

            return texto ?? "";
        }
    }
}