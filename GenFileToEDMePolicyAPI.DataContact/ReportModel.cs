using System;
using System.Collections.Generic;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataContact
{
    public class ReportModel
    {
        public string POLICY { get; set; }
        public string POLICY_HOLDING { get; set; }
        public string CUSTNAME { get; set; }
        public DateTime? ISU_DT { get; set; }
        public DateTime? MAT_DT { get; set; }
        public string NOW_DT { get; set; }
        public string BENEFITPERSON  { get; set; }
     
    }

    public class NewYearReportModel : ReportModel
    {
        public string IDCARD_NO { get; set; }
        public DateTime? ASS_DT { get; set; }
        public string ADDRESS { get; set; }
        public string MB_PHONE { get; set; }
    }

    public class SongKranReportModel : ReportModel
    {
        public string IDCARD_NO { get; set; }
        public DateTime? ASS_DT { get; set; }
        public string ADDRESS { get; set; }
        public string MB_PHONE { get; set; }
    }
}
