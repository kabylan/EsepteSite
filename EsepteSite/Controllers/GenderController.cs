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
    public class GenderController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private static readonly HttpClient client = new HttpClient();
        //private static readonly string appUrl = "http://esepte.com";
        //private static readonly string apiUrl = "http://api.esepte.com/Gender";

        private static readonly string appUrl = "https://localhost:44332";
        private static readonly string apiUrl = "https://localhost:44370/Gender";

        public GenderController(IWebHostEnvironment appEnvironment)
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
            List<Model> genders = await SaveFiles(uploads);

            // получить распознования
            genders = await RequestToEsepteAPI(genders);

            // вернуть результаты
            ViewBag.Recognized = "";

            // удалить файлы
            //await DeleteFiles(genders);
            ViewBag.Results = genders;


            return View("Index");
        }


        private async Task<List<Model>> RequestToEsepteAPI(List<Model> genders)
        {
            foreach (Model gender in genders)
            {
                // запрос
                var response = await client.GetAsync(apiUrl + "?imageLink=" + gender.ImageLink);

                // ответ
                var responseString = await response.Content.ReadAsStringAsync();

                // конвертирование
                ModelJSON jsonResult = JsonSerializer.Deserialize<ModelJSON>(responseString);
                gender.TypeRU = jsonResult.typeRU;

                Debug.Print("EsepteSite: " + gender.TypeRU);
            }

            return genders;
        }

        // Сохранение файлов
        private async Task<List<Model>> SaveFiles(IFormFileCollection uploads)
        {
            List<Model> genders = new List<Model>();

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
                genders.Add(new Model { Id = "collapse_" + id, ImageLink = appUrl + "/Uploads/" + imageName, ImagePath = path });
                id++;
                Debug.Print("EsepteSite: " + genders.Last().ImageLink);
            }

            return genders;
        }

        private async Task DeleteFiles(List<Model> genders)
        {
            foreach (var gender in genders)
            {
                System.IO.File.Delete(gender.ImagePath);
            }
        }

    }
}
