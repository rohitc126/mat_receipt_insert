using BusinessLayer;
using BusinessLayer.DAL;
using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace eSGBIZ.Controllers
{
    public class AgQualityMngtController : BaseController
    {

        [Authorize]
        public ActionResult SieveAnalysisList()
        {
            ViewBag.Header = "Sieve Analysis Test";
            SIEVE_ANALYSIS_LIST _sieve = new SIEVE_ANALYSIS_LIST();

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _sieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _sieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            return View(_sieve);

        }

        public ActionResult _SieveAnalysis_List()
        {
            return PartialView("_SieveAnalysis_List");
        }

        [HttpPost]
        public ActionResult _SieveAnalysis_Data_List(string brCode, string prodSizeCode, string lCode, string sCode, DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            SIEVE_ANALYSIS_LIST _sieve = new SIEVE_ANALYSIS_LIST();
            List<SIEVE_ANALYSIS_DATA_LIST> sievesDatas = new List<SIEVE_ANALYSIS_DATA_LIST>();
            try
            {
                _sieve.BRANCH_CODE = brCode;
                _sieve.PRODUCT_SIZE_CODE = prodSizeCode;
                _sieve.LOCATION_CODE = lCode;
                _sieve.SAMPLE_CODE = sCode;
                _sieve.From_DT = fDate;
                _sieve.To_DT = tDate;

                sievesDatas = new DAL_SIEVE_ANALYSIS_TEST().SELECT_SieveAnalysis_Data_List(_sieve);

                totalRow = sievesDatas.Count();

            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error : _CNs_Data_List ", ex.Message);
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                sievesDatas = sievesDatas.
                    Where(x => x.SAH_CODE.ToLower().Contains(searchValue.ToLower()) || x.Branch_Name.ToLower().Contains(searchValue.ToLower())
                        || x.SIZE.ToLower().Contains(searchValue.ToLower())
                        || x.SAMPLING_FROM.ToLower().Contains(searchValue.ToLower())
                        || x.PROPOSE.ToLower().Contains(searchValue.ToLower())
                        || x.SAMPLING_DATE.ToLower().Contains(searchValue.ToLower())
                        || x.TESTING_DATE.ToString().Contains(searchValue.ToLower())
                        || x.REMARKS.ToString().Contains(searchValue.ToLower())
                        || x.TESTED_BY.ToString().Contains(searchValue.ToLower())
                        || x.CHECKED_BY.ToLower().Contains(searchValue.ToLower())).ToList<SIEVE_ANALYSIS_DATA_LIST>();




            }
            int totalRowFilter = sievesDatas.Count();
            // sorting
            //sievesDatas = sievesDatas.OrderBy(sortColumnName + " " + sortDirection).ToList<SIEVE_ANALYSIS_DATA_LIST>();

            // Paging
            if (length == -1)
            {
                length = totalRow;
            }
            sievesDatas = sievesDatas.Skip(start).Take(length).ToList<SIEVE_ANALYSIS_DATA_LIST>();

            var jsonResult = Json(new { data = sievesDatas, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [Authorize]
        public ActionResult SieveAnalysisTest()
        {
            ViewBag.Header = "Sieve Analysis Test";
            SIEVE_ANALYSIS_TEST _sieve = new SIEVE_ANALYSIS_TEST();

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _sieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _sieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _sieve.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");

            return View(_sieve);

        }

        public ActionResult _GetSieveProduct_Dtl(string BranchCode, int productSizeCode)
        {
            List<SIEVE_ANALYSIS_DTL> dtl = new DAL_SIEVE_ANALYSIS_TEST().GET_SIEVE_ANALYSIS_DTL(emp.Comp_Code, BranchCode, emp.Employee_Code, productSizeCode);
            SIEVE_ANALYSIS_TEST _objSieve = new SIEVE_ANALYSIS_TEST();
            _objSieve.ANALYSIS_DTL_LIST = dtl;


            return PartialView("_GetSieveProduct_Dtl", _objSieve);
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("SieveAnalysisTest")]
        public ActionResult SieveAnalysisTest_Save(SIEVE_ANALYSIS_TEST _objSieve)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_SIEVE_ANALYSIS_TEST().INSERT_SIEVE_ANALYSIS_TEST(emp.Comp_Code, emp.BranchCode, emp.Employee_Code, _objSieve, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Sieve analysis test inserted successfully. Sample No. : </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("SieveAnalysisTest", "AgQualityMngt");
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _objSieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _objSieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _objSieve.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");


            return View(_objSieve);
        }



        public ActionResult SieveAnalysisTest_View(decimal SAH_ID)
        {
            SIEVE_ANALYSIS_REPORT _objSieve = new SIEVE_ANALYSIS_REPORT();
            _objSieve = new DAL_SIEVE_ANALYSIS_TEST().VIEW_SIEVE_ANALYSIS_TEST(SAH_ID);
            return PartialView("SieveAnalysisTest_View", _objSieve);

        }


        [Authorize]
        public ActionResult SieveAnalysis_DocView(string fileName)
        {
            ViewBag.Header = "Uploaded Document";
            string mimetype = GetMimeType(fileName);
            byte[] bytes;
            //FTP Server URL.
            string ftp_Path = ConfigurationManager.AppSettings["iTMSFTPHOST"].ToString();
            string ftp_User = ConfigurationManager.AppSettings["iTMSFTPUSER"].ToString();
            string ftp_Pwd = ConfigurationManager.AppSettings["iTMSFTPPWD"].ToString();
            //FTP Folder name. Leave blank if you want to Download file from root folder.
            //string ftpFolder = "iDMS/";
            string ftpFolder = "/";
            ftpFolder = ftpFolder + "/";
            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp_Path + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(ftp_User, ftp_Pwd);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse webResponse = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    CopyStream(webResponse.GetResponseStream(), stream);
                    bytes = stream.ToArray();

                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ContentType = mimetype;
                    //Response.ContentType = "application/msword";
                    //Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();

                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
            return File(bytes, mimetype);
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, read);
            }
        }

        public string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        public FileResult ShowDocument(string FilePath)
        {
            string DMS_Path = ConfigurationManager.AppSettings["DMSPATH"].ToString();
            string directoryPath = DMS_Path + "REPORT\\SIEVE ANALYSIS TEST\\" + FilePath;

            //return File(Server.MapPath("~/Files/") + FilePath, GetMimeType(FilePath));
            return File(directoryPath, GetMimeType(FilePath));
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSieveAnalysisHTML(decimal SAH_ID)
        {
            string resultHTML = "";
            string _tempBody = "";

            try
            {

                SIEVE_ANALYSIS_REPORT _objSieve = new SIEVE_ANALYSIS_REPORT();
                _objSieve = new DAL_SIEVE_ANALYSIS_TEST().VIEW_SIEVE_ANALYSIS_TEST(SAH_ID);

                using (StreamReader reader = new StreamReader(Server.MapPath("~/Print_Templates/SieveAnalysisReport.html")))
                {
                    _tempBody = reader.ReadToEnd();
                }

                _tempBody = _tempBody.Replace("[BRANCH_NAME]", _objSieve.Branch_Name.Split('-')[1].ToUpper());
                _tempBody = _tempBody.Replace("[PRINT_PRODUCT]", _objSieve.PRINT_PRODUCT);
                _tempBody = _tempBody.Replace("[PRINT_TITLE]", _objSieve.PRINT_TITLE);
                _tempBody = _tempBody.Replace("[SAMPLE_CODE]", _objSieve.SAH_CODE);
                _tempBody = _tempBody.Replace("[SAMPLING_FROM]", _objSieve.SAMPLING_FROM);
                _tempBody = _tempBody.Replace("[PROPOSE]", _objSieve.PROPOSE);
                _tempBody = _tempBody.Replace("[SAMPLING_DATE]", _objSieve.SAMPLING_DATE);
                _tempBody = _tempBody.Replace("[TESTING_DATE]", _objSieve.TESTING_DATE);

                StringBuilder sbHeader = new StringBuilder();
                sbHeader.Append("<table border='1' width='100%' cellspacing='2' cellpadding='0' style='border-collapse: collapse; font-family:Calibri'>");
                sbHeader.Append("<tr><th rowspan='2' class='font20 innerTH'>IS Sieve Size<br />in mm</th><th rowspan='2' class='font20 innerTH'>Material Retained <br /> on each sieve, gms</th>");
                sbHeader.Append("<th rowspan='2' class='font20 innerTH'>%<br />Retained</th><th rowspan='2' class='font20 innerTH'>%<br />Cum. Retained</th><th rowspan='2' class='font20 innerTH'>%<br />Passing</th>");

                if (_objSieve.sieveList[0].iSZONE > 0)
                {
                    sbHeader.Append("<th colspan='4' class='font20 innerTH' style='text-align:center;'>LIMIT, IS 383-1970 (RA-2016)</th></tr>");
                }
                else
                {
                    sbHeader.Append("<th style='text-align:center;' class='font20 innerTH'>LIMIT, IS 383-1970 (RA-2016)</th></tr>");
                }
                sbHeader.Append("<tr>");
                if (_objSieve.sieveList[0].iSZONE > 0)
                {
                    sbHeader.Append("<th style='text-align:center;' class='font20 innerTH'>Zone I </th><th style='text-align:center;' class='font20 innerTH'>Zone II</th><th style='text-align:center;' class='font20 innerTH'>Zone III </th><th style='text-align:center;' class='font20 innerTH'>Zone IV</th>");
                }
                sbHeader.Append("</tr>");

                _tempBody = _tempBody.Replace("[TBL_HEADER]", sbHeader.ToString());


                StringBuilder sbDetail = new StringBuilder();
                for (int i = 0; i < _objSieve.sieveList.Count(); i++)
                {
                    sbDetail.Append("<tr><td style='text-align: center;padding:3px;'>" + _objSieve.sieveList[i].SIEVE_SIZE + "</td>");
                    sbDetail.Append("<td style='text-align: center;padding:3px;'>" + _objSieve.sieveList[i].RETAINED_GRAMS + "</td>");
                    sbDetail.Append("<td style='text-align: center;padding:3px;'>" + _objSieve.sieveList[i].RETAINED_PER + "</td>");
                    sbDetail.Append("<td style='text-align: center;padding:3px;'>" + _objSieve.sieveList[i].CUM_RETAINED_PER + "</td>");
                    sbDetail.Append("<td style='text-align: center;padding:3px;'>" + _objSieve.sieveList[i].PASSING_PER + "</td>");


                    if (_objSieve.sieveList[0].iSZONE > 0)
                    {
                        if (_objSieve.sieveList[0].iSZONE == 1)
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px; background-color: #fffada;'>" + _objSieve.sieveList[i].LIMIT_ZONE_I + "</td>");

                        }
                        else
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px;'>" + _objSieve.sieveList[i].LIMIT_ZONE_I + "</td>");

                        }

                        if (_objSieve.sieveList[0].iSZONE == 2)
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px; background-color: #fffada;'>" + _objSieve.sieveList[i].LIMIT_ZONE_II + "</td>");
                        }
                        else
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px;'>" + _objSieve.sieveList[i].LIMIT_ZONE_II + "</td>");
                        }

                        if (_objSieve.sieveList[0].iSZONE == 3)
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px; background-color: #fffada;'>" + _objSieve.sieveList[i].LIMIT_ZONE_III + "</td>");

                        }
                        else
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px;'>" + _objSieve.sieveList[i].LIMIT_ZONE_III + "</td>");

                        }

                        if (_objSieve.sieveList[0].iSZONE == 4)
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px; background-color: #fffada;'>" + _objSieve.sieveList[i].LIMIT_ZONE_IV + "</td>");

                        }
                        else
                        {
                            sbDetail.Append("<td style='text-align: center; padding: 3px;'>" + _objSieve.sieveList[i].LIMIT_ZONE_IV + "</td>");

                        }
                    }
                    else
                    {
                        sbDetail.Append("<td style='text-align: center; padding: 3px;'>" + _objSieve.sieveList[i].LIMIT + "</td>");

                    }


                    sbDetail.Append("</tr>");
                }

                if (_objSieve.sieveList[0].iSZONE > 0)
                {
                    sbDetail.Append("<tr><td colspan='9' style='font-size:16px;font-weight:700;height:30px;'>Total Weight Of Sample in gram : " + _objSieve.TOTAL_WT_GRAM + "</td></tr>");
                    sbDetail.Append("<tr><td colspan='9' style='font-size:16px;font-weight:700;height:30px;'>Remarks : " + _objSieve.REMARKS + "</td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='font-size:16px;font-weight:700;' class='innerTH'>Tested By</td><td colspan='5' style='font-size:16px;font-weight:700;' class='innerTH'>Checked By</td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='font-size:16px;font-weight:700;border-top:none;border-bottom:none;'>SGX MINERALS PVT. LTD.</td><td colspan='5' style='font-size:16px;font-weight:700;border-top:none;border-bottom:none;'>SGX MINERALS PVT. LTD.</td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='border-top:none;border-bottom:none;'><b>Name :</b> " + _objSieve.TESTED_BY + " </td><td colspan='5' style='border-top:none;border-bottom:none;'><b>Name :</b> " + _objSieve.CHECKED_BY + " </td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='border-top:none;border-bottom:none;'><b>Designation :</b> " + _objSieve.TESTED_BY_DESG + " </td><td colspan='5' style='border-top:none;border-bottom:none;;vertical-align:top;'><b>Designation :</b> " + _objSieve.CHECKED_BY_DESG + " </td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='border-top:none;border-bottom:none;height:50px;'><b>Sign :</b> </td><td colspan='5' style='border-top:none;border-bottom:none;'><b>Sign :</b> </td></tr>");
                    sbDetail.Append("<tr><td colspan='4' style='border-top:none;border-bottom:none;'><b>Date :</b> </td><td colspan='5' style='border-top:none;border-bottom:none;'><b>Date :</b> </td></tr>");
                
                }
                else
                {
                    sbDetail.Append("<tr><td colspan='6' style='font-size:16px;font-weight:700;height:30px;'>Total Weight Of Sample in gram : " + _objSieve.TOTAL_WT_GRAM + "</td></tr>");
                    sbDetail.Append("<tr><td colspan='9' style='font-size:16px;font-weight:700;height:30px;'>Remarks : " + _objSieve.REMARKS + "</td></tr>"); 
                    sbDetail.Append("<tr><td colspan='3' style='font-size:16px;font-weight:700;' class='innerTH'>Tested By</td><td colspan='3' style='font-size:16px;font-weight:700;' class='innerTH'>Checked By</td></tr>");
                    sbDetail.Append("<tr><td colspan='3' style='font-size:16px;font-weight:700;border-top:none;border-bottom:none;' >SGX MINERALS PVT. LTD.</td><td colspan='3' style='font-size:16px;font-weight:700;border-top:none;border-bottom:none;'>SGX MINERALS PVT. LTD.</td></tr>");
                    sbDetail.Append("<tr><td colspan='3' style='border-top:none;border-bottom:none;'><b>Name :</b> " + _objSieve.TESTED_BY + " </td><td colspan='3' style='border-top:none;border-bottom:none;'><b>Name :</b> " + _objSieve.CHECKED_BY + " </td></tr>");
                    sbDetail.Append("<tr><td colspan='3' style='border-top:none;border-bottom:none;'><b>Designation :</b> " + _objSieve.TESTED_BY_DESG + " </td><td colspan='3' style='border-top:none;border-bottom:none;'><b>Designation :</b> " + _objSieve.CHECKED_BY_DESG + " </td></tr>");
                    sbDetail.Append("<tr><td colspan='3' style='border-top:none;border-bottom:none;height:50px;'><b>Sign :</b> </td><td colspan='3' style='border-top:none;border-bottom:none;'><b>Sign :</b> </td></tr>");
                    sbDetail.Append("<tr><td colspan='3' style='border-top:none;border-bottom:none;'><b>Date :</b> </td><td colspan='3' style='border-top:none;border-bottom:none;'><b>Date :</b> </td></tr>");

                }

                sbDetail.Append("</tbody>");
                _tempBody = _tempBody.Replace("[TBL_DTL_LIST]", sbDetail.ToString());

                _tempBody = _tempBody.Replace("[TBL_FOOTER]", "</table>");

                _tempBody = Regex.Replace(_tempBody, @"\[.*?\]", ""); //clear brackets which are not filled

                resultHTML = _tempBody;



            }
            catch (Exception)
            {
                return Content("Try again!!");
            }

            return Json(resultHTML, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SieveAnalysisTest_Edit(decimal SAH_ID)
        {
            ViewBag.Header = "Sieve Analysis Test";

            SIEVE_ANALYSIS_REPORT _objSieveRpt = new SIEVE_ANALYSIS_REPORT();
            _objSieveRpt = new DAL_SIEVE_ANALYSIS_TEST().VIEW_SIEVE_ANALYSIS_TEST(SAH_ID);

            SIEVE_ANALYSIS_TEST_EDIT _sieve = new SIEVE_ANALYSIS_TEST_EDIT(); 

            _sieve.SAH_ID = SAH_ID;
            _sieve.SAH_CODE = _objSieveRpt.SAH_CODE;

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _sieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _sieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _sieve.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");

            _sieve.BRANCH_CODE = _objSieveRpt.BRANCH_CODE;
            _sieve.PRODUCT_SIZE_CODE = Convert.ToString(_objSieveRpt.PRODUCT_SIZE_CODE);
            _sieve.LOCATION_CODE = Convert.ToString(_objSieveRpt.SAMPLING_FROM_CODE);
            _sieve.PROPOSE = _objSieveRpt.PROPOSE;

            
            _sieve.SAMPLING_DT = Convert.ToDateTime(_objSieveRpt.SAMPLING_DATE);
            _sieve.TESTING_DT = Convert.ToDateTime(_objSieveRpt.TESTING_DATE);

            _sieve.SAMPLING_DATE = _objSieveRpt.SAMPLING_DATE;
            _sieve.TESTING_DATE = _objSieveRpt.TESTING_DATE; 
              
            _sieve.REMARKS = _objSieveRpt.REMARKS; 
           
            _sieve.TESTED_BY_CODE = _objSieveRpt.TESTED_BY_CODE;
            _sieve.CHECKED_BY_CODE = _objSieveRpt.CHECKED_BY_CODE;

            _sieve.ANALYSIS_DTL_LIST = _objSieveRpt.sieveList;

            _sieve.TOTAL_SAMPLING_WT = _objSieveRpt.TOTAL_WT_GRAM;

            _sieve.FILE_PATH = _objSieveRpt.FILE_PATH;
            _sieve.IS_FILE_UPLOAD = _objSieveRpt.IS_FILE_UPLOAD;

            _sieve.IS_LOCKED = _objSieveRpt.IS_LOCKED;

            return PartialView("SieveAnalysisTest_Edit", _sieve);

        }



        [HttpPost]
        [SubmitButtonSelector(Name = "Update")]
        [ActionName("SieveAnalysisTest_Edit")]
        public ActionResult SieveAnalysisTest_Update(SIEVE_ANALYSIS_TEST_EDIT _objSieve)
        {
            ModelState["BRANCH_CODE"] = new ModelState();
            ModelState["PRODUCT_SIZE_CODE"] = new ModelState();
            ModelState["LOCATION_CODE"] = new ModelState();
            ModelState["UploadFile"] = new ModelState();

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_SIEVE_ANALYSIS_TEST().UPDATE_SIEVE_ANALYSIS_TEST(emp.Comp_Code, emp.BranchCode, emp.Employee_Code, _objSieve, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Sample No. : </b> <b style='color:red'> " + objMst.CODE + "</b> is updated successfully."), true);
                        return RedirectToAction("SieveAnalysisTest_Edit", "AgQualityMngt", new { SAH_ID = _objSieve.SAH_ID});
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _objSieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _objSieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _objSieve.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");


            return View(_objSieve);
        }



        [HttpPost]
        [SubmitButtonSelector(Name = "Lock")]
        [ActionName("SieveAnalysisTest_Edit")]
        public ActionResult SieveAnalysisTest_Lock(SIEVE_ANALYSIS_TEST_EDIT _objSieve)
        {
            ModelState["BRANCH_CODE"] = new ModelState();
            ModelState["PRODUCT_SIZE_CODE"] = new ModelState();
            ModelState["LOCATION_CODE"] = new ModelState();
            ModelState["UploadFile"] = new ModelState();

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_SIEVE_ANALYSIS_TEST().LOCK_SIEVE_ANALYSIS_TEST(emp.Comp_Code, emp.BranchCode, emp.Employee_Code, _objSieve.SAH_ID, _objSieve.SAH_CODE, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Sample No. : </b> <b style='color:red'> " + objMst.CODE + "</b> is locked successfully."), true);
                        return RedirectToAction("SieveAnalysisTest_Edit", "AgQualityMngt", new { SAH_ID = _objSieve.SAH_ID });
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }

            List<PRODUCT_SIZE_MST> sizeList = new DAL_Common().GetProductSizeList("AG", 39);
            _objSieve.PRODUCT_SIZ_LIST = new SelectList(sizeList, "ProductSize_Code", "Size");

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _objSieve.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _objSieve.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");


            return View(_objSieve);
        }


        [HttpPost]
        public ActionResult SieveAnalysisTest_Lock(decimal SAH_ID)
        {

            ResultMessage objMst;
            string result = new DAL_SIEVE_ANALYSIS_TEST().LOCK_SIEVE_ANALYSIS_TEST(emp.Comp_Code, emp.BranchCode, emp.Employee_Code, SAH_ID, "", out objMst);

            if (result == "")
            {
                result = "1";
            }
            else
            {
                result = "0";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }    
 
    }
}
