using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson1.Controllers {
    [ApiController]
    [Route("/")]
    public class TemperatureTrackerController : ControllerBase {

        private readonly ILogger<TemperatureTrackerController> _logger;

        public TemperatureTrackerController(ILogger<TemperatureTrackerController> logger) {
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult Create([FromQuery] string date, [FromQuery] string temperature) {
            try {
                DateTime Date = DateTime.Parse(date);
                int Temperature = Int32.Parse(temperature);
                if (TemperatureTracker.Tracker.Add(Date, Temperature)) return Ok($"Successfully added, {TemperatureTracker.Tracker.Count} records in data base.");
                else return Conflict("Such record already exists.");
            } catch { return BadRequest(); }
        }

        [HttpGet("read")]
        public IActionResult Read([FromQuery] string from, [FromQuery] string to) {
            try {
                DateTime Date1 = DateTime.Parse(from);
                DateTime Date2 = DateTime.Parse(to);
                return Ok(TemperatureTracker.Tracker.List(Date1, Date2));
            } catch { return BadRequest(); }
        }

        [HttpGet("readALL")]
        public IActionResult Read() => Ok(TemperatureTracker.Tracker.ListALL());

        [HttpPut("update")]
        public IActionResult Update([FromQuery] string date, [FromQuery] string temperature) {
            try {
                DateTime Date = DateTime.Parse(date);
                int Temperature = Int32.Parse(temperature);
                if (TemperatureTracker.Tracker.UpDate(Date, Temperature)) return Ok($"Successfully updated.");
                else return NotFound("No such record.");
            } catch { return BadRequest(); }
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string date) {
            try {
                DateTime Date = DateTime.Parse(date);
                if (TemperatureTracker.Tracker.Delete(Date)) return Ok($"Successfully deleted, {TemperatureTracker.Tracker.Count} records in data base.");
                else return NotFound("No such record.");
            } catch { return BadRequest(); }
        }

    }
}
