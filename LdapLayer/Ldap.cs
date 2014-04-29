using System;
using System.DirectoryServices.AccountManagement;
using LdapLayer.Model;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace LdapLayer
{
    /// <summary>
    /// Using UserPrincipalName is used for creating accounts
    /// UserPrincipalName is email address
    /// </summary>
    public class Ldap
    {
        private readonly PrincipalContext _rootPrincipal = null;
        private readonly bool _connected = false;
        public Ldap(ServerInfo serverInfo)
        {
            try
            {
                _rootPrincipal = new PrincipalContext(ContextType.Domain, 
                    serverInfo.Server, 
                    serverInfo.ServerContainer, 
                    ContextOptions.Negotiate, 
                    serverInfo.Admin, 
                    serverInfo.Password);
                _connected = true;
            }
            catch 
            {
                _connected = false;
            }
        }

        public UserInfo CreateNewLdapAccount(UserInfo userInfo)
        {
            //If server down, return error
            if (!_connected )
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.ServerConnectionFailed;
                return userInfo;
            }

            //If account already exists, return error
            if (LdapHelper.LdapAccountExists(userInfo, _rootPrincipal))
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.AccountAlreadyExists;
                return userInfo;
            }

            //Try creating new account
            try
            {
                var preNewUserInfo = LdapHelper.GetUniqueFirstNameLastName(userInfo, _rootPrincipal);
                var newUser = new UserPrincipal(_rootPrincipal)
                {
                    SamAccountName = preNewUserInfo.SamName,
                    DisplayName = String.Format("{0} {1}", preNewUserInfo.FirstName, preNewUserInfo.LastName),
                    Surname = preNewUserInfo.LastName,
                    GivenName = preNewUserInfo.FirstName,
                    UserPrincipalName = preNewUserInfo.Email,
                    EmailAddress = preNewUserInfo.Email,
                };
                newUser.ExpirePasswordNow();
                newUser.Save();
                userInfo.LdapTaskStatus = LdapTaskStatus.AccountCreatedSuccessfully;                
            }
            catch (Exception ex)
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.AccountCreationFailed;
                userInfo.Exception = ex;
            }

            return userInfo;
        }

        public UserInfo SetLdapAccountPassword(UserInfo userInfo)
        {
            //If server down, return error
            if (!_connected)
            {
                SetFailedConnectedInfo(userInfo);
                return userInfo;
            }

            //If account does not exist, return error
            var user = LdapHelper.GetLdapUser(userInfo, _rootPrincipal);
            if (user==null)
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.AccountDoesnotExist;
                return userInfo;
            }

            //If password is null
            if (String.IsNullOrEmpty(userInfo.Password))
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.PasswordCannotBeNull;
                return userInfo;
            }

            try
            {
                user.Enabled = true;
                user.PasswordNeverExpires = true;
                user.SetPassword(userInfo.Password);
                user.Save();
                userInfo.LdapTaskStatus=LdapTaskStatus.PasswordResetSuccessful;
            }
            catch (Exception ex)
            {
                userInfo.TaskFailed = true;
                userInfo.LdapTaskStatus = LdapTaskStatus.PasswordResetFailed;
                userInfo.Exception = ex;
            }

            return userInfo;
        }

        //public UserInfo UpdateLdapAccount(UserInfo oldUserInfo, UserInfo newUserInfo)
        //{
        //    //If server down, return error
        //    if (!_connected)
        //    {
        //        SetFailedConnectedInfo(oldUserInfo);
        //        return oldUserInfo;
        //    }

        //    //If account does not exist, return error
        //    var user = LdapHelper.GetLdapUser(oldUserInfo, _rootPrincipal);
        //    if (user == null)
        //    {
        //        oldUserInfo.TaskFailed = true;
        //        oldUserInfo.LdapTaskStatus = LdapTaskStatus.AccountDoesnotExist;
        //        return oldUserInfo;
        //    }

        //    var preNewUserInfo = newUserInfo;
        //    try
        //    {
        //        preNewUserInfo.SamName = user.SamAccountName;
        //        if (newUserInfo.FirstName.ToLower() != user.GivenName.ToLower() || newUserInfo.LastName.ToLower() != user.Surname.ToLower())
        //        {
        //            preNewUserInfo = LdapHelper.GetUniqueFirstNameLastName(newUserInfo, _rootPrincipal);
        //        }
        //        user.SamAccountName = preNewUserInfo.SamName;
        //        user.DisplayName = String.Format("{0} {1}", preNewUserInfo.FirstName, newUserInfo.LastName);
        //        user.Surname = preNewUserInfo.LastName;
        //        user.GivenName = preNewUserInfo.FirstName;
        //        user.UserPrincipalName = preNewUserInfo.Email;
        //        user.EmailAddress = preNewUserInfo.Email;
        //        user.Save();

        //        preNewUserInfo.LdapTaskStatus = LdapTaskStatus.UpdatedAccountSuccessfully;
        //    }
        //    catch (Exception ex)
        //    {
        //        preNewUserInfo.TaskFailed = true;
        //        preNewUserInfo.LdapTaskStatus = LdapTaskStatus.FailedToUpdateAccount;
        //        preNewUserInfo.Exception = ex;
        //    }

        //    return preNewUserInfo;
        //}

        public UserInfo GetUser(string userPrincipalName)
        {
            var user = LdapHelper.GetLdapUser(userPrincipalName, _rootPrincipal);
            if (user == null)
            {
                return null;
            }

            var userInfo = new UserInfo
            {
                FirstName=user.GivenName,
                LastName=user.Surname,
                Email=user.UserPrincipalName,
            };

            return userInfo;
        }

        public bool VerifyUser(UserInfo userInfo)
        {
            if (_connected == false)
            {
                return false;
            }

            try
            {
                var user = _rootPrincipal.ValidateCredentials(userInfo.Email, userInfo.Password);
                return user;
            }
            catch
            {
                return false;
            }
        }

        private void SetFailedConnectedInfo(UserInfo userInfo)
        {
            userInfo.TaskFailed = true;
            userInfo.LdapTaskStatus = LdapTaskStatus.ServerConnectionFailed;
        }
    }
}
