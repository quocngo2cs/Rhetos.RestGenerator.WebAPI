using System;
using System.Web;
using System.Web.Routing;

namespace Rhetos.WebApiRestGenerator.Utilities
{
    public class MatchAllPrefixRoute : RouteBase
    {
        private readonly string pathPrefix;
        private readonly Route innerRoute;
        private readonly IRouteHandler routeHandler;

        public MatchAllPrefixRoute(string pathPrefix, IRouteHandler routeHandler)
        {
            this.pathPrefix = pathPrefix;
            this.routeHandler = routeHandler;
            this.innerRoute = new Route(pathPrefix, routeHandler);
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var requestPath = httpContext.Request.Path.TrimStart(new[] { '/' });
            if (requestPath.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return new RouteData(this, this.routeHandler);
            }

            return innerRoute.GetRouteData(httpContext);
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return innerRoute.GetVirtualPath(requestContext, values);
        }
    }
}
