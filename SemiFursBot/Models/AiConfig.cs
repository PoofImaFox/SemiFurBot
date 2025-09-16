using Microsoft.Extensions.Configuration;

namespace SemiFursBot.Models {
    internal class AiConfig {
        public AiConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(AiConfig)).Bind(this);
        }

        public string OllamaServer { get; set; }
        public string ModelId { get; set; }
    }
}
