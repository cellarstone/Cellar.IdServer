// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Cellar.IdServer.Cellarstone;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the IdentityServer builder
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds test users.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddCellarUsers(this IIdentityServerBuilder builder, IConfigurationRoot config)
        {
            var mongoConnString = config.GetSection("MongoDbRepository:ConnectionString").Value;


            builder.Services.AddSingleton<CellarUserStore>();
            builder.AddProfileService<CellarUserProfileService>();
            builder.AddResourceOwnerValidator<CellarUserResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}