using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{

    [Table("Zone", Schema = "AE")]
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
