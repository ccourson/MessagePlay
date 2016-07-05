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
using System.IO;

namespace MessagePlay
{
    public static class Configuration<T>
    {
        public static string ToXML(T obj)
        {
            Logger.Info("ControllerBase.ToXML");

            return MyAPIGateway.Utilities.SerializeToXML(obj);
        }

        public static T FromXML(string xml)
        {
            Logger.Info("ControllerBase.FromXML");

            return MyAPIGateway.Utilities.SerializeFromXML<T>(xml);
        }

        public static T LoadFromLocalStorage()
        {
            Logger.Info("LoadFromLocalStorage");

            T obj;

            if (MyAPIGateway.Utilities.FileExistsInLocalStorage(typeof(T).Name + ".xml", typeof(MessagePlay)))
            {
                // WelcomePanel.xml exists so load it.
                TextReader textReader = MyAPIGateway.Utilities.ReadFileInLocalStorage(typeof(T).Name + ".xml", typeof(MessagePlay));
                string buffer = textReader.ReadToEnd();
                textReader.Close();
                obj = MyAPIGateway.Utilities.SerializeFromXML<T>(buffer);
            }
            else
            {
                obj = default(T);
            }

            return obj;
        }

        public static void CreateInLocalStorage(T obj)
        {
            Logger.Info("SaveToLocalStorage");

            string buffer = MyAPIGateway.Utilities.SerializeToXML(obj);
            TextWriter textWriter = MyAPIGateway.Utilities.WriteFileInLocalStorage(typeof(T).Name + ".xml", typeof(MessagePlay));
            textWriter.Write(buffer);
            textWriter.Close();
        }
    }
}
