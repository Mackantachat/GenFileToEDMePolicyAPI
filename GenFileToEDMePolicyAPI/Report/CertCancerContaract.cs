using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI.Report
{
    public partial class CertCancerContaract
    {
        public CertCancerContaract(ReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
