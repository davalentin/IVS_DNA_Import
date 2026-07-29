<%@ Page Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true"
    CodeBehind="TipologieNonAbilitate.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.TipologieNonAbilitate" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/TipologieNonAbilitate/UCTipologieNonAbilitate.ascx"
    TagName="UCTipologieNonAbilitate" TagPrefix="UCTNA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function validatePageFiltro() {
            var flag = true;
            flag = Page_ClientValidate('UCTipologieNonAbilitateFiltro');
            return flag;
        }

        function validatePageGrid() {
            var flag = true;
            flag = Page_ClientValidate('UCTipologieNonAbilitateGrid');
            return flag;
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione tipologie non abilitate</h2>
        <h6 class="page-subtitle">Abilitazione e disabilitazione tipologie</h6>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" class="full-width" style="width: 720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabTipologieNonAbilitateFiltro" ValidationGroup="UCTipologieNonAbilitateFiltro"
                    Font-Size="Small" CssClass="errorBox" />
                <asp:ValidationSummary runat="server" ID="tabTipologieNonAbilitateGrid" ValidationGroup="UCTipologieNonAbilitateGrid"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" class="full-width" style="width: 720px">
                <UCTNA:UCTipologieNonAbilitate runat="server" ID="ucTipologieNonAbilitate" Visible="true"
                    OnShowAvviso="event_ucShowAvviso" OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
