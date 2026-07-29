<%@ Page Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true"
    CodeBehind="BypassTipologieNonAbilitate.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.BypassTipologieNonAbilitate" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/BypassTipologieNonAbilitate/UCBypassTipologieNonAbilitate.ascx"
    TagName="UCBypassTipologieNonAbilitate" TagPrefix="UCBTNA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ui-autocomplete.ui-widget { font-size: 13px; }
    </style>
    <script type="text/javascript">
        function validatePageFiltro() {
            var flag = true;
            flag = Page_ClientValidate('UCBypassTipologieNonAbilitateFiltro');
            return flag;
        }

        function validatePageGrid() {
            var flag = true;
            flag = Page_ClientValidate('UCBypassTipologieNonAbilitateGrid');
            return flag;
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione bypass tipologie non abilitate</h2>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width: 720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabBypassTipologieNonAbilitateFiltro" ValidationGroup="UCBypassTipologieNonAbilitateFiltro"
                    Font-Size="Small" CssClass="errorBox" />
                <asp:ValidationSummary runat="server" ID="tabBypassTipologieNonAbilitateGrid" ValidationGroup="UCBypassTipologieNonAbilitateGrid"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px" class="full-width">
                <UCBTNA:UCBypassTipologieNonAbilitate runat="server" ID="ucBypassTipologieNonAbilitate" Visible="true"
                    OnShowAvviso="event_ucShowAvviso" OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
