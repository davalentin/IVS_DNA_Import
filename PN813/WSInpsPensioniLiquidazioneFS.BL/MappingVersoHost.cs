using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class MappingVersoHost
    {
        #region public members
        public static void ValorizzaAnagrafica(string matricolaOperatore, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo,
            GestioneLavorazione.DatiLavorazione datiLavorazione, ref Data.FSPL_FSRC AreaCalcolo,
            out GestionePagamento.DatiPagamento datiPagamento, out List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, out GestionePensione.DatiPatronato datiPatronato,
            out List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili, out GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out GestioneFondo.DatiFondo datiFondo,
            out GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out Object objectFondoXX,
            out GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, out GestioneAnagrafica.DatiAnagrafici datiAnagraficiDelegato, out GestioneAnagrafica.DatiAnagrafici datiAnagraficiTutore)
        {
            datiPagamento = null;
            listaRecordFondo = null;
            datiPatronato = null;
            listaStatiCivili = null;
            datiIstruttoria = null;
            datiFondo = null;
            datiDanteCausa = null;
            datiAnagraficiTitolare = null;

            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out listaRecordFondo);

            GestioneFondo.DatiFondoEL datiFondoEL = null;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoET datiFondoET = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            List<GestioneFondo.DatiFondoFST> listaDatiFondoFS = null;
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
            GestioneFondo.DatiFondoPI datiFondoPI = null;
            GestioneFondo.DatiFondoCL datiFondoCL = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoPM datiFondoPM = null;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiFondoINPDAP = null;
            datiAnagraficiDelegato = null;
            datiAnagraficiTutore = null;

            objectFondoXX = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                        GestioneFondo.GetFondoELByIdPensione(datiPensione.Id, out datiFondoEL);
                        objectFondoXX = datiFondoEL;
                        break;
                    case Utility.TipoFondo.TT:
                        GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                        objectFondoXX = datiFondoTT;
                        break;
                    case Utility.TipoFondo.ET:
                        GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiFondoET);
                        objectFondoXX = datiFondoET;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);
                        objectFondoXX = datiFondoVL;
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.GetFondoFSRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoFS);
                        objectFondoXX = listaDatiFondoFS;
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.GetFondoPTRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoPT);
                        objectFondoXX = listaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.GetFondoPIByIdPensione(datiPensione.Id, out datiFondoPI);
                        objectFondoXX = datiFondoPI;
                        break;
                    case Utility.TipoFondo.CL:
                        GestioneFondo.GetFondoCLByIdPensione(datiPensione.Id, out datiFondoCL);
                        objectFondoXX = datiFondoCL;
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        objectFondoXX = datiFondoES;
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.GetFondoDZByIdPensione(datiPensione.Id, out datiFondoDZ);
                        objectFondoXX = datiFondoDZ;
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        objectFondoXX = datiFondoGAS;
                        break;
                    case Utility.TipoFondo.PM:
                        GestioneFondo.GetFondoPMByIdPensione(datiPensione.Id, out datiFondoPM);
                        objectFondoXX = datiFondoPM;
                        break;
                }
            }
            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoINPDAP);
                objectFondoXX = listaDatiFondoINPDAP;

                GestioneDelegatoTutore.GetDelegatoByIdPensione(datiPensione.Id, out datiAnagraficiDelegato);
                GestioneDelegatoTutore.GetTutoreByIdPensione(datiPensione.Id, out datiAnagraficiTutore);
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = datiLavorazione != null ? Utility.IsRiaperturaDomanda(datiLavorazione.CodFase) : false;

            if (datiAnagraficiTitolare != null)
            {
                string data = "";
                INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Anagrafica anagrafica = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Anagrafica();
                anagrafica.TRATIPOR = "A";
                anagrafica.TRACOFIS = datiAnagraficiTitolare.CodiceFiscale;
                anagrafica.TRACONOM = GetCognomeNomeTagliato(datiAnagraficiTitolare.Cognome.Trim() + "/" + datiAnagraficiTitolare.Nome.Trim());
                anagrafica.TRAACQUI = !string.IsNullOrEmpty(datiAnagraficiTitolare.CognomeAcquisito) ?
                    datiAnagraficiTitolare.CognomeAcquisito.Length > 16 ? datiAnagraficiTitolare.CognomeAcquisito.Substring(0, 16) : datiAnagraficiTitolare.CognomeAcquisito : string.Empty;
                anagrafica.TRASESSO = datiAnagraficiTitolare.Sesso.HasValue ? datiAnagraficiTitolare.Sesso.Value.ToString() : "";
                anagrafica.TRAGGNAS = datiAnagraficiTitolare.DataNascita.HasValue ? (short)datiAnagraficiTitolare.DataNascita.Value.Day : (short)0;
                anagrafica.TRAMMNAS = datiAnagraficiTitolare.DataNascita.HasValue ? (short)datiAnagraficiTitolare.DataNascita.Value.Month : (short)0;
                anagrafica.TRAAANAS = datiAnagraficiTitolare.DataNascita.HasValue ? (short)datiAnagraficiTitolare.DataNascita.Value.Year : (short)0;

                if (datiAnagraficiTitolare.ResidenzaEstero.HasValue && datiAnagraficiTitolare.ResidenzaEstero.Value)
                {
                    anagrafica.TRACORES = datiAnagraficiTitolare.FrazioneResidenza;
                    if (string.IsNullOrEmpty(anagrafica.TRACORES))
                        anagrafica.TRACORES = datiAnagraficiTitolare.ComuneResidenza;
                    if (!string.IsNullOrEmpty(anagrafica.TRACORES) && anagrafica.TRACORES.Length > 22)
                        anagrafica.TRACORES = anagrafica.TRACORES.Substring(0, 22);
                    anagrafica.TRAPRNAS = 96;
                }
                else
                {
                    anagrafica.TRACORES = datiAnagraficiTitolare.ComuneResidenza;
                    if (!string.IsNullOrEmpty(anagrafica.TRACORES) && anagrafica.TRACORES.Length > 22)
                        anagrafica.TRACORES = anagrafica.TRACORES.Substring(0, 22);
                    short codProvNascita = 0;
                    GetCodiceProvinciaNascita(datiAnagraficiTitolare.ProvinciaNascita, out codProvNascita);
                    anagrafica.TRAPRNAS = codProvNascita;
                }
                if (!string.IsNullOrEmpty(datiAnagraficiTitolare.CAP) && datiAnagraficiTitolare.CAP.Length > 5)
                    datiAnagraficiTitolare.CAP = datiAnagraficiTitolare.CAP.Substring(0, 5);
                int resInt = 0;
                int.TryParse(datiAnagraficiTitolare.CAP, out resInt);
                anagrafica.TRACAPPP = resInt;
                anagrafica.TRAINDIR = datiAnagraficiTitolare.Indirizzo + " " + datiAnagraficiTitolare.NCivico;
                if (!string.IsNullOrEmpty(anagrafica.TRAINDIR) && anagrafica.TRAINDIR.Length > 32)
                    anagrafica.TRAINDIR = anagrafica.TRAINDIR.Substring(0, 32);
                anagrafica.TRARSEST = datiAnagraficiTitolare.ResidenzaEstero.HasValue ? (datiAnagraficiTitolare.ResidenzaEstero.Value ? (short)1 : (short)0) : (short)0;
                anagrafica.TRAPRRES = datiAnagraficiTitolare.ProvinciaResidenza;
                Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare;
                GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);
                if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                {
                    anagrafica.TRACIVIL = areaTitolare.ElencoStatiCivili[areaTitolare.ElencoStatiCivili.Count - 1].Codice.ToString();
                    listaStatiCivili = areaTitolare.ElencoStatiCivili;
                }
                else
                    anagrafica.TRACIVIL = datiAnagraficiTitolare.CodiceStatoCivile.HasValue ? datiAnagraficiTitolare.CodiceStatoCivile.Value.ToString() : "";
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    anagrafica.TRACATEG = !string.IsNullOrEmpty(datiPensione.SiglaCategoria) ? string.Format("{0}{1}", datiPensione.SiglaCategoria.Trim().Substring(0, 1), datiPensione.SiglaCategoria.Trim().Substring(2)) : string.Empty;
                else
                    anagrafica.TRACATEG = !string.IsNullOrEmpty(datiPensione.SiglaCategoria) ? datiPensione.SiglaCategoria.Trim() : string.Empty;
                anagrafica.TRACAUSA = datiPensione.CausaCarico.HasValue ? datiPensione.CausaCarico.Value : (short)0;
                anagrafica.TRACFSIT = "0";

                if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.FS) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                {
                    if (Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione, isRiaperturaDomanda))
                        anagrafica.TRACNTOP = datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault();
                    else
                        anagrafica.TRACNTOP = datiPensione.CentroOperativo.GetValueOrDefault();
                }
                else
                    anagrafica.TRACNTOP = datiPensione.CentroOperativo.GetValueOrDefault();

                anagrafica.TRAISOLA = 0;
                anagrafica.TRANUMDO = datiPensione.NDomus;
                anagrafica.TRASELIQ = datiPensione.CodiceSede;
                resInt = 0;
                int.TryParse(matricolaOperatore.Trim(), out resInt);
                anagrafica.TRAMATRI = resInt;
                data = datiPensione.DataPresentazioneDomanda.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DataPresentazioneDomanda.Month.ToString().PadLeft(2, '0') + datiPensione.DataPresentazioneDomanda.Day.ToString().PadLeft(2, '0');
                anagrafica.TRAPRESE = int.Parse(data);
                if (datiPensione.DataPerfezionamentoRequisiti.HasValue)
                {
                    data = datiPensione.DataPerfezionamentoRequisiti.Value.Year.ToString().PadLeft(4, '0') +
                            datiPensione.DataPerfezionamentoRequisiti.Value.Month.ToString().PadLeft(2, '0') +
                            datiPensione.DataPerfezionamentoRequisiti.Value.Day.ToString().PadLeft(2, '0');
                    anagrafica.TRANRPAT = int.Parse(data);
                }

                if (datiPensione.RequisitiAl1294.HasValue && datiPensione.RequisitiAl1294.Value)
                {
                    if (datiPensione.RequisitiAl996.HasValue && datiPensione.RequisitiAl996.Value)
                        anagrafica.TRAREQU1 = "2";
                    else
                        anagrafica.TRAREQU1 = "4";
                }
                else
                {
                    if (datiPensione.RequisitiAl996.HasValue && datiPensione.RequisitiAl996.Value)
                        anagrafica.TRAREQU1 = "1";
                    else
                        anagrafica.TRAREQU1 = "3";
                }

                if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.FS) && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                {
                    if (Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione, isRiaperturaDomanda))
                        anagrafica.TRACEDUT = datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault();
                    else
                        anagrafica.TRACEDUT = datiPensione.CentroOperativo.GetValueOrDefault();
                }
                else
                    anagrafica.TRACEDUT = datiPensione.CentroOperativo.HasValue ? datiPensione.CentroOperativo.Value : (short)0;

                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquiqate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquiqate);
                //if (datiNuoveLiquiqate != null)
                //    anagrafica.TRACEDUT = datiNuoveLiquiqate.CodiceProcesso.GetValueOrDefault();

                List<GestioneDecodifica.CodiceEliminazione> lstDecCodiceEliminazione;
                GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodiceEliminazione, Utility.TipoAppartenenza.FS);

                GestionePensione.DatiEliminazione datiEliminazione = null;
                GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
                if (datiEliminazione != null)
                {
                    if (datiEliminazione.CodiceMotivo.HasValue)
                    {
                        GestioneDecodifica.CodiceEliminazione codiceEliminazione = lstDecCodiceEliminazione.Find(x => x.Id == datiEliminazione.CodiceMotivo.Value.ToString());
                        if (codiceEliminazione != null)
                            anagrafica.TRACODEL = codiceEliminazione.TraduzioneSuGP.Value.ToString();
                    }

                    data = datiEliminazione.DecorrenzaEliminazione.HasValue ? datiEliminazione.DecorrenzaEliminazione.Value.Year.ToString().PadLeft(4, '0') +
                        datiEliminazione.DecorrenzaEliminazione.Value.Month.ToString().PadLeft(2, '0') : "0";
                    anagrafica.TRADECEL = int.Parse(data);
                    //11-05-2012: TRACNTEL = TRADECEL (data eliminazione contabile = decorrenza eliminazione)
                    anagrafica.TRACNTEL = anagrafica.TRADECEL;
                    data = datiEliminazione.DataEvento.HasValue ? datiEliminazione.DataEvento.Value.Year.ToString().PadLeft(4, '0') +
                        datiEliminazione.DataEvento.Value.Month.ToString().PadLeft(2, '0') + datiEliminazione.DataEvento.Value.Day.ToString().PadLeft(2, '0') : "0";
                    anagrafica.TRADATEV = int.Parse(data);
                }

                if (datiPensione.TipoCalcolo.HasValue && !Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
                {
                    byte? traduzioneSuGpTipoCalcolo = Utility.GetTraduzioneSuGpTipoCalcolo(datiPensione);
                    anagrafica.TRATIPCALC = traduzioneSuGpTipoCalcolo.HasValue ? traduzioneSuGpTipoCalcolo.Value.ToString() : "";
                }

                data = datiPensione.DataInteressiLegali.HasValue ? datiPensione.DataInteressiLegali.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DataInteressiLegali.Value.Month.ToString().PadLeft(2, '0') + datiPensione.DataInteressiLegali.Value.Day.ToString().PadLeft(2, '0') : "0";
                anagrafica.TRAINTLG = int.Parse(data);
                anagrafica.TRATRLAV = datiPensione.CodiceArretrati.HasValue ? datiPensione.CodiceArretrati.Value : (short)0;
                anagrafica.TRAAAPAG = datiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)datiPensione.DecorrenzaCalcoloArretrati.Value.Year : (short)0;
                anagrafica.TRAMMPAG = datiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)datiPensione.DecorrenzaCalcoloArretrati.Value.Month : (short)0;

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.EL:
                            if (datiFondoEL != null)
                            {
                                anagrafica.TRACODSI = datiFondoEL.Requisiti247_243.HasValue ? datiFondoEL.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoEL.AnzianitaAnni.HasValue ? datiFondoEL.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoEL.AnnoRequisiti.HasValue ? datiFondoEL.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoEL.NumeroTriSemRequisiti.HasValue ? datiFondoEL.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.TT:
                            if (datiFondoTT != null)
                            {
                                anagrafica.TRACODSI = datiFondoTT.Requisiti247_243.HasValue ? datiFondoTT.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoTT.AnzianitaAnni.HasValue ? datiFondoTT.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoTT.AnnoRequisiti.HasValue ? datiFondoTT.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoTT.NumeroTriSemRequisiti.HasValue ? datiFondoTT.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.ET:
                            if (datiFondoET != null)
                            {
                                anagrafica.TRACODSI = datiFondoET.Requisiti247_243.HasValue ? datiFondoET.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoET.AnzianitaAnni.HasValue ? datiFondoET.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoET.AnnoRequisiti.HasValue ? datiFondoET.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoET.NumeroTriSemRequisiti.HasValue ? datiFondoET.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.VL:
                            if (datiFondoVL != null)
                            {
                                anagrafica.TRACODSI = datiFondoVL.Requisiti247_243.HasValue ? datiFondoVL.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoVL.AnzianitaAnni.HasValue ? datiFondoVL.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoVL.AnnoRequisiti.HasValue ? datiFondoVL.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoVL.NumeroTriSemRequisiti.HasValue ? datiFondoVL.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.PT:
                            if (listaDatiFondoPT != null && listaDatiFondoPT.Count > 0)
                            {
                                anagrafica.TRACODSI = listaDatiFondoPT.FirstOrDefault().RequisitiAnte247.HasValue ? listaDatiFondoPT.FirstOrDefault().RequisitiAnte247.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = listaDatiFondoPT.FirstOrDefault().AnzianitaAnni.HasValue ? listaDatiFondoPT.FirstOrDefault().AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((listaDatiFondoPT.FirstOrDefault().AnnoRequisiti.HasValue ? listaDatiFondoPT.FirstOrDefault().AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (listaDatiFondoPT.FirstOrDefault().TrimesteRequisiti.HasValue ? listaDatiFondoPT.FirstOrDefault().TrimesteRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.FS:
                            if (listaDatiFondoFS != null && listaDatiFondoFS.Count > 0)
                            {
                                anagrafica.TRACODSI = listaDatiFondoFS.FirstOrDefault().RequisitiAnte247.HasValue ? listaDatiFondoFS.FirstOrDefault().RequisitiAnte247.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = listaDatiFondoFS.FirstOrDefault().AnzianitaAnni.HasValue ? listaDatiFondoFS.FirstOrDefault().AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((listaDatiFondoFS.FirstOrDefault().AnnoRequisiti.HasValue ? listaDatiFondoFS.FirstOrDefault().AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (listaDatiFondoFS.FirstOrDefault().TrimesteRequisiti.HasValue ? listaDatiFondoFS.FirstOrDefault().TrimesteRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.PI:
                        case Utility.TipoFondo.PL:
                            if (datiFondoPI != null)
                            {
                                anagrafica.TRACODSI = datiFondoPI.Requisiti247_243.HasValue ? datiFondoPI.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoPI.AnzianitaAnni.HasValue ? datiFondoPI.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoPI.AnnoRequisiti.HasValue ? datiFondoPI.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoPI.NumeroTriSemRequisiti.HasValue ? datiFondoPI.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.GAS:
                            if (datiFondoGAS != null)
                            {
                                anagrafica.TRACODSI = datiFondoGAS.Requisiti247_243.HasValue ? datiFondoGAS.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoGAS.AnzianitaAnni.HasValue ? datiFondoGAS.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoGAS.AnnoRequisiti.HasValue ? datiFondoGAS.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoGAS.NumeroTriSemRequisiti.HasValue ? datiFondoGAS.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.DZ:
                            if (datiFondoDZ != null)
                            {
                                anagrafica.TRACODSI = datiFondoDZ.Requisiti247_243.HasValue ? datiFondoDZ.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoDZ.AnzianitaAnni.HasValue ? datiFondoDZ.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoDZ.AnnoRequisiti.HasValue ? datiFondoDZ.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoDZ.NumeroTriSemRequisiti.HasValue ? datiFondoDZ.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.ES:
                            if (datiFondoES != null)
                            {
                                anagrafica.TRACODSI = datiFondoES.Requisiti247_243.HasValue ? datiFondoES.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoES.AnzianitaAnni.HasValue ? datiFondoES.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoES.AnnoRequisiti.HasValue ? datiFondoES.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoES.NumeroTriSemRequisiti.HasValue ? datiFondoES.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                        case Utility.TipoFondo.PM:
                            if (datiFondoPM != null)
                            {
                                //anagrafica.TRACODSI = datiFondoPM.Requisiti247_243.HasValue ? datiFondoPM.Requisiti247_243.Value ? "S" : "N" : "";
                                anagrafica.TRAANZ247 = datiFondoPM.AnzianitaAnni.HasValue ? datiFondoPM.AnzianitaAnni.Value.ToString().PadLeft(2, '0') : "00";
                                //TRARECUP occorre passarlo al calcolo sempre a zero per le PL
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                                {
                                    anagrafica.TRARECUP = int.Parse((datiFondoPM.AnnoRequisiti.HasValue ? datiFondoPM.AnnoRequisiti.Value.ToString().PadLeft(4, '0') : "0000") +
                                      (datiFondoPM.NumeroTriSemRequisiti.HasValue ? datiFondoPM.NumeroTriSemRequisiti.Value.ToString() : "0"));
                                }
                            }
                            break;
                    }
                }

                if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                {
                    GestionePensione.DatiSindacato datiSindacato = null;
                    GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out datiSindacato);
                    if (datiSindacato != null && Utility.IsSindacatoPresente(datiSindacato.CodiceSindacato))
                    {
                        DateTime? decorrenzaSindacato = Utility.GetDecorrenzaPerSindacatoANPPE(datiSindacato.DecorrenzaSindacato, datiSindacato.CodiceSindacato);

                        anagrafica.TRACODSI1 = datiSindacato.CodiceSindacato;
                        anagrafica.TRADECSI = int.Parse((decorrenzaSindacato.HasValue ? decorrenzaSindacato.Value.Month.ToString().PadLeft(2, '0')
                             + decorrenzaSindacato.Value.Year.ToString().PadLeft(4, '0') : "0"));
                    }
                }

                GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
                GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);
                if (datiDetrazioni != null)
                {
                    anagrafica.TRADETR1 = datiDetrazioni.DetrazioniReddito.HasValue ? datiDetrazioni.DetrazioniReddito.Value : (short)0;
                    anagrafica.TRADETR2 = datiDetrazioni.AgevolazionePensionati.HasValue ? datiDetrazioni.AgevolazionePensionati.Value : (short)0;
                    anagrafica.TRADETR3 = datiDetrazioni.ConiugeOFiglio.HasValue ? datiDetrazioni.ConiugeOFiglio.Value : (short)0;
                    anagrafica.TRADETR4 = datiDetrazioni.FigliMinori3AnniNoHandicap100.HasValue ? datiDetrazioni.FigliMinori3AnniNoHandicap100.Value : (short)0;
                    anagrafica.TRADETR5 = datiDetrazioni.FigliMinori3AnniNoHandicap50.HasValue ? datiDetrazioni.FigliMinori3AnniNoHandicap50.Value : (short)0;
                    anagrafica.TRADETR6 = short.Parse((datiDetrazioni.FigliMinori3AnniHandicap100.HasValue ? datiDetrazioni.FigliMinori3AnniHandicap100.Value.ToString() : "0") +
                        (datiDetrazioni.FigliMinori3AnniHandicap50.HasValue ? datiDetrazioni.FigliMinori3AnniHandicap50.Value.ToString() : "0"));
                    anagrafica.TRADETR7 = short.Parse((datiDetrazioni.FigliMaggiori3AnniNoHandicap100.HasValue ? datiDetrazioni.FigliMaggiori3AnniNoHandicap100.Value.ToString() : "0") +
                        (datiDetrazioni.FigliMaggiori3AnniNoHandicap50.HasValue ? datiDetrazioni.FigliMaggiori3AnniNoHandicap50.Value.ToString() : "0"));
                    anagrafica.TRADETR8 = short.Parse((datiDetrazioni.FigliMaggiori3AnniHandicap100.HasValue ? datiDetrazioni.FigliMaggiori3AnniHandicap100.Value.ToString() : "0") +
                        (datiDetrazioni.FigliMaggiori3AnniHandicap50.HasValue ? datiDetrazioni.FigliMaggiori3AnniHandicap50.Value.ToString() : "0"));
                    anagrafica.TRADETR9 = short.Parse((datiDetrazioni.AltriFamiliari100.HasValue ? datiDetrazioni.AltriFamiliari100.Value.ToString() : "0") +
                        (datiDetrazioni.AltriFamiliari50.HasValue ? datiDetrazioni.AltriFamiliari50.Value.ToString() : "0"));
                    anagrafica.TRADET10 = datiDetrazioni.AddizionaleLombardiaVeneto.HasValue ? datiDetrazioni.AddizionaleLombardiaVeneto.Value : (short)0;
                }

                //ENG - Memo 48_2023
                if (Utility.IsTitolareResidente_Cittadino_Bulgaria(datiPensione, datiAnagraficiTitolare))
                {
                    anagrafica.TRADETR1 = 2;
                }

                // ENG - Memo 49_2023
                if (Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    anagrafica.TRADETR1 = 3;
                }

                GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);
                if (datiPagamento != null)
                {
                    anagrafica.TRAPGEST = datiPagamento.TipoPagamento.HasValue ? (datiPagamento.TipoPagamento.Value == 'E' ? (short)1 : (short)0) : (short)0;
                }

                DateTime? dataValidita = null;
                bool? IsDecorrenzaValida = Utility.ControllaDataDecorrenzaInferiore(datiPensione, Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione), datiPensione.DecorrenzaOriginaria, out dataValidita);
                if (IsDecorrenzaValida.HasValue && IsDecorrenzaValida.Value)
                    anagrafica.TRA562 = "NO";

                anagrafica.TRAINPDAI = "";
                anagrafica.TRAITER1 = "R    ";
                anagrafica.TRASECOM = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;
                data = datiPensione.DataPresentazioneDomanda.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DataPresentazioneDomanda.Month.ToString().PadLeft(2, '0') + datiPensione.DataPresentazioneDomanda.Day.ToString().PadLeft(2, '0');
                anagrafica.TRAACQU1 = int.Parse(data);

                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                    anagrafica.TRACERTI = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                else
                    anagrafica.TRACERTI = 0;

                if (datiPensione.SiglaCategoria.StartsWith("S"))
                {
                    if (datiDanteCausa != null)
                    {
                        if (anagrafica.TRACERTI == 0)
                            anagrafica.TRACERTI = datiDanteCausa.Certificato.GetValueOrDefault();

                        anagrafica.TRADIAAA = datiDanteCausa.DecorrenzaPensione.HasValue ? (short)datiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                        anagrafica.TRADIAMM = datiDanteCausa.DecorrenzaPensione.HasValue ? (short)datiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                    }
                }
                else
                {
                    anagrafica.TRADIAAA = 0;
                    anagrafica.TRADIAMM = 0;
                }
                anagrafica.TRASPAAA = 0;
                anagrafica.TRASPAMM = 0;

                if (datiPensione.SiglaCategoria.StartsWith("S"))
                {
                    GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                    if (datiDanteCausa != null)
                    {
                        anagrafica.TRADIFAA = datiDanteCausa.DecorrenzaPensione.HasValue ? (short)datiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                        anagrafica.TRADIFMM = datiDanteCausa.DecorrenzaPensione.HasValue ? (short)datiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                        anagrafica.TRADIFGG = datiDanteCausa.DecorrenzaPensione.HasValue ? (short)datiDanteCausa.DecorrenzaPensione.Value.Day : (short)0;
                    }
                    anagrafica.TRASPFAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    anagrafica.TRASPFMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    anagrafica.TRASPFGG = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                }
                else
                {
                    anagrafica.TRADIFAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    anagrafica.TRADIFMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    anagrafica.TRADIFGG = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                    anagrafica.TRASPFAA = 0;
                    anagrafica.TRASPFMM = 0;
                    anagrafica.TRASPFGG = 0;
                }

                if (datiIstruttoria != null)
                {
                    if (datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                    {
                        List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari = null;
                        GestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolari);
                        if (elencoCodiciParticolari != null && elencoCodiciParticolari.Count > 0)
                        {
                            long codicePart = datiIstruttoria.CodiceParticolareSoggettoDerogato.Value;
                            GestioneDecodifica.CodiceParticolare codiceParticolare = elencoCodiciParticolari.Find(x => x.Id == codicePart);
                            if (codiceParticolare != null)
                            {
                                if (codiceParticolare.TraduzioneSuGp.HasValue)
                                {
                                    if (codiceParticolare.TraduzioneSuGp.Value == '3')
                                    {
                                        if (Utility.IsDomandaUsuranti(datiPensione))
                                            anagrafica.TRAESODAN = "U";
                                        else if (Utility.IsDomandaSalvaguardia122(datiPensione))
                                            anagrafica.TRAESODAN = codiceParticolare.TraduzioneSuGp.Value.ToString();
                                        // Altrimenti non passo nulla
                                    }
                                    else
                                        anagrafica.TRAESODAN = codiceParticolare.TraduzioneSuGp.Value.ToString();
                                }
                            }
                        }
                    }

                    anagrafica.TRACDCOM1 = datiIstruttoria.CodiceComunicazioneCampo1.HasValue ? datiIstruttoria.CodiceComunicazioneCampo1.Value.ToString() +
                        (datiIstruttoria.CodiceComunicazioneCampo2.HasValue ? datiIstruttoria.CodiceComunicazioneCampo2.Value.ToString() : " ") :
                        datiIstruttoria.CodiceComunicazioneCampo2.HasValue ? " " + datiIstruttoria.CodiceComunicazioneCampo2.Value.ToString() : "";
                    anagrafica.TRACDCOM3 = datiIstruttoria.CodiceComunicazioneCampo3.HasValue ? datiIstruttoria.CodiceComunicazioneCampo3.Value.ToString() : "Y";
                    anagrafica.TRACDCOM4 = datiIstruttoria.CodiceComunicazioneCampo4.HasValue ? datiIstruttoria.CodiceComunicazioneCampo4.Value.ToString() : "";
                }

                if (string.IsNullOrEmpty(anagrafica.TRAESODAN))
                    if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
                        anagrafica.TRAESODAN = " O";

                if (datiFondo != null)
                {
                    char? codiceSpecifico = null;
                    List<GestioneDecodifica.CodiceSpecifico> listaCodiciSpecifici = null;
                    GestioneDecodifica.GetCodiceSpecifico(out listaCodiciSpecifici);
                    if (listaCodiciSpecifici != null && datiFondo.CodiceSpecifico.HasValue)
                    {
                        byte id = datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : (byte)0;
                        GestioneDecodifica.CodiceSpecifico codSpec = listaCodiciSpecifici.Find(x => x.Id == id);
                        if (codSpec != null)
                        {
                            anagrafica.TRAUFPAG = codSpec.TipoPensione.HasValue ? "  " + codSpec.TipoPensione.Value.ToString() : "";
                            codiceSpecifico = codSpec.TraduzioneGp;
                        }
                    }

                    //if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                    //{
                    //    anagrafica.TRAUFPAG = Utility.GetTipoPensioneForVolo(tipoFondo, datiAssicurativi.fondoVL != null ? datiAssicurativi.fondoVL.CodiceArt22 : (short?)null, codiceSpecifico).ToString();
                    //    if (!string.IsNullOrEmpty(anagrafica.TRAUFPAG))
                    //        anagrafica.TRAUFPAG = anagrafica.TRAUFPAG.PadLeft(3, ' ');
                    //}

                    //if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.CL)
                    //{
                    //    anagrafica.TRAUFPAG = Utility.GetTipoPensioneForFondoCL(datiPensione).ToString();
                    //    if (!string.IsNullOrEmpty(anagrafica.TRAUFPAG))
                    //        anagrafica.TRAUFPAG = anagrafica.TRAUFPAG.PadLeft(3, ' ');
                    //}

                    if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.VL || tipoFondo.Value == Utility.TipoFondo.CL))
                    {
                        anagrafica.TRAUFPAG = Utility.GetTipoPensioneForFondi(datiPensione, tipoFondo, codiceSpecifico).ToString();
                        if (!string.IsNullOrEmpty(anagrafica.TRAUFPAG))
                            anagrafica.TRAUFPAG = anagrafica.TRAUFPAG.PadLeft(3, ' ');
                    }

                    if (datiPensione.SiglaCategoria.StartsWith("S"))
                        anagrafica.TRAUFPAG = "  7";
                }

                if (datiFondoTT != null)
                {
                    anagrafica.TRADIMISSIONI = datiFondoTT.DimissioniAnte97.HasValue ? datiFondoTT.DimissioniAnte97.Value ? "SI" : "NO" : "";
                }
                else
                    anagrafica.TRADIMISSIONI = "";

                // Inserimento campo “Raggiunto requisiti al 31/12/1997”
                if (datiFondoDZ != null && datiFondoDZ.RaggiuntoRequisiti311297.HasValue)
                {
                    if (datiFondoDZ.RaggiuntoRequisiti311297.Value)
                        anagrafica.TRA562 = "SI";
                    else
                        anagrafica.TRA562 = "NO";
                }

                if (datiLavorazione != null)
                {
                    anagrafica.TRATIPIR = datiLavorazione.TipoReversibilita.HasValue ? datiLavorazione.TipoReversibilita.Value.ToString() : "";
                    anagrafica.TRATPLIQ = string.IsNullOrEmpty(datiLavorazione.TipoLiquidazione) && Utility.IsDomandaINPDAP(datiPensione.Gestione) ? string.Empty : datiLavorazione.TipoLiquidazione.Trim();
                    if (datiEliminazione != null && datiLavorazione.TipoLiquidazione != null)
                    {
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                            anagrafica.TRATPLIQ = anagrafica.TRATPLIQ.PadRight(3, ' ').Remove(1).Insert(1, "7");
                        else
                            anagrafica.TRATPLIQ = anagrafica.TRATPLIQ.PadRight(3, ' ').Remove(2).Insert(2, "E");
                    }
                }

                if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && (Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione))
                      && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione))
                {
                    anagrafica.TRATPLIQ = "B1";
                }

                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagraficiTitolare.CodiceComuneNascita, Utility.TipoAppartenenza.FS.ToString(), 0, false, out codiceInpsComune);
                anagrafica.TRACONAS = codiceInpsComune;


                GestionePensione.GetPatronatoByIdPensione(datiPensione.Id, out datiPatronato);
                if (datiPatronato != null)
                {
                    short resShort = 0;
                    short.TryParse(datiPatronato.CodiceEnte, out resShort);
                    anagrafica.TRACDPAT = resShort;
                }

                AreaCalcolo.AreaInputVariabile.ListaAnagrafica = new List<Data.CMSGTRA.Anagrafica> { anagrafica };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAnagrafica[0].TRATIPOR));
            }
        }

        public static void ValorizzaDelegato(GestionePensione.DatiPensione datiPensione, GestionePagamento.DatiPagamento datiPagamento,
            GestionePensione.DatiPatronato datiPatronato, Utility.TipoFondo? tipoFondo, GestioneLavorazione.DatiLavorazione datiLavorazione, GestioneFondo.DatiFondo datiFondo,
            out Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, ref Data.FSPL_FSRC AreaCalcolo)
        {
            datiMaggiorazioniBenefici = null;
            Data.CMSGTRA.DelegatoNew delegato = new Data.CMSGTRA.DelegatoNew();
            delegato.TRBTIPOR = "B";
            if (datiPagamento != null)
            {
                delegato.TRBIBAN = !string.IsNullOrEmpty(datiPagamento.IBAN) ? datiPagamento.IBAN.ToUpperInvariant() : string.Empty;
                delegato.TRBBIC = datiPagamento.BIC;
                delegato.TRBASTERISCHI = "**";
                delegato.TRBFILLER = "";
                delegato.TRBCOABI = datiPagamento.ABI.HasValue ? datiPagamento.ABI.Value : 0;
                if (datiPagamento.TipoPagamento.HasValue && datiPagamento.TipoPagamento.Value == 'P' && datiPagamento.ABI.GetValueOrDefault() == 07601)
                    delegato.TRBCOCAB = datiPagamento.Frazionario.HasValue ? datiPagamento.Frazionario.Value : 0;
                else
                    delegato.TRBCOCAB = datiPagamento.CAB.HasValue ? datiPagamento.CAB.Value : 0;

                delegato.TRBCOCON = 0;
                delegato.TRBCOVAL = "";
                delegato.TRBCOPAG = datiPagamento.ModalitaPagamento.HasValue ? datiPagamento.ModalitaPagamento.Value.ToString() : "";
                if (datiPagamento.TipoPagamento.HasValue && datiPagamento.TipoPagamento.Value == 'E')
                    delegato.TRBPAESE = "E";
                else
                    delegato.TRBPAESE = "I";
                if (datiPensione != null)
                {
                    delegato.TRBOLDEAD = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString() : "";
                }

                if (datiPagamento.TipoPagamento.HasValue && datiPagamento.TipoPagamento.Value == 'P' &&
                    datiPagamento.ModalitaPagamento.HasValue && datiPagamento.ModalitaPagamento.Value == 'L' &&
                    string.IsNullOrEmpty(delegato.TRBIBAN))
                    delegato.TRBIBAN = !string.IsNullOrEmpty(datiPagamento.Libretto) ? datiPagamento.Libretto.ToUpperInvariant() : string.Empty;
            }

            if (datiLavorazione != null)
                delegato.TRBFASE = !string.IsNullOrEmpty(datiLavorazione.CodFase) ? datiLavorazione.CodFase.PadLeft(4, '0').Substring(1, 3) : string.Empty;

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                switch (datiPensione.TipoFelpe.GetValueOrDefault())
                {
                    case 1:
                        delegato.TRBUNICARPE = "2";
                        break;
                    case 2:
                        delegato.TRBUNICARPE = "6";
                        break;
                    case 3:
                        delegato.TRBUNICARPE = "5";
                        break;
                    default:
                        delegato.TRBUNICARPE = string.Empty;
                        break;
                }
            }
            else
            {
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                    ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.IsPLUnicarpe.GetValueOrDefault()))
                    delegato.TRBUNICARPE = "2";
                else
                    delegato.TRBUNICARPE = string.Empty;
            }

            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);
            if (datiMaggiorazioniBenefici != null)
            {
                delegato.TRBLG140 = datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue ?
                    int.Parse(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.Value.Year.ToString().PadLeft(4, '0') +
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.Value.Month.ToString().PadLeft(2, '0')) : 0;
                delegato.TRBSEN140 = datiMaggiorazioniBenefici.CodiceCieco.HasValue ? (short)datiMaggiorazioniBenefici.CodiceCieco : (short)0;
            }

            if (datiPatronato != null)
            {
                delegato.TRBTIPOENTEPAT = datiPatronato.CodiceEnte;
                if (!string.IsNullOrEmpty(datiPatronato.TipoUfficio))
                    delegato.TRBTIPOUFFPAT = datiPatronato.TipoUfficio.Trim() == "02" ? "01" : datiPatronato.TipoUfficio.Trim() == "23" ? "02" : "";
                delegato.TRBUFFZONALE = datiPatronato.CodiceUfficio;
                delegato.TRBNUMPRATICA = !string.IsNullOrEmpty(datiPatronato.NPratica) ? datiPatronato.NPratica.Length <= 8 ? datiPatronato.NPratica : datiPatronato.NPratica.Substring(0, 8) : string.Empty;
            }

            if (datiFondo != null)
            {
                delegato.TRBBONUS = datiFondo.AttribuzioneBonus.HasValue ? datiFondo.AttribuzioneBonus.Value ? "SI" : "NO" : "";
                delegato.TRBMESEDALBONUS = datiFondo.InizioBonus.HasValue ? (short)datiFondo.InizioBonus.Value.Month : (short)0;
                delegato.TRBANNODALBONUS = datiFondo.InizioBonus.HasValue ? (short)datiFondo.InizioBonus.Value.Year : (short)0;
                delegato.TRBMESEALBONUS = datiFondo.InizioBonus.HasValue ? (short)datiFondo.InizioBonus.Value.Month : (short)0;
                delegato.TRBANNOALBONUS = datiFondo.InizioBonus.HasValue ? (short)datiFondo.InizioBonus.Value.Year : (short)0;
            }

            List<GestioneRipartizioneFondi.DatiRipartizioneFondi> LdatiRipartizioneFondi = null;
            GestioneRipartizioneFondi.GetRipartizioneFondiByIdPensione(datiPensione.Id, out LdatiRipartizioneFondi);

            if (LdatiRipartizioneFondi != null && LdatiRipartizioneFondi.Count > 0)
            {
                foreach (GestioneRipartizioneFondi.DatiRipartizioneFondi onTer in LdatiRipartizioneFondi)
                {
                    if (onTer.Progressivo.HasValue && onTer.Progressivo.Value == 1)
                        delegato.TRBONERI1 = onTer.Importo.HasValue ? onTer.Importo.Value : 0M;
                    else if (onTer.Progressivo.HasValue && onTer.Progressivo.Value == 2)
                        delegato.TRBONERI2 = onTer.Importo.HasValue ? onTer.Importo.Value : 0M;
                    else if (onTer.Progressivo.HasValue && onTer.Progressivo.Value == 3)
                        delegato.TRBONERI3 = onTer.Importo.HasValue ? onTer.Importo.Value : 0M;
                }
            }

            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);
            if (datiNuoveLiquidate != null)
            {
                //ENG - Pensioni Ovunque (Sul campo TRBPROCESSO va inviato sempre il codice processo di WebDom)
                delegato.TRBPROCESSO = datiNuoveLiquidate.CodiceProcesso.GetValueOrDefault().ToString().PadLeft(3, '0');
            }

            if (Utility.IsTelematica(datiPensione.CodiceProcedura))
                delegato.TRBTELEM = "T";

            if (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0105" && datiPensione.Tipo == "0116")
                delegato.TRBSENTI = "S";

            if (delegato != null)
            {

                delegato.TRBINPDAP = datiPagamento != null && datiPagamento.TrattenutaInpdap.HasValue ? datiPagamento.TrattenutaInpdap.Value ? "SI" : "NO" : string.Empty;
                delegato.TRBMESEINPDAP = datiPagamento != null && datiPagamento.DataRinunciaTrattenutaInpdap.HasValue ? (short)datiPagamento.DataRinunciaTrattenutaInpdap.Value.Month : (short)0;
                delegato.TRBANNOINPDAP = datiPagamento != null && datiPagamento.DataRinunciaTrattenutaInpdap.HasValue ? (short)datiPagamento.DataRinunciaTrattenutaInpdap.Value.Year : (short)0; ;

                AreaCalcolo.AreaInputVariabile.ListaDelegato = new List<Data.CMSGTRA.DelegatoNew> { delegato };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaDelegato[0].TRBTIPOR));
            }
        }

        public static void ValorizzaFamiliareByIdPensione(GestionePensione.DatiPensione datiPensione, out Dictionary<string, char> componentiFamiliari, ref Data.FSPL_FSRC AreaCalcolo)
        {
            componentiFamiliari = new Dictionary<string, char>();
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);
            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
            GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(datiPensione.Id, out listaCodMaggFamiliari);
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            //DateTime? decorrenzaPrimoConiuge = null;
            long idAnagraficaPrimoConiuge = 0;
            if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.Any(x => x.IsConiugeOrUnitoCivile()) &&
                listaCodMaggFamiliari != null && listaCodMaggFamiliari.Count > 0)
            {
                var listaAppoggio = listaCodMaggFamiliari.FindAll(x => listaFamiliari.FindAll(y => y.IsConiugeOrUnitoCivile()).Select(y => y.IdAnagrafica).ToList().Contains(x.IdAnagrafica));
                idAnagraficaPrimoConiuge = listaAppoggio != null && listaAppoggio.Count > 0 ? listaAppoggio.OrderBy(x => x.Decorrenza).FirstOrDefault().IdAnagrafica : 0;
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFamiliare = new List<Data.CMSGTRA.Familiare>();
                short record = 0;
                short indicePRFAM = 0;
                List<char> codiciPRFAM = new List<char> { 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S' };
                codiciPRFAM.RemoveAll(x => listaFamiliari.Exists(y => y.Progressivo == x));

                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare familiare = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare();

                    GestioneAnagrafica.DatiAnagrafici datiAnagFam = listaAnagrafiche.Find(x => x.CodiceFiscale == fam.CodiceFiscale);

                    familiare.TRCTIPOR = "C";

                    //gestione unioni civile, se familiare è C= coniuge e tipo unione è U = unione civile, allora TRCCODFM = 7 = unito civile
                    // altrimenti TRCCODFM si imposta il valore trovato
                    if (fam.SiglaFamiliare.HasValue && fam.SiglaFamiliare.Value.ToString() == "C" && fam.TipoUnione == "U")
                        familiare.TRCCODFM = "7";
                    else
                        familiare.TRCCODFM = fam.SiglaFamiliare.HasValue ? fam.SiglaFamiliare.Value.ToString() : "";
                    //Eng - unione civile TRCPRFAM = "B" come per TipoUnione == "M"
                    if ((familiare.TRCCODFM == "C" || familiare.TRCCODFM == "7") && idAnagraficaPrimoConiuge > 0 && fam.IdAnagrafica > 0 && idAnagraficaPrimoConiuge == fam.IdAnagrafica && !listaFamiliari.Exists(y => y.IsConiugeOrUnitoCivile() && y.Progressivo == 'B')) //Entro in questo if solamente se il familiare è il primo coniuge e non è presente già un coniuge con progressivo B                 
                        familiare.TRCPRFAM = "B"; //per il primo coniuge imposto B se non è presente già un coniuge con progressivo B                     
                    else if (indicePRFAM > codiciPRFAM.Count)
                        throw new INPS.DNA.DnaApplicationException("Numero familiari differenti dal coniuge superiore a " + codiciPRFAM.Count);
                    else if (fam.Progressivo.HasValue)
                        familiare.TRCPRFAM = fam.Progressivo.Value.ToString();
                    else
                    {
                        componentiFamiliari.Add(fam.CodiceFiscale, codiciPRFAM[indicePRFAM]);
                        familiare.TRCPRFAM = codiciPRFAM[indicePRFAM].ToString();
                        indicePRFAM++;
                    }

                    bool searchPeriodiTitolare = false;
                    bool isTitolare = AreaCalcolo.AreaInputVariabile.ListaAnagrafica[0].TRACOFIS == fam.CodiceFiscale;

                    if (listaCodMaggFamiliari != null)
                    {
                        List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliariParziali =
                            listaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica && x.IdPensione == fam.IdPensione);
                        if (listaCodMaggFamiliariParziali != null && listaCodMaggFamiliariParziali.Count > 0)
                        {

                            familiare.LISTTRCCONTI = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI>();
                            for (int i = 0; i < listaCodMaggFamiliariParziali.Count; i++)
                            {
                                if (listaCodMaggFamiliariParziali[i].Decorrenza.HasValue || listaCodMaggFamiliariParziali[i].Cessazione.HasValue)
                                {
                                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI trcConti = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI();
                                    trcConti.TRCDECAA = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Year : (short)0;
                                    trcConti.TRCDECMM = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Month : (short)0;
                                    trcConti.TRCSOSAA = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Year : (short)0;
                                    trcConti.TRCSOSMM = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Month : (short)0;
                                    if (categoriaFondoPI != null)
                                    {
                                        trcConti.TRCQUOTA = listaCodMaggFamiliariParziali[i].QuotaAF != null ? listaCodMaggFamiliariParziali[i].QuotaAF : null;
                                        trcConti.TRCDIRAF = listaCodMaggFamiliariParziali[i].DirittoAF != null ? listaCodMaggFamiliariParziali[i].DirittoAF : "";
                                        trcConti.TRCCNFON = listaCodMaggFamiliariParziali[i].ContitolaritaFondo != null ? listaCodMaggFamiliariParziali[i].ContitolaritaFondo : null;
                                        trcConti.TRCCNAGO = listaCodMaggFamiliariParziali[i].ContitolaritaAgo != null ? listaCodMaggFamiliariParziali[i].ContitolaritaAgo : null;
                                    }
                                   
                                    familiare.LISTTRCCONTI.Add(trcConti);
                                }
                            }
                          
                        }
                        else if (isTitolare)
                            searchPeriodiTitolare = true;
                    }
                    else if (isTitolare)
                        searchPeriodiTitolare = true;
                    if (searchPeriodiTitolare && Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    {
                        List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                        List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheAventiDiritto = null;
                        GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagraficheAventiDiritto);
                        if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                        {
                            List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDiritto = null;
                            GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, null, out listaPeriodiAventiDiritto);

                            List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDirittoParziali = null;
                            GestioneAventiDiritto.AventiDiritto aventeDiritto = listaAventiDiritto.Find(x => x.IdAnagrafica == fam.IdAnagrafica);
                            if (aventeDiritto != null && listaPeriodiAventiDiritto != null)
                                listaPeriodiAventiDirittoParziali = listaPeriodiAventiDiritto.FindAll(x => x.IdAventeDiritto == aventeDiritto.Id);

                            if (familiare.LISTTRCCONTI == null)
                                familiare.LISTTRCCONTI = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI>();
                            foreach (var periodo in listaPeriodiAventiDirittoParziali)
                            {
                                INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI trcConti = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Familiare.TRCCONTI();
                                trcConti.TRCDECAA = periodo.DecorrenzaPeriodo.HasValue ? (short)periodo.DecorrenzaPeriodo.Value.Year : (short)0;
                                trcConti.TRCDECMM = periodo.DecorrenzaPeriodo.HasValue ? (short)periodo.DecorrenzaPeriodo.Value.Month : (short)0;
                                trcConti.TRCSOSAA = periodo.CessazionePeriodo.HasValue ? (short)periodo.CessazionePeriodo.Value.Year : (short)9999;
                                trcConti.TRCSOSMM = periodo.CessazionePeriodo.HasValue ? (short)periodo.CessazionePeriodo.Value.Month : (short)99;
                                familiare.LISTTRCCONTI.Add(trcConti);
                            }
                        }
                    }

                    if (datiAnagFam != null)
                    {
                        familiare.TRCCONOM = GetCognomeNomeTagliato(datiAnagFam.Cognome.Trim() + "/" + datiAnagFam.Nome.Trim());
                        familiare.TRCCOFIS = datiAnagFam.CodiceFiscale;
                        familiare.TRCSESSO = datiAnagFam.Sesso.HasValue ? datiAnagFam.Sesso.Value.ToString() : "";
                        familiare.TRCPRREC = record;
                        familiare.TRCCOACQ = !string.IsNullOrEmpty(datiAnagFam.CognomeAcquisito) ?
                            datiAnagFam.CognomeAcquisito.Length > 16 ? datiAnagFam.CognomeAcquisito.Substring(0, 16) : datiAnagFam.CognomeAcquisito : string.Empty;
                        familiare.TRCAANAS = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Year : (short)0;
                        familiare.TRCMMNAS = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Month : (short)0;
                        familiare.TRCGGNAS = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Day : (short)0;

                        if (datiAnagFam.ResidenzaEstero.HasValue && datiAnagFam.ResidenzaEstero.Value)
                        {
                            familiare.TRCPRNAS = 96;
                        }
                        else
                        {
                            short codProvNascita = 0;
                            GetCodiceProvinciaNascita(datiAnagFam.ProvinciaNascita, out codProvNascita);
                            familiare.TRCPRNAS = codProvNascita;
                        }
                        int codInpsComune = 0;
                        GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagFam.CodiceComuneNascita, Utility.TipoAppartenenza.FS.ToString(), 0, false, out codInpsComune);
                        familiare.TRCCONAS = codInpsComune;
                    }

                    GestioneDetrazioniContitolare.DatiDetrazioniContitolare datiDetrazioniContitolare = null;
                    GestioneDetrazioniContitolare.GetDetrazioniBySoggetto(fam.IdPensione, fam.IdAnagrafica, out datiDetrazioniContitolare);
                    if (datiDetrazioniContitolare != null)
                    {
                        familiare.TRCDETR1 = datiDetrazioniContitolare.DetrazioniReddito.HasValue ? datiDetrazioniContitolare.DetrazioniReddito.Value : (short)0;
                        familiare.TRCDETR2 = datiDetrazioniContitolare.AgevolazionePensionati.HasValue ? datiDetrazioniContitolare.AgevolazionePensionati.Value : (short)0;
                        familiare.TRCDETR3 = datiDetrazioniContitolare.ConiugeOFiglio.HasValue ? datiDetrazioniContitolare.ConiugeOFiglio.Value : (short)0;
                        familiare.TRCDETR4 = datiDetrazioniContitolare.FigliMinori3AnniNoHandicap100.HasValue ? datiDetrazioniContitolare.FigliMinori3AnniNoHandicap100.Value : (short)0;
                        familiare.TRCDETR5 = datiDetrazioniContitolare.FigliMinori3AnniNoHandicap50.HasValue ? datiDetrazioniContitolare.FigliMinori3AnniNoHandicap50.Value : (short)0;
                        familiare.TRCDETR6 = short.Parse((datiDetrazioniContitolare.FigliMinori3AnniHandicap100.HasValue ? datiDetrazioniContitolare.FigliMinori3AnniHandicap100.Value.ToString() : "0") +
                            (datiDetrazioniContitolare.FigliMinori3AnniHandicap50.HasValue ? datiDetrazioniContitolare.FigliMinori3AnniHandicap50.Value.ToString() : "0"));
                        familiare.TRCDETR7 = short.Parse((datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap100.HasValue ? datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap100.Value.ToString() : "0") +
                            (datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap50.HasValue ? datiDetrazioniContitolare.FigliMaggiori3AnniNoHandicap50.Value.ToString() : "0"));
                        familiare.TRCDETR8 = short.Parse((datiDetrazioniContitolare.FigliMaggiori3AnniHandicap100.HasValue ? datiDetrazioniContitolare.FigliMaggiori3AnniHandicap100.Value.ToString() : "0") +
                            (datiDetrazioniContitolare.FigliMaggiori3AnniHandicap50.HasValue ? datiDetrazioniContitolare.FigliMaggiori3AnniHandicap50.Value.ToString() : "0"));
                        familiare.TRCDETR9 = short.Parse((datiDetrazioniContitolare.AltriFamiliari100.HasValue ? datiDetrazioniContitolare.AltriFamiliari100.Value.ToString() : "0") +
                            (datiDetrazioniContitolare.AltriFamiliari50.HasValue ? datiDetrazioniContitolare.AltriFamiliari50.Value.ToString() : "0"));
                        familiare.TRCDET10 = datiDetrazioniContitolare.AddizionaleLombardiaVeneto.HasValue ? datiDetrazioniContitolare.AddizionaleLombardiaVeneto.Value : (short)0;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFamiliare.Add(familiare);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFamiliare[0].TRCTIPOR));
                }
            }
        }

        public static void ValorizzaDanteCausaByIdPensione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            datiAnagraficiDanteCausa = null;
            List<GestioneDecodifica.CodiceEliminazione> lstDecCodiceEliminazione;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out lstDecCodiceEliminazione, Utility.TipoAppartenenza.FS);

            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DanteCausa danteCausa = null;

            if (datiDanteCausa != null)
            {
                danteCausa = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DanteCausa();
                danteCausa.TRDTIPOR = "D";
                danteCausa.TRDMORAA = datiDanteCausa.DataMorte.HasValue ? (short)datiDanteCausa.DataMorte.Value.Year : (short)0;
                danteCausa.TRDMORMM = datiDanteCausa.DataMorte.HasValue ? (short)datiDanteCausa.DataMorte.Value.Month : (short)0;
                danteCausa.TRDMORGG = datiDanteCausa.DataMorte.HasValue ? (short)datiDanteCausa.DataMorte.Value.Day : (short)0;
                danteCausa.TRDCERTI = datiDanteCausa.Certificato.HasValue ? datiDanteCausa.Certificato.Value : 0;
                danteCausa.TRDCARIC = Utility.StringToNullableShort(datiDanteCausa.Sede).HasValue ? Utility.StringToNullableShort(datiDanteCausa.Sede).Value : (short)0;
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    danteCausa.TRDCATEG = !string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) ? string.Format("{0}{1}", datiDanteCausa.SiglaCategoria.Trim().Substring(0, 1), datiDanteCausa.SiglaCategoria.Trim().Substring(2)) : string.Empty;
                else
                    danteCausa.TRDCATEG = !string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) ? datiDanteCausa.SiglaCategoria.Trim() : string.Empty;

                if (datiDanteCausa.CodiceEliminazione.HasValue)
                {
                    short codiceEliminazione = 0;
                    short.TryParse(lstDecCodiceEliminazione.Find(x => x.Id == datiDanteCausa.CodiceEliminazione.Value.ToString()).TraduzioneSuGP.Value.ToString(), out codiceEliminazione);
                    danteCausa.TRDCODEL = codiceEliminazione;
                }

                danteCausa.TRDCDEAA = datiDanteCausa.DecorrenzaEliminazione.HasValue ? (short)datiDanteCausa.DecorrenzaEliminazione.Value.Year : (short)0;
                danteCausa.TRDCDEMM = datiDanteCausa.DecorrenzaEliminazione.HasValue ? (short)datiDanteCausa.DecorrenzaEliminazione.Value.Month : (short)0;
                danteCausa.TRDCNNAA = datiDanteCausa.DecorrenzaEliminazioneContabile.HasValue ? (short)datiDanteCausa.DecorrenzaEliminazioneContabile.Value.Year : (short)0;
                danteCausa.TRDCNNMM = datiDanteCausa.DecorrenzaEliminazioneContabile.HasValue ? (short)datiDanteCausa.DecorrenzaEliminazioneContabile.Value.Month : (short)0;
                danteCausa.TRDCFSIT = "";
                GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out datiAnagraficiDanteCausa);
                if (datiAnagraficiDanteCausa != null)
                {
                    danteCausa.TRDAANAS = datiAnagraficiDanteCausa.DataNascita.HasValue ? (short)datiAnagraficiDanteCausa.DataNascita.Value.Year : (short)0;
                    danteCausa.TRDMMNAS = datiAnagraficiDanteCausa.DataNascita.HasValue ? (short)datiAnagraficiDanteCausa.DataNascita.Value.Month : (short)0;
                    danteCausa.TRDGGNAS = datiAnagraficiDanteCausa.DataNascita.HasValue ? (short)datiAnagraficiDanteCausa.DataNascita.Value.Day : (short)0;
                    danteCausa.TRDSESSO = datiAnagraficiDanteCausa.Sesso.HasValue ? datiAnagraficiDanteCausa.Sesso.Value.ToString() : "";
                    danteCausa.TRDCOACQ = !string.IsNullOrEmpty(datiAnagraficiDanteCausa.CognomeAcquisito) ?
                        datiAnagraficiDanteCausa.CognomeAcquisito.Length > 16 ? datiAnagraficiDanteCausa.CognomeAcquisito.Substring(0, 16) : datiAnagraficiDanteCausa.CognomeAcquisito : string.Empty;
                    danteCausa.TRDCONOM = GetCognomeNomeTagliato(datiAnagraficiDanteCausa.Cognome.Trim() + "/" + datiAnagraficiDanteCausa.Nome.Trim());
                    danteCausa.TRDCOFIS = datiAnagraficiDanteCausa.CodiceFiscale;
                    int codiceInpsComune = 0;
                    GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagraficiDanteCausa.CodiceComuneNascita, Utility.TipoAppartenenza.FS.ToString(), 0, false, out codiceInpsComune);
                    danteCausa.TRDCONAS = codiceInpsComune;

                    if (datiAnagraficiDanteCausa.ResidenzaEstero.HasValue && datiAnagraficiDanteCausa.ResidenzaEstero.Value)
                    {
                        danteCausa.TRDPRNAS = 96;
                    }
                    else
                    {
                        short codProvNascita = 0;
                        GetCodiceProvinciaNascita(datiAnagraficiDanteCausa.ProvinciaNascita, out codProvNascita);
                        danteCausa.TRDPRNAS = codProvNascita;
                    }

                    danteCausa.TRDDTMATR = datiAnagraficiDanteCausa.DataMatrimonio.HasValue ?
                        int.Parse(datiAnagraficiDanteCausa.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                        datiAnagraficiDanteCausa.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                        datiAnagraficiDanteCausa.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0')) : 0;
                }

                AreaCalcolo.AreaInputVariabile.ListaDanteCausa = new List<Data.CMSGTRA.DanteCausa> { danteCausa };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaDanteCausa[0].TRDTIPOR));
            }
        }


        public static void ValorizzaDelegatoTutore(GestioneAnagrafica.DatiAnagrafici datiAnagraficiDelegato, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTutore, ref Data.FSPL_FSRC AreaCalcolo)
        {
            Data.CMSGTRA.Deleghe_Tutele delegaTutela = null;

            if (datiAnagraficiDelegato != null)
            {
                delegaTutela = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Deleghe_Tutele();
                delegaTutela.TRKTIPOR = "K";
                delegaTutela.COGN_GP1DCOGNOME = datiAnagraficiDelegato.Cognome;
                delegaTutela.NOME_GP1DNOME = datiAnagraficiDelegato.Nome;
                delegaTutela.CODFIS_GP1AP26 = datiAnagraficiDelegato.CodiceFiscale;
                delegaTutela.DATANAS_GP1AP22SA = datiAnagraficiDelegato.DataNascita.HasValue ? datiAnagraficiDelegato.DataNascita.Value.Year.ToString().PadLeft(4, '0') : "9999";
                delegaTutela.DATANAS_GP1AP22M = datiAnagraficiDelegato.DataNascita.HasValue ? datiAnagraficiDelegato.DataNascita.Value.Month.ToString().PadLeft(2, '0') : "99";
                delegaTutela.DATANAS_GP1AP22G = datiAnagraficiDelegato.DataNascita.HasValue ? datiAnagraficiDelegato.DataNascita.Value.Day.ToString().PadLeft(2, '0') : "99";
                delegaTutela.COMUNAS_GP1AP24 = datiAnagraficiDelegato.ComuneNascita;
                delegaTutela.PROVNAS_GP1AP25 = datiAnagraficiDelegato.ProvinciaNascita;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagraficiDelegato.CodiceComuneNascita, Utility.TipoAppartenenza.FS.ToString(), 0, false, out codiceInpsComune);
                delegaTutela.COMCOD_GP1AP23 = codiceInpsComune;
                delegaTutela.SESSO_GP1AP27 = datiAnagraficiDelegato.Sesso.HasValue ? datiAnagraficiDelegato.Sesso.Value.ToString() : "";
                delegaTutela.COMRES_GP1DCOMUNE = datiAnagraficiDelegato.ComuneResidenza;
                delegaTutela.PROVRES_GP1DPROV = datiAnagraficiDelegato.ProvinciaResidenza;

                if (datiAnagraficiDelegato.Indirizzo.Trim().Length > 52)
                {
                    delegaTutela.INDI1_GP1DINDIRIZZ = datiAnagraficiDelegato.Indirizzo.Trim().Substring(0, 52);
                    if (datiAnagraficiDelegato.Indirizzo.Trim().Length > 104)
                    {
                        delegaTutela.INDI2_GP1DINDIRIZB = datiAnagraficiDelegato.Indirizzo.Trim().Substring(52, 52);
                        if (datiAnagraficiDelegato.Indirizzo.Trim().Length > 156)
                            delegaTutela.INDI3_GP1DINDIRIZC = datiAnagraficiDelegato.Indirizzo.Trim().Substring(104, 52);
                        else
                            delegaTutela.INDI3_GP1DINDIRIZC = datiAnagraficiDelegato.Indirizzo.Trim().Substring(104);
                    }
                    else
                        delegaTutela.INDI2_GP1DINDIRIZB = datiAnagraficiDelegato.Indirizzo.Trim().Substring(52);
                }
                else
                    delegaTutela.INDI1_GP1DINDIRIZZ = datiAnagraficiDelegato.Indirizzo.Trim();

                delegaTutela.NUM_GP1DCIVICO = datiAnagraficiDelegato.NCivico;
                delegaTutela.FRA_GP1DFRAZIONE = datiAnagraficiDelegato.FrazioneResidenza;
                delegaTutela.CAP_GP1DCAP = datiAnagraficiDelegato.CAP;
                delegaTutela.ARCA1_GP1AP28 = datiAnagraficiDelegato.Codice1Arca;
                int resInt = 0;
                int.TryParse(datiAnagraficiDelegato.Codice2Arca, out resInt);
                delegaTutela.ARCA2_GP1AP29 = resInt;

                if (datiAnagraficiDelegato.ResidenzaEstero.HasValue && datiAnagraficiDelegato.ResidenzaEstero.Value)
                    delegaTutela.EST_GP1DRESIDOM = "9";
                else if (datiAnagraficiDelegato.ResidenzaEstero.HasValue && !datiAnagraficiDelegato.ResidenzaEstero.Value)
                    delegaTutela.EST_GP1DRESIDOM = "1";

                delegaTutela.CODDEL_GP1AP01 = datiAnagraficiDelegato.CodiceDelegato.HasValue ? datiAnagraficiDelegato.CodiceDelegato.Value.ToString() : "";

            }

            if (datiAnagraficiTutore != null)
            {
                if (delegaTutela == null)
                {
                    delegaTutela = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Deleghe_Tutele();
                    delegaTutela.TRKTIPOR = "K";
                }
                delegaTutela.COGN_GP1TCOGNOME = datiAnagraficiTutore.Cognome;
                delegaTutela.NOME_GP1TNOME = datiAnagraficiTutore.Nome;
                delegaTutela.CODFIS_GP1AP66 = datiAnagraficiTutore.CodiceFiscale;
                delegaTutela.DATANAS_GP1AP62SA = datiAnagraficiTutore.DataNascita.HasValue ? datiAnagraficiTutore.DataNascita.Value.Year.ToString().PadLeft(4, '0') : "9999";
                delegaTutela.DATANAS_GP1AP62M = datiAnagraficiTutore.DataNascita.HasValue ? datiAnagraficiTutore.DataNascita.Value.Month.ToString().PadLeft(2, '0') : "99";
                delegaTutela.DATANAS_GP1AP62G = datiAnagraficiTutore.DataNascita.HasValue ? datiAnagraficiTutore.DataNascita.Value.Day.ToString().PadLeft(2, '0') : "99";
                delegaTutela.COMUNAS_GP1AP64 = datiAnagraficiTutore.ComuneNascita;
                delegaTutela.PROVNAS_GP1AP65 = datiAnagraficiTutore.ProvinciaNascita;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagraficiTutore.CodiceComuneNascita, Utility.TipoAppartenenza.FS.ToString(), 0, false, out codiceInpsComune);
                delegaTutela.COMCOD_GP1AP63 = codiceInpsComune;
                delegaTutela.SESSO_GP1AP67 = datiAnagraficiTutore.Sesso.HasValue ? datiAnagraficiTutore.Sesso.Value.ToString() : "";
                delegaTutela.COMRES_GP1TCOMUNE = datiAnagraficiTutore.ComuneResidenza;
                delegaTutela.PROVRES_GP1TPROV = datiAnagraficiTutore.ProvinciaResidenza;

                if (datiAnagraficiTutore.Indirizzo.Trim().Length > 52)
                {
                    delegaTutela.INDI1_GP1TINDIRIZZ = datiAnagraficiTutore.Indirizzo.Trim().Substring(0, 52);
                    if (datiAnagraficiTutore.Indirizzo.Trim().Length > 104)
                    {
                        delegaTutela.INDI2_GP1TINDIRIZB = datiAnagraficiTutore.Indirizzo.Trim().Substring(52, 52);
                        if (datiAnagraficiTutore.Indirizzo.Trim().Length > 156)
                            delegaTutela.INDI3_GP1TINDIRIZC = datiAnagraficiTutore.Indirizzo.Trim().Substring(104, 52);
                        else
                            delegaTutela.INDI3_GP1TINDIRIZC = datiAnagraficiTutore.Indirizzo.Trim().Substring(104);
                    }
                    else
                        delegaTutela.INDI2_GP1TINDIRIZB = datiAnagraficiTutore.Indirizzo.Trim().Substring(52);
                }
                else
                    delegaTutela.INDI1_GP1TINDIRIZZ = datiAnagraficiTutore.Indirizzo.Trim();

                delegaTutela.NUM_GP1TCIVICO = datiAnagraficiTutore.NCivico;
                delegaTutela.FRA_GP1TFRAZIONE = datiAnagraficiTutore.FrazioneResidenza;
                delegaTutela.CAP_GP1TCAP = datiAnagraficiTutore.CAP;
                delegaTutela.ARCA1_GP1AP68 = datiAnagraficiTutore.Codice1Arca;
                int resInt = 0;
                int.TryParse(datiAnagraficiTutore.Codice2Arca, out resInt);
                delegaTutela.ARCA2_GP1AP69 = resInt;

                if (datiAnagraficiTutore.ResidenzaEstero.HasValue && datiAnagraficiTutore.ResidenzaEstero.Value)
                    delegaTutela.EST_GP1TRESIDOM = "9";
                else if (datiAnagraficiTutore.ResidenzaEstero.HasValue && !datiAnagraficiTutore.ResidenzaEstero.Value)
                    delegaTutela.EST_GP1TRESIDOM = "1";

                delegaTutela.CODDEL_GP1AP61 = datiAnagraficiTutore.CodiceTutore.HasValue ? datiAnagraficiTutore.CodiceTutore.Value.ToString() : "";

                delegaTutela.DATACES_GP1AP70A = datiAnagraficiTutore.CessValAmmSost.HasValue ? datiAnagraficiTutore.CessValAmmSost.Value.Year : 0;
                delegaTutela.DATACES_GP1AP70M = datiAnagraficiTutore.CessValAmmSost.HasValue ? datiAnagraficiTutore.CessValAmmSost.Value.Month : 0;

            }

            if (delegaTutela != null && ConfigurationManager.AppSettings["AbilitaRecordK"] != null && ConfigurationManager.AppSettings["AbilitaRecordK"] == "SI")
            {
                AreaCalcolo.AreaInputVariabile.ListaDelegheTutele = new List<Data.CMSGTRA.Deleghe_Tutele> { delegaTutela };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaDelegheTutele[0].TRKTIPOR));
            }
        }

        public static void ValorizzaSupplementi(GestionePensione.DatiPensione datiPensione, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            GestioneSupplementi.GetSupplementiByIdPensione(datiPensione.Id, out listaSupplementi);
            if (listaSupplementi != null && listaSupplementi.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaSupplementi = new List<Data.CMSGTRA.Supplementi>();
                Data.CMSGTRA.Supplementi supplementi = new Data.CMSGTRA.Supplementi();
                supplementi.TRETIPOR = "E";
                supplementi.LISTTRE_SUP14 = new List<Data.CMSGTRA.Supplementi.TRE_SUP14>();

                foreach (Liquidazione.BLCommon.Entity.DatiSupplementi supp in listaSupplementi)
                {
                    Data.CMSGTRA.Supplementi.TRE_SUP14 suppl = new Data.CMSGTRA.Supplementi.TRE_SUP14();
                    suppl.TREDECAA = supp.DecorrenzaSupplemento.HasValue ? (short)supp.DecorrenzaSupplemento.Value.Year : (short)0;
                    suppl.TREDECMM = supp.DecorrenzaSupplemento.HasValue ? (short)supp.DecorrenzaSupplemento.Value.Month : (short)0;
                    suppl.TREDPC01 = 0;
                    suppl.TREESC01 = 0;
                    suppl.TRES7201 = 0;

                    suppl.TREFLG01 = supp.QuotaSupplemento.HasValue ? supp.QuotaSupplemento.Value.ToString() : "";
                    suppl.TRENAT01 = supp.TipoSupplemento.HasValue ? supp.TipoSupplemento.Value.ToString() : "";

                    if (suppl.TREFLG01 == "B" && suppl.TRENAT01 == "R")
                        suppl.TRENAT01 = "S";
                    
                    if (supp.TipoSupplemento.HasValue && (supp.TipoSupplemento.Value == 'C' || supp.TipoSupplemento.Value == 'D'))
                        suppl.TRERMS01 = supp.MontanteSupplemento.HasValue ? supp.MontanteSupplemento.Value : 0M;
                    else if (supp.TipoSupplemento.HasValue && supp.TipoSupplemento.Value == 'R')
                        suppl.TRERMS01 = supp.RMSSupplemento.HasValue ? Math.Round(supp.RMSSupplemento.Value, 2) : 0M;

                    suppl.TRETIP01 = supp.CodGestioneSupplemento;
                    suppl.TRETOT01 = supp.NSettimaneSupplemento.HasValue ? supp.NSettimaneSupplemento.Value : 0;
                    supplementi.LISTTRE_SUP14.Add(suppl);
                }
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datiPensione) && tipoFondo == Utility.TipoFondo.ET)
                {
                    supplementi.LISTTRE_SUP14 = supplementi.LISTTRE_SUP14.OrderBy(s => s.TREFLG01 != "A" && s.TRENAT01 != "R" && s.TRETIP01 != "1").ToList();
                }
                AreaCalcolo.AreaInputVariabile.ListaSupplementi.Add(supplementi);
               
                AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaSupplementi[0].TRETIPOR));
            }
        }

        public static void ValorizzaTrattamentiFamiglia(List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaStatiCivili != null && listaStatiCivili.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaTrattamentiFamiglia = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia>();
                INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia trattamentiFamiglia = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia();
                trattamentiFamiglia.LISTTRFELENU = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia.TRFELENU>();
                foreach (GestioneAnagrafica.DatiStatoCivile stCiv in listaStatiCivili)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia.TRFELENU statoCivile = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattamentiFamiglia.TRFELENU();
                    short codiceStatoCivile = 0;
                    short.TryParse(stCiv.Codice.ToString(), out codiceStatoCivile);
                    statoCivile.TRFSTA01 = stCiv.Codice.ToString();
                    statoCivile.TRFDECAA = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Year : (short)0;
                    statoCivile.TRFDECMM = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Month : (short)0;
                    trattamentiFamiglia.LISTTRFELENU.Add(statoCivile);
                }
                trattamentiFamiglia.TRFTIPOR = "F";
                AreaCalcolo.AreaInputVariabile.ListaTrattamentiFamiglia.Add(trattamentiFamiglia);
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaTrattamentiFamiglia[0].TRFTIPOR));
            }
        }

        public static void ValorizzaMinimo_PensInv(long idPensione, out Entity.DatiBititolaritaInail datiBititolaritaInail, ref Data.FSPL_FSRC AreaCalcolo)
        {
            datiBititolaritaInail = null;

            #region datiBititolaritaInail

            GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
            GestionePensioneInailInabilita.GetInabilitaByIdPensione(idPensione, out datiInabilita);

            List<GestionePensioneInailInabilita.DatiPensioniINAIL> LdatiInabilita = null;
            GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(idPensione, out LdatiInabilita);

            if (datiInabilita != null || (LdatiInabilita != null && LdatiInabilita.Count > 0))
            {
                datiBititolaritaInail = new Entity.DatiBititolaritaInail();

                if (LdatiInabilita != null && LdatiInabilita.Count > 0)
                {
                    datiBititolaritaInail.LpensioniInail = new List<Entity.DatiBititolaritaInail.PensioniInail>();
                    foreach (GestionePensioneInailInabilita.DatiPensioniINAIL pi in LdatiInabilita)
                    {
                        Entity.DatiBititolaritaInail.PensioniInail pensioniInail = new Entity.DatiBititolaritaInail.PensioniInail();
                        Utility.ValorizzaOggetti(pi, pensioniInail);
                        datiBititolaritaInail.LpensioniInail.Add(pensioniInail);
                    }
                }
                if (datiInabilita != null)
                    Utility.ValorizzaOggetti(datiInabilita, datiBititolaritaInail);
            }

            #endregion datiBititolaritaInail

            if (datiInabilita != null)
            {
                Data.CMSGTRA.Minimo_PensInv minimo_PensInv = new Data.CMSGTRA.Minimo_PensInv();

                minimo_PensInv.TRGTIPOR = "G";
                //TODO da rivedere
                minimo_PensInv.TRGCEAA = datiInabilita.CessazioneDirittoIntegrazioneMinimo.HasValue ? (short)datiInabilita.CessazioneDirittoIntegrazioneMinimo.Value.Year : (short)0;
                minimo_PensInv.TRGCEMM = datiInabilita.CessazioneDirittoIntegrazioneMinimo.HasValue ? (short)datiInabilita.CessazioneDirittoIntegrazioneMinimo.Value.Month : (short)0;
                minimo_PensInv.TRGDIAA = datiInabilita.DecorrenzaDirittoIntegrazioneMinimo.HasValue ? (short)datiInabilita.DecorrenzaDirittoIntegrazioneMinimo.Value.Year : (short)0;
                minimo_PensInv.TRGDIMM = datiInabilita.DecorrenzaDirittoIntegrazioneMinimo.HasValue ? (short)datiInabilita.DecorrenzaDirittoIntegrazioneMinimo.Value.Month : (short)0;

                AreaCalcolo.AreaInputVariabile.ListaMinimo_PensInv = new List<Data.CMSGTRA.Minimo_PensInv> { minimo_PensInv };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaMinimo_PensInv[0].TRGTIPOR));
            }
        }

        public static void ValorizzaResidenza(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Utility.TipoFondo? tipoFondo,
             GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<string> listaBeneficiAmmessi = new List<string> { "01", "12", "13", "14", "15", "18", "19", "24" };
            Data.CMSGTRA.Residenza residenza = null;

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);

            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneri);

            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaBeneficiParticolari = null;
            GestioneBeneficiParticolari.GetBeneficiParticolariByIdPensione(datiPensione.Id, datiPensione, out listaBeneficiParticolari);

            if ((listaResidenzeEstere != null && listaResidenzeEstere.Count > 0) || (listaDatiOneri != null && listaDatiOneri.Count > 0) ||
                (listaBeneficiParticolari != null && listaBeneficiParticolari.Count > 0) || (datiMaggiorazioniBenefici != null && listaBeneficiAmmessi.Contains(datiMaggiorazioniBenefici.TipoSettimaneBeneficio)) ||
                ((tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.TT) &&
                Utility.IsDomandaAssegnoInvaliditaOrdinario(datiPensione) && datiIstruttoria != null && datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue))
            {
                residenza = new Data.CMSGTRA.Residenza();
                residenza.TRHTIPOR = "H";
            }

            if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                residenza.LISTTRHELERD = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHELERD>();
                foreach (GestioneAnagrafica.DatiResidenzaEstero res in listaResidenzeEstere)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHELERD residenzaEstero = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHELERD();
                    residenzaEstero.TRHAAR01 = res.Decorrenza.HasValue ? (short)res.Decorrenza.Value.Year : (short)0;
                    residenzaEstero.TRHMMR01 = res.Decorrenza.HasValue ? (short)res.Decorrenza.Value.Month : (short)0;
                    if (res.CodCatastaleStatoEE == "Z000")
                        residenzaEstero.TRHSTA01 = "ITA";
                    else
                    {
                        GestioneDecodifica.StatoEstero statoEstero = null;
                        GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(res.CodCatastaleStatoEE, out statoEstero);
                        if (statoEstero != null)
                            residenzaEstero.TRHSTA01 = statoEstero.Sigla;
                    }
                    residenza.LISTTRHELERD.Add(residenzaEstero);
                }
            }
            if (listaDatiOneri != null && listaDatiOneri.Count > 0)
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                residenza.LISTTRHONERE = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE>();
                foreach (GestioneOneri.DatiOneri on in listaDatiOneri)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE onere = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE();
                    onere.TRH_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ?
                        (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0'))
                        : string.Empty;

                    if (datiPensione.DecorrenzaOriginaria.HasValue && on.Scadenza.HasValue &&
                        Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, on.Scadenza.Value))
                        onere.TRH_SCADONERE = datiPensione.DecorrenzaOriginaria.HasValue ?
                        (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0'))
                        : string.Empty;
                    else
                        onere.TRH_SCADONERE = on.Scadenza.HasValue ?
                            (on.Scadenza.Value.Year.ToString().PadLeft(4, '0') + on.Scadenza.Value.Month.ToString().PadLeft(2, '0') + on.Scadenza.Value.Day.ToString().PadLeft(2, '0'))
                            : string.Empty;
                    List<GestioneDecodifica.GruppoOneri> listaGruppoOneri = null;
                    GestioneDecodifica.GetGruppoOneri(out listaGruppoOneri);
                    if (listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                    {
                        GestioneDecodifica.GruppoOneri gruppoOneri = listaGruppoOneri.Find(x => x.Id == (on.IdCodeGruppo.HasValue ? on.IdCodeGruppo.Value : 0));
                        if (gruppoOneri != null)
                        {
                            onere.TRH_CODGRUP = gruppoOneri.Code;
                        }
                    }
                    List<GestioneDecodifica.SottoGruppoOneri> listaSottoGruppoOneri = null;
                    GestioneDecodifica.GetSottoGruppoOneri(out listaSottoGruppoOneri);
                    if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                    {
                        GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = listaSottoGruppoOneri.Find(x => x.Id == (on.IdCodeSottoGruppo.HasValue ? on.IdCodeSottoGruppo.Value : 0));
                        if (sottoGruppoOneri != null)
                        {
                            onere.TRH_CODSGRUP = sottoGruppoOneri.Code;
                        }
                    }

                    onere.TRH_ANZCON = on.Settimane.GetValueOrDefault();
                    onere.TRH_ONERE = on.Onere.GetValueOrDefault();

                    if (!((tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.TT) && Utility.IsDomandaAssegnoInvaliditaOrdinario(datiPensione)) &&
                        (onere.TRH_CODGRUP == "5000" || onere.TRH_CODGRUP == "5300" || onere.TRH_CODGRUP == "5800" || onere.TRH_CODGRUP == "6000" || onere.TRH_CODGRUP == "6100"))
                        residenza.TRH_CESINCUM = on.ScadenzaBeneficio.HasValue ? (on.ScadenzaBeneficio.Value.Year * 100) + on.ScadenzaBeneficio.Value.Month : 0;

                    residenza.LISTTRHONERE.Add(onere);
                }
            }

            if (listaBeneficiParticolari != null && listaBeneficiParticolari.Count > 0)
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                if (residenza.LISTTRHONERE == null)
                    residenza.LISTTRHONERE = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE>();

                for (int i = 0; i < listaBeneficiParticolari.Count; i++)
                {
                    if (residenza.LISTTRHONERE.Count > i)
                    {
                        residenza.LISTTRHONERE[i].TRH_CODBENEF = listaBeneficiParticolari[i].CodiceBenefici;
                        residenza.LISTTRHONERE[i].TRH_ANZBENEF = listaBeneficiParticolari[i].Settimane.GetValueOrDefault();
                    }
                    else
                    {
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE onere = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE();
                        onere.TRH_CODBENEF = listaBeneficiParticolari[i].CodiceBenefici;
                        onere.TRH_ANZBENEF = listaBeneficiParticolari[i].Settimane.GetValueOrDefault();
                        residenza.LISTTRHONERE.Add(onere);
                    }
                }
            }

            if (datiMaggiorazioniBenefici != null && listaBeneficiAmmessi.Contains(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                if (residenza.LISTTRHONERE == null)
                    residenza.LISTTRHONERE = new List<Data.CMSGTRA.Residenza.TRHONERE>();

                if (residenza.LISTTRHONERE.Count > 0)
                {
                    residenza.LISTTRHONERE.First().TRH_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                    residenza.THR_SET_NONVE = datiMaggiorazioniBenefici.NSettimaneBeneficio.HasValue ? (short)datiMaggiorazioniBenefici.NSettimaneBeneficio.Value : 0;
                    residenza.THR_SET_NONVE_P95 = datiMaggiorazioniBenefici.SettAnzContribPost311295.HasValue ? datiMaggiorazioniBenefici.SettAnzContribPost311295.Value : 0;
                    residenza.TRH_NUM_FIGLI = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;
                }
                else
                {
                    Data.CMSGTRA.Residenza.TRHONERE onere = new Data.CMSGTRA.Residenza.TRHONERE();
                    onere.TRH_CODBENEF = datiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                    residenza.LISTTRHONERE.Add(onere);
                    residenza.THR_SET_NONVE = datiMaggiorazioniBenefici.NSettimaneBeneficio.HasValue ? (short)datiMaggiorazioniBenefici.NSettimaneBeneficio.Value : 0;
                    residenza.THR_SET_NONVE_P95 = datiMaggiorazioniBenefici.SettAnzContribPost311295.HasValue ? datiMaggiorazioniBenefici.SettAnzContribPost311295.Value : 0;
                    residenza.TRH_NUM_FIGLI = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;
                }
            }

            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione))
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                residenza.TRH_NUM_FIGLI = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;
            }

            if ((tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.TT) &&
                Utility.IsDomandaAssegnoInvaliditaOrdinario(datiPensione) && datiIstruttoria != null && datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
            {
                if (residenza == null)
                    residenza = new Data.CMSGTRA.Residenza();

                residenza.TRH_CESINCUM = int.Parse(datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Year.ToString().PadLeft(4, '0') +
                                         datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Month.ToString().PadLeft(2, '0') : "0");
            }

            //ENG - memo 28_2024
            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                GestioneFondo.DatiFondo datiFondo = null;
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);

                if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") ||
                    (datiPensione.IdTipoPLPerRIC.HasValue && datiPensione.IdTipoPLPerRIC == 7 && datiFondo != null && datiFondo.CodiceSpecifico != null &&
                    ((tipoFondo == Utility.TipoFondo.PT && datiFondo.CodiceSpecifico == 41) ||
                            (tipoFondo == Utility.TipoFondo.FS && datiFondo.CodiceSpecifico == 47) ||
                            (tipoFondo == Utility.TipoFondo.TT && datiFondo.CodiceSpecifico == 14) ||
                            (tipoFondo == Utility.TipoFondo.ET && datiFondo.CodiceSpecifico == 22) ||
                            (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (datiFondo.CodiceSpecifico == 181 || datiFondo.CodiceSpecifico == 182))))) &&
                    datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo == 19)
                {
                    if (datiPensione.DecorrenzaOriginaria.HasValue &&
                        Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 1, 1)))
                    {
                        if (residenza == null)
                            residenza = new Data.CMSGTRA.Residenza();

                        residenza.TRHTIPOR = "H";
                        residenza.TRH_NUM_FIGLI = datiPensione.NumeroFigli.HasValue ? datiPensione.NumeroFigli.Value.ToString() : string.Empty;
                        if (datiIstruttoria != null && datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
                        {
                            residenza.TRH_CESINCUM = int.Parse(datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Year.ToString().PadLeft(4, '0') +
                                                     datiIstruttoria.ScadenzaRevisioneSanitaria.Value.Month.ToString().PadLeft(2, '0') : "0");
                        }

                        //ENG - Figli senza benefici
                        if (datiPensione.NumeroFigli.HasValue && datiPensione.NumeroFigli.Value > 0)
                        {
                            if (datiMaggiorazioniBenefici == null || String.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                            {
                                if (residenza.LISTTRHONERE == null)
                                    residenza.LISTTRHONERE = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE>();

                                if (residenza.LISTTRHONERE.Count == 0)
                                    residenza.LISTTRHONERE.Add(new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE());

                                foreach (INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE temp in residenza.LISTTRHONERE)
                                {
                                    temp.TRH_DECONERE = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                                                                                      datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') +
                                                                                                      datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0')) : string.Empty;
                                }
                            }
                        }
                    }
                }
            }

            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
            {
                if (!string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta))
                {
                    if (residenza == null)
                        residenza = new Data.CMSGTRA.Residenza();

                    if (residenza.LISTTRHONERE == null)
                        residenza.LISTTRHONERE = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Residenza.TRHONERE>();

                    if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true))
                    {
                        if (datiPensione.CodiceTipoRichiesta == "KW" || datiPensione.CodiceTipoRichiesta == "KX")
                            residenza.LISTTRHONERE.First().TRH_CODBENEF = "20";
                    }
                    else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true))
                    {
                        if (datiPensione.CodiceTipoRichiesta == "KY" || datiPensione.CodiceTipoRichiesta == "KZ")
                            residenza.LISTTRHONERE.First().TRH_CODBENEF = "21";
                    }
                    else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                    {
                        if (datiPensione.CodiceTipoRichiesta == "KU" || datiPensione.CodiceTipoRichiesta == "KV")
                            residenza.LISTTRHONERE.First().TRH_CODBENEF = "22";
                    }
                }
            }

            if (residenza != null)
            {
                AreaCalcolo.AreaInputVariabile.ListaResidenza = new List<Data.CMSGTRA.Residenza> { residenza };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaResidenza[0].TRHTIPOR));
            }
        }

        public static void ValorizzaMaggiorazioneLegge(GestionePensione.DatiPensione datiPensione, out GestioneDL407.DatiDL407 datiDL407, ref Data.FSPL_FSRC AreaCalcolo)
        {
            GestioneDL407.GetDL407ByIdPensione(datiPensione.Id, out datiDL407);

            if (datiDL407 != null && !datiDL407.IsDL407NullForAnteArm())
            {
                AreaCalcolo.AreaInputVariabile.ListaMaggiorazioneLegge = new List<Data.CMSGTRA.MaggiorazioneLegge>();
                Data.CMSGTRA.MaggiorazioneLegge maggiorazioneLegge = new Data.CMSGTRA.MaggiorazioneLegge();

                maggiorazioneLegge.TRITIPOR = "I";
                maggiorazioneLegge.TRIRA336 = datiDL407.RetribPensSL336QuotaA.HasValue ? datiDL407.RetribPensSL336QuotaA.Value : 0M;
                maggiorazioneLegge.TRIRB336 = datiDL407.RetribPensSL336QuotaB.HasValue ? datiDL407.RetribPensSL336QuotaB.Value : 0M;
                maggiorazioneLegge.TRIRETQA = datiDL407.RetribPensQuotaA.HasValue ? datiDL407.RetribPensQuotaA.Value : 0M;
                maggiorazioneLegge.TRIRETQB = datiDL407.RetribPensQuotaB.HasValue ? datiDL407.RetribPensQuotaB.Value : 0M;
                maggiorazioneLegge.TRISEUTA = datiDL407.ServizioUtileAAQuotaA.HasValue ? datiDL407.ServizioUtileAAQuotaA.Value : (short)0;
                maggiorazioneLegge.TRISEUTB = datiDL407.ServizioUtileAAQuotaB.HasValue ? datiDL407.ServizioUtileAAQuotaB.Value : (short)0;
                maggiorazioneLegge.TRISEUTC = datiDL407.ServizioUtileAAQuotaC.HasValue ? datiDL407.ServizioUtileAAQuotaC.Value : (short)0;
                maggiorazioneLegge.TRIUTIAA = datiDL407.ServizioUtileAAQuotaA.HasValue ? datiDL407.ServizioUtileAAQuotaA.Value : (short)0;
                maggiorazioneLegge.TRIUTIAB = datiDL407.ServizioUtileAAQuotaB.HasValue ? datiDL407.ServizioUtileAAQuotaB.Value : (short)0;

                AreaCalcolo.AreaInputVariabile.ListaMaggiorazioneLegge.Add(maggiorazioneLegge);
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaMaggiorazioneLegge[0].TRITIPOR));
            }
        }

        public static void ValorizzaRenditaINAIL(Entity.DatiBititolaritaInail datiBititolaritaInail, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (datiBititolaritaInail != null && ((datiBititolaritaInail.LpensioniInail != null && datiBititolaritaInail.LpensioniInail.Count > 0) || datiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue ||
                datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.HasValue))
            {
                INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.RenditaINAIL renditaINAIL = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.RenditaINAIL();

                renditaINAIL.TRLTIPOR = "L";

                if (datiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue)
                    renditaINAIL.TRLDIRIT = datiBititolaritaInail.DirittoAssegnoAccompagnamento.Value ? "1" : "0";
                if (datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.HasValue)
                    renditaINAIL.TRLDECAC = int.Parse(datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.Value.Year.ToString() + datiBititolaritaInail.DecorrenzaAssegnoAccompangamento.Value.Month.ToString("00"));

                if (datiBititolaritaInail.LpensioniInail != null && datiBititolaritaInail.LpensioniInail.Count > 0)
                {
                    renditaINAIL.LISTTRGELERD = new List<Data.CMSGTRA.RenditaINAIL.TRGELERD>();
                    foreach (Entity.DatiBititolaritaInail.PensioniInail pensInail in datiBititolaritaInail.LpensioniInail)
                    {
                        Data.CMSGTRA.RenditaINAIL.TRGELERD pI = new Data.CMSGTRA.RenditaINAIL.TRGELERD();
                        pI.TRLDEC01 = pensInail.DecorrenzaRenditaInail.HasValue ? int.Parse((pensInail.DecorrenzaRenditaInail.Value.Year.ToString().PadLeft(4, '0') +
                            pensInail.DecorrenzaRenditaInail.Value.Month.ToString().PadLeft(2, '0'))) : 0;
                        pI.TRLIMP01 = pensInail.ImportoMensileInail.HasValue ? pensInail.ImportoMensileInail.Value : 0M;
                        pI.TRLEVE01 = pensInail.Evento.HasValue ? pensInail.Evento.Value ? "1" : "0" : "";
                        renditaINAIL.LISTTRGELERD.Add(pI);
                    }
                }

                AreaCalcolo.AreaInputVariabile.ListaRenditaINAIL = new List<Data.CMSGTRA.RenditaINAIL> { renditaINAIL };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaRenditaINAIL[0].TRLTIPOR));
            }
        }

        public static void ValorizzaTrattenuteLavAutonomi(ref Data.FSPL_FSRC AreaCalcolo)
        {
            //INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattenuteLavAutonomi trattenuteLavAutonomi = null;

            ////TODO recupero dati DB
            //object datiTrattenuteLavAutonomi = null;
            //if (datiTrattenuteLavAutonomi != null)
            //{
            //    AreaCalcolo.AreaInputVariabile.ListaTrattenuteLavAutonomi = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.TrattenuteLavAutonomi>();
            //    //TODO assegnazione dati DB alla transazione

            //    AreaCalcolo.AreaInputVariabile.ListaTrattenuteLavAutonomi.Add(trattenuteLavAutonomi);
            //    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaTrattenuteLavAutonomi[0].TRMTIPOR));
            //}
        }

        public static void ValorizzaAgoTeorico(ref Data.FSPL_FSRC AreaCalcolo)
        {
            //INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.AgoTeorico agoTeorico = null;

            ////TODO recupero dati DB
            //object datiAgoTeorico = null;
            //if (datiAgoTeorico != null)
            //{
            //    AreaCalcolo.AreaInputVariabile.ListaAgoTeorico = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.AgoTeorico>();
            //    //TODO assegnazione dati DB alla transazione

            //    AreaCalcolo.AreaInputVariabile.ListaAgoTeorico.Add(agoTeorico);
            //    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoTeorico[0].TRNTIPOR));
            //}
        }

        public static void ValorizzaMaggiorazioneSociale(Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (datiMaggiorazioniBenefici != null)
            {
                Data.CMSGTRA.MaggiorazioneSociale maggiorazioneSociale = new Data.CMSGTRA.MaggiorazioneSociale();
                maggiorazioneSociale.TRPTIPOR = "P";

                maggiorazioneSociale.TRPDCMAG = datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue ?
                    int.Parse(datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Year.ToString().PadLeft(4, '0') +
                    datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Month.ToString().PadLeft(2, '0') + "01") : 0;
                maggiorazioneSociale.TRPDATDO = datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue ?
                   int.Parse(datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Year.ToString().PadLeft(4, '0') +
                       datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Month.ToString().PadLeft(2, '0') + "01") : 0;

                AreaCalcolo.AreaInputVariabile.ListaMaggiorazioneSociale = new List<Data.CMSGTRA.MaggiorazioneSociale> { maggiorazioneSociale };
                AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaMaggiorazioneSociale[0].TRPTIPOR));
            }
        }

        public static void ValorizzaRecordR(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, ref Data.FSPL_FSRC AreaCalcolo)
        {
            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Redditi redditi = null;

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            GestioneControlliDinamici.ControlloDinamico ctrl06_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo06_2024", out ctrl06_2024);

            if (datiPensione != null)
            {
                AreaCalcolo.AreaInputVariabile.ListaRedditi = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Redditi>();
                redditi = new Data.CMSGTRA.Redditi();

                redditi.TRRTIPOR = "R";
                int res = 0;
                int.TryParse(datiPensione.Gruppo, out res);
                redditi.TRR_GRUPPO = res;
                int.TryParse(datiPensione.Prodotto, out res);
                redditi.TRR_PROD = res;
                int.TryParse(datiPensione.Tipo, out res);
                redditi.TRR_TIPO = res;
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    redditi.TRR_TIPO_DOM = Utility.GetFiltroByCodTipoRichiesta(datiPensione.CodiceTipoRichiesta);
                    if (datiPensione.DataCompletezza.HasValue)
                        redditi.TRR_DATA_COMP = int.Parse(datiPensione.DataCompletezza.Value.Year.ToString().PadLeft(4, '0') +
                                datiPensione.DataCompletezza.Value.Month.ToString().PadLeft(2, '0') +
                                datiPensione.DataCompletezza.Value.Day.ToString().PadLeft(2, '0'));

                    if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                        redditi.R_Note_TE08 = datiPensione.IdNota != null ? datiPensione.IdNota.Value : 0;
                }

                if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                    Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true))
                    redditi.TRR_TIPO_DOM = Utility.GetFiltroByCodTipoRichiesta(datiPensione.CodiceTipoRichiesta);

                //ENG - Nuovi campi FLAGSENT_R e CITTA_R
                if (datiPensione.GP1AV91A.HasValue)
                {
                    redditi.FLAGSENT_R = datiPensione.GP1AV91A.Value;
                }

                if (datiAnagraficiTitolare != null && !String.IsNullOrEmpty(datiAnagraficiTitolare.Cittadinanza))
                {
                    //ENG - Memo 48_2023
                    if (Utility.IsTitolareResidente_Cittadino_Bulgaria(datiPensione, datiAnagraficiTitolare))
                        redditi.CITTA_R = "BG";
                    else
                    {
                        List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                        GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);

                        if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                        {
                            GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.CodCatastale == datiAnagraficiTitolare.Cittadinanza);
                            if (statoEstero != null)
                                redditi.CITTA_R = !string.IsNullOrEmpty(statoEstero.Sigla) ? statoEstero.Sigla.Trim() == "ITA" ? "I" : statoEstero.Sigla.Trim() : string.Empty;
                        }
                    }

                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (Utility.IsDomandaPL(datiPensione) || Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)))
                    {
                        redditi.GP3CB02_R = datiAnagraficiTitolare.Cognome;
                        redditi.GP3CB03_R = datiAnagraficiTitolare.Nome;
                    }

                }

                //ENG - Memo 28_2024 0001-0001-0017 con decorrenza > 01.01.2024 e tipo di calcolo "contributivo" GP1TPCLC_R con secondo byte uguale a 1
                if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (!String.IsNullOrEmpty(datiPensione.Caratterizzazione))
                    {
                        redditi.GP1TPCLC_R = datiPensione.Caratterizzazione;
                    }
                    else if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017") &&
                        (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.IsDomandaTipoContributivoCumulo(datiPensione, null, null)) && datiPensione.DecorrenzaOriginaria.HasValue &&
                        Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01)))
                    {
                        redditi.GP1TPCLC_R = " 1      ";
                    }
                }

                //ENG - Memo 06_2024
                if (ctrl06_2024 != null && !String.IsNullOrEmpty(ctrl06_2024.ValoreControllo) && ctrl06_2024.ValoreControllo.ToUpperInvariant() == "SI")
                {
                    if (datiPensione.CodProPE.HasValue && datiPensione.CodProPE == 8)
                    {
                        if (!String.IsNullOrEmpty(redditi.GP1TPCLC_R))
                        {
                            redditi.GP1TPCLC_R = "1" + redditi.GP1TPCLC_R.Substring(1);
                        }
                        else
                        {
                            redditi.GP1TPCLC_R = "1       ";
                        }
                    }
                }
                redditi.GP1AV91B_R = datiPensione.GP1AV91B;
                //sovrascrivo valore per GDP RIC REV SIN che hanno il 3
                if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.GP1AV91B == "3")
                {
                    redditi.GP1AV91B_R = "1";
                }
                AreaCalcolo.AreaInputVariabile.ListaRedditi.Add(redditi);
                AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaRedditi[0].TRRTIPOR));
            }
        }

        public static void ValorizzaDatiNonCalcolo(GestionePensione.DatiPensione datiPensione, Dictionary<string, char> componentiFamiliari, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> listaRecordDatiNoCalcolo = null;
            GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out listaRecordDatiNoCalcolo);

            List<GestioneComponenteFamiliare.ComponenteFamiliare> listaComponentiFamiliari = null;
            GestioneComponenteFamiliare.GetComponenteFamiliareByIdPensione(datiPensione.Id, out listaComponentiFamiliari);

            if (listaRecordDatiNoCalcolo != null && listaRecordDatiNoCalcolo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaDatiNonCalcolo = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DatiNonCalcolo>();

                foreach (GestioneDatiNoCalcolo.RecordDatiNoCalcolo recordDatiNoCalcolo in listaRecordDatiNoCalcolo)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.DatiNonCalcolo datiNonCalcolo = new Data.CMSGTRA.DatiNonCalcolo();

                    List<GestioneComponenteFamiliare.ComponenteFamiliare> appListaComponentiFamiliari = listaComponentiFamiliari != null ? listaComponentiFamiliari.FindAll(x => x.IdRecordDatiNoCalcolo == recordDatiNoCalcolo.Id) : new List<GestioneComponenteFamiliare.ComponenteFamiliare>();

                    datiNonCalcolo.TRWTPREC = "W";
                    datiNonCalcolo.TRWFONDO = datiPensione.SiglaCategoria.Substring(0, 3);
                    datiNonCalcolo.TRWDECAA = !string.IsNullOrEmpty(recordDatiNoCalcolo.Decorrenza) ? short.Parse(recordDatiNoCalcolo.Decorrenza.Substring(6, 4)) : (short)0;
                    datiNonCalcolo.TRWDECMM = !string.IsNullOrEmpty(recordDatiNoCalcolo.Decorrenza) ? short.Parse(recordDatiNoCalcolo.Decorrenza.Substring(3, 2)) : (short)0;
                    datiNonCalcolo.TRWDECGG = !string.IsNullOrEmpty(recordDatiNoCalcolo.Decorrenza) ? short.Parse(recordDatiNoCalcolo.Decorrenza.Substring(0, 2)) : (short)0;
                    datiNonCalcolo.TRWCOL03 = recordDatiNoCalcolo.AdeguataAgo.HasValue ? recordDatiNoCalcolo.AdeguataAgo.Value : 0M;
                    datiNonCalcolo.TRWCOL04 = recordDatiNoCalcolo.AdeguataFondo.HasValue ? recordDatiNoCalcolo.AdeguataFondo.Value : 0M;
                    datiNonCalcolo.TRWCOL05 = recordDatiNoCalcolo.EccedenzaAgo.HasValue ? recordDatiNoCalcolo.EccedenzaAgo.Value : 0M;
                    datiNonCalcolo.TRWCOL06 = recordDatiNoCalcolo.QuotaAgoEsclusiva.HasValue ? recordDatiNoCalcolo.QuotaAgoEsclusiva.Value : 0M;
                    datiNonCalcolo.TRWCOL07 = recordDatiNoCalcolo.FacArt14.HasValue ? recordDatiNoCalcolo.FacArt14.Value : 0M;
                    datiNonCalcolo.TRWCOL08 = recordDatiNoCalcolo.IndIntSpeciale.HasValue ? recordDatiNoCalcolo.IndIntSpeciale.Value : 0M;
                    datiNonCalcolo.TRWCOL09 = recordDatiNoCalcolo.AssegniFamiliari.HasValue ? recordDatiNoCalcolo.AssegniFamiliari.Value : 0M;
                    datiNonCalcolo.TRWCOL10 = recordDatiNoCalcolo.AggFamigliaFondo.HasValue ? recordDatiNoCalcolo.AggFamigliaFondo.Value : 0M;
                    datiNonCalcolo.TRWCOL11 = recordDatiNoCalcolo.OnereCaricoAmm.HasValue ? recordDatiNoCalcolo.OnereCaricoAmm.Value : 0M;
                    datiNonCalcolo.TRWCOL12 = recordDatiNoCalcolo.Art21.HasValue ? recordDatiNoCalcolo.Art21.Value : 0M;
                    datiNonCalcolo.TRWCOL13 = recordDatiNoCalcolo.ImportoMensile.HasValue ? recordDatiNoCalcolo.ImportoMensile.Value : 0M;
                    datiNonCalcolo.TRWCOL14 = recordDatiNoCalcolo.Tredicesima.HasValue ? recordDatiNoCalcolo.Tredicesima.Value : 0M;

                    int index = 1;
                    foreach (GestioneComponenteFamiliare.ComponenteFamiliare componenteFamiliare in appListaComponentiFamiliari)
                    {
                        char value;
                        componentiFamiliari.TryGetValue(componenteFamiliare.CodiceFiscale, out value);
                        Utility.SetValueByNameProperty("TRWFAM" + index++.ToString().PadLeft(2, '0'), datiNonCalcolo, value.ToString());
                    }

                    AreaCalcolo.AreaInputVariabile.ListaDatiNonCalcolo.Add(datiNonCalcolo);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaDatiNonCalcolo[0].TRWTPREC));
                }
            }
        }

        public static void ValorizzaGp4Inpdap(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheAventiDiritto = null;
            GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagraficheAventiDiritto);
            if (listaAventiDiritto == null || listaAventiDiritto.Count == 0)
                return;
            List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDiritto = null;
            GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, null, out listaPeriodiAventiDiritto);

            AreaCalcolo.AreaInputVariabile.ListaGp4INPDAP = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP>();

            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP gp4Inpdap = new Data.CMSGTRA.Gp4INPDAP();

            if (datiDanteCausa != null)
            {
                if (datiDanteCausa.CategoriaFascicolo.HasValue && datiDanteCausa.SedeFascicolo.HasValue && datiDanteCausa.NumeroFascicolo.HasValue)
                {
                    gp4Inpdap.K_GP4DAA1 = datiDanteCausa.CategoriaFascicolo.Value;
                    gp4Inpdap.K_GP4DAA2_1 = datiDanteCausa.SedeFascicolo.Value;
                    gp4Inpdap.K_GP4DAA2_2 = datiDanteCausa.NumeroFascicolo.Value;
                }
            }

            if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
            {
                if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                {
                    listaAventiDiritto.ForEach(x => x.ListaPeriodi = listaPeriodiAventiDiritto.FindAll(y => y.IdAventeDiritto == x.Id));
                }

                gp4Inpdap.LISTK_GP4DB00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DB00>();

                foreach (var aventeDiritto in listaAventiDiritto)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DB00 gp4db00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DB00();
                    GestioneAnagrafica.DatiAnagrafici anagraficaAventeDiritto = listaAnagraficheAventiDiritto.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    if (aventeDiritto.IdAnagrafica == datiAnagraficiTitolare.Id)
                    {
                        string codCat = datiPensione.GetCodCategoria();
                        gp4db00.K_GP4KA01 = codCat.Length > 3 ? codCat.Substring(1, 3) : codCat;
                        short sede = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;
                        int nCertificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                        gp4db00.K_GP4KA02 = sede.ToString().PadLeft(4, '0').Substring(0, 2);
                        gp4db00.K_GP4KA03 = sede.ToString().PadLeft(4, '0').Substring(2, 2);
                        gp4db00.K_GP4KA04 = nCertificato.ToString().PadLeft(8, '0');
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(aventeDiritto.CategoriaPensione))
                            gp4db00.K_GP4KA01 = aventeDiritto.CategoriaPensione;
                        if (aventeDiritto.SedePensione.HasValue)
                        {
                            gp4db00.K_GP4KA02 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(0, 2);
                            gp4db00.K_GP4KA03 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(2, 2);
                        }
                        if (aventeDiritto.CertificatoPensione.HasValue)
                            gp4db00.K_GP4KA04 = aventeDiritto.CertificatoPensione.Value.ToString().PadLeft(8, '0');
                    }
                    if (anagraficaAventeDiritto != null && !string.IsNullOrEmpty(anagraficaAventeDiritto.CodiceFiscale))
                        gp4db00.K_GP4DB09 = anagraficaAventeDiritto.CodiceFiscale;
                    if (aventeDiritto.CSog.HasValue)
                        gp4db00.K_GP4DB13 = aventeDiritto.CSog.Value;
                    if (aventeDiritto.IdAnagrafica == datiAnagraficiTitolare.Id)
                    {
                        if (datiDanteCausa != null && datiAnagraficiDanteCausa.DataMatrimonio.HasValue)
                        {
                            int data = 0;
                            int.TryParse(datiAnagraficiDanteCausa.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                                datiAnagraficiDanteCausa.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                                datiAnagraficiDanteCausa.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                            gp4db00.K_GP4DB14 = data;
                        }
                    }
                    else if (aventeDiritto.DataMatrimonio.HasValue)
                    {
                        int data = 0;
                        int.TryParse(aventeDiritto.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                        gp4db00.K_GP4DB14 = data;
                    }
                    gp4db00.K_GP4DB15 = aventeDiritto.CodiceNucleo;
                    if (aventeDiritto.ListaPeriodi != null && aventeDiritto.ListaPeriodi.Count > 0)
                    {
                        gp4db00.LISTK_GP4DC00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00>();
                        foreach (var periodo in aventeDiritto.ListaPeriodi)
                        {
                            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00 gp4dc00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00();
                            if (periodo.PercSpettante.HasValue)
                                gp4dc00.K_GP4DC01 = periodo.PercSpettante.Value;
                            if (periodo.DecorrenzaPeriodo.HasValue)
                                gp4dc00.K_GP4DC02 = int.Parse(periodo.DecorrenzaPeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.DecorrenzaPeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            if (periodo.CessazionePeriodo.HasValue)
                                gp4dc00.K_GP4DC03 = int.Parse(periodo.CessazionePeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.CessazionePeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            else
                                gp4dc00.K_GP4DC03 = 999999;
                            if (periodo.GradoParentela.HasValue)
                            {
                                if (periodo.TipoUnione == "U")
                                    gp4dc00.K_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString() + periodo.TipoUnione;
                                else
                                    gp4dc00.K_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString();
                            }
                            if (periodo.CoeffRiduzione.HasValue)
                                gp4dc00.K_GP4DC05 = periodo.CoeffRiduzione.Value;
                            if (periodo.PercGiudice.HasValue)
                                gp4dc00.K_GP4DC07 = periodo.PercGiudice.Value;

                            gp4db00.LISTK_GP4DC00.Add(gp4dc00);
                        }
                    }
                    else //caso di avente diritto non richiedente senza periodi su GP4
                    {
                        gp4db00.LISTK_GP4DC00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00>();
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00 gp4dc00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4INPDAP.K_GP4DC00();
                        if (aventeDiritto.TipoUnione == "U")
                            gp4dc00.K_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString() + aventeDiritto.TipoUnione;
                        else
                            gp4dc00.K_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString();

                        if (aventeDiritto.DecParentelaDA == 'M' && anagraficaAventeDiritto != null && anagraficaAventeDiritto.DataNascita > datiAnagraficiDanteCausa.DataMorte)
                        {
                            DateTime decorrenza = Utility.FirstDayOfMonth(anagraficaAventeDiritto.DataNascita.Value.AddMonths(1));
                            gp4dc00.K_GP4DC02 = int.Parse(decorrenza.Year.ToString().PadLeft(4, '0') + decorrenza.Month.ToString().PadLeft(2, '0'));
                        }
                        else
                            gp4dc00.K_GP4DC02 = int.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0'));
                        gp4dc00.K_GP4DC03 = 999999;
                        gp4db00.LISTK_GP4DC00.Add(gp4dc00);
                    }

                    gp4Inpdap.LISTK_GP4DB00.Add(gp4db00);
                }
            }
            AreaCalcolo.AreaInputVariabile.ListaGp4INPDAP.Add(gp4Inpdap);
        }

        #region Fondo
        public static void ValorizzaFondoEL(GestionePensione.DatiPensione datiPensione, Object objectFondoXX,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

                AreaCalcolo.AreaInputVariabile.ListaFondoEL = new List<Data.CMSGTRA.Fondo.EL>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    Data.CMSGTRA.Fondo.EL fondoEL = new Data.CMSGTRA.Fondo.EL();
                    fondoEL.XELTIPOR = "X";
                    fondoEL.XELFONDO = "EL";
                    fondoEL.XELPROGR = record;
                    fondoEL.XELDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoEL.XELDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoEL.XELSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoEL.XELSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoEL.XELNONCA = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoEL.XELNATU1 = codNatura1;
                    }
                    fondoEL.XELNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoEL.XELNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoEL.XELPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoEL.XELPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoEL.XELPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoEL.XELUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoEL.XELUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoEL.XELUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoEL != null)
                    {
                        GestioneFondo.DatiFondoEL datiFondoEL = objectFondoXX as GestioneFondo.DatiFondoEL;
                        fondoEL.XELGRADO = datiFondoEL.GradoInvalidita.HasValue ? datiFondoEL.GradoInvalidita.Value : (short)0;
                        fondoEL.XELTEOAA = datiFondoEL.DecorrenzaTeorica.HasValue ? (short)datiFondoEL.DecorrenzaTeorica.Value.Year : (short)0;
                        fondoEL.XELTEOMM = datiFondoEL.DecorrenzaTeorica.HasValue ? (short)datiFondoEL.DecorrenzaTeorica.Value.Month : (short)0;
                        fondoEL.XELRISAA = datiFondoEL.AnnoRiscatti.HasValue ? datiFondoEL.AnnoRiscatti.Value : (short)0;
                        fondoEL.XELRISMM = datiFondoEL.MeseRiscatti.HasValue ? datiFondoEL.MeseRiscatti.Value : (short)0;
                        fondoEL.XELPREAA = datiFondoEL.AnnoAnzianitaPregressa.HasValue ? datiFondoEL.AnnoAnzianitaPregressa.Value : (short)0;
                        fondoEL.XELPREMM = datiFondoEL.MeseAnzianitaPregressa.HasValue ? datiFondoEL.MeseAnzianitaPregressa.Value : (short)0;
                        fondoEL.XELMILAA = datiFondoEL.AnnoServizioMilitare.HasValue ? datiFondoEL.AnnoServizioMilitare.Value : (short)0;
                        fondoEL.XELMILMM = datiFondoEL.MeseServizioMilitare.HasValue ? datiFondoEL.MeseServizioMilitare.Value : (short)0;
                        fondoEL.XELAR3AA = datiFondoEL.AnnoArt3Legge107971.HasValue ? datiFondoEL.AnnoArt3Legge107971.Value : (short)0;
                        fondoEL.XELAR3MM = datiFondoEL.MeseArt3Legge107971.HasValue ? datiFondoEL.MeseArt3Legge107971.Value : (short)0;
                        fondoEL.XELPRENE = datiFondoEL.ProRataEnel.HasValue ? datiFondoEL.ProRataEnel.Value : (short)0;

                        List<GestioneDecodifica.CodiceAzienda> elencoCodiceAzienda = null;
                        GestioneDecodifica.GetCodiceAzienda(out elencoCodiceAzienda);
                        if (elencoCodiceAzienda != null && elencoCodiceAzienda.Count > 0)
                        {
                            GestioneDecodifica.CodiceAzienda codiceAzienda = elencoCodiceAzienda.Find(x => x.Id == (datiFondoEL.CodiceAzienda.HasValue ? datiFondoEL.CodiceAzienda.Value : 0));
                            if (codiceAzienda != null)
                                fondoEL.XELAZIEN = !String.IsNullOrEmpty(codiceAzienda.TraduzioneGp) ? Utility.StringToNullableShort(codiceAzienda.TraduzioneGp).Value : (short)0;
                        }

                        fondoEL.XELCONVE = datiFondoEL.ConvenzioneInternazionale.HasValue ? datiFondoEL.ConvenzioneInternazionale.Value.ToString() : "";
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        if (!string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                        {
                            short resShort = 0;
                            short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                            if (resShort < 10)
                                fondoEL.XELNONVE = resShort;
                        }
                        fondoEL.XELMAGGI = datiMaggiorazioniBenefici.PercentualeMaggiorazione.HasValue ? datiMaggiorazioniBenefici.PercentualeMaggiorazione.Value : (short)0;
                        fondoEL.XELMG336 = datiMaggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670.HasValue ? datiMaggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670.Value : (short)0;
                        fondoEL.XELNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        fondoEL.XELN2336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.Value : 0M;
                        if (datiMaggiorazioniBenefici.ExCombattente.HasValue)
                        {
                            List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                            GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                            if (listaCodiciMaggExComb != null && listaCodiciMaggExComb.Count > 0)
                            {
                                GestioneDecodifica.CodiceMaggiorazioneExCombattenti codMaggExComb = listaCodiciMaggExComb.Find(x => x.Id == datiMaggiorazioniBenefici.ExCombattente.Value);
                                if (codMaggExComb != null)
                                    fondoEL.XELCOMBA = codMaggExComb.TraduzioneSuGP;
                            }
                        }
                    }

                    if (datiIstruttoria != null)
                    {
                        fondoEL.XELSEDE = datiIstruttoria.SedePrecedentePensione.HasValue ? datiIstruttoria.SedePrecedentePensione.Value : (short)0;
                        fondoEL.XELCATEG = datiIstruttoria.CodiceP18PrecedentePensione.HasValue ? datiIstruttoria.CodiceP18PrecedentePensione.Value : (short)0;
                        fondoEL.XELCERTI = datiIstruttoria.CertificatoPrecedentePensione.HasValue ? datiIstruttoria.CertificatoPrecedentePensione.Value : 0;
                    }

                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                        {
                            if (!string.IsNullOrEmpty(sU.Quota))
                            {
                                switch (sU.Quota.Trim().ToUpperInvariant())
                                {
                                    case "A":
                                        fondoEL.XELUTIAA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoEL.XELUTIMM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        fondoEL.XELRETPN = sU.RetribuzionePensionabile.HasValue ? sU.RetribuzionePensionabile.Value : 0M;
                                        break;
                                    case "B":
                                        fondoEL.XELUT2AA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoEL.XELUT2MM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        fondoEL.XELRE2PN = sU.RetribuzionePensionabile.HasValue ? sU.RetribuzionePensionabile.Value : 0M;
                                        break;
                                    case "C":
                                        fondoEL.XELUT3AA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoEL.XELUT3MM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (datiFondo != null)
                    {
                        fondoEL.XELTETTO = datiFondo.RetrPondAnnuaAGOLimite.HasValue ? datiFondo.RetrPondAnnuaAGOLimite.Value : 0M;

                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("EL", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                short res = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out res);
                                fondoEL.XELATTIV = res;
                            }
                        }

                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                                fondoEL.XELSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                        }

                        fondoEL.XELREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : "";
                        fondoEL.XELREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                        fondoEL.XELTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoEL.XELFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoEL.Add(fondoEL);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoEL[0].XELTIPOR));
                }
            }
        }

        public static void ValorizzaFondoTT(GestionePensione.DatiPensione datiPensione, Object objectFondoXX,
           List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
           GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

                AreaCalcolo.AreaInputVariabile.ListaFondoTT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.TT>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.TT fondoTT = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.TT();
                    fondoTT.XTTTIPOR = "X";
                    fondoTT.XTTFONDO = "TT";
                    fondoTT.XTTPROGR = record;
                    fondoTT.XTTDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoTT.XTTDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoTT.XTTSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoTT.XTTSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoTT.XTTNOCAL = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoTT.XTTNATU1 = codNatura1;
                    }
                    fondoTT.XTTNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoTT.XTTNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";

                    fondoTT.XTTPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoTT.XTTPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoTT.XTTPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoTT.XTTUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoTT.XTTUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoTT.XTTUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoTT != null)
                    {
                        GestioneFondo.DatiFondoTT datiFondoTT = objectFondoXX as GestioneFondo.DatiFondoTT;
                        fondoTT.XTTCONVE = datiFondoTT.ConvenzioneInternazionale.HasValue ? datiFondoTT.ConvenzioneInternazionale.Value.ToString() : "";

                        List<GestioneDecodifica.CodiceAzienda> elencoCodiceAzienda = null;
                        GestioneDecodifica.GetCodiceAzienda(out elencoCodiceAzienda);
                        if (elencoCodiceAzienda != null && elencoCodiceAzienda.Count > 0)
                        {
                            GestioneDecodifica.CodiceAzienda codiceAzienda = elencoCodiceAzienda.Find(x => x.Id == (datiFondoTT.Ditta.HasValue ? datiFondoTT.Ditta.Value : 0));
                            if (codiceAzienda != null)
                                fondoTT.XTTDITTA = !String.IsNullOrEmpty(codiceAzienda.TraduzioneGp) ? codiceAzienda.TraduzioneGp.PadLeft(2, ' ') : "";
                        }

                        fondoTT.XTTINAEF = datiFondoTT.RetribuzioneMensileInail.HasValue ? datiFondoTT.RetribuzioneMensileInail.Value : 0M;
                        fondoTT.XTTINARE = datiFondoTT.RenditaInailAnnua.HasValue ? datiFondoTT.RenditaInailAnnua.Value : 0M;
                        fondoTT.XTTLEG58 = datiFondoTT.CodiceArt5L58.HasValue ? datiFondoTT.CodiceArt5L58.Value ? "1" : "0" : "";
                        fondoTT.XTTPNGEN = datiFondoTT.PensioneDirettaGenitori.HasValue ? datiFondoTT.PensioneDirettaGenitori.Value : 0M;
                        fondoTT.XTTRISFGGG = datiFondoTT.PeriodiFigurativiGiorni.HasValue ? (short)datiFondoTT.PeriodiFigurativiGiorni.Value : (short)0;
                        fondoTT.XTTRISFGMM = datiFondoTT.PeriodiFigurativiMesi.HasValue ? (short)datiFondoTT.PeriodiFigurativiMesi.Value : (short)0;
                        fondoTT.XTTRISFGAA = datiFondoTT.PeriodiFigurativiAnni.HasValue ? (short)datiFondoTT.PeriodiFigurativiAnni.Value : (short)0;
                        fondoTT.XTTRISFIGG = datiFondoTT.RiscattiContributiFissiGiorni.HasValue ? (short)datiFondoTT.RiscattiContributiFissiGiorni.Value : (short)0;
                        fondoTT.XTTRISFIMM = datiFondoTT.RiscattiContributiFissiMesi.HasValue ? (short)datiFondoTT.RiscattiContributiFissiMesi.Value : (short)0;
                        fondoTT.XTTRISFIAA = datiFondoTT.RiscattiContributiFissiAnni.HasValue ? (short)datiFondoTT.RiscattiContributiFissiAnni.Value : (short)0;
                        fondoTT.XTTRISMTGG = datiFondoTT.RiscattiRiservaMatematicaGiorni.HasValue ? (short)datiFondoTT.RiscattiRiservaMatematicaGiorni.Value : (short)0;
                        fondoTT.XTTRISMTMM = datiFondoTT.RiscattiRiservaMatematicaMesi.HasValue ? (short)datiFondoTT.RiscattiRiservaMatematicaMesi.Value : (short)0;
                        fondoTT.XTTRISMTAA = datiFondoTT.RiscattiRiservaMatematicaAnni.HasValue ? (short)datiFondoTT.RiscattiRiservaMatematicaAnni.Value : (short)0;
                        fondoTT.XTTSPOBG = datiFondoTT.SupplementoLegge58367.HasValue ? datiFondoTT.SupplementoLegge58367.Value : 0M;

                        //Nel caso in cui la decorrenza della diretta sia >= del 01/1968, allora i campi del mese e anno della decorrenza teorica saranno popolati con la decorrenza della diretta.
                        if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue && datiDanteCausa.DecorrenzaPensione.Value.Year >= 1968)
                        {
                            fondoTT.XTTTEOAA = (short)datiDanteCausa.DecorrenzaPensione.Value.Year;
                            fondoTT.XTTTEOMM = (short)datiDanteCausa.DecorrenzaPensione.Value.Month;
                        }
                        else
                        {
                            fondoTT.XTTTEOAA = datiFondoTT.DecorrenzaTeorica.HasValue ? (short)datiFondoTT.DecorrenzaTeorica.Value.Year :
                            datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                            fondoTT.XTTTEOMM = datiFondoTT.DecorrenzaTeorica.HasValue ? (short)datiFondoTT.DecorrenzaTeorica.Value.Month :
                                datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                        }
                        fondoTT.XTTRTULT = datiFondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue ? datiFondoTT.RetribuzioneUltimoAnnoQuotaA.Value : 0M;
                        fondoTT.XTTRTBIE = datiFondoTT.RetribuzioneBiennio.HasValue ? datiFondoTT.RetribuzioneBiennio.Value : 0M;
                        fondoTT.XTTACCES = datiFondoTT.ElementiAccessori.HasValue ? datiFondoTT.ElementiAccessori.Value : 0M;
                        fondoTT.XTTPEN53 = datiFondoTT.PensioneMensileAl53.HasValue ? datiFondoTT.PensioneMensileAl53.Value : 0M;
                        fondoTT.XTTRTSUP = datiFondoTT.RetribuzioneSupplementi.HasValue ? datiFondoTT.RetribuzioneSupplementi.Value : 0M;
                    }

                    if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        short resShort = 0;
                        short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                        if (resShort < 10)
                            fondoTT.XTTNONVE = resShort;
                    }

                    if (datiIstruttoria != null)
                    {
                        fondoTT.XTTSEDE = datiIstruttoria.SedePrecedentePensione.HasValue ? datiIstruttoria.SedePrecedentePensione.Value : (short)0;
                        fondoTT.XTTCATEG = datiIstruttoria.CodiceP18PrecedentePensione.HasValue ? datiIstruttoria.CodiceP18PrecedentePensione.Value : (short)0;
                        fondoTT.XTTCERTI = datiIstruttoria.CertificatoPrecedentePensione.HasValue ? datiIstruttoria.CertificatoPrecedentePensione.Value : 0;
                    }

                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in datiServizioUtile)
                        {
                            if (!string.IsNullOrEmpty(servizioUtile.Quota))
                            {
                                switch (servizioUtile.Quota)
                                {
                                    // Servizio Utile ante 01/01/93
                                    case "A":
                                        fondoTT.XTTUTIAA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUTIMM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile Ridotto ante 01/01/93
                                    case "A2":
                                        fondoTT.XTTUTRAA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUTRMM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile post 31/12/92
                                    case "B":
                                        fondoTT.XTTRETPN = servizioUtile.RetribuzionePensionabile.HasValue ? servizioUtile.RetribuzionePensionabile.Value : 0M;
                                        fondoTT.XTTUT2AA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUT2MM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile Ridotto post 31/12/92
                                    case "B2":
                                        fondoTT.XTTUTR2A = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUTR2M = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile post 31/12/94
                                    case "C":
                                        fondoTT.XTTUT3AA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUT3MM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile Ridotto post 31/12/94
                                    case "C2":
                                        fondoTT.XTTUTR3A = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUTR3M = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile post 31/12/96
                                    case "D":
                                        fondoTT.XTTRETPD = servizioUtile.RetribuzionePensionabile.HasValue ? servizioUtile.RetribuzionePensionabile.Value : 0M;
                                        fondoTT.XTTUT4AA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUT4MM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                    // Servizio Utile Ridotto post 31/12/96
                                    case "D2":
                                        fondoTT.XTTUTR4A = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoTT.XTTUTR4M = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        break;
                                }
                            }
                        }
                    }

                    if (datiFondo != null)
                    {
                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("TT", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                short res = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out res);
                                fondoTT.XTTATTIV = res;
                            }
                        }
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                            {
                                fondoTT.XTTSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                            }
                        }
                        fondoTT.XTTREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : "";
                        fondoTT.XTTREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                        fondoTT.XTTTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoTT.XTTTETTO = datiFondo.RetrPondAnnuaAGOLimite.HasValue ? datiFondo.RetrPondAnnuaAGOLimite.Value : 0M;
                        fondoTT.XTTFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoTT.Add(fondoTT);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoTT[0].XTTTIPOR));
                }
            }
        }

        public static void ValorizzaFondoET(GestionePensione.DatiPensione datiPensione, Object objectFondoXX, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoET = new List<Data.CMSGTRA.Fondo.ET>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    Data.CMSGTRA.Fondo.ET fondoET = new Data.CMSGTRA.Fondo.ET();
                    fondoET.XETTIPOR = "X";
                    fondoET.XETFONDO = "ET";
                    fondoET.XETPROGR = record;
                    fondoET.XETDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoET.XETDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoET.XETSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoET.XETSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoET.XETNOCAL = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoET.XETNATU1 = codNatura1;
                    }
                    fondoET.XETNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoET.XETNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoET.XETPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoET.XETPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoET.XETPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoET.XETUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoET.XETUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoET.XETUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                            {
                                fondoET.XETSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                            }
                        }

                        fondoET.XETREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : "";
                        fondoET.XETREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                        fondoET.XETTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoET.XETFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoET != null)
                    {
                        GestioneFondo.DatiFondoET datiFondoET = objectFondoXX as GestioneFondo.DatiFondoET;
                        fondoET.XETCOM40 = datiFondoET.Competenze40Percento.HasValue ? datiFondoET.Competenze40Percento.Value : 0M;
                        fondoET.XETEFFET = datiFondoET.RetribuzioneEffettiva.HasValue ? datiFondoET.RetribuzioneEffettiva.Value : 0M;
                        fondoET.XETGRADO = datiFondoET.GradoInvalidita.HasValue ? datiFondoET.GradoInvalidita.Value : (short)0;
                        fondoET.XETINAIL = datiFondoET.ImportoRenditaInail.HasValue ? datiFondoET.ImportoRenditaInail.Value : 0M;
                        fondoET.XETMES13 = datiFondoET.Importo13ma.HasValue ? datiFondoET.Importo13ma.Value : 0M;
                        fondoET.XETMES14 = datiFondoET.Importo14ma.HasValue ? datiFondoET.Importo14ma.Value : 0M;
                        fondoET.XETPGTAB = datiFondoET.Stipendio.HasValue ? datiFondoET.Stipendio.Value : 0M;
                        fondoET.XETRETES = datiFondoET.RetribuzioneEsodo.HasValue ? datiFondoET.RetribuzioneEsodo.Value : 0M;
                        fondoET.XETSLEVA = datiFondoET.NSettimaneLeva.HasValue ? datiFondoET.NSettimaneLeva.Value : (short)0;
                        fondoET.XETSRICH = datiFondoET.NSettimaneRichiamato.HasValue ? datiFondoET.NSettimaneRichiamato.Value : (short)0;
                        fondoET.XETTEOAA = datiFondoET.DecorrenzaTeorica.HasValue ? (short)datiFondoET.DecorrenzaTeorica.Value.Year : (short)0;
                        fondoET.XETTEOMM = datiFondoET.DecorrenzaTeorica.HasValue ? (short)datiFondoET.DecorrenzaTeorica.Value.Month : (short)0;
                        fondoET.XETAAESO = datiFondoET.DataEsonero.HasValue ? (short)datiFondoET.DataEsonero.Value.Year : (short)0;
                        fondoET.XETMMESO = datiFondoET.DataEsonero.HasValue ? (short)datiFondoET.DataEsonero.Value.Month : (short)0;
                        fondoET.XETGGESO = datiFondoET.DataEsonero.HasValue ? (short)datiFondoET.DataEsonero.Value.Day : (short)0;
                        fondoET.XETACCES = datiFondoET.ElementiAccessori.HasValue ? datiFondoET.ElementiAccessori.Value : 0M;
                        fondoET.XETAG402 = datiFondoET.ContributiAgoLegge40245.HasValue ? datiFondoET.ContributiAgoLegge40245.Value : 0M;
                        fondoET.XETAG140 = datiFondoET.ContributiAgoLegge140830.HasValue ? datiFondoET.ContributiAgoLegge140830.Value : 0M;

                        List<GestioneDecodifica.CodiceAzienda> elencoCodiceAzienda = null;
                        GestioneDecodifica.GetCodiceAzienda(out elencoCodiceAzienda);
                        if (elencoCodiceAzienda != null && elencoCodiceAzienda.Count > 0)
                        {
                            GestioneDecodifica.CodiceAzienda codiceAzienda = elencoCodiceAzienda.Find(x => x.Id == (datiFondoET.CodAzienda.HasValue ? datiFondoET.CodAzienda.Value : 0));
                            if (codiceAzienda != null)
                            {
                                fondoET.XETCODAZ = !String.IsNullOrEmpty(codiceAzienda.TraduzioneGp) ? codiceAzienda.TraduzioneGp.Substring(0, 1) : string.Empty;
                                fondoET.XETNUMAZ = !String.IsNullOrEmpty(codiceAzienda.TraduzioneGp) ? int.Parse(codiceAzienda.TraduzioneGp.Substring(1)) : 0;
                            }
                        }
                        fondoET.XETCODES = datiFondoET.CodiceEsodo.HasValue ? datiFondoET.CodiceEsodo.Value ? (short)1 : (short)0 : (short)0;
                        fondoET.XETINTAA = datiFondoET.AAInterruzione.HasValue ? (short)datiFondoET.AAInterruzione.Value : (short)0;
                        fondoET.XETINTMM = datiFondoET.MMInterruzione.HasValue ? (short)datiFondoET.MMInterruzione.Value : (short)0;
                        fondoET.XETINTGG = datiFondoET.GGInterruzione.HasValue ? (short)datiFondoET.GGInterruzione.Value : (short)0;
                        fondoET.XETMILIT = datiFondoET.CodiceServizioMilitare.HasValue ? datiFondoET.CodiceServizioMilitare.Value ? (short)1 : (short)0 : (short)0;
                        fondoET.XETPTCOD = datiFondoET.PartTime.HasValue ? datiFondoET.PartTime.Value ? (short)1 : (short)0 : (short)0;
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        if (!string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                        {
                            short resShort = 0;
                            short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                            if (resShort < 10)
                                fondoET.XETCDCIE = resShort;
                        }
                        fondoET.XETNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        fondoET.XETN2336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.Value : 0M;
                        if (datiMaggiorazioniBenefici.ExCombattente.HasValue)
                        {
                            List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                            GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                            if (listaCodiciMaggExComb != null && listaCodiciMaggExComb.Count > 0)
                            {
                                GestioneDecodifica.CodiceMaggiorazioneExCombattenti codMaggExComb = listaCodiciMaggExComb.Find(x => x.Id == datiMaggiorazioniBenefici.ExCombattente.Value);
                                if (codMaggExComb != null)
                                    fondoET.XETEXCBT = codMaggExComb.TraduzioneSuGP;
                            }
                        }
                    }

                    if (datiIstruttoria != null)
                    {
                        fondoET.XETSEDE = datiIstruttoria.SedePrecedentePensione.HasValue ? datiIstruttoria.SedePrecedentePensione.Value : (short)0;
                        fondoET.XETCATEG = datiIstruttoria.CodiceP18PrecedentePensione.HasValue ? datiIstruttoria.CodiceP18PrecedentePensione.Value : (short)0;
                        fondoET.XETCERTI = datiIstruttoria.CertificatoPrecedentePensione.HasValue ? datiIstruttoria.CertificatoPrecedentePensione.Value : 0;
                    }

                    List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);
                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                        {
                            if (!string.IsNullOrEmpty(sU.Quota))
                            {
                                switch (sU.Quota.Trim().ToUpperInvariant())
                                {
                                    case "A":
                                        fondoET.XETUTIAA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoET.XETUTIMM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        fondoET.XETUTIGG = sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : (short)0;
                                        fondoET.XETRETPN = sU.RetribuzionePensionabile.HasValue ? sU.RetribuzionePensionabile.Value : 0M;
                                        break;
                                    case "B":
                                        fondoET.XETUT2AA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoET.XETUT2MM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        fondoET.XETUT2GG = sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : (short)0;
                                        fondoET.XETRE2PN = sU.RetribuzionePensionabile.HasValue ? sU.RetribuzionePensionabile.Value : 0M;
                                        break;
                                    case "C":
                                        fondoET.XETUT3AA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                                        fondoET.XETUT3MM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                                        fondoET.XETUT3GG = sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : (short)0;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoET.Add(fondoET);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoET[0].XETTIPOR));
                }
            }
        }

        public static void ValorizzaFondoVL(Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

                AreaCalcolo.AreaInputVariabile.ListaFondoVL = new List<Data.CMSGTRA.Fondo.VL>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    Data.CMSGTRA.Fondo.VL fondoVL = new Data.CMSGTRA.Fondo.VL();
                    fondoVL.XVLTIPOR = "X";
                    fondoVL.XVLFONDO = "VL";
                    fondoVL.XVLPROGR = record;
                    fondoVL.XVLDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoVL.XVLDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoVL.XVLSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoVL.XVLSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoVL.XVLNONCA = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoVL.XVLNATU1 = codNatura1;
                    }
                    fondoVL.XVLNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoVL.XVLNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoVL.XVLPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoVL.XVLPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoVL.XVLPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoVL.XVLUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoVL.XVLUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoVL.XVLUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("VL", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                short res = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out res);
                                if (res != 0 && res.ToString().Length > 1)
                                {
                                    fondoVL.XVLATTI1 = short.Parse(res.ToString().Substring(0, 1));
                                    fondoVL.XVLATTI2 = short.Parse(res.ToString().Substring(1, 2));
                                }
                                else
                                    fondoVL.XVLATTI1 = res;
                            }
                        }

                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                                fondoVL.XVLSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                        }

                        fondoVL.XVLREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : "";
                        fondoVL.XVLREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                        fondoVL.XVLTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoVL.XVLFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoVL != null)
                    {
                        GestioneFondo.DatiFondoVL datiFondoVL = objectFondoXX as GestioneFondo.DatiFondoVL;
                        fondoVL.XVLCODCP = datiFondoVL.CodiceCapitalizzazione.HasValue ? datiFondoVL.CodiceCapitalizzazione.Value : (short)0;
                        fondoVL.XVLIMPCP = datiFondoVL.ImportoPercentualeCapitalizzazione.HasValue ? datiFondoVL.ImportoPercentualeCapitalizzazione.Value : 0M;
                        fondoVL.XVLRISAA = datiFondoVL.RiscattiRicongiunzioniAA.HasValue ? (short)datiFondoVL.RiscattiRicongiunzioniAA.Value : (short)0;
                        fondoVL.XVLRISGG = datiFondoVL.RiscattiRicongiunzioniGG.HasValue ? (short)datiFondoVL.RiscattiRicongiunzioniGG.Value : (short)0;
                        fondoVL.XVLRISMM = datiFondoVL.RiscattiRicongiunzioniMM.HasValue ? (short)datiFondoVL.RiscattiRicongiunzioniMM.Value : (short)0;
                        fondoVL.XVLVOLAA = datiFondoVL.ProsecuzioneVolontariaAA.HasValue ? (short)datiFondoVL.ProsecuzioneVolontariaAA.Value : (short)0;
                        fondoVL.XVLVOLGG = datiFondoVL.ProsecuzioneVolontariaGG.HasValue ? (short)datiFondoVL.ProsecuzioneVolontariaGG.Value : (short)0;
                        fondoVL.XVLVOLMM = datiFondoVL.ProsecuzioneVolontariaMM.HasValue ? (short)datiFondoVL.ProsecuzioneVolontariaMM.Value : (short)0;
                        fondoVL.XVLART22 = datiFondoVL.CodiceArt22.HasValue ? datiFondoVL.CodiceArt22.Value : (short)0;
                        fondoVL.XVLINVAA = datiFondoVL.DataInvalidita.HasValue ? (short)datiFondoVL.DataInvalidita.Value.Year : (short)0;
                        fondoVL.XVLINVMM = datiFondoVL.DataInvalidita.HasValue ? (short)datiFondoVL.DataInvalidita.Value.Month : (short)0;
                        fondoVL.XVLINVGG = datiFondoVL.DataInvalidita.HasValue ? (short)datiFondoVL.DataInvalidita.Value.Day : (short)0;
                        fondoVL.XVLIRINT = datiFondoVL.AliquotaIrpef.HasValue ?
                            short.Parse(datiFondoVL.AliquotaIrpef.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture).Substring(0, datiFondoVL.AliquotaIrpef.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture).IndexOf(','))) : (short)0;
                        fondoVL.XVLIRDEC = datiFondoVL.AliquotaIrpef.HasValue ?
                            short.Parse(datiFondoVL.AliquotaIrpef.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture).Substring((datiFondoVL.AliquotaIrpef.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture).IndexOf(',') + 1))) : (short)0;
                        fondoVL.XVLSETT1 = datiFondoVL.RetribuzioneSettimanaleAgoQuotaA.HasValue ? datiFondoVL.RetribuzioneSettimanaleAgoQuotaA.Value : 0M;
                        fondoVL.XVLSETT2 = datiFondoVL.RetribuzioneSettimanaleAgoQuotaB.HasValue ? datiFondoVL.RetribuzioneSettimanaleAgoQuotaB.Value : 0M;
                    }

                    if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        short resShort = 0;
                        short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                        if (resShort < 10)
                            fondoVL.XVLNONVE = resShort;
                    }

                    if (datiIstruttoria != null)
                    {
                    }

                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in datiServizioUtile)
                        {
                            if (!string.IsNullOrEmpty(servizioUtile.Quota))
                            {
                                switch (servizioUtile.Quota)
                                {
                                    // dati servizio Utile ante 27/11/88
                                    case "A":
                                        fondoVL.XVLRETPN = servizioUtile.RetribuzionePensionabile.HasValue ? servizioUtile.RetribuzionePensionabile.Value : 0M;
                                        fondoVL.XVLUT1AA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoVL.XVLUT1MM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        fondoVL.XVLUT1GG = servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : (short)0;
                                        break;
                                    // dati servizio Utile ante '93
                                    case "A2":
                                        fondoVL.XVLUTIAA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoVL.XVLUTIMM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        fondoVL.XVLUTIGG = servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : (short)0;
                                        break;
                                    // dati servizio Utile post '92
                                    case "B":
                                        fondoVL.XVLRE1PN = servizioUtile.RetribuzionePensionabile.HasValue ? servizioUtile.RetribuzionePensionabile.Value : 0M;
                                        fondoVL.XVLUTBAA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoVL.XVLUTBMM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        fondoVL.XVLUTBGG = servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : (short)0;
                                        break;
                                    // dati servizio Utile post '94
                                    case "C":
                                        fondoVL.XVLUTCAA = servizioUtile.ServizioUtileAA.HasValue ? servizioUtile.ServizioUtileAA.Value : (short)0;
                                        fondoVL.XVLUTCMM = servizioUtile.ServizioUtileMM.HasValue ? servizioUtile.ServizioUtileMM.Value : (short)0;
                                        fondoVL.XVLUTCGG = servizioUtile.ServizioUtileGG.HasValue ? servizioUtile.ServizioUtileGG.Value : (short)0;
                                        break;
                                }
                            }
                        }
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoVL.Add(fondoVL);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoVL[0].XVLTIPOR));
                }
            }
        }

        public static void ValorizzaFondoPT(GestionePensione.DatiPensione datiPensione, Object objectFondoXX, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, bool isNuovaGestione, out List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtileByIdPensione, ref Data.FSPL_FSRC AreaCalcolo)
        {
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            datiServizioUtileByIdPensione = null;
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                long idPensione = datiPensione.Id;
                List<GestioneFondo.DatiFondoPT> lstDatiFondoPT = objectFondoXX as List<GestioneFondo.DatiFondoPT>;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(idPensione, out datiServizioUtileByIdPensione);

                List<GestioneDecodifica.CodiceSpecifico> listaDecodificaCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out listaDecodificaCodiceSpecifico);
                bool isRicostituzioneIndiretta = false;
                if (listaDecodificaCodiceSpecifico != null && listaDecodificaCodiceSpecifico.Count > 0)
                {
                    if (datiFondo != null)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = listaDecodificaCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico.GetValueOrDefault());
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S") && codiceSpecifico != null && codiceSpecifico.TraduzioneGp.GetValueOrDefault() == 'H')
                            isRicostituzioneIndiretta = true;
                    }
                }

                if (AreaCalcolo.UtilizzaNuovoTracciato)
                {
                    AreaCalcolo.AreaInputVariabile.ListaFondoPT_New = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT_New>();
                    short record = 0;
                    bool is460Presente = false;
                    for (int i = 0; i < listaRecordFondo.Count; i++)
                    {
                        long idRecordFondo = listaRecordFondo[i].Id;
                        GestioneFondo.DatiFondoPT datiFondoPT = null;
                        if (isNuovaGestione)
                            datiFondoPT = lstDatiFondoPT.Find(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiFondoPT = lstDatiFondoPT.FirstOrDefault();
                        record++;
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT_New fondoPT = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT_New();
                        fondoPT.XFSTIPOR = "X";
                        fondoPT.XFSFONDO = "PT";
                        fondoPT.XFSPROGR = record;
                        fondoPT.XFSDECAA = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Year : (short)0;
                        fondoPT.XFSDECMM = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Month : (short)0;
                        fondoPT.XFSDECGG = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Day : (short)0;
                        fondoPT.XFSSCAAA = listaRecordFondo[i].DataSospensione.HasValue ? (short)listaRecordFondo[i].DataSospensione.Value.Year : (short)0;
                        fondoPT.XFSSCAMM = listaRecordFondo[i].DataSospensione.HasValue ? (short)listaRecordFondo[i].DataSospensione.Value.Month : (short)0;
                        fondoPT.XFSNCALC = listaRecordFondo[i].CodiceNonCalcolo.GetValueOrDefault() == 'S' ? "1" : "0";
                        fondoPT.XFSNATU1 = listaRecordFondo[i].CodiceNatura1.HasValue ? listaRecordFondo[i].CodiceNatura1.Value.ToString() : (datiFondoPT != null && datiFondoPT.TitolareAltraPensione.HasValue ? (datiFondoPT.TitolareAltraPensione.Value ? "6" : "") : "");
                        //ENG - Reversibilita 024 XFSNATU1 valorizzato con il primo byte del codice natura
                        if (Utility.IsDomandaReversibilita(datiPensione) && datiPensione != null && !String.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length >= 1)
                        {
                            fondoPT.XFSNATU1 = datiPensione.NaturaPensione.Substring(0, 1);
                        }
                        fondoPT.XFSNATU2 = listaRecordFondo[i].CodiceNatura2.HasValue ? listaRecordFondo[i].CodiceNatura2.Value.ToString() : "";
                        fondoPT.XFSNATU3 = listaRecordFondo[i].CodiceNatura3.HasValue ? listaRecordFondo[i].CodiceNatura3.Value.ToString() : "";
                        fondoPT.XFSFLINP = "0";
                        fondoPT.XFSASSAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                        fondoPT.XFSASSMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                        fondoPT.XFSASSGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                        fondoPT.XFSCESAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                        fondoPT.XFSCESMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                        fondoPT.XFSCESGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                        if (datiFondo != null)
                        {
                            List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                            if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                            {
                                GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                                if (codiceSpecifico != null)
                                {
                                    fondoPT.XFSCSPEC = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                                }
                            }
                            fondoPT.XFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                            if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                            {
                                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                                GestioneDecodifica.GetAttivitaSvoltaByFondo("PT", null, out elencoAttivitaSvolte);
                                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                                {
                                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                    fondoPT.XFSPROF = attSvolta.TraduzioneSuGp;
                                }
                            }
                        }

                        if (datiFondoPT != null)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                            {
                                GestioneDecodifica.DecodificaCausaCessazione causaCess = ListaCausaCess.Find(x => x.Id == (datiFondoPT.CausaCessazione.HasValue ? datiFondoPT.CausaCessazione.Value : 0));
                                if (causaCess != null)
                                {
                                    short resShort = 0;
                                    short.TryParse(causaCess.TraduzioneSuGP, out resShort);
                                    fondoPT.XFSCAUSA = resShort;
                                }
                            }
                            fondoPT.XFSCONG = (datiFondoPT.IndennitaIntegrativaSpecialeConglobata.HasValue ?
                                datiFondoPT.IndennitaIntegrativaSpecialeConglobata.Value ? "1" : "0" : "0") +
                                (datiFondoPT.IntegrazioneMinimo.HasValue ?
                                datiFondoPT.IntegrazioneMinimo.Value ? "1" : "0" : "0");
                            fondoPT.XFSRID = datiFondoPT.RiduzioneL537.HasValue ?
                                (datiFondoPT.RiduzioneL537.Value ?
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "3" : "1" : "1") :
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "2" : "0" : "0")) :
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "2" : "0" : "0");
                            fondoPT.XFSONEREMEF = datiFondoPT.OnereMEF.HasValue ? datiFondoPT.OnereMEF.Value ? (short)1 : (short)0 : (short)0;
                            fondoPT.XRIPINPDAP = datiFondoPT.RipartizioneInpdap.HasValue ? datiFondoPT.RipartizioneInpdap.Value : 0;
                            fondoPT.XFSDIIS = datiFondoPT.DirittoIndennitaIntegrativaSpeciale.HasValue ?
                                        (datiFondoPT.DirittoIndennitaIntegrativaSpeciale.Value ?
                                        (datiFondoPT.PagamentoIndennitaIntegrativaSpeciale.HasValue ?
                                        datiFondoPT.PagamentoIndennitaIntegrativaSpeciale.Value ? (short)1 : (short)2 : (short)2) : (short)0) : (short)0;
                            fondoPT.XFSDECAL = datiFondoPT.DecorrenzaCalcolo.HasValue ? int.Parse(datiFondoPT.DecorrenzaCalcolo.Value.Year.ToString().PadLeft(4, '0') +
                                        datiFondoPT.DecorrenzaCalcolo.Value.Month.ToString().PadLeft(2, '0') +
                                        datiFondoPT.DecorrenzaCalcolo.Value.Day.ToString().PadLeft(2, '0')) : 0;
                            fondoPT.XFSF13ME = datiFondoPT.TrediciMensilita.HasValue ? datiFondoPT.TrediciMensilita.Value ? (short)1 : (short)0 : (short)0;

                            if ((Utility.IsDomandaPensioneIndiretta(datiPensione) || isRicostituzioneIndiretta) && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1996, 01, 01)))
                                fondoPT.XFSFAAGO = 1;
                        }

                        //if(vecchia gestione)
                        if (!isNuovaGestione)
                        {
                            if (datiFondoPT != null)
                            {
                                if (is460Presente)
                                {
                                    fondoPT.XFSDECAA = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Year : (short)0;
                                    fondoPT.XFSDECMM = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Month : (short)0;
                                    fondoPT.XFSDECGG = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Day : (short)0;
                                    fondoPT.XFSMESIRIS = datiFondoPT.NMesiRiscattati.HasValue ? datiFondoPT.NMesiRiscattati.Value : 0;
                                    fondoPT.XFSMESITOT = datiFondoPT.NMesiTotali.HasValue ? datiFondoPT.NMesiTotali.Value : 0;
                                    fondoPT.XFSPENS = long.Parse((datiFondoPT.SiglaCategoria.HasValue ? datiFondoPT.SiglaCategoria.Value.ToString().PadLeft(3, '0') : "000") +
                                        (datiFondoPT.CodiceSede.HasValue ? datiFondoPT.CodiceSede.ToString().PadLeft(4, '0') : "0000") +
                                        (datiFondoPT.Ncertificato.HasValue ? datiFondoPT.Ncertificato.Value.ToString().PadLeft(8, '0') : "00000000"));
                                }
                                if (is460Presente)
                                {
                                    fondoPT.XFSDECECAA = fondoPT.XFSDECAA;
                                    fondoPT.XFSDECECMM = fondoPT.XFSDECMM;
                                    fondoPT.XFSDECECGG = fondoPT.XFSDECGG;
                                }
                                else
                                {
                                    fondoPT.XFSDECECAA = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Year : (short)0;
                                    fondoPT.XFSDECECMM = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Month : (short)0;
                                    fondoPT.XFSDECECGG = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Day : (short)0;
                                }
                            }
                        }
                        //end if
                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = datiServizioUtileByIdPensione.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = datiServizioUtileByIdPensione;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            fondoPT.XFSQA14 = sU.QuoteArt14.HasValue ? sU.QuoteArt14.Value : 0M;
                                            fondoPT.XFSRETR = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoPT.XFSSU92 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            fondoPT.XFSIIS = sU.ImportoIndennitaIntegrativaSpeciale.HasValue ? sU.ImportoIndennitaIntegrativaSpeciale.Value : 0M;
                                            break;
                                        case "B1":
                                            fondoPT.XFSRETRM = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoPT.XFSSU94 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B2":
                                            fondoPT.XFSSU95 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B3":
                                            fondoPT.XFSSU97 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B4":
                                            if (sU.ServizioUtileCessazioneAA.HasValue || sU.ServizioUtileCessazioneMM.HasValue || sU.ServizioUtileCessazioneGG.HasValue)
                                            {
                                                fondoPT.XFSSUCE = ((((sU.ServizioUtileCessazioneAA.HasValue ? sU.ServizioUtileCessazioneAA.Value * 12 : 0) +
                                                    (sU.ServizioUtileCessazioneMM.HasValue ? sU.ServizioUtileCessazioneMM.Value : 0)) * 30) +
                                                    (sU.ServizioUtileCessazioneGG.HasValue ? sU.ServizioUtileCessazioneGG.Value : 0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        if (datiFondoPT != null)
                        {
                            //if(nuova gestione) valorizzo variabili di
                            if (isNuovaGestione)
                            {
                                //dati legge 460 non presente 
                                if (datiFondoPT.IsLegge460Null())
                                {
                                    if (datiFondoPT.ScadenzaIllimitata == true)
                                    {
                                        fondoPT.XFSDECECAA = 9999;
                                        fondoPT.XFSDECECMM = 99;
                                        fondoPT.XFSDECECGG = 99;
                                    }
                                    else
                                    {
                                        fondoPT.XFSDECECAA = datiFondoPT.ScadenzaBenefici.HasValue ? (short)datiFondoPT.ScadenzaBenefici.Value.Year : (short)0;
                                        fondoPT.XFSDECECMM = datiFondoPT.ScadenzaBenefici.HasValue ? (short)datiFondoPT.ScadenzaBenefici.Value.Month : (short)0;
                                        fondoPT.XFSDECECGG = datiFondoPT.ScadenzaBenefici.HasValue ? (short)1 : (short)0;
                                    }
                                }
                                //dati legge 460 presente
                                else
                                {
                                    fondoPT.XFSDECECAA = fondoPT.XFSDECAA;//AA decorrenzaRegistrazione
                                    fondoPT.XFSDECECMM = fondoPT.XFSDECMM;//MM decorrenzaRegistrazione
                                    fondoPT.XFSDECECGG = fondoPT.XFSDECGG;//GG decorrenzaRegistrazione
                                    fondoPT.XFSMESIRIS = datiFondoPT.NMesiRiscattati.HasValue ? datiFondoPT.NMesiRiscattati.Value : 0;
                                    fondoPT.XFSMESITOT = datiFondoPT.NMesiTotali.HasValue ? datiFondoPT.NMesiTotali.Value : 0;
                                    fondoPT.XFSPENS = long.Parse((datiFondoPT.SiglaCategoria.HasValue ? datiFondoPT.SiglaCategoria.Value.ToString().PadLeft(3, '0') : "000") +
                                        (datiFondoPT.CodiceSede.HasValue ? datiFondoPT.CodiceSede.ToString().PadLeft(4, '0') : "0000") +
                                        (datiFondoPT.Ncertificato.HasValue ? datiFondoPT.Ncertificato.Value.ToString().PadLeft(8, '0') : "00000000"));
                                }
                                fondoPT.XFSNO336 = datiFondoPT.RMSSenzaLegge33670QA.HasValue ? datiFondoPT.RMSSenzaLegge33670QA.Value : 0M;
                                fondoPT.XFSPAL335 = datiFondoPT.PALConBenefici.HasValue ? datiFondoPT.PALConBenefici.Value : 0M;
                            }

                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            {
                                if (datiFondoPT.PensioneAnnuaLorda214.HasValue && datiFondoPT.PensioneAnnuaLorda707.HasValue)
                                {
                                    if (Decimal.Compare(datiFondoPT.PensioneAnnuaLorda214.Value, datiFondoPT.PensioneAnnuaLorda707.Value) <= 0)
                                        fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda214.Value;
                                    else
                                        fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda707.Value;
                                }
                                else if (datiFondoPT.PensioneAnnuaLorda214.HasValue)
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda214.Value;
                                else if (datiFondoPT.PensioneAnnuaLorda707.HasValue)
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda707.Value;
                                else
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda.HasValue ? datiFondoPT.PensioneAnnuaLorda.Value : 0;
                            }
                            else
                            {
                                fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda.HasValue ? datiFondoPT.PensioneAnnuaLorda.Value : 0;
                            }

                            fondoPT.XFSSUAN = datiFondoPT.ServizioUtileDirittoAA.HasValue ? datiFondoPT.ServizioUtileDirittoAA.Value : (short)0;
                            fondoPT.XFSSUANMM = datiFondoPT.ServizioUtileDirittoMM.HasValue ? datiFondoPT.ServizioUtileDirittoMM.Value : (short)0;
                            fondoPT.XFSSUANGG = datiFondoPT.ServizioUtileDirittoGG.HasValue ? datiFondoPT.ServizioUtileDirittoGG.Value : (short)0;
                            fondoPT.XFSIMPC = datiFondoPT.IncrementoContrattuale.HasValue ? datiFondoPT.IncrementoContrattuale.Value : 0;
                            fondoPT.XFSSETDIR = datiFondoPT.VVUtiliDiritto.HasValue ? datiFondoPT.VVUtiliDiritto.Value : (short)0;
                            fondoPT.XFSSETMIS = datiFondoPT.VVUtiliMisura.HasValue ? datiFondoPT.VVUtiliMisura.Value : (short)0;

                            int assac = 0;
                            GetASSAC(datiFondoPT.PrivilegiataSuperinvaliditaIndennita, datiFondoPT.AssegnoIntegrativo, datiFondoPT.IntegrazioneIndennitaAssistenza,
                                datiFondoPT.IndennitaAccompagnamentoAggiuntiva, datiFondoPT.CumuloInfermita, datiFondoPT.Categoria2aInfermita, datiFondoPT.AssegnoCura,
                                datiFondoPT.IndennitaSpecialeAnnua, out assac);
                            fondoPT.XFSASSAC = assac;


                            AreaCalcolo.AreaInputVariabile.ListaFondoPT_New.Add(fondoPT);
                            AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoPT_New[i].XFSTIPOR));

                            //if(vecchia gestione)
                            if (!isNuovaGestione)
                            {
                                if (record == 1 && ((datiFondoPT.NMesiRiscattati.HasValue && datiFondoPT.NMesiRiscattati.Value > 0) ||
                                            (datiFondoPT.NMesiTotali.HasValue && datiFondoPT.NMesiTotali.Value > 0) ||
                                            (datiFondoPT.SiglaCategoria.HasValue && datiFondoPT.SiglaCategoria.Value > 0) ||
                                            (datiFondoPT.CodiceSede.HasValue && datiFondoPT.CodiceSede.Value > 0) ||
                                            (datiFondoPT.Ncertificato.HasValue && datiFondoPT.Ncertificato.Value > 0) ||
                                            (datiFondoPT.DecorrenzaSecondaria.HasValue)))
                                {
                                    is460Presente = true;
                                    i--;
                                }
                                else
                                    is460Presente = false;
                            }
                            //end if
                        }
                    }
                }
                else
                {
                    AreaCalcolo.AreaInputVariabile.ListaFondoPT = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT>();
                    short record = 0;
                    bool is460Presente = false;
                    for (int i = 0; i < listaRecordFondo.Count; i++)
                    {
                        long idRecordFondo = listaRecordFondo[i].Id;
                        GestioneFondo.DatiFondoPT datiFondoPT = null;
                        if (isNuovaGestione)
                            datiFondoPT = lstDatiFondoPT.Find(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiFondoPT = lstDatiFondoPT.FirstOrDefault();
                        record++;
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT fondoPT = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PT();
                        fondoPT.XFSTIPOR = "X";
                        fondoPT.XFSFONDO = "PT";
                        fondoPT.XFSPROGR = record;
                        fondoPT.XFSDECAA = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Year : (short)0;
                        fondoPT.XFSDECMM = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Month : (short)0;
                        fondoPT.XFSDECGG = listaRecordFondo[i].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[i].DecorrenzaValiditaDati.Value.Day : (short)0;
                        fondoPT.XFSSCAAA = listaRecordFondo[i].DataSospensione.HasValue ? (short)listaRecordFondo[i].DataSospensione.Value.Year : (short)0;
                        fondoPT.XFSSCAMM = listaRecordFondo[i].DataSospensione.HasValue ? (short)listaRecordFondo[i].DataSospensione.Value.Month : (short)0;
                        fondoPT.XFSNCALC = listaRecordFondo[i].CodiceNonCalcolo.GetValueOrDefault() == 'S' ? "1" : "0";
                        fondoPT.XFSNATU1 = listaRecordFondo[i].CodiceNatura1.HasValue ? listaRecordFondo[i].CodiceNatura1.Value.ToString() : (datiFondoPT != null && datiFondoPT.TitolareAltraPensione.HasValue ? (datiFondoPT.TitolareAltraPensione.Value ? "6" : "") : "");
                        //ENG - Reversibilita 024 XFSNATU1 valorizzato con il primo byte del codice natura
                        if (Utility.IsDomandaReversibilita(datiPensione) && datiPensione != null && !String.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length >= 1)
                        {
                            fondoPT.XFSNATU1 = datiPensione.NaturaPensione.Substring(0, 1);
                        }
                        fondoPT.XFSNATU2 = listaRecordFondo[i].CodiceNatura2.HasValue ? listaRecordFondo[i].CodiceNatura2.Value.ToString() : "";
                        fondoPT.XFSNATU3 = listaRecordFondo[i].CodiceNatura3.HasValue ? listaRecordFondo[i].CodiceNatura3.Value.ToString() : "";
                        fondoPT.XFSFLINP = "0";
                        fondoPT.XFSASSAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                        fondoPT.XFSASSMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                        fondoPT.XFSASSGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                        fondoPT.XFSCESAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                        fondoPT.XFSCESMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                        fondoPT.XFSCESGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                        if (datiFondo != null)
                        {
                            List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                            if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                            {
                                GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                                if (codiceSpecifico != null)
                                {
                                    fondoPT.XFSCSPEC = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                                }
                            }
                            fondoPT.XFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                            if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                            {
                                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                                GestioneDecodifica.GetAttivitaSvoltaByFondo("PT", null, out elencoAttivitaSvolte);
                                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                                {
                                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                    fondoPT.XFSPROF = attSvolta.TraduzioneSuGp;
                                }
                            }
                        }

                        if (datiFondoPT != null)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                            {
                                GestioneDecodifica.DecodificaCausaCessazione causaCess = ListaCausaCess.Find(x => x.Id == (datiFondoPT.CausaCessazione.HasValue ? datiFondoPT.CausaCessazione.Value : 0));
                                if (causaCess != null)
                                {
                                    short resShort = 0;
                                    short.TryParse(causaCess.TraduzioneSuGP, out resShort);
                                    fondoPT.XFSCAUSA = resShort;
                                }
                            }
                            fondoPT.XFSCONG = (datiFondoPT.IndennitaIntegrativaSpecialeConglobata.HasValue ?
                                datiFondoPT.IndennitaIntegrativaSpecialeConglobata.Value ? "1" : "0" : "0") +
                                (datiFondoPT.IntegrazioneMinimo.HasValue ?
                                datiFondoPT.IntegrazioneMinimo.Value ? "1" : "0" : "0");
                            fondoPT.XFSRID = datiFondoPT.RiduzioneL537.HasValue ?
                                (datiFondoPT.RiduzioneL537.Value ?
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "3" : "1" : "1") :
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "2" : "0" : "0")) :
                                (datiFondoPT.IISAbbattimentoAnni.HasValue ?
                                datiFondoPT.IISAbbattimentoAnni.Value ? "2" : "0" : "0");
                            fondoPT.XFSONEREMEF = datiFondoPT.OnereMEF.HasValue ? datiFondoPT.OnereMEF.Value ? (short)1 : (short)0 : (short)0;
                            fondoPT.XRIPINPDAP = datiFondoPT.RipartizioneInpdap.HasValue ? datiFondoPT.RipartizioneInpdap.Value : 0;
                            fondoPT.XFSDIIS = datiFondoPT.DirittoIndennitaIntegrativaSpeciale.HasValue ?
                                        (datiFondoPT.DirittoIndennitaIntegrativaSpeciale.Value ?
                                        (datiFondoPT.PagamentoIndennitaIntegrativaSpeciale.HasValue ?
                                        datiFondoPT.PagamentoIndennitaIntegrativaSpeciale.Value ? (short)1 : (short)2 : (short)2) : (short)0) : (short)0;
                            fondoPT.XFSDECAL = datiFondoPT.DecorrenzaCalcolo.HasValue ? int.Parse(datiFondoPT.DecorrenzaCalcolo.Value.Year.ToString().PadLeft(4, '0') +
                                        datiFondoPT.DecorrenzaCalcolo.Value.Month.ToString().PadLeft(2, '0') +
                                        datiFondoPT.DecorrenzaCalcolo.Value.Day.ToString().PadLeft(2, '0')) : 0;
                            fondoPT.XFSF13ME = datiFondoPT.TrediciMensilita.HasValue ? datiFondoPT.TrediciMensilita.Value ? (short)1 : (short)0 : (short)0;

                            if ((Utility.IsDomandaPensioneIndiretta(datiPensione) || isRicostituzioneIndiretta) && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1996, 01, 01)))
                                fondoPT.XFSFAAGO = 1;
                        }

                        //if(vecchia gestione)
                        if (!isNuovaGestione)
                        {
                            if (datiFondoPT != null)
                            {
                                if (is460Presente)
                                {
                                    fondoPT.XFSDECAA = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Year : (short)0;
                                    fondoPT.XFSDECMM = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Month : (short)0;
                                    fondoPT.XFSDECGG = datiFondoPT.DecorrenzaSecondaria.HasValue ? (short)datiFondoPT.DecorrenzaSecondaria.Value.Day : (short)0;
                                    fondoPT.XFSMESIRIS = datiFondoPT.NMesiRiscattati.HasValue ? datiFondoPT.NMesiRiscattati.Value : 0;
                                    fondoPT.XFSMESITOT = datiFondoPT.NMesiTotali.HasValue ? datiFondoPT.NMesiTotali.Value : 0;
                                    fondoPT.XFSPENS = long.Parse((datiFondoPT.SiglaCategoria.HasValue ? datiFondoPT.SiglaCategoria.Value.ToString().PadLeft(3, '0') : "000") +
                                        (datiFondoPT.CodiceSede.HasValue ? datiFondoPT.CodiceSede.ToString().PadLeft(4, '0') : "0000") +
                                        (datiFondoPT.Ncertificato.HasValue ? datiFondoPT.Ncertificato.Value.ToString().PadLeft(8, '0') : "00000000"));
                                }
                                if (is460Presente)
                                {
                                    fondoPT.XFSDECECAA = fondoPT.XFSDECAA;
                                    fondoPT.XFSDECECMM = fondoPT.XFSDECMM;
                                    fondoPT.XFSDECECGG = fondoPT.XFSDECGG;
                                }
                                else
                                {
                                    fondoPT.XFSDECECAA = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Year : (short)0;
                                    fondoPT.XFSDECECMM = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Month : (short)0;
                                    fondoPT.XFSDECECGG = datiFondoPT.DecorrenzaEconomica.HasValue ? (short)datiFondoPT.DecorrenzaEconomica.Value.Day : (short)0;
                                }
                            }
                        }
                        //end if
                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = datiServizioUtileByIdPensione.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = datiServizioUtileByIdPensione;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            fondoPT.XFSQA14 = sU.QuoteArt14.HasValue ? sU.QuoteArt14.Value : 0M;
                                            fondoPT.XFSRETR = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoPT.XFSSU92 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            fondoPT.XFSIIS = sU.ImportoIndennitaIntegrativaSpeciale.HasValue ? sU.ImportoIndennitaIntegrativaSpeciale.Value : 0M;
                                            break;
                                        case "B1":
                                            fondoPT.XFSRETRM = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoPT.XFSSU94 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B2":
                                            fondoPT.XFSSU95 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B3":
                                            fondoPT.XFSSU97 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B4":
                                            if (sU.ServizioUtileCessazioneAA.HasValue || sU.ServizioUtileCessazioneMM.HasValue || sU.ServizioUtileCessazioneGG.HasValue)
                                            {
                                                fondoPT.XFSSUCE = ((((sU.ServizioUtileCessazioneAA.HasValue ? sU.ServizioUtileCessazioneAA.Value * 12 : 0) +
                                                    (sU.ServizioUtileCessazioneMM.HasValue ? sU.ServizioUtileCessazioneMM.Value : 0)) * 30) +
                                                    (sU.ServizioUtileCessazioneGG.HasValue ? sU.ServizioUtileCessazioneGG.Value : 0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        if (datiFondoPT != null)
                        {
                            //if(nuova gestione) valorizzo variabili di
                            if (isNuovaGestione)
                            {
                                //dati legge 460 non presente 
                                if (datiFondoPT.IsLegge460Null())
                                {
                                    if (datiFondoPT.ScadenzaIllimitata == true)
                                    {
                                        fondoPT.XFSDECECAA = 9999;
                                        fondoPT.XFSDECECMM = 99;
                                        fondoPT.XFSDECECGG = 99;
                                    }
                                    else
                                    {
                                        fondoPT.XFSDECECAA = datiFondoPT.ScadenzaBenefici.HasValue ? (short)datiFondoPT.ScadenzaBenefici.Value.Year : (short)0;
                                        fondoPT.XFSDECECMM = datiFondoPT.ScadenzaBenefici.HasValue ? (short)datiFondoPT.ScadenzaBenefici.Value.Month : (short)0;
                                        fondoPT.XFSDECECGG = datiFondoPT.ScadenzaBenefici.HasValue ? (short)1 : (short)0;
                                    }
                                }
                                //dati legge 460 presente
                                else
                                {
                                    fondoPT.XFSDECECAA = fondoPT.XFSDECAA;//AA decorrenzaRegistrazione
                                    fondoPT.XFSDECECMM = fondoPT.XFSDECMM;//MM decorrenzaRegistrazione
                                    fondoPT.XFSDECECGG = fondoPT.XFSDECGG;//GG decorrenzaRegistrazione
                                    fondoPT.XFSMESIRIS = datiFondoPT.NMesiRiscattati.HasValue ? datiFondoPT.NMesiRiscattati.Value : 0;
                                    fondoPT.XFSMESITOT = datiFondoPT.NMesiTotali.HasValue ? datiFondoPT.NMesiTotali.Value : 0;
                                    fondoPT.XFSPENS = long.Parse((datiFondoPT.SiglaCategoria.HasValue ? datiFondoPT.SiglaCategoria.Value.ToString().PadLeft(3, '0') : "000") +
                                        (datiFondoPT.CodiceSede.HasValue ? datiFondoPT.CodiceSede.ToString().PadLeft(4, '0') : "0000") +
                                        (datiFondoPT.Ncertificato.HasValue ? datiFondoPT.Ncertificato.Value.ToString().PadLeft(8, '0') : "00000000"));
                                }

                                fondoPT.XFSPAL335 = datiFondoPT.PALConBenefici.HasValue ? datiFondoPT.PALConBenefici.Value : 0M;
                            }

                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            {
                                if (datiFondoPT.PensioneAnnuaLorda214.HasValue && datiFondoPT.PensioneAnnuaLorda707.HasValue)
                                {
                                    if (Decimal.Compare(datiFondoPT.PensioneAnnuaLorda214.Value, datiFondoPT.PensioneAnnuaLorda707.Value) <= 0)
                                        fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda214.Value;
                                    else
                                        fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda707.Value;
                                }
                                else if (datiFondoPT.PensioneAnnuaLorda214.HasValue)
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda214.Value;
                                else if (datiFondoPT.PensioneAnnuaLorda707.HasValue)
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda707.Value;
                                else
                                    fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda.HasValue ? datiFondoPT.PensioneAnnuaLorda.Value : 0;
                            }
                            else
                            {
                                fondoPT.XFSPAL = datiFondoPT.PensioneAnnuaLorda.HasValue ? datiFondoPT.PensioneAnnuaLorda.Value : 0;
                            }

                            fondoPT.XFSSUAN = datiFondoPT.ServizioUtileDirittoAA.HasValue ? datiFondoPT.ServizioUtileDirittoAA.Value : (short)0;
                            fondoPT.XFSIMPC = datiFondoPT.IncrementoContrattuale.HasValue ? datiFondoPT.IncrementoContrattuale.Value : 0;

                            int assac = 0;
                            GetASSAC(datiFondoPT.PrivilegiataSuperinvaliditaIndennita, datiFondoPT.AssegnoIntegrativo, datiFondoPT.IntegrazioneIndennitaAssistenza,
                                datiFondoPT.IndennitaAccompagnamentoAggiuntiva, datiFondoPT.CumuloInfermita, datiFondoPT.Categoria2aInfermita, datiFondoPT.AssegnoCura,
                                datiFondoPT.IndennitaSpecialeAnnua, out assac);
                            fondoPT.XFSASSAC = assac;


                            AreaCalcolo.AreaInputVariabile.ListaFondoPT.Add(fondoPT);
                            AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoPT[i].XFSTIPOR));

                            //if(vecchia gestione)
                            if (!isNuovaGestione)
                            {
                                if (record == 1 && ((datiFondoPT.NMesiRiscattati.HasValue && datiFondoPT.NMesiRiscattati.Value > 0) ||
                                            (datiFondoPT.NMesiTotali.HasValue && datiFondoPT.NMesiTotali.Value > 0) ||
                                            (datiFondoPT.SiglaCategoria.HasValue && datiFondoPT.SiglaCategoria.Value > 0) ||
                                            (datiFondoPT.CodiceSede.HasValue && datiFondoPT.CodiceSede.Value > 0) ||
                                            (datiFondoPT.Ncertificato.HasValue && datiFondoPT.Ncertificato.Value > 0) ||
                                            (datiFondoPT.DecorrenzaSecondaria.HasValue)))
                                {
                                    is460Presente = true;
                                    i--;
                                }
                                else
                                    is460Presente = false;
                            }
                            //end if
                        }
                    }
                }
            }
        }

        public static void ValorizzaFondoINPDAP(GestionePensione.DatiPensione datiPensione, Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo,
            out List<GestioneDatiServizioUtileINPDAP.ServizioUtile> datiServizioUtileINPDAPByIdPensione, ref Data.FSPL_FSRC AreaCalcolo)
        {
            datiServizioUtileINPDAPByIdPensione = null;
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                long idPensione = datiPensione.Id;
                List<GestionePensioneINPDAP.DatiPensioneINPDAP> lstDatiFondoINPDAP = objectFondoXX as List<GestionePensioneINPDAP.DatiPensioneINPDAP>;
                GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdPensione(idPensione, out datiServizioUtileINPDAPByIdPensione);
                List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(idPensione, out listaRecordDatiFondoINPDAP);

                AreaCalcolo.AreaInputVariabile.ListaFondoGDP = new List<Data.CMSGTRA.Fondo.GDP>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    long idRecordFondo = recordFondo.Id;
                    record++;
                    Data.CMSGTRA.Fondo.GDP fondoGDP = new Data.CMSGTRA.Fondo.GDP();
                    fondoGDP.TIPOR_GDP = "X";
                    fondoGDP.FONDO_GDP = "GDP";
                    fondoGDP.PROGR_GDP = record;
                    fondoGDP.DECPENS_GDP = recordFondo.DecorrenzaValiditaDati.HasValue ? Utility.StringToNullableInt(recordFondo.DecorrenzaValiditaDati.Value.Year.ToString() +
                        recordFondo.DecorrenzaValiditaDati.Value.Month.ToString().PadLeft(2, '0') +
                        recordFondo.DecorrenzaValiditaDati.Value.Day.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;
                    fondoGDP.SOSPENS_GDP = recordFondo.DataSospensione.HasValue ? Utility.StringToNullableInt(recordFondo.DataSospensione.Value.Year.ToString() +
                    recordFondo.DataSospensione.Value.Month.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;
                    fondoGDP.NCALC_GDP = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? "1" : "0";
                    fondoGDP.NATPENS1_GDP = recordFondo.CodiceNatura1.HasValue ? recordFondo.CodiceNatura1.Value.ToString() : "";
                    fondoGDP.NATPENS2_GDP = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoGDP.NATPENS3_GDP = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoGDP.FLINP_GDP = "0";
                    fondoGDP.DATASS_GDP = datiPensione.InizioAssicurazione.HasValue ? Utility.StringToNullableInt(datiPensione.InizioAssicurazione.Value.Year.ToString() +
                        datiPensione.InizioAssicurazione.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.InizioAssicurazione.Value.Day.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;
                    fondoGDP.DATACES_GDP = datiPensione.FineAssicurazione.HasValue ? Utility.StringToNullableInt(datiPensione.FineAssicurazione.Value.Year.ToString() +
                        datiPensione.FineAssicurazione.Value.Month.ToString().PadLeft(2, '0') +
                        datiPensione.FineAssicurazione.Value.Day.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;

                    GestionePensioneINPDAP.DatiPensioneINPDAP datiFondoINPDAP = lstDatiFondoINPDAP.Find(x => x.IdRecordFondo == idRecordFondo);
                    if (datiFondo != null)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                            {
                                fondoGDP.CSPEC_GDP = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                            }
                        }

                        fondoGDP.TPENS_GDP = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                    }

                    //sovrascrivo valore per GDP RIC REV SIN che hanno il 3
                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.GP1AV91B == "3")
                    {
                        fondoGDP.CSPEC_GDP = "X";
                    }

                    if (datiFondoINPDAP != null)
                    {
                        List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                        GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                        if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                        {
                            GestioneDecodifica.DecodificaCausaCessazione causaCess = ListaCausaCess.Find(x => x.Id == (datiFondoINPDAP.CausaCessazione.HasValue ? datiFondoINPDAP.CausaCessazione.Value : 0));
                            if (causaCess != null)
                            {
                                short resShort = 0;
                                short.TryParse(causaCess.TraduzioneSuGP, out resShort);
                                fondoGDP.CAUSA_GDP = resShort;
                            }
                        }
                        fondoGDP.L537_ANNI_UT_GDP = datiFondoINPDAP.RiduzioneL537.HasValue ?
                            (datiFondoINPDAP.RiduzioneL537.Value ?
                            (datiFondoINPDAP.IISAbbattimentoAnni.HasValue ?
                            datiFondoINPDAP.IISAbbattimentoAnni.Value ? (short)3 : (short)1 : (short)1) :
                            (datiFondoINPDAP.IISAbbattimentoAnni.HasValue ?
                            datiFondoINPDAP.IISAbbattimentoAnni.Value ? (short)2 : (short)0 : (short)0)) :
                            (datiFondoINPDAP.IISAbbattimentoAnni.HasValue ?
                            datiFondoINPDAP.IISAbbattimentoAnni.Value ? (short)2 : (short)0 : (short)0);
                        fondoGDP.DIIS_GDP = datiFondoINPDAP.DirittoIndennitaIntegrativaSpeciale.HasValue ? (datiFondoINPDAP.DirittoIndennitaIntegrativaSpeciale.Value ? (short)2 : (short)0) : (short)0;

                        //**Revisione Campi INPDAP**
                        //fondoGDP.ANNI_MAX_GDP = (short)(datiFondoINPDAP.AnniMax.HasValue ? datiFondoINPDAP.AnniMax.Value : 0);

                        if (datiFondoINPDAP.Microqualifica.HasValue)
                        {
                            GestioneDecodifica.DecMicroqualificaINPDAP microqualifica = null;
                            GestioneDecodifica.GetMicroqualificaById(datiFondoINPDAP.Microqualifica.Value, out microqualifica);
                            if (microqualifica != null)
                                fondoGDP.PROF_GDP = microqualifica.TraduzioneSuGP;
                        }

                    }

                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP = listaRecordDatiFondoINPDAP.Find(x => x.IdRecordFondo == idRecordFondo);
                    if (datiRecordFondoINPDAP != null)
                    {
                        fondoGDP.IIS_CONG_DIR_MIN_GDP = short.Parse((datiRecordFondoINPDAP.IndennitaIntegrativaSpecialeConglobata.HasValue ?
                            datiRecordFondoINPDAP.IndennitaIntegrativaSpecialeConglobata.Value ? "1" : "0" : "0") +
                            (datiRecordFondoINPDAP.IntegrazioneMinimo.HasValue ?
                            datiRecordFondoINPDAP.IntegrazioneMinimo.Value ? "1" : "0" : "0"));
                        fondoGDP.PAL_GDP_EURO = datiRecordFondoINPDAP.PensioneAnnuaLorda.HasValue ? datiRecordFondoINPDAP.PensioneAnnuaLorda.Value : 0;
                        fondoGDP.SUAN_GDP = datiRecordFondoINPDAP.ServizioUtileDirittoAA.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoAA.Value : (short)0;
                        fondoGDP.SUAN_MM_GDP = datiRecordFondoINPDAP.ServizioUtileDirittoMM.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoMM.Value : (short)0;
                        fondoGDP.SUAN_GG_GDP = datiRecordFondoINPDAP.ServizioUtileDirittoGG.HasValue ? datiRecordFondoINPDAP.ServizioUtileDirittoGG.Value : (short)0;
                        fondoGDP.NO336_GDP_EURO = datiRecordFondoINPDAP.RMSSenzaLegge33670QA.HasValue ? datiRecordFondoINPDAP.RMSSenzaLegge33670QA.Value : 0M;
                        fondoGDP.DECEC_GDP = datiRecordFondoINPDAP.ScadenzaBenefici.HasValue ? Utility.StringToNullableInt(datiRecordFondoINPDAP.ScadenzaBenefici.Value.Year.ToString() +
                            datiRecordFondoINPDAP.ScadenzaBenefici.Value.Month.ToString().PadLeft(2, '0') + datiRecordFondoINPDAP.ScadenzaBenefici.Value.Day.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;
                        fondoGDP.PAL_A2C12L33595_GDP = datiRecordFondoINPDAP.PALConBenefici.HasValue ? datiRecordFondoINPDAP.PALConBenefici.Value : 0M;
                        fondoGDP.DECCALC_GDP = datiRecordFondoINPDAP.DecorrenzaCalcolo.HasValue ? Utility.StringToNullableInt(datiRecordFondoINPDAP.DecorrenzaCalcolo.Value.Year.ToString() +
                            datiRecordFondoINPDAP.DecorrenzaCalcolo.Value.Month.ToString().PadLeft(2, '0') +
                            datiRecordFondoINPDAP.DecorrenzaCalcolo.Value.Day.ToString().PadLeft(2, '0')).GetValueOrDefault() : 0;
                        fondoGDP.F13ME_GDP = datiRecordFondoINPDAP.TrediciMensilita.HasValue ? datiRecordFondoINPDAP.TrediciMensilita.Value ? (short)1 : (short)0 : (short)0;
                        fondoGDP.DIVISORE_GDP = datiRecordFondoINPDAP.Divisore.HasValue ? datiRecordFondoINPDAP.Divisore.Value : (short)1;
                        fondoGDP.CAPITOLO_GDP = string.IsNullOrEmpty(datiRecordFondoINPDAP.Capitolo) ? "000" : datiRecordFondoINPDAP.Capitolo;

                        int assac = 0;
                        GetASSAC(datiRecordFondoINPDAP.PrivilegiataSuperinvaliditaIndennita, datiRecordFondoINPDAP.AssegnoIntegrativo, datiRecordFondoINPDAP.IntegrazioneIndennitaAssistenza,
                            datiRecordFondoINPDAP.IndennitaAccompagnamentoAggiuntiva, datiRecordFondoINPDAP.CumuloInfermita, datiRecordFondoINPDAP.Categoria2aInfermita, datiRecordFondoINPDAP.AssegnoCura,
                            datiRecordFondoINPDAP.IndennitaSpecialeAnnua, out assac);
                        fondoGDP.ASSAC_GDP = assac;
                        //sovrascrivo valorizzazione precedente
                        //rimosso con revisione 4.8
                        //fondoGDP.NATPENS1_GDP = recordFondo.CodiceNatura1.HasValue ? recordFondo.CodiceNatura1.Value.ToString() : (datiRecordFondoINPDAP != null && datiRecordFondoINPDAP.TitolareAltraPensione.HasValue ? (datiRecordFondoINPDAP.TitolareAltraPensione.Value ? "6" : "") : "");

                        // TODO: Implementare il mapping con i campi corretti
                        //if (!datiRecordFondoINPDAP.IsLegge460Null())
                        //{
                        //    if (recordFondo.DecorrenzaValiditaDati.HasValue)
                        //    {
                        //        fondoGDP.XFSDECECAA = recordFondo.DecorrenzaValiditaDati.Value.Year;//AA decorrenzaRegistrazione
                        //        fondoGDP.XFSDECECMM = recordFondo.DecorrenzaValiditaDati.Value.Month;//MM decorrenzaRegistrazione
                        //        fondoGDP.XFSDECECGG = recordFondo.DecorrenzaValiditaDati.Value.Day;//GG decorrenzaRegistrazione
                        //    }
                        //    fondoGDP.XFSMESIRIS = datiRecordFondoINPDAP.NMesiRiscattati.HasValue ? datiRecordFondoINPDAP.NMesiRiscattati.Value : 0;
                        //    fondoGDP.XFSMESITOT = datiRecordFondoINPDAP.NMesiTotali.HasValue ? datiRecordFondoINPDAP.NMesiTotali.Value : 0;
                        //    fondoGDP.XFSPENS = long.Parse((datiRecordFondoINPDAP.SiglaCategoria.HasValue ? datiRecordFondoINPDAP.SiglaCategoria.Value.ToString().PadLeft(3, '0') : "000") +
                        //        (datiRecordFondoINPDAP.CodiceSede.HasValue ? datiRecordFondoINPDAP.CodiceSede.ToString().PadLeft(4, '0') : "0000") +
                        //        (datiRecordFondoINPDAP.Ncertificato.HasValue ? datiRecordFondoINPDAP.Ncertificato.Value.ToString().PadLeft(8, '0') : "00000000"));
                        //}
                    }

                    List<GestioneDatiServizioUtileINPDAP.ServizioUtile> datiServizioUtile = datiServizioUtileINPDAPByIdPensione.FindAll(x => x.IdRecordFondo == idRecordFondo);
                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile sU in datiServizioUtile)
                        {
                            if (!string.IsNullOrEmpty(sU.Quota))
                            {
                                switch (sU.Quota.Trim().ToUpperInvariant())
                                {
                                    case "A":
                                        fondoGDP.QA14_GDP_EURO = sU.QuoteArt14.HasValue ? sU.QuoteArt14.Value : 0M;
                                        fondoGDP.RETR_GDP_EURO = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                        fondoGDP.SU92_GDP = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                            (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                            (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                        fondoGDP.IIS_GDP_EURO = sU.ImportoIndennitaIntegrativaSpeciale.HasValue ? sU.ImportoIndennitaIntegrativaSpeciale.Value : 0M;
                                        break;
                                    case "B1":
                                        fondoGDP.RETRM_GDP_EURO = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                        fondoGDP.SU94_GDP = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                            (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                            (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                        break;
                                    case "B2":
                                        fondoGDP.SU95_GDP = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                            (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                            (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                        break;
                                    case "B3":
                                        fondoGDP.SU97_GDP = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                            (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                            (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                        break;
                                    case "B4":
                                        if (sU.ServizioUtileCessazioneAA.HasValue || sU.ServizioUtileCessazioneMM.HasValue || sU.ServizioUtileCessazioneGG.HasValue)
                                        {
                                            fondoGDP.SUCE_GDP = ((((sU.ServizioUtileCessazioneAA.HasValue ? sU.ServizioUtileCessazioneAA.Value * 12 : 0) +
                                                (sU.ServizioUtileCessazioneMM.HasValue ? sU.ServizioUtileCessazioneMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileCessazioneGG.HasValue ? sU.ServizioUtileCessazioneGG.Value : 0));
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoGDP.Add(fondoGDP);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoGDP[0].TIPOR_GDP));
                }
            }
        }

        public static void ValorizzaFondoFS(GestionePensione.DatiPensione datiPensione, Object objectFondoXX,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneFondo.DatiFondo datiFondo, bool isNuovaGestione, out List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtileByIdPensione,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            datiServizioUtileByIdPensione = null;
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                long idPensione = datiPensione.Id;
                List<GestioneFondo.DatiFondoFST> lstDatiFondoFST = objectFondoXX as List<GestioneFondo.DatiFondoFST>;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(idPensione, out datiServizioUtileByIdPensione);

                List<GestioneDecodifica.CodiceSpecifico> listaDecodificaCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out listaDecodificaCodiceSpecifico);
                bool isRicostituzioneIndiretta = false;
                if (listaDecodificaCodiceSpecifico != null && listaDecodificaCodiceSpecifico.Count > 0)
                {
                    if (datiFondo != null)
                    {
                        GestioneDecodifica.CodiceSpecifico codiceSpecifico = listaDecodificaCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico.GetValueOrDefault());
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S") && codiceSpecifico != null && codiceSpecifico.TraduzioneGp.GetValueOrDefault() == 'H')
                            isRicostituzioneIndiretta = true;
                    }
                }

                if (AreaCalcolo.UtilizzaNuovoTracciato)
                {
                    AreaCalcolo.AreaInputVariabile.ListaFondoFS_New = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS_New>();
                    short record = 0;
                    foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                    {
                        long idRecordFondo = recordFondo.Id;
                        GestioneFondo.DatiFondoFST datiFondoFST = null;
                        if (isNuovaGestione)
                            datiFondoFST = lstDatiFondoFST.Find(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiFondoFST = lstDatiFondoFST.FirstOrDefault();
                        record++;
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS_New fondoFS = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS_New();
                        fondoFS.XFSTIPOR = "X";
                        fondoFS.XFSFONDO = "FS";
                        fondoFS.XFSPROGR = record;
                        fondoFS.XFSDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                        fondoFS.XFSDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                        fondoFS.XFSDECGG = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Day : (short)0;
                        fondoFS.XFSSCAAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                        fondoFS.XFSSCAMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                        fondoFS.XFSNCALC = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? "1" : "0";
                        fondoFS.XFSNATU1 = recordFondo.CodiceNatura1.HasValue ? recordFondo.CodiceNatura1.Value.ToString() : (datiFondoFST != null && datiFondoFST.TitolareAltraPensione.HasValue ? (datiFondoFST.TitolareAltraPensione.Value ? "6" : "") : "");
                        //ENG - Reversibilita 024 XFSNATU1 valorizzato con il primo byte del codice natura
                        if (Utility.IsDomandaReversibilita(datiPensione) && datiPensione != null && !String.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length >= 1)
                        {
                            fondoFS.XFSNATU1 = datiPensione.NaturaPensione.Substring(0, 1);
                        }
                        fondoFS.XFSNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                        fondoFS.XFSNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                        fondoFS.XFSFLINP = "0";
                        fondoFS.XFSASSAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                        fondoFS.XFSASSMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                        fondoFS.XFSASSGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                        fondoFS.XFSCESAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                        fondoFS.XFSCESMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                        fondoFS.XFSCESGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;


                        if (datiFondo != null)
                        {
                            List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                            if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                            {
                                GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                                if (codiceSpecifico != null)
                                {
                                    fondoFS.XFSCSPEC = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                                }
                            }

                            fondoFS.XFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                            if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                            {
                                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                                GestioneDecodifica.GetAttivitaSvoltaByFondo("FS", null, out elencoAttivitaSvolte);
                                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                                {
                                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                    fondoFS.XFSPROF = attSvolta.TraduzioneSuGp;
                                }
                            }
                        }

                        if (datiFondoFST != null)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                            {
                                GestioneDecodifica.DecodificaCausaCessazione causaCess = ListaCausaCess.Find(x => x.Id == (datiFondoFST.CausaCessazione.HasValue ? datiFondoFST.CausaCessazione.Value : 0));
                                if (causaCess != null)
                                {
                                    short resShort = 0;
                                    short.TryParse(causaCess.TraduzioneSuGP, out resShort);
                                    fondoFS.XFSCAUSA = resShort;
                                }
                            }
                            fondoFS.XFSCONG = (datiFondoFST.IndennitaIntegrativaSpecialeConglobata.HasValue ?
                                datiFondoFST.IndennitaIntegrativaSpecialeConglobata.Value ? "1" : "0" : "0") +
                                (datiFondoFST.IntegrazioneMinimo.HasValue ?
                                datiFondoFST.IntegrazioneMinimo.Value ? "1" : "0" : "0");
                            fondoFS.XFSRID = datiFondoFST.RiduzioneL537.HasValue ?
                                (datiFondoFST.RiduzioneL537.Value ?
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "3" : "1" : "1") :
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "2" : "0" : "0")) :
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "2" : "0" : "0");
                            fondoFS.XFSDECECAA = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Year : (short)0;
                            fondoFS.XFSDECECMM = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Month : (short)0;
                            fondoFS.XFSDECECGG = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Day : (short)0;

                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            {
                                if (datiFondoFST.PensioneAnnuaLorda214.HasValue && datiFondoFST.PensioneAnnuaLorda707.HasValue)
                                {
                                    if (Decimal.Compare(datiFondoFST.PensioneAnnuaLorda214.Value, datiFondoFST.PensioneAnnuaLorda707.Value) <= 0)
                                        fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda214.Value;
                                    else
                                        fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda707.Value;
                                }
                                else if (datiFondoFST.PensioneAnnuaLorda214.HasValue)
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda214.Value;
                                else if (datiFondoFST.PensioneAnnuaLorda707.HasValue)
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda707.Value;
                                else
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda.HasValue ? datiFondoFST.PensioneAnnuaLorda.Value : 0;
                            }
                            else
                            {
                                fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda.HasValue ? datiFondoFST.PensioneAnnuaLorda.Value : 0;
                            }

                            fondoFS.XFSSUAN = datiFondoFST.ServizioUtileDirittoAA.HasValue ? datiFondoFST.ServizioUtileDirittoAA.Value : (short)0;
                            fondoFS.XFSSUANMM = datiFondoFST.ServizioUtileDirittoMM.HasValue ? datiFondoFST.ServizioUtileDirittoMM.Value : (short)0;
                            fondoFS.XFSSUANGG = datiFondoFST.ServizioUtileDirittoGG.HasValue ? datiFondoFST.ServizioUtileDirittoGG.Value : (short)0;
                            fondoFS.XFSDIIS = datiFondoFST.DirittoIndennitaIntegrativaSpeciale.HasValue ?
                                    (datiFondoFST.DirittoIndennitaIntegrativaSpeciale.Value ?
                                    (datiFondoFST.PagamentoIndennitaIntegrativaSpeciale.HasValue ?
                                    datiFondoFST.PagamentoIndennitaIntegrativaSpeciale.Value ? (short)1 : (short)2 : (short)2) : (short)0) : (short)0;
                            fondoFS.XFSDECAL = datiFondoFST.DecorrenzaCalcolo.HasValue ? int.Parse(datiFondoFST.DecorrenzaCalcolo.Value.Year.ToString().PadLeft(4, '0') +
                                        datiFondoFST.DecorrenzaCalcolo.Value.Month.ToString().PadLeft(2, '0') +
                                        datiFondoFST.DecorrenzaCalcolo.Value.Day.ToString().PadLeft(2, '0')) : 0;
                            fondoFS.XFSF13ME = datiFondoFST.TrediciMensilita.HasValue ? datiFondoFST.TrediciMensilita.Value ? (short)1 : (short)0 : (short)0;
                            fondoFS.XFSSETDIR = datiFondoFST.VVUtiliDiritto.HasValue ? datiFondoFST.VVUtiliDiritto.Value : (short)0;
                            fondoFS.XFSSETMIS = datiFondoFST.VVUtiliMisura.HasValue ? datiFondoFST.VVUtiliMisura.Value : (short)0;

                            if ((Utility.IsDomandaPensioneIndiretta(datiPensione) || isRicostituzioneIndiretta) && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1996, 01, 01)))
                                fondoFS.XFSFAAGO = 1;

                            int assac = 0;
                            GetASSAC(datiFondoFST.PrivilegiataSuperinvaliditaIndennita, datiFondoFST.AssegnoIntegrativo, datiFondoFST.IntegrazioneIndennitaAssistenza,
                                datiFondoFST.IndennitaAccompagnamentoAggiuntiva, datiFondoFST.CumuloInfermita, datiFondoFST.Categoria2aInfermita, datiFondoFST.AssegnoCura,
                                datiFondoFST.IndennitaSpecialeAnnua, out assac);
                            fondoFS.XFSASSAC = assac;
                        }

                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = datiServizioUtileByIdPensione.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = datiServizioUtileByIdPensione;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            fondoFS.XFSQA14 = sU.QuoteArt14.HasValue ? sU.QuoteArt14.Value : 0M;
                                            fondoFS.XFSRETR = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoFS.XFSSU92 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            fondoFS.XFSIIS = sU.ImportoIndennitaIntegrativaSpeciale.HasValue ? sU.ImportoIndennitaIntegrativaSpeciale.Value : 0M;
                                            break;
                                        case "B1":
                                            fondoFS.XFSRETRM = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoFS.XFSSU94 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B2":
                                            fondoFS.XFSSU95 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B3":
                                            fondoFS.XFSSU97 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B4":
                                            if (sU.ServizioUtileCessazioneAA.HasValue || sU.ServizioUtileCessazioneMM.HasValue || sU.ServizioUtileCessazioneGG.HasValue)
                                            {
                                                fondoFS.XFSSUCE = ((((sU.ServizioUtileCessazioneAA.HasValue ? sU.ServizioUtileCessazioneAA.Value * 12 : 0) +
                                                    (sU.ServizioUtileCessazioneMM.HasValue ? sU.ServizioUtileCessazioneMM.Value : 0)) * 30) +
                                                    (sU.ServizioUtileCessazioneGG.HasValue ? sU.ServizioUtileCessazioneGG.Value : 0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        if (isNuovaGestione)
                        {
                            //per la nuova gestione il campo è stato spostato nella pensione DatiFondoFS
                            if (datiFondoFST != null)
                            {
                                fondoFS.XFSNO336 = datiFondoFST.RMSSenzaLegge33670QA.HasValue ? datiFondoFST.RMSSenzaLegge33670QA.Value : 0M;
                                if (datiFondoFST.ScadenzaIllimitata.HasValue && datiFondoFST.ScadenzaIllimitata.Value)
                                {
                                    fondoFS.XFSDECECAA = 9999;
                                    fondoFS.XFSDECECMM = 99;
                                    fondoFS.XFSDECECGG = 99;
                                }
                                else
                                {
                                    fondoFS.XFSDECECAA = datiFondoFST.ScadenzaBenefici.HasValue ? (short)datiFondoFST.ScadenzaBenefici.Value.Year : (short)0;
                                    fondoFS.XFSDECECMM = datiFondoFST.ScadenzaBenefici.HasValue ? (short)datiFondoFST.ScadenzaBenefici.Value.Month : (short)0;
                                    fondoFS.XFSDECECGG = datiFondoFST.ScadenzaBenefici.HasValue ? (short)1 : (short)0;
                                }
                                fondoFS.XFSPAL335 = datiFondoFST.PALConBenefici.HasValue ? datiFondoFST.PALConBenefici.Value : 0M;
                            }
                        }
                        else
                        {
                            if (datiMaggiorazioniBenefici != null)
                                fondoFS.XFSNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        }

                        AreaCalcolo.AreaInputVariabile.ListaFondoFS_New.Add(fondoFS);
                        AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoFS_New[0].XFSTIPOR));
                    }
                }
                else
                {
                    AreaCalcolo.AreaInputVariabile.ListaFondoFS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS>();
                    short record = 0;
                    foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                    {
                        long idRecordFondo = recordFondo.Id;
                        GestioneFondo.DatiFondoFST datiFondoFST = null;
                        if (isNuovaGestione)
                            datiFondoFST = lstDatiFondoFST.Find(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiFondoFST = lstDatiFondoFST.FirstOrDefault();
                        record++;
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS fondoFS = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.FS();
                        fondoFS.XFSTIPOR = "X";
                        fondoFS.XFSFONDO = "FS";
                        fondoFS.XFSPROGR = record;
                        fondoFS.XFSDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                        fondoFS.XFSDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                        fondoFS.XFSDECGG = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Day : (short)0;
                        fondoFS.XFSSCAAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                        fondoFS.XFSSCAMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                        fondoFS.XFSNCALC = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? "1" : "0";
                        fondoFS.XFSNATU1 = recordFondo.CodiceNatura1.HasValue ? recordFondo.CodiceNatura1.Value.ToString() : (datiFondoFST != null && datiFondoFST.TitolareAltraPensione.HasValue ? (datiFondoFST.TitolareAltraPensione.Value ? "6" : "") : "");
                        //ENG - Reversibilita 024 XFSNATU1 valorizzato con il primo byte del codice natura
                        if (Utility.IsDomandaReversibilita(datiPensione) && datiPensione != null && !String.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length >= 1)
                        {
                            fondoFS.XFSNATU1 = datiPensione.NaturaPensione.Substring(0, 1);
                        }
                        fondoFS.XFSNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                        fondoFS.XFSNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                        fondoFS.XFSFLINP = "0";
                        fondoFS.XFSASSAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                        fondoFS.XFSASSMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                        fondoFS.XFSASSGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                        fondoFS.XFSCESAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                        fondoFS.XFSCESMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                        fondoFS.XFSCESGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                        if (datiFondo != null)
                        {
                            List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                            if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                            {
                                GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                                if (codiceSpecifico != null)
                                {
                                    fondoFS.XFSCSPEC = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                                }
                            }

                            fondoFS.XFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                            if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                            {
                                List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                                GestioneDecodifica.GetAttivitaSvoltaByFondo("FS", null, out elencoAttivitaSvolte);
                                if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                                {
                                    GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                    fondoFS.XFSPROF = attSvolta.TraduzioneSuGp;
                                }
                            }
                        }

                        if (datiFondoFST != null)
                        {
                            List<GestioneDecodifica.DecodificaCausaCessazione> ListaCausaCess = null;
                            GestioneDecodifica.GetElencoCodiciCausaCessazione(out ListaCausaCess);
                            if (ListaCausaCess != null && ListaCausaCess.Count > 0)
                            {
                                GestioneDecodifica.DecodificaCausaCessazione causaCess = ListaCausaCess.Find(x => x.Id == (datiFondoFST.CausaCessazione.HasValue ? datiFondoFST.CausaCessazione.Value : 0));
                                if (causaCess != null)
                                {
                                    short resShort = 0;
                                    short.TryParse(causaCess.TraduzioneSuGP, out resShort);
                                    fondoFS.XFSCAUSA = resShort;
                                }
                            }
                            fondoFS.XFSCONG = (datiFondoFST.IndennitaIntegrativaSpecialeConglobata.HasValue ?
                                datiFondoFST.IndennitaIntegrativaSpecialeConglobata.Value ? "1" : "0" : "0") +
                                (datiFondoFST.IntegrazioneMinimo.HasValue ?
                                datiFondoFST.IntegrazioneMinimo.Value ? "1" : "0" : "0");
                            fondoFS.XFSRID = datiFondoFST.RiduzioneL537.HasValue ?
                                (datiFondoFST.RiduzioneL537.Value ?
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "3" : "1" : "1") :
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "2" : "0" : "0")) :
                                (datiFondoFST.IISAbbattimentoAnni.HasValue ?
                                datiFondoFST.IISAbbattimentoAnni.Value ? "2" : "0" : "0");
                            fondoFS.XFSDECECAA = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Year : (short)0;
                            fondoFS.XFSDECECMM = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Month : (short)0;
                            fondoFS.XFSDECECGG = datiFondoFST.DecorrenzaEconomica.HasValue ? (short)datiFondoFST.DecorrenzaEconomica.Value.Day : (short)0;

                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            {
                                if (datiFondoFST.PensioneAnnuaLorda214.HasValue && datiFondoFST.PensioneAnnuaLorda707.HasValue)
                                {
                                    if (Decimal.Compare(datiFondoFST.PensioneAnnuaLorda214.Value, datiFondoFST.PensioneAnnuaLorda707.Value) <= 0)
                                        fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda214.Value;
                                    else
                                        fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda707.Value;
                                }
                                else if (datiFondoFST.PensioneAnnuaLorda214.HasValue)
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda214.Value;
                                else if (datiFondoFST.PensioneAnnuaLorda707.HasValue)
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda707.Value;
                                else
                                    fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda.HasValue ? datiFondoFST.PensioneAnnuaLorda.Value : 0;
                            }
                            else
                            {
                                fondoFS.XFSPAL = datiFondoFST.PensioneAnnuaLorda.HasValue ? datiFondoFST.PensioneAnnuaLorda.Value : 0;
                            }
                            fondoFS.XFSSUAN = datiFondoFST.ServizioUtileDirittoAA.HasValue ? datiFondoFST.ServizioUtileDirittoAA.Value : (short)0;

                            int assac = 0;
                            GetASSAC(datiFondoFST.PrivilegiataSuperinvaliditaIndennita, datiFondoFST.AssegnoIntegrativo, datiFondoFST.IntegrazioneIndennitaAssistenza,
                                datiFondoFST.IndennitaAccompagnamentoAggiuntiva, datiFondoFST.CumuloInfermita, datiFondoFST.Categoria2aInfermita, datiFondoFST.AssegnoCura,
                                datiFondoFST.IndennitaSpecialeAnnua, out assac);
                            fondoFS.XFSASSAC = assac;
                        }

                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = datiServizioUtileByIdPensione.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = datiServizioUtileByIdPensione;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            fondoFS.XFSQA14 = sU.QuoteArt14.HasValue ? sU.QuoteArt14.Value : 0M;
                                            fondoFS.XFSRETR = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoFS.XFSSU92 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            fondoFS.XFSIIS = sU.ImportoIndennitaIntegrativaSpeciale.HasValue ? sU.ImportoIndennitaIntegrativaSpeciale.Value : 0M;
                                            break;
                                        case "B1":
                                            fondoFS.XFSRETRM = sU.Retribuzione.HasValue ? sU.Retribuzione.Value : 0M;
                                            fondoFS.XFSSU94 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B2":
                                            fondoFS.XFSSU95 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B3":
                                            fondoFS.XFSSU97 = ((((sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value * 12 : 0) +
                                                (sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : 0)) * 30) +
                                                (sU.ServizioUtileGG.HasValue ? sU.ServizioUtileGG.Value : 0));
                                            break;
                                        case "B4":
                                            if (sU.ServizioUtileCessazioneAA.HasValue || sU.ServizioUtileCessazioneMM.HasValue || sU.ServizioUtileCessazioneGG.HasValue)
                                            {
                                                fondoFS.XFSSUCE = ((((sU.ServizioUtileCessazioneAA.HasValue ? sU.ServizioUtileCessazioneAA.Value * 12 : 0) +
                                                    (sU.ServizioUtileCessazioneMM.HasValue ? sU.ServizioUtileCessazioneMM.Value : 0)) * 30) +
                                                    (sU.ServizioUtileCessazioneGG.HasValue ? sU.ServizioUtileCessazioneGG.Value : 0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        fondoFS.XFSDIIS = datiFondoFST.DirittoIndennitaIntegrativaSpeciale.HasValue ?
                                    (datiFondoFST.DirittoIndennitaIntegrativaSpeciale.Value ?
                                    (datiFondoFST.PagamentoIndennitaIntegrativaSpeciale.HasValue ?
                                    datiFondoFST.PagamentoIndennitaIntegrativaSpeciale.Value ? (short)1 : (short)2 : (short)2) : (short)0) : (short)0;

                        fondoFS.XFSDECAL = datiFondoFST.DecorrenzaCalcolo.HasValue ? int.Parse(datiFondoFST.DecorrenzaCalcolo.Value.Year.ToString().PadLeft(4, '0') +
                                    datiFondoFST.DecorrenzaCalcolo.Value.Month.ToString().PadLeft(2, '0') +
                                    datiFondoFST.DecorrenzaCalcolo.Value.Day.ToString().PadLeft(2, '0')) : 0;

                        fondoFS.XFSF13ME = datiFondoFST.TrediciMensilita.HasValue ? datiFondoFST.TrediciMensilita.Value ? (short)1 : (short)0 : (short)0;

                        if ((Utility.IsDomandaPensioneIndiretta(datiPensione) || isRicostituzioneIndiretta) && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1996, 01, 01)))
                            fondoFS.XFSFAAGO = 1;

                        if (isNuovaGestione)
                        {
                            //per la nuova gestione il campo è stato spostato nella pensione DatiFondoFS
                            if (datiFondoFST != null)
                            {
                                fondoFS.XFSNO336 = datiFondoFST.RMSSenzaLegge33670QA.HasValue ? datiFondoFST.RMSSenzaLegge33670QA.Value : 0M;
                                if (datiFondoFST.ScadenzaIllimitata.HasValue && datiFondoFST.ScadenzaIllimitata.Value)
                                {
                                    fondoFS.XFSDECECAA = 9999;
                                    fondoFS.XFSDECECMM = 99;
                                    fondoFS.XFSDECECGG = 99;
                                }
                                else
                                {
                                    fondoFS.XFSDECECAA = datiFondoFST.ScadenzaBenefici.HasValue ? (short)datiFondoFST.ScadenzaBenefici.Value.Year : (short)0;
                                    fondoFS.XFSDECECMM = datiFondoFST.ScadenzaBenefici.HasValue ? (short)datiFondoFST.ScadenzaBenefici.Value.Month : (short)0;
                                    fondoFS.XFSDECECGG = datiFondoFST.ScadenzaBenefici.HasValue ? (short)1 : (short)0;
                                }
                                fondoFS.XFSPAL335 = datiFondoFST.PALConBenefici.HasValue ? datiFondoFST.PALConBenefici.Value : 0M;
                            }
                        }
                        else
                        {
                            if (datiMaggiorazioniBenefici != null)
                                fondoFS.XFSNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        }

                        AreaCalcolo.AreaInputVariabile.ListaFondoFS.Add(fondoFS);
                        AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoFS[0].XFSTIPOR));
                    }
                }
            }
        }

        public static void ValorizzaFondoPI(List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoPI = new List<Data.CMSGTRA.Fondo.PI>();

                Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

                List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listaDatiServizioUtile);

                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    Data.CMSGTRA.Fondo.PI fondoPI = new Data.CMSGTRA.Fondo.PI();
                    fondoPI.XPITIPOR = "X";
                    fondoPI.XPIFONDO = "PI";
                    fondoPI.XPIPROGR = record;
                    fondoPI.XPIDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoPI.XPIDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoPI.XPIDECGG = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Day : (short)0;
                    fondoPI.XPISCAAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoPI.XPISCAMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoPI.XPINCALC = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    fondoPI.XPINATU1 = recordFondo.CodiceNatura1.HasValue ? recordFondo.CodiceNatura1.Value.ToString() : "";

                    short resShort = 0;
                    if (recordFondo.CodiceNatura2.HasValue)
                    {
                        short.TryParse(recordFondo.CodiceNatura2.Value.ToString(), out resShort);
                        fondoPI.XPINATU2 = resShort;
                    }
                    fondoPI.XPINATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";

                    fondoPI.XPIASSAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoPI.XPIASSMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoPI.XPIASSGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoPI.XPICESAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoPI.XPICESMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoPI.XPICESGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                            {
                                fondoPI.XPISPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                            }
                        }

                        fondoPI.XPITPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            char? enteFondo = Utility.GetCharCategoriaFondoPI(categoriaFondoPI);

                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("PI", enteFondo, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                resShort = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out resShort);
                                fondoPI.XPIATTIV = resShort;
                            }
                        }
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoPI != null)
                    {
                        GestioneFondo.DatiFondoPI datiFondoPI = objectFondoXX as GestioneFondo.DatiFondoPI;
                        try
                        {
                            if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                            {
                                string livello = datiFondoPI.Livello.HasValue ? datiFondoPI.Livello.ToString().PadLeft(3, '0') : "000"; // 3 caratteri
                                string settimaneMaggiorazione = datiFondoPI.SettimaneMaggiorazione.HasValue ? datiFondoPI.SettimaneMaggiorazione.ToString().PadLeft(4, '0').Substring(0, 3) : "000"; // 3 caratteri
                                string settimaneMaggiorazioneDecimal = datiFondoPI.SettimaneMaggiorazione.HasValue ? datiFondoPI.SettimaneMaggiorazione.ToString().PadLeft(4, '0').Substring(3, 1) : "0"; // 1 carattere
                                string settimaneEsclusive = datiFondoPI.SettimaneEsclusive.HasValue ? datiFondoPI.SettimaneEsclusive.ToString().PadLeft(4, '0').Substring(0, 3) : "000"; // 3 caratteri
                                string settimaneEsclusiveDP346 = datiFondoPI.SettimaneEsclusive.HasValue ? datiFondoPI.SettimaneEsclusive.ToString().PadLeft(4, '0').Substring(3, 1) : "0"; // 1 carattere
                                string settimaneInpdai = datiFondoPI.SettimaneINPDAI.HasValue ? datiFondoPI.SettimaneINPDAI.ToString().PadLeft(4, '0') : "0000"; // 4 caratteri

                                // 3 byte Livello
                                // 4 byte SettimaneMaggiorazione
                                // 3 byte SettimaneEsclusive
                                fondoPI.XPIONLEG = decimal.Parse(livello + settimaneMaggiorazione + "," + settimaneMaggiorazioneDecimal + settimaneEsclusive);
                                // 1 byte SettimaneEsclusive
                                // 4 byte SettimaneINPDAI
                                // 5 byte 0
                                fondoPI.XPIDP346 = decimal.Parse(settimaneEsclusiveDP346 + settimaneInpdai + "0");
                            }
                            else
                                fondoPI.XPIONLEG = datiFondoPI != null && !string.IsNullOrEmpty(datiFondoPI.NumeroMatricola) ?
                                    decimal.Parse(datiFondoPI.NumeroMatricola.PadLeft(8, '0').Substring(0, 6) + "," + datiFondoPI.NumeroMatricola.PadLeft(8, '0').Substring(6, 2)) : 0;
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }

                        fondoPI.XPIQUALI = datiFondoPI.Qualifica;
                        if (categoriaFondoPI.HasValue && categoriaFondoPI.Value != Utility.CategoriaFondoPI.V)
                            fondoPI.XPIRDISD = datiFondoPI.DecorrenzaPensioneEliminata.HasValue ? (short)datiFondoPI.DecorrenzaPensioneEliminata.Value.Year : (short)0;
                        fondoPI.XPIRDPNI = datiFondoPI.DecorrenzaPensioneEliminata.HasValue ? (short)datiFondoPI.DecorrenzaPensioneEliminata.Value.Month : (short)0;
                        fondoPI.XPIRDISI = datiFondoPI.DecorrenzaPensioneEliminata.HasValue ? (short)datiFondoPI.DecorrenzaPensioneEliminata.Value.Day : (short)0;
                        fondoPI.XPIRISCA = datiFondoPI.RiscattiAA.HasValue ? datiFondoPI.RiscattiAA.Value : (short)0;
                        fondoPI.XPIRISCM = datiFondoPI.RiscattiMM.HasValue ? datiFondoPI.RiscattiMM.Value : (short)0;
                        fondoPI.XPIRISCG = datiFondoPI.RiscattiGG.HasValue ? datiFondoPI.RiscattiGG.Value : (short)0;
                        fondoPI.XPISTIPE = datiFondoPI.StipendioAnnuo.HasValue ? datiFondoPI.StipendioAnnuo.Value : 0M;
                        fondoPI.XPIFACOL = datiFondoPI.PensioneFacoltativaMensile.HasValue ? datiFondoPI.PensioneFacoltativaMensile.Value : 0M;
                        if (categoriaFondoPI.HasValue && categoriaFondoPI.Value != Utility.CategoriaFondoPI.U)
                        {
                            fondoPI.XPICAPDE = datiFondoPI.DecorrenzaPrescrizione.HasValue ? (short)datiFondoPI.DecorrenzaPrescrizione.Value.Year : (short)0;
                            fondoPI.XPICAPIN = datiFondoPI.DecorrenzaPrescrizione.HasValue ? (short)datiFondoPI.DecorrenzaPrescrizione.Value.Month : (short)0;
                        }
                        fondoPI.XPIINTEG = datiFondoPI.ImportoIIS.HasValue ? datiFondoPI.ImportoIIS.Value : 0M;
                        fondoPI.XPINONVE = datiFondoPI.NonVedente.HasValue ? datiFondoPI.NonVedente.Value ? (short)1 : (short)0 : (short)0;
                        fondoPI.XPICAMPA = datiFondoPI.ServizioNonUtileAA.HasValue ? datiFondoPI.ServizioNonUtileAA.Value : (short)0;
                        fondoPI.XPICAMPM = datiFondoPI.ServizioNonUtileMM.HasValue ? datiFondoPI.ServizioNonUtileMM.Value : (short)0;
                        fondoPI.XPICAMPG = datiFondoPI.ServizioNonUtileGG.HasValue ? datiFondoPI.ServizioNonUtileGG.Value : (short)0;

                        fondoPI.XPI36BIS = datiFondoPI.StipendioBase.HasValue ? datiFondoPI.StipendioBase.Value : 0M;
                        fondoPI.XPIOKIIS = datiFondoPI.AttCon.HasValue ? datiFondoPI.AttCon.Value.ToString() : string.Empty;

                        if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.U)
                        {
                            // cifre decimali
                            fondoPI.XPICAPDE = datiFondoPI.PercentualeCapitalizzazione.HasValue ? (short)(datiFondoPI.PercentualeCapitalizzazione.Value % 1) : (short)0;
                            // cifre intere
                            fondoPI.XPICAPIN = datiFondoPI.PercentualeCapitalizzazione.HasValue ? (short)(datiFondoPI.PercentualeCapitalizzazione.Value / 1) : (short)0;

                            fondoPI.XPIAS762 = datiFondoPI.PensComplRiv1_95.HasValue ? datiFondoPI.PensComplRiv1_95.Value : 0M;
                        }
                        else if (categoriaFondoPI.HasValue && categoriaFondoPI.Value == Utility.CategoriaFondoPI.V)
                        {
                            fondoPI.XPIAS762 = datiFondoPI.RMSQuotaB.HasValue ? datiFondoPI.RMSQuotaB.Value : 0M;
                            fondoPI.XPIRDISD = datiFondoPI.NSettimaneQuotaA.HasValue ? datiFondoPI.NSettimaneQuotaA.Value : (short)0;
                        }

                        fondoPI.XPIMEDIC = datiFondoPI.CodiceMaggiorazione.HasValue ? datiFondoPI.CodiceMaggiorazione.Value.ToString() : string.Empty;
                        fondoPI.XPIINAIL = datiFondoPI.RMSQuotaA.HasValue ? datiFondoPI.RMSQuotaA.Value : 0M;
                        fondoPI.XPIRDPND = datiFondoPI.NSettimaneQuotaB.HasValue ? datiFondoPI.NSettimaneQuotaB.Value : (short)0;
                    }

                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                    {
                        fondoPI.XPISERVA = listaDatiServizioUtile.LastOrDefault() != null ? listaDatiServizioUtile.LastOrDefault().ServizioUtileAA.HasValue ?
                                listaDatiServizioUtile.LastOrDefault().ServizioUtileAA.Value : (short)0 : (short)0;
                        fondoPI.XPISERVM = listaDatiServizioUtile.LastOrDefault() != null ? listaDatiServizioUtile.LastOrDefault().ServizioUtileMM.HasValue ?
                           listaDatiServizioUtile.LastOrDefault().ServizioUtileMM.Value : (short)0 : (short)0;
                        fondoPI.XPISERVG = listaDatiServizioUtile.LastOrDefault() != null ? listaDatiServizioUtile.LastOrDefault().ServizioUtileGG.HasValue ?
                           listaDatiServizioUtile.LastOrDefault().ServizioUtileGG.Value : (short)0 : (short)0;
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        fondoPI.XPIEXCBT = datiMaggiorazioniBenefici.ExCombattente.HasValue ? datiMaggiorazioniBenefici.ExCombattente.Value.ToString() : string.Empty;
                        fondoPI.XPINO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoPI.Add(fondoPI);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoPI[0].XPITIPOR));
                }
            }
        }

        public static void ValorizzaFondoGAS(GestionePensione.DatiPensione datiPensione, Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneFondo.DatiFondo datiFondo, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoGAS = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GAS>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GAS fondoGAS = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.GAS();
                    fondoGAS.XGATIPOR = "X";
                    fondoGAS.XGAFONDO = "GAS";
                    fondoGAS.XGAPROGR = record;
                    fondoGAS.XGADECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoGAS.XGADECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoGAS.XGASOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoGAS.XGASOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoGAS.XGANOCAL = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoGAS.XGANATU1 = codNatura1;
                    }
                    fondoGAS.XGANATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoGAS.XGANATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoGAS.XGAPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoGAS.XGAPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoGAS.XGAPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoGAS.XGAUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoGAS.XGAUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoGAS.XGAUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("GAS", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                short res = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out res);
                                fondoGAS.XGAATTIV = res;
                            }
                        }
                        List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                                fondoGAS.XGASPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                        }
                        fondoGAS.XGATPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoGAS.XGAFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoGAS != null)
                    {
                        GestioneFondo.DatiFondoGAS datiFondoGAS = objectFondoXX as GestioneFondo.DatiFondoGAS;
                        fondoGAS.XGARISCU = datiFondoGAS.MesiUtiliIndennitaAggiuntiva.HasValue ? datiFondoGAS.MesiUtiliIndennitaAggiuntiva.Value : (short)0;
                        fondoGAS.XGARISCN = datiFondoGAS.MesiNonUtiliIndennitaAggiuntiva.HasValue ? datiFondoGAS.MesiNonUtiliIndennitaAggiuntiva.Value : (short)0;
                        fondoGAS.XGAINDMM = datiFondoGAS.ServizioUtileIndennitaAggiuntiva.HasValue ? datiFondoGAS.ServizioUtileIndennitaAggiuntiva.Value : (short)0;
                        fondoGAS.XGAINDRT = datiFondoGAS.Retribuzione.HasValue ? (short)datiFondoGAS.Retribuzione.Value : 0;
                        fondoGAS.XGAPNRID = (datiFondoGAS.CodicePensioneRidotta.HasValue && datiFondoGAS.CodicePensioneRidotta.Value) ? (short)1 : (short)0;
                        fondoGAS.XGACONGU = datiFondoGAS.Conguaglio.HasValue ? datiFondoGAS.Conguaglio.Value : 0M;
                        fondoGAS.XGAANT46 = datiFondoGAS.MesiAnte46.HasValue ? datiFondoGAS.MesiAnte46.Value : (short)0;
                        fondoGAS.XGAPOS46 = datiFondoGAS.AnzianitaUtileDal46.HasValue ? datiFondoGAS.AnzianitaUtileDal46.Value : (short)0;
                        fondoGAS.XGADIMIS = (datiFondoGAS.CodiceDimissioni.HasValue && datiFondoGAS.CodiceDimissioni.Value) ? (short)1 : (short)0;
                        fondoGAS.XGARIDUZ = datiFondoGAS.PercentualeRiduzione.HasValue ? datiFondoGAS.PercentualeRiduzione.Value : (short)0;
                        fondoGAS.XGACONVE = !String.IsNullOrEmpty(datiFondoGAS.Convenzione) ? datiFondoGAS.Convenzione : "";
                        fondoGAS.XGADITTA = !String.IsNullOrEmpty(datiFondoGAS.Ditta) ? datiFondoGAS.Ditta : "";
                    }

                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        fondoGAS.XGAUTIAA = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().ServizioUtileAA.HasValue ? datiServizioUtile.FirstOrDefault().ServizioUtileAA.Value : (short)0;
                        fondoGAS.XGAUTIMM = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().ServizioUtileMM.HasValue ? datiServizioUtile.FirstOrDefault().ServizioUtileMM.Value : (short)0;
                        fondoGAS.XGARETPN = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().RetribuzionePensionabile.HasValue ? datiServizioUtile.FirstOrDefault().RetribuzionePensionabile.Value : 0;
                    }

                    if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        short resShort = 0;
                        short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                        if (resShort < 10)
                            fondoGAS.XGANONVE = resShort;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoGAS.Add(fondoGAS);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoGAS[0].XGATIPOR));
                }
            }
        }

        public static void ValorizzaFondoES(GestionePensione.DatiPensione datiPensione, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo,
            Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoES = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ES>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ES fondoES = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.ES();
                    fondoES.XESTIPOR = "X";
                    fondoES.XESFONDO = "ES";
                    fondoES.XESPROGR = record;
                    fondoES.XESDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoES.XESDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoES.XESSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoES.XESSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoES.XESNOCAL = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoES.XESNATU1 = codNatura1;
                    }
                    fondoES.XESNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoES.XESNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";

                    if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                    {
                        fondoES.XESUTIAA = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().ServizioUtileAA.HasValue ? datiServizioUtile.FirstOrDefault().ServizioUtileAA.Value : (short)0;
                        fondoES.XESUTIMM = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().ServizioUtileMM.HasValue ? datiServizioUtile.FirstOrDefault().ServizioUtileMM.Value : (short)0;
                        fondoES.XESRETPN = datiServizioUtile.FirstOrDefault() != null && datiServizioUtile.FirstOrDefault().RetribuzionePensionabile.HasValue ? datiServizioUtile.FirstOrDefault().RetribuzionePensionabile.Value : 0;
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoES != null)
                    {
                        GestioneFondo.DatiFondoES datiFondoES = objectFondoXX as GestioneFondo.DatiFondoES;
                        if (!datiFondoES.isNull())
                        {
                            fondoES.LISTXESCALCO = new List<Data.CMSGTRA.Fondo.ES.XESCALCO>();
                            if (datiFondoES.Retribuzione.HasValue && datiFondoES.MMServizioUtile.HasValue)
                                fondoES.LISTXESCALCO.Add(new Data.CMSGTRA.Fondo.ES.XESCALCO() { XESCALMM = (short)datiFondoES.MMServizioUtile.Value, XESCALRT = datiFondoES.Retribuzione.Value });
                            if (datiFondoES.Retribuzione2.HasValue && datiFondoES.MMServizioUtile2.HasValue)
                                fondoES.LISTXESCALCO.Add(new Data.CMSGTRA.Fondo.ES.XESCALCO() { XESCALMM = (short)datiFondoES.MMServizioUtile2.Value, XESCALRT = datiFondoES.Retribuzione2.Value });
                            if (datiFondoES.Retribuzione3.HasValue && datiFondoES.MMServizioUtile3.HasValue)
                                fondoES.LISTXESCALCO.Add(new Data.CMSGTRA.Fondo.ES.XESCALCO() { XESCALMM = (short)datiFondoES.MMServizioUtile3.Value, XESCALRT = datiFondoES.Retribuzione3.Value });
                            if (datiFondoES.Retribuzione4.HasValue && datiFondoES.MMServizioUtile4.HasValue)
                                fondoES.LISTXESCALCO.Add(new Data.CMSGTRA.Fondo.ES.XESCALCO() { XESCALMM = (short)datiFondoES.MMServizioUtile4.Value, XESCALRT = datiFondoES.Retribuzione4.Value });
                            fondoES.XESANNUT = (datiFondoES.AnnoUtile.HasValue && datiFondoES.AnnoUtile.Value) ? (short)1 : (short)0;
                            fondoES.XESART58 = (datiFondoES.Articolo58.HasValue) ? (datiFondoES.Articolo58.Value) : (short)0;
                            fondoES.XESART59 = (datiFondoES.Articolo59.HasValue && datiFondoES.Articolo59.Value) ? (short)1 : (short)0;
                            //ATTENZIONE : sul campo CodiciRetributivi verificare  se qaundo viene lasciato spazio è corretto inserire 0
                            fondoES.XESCDRET = (datiFondoES.CodiciRetributivi.HasValue) ? datiFondoES.CodiciRetributivi.Value : (short)0;
                            //ATTENZIONE : sul campo ClassePensioneAnte50 verificare  se qaundo viene lasciato spazio è corretto inserire 0
                            fondoES.XESCLASS = (datiFondoES.ClassePensioneAnte50.HasValue) ? datiFondoES.ClassePensioneAnte50.Value : (short)0;
                            fondoES.XESCODES = (datiFondoES.CodiceEsattoria != null) ? (short.Parse(datiFondoES.CodiceEsattoria)) : (short)0;
                            fondoES.XESCODIC = (datiFondoES.CodiceDz.HasValue && datiFondoES.CodiceDz.Value) ? (short)1 : (short)0;
                            fondoES.XESOPTAN = (datiFondoES.Optanti.HasValue && datiFondoES.Optanti.Value) ? (short)1 : (short)0;
                            fondoES.XESPRIVI = (datiFondoES.MaggiorazionePrivilegiata.HasValue && datiFondoES.MaggiorazionePrivilegiata.Value) ? (short)1 : (short)0;
                            //ATTENZIONE : sul campo Promiscui verificare  se qaundo viene lasciato spazio è corretto inserire 0
                            fondoES.XESPROMI = (datiFondoES.Promiscui.HasValue) ? datiFondoES.Promiscui.Value : (short)0;
                            fondoES.XESSALTU = (datiFondoES.Saltuari.HasValue && datiFondoES.Saltuari.Value) ? (short)1 : (short)0;
                            fondoES.XESCONVE = datiFondoES.ConvenzioneInternazionale.HasValue ? datiFondoES.ConvenzioneInternazionale.Value.ToString() : string.Empty;
                            fondoES.XESRISAA = datiFondoES.AnniRiscatti.HasValue ? (short)datiFondoES.AnniRiscatti.Value : (short)0;
                            fondoES.XESRISMM = datiFondoES.MesiRiscatti.HasValue ? (short)datiFondoES.MesiRiscatti.Value : (short)0;

                            //ANTE 67
                            fondoES.LISTXES57ELE = new List<Data.CMSGTRA.Fondo.ES.XES57ELE>();
                            if (datiFondoES.ContributiLegge37758Art57Periodo1.HasValue && datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo1.HasValue)
                            {
                                Data.CMSGTRA.Fondo.ES.XES57ELE elem = new Data.CMSGTRA.Fondo.ES.XES57ELE();
                                elem.XES57CTR = datiFondoES.ContributiLegge37758Art57Periodo1.Value;
                                elem.XES57DAA = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo1.Value.Year;
                                elem.XES57DMM = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo1.Value.Month;
                                fondoES.LISTXES57ELE.Add(elem);
                            }
                            if (datiFondoES.ContributiLegge37758Art57Periodo2.HasValue && datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo2.HasValue)
                            {
                                Data.CMSGTRA.Fondo.ES.XES57ELE elem = new Data.CMSGTRA.Fondo.ES.XES57ELE();
                                elem.XES57CTR = datiFondoES.ContributiLegge37758Art57Periodo2.Value;
                                elem.XES57DAA = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo2.Value.Year;
                                elem.XES57DMM = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo2.Value.Month;
                                fondoES.LISTXES57ELE.Add(elem);
                            }
                            if (datiFondoES.ContributiLegge37758Art57Periodo3.HasValue && datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo3.HasValue)
                            {
                                Data.CMSGTRA.Fondo.ES.XES57ELE elem = new Data.CMSGTRA.Fondo.ES.XES57ELE();
                                elem.XES57CTR = datiFondoES.ContributiLegge37758Art57Periodo3.Value;
                                elem.XES57DAA = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo3.Value.Year;
                                elem.XES57DMM = (short)datiFondoES.DecorrenzaLegge37758Art57Pre67Periodo3.Value.Month;
                                fondoES.LISTXES57ELE.Add(elem);
                            }
                            fondoES.XES24CTR = (datiFondoES.ContributiLegge37758Art24.HasValue) ? (datiFondoES.ContributiLegge37758Art24.Value) : 0;
                            fondoES.XES24DAA = (datiFondoES.DecorrenzaArticolo24.HasValue) ? (short)(datiFondoES.DecorrenzaArticolo24.Value).Year : (short)0;
                            fondoES.XES24DMM = (datiFondoES.DecorrenzaArticolo24.HasValue) ? (short)(datiFondoES.DecorrenzaArticolo24.Value).Month : (short)0;
                            fondoES.XESCODPN = datiFondoES.CodicePensioneInPagamentoPre67.HasValue ? datiFondoES.CodicePensioneInPagamentoPre67.Value.ToString() : string.Empty;
                            fondoES.XESIMPAG = datiFondoES.ImportoInPagamentoPre67.HasValue ? datiFondoES.ImportoInPagamentoPre67.Value : 0;
                            fondoES.XESPNFON = datiFondoES.PensioneFondoAl67.HasValue ? datiFondoES.PensioneFondoAl67.Value : 0;
                        }
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        if (datiMaggiorazioniBenefici.ExCombattente.HasValue)
                        {
                            List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                            GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                            if (listaCodiciMaggExComb != null && listaCodiciMaggExComb.Count > 0)
                            {
                                GestioneDecodifica.CodiceMaggiorazioneExCombattenti codMaggExComb = listaCodiciMaggExComb.Find(x => x.Id == datiMaggiorazioniBenefici.ExCombattente.Value);
                                if (codMaggExComb != null)
                                    fondoES.XESCOMBA = codMaggExComb.TraduzioneSuGP;
                            }
                        }
                        fondoES.XESNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        if (!string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                        {
                            short resShort = 0;
                            short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                            if (resShort < 10)
                                fondoES.XESNONVE = resShort;
                        }
                    }

                    fondoES.XESPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoES.XESPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoES.XESPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoES.XESUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoES.XESUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoES.XESUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        if (!string.IsNullOrEmpty(datiFondo.AttivitaSvolta))
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("ES", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attivitaSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                if (attivitaSvolta != null)
                                {
                                    short resShort = 0;
                                    short.TryParse(attivitaSvolta.TraduzioneSuGp, out resShort);
                                    fondoES.XESATTIV = resShort;
                                }
                            }
                        }
                        fondoES.XESFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                        if (datiFondo.CodiceSpecifico.HasValue)
                        {
                            List<GestioneDecodifica.CodiceSpecifico> elencoCodiciSpecifici = null;
                            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiciSpecifici);
                            if (elencoCodiciSpecifici != null && elencoCodiciSpecifici.Count > 0)
                            {
                                GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiciSpecifici.Find(x => x.Id == datiFondo.CodiceSpecifico.Value);
                                if (codiceSpecifico != null)
                                    fondoES.XESSPECI = codiceSpecifico.TraduzioneGp.ToString();
                            }
                        }
                        fondoES.XESTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                        //Comma 707
                        fondoES.XESSETA_707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : (short)0;
                        fondoES.XESSETB_707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : (short)0;
                        // Al momento non verrà mappato perchè non abbiamo le specifiche
                        //fondoES.XESCALC707    
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoES.Add(fondoES);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoES[0].XESTIPOR));
                }
            }
        }

        public static void ValorizzaFondoDZ(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoDZ = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.DZ>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.DZ fondoDZ = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.DZ();
                    fondoDZ.XDZTIPOR = "X";
                    fondoDZ.XDZFONDO = "DZ";
                    fondoDZ.XDZPROGR = record;
                    fondoDZ.XDZDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoDZ.XDZDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoDZ.XDZSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoDZ.XDZSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoDZ.XDZNOCAL = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoDZ.XDZNATU1 = codNatura1;
                    }
                    fondoDZ.XDZNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoDZ.XDZNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoDZ.XDZPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoDZ.XDZPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoDZ.XDZPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoDZ.XDZUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoDZ.XDZUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoDZ.XDZUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    if (datiFondo != null)
                    {
                        fondoDZ.XDZTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoDZ.XDZFISSE = datiFondo.CodiceDirittoQuoteFisse.HasValue ? datiFondo.CodiceDirittoQuoteFisse.Value : (short)0;
                        fondoDZ.XDZREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.ToString() : string.Empty;
                        fondoDZ.XDZREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.ToString()) : (short)0; List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                        if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                            if (codiceSpecifico != null)
                            {
                                fondoDZ.XDZSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : "";
                            }
                        }
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoDZ != null)
                    {
                        GestioneFondo.DatiFondoDZ datiFondoDZ = objectFondoXX as GestioneFondo.DatiFondoDZ;
                        if (datiFondoDZ != null)
                        {
                            fondoDZ.XDZ50CLA = datiFondoDZ.ClasseAnte50.HasValue ? datiFondoDZ.ClasseAnte50.Value : (short)0;
                            fondoDZ.XDZANZAA = datiFondoDZ.MaggiorazioneAnzianitaEsodoAA.HasValue ? datiFondoDZ.MaggiorazioneAnzianitaEsodoAA.Value : (short)0;
                            fondoDZ.XDZANZMM = datiFondoDZ.MaggiorazioneAnzianitaEsodoMM.HasValue ? datiFondoDZ.MaggiorazioneAnzianitaEsodoMM.Value : (short)0;
                            fondoDZ.XDZCESAA = datiFondoDZ.DataCessazioneServizio.HasValue ? (short)datiFondoDZ.DataCessazioneServizio.Value.Year : (short)0;
                            fondoDZ.XDZCESMM = datiFondoDZ.DataCessazioneServizio.HasValue ? (short)datiFondoDZ.DataCessazioneServizio.Value.Month : (short)0;
                            fondoDZ.XDZCESGG = datiFondoDZ.DataCessazioneServizio.HasValue ? (short)datiFondoDZ.DataCessazioneServizio.Value.Day : (short)0;
                            fondoDZ.XDZCODDZ = datiFondoDZ.CodiceDZ.HasValue ? datiFondoDZ.CodiceDZ.Value ? (short)1 : (short)0 : (short)0;
                            fondoDZ.XDZCODES = datiFondoDZ.CodiceEsodo.HasValue ? datiFondoDZ.CodiceEsodo.Value ? (short)1 : (short)0 : (short)0;
                            fondoDZ.XDZCODIG = datiFondoDZ.CodiceBenefici.HasValue ? datiFondoDZ.CodiceBenefici.Value : (short)0;
                            short res = 0;
                            short.TryParse(datiFondoDZ.Ditta, out res);
                            fondoDZ.XDZCODIT = res;
                            fondoDZ.XDZCPANE = datiFondoDZ.CodiceCaroPane.HasValue ? datiFondoDZ.CodiceCaroPane.Value ? (short)1 : (short)0 : (short)0;
                            fondoDZ.XDZPERCE = datiFondoDZ.PercentualeLiquidazionePensione.HasValue ? datiFondoDZ.PercentualeLiquidazionePensione.Value : 0;
                            fondoDZ.XDZPRIVA = datiFondoDZ.MaggiorazionePensionePrivilegiataAA.HasValue ? datiFondoDZ.MaggiorazionePensionePrivilegiataAA.Value : (short)0;
                            fondoDZ.XDZPRIVM = datiFondoDZ.MaggiorazionePensionePrivilegiataMM.HasValue ? datiFondoDZ.MaggiorazionePensionePrivilegiataMM.Value : (short)0;
                            fondoDZ.XDZRETNO = datiFondoDZ.RetribuzioneAlNettoBeneficiEsodo.HasValue ? datiFondoDZ.RetribuzioneAlNettoBeneficiEsodo.Value : 0;
                            fondoDZ.XDZRISAA = datiFondoDZ.RiscattiAA.HasValue ? datiFondoDZ.RiscattiAA.Value : (short)0;
                            fondoDZ.XDZRISMM = datiFondoDZ.RiscattiMM.HasValue ? datiFondoDZ.RiscattiMM.Value : (short)0;
                            fondoDZ.XDZANBAS = recordFondo.PensioneBaseAnnua.HasValue ? recordFondo.PensioneBaseAnnua.Value : 0M;
                        }
                    }

                    List<GestioneDatiServizioUtile.ServizioUtile> datiSU = datiServizioUtile.FindAll(x => x.IdRecordFondo == recordFondo.Id);
                    if (datiSU != null && datiSU.Count > 0)
                    {
                        foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiSU)
                        {
                            short servizioUtileAA = sU.ServizioUtileAA.HasValue ? sU.ServizioUtileAA.Value : (short)0;
                            short servizioUtileMM = sU.ServizioUtileMM.HasValue ? sU.ServizioUtileMM.Value : (short)0;
                            decimal retribuzionePensionabile = sU.RetribuzionePensionabile.HasValue ? sU.RetribuzionePensionabile.Value : 0;

                            switch (sU.Quota.Trim().ToUpperInvariant())
                            {
                                case "A":
                                    fondoDZ.XDZUTIAA = servizioUtileAA;
                                    fondoDZ.XDZUTIMM = servizioUtileMM;
                                    fondoDZ.XDZRETRI = retribuzionePensionabile;
                                    break;
                                case "B":
                                    fondoDZ.XDZUT2AA = servizioUtileAA;
                                    fondoDZ.XDZUT2MM = servizioUtileMM;
                                    fondoDZ.XDZRETR2 = retribuzionePensionabile;
                                    break;
                            }
                        }
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        if (!string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                        {
                            short resShort = 0;
                            short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                            if (resShort < 10)
                                fondoDZ.XDZNONVE = resShort;
                        }

                        if (datiMaggiorazioniBenefici.ExCombattente.HasValue)
                        {
                            List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiciMaggExComb = null;
                            GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiciMaggExComb);
                            if (listaCodiciMaggExComb != null && listaCodiciMaggExComb.Count > 0)
                            {
                                GestioneDecodifica.CodiceMaggiorazioneExCombattenti codMaggExComb = listaCodiciMaggExComb.Find(x => x.Id == datiMaggiorazioniBenefici.ExCombattente.Value);
                                if (codMaggExComb != null)
                                    fondoDZ.XDZCOMBA = codMaggExComb.TraduzioneSuGP;
                            }
                        }
                        //QUOTA A
                        fondoDZ.XDZNO336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QA.Value : 0M;
                        //QUOTA B
                        fondoDZ.XDZN2336 = datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue ? datiMaggiorazioniBenefici.RMSSenzaLegge33670QB.Value : 0M;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoDZ.Add(fondoDZ);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoDZ[0].XDZTIPOR));
                }
            }
        }

        public static void ValorizzaFondoCL(GestionePensione.DatiPensione datiPensione, Object objectFondoXX, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoCL = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.CL>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.CL fondoCL = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.CL();
                    fondoCL.XCLTIPOR = "X";
                    fondoCL.XCLFONDO = "CL";
                    fondoCL.XCLPROGR = record;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoCL.XCLNATUR = codNatura1;
                    }
                    fondoCL.XCLDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoCL.XCLDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    fondoCL.XCLSCAAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoCL.XCLSCAMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoCL.XCLNONCA = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    fondoCL.XCLNAFIL = recordFondo.CodiceNatura2.HasValue && recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura2.Value.ToString() + recordFondo.CodiceNatura3.Value.ToString() : string.Empty;
                    fondoCL.XCLPVRAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoCL.XCLPVRMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoCL.XCLPVRGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoCL.XCLUVRAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoCL.XCLUVRMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoCL.XCLUVRGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;
                    if (datiFondo != null)
                    {
                        fondoCL.XCLTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        fondoCL.XCLREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : string.Empty;
                        fondoCL.XCLREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("CL", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                short res = 0;
                                short.TryParse(attSvolta.TraduzioneSuGp, out res);
                                fondoCL.XCLATTIV = res;
                            }
                        }
                    }
                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoCL != null)
                    {
                        GestioneFondo.DatiFondoCL datiFondoCL = objectFondoXX as GestioneFondo.DatiFondoCL;
                        fondoCL.XCLVITAL = datiFondoCL.ImportoAltraPensione.HasValue ? datiFondoCL.ImportoAltraPensione.Value : 0M;
                        //ENG - CodicePensioneSenzaRequisiti valorizzato per le PL e RIC categoria pensione VCL
                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda && datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() != "VCL")
                            fondoCL.XCLNOREQ = 0;
                        else
                            fondoCL.XCLNOREQ = datiFondoCL.CodicePensioneSenzaRequisiti.HasValue ? datiFondoCL.CodicePensioneSenzaRequisiti.Value ? (short)1 : (short)0 : (short)0;
                        fondoCL.XCLDIFFE = datiFondoCL.AnniDifferimento.HasValue ? datiFondoCL.AnniDifferimento.Value : (short)0;
                        fondoCL.XCLPERFE = datiFondoCL.EtaPerfezionamentoRequisiti.HasValue ? (short)datiFondoCL.EtaPerfezionamentoRequisiti : (short)0;
                        fondoCL.XCLAAREQ = datiFondoCL.DataPerfezionamentoRequisiti.HasValue ? (short)datiFondoCL.DataPerfezionamentoRequisiti.Value.Year : (short)0;
                        fondoCL.XCLMMREQ = datiFondoCL.DataPerfezionamentoRequisiti.HasValue ? (short)datiFondoCL.DataPerfezionamentoRequisiti.Value.Month : (short)0;
                        fondoCL.XCLCONTR_PROV = datiFondoCL.ContrProvv.HasValue ? datiFondoCL.ContrProvv.ToString() : string.Empty;
                    }

                    if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        short resShort = 0;
                        short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                        if (resShort < 10)
                            fondoCL.XCLNONVE = resShort;
                    }

                    List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listaDatiServizioUtile);
                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                    {
                        fondoCL.XCLUTIAA = listaDatiServizioUtile[0].ServizioUtileAA.GetValueOrDefault();
                        fondoCL.XCLUTIMM = listaDatiServizioUtile[0].ServizioUtileMM.GetValueOrDefault();
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoCL.Add(fondoCL);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoCL[0].XCLTIPOR));
                }
            }
        }

        public static void ValorizzaFondoPM(GestionePensione.DatiPensione datiPensione, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo, Object objectFondoXX,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaFondoPM = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PM>();
                short record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    record++;
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PM fondoPM = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Fondo.PM();
                    fondoPM.XPMTIPOR = "X";
                    fondoPM.XPMFONDO = "PM";
                    fondoPM.XPMPROGR = record;
                    fondoPM.XPMDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                    fondoPM.XPMDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    if (recordFondo.CodiceNatura1.HasValue)
                    {
                        short codNatura1 = 0;
                        short.TryParse(recordFondo.CodiceNatura1.Value.ToString(), out codNatura1);
                        fondoPM.XPMNATU1 = codNatura1;
                    }
                    fondoPM.XPMNATU2 = recordFondo.CodiceNatura2.HasValue ? recordFondo.CodiceNatura2.Value.ToString() : "";
                    fondoPM.XPMNATU3 = recordFondo.CodiceNatura3.HasValue ? recordFondo.CodiceNatura3.Value.ToString() : "";
                    fondoPM.XPMNCALC = recordFondo.CodiceNonCalcolo.GetValueOrDefault() == 'S' ? (short)1 : (short)0;
                    fondoPM.XPMSOSAA = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Year : (short)0;
                    fondoPM.XPMSOSMM = recordFondo.DataSospensione.HasValue ? (short)recordFondo.DataSospensione.Value.Month : (short)0;
                    fondoPM.XPMPRIAA = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Year : (short)0;
                    fondoPM.XPMPRIMM = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Month : (short)0;
                    fondoPM.XPMPRIGG = datiPensione.InizioAssicurazione.HasValue ? (short)datiPensione.InizioAssicurazione.Value.Day : (short)0;
                    fondoPM.XPMULTAA = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Year : (short)0;
                    fondoPM.XPMULTMM = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Month : (short)0;
                    fondoPM.XPMULTGG = datiPensione.FineAssicurazione.HasValue ? (short)datiPensione.FineAssicurazione.Value.Day : (short)0;

                    StringBuilder xpmattiv = new StringBuilder();
                    if (datiFondo != null)
                    {
                        fondoPM.XPMTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                        if (!String.IsNullOrEmpty(datiFondo.AttivitaSvolta) && datiFondo.AttivitaSvolta.Trim() != "")
                        {
                            List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                            GestioneDecodifica.GetAttivitaSvoltaByFondo("PM", null, out elencoAttivitaSvolte);
                            if (elencoAttivitaSvolte != null && elencoAttivitaSvolte.Count > 0)
                            {
                                GestioneDecodifica.AttivitaSvolta attSvolta = elencoAttivitaSvolte.Find(x => x.Id == datiFondo.AttivitaSvolta);
                                if (attSvolta != null)
                                    xpmattiv.Append(attSvolta.TraduzioneSuGp);
                                else
                                    xpmattiv.Append(" ");
                            }
                        }
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoPM != null)
                    {
                        GestioneFondo.DatiFondoPM datiFondoPM = objectFondoXX as GestioneFondo.DatiFondoPM;
                        fondoPM.XPMANULT = datiFondoPM.AnnoUtileUltimoDecennio.GetValueOrDefault() ? (short)1 : (short)0;
                        fondoPM.XPMTILIQ = datiFondoPM.TipoLiquidazione.HasValue ? datiFondoPM.TipoLiquidazione.Value : (short)0;
                        if (datiFondoPM.AttivitaSvolta2.HasValue)
                            xpmattiv.Append(datiFondoPM.AttivitaSvolta2.Value.ToString());
                        fondoPM.XPMATTIV = xpmattiv.ToString();
                    }

                    if (datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                    {
                        short resShort = 0;
                        short.TryParse(datiMaggiorazioniBenefici.TipoSettimaneBeneficio, out resShort);
                        if (resShort < 10)
                            fondoPM.XPMNONVE = resShort;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaFondoPM.Add(fondoPM);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new INPS.Pensioni.LiquidazioneFs.Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaFondoPM[0].XPMTIPOR));
                }
            }
        }

        public static void ValorizzaGp4Ipost(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheAventiDiritto = null;
            GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagraficheAventiDiritto);
            if (listaAventiDiritto == null || listaAventiDiritto.Count == 0)
                return;
            List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDiritto = null;
            GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, null, out listaPeriodiAventiDiritto);

            AreaCalcolo.AreaInputVariabile.ListaGp4IPOST = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST>();

            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST gp4Ipost = new Data.CMSGTRA.Gp4IPOST();

            if (datiDanteCausa != null)
            {
                if (datiDanteCausa.CategoriaFascicolo.HasValue && datiDanteCausa.SedeFascicolo.HasValue && datiDanteCausa.NumeroFascicolo.HasValue)
                {
                    gp4Ipost.K_GP4DAA1 = datiDanteCausa.CategoriaFascicolo.Value;
                    gp4Ipost.K_GP4DAA2_1 = datiDanteCausa.SedeFascicolo.Value;
                    gp4Ipost.K_GP4DAA2_2 = datiDanteCausa.NumeroFascicolo.Value;
                }
            }

            if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
            {
                if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                {
                    listaAventiDiritto.ForEach(x => x.ListaPeriodi = listaPeriodiAventiDiritto.FindAll(y => y.IdAventeDiritto == x.Id));
                }

                gp4Ipost.LISTK_GP4DB00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DB00>();

                foreach (var aventeDiritto in listaAventiDiritto)
                {
                    INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DB00 gp4db00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DB00();
                    GestioneAnagrafica.DatiAnagrafici anagraficaAventeDiritto = listaAnagraficheAventiDiritto.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    if (aventeDiritto.IdAnagrafica == datiAnagraficiTitolare.Id)
                    {
                        string codCat = datiPensione.GetCodCategoria();
                        gp4db00.K_GP4KA01 = codCat.Length > 3 ? codCat.Substring(1, 3) : codCat;
                        short sede = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;
                        int nCertificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                        gp4db00.K_GP4KA02 = sede.ToString().PadLeft(4, '0').Substring(0, 2);
                        gp4db00.K_GP4KA03 = sede.ToString().PadLeft(4, '0').Substring(2, 2);
                        gp4db00.K_GP4KA04 = nCertificato.ToString().PadLeft(8, '0');
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(aventeDiritto.CategoriaPensione))
                            gp4db00.K_GP4KA01 = aventeDiritto.CategoriaPensione;
                        if (aventeDiritto.SedePensione.HasValue)
                        {
                            gp4db00.K_GP4KA02 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(0, 2);
                            gp4db00.K_GP4KA03 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(2, 2);
                        }
                        if (aventeDiritto.CertificatoPensione.HasValue)
                            gp4db00.K_GP4KA04 = aventeDiritto.CertificatoPensione.Value.ToString().PadLeft(8, '0');
                    }
                    if (anagraficaAventeDiritto != null && !string.IsNullOrEmpty(anagraficaAventeDiritto.CodiceFiscale))
                        gp4db00.K_GP4DB09 = anagraficaAventeDiritto.CodiceFiscale;
                    if (aventeDiritto.CSog.HasValue)
                        gp4db00.K_GP4DB13 = aventeDiritto.CSog.Value;
                    if (aventeDiritto.IdAnagrafica == datiAnagraficiTitolare.Id)
                    {
                        if (datiDanteCausa != null && datiAnagraficiDanteCausa.DataMatrimonio.HasValue)
                        {
                            int data = 0;
                            int.TryParse(datiAnagraficiDanteCausa.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                                datiAnagraficiDanteCausa.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                                datiAnagraficiDanteCausa.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                            gp4db00.K_GP4DB14 = data;
                        }
                    }
                    else if (aventeDiritto.DataMatrimonio.HasValue)
                    {
                        int data = 0;
                        int.TryParse(aventeDiritto.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                        gp4db00.K_GP4DB14 = data;
                    }
                    gp4db00.K_GP4DB15 = aventeDiritto.CodiceNucleo;
                    if (aventeDiritto.ListaPeriodi != null && aventeDiritto.ListaPeriodi.Count > 0)
                    {
                        gp4db00.LISTK_GP4DC00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00>();
                        foreach (var periodo in aventeDiritto.ListaPeriodi)
                        {
                            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00 gp4dc00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00();
                            if (periodo.PercSpettante.HasValue)
                                gp4dc00.K_GP4DC01 = periodo.PercSpettante.Value;
                            if (periodo.DecorrenzaPeriodo.HasValue)
                                gp4dc00.K_GP4DC02 = int.Parse(periodo.DecorrenzaPeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.DecorrenzaPeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            if (periodo.CessazionePeriodo.HasValue)
                                gp4dc00.K_GP4DC03 = int.Parse(periodo.CessazionePeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.CessazionePeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            else
                                gp4dc00.K_GP4DC03 = 999999;
                            if (periodo.GradoParentela.HasValue)
                            {
                                if (periodo.TipoUnione == "U")
                                    gp4dc00.K_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString() + periodo.TipoUnione;
                                else
                                    gp4dc00.K_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString();
                            }
                            if (periodo.CoeffRiduzione.HasValue)
                                gp4dc00.K_GP4DC05 = periodo.CoeffRiduzione.Value;
                            if (periodo.PercGiudice.HasValue)
                                gp4dc00.K_GP4DC07 = periodo.PercGiudice.Value;

                            gp4db00.LISTK_GP4DC00.Add(gp4dc00);
                        }
                    }
                    else //caso di avente diritto non richiedente senza periodi su GP4
                    {
                        gp4db00.LISTK_GP4DC00 = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00>();
                        INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00 gp4dc00 = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Gp4IPOST.K_GP4DC00();
                        if (aventeDiritto.TipoUnione == "U")
                            gp4dc00.K_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString() + aventeDiritto.TipoUnione;
                        else
                            gp4dc00.K_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString();

                        if (aventeDiritto.DecParentelaDA == 'M' && anagraficaAventeDiritto != null && anagraficaAventeDiritto.DataNascita > datiAnagraficiDanteCausa.DataMorte)
                        {
                            DateTime decorrenza = Utility.FirstDayOfMonth(anagraficaAventeDiritto.DataNascita.Value.AddMonths(1));
                            gp4dc00.K_GP4DC02 = int.Parse(decorrenza.Year.ToString().PadLeft(4, '0') + decorrenza.Month.ToString().PadLeft(2, '0'));
                        }
                        else
                            gp4dc00.K_GP4DC02 = int.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0'));
                        gp4dc00.K_GP4DC03 = 999999;
                        gp4db00.LISTK_GP4DC00.Add(gp4dc00);
                    }

                    gp4Ipost.LISTK_GP4DB00.Add(gp4db00);
                }
            }
            AreaCalcolo.AreaInputVariabile.ListaGp4IPOST.Add(gp4Ipost);
        }

        #endregion Fondo

        #region Ago
        public static void ValorizzaAgoEL(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, GestioneDL407.DatiDL407 datiDL407, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) &&
                Utility.IsDomandaAnteArmonizzazione(datiPensione, Utility.TipoFondo.EL, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return;

            INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.EL agoEL = new INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.EL();
            agoEL.YELFONDO = "EL";
            agoEL.YELTIPOR = "Y";
            agoEL.YELPROGR = 1;
            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                agoEL.YELDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                agoEL.YELDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                agoEL.YELDECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            }
            else
            {
                agoEL.YELDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                agoEL.YELDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                agoEL.YELDECSS = short.Parse(agoEL.YELDECAA.ToString().PadLeft(4, '0').Substring(0, 2));
            }
            if (datiCalcoloContributivo != null)
            {
                agoEL.YELCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                agoEL.YELMONTA = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                agoEL.YELSETTE = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;
                agoEL.YELIMPCRT = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                agoEL.YELMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoEL.YELSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
            }

            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
            if (datiCalcoloRetributivo != null)
            {
                agoEL.YELRSETA = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                agoEL.YELRSETB = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                agoEL.YELRSETD = datiCalcoloRetributivo.RMSQuotaD.HasValue ? datiCalcoloRetributivo.RMSQuotaD.Value : 0M;
                agoEL.YELSETTA = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoEL.YELSETTB = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                agoEL.YELSETTC = datiCalcoloRetributivo.NSettimaneQuotaC.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaC.Value : 0;
                agoEL.YELSETTD = datiCalcoloRetributivo.NSettimaneQuotaD.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaD.Value : 0;
                agoEL.YELTETTO = datiCalcoloRetributivo.RetribuzionePonderataAnnua.HasValue ? datiCalcoloRetributivo.RetribuzionePonderataAnnua.Value : 0M;
            }

            if (datiDL407 == null)
                GestioneDL407.GetDL407ByIdPensione(datiPensione.Id, out datiDL407);
            if (datiDL407 != null)
            {
                agoEL.YELRETRA = datiDL407.RMSQuotaA.HasValue ? datiDL407.RMSQuotaA.Value : 0M;
                agoEL.YELRETRB = datiDL407.RMSQuotaB.HasValue ? datiDL407.RMSQuotaB.Value : 0M;
                agoEL.YELRETRD = datiDL407.RMSQuotaD.HasValue ? datiDL407.RMSQuotaD.Value : 0M;
                agoEL.YELRETTA = datiDL407.NSettimaneQuotaA.HasValue ? datiDL407.NSettimaneQuotaA.Value : 0;
                agoEL.YELRETTB = datiDL407.NSettimaneQuotaB.HasValue ? datiDL407.NSettimaneQuotaB.Value : 0;
                agoEL.YELRETTC = datiDL407.NSettimaneQuotaC.HasValue ? datiDL407.NSettimaneQuotaC.Value : 0;
                agoEL.YELRETTD = datiDL407.NSettimaneQuotaD.HasValue ? datiDL407.NSettimaneQuotaD.Value : 0;
            }

            if (datiFondo != null)
            {
                agoEL.YELTIPEN = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                agoEL.YELFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoEL.YELPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;
                //Comma 707
                agoEL.YELIMP707 = datiFondo.RetribuzionePonderataAGO707.HasValue ? datiFondo.RetribuzionePonderataAGO707.Value : 0M;
                agoEL.YELSETA707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : (short)0;
                agoEL.YELSETB707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : (short)0;
                agoEL.YELSETC707 = datiFondo.QuotaC707.HasValue ? datiFondo.QuotaC707.Value : (short)0;
                agoEL.YELSETD707 = datiFondo.QuotaD707.HasValue ? datiFondo.QuotaD707.Value : (short)0;
                agoEL.YELSETDIR = datiFondo.SettimaneUtiliDiritto.HasValue ? datiFondo.SettimaneUtiliDiritto.Value : (int)0;
                // Al momento non verrà mappato perchè non abbiamo le specifiche
                //agoEL.YELCALC707    
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoEL = new List<Data.CMSGTRA.Ago.EL> { agoEL };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoEL[0].YELTIPOR));
        }

        public static void ValorizzaAgoTT(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) &&
                Utility.IsDomandaAnteArmonizzazione(datiPensione, Utility.TipoFondo.TT, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: objectFondoXX))
                return;

            Data.CMSGTRA.Ago.TT agoTT = new Data.CMSGTRA.Ago.TT();
            agoTT.YTTFONDO = "TT";
            agoTT.YTTTIPOR = "Y";
            agoTT.YTTPROGR = 1;
            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                agoTT.YTTDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                agoTT.YTTDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                agoTT.YTTDECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            }
            else
            {
                agoTT.YTTDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                agoTT.YTTDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                agoTT.YTTDECSS = short.Parse(agoTT.YTTDECAA.ToString().PadLeft(4, '0').Substring(0, 2));
            }

            if (datiCalcoloContributivo != null)
            {
                agoTT.YTTCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                agoTT.YTTMONTA = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                agoTT.YTTSETTE = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;
                agoTT.YTTIMPCRT = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                agoTT.YTTMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoTT.YTTSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
            }

            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
            if (datiCalcoloRetributivo != null)
            {
                agoTT.YTTRSETA = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                agoTT.YTTRSETB = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                agoTT.YTTRSETD = datiCalcoloRetributivo.RMSQuotaD.HasValue ? datiCalcoloRetributivo.RMSQuotaD.Value : 0M;
                agoTT.YTTSETTA = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoTT.YTTSETTB = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                agoTT.YTTSETTC = datiCalcoloRetributivo.NSettimaneQuotaC.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaC.Value : 0;
                agoTT.YTTSETTD = datiCalcoloRetributivo.NSettimaneQuotaD.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaD.Value : 0;
                agoTT.YTTTETTO = datiCalcoloRetributivo.RetribuzionePonderataAnnua.HasValue ? datiCalcoloRetributivo.RetribuzionePonderataAnnua.Value : 0M;
            }

            GestioneDL407.DatiDL407 datiDL407 = null;
            GestioneDL407.GetDL407ByIdPensione(datiPensione.Id, out datiDL407);
            if (datiDL407 != null)
            {
                agoTT.YTTRETRA = datiDL407.RMSQuotaA.HasValue ? datiDL407.RMSQuotaA.Value : 0M;
                agoTT.YTTRETRB = datiDL407.RMSQuotaB.HasValue ? datiDL407.RMSQuotaB.Value : 0M;
                agoTT.YTTRETRD = datiDL407.RMSQuotaD.HasValue ? datiDL407.RMSQuotaD.Value : 0M;
                agoTT.YTTRETTA = datiDL407.NSettimaneQuotaA.HasValue ? datiDL407.NSettimaneQuotaA.Value : 0;
                agoTT.YTTRETTB = datiDL407.NSettimaneQuotaB.HasValue ? datiDL407.NSettimaneQuotaB.Value : 0;
                agoTT.YTTRETTC = datiDL407.NSettimaneQuotaC.HasValue ? datiDL407.NSettimaneQuotaC.Value : 0;
                agoTT.YTTRETTD = datiDL407.NSettimaneQuotaD.HasValue ? datiDL407.NSettimaneQuotaD.Value : 0;
            }

            if (datiFondo != null)
            {
                agoTT.YTTTIPEN = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                agoTT.YTTFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoTT.YTTPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;
                //Comma 707
                agoTT.YTTIMP707 = datiFondo.RetribuzionePonderataAGO707.HasValue ? datiFondo.RetribuzionePonderataAGO707.Value : 0M;
                agoTT.YTTSETA707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : (short)0;
                agoTT.YTTSETB707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : (short)0;
                agoTT.YTTSETC707 = datiFondo.QuotaC707.HasValue ? datiFondo.QuotaC707.Value : (short)0;
                agoTT.YTTSETD707 = datiFondo.QuotaD707.HasValue ? datiFondo.QuotaD707.Value : (short)0;
                agoTT.YTTSETDIR = datiFondo.SettimaneUtiliDiritto.HasValue ? datiFondo.SettimaneUtiliDiritto.Value : (int)0;
                // 2015-01-22 G.Arru - Al momento non verrà mappato perchè non abbiamo le specifiche
                //agoTT.YTTCALC707    
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoTT = new List<Data.CMSGTRA.Ago.TT> { agoTT };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoTT[0].YTTTIPOR));
        }

        public static void ValorizzaAgoET(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) &&
                Utility.IsDomandaAnteArmonizzazione(datiPensione, Utility.TipoFondo.ET, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return;

            GestioneFondo.DatiFondoET datiFondoET = objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoET != null ? objectFondoXX as GestioneFondo.DatiFondoET : null;
            Data.CMSGTRA.Ago.ET agoET = new Data.CMSGTRA.Ago.ET();
            agoET.YETFONDO = "ET";
            agoET.YETTIPOR = "Y";
            agoET.YETPROGR = 1;
            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                agoET.YETDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                agoET.YETDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                agoET.YETDECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            }
            else
            {
                agoET.YETDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                agoET.YETDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                agoET.YETDECSS = short.Parse(agoET.YETDECAA.ToString().PadLeft(4, '0').Substring(0, 2));
            }
            if (datiCalcoloContributivo != null)
            {
                agoET.YETCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                agoET.YETMONTA = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                agoET.YETSETTE = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;
                agoET.YETIMPCRT = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                agoET.YETMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoET.YETSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
            }

            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
            if (datiCalcoloRetributivo != null)
            {
                agoET.YETRSETA = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                agoET.YETRSETB = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                agoET.YETRSETD = datiCalcoloRetributivo.RMSQuotaD.HasValue ? datiCalcoloRetributivo.RMSQuotaD.Value : 0M;
                agoET.YETSETTA = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoET.YETSETTB = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                agoET.YETSETTC = datiCalcoloRetributivo.NSettimaneQuotaC.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaC.Value : 0;
                agoET.YETSETTD = datiCalcoloRetributivo.NSettimaneQuotaD.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaD.Value : 0;
                agoET.YETTETTO = datiCalcoloRetributivo.RetribuzionePonderataAnnua.HasValue ? datiCalcoloRetributivo.RetribuzionePonderataAnnua.Value : 0M;
            }

            GestioneDL407.DatiDL407 datiDL407 = null;
            GestioneDL407.GetDL407ByIdPensione(datiPensione.Id, out datiDL407);
            if (datiDL407 != null)
            {
                agoET.YETRETRA = datiDL407.RMSQuotaA.HasValue ? datiDL407.RMSQuotaA.Value : 0M;
                agoET.YETRETRB = datiDL407.RMSQuotaB.HasValue ? datiDL407.RMSQuotaB.Value : 0M;
                agoET.YETRETRD = datiDL407.RMSQuotaD.HasValue ? datiDL407.RMSQuotaD.Value : 0M;
                agoET.YETRETTA = datiDL407.NSettimaneQuotaA.HasValue ? datiDL407.NSettimaneQuotaA.Value : 0;
                agoET.YETRETTB = datiDL407.NSettimaneQuotaB.HasValue ? datiDL407.NSettimaneQuotaB.Value : 0;
                agoET.YETRETTC = datiDL407.NSettimaneQuotaC.HasValue ? datiDL407.NSettimaneQuotaC.Value : 0;
                agoET.YETRETTD = datiDL407.NSettimaneQuotaD.HasValue ? datiDL407.NSettimaneQuotaD.Value : 0;
            }

            if (datiFondo != null)
            {
                agoET.YETFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoET.YETPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;

                //Comma 707 ET
                agoET.YETSETAFAA707 = datiFondo.QuotaA707AA.HasValue ? datiFondo.QuotaA707AA.Value : (short)0;
                agoET.YETSETAFMM707 = datiFondo.QuotaA707MM.HasValue ? datiFondo.QuotaA707MM.Value : (short)0;
                agoET.YETSETAFGG707 = datiFondo.QuotaA707GG.HasValue ? datiFondo.QuotaA707GG.Value : (short)0;
                agoET.YETSETBFAA707 = datiFondo.QuotaB707AA.HasValue ? datiFondo.QuotaB707AA.Value : (short)0;
                agoET.YETSETBFMM707 = datiFondo.QuotaB707MM.HasValue ? datiFondo.QuotaB707MM.Value : (short)0;
                agoET.YETSETBFGG707 = datiFondo.QuotaB707GG.HasValue ? datiFondo.QuotaB707GG.Value : (short)0;
                agoET.YETSETCFAA707 = datiFondo.QuotaC707AA.HasValue ? datiFondo.QuotaC707AA.Value : (short)0;
                agoET.YETSETCFMM707 = datiFondo.QuotaC707MM.HasValue ? datiFondo.QuotaC707MM.Value : (short)0;
                agoET.YETSETCFGG707 = datiFondo.QuotaC707GG.HasValue ? datiFondo.QuotaC707GG.Value : (short)0;
                agoET.YETSETAGOA707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : (short)0;
                agoET.YETSETAGOB707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : (short)0;
                agoET.YETIMP707 = datiFondo.RetribuzionePonderataAGO707.HasValue ? datiFondo.RetribuzionePonderataAGO707.Value : 0M;
                agoET.YETSETDIR = datiFondo.SettimaneUtiliDiritto.HasValue ? datiFondo.SettimaneUtiliDiritto.Value : (int)0;
                // Al momento non verrà mappato perchè non abbiamo le specifiche
                //agoET.YETCALC707    
            }

            if (datiFondoET != null)
            {
                agoET.YETANZTO = datiFondoET.SetAnzTotAltraPensione ?? 0;
                agoET.YETBASEA = datiFondoET.BaseAltraPensione ?? 0M;
                agoET.YETCATEG = !string.IsNullOrEmpty(datiFondoET.CategoriaAltraPensione) ? datiFondoET.CategoriaAltraPensione : string.Empty;
                agoET.YETCERTI = datiFondoET.CertificatoAltraPensione ?? 0;
                agoET.YETMEDIM = datiFondoET.RmsImpAltraPensione ?? 0M;
                agoET.YETORIAA = datiFondoET.DecorrenzaAltraPensione.HasValue ? (short)datiFondoET.DecorrenzaAltraPensione.Value.Year : (short)0;
                agoET.YETORIMM = datiFondoET.DecorrenzaAltraPensione.HasValue ? (short)datiFondoET.DecorrenzaAltraPensione.Value.Month : (short)0;
                agoET.YETRIVPR = datiFondoET.RevAltraPensione ?? 0;
                agoET.YETSP1AA = datiFondoET.DecorrenzaPrimoSupplemento.HasValue ? (short)datiFondoET.DecorrenzaPrimoSupplemento.Value.Year : (short)0;
                agoET.YETSP1MM = datiFondoET.DecorrenzaPrimoSupplemento.HasValue ? (short)datiFondoET.DecorrenzaPrimoSupplemento.Value.Month : (short)0;
                agoET.YETSP1CT = datiFondoET.ImpContribPrimoSupplemento ?? 0;
                agoET.YETSP2AA = datiFondoET.DecorrenzaSecondoSupplemento.HasValue ? (short)datiFondoET.DecorrenzaSecondoSupplemento.Value.Year : (short)0;
                agoET.YETSP2MM = datiFondoET.DecorrenzaSecondoSupplemento.HasValue ? (short)datiFondoET.DecorrenzaSecondoSupplemento.Value.Month : (short)0;
                agoET.YETSP2CT = datiFondoET.ImpContribSecondoSupplemento ?? 0;
                agoET.YETTIPLQ = datiFondoET.TipoLiquidazione ?? 0;
            }
            AreaCalcolo.AreaInputVariabile.ListaAgoET = new List<Data.CMSGTRA.Ago.ET> { agoET };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoET[0].YETTIPOR));
        }

        public static void ValorizzaAgoVL(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref Data.FSPL_FSRC AreaCalcolo)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) &&
                Utility.IsDomandaAnteArmonizzazione(datiPensione, Utility.TipoFondo.VL, decorrenzaPensioneOrDecorrenzaPensioneDC))
                return;

            GestioneFondo.DatiFondoVL datiFondoVL = objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoVL != null ? objectFondoXX as GestioneFondo.DatiFondoVL : null;
            Data.CMSGTRA.Ago.VL agoVL = new Data.CMSGTRA.Ago.VL();
            agoVL.YVLFONDO = "VL";
            agoVL.YVLTIPOR = "Y";
            agoVL.YVLPROGR = 1;
            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);

            if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                agoVL.YVLDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
                agoVL.YVLDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                agoVL.YVLDECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            }
            else
            {
                agoVL.YVLDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
                agoVL.YVLDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                agoVL.YVLDECSS = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
            }

            byte? traduzioneSuGpTipoCalcolo = Utility.GetTraduzioneSuGpTipoCalcolo(datiPensione);
            agoVL.YVLTIPLQ = traduzioneSuGpTipoCalcolo.HasValue ? traduzioneSuGpTipoCalcolo.Value : (short)0;
            //mail 27/08/2013: in caso di contributivo occorre sostituire il valore 4 con il valore 2
            if (agoVL.YVLTIPLQ == 4)
                agoVL.YVLTIPLQ = 2;

            if (datiCalcoloContributivo != null)
            {
                agoVL.YVLCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                if (agoVL.YVLTIPLQ == 2)
                {
                    decimal sommaMontanti = datiCalcoloContributivo.MontanteAnte0697.GetValueOrDefault() + datiCalcoloContributivo.Montante.GetValueOrDefault();
                    agoVL.YVLMONTA = sommaMontanti;
                    agoVL.YVLMONT2 = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                }
                else
                {
                    agoVL.YVLMONTA = datiCalcoloContributivo.MontanteAnte0697.HasValue ? datiCalcoloContributivo.MontanteAnte0697.Value : 0M;
                    agoVL.YVLMONT2 = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                }
                agoVL.YVLANZ1A = datiCalcoloContributivo.AnzianitaAnte0697AA.HasValue ? datiCalcoloContributivo.AnzianitaAnte0697AA.Value : (short)0;
                agoVL.YVLANZ1G = datiCalcoloContributivo.AnzianitaAnte0697GG.HasValue ? datiCalcoloContributivo.AnzianitaAnte0697GG.Value : (short)0;
                agoVL.YVLANZ1M = datiCalcoloContributivo.AnzianitaAnte0697MM.HasValue ? datiCalcoloContributivo.AnzianitaAnte0697MM.Value : (short)0;
                agoVL.YVLANZ2A = datiCalcoloContributivo.AnzianitaPost0697AA.HasValue ? datiCalcoloContributivo.AnzianitaPost0697AA.Value : (short)0;
                agoVL.YVLANZ2G = datiCalcoloContributivo.AnzianitaPost0697GG.HasValue ? datiCalcoloContributivo.AnzianitaPost0697GG.Value : (short)0;
                agoVL.YVLANZ2M = datiCalcoloContributivo.AnzianitaPost0697MM.HasValue ? datiCalcoloContributivo.AnzianitaPost0697MM.Value : (short)0;
                agoVL.YVLIMPCRT = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                agoVL.YVLMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoVL.YVLSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : 0;
            }

            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
            if (datiCalcoloRetributivo != null)
            {
                agoVL.YVLRSETA = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                agoVL.YVLRSETB = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                agoVL.YVLRSETD = datiCalcoloRetributivo.RMSQuotaD.HasValue ? datiCalcoloRetributivo.RMSQuotaD.Value : 0M;
                agoVL.YVLSET1A = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoVL.YVLSET2A = datiCalcoloRetributivo.NSettimaneQuotaA2.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaA2.Value : 0;
                agoVL.YVLSETTB = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                agoVL.YVLSET1C = datiCalcoloRetributivo.NSettimaneQuotaC.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaC.Value : 0;
                agoVL.YVLSET2C = datiCalcoloRetributivo.NSettimaneQuotaC2.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaC2.Value : 0;
                agoVL.YVLSETTD = datiCalcoloRetributivo.NSettimaneQuotaD.HasValue ? (short)datiCalcoloRetributivo.NSettimaneQuotaD.Value : 0;
                agoVL.YVLTETTO = datiCalcoloRetributivo.RetribuzionePonderataAnnua.HasValue ? datiCalcoloRetributivo.RetribuzionePonderataAnnua.Value : 0M;
            }

            if (datiFondoVL != null)
            {
                agoVL.YVLPRECO = datiFondoVL.LavoratorePrecoce.HasValue ? datiFondoVL.LavoratorePrecoce.Value ? "S" : "N" : "N";
            }

            if (datiFondo != null)
            {
                agoVL.YVLTIPEN = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                agoVL.YVLFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoVL.YVLPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;

                //Comma 707
                agoVL.YVLSETA1707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : (short)0;
                agoVL.YVLSETA2707 = datiFondo.QuotaA2707.HasValue ? datiFondo.QuotaA2707.Value : (short)0;
                agoVL.YVLSETB707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : (short)0;
                agoVL.YVLSETC1707 = datiFondo.QuotaC707.HasValue ? datiFondo.QuotaC707.Value : (short)0;
                agoVL.YVLSETC2707 = datiFondo.QuotaC2707.HasValue ? datiFondo.QuotaC2707.Value : (short)0;
                agoVL.YVLSETD707 = datiFondo.QuotaD707.HasValue ? datiFondo.QuotaD707.Value : (short)0;
                agoVL.YVLSETDIR = datiFondo.SettimaneUtiliDiritto.HasValue ? datiFondo.SettimaneUtiliDiritto.Value : (int)0;
                // 2015-01-22 G.Arru - Al momento non verrà mappato perchè non abbiamo le specifiche
                //agoVL.YVLCALC707    
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoVL = new List<Data.CMSGTRA.Ago.VL> { agoVL };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoVL[0].YVLTIPOR));
        }

        public static void ValorizzaAgoGAS(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, Object objectFondoXX, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            GestioneFondo.DatiFondoGAS datiFondoGAS = objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoGAS != null ? objectFondoXX as GestioneFondo.DatiFondoGAS : null;

            Data.CMSGTRA.Ago.GAS agoGAS = new Data.CMSGTRA.Ago.GAS();
            agoGAS.YGAFONDO = "GAS";
            agoGAS.YGATIPOR = "Y";
            agoGAS.YGAPROGR = 1;

            if (datiFondoGAS != null)
            {
                agoGAS.YGATEOAA = datiFondoGAS.DecorrenzaTeorica.HasValue ? (short)datiFondoGAS.DecorrenzaTeorica.Value.Year : (short)0;
                agoGAS.YGATEOMM = datiFondoGAS.DecorrenzaTeorica.HasValue ? (short)datiFondoGAS.DecorrenzaTeorica.Value.Month : (short)0;
                agoGAS.YGASOSAA = datiFondoGAS.SospensioneAGO.HasValue ? (short)datiFondoGAS.SospensioneAGO.Value.Year : (short)0;
                agoGAS.YGASOSMM = datiFondoGAS.SospensioneAGO.HasValue ? (short)datiFondoGAS.SospensioneAGO.Value.Month : (short)0;
                if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
                {
                    agoGAS.YGADECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                    agoGAS.YGADECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                    agoGAS.YGADECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
                }
                else
                {
                    agoGAS.YGADECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    agoGAS.YGADECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    agoGAS.YGADECSS = short.Parse(agoGAS.YGADECAA.ToString().PadLeft(4, '0').Substring(0, 2));
                }
                agoGAS.YGATPLIQ = datiFondoGAS.CodiceTipoLiquidazione.HasValue ? datiFondoGAS.CodiceTipoLiquidazione.Value : (short)0;
                agoGAS.YGAANZVV = datiFondoGAS.SettimaneAnzianitaEsclusiva.HasValue ? datiFondoGAS.SettimaneAnzianitaEsclusiva.Value : 0;
                agoGAS.YGADIFFE = datiFondoGAS.AnniDifferimento.HasValue ? (short)datiFondoGAS.AnniDifferimento.Value : (short)0;
                agoGAS.YGAMATUR = datiFondoGAS.EtaMaturazioneRequisiti.HasValue ? datiFondoGAS.EtaMaturazioneRequisiti.Value : (short)0;
                agoGAS.YGASPECI = datiFondoGAS.CodiceSpecificoAgo.HasValue ? datiFondoGAS.CodiceSpecificoAgo.ToString() : string.Empty;
                agoGAS.YGACNTOT = datiFondoGAS.ContributiTotaliSupplementoDPR143271.HasValue ? datiFondoGAS.ContributiTotaliSupplementoDPR143271.Value : 0M;
                agoGAS.YGACNESC = datiFondoGAS.ContribuzioneEsclusivaDPR143271.HasValue ? datiFondoGAS.ContribuzioneEsclusivaDPR143271.Value : 0M;
                agoGAS.YGACNT14 = datiFondoGAS.CCTotaliArt14.HasValue ? datiFondoGAS.CCTotaliArt14.Value : 0M;
                agoGAS.YGACNE14 = datiFondoGAS.ContribuzioneEsclusiva.HasValue ? datiFondoGAS.ContribuzioneEsclusiva.Value : 0M;
                agoGAS.YGACDCAA = datiFondoGAS.DecDPCM.HasValue ? (short)datiFondoGAS.DecDPCM.Value.Year : (short)0;
                agoGAS.YGACDCMM = datiFondoGAS.DecDPCM.HasValue ? (short)datiFondoGAS.DecDPCM.Value.Month : (short)0;
                agoGAS.YGADPCRT = datiFondoGAS.RMSArt14.HasValue ? datiFondoGAS.RMSArt14.Value : 0M;
                agoGAS.YGAS72RT = datiFondoGAS.RMSSent72.HasValue ? datiFondoGAS.RMSSent72.Value : 0M;
                agoGAS.YGACNT11 = datiFondoGAS.CCTotaliArt11.HasValue ? datiFondoGAS.CCTotaliArt11.Value : 0M;
                agoGAS.YGACNE11 = datiFondoGAS.CCEsclusivaArt11.HasValue ? datiFondoGAS.CCEsclusivaArt11.Value : 0M;
            }

            if (datiFondo != null)
            {
                agoGAS.YGAREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : "";
                agoGAS.YGAREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                agoGAS.YGATPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                agoGAS.YGAFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoGAS.YGAPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;

                agoGAS.YGASETA_707 = datiFondo.QuotaA707.GetValueOrDefault();
                agoGAS.YGASETB_707 = datiFondo.QuotaB707.GetValueOrDefault();
                agoGAS.YGASETAES_707 = datiFondo.QuotaAES707.GetValueOrDefault();
                agoGAS.YGASETBES_707 = datiFondo.QuotaBES707.GetValueOrDefault();
            }

            GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
            if (datiCalcoloRetributivo != null)
            {
                agoGAS.YGARETPN = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                agoGAS.YGAANZTO = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoGAS.YGAANZES = datiCalcoloRetributivo.NSettimaneEsclusiveQuotaA.HasValue ? datiCalcoloRetributivo.NSettimaneEsclusiveQuotaA.Value : 0;
                agoGAS.YGARE2PN = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                agoGAS.YGAANZT2 = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                agoGAS.YGAANZE2 = datiCalcoloRetributivo.NSettimaneEsclusiveQuotaB.HasValue ? datiCalcoloRetributivo.NSettimaneEsclusiveQuotaB.Value : 0;
            }

            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);
            if (datiCalcoloContributivo != null)
            {
                agoGAS.YGAMONTA = datiCalcoloContributivo.Montante.HasValue ? datiCalcoloContributivo.Montante.Value : 0M;
                agoGAS.YGAESCLU = datiCalcoloContributivo.MontanteEsclusivo.HasValue ? datiCalcoloContributivo.MontanteEsclusivo.Value : 0M;
                agoGAS.YGASETTE = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;
                agoGAS.YGAMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoGAS.YGASETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
                agoGAS.YGAMONTAE2012 = datiCalcoloContributivo.MontanteEsclusivoQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteEsclusivoQuotaDL214.Value : 0M;
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoGAS = new List<Data.CMSGTRA.Ago.GAS> { agoGAS };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoGAS[0].YGATIPOR));
        }

        public static void ValorizzaAgoFS(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, Object objectFondoXX,
            bool isNuovaGestione, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaAgoFS = new List<Data.CMSGTRA.Ago.FS>();
                List<GestioneFondo.DatiFondoFST> lstDatiFondoFST = objectFondoXX as List<GestioneFondo.DatiFondoFST>;

                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
                GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

                List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = null;
                GestioneCalcolo.GetDatiServizioUtile707ByIdPensione(datiPensione.Id, out listaDatiServizioUtile707);

                byte record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    Data.CMSGTRA.Ago.FS agoFS = new Data.CMSGTRA.Ago.FS();
                    long idRecordFondo = recordFondo.Id;
                    record++;
                    agoFS.YFSFONDO = "FS";
                    agoFS.YFSTIPRC = "Y";
                    agoFS.YFSPROGR = record;

                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0 ? listaDatiCalcoloContributivo.FirstOrDefault(x => x.IdRecordFondo == idRecordFondo) : null;

                    GestioneFondo.DatiFondoFST datiFondoFST = null;
                    if (isNuovaGestione)
                        datiFondoFST = lstDatiFondoFST.Find(x => x.IdRecordFondo == idRecordFondo);
                    else
                        datiFondoFST = lstDatiFondoFST.FirstOrDefault();

                    if (datiPensione.SiglaCategoria.StartsWith("S"))
                    {
                        agoFS.YFSDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                        agoFS.YFSDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    }
                    else
                    {
                        agoFS.YFSDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0')) : (short)0;
                        agoFS.YFSDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    }

                    if (datiFondo != null)
                    {
                        agoFS.YFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                    }


                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                            !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                    {
                        if (datiFondoFST != null && datiFondoFST.PensioneAnnuaLorda214.HasValue && datiFondoFST.PensioneAnnuaLorda707.HasValue)
                        {
                            if (Decimal.Compare(datiFondoFST.PensioneAnnuaLorda707.Value, datiFondoFST.PensioneAnnuaLorda214.Value) < 0)
                                agoFS.YFSTIPCALC = "A";
                        }

                    }
                    else if (datiIstruttoria != null)
                    {
                        agoFS.YFSTIPCALC = datiIstruttoria.TipoCalcoloVincenteUnicarpe.HasValue ? datiIstruttoria.TipoCalcoloVincenteUnicarpe.ToString() : string.Empty;
                    }

                    if (datiCalcoloContributivo != null)
                    {
                        agoFS.YFSCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                        agoFS.YFSMONTA = datiCalcoloContributivo.Montante.HasValue ? Math.Round(datiCalcoloContributivo.Montante.Value, 4) : 0M;
                        agoFS.YFSQUOTAC = datiCalcoloContributivo.MontanteContributivo.HasValue ? datiCalcoloContributivo.MontanteContributivo.Value : 0M;
                        agoFS.YFSSETTC = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;

                        agoFS.YFSQUOTA2012 = datiCalcoloContributivo.QuotaContributivaAnnua.HasValue ? datiCalcoloContributivo.QuotaContributivaAnnua.Value : 0M;
                        agoFS.YFSCONTR2012 = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                        agoFS.YFSMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                        agoFS.YFSSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
                    }

                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                    {
                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = listaDatiServizioUtile.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = listaDatiServizioUtile;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoFS.YFSQUOTA92 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoFS.YFSQUOTA94 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoFS.YFSQUOTA95 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoFS.YFSQUOTA97 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoFS.YFSQUOTACE = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }


                    if (datiFondoFST != null)
                    {
                        agoFS.YFSCOEFTRA = datiFondoFST.CoefficienteTrasformazione.HasValue ? datiFondoFST.CoefficienteTrasformazione.Value : 0M;
                        agoFS.YFSPAL707 = datiFondoFST.PensioneAnnuaLorda707.HasValue ? datiFondoFST.PensioneAnnuaLorda707.Value : 0M;
                        agoFS.YFSPAL214 = datiFondoFST.PensioneAnnuaLorda214.HasValue ? datiFondoFST.PensioneAnnuaLorda214.Value : 0M;
                    }

                    if (listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                    {
                        List<GestioneCalcolo.ServizioUtile707> datiServizioUtile707 = null;
                        if (isNuovaGestione)
                            datiServizioUtile707 = listaDatiServizioUtile707.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile707 = listaDatiServizioUtile707;

                        if (datiServizioUtile707 != null && datiServizioUtile707.Count > 0)
                        {
                            foreach (GestioneCalcolo.ServizioUtile707 sU in datiServizioUtile707)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoFS.YFSSU92_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoFS.YFSQUOTA92_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoFS.YFSSU94_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoFS.YFSQUOTA94_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoFS.YFSSU95_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoFS.YFSQUOTA95_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoFS.YFSSU97_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoFS.YFSQUOTA97_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoFS.YFSSUCE_707 = (short)Math.Round((sU.ServizioUtileCessazioneAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileCessazioneMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileCessazioneGG.GetValueOrDefault() / 6.923));
                                            agoFS.YFSQUOTACE_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    AreaCalcolo.AreaInputVariabile.ListaAgoFS.Add(agoFS);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoFS[0].YFSTIPRC));
                }
            }
        }

        public static void ValorizzaAgoPT(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile, Object objectFondoXX, bool isNuovaGestione, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaAgoPT = new List<Data.CMSGTRA.Ago.PT>();
                List<GestioneFondo.DatiFondoPT> lstDatiFondoPT = objectFondoXX as List<GestioneFondo.DatiFondoPT>;

                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
                GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

                List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = null;
                GestioneCalcolo.GetDatiServizioUtile707ByIdPensione(datiPensione.Id, out listaDatiServizioUtile707);

                byte record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    Data.CMSGTRA.Ago.PT agoPT = new Data.CMSGTRA.Ago.PT();
                    long idRecordFondo = recordFondo.Id;
                    record++;
                    agoPT.YFSFONDO = "PT";
                    agoPT.YFSTIPRC = "Y";
                    agoPT.YFSPROGR = record;

                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0 ? listaDatiCalcoloContributivo.FirstOrDefault(x => x.IdRecordFondo == idRecordFondo) : null;


                    GestioneFondo.DatiFondoPT datiFondoPT = null;
                    if (isNuovaGestione)
                        datiFondoPT = lstDatiFondoPT.Find(x => x.IdRecordFondo == idRecordFondo);
                    else
                        datiFondoPT = lstDatiFondoPT.FirstOrDefault();

                    if (datiPensione.SiglaCategoria.StartsWith("S"))
                    {
                        agoPT.YFSDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                        agoPT.YFSDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    }
                    else
                    {
                        agoPT.YFSDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0')) : (short)0;
                        agoPT.YFSDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    }

                    if (datiFondo != null)
                    {
                        agoPT.YFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                    }


                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                           !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                    {
                        if (datiFondoPT != null && datiFondoPT.PensioneAnnuaLorda214.HasValue && datiFondoPT.PensioneAnnuaLorda707.HasValue)
                        {
                            if (Decimal.Compare(datiFondoPT.PensioneAnnuaLorda707.Value, datiFondoPT.PensioneAnnuaLorda214.Value) < 0)
                                agoPT.YFSTIPCALC = "A";
                        }

                    }
                    else if (datiIstruttoria != null)
                    {
                        agoPT.YFSTIPCALC = datiIstruttoria.TipoCalcoloVincenteUnicarpe.HasValue ? datiIstruttoria.TipoCalcoloVincenteUnicarpe.ToString() : string.Empty;
                    }

                    if (datiCalcoloContributivo != null)
                    {
                        agoPT.YFSCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                        agoPT.YFSMONTA = datiCalcoloContributivo.Montante.HasValue ? Math.Round(datiCalcoloContributivo.Montante.Value, 4) : 0M;
                        agoPT.YFSQUOTAC = datiCalcoloContributivo.MontanteContributivo.HasValue ? datiCalcoloContributivo.MontanteContributivo.Value : 0M;
                        agoPT.YFSSETTC = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;

                        agoPT.YFSQUOTA2012 = datiCalcoloContributivo.QuotaContributivaAnnua.HasValue ? datiCalcoloContributivo.QuotaContributivaAnnua.Value : 0M;
                        agoPT.YFSCONTR2012 = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                        agoPT.YFSMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                        agoPT.YFSSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
                    }

                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                    {
                        List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null;
                        if (isNuovaGestione)
                            datiServizioUtile = listaDatiServizioUtile.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile = listaDatiServizioUtile;

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtile.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoPT.YFSQUOTA92 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoPT.YFSQUOTA94 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoPT.YFSQUOTA95 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoPT.YFSQUOTA97 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoPT.YFSQUOTACE = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (datiFondoPT != null)
                    {
                        agoPT.YFSCOEFTRA = datiFondoPT.CoefficienteTrasformazione.HasValue ? datiFondoPT.CoefficienteTrasformazione.Value : 0M;
                        agoPT.YFSPAL707 = datiFondoPT.PensioneAnnuaLorda707.HasValue ? datiFondoPT.PensioneAnnuaLorda707.Value : 0M;
                        agoPT.YFSPAL214 = datiFondoPT.PensioneAnnuaLorda214.HasValue ? datiFondoPT.PensioneAnnuaLorda214.Value : 0M;
                    }

                    if (listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                    {
                        List<GestioneCalcolo.ServizioUtile707> datiServizioUtile707 = null;
                        if (isNuovaGestione)
                            datiServizioUtile707 = listaDatiServizioUtile707.FindAll(x => x.IdRecordFondo == idRecordFondo);
                        else
                            datiServizioUtile707 = listaDatiServizioUtile707;

                        if (datiServizioUtile707 != null && datiServizioUtile707.Count > 0)
                        {
                            foreach (GestioneCalcolo.ServizioUtile707 sU in datiServizioUtile707)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoPT.YFSSU92_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoPT.YFSQUOTA92_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoPT.YFSSU94_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoPT.YFSQUOTA94_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoPT.YFSSU95_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoPT.YFSQUOTA95_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoPT.YFSSU97_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoPT.YFSQUOTA97_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoPT.YFSSUCE_707 = (short)Math.Round((sU.ServizioUtileCessazioneAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileCessazioneMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileCessazioneGG.GetValueOrDefault() / 6.923));
                                            agoPT.YFSQUOTACE_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    AreaCalcolo.AreaInputVariabile.ListaAgoPT.Add(agoPT);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoPT[0].YFSTIPRC));
                }
            }
        }

        public static void ValorizzaAgoDZ(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
                GestioneCalcolo.GetCalcoloRetributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiRetributivi);

                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = null;
                GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiContributivi);

                AreaCalcolo.AreaInputVariabile.ListaAgoDZ = new List<INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA.Ago.DZ>();
                byte record = 0;

                if (listaDatiContributivi == null && listaDatiRetributivi == null)
                {
                    return;
                }

                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {

                    Data.CMSGTRA.Ago.DZ agoDZ = new Data.CMSGTRA.Ago.DZ();
                    long idRecordFondo = recordFondo.Id;
                    record++;
                    agoDZ.YDZFONDO = "DZ";
                    agoDZ.YDZTIPOR = "Y";
                    agoDZ.YDZPROGR = record;

                    if (listaRecordFondo != null && listaRecordFondo.Count > 0)
                    {
                        agoDZ.YDZDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? short.Parse(recordFondo.DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
                        agoDZ.YDZDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                        agoDZ.YDZDECSS = recordFondo.DecorrenzaValiditaDati.HasValue ? short.Parse(recordFondo.DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
                    }
                    else
                    {
                        agoDZ.YDZDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
                        agoDZ.YDZDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                        agoDZ.YDZDECSS = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
                    }

                    if (datiFondo != null)
                    {
                        agoDZ.YDZFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                        agoDZ.YDZPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;
                        agoDZ.YDZSETA_707 = datiFondo.QuotaA707.HasValue ? datiFondo.QuotaA707.Value : 0;
                        agoDZ.YDZSETB_707 = datiFondo.QuotaB707.HasValue ? datiFondo.QuotaB707.Value : 0;
                        agoDZ.YDZTIPEN = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                    }

                    GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = listaDatiRetributivi != null && listaDatiRetributivi.Count > 0 ? listaDatiRetributivi.FirstOrDefault(x => x.IdRecordFondo == idRecordFondo) : null;

                    if (datiCalcoloRetributivo != null)
                    {
                        agoDZ.YDZRSETA = datiCalcoloRetributivo.RMSQuotaA.HasValue ? datiCalcoloRetributivo.RMSQuotaA.Value : 0M;
                        agoDZ.YDZSETTA = datiCalcoloRetributivo.NSettimaneQuotaA.HasValue ? datiCalcoloRetributivo.NSettimaneQuotaA.Value : 0;
                        agoDZ.YDZRSETB = datiCalcoloRetributivo.RMSQuotaB.HasValue ? datiCalcoloRetributivo.RMSQuotaB.Value : 0M;
                        agoDZ.YDZSETTB = datiCalcoloRetributivo.NSettimaneQuotaB.HasValue ? datiCalcoloRetributivo.NSettimaneQuotaB.Value : 0;
                    }

                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                    GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(recordFondo.Id, out datiCalcoloContributivo);
                    if (datiCalcoloContributivo != null)
                    {
                        agoDZ.YDZMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                        agoDZ.YDZSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                        agoDZ.YDZIMPCRT = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                    }

                    if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoDZ != null)
                    {
                        GestioneFondo.DatiFondoDZ datiFondoDZ = objectFondoXX as GestioneFondo.DatiFondoDZ;
                        agoDZ.YDZSCAAA = datiFondoDZ.Sospensione.HasValue ? short.Parse(datiFondoDZ.Sospensione.Value.Year.ToString().PadLeft(4, '0').Substring(2, 2)) : (short)0;
                        agoDZ.YDZSCAMM = datiFondoDZ.Sospensione.HasValue ? (short)datiFondoDZ.Sospensione.Value.Month : (short)0;
                        agoDZ.YDZSCASS = datiFondoDZ.Sospensione.HasValue ? short.Parse(datiFondoDZ.Sospensione.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
                    }

                    AreaCalcolo.AreaInputVariabile.ListaAgoDZ.Add(agoDZ);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoDZ[0].YDZTIPOR));
                }
            }
        }

        public static void ValorizzaAgoES(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            Data.CMSGTRA.Ago.ES agoES = new Data.CMSGTRA.Ago.ES();
            agoES.YESFONDO = "ES";
            agoES.YESTIPOR = "Y";
            agoES.YESPROGR = 1;

            if (datiFondo != null)
            {
                agoES.YESREQU1 = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : string.Empty;
                agoES.YESREQU2 = datiFondo.CodiceRequisiti2.HasValue ? short.Parse(datiFondo.CodiceRequisiti2.Value.ToString()) : (short)0;
                agoES.YESFLAG214 = datiFondo.RiduzioneRetributiva ? "S" : "N";
                agoES.YESPERC214 = datiFondo.RiduzioneRetributivaPercentuale.HasValue ? datiFondo.RiduzioneRetributivaPercentuale.Value : 0M;
                if (datiFondo.TipoPensione.HasValue)
                {
                    short tipoPensione = 0;
                    short.TryParse(datiFondo.TipoPensione.Value.ToString(), out tipoPensione);
                    agoES.YESTPENS = tipoPensione;
                }
            }

            GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out calcoloRetributivo);

            if (calcoloRetributivo != null)
            {
                agoES.YESANZT2 = calcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)calcoloRetributivo.NSettimaneQuotaB.Value : (short)0;
                agoES.YESANZTO = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoES.YESRE2PN = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                agoES.YESRETPN = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
                agoES.YESVOLON = calcoloRetributivo.NSettAnzianitaVV.HasValue ? calcoloRetributivo.NSettAnzianitaVV.Value : 0;
            }

            GestioneCalcolo.DatiCalcoloContributivo calcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out calcoloContributivo);

            if (calcoloContributivo != null)
            {
                agoES.YESESCLU = calcoloContributivo.MontanteEsclusivo.HasValue ? calcoloContributivo.MontanteEsclusivo.Value : 0M;
                agoES.YESMONTA = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                agoES.YESMONTA2012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? (int)calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoES.YESSETT2012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? calcoloContributivo.NSettimaneQuotaDL214.Value : 0;
                agoES.YESSETTE = calcoloContributivo.NSettimane.HasValue ? (short)calcoloContributivo.NSettimane.Value : (short)0;
                agoES.YESTOTRT = calcoloContributivo.ImportoContributivoTotale.HasValue ? calcoloContributivo.ImportoContributivoTotale.Value : 0M;
                agoES.YESIMPCRT = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? (int)calcoloContributivo.ImportoContribTotaleQuotaDL214.Value / 10 : 0;
            }

            if (objectFondoXX != null && objectFondoXX as GestioneFondo.DatiFondoES != null)
            {
                GestioneFondo.DatiFondoES datiFondoES = objectFondoXX as GestioneFondo.DatiFondoES;
                agoES.YESART11 = datiFondoES.IntegrazioneArticolo11.HasValue ? datiFondoES.IntegrazioneArticolo11.Value : 0M;
                agoES.YESBALTR = datiFondoES.BaseAltraPensione.HasValue ? datiFondoES.BaseAltraPensione.Value : 0M;
                agoES.YESCALTR = datiFondoES.CategoriaAltraPensione.HasValue ? datiFondoES.CategoriaAltraPensione.Value.ToString().PadLeft(3, '0') : string.Empty;
                agoES.YESCTR24 = datiFondoES.ImportoContributiLegge37758Art24.HasValue ? datiFondoES.ImportoContributiLegge37758Art24.Value : 0M;
                agoES.YESCTR57 = datiFondoES.ImportoContributiLegge37758Art57.HasValue ? datiFondoES.ImportoContributiLegge37758Art57.Value : 0M;
                if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
                {
                    agoES.YESDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                    agoES.YESDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
                    agoES.YESDECSS = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? short.Parse(listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year.ToString().PadLeft(4, '0').Substring(0, 2)) : (short)0;
                }
                else
                {
                    agoES.YESDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    agoES.YESDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    agoES.YESDECSS = datiPensione.DecorrenzaOriginaria.HasValue ? (short)(datiPensione.DecorrenzaOriginaria.Value.Year / 100) : (short)0;
                }
                agoES.YESDIFFA = datiFondoES.AnniDifferimento.HasValue ? (short)datiFondoES.AnniDifferimento.Value : (short)0;
                agoES.YESMATUR = datiFondoES.EtaMaturazioneRequisiti.HasValue ? datiFondoES.EtaMaturazioneRequisiti.Value : (short)0;
                agoES.YESSA224 = datiFondoES.SettimaneArt24QB.HasValue ? (short)datiFondoES.SettimaneArt24QB.Value : (short)0;
                agoES.YESSAR24 = datiFondoES.SettimaneArt24QA.HasValue ? datiFondoES.SettimaneArt24QA.Value : 0;
                agoES.YESSAR57 = datiFondoES.NSettimaneLegge37758Art57.HasValue ? datiFondoES.NSettimaneLegge37758Art57.Value : 0;
                agoES.YESSOSAA = datiFondoES.Sospensione.HasValue ? (short)datiFondoES.Sospensione.Value.Year : (short)0;
                agoES.YESSOSMM = datiFondoES.Sospensione.HasValue ? (short)datiFondoES.Sospensione.Value.Month : (short)0;
                agoES.YESSOSSS = datiFondoES.Sospensione.HasValue ? (short)(datiFondoES.Sospensione.Value.Year / 100) : (short)0;
                agoES.YESSPECI = datiFondoES.CodiceSpecificoAgo.HasValue ? datiFondoES.CodiceSpecificoAgo.Value.ToString() : string.Empty;
                agoES.YESSUP14 = datiFondoES.ImportoContributiLegge143271Art14.HasValue ? datiFondoES.ImportoContributiLegge143271Art14.Value : 0M;
                agoES.YESTEOAA = datiFondoES.DecorrenzaTeorica.HasValue ? (short)datiFondoES.DecorrenzaTeorica.Value.Year : (short)0;
                agoES.YESTEOMM = datiFondoES.DecorrenzaTeorica.HasValue ? (short)datiFondoES.DecorrenzaTeorica.Value.Month : (short)0;
                agoES.YESTPLIQ = datiFondoES.CodiceTipoLiquidazione.HasValue ? datiFondoES.CodiceTipoLiquidazione.Value : (short)0;
                agoES.YESDIFFQ = datiFondoES.ContributiDifferimentoQuota.HasValue ? datiFondoES.ContributiDifferimentoQuota.Value : 0M;

                agoES.YESCDCAA = datiFondoES.DecDPCM.HasValue ? (short)(datiFondoES.DecDPCM.Value.Year) : (short)0;
                agoES.YESCDCMM = datiFondoES.DecDPCM.HasValue ? (short)(datiFondoES.DecDPCM.Value.Month) : (short)0;
                agoES.YESDPCRT = datiFondoES.RmsDPCM.HasValue ? datiFondoES.RmsDPCM.Value : 0;
                agoES.YESS72RT = datiFondoES.RMSSent72.HasValue ? datiFondoES.RMSSent72.Value : 0;

                //Dati S.L 336
                agoES.YESZA14C = datiFondoES.CCArt14SenzaLegge33670 ?? 0;
                agoES.YESZANZI = datiFondoES.NSettimaneAnzianitaTotaliSenzaLegge33670 ?? 0;
                agoES.YESZRET2 = datiFondoES.RMSSenzaLegge33670QB ?? 0;
                agoES.YESZRETS = datiFondoES.RMSSenzaLegge33670QA ?? 0;
                agoES.YESZSPAG = datiFondoES.ContributiSupplementoAgo ?? 0;
                agoES.YESZSPFO = datiFondoES.ContributiSupplementoFondo ?? 0;
                agoES.YESZST24 = datiFondoES.NSettimaneSenzaLegge33670Art24QuotaA ?? 0;
                agoES.YESZST57 = datiFondoES.NSettimaneSenzaLegge33670Art57QuotaA ?? 0;
                agoES.YESZTOTC = datiFondoES.ContributiTotaliSenzaLegge33670 ?? 0;
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoES = new List<Data.CMSGTRA.Ago.ES> { agoES };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoES[0].YESTIPOR));
        }

        public static void ValorizzaAgoPM(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
            ref Data.FSPL_FSRC AreaCalcolo)
        {
            Data.CMSGTRA.Ago.PM agoPM = new Data.CMSGTRA.Ago.PM();
            agoPM.YPMFONDO = "PM";
            agoPM.YPMTIPOR = "Y";
            agoPM.YPMPROGR = 1;

            if (datiPensione.SiglaCategoria.StartsWith("S") && listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                agoPM.YPMDECAA = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Year : (short)0;
                agoPM.YPMDECMM = listaRecordFondo[0].DecorrenzaValiditaDati.HasValue ? (short)listaRecordFondo[0].DecorrenzaValiditaDati.Value.Month : (short)0;
            }
            else
            {
                agoPM.YPMDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                agoPM.YPMDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
            }

            GestioneCalcolo.DatiCalcoloRetributivo calcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out calcoloRetributivo);

            if (calcoloRetributivo != null)
            {
                agoPM.YPMANZE1 = calcoloRetributivo.NSettimaneEsclusiveQuotaB.HasValue ? (short)calcoloRetributivo.NSettimaneEsclusiveQuotaB.Value : (short)0;
                agoPM.YPMANZES = calcoloRetributivo.NSettimaneEsclusiveQuotaA.HasValue ? calcoloRetributivo.NSettimaneEsclusiveQuotaA.Value : 0;
                agoPM.YPMANZT1 = calcoloRetributivo.NSettimaneQuotaB.HasValue ? (short)calcoloRetributivo.NSettimaneQuotaB.Value : (short)0;
                agoPM.YPMANZTO = calcoloRetributivo.NSettimaneQuotaA.HasValue ? calcoloRetributivo.NSettimaneQuotaA.Value : 0;
                agoPM.YPMRETP1 = calcoloRetributivo.RMSQuotaB.HasValue ? calcoloRetributivo.RMSQuotaB.Value : 0M;
                agoPM.YPMRETPN = calcoloRetributivo.RMSQuotaA.HasValue ? calcoloRetributivo.RMSQuotaA.Value : 0M;
            }

            GestioneCalcolo.DatiCalcoloContributivo calcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out calcoloContributivo);

            if (calcoloContributivo != null)
            {
                agoPM.YPMIMPCRT = calcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? calcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                agoPM.YPMMONTA = calcoloContributivo.Montante.HasValue ? calcoloContributivo.Montante.Value : 0M;
                agoPM.YPMMONTA2012 = calcoloContributivo.MontanteQuotaDL214.HasValue ? calcoloContributivo.MontanteQuotaDL214.Value : 0M;
                agoPM.YPMSETT2012 = calcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)calcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
                agoPM.YPMSETTE = calcoloContributivo.NSettimane.HasValue ? (short)calcoloContributivo.NSettimane.Value : (short)0;
            }

            GestioneFondo.DatiFondoPM datiFondoPM = null;
            GestioneFondo.GetFondoPMByIdPensione(datiPensione.Id, out datiFondoPM);

            if (datiFondoPM != null)
            {
                agoPM.YPMTIPLQ = datiFondoPM.CodiceTipoLiquidazione.HasValue ? datiFondoPM.CodiceTipoLiquidazione.Value : (short)0;
                agoPM.YPMTPCOD = datiFondoPM.CL413.HasValue ? datiFondoPM.CL413.Value.ToString() : string.Empty;
            }

            if (datiFondo != null)
            {
                short resShort = 0;
                if (datiFondo.CodiceRequisiti2.HasValue)
                {
                    short.TryParse(datiFondo.CodiceRequisiti2.Value.ToString(), out resShort);
                    agoPM.YPM503AS = resShort;
                }
                agoPM.YPM503ET = datiFondo.CodiceRequisiti1.HasValue ? datiFondo.CodiceRequisiti1.Value.ToString() : string.Empty;
                agoPM.YPMTIPEN = GetTipoPensione(datiPensione, datiFondo.TipoPensione);

                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codiceSpecifico = elencoCodiceSpecifico.Find(x => x.Id == (datiFondo.CodiceSpecifico.HasValue ? datiFondo.CodiceSpecifico.Value : 0));
                    if (codiceSpecifico != null)
                        agoPM.YPMSPECI = codiceSpecifico.TraduzioneGp.HasValue ? codiceSpecifico.TraduzioneGp.Value.ToString() : string.Empty;
                }

                agoPM.YPMSETA_707 = datiFondo.QuotaA707.GetValueOrDefault();
                agoPM.YPMSETB_707 = datiFondo.QuotaB707.GetValueOrDefault();
            }

            AreaCalcolo.AreaInputVariabile.ListaAgoPM = new List<Data.CMSGTRA.Ago.PM> { agoPM };
            AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoPM[0].YPMTIPOR));
        }

        public static void ValorizzaAgoINPDAP(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo,
           List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtile, Object objectFondoXX, ref Data.FSPL_FSRC AreaCalcolo)
        {
            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                AreaCalcolo.AreaInputVariabile.ListaAgoGDP = new List<Data.CMSGTRA.Ago.GDP>();
                List<GestionePensioneINPDAP.DatiPensioneINPDAP> lstDatiFondoINPDAP = objectFondoXX as List<GestionePensioneINPDAP.DatiPensioneINPDAP>;

                List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);

                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
                GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

                List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtile707 = null;
                GestioneCalcolo.GetDatiServizioUtileINPDAP707ByIdPensione(datiPensione.Id, out listaDatiServizioUtile707);

                byte record = 0;
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo)
                {
                    Data.CMSGTRA.Ago.GDP agoGDP = new Data.CMSGTRA.Ago.GDP();
                    long idRecordFondo = recordFondo.Id;
                    record++;
                    agoGDP.YFSFONDO = "GDP";
                    agoGDP.YFSTIPRC = "Y";
                    agoGDP.YFSPROGR = record;

                    GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                    datiCalcoloContributivo = listaDatiCalcoloContributivo != null ? listaDatiCalcoloContributivo.Where(x => x.IdRecordFondo == idRecordFondo).FirstOrDefault() : null;

                    //if (datiPensione.SiglaCategoria.StartsWith("S"))
                    //{
                        agoGDP.YFSDECAA = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Year : (short)0;
                        agoGDP.YFSDECMM = recordFondo.DecorrenzaValiditaDati.HasValue ? (short)recordFondo.DecorrenzaValiditaDati.Value.Month : (short)0;
                    //}
                    //else
                    //{
                    //    agoGDP.YFSDECAA = datiPensione.DecorrenzaOriginaria.HasValue ? short.Parse(datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0')) : (short)0;
                    //    agoGDP.YFSDECMM = datiPensione.DecorrenzaOriginaria.HasValue ? (short)datiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    //}

                    if (datiFondo != null)
                    {
                        agoGDP.YFSTPENS = GetTipoPensione(datiPensione, datiFondo.TipoPensione);
                    }

                    if (datiIstruttoria != null)
                    {
                        agoGDP.YFSTIPCALC = datiIstruttoria.TipoCalcoloVincenteUnicarpe.HasValue ? datiIstruttoria.TipoCalcoloVincenteUnicarpe.ToString() : string.Empty;
                    }

                    if (datiCalcoloContributivo != null)
                    {
                        agoGDP.YFSCONTR = datiCalcoloContributivo.ImportoContributivoTotale.HasValue ? datiCalcoloContributivo.ImportoContributivoTotale.Value : 0M;
                        agoGDP.YFSMONTA = datiCalcoloContributivo.Montante.HasValue ? Math.Round(datiCalcoloContributivo.Montante.Value, 4) : 0M;

                        agoGDP.YFSQUOTAC = datiCalcoloContributivo.MontanteContributivo.HasValue ? datiCalcoloContributivo.MontanteContributivo.Value : 0M;
                        agoGDP.YFSSETTC = datiCalcoloContributivo.NSettimane.HasValue ? (short)datiCalcoloContributivo.NSettimane.Value : (short)0;

                        agoGDP.YFSQUOTA2012 = datiCalcoloContributivo.QuotaContributivaAnnua.HasValue ? datiCalcoloContributivo.QuotaContributivaAnnua.Value : 0M;
                        agoGDP.YFSCONTR2012 = datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcoloContributivo.ImportoContribTotaleQuotaDL214.Value : 0M;
                        agoGDP.YFSMONTA2012 = datiCalcoloContributivo.MontanteQuotaDL214.HasValue ? datiCalcoloContributivo.MontanteQuotaDL214.Value : 0M;
                        agoGDP.YFSSETT2012 = datiCalcoloContributivo.NSettimaneQuotaDL214.HasValue ? (short)datiCalcoloContributivo.NSettimaneQuotaDL214.Value : (short)0;
                    }

                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                    {
                        List<GestioneDatiServizioUtileINPDAP.ServizioUtile> datiServizioUtile = null;
                        datiServizioUtile = listaDatiServizioUtile.FindAll(x => x.IdRecordFondo == idRecordFondo);

                        if (datiServizioUtile != null && datiServizioUtile.Count > 0)
                        {
                            foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile sU in datiServizioUtile)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoGDP.YFSQUOTA92 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoGDP.YFSQUOTA94 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoGDP.YFSQUOTA95 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoGDP.YFSQUOTA97 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoGDP.YFSQUOTACE = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiFondoINPDAP = null;
                    datiFondoINPDAP = listaRecordDatiFondoINPDAP.Find(x => x.IdRecordFondo == idRecordFondo);

                    if (datiFondoINPDAP != null)
                    {
                        agoGDP.YFSCOEFTRA = datiFondoINPDAP.CoefficienteTrasformazione.HasValue ? datiFondoINPDAP.CoefficienteTrasformazione.Value : 0M;
                        agoGDP.YFSPAL707 = datiFondoINPDAP.PensioneAnnuaLorda707.HasValue ? datiFondoINPDAP.PensioneAnnuaLorda707.Value : 0M;
                    }

                    if (listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count > 0)
                    {
                        List<GestioneCalcolo.ServizioUtileINPDAP707> datiServizioUtile707 = null;
                        datiServizioUtile707 = listaDatiServizioUtile707.FindAll(x => x.IdRecordFondo == idRecordFondo);

                        if (datiServizioUtile707 != null && datiServizioUtile707.Count > 0)
                        {
                            foreach (GestioneCalcolo.ServizioUtileINPDAP707 sU in datiServizioUtile707)
                            {
                                if (!string.IsNullOrEmpty(sU.Quota))
                                {
                                    switch (sU.Quota.Trim().ToUpperInvariant())
                                    {
                                        case "A":
                                            agoGDP.YFSSU92_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoGDP.YFSQUOTA92_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B1":
                                            agoGDP.YFSSU94_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoGDP.YFSQUOTA94_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B2":
                                            agoGDP.YFSSU95_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoGDP.YFSQUOTA95_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B3":
                                            agoGDP.YFSSU97_707 = (short)Math.Round((sU.ServizioUtileAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileGG.GetValueOrDefault() / 6.923));
                                            agoGDP.YFSQUOTA97_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                        case "B4":
                                            agoGDP.YFSSUCE_707 = (short)Math.Round((sU.ServizioUtileCessazioneAA.GetValueOrDefault() * 52) +
                                                (sU.ServizioUtileCessazioneMM.GetValueOrDefault() * 4.333) +
                                                (sU.ServizioUtileCessazioneGG.GetValueOrDefault() / 6.923));
                                            agoGDP.YFSQUOTACE_707 = sU.QuotaPensioneRetributivaAnnua.HasValue ? sU.QuotaPensioneRetributivaAnnua.Value : 0M;
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    GestionePensioneINPDAP.DatiPensioneINPDAP datiINPDAP = lstDatiFondoINPDAP.Find(x => x.IdRecordFondo == idRecordFondo);
                    if (datiINPDAP != null)
                    {
                        if (datiINPDAP.DataRivalsaINPDAP.HasValue)
                            agoGDP.YFSDATA_RIVALSA = int.Parse(datiINPDAP.DataRivalsaINPDAP.Value.Year.ToString().PadLeft(4, '0') +
                                datiINPDAP.DataRivalsaINPDAP.Value.Month.ToString().PadLeft(2, '0') +
                                datiINPDAP.DataRivalsaINPDAP.Value.Day.ToString().PadLeft(2, '0'));

                        if (datiINPDAP.Comparto.GetValueOrDefault() > 0)
                            agoGDP.YFSCOMPARTO = datiINPDAP.Comparto.ToString().PadLeft(2, '0');

                        if (datiINPDAP.Settore.GetValueOrDefault() > 0)
                            agoGDP.YFSSETTORE = datiINPDAP.Settore.ToString().PadLeft(3, '0');

                        if (datiINPDAP.Ruolo.GetValueOrDefault() > 0)
                            agoGDP.YFSRUOLO = datiINPDAP.Ruolo.ToString().PadLeft(4, '0');
                    }

                    AreaCalcolo.AreaInputVariabile.ListaAgoGDP.Add(agoGDP);
                    AreaCalcolo.Request.LISTBLOCCO.Add(new Data.HostRequest.FSPL_FSRCRequest.BLOCCO(AreaCalcolo.AreaInputVariabile.ListaAgoGDP[0].YFSTIPRC));
                }
            }
        }

        #endregion Ago
        #endregion public members

        #region private methods
        private static void GetCodiceProvinciaNascita(string provinciaNascita, out short codProvNascita)
        {
            codProvNascita = 0;
            string query = (from s in INPS.DNA.Context.OfficeList.Offices
                            where (s.Value.ExtendedProperties != null ? s.Value.ExtendedProperties["PR"].Trim() : s.Value.Province.Trim()) == provinciaNascita.Trim()
                            select s.Value.SSCode).FirstOrDefault<string>();
            short.TryParse(query, out codProvNascita);
        }

        private static void GetASSAC(int? PrivilegiataSuperinvaliditaIndennita, int? AssegnoIntegrativo,
            int? IntegrazioneIndennitaAssistenza, int? IndennitaAccompagnamentoAggiuntiva,
            int? CumuloInfermita, int? Categoria2aInfermita, int? AssegnoCura,
            int? IndennitaSpecialeAnnua, out int assac)
        {
            assac = 0;

            string strAssac = string.Empty;

            List<GestioneDecodifica.DecPensioniPrivilegiate> listaDecPensioniPrivilegiate = null;
            GestioneDecodifica.GetElencoPensioniPrivilegiate(out listaDecPensioniPrivilegiate);
            if (listaDecPensioniPrivilegiate != null && listaDecPensioniPrivilegiate.Count > 0)
            {
                GestioneDecodifica.DecPensioniPrivilegiate decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (PrivilegiataSuperinvaliditaIndennita.HasValue ? PrivilegiataSuperinvaliditaIndennita.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac = decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac = "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (AssegnoIntegrativo.HasValue ? AssegnoIntegrativo.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (IntegrazioneIndennitaAssistenza.HasValue ? IntegrazioneIndennitaAssistenza.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (IndennitaAccompagnamentoAggiuntiva.HasValue ? IndennitaAccompagnamentoAggiuntiva.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (CumuloInfermita.HasValue ? CumuloInfermita.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (Categoria2aInfermita.HasValue ? Categoria2aInfermita.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (AssegnoCura.HasValue ? AssegnoCura.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                decPensioniPrivilegiate = listaDecPensioniPrivilegiate.Find(x => x.Id == (IndennitaSpecialeAnnua.HasValue ? IndennitaSpecialeAnnua.Value : 0));
                if (decPensioniPrivilegiate != null && decPensioniPrivilegiate.TraduzioneSuGP.HasValue)
                    strAssac += decPensioniPrivilegiate.TraduzioneSuGP.Value.ToString();
                else
                    strAssac += "0";

                int.TryParse(strAssac, out assac);
            }
        }

        private static string GetCognomeNomeTagliato(string conom)
        {
            if (!string.IsNullOrEmpty(conom) && conom.Trim().Length > 32)
            {
                //try
                //{
                //    //taglio cognome
                //    return conom.Substring(0, conom.IndexOf('/') - (conom.Length - 32)) + conom.Substring(conom.IndexOf('/'));
                //}
                //catch (Exception)
                //{
                //    //taglio nome
                //    return conom.Substring(0, 32);
                //}

                //taglio nome
                return conom.Substring(0, 32);
            }
            else
                return conom;
        }

        private static short GetTipoPensione(GestionePensione.DatiPensione datiPensione, char? tipoPensione)
        {
            short ret = 0;
            if (!tipoPensione.HasValue)
            {
                try
                {
                    tipoPensione = GestioneLiquidazionePensione.GetTipoPensione(datiPensione).First().Value;
                }
                catch (Exception)
                {
                    tipoPensione = Utility.GeTipoPensioneByCodeProdotto(datiPensione.Prodotto);
                }
            }

            short.TryParse(tipoPensione.Value.ToString(), out ret);
            return ret;
        }
        #endregion private methods
    }
}

