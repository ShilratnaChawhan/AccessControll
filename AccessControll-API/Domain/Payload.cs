namespace AccessControll_API.Domain
{
    public class Payload
    {
        public long UserId { get; set; }
        public long? RoleId { get; set; }
        public long? PersonId { get; set; }
        public string? RefreshToken { get; set; }
    }
}
