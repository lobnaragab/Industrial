using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewIndustrial
{
    public partial class Site : System.Web.UI.MasterPage
    {
        private decimal UserID
        {
            get { return Convert.ToDecimal(ViewState["UserID"]); }
            set { ViewState["UserID"] = value; }
        }

        private decimal LangID
        {
            get { return Convert.ToDecimal(ViewState["LangID"]); }
            set { ViewState["LangID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Count <= 0)
                Response.Redirect("~/Login.aspx");
            if (Session["UserIDLogin"] == null)
                Response.Redirect("~/Login.aspx");
            try
            {
                LoadStyleDirection();
                if (Session["UserName"] != null)
                {
                    Lbl_UserName.Text = Session["UserName"].ToString();
                }
                else Response.Redirect("~/Login.aspx");
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        private bool CheckMCTabsPerm()
        {
            bool bCheck = false;

            return bCheck;
        }

        //On page load call this function to set the layout
        private void LoadStyleDirection()
        {
            if (Session["MyUICulture"].ToString() == "en-US")
            {
                form1.Style.Add(HtmlTextWriterStyle.Direction, "ltr");

                //IBtnLang.Attributes.Add("class", "changelanguageEnglish");
                IBtnLang.ImageUrl = "~/Images/SA.png";
                ImageButton2.ToolTip = "Hedab Al-Majd";
                ImageButton1.ToolTip = "Home";


                IBtnLogOff.OnClientClick = "return confirm('Are You Sure You Want To Logout?');";
                //ImageButton3.OnClientClick = "confirmAction(this,'Synchronize Data'); return false;";
                //IBtnLogOff.OnClientClick = "confirmAction(this,'Logout'); return false;";
                LblLogIn.Text = "Welcome:";
                IBtnLogOff.ToolTip = "Log Off";
                IBtnLang.ToolTip = "Change Language";
                //RadButton1.ToolTip = "User Information";
            }
            else if (Session["MyUICulture"].ToString() == "ar-SA")
            {
                form1.Style.Add(HtmlTextWriterStyle.Direction, "rtl");
                Div1.Attributes.Add("dir", "ltr");
                //IBtnLang.Attributes.Add("class", "changelanguageArabic");
                IBtnLang.ImageUrl = "~/Images/USA.png";
                ImageButton2.ToolTip = "هضاب المجد";
                ImageButton1.ToolTip = "الرئيسية";
                //ImageButton3.OnClientClick = "confirmActionAR(this,'نقل البيانات'); return false;";
                //IBtnLogOff.OnClientClick = "confirmActionAR(this,'الخروج'); return false;"

                IBtnLogOff.OnClientClick = "return confirm('هل تريد الخروج؟');";
                LblLogIn.Text = "مرحبا:";
                IBtnLogOff.ToolTip = "خروج";
                IBtnLang.ToolTip = "تغيير اللغة";
                //RadButton1.ToolTip = "بيانات المستخدم";
            }
        }

        protected void IBtnLang_Click(object sender, EventArgs e)
        {
            if (Session["MyUICulture"].ToString() == "en-US")
            {
                Session["MyUICulture"] = new CultureInfo("ar-SA");
                Session["MyCulture"] = new CultureInfo("ar-SA");
                Session["LangID"] = "2";
            }
            else if (Session["MyUICulture"].ToString() == "ar-SA")
            {
                Session["MyUICulture"] = new CultureInfo("en-US");
                Session["MyCulture"] = new CultureInfo("en-US");
                Session["LangID"] = "1";
            }
            Response.Redirect(Request.RawUrl);
        }
    }
}