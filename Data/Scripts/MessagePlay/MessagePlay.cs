/*
 *  Copyright (C) Chris Courson, 2016. All rights reserved.
 * 
 *  This file is part of MessagePlay, a Space Engineers mod available through Steam.
 *
 *  Foobar is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Foobar is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */
using Sandbox.ModAPI;
using System;
using System.Timers;
using VRage.Game.Components;

namespace MessagePlay
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class MessagePlay : MySessionComponentBase
    {
        private bool isInit;
        private HelpTopics helpTopics;
        private WelcomePanel welcomePanel;
        private Announcements announcements;
        private Timer timer;
        private int messageIndex;
        private bool welcomePanelShown;

        public void Init()
        {
            isInit = true;

            Logger.Init(typeof(MessagePlay).Name, Logger.ErrorLevel.NONE);

            Logger.Info("Init - START");
            Logger.Indent();

            MyAPIGateway.Multiplayer.RegisterMessageHandler(1001, MessageHandler);
            Logger.Debug("MessageHandler registered with id 1001.");

            Logger.Debug("MyAPIGateway.Session.Player is null: " + (MyAPIGateway.Session.Player == null));
            if (MyAPIGateway.Multiplayer.IsServer) LoadConfigurations();

            if (!MyAPIGateway.Multiplayer.IsServer || MyAPIGateway.Session.Player != null)
            {
                MyAPIGateway.Utilities.MessageEntered += MessageEnteredHandler;
                Message.RequestWelcome();
                Message.RequestHelp();
                Logger.Debug("RequestWelcome");
            }

            Logger.Outdent();
            Logger.Info("Init - END");
        }

        private void LoadConfigurations()
        {
            if (timer != null) timer.Close();

            Logger.Debug("LoadConfigurations");
            helpTopics = Configuration<HelpTopics>.LoadFromLocalStorage();
            if (helpTopics == null)
            {
                helpTopics = new HelpTopics();
                Configuration<HelpTopics>.CreateInLocalStorage(helpTopics);
            }
            welcomePanel = Configuration<WelcomePanel>.LoadFromLocalStorage();
            if (welcomePanel == null)
            {
                welcomePanel = new WelcomePanel();
                Configuration<WelcomePanel>.CreateInLocalStorage(welcomePanel);
            }
            announcements = Configuration<Announcements>.LoadFromLocalStorage();
            if (announcements == null)
            {
                announcements = new Announcements();
                Configuration<Announcements>.CreateInLocalStorage(announcements);
            }

            timer = new Timer()
            {
                Interval = announcements.delaySeconds * 60000,
                AutoReset = true,
                Enabled = announcements.enabled
            };
            timer.Elapsed += Timer_Elapsed;
            messageIndex = 0;
            timer.Start();
        }

        // this is called at fps rate (60Hz default)
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (!isInit) Init();
        }

        private void MessageHandler(byte[] obj)
        {
            Logger.Info("MessageHandler - START");
            Logger.Indent();

            Message message = new Message(obj);
            Logger.Debug("Message from: " + message.playerId);

            switch (message.messageType)
            {
                case MessageType.WelcomeRequest:
                    Logger.Debug("WelcomeRequest");
                    new Message(MessageType.WelcomeResponse, Configuration<WelcomePanel>.ToXML(welcomePanel)).SendTo(message.playerId);
                    break;
                case MessageType.WelcomeResponse:
                    Logger.Debug("WelcomeResponse");
                    welcomePanel = Configuration<WelcomePanel>.FromXML(message.message);
                    if (welcomePanel.enabled && !welcomePanelShown) welcomePanel.Show();
                    welcomePanelShown = true;
                    break;
                case MessageType.HelpRequest:
                    Logger.Debug("HelpRequest");
                    new Message(MessageType.HelpResponse, Configuration<HelpTopics>.ToXML(helpTopics)).SendTo(message.playerId);
                    break;
                case MessageType.HelpResponse:
                    Logger.Debug("HelpResponse");
                    helpTopics = Configuration<HelpTopics>.FromXML(message.message);
                    break;
                case MessageType.Announcement:
                    Logger.Debug("Announcement");
                    Announcement announcement = Configuration<Announcement>.FromXML(message.message);
                    MyAPIGateway.Utilities.ShowMessage(announcement.sender, announcement.message);
                    break;
                case MessageType.ReloadRequest:
                    Logger.Debug("ReloadRequest");
                    LoadConfigurations();
                    new Message(MessageType.WelcomeResponse, Configuration<WelcomePanel>.ToXML(welcomePanel)).SendToOthers();
                    new Message(MessageType.HelpResponse, Configuration<HelpTopics>.ToXML(helpTopics)).SendToOthers();
                    new Message(MessageType.ReloadResponse, null).SendTo(message.playerId);
                    break;
                case MessageType.ReloadResponse:
                    Logger.Debug("ReloadResponse");
                    MyAPIGateway.Utilities.ShowMessage("MessagePlay", "Reloaded.");
                    break;
            }

            Logger.Outdent();
            Logger.Info("MessageHandler - END");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Logger.Info("Timer_Elapsed");
            if (announcements.enabled)
            {
                Announcement announcement = new Announcement(announcements.announcements[messageIndex].sender, announcements.announcements[messageIndex].message);
                new Message(MessageType.Announcement, Configuration<Announcement>.ToXML(announcement)).SendToOthers();
                messageIndex = (messageIndex + 1) % announcements.announcements.Count;
                timer.Interval = announcements.intervalMinutes * 10000;
            }
        }

        protected override void UnloadData()
        {
            Logger.Info("UnloadData");

            MyAPIGateway.Multiplayer.UnregisterMessageHandler(1001, MessageHandler);
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                timer.Close();
            }
            if (!MyAPIGateway.Multiplayer.IsServer || MyAPIGateway.Session.Player != null)
            {
                MyAPIGateway.Utilities.MessageEntered -= MessageEnteredHandler;
            }

            Logger.Close();
        }

        internal void MessageEnteredHandler(string messageText, ref bool sendToOthers)
        {
            Logger.Info("MessageEnteredHandler - START");
            Logger.Indent();

            if (messageText.Equals("mp reload"))
            {
                Message.RequestReload();
                sendToOthers = false;
            }
            else if (helpTopics.HasHelpKey(messageText))
            {
                helpTopics.ShowHelpFor(messageText);
                sendToOthers = false;
            }

            Logger.Outdent();
            Logger.Info("MessageEnteredHandler - END");
        }

        public static string ExpandMacros(ref string text)
        {
            string expandedText = string.Copy(text);
            expandedText = expandedText.Replace("@PlayerID", MyAPIGateway.Multiplayer.MyId.ToString());
            expandedText = expandedText.Replace("@PlayerName", MyAPIGateway.Multiplayer.MyName);
            expandedText = expandedText.Replace("@ServerID", MyAPIGateway.Multiplayer.ServerId.ToString());
            expandedText = expandedText.Replace("@AssemblerEfficiencyMultiplier", MyAPIGateway.Session.AssemblerEfficiencyMultiplier.ToString());
            expandedText = expandedText.Replace("@AssemblerSpeedMultiplier", MyAPIGateway.Session.AssemblerSpeedMultiplier.ToString());
            expandedText = expandedText.Replace("@AutoHealing", MyAPIGateway.Session.AutoHealing.ToString());
            expandedText = expandedText.Replace("@CargoShipsEnabled", MyAPIGateway.Session.CargoShipsEnabled.ToString());
            expandedText = expandedText.Replace("@CreativeMode", MyAPIGateway.Session.CreativeMode.ToString());
            expandedText = expandedText.Replace("@Description", MyAPIGateway.Session.Description);
            expandedText = expandedText.Replace("@ElapsedPlayTime", MyAPIGateway.Session.ElapsedPlayTime.ToString());
            expandedText = expandedText.Replace("@EnvironmentHostility", MyAPIGateway.Session.EnvironmentHostility.ToString());
            expandedText = expandedText.Replace("@GameDateTime", MyAPIGateway.Session.GameDateTime.ToString());
            expandedText = expandedText.Replace("@GrinderSpeedMultiplier", MyAPIGateway.Session.GrinderSpeedMultiplier.ToString());
            expandedText = expandedText.Replace("@HackSpeedMultiplier", MyAPIGateway.Session.HackSpeedMultiplier.ToString());
            expandedText = expandedText.Replace("@HasAdminPrivileges", MyAPIGateway.Session.HasAdminPrivileges.ToString());
            expandedText = expandedText.Replace("@InventoryMultiplier", MyAPIGateway.Session.InventoryMultiplier.ToString());
            expandedText = expandedText.Replace("@MaxFloatingObjects", MyAPIGateway.Session.MaxFloatingObjects.ToString());
            expandedText = expandedText.Replace("@MaxPlayers", MyAPIGateway.Session.MaxPlayers.ToString());
            expandedText = expandedText.Replace("@WorldName", MyAPIGateway.Session.Name.ToString());
            expandedText = expandedText.Replace("@IsDedicated", MyAPIGateway.Utilities.IsDedicated.ToString());
            expandedText = expandedText.Replace("@AutoSaveInMinutes", MyAPIGateway.Session.AutoSaveInMinutes.ToString());
            expandedText = expandedText.Replace("@IsAdmin", MyAPIGateway.Session.Player.IsAdmin.ToString());
            expandedText = expandedText.Replace("@IsPromoted", MyAPIGateway.Session.Player.IsPromoted.ToString());
            expandedText = expandedText.Replace("@SteamUserId", MyAPIGateway.Session.Player.SteamUserId.ToString());
            expandedText = expandedText.Replace("@DisplayName", MyAPIGateway.Session.Player.DisplayName.ToString());
            expandedText = expandedText.Replace("@RefinerySpeedMultiplier", MyAPIGateway.Session.RefinerySpeedMultiplier.ToString());
            expandedText = expandedText.Replace("@TimeOnBigShip", MyAPIGateway.Session.TimeOnBigShip.ToString());
            expandedText = expandedText.Replace("@TimeOnFoot", MyAPIGateway.Session.TimeOnFoot.ToString());
            expandedText = expandedText.Replace("@TimeOnJetpack", MyAPIGateway.Session.TimeOnJetpack.ToString());
            expandedText = expandedText.Replace("@TimeOnSmallShip", MyAPIGateway.Session.TimeOnSmallShip.ToString());
            expandedText = expandedText.Replace("@WeaponsEnabled", MyAPIGateway.Session.WeaponsEnabled.ToString());
            expandedText = expandedText.Replace("@WelderSpeedMultiplier", MyAPIGateway.Session.WelderSpeedMultiplier.ToString());
            return expandedText;
        }
    }
}
