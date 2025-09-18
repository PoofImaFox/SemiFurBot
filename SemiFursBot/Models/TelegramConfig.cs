using Microsoft.Extensions.Configuration;

namespace SemiFursBot.Models {
    internal class TelegramConfig {
        public TelegramConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(TelegramConfig)).Bind(this);
        }

        public string Token { get; set; }
    }
}
