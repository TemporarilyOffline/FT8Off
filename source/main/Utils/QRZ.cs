using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace wsjt_message.Listener.Utils
{
    public class QRZ

    /* A QRZ XML Membership is required for use of this listener with */
    {
        static string qrzkey;
        static readonly string qrz_base = "https://xmldata.qrz.com/xml/current/?";
        private static string NewQRZKey()
        {
            string qrz_un = "";
            string qrz_pw = "";
            string qrz_url = qrz_base + "username=" + qrz_un + ";password=" + qrz_pw;
            XElement xml = XElement.Load(qrz_url);
            XNamespace qrzns = "http://xmldata.qrz.com";
            string qkey = xml.Element(qrzns + "Session").Element(qrzns + "Key").Value;
            return qkey;
        }

        public static int DCXXEntity(string callsign)
        {
            if (qrzkey == "" || qrzkey == null)
            {
                qrzkey = NewQRZKey();
            }
            string DXCC;
            string qerror="";
            string qrz_url = qrz_base + "s=" + qrzkey + ";callsign=" + callsign;
            XElement xml = XElement.Load(qrz_url);
            XNamespace qrzns = "http://xmldata.qrz.com";
            if (xml.Element(qrzns + "Session").Element(qrzns + "Error") != null)
            {
                 qerror= xml.Element(qrzns + "Session").Element(qrzns + "Error").Value;
            }
            if (qerror =="Session Timeout" )
            {
                qrzkey = NewQRZKey();
                qrz_url = qrz_base + "s=" + qrzkey + ";callsign=" + callsign;
                xml = XElement.Load(qrz_url);
                qrzns = "http://xmldata.qrz.com";
                DXCC = xml.Element(qrzns + "Callsign").Element(qrzns + "dxcc").Value;
            }
            else if (qerror.StartsWith("Not Found"))
            {
                DXCC = "999";
            }
            else if (xml.Element(qrzns+"Callsign").Element(qrzns+"dxcc") !=null)
            {
                DXCC = xml.Element(qrzns + "Callsign").Element(qrzns + "dxcc").Value;
            }
            else
            {
                DXCC="999";
            }
            Console.WriteLine(DXCC);
            return Int32.Parse(DXCC);
        }

    }
}