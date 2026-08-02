var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient(); // Thêm dòng này 

var app = builder.Build();


app.MapRazorPages();
app.Run();