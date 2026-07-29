<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="MessaggiHermesEdit.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.MessaggiHermesEdit" %>

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
        function imgbtnVisibleMessaggioHermes_ClientClick() {
            var src = $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src");
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_off.png");
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("title", "Messaggio Hermes non visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_on.png");
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("title", "Messaggio Hermes visibile. Clicca per modificarne la visibilità.");
            }
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#textareaEditMessaggioHermes").cleditor({
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

            $("#textareaTitoloMessaggioHermes").cleditor({
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
            var textTitoloMessaggioHermes = $("#textareaTitoloMessaggioHermes").val();
            textTitoloMessaggioHermes = textTitoloMessaggioHermes.replace(/\n/gi, "");
            textTitoloMessaggioHermes = encodeURI(textTitoloMessaggioHermes);
            $("#<%=HiddenFieldTitoloMessaggioHermes.ClientID%>").val(textTitoloMessaggioHermes);
            $("#textareaTitoloMessaggioHermes").val('').blur();

            var src = $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src");
            $("#<%=HiddenFieldVisibleMessaggioHermes.ClientID%>").val(src);

            var textEditMessaggioHermes = $("#textareaEditMessaggioHermes").val();
            textEditMessaggioHermes = encodeURI(textEditMessaggioHermes);
            $("#<%=HiddenFieldTextEditMessaggioHermes.ClientID%>").val(textEditMessaggioHermes);
            $("#textareaEditMessaggioHermes").val('').blur();

            var textUrlMessaggioHermes = $("#<%=TextBoxUrlMessaggioHermes.ClientID%>").val();
            textUrlMessaggioHermes = encodeURI(textUrlMessaggioHermes);
            $("#<%=HiddenFieldUrlMessaggioHermes.ClientID%>").val(textUrlMessaggioHermes);
            $("#<%=TextBoxUrlMessaggioHermes.ClientID%>").val();

            var textCategoriaMessaggioHermes = $("#<%=ddlCategoriaMessaggioHermes.ClientID%>").val();
            textCategoriaMessaggioHermes = encodeURI(textCategoriaMessaggioHermes);
            $("#<%=HiddenFieldCategoriaMessaggioHermes.ClientID%>").val(textCategoriaMessaggioHermes);
            $("#<%=ddlCategoriaMessaggioHermes.ClientID%>").val();

        }

        function DeCodeURI() {
            var textTitoloMessaggioHermes = $("#<%=HiddenFieldTitoloMessaggioHermes.ClientID%>").val();
            textTitoloMessaggioHermes = decodeURI(textTitoloMessaggioHermes);
            $("#textareaTitoloMessaggioHermes").val(textTitoloMessaggioHermes).blur();

            var src = $("#<%=HiddenFieldVisibleMessaggioHermes.ClientID%>").val();
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_on.png");
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("title", "Messaggio Hermes visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_off.png");
                $("#<%=imgbtnVisibleMessaggioHermes.ClientID%>").attr("title", "Messaggio Hermes non visibile. Clicca per modificarne la visibilità.");
            }

            var textEditMessaggioHermes = $("#<%=HiddenFieldTextEditMessaggioHermes.ClientID%>").val();
            textEditMessaggioHermes = decodeURI(textEditMessaggioHermes);
            $("#textareaEditMessaggioHermes").val(textEditMessaggioHermes).blur();

            var textUrlMessaggioHermes = $("#<%=HiddenFieldUrlMessaggioHermes.ClientID%>").val();
            textUrlMessaggioHermes = decodeURI(textUrlMessaggioHermes);
            $("#<%=TextBoxUrlMessaggioHermes.ClientID%>").val(textUrlMessaggioHermes);

            var textCategoriaMessaggioHermes = $("#<%=HiddenFieldCategoriaMessaggioHermes.ClientID%>").val();
            textCategoriaMessaggioHermes = decodeURI(textCategoriaMessaggioHermes);
            $("#<%=ddlCategoriaMessaggioHermes.ClientID%>").val(textCategoriaMessaggioHermes);
        }

        function validateMessaggiHermes(source, args) {
            var titolo = $("#textareaTitoloMessaggioHermes").val();
            titolo = stripHTML(titolo);
            if (!titolo || titolo === undefined || titolo == "" || $.trim(titolo) == "")
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        function validateLengthMessaggiHermes(source, args) {
            var titolo = $("#textareaTitoloMessaggioHermes").val();
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
    
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="ValidationGroupMessaggioHermes"
        Font-Size="Small" Visible="true" />
    <asp:Panel ID="PanelTitoloMessaggioHermes" runat="server">
        <div class="container">
            <table width="100%" class="tblMessaggioHermes">
                <tr>
                    <td>
                        <asp:Label ID="lblVisibleMessaggioHermes" runat="server" Text="Visibilit&agrave:" Font-Bold="true" CssClass="d-block"></asp:Label>
                        <asp:ImageButton ID="imgbtnVisibleMessaggioHermes" runat="server" Height="25px" Width="25px"
                            ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips" TabIndex="2"
                            OnClientClick="return imgbtnVisibleMessaggioHermes_ClientClick();" ToolTip="Messaggio Hermes visibile. Clicca per modificarne la visibilità." CssClass="section-alert__img section-alert__img--toggle" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="LabelTitoloMessaggioHermes" runat="server" Text="Titolo:" Font-Bold="true" CssClass="d-block mt-8"></asp:Label>

                        <textarea id="textareaTitoloMessaggioHermes" name="textareaTitoloMessaggioHermes">
                        </textarea>

                        <asp:CustomValidator runat="server" Display="Dynamic" ErrorMessage="Inserire un titolo"
                            Text="*" CssClass="field-is-required" ValidationGroup="ValidationGroupMessaggioHermes" ID="customCheckTitoloValidator"
                            ClientValidationFunction="validateMessaggiHermes" />
                        <asp:CustomValidator runat="server" Display="Dynamic" ErrorMessage="Il titolo con i suoi componenti html è troppo lungo"
                            Text="*" CssClass="field-is-required" ValidationGroup="ValidationGroupMessaggioHermes" ID="customCheckLengthTitolo"
                            ClientValidationFunction="validateLengthMessaggiHermes" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="PanelUrlMessaggioHermes" runat="server">
        <div class="container mt-8">
            <table width="100%" class="tblMessaggioHermes">
                <tr>
                    <td width="75%" align="left">
                        <asp:Label ID="LabelUrlMessaggioHermes" runat="server" Text="Url:" Font-Bold="true"></asp:Label>
                    </td>
                    <td width="25%" align="left">
                        <asp:Label ID="LabelCategoriaMessaggioHermes" runat="server" Text="Tipologia:" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="75%" align="left">
                        <asp:TextBox Width="90%" ID="TextBoxUrlMessaggioHermes" runat="server" MaxLength="500"
                            TabIndex="3" CssClass="tb8"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorUrlMessaggioHermes"
                            ControlToValidate="TextBoxUrlMessaggioHermes" ErrorMessage="Inserire Url" Text="*" CssClass="field-is-required"
                            Display="Dynamic" ValidationGroup="ValidationGroupMessaggioHermes" />
                        <asp:RegularExpressionValidator runat="server" ID="RequlagExpUrlMessaggioHermes"
                            ControlToValidate="TextBoxUrlMessaggioHermes" ErrorMessage="Url non corretto. Deve iniziare con (http://, https://, ftp://)"
                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="ValidationGroupMessaggioHermes" ValidationExpression="^(http|https|ftp)\://.+" />
                    </td>
                    <td width="25%" align="left" style="vertical-align: top">
                        <asp:DropDownList ID="ddlCategoriaMessaggioHermes" runat="server" TabIndex="3" CssClass="txtUppercase tb8"
                            Width="140px">
                            <asp:ListItem Text="MESSAGGIO" Value="MESSAGGIO"></asp:ListItem>
                            <asp:ListItem Text="CIRCOLARE" Value="CIRCOLARE"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="PanelEditMessaggioHermes" runat="server">
        <div class="container mt-8">
            <table width="100%" class="tblMessaggioHermes">
                <tr>
                    <td width="100%" align="left">
                        <asp:Label ID="LabelEditMessaggioHermes" runat="server" Text="Testo:" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="left">
                        <textarea id="textareaEditMessaggioHermes" name="textareaEditMessaggioHermes" tabindex="4">
                        </textarea>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HiddenFieldTitoloMessaggioHermes" runat="server" Value="" />
    <asp:HiddenField ID="HiddenFieldVisibleMessaggioHermes" runat="server" Value='<%# setImage("turn_on.png") %>' />
    <asp:HiddenField ID="HiddenFieldTextEditMessaggioHermes" runat="server" Value="" />
    <asp:HiddenField ID="HiddenFieldUrlMessaggioHermes" runat="server" Value="" />
    <asp:HiddenField ID="HiddenFieldCategoriaMessaggioHermes" runat="server" Value="" />


    <div class="container">
        <div class="justify-end">
            <asp:Button ID="btnIndietro" runat="server" Text="Indietro" SkinID="btnAzione1" TabIndex="7"
                            Width="121px" OnClientClick="EnCodeURI();BlockUI();" OnClick="btnIndietro_Click" />

            <asp:Button ID="btnAggiorna" runat="server" Text="" SkinID="btnAzione1" TabIndex="8"
                            Width="121px" ValidationGroup="ValidationGroupTitoloMessaggioHermes" CausesValidation="false"
                            OnClientClick="if(Page_ClientValidate('ValidationGroupMessaggioHermes')){aspnetForm.target ='_self'; EnCodeURI(); BlockUI();}"
                            OnClick="btnAggiorna_Click" CssClass="primary mr-0" />
        </div>
    </div>
    

    <asp:Panel ID="PanelAggiornaMessaggioHermes" runat="server">
        <div class="container">
            <table width="100%" class="tblMessaggioHermes section-alert__edit-buttons">
                <tr class="is-contents">
                    <td align="right" class="section-alert__edit-save">
                        
                    </td>
                    <td align="left">
                        
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
