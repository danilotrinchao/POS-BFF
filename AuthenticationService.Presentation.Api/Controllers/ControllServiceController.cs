using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllServiceController : ControllerBase
    {
        private readonly IConsumerService _consumerService;
        public ControllServiceController(IConsumerService consumer)
        {
            _consumerService = consumer;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetSaleById(int userid)
        {
            try
            {
                var service = await _consumerService.GetServicesByUserId(userid);
                if (service == null)
                {
                    return NotFound();
                }
                return Ok(service);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve services: {ex.Message}");
            }
        }

        [HttpPut("stop/{id}")]
        public async Task<IActionResult> StopConsumerService(Guid id, int timeLeft)
        {
            try
            {
                var result = await _consumerService.UpdateConsumerService(id, timeLeft);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to stop sservice: {ex.Message}");
            }
        }
    }
}
