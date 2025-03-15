using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_User
    {
        [Key]
        public long User_Id { get; set; }
        public long? Person_Id { get; set; }
        public long? Role_Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password_Hash { get; set; }
        public string? Password_Salt { get; set; }
        public string? Phone_Number { get; set; }
        public bool? Is_Active { get; set; }
        public string? Reset_Token { get; set; }
        public string? Auth_Token { get; set; }
        public string? Refresh_Token { get; set; }
        public bool? Is_Logged_In { get; set; }
        public bool? Is_Locked { get; set; }
        public DateTime? Locked_Until { get; set; }

    }
}
