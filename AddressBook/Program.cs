using System;
using System.Configuration;

namespace AddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = ConfigurationManager.AppSettings["ApplicationName"];
            Console.WriteLine("Welcome to: ");
            Console.WriteLine(name);
            Console.WriteLine(new string('-', Console.WindowWidth - 4));
            Console.WriteLine();
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

            Rolodex rolodex = new Rolodex();
            rolodex.DoStuff();
        }
    }
}
