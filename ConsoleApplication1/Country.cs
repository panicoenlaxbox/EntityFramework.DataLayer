using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Country : EntityWithState
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}