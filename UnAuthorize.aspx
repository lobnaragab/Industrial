<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UnAuthorize.aspx.cs" Inherits="NewIndustrial.UnAuthorize" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadNotification ID="notification" runat="server" VisibleOnPageLoad="true"
        Title="Not Allowed" AutoCloseDelay="0" Width="350" Position="BottomCenter" ShowCloseButton="false"
        OffsetY="-250" OffsetX="50" Height="150" EnableRoundedCorners="true" EnableShadow="true">
        <ContentTemplate>
            <br />
            <center>
                <b>
                    <asp:Literal ID="lit" runat="server" Text="You don't have permission on this page"></asp:Literal>
            </center>
            </b>
            <br />
            <br />
            <center>
                <telerik:RadButton ID="BtnSave" runat="server" Text='Back to Home Page' OnClick="Back_Click">
                </telerik:RadButton>
            </center>
        </ContentTemplate>
    </telerik:RadNotification>
</asp:Content>
