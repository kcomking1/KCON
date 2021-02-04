

namespace NPOI
{
    using System;

    /// <summary>
    /// Represents a custom attribute for some simple statistics.
    /// </summary>
    /// <remarks>
    /// Only for vertical, not for horizontal statistics. and in current version, 
    /// doesn't allow apply multiple <see cref="StatisticsAttribute"/> to one class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StatisticsHeadAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsAttribute"/> class.
        /// </summary>
        public StatisticsHeadAttribute()
        {
            StatisticsConfig = new StatisticsHeadConfig();
        }

        /// <summary>
        /// Gets or sets the statistics name. (e.g. Total)
        /// </summary>
        /// <remarks>
        /// In current version, the default name location is (last row, first cell)
        /// </remarks>
        public string Name
        {
            get
            {
                return StatisticsConfig.Name;
            }
            set
            {
                StatisticsConfig.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the cell formula, such as SUM, AVERAGE and so on, which applyable for vertical statistics.
        /// </summary>
        public string Formula
        {
            get
            {
                return StatisticsConfig.Formula;
            }
            set
            {
                StatisticsConfig.Formula = value;
            }
        }



        /// <summary>
        /// Gets or sets the column indexes for statistics. if <see cref="Formula"/> is SUM, 
        /// and <see cref="Rows"/> is [1,3], for example, the column No. 1 and 3 will be
        /// SUM for first row to last row.
        /// </summary>
        public int Rows
        {
            get
            {
                return StatisticsConfig.Rows;
            }
            set
            {
                StatisticsConfig.Rows = value;
            }
        }

       

        
        public int StartCell
        {
            get
            {
                return StatisticsConfig.StartCell;
            }
            set
            {
                StatisticsConfig.StartCell = value;
            }
        }

        public int EndCell
        {
            get
            {
                return StatisticsConfig.EndCell;
            }
            set
            {
                StatisticsConfig.EndCell = value;
            }
        }

        /// <summary>
        /// Gets the statistics config.
        /// </summary>
        /// <value>The statistics config.</value>
        internal StatisticsHeadConfig StatisticsConfig { get; }
        /// <summary>
        /// 是否新起一行
        /// </summary>
        public bool NewRow
        {
            get
            {
                return StatisticsConfig.NewRow;
            }
            set
            {
                StatisticsConfig.NewRow = value;
            }
        }
    }
}
