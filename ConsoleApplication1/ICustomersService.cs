using System.Linq;

namespace ConsoleApplication1
{
    public interface ICustomersService
    {
        IQueryable<CustomerDTO> GetAll_DTO(string name);
    }
}