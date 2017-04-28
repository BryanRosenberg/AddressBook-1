namespace AddressBook
{
    public class Company : Contact
    {
        public Company(string name, string phoneNumber)
            : base(phoneNumber)
        {
            _name = name;
        }

        public override bool Matches(string term)
        {
            return _name.ToLower().StartsWith(term.ToLower());
        }

        public override string ToString()
        {
            return $"CMPANY: {_name} {base.ToString()}";
        }

        private string _name;
    }
}
