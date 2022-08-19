using Recorder;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

// �����ļ�
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var builder = WebApplication.CreateBuilder(args);

// ��������
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

// �����ļ�
builder.Services.AddSingleton(new AppSettingsHelper(configuration));

// ע��Jobs����
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton<QuartzFactory>();
builder.Services.AddSingleton<SynchronizeJob>();
builder.Services.AddSingleton<IJobFactory, IOCJobFactory>();

// ע��HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // �����м����������Swagger��ΪJson�ս��
    app.UseSwagger();
    // �����м�������Swagger-ui,ָ��Swagger Json�ս��
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recorder API V1");
    });
}

// Quartz �Ƿ�����
if (true)
{
    // ��ȡ�����е�QuartzFactory
    var quartz = app.Services.GetRequiredService<QuartzFactory>();
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        await quartz.Start();
    });
}

// https
//app.UseHttpsRedirection();
app.UseAuthorization();
// ����Cors
app.UseCors("any");
app.MapControllers();
app.Run();
