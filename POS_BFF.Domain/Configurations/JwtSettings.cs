using Microsoft.Extensions.DependencyInjection;

namespace POS_BFF.Core.Domain.Configurations
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public List<string> Audience { get; set; }
        public int DurationInMinutes { get; set; }

        [ActivatorUtilitiesConstructor]  // Indica o construtor a ser usado
        public JwtSettings() { }

        // Construtor adicional (privado ou sem o atributo)
        private JwtSettings(string key)
        {
            Key = key;
        }
    }

}
