using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1
{
    public abstract class EntityWithState
    {
        [NotMapped]
        public State State { get; set; }
    }
}