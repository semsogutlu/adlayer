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
                Email = "thisisatest@xxxx.org",
                FirstName = "00003",
                LastName = "00003",
                Password = "Password_1$"
            };

            var newUserInfo = new UserInfo
            {
                Email = "thisisatest_xxxx@xxxx.org",
                FirstName = "00017",
                LastName = "00017",
                Password = "1.xxxxx.1"
            };

            _ldapAuthentication.UpdateLdapAccount(oldUserInfo, newUserInfo);
        }
    }
}
