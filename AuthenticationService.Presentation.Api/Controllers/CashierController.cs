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

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
        {
            try
            {
                var order = await _cashierOrderServiceGateway.GetOrderByIdAsync(id);
                return Ok(order);
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _cashierOrderServiceGateway.GetAllOrdersAsync();
                return Ok(orders);
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

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] SaleDTO saleDto)
        {
            try
            {
                var orderId = await _cashierOrderServiceGateway.CreateOrderAsync(saleDto);
                return Ok(orderId);
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

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrder(Guid id, [FromBody] SaleDTO saleDto)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.UpdateOrderAsync(id, saleDto);
                if (success)
                {
                    return NoContent();
                }
                return NotFound();
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(Guid id)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.DeleteOrderAsync(id);
                if (success)
                {
                    return NoContent();
                }
                return NotFound();
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

        [HttpPost("open")]
        public async Task<ActionResult> OpenCashier(decimal initialBalance, int employeerId)
        {
            try
            {
                var success = await _cashierOrderServiceGateway.OpenCashier(initialBalance, employeerId);
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
