<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCGestioneBancheFideiussione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneBancheFideiussione.UCGestioneBancheFideiussione" %>
<table class="tabellaFormattazione">
    <!--filtro ricerca-->
    <tr>
        <td>
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" CssClass="form-container background-light-blue">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Codice Azienda:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroCodiceAzienda"
                                Width="100px" MaxLength="4" />
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Matricola:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroMatricola" Width="100px"
                                MaxLength="11" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Progressivo:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroProgressivo" CssClass="tb8 txtUppercase"
                                Width="100px" Enabled="false" MaxLength="2" />
                        </td>
                        <td class="Row1">
                            <label>
                                Anno:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroAnno" CssClass="tb8 txtUppercase" Width="100px"
                                Enabled="false" MaxLength="4" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="end">
                            <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();" />
                            <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="BlockUI();" CssClass="primary mr-0" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <!-- fine filtro ricerca-->
    <!--- griglia banche-->
    <tr>
        <td>
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Banche Fideiussione</label>
            <center>
                <asp:GridView runat="server" ID="gvBancheFideiussione" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvBancheFideiussione_RowEditing" Width="1050px" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvBancheFideiussione_RowCommand" OnRowCancelingEdit="gvBancheFideiussione_RowCancelingEdit"
                    OnRowDataBound="gvBancheFideiussione_RowDataBound" OnPageIndexChanging="gvBancheFideiussione_onPageIndexChanging"
                    OnRowDeleting="gvBancheFideiussione_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Codice Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="9%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceAzienda" Text='<%# Bind("CodiceAzienda")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceAzienda" MaxLength="4"
                                    Text=' <%# Bind("CodiceAzienda")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regularTxtCodiceAzienda" ControlToValidate="txtCodiceAzienda"
                                    Display="Dynamic" ErrorMessage="Inserire il Codice Azienda in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Matricola" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMatricola" Text='<%#Bind("Matricola")%>'> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" ID="txtMatricola" Text='<%#Bind("Matricola")%>'
                                    runat="server" Width="95px" MaxLength="11">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regularTxtMatricola" ControlToValidate="txtMatricola"
                                    Display="Dynamic" ErrorMessage="Inserire la Matricola in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banca Fideiussione" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="14%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblBancaFideiussione" Text='<%#Bind("BancaFideiussione")%>'> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" ID="txtBancaFideiussione" runat="server"
                                    MaxLength="200" Text=' <%# Bind("BancaFideiussione")%>' Width="120px">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Progressivo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblProgressivo" Text='<%# Bind("Progressivo")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtProgressivo" MaxLength="2"
                                    Text=' <%# Bind("Progressivo")%>' Width="30px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtProgressivo" ControlToValidate="txtProgressivo"
                                    Display="Dynamic" ErrorMessage="Inserire il Progressivo in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Anno" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="6%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAnno" Text='<%# Bind("Anno")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtAnno" MaxLength="4"
                                    Text=' <%# Bind("Anno")%>' Width="40px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtAnno" ControlToValidate="txtAnno"
                                    Display="Dynamic" ErrorMessage="Inserire l'Anno in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Inizio Esodo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="14%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblInizioEsodo" Text='<%# Bind("InizioEsodo", "{0:dd/MM/yyyy}")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                    ID="txtInizioEsodo" MaxLength="10" Text='<%# Bind("InizioEsodo", "{0:dd/MM/yyyy}")%>'>
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateInizioEsodo" ControlToValidate="txtInizioEsodo"
                                    Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                    ValidationGroup="GrigliaBanche" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtInizioEsodo" Display="Dynamic"
                                    ErrorMessage="La data di Inizio Esodo inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaBanche"
                                    ID="customCheckDataInizioEsodo" ClientValidationFunction="checkCorrettezzaData" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fine Esodo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="14%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblFineEsodo" Text='<%# Bind("FineEsodo", "{0:dd/MM/yyyy}")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                    ID="txtFineEsodo" MaxLength="10" Text=' <%# Bind("FineEsodo","{0:dd/MM/yyyy}")%>'>
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateFineEsodo" ControlToValidate="txtFineEsodo"
                                    Display="Dynamic" ErrorMessage="Inserire la data nel formato giorno/mese/anno"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                    ValidationGroup="GrigliaBanche" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtFineEsodo" Display="Dynamic"
                                    ErrorMessage="La data di Fine Esodo inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaBanche"
                                    ID="customCheckFineEsodo" ClientValidationFunction="checkCorrettezzaData" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ABI" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="7%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblABI" Text='<%# Bind("ABI")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtABI" MaxLength="5"
                                    Text=' <%# Bind("ABI")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtABI" ControlToValidate="txtABI"
                                    Display="Dynamic" ErrorMessage="Inserire ABI in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CAB" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="8%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCAB" Text='<%# Bind("CAB")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCAB" MaxLength="7"
                                    Text=' <%# Bind("CAB")%>' Width="60px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtCAB" ControlToValidate="txtCAB"
                                    Display="Dynamic" ErrorMessage="Inserire CAB in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaBanche" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="2%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                    OnClientClick="BlockUI();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
    <!--fine griglia banche-->
    <!--griglia aziende--->
    <tr>
        <td>
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Aziende</label>
            <center>
                <asp:GridView runat="server" ID="gvAziende" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAziende_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAziende_RowCommand"
                    OnRowDataBound="gvAziende_RowDataBound" OnPageIndexChanging="gvAziende_onPageIndexChanging"
                    PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Codice Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceAzienda" Text='<%# Bind("TraduzioneSuGP")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceAzienda" MaxLength="4"
                                    Text=' <%# Bind("TraduzioneSuGP")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtCodiceAzienda" ControlToValidate="txtCodiceAzienda"
                                    Display="Dynamic" ErrorMessage="Inserire il Codice Azienda in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaAziende" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrizione" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="75%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescrizione" Text='<%#Bind("Descrizione")%>'> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8" runat="server" ID="txtDescrizione" MaxLength="150" Text='<%#Bind("Descrizione")%>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
    <!--fine griglia aziende-->
    <!--griglia aziende giorno mese anno-->
    <tr>
        <td>
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Aziende per filtro L92 con scadenza assegno in formato Giorno/Mese/Anno</label>
            <center>
                <asp:GridView runat="server" ID="gvAziendeGGmmAAAA" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAziendeGGmmAAAA_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAziendeGGmmAAAA_RowCommand"
                    OnRowDataBound="gvAziendeGGmmAAAA_RowDataBound" OnPageIndexChanging="gvAziendeGGmmAAAA_onPageIndexChanging"
                    PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Codice Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="13%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceAziendaGGmmAAAA" Text='<%# Bind("TraduzioneSuGP")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceAziendaGGmmAAAA" MaxLength="4"
                                    Text=' <%# Bind("TraduzioneSuGP")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtCodiceAziendaGGmmAAAA" ControlToValidate="txtCodiceAziendaGGmmAAAA"
                                    Display="Dynamic" ErrorMessage="Inserire il Codice Azienda in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaAziendeGGmmAAAA" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrizione" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="65%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescrizione" >                                
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Progressivo richiesto" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="13%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceProgressivoRichiesto" Text='<%# Bind("ProgressivoRichiesto")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceProgressivoRichiesto" MaxLength="2"
                                    Text=' <%# Bind("ProgressivoRichiesto")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtCodiceProgressivoRichiesto" ControlToValidate="txtCodiceProgressivoRichiesto"
                                    Display="Dynamic" ErrorMessage="Inserire il Progressivo in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{2}$" ValidationGroup="GrigliaAziendeGGmmAAAA" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnElimina" CommandName="Elimina" CommandArgument="Elimina" runat="server"
                                OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
    <!--fine griglia aziende giorno mese anno-->
</table>
