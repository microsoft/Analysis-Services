using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Keeps track of tables for processing, and dynamically increments the rows processed in each partition.
    /// </summary>
    public class ProcessingTable : IComparable<ProcessingTable>
    {
        private string _name;
        private string _id;
        private List<PartitionRowCounter> _partitions = new List<PartitionRowCounter>();

        /// <summary>
        /// Name of the table being processed.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Id of the table being processed. Can be different for tabular models with multidimensional metadata.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Collection of PartitionRowCounter objects.
        /// </summary>
        public List<PartitionRowCounter> Partitions => _partitions;

        /// <summary>
        /// Initializes a new instance of the PartitionRowCounter class using a name and id.
        /// </summary>
        public ProcessingTable(string name, string id)
        {
            _name = name;
            _id = id;
        }

        /// <summary>
        /// Get the total row count for all partitions associated with the table.
        /// </summary>
        /// <returns>Total row count.</returns>
        public long GetRowCount()
        {
            long rowCount = 0;
            foreach (PartitionRowCounter partition in _partitions)
            {
                rowCount += partition.RowCount;
            }
            return rowCount;
        }

        /// <summary>
        /// Find partition based on its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PartitionRowCounter object. Null if not found.</returns>
        public PartitionRowCounter FindPartition(string id)
        {
            foreach (PartitionRowCounter partition in _partitions)
            {
                if (partition.Id == id)
                {
                    return partition;
                }
            }
            return null;
        }

        /// <summary>
        /// Verifies if table contains a partition with the specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Boolean specifying whether the partition is contained by the table.</returns>
        public bool ContainsPartition(string id)
        {
            foreach (PartitionRowCounter partition in _partitions)
            {
                if (partition.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        public int CompareTo(ProcessingTable other) => string.Compare(this.Name, other.Name);
    }
}
