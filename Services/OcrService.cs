using Tesseract;

namespace Maquinarias.Services
{
    public class OcrService
    {
        private readonly IWebHostEnvironment _environment;

        public OcrService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> LeerTextoAsync(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
                return "";

            string tessdataPath = Path.Combine(
                _environment.ContentRootPath,
                "tessdata"
            );

            if (!Directory.Exists(tessdataPath))
                throw new DirectoryNotFoundException(
                    $"No se encontró la carpeta tessdata: {tessdataPath}"
                );

            string archivoTemporal = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}.png"
            );

            try
            {
                // Guardar temporalmente la imagen
                using (var stream = new FileStream(
                    archivoTemporal,
                    FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                using var engine = new TesseractEngine(
                    tessdataPath,
                    "eng",
                    EngineMode.Default
                );

                using var img = Pix.LoadFromFile(archivoTemporal);

                using var page = engine.Process(
                    img,
                    PageSegMode.SingleLine
                );

                return page.GetText() ?? "";
            }
            finally
            {
                // Eliminar imagen temporal
                if (System.IO.File.Exists(archivoTemporal))
                {
                    System.IO.File.Delete(archivoTemporal);
                }
            }
        }
    }
}