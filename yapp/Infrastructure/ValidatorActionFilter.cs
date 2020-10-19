using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace yapp.Infrastructure
{
    public class ValidatorActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ModelState.IsValid)
            {
                return;
            }

            var result = new ContentResult();
            var errors = new Dictionary<string, string[]>();

            foreach (var valuePair in filterContext.ModelState)
            {
                errors.Add(valuePair.Key, valuePair.Value.Errors.Select(x => x.ErrorMessage).ToArray());
            }

            // todo: use special model for errors
            var content = JsonConvert.SerializeObject(new {errors});
            result.Content = content;
            // todo: can be replaced with JSON enum
            result.ContentType = "application/json";

            // no default enum for this
            filterContext.HttpContext.Response.StatusCode = 422; // unprocessable entity
            filterContext.Result = result;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
