using System.Data.Entity;

namespace ConsoleApplication1
{
    public static class StateExtensions
    {
        public static EntityState ConvertToEntityState(this State state)
        {
            switch (state)
            {
                case State.Added:
                    return EntityState.Added;
                case State.Deleted:
                    return EntityState.Deleted;
                case State.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}