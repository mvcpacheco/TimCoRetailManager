﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.Helpers
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient apiClient;

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            var api = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (var response = await apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<AuthenticatedUser>();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}