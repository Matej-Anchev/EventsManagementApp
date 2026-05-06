using Domain.Dto.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundEventEntryService : IInboundEventEntryService
{
    private readonly IRepository<InboundEventEntry> _repository;

    public InboundEventEntryService(IRepository<InboundEventEntry> repository)
    {
        _repository = repository;
    }

    public async Task<InboundEventEntry> CreateAsync(string rawPayload, Guid apiClientId)
    {
        var entry = new InboundEventEntry
        {
            RawPayload = rawPayload,
            ApiClientId = apiClientId,
            ReceivedAt = DateTime.UtcNow,
            Status = InboundEventStatus.Pending
        };

        return await _repository.InsertAsync(entry);
    }

    public async Task<InboundEventEntry> GetByIdNotNullAsync(Guid id)
    {
        var result = await _repository.Get(x => x, x => x.Id == id);
        if (result == null)
            throw new InvalidOperationException();

        return result;
    }
}