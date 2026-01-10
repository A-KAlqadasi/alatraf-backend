// Saga logic – to be migrated: handler previously reserved inventory and published next step
// Trigger: SaleCreatedDomainEvent (domain)
// Side effects: inventory lookup, decrement, reservation creation, compensation, publish next event
// Persistence boundary: SaveChanges inside handler
// NOTE: Handler code currently commented out; kept for reference during migration
