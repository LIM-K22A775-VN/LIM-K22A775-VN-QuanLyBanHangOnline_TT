using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuanLyBanHangOnline.Data;
var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ CORS vào container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL của ứng dụng Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});




builder.Services.AddDbContext<reviewContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("reviewContext") ?? throw new InvalidOperationException("Connection string 'reviewContext' not found.")));
builder.Services.AddDbContext<orderdetailContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("orderdetailContext") ?? throw new InvalidOperationException("Connection string 'orderdetailContext' not found.")));
builder.Services.AddDbContext<orderContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("orderContext") ?? throw new InvalidOperationException("Connection string 'orderContext' not found.")));
builder.Services.AddDbContext<QuanLyBanHangOnlineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyBanHangOnlineContext") ?? throw new InvalidOperationException("Connection string 'QuanLyBanHangOnlineContext' not found.")));
builder.Services.AddDbContext<productContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("productContext") ?? throw new InvalidOperationException("Connection string 'productContext' not found.")));
builder.Services.AddDbContext<UseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UseContext") ?? throw new InvalidOperationException("Connection string 'UseContext' not found.")));
builder.Services.AddDbContext<staffContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("staffContext") ?? throw new InvalidOperationException("Connection string 'staffContext' not found.")));
builder.Services.AddDbContext<adminContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("adminContext") ?? throw new InvalidOperationException("Connection string 'adminContext' not found.")));

// Add services to the container.

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


// QUAN TRỌNG: Kích hoạt CORS tại đây
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
