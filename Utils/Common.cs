using System;
using System.Collections.Generic;

namespace wsjt_message.Listener.Utils
{
    public class Common
    {
        #region Main Header
        /// <summary>
        /// Print Main Header
        /// </summary>
        public static void StartUpHeader(int port)
        {
            string date = "Date".PadRight(9);
            string utc = "UTC".PadRight(8);
            string db = "dB".PadRight(5);
            string dt = "dT".PadRight(5);
            string df = "dF".PadRight(5);
            string mode = "Mode".PadRight(7);
            Console.Clear();
            Console.WriteLine($"WSJT-X UDP Monitor");
            Console.WriteLine($"UDP Listen Port .: {port}");
            Console.WriteLine($"Little Endian....: {BitConverter.IsLittleEndian}");
            Console.WriteLine($"Start Date UTC ..: {DateTime.UtcNow}");
            Console.WriteLine("\n");
            Console.WriteLine("Program adatped for FT8OFF by KD2FMW\n");
            Console.WriteLine("FT8OFF Is ready to start receiving packets\n");
        }
        #endregion

        #region GetModeName
        /// <summary>
        /// Get the mode based on char[]
        /// ref: http://physics.princeton.edu/pulsar/K1JT/wsjtx-doc/wsjtx-main-2.1.0.html#_decoded_lines
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string mode name</returns>
        public static string GetModeName(string value)
        {
            string mode = "";
            string ft8 = "~";
            string jt4 = "$";
            string jt9 = "@";
            string jt65 = "#";
            string qra64 = ":";
            string msk144 = "&";

            if (value.Contains(ft8))
            {
                mode = "FT8";
            }
            else if (value.Contains(jt4))
            {
                mode = "JT4";
            }
            else if (value.Contains(jt9))
            {
                mode = "JT9";
            }
            else if (value.Contains(jt65))
            {
                mode = "JT65";
            }
            else if (value.Contains(qra64))
            {
                mode = "QRA64";
            }
            else if (value.Contains(msk144))
            {
                mode = "MSK144";
            }
            else
            {
                mode = "ISCAT";
            }
            return mode;
        }
        #endregion

        #region Convert Milliseconds to HHMMSS
        /// <summary>
        /// Convert time since Midnight (in milliseconds) to HH:MM:SS
        /// </summary>
        /// <param name="millisecs"></param>
        /// <returns>string Time in HH:MM:SS</returns>
        public static string GetTime(int millisecs)
        {
            return DateTime.FromBinary(599266080000000000).AddMilliseconds(millisecs).ToString("HHmmss");
        }
        #endregion

        #region Get Name based on UDP Message Id
        /// <summary>
        /// Get message name based on UDP message Id
        /// ref: https://sourceforge.net/p/wsjt/wsjtx/ci/master/tree/NetworkMessage.hpp
        /// </summary>
        /// <param name="id">int id representing message id</param>
        /// <returns>string name of message type</returns>
        public static string GetMessageTypeName(int id)
        {
            int number = id;
            string name;
            Dictionary<int, string> messageType = new Dictionary<int, string>() {
                {0, "Heartbeat"},
                {1, "Status"},
                {2, "Decode"},
                {3, "Clear"},
                {4, "Reply"},
                {5, "QSO Logged"},
                {6, "Close"},
                {7, "Reply"},
                {8, "Halt TX"},
                {9, "Free Text"},
                {10, "WSPR Decode"},
                {11, "Location"},
                {12, "Logged ADIF"},
                {13, "Highlight Callsign"},
                {14, "Switch Configuration"},
                {15, "Configure" }
            };

            if (messageType.ContainsKey(number))
            {
                name = messageType[id];
            }
            else
            {
                name = "Unknown";
            }
            return name;
        }
        #endregion

    } // end Class Common

} // end namespace JTMonitor.Listener.Utils.Common
