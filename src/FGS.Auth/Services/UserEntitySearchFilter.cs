using FGS.Common.SearchFilter;

namespace FGS.Auth.Services;

public class UserEntitySearchFilter : BaseSearchFilter
{
    public UserEntitySearchFilter()
    {
        IgnoreCase = true;
        Limit = 20;
    }
}