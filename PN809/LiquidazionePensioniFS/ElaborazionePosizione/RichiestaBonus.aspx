<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="RichiestaBonus.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.RichiestaBonus" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/RichiestaBonus/UCRichiestaBonus.ascx" TagName="UCRichiestaBonus"
    TagPrefix="UCRB" %>
<%@ Register Src="~/UserControls/RichiestaBonus/UCEsitoPrenotazione.ascx" TagName="UCEsitoPrenotazione"
    TagPrefix="UCEP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
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
            return true;
        }

        function CreatePopUpPrescrizione() {
            // jQuery UI Dialog
            var result;
            $('#dialog-confirmPage').dialog({
                autoOpen: false,
                width: 450,
                height: 220,
                modal: true,
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'hidden'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    "Chiudi": function () {
                        $(this).dialog("close");
                        result = false;
                    },
                    'Ok': function () {
                        $(this).dialog('close');
                        $("#<%= HiddenFieldIsConfermato.ClientID %>").val("SI");
                        document.getElementById('<%= btnSalva.ClientID %>').click();
                        return true;
                    }
                }
            });
            $("#dialog-confirmPage").parent().appendTo($("form:first"));
        }
        function ShowPopUpPrescrizione() {
            CreatePopUpPrescrizione();
            $('#dialog-confirmPage').dialog('open');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#richiesta_bonus"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCRichiestaBonus"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlRichiestaBonus">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <div>
                <ul class="tabsLine2 tabs">
                    <asp:Panel runat="server" ID="pnlTabRichiestaBonus">
                        <li><a href="#richiesta_bonus">Richiesta Bonus
                            <asp:Image ID="imgRichiestaBonus" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabEsitoPrenotazione">
                        <li><a href="#esito_prenotazione">Esito Prenotazione
                            <asp:Image ID="imgEsitoPrenotazione" ImageAlign="Top" runat="server" Visible="false" /></a></li>
                    </asp:Panel>
                </ul>
            </div>
            <div class="tab_container" style="min-height: 90px; padding-top: 15px">
                <div id="richiesta_bonus" class="tab_content">
                    <UCRB:UCRichiestaBonus runat="server" ID="ucRichiestaBonus" OnEliminazioneRichiestaBonus="event_ucEliminazioneRichiestaBonus"
                        OnShowAvviso="event_ucShowAvviso" OnHideAvviso="event_ucHideAvviso" OnSalvaRichiestaBonus="SalvaRichiestaBonus" />
                </div>
                <div id="esito_prenotazione" class="tab_content">
                    <UCEP:UCEsitoPrenotazione runat="server" ID="ucEsitoPrenotazione" OnShowAvviso="event_ucShowAvviso"
                        OnHideAvviso="event_ucHideAvviso" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="150px" OnClick="SalvaRichiestaBonus" OnClientClick="mainValidate()" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="aspnetForm.target ='_self'; BlockUI()"
                            PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="display: none">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            <asp:Label ID="lblPrescrizione" runat="server">Sono stati selezionati degli anni soggetti a prescrizione. Si desidera procedere?</asp:Label></p>
    </div>
    <asp:HiddenField runat="server" ID="HiddenFieldIsConfermato" Value="" />
</asp:Content>
