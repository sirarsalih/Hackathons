namespace Itera.Fagdag.November.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public double Price { get; set; }
        public int SizeMin { get; set; }
        public int SizeMax { get; set; }
        public string Variant { get; set; }
    }
}