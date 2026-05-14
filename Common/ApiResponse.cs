namespace KidsLearn.Common;

// Mục đích: Chuẩn hóa 100% response trả về từ API
// Thay vì mỗi Controller trả về format khác nhau
// Frontend chỉ cần xử lý 1 cấu trúc duy nhất: { success, message, data }
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    // Factory method — tạo response thành công
    public static ApiResponse<T> Ok(T data, string message = "Thành công")
        => new() { Success = true, Message = message, Data = data };

    // Factory method — tạo response thất bại
    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };
}

// Phiên bản không có Data — dùng khi chỉ cần thông báo
// Ví dụ: Xóa thành công, Cập nhật thành công
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Ok(string message = "Thành công")
        => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message)
        => new() { Success = false, Message = message };
}