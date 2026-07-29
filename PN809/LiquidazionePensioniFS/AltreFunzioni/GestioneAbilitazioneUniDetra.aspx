<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
AutoEventWireup="true" CodeBehind="GestioneAbilitazioneUniDetra.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneAbilitazioneUniDetra" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneAbilitazioneUniDetra/UCAbilitazioneUniDetra.ascx"
    TagName="UCAbilitazioneUniDetra" TagPrefix="UCAUD"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:ValidationSummary runat="server" ID="tabAbilitazioneUniDetra" ValidationGroup="UCAbilitazioneUniDetra" Font-Size="Small" CssClass="errorBox" />
<table class="" width="720px">
        <tr>
            <td align="center" style="width:720px">
                <label style="color: #336699; font-weight: bold; font-size:larger; width:720px">
                    Abilitazione UNIDETRA</label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="left" style="width:720px">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center" style="width:720px">
            <UCAUD:UCAbilitazioneUniDetra runat="server" ID="UCAbilitazioneUniDetra" Visible="true" OnShowAvviso="event_ucShowAvviso" 
                OnShowInfo="event_ucShowInfo" OnHideInfo="event_ucHideInfo"/>
            </td>
        </tr>
    </table>
</asp:Content>

