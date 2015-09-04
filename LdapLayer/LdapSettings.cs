using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LdapLayer
{
    public class LdapSettings
    {
        public static void refreshLdapSettings()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }


        public static string LdapServer1
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapServer1"];
            }
        }

        public static string LdapServer2
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapServer2"];
            }
        }

        public static string LdapAdminUser
        {
            get
            {
                var val = System.Configuration.ConfigurationManager.AppSettings["LdapAdminUser"];
                return val;
            }
        }

        public static string LdapAdminPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapAdminPassword"];
            }
        }

        public static string LdapContainer
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapContainer"];
            }
        }


        // below are ldap load balancer settings, currently the ldap load balancer is not in user, it may be revisited in the future


        public static string LdapServer
        {
            get
            {
                var val = System.Configuration.ConfigurationManager.AppSettings["LdapLoadBalancer"];
                return val;
            }
        }

        public static string LdapServerAdminUser
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapLoadBalAdminUser"];
            }
        }

        public static string LdapServerAdminPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapLoadBalAdminPassword"];
            }
        }

        public static string LdapProdContainerPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LdapLoadBalContainer"];
            }
        }



    }
}
