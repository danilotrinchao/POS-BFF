using Microsoft.AspNetCore.Mvc;
using POS_BFF.Core.Domain.Gateways.Company;
using POS_BFF.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS_BFF.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyEmployeerController : ControllerBase
    {
        private readonly ICompanyEmployeerGateway _companyEmployeerGateway;

        public CompanyEmployeerController(ICompanyEmployeerGateway companyEmployeerGateway)
        {
            _companyEmployeerGateway = companyEmployeerGateway;
        }

        // POST: api/CompanyEmployeer
        [HttpPost]
        public async Task<IActionResult> CreateEmployeer([FromBody] EmployeerDTO employeer, [FromQuery] Guid tenantId)
        {
            if (employeer == null || tenantId == Guid.Empty)
                return BadRequest("Dados inválidos.");

            var employeerId = await _companyEmployeerGateway.CreateEmployeer(employeer, tenantId);
            return CreatedAtAction(nameof(GetEmployeerById), new { id = employeerId, tenantId }, employeer);
        }

        // GET: api/CompanyEmployeer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeerById(Guid id, [FromQuery] Guid tenantId)
        {
            if (id == Guid.Empty || tenantId == Guid.Empty)
                return BadRequest("Dados inválidos.");

            var employeer = await _companyEmployeerGateway.GetEmployeerById(id, tenantId);
            if (employeer == null)
                return NotFound();

            return Ok(employeer);
        }

        // GET: api/CompanyEmployeer/email/{email}
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetEmployeerByEmail(string email, [FromQuery] Guid tenantId)
        {
            if (string.IsNullOrEmpty(email) || tenantId == Guid.Empty)
                return BadRequest("Dados inválidos.");

            var employeer = await _companyEmployeerGateway.GetEmployeerByEmailAsync(email, tenantId);
            if (employeer == null)
                return NotFound();

            return Ok(employeer);
        }

        // GET: api/CompanyEmployeer
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeers([FromQuery] Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("TenantId inválido.");

            var employeers = await _companyEmployeerGateway.GetAllEmployeers(tenantId);
            return Ok(employeers);
        }

        // PUT: api/CompanyEmployeer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeer(Guid id, [FromBody] EmployeerDTO employeer, [FromQuery] Guid tenantId)
        {
            if (employeer == null || id != employeer.Id || tenantId == Guid.Empty)
                return BadRequest("Dados inválidos.");

            var success = await _companyEmployeerGateway.UpdateEmployeerAsync(employeer, tenantId);
            if (!success)
                return StatusCode(500, "Erro ao atualizar o empregado.");

            return NoContent();
        }

        // DELETE: api/CompanyEmployeer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeer(Guid id, [FromQuery] Guid tenantId)
        {
            if (id == Guid.Empty || tenantId == Guid.Empty)
                return BadRequest("Dados inválidos.");

            var success = await _companyEmployeerGateway.DeleteEmployeer(id, tenantId);
            if (!success)
                return StatusCode(500, "Erro ao excluir o empregado.");

            return NoContent();
        }
    }
}
