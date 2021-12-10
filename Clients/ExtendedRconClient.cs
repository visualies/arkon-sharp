using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ArkonSharp.Entities;

namespace ArkonSharp.Clients
{
    public class ExtendedRconClient : BaseClient
    {
        public ExtendedRconClient(string address, int port, string password) : base(address, port, password)
        {
        }

        public async Task<string> AddExperienceAsync(long steamId, int amount, bool tribeShare,
            bool preventSharingWithTribe)
        {
            var command =
                $"AddExperience {steamId} {amount} {Convert.ToInt32(tribeShare)} {Convert.ToInt32(preventSharingWithTribe)}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> UnlockEngramAsync(long steamId, string blueprintPath)
        {
            var command = $"UnlockEngram {steamId} {blueprintPath}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendChatMessageAsync(string message, string author)
        {
            var command = $"ClientChat '{message}' {author}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> GiveItemAsync(long steamId, string blueprintPath, int amount, int quality,
            bool foreBlueprint = false)
        {
            var command = $"GiveItem {steamId} {blueprintPath} {amount} {quality} {Convert.ToInt32(foreBlueprint)}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendServerMessageToTribeAsync(long tribeId, string message)
        {
            var command = $"TribeChatMsg {1} {tribeId} {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendBroadcastMessageToTribeAsync(long tribeId, string message)
        {
            var command = $"TribeChatMsg {2} {tribeId} {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendNotificationMessageToTribeAsync(long tribeId, string message)
        {
            var command = $"TribeChatMsg {3} {tribeId} {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<long> GetTribeIdOfPlayerAsync(long steamId)
        {
            var command = $"GetTribeIdOfPlayer {steamId}";
            var response = await ExecuteCommandAsync(command);

            if (response.Contains("Not enough arguments")) throw new Exception("Player is no longer online");

            var enumerable = response.Reverse().Skip(1).TakeWhile(char.IsDigit).Reverse();
            var tribeId = Convert.ToInt64(string.Concat(enumerable));

            return tribeId;
        }

        public async Task<IEnumerable<ExtendedRconPlayerOnline>> GetOnlinePlayersAsync()
        {
            var playerList = new List<ExtendedRconPlayerOnline>();
            var command = "ListAllPlayerSteamID";
            var response = await ExecuteCommandAsync(command);

            switch (response)
            {
                case "No Players Online\n":
                    return playerList;
                case "Command returned no data\n":
                    return playerList;
            }

            var lines = response.Split('\n');
            foreach (var line in lines)
            {
                if (line.Length < 3) continue;

                var steamId = string.Concat(line.Reverse().TakeWhile(char.IsDigit).Reverse());
                var tribename = string.Concat(line.Reverse().SkipWhile(a => a != ']').TakeWhile(a => a != '[').Skip(1).Reverse());
                var playername = string.Concat(line.Reverse().SkipWhile(a => a != '[').Skip(2).Reverse());
                
                var player = new ExtendedRconPlayerOnline(Convert.ToInt64(steamId), playername, tribename);

                playerList.Add(player);
            }

            return playerList;
        }

        public async Task<IEnumerable<ExtendedRconPlayerPosition>> GetOnlinePlayerLocationsAsync()
        {
            var positionList = new List<ExtendedRconPlayerPosition>();

            var command = "ListAllPlayerSteamID";
            var response = await ExecuteCommandAsync(command);

            if (response == "\n") return positionList;
            if (response == "Command returned no data\n") return positionList;

            var lines = response.Split('\n');
            foreach (var line in lines)
            {
                if (line.Length < 3) continue;

                const string bracket = ")";
                var bracketCount = (line.Length - line.Replace(bracket, "").Length) / bracket.Length;

                //hpyhen comparison needs an extra string since coords can contain hyphen
                var hypenString = string.Concat(line.Reverse().SkipWhile(a => a != 'X').Reverse());
                var hyphen = "-";
                var hyphenCount = (line.Length - line.Replace(hyphen, "").Length) / hyphen.Length;

                string playername;
                string tribename;

                if (line.Count(f => f == '(') == 1)
                {
                    playername = string.Concat(line.Reverse().SkipWhile(a => a != '(').Skip(4).Reverse());
                    tribename = string.Concat(line.Reverse().SkipWhile(a => a != ')').Skip(1).TakeWhile(a => a != '(')
                        .Reverse());
                }
                else if (hypenString.Count(f => f == '-') == 1)
                {
                    playername = string.Concat(hypenString.Reverse().SkipWhile(a => a != '-').Skip(2).Reverse());
                    tribename = string.Concat(hypenString.Reverse().SkipWhile(a => a != ')').Skip(1)
                        .TakeWhile(a => a != '-').Reverse().Skip(2));
                }
                else
                {
                    //continue if tribe name and name are impossible to separate 
                    continue;
                }

                //continue if player is "" (player is dead)
                if (string.IsNullOrWhiteSpace(playername)) continue;

                var z = float.Parse(string.Concat(line.Reverse().TakeWhile(a => a != '=').Reverse()),
                    CultureInfo.InvariantCulture);
                var y = float.Parse(
                    string.Concat(line.Reverse().SkipWhile(a => a != 'Z').Skip(1).TakeWhile(a => a != '=').Reverse()),
                    CultureInfo.InvariantCulture);
                var x = float.Parse(
                    string.Concat(line.Reverse().SkipWhile(a => a != 'Y').Skip(1).TakeWhile(a => a != '=').Reverse()),
                    CultureInfo.InvariantCulture);

                var coords = new Vector3(x, y, z);
                var position = new ExtendedRconPlayerPosition(coords, playername, tribename);

                positionList.Add(position);
            }

            return positionList;
        }
    }
}