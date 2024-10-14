using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Requests;
using POS_BFF.Infra.ExternalServices.SalesGateway;
using Microsoft.AspNetCore.Mvc;

namespace POS_BFF.Presentation.Api.Controllers
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
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO saleDto, Guid tenantId)
        {
            try
            {

                var saleId = await _salesOrderServiceGateway.CreateSaleAsync(saleDto, tenantId);
                return CreatedAtAction(nameof(GetSaleById), new { id = saleId }, null);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create sale: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleById(Guid id, Guid TenantId)
        {
            try
            {
                var sale = await _salesOrderServiceGateway.GetSaleByIdAsync(id, TenantId);
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
        public async Task<IActionResult> GetAllSales(Guid tenantId)
        {
            try
            {
                var sales = await _salesOrderServiceGateway.GetAllSalesAsync(tenantId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve sales: {ex.Message}");
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteSale(Guid id, Guid tenantId, [FromBody] SaleDTO saleDTO)
        {
            try
            {
                var result = await _salesOrderServiceGateway.CompleteSaleAsync(id, saleDTO, tenantId);
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
        public async Task<IActionResult> CancelSale(Guid id, Guid tenantId)
        {
            try
            {
                var result = await _salesOrderServiceGateway.CancelSaleAsync(id, tenantId);
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
