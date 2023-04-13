using System;
using System.Text.Json.Serialization;

namespace Jyben.Racing.Telemetry.Infrastructure.Models
{
    public class TelemetryPilote
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("nom")]
        public string Nom { get; set; }

        [JsonPropertyName("trace")]
        public Trace Trace { get; set; }
    }

    public class Trace
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("vitesse")]
        public double Vitesse { get; set; }
    }
}

