<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneAziendeEditorialiPerTipo0171.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAziendeEditorialiPerTipo0171" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAziendeEditorialiPerTipo0171/UCGestioneAziendeEditorialiPerTipo0171.ascx"
    TagName="UCGestAzEditoriali" TagPrefix="UCGAE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione aziende editoriali art. 1 comma 154 legge 205/2017</h2>
    </div>

    <table class="" width="720px">
        <tr>
            <td align="left" style="width: 720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
                <asp:ValidationSummary runat="server" ID="grigliaAnagraficheAccordi" ValidationGroup="GrigliaAccordi"
                    Font-Size="Small" CssClass="errorBox" />
                <asp:ValidationSummary runat="server" ID="grigliaAnagraficheAziende" ValidationGroup="GrigliaAziende"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px">
                <UCGAE:UCGestAzEditoriali runat="server" ID="ucGestAzEditoriali" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
