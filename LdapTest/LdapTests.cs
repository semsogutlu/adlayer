using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LdapLayer;
using LdapLayer.Model;

namespace LdapTest
{
    [TestClass]
    public class Ldap_Tests
    {
        public ServerInfo ServerInfo
        {
            get;
            set;
        }

        [TestInitialize]
        public void Initialize()
        {
            ServerInfo = new ServerInfo
            {
                Server = "XXX",
                Admin = "XXX",
                Password = "XXX",
                ServerContainer = "OU=users ..."
            };
        }

        [TestMethod]
        public void Ldap_Tests_CreateNewAccount()
        {
            var userInfo = new UserInfo
            {
                Email = String.Format("{0}@test.org", Guid.NewGuid().ToString()),
                LastName = "Smith",
                FirstName = "Sophia",
                Password = "Password_1$"
            };

            //Create account
            var ldap = new Ldap(ServerInfo);
            ldap.CreateNewLdapAccount(userInfo);
            
            //Get user
            var queryForUser = ldap.GetUser(userInfo.Email);
            Assert.IsTrue(queryForUser != null);
            Assert.IsTrue(queryForUser.FirstName == userInfo.FirstName);
            Assert.IsTrue(queryForUser.LastName == userInfo.LastName);
            Assert.IsTrue(queryForUser.Email == userInfo.Email);
        }

        [TestMethod]
        public void Ldap_Tests_CreateNewAccount_ExceptionHandling()
        {
            ServerInfo.Server = "1.1.1.1.1";

            var userInfo = new UserInfo
            {
                Email = String.Format("{0}@test.org", Guid.NewGuid().ToString()),
                LastName = "Smith",
                FirstName = "Sophia",
                Password = "Password_1$"
            };

            //Create account
            var ldap = new Ldap(ServerInfo);
            ldap.CreateNewLdapAccount(userInfo);

            //Get user
            var queryForUser = ldap.GetUser(userInfo.Email);
            Assert.IsTrue(queryForUser == null);
            Assert.IsTrue(userInfo.TaskFailed==true);
            Assert.IsTrue(userInfo.LdapTaskStatus==LdapTaskStatus.ServerConnectionFailed);
        }

        [TestMethod]
        public void Ldap_Tests_SetPassword()
        {
            var userInfo = new UserInfo
            {
                Email = String.Format("{0}@test.org", Guid.NewGuid().ToString()),
                LastName = "Smith",
                FirstName = "Sophia",
                Password = "Password_1$"
            };

            //Create account
            var ldap = new Ldap(ServerInfo);
            ldap.CreateNewLdapAccount(userInfo);
            ldap.SetLdapAccountPassword(userInfo);

            //Verify
            Assert.IsTrue(userInfo.TaskFailed == false);
            Assert.IsTrue(userInfo.LdapTaskStatus == LdapTaskStatus.PasswordResetSuccessful);
            Assert.IsTrue(ldap.VerifyUser(userInfo));
        }
    }
}
