<%@ Page Language="C#" Title="" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="SbloccoDomanda.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SbloccoDomanda" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/SbloccoDomanda/UCSblocco.ascx" TagName="UCSblocco" TagPrefix="UCS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    function validatePage() {
        var flag = true;
        flag = Page_ClientValidate('UCTabSblocco');
        return flag;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione sblocco domanda WebDom</h2>
        <h6 class="page-subtitle">Sblocco domanda WebDom</h6>
    </div>

    <table width="720px" class="full-width form-container background-light-blue">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
        <td>
            <asp:ValidationSummary runat="server" ID="tabSblocco" ValidationGroup="UCTabSblocco" Font-Size="Small" CssClass="errorBox" />
        </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
            <UCS:UCSblocco runat="server" ID="ucSblocco" Visible="true" OnShowAvviso="event_ucShowAvviso" 
                OnShowInfo="event_ucShowInfo" OnHideInfo="event_ucHideInfo" OnReloadUChangeSede="event_ReloadUChangeSede"/>
            </td>
        </tr>
    </table>
</asp:Content>
