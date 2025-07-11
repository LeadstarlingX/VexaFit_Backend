using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Serializer
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJsonFieldsSerializer
    {
        /// <summary>
        /// Serializes the specified object to serialize.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
