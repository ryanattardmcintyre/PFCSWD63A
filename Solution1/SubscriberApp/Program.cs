using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace SubscriberApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SendEmail();

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                @"C:\Users\Ryan\Downloads\pfc2021-05871ada539e.json");

            string projectId = "pfc2021";
            string subscriptionId = "pfcsubscriptionswd63a";

            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0; List<string> messages = new List<string>();
            bool acknowledge = false; //for testing purposes only!!
            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, returnImmediately: true, maxMessages: 1);
                // Print out each received message.
                foreach (ReceivedMessage msg in response.ReceivedMessages)
                {
                    string text = msg.Message.Data.ToStringUtf8();
                    messages.Add(text);

                    Console.WriteLine($"Message {msg.Message.MessageId}: {text}");
                    Interlocked.Increment(ref messageCount);
 
                }

                //send email
                dynamic deserializedObject = JsonConvert.DeserializeObject(messages[0].ToString());
                string email = (string) deserializedObject.email;
                string title = (string)deserializedObject.blog.Title;
                bool emailSent = SendEmail( email, title);

                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
 

                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                //logging
            }

        }

        static bool SendEmail(string recipient, string title)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3"); 
            client.Authenticator = new HttpBasicAuthenticator("api", "");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "");
            request.AddParameter("to", recipient);
            request.AddParameter("subject", "testing pfc demo");
            request.AddParameter("text", "A new blog with the name " + title + " has been created");
            request.Method = Method.POST;
            var statusCode= client.Execute(request).StatusCode;
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else return false;
           
        }


        static void SendEmail()
        {
            HttpClient client = new HttpClient();
            var uri = new Uri("https://us-central1-pfc2021.cloudfunctions.net/hello-http-function3");

            var t =  client.GetAsync(uri);
            t.Wait();
            Task<string> t2 = t.Result.Content.ReadAsStringAsync();
            t2.Wait();
            string msg = t2.Result;

            Console.ReadLine();
        }
    }
}
