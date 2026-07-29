<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiFondo_PI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondo_PI" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Panel runat="server" ID="pnlTopDatiFondo">
    <asp:Panel runat="server" ID="pnlElencoDatiFondo" Visible="true">
        <div class="containerWidth xs" style="margin-top: 5px;"></div>
        <div style="border: 1px solid black; margin-right: 3px; margin-left: 3px;" class="reset-style">
            <asp:GridView ID="gvElencoDatiFondo" runat="server"
                SkinID="grdElenco1"
                AutoGenerateColumns="False"
                Width="100%"
                GridLines="None"
                ShowHeader="true"
                ShowHeaderWhenEmpty="true"
                OnRowCommand="gvElenco_RowCommand"
                OnRowDataBound="gvElenco_RowDataBound"
                CssClass="intestazioneTabella full-width intestazioneTabella__with-pagination"
                PagerStyle-CssClass="default-pagination-tables">

                <Columns>

                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                        ItemStyle-CssClass="TblRecordset3">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="img" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        HeaderText="Decorrenza Fondo"
                        DataField="DecorrenzaFondo"
                        HeaderStyle-CssClass="intestazioneTabella"
                        ItemStyle-CssClass="TblRecordset3"
                        ItemStyle-HorizontalAlign="Center" />

                    <asp:TemplateField HeaderText=""
                        HeaderStyle-CssClass="intestazioneTabella"
                        ItemStyle-HorizontalAlign="Center"
                        ItemStyle-CssClass="TblRecordset3">

                        <ItemTemplate>

                            <asp:HiddenField ID="hdnIdFondo"
                                runat="server"
                                Value='<%# Eval("IdFondo") %>' />

                            <asp:HiddenField ID="hdnIdRecordFondo"
                                runat="server"
                                Value='<%# Eval("IdRecordFondo") %>' />

                            <asp:Button ID="btnModificaDatiFondo"
                                runat="server"
                                Text="Modifica"
                                SkinID="btnAzione1"
                                CommandName="Modifica"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="editIconOnly tertiary"
                                OnClientClick="BlockUI();" />

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

                <EmptyDataTemplate>
                    <table width="100%" style="min-height: 40px;">
                        <tr>
                            <td style="text-align: center; padding: 10px;">
                                <asp:Label ID="lblNoData"
                                    runat="server"
                                    Text="Nessun dato trovato."
                                    SkinID="lblNoData" />
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>


            </asp:GridView>
            <div style="text-align: center; margin: 20px 0;">
                <asp:Button
                    ID="btnAggiungiFondo"
                    runat="server"
                    Text="Aggiungi Fondo"
                    SkinID="btnAzione1"
                    OnClick="btnAggiungiFondo_Click"
                    OnClientClick="BlockUI();"
                     CssClass="tertiary force-left" />
            </div>
        </div>

    </asp:Panel>

    <br />

    <asp:Panel runat="server" ID="pnlDettaglioDatiFondo" Visible="false">

        <div style="border: 1px solid #000080; width: 710px; margin: 4px;" class="reset-style full-width">
            <table class="tabellaFormattazione grid grid-size-20">

                <tr>
                    <td class="Row1 shift-full-grid"><span class="section-label">Dati Fondo</span></td>
                </tr>

                <tr>
                    <td class="Row1">Decorrenza Fondo:    
                    </td>
                    <td class="field" style="text-align: left">
                        <asp:TextBox runat="server" ID="txtDecorrenzaFondo" CssClass="txtUppercase tb8 date-picker-base" Text="gg/mm/aaaa" MaxLength="10" Width="110px"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDecorrenzaFondo" ControlToValidate="txtDecorrenzaFondo"
                            ErrorMessage="Decorrenza Fondo in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiFondoPI" Enabled="true" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaFondo" Display="Dynamic"
                            ErrorMessage="Decorrenza Fondo: data illogica" Text="*" ValidationGroup="UCTabDatiFondoPI"
                            ID="customCheckDataDataSistema" ClientValidationFunction="checkCorrettezzaData" />
                    </td>

                    <td class="Row1">Codice non calcolo:    
                    </td>
                    <td>
                        <asp:DropDownList
                            CssClass="tb8 txtUppercase xxs"
                            ID="ddlCodiceNonCalcolo"
                            runat="server"
                            Width="70px">
                            <asp:ListItem Text=" " Value=" "></asp:ListItem>
                            <asp:ListItem Text="1 - SI" Value="S"></asp:ListItem>
                            <asp:ListItem Text="0 - NO" Value="N"></asp:ListItem>
                        </asp:DropDownList>
                    </td>

                </tr>

                <tr>

                    <td class="Row1" style="width: 25%">Cod. non vedente:</td>
                    <td class="field" style="width: 25%">
                        <asp:CheckBox ID="chkCodNonVedente" runat="server" />
                    </td>

                    <td class="Row1" style="width: 25%">Qualifica:</td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtQualifica" runat="server" CssClass="tb8 txtUppercase" Width="60%" />
                    </td>

                </tr>

                <tr>
                    <asp:Panel runat="server" ID="pnlRiscatti">

                        <td class="Row1" style="width: 25%">
                            <asp:Label runat="server" ID="lblRiscatti" Text="Riscatti:"></asp:Label>
                        </td>
                        <td class="Row1 fileds-date-input" colspan="3">
                            <asp:TextBox ID="txtRiscattiAA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                                TabIndex="14" MaxLength="2"></asp:TextBox>
                            <label>
                                AA</label>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtRiscattiAA"
                                ErrorMessage="Riscatti AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiFondoPI" CssClass="field-is-required"/>
                            <asp:TextBox ID="txtRiscattiMM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                                TabIndex="15" MaxLength="2" ></asp:TextBox>
                            <label>
                                MM</label>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtRiscattiMM"
                                ErrorMessage="Riscatti MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiFondoPI" CssClass="field-is-required"/>
                            <asp:TextBox ID="txtRiscattiGG" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                                TabIndex="16" MaxLength="2"></asp:TextBox>
                            <label>
                                GG</label>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtRiscattiGG"
                                ErrorMessage="Riscatti GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiFondoPI" CssClass="field-is-required"/>
                        </td>
                    </asp:Panel>

                </tr>
                <tr>

                    <td class="Row1">Importo pensione:</td>
                    <td class="field">
                        <asp:TextBox ID="txtImportoPensione" runat="server" CssClass="tb8 txtUppercase" Width="60%" />
                    </td>

                    <td class="Row1" runat="server" id="trLblPensioneFacoltativaMensile" visible="false"
                        style="width: 25%">
                        <label>
                            Pensione Facoltativa:</label>
                    </td>
                    <td class="field" runat="server" id="trTxtPensioneFacoltativaMensile" visible="false"
                        style="width: 25%">
                        <asp:TextBox runat="server" ID="txtPensioneFacoltativaMensile" MaxLength="9" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtPensioneFacoltativaMensile" runat="server"
                            ControlToValidate="txtPensioneFacoltativaMensile" Display="Dynamic" Enabled="true"
                            ErrorMessage="Pensione facoltativa: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,4}(,\d{1,4})?$" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Importo IIS: </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtImportoIIS" runat="server" CssClass="tb8 txtUppercase" Width="60%" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>Incremento DPR 346: </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtIncrementoDPR346" runat="server" CssClass="tb8 txtUppercase" Width="60%" />
                        <asp:RegularExpressionValidator ID="REVIncrementoDPR346" runat="server"
                            ControlToValidate="txtIncrementoDPR346" Display="Dynamic" Enabled="true"
                            ErrorMessage="Incremento DPR 346: Inserire valori interi o decimali"
                            Text="*" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Assegno personale 36/bis:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtStipendioBase" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtStipendioBase" runat="server" ControlToValidate="txtStipendioBase"
                            Display="Dynamic" Enabled="true" ErrorMessage="Stipendio Base: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                        <asp:RequiredFieldValidator runat="server" ID="RFVtxtStipendioBase" ControlToValidate="txtStipendioBase"
                            Display="Dynamic" Enabled="true" ErrorMessage="Stipendio Base: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcoloPI" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>

                <asp:Panel runat="server" ID="pnlControcodiceRetr" Visible="true">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Controcodice retribuzione:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" ID="txtControCodiceRetribuzione" MaxLength="3" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REVtxtControCodiceRetribuzione" runat="server" ControlToValidate="txtControCodiceRetribuzione"
                                Display="Dynamic" Enabled="true" ErrorMessage="Controcodice retribuzione: Inserire valori interi"
                                Text="*" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
                            <asp:RequiredFieldValidator runat="server" ID="RFVtxtControCodiceRetribuzione" ControlToValidate="txtControCodiceRetribuzione"
                                Display="Dynamic" Enabled="true" ErrorMessage="Controcodice retribuzione: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcoloPI" Text="*"></asp:RequiredFieldValidator>
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Diritto IIS:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:DropDownList runat="server" ID="ddlAttCon" CssClass="tb8 txtUppercase" Width="60%">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 25%">
                             <asp:Label ID="lblNumeroMatricola" runat="server">Matricola:</asp:Label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" ID="txtNumeroMatricola" MaxLength="8" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>

                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Scatti:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" ID="txtScatti" MaxLength="2" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>

                        </td>
                    </tr>
                </asp:Panel>

            </table>
        </div>

        <asp:Panel runat="server" ID="pnlCatV" Visible="false">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Retribuzione Media Settimanale A:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtRMSQuotaAFondo" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSQuotaAFondo" runat="server" ControlToValidate="txtRMSQuotaAFondo"
                            Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Settimane A:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtNSettimaneQuotaA" MaxLength="4" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaA" runat="server" ControlToValidate="txtNSettimaneQuotaA"
                            Display="Dynamic" Enabled="true" ErrorMessage="Settimane A: Inserire valori interi"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Retribuzione Media Settimanale B:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtRMSQuotaBFondo" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSQuotaBFondo" runat="server" ControlToValidate="txtRMSQuotaBFondo"
                            Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Settimane B:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtNSettimaneQuotaB" MaxLength="4" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaB" runat="server" ControlToValidate="txtNSettimaneQuotaB"
                            Display="Dynamic" Enabled="true" ErrorMessage="Settimane B: Inserire valori interi"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlCatU" Visible="false">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>

                    <td class="Row1" style="width: 25%">
                        <label>
                            Codice di Maggiorazione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:DropDownList runat="server" ID="ddlCodiceMaggiorazione" CssClass="tb8 txtUppercase" Width="30%">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="0" Value="0"></asp:ListItem>
                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Pens.Compl.Riv 1/95:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtPensComplRiv1_95" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtPensComplRiv1_95" runat="server" ControlToValidate="txtPensComplRiv1_95"
                            Display="Dynamic" Enabled="true" ErrorMessage="Pens.Compl.Riv 1/95: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlCatAB" Visible="false">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Percentuale di Capitalizzazione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtPercentualeCapitalizzazione" MaxLength="7" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                        %
					<asp:RegularExpressionValidator ID="REVtxtPercentualeCapitalizzazione" runat="server" ControlToValidate="txtPercentualeCapitalizzazione"
                        Display="Dynamic" Enabled="true" ErrorMessage="Percentuale di Capitalizzazione: Inserire valori interi o decimali (max 2 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,2}(,\d{1,4})?$" />
                    </td>
                    <td class="Row1" style="width: 50%"></td>
                </tr>
            </table>
        </asp:Panel>


        <br />

        <div class="containerWidth xs reset-style full-width" style="margin-top: 10px;">
            <table width="100%" class="tab-actions-group">
                <tr>
                    <td style="text-align: center;"  class="tab-actions-group__first">

                        <asp:Button
                            ID="btnSalvaDettaglioFondo"
                            runat="server"
                            SkinID="btnAzione1"
                            Text="Salva"
                            OnClick="btnSalvaDettaglioFondo_Click"
                            OnClientClick="BlockUI();" 
                            CssClass="primary force-right" />

                        &nbsp;
                        
                        <asp:Button
                            ID="btnEliminaDettaglioFondo"
                            runat="server"
                            SkinID="btnAzione1"
                            Text="Elimina"
                            CausesValidation="false"
                            OnClick="btnEliminaDettaglio_Click"
                            OnClientClick="BlockUI();"
                            CssClass="ghost-delete force-center" />

                        &nbsp;
                        <asp:Button
                            ID="btnTornaElencoFondo"
                            runat="server"
                            SkinID="btnAzione1"
                            Text="Torna alla lista"
                            CausesValidation="false"
                            OnClick="btnTornaElencoFondo_Click"
                            OnClientClick="BlockUI();"
                            CssClass="force-left" />

                    </td>
                </tr>
            </table>
        </div>

    </asp:Panel>
</asp:Panel>

<asp:HiddenField ID="hfIdFondo" runat="server" />
<asp:HiddenField ID="hfIdRecordFondo" runat="server" />

<asp:HiddenField runat="server" ID="hdnAttCon" Value="" />
 <asp:HiddenField ID="hdnTest2" runat="server" />
