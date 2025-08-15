using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WarehouseManager.Client.Services;

namespace WarehouseManager.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

            if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var baseUri))
            {
                throw new InvalidOperationException("Неверный базовый адрес API в конфигурации.");
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseUri });

            builder.Services.AddScoped <NotificationService>();
            builder.Services.AddScoped<ModalFormService>();

            await builder.Build().RunAsync();
        }
    }
}
