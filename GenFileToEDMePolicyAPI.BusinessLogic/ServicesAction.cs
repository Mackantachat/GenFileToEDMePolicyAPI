using GenFileToEDMePolicyAPI.DataAccess;
using GenFileToEDMePolicyAPI.DataContact;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GenFileToEDMePolicyAPI.BusinessLogic
{
    public partial class ServicesAction
    {
        private readonly string connectionString;
        public ServicesAction(string ConnectionName) => this.connectionString = ConnectionName;


        public ReportModel[] getData(string policyId) => getData(policyId, (Repository)null);

        private ReportModel[] getData(string policyId, Repository repository)
        {
            bool internalConnection = false;
            ReportModel[] data = null;
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.GetData(policyId);
                var benefitPerson = repository.GetBenefitPersonFirstYear(policyId);
                if (benefitPerson == null)
                {
                    benefitPerson = repository.GetBenefitPersonNextYear(policyId);
                }
                foreach (var rep in data)
                {
                    rep.BENEFITPERSON = "เหมือนกรมธรรม์เลขที่ " + benefitPerson.BENEFITPERSON;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }

        public PilicyIdentityModel GetPolicyIdentity(string policyId) => GetPolicyIdentity(policyId, (Repository)null);

        private PilicyIdentityModel GetPolicyIdentity(string policyId, Repository repository)
        {
            bool internalConnection = false;
            PilicyIdentityModel data = new PilicyIdentityModel();
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.GetPolicyIdentity(policyId);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }

        public P_LIFE_ID GetISUDate(string policyId) => GetISUDate(policyId, (Repository)null);

        private P_LIFE_ID GetISUDate(string policyId, Repository repository)
        {
            bool internalConnection = false;
            P_LIFE_ID data = null;
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.GetISUDate(policyId);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }

        public void WriteLog(string _UserID, Log obj) => WriteLog(_UserID, obj, (Repository)null);

        private void WriteLog(string _UserID, Log obj, Repository repository)
        {
            //ลงตาราง epolicy
            bool internalConnection = false;
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {

                P_POLPRINTING _printing = new P_POLPRINTING();
                _printing.POLICY_ID = obj.POLICY_ID;
                _printing.PRINT_BY = "EPO"; //*
                _printing.PRINT_DT = DateTime.Now;
                _printing.PRINT_ID = _UserID; //*
                _printing.PRINT_OFFICE = "สนญ"; //*
                repository.AddP_POLPRINTING(ref _printing);
                //prtPolprinting = client.AddP_POLPRINTING(ref _printing);

                DataContact.P_ELECTRONIC_PRINTING _ELEC = new DataContact.P_ELECTRONIC_PRINTING();
                _ELEC.PRINTING_ID_REF = _printing.POLPRINTING_ID;
                _ELEC.POLICY_ID = obj.POLICY_ID;
                _ELEC.EDOCUMENT_CODE = 1; //*
                _ELEC.CREATE_DATE = DateTime.Now;
                _ELEC.CREATE_ID = _UserID;
                _ELEC.EASY_FLG = 'N';
                _ELEC.TMN = 'N';
                repository.AddP_ELECTRONIC_PRINTING(ref _ELEC);
                //client.AddP_ELECTRONIC_PRINTING(ref _ELEC);


                P_ELECTRONIC_FILE addObjPol = new P_ELECTRONIC_FILE();
                addObjPol.EDOCUMENT_CODE = 1;
                addObjPol.FILE_NAME = obj.POLICY_HOLDING + "_" + obj.POLICY.PadLeft(8, '0');
                addObjPol.REF_NO = obj.POLICY_ID.ToString();
                addObjPol.REF_DT = obj.ISU_DT;
                addObjPol.PEP_ID = _ELEC.PEP_ID;
                addObjPol.EASY_ID = obj.IMAGE_ID;
                repository.AddP_ELECTRONIC_FILE(ref addObjPol);
                //client.AddP_ELECTRONIC_FILE(ref addObjPol);
                if (internalConnection)
                {
                    repository.commitTransaction();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                }
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
        }
        public NewYearReportModel[] GetDataNewYearReport(string policyId) => GetDataNewYearReport(policyId, (Repository)null);

        private NewYearReportModel[] GetDataNewYearReport(string policyId, Repository repository)
        {
            bool internalConnection = false;
            NewYearReportModel[] data = null;
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.GetDataNewYearReport(policyId);
                var benefitPerson = repository.GetBenefitPersonFirstYear(policyId);
                foreach (var rep in data)
                {
                    rep.BENEFITPERSON = "ตามกรมธรรม์เลขที่ " + benefitPerson.BENEFITPERSON;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }

        public SongKranReportModel[] GetDataSongKranReport(string policyId) => GetDataSongKranReport(policyId, (Repository)null);

        private SongKranReportModel[] GetDataSongKranReport(string policyId, Repository repository)
        {
            bool internalConnection = false;
            SongKranReportModel[] data = null;
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.GetDataSongKranReport(policyId);
                var benefitPerson = repository.GetBenefitPersonFirstYear(policyId);
                foreach (var rep in data)
                {
                    rep.BENEFITPERSON = "ตามกรมธรรม์เลขที่ " + benefitPerson.BENEFITPERSON;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }

        public TypeCertificate CheckTypeCert(string policyId) => CheckTypeCert(policyId, (Repository)null);

        private TypeCertificate CheckTypeCert(string policyId, Repository repository)
        {
            bool internalConnection = false;
            TypeCertificate data = new TypeCertificate();
            if (repository is null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                internalConnection = true;
            }

            try
            {
                data = repository.CheckTypeCert(policyId);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
            return data;
        }
    }


}
