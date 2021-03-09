using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebApplication1.Services.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private IDatabase db;
        private readonly IConfiguration _config;
        public CacheRepository(IConfiguration config)
        {
            _config = config;
            string connectionString = _config.GetConnectionString("CacheConnection");

            var cm = ConnectionMultiplexer.Connect(connectionString);
            db = cm.GetDatabase();

        }

        public List<Menu> GetMenus()
        {
            if (db.KeyExists("navbar-menus"))
            {
                string result = db.StringGet("navbar-menus");
                var menus = JsonConvert.DeserializeObject<List<Menu>>(result);
                return menus;
            }
            else return new List<Menu>();
        }

        public void UpsertMenu(Menu m)
        { 
            var originalMenusList = GetMenus();

            var myMenu = originalMenusList.SingleOrDefault(x => x.Title == m.Title);
            if (myMenu == null)
            {
                originalMenusList.Add(m);
            }
            else
            {
                myMenu.Url = m.Url;
                myMenu.Title = m.Title;
            }

            var serializedMenuList = JsonConvert.SerializeObject(originalMenusList);

            db.StringSet("navbar-menus", serializedMenuList);

        }
    }
}
