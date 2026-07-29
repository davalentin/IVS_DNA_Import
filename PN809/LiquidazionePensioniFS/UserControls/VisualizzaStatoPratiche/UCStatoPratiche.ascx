<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCStatoPratiche.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.VisualizzaStatoPratiche.UCStatoPratiche" %>
<script type="text/javascript">
    $(document).ready(function () {
        var userControlName = '<%=this.ClientID %>';

        if (document.getElementById(userControlName + "_<%=addButton.ID %>") != null) {
            document.getElementById(userControlName + "_<%=addButton.ID %>").style.display = "none";
        }

        if (document.getElementById(userControlName + "_<%=removeButton.ID %>") != null) {
            document.getElementById(userControlName + "_<%=removeButton.ID %>").style.display = "none";
        }

        if ((document.getElementById("<%=ucHdnNCriteri.ClientID %>").value == "0")) {
            DisabilitaTutti('<%=this.ClientID %>');
        }
        if ((document.getElementById(userControlName + "_<%=ddlVisualizzazioneStatoPratiche.ID %>").value) != "") {
            ddlScelta('<%=this.ClientID %>');
        }
        else {

            ddlScelta('<%=this.ClientID %>');
        }

        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        //alert(availableTags);
        $("#<%=txtSede.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        document.getElementById("<%=pnlStatoPratiche.ClientID %>").removeAttribute("style");

    });


    function AddCriterioClient_Click() {
        document.getElementById("<%=ucHdnNCriteri.ClientID %>").value = "1";
        return false;

    }

    function DisabilitaTutti(userControlName) {
        document.getElementById(userControlName + "_trNumeroDomanda").style.display = "none";
        document.getElementById(userControlName + "_trCategoriaPensione").style.display = "none";
        document.getElementById(userControlName + "_trStatoPratica").style.display = "none";
        document.getElementById(userControlName + "_trSede").style.display = "none";
        document.getElementById(userControlName + "_trFondo").style.display = "none";
        document.getElementById(userControlName + "_trCassa").style.display = "none";
        document.getElementById(userControlName + "_trAnagrafica").style.display = "none";
        document.getElementById(userControlName + "_trCodiceFiscale").style.display = "none";
        document.getElementById(userControlName + "_trDataPresentazione").style.display = "none";
        document.getElementById(userControlName + "_trDataElaborazione").style.display = "none";
        document.getElementById(userControlName + "_trMatricola").style.display = "none";
        document.getElementById(userControlName + "_trTipoDomandaInLavorazione").style.display = "none";
        document.getElementById(userControlName + "_trTipoDomandaLavorata").style.display = "none";
        document.getElementById(userControlName + "_trGruppo").style.display = "none";
        document.getElementById(userControlName + "_trProdotto").style.display = "none";
        document.getElementById(userControlName + "_trTipo").style.display = "none";

        document.getElementById(userControlName + "_<%=txtNumeroDomanda.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlCategoriaPensione.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlStatoPratica.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtSede.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlFondo.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlCassa.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtNome.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtCognome.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtCodiceFiscale.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtDataPresentazioneMin.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtDataPresentazioneMax.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtDataElaborazioneMin.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtDataElaborazioneMax.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=txtMatricola.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlTipoDomandaInLavorazione.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlTipoDomandaLavorata.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlGruppo.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlProdotto.ID %>").disabled = true;
        document.getElementById(userControlName + "_<%=ddlTipo.ID %>").disabled = true;
        return;
    }

    function SetReadOnly(userControlName) {
        document.getElementById(userControlName + "_<%=txtNumeroDomanda.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlCategoriaPensione.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlStatoPratica.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtSede.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlFondo.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlCassa.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtNome.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtCognome.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtCodiceFiscale.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtDataPresentazioneMin.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtDataPresentazioneMax.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtDataElaborazioneMin.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtDataElaborazioneMax.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=txtMatricola.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlTipoDomandaInLavorazione.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlTipoDomandaLavorata.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlGruppo.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlProdotto.ID %>").readOnly = true;
        document.getElementById(userControlName + "_<%=ddlTipo.ID %>").readOnly = true;
        return;
    }

    function ddlScelta(userControlName) {
        var ddlValue = document.getElementById(userControlName + "_<%=ddlVisualizzazioneStatoPratiche.ID %>").value;

        if (document.getElementById("<%=addButton.ClientID %>") != null) {
            document.getElementById("<%=addButton.ClientID %>").style.display = "block";
        }
        if ((document.getElementById("<%=removeButton.ClientID %>") != null) && (userControlName != "ctl00_ContentPlaceHolder1_ucStatoPratiche")) {
            document.getElementById("<%=removeButton.ClientID %>").style.display = "block";
        }

        //Controllo per rimuovere il pulsante Aggiungi se mi trovo nello UC ucStatoPratiche3
        if ((document.getElementById("<%=addButton.ClientID %>") != null) && (userControlName == "ctl00_ContentPlaceHolder1_ucStatoPratiche3")) {
            document.getElementById("<%=addButton.ClientID %>").style.display = "none";
        }

        switch (ddlValue) {
            case "":
                if (document.getElementById("<%=addButton.ClientID %>") != null) {
                    document.getElementById("<%=addButton.ClientID %>").style.display = "none";
                }
                DisabilitaTutti(userControlName);
                break;
            case "NumeroDomanda":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trNumeroDomanda").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtNumeroDomanda.ID %>").disabled = false;
                if (document.getElementById(userControlName + "_<%=addButton.ID %>") != null) {
                    document.getElementById(userControlName + "_<%=addButton.ID %>").style.display = "none";
                }
                break;
            case "CategoriaPensione":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trCategoriaPensione").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlCategoriaPensione.ID %>").disabled = false;

                break;
            case "StatoPratica":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trStatoPratica").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlStatoPratica.ID %>").disabled = false;
                break;
            case "Sede":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trSede").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtSede.ID %>").disabled = false;
                break;
            case "Fondo":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trFondo").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlFondo.ID %>").disabled = false;
                break;
            case "Cassa":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trCassa").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlCassa.ID %>").disabled = false;
                break;
            case "Anagrafica":
                DisabilitaTutti(userControlName);

                document.getElementById(userControlName + "_trAnagrafica").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtNome.ID %>").disabled = false;
                document.getElementById(userControlName + "_<%=txtCognome.ID %>").disabled = false;
                break;
            case "CodiceFiscale":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trCodiceFiscale").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtCodiceFiscale.ID %>").disabled = false;
                break;
            case "DataPresentazione":
                DisabilitaTutti(userControlName);

                document.getElementById(userControlName + "_trDataPresentazione").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtDataPresentazioneMin.ID %>").disabled = false;
                document.getElementById(userControlName + "_<%=txtDataPresentazioneMax.ID %>").disabled = false;
                break;
            case "DataElaborazione":
                DisabilitaTutti(userControlName);

                document.getElementById(userControlName + "_trDataElaborazione").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtDataElaborazioneMin.ID %>").disabled = false;
                document.getElementById(userControlName + "_<%=txtDataElaborazioneMax.ID %>").disabled = false;
                break;
            case "Matricola":
                DisabilitaTutti(userControlName);

                document.getElementById(userControlName + "_trMatricola").style.display = "table-row";
                document.getElementById(userControlName + "_<%=txtMatricola.ID %>").disabled = false;
                if (document.getElementById(userControlName + "_<%=txtMatricola.ID %>").value == "")
                    document.getElementById(userControlName + "_<%=txtMatricola.ID %>").value = document.getElementById("<%=HiddenFieldMatricolaValue.ClientID%>").value;
                if (document.getElementById("<%=HiddenFieldMatricolaEnabled.ClientID%>").value == "false")
                    document.getElementById(userControlName + "_<%=txtMatricola.ID %>").readOnly = true;
                break;
            case "PL/TRFe/oRICinlavorazione":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trTipoDomandaInLavorazione").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlTipoDomandaInLavorazione.ID %>").disabled = false;
                break;
            case "PL/TRFe/oRIClavorate":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trTipoDomandaLavorata").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlTipoDomandaLavorata.ID %>").disabled = false;
                break;
            case "Gruppo":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trGruppo").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlGruppo.ID %>").disabled = false;
                break;
            case "Prodotto":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trProdotto").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlProdotto.ID %>").disabled = false;
                break;
            case "Tipo":
                DisabilitaTutti(userControlName);
                document.getElementById(userControlName + "_trTipo").style.display = "table-row";
                document.getElementById(userControlName + "_<%=ddlTipo.ID %>").disabled = false;
                break;
        }
        return;
    }

    function DisabilitaValidationSummary() {
        if (typeof (Page_ValidationSummaries) != "undefined") { //hide the validation summaries
            for (sums = 0; sums < Page_ValidationSummaries.length; sums++) {
                summary = Page_ValidationSummaries[sums];
                summary.style.display = "none";
            }
        }
    }

      
</script>
<asp:Panel runat="server" ID="pnlStatoPratiche" Width="720px" Style="display: none">
    <table class="tabellaFormattazione tableSearch tableSearch__grid" style="width: 720px">
        <tr style="width: 720px">
            <td style="width: 180px" class="field">
                <asp:DropDownList runat="server" CssClass="tb8" ID="ddlVisualizzazioneStatoPratiche"
                    Width="200px" TabIndex="1">
                    <asp:ListItem Text="" Value="" />
                </asp:DropDownList>
                <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                    ValidationGroup="VisualizzaStatoPratiche" ID="ddlVisualizzazioneStatoPratiche_CV"
                    ClientValidationFunction="validateDropDownList" ErrorMessage="Scegliere Criterio di ricerca"
                    Width="0px" />
            </td>
            <td style="width: 400px" class="field">
                <table width="400px" style="table-layout: fixed" class="tableSearch tableSearch__grid tableSearch__grid--inner">
                    <tr id="trNumeroDomanda" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblBlanckNumeroDomanda" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" MaxLength="13" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda"
                                Width="145px" TabIndex="8"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" Display="None" runat="server" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtNumeroDomanda_CV" ClientValidationFunction="validateNumeroDomanda"
                                ErrorMessage="Numero domanda non valido" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trCategoriaPensione" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblCategoriaPensione" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlCategoriaPensione" CssClass="tb8 txtUppercase"
                                Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlCategoriaPensione_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere la categoria da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trStatoPratica" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblStatoPratica" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 346px" class="field full-grid" align="left" colspan="3">
                            <asp:DropDownList runat="server" Width="185px" ID="ddlStatoPratica" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlStatoPratica_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere lo stato della pratica da ricercare" Width="0px" />
                        </td>
                    </tr>
                    <tr id="trSede" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblSede" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 346px" class="field full-grid" align="left" colspan="3">
                            <asp:TextBox runat="server" ID="txtSede" CssClass="tb8 txtUppercase" Width="340px"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" Display="None" runat="server" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtSede_CV" ClientValidationFunction="validateSede"
                                ErrorMessage="Scegliere la sede da ricercare" Width="0px" />
                        </td>
                    </tr>
                    <tr id="trFondo" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblFondo" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlFondo" CssClass="tb8 txtUppercase" Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlFondo_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere il fondo da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trCassa" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblCassa" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlCassa" CssClass="tb8 txtUppercase" Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlCassa_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere la cassa da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trAnagrafica" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblCognome" Text="Cognome:" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtCognome" CssClass="tb8 txtUppercase" Width="145px"
                                TabIndex="1"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtCognome_CV" ClientValidationFunction="validateCognomeNome"
                                ErrorMessage="Cognome non valido" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                            <asp:Label runat="server" ID="lblNome" Text="Nome:" Width="46px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtNome" CssClass="tb8 txtUppercase" Width="145px"
                                TabIndex="2"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtNome_CV" ClientValidationFunction="validateCognomeNome"
                                ErrorMessage="Nome non valido" Width="0px" />
                        </td>
                    </tr>
                    <tr id="trCodiceFiscale" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblCodiceFiscale" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtCodiceFiscale" CssClass="tb8 txtUppercase" MaxLength="16"
                                Width="145px"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtCodiceFiscale_CV" ClientValidationFunction="validateCodiceFiscale"
                                ErrorMessage="Codice fiscale non valido" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trDataPresentazione" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblDataPresentazioneDal" Text="Dal:" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtDataPresentazioneMin" Width="110px" CssClass="tb8 txtUppercase date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10" TabIndex="1"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataPresentazioneMin_CV" ClientValidationFunction="validateDateForToDay"
                                ErrorMessage="Data Presentazione (Dal:) non valida. Rispettare il formato GG/MM/AAAA e indicare una data anteriore all'odierna"
                                Width="0px" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataPresentazioneMin" Display="Dynamic"
                                ErrorMessage="Data Presentazione Dal: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="VisualizzaStatoPratiche"
                                ID="customCheckDataDataPresentazioneMin" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                            <asp:Label runat="server" ID="lblDataPresentazioneAl" Text="Al:" Width="46px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtDataPresentazioneMax" Width="110px" CssClass="tb8 txtUppercase date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10" TabIndex="2"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataPresentazioneMax_CV" ClientValidationFunction="validateDateForToDay"
                                ErrorMessage="Data Presentazione (Al:) non valida. Rispettare il formato GG/MM/AAAA e indicare una data anteriore all'odierna"
                                Width="0px" />
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataPresentazione_CV" ClientValidationFunction="validateDataSequence"
                                ErrorMessage="Data Presentazione (Dal:) è successiva a Data Presentazione (Al:)"
                                Width="0px" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataPresentazioneMax" Display="Dynamic"
                                ErrorMessage="Data Presentazione Al: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="VisualizzaStatoPratiche"
                                ID="customCheckDataDataPresentazioneMax" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                    </tr>
                    <tr id="trDataElaborazione" runat="server" style="width: 400px">
                        <td style="width: 62px;" class="field" align="right">
                            <asp:Label runat="server" ID="lblDataElaborazioneDal" Text="Dal:" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtDataElaborazioneMin" Width="110px" CssClass="tb8 txtUppercase date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10" TabIndex="1"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataElaborazioneMin_CV" ClientValidationFunction="validateDateForToDay"
                                ErrorMessage="Data Elaborazione (Dal:) non valida. Rispettare il formato GG/MM/AAAA e indicare una data anteriore all'odierna"
                                Width="0px" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataElaborazioneMin" Display="Dynamic"
                                ErrorMessage="Data Elaborazione Dal: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="VisualizzaStatoPratiche"
                                ID="customCheckDataDataElaborazioneMin" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                        <td style="width: 46px;" class="field" align="right">
                            <asp:Label runat="server" ID="lblDataElaborazioneAl" Text="Al:" Width="46px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" ID="txtDataElaborazioneMax" Width="110px" CssClass="tb8 txtUppercase date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10" TabIndex="2"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataElaborazioneMax_CV" ClientValidationFunction="validateDateForToDay"
                                ErrorMessage="Data Elaborazione (Al:) non valida. Rispettare il formato GG/MM/AAAA e indicare una data anteriore all'odierna"
                                Width="0px" />
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtDataElaborazione_CV" ClientValidationFunction="validateDataSequence"
                                ErrorMessage="Data Elaborazione (Dal:) è successiva a Data Elaborazione (Al:)"
                                Width="0px" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataElaborazioneMax" Display="Dynamic"
                                ErrorMessage="Data Elaborazione Al: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="VisualizzaStatoPratiche"
                                ID="customCheckDataDataElaborazionemax" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                    </tr>
                    <tr id="trMatricola" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblMatricola" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:TextBox runat="server" MaxLength="8" CssClass="tb8 txtUppercase" ID="txtMatricola"
                                Width="145px" TabIndex="8"></asp:TextBox>
                            <asp:CustomValidator EnableClientScript="true" Display="None" runat="server" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="txtMatricola_CV" ClientValidationFunction="validateMatricola"
                                ErrorMessage="Matricola non valida" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trTipoDomandaInLavorazione" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblTipoDomandaInLavorazione" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" Width="145px" ID="ddlTipoDomandaInLavorazione" CssClass="tb8 txtUppercase">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="PL" Text="PL/TRF"></asp:ListItem>
                                <asp:ListItem Value="RIC" Text="RIC"></asp:ListItem>
                                <asp:ListItem Value="ALL" Text="PL/TRF e RIC"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlTipoDomandaInLavorazione_CV"
                                ClientValidationFunction="validateDropDownList" ErrorMessage="Scegliere il tipo di domande in lavorazione da ricercare"
                                Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trTipoDomandaLavorata" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblTipoDomandaLavorata" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" Width="145px" ID="ddlTipoDomandaLavorata" CssClass="tb8 txtUppercase">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="PL" Text="PL/TRF"></asp:ListItem>
                                <asp:ListItem Value="RIC" Text="RIC"></asp:ListItem>
                                <asp:ListItem Value="ALL" Text="PL/TRF e RIC"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlTipoDomandaLavorata_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere il tipo di domande lavorate da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trGruppo" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblGruppo" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlGruppo" CssClass="tb8 txtUppercase" Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlGruppo_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere il gruppo da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trProdotto" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblProdotto" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlProdotto" CssClass="tb8 txtUppercase" Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlProdotto_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere il Prodotto da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                    <tr id="trTipo" runat="server" style="width: 400px">
                        <td style="width: 62px" class="field" align="right">
                            <asp:Label runat="server" ID="lblTipo" Width="62px"></asp:Label>
                        </td>
                        <td style="width: 148px" class="field" align="left">
                            <asp:DropDownList runat="server" ID="ddlTipo" CssClass="tb8 txtUppercase" Width="145px">
                            </asp:DropDownList>
                            <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                                ValidationGroup="VisualizzaStatoPratiche" ID="ddlTipo_CV" ClientValidationFunction="validateDropDownList"
                                ErrorMessage="Scegliere il Tipo da ricercare" Width="0px" />
                        </td>
                        <td style="width: 46px" class="field" align="right">
                        </td>
                        <td style="width: 148px" class="field" align="left">
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 70px; vertical-align: middle;" align="right" runat="server" id="btnAggiungi">
                <asp:Button CssClass="tertiary" ID="addButton" Width="80px" runat="server" Text="Aggiungi" SkinID="btnAzione1"
                    OnClick="AddParametro" CausesValidation="false" OnClientClick="if(Page_ClientValidate('VisualizzaStatoPratiche')){aspnetForm.target ='_self'; BlockUI();}" />
            </td>
            <td style="width: 70px; vertical-align: middle;" runat="server" id="btnRemove">
                <asp:Button ID="removeButton" Width="80px" runat="server" Text="Rimuovi" SkinID="btnAzione1"
                    CausesValidation="false" OnClick="RemoveParametro" OnClientClick="BlockUI()" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
    <input type="hidden" id="DoCheckDataPresentazioneSequence" value="true" />
    <input type="hidden" id="DoCheckDataElaborazioneSequence" value="true" />
    <asp:HiddenField runat="server" ID="ucHdnNCriteri" Value="0" />
    <asp:HiddenField runat="server" ID="HiddenFieldSedi" />
    <asp:HiddenField runat="server" ID="HiddenFieldMatricolaValue" />
    <asp:HiddenField runat="server" ID="HiddenFieldMatricolaEnabled" />
</asp:Panel>
