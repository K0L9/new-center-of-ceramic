using System.Threading.Tasks;

namespace CenterOfCeramic.Interfaces
{
    public interface IDatabaseSeeder
    {
        Task InitializeAsync();
    }
}
