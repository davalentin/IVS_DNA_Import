<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Familiare.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Familiare" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Familiari/UCFamiliari.ascx" TagName="UCFamiliari"
    TagPrefix="UCF" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });

        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlRiepilogoFamiliari.ClientID%>") != null) {
                flag = Page_ClientValidate('UCFamiliariCF');
            }
            if (flag) {
                if (document.getElementById("<%=pnlRiepilogoFamiliari.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCFamiliari');
                }
            }

            return flag;
        }

        function CreatePopUpConsultazioneANF() {
            // jQuery UI Dialog
            var result;
            $('#consultazioneANF').dialog({
                autoOpen: false,
                width: 720,
                height: 360,
                modal: true,
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'hidden'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    "OK": function () {
                        $(this).dialog("close");
                        result = false;
                    }
                }
            });
            $("#consultazioneANF").parent().appendTo($("form:first"));
        }

        function ShowPopUpConsultazioneANF() {
            CreatePopUpConsultazioneANF();
            $('#consultazioneANF').dialog('open');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="consultazioneANF" style="display: none;" title="Consultazione Unificata ANF">
        <asp:Label ID="lblConsultazioneANF" runat="server"></asp:Label>
    </div>
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#familiari" runat="server" />
    <asp:ValidationSummary runat="server" ID="validateSummaryCF" ValidationGroup="UCFamiliariCF"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCFamiliari"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlFamiliari" OnFamiliariSalvati="FamiliariSalvati">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth md">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlRiepilogoFamiliari">
                    <li><a href="#familiari">Familiari
                        <asp:Image ID="imgRiepilogo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container">
                <div id="familiari" class="tab_content">
                    <UCF:UCFamiliari runat="server" ID="ucFamiliari" OnSalvaFamiliari="FamiliariSalvati"
                        OnFamiliariNonSalvati="FamiliariNonSalvati" OnAddModFamiliareEvent="event_ucAddModFamiliare"
                        OnEliminaFamiliari="FamiliariEliminati" OnShowAvviso="event_ucShowAvviso" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="width: 7px;">
                    </td>
                    <td align="center">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="150px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HdnCodiceFiscaleANF" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="HdnDataConsultazioneANF" runat="server"></asp:HiddenField>
</asp:Content>
