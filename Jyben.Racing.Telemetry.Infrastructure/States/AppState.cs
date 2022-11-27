using Jyben.Racing.Telemetry.Infrastructure.Models;

namespace Jyben.Racing.Telemetry.Infrastructure.States
{
    public class AppState
    {
        public PermissionsModel? Permissions { get; set; }

        /// <summary>
        /// The event that will be raised for state changed
        /// </summary>
        public event Action OnStateChange = default!;

        /// <summary>
        /// The method that will be accessed by the sender component 
        /// to update the state
        /// </summary>
        public void SetPermissions(PermissionsModel permissions)
        {
            Permissions = permissions;
            NotifyStateChanged();
        }

        /// <summary>
        /// The state change event notification
        /// </summary>
        private void NotifyStateChanged() => OnStateChange?.Invoke();
    }
}