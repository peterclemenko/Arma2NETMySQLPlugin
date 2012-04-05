﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arma2NETMySQLPlugin
{
    class Logger
    {
        public enum loggerState
        {
            Started,
            Stopped
        }

        public enum LogType
        {
            Info,
            Warning,
            Error
        }

        public loggerState state = loggerState.Stopped;
        public loggerState State { get { return state; } }

        private static FileStream fs = null;
        private static StreamWriter sw = null;

        public Logger()
        {
            //Constructor
            if (State == loggerState.Stopped)
            {
                //check to see if the logs folder exists, if not create it
                if (!System.IO.Directory.Exists("logs"))
                {
                    System.IO.Directory.CreateDirectory("logs");
                }

                //Setup file streams
                DateTime dateValue = new DateTime();
                dateValue = DateTime.Now;
                string relativepath = Path.Combine("logs", dateValue.ToString("MM-dd-yyyy_HH-mm-ss") + ".log");
                fs = new FileStream(relativepath, FileMode.Append);
                sw = new StreamWriter(fs);

                state = loggerState.Started;
            }
        }

        public static void addMessage(LogType type, string message)
        {
            DateTime time = new DateTime();
            time = DateTime.Now;
            string towrite = type.ToString() + ": " + time.ToString("HH:mm:ss - ") + message;

            //This locks file writing for a second, this prevents multiple external threads to be writing to the file
            //using this method at exactly the same time.
            //http://msdn.microsoft.com/en-us/library/c5kehkcz.aspx
            //http://www.dotnetperls.com/lock
            lock (sw)
            {
                sw.WriteLine(towrite);
                sw.Flush();
            }
        }

        public void Stop()
        {
            if (State == loggerState.Started)
            {
                try
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION: An exception occured while stopping the logger.\n**\t{0}", ex.ToString());
                }
            }
        }
    }
}