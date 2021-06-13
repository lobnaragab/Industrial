

using ObjectModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewIndustrial
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TxBxUserName.Focus();
        }

        protected void Login_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //check language
                if (RB_Ar.Checked)
                {
                    Session["MyUICulture"] = new CultureInfo("ar-SA");
                    Session["MyCulture"] = new CultureInfo("ar-SA");
                    Session["LangID"] = "2";
                }
                else if (RB_En.Checked)
                {
                    Session["MyUICulture"] = new CultureInfo("en-US");
                    Session["MyCulture"] = new CultureInfo("en-US");
                    Session["LangID"] = "1";
                }
                string sUserName = TxBxUserName.Text;
                string sPWD = TxBxPassword.Text;
                Session["UserName"] = sUserName;
                
                //FormsAuthentication.RedirectFromLoginPage(TxBxUserName.Text, false);
                DBAccess DBObject = new DBAccess();
                //check the user and password in table and get User ID
                int userID = CheckUser(sUserName, sPWD);
                if(userID != 0) 
                { 
                    Session["UserIDLogin"] = userID;
                    ViewState["UserID"] = userID;
                    ViewState["UserIDLogin"] = userID;
                    List<tbl_UserRole_OB> objRole;
                    tbl_User_OB UserLogin = DBObject.GetUserByName(sUserName, sPWD, out objRole);
                    if (UserLogin.UserID != 0)
                    {
                        //check if user is enabled oor not
                        if ((bool)UserLogin.bEnable == false)
                        {
                            Lbl_Failure.Visible = true;
                            Lbl_Failure.Text = "Invalid UserName or Password!";
                        }
                        else
                        {
                            Session["objPermission"] = objRole;

                            decimal UserID = UserLogin.UserID;
                            Session["UserID"] = UserID;

                            Response.Redirect("~/Forms/Dashboard.aspx", true);
                            //FormsAuthentication.RedirectFromLoginPage(TxBxUserName.Text, false);
                        }
                    }
                    else
                    {
                        Lbl_Failure.Visible = true;
                        //Lbl_Failure.Text = "error";
                        Lbl_Failure.Text = "Invalid UserName or Password!";
                    }
                }
                else FormsAuthentication.RedirectFromLoginPage(TxBxUserName.Text, false);

            }
            catch (Exception ex)
            {
                Lbl_Failure.Visible = true;
                Lbl_Failure.Text = ex.Message;
                MainPage.Show(ex.Message, this.Page);
            }
        }

        public int CheckUser(string UserName, string Password)
        {
            int userID = 0;
            var con = ConfigurationManager.ConnectionStrings["IndustrialConnectionString"].ToString();         
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                string oString = "Select * FROM [INDUSTRIALV2].[dbo].[User] where Username=@UserName and Password=@Password";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                oCmd.Parameters.AddWithValue("@UserName", UserName);
                oCmd.Parameters.AddWithValue("@Password", Password);
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        userID = Convert.ToInt32(oReader["ID"].ToString());
                        //matchingPerson.lastName = oReader["LastName"].ToString();
                    }

                    myConnection.Close();
                }
            }
            return userID;
        }
    }
}