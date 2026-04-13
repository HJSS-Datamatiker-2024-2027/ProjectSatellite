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

        public async Task<ExtensionLicense> GetAsync(string tenantId)
        {
            try
            {
                var data = await _redisCache.StringGetAsync($"license:{tenantId}");

                if (data.IsNullOrEmpty)
                {
                    throw new Exception($"License with tenantId {tenantId} was not found!"); //TODO: Cache miss er normale, så vi der burde nok ikke smides en exception her!
                }

                var result = JsonSerializer.Deserialize<ExtensionLicense>((string)data!);
                if (result == null)
                {
                    throw new Exception("Failed to deserialize ExtensionLicense");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting license with tenantId {tenantId}. Message was {ex.Message}");
            }
        }

        public async Task<bool> InsertAsync(ExtensionLicense extensionLicense)
        {
            try
            {
                var json = JsonSerializer.Serialize(extensionLicense);

                bool success = await _redisCache.StringSetAsync($"license:{extensionLicense.TenantId}", json, TimeSpan.FromMinutes(5));

                //TODO: slet dette:
                //await _redisCache.SetAddAsync("licenses:all", extensionLicense.TenantId);

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting extensionLicense into the cachce. Message was {ex.Message}");
            }
        }
    }
}
