<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAggiornamentiHomePage.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCAggiornamentiHomePage" %>
<style type="text/css" media="screen">
    #aggiornamenti h3.trigger
    {
        width: 90%;
        margin: 0px 0px 0px 10px;
    }
    #aggiornamenti div.collapsibleContainer
    {
        width: 90%;
        border-width: 0px;
        margin-left: 50px;
    }
    
    #aggiornamenti .fakeLink
    {
        color: Black;
        cursor: pointer;
    }
    
    #aggiornamenti .fakeLink:hover
    {
        color: #ccc;
    }
    
    #aggiornamenti > ul > li
    {
        margin: 5px 0px;
    }
    
    #aggiornamenti .aggiornamentoStatico
    {
        font-weight: bold;
        padding: 0px 0px 0px 20px;
    }
    
    #aggiornamenti .blink
    {
        color: Red;
    }
    
    .btnPagingAggiornamenti
    {
        font-family: Arial;
        color: #ffffff;
        font-size: 16px;
        background: #4F81BD;
        padding: 5px 10px 5px 10px;
        border: solid #1f628d 1px;
        text-decoration: none;
    }
    
    .btnPagingAggiornamenti:hover
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
    
    input.btnPagingAggiornamenti.active
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
        $("#aggiornamenti .blink").fadeTo(3000, 1).fadeTo(500, 0, myBlink);
    }

    $(document).ready(function () {
        LoadAggiornamenti();
    });

    function LoadAggiornamenti() {
        $("#LoadingAggiornamenti").show();
        $("#lblErrore").hide();
        $("#aggiornamenti").hide();
        $("#pageAggiornamenti").hide();

        // QUESTI WEBMETHOD SONO DICHIARATI IN Default.aspx.cs
        // Binding degli aggiornamenti
        $.ajax({
            type: "POST",
            url: "Default.aspx/LoadAggiornamenti",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                // Se non trovo niente, nascondo il div
                if (data.d == null || data.d.length <= 0) {
                    $("#LoadingAggiornamenti").hide();
                    return;
                }

                $(".btnPagingAggiornamenti").remove();
                $(".btnPagingAggiornamentiNavPage").remove();

                var pageSize = 8;
                var pageNumber = $("#<%= pageNumberAggiornamenti.ClientID %>").val();
                var size = (pageNumber - 1) * pageSize + pageSize;
                if (size > data.d.length)
                    size = data.d.length;

                if (pageTheme == "iFrame") {
                    $(".homepage-aggiornamenti-message").remove();

                    var testo = $("<section class='homepage-aggiornamenti-message'><div class='homepage-aggiornamenti-message__caption'><img src='./App_Themes/" + pageTheme + "/Images/article.svg' class='homepage-aggiornamenti-message__icon'/><p class='homepage-aggiornamenti-message__info'>LIQPENS</p><p class='homepage-aggiornamenti-message__date'></p></div><h4 class='homepage-aggiornamenti-message__title'></h4><p class='homepage-aggiornamenti-message__text'></p></section>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        const [date, title] = data.d[i].Titolo.split(" - ");
                        $('.homepage-aggiornamenti-message__date', nodoTemp).html(date);
                        $('.homepage-aggiornamenti-message__title', nodoTemp).html(title);
                        $('.homepage-aggiornamenti-message__text', nodoTemp).html(data.d[i].Testo);
                        $("#aggiornamenti").append(nodoTemp);
                    }
                } else {

                    $(".titoloAggiornamento").parent().remove();

                    //var testo = $("<li class='customBullet'><label class='titoloAvviso fakeLink'></label><div class='testoAvviso'></div></li>");
                    var testo = $("<div class='accordion'><h3 class='trigger fakeLink titoloAggiornamento'></h3><div class='collapsibleContainer PnlContenitoreDatiInterno testoAggiornamento'></div></div>");
                    var nodoTemp = null;

                    for (var i = (pageNumber - 1) * pageSize; i < size; i++) {
                        nodoTemp = $(testo).clone(true);
                        $('.titoloAggiornamento', nodoTemp).html(data.d[i].Titolo);
                        $('.testoAggiornamento', nodoTemp).html(data.d[i].Testo);
                        $("#aggiornamenti").append(nodoTemp);
                    }
                }

                var nPage = Math.ceil(data.d.length / pageSize);
                if (nPage > 1) {
                    var elenco = createPagination(nPage, pageNumber, pageSize, data.d.length, "btnPagingAggiornamenti", "SetPageAggiornamenti");

                    $("#pageAggiornamenti").append(elenco);

                    $(".btnPagingAggiornamenti").each(function () {
                        if ($(this)[0].value == pageNumber)
                            $($(this)[0]).addClass("active");
                    });
                }

                if (pageTheme != "iFrame") {
                    $(".testoAggiornamento").hide();
                    $("#aggiornamenti .fakeLink").click(function () {
                        $(this).toggleClass("active").next().slideToggle("fast");
                    });
                }

                if ($("a[href='#aggiornamenti']").parent().hasClass("active")) {
                    $("#aggiornamenti").show();
                    $("#aggiornamenti").css("display", "<%# Page.Theme == "iFrame" ? "flex" : "block" %>");
                }

                $("#LoadingAggiornamenti").hide();
            },
            error: function (data) {
                //In caso di errore uscirà l'errorMessage
                $("#lblErrore").show();
                $("#aggiornamenti").hide();
                $("#pageAggiornamenti").hide();
                if (data.responseText != null && data.responseText != "" && data.responseText.split("###") != 3)
                    $("#lblErrore").text(data.responseText.split("###")[1]);
                else
                    $("#lblErrore").text("Errore durante il caricamento degli aggiornamenti.");

                $("#LoadingAggiornamenti").hide();
            }
        });

        myBlink(); // Faccio partire il blinking.
        $("#pageAggiornamenti").show();

        ResizerAggiornamenti(0);
    }

    function SetPageAggiornamenti(page) {
        $("#<%= pageNumberAggiornamenti.ClientID %>").val(page);
        LoadAggiornamenti();
        LoadSelectedTab(false);
    }

    function ResizerAggiornamenti(fontSize) {
        var flagFontModified = false;
        if (fontSize == 0) {
            fontSize = $(".titoloAggiornamento").css('fontSize');
            if (fontSize == "medium")
                fontSize = 16;
        }
        var listAggiornamenti = $(".titoloAggiornamento");
        if (fontSize !== undefined) {
            if (typeof fontSize == 'string')
                fontSize = fontSize.replace("px", "");
            --fontSize;
        }
        for (i = 0; i < listAggiornamenti.length; i++) {
            if (listAggiornamenti[i].scrollHeight > 32) {
                if (fontSize > 8) {
                    $(".titoloAggiornamento").css('fontSize', fontSize);
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
            ResizerAggiornamenti(fontSize);
        }, 1);
    }

</script>
<div style="width: 98%;">
    <center>
        <label id="lblErrore" style="display: none;">
        </label>
    </center>
    <div id="aggiornamenti" style="width: 100%; display: none; overflow-y: auto; overflow-x: hidden;
        height: 80%">
    </div>
    <div id="pageAggiornamenti" style="width: 100%; display: none; overflow-y: auto; padding: 5px; text-align: right;
        overflow-x: hidden; height: 80%">
    </div>
    <center>
        <asp:Image ID="LoadingAggiornamenti" runat="server" CssClass="loading" ImageUrl="../App_Themes/<%= Page.Theme %>/Images/ajax-loader.gif" /></center>
</div>

<asp:HiddenField runat="server" id="pageNumberAggiornamenti" value="1" />