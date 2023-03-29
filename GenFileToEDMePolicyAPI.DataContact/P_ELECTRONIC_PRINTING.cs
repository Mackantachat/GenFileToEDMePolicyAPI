using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class P_ELECTRONIC_PRINTING
    {
        public long? PEP_ID { get; set; }

        public long? PRINTING_ID_REF { get; set; }

        public long? POLICY_ID { get; set; }

        public int? EDOCUMENT_CODE { get; set; }

        public DateTime? CREATE_DATE { get; set; }

        public string CREATE_ID { get; set; }

        public DateTime? PROVE_DATE { get; set; }

        public string PROVE_ID { get; set; }

        public char? EASY_FLG { get; set; }

        public DateTime? EASY_DT { get; set; }

        public string EMAIL { get; set; }

        public char? TMN { get; set; }

        public DateTime? TMN_DT { get; set; }

        public string TMN_ID { get; set; }

        public string TMN_CAUSE { get; set; }

        public char? TMN_AFT_APR { get; set; }
    }
}
