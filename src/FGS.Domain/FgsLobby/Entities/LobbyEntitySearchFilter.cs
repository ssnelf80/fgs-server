using System.Linq.Expressions;
using FGS.Domain.Base;
using FGS.Domain.Services;

namespace FGS.Domain.FgsLobby.Entities;

public class LobbyEntitySearchFilter : BaseSearchFilter<LobbyEntity>
{
    public override Expression<Func<LobbyEntity, bool>> GetWhereExpression()
    {
        PredicateBuilder<LobbyEntity> predicate = new();

        if (string.IsNullOrEmpty(SearchString))
            return predicate.GetExpression();

        if (Guid.TryParse(SearchString, out Guid guid))
            predicate.Or(x => x.Id == guid);

        if (IgnoreCase)
        {
            predicate.Or(x => x.Name.ToLower().Contains(GetLowercaseSearchString()));
        }
        else
        {
            predicate.Or(x => x.Name.Contains(SearchString));
        }
        
        return predicate.GetExpression();
    }
}