using EasyTimingCloud.Controllers;
using EasyTimingWheel;
using System.ComponentModel;

namespace EasyTimingCloud
{
    public class TimingBackgroundService : BackgroundService
    {
        private readonly ILogger<TimingBackgroundService> _logger;
        private readonly EasyTimingWheelManager _etwm;

        public TimingBackgroundService(ILogger<TimingBackgroundService> logger, EasyTimingWheelManager etwm)
        {
            _logger = logger;
            _etwm = etwm;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _etwm.TimeSync();
            _logger.LogInformation("(TimingBackgroundService: {cdate}, Now: {ndate})"
                , _etwm.GetClockDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff")
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            await _etwm.StartLoopAsync(stoppingToken);
        }
    }
}
