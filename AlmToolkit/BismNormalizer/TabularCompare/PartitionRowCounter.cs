using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Counter for number of rows processed for a partition.
    /// </summary>
    public class PartitionRowCounter
    {
        private string _id;
        private long _rowCount;

        /// <summary>
        /// Id of the partition being processed.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Count of rows counted for the partition.
        /// </summary>
        public long RowCount
        {
            get { return _rowCount; }
            set { _rowCount = value; }
        }

        /// <summary>
        /// Initializes a new instance of the PartitionRowCounter class using an id.
        /// </summary>
        public PartitionRowCounter(string id)
        {
            _id = id;
            _rowCount = 0;
        }
    }
}
