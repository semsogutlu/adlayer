using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using LdapLayer.Model;

namespace LdapLayer
{
    public class LdapHelper
    {
        public static bool LdapAccountExists(UserInfo userInfo, PrincipalContext root)
        {
            var user = GetLdapUser(userInfo, root);
            return user != null;
        }

        public static UserPrincipal GetLdapUser(UserInfo userInfo, PrincipalContext root)
        {
            var user = UserPrincipal.FindByIdentity(root, IdentityType.UserPrincipalName, userInfo.Email);
            return user;
        }

        public static UserInfo GetUniqueFirstNameLastName(UserInfo userInfo, PrincipalContext root)
        {
            //Search if there are already users with same first and last name
            var up = new UserPrincipal(root)
            {
                Surname = string.Format("*{0}*", userInfo.LastName),
                GivenName = string.Format("*{0}*", userInfo.FirstName)
            };

            var safeFirstName = RemoveChars(userInfo.FirstName);
            var safeLastName = RemoveChars(userInfo.LastName);

            var ps = new PrincipalSearcher(up);
            var srcCount = ps.FindAll().Count();

            var firstNameLast = string.Format("{0} {1}", safeFirstName, safeLastName);
            if (srcCount < 1)
            {
                userInfo.SamName = firstNameLast;
                if (firstNameLast.Length > 20)
                {
                    userInfo.SamName = firstNameLast.Substring(0, 20);
                }
            }
            else
            {
                var firstNameLastNameCount = string.Format("{0} {1}{2}", safeFirstName, safeLastName, srcCount);
                if (firstNameLastNameCount.Length > 20)
                {
                    var byHowMuch = firstNameLastNameCount.Length - 20;
                    firstNameLast = firstNameLast.Substring(0, firstNameLast.Length - byHowMuch);
                    userInfo.SamName = string.Format("{0}{1}", firstNameLast, srcCount);
                }
                else
                {
                    userInfo.SamName = firstNameLastNameCount;
                }
                userInfo.LastName = string.Format("{0}{1}", userInfo.LastName, srcCount);
            }

            return userInfo;
        }

        private static readonly char[] _charsToEscape = { '\\', ',', '#', '+', '<', '>', ';', '"', '=' };

        public static string RemoveChars(string str)
        {
            // if you do not remove invalid characters or trailing periods from the SamAccountName, you get this error:
            // A device attached to the system is not functioning.

            var charsToRemove = _charsToEscape;
            var ret = str;
            foreach (var chr in charsToRemove)
            {
                ret = ret.Replace(chr.ToString(), "");
            }

            ret = ret.TrimEnd('.');

            return ret;
        }

        public static string EscapeChars(string str)
        {
            // https://social.technet.microsoft.com/wiki/contents/articles/5312.active-directory-characters-to-escape.aspx
            var ret = str;
            foreach (var chr in _charsToEscape)
            {
                ret = ret.Replace(chr.ToString(), "\\" + chr);
            }

            return ret;
        }
    }
}
