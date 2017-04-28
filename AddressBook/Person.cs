namespace AddressBook
{
    public class Person : Contact
    {
        public Person(string lastName, string phoneNumber)
            : this(null, lastName, phoneNumber)
        {
        }

        public Person(string firstName, string lastName, string phoneNumber)
            : base(phoneNumber)
        {
            _lastName = lastName;
            _firstName = firstName;
        }

        public override bool Matches(string term)
        {
            return _lastName.ToLower().StartsWith(term.ToLower()) ||
                   _firstName.ToLower().StartsWith(term.ToLower());
        }

        public override string ToString()
        {
            return $"PERSON: {_lastName}, {_firstName} {base.ToString()}";
        }

        private string _lastName;
        private string _firstName;
    }
}
