namespace AddressBook
{
    public abstract class Contact : IMatchable
    {
        public Contact(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public abstract bool Matches(string term);

        public override string ToString()
        {
            return _phoneNumber;
        }

        private string _phoneNumber;
    }
}
