// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Validation;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using System;

namespace Cellar.IdServer.Cellarstone
{
    /// <summary>
    /// Resource owner password validator for test users
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class CellarUserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IdentityServerOptions _options;
        private readonly CellarUserStore _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestUserResourceOwnerPasswordValidator"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="users">The users.</param>
        public CellarUserResourceOwnerPasswordValidator(IdentityServerOptions options, CellarUserStore users)
        {
            _options = options;
            _users = users;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_users.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _users.FindByEmail(context.UserName);
                context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password, DateTime.Now, user.Claims);
            }

            return Task.FromResult(0);
        }
    }
}