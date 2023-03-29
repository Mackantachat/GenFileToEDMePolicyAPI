using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class P_ELECTRONIC_FILE
    {
        public long? PEF_ID { get; set; }

        public long? PEP_ID { get; set; }

        public string FILE_NAME { get; set; }

        public long? EDOCUMENT_CODE { get; set; }

        public string REF_NO { get; set; }

        public DateTime? REF_DT { get; set; }

        public string NAME { get; set; }

        public string SURNAME { get; set; }

        public long? EASY_ID { get; set; }
    }
}
