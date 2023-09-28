using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a tabular object for comparison.
    /// </summary>
    public interface ITabularObject
    {
        /// <summary>
        /// Name of the tabular object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Long name of the tabular object.
        /// </summary>
        string LongName { get; }

        /// <summary>
        /// Id of the tabular object.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Object definition of the tabular object.
        /// </summary>
        string ObjectDefinition { get; }

        /// <summary>
        /// Substitute Id of the tabular object.
        /// </summary>
        string SubstituteId { get; }
    }
}
