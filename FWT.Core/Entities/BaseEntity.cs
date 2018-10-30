using System.ComponentModel.DataAnnotations;

namespace FWT.Core.Entities
{
    public abstract class BaseEntity<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
