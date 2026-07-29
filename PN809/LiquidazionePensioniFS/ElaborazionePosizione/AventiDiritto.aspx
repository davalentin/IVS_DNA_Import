<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="AventiDiritto.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AventiDiritto" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/AventiDiritto/UCAventiDiritto.ascx" TagName="UCAventiDiritto"
    TagPrefix="UCAD" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
    <script type="text/javascript" src="Javascript/validate2.js"></script>
    <script type="text/javascript" src="Javascript/Utility.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            GestioneVisibilitaPannelliRepeater();
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });

        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlTabAventiDiritto.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate("Selezione");
                if (flag)
                    flag = Page_ClientValidate('UCAventiDiritto');
                if (flag)
                    flag = Page_ClientValidate('UCAventiDirittoGrid');
            }
            return flag;
        }

        function confirmPage() {
            if (isConfirmPopUp())
                $('#dialog-confirm').dialog('open');
            else
                document.getElementById('<%= btnSalvaTutto.ClientID %>').click();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#aventiDiritto" runat="server" />
    <asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCAventiDiritto"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummaryGrid" ValidationGroup="UCAventiDirittoGrid"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummarySelezione" ValidationGroup="Selezione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlAventiPeriodo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerMinWidth">
            <ul class="tabsLine2 tabs" style="width: 1000px">
                <asp:Panel runat="server" ID="pnlTabAventiDiritto">
                    <li><a href="#aventiDiritto">Aventi Diritto
                        <asp:Image ID="imgAventiDiritto" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="width: 1000px">
                <div id="aventiDiritto" class="tab_content">
                    <UCAD:UCAventiDiritto runat="server" ID="ucAventiDiritto" OnShowAvviso="event_ucShowAvviso"
                        OnAggiornaSemaforo="event_ucAggiornaSemaforo" />
                </div>
            </div>
            <table style="width: 1000px" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPopUp" runat="server" Text="Salva Tutto" SkinID="btnAzione1" CausesValidation="false"
                            Width="180px" OnClientClick="if(mainValidateForConfirm()){ confirmPage();} return false;" CssClass="tertiary" />
                        <asp:Button ID="btnSalvaTutto" runat="server" Text="Salva Tutto" SkinID="btnAzione1"
                            CausesValidation="false" Width="180px" OnClick="btnSalvaTutto_Click" OnClientClick="mainValidate()"
                            Style="display: none"  CssClass="tertiary"/>
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="180px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
