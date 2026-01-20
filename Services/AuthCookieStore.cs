using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleCounterDesktop.Services
{
    public static class AuthCookieStore
    {
        private const string CookieKey = "AuthCookie";
        private const string UserKey = "User";

        public static async Task SaveCookie(string cookie)
        {
            await SecureStorage.SetAsync(CookieKey, cookie);
        }

        public static async Task<string?> GetCookie() { 
            return await SecureStorage.GetAsync(CookieKey);
        }
        public static void ClearCookie()
        {
            SecureStorage.Remove(CookieKey);
        }

        public static async Task SaveUser(string user)
        {
            await SecureStorage.SetAsync(UserKey, user);
        }

        public static async Task<string?> GetUser()
        {
            return await SecureStorage.GetAsync(UserKey);
        }
        public static void ClearUser()
        {
            SecureStorage.Remove(UserKey);
        }
    }
}
