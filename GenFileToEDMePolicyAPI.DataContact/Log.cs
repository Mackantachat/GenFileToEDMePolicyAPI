using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class Log
    {
        public Int64? POLICY_ID { get; set; }

        public string COUNTPRINT { get; set; }

        public string POLICY { get; set; }

        public DateTime? ISU_DT { get; set; }

        public string POLICY_HOLDING { get; set; }
        public long? IMAGE_ID { get; set; }
    }
}
