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
        private readonly RestClient _restClient;

        private const string _username = "admin"; //TODO: Smid i .env
        private const string _password = "Password!"; //TODO: Smid i .env

        public BCApiClient(string baseUri)
        {
            _baseUri = baseUri;
            _restClient = new RestClient(_baseUri);
        }

        public async Task<ExtensionLicenseResponse> GetAsync()
        {
            var request = new RestRequest();
            request.Method = Method.Get;

            string authHeader = "Basic " + Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_username}:{_password}")
            );

            request.AddHeader("Authorization", authHeader);

            var response = await _restClient.ExecuteAsync<ExtensionLicenseResponse>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                throw new Exception(); //TODO: Tilføj ordentlig error
            }

            return response.Data; //TODO: Temp løsning
        }
    }
}
