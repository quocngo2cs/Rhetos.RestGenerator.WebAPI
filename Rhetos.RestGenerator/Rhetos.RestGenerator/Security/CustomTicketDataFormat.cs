using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Rhetos.WebApiRestGenerator.Security
{
    public class CustomTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly TicketDataFormat innerDataFormat;

        public CustomTicketDataFormat(IDataProtector protector)
        {
            innerDataFormat = new TicketDataFormat(protector);
        }

        string ISecureDataFormat<AuthenticationTicket>.Protect(AuthenticationTicket data)
        {
            var output = innerDataFormat.Protect(data);
            return output;
        }

        AuthenticationTicket ISecureDataFormat<AuthenticationTicket>.Unprotect(string protectedText)
        {
            FormsAuthenticationTicket ticket;
            try
            {
                ticket = FormsAuthentication.Decrypt(protectedText);
            }
            catch
            {
                return null;
            }

            if (ticket == null)
            {
                return null;
            }
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = ticket.Expiration.ToUniversalTime(),
                IsPersistent = ticket.IsPersistent,
                IssuedUtc = ticket.IssueDate.ToUniversalTime()
            };
            var identities = new[]
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, ticket.Name)
            };

            var identity = new ClaimsIdentity(identities, "ApplicationCookie");

            var authTicket = new AuthenticationTicket(identity, authProperties);

            return authTicket;
        }
    }
}
