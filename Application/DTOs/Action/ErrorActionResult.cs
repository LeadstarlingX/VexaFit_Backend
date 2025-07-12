using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.DTOs.Action
{
    public class ErrorActionResult(string jsonString, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : ActionResult
    {
        private readonly string _jsonString = jsonString;
        private readonly HttpStatusCode _statusCode = statusCode;

        public override void ExecuteResult(ActionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var response = context.HttpContext.Response;
            if (!context.HttpContext.Response.HasStarted)
            {
                response.StatusCode = (int)_statusCode;
                response.ContentType = "application/json";
                using (TextWriter writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
                {
                    writer.Write(_jsonString);
                }
            }
        }
    }
}
