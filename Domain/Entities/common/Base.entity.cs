using System.ComponentModel.DataAnnotations;

namespace Shopniu_api.Domain.Entities.common
{
    public class BaseEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}