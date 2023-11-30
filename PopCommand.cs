using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Oxide.Plugins
{
    [Info("PopCommand", "HandyS11", "1.0.0")]
    [Description("Displays the population informations of your server")]
    public class PopCommand : RustPlugin
    {
        #region Fields

        private const string popCommand = "popcommand.use";
        private const string popCommandAdmin = "popcommand.admin";

        private Configuration config;

        #endregion

        #region Configuration

        private sealed class Configuration
        {
            [JsonProperty(PropertyName = "Use permission for lambda players")]
            public bool UsePermissionForPlayers { get; set; }

            [JsonProperty(PropertyName = "Broadcast to every player on /pop")]
            public bool DoBroadcast { get; set; }

            [JsonProperty(PropertyName = "Chat default avatar")]
            public ulong ChatAvatar { get; set; }

            [JsonProperty(PropertyName = "Display Options")]
            public DisplayOptions DisplayOption { get; set; }
        }

        public class DisplayOptions
        {
            [JsonProperty(PropertyName = "Show player count")]
            public bool ShowPlayerCount { get; set; }

            [JsonProperty(PropertyName = "Show server slots")]
            public bool ShowServerSlots { get; set; }

            [JsonProperty(PropertyName = "Show sleepers")]
            public bool ShowSleepers { get; set; }

            [JsonProperty(PropertyName = "Show joining players")]
            public bool ShowJoiningPlayers { get; set; }

            [JsonProperty(PropertyName = "Show players in queue")]
            public bool ShowPlayersInQueue { get; set; }
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                UsePermissionForPlayers = false,
                DoBroadcast = false,
                ChatAvatar = 0,
                DisplayOption = new DisplayOptions
                {
                    ShowPlayerCount = true,
                    ShowServerSlots = true,
                    ShowSleepers = false,
                    ShowJoiningPlayers = false,
                    ShowPlayersInQueue = false,
                },
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<Configuration>();

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config, true);
        }

        #endregion

        #region Hooks

        private void Init()
        {
            permission.RegisterPermission(popCommand, this);
            permission.RegisterPermission(popCommandAdmin, this);
        }

        private void Unload()
        {
            config = null;
        }

        #endregion

        #region Functions

        private void SendChatMessage(BasePlayer player, string message)
        {
            Player.Message(player, message, config.ChatAvatar);
        }

        private void SendMessageToAll(string message)
        {
            Server.Broadcast(message, config.ChatAvatar);
        }

        private bool HasPermission(BasePlayer player, string permissionName)
        {
            return permission.UserHasPermission(player.UserIDString, permissionName);
        }

        private List<int> GetDatas()
        {
            List<int> datas = new List<int>();

            if (config.DisplayOption.ShowPlayerCount)
                datas.Add(BasePlayer.activePlayerList.Count);
            if (config.DisplayOption.ShowServerSlots)
                datas.Add(ServerMgr.AvailableSlots);
            if (config.DisplayOption.ShowSleepers)
                datas.Add(BasePlayer.sleepingPlayerList.Count);
            if (config.DisplayOption.ShowJoiningPlayers)
                datas.Add(ServerMgr.Instance.connectionQueue.joining.Count);
            if (config.DisplayOption.ShowPlayersInQueue)
                datas.Add(ServerMgr.Instance.connectionQueue.queue.Count);

            return datas;
        }

        private bool VerifyPlaceholders(string template, int expectedCount)
        {
            int actualCount = Regex.Matches(template, @"\{\d+\}").Count;
            return actualCount <= expectedCount;
        }

        #endregion

        #region Command

        private static class Command
        {
            public const string PopPlayer = "pop";
            public const string PopAdmin = "apop";
        }

        [ChatCommand(Command.PopPlayer)]
        private void PopPlayer(BasePlayer player, string command, string[] args)
        {
            if (config.UsePermissionForPlayers && !HasPermission(player, popCommand))
            {
                SendChatMessage(player, GetMessage(MessageKey.PopPermissionDeny, player.UserIDString));
                return;
            }

            if (config.DoBroadcast)
            {
                SendMessageToAll(GetMessage(MessageKey.PopMessage, player.UserIDString, GetDatas().Cast<object>().ToArray()));
                return;
            }
            SendChatMessage(player, GetMessage(MessageKey.PopMessage, player.UserIDString, GetDatas().Cast<object>().ToArray()));
        }

        [ChatCommand(Command.PopAdmin)]
        private void PopAdmin(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin || !HasPermission(player, popCommandAdmin))
            {
                SendChatMessage(player, GetMessage(MessageKey.PopAdminPermissionDeny, player.UserIDString));
                return;
            }

            SendChatMessage(player, GetMessage(MessageKey.PopMessageAdmin, player.UserIDString,
                BasePlayer.activePlayerList.Count,
                ServerMgr.AvailableSlots,
                BasePlayer.sleepingPlayerList.Count,
                ServerMgr.Instance.connectionQueue.joining.Count,
                ServerMgr.Instance.connectionQueue.queue.Count
            ));
        }

        #endregion

        #region Localization

        private static class MessageKey
        {
            public const string PopMessage = "PopCommand.ChatMessage";
            public const string PopMessageAdmin = "PopCommand.AdminMessage";
            public const string PopPermissionDeny = "PopCommand.PermissionDeny";
            public const string PopAdminPermissionDeny = "PopCommand.AdminPermissionDeny";
            public const string PopError = "PopCommand.Error";
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [MessageKey.PopMessage] = "Players onlines: {0}/{1}",
                [MessageKey.PopMessageAdmin] = "Players onlines: {0}/{1} | Sleeping: {2} | Joining: {3} | Queued: {4}",
                [MessageKey.PopPermissionDeny] = "You are not allowed to run this command!",
                [MessageKey.PopAdminPermissionDeny] = "Only administrators can run this command!",
                [MessageKey.PopError] = "The config/lang file contains some errors!",
            }, this);
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [MessageKey.PopMessage] = "Joueurs en ligne : {0}/{1}",
                [MessageKey.PopMessageAdmin] = "Joueurs en ligne : {0}/{1} | Endormi : {2} | En train de rejoindre : {3} | Dans la queue : {4}",
                [MessageKey.PopPermissionDeny] = "Vous n'êtes pas autorisé à utiliser cette commande !",
                [MessageKey.PopAdminPermissionDeny] = "Seul les administrateurs peuvent utiliser cette commande !",
                [MessageKey.PopError] = "Le fichier de configuration/traduction contient des erreurs !",
            }, this, "fr");
        }

        private string GetMessage(string messageKey, string playerId = null, params object[] datas)
        {
            try
            {
                string template = lang.GetMessage(messageKey, this, playerId);
                if (!VerifyPlaceholders(template, datas.Length))
                {
                    Puts("Wrong number of params compared to the string");
                    return lang.GetMessage(MessageKey.PopError, this, playerId);
                }
                return string.Format(template, datas);
            }
            catch (Exception exception)
            {
                PrintError(exception.ToString());
                throw;
            }
        }

        #endregion
    }
}
