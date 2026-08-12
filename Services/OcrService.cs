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
            Console.WriteLine("OCR: inicio");

            if (imagen == null || imagen.Length == 0)
            {
                Console.WriteLine("OCR: imagen vacía");
                return "";
            }

            Console.WriteLine(
                $"OCR: imagen recibida. Nombre={imagen.FileName}, Tamaño={imagen.Length}"
            );

            string tessdataPath = Path.Combine(
                _environment.ContentRootPath,
                "tessdata"
            );

            Console.WriteLine(
                $"OCR: ContentRootPath={_environment.ContentRootPath}"
            );

            Console.WriteLine(
                $"OCR: tessdataPath={tessdataPath}"
            );

            Console.WriteLine(
                $"OCR: existe carpeta={Directory.Exists(tessdataPath)}"
            );

            string trainedDataPath =
                Path.Combine(tessdataPath, "eng.traineddata");

            Console.WriteLine(
                $"OCR: eng.traineddata existe={System.IO.File.Exists(trainedDataPath)}"
            );

            string archivoTemporal = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}.png"
            );

            try
            {
                Console.WriteLine(
                    $"OCR: guardando imagen temporal {archivoTemporal}"
                );

                using (var stream = new FileStream(
                    archivoTemporal,
                    FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                Console.WriteLine(
                    "OCR: imagen temporal guardada"
                );

                Console.WriteLine(
                    "OCR: creando TesseractEngine"
                );

                using var engine = new TesseractEngine(
                    tessdataPath,
                    "eng",
                    EngineMode.Default
                );

                Console.WriteLine(
                    "OCR: TesseractEngine creado correctamente"
                );

                Console.WriteLine(
                    "OCR: cargando imagen"
                );

                using var img = Pix.LoadFromFile(
                    archivoTemporal
                );

                Console.WriteLine(
                    "OCR: imagen cargada correctamente"
                );

                Console.WriteLine(
                    "OCR: procesando imagen"
                );

                using var page = engine.Process(
                    img,
                    PageSegMode.SingleLine
                );

                Console.WriteLine(
                    "OCR: imagen procesada correctamente"
                );

                var texto = page.GetText() ?? "";

                Console.WriteLine(
                    $"OCR: texto detectado={texto}"
                );

                return texto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "===================================="
                );

                Console.WriteLine(
                    "OCR: ERROR"
                );

                Console.WriteLine(
                    $"OCR: Tipo={ex.GetType().FullName}"
                );

                Console.WriteLine(
                    $"OCR: Mensaje={ex.Message}"
                );

                Console.WriteLine(
                    $"OCR: StackTrace={ex.StackTrace}"
                );

                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"OCR: InnerException={ex.InnerException.Message}"
                    );
                }

                Console.WriteLine(
                    "===================================="
                );

                throw;
            }
            finally
            {
                if (System.IO.File.Exists(archivoTemporal))
                {
                    System.IO.File.Delete(archivoTemporal);

                    Console.WriteLine(
                        "OCR: archivo temporal eliminado"
                    );
                }
            }
        }
    }
}