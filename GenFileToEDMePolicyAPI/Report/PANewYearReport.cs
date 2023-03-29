using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI.Report
{
    public partial class PANewYearReport
    {
        public PANewYearReport(NewYearReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
