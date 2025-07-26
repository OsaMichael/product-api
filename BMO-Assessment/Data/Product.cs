namespace BMO_Assessment.Data
{
    public class Product
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
