// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cellar.IdServer.Cellarstone
{
    /// <summary>
    /// In-memory user object for testing. Not intended for modeling users in production.
    /// </summary>
    public class CellarUser
    {
        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }


        public ICollection<string> Roles {get;set;}

        // /// <summary>
        // /// Gets or sets the provider name.
        // /// </summary>
        // public string ProviderName { get; set; }

        // /// <summary>
        // /// Gets or sets the provider subject identifier.
        // /// </summary>
        // public string ProviderSubjectId { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        public ICollection<Claim> Claims { get; set; } = new HashSet<Claim>(new ClaimComparer());
    }
}