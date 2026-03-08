namespace Miru.Shared.Common;

public class PagingResult<T>
{
    /// <summary>
    /// Total number of items.
    /// </summary>
    public long TotalCount { get; set; }
    
    /// <summary>
    /// Page index.
    /// </summary>
    public int PageIndex { get; set; }
    
    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int CountPerPage { get; set; }

    /// <summary>
    /// Lists of items.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];
}