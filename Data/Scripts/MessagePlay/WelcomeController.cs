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

namespace MessagePlay
{
    public class WelcomePanel
    {
        public bool enabled;
        public string title;
        public string subtitle;
        public string description;
        public string buttonCaption;

        public WelcomePanel()
        {
            Logger.Info("New WelcomePanel");

            enabled = false;
            title = "Title text";
            subtitle = "Subtitle text";
            description = "Main body text";
            buttonCaption = "Button text";
        }

        public void Show()
        {
            Logger.Info("WelcomePanel.Show");

            MyAPIGateway.Utilities.ShowMissionScreen(title, subtitle, null, MessagePlay.ExpandMacros(ref description), null, buttonCaption);
        }
    }
}
