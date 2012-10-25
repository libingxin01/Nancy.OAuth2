﻿namespace Nancy.OAuth2
{
    using ModelBinding;
    using Security;

    public class AuthorizationModule : NancyModule
    {
        /// <summary>
        /// The base address and the various end-points in the module should be grabbed from an
        /// OAuthConfiguration and should provide sensible default-values that can be overriden
        /// when you enable OAuth in your application.
        /// </summary>
        public AuthorizationModule(
            IAuthorizationEndPointService service, 
            IErrorResponseBuilder errorResponseBuilder) : base(OAuth.Configuration.GetFullPath(x => x.AuthorizationRequestRoute))
        {
            this.RequiresAuthentication();

            Get["/", ctx => OAuth.IsEnabled] = parameters =>
            {
                var request =
                    this.Bind<AuthorizationRequest>();

                // Should a seperate check for AuthorizationRequest.ResponseType == "code" be performed
                // before the request is validated? According to the specification, it always has to
                // be set to "code" for authorization code grant. By checking it here, and returning
                // an error of AuthorizationErrorType.UnsupportedResponseType it could be made sure that
                // the user implementation never miss it?!

                var results =
                    service.ValidateRequest(request, this.Context);

                if (!results.IsValid)
                {
                    return Response.AsErrorResponse(errorResponseBuilder.Build(results.ErrorType, request.State), request.RedirectUrl);
                }

                // Use something more secure than the username
                this.Session[Context.CurrentUser.UserName] = request;
                
                var authorizationView =
                    service.GetAuthorizationView(request, this.Context);

                return View[authorizationView.Item1, authorizationView.Item2];
            };

            Post[OAuth.Configuration.AuthorizationAllowRoute, ctx => OAuth.IsEnabled] = parameters =>
            {

                var token =
                    service.GenerateAuthorizationToken(this.Context);

                var request =
                    this.Session[Context.CurrentUser.UserName] as AuthorizationRequest;

                if (request == null)
                {
                    return HttpStatusCode.InternalServerError;
                }

                var response =
                    new AuthorizationResponse
                    {
                        Code = token,
                        State = request.State
                    };

                // TODO: Perhaps use an UriBuilder instead?
                var url =
                    string.Concat(request.RedirectUrl, response.AsQueryString());

                return Response.AsRedirect(url);
            };

            Post[OAuth.Configuration.AuthorizationDenyRoute, ctx => OAuth.IsEnabled] = parameters =>
            {
                var request =
                    this.Session[Context.CurrentUser.UserName] as AuthorizationRequest;

                return request == null ? 
                    HttpStatusCode.InternalServerError : 
                    Response.AsErrorResponse(errorResponseBuilder.Build(ErrorType.AccessDenied, request.State), request.RedirectUrl);
            };
        }
    }
}