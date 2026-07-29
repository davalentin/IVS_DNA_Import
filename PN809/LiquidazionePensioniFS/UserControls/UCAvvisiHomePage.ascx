<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAvvisiHomePage.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCAvvisiHomePage" %>
<style type="text/css" media="screen">
    #avvisi h3.trigger
    {
        width: 90%;
        margin: 0px 0px 0px 10px;
    }
    #avvisi div.collapsibleContainer
    {
        width: 90%;
        border-width: 0px;
        margin-left: 50px;
    }
    
    #avvisi .fakeLink
    {
        color: Black;
        cursor: pointer;
    }
    
    #avvisi .fakeLink:hover
    {
        color: #ccc;
    }
    
    #avvisi > ul > li
    {
        margin: 5px 0px;
    }
    
    #avvisi .avvisoStatico
    {
        font-weight: bold;
        padding: 0px 0px 0px 20px;
    }
    
    #avvisi .blink
    {
        color: Red;
    }
    
    .titoloAvviso p
    {
        display: inline-block;
    }
    
    .btnPagingAvvisi
    {
        font-family: Arial;
        color: #ffffff;
        font-size: 16px;
        background: #4F81BD;
        padding: 5px 10px 5px 10px;
        border: solid #1f628d 1px;
        text-decoration: none;
    }
    
    .btnPagingAvvisi:hover
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
    
    input.btnPagingAvvisi.active
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
    var pageTheme = "<%= Page.Theme %>";

    function myBlink() {
        $("#avvisi .blink").fadeTo(3000, 1).fadeTo(500, 0, myBlink);
    }

    $(document).ready(function () {
        LoadAvvisi();
    });

    function LoadAvvisi() {
        $("#LoadingAvvisi").show();
        $("#lblErrore").hide();
        $("#avvisi").hide();
        $("#pageAvvisi").hide();

        // QUESTI WEBMETHOD SONO DICHIARATI IN Default.aspx.cs
        // Binding degli avvisi
        $.ajax({
            type: "POST",
            url: "Default.aspx/LoadAvvisi",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                // Se non trovo niente, nascondo il div
                if (data.d == null || data.d.length <= 0) {
                    $("#LoadingAvvisi").hide();
                    return;
                }

                $(".btnPagingAvvisi").remove();
                $(".btnPagingAvvisiNavPage").remove();

                var pageSize = 8;
                var pageNumber = $("#<%= pageNumberAvvisi.ClientID %>").val();
                var size = (pageNumber - 1) * pageSize + pageSize;
                if (size > data.d.length)
                    size = data.d.length;

                if (pageTheme == "iFrame") {
                    $(".homepage-avvisi-message").remove();

                    var testo = $("<section class='homepage-avvisi-message'><div class='homepage-avvisi-message__caption'><img src='./App_Themes/" + pageTheme + "/Images/article.svg' class='homepage-avvisi-message__icon'/><p class='homepage-avvisi-message__info'>Avviso</p><p class='homepage-avvisi-message__date'></p></div><h4 class='homepage-avvisi-message__title'></h4><p class='homepage-avvisi-message__text'></p></section>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        const [date, title] = data.d[i].Titolo.split(" - ")
                        $('.homepage-avvisi-message__date', nodoTemp).html(date);
                        $('.homepage-avvisi-message__title', nodoTemp).html(title);
                        $('.homepage-avvisi-message__text', nodoTemp).html(data.d[i].Testo);
                        $("#avvisi").append(nodoTemp);
                    }

                } else {
                    $(".titoloAvviso").parent().remove();

                    //var testo = $("<li class='customBullet'><label class='titoloAvviso fakeLink'></label><div class='testoAvviso'></div></li>");
                    var testo = $("<div class='accordion'><h3 class='trigger fakeLink titoloAvviso'></h3><div class='collapsibleContainer PnlContenitoreDatiInterno testoAvviso'></div></div>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        $('.titoloAvviso', nodoTemp).html(data.d[i].Titolo);
                        $('.testoAvviso', nodoTemp).html(data.d[i].Testo);
                        $("#avvisi").append(nodoTemp);
                    }
                }

                var nPage = Math.ceil(data.d.length / pageSize);
                if (nPage > 1) {
                    var elenco = createPagination(nPage, pageNumber, pageSize, data.d.length, "btnPagingAvvisi", "SetPageAvvisi");

                    $("#pageAvvisi").append(elenco);

                    $(".btnPagingAvvisi").each(function () {
                        if ($(this)[0].value == pageNumber)
                            $($(this)[0]).addClass("active");
                    });
                }

                if (pageTheme != "iFrame") {
                    $(".testoAvviso").hide();
                    $("#avvisi .fakeLink").click(function () {
                        $(this).toggleClass("active").next().slideToggle("fast");
                    });
                }

                if ($("a[href='#avvisi']").parent().hasClass("active")) {
                    if ("<%=Page.Theme%>" == "iFrame") {
                        $("#avvisi").css("display", "flex")
                    }
                    else {
                        $("#avvisi").show();
                    }
                }

                $("#LoadingAvvisi").hide();

                ResizerAvvisi(0);
            },
            error: function (data) {
                //In caso di errore uscirà l'errorMessage
                $("#lblErrore").show();
                $("#avvisi").hide();
                $("#pageAvvisi").hide();
                if (data.responseText != null && data.responseText != "" && data.responseText.split("###") != 3)
                    $("#lblErrore").text(data.responseText.split("###")[1]);
                else
                    $("#lblErrore").text("Errore durante il caricamento degli avvisi.");

                $("#LoadingAvvisi").hide();
            }
        });

        myBlink(); // Faccio partire il blinking.
        $("#pageAvvisi").show();
    }

    function SetPageAvvisi(page) {
        $("#<%= pageNumberAvvisi.ClientID %>").val(page);
        LoadAvvisi();
        LoadSelectedTab(false);
    }

    function ResizerAvvisi(fontSize) {
        var flagFontModified = false;
        var listAvvisiWithFont = $(".titoloAvviso font");
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
            fontSize = $(".titoloAvviso").css('fontSize');
            if (fontSize == "medium")
                fontSize = 16;
        }
        var listAvvisi = $(".titoloAvviso");
        if (fontSize !== undefined) {
            if (typeof fontSize == 'string')
                fontSize = fontSize.replace("px", "");
            --fontSize;
        }
        for (i = 0; i < listAvvisi.length; i++) {
            if (listAvvisi[i].scrollHeight > 32) {
                if (fontSize > 8) {
                    $(".titoloAvviso").css('fontSize', fontSize);
                    flagFontModified = true;
                }
            }
        }

        if (fontSize !== undefined)
            if (!flagFontModified)
                ++fontSize;

        if (fontSize === undefined)
            fontSize = 0;
        var timeoutAvvisi = setTimeout(function () {
            ResizerAvvisi(fontSize);
        }, 1);
    }

</script>
<div style="width: 98%;">
    <center>
        <label id="lblErrore" style="display: none;">
        </label>
    </center>
    <div id="avvisi" style="width: 100%; display: none; overflow-y: auto; overflow-x: hidden;
        height: 80%">
    </div>
    <div id="pageAvvisi" style="width: 100%; display: none; overflow-y: auto; padding: 5px;
        text-align: right; overflow-x: hidden; height: 80%">
    </div>
    <center>
        <asp:Image ID="LoadingAvvisi" runat="server" CssClass="loading" ImageUrl="../App_Themes/<%= Page.Theme %>/Images/ajax-loader.gif" /></center>
</div>
<asp:HiddenField runat="server" ID="pageNumberAvvisi" Value="1" />
