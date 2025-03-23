using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_OTP_Log
    {
        [Key]
        public long OTP_Log_Id { get; set; }
        public long? User_Id { get; set; }
        public string? OTP_Type { get; set; }
        public string? OTP { get; set; }
        public bool? Is_Verified { get; set; }
        public bool? Is_Active { get; set; }
        public DateTime? Expires_At { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
