using AuthenticationService.Core.Domain.Gateways.Cashier;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashierOrderController : ControllerBase
    {
        private readonly ICashierOrderServiceGateway _cashierOrderServiceGateway;

        public CashierOrderController(ICashierOrderServiceGateway cashierOrderServiceGateway)
        {
            _cashierOrderServiceGateway = cashierOrderServiceGateway;
        }

        [HttpPost("open")]
        public async Task<ActionResult> OpenCashier([FromBody]decimal initialBalance, int employeerId)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.OpenCashier(initialBalance, employeerId);
                if (success != null)
                {
                    return Ok(success);
                }
                return BadRequest();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("close/{cashierId}")]
        public async Task<ActionResult> CloseCashier(Guid cashierId)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.CloseCashier(cashierId);
                if (success)
                {
                    return NoContent();
                }
                return BadRequest();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
