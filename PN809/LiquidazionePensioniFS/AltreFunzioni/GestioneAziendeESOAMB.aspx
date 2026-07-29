<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneAziendeESOAMB.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAziendeESOAMB" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAziendeESOAMB/UCAziendeESOAMB.ascx"
    TagName="UCAzESOAMB" TagPrefix="UCAESOAMB" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione Aziende ESOAMB</h2>
    </div>

    <table class="" width="720px">
        <tr>
            <td align="left" style="width: 720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
                <asp:ValidationSummary runat="server" ID="ValidGrigliaAziendeESOAMB" ValidationGroup="GrigliaAziendeESOAMB"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px">
                <UCAESOAMB:UCAzESOAMB runat="server" ID="ucAzESOAMB" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
