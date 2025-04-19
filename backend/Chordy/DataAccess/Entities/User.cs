﻿namespace Chordy.DataAccess.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
    }
}
