using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

//Cấu hình JWT 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using QuanLyBanHangOnline.Infrastructure.Jwt;
using QuanLyBanHangOnline.Services.Interfaces;
using QuanLyBanHangOnline.Services.Implementations;
using System.Reflection;
using FluentValidation.AspNetCore;
using FluentValidation;
using QuanLyBanHangOnline.Validations.Products;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);
// --- DỊCH VỤ HỆ THỐNG ---

// Đăng ký AdminService
builder.Services.AddScoped<IAdminService, AdminService>();
// Đăng ký UserService
builder.Services.AddScoped<IUserService, UserService>();
// Đăng ký OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpContextAccessor();
// Đăng ký ProductService
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();

//Cấu hình Authentication  , JWT 
builder.Services.AddSystemAuthenticationJwt(builder.Configuration);

//.2 Cấu hình Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyBanHangOnlineContext") ?? throw new InvalidOperationException("Connection string 'reviewContext' not found.")));

// 3. Cấu hình CORS
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

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Dòng này giúp chuyển đổi tất cả Enum sang String khi trả về JSON
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Đăng ký FluentValidation
builder.Services.AddFluentValidationAutoValidation(); // Tự động kiểm tra khi có request
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>(); // Tự động tìm tất cả các Validator trong cùng Assembly

// 4. Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuanLyBanHangOnline API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });


    // Thêm ghi chú XML để Swagger đọc được các mô tả (Summary)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// --- PIPELINE XỬ LÝ ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Đảm bảo có dòng này để truy cập được ảnh trong wwwroot
app.UseStaticFiles();


// QUAN TRỌNG: Kích hoạt CORS tại đây
app.UseCors("AllowAngular");

//Bật middleware
app.UseAuthentication(); // Xác thực: Bạn là ai?
app.UseAuthorization();  // Phân quyền: Bạn được làm gì?




app.MapControllers();

app.Run();