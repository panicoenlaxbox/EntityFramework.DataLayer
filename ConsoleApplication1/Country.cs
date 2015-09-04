using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Country : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}