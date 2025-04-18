using System;
using System.Text.Json.Serialization;

namespace GlucoPilot.LibreLinkClient.Models;

public sealed record ConnectionData
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("patientId")]
    public Guid PatientId { get; init; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; init; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; init; }

    [JsonPropertyName("sensor")]
    public SensorData? Sensor { get; init; }

    [JsonPropertyName("glucoseMeasurement")]
    public GraphData? CurrentMeasurement { get; init; }
}