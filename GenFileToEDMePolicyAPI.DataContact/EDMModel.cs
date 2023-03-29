using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class EDMModel
    {
        public string APP_NO { get; set; }
        public string POLICY_ID { get; set; }
        public string POLICY_NO { get; set; }
        public string CHANNEL_TYPE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_SURNAME { get; set; }
        public string END_NO { get; set; }
        public string N_USERID { get; set; }
        public string CATE_TYPE { get; set; }
        public string CERT_NO { get; set; }
        public File DataFile { get; set; }
    }
    public class File
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileData { get; set; }

    }

}
