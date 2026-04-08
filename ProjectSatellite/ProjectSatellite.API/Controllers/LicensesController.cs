using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectSatellite.APIClients;

namespace ProjectSatellite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicensesController : ControllerBase
    {
        private readonly IApiClient _bcApiClient;

        public LicensesController(IApiClient licenseApiClient)
        {
            _bcApiClient = licenseApiClient;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            try
            {
                return Ok(await _bcApiClient.GetAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
