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
using System.IO;

namespace MessagePlay
{
    public static class Logger
    {
        private static Log log;

        public static void Init(string filename, ErrorLevel errorlevel)
        {
            log = new Log(filename, errorlevel);
            log.Add(ErrorLevel.INFO, "Logger initialized");
        }

        public static void Fatal(string text, object[] args = null)
        {
            if (log != null) log.Add(ErrorLevel.FATAL, args == null ? text : string.Format(text, args));
        }

        public static void Error(string text, object[] args = null)
        {
            if (log != null) log.Add(ErrorLevel.ERROR, args == null ? text : string.Format(text, args));
        }

        public static void Warn(string text, object[] args = null)
        {
            if (log != null) log.Add(ErrorLevel.WARN, args == null ? text : string.Format(text, args));
        }

        public static void Info(string text, object[] args = null)
        {
            if (log != null) log.Add(ErrorLevel.INFO, args == null ? text : string.Format(text, args));
        }

        public static void Debug(string text, object[] args = null)
        {
            if (log != null) log.Add(ErrorLevel.DEBUG, args == null ? text : string.Format(text, args));
        }

        public static void Indent()
        {
            if (log != null) log.indention++;
        }

        public static void Outdent()
        {
            if (log != null && log.indention > 0) log.indention--;
        }

        public static void Close()
        {
            if (log != null) log.Close();
        }

        public class Log
        {
            public int indention;
            private ErrorLevel errorlevel;
            private TextWriter textWriter;

            public Log(string filename, ErrorLevel errorlevel)
            {
                this.errorlevel = errorlevel;
                textWriter = MyAPIGateway.Utilities.WriteFileInLocalStorage(filename + ".log", typeof(Logger));
            }

            public void Add(ErrorLevel errorlevel, string entry)
            {
                if (this.errorlevel >= errorlevel)
                {
                    textWriter.WriteLineAsync(string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] - {1,8}:   {2}{3}",
                        DateTime.Now, errorlevel, new string(' ', 3 * indention), entry));
                    textWriter.FlushAsync();
                }
            }

            public void Close()
            {
                log.Add(ErrorLevel.INFO, "Logger closed");
                textWriter.Flush();
                textWriter.Close();
            }
        }

        public enum ErrorLevel
        {
            NONE,
            FATAL,
            ERROR,
            WARN,
            INFO,
            DEBUG
        }
    }
}
