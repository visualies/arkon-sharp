using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class ArkPermissionsClient : BaseClient
    {
        public ArkPermissionsClient(string address, int port, string password) : base(address, port, password)
        {
        }

        #region Player Commands
        public async Task<string> AddPlayerPermissionGroupAsync(long steamId, string group)
        {
            var command = $"Permissions.Add {steamId} {group}";
            return await ExecuteCommandAsync(command);
        }
        
        public async Task<string> RemovePlayerPermissionGroupAsync(long steamId, string group)
        {
            var command = $"Permissions.Remove {steamId} {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> AddTimedPlayerPermissionGroupAsync(long steamId, string group, int hours, int delayHours)
        {
            var command = $"Permissions.AddTimed {steamId} {group} {hours} {delayHours}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> RemoveTimedPlayerPermissionGroupAsync(long steamId, string group)
        {
            var command = $"Permissions.RemoveTimed {steamId} {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> ListPlayerPermissionGroupsAsync(long steamId)
        {
            var command = $"Permissions.PlayerGroups {steamId}";
            return await ExecuteCommandAsync(command);
        }

        #endregion


        #region Tribe Commands
        public async Task<string> AddTribePermissionGroupAsync(long tribeId, string group)
        {
            var command = $"Permissions.AddTribe {tribeId} {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> RemoveTribePermissionGroupAsync(long tribeId, string group)
        {
            var command = $"Permissions.RemoveTribe {tribeId} {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> AddTimedTribePermissionGroupAsync(long tribeId, string group, int hours, int delayHours)
        {
            var command = $"Permissions.AddTimedTribe {tribeId} {group} {hours} {delayHours}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> RemoveTimedTribePermissionGroupAsync(long tribeId, string group)
        {
            var command = $"Permissions.RemoveTimedTribe {tribeId} {group}";
            return await ExecuteCommandAsync(command);
        }
        #endregion


        #region GroupCommands
        public async Task<string> AddGroupAsync(string group)
        {
            var command = $"Permissions.AddGroup {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> RemoveGroupAsync(string group)
        {
            var command = $"Permissions.RemoveGroup {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> GrantGroupPermissionAsync(string group, string permission)
        {
            var command = $"Permissions.Grant {group} {permission}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> RevokeGroupPermissionAsync(string group, string permission)
        {
            var command = $"Permissions.Revoke {group} {permission}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> ListGroupPermissionsAsync(string group)
        {
            var command = $"Permissions.GroupPermissions {group}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> ListGroupsAsync()
        {
            var command = $"Permissions.ListGroups";
            return await ExecuteCommandAsync(command);
        }

        #endregion
    }
}