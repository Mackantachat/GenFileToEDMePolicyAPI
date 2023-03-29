using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI.Report
{
    public partial class PASongkran
    {
        public PASongkran(SongKranReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
