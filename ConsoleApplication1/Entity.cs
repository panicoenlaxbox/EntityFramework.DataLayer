using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1
{
    public abstract class Entity
    {
        public int Id { get; set; }
        [NotMapped]
        public State State { get; set; }
    }
}