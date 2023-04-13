namespace Jyben.Racing.Telemetry.Services
{
    public interface IPermissionsService
    {
        Task<bool> CheckLocationPermission();
    }
}
