using DevExpress.XtraPrinting;
using GenFileToEDMePolicyAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccessUtility;
using GenFileToEDMePolicyAPI.DataContact;
using GenFileToEDMePolicyAPI.BusinessLogic;
using System.Globalization;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using PDF_SignServiceWcfSvc;
using ITUtility;
using System.Text;
using System.ServiceModel;

namespace GenFileToEDMePolicyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenFileController : ControllerBase
    {
        private static byte[] filereport = null;
        private static ReportModel[] report = null;
        private readonly IConfiguration Configuration;
        private readonly ServicesAction serviceAction;

        public GenFileController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            var connecttionString = Configuration.GetConnectionString("BLA");
            this.serviceAction = new ServicesAction(connecttionString);
        }
        [HttpGet("Test")]
        public ActionResult GenerateFile()
        {
            return Ok();
        }
        [HttpPost]
        [Route("GenerateFile")]
        public IActionResult GenerateFile([FromBody] PolicyRequest policy)
        {
            try
            {
                var typeCert = serviceAction.CheckTypeCert(policy.PolicyId);
                byte[] fileOut = null;
                FileContentResult file = null;
                if (typeCert.PL_CODE.Equals("011"))
                {
                    file = GenerateGroupAccidentCertificateReport(policy.PolicyId, out fileOut);
                }
                else if (typeCert.PL_CODE.Equals("012"))
                {
                    file = GenerateFilePaNewYearReport(policy.PolicyId, out fileOut);
                }
                else if (typeCert.PL_CODE.Equals("013"))
                {
                    file = GenerateFilePaSongKranReport(policy.PolicyId, out fileOut);
                }
                else if (typeCert.PL_CODE.Equals("014"))
                {
                    file = GenerateCertCancerContractReport(policy.PolicyId, out fileOut);
                }
                else if (typeCert.PL_CODE.Equals("015"))
                {
                    file = GenerateFilePaNewYearSafeTravel(policy.PolicyId, out fileOut);
                }
                else if (typeCert.PL_CODE.Equals("016"))
                {
                    file = GenerateFileHappySongKranReport(policy.PolicyId, out fileOut);
                }
                #region "Keep file to EDM and Logging to 3 Table"
                var poId = serviceAction.GetPolicyIdentity(policy.PolicyId);
                EDMModel eDM_DATA = new EDMModel()
                {
                    DataFile = new DataContact.File
                    {
                        ContentType = file.ContentType,
                        FileName = file.FileDownloadName,
                        FileData = Convert.ToBase64String(fileOut)
                    },
                    CATE_TYPE = "ELECTRONIC_POLICY",
                    APP_NO = poId.APP_NO,
                    CHANNEL_TYPE = poId.CHANNEL_TYPE,
                    POLICY_ID = policy.PolicyId,
                    POLICY_NO = poId.POLICY_NUMBER,
                    CERT_NO = poId.CERT_NUMBER,
                    N_USERID = "NB0001",
                };
                var imageId = EDMImport(eDM_DATA);
                var isuDate = serviceAction.GetISUDate(eDM_DATA.POLICY_ID);
                Log obj = new Log();
                obj.POLICY = eDM_DATA.CERT_NO;
                obj.POLICY_ID = Convert.ToInt64(eDM_DATA.POLICY_ID);
                obj.COUNTPRINT = "";
                obj.POLICY_HOLDING = eDM_DATA.POLICY_NO;
                obj.ISU_DT = isuDate.ISU_DT;
                obj.IMAGE_ID = imageId;
                serviceAction.WriteLog(eDM_DATA.N_USERID, obj);
                #endregion "Keep file to EDM and Logging to 3 Table"

                return Ok(new { imageId });
                //return file;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }

            //return file;
        }

        private long? EDMImport(EDMModel data)
        {
            long? image_id = null;
            var emdConfig = Configuration.GetSection("EDM");
            var client = new RestClient($"{emdConfig.Value}api/v1/Endrose/EDMImport");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(data);
            var response = client.Execute<long?>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                image_id = response.Data;
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject(response.Content).ToString());
            }
            return image_id;
        }

        private FileContentResult GenerateFilePaNewYearReport(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(Configuration["PolicyWcf"]);
            //broker = new BrokerPrintWcfSvc.BrokerPrintSvcClient(binding, endpoint);
            PolicyWcf.PolicySvcClient client = new PolicyWcf.PolicySvcClient(binding, endpoint);
            string[] address = null;
            var zipcode = string.Empty;
            var pr = client.GetCompositeAddressByPolicyId(Convert.ToInt64(policy), 1, out address, out zipcode);

            var reportData = serviceAction.GetDataNewYearReport(policy);
            foreach (var rep in reportData)
            {
                rep.ADDRESS = address[0].ToString() + " " + zipcode;
                rep.POLICY = rep.POLICY.TrimStart('0');
            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new Report.PANewYearReport(reportData);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = "NewYearCertificateReport" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client2 = new PDF_SignServiceClient())
            {
                var pr2 = client2.KeySelectedPdfSign(filereport, 3, out fileOut);
            }
            return file;

        }

        private FileContentResult GenerateFilePaSongKranReport(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(Configuration["PolicyWcf"]);
            //broker = new BrokerPrintWcfSvc.BrokerPrintSvcClient(binding, endpoint);
            PolicyWcf.PolicySvcClient client = new PolicyWcf.PolicySvcClient(binding, endpoint);
            string[] address = null;
            var zipcode = string.Empty;
            var pr = client.GetCompositeAddressByPolicyId(Convert.ToInt64(policy), 1, out address, out zipcode);

            var reportData = serviceAction.GetDataSongKranReport(policy);
            foreach (var rep in reportData)
            {
                rep.ADDRESS = address[0].ToString() + " " + zipcode;
                rep.POLICY = rep.POLICY.TrimStart('0');
            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new Report.PASongkran(reportData);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = policy + "_SongKranCertificateReport" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client2 = new PDF_SignServiceClient())
            {
                var pr2 = client2.KeySelectedPdfSign(filereport, 3, out fileOut);
            }
            return file;

        }

        private FileContentResult GenerateGroupAccidentCertificateReport(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            var year = dateNow.ToString("yyyy", new CultureInfo("th-TH"));
            var toDay = $"{dateNow.Day.ToString()}/{dateNow.Month.ToString().PadLeft(2, '0')}/{year}";
            report = serviceAction.getData(policy);
            foreach (var rep in report)
            {
                rep.NOW_DT = toDay;

            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new PDFReport(report);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = "GroupAccidentCertificateReport_" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client = new PDF_SignServiceClient())
            {
                var pr = client.KeySelectedPdfSign(filereport, 3, out fileOut);

            }
            return file;
        }

        private FileContentResult GenerateCertCancerContractReport(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            var year = dateNow.ToString("yyyy", new CultureInfo("th-TH"));
            var toDay = $"{dateNow.Day.ToString()}/{dateNow.Month.ToString().PadLeft(2, '0')}/{year}";
            report = serviceAction.getData(policy);
            foreach (var rep in report)
            {
                rep.NOW_DT = toDay;

            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new Report.CertCancerContaract(report);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = "CertCancerContractReport" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client = new PDF_SignServiceClient())
            {
                var pr = client.KeySelectedPdfSign(filereport, 3, out fileOut);

            }
            return file;
        }

        private FileContentResult GenerateFilePaNewYearSafeTravel(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(Configuration["PolicyWcf"]);
            //broker = new BrokerPrintWcfSvc.BrokerPrintSvcClient(binding, endpoint);
            PolicyWcf.PolicySvcClient client = new PolicyWcf.PolicySvcClient(binding, endpoint);
            string[] address = null;
            var zipcode = string.Empty;
            var pr = client.GetCompositeAddressByPolicyId(Convert.ToInt64(policy), 1, out address, out zipcode);

            var reportData = serviceAction.GetDataNewYearReport(policy);
            foreach (var rep in reportData)
            {
                rep.ADDRESS = address[0].ToString() + " " + zipcode;
                rep.POLICY = rep.POLICY.TrimStart('0');
            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new Report.PANewYearSafeTravel(reportData);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = "PANewYearSafeTravel" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client2 = new PDF_SignServiceClient())
            {
                var pr2 = client2.KeySelectedPdfSign(filereport, 3, out fileOut);
            }
            return file;

        }

        private FileContentResult GenerateFileHappySongKranReport(string policy, out byte[] fileOut)
        {
            var dateNow = DateTime.Now;
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(Configuration["PolicyWcf"]);
            //broker = new BrokerPrintWcfSvc.BrokerPrintSvcClient(binding, endpoint);
            PolicyWcf.PolicySvcClient client = new PolicyWcf.PolicySvcClient(binding, endpoint);
            string[] address = null;
            var zipcode = string.Empty;
            var pr = client.GetCompositeAddressByPolicyId(Convert.ToInt64(policy), 1, out address, out zipcode);

            var reportData = serviceAction.GetDataSongKranReport(policy);
            foreach (var rep in reportData)
            {
                rep.ADDRESS = address[0].ToString() + " " + zipcode;
                rep.POLICY = rep.POLICY.TrimStart('0');
            }
            using (MemoryStream ms = new MemoryStream())
            {
                var rpt = new Report.HappySongkran66Report(reportData);
                rpt.CreateDocument();
                rpt.ExportToPdf(ms);
                filereport = ms.ToArray();
            }
            var fileName = policy + "_Happy_SongKranCertificateReport" + dateNow.ToString() + ".pdf";
            var file = File(filereport, "application/pdf", fileName);

            using (PDF_SignServiceClient client2 = new PDF_SignServiceClient())
            {
                var pr2 = client2.KeySelectedPdfSign(filereport, 3, out fileOut);
            }
            return file;

        }

        private BasicHttpBinding SetHttpBinding()
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.CloseTimeout = TimeSpan.FromMinutes(10);
                binding.OpenTimeout = TimeSpan.FromMinutes(10);
                binding.SendTimeout = TimeSpan.FromMinutes(10);
                binding.MaxBufferPoolSize = Int32.MaxValue;
                binding.MaxBufferSize = Int32.MaxValue;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                binding.TransferMode = TransferMode.Streamed;
                binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
                binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
                binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
                return binding;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
