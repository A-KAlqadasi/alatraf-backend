

namespace AlatrafClinic.Application.Features.People.Persons.Dtos
{
    public class PersonDto
    {
        public int PersonId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string? Phone { get; set; }
        public string? NationalNo { get; set; }
        public string? Address { get; set; }
        public bool Gender { get; set; }
    }
}