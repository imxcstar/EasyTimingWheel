using EasyTimingWheel;
using Microsoft.AspNetCore.Mvc;

namespace EasyTimingCloud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimingController : ControllerBase
    {
        private readonly ILogger<TimingController> _logger;
        private readonly EasyTimingWheelManager _etwm;

        public TimingController(ILogger<TimingController> logger, EasyTimingWheelManager etwm)
        {
            _logger = logger;
            _etwm = etwm;
        }

        [HttpPost(Name = "add")]
        public TimingTaskInfo Add([FromBody] AddTimingTaskRequest request)
        {
            _etwm.AddTask(new TimingWheelCronTask(request.Name, request.Cron, _etwm.GetClockChain(), (task, param) =>
            {
                var dateTime = _etwm.GetClockDateTime();
                Console.WriteLine($"({dateTime:yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}): {task.Name}");
            }));
            return new TimingTaskInfo();
        }

        [HttpDelete(Name = "remove")]
        public Task Remove()
        {
            throw new NotImplementedException();
        }

        [HttpPut(Name = "update")]
        public TimingTaskInfo Update()
        {
            throw new NotImplementedException();
        }

        [HttpGet(Name = "list")]
        public IEnumerable<TimingTaskInfo> GetList()
        {
            throw new NotImplementedException();
        }
    }

    public class AddTimingTaskRequest
    {
        public string Name { get; set; } = null!;
        public int Type { get; set; }
        public string Cron { get; set; } = null!;
    }

    public class TimingTaskInfo
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Cron { get; set; }
        public bool Enable { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
