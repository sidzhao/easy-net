﻿using System;
using EasyNet.Data;
using Microsoft.AspNetCore.Identity;

namespace EasyNet.Identity.EntityFrameworkCore.Domain.Entities
{
	public class EasyNetUserLogin : EasyNetUserLogin<int>
	{
	}

    public class EasyNetUserLogin<TUserKey> : IdentityUserLogin<TUserKey>, IEntity<int>
       where TUserKey : IEquatable<TUserKey>
    {
        public int Id { get; set; }
    }
}
