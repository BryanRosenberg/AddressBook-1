using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex(string connectionString)
        {
            _connectionString = connectionString;
            _contacts = new List<Contact>();
            _recipes = new Dictionary<RecipeType, List<Recipe>>();
            
            // Initializations
            _recipes.Add(RecipeType.Appetizers, new List<Recipe>()); //Can add only once
            _recipes[RecipeType.Entrees] = new List<Recipe>(); //Overwrites previous entry
            _recipes.Add(RecipeType.Desserts, new List<Recipe>());
        }

        public void DoStuff()
        {
            // Print a menu
            ShowMenu();
            // Get the user's choice
            MenuOption choice = GetMenuOption();
            
            // while the user does not want to exit
            while (choice != MenuOption.Exit)
            {
                // figure out what they want to do
                // get information
                // do stuff
                switch(choice)
                {
                    case MenuOption.AddPerson:
                        DoAddPerson();
                        break;
                    case MenuOption.AddCompany:
                        DoAddCompany();
                        break;
                    case MenuOption.ListContacts:
                        DoListContacts();
                        break;
                    case MenuOption.SearchContacts:
                        DoSearchContacts();
                        break;
                    case MenuOption.RemoveContact:
                        DoRemoveContact();
                        break;
                    case MenuOption.AddRecipe:
                        DoAddRecipe();
                        break;
                    case MenuOption.ListRecipes:
                        DoListRecipes();
                        break;
                    case MenuOption.SearchEverything:
                        DoSearchEverything();
                        break;
                }
                ShowMenu();
                choice = GetMenuOption();
            }
        }

        private void LoadRecipesFromDbToList()
        {
            _recipes[RecipeType.Appetizers] = new List<Recipe>();
            _recipes[RecipeType.Entrees] = new List<Recipe>();
            _recipes[RecipeType.Desserts] = new List<Recipe>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) // this is an IDosposable
            {
                connection.Open();

                SqlCommand command;

                command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT RecipeType, RecipeTitle 
                      FROM Recipes 
                     ORDER 
                        BY RecipeType, RecipeTitle";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader.GetString(0);
                    string title = reader.GetString(1);

                    RecipeType choice = (RecipeType)Enum.Parse(typeof(RecipeType), type);
                    Recipe recipe = new Recipe(title);
                    List<Recipe> specificRecipes = _recipes[choice]; // running list
                    specificRecipes.Add(recipe);
                }
            }
        }

        private void DoListRecipes()
        {
            Console.Clear();
            Console.WriteLine("RECIPES!");

            using (SqlConnection connection = new SqlConnection(_connectionString)) // this is an IDosposable
            {
                connection.Open();

                SqlCommand command;
                command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT RecipeType, RecipeTitle 
                      FROM Recipes 
                     ORDER 
                        BY RecipeType, RecipeTitle
                ";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader.GetString(0);
                    string title = reader.GetString(1);
                    Console.WriteLine($"{type},{title}");
                }
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to the menu...");
                Console.ReadLine();
            }

        }

        private void DoAddRecipe()
        {
            Console.Clear();
            Console.WriteLine("Please enter your recipe title.");
            string title = GetNonEmptyStringFromUser();
            Recipe recipe = new Recipe(title);

            Console.WriteLine("What kind of recipe is this?");
            for (int i = 0; i < (int)RecipeType.UPPER_LIMIT; i += 1)
            {
                Console.WriteLine($"{i}. {(RecipeType)i}");
            }
            string input = Console.ReadLine();
            int num = int.Parse(input);
            RecipeType choice = (RecipeType)num;

            
            using (SqlConnection connection = new SqlConnection(_connectionString)) // this is an IDosposable
            {
                connection.Open();

                SqlCommand command;
                command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Recipes(RecipeType, RecipeTitle)
                    VALUES(@RecipeChoice,@RecipeTitle)
                ";

                command.Parameters.AddWithValue("@RecipeChoice", choice.ToString());
                command.Parameters.AddWithValue("@RecipeTitle", title);
                command.ExecuteNonQuery();
            }
        }

        //private void LoadAllContactsIntoContactsList()
        //{
        //    _contacts.Clear();

        //    string fileNameC = "company.dat";

        //    using (StreamReader reader = File.OpenText(fileNameC))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string line = reader.ReadLine();
        //            string[] parts = line.Split('|');
        //            _contacts.Add(new Company(parts[0],parts[1]));
        //        }
        //    }

        //    string fileNameP = "person.dat";
        //    using (StreamReader reader = File.OpenText(fileNameP))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string line = reader.ReadLine();
        //            string[] parts = line.Split('|');
        //            _contacts.Add(new Person(parts[0], parts[1], parts[2]));
        //        }
        //    }
        //}

        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            string fileName = "contacts.dat";
            string fileTemp = "temp.dat";
            File.Delete(fileTemp);

            using (StreamReader reader = File.OpenText(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!line.ToLower().Contains(term.ToLower()))
                    {
                        using (StreamWriter writer = new StreamWriter(fileTemp, true))
                        {
                            writer.WriteLine(line);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Remove {line}? (y/n)");
                        string input = Console.ReadLine();
                        if (input.ToLower() == "y")
                        {
                        }
                        else
                        {
                            using (StreamWriter writer = new StreamWriter(fileTemp, true))
                            {
                                writer.WriteLine(line);
                            }
                        }
                        
                    }
                }
            }
            File.Copy(fileTemp, fileName, true);
            File.Delete(fileTemp);
            
            Console.WriteLine("No more contacts found.");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }
           
        private void DoSearchEverything()
        {
            LoadRecipesFromDbToList();

            Console.Clear();
            Console.WriteLine("SEARCH EVERYTHING!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(_contacts);

            // Curtis replaced these with adding data fetched from the Recipes table
            // directly to the matchables list;  I need to remove the lines below and insert the code
            matchables.AddRange(_recipes[RecipeType.Appetizers]);
            matchables.AddRange(_recipes[RecipeType.Entrees]);
            matchables.AddRange(_recipes[RecipeType.Desserts]);

            foreach (IMatchable matcher in matchables)
            {
                if (matcher.Matches(term))
                {
                    Console.WriteLine($"> {matcher}");
                }
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }


        private void DoSearchContacts()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            string fileName = "contacts.dat";

            using (StreamReader reader = File.OpenText(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains(term.ToLower()))
                    {
                        string[] parts = line.Split('|');
                        Console.WriteLine(string.Join("\t", parts));
                    }
                    
                }
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");

            string fileName = "contacts.dat";

            if (File.Exists(fileName))
            {
                using (StreamReader reader = File.OpenText(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split('|');

                        if (parts[0] == "C")
                        {
                            Console.WriteLine(new Company(parts[1], parts[2]));
                        }
                        else
                        if (parts[0] == "P")
                        {
                            Console.WriteLine(new Person(parts[1], parts[2], parts[3]));
                        }
                        else
                        {
                            Console.WriteLine("You have junk in your contacts file.");
                        }
                    }
                }
            }

            TimeSpan interval = new TimeSpan(0, 0, 1);
            Console.WriteLine();
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    Console.Write("Sleep for 5 seconds");
                }
                else
                {
                    Console.Write(" .");
                }
                Thread.Sleep(interval);
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoAddCompany()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the company.");
            Console.Write("Company name: ");
            string name = Console.ReadLine();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();
            
            string fileName = "contacts.dat";
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                //writer.WriteLine(string.Concat("C","|",name,"|",phoneNumber));
                writer.WriteLine(string.Join("|", "C", name, phoneNumber));
            }
        }

        private void DoAddPerson()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the person.");
            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = GetNonEmptyStringFromUser();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            string fileName = "contacts.dat";
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                //writer.WriteLine(string.Concat("P", "|", firstName, "|", lastName, "|", phoneNumber));
                writer.WriteLine(string.Join("|", "P", firstName, lastName, phoneNumber));
            }
        }

        private string GetNonEmptyStringFromUser()
        {
            string input = Console.ReadLine();
            while (input.Length == 0)
            {
                Console.WriteLine("That is not valid.");
                input = Console.ReadLine();
            }
            return input;
        }

        private int GetNumberFromUser()
        {
            while (true)
            {
                try
                {
                    string input = Console.ReadLine();
                    return int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You should type a number.");
                    Console.WriteLine();
                    Console.Write("What would you like to do? ");
                }
                finally
                {
                    //Console.WriteLine("THIS will always be printed!");
                }
            }
        }

        private MenuOption GetMenuOption()
        {
            int choice = GetNumberFromUser();

            while (choice < 0 || choice >= (int)MenuOption.UPPER_LIMIT)
            {
                Console.WriteLine("That is not a valid selection.");
                Console.WriteLine();
                Console.Write("What would you like to do? ");
                choice = GetNumberFromUser();
            }
            
            return (MenuOption)choice;
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"ROLODEX! ({_contacts.Count}) ({_recipes.Count})");
            Console.WriteLine("1. Add a person");
            Console.WriteLine("2. Add a company");
            Console.WriteLine("3. List all contacts");
            Console.WriteLine("4. Search contacts");
            Console.WriteLine("5. Remove a contact");
            Console.WriteLine("-----------------------");
            Console.WriteLine("6. Add a recipe");
            Console.WriteLine("7. List recipes");
            Console.WriteLine("8. Search everything!");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("What would you like to do? ");
        }
    
        private readonly List<Contact> _contacts;
        private Dictionary<RecipeType, List<Recipe>> _recipes;
        private readonly string _connectionString; // readonly = can only be set in constructor
    }
}
