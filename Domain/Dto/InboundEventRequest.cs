namespace Domain.Dto;

public record InboundEventRequest(
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    string VenueName,
    string? VenueCity,
    List<InboundSectionPricing> Pricing
);

public abstract record InboundSectionPricing(
    string SectionName,
    decimal Price,
    string Currency = "USD");