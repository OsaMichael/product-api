using System.ComponentModel.DataAnnotations;

namespace BMO_Assessment.Models
{
    public class ProductRequest
    {
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }
        public string Description { get; set; }
    }
}
