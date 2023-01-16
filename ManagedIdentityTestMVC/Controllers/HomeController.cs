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
            var userAssignedMI = "6d6870f0-dc65-49d6-aadc-7c219c889e8f";
            ViewBag.AccessToken1 = GetAccessToken(userAssignedMI);
            ViewBag.AccessToken2 = GetAccessToken();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetAccessToken(string clientId = "")
        {
            try
            {
                var credential = string.IsNullOrEmpty(clientId) ? new ManagedIdentityCredential() : new ManagedIdentityCredential(clientId);
                var accessToken = credential.GetToken(new TokenRequestContext(new[] { "https://cosmos.documents.azure.com//.default" }));
                // To print the token, you can convert it to string 
                var accessTokenString = accessToken.Token.ToString();

                var cosmosUrl = @"https://cosmos-eus-test.documents.azure.com:443/";
                var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
                var client = new CosmosClient(cosmosUrl, credential, options);
                var res = client.GetDatabase("Office");
                return accessTokenString;
            }
            catch (Exception ex)
            {
                var error = "Error: " + ex.Message.ToString();
                ViewBag.Error = error;
            }
            return "No token";
        }
    }
}