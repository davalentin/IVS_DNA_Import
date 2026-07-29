<%@ Page Language="C#" Title="" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneLiquidazioni.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneLiquidazioni" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/GestioneLiquidazioni/UCLiquidazioni.ascx" TagName="UCLiquidazioni" TagPrefix="UCL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione abilitazione liquidazioni</h2>
        <h6 class="page-subtitle">Abilitazione e disabilitazione liquidazine sedi</h6>
    </div>
    
    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCL:UCLiquidazioni runat="server" ID="ucLiquidazioni" Visible="true" OnShowAvviso="event_ucShowAvviso" 
                OnShowInfo="event_ucShowInfo" OnHideInfo="event_ucHideInfo"/>
            </td>
        </tr>
    </table>
</asp:Content>
