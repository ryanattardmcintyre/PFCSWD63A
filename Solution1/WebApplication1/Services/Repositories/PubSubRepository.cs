using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Domain;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Repositories
{
    public class PubSubRepository : IPubSubRepository
    {
        string projectId;
        string topicId;
        public PubSubRepository(IConfiguration config)
        {
           projectId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
           topicId = config.GetSection("AppSettings").GetSection("TopicId").Value;
        }

        public void PublishMessage(string email, Blog b, string category)
        {
            TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);

            Task<PublisherClient> taskP = PublisherClient.CreateAsync(topicName);
            taskP.Wait();
            PublisherClient publisher = taskP.Result; 

            dynamic onTheFlyObject = new { email= email, blog= b }; //anonymous object
            string serializedOnTheFlyObject = JsonConvert.SerializeObject(onTheFlyObject);
            
            var pubsubMessage = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(serializedOnTheFlyObject),
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
                {
                    { "category", category } 
                }
            };
            
            Task<string> taskM = publisher.PublishAsync(pubsubMessage);
            taskM.Wait();
            string message = taskM.Result;  //a reference id is returned
        }
    }
}
