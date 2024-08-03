using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Gateways.Sales;
using AuthenticationService.Core.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
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
        public async Task<IActionResult> AddProduct(PhysiqueProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _saleProductServiceGateway.AddProductAsync(productDto);
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
        public async Task<IActionResult> AddService(VirtualProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _saleProductServiceGateway.AddServicetAsync(productDto);
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
        public async Task<IActionResult> EditProduct(PhysiqueProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var success = await _saleProductServiceGateway.UpdateProductAsync(productDto.Id, productDto);
                if (success)
                {
                    return Ok(success);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            return Ok(productDto);
        }
        [HttpPut("editService")]
        public async Task<IActionResult> EditService(VirtualProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var success = await _saleProductServiceGateway.UpdateServiceAsync(productDto.Id, productDto);
                if (success)
                {
                    return Ok(success);
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            return Ok(productDto);
        }

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var success = await _saleProductServiceGateway.DeleteProductAsync(id);
            if (success)
            {
                return Ok(success);
            }
            return NotFound();
        }
        [HttpDelete("deleteService")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var success = await _saleProductServiceGateway.DeleteServiceAsync(id);
            if (success)
            {
                return Ok(success);
            }
            return NotFound();
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProduct(Guid id )
        {
            var product = await _saleProductServiceGateway.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("GetServiceById")]
        public async Task<IActionResult> GetService(Guid id)
        {
            var product = await _saleProductServiceGateway.GetServiceById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _saleProductServiceGateway.GetAllProductsAsync();
            return Ok(products);
        }
        [HttpGet("AllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var products = await _saleProductServiceGateway.GetAllServicesAsync();
            return Ok(products);
        }
    }
}
