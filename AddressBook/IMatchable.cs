namespace AddressBook
{
    // Created this Interface so that one could
    // search contacts and recipes at the same time
    public interface IMatchable
    {

        // Can only be public
        bool Matches(string term);
    }
}
