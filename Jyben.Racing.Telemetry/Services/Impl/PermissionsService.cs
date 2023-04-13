namespace Jyben.Racing.Telemetry.Services.Impl
{
    public class PermissionsService : IPermissionsService
    {
        public PermissionsService()
        {
        }

        public async Task<bool> CheckLocationPermission()
        {
            var statusLocation = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            
            return statusLocation == PermissionStatus.Granted || statusLocation == PermissionStatus.Restricted;
        }
    }
}
