﻿using Azure.Core;
using Azure.Identity;
using ManagedIdentityTestMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace ManagedIdentityTestMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            var clientId = "d6e0b510-ae0c-4218-bb78-e5615ebe8b6e";
            GetAccessToken(clientId);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GetAccessToken(string clientId)
        {
            try
            {
                var credOptions = new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = clientId,
                    //TenantId = "b1914422-c6d8-4aec-b71b-3aa233f6165e"
                };
                var credential = new ManagedIdentityCredential(clientId);
                var accessToken = credential.GetToken(new TokenRequestContext(new[] { "https://cosmos.documents.azure.com//.default" }));
                // To print the token, you can convert it to string 
                var accessTokenString = accessToken.Token.ToString();
                ViewBag.AccessToken = accessTokenString;

                //var cosmosUrl = @"https://cosmos-eus-test.documents.azure.com:443/";
                //var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
                //var client = new CosmosClient(cosmosUrl, credential, options);
                //var res = client.CreateDatabaseIfNotExistsAsync("Office").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var error = "Error: " + ex.Message.ToString();
                ViewBag.Error = error;
            }
        }
    }
}