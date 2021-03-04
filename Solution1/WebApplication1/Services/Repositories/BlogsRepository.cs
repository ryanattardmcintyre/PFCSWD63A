using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models.Domain;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Repositories
{
  

    public class BlogsRepository : IBlogsRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogsRepository (ApplicationDbContext context)
        { _context = context;  }

        public void AddBlog(Blog b)
        {
            b.BlogId = Guid.NewGuid();

            _context.Blogs.Add(b);
            _context.SaveChanges();
        }

        public void DeleteBlog(Guid id)
        {
            _context.Blogs.Remove(GetBlog(id));
        }

        public Blog GetBlog(Guid id)
        {
            return _context.Blogs.SingleOrDefault(x => x.BlogId == id);
        }

        public IQueryable<Blog> GetBlogs()
        {
            return _context.Blogs;
        }

        public void UpdateBlog(Blog b)
        {
            var originalBlog = GetBlog(b.BlogId);
            originalBlog.Url = b.Url;
            _context.SaveChanges();
        }
    }
}
