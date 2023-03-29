using System;
using DevExpress.XtraReports.UI;
using GenFileToEDMePolicyAPI.DataContact;

namespace GenFileToEDMePolicyAPI.Report
{
    public partial class HappySongkran66Report
    {
        public HappySongkran66Report(SongKranReportModel[] data)
        {
            InitializeComponent();
            this.DataSource = data;
        }
    }
}
