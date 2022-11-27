using Jyben.Racing.Telemetry.Infrastructure.Models;
using Jyben.Racing.Telemetry.Infrastructure.States;
using Jyben.Racing.Telemetry.Services;
using Microsoft.AspNetCore.Components;

namespace Jyben.Racing.Telemetry.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] public IPermissionsService PermissionsService { get; set; }
        [Inject] public AppState AppState { get; set; }

        private bool _isDisposed;

        protected override async Task OnInitializedAsync()
        {
            AppState.OnStateChange += OnAppStateChanged;

            var hasSensorsPermission = await PermissionsService.CheckSensorsPermission();
            var hasLocationPermission = await PermissionsService.CheckLocationPermission();

            var permissionsModel = new PermissionsModel()
            {
                HasLocationPermissions = hasSensorsPermission,
                HasSensorsPermissions = hasLocationPermission
            };

            AppState.SetPermissions(permissionsModel);
        }

        private void OnAppStateChanged()
        {
            StateHasChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                AppState.OnStateChange -= OnAppStateChanged;
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}