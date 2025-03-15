using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_Menu
    {
        [Key]
        public long Menu_Id { get; set; }
        public string? Menu_Code { get; set; }
        public bool? Is_Active { get; set; }
    }
}
