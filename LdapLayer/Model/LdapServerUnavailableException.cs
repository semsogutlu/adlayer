using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LdapLayer.Model
{
    public class LdapServerUnavailableException : ApplicationException
    {
        public LdapServerUnavailableException()
            : base("A failure occurred communicating with the ldap server.")
        {
        }

        public LdapServerUnavailableException(string msg)
            : base("A failure occurred communicating with the ldap server: " + msg)
        {
        }

        public LdapServerUnavailableException(string msg, Exception e)
            : base("A failure occurred communicating with the ldap server: " + msg, e)
        {

        }
    }
}
