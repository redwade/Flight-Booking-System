namespace AI.Core.Entities;

public class FlightRecommendation
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FlightId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
