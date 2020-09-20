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
        private static readonly string appUrl = "https://localhost:44332";
        private static readonly string esepteKomnataUrl = "https://localhost:44370";

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

            // получить распознования
            komnatas = await RequestToEsepteKomnata(komnatas);

            // вернуть результаты
            ViewBag.Recognized = "";
            
            // удалить файлы
            //await DeleteFiles(komnatas);
            ViewBag.Komnatas = komnatas;

            return View("EsepteKomnata");
        }


        private async Task<List<Komnata>> RequestToEsepteKomnata(List<Komnata> komnatas)
        {
            foreach (Komnata komnata in komnatas)
            {
                // запрос
                var response = await client.GetAsync(esepteKomnataUrl + "/komnata?imageLink=" + komnata.ImageLink);

                // ответ
                var responseString = await response.Content.ReadAsStringAsync();

                // конвертирование
                KomnataJSON komnataJSON = JsonSerializer.Deserialize<KomnataJSON>(responseString);
                komnata.KomnataType = komnataJSON.komnataType;

                Debug.Print("EsepteSite: " + komnata.KomnataType);
            }

            return komnatas;
        }

        // Сохранение файлов
        private async Task<List<Komnata>> SaveFiles(IFormFileCollection uploads)
        {
            List<Komnata> komnatas = new List<Komnata>();

            int id = 1;
            foreach (var uploadedFile in uploads)
            {
                string imageName = "Image_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_ffff") + Path.GetExtension(uploadedFile.FileName);

                // путь к папке Files
                string path = _appEnvironment.WebRootPath  + "/Uploads/" + imageName;

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                komnatas.Add(new Komnata { Id = "collapse_" + id, ImageLink = appUrl + "/Uploads/" + imageName, ImagePath = path });
                id++;
                Debug.Print("EsepteSite: " + komnatas.Last().ImageLink);
            }

            return komnatas;
        }

        private async Task DeleteFiles(List<Komnata> komnatas)
        {
            foreach (var komnata in komnatas)
            {
                System.IO.File.Delete(komnata.ImagePath);
            }
        }

    }
}
