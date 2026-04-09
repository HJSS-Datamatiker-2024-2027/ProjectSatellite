using ProjectSatellite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.DAL
{
    public interface IExtensionLicenseDAO
    {
        public Task<ExtensionLicense> GetAsync(string tenantId);
        public Task<bool> InsertAsync(ExtensionLicense extensionLicense);
    }
}
