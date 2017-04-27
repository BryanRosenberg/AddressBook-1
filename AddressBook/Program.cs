using System;
using System.Configuration;

// inside Roledex.cs, 
// make recipes only go into the datbase.
// forget the search everything.
// - add recipes
// - list recipes

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
