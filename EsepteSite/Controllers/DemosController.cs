using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EsepteSite.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;

namespace EsepteSite.Controllers
{
    public class DemosController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private static readonly HttpClient client = new HttpClient();


        public DemosController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult EsepteKomnata()
        {
            ViewBag.Recognized = "";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecognizeKomnata(IFormFileCollection uploads)
        {
            // сохранить файлы
            List<Komnata> komnatas = await SaveFiles(uploads);

            komnatas = await RequestToEsepteKomnata(komnatas);

            ViewBag.Recognized = "";

            foreach (Komnata komnata in komnatas)
            {
                ViewBag.Recognized += komnata.KomnataType + " ";
            }

            return View("EsepteKomnata");
        }


        private async Task<List<Komnata>> RequestToEsepteKomnata(List<Komnata> komnatas)
        {
            foreach (Komnata komnata in komnatas)
            {
                var response = await client.GetAsync("http://localhost:54858/komnata?imageName=" + komnata.ImageName);

                var responseString = await response.Content.ReadAsStringAsync();

                KomnataJSON komnataJSON = JsonSerializer.Deserialize<KomnataJSON>(responseString);
                komnata.KomnataType = komnataJSON.komnataType;

                Debug.Print(komnata.KomnataType);
            }

            return komnatas;
        }

        // Сохранение файлов
        private async Task<List<Komnata>> SaveFiles(IFormFileCollection uploads)
        {
            List<Komnata> komnatas = new List<Komnata>();

            foreach (var uploadedFile in uploads)
            {
                // путь к папке Files
                string path = "C:\\Users\\esept\\Downloads\\EsepteKomnataUploads\\" + uploadedFile.FileName;

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                komnatas.Add(new Komnata { ImageName = uploadedFile.FileName, ImagePath = path });
            }

            return komnatas;
        }










        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
