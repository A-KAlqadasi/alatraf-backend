using AlatrafClinic.Application.Common.Interfaces.Messaging;
using AlatrafClinic.Infrastructure.Data;
using AlatrafClinic.Infrastructure.Data.Outbox;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Inbox;

public sealed class Inbox : IInbox
{
    private readonly AlatrafClinicDbContext _db;

    public Inbox(AlatrafClinicDbContext db)
    {
        _db = db;
    }

    public async Task<bool> TryProcessAsync(
        Guid messageId,
        string handlerName,
        CancellationToken ct)
    {
        try
        {
            _db.ProcessedMessages.Add(new ProcessedMessage
            {
                MessageId = messageId,
                HandlerName = handlerName
            });

            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            // Unique constraint violated
            return false;
        }
    }
}
