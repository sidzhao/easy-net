using System;
using EasyNet.Data;
using Microsoft.AspNetCore.Identity;

namespace EasyNet.Identity.EntityFrameworkCore.Domain.Entities
{
	public class EasyNetRoleClaim : EasyNetRoleClaim<int>
	{
	}

    public class EasyNetRoleClaim<TRoleKey> : IdentityRoleClaim<TRoleKey>, IEntity<int>
        where TRoleKey : IEquatable<TRoleKey>
    {
    }
}
