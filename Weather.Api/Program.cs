using Serilog;

using System.Diagnostics;
using System.Runtime.CompilerServices;

public static partial class Program
{
    public static void LineNumber(
        [CallerFilePath] string path = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string membername = "")
    {
        Log.Debug($"Debug hint at line path:{path}, {lineNumber}, {membername}");
    }
    public static void Main(string[] args)
    {
        Directory.CreateDirectory("logs");

        // فقط برای شروع اپ یک logger مینیمال لازم داریم
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // اینجا دیگر CreateLogger() نزن!
            builder.Host.UseSerilog((ctx, lc) => lc
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File(
                    path: "logs/log-.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    formatter: new Serilog.Formatting.Json.JsonFormatter()
                ))
            );

            var app = builder.Build();

            app.MapGet("/", () =>
            {
                Log.Information($"Weather endpoint called at {DateTime.UtcNow}");
                LineNumber();
                try
                {
                    throw new Exception("Fake exception!");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error message{ex.Message}");
                }
                return new { Temp = 20, Condition = "Cloudy" };
            });

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
