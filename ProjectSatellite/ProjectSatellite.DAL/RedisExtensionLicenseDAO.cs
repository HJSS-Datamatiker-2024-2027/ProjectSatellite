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

        public async Task<ExtensionLicense> GetAsync(Guid tenantId, int extensionId)
        {
            try
            {
                var data = await _redisCache.HashGetAsync($"licenses:{tenantId}", extensionId);

                ExtensionLicense license = JsonSerializer.Deserialize<ExtensionLicense>((string)data!);
                if (license == null)
                {
                    throw new Exception("Failed to deserialize license");
                }

                return license;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting license with tenantId {tenantId}. Message was {ex.Message}");
            }
        }

        public async Task<IEnumerable<ExtensionLicense>> GetAllAsync(Guid tenantId)
        {
            try
            {
                var data = await _redisCache.HashGetAllAsync($"licenses:{tenantId}");

                IEnumerable<ExtensionLicense> licenses = data.Select(extId => JsonSerializer.Deserialize<ExtensionLicense>((string)extId.Value!)).ToList();
                if (licenses == null)
                {
                    throw new Exception("Failed to deserialize licenses");
                }
                
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

                bool success = await _redisCache.HashSetAsync($"licenses:{extensionLicense.TenantId}", extensionLicense.ExtensionId, json);

                await _redisCache.KeyExpireAsync($"licenses:{extensionLicense.TenantId}", TimeSpan.FromMinutes(5));

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting extensionLicense into the cachce. Message was {ex.Message}");
            }
        }
    }
}
