using EasyTimingWheel;

var etwM = new EasyTimingWheelManager();

Console.WriteLine($"({etwM.GetClockDateTime():yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff})");

etwM.AddTask(new TimingWheelCronTask("test", "20/5 * * * * ?", etwM.GetClockChain(), (task, param) =>
{
    var dateTime = etwM.GetClockDateTime();
    Console.WriteLine($"({dateTime:yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}): {task.Name}");
}));

etwM.AddTask(new TimingWheelTask($"test_2", 3, (task, param) =>
{
    var dateTime = etwM.GetClockDateTime();
    Console.WriteLine($"({dateTime:yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}): {task.Name}");
}));

etwM.TimeSync();
await etwM.StartLoopAsync();