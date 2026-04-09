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
        private readonly IExtensionLicenseDAO _extensionLicenseDAO; //TODO: temp

        public LicensesController(IApiClient licenseApiClient, IExtensionLicenseDAO extensionLicenseDAO)
        {
            _bcApiClient = licenseApiClient;
            _extensionLicenseDAO = extensionLicenseDAO;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            try
            {
                ExtensionLicenseResponse response = await _bcApiClient.GetAsync();

                foreach (var license in response.Value)
                {
                    await _extensionLicenseDAO.InsertAsync(license);
                }

                return Ok(await _extensionLicenseDAO.GetAsync(response.Value[0].TenantId.ToString()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
