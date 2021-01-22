using MySql.Data.MySqlClient;
using System;

// ********************************************************
// Database connector and Insert scripting custom generated
// for the FT8OFF "Contest" by KD2FMW
// *********************************************************

namespace wsjt_message.Listener.Utils
{
    public class DataBase
    {
        static string ConnectionString = DBconnection();
        private static MySqlConnection conn;
        private static MySqlCommand cmd;

        public static string ConnectionString1 { get => ConnectionString; set => ConnectionString = value; }

        private static string DBconnection() //MySQL connection String
        {
            string username = "";
            string password = "";
            string server = "";
            string database = "";
            string myConnector = "user=" + username + ";password=" + password + ";server=" + server + ";port=3306;database=" + database;
            return myConnector;
        }
        public static void DBOpen()
        {
            conn = new MySqlConnection(ConnectionString);
            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void DBClose()
        {
            conn.Close();
        }
        public static void DBquery(string Query)
        {
            cmd = new MySqlCommand(Query, conn);
        }
        public static void ADIFinsert(string[] adifout) // Need to seprate the array to field/value pairs
        {
            int len = adifout.Length;
            int a;
            int x = 0;
            string[] colnm = new string[18]; // there are 18 possible fields to be output from WSJT-X
            string[] value = new string[18]; // therefore there are 18 possible values
            for (a = 0; a < len; a++) // a will iterate through the primary array. a will increase twice with each iteration of the loop
            {
                colnm[x] = adifout[a]; //x will iteratre through the second arrays for the proper split. x will iterate once through each loop
                a++;
                value[x] = adifout[a];
                x++;
            }
            //STRING    call
            x = Array.IndexOf(colnm, "call");    //find the position of the field name
            string call = value[x];             //assign the calue for the field
            //STRING    gridsquare
            x = Array.IndexOf(colnm, "gridsquare");
            string gridsquare = value[x];
            //STRING    mode
            x = Array.IndexOf(colnm, "mode");
            string mode = value[x];
            //STRING    rst_sent
            x = Array.IndexOf(colnm, "rst_sent");
            string rst_sent = value[x];
            //STRING    rst_rcvd
            x = Array.IndexOf(colnm, "rst_rcvd");
            string rst_rcvd = value[x];
            //DATE      qso_date
            x = Array.IndexOf(colnm, "qso_date");
            string qso_date = (value[x]);
            //TIME      time_on
            x = Array.IndexOf(colnm, "time_on");
            string time_on = (value[x]);
            //DATE      qso_date_off
            x = Array.IndexOf(colnm, "qso_date_off");
            string qso_date_off = (value[x]);
            //TIME      time_off
            x = Array.IndexOf(colnm, "time_off");
            string time_off = (value[x]);
            //STRING    band
            x = Array.IndexOf(colnm, "band");
            string band = value[x];
            //STRING    freq
            x = Array.IndexOf(colnm, "freq");
            string freq = value[x];
            //STRING    station_callsign
            x = Array.IndexOf(colnm, "station_callsign");
            string station_callsign = value[x];
            //STRING    my_gridsquare
            x = Array.IndexOf(colnm, "my_gridsquare");
            string my_gridsquare = value[x];
            //STRING    tx_pwr
            string tx_pwr = "";
            int qrp = 0;
            x = Array.IndexOf(colnm, "tx_pwr");
            if (x > -1)
            {
                tx_pwr = value[x];
                if (Convert.ToInt32(tx_pwr) >= 1 && Convert.ToInt32(tx_pwr) <= 20) //Calculate the 1 point QRP bonus
                {
                    qrp = 1;
                }
            }
            //STRING    comments
            string comment = "";
            x = Array.IndexOf(colnm, "comment");
            if (x > -1)
            {
                comment = value[x];
            }
            //STRING    name
            string name = "";
            x = Array.IndexOf(colnm, "name");
            if (x > -1)
            {
                name = value[x];
            }
            //STRING    operator_.call
            string operator_call = ""; // the ADIF field name operator is a reserved word
            x = Array.IndexOf(colnm, "operator");
            if (x > -1)
            {
                operator_call = value[x];
            }
            //STRING    propmode
            string propmode = "";
            x = Array.IndexOf(colnm, "propmode");
            if (x > -1)
            {
                propmode = value[x];
            }
            //INT       is_dx
            int is_dx = 0;
            //IN PROGRESS
            //INT       is_event
            int is_event = 0;
            if (call.Length <= 3)
            {
                is_event = 3 + qrp;
            }
            DBinsert(call, gridsquare, mode, rst_sent, rst_rcvd, qso_date, time_on, qso_date_off, time_off, band, freq, station_callsign, my_gridsquare, tx_pwr, comment, name, operator_call, propmode, is_dx, is_event);
        }

        public static void DBinsert(string call, string gridsquare, string mode, string rst_sent, string rst_rcvd, string qso_date, string time_on, string qso_date_off, string time_off, string band, string freq, string station_callsign, string my_gridsquare, string tx_pwr, string comment, string name, string operator_call, string propmode, int is_dx, int is_event)
        {
            //ADIF Log insert Query
            string query = "INSERT INTO ft8log VALUES (default,@call, @gridsquare, @mode, @rst_sent, @rst_rcvd, @qso_date, @time_on, @qso_date_off, @time_off, @band, @freq, @station_callsign, @my_gridsquare, @tx_pwr, @comment, @name, @operator, @propmode, @is_dx, @is_event)";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@call", call);
            cmd.Parameters.AddWithValue("@gridsquare", gridsquare);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@rst_sent", rst_sent);
            cmd.Parameters.AddWithValue("@rst_rcvd", rst_rcvd);
            cmd.Parameters.AddWithValue("@qso_date", qso_date);
            cmd.Parameters.AddWithValue("@time_on", time_on);
            cmd.Parameters.AddWithValue("@qso_date_off", qso_date_off);
            cmd.Parameters.AddWithValue("@time_off", time_off);
            cmd.Parameters.AddWithValue("@band", band);
            cmd.Parameters.AddWithValue("@freq", freq);
            cmd.Parameters.AddWithValue("@station_callsign", station_callsign);
            cmd.Parameters.AddWithValue("@my_gridsquare", my_gridsquare);
            cmd.Parameters.AddWithValue("@tx_pwr", tx_pwr);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@operator", operator_call);
            cmd.Parameters.AddWithValue("@propmode", propmode);
            cmd.Parameters.AddWithValue("@is_dx", is_dx);
            cmd.Parameters.AddWithValue("@is_event", is_event);

            cmd.ExecuteNonQuery();
        }
        public static void DBADIFRaw(string adif)
        {
            string query = "INSERT INTO ft8adif VALUES(@rawadif)";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@rawadif", adif);
            cmd.ExecuteNonQuery();
        }
    }
}

