// ============================================================
// ValidationFilter.cs — Custom Action Filter
// ============================================================
// Action Filter beroperasi setelah routing MVC selesai.
// Kita dapat memotong request sebelum dieksekusi controller
// untuk memeriksa validitas model state secara global.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TesBackendNet.Framework.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Jika model state (misal anotasi DTO) tidak valid, bypass controller
        // dan langsung kembalikan HTTP 400 Bad Request
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new
            {
                Success = false,
                Message = "Model validation failed via Filter.",
                Errors = errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Dijalankan SETELAH action method di controller selesai dieksekusi
    }
}
