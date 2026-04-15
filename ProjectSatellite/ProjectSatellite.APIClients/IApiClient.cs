using ProjectSatellite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.APIClients
{
    public interface IApiClient
    {
        public Task<ExtensionLicense> GetAsync(Guid tenantId, int extensionId);
        public Task<IEnumerable<ExtensionLicense>> GetAllAsync(Guid tenantId);
    }
}
