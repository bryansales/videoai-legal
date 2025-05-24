using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using VideoAI.Data;
using VideoAI.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("VideoAIDb"));
builder.Services.AddControllers()
  .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title="VideoAI", Version="v1" }));
// DI registrations
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddSingleton<IOpenAiService, OpenAiService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IAudioService, AudioService>();
builder.Services.AddSingleton<IVideoService, VideoService>();
builder.Services.AddSingleton(sp => new StableDiffusionClient(builder.Configuration["StableDiffusion:ApiKey"]));
builder.Services.AddSingleton(sp => new ElevenLabsClient(builder.Configuration["ElevenLabs:ApiKey"], builder.Configuration["ElevenLabs:VoiceId"]));
builder.Services.AddSingleton(sp => new TikTokClient(builder.Configuration["TikTok:ClientKey"], builder.Configuration["TikTok:ClientSecret"]));
builder.Services.AddHostedService<VideoJobWorker>();
var app = builder.Build();
if(app.Environment.IsDevelopment()){ app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();
