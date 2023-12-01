using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    /// <summary>
    /// The objective is to synchronize meter time with the MDC time
    /// </summary>
    /// <remarks>
    /// Attributes: load_limit, maximum_retries, retry_interval, threshold_duration, retry_clear_interval, request_datetime
    /// </remarks>
    public class SanctionedLoadControl
    {
        /// <summary>
        /// Threshold Limit for kW
        /// </summary>
        public double load_limit { get; set; }
        /// <summary>
        /// Maximum Retries in number
        /// </summary>
        public int maximum_retries { get; set; }
        /// <summary>
        /// Time Interval for retry in seconds
        /// </summary>
        public int retry_interval { get; set; }
        /// <summary>
        /// Duration to accept threshold crossing limit in seconds
        /// </summary>
        public int threshold_duration { get; set; }
        /// <summary>
        /// Time after retries count is cleared in seconds
        /// </summary>
        public int retry_clear_interval { get; set; }
        /// <summary>
        /// Date & Time at which request is made
        /// </summary>
        public DateTime request_datetime { get; set; }

        /// <summary>
        /// Default Constructor
        /// Set default values for all attributes
        /// </summary>
        public SanctionedLoadControl()
        {
            load_limit = 0.0;
            maximum_retries = 0;
            retry_interval = 0;
            threshold_duration = 0;
            retry_clear_interval = 0;
            request_datetime = DateTime.Now;
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="load_limit"></param>
        /// <param name="maximum_retries"></param>
        /// <param name="retry_interval"></param>
        /// <param name="threshold_duration"></param>
        /// <param name="retry_clear_interval"></param>
        /// <param name="request_datetime"></param>
        public SanctionedLoadControl(double load_limit, int maximum_retries, int retry_interval, int threshold_duration, int retry_clear_interval, DateTime request_datetime)
        {
            this.load_limit = load_limit;
            this.maximum_retries = maximum_retries;
            this.retry_interval = retry_interval;
            this.threshold_duration = threshold_duration;
            this.retry_clear_interval = retry_clear_interval;
            this.request_datetime = request_datetime;
        }
    }
}
