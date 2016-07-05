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
using System.Collections.Generic;

namespace MessagePlay
{
    public class Announcements
    {
        public bool enabled { get; set; }
        public int intervalMinutes;
        public int delaySeconds;
        public List<Announcement> announcements { get; set; }
        private int index;

        public Announcements()
        {
            Logger.Info("New Announcements");

            enabled = false;
            intervalMinutes = 1;
            delaySeconds = 10;
            announcements = new List<Announcement>();
            index = 0;
        }
    }

    public class Announcement
    {
        public string sender;
        public string message;

        public Announcement()
        {

        }

        public Announcement(string sender, string message)
        {
            Logger.Info("New Announcement");

            this.sender = sender;
            this.message = message;
        }
    }
}
