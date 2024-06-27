using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requests;
using AuthenticationService.Infra.ExternalServices.SalesGateway;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISaleOrderServiceGateway _salesOrderServiceGateway;

        public SalesOrderController(ISaleOrderServiceGateway salesOrderServiceGateway)
        {
            _salesOrderServiceGateway = salesOrderServiceGateway;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO saleDto)
        {
            try
            {

                var saleId = await _salesOrderServiceGateway.CreateSaleAsync(saleDto);
                return CreatedAtAction(nameof(GetSaleById), new { id = saleId }, null);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create sale: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            try
            {
                var sale = await _salesOrderServiceGateway.GetSaleByIdAsync(id);
                if (sale == null)
                {
                    return NotFound();
                }
                return Ok(sale);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve sale: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSales()
        {
            try
            {
                var sales = await _salesOrderServiceGateway.GetAllSalesAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve sales: {ex.Message}");
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteSale(Guid id, [FromBody] SaleDTO saleDTO)
        {
            try
            {
                var result = await _salesOrderServiceGateway.CompleteSaleAsync(id, saleDTO);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to complete sale: {ex.Message}");
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelSale(Guid id)
        {
            try
            {
                var result = await _salesOrderServiceGateway.CancelSaleAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to cancel sale: {ex.Message}");
            }
        }
    }
}
