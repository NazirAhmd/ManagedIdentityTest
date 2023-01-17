using Azure.Core;
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
        private readonly IList<string> _errors = new List<string>();
        private readonly IList<string> _cosmosDatabases = new List<string>();

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
            var registeredAppId = "c6240593-1709-450d-b8bd-c721567327c7";
            ViewBag.AccessToken1 = GetAccessToken(userAssignedMI);
            ViewBag.AccessToken2 = GetAccessToken();
            ViewBag.AccessToken3 = GetAccessToken(registeredAppId);
            ViewBag.Error = string.Join("------ERROR---------", _errors);
            ViewBag.Databases = string.Join("------DB---------", _cosmosDatabases); ;
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
                Database res = client.GetDatabase("Office").GetContainer("Employee").Database;
                _cosmosDatabases.Add(res.Id);
                return accessTokenString;
            }
            catch (Exception ex)
            {
                _errors.Add(ex.Message);
            }
            return "No token";
        }
    }
}