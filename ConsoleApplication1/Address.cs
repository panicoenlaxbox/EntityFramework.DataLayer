using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1
{
    public class Address : Entity
    {
        public string Region { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}