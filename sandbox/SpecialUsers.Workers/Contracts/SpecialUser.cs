namespace SpecialUsers.Workers.Contracts
{
    public record SpecialUser(string Name, SpecialUser.OriginEnum Origin)
    {
        public enum OriginEnum
        {
            Wiki,
            Forum
        }
    }
}
