using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using LdapLayer.Model;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace LdapLayer
{
    /// <summary>
    /// Implementation: http://msdn.microsoft.com/en-us/library/bb344891(v=vs.100).aspx
    /// Active Directory Implementation
    /// </summary>
    public class LdapAuthentication
    {
        private readonly PrincipalContext _rootPrincipal = null;
        public PrincipalContext RootPrincipal
        {
            get { return _rootPrincipal; }
        }

        public LdapAuthentication()
        {
            LdapSettings.refreshLdapSettings();
            var server = LdapSettings.LdapServer1;
            var admin = LdapSettings.LdapAdminUser;
            var pwd = LdapSettings.LdapAdminPassword;
            var container = LdapSettings.LdapContainer;

            try
            {
                _rootPrincipal = new PrincipalContext(ContextType.Domain, server, container, ContextOptions.Negotiate, admin, pwd);
                return;
            }
            catch (PrincipalServerDownException pex)
            {
                server = LdapSettings.LdapServer2;
                //_rootPrincipal = new PrincipalContext(ContextType.Domain, server, container, ContextOptions.Negotiate,admin, pwd);
            }

            try
            {
                _rootPrincipal = new PrincipalContext(ContextType.Domain, server, container, ContextOptions.Negotiate, admin, pwd);
            }
            catch (PrincipalServerDownException pex)
            {
                throw new LdapServerUnavailableException("Both ldap servers are down");
            }
        }

        public AccountStatus CreateNewLdapAccount(UserInfo userInfo, out string errorText, bool pswdPolicyChk = false)
        {
            errorText = string.Empty;
            if (LdapHelper.LdapAccountExists(userInfo, RootPrincipal))
            {
                return AccountStatus.AccountAlreadyExists;
            }

            try
            {
                userInfo.FirstName = LdapHelper.EscapeChars(userInfo.FirstName);
                userInfo.LastName = LdapHelper.EscapeChars(userInfo.LastName);
                var preNewUserInfo = LdapHelper.GetUniqueFirstNameLastName(userInfo, RootPrincipal);
                var newUser = new UserPrincipal(RootPrincipal)
                {
                    SamAccountName = preNewUserInfo.SamName,
                    DisplayName = String.Format("{0} {1}", preNewUserInfo.FirstName, preNewUserInfo.LastName),
                    Surname = preNewUserInfo.LastName,
                    GivenName = preNewUserInfo.FirstName,
                    UserPrincipalName = preNewUserInfo.Email,
                    EmailAddress = preNewUserInfo.Email,
                };

                if (!String.IsNullOrEmpty(userInfo.Password))
                {
                    newUser.Enabled = true;
                    newUser.PasswordNeverExpires = true;
                    newUser.SetPassword(userInfo.Password);
                }
                else
                {
                    newUser.ExpirePasswordNow();
                }
                newUser.Save();
                return AccountStatus.NewAccount;
            }
            catch (Exception ex)
            {
                errorText = String.Format("Exception creating LDAP account for {0} with exception {1}", userInfo.Email, ex.Message);
                return AccountStatus.AccountCreationFailed;
            }
        }

        public string SetLdapAccountPassword(UserInfo userInfo, string passWord)
        {
            var user = LdapHelper.GetLdapUser(userInfo, RootPrincipal);
            try
            {
                if (user != null)
                {
                    user.Enabled = true;
                    user.PasswordNeverExpires = true;
                    user.SetPassword(passWord);
                    user.Save();
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                var error = String.Format("Exception setting password for {0} with exception {1}", userInfo.Email, ex.Message);
                return error;
            }
        }

        public string UpdateLdapAccount(UserInfo oldUserInfo, UserInfo newUserInfo)
        {
            var user = LdapHelper.GetLdapUser(oldUserInfo, RootPrincipal);
            try
            {
                if (user != null)
                {
                    var preNewUserInfo = newUserInfo;
                    preNewUserInfo.SamName = user.SamAccountName;
                    if (newUserInfo.FirstName.ToLower() != user.GivenName.ToLower() || newUserInfo.LastName.ToLower() != user.Surname.ToLower())
                    {
                        preNewUserInfo = LdapHelper.GetUniqueFirstNameLastName(newUserInfo, RootPrincipal);
                    }
                    
                    using (DirectoryEntry entry = (DirectoryEntry) user.GetUnderlyingObject())
                    {
                        entry.InvokeSet("sAMAccountName", preNewUserInfo.SamName);
                        entry.InvokeSet("sn", preNewUserInfo.LastName);
                        entry.InvokeSet("givenName", preNewUserInfo.FirstName);
                        entry.InvokeSet("userPrincipalName", preNewUserInfo.Email);
                        if (!String.IsNullOrEmpty(newUserInfo.Password))
                        {
                            entry.Invoke("SetPassword", new object[] { newUserInfo.Password });
                        }
                        entry.InvokeSet("displayName", preNewUserInfo.SamName);
                        entry.InvokeSet("mail", preNewUserInfo.Email);
                        entry.CommitChanges();
                        entry.Rename("CN=" + preNewUserInfo.SamName);
                        entry.CommitChanges();
                    }

                    //user.SamAccountName = preNewUserInfo.SamName;
                    //user.DisplayName = String.Format("{0} {1}", preNewUserInfo.FirstName, newUserInfo.LastName);
                    //user.Surname = preNewUserInfo.LastName;
                    //user.GivenName = preNewUserInfo.FirstName;
                    //user.UserPrincipalName = preNewUserInfo.Email;
                    //user.EmailAddress = preNewUserInfo.Email;
                    //if (!String.IsNullOrEmpty(newUserInfo.Password))
                    //{
                    //    user.Enabled = true;
                    //    user.PasswordNeverExpires = true;
                    //    user.SetPassword(newUserInfo.Password);
                    //}
                    //user.Save();

                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                var error = String.Format("Exception updating email address for {0} to {1} - {2}", oldUserInfo.Email, newUserInfo.Email, ex.Message);
                return error;
            }
        }

        public void RemoveLdapAccount(UserInfo userInfo)
        {
            var user = LdapHelper.GetLdapUser(userInfo, RootPrincipal);
            if (user != null)
            {
                user.Delete();
            }
        }

        public UserPrincipal GetUser(UserInfo userInfo)
        {
            var user = LdapHelper.GetLdapUser(userInfo, RootPrincipal);
            return user;
        }

        public bool VerifyUserCredentials(UserInfo userInfo, string password)
        {
            var isValidUser = _rootPrincipal.ValidateCredentials(userInfo.Email, password);
            return isValidUser;
        }

        public bool IsUserExists(UserInfo userInfo)
        {
            var user = LdapHelper.GetLdapUser(userInfo, RootPrincipal);
            return user != null;
        }
    }
}
