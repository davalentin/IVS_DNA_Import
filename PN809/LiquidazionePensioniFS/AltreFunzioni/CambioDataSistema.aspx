<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="CambioDataSistema.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.CambioDataSistema" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/CambioDataSistema/UCCambioDataSistema.ascx" TagName="UCCambioDataSistema" TagPrefix="UCCDS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione cambio data di sistema</h2>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width: 720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary runat="server" ID="tabDataSistema" ValidationGroup="UCDataSistema"
                    Font-Size="Small" CssClass="errorBox" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 720px" class="full-width form-container background-light-blue">
                <UCCDS:UCCambioDataSistema runat="server" ID="ucCambioDataSistema" Visible="true"
                    OnShowAvviso="event_ucShowAvviso"/>
            </td>
        </tr>
    </table>
</asp:Content>
