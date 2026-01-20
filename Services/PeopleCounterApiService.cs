using PeopleCounterDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace PeopleCounterDesktop.Services
{
    public class PeopleCounterApiService
    {
        private readonly HttpClient _httpClient = ApiClient.Client;

        public async Task<List<BuildingSummary>> GetBuildings()
        {
            return await _httpClient
                .GetFromJsonAsync<List<BuildingSummary>>("mqtt/buildings")
                ?? new List<BuildingSummary>();
        }

        public async Task<List<PeopleCounterModel>> GetSensors(string building)
        {
            return await _httpClient.GetFromJsonAsync<List<PeopleCounterModel>>($"mqtt/building/{building}");
        }

        public async Task<bool> Login(LoginRequest request) {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            if (!response.IsSuccessStatusCode)
                return false;

            if (response.Headers.TryGetValues("Set-Cookie",out var cookies))
            {
                var raw = cookies.First();
                var cookie = raw.Split(';')[0];
                await AuthCookieStore.SaveCookie(cookie);
            }

            return true;

        }

        public async Task<bool> IsLoggedInAsync()
        {
            var response = await _httpClient.GetAsync("auth/me");
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _httpClient.PostAsync("auth/logout", null);
        }

        public async Task ResetDevice(string device) {

            var response = await _httpClient.PostAsync($"device/{device}/reset",null);


            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exception("You are not authorized (Admin only)");
                

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Not logged in");
         

            response.EnsureSuccessStatusCode();
        }


        public async Task ResetBuilding(string building)
        {
            var response = await _httpClient
                .PostAsync($"device/building/{Uri.EscapeDataString(building)}/reset", null);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Not logged in");

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Admin access required");

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<SegmentChartModel>> GetChartAsync(
                string deviceId,
                DateTime from,
                DateTime to,
                string bucket)
        {
            var url =
                $"device/chart?" +
                $"deviceId={deviceId}" +
                $"&from={from:yyyy-MM-ddTHH:mm:ss}" +
                $"&to={to:yyyy-MM-ddTHH:mm:ss}" +
                $"&bucket={bucket}";

            var result = await _httpClient.GetFromJsonAsync<List<SegmentChartModel>>(url);
            return result ?? new();
        }

        public async Task<List<SensorTrendDto>> GetTrendAsync(string deviceId, DateTime from, DateTime to, string bucket)
        {
            var fromStr = from.ToString("o");
            var toStr = to.ToString("o");

            var url =
                $"device/trend?" +
                $"deviceId={Uri.EscapeDataString(deviceId)}" +
                $"&from={Uri.EscapeDataString(fromStr)}" +
                $"&to={Uri.EscapeDataString(toStr)}" +
                $"&bucket={bucket}";

            return await _httpClient.GetFromJsonAsync<List<SensorTrendDto>>(url)
                   ?? new();
        }


        public async Task<List<SensorTrendDto>> GetTrendLocationAsync(string location, DateTime from, DateTime to, string bucket)
        {
            var fromStr = from.ToString("o");
            var toStr = to.ToString("o");

            var url =
                $"device/trendlocation?" +
                $"location={Uri.EscapeDataString(location)}" +
                $"&from={Uri.EscapeDataString(fromStr)}" +
                $"&to={Uri.EscapeDataString(toStr)}" +
                $"&bucket={bucket}";

            return await _httpClient.GetFromJsonAsync<List<SensorTrendDto>>(url)
                   ?? new();
        }


        public async Task<List<string>> GetDevicesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<string>>("device/list")
                   ?? new();
        }
        public async Task<List<string>> GetLocationAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<string>>("device/location")
                   ?? new();
        }

    }
}
