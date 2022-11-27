namespace Jyben.Racing.Telemetry.Services.Impl
{
    public class PermissionsService : IPermissionsService
    {
        public PermissionsService()
        {
        }

        public async Task<bool> CheckLocationPermission()
        {
            var statusLocation = await Permissions.RequestAsync<Permissions.LocationAlways>();
            
            return statusLocation == PermissionStatus.Granted || statusLocation == PermissionStatus.Restricted;
        }

        public async Task<bool> CheckSensorsPermission()
        {
            var statusSensors = await Permissions.RequestAsync<Permissions.Sensors>();

            return statusSensors == PermissionStatus.Granted;
        }
    }
}
