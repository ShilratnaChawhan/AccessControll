using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_Role
    {
        [Key]
        public long? Role_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? Description { get; set; }
        public bool? Is_Active { get; set; }
    }
}
