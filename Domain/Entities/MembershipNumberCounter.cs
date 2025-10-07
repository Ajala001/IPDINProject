namespace App.Core.Entities
{
    public class MembershipNumberCounter
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public int LastNumber { get; set; }
    }

}
