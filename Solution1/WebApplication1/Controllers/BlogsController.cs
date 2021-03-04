using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models.Domain;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Repositories;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {

        private readonly IBlogsRepository _blogsRepo;
        private readonly IConfiguration _config;

        public BlogsController(IBlogsRepository blogsRepo, IConfiguration config)
        {
            _config = config;
            _blogsRepo = blogsRepo;
        }


        public IActionResult Index()
        {
            var list = _blogsRepo.GetBlogs();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(IFormFile logo, Blog b)
        {
            if (_blogsRepo.GetBlogs().Where(x => x.Url == b.Url).Count() == 0)
            {
                _blogsRepo.AddBlog(b);
            }
            else
                TempData["warning"] = "Blog exists already";

            string bucketName = _config.GetSection("AppSettings").GetSection("LogosBucket").Value;

            //1. Upload a picture on cloud storage
            if (logo != null)
            {
                var storage = StorageClient.Create();

                string uniqueFilename = Guid.NewGuid() + System.IO.Path.GetExtension(logo.FileName);

                using (var myfile = logo.OpenReadStream())
                {
                    storage.UploadObject(bucketName, uniqueFilename, null, myfile);
                }
                //2. add a reference to the picture to the blog > b
                b.Url = $"https://storage.googleapis.com/{bucketName}/{uniqueFilename}";

                //3. save everything into the db
                _blogsRepo.AddBlog(b);
            }


            return RedirectToAction("Index");
        }
    }
}
