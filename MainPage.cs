using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.Security;
namespace NewIndustrial
{
  
    
    public class MainPage : Page

    {

        #region[Public Static Method]
        //public static void Show(string message, Control Cntrl)
        //{
        //    HtmlContainerControl myObject;
        //    myObject = (HtmlContainerControl)Cntrl.Page.Master.FindControl("DvError");
        //    myObject.Visible = true;
        //    myObject.InnerText = message;



        //    StringBuilder scriptBuidler = new StringBuilder(@"<script type='text/javascript' language='javascript'>");
        //    scriptBuidler.Append(@"alert('");
        //    scriptBuidler.Append(message);
        //    scriptBuidler.Append(@"');");
        //    scriptBuidler.Append(@"</script>");
        //    ScriptManager.RegisterClientScriptBlock(Cntrl, typeof(Page), "Ceitcon", scriptBuidler.ToString(), false);

        //}
        public static void ShowError(string message, Control Cntrl)
        {
            HtmlContainerControl spnMessage;
            spnMessage = (HtmlContainerControl)Cntrl.Page.Master.FindControl("DvError");
            spnMessage.InnerHtml = "<img src='../Images/error.png' alt='Error'>" + " " + message;
            spnMessage.Style.Add(HtmlTextWriterStyle.Color, "Red");
            spnMessage.Style.Add(HtmlTextWriterStyle.TextAlign, "Center");
            ScriptManager.RegisterStartupScript(Cntrl, typeof(Page), "HID", "hidediv('DvError');", true);
        }
        public static void ShowSuccess(string message, Control Cntrl)
        {
            HtmlContainerControl spnMessage;
            spnMessage = (HtmlContainerControl)Cntrl.Page.Master.FindControl("DvError");
            spnMessage.InnerHtml = "<img src='../Images/success.png' alt='Success'>" + " " + message;
            spnMessage.Style.Add(HtmlTextWriterStyle.Color, "Green");
            spnMessage.Style.Add(HtmlTextWriterStyle.TextAlign, "Center");
            ScriptManager.RegisterStartupScript(Cntrl, typeof(Page), "HID", "hidediv('DvError');", true);
        }
        public static void ShowWarning(string message, Control Cntrl)
        {
            HtmlContainerControl spnMessage;
            spnMessage = (HtmlContainerControl)Cntrl.Page.Master.FindControl("DvError");
            spnMessage.InnerHtml = "<img src='../Images/warning.png' alt='Warning'>" + " " + message;
            spnMessage.Style.Add(HtmlTextWriterStyle.Color, "Yellow");
            spnMessage.Style.Add(HtmlTextWriterStyle.TextAlign, "Center");
            ScriptManager.RegisterStartupScript(Cntrl, typeof(Page), "HID", "hidediv('DvError');", true);
        }
        #endregion

        public static void Show(string message, Control Cntrl)
        {
            StringBuilder scriptBuidler = new StringBuilder(@"<script type='text/javascript' language='javascript'>");
            scriptBuidler.Append(@"alert('");
            scriptBuidler.Append(message);
            scriptBuidler.Append(@"');");
            scriptBuidler.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(Cntrl, typeof(Page), "Ceitcon", scriptBuidler.ToString(), false);
        }

        public static void ShowSuccessAjax(string message, Control Cntrl)
        {
            HtmlContainerControl spnMessage;
            spnMessage = (HtmlContainerControl)Cntrl.Page.Master.FindControl("DvErrorAjax");
            spnMessage.InnerHtml = "<img src='../Images/success.png' alt='Success'>" + " " + message;
            spnMessage.Style.Add(HtmlTextWriterStyle.Color, "Green");
            spnMessage.Style.Add(HtmlTextWriterStyle.TextAlign, "Center");
            ScriptManager.RegisterStartupScript(Cntrl, typeof(Page), "HID", "hidediv('DvError');", true);
        }


    }

      
}