using DataAccessUtility;
using GenFileToEDMePolicyAPI.DataContact;
using ITUtility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GenFileToEDMePolicyAPI.DataAccess
{
    public class Repository : RepositoryBaseManagedCore
    {
        public Repository(string ConnectionName) : base(ConnectionName)
        {

        }
        public Repository(OracleConnection connection) : base(connection)
        {

        }
        public ReportModel[] GetData(string policyId)
        {
            ReportModel[] returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"SELECT
                                        pid.policy,
                                        ph.policy_holding,
                                        pnid.prename
                                        || pnid.name
                                        || '  '
                                        || pnid.surname custname,
                                        pl.isu_dt,
                                        pl.mat_dt
                                    FROM
                                        p_policy_id       pid,
                                        p_policyholding   ph,
                                        p_pol_name        ppn,
                                        p_name_id         pnid,
                                        p_life_id         pl
                                    WHERE
                                        pid.policy_id = ph.policy_id
                                        AND ppn.policy_id = pid.policy_id
                                        AND ppn.name_id = pnid.name_id
                                        AND pl.policy_id = pid.policy_id
                                        AND pnid.tmn = 'N'
                                        AND pid.policy_id = :policyId
                                    ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<ReportModel>().ToArray();
                }
            }

            return returnValue;
        }

        public ReportModel GetBenefitPersonFirstYear(string policyId)
        {
            ReportModel retuenValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"select (case when ph.POLICY_HOLDING is not null then ph.POLICY_HOLDING || '/' ||ppi.policy else ppi.policy end)as benefitperson
                                    from policy.u_policy_bundle_campaign upbc
                                    inner join policy.u_policy_bundle_pack upbp on (upbp.zbc_id = upbc.zbc_id and upbp.uapp_id =upbc.uapp_id )
                                    left join policy.p_policy_id ppi on ppi.policy_id = upbc.policy_id
                                    left join policy.p_policyholding ph on ph.policy_id = ppi.policy_id
                                    where upbp.policy_id_bundle = :policyId and upbp.tmn_flg = 'N'";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    retuenValue = dt.AsEnumerable<ReportModel>().FirstOrDefault();
                }
            }

            return retuenValue;
        }

        public ReportModel GetBenefitPersonNextYear(string policyId)
        {
            ReportModel retuenValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"select (case when ph.POLICY_HOLDING is not null then ph.POLICY_HOLDING || '/' ||ppi.policy else ppi.policy end)as benefitperson
                                    from policy.P_PF_RENEWAL ppr
                                    inner join policy.p_policy_id ppi on ppi.policy_id = ppr.policy_id
                                    left join policy.p_policyholding ph on ph.policy_id = ppi.policy_id
                                    where new_free_policy_id = :policyId and tmn = 'N'";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    retuenValue = dt.AsEnumerable<ReportModel>().FirstOrDefault();
                }
            }

            return retuenValue;
        }

        public PilicyIdentityModel GetPolicyIdentity(string policyId)
        {
            PilicyIdentityModel returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"select * from POLICY.P_POLICY_IDENTITY PI
                                        INNER JOIN POLICY.P_APPL_ID AP ON ap.policy_id = pi.policy_id
                                  where PI.POLICY_ID = :policyId 
                                    ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<PilicyIdentityModel>().FirstOrDefault();

                }
            }

            return returnValue;
        }

        public P_LIFE_ID GetISUDate(string policyId)
        {
            P_LIFE_ID returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"select * from POLICY.p_life_id where policy_id = :policyId ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<P_LIFE_ID>().FirstOrDefault();

                }
            }

            return returnValue;
        }

        public void AddP_ELECTRONIC_FILE(ref P_ELECTRONIC_FILE addObjPol)
        {
            try
            {

                string sqlStr;
                OracleCommand oCmd = null;

                sqlStr = "SELECT POLICY.PEF_ID_SEQ.NEXTVAL FROM DUAL";
                oCmd = new OracleCommand(sqlStr, connection);
                addObjPol.PEF_ID = Convert.ToInt64(oCmd.ExecuteScalar());

                StringBuilder SQL = new StringBuilder();
                StringBuilder SQLValue = new StringBuilder();

                SQL.Append(" INSERT INTO \"POLICY\".P_ELECTRONIC_FILE ");
                SQLValue.Append(" VALUES ");
                SQL.Append(" ( ");
                SQLValue.Append(" ( ");

                SQL.Append(" PEF_ID ");
                SQLValue.Append(" " + Utility.SQLValueString(addObjPol.PEF_ID));

                SQL.Append(",PEP_ID ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.PEP_ID));

                SQL.Append(",FILE_NAME ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.FILE_NAME));

                SQL.Append(",EDOCUMENT_CODE ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.EDOCUMENT_CODE));

                SQL.Append(",REF_NO ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.REF_NO));

                SQL.Append(",REF_DT ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.REF_DT));

                SQL.Append(",EASY_ID ");
                SQLValue.Append("," + Utility.SQLValueString(addObjPol.EASY_ID));

                SQL.Append(" ) ");
                SQLValue.Append(" ) ");
                SQL.Append(SQLValue.ToString());

                sqlStr = SQL.ToString();
                oCmd = new OracleCommand(sqlStr, connection);
                int recordCount = oCmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine("erro : AddP_POLPRINTING " + e.Message);
            }
        }

        public void AddP_ELECTRONIC_PRINTING(ref P_ELECTRONIC_PRINTING _ELEC)
        {

            string sqlStr;
            OracleCommand oCmd;

            sqlStr = "SELECT POLICY.PEP_ID_SEQ.NEXTVAL FROM DUAL";
            oCmd = new OracleCommand(sqlStr, connection);
            _ELEC.PEP_ID = Convert.ToInt64(oCmd.ExecuteScalar());

            StringBuilder SQL = new StringBuilder();
            StringBuilder SQLValue = new StringBuilder();

            SQL.Append(" INSERT INTO \"POLICY\".P_ELECTRONIC_PRINTING ");
            SQLValue.Append(" VALUES ");
            SQL.Append(" ( ");
            SQLValue.Append(" ( ");

            SQL.Append(" PEP_ID ");
            SQLValue.Append(" " + Utility.SQLValueString(_ELEC.PEP_ID));

            SQL.Append(",PRINTING_ID_REF ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.PRINTING_ID_REF));

            SQL.Append(",POLICY_ID ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.POLICY_ID));

            SQL.Append(",EDOCUMENT_CODE ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.EDOCUMENT_CODE));

            SQL.Append(",CREATE_DATE ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.CREATE_DATE));

            SQL.Append(",CREATE_ID ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.CREATE_ID));

            SQL.Append(",PROVE_DATE ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.PROVE_DATE));

            SQL.Append(",PROVE_ID ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.PROVE_ID));

            SQL.Append(",EASY_FLG ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.EASY_FLG));

            SQL.Append(",EMAIL ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.EMAIL));

            SQL.Append(",TMN ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.TMN));

            SQL.Append(",TMN_DT ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.TMN_DT));

            SQL.Append(",TMN_ID ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.TMN_ID));

            SQL.Append(",TMN_CAUSE ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.TMN_CAUSE));

            SQL.Append(",TMN_AFT_APR ");
            SQLValue.Append("," + Utility.SQLValueString(_ELEC.TMN_AFT_APR));

            SQL.Append(" ) ");
            SQLValue.Append(" ) ");
            SQL.Append(SQLValue.ToString());

            sqlStr = SQL.ToString();
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();




        }

        public void AddP_POLPRINTING(ref P_POLPRINTING _printing)
        {

            string sqlStr;
            OracleCommand oCmd;

            sqlStr = "SELECT P_POLPRINTING_SEQ.NEXTVAL FROM DUAL";
            oCmd = new OracleCommand(sqlStr, connection);
            _printing.POLPRINTING_ID = Convert.ToInt64(oCmd.ExecuteScalar());

            sqlStr =
                "INSERT INTO \"POLICY\".P_POLPRINTING(\n" +
                "   POLPRINTING_ID,\n" +
                "   POLICY_ID,\n" +
                "   PRINT_BY,\n" +
                "   PRINT_DT,\n" +
                "   PRINT_ID,\n" +
                "   PRINT_OFFICE)\n" +
                "VALUES(\n" +
                "   " + Utility.SQLValueString(_printing.POLPRINTING_ID) + ",\n" +
                "   " + Utility.SQLValueString(_printing.POLICY_ID) + ",\n" +
                "   " + Utility.SQLValueString(_printing.PRINT_BY) + ",\n" +
                "   " + Utility.SQLValueString(_printing.PRINT_DT) + ",\n" +
                "   " + Utility.SQLValueString(_printing.PRINT_ID) + ",\n" +
                "   " + Utility.SQLValueString(_printing.PRINT_OFFICE) + ")\n" +
                "";
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();


        }

        public NewYearReportModel[] GetDataNewYearReport(string policyId)
        {
            NewYearReportModel[] returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"SELECT
                                    pid.policy,
                                    ph.policy_holding,
                                    pnid.prename
                                    || pnid.name
                                    || '  '
                                    || pnid.surname custname,
                                    pnid.idcard_no,
                                    pnid.mb_phone,
                                    pl.isu_dt,
                                    pl.ass_dt,
                                    pnid.name_id
                                FROM
                                    p_policy_id       pid,
                                    p_policyholding   ph,
                                    p_pol_name        ppn,
                                    p_name_id         pnid,
                                    p_life_id         pl
                                WHERE
                                    pid.policy_id = ph.policy_id
                                    AND ppn.policy_id = pid.policy_id
                                    AND ppn.name_id = pnid.name_id
                                    AND pl.policy_id = pid.policy_id
                                    AND pnid.tmn = 'N'
                                    AND pid.policy_id = :policyId
                                    ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<NewYearReportModel>().ToArray();
                }
            }

            return returnValue;
        }

        public SongKranReportModel[] GetDataSongKranReport(string policyId)
        {
            SongKranReportModel[] returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"SELECT
                                    pid.policy,
                                    ph.policy_holding,
                                    pnid.prename
                                    || pnid.name
                                    || '  '
                                    || pnid.surname custname,
                                    pnid.idcard_no,
                                    pnid.mb_phone,
                                    pl.isu_dt,
                                    pl.ass_dt,
                                    pnid.name_id
                                FROM
                                    p_policy_id       pid,
                                    p_policyholding   ph,
                                    p_pol_name        ppn,
                                    p_name_id         pnid,
                                    p_life_id         pl
                                WHERE
                                    pid.policy_id = ph.policy_id
                                    AND ppn.policy_id = pid.policy_id
                                    AND ppn.name_id = pnid.name_id
                                    AND pl.policy_id = pid.policy_id
                                    AND pnid.tmn = 'N'
                                    AND pid.policy_id = :policyId
                                    ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<SongKranReportModel>().ToArray();
                }
            }

            return returnValue;
        }

        public TypeCertificate CheckTypeCert(string policyId)
        {
            TypeCertificate returnValue = null;
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = connection;
            oCmd.CommandType = System.Data.CommandType.Text;
            oCmd.CommandText = $@"select PL_BLOCK,PL_CODE,PL_CODE2 From policy.p_policy_id pid
                    left join policy.p_life_id lf on pid.policy_id = lf.policy_id
                    where lf.pl_block='F' and pid.policy_id = :policyId    ";
            using (OracleParameter pSearchKey = new OracleParameter("policyId", policyId))
            {
                oCmd.BindByName = true;
                oCmd.Parameters.Add(pSearchKey);
            }
            using (DataTable dt = ITUtility.Utility.FillDataTable(oCmd))
            {

                if (dt.Rows.Count > 0)
                {
                    returnValue = dt.AsEnumerable<TypeCertificate>().FirstOrDefault();
                }
            }
            

            return returnValue;
        }
    }
}
