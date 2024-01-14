using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    /// <summary>
    /// The load management service is to be used to auto disconnect/reconnect the supply on defined intervals as set by the DISCO.
    /// </summary>
    /// <remarks>
    /// Attributes: request_datetime
    /// </remarks>
    public class LoadSheddingScheduling
    {
        public DateTime start_datetime { get; set; }
        public DateTime end_datetime { get; set; }
        public JsonArray load_schedding_slabs { get; set; }
        /// <summary>
        /// Date & Time at which request is made
        /// </summary>
        public DateTime request_datetime { get; set; }

        /// <summary>
        /// Default Constructor
        /// Set Defaulkt Values to Attributes
        /// </summary>
        public LoadSheddingScheduling()
        {
            start_datetime = DateTime.Now;
            end_datetime = DateTime.Now;
            load_schedding_slabs = new JsonArray();
            request_datetime = DateTime.Now;
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="request_datetime"></param>
        public LoadSheddingScheduling(DateTime start_datetime, DateTime end_datetime, DateTime request_datetime)
        {
            this.start_datetime = start_datetime;
            this.end_datetime = end_datetime;
            load_schedding_slabs = new JsonArray();
            this.request_datetime = request_datetime;
        }


        public string ProcessCommand(string re)
        {
            string command = "C7 01 81 00 00";





            return command;
        }
    }
}
