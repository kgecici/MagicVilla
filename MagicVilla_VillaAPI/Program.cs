using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("log/logs.txt", rollingInterval:RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddControllers(option =>
        {
            // application/json haricinde birsey gelirse not accaptable donsun
            //option.ReturnHttpNotAcceptable = true;
        }
    ).AddNewtonsoftJson()
    //.AddXmlDataContractSerializerFormatters() // accept header application/xml gelirse response un otomatik olarak xml donmesini saglar
    ;
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

app.Run();

