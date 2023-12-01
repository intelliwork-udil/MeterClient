using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    /// <summary>
    /// The objective is to remotely disconnect/reconnect a meter.
    /// </summary>
    /// <remarks>
    /// Attributes: relay_operate, request_datetime
    /// </remarks>
    public class AuxRelayOperations
    {
        /// <summary>
        /// 0 for Disconnect, 1 for Connect
        /// </summary>
        public bool relay_operate { get; set; }
        /// <summary>
        /// Date & Time at which request is made
        /// </summary>
        public DateTime request_datetime { get; set; }

        /// <summary>
        /// Default Constructor
        /// Set relay_operate to false and request_datetime to current date & time
        /// </summary>
        public AuxRelayOperations()
        {
            relay_operate = false;
            request_datetime = DateTime.Now;
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="relay_operate"></param>
        /// <param name="request_datetime"></param>
        public AuxRelayOperations(bool relay_operate, DateTime request_datetime)
        {
            this.relay_operate = relay_operate;
            this.request_datetime = request_datetime;
        }
    }
}
