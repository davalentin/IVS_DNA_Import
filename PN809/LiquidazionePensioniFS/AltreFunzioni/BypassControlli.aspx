<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="BypassControlli.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.BypassControlli" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/BypassControlli/UCBypassControlli.ascx"
    TagName="UCBypassControlli" TagPrefix="UCBYC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validatePageFiltro() {
            var flag = true;
            flag = Page_ClientValidate('UCBypassControlliFiltro');
            return flag;
        }

        function validatePageInsert() {
            var flag = true;
            flag = Page_ClientValidate('UCBypassControlliInsert');
            return flag;
        }
    </script>
    <style type="text/css" media="screen">
        .fixed-dialog
        {
            position: fixed;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione bypass controlli</h2>
        <h6 class="page-subtitle">Bypass controlli per domanda</h6>
    </div>

    <table width="720px" class="full-width">
        <tr>
            <td align="left" style="width: 765px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" width="760px" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 765px" class="full-width">
                <UCBYC:UCBypassControlli runat="server" ID="ucBypassControlli" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
