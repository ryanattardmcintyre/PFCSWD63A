using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Domain;

namespace WebApplication1.Services.Interfaces
{
    public interface IPubSubRepository
    {
        void PublishMessage(string email, Blog b, string category);


    }
}
