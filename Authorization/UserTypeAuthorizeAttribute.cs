using Microsoft.AspNetCore.Authorization;

namespace PainelDTI.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class UserTypeAuthorizeAttribute : AuthorizeAttribute
{
    public UserTypeAuthorizeAttribute(params int[] userTypes)
    {
        if (userTypes.Length == 0)
        {
            throw new ArgumentException("Informe ao menos um userType.", nameof(userTypes));
        }

        Roles = string.Join(',', userTypes.Select(value => value.ToString()));
    }
}
