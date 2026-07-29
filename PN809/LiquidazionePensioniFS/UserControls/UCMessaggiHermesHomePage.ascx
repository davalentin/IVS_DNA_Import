<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMessaggiHermesHomePage.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCMessaggiHermesHomePage" %>

<style type="text/css" media="screen">
    #messaggiHermes h3.trigger
    {
        width: 90%;
        margin: 0px 0px 0px 10px;
    }
    #messaggiHermes div.collapsibleContainer
    {
        width: 90%;
        border-width: 0px;
        margin-left: 50px;
    }
    
    #messaggiHermes .fakeLink
    {
        color: Black;
        cursor: pointer;
    }
    
    #messaggiHermes .fakeLink:hover
    {
        color: #ccc;
    }
    
    #messaggiHermes > ul > li
    {
        margin: 5px 0px;
    }
    
    #messaggiHermes .messaggioHermesStatico
    {
        font-weight: bold;
        padding: 0px 0px 0px 20px;
    }
    
    #messaggiHermes .blink 
    {
        color: Red;
    }
    
    .titoloMessaggioHermes p
    {
        display: inline-block;
    }

    .btnPagingMessaggiHermes
    {
        font-family: Arial;
        color: #ffffff;
        font-size: 16px;
        background: #4F81BD;
        padding: 5px 10px 5px 10px;
        border: solid #1f628d 1px;
        text-decoration: none;
    }
    
    .btnPagingMessaggiHermes:hover
    {
        background: #ffffff;
        background-image: -webkit-linear-gradient(top, #ffffff, #3498db);
        background-image: -moz-linear-gradient(top, #ffffff, #3498db);
        background-image: -ms-linear-gradient(top, #ffffff, #3498db);
        background-image: -o-linear-gradient(top, #ffffff, #3498db);
        background-image: linear-gradient(to bottom, #ffffff, #3498db);
        color: #00248e;
        text-decoration: none;
    }
    
    input.btnPagingMessaggiHermes.active
    {
        background: #ffffff;
        background-image: -webkit-linear-gradient(top, #ffffff, #3498db);
        background-image: -moz-linear-gradient(top, #ffffff, #3498db);
        background-image: -ms-linear-gradient(top, #ffffff, #3498db);
        background-image: -o-linear-gradient(top, #ffffff, #3498db);
        background-image: linear-gradient(to bottom, #ffffff, #3498db);
        color: #00248e;
        text-decoration: none;
    }
</style>

<script type="text/javascript">
    function myBlink() {
        $("#messaggiHermes .blink").fadeTo(3000, 1).fadeTo(500, 0, myBlink);
    }

    $(document).ready(function () {
        LoadMessaggiHermes();
    });

    function LoadMessaggiHermes() {
        $("#LoadingMessaggiHermes").show();
        $("#lblErrore").hide();
        $("#messaggiHermes").hide();
        $("#pageMessaggiHermes").hide();

        // QUESTI WEBMETHOD SONO DICHIARATI IN Default.aspx.cs
        // Binding dei messaggi Hermes
        $.ajax({
            type: "POST",
            url: "Default.aspx/LoadMessaggiHermes",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                // Se non trovo niente, nascondo il div
                if (data.d == null || data.d.length <= 0) {
                    $("#LoadingMessaggiHermes").hide();
                    return;
                }

                $(".btnPagingMessaggiHermes").remove();
                $(".btnPagingMessaggiHermesNavPage").remove();

                var pageSize = 8;
                var pageNumber = $("#<%= pageNumberMessaggiHermes.ClientID %>").val();
                var size = (pageNumber - 1) * pageSize + pageSize;
                if (size > data.d.length)
                    size = data.d.length;

                if (pageTheme == "iFrame") {
                    $(".homepage-hermes-message").remove();

                    var testo = $("<section class='homepage-hermes-message'><div class='homepage-hermes-message__caption'><img src='./App_Themes/" + pageTheme + "/Images/article.svg' class='homepage-hermes-message__icon'/><p class='homepage-hermes-message__info'>Messaggio Hermes</p><p class='homepage-hermes-message__date'></p></div><a href='' target='_blank' class='homepage-hermes-message__title link-button tertiary ghost ghost--small'></a><p class='homepage-hermes-message__text richtext'></p></section>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        const [date, title] = data.d[i].Titolo.split(" - ");
                        const [text] = data.d[i].Testo.split(/(\\u003c|<)a/i);
                        const url = data.d[i].Url;
                        $('.homepage-hermes-message__date', nodoTemp).html(date);
                        $('.homepage-hermes-message__title', nodoTemp).html(title);
                        $('.homepage-hermes-message__title', nodoTemp).attr("href", url);
                        $('.homepage-hermes-message__text', nodoTemp).html(text);
                        $("#messaggiHermes").append(nodoTemp);
                    }
                } else {
                    $(".titoloMessaggioHermes").parent().remove();

                    var testo = $("<div class='accordion'><h3 class='trigger fakeLink titoloMessaggioHermes'></h3><div class='collapsibleContainer PnlContenitoreDatiInterno testoMessaggioHermes richtext'></div></div>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        $('.titoloMessaggioHermes', nodoTemp).html(data.d[i].Titolo);
                        $('.testoMessaggioHermes', nodoTemp).html(data.d[i].Testo);
                        $("#messaggiHermes").append(nodoTemp);
                    }
                }

                var nPage = Math.ceil(data.d.length / pageSize);
                if (nPage > 1) {
                    var elenco = createPagination(nPage, pageNumber, pageSize, data.d.length, "btnPagingMessaggiHermes", "SetPageMessaggiHermes");

                    $("#pageMessaggiHermes").append(elenco);

                    $(".btnPagingMessaggiHermes").each(function () {
                        if ($(this)[0].value == pageNumber)
                            $($(this)[0]).addClass("active");
                    });
                }

                if (pageTheme != "iFrame") {
                    $(".testoMessaggioHermes").hide();
                    $("#messaggiHermes .fakeLink").click(function () {
                        $(this).toggleClass("active").next().slideToggle("fast");
                    });
                }

                if ($("a[href='#messaggiHermes']").parent().hasClass("active"))
                    $("#messaggiHermes").show();

                $("#LoadingMessaggiHermes").hide();

                ResizerMessaggiHermes(0);
            },
            error: function (data) {
                //In caso di errore uscirà l'errorMessage
                $("#lblErrore").show();
                $("#messaggiHermes").hide();
                $("#pageMessaggiHermes").hide();
                if (data.responseText != null && data.responseText != "" && data.responseText.split("###") != 3)
                    $("#lblErrore").text(data.responseText.split("###")[1]);
                else
                    $("#lblErrore").text("Errore durante il caricamento dei messaggi Hermes.");

                $("#LoadingMessaggiHermes").hide();
            }
        });

        myBlink(); // Faccio partire il blinking.
        $("#pageMessaggiHermes").show();
    }

    function SetPageMessaggiHermes(page) {
        $("#<%= pageNumberMessaggiHermes.ClientID %>").val(page);
        LoadMessaggiHermes();
        LoadSelectedTab(false);
    }

    function ResizerMessaggiHermes(fontSize) {
        var flagFontModified = false;
        var listAvvisiWithFont = $(".titoloMessaggioHermes font");
        if (listAvvisiWithFont.length > 0) {
            for (i = 0; i < listAvvisiWithFont.length; i++) {
                var fontSizeOfFont = listAvvisiWithFont[i].size;
                while (listAvvisiWithFont[i].scrollHeight > 32 && fontSizeOfFont > 1) {
                    --fontSizeOfFont;
                    if (fontSizeOfFont > 1) {
                        listAvvisiWithFont[i].size = fontSizeOfFont;
                    }
                }
            }
        }
        if (fontSize == 0) {
            fontSize = $(".titoloMessaggioHermes").css('fontSize');
            if (fontSize == "medium")
                fontSize = 16;
        }
        var listAvvisi = $(".titoloMessaggioHermes");
        if (fontSize !== undefined) {
            if (typeof fontSize == 'string')
                fontSize = fontSize.replace("px", "");
            --fontSize;
        }
        for (i = 0; i < listAvvisi.length; i++) {
            if (listAvvisi[i].scrollHeight > 32) {
                if (fontSize > 8) {
                    $(".titoloMessaggioHermes").css('fontSize', fontSize);
                    flagFontModified = true;
                }
            }
        }

        if (fontSize !== undefined)
            if (!flagFontModified)
                ++fontSize;

        if (fontSize === undefined)
            fontSize = 0;
        setTimeout(function () {
            ResizerMessaggiHermes(fontSize);
        }, 1);
    }
</script>

<div style="width: 98%;">
    <center><label id="lblErrore" style="display: none;"></label></center>
    <div id="messaggiHermes" style="width: 100%; display: none; overflow-y: auto; overflow-x: hidden;
        height: 80%">
    </div>
    <div id="pageMessaggiHermes" style="width: 100%; display: none; overflow-y: auto; padding: 5px; text-align: right;
        overflow-x: hidden; height: 80%">
    </div>
    <center><asp:Image ID="LoadingMessaggiHermes" runat="server" CssClass="loading" ImageUrl="../App_Themes/<%= Page.Theme %>/Images/ajax-loader.gif" /></center>
</div>

<asp:HiddenField runat="server" id="pageNumberMessaggiHermes" value="1" />