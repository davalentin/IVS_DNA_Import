<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCBititolaritaINAIL.ascx.cs" 
Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCBititolaritaINAIL" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />

<asp:Panel ID="pnlIntegrazioneMinimoBititolare" runat="server" Visible="true"><br />
    <div id="pdivElimContestuale" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td colspan="4" class="Row1" style="text-align:left">
                    <label style="font-weight: bold">Integrazione minimo bititolare</label>
                </td>
            </tr>
            <tr>
                    <label class="section-label">Decorrenza Diritto</label>

                <td class="Row1" style="width:25%">
                    <asp:TextBox ID="txtDecorrenzaDiritto" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="1" Width="50%" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1"
                        ControlToValidate="txtDecorrenzaDiritto" Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Diritto: Inserire una data valida"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBititolarita" Text="*" CssClass="field-is-required" />   
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaDiritto" Display="Dynamic"
                        ErrorMessage="Decorrenza Diritto: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBititolarita"
                        ID="customCheckDataDecorrenzaDiritto" ClientValidationFunction="checkCorrettezzaData" />            
                </td>
                <td class="Row1" style="width:25%">
                    <label id="decorrenzaCessazione">Decorrenza Cessazione:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:TextBox ID="txtDecorrenzaCessazione" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="2" Width="50%" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2"
                        ControlToValidate="txtDecorrenzaCessazione" Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Cessazione: Inserire una data valida"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBititolarita" Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaCessazione" Display="Dynamic"
                        ErrorMessage="Decorrenza Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBititolarita"
                        ID="customCheckDataDecorrenzaCessazione" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<asp:Panel ID="PnlInvalidita" runat="server" Visible="true"><br />
    <div id="divInvalidita" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td colspan="4" class="shift-full-grid" style="text-align:left">
                    <label style="font-weight: bold" class="section-label mt-32">Pensione Invalidità</label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width:25%">
                    <label>Sospensione:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:TextBox ID="txtSospensione" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="3" Width="50%" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3"
                        ControlToValidate="txtSospensione" Display="Dynamic" Enabled="true" ErrorMessage="Sospensione: Inserire una data valida"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBititolarita" Text="*" CssClass="field-is-required" />   
                    <asp:CustomValidator runat="server" ControlToValidate="txtSospensione" Display="Dynamic"
                        ErrorMessage="Sospensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBititolarita"
                        ID="customCheckDataSospensione" ClientValidationFunction="checkCorrettezzaData" />              
                </td>
                <td class="Row1" style="width:25%">
                    <label>Ripristino:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:TextBox ID="txtRipristino" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="4" Width="50%" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4"
                        ControlToValidate="txtRipristino" Display="Dynamic" Enabled="true" ErrorMessage="Ripristino: Inserire una data valida"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBititolarita" Text="*" CssClass="field-is-required" />   
                    <asp:CustomValidator runat="server" ControlToValidate="txtRipristino" Display="Dynamic"
                        ErrorMessage="Ripristino: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBititolarita"
                        ID="customCheckDataRipristino" ClientValidationFunction="checkCorrettezzaData" />             
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width:25%">
                    <label>Importo da recuperare:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:TextBox ID="txtImportoRecuperare" runat="server" CssClass="tb8 txtUppercase" MaxLength="8" TabIndex="5" Width="25%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5"
                        ControlToValidate="txtImportoRecuperare" Display="Dynamic" ErrorMessage="Importo da recuperare: inserire l'importo in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d+(\,\d{1,4})?" ValidationGroup="UCTabBititolarita" />                               
                </td>
            </tr>
        </table>
     </div>
</asp:Panel>

<asp:Panel ID="pnlGridViewRenditaINAIL" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align:left">
                                <asp:Label ID="lblTitoloRenditaINAIL" runat="server" Text="Rendita INAIL" style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:GridView runat="server" ID="gvRenditaINAIL" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="false" OnRowCommand="gvRenditaINAIL_RowCommand"
                        OnRowDataBound="gvRenditaINAIL_RowDataBound" OnRowCancelingEdit="gvRenditaINAIL_RowCancelingEdit"
                        OnRowEditing="gvRenditaINAIL_RowEditing" OnRowUpdating="gvRenditaINAIL_RowUpdating"
                        EnableViewState="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1" 
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="29%" ItemStyle-Width="29%" FooterStyle-Width="29%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>' 
                                        Width="70px"/>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabINAIL" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtDecorrenzaCE" runat="server" ErrorMessage="Decorrenza: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenza" ValidationGroup="UCTabINAIL" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator> 
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabINAIL"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />  
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="29%" ItemStyle-Width="29%" FooterStyle-Width="29%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImporto"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtImporto" runat="server" MaxLength="11"
                                        Width="50%" Text='<%#Bind("Importo") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimane"
                                        ControlToValidate="txtImporto" Display="Dynamic" ErrorMessage="Importo: inserire l'importo in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d+(\,\d{1,4})?" ValidationGroup="UCTabINAIL" Enabled="true"/>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtImporto" runat="server" ErrorMessage="Importo: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtImporto" ValidationGroup="UCTabINAIL" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Evento" HeaderStyle-CssClass="intestazioneTabella Row1" 
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="30%" ItemStyle-Width="30%" FooterStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEvento_item" Width="120px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlEvento" Width="120px" CssClass="txtUppercase tb8 xxs"> 
                                        <asp:ListItem Value="" Text=""></asp:ListItem>  
                                        <asp:ListItem Value="SI" Text="SI"></asp:ListItem>  
                                        <asp:ListItem Value="NO" Text="NO"></asp:ListItem>                                  
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlEvento" runat="server" ErrorMessage="Evento: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlEvento" ValidationGroup="UCTabINAIL" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteRenditaINAIL" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditRenditaINAIL" Value="false" /> 
</asp:Panel>

<asp:Panel ID="pnlAssegnoAcc" runat="server" Visible="true" CssClass="mt-32"><br />
    <div id="divAssegnoAcc" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td colspan="4" class="Row1 shift-full-grid" style="text-align:left">
                    <label style="font-weight: bold" class="section-label">Assegno Accompagnamento</label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width:25%">
                    <label>Diritto:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:DropDownList runat="server" ID="ddlDiritto" Width="50px" CssClass="txtUppercase tb8 xxs" >
                        <asp:ListItem Text=""   Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    </asp:DropDownList>                 
                </td>
                <td class="Row1" style="width:25%">
                    <label>Decorrenza:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:TextBox ID="txtDecAssegno" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="2" Width="50%" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7"
                        ControlToValidate="txtDecAssegno" Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Assegno Accompagnamento: Inserire una data valida"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBititolarita" Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecAssegno" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBititolarita"
                        ID="customCheckDataDecorrenzaAssegno" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<div style="width: 100%; margin-top: 25px; margin-right: 40px;">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaBititolaritaInail" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Bititolarità/Inail" 
                    Width="180px" OnClick="SalvaBititolaritaInail_Click" OnClientClick="if(Page_ClientValidate('UCTabBititolarita')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaBititolaritaInail" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Bititolarità/Inail" 
                    Width="180px" OnClick="EliminaBititolaritaInail_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati Bititolarità/Inail?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>