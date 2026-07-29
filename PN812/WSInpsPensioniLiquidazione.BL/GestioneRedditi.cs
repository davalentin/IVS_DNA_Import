using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.Redditi;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneRedditi
    {
        #region private members
        private static void ValorizzaRichiestaSrvRedditiPerGet(ref ContenitoreObject contenitore, ref ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore, short sedeOperatore, out Operatore WsOperatore, out Titolare WsTitolare,
            out ListaFamiliari WsListaFamiliari, out Pensione WsPensione, out string Url, out string errori)
        {
            Url = "";
            errori = "";
            WsOperatore = new Operatore();
            WsTitolare = new Titolare();
            WsListaFamiliari = new ListaFamiliari();
            WsPensione = new Pensione();

            if (contenitore.DatiPensione == null)
                return;
            if (contenitore.DatiAnagraficiTitolare == null)
                return;

            BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioneBenefici = contenitore.DatiMaggiorazioniBenefici;
            BLCommon.GestioneIstruttoria.DatiIstruttoria datiIstruttoria = contenitore.DatiIstruttoria;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = contenitore.DatiPensioniDatiGenerici;
            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGp = contenitore.DatiStoricoGP;
            Utility.TipoAppartenenza? tipoAppartenenza = contenitore.TipoAppartenenza;
            bool isRiaperturaDomanda = contenitore.IsRiaperturaDomanda;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, contenitore.DatiPensione.SiglaCategoria);
            //ENG - RIC Variazione Dati Contitolari
            List<GestioneFamiliari.Familiare> familiari = contenitore.ListaFamiliari;

            //ENG - Memo 79/2023
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoMemo79_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo79_2023", out controlloDinamicoMemo79_2023);

            //ENG - Memo 57/2023
            GestioneControlliDinamici.ControlloDinamico ctrlMemo57_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out ctrlMemo57_2023);

            #region Operatore
            WsOperatore.Matricola = matricolaOperatore;
            WsOperatore.SedeLavoro = sedeOperatore.ToString().PadLeft(4, '0');
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                WsOperatore.Procedura = "RC";
            else
                WsOperatore.Procedura = "PL";
            #endregion Operatore

            #region Titolare
            WsTitolare.CodiceARCA1 = "";
            WsTitolare.CodiceARCA2 = "00000000";
            //ENG - RIC Variazione Dati Contitolari
            if (tipoAppartenenza == Utility.TipoAppartenenza.AGO && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione)
                && Utility.IsRicostituzione_VariazioneDatiContitolari(contenitore.DatiPensione) && familiari != null
                && familiari.Count() > 0 && familiari.Exists(x => x.FlagTitolare.HasValue && x.FlagTitolare.Value))
            {
                WsTitolare.CodiceFiscale = familiari.Find(x => x.FlagTitolare.HasValue && x.FlagTitolare.Value).CodiceFiscale;
            }
            else
            {
                WsTitolare.CodiceFiscale = contenitore.DatiAnagraficiTitolare.CodiceFiscale;
            }
            WsTitolare.DataNascita = contenitore.DatiAnagraficiTitolare.DataNascita.Value;
            WsTitolare.SiglaTitolare = "T";
            WsTitolare.CodiciStatoResidenza = new StatoResidenza[1];
            WsTitolare.CodiciStatoResidenza[0] = new StatoResidenza();
            WsTitolare.CodiciStatoResidenza[0].Decorrenza = new DataDecorrenza();
            WsTitolare.CodiciStatoResidenza[0].Decorrenza.Anno = contenitore.DatiPensione.DataPresentazioneDomanda.Year;
            WsTitolare.CodiciStatoResidenza[0].Decorrenza.Mese = contenitore.DatiPensione.DataPresentazioneDomanda.Month;
            if (String.IsNullOrEmpty(contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza))
                WsTitolare.CodiciStatoResidenza[0].Sigla = "";
            else if (contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza.StartsWith("Z"))
            {
                GestioneDecodifica.StatoEstero statoEstero = null;
                try
                {
                    GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza.Trim(), out statoEstero);
                }
                catch (InvalidOperationException)
                {
                    errori = string.Format("Stato di residenza non univoco per il seguente parametro di input: Codice Catastale – {0}.", contenitore.DatiAnagraficiTitolare.CodiceComuneResidenza.Trim());
                    return;
                }
                if (statoEstero != null)
                    WsTitolare.CodiciStatoResidenza[0].Sigla = statoEstero.Sigla;
                else
                    WsTitolare.CodiciStatoResidenza[0].Sigla = "";
            }
            else
                WsTitolare.CodiciStatoResidenza[0].Sigla = "I";
            #endregion Titolare

            #region Pensione
            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                List<GestioneCalcolo.QuotePensione> lQuotePensione = contenitore.ListaQuotePensione;
                List<GestioneDecodifica.DecEnteGestioneFondo> listaApp = contenitoreDecodifica.ElencoDecEnteGestioneFondo.FindAll(x => x.Codice == "A1" || x.Codice == "A5" ||
                    x.Codice == "A6" || x.Codice == "A7" || x.Codice == "A8" || x.Codice == "A9" || x.Codice == "B1" || x.Codice == "B2" || x.Codice == "B3" || x.Codice == "B4" ||
                    x.Codice == "F0" || x.Codice == "C0" || x.Codice == "D0" || x.Codice == "E0" || x.Codice == "C1" || x.Codice == "C2" || x.Codice == "C3" || x.Codice == "C4" ||
                    x.Codice == "C5" || x.Codice == "D1" || x.Codice == "E1" || x.Codice == "E2" || x.Codice == "SP");

                List<GestioneDecodifica.DecEnteGestioneFondo> listaApp2 = contenitoreDecodifica.ElencoDecEnteGestioneFondo.FindAll(x => x.Codice == "PR");
                if (lQuotePensione != null && lQuotePensione.Count > 0 && lQuotePensione.Exists(x => listaApp2.Exists(y => y.Id == x.EnteGestioneFondo)))
                {
                    WsPensione.ANFPensTOT = "2";
                }
                else if (lQuotePensione != null && lQuotePensione.Count > 0 && lQuotePensione.Exists(x => listaApp.Exists(y => y.Id == x.EnteGestioneFondo)))
                {
                    WsPensione.ANFPensTOT = "1";
                }
                else
                {
                    if (datiIstruttoria != null && datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                    {
                        if (contenitoreDecodifica.ElencoCodiceParticolare != null && contenitoreDecodifica.ElencoCodiceParticolare.Count > 0)
                        {
                            long codicePart = datiIstruttoria.CodiceParticolareSoggettoDerogato.Value;
                            GestioneDecodifica.CodiceParticolare codiceParticolare = contenitoreDecodifica.ElencoCodiceParticolare.Find(x => x.Id == codicePart);
                            if (codiceParticolare != null && codiceParticolare.TraduzioneSuGp == '1')
                                WsPensione.ANFPensTOT = "1";
                        }
                    }
                }

                if (String.IsNullOrEmpty(WsPensione.ANFPensTOT))
                    WsPensione.ANFPensTOT = "2";
            }
            else
                WsPensione.ANFPensTOT = "";

            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            if (contenitore.DatiPensione.IsRicRinnovata.GetValueOrDefault())
            {
                WsPensione.AnnoCompetenza = dataSistema.Year + 1;
                if (WsPensione.AnnoCompetenza != dataSistema.Year + 1)
                {
                    errori = string.Format("L'anno di competenza deve essere l'anno di sistema + 1");
                    return;
                }
            }
            else
                WsPensione.AnnoCompetenza = annoCompetenza;

            WsPensione.Cessazione = new DataDecorrenza();
            if (contenitore.DatiEliminazione != null)
            {
                if (tipoAppartenenza == Utility.TipoAppartenenza.AGO || tipoAppartenenza == Utility.TipoAppartenenza.CI)
                {
                    WsPensione.Cessazione.Anno = contenitore.DatiEliminazione.DataEvento.HasValue ? contenitore.DatiEliminazione.DataEvento.Value.Year : 0;
                    WsPensione.Cessazione.Mese = contenitore.DatiEliminazione.DataEvento.HasValue ? contenitore.DatiEliminazione.DataEvento.Value.Month : 0;
                }
                else
                {
                    if (Utility.IsDomandaINPDAP(contenitore.DatiPensione.Gestione) || tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)
                    {
                        WsPensione.Cessazione.Anno = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Year : 0;
                        WsPensione.Cessazione.Mese = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Month : 0;
                    }
                    else
                    {
                        int meseDecorrenzaEliminazione = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Month : 0;
                        int meseDataEvento = contenitore.DatiEliminazione.DataEvento.HasValue ? contenitore.DatiEliminazione.DataEvento.Value.Month : 0;
                        if (meseDataEvento == 12 && meseDecorrenzaEliminazione == 1)
                        {
                            WsPensione.Cessazione.Anno = contenitore.DatiEliminazione.DataEvento.HasValue ? contenitore.DatiEliminazione.DataEvento.Value.Year : 0;
                            WsPensione.Cessazione.Mese = contenitore.DatiEliminazione.DataEvento.HasValue ? contenitore.DatiEliminazione.DataEvento.Value.Month : 0;
                        }
                        else
                        {
                            WsPensione.Cessazione.Anno = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Year : 0;
                            WsPensione.Cessazione.Mese = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Month : 0;
                        }
                    }
                }
            }
            else if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && datiGenericiAgoCi != null)
            {
                WsPensione.Cessazione.Anno = datiGenericiAgoCi.ScadenzaAssegno.HasValue ? datiGenericiAgoCi.ScadenzaAssegno.Value.Year : 0;
                WsPensione.Cessazione.Mese = datiGenericiAgoCi.ScadenzaAssegno.HasValue ? datiGenericiAgoCi.ScadenzaAssegno.Value.Month : 0;
            }
            else
            {
                WsPensione.Cessazione.Anno = 0;
                WsPensione.Cessazione.Mese = 0;
            }
            WsPensione.ChiavePensione = new ChiavePensione();
            string categoriaNumerica = contenitore.DatiPensione.GetCodCategoria();
            if (categoriaNumerica.Length == 4)
                categoriaNumerica = categoriaNumerica.Substring(1, 3);
            WsPensione.ChiavePensione.Categoria = categoriaNumerica;
            WsPensione.ChiavePensione.Certificato = contenitore.DatiPensione.NCertificato.HasValue ? contenitore.DatiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "";

            if (tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
                WsPensione.ChiavePensione.Sede = contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0');
            else
                WsPensione.ChiavePensione.Sede = contenitore.DatiPensione.CodiceSedeDestinazione.HasValue ? contenitore.DatiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                    contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0');

            if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0011" && contenitore.DatiPensione.Tipo == "0001")
                WsPensione.CodiceNatura = !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(0, 1) + "B" + contenitore.DatiPensione.NaturaPensione.Substring(2, 1) : null;
            else
                WsPensione.CodiceNatura = String.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? "" : contenitore.DatiPensione.NaturaPensione;

            WsPensione.DataPresDomanda = contenitore.DatiPensione.DataPresentazioneDomanda;
            WsPensione.Decorrenza = new DataDecorrenza();
            WsPensione.Decorrenza.Anno = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : 0;
            WsPensione.Decorrenza.Mese = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : 0;
            if (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
            {
                if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
                {
                    if (contenitoreDecodifica.ElencoDecAziendaAll != null)
                    {
                        short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                        GestioneDecodificaAzienda.DecAzienda decAziendaEditoria = contenitoreDecodifica.ElencoDecAziendaAll.Find(x => x.Id == codiceBancaEsodati);
                        if (decAziendaEditoria != null)
                            WsPensione.EntiCreditizi = decAziendaEditoria.TraduzioneSuGP;
                    }
                }
            }
            else
                WsPensione.EntiCreditizi = "";


            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                    case Utility.TipoFondo.PM:
                    case Utility.TipoFondo.PMS:
                        WsPensione.FlgPensioneContributiva = 0;
                        break;
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.VL:
                        if (contenitore.DatiContributivi != null)
                        {
                            if ((contenitore.DatiContributivi.ImportoContributivoTotale.HasValue && contenitore.DatiContributivi.ImportoContributivoTotale.Value > 0M) ||
                                (contenitore.DatiContributivi.Montante.HasValue && contenitore.DatiContributivi.Montante.Value > 0M))
                                WsPensione.FlgPensioneContributiva = 1;
                            else
                                WsPensione.FlgPensioneContributiva = 0;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        //case Utility.TipoFondo.PT:
                        if (contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0)
                        {
                            if (contenitore.ListaDatiContributivi.Exists(x => ((x.ImportoContributivoTotale.HasValue && x.ImportoContributivoTotale.Value > 0M) ||
                                (x.Montante.HasValue && x.Montante.Value > 0M))))
                                WsPensione.FlgPensioneContributiva = 1;
                            else
                                WsPensione.FlgPensioneContributiva = 0;
                        }
                        break;
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.GAS:
                        if (contenitore.DatiContributivi != null)
                        {
                            if (contenitore.DatiContributivi.Montante.HasValue && contenitore.DatiContributivi.Montante.Value > 0M)
                                WsPensione.FlgPensioneContributiva = 1;
                            else
                                WsPensione.FlgPensioneContributiva = 0;
                        }
                        break;
                }
            }
            else if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                if (contenitore.DatiPensione.Contributivo.GetValueOrDefault() == '8')
                    WsPensione.FlgPensioneContributiva = 8;
            }

            if (contenitore.DatiPensione.TipoCalcolo.HasValue && (tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.AGO))
            {
                string strTipoCalcolo = contenitore.DatiPensione.TipoCalcolo.Value.ToString();
                GestioneDecodifica.TipoCalcolo tipoCalcolo = contenitoreDecodifica.ElencoTipoCalcolo.Find(x => x.Id == strTipoCalcolo);
                if (tipoCalcolo != null && tipoCalcolo.TraduzioneSuGP.HasValue)
                {
                    // Contributivo per AGO
                    if (tipoCalcolo.Tipo == "Inps" && (tipoCalcolo.TraduzioneSuGP.Value == 1 || tipoCalcolo.TraduzioneSuGP.Value == 4))
                    {
                        if ((tipoAppartenenza == Utility.TipoAppartenenza.AGO && (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, isRiaperturaDomanda) ||
                             (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && Utility.IsDomandaReversibilita(contenitore.DatiPensione)))) &&
                            tipoCalcolo.TraduzioneSuGP.Value == 1 && datiStoricoGp != null && datiStoricoGp.Contributivo == '1')
                            WsPensione.FlgPensioneContributiva = 0;
                        else
                            WsPensione.FlgPensioneContributiva = 8;
                    }
                    // Contributivo per FS
                    else if (tipoAppartenenza == Utility.TipoAppartenenza.FS && tipoCalcolo.TraduzioneSuGP.Value == 4)
                        WsPensione.FlgPensioneContributiva = 8;
                }
            }
            //Contributivo per CI
            else if (tipoAppartenenza != null && tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, null))
                WsPensione.FlgPensioneContributiva = 8;
            else if (tipoAppartenenza == Utility.TipoAppartenenza.CI && contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0 &&
                (contenitore.ListaDatiRetributivi == null || contenitore.ListaDatiRetributivi.Count == 0))
            {
                if (contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 == 0 || contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 == null || Utility.IsDomandaSperimentaleDonnaOrRicostituzione(contenitore.DatiPensione) ||
                    Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(contenitore.DatiPensione) || (contenitore.DatiNuoveLiquidate != null && contenitore.DatiNuoveLiquidate.FlagContributiva.GetValueOrDefault() == true) || Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, true))
                    WsPensione.FlgPensioneContributiva = 8;
                else if (contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 != null && contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295 > 0)
                    WsPensione.FlgPensioneContributiva = 2;
            }

            if(Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, isRiaperturaDomanda) != null)
                WsPensione.FlgPensioneContributiva = 0;

            if (tipoAppartenenza != null && tipoAppartenenza == BLCommon.Utility.TipoAppartenenza.FS)
            {
                WsPensione.FondiSpeciali = new FondiSpeciali();
                //per le ricostituzioni dipende dal codice specifico
                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                {
                    if (contenitore.DatiFondo != null)
                    {
                        if (contenitoreDecodifica.ElencoCodiceSpecifico != null && contenitore.DatiFondo.CodiceSpecifico.HasValue)
                        {
                            byte? codiceSpecifico = contenitore.DatiFondo.CodiceSpecifico;
                            GestioneDecodifica.CodiceSpecifico codSpec = contenitoreDecodifica.ElencoCodiceSpecifico.Find(x => x.Id == codiceSpecifico);
                            if (codSpec != null)
                            {
                                WsPensione.FondiSpeciali.Codice = codSpec.TipoPensione.HasValue ? 0 + int.Parse(codSpec.TipoPensione.Value.ToString()) : 0;
                            }
                        }
                    }
                }
                else
                {
                    int tipoPensione = GetTipoPensionePerDRedd(ref contenitore, ref contenitoreDecodifica);
                    WsPensione.FondiSpeciali.Codice = tipoPensione;
                }

                if (contenitore.DatiPensione.SiglaCategoria.StartsWith("S"))
                    WsPensione.FondiSpeciali.Tipologia = 3;
                else if (contenitore.DatiPensione.SiglaCategoria.StartsWith("I"))
                    WsPensione.FondiSpeciali.Tipologia = 2;
                else if (contenitore.DatiPensione.SiglaCategoria.StartsWith("V"))
                    WsPensione.FondiSpeciali.Tipologia = 1;
            }
            else
            {
                WsPensione.FondiSpeciali = new FondiSpeciali();
                WsPensione.FondiSpeciali.Codice = 0;
                WsPensione.FondiSpeciali.Tipologia = 0;
            }

            WsPensione.IdPensione = 0;
            WsPensione.InvaliditaCivile = null;
            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione) || Utility.IsDomandaQuota100(contenitore.DatiPensione) || Utility.IsDomandaQuota102(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione) ||
                Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
            {
                string code = Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione) ? "5000" : (Utility.IsDomandaQuota100(contenitore.DatiPensione) ? "5300" : (Utility.IsDomandaQuota102(contenitore.DatiPensione) ? "5800" : string.Empty));
                if (Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione))
                    code = "6000";
                if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
                    code = "6100";
                GestioneDecodifica.GruppoOneri gruppoOneri = contenitoreDecodifica.ElencoDecCodeGruppoOnere != null ? contenitoreDecodifica.ElencoDecCodeGruppoOnere.FirstOrDefault(x => x.Code == code) : null;

                if (gruppoOneri != null && contenitore.ListaDatiOneri != null && contenitore.ListaDatiOneri.Count > 0 && contenitore.ListaDatiOneri.Exists(x => x.IdCodeGruppo == gruppoOneri.Id) &&
                    contenitore.ListaDatiOneri.FirstOrDefault(x => x.IdCodeGruppo == gruppoOneri.Id).ScadenzaBeneficio.HasValue)
                {
                    WsPensione.InvaliditaCivile = new InvaliditaCivile();
                    WsPensione.InvaliditaCivile.Codice = "00";
                    WsPensione.InvaliditaCivile.FasceInvalidita = new FasceInvalidita[1];
                    WsPensione.InvaliditaCivile.FasceInvalidita[0] = new FasceInvalidita();
                    WsPensione.InvaliditaCivile.FasceInvalidita[0].Decorrenza = new DataDecorrenza();
                    WsPensione.InvaliditaCivile.FasceInvalidita[0].Decorrenza.Anno = contenitore.ListaDatiOneri.FirstOrDefault(x => x.IdCodeGruppo == gruppoOneri.Id).ScadenzaBeneficio.GetValueOrDefault().Year;
                    WsPensione.InvaliditaCivile.FasceInvalidita[0].Decorrenza.Mese = contenitore.ListaDatiOneri.FirstOrDefault(x => x.IdCodeGruppo == gruppoOneri.Id).ScadenzaBeneficio.GetValueOrDefault().Month;
                    WsPensione.InvaliditaCivile.FasceInvalidita[0].CodiceFascia = "00";
                }
            }

            WsPensione.MaggiorazioneSociale = new MaggiorazioneSociale();
            WsPensione.MaggiorazioneSociale.Decorrenza = new DataDecorrenza();
            WsPensione.MaggiorazioneSociale.Cessazione = new DataDecorrenza();
            if (datiMaggiorazioneBenefici != null)
            {
                WsPensione.MaggiorazioneSociale.Decorrenza.Anno = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.HasValue ? datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.Value.Year : 0;
                WsPensione.MaggiorazioneSociale.Decorrenza.Mese = datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.HasValue ? datiMaggiorazioneBenefici.DecorrenzaMaggiorazioneSociale.Value.Month : 0;
                WsPensione.MaggiorazioneSociale.Cessazione.Anno = datiMaggiorazioneBenefici.CessazioneMaggiorazioneSociale.HasValue ? datiMaggiorazioneBenefici.CessazioneMaggiorazioneSociale.Value.Year : 0;
                WsPensione.MaggiorazioneSociale.Cessazione.Mese = datiMaggiorazioneBenefici.CessazioneMaggiorazioneSociale.HasValue ? datiMaggiorazioneBenefici.CessazioneMaggiorazioneSociale.Value.Month : 0;
            }
            else
            {
                WsPensione.MaggiorazioneSociale.Decorrenza.Anno = 0;
                WsPensione.MaggiorazioneSociale.Decorrenza.Mese = 0;
                WsPensione.MaggiorazioneSociale.Cessazione.Anno = 0;
                WsPensione.MaggiorazioneSociale.Cessazione.Mese = 0;
            }

            WsPensione.NumeroWebdom = contenitore.DatiPensione.NDomus.ToString().PadLeft(13, '0');
            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 2;
            else if (Utility.IsDomandaQuota100(contenitore.DatiPensione) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 3;
            else if (Utility.IsDomandaQuota102(contenitore.DatiPensione) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 4;
            else if ((Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione)) && !Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 6;
            else if (Utility.IsPensioneInabilitaProficuoLavoroCumulo(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 7;
            else if (ctrlMemo57_2023 != null && !String.IsNullOrEmpty(ctrlMemo57_2023.ValoreControllo) && ctrlMemo57_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI" &&
                Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && !String.IsNullOrEmpty(contenitore.DatiPensione.AnnoMonitoraggio) &&
                contenitore.DatiPensione.AnnoMonitoraggio.Trim() == "2024")
                WsPensione.Sentenza240 = 8;
            else if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
                WsPensione.Sentenza240 = 9;
            else
                WsPensione.Sentenza240 = 0;

            //ENG - Memo 79/2023
            //ENG - Esteso anche alla linea FS
            if (controlloDinamicoMemo79_2023 != null && !String.IsNullOrEmpty(controlloDinamicoMemo79_2023.ValoreControllo)
                && !String.IsNullOrEmpty(controlloDinamicoMemo79_2023.ValoreControllo.Trim())
                && controlloDinamicoMemo79_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (contenitore.DatiPensione.GP1AV91A.HasValue && contenitore.DatiPensione.GP1AV91A.Value == 5)
                    WsPensione.Sentenza240 = 5;
            }
            #endregion Pensione

            #region Familiari
            WsListaFamiliari.Familiari = new INPS.Pensioni.Liquidazione.ServiceReferences.Redditi.Familiare[15];

            List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari = null;
            List<Entity.Anagrafica> listaAnagrafiche = null;
            GestioneAreaFamiliari.GetFamiliariByDatiPensione(ref contenitore, out listaFamiliari, out listaAnagrafiche);

            if (listaFamiliari != null && listaFamiliari.Count > 0 && listaAnagrafiche != null && listaAnagrafiche.Count > 0)
            {
                for (int i = 0; i < listaFamiliari.Count; i++)
                {
                    if (WsTitolare.CodiceFiscale.Equals(listaFamiliari[i].Familiare.CodiceFiscale) || (WsListaFamiliari != null && WsListaFamiliari.Familiari != null &&
                        WsListaFamiliari.Familiari.Count() > 0 && WsListaFamiliari.Familiari.ToList().FindIndex(x => x != null && x.CFAnagrafica == listaFamiliari[i].Familiare.CodiceFiscale) > -1))
                    {
                        string certificato = contenitore.DatiPensione.NCertificato.ToString().PadLeft(8, '0');
                        if (contenitore.DatiPensione.SiglaCategoria.StartsWith("S") ||
                            ((contenitore.DatiPensione.SiglaCategoria.Trim().Equals("PMO") || contenitore.DatiPensione.SiglaCategoria.Trim().Equals("PSO")) && (certificato.Substring(2, 1).Equals("3") || certificato.Substring(2, 1).Equals("6"))))
                            WsTitolare.SiglaTitolare = listaFamiliari[i].Familiare.SiglaFamiliare.GetValueOrDefault().ToString();
                        continue;
                    }

                    WsListaFamiliari.Familiari[i] = new INPS.Pensioni.Liquidazione.ServiceReferences.Redditi.Familiare();
                    WsListaFamiliari.Familiari[i].CFAnagrafica = listaAnagrafiche[i].CodiceFiscale;
                    if (!String.IsNullOrEmpty(WsListaFamiliari.Familiari[i].CFAnagrafica))
                        WsListaFamiliari.Familiari[i].FlagCFAnag = "F";
                    else
                        WsListaFamiliari.Familiari[i].FlagCFAnag = "A";
                    WsListaFamiliari.Familiari[i].CodiceARCA1 = "";
                    WsListaFamiliari.Familiari[i].CodiceARCA2 = "00000000";
                    WsListaFamiliari.Familiari[i].SiglaFamiliare = listaFamiliari[i].Familiare.SiglaFamiliare.HasValue ?
                        listaFamiliari[i].Familiare.SiglaFamiliare.Value.ToString() : "";
                    if (listaFamiliari[i].ElencoCodMaggFamiliari != null)
                    {
                        WsListaFamiliari.Familiari[i].ListaDirittoANF = new DirittoANF[listaFamiliari[i].ElencoCodMaggFamiliari.Count];
                        int j = 0;
                        foreach (GestioneFamiliari.CodMaggFamiliari codMagg in listaFamiliari[i].ElencoCodMaggFamiliari)
                        {
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j] = new DirittoANF();
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Decorrenza = new DataDecorrenza();
                            if (WsListaFamiliari.Familiari[i].SiglaFamiliare == "G" || WsListaFamiliari.Familiari[i].SiglaFamiliare == "F")
                                WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Codice = "0";
                            else
                                WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Codice = "1";
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Decorrenza.Mese = codMagg.Decorrenza.HasValue ?
                                codMagg.Decorrenza.Value.Month : 0;
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Decorrenza.Anno = codMagg.Decorrenza.HasValue ?
                                codMagg.Decorrenza.Value.Year : 0;
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Cessazione = new DataDecorrenza();
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Cessazione.Mese = codMagg.Cessazione.HasValue ?
                                codMagg.Cessazione.Value.Month : 0;
                            WsListaFamiliari.Familiari[i].ListaDirittoANF[j].Cessazione.Anno = codMagg.Cessazione.HasValue ?
                                codMagg.Cessazione.Value.Year : 0;
                            j++;
                        }
                    }
                }
            }
            #endregion Familiari

            RecuperaUrl(WsPensione.ChiavePensione, WsOperatore.Procedura, matricolaOperatore, sedeOperatore, out Url);
        }

        private static void ValorizzaRichiestaSrvRedditiPerVerify(ref ContenitoreObject contenitore, string matricolaOperatore, short sedeOperatore, out ChiavePensione WsChiavePensione, out string Url)
        {
            Url = "";
            WsChiavePensione = null;

            if (contenitore.DatiPensione == null)
                return;

            RecuperaChiavePensione(contenitore.DatiPensione, out WsChiavePensione);

            string procedura = "";
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                procedura = "RC";
            else
                procedura = "PL";

            RecuperaUrl(WsChiavePensione, procedura, matricolaOperatore, sedeOperatore, out Url);
        }

        private static void RecuperaChiavePensione(GestionePensione.DatiPensione datiPensione, out ChiavePensione WsChiavePensione)
        {
            WsChiavePensione = new ChiavePensione();
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            string categoriaNumerica = datiPensione.GetCodCategoria();
            if (categoriaNumerica.Length == 4)
                categoriaNumerica = categoriaNumerica.Substring(1, 3);
            WsChiavePensione.Categoria = categoriaNumerica;
            WsChiavePensione.Certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "";

            if (tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaAPEPrecoci(datiPensione))
                WsChiavePensione.Sede = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
            else
                WsChiavePensione.Sede = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                    datiPensione.CodiceSede.ToString().PadLeft(4, '0');
        }

        private static void ValorizzaAreaRedditiFromSrvRedditi(Output_VerificaPresRedditi WsRisposta, out AreaRedditi areaRedditi)
        {
            areaRedditi = new AreaRedditi();

            if (WsRisposta == null)
            {
                areaRedditi.MessaggioVideo = "Non sono presenti redditi negli archivi centrali";
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return;
            }
            areaRedditi.UltimaModifica = WsRisposta.DataUltimaModifica;
            areaRedditi.StatoPensione = WsRisposta.StatoPensione;
            if (WsRisposta.ListaAnniRilevanze != null && WsRisposta.ListaAnniRilevanze.Length > 0)
            {
                areaRedditi.ListaRedditi = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd>();
                foreach (AnniRilevanze elementoReddito in WsRisposta.ListaAnniRilevanze)
                {
                    for (int i = 0; i < elementoReddito.Rilevanze.Length; i += 2)
                        areaRedditi.ListaRedditi.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd((short)elementoReddito.Anno, elementoReddito.Rilevanze.Substring(i, 2)));
                }
            }
            else if (areaRedditi.UltimaModifica == DateTime.MinValue)
            {
                areaRedditi.MessaggioVideo = "Non sono presenti redditi negli archivi centrali";
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return;
            }

            areaRedditi.MessaggioVideo = "";
            areaRedditi.Esito = TipoRitornoRedditi.NessunErrore;
        }

        private static void ConfrontaRedditi(bool IsSalvataggio, AreaRedditi redditiOriginali, AreaRedditi ultimiRedditi)
        {
            if (ultimiRedditi == null || redditiOriginali == null)
            {
                if (ultimiRedditi == null)
                    ultimiRedditi = new AreaRedditi();
                ultimiRedditi.MessaggioVideo = "Aree reddituali non valorizzate correttamente";
                ultimiRedditi.Esito = TipoRitornoRedditi.Errore;
                return;
            }

            if ((ultimiRedditi.UltimaModifica != redditiOriginali.UltimaModifica) ||
                (ultimiRedditi.ListaRedditi == null && redditiOriginali.ListaRedditi != null) ||
                (ultimiRedditi.ListaRedditi != null && redditiOriginali.ListaRedditi == null))
            {
                ultimiRedditi.MessaggioVideo = "I redditi sono stati variati";
                ultimiRedditi.Esito = TipoRitornoRedditi.Informativa;
                return;
            }

            if (ultimiRedditi.ListaRedditi != null && redditiOriginali.ListaRedditi != null)
            {
                if (ultimiRedditi.ListaRedditi.Count != redditiOriginali.ListaRedditi.Count)
                {
                    ultimiRedditi.MessaggioVideo = "I redditi sono stati variati";
                    ultimiRedditi.Esito = TipoRitornoRedditi.Informativa;
                    return;
                }
                foreach (BLCommon.GestioneRedditi.RedditoDRedd ultimoReddito in ultimiRedditi.ListaRedditi)
                {
                    bool IsPresente = false;
                    foreach (BLCommon.GestioneRedditi.RedditoDRedd redditoOriginale in redditiOriginali.ListaRedditi)
                    {
                        if (ultimoReddito.AnnoReddito == redditoOriginale.AnnoReddito &&
                            ultimoReddito.Rilevanza == redditoOriginale.Rilevanza)
                        {
                            IsPresente = true;
                            break;
                        }
                    }
                    if (!IsPresente)
                    {
                        ultimiRedditi.MessaggioVideo = "I redditi sono stati variati";
                        ultimiRedditi.Esito = TipoRitornoRedditi.Informativa;
                        return;
                    }
                }
            }

            if (!IsSalvataggio)
            {
                if (ultimiRedditi.ListaRedditi == null || ultimiRedditi.ListaRedditi.Count == 0)
                {
                    if (ultimiRedditi.StatoPensione)
                    {
                        ultimiRedditi.MessaggioVideo = "Non sono stati acquisiti redditi. Redditi non obbligatori";
                        ultimiRedditi.Esito = TipoRitornoRedditi.Informativa;
                        return;
                    }
                    else
                    {
                        ultimiRedditi.MessaggioVideo = "Non sono stati acquisiti redditi. Redditi obbligatori";
                        ultimiRedditi.Esito = TipoRitornoRedditi.Errore;
                        return;
                    }
                }
            }
            else
            {
                if (ultimiRedditi.StatoPensione)
                {
                    ultimiRedditi.MessaggioVideo = "";
                    ultimiRedditi.Esito = TipoRitornoRedditi.NessunErrore;
                    return;
                }
                else
                {
                    ultimiRedditi.MessaggioVideo = "Dati incompleti. Redditi non acquisiti";
                    ultimiRedditi.Esito = TipoRitornoRedditi.Errore;
                    return;
                }
            }

            ultimiRedditi.MessaggioVideo = "";
            ultimiRedditi.Esito = TipoRitornoRedditi.NessunErrore;
        }

        private static bool ConfrontaRedditiDBByIdPensione(ref ContenitoreObject contenitore, AreaRedditi redditi, out bool IsPresentiRedditiDB)
        {
            IsPresentiRedditiDB = false;
            if (contenitore.ListaRedditoDRedd != null)
            {
                IsPresentiRedditiDB = true;
                if (redditi == null || redditi.ListaRedditi == null)
                    return false;
                if (contenitore.ListaRedditoDRedd.Count != redditi.ListaRedditi.Count)
                    return false;
                foreach (BLCommon.GestioneRedditi.RedditoDRedd redditoDB in contenitore.ListaRedditoDRedd)
                {
                    bool presente = false;
                    foreach (BLCommon.GestioneRedditi.RedditoDRedd redditoSrv in redditi.ListaRedditi)
                    {
                        if (redditoDB.AnnoReddito == redditoSrv.AnnoReddito && redditoDB.Rilevanza == redditoSrv.Rilevanza)
                            presente = true;
                    }
                    if (!presente)
                        return false;
                }
            }
            return true;
        }

        private static bool VerifyRedditiFromSrvRedditi(long numDomanda, ChiavePensione WsChiave, out Output_VerificaPresRedditi WsRisposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            RED_ServiceSoapClient proxy = new RED_ServiceSoapClient();
            WsRisposta = null;
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    WsRisposta = proxy.verificaInserimentoRedditi(WsChiave);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio Redditi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la verifica dei redditi";
                        string parametri = null;
                        try
                        {
                            parametri = Utility.GetXmlFromObject(WsChiave);
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool GetRedditiFromSrvRedditi(Operatore WsOperatore, Titolare WsTitolare, ListaFamiliari WsListaFamiliari, Pensione WsPensione, out Output_VerificaPresRedditi WsRisposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            RED_ServiceSoapClient proxy = new RED_ServiceSoapClient();
            WsRisposta = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(new AreaInputDatiSQL(WsOperatore, WsTitolare, WsListaFamiliari, WsPensione), Utility.Servizio.SrvRedditi, Utility.MetodoServizio.insertDatiSQL, Utility.SOAPLogDirection.IN, WsPensione.NumeroWebdom, guid);
                    WsRisposta = proxy.insertDatiSQL(WsPensione, WsTitolare, WsListaFamiliari, WsOperatore);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio Redditi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero dei redditi";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        long numDomanda = 0;
                        long.TryParse(WsPensione.NumeroWebdom, out numDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(WsRisposta, Utility.Servizio.SrvRedditi, Utility.MetodoServizio.insertDatiSQL, Utility.SOAPLogDirection.OUT, WsPensione.NumeroWebdom, guid);
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static void RecuperaUrl(ChiavePensione WsChiavePensione, string procedura, string matricolaOperatore, short sedeOperatore, out string url)
        {
            url = "";
            if (ConfigurationManager.AppSettings["UrlRedditi"] != null)
            {
                url = ConfigurationManager.AppSettings["UrlRedditi"];
                url += "?Matricola=" + matricolaOperatore.PadLeft(8, '0');
                url += "&SedeLavoro=" + sedeOperatore.ToString().PadLeft(4, '0');
                url += "&Categoria=" + WsChiavePensione.Categoria;
                url += "&Sede=" + WsChiavePensione.Sede;
                url += "&Certificato=" + WsChiavePensione.Certificato;
                url += "&Procedura=" + procedura;
            }
        }

        private static int GetTipoPensionePerDRedd(ref ContenitoreObject contenitore, ref ContenitoreDecodifica contenitoreDecodifica)
        {
            int tipoPensione = 0;

            if (contenitore.DatiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione, contenitore.DatiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                {
                    char? codiceSpecifico = null;
                    if (contenitore.DatiFondo != null)
                    {
                        if (contenitoreDecodifica.ElencoCodiceSpecifico != null && contenitore.DatiFondo.CodiceSpecifico.HasValue)
                        {
                            byte? codice = contenitore.DatiFondo.CodiceSpecifico;
                            GestioneDecodifica.CodiceSpecifico codSpec = contenitoreDecodifica.ElencoCodiceSpecifico.Find(x => x.Id == codice);
                            if (codSpec != null)
                            {
                                codiceSpecifico = codSpec.TraduzioneGp;
                            }
                        }
                    }

                    if (!codiceSpecifico.HasValue)
                    {
                        byte? cS = Utility.CalcolaCodiceSpecificoForVolo(contenitore.DatiPensione);
                        if (cS.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodiceSpecifico != null)
                            {
                                GestioneDecodifica.CodiceSpecifico codSpec = contenitoreDecodifica.ElencoCodiceSpecifico.Find(x => x.Id == cS.Value);
                                if (codSpec != null)
                                {
                                    codiceSpecifico = codSpec.TraduzioneGp;
                                }
                            }
                        }
                    }

                    byte? codArt22 = contenitore.DatiFondoVL != null ? contenitore.DatiFondoVL.CodiceArt22 : (byte?)null;
                    if (!codArt22.HasValue)
                        codArt22 = Utility.CalcolaArticolo22ForVolo(contenitore.DatiPensione);
                    tipoPensione = Utility.GetTipoPensioneForVolo(tipoFondo, codArt22, codiceSpecifico);
                }
                else
                {
                    switch (contenitore.DatiPensione.Gruppo)
                    {
                        case "0001":
                            switch (contenitore.DatiPensione.Prodotto)
                            {
                                case "0001":
                                    tipoPensione = 1;
                                    break;
                                case "0002":
                                    switch (contenitore.DatiPensione.Tipo)
                                    {
                                        case "0008":
                                            tipoPensione = 3;
                                            break;
                                        default:
                                            tipoPensione = 2;
                                            break;
                                    }
                                    break;
                                case "0003":
                                    tipoPensione = 8;
                                    break;
                            }
                            break;
                        case "0002":
                            switch (contenitore.DatiPensione.Prodotto)
                            {
                                case "0011":
                                    tipoPensione = 5;
                                    break;
                                case "0012":
                                    if (contenitore.DatiPensione.Tipo != "0047" || (tipoFondo != Utility.TipoFondo.FS && tipoFondo != Utility.TipoFondo.PT && !Utility.IsDomandaINPDAP(contenitore.DatiPensione.Gestione)))
                                        tipoPensione = 6;
                                    break;
                            }
                            break;
                        case "0003":
                            tipoPensione = 7;
                            break;
                    }
                }
            }

            return tipoPensione;
        }

        private static bool EliminaRedditiFromSrvRedditi(long numDomanda, ChiavePensione WsChiave, out string errori)
        {
            bool erroreTenico = false;
            errori = string.Empty;
            RED_ServiceSoapClient proxy = new RED_ServiceSoapClient();
            string stackTrace = null;

            AuthHeader auth = new AuthHeader();
            if (ConfigurationManager.AppSettings["ChiaveAccessoRedditi"] != null)
                auth.ChiaveAccesso = ConfigurationManager.AppSettings["ChiaveAccessoRedditi"];
            Esito esito = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    esito = proxy.eliminaRedditiAncheTemporanei(auth, WsChiave, true);
                    if (esito.Codice != 0)
                    {
                        errori = esito.Descrizione;
                        return false;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTenico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTenico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTenico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTenico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio Redditi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTenico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTenico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'eliminazione dei redditi";
                        string parametri = null;
                        try
                        {
                            parametri = Utility.GetXmlFromObject(WsChiave);
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool VerificaPresenzaRedditi(string ndomus, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;
            RED_ServiceSoapClient proxy = new RED_ServiceSoapClient();
            Output_RedditiTemp risposta = null;
            //AuthHeader auth = new AuthHeader();
            //if (ConfigurationManager.AppSettings["ChiaveAccessoRedditi"] != null)
            //    auth.ChiaveAccesso = ConfigurationManager.AppSettings["ChiaveAccessoRedditi"];

            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    risposta = proxy.getRedditiTemp(ndomus);
                    if (risposta != null && risposta.Esito != null && risposta.Esito.Codice == 0 && risposta.ListaRedditiTemp != null &&
                        risposta.ListaRedditiTemp.RedditoTemp != null && risposta.ListaRedditiTemp.RedditoTemp.Count() > 0)
                        return true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio Redditi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio Redditi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la ricerca di redditi temporanei";
                        string parametri = null;
                        long numeroDomanda = 0;
                        long.TryParse(ndomus, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvRedditi, Utility.MetodoServizio.getRedditiTemp, Utility.SOAPLogDirection.OUT, ndomus, guid);
                    Utility.CloseClient(proxy);
                }
            }
        }
        #endregion private members

        #region public members
        public static bool GetRedditiDB(ref ContenitoreObject contenitore, out AreaRedditi areaRedditi)
        {
            areaRedditi = new AreaRedditi();
            areaRedditi.ListaRedditi = contenitore.ListaRedditoDRedd;
            return true;
        }

        public static bool GetRedditiByDatiPensione(ref ContenitoreObject contenitore, ref ContenitoreDecodifica contenitoreDecodifica, string matricolaOperatore, short sedeOperatore, out AreaRedditi areaRedditi)
        {
            areaRedditi = null;
            Operatore WsOperatore = null;
            Titolare WsTitolare = null;
            ListaFamiliari WsListaFamiliari = null;
            Pensione WsPensione = null;
            string Url = "";
            string errori = "";
            Output_VerificaPresRedditi WsRisposta = null;
            ValorizzaRichiestaSrvRedditiPerGet(ref contenitore, ref contenitoreDecodifica, matricolaOperatore, sedeOperatore, out WsOperatore, out WsTitolare, out WsListaFamiliari, out WsPensione, out Url, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                areaRedditi = new AreaRedditi();
                areaRedditi.Url = Url;
                areaRedditi.MessaggioVideo = errori;
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return false;
            }
            StringBuilder messaggioVideoStrBuilder = new StringBuilder();
            GetRedditiFromSrvRedditi(WsOperatore, WsTitolare, WsListaFamiliari, WsPensione, out WsRisposta, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                areaRedditi = new AreaRedditi();
                areaRedditi.Url = Url;
                areaRedditi.MessaggioVideo = errori;
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return false;
            }
            if (WsRisposta.ListaEsito != null && (WsRisposta.ListaEsito.Length != 1 || WsRisposta.ListaEsito[0].Codice != 0))
            {
                areaRedditi = new AreaRedditi();
                areaRedditi.Url = Url;
                foreach (Esito esito in WsRisposta.ListaEsito)
                {
                    if (esito != null)
                    {
                        messaggioVideoStrBuilder.Append(esito.Codice.ToString() + ": " + esito.Descrizione + ". ");
                    }
                }
                areaRedditi.MessaggioVideo = messaggioVideoStrBuilder.ToString();
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return true;
            }

            ValorizzaAreaRedditiFromSrvRedditi(WsRisposta, out areaRedditi);

            areaRedditi.Url = Url;
            bool IsPresentiRedditiDB = false;
            if (!ConfrontaRedditiDBByIdPensione(ref contenitore, areaRedditi, out IsPresentiRedditiDB))
            {
                areaRedditi.MessaggioVideo = "I redditi salvati sono differenti da quelli recuperati dal servizio DRedd. Rieseguire il salvataggio.";
                areaRedditi.Esito = TipoRitornoRedditi.Informativa;
            }

            if (string.IsNullOrEmpty(areaRedditi.MessaggioVideo))
            {
                if (IsPresentiRedditiDB)
                {
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);
                    //messaggio informativo in caso di variazione familiari e/o sede destinazione
                    if (contenitore.DatiQuadroRedditi.TabRedditi == 0 || contenitore.DatiQuadroRedditi.TabRedditi == 1)
                    {
                        areaRedditi.MessaggioVideo = "Potrebbero essere state effettuate variazioni al menu dei familiari o alla sede di destinazione o alla residenza o alla data evento eliminazione" +
                            (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO ? " o al tipo calcolo" : string.Empty) + ". Si prega di verificare i redditi inseriti";
                        areaRedditi.Esito = TipoRitornoRedditi.Informativa;
                    }
                }
            }

            return true;
        }

        public static bool VerifyRedditiByDatiPensione(ref ContenitoreObject contenitore, string matricolaOperatore, short sedeOperatore, bool IsSalvataggio, AreaRedditi areaRedditiOriginale, GestionePensione.DatiEliminazione datiEliminazione, out AreaRedditi areaRedditi)
        {
            areaRedditi = null;
            string Url = "";
            ChiavePensione WsChiavePensione = null;
            Output_VerificaPresRedditi WsRisposta = null;
            ValorizzaRichiestaSrvRedditiPerVerify(ref contenitore, matricolaOperatore, sedeOperatore, out WsChiavePensione, out Url);
            string errori = "";
            StringBuilder messaggioVideoStrBuilder = new StringBuilder();
            VerifyRedditiFromSrvRedditi(contenitore.DatiPensione.NDomus, WsChiavePensione, out WsRisposta, out errori);
            if (!String.IsNullOrEmpty(errori))
            {
                areaRedditi = new AreaRedditi();
                areaRedditi.Url = Url;
                areaRedditi.MessaggioVideo = errori;
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return false;
            }
            if (WsRisposta.ListaEsito != null && (WsRisposta.ListaEsito.Length != 1 || WsRisposta.ListaEsito[0].Codice != 0))
            {
                areaRedditi = new AreaRedditi();
                areaRedditi.Url = Url;
                foreach (Esito esito in WsRisposta.ListaEsito)
                {
                    if (esito != null)
                    {
                        messaggioVideoStrBuilder.Append(esito.Codice.ToString() + ": " + esito.Descrizione + ". ");
                    }
                }
                areaRedditi.MessaggioVideo = messaggioVideoStrBuilder.ToString();
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
                return true;
            }

            ValorizzaAreaRedditiFromSrvRedditi(WsRisposta, out areaRedditi);
            //if (areaRedditi.Esito != TipoRitornoRedditi.NessunErrore)
            //    return;
            ConfrontaRedditi(IsSalvataggio, areaRedditiOriginale, areaRedditi);

            string messaggioVideo = null;
            if (IsSalvataggio && areaRedditi != null && !GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(areaRedditi.ListaRedditi, (datiEliminazione != null) ? datiEliminazione.DataEvento : null, out messaggioVideo))
            {
                areaRedditi.MessaggioVideo = messaggioVideo;
                areaRedditi.Esito = TipoRitornoRedditi.Errore;
            }

            if (IsSalvataggio && areaRedditi != null && areaRedditi.Esito == TipoRitornoRedditi.NessunErrore)
            {
                BLCommon.GestioneRedditi.SalvaRedditiDRedd(contenitore.DatiPensione, areaRedditi.ListaRedditi);
            }

            if (areaRedditi != null)
                areaRedditi.Url = Url;

            return true;
        }

        public static bool EliminaRedditiByDatiPensione(ref ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            BLCommon.GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = contenitore.DatiQuadroRedditi;
            BLCommon.GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = contenitore.DatiQuadroRichiestaBonus;
            BLCommon.GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                BLCommon.GestioneRedditi.EliminaAllRedditiDRedd(datiPensione.Id);

                if ((tipoAppartenenza == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzione(datiPensione.Gruppo) && !(Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))) ||
                    (tipoAppartenenza == Utility.TipoAppartenenza.FS && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                     tipoFondo != null && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                {
                    datiQuadroRedditi.Tipo = 1;
                    datiQuadroRedditi.TabRedditi = 1;
                }
                else if (datiQuadroRedditi.Tipo.Value == 1)
                    datiQuadroRedditi.TabRedditi = 1;
                else if (datiQuadroRedditi.Tipo.Value == 2)
                    datiQuadroRedditi.TabRedditi = 0;

                BLCommon.GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);

                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;

                if (Utility.IsBonusBooking(datiPensione))
                {
                    if (datiPensione.Tipo == "0167") //BONUS 14°
                    {
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBooking" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out ctrl);
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingSedi" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out sediDaControllare);
                    }
                    else //BONUS 154
                    {
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out ctrl);
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154Sedi" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out sediDaControllare);
                    }
                }

                if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                   (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                    sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0')))) &&
                    Utility.IsRicostituzioneOrRiapertura(datiPensione, false) && (datiPensione.Prodotto == "0101" || datiPensione.Prodotto == "0301" || datiPensione.Prodotto == "0401"))
                {
                    //Al variare dei redditi si deve rendere obbligatoria la riacquisizione degli anni bonus
                    BLCommon.GestioneAnniRichiestaBonus.EliminaAnniRichiestaBonusByIdPensione(datiPensione.Id);
                    datiQuadroRichiestaBonus.TabRichiestaBonus = 0;
                    BLCommon.GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, datiQuadroRichiestaBonus);
                }

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiQuadroRedditi = datiQuadroRedditi;
            //--------------------------------------------------------------------
            return true;
        }

        public static bool ElimninaRedditiSrvRedditiByDatiPensione(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            ChiavePensione WsChiavePensione = null;
            RecuperaChiavePensione(datiPensione, out WsChiavePensione);

            if (!EliminaRedditiFromSrvRedditi(datiPensione.NDomus, WsChiavePensione, out errori))
                return false;

            return true;
        }

        public static bool VerificaPresenzaRedditi(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            return VerificaPresenzaRedditi(datiPensione.NDomus.ToString(), out errori);
        }

        #endregion public members

        #region nested class
        public class AreaRedditi
        {
            #region private properties
            private List<BLCommon.GestioneRedditi.RedditoDRedd> _ListaRedditi;
            private DateTime _UltimaModifica;
            private bool _StatoPensione;
            private TipoRitornoRedditi _Esito;
            private string _MessaggioVideo;
            private string _Url;
            #endregion private properties

            #region public properties
            public List<BLCommon.GestioneRedditi.RedditoDRedd> ListaRedditi { get { return _ListaRedditi; } set { _ListaRedditi = value; } }
            public DateTime UltimaModifica { get { return _UltimaModifica; } set { _UltimaModifica = value; } }
            public bool StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } }
            public TipoRitornoRedditi Esito { get { return _Esito; } set { _Esito = value; } }
            public string MessaggioVideo { get { return _MessaggioVideo; } set { _MessaggioVideo = value; } }
            public string Url { get { return _Url; } set { _Url = value; } }
            #endregion public properties
        }

        public enum TipoRitornoRedditi
        {
            NessunErrore,
            Errore,
            Informativa
        };

        [Serializable]
        private class AreaInputDatiSQL
        {
            public AreaInputDatiSQL(Operatore WsOperatore, Titolare WsTitolare, ListaFamiliari WsListaFamiliari, Pensione WsPensione)
            {
                this.WsListaFamiliari = WsListaFamiliari;
                this.WsOperatore = WsOperatore;
                this.WsPensione = WsPensione;
                this.WsTitolare = WsTitolare;
            }

            public Operatore WsOperatore { get; set; }
            public Titolare WsTitolare { get; set; }
            public ListaFamiliari WsListaFamiliari { get; set; }
            public Pensione WsPensione { get; set; }
        }
        #endregion nested class
    }
}
