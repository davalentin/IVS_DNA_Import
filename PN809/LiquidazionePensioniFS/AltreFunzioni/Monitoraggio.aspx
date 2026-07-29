<%@ Page Language="C#" Title="" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="Monitoraggio.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Monitoraggio" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/Monitoraggio/UCMonitoraggio.ascx" TagName="UCMonitoraggio" TagPrefix="UCM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h2 class="page-title-secondlevel">Monitoraggio</h2>
    </div>

    <asp:ValidationSummary runat="server" ID="tabMonitoraggio" ValidationGroup="UCMonitoraggio" Font-Size="Small" CssClass="errorBox" />

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
            <UCM:UCMonitoraggio runat="server" ID="ucMonitoraggio" Visible="true" OnShowAvviso="event_ucShowAvviso" 
                OnShowInfo="event_ucShowInfo" OnHideInfo="event_ucHideInfo"/>
            </td>
        </tr>
    </table>
</asp:Content>
