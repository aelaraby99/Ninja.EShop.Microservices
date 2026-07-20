using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace NINJA.EShop.Shared.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest,TResponse>> logger)
        : IPipelineBehavior<TRequest,TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request,RequestHandlerDelegate<TResponse> next,CancellationToken cancellationToken)
        {
            logger.LogInformation("[Start] Handle Request = {Request} - Response = {Response} - RequestData = {RequestData}",
                typeof(TRequest).Name,typeof(TResponse).Name,request);
            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
                logger.LogWarning("[PERFORMANCE] Handle Request = {Request} - Response = {Response} - RequestData = {RequestData} - TimeTaken = {TimeTaken} Seconds",
                    typeof(TRequest).Name,typeof(TResponse).Name,request,timeTaken.Seconds);
            logger.LogInformation("[End] Handled Request = {Request} - Response = {Response} - RequestData = {RequestData} - TimeTaken = {TimeTaken} Seconds",
                typeof(TRequest).Name,typeof(TResponse).Name,request,timeTaken.Seconds);
            return response;
        }
    }
}