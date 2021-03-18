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
        private readonly IPubSubRepository _pubSubRepo;

        public BlogsController(IBlogsRepository blogsRepo, IConfiguration config, IPubSubRepository pubSubRepo)
        {
            _pubSubRepo = pubSubRepo;
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
            //if (_blogsRepo.GetBlogs().Where(x => x.Url == b.Url).Count() == 0)
            //{
            //    _blogsRepo.AddBlog(b);
            //}
            //else
            //    TempData["warning"] = "Blog exists already";

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

                //4. eventually send an email as a receipt back to the user confirming that blog was saved
                // but first we need to add this task to a queue

                _pubSubRepo.PublishMessage(HttpContext.User.Identity.Name, b, "flower");

            }


            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid id)
        {
            _blogsRepo.DeleteBlog(id);
            return RedirectToAction("Index");
        }
    }
}
