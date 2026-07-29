<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="CambioDataINDCOM.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.CambioDataINDCOM" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/CambioDataINDCOM/UCCambioDataINDCOM.ascx" TagName="UCCambioDataINDCOM" TagPrefix="UCCDS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione data limite domande di indennizzo per cessazione dell'attività commerciale - legge 145/2018</h2>
    </div>

    <table class="" width="720px">
        <tr>
            <td align="left" style="width: 720px">
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
            <td align="left" style="width: 720px">
                <UCCDS:UCCambioDataINDCOM runat="server" ID="ucCambioDataINDCOM" Visible="true"
                    OnShowAvviso="event_ucShowAvviso"/>
            </td>
        </tr>
    </table>
</asp:Content>
