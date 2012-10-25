﻿namespace Nancy.OAuth2
{
    using System;
    using System.Collections.Generic;
    using ModelBinding;

    public class AuthorizationRequest
    {
        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string RedirectUrl { get; set; }

        public IEnumerable<string> Scope { get; set; }

        public string State { get; set; }
    }

    public class AuthorizationRequestBinder : IModelBinder
    {
        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <param name="blackList">Blacklisted property names</param>
        /// <param name="instance">Existing instance of the object</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType, object instance = null, params string[] blackList)
        {
            //return new AuthorizationRequest
            //{
            //    RedirectUrl = context.Request.Query["redirect_url"],
            //    ResponseType = context.Request.Query["response_type"],
            //    ClientId = context.Request.Query["client_id"],
            //    Scope = ((string)context.Request.Query["scope"]).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
            //    State = context.Request.Query["state"]
            //};

            return new AuthorizationRequest
            {
                RedirectUrl = "http://nancyfx.org",
                ResponseType = "code",
                ClientId = "4D71889E-89D2-46DB-BC30-60428073B4AA",
                Scope = new [] { "read", "write", "delete"},
                State = "state"
            };
        }

        /// <summary>
        /// Whether the binder can bind to the given model type
        /// </summary>
        /// <param name="modelType">Required model type</param>
        /// <returns>True if binding is possible, false otherwise</returns>
        public bool CanBind(Type modelType)
        {
            return modelType == typeof(AuthorizationRequest);
        }
    }
}