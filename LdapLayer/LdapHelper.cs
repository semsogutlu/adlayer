using System.DirectoryServices.AccountManagement;
using System.Linq;
using LdapLayer.Model;

namespace LdapLayer
{
    public class LdapHelper
    {
        public static UserInfo GetUniqueFirstNameLastName(UserInfo userInfo, PrincipalContext root)
        {
            //Search if there are already users with same first and last name
            var up = new UserPrincipal(root)
            {
                Surname =  string.Format("*{0}*",userInfo.LastName),
                GivenName = string.Format("*{0}*", userInfo.FirstName)
            };

            var ps = new PrincipalSearcher(up);
            var srcCount = ps.FindAll().Count();

            var firstNameLast = string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName);
            if (srcCount < 1)
            {
                //No one contains the same first and last name
                userInfo.SamName = firstNameLast;
                if (firstNameLast.Length > 20)
                {
                    //If SamName length is greater than 20, Substring it.
                    //Since no one contains same first and lastname , SamName will be unique
                    userInfo.SamName = firstNameLast.Substring(0, 20);
                }
            }
            else
            {
                //Some accounts already contain the same first and last name
                //Create a name combo that is unique by adding the srcCount
                var firstNameLastNameCount = string.Format("{0} {1}{2}", userInfo.FirstName, userInfo.LastName, srcCount);
                if (firstNameLastNameCount.Length > 20)
                {
                    //Oops the name combo length is greater than 20, so Substring it to 20
                    //However, retain the srcCount so even after Substring it will be unique
                    var byHowMuch = firstNameLastNameCount.Length - 20;
                    firstNameLast = firstNameLast.Substring(0, firstNameLast.Length - byHowMuch);
                    userInfo.SamName = string.Format("{0}{1}", firstNameLast, srcCount);
                }
                else
                {
                    //Great, assign the SamName directly
                    userInfo.SamName = firstNameLastNameCount;
                }
                userInfo.LastName = string.Format("{0}{1}", userInfo.LastName, srcCount);
            }

            return userInfo;
        }

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

        public static UserPrincipal GetLdapUser(string userPrincipalName, PrincipalContext root)
        {
            try
            {
                var user = UserPrincipal.FindByIdentity(root, IdentityType.UserPrincipalName, userPrincipalName);
                return user;
            }
            catch
            {
                return null;
            }           
        }
    }
}
