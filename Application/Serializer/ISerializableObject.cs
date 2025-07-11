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
    public interface ISerializableObject
    {
        /// <summary>
        /// Gets the name of the primary property.
        /// </summary>
        /// <returns></returns>
        string GetPrimaryPropertyName();
        /// <summary>
        /// Gets the type of the primary property.
        /// </summary>
        /// <returns></returns>
        Type GetPrimaryPropertyType();
    }
}
