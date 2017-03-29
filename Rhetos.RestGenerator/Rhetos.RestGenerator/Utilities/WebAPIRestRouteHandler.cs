using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Routing;
using Rhetos.WebApiRestGenerator.Utilities;

namespace Rhetos.WebApiRestGenerator.Utilities
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

            if (CanHaveBody(context))
            {
                SetupContentHeaders(context, httpRequestMessage);
            }

            var cookieContainer = new CookieContainer();
            SetupCookieHeaders(context, httpRequestMessage, cookieContainer);

            using (var httpClient = new HttpClient(new HttpClientHandler() { CookieContainer = cookieContainer }))
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
                response.Content.CopyToAsync(context.Response.OutputStream).Wait();
            }
        }

        private static void SetupCookieHeaders(HttpContext context, HttpRequestMessage httpRequestMessage, CookieContainer cookieContainer)
        {
            foreach (var key in context.Request.Cookies.AllKeys)
            {
                var httpCookie = context.Request.Cookies[key];
                var cookie = new Cookie(httpCookie.Name, httpCookie.Value, "/", httpRequestMessage.RequestUri.DnsSafeHost);
                cookieContainer.Add(cookie);
            }
        }

        private static void SetupContentHeaders(HttpContext context, HttpRequestMessage httpRequestMessage)
        {
            if (context.Request.InputStream != null)
                httpRequestMessage.Content = new StreamContent(context.Request.InputStream);

            httpRequestMessage.Content.Headers.Add("Content-Type", context.Request.ContentType);
            httpRequestMessage.Content.Headers.ContentLength = context.Request.ContentLength;
        }

        private Uri BuildRequestUri(HttpContext context)
        {
            var uriBuilder = new UriBuilder(context.Request.Url);
            uriBuilder.Host = Configurations.WebApiHost;
            uriBuilder.Port = Configurations.WebApiPort;

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

        private static bool CanHaveBody(HttpContext context)
        {
            var method = context.Request.HttpMethod;
            return (method == HttpMethod.Post.Method
                || method == HttpMethod.Delete.Method
                || method == HttpMethod.Put.Method)
                && context.Request.InputStream.Length > 0;
        }
    }
}
