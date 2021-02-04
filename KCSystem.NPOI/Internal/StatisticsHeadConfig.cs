

namespace NPOI
{
    /// <summary>
    /// Represents the excel statistics for the specified model.
    /// </summary>
    internal class StatisticsHeadConfig
    {
		/// <summary>
		/// Gets or sets the statistics name. (e.g. Total)
		/// </summary>
		/// <remarks>
		/// In current version, the default name location is (last row, first cell)
		/// </remarks>
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cell formula, such as SUM, AVERAGE and so on, which applyable for vertical statistics.
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Gets or sets the column indexes for statistics. if <see cref="Formula"/> is SUM, 
        /// and <see cref="Columns"/> is [1,3], for example, the column No. 1 and 3 will be
        /// SUM for first row to last row.
        /// </summary>
        public int Rows { get; set; } = 1;

        public int StartCell { get; set; }

        public int EndCell { get; set; }
        public bool NewRow { get; set; }
    }
}
