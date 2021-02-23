using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models.Domain;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Repositories
{
    public class PostsRepository: IPostsRepository
    {
        private readonly ApplicationDbContext _context;
        public PostsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddPost(Post b)
        {
            _context.Posts.Add(b);
            _context.SaveChanges();
        }

        public void DeletePost(int id)
        {
            _context.Posts.Remove(GetPost(id));
        }

        public Post GetPost(int id)
        {
            return _context.Posts.SingleOrDefault(x => x.PostId == id);
        }

        public IQueryable<Post> GetPosts()
        {
            return _context.Posts;
        }

        public IQueryable<Post> GetPosts(int blogId)
        {
            return GetPosts().Where(x => x.BlogId == blogId);
        }

        public void UpdatePost(Post b)
        {
            var originalPost = GetPost(b.PostId);
            originalPost.Content = b.Content;
            originalPost.Title = b.Title;
            originalPost.BlogId = b.BlogId;
            _context.SaveChanges();
        }
    }
}
