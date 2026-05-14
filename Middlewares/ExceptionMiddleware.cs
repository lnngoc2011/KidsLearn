using System.Net;
using System.Text.Json;
using KidsLearn.Common;

namespace KidsLearn.Middlewares;

// Mục đích: Bắt TẤT CẢ exception chưa được xử lý trong toàn bộ pipeline
// Thay vì mỗi Controller đều phải try/catch
// Đảm bảo mọi lỗi đều trả về JSON chuẩn, không bao giờ trả về HTML error page
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Chuyển request xuống middleware tiếp theo
            await _next(context);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi để debug (không hiển thị cho user)
            _logger.LogError(ex, "Lỗi không mong muốn: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Phân loại exception → trả về StatusCode phù hợp
        var (statusCode, message) = exception switch
        {
            // Lỗi nghiệp vụ — do developer tự throw (404, 400...)
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),

            // Lỗi hệ thống — không lộ chi tiết cho user
            _ => (HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(message);

        // Serialize sang JSON với camelCase để đồng bộ với Frontend
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}