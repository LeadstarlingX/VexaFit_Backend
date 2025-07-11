using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Serializer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Application.Serializer.ISerializableObject" />
    public class ApiResponse(bool Result, string Message, int Code, object? Data = null) : ISerializableObject
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Code { get; set; } = Code;
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = Message;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ApiResponse"/> is result.
        /// </summary>
        /// <value>
        ///   <c>true</c> if result; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Result { get; set; } = Result;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object? Data { get; set; } = Data;

        /// <summary>
        /// Gets the name of the primary property.
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryPropertyName()
        {
            return "response";
        }

        /// <summary>
        /// Gets the type of the primary property.
        /// </summary>
        /// <returns></returns>
        public Type GetPrimaryPropertyType()
        {
            return typeof(ApiResponse);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Application.Serializer.ISerializableObject" />
    public class ApiResponse<T> : ActionResult
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; set; }
        public required string Message { get; set; }
        public bool Result { get; set; }
        public required T Data { get; set; }


        public string GetPrimaryPropertyName()
        {
            return "response";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ApiResponse<T>);
        }
    }
}
