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

        public string ProcessCommand(string re)
        {
            string command = "";
            if (re.Contains("C0 01 81 00 46 00 00 60 03 0A FF 02 00"))
            {
                string relayOperateHex = relay_operate ? "01" : "00";
                command = "C4 01 81 00 03 " + relayOperateHex;
            }
            else if (re.Contains("C0 01 81 00 46 00 00 60 03 0A FF 03 00"))
            {
                string relayOperateHex = relay_operate ? "01" : "00";
                command = "C4 01 81 00 16 " + relayOperateHex;
            }
            return command;
        }
    }

}
