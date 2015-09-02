using System.Data.Entity;

namespace ConsoleApplication1
{
    public static class DbContextExtensions
    {
        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<EntityWithState>())
            {
                entry.State = entry.Entity.State.ConvertToEntityState();
            }
        }
    }
}