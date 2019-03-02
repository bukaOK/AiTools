using System.Threading.Tasks;

namespace AiTools.DAL.Initializers
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}
