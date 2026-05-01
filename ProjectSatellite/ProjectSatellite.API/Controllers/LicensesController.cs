using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectSatellite.APIClients;
using ProjectSatellite.DAL;
using ProjectSatellite.Models;

namespace ProjectSatellite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicensesController : ControllerBase
    {
        private readonly IApiClient _bcApiClient;
        private readonly IExtensionLicenseDAO _extensionLicenseDAO;

        public LicensesController(IApiClient licenseApiClient, IExtensionLicenseDAO extensionLicenseDAO)
        {
            _bcApiClient = licenseApiClient;
            _extensionLicenseDAO = extensionLicenseDAO;
        }

        [HttpGet("cloud")]
        public async Task<ActionResult> GetAllAsync(Guid tenantId, string temp)
        {
            try
            {
                IEnumerable<ExtensionLicense>? cacheResult = await _extensionLicenseDAO.GetAllAsync(tenantId);

                if (cacheResult == null || !cacheResult.Any())
                {
                    IEnumerable<ExtensionLicense> bcResult = await _bcApiClient.CloudGetAllAsync(tenantId);

                    foreach (ExtensionLicense license in bcResult)
                    {
                        await _extensionLicenseDAO.InsertAsync(license);
                    }

                    return Ok(bcResult);
                }

                return Ok(cacheResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync(Guid tenantId)
        {
            try
            {
                IEnumerable<ExtensionLicense>? cacheResult = await _extensionLicenseDAO.GetAllAsync(tenantId);

                if (cacheResult == null || !cacheResult.Any())
                {
                    IEnumerable<ExtensionLicense> bcResult = await _bcApiClient.GetAllAsync(tenantId);

                    foreach (ExtensionLicense license in bcResult)
                    {
                        await _extensionLicenseDAO.InsertAsync(license);
                    }
                    
                    return Ok(bcResult);
                }
                
                return Ok(cacheResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{extensionId}")]
        public async Task<ActionResult> GetAsync(Guid tenantId, Guid extensionId)
        {
            try
            {
                ExtensionLicense? cacheResult = await _extensionLicenseDAO.GetAsync(tenantId, extensionId);

                if (cacheResult == null)
                {
                    ExtensionLicense bcResult = await _bcApiClient.GetAsync(tenantId, extensionId);

                    await _extensionLicenseDAO.InsertAsync(bcResult);

                    return Ok(bcResult);
                }

                return Ok(cacheResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(Guid tenantId, Guid extensionId)
        {
            try
            {
                bool success = await _extensionLicenseDAO.DeleteAsync(tenantId, extensionId);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
