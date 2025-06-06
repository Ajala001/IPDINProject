﻿namespace App.Core.Entities
{
    public class Level : Auditables
    {
        public required string Name { get; set; }
        public required int Dues { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
