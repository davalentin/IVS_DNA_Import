<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="GestioneFAQ.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.GestioneFAQ" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css" media="screen">
        /*.container
        {
            font-family: Verdana,Arial,Helvetica,sans-serif;
            font-size: small;
            height: auto;
            width: 720px;
            border: 1px;
            border-style: solid;
            border-color: #d4d0c8;
            float: none;
            margin-top: 15px;
        }*/
        table.tblAvviso
        {
            margin: 5px 5px 5px 5px;
            padding: 5px 5px 5px 5px;
        }
        
        .fixed-dialog{
          position: fixed;
        }
    </style>

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

        function CreatePopUp() {
            $('#divdialog').dialog({
                autoOpen: false,
                show: 'blind',
                hide: 'blind',
                width: 650,
                modal: true,
                resizable: false,
                draggable: true,
                centerX: true,
                centerY: true,
                dialogClass: 'fixed-dialog',
                open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    'Ok': function () {
                        $(this).dialog('close');
                        return true;
                    }
                }
            });
        }

        function ShowRisposta() {
            CreatePopUp();
            var text = $(document.getElementById("<%=hdnTextDialog.ClientID %>")).val();
            text = replaceAll("\n", "<br />", text);
            $('#textDialog').html(text);
            $('#divdialog').dialog('open');
            SetScroll();
            return false;
        }

        function SetScroll() {
            window.scrollBy(document.getElementById("<%= scrollX.ClientID %>").value, document.getElementById("<%= scrollY.ClientID %>").value);
        }

        function findScrollPosition() {
            var scrolledX;
            var scrolledY;

            scrolledX = document.documentElement.scrollLeft;
            scrolledY = document.documentElement.scrollTop;

            document.getElementById("<%= scrollX.ClientID %>").value = scrolledX;
            document.getElementById("<%= scrollY.ClientID %>").value = scrolledY;
        }

        function replaceAll(find, replace, str) {
            var re = new RegExp(find, 'g');
            str = str.replace(re, replace);
            return str; 
        }
    </script>

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione FAQ</h2>
    </div>

    <asp:Panel runat="server" ID="PanelAvviso">
        <UCA:UCAvviso runat="server" ID="ucAvviso" />
    </asp:Panel>

    <asp:Panel ID="PanelFAQ" runat="server" ScrollBars="Auto" CssClass="overflow-initial">
        <div id="boxFAQ" style="margin-top: 10px; width: 720px; clear: both;"
            runat="server" class="boxDenunce section-alert__box section-alert">
            <h3>
                <table width="100%" cellpadding="5">
                    <tr>
                        <td align="left" width="70%">
                        </td>
                        <td align="right" width="30%">
                            <asp:LinkButton ID="lnkBtnNuovaFAQ" runat="server" Text="Nuova domanda" ForeColor="Navy"
                                OnClientClick="BlockUI();" Font-Size="Small" OnClick="lnkBtnNuovaDomanda_Click" CssClass="link-button tertiary">
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </h3>
            <div class="bckGridViewElenco">
                <asp:GridView ID="grdListaFAQ" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                    Width="100%" BorderWidth="1" PageSize="7" SkinID="grdElenco1" OnRowDataBound="grdListaFAQ_RowDataBound"
                    OnRowCommand="grdListaFAQ_RowCommand" OnRowDeleting="grdListaFAQ_RowDeleting" 
                    OnPageIndexChanging="grdListaFAQ_onPageIndexChanging" PagerSettings-Mode="NumericFirstLast" CssClass="data-table">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Lista FAQ vuota." SkinID="lblNoData"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" Visible="false">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHidden" runat="server" Text="Hidden" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIdFAQ" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderCodice" runat="server" Text="Id" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceFAQ" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderDomanda" runat="server" Text="Domanda" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDomanda" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ControlStyle-CssClass="link-button tertiary ghost ghost--small">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderRisposta" runat="server" Text="Risposta" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lblRisposta" Text='Vedi Risposta' Width="75px"  CommandArgument='<%#Eval("Risposta") %>' CommandName="ShowRisposta"
                                OnClientClick="findScrollPosition();"> 
                            </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderNascondi" runat="server" Text="Visibilit&agrave" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnNascondiRendiVisibile" runat="server" Height="25px"
                                    Width="25px" ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips"
                                    CommandName="Visible" ToolTip="FAQ visibile. Clicca per modificarne la visibilità."
                                    OnClientClick="BlockUI();" CssClass="section-alert__img section-alert__img--toggle"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="4%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderUpdateFAQ" runat="server" Text="" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnUpdateFAQ" runat="server" Height="24px" Width="24px"
                                    ImageUrl='<%# setImage("pencil.png") %>' class="tooltips" 
                                    CommandName="Update" ToolTip="Clicca per modificare le informazioni inerenti alla domanda."
                                    OnClientClick="BlockUI();" CssClass="section-alert__img"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="4%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderDeleteFAQ" runat="server" Text="" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgBtnDeleteFAQ" runat="server" Height="24px" Width="24px"
                                    ImageUrl='<%# setImage("delete24.png") %>' class="tooltips" 
                                    CommandName="Delete" ToolTip="Clicca per eliminare la domanda." OnClientClick="BlockUI();" CssClass="section-alert__img"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div id="divdialog" title="Nota" style="border-style: none; border-color: White; display: none; vertical-align: top"><div id="textDialog"></div></div>
        <asp:HiddenField runat="server" ID="hdnTextDialog" />
        <asp:HiddenField runat="server" ID="scrollX" />
        <asp:HiddenField runat="server" ID="scrollY" />
    </asp:Panel>
</asp:Content>