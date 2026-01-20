using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PeopleCounterDesktop.Services
{
    public static class ApiClient
    {
        public static CookieContainer CookieContainer { get; } = new();
        public static HttpClient Client { get; }

        static ApiClient()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = CookieContainer
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
        }
    }
}
