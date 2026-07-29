using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Caching;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using System.ComponentModel;
using INPS.DNA.Context;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public class CodeUtility
    {
        #region LiquidazionePensione

        internal static List<RecordFondo> CreaRecord()
        {
            List<RecordFondo> elencoRecordFondo = new List<RecordFondo>();
            elencoRecordFondo = AggiungiRecord(elencoRecordFondo, '\0', '\0', '\0', ' ', new DateTime(), new DateTime());
            return elencoRecordFondo;
        }

        internal static List<RecordFondo> AggiungiRecord(List<RecordFondo> listaRecord, char? natura1, char? natura2, char? natura3, Char nonCalcolo, DateTime? decorrenzaRecord, DateTime? sospensioneRecord)
        {
            RecordFondo record = new RecordFondo();
            record._CodiceNatura1 = natura1;
            record._CodiceNatura2 = natura2;
            record._CodiceNatura3 = natura3;
            record._CodiceNonCalcolo = nonCalcolo;
            record._DecorrenzaValiditaDati = decorrenzaRecord;
            record._DataSospensione = sospensioneRecord;
            listaRecord.Add(record);
            return listaRecord;
        }

        internal static List<RecordFondo> EliminaRecordVuoti(List<RecordFondo> elencoRecordFondo)
        {
            int i = 0;
            int j = 0;
            List<int> elementiDaEliminare = new List<int>();
            foreach (RecordFondo recordFondo in elencoRecordFondo)
            {
                if (!recordFondo._CodiceNatura1.HasValue && !recordFondo._CodiceNatura2.HasValue &&
                    !recordFondo._CodiceNatura3.HasValue && !recordFondo._DataSospensione.HasValue &&
                    (!recordFondo._DecorrenzaValiditaDati.HasValue || recordFondo._DecorrenzaValiditaDati.Value == DateTime.MinValue))
                {
                    elementiDaEliminare.Add(j);
                    elementiDaEliminare[j] = i;
                    j++;
                }
                i++;
            }

            for (int z = 0; z < j; z++)
                elencoRecordFondo.RemoveAt(elementiDaEliminare[z]);
            return elencoRecordFondo;
        }

        internal static List<RecordFondo> ModificaRecord(List<RecordFondo> elencoRecordFondo, int index, char? natura1, char? natura2, char? natura3, Char nonCalcolo, DateTime? decorrenzaRecord, DateTime? sospensioneRecord)
        {
            elencoRecordFondo[index]._CodiceNatura1 = natura1;
            elencoRecordFondo[index]._CodiceNatura2 = natura2;
            elencoRecordFondo[index]._CodiceNatura3 = natura3;
            elencoRecordFondo[index]._DecorrenzaValiditaDati = decorrenzaRecord;
            elencoRecordFondo[index]._DataSospensione = sospensioneRecord;
            elencoRecordFondo[index]._CodiceNonCalcolo = nonCalcolo;

            return elencoRecordFondo;

        }

        internal static void SetCampiGridEdit(GridViewRow Row, bool IsFirstRecord, object DecorrenzaPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            string formatDate = string.Empty;
            string validatorDate = string.Empty;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        formatDate = "{0:dd/MM/yyyy}";
                        validatorDate = "01/01/0001";
                        break;
                    default:
                        formatDate = "{0:MM/yyyy}";
                        validatorDate = "01/0001";
                        break;
                }
            }

            DropDownList ddlCodNatura1 = (DropDownList)Row.FindControl("ddlCodNatura1");
            DropDownList ddlCodNatura2 = (DropDownList)Row.FindControl("ddlCodNatura2");
            DropDownList ddlCodNatura3 = (DropDownList)Row.FindControl("ddlCodNatura3");
            DropDownList ddlCodiceNonCalcolo = (DropDownList)Row.FindControl("ddlCodiceNonCalcolo");
            TextBox txtDecorrenzaRecordFondo = (TextBox)Row.FindControl("txtDecorrenzaRecordFondo");
            TextBox txtSospensioneRecordFondo = (TextBox)Row.FindControl("txtSospensioneRecordFondo");

            ddlCodNatura1.SelectedValue = ((RecordFondo)Row.DataItem)._CodiceNatura1.ToString();
            ddlCodNatura2.SelectedValue = ((RecordFondo)Row.DataItem)._CodiceNatura2.ToString();
            ddlCodNatura3.SelectedValue = ((RecordFondo)Row.DataItem)._CodiceNatura3.ToString();
            ddlCodiceNonCalcolo.SelectedValue = ((RecordFondo)Row.DataItem)._CodiceNonCalcolo.ToString();

            string decorrenzaValiditaDati = String.Format(formatDate, ((RecordFondo)Row.DataItem)._DecorrenzaValiditaDati);
            if (IsFirstRecord)
            {
                if (DecorrenzaPensione != null)
                    txtDecorrenzaRecordFondo.Text = String.Format(formatDate, (DateTime?)DecorrenzaPensione);
            }
            else
            {
                if (String.Equals(decorrenzaValiditaDati, validatorDate))
                    txtDecorrenzaRecordFondo.Text = string.Empty;
            }
            string dataSospensione = String.Format(formatDate, ((RecordFondo)Row.DataItem)._DataSospensione);
            if (String.Equals(dataSospensione, validatorDate))
                txtSospensioneRecordFondo.Text = string.Empty;
        }

        internal static void ManageCampiGridEdit(GridViewRow row, bool isFirstRecord, AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            bool isCodiceNonCalcoloEnabled = true;
            bool isDecorrenzaEnabled = true;
            bool isSospensioneEnabled = true;
            RecordFondo record = row.DataItem as RecordFondo;
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    break;
                default:
                    if (Utility.IsDomandaReversibilita(datiPensione))
                    {
                        isCodiceNonCalcoloEnabled = isFirstRecord ? false : (record != null ? !record._IsFromDB : true);
                        isDecorrenzaEnabled = isFirstRecord ? false : (record != null ? !record._IsFromDB : true);
                        isSospensioneEnabled = isFirstRecord ? false : (record != null ? !record._IsFromDB : true);
                    }
                    break;
            }
            DropDownList ddlCodiceNonCalcolo = (DropDownList)row.FindControl("ddlCodiceNonCalcolo");
            TextBox txtDecorrenzaRecordFondo = (TextBox)row.FindControl("txtDecorrenzaRecordFondo");
            TextBox txtSospensioneRecordFondo = (TextBox)row.FindControl("txtSospensioneRecordFondo");
            ddlCodiceNonCalcolo.Enabled = isCodiceNonCalcoloEnabled;
            txtDecorrenzaRecordFondo.Enabled = isDecorrenzaEnabled;
            txtSospensioneRecordFondo.Enabled = isSospensioneEnabled;
        }

        internal static bool LoadRecordEsenzioneFiscaleAGO_CI(string id, string gruppo, bool isRiapertura, bool? isEsenzioneFiscaleEstero, bool isEsenzioneFiscaleVittima)
        {
            if ((id == "1" && (gruppo.Equals("0031") || isRiapertura) && isEsenzioneFiscaleVittima) || (id == "2" && isEsenzioneFiscaleEstero.GetValueOrDefault() == true))
                return true;
            else
                return false;
        }

        internal static void ManagePanelEsenzioneFiscaleAGO_CI(ref Panel panel, bool isVisible, string gruppo, bool isRiapertura)
        {
            if (gruppo.Equals("0031") || isRiapertura)
                panel.Visible = true;
            else
                panel.Visible = isVisible;
        }

        internal static bool LoadRecordEsenzioneFiscaleFS(string id, bool? isEsenzioneFiscale, bool? isCodComunicazioniEsenzioneFiscaleVittimaVisibile, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, bool isDomandaINPDAP, bool isRiaperturaDomanda, AreaTitolare.DatiPensione datiPensione)
        {
            //mettere qui modifica??
            if (((tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || isDomandaINPDAP) &&
                (id != "2" && id != "1") || (id == "2" && isEsenzioneFiscale.GetValueOrDefault() == true)
                || (id == "1" && (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) || isCodComunicazioniEsenzioneFiscaleVittimaVisibile.GetValueOrDefault() == true)))
                ||
                (!(tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || isDomandaINPDAP) &&
                (id != "2" && id != "1") || (id == "2" && isEsenzioneFiscale.GetValueOrDefault() == true) || (id == "1" && isCodComunicazioniEsenzioneFiscaleVittimaVisibile.GetValueOrDefault() == true)))
                return true;
            else
                return false;
        }

        internal static void ManageRecordEsenzioneFiscale(ref DropDownList ddlCodComunicazioni4, bool isRicostituzione,
            bool isRicostituzioneperMotividocumentaliesenzionefiscalevittimedeldovere)
        {
            if (isRicostituzione && !isRicostituzioneperMotividocumentaliesenzionefiscalevittimedeldovere && ddlCodComunicazioni4 != null && ddlCodComunicazioni4.SelectedItem.Text == "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE")
                ddlCodComunicazioni4.Enabled = false;

        }

        #endregion LiquidazionePensione

        #region VisualizzaStatoPratica

        public static List<string> GetCriteriRicerca(UtilityTipoAppartenenza tipoAppRuolo, UtilityRuolo ruolo)
        {
            List<string> listaCriteri;

            if (tipoAppRuolo == UtilityTipoAppartenenza.AGO || tipoAppRuolo == UtilityTipoAppartenenza.CI)
            {
                listaCriteri = new List<string> {"", "Numero Domanda", "Categoria Pensione", "Stato Pratica", 
                                //10-05-2012: commentato sede perchè privo di significato in presenza di filtro lato WCF su sede + co operatore
                                //"Sede", 
                                "Anagrafica", "Codice Fiscale",
                                "Data Presentazione", "Data Elaborazione", "Matricola", "PL/TRF e/o RIC in lavorazione",
                                "PL/TRF e/o RIC lavorate", "Gruppo", "Prodotto", "Tipo"};
            }
            else
            {
                listaCriteri = new List<string> {"", "Numero Domanda", "Categoria Pensione", "Stato Pratica", 
                                //10-05-2012: commentato sede perchè privo di significato in presenza di filtro lato WCF su sede + co operatore
                                //"Sede", 
                                "Fondo", "Cassa", "Anagrafica", "Codice Fiscale",
                                "Data Presentazione", "Data Elaborazione", "Matricola", "PL/TRF e/o RIC in lavorazione",
                                "PL/TRF e/o RIC lavorate", "Gruppo", "Prodotto", "Tipo"};
            }

            if (ruolo == UtilityRuolo.AMMINISTRATORE)
                listaCriteri.Add("Sede");

            return listaCriteri;
        }

        public static string LoadSediECo()
        {
            string retString = string.Empty;
            string sedi = string.Empty;
            List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediECoProvinciali();
            foreach (INPS.DNA.Office office in listOffice)
            {
                sedi = string.Concat(office.AspnCode, "-", (office.ExtendedProperties != null ? office.ExtendedProperties["SEDE"].Trim() : office.Name.Trim()), ";");
                retString = string.Concat(retString, sedi);
            }
            return retString;
        }

        public static List<INPS.DNA.Office> GetListaSediECoProvinciali()
        {
            //recupero elenco sedi escludendo quelle regionali (ZZCode == 80)
            return (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                    where o.ZZCode != "80"
                    select o).OrderBy(x => (x.ExtendedProperties != null ? x.ExtendedProperties["SEDE"].Trim() : x.Name.Trim())).ToList();
        }

        public static List<INPS.DNA.Office> GetListaSediProvinciali()
        {
            //recupero elenco sedi escludendo quelle regionali (ZZCode == 80) e i centri operativi veri (ultime 2 cifre dell'AspnCode != 00)
            return (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                    where o.AspnCode.PadLeft(4, '0').PadRight(6, '0').Substring(4, 2) == "00" && o.ZZCode != "80"
                    select o).OrderBy(x => (x.ExtendedProperties != null ? x.ExtendedProperties["SEDE"].Trim() : x.Name.Trim())).ToList();
        }

        #endregion VisualizzaStatoPratica

        #region Trasversali

        public Presenter.SvrLiquidazione.AreaDecodifica GetValuesDecodifica()
        {
            Presenter.SvrLiquidazione.AreaDecodifica rispostaDecodifica = new AreaDecodifica();
            if (Cache.Get<Presenter.SvrLiquidazione.AreaDecodifica>("Decodifica") == null)
            {
                rispostaDecodifica = Utility.ServizioGetDecodifica();
                InserisciElementoBianco(rispostaDecodifica);
                Cache.Add<Presenter.SvrLiquidazione.AreaDecodifica>("Decodifica", rispostaDecodifica);
            }
            else
                rispostaDecodifica = Cache.Get<Presenter.SvrLiquidazione.AreaDecodifica>("Decodifica");
            return rispostaDecodifica;
        }

        private void InserisciElementoBianco(AreaDecodifica rispostaDecodifica)
        {
            AreaDecodifica.DatiStatoEstero liEstero = new AreaDecodifica.DatiStatoEstero();
            AreaDecodifica.DatiComunicazioneCampo3 liComunicazioniC3 = new AreaDecodifica.DatiComunicazioneCampo3();
            AreaDecodifica.DatiComunicazioneCampo4 liComunicazioniC4 = new AreaDecodifica.DatiComunicazioneCampo4();
            AreaDecodifica.DatiCategoriaPensione liCategoriaPensione = new AreaDecodifica.DatiCategoriaPensione();
            AreaDecodifica.DatiFondoPensione liFondoPensione = new AreaDecodifica.DatiFondoPensione();
            AreaDecodifica.DatiStatoPensione liStatoPensione = new AreaDecodifica.DatiStatoPensione();
            AreaDecodifica.DatiCategoriaPensione liCodCategoria = new AreaDecodifica.DatiCategoriaPensione();
            AreaDecodifica.DatiComunicazioneCampi1_2 liCodComunicazioneCampo1_2 = new AreaDecodifica.DatiComunicazioneCampi1_2();
            AreaDecodifica.DatiCodiciNatura liCodNatura1 = new AreaDecodifica.DatiCodiciNatura();
            AreaDecodifica.DatiCodiciNatura liCodNatura2 = new AreaDecodifica.DatiCodiciNatura();
            AreaDecodifica.DatiCodiciNatura liCodNatura3 = new AreaDecodifica.DatiCodiciNatura();
            AreaDecodifica.DatiParentelaDC liParentelaDC = new AreaDecodifica.DatiParentelaDC();
            AreaDecodifica.DatiCodiciImportoAltraPensione liImportoAltraPensione = new AreaDecodifica.DatiCodiciImportoAltraPensione();
            AreaDecodifica.DatiTipoPensione liTipoPensione = new AreaDecodifica.DatiTipoPensione();
            AreaDecodifica.DatiCodeGestioneCalcoloContrib liCodeGestioneCalcoloContrib = new AreaDecodifica.DatiCodeGestioneCalcoloContrib();
            AreaDecodifica.DatiCodeGestioneCalcoloRetrib liCodeGestioneCalcoloRetrib = new AreaDecodifica.DatiCodeGestioneCalcoloRetrib();

            List<AreaDecodifica.DatiStatoEstero> listStatoEstero = rispostaDecodifica.ElencoStatiEsteri.ToList();
            List<AreaDecodifica.DatiTipoCalcolo> listTipoCalcolo = rispostaDecodifica.ElencoTipoCalcolo.ToList();
            List<AreaDecodifica.DatiCausaCarico> listCausaCarico = rispostaDecodifica.ElencoCausaCarico.ToList();
            List<AreaDecodifica.DatiComunicazioneCampi1_2> listComunicazioniC1_2 = rispostaDecodifica.ElencoComunicazioneCampi1_2.ToList();
            List<AreaDecodifica.DatiComunicazioneCampo4> listComunicazioniC4 = rispostaDecodifica.ElencoComunicazioneCampo4.ToList();
            List<AreaDecodifica.DatiComunicazioneCampo3> listComunicazioniC3 = rispostaDecodifica.ElencoComunicazioneCampo3.ToList();
            List<AreaDecodifica.DatiGradoInvalidita> listGradoInvalidita = rispostaDecodifica.ElencoGradoInvalidita.ToList();
            List<AreaDecodifica.DatiProrataEnel> listProrataEnel = rispostaDecodifica.ElencoProrataEnel.ToList();
            List<AreaDecodifica.DatiCodiceAzienda> listCodiceAzienda = rispostaDecodifica.ElencoCodiceAzienda.ToList();
            List<AreaDecodifica.DatiCategoriaPensione> listCategoriaPensione = rispostaDecodifica.ElencoCategoriePensione.ToList();
            List<AreaDecodifica.DatiFondoPensione> listFondiSpeciali = rispostaDecodifica.ElencoFondiPensione.ToList();
            List<AreaDecodifica.DatiStatoPensione> listStatoPensione = rispostaDecodifica.ElencoStatiPensione.ToList();
            List<AreaDecodifica.DatiCodiciNatura> listCodiciNatura = rispostaDecodifica.ElencoCodiciNatura.ToList();
            List<AreaDecodifica.DatiParentelaDC> listParentelaDC = rispostaDecodifica.ElencoParentelaDC.ToList();
            List<AreaDecodifica.DatiCodiciImportoAltraPensione> listImportoAltraPensione = rispostaDecodifica.ElencoCodiciImportoAltraPensione.ToList();
            List<AreaDecodifica.DatiCodiciProvenienza> listCodiciProvenienza = rispostaDecodifica.ElencoCodiciProvenienza.ToList();
            List<AreaDecodifica.DatiTipoPensione> listTipoPensione = rispostaDecodifica.ElencoTipoPensione.ToList();
            List<AreaDecodifica.DatiCodeGestioneCalcoloContrib> listCodeGestioneCalcoloContrib = rispostaDecodifica.ElencoCodeGestioneCalcoloContrib.ToList();
            List<AreaDecodifica.DatiCodeGestioneCalcoloRetrib> listCodeGestioneCalcoloRetrib = rispostaDecodifica.ElencoCodeGestioneCalcoloRetrib.ToList();

            liEstero.CodCatastale = "";
            liEstero.Descrizione = "";
            listStatoEstero.Insert(0, liEstero);
            rispostaDecodifica.ElencoStatiEsteri = listStatoEstero.ToArray();

            liCodComunicazioneCampo1_2.Campo2 = ' ';
            liCodComunicazioneCampo1_2.Descrizione = "";
            liCodComunicazioneCampo1_2.Tipologia = "";
            listComunicazioniC1_2.Add(liCodComunicazioneCampo1_2);

            liComunicazioniC3.Descrizione = "";
            liComunicazioniC3.Id = "";
            liComunicazioniC4.Descrizione = "";
            liComunicazioniC4.Id = "";
            listComunicazioniC4.Add(liComunicazioniC4);
            listComunicazioniC3.Add(liComunicazioniC3);

            liCategoriaPensione.Codice = "";
            liCategoriaPensione.Sigla = "";
            listCategoriaPensione.Add(liCategoriaPensione);

            liFondoPensione.DescFondo = "";
            liFondoPensione.CodFondo = "";
            listFondiSpeciali.Add(liFondoPensione);

            liStatoPensione.DecodificaStato = "";
            liStatoPensione.CodiceStato = "";
            listStatoPensione.Add(liStatoPensione);

            liParentelaDC.Descrizione = string.Empty;
            liParentelaDC.Id = string.Empty;
            listParentelaDC.Add(liParentelaDC);
            liImportoAltraPensione.Id = string.Empty;
            liImportoAltraPensione.Descrizione = string.Empty;
            listImportoAltraPensione.Add(liImportoAltraPensione);

            liTipoPensione.Id = ' ';
            liTipoPensione.Descrizione = string.Empty;
            listTipoPensione.Add(liTipoPensione);

            liCodeGestioneCalcoloContrib.Id = 0;
            liCodeGestioneCalcoloContrib.Descrizione = string.Empty;
            liCodeGestioneCalcoloContrib.IsFondo = false;
            liCodeGestioneCalcoloContrib.TraduzioneSuGP = string.Empty;

            liCodeGestioneCalcoloRetrib.Id = 0;
            liCodeGestioneCalcoloRetrib.Descrizione = string.Empty;
            liCodeGestioneCalcoloRetrib.IsFondo = false;
            liCodeGestioneCalcoloRetrib.TraduzioneSuGP = string.Empty;

            listCodeGestioneCalcoloContrib = listCodeGestioneCalcoloContrib.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCodeGestioneCalcoloContrib = listCodeGestioneCalcoloContrib.ToArray();

            listCodeGestioneCalcoloRetrib = listCodeGestioneCalcoloRetrib.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCodeGestioneCalcoloRetrib = listCodeGestioneCalcoloRetrib.ToArray();

            listTipoPensione = listTipoPensione.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoTipoPensione = listTipoPensione.ToArray();

            listCodiciProvenienza = listCodiciProvenienza.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCodiciProvenienza = listCodiciProvenienza.ToArray();

            listImportoAltraPensione = listImportoAltraPensione.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCodiciImportoAltraPensione = listImportoAltraPensione.ToArray();

            listParentelaDC = listParentelaDC.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoParentelaDC = listParentelaDC.ToArray();

            listTipoCalcolo = listTipoCalcolo.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoTipoCalcolo = listTipoCalcolo.ToArray();

            listCausaCarico = listCausaCarico.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCausaCarico = listCausaCarico.ToArray();

            listComunicazioniC1_2 = listComunicazioniC1_2.OrderBy(x => x.Campo2.GetValueOrDefault()).ToList();
            rispostaDecodifica.ElencoComunicazioneCampi1_2 = listComunicazioniC1_2.ToArray();
            listComunicazioniC3 = listComunicazioniC3.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoComunicazioneCampo3 = listComunicazioniC3.ToArray();

            listComunicazioniC4 = listComunicazioniC4.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoComunicazioneCampo4 = listComunicazioniC4.ToArray();

            listGradoInvalidita = listGradoInvalidita.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoGradoInvalidita = listGradoInvalidita.ToArray();
            listProrataEnel = listProrataEnel.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoProrataEnel = listProrataEnel.ToArray();

            listCodiceAzienda = listCodiceAzienda.OrderBy(x => x.Descrizione).ToList();
            rispostaDecodifica.ElencoCodiceAzienda = listCodiceAzienda.ToArray();

            listCategoriaPensione = listCategoriaPensione.OrderBy(x => x.Sigla).ToList();
            rispostaDecodifica.ElencoCategoriePensione = listCategoriaPensione.ToArray();

            listFondiSpeciali = listFondiSpeciali.OrderBy(x => x.DescFondo).ToList();
            rispostaDecodifica.ElencoFondiPensione = listFondiSpeciali.ToArray();

            listStatoPensione = listStatoPensione.OrderBy(x => x.DecodificaStato).ToList();
            rispostaDecodifica.ElencoStatiPensione = listStatoPensione.ToArray();
            listCodiciNatura = listCodiciNatura.OrderBy(x => x.TraduzioneSuGP.GetValueOrDefault()).ToList();
            rispostaDecodifica.ElencoCodiciNatura = listCodiciNatura.ToArray();
        }

        public static string LoadSedi()
        {
            string retString = string.Empty;
            string sedi = string.Empty;
            List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediProvinciali();
            foreach (INPS.DNA.Office office in listOffice)
            {
                sedi = string.Concat(office.AspnCode.PadLeft(4, '0').Substring(0, 4), "-", (office.ExtendedProperties != null ? office.ExtendedProperties["SEDE"].Trim() : office.Name.Trim()), ";");
                retString = string.Concat(retString, sedi);
            }
            return retString;
        }

        public static short ControlSede(string sedeInserita)
        {
            string[] resultCriterio = { "", "" };
            string sedeToSplit = string.Empty;
            string[] sede = null;
            short codSede = 0;

            sedeToSplit = sedeInserita;
            sede = sedeToSplit.Split('-');
            resultCriterio[0] = sede[0];
            if (resultCriterio[0].Length >= 6)
                Int16.TryParse(resultCriterio[0].Substring(0, 4), out codSede);
            else
                Int16.TryParse(resultCriterio[0], out codSede);

            if (codSede == 0)
            {
                List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediProvinciali();
                foreach (INPS.DNA.Office office in listOffice)
                {
                    if ((office.ExtendedProperties != null ? office.ExtendedProperties["SEDE"].Trim() : office.Name.Trim()) == sedeInserita.ToUpperInvariant().Trim())
                        codSede = short.Parse(office.AspnCode.PadLeft(4, '0').Substring(0, 4));
                }
            }

            return codSede;
        }

        public static byte ControlCentroOperativo(string sedeInserita)
        {
            string[] resultCriterio = { "", "" };
            string sedeToSplit = string.Empty;
            string[] sede = null;
            byte codCentroOperativo = 0;

            sedeToSplit = sedeInserita;
            sede = sedeToSplit.Split('-');
            resultCriterio[0] = sede[0];
            if (resultCriterio[0].Length >= 6)
                byte.TryParse(resultCriterio[0].Substring(4, 2), out codCentroOperativo);

            if (codCentroOperativo == 0)
            {
                List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediProvinciali();
                foreach (INPS.DNA.Office office in listOffice)
                {
                    if ((office.ExtendedProperties != null ? office.ExtendedProperties["SEDE"].Trim() : office.Name.Trim()) == sedeInserita.ToUpperInvariant().Trim())
                        codCentroOperativo = byte.Parse(office.AspnCode.PadLeft(6, '0').Substring(4, 2));
                }
            }

            return codCentroOperativo;
        }

        public static string GetSede(string codSede)
        {
            string sede = string.Empty;
            if (codSede == "9933")
                sede = "9933-ENPALS";
            else if (codSede == "7005")
                sede = "7005-ROMA CENTRO";
            else
            {
                List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediProvinciali();
                INPS.DNA.Office officeName = null;
                officeName = listOffice.Find(delegate(INPS.DNA.Office code)
                { return (code.AspnCode.PadLeft(4, '0').Substring(0, 4) == codSede); });

                if (officeName == null)
                    return string.Empty;

                sede = string.Concat(codSede, "-", (officeName.ExtendedProperties != null ? officeName.ExtendedProperties["SEDE"].Trim() : officeName.Name.Trim()));
            }
            return sede;
        }

        public static string GetSedeDa6(string codSede)
        {
            string sede = string.Empty;
            List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediECoProvinciali();
            INPS.DNA.Office officeName = null;
            officeName = listOffice.Find(delegate(INPS.DNA.Office code)
            { return (code.AspnCode.PadLeft(6, '0') == codSede); });

            if (officeName == null)
                return string.Empty;

            sede = string.Concat(codSede, "-", (officeName.ExtendedProperties != null ? officeName.ExtendedProperties["SEDE"].Trim() : officeName.Name.Trim()));
            return sede;
        }

        public static List<string> GetSediConRegioni()
        {
            List<string> listaSedi = null;
            List<INPS.DNA.Office> listOffice = CodeUtility.GetListaSediECoProvinciali();
            if (listOffice != null && listOffice.Count > 0)
            {
                listOffice = listOffice.OrderBy(x => x.AspnCode).ToList();

                listaSedi = new List<string>();
                foreach (INPS.DNA.Office office in listOffice)
                {
                    listaSedi.Add(office.AspnCode.Trim() + "-" + (office.ExtendedProperties != null ? office.ExtendedProperties["REGIONE"].Trim() : office.RegionName.Trim()));
                }
            }
            return listaSedi;
        }

        public static void ClearForm(Control Controlli, int editIndexGrid)
        {
            if (Controlli.Controls.Count > 0)
            {
                foreach (Control ctrl in Controlli.Controls)
                {
                    ClearForm(ctrl, editIndexGrid);

                    switch (ctrl.GetType().Name)
                    {
                        case "TextBox":
                            TextBox txt = ctrl as TextBox;
                            txt.Text = string.Empty;
                            break;
                        case "CheckBox":
                            CheckBox chk = ctrl as CheckBox;
                            chk.Checked = false;
                            break;
                        case "RadioButton":
                            RadioButton rdb = ctrl as RadioButton;
                            rdb.Checked = false;
                            break;
                        case "DropDownList":
                            DropDownList ddl = ctrl as DropDownList;
                            ddl.ClearSelection();
                            break;
                        case "GridView":
                            GridView grid = ctrl as GridView;
                            grid.DataSource = null;
                            grid.DataBind();
                            grid.EditIndex = editIndexGrid;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void SetItemBlankDdl(DropDownList ddl)
        {
            ListItem li1 = new ListItem();
            li1.Text = string.Empty;
            li1.Value = string.Empty;
            ddl.Items.Add(li1);
        }

        internal static void SetValueDdl(DropDownList ddl, string testo, string descrizione, string id)
        {
            ListItem li1 = new ListItem();
            li1.Attributes.Add("title", descrizione);
            li1.Text = testo;
            li1.Value = id;
            ddl.Items.Add(li1);
        }

        internal static void SetValueDdl(DropDownList ddl, string descrizione, string id)
        {
            ListItem li1 = new ListItem();
            li1.Attributes.Add("title", descrizione);
            li1.Text = descrizione;
            li1.Value = id;
            ddl.Items.Add(li1);
        }

        internal static void EnableEditableMode(TableCell cell_CancelSave, string ValidationGroup, string Theme, bool isCommandName = true)
        {
            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Theme + "/Images/cancel24.png " + (isCommandName ? string.Empty : "title=Annulla ") + "/>";
            if (isCommandName)
            {
                cancel.ToolTip = "Annulla";
                cancel.CommandName = "Annulla";
            }

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Theme + "/Images/save24.png " + (isCommandName ? string.Empty : "title=Salva ") + "/>";
            if (isCommandName)
            {
                save.ToolTip = "Salva";
                save.CommandName = "Salva";
            }
            save.CausesValidation = true;
            save.ValidationGroup = ValidationGroup;
        }

        internal static void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete, string Theme, string IdButton)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";

            if (!String.IsNullOrEmpty(IdButton))
            {
                LinkButton delete = (LinkButton)(cell_Delete.FindControl(IdButton));
                delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Theme + "/Images/delete24.png />";
            }
        }

        internal static void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete, string Theme, bool isEditVisible, bool IsDeleteVisible)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            edit.Visible = isEditVisible;

            LinkButton delete = (LinkButton)(cell_Delete.Controls[0]);
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Theme + "/Images/delete24.png />";
            delete.ToolTip = "Elimina";
            delete.Visible = IsDeleteVisible;
        }

        public static bool IsDomandaSperimentaleDonna(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            return (tipologiaProdottoPensione == TipologiaPensioneProdotto.pr_Anzianita && (datiPensione.CodeTipo == "0050" || datiPensione.CodeTipo == "0176")) ? true : false;
        }

        public static bool IsDomandaVittimeTerrorismo(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            return (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80) ? true : false;
        }

        public static bool IsRicostituzioneCumuloProgressiva(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            return tipologiaTipoPensione == TipologiaPensioneTipo.tp_Ricostituzione_Cumulo_Progressiva && Utility.IsDomandaVOCUM(siglaCategoria);
        }

        public static bool IsRicostituzioneVariazioneDatiContitolari(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            return tipologiaTipoPensione == TipologiaPensioneTipo.tp_RicostituzioneVariazioneDatiContitolari;
        }

        public static bool IsTipoContributivoConOpzione(AreaTitolare.DatiPensione datiPensione, bool? isOpzione = null)
        {
            if (isOpzione.GetValueOrDefault())
                return true;
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            return (tipologiaTipoPensione == TipologiaPensioneTipo.tp_Anzianita_TipoContributivoConOpzione ||
                    tipologiaTipoPensione == TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoConOpzione);
        }

        public static bool IsDomandaAUT(string siglaCategoria)
        {
            if (string.IsNullOrEmpty(siglaCategoria))
                return false;

            if (siglaCategoria.Trim().ToUpperInvariant() == "VOAUT" ||
                siglaCategoria.Trim().ToUpperInvariant() == "IOAUT" ||
                siglaCategoria.Trim().ToUpperInvariant() == "SOAUT")
                return true;

            return false;
        }

        internal static void GetTipologiaPensione(string gruppo, string prodotto, string tipo, out TipologiaPensioneGruppo gruppoPensione, out TipologiaPensioneProdotto prodottoPensione, out TipologiaPensioneTipo tipoPensione)
        {
            gruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            prodottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            tipoPensione = TipologiaPensioneTipo.tp_NessunValore;

            if (!String.IsNullOrEmpty(gruppo))
                gruppoPensione = GetTipologiaPensioneGruppo(gruppo);

            if (!String.IsNullOrEmpty(prodotto))
                prodottoPensione = GetTipologiaPensioneProdotto(gruppoPensione, prodotto);

            if (!String.IsNullOrEmpty(tipo))
                tipoPensione = GetTipologiaPensioneTipo(prodottoPensione, tipo);
        }

        private static TipologiaPensioneGruppo GetTipologiaPensioneGruppo(string gruppo)
        {
            TipologiaPensioneGruppo tipologiaPensione = TipologiaPensioneGruppo.gr_NessunValore;

            switch (gruppo)
            {
                case "0001":
                    tipologiaPensione = TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia;
                    break;
                case "0002":
                    tipologiaPensione = TipologiaPensioneGruppo.gr_Inabilita_Invalidita;
                    break;
                case "0003":
                    tipologiaPensione = TipologiaPensioneGruppo.gr_Superstiti;
                    break;
                case "0031":
                    tipologiaPensione = TipologiaPensioneGruppo.gr_Ricostituzione;
                    break;
                case "0051":
                    tipologiaPensione = TipologiaPensioneGruppo.gr_Ripristini_Riliquidazioni;
                    break;
            }

            return tipologiaPensione;
        }

        private static TipologiaPensioneProdotto GetTipologiaPensioneProdotto(TipologiaPensioneGruppo tipologiaPensioneGruppo, string prodotto)
        {
            TipologiaPensioneProdotto tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_NessunValore;

            switch (tipologiaPensioneGruppo)
            {
                case TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia:

                    switch (prodotto)
                    {
                        case "0001":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Anzianita;
                            break;
                        case "0002":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Vecchiaia;
                            break;
                    }

                    break;
                case TipologiaPensioneGruppo.gr_Inabilita_Invalidita:
                    switch (prodotto)
                    {
                        case "0011":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_InvaliditaAssegno;
                            break;
                        case "0012":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_InabilitaPensione;
                            break;
                        case "0013":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_InvaliditaPensione;
                            break;
                    }
                    break;
                case TipologiaPensioneGruppo.gr_Ricostituzione:
                    switch (prodotto)
                    {
                        case "0110":
                        case "0310":
                        case "0410":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_VariazioneDecorrenza;
                            break;
                        case "0107":
                        case "0307":
                        case "0407":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_MotiviContributivi;
                            break;
                        case "0413":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_VariazioneDatiContitolari;
                            break;
                        case "0102":
                        case "0302":
                        case "0402":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Supplemento;
                            break;
                    }
                    break;
                case TipologiaPensioneGruppo.gr_Superstiti:
                    switch (prodotto)
                    {
                        case "0021":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Reversibilita;
                            break;
                        case "0022":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Indiretta;
                            break;
                    }
                    break;
                case TipologiaPensioneGruppo.gr_Ripristini_Riliquidazioni:
                    switch (prodotto)
                    {
                        case "0121":
                        case "0321":
                        case "0421":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Ripristino;
                            break;
                        case "0122":
                        case "0322":
                        case "0422":
                            tipologiaPensioneProdotto = TipologiaPensioneProdotto.pr_Riliquidazione;
                            break;
                    }
                    break;
            }
            return tipologiaPensioneProdotto;
        }

        private static TipologiaPensioneTipo GetTipologiaPensioneTipo(TipologiaPensioneProdotto prodottoPensione, string tipo)
        {
            TipologiaPensioneTipo tipologiaPensioneTipo = TipologiaPensioneTipo.tp_NessunValore;

            switch (prodottoPensione)
            {
                case TipologiaPensioneProdotto.pr_Anzianita:
                    switch (tipo)
                    {
                        case "0051":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Precoci;
                            break;
                        case "0158":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80;
                            break;
                        case "0159":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80;
                            break;
                        case "0030":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Anzianita_TipoContributivoConOpzione;
                            break;
                        case "0017":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Anzianita_TipoContributivoPuro;
                            break;
                        case "0045":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Anzianita_InComputo;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_Vecchiaia:
                    switch (tipo)
                    {
                        case "0002":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI;
                            break;
                        case "0009":
                        case "0192":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_Supplementare;
                            break;
                        case "0159":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80;
                            break;
                        case "0030":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoConOpzione;
                            break;
                        case "0017":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoPuro;
                            break;
                        case "0045":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_InComputo;
                            break;
                        case "0173":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Vecchiaia_GravosiUsuranti;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_InvaliditaPensione:
                    switch (tipo)
                    {
                        case "0001":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Invalidita_Ordinaria;
                            break;
                        case "0009":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Invalidita_Supplementare;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_Reversibilita:
                    switch (tipo)
                    {
                        case "0009":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Reversibilita_Supplementare;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_Indiretta:
                    switch (tipo)
                    {
                        case "0009":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Indiretta_Supplementare;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_InvaliditaAssegno: // Prodotto 0011
                    switch (tipo)
                    {
                        case "0001":
                        case "0045":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_InvaliditaAssegno_Ordinario;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_InabilitaPensione:
                    switch (tipo)
                    {
                        case "0168":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Inabilita_Art1_C250_Legge232;
                            break;
                        case "0001":
                        case "0045":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Inabilita_Ordinaria;
                            break;
                        case "0052":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_MotiviContributivi:
                    switch (tipo)
                    {
                        case "0169":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_RicostituzioneContributivaPerEsecuzioneSentenza;
                            break;
                        case "0177":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_Ricostituzione_Cumulo_Progressiva;
                            break;
                    }
                    break;
                case TipologiaPensioneProdotto.pr_VariazioneDatiContitolari:
                    switch (tipo)
                    {
                        case "0001":
                            tipologiaPensioneTipo = TipologiaPensioneTipo.tp_RicostituzioneVariazioneDatiContitolari;
                            break;
                    }
                    break;
            }

            return tipologiaPensioneTipo;
        }

        public static string ConvertDecimalFixedPoint(string input, int cifreDecimali)
        {
            decimal d = 0M;
            decimal.TryParse(input, out d);
            return d.ToString("F" + cifreDecimali.ToString());
        }

        public static void AggiornaSemafori(IQuadriSemafori quadro, Control currentPage, UCInfo ucInfo)
        {
            if (quadro.areaInfoPratica == null)
                throw new DNA.DnaApplicationException("Manca la valorizzazione dell'area info pratica");

            AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)System.Web.HttpContext.Current.Session["Domanda"];

            quadro.areaInfoPratica.AreaQuadri = (AreaQuadri)System.Web.HttpContext.Current.Session["Semaforo"];
            quadro.areaInfoPratica.MatricolaOperatore = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            quadro.areaInfoPratica.SedeOperatore = short.Parse(INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.PadLeft(4, '0').Substring(0, 4));

            PresenterMenuLeft presenter = new PresenterMenuLeft();

            try
            {
                presenter.GetQuadri(quadro);
            }
            catch (Exception Ex)
            {
                ScriptManager.RegisterClientScriptBlock(currentPage, currentPage.GetType(), "alert", "alert('" + Ex.Message.Replace("'", " ") + "');", true);
            }

            datiDomanda.Stato = quadro.areaInfoPratica != null ? quadro.areaInfoPratica.StatoPensione : string.Empty;
            datiDomanda.MatricolaUtenteAcquisizione = quadro.areaInfoPratica != null ? quadro.areaInfoPratica.MatricolaUtenteAcquisizione : string.Empty;
            datiDomanda.IsMatchMatricola = quadro.areaInfoPratica != null ? quadro.areaInfoPratica.IsMatchMatricola : false;
            datiDomanda.IsCalcoloAbilitato = quadro.areaInfoPratica != null ? quadro.areaInfoPratica.IsCalcoloAbilitato : false;

            System.Web.HttpContext.Current.Session["Domanda"] = datiDomanda;

            System.Web.HttpContext.Current.Session["Semaforo"] = quadro.areaInfoPratica != null ? quadro.areaInfoPratica.AreaQuadri : null;

            if (ucInfo != null)
            {
                CustomBasePage c = new CustomBasePage();
                c.ValorizzaInfoLiquidazione(ucInfo);
            }
        }

        public static bool IsTabPrepensionamentoVisible(int? attivitaEconomica, int? professioneIndividuale, string naturaPensione)
        {
            if ((attivitaEconomica.GetValueOrDefault() == 92 && professioneIndividuale.GetValueOrDefault() == 257) ||
                (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 326) ||
                (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 350) ||
                (attivitaEconomica.GetValueOrDefault() == 4 && professioneIndividuale.GetValueOrDefault() == 350) ||
                (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(2, 1).Equals("O")))
                return true;

            return false;
        }

        public static T? GetValueFromDescription<T>(string description) where T : struct
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return null;
        }

        public static bool ChangeSede(Ruoli ruolo, string sedeDomanda, bool isSedeChiusaStessaProvinciaOperatore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (CodeUtility.GetRuolo(ruolo) == UtilityRuolo.AMMINISTRATORE)
            {
                try
                {
                    INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.OfficeFullList.FirstOrDefault(x => x.AspnCode == sedeDomanda);

                    if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice == null)
                    {
                        messaggioVideo = "Sede della domanda non valida.";
                        return false;
                    }
                }
                catch
                {
                    messaggioVideo = "Sede della domanda non valida.";
                    return false;
                }
            }
            else
            {
                PresenterSedi presenter = new PresenterSedi();
                List<string> sediAbilitate = presenter.GetOfficeAspnCodeAbilitati(INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(ruolo.ToString()).ToList<string>());

                if (sediAbilitate.Contains(sedeDomanda) || isSedeChiusaStessaProvinciaOperatore)
                {
                    try
                    {
                        INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.OfficeFullList.FirstOrDefault(x => x.AspnCode == sedeDomanda);

                        if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice == null)
                        {
                            messaggioVideo = "Sede della domanda non valida.";
                            return false;
                        }
                    }
                    catch
                    {
                        messaggioVideo = "Sede della domanda non valida.";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Utente non abilitato sulla sede selezionata.";
                    return false;
                }
            }

            return true;
        }

        //ENG - per le pensioni della nuova opzione donna (tipo 0190) il secondo byte del codice natura "O" deve essere sempre selezionato e bloccato
        public static void DisableCodNatura2PerSperDonna(DropDownList ddlCodNatura2, bool isCodiceNatura2DisabledPerSperDonna)
        {
            if (ddlCodNatura2.SelectedValue == "O" || isCodiceNatura2DisabledPerSperDonna)
            {
                ddlCodNatura2.SelectedValue = "O";
                ddlCodNatura2.Enabled = false;
            }
        }

        public static void DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(DropDownList ddlCodNatura2, bool isCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292)
        {
            if (isCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292)
            {
                ddlCodNatura2.SelectedValue = "O";
                ddlCodNatura2.Enabled = false;
            }
        }

        public static void DisableEliminaForRicostituzioni(Button buttonElimina)
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)System.Web.HttpContext.Current.Session["DatiPensione"];
            AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)System.Web.HttpContext.Current.Session["Domanda"];

            if (datiPensione == null || datiDomanda == null)
                return;

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || datiDomanda.IsDomandaRiapertura)
                buttonElimina.Enabled = false;
        }

        public static void DisableEliminaForRipristini(Button buttonElimina)
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)HttpContext.Current.Session["DatiPensione"];
            AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)HttpContext.Current.Session["Domanda"];

            if (datiPensione == null || datiDomanda == null)
                return;

            TipologiaPensioneGruppo tipologiaGruppoPensione;
            TipologiaPensioneProdotto tipologiaProdottoPensione;
            TipologiaPensioneTipo tipologiaTipoPensione;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ripristini_Riliquidazioni && tipologiaProdottoPensione == TipologiaPensioneProdotto.pr_Ripristino)
                buttonElimina.Enabled = false;
        }

        public static string GetPrevalDecForExCombattente_Maggiorazione()
        {
            return string.Empty;
            //Rimossa prevalorizzazione doc -> SF_IVS_Funzionalità IVS@20211129_v7.5
            string sDecorrenza = "MM/AAAA";
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (datiPensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione)
            {
                sDecorrenza = String.Format("{0:MM/yyyy}", datiPensione.DataPresentazioneDomanda.Value.AddMonths(1));
            }
            return sDecorrenza;
        }

        public static int? StringToNullableInt(string value)
        {
            try
            {
                int output = 0;
                if (Int32.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Int64? StringToNullableInt64(string value)
        {
            try
            {
                Int64 output = 0;
                if (Int64.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static long? StringToNullableLong(string value)
        {
            try
            {
                long output = 0;
                if (long.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static short? StringToNullableShort(string value)
        {
            try
            {
                short output = 0;
                if (short.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static char? StringToNullableChar(string value)
        {
            try
            {
                if (value != null && value.Length >= 1)
                {
                    //Gestione LOW VALUE \0
                    if (((char?)((value.ToCharArray(0, 1)))[0]).Value != '\0')
                        return (char?)((value.ToCharArray(0, 1)))[0];
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte? StringToNullableByte(string value)
        {
            try
            {
                return byte.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool? StringToNullableBool(string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static decimal? StringToNullableDecimal(string value)
        {
            try
            {
                return decimal.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static short? ShortToNullableShort(short value)
        {
            //per value = 0 il valore ritornato è null
            if (value == 0)
                return (short?)null;
            else
                return value;
        }

        public static char? GetCharCategoriaFondoPI(UtilityCategoriaFondoPI? categoriaFondoPI)
        {
            if (categoriaFondoPI.HasValue)
            {
                switch (categoriaFondoPI.Value)
                {
                    case UtilityCategoriaFondoPI.A: return 'A';
                    case UtilityCategoriaFondoPI.B: return 'B';
                    case UtilityCategoriaFondoPI.C: return 'C';
                    case UtilityCategoriaFondoPI.D: return 'D';
                    case UtilityCategoriaFondoPI.E: return 'E';
                    case UtilityCategoriaFondoPI.F: return 'F';
                    case UtilityCategoriaFondoPI.G: return 'G';
                    case UtilityCategoriaFondoPI.H: return 'H';
                    case UtilityCategoriaFondoPI.I: return 'I';
                    case UtilityCategoriaFondoPI.J: return 'J';
                    case UtilityCategoriaFondoPI.L: return 'L';
                    case UtilityCategoriaFondoPI.M: return 'M';
                    case UtilityCategoriaFondoPI.N: return 'N';
                    case UtilityCategoriaFondoPI.O: return 'O';
                    case UtilityCategoriaFondoPI.P: return 'P';
                    case UtilityCategoriaFondoPI.Q: return 'Q';
                    case UtilityCategoriaFondoPI.R: return 'R';
                    case UtilityCategoriaFondoPI.S: return 'S';
                    case UtilityCategoriaFondoPI.T: return 'T';
                    case UtilityCategoriaFondoPI.U: return 'U';
                    case UtilityCategoriaFondoPI.V: return 'V';
                    case UtilityCategoriaFondoPI.W: return 'W';
                    case UtilityCategoriaFondoPI.X: return 'X';
                    case UtilityCategoriaFondoPI.Y: return 'Y';
                    case UtilityCategoriaFondoPI.Z: return 'Z';
                    case UtilityCategoriaFondoPI.Uno: return '1';
                    default:
                        return null;
                }
            }

            return null;
        }

        public static bool IsGridViewInEditPresent(Control Controlli)
        {
            bool ret = false;
            if (Controlli.Controls.Count > 0)
            {
                foreach (Control ctrl in Controlli.Controls)
                {
                    if (IsGridViewInEditPresent(ctrl))
                    {
                        ret = true;
                        break;
                    }

                    switch (ctrl.GetType().Name)
                    {
                        case "GridView":
                            GridView grid = ctrl as GridView;
                            if (grid.EditIndex != -1)
                            {
                                ret = true;
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return ret;
        }

        public static string GetSI_NO(bool? value)
        {
            return value != null && value.GetValueOrDefault() ? "SI" : "NO";
        }

        public static bool? GetBoolFromSI_NO(string value)
        {
            if (value == "SI")
                return true;
            if (value == "NO")
                return false;

            return null;
        }

        /// <summary>
        /// Verifica se almeno un controllo è visibile
        /// Al momento sono implementati soltanto Label, TextBox e DropDownList
        /// </summary>
        /// <param name="Controlli"></param>
        /// <returns></returns>
        public static bool IsContentVisible(Control Controlli)
        {
            bool ret = false;
            if (Controlli.Controls.Count > 0)
            {
                foreach (Control ctrl in Controlli.Controls)
                {
                    if (IsContentVisible(ctrl))
                    {
                        ret = true;
                        break;
                    }

                    switch (ctrl.GetType().Name)
                    {
                        case "Label":
                            Label lbl = ctrl as Label;
                            if (lbl.Visible)
                            {
                                ret = true;
                                break;
                            }
                            break;
                        case "TextBox":
                            TextBox txt = ctrl as TextBox;
                            if (txt.Visible)
                            {
                                ret = true;
                                break;
                            }
                            break;
                        case "DropDownList":
                            DropDownList ddl = ctrl as DropDownList;
                            if (ddl.Visible)
                            {
                                ret = true;
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return ret;
        }

        public static bool IsRicostituzioneOrRiaperturaAGOAutomaticaAbilitata(AreaTitolare.DatiPensione datiPensione, bool isRiaperturaDomanda, string siglaCategoria)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda, siglaCategoria))
                return true;

            return false;
        }

        public static bool IsRicostituzioneOrRiaperturaAGOAbilitata(AreaTitolare.DatiPensione datiPensione, bool isRiaperturaDomanda, string siglaCategoria)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            List<string> sigleCategorieAmmesse = new List<string>() { "VO", "IO", "VR", "IR", "VOART", "IOART", "VOCOM", "IOCOM", "VOAUT", "IOAUT", "VDAI", "IDAI", "SO", "SR", "SOART", "SOCOM", "SOAUT", "SDAI" };

            if ((isRiaperturaDomanda || tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione) && !string.IsNullOrEmpty(siglaCategoria) && sigleCategorieAmmesse.Contains(siglaCategoria.Trim()))
                return true;

            return false;
        }

        public static bool IsRicostituzioneOrRiapertura(AreaTitolare.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (isRiaperturaDomanda || tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione)
                return true;

            return false;
        }

        public static bool IsRicostituzione(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione)
                return true;

            return false;
        }

        public static bool IsRicostituzioneNonContributiva(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione && tipologiaProdottoPensione != TipologiaPensioneProdotto.pr_MotiviContributivi)
                return true;

            return false;
        }

        public static bool IsRicostituzioneContributiva(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione && tipologiaProdottoPensione == TipologiaPensioneProdotto.pr_MotiviContributivi)
                return true;

            return false;
        }

        public static bool IsRicostituzioneSupplemento(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione && tipologiaProdottoPensione == TipologiaPensioneProdotto.pr_Supplemento)
                return true;

            return false;
        }

        public static bool IsRicostituzioneContributivaPerEsecuzioneSentenza(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo tipologiaGruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto tipologiaProdottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipologiaTipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == TipologiaPensioneGruppo.gr_Ricostituzione && tipologiaProdottoPensione == TipologiaPensioneProdotto.pr_MotiviContributivi &&
                tipologiaTipoPensione == TipologiaPensioneTipo.tp_RicostituzioneContributivaPerEsecuzioneSentenza)
                return true;

            return false;
        }

        public static bool IsDomandaSuperstitiOrRicostituzione(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.StartsWith("S"))
                return true;
            return false;
        }

        public static bool IsEnpalsManualePL(bool isDomandaENPALS, bool isRicostituzioneOrRiapertura, bool? isDatiENPALSRecuperati)
        {
            if (isDomandaENPALS && !isRicostituzioneOrRiapertura && isDatiENPALSRecuperati.HasValue && !isDatiENPALSRecuperati.Value)
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneAOI(AreaTitolare.DatiPensione datiPensione)
        {

            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0322" && datiPensione.CodeTipo == "0023")
                return true;

            return false;
        }

        public static void AddClass(HtmlGenericControl control, string cssClass)
        {
            if (string.IsNullOrEmpty(control.Attributes["class"]))
                control.Attributes["class"] = string.Empty;
            control.Attributes["class"] = string.Format("{0} {1}", control.Attributes["class"].Trim(), cssClass);
        }

        public static string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
        }

        public static string GetStatoDomandaANF(string codiceFiscale, INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf datiDomanda, bool isDettaglioPratica)
        {
            string stato = "NON TROVATO";

            if (datiDomanda != null)
            {
                if (datiDomanda.statoDomanda.HasValue)
                {
                    switch (datiDomanda.statoDomanda)
                    {
                        case 1:
                            stato = "PRE-LAVORAZIONE";
                            break;
                        case 2:
                            stato = "IN LAVORAZIONE";
                            break;
                        case 3:
                            stato = "ACCOLTA";
                            break;
                        case 4:
                            if (datiDomanda.respinto == "1")
                                stato = "RESPINTA";
                            else
                                stato = "ACCOLTA";
                            break;
                        case 5:
                            stato = "RESPINTA";
                            break;
                        default:
                            break;
                    }
                }
            }

            if (isDettaglioPratica)
            {
                switch (datiDomanda.codicePratica2)
                {
                    case "1":
                        stato += " con domanda di PRIMA ISTANZA";
                        break;
                    case "3":
                        stato += " con domanda di RIESAME";
                        break;
                    case "4":
                        stato += " con domanda di RICORSO";
                        break;
                }
            }
            return stato;
        }
        #endregion Trasversali

        #region non chiamate

        public string DecodificaStatoCivile(string statoCorrente)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            areaDecodifica.GetValuesDecodifica();
            Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoCivile[] listStatoCivile = areaDecodifica.GetValuesDecodifica().ElencoStatiCivili;
            foreach (AreaDecodifica.DatiStatoCivile statoCivile in listStatoCivile)
            {
                if (statoCorrente == statoCivile.Descrizione)
                    return statoCivile.Id.ToString();
            }
            return null;
        }

        public static AreaDecodifica.DatiStatoCivile GetDescrizioneStatoCivile(string codice)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoCivile[] listStatoCivile = areaDecodifica.GetValuesDecodifica().ElencoStatiCivili;
            AreaDecodifica.DatiStatoCivile reDec = listStatoCivile.ToList().Find(
                delegate(AreaDecodifica.DatiStatoCivile statoCivile)
                {
                    return statoCivile.Id == codice[0];
                }
                );
            return reDec;

        }

        #endregion non chiamate

        #region Gestione FORM
        internal static void BloccaForm(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, Control Controlli)
        {
            if (Controlli.Controls.Count > 0)
            {
                foreach (Control ctrl in Controlli.Controls)
                {
                    BloccaForm(datiDomanda, ctrl);

                    switch (ctrl.GetType().Name)
                    {
                        case "Panel":
                            Panel pan = ctrl as Panel;
                            String sPanID = pan.ID.ToUpperInvariant();
                            pan.Enabled = false;
                            BloccaRicercaDelegatoTutore(ref pan);
                            SbloccaAreaSede(ref pan);
                            SbloccaAreaRuolo(ref pan);
                            SbloccaGestioneCalcoloNoInd(ref pan);
                            BypassaAggiornaStazLavoro(datiDomanda, ref pan);
                            BypassaAggiornaWebDom(datiDomanda, ref pan);
                            BypassaAggiornaFelpe(datiDomanda, ref pan);
                            BypassaAggiornaOneri(datiDomanda, ref pan);
                            BypassaAggiornaSai(datiDomanda, ref pan);
                            BypassaAggiornaINPDAP(datiDomanda, ref pan);
                            BypassaAggiornaTotal(datiDomanda, ref pan);
                            BypassaAggiornaTot(datiDomanda, ref pan);
                            BypassaAggiornaNoteDebito(datiDomanda, ref pan);
                            BypassaPresaInCarico(datiDomanda, ref pan);
                            BypassaPulsantiIntestazione(ref pan);
                            BypassaPulsantiConsultazione(ref pan);
                            BypassaAggiornaNoteDebito(datiDomanda, ref pan);
                            BypassaAggiornaPianiDiPagamento(datiDomanda, ref pan);
                            BypassaAggiornaEquoInd(datiDomanda, ref pan);
                            BypassaAggiornaIndennSpec(datiDomanda, ref pan);
                            break;
                        case "TextBox":
                            TextBox txt = ctrl as TextBox;
                            txt.CssClass = txt.CssClass.Replace("date-picker-maxActual", "").
                                Replace("date-picker-base-maxActual", "").
                                Replace("date-picker-base", "").
                                Replace("date-picker-year", "").
                                Replace("date-picker", "");
                            txt.Enabled = false;
                            BypassaPulsantiIntestazione(ref txt);
                            break;
                        case "CheckBox":
                            CheckBox chk = ctrl as CheckBox;
                            chk.Enabled = false;
                            break;
                        case "RadioButton":
                            RadioButton rdb = ctrl as RadioButton;
                            rdb.Enabled = false;
                            break;
                        case "Button":
                            Button cmd = ctrl as Button;
                            cmd.Enabled = false;
                            BypassaBottoniRicerca(ref cmd);
                            BypassaAggiornaStazLavoro(datiDomanda, ref cmd);
                            BypassaAggiornaWebDom(datiDomanda, ref cmd);
                            BypassaAggiornaFelpe(datiDomanda, ref cmd);
                            BypassaAggiornaOneri(datiDomanda, ref cmd);
                            BypassaAggiornaSai(datiDomanda, ref cmd);
                            BypassaAggiornaINPDAP(datiDomanda, ref cmd);
                            BypassaAggiornaTotal(datiDomanda, ref cmd);
                            BypassaAggiornaTot(datiDomanda, ref cmd);
                            BypassaAggiornaNoteDebito(datiDomanda, ref cmd);
                            BypassaPresaInCarico(datiDomanda, ref cmd);
                            BypassaPulsantiIntestazione(ref cmd);
                            BypassaPulsantiConsultazione(ref cmd);
                            BypassaAggiornaNoteDebito(datiDomanda, ref cmd);
                            BypassaAggiornaPianiDiPagamento(datiDomanda, ref cmd);
                            BypassaAggiornaEquoInd(datiDomanda, ref cmd);
                            BypassaAggiornaIndennSpec(datiDomanda, ref cmd);
                            break;
                        case "DropDownList":
                            DropDownList ddl = ctrl as DropDownList;
                            ddl.Enabled = false;
                            BypassaPulsantiIntestazione(ref ddl);
                            break;
                        case "LinkButton":
                            LinkButton lnk = ctrl as LinkButton;
                            lnk.Enabled = false;
                            break;
                        case "ImageButton":
                            ImageButton img = ctrl as ImageButton;
                            img.Enabled = false;
                            BypassaPulsantiIntestazione(ref img);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal static bool IsConsultazione(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, object ruolo)
        {
            bool esito = false;
            if (datiDomanda != null)
            {
                CodeUtility.StatoPensione? stato = CodeUtility.GetValueFromDescription<CodeUtility.StatoPensione>(datiDomanda.Stato);
                switch (stato)
                {
                    case CodeUtility.StatoPensione.CalcoloNoInd:
                    case CodeUtility.StatoPensione.CalcoloNoIndWait:
                    case CodeUtility.StatoPensione.Calcolata:
                    case CodeUtility.StatoPensione.CalcolataNoWebDom:
                    case CodeUtility.StatoPensione.CalcolataNoFelpe:
                    case CodeUtility.StatoPensione.CalcolataNoOneri:
                    case CodeUtility.StatoPensione.CalcolataNoSai:
                    case CodeUtility.StatoPensione.CalcolataNoSin:
                    case CodeUtility.StatoPensione.CalcolataNoTotal:
                    case CodeUtility.StatoPensione.CalcolataNoStazLavoro:
                    case CodeUtility.StatoPensione.CalcolataNoNoteDebito:
                    case CodeUtility.StatoPensione.CalcolataNo6Scatti:
                    case CodeUtility.StatoPensione.CalcolataNoEquoInd:
                    case CodeUtility.StatoPensione.CalcolataNoIndennSpec:
                        esito = true;
                        break;
                    default:
                        if ((!datiDomanda.IsMatchMatricola && datiDomanda.TipoAppartenenza.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO) ||
                            (!datiDomanda.IsMatchMatricola && datiDomanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                             CodeUtility.GetRuolo(ruolo) != UtilityRuolo.AMMINISTRATORE))
                            esito = true;
                        break;
                }
            }
            return esito;
        }

        private static void BloccaRicercaDelegatoTutore(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLDELEGATORICERCA") || panID.Contains("PNLTUTORERICERCA"))
                pan.Visible = false;
        }

        internal static bool ControlloNavigazioneCalcoloNoIndEditabile(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, Page page)
        {
            string paginaChiamante = Path.GetFileName(page.AppRelativeVirtualPath);
            CodeUtility.StatoPensione? stato = CodeUtility.GetValueFromDescription<CodeUtility.StatoPensione>(datiDomanda.Stato);
            if ((stato.Equals(CodeUtility.StatoPensione.CalcoloNoInd) || stato.Equals(CodeUtility.StatoPensione.CalcoloNoIndWait)) && paginaChiamante.Equals("AggiornaCalcoloNoInd.aspx"))
                return true;
            return false;
        }

        private static void SbloccaGestioneCalcoloNoInd(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLTABVALUTAZIONEEVENTUALESCELTA") || 
                panID.Contains("PNLTABELENCOCASUALIDEBITO") ||
                panID.Contains("PNLAGGIORNACALCOLONOIND"))
                pan.Enabled = true;
        }

        private static void SbloccaAreaSede(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLINTESTAZIONE") || panID.Contains("PNLCHGSEDE"))
                pan.Enabled = true;
        }

        private static void SbloccaAreaRuolo(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLRUOLO") || panID.Contains("PNLCHGRUOLO"))
                pan.Enabled = true;
        }

        private static void BypassaBottoniRicerca(ref Button btn)
        {
            string btnID = btn.ID.ToUpperInvariant().Trim();
            if (btnID.Contains("BTNVISUALIZZA") || btnID.Contains("BTNTORNAPOSIZIONI") || btnID.Contains("BTNRICERCA") || btnID.Contains("BTNRISULTATI") ||
                btnID.Contains("BTNTORNARICERCA") || btnID.Contains("BTNTORNARICERCAELENCOCASUALI"))
                btn.Enabled = true;
        }

        private static void BypassaAggiornaStazLavoro(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro))
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNACI05") || panID.Contains("PNLTABAGGCI05"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaStazLavoro(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro))
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGCI05"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaWebDom(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO WEBDOM")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNAWEBDOM") || panID.Contains("PNLTABAGGWEBDOM"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaWebDom(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO WEBDOM")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGWEBDOM"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaFelpe(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO FELPE")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNAFELPE") || panID.Contains("PNLTABAGGFELPE"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaFelpe(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO FELPE")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGFELPE"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaOneri(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO ONERI")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNAONERI") || panID.Contains("PNLTABAGGONERI"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaOneri(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO ONERI")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGONERI"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaSai(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SAI")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNASAI") || panID.Contains("PNLTABAGGSAI"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaSai(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SAI")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGSAI"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaINPDAP(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SIN")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNAINPDAP") || panID.Contains("PNLTABAGGINPDAP"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaINPDAP(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SIN")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGINPDAP"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaTotal(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO TOTAL")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNATOTAL") || panID.Contains("PNLTABAGGTOTAL"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaTotal(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO TOTAL")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGTOTAL"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaTot(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO TOT")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNATOT") || panID.Contains("PNLTABAGGTOT"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaTot(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO TOT")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGTOT"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaNoteDebito(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO NOTE DEBITO")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNANOTEDEBITO") || panID.Contains("PNLTABAGGNOTEDEBITO"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaNoteDebito(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO NOTE DEBITO")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGNOTEDEBITO"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaPianiDiPagamento(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SEI SCATTI")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGIORNAPIANIDIPAGAMENTO") || panID.Contains("PNLTABAGGPIANIDIPAGAMENTO"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaPianiDiPagamento(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO SEI SCATTI")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGPIANIDIPAGAMENTO"))
                        btn.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaEquoInd(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO EQUOIND")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGEQUOIND"))
                        btn.Enabled = true;
                }
            }
        }
        private static void BypassaAggiornaEquoInd(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO EQUOIND")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGEQUOIND") || panID.Contains("PNLTABAGGEQUOIND") || panID.Contains("PNLAGGIORNAEQUOIND"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaAggiornaIndennSpec(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO INDENN SPEC")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNAGGINDENNSPEC"))
                        btn.Enabled = true;
                }
            }
        }
        private static void BypassaAggiornaIndennSpec(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato == "CALCOLO NO INDENN SPEC")
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLAGGINDENNSPEC") || panID.Contains("PNLTABAGGIORNAINDENNSPEC") || panID.Contains("PNLAGGIORNAINDENNSPEC"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaPresaInCarico(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Panel pan)
        {
            if (datiDomanda != null)
            {
                if (!datiDomanda.IsMatchMatricola)
                {
                    string panID = pan.ID.ToUpperInvariant().Trim();
                    if (panID.Contains("PNLPRESAINCARICO"))
                        pan.Enabled = true;
                }
            }
        }

        private static void BypassaPresaInCarico(AreaRispostaRiepilogo.DatiRiepilogoDomanda datiDomanda, ref Button btn)
        {
            if (datiDomanda != null)
            {
                if (datiDomanda.Stato != "CALCOLATA")
                {
                    string btnID = btn.ID.ToUpperInvariant().Trim();
                    if (btnID.Contains("BTNPRESAINCARICO"))
                        btn.Enabled = true;
                    //else if (!(btnID.IndexOf("BTNTORNAARICERCA") < 0))
                    //    btn.Enabled = true;
                }
            }
        }

        private static void BypassaPulsantiIntestazione(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLPULSANTIINTESTAZIONE") || panID.Contains("PNLPROCEDURADPI"))
                pan.Enabled = true;
        }

        private static void BypassaPulsantiIntestazione(ref ImageButton img)
        {
            string imgID = img.ID.ToUpperInvariant().Trim();
            if (imgID.Contains("IMGMANUALE") || imgID.Contains("IMGSEGNALAZIONE") || imgID.Contains("IMGDPI") || imgID.Contains("IMGFAQ"))
                img.Enabled = true;
        }

        private static void BypassaPulsantiIntestazione(ref TextBox txt)
        {
            string txtID = txt.ID.ToUpperInvariant().Trim();
            if (txtID.Contains("TXTOGGETTO") || txtID.Contains("TXTMESSAGGIO") || txtID.Contains("TXTTELEFONO") || txtID.Contains("TXTNUMERODOMUS") ||
                txtID.Contains("TXTCODICEFISCALE") || txtID.Contains("TXTCATEGORIA") || txtID.Contains("TXTSEDE") || txtID.Contains("TXTCERTIFICATO") ||
                txtID.Contains("TXTCODICEERRORE"))
                txt.Enabled = true;
        }

        private static void BypassaPulsantiIntestazione(ref DropDownList ddl)
        {
            string ddlID = ddl.ID.ToUpperInvariant().Trim();
            if (ddlID.Contains("DDLTIPOLOGIA"))
                ddl.Enabled = true;
        }

        private static void BypassaPulsantiIntestazione(ref Button btn)
        {
            string btnID = btn.ID.ToUpperInvariant().Trim();
            if (btnID.Contains("BTNINVIASEGNALAZIONE"))
                btn.Enabled = true;
        }

        private static void BypassaPulsantiConsultazione(ref Button btn)
        {
            string btnID = btn.ID.ToUpperInvariant().Trim();
            if (btnID.Contains("BTNCONSULTA") || btnID.Contains("BTNTORNAELENCOREGISTRAZIONI"))
                btn.Enabled = true;
        }

        private static void BypassaPulsantiConsultazione(ref Panel pan)
        {
            string panID = pan.ID.ToUpperInvariant().Trim();
            if (panID.Contains("PNLREGISTRAZIONIFONDO") || panID.Contains("PNLDATIFONDO"))
                pan.Enabled = true;
        }

        internal static void SetTabIndex(Control Controlli, ref short count)
        {
            if (Controlli.Controls.Count > 0)
            {
                foreach (Control ctrl in Controlli.Controls)
                {
                    SetTabIndex(ctrl, ref count);

                    switch (ctrl.GetType().Name)
                    {
                        case "TextBox":
                            TextBox txt = ctrl as TextBox;
                            txt.TabIndex = count;
                            count++;
                            break;
                        case "CheckBox":
                            CheckBox chk = ctrl as CheckBox;
                            chk.TabIndex = count;
                            count++;
                            break;
                        case "RadioButton":
                            RadioButton rdb = ctrl as RadioButton;
                            rdb.TabIndex = count;
                            count++;
                            break;
                        case "DropDownList":
                            DropDownList ddl = ctrl as DropDownList;
                            ddl.TabIndex = count;
                            count++;
                            break;
                    }
                }
            }
        }

        public static string GetLabelFondoCassa(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (domanda != null && domanda.IsDomandaINPDAP)
                return "Cassa";
            else
                return "Fondo";
        }

        #endregion Gestione FORM

        #region Gestione Profilazione
        internal static Dictionary<string, string> GetRuoliAbilitati()
        {
            Dictionary<string, string> listaRuoli = new Dictionary<string, string>();

            string securityGroupName = INPS.DNA.Context.ApplicationInfo.SecurityGroupName;

            if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName] != null &&
                INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList != null)
            {
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P4677.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P4677.ToString()))
                    listaRuoli.Add(Ruoli.P4677.ToString(), Utility.GetDescription(Ruoli.P4677));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8854.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8854.ToString()))
                    listaRuoli.Add(Ruoli.P8854.ToString(), Utility.GetDescription(Ruoli.P8854));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8855.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8855.ToString()))
                    listaRuoli.Add(Ruoli.P8855.ToString(), Utility.GetDescription(Ruoli.P8855));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P4678.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P4678.ToString()))
                    listaRuoli.Add(Ruoli.P4678.ToString(), Utility.GetDescription(Ruoli.P4678));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8856.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8856.ToString()))
                    listaRuoli.Add(Ruoli.P8856.ToString(), Utility.GetDescription(Ruoli.P8856));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8857.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8857.ToString()))
                    listaRuoli.Add(Ruoli.P8857.ToString(), Utility.GetDescription(Ruoli.P8857));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8974.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8974.ToString()))
                    listaRuoli.Add(Ruoli.P8974.ToString(), Utility.GetDescription(Ruoli.P8974));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8975.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8975.ToString()))
                    listaRuoli.Add(Ruoli.P8975.ToString(), Utility.GetDescription(Ruoli.P8975));
                if (INPS.DNA.Security.DnaPrincipal.Current.Applications[securityGroupName].KeyList.Contains(Ruoli.P8976.ToString()) &&
                    !listaRuoli.ContainsKey(Ruoli.P8976.ToString()))
                    listaRuoli.Add(Ruoli.P8976.ToString(), Utility.GetDescription(Ruoli.P8976));
            }
            if (listaRuoli.Count == 0)
                listaRuoli = null;

            return listaRuoli;
        }

        internal static bool IsMultiRuolo()
        {
            Dictionary<string, string> listaRuoli = GetRuoliAbilitati();
            if (listaRuoli != null && listaRuoli.Count > 1)
                return true;
            return false;
        }

        internal static bool IsAmministratore(object objRuolo)
        {
            if (objRuolo != null)
            {
                Ruoli ruolo = (Ruoli)objRuolo;
                switch (ruolo)
                {
                    case Ruoli.P4677:
                    case Ruoli.P8854:
                    case Ruoli.P8855:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        internal static bool IsDirettore_RdP(object objRuolo)
        {
            if (objRuolo != null)
            {
                Ruoli ruolo = (Ruoli)objRuolo;
                switch (ruolo)
                {
                    case Ruoli.P8974:
                    case Ruoli.P8975:
                    case Ruoli.P8976:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        internal static UtilityRuolo GetRuolo(object objRuolo)
        {
            UtilityRuolo ruolo = UtilityRuolo.UTENTE;

            if (IsAmministratore(objRuolo))
                ruolo = UtilityRuolo.AMMINISTRATORE;
            else if (IsDirettore_RdP(objRuolo))
                ruolo = UtilityRuolo.DIRETTORE_RDP;

            return ruolo;
        }

        internal static bool IsAmministratoreAGO(object objRuolo)
        {
            if (objRuolo != null)
            {
                Ruoli ruolo = (Ruoli)objRuolo;
                switch (ruolo)
                {
                    case Ruoli.P8854:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        #endregion Gestione Profilazione

        #region Gestione Versioning
        internal static void SetVersioni(Dictionary<string, string> listaVersioni)
        {
            if (listaVersioni != null)
                System.Web.HttpContext.Current.Session.Add("ListaVersioni", listaVersioni);
        }

        internal static Dictionary<string, string> GetVersioni()
        {
            return (Dictionary<string, string>)System.Web.HttpContext.Current.Session["ListaVersioni"];
        }
        #endregion Gestione Versioning

        #region Gestione Avvisi
        internal static void SetAvvisi(AreaAvvisi areaAvvisi)
        {
            System.Web.HttpContext.Current.Session.Remove("Avvisi");
            if (areaAvvisi != null && areaAvvisi.ElencoAvvisi != null && areaAvvisi.ElencoAvvisi.Length > 0)
                System.Web.HttpContext.Current.Session.Add("Avvisi", areaAvvisi.ElencoAvvisi.ToList());
        }

        internal static List<Presenter.SvrLiquidazione.Avvisi> GetAvvisi()
        {
            return (List<Presenter.SvrLiquidazione.Avvisi>)System.Web.HttpContext.Current.Session["Avvisi"];
        }
        #endregion Gestione Avvisi

        #region Gestione Messaggi Hermes
        internal static void SetMessaggiHermes(AreaMessaggiHermes areaMessaggiHermes)
        {
            System.Web.HttpContext.Current.Session.Remove("MessaggiHermes");
            if (areaMessaggiHermes != null && areaMessaggiHermes.ElencoMessaggiHermes != null && areaMessaggiHermes.ElencoMessaggiHermes.Length > 0)
                System.Web.HttpContext.Current.Session.Add("MessaggiHermes", areaMessaggiHermes.ElencoMessaggiHermes.ToList());
        }

        internal static List<Presenter.SvrLiquidazione.MessaggiHermes> GetMessaggiHermes()
        {
            return (List<Presenter.SvrLiquidazione.MessaggiHermes>)System.Web.HttpContext.Current.Session["MessaggiHermes"];
        }
        #endregion Gestione Messaggi Hermes

        #region Gestione Aggiornamenti
        internal static void SetAggiornamenti(AreaAggiornamenti areaAggiornamenti)
        {
            System.Web.HttpContext.Current.Session.Remove(EnumSession.Aggiornamenti.ToString());
            if (areaAggiornamenti != null && areaAggiornamenti.ElencoAggiornamenti != null && areaAggiornamenti.ElencoAggiornamenti.Length > 0)
                System.Web.HttpContext.Current.Session.Add(EnumSession.Aggiornamenti.ToString(), areaAggiornamenti.ElencoAggiornamenti.ToList());
        }

        internal static List<Presenter.SvrLiquidazione.Aggiornamenti> GetAggiornamenti()
        {
            return (List<Presenter.SvrLiquidazione.Aggiornamenti>)System.Web.HttpContext.Current.Session[EnumSession.Aggiornamenti.ToString()];
        }
        #endregion Gestione Aggiornamenti

        #region FS - Gestione Ddl Causa Carico

        /// <summary>
        /// Restituisce la lista dei valori di decodifica in base al tipo domanda
        /// </summary>
        public static List<AreaDecodifica.DatiCausaCarico> FS_GetDdlCausaCaricoByTipoDomanda(AreaTitolare.DatiPensione datiPensione, AreaDecodifica.DatiCausaCarico[] listaCausaCarico)
        {
            List<AreaDecodifica.DatiCausaCarico> lst = new List<AreaDecodifica.DatiCausaCarico>();
            foreach (AreaDecodifica.DatiCausaCarico causaCarico in listaCausaCarico)
            {
                switch (datiPensione.Tipo)
                {
                    case AreaTitolare.DatiPensione.TipoDomanda.Ripristino:
                    case AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti:
                        if (causaCarico.Id == "8" || causaCarico.Id == "9")
                            lst.Add(causaCarico);
                        break;
                    default:
                        lst.Add(causaCarico);
                        break;
                }
            }
            return lst;
        }

        /// <summary>
        /// Restituisce il SelecedValue della ddlCausaCarico presente in LiquidazionePensione\DatiGenerici e se la ddl deve essere abilitata.
        /// </summary>
        public static string FS_SelectedValueDdlCausaCaricoByTipoDomanda(AreaTitolare.DatiPensione datiPensione, string causaCarico, out bool ddlEnabled)
        {
            ddlEnabled = true;
            string selectedValue = string.Empty;
            switch (datiPensione.Tipo)
            {
                case AreaTitolare.DatiPensione.TipoDomanda.Normale:
                case AreaTitolare.DatiPensione.TipoDomanda.Superstiti:
                    selectedValue = "1";
                    ddlEnabled = false;
                    break;
                case AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti:
                case AreaTitolare.DatiPensione.TipoDomanda.Ripristino:
                    selectedValue = "9";
                    ddlEnabled = false;
                    break;
                case AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione:
                    ddlEnabled = true;
                    selectedValue = "2";
                    break;
            }
            return selectedValue;
        }

        #endregion FS - Gestione Ddl Causa Carico

        public static List<string> GetCategoriePensione(AreaDecodifica valoriDecodificati, TipoAppartenenzaRuolo tipoAppRuolo)
        {
            AreaDecodifica.DatiCategoriaPensione[] listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione;

            List<string> listaCatAmmesse = new List<string>();
            foreach (AreaDecodifica.DatiCategoriaPensione categoria in listaCategoriePensioni)
            {
                if (!String.IsNullOrEmpty(categoria.Sigla))
                    if (categoria.Appartenenza != Utility.GetDescription(tipoAppRuolo))
                        continue;
                string codiceCategoria = categoria.Codice;
                int codice;
                categoria.Sigla = categoria.Sigla.Trim();
                Int32.TryParse(codiceCategoria.Trim(), out codice);
                if (codice < 99 || (codice > 200 && codice < 207) || (codice > 212 && codice < 243) || codice >= 170 && codice <= 172 || codice == 198 || codice == 199 || codice == 127 || codice == 128 ||
                    codice == 129 || codice == 143 || codice == 197 || codice == 196 || codice == 200 || codice == 0243 || codice == 0244 || codice == 0245)
                {
                    switch (categoria.Sigla)
                    {
                        case "EL":
                        case "VL":
                        case "ES":
                        case "GAS":
                        case "FS":
                        case "ET":
                        case "TT":
                        case "DZ":
                        case "CL":
                        case "PM":
                        case "PL":
                            listaCatAmmesse.Add("V" + categoria.Sigla);
                            listaCatAmmesse.Add("I" + categoria.Sigla);
                            listaCatAmmesse.Add("S" + categoria.Sigla);
                            break;
                        case "PI":
                            foreach (string elem in new List<string>() { "A", "1", "Y", "U", "V" })
                            {
                                listaCatAmmesse.Add("V" + categoria.Sigla + elem);
                                listaCatAmmesse.Add("I" + categoria.Sigla + elem);
                                listaCatAmmesse.Add("S" + categoria.Sigla + elem);
                            }
                            break;
                        case "PMS":
                        case "PMO":
                            break;
                        default:
                            listaCatAmmesse.Add(categoria.Sigla);
                            break;
                    }
                }
            }
            return listaCatAmmesse;
        }

        public static bool IsContributiva(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo gruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto prodottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out gruppoPensione, out prodottoPensione, out tipoPensione);
            if (gruppoPensione == TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia &&
                (tipoPensione == TipologiaPensioneTipo.tp_Anzianita_TipoContributivoConOpzione || tipoPensione == TipologiaPensioneTipo.tp_Anzianita_TipoContributivoPuro ||
                 tipoPensione == TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoConOpzione || tipoPensione == TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoPuro))
                return true;
            return false;
        }

        public static bool IsContributivaPura(AreaTitolare.DatiPensione datiPensione)
        {
            TipologiaPensioneGruppo gruppoPensione = TipologiaPensioneGruppo.gr_NessunValore;
            TipologiaPensioneProdotto prodottoPensione = TipologiaPensioneProdotto.pr_NessunValore;
            TipologiaPensioneTipo tipoPensione = TipologiaPensioneTipo.tp_NessunValore;
            GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out gruppoPensione, out prodottoPensione, out tipoPensione);
            if (gruppoPensione == TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia &&
                (tipoPensione == TipologiaPensioneTipo.tp_Anzianita_TipoContributivoPuro ||
                tipoPensione == TipologiaPensioneTipo.tp_Vecchiaia_TipoContributivoPuro))
                return true;
            return false;
        }

        public static AreaTitolare.DatiPensione GetDatiPensioneFromSession()
        {
            return (AreaTitolare.DatiPensione)System.Web.HttpContext.Current.Session["DatiPensione"];
        }


        public static TipoFondo? GetTipoFondoByCategoria(UtilityTipoAppartenenza? tipoAppartenenza, string siglaCategoria)
        {
            TipoFondo? tipoFondo = null;
            if (tipoAppartenenza == UtilityTipoAppartenenza.FS)
            {
                string fondo = string.Empty;
                GetFondoBySiglaCategoria(siglaCategoria, out fondo);
                tipoFondo = GetEnumTipoFondoByCategoria(fondo);
            }
            return tipoFondo;
        }

        private static void GetFondoBySiglaCategoria(string siglaCategoria, out string fondo)
        {
            fondo = string.Empty;
            if (string.IsNullOrEmpty(siglaCategoria))
                return;
            if (siglaCategoria.Trim().Length < 3)
                fondo = siglaCategoria.Trim();
            else if (siglaCategoria.Trim().Length == 3)
                fondo = siglaCategoria.Substring(1, 2).Trim();
            else
                fondo = siglaCategoria.Substring(1, 3).Trim();
        }

        private static TipoFondo? GetEnumTipoFondoByCategoria(string fondo)
        {
            TipoFondo? tipoFondo = null;
            while (!string.IsNullOrEmpty(fondo) && tipoFondo == null)
            {
                tipoFondo = CodeUtility.GetValueFromDescription<TipoFondo>(fondo);
                fondo = fondo.Remove(fondo.Length - 1);
            }

            return tipoFondo;
        }
        #region Enum

        public enum EnumSession
        {
            Aggiornamenti,
            Courtesy_Type
        }

        public enum CourtesyType
        {
            SessionExpired,
            RuoloNonAbilitato
        }

        public enum TipologiaPensioneGruppo
        {
            gr_NessunValore,
            gr_Anzianita_Vecchiaia,
            gr_Inabilita_Invalidita,
            gr_Superstiti,
            gr_Ricostituzione,
            gr_Ripristini_Riliquidazioni
        };

        public enum TipologiaPensioneProdotto
        {
            pr_NessunValore,
            pr_Anzianita,
            pr_Vecchiaia,
            pr_InabilitaPensione,
            pr_InvaliditaAssegno,
            pr_InvaliditaPensione,
            pr_VariazioneDecorrenza,
            pr_MotiviContributivi,
            pr_Reversibilita,
            pr_Indiretta,
            pr_VariazioneDatiContitolari,
            pr_Supplemento,
            pr_Ripristino,
            pr_Riliquidazione
        };

        public enum TipologiaPensioneTipo
        {
            tp_NessunValore,
            tp_Vecchiaia_TrasfAOI,
            tp_Vecchiaia_Supplementare,
            tp_Invalidita_Ordinaria,
            [Description("Pensione anticipata con benefici L. 206/2004 - vittime Invalidità => 80%")]
            tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80,
            [Description("Pensione anticipata con benefici L. 206/2004 - vittime Invalidità < 80%")]
            tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80,
            [Description("Pensione di vecchiaia con benefici L. 206/2004 - vittime Invalidità < 80%")]
            tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80,
            // G 0002, P 0013, T 0009
            tp_Invalidita_Supplementare,
            // G 0003, P 0021, T 0009
            tp_Reversibilita_Supplementare,
            // G 0003, P 0022, T 0009
            tp_Indiretta_Supplementare,
            // G 0002, P 0011, T 0001
            tp_InvaliditaAssegno_Ordinario,
            tp_RicostituzioneContributivaPerEsecuzioneSentenza,
            // G 0002, P 0012, T 0168
            tp_Inabilita_Art1_C250_Legge232,
            // G 0001, P 0001, T 0051
            tp_Precoci,
            // G 0031, P 0107, T 0177
            tp_Ricostituzione_Cumulo_Progressiva,
            // G 0001, P 0002, T 0030
            tp_Vecchiaia_TipoContributivoConOpzione,
            // G 0001, P 0001, T 0030
            tp_Anzianita_TipoContributivoConOpzione,
            // G 0001, P 0002, T 0017
            tp_Vecchiaia_TipoContributivoPuro,
            // G 0001, P 0001, T 0017
            tp_Anzianita_TipoContributivoPuro,
            // G 0001, P 0001, T 0045
            tp_Anzianita_InComputo,
            // G 0001, P 0002, T 0045
            tp_Vecchiaia_InComputo,
            // G 0001, P 0002, T 0173
            tp_Vecchiaia_GravosiUsuranti,
            // G 0031, P0413, T 0001
            tp_RicostituzioneVariazioneDatiContitolari,
            // G 0002, P0012, T 0001
            tp_Inabilita_Ordinaria,
            // G 0002, P0012, T 0052
            tp_Inabilita_Art2_C12_Legge335
        };

        public enum StatoPensione
        {
            [Description("DA ACQUISIRE")]
            DaAcquisire = 0,
            [Description("IN ACQUISIZIONE")]
            InAcquisizione = 1,
            [Description("NON LAVORABILE")]
            NonLavorabile = 2,
            [Description("DA CALCOLARE")]
            DaCalcolare = 3,
            [Description("CALCOLATA")]
            Calcolata = 4,
            [Description("SCARTO DA CALCOLO")]
            ScartoDaCalcolo = 5,
            [Description("CALCOLO VERIFY")]
            CalcoloVerify = 6,
            [Description("SCARTO VERIFY")]
            ScartoVerify = 7,
            [Description("CALCOLO NO WEBDOM")]
            CalcolataNoWebDom = 8,
            [Description("CALCOLO NO FELPE")]
            CalcolataNoFelpe = 9,
            [Description("CALCOLO NO ONERI")]
            CalcolataNoOneri = 10,
            [Description("CALCOLO NO SAI")]
            CalcolataNoSai = 11,
            [Description("CALCOLO NO STAZ. LAVORO")]
            CalcolataNoStazLavoro = 12,
            [Description("CALCOLO NO TOTAL")]
            CalcolataNoTotal = 13,
            [Description("CALCOLO NO SIN")]
            CalcolataNoSin = 14,
            [Description("CALCOLATA NO BOOKING")]
            CalcolataNoBooking = 15,
            [Description("CALCOLO NO TOT")]
            CalcolataNoTot = 16,
            [Description("CALCOLO NO NOTE DEBITO")]
            CalcolataNoNoteDebito = 17,
            [Description("CALCOLO NO SEI SCATTI")]
            CalcolataNo6Scatti = 18,
            [Description("CALCOLO NO EQUOIND")]
            CalcolataNoEquoInd = 19,
            [Description("CALCOLO NO INDEB")]
            CalcoloNoInd = 20,
            [Description("CALCOLO NO INDEB WAIT")]
            CalcoloNoIndWait = 21,
            [Description("CALCOLO NO INDENN SPEC")]
            CalcolataNoIndennSpec = 22
        };

        public enum TipoFondo
        {
            FS,
            PM,
            PMS,
            VL,
            ES,
            ET,
            TT,
            DZ,
            GAS,
            EL,
            CL,
            PMO,
            PI,
            PL,
            PT
        };
        #endregion Enum

        #region SCRIPE
        public static void SetScripeSession(Control ctrl, string scripeCF, string scripeDOMUS)
        {
            if (ConfigurationManager.AppSettings["SCRIPE"] != null)
            {
                scripeDOMUS = scripeDOMUS.Length == 13 ? scripeDOMUS : string.Empty;
                ScriptManager.RegisterStartupScript(ctrl, ctrl.GetType(), "scripe_session", "function scripeCallback() { ScriPe.impostaDatiSessione({'codice_fiscale':'" + scripeCF + "','numero_webdom':'" + scripeDOMUS + "'}); }", true);
            }
        }
        #endregion
    }
}
