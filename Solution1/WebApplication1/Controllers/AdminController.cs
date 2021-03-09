﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {

        private readonly ICacheRepository _cacheRepo;
        public AdminController(ICacheRepository cacheRepo)
        {
            _cacheRepo = cacheRepo;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Menu m)
        {
            _cacheRepo.UpsertMenu(m);
            return View();

        }
    }
}
