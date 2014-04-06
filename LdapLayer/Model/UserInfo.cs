using System;
namespace LdapLayer.Model
{
    public class UserInfo
    {
        /// <summary>
        /// Required when creating new account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Required when creating new account
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Required when creating new account
        /// </summary>
        public string Email { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// This value is set when returned 
        /// </summary>
        public string SamName { get; set; }
        /// <summary>
        /// This value is set when returned 
        /// </summary>
        public bool TaskFailed { get; set; }
        /// <summary>
        /// This value is set when returned 
        /// </summary>
        public LdapTaskStatus LdapTaskStatus { get; set; }
        /// <summary>
        /// This value is set when returned 
        /// </summary>
        public Exception Exception { get; set; }
    }
}
