using LigaTestApp.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;

namespace LigaTestApp
{
    internal class Program
    {
        private static readonly HttpClient _httpClient;
        private const string _baseUrl = "https://dog.ceo/api/";
        private const string _fileFolder = "images";

        static Program()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now:G}] Начинаем");

            try
            {
                var randomResponse = await _httpClient.GetFromJsonAsync<RandomImageResponse>($"{_httpClient.BaseAddress}breeds/image/random");

                if (randomResponse is null || randomResponse.Status != "success")
                    throw new ArgumentException("не получили случайное фото собаки :(");

                var dogBreed = randomResponse.Message.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[^2];
                var breedResponse = await _httpClient.GetFromJsonAsync<ByBreedResponse>($"{_httpClient.BaseAddress}breed/{dogBreed}/images");

                if (breedResponse is null || breedResponse.Status != "success")
                    throw new ArgumentException("не получили картинку с собакой :(");

                foreach (var item in breedResponse.Message.Take(3))
                {
                    if (TryDownloadAndSaveImage(item, out var imageName))
                        OpenImage(imageName);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Упс. Ошибка: {ex.Message}\r\nStackTrace: {ex.StackTrace}\r\n");
            }

            Console.WriteLine($"[{DateTime.Now:G}] Готово");
            Console.ReadKey();
        }

        /// <summary>
        /// Попробовать скачать и сохранить изображение
        /// </summary>
        /// <param name="url">URL источника</param>
        /// <param name="fileName">Название изображения</param>
        /// <returns></returns>
        private static bool TryDownloadAndSaveImage(string url, out string fileName)
        {
            fileName = default;

            try
            {
                byte[] imageBytes = _httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();

                if (imageBytes.Length < 1)
                    return false;

                if (!Directory.Exists(_fileFolder))
                    Directory.CreateDirectory(_fileFolder);

                fileName = url.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[^1];
                var filePath = $@"{_fileFolder}\{fileName}";

                Console.WriteLine($"[{DateTime.Now:G}] Скачали фото - {fileName}");
                File.WriteAllBytes(filePath, imageBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\r\nStackTrace: {ex.StackTrace}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Открыть изображение
        /// </summary>
        /// <param name="fileName">Название изображения</param>
        private static void OpenImage(string fileName)
        {
            var filePath = $@"{_fileFolder}\{fileName}";
            if (File.Exists(filePath))
                Process.Start("explorer.exe", $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{filePath}");
        }
    }
}