using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorTestProject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;

//var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebAssemblyHostBuilder.CreateDefault(args);
//var builder = WebApplication.CreateBuilder(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:10069/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// builder.Services.AddHttpClient();
// builder.Services.AddCors();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: MyAllowSpecificOrigins,
//                       builder =>
//                       {
//                           builder.WithOrigins("http://localhost:5041",
//                                               "https://localhost:10069",
//                                               "https://localhost:7147");
//                       });
// });

// builder.Services.AddCors(options =>
//      {
//          options.AddPolicy("NewPolicy", builder =>
//           builder.AllowAnyOrigin()
//                        .AllowAnyMethod()
//                        .AllowAnyHeader());
//      });


await builder.Build().RunAsync();;
//var app = builder.Build();
//app.UseCors(MyAllowSpecificOrigins);

// var app = builder.Build();
// app.UseCors("CorsAllowAll");
// await app.RunAsync();
