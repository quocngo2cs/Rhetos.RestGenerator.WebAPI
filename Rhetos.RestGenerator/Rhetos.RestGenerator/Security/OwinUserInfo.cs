using Rhetos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Rhetos.Security;
using System.Security.Principal;

namespace Rhetos.RestGenerator.Security
{
    class OwinUserInfo : IUserInfo
    {
        private string _workstation;
        public OwinUserInfo(IWindowsSecurity windowsSecurity)
        {
            _workstation = windowsSecurity.GetClientWorkstation();
        }
        public bool IsUserRecognized
        {
            get
            {
                return (
                    Thread.CurrentPrincipal != null
                    && Thread.CurrentPrincipal.Identity != null
                    && Thread.CurrentPrincipal.Identity.IsAuthenticated);
            }
        }

        public string UserName
        {
            get
            {
                CheckIfUserRecognized();
                return Thread.CurrentPrincipal.Identity.Name;
            }
        }

        public string Workstation
        {
            get
            {
                CheckIfUserRecognized();
                return _workstation;
            }
        }

        public string Report()
        {
            return UserName + "," + Workstation;
        }

        private void CheckIfUserRecognized()
        {
            if (!IsUserRecognized)
                throw new ClientException("User is not authenticated.");
        }
    }
}
