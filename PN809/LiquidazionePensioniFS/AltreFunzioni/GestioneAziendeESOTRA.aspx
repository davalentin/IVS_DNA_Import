<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneAziendeESOTRA.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAziendeESOTRA" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAziendeESOTRA/UCAziendeESOTRA.ascx"
    TagName="UCAzESOTRA" TagPrefix="UCAESOTRA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="" width="720px">
        <tr>
            <td align="center" style="width: 720px">
                <label style="color: #336699; font-weight: bold; font-size: larger; width: 720px">
                    Gestione Aziende ESOTRA</label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
                <asp:ValidationSummary runat="server" ID="ValidGrigliaAziendeESOTRA" ValidationGroup="GrigliaAziendeESOTRA"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px">
                <UCAESOTRA:UCAzESOTRA runat="server" ID="ucAzESOTRA" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
