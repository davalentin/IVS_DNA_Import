<%@ Page Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
AutoEventWireup="true" CodeBehind="LavorazioneManualeAutomatiche.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.LavorazioneManualeAutomatiche" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/LavorazioneManualeAutomatiche/UCLavorazioneManualeAutomatiche.ascx"
    TagName="UCBypassControlli" TagPrefix="UCBYC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css" media="screen">
        .fixed-dialog
        {
            position: fixed;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione di domande manuali</h2>
        <h6 class="page-subtitle">Autorizzazione lavorazione manuale domande</h6>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width: 765px" class="full-width" >
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" width="760px" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 765px" class="full-width" >
                <UCBYC:UCBypassControlli runat="server" ID="ucBypassControlli" Visible="true" OnShowAvviso="event_ucShowAvviso"
                    OnHideInfo="event_ucHideInfo" />
            </td>
        </tr>
    </table>
</asp:Content>
