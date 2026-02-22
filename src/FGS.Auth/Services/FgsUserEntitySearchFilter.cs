using System.Linq.Expressions;
using FGS.Adapters;
using FGS.Auth.Entities;
using FGS.Domain.Base;
using FGS.Domain.Services;

namespace FGS.Auth.Services;

public class FgsUserEntitySearchFilter : BaseSearchFilter<FgsUser>
{
    public override Expression<Func<FgsUser, bool>> GetWhereExpression()
    {
        PredicateBuilder<FgsUser> predicate = new();
        if (string.IsNullOrWhiteSpace(SearchString)) 
            return predicate.GetExpression();
        
        predicate.Or(x => x.Id == SearchString);
        if (IgnoreCase)
        {
            predicate.Or(x 
                => x.UserName != null && x.UserName.ToLower().Contains(GetLowercaseSearchString()));
            predicate.Or(x 
                => x.DisplayName.Contains(GetLowercaseSearchString()));
        }
        else
        {
            predicate.Or(x => x.UserName != null && x.UserName.Contains(SearchString));
            predicate.Or(x => x.DisplayName.Contains(SearchString));
        }

        return predicate.GetExpression();
    }
}