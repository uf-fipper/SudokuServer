using Microsoft.AspNetCore.Mvc;

namespace SudokuServer.Models.Vo;

public class BaseVo(string code, string message) : IResult, IActionResult
{
    public string Code { get; set; } = code;

    public string Message { get; set; } = message;

    public static BaseVo<T> Success<T>(T data) => new(data);

    public static BaseVo Fail(string code, string message) => new(code, message);

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return httpContext.Response.WriteAsJsonAsync(this);
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        return context.HttpContext.Response.WriteAsJsonAsync(this);
    }
}

public class BaseVo<T>(T data) : BaseVo("0", "success")
{
    public T Data { get; set; } = data;
}
