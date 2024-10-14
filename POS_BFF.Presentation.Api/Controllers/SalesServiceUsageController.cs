using POS_BFF.Core.Domain.Gateways.Sales;
using Microsoft.AspNetCore.Mvc;

namespace POS_BFF.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesServiceUsageController : ControllerBase
    {
        private readonly ISalesServiceUsageGateway _salesServiceUsageGateway;
        private readonly ILogger<SalesServiceUsageController> _logger;

        public SalesServiceUsageController(ISalesServiceUsageGateway salesServiceUsageGateway, ILogger<SalesServiceUsageController> logger)
        {
            _salesServiceUsageGateway = salesServiceUsageGateway;
            _logger = logger;
        }

        [HttpPost("start/{orderItemId}")]
        public async Task<IActionResult> StartService(Guid orderItemId, Guid TenantId)
        {
            try
            {
                await _salesServiceUsageGateway.StartServiceAsync(orderItemId, TenantId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to start service");
            }
        }

        [HttpPost("pause/{orderItemId}")]
        public async Task<IActionResult> PauseService(Guid orderItemId, Guid TenantId)
        {
            try
            {
                await _salesServiceUsageGateway.PauseServiceAsync(orderItemId, TenantId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error pausing service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to pause service");
            }
        }

        [HttpPost("stop/{orderItemId}")]
        public async Task<IActionResult> StopService(Guid orderItemId, Guid TenantId)
        {
            try
            {
                await _salesServiceUsageGateway.StopServiceAsync(orderItemId, TenantId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error stopping service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to stop service");
            }
        }

        [HttpGet("{orderItemId}")]
        public async Task<IActionResult> GetServiceUsage(Guid orderItemId, Guid TenantId)
        {
            try
            {
                var serviceUsage = await _salesServiceUsageGateway.GetServiceUsageByOrderItemIdAsync(orderItemId, TenantId);
                return Ok(serviceUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service usage for order item ID {orderItemId}");
                return StatusCode(500, "Failed to retrieve service usage");
            }
        }
    }
}
