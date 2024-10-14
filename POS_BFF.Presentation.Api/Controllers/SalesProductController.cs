using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace POS_BFF.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesProductController : ControllerBase
    {
        private readonly ISaleProductServiceGateway _saleProductServiceGateway;

        public SalesProductController(ISaleProductServiceGateway saleProductServiceGateway)
        {
            _saleProductServiceGateway = saleProductServiceGateway;
        }

        [HttpPost("product")]
        public async Task<IActionResult> AddProduct(PhysiqueProductDTO productDto, Guid TenantId)
        {
            if (ModelState.IsValid)
            {
                var result = await _saleProductServiceGateway.AddProductAsync(productDto, TenantId);
                if (result == null)
                {
                    return Ok(result);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPost("service")]
        public async Task<IActionResult> AddService(VirtualProductDTO productDto, Guid TenantId)
        {
            if (ModelState.IsValid)
            {
                var result = await _saleProductServiceGateway.AddServicetAsync(productDto, TenantId);
                if (result == null)
                {
                    return Ok(result);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPut("editProduct")]
        public async Task<IActionResult> EditProduct(PhysiqueProductDTO productDto, Guid TenantId)
        {
            if (ModelState.IsValid)
            {
                var success = await _saleProductServiceGateway.UpdateProductAsync(productDto.Id, productDto, TenantId);
                if (success)
                {
                    return Ok(success);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            return Ok(productDto);
        }
        [HttpPut("editService")]
        public async Task<IActionResult> EditService(VirtualProductDTO productDto, Guid TenantId)
        {
            if (ModelState.IsValid)
            {
                var success = await _saleProductServiceGateway.UpdateServiceAsync(productDto.Id, productDto, TenantId);
                if (success)
                {
                    return Ok(success);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            return Ok(productDto);
        }

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id, Guid TenantId)
        {
            var success = await _saleProductServiceGateway.DeleteProductAsync(id, TenantId);
            if (success)
            {
                return Ok(success);
            }
            return NotFound();
        }
        [HttpDelete("deleteService")]
        public async Task<IActionResult> DeleteService(Guid id, Guid TenantId)
        {
            var success = await _saleProductServiceGateway.DeleteServiceAsync(id, TenantId);
            if (success)
            {
                return Ok(success);
            }
            return NotFound();
        }
        [HttpGet("GetProductByCodeBar")]
        public async Task<IActionResult> GetProductByCodeBar(string barcode, Guid TenantId)
        {
            var product = await _saleProductServiceGateway.GetProductByBarCode(barcode, TenantId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProduct(Guid id , Guid TenantId)
        {
            var product = await _saleProductServiceGateway.GetProductById(id, TenantId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("GetServiceById")]
        public async Task<IActionResult> GetService(Guid id, Guid TenantId)
        {
            var product = await _saleProductServiceGateway.GetServiceById(id, TenantId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProducts(Guid TenantId)
        {
            var products = await _saleProductServiceGateway.GetAllProductsAsync(TenantId);
            return Ok(products);
        }
        [HttpGet("AllServices")]
        public async Task<IActionResult> GetAllServices(Guid TenantId)
        {
            var products = await _saleProductServiceGateway.GetAllServicesAsync(TenantId);
            return Ok(products);
        }
    }
}
