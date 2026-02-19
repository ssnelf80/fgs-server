using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FGS.Common.SearchFilter;

public abstract class BaseSearchFilter
{
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;
    [Range(0, int.MaxValue)]
    public int Limit { get; set; } = int.MaxValue;
   
    public bool IgnoreCase { get; set; } = false;
    public string SearchString { get; set; } = string.Empty;
}