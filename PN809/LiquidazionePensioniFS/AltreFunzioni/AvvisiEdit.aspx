<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="AvvisiEdit.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AvvisiEdit" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css" media="screen">
        .header
        {
            text-align: center;
        }
    </style>
    <link rel="stylesheet" type="text/css" href="../Javascript/cleditor/jquery.cleditor.css" />
    <script type="text/javascript" src="../Javascript/cleditor/jquery.cleditor.js"></script>
    <script type="text/javascript" src="../Javascript/cleditor/jquery.cleditor.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".tooltips").hover(
		    function () { $(this).next().css({ display: "block" }) },
		    function () { $(this).next().css({ display: "none" }); }
	        );
            $(".tooltips").mousemove(function (e) {
                var mousex = e.pageX + 10;
                var mousey = e.pageY + 1;
                $(this).next().context.alt = '';
                $(this).next().css({ top: mousey, left: mousex }).fadeIn(0);
            });
        });
    </script>
    <script type="text/javascript">
        function imgbtnVisibleAvviso_ClientClick() {
            var src = $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src");
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_off.png");
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("title", "Avviso non visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_on.png");
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("title", "Avviso visibile. Clicca per modificarne la visibilità.");
            }
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#textareaEditAvviso").cleditor({
                height: 330,
                controls:     // controls to add to the toolbar
          "bold italic underline strikethrough subscript superscript | font size style | " +
          "color highlight removeformat | bullets numbering | outdent " +
          "indent | alignleft center alignright justify | " +
          "cut copy paste",
                colors:       // colors in the color popup
          "FFF FCC FC9 FF9 FFC 9F9 9FF CFF CCF FCF " +
          "CCC F66 F96 FF6 FF3 6F9 3FF 6FF 99F F9F " +
          "BBB F00 F90 FC6 FF0 3F3 6CC 3CF 66C C6C " +
          "999 C00 F60 FC3 FC0 3C0 0CC 36F 63F C3C " +
          "666 900 C60 C93 990 090 399 33F 60C 939 " +
          "333 600 930 963 660 060 366 009 339 636 " +
          "000 300 630 633 330 030 033 006 309 303",
                fonts:        // font names in the font popup
          "Arial,Arial Black,Comic Sans MS,Courier New,Narrow,Garamond," +
          "Georgia,Impact,Sans Serif,Serif,Tahoma,Trebuchet MS,Verdana",
                sizes:        // sizes in the font size popup
          "1,2,3,4,5,6,7",
                styles:       // styles in the style popup
          [["Paragrafo", "<p>"], ["Titolo 1", "<h1>"], ["Titolo 2", "<h2>"],
           ["Titolo 3", "<h3>"], ["Titolo 4", "<h4>"], ["Titolo 5", "<h5>"],
           ["Titolo 6", "<h6>"]],
                useCSS: true, // use CSS to style HTML when possible (not supported in ie)
                docType:       // Document type contained within the editor
          '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">',
                docCSSFile:    // CSS file used to style the document contained within the editor
          "../App_Themes/BlueINPS1/MainStyle.css",
                bodyStyle:    // style to assign to document body contained within the editor
          "margin:4px; font:10pt Arial,Verdana; cursor:text"
            });

            $("#textareaTitoloAvviso").cleditor({
                height: 70,
                controls:     // controls to add to the toolbar
          "bold italic underline strikethrough subscript superscript | font size style | " +
          "color highlight removeformat | outdent | indent | cut copy paste",
                colors:       // colors in the color popup
          "FFF FCC FC9 FF9 FFC 9F9 9FF CFF CCF FCF " +
          "CCC F66 F96 FF6 FF3 6F9 3FF 6FF 99F F9F " +
          "BBB F00 F90 FC6 FF0 3F3 6CC 3CF 66C C6C " +
          "999 C00 F60 FC3 FC0 3C0 0CC 36F 63F C3C " +
          "666 900 C60 C93 990 090 399 33F 60C 939 " +
          "333 600 930 963 660 060 366 009 339 636 " +
          "000 300 630 633 330 030 033 006 309 303",
                fonts:        // font names in the font popup
          "Arial,Arial Black,Comic Sans MS,Courier New,Narrow,Garamond," +
          "Georgia,Impact,Sans Serif,Serif,Tahoma,Trebuchet MS,Verdana",
                sizes:        // sizes in the font size popup
          "1,2,3,4,5,6,7",
                styles:       // styles in the style popup
          [["Paragrafo", "<p>"], ["Titolo 1", "<h1>"], ["Titolo 2", "<h2>"],
           ["Titolo 3", "<h3>"], ["Titolo 4", "<h4>"], ["Titolo 5", "<h5>"],
           ["Titolo 6", "<h6>"]],
                useCSS: true, // use CSS to style HTML when possible (not supported in ie)
                docType:       // Document type contained within the editor
          '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">',
                docCSSFile:    // CSS file used to style the document contained within the editor
          "../App_Themes/BlueINPS1/MainStyle.css",
                bodyStyle:    // style to assign to document body contained within the editor
          "margin:4px; font:10pt Arial,Verdana; cursor:text"
            });

            // Il valore 0 dell'array indica il primo elemento con cleditor presente sulla pagina
            // Nel caso in cui volessi il secondo dovrei mettere 1
            $($(".cleditorMain iframe")[0].contentWindow.document).bind('keypress', function (e) {
                if (e.which == '13') {
                    e.preventDefault();
                }
                // MaxLength
                if ($(this).text().length > 70) {
                    e.preventDefault();
                }
            });
        });   
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            DeCodeURI();
        });
    </script>
    <script type="text/javascript">
        function EnCodeURI() {
            var textTitoloAvviso = $("#textareaTitoloAvviso").val();
            textTitoloAvviso = textTitoloAvviso.replace(/\n/gi, "");
            textTitoloAvviso = encodeURI(textTitoloAvviso);
            $("#<%=HiddenFieldTitoloAvviso.ClientID%>").val(textTitoloAvviso);
            $("#textareaTitoloAvviso").val('').blur();

            var src = $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src");
            $("#<%=HiddenFieldVisibleAvviso.ClientID%>").val(src);

            var textEditAvviso = $("#textareaEditAvviso").val();
            textEditAvviso = encodeURI(textEditAvviso);
            $("#<%=HiddenFieldTextEditAvviso.ClientID%>").val(textEditAvviso);
            $("#textareaEditAvviso").val('').blur();
        }

        function DeCodeURI() {
            var textTitoloAvviso = $("#<%=HiddenFieldTitoloAvviso.ClientID%>").val();
            textTitoloAvviso = decodeURI(textTitoloAvviso);
            $("#textareaTitoloAvviso").val(textTitoloAvviso).blur();

            var src = $("#<%=HiddenFieldVisibleAvviso.ClientID%>").val();
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_on.png");
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("title", "Avviso visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_off.png");
                $("#<%=imgbtnVisibleAvviso.ClientID%>").attr("title", "Avviso non visibile. Clicca per modificarne la visibilità.");
            }

            var textEditAvviso = $("#<%=HiddenFieldTextEditAvviso.ClientID%>").val();
            textEditAvviso = decodeURI(textEditAvviso);
            $("#textareaEditAvviso").val(textEditAvviso).blur();
        }

        function validateAvvisi(source, args) {
            var titolo = $("#textareaTitoloAvviso").val();
            titolo = stripHTML(titolo);
            if (!titolo || titolo === undefined || titolo == "" || $.trim(titolo) == "")
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        function validateLengthAvvisi(source, args) {
            var titolo = $("#textareaTitoloAvviso").val();
            titolo = titolo.replace(/\n/gi, "");
            titolo = encodeURI(titolo);
            if (titolo && titolo.length > 500)
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        function stripHTML(text) {
            var re = /(<([^>]+)>)/gi;
            text = text.replace(re, "");
            text = text.replace(/\n/gi, "");
            return text.replace(/&nbsp;/gi, "");
        }
    </script>

    <div class="page-title">
        <h2 class="page-title-secondlevel"><asp:Label ID="lblIntestazione" runat="server" Text=""></asp:Label></h2>
    </div>

    <asp:Panel runat="server" ID="PanelAvviso">
        <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    </asp:Panel>
    
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="ValidationGroupTitoloAvviso"
        Font-Size="Small" Visible="true" />
    <asp:Panel ID="PanelTitoloAvviso" runat="server">
        <div class="container">
            <table width="100%" class="tblAvviso">
                <tr>
                    <td>
                        <asp:Label ID="lblVisibleAvviso" runat="server" Text="Visibile:" Font-Bold="true" CssClass="d-block"></asp:Label>
                        <asp:ImageButton ID="imgbtnVisibleAvviso" runat="server" Height="25px" Width="25px"
                            ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips" TabIndex="2"
                            OnClientClick="return imgbtnVisibleAvviso_ClientClick();" ToolTip="Avviso visibile. Clicca per modificarne la visibilità." CssClass="section-alert__img section-alert__img--toggle" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="LabelTitoloAvviso" runat="server" Text="Titolo:" Font-Bold="true" CssClass="d-block mt-8"></asp:Label>

                        <textarea id="textareaTitoloAvviso" name="textareaTitoloAvviso"></textarea>
                        <asp:CustomValidator runat="server" Display="Dynamic" ErrorMessage="Inserire un titolo"
                            Text="*" CssClass="field-is-required" ValidationGroup="ValidationGroupTitoloAvviso" ID="customCheckTitoloValidator"
                            ClientValidationFunction="validateAvvisi" />
                        <asp:CustomValidator runat="server" Display="Dynamic" ErrorMessage="Il titolo con i suoi componenti html è troppo lungo"
                            Text="*" CssClass="field-is-required" ValidationGroup="ValidationGroupTitoloAvviso" ID="customCheckLengthTitolo"
                            ClientValidationFunction="validateLengthAvvisi" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="PanelEditAvviso" runat="server">
        <div class="container">
            <table width="100%" class="tblAvviso">
                <tr>
                    <td width="100%" align="left">
                        <asp:Label ID="LabelEditAvviso" runat="server" Text="Testo:" Font-Bold="true" CssClass="d-block mt-8"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="left">
                        <textarea id="textareaEditAvviso" name="textareaEditAvviso" tabindex="3">
                        </textarea>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HiddenFieldTitoloAvviso" runat="server" Value="" />
    <asp:HiddenField ID="HiddenFieldVisibleAvviso" runat="server" Value='<%# setImage("turn_on.png") %>' />
    <asp:HiddenField ID="HiddenFieldTextEditAvviso" runat="server" Value="" />

    <div class="container">
        <div class="justify-end">
            <asp:Button ID="btnIndietro" runat="server" Text="Indietro" SkinID="btnAzione1" TabIndex="7"
                            Width="121px" OnClientClick="EnCodeURI();BlockUI();" OnClick="btnIndietro_Click" />

            <asp:Button ID="btnAggiorna" runat="server" Text="" SkinID="btnAzione1" TabIndex="8"
                            Width="121px" ValidationGroup="ValidationGroupTitoloAvviso" CausesValidation="false"
                            OnClientClick="if(Page_ClientValidate('ValidationGroupTitoloAvviso')){aspnetForm.target ='_self'; EnCodeURI(); BlockUI();}"
                            OnClick="btnAggiorna_Click" CssClass="primary button mr-0" />
        </div>
    </div>
</asp:Content>
