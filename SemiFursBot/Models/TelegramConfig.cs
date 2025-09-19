using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace SemiFursBot.Models {
    internal class TelegramConfig {
        public TelegramConfig(IConfiguration configuration) {
            configuration.GetSection(nameof(TelegramConfig)).Bind(this);
            LoadCache();
        }

        public string Token { get; set; }
        public Dictionary<string, (int, long)> TopicNames { get; set; }

        public void SaveCache() {
            var jsonContent = JsonConvert.SerializeObject(TopicNames);
            File.WriteAllText("cacheTopics.json", jsonContent);
        }

        public void LoadCache() {
            if (!File.Exists("cacheTopics.json")) {
                TopicNames = new Dictionary<string, (int, long)>();
                return;
            }

            var jsonContent = File.ReadAllText("cacheTopics.json");
            TopicNames = JsonConvert.DeserializeObject<Dictionary<string, (int, long)>>(jsonContent);
        }
    }
}
