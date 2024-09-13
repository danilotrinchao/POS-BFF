using POS_BFF.Core.Domain.Gateways.Cashier;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using POS_BFF.Core.Domain.Gateways.Authentication;

namespace POS_BFF.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashierOrderController : ControllerBase
    {
        private readonly ICashierOrderServiceGateway _cashierOrderServiceGateway;
        private readonly ISaleOrderServiceGateway _saleOrderServiceGateway;
      

        public CashierOrderController(ICashierOrderServiceGateway cashierOrderServiceGateway, ISaleOrderServiceGateway saleOrderServiceGateway)
        {
            _cashierOrderServiceGateway = cashierOrderServiceGateway;
            _saleOrderServiceGateway = saleOrderServiceGateway;
        }

        [HttpPost("open")]
        public async Task<ActionResult> OpenCashier([FromBody]decimal initialBalance, int employeerId, Guid tentantId)
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

        [HttpPut("close/{employeerId}")]
        public async Task<ActionResult> CloseCashier(int employeerId)
        {
            try
            {
                var totals = await _saleOrderServiceGateway.GetDailyTotals();
                var success = await _cashierOrderServiceGateway.CloseCashier(employeerId, totals);
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


        [HttpGet("getCashier")]
        public async Task<ActionResult> GetIsCashierOpenedEmployeerId(int employeerId)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.GetOpenedCashierByEmployeerId(employeerId);
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
    }
}
