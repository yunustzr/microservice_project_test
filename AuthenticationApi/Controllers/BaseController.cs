using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AuthenticationApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Problem400(string title, string detail) =>
            BadRequest(new ProblemDetails
            {
                Title    = title,
                Detail   = detail,
                Status   = StatusCodes.Status400BadRequest,
                Instance = Request.Path
            });

        protected IActionResult ValidationProblem() =>
            BadRequest(new ValidationProblemDetails(ModelState)
            {
                Title    = "Validation Errors",
                Status   = StatusCodes.Status400BadRequest,
                Instance = Request.Path
            });

        protected IActionResult Problem401(string title, string detail) =>
            Unauthorized(new ProblemDetails
            {
                Title    = title,
                Detail   = detail,
                Status   = StatusCodes.Status401Unauthorized,
                Instance = Request.Path
            });


        protected IActionResult ValidationProblemModel() => BadRequest(new ValidationProblemDetails(ModelState)
        {
            Title    = "Validation Errors",
            Status   = StatusCodes.Status400BadRequest,
            Instance = Request.Path
        });


        protected IActionResult Problem500(string title, string detail) => StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
        {
            Title    = title,
            Detail   = detail,
            Status   = StatusCodes.Status500InternalServerError,
            Instance = Request.Path
        });


    }
}

