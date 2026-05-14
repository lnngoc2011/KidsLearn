namespace KidsLearn.Common;

// Mục đích: Hỗ trợ phân trang cho Admin
// Khi danh sách User/Quiz/Vocab lớn, không thể trả về tất cả 1 lần
// Frontend gửi: ?page=1&pageSize=10
// Backend trả về: dữ liệu trang hiện tại + thông tin tổng số trang
public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }    // Tổng số bản ghi
    public int PageNumber { get; set; }    // Trang hiện tại
    public int PageSize { get; set; }      // Số item mỗi trang
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    // Static method tạo PaginatedList từ IQueryable
    // Tại sao dùng IQueryable? Vì EF Core sẽ tự sinh SQL OFFSET/FETCH
    // Không load toàn bộ dữ liệu lên RAM rồi mới phân trang
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await Task.FromResult(source.Count());
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}