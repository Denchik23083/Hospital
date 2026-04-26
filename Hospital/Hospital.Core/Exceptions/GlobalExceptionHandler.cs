using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hospital.Core.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails();

            switch (exception)
            {
                case ConflictException:
                    httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Title = "Conflict";
                    problemDetails.Detail = exception.Message;
                    break;

                case UnauthorizedException:
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Detail = exception.Message;
                    break;

                case UserNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "User not found";
                    problemDetails.Detail = exception.Message;
                    break;

                case PatientNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Patient not found";
                    problemDetails.Detail = exception.Message;
                    break;

                case DoctorNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Doctor not found";
                    problemDetails.Detail = exception.Message;
                    break;

                case BookingNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Booking not found";
                    problemDetails.Detail = exception.Message;
                    break;

                case DoctorSlotNotFoundException:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Doctor slot not found";
                    problemDetails.Detail = exception.Message;
                    break;

                case SlotAlreadyBookedException:
                    httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Title = "Slot already booked";
                    problemDetails.Detail = exception.Message;
                    break;
                    
                default:
                    _logger.LogError(exception, "Unhandled exception occurred");

                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Server error";
                    problemDetails.Detail = "An unexpected error occurred.";
                    break;
            }

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
