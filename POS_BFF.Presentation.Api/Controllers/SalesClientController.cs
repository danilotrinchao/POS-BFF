using POS_BFF.Application.Contracts;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Requets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS_BFF.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesClientController : ControllerBase
    {
        private readonly ISaleClientServiceGateway _saleServiceGateway;

        public SalesClientController(ISaleClientServiceGateway saleServiceGateway)
        {
            _saleServiceGateway = saleServiceGateway;
        }

        [HttpPost("clients")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> CreateClient(ClientRequest client, Guid TenantId)
        {
            var clientId = await _saleServiceGateway.CreateClientAsync(client, TenantId);
            return CreatedAtAction(nameof(GetClientById), new { id = clientId }, client);
        }

        [HttpGet("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> GetClientById(Guid id, Guid TenantId)
        {
            var client = await _saleServiceGateway.GetClientByIdAsync(id, TenantId);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpGet("clients")]
        
        public async Task<IActionResult> GetAllClients(Guid TenantId)
        {
            var clients = await _saleServiceGateway.GetAllClientsAsync(TenantId);
            return Ok(clients);
        }

        [HttpPut("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> UpdateClient(Guid id, Guid TenantId, ClientRequest client)
        {
            if (id != client.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var updated = await _saleServiceGateway.UpdateClientAsync(client, TenantId);
            if (!updated)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> DeleteClient(Guid id, Guid TenantId)
        {
            var deleted = await _saleServiceGateway.DeleteClientAsync(id, TenantId);
            if (!deleted)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }

}