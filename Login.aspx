<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NewIndustrial.Login" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link href="../Css/ValidateCallOut.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #content
        {
            position: absolute;
            width: 527px;
            height: 406px;
            top: 50%;
            left: 50%;
            margin-left: -270px; /* Negative half of width. */
            margin-top: -190px; /* Negative half of height. */
            background-image: url('./Images/loginBackground2.png');
            
        }
        .table
        {
            position: relative;
            width: 545px;
            height: 400px;
            margin-top: -130px;
            margin-left: 28px;
            top: 200px;
            left: 53px;
        }
        
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div  id="content">
        <table class="table">
            <tr style="height: 30px">
                <td>
                </td>
            </tr>
            <tr style="height: 45px">
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="position: absolute; top: 120px">
                        <table width="450px" style="font-size: 12px; font-family: Segoe UI, Arial, sans-serif">
                            <tr>
                                <td>
                                    <asp:Label ID="Lbl_Failure" runat="server" Text="Invalid UserName or Password!" Visible="false"
                                        ForeColor="Red" Font-Bold="true" Font-Names="Arial" Font-Size="Smaller" Style="position: absolute;
                                        top: -20px; left: 110px; bottom: 184px;"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span style="color: #3196FB; font-weight: bold; font-size: small; float: right; font-family: Segoe UI,​ Arial,​ sans-serif">
                                        Username</span>
                                </td>
                                <td>
                                    <asp:TextBox Style="margin-left: 10px" ID="TxBxUserName" runat="server" 
                                        Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td align="left">
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="Login" Style="margin-left: 10px"
                                        Display="None" Font-Size="Small" ErrorMessage="Please enter your Username" ControlToValidate="TxBxUserName"></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender8" runat="server" TargetControlID="RequiredFieldValidator1"
                                        CssClass="customCalloutStyle" PopupPosition="Right">
                                    </asp:ValidatorCalloutExtender>
                                </td>
                                <td align="left">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span style="color: #3196FB; font-weight: bold; font-size: small; float: right; font-family: Segoe UI,​ Arial,​ sans-serif">
                                        Password</span>
                                </td>
                                <td>
                                    <asp:TextBox Style="margin-left: 10px" ID="TxBxPassword" TextMode="Password" runat="server"
                                        Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Style="margin-left: 10px" ValidationGroup="Login"
                                        Font-Size="Small" ErrorMessage="Please enter your Password" ControlToValidate="TxBxPassword"
                                        Display="None"></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" TargetControlID="RequiredFieldValidator2"
                                        CssClass="customCalloutStyle" PopupPosition="Right">
                                    </asp:ValidatorCalloutExtender>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <div style="position: relative; left: 50px">
                                        <asp:RadioButton ID="RB_En" runat="server" Text="English" GroupName="Lang" Checked="True" />
                                        <asp:RadioButton ID="RB_Ar"  runat="server" Text="عربي" GroupName="Lang" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <div class="content" style="position: absolute; top: 100px; left: 275px">
                                        <asp:ImageButton ID="Submitbtn" runat="server" Height="19px" Width="40px" ImageUrl="~/images/login.png"
                                            OnClick="Login_Click" ValidationGroup="Login" />
                                    </div>
                                    <br />
                                    <div class="content" style="position: absolute; top: 120px; left: 245px; font-size: xx-small;
                                        font-family: Segoe UI, Arial, sans-serif; display:none">
                                        <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/ForgetPassword.aspx">Forget Password?</asp:LinkButton>
                                    </div>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <%-- <div>
            <asp:Image ID="Image1" runat="server" Style="position: relative; top:-300px; right: 250px;"
                ImageUrl="~/images/login_footer.jpg"></asp:Image>
        </div>--%>
    </div>
    </form>
</body>
</html>
