﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace yapp.Infrastructure.Errors
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IStringLocalizer<ErrorHandlingMiddleware> _localizer;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            IStringLocalizer<ErrorHandlingMiddleware> localizer,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger, _localizer);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger logger,
            IStringLocalizer localizer)
        {
            if (exception is RestException re)
            {
                context.Response.StatusCode = (int) re.Code;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(exception.Message))
                {
                    logger.LogError(string.Empty, exception.Message);
                }

                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new
                {
                    errors = localizer[Constants.ErrorHandlingMiddleware.InternalServerError].Value
                });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
