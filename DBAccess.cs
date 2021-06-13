using NewIndustrial;
using ObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NewIndustrial
{
    public class DBAccess
    {
        private readonly string sconnection = System.Configuration.ConfigurationManager.ConnectionStrings["IndustrialConnectionString"].ConnectionString;
        private string GetUserName(decimal iUserID)
        {
            string sUserName = "";
            using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
            {
                var dbUser = dbContext.Users.SingleOrDefault(p => p.ID == iUserID);
                if (dbUser != null)
                    sUserName = dbUser.Username;
            }
            return sUserName;
        }
        private void LogEntry(tbl_Logs_OB objLog)
        {
            using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
            {
                tbl_Log dbELog = new tbl_Log()
                {
                    Activity = objLog.Activity,
                    ActivityPage = objLog.ActivityPage,
                    UserName = objLog.UserName,
                    ActivityParameters = objLog.ActivityParameters,
                    ActivityDateTime = DateTime.Now,
                    Description = objLog.Description,
                };

                dbContext.tbl_Logs.InsertOnSubmit(dbELog);
                dbContext.SubmitChanges();
            }
        }

        #region "Branch"
        public int AddBranch(string sBranchName, string BranchDesc
            , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Branch name exists before.
                    var iBranchName = dbContext.tbl_Branches.SingleOrDefault(p => p.BranchName == sBranchName);
                    if (iBranchName != null) //Branch name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    tbl_Branch objBranch = new tbl_Branch
                    {
                        BranchName = sBranchName,
                        BranchDesc = BranchDesc,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.tbl_Branches.InsertOnSubmit(objBranch);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "BranchName:" + sBranchName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Branch: " + sBranchName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchName:" + sBranchName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchName:" + sBranchName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateBranch(decimal BranchID, string sBranchName, string DeptDesc, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbBranch = dbContext.tbl_Branches.SingleOrDefault(p => p.BranchName == sBranchName && p.BranchID != BranchID);

                    if (dbBranch != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbBranchID = dbContext.tbl_Branches.SingleOrDefault(p => p.BranchID == BranchID);
                    dbBranchID.BranchName = sBranchName;
                    dbBranchID.BranchDesc = DeptDesc;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "BranchID:" + BranchID + ";BranchName:" + sBranchName + ";DeptDesc:" + DeptDesc;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Branch: " + sBranchName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchID:" + BranchID + ";BranchName:" + sBranchName + ";DeptDesc:" + DeptDesc;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchID:" + BranchID + ";BranchName:" + sBranchName + ";DeptDesc:" + DeptDesc;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int DeleteBranch(decimal BranchID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.tbl_Branches.SingleOrDefault(p => p.BranchID == BranchID);
                    if (dbID != null)
                    {
                        dbContext.tbl_Branches.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "BranchID:" + BranchID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Branch: " + dbID.BranchName + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchID:" + BranchID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "BranchID:" + BranchID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public List<tbl_Branch_OB> GetAllBranches()
        {
            List<tbl_Branch_OB> objBranches = new List<tbl_Branch_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbBranches = (from t in dbContext.tbl_Branches
                                      where t.BranchID != -1
                                      select new
                                      {
                                          t.BranchID,
                                          t.BranchName,
                                          t.BranchDesc
                                      }).OrderBy(p => p.BranchID);
                    foreach (var item in dbBranches)
                    {
                        tbl_Branch_OB objBranch = new tbl_Branch_OB();
                        objBranch.BranchID = item.BranchID;
                        objBranch.BranchName = item.BranchName;
                        objBranch.BranchEngDesc = item.BranchDesc;
                        objBranches.Add(objBranch);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Branch.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Branch.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objBranches;
        }
        public tbl_Branch_OB GetBranchByID(decimal BranchID)
        {
            tbl_Branch_OB objBranches = new tbl_Branch_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbBranches = (from t in dbContext.tbl_Branches
                                      where t.BranchID == BranchID && t.BranchID != -1
                                      select new
                                      {
                                          t.BranchID,
                                          t.BranchName,
                                          t.BranchDesc
                                      }).OrderBy(p => p.BranchID);
                    foreach (var item in dbBranches)
                    {
                        tbl_Branch_OB objBranch = new tbl_Branch_OB();
                        objBranch.BranchID = item.BranchID;
                        objBranch.BranchName = item.BranchName;
                        objBranch.BranchEngDesc = item.BranchDesc;
                        objBranches = objBranch;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Branch.aspx";
                LogObj.ActivityParameters = "BranchID:" + BranchID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Branch.aspx";
                LogObj.ActivityParameters = "BranchID:" + BranchID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objBranches;
        }
        #endregion

        #region "Category"
        public List<tbl_Category_OB> GetCategoriesList()
        {
            List<tbl_Category_OB> objCategorys = new List<tbl_Category_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Categories
                                    select new
                                    {
                                        t.ID,
                                        t.Name
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Category_OB objCategory = new tbl_Category_OB();
                        objCategory.ID = item.ID;
                        objCategory.Name = item.Name;
                        objCategorys.Add(objCategory);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Categorys.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Categorys.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objCategorys;
        }
        public int AddCategory(string sCategoryName
            , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Category name exists before.
                    var iCategoryName = dbContext.Categories.SingleOrDefault(p => p.Name == sCategoryName);
                    if (iCategoryName != null) //Category name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Category objCategory = new Category
                    {
                        Name = sCategoryName,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Categories.InsertOnSubmit(objCategory);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "CategoryName:" + sCategoryName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Category: " + sCategoryName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryName:" + sCategoryName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryName:" + sCategoryName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateCategory(decimal CategoryID, string sCategoryName, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbCategory = dbContext.Categories.SingleOrDefault(p => p.Name == sCategoryName && p.ID != CategoryID);

                    if (dbCategory != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbCategoryID = dbContext.Categories.SingleOrDefault(p => p.ID == CategoryID);
                    dbCategoryID.Name = sCategoryName;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "CategoryID:" + CategoryID + ";CategoryName:" + sCategoryName;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Category: " + sCategoryName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryID:" + CategoryID + ";CategoryName:" + sCategoryName;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryID:" + CategoryID + ";CategoryName:" + sCategoryName;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int DeleteCategory(decimal CategoryID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbMatExist2 = dbContext.Products.Where(p => p.ID_Category == CategoryID);
                    if (dbMatExist2.Count() != 0) { iResult = 0; return iResult; }
                    var dbID = dbContext.Categories.SingleOrDefault(p => p.ID == CategoryID);
                    if (dbID != null)
                    {
                        dbContext.Categories.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "CategoryID:" + CategoryID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Category: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryID:" + CategoryID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CategoryID:" + CategoryID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public tbl_Category_OB GetCategoryByID(decimal CategoryID)
        {
            tbl_Category_OB objCategorys = new tbl_Category_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbCategorys = (from t in dbContext.Categories
                                       where t.ID == CategoryID && t.ID != -1
                                       select new
                                       {
                                           t.ID,
                                           t.Name,
                                       }).OrderBy(p => p.ID);
                    foreach (var item in dbCategorys)
                    {
                        tbl_Category_OB objCategory = new tbl_Category_OB();
                        objCategory.ID = item.ID;
                        objCategory.Name = item.Name;
                        objCategorys = objCategory;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Category.aspx";
                LogObj.ActivityParameters = "CategoryID:" + CategoryID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Category.aspx";
                LogObj.ActivityParameters = "CategoryID:" + CategoryID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objCategorys;
        }
        #endregion

        #region "Clients"
        public List<tbl_Client_OB> GetClientsList()
        {
            List<tbl_Client_OB> objClients = new List<tbl_Client_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Clients
                                    select new
                                    {
                                        t.ID,
                                        t.Name,
                                        t.Phone,
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Client_OB objClient = new tbl_Client_OB();
                        objClient.ID = item.ID;
                        objClient.Name = item.Name;
                        objClient.Phone = item.Phone;

                        objClients.Add(objClient);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Clients.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Clients.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objClients;
        }

        public tbl_Client_OB GetClientByID(decimal ClientID)
        {
            tbl_Client_OB objClients = new tbl_Client_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClients = (from t in dbContext.Clients
                                     where t.ID == ClientID && t.ID != -1
                                     select new
                                     {
                                         t.ID,
                                         t.Name,
                                         t.Phone,
                                         t.Address,
                                         t.Fax,
                                         t.Email
                                     }).OrderBy(p => p.ID);
                    foreach (var item in dbClients)
                    {
                        tbl_Client_OB objClient = new tbl_Client_OB();
                        objClient.ID = item.ID;
                        objClient.Name = item.Name;
                        objClient.Phone = item.Phone;
                        objClient.Address = item.Address;
                        objClient.Fax = item.Fax;
                        objClient.Email = item.Email;
                        objClients = objClient;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Client.aspx";
                LogObj.ActivityParameters = "ClientID:" + ClientID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Client.aspx";
                LogObj.ActivityParameters = "ClientID:" + ClientID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objClients;
        }

        public int AddClient(string sClientName, string Phone, string Address, string Email, string Fax
           , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Client name exists before.
                    var iClientName = dbContext.Clients.SingleOrDefault(p => p.Name == sClientName);
                    if (iClientName != null) //Client name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Client objClient = new Client
                    {
                        Name = sClientName,
                        Phone = Phone,
                        Address = Address,
                        Email = Email,
                        Fax = Fax,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Clients.InsertOnSubmit(objClient);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ClientName:" + sClientName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Client: " + sClientName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientName:" + sClientName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientName:" + sClientName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdateClient(decimal ClientID, string sClientName, string Phone, string Address, string Email, string Fax, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = dbContext.Clients.SingleOrDefault(p => p.Name == sClientName && p.ID != ClientID);

                    if (dbClient != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbClientID = dbContext.Clients.SingleOrDefault(p => p.ID == ClientID);
                    dbClientID.Name = sClientName;
                    dbClientID.Address = Address;
                    dbClientID.Phone = Phone;
                    dbClientID.Email = Email;
                    dbClientID.Fax = Fax;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ClientID:" + ClientID + ";ClientName:" + sClientName + ";Address:" + Address;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Client: " + sClientName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientID:" + ClientID + ";ClientName:" + sClientName + ";Address:" + Address;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientID:" + ClientID + ";ClientName:" + sClientName + ";Address:" + Address;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteClient(decimal ClientID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.Clients.SingleOrDefault(p => p.ID == ClientID);
                    if (dbID != null)
                    {
                        dbContext.Clients.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ClientID:" + ClientID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Client: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientID:" + ClientID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ClientID:" + ClientID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        #endregion        

        #region "Department"

        public int AddDepartment(string sDepartmentName, string DepartmentDesc
            , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Department name exists before.
                    var iDepartmentName = dbContext.tbl_Departments.SingleOrDefault(p => p.DepartmentName == sDepartmentName);
                    if (iDepartmentName != null) //Department name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    tbl_Department objDepartment = new tbl_Department
                    {
                        DepartmentName = sDepartmentName,
                        DepartmentDesc = DepartmentDesc,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.tbl_Departments.InsertOnSubmit(objDepartment);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "DepartmentName:" + sDepartmentName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Department: " + sDepartmentName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentName:" + sDepartmentName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentName:" + sDepartmentName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdateDepartment(decimal DepartmentID, string sDepartmentName, string DeptDesc, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbDepartment = dbContext.tbl_Departments.SingleOrDefault(p => p.DepartmentName == sDepartmentName && p.DepartmentID != DepartmentID);

                    if (dbDepartment != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbDepartmentID = dbContext.tbl_Departments.SingleOrDefault(p => p.DepartmentID == DepartmentID);
                    dbDepartmentID.DepartmentName = sDepartmentName;
                    dbDepartmentID.DepartmentDesc = DeptDesc;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "DepartmentID:" + DepartmentID + ";DepartmentName:" + sDepartmentName + ";DeptDesc:" + DeptDesc;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Department: " + sDepartmentName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID + ";DepartmentName:" + sDepartmentName + ";DeptDesc:" + DeptDesc;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID + ";DepartmentName:" + sDepartmentName + ";DeptDesc:" + DeptDesc;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteDepartment(decimal DepartmentID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.tbl_Departments.SingleOrDefault(p => p.DepartmentID == DepartmentID);
                    if (dbID != null)
                    {
                        dbContext.tbl_Departments.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "DepartmentID:" + DepartmentID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Department: " + dbID.DepartmentName + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_Department_OB> GetAllDepartments()
        {
            List<tbl_Department_OB> objDepartments = new List<tbl_Department_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbDepartments = (from t in dbContext.tbl_Departments
                                         where t.DepartmentID != -1
                                         select new
                                         {
                                             t.DepartmentID,
                                             t.DepartmentName,
                                             t.DepartmentDesc
                                         }).OrderBy(p => p.DepartmentID);
                    foreach (var item in dbDepartments)
                    {
                        tbl_Department_OB objDepartment = new tbl_Department_OB();
                        objDepartment.DepartmentID = item.DepartmentID;
                        objDepartment.DepartmentName = item.DepartmentName;
                        objDepartment.DepartmentEngDesc = item.DepartmentDesc;
                        objDepartments.Add(objDepartment);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Department.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Department.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objDepartments;
        }

        public tbl_Department_OB GetDepartmentByID(decimal DepartmentID)
        {
            tbl_Department_OB objDepartments = new tbl_Department_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbDepartments = (from t in dbContext.tbl_Departments
                                         where t.DepartmentID == DepartmentID && t.DepartmentID != -1
                                         select new
                                         {
                                             t.DepartmentID,
                                             t.DepartmentName,
                                             t.DepartmentDesc
                                         }).OrderBy(p => p.DepartmentID);
                    foreach (var item in dbDepartments)
                    {
                        tbl_Department_OB objDepartment = new tbl_Department_OB();
                        objDepartment.DepartmentID = item.DepartmentID;
                        objDepartment.DepartmentName = item.DepartmentName;
                        objDepartment.DepartmentEngDesc = item.DepartmentDesc;
                        objDepartments = objDepartment;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Department.aspx";
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Department.aspx";
                LogObj.ActivityParameters = "DepartmentID:" + DepartmentID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objDepartments;
        }

        #endregion

        #region "Industrial"
        public int AddIndustrial(out int IndustrialID, int nProductID, float fQuantity
 , int? nCreatedBy, string sActivityPage)
        {
            IndustrialID = 0;
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    Industrial objInd = new Industrial
                    {
                        ID = dbContext.Industrials.Any() ? dbContext.Industrials.Max(p => p.ID) + 1 : 1,
                        ID_Product = nProductID,
                        Quantity = fQuantity,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    IndustrialID = objInd.ID;
                    dbContext.Industrials.InsertOnSubmit(objInd);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "nProductID:" + nProductID;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Ind Product: " + nProductID + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nProductID:" + nProductID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nProductID:" + nProductID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public List<tbl_Industrial_OB> GetIndustrialHistory(DateTime dtCreated, string ProductName, string sUserName, float fQuantity, int nRows, int nPageSize)
        {
            List<tbl_Industrial_OB> objIndustrial = new List<tbl_Industrial_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Industrials
                                 join t in dbContext.Products on t01.ID_Product equals t.ID
                                 join t1 in dbContext.Users on t01.CreatedBy equals t1.ID

                                 where t.Name.Contains(ProductName)
                                 && t01.Quantity == fQuantity
                                 && t01.CreatedDateTime == dtCreated
                                 && t1.Username.Contains(sUserName)
                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.CreatedDateTime,
                                     Product = t.Name,
                                     t01.CreatedBy
                                 }).OrderByDescending(p => p.CreatedDateTime);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Industrial_OB objLog = new tbl_Industrial_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.CreatedDateTime = (DateTime)item.CreatedDateTime;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.sUserName = GetUserName(Convert.ToDecimal(item.CreatedBy));
                        objIndustrial.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrial;
        }
        public List<tbl_Industrial_OB> GetIndustrialHistory_CP(int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_Industrial_OB> objIndustrial = new List<tbl_Industrial_OB>();
            int nTemp = 0;
            try
            {

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Industrials
                                 join t in dbContext.Products on t01.ID_Product equals t.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.CreatedDateTime,
                                     Product = t.Name,
                                     t01.CreatedBy
                                 }).OrderByDescending(p => p.CreatedDateTime);
                    nTemp = dbLog.Count();
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Industrial_OB objLog = new tbl_Industrial_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.CreatedDateTime = (DateTime)item.CreatedDateTime;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.sUserName = GetUserName(Convert.ToDecimal(item.CreatedBy));
                        objIndustrial.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objIndustrial;
        }
        public List<tbl_Industrial_OB> GetIndustrialHistorywithoutParams(int nRows, int nPageSize)
        {
            List<tbl_Industrial_OB> objIndustrial = new List<tbl_Industrial_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Industrials
                                 join t in dbContext.Products on t01.ID_Product equals t.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.CreatedDateTime,
                                     Product = t.Name,
                                     t01.CreatedBy
                                 }).OrderByDescending(p => p.CreatedDateTime);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Industrial_OB objLog = new tbl_Industrial_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.CreatedDateTime = (DateTime)item.CreatedDateTime;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.sUserName = GetUserName(Convert.ToDecimal(item.CreatedBy));
                        objIndustrial.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrial;
        }
        public tbl_Industrial_OB GetIndustrialByID(decimal IndustrialID)
        {
            tbl_Industrial_OB objIndustrials = new tbl_Industrial_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbIndustrials = (from t in dbContext.Industrials
                                         where t.ID == IndustrialID && t.ID != -1
                                         select new
                                         {
                                             t.ID,
                                             t.CreatedDateTime,
                                             t.CreatedBy,
                                             t.Quantity,
                                             t.ID_Product
                                         }).OrderBy(p => p.ID);
                    foreach (var item in dbIndustrials)
                    {
                        tbl_Industrial_OB objIndustrial = new tbl_Industrial_OB();
                        objIndustrial.ID = item.ID;
                        objIndustrial.CreatedDateTime = (DateTime)item.CreatedDateTime;
                        objIndustrial.CreatedBy = (int)item.CreatedBy;
                        objIndustrial.sUserName = GetUserName((int)item.CreatedBy);

                        objIndustrial.Quantity = float.Parse(item.Quantity.ToString());
                        objIndustrial.ID_Product = (int)item.ID_Product;
                        objIndustrial.Product = GetProductInfo((int)item.ID_Product).Name;
                        objIndustrials = objIndustrial;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "IndustrialID:" + IndustrialID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "IndustrialID:" + IndustrialID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrials;
        }
        public int GetIndustrialCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Industrials

                                 select new
                                 {
                                     t01.ID
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }
        #endregion

        #region "IndustrialLine"
        public int AddIndustrialItems(int nIndustrialID, int nMaterialID, float fQuantity
  , int? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    IndustrialLine objIndustrial = new IndustrialLine
                    {
                        ID_Industrial = nIndustrialID,
                        ID_Material = nMaterialID,

                        Quantity = fQuantity,

                    };
                    dbContext.IndustrialLines.InsertOnSubmit(objIndustrial);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "IndustrialID: " + nIndustrialID + "Material: " + nMaterialID;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Material: " + nMaterialID + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "IndustrialID: " + nIndustrialID + "Material: " + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "IndustrialID: " + nIndustrialID + "Material: " + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_IndustrialLine_OB> GetIndustrialLineByIndustrialID(int IndustrialID)
        {
            List<tbl_IndustrialLine_OB> objIndustrial = new List<tbl_IndustrialLine_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Industrials
                                 join t in dbContext.IndustrialLines on t01.ID equals t.ID_Industrial
                                 join t1 in dbContext.Materials on t.ID_Material equals t1.ID
                                 where
                                   t.ID_Industrial == IndustrialID

                                 select new
                                 {
                                     t01.ID,
                                     t.ID_Material,
                                     t.Quantity
                                 }).OrderBy(p => p.ID);
                    foreach (var item in dbLog)
                    {
                        tbl_IndustrialLine_OB objLog = new tbl_IndustrialLine_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Material = GetProductInfo((int)item.ID_Material).Name;
                        objIndustrial.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "IndustrialID:" + IndustrialID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Industrial.aspx";
                LogObj.ActivityParameters = "IndustrialID:" + IndustrialID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrial;
        }
        #endregion

        #region "IndustrialStandard"
        public int AddIndustrialStandard( int nMaterialID, int nProductID, float fQuantity
            , int? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                   
                    IndustrialStandard objInd = new IndustrialStandard
                    {
                        ID = dbContext.IndustrialStandards.Any() ? dbContext.IndustrialStandards.Max(p => p.ID) + 1 : 1,
                        ID_Product = nProductID,
                        ID_Material = nMaterialID,
                        Quantity = fQuantity,
                    };
                    dbContext.IndustrialStandards.InsertOnSubmit(objInd);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "nProductID:" + nProductID + "nMaterialID:" + nMaterialID;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Ind Product: " + nProductID + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nProductID:" + nProductID + "nMaterialID:" + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nProductID:" + nProductID + "nMaterialID:" + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteIndustrialStandard(decimal ProductID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.IndustrialStandards.SingleOrDefault(p => p.ID_Product == ProductID);
                    if (dbID != null)
                    {
                        dbContext.IndustrialStandards.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductID:" + ProductID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "IndustrialStandard for Product: " + ProductID + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_IndustrialStandard_OB> GetIndustrialStandardHistory( string ProductName, string sMatName, float fQuantity, int nRows, int nPageSize)
        {
            List<tbl_IndustrialStandard_OB> objIndustrialStandard = new List<tbl_IndustrialStandard_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards
                                 join t in dbContext.Products on t01.ID_Product equals t.ID
                                 join t1 in dbContext.Materials on t01.ID_Material equals t1.ID

                                 where t.Name.Contains(ProductName)
                                 && t01.Quantity == fQuantity
                                 && t1.Name.Contains(sMatName)
                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.ID_Material,
                                     Product = t.Name,
                                     Material = t1.Name
                                 }).OrderByDescending(p => p.ID);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_IndustrialStandard_OB objLog = new tbl_IndustrialStandard_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;                       
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.MaterialID = (int)item.ID_Material;
                        objLog.Material = item.Material;
                        objIndustrialStandard.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrialStandard;
        }
        public List<tbl_IndustrialStandard_OB> GetIndustrialStandardHistory_CP(int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_IndustrialStandard_OB> objIndustrialStandard = new List<tbl_IndustrialStandard_OB>();
            int nTemp = 0;
            try
            {

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards
                                 join t in dbContext.Products on t01.ID_Product equals t.ID
                                 join t1 in dbContext.Materials on t01.ID_Material equals t1.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.ID_Material,
                                     Product = t.Name,
                                     Material = t1.Name
                                 }).OrderByDescending(p => p.ID);
                    nTemp = dbLog.Count();
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_IndustrialStandard_OB objLog = new tbl_IndustrialStandard_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.MaterialID = (int)item.ID_Material;
                        objLog.Material = item.Material;
                        objIndustrialStandard.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objIndustrialStandard;
        }
        public List<tbl_IndustrialStandard_OB> GetIndustrialStandardHistorywithoutParams(int nRows, int nPageSize)
        {
            List<tbl_IndustrialStandard_OB> objIndustrialStandard = new List<tbl_IndustrialStandard_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards
                                 join t in dbContext.Products on t01.ID_Product equals t.ID
                                 join t1 in dbContext.Materials on t01.ID_Material equals t1.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.ID_Material,
                                     Product = t.Name,
                                     Material = t1.Name
                                 }).OrderByDescending(p => p.ID);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_IndustrialStandard_OB objLog = new tbl_IndustrialStandard_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Product = item.Product;
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.MaterialID = (int)item.ID_Material;
                        objLog.Material = item.Material;
                        objIndustrialStandard.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrialStandard;
        }
        public tbl_IndustrialStandard_OB GetIndustrialStandardByID(decimal IndustrialStandardID)
        {
            tbl_IndustrialStandard_OB objIndustrialStandards = new tbl_IndustrialStandard_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards
                                 where t01.ID == IndustrialStandardID
                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.ID_Material,
                                     
                                 }).OrderByDescending(p => p.ID);
                    foreach (var item in dbLog)
                    {
                        tbl_IndustrialStandard_OB objLog = new tbl_IndustrialStandard_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.MaterialID = (int)item.ID_Material;
                        
                        objIndustrialStandards = objLog;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "IndustrialStandardID:" + IndustrialStandardID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "IndustrialStandardID:" + IndustrialStandardID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrialStandards;
        }

        public List<tbl_IndustrialStandard_OB> GetIndustrialStandardByProdID(decimal ProdID)
        {
            List<tbl_IndustrialStandard_OB> objIndustrialStandards = new List<tbl_IndustrialStandard_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards
                                 join t in dbContext.Materials on t01.ID_Material equals t.ID
                                 where t01.ID_Product == ProdID
                                 select new
                                 {
                                     t01.ID,
                                     t01.Quantity,
                                     t01.ID_Product,
                                     t01.ID_Material,
                                     t.Name

                                 }).OrderByDescending(p => p.ID);
                    foreach (var item in dbLog)
                    {
                        tbl_IndustrialStandard_OB objLog = new tbl_IndustrialStandard_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.MatQuantity = float.Parse(item.Quantity.ToString());
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.MaterialID = (int)item.ID_Material;
                        objLog.Material = item.Name;
                        objIndustrialStandards.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "ProdID:" + ProdID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "ProdID:" + ProdID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objIndustrialStandards;
        }
        public int GetIndustrialStandardCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.IndustrialStandards

                                 select new
                                 {
                                     t01.ID
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "IndustrialStandard.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }
        #endregion

        #region "Materials"

        public List<tbl_Material_OB> GetMaterialsList()
        {
            List<tbl_Material_OB> objMaterials = new List<tbl_Material_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Materials
                                    select new
                                    {
                                        t.ID,
                                        t.Name,
                                        t.Quantity,
                                        t.Description
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Material_OB objPro = new tbl_Material_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Quantity = (float)item.Quantity;

                        objPro.Description = item.Description;
                        objMaterials.Add(objPro);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Materials.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Materials.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objMaterials;
        }

        public tbl_Material_OB GetMaterialInfo(int MaterialID)
        {
            tbl_Material_OB objMaterials = new tbl_Material_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Materials
                                    where t.ID == MaterialID
                                    select new
                                    {
                                        t.ID,
                                        t.Name,
                                        t.Quantity,
                                        t.Description
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Material_OB objPro = new tbl_Material_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Quantity = (float)item.Quantity;
                        objPro.Description = item.Description;
                        objMaterials = objPro;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Materials.aspx";
                LogObj.ActivityParameters = "ID:" + MaterialID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Materials.aspx";
                LogObj.ActivityParameters = "ID:" + MaterialID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objMaterials;
        }

        public int AddMaterial(string sMaterialName, string sDescription,  float fQuantity
   , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Material name exists before.
                    var iMaterialName = dbContext.Materials.SingleOrDefault(p => p.Name == sMaterialName);
                    if (iMaterialName != null) //Material name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Material objMaterial = new Material
                    {
                        Name = sMaterialName,
                        Description = sDescription,
                        Quantity = fQuantity,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Materials.InsertOnSubmit(objMaterial);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Material: " + sMaterialName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdateMaterial(decimal MaterialID, string sMaterialName, string sDescription, float fQuantity, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbMaterial = dbContext.Materials.SingleOrDefault(p => p.Name == sMaterialName && p.ID != MaterialID);

                    if (dbMaterial != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbMaterialID = dbContext.Materials.SingleOrDefault(p => p.ID == MaterialID);
                    dbMaterialID.Name = sMaterialName;
                    dbMaterialID.Description = sDescription;
                    dbMaterialID.Quantity = fQuantity;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "MaterialID:" + MaterialID + ";MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Material: " + sMaterialName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialID:" + MaterialID + ";MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialID:" + MaterialID + ";MaterialName:" + sMaterialName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteMaterial(decimal MaterialID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    //Check if Material related to sales or purchase
                    var dbMatExist = dbContext.PurchaseLines.Where(p => p.ID_Material == MaterialID);
                    if (dbMatExist.Count() != 0) { iResult = 0; return iResult; }

                    

                    var dbID = dbContext.Materials.SingleOrDefault(p => p.ID == MaterialID);
                    if (dbID != null)
                    {
                        dbContext.Materials.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "MaterialID:" + MaterialID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Material: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialID:" + MaterialID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "MaterialID:" + MaterialID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        #endregion     

        #region "PaymentType"
        public List<tbl_PaymentType_OB> GetPaymentTypes()
        {
            List<tbl_PaymentType_OB> objPayment = new List<tbl_PaymentType_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.PaymentTypes
                                    select new
                                    {
                                        t.ID,
                                        t.Name
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_PaymentType_OB objP = new tbl_PaymentType_OB();
                        objP.ID = item.ID;
                        objP.Name = item.Name;

                        objPayment.Add(objP);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "PaymentType.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "PaymentType.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPayment;
        }
        public tbl_PaymentType_OB GetPaymentTypebyID(decimal PaymentID)
        {
            tbl_PaymentType_OB objPayment = new tbl_PaymentType_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.PaymentTypes
                                    where t.ID == PaymentID
                                    select new
                                    {
                                        t.ID,
                                        t.Name
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_PaymentType_OB objP = new tbl_PaymentType_OB();
                        objP.ID = item.ID;
                        objP.Name = item.Name;

                        objPayment = objP;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "PaymentType.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "PaymentType.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPayment;
        }
        public int AddPaymentType(string sPaymentTypeName, decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the PaymentType name exists before.
                    var iPaymentTypeName = dbContext.PaymentTypes.SingleOrDefault(p => p.Name == sPaymentTypeName);
                    if (iPaymentTypeName != null) //PaymentType name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    PaymentType objPaymentType = new PaymentType
                    {
                        Name = sPaymentTypeName
                    };
                    dbContext.PaymentTypes.InsertOnSubmit(objPaymentType);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "PaymentTypeName:" + sPaymentTypeName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "PaymentType: " + sPaymentTypeName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeName:" + sPaymentTypeName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeName:" + sPaymentTypeName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdatePaymentType(decimal PaymentTypeID, string sPaymentTypeName, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbPaymentType = dbContext.PaymentTypes.SingleOrDefault(p => p.Name == sPaymentTypeName && p.ID != PaymentTypeID);

                    if (dbPaymentType != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbPaymentTypeID = dbContext.PaymentTypes.SingleOrDefault(p => p.ID == PaymentTypeID);
                    dbPaymentTypeID.Name = sPaymentTypeName;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID + ";PaymentTypeName:" + sPaymentTypeName;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "PaymentType: " + sPaymentTypeName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID + ";PaymentTypeName:" + sPaymentTypeName;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID + ";PaymentTypeName:" + sPaymentTypeName;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeletePaymentType(decimal PaymentTypeID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbMatExist2 = dbContext.Sales.Where(p => p.ID_PaymentType == PaymentTypeID);
                    if (dbMatExist2.Count() != 0) { iResult = 0; return iResult; }


                    var dbID = dbContext.PaymentTypes.SingleOrDefault(p => p.ID == PaymentTypeID);
                    if (dbID != null)
                    {
                        dbContext.PaymentTypes.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "PaymentType: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PaymentTypeID:" + PaymentTypeID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        #endregion        

        #region "Products"
        public List<tbl_Product_OB> GetProductsList()
        {
            List<tbl_Product_OB> objProducts = new List<tbl_Product_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Products
                                    select new
                                    {
                                        t.ID,
                                        t.Name,
                                        t.Price,
                                        t.Quantity,
                                        t.ID_Category,
                                        t.ID_Unit
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Product_OB objPro = new tbl_Product_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Price = (double)item.Price;
                        objPro.Quantity = (double)item.Quantity;
                        objPro.ID_Category = (int)item.ID_Category;
                        objPro.ID_Unit = (int)item.ID_Unit;
                        objProducts.Add(objPro);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Products.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Products.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objProducts;
        }
        public tbl_Product_OB GetProductInfo(int ProductID)
        {
            tbl_Product_OB objProducts = new tbl_Product_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Products
                                    where t.ID == ProductID
                                    select new
                                    {
                                        t.ID,
                                        t.Name,
                                        t.Price,
                                        t.Quantity,
                                        t.ID_Category,
                                        t.ID_Unit,
                                        t.Description
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Product_OB objPro = new tbl_Product_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Price = (double)item.Price;
                        objPro.Quantity = (double)item.Quantity;
                        objPro.ID_Category = (int)item.ID_Category;
                        objPro.ID_Unit = (int)item.ID_Unit;
                        objPro.Description = item.Description;
                        objProducts = objPro;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Products.aspx";
                LogObj.ActivityParameters = "ID:"+ProductID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Products.aspx";
                LogObj.ActivityParameters = "ID:" + ProductID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objProducts;
        }
        public int AddProduct(string sProductName, string sDescription, float fPrice, float fQuantity, int nUnitID, int nCategoryID
   , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Product name exists before.
                    var iProductName = dbContext.Products.SingleOrDefault(p => p.Name == sProductName);
                    if (iProductName != null) //Product name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Product objProduct = new Product
                    {
                        Name = sProductName,
                        Description = sDescription, 
                        Price = fPrice,
                        Quantity = fQuantity,
                        ID_Unit = nUnitID,
                        ID_Category = nCategoryID,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Products.InsertOnSubmit(objProduct);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductName:" + sProductName + ";Quantity:" + fQuantity;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Product: " + sProductName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductName:" + sProductName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductName:" + sProductName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateProduct(decimal ProductID, string sProductName, string sDescription, float fPrice, float fQuantity, int nUnitID, int nCategoryID, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbProduct = dbContext.Products.SingleOrDefault(p => p.Name == sProductName && p.ID != ProductID);
                    #region "Validation"
                    if (dbProduct != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbProductID = dbContext.Products.SingleOrDefault(p => p.ID == ProductID);
                    dbProductID.Name = sProductName;
                    dbProductID.Description = sDescription;
                    dbProductID.Price = fPrice;
                    dbProductID.Quantity = fQuantity;
                    dbProductID.ID_Unit = nUnitID;
                    dbProductID.ID_Category = nCategoryID;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductID:" + ProductID + ";ProductName:" + sProductName + ";Quantity:" + fQuantity;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Product: " + sProductName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID + ";ProductName:" + sProductName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID + ";ProductName:" + sProductName + ";Quantity:" + fQuantity;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateProductSalesQty(decimal ProductID, float fNewQuantity, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbProductID = dbContext.Products.SingleOrDefault(p => p.ID == ProductID);
                    double fOldQty = (double)dbProductID.Quantity;
                    
                    
                    dbProductID.Quantity = fOldQty - fNewQuantity;
                    
                    

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductID:" + ProductID;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Product: " + ProductID + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateProductPurchaseQty(decimal ProductID, float fNewQuantity, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbProductID = dbContext.Products.SingleOrDefault(p => p.ID == ProductID);
                    double fOldQty = (double)dbProductID.Quantity;
                    dbProductID.Quantity = fOldQty + fNewQuantity;
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductID:" + ProductID;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Product: " + ProductID + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int DeleteProduct(decimal ProductID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbMatExist2 = dbContext.SaleLines.Where(p => p.ID_Product == ProductID);
                    if (dbMatExist2.Count() != 0) { iResult = 0; return iResult; }

                    var dbID = dbContext.Products.SingleOrDefault(p => p.ID == ProductID);
                    if (dbID != null)
                    {
                        dbContext.Products.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "ProductID:" + ProductID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Product: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "ProductID:" + ProductID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_Product_OB> GetProductsStocks()
        {
            List<tbl_Product_OB> objPros = new List<tbl_Product_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbPRo = (from t0 in dbContext.Products
                                  join t in dbContext.Units on new { ID = Convert.ToInt32(t0.ID_Unit) } equals new { ID = t.ID }
                                  join t1 in dbContext.Categories on new { ID2 = Convert.ToInt32(t0.ID_Category) } equals new { ID2 = t1.ID }
                                  select new
                                  {
                                      t0.ID,
                                      t0.Name,
                                      t0.Description,
                                      t0.ID_Unit,
                                      t0.ID_Category,
                                      UnitName = t.Name,
                                      t0.Quantity,t0.Price,
                                      CategoryName = t1.Name
                                  }).OrderBy(p => p.ID);
                    foreach (var item in dbPRo)
                    {
                        tbl_Product_OB objPro = new tbl_Product_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Price = (double)item.Price;
                        objPro.Quantity = (double)item.Quantity;
                        objPro.Description = item.Description;
                        objPro.ID_Unit = (int)item.ID_Unit;
                        objPro.ID_Category = (int)item.ID_Category;
                        objPro.UnitName = item.UnitName;                        
                        objPro.CategoryName = item.CategoryName;


                        objPros.Add(objPro);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();

                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPros;
        }

        public int GetProductsStocksCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Products

                                 select new
                                 {
                                     t01.ID,
                                     t01.Name,
                                     t01.Description
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }

        public List<tbl_Product_OB> GetProductsStocks_CP(int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_Product_OB> objPros = new List<tbl_Product_OB>();
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbPRo = (from t0 in dbContext.Products
                                 join t in dbContext.Units on new { ID = Convert.ToInt32(t0.ID_Unit) } equals new { ID = t.ID }
                                 join t1 in dbContext.Categories on new { ID2 = Convert.ToInt32(t0.ID_Category) } equals new { ID2 = t1.ID }
                                 select new
                                 {
                                     t0.ID,
                                     t0.Name,
                                     t0.Description,
                                     t0.ID_Unit,
                                     t0.ID_Category,
                                     UnitName = t.Name,
                                     t0.Quantity,
                                     t0.Price,
                                     CategoryName = t1.Name
                                 }).OrderBy(p => p.ID);
                    nTemp = dbPRo.Count();
                    var dbLogSkip = dbPRo.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Product_OB objPro = new tbl_Product_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Price = (double)item.Price;
                        objPro.Quantity = (double)item.Quantity;
                        objPro.Description = item.Description;
                        objPro.ID_Unit = (int)item.ID_Unit;
                        objPro.ID_Category = (int)item.ID_Category;
                        objPro.UnitName = item.UnitName;
                        objPro.CategoryName = item.CategoryName;


                        objPros.Add(objPro);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();

                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objPros;
        }

        public List<tbl_Product_OB> GetProductsStocks_Filter(string Product, float Qty, float Price, string sDesc, string UnitName, string CategoryName, int nRows, int nPageSize)
        {
            List<tbl_Product_OB> objPros = new List<tbl_Product_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbPRo = (from t0 in dbContext.Products
                                 join t in dbContext.Units on new { ID = Convert.ToInt32(t0.ID_Unit) } equals new { ID = t.ID }
                                 join t1 in dbContext.Categories on new { ID2 = Convert.ToInt32(t0.ID_Category) } equals new { ID2 = t1.ID }
                                 where t0.Name.Contains(Product)
                                && t0.Quantity == Qty
                                && t0.Price == Price
                                && t0.Description.Contains(sDesc)
                                && t.Name.Contains(UnitName)
                                && t1.Name.Contains(CategoryName)
                                 select new
                                 {
                                     t0.ID,
                                     t0.Name,
                                     t0.Description,
                                     t0.ID_Unit,
                                     t0.ID_Category,
                                     UnitName = t.Name,
                                     t0.Quantity,
                                     t0.Price,
                                     CategoryName = t1.Name
                                 }).OrderBy(p => p.ID);
                    var dbLogSkip = dbPRo.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Product_OB objPro = new tbl_Product_OB();
                        objPro.ID = item.ID;
                        objPro.Name = item.Name;
                        objPro.Price = (double)item.Price;
                        objPro.Quantity = (double)item.Quantity;
                        objPro.Description = item.Description;
                        objPro.ID_Unit = (int)item.ID_Unit;
                        objPro.ID_Category = (int)item.ID_Category;
                        objPro.UnitName = item.UnitName;
                        objPro.CategoryName = item.CategoryName;


                        objPros.Add(objPro);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();

                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Stocks.aspx";
                LogObj.ActivityParameters = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPros;
        }
        #endregion

        #region "Purchase"
        public int AddPurchaseInvoice(out int PurchaseID, tbl_Purchase_OB objNewPurchase
  , int? nCreatedBy, string sActivityPage)
        {
            PurchaseID = 0;
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    Purchase objPurchase = new Purchase
                    {
                        ID = dbContext.Purchases.Any() ? dbContext.Purchases.Max(p => p.ID) + 1 : 1,
                        ID_S_Seller = objNewPurchase.ID_S_Seller,
                        ID_E_Buyer = objNewPurchase.ID_E_Buyer,
                        OrderDate = objNewPurchase.OrderDate,
                        CustomerPO = objNewPurchase.CustomerPO,
                        LoadingNote = objNewPurchase.LoadingNote,
                        CustomerName = objNewPurchase.CustomerName,
                        CustomerNo = objNewPurchase.CustomerNo,
                        ShipmentNo = objNewPurchase.ShipmentNo,
                        ShipTo = objNewPurchase.ShipTo,
                        DriverLicense = objNewPurchase.DriverLicense,
                        DriverName = objNewPurchase.DriverName,
                        SealNo = objNewPurchase.SealNo,
                        TruckNo = objNewPurchase.TruckNo,
                        CarrierName = objNewPurchase.CarrierName,
                        CarrierNo = objNewPurchase.CarrierNo,
                        BagsQty = objNewPurchase.BagsQty,
                        GrossWeight = objNewPurchase.GrossWeight,
                        TareWeight = objNewPurchase.TareWeight,
                        NetWeight = objNewPurchase.NetWeight
                    };
                    PurchaseID = objPurchase.ID;
                    dbContext.Purchases.InsertOnSubmit(objPurchase);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "CustomerPO:" + objNewPurchase.CustomerPO;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Purchase Invoice: " + objNewPurchase.CustomerPO + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CustomerPO:" + objNewPurchase.CustomerPO;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "CustomerPO:" + objNewPurchase.CustomerPO;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_Purchase_OB> GetPurchaseHistory(DateTime dtOrderDate, string sUserName, string sSupplierName,
            string sCustomerPO, int nRows, int nPageSize)
        {
            List<tbl_Purchase_OB> objPurchase = new List<tbl_Purchase_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Purchases
                                 join t in dbContext.Users on t01.ID_E_Buyer equals t.ID
                                 join t1 in dbContext.Suppliers on t01.ID_S_Seller equals t1.ID
                                 where t.Username.Contains(sUserName)
                                 && t01.OrderDate == dtOrderDate
                                 && t01.CustomerPO.Contains(sCustomerPO)
                                 && t1.Name.Contains(sSupplierName)
                                
                                  select new
                                 {
                                    t01.ID,
                                    t01.OrderDate,
                                    t.Username, 
                                    t01.CustomerPO,
                                   SupplierName = t1.Name
                                  }).OrderByDescending(p => p.OrderDate);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Purchase_OB objLog = new tbl_Purchase_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = ((DateTime)objLog.OrderDate).ToShortDateString();
                        objLog.UserName = item.Username;
                        objLog.CustomerPO = item.CustomerPO;
                        objLog.SupplierName = item.SupplierName;
                        objPurchase.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPurchase;
        }

        public List<tbl_Purchase_OB> GetPurchaseHistory_CP( int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_Purchase_OB> objPurchase = new List<tbl_Purchase_OB>();
            int nTemp = 0;
            try
            {
               
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Purchases
                                 join t in dbContext.Users on t01.ID_E_Buyer equals t.ID
                                 join t1 in dbContext.Suppliers on t01.ID_S_Seller equals t1.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.OrderDate,
                                     t.Username,
                                     t01.CustomerPO,
                                     SupplierName = t1.Name
                                 }).OrderByDescending(p => p.OrderDate);
                    nTemp = dbLog.Count();
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Purchase_OB objLog = new tbl_Purchase_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = ((DateTime)objLog.OrderDate).ToShortDateString();
                        objLog.UserName = item.Username;
                        objLog.CustomerPO = item.CustomerPO;
                        objLog.SupplierName = item.SupplierName;
                        objPurchase.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objPurchase;
        }
        public List<tbl_Purchase_OB> GetPurchaseHistorywithoutParams(DateTime dtOrderDate, string sUserName, string sSupplierName,
           string sCustomerPO, int nRows, int nPageSize)
        {
            List<tbl_Purchase_OB> objPurchase = new List<tbl_Purchase_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Purchases
                                 join t in dbContext.Users on t01.ID_E_Buyer equals t.ID
                                 join t1 in dbContext.Suppliers on t01.ID_S_Seller equals t1.ID
                                 where t.Username.Contains(sUserName)
                                 && t01.OrderDate == dtOrderDate
                                 && t01.CustomerPO.Contains(sCustomerPO)
                                 && t1.Name.Contains(sSupplierName)

                                 select new
                                 {
                                     t01.ID,
                                     t01.OrderDate,
                                     t.Username,
                                     t01.CustomerPO,
                                     SupplierName = t1.Name
                                 }).OrderByDescending(p => p.OrderDate);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Purchase_OB objLog = new tbl_Purchase_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = ((DateTime)objLog.OrderDate).ToShortDateString();
                        objLog.UserName = item.Username;
                        objLog.CustomerPO = item.CustomerPO;
                        objLog.SupplierName = item.SupplierName;
                        objPurchase.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPurchase;
        }

        public tbl_Purchase_OB GetPurchaseByID( decimal PurchaseID)
        {
            tbl_Purchase_OB objPurchases = new tbl_Purchase_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbPurchases = (from t in dbContext.Purchases
                                       where t.ID == PurchaseID && t.ID != -1
                                       select new
                                       {
                                           t.ID,
                                           t.OrderDate,
                                           t.ID_E_Buyer,
                                           t.ID_S_Seller,
                                           t.CustomerPO,
                                           t.LoadingNote,
                                           t.NetWeight,
                                           t.BagsQty,
                                           t.CarrierName,
                                           t.CarrierNo,
                                           t.CustomerName,
                                           t.CustomerNo,
                                           t.DriverLicense,
                                           t.DriverName,
                                           t.GrossWeight,
                                           t.SealNo,
                                           t.ShipmentNo,
                                           t.ShipTo,
                                           t.TareWeight,
                                           t.TruckNo
                                       }).OrderBy(p => p.ID);
                    foreach (var item in dbPurchases)
                    {
                        tbl_Purchase_OB objPurchase = new tbl_Purchase_OB();
                        objPurchase.ID = item.ID;
                        objPurchase.OrderDate = (DateTime)item.OrderDate;
                        objPurchase.sOrderDate = ((DateTime)objPurchase.OrderDate).ToShortDateString();
                        objPurchase.ID_E_Buyer = (int)item.ID_E_Buyer;
                        objPurchase.ID_S_Seller = (int)item.ID_S_Seller;
                        objPurchase.CustomerPO = item.CustomerPO;
                        objPurchase.LoadingNote = item.LoadingNote;

                        objPurchase.NetWeight = float.Parse(item.NetWeight.ToString());
                        objPurchase.BagsQty = float.Parse(item.BagsQty.ToString());
                        objPurchase.CarrierName = item.CarrierName;
                        objPurchase.CarrierNo = item.CarrierNo;
                        objPurchase.CustomerName = item.CustomerName;
                        objPurchase.CustomerNo = item.CustomerNo;

                        objPurchase.DriverLicense = item.DriverLicense;
                        objPurchase.DriverName = item.DriverName;
                        objPurchase.GrossWeight = float.Parse(item.GrossWeight.ToString());
                        objPurchase.SealNo = item.SealNo;
                        objPurchase.ShipmentNo = item.ShipmentNo;
                        objPurchase.ShipTo = item.ShipTo;
                        objPurchase.TruckNo = item.TruckNo;
                        objPurchase.TareWeight = float.Parse(item.TareWeight.ToString());
                        objPurchases = objPurchase;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "PurchaseID:" + PurchaseID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "PurchaseID:" + PurchaseID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPurchases;
        }

        public int GetPurchaseCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Purchases

                                 select new
                                 {
                                     t01.ID
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }
        #endregion

        #region "Purchase Line"
        public int AddPurchasesInvoiceItems(int nPurchaseID, int nMaterialID,  float fQuantity, float fValue, float fTotal,string sPack
  , int? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    PurchaseLine objPurchase = new PurchaseLine
                    {
                        ID_Order = nPurchaseID,
                        ID_Material = nMaterialID,
                        Total = fTotal,
                        Quantity = fQuantity,
                        Price = fValue,
                        Package = sPack
                    };
                    dbContext.PurchaseLines.InsertOnSubmit(objPurchase);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "PurchaseID: " + nPurchaseID + "Material: " + nMaterialID;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Material ID: " + nMaterialID + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PurchaseID: " + nPurchaseID + "Material: " + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "PurchaseID: " + nPurchaseID + "Material: " + nMaterialID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_PurchaseLine_OB> GetPurchaseLineByPurchaseID(int PurchaseID)
        {
            List<tbl_PurchaseLine_OB> objPurchase = new List<tbl_PurchaseLine_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Purchases
                                 join t in dbContext.PurchaseLines on t01.ID equals t.ID_Order
                                 join t1 in dbContext.Materials on t.ID_Material equals t1.ID
                                 where
                                   t.ID_Order == PurchaseID

                                 select new
                                 {
                                     t01.ID,
                                     t.ID_Material,
                                     t.Quantity,
                                     t.Price,
                                     t.Total,
                                     t.Package
                                 }).OrderBy(p => p.ID);
                    foreach (var item in dbLog)
                    {
                        tbl_PurchaseLine_OB objLog = new tbl_PurchaseLine_OB();
                        objLog.ID = item.ID;
                        objLog.MaterialID = (int)item.ID_Material;
                        objLog.ID_Material = (int)item.ID_Material;
                        objLog.Material = GetMaterialInfo((int)item.ID_Material).Name;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.Price = float.Parse(item.Price.ToString());
                        objLog.Total = float.Parse(item.Total.ToString());
                        objLog.Packaging = item.Package;
                        objPurchase.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "PurchaseID:" + PurchaseID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Purchase.aspx";
                LogObj.ActivityParameters = "PurchaseID:" + PurchaseID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objPurchase;
        }
        #endregion

        #region "Role/UserRole"
        public List<tbl_UserRole_OB> GetAllUserRoles()
        {
            List<tbl_UserRole_OB> objUserRoles = new List<tbl_UserRole_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUserRole = (from
                                      t in dbContext.Roles
                                      select new
                                      {

                                          t.ID,
                                          t.sRoleName,
                                          t.sRoleDescription,
                                          t.sToolTip,
                                          t.sRoleNameArabic
                                      }).OrderBy(p => p.ID);

                    foreach (var item in dbUserRole)
                    {
                        tbl_UserRole_OB objUserRole = new tbl_UserRole_OB();
                        objUserRole.nRoleID = (decimal)item.ID;
                        objUserRole.sRoleName = item.sRoleName;
                        objUserRole.sRoleDescription = item.sRoleDescription;
                        objUserRole.sToolTip = item.sToolTip;
                        objUserRole.sRoleNameArabic = item.sRoleNameArabic;
                        objUserRole.bSave = false;
                        objUserRole.bEdit = false;
                        objUserRole.bDelete = false;
                        objUserRole.bView = false;
                        objUserRoles.Add(objUserRole);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUserRoles;

        }
        public bool SaveUserRole(List<tbl_UserRole_OB> objUserRole)
        {
            using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
            {
                try
                {
                    dbContext.Connection.Open();
                    dbContext.Transaction = dbContext.Connection.BeginTransaction();
                    List<decimal> objUser = objUserRole.Select(p => p.nUserID).Distinct().ToList();
                    if (objUser.Count > 0)
                    {
                        try
                        {
                            //Delete All roles of that user
                            var objUI = dbContext.Userroles.Where(p => p.ID_User == objUser[0]);
                            foreach (var item in objUI)
                            {
                                dbContext.Userroles.DeleteOnSubmit(item);
                                dbContext.SubmitChanges();
                            }
                        }
                        catch (SqlException SQLEx)
                        {
                            //Record the transaction at the log data base
                            tbl_Logs_OB LogObj = new tbl_Logs_OB();
                            LogObj.Activity = "Delete All User Roles";
                            LogObj.ActivityPage = "UserPermission.aspx";
                            LogObj.ActivityParameters = "";
                            LogObj.UserName = GetUserName((decimal)objUserRole[0].CreatedBy);
                            LogObj.Description = SQLEx.Message;
                            LogEntry(LogObj);
                            return false;
                        }
                        catch (Exception Ex)
                        {
                            //Record the transaction at the log data base
                            tbl_Logs_OB LogObj = new tbl_Logs_OB();
                            LogObj.Activity = "Delete All User Roles";
                            LogObj.ActivityPage = "UserPermission.aspx";
                            LogObj.ActivityParameters = "";
                            LogObj.UserName = GetUserName((decimal)objUserRole[0].CreatedBy);
                            LogObj.Description = Ex.Message;
                            LogEntry(LogObj);
                            return false;
                        }
                    }

                    //Then Insert all roles
                    foreach (var item in objUserRole)
                    {
                        Userrole objUserRoleInsert = new Userrole
                        {
                            bSave = item.bSave,
                            bEdit = item.bEdit,
                            bDelete = item.bDelete,
                            bView = item.bView,
                            ID_Role = (int)item.nRoleID,
                            ID_User = (int)item.nUserID,
                            CreatedBy = (int)item.CreatedBy,
                            CreatedDateTime = DateTime.Now
                        };

                        dbContext.Userroles.InsertOnSubmit(objUserRoleInsert);
                        dbContext.SubmitChanges();
                    }
                    dbContext.Transaction.Commit();
                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj2 = new tbl_Logs_OB();
                    LogObj2.Activity = "Update";
                    LogObj2.ActivityPage = "User.aspx";
                    LogObj2.ActivityParameters = "UserID:" + objUser[0];
                    LogObj2.UserName = GetUserName((decimal)objUserRole[0].CreatedBy);
                    LogObj2.Description = "Permission for UserID: " + objUser[0] + " has been updated successfully.";
                    LogEntry(LogObj2);
                    return true;
                }

                catch (SqlException SQLEx)
                {
                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Add User Permission";
                    LogObj.ActivityPage = "UserPermission.aspx";
                    LogObj.ActivityParameters = "";
                    LogObj.UserName = GetUserName((decimal)objUserRole[0].CreatedBy);
                    LogObj.Description = SQLEx.Message;
                    LogEntry(LogObj);
                    return false;
                }
                catch (Exception Ex)
                {
                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Add User Permission";
                    LogObj.ActivityPage = "UserPermission.aspx";
                    LogObj.ActivityParameters = "";
                    LogObj.UserName = GetUserName((decimal)objUserRole[0].CreatedBy);
                    LogObj.Description = Ex.Message;
                    LogEntry(LogObj);
                    return false;
                }
            }
        }
        public List<tbl_UserRole_OB> GetUserRoleByUserIDAll(decimal nUserID)
        {
            List<tbl_UserRole_OB> objUserRoles = new List<tbl_UserRole_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var temp = (from t0 in dbContext.Userroles
                                join t in dbContext.Roles on t0.ID_Role equals t.ID
                                where t0.ID_User == nUserID
                                select new
                                {
                                    t0.ID_User,
                                    nRoleID = (System.Decimal?)t0.ID_Role,
                                    t.sRoleName,
                                    t0.bSave,
                                    t0.bEdit,
                                    t0.bDelete,
                                    t0.bView,
                                    t.sRoleDescription,
                                    t.sToolTip,
                                    t.sRoleNameArabic
                                }).OrderBy(p => p.ID_User);
                    if (temp.Count() == 0)
                    {
                        var dbUserRole = (from
                                          t in dbContext.Roles
                                          where t.sToolTip == "All"
                                          select new
                                          {
                                              nUserID = nUserID,
                                              t.ID,
                                              t.sRoleName,
                                              bSave = "false",
                                              bEdit = "false",
                                              bDelete = "false",
                                              bView = "false",
                                              t.sRoleDescription,
                                              t.sToolTip,
                                              t.sRoleNameArabic
                                          }).OrderBy(p => p.nUserID);

                        foreach (var item in dbUserRole)
                        {
                            tbl_UserRole_OB objUserRole = new tbl_UserRole_OB();
                            objUserRole.nUserID = item.nUserID;
                            objUserRole.nRoleID = (decimal)item.ID;
                            objUserRole.sRoleName = item.sRoleName;
                            objUserRole.bView = Convert.ToBoolean(item.bView);
                            objUserRole.bDelete = Convert.ToBoolean(item.bDelete);
                            objUserRole.bEdit = Convert.ToBoolean(item.bEdit);
                            objUserRole.bSave = Convert.ToBoolean(item.bSave);
                            objUserRole.sRoleDescription = item.sRoleDescription;
                            objUserRole.sToolTip = item.sToolTip;
                            objUserRole.sRoleNameArabic = item.sRoleNameArabic;
                            objUserRoles.Add(objUserRole);
                        }
                    }
                    else
                    {
                        foreach (var item in temp)
                        {
                            tbl_UserRole_OB objUserRole = new tbl_UserRole_OB();
                            objUserRole.nUserID = item.ID_User;
                            objUserRole.nRoleID = (decimal)item.nRoleID;
                            objUserRole.sRoleName = item.sRoleName;
                            objUserRole.bView = Convert.ToBoolean(item.bView);
                            objUserRole.bDelete = Convert.ToBoolean(item.bDelete);
                            objUserRole.bEdit = Convert.ToBoolean(item.bEdit);
                            objUserRole.bSave = Convert.ToBoolean(item.bSave);
                            objUserRole.sRoleDescription = item.sRoleDescription;
                            objUserRole.sToolTip = item.sToolTip;
                            objUserRole.sRoleNameArabic = item.sRoleNameArabic;
                            objUserRoles.Add(objUserRole);
                        }
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUserRoles;

        }
        public List<tbl_UserRole_OB> GetUserRoleByUserID(decimal nUserID)
        {
            List<tbl_UserRole_OB> objUserRoles = new List<tbl_UserRole_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUserRole = (from t0 in dbContext.Userroles
                                      join t in dbContext.Roles on t0.ID_Role equals t.ID
                                      where t0.ID_User == nUserID
                                      select new
                                      {
                                          t0.ID_User,
                                          nRoleID = (System.Decimal?)t0.ID_Role,
                                          t.sRoleName,
                                          t0.bSave,
                                          t0.bEdit,
                                          t0.bDelete,
                                          t0.bView,
                                          t.sRoleDescription,
                                          t.sToolTip,
                                          t.sRoleNameArabic
                                      }).OrderBy(p => p.ID_User);

                    foreach (var item in dbUserRole)
                    {
                        tbl_UserRole_OB objUserRole = new tbl_UserRole_OB();
                        objUserRole.nUserID = item.ID_User;
                        objUserRole.nRoleID = (decimal)item.nRoleID;
                        objUserRole.sRoleName = item.sRoleName;
                        objUserRole.bSave = item.bSave;
                        objUserRole.bEdit = item.bEdit;
                        objUserRole.bDelete = item.bDelete;
                        objUserRole.bView = item.bView;
                        objUserRole.sRoleDescription = item.sRoleDescription;
                        objUserRole.sToolTip = item.sToolTip;
                        objUserRole.sRoleNameArabic = item.sRoleNameArabic;
                        objUserRoles.Add(objUserRole);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUserRoles;

        }
        public List<tbl_UserRole_OB> GetUserRole2(decimal iUserID)
        {
            List<tbl_UserRole_OB> objList = new List<tbl_UserRole_OB>();
            try
            {
                using (IndustryEntityContextDataContext db = new IndustryEntityContextDataContext(sconnection))
                {
                    var listnew = (from t1 in db.Roles
                                   join t2 in db.Userroles on new { ID_Role = t1.ID } equals new { ID_Role = t2.ID_Role } into t2_join
                                   from t2 in t2_join.DefaultIfEmpty()
                                   where
                                     !
                                       (from Userroles in db.Userroles
                                        where
                                                Userroles.ID_User == iUserID
                                        select new
                                        {
                                            Userroles.ID_Role
                                        }).Contains(new { ID_Role = t1.ID })
                                   select new
                                   {

                                       nUserID = 0,
                                       t1.ID,
                                       t1.sRoleName,
                                       bSave = "false",
                                       bEdit = "false",
                                       bDelete = "false",
                                       bView = "false",
                                       t1.sRoleDescription,
                                       t1.sToolTip,
                                       t1.sRoleNameArabic
                                   }
                        );
                    //var List = (from t in db.Roles
                    //            where
                    //              !
                    //                (from t0 in db.Userroles
                    //                 where
                    //                   t0.ID_User == iUserID
                    //                 select new
                    //                 {
                    //                     t0.ID
                    //                 }).Contains(new { t.ID })
                                    
                    //                 && (t.sToolTip != "All" || t.sToolTip == null)
                    //            select new
                    //            {
                    //                nUserID = 0,
                    //                t.ID,                                    
                    //                t.sRoleName,
                    //                bSave = "false",
                    //                bEdit = "false",
                    //                bDelete = "false",
                    //                bView = "false",
                    //                t.sRoleDescription,
                    //                t.sToolTip,
                    //                t.sRoleNameArabic
                    //            }
                    //        );

                    foreach (var item in listnew)
                    {
                        tbl_UserRole_OB objPermission = new tbl_UserRole_OB();
                        objPermission.bView = Convert.ToBoolean(item.bView);
                        objPermission.bDelete = Convert.ToBoolean(item.bDelete);
                        objPermission.bEdit = Convert.ToBoolean(item.bEdit);
                        objPermission.bSave = Convert.ToBoolean(item.bSave);
                        objPermission.nRoleID = item.ID;
                        objPermission.sRoleName = item.sRoleName;
                        objPermission.sRoleDescription = item.sRoleDescription;
                        objPermission.sToolTip = item.sToolTip;
                        objPermission.sRoleNameArabic = item.sRoleNameArabic;
                        if (objPermission.nUserID == 0) { objPermission.nUserID = 0; }
                        else
                        {
                            objPermission.nUserID = (decimal)item.nUserID;
                        }
                        objList.Add(objPermission);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + iUserID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + iUserID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objList;

        }
        public List<tbl_UserRole_OB> GetUserRoleByUserPermID(decimal nUserID, decimal nRoleID)
        {
            List<tbl_UserRole_OB> objUserRoles = new List<tbl_UserRole_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUserRole = (from t0 in dbContext.Userroles
                                      join t in dbContext.Roles on t0.ID_Role equals t.ID
                                      where t0.ID_User == nUserID && t0.ID_Role == nRoleID
                                      select new
                                      {
                                          t0.ID,
                                          t0.ID_User,
                                          nRoleID = (System.Decimal?)t0.ID_Role,
                                          t.sRoleName,
                                          t0.bSave,
                                          t0.bEdit,
                                          t0.bDelete,
                                          t.sRoleDescription,
                                          t.sToolTip,
                                          t.sRoleNameArabic
                                      }).OrderBy(p => p.ID_User);

                    foreach (var item in dbUserRole)
                    {
                        tbl_UserRole_OB objUserRole = new tbl_UserRole_OB();
                        objUserRole.nUserID = item.ID_User;
                        objUserRole.nRoleID = (decimal)item.nRoleID;
                        objUserRole.sRoleName = item.sRoleName;
                        objUserRole.bSave = item.bSave;
                        objUserRole.bEdit = item.bEdit;
                        objUserRole.bDelete = item.bDelete;
                        objUserRole.sRoleDescription = item.sRoleDescription;
                        objUserRole.sToolTip = item.sToolTip;
                        objUserRole.sRoleNameArabic = item.sRoleNameArabic;
                        objUserRoles.Add(objUserRole);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID + ";nRoleID:" + nRoleID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "UserRole.aspx";
                LogObj.ActivityParameters = "UserID:" + nUserID + ";nRoleID:" + nRoleID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUserRoles;

        }
        #endregion

        #region "Settings"
        public tbl_Settings_OB GetSettingsByKey(string sKey)
        {
            tbl_Settings_OB objSett = new tbl_Settings_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSett = (from t in dbContext.Settings
                                  where t.Key == sKey
                                  select new
                                  {
                                      t.ID,
                                      t.Name,
                                      t.Key,
                                      t.Value,
                                  }).OrderBy(p => p.ID);
                    foreach (var item in dbSett)
                    {
                        tbl_Settings_OB objS = new tbl_Settings_OB();
                        objS.ID = item.ID;
                        objS.Name = item.Name;
                        objS.Key = item.Key;
                        objS.Value = item.Value;
                        objSett = objS;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Settings.aspx";
                LogObj.ActivityParameters = "Key:" + sKey;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Settings.aspx";
                LogObj.ActivityParameters = "Key:" + sKey;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSett;
        }
        #endregion

        #region "Sales"
        public int GetMaxInvoiceNumber()
        {
            tbl_Sale_OB objSett = new tbl_Sale_OB();
            int intIdt = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSett = (from t in dbContext.Sales
                                  select new
                                  {
                                      t.ID,                                     
                                  }).OrderBy(p => p.ID);
                    dbContext.Sales.OrderByDescending(u => u.ID).FirstOrDefault();
                    foreach (var item in dbSett)
                    {
                        tbl_Sale_OB objS = new tbl_Sale_OB();
                        objS.ID = item.ID;

                    }
                   intIdt = dbContext.Sales.Max(u => u.ID);
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return intIdt;
        }
       public int AddSalesInvoice(out int SaleID, DateTime? dtOrderDate, bool bIsPayed, int nPaymentTypeID, int nUserID, int nClientID
            ,string sInvoiceNumber, string sTaxNumber, float fInvoiceValue, float fDiscount, float fVAT, float fNetTotal, string sValueArabic 
  , int? nCreatedBy, string sActivityPage)
        {
            SaleID = 0;
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    Sale objSale = new Sale
                    {
                        ID  = dbContext.Sales.Any() ? dbContext.Sales.Max(p => p.ID) + 1 : 1,

                        OrderDate = dtOrderDate,
                        IsPayed = bIsPayed,
                        ID_PaymentType = nPaymentTypeID, 
                        ID_E_Seller = nUserID,
                        ID_C_Buyer = nClientID, 
                        InvoiceNumber = sInvoiceNumber, 
                        TaxNumber = sTaxNumber,
                        InvoiceValue = fInvoiceValue,
                        Discount = fDiscount,
                        VAT = fVAT,
                        NetTotal = fNetTotal,
                        ValueArabic = sValueArabic
                    };
                    SaleID = objSale.ID;
                    dbContext.Sales.InsertOnSubmit(objSale);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "sInvoiceNumber:" + sInvoiceNumber;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Sale Invoice: " + sInvoiceNumber + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "InvoiceNumber:" + sInvoiceNumber ;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "sInvoiceNumber:" + sInvoiceNumber;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_Sale_OB> GetSalesHistory(DateTime dtOrderDate, string sUserName, string sClientName,
    string sInvoiceNumber, int nRows, int nPageSize)
        {
            List<tbl_Sale_OB> objSales = new List<tbl_Sale_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Sales
                                 join t in dbContext.Users on t01.ID_E_Seller equals t.ID
                                 join t1 in dbContext.Clients on t01.ID_C_Buyer equals t1.ID
                                 where t.Username.Contains(sUserName)
                                 && t01.OrderDate == dtOrderDate
                                 && t1.Name.Contains(sClientName)

                                 select new
                                 {
                                     t01.ID,
                                     t01.OrderDate,
                                     t.Username,
                                     t01.InvoiceNumber,
                                     ClientName = t1.Name,
                                     t01.IsPayed
                                 }).OrderByDescending(p => p.OrderDate);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Sale_OB objLog = new tbl_Sale_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = objLog.OrderDate.ToShortDateString();
                        objLog.sUserName = item.Username;
                        objLog.InvoiceNumber = item.InvoiceNumber;
                        objLog.sClientName = item.ClientName;
                        objLog.IsPaid = (bool)item.IsPayed;
                        if ((bool)item.IsPayed) { objLog.sIsPaid = "تم الدفع"; }
                        else objLog.sIsPaid = "لم يتم";
                        objSales.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSales;
        }

        public List<tbl_Sale_OB> GetSalesHistory_CP(int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_Sale_OB> objSales = new List<tbl_Sale_OB>();
            int nTemp = 0;
            try
            {

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Sales
                                 join t in dbContext.Users on t01.ID_E_Seller equals t.ID
                                 join t1 in dbContext.Clients on t01.ID_C_Buyer equals t1.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.OrderDate,
                                     t.Username,
                                     t01.InvoiceNumber,
                                     ClientName = t1.Name,
                                     t01.IsPayed
                                 }).OrderByDescending(p => p.OrderDate);
                    nTemp = dbLog.Count();
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Sale_OB objLog = new tbl_Sale_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = objLog.OrderDate.ToShortDateString();
                        objLog.sUserName = item.Username;
                        objLog.InvoiceNumber = item.InvoiceNumber;
                        objLog.sClientName = item.ClientName;
                        objLog.IsPaid = (bool)item.IsPayed;
                        if ((bool)item.IsPayed) { objLog.sIsPaid = "تم الدفع"; }
                        else objLog.sIsPaid = "لم يتم";
                        objSales.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objSales;
        }
        public List<tbl_Sale_OB> GetSalesHistorywithoutParams(DateTime dtOrderDate, string sUserName, string sSupplierName,
           string sCustomerPO, int nRows, int nPageSize)
        {
            List<tbl_Sale_OB> objSales = new List<tbl_Sale_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Sales
                                 join t in dbContext.Users on t01.ID_E_Seller equals t.ID
                                 join t1 in dbContext.Clients on t01.ID_C_Buyer equals t1.ID

                                 select new
                                 {
                                     t01.ID,
                                     t01.OrderDate,
                                     t.Username,
                                     t01.InvoiceNumber,
                                     ClientName = t1.Name,
                                     t01.IsPayed
                                 }).OrderByDescending(p => p.OrderDate);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Sale_OB objLog = new tbl_Sale_OB();
                        objLog.ID = item.ID;
                        objLog.OrderDate = (DateTime)item.OrderDate;
                        objLog.sOrderDate = objLog.OrderDate.ToShortDateString();
                        objLog.sUserName = item.Username;
                        objLog.InvoiceNumber = item.InvoiceNumber;
                        objLog.sClientName = item.ClientName;
                        objLog.IsPaid = (bool)item.IsPayed;
                        if ((bool)item.IsPayed) { objLog.sIsPaid = "تم الدفع"; }
                        else objLog.sIsPaid = "لم يتم";
                        objSales.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSales;
        }

        public tbl_Sale_OB GetSalesByID(decimal SalesID)
        {
            tbl_Sale_OB objSaless = new tbl_Sale_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSaless = (from t in dbContext.Sales
                                    where t.ID == SalesID && t.ID != -1
                                    select new
                                    {
                                        t.ID,
                                        t.OrderDate,
                                        t.ID_C_Buyer,
                                        t.ID_E_Seller,
                                        t.IsPayed,
                                        t.ID_PaymentType,
                                        t.InvoiceNumber,
                                        t.TaxNumber,
                                        t.InvoiceValue,
                                        t.Discount,
                                        t.VAT,
                                        t.NetTotal,
                                        t.ValueArabic
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbSaless)
                    {
                        tbl_Sale_OB objSales = new tbl_Sale_OB();
                        objSales.ID = item.ID;
                        objSales.OrderDate = (DateTime)item.OrderDate;
                        objSales.sOrderDate = objSales.OrderDate.ToShortDateString();
                        objSales.ID_E_Buyer = (int)item.ID_C_Buyer;
                        objSales.ID_S_Seller = (int)item.ID_E_Seller;
                        objSales.IsPaid = (bool)item.IsPayed;
                        if ((bool)item.IsPayed) { objSales.sIsPaid = "تم الدفع"; }
                        else objSales.sIsPaid = "لم يتم";
                        objSales.ID_PaymentType = (int)item.ID_PaymentType;
                        objSales.TaxNumber = item.TaxNumber;
                        objSales.InvoiceNumber = item.InvoiceNumber;
                        objSales.InvoiceValue = float.Parse(item.InvoiceValue.ToString());
                        objSales.Discount = float.Parse(item.Discount.ToString());
                        objSales.VAT = float.Parse(item.VAT.ToString());
                        objSales.NetTotal = float.Parse(item.NetTotal.ToString());
                        objSales.ValueArabic = item.ValueArabic;
                        objSaless = objSales;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "SalesID:" + SalesID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "SalesID:" + SalesID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSaless;
        }

        public int GetSalesCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Sales

                                 select new
                                 {
                                     t01.ID
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }
        #endregion

        #region "Sales Line"
        public int AddSalesInvoiceItems( int nSaleID, int nProductID, int nUnitID, float fQuantity, float fValue, float fTotal
  , int? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    SaleLine objSale = new SaleLine
                    {
                        ID_Order= nSaleID,
                        ID_Product = nProductID,
                        Total = fTotal,
                        Quantity = fQuantity,
                        Price = fValue,
                        UnitID = nUnitID
                    };
                    dbContext.SaleLines.InsertOnSubmit(objSale);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "SaleID: " + nSaleID+"Product: "+nProductID;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "nProductID: " + nProductID + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SaleID: " + nSaleID + "Product: " + nProductID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SaleID: " + nSaleID + "Product: " + nProductID;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public List<tbl_SaleLine_OB> GetSalesLineBySalesID(int SalesID)
        {
            List<tbl_SaleLine_OB> objSales = new List<tbl_SaleLine_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.Sales
                                 join t in dbContext.SaleLines on t01.ID equals t.ID_Order
                                 join t1 in dbContext.Products on t.ID_Product equals t1.ID
                                 where
                                   t.ID_Order == SalesID

                                 select new
                                 {
                                     t01.ID,
                                     t.ID_Product,
                                     t.ID_Order,
                                     t.UnitID,
                                     t.Quantity,
                                     t.Price,
                                     t.Total                                    
                                 }).OrderBy(p => p.ID);
                    foreach (var item in dbLog)
                    {
                        tbl_SaleLine_OB objLog = new tbl_SaleLine_OB();
                        objLog.ID = item.ID;
                        objLog.ID_Product = (int)item.ID_Product;
                        objLog.ID_Order = (int)item.ID_Order;
                        objLog.Unit = GetUnitByID((int)item.UnitID).Name;
                        objLog.Quantity = float.Parse(item.Quantity.ToString());
                        objLog.UnitPrice = float.Parse(item.Price.ToString());
                        objLog.TotalPrice = float.Parse(item.Total.ToString());
                        objLog.Product = GetProductInfo((int)item.ID_Product).Name;
                        objSales.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "SalesID:" + SalesID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Sales.aspx";
                LogObj.ActivityParameters = "SalesID:" + SalesID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSales;
        }
        #endregion

        #region "Settings"
        public List<tbl_Settings_OB> GetAllSettings()
        {
            List<tbl_Settings_OB> objSettings = new List<tbl_Settings_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSettings = (from t in dbContext.Settings
                                      where t.ID != -1
                                      select new
                                      {
                                          t.ID,
                                          t.Name,
                                          t.Value
                                      }).OrderBy(p => p.ID);
                    foreach (var item in dbSettings)
                    {
                        tbl_Settings_OB objSetting = new tbl_Settings_OB();
                        objSetting.ID = item.ID;
                        objSetting.Name = item.Name;
                        objSetting.Value = item.Value;
                        objSettings.Add(objSetting);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Setting.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Setting.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSettings;
        }
        public tbl_Settings_OB GetSettingByID(int nID)
        {
            tbl_Settings_OB objSettings = new tbl_Settings_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSettings = (from t in dbContext.Settings
                                      where t.ID != -1 && t.ID == nID
                                      select new
                                      {
                                          t.ID,
                                          t.Name,
                                          t.Value
                                      }).OrderBy(p => p.ID);
                    foreach (var item in dbSettings)
                    {
                        tbl_Settings_OB objSetting = new tbl_Settings_OB();
                        objSetting.ID = item.ID;
                        objSetting.Name = item.Name;
                        objSetting.Value = item.Value;
                        objSettings = objSetting;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Setting.aspx";
                LogObj.ActivityParameters = "ID"+nID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Setting.aspx";
                LogObj.ActivityParameters = "ID" + nID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSettings;
        }

        public int UpdateSetting(int SettingID, string sValue, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                                       
                    var dbSettingID = dbContext.Settings.SingleOrDefault(p => p.ID == SettingID);

                    dbSettingID.Value = sValue;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "SettingID:" + SettingID + ";Value:" + sValue;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Setting: " + SettingID + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SettingID:" + SettingID + ";Value:" + sValue;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SettingID:" + SettingID + ";Value:" + sValue;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        #endregion

        #region "Suppliers"
        public List<tbl_Supplier_OB> GetSuppliersList()
        {
            List<tbl_Supplier_OB> objSuppliers = new List<tbl_Supplier_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSupplier = (from t in dbContext.Suppliers
                                      select new
                                      {
                                          t.ID,
                                          t.Name,
                                          t.Phone,
                                      }).OrderBy(p => p.ID);
                    foreach (var item in dbSupplier)
                    {
                        tbl_Supplier_OB objSupplier = new tbl_Supplier_OB();
                        objSupplier.ID = item.ID;
                        objSupplier.Name = item.Name;
                        objSupplier.Phone = item.Phone;

                        objSuppliers.Add(objSupplier);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Suppliers.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Suppliers.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSuppliers;
        }

        public tbl_Supplier_OB GetSupplierByID(decimal SupplierID)
        {
            tbl_Supplier_OB objSuppliers = new tbl_Supplier_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSuppliers = (from t in dbContext.Suppliers
                                       where t.ID == SupplierID && t.ID != -1
                                       select new
                                       {
                                           t.ID,
                                           t.Name,
                                           t.Phone,
                                           t.Address,
                                           t.Fax,
                                           t.Email
                                       }).OrderBy(p => p.ID);
                    foreach (var item in dbSuppliers)
                    {
                        tbl_Supplier_OB objSupplier = new tbl_Supplier_OB();
                        objSupplier.ID = item.ID;
                        objSupplier.Name = item.Name;
                        objSupplier.Phone = item.Phone;
                        objSupplier.Address = item.Address;
                        objSupplier.Fax = item.Fax;
                        objSupplier.Email = item.Email;
                        objSuppliers = objSupplier;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Supplier.aspx";
                LogObj.ActivityParameters = "SupplierID:" + SupplierID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Supplier.aspx";
                LogObj.ActivityParameters = "SupplierID:" + SupplierID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objSuppliers;
        }

        public int AddSupplier(string sSupplierName, string Phone, string Address, string Email, string Fax
           , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Supplier name exists before.
                    var iSupplierName = dbContext.Suppliers.SingleOrDefault(p => p.Name == sSupplierName);
                    if (iSupplierName != null) //Supplier name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Supplier objSupplier = new Supplier
                    {
                        Name = sSupplierName,
                        Phone = Phone,
                        Address = Address,
                        Email = Email,
                        Fax = Fax,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Suppliers.InsertOnSubmit(objSupplier);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "SupplierName:" + sSupplierName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Supplier: " + sSupplierName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierName:" + sSupplierName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierName:" + sSupplierName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdateSupplier(decimal SupplierID, string sSupplierName, string Phone, string Address, string Email, string Fax, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbSupplier = dbContext.Suppliers.SingleOrDefault(p => p.Name == sSupplierName && p.ID != SupplierID);

                    if (dbSupplier != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbSupplierID = dbContext.Suppliers.SingleOrDefault(p => p.ID == SupplierID);
                    dbSupplierID.Name = sSupplierName;
                    dbSupplierID.Address = Address;
                    dbSupplierID.Phone = Phone;
                    dbSupplierID.Email = Email;
                    dbSupplierID.Fax = Fax;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "SupplierID:" + SupplierID + ";SupplierName:" + sSupplierName + ";Address:" + Address;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Supplier: " + sSupplierName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierID:" + SupplierID + ";SupplierName:" + sSupplierName + ";Address:" + Address;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierID:" + SupplierID + ";SupplierName:" + sSupplierName + ";Address:" + Address;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteSupplier(decimal SupplierID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.Suppliers.SingleOrDefault(p => p.ID == SupplierID);
                    if (dbID != null)
                    {
                        dbContext.Suppliers.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "SupplierID:" + SupplierID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Supplier: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierID:" + SupplierID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "SupplierID:" + SupplierID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        #endregion        

        #region "Units"
        public List<tbl_Unit_OB> GetUnitsList()
        {
            List<tbl_Unit_OB> objUnits = new List<tbl_Unit_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbClient = (from t in dbContext.Units
                                    select new
                                    {
                                        t.ID,
                                        t.Name
                                    }).OrderBy(p => p.ID);
                    foreach (var item in dbClient)
                    {
                        tbl_Unit_OB objUnit = new tbl_Unit_OB();
                        objUnit.ID = item.ID;
                        objUnit.Name = item.Name;
                        objUnits.Add(objUnit);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Units.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Units.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUnits;
        }

        public int AddUnit(string sUnitName
            , decimal? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"
                    //Check if the Unit name exists before.
                    var iUnitName = dbContext.Units.SingleOrDefault(p => p.Name == sUnitName);
                    if (iUnitName != null) //Unit name already exists
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    Unit objUnit = new Unit
                    {
                       Name = sUnitName,
                        CreatedBy = (int)nCreatedBy,
                        CreatedDateTime = DateTime.Now
                    };
                    dbContext.Units.InsertOnSubmit(objUnit);
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "UnitName:" + sUnitName;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "Unit: " + sUnitName + " has been added successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitName:" + sUnitName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitName:" + sUnitName;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int UpdateUnit(decimal UnitID, string sUnitName, decimal? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                #region "Validation"

                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUnit = dbContext.Units.SingleOrDefault(p => p.Name == sUnitName && p.ID != UnitID);

                    if (dbUnit != null) //The updated name exists before with different ID
                    {
                        iResult = 0;
                        return iResult;
                    }
                    #endregion

                    var dbUnitID = dbContext.Units.SingleOrDefault(p => p.ID == UnitID);
                    dbUnitID.Name = sUnitName;

                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "UnitID:" + UnitID + ";UnitName:" + sUnitName;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "Unit: " + sUnitName + " has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitID:" + UnitID + ";UnitName:" + sUnitName ;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitID:" + UnitID + ";UnitName:" + sUnitName;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public int DeleteUnit(decimal UnitID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {

                    var dbID = dbContext.Units.SingleOrDefault(p => p.ID == UnitID);
                    if (dbID != null)
                    {
                        dbContext.Units.DeleteOnSubmit(dbID);

                        dbContext.SubmitChanges();
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "UnitID:" + UnitID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "Unit: " + dbID.Name + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitID:" + UnitID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UnitID:" + UnitID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }

        public tbl_Unit_OB GetUnitByID(decimal UnitID)
        {
            tbl_Unit_OB objUnits = new tbl_Unit_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUnits = (from t in dbContext.Units
                                   where t.ID == UnitID && t.ID != -1
                                   select new
                                   {
                                       t.ID,
                                       t.Name,
                                   }).OrderBy(p => p.ID);
                    foreach (var item in dbUnits)
                    {
                        tbl_Unit_OB objUnit = new tbl_Unit_OB();
                        objUnit.ID = item.ID;
                        objUnit.Name = item.Name;
                        objUnits = objUnit;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Unit.aspx";
                LogObj.ActivityParameters = "UnitID:" + UnitID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "Unit.aspx";
                LogObj.ActivityParameters = "UnitID:" + UnitID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUnits;
        }
        #endregion

        #region "User"
        public tbl_User_OB GetUserFullName(decimal nUserID)
        {
            tbl_User_OB objUsers = new tbl_User_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUsers = from t in dbContext.Users
                                  where
                                    t.ID == nUserID
                                  select new
                                  {
                                      t.ID,
                                      t.FirstName,
                                      t.LastName,
                                      t.DepartmentID,
                                      t.Phone,
                                      t.Username,
                                      t.Password,
                                      t.bEnable,
                                      t.bAdmin,
                                      t.Email,
                                      t.IdentityNo,
                                      t.BranchID
                                  };
                    foreach (var item in dbUsers)
                    {
                        tbl_User_OB objUser = new tbl_User_OB();
                        objUser.UserID = item.ID;
                        objUser.FirstName = item.FirstName;
                        objUser.LastName = item.LastName;
                        objUser.Phone = item.Phone;
                        objUser.UserName = item.Username;
                        objUser.sPassword = item.Password;

                        if (item.DepartmentID == null)
                        {
                            objUser.DepartmentID = -1;
                        }
                        else
                        {
                            objUser.DepartmentID = (decimal)item.DepartmentID;
                        }

                        if (item.BranchID == null)
                        {
                            objUser.BranchID = -1;
                        }
                        else
                        {
                            objUser.BranchID = (decimal)item.BranchID;
                        }
                        objUser.bEnable = item.bEnable;
                        objUser.EmailAddress = item.Email;
                        objUser.IDNo = item.IdentityNo;
                        objUsers = objUser;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "nUserID:" + nUserID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "nUserID:" + nUserID; ;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUsers;
        }
        public List<tbl_User_OB> GetUsersList()
        {
            List<tbl_User_OB> objUsers = new List<tbl_User_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUser = (from t in dbContext.Users
                                  select new
                                  {
                                      t.ID,
                                      t.FirstName,
                                      t.LastName,
                                      t.DepartmentID,
                                      t.Phone,
                                      t.Username,
                                      t.Password,
                                      t.bEnable,
                                      t.bAdmin,
                                      t.Email,
                                      t.IdentityNo,
                                      t.BranchID
                                  }).OrderBy(p => p.ID);
                    foreach (var item in dbUser)
                    {
                        tbl_User_OB objUser = new tbl_User_OB();
                        objUser.UserID = item.ID;
                        objUser.FirstName = item.FirstName;
                        objUser.LastName = item.LastName;
                        objUser.Phone = item.Phone;
                        objUser.UserName = item.Username;
                        objUser.sPassword = item.Password;
                        if (item.DepartmentID == null)
                        {
                            objUser.DepartmentID = -1;
                        }
                        else
                        {
                            objUser.DepartmentID = (decimal)item.DepartmentID;
                        }
                        if (item.BranchID == null)
                        {
                            objUser.BranchID = -1;
                        }
                        else
                        {
                            objUser.BranchID = (decimal)item.BranchID;
                        }

                        objUser.bEnable = item.bEnable;
                        objUser.EmailAddress = item.Email;
                        objUser.IDNo = item.IdentityNo;
                        objUsers.Add(objUser);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUsers;
        }
        public tbl_User_OB GetUserByID(decimal UserID)
        {
            tbl_User_OB objUsers = new tbl_User_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUser = (from t in dbContext.Users
                                  where t.ID == UserID
                                  select new
                                  {
                                      t.ID,
                                      t.FirstName,
                                      t.LastName,
                                      t.DepartmentID,
                                      t.Phone,
                                      t.Username,
                                      t.Password,
                                      t.bEnable,
                                      t.bAdmin,
                                      t.Email,
                                      t.IdentityNo,
                                      t.BranchID
                                  }).OrderBy(p => p.ID);
                    foreach (var item in dbUser)
                    {
                        tbl_User_OB objUser = new tbl_User_OB();
                        objUser.UserID = item.ID;
                        objUser.FirstName = item.FirstName;
                        objUser.LastName = item.LastName;
                        objUser.Phone = item.Phone;
                        objUser.UserName = item.Username;
                        objUser.sPassword = item.Password;

                        if (item.DepartmentID == null)
                        {
                            objUser.DepartmentID = -1;
                        }
                        else
                        {
                            objUser.DepartmentID = (decimal)item.DepartmentID;
                        }
                        if (item.BranchID == null)
                        {
                            objUser.BranchID = -1;
                        }
                        else
                        {
                            objUser.BranchID = (decimal)item.BranchID;
                        }
                        objUser.bEnable = item.bEnable;
                        objUser.bAdmin = item.bAdmin;
                        objUser.EmailAddress = item.Email;
                        objUser.IDNo = item.IdentityNo;
                        objUsers = objUser;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "UserID:" + UserID;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "UserID:" + UserID;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objUsers;
        }
        public int AddUser(out decimal UserID, string FirstName, string LastName, int? DepCode, string Login, string Password
    , bool bAdmin, string Mobile, string EmailAddress, bool bEnable, string IDNo, int? nBranchID
    , int? nCreatedBy, string sActivityPage)
        {
            int iResult = 1;
            UserID = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"


                    var sEmail = dbContext.Users.SingleOrDefault(p => p.Email == EmailAddress);
                    if (sEmail != null)
                    {
                        iResult = -3;
                        UserID = -3;
                        return iResult;
                    }

                    var sPhone = dbContext.Users.SingleOrDefault(p => p.Phone == Mobile);
                    if (sPhone != null)
                    {
                        iResult = -4;
                        UserID = -4;
                        return iResult;
                    }

                    var sIDNo = dbContext.Users.SingleOrDefault(p => p.IdentityNo == IDNo);
                    if (sIDNo != null)
                    {
                        iResult = -5;
                        UserID = -5;
                        return iResult;
                    }
                    #endregion



                    User objUser = new User
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        DepartmentID = DepCode,
                        Username = Login,
                        Password = Password,
                        bAdmin = bAdmin,
                        Phone = Mobile,
                        Email = EmailAddress,
                        bEnable = bEnable,
                        IdentityNo = IDNo,
                        BranchID = nBranchID,
                        CreatedBy = nCreatedBy,
                        CreatedDateTime = DateTime.Now,
                    };

                    dbContext.Users.InsertOnSubmit(objUser);
                    dbContext.SubmitChanges();
                    UserID = objUser.ID;
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Save";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepartmentCode:" + DepCode + ";BranchID:" + nBranchID + ";Login:" + Login + ";sPassword:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                    LogObj.UserName = GetUserName((decimal)nCreatedBy);
                    LogObj.Description = "User: " + Login + "has been added successfully.";
                    LogEntry(LogObj);


                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepartmentCode:" + DepCode + ";BranchID:" + nBranchID + ";Login:" + Login + ";sPassword:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Save";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepartmentCode:" + DepCode+";BranchID:"+nBranchID + ";Login:" + Login + ";sPassword:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                LogObj.UserName = GetUserName((decimal)nCreatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int UpdateUser(decimal UserID, string FirstName, string LastName, int? DepCode, string Login, string Password
            , bool bAdmin, string Mobile, string EmailAddress, bool bEnable, string IDNo, int? nBranchID
            , int? nUpdatedBy, string sActivityPage)
        {
            int iResult = 1;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    #region "Validation"


                    var sEmail = dbContext.Users.SingleOrDefault(p => p.Email == EmailAddress && p.ID != UserID);
                    if (sEmail != null)
                    {
                        iResult = -3;
                        return iResult;
                    }

                    var sPhone = dbContext.Users.SingleOrDefault(p => p.Phone == Mobile && p.ID != UserID);
                    if (sPhone != null)
                    {
                        iResult = -4;
                        return iResult;
                    }

                    var sIDNo = dbContext.Users.SingleOrDefault(p => p.IdentityNo == IDNo && p.ID != UserID);
                    if (sIDNo != null)
                    {
                        iResult = -5;
                        return iResult;
                    }
                    #endregion

                    var dbID = dbContext.Users.SingleOrDefault(p => p.ID == UserID);
                    dbID.FirstName = FirstName;
                    dbID.LastName = LastName;
                    dbID.Username = Login;
                    dbID.Password = Password;
                    dbID.bAdmin = bAdmin;
                    dbID.Phone = Mobile;
                    dbID.Email = EmailAddress;
                    dbID.bEnable = bEnable;
                    dbID.IdentityNo = IDNo;
                    dbID.DepartmentID = DepCode;
                    dbID.BranchID = nBranchID;
                    dbContext.SubmitChanges();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Update";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "UserID:" + UserID + ";FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepCode:" + DepCode + ";BranchID:" + nBranchID + ";Login:" + Login + ";Password:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                    LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                    LogObj.Description = "UserID: " + UserID + "has been updated successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UserID:" + UserID + ";FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepCode:" + DepCode + ";BranchID:" + nBranchID + ";Login:" + Login + ";Password:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Update";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "UserID:" + UserID + ";FirstName:" + FirstName + ";LastName:" + LastName
                        + ";DepCode:" + DepCode + ";BranchID:" + nBranchID + ";Login:" + Login + ";Password:" + Password
                        + ";bAdmin:" + bAdmin + ";Mobile:" + Mobile + ";EmailAddress:" + EmailAddress;
                LogObj.UserName = GetUserName((decimal)nUpdatedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public int DeleteUser(decimal nUserID, decimal nDeletedBy, string sActivityPage)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbID = dbContext.Users.SingleOrDefault(p => p.ID == nUserID);
                    if (dbID != null)
                    {
                        dbContext.Users.DeleteOnSubmit(dbID);
                        dbContext.SubmitChanges();
                        var dbUserPerm = dbContext.Userroles.Where(p => p.ID_User == nUserID);
                        foreach (var item in dbUserPerm)
                        {
                            dbContext.Userroles.DeleteOnSubmit(item);
                            dbContext.SubmitChanges();
                        }
                        iResult = 1;
                    }

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Delete";
                    LogObj.ActivityPage = sActivityPage;
                    LogObj.ActivityParameters = "nUserID:" + nUserID;
                    LogObj.UserName = GetUserName((decimal)nDeletedBy);
                    LogObj.Description = "nUserID: " + nUserID + " has been deleted successfully.";
                    LogEntry(LogObj);

                    return iResult;
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nUserID:" + nUserID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Delete";
                LogObj.ActivityPage = sActivityPage;
                LogObj.ActivityParameters = "nUserID:" + nUserID;
                LogObj.UserName = GetUserName((decimal)nDeletedBy);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            return iResult;
        }
        public tbl_User_OB GetUserByName(string UserLogin, string Password,  out List<tbl_UserRole_OB> objRole)
        {
            tbl_User_OB objUsers = new tbl_User_OB();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbUser = (from t in dbContext.Users
                                  where
                                     t.Username == UserLogin && t.Password == Password 
                                  select new
                                  {
                                      t.ID,
                                      t.FirstName,
                                      t.LastName,
                                      t.Phone,
                                      t.Email,
                                      t.bAdmin,
                                      t.Username,
                                      t.Password,
                                      t.DepartmentID,
                                      t.bEnable,
                                      t.IdentityNo,
                                      t.BranchID
                                  }).OrderBy(p => p.ID);
                    foreach (var item in dbUser)
                    {
                        tbl_User_OB objUser = new tbl_User_OB();
                        objUser.UserID = item.ID;
                        objUser.FirstName = item.FirstName;
                        objUser.LastName = item.LastName;
                        objUser.UserName = item.FirstName + " " + item.LastName;
                        objUser.Phone = item.Phone;
                        objUser.EmailAddress = item.Email;
                        objUser.bAdmin = item.bAdmin;
                        if (item.DepartmentID == null)
                        {
                            objUser.DepartmentID = -1;
                        }
                        else
                        {
                            objUser.DepartmentID = (decimal)item.DepartmentID;
                        }
                        if (item.BranchID == null)
                        {
                            objUser.BranchID = -1;
                        }
                        else
                        {
                            objUser.BranchID = (decimal)item.BranchID;
                        }
                        objUser.sLogin = item.Username;
                        objUser.sPassword = item.Password;
                        objUser.bEnable = item.bEnable;
                        objUser.IDNo = item.IdentityNo;
                        objUsers = objUser;
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "UserLogin:" + UserLogin + ";Password:" + Password;
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "User.aspx";
                LogObj.ActivityParameters = "UserLogin:" + UserLogin + ";Password:" + Password;
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            objRole = GetUserRoleByUserID(objUsers.UserID);
            return objUsers;
        }
        #endregion

        #region "Logs"
        public List<tbl_Logs_OB> GetLogReport()
        {
            List<tbl_Logs_OB> objLogReport = new List<tbl_Logs_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.tbl_Logs

                                 select new
                                 {
                                     t01.LogID,
                                     t01.UserName,
                                     t01.Activity,
                                     t01.ActivityDateTime,
                                     temp = t01.ActivityDateTime.Value.ToShortDateString(),
                                     //ActivityDateTime =  t01.ActivityDateTime.Value.ToShortDateString(),
                                     t01.ActivityPage,
                                     t01.ActivityParameters,
                                     t01.Description
                                 }).OrderBy(p => p.LogID);
                    foreach (var item in dbLog)
                    {
                        tbl_Logs_OB objLog = new tbl_Logs_OB();
                        objLog.LogID = item.LogID;
                        objLog.UserName = item.UserName;
                        objLog.Activity = item.Activity;
                        objLog.ActivityDateTime = item.ActivityDateTime;
                        objLog.ActivityPage = item.ActivityPage;
                        objLog.ActivityParameters = item.ActivityParameters;
                        objLog.Description = item.Description;
                        objLog.Page = item.ActivityPage.Split('.')[0];
                        objLogReport.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objLogReport;
        }

        public List<tbl_Logs_OB> GetLogReport_CP(int nRows, int nPageSize, out int nTotalRecords)
        {
            List<tbl_Logs_OB> objLogReport = new List<tbl_Logs_OB>();
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.tbl_Logs

                                 select new
                                 {
                                     t01.LogID,
                                     t01.UserName,
                                     t01.Activity,
                                     //ActivityDateTime = Convert.ToString(t01.ActivityDateTime),
                                     t01.ActivityDateTime,
                                     t01.ActivityPage,
                                     t01.ActivityParameters,
                                     t01.Description
                                 }).OrderBy(p => p.LogID);
                    nTemp = dbLog.Count();
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Logs_OB objLog = new tbl_Logs_OB();
                        objLog.LogID = item.LogID;
                        objLog.UserName = item.UserName;
                        objLog.Activity = item.Activity;
                        objLog.ActivityDateTime = item.ActivityDateTime;
                        objLog.ActivityPage = item.ActivityPage;
                        objLog.ActivityParameters = item.ActivityParameters;
                        objLog.Description = item.Description;
                        objLog.Page = item.ActivityPage.Split('.')[0];
                        objLogReport.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();                
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            nTotalRecords = nTemp;
            return objLogReport;
        }

        public List<tbl_Logs_OB> GetLogReportFilter(string sUserName, string sActivity, string sPage, string sDesc, int nRows, int nPageSize)
        {
            List<tbl_Logs_OB> objLogReport = new List<tbl_Logs_OB>();
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.tbl_Logs
                                 where t01.UserName.Contains(sUserName)
                                 && t01.Activity.Contains(sActivity)
                                 && t01.ActivityPage.Contains(sPage)
                                 && t01.Description.Contains(sDesc)
                                 //&& t01.ActivityDateTime.Value.Month.ToString() + "/" + t01.ActivityDateTime.Value.Day.ToString() + "/" + t01.ActivityDateTime.Value.Year.ToString() + " 12:00:00 AM" == sDate
                                 select new
                                 {
                                     t01.LogID,
                                     t01.UserName,
                                     t01.Activity,
                                     t01.ActivityDateTime,
                                     t01.ActivityPage,
                                     t01.ActivityParameters,
                                     t01.Description
                                 }).OrderBy(p => p.LogID);
                    var dbLogSkip = dbLog.Skip(nRows).Take(nPageSize).ToList();
                    foreach (var item in dbLogSkip)
                    {
                        tbl_Logs_OB objLog = new tbl_Logs_OB();
                        objLog.LogID = item.LogID;
                        objLog.UserName = item.UserName;
                        objLog.Activity = item.Activity;
                        objLog.ActivityDateTime = item.ActivityDateTime;
                        objLog.ActivityPage = item.ActivityPage;
                        objLog.ActivityParameters = item.ActivityParameters;
                        objLog.Description = item.Description;
                        objLog.Page = item.ActivityPage.Split('.')[0];
                        objLogReport.Add(objLog);
                    }
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();                
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return objLogReport;
        }

        public int GetLogReportCount()
        {
            int nTemp = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    var dbLog = (from t01 in dbContext.tbl_Logs

                                 select new
                                 {
                                     t01.LogID,
                                     t01.UserName,
                                     t01.Activity,
                                     t01.ActivityDateTime,
                                     t01.ActivityPage,
                                     t01.ActivityParameters,
                                     t01.Description
                                 }).Count();
                    nTemp = Convert.ToInt32(dbLog.ToString());
                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Select";
                LogObj.ActivityPage = "LogReport.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = "";
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
            }
            return nTemp;
        }
        #endregion

        #region "Backup&Restore"
        public int BackupHID(decimal nUserID, string sPath, out string sFileName)
        {
            int iResult = 0;
            try
            {
                using (IndustryEntityContextDataContext dbContext = new IndustryEntityContextDataContext(sconnection))
                {
                    //C:\\HIDBackup\\HIDBackup_
                    string strQry = "backup database INDUSTRIALV2 to disk ='" + sPath + "INDUSTRIALV2Backup_" + DateTime.Now.Day.ToString() + "-"
                        + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".bak' WITH FORMAT,MEDIANAME = 'Z_SQLServerBackups',NAME = 'Full Backup of Database';";
                    SqlConnection sConn = new SqlConnection(sconnection);
                    if (sConn.State != System.Data.ConnectionState.Open)
                        sConn.Open();
                    SqlCommand sCmd = new SqlCommand(strQry, sConn);
                    sCmd.ExecuteScalar();
                    iResult = 1;

                    //Record the transaction at the log data base
                    tbl_Logs_OB LogObj = new tbl_Logs_OB();
                    LogObj.Activity = "Backup";
                    LogObj.ActivityPage = "BackupandRestore.aspx";
                    LogObj.ActivityParameters = "";
                    LogObj.UserName = GetUserName((decimal)nUserID);
                    LogObj.Description = "Backup has been done successfully.";
                    LogEntry(LogObj);

                }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Backup";
                LogObj.ActivityPage = "";
                LogObj.ActivityParameters = "";
                LogObj.UserName = GetUserName((decimal)nUserID);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Backup";
                LogObj.ActivityPage = "";
                LogObj.ActivityParameters = "";
                LogObj.UserName = GetUserName((decimal)nUserID);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
            }
            sFileName = "HIDBackup_" + DateTime.Now.Day.ToString() + "-"
                        + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            return iResult;
        }

        public int RestoreHID(decimal nUserID, string sBackup, string sPath, out string exc)
        {
            int iResult = 0;
            try
            {
                //using (HID_DBDataContext dbContext = new HID_DBDataContext(sconnection))
                //{
                SqlConnection sConn = new SqlConnection(sconnection);
                if (sConn.State != System.Data.ConnectionState.Open)
                    sConn.Open();

                SqlConnection.ClearAllPools();
                //string strQry = " Restore database HID from disk ='C:\\HIDBackup\\" + sBackup + "' WITH FILE=1, NORECOVERY";

                SqlCommand sCmd = new SqlCommand("RestoreDatabase", sConn);
                sCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter SqlParameter_DBName = new SqlParameter("@DBName", SqlDbType.NVarChar, 100, "@DBName");
                SqlParameter_DBName.Value = sBackup;
                sCmd.Parameters.Add(SqlParameter_DBName);

                SqlParameter SqlParameter_strPath = new SqlParameter("@BackupPath", SqlDbType.NVarChar, 500, "@BackupPath");
                SqlParameter_strPath.Value = sPath;
                sCmd.Parameters.Add(SqlParameter_strPath);

                sCmd.CommandTimeout = 1000;
                sCmd.ExecuteScalar();
                iResult = 1;

                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();
                LogObj.Activity = "Restore";
                LogObj.ActivityPage = "BackupandRestore.aspx";
                LogObj.ActivityParameters = "";
                LogObj.UserName = GetUserName((decimal)nUserID);
                LogObj.Description = "Restore has been done successfully.";
                LogEntry(LogObj);
                exc = "mop";
                return iResult;
                // }
            }
            catch (SqlException SQLEx)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();

                LogObj.Activity = "Restore";
                LogObj.ActivityPage = "";
                LogObj.ActivityParameters = "";
                LogObj.UserName = GetUserName((decimal)nUserID);
                LogObj.Description = SQLEx.Message;
                LogEntry(LogObj);
                iResult = -1;
                exc = SQLEx.Message;
                return iResult;
            }
            catch (Exception Ex)
            {
                //Record the transaction at the log data base
                tbl_Logs_OB LogObj = new tbl_Logs_OB();

                LogObj.Activity = "Restore";
                LogObj.ActivityPage = "";
                LogObj.ActivityParameters = "";
                LogObj.UserName = GetUserName((decimal)nUserID);
                LogObj.Description = Ex.Message;
                LogEntry(LogObj);
                iResult = -2;
                exc = Ex.Message;
                return iResult;
            }

        }
        #endregion
    }
}