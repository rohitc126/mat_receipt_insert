using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DAL;
using System.Configuration; 
namespace eSGBIZ.Controllers
{
    public class MaterialController : BaseController
    {
        public ActionResult MaterialRequisitionEntry()
        {
            Material_Entry Entry = new Material_Entry();
            ViewBag.Header = "Raw Material Requisition Entry";

          

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            Entry.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

            List<BrandName_Master> brandNameMasterList = new DAL_Common().GeBrandNameMaster();
            Entry.BRAND_LIST = new SelectList(brandNameMasterList, "BRAND_ID", "BRAND_NAME");

            
            return View(Entry);
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("MaterialRequisitionEntry")]
        public ActionResult MaterialRequisitionEntry_Save(Material_Entry Entry)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_MATERIAL_REQUISITION_ENTRY().INSERT_MATERIAL_REQUISITION_ENTRY(emp.Employee_Code, Entry, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Material requisition inserted successfully. </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("MaterialRequisitionEntry", "Material");
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


            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            Entry.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

            List<BrandName_Master> brandNameMasterList = new DAL_Common().GeBrandNameMaster();
            Entry.BRAND_LIST = new SelectList(brandNameMasterList, "BRAND_ID", "BRAND_NAME");

           
               
            return View(Entry);
        }



        [Authorize]
        public ActionResult MaterialRequisitionList()
        { 
            ViewBag.Header = "Raw Material Requisition";
            Material_Req_List Entry = new Material_Req_List();

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name"); 

            return View(Entry);

        }

        public ActionResult _MaterialRequisition_List()
        {
            return PartialView("_MaterialRequisition_List");
        }

        [HttpPost]
        public ActionResult _MaterialRequisition_List(string brCode, DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            Material_Req_List _MatReq = new Material_Req_List();
            List<MATERIAL_REQ_DATA_LIST> MatReqDatas = new List<MATERIAL_REQ_DATA_LIST>();
            try
            {
                if (string.IsNullOrEmpty(brCode))
                {
                    brCode = "";
                }
                _MatReq.BRANCH_CODE = brCode;
                _MatReq.From_DT = fDate;
                _MatReq.To_DT = tDate;

                MatReqDatas = new DAL_MATERIAL_REQUISITION_ENTRY().Select_Material_Data_List(_MatReq);

                totalRow = MatReqDatas.Count();

            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error : _CNs_Data_List ", ex.Message);
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                MatReqDatas = MatReqDatas.
                    Where(x => x.REQUISITION_CODE.ToLower().Contains(searchValue.ToLower()) || x.Material_Name.ToLower().Contains(searchValue.ToLower())
                        || x.REQUISITION_DATE.ToLower().Contains(searchValue.ToLower())
                         ).ToList<MATERIAL_REQ_DATA_LIST>();




            }
            int totalRowFilter = MatReqDatas.Count();
            // sorting
            //sievesDatas = sievesDatas.OrderBy(sortColumnName + " " + sortDirection).ToList<SIEVE_ANALYSIS_DATA_LIST>();

            // Paging
            if (length == -1)
            {
                length = totalRow;
            }
            MatReqDatas = MatReqDatas.Skip(start).Take(length).ToList<MATERIAL_REQ_DATA_LIST>();

            var jsonResult = Json(new { data = MatReqDatas, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }




        [Authorize]
        public ActionResult MaterialRequisitionReceiptList()
        {
            ViewBag.Header = "Raw Material Receipt";
            Material_Req_List Entry = new Material_Req_List();

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            return View(Entry);

        }

        public ActionResult _MaterialRequisitionReceipt_List()
        {
            return PartialView("_MaterialRequisitionReceipt_List");
        }

        [HttpPost]
        public ActionResult _MaterialRequisitionReceipt_List(string brCode, DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            Material_Req_List _MatReq = new Material_Req_List();
            List<MATERIAL_REQ_DATA_LIST> MatReqDatas = new List<MATERIAL_REQ_DATA_LIST>();
            try
            {
                if (string.IsNullOrEmpty(brCode))
                {
                    brCode = "";
                }
                _MatReq.BRANCH_CODE = brCode;
                _MatReq.From_DT = fDate;
                _MatReq.To_DT = tDate;

                MatReqDatas = new DAL_MATERIAL_REQUISITION_ENTRY().Select_Material_Data_List(_MatReq);

                totalRow = MatReqDatas.Count();

            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error : _CNs_Data_List ", ex.Message);
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                MatReqDatas = MatReqDatas.
                    Where(x => x.REQUISITION_CODE.ToLower().Contains(searchValue.ToLower()) || x.Material_Name.ToLower().Contains(searchValue.ToLower())
                        || x.REQUISITION_DATE.ToLower().Contains(searchValue.ToLower())
                         ).ToList<MATERIAL_REQ_DATA_LIST>();




            }
            int totalRowFilter = MatReqDatas.Count();
            // sorting
            //sievesDatas = sievesDatas.OrderBy(sortColumnName + " " + sortDirection).ToList<SIEVE_ANALYSIS_DATA_LIST>();

            // Paging
            if (length == -1)
            {
                length = totalRow;
            }
            MatReqDatas = MatReqDatas.Skip(start).Take(length).ToList<MATERIAL_REQ_DATA_LIST>();

            var jsonResult = Json(new { data = MatReqDatas, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult MaterialReceiptEntry(decimal REQUISITION_ID)
        {
            MaterialReceiptEntry _objRAWMAt = new MaterialReceiptEntry();
            ViewBag.Header = "Raw Material Receipt Entry";


            _objRAWMAt = new DAL_MATERIAL_REQUISITION_ENTRY().Edit_RAW_MATERIAL_RECEIPT_ENTRY(REQUISITION_ID);

            List<VendorName_Master> VendorMasterList = new DAL_MATERIAL_REQUISITION_ENTRY().GetVendorList("0","SGX0205");
            _objRAWMAt.VENDOR_LIST = new SelectList(VendorMasterList, "VendorCode", "VendorName");

            return View(_objRAWMAt);

        }



        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("MaterialReceiptEntry")]
        public ActionResult MaterialReceiptEntry_Insert(MaterialReceiptEntry _RECEIPT)
        {


           
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_MATERIAL_REQUISITION_ENTRY().INSERT_MATERIAL_RECEIPT_ENTRY(emp.Employee_Code, _RECEIPT, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Material Receipt inserted successfully. </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("MaterialReceiptEntry", "Material");
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

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            _RECEIPT.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            _RECEIPT.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

            List<BrandName_Master> brandNameMasterList = new DAL_Common().GeBrandNameMaster();
            _RECEIPT.BRAND_LIST = new SelectList(brandNameMasterList, "BRAND_ID", "BRAND_NAME");
            return View(_RECEIPT);
        }

    }
}