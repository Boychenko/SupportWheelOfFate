using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportWheelOfFate.Common.Extentions;
using SupportWheelOfFate.Queries.Schedule;

namespace SupportWheelOfFate.Api.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [Route("{week?}")]
        //TODO: Consider adding model binder for datetime for this specific format
        public async Task<IActionResult> Get(string week = null)
        {
            var response = await _mediator.Send(new ScheduleQuery() { Date = week.DateFromQueryParam() });
            if (response.Entries.Count == 0)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
