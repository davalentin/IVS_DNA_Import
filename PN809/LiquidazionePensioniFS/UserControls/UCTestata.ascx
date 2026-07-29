<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCTestata.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCTestata" %>

<script type="text/javascript">
    function ShowSegnalazione() {

        CreateSegnalazione();
        EnableValidationDialog();
        CleanEdit();
        $('#dialogSegnalazione').dialog('open');
    }

    function ShowRemedy() {
        window.open(document.getElementById("<%= hRemedy.ClientID %>").value);
    }


    function ShowEsitoSegnalazione() {

        CreateEsitoSegnalazione();
        $('#dialogEsitoSegnalazione').dialog('open');
    }

    function CreateSegnalazione() {
        $('#dialogSegnalazione').dialog(
            {
                autoOpen: false,
                show: 'blind',
                hide: 'blind',
                width: 650,
                modal: true,
                resizable: false,
                draggable: true,
                centerX: true,
                centerY: true,
                open: function (event, ui) {
                    $('body').css('overflow-x', 'hidden');
                    $('body').css('overflow-y', 'auto');
                    $('.ui-widget-overlay').css('width', '100%');
                    $('#dialogSegnalazione').addClass("ui-dialog-content--scroll-padding");
                },
                close: function (event, ui) {
                    $('body').css('overflow', 'auto');
                },
                buttons:
                {
                    'Chiudi': function () {
                        DisableValidationDialog();
                        document.getElementById('<%= hEsito.ClientID %>').value = "";
                        $(this).dialog('close');
                    },
                    'Invia': function () {
                        if (Page_ClientValidate("DialSegnalazione")) {
                            DisableValidationDialog();

                            var ddl = document.getElementById('<%=ddlDestinatario.ClientID %>');
                            var selected = ddl.options[ddl.selectedIndex].value;
                            document.getElementById('<%=hDestinatario.ClientID%>').value = selected;
                            ddl = document.getElementById('<%=ddlTipologia.ClientID %>');
                            selected = ddl.options[ddl.selectedIndex].innerHTML;
                            document.getElementById('<%=hTipologia.ClientID %>').value = selected;
                            document.getElementById('<%=hMessaggio.ClientID%>').value = document.getElementById('<%=txtMessaggio.ClientID%>').value;
                            document.getElementById('<%=hTelefono.ClientID%>').value = document.getElementById('<%=txtTelefono.ClientID%>').value;
                            document.getElementById('<%=hNumeroDomus.ClientID %>').value = document.getElementById('<%=txtNumeroDomus.ClientID %>').value;
                            document.getElementById('<%=hCodiceFiscale.ClientID %>').value = document.getElementById('<%=txtCodiceFiscale.ClientID %>').value;
                            document.getElementById('<%=hCategoria.ClientID %>').value = document.getElementById('<%=txtCategoria.ClientID %>').value;
                            document.getElementById('<%=hSede.ClientID %>').value = document.getElementById('<%=txtSede.ClientID %>').value;
                            document.getElementById('<%=hCertificato.ClientID %>').value = document.getElementById('<%=txtCertificato.ClientID %>').value;
                            document.getElementById('<%=hCodiceErrore.ClientID %>').value = document.getElementById('<%=txtCodiceErrore.ClientID %>').value;
                            document.getElementById('<%=hDecorrenzaPensione.ClientID %>').value = document.getElementById('<%=txtDecorrenzaPensione.ClientID %>').value;
                            $(this).dialog('close');
                            document.getElementById('<%= btnInviaSegnalazione.ClientID %>').click();
                        }
                    }
                }
            });

        $('#dialogSegnalazione').ready(function () {
        });
    }

    function CreateEsitoSegnalazione() {
        $('#dialogEsitoSegnalazione').dialog(
            {
                autoOpen: false,
                show: 'blind',
                hide: 'blind',
                width: 450,
                modal: true,
                resizable: false,
                draggable: true,
                centerX: true,
                centerY: true,
                open: function (event, ui) {
                    $('body').css('overflow-x', 'hidden'); $('body').css('overflow-y', 'auto'); $('.ui-widget-overlay').css('width', '100%');
                    $('.ui-dialog').css('text-align', 'center'); $('.ui-dialog-title').css('text-align', 'center'); $('.ui-dialog-title').css('width', '100%');
                    $('.ui-dialog-buttonpane').css('text-align', 'center'); $('.ui-dialog-buttonset').css('float', 'none');
                },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons:
                {
                    'OK': function () {
                        $(this).dialog('close');
                    }
                }
            });
    }

    function DisableValidationDialog() {
        ValidatorEnable(document.getElementById('<%= RFdestinatario.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= RFTipologia.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= RFmessaggio.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REtelefono.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= RFVtxtTelefono.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtNumeroDomus.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= txtCodiceFiscale_CV.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtCategoria.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtSede.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtCertificato.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtCodiceErrore.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtNumeroDomus.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtCodiceFiscale.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtCategoria.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtSede.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtCertificato.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtCodiceErrore.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= CVtxtDecorrenzaPensione.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= RFVtxtDecorrenzaPensione.ClientID %>'), false);

        document.getElementById('<%= ValidationSegnalazione.ClientID %>').style.display = "none";
    }

    function EnableValidationDialog() {
        ValidatorEnable(document.getElementById('<%= RFdestinatario.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= RFTipologia.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= RFmessaggio.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= REtelefono.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= RFVtxtTelefono.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= REVtxtNumeroDomus.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= txtCodiceFiscale_CV.ClientID %>'), false);
        ValidatorEnable(document.getElementById('<%= REVtxtCategoria.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= REVtxtSede.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= REVtxtCertificato.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= REVtxtCodiceErrore.ClientID %>'), true);
        ValidatorEnable(document.getElementById('<%= CVtxtDecorrenzaPensione.ClientID %>'), true);

        <%--var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        if (selected === "a" || selected === "b" || selected === "c") {
            ValidatorEnable(document.getElementById('<%= CVtxtNumeroDomus.ClientID %>'), true);
            ValidatorEnable(document.getElementById('<%= CVtxtCodiceFiscale.ClientID %>'), true);
            ValidatorEnable(document.getElementById('<%= CVtxtCategoria.ClientID %>'), true);
            ValidatorEnable(document.getElementById('<%= CVtxtSede.ClientID %>'), true);
            ValidatorEnable(document.getElementById('<%= CVtxtCertificato.ClientID %>'), true);
        }
        else {
            ValidatorEnable(document.getElementById('<%= CVtxtNumeroDomus.ClientID %>'), false);
            ValidatorEnable(document.getElementById('<%= CVtxtCodiceFiscale.ClientID %>'), false);
            ValidatorEnable(document.getElementById('<%= CVtxtCategoria.ClientID %>'), false);
            ValidatorEnable(document.getElementById('<%= CVtxtSede.ClientID %>'), false);
            ValidatorEnable(document.getElementById('<%= CVtxtCertificato.ClientID %>'), false);
        }

        if (selected === "c") {
            ValidatorEnable(document.getElementById('<%= CVtxtCodiceErrore.ClientID %>'), true);
            ValidatorEnable(document.getElementById('<%= RFVtxtDecorrenzaPensione.ClientID %>'), true);
        }
        else {
            ValidatorEnable(document.getElementById('<%= CVtxtCodiceErrore.ClientID %>'), false);
            ValidatorEnable(document.getElementById('<%= RFVtxtDecorrenzaPensione.ClientID %>'), false);
        }--%>
    }

    function CleanEdit() {
        if (document.getElementById('<%= hEsito.ClientID %>').value == "") {
            var ddl = document.getElementById('<%= ddlDestinatario.ClientID %>');
            ddl.options[0].selected = true;
            ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
            ddl.options[0].selected = true;

            document.getElementById('<%= txtMessaggio.ClientID %>').value = "";
            document.getElementById('<%= txtTelefono.ClientID %>').value = "";
            document.getElementById('<%= txtNumeroDomus.ClientID %>').value = "";
            document.getElementById('<%= txtCodiceFiscale.ClientID %>').value = "";
            document.getElementById('<%= txtCategoria.ClientID %>').value = "";
            document.getElementById('<%= txtSede.ClientID %>').value = "";
            document.getElementById('<%= txtCertificato.ClientID %>').value = "";
            document.getElementById('<%= txtCodiceErrore.ClientID %>').value = "";
            document.getElementById('<%= lblEsitoError.ClientID %>').innerHTML = "";
            document.getElementById('<%= hDestinatario.ClientID %>').value = "";
            document.getElementById('<%= hTipologia.ClientID %>').value = "";
            document.getElementById('<%= hMessaggio.ClientID %>').value = "";
            document.getElementById('<%= hTelefono.ClientID %>').value = "";
            document.getElementById('<%= hNumeroDomus.ClientID %>').value = "";
            document.getElementById('<%= hCodiceFiscale.ClientID %>').value = "";
            document.getElementById('<%= hCategoria.ClientID %>').value = "";
            document.getElementById('<%= hSede.ClientID %>').value = "";
            document.getElementById('<%= hCertificato.ClientID %>').value = "";
            document.getElementById('<%= hCodiceErrore.ClientID %>').value = "";
            document.getElementById('<%= txtDecorrenzaPensione.ClientID %>').value = "";
            document.getElementById('<%= hDecorrenzaPensione.ClientID %>').value = "";
        }
        else {
            var ddl = document.getElementById('<%=ddlDestinatario.ClientID %>');
            setSelectedIndex(ddl, document.getElementById('<%= hDestinatario.ClientID %>').value);
            ddl = document.getElementById('<%=ddlTipologia.ClientID %>');
            setSelectedIndex(ddl, document.getElementById('<%= hTipologia.ClientID %>').value);

            document.getElementById('<%= txtMessaggio.ClientID %>').value = document.getElementById('<%= hMessaggio.ClientID %>').value;
            document.getElementById('<%= txtTelefono.ClientID %>').value = document.getElementById('<%= hTelefono.ClientID %>').value;
            document.getElementById('<%= txtNumeroDomus.ClientID %>').value = document.getElementById('<%= hNumeroDomus.ClientID %>').value;
            document.getElementById('<%= txtCodiceFiscale.ClientID %>').value = document.getElementById('<%= hCodiceFiscale.ClientID %>').value;
            document.getElementById('<%= txtCategoria.ClientID %>').value = document.getElementById('<%= hCategoria.ClientID %>').value;
            document.getElementById('<%= txtSede.ClientID %>').value = document.getElementById('<%= hSede.ClientID %>').value;
            document.getElementById('<%= txtCertificato.ClientID %>').value = document.getElementById('<%= hCertificato.ClientID %>').value;
            document.getElementById('<%= txtCodiceErrore.ClientID %>').value = document.getElementById('<%= hCodiceErrore.ClientID %>').value;
            document.getElementById('<%= lblEsitoError.ClientID %>').innerHTML = document.getElementById('<%= hEsito.ClientID %>').value;
           
            document.getElementById('<%= txtDecorrenzaPensione.ClientID %>').value = document.getElementById('<%= hDecorrenzaPensione.ClientID %>').value;
        }
    }

    function validatePageTestata() {
        var flag = true;
        flag = Page_ClientValidate('DialSegnalazione');
        return flag;
    }

    function setSelectedIndex(ddl, v) {
        for (var i = 0; i < ddl.options.length; i++) {
            if (ddl.options[i].value == v || ddl.options[i].text == v) {
                ddl.options[i].selected = true;
                return;
            }
        }
    }

    function checkNumeroDomus(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "a" || selected === "b" || selected === "c") {
            var nDomus = document.getElementById('<%= txtNumeroDomus.ClientID %>');
            if (nDomus.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function checkCodiceFiscaleTitolare(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "a" || selected === "b" || selected === "c") {
            var codiceFiscale = document.getElementById('<%= txtCodiceFiscale.ClientID %>');
            if (codiceFiscale.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function checkCategoria(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "a" || selected === "b" || selected === "c") {
            var categoria = document.getElementById('<%= txtCategoria.ClientID %>');
            if (categoria.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function checkSede(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "a" || selected === "b" || selected === "c") {
            var sede = document.getElementById('<%= txtSede.ClientID %>');
            if (sede.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function checkCertificato(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "a" || selected === "b" || selected === "c") {
            var certificato = document.getElementById('<%= txtCertificato.ClientID %>');
            if (certificato.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function checkCodiceErrore(source, args) {
        var ddl = document.getElementById('<%= ddlTipologia.ClientID %>');
        var selected = ddl.options[ddl.selectedIndex].value;
        args.IsValid = true;
        <%--if (selected === "c") {
            var codiceErrore = document.getElementById('<%= txtCodiceErrore.ClientID %>');
            if (codiceErrore.value === "")
                args.IsValid = false;
        }--%>

        return false;
    }

    function OpenValutazione() {        
        window.open(document.getElementById("<%= hValutazione.ClientID %>").value);       
    }

    function OpenProceduraDPI() {
        window.open(document.getElementById("<%= hUrlDPI.ClientID %>").value);
    }
</script>

<script type="text/javascript">
    function myBlink() {
        $("#versioni .blink").fadeTo(3000, 1).fadeTo(500, 0, myBlink);
    }

    $(document).ready(function() {
        $(".loading").show();
        $("#lblErrore").hide();
        $("#versioni").hide();

        // QUESTI WEBMETHOD SONO DICHIARATI IN Default.aspx.cs
        // Binding degli avvisi
        $.ajax({
            type: "POST",
            url: document.getElementById("<%= hPath.ClientID %>").value + "Default.aspx/LoadVersioni",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(data) {
                // Se non trovo niente, nascondo il div
                if (data.d == null || data.d.length <= 0) {
                    $(".loading").hide();
                    return;
                }

                var testo = $("<div><h6 class='trigger titoloVersione'></h6></div>");
                var nodoTemp = null;
                for (var i = 0; i < data.d.length; i++) {
                    nodoTemp = $(testo).clone(true);
                    $('.titoloVersione', nodoTemp).html(data.d[i].Titolo);
                    $("#versioni").append(nodoTemp);
                }

                $("#versioni").show();
                $(".loading").hide();
            },
            error: function(data) {
                //In caso di errore uscirà l'errorMessage
                $("#lblErrore").show();
                $("#versioni").hide();
                if (data.responseText != null && data.responseText != "" && data.responseText.split("###") != 3)
                    $("#lblErrore").text(data.responseText.split("###")[1]);
                else
                    $("#lblErrore").text("Errore durante il caricamento delle versioni.");

                $(".loading").hide();
            }
        });

        myBlink(); // Faccio partire il blinking.
    });
</script>

<script type="text/javascript">
    $(document).ready(function () {
        var menu = $(".nav-menu");
        var menuList = $(".nav-menu__content");
        var menuButton = $(".nav-menu__action-button.nav-menu__action-button--arrow");

        menuButton.mousedown(function (e) {
            e.stopPropagation();
            menu.toggleClass("nav-menu--open");
        });

        menuList.mousedown(function (e) {
            e.stopPropagation();
        });

        $(this).mousedown(function (e) {
            if (
                menu.hasClass("nav-menu--open")
                && !menuButton.is(e.target) && menuButton.has(e.target).length === 0
                && !menuList.is(e.target) && menuList.has(e.target).length === 0
            ) {
                menu.removeClass("nav-menu--open");
            }
        });
    });
    
    function OpenManuale() {
        window.open(document.getElementById("<%= hPath.ClientID %>").value + 'Manuali/Manuale_' + document.getElementById('<%= hTipoApp.ClientID %>').value + '.pdf');
    }

    function OpenFAQ() {
        window.open("../ElaborazionePosizione/StampaFAQ.aspx?TipoApp=" + document.getElementById('<%= hTipoApp.ClientID %>').value);
    }

    function OpenFeedack() {
        window.open(document.getElementById("<%= hValutazione.ClientID %>").value, "_blank");
    }


</script>

<%--<table style="width: 100%; display: none;" border="0" cellpadding="0" cellspacing="0" runat="server" id="tblHeader">
    <tr>
        <td style="width: 350px">
            <asp:Image ID="ImgLogo" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/logofc.png" Width="350px" AlternateText="Sistema per il calcolo delle pensioni" />
        </td>
        <td style="text-align: right;">
            <asp:Panel runat="server" ID="pnlPulsantiIntestazione" Visible="true" style="display: inline;">
                <asp:ImageButton ID="ImgValutazione" style="margin-bottom:5px" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/valutazione.png"                    
                    ToolTip="Aiutaci a migliorare la procedura" OnClientClick="OpenValutazione(); return false;"/>
                <asp:ImageButton ID="ImgManuale" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/book.png"
                    ToolTip="Manuale operatore" OnClientClick="OpenManuale(); return false;" />
                <asp:ImageButton ID="ImgSegnalazione" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/mail.png"
                    ToolTip="Invio segnalazione" OnClientClick="ShowSegnalazione(); return false;" />
                <asp:ImageButton ID="ImgFAQ" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/faq_icon.png"
                    ToolTip="FAQ" OnClientClick="OpenFAQ(); return false;" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlProceduraDPI" Visible="false" style="display: inline;">
                <asp:ImageButton ID="ImgDPI" runat="server" ImageUrl="../App_Themes/BlueINPS1/Images/DPI.png"
                    ToolTip="Dettaglio domanda telematica" OnClientClick="OpenProceduraDPI(); return false;" />
            </asp:Panel>
        </td>
        <td style="text-align: right; width: 10px">
        </td>
        <td style="width: 220px; text-align: left">
            <div>
                <center>
                    <label id="lblErrore" style="display: none;">
                    </label>
                </center>
                <div id="versioni" style="width: 99%; display: block;">
                </div>
                <center>
                    <asp:Image ID="LoadingVersioni" runat="server" CssClass="loading" ImageUrl="~/App_Themes/BlueINPS1/Images/ajax-loader.gif" />
                </center>
            </div>
        </td>
    </tr>
</table>--%>

<header id="containerHeader" class="header">
    <div class="container">
        <div class="row">
            <div class="col-12 header-wrapper">
                <section class="header__title">
                    <img src="../App_Themes/<%= Page.Theme %>/Images/INPS.png" alt="logo" class="header__title-logo"/>
                    <p class="header__title-text">
                        Liquidazione Pensioni (Nuova IVS)
                    </p>
                </section>
                <nav class="header_menu">
                    <asp:Panel CssClass="header-links-cointainer" runat="server" ID="pnlPulsantiIntestazione" Visible="true" style="display: flex">
                    <%--
                    <button onclick="OpenValutazione()" class="header_menu-button" type="button" ">
                        <span class="header_menu-button-message">
                            Fornisci un feedback
                        </span>
                        <img src="../App_Themes/<%= Page.Theme %>/Images/message.png" alt="feedback" class="header_menu-button-icon" ID="ImgValutazione"/>
                    </button>

                    <button onclick="OpenFAQ()" class="header_menu-button" type="button" ">
                        <span class="header_menu-button-message">
                            FAQ
                        </span>
                        <img src="../App_Themes/<%= Page.Theme %>/Images/faq.png" alt="faq" class="header_menu-button-icon" ID="ImgFAQ"/>
                    </button>
                    --%>

                    <div class="nav-menu">
                        <div class="nav-menu__action">
                            <button type="button" class="nav-menu__action-button nav-menu__action-button--main nav-menu__action-button--arrow">Supporto</button>
                        </div>
                        <div class="nav-menu__content">
                            <button type="button" class="nav-menu__action-button" onclick="OpenManuale()">Manuale utente</button>
                            <button type="button" class="nav-menu__action-button" onclick="OpenFAQ()">FAQ</button>
                            <button type="button" class="nav-menu__action-button" onclick="OpenFeedack()">Lasciaci la tua opinione</button>
                        </div>
                    </div>
                        
                    <div class="nav-menu">
                        <div class="nav-menu__action">
                            <button onclick="ShowSegnalazione()" class="nav-menu__action-button nav-menu__action-button--main nav-menu__action-button--support nav-menu__action-button--outline" type="button" >
                                Segnalaci un problema
                            </button>
                        </div>
                    </div>

                    <hr class="vertical-separator ml-16" />
                    </asp:Panel>

                    <div class="user-container">
                        <div class="user-logged">
                            <asp:Label runat="server" ID="lblUtente" Font-Bold="true"></asp:Label> (<asp:Label runat="server" ID="lblMatricola" Font-Bold="true"></asp:Label>)
                        </div>
                        <div class="user-badge">
                            <asp:Label runat="server" ID="lblUserInitial" Font-Bold="true"></asp:Label>
                        </div>
                    </div>
                   
                </nav>
            </div>
        </div>
    </div>
</header>





<asp:Button ID="btnInviaSegnalazione" CausesValidation="true" ValidationGroup="DialSegnalazione"
    Style="display: none" runat="server" OnClick="btnInviaSegnalazione_Click" OnClientClick="if(validatePageTestata()){aspnetForm.target ='_self'; BlockUI();}"
    Text="" />
<div id="dialogSegnalazione" title="Invio Segnalazione" style="border-style: none;
    border-color: White; display: none; vertical-align: top">
    <asp:ValidationSummary runat="server" ID="ValidationSegnalazione" ValidationGroup="DialSegnalazione"
        Font-Size="Small" CssClass="errorBox" />
    <table  class="tabellaFormattazione grid grid-size-15-auto gap-8">
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Destinatario:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:DropDownList ID="ddlDestinatario" runat="server" Width="440px" CssClass="tb8 txtUppercase"
                    Enabled="false">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFdestinatario" ControlToValidate="ddlDestinatario"
                    Enabled="false" ErrorMessage="Destinatario obbligatorio" ValidationGroup="DialSegnalazione"
                    Text="*" CssClass="field-is-required" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>Tipologia di segnalazione:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:DropDownList ID="ddlTipologia" runat="server" Width="440px" CssClass="tb8 txtUppercase"
                    onchange="EnableValidationDialog()">
                    <asp:ListItem Text="" Value="" />
                    <asp:ListItem Text="ERRORI IN PRIMA LIQUIDAZIONE" Value="a" />
                    <asp:ListItem Text="ERRORI IN PRIMA LIQUIDAZIONE CUMUL" Value="b" />
                    <asp:ListItem Text="ERRORI IN RICOSTITUZIONE" Value="c" />
                    <asp:ListItem Text="ERRORI IN TRASFORMAZIONE" Value="d" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFTipologia" ControlToValidate="ddlTipologia"
                    Enabled="false" ErrorMessage="Tipologia di segnalazione obbligatoria" ValidationGroup="DialSegnalazione"
                    Text="*" CssClass="field-is-required" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Messaggio:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox CssClass="tb8" runat="server" ID="txtMessaggio" TextMode="MultiLine"
                    Height="150px" Width="435px" MaxLength="2000"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RFmessaggio" ControlToValidate="txtMessaggio"
                    Enabled="false" ErrorMessage="Messaggio obbligatorio" ValidationGroup="DialSegnalazione"
                    Text="*" CssClass="field-is-required" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Telefono:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox CssClass="tb8" runat="server" ID="txtTelefono" Width="435px" MaxLength="15"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REtelefono" ControlToValidate="txtTelefono"
                    Enabled="false" ErrorMessage="Telefono non corretto" ValidationGroup="DialSegnalazione"
                    ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTelefono" ControlToValidate="txtTelefono"
                    Enabled="false" ErrorMessage="Telefono obbligatorio" ValidationGroup="DialSegnalazione"
                    Text="*" CssClass="field-is-required" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Numero domus:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox CssClass="tb8" runat="server" ID="txtNumeroDomus" Width="435px" MaxLength="13"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtNumeroDomus" ControlToValidate="txtNumeroDomus"
                    Enabled="false" ErrorMessage="Numero domus non corretto" ValidationGroup="DialSegnalazione"
                    ValidationExpression="^[0-9]{13}$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                <asp:CustomValidator runat="server" ID="CVtxtNumeroDomus" ControlToValidate="txtNumeroDomus"
                    Display="Dynamic" Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Numero domus obbligatorio"
                    ValidationGroup="DialSegnalazione" ClientValidationFunction="checkNumeroDomus"
                    ValidateEmptyText="true" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Codice fiscale titolare:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceFiscale" Width="435px"
                    MaxLength="16"></asp:TextBox>
                <asp:CustomValidator ControlToValidate="txtCodiceFiscale" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="DialSegnalazione"
                    ID="txtCodiceFiscale_CV" ClientValidationFunction="validateCodiceFiscale"
                    ErrorMessage="Codice fiscale non valido" />
                <asp:CustomValidator runat="server" ID="CVtxtCodiceFiscale" ControlToValidate="txtCodiceFiscale"
                    Display="Dynamic" Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Codice fiscale titolare obbligatorio"
                    ValidationGroup="DialSegnalazione" ClientValidationFunction="checkCodiceFiscaleTitolare"
                    ValidateEmptyText="true" />
            </td>
        </tr>
        <tr>
        <td align="right">
                            <label>
                                Categoria:</label>
                        </td>
            <td align="left" class="full-grid">
            
                
                       
                        
                        
                            <asp:TextBox CssClass="tb8" style="float:left;margin-top:3px" runat="server" ID="txtCategoria" Width="40px" MaxLength="3"></asp:TextBox>
                </td>
            </tr>
            <tr>
            <td class="shift-full-grid" colspan="2">
                        <table style="padding-left: 0px; padding-right: 0px; float:right ; padding-top:0px; width:420px; margin-right: 14px;" class="tabellaFormattazione grid grid-size-15-auto gap-8 float-none">
                         <tr>
                         <td  align="left" style="width:7%" class="shift-full-grid">
                            <asp:RegularExpressionValidator runat="server" ID="REVtxtCategoria" ControlToValidate="txtCategoria"
                                Enabled="false" ErrorMessage="Categoria non corretta" ValidationGroup="DialSegnalazione"
                                ValidationExpression="^[0-9]{3}$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                            <asp:CustomValidator runat="server" ID="CVtxtCategoria" ControlToValidate="txtCategoria"
                                Display="Dynamic" Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Categoria obbligatoria"
                                ValidationGroup="DialSegnalazione" ClientValidationFunction="checkCategoria"
                                ValidateEmptyText="true" />
                        </td>
                        <td  align="right" style="width:10%">
                            <label>
                                Sede:</label>
                        </td>
                        <td  align="left" style="width:14%">
                            <asp:TextBox CssClass="tb8" runat="server" ID="txtSede" Width="60px" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="REVtxtSede" ControlToValidate="txtSede"
                                Enabled="false" ErrorMessage="Sede non corretta" ValidationGroup="DialSegnalazione"
                                ValidationExpression="^[0-9]{4}$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                            <asp:CustomValidator runat="server" ID="CVtxtSede" ControlToValidate="txtSede" Display="Dynamic"
                                Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Sede obbligatoria" ValidationGroup="DialSegnalazione"
                                ClientValidationFunction="checkSede" ValidateEmptyText="true" />
                        </td>
                        <td  align="right" style="width:14%">
                            <label>
                                Certificato:</label>
                        </td>
                        <td  align="left" style="width:20%">
                            <asp:TextBox CssClass="tb8" runat="server" ID="txtCertificato" Width="90px" MaxLength="8"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="REVtxtCertificato" ControlToValidate="txtCertificato"
                                Enabled="false" ErrorMessage="Certificato non corretto" ValidationGroup="DialSegnalazione"
                                ValidationExpression="^[0-9]{8}$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                            <asp:CustomValidator runat="server" ID="CVtxtCertificato" ControlToValidate="txtCertificato"
                                Display="Dynamic" Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Certificato obbligatoria"
                                ValidationGroup="DialSegnalazione" ClientValidationFunction="checkCertificato"
                                ValidateEmptyText="true" />
                        </td>
                         <td  align="left" style="width:7%" class="blue-none shift-full-grid">
                             </td>
                    </tr>
                </table>
            </td>
        </tr>
         <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Decorrenza Pensione:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaPensione" Width="120px" 
                        CssClass="txtUppercase tb8 date-picker-base" MaxLength="10" TabIndex="6"></asp:TextBox>
                      <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenzaPensione" ControlToValidate="txtDecorrenzaPensione"
                        Enabled="false" ErrorMessage="Decorrenza Pensione: campo obbligatorio" ValidationGroup="DialSegnalazione"
                        Text="*" CssClass="field-is-required" Display="Dynamic"/>
                                                       
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPensione" 
                        ErrorMessage="Decorrenza Pensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="DialSegnalazione"
                        ID="CVtxtDecorrenzaPensione" ClientValidationFunction="checkCorrettezzaData" Enabled="false" Display="Dynamic"/>  
            </td>
        </tr>
        <tr>
            <td style="width: 20%;" align="right">
                <label>
                    Codice errore:</label>
            </td>
            <td style="width: 80%;" align="left" class="full-grid">
                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceErrore" Width="435px"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtCodiceErrore" ControlToValidate="txtCodiceErrore"
                    Enabled="false" ErrorMessage="Codice errore non corretto" ValidationGroup="DialSegnalazione"
                    ValidationExpression="^[0-9a-zA-Z]+$" Text="*" CssClass="field-is-required" Display="Dynamic"/>
                <asp:CustomValidator runat="server" ID="CVtxtCodiceErrore" ControlToValidate="txtCodiceErrore"
                    Display="Dynamic" Text="*" CssClass="field-is-required" Enabled="false" ErrorMessage="Codice errore obbligatorio"
                    ValidationGroup="DialSegnalazione" ClientValidationFunction="checkCodiceErrore"
                    ValidateEmptyText="true" />
            </td>
        </tr>
       
    </table>
    <table style="width: 98%; margin-top: 10px; margin-bottom: 20px; border: 0px dotted #0099CC" class="tabellaFormattazione grid grid-size-15-auto gap-8 mt-32">
            <tr>
            <td style="width: 90%;" align="left" colspan="2" class="shift-full-grid">
                Per segnalazioni di natura tecnica 
            <a href="javascript:void(0);"
               onclick="ShowRemedy()"
               style="color:#000; text-decoration:none; cursor:pointer; display:inline-flex;align-items:center;gap:6px;font-weight:bold; ">
                <span >
                    CLICCA QUI
                </span>
            </a>
            </td>
         </tr>
        <tr>
            <td align="left" colspan="2" class="shift-full-grid">
                <asp:Label ID="lblEsitoError" runat="server" Text="" ForeColor="Red" />
            </td>
        </tr>
    </table>
</div>
<div id="dialogEsitoSegnalazione" title="Esito Invio Segnalazione" style="border-style: none;
    border-color: White; display: none; vertical-align: top; text-align: center;">
    <asp:Label ID="lblEsitoSegnalazione" runat="server" Text="" />
</div>
<asp:HiddenField runat="server" ID="hDestinatario" />
<asp:HiddenField runat="server" ID="hTipologia" />
<asp:HiddenField runat="server" ID="hMessaggio" />
<asp:HiddenField runat="server" ID="hTelefono" />
<asp:HiddenField runat="server" ID="hNumeroDomus" />
<asp:HiddenField runat="server" ID="hCodiceFiscale" />
<asp:HiddenField runat="server" ID="hCategoria" />
<asp:HiddenField runat="server" ID="hSede" />
<asp:HiddenField runat="server" ID="hCertificato" />
<asp:HiddenField runat="server" ID="hCodiceErrore" />
<asp:HiddenField runat="server" ID="hTipoApp" />
<asp:HiddenField runat="server" ID="hEsito" />
<asp:HiddenField runat="server" ID="hPath" />
<asp:HiddenField runat="server" ID="hUrlDPI" />
<asp:HiddenField runat="server" ID="hDecorrenzaPensione" />
<asp:HiddenField runat="server" ID="hValutazione" />
<asp:HiddenField runat="server" ID="hRemedy" />
<asp:HiddenField runat="server" ID="hCurrentTheme" />
