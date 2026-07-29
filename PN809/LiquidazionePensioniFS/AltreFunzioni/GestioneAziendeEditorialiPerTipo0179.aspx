<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneAziendeEditorialiPerTipo0179.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAziendeEditorialiPerTipo0179" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAziendeEditorialiPerTipo0179/UCGestioneAziendeEditorialiPerTipo0179.ascx"
    TagName="UCGestAzEditoriali" TagPrefix="UCGAE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione aziende editoriali art. 1 comma 500 legge 160/2019</h2>
    </div>

    <table class="">
        <tr>
            <td align="left">
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
