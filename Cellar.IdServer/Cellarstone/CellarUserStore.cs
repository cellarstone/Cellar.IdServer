// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Cellar.IdServer.Cellarstone
{
    /// <summary>
    /// Store for users
    /// </summary>
    public class CellarUserStore
    {
        //public string ConnectionString { get; set; } = "";
        public string DatabaseName { get; set; } = "TestIdentityMongo";
        public bool IsSSL { get; set; } = false;

        private IMongoDatabase _database { get; }

        private readonly ILogger _logger;

        public CellarUserStore(
            ILogger<CellarUserStore> logger)
        {
            //this.ConnectionString = connString;
            _logger = logger;

            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb://mongo-0.mongo,mongo-1.mongo,mongo-2.mongo:27017/"));
                if (IsSSL)
                {
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                }
                var mongoClient = new MongoClient(settings);
                _database = mongoClient.GetDatabase(DatabaseName);



                //UNIKATNI POLE
                // var options = new CreateIndexOptions() { Unique = true };
                // var field = new StringFieldDefinition<Category>("Name");
                // var indexDefinition = new IndexKeysDefinitionBuilder<Category>().Ascending(field);
                // _database.GetCollection<Category>("Categories").Indexes.CreateOne(indexDefinition, options);


                // CellarUser asdf = new CellarUser();
                // asdf.Email = "user@user.com";
                // asdf.Password = "password";

                // Users.InsertOne(asdf);

            }
            catch (Exception ex)
            {
                LogException(ex);
                throw new Exception("Can not access to db server.", ex);
            }
        }

        public IMongoCollection<CellarUser> Users
        {
            get
            {
                return _database.GetCollection<CellarUser>("Users");
            }
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool ValidateCredentials(string email, string password)
        {
            var user = FindByEmail(email);
            if (user != null)
            {
                return user.Password.Equals(password);
            }

            return false;
        }

        /// <summary>
        /// Finds the user by subject identifier.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public CellarUser FindById(string id)
        {
            return Users.Find(x => x.Id.ToString() == id).FirstOrDefault();
        }

        /// <summary>
        /// Finds the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public CellarUser FindByEmail(string email)
        {
            var res = Users.Find(u => u.Email == email).FirstOrDefault();

            return res;
        }

        /// <summary>
        /// Finds the user by external provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public CellarUser FindByExternalProvider(string provider, string userId)
        {
            return null;
            // return _users.FirstOrDefault(x =>
            //     x.ProviderName == provider &&
            //     x.ProviderSubjectId == userId);
        }

        /// <summary>
        /// Automatically provisions a user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public CellarUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId();

            // check if a display name is available, otherwise fallback to subject id
            var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

            // create new user
            var user = new CellarUser
            {
                Email = name,
                // ProviderName = provider,
                // ProviderSubjectId = userId,
                Claims = filtered
            };

            // add user to in-memory store
            Users.InsertOne(user);

            return user;
        }




        /// </// <summary>
        /// HELPER return and log exception
        /// </summary>
        public void LogException(Exception exception)
        {
            Guid errNo = Guid.NewGuid();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(errNo.ToString());
            sb.AppendLine(exception.Message);
            if (exception.InnerException != null)
                sb.AppendLine(exception.InnerException.Message);
            sb.AppendLine(exception.StackTrace);

            _logger.LogCritical(sb.ToString());
        }
    }
}