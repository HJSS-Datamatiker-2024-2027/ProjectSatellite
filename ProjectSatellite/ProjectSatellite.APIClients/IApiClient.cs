using ProjectSatellite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.APIClients
{
    public interface IApiClient
    {
        public Task<ExtensionLicense> GetAsync(Guid tenantId, Guid extensionId);
        public Task<IEnumerable<ExtensionLicense>> GetAllAsync(Guid tenantId);
        public Task<ExtensionLicense> CloudGetAsync(Guid tenantId, Guid extensionId);
        public Task<IEnumerable<ExtensionLicense>> CloudGetAllAsync(Guid tenantId);
        public Task<ExtensionLicense> CloudPostAsync(Guid tenantId, Guid extensionId);
    }
}
