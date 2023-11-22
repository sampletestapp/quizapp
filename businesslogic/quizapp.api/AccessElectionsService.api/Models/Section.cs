using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    [Table("Section", Schema = "AE")]
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
