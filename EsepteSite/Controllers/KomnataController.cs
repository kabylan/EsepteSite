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
    public class KomnataController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private static readonly HttpClient client = new HttpClient();
        private static readonly string appUrl = "https://localhost:44332";
        //private static readonly string apiUrl = "https://localhost:44370/komnata";
        private static readonly string apiUrl = "http://79.174.12.133/komnata";

        public KomnataController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            ViewBag.Recognized = "";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Recognize(IFormFileCollection uploads)
        {
            // сохранить файлы
            List<Model> komnatas = await SaveFiles(uploads);

            // получить распознования
            komnatas = await RequestToEsepteAPI(komnatas);

            // вернуть результаты
            ViewBag.Recognized = "";
            
            // удалить файлы
            //await DeleteFiles(komnatas);
            ViewBag.Results = komnatas;

            return View("Index");
        }


        private async Task<List<Model>> RequestToEsepteAPI(List<Model> komnatas)
        {
            foreach (Model komnata in komnatas)
            {
                // запрос
                var response = await client.GetAsync(apiUrl + "?imageLink=" + komnata.ImageLink);

                // ответ
                var responseString = await response.Content.ReadAsStringAsync();

                // конвертирование
                ModelJSON jsonResult = JsonSerializer.Deserialize<ModelJSON>(responseString);
                komnata.TypeRU = jsonResult.typeRU;

                Debug.Print("EsepteSite: " + komnata.TypeRU);
            }

            return komnatas;
        }

        // Сохранение файлов
        private async Task<List<Model>> SaveFiles(IFormFileCollection uploads)
        {
            List<Model> komnatas = new List<Model>();

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
                komnatas.Add(new Model { Id = "collapse_" + id, ImageLink = appUrl + "/Uploads/" + imageName, ImagePath = path });
                id++;
                Debug.Print("EsepteSite: " + komnatas.Last().ImageLink);
            }

            return komnatas;
        }

        private async Task DeleteFiles(List<Model> komnatas)
        {
            foreach (var komnata in komnatas)
            {
                System.IO.File.Delete(komnata.ImagePath);
            }
        }

    }
}
