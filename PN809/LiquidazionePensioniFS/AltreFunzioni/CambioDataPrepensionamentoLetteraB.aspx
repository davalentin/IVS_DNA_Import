<%@ Page Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="CambioDataPrepensionamentoLetteraB.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.CambioDataPrepensionamentoLetteraB" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/CambioDataPrepensionamentoLetteraB/UCCambioDataPrepensionamentoLetteraB.ascx" TagName="UCCambioDataPrepensionamentoLetteraB" TagPrefix="UCCDS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione data limite domande Aziende Editoriali art. 37 legge 416/1981, lettera (b)</h2>
    </div>

    <table class="" width="720px">
        <tr>
            <td align="left" style="width: 720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabDataSistema" ValidationGroup="UCDataSistema"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px">
                <UCCDS:UCCambioDataPrepensionamentoLetteraB runat="server" ID="UCCambioDataPrepensionamentoLetteraB" Visible="true"
                    OnShowAvviso="event_ucShowAvviso"/>
            </td>
        </tr>
    </table>
</asp:Content>
