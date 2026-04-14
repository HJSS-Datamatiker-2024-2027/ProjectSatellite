using ProjectSatellite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.APIClients
{
    public interface IApiClient
    {
        public Task<ExtensionLicenseResponse> GetAsync(Guid tenantId, int extensionId);
        public Task<ExtensionLicenseResponse> GetAllAsync(Guid tenantId);
    }
}
