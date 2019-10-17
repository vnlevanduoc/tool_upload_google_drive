using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib_google_drive;

namespace zitzac_tool_upload
{
    class Program
    {
        static void Main(string[] args)
        {
            google_drive drive = new google_drive(@"D:\Source\HBCD_PE_x64.iso");

            Console.WriteLine("Google Drive API Sample");

            try
            {
               drive.RunUpload().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();  
            Console.Read();
        }
    }
}
