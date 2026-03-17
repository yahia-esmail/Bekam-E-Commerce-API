namespace Bekam.Application.DTOs.Users;
public class UserSpecParams
{
    private const int MaxPageSize = 20;
    private int pageSize = 10;
    private int pageNumber = 1;

    private string? search;

    public string? Search
    {
        get => search;
        set => search = value?.Trim().ToUpper();
    }

    public bool? IsActive { get; set; }
    public bool? IsLocked { get; set; }

    public int PageSize
    {
        get => pageSize;
        set => pageSize = value <= 0 ? 10 : Math.Min(value, MaxPageSize);
    }

    public int PageNumber
    {
        get => pageNumber;
        set => pageNumber = Math.Max(value, 1);
    }
}