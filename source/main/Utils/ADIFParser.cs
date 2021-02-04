using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ********************************************************
// ADIF field enumeration and scripting custom generated
// for the FT8OFF "Contest" by KD2FMW
// *********************************************************

namespace wsjt_message.Listener.Utils

{
    public static class ADIFParser
    {
        public static string[] ADIF(string adifdata)
        {
             int position = adifdata.IndexOf("<EOH>");//Find where header ends
            string adifrow;
            if (position < 1)
            {
                //Console.WriteLine("Header not present");
                adifrow = adifdata;
            }
            else
            {
                position += 5;//move to start of ADIF row
                adifrow = adifdata[position..];
            }
            //Console.WriteLine($"{position}");

            //Console.WriteLine(adifrow);
            string[] adifout = ReadRecord(adifrow);
            return adifout;

        }

        static string[] ReadRecord(string adifIN)
        {
            string[] adifout = new string[36];
            int eor = adifIN.IndexOf("<EOR>");
            if(eor<0)
            {
                eor=adifIN.IndexOf("<eor>");
            }
            int a, x;
            x = 0;
            for (a = 0; a < eor; a++)
            {
                if (adifIN[a] == '<')
                {
                    string tag_name = "";    //ADIF Tag Name
                    string value = "";       //ADIF Value
                    string len_string = "";  //ADIF Value Length
                    int len;            //ADIF value Length
                    a++;
                    while (adifIN[a] != ':') // Iteration to retrieve ADIF tag
                    {
                        tag_name += adifIN[a]; //append characters to tag_name
                        a++;
                    }
                    a++; //Iterate beyond the ":"
                    while (adifIN[a] != '>' && adifIN[a] != ':') // obtaining the ADIF value length
                    {
                        len_string += adifIN[a];
                        a++;
                    }
                    if (adifIN[a] == ':')
                    {
                        while (adifIN[a] != '>')
                        {
                            a++;
                        }
                    }
                    len = Convert.ToInt32(len_string);
                    adifout[x] = tag_name;
                    x++;
                    while (len > 0)
                    {
                        a++;
                        value += adifIN[a];
                        len--;
                    }
                    //Console.WriteLine($"{tag_name} {value}");

                    adifout[x] = value;
                    x++;
                }

            }
            //testing the array before returning to main
            //foreach (string i in adifout)
            //{
            //    Console.WriteLine(i);
           // }
            return adifout;
        }
    }
}
