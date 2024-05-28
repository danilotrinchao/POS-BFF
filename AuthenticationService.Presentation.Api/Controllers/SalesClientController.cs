using AuthenticationService.Application.Contracts;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
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
        public async Task<IActionResult> CreateClient(ClientRequest client)
        {
            var clientId = await _saleServiceGateway.CreateClientAsync(client);
            return CreatedAtAction(nameof(GetClientById), new { id = clientId }, client);
        }

        [HttpGet("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _saleServiceGateway.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpGet("clients")]
        
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _saleServiceGateway.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpPut("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> UpdateClient(int id, ClientRequest client)
        {
            if (id != client.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var updated = await _saleServiceGateway.UpdateClientAsync(client);
            if (!updated)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("clients/{id}")]
        [Authorize(Policy = "SellerOrAdmin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var deleted = await _saleServiceGateway.DeleteClientAsync(id);
            if (!deleted)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }

}
