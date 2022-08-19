using Recorder;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

// 配置文件
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var builder = WebApplication.CreateBuilder(args);

// 跨域配置
builder.Services.AddCors(options => options.AddPolicy("any", prop => prop.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Recorder API", Version = "v1" });
});

builder.Services.AddControllersWithViews();

// 配置文件
builder.Services.AddSingleton(new AppSettingsHelper(configuration));

// 注册Jobs任务
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton<QuartzFactory>();
builder.Services.AddSingleton<SynchronizeJob>();
builder.Services.AddSingleton<IJobFactory, IOCJobFactory>();

// 注册HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 启用中间件服务生成Swagger作为Json终结点
    app.UseSwagger();
    // 启用中间件服务对Swagger-ui,指定Swagger Json终结点
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recorder API V1");
    });
}

// Quartz 是否启动
if (true)
{
    // 获取容器中的QuartzFactory
    var quartz = app.Services.GetRequiredService<QuartzFactory>();
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        await quartz.Start();
    });
}

// https
//app.UseHttpsRedirection();
app.UseAuthorization();
// 配置Cors
app.UseCors("any");
app.MapControllers();
app.Run();
