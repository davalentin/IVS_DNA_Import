<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSentenzaArt4.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCSentenzaArt4" %>

<asp:Panel runat="server" ID="pnlSentenzaArt4">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvSentenzaArt4" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvSentenzaArt4_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvSentenzaArt4_RowCommand" OnRowDataBound="gvSentenzaArt4_RowDataBound"
                        OnPageIndexChanging="gvSentenzaArt4_onPageIndexChanging"  PagerStyle-CssClass="default-pagination-tables">

                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza Sentenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaSentenzaArt4" Text='<%# Bind("DecorrenzaSentenza", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase">      
                                    </asp:Label>
                                     <asp:HiddenField runat="server" ID="hdnIsFromGP" Value='<%#Bind("IsFromGP")%>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaSentenzaArt4"
                                        MaxLength="7" Text=' <%# Bind("DecorrenzaSentenza", "{0:MM/yyyy}")%>' Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredTxtDecorrenzaSentenzaArt4" ControlToValidate="txtDecorrenzaSentenzaArt4"
                                        Enabled="true" ErrorMessage="Decorrenza Sentenza obbligatoria" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSentenzaArt4" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaSentenzaArt4"
                                        Display="Dynamic" ControlToValidate="txtDecorrenzaSentenzaArt4" Enabled="true"
                                        ErrorMessage="Decorrenza Sentenza Art. 4: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSentenzaArt4"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSentenzaArt4" Display="Dynamic"
                                        ErrorMessage="Decorrenza Sentenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSentenzaArt4"
                                        ID="customCheckDataDataSentenzaArt4" ClientValidationFunction="checkCorrettezzaData" />
                                     <asp:HiddenField runat="server" ID="hdnIsFromGP" Value='<%#Bind("IsFromGP")%>'/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Sentenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoSentenzaArt4" Text='<%#Bind("ImportoSentenza")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtImportoSentenzaArt4" runat="server" Text='<%#Bind("ImportoSentenza")%>' CssClass="tb8 txtUppercase"
                                        Width="50%" MaxLength="16"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                                        ControlToValidate="txtImportoSentenzaArt4" Enabled="true" ErrorMessage="Importo Sentenza: Inserire valori interi o decimali (max 8 interi e 7 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSentenzaArt4" ValidationExpression="\d{0,8}(,\d{1,7})?" />
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="txtImportoSentenzaArt4"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Importo Sentenza: campo obbligatorio"
                                        ValidationGroup="UCTabSentenzaArt4" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Elimina" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>                           
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <div id="tastoAnnulla" style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalva" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Sentenza Art.4" Width="170px" 
                        OnClick="btnSalvaSentenzaArt4_Click" OnClientClick="if(Page_ClientValidate('UCTabSentenzaArt4')){aspnetForm.target ='_self'; BlockUI();}" CausesValidation="false" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnElimina" runat="server" Enabled="true" SkinID="btnAzione1" Text="Elimina Sentenza Art.4" Width="170px"
                        OnClick="btnEliminaSentenzaArt4_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la Sentenza Art. 4?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>  
</asp:Panel>

