using System;
using System.Linq;
using System.Text;

namespace wsjt_message.Listener.Utils
{
    public class ArrayTools
    {
        #region Byte ToBoolean
        /// <summary>
        /// Btye to ToBoolean, Credit : Laurie, VK4AMA
        /// </summary>
        /// <param name="bytes">byte[] bytes</param>
        /// <param name="skip">int number of bytes to skip</param>
        /// <param name="take">int number of bytes to take</param>
        /// <returns>bool</returns>
        public static bool ToBoolean(byte[] bytes, int skip, int take)
        {
            if (bytes != null && skip + take <= bytes.Length && take == 1)
            {
                return BitConverter.ToBoolean(bytes.Skip(skip).Take(take).ToArray(), 0);
            }

            return false;
        }
        #endregion

        #region Byte ToAsciiString
        /// <summary>
        /// Convert Byte To ASCII String, Credit: Laurie, VK4AMA
        /// KI7MT Comment: The conditional protion of this method is causing random
        /// null messages. Just a guess, but, the byte.length is probably changing
        /// in the byte[] causing the method to return an empy string. 
        /// </summary>
        /// <param name="bytes">byte[] bytes</param>
        /// <param name="skip">int number of bytes to skip</param>
        /// <param name="take">int number of bytes to take</param>
        /// <returns>string</returns>
        public static string ToAsciiString(byte[] bytes, int skip, int take)
        {
            return Encoding.ASCII.GetString(bytes.Skip(skip).Take(take).ToArray());
            // if (bytes != null && skip + take <= bytes.Length && take > 0)
            // {
            //     return Encoding.ASCII.GetString(bytes.Skip(skip).Take(take).ToArray());
            // }

            //return string.Empty;
        }
        #endregion

        #region Byte ToInt32
        /// <summary>
        /// Convert Byte ToInt32, Credit Laurie, VK4AMA
        /// </summary>
        /// <param name="bytes">byte[] bytes</param>
        /// <param name="skip">int number of bytes to skip</param>
        /// <param name="take">int number of bytes to take</param>
        /// <returns>int value</returns>
        public static int ToInt32(byte[] bytes, int skip, int take)
        {
            if (bytes == null || skip + take > bytes.Length || take != 4)
            {
                return 0;
            }

            bytes = bytes.Skip(skip).Take(take).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }
        #endregion

        #region Byte ToDouble
        /// <summary>
        /// Convert Byte ToDouble, Credit Laurie, VK4AMA
        /// </summary>
        /// <param name="bytes">byte[] bytes</param>
        /// <param name="skip">int number of bytes to skip</param>
        /// <param name="take">int number of bytes to take</param>
        /// <returns>double value</returns>
        public static double ToDouble(byte[] bytes, int skip, int take)
        {
            if (bytes == null || skip + take > bytes.Length || take != 8)
            {
                return 0.0;
            }

            bytes = bytes.Skip(skip).Take(take).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToDouble(bytes, 0);
        }
        #endregion

    } // end Class ArrayTools

} // end Namespace JTMonitor.Listener.Utils.ArrayTools
