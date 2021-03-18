using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Domain;

namespace WebApplication1.Services.Interfaces
{
    public interface IBlogsRepository
    {
        void AddBlog(Blog b);

        Blog GetBlog(Guid id);

        void DeleteBlog(Guid id);

        IQueryable<Blog> GetBlogs();

        void UpdateBlog(Blog b);

    }
}
