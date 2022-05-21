// See https://aka.ms/new-console-template for more information
using OS_Project;
using System;
using System.Reflection;//reflection Assembly.GetExecutingAssembly().Location
using System.IO;//directory//whrite in file
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OS_Project
{
    class Program
    {
        public static directory current = new directory();
        public static string curpath = "";
        //Change to our Vallues Controller
        public static void Main(string[] args)
        {
            Virual_Disk.intialize();
            string cmd = "", arg = "";
            Virual_Disk.get_block(6);
            while (true)
            {
                Console.Write(Commands.getpath() + '>');
                string orig_str = Console.ReadLine();
                string mudif_string = orig_str.ToLower().TrimStart().TrimEnd();
                string[] inp = Commands.input(mudif_string);
                Commands.command(inp, orig_str);
            }

            Console.ReadKey();
        }
    }
}
