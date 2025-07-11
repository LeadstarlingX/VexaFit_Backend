using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helper;
using Newtonsoft.Json.Linq;

namespace Application.Serializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Application.Serializer.IJsonFieldsSerializer" />
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="jsonFields">The json fields.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">objectToSerialize</exception>
        public string Serialize(ISerializableObject objectToSerialize, string jsonFields)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException(nameof(objectToSerialize));
            }

            IList<string> fieldsList = null;

            if (!string.IsNullOrEmpty(jsonFields))
            {
                var primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsList = GetPropertiesIntoList(jsonFields);

                // Always add the root manually
                fieldsList.Add(primaryPropertyName);
            }

            var json = Serialize(objectToSerialize, fieldsList!);

            return json;
        }

        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="jsonFields">The json fields.</param>
        /// <returns></returns>
        private string Serialize(object objectToSerialize, IList<string> jsonFields = null)
        {
            var jToken = JToken.FromObject(objectToSerialize);

            if (jsonFields != null)
            {
                jToken = jToken.RemoveEmptyChildrenAndFilterByFields(jsonFields);
            }

            var jTokenResult = jToken.ToString();

            return jTokenResult;
        }

        /// <summary>
        /// Gets the properties into list.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        private IList<string> GetPropertiesIntoList(string fields)
        {
            IList<string> properties = fields.ToLowerInvariant()
                                             .Split(new[]
                                                    {
                                                        ','
                                                    }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim())
                                             .Distinct()
                                             .ToList();

            return properties;
        }
    }

}
