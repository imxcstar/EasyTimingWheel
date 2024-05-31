using EasyTimingCloud;
using EasyTimingCloud.Entities;
using EasyTimingWheel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var etwm = new EasyTimingWheelManager();

builder.Services.AddSingleton(etwm);

builder.Services.AddHostedService<TimingBackgroundService>();

builder.Services.AddSingleton(r =>
{
    var builder = new FreeSql.FreeSqlBuilder();
    var dbType = Environment.GetEnvironmentVariable("DBTYPE")?.ToLower() ?? "sqlite";
    builder = dbType switch
    {
        "sqlite" => builder.UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=data.db"),
        "mysql" => builder.UseConnectionString(FreeSql.DataType.MySql,
                        $"Server={Environment.GetEnvironmentVariable("MYSQL_SERVER")};Database={Environment.GetEnvironmentVariable("MYSQL_DB")};Uid={Environment.GetEnvironmentVariable("MYSQL_USERNAME")};Pwd={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};{Environment.GetEnvironmentVariable("DB_CONNECTIONSTRING") ?? ""};"),
        _ => throw new NotSupportedException($"DBTYPE {dbType} not supported."),
    };
    var fsql = builder
        .UseAutoSyncStructure(true)
        .Build();
    return fsql;
});

builder.Services.AddFreeRepository();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (IServiceScope serviceScope = app.Services.CreateScope())
{
    var taskDB = serviceScope.ServiceProvider.GetRequiredService<IFreeSql>().GetRepository<TaskEntity>();
    var allTasks = taskDB.Where(x => x.Enable).ToList();
    foreach (var task in allTasks)
    {
        etwm.AddTask(new TimingWheelCronTask(task.Name, task.Cron, etwm.GetClockChain(), (task, param) =>
        {
            var dateTime = etwm.GetClockDateTime();
            Console.WriteLine($"({dateTime:yyyy-MM-dd HH:mm:ss.fff}, Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}): {task.Name}");
        }));
    }
}

app.Run();
