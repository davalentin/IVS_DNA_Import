<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneProvvisoriePerCoefficienti.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneProvvisoriePerCoefficienti" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/ProvvisoriePerCoefficienti/UCProvvisoriePerCoefficienti.ascx" TagName="UCProvvisCoefficienti" TagPrefix="UCPC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione provvisorie per coefficienti</h2>
    </div>

    <table>
        <tr>
            <td align="left">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabDecorrenzaProvvisoriaObbligatoria" ValidationGroup="UCProvvisoriePerCoefficienti"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <UCPC:UCProvvisCoefficienti runat="server" ID="ucProvvisorieCoefficienti" Visible="true"
                    OnShowAvviso="event_ucShowAvviso"/>
            </td>
        </tr>
    </table>
</asp:Content>
