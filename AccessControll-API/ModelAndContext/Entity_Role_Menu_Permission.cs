using System.ComponentModel.DataAnnotations;

namespace AccessControll_API.ModelAndContext
{
    public class Entity_Role_Menu_Permission
    {
        [Key]
        public long Role_Menu_Permission_Id { get; set; }
        public long? Role_Id { get; set; }
        public long? Menu_Id { get; set; }
        public bool? Can_View { get; set; }
        public bool? Can_Create { get; set; }
        public bool? Can_Edit { get; set; }
        public bool? Can_Delete { get; set; }
        public bool? Is_Active { get; set; }

    }
}
