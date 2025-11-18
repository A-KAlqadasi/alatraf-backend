using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;
using AlatrafClinic.Domain.Patients.Cards.WoundedCards;
using AlatrafClinic.Domain.Patients.Enums;

public interface IPatientRepository : IGenericRepository<Patient, int>
{
    Task<Patient?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> GetByPatientTypeAsync(PatientType type, CancellationToken cancellationToken = default);
    Task<Patient?> GetByIdWithPersonAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IQueryable<Patient>> GetPatientsWithPersonQueryAsync(CancellationToken cancellationToken = default);

    // disabled cards
    Task AddDisabledCardAsync(DisabledCard disabledCard, CancellationToken ct = default);
    Task UpdateDisabledCardAsync(DisabledCard disabledCard, CancellationToken ct = default);
    Task DeleteDisabledCardAsync(DisabledCard disabledCard, CancellationToken ct = default);
    Task<DisabledCard?> GetDisabledCardByNumber(string cardNumber, CancellationToken ct = default);
    Task<bool> IsDisabledCardExists(string cardNumber, CancellationToken ct = default);
    Task<bool> IsDisabledCardExists(int disabledCardId, CancellationToken ct = default);
    Task<DisabledCard?> GetDisabledCardByIdAsync(int disabledCardId, CancellationToken ct = default);
    Task<IReadOnlyList<DisabledCard>> GetDisabledCardsAsync(CancellationToken ct = default);

    // wounded cards
    Task<bool> IsWoundedCardExists(string cardNumber, CancellationToken ct = default);
    Task<bool> IsWoundedCardExists(int woundedCardId, CancellationToken ct = default);
    Task<WoundedCard?> GetWoundedCardByNumber(string cardNumber, CancellationToken ct = default);
    Task AddWoundedCardAsync(WoundedCard woundedCard, CancellationToken ct = default);
    Task UpdateWoundedCardAsync(WoundedCard woundedCard, CancellationToken ct = default);
    Task DeleteWoundedCardAsync(WoundedCard woundedCard, CancellationToken ct = default);
    Task<IReadOnlyList<WoundedCard>> GetWoundedCardsAsync(CancellationToken ct = default);
    Task<WoundedCard?> GetWoundedCardByIdAsync(int woundedCardId, CancellationToken ct = default);
}