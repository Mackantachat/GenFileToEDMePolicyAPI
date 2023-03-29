using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI.Report
{
    public partial class PANewYearSafeTravel
    {
        public PANewYearSafeTravel(NewYearReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
