using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI
{
    public partial class PDFReport
    {
        public PDFReport(ReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
