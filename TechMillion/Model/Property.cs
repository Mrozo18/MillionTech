namespace TechMillion.Model
{
    public class Property
    {
        public int IdProperty { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal Price { get; set; }
        public string? CodeInternal { get; set; }
        public int Year { get; set; }
        public int IdOwner { get; set; }
        public Owner? Owner { get; set; }
        public ICollection<PropertyTrace>? PropertyTrace { get; set; }
    }
}
