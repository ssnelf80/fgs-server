using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace FGS.Common.SearchFilter;

public abstract class BaseSearchFilter<T>
{
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;
    [Range(0, int.MaxValue)]
    public int Limit { get; set; } = int.MaxValue;
   
    public bool IgnoreCase { get; set; } = false;
    public string SearchString { get; set; } = string.Empty;
    public string GetLowercaseSearchString() => SearchString.ToLower();
    
    public abstract Expression<Func<T, bool>> GetWhereExpression();
}