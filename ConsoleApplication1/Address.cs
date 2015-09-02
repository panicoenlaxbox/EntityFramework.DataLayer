namespace ConsoleApplication1
{
    public class Address : EntityWithState
    {
        public int AddressId { get; set; }
        public string Region { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}