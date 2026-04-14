using ProjectSatellite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.DAL
{
    public interface IExtensionLicenseDAO
    {
        public Task<ExtensionLicense> GetAsync(Guid tenantId, int extensionId);
        public Task<IEnumerable<ExtensionLicense>> GetAllAsync(Guid tenantId);
        public Task<bool> InsertAsync(ExtensionLicense extensionLicense);
    }
}
