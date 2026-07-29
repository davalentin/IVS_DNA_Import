<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneAziendeVOESO.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAziendeVOESO" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAziendeVOESO/UCAziendeVOESO.ascx"
    TagName="UCAzVOESO" TagPrefix="UCAvoeso" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel"><asp:Label runat="server" ID="lblTitle" Text="Gestione Aziende VOESO"></asp:Label></h2>
    </div>

    <table class="containerWidth xs">
        <tr>
            <td align="left">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
                <asp:ValidationSummary runat="server" ID="ValidGrigliaAziendeVOESO" ValidationGroup="GrigliaAziendeVOESO"
                    Font-Size="Small" CssClass="errorBox" />
                <asp:ValidationSummary runat="server" ID="ValidFiltroAziendeVOESO" ValidationGroup="FiltroAziendeVOESO"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <UCAvoeso:UCAzVOESO runat="server" ID="ucAzVOESO" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" OnChangeTipo="event_ucChangeTipo" />
            </td>
        </tr>
    </table>
</asp:Content>
