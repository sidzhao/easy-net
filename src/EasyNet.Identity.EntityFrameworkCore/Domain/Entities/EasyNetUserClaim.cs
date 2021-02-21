using System;
using EasyNet.Data;
using Microsoft.AspNetCore.Identity;

namespace EasyNet.Identity.EntityFrameworkCore.Domain.Entities
{
	public class EasyNetUserClaim : EasyNetUserClaim<int>
	{
	}

    public class EasyNetUserClaim<TUserKey> : IdentityUserClaim<TUserKey>, IEntity<int>
        where TUserKey : IEquatable<TUserKey>
    {
    }
}
