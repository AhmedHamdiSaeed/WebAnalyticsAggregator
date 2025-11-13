using Application.Implementations;
using Application.Interfaces;
using DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAnalyticsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportsService;

        public ReportsController(IReportService reportsService)
        {
            _reportsService = reportsService;
        }
        [HttpGet("overview")]
        public async Task<ActionResult<OverviewReportDto>> GetOverview()
        {
            var result = await _reportsService.GetOverviewReportAsync();
            if(result.IsSuccess)
            return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("pages")]
        public async Task<ActionResult<IEnumerable<PageReportDto>>> GetPerPageReport()
        {
            var report = await _reportsService.GetPerPageReportAsync();
            return Ok(report);
        }
    }
}
