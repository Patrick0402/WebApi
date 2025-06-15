using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Model
{
    [Table("company")]
    public class CompanyModel
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("name")]
        public string Name { get; private set; } = string.Empty;

    }
}
