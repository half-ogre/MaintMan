
namespace MaintMan
{
    public interface IMaintenanceTarballCreator
    {
        byte[] Create(string maintenanceHtml);
    }
}