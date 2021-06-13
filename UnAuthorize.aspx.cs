using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace NewIndustrial
{
    public partial class UnAuthorize : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Forms/Dashboard.aspx");
        }
    }
}