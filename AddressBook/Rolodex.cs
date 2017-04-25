using System;
using System.Collections.Generic;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex()
        {
            _contacts = new List<Contact>();
            _recipes = new List<Recipe>();
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
                    case MenuOption.SearchEverything:
                        DoSearchEverything();
                        break;
                }
                ShowMenu();
                choice = GetMenuOption();
            }
        }

        private void DoAddRecipe()
        {
            Console.Clear();
            Console.WriteLine("Please enter your recipe title.");
            string title = GetNonEmptyStringFromUser();
            Recipe recipe = new Recipe(title);
            _recipes.Add(new Recipe(title));
        }

        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.Write($"Remove {contact}? (y/N)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        _contacts.Remove(contact);
                        return;
                    }
                }
            }

            Console.WriteLine("No more contacts found.");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoSearchEverything()
        {
            Console.Clear();
            Console.WriteLine("SEARCH EVERYTHING!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(_contacts);
            matchables.AddRange(_recipes);

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

            foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.WriteLine($"> {contact}");
                }
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");
            
            foreach (Contact contact in _contacts)
            {
                Console.WriteLine($"> {contact}");
            }

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

            _contacts.Add(new Company(name, phoneNumber));
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

            _contacts.Add(new Person(firstName, lastName, phoneNumber));
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

        private MenuOption GetMenuOption()
        {
            string input = Console.ReadLine();
            int choice = int.Parse(input);

            while (choice < 0 || choice >= (int)MenuOption.UPPER_LIMIT)
            {
                Console.WriteLine("That is not valid.");
                input = Console.ReadLine();
                choice = int.Parse(input);
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
            Console.WriteLine("7. Search everything!");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("What would you like to do? ");
        }
    
        private List<Contact> _contacts;
        private List<Recipe> _recipes;
    }
}
