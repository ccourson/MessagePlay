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
using System.Text;

namespace MessagePlay
{
    public class Message
    {
        public ulong playerId;
        public MessageType messageType;
        public string message;

        public Message()
        {

        }

        public Message(MessageType messageType, string message)
        {
            Logger.Info("Message(Message, object)");

            playerId = MyAPIGateway.Multiplayer.MyId;
            this.messageType = messageType;
            this.message = message;
        }

        public Message(byte[] bytes)
        {
            Logger.Info("Message(byte[])");

            string xml = Encoding.ASCII.GetString(bytes);
            Logger.Debug(xml);
            Message message = MyAPIGateway.Utilities.SerializeFromXML<Message>(xml);
            Logger.Debug(string.Format("Decoded message type: {0}, playerId: {1}", message.messageType, message.playerId));
            this.playerId = message.playerId;
            this.messageType = message.messageType;
            this.message = message.message;
        }

        public byte[] GetBytes()
        {
            Logger.Info("Message.GetBytes");

            string xml = MyAPIGateway.Utilities.SerializeToXML(this);
            return Encoding.ASCII.GetBytes(xml);
        }

        public void SendToServer()
        {
            Logger.Info("Message.SendToServer");

            MyAPIGateway.Multiplayer.SendMessageToServer(1001, GetBytes());
        }

        public void SendTo(ulong id)
        {
            Logger.Info("Message.SendTo");

            Logger.Debug(id.ToString());
            MyAPIGateway.Multiplayer.SendMessageTo(1001, GetBytes(), id);
        }

        public void SendToOthers()
        {
            Logger.Info("Message.SendToOthers");

            MyAPIGateway.Multiplayer.SendMessageToOthers(1001, GetBytes());
        }

        public static void RequestWelcome()
        {
            Logger.Info("Message.RequestWelcome");

            MyAPIGateway.Multiplayer.SendMessageToServer(1001, new Message(MessageType.WelcomeRequest, null).GetBytes());
        }

        public static void RequestHelp()
        {
            Logger.Info("Message.RequestHelp");

            MyAPIGateway.Multiplayer.SendMessageToServer(1001, new Message(MessageType.HelpRequest, null).GetBytes());
        }

        public static void RequestReload()
        {
            Logger.Info("Message.RequestReload");

            MyAPIGateway.Multiplayer.SendMessageToServer(1001, new Message(MessageType.ReloadRequest, null).GetBytes());
        }
    }

    public enum MessageType
    {
        WelcomeRequest,
        WelcomeResponse,
        HelpRequest,
        HelpResponse,
        Announcement,
        ReloadRequest,
        ReloadResponse
    }
}