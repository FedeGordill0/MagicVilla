using MagicVilla_API;
using MagicVilla_API.Datos;
using MagicVilla_API.Repositorio;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Agregamos los nuevos servicios
//relacionar dbContext con la cadena de conexnio y tambien el motor de bd
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Agregamos servicios de AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

//LA INTERFAZ Y SU IMPLEMENTACIÓN DEBEN SER AGREGADAS COMO SERVICIO PARA PODER SER INYECTADAS EN EL CONTROLADOR
builder.Services.AddScoped<IVillaRepositorio,VillaRepositorio>(); //=> Los servicios AddScoped se crean mediante una solicitud y luego una vez utilizado se destruye

builder.Services.AddScoped<INumeroVillaRepositorio, NumeroVillaRepositorio>();

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

