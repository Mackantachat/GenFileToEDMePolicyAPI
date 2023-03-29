using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class P_POLPRINTING
    {
        public long? POLPRINTING_ID { get; set; }
        public long? POLICY_ID { get; set; }
        public string PRINT_BY { get; set; }
        public DateTime? PRINT_DT { get; set; }
        public string PRINT_ID { get; set; }
        public string PRINT_OFFICE { get; set; }
    }
}
