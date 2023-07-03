using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DAL
{
    public class DAL_SIEVE_ANALYSIS_TEST
    {
        #region SieveAnalysis List

        public List<SIEVE_ANALYSIS_DATA_LIST> SELECT_SieveAnalysis_Data_List(SIEVE_ANALYSIS_LIST _Sieve)
        {
            SqlParameter[] param = { 
                                       new SqlParameter("@BRANCH_CODE", _Sieve.BRANCH_CODE),
                                       new SqlParameter("@PRODUCT_SIZE_CODE", _Sieve.PRODUCT_SIZE_CODE ),
                                       new SqlParameter("@SAMPLING_FROM", _Sieve.LOCATION_CODE),
                                       new SqlParameter("@SAH_CODE", _Sieve.SAMPLE_CODE),
                                       new SqlParameter("@FROM_DT", _Sieve.From_DT),
                                       new SqlParameter("@TO_DT", _Sieve.To_DT) 
                                   };

            DataSet ds = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataSet("[AGG].[USP_SELECT_SIEVE_ANALYSIS_LIST]", CommandType.StoredProcedure, param);

            List<SIEVE_ANALYSIS_DATA_LIST> _list = new List<SIEVE_ANALYSIS_DATA_LIST>();
            DataTable dt = ds.Tables[0];
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    _list.Add(new SIEVE_ANALYSIS_DATA_LIST
                    {
                        SAH_ID = Convert.ToDecimal(row["SAH_ID"]),
                        SAH_CODE = Convert.ToString(row["SAH_CODE"]),
                        Branch_Name = Convert.ToString(row["Branch_Name"]),
                        SIZE = Convert.ToString(row["SIZE"]),
                        SAMPLING_FROM = Convert.ToString(row["SAMPLING_FROM"]),
                        PROPOSE = Convert.ToString(row["PROPOSE"]),
                        SAMPLING_DATE = Convert.ToString(row["SAMPLING_DATE"]),
                        TESTING_DATE = Convert.ToString(row["TESTING_DATE"]),
                        REMARKS = Convert.ToString(row["REMARKS"]),

                        TESTED_BY_CODE = Convert.ToString(row["TESTED_BY_CODE"]),
                        TESTED_BY = Convert.ToString(row["TESTED_BY"]),
                        CHECKED_BY_CODE = Convert.ToString(row["CHECKED_BY_CODE"]),
                        CHECKED_BY = Convert.ToString(row["CHECKED_BY"]),
                        ADDED_BY_CODE = Convert.ToString(row["ADDED_BY_CODE"]),
                        ADDED_BY = Convert.ToString(row["ADDED_BY"]),
                        IS_LOCKED = Convert.ToInt32(row["IS_LOCKED"]) 

                    });
                }
            }

            return _list;
        }
        #endregion

        #region SieveAnalysis View

        public SIEVE_ANALYSIS_REPORT VIEW_SIEVE_ANALYSIS_TEST(decimal SAH_ID)
        {
            SqlParameter[] param = { new SqlParameter("@SAH_ID", SAH_ID) };

            DataSet ds = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataSet("[AGG].[USP_VIEW_SIEVE_ANALYSIS_TEST]", CommandType.StoredProcedure, param);
            SIEVE_ANALYSIS_REPORT _objSieve = new SIEVE_ANALYSIS_REPORT(); 

            List<SIEVE_ANALYSIS_DTL> _list = new List<SIEVE_ANALYSIS_DTL>();
            SIEVE_ANALYSIS_DTL dtl = null;
            DataTable dt = ds.Tables[0];
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dtl = new SIEVE_ANALYSIS_DTL();
                _objSieve.SAH_ID = Convert.ToDecimal(dt.Rows[0]["SAH_ID"]);
                _objSieve.SAH_CODE = Convert.ToString(dt.Rows[0]["SAH_CODE"]);
                _objSieve.BRANCH_CODE = Convert.ToString(dt.Rows[0]["BRANCH_CODE"]);
                _objSieve.Branch_Name = Convert.ToString(dt.Rows[0]["Branch_Name"]);
                _objSieve.PRODUCT_SIZE_CODE = Convert.ToInt32(dt.Rows[0]["PRODUCT_SIZE_CODE"]);
                _objSieve.SIZE = Convert.ToString(dt.Rows[0]["SIZE"]);
                _objSieve.SAMPLING_FROM_CODE = Convert.ToString(dt.Rows[0]["SAMPLING_FROM_CODE"]);
                _objSieve.SAMPLING_FROM = Convert.ToString(dt.Rows[0]["SAMPLING_FROM"]);
                _objSieve.PROPOSE = Convert.ToString(dt.Rows[0]["PROPOSE"]);
                _objSieve.SAMPLING_DATE = Convert.ToString(dt.Rows[0]["SAMPLING_DATE"]);
                _objSieve.TESTING_DATE = Convert.ToString(dt.Rows[0]["TESTING_DATE"]);
                _objSieve.REMARKS = Convert.ToString(dt.Rows[0]["REMARKS"]);

                _objSieve.TESTED_BY_CODE = Convert.ToString(dt.Rows[0]["TESTED_BY_CODE"]);
                _objSieve.TESTED_BY = Convert.ToString(dt.Rows[0]["TESTED_BY"]);
                _objSieve.TESTED_BY_DESG = Convert.ToString(dt.Rows[0]["TESTED_BY_DESG"]);

                _objSieve.CHECKED_BY_CODE = Convert.ToString(dt.Rows[0]["CHECKED_BY_CODE"]);
                _objSieve.CHECKED_BY = Convert.ToString(dt.Rows[0]["CHECKED_BY"]);
                _objSieve.CHECKED_BY_DESG = Convert.ToString(dt.Rows[0]["CHECKED_BY_DESG"]);

                _objSieve.ADDED_BY_CODE = Convert.ToString(dt.Rows[0]["ADDED_BY_CODE"]);
                _objSieve.ADDED_BY = Convert.ToString(dt.Rows[0]["ADDED_BY"]); 
                _objSieve.IS_FILE_UPLOAD = Convert.ToInt32(dt.Rows[0]["IS_FILE_UPLOAD"]);
                _objSieve.FILE_PATH = Convert.ToString(dt.Rows[0]["FILE_PATH"]);

                _objSieve.PRINT_PRODUCT = Convert.ToString(dt.Rows[0]["PRINT_PRODUCT"]);
                _objSieve.PRINT_TITLE = Convert.ToString(dt.Rows[0]["PRINT_TITLE"]);
                _objSieve.TOTAL_WT_GRAM = Convert.ToDecimal(dt.Rows[0]["TOTAL_WT_GRAM"]);

                _objSieve.SAMPLING_DT = Convert.ToDateTime(dt.Rows[0]["SAMPLING_DT"]);
                _objSieve.TESTING_DT = Convert.ToDateTime(dt.Rows[0]["TESTING_DT"]);


                _objSieve.IS_LOCKED = Convert.ToInt32(dt.Rows[0]["IS_LOCKED"]); 
            }

            DataTable dt2 =ds.Tables[1];
            foreach (DataRow row in dt2.Rows)
            {
                dtl = new SIEVE_ANALYSIS_DTL();

                dtl.SAHD_ID = Convert.ToDecimal(row["SAHD_ID"] == DBNull.Value ? 0 : row["SAHD_ID"]);
                dtl.SIEVE_SIZE_ID = Convert.ToInt32(row["SIEVE_SIZE_ID"] == DBNull.Value ? 0 : row["SIEVE_SIZE_ID"]);
                dtl.SIEVE_SIZE = Convert.ToString(row["SIEVE_SIZE"] == DBNull.Value ? "" : row["SIEVE_SIZE"]);

                dtl.RETAINED_GRAMS = Convert.ToInt32(row["RETAINED_GRAMS"] == DBNull.Value ? 0 : row["RETAINED_GRAMS"]);
                dtl.RETAINED_PER = Convert.ToDecimal(row["RETAINED_PER"] == DBNull.Value ? 0 : row["RETAINED_PER"]);
                dtl.CUM_RETAINED_PER = Convert.ToDecimal(row["CUM_RETAINED_PER"] == DBNull.Value ? 0 : row["CUM_RETAINED_PER"]);
                dtl.PASSING_PER = Convert.ToDecimal(row["PASSING_PER"] == DBNull.Value ? 0 : row["PASSING_PER"]);


                dtl.LIMIT = Convert.ToString(row["SIEVE_LIMIT"] == DBNull.Value ? "" : row["SIEVE_LIMIT"]);
                dtl.LIMIT_ZONE_I = Convert.ToString(row["LIMIT_ZONE_I"] == DBNull.Value ? "" : row["LIMIT_ZONE_I"]);
                dtl.LIMIT_ZONE_II = Convert.ToString(row["LIMIT_ZONE_II"] == DBNull.Value ? "" : row["LIMIT_ZONE_II"]);
                dtl.LIMIT_ZONE_III = Convert.ToString(row["LIMIT_ZONE_III"] == DBNull.Value ? "" : row["LIMIT_ZONE_III"]);
                dtl.LIMIT_ZONE_IV = Convert.ToString(row["LIMIT_ZONE_IV"] == DBNull.Value ? "" : row["LIMIT_ZONE_IV"]);



                string PRODUCT_SIZE = Convert.ToString(row["PRODUCT_SIZE"]).ToUpper();
                if (PRODUCT_SIZE.Contains("ZONE IV"))
                {
                    dtl.iSZONE = 4;
                }
                else if (PRODUCT_SIZE.Contains("ZONE III"))
                {
                    dtl.iSZONE = 3;
                }
                else if (PRODUCT_SIZE.Contains("ZONE II"))
                {
                    dtl.iSZONE = 2;
                }
                else if (PRODUCT_SIZE.Contains("ZONE I"))
                {
                    dtl.iSZONE = 1;
                }
                else
                {
                    dtl.iSZONE = 0;
                }

                _list.Add(dtl);

            }

            _objSieve.sieveList = _list;

            return _objSieve;
        }
        #endregion


        public List<SIEVE_ANALYSIS_DTL> GET_SIEVE_ANALYSIS_DTL(string Comp_Code, string Branch_Code, string Emp_Code, int productSizeCode)
        {
            SqlParameter[] param = {   
                                       new SqlParameter("@PRODUCT_SIZE_CODE", productSizeCode) 
                                   };

            DataTable dt = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataTable("[AGG].[USP_SELECT_SIEVE_ANALYSIS_DTL]", CommandType.StoredProcedure, param);
            List<SIEVE_ANALYSIS_DTL> list = new List<SIEVE_ANALYSIS_DTL>();

            SIEVE_ANALYSIS_DTL dtl = null;

            foreach (DataRow row in dt.Rows)
            {
                dtl = new SIEVE_ANALYSIS_DTL();
                dtl.SIEVE_SIZE_ID = Convert.ToInt32(row["SIEVE_SIZE_ID"] == DBNull.Value ? 0 : row["SIEVE_SIZE_ID"]);
                dtl.SIEVE_SIZE = Convert.ToString(row["SIEVE_SIZE"] == DBNull.Value ? "" : row["SIEVE_SIZE"]);

                //dtl.RETAINED_GRAMS = Convert.ToInt32(row["RETAINED_GRAMS"] == DBNull.Value ? 0 : row["RETAINED_GRAMS"]);
                //dtl.RETAINED_PER = Convert.ToDecimal(row["RETAINED_PER"] == DBNull.Value ? 0 : row["RETAINED_PER"]);
                //dtl.CUM_RETAINED_PER = Convert.ToDecimal(row["CUM_RETAINED_PER"] == DBNull.Value ? 0 : row["CUM_RETAINED_PER"]);
                //dtl.PASSING_PER = Convert.ToDecimal(row["PASSING_PER"] == DBNull.Value ? 0 : row["PASSING_PER"]);


                dtl.LIMIT = Convert.ToString(row["SIEVE_LIMIT"] == DBNull.Value ? "" : row["SIEVE_LIMIT"]);
                dtl.LIMIT_ZONE_I = Convert.ToString(row["LIMIT_ZONE_I"] == DBNull.Value ? "" : row["LIMIT_ZONE_I"]);
                dtl.LIMIT_ZONE_II = Convert.ToString(row["LIMIT_ZONE_II"] == DBNull.Value ? "" : row["LIMIT_ZONE_II"]);
                dtl.LIMIT_ZONE_III = Convert.ToString(row["LIMIT_ZONE_III"] == DBNull.Value ? "" : row["LIMIT_ZONE_III"]);
                dtl.LIMIT_ZONE_IV = Convert.ToString(row["LIMIT_ZONE_IV"] == DBNull.Value ? "" : row["LIMIT_ZONE_IV"]);

               

                string PRODUCT_SIZE = Convert.ToString(row["PRODUCT_SIZE"]).ToUpper();
                if (PRODUCT_SIZE.Contains("ZONE IV"))
                {  
                    dtl.iSZONE = 4; 
                }
                else if (PRODUCT_SIZE.Contains("ZONE III"))
                {
                    dtl.iSZONE = 3;
                }
                else if (PRODUCT_SIZE.Contains("ZONE II"))
                {
                    dtl.iSZONE = 2;
                }
                else if (PRODUCT_SIZE.Contains("ZONE I"))
                {
                    dtl.iSZONE = 1;
                }
                else  
                {
                    dtl.iSZONE = 0;
                }
                list.Add(dtl);

            }
            return list;
        }

        public string INSERT_SIEVE_ANALYSIS_TEST(string Comp_Code, string Branch_Code, string Emp_Code, SIEVE_ANALYSIS_TEST _sieveAnalysis, out ResultMessage oblMsg)
        {
            string errorMsg = "";
            oblMsg = new ResultMessage();
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {
                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    int IS_FILE_UPLOAD = 0;

                    string DMS_Path = ConfigurationManager.AppSettings["DMSPATH"].ToString();
                    string filePath = "REPORT\\SIEVE ANALYSIS TEST\\";
                    string directoryPath = DMS_Path + filePath;
                    string ext = "";
                    if (_sieveAnalysis.UploadFile != null)
                    {
                        IS_FILE_UPLOAD = 1;
                        ext = new System.IO.FileInfo(_sieveAnalysis.UploadFile.FileName).Extension;
                    }


                    SqlParameter[] param =
                    { 
                      new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                     ,new SqlParameter("@SAH_ID", SqlDbType.Decimal) 
                     ,new SqlParameter("@SAH_CODE", SqlDbType.VarChar, 25)  
                     ,new SqlParameter("@COMP_CODE", Comp_Code)
                     ,new SqlParameter("@BRANCH_CODE", _sieveAnalysis.BRANCH_CODE)
                     ,new SqlParameter("@PRODUCT_SIZE_CODE", (_sieveAnalysis.PRODUCT_SIZE_CODE == null)?(object)DBNull.Value : _sieveAnalysis.PRODUCT_SIZE_CODE)    
                     ,new SqlParameter("@SAMPLING_FROM", (_sieveAnalysis.LOCATION_CODE == null)?(object)DBNull.Value : _sieveAnalysis.LOCATION_CODE)  
                     ,new SqlParameter("@PROPOSE", (_sieveAnalysis.PROPOSE == null)?(object)DBNull.Value : _sieveAnalysis.PROPOSE)  
                     ,new SqlParameter("@SAMPLING_DATE", (_sieveAnalysis.SAMPLING_DT == null)?(object)DBNull.Value : _sieveAnalysis.SAMPLING_DT)  
                     ,new SqlParameter("@TESTING_DATE", (_sieveAnalysis.TESTING_DT == null)?(object)DBNull.Value : _sieveAnalysis.TESTING_DT) 
                     ,new SqlParameter("@REMARKS", (_sieveAnalysis.REMARKS == null)?(object)DBNull.Value : _sieveAnalysis.REMARKS)  
                     ,new SqlParameter("@TESTED_BY_CODE", (_sieveAnalysis.TESTED_BY_CODE == null)?(object)DBNull.Value : _sieveAnalysis.TESTED_BY_CODE)  
                     ,new SqlParameter("@CHECKED_BY_CODE", (_sieveAnalysis.CHECKED_BY_CODE == null)?(object)DBNull.Value : _sieveAnalysis.CHECKED_BY_CODE)  
                     ,new SqlParameter("@ADDED_BY", Emp_Code) 
                     ,new SqlParameter("@IS_FILE_UPLOAD", IS_FILE_UPLOAD)  
                     ,new SqlParameter("@FILE_PATH", string.IsNullOrEmpty(ext)?(object)DBNull.Value:ext) 
                     ,new SqlParameter("@TOTAL_WT_GRAM", _sieveAnalysis.ANALYSIS_DTL_LIST.Sum(x=>x.RETAINED_GRAMS)) 

                    
                    };

                    param[0].Direction = ParameterDirection.Output;
                    param[1].Direction = ParameterDirection.Output;
                    param[2].Direction = ParameterDirection.Output;
                    new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_SIEVE_ANALYSIS_HDR]", CommandType.StoredProcedure, out command, connection, transactionScope, param);
                    decimal SAH_ID = (decimal)command.Parameters["@SAH_ID"].Value;
                    string SAH_CODE = (string)command.Parameters["@SAH_CODE"].Value;
                    string error_1 = (string)command.Parameters["@ERRORSTR"].Value;

                    if (SAH_ID == -1) { errorMsg = error_1; }

                    if (SAH_ID > 0)
                    {

                        if (_sieveAnalysis.ANALYSIS_DTL_LIST != null)
                        {
                            foreach (SIEVE_ANALYSIS_DTL item in _sieveAnalysis.ANALYSIS_DTL_LIST)
                            {

                                SqlParameter[] param2 =
                                    {
                                       new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                                      ,new SqlParameter("@SAHD_ID", SqlDbType.Decimal)  
                                      ,new SqlParameter("@SAH_ID", SAH_ID)  
                                      ,new SqlParameter("@SIEVE_SIZE_ID", (item.SIEVE_SIZE_ID == null) ? 0 : item.SIEVE_SIZE_ID)
                                      ,new SqlParameter("@RETAINED_GRAMS" , (item.RETAINED_GRAMS == null)? 0 : item.RETAINED_GRAMS) 
                                      ,new SqlParameter("@RETAINED_PER", (item.RETAINED_PER == null)? 0 : item.RETAINED_PER) 
                                      ,new SqlParameter("@CUM_RETAINED_PER", (item.CUM_RETAINED_PER == null)? 0 : item.CUM_RETAINED_PER) 
                                      ,new SqlParameter("@PASSING_PER", (item.PASSING_PER == null)? 0 : item.PASSING_PER)  
                                    };
                                param2[0].Direction = ParameterDirection.Output;
                                param2[1].Direction = ParameterDirection.Output;
                                new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_SIEVE_ANALYSIS_DTL]", CommandType.StoredProcedure, out command, connection, transactionScope, param2);
                                decimal SAHD_ID = (decimal)command.Parameters["@SAHD_ID"].Value;
                                string error_2 = (string)command.Parameters["@ERRORSTR"].Value;
                                if (SAHD_ID == -1) { errorMsg = error_2; break; }


                            }
                        }
                    }

                    
                    if (errorMsg == "")
                    {
                        // Below code is used for attached slip file
                        if (_sieveAnalysis.UploadFile != null)
                        {
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            if (_sieveAnalysis.UploadFile != null)
                            {
                                string fileName = SAH_CODE.Replace("/","_") + ext;
                                _sieveAnalysis.UploadFile.SaveAs(directoryPath + fileName);
                            }
                        }

                        oblMsg.ID = SAH_ID;
                        oblMsg.CODE = SAH_CODE;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();
                    }
                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        transactionScope.Rollback();
                    }
                    catch (Exception ex1)
                    {
                        errorMsg = "Error: Exception occured. " + ex1.StackTrace.ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }

        public string UPDATE_SIEVE_ANALYSIS_TEST(string Comp_Code, string Branch_Code, string Emp_Code, SIEVE_ANALYSIS_TEST_EDIT _sieveAnalysis, out ResultMessage oblMsg)
        {
            string errorMsg = "";
            oblMsg = new ResultMessage();
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {
                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    int IS_FILE_UPLOAD = 0;

                    string DMS_Path = ConfigurationManager.AppSettings["DMSPATH"].ToString();
                    string filePath = "REPORT\\SIEVE ANALYSIS TEST\\";
                    string directoryPath = DMS_Path + filePath;
                    string ext = "";
                    if (_sieveAnalysis.UploadFile != null)
                    {
                        IS_FILE_UPLOAD = 1;
                        ext = new System.IO.FileInfo(_sieveAnalysis.UploadFile.FileName).Extension;
                    }


                    SqlParameter[] param =
                    { 
                      new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                     ,new SqlParameter("@SAH_ID", _sieveAnalysis.SAH_ID)  
                     ,new SqlParameter("@SAMPLING_FROM", (_sieveAnalysis.LOCATION_CODE == null)?(object)DBNull.Value : _sieveAnalysis.LOCATION_CODE)  
                     ,new SqlParameter("@PROPOSE", (_sieveAnalysis.PROPOSE == null)?(object)DBNull.Value : _sieveAnalysis.PROPOSE)  
                     ,new SqlParameter("@SAMPLING_DATE", (_sieveAnalysis.SAMPLING_DT == null)?(object)DBNull.Value : _sieveAnalysis.SAMPLING_DT)  
                     ,new SqlParameter("@TESTING_DATE", (_sieveAnalysis.TESTING_DT == null)?(object)DBNull.Value : _sieveAnalysis.TESTING_DT) 
                     ,new SqlParameter("@REMARKS", (_sieveAnalysis.REMARKS == null)?(object)DBNull.Value : _sieveAnalysis.REMARKS)  
                     ,new SqlParameter("@TESTED_BY_CODE", (_sieveAnalysis.TESTED_BY_CODE == null)?(object)DBNull.Value : _sieveAnalysis.TESTED_BY_CODE)  
                     ,new SqlParameter("@CHECKED_BY_CODE", (_sieveAnalysis.CHECKED_BY_CODE == null)?(object)DBNull.Value : _sieveAnalysis.CHECKED_BY_CODE)  
                     ,new SqlParameter("@IS_FILE_UPLOAD", IS_FILE_UPLOAD)  
                     ,new SqlParameter("@FILE_PATH", string.IsNullOrEmpty(ext)?(object)DBNull.Value:ext) 
                     ,new SqlParameter("@TOTAL_WT_GRAM", _sieveAnalysis.ANALYSIS_DTL_LIST.Sum(x=>x.RETAINED_GRAMS)) 
                    };

                    param[0].Direction = ParameterDirection.Output;
                    
                    new DataAccess().InsertWithTransaction("[AGG].[USP_UPDATE_SIEVE_ANALYSIS_HDR]", CommandType.StoredProcedure, out command, connection, transactionScope, param);
                    
                    string error_1 = (string)command.Parameters["@ERRORSTR"].Value;

                    if (error_1 != "") { errorMsg = error_1; }

                    if (errorMsg == "")
                    {

                        if (_sieveAnalysis.ANALYSIS_DTL_LIST != null)
                        {
                            foreach (SIEVE_ANALYSIS_DTL item in _sieveAnalysis.ANALYSIS_DTL_LIST)
                            {

                                SqlParameter[] param2 =
                                    {
                                       new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                                      ,new SqlParameter("@SAHD_ID", item.SAHD_ID)  
                                      ,new SqlParameter("@SAH_ID", _sieveAnalysis.SAH_ID)  
                                      ,new SqlParameter("@SIEVE_SIZE_ID", (item.SIEVE_SIZE_ID == null) ? 0 : item.SIEVE_SIZE_ID)
                                      ,new SqlParameter("@RETAINED_GRAMS" , (item.RETAINED_GRAMS == null)? 0 : item.RETAINED_GRAMS) 
                                      ,new SqlParameter("@RETAINED_PER", (item.RETAINED_PER == null)? 0 : item.RETAINED_PER) 
                                      ,new SqlParameter("@CUM_RETAINED_PER", (item.CUM_RETAINED_PER == null)? 0 : item.CUM_RETAINED_PER) 
                                      ,new SqlParameter("@PASSING_PER", (item.PASSING_PER == null)? 0 : item.PASSING_PER)  
                                    };
                                param2[0].Direction = ParameterDirection.Output; 
                                new DataAccess().InsertWithTransaction("[AGG].[USP_UPDATE_SIEVE_ANALYSIS_DTL]", CommandType.StoredProcedure, out command, connection, transactionScope, param2);
                                
                                string error_2 = (string)command.Parameters["@ERRORSTR"].Value;
                                if (error_2 != "") { errorMsg = error_2; break; }


                            }
                        }
                    }


                    if (errorMsg == "")
                    {
                        // Below code is used for attached slip file
                        if (_sieveAnalysis.UploadFile != null)
                        {
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            if (_sieveAnalysis.UploadFile != null)
                            {
                                string fileName = _sieveAnalysis.SAH_CODE.Replace("/", "_") + ext;
                                _sieveAnalysis.UploadFile.SaveAs(directoryPath + fileName);
                            }
                        }

                        oblMsg.ID = _sieveAnalysis.SAH_ID;
                        oblMsg.CODE = _sieveAnalysis.SAH_CODE;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();
                    }
                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        transactionScope.Rollback();
                    }
                    catch (Exception ex1)
                    {
                        errorMsg = "Error: Exception occured. " + ex1.StackTrace.ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }

        public string LOCK_SIEVE_ANALYSIS_TEST(string Comp_Code, string Branch_Code, string Emp_Code, decimal SAH_ID,string SAH_CODE ,out ResultMessage oblMsg)
        {
            string errorMsg = "";
            oblMsg = new ResultMessage();
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {
                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlParameter[] param =
                    { 
                      new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                     ,new SqlParameter("@SAH_ID", SAH_ID)  
                     ,new SqlParameter("@LOCKED_BY", Emp_Code)  
                     
                    };

                    param[0].Direction = ParameterDirection.Output;

                    new DataAccess().InsertWithTransaction("[AGG].[USP_UPDATE_SIEVE_ANALYSIS_LOCK]", CommandType.StoredProcedure, out command, connection, transactionScope, param);

                    string error_1 = (string)command.Parameters["@ERRORSTR"].Value;

                    if (error_1 != "") { errorMsg = error_1; }

                    if (errorMsg == "")
                    {
                        oblMsg.ID = SAH_ID;
                        oblMsg.CODE = SAH_CODE;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();
                    }
                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        transactionScope.Rollback();
                    }
                    catch (Exception ex1)
                    {
                        errorMsg = "Error: Exception occured. " + ex1.StackTrace.ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }

    
    
    
    }
}
