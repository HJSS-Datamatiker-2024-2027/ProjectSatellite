using ProjectSatellite.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ProjectSatellite.DAL
{
    public class RedisExtensionLicenseDAO : IExtensionLicenseDAO
    {
        private readonly IDatabase _redisCache;

        public RedisExtensionLicenseDAO(IDatabase redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<ExtensionLicense?> GetAsync(Guid tenantId, Guid extensionId)
        {
            try
            {
                var data = await _redisCache.HashGetAsync($"licenses:{tenantId}", extensionId.ToString());
                if (data.IsNullOrEmpty)
                {
                    return null;
                }

                ExtensionLicense? license = JsonSerializer.Deserialize<ExtensionLicense>(data.ToString());

                return license;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting license with tenantId {tenantId}. Message was {ex.Message}");
            }
        }

        public async Task<IEnumerable<ExtensionLicense>?> GetAllAsync(Guid tenantId)
        {
            try
            {
                var data = await _redisCache.HashGetAllAsync($"licenses:{tenantId}");
                if (data.Length == 0)
                {
                    return null;
                }

                IEnumerable<ExtensionLicense> licenses = data.Select(extId => JsonSerializer.Deserialize<ExtensionLicense>(extId.Value.ToString())).Where(extLicense => extLicense != null).Select(extLicense => extLicense!).ToList();
                
                return licenses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all licenses with tenantId {tenantId}. Message was {ex.Message}");
            }
        }

        public async Task<bool> InsertAsync(ExtensionLicense extensionLicense)
        {
            try
            {
                var json = JsonSerializer.Serialize(extensionLicense);

                bool success = await _redisCache.HashSetAsync($"licenses:{extensionLicense.TenantId}", extensionLicense.ExtensionId.ToString(), json);

                //await _redisCache.KeyExpireAsync($"licenses:{extensionLicense.TenantId}", TimeSpan.FromMinutes(5));

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting extensionLicense into the cachce. Message was {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(Guid tenantId, Guid extensionId)
        {
            try
            {
                bool success = await _redisCache.HashDeleteAsync($"licenses:{tenantId}", extensionId.ToString());

                return success;
            } 
            catch (Exception ex)
            {
                throw new Exception($"Error deleting extensionLicense with tenantId {tenantId} and extensionId {extensionId}. Message was {ex.Message}");
            }
        }
    }
}
