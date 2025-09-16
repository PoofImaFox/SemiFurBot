using Microsoft.Extensions.Hosting;

using SemiFursBot.Extensions;

namespace SemiFursBot {
    internal class Program {
        static async Task Main(string[] args) {
            var host = new HostBuilder()
                .UseConsoleLifetime()
                .UseStartup<Startup>()
                .Build();

            await host.RunAsync();
        }
    }
}
