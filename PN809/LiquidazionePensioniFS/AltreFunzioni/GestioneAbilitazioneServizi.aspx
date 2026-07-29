<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="GestioneAbilitazioneServizi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAbilitazioneServizi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAbilitazioneServizi/UCAbilitazionePolarizzazioneENPALS.ascx" TagName="UCAPENPALS" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Abilitazione polarizzazione ENPALS</h2>
        <h6 class="page-subtitle">Gestione per le chiavi di abilitazione</h6>
    </div>

    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <table class="" width="720px">
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabAbilitazionePolarizzazioneENPALS" ValidationGroup="UCAbilitazionePolarizzazioneENPALS" Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="center" style="width: 720px">
                <UCA:UCAPENPALS runat="server" ID="UCAbilitazionePolarizzazioneENPALS" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnShowInfo="event_ucShowInfo" OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>

