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

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var productId = await _saleProductServiceGateway.AddProductAsync(productDto);
                if (productId != Guid.Empty)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
            }
            return Ok(productDto);
        }

        [HttpPut]
        public async Task<IActionResult> EditProduct(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var success = await _saleProductServiceGateway.UpdateProductAsync(productDto.Id, productDto);
                if (success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            return Ok(productDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var success = await _saleProductServiceGateway.DeleteProductAsync(id);
            if (success)
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(Guid id )
        {
            var product = await _saleProductServiceGateway.GetProductAsync(id);
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
    }
}
