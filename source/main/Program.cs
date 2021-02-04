using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ********************************************************************
// This program was adapted for use for the FT8OFF "Contest" by KD2FMW.
// The program was adapted from the source code of the JTSDK JTMonitor.
// ********************************************************************

namespace wsjt_message.Listener

    { public class Program

    {
        public static IHostBuilder CreateHostBuilder(string[] args)=>
        Host.CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureServices((HostContext, services)=>
            {
                services.AddHostedService<Worker>();
            });
        #region Class variables
        /// <summary>
        /// Port number of WSJT-X UDP
        /// </summary>
        private const int listenPort = 2238;

        /// <summary>
        /// WSJT-X UDP Client Magic Number
        /// </summary>
        private const string magicNumber = "adbccbda";
        private const string adiftextonly="<call";
        #endregion

        #region Start Listener
        /// <summary>
        /// WSJT-X UPD Listener
        /// </summary>
        private static void StartListener()
        {
            // Setup new client listener on port listenPort
            UdpClient listener = new UdpClient();
            listener.ExclusiveAddressUse = false;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            // Set the socket configuration options
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.ExclusiveAddressUse = false;

            // Bind CLient to the group
            listener.Client.Bind(groupEP);

            // Set arbitrary broadcast IP
            IPAddress multicastaddress = IPAddress.Parse("239.0.0.1");

            // Join the multicast group
            listener.JoinMulticastGroup(multicastaddress);

            // Print the start up header
            Utils.Common.StartUpHeader(listenPort);

            // Start receving messages
            try
            {
                while (true)
                {
                    // Receive message from groupEP
                    int position = 0;
                    Byte[] bytes = listener.Receive(ref groupEP);

                    // Magic Number, fieldLength = 0
                    byte[] message = bytes.Skip(0).Take(4).ToArray();
                    string magic = BitConverter.ToString(message, 0).Replace("-", "").ToLower();

                    // if the magic numbe doesn't match, do nothing.
                    if (magic == magicNumber)
                    {
                        // Schema Version: qint32
                        position += 4; // skip total 4
                        int schema = Utils.ArrayTools.ToInt32(bytes, position, 4);

                        // Message Type: qint32
                        position += 4; // skip total 8
                        int msgKey = Utils.ArrayTools.ToInt32(bytes, position, 4);
                        string msgValue = Utils.Common.GetMessageTypeName(msgKey);

                        // if we don't know what the Message Type is, do nothing
                        if (msgValue != "Unknown")
                        {
                            // Unique ID
                            position += 4; // skip total 12
                            string uniqueKey = Utils.ArrayTools.ToAsciiString(bytes, position, 10);

                            // Get Date UTC Now
                            DateTime utcDateNow = DateTime.UtcNow;
                            string utcDate = utcDateNow.ToString("yyyyMMdd").PadRight(9);

                            switch (msgKey)
                            {
                                case (0):
                                    // nothing needed from this right now.
                                    // Testing - KD2FMW 2021-01-17
                                    // Heartbeat (0)
                                    
                                    //Maximum Schema Number : qint32
                                    position += 10; //skip total 22
                                    int maxschnum = Utils.ArrayTools.ToInt32(bytes, position, 4);
                                    string max_schema_num = maxschnum.ToString().PadRight(4);
                                    // version : utf8
                                    position += 8; //skip total 30
                                    string vers = Utils.ArrayTools.ToAsciiString(bytes, position, 5);
                                    //revision : utf8
                                    position +=9; //skip total 39
                                    string ver = Utils.ArrayTools.ToAsciiString(bytes, position, 6);
                                    //Write the line
                                    //Console.WriteLine($"{max_schema_num}{vers}");
                                    break;
                                case (1):
                                    // Status Message (1)
                                    // All we need from this is Frequency and Mode
                                    // These values are used for logging and screen display
                                    break;
                                case (2):
                                    // isNew : bool
                                    position += 10; // skip total 22
                                    bool isNew = Utils.ArrayTools.ToBoolean(bytes, position, 1);

                                    // (Time) Qtime : qint32
                                    position += 1; // skip total 32
                                    int dtg = Utils.ArrayTools.ToInt32(bytes, position, 4);
                                    string time = Utils.Common.GetTime(dtg).PadRight(5);

                                    // (dB) SNR : qint32
                                    position += 4; // skip total 33
                                    int snr = Utils.ArrayTools.ToInt32(bytes, position, 4);
                                    string dB = snr.ToString().PadLeft(4);

                                    // DT : serialized as double
                                    position += 4; // skip total 37
                                    double dt = Utils.ArrayTools.ToDouble(bytes, position, 8);
                                    string delta = dt.ToString(string.Format("0.0")).PadLeft(5);

                                    // Delta Frequency : qint32
                                    position += 8; // skip total 45
                                    double df = Utils.ArrayTools.ToInt32(bytes, position, 4);
                                    string freq = df.ToString().PadLeft(6);

                                    // Mode : UTF8
                                    // This needs to be fixed
                                    position += 8; // skip total 53
                                    string val = Utils.ArrayTools.ToAsciiString(bytes, position, 4).Trim();
                                    string mode = Utils.Common.GetModeName(val).PadRight(7);

                                    // Message : UTF8
                                    position += 5; //skip total 61
                                    string dmsg = Utils.ArrayTools.ToAsciiString(bytes, position, 20).Trim();

                                    // print the decode line
                                    //Console.WriteLine($"{utcDate}{time}{dB}{delta}{freq}  {mode}{dmsg}");
                                    break;
                                case 5: //QSO Logged
                                    // Date & Time Off QDateTime
                                    // DX call utf8
                                    // DX grid utf8
                                    // Tx frequency(Hz)      quint64
                                    // Mode     utf8
                                    // Report sent utf8
                                    // Report received utf8
                                    // Tx power utf8
                                    // Comments               utf8
                                    // Name                   utf8
                                    // Date &Time On QDateTime
                                    // Operator call utf8
                                    // My call utf8
                                    // My grid utf8
                                    // Exchange sent utf8
                                    // Exchange received utf8
                                    // ADIF Propagation mode  utf8
                                    break;
                                case 12: //ADIF text utf8
                                    position += 15; //skip total 27
                                    int adif_len = bytes.Length - position; //Get total number of bytes for the ADIF portion of the message
                                    string adif = Utils.ArrayTools.ToAsciiString(bytes, position, adif_len);
                                    //Console.WriteLine($"{adif}");
                                    Utils.DataBase.DBOpen();
                                    Utils.DataBase.DBADIFRaw(adif);
                                    string[] adifout = Utils.ADIFParser.ADIF(adif);
                                    Utils.DataBase.ADIFinsert(adifout);
                                    Utils.DataBase.DBClose();
                                    //Console.WriteLine("Contact Logged");
                                    break;
                                default:
                                    break;
                            } // end message switch

                        } // end unknown Message Id

                    } // end if magic number is not "adbccbda"
                    else
                    {
                        message = bytes.Skip(0).Take(bytes.Length).ToArray();
                        string adifraw = BitConverter.ToString(message).Replace("-","");
                        //Console.WriteLine($"{adifraw}");
                        string adifonly = Utils.ArrayTools.ToAsciiString(message, 0, adifraw.Length);
                        if (adifonly.StartsWith(adiftextonly))
                        {

                            Utils.ADIFParser.ADIF(adifonly);
                            //Console.WriteLine("Testing ADIF");
                            Utils.DataBase.DBOpen();
                            string[] adifout = Utils.ADIFParser.ADIF(adifonly);
                            Utils.DataBase.ADIFinsert(adifout);
                            Utils.DataBase.DBADIFRaw(adifonly);
                            Utils.DataBase.DBClose();
                            //Console.WriteLine("Contact Logged");
                        }
                    }

                } // end while loop

            } // end try block
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        } // end Listener
        #endregion

        #region Start Main Method
        /// <summary>
        /// Start Main Method
        /// </summary>
        /// <param name="args"></param>
        

        
        private static void Main(string[] args)
        {
            StartListener();
        } // end - Main Method
        #endregion

    } // end Program

} // end - Namspace wsjt_message.Listener.Program
