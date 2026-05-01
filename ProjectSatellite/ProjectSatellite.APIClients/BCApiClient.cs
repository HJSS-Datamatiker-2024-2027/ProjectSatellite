using Microsoft.Identity.Client;
using ProjectSatellite.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.APIClients
{
    public class BCApiClient : IApiClient
    {
        private readonly string _baseUri;
        private readonly string _cloudBaseUri;
        private readonly RestClient _restClient;
        private readonly RestClient _cloudRestClient;

        private const string _username = "admin"; //TODO: Smid i .env
        private const string _password = "Password!"; //TODO: Smid i .env

        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public BCApiClient(string baseUri, string cloudBaseUri, string tenantId, string clientId, string clientSecret)
        {
            _baseUri = baseUri;
            _cloudBaseUri = cloudBaseUri;
            _restClient = new RestClient(_baseUri);
            _cloudRestClient = new RestClient(_cloudBaseUri);
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<ExtensionLicense> GetAsync(Guid tenantId, Guid extensionId)
        {
            var request = new RestRequest();
            request.Method = Method.Get;

            string authHeader = "Basic " + Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_username}:{_password}")
            );

            request.AddHeader("Authorization", authHeader);

            request.AddQueryParameter("$filter", $"tenantId eq {tenantId} and extensionId eq {extensionId}");

            var response = await _restClient.ExecuteAsync<ExtensionLicenseResponse>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                throw new Exception($"Error getting ExtensionLicense with tenantId {tenantId} and extensionId {extensionId}.");
            }

            return response.Data.Value.First();
        }

        public async Task<IEnumerable<ExtensionLicense>> GetAllAsync(Guid tenantId)
        {
            var request = new RestRequest();
            request.Method = Method.Get;

            string authHeader = "Basic " + Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_username}:{_password}")
            );

            request.AddHeader("Authorization", authHeader);

            request.AddQueryParameter("$filter", $"tenantId eq {tenantId}");

            var response = await _restClient.ExecuteAsync<ExtensionLicenseResponse>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                throw new Exception($"Error getting ExtensionLicense with tenantId {tenantId}.");
            }

            return response.Data.Value;
        }

        public async Task<ExtensionLicense> CloudGetAsync(Guid tenantId, Guid extenstionId)
        {
            throw new NotImplementedException();
        } 

        public async Task<IEnumerable<ExtensionLicense>> CloudGetAllAsync(Guid tenantId)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithClientSecret(_clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{_tenantId}")
                .Build();

            var scopes = new[] { "https://api.businesscentral.dynamics.com/.default" };
            var tokenResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            string accessToken = tokenResult.AccessToken;

            Console.WriteLine(accessToken);

            var request = new RestRequest();
            request.Method = Method.Get;

            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddQueryParameter("company", "CRONUS Danmark A/S");
            request.AddQueryParameter("$filter", $"tenantId eq {_tenantId}", false);

            var response = await _cloudRestClient.ExecuteAsync<ExtensionLicenseResponse>(request);

            Console.WriteLine(response.ResponseUri);

            if (!response.IsSuccessful || response.Data == null)
            {
                throw new Exception($"Error getting ExtensionLicense with tenantId {tenantId}. " +
                    $"Status: {response.StatusCode} " +
                    $"Description: {response.StatusDescription} " +
                    $"Content: {response.Content} " +
                    $"URI: {response.ResponseUri}");
            }

            return response.Data.Value;
        }
    }
}
