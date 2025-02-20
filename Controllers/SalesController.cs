using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPITemplate.Interface;

namespace WebAPITemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpGet("total-sales")]
        public async Task<IActionResult> GetTotalSales()
        {
            var response = await _salesService.GetTotalSales();
            if (response.IsSuccess) return Ok(response.clsReturnResult);
            return BadRequest(response.clsReturnResult);
        }
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var result = await _salesService.GetMonthlySales();
            return result.IsSuccess ? Ok(result.clsReturnResult) : StatusCode(500, result.clsReturnResult);
        }

        [HttpGet("popular-items")]
        public async Task<IActionResult> GetMostPopularItems()
        {
            var result = await _salesService.GetMostPopularItemEachMonth();
            return result.IsSuccess ? Ok(result.clsReturnResult) : StatusCode(500, result.clsReturnResult);
        }

        [HttpGet("top-revenue-items")]
        public async Task<IActionResult> GetTopRevenueItems()
        {
            var result = await _salesService.GetTopRevenueItemsEachMonth();
            return result.IsSuccess ? Ok(result.clsReturnResult) : StatusCode(500, result.clsReturnResult);
        }

        [HttpGet("popular-item-stats")]
        public async Task<IActionResult> GetPopularItemStats()
        {
            var result = await _salesService.GetPopularItemStats();
            return result.IsSuccess ? Ok(result.clsReturnResult) : StatusCode(500, result.clsReturnResult);
        }
    }
}
