<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="UCInfo.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCInfo" %>
<script language="javascript" type="text/javascript">


    $(document).ready(function () {
        var StatoPannello = "close";
        document.getElementById("<%=StatoPannello.ClientID%>").value = StatoPannello;
        StatoPannello = document.getElementById("<%=StatoPannello.ClientID%>").value;
        //Hide (Collapse) the toggle containers on load
        if (StatoPannello == "open") {
            $(".collapsibleContainer").show();
        }
        else {
            $(".collapsibleContainer").hide();
        }

        //Switch the "Open" and "Close" state per click then slide up/down (depending on open/close state)
        $("h2.trigger").click(function () {
            $(this).toggleClass("active").next().slideToggle("slow");
            if (StatoPannello == "open")
                StatoPannello = "close";
            else
                StatoPannello = "open";
            document.getElementById("<%=StatoPannello.ClientID%>").value = StatoPannello;
        });
    });    
</script>
<asp:Panel ID="pnlContainer" runat="server" Visible="true" CssClass="containerWidth xs accordion">
    <h2 class="trigger" style="width: 90%;">
        <a runat="server" id="linkDomanda" style="cursor: pointer">Domanda:
            <asp:Label runat="server" ID="lblDomanda" Font-Names="Verdana"></asp:Label></a></h2>
    <div class="collapsibleContainer PnlContenitoreDatiInterno" style="width: 98.6%;
        display: none">

        <div id="oldCruscotto">
            <table width="99%" class="TblInfo">
            <tr>
                <td style="width: 16%; text-align: right; font-weight: bold">
                    Tipo Domanda:
                </td>
                <td style="width: 45%; text-align: left;">
                    <asp:Label runat="server" ID="lblTipoDomandaOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 17%; text-align: right; font-weight: bold">
                    Stato Domanda:
                </td>
                <td style="width: 22%; text-align: left;">
                    <asp:Label runat="server" ID="lblStatoDomandaOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
        <table width="99%" class="TblInfo">
            <tr>
                <td style="width: 10%; text-align: right; font-weight: bold">
                    Gestione:
                </td>
                <td style="width: 25%; text-align: left;">
                    <asp:Label runat="server" ID="lblGestioneOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 7%; text-align: right; font-weight: bold">
                    Fondo:
                </td>
                <td style="width: 18%; text-align: left;">
                    <asp:Label runat="server" ID="lblFondoOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 7%; text-align: right; font-weight: bold">
                    Ente:
                </td>
                <td style="width: 19%; text-align: left;">
                    <asp:Label runat="server" ID="lblEnteOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 7%; text-align: right; font-weight: bold">
                    Filtro:
                </td>
                <td style="width: 7%; text-align: left;">
                    <asp:Label runat="server" ID="lblFiltroOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
        <table width="99%" class="TblInfo">
            <tr>
                <td style="width: 12%; text-align: right; font-weight: bold;">
                    Categoria:
                </td>
                <td style="width: 5%; text-align: left">
                    <asp:Label runat="server" ID="lblCategoriaOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 9%; text-align: right; font-weight: bold;">
                    Sede:
                </td>
                <td style="width: 11%; text-align: left">
                    <asp:Label runat="server" ID="lblSedeOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 13%; text-align: right; font-weight: bold;">
                    Certificato:
                </td>
                <td style="width: 11%; text-align: left">
                    <asp:Label runat="server" ID="lblCertificatoOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 7%; text-align: right; font-weight: bold;">
                    Tipo:
                </td>
                <td style="width: 5%; text-align: left;">
                    <asp:Label runat="server" ID="lblTipoOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 11%; text-align: right; font-weight: bold;" runat="server" id="tdLblUnicarpe">
                    Unicarpe:
                </td>
                <td style="width: 4%; text-align: left" runat="server" id="tdValueUnicarpe">
                    <asp:Label runat="server" ID="lblUnicarpeOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 8%; text-align: right; font-weight: bold;" runat="server" id="tdLblFonte"
                    visible="false">
                    Fonte:
                </td>
                <td style="width: 7%; text-align: left" runat="server" id="tdValueFonte" visible="false">
                    <asp:Label runat="server" ID="lblFonteOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 8%; text-align: right; font-weight: bold;">
                    Telematica:
                </td>
                <td style="width: 4%; text-align: left">
                    <asp:Label runat="server" ID="lblTelematicaOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
            </tr>
        </table>
        <hr id="hrSedeAttuale" runat="server" />
        <table width="99%" class="TblInfo">
            <tr>
                <td style="width: 20%; text-align: right; font-weight: bold;" runat="server" id="idTdLabelSedeAttuale">
                    Sede Di Gestione:
                </td>
                <td style="width: 75%; text-align: left" runat="server" id="idTdValoreSedeAttuale">
                    <asp:Label runat="server" ID="lblSedeAttualeOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
        <table width="99%" class="TblInfo">
            <tr>
                <td style="width: 18%; text-align: right; font-weight: bold;">
                    Codice Fiscale:
                </td>
                <td style="width: 20%; text-align: left">
                    <asp:Label runat="server" ID="lblCodiceFiscaleOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 16%; text-align: right; font-weight: bold;">
                    Nome:
                </td>
                <td style="width: 15%; text-align: left">
                    <asp:Label runat="server" ID="lblNomeOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
                <td style="width: 16%; text-align: right; font-weight: bold;">
                    Cognome:
                </td>
                <td style="width: 15%; text-align: left">
                    <asp:Label runat="server" ID="lblCognomeOld" CssClass="txtUppercase lblLightBlue"></asp:Label>
                </td>
            </tr>
        </table>
        </div>

        <div id="newCruscotto" style="display:none">
            <div class="row">
            <div class="col grid-item">
                <div class="label">Tipo domanda</div>
                <asp:Label runat="server" ID="lblTipoDomanda"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Stato domanda</div>
                <asp:Label runat="server" ID="lblStatoDomanda"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Gestione</div>
                <asp:Label runat="server" ID="lblGestione"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Fondo</div>
                <asp:Label runat="server" ID="lblFondo"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Tipologia (Filtro)</div>
                <asp:Label runat="server" ID="lblFiltro"></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col grid-item">
                <div class="label">Categoria</div>
                <asp:Label runat="server" ID="lblCategoria"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Sede</div>
                <asp:Label runat="server" ID="lblSede"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Tipo</div>
                <asp:Label runat="server" ID="lblTipo"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Unicarpe</div>
                <asp:Label runat="server" ID="lblUnicarpe"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Telematica</div>
                <asp:Label runat="server" ID="lblTelematica"></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col grid-item">
                <div class="label">Ente</div>
                <asp:Label runat="server" ID="lblEnte"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Certificato</div>
                <asp:Label runat="server" ID="lblCertificato"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Codice fiscale</div>
                <asp:Label runat="server" ID="lblCodiceFiscale"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Nome</div>
                <asp:Label runat="server" ID="lblNome"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Cognome</div>
                <asp:Label runat="server" ID="lblCognome"></asp:Label>
            </div>
        </div>

        <div class="row">
            <div class="col grid-item">
                <div class="label">Fonte</div>
                <asp:Label runat="server" ID="lblFonte"></asp:Label>
            </div>
            <div class="col grid-item">
                <div class="label">Sede Di Gestione</div>
                <asp:Label runat="server" ID="lblSedeAttuale"></asp:Label>
            </div>
            <div class="col grid-item"></div>
            <div class="col grid-item"></div>
            <div class="col grid-item"></div>
        </div>
        </div>
        
    </div>
    <asp:HiddenField runat="server" ID="StatoPannello" Value="open" EnableViewState="true" />
    <asp:HiddenField runat="server" ID="hCodiceCategoria" />
    <asp:HiddenField runat="server" ID="hTipoAutomazione" />
    <asp:HiddenField runat="server" ID="hGruppo" />
    <asp:HiddenField runat="server" ID="hProdotto" />
    <asp:HiddenField runat="server" ID="hTipo" />
    <asp:HiddenField runat="server" ID="hGestione" />
    <asp:HiddenField runat="server" ID="hFondo" />
    <asp:HiddenField runat="server" ID="hEnte" />
      <asp:HiddenField runat="server" ID="hDecorrenzaPensione" />
      <asp:HiddenField runat="server" ID="hCodiceFase" />
</asp:Panel>
