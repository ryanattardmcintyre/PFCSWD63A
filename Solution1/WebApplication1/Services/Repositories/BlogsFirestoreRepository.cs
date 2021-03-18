using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Domain;
using WebApplication1.Services.Interfaces;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services.Repositories
{
    public class BlogsFirestoreRepository : IBlogsRepository
    {

        FirestoreDb db;
        public BlogsFirestoreRepository(IConfiguration config)
        {
            var projId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
            db = FirestoreDb.Create(projId);
        }


        //Collections >> Tables
        //Documents >> Rows

        public void AddBlog(Blog b)
        {
            DocumentReference docRef = db.Collection("blogs").Document();
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString()},
                { "Title", b.Title },
                { "Url", b.Url }
            };
          
            docRef.SetAsync(city).Wait();
        }

        public void DeleteBlog(Guid id)
        {
            Query capitalQuery = db.Collection("blogs").WhereEqualTo("Id", id.ToString());
            Task<QuerySnapshot> task = capitalQuery.GetSnapshotAsync();
            task.Wait();

            QuerySnapshot capitalQuerySnapshot = task.Result;
            DocumentSnapshot documentSnapshot = capitalQuerySnapshot.Documents[0];

            DocumentReference cityRef = documentSnapshot.Reference;
            cityRef.DeleteAsync().Wait();
        }

        public Blog GetBlog(Guid id)
        {
            Query capitalQuery = db.Collection("blogs").WhereEqualTo("Id", id.ToString());
            Task<QuerySnapshot> task = capitalQuery.GetSnapshotAsync();
            task.Wait();

            QuerySnapshot capitalQuerySnapshot = task.Result;
            DocumentSnapshot documentSnapshot = capitalQuerySnapshot.Documents[0];
            
            Dictionary<string, object> city = documentSnapshot.ToDictionary();

            Blog b = new Blog()
            {
                BlogId = city.ContainsKey("Id")? new Guid(city["Id"].ToString()) : new Guid(),
                Title = city.ContainsKey("Title") ? city["Title"].ToString() : "",
                Url = city.ContainsKey("Url") ? city["Url"].ToString(): ""
            };

            return b;
        }

        public IQueryable<Blog> GetBlogs()
        {
            Query allCitiesQuery = db.Collection("blogs");
            Task<QuerySnapshot> result =  allCitiesQuery.GetSnapshotAsync();
            result.Wait();
            QuerySnapshot allCitiesQuerySnapshot = result.Result;

            List<Blog> blogs = new List<Blog>();

            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Dictionary<string, object> city = documentSnapshot.ToDictionary();

                Blog b = new Blog()
                {
                    BlogId = city.ContainsKey("Id") ? new Guid(city["Id"].ToString()) : new Guid(),
                    Title = city.ContainsKey("Title") ? city["Title"].ToString() : "",
                    Url = city.ContainsKey("Url") ? city["Url"].ToString() : ""
                };

                blogs.Add(b);
            }

            return blogs.AsQueryable();
        }

        public void UpdateBlog(Blog b)
        {
            Query capitalQuery = db.Collection("blogs").WhereEqualTo("Id", b.BlogId.ToString());
            Task<QuerySnapshot> task = capitalQuery.GetSnapshotAsync();
            task.Wait();

            QuerySnapshot capitalQuerySnapshot = task.Result;
            DocumentSnapshot documentSnapshot = capitalQuerySnapshot.Documents[0];

            DocumentReference cityRef = documentSnapshot.Reference;

            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Id", Guid.NewGuid().ToString()},
                { "Title", b.Title },
                { "Url", b.Url }
            };

            cityRef.SetAsync(city).Wait();


        }
    }
}
