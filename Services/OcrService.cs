using Tesseract;

namespace Maquinarias.Services
{
    public class OcrService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<OcrService> _logger;

        public OcrService(
            IWebHostEnvironment environment,
            ILogger<OcrService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> LeerTextoAsync(IFormFile imagen)
        {
            _logger.LogInformation("OCR: inicio");

            if (imagen == null || imagen.Length == 0)
            {
                _logger.LogWarning("OCR: imagen vacía");
                return "";
            }

            _logger.LogInformation(
                "OCR: imagen recibida. Nombre={Nombre}, Tamaño={Tamano}",
                imagen.FileName,
                imagen.Length);

            string tessdataPath = Path.Combine(
                _environment.ContentRootPath,
                "tessdata");

            _logger.LogInformation(
                "OCR: tessdataPath={Path}",
                tessdataPath);

            if (!Directory.Exists(tessdataPath))
            {
                _logger.LogError(
                    "OCR: NO existe tessdata en {Path}",
                    tessdataPath);

                throw new DirectoryNotFoundException(
                    $"No se encontró la carpeta tessdata: {tessdataPath}");
            }

            string trainedData = Path.Combine(
                tessdataPath,
                "eng.traineddata");

            _logger.LogInformation(
                "OCR: eng.traineddata existe={Existe}",
                File.Exists(trainedData));

            if (!File.Exists(trainedData))
            {
                throw new FileNotFoundException(
                    "No se encontró eng.traineddata",
                    trainedData);
            }

            string archivoTemporal = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}.png");

            try
            {
                _logger.LogInformation(
                    "OCR: guardando imagen temporal {Archivo}",
                    archivoTemporal);

                using (var stream = new FileStream(
                    archivoTemporal,
                    FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                _logger.LogInformation(
                    "OCR: imagen temporal guardada");

                _logger.LogInformation(
                    "OCR: creando TesseractEngine");

                using var engine = new TesseractEngine(
                    tessdataPath,
                    "eng",
                    EngineMode.Default);

                _logger.LogInformation(
                    "OCR: TesseractEngine creado correctamente");

                _logger.LogInformation(
                    "OCR: cargando imagen con Pix");

                using var img = Pix.LoadFromFile(
                    archivoTemporal);

                _logger.LogInformation(
                    "OCR: imagen cargada correctamente");

                _logger.LogInformation(
                    "OCR: procesando imagen");

                using var page = engine.Process(
                    img,
                    PageSegMode.SingleLine);

                _logger.LogInformation(
                    "OCR: procesamiento terminado");

                string texto = page.GetText() ?? "";

                _logger.LogInformation(
                    "OCR: texto detectado={Texto}",
                    texto);

                return texto;
            }
            finally
            {
                if (System.IO.File.Exists(archivoTemporal))
                {
                    System.IO.File.Delete(archivoTemporal);

                    _logger.LogInformation(
                        "OCR: archivo temporal eliminado");
                }
            }
        }
    }
}