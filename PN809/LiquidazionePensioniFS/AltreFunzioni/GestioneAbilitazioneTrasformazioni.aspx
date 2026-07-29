<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="GestioneAbilitazioneTrasformazioni.aspx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAbilitazioneTrasformazioni" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAbilitazioneTrasformazioni/UCGestioneAbilTrasf.ascx"
    TagName="UCAbilTrasf" TagPrefix="UCAT" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione abilitazione trasformazioni</h2>
        <h6 class="page-subtitle">Abilitazione e disabilitazione trasformazioni da provvisoria a definitiva per sedi</h6>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width: 720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px" class="full-width">
                <UCAT:UCAbilTrasf runat="server" id="ucGestioneAbilTrasf" visible="true" onshowavviso="event_ucShowAvviso"
                    onshowinfo="event_ucShowInfo" onhideinfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
