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
    /// Attributes: request_datetime
    /// </remarks>
    public class TimeSynchronization
    {
        /// <summary>
        /// Date & Time at which request is made
        /// </summary>
        public DateTime request_datetime { get; set; }

        /// <summary>
        /// Default Constructor
        /// Set Defaulkt Values to Attributes
        /// </summary>
        public TimeSynchronization()
        {
            request_datetime = DateTime.Now;
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="request_datetime"></param>
        public TimeSynchronization(DateTime request_datetime)
        {
            this.request_datetime = request_datetime;
        }
    }
}
