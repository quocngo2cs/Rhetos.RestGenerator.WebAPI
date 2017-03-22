using System;
using System.Net.Http;
using System.Web;
using System.Web.Routing;

namespace Rhetos.RestGenerator.Utilities
{
    public class WebAPIRestRouteHandler : IRouteHandler
    {
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return new WebAPIRestHttpHandler();
        }
    }

    public class WebAPIRestHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            Console.WriteLine("WebAPI service route.");

            var method = GetHttpMethod(context);
            var requestUri = BuildRequestUri(context);
            var httpRequestMessage = new HttpRequestMessage(method, requestUri);

            foreach (var headerKey in context.Request.Headers.AllKeys)
            {
                var headerValue = context.Request.Headers[headerKey];

                // Try-catch here to bypass the exception when it tries to set Content-* headers to request message.
                // This is a workaround to deal with a problem that AddWithoutValidation is not found
                // when integrating the plugin with Rhetos
                try
                {
                    httpRequestMessage.Headers.Add(headerKey, headerValue);
                }
                catch { /* Do nothing */ }
            }

            if (CanHaveBody(method))
            {
                httpRequestMessage.Content = new StreamContent(context.Request.InputStream);
                httpRequestMessage.Content.Headers.Add("Content-Type", context.Request.ContentType);
                httpRequestMessage.Content.Headers.ContentLength = context.Request.ContentLength;
            }

            using (var httpClient = new HttpClient())
            using (var response = httpClient.SendAsync(httpRequestMessage).Result)
            {
                context.Response.StatusCode = (int)response.StatusCode;

                foreach (var header in response.Headers)
                {
                    foreach (var headerValue in header.Value)
                    {
                        context.Response.AddHeader(header.Key, headerValue);
                    }
                }

                response.Content.CopyToAsync(context.Response.OutputStream);
            }
        }

        private Uri BuildRequestUri(HttpContext context)
        {
            var uriBuilder = new UriBuilder("http://localhost:9100/");
            uriBuilder.Path = context.Request.Path;
            uriBuilder.Query = context.Request.Url.Query;

            return uriBuilder.Uri;
        }

        private static HttpMethod GetHttpMethod(HttpContext context)
        {
            var requestMethod = context.Request.HttpMethod;
            HttpMethod method = HttpMethod.Get;

            if (requestMethod == HttpMethod.Get.Method)
                method = HttpMethod.Get;
            else if (requestMethod == HttpMethod.Head.Method)
                method = HttpMethod.Head;
            else if (requestMethod == HttpMethod.Options.Method)
                method = HttpMethod.Options;
            else if (requestMethod == HttpMethod.Post.Method)
                method = HttpMethod.Post;
            else if (requestMethod == HttpMethod.Put.Method)
                method = HttpMethod.Put;
            else if (requestMethod == HttpMethod.Trace.Method)
                method = HttpMethod.Trace;
            else if (requestMethod == HttpMethod.Delete.Method)
                method = HttpMethod.Delete;

            return method;
        }

        private static bool CanHaveBody(HttpMethod method)
        {
            return method == HttpMethod.Post
                || method == HttpMethod.Delete
                || method == HttpMethod.Put;
        }
    }
}
