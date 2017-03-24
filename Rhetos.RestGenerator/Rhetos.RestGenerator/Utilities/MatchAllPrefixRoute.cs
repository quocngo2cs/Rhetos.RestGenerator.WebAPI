using System;
using System.Web;
using System.Web.Routing;

namespace Rhetos.WebApiRestGenerator.Utilities
{
    public class MatchAllPrefixRoute : RouteBase
    {
        private readonly Route innerRoute;
        private readonly IRouteHandler routeHandler;
        private readonly Uri basePath;

        public MatchAllPrefixRoute(string pathPrefix, IRouteHandler routeHandler)
        {
            this.basePath = new Uri(pathPrefix, UriKind.RelativeOrAbsolute);
            this.routeHandler = routeHandler;
            this.innerRoute = new Route(pathPrefix, routeHandler);
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext.Request.Url.IsBaseOf(basePath))
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
