<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="Avvisi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Avvisi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css" media="screen">
        table.tblAvviso
        {
            margin: 5px 5px 5px 5px;
            padding: 5px 5px 5px 5px;
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
    </script>

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione avvisi</h2>
    </div>

    <asp:Panel runat="server" ID="PanelAvviso">
        <UCA:UCAvviso runat="server" ID="ucAvviso" />
    </asp:Panel>

    <asp:Panel ID="PanelAvvisiVediTutti" runat="server" ScrollBars="Auto" CssClass="overflow-initial">
        <div id="boxAvvisiVediTutti" style="margin-top: 10px; width: 720px; clear: both;"
            runat="server" class="boxDenunce section-alert__box section-alert">
            <h3>
                <table width="100%" cellpadding="5">
                    <tr>
                        <td align="left" width="70%">
                        </td>
                        <td align="right" width="30%">
                            <asp:LinkButton ID="lnkBtnNuovoAvviso" runat="server" Text="Nuovo avviso" ForeColor="Navy"
                                OnClientClick="BlockUI();" Font-Size="Small" OnClick="lnkBtnNuovoAvviso_Click" CssClass="link-button tertiary">
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </h3>
            <div class="bckGridViewElenco">
                <asp:GridView ID="grdListaAvvisiVediTutti" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                    Width="100%" BorderWidth="1" SkinID="grdElenco1" OnRowDataBound="grdListaAvvisiVediTutti_RowDataBound"
                    OnRowCommand="grdListaAvvisiVediTutti_RowCommand" OnRowDeleting="grdListaAvvisiVediTutti_RowDeleting"
                    PageSize="5" PagerStyle-Font-Size="20px" PagerSettings-Mode="NumericFirstLast"
                    OnPageIndexChanging="grdListaAvvisiVediTutti_onPageIndexChanging" CssClass="data-table"
                    PagerSettings-firstpageimageurl="~/App_Themes/iFrame/Images/first-page.svg" PagerSettings-lastpageimageurl="~/App_Themes/iFrame/Images/last-page.svg">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Lista avvisi generici vuota." SkinID="lblNoData"></asp:Label>
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
                                            <asp:Label ID="lblHeaderIdAvviso" runat="server" Text="Id" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIdAvviso" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderDataAvviso" runat="server" Text="Data" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataAvviso" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="30%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderTitolo" runat="server" Text="Titolo" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="text-align: left">
                                    <asp:Label runat="server" ID="lblTitolo" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="left" class="is-contents">
                                            <asp:Label ID="lblHeaderTesto" runat="server" Text="Testo" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="text-align: left">
                                    <asp:Label runat="server" ID="lblTesto" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderNascondi" runat="server" Text="Visibile" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnNascondiRendiVisibile" runat="server" Height="25px" Width="25px"
                                    ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips" CommandName="Visible"
                                    ToolTip="Avviso visibile. Clicca per modificarne la visibilità." OnClientClick="BlockUI();" CssClass="section-alert__img section-alert__img--toggle" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="4%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderUpdateAvviso" runat="server" Text="" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnUpdateAvviso" runat="server" Height="24px" Width="24px"
                                    ImageUrl='<%# setImage("pencil.png") %>' class="tooltips" CommandName="Update"
                                    ToolTip="Clicca per modificare le informazioni inerenti all'avviso." OnClientClick="BlockUI();" CssClass="section-alert__img" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="TblRecordset3" ItemStyle-VerticalAlign="Middle"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="4%">
                            <HeaderTemplate>
                                <table width="100%" class="is-contents">
                                    <tr class="is-contents">
                                        <td align="center" class="is-contents">
                                            <asp:Label ID="lblHeaderDeleteAvviso" runat="server" Text="" Font-Bold="True" CssClass="section-alert__table-th"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgBtnDeleteAvviso" runat="server" Height="24px" Width="24px"
                                    ImageUrl='<%# setImage("delete24.png") %>' class="tooltips" CommandName="Delete"
                                    ToolTip="Clicca per eliminare l'avviso." OnClientClick="BlockUI();" CssClass="section-alert__img" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%---------------------------------------------------------------------------------------------%>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
