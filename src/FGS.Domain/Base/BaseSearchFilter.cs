using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace FGS.Domain.Base;

public abstract class BaseSearchFilter<T>
{
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;
    [Range(0, int.MaxValue)]
    public int Limit { get; set; } = int.MaxValue;
   
    public bool IgnoreCase { get; set; } = false;
    public string SearchString { get; set; } = string.Empty;
    protected string GetLowercaseSearchString() => SearchString.ToLower();
    
    public abstract Expression<Func<T, bool>> GetWhereExpression();
}