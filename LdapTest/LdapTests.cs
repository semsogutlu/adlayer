using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LdapLayer;
using LdapLayer.Model;

namespace LdapTest
{
    [TestClass]
    public class Ldap_Tests
    {
        private LdapAuthentication _ldapAuthentication=new LdapAuthentication();
        
        [TestMethod]
        public void Ldap_Tests_CreateNewAccount()
        {
            var oldUserInfo = new UserInfo
            {
                Email = "thisisatest@tntp.org",
                FirstName = "00002",
                LastName = "00002",
                Password = "Password_1$"
            };

            var newUserInfo = new UserInfo
            {
                Email = "thisisatest@tntp.org",
                FirstName = "00003",
                LastName = "00003",
                Password = "Password_1$"
            };

            string error;
            //var user = _ldapAuthentication.CreateNewLdapAccount(userInfo,out error);
            _ldapAuthentication.UpdateLdapAccount(oldUserInfo, newUserInfo);
        }
    }
}
