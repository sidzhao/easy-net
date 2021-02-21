using System;
using EasyNet.Data;
using Microsoft.AspNetCore.Identity;

namespace EasyNet.Identity.EntityFrameworkCore.Domain.Entities
{
    public class EasyNetUser : EasyNetUser<int>
    {
    }

    public class EasyNetUser<TPrimaryKey> : IdentityUser<TPrimaryKey>, IEntity<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        public bool IsAdmin { get; set; }
    }
}
