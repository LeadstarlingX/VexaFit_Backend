using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Application.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class JTokenHelper
    {
        /// <summary>
        /// Removes the empty children and filter by fields.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="jsonFields">The json fields.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static JToken RemoveEmptyChildrenAndFilterByFields(this JToken token, IList<string> jsonFields, int level = 1)
        {
            if (token.Type == JTokenType.Object)
            {
                var copy = new JObject();

                foreach (var prop in token.Children<JProperty>())
                {
                    var child = prop.Value;

                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildrenAndFilterByFields(jsonFields, level + 1);
                    }

                    var allowedFields = jsonFields.Contains(prop.Name.ToLowerInvariant()) || level > 3;
                    var notEmpty = !child.IsEmptyOrDefault() || level == 1 || level == 3;

                    if (notEmpty && allowedFields)
                    {
                        copy.Add(prop.Name, child);
                    }
                }

                return copy;
            }

            if (token.Type == JTokenType.Array)
            {
                var copy = new JArray();

                foreach (var item in token.Children())
                {
                    var child = item;

                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildrenAndFilterByFields(jsonFields, level + 1);
                    }

                    if (!child.IsEmptyOrDefault())
                    {
                        copy.Add(child);
                    }
                }

                return copy;
            }

            return token;
        }

        /// <summary>
        /// Determines whether [is empty or default].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <c>true</c> if [is empty or default] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmptyOrDefault(this JToken token)
        {
            return token.Type == JTokenType.Array && !token.HasValues || token.Type == JTokenType.Object && !token.HasValues;
        }
    }
}
