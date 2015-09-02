using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Customer : EntityWithState
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}