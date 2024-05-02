using Microsoft.AspNetCore.Mvc;
using secullumTest.Models;

namespace secullumTest.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase {
        private readonly IRecordRepository _recordRepository;

        public RecordController(IRecordRepository recordRepository) {
            _recordRepository = recordRepository;
        }

        [HttpGet]
        public IEnumerable<Record> Get() {
            return _recordRepository.GetRecords();
        }

        [HttpPost("{Employee}")]
        public ActionResult<Record> GetBy(Record record) {
            record = _recordRepository.GetRecordByEmployee(record);

            if (record == null) {
                return NotFound();
            }
            return record;
        }

        [HttpGet("totalHours")]
        public IEnumerable<object> GetTotalHoursPerEmployeePerDay() {
            return _recordRepository.GetTotalHoursPerEmployeePerDay();
        }
    }
}