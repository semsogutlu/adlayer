namespace LdapLayer.Model
{
    public enum LdapTaskStatus
    {
        AccountCreationFailed = 0,
        AccountCreatedSuccessfully = 1,
        PasswordResetFailed = 2,
        PasswordResetSuccessful = 3,
        ServerConnectionFailed = 4,
        AccountAlreadyExists = 5,
        AccountDoesnotExist = 6,
        PasswordCannotBeNull = 7,
        UpdatedAccountSuccessfully = 8,
        FailedToUpdateAccount=9
    }
}
