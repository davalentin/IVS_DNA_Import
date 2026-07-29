<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="AggiornamentiEdit.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AggiornamentiEdit" %>

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
        function imgbtnVisibleAggiornamento_ClientClick() {
            var src = $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src");
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_off.png");
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("title", "Aggiornamento non visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_on.png");
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("title", "Aggiornamento visibile. Clicca per modificarne la visibilità.");
            }
            return false;
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#textareaEditAggiornamento").cleditor({
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
        });   
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            DeCodeURI();
        });
    </script>
    <script type="text/javascript">
        function EnCodeURI() {
            var textTitoloAggiornamento = $("#<%=TextBoxTitoloAggiornamento.ClientID%>").val();
            textTitoloAggiornamento = encodeURI(textTitoloAggiornamento);
            $("#<%=HiddenFieldTitoloAggiornamento.ClientID%>").val(textTitoloAggiornamento);
            $("#<%=TextBoxTitoloAggiornamento.ClientID%>").val();

            var src = $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src");
            $("#<%=HiddenFieldVisibleAggiornamento.ClientID%>").val(src);

            var textEditAggiornamento = $("#textareaEditAggiornamento").val();
            textEditAggiornamento = encodeURI(textEditAggiornamento);
            $("#<%=HiddenFieldTextEditAggiornamento.ClientID%>").val(textEditAggiornamento);
            $("#textareaEditAggiornamento").val('').blur();
        }

        function DeCodeURI() {
            var textTitoloAggiornamento = $("#<%=HiddenFieldTitoloAggiornamento.ClientID%>").val();
            textTitoloAggiornamento = decodeURI(textTitoloAggiornamento);
            $("#<%=TextBoxTitoloAggiornamento.ClientID%>").val(textTitoloAggiornamento);

            var src = $("#<%=HiddenFieldVisibleAggiornamento.ClientID%>").val();
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_on.png");
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("title", "Aggiornamento visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_off.png");
                $("#<%=imgbtnVisibleAggiornamento.ClientID%>").attr("title", "Aggiornamento non visibile. Clicca per modificarne la visibilità.");
            }

            var textEditAggiornamento = $("#<%=HiddenFieldTextEditAggiornamento.ClientID%>").val();
            textEditAggiornamento = decodeURI(textEditAggiornamento);
            $("#textareaEditAggiornamento").val(textEditAggiornamento).blur();
        }
    </script>

    <div class="page-title">
        <h2 class="page-title-secondlevel"><asp:Label ID="lblIntestazione" runat="server" Text=""></asp:Label></h2>
    </div>

    <asp:Panel runat="server" ID="PanelAvviso">
        <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    </asp:Panel>

    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="ValidationGroupTitoloAggiornamento"
        Font-Size="Small" Visible="true" />
    <asp:Panel ID="PanelTitoloAggiornamento" runat="server">
        <div class="container">
            <table width="100%" class="tblAggiornamento">
                <tr>
                    <td>
                        <asp:Label ID="lblVisibleAggiornamento" runat="server" Text="Visibilit&agrave:" Font-Bold="true" CssClass="d-block"></asp:Label>
                        <asp:ImageButton ID="imgbtnVisibleAggiornamento" runat="server" Height="25px" Width="25px"
                            ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips" TabIndex="2"
                            OnClientClick="return imgbtnVisibleAggiornamento_ClientClick();" ToolTip="Aggiornamento visibile. Clicca per modificarne la visibilità." CssClass="section-alert__img--toggle section-alert__img"/>
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="LabelTitoloAggiornamento" runat="server" Text="Titolo:" Font-Bold="true" CssClass="d-block mt-8"></asp:Label>

                        <asp:TextBox Width="90%" ID="TextBoxTitoloAggiornamento" runat="server" MaxLength="70"
                            TabIndex="1" CssClass="tb8"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTitoloAggiornamento"
                            ControlToValidate="TextBoxTitoloAggiornamento" ErrorMessage="Inserire un titolo"
                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="ValidationGroupTitoloAggiornamento" />
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="TextBoxTitoloAggiornamento"
                            ErrorMessage="Lunghezza massima non consentita" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="ValidationGroupTitoloAggiornamento"
                            ValidationExpression="^[\s\S]{0,70}$" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="PanelEditAggiornamento" runat="server">
        <div class="container">
            <table width="100%" class="tblAggiornamento">
                <tr>
                    <td width="100%" align="left">
                        <asp:Label ID="LabelEditAggiornamento" runat="server" Text="Testo:" Font-Bold="true" CssClass="d-block mt-8"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="left">
                        <textarea id="textareaEditAggiornamento" name="textareaEditAggiornamento" tabindex="3">
                        </textarea>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HiddenFieldTitoloAggiornamento" runat="server" Value="" />
    <asp:HiddenField ID="HiddenFieldVisibleAggiornamento" runat="server" Value='<%# setImage("turn_on.png") %>' />
    <asp:HiddenField ID="HiddenFieldTextEditAggiornamento" runat="server" Value="" />

    <div class="container">
        <div class="justify-end">
            <asp:Button ID="btnIndietro" runat="server" Text="Indietro" SkinID="btnAzione1" TabIndex="7"
                                Width="121px" OnClientClick="EnCodeURI();BlockUI();" OnClick="btnIndietro_Click" />

            <asp:Button ID="btnAggiorna" runat="server" Text="" SkinID="btnAzione1" TabIndex="8"
                                Width="121px" ValidationGroup="ValidationGroupTitoloAggiornamento" CausesValidation="false"
                                OnClientClick="if(Page_ClientValidate('ValidationGroupTitoloAggiornamento')){aspnetForm.target ='_self'; EnCodeURI(); BlockUI();}"
                                OnClick="btnAggiorna_Click" CssClass="primary mr-0" />
        </div>
    </div>
    
</asp:Content>
