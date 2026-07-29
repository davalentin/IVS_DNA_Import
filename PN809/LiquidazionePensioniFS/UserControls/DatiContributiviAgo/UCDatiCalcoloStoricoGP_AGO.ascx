<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloStoricoGP_AGO.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiCalcoloStoricoGP_AGO" %>
<asp:Panel runat="server" ID="pnlAGO" Visible="false">
    <br />
    <div id="pdivRetributivo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class=" full-grid">
                    <asp:Label runat="server" ID="lblDatiRetributivi"> Dati Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center" class=" full-grid">
                    <asp:GridView runat="server" ID="gvDatiRetributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="false" AllowPaging="false" OnRowDataBound="gvDatiRetributivi_RowDataBound"
                        EnableViewState="true" OnLoad="gvDatiRetributivi_Load">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="50px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reddito / Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia" Width="150px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sett. 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane707"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quote Retributivo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuoteRetributivo"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
        <br />
    </div>
    <div id="pdivContributivo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCalcoloContributivo">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvDatiContributivi" runat="server" AllowPaging="false" AutoGenerateColumns="false"
                        AutoGenerateEditButton="false" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella"
                        EnableViewState="true" OnRowDataBound="gvDatiContributivi_RowDataBound" OnLoad="gvDatiContributivi_Load"
                        SkinID="grdElenco1" Width="100%">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato retributivo inserito."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Codice Gestione"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestione_item" runat="server" CssClass="txtUppercase" Width="150px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Ammontare"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmmontareContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontanteContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota Contributiva"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblQuotaContributiva" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlDomandeAUT" runat="server" Visible="false">
        <div id="divPnlDomandeAut" runat="server" style="margin-left: 10px; margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 99%">
                <tr>
                    <td colspan="2">
                        <br />
                    </td>
                </tr>
                <tr class="Row1">
                    <td style="width: 25%">
                        <asp:Label ID="lblFacoltaComputo" runat="server"> Facoltà di computo: </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlFacoltaComputo" CssClass="tb8 txtUppercase xxs"
                            Width="10%">
                            <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlImportoLordoDecorrenza" Visible="false">
        <div id="divPnlImportoLordoDecorrenza" runat="server" style="margin-left: 10px; margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 99%">
                <tr>
                    <td colspan="4">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Codice gestione:
                        </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <label>
                            E - AZIENDA</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Importo lordo alla decorrenza:
                        </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtImportoLordoAllaDecorrenza" CssClass="tb8 txtUppercase"
                            MaxLength="16" Width="90%"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCumulo" Visible="false">
    <div id="divQuotePensione" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblQuotePensione"> Quote Pensione:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvQuotePensione" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="false" AllowPaging="false" OnRowDataBound="gvQuotePensione_RowDataBound"
                        OnDataBound="gvQuotePensione_DataBound" EnableViewState="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Ente/Gestione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnteGestioneFondo" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Descrizione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="27%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDescrizioneFondo" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuota" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane" Width="50px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoQuota" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="4%" Visible="false">
                                <ItemTemplate>
                                    <asp:Image ID="imgVisualizzaTrattenute" alt="Visualizza dati trattenute" title="Visualizza dati trattenute"
                                        Style="cursor: pointer" src="../App_Themes/<%= Page.Theme %>/Images/plus.png" runat="server" />
                                    <asp:HiddenField ID="hdnVisualizzaTrattenute" runat="server" />
                                    </td></tr><tr style="display: none">
                                        <td>
                                            <table width="100%">
                                                <td style="width: 22%">
                                                    <label style="font-weight: bold">
                                                        Trattenute:</label>
                                                </td>
                                                <td style="margin: 15px auto;">
                                                    <asp:GridView runat="server" ID="gvTrattenute" SkinID="grdElenco1" CssClass="intestazioneTabella"
                                                        BorderWidth="1" Width="100%" BorderColor="Black" AutoGenerateColumns="false"
                                                        AllowPaging="false" EnableViewState="true">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Anno competenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblAnnoCompetenza" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("AnnoCompetenza") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Codice trattenute" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblCodiceTrattenute" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("CodiceTrattenute") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Importo trattenute" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblImportoTrattenute" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("ImportoTrattenute")%>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="divQuoteMiglioramentiContrattuali" runat="server" style="margin-left: 10px;
        margin-right: 10px;" visible="false">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblQuoteMiglioramentiContrattuali"> Quote Miglioramenti Contrattuali:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvQuoteMiglioramentiContrattuali" SkinID="grdElenco1"
                        AutoGenerateColumns="false" CssClass="intestazioneTabella" BorderWidth="1" Width="100%"
                        BorderColor="Black" AutoGenerateEditButton="false" AllowPaging="false" OnRowDataBound="gvQuoteMiglioramentiContrattuali_RowDataBound"
                        OnDataBound="gvQuoteMiglioramentiContrattuali_DataBound" EnableViewState="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnteGestioneFondoMiglioramenti" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuotaMiglioramenti" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoQuotaMiglioramenti" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlDAI" Visible="false">
    <br />
    <div id="pDivRetributivoDAI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label1"> Dati Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:GridView runat="server" ID="gvDatiRetributiviDAI" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="false" OnRowDataBound="gvDatiRetributiviDAI_RowDataBound"
                        EnableViewState="true" OnLoad="gvDatiRetributiviDAI_Load">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTipoQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Giorni / Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RMS / RMG" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia" Width="120px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Giorni / Settimane 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane707"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div id="pDivContributivoDAI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label2">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:GridView ID="gvDatiContributiviDAI" runat="server" AllowPaging="false" AutoGenerateColumns="false"
                        AutoGenerateEditButton="false" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella"
                        EnableViewState="true" OnRowDataBound="gvDatiContributivi_RowDataBound" SkinID="grdElenco1"
                        Width="100%">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato retributivo inserito."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Codice Gestione"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestione_item" runat="server" CssClass="txtUppercase" Width="150px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Ammontare"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmmontareContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontanteContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <asp:Panel ID="pnlContributoSolidarieta" Style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
        margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class="Row1" colspan="4">
                    <b>
                        <label>
                            Contributo di solidarietà L. 214/2011</label></b>
                </td>
            </tr>
            <tr>
                <td align="left" style="width: 25%; padding-left: 10px" class="Row1">
                    <label>
                        Anzianità al '95:</label>
                </td>
                <td align="left" style="width: 25%" class="field">
                    <asp:TextBox runat="server" ID="txtAnzAl95" CssClass="txtUppercase tb8" Width="70%"
                        MaxLength="9"></asp:TextBox>
                </td>
                <td align="left" style="width: 25%" class="Row1">
                    <label>
                        Quota al '95:</label>
                </td>
                <td align="left" style="width: 25%" class="field">
                    <asp:TextBox runat="server" ID="txtQuotaAl95" CssClass="txtUppercase tb8" Width="70%"
                        MaxLength="12"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<asp:Panel runat="server" ID="pnlDatiCalcoloAPESociale" Visible="false">
    <table>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Importo Lordo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtImportoLordo" CssClass="tb8 txtUppercase" MaxLength="16"
                    Width="90%"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="min-height: 100px;">
</div>
