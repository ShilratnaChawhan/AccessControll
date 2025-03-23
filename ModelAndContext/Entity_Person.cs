using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_Person
    {
        [Key]
        public long Person_Id { get; set; }
        public string? Person_Initials { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Middle_Name { get; set; }
        public string? Email_Address { get; set; }
        public string? Phone_Number { get; set; }
        public bool? Is_Active { get; set; }
        public byte[]? Image { get; set; }

    }
}
