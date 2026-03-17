namespace Bekam.Application.DTOs.Product;
public class ProductSpecParams
{
    private const int MaxPageSize = 15;
    private int pageSize = 15;
    private int _PageNumber = 1;

    private string? search;

    public string? Sort { get; set; }
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public string? Search
    {
        get { return search; }
        set { search = value?.Trim().ToUpper(); }
    }
    public int PageSize
    {
        get { return pageSize; }
        set
        {
            pageSize = value <= 0 ? 15 : Math.Min(value, MaxPageSize);
        }
    }
    public int PageNumber
    {
        get => _PageNumber;
        set => _PageNumber = Math.Max(value, 1);
    }
}
