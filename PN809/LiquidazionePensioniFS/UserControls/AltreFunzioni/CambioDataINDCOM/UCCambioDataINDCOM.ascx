<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCCambioDataINDCOM.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioDataINDCOM.UCCambioDataINDCOM" %>

<script type="text/javascript">
    function ShowNota() {
        CreatePopUp();
        var text = $(document.getElementById("<%=hdnTextDialog.ClientID %>")).val();
        $('#textDialog').value = text;
        $(document.getElementById("<%=textDialog.ClientID %>")).val(text);
        $('#divdialog').dialog('open');
        SetScroll();
        return false;
    }
    function CreatePopUp() {
        $('#divdialog').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            modal: true,
            resizable: false,
            draggable: true,
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

    function ShowNotaEdit() {
        CreatePopUpEdit();
        var text = $(document.getElementById("<%=hdnTextDialogEdit.ClientID %>")).val();
        $('#textDialogEdit').value = text;
        $(document.getElementById("<%=TextDialogEdit.ClientID %>")).val(text);
        $('#divdialogedit').dialog('open');
        return false;
    }

    function CreatePopUpEdit() {
        $('#divdialogedit').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            modal: true,
            resizable: false,
            draggable: true,
            dialogClass: 'fixed-dialog',
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Modifica': function () {
                    var text = $(document.getElementById("<%=TextDialogEdit.ClientID %>")).val();;
                    $('#textDialogEdit').value = text;
                    $(document.getElementById("<%=hdnTextDialogEdit.ClientID %>")).val(text);

                    $(this).dialog('close');
                    document.getElementById("<%=btnModifica.ClientID %>").click();
                    return true;
                }
            }
        });
    }
</script>

<asp:ValidationSummary runat="server" ID="validtxtDataINDCOM" ValidationGroup="UCCambioDataINDCOM"
    Font-Size="Small" CssClass="errorBox" />
<%--<asp:ValidationSummary runat="server" ID="validationtxtNote" ValidationGroup="UCNumeroCaratteri"
    Font-Size="Small" CssClass="errorBox" />--%>

<asp:Panel ID="panel" runat="server">
                <div class="form-container">
                    <div class="single-line-container">
                        <label class="input-label">Cambio data limite domande INDCOM:</label>

                        <div>
                            <asp:TextBox runat="server" ID="txtDataINDCOM" CssClass="txtUppercase tb8 date-picker-base" Text="gg/mm/aaaa" MaxLength="10" Width="110px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDataSistema" ControlToValidate="txtDataINDCOM"
                                    ErrorMessage="Data in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCCambioDataINDCOM" Enabled="true" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataINDCOM" Display="Dynamic"
                                ErrorMessage="Data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCCambioDataINDCOM"
                                ID="customCheckDataINDCOM" ClientValidationFunction="checkCorrettezzaData" />
                        </div>

                        <asp:Button runat="server" ID="btnApplica" OnClick="btnApplica_Click" OnClientClick="if(Page_ClientValidate('UCCambioDataINDCOM')){aspnetForm.target ='_self'; BlockUI();}"
                            Text="Inserisci" SkinID="btnAzione1" Width="80px" CausesValidation="false" CssClass="primary" />
                    </div>
                </div>
    <table style="width: 100%;" class="tabellaFormattazione">
        <%--STORICO--%>
        <tr>
            <td style="width: 720px">
                <br />
                <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                    Storico date INDCOM</label>
                <asp:GridView runat="server" ID="gvStoricoDataIndcom" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                    Width="100%" PageSize="10" AllowPaging="true" OnRowCommand="gvStoricoDataIndcom_RowCommand"
                    OnRowDataBound="gvStoricoDataIndcom_RowDataBound" OnPageIndexChanging="gvStoricoDataIndcom_onPageIndexChanging"
                    PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun record trovato." SkinID="lblNoData"
                                Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Matricola" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbMatricola" Text='<%#Bind("Matricola")%>'> 
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Modifica" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbDataModifica"> 
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Limite INDCOM" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataLimiteINDCOM"> 
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Note" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lblNote" Text='<%# ValorizzaTesto(((GridViewRow) Container)) %>'
                                    CommandArgument='<%#Eval("Note") %>' CommandName="ShowNota" OnClientClick="findScrollPosition();" CssClass="link-button tertiary ghost ghost--small"> 
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" CommandName="Modifica" 
                                     CommandArgument='<%#Eval("Note") %>' runat="server" OnClientClick="BlockUI();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblId" runat="server" />
                                    <asp:HiddenField runat="server" ID="id" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <div id="divdialog" title="Nota" style="display: none; border-style: none; border-color: White;">
       <textarea readonly CssClass="tb8 txtUppercase" ID="textDialog" runat="server" 
                                TextMode="MultiLine" Rows="10">
                            </textarea>
        <%--<div id="textDialog">--%>
       <%--</div>--%>
    </div>

    <div style="display: flex; justify-content: end">
        <asp:Button ID="btnModifica" runat="server" Text="" OnClick="btnModifica_Click" CssClass="transparentButton primary mr-0 mt-16" />
    </div>
    
    
    <%--popup modifica nota--%>
        <div id="divdialogedit" title="Nota" style="display: none; border-style: none; border-color: White;">
                          <%--  <asp:TextBox CssClass="tb8 txtUppercase" ID="TextDialogEdit" runat="server" 
                                TextMode="MultiLine" Rows="5">
                            </asp:TextBox>--%>
            
            <textarea CssClass="tb8 txtUppercase" ID="TextDialogEdit" runat="server" 
                                TextMode="MultiLine" Rows="10" maxlength="1000">
                            </textarea>
                            <%--<asp:RegularExpressionValidator ID="revTxtNote" runat="server" ControlToValidate="TextDialogEdit"
                                ErrorMessage="Inserimento: E' possibile inserire massimo 1000 caratteri." SetFocusOnError="true"
                                ValidationExpression="[\s\S]{0,1000}" ValidationGroup="UCNumeroCaratteri"
                                Text="*" CssClass="field-is-required" />--%>

        </div>
        <asp:HiddenField runat="server" ID="hdnTextDialog" />
        <asp:HiddenField runat="server" ID="hdnTextDialogEdit" />
    <asp:HiddenField runat="server" ID="hdnIdDialogEdit" />
    <asp:HiddenField runat="server" ID="hdnIndexGrid" />
</asp:Panel>

