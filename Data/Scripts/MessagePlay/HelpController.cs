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
using System.Collections.Generic;

namespace MessagePlay
{
    public class HelpTopics
    {
        public bool enabled;
        public List<HelpTopic> topics;

        public HelpTopics()
        {
            Logger.Info("New HelpTopics");

            enabled = true;
            topics = new List<HelpTopic>();
            topics.Add(new HelpTopic("help", "Title", "Subtitle", "=== Chat Commands ===\r\nhelp - This help panel."));
        }

        public bool HasHelpKey(string key)
        {
            Logger.Info("HasHelpKey");

            foreach (HelpTopic topic in topics)
            {
                if (topic.key.Equals(key)) return true;
            }
            return false;
        }

        public void ShowHelpFor(string key)
        {
            Logger.Info("ShowHelpFor");

            foreach (HelpTopic topic in topics)
            {
                if (topic.key.Equals(key))
                {
                    MyAPIGateway.Utilities.ShowMissionScreen(topic.title, topic.subtitle, null, MessagePlay.ExpandMacros(ref topic.description));
                    return;
                }
            }
        }
    }

    public class HelpTopic
    {
        public string key;
        public string title;
        public string subtitle;
        public string description;

        public HelpTopic()
        {
            Logger.Info("New HelpTopic");
        }

        public HelpTopic(string key, string title, string subtitle, string description)
        {
            Logger.Info("New HelpTopic(key, title, subtitle, description)");

            this.key = key;
            this.title = title;
            this.subtitle = subtitle;
            this.description = description;
        }
    }
}
