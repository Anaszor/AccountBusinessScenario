namespace Application.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Gender { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Photo { get; set; }
        public string Address { get; set; } = default!;
        public decimal Balance { get; set; }
    }
}