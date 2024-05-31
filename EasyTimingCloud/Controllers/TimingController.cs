using EasyTimingCloud.Entities;
using EasyTimingWheel;
using FreeSql;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace EasyTimingCloud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimingController : ControllerBase
    {
        private readonly ILogger<TimingController> _logger;
        private readonly EasyTimingWheelManager _etwm;
        private readonly IBaseRepository<TaskEntity> _taskDB;

        public TimingController(ILogger<TimingController> logger, EasyTimingWheelManager etwm, IBaseRepository<TaskEntity> taskDB)
        {
            _logger = logger;
            _etwm = etwm;
            _taskDB = taskDB;
        }

        [HttpPost(Name = "add")]
        public TimingTaskInfo Add([FromBody] AddTimingTaskRequest request)
        {
            _taskDB.InsertAsync(request.Adapt<TaskEntity>());
            _etwm.AddTask(new TimingWheelCronTask(request.Name, request.Cron, _etwm.GetClockChain(), (task, param) =>
            {
                var dateTime = _etwm.GetClockDateTime();
                Console.WriteLine($"({dateTime:yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}): {task.Name}");
            }));
            return new TimingTaskInfo();
        }

        [HttpDelete(Name = "remove")]
        public async Task Remove([FromBody] RemoveTimingTaskRequest request)
        {
            await _taskDB.DeleteAsync(x => x.Id == request.Id);
        }

        [HttpPut(Name = "update")]
        public async Task Update([FromBody] UpdateTimingTaskRequest request)
        {
            var entity = _taskDB.Where(x => x.Id == request.Id).First();
            if (entity == null)
                return;
            entity = request.Adapt<TaskEntity>();
            await _taskDB.UpdateAsync(entity);
        }

        [HttpGet(Name = "list")]
        public IEnumerable<TimingTaskInfo> GetList()
        {
            return _taskDB.Select.ToList(x => new TimingTaskInfo()
            {
                Id = x.Id,
                CreateTime = x.CreateTime,
                Cron = x.Cron,
                Enable = x.Enable,
                Name = x.Name,
                Type = x.Type
            });
        }
    }

    public class RemoveTimingTaskRequest
    {
        public string Id { get; set; } = null!;
    }

    public class UpdateTimingTaskRequest : AddTimingTaskRequest
    {
        public string Id { get; set; } = null!;
    }

    public class AddTimingTaskRequest
    {
        public string Name { get; set; } = null!;
        public int Type { get; set; }
        public string Cron { get; set; } = null!;
    }

    public class TimingTaskInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Cron { get; set; }
        public bool Enable { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
