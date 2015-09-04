using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LdapLayer.Model
{
    public enum AccountStatus
    {
        NewAccount = 0,
        AccountAlreadyExists = 1,
        AccountCreationFailed = 2
    }
}
