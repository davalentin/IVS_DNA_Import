using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class MappingVersoHostNew
    {
        #region public members
        public static void ValorizzaRichiesta(string matricolaOperatore, short sedeOperatore, ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            DateTime dataSistema, int annoCompetenza, out Data.HostRequest.GAPL_GARCRequestNew richiesta, out string messaggioEccezione)
        {
            richiesta = new Data.HostRequest.GAPL_GARCRequestNew();
            messaggioEccezione = string.Empty;

            #region Intestazione
            Data.CAREPET.Intestazione intestazione = null;
            ValorizzaIntestazione(ref contenitore, annoCompetenza, out intestazione);
            richiesta.Intestazione = intestazione;
            #endregion Intestazione

            #region DatiGenerici
            Data.CAREPET.DatiGenericiNew datiGenerici = null;
            ValorizzaDatiGenerici(ref contenitore, matricolaOperatore, dataSistema, out datiGenerici);
            richiesta.DatiGenerici = datiGenerici;
            #endregion DatiGenerici

            #region Pensionato
            Data.CAREPET.Pensionato pensionato = null;
            ValorizzaPensionato(ref contenitore, out pensionato);
            richiesta.Pensionato = pensionato;
            #endregion Pensionato

            #region Istruttoria
            Data.CAREPET.Istruttoria istruttoria = null;
            ValorizzaIstruttoria(ref contenitore, ref contenitoreDecodifica, out istruttoria);
            richiesta.Istruttoria = istruttoria;
            #endregion Istruttoria

            #region Pagamento
            Data.CAREPET.Pagamento pagamento = null;
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            ValorizzaPagamento(ref contenitore, ref contenitoreDecodifica, tipoDomanda, out pagamento);
            richiesta.Pagamento = pagamento;
            #endregion Pagamento

            #region StatoCivile
            Data.CAREPET.StatoCivile statoCivile = null;
            ValorizzaStatoCivile(ref contenitore, out statoCivile);
            richiesta.StatoCivile = statoCivile;
            #endregion StatoCivile

            #region Sentenze
            Data.CAREPET.Sentenze sentenze = null;
            ValorizzaSentenze(ref contenitore, out sentenze);
            richiesta.Sentenze = sentenze;
            #endregion Sentenze

            #region INAIL_Accompagnamento
            Data.CAREPET.INAIL_Accompagnamento inail_Accompagnamento = null;
            DateTime? inail_CessazioneAssegnoAccompangamento = null;
            ValorizzaINAIL_Accompagnamento(ref contenitore, out inail_CessazioneAssegnoAccompangamento, out inail_Accompagnamento);
            richiesta.INAIL_Accompagnamento = inail_Accompagnamento;
            #endregion INAIL_Accompagnamento

            #region PensioniAbbinate
            Data.CAREPET.PensioniAbbinate pensioniAbbinate = null;
            ValorizzaPensioniAbbinate(out pensioniAbbinate);
            richiesta.PensioniAbbinate = pensioniAbbinate;
            #endregion PensioniAbbinate

            #region ResidenzeEstero
            Data.CAREPET.ResidenzeEstero residenzeEstero = null;
            ValorizzaResidenzeEstero(ref contenitore, out residenzeEstero);
            richiesta.ResidenzeEstero = residenzeEstero;
            #endregion ResidenzeEstero

            #region DanteCausa
            Data.CAREPET.DanteCausa danteCausa = null;
            ValorizzaDanteCausa(ref contenitore, ref contenitoreDecodifica, out danteCausa);
            richiesta.DanteCausa = danteCausa;
            #endregion DanteCausa

            #region DatiRetributivi_Contributivi_BIS
            Data.CAREPET.DatiRetributivi_Contributivi datiRetributivi_Contributivi = null;
            Data.CAREPET.DatiRetributiviBIS datiRetributiviBIS = null;
            ValorizzaDatiRetributivi_Contributivi_BIS(ref contenitore, ref contenitoreDecodifica, out datiRetributivi_Contributivi, out datiRetributiviBIS);
            richiesta.DatiRetributivi_Contributivi = datiRetributivi_Contributivi;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRetributiviAgoBis", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                richiesta.DatiRetributiviBIS = datiRetributiviBIS;
            }
            #endregion DatiRetributivi_Contributivi_BIS

            #region IntegrazioneArticolo11
            Data.CAREPET.IntegrazioneArticolo11 integrazioneArticolo11 = null;
            ValorizzaIntegrazioneArticolo11ByIdPensione(ref contenitore, out integrazioneArticolo11);
            richiesta.IntegrazioneArticolo11 = integrazioneArticolo11;
            #endregion IntegrazioneArticolo11

            #region PannelloContributivo
            Data.CAREPET.PannelloContributivo pannelloContributivo = null;
            ValorizzaPannelloContributivo(ref contenitore, ref contenitoreDecodifica, out pannelloContributivo);
            richiesta.PannelloContributivo = pannelloContributivo;
            #endregion PannelloContributivo

            #region Supplementi
            Data.CAREPET.Supplementi supplementi = null;
            ValorizzaSupplementi(ref contenitore, ref contenitoreDecodifica, out supplementi);
            richiesta.Supplementi = supplementi;
            #endregion Supplementi

            #region Bititolarieta
            Data.CAREPET.Bititolarieta bititolarieta = null;
            ValorizzaBititolarieta(ref contenitore, out bititolarieta);
            richiesta.Bititolarieta = bititolarieta;
            #endregion Bititolarieta

            #region Redditi
            Data.CAREPET.Redditi redditi = null;
            ValorizzaRedditi(ref contenitore, out redditi);
            richiesta.Redditi = redditi;
            #endregion Redditi

            #region Invciv
            Data.CAREPET.Invciv invciv = null;
            ValorizzaInvciv(ref contenitore, out invciv);
            richiesta.Invciv = invciv;
            #endregion Invciv

            #region Ricoveri
            Data.CAREPET.Ricoveri ricoveri = null;
            ValorizzaRicoveri(out ricoveri);
            richiesta.Ricoveri = ricoveri;
            #endregion Ricoveri

            #region Delegato
            Data.CAREPET.Delegato delegato = null;

            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !contenitore.IsRiaperturaDomanda)
            {
                ValorizzaDelegato(ref contenitore, out delegato);
                richiesta.Delegato = delegato;
            }
            #endregion Delegato

            #region Tutore
            Data.CAREPET.Tutore tutore = null;
            ValorizzaTutore(ref contenitore, out tutore);
            richiesta.Tutore = tutore;
            #endregion Tutore

            #region Familiari
            Data.CAREPET.Familiari familiari = null;
            ValorizzaFamiliari(ref contenitore, tipoDomanda, dataSistema, out familiari);
            richiesta.Familiari = familiari;
            #endregion Familiari

            #region Errori
            Data.CAREPET.Errori errori = null;
            ValorizzaErrori(out errori);
            richiesta.Errori = errori;
            #endregion Errori

            #region DatiNuovi
            Data.CAREPET.DatiNuovi datiNuovi = null;
            ValorizzaDatiNuovi(ref contenitore, ref contenitoreDecodifica, out datiNuovi);
            richiesta.DatiNuovi = datiNuovi;
            #endregion DatiNuovi

            #region Coda
            Data.CAREPET.Coda coda = null;
            ValorizzaCoda(ref contenitore, ref contenitoreDecodifica, tipoDomanda, inail_CessazioneAssegnoAccompangamento, out coda);
            richiesta.Coda = coda;
            #endregion Coda

            #region SPRDSC21
            Data.CAREPET.SPRDSC21New sprdsc21 = null;
            ValorizzaSPRDSC21(ref contenitore, datiGenerici, out sprdsc21, out messaggioEccezione);
            richiesta.SPRDSC21 = sprdsc21;
            #endregion SPRDSC21

            #region NuoviDati2024
            Data.CAREPET.NuoviDati2024 nuoviDati2024 = null;
            ValorizzaNuoviDati2024(ref contenitore, out nuoviDati2024);
            richiesta.NuoviDati2024 = nuoviDati2024;
            #endregion NuoviDati2024
        }

        public static void ValorizzaRichiesta(Data.GAPL_GARC_New AreaCalcolo, out Data.HostRequest.CopericonRequest richiesta)
        {
            string errori = string.Empty;
            long nDomus = AreaCalcolo.RequestNew.DatiGenerici.T_NDOMUS;
            richiesta = new Data.HostRequest.CopericonRequest();
            richiesta.TipoProcedura = 1;
            richiesta.DataPrelievo = DateTime.Now;
            richiesta.MatricolaOperatore = AreaCalcolo.RequestNew.DatiGenerici.T_TP1MATRICOLA;
            richiesta.CodCategoria = AreaCalcolo.RequestNew.DatiGenerici.T_GP1AB01_V;
            richiesta.CodSede = AreaCalcolo.RequestNew.DatiGenerici.T_GP1AB02_V.ToString().PadLeft(4, '0');
            richiesta.Certificato = AreaCalcolo.RequestNew.DatiGenerici.T_GP1AB03_V.ToString().PadLeft(8, '0');
            richiesta.CodBeneficiLegge2062004 = !string.IsNullOrEmpty(AreaCalcolo.RequestNew.Pagamento.T_GP1AC01_V) ? AreaCalcolo.RequestNew.Pagamento.T_GP1AC01_V.PadRight(3, ' ') : string.Empty.PadLeft(3, ' ');
            long resLong = 0;
            long.TryParse(AreaCalcolo.RequestNew.Pagamento.T_GP1AM01_V, out resLong);
            richiesta.CodEliminazione = resLong;
            if (richiesta.CodEliminazione > 0)
                richiesta.DataEliminazione = new DateTime(AreaCalcolo.RequestNew.Pagamento.T_GP1AM02A_V, AreaCalcolo.RequestNew.Pagamento.T_GP1AM02M_V, 1);

            ServiceReferences.DatiPensioni.DatiTGP1Response datiGP1 = null;
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(nDomus, richiesta.CodCategoria + richiesta.CodSede + richiesta.Certificato, out datiGP1, out errori);

            if (datiGP1 != null && datiGP1.ElementoDatiTGP1 != null)
            {
                if (datiGP1.ElementoDatiTGP1.GP1AZ50N != null)
                    richiesta.MeseEstrazioneRata = Utility.StringToNullableInt64(datiGP1.ElementoDatiTGP1.GP1AZ50N.Valore.Codice).GetValueOrDefault();
                if (datiGP1.ElementoDatiTGP1.GP1CIDEMIN != null)
                    richiesta.CodParticolareRinnovo = datiGP1.ElementoDatiTGP1.GP1CIDEMIN.Valore.Codice;
                if (datiGP1.ElementoDatiTGP1.GP1T11 != null && datiGP1.ElementoDatiTGP1.GP1T11.Count() > 0)
                {
                    if (datiGP1.ElementoDatiTGP1.GP1T11[0].GP1CMPNTIP != null)
                        richiesta.CodMovimentazione = datiGP1.ElementoDatiTGP1.GP1T11[0].GP1CMPNTIP.Valore.Codice;
                    if (datiGP1.ElementoDatiTGP1.GP1T11[0].GP1DMPN != null)
                        richiesta.DataMovimentazione = Utility.DataFromString(datiGP1.ElementoDatiTGP1.GP1T11[0].GP1DMPN.Valore.Codice, Utility.FormatoData.AAAAmmGG);
                }
            }

            ServiceReferences.DatiPensioni.DatiTGP5Response datiGP5 = null;
            GestioneDatiPensioni.GetDatiTGP5ByChiavePensione(nDomus, richiesta.CodCategoria + richiesta.CodSede + richiesta.Certificato, out datiGP5, out errori);

            if (datiGP5 != null && datiGP5.ListaDatiTGP5 != null && datiGP5.ListaDatiTGP5.Count() > 0)
            {
                if (datiGP5.ListaDatiTGP5[0].GP5HC01Z != null)
                {
                    string decorrenza = datiGP5.ListaDatiTGP5[0].GP5HC01Z.Valore.Codice.PadLeft(6, '0');
                    richiesta.AnnoDecorrenza = short.Parse(decorrenza.Substring(0, 4));
                    richiesta.MeseDecorrenza = short.Parse(decorrenza.Substring(4, 2));
                }
                if (datiGP5.ListaDatiTGP5[0].GP5HG00 != null && datiGP5.ListaDatiTGP5[0].GP5HG00.Count() > 0)
                {
                    if (datiGP5.ListaDatiTGP5[0].GP5HG00[0].GP5HG01 != null)
                        richiesta.CodFondo = Utility.StringToNullableDecimalPoint(datiGP5.ListaDatiTGP5[0].GP5HG00[0].GP5HG01.Valore.Codice).GetValueOrDefault();
                }
            }

            ServiceReferences.DatiPensioni.DatiTGP6Response datiGP6 = null;
            GestioneDatiPensioni.GetDatiTGP6ByChiavePensione(nDomus, richiesta.CodCategoria + richiesta.CodSede + richiesta.Certificato, out datiGP6, out errori);
            if (datiGP6 != null && datiGP6.ListaDatiTGP6 != null && datiGP6.ListaDatiTGP6.Count() > 0)
            {
                if (datiGP6.ListaDatiTGP6[0].GP6HG00 != null && datiGP6.ListaDatiTGP6[0].GP6HG00.Count() > 0)
                {
                    if (datiGP6.ListaDatiTGP6[0].GP6HG00[0].GP6HG01 != null)
                        richiesta.CodFondoStorico = Utility.StringToNullableDecimalPoint(datiGP6.ListaDatiTGP6[0].GP6HG00[0].GP6HG01.Valore.Codice).GetValueOrDefault();
                }
            }

            ServiceReferences.DatiPensioni.DatiTGP8Response datiGP8 = null;
            GestioneDatiPensioni.GetDatiTGP8ByChiavePensione(nDomus, richiesta.CodCategoria + richiesta.CodSede + richiesta.Certificato, out datiGP8, out errori);
            if (datiGP8 != null && datiGP8.ListaElementoDatiTGP8 != null && datiGP8.ListaElementoDatiTGP8.Count() > 0)
            {
                if (datiGP8.ListaElementoDatiTGP8[0].GP8MD00 != null && datiGP8.ListaElementoDatiTGP8[0].GP8MD00.Count() > 0)
                {
                    if (datiGP8.ListaElementoDatiTGP8[0].GP8MD00[0].GP8MD05E != null)
                        richiesta.ImportoTrattenuteErarialiAP = Utility.StringToNullableDecimalPoint(datiGP8.ListaElementoDatiTGP8[0].GP8MD00[0].GP8MD05E.Valore.Codice).GetValueOrDefault();
                }
            }

            richiesta.InvioMail = true;
        }
        #endregion public members

        #region private methods
        internal static short GetGP1AXE3byReqAnzVecch(bool? vecch94, bool? anz94, bool? anz96)
        {
            if (!vecch94.HasValue && !anz94.HasValue && !anz96.HasValue)
                return 0;

            if (vecch94.HasValue && vecch94.Value)
            {
                if (anz94.HasValue && anz94.Value)
                {
                    if (anz96.HasValue && anz96.Value)
                        return 2;
                    else
                        return 4;
                }
                else
                {
                    if (anz96.HasValue && anz96.Value)
                        return 1;
                    else
                        return 3;
                }
            }
            else
            {
                if (anz94.HasValue && anz94.Value)
                {
                    if (anz96.HasValue && anz96.Value)
                        return 6;
                    else
                        return 8;
                }
                else
                {
                    if (anz96.HasValue && anz96.Value)
                        return 5;
                    else
                        return 7;
                }
            }
        }

        private static void GetCodiceProvinciaNascita(string provinciaNascita, out short codProvNascita)
        {
            codProvNascita = 0;
            string query = (from s in INPS.DNA.Context.OfficeList.Offices
                            where (s.Value.ExtendedProperties != null ? s.Value.ExtendedProperties["PR"].Trim() : s.Value.Province.Trim()) == provinciaNascita.Trim()
                            select s.Value.SSCode).FirstOrDefault<string>();
            short.TryParse(query, out codProvNascita);
        }

        private static void ValorizzaIntestazione(ref EntityBLCommon.ContenitoreObject contenitore, int annoCompetenza, out Data.CAREPET.Intestazione intestazione)
        {
            intestazione = new Data.CAREPET.Intestazione();
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);

            //ENG - Implementazione Meta Processo
            GestioneControlliDinamici.ControlloDinamico ctrl_SbloccaMetaProcesso = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrl_SbloccaMetaProcesso);

            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico != null && Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault() && Utility.DataStrettamenteSuccessivaA(new DateTime(1997, 01, 01), contenitore.DatiPensione.DecorrenzaOriginaria.GetValueOrDefault()))
            {
                string year = contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.GetValueOrDefault().ToString().Split('/')[2].Substring(0, 4);
                string month = contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.GetValueOrDefault().ToString().Split('/')[1];
                string day = contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.GetValueOrDefault().ToString().Split('/')[0];
                string date = year + month;
                int data = 0;
                int.TryParse(date, out data);
                intestazione.T_GP1AF09Z = data;

            }
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || contenitore.IsRiaperturaDomanda)
                intestazione.PNINSIEME = "R" + annoCompetenza.ToString().PadLeft(4, '0').Substring(2, 2);
            else
                intestazione.PNINSIEME = "N" + annoCompetenza.ToString().PadLeft(4, '0').Substring(2, 2);
            if (Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione))
                intestazione.T_WEBDOAS4 = "DA";
            else
                intestazione.T_WEBDOAS4 = "NO";
            if (!Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) &&
                (contenitore.TipoCalcolo == Utility.TipoCalcolo.Contributivo || ((Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.Contributivo == '8')))
                intestazione.T_CONTRIBUTIVA = "SI";
            else
                intestazione.T_CONTRIBUTIVA = "NO";

            if (contenitore.DatiNuoveLiquidate != null)
            {
                if (ctrl_SbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrl_SbloccaMetaProcesso.ValoreControllo) && ctrl_SbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    intestazione.T_PROCESSO = contenitore.DatiNuoveLiquidate.CodiceProcesso.GetValueOrDefault();
                }
                else if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                {
                    if (Utility.isRicostituzioneOrRiaperturaPolarizzata(contenitore.DatiPensione, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)))
                        intestazione.T_PROCESSO = contenitore.DatiNuoveLiquidate.CodiceProcessoGP1ALZ6.GetValueOrDefault();
                    else
                        intestazione.T_PROCESSO = contenitore.DatiNuoveLiquidate.CodiceProcesso.GetValueOrDefault();
                }
                else
                    intestazione.T_PROCESSO = contenitore.DatiNuoveLiquidate.CodiceProcessoDestinazione.HasValue ? contenitore.DatiNuoveLiquidate.CodiceProcessoDestinazione.Value :
                        contenitore.DatiNuoveLiquidate.CodiceProcesso.HasValue ? (short)contenitore.DatiNuoveLiquidate.CodiceProcesso.Value : (short)0;

                intestazione.T_RAFLGPRW = contenitore.DatiNuoveLiquidate.FlagProvvisoria.HasValue ? contenitore.DatiNuoveLiquidate.FlagProvvisoria.Value ? "X" : " " : string.Empty;
            }
        }

        private static void ValorizzaDatiGenerici(ref EntityBLCommon.ContenitoreObject contenitore, string matricolaOperatore, DateTime dataSistema, out Data.CAREPET.DatiGenericiNew datiGenerici)
        {
            datiGenerici = new Data.CAREPET.DatiGenericiNew();
            string codCat = contenitore.DatiPensione.GetCodCategoria();
            //ENG - Memo 57_2023
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoAbilitazioneMemo57_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out controlloDinamicoAbilitazioneMemo57_2023);

            GestioneControlliDinamici.ControlloDinamico ctrlSbloccaMetaProcesso = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrlSbloccaMetaProcesso);

            datiGenerici.T_GP1AB01_V = codCat.Length > 3 ? codCat.Substring(1, 3) : codCat;
            datiGenerici.T_GP1AB02_V = contenitore.DatiPensione.CodiceSedeDestinazione.HasValue ? contenitore.DatiPensione.CodiceSedeDestinazione.Value : contenitore.DatiPensione.CodiceSede;
            datiGenerici.T_GP1AB03_V = contenitore.DatiPensione.NCertificato.HasValue ? contenitore.DatiPensione.NCertificato.Value : 0;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                datiGenerici.T_TP1CAT8_V = !string.IsNullOrEmpty(contenitore.DatiPensione.SiglaCategoria) ? contenitore.DatiPensione.SiglaCategoria.Trim().Substring(0, 6) : string.Empty;
            else
                datiGenerici.T_TP1CAT8_V = !string.IsNullOrEmpty(contenitore.DatiPensione.SiglaCategoria) ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty;

            if (ctrlSbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrlSbloccaMetaProcesso.ValoreControllo) && ctrlSbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                datiGenerici.T_TP1COP_V = contenitore.DatiPensione.CentroOperativo.GetValueOrDefault();
            }
            else if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                if (Utility.isRicostituzioneOrRiaperturaPolarizzata(contenitore.DatiPensione, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)))
                    datiGenerici.T_TP1COP_V = contenitore.DatiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault();
                else
                    datiGenerici.T_TP1COP_V = contenitore.DatiPensione.CentroOperativo.GetValueOrDefault();
            }
            else
            {
                datiGenerici.T_TP1COP_V = contenitore.DatiPensione.CentroOperativoDestinazione.HasValue ? contenitore.DatiPensione.CentroOperativoDestinazione.Value :
                    contenitore.DatiPensione.CentroOperativo.HasValue ? (short)contenitore.DatiPensione.CentroOperativo.Value : (short)0;
            }
            if (contenitore.DatiPensione.TipoAutomazione != null)
                datiGenerici.T_CODPRO = contenitore.DatiPensione.CodiceProcedura.Replace('N', 'A');
            else
                datiGenerici.T_CODPRO = contenitore.DatiPensione.CodiceProcedura;
            datiGenerici.T_TP1ELABA = (short)dataSistema.Year;
            datiGenerici.T_TP1ELABG = (short)dataSistema.Day;
            datiGenerici.T_TP1ELABM = (short)dataSistema.Month;
            datiGenerici.T_TP1DATACQA = (short)dataSistema.Year;
            datiGenerici.T_TP1DATACQG = (short)dataSistema.Day;
            datiGenerici.T_TP1DATACQM = (short)dataSistema.Month;
            int resInt = 0;
            int.TryParse(matricolaOperatore, out resInt);
            if (contenitore.DatiPensione.TipoAutomazione != null && GestioneCtrlMatricoleAutomazione.IsMatricolaForAutomazione(matricolaOperatore))
                datiGenerici.T_TP1MATRICOLA = 0;
            else
                datiGenerici.T_TP1MATRICOLA = resInt;
            datiGenerici.T_NDOMUS = contenitore.DatiPensione.NDomus;

            if (contenitore.DatiIstruttoria != null)
                datiGenerici.T_TP1IS = contenitore.DatiIstruttoria.CodiceIsola.HasValue ? (short)contenitore.DatiIstruttoria.CodiceIsola.Value : (short)0;

            if (contenitore.DatiMaggiorazioniBenefici != null)
            {
                if (Utility.IsDomandaAGOReversibile(contenitore.DatiPensione))
                    datiGenerici.T_GP1ALA3 = contenitore.DatiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.HasValue ? contenitore.DatiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.Value : (short)0;
                else
                    datiGenerici.T_GP1ALA2 = contenitore.DatiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.HasValue ? contenitore.DatiMaggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.Value : (short)0;
            }

            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ReqArt2DL503.HasValue)
            {
                datiGenerici.T_GP1AV91M = contenitore.DatiPensioniDatiGenerici.ReqArt2DL503.Value;
            }

            //ENG - Memo 57_2023
            if (controlloDinamicoAbilitazioneMemo57_2023 != null && !String.IsNullOrEmpty(controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo) &&
                controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
                    datiGenerici.T_GP1AT22 = contenitore.DatiPensione.AnnoMonitoraggio;
            }

            //ENG - AGO SUPERSTITI SPACCHETTATE (SO 0003, SR 0017, SOART 0020, SOCOM 0023)
            if (Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) ||
                   Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                {
                    if (contenitore.DatiPensione.GP1AJSP.HasValue)
                        datiGenerici.T_GP1AJSP = contenitore.DatiPensione.GP1AJSP.Value.ToString();
                }
                else
                {
                    datiGenerici.T_GP1AJSP = "1";
                }
            }


        }

        private static void ValorizzaPensionato(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Pensionato pensionato)
        {
            pensionato = new Data.CAREPET.Pensionato();

            if (contenitore.DatiAreaTitolare.Anagrafica != null)
            {
                pensionato.T_GP3CB02T_V = contenitore.DatiAreaTitolare.Anagrafica.Cognome;
                pensionato.T_GP3CB03T_V = contenitore.DatiAreaTitolare.Anagrafica.Nome;
                pensionato.T_GP3CB04T_V = contenitore.DatiAreaTitolare.Anagrafica.CognomeAcquisito;
                pensionato.T_GP3CB05T_V = contenitore.DatiAreaTitolare.Anagrafica.Sesso.HasValue ? contenitore.DatiAreaTitolare.Anagrafica.Sesso.Value.ToString() : "";
                pensionato.T_GP3CB06TA_V = contenitore.DatiAreaTitolare.Anagrafica.DataNascita.HasValue ? (short)contenitore.DatiAreaTitolare.Anagrafica.DataNascita.Value.Year : (short)0;
                pensionato.T_GP3CB06TG_V = contenitore.DatiAreaTitolare.Anagrafica.DataNascita.HasValue ? (short)contenitore.DatiAreaTitolare.Anagrafica.DataNascita.Value.Day : (short)0;
                pensionato.T_GP3CB06TM_V = contenitore.DatiAreaTitolare.Anagrafica.DataNascita.HasValue ? (short)contenitore.DatiAreaTitolare.Anagrafica.DataNascita.Value.Month : (short)0;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(contenitore.DatiAreaTitolare.Anagrafica.CodiceComuneNascita, Utility.TipoAppartenenza.AGO.ToString(), 0, false, out codiceInpsComune);
                pensionato.T_GP3CB07T_V = codiceInpsComune;
                pensionato.T_GP3CB08T_V = contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale;
                pensionato.T_GP3CB10T_V = contenitore.DatiAreaTitolare.Anagrafica.Codice1Arca;
                int resInt = 0;
                int.TryParse(contenitore.DatiAreaTitolare.Anagrafica.Codice2Arca, out resInt);
                pensionato.T_GP3CB11T_V = resInt;
                pensionato.T_GP3CB17T_V = contenitore.DatiAreaTitolare.Anagrafica.ComuneNascita;
                if (!string.IsNullOrEmpty(contenitore.DatiAreaTitolare.Anagrafica.ProvinciaNascita))
                {
                    if (contenitore.DatiAreaTitolare.Anagrafica.ProvinciaNascita.Trim().Length >= 4)
                        pensionato.T_GP3CB27T_V = "EE";
                    else
                        pensionato.T_GP3CB27T_V = contenitore.DatiAreaTitolare.Anagrafica.ProvinciaNascita.Trim();
                }
                pensionato.T_GP1RCOMUNE_V = contenitore.DatiAreaTitolare.Anagrafica.ComuneResidenza;
                pensionato.T_GP1RPROV_V = contenitore.DatiAreaTitolare.Anagrafica.ProvinciaResidenza.Trim();

                if (!string.IsNullOrEmpty(contenitore.DatiAreaTitolare.Anagrafica.Indirizzo))
                {
                    if (contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Length > 52)
                    {
                        pensionato.T_GP1RIND1_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Substring(0, 52);
                        if (contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Length > 104)
                        {
                            pensionato.T_GP1RIND2_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Substring(52, 52);
                            if (contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Length > 156)
                                pensionato.T_GP1RIND3_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Substring(104, 52);
                            else
                                pensionato.T_GP1RIND3_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Substring(104);
                        }
                        else
                            pensionato.T_GP1RIND2_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim().Substring(52);
                    }
                    else
                        pensionato.T_GP1RIND1_V = contenitore.DatiAreaTitolare.Anagrafica.Indirizzo.Trim();
                }

                pensionato.T_GP1RCIVICO_V = contenitore.DatiAreaTitolare.Anagrafica.NCivico;
                pensionato.T_GP1RFRAZIONE_V = contenitore.DatiAreaTitolare.Anagrafica.FrazioneResidenza;
                pensionato.T_GP1RCAP_V = contenitore.DatiAreaTitolare.Anagrafica.CAP;

                if (contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.HasValue && contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.Value)
                {
                    pensionato.T_GP1AZ03 = 1;
                    pensionato.T_GP1RRESIDOM_V = "9";
                    //IL CAMPO SU HOST E' DI SOLI 3 CARATTERI, PER IL MOMENTO ATTIVITA' SOSPESA, POI BASTERA' DECOMMENTARE
                    ////Per tutte le domande della linea AGO, che abbiano titolare residente all’estero
                    //pensionato.T_GP1RPROV_V = "I0-AUSGSR";
                }
                else if (contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.HasValue && !contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.Value)
                {
                    pensionato.T_GP1AZ03 = 0;
                    pensionato.T_GP1RRESIDOM_V = "1";
                }
            }
        }

        private static void ValorizzaIstruttoria(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out Data.CAREPET.Istruttoria istruttoria)
        {
            istruttoria = new Data.CAREPET.Istruttoria();

            if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                istruttoria.T_GP1AV11_V = contenitore.DatiPensione.GP1AV11.GetValueOrDefault();
            }
            else
            {
                istruttoria.T_GP1AV11_V = contenitore.DatiPensione.CentroOperativoDestinazione.HasValue ?
                    contenitore.DatiPensione.CentroOperativoDestinazione.GetValueOrDefault() : contenitore.DatiPensione.CentroOperativo.GetValueOrDefault();
            }
            istruttoria.T_GP1AN06A = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Year;
            istruttoria.T_GP1AN06M = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Month;
            istruttoria.T_GP1AN06G = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Day;
            istruttoria.T_TP1NOARC = contenitore.DatiPensione.FlagVerify.HasValue ? contenitore.DatiPensione.FlagVerify.Value ? "1" : "0" : "";
            istruttoria.T_GP2BM03A = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Year : (short)0;
            istruttoria.T_GP2BM03M = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Month : (short)0;
            istruttoria.T_GP2BM03G = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Day : (short)0;
            istruttoria.T_GP1AD01A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
            istruttoria.T_GP1AD01M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
            if (Utility.IsDomandaIOCUM(contenitore.DatiPensione.SiglaCategoria) && (Utility.IsDomandaPensioneInabilita(contenitore.DatiPensione) || Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                && contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.EnteIstruttoreExInpdap.GetValueOrDefault())
                istruttoria.T_GP1AD02 = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;

            istruttoria.T_GP1AD01_OA_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
            istruttoria.T_GP1AD01_OM_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
            if (!Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria)
                && !(Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                && !Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
            {
                istruttoria.T_TP1ILEGA = contenitore.DatiPensione.DataInteressiLegali.HasValue ? (short)contenitore.DatiPensione.DataInteressiLegali.Value.Year : (short)0;
                istruttoria.T_TP1ILEGM = contenitore.DatiPensione.DataInteressiLegali.HasValue ? (short)contenitore.DatiPensione.DataInteressiLegali.Value.Month : (short)0;
                istruttoria.T_TP1ILEGG = contenitore.DatiPensione.DataInteressiLegali.HasValue ? (short)contenitore.DatiPensione.DataInteressiLegali.Value.Day : (short)0;
            }
            istruttoria.T_GP1AJ05 = contenitore.DatiPensione.CodiceArretrati.HasValue ? contenitore.DatiPensione.CodiceArretrati.Value : (short)0;
            istruttoria.T_GP1AJ01_V = contenitore.DatiPensione.CausaCarico.HasValue ? contenitore.DatiPensione.CausaCarico.Value : (short)0;

            if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0011" && contenitore.DatiPensione.Tipo == "0001")
                istruttoria.T_GP1AF02 = !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione.Substring(0, 1) + "B" + contenitore.DatiPensione.NaturaPensione.Substring(2, 1) : null;
            else
                istruttoria.T_GP1AF02 = contenitore.DatiPensione.NaturaPensione;

            istruttoria.T_GP1AXA4A_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Year : (short)0;
            istruttoria.T_GP1AXA4M_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Month : (short)0;
            istruttoria.T_GP1AT03A = contenitore.DatiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)contenitore.DatiPensione.DecorrenzaCalcoloArretrati.Value.Year : (short)0;
            istruttoria.T_GP1AT03M = contenitore.DatiPensione.DecorrenzaCalcoloArretrati.HasValue ? (short)contenitore.DatiPensione.DecorrenzaCalcoloArretrati.Value.Month : (short)0;

            if (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria)
                && ((Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione) && contenitore.DatiPensione.CodiceTipoRichiesta == "71")
                || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.Contributivo == '7')))
                istruttoria.T_GP1AF03_V = 7.ToString();
            else if ((Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) ||
                (Utility.IsDomandaReversibilita(contenitore.DatiPensione) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))) &&
                contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.Contributivo == '1')
                istruttoria.T_GP1AF03_V = 1.ToString();
            else if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.Contributivo == '8' &&
                (Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria)))
                istruttoria.T_GP1AF03_V = 8.ToString();
            else if (Utility.IsDomandaAUT(contenitore.DatiPensione))
                //Per domande AUT il tipoCalcolo è sempre Contributivo. Sulla variabile di mapping GP1AF03 verrà sempre mappato 
                //con il valore 8, corrispondente alla decodifica Contributivo uguale a 'SI'.
                istruttoria.T_GP1AF03_V = 8.ToString();
            else if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (Utility.IsDomandaTrasformazioneAOI(contenitore.DatiPensione).GetValueOrDefault())
                    istruttoria.T_GP1AF03_V = 7.ToString();
                else if (contenitore.DatiPensione.TipoCalcolo.GetValueOrDefault() == 1) // Tipo calcolo contributivo
                    istruttoria.T_GP1AF03_V = 8.ToString();
                else
                    istruttoria.T_GP1AF03_V = 2.ToString();
            }
            else if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
                istruttoria.T_GP1AF03_V = 1.ToString();
            else if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) ||
                     (Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "FS") ||
                     Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) ||
                     Utility.IsIsoPensioneRicWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) ||
                     Utility.IsDomandaESPA_L26(contenitore.DatiPensione) || Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione) ||
                     (Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)))
                istruttoria.T_GP1AF03_V = string.Empty;
            else if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria))
                istruttoria.T_GP1AF03_V = contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.Contributivo.HasValue ? contenitore.DatiStoricoGP.Contributivo.ToString() : string.Empty;
            else if (Utility.IsDomandaINPGI(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.TipoCalcolo.GetValueOrDefault() == 1) // Tipo calcolo contributivo
                istruttoria.T_GP1AF03_V = 8.ToString();
            else if (Utility.IsDomandaINPGI(contenitore.DatiPensione.SiglaCategoria) && (contenitore.DatiPensione.TipoCalcolo.GetValueOrDefault() == 2 || contenitore.DatiPensione.TipoCalcolo.GetValueOrDefault() == 21))
                istruttoria.T_GP1AF03_V = 2.ToString();
            else
                istruttoria.T_GP1AF03_V = contenitore.DatiPensione.Contributivo.HasValue ? contenitore.DatiPensione.Contributivo.Value.ToString() : 0.ToString();

            short codiceAziendaTraduzioneSuGP = 0;
            if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
            {
                if (Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (contenitoreDecodifica.ElencoDecodificaBanchePerSede != null)
                    {
                        short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                        short sede = contenitore.DatiPensione.CodiceSede;
                        var banca = contenitoreDecodifica.ElencoDecodificaBanchePerSede.Find(x => x.CodiceSede == sede.ToString().PadLeft(4, '0') && x.Id == codiceBancaEsodati);
                        if (banca != null)
                            istruttoria.T_GP1CENTCRD_V = short.Parse(banca.TraduzioneSuGP);
                    }
                }
                else
                {
                    if (contenitoreDecodifica.ElencoDecAziendaAll != null)
                    {
                        short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                        GestioneDecodificaAzienda.DecAzienda decAziendaEditoria = contenitoreDecodifica.ElencoDecAziendaAll.Find(x => x.Id == codiceBancaEsodati);
                        if (decAziendaEditoria != null)
                            istruttoria.T_GP1CENTCRD_V = codiceAziendaTraduzioneSuGP = short.Parse(decAziendaEditoria.TraduzioneSuGP);
                    }
                }
            }

            if (contenitore.DatiPensioniDatiGenerici != null)
            {
                if (contenitore.DatiPensioniDatiGenerici.EnteCassa.HasValue)
                {
                    long enteCassa = contenitore.DatiPensioniDatiGenerici.EnteCassa.Value;
                    if (contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale != null && contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Count > 0)
                    {
                        GestioneDecodifica.DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionale = contenitoreDecodifica.ElencoDecodificaEnteCassaProfessionale.Find(x => x.Id == enteCassa);
                        if (decodificaEnteCassaProfessionale != null)
                            istruttoria.T_GP1CENTCRD_V = short.Parse(decodificaEnteCassaProfessionale.TraduzioneSuGP);
                    }
                }
                istruttoria.T_GP1AXB8_V = contenitore.DatiPensioniDatiGenerici.ImportoUltimaRetribuzione ?? 0;
                if (Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (IsFormatoScadenzaAssegnoGGMMAAAA(ref contenitore, ref contenitoreDecodifica, codiceAziendaTraduzioneSuGP,
                        contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria : null,
                        contenitore.IsRiaperturaDomanda))
                    {
                        istruttoria.T_GP1AG02A = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Year : (short)0;
                        istruttoria.T_GP1AG02M = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Month : (short)0;
                        istruttoria.T_GP1AG02G = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Day : (short)0;

                        istruttoria.T_GP1AF06A_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Year : (short)0;
                        istruttoria.T_GP1AF06M_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Month : (short)0;
                    }
                    else
                    {
                        istruttoria.T_GP1AF06A_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Year : (short)0;
                        istruttoria.T_GP1AF06M_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Month : (short)0;
                    }
                }
                else if (Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) ||
                         Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
                {
                    istruttoria.T_GP1AF06A_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Year : (short)0;
                    istruttoria.T_GP1AF06M_V = contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.ScadenzaAssegno.Value.Month : (short)0;
                }
            }

            istruttoria.T_GP1AV04 = contenitore.DatiPensione.AttivitaEconomica.HasValue ? contenitore.DatiPensione.AttivitaEconomica.Value : 0;
            istruttoria.T_GP1AV05 = contenitore.DatiPensione.ProfessioneIndividuale.HasValue ? contenitore.DatiPensione.ProfessioneIndividuale.Value : 0;
            istruttoria.T_GP1ALA1_V = contenitore.DatiPensione.AliquotaTFREsodati.HasValue ? contenitore.DatiPensione.AliquotaTFREsodati.Value : 0M;
            istruttoria.T_GP2BM01A = contenitore.DatiPensione.InizioAssicurazione.HasValue ? (short)contenitore.DatiPensione.InizioAssicurazione.Value.Year : (short)0;
            istruttoria.T_GP2BM01M = contenitore.DatiPensione.InizioAssicurazione.HasValue ? (short)contenitore.DatiPensione.InizioAssicurazione.Value.Month : (short)0;
            istruttoria.T_GP2BM01G = contenitore.DatiPensione.InizioAssicurazione.HasValue ? (short)contenitore.DatiPensione.InizioAssicurazione.Value.Day : (short)0;
            istruttoria.T_GP2BM02A = contenitore.DatiPensione.FineAssicurazione.HasValue ? (short)contenitore.DatiPensione.FineAssicurazione.Value.Year : (short)0;
            istruttoria.T_GP2BM02M = contenitore.DatiPensione.FineAssicurazione.HasValue ? (short)contenitore.DatiPensione.FineAssicurazione.Value.Month : (short)0;
            istruttoria.T_GP2BM02G = contenitore.DatiPensione.FineAssicurazione.HasValue ? (short)contenitore.DatiPensione.FineAssicurazione.Value.Day : (short)0;

            //Al termine dei controlli per registrare il valore giusto al campo RAD411
            //effettuare queste operazioni:
            bool posticipo = false;
            int panvein = 0;
            GestioneControlli.GetPanvein_Posticipo(contenitore.DatiPensione.NaturaPensione, contenitore.DatiPensione, out panvein, out posticipo);

            if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                (Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaSPED(contenitore.DatiPensione) ||
                Utility.IsDomandaAUT(contenitore.DatiPensione) ||
                Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaMIN(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsRenditaCasalinghe(contenitore.DatiPensione) ||
                Utility.IsRenditaFacoltativa(contenitore.DatiPensione) ||
                Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaPMO(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria)))
                istruttoria.T_GP1AXE3 = 0;
            else if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                (Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaSPED(contenitore.DatiPensione) ||
                Utility.IsDomandaAUT(contenitore.DatiPensione) ||
                Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaMIN(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsRenditaCasalinghe(contenitore.DatiPensione) ||
                Utility.IsRenditaFacoltativa(contenitore.DatiPensione) ||
                Utility.IsDomandaVOST(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaPMO(contenitore.DatiPensione.SiglaCategoria) ||
                Utility.IsDomandaINDCOM(contenitore.DatiPensione.SiglaCategoria)))
                istruttoria.T_GP1AXE3 = contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP1AXE3.HasValue ? contenitore.DatiStoricoGP.GP1AXE3.Value : (short)0;
            else
                istruttoria.T_GP1AXE3 = ValorizzaGP1AXE3(contenitore.DatiPensione, contenitore.DatiStoricoGP, contenitore.IsRiaperturaDomanda, panvein);

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
            {
                List<GestioneDecodifica.DecEnteGestioneFondo> listaApp = contenitoreDecodifica.ElencoDecEnteGestioneFondo.FindAll(x => x.Codice == "A1" || x.Codice == "A5" || x.Codice == "A6" ||
                    x.Codice == "A7" || x.Codice == "A8" || x.Codice == "A9" || x.Codice == "B1" || x.Codice == "B2" || x.Codice == "B4" || x.Codice == "F0"
                    || x.Codice == "C1" || x.Codice == "C2" || x.Codice == "C3" || x.Codice == "C4" || x.Codice == "C5" || x.Codice == "D1" || x.Codice == "E1" || x.Codice == "E2");

                if (contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0 && contenitore.ListaQuotePensione.Exists(x => listaApp.Exists(y => y.Id == x.EnteGestioneFondo)))
                    istruttoria.T_GP1AJ11 = "1";
                else
                    istruttoria.T_GP1AJ11 = "2";
            }

            if (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                List<GestioneDecodifica.DecEnteGestioneFondo> listaApp = contenitoreDecodifica.ElencoDecEnteGestioneFondo.FindAll(x => x.Codice == "A1" || x.Codice == "A5" || x.Codice == "A6" ||
                    x.Codice == "A7" || x.Codice == "A8" || x.Codice == "A9" || x.Codice == "B1" || x.Codice == "B2" || x.Codice == "B3" || x.Codice == "B4" || x.Codice == "F0"
                    || x.Codice == "C1" || x.Codice == "C2" || x.Codice == "C3" || x.Codice == "C4" || x.Codice == "C5" || x.Codice == "D1" || x.Codice == "E1" || x.Codice == "E2" ||
                    x.Codice == "SP");

                if (contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0 && contenitore.ListaQuotePensione.Exists(x => listaApp.Exists(y => y.Id == x.EnteGestioneFondo)))
                    istruttoria.T_GP1AJ11 = "1";
                else
                    istruttoria.T_GP1AJ11 = "2";
            }

            if (contenitore.DatiIstruttoria != null)
            {
                istruttoria.T_GP1AJ08_V = contenitore.DatiIstruttoria.CodiceCdCmMr.HasValue ? contenitore.DatiIstruttoria.CodiceCdCmMr.Value : (short)0;
                istruttoria.T_GP1AG03A = contenitore.DatiIstruttoria.DecorrenzaOpzione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaOpzione.Value.Year : (short)0;
                istruttoria.T_GP1AG03M = contenitore.DatiIstruttoria.DecorrenzaOpzione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaOpzione.Value.Month : (short)0;

                //Richiesta 20151221 (MAIL Pasquale Cozzolino oggetto: 'FW: LiqPens AGO - Segnalazioni')
                if (!Utility.IsCategoriaAutonomi(contenitore.DatiPensione.SiglaCategoria.Trim().ToUpperInvariant()) ||
                    Utility.IsDomandaIndennitaUnaTantum_AGO(contenitore.DatiPensione))
                {
                    if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                    {
                        int? NSettimaneTotali = contenitore.DatiIstruttoria.NSettimaneOBG.Value + contenitore.DatiIstruttoria.NSettimaneOI.Value;
                        istruttoria.T_GP1AV08 = NSettimaneTotali.HasValue ? NSettimaneTotali.Value : 0;
                    }
                    else
                    {
                        istruttoria.T_GP1AV08 = contenitore.DatiIstruttoria.NSettimaneOBG.HasValue ? contenitore.DatiIstruttoria.NSettimaneOBG.Value : 0;
                    }
                    istruttoria.T_GP1AV09 = contenitore.DatiIstruttoria.NContributiVolontari.HasValue ? contenitore.DatiIstruttoria.NContributiVolontari.Value : 0;
                    istruttoria.T_GP1AV10 = contenitore.DatiIstruttoria.NContributiVVAnzianita.HasValue ? contenitore.DatiIstruttoria.NContributiVVAnzianita.Value : 0;
                }
                if (Utility.IsCategoriaAutonomi(contenitore.DatiPensione.SiglaCategoria.Trim().ToUpperInvariant()))
                {
                    if (Utility.IsDomandaOrganizzazioniInternazionali(contenitore.DatiPensione))
                    {
                        int? NSettimaneTotali = contenitore.DatiIstruttoria.NSettimaneOBG.Value + contenitore.DatiIstruttoria.NSettimaneOI.Value;
                        istruttoria.T_GP2BN02 = NSettimaneTotali.HasValue ? NSettimaneTotali.Value : 0;
                    }
                    else
                    {
                        istruttoria.T_GP2BN02 = contenitore.DatiIstruttoria.NSettimaneOBG.HasValue ? contenitore.DatiIstruttoria.NSettimaneOBG.Value : 0;
                    }
                }


                if (!Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) &&
                    !Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) &&
                    !Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) &&
                    !Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) &&
                    !Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
                {
                    istruttoria.T_GP1AF06A_V = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.Value.Year : (short)0;
                    istruttoria.T_GP1AF06M_V = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.Value.Month : (short)0;
                }

                istruttoria.T_GP1AP49 = contenitore.DatiIstruttoria.CodiceMobilita.HasValue ? contenitore.DatiIstruttoria.CodiceMobilita.Value : (short)0;
                if (!IsFormatoScadenzaAssegnoGGMMAAAA(ref contenitore, ref contenitoreDecodifica, codiceAziendaTraduzioneSuGP,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria : null,
                    contenitore.IsRiaperturaDomanda) &&
                    !Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) &&
                    !(Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria)) &&
                    !Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria))
                {
                    istruttoria.T_GP1AG02A = contenitore.DatiIstruttoria.DataDomandaOpzione.HasValue ? (short)contenitore.DatiIstruttoria.DataDomandaOpzione.Value.Year : (short)0;
                    istruttoria.T_GP1AG02M = contenitore.DatiIstruttoria.DataDomandaOpzione.HasValue ? (short)contenitore.DatiIstruttoria.DataDomandaOpzione.Value.Month : (short)0;
                    istruttoria.T_GP1AG02G = contenitore.DatiIstruttoria.DataDomandaOpzione.HasValue ? (short)contenitore.DatiIstruttoria.DataDomandaOpzione.Value.Day : (short)0;
                }
                if (contenitore.DatiIstruttoria.TipoPensioneExInpdai.HasValue)
                {
                    if (contenitoreDecodifica.ElencoPensioneExInpdai != null && contenitoreDecodifica.ElencoPensioneExInpdai.Count > 0)
                    {
                        byte tipoPensioneExInpdai = contenitore.DatiIstruttoria.TipoPensioneExInpdai.Value;
                        GestioneDecodifica.PensioneExInpdai pensioneExInpdai = contenitoreDecodifica.ElencoPensioneExInpdai.Find(x => x.Id == tipoPensioneExInpdai);
                        if (pensioneExInpdai != null)
                            istruttoria.T_GP1AV91I = pensioneExInpdai.TraduzioneSuGp.HasValue ? pensioneExInpdai.TraduzioneSuGp.Value : (short)0;
                    }
                }
                istruttoria.T_GP1CPOSLVR = contenitore.DatiIstruttoria.CodPosizioneLavoro;
                istruttoria.T_GP1AJ02 = contenitore.DatiIstruttoria.CodiceDomandaRicorso.HasValue ? contenitore.DatiIstruttoria.CodiceDomandaRicorso.Value : (short)0;
                istruttoria.T_GP1AV53 = contenitore.DatiIstruttoria.CodiceP18PrecedentePensione.HasValue ? contenitore.DatiIstruttoria.CodiceP18PrecedentePensione.Value : (short)0;
                istruttoria.T_GP1AV54 = contenitore.DatiIstruttoria.SedePrecedentePensione.HasValue ? contenitore.DatiIstruttoria.SedePrecedentePensione.Value : (short)0;
                istruttoria.T_GP1AV55 = contenitore.DatiIstruttoria.CertificatoPrecedentePensione.HasValue ? contenitore.DatiIstruttoria.CertificatoPrecedentePensione.Value : 0;

                if (contenitore.DatiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "PSO")
                {
                    istruttoria.T_GP1AV53 = (short)0;
                    istruttoria.T_GP1AV54 = (short)0;
                    istruttoria.T_GP1AV55 = 0;
                }
                if (contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault() > 0)
                {
                    if (!string.IsNullOrEmpty(contenitore.DatiPensione.SiglaCategoria))
                    {
                        switch (contenitore.DatiPensione.SiglaCategoria.Trim().ToUpperInvariant())
                        {
                            case "VO":
                            case "IO":
                            case "SO":
                            case "VOMIN":
                            case "IOART":
                            case "IOMIN":
                            case "VOP":
                            case "IOP":
                            case "PMO":
                            case "SOMIN":
                            case "SOP":
                                if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) == null && (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || contenitore.DatiPensione.Gruppo == "0003" ||
                                string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) || contenitore.DatiPensione.NaturaPensione.Substring(0, 1) != "5"))
                                {
                                    if (contenitore.DatiIstruttoria.NSettimaneOBG.Value > 780)
                                        istruttoria.T_GP1AF08 = 1;
                                    else
                                        istruttoria.T_GP1AF08 = 2;
                                }
                                else if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null && contenitore.DatiIstruttoria.GP1AF08.HasValue)
                                {
                                    istruttoria.T_GP1AF08 = contenitore.DatiIstruttoria.GP1AF08.Value;
                                }

                                break;

                        }

                    }
                }

                istruttoria.T_TP1MENT = contenitore.DatiIstruttoria.CodiceIsola.HasValue ? (short)contenitore.DatiIstruttoria.CodiceIsola.Value : (short)0;

                istruttoria.T_GP1AV51A = contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione.Value.Year : (short)0;
                istruttoria.T_GP1AV51M = contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaOriginariaAltraPensione.Value.Month : (short)0;

                if (contenitore.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    if (contenitoreDecodifica.ElencoCodiceParticolare != null && contenitoreDecodifica.ElencoCodiceParticolare.Count > 0)
                    {
                        long codicePart = contenitore.DatiIstruttoria.CodiceParticolareSoggettoDerogato.Value;
                        GestioneDecodifica.CodiceParticolare codiceParticolare = contenitoreDecodifica.ElencoCodiceParticolare.Find(x => x.Id == codicePart);
                        if (codiceParticolare != null)
                            istruttoria.T_GP1AJ11 = codiceParticolare.TraduzioneSuGp.HasValue && (codiceParticolare.TraduzioneSuGp.Value != '3' || !Utility.IsDomandaUsuranti(contenitore.DatiPensione)) ? codiceParticolare.TraduzioneSuGp.Value.ToString() : string.Empty;
                    }
                }

                if (Utility.IsDomandaPSO(contenitore.DatiPensione.SiglaCategoria))
                {
                    istruttoria.T_GP1AJ11 = contenitore.DatiIstruttoria.CodiceEnte.HasValue ? contenitore.DatiIstruttoria.CodiceEnte.Value.ToString() : string.Empty;
                }

                istruttoria.T_GP1AP47 = contenitore.DatiIstruttoria.Legge44997.HasValue ? contenitore.DatiIstruttoria.Legge44997.Value : (short)0;

                if (!string.IsNullOrEmpty(contenitore.DatiIstruttoria.ModalitaLiquidazione))
                {
                    if (contenitoreDecodifica.ElencoDecModalitaLiquidazione != null && contenitoreDecodifica.ElencoDecModalitaLiquidazione.Count > 0)
                    {
                        string modalitaLiquidazione = contenitore.DatiIstruttoria.ModalitaLiquidazione;
                        GestioneDecodifica.DecModalitaLiquidazione decModalitaLiquidazione = contenitoreDecodifica.ElencoDecModalitaLiquidazione.Find(x => x.ValoreAggPeco.Trim() == modalitaLiquidazione.Trim());
                        if (decModalitaLiquidazione != null)
                            istruttoria.T_GP1AZ11E_V = decModalitaLiquidazione.TraduzioneGp.ToString();
                    }
                }
                else if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    istruttoria.T_GP1AZ11E_V = "4";

                istruttoria.T_TP1COLIQ = contenitore.DatiIstruttoria.CodiceLiquidazione.HasValue ? contenitore.DatiIstruttoria.CodiceLiquidazione.Value.ToString() : string.Empty;
                istruttoria.T_GP1AV72_V = contenitore.DatiIstruttoria.NRiconoscimentiInvalidita.HasValue ? contenitore.DatiIstruttoria.NRiconoscimentiInvalidita.Value : (short)0;
            }

            if (contenitore.DatiAreaTitolare.Patronato != null)
            {
                short codEnte = 0;
                short.TryParse(contenitore.DatiAreaTitolare.Patronato.CodiceEnte, out codEnte);
                if (codEnte < 100)
                    istruttoria.T_GP1AV01 = codEnte;
            }

            //aggiunta la condizione che deve essere diverso da ricostituzione e riapertura che era presente in GestionePrelievo.cs
            if (contenitore.DatiDetrazioni != null && !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                string detrazioni = (contenitore.DatiDetrazioni.DetrazioniReddito.HasValue ? contenitore.DatiDetrazioni.DetrazioniReddito.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.AgevolazionePensionati.HasValue ? contenitore.DatiDetrazioni.AgevolazionePensionati.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.ConiugeOFiglio.HasValue ? contenitore.DatiDetrazioni.ConiugeOFiglio.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMinori3AnniNoHandicap100.HasValue ? contenitore.DatiDetrazioni.FigliMinori3AnniNoHandicap100.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMinori3AnniNoHandicap50.HasValue ? contenitore.DatiDetrazioni.FigliMinori3AnniNoHandicap50.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMinori3AnniHandicap100.HasValue ? contenitore.DatiDetrazioni.FigliMinori3AnniHandicap100.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMinori3AnniHandicap50.HasValue ? contenitore.DatiDetrazioni.FigliMinori3AnniHandicap50.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMaggiori3AnniNoHandicap100.HasValue ? contenitore.DatiDetrazioni.FigliMaggiori3AnniNoHandicap100.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMaggiori3AnniNoHandicap50.HasValue ? contenitore.DatiDetrazioni.FigliMaggiori3AnniNoHandicap50.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMaggiori3AnniHandicap100.HasValue ? contenitore.DatiDetrazioni.FigliMaggiori3AnniHandicap100.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.FigliMaggiori3AnniHandicap50.HasValue ? contenitore.DatiDetrazioni.FigliMaggiori3AnniHandicap50.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.AltriFamiliari100.HasValue ? contenitore.DatiDetrazioni.AltriFamiliari100.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.AltriFamiliari50.HasValue ? contenitore.DatiDetrazioni.AltriFamiliari50.Value.ToString() : "0") +
                   (contenitore.DatiDetrazioni.AddizionaleLombardiaVeneto.HasValue ? contenitore.DatiDetrazioni.AddizionaleLombardiaVeneto.Value.ToString() : "0");

                istruttoria.T_GP3CDTI_V = Utility.StringToNullableInt64(detrazioni).HasValue ? Utility.StringToNullableInt64(detrazioni).Value : 0;
                if ((Utility.IsDomandaRipristino(contenitore.DatiPensione)).GetValueOrDefault())
                {
                    istruttoria.T_GP3DDTIVRCA_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Year : (short)0; ;
                    istruttoria.T_GP3DDTIVRCM_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Month : (short)0;
                    istruttoria.T_GP3DDTIVRCG_V = 1;
                }
                else if (contenitore.DatiDetrazioni.DecorrenzaDetrazioneImposte.HasValue)
                {
                    istruttoria.T_GP3DDTIVRCA_V = (short)contenitore.DatiDetrazioni.DecorrenzaDetrazioneImposte.Value.Year;
                    istruttoria.T_GP3DDTIVRCM_V = (short)contenitore.DatiDetrazioni.DecorrenzaDetrazioneImposte.Value.Month;
                    istruttoria.T_GP3DDTIVRCG_V = (short)contenitore.DatiDetrazioni.DecorrenzaDetrazioneImposte.Value.Day;
                }
                else
                {
                    istruttoria.T_GP3DDTIVRCA_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    istruttoria.T_GP3DDTIVRCM_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    istruttoria.T_GP3DDTIVRCG_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                }
            }

            if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione) ||
                Utility.IsDomandaBeneficioTerrorismoLegge206_2004(contenitore.DatiPensione))
            {
                istruttoria.T_GP3CDTI_V = 30000000000000;

                if (!Utility.IsDomandaBeneficioTerrorismoLegge206_2004(contenitore.DatiPensione))
                {
                    istruttoria.T_GP3DDTIVRCA_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    istruttoria.T_GP3DDTIVRCM_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    istruttoria.T_GP3DDTIVRCG_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                }
            }

            //ENG - Memo 48_2023
            if (Utility.IsTitolareResidente_Cittadino_Bulgaria(contenitore.DatiPensione, contenitore.DatiAreaTitolare != null ? contenitore.DatiAreaTitolare.Anagrafica : null))
            {
                istruttoria.T_GP3CDTI_V = 20000000000000;
            }

            if (contenitore.DatiSindacato != null || Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
            {
                istruttoria.LISTT_GP2BG10 = new List<Data.CAREPET.Istruttoria.T_GP2BG10>();
                Data.CAREPET.Istruttoria.T_GP2BG10 sindacato = new Data.CAREPET.Istruttoria.T_GP2BG10();
                sindacato.T_GP2BG11_V = contenitore.DatiSindacato != null && Utility.IsSindacatoPresente(contenitore.DatiSindacato.CodiceSindacato) ? contenitore.DatiSindacato.CodiceSindacato : "  ";
                if ((contenitore.DatiSindacato != null && Utility.IsSindacatoPresente(contenitore.DatiSindacato.CodiceSindacato)) ||
                    Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                {
                    DateTime? decorrenzaSindacato = null;
                    if (contenitore.DatiSindacato != null)
                        decorrenzaSindacato = Utility.GetDecorrenzaPerSindacatoANPPE(contenitore.DatiSindacato.DecorrenzaSindacato, contenitore.DatiSindacato.CodiceSindacato);

                    if (Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                    {
                        sindacato.T_GP2BG12A_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Year : (short)0;
                        sindacato.T_GP2BG12M_V = contenitore.DatiPensione.DataInizioCalcolo.HasValue ? (short)contenitore.DatiPensione.DataInizioCalcolo.Value.Month : (short)0;
                    }
                    else
                    {
                        sindacato.T_GP2BG12A_V = decorrenzaSindacato.HasValue ? (short)decorrenzaSindacato.Value.Year : (short)0;
                        sindacato.T_GP2BG12M_V = decorrenzaSindacato.HasValue ? (short)decorrenzaSindacato.Value.Month : (short)0;
                    }
                    if (contenitore.DatiSindacato != null)
                    {
                        sindacato.T_GP2BG13A_V = contenitore.DatiSindacato.CessazioneSindacato.HasValue ? (short)contenitore.DatiSindacato.CessazioneSindacato.Value.Year : (short)9999;
                        sindacato.T_GP2BG13M_V = contenitore.DatiSindacato.CessazioneSindacato.HasValue ? (short)contenitore.DatiSindacato.CessazioneSindacato.Value.Month : (short)99;
                    }
                }
                istruttoria.LISTT_GP2BG10.Add(sindacato);
            }

            if (contenitore.DatiMaggiorazioniBenefici != null)
            {
                istruttoria.T_GP1AXF3 = contenitore.DatiMaggiorazioniBenefici.Attivitausuranti.HasValue ? contenitore.DatiMaggiorazioniBenefici.Attivitausuranti.Value ? "1" : "0" : string.Empty;

                istruttoria.T_GP1AJ03 = contenitore.DatiMaggiorazioniBenefici.CodiceCieco.HasValue ? (short)contenitore.DatiMaggiorazioniBenefici.CodiceCieco : (short)0;
                istruttoria.T_GP2BN53A = contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue ? (short)contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.Value.Year : (short)0;
                istruttoria.T_GP2BN53M = contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue ? (short)contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.Value.Month : (short)0;
                istruttoria.T_GP1AV61 = contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio;
                istruttoria.T_GP1NSETBEN = contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.HasValue ? contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.Value : 0;
                istruttoria.T_GP1AXF1 = contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento1Percento.HasValue ? contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento1Percento.Value : 0;
                istruttoria.T_GP1AXF2 = contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento05Percento.HasValue ? contenitore.DatiMaggiorazioniBenefici.NSettimaneIncremento05Percento.Value : 0;
                istruttoria.T_TP1SENT = contenitore.DatiMaggiorazioniBenefici.Sentenza495240.HasValue ? contenitore.DatiMaggiorazioniBenefici.Sentenza495240.Value : (short)0;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiEnpals != null)
                {
                    istruttoria.T_GP1AV08 = contenitore.DatiEnpals.AnzianitaContributiva.HasValue ? contenitore.DatiEnpals.AnzianitaContributiva.Value : 0;
                    // Commentato perchè i dati vengono già mappati correttamente nelle variabili tramite il TipoSettimaneBeneficio e le NSettimaneBeneficio
                    //if (datiEnpals.NumeroContributiNLNonVedenti.HasValue)
                    //{
                    //    istruttoria.T_GP1AV61 = "01";
                    //    istruttoria.T_GP1NSETBEN = datiEnpals.NumeroContributiNLNonVedenti.Value;
                    //}
                }
            }

            // Per domande con dati non provenienti da Felpe il campo non dovrà essere visibile 
            //(viene passato automaticamente al calcolo il valore 8 al tracciato di calcolo nel gp1az11e se viene selezionata la casella “Provvisoria”)
            if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true) != Utility.TipoUnicarpe.Automatica && contenitore.DatiNuoveLiquidate != null &&
                contenitore.DatiNuoveLiquidate.FlagProvvisoria.HasValue && contenitore.DatiNuoveLiquidate.FlagProvvisoria.Value)
                istruttoria.T_GP1AZ11E_V = "8";

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiEnpals != null)
                {
                    if (!string.IsNullOrEmpty(contenitore.DatiEnpals.TipoLiquidazione))
                    {
                        if (contenitore.DatiEnpals.TipoLiquidazione == "0")
                            istruttoria.T_GP1AZ11E_V = "0";
                        else
                        {
                            istruttoria.T_GP1AZ11E_V = "8";
                            if (contenitore.DatiEnpals.TipoLiquidazioneProvvisoria == "0")
                            {
                                if (Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione) || contenitore.IsRiaperturaDomanda)
                                    istruttoria.T_GP1AZ11F = 3;
                                else
                                    istruttoria.T_GP1AZ11F = 0;
                            }
                            else if (contenitore.DatiEnpals.TipoLiquidazioneProvvisoria == "3")
                                istruttoria.T_GP1AZ11F = 3;
                            else if (contenitore.DatiEnpals.TipoLiquidazioneProvvisoria == "4")
                                istruttoria.T_GP1AZ11F = 4;
                        }
                    }
                }

                //ENG - REVERSIBILITA' ENPALS: il campo T_GP1AZ11E_V deve essere passato sempre a "0"
                if (Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                {
                    istruttoria.T_GP1AZ11E_V = "0";
                }
            }
            else if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
            {
                if (contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP1AZ11F.HasValue)
                    istruttoria.T_GP1AZ11F = contenitore.DatiStoricoGP.GP1AZ11F.Value;
            }

            if (Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione) && contenitore.DatiIstruttoria.CodiceAziendaEditoria.HasValue)
                istruttoria.T_GP1CENTCRD_V = contenitore.DatiIstruttoria.CodiceAziendaEditoria.Value;
            else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione) && contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171.HasValue)
                istruttoria.T_GP1CENTCRD_V = contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171.Value;
            else if (Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) && contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179.HasValue)
                istruttoria.T_GP1CENTCRD_V = contenitore.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179.Value;
            else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione) && contenitore.DatiIstruttoria.CodiceAziendaEditoriaLetteraB.HasValue)
                istruttoria.T_GP1CENTCRD_V = contenitore.DatiIstruttoria.CodiceAziendaEditoriaLetteraB.Value;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            //Per i precoci la valorizzazione dl campo GP1AF06 avviene mediante la Scadenza Beneficio dell'onere acquisito
            //ENG - RIC REVERSIBILITA ENPALS: anche per le RIC deve essere inviata la Scadenza Beneficio
            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione) || Utility.IsDomandaQuota100(contenitore.DatiPensione) || Utility.IsDomandaQuota102(contenitore.DatiPensione) ||
                (Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione)) || Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione) ||
                (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione))) ||
                (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione)))) ||
                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
            {
                if (contenitore.ListaDatiOneri != null && contenitore.ListaDatiOneri.Count > 0 && contenitoreDecodifica.ElencoDecCodeGruppoOnere != null && contenitoreDecodifica.ElencoDecCodeGruppoOnere.Count > 0)
                {
                    string codeGruppoOneri = string.Empty;
                    if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
                        codeGruppoOneri = "5000";
                    else if (Utility.IsDomandaQuota100(contenitore.DatiPensione))
                        codeGruppoOneri = "5300";
                    else if (Utility.IsDomandaQuota102(contenitore.DatiPensione))
                        codeGruppoOneri = "5800";
                    else if (Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione))
                        codeGruppoOneri = "6000";
                    else if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione) ||
                        Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione))
                        codeGruppoOneri = "6100";
                    GestioneDecodifica.GruppoOneri gruppoOneri = contenitoreDecodifica.ElencoDecCodeGruppoOnere.FirstOrDefault(x => x.Code == codeGruppoOneri);
                    if (gruppoOneri != null)
                    {
                        GestioneOneri.DatiOneri onere = contenitore.ListaDatiOneri.FirstOrDefault(x => x.IdCodeGruppo == gruppoOneri.Id);
                        if (onere != null)
                        {
                            istruttoria.T_GP1AF06A_V = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Year : (short)0;
                            istruttoria.T_GP1AF06M_V = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Month : (short)0;
                        }
                    }
                    else if (Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                    {
                        GestioneOneri.DatiOneri onere = contenitore.ListaDatiOneri.FirstOrDefault();
                        if (onere != null)
                        {
                            istruttoria.T_GP1AF06A_V = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Year : (short)0;
                            istruttoria.T_GP1AF06M_V = onere.ScadenzaBeneficio.HasValue ? (short)onere.ScadenzaBeneficio.Value.Month : (short)0;
                        }
                    }
                }
            }

            //ENG - Aggiornamento Memo INPGI
            GestioneControlliDinamici.ControlloDinamico ctrlAggiornamentoMemo_INPGI_20240307 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAggiornamentoMemo_INPGI_20240307);
            if (Utility.IsDomandaVOPGI_AGI(contenitore.DatiPensione))
            {
                if (ctrlAggiornamentoMemo_INPGI_20240307 != null && !String.IsNullOrEmpty(ctrlAggiornamentoMemo_INPGI_20240307.ValoreControllo)
                    && ctrlAggiornamentoMemo_INPGI_20240307.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    istruttoria.T_GP1AJ11 = "1";
                }
            }

        }

        private static short ValorizzaGP1AXE3(GestionePensione.DatiPensione datiPensione, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, bool isRiaperturaDomanda, int panvein)
        {
            short gp1axe3 = 0;
            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) || Utility.IsDomandaPensioneInabilitaOrRicostituzioneAGO_CI(datiPensione) ||
                    (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("V") &&
                     (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1995, 2, 1)) || Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2008, 12, 31)))))
                    gp1axe3 = 0;
                else if (datiStoricoGP != null && datiStoricoGP.GP1AXE3.HasValue)
                    gp1axe3 = datiStoricoGP.GP1AXE3.Value;
            }
            else
            {
                //Se la Decorrenza Originaria è inferiore a febbraio 1995 o superiore a dicembre 2000 mettere zero al
                //campo RAD411
                if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1995, 2, 1)) || Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2000, 12, 31)))
                    gp1axe3 = 0;
                //Se il campo PANVEIN è = a 1 è il campo Vec. al 12/94 è = a “NO”
                //mettere 1 al campo RAD411
                else if (panvein == 1 && (!datiPensione.RequisitiVecchiaiaAl1294.HasValue || !datiPensione.RequisitiVecchiaiaAl1294.Value))
                    gp1axe3 = 1;
                //Se il campo PANVEIN è = a 1 è il campo Vec. al 12/94 è = a “SI”
                //mettere 2 al campo RAD411
                else if (panvein == 1 && datiPensione.RequisitiVecchiaiaAl1294.HasValue && datiPensione.RequisitiVecchiaiaAl1294.Value)
                    gp1axe3 = 2;
                else
                {
                    //Se il campo PANVEIN è = a zero e il campo Anz. al 12/94 è vuoto
                    //mettere “NO” al campo Anz. al 12/94
                    if (panvein == 0 && !datiPensione.RequisitiAl1294.HasValue)
                        datiPensione.RequisitiAl1294 = false;
                    //Se il campo PANVEIN è = a zero e il campo Anz. al 09/96 è vuoto
                    //mettere “NO” al campo Anz. al 09/96
                    if (panvein == 0 && !datiPensione.RequisitiAl996.HasValue)
                        datiPensione.RequisitiAl996 = false;
                    //Utilizzare la tabella sopra riportata per valorizzare il campo
                    //RAD411
                    gp1axe3 = GetGP1AXE3byReqAnzVecch(datiPensione.RequisitiVecchiaiaAl1294, datiPensione.RequisitiAl1294, datiPensione.RequisitiAl996);
                }
            }
            return gp1axe3;
        }

        private static void ValorizzaPagamento(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, Utility.TipoDomanda tipoDomanda,
            out Data.CAREPET.Pagamento pagamento)
        {
            pagamento = new Data.CAREPET.Pagamento();

            if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione && !contenitore.IsRiaperturaDomanda))
                if (contenitore.DatiPagamento != null)
                {
                    pagamento.T_GP1CABI_V = contenitore.DatiPagamento.ABI.HasValue ? contenitore.DatiPagamento.ABI.Value : 0;
                    if (contenitore.DatiPagamento.TipoPagamento.HasValue && contenitore.DatiPagamento.TipoPagamento.Value == 'P' && contenitore.DatiPagamento.ABI.GetValueOrDefault() == 07601)
                        pagamento.T_GP1CCAB_V = contenitore.DatiPagamento.Frazionario.HasValue ? contenitore.DatiPagamento.Frazionario.Value : 0;
                    else
                        pagamento.T_GP1CCAB_V = contenitore.DatiPagamento.CAB.HasValue ? contenitore.DatiPagamento.CAB.Value : 0;

                    pagamento.T_GP1CTIPPAG_V = contenitore.DatiPagamento.ModalitaPagamento.HasValue ? contenitore.DatiPagamento.ModalitaPagamento.Value.ToString() : "";
                    //pagamento.T_GP1CNCC_V = contenitore.DatiPagamento.Libretto;
                }

            if (contenitore.DatiEliminazione != null)
            {
                if (contenitore.DatiEliminazione.CodiceMotivo.HasValue)
                {
                    string codiceMotivo = contenitore.DatiEliminazione.CodiceMotivo.Value.ToString();
                    GestioneDecodifica.CodiceEliminazione codiceEliminazione = contenitoreDecodifica.ElencoCodiceEliminazione.Find(x => x.Id == codiceMotivo);
                    if (codiceEliminazione != null)
                        pagamento.T_GP1AM01_V = codiceEliminazione.TraduzioneSuGP.Value.ToString();
                    else
                        pagamento.T_GP1AM01_V = string.Empty;
                }
                else
                    pagamento.T_GP1AM01_V = string.Empty;
                pagamento.T_GP1AM02A_V = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? (short)contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Year : (short)0;
                pagamento.T_GP1AM02M_V = contenitore.DatiEliminazione.DecorrenzaEliminazione.HasValue ? (short)contenitore.DatiEliminazione.DecorrenzaEliminazione.Value.Month : (short)0;
                pagamento.T_GP1AM03A_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.Year : (short)0;
                pagamento.T_GP1AM03M_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.Month : (short)0;
                pagamento.T_GP1AM03G_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.Day : (short)0;

                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo102", out ctrl);
                if (ctrl != null && ctrl.ValoreControllo == "SI")
                {
                    if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                    (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaVOCRED_CRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria)) &&
                    pagamento.T_GP1AM01_V == "6") //codice motivo 6: ELIMINAZIONE SENZA RATA ESTRATTA
                    {
                        pagamento.T_GP1AM05A_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                        pagamento.T_GP1AM05M_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                        pagamento.T_GP1AM05G_V = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)1 : (short)0;
                    }
                    else
                    {
                        // Il campo GP1AM05AZ deve essere valorizzato con il primo giorno del mese successivo alla Data Evento
                        pagamento.T_GP1AM05A_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.AddMonths(1).Year : (short)0;
                        pagamento.T_GP1AM05M_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.AddMonths(1).Month : (short)0;
                        pagamento.T_GP1AM05G_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)1 : (short)0;
                    }
                }
                else
                {
                    // Il campo GP1AM05AZ deve essere valorizzato con il primo giorno del mese successivo alla Data Evento
                    pagamento.T_GP1AM05A_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.AddMonths(1).Year : (short)0;
                    pagamento.T_GP1AM05M_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)contenitore.DatiEliminazione.DataEvento.Value.AddMonths(1).Month : (short)0;
                    pagamento.T_GP1AM05G_V = contenitore.DatiEliminazione.DataEvento.HasValue ? (short)1 : (short)0;
                }

                pagamento.T_GP1AP2A = contenitore.DatiEliminazione.DataFineCalcoloArretrati.HasValue ? (short)contenitore.DatiEliminazione.DataFineCalcoloArretrati.Value.Year : (short)0;
                pagamento.T_GP1AP2M = contenitore.DatiEliminazione.DataFineCalcoloArretrati.HasValue ? (short)contenitore.DatiEliminazione.DataFineCalcoloArretrati.Value.Month : (short)0;
            }

            if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione) || Utility.IsDomandaBeneficioTerrorismoLegge206_2004(contenitore.DatiPensione))
            {
                if (contenitore.DatiBeneficioVittimeTerrorismo != null)
                {
                    string gp1ac01 = string.Empty;
                    if (contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione.HasValue)
                        gp1ac01 += contenitore.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione.Value.ToString();
                    else
                        gp1ac01 += " ";
                    if (contenitore.DatiBeneficioVittimeTerrorismo.CodiceEvento.HasValue)
                        gp1ac01 += contenitore.DatiBeneficioVittimeTerrorismo.CodiceEvento.Value.ToString();
                    else
                        gp1ac01 += " ";
                    if (contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio.HasValue)
                        gp1ac01 += contenitore.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio.Value.ToString();
                    else
                        gp1ac01 += " ";

                    pagamento.T_GP1AC01_V = gp1ac01;
                }
            }
        }

        private static void ValorizzaStatoCivile(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.StatoCivile statoCivile)
        {
            statoCivile = new Data.CAREPET.StatoCivile();

            if (contenitore.DatiAreaTitolare.ElencoStatiCivili != null && contenitore.DatiAreaTitolare.ElencoStatiCivili.Count > 0)
            {
                statoCivile.LISTT_GP2KM7A = new List<Data.CAREPET.StatoCivile.T_GP2KM7A>();
                foreach (GestioneAnagrafica.DatiStatoCivile stCiv in contenitore.DatiAreaTitolare.ElencoStatiCivili)
                {
                    Data.CAREPET.StatoCivile.T_GP2KM7A t_GP2KM7A = new Data.CAREPET.StatoCivile.T_GP2KM7A();
                    t_GP2KM7A.T_GP2KM72A = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Year : (short)0;
                    t_GP2KM7A.T_GP2KM72M = stCiv.Decorrenza.HasValue ? (short)stCiv.Decorrenza.Value.Month : (short)0;
                    t_GP2KM7A.T_GP2KM76 = stCiv.Codice.ToString();
                    statoCivile.LISTT_GP2KM7A.Add(t_GP2KM7A);
                }
            }
        }

        private static void ValorizzaSentenze(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Sentenze sentenze)
        {
            sentenze = new Data.CAREPET.Sentenze();
            sentenze.T_GP1AXE1_V = contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.CodRicalcoloSentenza.HasValue ?
                contenitore.DatiPensioniDatiGenerici.CodRicalcoloSentenza.Value : (short)0;
            if (contenitore.ListaDatiSentenze != null && contenitore.ListaDatiSentenze.Count > 0)
            {
                List<Data.CAREPET.Sentenze.T_GP2SEN0> listt_gp2sen0 = new List<Data.CAREPET.Sentenze.T_GP2SEN0>();
                foreach (GestioneSentenze.DatiSentenze sen in contenitore.ListaDatiSentenze)
                {
                    Data.CAREPET.Sentenze.T_GP2SEN0 gp2sen0 = new Data.CAREPET.Sentenze.T_GP2SEN0();
                    gp2sen0.T_GP2SEN1 = sen.CodSentenzaMerito;
                    gp2sen0.T_GP2SEN2 = sen.CodSentenza;
                    gp2sen0.T_GP2SEN3A = sen.DecorrenzaDal.HasValue ? (short)sen.DecorrenzaDal.Value.Year : (short)0;
                    gp2sen0.T_GP2SEN3M = sen.DecorrenzaDal.HasValue ? (short)sen.DecorrenzaDal.Value.Month : (short)0;
                    gp2sen0.T_GP2SEN4A = sen.DecorrenzaAl.HasValue ? (short)sen.DecorrenzaAl.Value.Year : (short)9999;
                    gp2sen0.T_GP2SEN4M = sen.DecorrenzaAl.HasValue ? (short)sen.DecorrenzaAl.Value.Month : (short)99;
                    listt_gp2sen0.Add(gp2sen0);
                }
                sentenze.LISTT_GP2SEN0 = listt_gp2sen0;
            }
        }

        private static void ValorizzaINAIL_Accompagnamento(ref EntityBLCommon.ContenitoreObject contenitore, out DateTime? inail_CessazioneAssegnoAccompangamento,
            out Data.CAREPET.INAIL_Accompagnamento inail_Accompagnamento)
        {
            inail_CessazioneAssegnoAccompangamento = null;
            inail_Accompagnamento = new Data.CAREPET.INAIL_Accompagnamento();

            if (contenitore.ListaDatiPensioniINAIL != null && contenitore.ListaDatiPensioniINAIL.Count > 0)
            {
                inail_Accompagnamento.LISTT_GP2BINA = new List<Data.CAREPET.INAIL_Accompagnamento.T_GP2BINA>();
                foreach (GestionePensioneInailInabilita.DatiPensioniINAIL iN in contenitore.ListaDatiPensioniINAIL)
                {
                    Data.CAREPET.INAIL_Accompagnamento.T_GP2BINA inail = new Data.CAREPET.INAIL_Accompagnamento.T_GP2BINA();

                    inail.T_GP2BIN1A = iN.DecorrenzaRenditaInail.HasValue ? (short)iN.DecorrenzaRenditaInail.Value.Year : (short)0;
                    inail.T_GP2BIN1M = iN.DecorrenzaRenditaInail.HasValue ? (short)iN.DecorrenzaRenditaInail.Value.Month : (short)0;
                    inail.T_GP2BIN2 = iN.ImportoMensileInail.HasValue ? iN.ImportoMensileInail.Value : 0M;
                    inail.T_GP2BIN3 = iN.Evento.HasValue ? iN.Evento.Value ? (short)1 : (short)0 : (short)0;
                    inail_Accompagnamento.LISTT_GP2BINA.Add(inail);
                }
            }

            if (contenitore.DatiInabilita != null)
            {
                inail_Accompagnamento.T_GP2BACCA = contenitore.DatiInabilita.DecorrenzaAssegnoAccompangamento.HasValue ? (short)contenitore.DatiInabilita.DecorrenzaAssegnoAccompangamento.Value.Year : (short)0;
                inail_Accompagnamento.T_GP2BACCM = contenitore.DatiInabilita.DecorrenzaAssegnoAccompangamento.HasValue ? (short)contenitore.DatiInabilita.DecorrenzaAssegnoAccompangamento.Value.Month : (short)0;

                inail_CessazioneAssegnoAccompangamento = contenitore.DatiInabilita.CessazioneAssegnoAccompangamento;
            }
        }

        private static void ValorizzaPensioniAbbinate(out Data.CAREPET.PensioniAbbinate pensioniAbbinate)
        {
            pensioniAbbinate = new Data.CAREPET.PensioniAbbinate();
        }

        private static void ValorizzaResidenzeEstero(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.ResidenzeEstero residenzeEstero)
        {
            residenzeEstero = new Data.CAREPET.ResidenzeEstero();

            if (contenitore.DatiAreaTitolare.ElencoResidenzeEstere != null && contenitore.DatiAreaTitolare.ElencoResidenzeEstere.Count > 0)
            {
                residenzeEstero.LISTT_GP2BS00 = new List<Data.CAREPET.ResidenzeEstero.T_GP2BS00>();
                foreach (GestioneAnagrafica.DatiResidenzaEstero resEst in contenitore.DatiAreaTitolare.ElencoResidenzeEstere)
                {
                    Data.CAREPET.ResidenzeEstero.T_GP2BS00 t_GP2BS00 = new Data.CAREPET.ResidenzeEstero.T_GP2BS00();
                    t_GP2BS00.T_GP2BS01A = resEst.Decorrenza.HasValue ? (short)resEst.Decorrenza.Value.Year : (short)0;
                    t_GP2BS00.T_GP2BS01M = resEst.Decorrenza.HasValue ? (short)resEst.Decorrenza.Value.Month : (short)0;
                    if (resEst.CodCatastaleStatoEE == "Z000")
                        t_GP2BS00.T_GP2BS02 = "I";
                    else
                    {
                        GestioneDecodifica.StatoEstero statoEstero = null;
                        GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(resEst.CodCatastaleStatoEE, out statoEstero);
                        if (statoEstero != null)
                            t_GP2BS00.T_GP2BS02 = statoEstero.Sigla;
                    }
                    residenzeEstero.LISTT_GP2BS00.Add(t_GP2BS00);
                }
            }
        }

        private static void ValorizzaDanteCausa(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out Data.CAREPET.DanteCausa danteCausa)
        {
            danteCausa = new Data.CAREPET.DanteCausa();
            if (contenitore.DatiAnagraficiDanteCausa != null)
            {
                danteCausa.T_GP7LC11 = contenitore.DatiAnagraficiDanteCausa.Cognome;
                danteCausa.T_GP7LC21 = contenitore.DatiAnagraficiDanteCausa.Nome;
                danteCausa.T_GP7LC01 = !string.IsNullOrEmpty(contenitore.DatiAnagraficiDanteCausa.CodiceFiscale) && contenitore.DatiAnagraficiDanteCausa.CodiceFiscale.Contains("DANTEC_") ? string.Empty : contenitore.DatiAnagraficiDanteCausa.CodiceFiscale;
                danteCausa.T_GP7LC31 = contenitore.DatiAnagraficiDanteCausa.Sesso.HasValue ? contenitore.DatiAnagraficiDanteCausa.Sesso.Value.ToString() : "";
                danteCausa.T_GP7LC41A = contenitore.DatiAnagraficiDanteCausa.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataNascita.Value.Year : (short)0;


                if (contenitore.DatiAnagraficiDanteCausa.DataNascita.HasValue && contenitore.DatiAnagraficiDanteCausa.DataNascita.Value.Minute == 1)
                    danteCausa.T_GP7LC41M = (short)0;
                else
                    danteCausa.T_GP7LC41M = contenitore.DatiAnagraficiDanteCausa.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataNascita.Value.Month : (short)0;

                if (contenitore.DatiAnagraficiDanteCausa.DataNascita.HasValue && contenitore.DatiAnagraficiDanteCausa.DataNascita.Value.Second == 1)
                    danteCausa.T_GP7LC41G = (short)0;
                else
                    danteCausa.T_GP7LC41G = contenitore.DatiAnagraficiDanteCausa.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataNascita.Value.Day : (short)0;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(contenitore.DatiAnagraficiDanteCausa.CodiceComuneNascita, Utility.TipoAppartenenza.AGO.ToString(), 0, false, out codiceInpsComune);
                danteCausa.T_GP7LC51 = codiceInpsComune;
                if (!string.IsNullOrEmpty(contenitore.DatiAnagraficiDanteCausa.Cittadinanza))
                {
                    if (contenitoreDecodifica.ElencoStatoEstero != null)
                    {
                        string app = contenitore.DatiAnagraficiDanteCausa.Cittadinanza;
                        GestioneDecodifica.StatoEstero statoEstero = contenitoreDecodifica.ElencoStatoEstero.Find(x => x.CodCatastale == app);
                        if (statoEstero != null)
                            danteCausa.T_GP7LH01 = !string.IsNullOrEmpty(statoEstero.Sigla) ? statoEstero.Sigla.Trim() == "ITA" ? "I" : statoEstero.Sigla.Trim() : string.Empty;
                    }
                }
                if (contenitore.DatiDanteCausa != null)
                {
                    danteCausa.T_GP7LC03A = contenitore.DatiDanteCausa.DataMorte.HasValue ? (short)contenitore.DatiDanteCausa.DataMorte.Value.Year : (short)0;

                    if (contenitore.DatiDanteCausa.DataMorte.HasValue && contenitore.DatiDanteCausa.DataMorte.Value.Minute == 1)
                        danteCausa.T_GP7LC03M = (short)0;
                    else
                        danteCausa.T_GP7LC03M = contenitore.DatiDanteCausa.DataMorte.HasValue ? (short)contenitore.DatiDanteCausa.DataMorte.Value.Month : (short)0;

                    if (contenitore.DatiDanteCausa.DataMorte.HasValue && contenitore.DatiDanteCausa.DataMorte.Value.Second == 1)
                        danteCausa.T_GP7LC03G = (short)0;
                    else
                        danteCausa.T_GP7LC03G = contenitore.DatiDanteCausa.DataMorte.HasValue ? (short)contenitore.DatiDanteCausa.DataMorte.Value.Day : (short)0;
                    short resShort = 0;
                    if (!string.IsNullOrEmpty(contenitore.DatiDanteCausa.SiglaCategoria))
                    {
                        string codCat = "";
                        GestioneDecodifica.GetCodCategoriaBySiglaCategoria(contenitore.DatiDanteCausa.SiglaCategoria, out codCat);
                        short.TryParse(codCat, out resShort);
                        danteCausa.T_GP7LB01 = resShort;
                    }
                    short.TryParse(contenitore.DatiDanteCausa.Sede, out resShort);
                    danteCausa.T_GP7LB02 = resShort;
                    danteCausa.T_GP7LB03 = contenitore.DatiDanteCausa.Certificato.HasValue ? contenitore.DatiDanteCausa.Certificato.Value : 0;
                    danteCausa.T_GP7LC02A = contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                    danteCausa.T_GP7LC02M = contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                    danteCausa.T_GP7LACQA = contenitore.DatiDanteCausa.DecorrenzaAltraPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaAltraPensione.Value.Year : (short)0;
                    danteCausa.T_GP7LACQM = contenitore.DatiDanteCausa.DecorrenzaAltraPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaAltraPensione.Value.Month : (short)0;
                    if (contenitore.DatiDanteCausa.ProvenienzaPensione.HasValue)
                    {
                        short.TryParse(contenitore.DatiDanteCausa.ProvenienzaPensione.Value.ToString(), out resShort);
                        danteCausa.T_GP7LC04 = resShort;
                    }
                    if (contenitore.DatiDanteCausa.CodiceTipoPensione.HasValue)
                    {
                        string codTipoPensione = contenitore.DatiDanteCausa.CodiceTipoPensione.Value.ToString();
                        if (contenitoreDecodifica.ElencoTipoCalcolo != null && contenitoreDecodifica.ElencoTipoCalcolo.Exists(x => x.Id == codTipoPensione))
                            danteCausa.T_GP7LC19 = contenitoreDecodifica.ElencoTipoCalcolo.FirstOrDefault(x => x.Id == codTipoPensione).TraduzioneSuGP.ToString();
                    }
                    danteCausa.T_GP7LC29 = contenitore.DatiDanteCausa.CodiceBeneficiLegge.HasValue ? (short)contenitore.DatiDanteCausa.CodiceBeneficiLegge.Value : (short)0;
                    if (contenitore.DatiDanteCausa.Maggiorazione781Contributi.HasValue)
                    {
                        short.TryParse(contenitore.DatiDanteCausa.Maggiorazione781Contributi.Value.ToString(), out resShort);
                        danteCausa.T_GP7LC39 = resShort;
                    }
                    if (!string.IsNullOrEmpty(contenitore.DatiDanteCausa.CategoriaAltraPensione))
                    {
                        resShort = 0;
                        short.TryParse(contenitore.DatiDanteCausa.CategoriaAltraPensione, out resShort);
                        danteCausa.T_GP7LCAT = resShort != 0 ? resShort.ToString().PadLeft(3, '0') : contenitore.DatiDanteCausa.CategoriaAltraPensione.PadLeft(3, ' ');
                    }
                    danteCausa.T_GP7LCESA = contenitore.DatiDanteCausa.CessazioneAltraPensione.HasValue ? (short)contenitore.DatiDanteCausa.CessazioneAltraPensione.Value.Year : (short)0;
                    danteCausa.T_GP7LCESM = contenitore.DatiDanteCausa.CessazioneAltraPensione.HasValue ? (short)contenitore.DatiDanteCausa.CessazioneAltraPensione.Value.Month : (short)0;
                    danteCausa.T_GP7LCIM = contenitore.DatiDanteCausa.CodiceImportoAltraPensione.HasValue ? contenitore.DatiDanteCausa.CodiceImportoAltraPensione.Value.ToString() : string.Empty;
                    danteCausa.T_GP7LCUC = contenitore.DatiDanteCausa.CodiceUCAltraPensione.HasValue ? contenitore.DatiDanteCausa.CodiceUCAltraPensione.Value.ToString() : string.Empty;
                    danteCausa.T_GP7LE01_V = contenitore.DatiDanteCausa.ImportoPensione311284.HasValue ? contenitore.DatiDanteCausa.ImportoPensione311284.Value : 0M;
                    danteCausa.T_GP7LE02_V = contenitore.DatiDanteCausa.ImportoPensione1185.HasValue ? contenitore.DatiDanteCausa.ImportoPensione1185.Value : 0M;
                    danteCausa.T_GP7LE03_V = contenitore.DatiDanteCausa.ImportoPensione1190.HasValue ? contenitore.DatiDanteCausa.ImportoPensione1190.Value : 0M;
                    danteCausa.T_GP7LE04 = contenitore.DatiDanteCausa.NContributiDiretta.HasValue ? contenitore.DatiDanteCausa.NContributiDiretta.Value : 0;
                    danteCausa.T_GP7LENT = contenitore.DatiDanteCausa.EnteAltraPensione.HasValue ? contenitore.DatiDanteCausa.EnteAltraPensione.Value : (short)0;
                    danteCausa.T_GP7LNPE = contenitore.DatiDanteCausa.NaturaPensioneAltraPensione;
                }
            }
        }

        private static void ValorizzaDatiRetributivi_Contributivi_BIS(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out Data.CAREPET.DatiRetributivi_Contributivi datiRetributivi_Contributivi,
            out Data.CAREPET.DatiRetributiviBIS datiRetributiviBIS)
        {
            datiRetributivi_Contributivi = new Data.CAREPET.DatiRetributivi_Contributivi();
            datiRetributiviBIS = new Data.CAREPET.DatiRetributiviBIS();

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true);

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiCalcoloRetributivoENPALS != null && !contenitore.DatiCalcoloRetributivoENPALS.IsDatiCalcoloRetributivoEnpalsNull())
                {
                    datiRetributivi_Contributivi.LISTT_GP2BC00 = new List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00>();
                    datiRetributiviBIS.LISTT_GP2BC00_BIS = new List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS>();

                    if (contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaA.HasValue || contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaA.HasValue ||
                        contenitore.DatiCalcoloRetributivoENPALS.RMQuotaA.HasValue || contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaA.HasValue)
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retrA = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00();
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retrA_bis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS();

                        retrA.T_GP2BC02 = retrA_bis.T_GP2BC02_BIS = contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaA.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaA.Value : 0;
                        retrA.T_GP2BC03 = retrA_bis.T_GP2BC03_BIS = contenitore.DatiCalcoloRetributivoENPALS.RMQuotaA.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.RMQuotaA.Value : 0;

                        retrA.T_GP2BC0B = retrA_bis.T_GP2BC0B_BIS = "A";

                        Utility.DifferenzaDateTime decorrenza = null;

                        if (!string.IsNullOrEmpty(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA) && contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA.Contains('/'))
                        {
                            int year = 0;
                            int month = 0;
                            int day = 0;
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA.Split('/')[2], out year);
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA.Split('/')[1], out month);
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaA.Split('/')[0], out day);

                            decorrenza = new Utility.DifferenzaDateTime(year, month, day);
                        }

                        short meseDec = 0;
                        short annoDec = 0;
                        GetDecorrenzaRetr(string.Empty, 'A', null, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiControlloFelpe, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                            contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOpzione : (DateTime?)null, decorrenza, null, out meseDec, out annoDec);
                        retrA.T_GP2BC01A = retrA_bis.T_GP2BC01A_BIS = annoDec;
                        retrA.T_GP2BC01M = retrA_bis.T_GP2BC01M_BIS = meseDec;

                        retrA.T_GP2BC09 = retrA_bis.T_GP2BC09_BIS = "1 ";
                        retrA.T_GP2BC10 = retrA_bis.T_GP2BC10_BIS = contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaA707.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaA707.Value : 0;

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(retrA);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(retrA_bis);
                    }

                    if (contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaB.HasValue || contenitore.DatiCalcoloRetributivoENPALS.NTotaleContributiCalcoloQuotaB.HasValue ||
                        contenitore.DatiCalcoloRetributivoENPALS.RMQuotaB.HasValue || contenitore.DatiCalcoloRetributivoENPALS.ImportoQuotaB.HasValue)
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retrB = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00();
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retrB_bis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS();

                        retrB.T_GP2BC02 = retrB_bis.T_GP2BC02_BIS = contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaB.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.PeriodiQuotaB.Value : 0;
                        retrB.T_GP2BC03 = retrB_bis.T_GP2BC03_BIS = contenitore.DatiCalcoloRetributivoENPALS.RMQuotaB.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.RMQuotaB.Value : 0;

                        retrB.T_GP2BC0B = retrB_bis.T_GP2BC0B_BIS = "B";

                        Utility.DifferenzaDateTime decorrenza = null;

                        if (!string.IsNullOrEmpty(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB) && contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB.Contains('/'))
                        {
                            int year = 0;
                            int month = 0;
                            int day = 0;
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB.Split('/')[2], out year);
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB.Split('/')[1], out month);
                            int.TryParse(contenitore.DatiCalcoloRetributivoENPALS.DecorrenzaQuotaB.Split('/')[0], out day);

                            decorrenza = new Utility.DifferenzaDateTime(year, month, day);
                        }

                        short meseDec = 0;
                        short annoDec = 0;
                        GetDecorrenzaRetr(string.Empty, 'B', null, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiControlloFelpe, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                            contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOpzione : (DateTime?)null, decorrenza, null, out meseDec, out annoDec);
                        retrB.T_GP2BC01A = retrB_bis.T_GP2BC01A_BIS = annoDec;
                        retrB.T_GP2BC01M = retrB_bis.T_GP2BC01M_BIS = meseDec;

                        retrB.T_GP2BC09 = retrB_bis.T_GP2BC09_BIS = "1 ";
                        retrB.T_GP2BC10 = retrB_bis.T_GP2BC10_BIS = contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaB707.HasValue ? contenitore.DatiCalcoloRetributivoENPALS.GiorniQuotaB707.Value : 0;

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(retrB);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(retrB_bis);
                    }

                    if (contenitore.IsRiaperturaDomanda)
                    {
                        if (contenitore.DatiCalcoloRetributivoENPALS.Equals(contenitore.DatiCalcoloRetributivoENPALSStorico) &&
                            contenitore.DatiCalcoloContributivoENPALS != null && contenitore.DatiCalcoloContributivoENPALS.Equals(contenitore.DatiCalcoloContributivoENPALSStorico))
                        {
                            datiRetributivi_Contributivi.LISTT_GP2BC00.First().T_GP2BC03 += 0.01m;
                            datiRetributiviBIS.LISTT_GP2BC00_BIS.First().T_GP2BC03_BIS += 0.01m;
                        }
                    }
                }
            }
            else
            {
                if (contenitore.ListaDatiRetributivi != null && contenitore.ListaDatiRetributivi.Count > 0)
                {
                    datiRetributivi_Contributivi.LISTT_GP2BC00 = new List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00>();
                    datiRetributiviBIS.LISTT_GP2BC00_BIS = new List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS>();

                    bool abilitaNuovoFlusso = IsFlussoAdeguata(contenitoreDecodifica.ElencoCtrlCatAdeguata, contenitore.DatiPensione.SiglaCategoria != null ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty, contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto, contenitore.DatiPensione.Tipo, Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda));
                    bool? variazioneDatiCalcolo = false;

                    if (abilitaNuovoFlusso) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo cR in contenitore.ListaDatiRetributivi)
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retr = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00();
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retr_bis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS();

                        if (cR.QuotePrimeLiquidate.HasValue && cR.QuotePrimeLiquidate.Value == 'A')
                        {
                            retr.T_GP2BC02 = retr_bis.T_GP2BC02_BIS = cR.NSettimaneQuotaA.HasValue ? cR.NSettimaneQuotaA.Value : 0;
                            retr.T_GP2BC03 = retr_bis.T_GP2BC03_BIS = cR.RMSQuotaA.HasValue ? cR.RMSQuotaA.Value : 0M;
                        }
                        else if (cR.QuotePrimeLiquidate.HasValue && cR.QuotePrimeLiquidate.Value == 'B')
                        {
                            retr.T_GP2BC02 = retr_bis.T_GP2BC02_BIS = cR.NSettimaneQuotaB.HasValue ? cR.NSettimaneQuotaB.Value : 0;
                            retr.T_GP2BC03 = retr_bis.T_GP2BC03_BIS = cR.RMSQuotaB.HasValue ? cR.RMSQuotaB.Value : 0M;
                        }
                        string codiceGestione = string.Empty;
                        if (cR.CodiceGestione.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Find(x => x.Id == cR.CodiceGestione.Value && !x.IsFondo);
                                if (codeGestioneCalcoloRetributivo != null)
                                    retr.T_GP2BC09 = retr_bis.T_GP2BC09_BIS = codiceGestione = codeGestioneCalcoloRetributivo.TraduzioneSuGP.PadLeft(2, ' ');
                            }
                        }

                        short meseDec = 0;
                        short annoDec = 0;
                        if (contenitore.DatiPensione.DecorrenzaOriginaria.HasValue)
                        {
                            GetDecorrenzaRetr(retr.T_GP2BC09.Trim(), cR.QuotePrimeLiquidate.HasValue ? cR.QuotePrimeLiquidate.Value : ' ', cR.CodiceTipoQuota,
                                contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.DatiControlloFelpe, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI,
                                contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.DecorrenzaOpzione : (DateTime?)null,
                                cR.DecorrenzaOriginariaPensione.HasValue ? new Utility.DifferenzaDateTime(cR.DecorrenzaOriginariaPensione.Value) : null, cR.RMS,
                                out meseDec, out annoDec);
                        }

                        //ENG - SOPGI: passare nel campo GP2BC01Z la decorrenza della pensione del dante causa
                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaReversibilita(contenitore.DatiPensione))
                        {
                            retr.T_GP2BC01A = retr_bis.T_GP2BC01A_BIS = contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                        }
                        else
                        {
                            retr.T_GP2BC01A = retr_bis.T_GP2BC01A_BIS = annoDec;
                        }

                        //specializzazione ante 96
                        if (cR.DecorrenzaOriginariaPensione.HasValue && (cR.DecorrenzaOriginariaPensione.Value.Second == 28 || cR.DecorrenzaOriginariaPensione.Value.Second == 30))
                            meseDec = (short)(cR.DecorrenzaOriginariaPensione.Value.Second == 28 ? 88 : (cR.DecorrenzaOriginariaPensione.Value.Second == 30 ? 90 : meseDec));

                        //ENG - SOPGI: passare nel campo GP2BC01Z la decorrenza della pensione del dante causa
                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaReversibilita(contenitore.DatiPensione))
                        {
                            retr.T_GP2BC01M = retr_bis.T_GP2BC01M_BIS = contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                        }
                        else
                        {
                            retr.T_GP2BC01M = retr_bis.T_GP2BC01M_BIS = meseDec;
                        }

                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) &&
                            (Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                             Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo)))
                            retr.T_GP2BC0B = retr_bis.T_GP2BC0B_BIS = MappingQuotaPerVittimeTerrorismo(cR.CodiceTipoQuota, cR.QuotePrimeLiquidate, codiceGestione, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI);
                        else
                            retr.T_GP2BC0B = retr_bis.T_GP2BC0B_BIS = cR.QuotePrimeLiquidate.HasValue ? cR.QuotePrimeLiquidate.Value.ToString() : string.Empty;

                        //retr.T_GP2BC08 = retr_bis.T_GP2BC08_BIS = cR.NSettimane707.HasValue ? cR.NSettimane707.Value : 0;
                        //ENG - Aggiornamento Memo 68/2022 IOPGI
                        //ENG - Spacchettate SOPGI
                        if (!Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione)) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                            retr.T_GP2BC10 = retr_bis.T_GP2BC10_BIS = cR.NSettimane707.HasValue ? cR.NSettimane707.Value : 0;
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                        {
                            if (cR.QuotePrimeLiquidate == 'A')
                                retr.T_GP2BC0A = retr_bis.T_GP2BC0A_BIS = 1;
                            else if (cR.QuotePrimeLiquidate == 'B')
                                retr.T_GP2BC0A = retr_bis.T_GP2BC0A_BIS = 2;
                        }
                        if (abilitaNuovoFlusso)
                        {
                            if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                            {
                                //Per le manuali, invio solo TFR/RIC se non ci sono state variazioni nei dati calcolo
                                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !variazioneDatiCalcolo.GetValueOrDefault())
                                {
                                    retr.T_GP2BC0D = retr_bis.T_GP2BC0D_BIS = cR.PL_Quotar.HasValue ? cR.PL_Quotar.Value : 0M;
                                    retr_bis.T_GP2BC0F_BIS = cR.PL_Quotar707.HasValue ? cR.PL_Quotar707.Value : 0M;
                                }
                            }
                            //Per le automatiche invio sempre
                            else
                            {
                                retr.T_GP2BC0D = retr_bis.T_GP2BC0D_BIS = cR.PL_Quotar.HasValue ? cR.PL_Quotar.Value : 0M;
                                retr_bis.T_GP2BC0F_BIS = cR.PL_Quotar707.HasValue ? cR.PL_Quotar707.Value : 0M;
                            }
                        }

                        //ENG - Aggiornamento Memo 68/2022 IOPGI
                        //ENG - Spacchettate SOPGI
                        if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                            || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                        {
                            retr.T_GP2BC0D = retr_bis.T_GP2BC0D_BIS = cR.PL_Quotar.HasValue ? cR.PL_Quotar.Value : 0M;
                            //retr_bis.T_GP2BC0F_BIS = cR.PL_Quotar707.HasValue ? cR.PL_Quotar707.Value : 0M;
                        }

                        //campi nuovi ante96
                        retr.T_GP2BC08 = retr_bis.T_GP2BC08_BIS = cR.NSettAnzianitaVV.HasValue ? cR.NSettAnzianitaVV.Value : 0;
                        retr.T_GP2BC04 = retr_bis.T_GP2BC04_BIS = cR.NSettimaneExCombattente.HasValue ? cR.NSettimaneExCombattente.Value : 0;
                        retr.T_GP2BC05 = retr_bis.T_GP2BC05_BIS = cR.RMSExCombattente.HasValue ? cR.RMSExCombattente.Value : 0M;

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(retr);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(retr_bis);

                    }
                    if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault() && Utility.DataStrettamenteSuccessivaA(new DateTime(1997, 01, 01), contenitore.DatiPensione.DecorrenzaOriginaria.GetValueOrDefault()))
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 nuovoElemento = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00()
                        {
                            T_GP2BC01A = 2003,
                            T_GP2BC01M = 12,
                            T_GP2BC09 = "A",
                            T_GP2BC0B = "R",
                            T_GP2BC0D = contenitore.DatiPensioniDatiGenerici.ImportoAl200312.GetValueOrDefault()
                        };
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS nuovoElementoBis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS
                        {
                            T_GP2BC01A_BIS = 2003,
                            T_GP2BC01M_BIS = 12,
                            T_GP2BC09_BIS = "A",
                            T_GP2BC0B_BIS = "R",
                            T_GP2BC0D_BIS = contenitore.DatiPensioniDatiGenerici.ImportoAl200312.GetValueOrDefault()
                        };

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(nuovoElemento);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(nuovoElementoBis);
                    }
                    if (contenitore.IsRiaperturaDomanda)
                    {
                        if (variazioneDatiCalcolo == null) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);

                        if (!variazioneDatiCalcolo.GetValueOrDefault())
                        {
                            datiRetributivi_Contributivi.LISTT_GP2BC00.First().T_GP2BC03 += 0.01m;
                            datiRetributiviBIS.LISTT_GP2BC00_BIS.First().T_GP2BC03_BIS += 0.01m;
                        }
                    }

                }
            }

            if (Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
            {
                if (contenitore.ListaDatiCalcoloVittimeTerrorismo != null && contenitore.ListaDatiCalcoloVittimeTerrorismo.Count > 0)
                {
                    if (datiRetributivi_Contributivi.LISTT_GP2BC00 == null)
                    {
                        datiRetributivi_Contributivi.LISTT_GP2BC00 = new List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00>();
                        datiRetributiviBIS.LISTT_GP2BC00_BIS = new List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS>();
                    }
                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'R'))
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retr = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00();
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retr_bis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS();

                        retr.T_GP2BC01A = retr_bis.T_GP2BC01A_BIS = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Year : (short)0;
                        retr.T_GP2BC01M = retr_bis.T_GP2BC01M_BIS = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Month : (short)0;

                        string codiceGestione = string.Empty;
                        if (datiCalcoloVittimeTerrorismo.CodiceGestioneRetr.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Find(x => x.Id == datiCalcoloVittimeTerrorismo.CodiceGestioneRetr.Value && !x.IsFondo);
                                if (codeGestioneCalcoloRetributivo != null)
                                    codiceGestione = codeGestioneCalcoloRetributivo.TraduzioneSuGP;
                            }
                        }
                        retr.T_GP2BC02 = retr_bis.T_GP2BC02_BIS = datiCalcoloVittimeTerrorismo.Settimane.HasValue ? datiCalcoloVittimeTerrorismo.Settimane.Value : 0;
                        retr.T_GP2BC03 = retr_bis.T_GP2BC03_BIS = datiCalcoloVittimeTerrorismo.RMS.HasValue ? datiCalcoloVittimeTerrorismo.RMS.Value : 0M;
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                        {
                            retr.T_GP2BC0B = retr_bis.T_GP2BC0B_BIS = MappingQuotaPerVittimeTerrorismo(datiCalcoloVittimeTerrorismo.CodiceTipoQuota, datiCalcoloVittimeTerrorismo.Quota, codiceGestione, contenitoreDecodifica.ElencoCtrlDecorrenzaRetrExINPDAI);
                            retr.T_GP2BC09 = retr_bis.T_GP2BC09_BIS = !string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1).PadLeft(2, ' ') : "  ";
                            retr.T_GP2BC0C = retr_bis.T_GP2BC0C_BIS = datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ";
                        }
                        else
                        {
                            retr.T_GP2BC0B = retr_bis.T_GP2BC0B_BIS = datiCalcoloVittimeTerrorismo.Quota.HasValue ? datiCalcoloVittimeTerrorismo.Quota.ToString() : string.Empty;
                            retr.T_GP2BC09 = retr_bis.T_GP2BC09_BIS = (!string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1) : " ") +
                                (datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ");
                        }
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) ||
                            (!string.IsNullOrEmpty(codiceGestione) && new List<string> { "1", "2", "3", "4", "A" }.Contains(codiceGestione.Trim()) &&
                            (datiCalcoloVittimeTerrorismo.Beneficio.GetValueOrDefault() == 'Y' || datiCalcoloVittimeTerrorismo.Beneficio.GetValueOrDefault() == 'W')))
                        {
                            if (datiCalcoloVittimeTerrorismo.Quota == 'A')
                                retr.T_GP2BC0A = retr_bis.T_GP2BC0A_BIS = 1;
                            else if (datiCalcoloVittimeTerrorismo.Quota == 'B')
                                retr.T_GP2BC0A = retr_bis.T_GP2BC0A_BIS = 2;
                        }

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(retr);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(retr_bis);
                    }
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
            {
                if (contenitore.ListaDatiRetributiviINPGI != null && contenitore.ListaDatiRetributiviINPGI.Count > 0)
                {
                    if (datiRetributivi_Contributivi.LISTT_GP2BC00 == null)
                    {
                        datiRetributivi_Contributivi.LISTT_GP2BC00 = new List<Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00>();
                        datiRetributiviBIS.LISTT_GP2BC00_BIS = new List<Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS>();
                    }

                    foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI cR in contenitore.ListaDatiRetributiviINPGI)
                    {
                        Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00 retr = new Data.CAREPET.DatiRetributivi_Contributivi.T_GP2BC00();
                        Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS retr_bis = new Data.CAREPET.DatiRetributiviBIS.T_GP2BC00_BIS();

                        //ENG - SOPGI: passare nel campo GP2BC01Z la decorrenza della pensione del dante causa
                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) && Utility.IsDomandaReversibilita(contenitore.DatiPensione))
                        {
                            retr.T_GP2BC01A = retr_bis.T_GP2BC01A_BIS = contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                            retr.T_GP2BC01M = retr_bis.T_GP2BC01M_BIS = contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                        }
                        else
                        {
                            retr.T_GP2BC01A = retr_bis.T_GP2BC01A_BIS = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                            retr.T_GP2BC01M = retr_bis.T_GP2BC01M_BIS = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                        }
                        retr.T_GP2BC02 = retr_bis.T_GP2BC02_BIS = cR.Settimane.HasValue ? cR.Settimane.Value : 0;
                        //retr.T_GP2BC03 = retr_bis.T_GP2BC03_BIS = 1M;
                        retr.T_GP2BC0D = retr_bis.T_GP2BC0D_BIS = cR.ImportoCalcolato.HasValue ? cR.ImportoCalcolato.Value : 0M;
                        retr.T_GP2BC10 = retr_bis.T_GP2BC10_BIS = cR.SettimaneComma707.HasValue ? cR.SettimaneComma707.Value : 0;
                        retr_bis.T_GP2BC0F_BIS = cR.ImportoComma707.HasValue ? cR.ImportoComma707.Value : 0M;
                        retr.T_GP2BC03 = retr_bis.T_GP2BC03_BIS = cR.RetribuzioneMediaSettimanale.HasValue ? cR.RetribuzioneMediaSettimanale.Value : 0M;

                        if (cR.CodiceGestione.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI != null && contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneQuotaFondoINPGI codeGestioneCalcoloRetributivoINPGI = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Find(x => x.Id == cR.CodiceGestione.Value);
                                if (codeGestioneCalcoloRetributivoINPGI != null)
                                    retr.T_GP2BC09 = retr_bis.T_GP2BC09_BIS = codeGestioneCalcoloRetributivoINPGI.TraduzioneSuGP.PadLeft(2, ' ');
                            }
                        }

                        datiRetributivi_Contributivi.LISTT_GP2BC00.Add(retr);
                        datiRetributiviBIS.LISTT_GP2BC00_BIS.Add(retr_bis);
                    }
                }
            }
        }

        private static string MappingQuotaPerVittimeTerrorismo(string codiceTipoQuota, char? quota, string codiceGestione, List<CtrlDecorrenzaRetrExINPDAI> elencoCtrlDecorrenzaRetrExINPDAI)
        {
            string retValue = string.Empty;
            if (!string.IsNullOrEmpty(codiceGestione) && (codiceGestione.Trim() == "A" || codiceGestione.Trim() == "S"))
            {
                if (elencoCtrlDecorrenzaRetrExINPDAI != null && elencoCtrlDecorrenzaRetrExINPDAI.Count > 0 &&
                    elencoCtrlDecorrenzaRetrExINPDAI.Exists(x => x.Gestione.Trim() == codiceGestione.Trim() && x.Quota == quota && x.TipoQuota == codiceTipoQuota))
                {
                    byte? codiceDecorrenza = elencoCtrlDecorrenzaRetrExINPDAI.FirstOrDefault(x => x.Gestione.Trim() == codiceGestione.Trim() && x.Quota == quota && x.TipoQuota == codiceTipoQuota).CodiceDecorrenza;
                    switch (codiceDecorrenza)
                    {
                        case 76:
                            retValue = "1";
                            break;
                        case 21:
                            retValue = "2";
                            break;
                        case 31:
                            retValue = "3";
                            break;
                        case 41:
                            retValue = "4";
                            break;
                        case 51:
                            retValue = "5";
                            break;
                        case 16:
                            retValue = "6";
                            break;
                        case 17:
                            retValue = "7";
                            break;
                        case 91:
                            retValue = "9";
                            break;
                        case 71:
                            retValue = "A";
                            break;
                        case 61:
                            retValue = "B";
                            break;
                        case 72:
                            retValue = "C";
                            break;
                        case 62:
                            retValue = "D";
                            break;
                        case 73:
                            retValue = "E";
                            break;
                        case 63:
                            retValue = "F";
                            break;
                        case 74:
                            retValue = "G";
                            break;
                        case 64:
                            retValue = "H";
                            break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(codiceTipoQuota) && codiceTipoQuota == "B9")
                retValue = "9";
            else if (quota.HasValue)
                retValue = quota.Value.ToString();
            return retValue;
        }

        private static void ValorizzaIntegrazioneArticolo11ByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.IntegrazioneArticolo11 integrazioneArticolo11)
        {
            integrazioneArticolo11 = new Data.CAREPET.IntegrazioneArticolo11();

            if (contenitore.DatiIntegrazioneArt11 != null)
            {
                integrazioneArticolo11.LISTGPINTAR11 = new List<Data.CAREPET.IntegrazioneArticolo11.GPINTAR11>();
                Data.CAREPET.IntegrazioneArticolo11.GPINTAR11 i = new Data.CAREPET.IntegrazioneArticolo11.GPINTAR11();
                i.T_GP2BC06A = contenitore.DatiIntegrazioneArt11.Decorrenza.HasValue ? (short)contenitore.DatiIntegrazioneArt11.Decorrenza.Value.Year : (short)0;
                i.T_GP2BC06M = contenitore.DatiIntegrazioneArt11.Decorrenza.HasValue ? (short)contenitore.DatiIntegrazioneArt11.Decorrenza.Value.Month : (short)0;
                i.T_GP2BC07 = contenitore.DatiIntegrazioneArt11.ImportoIVS.HasValue ? contenitore.DatiIntegrazioneArt11.ImportoIVS.Value : 0M;
                integrazioneArticolo11.LISTGPINTAR11.Add(i);
            }
        }

        private static void ValorizzaPannelloContributivo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            out Data.CAREPET.PannelloContributivo pannelloContributivo)
        {
            pannelloContributivo = new Data.CAREPET.PannelloContributivo();
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true);

            if (contenitore.IsRiaperturaDomanda && (Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || (Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "DAP") || Utility.IsDomandaESPA_L26(contenitore.DatiPensione)))
            {
                if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza.HasValue && contenitore.DatiStoricoGP != null && contenitore.DatiStoricoGP.GP2BB06 == contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza.Value)
                {
                    contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza.Value + 0.01m;
                }
            }

            #region ENPALS
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiEnpals != null)
                {
                    if (contenitore.DatiEnpals.ImportoPensione.HasValue)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        DateTime? decorrenzaDaInviare = null;
                        if (!string.IsNullOrEmpty(contenitore.DatiEnpals.DecorrenzaImportoPensione) && contenitore.DatiEnpals.DecorrenzaImportoPensione.Contains('/'))
                            decorrenzaDaInviare = Utility.DataFromString(contenitore.DatiEnpals.DecorrenzaImportoPensione.Replace("/", ""), Utility.FormatoData.GGmmAAAA);
                        else if (contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue)
                            decorrenzaDaInviare = contenitore.DatiDanteCausa.DecorrenzaPensione;
                        else
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;

                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB05 = "M0";
                        contr.T_GP2BB06 = contenitore.DatiEnpals.ImportoPensione.Value;

                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }

                    if (contenitore.DatiEnpals.ImportoPensione707.HasValue)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        DateTime? decorrenzaDaInviare = null;
                        if (!string.IsNullOrEmpty(contenitore.DatiEnpals.DecorrenzaImportoPensione707) && contenitore.DatiEnpals.DecorrenzaImportoPensione707.Contains('/'))
                            decorrenzaDaInviare = Utility.DataFromString(contenitore.DatiEnpals.DecorrenzaImportoPensione707.Replace("/", ""), Utility.FormatoData.GGmmAAAA);
                        else if (contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue)
                            decorrenzaDaInviare = contenitore.DatiDanteCausa.DecorrenzaPensione;
                        else
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;

                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB05 = "M2";
                        contr.T_GP2BB06 = contenitore.DatiEnpals.ImportoPensione707.Value;

                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }

                    if (contenitore.DatiEnpals.ImportoIIS.HasValue || contenitore.DatiEnpals.DecorrenzaImportoIIS.HasValue)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        contr.T_GP2BB04A = contenitore.DatiEnpals.DecorrenzaImportoIIS.HasValue ? (short)contenitore.DatiEnpals.DecorrenzaImportoIIS.Value.Year : (short)0;
                        contr.T_GP2BB04M = contenitore.DatiEnpals.DecorrenzaImportoIIS.HasValue ? (short)contenitore.DatiEnpals.DecorrenzaImportoIIS.Value.Month : (short)0;
                        contr.T_GP2BB04G = contenitore.DatiEnpals.DecorrenzaImportoIIS.HasValue ? (short)contenitore.DatiEnpals.DecorrenzaImportoIIS.Value.Day : (short)0;
                        contr.T_GP2BB05 = "I1";
                        contr.T_GP2BB09 = contenitore.DatiEnpals.ImportoIIS.HasValue ? contenitore.DatiEnpals.ImportoIIS.Value : 0M;

                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }
                if (contenitore.DatiCalcoloContributivoENPALS != null && !contenitore.DatiCalcoloContributivoENPALS.IsDatiCalcoloContributivoEnpalsNull())
                {
                    if (contenitore.DatiCalcoloContributivoENPALS.Montante.HasValue)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        DateTime? decorrenzaDaInviare = null;
                        if (!string.IsNullOrEmpty(contenitore.DatiCalcoloContributivoENPALS.Decorrenza) && contenitore.DatiCalcoloContributivoENPALS.Decorrenza.Contains('/'))
                            decorrenzaDaInviare = Utility.DataFromString(contenitore.DatiCalcoloContributivoENPALS.Decorrenza.Replace("/", ""), Utility.FormatoData.GGmmAAAA);
                        if (!decorrenzaDaInviare.HasValue)
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;

                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB07 = contenitore.DatiCalcoloContributivoENPALS.ImportoContributivoTotale.HasValue ? contenitore.DatiCalcoloContributivoENPALS.ImportoContributivoTotale.Value : 0;
                        contr.T_GP2BB06 = contenitore.DatiCalcoloContributivoENPALS.Montante.HasValue ? contenitore.DatiCalcoloContributivoENPALS.Montante.Value : 0;
                        contr.T_GP2BB05 = "1 ";

                        if (contenitore.DatiContribuzioneEnpalsSAI != null && contenitore.DatiContribuzioneEnpalsSAI.QuotaC != null)
                        {
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Enpals.GetValueOrDefault();
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Estera.GetValueOrDefault();
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Figurativa.GetValueOrDefault();
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Inps.GetValueOrDefault();
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Ufficio.GetValueOrDefault();
                            contr.T_GP2BB08 += (short)contenitore.DatiContribuzioneEnpalsSAI.QuotaC.Volontaria.GetValueOrDefault();
                        }
                        else
                            contr.T_GP2BB08 = contenitore.DatiCalcoloContributivoENPALS.NumeroContributiTotale.GetValueOrDefault();

                        //26-02-2016 G.Arru - Commentato in seguito a mail (Oggetto: LiqPens AGO - Modifiche urgenti)
                        //if (cC.Quota.GetValueOrDefault() == 'C')
                        //{
                        //    contr.T_GP2BB0B = "C";
                        //    contr.T_GP2BB0A = "3";
                        //}
                        //else 
                        if (contenitore.DatiCalcoloContributivoENPALS.Quota.GetValueOrDefault() == 'D')
                        {
                            contr.T_GP2BB0B = "D";
                            contr.T_GP2BB0A = "4";
                        }

                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }

                if (contenitore.ListaDatiSuppRecordENPALS != null && contenitore.ListaDatiSuppRecordENPALS.Count > 0)
                {
                    foreach (EntityBLCommon.DatiSuppRecordENPALS datiSuppRecordENPALS in contenitore.ListaDatiSuppRecordENPALS)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        contr.T_GP2BB05 = "M1";
                        contr.T_GP2BB09 = datiSuppRecordENPALS.Importo.HasValue ? datiSuppRecordENPALS.Importo.Value : 0M;
                        contr.T_GP2BB04A = datiSuppRecordENPALS.Decorrenza.HasValue ? (short)datiSuppRecordENPALS.Decorrenza.Value.Year : (short)0;
                        contr.T_GP2BB04M = datiSuppRecordENPALS.Decorrenza.HasValue ? (short)datiSuppRecordENPALS.Decorrenza.Value.Month : (short)0;
                        contr.T_GP2BB04G = datiSuppRecordENPALS.Decorrenza.HasValue ? (short)datiSuppRecordENPALS.Decorrenza.Value.Day : (short)0;

                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }

                if (contenitore.ListaDatiSentenzaArt4 != null && contenitore.ListaDatiSentenzaArt4.Count > 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null)
                        pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    foreach (GestioneSentenzaArt4.DatiSentenzaArt4 datiSentenzaArt4 in contenitore.ListaDatiSentenzaArt4)
                    {
                        if (datiSentenzaArt4 != null)
                        {
                            if (datiSentenzaArt4.ImportoSentenza.HasValue)
                            {
                                Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                                DateTime? decorrenzaDaInviare = null;
                                if (datiSentenzaArt4.DecorrenzaSentenza.HasValue)
                                    decorrenzaDaInviare = Utility.DataFromInt(datiSentenzaArt4.DecorrenzaSentenza.Value.Year, datiSentenzaArt4.DecorrenzaSentenza.Value.Month, 1);

                                contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                                contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                                contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                                contr.T_GP2BB05 = "S1";
                                contr.T_GP2BB09 = datiSentenzaArt4.ImportoSentenza.HasValue ? (decimal)datiSentenzaArt4.ImportoSentenza : (decimal)0;

                                if (pannelloContributivo.LISTT_GP2BB03 == null)
                                    pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                                pannelloContributivo.LISTT_GP2BB03.Add(contr);
                            }
                        }
                    }
                }

                if (contenitore.IsRiaperturaDomanda && (contenitore.DatiCalcoloRetributivoENPALS == null || contenitore.DatiCalcoloRetributivoENPALS.IsDatiCalcoloRetributivoEnpalsNull()))
                {
                    if (contenitore.DatiCalcoloContributivoENPALS != null && contenitore.DatiCalcoloContributivoENPALS.Equals(contenitore.DatiCalcoloContributivoENPALSStorico) &&
                            pannelloContributivo.LISTT_GP2BB03.Exists(x => x.T_GP2BB05 == "1 "))
                        pannelloContributivo.LISTT_GP2BB03.FirstOrDefault(x => x.T_GP2BB05 == "1 ").T_GP2BB06 += 0.01m;
                }
            }
            #endregion ENPALS
            #region Cumulo
            else if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                if (contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0)
                {
                    pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();
                    List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
                    if (!contenitore.ListaQuotePensione.Exists(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo) == null))
                        contenitore.ListaQuotePensione = contenitore.ListaQuotePensione.OrderBy(x => x.Decorrenza).ThenBy(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo).Codice).ToList();
                    foreach (GestioneCalcolo.QuotePensione quotePensione in contenitore.ListaQuotePensione)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        if ((Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa)) ||
                            (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault())
                            || (Utility.IsDomandaIOCUM(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaPensioneIndirettaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa))))
                        {
                            contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                            contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                            contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                        }
                        else
                        {
                            if ((Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa)) ||
                                (Utility.IsDomandaSOTOT(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilitaOrRicostituzione(contenitore.DatiPensione, contenitore.DatiDanteCausa)))
                            {
                                contr.T_GP2BB04A = contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Year : (short)0;
                                contr.T_GP2BB04M = contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Month : (short)0;
                                contr.T_GP2BB04G = contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaPensione.Value.Day : (short)0;
                            }
                            else
                            {
                                contr.T_GP2BB04A = quotePensione.Decorrenza.HasValue ? (short)quotePensione.Decorrenza.Value.Year : (short)0;
                                contr.T_GP2BB04M = quotePensione.Decorrenza.HasValue ? (short)quotePensione.Decorrenza.Value.Month : (short)0;
                                contr.T_GP2BB04G = quotePensione.Decorrenza.HasValue ? (short)quotePensione.Decorrenza.Value.Day : (short)0;
                            }
                        }

                        if (quotePensione.EnteGestioneFondo != 0)
                        {
                            GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondo = elencoDecEnteGestioneFondo.Find(x => x.Id == quotePensione.EnteGestioneFondo);
                            contr.T_GP2BB05 = decEnteGestioneFondo.Codice;
                        }

                        if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TipoCumulo.HasValue && !contenitore.DatiPensioniDatiGenerici.TipoCumulo.Value &&
                            contr.T_GP2BB04A == 9999)
                            contr.T_GP2BB06 = 0.001M;
                        else
                            contr.T_GP2BB06 = quotePensione.Importo.HasValue ? quotePensione.Importo.Value : 0M;

                        contr.T_GP2BB08 = quotePensione.Settimane.HasValue ? quotePensione.Settimane.Value : (short)0;

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }

                if (contenitore.ListaQuoteMiglioramentiContrattuali != null && contenitore.ListaQuoteMiglioramentiContrattuali.Count > 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null || pannelloContributivo.LISTT_GP2BB03.Count() == 0)
                        pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();
                    foreach (GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali quoteMiglioramenti in contenitore.ListaQuoteMiglioramentiContrattuali)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        contr.T_GP2BB04A = !string.IsNullOrEmpty(quoteMiglioramenti.DataDecorrenza) ? short.Parse(quoteMiglioramenti.DataDecorrenza.Substring(6, 4)) : (short)0;
                        contr.T_GP2BB04M = !string.IsNullOrEmpty(quoteMiglioramenti.DataDecorrenza) ? short.Parse(quoteMiglioramenti.DataDecorrenza.Substring(3, 2)) : (short)0;
                        contr.T_GP2BB04G = !string.IsNullOrEmpty(quoteMiglioramenti.DataDecorrenza) ? short.Parse(quoteMiglioramenti.DataDecorrenza.Substring(0, 2)) : (short)0;
                        contr.T_GP2BB05 = quoteMiglioramenti.Codice;
                        contr.T_GP2BB06 = !string.IsNullOrEmpty(quoteMiglioramenti.Quota) ? decimal.Parse(quoteMiglioramenti.Quota) : 0M;

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }
            }
            #endregion Cumulo
            else if (Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaVOCOOP_L92(contenitore.DatiPensione) || Utility.IsDomandaVOESO_L92(contenitore.DatiPensione) ||
                     Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB_L26(contenitore.DatiPensione) || Utility.IsDomandaVESO92WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null)
                     || (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.GetFiltro() == "ESA" && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione)) || Utility.IsDomandaESPA_L26(contenitore.DatiPensione)
                     || Utility.IsDomandaVESO33_DAP(contenitore.DatiPensione) || Utility.IsDomandaCOOP28_DAP(contenitore.DatiPensione) || ((Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria)) &&
                    contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault()))
            {
                pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                contr.T_GP2BB06 = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza ?? 0 : 0;
                contr.T_GP2BB05 = "E";
                pannelloContributivo.LISTT_GP2BB03.Add(contr);
            }
            else if (Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) ||
                (Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.IsRicExtracalcolo.GetValueOrDefault()))
            {
                pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                contr.T_GP2BB06 = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza ?? 0 : 0;
                contr.T_GP2BB05 = "L";
                pannelloContributivo.LISTT_GP2BB03.Add(contr);
            }
            else if (((Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria)) && contenitore.DatiPensione.GetFiltro() == "FS") ||
                Utility.IsDomandaVESO29WithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null) ||
                Utility.IsDomandaVOESOWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null))
            {
                pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                contr.T_GP2BB06 = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza ?? 0 : 0;
                if (Utility.IsDomandaVOESOWithGP2BB05(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : null))
                    contr.T_GP2BB05 = contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.GP2BB05 : "L1";
                else
                    contr.T_GP2BB05 = "L1";
                pannelloContributivo.LISTT_GP2BB03.Add(contr);
            }
            else if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
            {
                pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                contr.T_GP2BB06 = contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.ImportoLordo ?? 0 : 0;
                contr.T_GP2BB05 = "M0";
                pannelloContributivo.LISTT_GP2BB03.Add(contr);
            }
            else if ((Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione)) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                if (contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria.HasValue && contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria != 0)
                {
                    pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                    contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                    contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                    contr.T_GP2BB06 = (decimal)contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria;
                    contr.T_GP2BB05 = "F";
                    pannelloContributivo.LISTT_GP2BB03.Add(contr);
                }
                if (contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001.HasValue && contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001 != 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null) pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                    contr.T_GP2BB04A = 2001;
                    contr.T_GP2BB04M = 1;
                    contr.T_GP2BB04G = 1;
                    contr.T_GP2BB06 = (decimal)contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001;
                    contr.T_GP2BB05 = "F";
                    pannelloContributivo.LISTT_GP2BB03.Add(contr);
                }
            }
            else
            {
                if (contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0)
                {
                    pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();
                    bool abilitaNuovoFlusso = IsFlussoAdeguata(contenitoreDecodifica.ElencoCtrlCatAdeguata, contenitore.DatiPensione.SiglaCategoria != null ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty, contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto, contenitore.DatiPensione.Tipo, Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda));
                    bool? variazioneDatiCalcolo = false;
                    if (abilitaNuovoFlusso) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);
                    foreach (GestioneCalcolo.DatiCalcoloContributivo cC in contenitore.ListaDatiContributivi)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                        DateTime? decorrenzaDaInviare = null;
                        if (contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue)
                            decorrenzaDaInviare = contenitore.DatiDanteCausa.DecorrenzaPensione;
                        else
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;

                        if (Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) || (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null))
                        {
                            if (cC.DecorrenzaCalcoloContibutivo.HasValue) decorrenzaDaInviare = cC.DecorrenzaCalcoloContibutivo;
                        }

                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB08 = cC.NSettimane.HasValue ? (short)cC.NSettimane.Value :
                            cC.NSettimaneQuotaDL214.HasValue ? (short)cC.NSettimaneQuotaDL214.Value : (short)0;
                        contr.T_GP2BB07 = cC.ImportoContributivoTotale.HasValue ? cC.ImportoContributivoTotale.Value :
                            cC.ImportoContribTotaleQuotaDL214.HasValue ? cC.ImportoContribTotaleQuotaDL214.Value : 0M;
                        contr.T_GP2BB06 = cC.Montante.HasValue ? cC.Montante.Value :
                            cC.MontanteQuotaDL214.HasValue ? cC.MontanteQuotaDL214.Value : 0M;

                        if (abilitaNuovoFlusso)
                        {
                            if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                            {
                                //Per le manuali, invio solo TFR/RIC se non ci sono state variazioni nei dati calcolo
                                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !variazioneDatiCalcolo.GetValueOrDefault())
                                {
                                    contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                                }
                            }
                            //Per le automatiche invio sempre
                            else
                            {
                                contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                            }
                        }

                        //ENG - Aggiornamento Memo 68/2022 IOPGI
                        //ENG - Spacchettate SOPGI
                        if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                            || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
                        {
                            contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                        }

                        if (cC.CodiceGestione.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Find(x => x.Id == cC.CodiceGestione.Value && !x.IsFondo);
                                if (codeGestioneCalcoloContributivo != null)
                                    contr.T_GP2BB05 = codeGestioneCalcoloContributivo.TraduzioneSuGP;
                            }
                        }

                        var ctrlSettimane = Utility.IsDomandaAUT(contenitore.DatiPensione) ? (cC.NSettimane.HasValue && cC.NSettimane.Value != 0 ? true : false) : cC.NSettimane.HasValue;
                        if (!(Utility.IsDomandaBancRicAnte1991(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)) != null))
                        {
                            if (ctrlSettimane || cC.ImportoContributivoTotale.HasValue || cC.Montante.HasValue)
                            {
                                contr.T_GP2BB0B = "C";
                                contr.T_GP2BB0A = "3";
                            }
                            else if (cC.NSettimaneQuotaDL214.HasValue || cC.ImportoContribTotaleQuotaDL214.HasValue || cC.MontanteQuotaDL214.HasValue)
                            {
                                contr.T_GP2BB0B = "D";
                                contr.T_GP2BB0A = "4";
                            }
                        }
                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                    if (contenitore.IsRiaperturaDomanda && (contenitore.ListaDatiRetributivi == null || contenitore.ListaDatiRetributivi.Count == 0))
                    {
                        if (variazioneDatiCalcolo == null) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);

                        if (!variazioneDatiCalcolo.GetValueOrDefault())
                            pannelloContributivo.LISTT_GP2BB03.First().T_GP2BB06 += 0.01m;
                    }
                }
                //Esattoriali
                if (contenitore.ListaDatiQuotaFondoIntegrativo != null && contenitore.ListaDatiQuotaFondoIntegrativo.Count > 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null)
                        pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    bool abilitaNuovoFlusso = IsFlussoAdeguata(contenitoreDecodifica.ElencoCtrlCatAdeguata, contenitore.DatiPensione.SiglaCategoria != null ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty, contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto, contenitore.DatiPensione.Tipo, Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda));
                    bool variazioneDatiCalcolo = false;
                    if (abilitaNuovoFlusso) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);

                    foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo cC in contenitore.ListaDatiQuotaFondoIntegrativo)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                        DateTime? decorrenzaDaInviare = null;
                        if (contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue)
                            decorrenzaDaInviare = contenitore.DatiDanteCausa.DecorrenzaPensione;
                        else
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;
                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB08 = cC.NSettimane.HasValue ? (short)cC.NSettimane.Value :
                            cC.NSettimaneQuotaD.HasValue ? (short)cC.NSettimaneQuotaD.Value : (short)0;
                        contr.T_GP2BB07 = cC.ImportoContributivoTotale.HasValue ? cC.ImportoContributivoTotale.Value :
                            cC.ImportoContribTotaleQuotaD.HasValue ? cC.ImportoContribTotaleQuotaD.Value : 0M;
                        contr.T_GP2BB06 = cC.Montante.HasValue ? cC.Montante.Value :
                            cC.MontanteQuotaD.HasValue ? cC.MontanteQuotaD.Value : 0M;

                        if (abilitaNuovoFlusso)
                        {
                            if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                            {
                                //Per le manuali, invio solo TFR/RIC se non ci sono state variazioni nei dati calcolo
                                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !variazioneDatiCalcolo)
                                {
                                    contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                                }
                            }
                            //Per le automatiche invio sempre
                            else
                            {
                                contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                            }
                        }

                        if (cC.CodiceGestione.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo != null && contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo codeGestioneQuotaFondoIntegrativo = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo.Find(x => x.Id == cC.CodiceGestione.Value);
                                if (codeGestioneQuotaFondoIntegrativo != null)
                                    contr.T_GP2BB05 = codeGestioneQuotaFondoIntegrativo.TraduzioneSuGP;
                            }
                        }

                        if (cC.NSettimane.HasValue || cC.ImportoContributivoTotale.HasValue || cC.Montante.HasValue)
                        {
                            contr.T_GP2BB0B = "C";
                            contr.T_GP2BB0A = "3";
                        }
                        else if (cC.NSettimaneQuotaD.HasValue || cC.ImportoContribTotaleQuotaD.HasValue || cC.MontanteQuotaD.HasValue)
                        {
                            contr.T_GP2BB0B = "D";
                            contr.T_GP2BB0A = "4";
                        }
                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }
                else if (contenitore.ListaDatiQuotaFondoIntegrativoStorico != null && contenitore.ListaDatiQuotaFondoIntegrativoStorico.Count > 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null)
                        pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    bool abilitaNuovoFlusso = IsFlussoAdeguata(contenitoreDecodifica.ElencoCtrlCatAdeguata, contenitore.DatiPensione.SiglaCategoria != null ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty, contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto, contenitore.DatiPensione.Tipo, Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda));
                    bool variazioneDatiCalcolo = false;
                    if (abilitaNuovoFlusso) variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);

                    foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo cC in contenitore.ListaDatiQuotaFondoIntegrativoStorico)
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                        DateTime? decorrenzaDaInviare = null;
                        if (contenitore.DatiDanteCausa != null && contenitore.DatiDanteCausa.DecorrenzaPensione.HasValue)
                            decorrenzaDaInviare = contenitore.DatiDanteCausa.DecorrenzaPensione;
                        else
                            decorrenzaDaInviare = contenitore.DatiPensione.DecorrenzaOriginaria;
                        contr.T_GP2BB04A = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Year : (short)0;
                        contr.T_GP2BB04M = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Month : (short)0;
                        contr.T_GP2BB04G = decorrenzaDaInviare.HasValue ? (short)decorrenzaDaInviare.Value.Day : (short)0;
                        contr.T_GP2BB08 = cC.NSettimane.HasValue ? (short)cC.NSettimane.Value :
                            cC.NSettimaneQuotaD.HasValue ? (short)cC.NSettimaneQuotaD.Value : (short)0;
                        contr.T_GP2BB07 = cC.ImportoContributivoTotale.HasValue ? cC.ImportoContributivoTotale.Value :
                            cC.ImportoContribTotaleQuotaD.HasValue ? cC.ImportoContribTotaleQuotaD.Value : 0M;
                        contr.T_GP2BB06 = cC.Montante.HasValue ? cC.Montante.Value :
                            cC.MontanteQuotaD.HasValue ? cC.MontanteQuotaD.Value : 0M;

                        if (abilitaNuovoFlusso)
                        {
                            if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                            {
                                //Per le manuali, invio solo TFR/RIC se non ci sono state variazioni nei dati calcolo
                                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !variazioneDatiCalcolo)
                                {
                                    contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                                }
                            }
                            //Per le automatiche invio sempre
                            else
                            {
                                contr.T_GP2BB0D = cC.PL_Quotac.HasValue ? cC.PL_Quotac.Value : 0M;
                            }
                        }

                        if (cC.CodiceGestione.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo != null && contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo codeGestioneQuotaFondoIntegrativo = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoIntegrativo.Find(x => x.Id == cC.CodiceGestione.Value);
                                if (codeGestioneQuotaFondoIntegrativo != null)
                                    contr.T_GP2BB05 = codeGestioneQuotaFondoIntegrativo.TraduzioneSuGP;
                            }
                        }

                        if (cC.NSettimane.HasValue || cC.ImportoContributivoTotale.HasValue || cC.Montante.HasValue)
                        {
                            contr.T_GP2BB0B = "C";
                            contr.T_GP2BB0A = "3";
                        }
                        else if (cC.NSettimaneQuotaD.HasValue || cC.ImportoContribTotaleQuotaD.HasValue || cC.MontanteQuotaD.HasValue)
                        {
                            contr.T_GP2BB0B = "D";
                            contr.T_GP2BB0A = "4";
                        }
                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }

                //ENG - Aggiornamento Memo 68/2022 IOPGI
                //ENG - Spacchettate SOPGI
                if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa)
                    || Utility.CheckMemo97(contenitore.DatiPensione)
                    )
                {
                    if (contenitore.ListaDatiContributiviINPGI != null && contenitore.ListaDatiContributiviINPGI.Count > 0)
                    {
                        if (pannelloContributivo.LISTT_GP2BB03 == null)
                            pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();
                        foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI cC in contenitore.ListaDatiContributiviINPGI)
                        {
                            Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();
                            //ENG - INPGI migrate
                            if (((Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || Utility.IsDomandaRipristino(contenitore.DatiPensione).Value) ||
                                (Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(contenitore.DatiPensione) && !contenitore.IsRiaperturaDomanda)) && contenitore.DatiPensione.GP1AV91B == "2")
                                contr.T_GP2BB06 = cC.Montante.HasValue ? cC.Montante.Value : 0M;
                            else
                                contr.T_GP2BB06 = contr.T_GP2BB07 = cC.Montante.HasValue ? cC.Montante.Value : 0M;
                            contr.T_GP2BB0D = cC.Quota.HasValue ? cC.Quota.Value : 0M;
                            contr.T_GP2BB04A = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                            contr.T_GP2BB04M = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                            contr.T_GP2BB04G = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                            contr.T_GP2BB08 = cC.Settimane.HasValue ? (short)cC.Settimane.Value : (short)0;

                            if (Utility.CheckMemo97(contenitore.DatiPensione))
                            {
                                if (contenitore.DatiPensione.DecorrenzaOriginaria.Value < new DateTime(1999, 1, 1))
                                {
                                    contr.T_GP2BB04A = 1999;
                                    contr.T_GP2BB04M = 1;
                                    contr.T_GP2BB04G = 1;
                                }
                            }

                            if (cC.CodiceGestione.HasValue)
                            {
                                if (contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI != null && contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Count > 0)
                                {
                                    GestioneDecodifica.CodeGestioneQuotaFondoINPGI codeGestioneQuotaFondoINPGI = contenitoreDecodifica.ElencoCodeGestioneQuotaFondoINPGI.Find(x => x.Id == cC.CodiceGestione.Value);
                                    if (codeGestioneQuotaFondoINPGI != null)
                                        contr.T_GP2BB05 = codeGestioneQuotaFondoINPGI.TraduzioneSuGP;
                                }
                            }
                            pannelloContributivo.LISTT_GP2BB03.Add(contr);
                        }
                    }
                }
            }


            if (contenitore.DatiSupplementiBase != null)
            {
                if (contenitore.DatiSupplementiBase.RenditaFacoltativaOrdinaria.HasValue && contenitore.DatiSupplementiBase.RenditaFacoltativaOrdinaria.Value > 0M)
                    pannelloContributivo.T_GP1AF04 = contenitore.DatiSupplementiBase.RenditaFacoltativaOrdinaria.Value;
                if (contenitore.DatiSupplementiBase.RenditaFacoltativaConvenzionale.HasValue && contenitore.DatiSupplementiBase.RenditaFacoltativaConvenzionale.Value > 0M)
                    pannelloContributivo.T_GP1AF04 = contenitore.DatiSupplementiBase.RenditaFacoltativaConvenzionale.Value;
            }

            #region Vittime Terrorismo
            if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
            {
                if (contenitore.ListaDatiCalcoloVittimeTerrorismo != null && contenitore.ListaDatiCalcoloVittimeTerrorismo.Count > 0)
                {
                    if (pannelloContributivo.LISTT_GP2BB03 == null)
                        pannelloContributivo.LISTT_GP2BB03 = new List<Data.CAREPET.PannelloContributivo.T_GP2BB03>();

                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'C'))
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        contr.T_GP2BB04A = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Year : (short)0;
                        contr.T_GP2BB04M = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Month : (short)0;
                        contr.T_GP2BB04G = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Day : (short)0;

                        string codiceGestione = string.Empty;
                        if (datiCalcoloVittimeTerrorismo.CodiceGestioneContr.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestioneCalcoloContributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloContributivo.Find(x => x.Id == datiCalcoloVittimeTerrorismo.CodiceGestioneContr.Value && !x.IsFondo);
                                if (codeGestioneCalcoloContributivo != null)
                                    codiceGestione = codeGestioneCalcoloContributivo.TraduzioneSuGP;
                            }
                        }

                        contr.T_GP2BB0B = datiCalcoloVittimeTerrorismo.Quota.HasValue ? datiCalcoloVittimeTerrorismo.Quota.Value.ToString() : string.Empty;
                        contr.T_GP2BB08 = datiCalcoloVittimeTerrorismo.Settimane.HasValue ? datiCalcoloVittimeTerrorismo.Settimane.Value : 0;
                        contr.T_GP2BB07 = datiCalcoloVittimeTerrorismo.Ammontare.HasValue ? datiCalcoloVittimeTerrorismo.Ammontare.Value : 0M;
                        contr.T_GP2BB06 = datiCalcoloVittimeTerrorismo.Montante.HasValue ? datiCalcoloVittimeTerrorismo.Montante.Value : 0M;
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                        {
                            contr.T_GP2BB05 = !string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1).PadRight(2, ' ') : "  ";
                            contr.T_GP2BB0C = datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ";
                        }
                        else
                        {
                            contr.T_GP2BB05 = (!string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1) : " ") +
                                (datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ");
                        }
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria) ||
                            (!string.IsNullOrEmpty(codiceGestione) && new List<string> { "1", "2", "3", "4", "A" }.Contains(codiceGestione.Trim()) &&
                            (datiCalcoloVittimeTerrorismo.Beneficio.GetValueOrDefault() == 'Y' || datiCalcoloVittimeTerrorismo.Beneficio.GetValueOrDefault() == 'W')))
                        {
                            if (contr.T_GP2BB0B == "C")
                                contr.T_GP2BB0A = "3";
                            else if (contr.T_GP2BB0B == "D")
                                contr.T_GP2BB0A = "4";
                        }

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }

                    foreach (GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo in contenitore.ListaDatiCalcoloVittimeTerrorismo.FindAll(x => x.Tipo == 'I'))
                    {
                        Data.CAREPET.PannelloContributivo.T_GP2BB03 contr = new Data.CAREPET.PannelloContributivo.T_GP2BB03();

                        contr.T_GP2BB04A = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Year : (short)0;
                        contr.T_GP2BB04M = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Month : (short)0;
                        contr.T_GP2BB04G = datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.HasValue ? (short)datiCalcoloVittimeTerrorismo.DecorrenzaBeneficio.Value.Day : (short)0;

                        string codiceGestione = string.Empty;
                        if (datiCalcoloVittimeTerrorismo.CodiceGestioneRetr.HasValue)
                        {
                            if (contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo != null && contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Count > 0)
                            {
                                GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo = contenitoreDecodifica.ElencoCodeGestioneCalcoloRetributivo.Find(x => x.Id == datiCalcoloVittimeTerrorismo.CodiceGestioneRetr.Value && !x.IsFondo);
                                if (codeGestioneCalcoloRetributivo != null)
                                    codiceGestione = codeGestioneCalcoloRetributivo.TraduzioneSuGP;
                            }
                        }

                        contr.T_GP2BB08 = datiCalcoloVittimeTerrorismo.Settimane.HasValue ? datiCalcoloVittimeTerrorismo.Settimane.Value : 0;
                        contr.T_GP2BB06 = datiCalcoloVittimeTerrorismo.ImportoPensione.HasValue ? datiCalcoloVittimeTerrorismo.ImportoPensione.Value : 0M;
                        if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                        {
                            contr.T_GP2BB05 = !string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1).PadRight(2, ' ') : "  ";
                            contr.T_GP2BB0C = datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ";
                        }
                        else
                        {
                            contr.T_GP2BB05 = (!string.IsNullOrEmpty(codiceGestione) ? codiceGestione.Substring(0, 1) : " ") +
                                (datiCalcoloVittimeTerrorismo.Beneficio.HasValue ? datiCalcoloVittimeTerrorismo.Beneficio.Value.ToString() : " ");
                        }

                        pannelloContributivo.LISTT_GP2BB03.Add(contr);
                    }
                }
            }
            #endregion Vittime Terrorismo
        }

        private static void ValorizzaSupplementi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out Data.CAREPET.Supplementi supplementi)
        {
            supplementi = new Data.CAREPET.Supplementi();

            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
            List<EntityBLCommon.DatiSupplementi> listaSupplementi = new List<EntityBLCommon.DatiSupplementi>();
            if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione) && !Utility.IsDomandaENPALS(contenitore.DatiPensione.SiglaCategoria) &&
                !Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && !(Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) != null))
                listaSupplementi = contenitore.ListaDatiSupplementiNoStorico;
            else
                listaSupplementi = contenitore.ListaDatiSupplementi;

            if (listaSupplementi != null && listaSupplementi.Count > 0)
            {
                supplementi.LISTT_GP2BE00 = new List<Data.CAREPET.Supplementi.T_GP2BE00>();
                List<EntityBLCommon.DatiSupplementi> listaSupplementiApp = new List<EntityBLCommon.DatiSupplementi>();
                listaSupplementiApp.AddRange(listaSupplementi.ToList());
                foreach (EntityBLCommon.DatiSupplementi s in listaSupplementi)
                {
                    short meseDec = 0;
                    EntityBLCommon.DatiSupplementi suppFittizio = null;
                    Data.CAREPET.Supplementi.T_GP2BE00 supp = new Data.CAREPET.Supplementi.T_GP2BE00();
                    supp.T_GP2BE01A = s.DecorrenzaSupplemento.HasValue ? (short)s.DecorrenzaSupplemento.Value.Year : (short)0;
                    if (Utility.IsDomandaINPDAI(contenitore.DatiPensione.SiglaCategoria))
                    {
                        if (!(s.QuotaSupplemento.HasValue && s.QuotaSupplemento.Value == 'A' && s.NSettimaneSupplemento.GetValueOrDefault() == 1 && s.RMSSupplemento.HasValue && s.RMSSupplemento.Value < 1) &&
                            s.TipoSupplemento == 'R')
                        {
                            AlteraSupplementiINPDAI(contenitore.DatiPensione, s, listaSupplementiApp, out meseDec, out suppFittizio);
                            if (suppFittizio != null)
                                listaSupplementiApp.Add(suppFittizio);
                            supp.T_GP2BE01M = meseDec;
                        }
                        else
                            supp.T_GP2BE01M = s.DecorrenzaSupplemento.HasValue ? (short)s.DecorrenzaSupplemento.Value.Month : (short)0;
                    }
                    else
                    {
                        if (s.QuotaSupplemento.HasValue && s.QuotaSupplemento.Value == 'B' && s.CodiceLiquidazione.GetValueOrDefault() != 6)
                        {
                            AlteraSupplementi(contenitore.DatiPensione, s, listaSupplementi, out meseDec, out suppFittizio);
                            supp.T_GP2BE01M = meseDec;
                        }
                        else
                            supp.T_GP2BE01M = s.DecorrenzaSupplemento.HasValue ? (short)s.DecorrenzaSupplemento.Value.Month : (short)0;

                        if (Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && s.QuotaSupplemento.HasValue && s.QuotaSupplemento.Value == 'A' && s.CodGestioneSupplemento == "H")
                        {
                            supp.T_GP2BE01M = 75;
                        }
                    }
                    supp.T_GP2BE02 = s.CodGestioneSupplemento;
                    supp.T_GP2BE03 = s.MontanteSupplemento.HasValue ? s.MontanteSupplemento.Value : 0M;
                    supp.T_GP2BE04 = s.AmmontareContributivo.HasValue ? s.AmmontareContributivo.Value : 0M;
                    supp.T_GP2BE05 = s.RMSSupplemento.HasValue ? Math.Round(s.RMSSupplemento.Value, 4) : 0M;
                    supp.T_GP2BE06 = s.NSettimaneSupplemento.HasValue ? s.NSettimaneSupplemento.Value : 0;
                    supp.T_GP2BE07 = s.CodiceLiquidazione.HasValue ? s.CodiceLiquidazione.Value : (short)0;

                    supplementi.LISTT_GP2BE00.Add(supp);

                    if (suppFittizio != null)
                    {
                        supp = new Data.CAREPET.Supplementi.T_GP2BE00();
                        supp.T_GP2BE01A = suppFittizio.DecorrenzaSupplemento.HasValue ? (short)suppFittizio.DecorrenzaSupplemento.Value.Year : (short)0;
                        supp.T_GP2BE01M = suppFittizio.DecorrenzaSupplemento.HasValue ? (short)suppFittizio.DecorrenzaSupplemento.Value.Month : (short)0;
                        supp.T_GP2BE02 = suppFittizio.CodGestioneSupplemento;
                        supp.T_GP2BE03 = suppFittizio.MontanteSupplemento.HasValue ? suppFittizio.MontanteSupplemento.Value : 0M;
                        supp.T_GP2BE04 = suppFittizio.AmmontareContributivo.HasValue ? suppFittizio.AmmontareContributivo.Value : 0M;
                        supp.T_GP2BE05 = suppFittizio.RMSSupplemento.HasValue ? Math.Round(suppFittizio.RMSSupplemento.Value, 4) : 0M;
                        supp.T_GP2BE06 = suppFittizio.NSettimaneSupplemento.HasValue ? suppFittizio.NSettimaneSupplemento.Value : 0;
                        supp.T_GP2BE07 = suppFittizio.CodiceLiquidazione.HasValue ? suppFittizio.CodiceLiquidazione.Value : (short)0;
                        supplementi.LISTT_GP2BE00.Add(supp);
                    }
                }
            }

            if (contenitore.ListaDatiSuppRecordENPALS != null && contenitore.ListaDatiSuppRecordENPALS.Count > 0)
            {
                foreach (EntityBLCommon.DatiSuppRecordENPALS datiSuppRecordENPALS in contenitore.ListaDatiSuppRecordENPALS)
                {
                    if (contenitore.ListaDatiSupplementiENPALS != null && contenitore.ListaDatiSupplementiENPALS.Count > 0)
                    {
                        List<EntityBLCommon.DatiSupplementiENPALS> listApp = contenitore.ListaDatiSupplementiENPALS.FindAll(x => x.IdSuppRecordENPALS == datiSuppRecordENPALS.IdSuppRecordEnpals);

                        if (listApp != null && listApp.Count > 0)
                        {
                            if (listApp.Exists(x => x.Quota.GetValueOrDefault() == 'B' && !listApp.Exists(y => y.Quota.GetValueOrDefault() == 'A' && x.Decorrenza == y.Decorrenza)))
                            {
                                listApp.Add(new EntityBLCommon.DatiSupplementiENPALS
                                {
                                    TipoSupplemento = 'R',
                                    Quota = 'A',
                                    Periodi = 1,
                                    RM = 0.004M
                                });
                            }

                            foreach (EntityBLCommon.DatiSupplementiENPALS s in listApp)
                            {
                                //02-12-2015 MAIL Oggetto: 'Liq Pens Enpals - mapping supplementi'
                                if ((s.ImportoContributivoTotale == 0 && s.TipoSupplemento == 'C') || (s.RM == 0 && s.TipoSupplemento == 'R'))
                                    continue;

                                Data.CAREPET.Supplementi.T_GP2BE00 supp = new Data.CAREPET.Supplementi.T_GP2BE00();

                                supp.T_GP2BE03 = s.Montante.HasValue ? s.Montante.Value : 0M;
                                supp.T_GP2BE04 = s.ImportoContributivoTotale.HasValue ? s.ImportoContributivoTotale.Value : 0M;
                                supp.T_GP2BE05 = s.RM.HasValue ? s.RM.Value : 0M;
                                supp.T_GP2BE06 = s.Periodi.HasValue ? s.Periodi.Value : 0;
                                supp.T_GP2BE0B = s.Quota.HasValue ? s.Quota.Value.ToString() : string.Empty;
                                supp.T_GP2BE01A = datiSuppRecordENPALS.Decorrenza.HasValue ? (short)datiSuppRecordENPALS.Decorrenza.Value.Year : (short)0;
                                if (s.Quota.GetValueOrDefault() == 'B')
                                    supp.T_GP2BE01M = 61;
                                else
                                    supp.T_GP2BE01M = datiSuppRecordENPALS.Decorrenza.HasValue ? (short)datiSuppRecordENPALS.Decorrenza.Value.Month : (short)0;
                                supp.T_GP2BE02 = "1";

                                supp.T_GP2BE11RZA = datiSuppRecordENPALS.InizioSupplemento.HasValue ? (short)datiSuppRecordENPALS.InizioSupplemento.Value.Year : (short)0;
                                supp.T_GP2BE11RZM = datiSuppRecordENPALS.InizioSupplemento.HasValue ? (short)datiSuppRecordENPALS.InizioSupplemento.Value.Month : (short)0;
                                supp.T_GP2BE11RZG = datiSuppRecordENPALS.InizioSupplemento.HasValue ? (short)datiSuppRecordENPALS.InizioSupplemento.Value.Day : (short)0;

                                supp.T_GP2BE12RZA = datiSuppRecordENPALS.FineSupplemento.HasValue ? (short)datiSuppRecordENPALS.FineSupplemento.Value.Year : (short)0;
                                supp.T_GP2BE12RZM = datiSuppRecordENPALS.FineSupplemento.HasValue ? (short)datiSuppRecordENPALS.FineSupplemento.Value.Month : (short)0;
                                supp.T_GP2BE12RZG = datiSuppRecordENPALS.FineSupplemento.HasValue ? (short)datiSuppRecordENPALS.FineSupplemento.Value.Day : (short)0;

                                if (s.TipoSupplemento == 'C')
                                {
                                    if (contenitore.DatiContribuzioneEnpalsSAS != null)
                                    {
                                        if (contenitore.DatiContribuzioneEnpalsSAS.QuotaC != null)
                                        {
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Enpals.GetValueOrDefault();
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Estera.GetValueOrDefault();
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Figurativa.GetValueOrDefault();
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Inps.GetValueOrDefault();
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Ufficio.GetValueOrDefault();
                                            supp.T_GP2BE06 += (short)contenitore.DatiContribuzioneEnpalsSAS.QuotaC.Volontaria.GetValueOrDefault();
                                        }
                                    }
                                }

                                if (s.Quota.GetValueOrDefault() == 'C')
                                {
                                    supp.T_GP2BE0B = "C";
                                    supp.T_GP2BE07 = 3;
                                }
                                else if (s.Quota.GetValueOrDefault() == 'D')
                                {
                                    supp.T_GP2BE0B = "D";
                                    supp.T_GP2BE07 = 4;
                                }

                                if (supplementi.LISTT_GP2BE00 == null)
                                    supplementi.LISTT_GP2BE00 = new List<Data.CAREPET.Supplementi.T_GP2BE00>();

                                supplementi.LISTT_GP2BE00.Add(supp);
                            }
                        }
                    }
                }
            }

            //ENG - Memo 32_a/2018
            if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(contenitore.DatiPensione))
            {
                List<EntityBLCommon.DatiSupplementiCumulo> listaSupplementiDaInviare = new List<EntityBLCommon.DatiSupplementiCumulo>();
                List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;

                //Supplementi presenti nella GAIN ma non nel servizio CUMULO
                if (contenitore.ListaDatiSupplementiCumuloStorico != null && contenitore.ListaDatiSupplementiCumuloStorico.Count > 0)
                {
                    foreach (EntityBLCommon.DatiSupplementiCumulo supp in contenitore.ListaDatiSupplementiCumuloStorico)
                    {
                        if (contenitore.ListaDatiSupplementiCumulo == null ||
                            contenitore.ListaDatiSupplementiCumulo.Count() == 0 ||
                            !contenitore.ListaDatiSupplementiCumulo.Exists(x => x.EnteGestioneFondo == supp.EnteGestioneFondo && x.Decorrenza == supp.Decorrenza))
                            listaSupplementiDaInviare.Add(supp);
                    }
                }

                //Supplementi presenti sia nella GAIN sia nel servizio CUMULO con tipo variazione pari a 0
                if (contenitore.ListaDatiSupplementiCumuloStorico != null && contenitore.ListaDatiSupplementiCumuloStorico.Count > 0
                    && contenitore.ListaDatiSupplementiCumulo != null && contenitore.ListaDatiSupplementiCumulo.Count > 0)
                {
                    foreach (EntityBLCommon.DatiSupplementiCumulo supp in contenitore.ListaDatiSupplementiCumulo)
                    {
                        if (supp.TipoVariazione.HasValue && supp.TipoVariazione.Value == 0)
                        {
                            if (contenitore.ListaDatiSupplementiCumuloStorico.Exists(x => x.EnteGestioneFondo == supp.EnteGestioneFondo && x.Decorrenza == supp.Decorrenza))
                                listaSupplementiDaInviare.Add(supp);
                        }
                    }
                }

                if (listaSupplementiDaInviare != null && listaSupplementiDaInviare.Count > 0)
                {
                    supplementi.LISTT_GP2BE00 = new List<Data.CAREPET.Supplementi.T_GP2BE00>();
                    if (!listaSupplementiDaInviare.Exists(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo) == null))
                        listaSupplementiDaInviare = listaSupplementiDaInviare.OrderBy(x => x.Decorrenza).ThenBy(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo).Codice).ToList();

                    foreach (EntityBLCommon.DatiSupplementiCumulo supp in listaSupplementiDaInviare)
                    {
                        Data.CAREPET.Supplementi.T_GP2BE00 quotaSupp = new Data.CAREPET.Supplementi.T_GP2BE00();

                        quotaSupp.T_GP2BE01A = supp.Decorrenza.HasValue ? (short)supp.Decorrenza.Value.Year : (short)0;
                        quotaSupp.T_GP2BE01M = supp.Decorrenza.HasValue ? (short)supp.Decorrenza.Value.Month : (short)0;
                        if (supp.EnteGestioneFondo != 0)
                        {
                            GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondo = elencoDecEnteGestioneFondo.Find(x => x.Id == supp.EnteGestioneFondo);
                            quotaSupp.T_GP2BE02 = decEnteGestioneFondo.Codice;
                        }
                        quotaSupp.T_GP2BE03 = supp.Importo.HasValue ? supp.Importo.Value : 0M;
                        quotaSupp.T_GP2BE06 = supp.Settimane.HasValue ? supp.Settimane.Value : (short)0;

                        supplementi.LISTT_GP2BE00.Add(quotaSupp);
                    }
                }

            }
            else if (contenitore.ListaDatiSupplementiCumulo != null && contenitore.ListaDatiSupplementiCumulo.Count > 0)
            {
                supplementi.LISTT_GP2BE00 = new List<Data.CAREPET.Supplementi.T_GP2BE00>();
                List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
                if (!contenitore.ListaDatiSupplementiCumulo.Exists(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo) == null))
                    contenitore.ListaDatiSupplementiCumulo = contenitore.ListaDatiSupplementiCumulo.OrderBy(x => x.Decorrenza).ThenBy(x => elencoDecEnteGestioneFondo.Find(y => y.Id == x.EnteGestioneFondo).Codice).ToList();
                foreach (EntityBLCommon.DatiSupplementiCumulo supp in contenitore.ListaDatiSupplementiCumulo)
                {
                    Data.CAREPET.Supplementi.T_GP2BE00 quotaSupp = new Data.CAREPET.Supplementi.T_GP2BE00();

                    quotaSupp.T_GP2BE01A = supp.Decorrenza.HasValue ? (short)supp.Decorrenza.Value.Year : (short)0;
                    quotaSupp.T_GP2BE01M = supp.Decorrenza.HasValue ? (short)supp.Decorrenza.Value.Month : (short)0;
                    if (supp.EnteGestioneFondo != 0)
                    {
                        GestioneDecodifica.DecEnteGestioneFondo decEnteGestioneFondo = elencoDecEnteGestioneFondo.Find(x => x.Id == supp.EnteGestioneFondo);
                        quotaSupp.T_GP2BE02 = decEnteGestioneFondo.Codice;
                    }
                    quotaSupp.T_GP2BE03 = supp.Importo.HasValue ? supp.Importo.Value : 0M;
                    quotaSupp.T_GP2BE06 = supp.Settimane.HasValue ? supp.Settimane.Value : (short)0;

                    //ENG  - Il campo GP2BE07 deve essere valorizzato per tutte le RIC VOCUM
                    if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(contenitore.DatiPensione)
                        || (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria)))
                    {
                        if (supp.AdeguamentoProQuotaCasse.HasValue && supp.AdeguamentoProQuotaCasse.Value)
                            quotaSupp.T_GP2BE07 = 5;
                    }

                    supplementi.LISTT_GP2BE00.Add(quotaSupp);
                }

            }
        }

        private static void ValorizzaBititolarieta(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Bititolarieta bititolarieta)
        {
            bititolarieta = new Data.CAREPET.Bititolarieta();

            if (contenitore.ListaAltraPensione != null && contenitore.ListaAltraPensione.Count > 0)
            {
                bititolarieta.LISTT_GP2A15 = new List<Data.CAREPET.Bititolarieta.T_GP2A15>();
                foreach (GestioneAltrePensioni.AltraPensione aP in contenitore.ListaAltraPensione)
                {
                    Data.CAREPET.Bititolarieta.T_GP2A15 altraPensione = new Data.CAREPET.Bititolarieta.T_GP2A15();

                    altraPensione.T_GP2CAT = aP.Categoria.PadLeft(3, ' ');
                    altraPensione.T_GP2CER = aP.Certificato.HasValue ? aP.Certificato.Value : 0;
                    altraPensione.T_GP2CESA = aP.Cessazione.HasValue ? (short)aP.Cessazione.Value.Year : (short)0;
                    altraPensione.T_GP2CESM = aP.Cessazione.HasValue ? (short)aP.Cessazione.Value.Month : (short)0;
                    altraPensione.T_GP2CODU = aP.CodiceUC.HasValue ? aP.CodiceUC.Value.ToString() : string.Empty;
                    altraPensione.T_GP2CTM = aP.CodiceImporto.HasValue ? aP.CodiceImporto.Value.ToString() : string.Empty;
                    altraPensione.T_GP2DECA = aP.Decorrenza.HasValue ? (short)aP.Decorrenza.Value.Year : (short)0;
                    altraPensione.T_GP2DECM = aP.Decorrenza.HasValue ? (short)aP.Decorrenza.Value.Month : (short)0;
                    altraPensione.T_GP2ENTE = aP.Ente.HasValue ? (short)aP.Ente.Value : (short)0;
                    bititolarieta.LISTT_GP2A15.Add(altraPensione);
                }
            }
        }

        private static void ValorizzaRedditi(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Redditi redditi)
        {
            redditi = new Data.CAREPET.Redditi();

            if (contenitore.DatiMaggiorazioniBenefici != null && contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
            {
                redditi.RedditiMaggiorazione = new Data.CAREPET.Redditi.Maggiorazione();
                redditi.RedditiMaggiorazione.T_GP1AF07A = (short)contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Year;
                redditi.RedditiMaggiorazione.T_GP1AF07M = (short)contenitore.DatiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.Value.Month;
            }

            bool IsDomandaRiliquidazioneIndiretta = contenitore.DatiPensione.Gruppo == "0051" && contenitore.DatiPensione.Prodotto == "0422" && contenitore.DatiPensione.Tipo == "0026";
            if (contenitore.ListaDatiRedditoSentenza495_93 != null && contenitore.ListaDatiRedditoSentenza495_93.Count > 0 && !Utility.IsDomandaSOAUT_Supplementare(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
            {
                redditi.RedditiSentenza495_93 = new Data.CAREPET.Redditi.Sentenza495_93();
                redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z = new List<Data.CAREPET.Redditi.Sentenza495_93.T_GP7LKE0Z>();

                contenitore.ListaDatiRedditoSentenza495_93.Sort(delegate(GestioneDanteCausa.DatiRedditoSentenza495_93 r1, GestioneDanteCausa.DatiRedditoSentenza495_93 r2)
                {
                    return r1.AnnoReddito.Value.CompareTo(r2.AnnoReddito);
                });

                foreach (GestioneDanteCausa.DatiRedditoSentenza495_93 redditoSentenza495_93 in contenitore.ListaDatiRedditoSentenza495_93)
                {
                    Data.CAREPET.Redditi.Sentenza495_93.T_GP7LKE0Z T_GP7LKE0Z = new Data.CAREPET.Redditi.Sentenza495_93.T_GP7LKE0Z();
                    T_GP7LKE0Z.T_GP7LKE1 = redditoSentenza495_93.AnnoReddito.HasValue ? redditoSentenza495_93.AnnoReddito.Value : (short)0;

                    if (redditoSentenza495_93.AnnoReddito.Value < 2009) // Ante 2008
                    {
                        T_GP7LKE0Z.T_GP7LKE2 = redditoSentenza495_93.RedditoTitolare.HasValue ? redditoSentenza495_93.RedditoTitolare.Value : 0.0M;
                        T_GP7LKE0Z.T_GP7LKE3 = redditoSentenza495_93.RedditoConiuge.HasValue ? redditoSentenza495_93.RedditoConiuge.Value : 0.0M;
                    }
                    else // Post 2008
                    {
                        T_GP7LKE0Z.T_GP7LKE2 = redditoSentenza495_93.RedditoTitolare.GetValueOrDefault() + redditoSentenza495_93.RedditoDaPensioneDC.GetValueOrDefault();
                        T_GP7LKE0Z.T_GP7LKE2CD = "D";
                        T_GP7LKE0Z.T_GP7LKE2CP = "C";
                        T_GP7LKE0Z.T_GP7LKE2D = redditoSentenza495_93.RedditoTitolare.HasValue ? redditoSentenza495_93.RedditoTitolare.Value : 0.0M;
                        T_GP7LKE0Z.T_GP7LKE2P = redditoSentenza495_93.RedditoDaPensioneDC.HasValue ? redditoSentenza495_93.RedditoDaPensioneDC.Value : 0.0M;
                        T_GP7LKE0Z.T_GP7LKE3 = redditoSentenza495_93.RedditoDaPensioneConiuge.GetValueOrDefault() + redditoSentenza495_93.RedditoConiuge.GetValueOrDefault();
                        T_GP7LKE0Z.T_GP7LKE3CD = "D";
                        T_GP7LKE0Z.T_GP7LKE3CP = "C";
                        T_GP7LKE0Z.T_GP7LKE3D = redditoSentenza495_93.RedditoConiuge.HasValue ? redditoSentenza495_93.RedditoConiuge.Value : 0.0M;
                        T_GP7LKE0Z.T_GP7LKE3P = redditoSentenza495_93.RedditoDaPensioneConiuge.HasValue ? redditoSentenza495_93.RedditoDaPensioneConiuge.Value : 0.0M;
                    }

                    if (redditoSentenza495_93.CodiceDiReddito != null && redditoSentenza495_93.CodiceDiReddito.Length > 0)
                    {
                        string[] gp = redditoSentenza495_93.CodiceDiReddito.Split('-');

                        T_GP7LKE0Z.T_GP7LKE4A = gp.Length > 0 ? short.Parse(gp[0]) : (short)0;
                        T_GP7LKE0Z.T_GP7LKE4B = gp.Length > 1 ? short.Parse(gp[1]) : (short)0;
                        T_GP7LKE0Z.T_GP7LKE4C = gp.Length > 2 ? short.Parse(gp[2]) : (short)0;
                        T_GP7LKE0Z.T_GP7LKE4D = gp.Length > 3 ? short.Parse(gp[3]) : (short)0;
                    }

                    redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z.Add(T_GP7LKE0Z);
                }
            }
        }

        private static void ValorizzaInvciv(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Invciv invciv)
        {
            invciv = new Data.CAREPET.Invciv();

            if (Utility.IsRenditaCasalinghe(contenitore.DatiPensione) || Utility.IsRenditaFacoltativa(contenitore.DatiPensione))
            {
                invciv.T_GP2BB061_V = contenitore.DatiPensioniDatiGenerici.ImportoMensileAllaDecorrenzaOriginaria ?? 0;
                invciv.T_GP2BB062_V = contenitore.DatiPensioniDatiGenerici.ImportoMensileAlGennaio2001 ?? 0;
            }
        }

        private static void ValorizzaRicoveri(out Data.CAREPET.Ricoveri ricoveri)
        {
            ricoveri = new Data.CAREPET.Ricoveri();
        }

        private static void ValorizzaDelegato(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Delegato delegato)
        {
            delegato = new Data.CAREPET.Delegato();

            if (contenitore.DatiAnagraficiDelegato != null)
            {
                delegato.T_GP1DCOGNOME_V = contenitore.DatiAnagraficiDelegato.Cognome;
                delegato.T_GP1DNOME_V = contenitore.DatiAnagraficiDelegato.Nome;
                delegato.T_GP1AP26_V = contenitore.DatiAnagraficiDelegato.CodiceFiscale;
                delegato.T_GP1AP22A_V = contenitore.DatiAnagraficiDelegato.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDelegato.DataNascita.Value.Year : (short)0;
                delegato.T_GP1AP22M_V = contenitore.DatiAnagraficiDelegato.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDelegato.DataNascita.Value.Month : (short)0;
                delegato.T_GP1AP22G_V = contenitore.DatiAnagraficiDelegato.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiDelegato.DataNascita.Value.Day : (short)0;
                delegato.T_GP1AP24_V = contenitore.DatiAnagraficiDelegato.ComuneNascita;
                delegato.T_GP1AP25_V = contenitore.DatiAnagraficiDelegato.ProvinciaNascita;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(contenitore.DatiAnagraficiDelegato.CodiceComuneNascita, Utility.TipoAppartenenza.AGO.ToString(), 0, false, out codiceInpsComune);
                delegato.T_GP1AP23_V = codiceInpsComune;
                delegato.T_GP1AP27_V = contenitore.DatiAnagraficiDelegato.Sesso.HasValue ? contenitore.DatiAnagraficiDelegato.Sesso.Value.ToString() : "";
                delegato.T_GP1DCOMUNE_V = contenitore.DatiAnagraficiDelegato.ComuneResidenza;
                delegato.T_GP1DPROV_V = contenitore.DatiAnagraficiDelegato.ProvinciaResidenza;

                if (contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Length > 52)
                {
                    delegato.T_GP1DIND1_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Substring(0, 52);
                    if (contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Length > 104)
                    {
                        delegato.T_GP1DIND2_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Substring(52, 52);
                        if (contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Length > 156)
                            delegato.T_GP1DIND3_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Substring(104, 52);
                        else
                            delegato.T_GP1DIND3_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Substring(104);
                    }
                    else
                        delegato.T_GP1DIND2_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim().Substring(52);
                }
                else
                    delegato.T_GP1DIND1_V = contenitore.DatiAnagraficiDelegato.Indirizzo.Trim();

                delegato.T_GP1DCIVICO_V = contenitore.DatiAnagraficiDelegato.NCivico;
                delegato.T_GP1DFRAZIONE_V = contenitore.DatiAnagraficiDelegato.FrazioneResidenza;
                delegato.T_GP1DCAP_V = contenitore.DatiAnagraficiDelegato.CAP;
                delegato.T_GP1AP28_V = contenitore.DatiAnagraficiDelegato.Codice1Arca;
                int resInt = 0;
                int.TryParse(contenitore.DatiAnagraficiDelegato.Codice2Arca, out resInt);
                delegato.T_GP1AP29_V = resInt;

                if (contenitore.DatiAnagraficiDelegato.ResidenzaEstero.HasValue && contenitore.DatiAnagraficiDelegato.ResidenzaEstero.Value)
                    delegato.T_GP1DRESIDOM_V = "9";
                else if (contenitore.DatiAnagraficiDelegato.ResidenzaEstero.HasValue && !contenitore.DatiAnagraficiDelegato.ResidenzaEstero.Value)
                    delegato.T_GP1DRESIDOM_V = "1";

                delegato.T_GP1AP01_V = contenitore.DatiAnagraficiDelegato.CodiceDelegato.HasValue ? contenitore.DatiAnagraficiDelegato.CodiceDelegato.Value.ToString() : "";
            }
        }

        private static void ValorizzaTutore(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.Tutore tutore)
        {
            tutore = new Data.CAREPET.Tutore();

            if (contenitore.DatiAnagraficiTutore != null)
            {
                tutore.T_GP1TCOGNOME_V = contenitore.DatiAnagraficiTutore.Cognome;
                tutore.T_GP1TNOME_V = contenitore.DatiAnagraficiTutore.Nome;
                tutore.T_GP1AP66_V = contenitore.DatiAnagraficiTutore.CodiceFiscale;
                tutore.T_GP1AP62A_V = contenitore.DatiAnagraficiTutore.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiTutore.DataNascita.Value.Year : (short)0;
                tutore.T_GP1AP62M_V = contenitore.DatiAnagraficiTutore.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiTutore.DataNascita.Value.Month : (short)0;
                tutore.T_GP1AP62G_V = contenitore.DatiAnagraficiTutore.DataNascita.HasValue ? (short)contenitore.DatiAnagraficiTutore.DataNascita.Value.Day : (short)0;
                tutore.T_GP1AP64_V = contenitore.DatiAnagraficiTutore.ComuneNascita;
                tutore.T_GP1AP65_V = contenitore.DatiAnagraficiTutore.ProvinciaNascita;
                int codiceInpsComune = 0;
                GestioneDBSComuni.GetCodInpsComuneByCodCatastale(contenitore.DatiAnagraficiTutore.CodiceComuneNascita, Utility.TipoAppartenenza.AGO.ToString(), 0, false, out codiceInpsComune);
                tutore.T_GP1AP63_V = codiceInpsComune;
                tutore.T_GP1AP67_V = contenitore.DatiAnagraficiTutore.Sesso.HasValue ? contenitore.DatiAnagraficiTutore.Sesso.Value.ToString() : "";
                tutore.T_GP1TCOMUNE_V = contenitore.DatiAnagraficiTutore.ComuneResidenza;
                tutore.T_GP1TPROV_V = contenitore.DatiAnagraficiTutore.ProvinciaResidenza;

                if (contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Length > 52)
                {
                    tutore.T_GP1TIND1_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Substring(0, 52);
                    if (contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Length > 104)
                    {
                        tutore.T_GP1TIND2_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Substring(52, 52);
                        if (contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Length > 156)
                            tutore.T_GP1TIND3_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Substring(104, 52);
                        else
                            tutore.T_GP1TIND3_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Substring(104);
                    }
                    else
                        tutore.T_GP1TIND2_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim().Substring(52);
                }
                else
                    tutore.T_GP1TIND1_V = contenitore.DatiAnagraficiTutore.Indirizzo.Trim();

                tutore.T_GP1TCIVICO_V = contenitore.DatiAnagraficiTutore.NCivico;
                tutore.T_GP1TFRAZIONE_V = contenitore.DatiAnagraficiTutore.FrazioneResidenza;
                tutore.T_GP1TCAP_V = contenitore.DatiAnagraficiTutore.CAP;
                tutore.T_GP1AP68_V = contenitore.DatiAnagraficiTutore.Codice1Arca;
                int resInt = 0;
                int.TryParse(contenitore.DatiAnagraficiTutore.Codice2Arca, out resInt);
                tutore.T_GP1AP69_V = resInt;

                if (contenitore.DatiAnagraficiTutore.ResidenzaEstero.HasValue && contenitore.DatiAnagraficiTutore.ResidenzaEstero.Value)
                    tutore.T_GP1TRESIDOM_V = "9";
                else if (contenitore.DatiAnagraficiTutore.ResidenzaEstero.HasValue && !contenitore.DatiAnagraficiTutore.ResidenzaEstero.Value)
                    tutore.T_GP1TRESIDOM_V = "1";

                tutore.T_GP1AP61_V = contenitore.DatiAnagraficiTutore.CodiceTutore.HasValue ? contenitore.DatiAnagraficiTutore.CodiceTutore.Value.ToString() : "";
                if (contenitore.DatiAnagraficiTutore.CessValAmmSost.HasValue)
                {
                    if (contenitore.DatiAnagraficiTutore.CessValAmmSost.Value.Year == 9999 && contenitore.DatiAnagraficiTutore.CessValAmmSost.Value.Month == 12)
                    {
                        tutore.T_GP1AP70A = "9999";
                        tutore.T_GP1AP70M = "99";
                    }
                    else
                    {
                        tutore.T_GP1AP70A = contenitore.DatiAnagraficiTutore.CessValAmmSost.Value.Year.ToString().PadLeft(4, '0');
                        tutore.T_GP1AP70M = contenitore.DatiAnagraficiTutore.CessValAmmSost.Value.Month.ToString().PadLeft(2, '0');
                    }
                }
            }
        }

        private static void ValorizzaFamiliari(ref EntityBLCommon.ContenitoreObject contenitore, Utility.TipoDomanda tipoDomanda, DateTime dataSistema, out Data.CAREPET.Familiari familiari)
        {
            familiari = new Data.CAREPET.Familiari();

            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = contenitore.ListaAventiDiritto;
            GestioneAventiDiritto.SortAventiDiritto(contenitore.DatiAreaTitolare.Anagrafica.CodiceFiscale, ref listaAventiDiritto, contenitore.ListaAnagraficaAventiDiritto);
            contenitore.ListaAventiDiritto = listaAventiDiritto;

            List<GestioneFamiliari.Familiare> listaFamiliari = contenitore.ListaFamiliari.ToList();
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = contenitore.ListaAnagraficaFamiliari.ToList();

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                familiari.LISTT_GP3 = new List<Data.CAREPET.Familiari.T_GP3>();
                int count = 0;
                GestioneFamiliari.Familiare titolare = listaFamiliari.Find(x => x.TipoComponente.HasValue && x.TipoComponente.Value == 'T');
                GestioneFamiliari.Familiare coniuge = listaFamiliari.Find(x => x.IsConiugeOrUnitoCivile());
                if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                {
                    listaFamiliari = (from ad in listaFamiliari
                                      join an in listaAnagraficaFamiliari on ad.IdAnagrafica equals an.Id
                                      orderby an.DataNascita
                                      select ad).ToList();
                    listaAnagraficaFamiliari = (from an in listaAnagraficaFamiliari
                                                orderby an.DataNascita
                                                select an).ToList();
                    if (coniuge != null)
                    {
                        listaFamiliari.Remove(coniuge);
                        listaFamiliari.Insert(0, coniuge);
                    }
                    if (titolare != null)
                    {
                        listaFamiliari.Remove(titolare);
                        if (!Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                            listaFamiliari.Insert(0, titolare);
                    }
                }
                else
                    listaFamiliari = listaFamiliari.OrderBy(x => x.Progressivo.GetValueOrDefault()).ToList();

                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    count++;
                    Data.CAREPET.Familiari.T_GP3 t_GP3 = new Data.CAREPET.Familiari.T_GP3();

                    GestioneAnagrafica.DatiAnagrafici datiAnagFam = contenitore.ListaAnagraficaFamiliari.Find(x => x.CodiceFiscale == fam.CodiceFiscale);

                    t_GP3.T_GP3CB08 = fam.CodiceFiscale;
                    //t_GP3.T_GP3CH01 = fam.SiglaFamiliare.HasValue ? fam.SiglaFamiliare.Value.ToString() : "";
                    //ENG - Spacchettate SOPGI
                    if ((Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                        || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)) && titolare != null && fam.CodiceFiscale == titolare.CodiceFiscale)
                    {
                        if (contenitore.DatiIstruttoria != null)
                        {
                            t_GP3.T_GP3CK20A = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.Value.Year : (short)0;
                            t_GP3.T_GP3CK20M = contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.HasValue ? (short)contenitore.DatiIstruttoria.ScadenzaRevisioneSanitaria.Value.Month : (short)0;
                        }
                    }
                    else
                    {
                        t_GP3.T_GP3CK20A = fam.ScadenzaRevisioneSanitaria.HasValue ? (short)fam.ScadenzaRevisioneSanitaria.Value.Year : (short)0;
                        t_GP3.T_GP3CK20M = fam.ScadenzaRevisioneSanitaria.HasValue ? (short)fam.ScadenzaRevisioneSanitaria.Value.Month : (short)0;
                    }
                    t_GP3.T_GP3CB12A_V = fam.DataMorte.HasValue ? (short)fam.DataMorte.Value.Year : (short)0;
                    t_GP3.T_GP3CB12M_V = fam.DataMorte.HasValue ? (short)fam.DataMorte.Value.Month : (short)0;
                    t_GP3.T_GP3CB12G_V = fam.DataMorte.HasValue ? (short)fam.DataMorte.Value.Day : (short)0;

                    //ENG - Spacchettate SOPGI
                    if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                        || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                    {
                        if (count == 1)
                        {
                            t_GP3.T_GP3CB09_V = "C";
                            t_GP3.T_GP3FTITPRN = "1";
                        }
                        else
                        {
                            t_GP3.T_GP3CB09_V = "F";
                            t_GP3.T_GP3FTITPRN = "0";
                        }
                    }
                    else if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                    {
                        t_GP3.T_GP3CB09_V = "C";
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || contenitore.IsRiaperturaDomanda)
                        {
                            if (fam.FlagTitolare.GetValueOrDefault() || fam.TipoComponente.GetValueOrDefault() == 'T')
                                t_GP3.T_GP3FTITPRN = "1";
                            else
                                t_GP3.T_GP3FTITPRN = "0";
                        }
                        else
                        {
                            if (count == 1)
                                t_GP3.T_GP3FTITPRN = "1";
                            else
                                t_GP3.T_GP3FTITPRN = "0";
                        }
                    }
                    else
                    {
                        if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || contenitore.IsRiaperturaDomanda) &&
                            fam.TipoComponente.GetValueOrDefault() == 'T' && !fam.SiglaFamiliare.HasValue)
                        {
                            t_GP3.T_GP3CB09_V = "T";
                            t_GP3.T_GP3FTITPRN = "1";
                        }
                        else
                        {
                            t_GP3.T_GP3CB09_V = "F";
                            t_GP3.T_GP3FTITPRN = "0";
                        }
                    }

                    t_GP3.T_GP3CB02 = datiAnagFam.Cognome;
                    t_GP3.T_GP3CB03 = datiAnagFam.Nome;
                    t_GP3.T_GP3CB04 = datiAnagFam.CognomeAcquisito;
                    t_GP3.T_GP3CB05 = datiAnagFam.Sesso.HasValue ? datiAnagFam.Sesso.Value.ToString() : "";
                    t_GP3.T_GP3CB06A = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Year : (short)0;
                    t_GP3.T_GP3CB06M = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Month : (short)0;
                    t_GP3.T_GP3CB06G = datiAnagFam.DataNascita.HasValue ? (short)datiAnagFam.DataNascita.Value.Day : (short)0;
                    t_GP3.T_GP3CB17 = datiAnagFam.ComuneNascita;
                    int codiceInpsComune = 0;
                    GestioneDBSComuni.GetCodInpsComuneByCodCatastale(datiAnagFam.CodiceComuneNascita, Utility.TipoAppartenenza.AGO.ToString(), 0, false, out codiceInpsComune);
                    t_GP3.T_GP3CB07 = codiceInpsComune;
                    if (datiAnagFam.ProvinciaNascita.Trim().Length >= 4)
                        t_GP3.T_GP3CB27 = "EE";
                    else
                        t_GP3.T_GP3CB27 = datiAnagFam.ProvinciaNascita.Trim();
                    t_GP3.T_GP3CB10 = datiAnagFam.Codice1Arca;
                    int resInt = 0;
                    int.TryParse(datiAnagFam.Codice2Arca, out resInt);
                    t_GP3.T_GP3CB11 = resInt;

                    List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliariParziali = null;
                    if (contenitore.ListaCodMaggFamiliari != null && contenitore.ListaCodMaggFamiliari.Count > 0)
                        listaCodMaggFamiliariParziali = contenitore.ListaCodMaggFamiliari.FindAll(x => x.IdAnagrafica == fam.IdAnagrafica && x.IdPensione == fam.IdPensione);

                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDirittoParziali = null;
                    GestioneAventiDiritto.AventiDiritto aventeDiritto = listaAventiDiritto.Find(x => x.IdAnagrafica == fam.IdAnagrafica);
                    if (aventeDiritto != null)
                        listaPeriodiAventiDirittoParziali = contenitore.ListaPeriodoAventiDiritto.FindAll(x => x.IdAventeDiritto == aventeDiritto.Id);

                    if (listaCodMaggFamiliariParziali != null && listaCodMaggFamiliariParziali.Count > 0)
                    {
                        t_GP3.LISTT_GP3CK = new List<Data.CAREPET.Familiari.T_GP3.T_GP3CK>();
                        for (int i = 0; i < listaCodMaggFamiliariParziali.Count; i++)
                        {
                            if (listaCodMaggFamiliariParziali[i].Decorrenza.HasValue || listaCodMaggFamiliariParziali[i].Cessazione.HasValue)
                            {
                                //Quando i dati vengono mandati al calcolo se esiste un elemento della Tabella
                                //Familiari (FATAB) con Sigla (FASIGLA) = a “N”, “J” la Data Fine Carico (FADACE)
                                //può essere modificata seguendo il seguente criterio:
                                //– si prende la data del Sistema
                                //– si aggiunge 1 all'anno
                                //– si muove 1 al mese (gennaio)
                                //– si confronta la Data Fine Carico (FADACE) acquisita con la data costruita in
                                //precedenza
                                //– se la Data Fine Carico (FADACE) è superiore a quella costruita la Data Fine
                                //Carico (FADACE) deve essere sostituita dalla Data costruita
                                if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione) &&
                                    fam.SiglaFamiliare.HasValue && (fam.SiglaFamiliare.Value == 'N' || fam.SiglaFamiliare.Value == 'J') &&
                                    listaCodMaggFamiliariParziali[i].Cessazione.HasValue)
                                {
                                    DateTime dataCompare = new DateTime(dataSistema.AddYears(1).Year, 1, dataSistema.AddYears(1).Day);
                                    if (Utility.DataStrettamenteSuccessivaA(listaCodMaggFamiliariParziali[i].Cessazione.Value, dataCompare))
                                        listaCodMaggFamiliariParziali[i].Cessazione = dataCompare;
                                }

                                Data.CAREPET.Familiari.T_GP3.T_GP3CK t_GP3CK = new Data.CAREPET.Familiari.T_GP3.T_GP3CK();
                                t_GP3CK.T_GP3CH01 = listaCodMaggFamiliariParziali[i].SiglaFamiliare.HasValue ? listaCodMaggFamiliariParziali[i].SiglaFamiliare.Value.ToString() : "";
                                t_GP3CK.T_GP3CK01A = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Year : (short)0;
                                t_GP3CK.T_GP3CK01M = listaCodMaggFamiliariParziali[i].Decorrenza.HasValue ? (short)listaCodMaggFamiliariParziali[i].Decorrenza.Value.Month : (short)0;
                                t_GP3CK.T_GP3CK02A = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Year : (short)0;
                                t_GP3CK.T_GP3CK02M = listaCodMaggFamiliariParziali[i].Cessazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].Cessazione.Value.Month : (short)0;
                                t_GP3CK.T_GP3CK04 = listaCodMaggFamiliariParziali[i].CodiceMaggiorazione.HasValue ? (short)listaCodMaggFamiliariParziali[i].CodiceMaggiorazione.Value : (short)0;
                                t_GP3CK.T_GP3CH01B = !string.IsNullOrEmpty(listaCodMaggFamiliariParziali[i].TipoUnione) && listaCodMaggFamiliariParziali[i].TipoUnione == "U" ? listaCodMaggFamiliariParziali[i].TipoUnione : null;
                                t_GP3.LISTT_GP3CK.Add(t_GP3CK);
                            }
                        }
                    }
                    else if (listaPeriodiAventiDirittoParziali != null && listaPeriodiAventiDirittoParziali.Count > 0)
                    {
                        t_GP3.LISTT_GP3CK = new List<Data.CAREPET.Familiari.T_GP3.T_GP3CK>();
                        foreach (var periodo in listaPeriodiAventiDirittoParziali)
                        {
                            Data.CAREPET.Familiari.T_GP3.T_GP3CK t_GP3CK = new Data.CAREPET.Familiari.T_GP3.T_GP3CK();
                            t_GP3CK.T_GP3CH01 = periodo.GradoParentela.HasValue ? periodo.GradoParentela.Value.ToString() : "";
                            t_GP3CK.T_GP3CH01B = !string.IsNullOrEmpty(periodo.TipoUnione) && periodo.TipoUnione == "U" ? periodo.TipoUnione : null;
                            t_GP3CK.T_GP3CK01A = periodo.DecorrenzaPeriodo.HasValue ? (short)periodo.DecorrenzaPeriodo.Value.Year : (short)0;
                            t_GP3CK.T_GP3CK01M = periodo.DecorrenzaPeriodo.HasValue ? (short)periodo.DecorrenzaPeriodo.Value.Month : (short)0;
                            t_GP3CK.T_GP3CK02A = periodo.CessazionePeriodo.HasValue ? (short)periodo.CessazionePeriodo.Value.Year : (short)9999;
                            t_GP3CK.T_GP3CK02M = periodo.CessazionePeriodo.HasValue ? (short)periodo.CessazionePeriodo.Value.Month : (short)99;
                            t_GP3CK.T_GP3CK04 = (short)0;
                            t_GP3.LISTT_GP3CK.Add(t_GP3CK);
                        }
                    }

                    familiari.LISTT_GP3.Add(t_GP3);
                }
            }
        }

        private static void ValorizzaErrori(out Data.CAREPET.Errori errori)
        {
            errori = new Data.CAREPET.Errori();
        }

        private static void ValorizzaDatiNuovi(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out Data.CAREPET.DatiNuovi datiNuovi)
        {
            datiNuovi = new Data.CAREPET.DatiNuovi();

            if (contenitore.DatiAreaTitolare.Anagrafica != null)
            {
                if (!string.IsNullOrEmpty(contenitore.DatiAreaTitolare.Anagrafica.Cittadinanza))
                {
                    if (contenitoreDecodifica.ElencoStatoEstero != null && contenitoreDecodifica.ElencoStatoEstero.Count > 0)
                    {
                        //ENG - Memo 48_2023
                        if (Utility.IsTitolareResidente_Cittadino_Bulgaria(contenitore.DatiPensione, contenitore.DatiAreaTitolare.Anagrafica))
                            datiNuovi.T_GP1AXBA = "BG";
                        else
                        {
                            string app = contenitore.DatiAreaTitolare.Anagrafica.Cittadinanza;
                            GestioneDecodifica.StatoEstero statoEstero = contenitoreDecodifica.ElencoStatoEstero.Find(x => x.CodCatastale == app);
                            if (statoEstero != null)
                                datiNuovi.T_GP1AXBA = !string.IsNullOrEmpty(statoEstero.Sigla) ? statoEstero.Sigla.Trim() == "ITA" ? "I" : statoEstero.Sigla.Trim() : string.Empty;
                        }
                    }
                }
            }

            if (Utility.IsDomandaVOCRED_CRED27__DAP(contenitore.DatiPensione) || (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(contenitore.DatiPensione) && contenitore.DatiPensione.CodiceTipoRichiesta == "71"))
                datiNuovi.T_GP1AV91H = 9;
            else if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria))
                datiNuovi.T_GP1AV91H = contenitore.DatiStoricoGP.GP1AV91H != null ? (short)contenitore.DatiStoricoGP.GP1AV91H : (short)0;
        }

        private static void ValorizzaCoda(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, Utility.TipoDomanda tipoDomanda,
            DateTime? inail_CessazioneAssegnoAccompangamento, out Data.CAREPET.Coda coda)
        {
            coda = new Data.CAREPET.Coda();

            coda.T_ENPALS = Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) ? "S" : "N";
            coda.AreaDati2006 = new Data.CAREPET.Coda.Dati2006();
            coda.AreaDati2007 = new Data.CAREPET.Coda.Dati2007();
            coda.AreaDati2008 = new Data.CAREPET.Coda.Dati2008();
            coda.AreaDati2009 = new Data.CAREPET.Coda.Dati2009();
            coda.AreaDati2010 = new Data.CAREPET.Coda.Dati2010();
            coda.AreaDati2012 = new Data.CAREPET.Coda.Dati2012();
            coda.AreaDati2013 = new Data.CAREPET.Coda.Dati2013();
            coda.AreaDati2014 = new Data.CAREPET.Coda.Dati2014();
            coda.AreaDati2016 = new Data.CAREPET.Coda.Dati2016();
            coda.AreaDati2017 = new Data.CAREPET.Coda.Dati2017();
            coda.AreaDati2018 = new Data.CAREPET.Coda.Dati2018();
            coda.AreaDati2019 = new Data.CAREPET.Coda.Dati2019();
            coda.AreaDati2020 = new Data.CAREPET.Coda.Dati2020();
            coda.AreaDati2021 = new Data.CAREPET.Coda.Dati2021();

            GestioneControlliDinamici.ControlloDinamico ctrl28_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrl28_2024);

            GestioneControlliDinamici.ControlloDinamico ctrl06_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo06_2024", out ctrl06_2024);

            //ENG - Implementazione Meta Processo
            GestioneControlliDinamici.ControlloDinamico ctrl_SbloccaMetaProcesso = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrl_SbloccaMetaProcesso);

            //Per tutte le domande della linea AGO, che abbiano titolare residente all’estero
            if (contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.HasValue && contenitore.DatiAreaTitolare.Anagrafica.ResidenzaEstero.Value)
                coda.AreaDati2006.T_STATOESTERO = contenitore.DatiAreaTitolare.Anagrafica.ComuneResidenza;

            coda.AreaDati2007.T_GP1AN87D = "000000";
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(contenitore.DatiPensione, true);
            bool abilitaNuovoFlusso = IsFlussoAdeguata(contenitoreDecodifica.ElencoCtrlCatAdeguata, contenitore.DatiPensione.SiglaCategoria != null ? contenitore.DatiPensione.SiglaCategoria.Trim() : string.Empty, contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto, contenitore.DatiPensione.Tipo, Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda));
            bool variazioneDatiCalcolo = false; //calcolo il dato solo per RIC/TFR manuali

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo93 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo93", out ctrlAbilitazioneMemo93);

            if (abilitaNuovoFlusso && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
            {
                //lo valorizzo prima per utilizzarlo anche per PL_Coeftrasf
                variazioneDatiCalcolo = CheckVariazioneDatiNumericiDatiCalcolo(contenitore.ListaDatiRetributivi, contenitore.ListaDatiRetributiviStorico, contenitore.ListaDatiContributivi, contenitore.ListaDatiContributiviStorico, contenitore.ListaDatiQuotaFondoIntegrativo, contenitore.ListaDatiQuotaFondoIntegrativoStorico);

                if (variazioneDatiCalcolo)
                {
                    coda.AreaDati2010.T_UNICARPE_V = string.Empty;
                }
                else
                {
                    coda.AreaDati2010.T_UNICARPE_V = "V";
                }
            }
            else
            {
                coda.AreaDati2010.T_UNICARPE_V = tipoUnicarpe == Utility.TipoUnicarpe.Automatica ? "U" : string.Empty;
            }

            coda.AreaDati2013.T_GP2PCANT = "N";

            coda.AreaDati2016.T_GP1DGRP = contenitore.DatiPensione.Gruppo;
            coda.AreaDati2016.T_GP1DPRD = contenitore.DatiPensione.Prodotto;
            coda.AreaDati2016.T_GP1DTIP = contenitore.DatiPensione.Tipo;
            // Commentato a seguito della mail del 28 novembre con oggetto: LIQPENS Ago - Intervento pro nuova competenza 2017
            //coda.AreaDati2016.T_GP1CENTINT = short.Parse(datiPensione.Fondo);
            coda.AreaDati2016.T_GP1DTIPOL = contenitore.DatiPensione.GetFiltro();

            if (contenitore.DatiLavorazione != null)
                coda.AreaDati2016.T_GP1DFASE = contenitore.DatiLavorazione.CodFase;

            if (contenitore.DatiPagamento != null)
            {
                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !contenitore.IsRiaperturaDomanda)
                {
                    coda.AreaDati2007.T_GP1BIC = contenitore.DatiPagamento.BIC;
                    coda.AreaDati2007.T_GP1IBAN = !string.IsNullOrEmpty(contenitore.DatiPagamento.IBAN) ? contenitore.DatiPagamento.IBAN.ToUpperInvariant() : string.Empty;
                    if (contenitore.DatiPagamento.TipoPagamento.GetValueOrDefault() == 'P' &&
                        contenitore.DatiPagamento.ModalitaPagamento.GetValueOrDefault() == 'L' && string.IsNullOrEmpty(coda.AreaDati2007.T_GP1IBAN))
                        coda.AreaDati2007.T_GP1IBAN = !string.IsNullOrEmpty(contenitore.DatiPagamento.Libretto) ? contenitore.DatiPagamento.Libretto.ToUpperInvariant() : string.Empty;
                }

                coda.AreaDati2007.T_GP1AN87A = contenitore.DatiPagamento.TrattenutaInpdap.HasValue ? contenitore.DatiPagamento.TrattenutaInpdap.Value ? "SI" : "NO" : string.Empty;
                coda.AreaDati2007.T_GP1AN87D = contenitore.DatiPagamento.DataRinunciaTrattenutaInpdap.HasValue ?
                    (contenitore.DatiPagamento.DataRinunciaTrattenutaInpdap.Value.Year.ToString().PadLeft(4, '0') +
                    contenitore.DatiPagamento.DataRinunciaTrattenutaInpdap.Value.Month.ToString().PadLeft(2, '0')) : "000000";
            }

            //GP1AN87B - ENPALS
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                string datoGP1AN87B = string.Empty;

                if (contenitore.DatiEnpals != null)
                    datoGP1AN87B = contenitore.DatiEnpals.GP1AN87B;

                //if (string.IsNullOrEmpty(datoGP1AN87B))
                //{
                //    if (datiCalcoloENPALS != null && datiCalcoloENPALS.ImportoProRataTemporis.HasValue) //Importo pto rata temporis valorizzato
                //    {
                //        datoGP1AN87B = "RT";
                //    }
                //    else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0009") || // 0001, 0002, 0009 (Pensione supplementare di vecchiaia)
                //        (contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0013" && contenitore.DatiPensione.Tipo == "0009") || // 0002, 0013, 0009 (Pensione supplementare di invalidità)
                //        (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0021" && contenitore.DatiPensione.Tipo == "0009") || // 0003, 0021, 0009 (Pensione supplementare di reversibilita')
                //        (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0022" && contenitore.DatiPensione.Tipo == "0009")) // 0003, 0022, 0009 (Pensione supplementare indiretta)
                //    {
                //        datoGP1AN87B = "PS";
                //    }
                //    else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0158") || // 0001, 0001, 0158
                //        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0159") || // 0001, 0001, 0159
                //        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0159")) // 0001, 0002, 0159
                //    {
                //        datoGP1AN87B = "VT";
                //    }
                //}

                if (string.IsNullOrEmpty(datoGP1AN87B) && !Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                    !Utility.IsDomandaReversibilita(contenitore.DatiPensione))
                {
                    if (contenitore.DatiCalcoloRetributivoENPALS != null && contenitore.DatiCalcoloRetributivoENPALS.ImportoProRataTemporis.HasValue) //Importo pto rata temporis valorizzato
                    {
                        datoGP1AN87B = "0T";
                    }
                    else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0009") || // 0001, 0002, 0009 (Pensione supplementare di vecchiaia)
                        (contenitore.DatiPensione.Gruppo == "0002" && contenitore.DatiPensione.Prodotto == "0013" && contenitore.DatiPensione.Tipo == "0009") || // 0002, 0013, 0009 (Pensione supplementare di invalidità)
                        (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0021" && contenitore.DatiPensione.Tipo == "0009") || // 0003, 0021, 0009 (Pensione supplementare di reversibilita')
                        (contenitore.DatiPensione.Gruppo == "0003" && contenitore.DatiPensione.Prodotto == "0022" && contenitore.DatiPensione.Tipo == "0009")) // 0003, 0022, 0009 (Pensione supplementare indiretta)
                    {
                        datoGP1AN87B = "0S";
                    }
                    else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0158") || // 0001, 0001, 0158
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0159") || // 0001, 0001, 0159
                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0159")) // 0001, 0002, 0159
                    {
                        datoGP1AN87B = "0V";
                    }
                    else if (Utility.GetTipoSalvaguardia(contenitore.DatiPensione) != Utility.TipoSalvaguardia.Nessuna)
                        datoGP1AN87B = "0G";
                    else if ((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0002" && contenitore.DatiPensione.Tipo == "0008")) // 0001, 0002, 0008
                        datoGP1AN87B = "VA";
                }

                coda.AreaDati2007.T_GP1AN87B = datoGP1AN87B;
            }

            if (contenitore.DatiIstruttoria != null)
            {
                //Memo 79
                if (contenitore.DatiIstruttoria.NSettimaneOI.HasValue)
                {
                    decimal settimaneoi = 2M;
                    Decimal.TryParse(contenitore.DatiIstruttoria.NSettimaneOI.Value.ToString("F2"), out settimaneoi);
                    if (coda.AreaDati2012.LISTT_GP2BM10 == null)
                    {
                        coda.AreaDati2012.LISTT_GP2BM10 = new List<Data.CAREPET.Coda.Dati2012.T_GP2BM10>();
                        coda.AreaDati2012.LISTT_GP2BM10.Add(new Data.CAREPET.Coda.Dati2012.T_GP2BM10());
                    }
                    coda.AreaDati2012.LISTT_GP2BM10.First().T_GP2BM13 = settimaneoi;
                    if (contenitore.DatiIstruttoria.NSettimaneOI.Value > 0)
                    {
                        coda.AreaDati2012.LISTT_GP2BM10.First().T_GP2BMTA = "OI";
                    }
                }
                //

                coda.AreaDati2007.T_GP1AV56AA = contenitore.DatiIstruttoria.DecorrenzaCaricoPrecedentePensione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaCaricoPrecedentePensione.Value.Year : (short)0;
                coda.AreaDati2007.T_GP1AV56MM = contenitore.DatiIstruttoria.DecorrenzaCaricoPrecedentePensione.HasValue ? (short)contenitore.DatiIstruttoria.DecorrenzaCaricoPrecedentePensione.Value.Month : (short)0;
                coda.AreaDati2010.T_ESENZVITTIME = contenitore.DatiIstruttoria.CodiceComunicazioneCampo4.HasValue ? contenitore.DatiIstruttoria.CodiceComunicazioneCampo4.Value == 1 ? "SI" : "" : "";
                if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && coda.AreaDati2010.T_ESENZVITTIME == "" && contenitore.DatiDetrazioni != null &&
                    contenitore.DatiDetrazioni.DetrazioniReddito.HasValue && contenitore.DatiDetrazioni.DetrazioniReddito.Value == 3)
                    coda.AreaDati2010.T_ESENZVITTIME = "SI";
                else if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                    Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                    Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione))
                {
                    coda.AreaDati2010.T_ESENZVITTIME = "SI";
                }
                coda.AreaDati2010.T_ESENZESTERO = contenitore.DatiIstruttoria.CodiceComunicazioneCampo4.HasValue ? contenitore.DatiIstruttoria.CodiceComunicazioneCampo4.Value == 2 ? "SI" : "NO" : "NO";
                //Intervento, se viene selezionato "Nessuna Esenzione" il campo andrà valorizzato a NO.
                if (!Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                    !(Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria) ||
                      Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) ||
                      Utility.IsDomandaCRED27(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaCOOP28(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) ||
                      Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria)) &&
                    coda.AreaDati2010.T_ESENZESTERO == "NO" && contenitore.DatiDetrazioni != null && contenitore.DatiDetrazioni.DetrazioniReddito.HasValue && contenitore.DatiDetrazioni.DetrazioniReddito.Value == 2)
                    coda.AreaDati2010.T_ESENZESTERO = "SI";
                //Richiesta 20151221 (MAIL Pasquale Cozzolino oggetto: 'FW: LiqPens AGO - Segnalazioni')
                if (Utility.IsCategoriaAutonomi(contenitore.DatiPensione.SiglaCategoria.Trim().ToUpperInvariant()))
                {
                    coda.AreaDati2007.T_GP2BN03 = contenitore.DatiIstruttoria.NContributiVolontari.HasValue ? (short)contenitore.DatiIstruttoria.NContributiVolontari.Value : (short)0;
                    coda.AreaDati2007.T_GP2BN04 = contenitore.DatiIstruttoria.NContributiVVAnzianita.HasValue ? (short)contenitore.DatiIstruttoria.NContributiVVAnzianita.Value : (short)0;
                }

                coda.AreaDati2013.T_GP2BL10E = contenitore.DatiIstruttoria.RiduzioneAssegno.HasValue ? contenitore.DatiIstruttoria.RiduzioneAssegno.Value : 0M;
            }

            coda.AreaDati2008.T_GP1RICDOMA = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Year;
            coda.AreaDati2008.T_GP1RICDOMM = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Month;
            coda.AreaDati2008.T_GP1RICDOMG = (short)contenitore.DatiPensione.DataPresentazioneDomanda.Day;

            if (!Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria)
                && !Utility.IsDomandaVOCRED(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaVOCOOP(contenitore.DatiPensione.SiglaCategoria)
                && !(Utility.IsDomandaVESO33(contenitore.DatiPensione.SiglaCategoria) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                && !Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
            {
                coda.AreaDati2008.T_GP1INTLEGA = contenitore.DatiPensione.DataCompletezza.HasValue ? (short)contenitore.DatiPensione.DataCompletezza.Value.Year : (short)0;
                coda.AreaDati2008.T_GP1INTLEGM = contenitore.DatiPensione.DataCompletezza.HasValue ? (short)contenitore.DatiPensione.DataCompletezza.Value.Month : (short)0;
                coda.AreaDati2008.T_GP1INTLEGG = contenitore.DatiPensione.DataCompletezza.HasValue ? (short)contenitore.DatiPensione.DataCompletezza.Value.Day : (short)0;
            }

            if (contenitore.DatiAreaTitolare.Patronato != null)
            {
                short codEnte = 0;
                short.TryParse(contenitore.DatiAreaTitolare.Patronato.CodiceEnte, out codEnte);
                coda.AreaDati2008.T_GP1RICPCOD = codEnte;
                int nPratica = 0;
                int.TryParse(contenitore.DatiAreaTitolare.Patronato.NPratica, out nPratica);
                if (nPratica.ToString().Length <= 8)
                    coda.AreaDati2008.T_GP1RICPNUM = nPratica;
                if (!string.IsNullOrEmpty(contenitore.DatiAreaTitolare.Patronato.TipoUfficio) && !string.IsNullOrEmpty(contenitore.DatiAreaTitolare.Patronato.TipoUfficio.Trim()))
                {
                    if (contenitoreDecodifica.ElencoCtrlTipoUfficio != null && contenitoreDecodifica.ElencoCtrlTipoUfficio.Count > 0)
                    {
                        string tipoUfficio = contenitore.DatiAreaTitolare.Patronato.TipoUfficio;
                        GestioneDecodifica.CtrlTipoUfficio ctrlTipoUfficio = contenitoreDecodifica.ElencoCtrlTipoUfficio.Find(x => x.CodTipoUfficio.ToUpperInvariant().Trim() == tipoUfficio.ToUpperInvariant().Trim());
                        if (ctrlTipoUfficio != null)
                            coda.AreaDati2008.T_GP1RICPTUFF = ctrlTipoUfficio.TraduzioneSuGP;
                    }
                }

                coda.AreaDati2008.T_GP1RICPZON = contenitore.DatiAreaTitolare.Patronato.CodiceUfficio;
            }

            if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                coda.AreaDati2012.T_GP1ALZ6 = contenitore.DatiPensione.CodiceSedeGP1ALZ6.GetValueOrDefault().ToString().PadLeft(4, '0') + (contenitore.DatiPensione.CentroOperativoGP1ALZ6.HasValue ? contenitore.DatiPensione.CentroOperativoGP1ALZ6.Value.ToString().PadLeft(2, '0') : "00");
            else
                coda.AreaDati2012.T_GP1ALZ6 = contenitore.DatiPensione.CodiceSedeDestinazione.HasValue ?
                    contenitore.DatiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') + (contenitore.DatiPensione.CentroOperativoDestinazione.HasValue ? contenitore.DatiPensione.CentroOperativoDestinazione.Value.ToString().PadLeft(2, '0') : "00") :
                    contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0') + (contenitore.DatiPensione.CentroOperativo.HasValue ? contenitore.DatiPensione.CentroOperativo.Value.ToString().PadLeft(2, '0') : "00");

            if (contenitore.DatiPensioniDatiGenerici != null)
            {
                coda.AreaDati2013.T_GP2PCANT = contenitore.DatiPensioniDatiGenerici.RiduzioneRetributiva ? "S" : "N";
                coda.AreaDati2013.T_GP2PCPER = contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale.HasValue ? contenitore.DatiPensioniDatiGenerici.RiduzioneRetributivaPercentuale.Value : 0M;

                coda.AreaDati2013.T_GP2BH01E = contenitore.DatiPensioniDatiGenerici.AnzAl95.HasValue ? contenitore.DatiPensioniDatiGenerici.AnzAl95.Value : 0M;
                coda.AreaDati2013.T_GP2BL01E = contenitore.DatiPensioniDatiGenerici.QuotaAl95.HasValue ? contenitore.DatiPensioniDatiGenerici.QuotaAl95.Value : 0M;
                coda.AreaDati2008.T_GP2BM04A = contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.Value.Year : (short)0;
                coda.AreaDati2008.T_GP2BM04M = contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.Value.Month : (short)0;
                coda.AreaDati2008.T_GP2BM04G = contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.InizioUltimoLavoro.Value.Day : (short)0;
                coda.AreaDati2008.T_GP2BM05A = contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.Value.Year : (short)0;
                coda.AreaDati2008.T_GP2BM05M = contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.Value.Month : (short)0;
                coda.AreaDati2008.T_GP2BM05G = contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.HasValue ? (short)contenitore.DatiPensioniDatiGenerici.FineUltimoLavoro.Value.Day : (short)0;

                if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria))
                {
                    string codiceBancaEsodatiTraduzioneSuGP = string.Empty;
                    if (contenitore.DatiPensione.CodiceBancaEsodati.HasValue)
                    {
                        if (contenitoreDecodifica.ElencoDecAzienda != null && contenitoreDecodifica.ElencoDecAzienda.Count > 0)
                        {
                            short codiceBancaEsodati = contenitore.DatiPensione.CodiceBancaEsodati.Value;
                            GestioneDecodificaAzienda.DecAzienda decAzienda = contenitoreDecodifica.ElencoDecAzienda.Find(x => x.Id == codiceBancaEsodati);
                            if (decAzienda != null)
                                codiceBancaEsodatiTraduzioneSuGP = decAzienda.TraduzioneSuGP;
                        }
                    }

                    if (!string.IsNullOrEmpty(codiceBancaEsodatiTraduzioneSuGP) && contenitore.DatiPensioniDatiGenerici.AnnoBancaFideiussoria.HasValue &&
                        contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria.HasValue)
                    {
                        short annoBancaFideiussoria = contenitore.DatiPensioniDatiGenerici.AnnoBancaFideiussoria.Value;
                        byte progressvoBancaFideiussoria = contenitore.DatiPensioniDatiGenerici.ProgressivoBancaFideiussoria.Value;
                        GestioneBancheFideiussione.DecBancaFideiussione decBancaFideiussione = null;
                        GestioneBancheFideiussioneESPA.DecBancaFideiussione decBancaFideiussioneESPA = null;
                        if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria))
                        {
                            decBancaFideiussione = contenitoreDecodifica.ElencoDecBancaFideiussione.FirstOrDefault(x =>
                                x.CodiceAzienda == codiceBancaEsodatiTraduzioneSuGP && x.Anno == annoBancaFideiussoria && x.Progressivo == progressvoBancaFideiussoria);
                        }
                        else
                        {
                            decBancaFideiussioneESPA = contenitoreDecodifica.ElencoDecBancaFideiussioneESPA.FirstOrDefault(x =>
                                    x.CodiceAzienda == codiceBancaEsodatiTraduzioneSuGP && x.Anno == annoBancaFideiussoria && x.Progressivo == progressvoBancaFideiussoria);
                        }

                        if (decBancaFideiussione != null)
                        {
                            coda.AreaDati2014.T_GP1PRESO = progressvoBancaFideiussoria;
                            coda.AreaDati2014.T_GP1AAESO = annoBancaFideiussoria;
                            coda.AreaDati2014.T_GP1ABIFIDJ = decBancaFideiussione.ABI.GetValueOrDefault().ToString().PadLeft(5, '0');
                            coda.AreaDati2014.T_GP1CABFIDJ = decBancaFideiussione.CAB.GetValueOrDefault().ToString().PadLeft(7, '0');
                        }
                        else if (decBancaFideiussioneESPA != null)
                        {
                            coda.AreaDati2014.T_GP1PRESO = progressvoBancaFideiussoria;
                            coda.AreaDati2014.T_GP1AAESO = annoBancaFideiussoria;
                            coda.AreaDati2014.T_GP1ABIFIDJ = decBancaFideiussioneESPA.ABI.GetValueOrDefault().ToString().PadLeft(5, '0');
                            coda.AreaDati2014.T_GP1CABFIDJ = decBancaFideiussioneESPA.CAB.GetValueOrDefault().ToString().PadLeft(7, '0');
                        }
                    }
                }
                if (abilitaNuovoFlusso)
                {
                    if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                    {
                        //Per le manuali, invio solo TFR/RIC se non ci sono state variazioni nei dati calcolo
                        if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && !variazioneDatiCalcolo)
                        {
                            coda.AreaDati2020.T_GP2BB10_UNICO = contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf.GetValueOrDefault();
                        }
                    }
                    //Per le automatiche invio sempre
                    else
                    {
                        coda.AreaDati2020.T_GP2BB10_UNICO = contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf.GetValueOrDefault();
                    }
                }

                if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensioniDatiGenerici.TipologiaCumulo.HasValue)
                    coda.AreaDati2021.T_GP1AJTIPCUM = contenitore.DatiPensioniDatiGenerici.TipologiaCumulo.Value.ToString();
            }

            if (contenitore.DatiMaggiorazioniBenefici != null)
            {
                coda.AreaDati2007.T_GP1AF17AA = contenitore.DatiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue ? (short)contenitore.DatiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Year : (short)0;
                coda.AreaDati2007.T_GP1AF17MM = contenitore.DatiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue ? (short)contenitore.DatiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale.Value.Month : (short)0;
            }

            if (contenitore.ListaDatiOneri != null && contenitore.ListaDatiOneri.Count > 0)
            {
                coda.AreaDati2008.LISTT_ELTAB_GP2PB = new List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB>();

                foreach (GestioneOneri.DatiOneri o in contenitore.ListaDatiOneri)
                {
                    Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB onere = new Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB();

                    if (contenitoreDecodifica.ElencoDecCodeGruppoOnere != null && contenitoreDecodifica.ElencoDecCodeGruppoOnere.Count > 0)
                    {
                        GestioneDecodifica.GruppoOneri gruppoOneri = contenitoreDecodifica.ElencoDecCodeGruppoOnere.Find(x => x.Id == (o.IdCodeGruppo.HasValue ? o.IdCodeGruppo.Value : (long)0));
                        if (gruppoOneri != null)
                        {
                            short res = 0;
                            short.TryParse(gruppoOneri.Code, out res);
                            onere.T_GP2PBPLEG = res;
                        }
                    }
                    if (contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere != null && contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere.Count > 0)
                    {
                        GestioneDecodifica.SottoGruppoOneri sottoGruppoOneri = contenitoreDecodifica.ElencoDecCodeSottoGruppoOnere.Find(x => x.Id == (o.IdCodeSottoGruppo.HasValue ? o.IdCodeSottoGruppo.Value : (long)0));
                        if (sottoGruppoOneri != null)
                        {
                            short res = 0;
                            short.TryParse(sottoGruppoOneri.Code, out res);
                            onere.T_GP2PBPLEG1 = res;
                        }
                    }
                    if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione) || onere.T_GP2PBPLEG == 4400)
                    {
                        onere.T_GP2PBPVARA = o.Decorrenza.HasValue ? (short)o.Decorrenza.Value.Year : (short)0;
                        onere.T_GP2PBPVARM = o.Decorrenza.HasValue ? (short)o.Decorrenza.Value.Month : (short)0;
                    }
                    else
                    {
                        onere.T_GP2PBPVARA = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                        onere.T_GP2PBPVARM = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                    }
                    if (contenitore.DatiPensione.DecorrenzaOriginaria.HasValue && o.Scadenza.HasValue &&
                        Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, o.Scadenza.Value))
                    {
                        onere.T_GP2PBCESA = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                        onere.T_GP2PBCESM = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                        onere.T_GP2PBCESG = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Day : (short)0;
                    }
                    else
                    {
                        onere.T_GP2PBCESA = o.Scadenza.HasValue ? (short)o.Scadenza.Value.Year : (short)0;
                        onere.T_GP2PBCESM = o.Scadenza.HasValue ? (short)o.Scadenza.Value.Month : (short)0;
                        onere.T_GP2PBCESG = o.Scadenza.HasValue ? (short)o.Scadenza.Value.Day : (short)0;
                    }
                    onere.T_GP2PBPONR = o.Onere.GetValueOrDefault();
                    onere.T_GP2PBBSET = o.Settimane.GetValueOrDefault();

                    if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true) ||
                        Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true))
                    {
                        if (contenitore.DatiPensione.NumeroFigli.HasValue)
                            onere.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;

                        if (!string.IsNullOrEmpty(coda.AreaDati2016.T_GP1DTIPOL))
                        {
                            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione, true, true))
                            {
                                if (coda.AreaDati2016.T_GP1DTIPOL == "KWA" || coda.AreaDati2016.T_GP1DTIPOL == "KXM")
                                    onere.T_GP2PBBPAR = 20;
                            }
                            else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione, true, true))
                            {
                                if (coda.AreaDati2016.T_GP1DTIPOL == "KYA" || coda.AreaDati2016.T_GP1DTIPOL == "KZM")
                                    onere.T_GP2PBBPAR = 21;
                            }
                            else if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione, true, true))
                            {
                                if (coda.AreaDati2016.T_GP1DTIPOL == "KUA" || coda.AreaDati2016.T_GP1DTIPOL == "KVM")
                                    onere.T_GP2PBBPAR = 22;
                            }
                        }
                    }

                    if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione) ||
                        Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione))
                    {
                        if (contenitore.DatiPensione.NumeroFigli.HasValue)
                            onere.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;

                        if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(contenitore.DatiPensione))
                            onere.T_GP2PBBPAR = 20;
                        else if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(contenitore.DatiPensione))
                            onere.T_GP2PBBPAR = 21;
                        else if (Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(contenitore.DatiPensione))
                            onere.T_GP2PBBPAR = 22;
                    }

                    //ENG - Memo 57_2023
                    GestioneControlliDinamici.ControlloDinamico controlloDinamicoAbilitazioneMemo57_2023 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out controlloDinamicoAbilitazioneMemo57_2023);
                    if (controlloDinamicoAbilitazioneMemo57_2023 != null && !String.IsNullOrEmpty(controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo) &&
                        controlloDinamicoAbilitazioneMemo57_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                    {

                        if (Utility.IsDomandaAPESociale(contenitore.DatiPensione.SiglaCategoria))
                        {
                            if (contenitore.DatiPensione.NumeroFigli.HasValue)
                                onere.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;
                        }
                    }

                    //ENG - RIC INPGI MIGRATE
                    if (Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)
                        && (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria))
                        && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2")
                    {
                        if (o.GP2PBB80.HasValue)
                            onere.T_GP2PBB80 = o.GP2PBB80.Value;
                    }

                    coda.AreaDati2008.LISTT_ELTAB_GP2PB.Add(onere);
                }
            }

            if (contenitore.ListaDatiBeneficiParticolari != null && contenitore.ListaDatiBeneficiParticolari.Count > 0 &&
                !(contenitore.DatiMaggiorazioniBenefici != null && contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))
            {
                if (coda.AreaDati2008.LISTT_ELTAB_GP2PB == null)
                    coda.AreaDati2008.LISTT_ELTAB_GP2PB = new List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB>();

                bool isSettimaneBeneficioFromAnzContribPost311295 = contenitore.DatiMaggiorazioniBenefici != null && (contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "16" || contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "17") ? true : false;
                for (int i = 0; i < contenitore.ListaDatiBeneficiParticolari.Count; i++)
                {
                    if (coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count > i)
                    {
                        // Ad oggi è permesso inserire soltanto un record benefici particolari e nel caso di Amianto 181, va inviato sul suo corrispettivo record
                        // Nel caso in cui ci dovesse essere una nuova gestione che prevede l'inserimento di più record benefici particolari bisognerà rivedere questa gestione
                        int j = i;
                        if (Utility.IsDomandaConBeneficioAmianto181(contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale))
                            j = coda.AreaDati2008.LISTT_ELTAB_GP2PB.FindIndex(x => x.T_GP2PBPLEG == 2000 && x.T_GP2PBPLEG1 == 2010);

                        short res = 0;
                        short.TryParse(contenitore.ListaDatiBeneficiParticolari[i].CodiceBenefici, out res);
                        coda.AreaDati2008.LISTT_ELTAB_GP2PB[j].T_GP2PBBPAR = res;
                        coda.AreaDati2008.LISTT_ELTAB_GP2PB[j].T_GP2PBPSET = contenitore.ListaDatiBeneficiParticolari[i].Settimane.GetValueOrDefault();
                        coda.AreaDati2008.LISTT_ELTAB_GP2PB[j].T_GP2PBBSET = contenitore.DatiMaggiorazioniBenefici != null ?
                            (isSettimaneBeneficioFromAnzContribPost311295 ? contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295.GetValueOrDefault() :
                            short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4))) : (short)0;
                        //ENG - RIC INPGI MIGRATE: escluse perchè il campo viene valorizzato in un altro modo
                        if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)
                            && (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria))
                            && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2"))
                            coda.AreaDati2008.LISTT_ELTAB_GP2PB[j].T_GP2PBB80 = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997.GetValueOrDefault() : (short)0;
                    }
                    else
                    {
                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB onere = new Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB();
                        short res = 0;
                        short.TryParse(contenitore.ListaDatiBeneficiParticolari[i].CodiceBenefici, out res);
                        onere.T_GP2PBBPAR = res;
                        onere.T_GP2PBPSET = contenitore.ListaDatiBeneficiParticolari[i].Settimane.GetValueOrDefault();
                        onere.T_GP2PBBSET = contenitore.DatiMaggiorazioniBenefici != null ?
                            (isSettimaneBeneficioFromAnzContribPost311295 ? contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295.GetValueOrDefault() :
                            short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4))) : (short)0;
                        //ENG - RIC INPGI MIGRATE: escluse perchè il campo viene valorizzato in un altro modo
                        if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)
                            && (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria))
                            && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2"))
                            onere.T_GP2PBB80 = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997.GetValueOrDefault() : (short)0;

                        coda.AreaDati2008.LISTT_ELTAB_GP2PB.Add(onere);
                    }
                }
            }
            // Se non è presente il beneficio tra i benefici particolari allora lo recupero dai campi TipoSettimaneBeneficio e NSettimaneBeneficio
            // Vedi mail del 28 novembre 2016 con oggetto: LIQPENS Ago - Intervento pro nuova competenza 2017
            else if (contenitore.DatiMaggiorazioniBenefici != null || contenitore.DatiIstruttoria != null)
            {
                string codCat = contenitore.DatiPensione.GetCodCategoria();
                if (!new List<string> { "070", "071", "072", "073", "074", "075" }.Contains(codCat))
                {
                    if ((contenitore.DatiMaggiorazioniBenefici != null && (!string.IsNullOrEmpty(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio) ||
                        contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.HasValue)) ||
                        (contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.Legge44997.HasValue))
                    {
                        if (coda.AreaDati2008.LISTT_ELTAB_GP2PB == null)
                            coda.AreaDati2008.LISTT_ELTAB_GP2PB = new List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB>();

                        Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab_gp2pb = null;

                        bool isDomandaRotabili = (Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(contenitore.DatiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(contenitore.DatiPensione)) &&
                                                  contenitore.DatiMaggiorazioniBenefici != null && contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "04";

                        if (coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count > 0)
                        {
                            eltab_gp2pb = coda.AreaDati2008.LISTT_ELTAB_GP2PB.Find(x => x.T_GP2PBBPAR == 0 && x.T_GP2PBBSET == 0);
                            if (eltab_gp2pb != null)
                            {
                                if (contenitore.DatiMaggiorazioniBenefici != null)
                                {
                                    short res = 0;
                                    short.TryParse(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio, out res);
                                    if (!Utility.IsDomandaAUT(contenitore.DatiPensione) ||
                                        (Utility.IsDomandaAUT(contenitore.DatiPensione) && (res == 12 || res == 15 || res == 14 || res == 18 || res == 19 || res == 24)))
                                    {
                                        eltab_gp2pb.T_GP2PBBPAR = res;

                                        if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
                                            eltab_gp2pb.T_GP2PBBSET = Convert.ToInt16(contenitore.DatiMaggiorazioniBenefici.NSettIntegrazioneContributivaConcessa.GetValueOrDefault());
                                        else
                                            eltab_gp2pb.T_GP2PBBSET = contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01" || isDomandaRotabili || Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione) ? contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295.GetValueOrDefault() :
                                                short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4));
                                        if (isDomandaRotabili || Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione))
                                            eltab_gp2pb.T_GP2PBPSET = short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4));
                                    }
                                    if (contenitore.DatiPensione.NumeroFigli.HasValue)
                                        eltab_gp2pb.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;
                                }
                                //ENG - RIC INPGI MIGRATE: escluse perchè il campo viene valorizzato in un altro modo
                                if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)
                                    && (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria))
                                    && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2"))
                                    eltab_gp2pb.T_GP2PBB80 = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997.GetValueOrDefault() : (short)0;
                            }
                        }

                        if (eltab_gp2pb == null)
                        {
                            eltab_gp2pb = new Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB();
                            short res = 0;
                            if (contenitore.DatiMaggiorazioniBenefici != null)
                            {
                                short.TryParse(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio, out res);
                                if (!Utility.IsDomandaAUT(contenitore.DatiPensione) ||
                                    (Utility.IsDomandaAUT(contenitore.DatiPensione) && (res == 12 || res == 15 || res == 14 || res == 18 || res == 19 || res == 24)))
                                {
                                    eltab_gp2pb.T_GP2PBBPAR = res;

                                    if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione))
                                        eltab_gp2pb.T_GP2PBBSET = Convert.ToInt16(contenitore.DatiMaggiorazioniBenefici.NSettIntegrazioneContributivaConcessa.GetValueOrDefault());
                                    else
                                        eltab_gp2pb.T_GP2PBBSET = contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01" || isDomandaRotabili || Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione) ? contenitore.DatiMaggiorazioniBenefici.SettAnzContribPost311295.GetValueOrDefault() :
                                            short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4));
                                    if (isDomandaRotabili || Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione))
                                        eltab_gp2pb.T_GP2PBPSET = short.Parse(contenitore.DatiMaggiorazioniBenefici.NSettimaneBeneficio.GetValueOrDefault().ToString().PadLeft(5, '0').Substring(1, 4));
                                }
                                if (contenitore.DatiPensione.NumeroFigli.HasValue)
                                    eltab_gp2pb.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;
                            }
                            if (!Utility.IsDomandaAUT(contenitore.DatiPensione) ||
                                (Utility.IsDomandaAUT(contenitore.DatiPensione) && (res == 12 || res == 15 || res == 14 || res == 18 || res == 19 || res == 24)))
                            {
                                eltab_gp2pb.T_GP2PBPVARA = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                                eltab_gp2pb.T_GP2PBPVARM = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                            }
                            //ENG - RIC INPGI MIGRATE: escluse perchè il campo viene valorizzato in un altro modo
                            if (!(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo)
                                && (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(contenitore.DatiPensione.SiglaCategoria))
                                && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2"))
                                eltab_gp2pb.T_GP2PBB80 = contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.Legge44997.GetValueOrDefault() : (short)0;

                            coda.AreaDati2008.LISTT_ELTAB_GP2PB.Add(eltab_gp2pb);
                        }
                    }
                    else
                    {
                        //ENG - Memo 28_2024
                        if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI" && contenitore.DatiMaggiorazioniBenefici == null)
                        {
                            if (((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0017") ||
                                        (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0045" && contenitore.DatiPensione.CodiceTipoRichiesta == "AV") ||
                                        (contenitore.DatiPensione.IdTipoPLPerRIC.HasValue && ((!string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) &&
                                        (contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "1" || contenitore.DatiPensione.NaturaPensione.Substring(0, 1) == "2") && contenitore.DatiPensione.IdTipoPLPerRIC == 7) || contenitore.DatiPensione.IdTipoPLPerRIC == 26))) &&
                                        ((contenitore.DatiPensione.TipoCalcolo.HasValue && contenitore.DatiPensione.TipoCalcolo == (byte)Utility.TipoCalcolo.Contributivo) || Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria)))
                            {
                                if (contenitore.DatiPensione.DecorrenzaOriginaria.HasValue
                                    && Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 1, 1)))
                                {
                                    if (coda.AreaDati2008.LISTT_ELTAB_GP2PB == null)
                                        coda.AreaDati2008.LISTT_ELTAB_GP2PB = new List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB>();

                                    Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB eltab_gp2pb = null;

                                    if (coda.AreaDati2008.LISTT_ELTAB_GP2PB != null && coda.AreaDati2008.LISTT_ELTAB_GP2PB.Count > 0)
                                    {
                                        eltab_gp2pb = coda.AreaDati2008.LISTT_ELTAB_GP2PB.Find(x => x.T_GP2PBBPAR == 0 && x.T_GP2PBBSET == 0);
                                        if (eltab_gp2pb != null)
                                        {
                                            if (contenitore.DatiPensione.NumeroFigli.HasValue)
                                            {
                                                eltab_gp2pb.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;

                                                //ENG - Figli senza benefici
                                                if (contenitore.DatiPensione.NumeroFigli.Value > 0)
                                                {
                                                    if (contenitore.DatiMaggiorazioniBenefici == null || String.IsNullOrEmpty(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                                                    {
                                                        eltab_gp2pb.T_GP2PBPVARA = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                                                        eltab_gp2pb.T_GP2PBPVARM = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (eltab_gp2pb == null)
                                    {
                                        if (contenitore.DatiPensione.NumeroFigli.HasValue)
                                        {
                                            eltab_gp2pb = new Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB();
                                            eltab_gp2pb.T_GP2PBNFGL = contenitore.DatiPensione.NumeroFigli.Value;

                                            //ENG - Figli senza benefici
                                            if (contenitore.DatiPensione.NumeroFigli.Value > 0)
                                            {
                                                if (contenitore.DatiMaggiorazioniBenefici == null || String.IsNullOrEmpty(contenitore.DatiMaggiorazioniBenefici.TipoSettimaneBeneficio))
                                                {
                                                    eltab_gp2pb.T_GP2PBPVARA = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year : (short)0;
                                                    eltab_gp2pb.T_GP2PBPVARM = contenitore.DatiPensione.DecorrenzaOriginaria.HasValue ? (short)contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month : (short)0;
                                                }
                                            }

                                            coda.AreaDati2008.LISTT_ELTAB_GP2PB.Add(eltab_gp2pb);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (contenitore.DatiDanteCausa != null)
            {
                coda.AreaDati2007.LISTT_ELTAB_GP7LC = new List<Data.CAREPET.Coda.Dati2007.T_ELTAB_GP7LC>();
                Data.CAREPET.Coda.Dati2007.T_ELTAB_GP7LC T_ELTAB_GP7LC = new Data.CAREPET.Coda.Dati2007.T_ELTAB_GP7LC();
                if (!string.IsNullOrEmpty(contenitore.DatiDanteCausa.StatoEEResidenza))
                {
                    string statoEEResidenza = contenitore.DatiDanteCausa.StatoEEResidenza;
                    GestioneDecodifica.StatoEstero statoEstero = contenitoreDecodifica.ElencoStatoEstero.Find(x => x.CodCatastale == statoEEResidenza);
                    if (statoEstero != null)
                        T_ELTAB_GP7LC.T_GP7LC61 = statoEstero.Sigla;
                }
                T_ELTAB_GP7LC.T_GP7LC62A = contenitore.DatiDanteCausa.DecorrenzaResidenza.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaResidenza.Value.Year : (short)0;
                T_ELTAB_GP7LC.T_GP7LC62M = contenitore.DatiDanteCausa.DecorrenzaResidenza.HasValue ? (short)contenitore.DatiDanteCausa.DecorrenzaResidenza.Value.Month : (short)0;
                coda.AreaDati2007.LISTT_ELTAB_GP7LC.Add(T_ELTAB_GP7LC);
            }

            if (contenitore.DatiAnagraficiDanteCausa != null)
            {
                coda.AreaDati2012.T_GP7LC42A = contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Year : (short)0;
                coda.AreaDati2012.T_GP7LC42M = contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Month : (short)0;
                coda.AreaDati2012.T_GP7LC42G = contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.HasValue ? (short)contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Day : (short)0;
            }

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
            {
                if (contenitore.DatiEnpals != null)
                {
                    if (coda.AreaDati2008.LISTT_ELTAB_GP2PB == null)
                        coda.AreaDati2008.LISTT_ELTAB_GP2PB = new List<Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB>();

                    Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB t_ELTAB_GP2PB = new Data.CAREPET.Coda.Dati2008.T_ELTAB_GP2PB();

                    if (contenitore.DatiEnpals.IndicatoreInvalidita80.HasValue)
                    {
                        short res = 0;
                        short.TryParse(contenitore.DatiEnpals.IndicatoreInvalidita80.Value.ToString(), out res);
                        t_ELTAB_GP2PB.T_GP2PBB80 = res;

                        coda.AreaDati2008.LISTT_ELTAB_GP2PB.Add(t_ELTAB_GP2PB);
                    }
                }
            }

            if (Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria))
            {
                string gp1aj10 = string.Empty;
                if (contenitore.DatiPensioniDatiGenerici != null && Utility.IsDomandaCumulo(contenitore.DatiPensione.SiglaCategoria))
                {
                    if (contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault())
                        gp1aj10 = "I";
                    else if (new List<char> { 'E', 'M' }.Contains(contenitore.DatiPensioniDatiGenerici.CumuloEsterno.GetValueOrDefault()))
                        gp1aj10 = contenitore.DatiPensioniDatiGenerici.CumuloEsterno.GetValueOrDefault().ToString();
                    coda.AreaDati2013.T_GP1AJ10 = gp1aj10;
                }

                if (Utility.IsDomandaTotalizzazione(contenitore.DatiPensione.SiglaCategoria) && contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0)
                {
                    List<string> lstCodiciEsterni = new List<string> { "F0", "F1", "G0", "H0", "I0", "J0", "K0", "L0", "N0", "O0", "P0", "Q0", "R0", "S0", "T0", "U0", "V0", "Z0", "Z1", "PR" };
                    if (ctrlAbilitazioneMemo93 != null && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93.ValoreControllo) && ctrlAbilitazioneMemo93.ValoreControllo.ToUpperInvariant().Trim() == "SI")
                        lstCodiciEsterni.Remove("F0");

                    var listaDecEnteGestioneFondo = contenitoreDecodifica.ElencoDecEnteGestioneFondo;
                    if (listaDecEnteGestioneFondo == null)
                        GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);

                    var lstRecordCodGestione = from Record in contenitore.ListaQuotePensione
                                               join dec in listaDecEnteGestioneFondo on Record.EnteGestioneFondo equals dec.Id
                                               select new { Record, TraduzioneSuGP = dec.Codice.Trim() };

                    if (lstRecordCodGestione.Where(x => lstCodiciEsterni.Contains(x.TraduzioneSuGP)).FirstOrDefault() != null)
                        gp1aj10 = "E";
                    else
                        gp1aj10 = "I";

                    coda.AreaDati2013.T_GP1AJ10 = gp1aj10;
                }


                if (contenitore.DatiPensione.SiglaCategoria == "VOCUM")
                {
                    if (contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue && contenitore.DatiPensione.DecorrenzaOriginaria.HasValue &&
                        Utility.DataSuccessivaA(contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value, contenitore.DatiPensione.DecorrenzaOriginaria.Value))
                        coda.AreaDati2017.T_GP1AV91C = 1;
                }

                if (contenitore.ListaTrattenuteQuotePensione != null && contenitore.ListaTrattenuteQuotePensione.Count > 0)
                {
                    coda.AreaDati2014.LISTT_TABTRATTOT = new List<Data.CAREPET.Coda.Dati2014.T_TABTRATTOT>();
                    foreach (GestioneCalcolo.TrattenuteQuotePensione t in contenitore.ListaTrattenuteQuotePensione)
                    {
                        string codiceGestione = contenitoreDecodifica.ElencoDecEnteGestioneFondo.FirstOrDefault(x => x.Id == t.EnteGestioneFondoQuote).Codice;
                        Data.CAREPET.Coda.Dati2014.T_TABTRATTOT.T_CONTRIB trattenute = new Data.CAREPET.Coda.Dati2014.T_TABTRATTOT.T_CONTRIB();
                        trattenute.T_ANNOTOT = t.AnnoCompetenza;
                        trattenute.T_CODTRAT = t.CodiceTrattenute;
                        trattenute.T_TRATTOT = t.ImportoTrattenute;
                        if (!coda.AreaDati2014.LISTT_TABTRATTOT.Exists(x => x.T_GESTOT == codiceGestione))
                        {
                            Data.CAREPET.Coda.Dati2014.T_TABTRATTOT datiTrattenute = new Data.CAREPET.Coda.Dati2014.T_TABTRATTOT();
                            datiTrattenute.T_GESTOT = codiceGestione;
                            datiTrattenute.LISTT_CONTRIB = new List<Data.CAREPET.Coda.Dati2014.T_TABTRATTOT.T_CONTRIB>();
                            datiTrattenute.LISTT_CONTRIB.Add(trattenute);
                            coda.AreaDati2014.LISTT_TABTRATTOT.Add(datiTrattenute);
                        }
                        else
                            coda.AreaDati2014.LISTT_TABTRATTOT.FirstOrDefault(x => x.T_GESTOT == codiceGestione).LISTT_CONTRIB.Add(trattenute);
                    }
                }

                if (!contenitore.IsRiaperturaDomanda)
                {
                    if (contenitore.DatiPensioniDatiGenerici != null)
                    {
                        if (!contenitore.DatiPensioniDatiGenerici.TipoCumulo.GetValueOrDefault() && contenitore.DatiPensioniDatiGenerici.CumuloEsterno.GetValueOrDefault() == 'E'
                            && !(Utility.IsDomandaIOCUM(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) && Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione))))
                        {
                            if (contenitore.ListaQuotePensione != null && contenitore.ListaQuotePensione.Count > 0)
                            {
                                DateTime? decorrenzaCompare = contenitore.ListaQuotePensione.OrderBy(x => x.Decorrenza).Last().Decorrenza;
                                if (contenitore.ListaQuotePensione.All(x => x.Decorrenza.HasValue && x.Decorrenza == decorrenzaCompare))
                                {
                                    coda.AreaDati2019.T_GP1AJ10Z = contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                                                   contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0');
                                    coda.AreaDati2019.T_GP1AJ10OLD = gp1aj10;
                                }
                                else
                                {
                                    coda.AreaDati2019.T_GP1AJ10Z = decorrenzaCompare.Value.Year.ToString().PadLeft(4, '0') +
                                                                   decorrenzaCompare.Value.Month.ToString().PadLeft(2, '0');
                                    coda.AreaDati2019.T_GP1AJ10OLD = "M";
                                }
                            }
                        }
                        else
                        {
                            coda.AreaDati2019.T_GP1AJ10Z = contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                                           contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0');
                            coda.AreaDati2019.T_GP1AJ10OLD = gp1aj10;
                        }
                    }
                }
            }

            if (Utility.IsDomandaAUT(contenitore.DatiPensione))
            {
                if (contenitore.DatiPensioniDatiGenerici != null)
                {
                    if (contenitore.DatiPensioniDatiGenerici.FacoltaComputo.HasValue)
                    {
                        if (contenitore.DatiPensioniDatiGenerici.FacoltaComputo.GetValueOrDefault())
                            coda.AreaDati2013.T_GP1AJ10 = "F";
                    }
                    //inserito come paracadute quando il quadro dati calcolo è giallo e non alimenta FacoltaComputo sulla tabella generici
                    else if (contenitore.DatiIstruttoria != null)
                    {
                        if (contenitore.DatiIstruttoria.FacoltaComputoPrecedentePensione != null)
                            coda.AreaDati2013.T_GP1AJ10 = contenitore.DatiIstruttoria.FacoltaComputoPrecedentePensione.ToString();
                    }
                }
            }

            if (Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) ||
                Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo))
            {
                if (contenitore.DatiBeneficioVittimeTerrorismo != null)
                {
                    if (contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.HasValue && contenitoreDecodifica.ElencoSoggettoBeneficiario != null && contenitoreDecodifica.ElencoSoggettoBeneficiario.Count > 0)
                    {
                        long soggettoBeneficiario = contenitore.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.Value;
                        GestioneDecodifica.SoggettoBeneficiario decSoggettoBeneficiario = contenitoreDecodifica.ElencoSoggettoBeneficiario.Find(x => x.Id == soggettoBeneficiario);
                        if (decSoggettoBeneficiario != null)
                        {
                            coda.AreaDati2009.T_GP1AC021 = decSoggettoBeneficiario.TraduzioneSuGP.Substring(0, 1);
                            coda.AreaDati2009.T_GP1AC022 = decSoggettoBeneficiario.TraduzioneSuGP.Substring(1, 1);
                            coda.AreaDati2009.T_GP1AC023 = decSoggettoBeneficiario.TraduzioneSuGP.Substring(2, 1);
                        }
                    }

                    coda.AreaDati2009.T_GP1AP35A = contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue ? (short)contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.Value.Year : (short)0;
                    coda.AreaDati2009.T_GP1AP35M = contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue ? (short)contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.Value.Month : (short)0;
                    coda.AreaDati2009.T_GP1AP35G = contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue ? (short)contenitore.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.Value.Day : (short)0;
                }
            }

            if (inail_CessazioneAssegnoAccompangamento.HasValue)
            {
                coda.AreaDati2007.T_GP2BACFAA = (short)inail_CessazioneAssegnoAccompangamento.Value.Year;
                coda.AreaDati2007.T_GP2BACFMM = (short)inail_CessazioneAssegnoAccompangamento.Value.Month;
            }
            else
            {
                coda.AreaDati2007.T_GP2BACFAA = (short)0;
                coda.AreaDati2007.T_GP2BACFMM = (short)0;
            }

            if (Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione))
            {
                coda.AreaDati2015.T_GP1AD03A = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Year : (short)0;
                coda.AreaDati2015.T_GP1AD03M = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Month : (short)0;
                coda.AreaDati2015.T_GP1AD03G = contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue ? (short)contenitore.DatiPensione.DataPerfezionamentoRequisiti.Value.Day : (short)0;
            }

            if (Utility.IsDomandaUnicarpe(contenitore.DatiPensione, false) == Utility.TipoUnicarpe.Yes)
                coda.AreaDati2017.T_GP1CARPE = "S";
            else
                coda.AreaDati2017.T_GP1CARPE = "N";

            // Se è presente la sede di destinazione (diversa dalla sede della domanda), bisogna indicare la sede di lavorazione della domanda
            if ((contenitore.DatiPensione.CodiceSedeDestinazione.HasValue &&
                (contenitore.DatiPensione.CodiceSedeDestinazione.Value != contenitore.DatiPensione.CodiceSede || contenitore.DatiPensione.CentroOperativoDestinazione.GetValueOrDefault() != contenitore.DatiPensione.CentroOperativo.GetValueOrDefault()))
                || (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                || (ctrl_SbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrl_SbloccaMetaProcesso.ValoreControllo) && ctrl_SbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI" && contenitore.DatiPensione.CodiceSedeLavorazione.HasValue))
            {
                string sedeDomanda = null;
                if (ctrl_SbloccaMetaProcesso != null && !String.IsNullOrEmpty(ctrl_SbloccaMetaProcesso.ValoreControllo) && ctrl_SbloccaMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI"
                    && contenitore.DatiPensione.CodiceSedeLavorazione.HasValue && contenitore.DatiPensione.CodiceSedeLavorazione.Value > 0)
                    coda.AreaDati2017.T_SEDE_DOMANDA = sedeDomanda = contenitore.DatiPensione.CodiceSedeLavorazione.ToString().PadLeft(4, '0') + contenitore.DatiPensione.CentroOperativo.GetValueOrDefault().ToString().PadLeft(2, '0');
                else
                    if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) &&
                    !Utility.isRicostituzioneOrRiaperturaPolarizzata(contenitore.DatiPensione, Utility.IsRiaperturaDomanda(contenitore.DatiPensione.Id)))
                        coda.AreaDati2017.T_SEDE_DOMANDA = sedeDomanda = contenitore.DatiPensione.CodiceSedeGP1ALZ6.ToString().PadLeft(4, '0') + contenitore.DatiPensione.CentroOperativo.ToString().PadLeft(2, '0');
                    else
                        coda.AreaDati2017.T_SEDE_DOMANDA = sedeDomanda = contenitore.DatiPensione.CodiceSede.ToString().PadLeft(4, '0') + contenitore.DatiPensione.CentroOperativo.GetValueOrDefault().ToString().PadLeft(2, '0');

                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SedePoloENPALS", out ctrl);
                string codiceSedePoloEnpals = string.Empty;
                if (ctrl != null && !String.IsNullOrEmpty(ctrl.ValoreControllo))
                    codiceSedePoloEnpals = ctrl.ValoreControllo.PadRight(6, '0');

                if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && sedeDomanda == codiceSedePoloEnpals)
                    coda.AreaDati2017.T_DES_SEDE_DOMANDA = "ROMA POLO PALS";
                else
                {
                    KeyValuePair<string, DNA.Office> sede = DNA.Context.OfficeList.Offices.FirstOrDefault(x => x.Value.AspnCode == sedeDomanda);
                    if (!sede.Equals(default(KeyValuePair<string, DNA.Office>)))
                        coda.AreaDati2017.T_DES_SEDE_DOMANDA = sede.Value.ExtendedProperties != null ? sede.Value.ExtendedProperties["SEDE"].Trim() : sede.Value.Name.Trim();
                    if (!String.IsNullOrEmpty(coda.AreaDati2017.T_DES_SEDE_DOMANDA) && coda.AreaDati2017.T_DES_SEDE_DOMANDA.Length > 22)
                        coda.AreaDati2017.T_DES_SEDE_DOMANDA = coda.AreaDati2017.T_DES_SEDE_DOMANDA.Substring(0, 22);
                }
            }

            //ENG - Memo 28_2024 0001-0001-0017 e 0001-0001-0045 con filtro "PAV" con decorrenza > 01.01.2024 e tipo di calcolo "contributivo"
            //GP1TPCLC con secondo byte uguale a 1
            if (ctrl28_2024 != null && !String.IsNullOrEmpty(ctrl28_2024.ValoreControllo) && ctrl28_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (!String.IsNullOrEmpty(contenitore.DatiPensione.Caratterizzazione))
                {
                    coda.AreaDati2018.T_GP1TPCLC = contenitore.DatiPensione.Caratterizzazione;
                }
                if (((contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0017") ||
                   (contenitore.DatiPensione.Gruppo == "0001" && contenitore.DatiPensione.Prodotto == "0001" && contenitore.DatiPensione.Tipo == "0045" && contenitore.DatiPensione.CodiceTipoRichiesta == "AV")) &&
                    (Utility.IsDomandaTipoContributivo(contenitore.DatiPensione, null, null) || Utility.IsDomandaTipoContributivoCumulo(contenitore.DatiPensione, null, null) ||
                    (contenitore.DatiPensione.TipoCalcolo.HasValue && contenitore.DatiPensione.TipoCalcolo == (byte)Utility.TipoCalcolo.Contributivo)) && contenitore.DatiPensione.DecorrenzaOriginaria.HasValue &&
                    Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensione.DecorrenzaOriginaria.Value, new DateTime(2024, 01, 01)))
                {
                    coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '1', 2);
                }
            }
            //ENG - Memo 06_2024
            if (ctrl06_2024 != null && !String.IsNullOrEmpty(ctrl06_2024.ValoreControllo) && ctrl06_2024.ValoreControllo.ToUpperInvariant() == "SI")
            {
                if (contenitore.DatiPensione.CodProPE.HasValue && contenitore.DatiPensione.CodProPE == 8)
                {
                    if (!String.IsNullOrEmpty(coda.AreaDati2018.T_GP1TPCLC))
                    {
                        coda.AreaDati2018.T_GP1TPCLC = "1" + coda.AreaDati2018.T_GP1TPCLC.Substring(1);
                    }
                    else
                    {
                        coda.AreaDati2018.T_GP1TPCLC = "1       ";
                    }
                }
            }


            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(contenitore.DatiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(contenitore.DatiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa))
            {
                if ((contenitore.ListaDatiContributiviINPGI != null && contenitore.ListaDatiContributiviINPGI.Count > 0) || (contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Count > 0))
                    coda.AreaDati2020.T_GP2BB10_UNICO = contenitore.DatiPensioniDatiGenerici.PL_Coeftrasf.GetValueOrDefault();
            }



            //ENG  - Memo 108_2024 (per la gestione del campo "CaratterizzazioneLegge" non serve la chiave "AbilitazioneMemo108_2024")
            if (Utility.IsDomandaVOCUM(contenitore.DatiPensione.SiglaCategoria))
            {
                if (!String.IsNullOrEmpty(contenitore.DatiPensione.Caratterizzazione) && !String.IsNullOrEmpty(contenitore.DatiPensione.Caratterizzazione.Trim()))
                {
                    //string caratterizzazione = contenitore.DatiPensione.Caratterizzazione;

                    //string primoCarattere = caratterizzazione.Substring(0, 1);
                    //switch (primoCarattere)
                    //{
                    //    case "1":
                    //        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '2', 2);
                    //        break;
                    //    case "2":
                    //        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '1', 3);
                    //        break;
                    //    case "3":
                    //        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '2', 1);
                    //        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '1', 3);
                    //        break;
                    //}

                    //if (primoCarattere == "1" || primoCarattere == "2" || primoCarattere == "3")
                    //{
                    //    if (contenitore.DatiPensione.CodProPE.HasValue == false || contenitore.DatiPensione.CodProPE != 8)
                    //    {
                    //        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, ' ', 1);
                    //    }
                    //}


                    if (contenitore.DatiPensione.Caratterizzazione.StartsWith("1"))
                    {
                        //coda.AreaDati2018.T_GP1TPCLC = " 2      ";
                        coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, '2', 2);
                        if (!(contenitore.DatiPensione.CodProPE.HasValue && contenitore.DatiPensione.CodProPE == 8))
                        {
                            coda.AreaDati2018.T_GP1TPCLC = Utility.InserisciValoreCaratterizzazione(coda.AreaDati2018.T_GP1TPCLC, ' ', 1);
                        }
                    }
                }
            }
        }

        private static void ValorizzaSPRDSC21(ref EntityBLCommon.ContenitoreObject contenitore, Data.CAREPET.DatiGenericiNew datiGenerici, out Data.CAREPET.SPRDSC21New sprdsc21, out string messaggioEccezione)
        {
            sprdsc21 = null;
            messaggioEccezione = string.Empty;
            GestioneControlliDinamici.ControlloDinamico ctrlBloccoGP4DC03_GP4DC02 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("BloccoGP4DC03<GP4DC02", out ctrlBloccoGP4DC03_GP4DC02);

            if (contenitore.DatiDanteCausa != null)
            {
                if (contenitore.DatiDanteCausa.CategoriaFascicolo.HasValue && contenitore.DatiDanteCausa.SedeFascicolo.HasValue && contenitore.DatiDanteCausa.NumeroFascicolo.HasValue)
                {
                    sprdsc21 = new Data.CAREPET.SPRDSC21New();
                    sprdsc21.T_GP4DAA1 = contenitore.DatiDanteCausa.CategoriaFascicolo.Value;
                    sprdsc21.T_GP4DAA2_1 = contenitore.DatiDanteCausa.SedeFascicolo.Value;
                    sprdsc21.T_GP4DAA2_2 = contenitore.DatiDanteCausa.NumeroFascicolo.Value;
                }
            }

            if (contenitore.ListaAventiDiritto != null && contenitore.ListaAventiDiritto.Count > 0)
            {
                if (contenitore.ListaPeriodoAventiDiritto != null && contenitore.ListaPeriodoAventiDiritto.Count > 0)
                {
                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDiritto = contenitore.ListaPeriodoAventiDiritto;
                    contenitore.ListaAventiDiritto.ForEach(x => x.ListaPeriodi = listaPeriodiAventiDiritto.FindAll(y => y.IdAventeDiritto == x.Id));
                }

                if (sprdsc21 == null)
                    sprdsc21 = new Data.CAREPET.SPRDSC21New();
                sprdsc21.LISTT_GP4DB00 = new List<Data.CAREPET.SPRDSC21New.T_GP4DB00>();

                foreach (var aventeDiritto in contenitore.ListaAventiDiritto)
                {
                    Data.CAREPET.SPRDSC21New.T_GP4DB00 gp4db00 = new Data.CAREPET.SPRDSC21New.T_GP4DB00();
                    GestioneAnagrafica.DatiAnagrafici anagraficaAventeDiritto = contenitore.ListaAnagraficaAventiDiritto.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    if (aventeDiritto.IdAnagrafica == contenitore.DatiAreaTitolare.Anagrafica.Id)
                    {
                        gp4db00.T_GP4KA01 = datiGenerici.T_GP1AB01_V;
                        gp4db00.T_GP4KA02 = datiGenerici.T_GP1AB02_V.ToString().PadLeft(4, '0').Substring(0, 2);
                        gp4db00.T_GP4KA03 = datiGenerici.T_GP1AB02_V.ToString().PadLeft(4, '0').Substring(2, 2);
                        gp4db00.T_GP4KA04 = datiGenerici.T_GP1AB03_V.ToString().PadLeft(8, '0');
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(aventeDiritto.CategoriaPensione))
                            gp4db00.T_GP4KA01 = aventeDiritto.CategoriaPensione;
                        if (aventeDiritto.SedePensione.HasValue)
                        {
                            gp4db00.T_GP4KA02 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(0, 2);
                            gp4db00.T_GP4KA03 = aventeDiritto.SedePensione.Value.ToString().PadLeft(4, '0').Substring(2, 2);
                        }
                        if (aventeDiritto.CertificatoPensione.HasValue)
                            gp4db00.T_GP4KA04 = aventeDiritto.CertificatoPensione.Value.ToString().PadLeft(8, '0');
                    }
                    if (anagraficaAventeDiritto != null && !string.IsNullOrEmpty(anagraficaAventeDiritto.CodiceFiscale))
                        gp4db00.T_GP4DB09 = anagraficaAventeDiritto.CodiceFiscale;
                    if (aventeDiritto.CSog.HasValue)
                        gp4db00.T_GP4DB13 = aventeDiritto.CSog.Value;
                    if (aventeDiritto.IdAnagrafica == contenitore.DatiAreaTitolare.Anagrafica.Id)
                    {
                        if (contenitore.DatiAnagraficiDanteCausa != null && contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.HasValue)
                        {
                            int data = 0;
                            int.TryParse(contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                                contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                                contenitore.DatiAnagraficiDanteCausa.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                            gp4db00.T_GP4DB14 = data;
                        }
                    }
                    else if (aventeDiritto.DataMatrimonio.HasValue)
                    {
                        int data = 0;
                        int.TryParse(aventeDiritto.DataMatrimonio.Value.Day.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Month.ToString().PadLeft(2, '0') +
                            aventeDiritto.DataMatrimonio.Value.Year.ToString().PadLeft(4, '0'), out data);
                        gp4db00.T_GP4DB14 = data;
                    }
                    gp4db00.T_GP4DB15 = aventeDiritto.CodiceNucleo;
                    if (aventeDiritto.ListaPeriodi != null && aventeDiritto.ListaPeriodi.Count > 0)
                    {
                        gp4db00.LISTT_GP4DC00 = new List<Data.CAREPET.SPRDSC21New.T_GP4DC00>();
                        int i = 1;
                        foreach (var periodo in aventeDiritto.ListaPeriodi)
                        {
                            Data.CAREPET.SPRDSC21New.T_GP4DC00 gp4dc00 = new Data.CAREPET.SPRDSC21New.T_GP4DC00();
                            if (periodo.PercSpettante.HasValue)
                                gp4dc00.T_GP4DC01 = periodo.PercSpettante.Value;
                            if (periodo.DecorrenzaPeriodo.HasValue)
                                gp4dc00.T_GP4DC02 = int.Parse(periodo.DecorrenzaPeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.DecorrenzaPeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            if (periodo.CessazionePeriodo.HasValue)
                                gp4dc00.T_GP4DC03 = int.Parse(periodo.CessazionePeriodo.Value.Year.ToString().PadLeft(4, '0') + periodo.CessazionePeriodo.Value.Month.ToString().PadLeft(2, '0'));
                            else
                            {
                                //ENG - Spacchettate SOPGI
                                if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                                    || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                                {
                                    int cessazioneMassima = Utility.CalcolaCessazioneMassimaAventeDiritto(aventeDiritto.DecParentelaDA, anagraficaAventeDiritto.DataNascita);
                                    if (i > 1 && !periodo.CessazionePeriodo.HasValue && periodo.GradoParentela.HasValue && periodo.GradoParentela.Value == 'I')
                                        gp4dc00.T_GP4DC03 = 999999;
                                    else
                                        if (cessazioneMassima > 0)
                                            gp4dc00.T_GP4DC03 = cessazioneMassima;
                                        else
                                            gp4dc00.T_GP4DC03 = 999999;
                                }
                                else
                                    gp4dc00.T_GP4DC03 = 999999;
                            }

                            //ENG - SPACCHETTATE AGO: gestione blocco cessazione periodo minore della decorrenza periodo
                            if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                                || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                            {
                                if (ctrlBloccoGP4DC03_GP4DC02 != null && !String.IsNullOrEmpty(ctrlBloccoGP4DC03_GP4DC02.ValoreControllo) && ctrlBloccoGP4DC03_GP4DC02.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                                {
                                    if (gp4dc00.T_GP4DC03 != 999999 && gp4dc00.T_GP4DC03 < gp4dc00.T_GP4DC02)
                                    {
                                        string dataCessazionePeriodo = gp4dc00.T_GP4DC03.ToString().Length == 6 ? gp4dc00.T_GP4DC03.ToString().Substring(4, 2) + "/" + gp4dc00.T_GP4DC03.ToString().Substring(0, 4) : string.Empty;
                                        string dataDecorrenzaPeriodo = gp4dc00.T_GP4DC02.ToString().Length == 6 ? gp4dc00.T_GP4DC02.ToString().Substring(4, 2) + "/" + gp4dc00.T_GP4DC02.ToString().Substring(0, 4) : string.Empty;
                                        string codiceFiscaleAventeDiritto = anagraficaAventeDiritto != null ? anagraficaAventeDiritto.CodiceFiscale : string.Empty;

                                        messaggioEccezione = String.Format("PER IL SOGGETTO {0} VERIFICARE RELAZIONE CON IL DANTE CAUSA. LA CESSAZIONE CALCOLATA ({1:MM/yyyy}) RISULTA ANTECEDENTE LA DECORRENZA ({2:MM/yyyy}).", codiceFiscaleAventeDiritto, dataCessazionePeriodo, dataDecorrenzaPeriodo);
                                        throw new INPS.DNA.DnaApplicationException(messaggioEccezione);
                                    }
                                }
                            }


                            if (periodo.GradoParentela.HasValue)
                            {
                                if (periodo.TipoUnione == "U")
                                    gp4dc00.T_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString() + periodo.TipoUnione;
                                else
                                    gp4dc00.T_GP4DC04 = periodo.GradoParentela.GetValueOrDefault().ToString();
                            }
                            if (periodo.CoeffRiduzione.HasValue)
                                gp4dc00.T_GP4DC05 = periodo.CoeffRiduzione.Value;
                            if (periodo.PercGiudice.HasValue)
                                gp4dc00.T_GP4DC07 = periodo.PercGiudice.Value;

                            gp4db00.LISTT_GP4DC00.Add(gp4dc00);
                            i++;
                        }
                    }
                    else //caso di avente diritto non richiedente senza periodi su GP4
                    {
                        gp4db00.LISTT_GP4DC00 = new List<Data.CAREPET.SPRDSC21New.T_GP4DC00>();
                        Data.CAREPET.SPRDSC21New.T_GP4DC00 gp4dc00 = new Data.CAREPET.SPRDSC21New.T_GP4DC00();
                        if (aventeDiritto.TipoUnione == "U")
                            gp4dc00.T_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString() + aventeDiritto.TipoUnione;
                        else
                            gp4dc00.T_GP4DC04 = aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString();

                        if (aventeDiritto.DecParentelaDA == 'M' && anagraficaAventeDiritto != null && anagraficaAventeDiritto.DataNascita > contenitore.DatiDanteCausa.DataMorte)
                        {
                            DateTime decorrenza = Utility.FirstDayOfMonth(anagraficaAventeDiritto.DataNascita.Value.AddMonths(1));
                            gp4dc00.T_GP4DC02 = int.Parse(decorrenza.Year.ToString().PadLeft(4, '0') + decorrenza.Month.ToString().PadLeft(2, '0'));
                        }
                        else
                            gp4dc00.T_GP4DC02 = int.Parse(contenitore.DatiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0') +
                                contenitore.DatiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0'));

                        //ENG - Spacchettate SOPGI
                        if (Utility.IsDomandaSpacchettamentoENPALS(contenitore.DatiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(contenitore.DatiPensione, contenitore.DatiDanteCausa) || Utility.IsDomandaSpacchettamentoSO(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda)
                            || Utility.IsDomandaSpacchettamentoSOART(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda))
                        {
                            int cessazioneMassima = Utility.CalcolaCessazioneMassimaAventeDiritto(aventeDiritto.DecParentelaDA, anagraficaAventeDiritto.DataNascita);
                            if (cessazioneMassima > 0)
                                gp4dc00.T_GP4DC03 = cessazioneMassima;
                            else
                                gp4dc00.T_GP4DC03 = 999999;

                            //ENG - SPACCHETTATE AGO: gestione blocco cessazione periodo minore della decorrenza periodo
                            if (ctrlBloccoGP4DC03_GP4DC02 != null && !String.IsNullOrEmpty(ctrlBloccoGP4DC03_GP4DC02.ValoreControllo) && ctrlBloccoGP4DC03_GP4DC02.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                            {
                                if (gp4dc00.T_GP4DC03 != 999999 && gp4dc00.T_GP4DC03 < gp4dc00.T_GP4DC02)
                                {
                                    string dataCessazionePeriodo = gp4dc00.T_GP4DC03.ToString().Length == 6 ? gp4dc00.T_GP4DC03.ToString().Substring(4, 2) + "/" + gp4dc00.T_GP4DC03.ToString().Substring(0, 4) : string.Empty;
                                    string dataDecorrenzaPeriodo = gp4dc00.T_GP4DC02.ToString().Length == 6 ? gp4dc00.T_GP4DC02.ToString().Substring(4, 2) + "/" + gp4dc00.T_GP4DC02.ToString().Substring(0, 4) : string.Empty;
                                    string codiceFiscaleAventeDiritto = anagraficaAventeDiritto != null ? anagraficaAventeDiritto.CodiceFiscale : string.Empty;

                                    messaggioEccezione = String.Format("PER IL SOGGETTO {0} VERIFICARE RELAZIONE CON IL DANTE CAUSA. LA CESSAZIONE CALCOLATA ({1:MM/yyyy}) RISULTA ANTECEDENTE LA DECORRENZA ({2:MM/yyyy}).", codiceFiscaleAventeDiritto, dataCessazionePeriodo, dataDecorrenzaPeriodo);
                                    throw new INPS.DNA.DnaApplicationException(messaggioEccezione);
                                }
                            }
                        }
                        else
                            gp4dc00.T_GP4DC03 = 999999;

                        gp4db00.LISTT_GP4DC00.Add(gp4dc00);
                    }

                    sprdsc21.LISTT_GP4DB00.Add(gp4db00);
                }
            }
        }

        private static void ValorizzaNuoviDati2024(ref EntityBLCommon.ContenitoreObject contenitore, out Data.CAREPET.NuoviDati2024 nuoviDati2024)
        {
            nuoviDati2024 = new Data.CAREPET.NuoviDati2024();

            if (contenitore.DatiPensione.DataCondizioniPerComputo.HasValue)
            {
                nuoviDati2024.AreaDati2024.T_GP1AJ10ZD = Convert.ToString(contenitore.DatiPensione.DataCondizioniPerComputo.Value).Substring(6, 4) + Convert.ToString(contenitore.DatiPensione.DataCondizioniPerComputo.Value).Substring(3, 2) + Convert.ToString(contenitore.DatiPensione.DataCondizioniPerComputo.Value).Substring(0, 2);
            }

            if (contenitore.DatiPensione.GP1AV91A.HasValue)
            {
                nuoviDati2024.AreaDati2024.T_GP1AV91A = contenitore.DatiPensione.GP1AV91A.Value;
            }

            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295.HasValue)
            {
                nuoviDati2024.AreaDatiGP2BO00.T_GP2BO05E = Convert.ToDecimal(contenitore.DatiPensioniDatiGenerici.ContributiItalianiEdEsteriAl1295);
            }

            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.ContribuzioneEsteraTotale.HasValue)
            {
                nuoviDati2024.AreaDatiGP2BO00.T_GP2BO08 = Convert.ToInt32(contenitore.DatiPensioniDatiGenerici.ContribuzioneEsteraTotale);
            }

            if (contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.TotaleSettimaneEstereUtiliPerDiritto.HasValue)
            {
                nuoviDati2024.AreaDatiGP2BO00.T_GP2BO09 = (short)contenitore.DatiPensioniDatiGenerici.TotaleSettimaneEstereUtiliPerDiritto;
            }

            //ENG - MEMO 74_2023 
            List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioneEstere = null;
            GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(contenitore.DatiPensione.Id, out listaPrestazioneEstere);

            List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> listaImportiPrestazioneEstere = null;
            GestioneDatiEsteriCumulo.GetImportiEsteriCumuloByIdPensione(contenitore.DatiPensione.Id, out listaImportiPrestazioneEstere);

            if (listaPrestazioneEstere != null && listaPrestazioneEstere.Count() > 0)
            {
                nuoviDati2024.LISTT_GP2BR00 = new List<Data.CAREPET.NuoviDati2024.DatiGP2BR00>();
                foreach (GestioneDatiEsteriCumulo.PensioneEsteraCumulo prestazioneEstera in listaPrestazioneEstere)
                {
                    Data.CAREPET.NuoviDati2024.DatiGP2BR00 statoEstero = new Data.CAREPET.NuoviDati2024.DatiGP2BR00();

                    if (!string.IsNullOrEmpty(prestazioneEstera.CodiceStato))
                    {
                        short resShort = 0;
                        short.TryParse(prestazioneEstera.CodiceStato, out resShort);
                        statoEstero.T_GP2BR02 = resShort;
                    }
                    if (!string.IsNullOrEmpty(prestazioneEstera.CodiceIstituzione))
                    {
                        short resShort = 0;
                        short.TryParse(prestazioneEstera.CodiceIstituzione, out resShort);
                        statoEstero.T_GP2BR03 = resShort;
                    }

                    if (!string.IsNullOrEmpty(prestazioneEstera.MatricolaEstera))
                        statoEstero.T_GP2BR04 = prestazioneEstera.MatricolaEstera;
                    if (prestazioneEstera.SettimaneMisura.HasValue)
                        statoEstero.T_GP2BR05 = prestazioneEstera.SettimaneMisura.Value;
                    if (prestazioneEstera.ContributiDiritto.HasValue)
                        statoEstero.T_GP2BR08 = short.Parse(prestazioneEstera.ContributiDiritto.Value.ToString());

                    nuoviDati2024.AreaDatiGP2BO00.T_GP2BO01 = listaPrestazioneEstere[0].CodiceConvenzione.HasValue ? (short)listaPrestazioneEstere[0].CodiceConvenzione.Value : contenitore.DatiPensioniDatiGenerici.CodiceConvenzioneAgo.GetValueOrDefault();

                    if (listaImportiPrestazioneEstere != null && listaImportiPrestazioneEstere.Count() > 0)
                    {
                        List<GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo> listaImportiEsteriPerStatoEstero = listaImportiPrestazioneEstere.FindAll(x => x.IdPensioneEsteraCumulo == prestazioneEstera.Id);
                        if (listaImportiEsteriPerStatoEstero != null && listaImportiEsteriPerStatoEstero.Count() > 0)
                        {
                            statoEstero.LISTT_GP2BR10N = new List<Data.CAREPET.NuoviDati2024.DatiGP2BR00.T_GP2BR10N>();
                            foreach (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importoEstero in listaImportiEsteriPerStatoEstero)
                            {
                                Data.CAREPET.NuoviDati2024.DatiGP2BR00.T_GP2BR10N importo = new Data.CAREPET.NuoviDati2024.DatiGP2BR00.T_GP2BR10N();
                                if (importoEstero.DecorrenzaPrestazione.HasValue)
                                {
                                    importo.T_GP2BR12SA = (short)importoEstero.DecorrenzaPrestazione.Value.Year;
                                    importo.T_GP2BR12M = (short)importoEstero.DecorrenzaPrestazione.Value.Month;
                                }
                                if (importoEstero.CessazionePrestazione.HasValue)
                                {
                                    importo.T_GP2BR13SA = (short)importoEstero.CessazionePrestazione.Value.Year;
                                    importo.T_GP2BR13M = (short)importoEstero.CessazionePrestazione.Value.Month;
                                }
                                if (importoEstero.ImportoPrestazione.HasValue)
                                    importo.T_GP2BR14N = importoEstero.ImportoPrestazione.Value;

                                statoEstero.LISTT_GP2BR10N.Add(importo);
                            }
                        }
                    }
                    nuoviDati2024.LISTT_GP2BR00.Add(statoEstero);
                }
            }
        }

        internal static void GetDecorrenzaRetr(string gestione, char quota, string tipoQuota, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDA,
            GestioneDatiControlloFelpe.ControlloFelpe controlloFelpe, List<CtrlDecorrenzaRetrExINPDAI> elencoCtrlDecorrenzaRetrExINPDAI, DateTime? decorrenzaOpzione,
            Utility.DifferenzaDateTime decorrenzaDatiRetributivi, decimal? rms, out short meseDec, out short annoDec)
        {
            meseDec = 0;
            annoDec = 0;

            bool posticipo = false;
            int panvein = 0;
            GestioneControlli.GetPanvein_Posticipo(datiPensione.NaturaPensione, datiPensione, out panvein, out posticipo);

            string siglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();
            DateTime decorrenzaOriginaria = DateTime.MinValue;
            string certificato = string.Empty;
            GestioneControlli.GetCertificato_DecorrenzaPensione(datiPensione, out decorrenzaOriginaria, out certificato);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            //Se il Campo DAU107 è = a “1” o “2” mettere il Campo DAU106AA in PAA
            if (datiDA != null && datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2) && datiDA.DecorrenzaPensione.HasValue)
                annoDec = (short)datiDA.DecorrenzaPensione.Value.Year;
            //altrimenti mettere il Campo RAU104AA in PAA
            else
                annoDec = (short)decorrenzaOriginaria.Year;
            //Se il Campo RAU113DA è maggiore di zero mettere il Campo RAU113DA in PAA
            if (decorrenzaOpzione.HasValue)
                annoDec = (short)decorrenzaOpzione.Value.Year;
            //Se la variabile W-POSTICIPO è = a “S” mettere il Campo INIZIOSCO(1:4) in PAA
            if (posticipo)
            {
                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                    annoDec = (short)controlloFelpe.InizioBonus.Value.Year;
                else
                    annoDec = 0;
            }

            if (decorrenzaDatiRetributivi != null)
                annoDec = (short)decorrenzaDatiRetributivi.Year;

            //var IsAnte96 = Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDA, Utility.IsRiaperturaDomanda(datiPensione.Id));
            //switch (IsAnte96)
            //{
            //    case Utility.TipoAnte96.Ante96Retributive:
            //        if (datiDA != null && datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2))
            //            meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
            //        else
            //            meseDec = (short)datiPensione.DecorrenzaOriginaria.Value.Month;
            //        return;
            //    default:
            //        break;
            //}

            //Il mese degli elementi presenti nella Tabella Retributiva contiene un numero che quando non è un mese dell'anno (01 -12) 
            //proveniente da altri Campi è un codice numerico identificato nei seguenti modi:
            //Se la Categoria (RACATEG) è = a “VO”, “IO”, “VOP”, “IOP”, “VOMIN”, “VMP”, “IMP”, 
            //(“PMO” con il 3° carattere del Certificato diverso da 3 o 6) oppure se la Categoria (RACATEG) è = “SO”, “SOP”, “SOMIN” 
            //(PMO” con il 3° carattere del Certificato = a 3 o 6) e il Campo DAU107 = a zero testare i Campi VRGEST, VRQUOTA e RAU113DA 
            //per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM il valore indicato: 
            //VRGEST VRQUOTA RAU113DA → VRDECMM 
            //“1” “A” 0 → RAU104MM
            //“1H” “A” 0 → RAU104MM
            //“S” “A” 0 → INIZIOSCO(5:2) 
            //“Q” “A” 0 → RAU104MM 
            //“1” “A” > 0 → RAU113DM 
            //“1H” “A” > 0 → RAU113DM
            //“Q” “A” > 0 → RAU113DM 
            //“1” “B” → 61
            //“1H” “B” → 61
            //“Q” “B” → 61 
            //“7” “A” → 99 
            //“7” “B” → 98 
            //“7” spazio → 99
            if (siglaCategoria == "VO" || siglaCategoria == "IO" || siglaCategoria == "VOP" ||
                siglaCategoria == "IOP" || siglaCategoria == "VOMIN" || siglaCategoria == "VMP" ||
                siglaCategoria == "IMP" || (siglaCategoria == "PMO" && certificato.Substring(2, 1) != "3" && certificato.Substring(2, 1) != "6") ||
                ((siglaCategoria == "SO" || siglaCategoria == "SOP" || siglaCategoria == "SOMIN" ||
                (siglaCategoria == "PMO" && (certificato.Substring(2, 1) == "3" || certificato.Substring(2, 1) == "6"))) &&
                (datiDA == null || !datiDA.ProvenienzaPensione.HasValue || datiDA.ProvenienzaPensione.Value == 0)))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                            case "Q":
                            case "1H":
                            case "P":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)decorrenzaOriginaria.Month;
                                //specializzazione ante96 
                                if (annoDec != 0 && annoDec < 1996 && decorrenzaDatiRetributivi != null)
                                    meseDec = (short)decorrenzaDatiRetributivi.Month;
                                break;
                            case "7":
                                meseDec = 99;
                                break;
                            case "S":
                                //ENG-annoDec valorizzato con anno di InizioBonus di Felpe per quota A e gestione "S"
                                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                                {
                                    meseDec = (short)controlloFelpe.InizioBonus.Value.Month;
                                    annoDec = (short)controlloFelpe.InizioBonus.Value.Year;
                                }
                                else if (decorrenzaDatiRetributivi != null)
                                    meseDec = (short)decorrenzaDatiRetributivi.Month;
                                else
                                    meseDec = 0;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                            case "Q":
                            case "1H":
                            case "P":
                                meseDec = 61;
                                //ENG-annoDec valorizzato con anno di InizioBonus di Felpe per quota B e gestione "1"
                                if (gestione == "1" && (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue))
                                {
                                    annoDec = (short)controlloFelpe.InizioBonus.Value.Year;
                                }
                                break;
                            case "7":
                                meseDec = 98;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "7":
                                meseDec = 99;
                                break;
                        }
                        break;
                }
            }

            //ENG - Aggiornamento Memo 68/2022 IOPGI
            //ENG - Spacchettamento SOPGI
            if (siglaCategoria == "VOPGI" || (Utility.IsDomandaIOPGI(siglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDA))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)decorrenzaOriginaria.Month;
                                break;
                            case "2":
                                meseDec = 72;
                                break;
                            case "3":
                                meseDec = 73;
                                break;
                            case "4":
                                meseDec = 74;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                                meseDec = 61;
                                break;
                            case "2":
                                meseDec = 62;
                                break;
                            case "3":
                                meseDec = 63;
                                break;
                            case "4":
                                meseDec = 64;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "7":
                                meseDec = 99;
                                break;
                        }
                        break;
                }
            }

            //Se la Categoria (RACATEG) è = a “VOBANC”, “IOBANC”, (“SOBANC” e il Campo DAU107 = a zero) 
            //testare i Campi VRGEST, VRQUOTA e RAU113DA per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM 
            //il valore indicato: 
            //VRGEST VRQUOTA RAU113DA → VRDECMM 
            //“H” “A” 0 → 75 
            //“H” spazio 0 → RAU104MM 
            //“H” “A” > 0 → 75 
            //“H” spazio > 0 → RAU113DM 
            //“H” “B” → 65 
            //“1” “A” 0 → RAU104MM 
            //“1” spazio 0 → RAU104MM 
            //“1” “A” > 0 → RAU113DM 
            //“1” spazio > 0 → RAU113DM 
            //“1” “B” → 61
            if (siglaCategoria == "VOBANC" || siglaCategoria == "IOBANC" ||
                (siglaCategoria == "SOBANC" && (datiDA == null || !datiDA.ProvenienzaPensione.HasValue || datiDA.ProvenienzaPensione.Value == 0)))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)decorrenzaOriginaria.Month;
                                break;
                            case "H":
                                meseDec = 75;
                                break;
                            case "S":
                                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                                    meseDec = (short)controlloFelpe.InizioBonus.Value.Month;
                                else if (decorrenzaDatiRetributivi != null)
                                    meseDec = (short)decorrenzaDatiRetributivi.Month;
                                else
                                    meseDec = 0;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                meseDec = 61;
                                break;
                            case "H":
                                meseDec = 65;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "1":
                            case "H":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)decorrenzaOriginaria.Month;
                                break;
                        }
                        break;

                }
            }

            //Se la Categoria (RACATEG) è = a “SO”, “SOP”, “SOMIN”, (“PMO” con il 3° carattere del Certificato = a 3 o 6) 
            //e DAU107 è = a “1” o “2” testare i Campi VRGEST, VRQUOTA e RAU113DA 
            //per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM il valore indicato: 
            //VRGEST VRQUOTA RAU113DA → VRDECMM 
            //“1” “A” 0 → DAU106MM 
            //“1H” “A” 0 → DAU106MM 
            //“Q” “A” 0 → DAU106MM 
            //“1” “A” > 0 → RAU113DM
            //“1H” “A” > 0 → RAU113DM
            //“Q” “A” > 0 → RAU113DM 
            //“1” “B” → 61 
            //“1H” “B” → 61 
            //“Q” “B” → 61 
            //“7” “A” → 99 
            //“7” “B” → 98 
            //“7” spazio → 99
            if ((siglaCategoria == "SO" || siglaCategoria == "SOP" || siglaCategoria == "SOMIN" ||
                (siglaCategoria == "PMO" && (certificato.Substring(2, 1) == "3" || certificato.Substring(2, 1) == "6"))) &&
                (datiDA != null && datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2)))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                                if (siglaCategoria == "SOMIN" && rms != null)
                                    meseDec = Convert.ToInt16(rms);
                                else
                                {
                                    if (decorrenzaOpzione.HasValue)
                                        meseDec = (short)decorrenzaOpzione.Value.Month;
                                    else
                                        meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;

                                    //specializzazione ante96 
                                    if (annoDec != 0 && annoDec < 1996 && decorrenzaDatiRetributivi != null)
                                        meseDec = (short)decorrenzaDatiRetributivi.Month;
                                }
                                break;
                            case "Q":
                            case "1H":
                            case "P":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
                                break;
                            case "7":
                                meseDec = 99;
                                break;
                            case "S":
                                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                                    meseDec = (short)controlloFelpe.InizioBonus.Value.Month;
                                else if (decorrenzaDatiRetributivi != null)
                                    meseDec = (short)decorrenzaDatiRetributivi.Month;
                                else
                                    meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                                if (siglaCategoria == "SOMIN" && rms != null)
                                    meseDec = Convert.ToInt16(rms);
                                else
                                    meseDec = 61;
                                break;
                            case "Q":
                            case "1H":
                            case "P":
                                meseDec = 61;
                                break;
                            case "7":
                                meseDec = 98;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "7":
                                meseDec = 99;
                                break;
                        }
                        break;
                }
            }

            //Se la Categoria (RACATEG) è = a “SOBANC” e DAU107 è = a “1” o “2” testare i Campi VRGEST, VRQUOTA e RAU113DA 
            //per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM il valore indicato: 
            //VRGEST VRQUOTA RAU113DA → VRDECMM 
            //“1” “A” 0 → DAU106MM 
            //“1” spazio 0 → DAU106MM 
            //“1” “A” > 0 → RAU113DM 
            //“1” spazio > 0 → RAU113DM 
            //“1” “B” → 61 
            //“H” “A” 0 → 75 
            //“H” spazio 0 → DAU106MM 
            //“H” “A” > 0 → 75 
            //“H” spazio > 0 → RAU113DM 
            //“H” “B” → 65
            if (siglaCategoria == "SOBANC" && (datiDA != null &&
                datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2)))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
                                break;
                            case "H":
                                meseDec = 75;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                meseDec = 61;
                                break;
                            case "H":
                                meseDec = 65;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "1":
                            case "H":
                            case "P":
                                if (decorrenzaOpzione.HasValue)
                                    meseDec = (short)decorrenzaOpzione.Value.Month;
                                else
                                    meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
                                break;
                        }
                        break;
                }
            }

            //Se la Categoria (RACATEG) è = a “VOCRED”, “VOCOOP”, “VOESO”, "VESO33", "VESO92", "VESO29" testare i Campi VRGEST, VRQUOTA 
            //per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM il valore indicato: VRGEST VRQUOTA → VRDECMM 
            //“1” “A” → RAU104MM 
            //“2” “A” → 72 
            //“3” “A” → 73 
            //“4” “A” → 74 
            //“1” “B” → 61 
            //“2” “B” → 62 
            //“3” “B” → 63 
            //“4” “B” → 64
            if (Utility.IsDomandaVOCRED_CRED27(siglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(siglaCategoria) || Utility.IsDomandaVOESO(siglaCategoria) || Utility.IsDomandaVESO33(siglaCategoria) ||
                Utility.IsDomandaVESO92(siglaCategoria) || Utility.IsDomandaVESO29(siglaCategoria) || Utility.IsDomandaESOTEL(siglaCategoria) || Utility.IsDomandaESOAMB(siglaCategoria) || Utility.IsDomandaESPA(siglaCategoria))
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                meseDec = (short)decorrenzaOriginaria.Month;
                                break;
                            case "2":
                                meseDec = 72;
                                break;
                            case "3":
                                meseDec = 73;
                                break;
                            case "4":
                                meseDec = 74;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                meseDec = 61;
                                break;
                            case "2":
                                meseDec = 62;
                                break;
                            case "3":
                                meseDec = 63;
                                break;
                            case "4":
                                meseDec = 64;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            //Se la Categoria (RACATEG) è = a “VR”, “IR”, “SR”, “VOART”, “IOART”, “SOART”, “VOCOM”, “IOCOM”, “SOCOM”:
            //se la variabile PAA è inferiore a 1990 mettere 1990 in PAA
            //testare i Campi VRGEST, VRQUOTA per ogni elemento della tabella stessa ed inserire nel Campo relativo VRDECMM 
            //il valore indicato: VRGEST VRQUOTA → VRDECMM 
            //“1” “A” → 71 
            //“1H” “A” → 71 
            //“1” spazio → 71 
            //“Q” “A” → 71 
            //“2” “A” → 72 
            //“2H” “A” → 72 
            //“2” spazio → 72 
            //“3” “A” → 73 
            //“3H” “A” → 73 
            //“3” spazio → 73 
            //“4” “A” → 74 
            //“4H” “A” → 74 
            //“4” spazio → 74 
            //“Q” “B” → 61 
            //“1” “B” → 61 
            //“1H” “B” → 61 
            //“2” “B” → 62 
            //“2H” “B” → 62 
            //“3” “B” → 63 
            //“3H” “B” → 63 
            //“4” “B” → 64 
            //“4H” “B” → 64
            //“S” “A” → INIZIOSCO(5:2)

            //Eng - Ric Vr, VOART, VOCOM che dal prelievo arrivano con codice gestione I, M o N
            //Se la Categoria (RACATEG) è = a “VR”, “VOART”, “VOCOM” e sono Ricostituzioni:
            //“I” → 66
            //“M” → 67
            //“N” → 68
            if (siglaCategoria == "VR" || siglaCategoria == "IR" || siglaCategoria == "SR" || siglaCategoria == "VOART" ||
                siglaCategoria == "IOART" || siglaCategoria == "SOART" || siglaCategoria == "VOCOM" || siglaCategoria == "IOCOM" || siglaCategoria == "SOCOM")
            {
                if (annoDec < 1990)
                    annoDec = 1990;
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {
                            case "1":
                            case "Q":
                            case "1H":
                            case "P":
                                meseDec = 71;
                                break;
                            case "2":
                            case "2H":
                                meseDec = 72;
                                break;
                            case "3":
                            case "3H":
                                meseDec = 73;
                                break;
                            case "4":
                            case "4H":
                                meseDec = 74;
                                break;
                            case "S":
                                if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                                    meseDec = (short)controlloFelpe.InizioBonus.Value.Month;
                                else if (decorrenzaDatiRetributivi != null)
                                    meseDec = (short)decorrenzaDatiRetributivi.Month;
                                else
                                    meseDec = 0;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "1":
                            case "Q":
                            case "1H":
                            case "P":
                                meseDec = 61;
                                break;
                            case "2":
                            case "2H":
                                meseDec = 62;
                                break;
                            case "3":
                            case "3H":
                                meseDec = 63;
                                break;
                            case "4":
                            case "4H":
                                meseDec = 64;
                                break;
                            case "I":
                                if ((siglaCategoria == "VR" || siglaCategoria == "VOART" || siglaCategoria == "VOCOM") && Utility.IsRicostituzione(datiPensione.Gruppo))
                                    meseDec = 66;
                                break;
                            case "M":
                                if ((siglaCategoria == "VR" || siglaCategoria == "VOART" || siglaCategoria == "VOCOM") && Utility.IsRicostituzione(datiPensione.Gruppo))
                                    meseDec = 67;
                                break;
                            case "N":
                                if ((siglaCategoria == "VR" || siglaCategoria == "VOART" || siglaCategoria == "VOCOM") && Utility.IsRicostituzione(datiPensione.Gruppo))
                                    meseDec = 68;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "1":
                            case "P":
                                meseDec = 71;
                                break;
                            case "2":
                                meseDec = 72;
                                break;
                            case "3":
                                meseDec = 73;
                                break;
                            case "4":
                                meseDec = 74;
                                break;
                        }
                        break;
                }
            }

            if (siglaCategoria == "VDAI" || siglaCategoria == "IDAI" || siglaCategoria == "SDAI")
            {
                byte? decorrenzaExInpdai = GestioneContrib.GetDecorrenzaExInpdai(gestione, quota, tipoQuota, elencoCtrlDecorrenzaRetrExINPDAI);
                if (decorrenzaExInpdai == 76)
                {
                    //Per il 76 (primo record) dobbiamo inviare il mese anziche il valore salvato
                    Utility.DifferenzaDateTime decorrenzaRetrExInpdai = GestioneContrib.GetDecorrenzaCalcoloRetrExInpdai(datiDA, decorrenzaOriginaria, controlloFelpe, datiPensione, decorrenzaDatiRetributivi);
                    if (decorrenzaRetrExInpdai != null)
                        decorrenzaExInpdai = (byte)decorrenzaRetrExInpdai.Month;
                }
                meseDec = decorrenzaExInpdai.GetValueOrDefault();
            }

            if (siglaCategoria == "VOSPETT" || siglaCategoria == "VOSPORT" || siglaCategoria == "IOSPETT" || siglaCategoria == "IOSPORT" ||
                siglaCategoria == "SOSPETT" || siglaCategoria == "SOSPORT")
            {
                switch (quota)
                {
                    case 'A':
                        //Se il Campo DAU107 è = a “1” o “2” mettere il Campo DAU106AA in PAA
                        if (datiDA != null && datiDA.ProvenienzaPensione.HasValue && (datiDA.ProvenienzaPensione.Value == 1 || datiDA.ProvenienzaPensione.Value == 2) && datiDA.DecorrenzaPensione.HasValue)
                            meseDec = (short)datiDA.DecorrenzaPensione.Value.Month;
                        //altrimenti mettere il Campo RAU104AA in PAA
                        else
                            meseDec = (short)decorrenzaOriginaria.Month;
                        //Se il Campo RAU113DA è maggiore di zero mettere il Campo RAU113DA in PAA
                        if (decorrenzaOpzione.HasValue)
                            meseDec = (short)decorrenzaOpzione.Value.Month;
                        //Se la variabile W-POSTICIPO è = a “S” mettere il Campo INIZIOSCO(1:4) in PAA
                        if (posticipo)
                        {
                            if (controlloFelpe != null && controlloFelpe.InizioBonus.HasValue)
                                meseDec = (short)controlloFelpe.InizioBonus.Value.Month;
                            else
                                meseDec = 0;
                        }

                        if (decorrenzaDatiRetributivi != null)
                            meseDec = (short)decorrenzaDatiRetributivi.Month;

                        break;

                    case 'B':
                        meseDec = 61;
                        break;
                }
            }

            if (siglaCategoria == "IOMIN")
            {
                switch (quota)
                {
                    case 'A':
                        switch (gestione)
                        {

                            case "7":
                                meseDec = 99;
                                break;
                        }
                        break;
                    case 'B':
                        switch (gestione)
                        {
                            case "7":
                                meseDec = 98;
                                break;
                        }
                        break;
                    default:
                        switch (gestione)
                        {
                            case "7":
                                meseDec = 99;
                                break;
                        }
                        break;
                }
            }

        }

        internal static void AlteraSupplementi(GestionePensione.DatiPensione datiPensione, INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi elementoB,
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi, out short meseDec,
            out INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi suppFittizio)
        {
            bool settingMese = false;
            suppFittizio = null;
            meseDec = elementoB.DecorrenzaSupplemento.HasValue ? (short)elementoB.DecorrenzaSupplemento.Value.Month : (short)0;
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaElementiA =
                (from s in listaSupplementi where s.QuotaSupplemento.HasValue && s.QuotaSupplemento.Value == 'A' select s).ToList<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            if (listaElementiA != null && listaElementiA.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi elementoA in listaElementiA)
                {
                    if (elementoA != null && elementoB != null && elementoA.CodGestioneSupplemento == elementoB.CodGestioneSupplemento &&
                    elementoA.DecorrenzaSupplemento.GetValueOrDefault() == elementoB.DecorrenzaSupplemento.GetValueOrDefault() &&
                    elementoB.CodiceLiquidazione.GetValueOrDefault() != 3 && elementoB.CodiceLiquidazione.GetValueOrDefault() != 4)
                    {
                        switch (elementoA.CodGestioneSupplemento)
                        {
                            case "1":
                                // Bonifica per la domnanda 2096736500090 di produzione, la quale dal prelievo recupera il valore 91
                                if (datiPensione.Id == 1188517 && elementoB.Id == 22379)
                                    meseDec = 91;
                                else
                                    meseDec = 61;
                                settingMese = true;
                                break;
                            case "2":
                                meseDec = 62;
                                settingMese = true;
                                break;
                            case "3":
                                meseDec = 63;
                                settingMese = true;
                                break;
                            case "4":
                                meseDec = 64;
                                settingMese = true;
                                break;
                            case "7":
                                meseDec = 98;
                                settingMese = true;
                                break;
                            case "H":
                                meseDec = 65;
                                settingMese = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (settingMese)
                        break;
                }
            }

            if (!settingMese)
            {
                switch (elementoB.CodGestioneSupplemento)
                {
                    case "I":
                        meseDec = 66;
                        break;
                    case "M":
                        meseDec = 67;
                        break;
                    case "N":
                        meseDec = 68;
                        break;
                    case "1":
                        meseDec = 61;
                        break;
                    case "2":
                        meseDec = 62;
                        break;
                    case "3":
                        meseDec = 63;
                        break;
                    case "4":
                        meseDec = 64;
                        break;
                    case "7":
                        meseDec = 98;
                        break;
                    case "H":
                        meseDec = 65;
                        break;
                    default:
                        break;
                }

                //creazione record fittizio
                switch (elementoB.CodGestioneSupplemento)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "7":
                    case "H":
                        suppFittizio = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        suppFittizio.CodGestioneSupplemento = elementoB.CodGestioneSupplemento;
                        suppFittizio.DecorrenzaSupplemento = elementoB.DecorrenzaSupplemento;
                        suppFittizio.NSettimaneSupplemento = 1;
                        suppFittizio.RMSSupplemento = 0.0040M;
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void AlteraSupplementiINPDAI(GestionePensione.DatiPensione datiPensione, INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi supp,
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementiApp, out short meseDec,
            out INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi suppFittizio)
        {
            bool settingMese = false;
            suppFittizio = null;
            meseDec = supp.DecorrenzaSupplemento.HasValue ? (short)supp.DecorrenzaSupplemento.Value.Month : (short)0;
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaElementiA = (from s in listaSupplementiApp
                                                                                               where s.QuotaSupplemento.HasValue && s.QuotaSupplemento.Value == 'A' && s.NSettimaneSupplemento.GetValueOrDefault() == 1 && s.RMSSupplemento.HasValue && s.RMSSupplemento.Value < 1
                                                                                               select s).ToList();
            if (listaElementiA != null && listaElementiA.Count > 0)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi elementoA in listaElementiA)
                {
                    if (elementoA != null && supp != null && elementoA.CodGestioneSupplemento == supp.CodGestioneSupplemento &&
                    elementoA.DecorrenzaSupplemento.GetValueOrDefault() == supp.DecorrenzaSupplemento.GetValueOrDefault() &&
                    supp.CodiceLiquidazione.GetValueOrDefault() != 3 && supp.CodiceLiquidazione.GetValueOrDefault() != 4)
                    {
                        switch (supp.CodGestioneSupplemento)
                        {
                            case "A":
                                switch (supp.QuotaSupplemento)
                                {
                                    case 'A':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "":
                                                meseDec = 17;
                                                settingMese = true;
                                                break;
                                            case "A":
                                                meseDec = 16;
                                                settingMese = true;
                                                break;
                                        }
                                        break;
                                    case 'B':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "B1":
                                                meseDec = 21;
                                                settingMese = true;
                                                break;
                                            case "B2":
                                                meseDec = 31;
                                                settingMese = true;
                                                break;
                                            case "B3":
                                                meseDec = 41;
                                                settingMese = true;
                                                break;
                                            case "B4":
                                                meseDec = 51;
                                                settingMese = true;
                                                break;
                                            case "B6":
                                                meseDec = 16;
                                                settingMese = true;
                                                break;

                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "1":
                                switch (supp.QuotaSupplemento)
                                {
                                    case 'A':
                                        meseDec = 71;
                                        settingMese = true;
                                        break;
                                    case 'B':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "B":
                                                meseDec = 61;
                                                settingMese = true;
                                                break;
                                            case "B9":
                                                meseDec = 91;
                                                settingMese = true;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case "2":
                                switch (supp.QuotaSupplemento)
                                {
                                    case 'A':
                                        meseDec = 72;
                                        settingMese = true;
                                        break;
                                    case 'B':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "B":
                                                meseDec = 62;
                                                settingMese = true;
                                                break;
                                            case "B9":
                                                meseDec = 92;
                                                settingMese = true;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case "3":
                                switch (supp.QuotaSupplemento)
                                {
                                    case 'A':
                                        meseDec = 73;
                                        settingMese = true;
                                        break;
                                    case 'B':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "B":
                                                meseDec = 63;
                                                settingMese = true;
                                                break;
                                            case "B9":
                                                meseDec = 93;
                                                settingMese = true;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case "4":
                                switch (supp.QuotaSupplemento)
                                {
                                    case 'A':
                                        meseDec = 74;
                                        settingMese = true;
                                        break;
                                    case 'B':
                                        switch (supp.CodTipoQuota)
                                        {
                                            case "B":
                                                meseDec = 64;
                                                settingMese = true;
                                                break;
                                            case "B9":
                                                meseDec = 94;
                                                settingMese = true;
                                                break;
                                        }
                                        break;
                                }
                                break;
                            case "I":
                                meseDec = 66;
                                settingMese = true;
                                break;
                            case "M":
                                meseDec = 67;
                                settingMese = true;
                                break;
                            case "N":
                                meseDec = 68;
                                settingMese = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (settingMese)
                        break;
                }
            }

            if (!settingMese)
            {
                switch (supp.CodGestioneSupplemento)
                {
                    case "A":
                        switch (supp.QuotaSupplemento)
                        {
                            case 'A':
                                switch (supp.CodTipoQuota)
                                {
                                    case "":
                                        meseDec = 17;
                                        break;
                                    case "A":
                                        meseDec = 16;
                                        break;
                                }
                                break;
                            case 'B':
                                switch (supp.CodTipoQuota)
                                {
                                    case "B1":
                                        meseDec = 21;
                                        break;
                                    case "B2":
                                        meseDec = 31;
                                        break;
                                    case "B3":
                                        meseDec = 41;
                                        break;
                                    case "B4":
                                        meseDec = 51;
                                        break;
                                    case "B6":
                                        meseDec = 16;
                                        break;

                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "1":
                        switch (supp.QuotaSupplemento)
                        {
                            case 'A':
                                meseDec = 71;
                                break;
                            case 'B':
                                switch (supp.CodTipoQuota)
                                {
                                    case "B":
                                        meseDec = 61;
                                        break;
                                    case "B9":
                                        meseDec = 91;
                                        break;
                                }
                                break;
                        }
                        break;
                    case "2":
                        switch (supp.QuotaSupplemento)
                        {
                            case 'A':
                                meseDec = 72;
                                break;
                            case 'B':
                                switch (supp.CodTipoQuota)
                                {
                                    case "B":
                                        meseDec = 62;
                                        break;
                                    case "B9":
                                        meseDec = 92;
                                        break;
                                }
                                break;
                        }
                        break;
                    case "3":
                        switch (supp.QuotaSupplemento)
                        {
                            case 'A':
                                meseDec = 73;
                                break;
                            case 'B':
                                switch (supp.CodTipoQuota)
                                {
                                    case "B":
                                        meseDec = 63;
                                        break;
                                    case "B9":
                                        meseDec = 93;
                                        break;
                                }
                                break;
                        }
                        break;
                    case "4":
                        switch (supp.QuotaSupplemento)
                        {
                            case 'A':
                                meseDec = 74;
                                break;
                            case 'B':
                                switch (supp.CodTipoQuota)
                                {
                                    case "B":
                                        meseDec = 64;
                                        break;
                                    case "B9":
                                        meseDec = 94;
                                        break;
                                }
                                break;
                        }
                        break;
                    case "I":
                        meseDec = 66;
                        break;
                    case "M":
                        meseDec = 67;
                        break;
                    case "N":
                        meseDec = 68;
                        break;
                    default:
                        break;
                }

                //creazione record fittizio
                switch (supp.CodGestioneSupplemento)
                {
                    case "A":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "I":
                    case "M":
                    case "N":
                        suppFittizio = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
                        suppFittizio.CodGestioneSupplemento = supp.CodGestioneSupplemento;
                        suppFittizio.QuotaSupplemento = 'A';
                        suppFittizio.CodTipoQuota = "";
                        suppFittizio.DecorrenzaSupplemento = supp.DecorrenzaSupplemento;
                        suppFittizio.NSettimaneSupplemento = 1;
                        suppFittizio.RMSSupplemento = 0.0040M;
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool IsFormatoScadenzaAssegnoGGMMAAAA(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            short codiceAziendaTraduzioneSuGP, byte? progressivoBancaFideiussoria, bool isRiapertura)
        {
            if (((Utility.IsDomandaVESO92_L92(contenitore.DatiPensione) || Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA_L26(contenitore.DatiPensione) ||
                Utility.IsDomandaVESO92RicWithScadenzaAssegnoGGMMAAAA(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.IsScadenzaAssegnoConGiorno : null)) &&
                contenitoreDecodifica.ElencoDecAziendeScadenzaAssegnoGGmmAAAA.Exists(x => x.TraduzioneSuGP.Trim() == codiceAziendaTraduzioneSuGP.ToString() &&
                    (!x.ProgressivoRichiesto.HasValue || x.ProgressivoRichiesto == progressivoBancaFideiussoria)))
                //oppure non è una RIC di  VESO29/VOESO con CodiceTipoRichiesta != "74"
                    || (!Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && ((Utility.IsDomandaVESO29(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.CodiceTipoRichiesta != "74") ||
                        (Utility.IsDomandaVOESO(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.CodiceTipoRichiesta != "74" && contenitore.DatiPensione.CodiceTipoRichiesta != "71" && contenitore.DatiPensione.CodiceTipoRichiesta != "70")))
                //oppure è una RIC con flag scadenza asssegno con giorno 
                    || Utility.IsDomandaIsoPensioneRicWithScadenzaAssegnoGGMMAAAA(contenitore.DatiPensione.SiglaCategoria, contenitore.DatiPensione.Gruppo, contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.IsScadenzaAssegnoConGiorno : null))
                return true;

            return false;
        }

        private static bool CheckVariazioneDatiNumericiDatiCalcolo(List<GestioneCalcolo.DatiCalcoloRetributivo> ListaDatiRetributivi, List<GestioneCalcolo.DatiCalcoloRetributivo> ListaDatiRetributiviStorico, List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiContributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiContributiviStorico, List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaDatiQuotaFondoIntegrativo, List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaDatiQuotaFondoIntegrativoStorico)
        {
            bool variazioneRetributivi = false;
            bool variazioneContributivi = false;
            bool variazioneQuotaFondoIntegrativo = false;
            if (ListaDatiRetributivi != null && ListaDatiRetributivi.Count > 0 && ListaDatiRetributiviStorico != null && ListaDatiRetributiviStorico.Count > 0)
            {
                foreach (var retr in ListaDatiRetributivi)
                {
                    bool findInStorico = false;
                    foreach (var retrStorico in ListaDatiRetributiviStorico)
                    {
                        var itemStorico = new { retrStorico.CodiceGestione, retrStorico.QuotePrimeLiquidate, retrStorico.CodiceTipoQuota, retrStorico.NSettimaneQuotaA, retrStorico.NSettimaneQuotaB, retrStorico.RMSQuotaA, retrStorico.RMSQuotaB, retrStorico.NSettimane707 };
                        var item = new { retr.CodiceGestione, retr.QuotePrimeLiquidate, retrStorico.CodiceTipoQuota, retr.NSettimaneQuotaA, retr.NSettimaneQuotaB, retr.RMSQuotaA, retr.RMSQuotaB, retr.NSettimane707 };
                        if (item.Equals(itemStorico))
                        {
                            findInStorico = true;
                            break;
                        }
                    }
                    if (!findInStorico)
                    {
                        variazioneRetributivi = true;
                        break;
                    }
                }
            }

            if (ListaDatiContributivi != null && ListaDatiContributivi.Count > 0 && ListaDatiContributiviStorico != null && ListaDatiContributiviStorico.Count > 0)
            {
                foreach (var contr in ListaDatiContributivi)
                {
                    bool findInStorico = false;
                    foreach (var contrStor in ListaDatiContributiviStorico)
                    {
                        var itemStorico = new { contrStor.CodiceGestione, contrStor.ImportoContributivoTotale, contrStor.ImportoContribTotaleQuotaDL214, contrStor.Montante, contrStor.MontanteQuotaDL214, contrStor.NSettimane, contrStor.NSettimaneQuotaDL214 };
                        var item = new { contr.CodiceGestione, contr.ImportoContributivoTotale, contr.ImportoContribTotaleQuotaDL214, contr.Montante, contr.MontanteQuotaDL214, contr.NSettimane, contr.NSettimaneQuotaDL214 };
                        if (item.Equals(itemStorico))
                        {
                            findInStorico = true;
                            break;
                        }
                    }
                    if (!findInStorico)
                    {
                        variazioneContributivi = true;
                        break;
                    }
                }
            }

            if (ListaDatiQuotaFondoIntegrativo != null && ListaDatiQuotaFondoIntegrativo.Count() > 0 && ListaDatiQuotaFondoIntegrativoStorico != null && ListaDatiQuotaFondoIntegrativoStorico.Count > 0)
            {
                foreach (var quota in ListaDatiQuotaFondoIntegrativo)
                {
                    bool findInStorico = false;
                    foreach (var quotaStor in ListaDatiQuotaFondoIntegrativoStorico)
                    {
                        var itemStorico = new { quotaStor.CodiceGestione, quotaStor.ImportoContributivoTotale, quotaStor.ImportoContribTotaleQuotaD, quotaStor.Montante, quotaStor.MontanteQuotaD, quotaStor.NSettimane, quotaStor.NSettimaneQuotaD };
                        var item = new { quota.CodiceGestione, quota.ImportoContributivoTotale, quota.ImportoContribTotaleQuotaD, quota.Montante, quota.MontanteQuotaD, quota.NSettimane, quota.NSettimaneQuotaD };
                        if (item.Equals(itemStorico))
                        {
                            findInStorico = true;
                            break;
                        }
                    }
                    if (!findInStorico)
                    {
                        variazioneQuotaFondoIntegrativo = true;
                        break;
                    }
                }
            }

            bool ret = variazioneRetributivi || variazioneContributivi || variazioneQuotaFondoIntegrativo;
            return ret;
        }

        private static bool IsFlussoAdeguata(List<GestioneDecodifica.CtrlCatAdeguata> elencoCtrlCatAdeguata, string codCategoria, string gruppo, string prodotto, string tipo, bool isTrasfRic)
        {
            if (elencoCtrlCatAdeguata != null)
            {
                var dataAttuale = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var res = elencoCtrlCatAdeguata.Where(x => (x.CodCategoria.Trim() == codCategoria || x.CodCategoria == null)
                && x.CodGruppo == gruppo &&
                (x.CodProdotto == prodotto || x.CodProdotto == null) &&
                (x.CodTipo == tipo || x.CodTipo == null) &&
                (x.IsTrasfRic == isTrasfRic || x.IsTrasfRic == null)
                && (x.DataInizio == null || Utility.DataSuccessivaA(dataAttuale, x.DataInizio.Value))
                && (x.DataFine == null || Utility.DataSuccessivaA(x.DataFine.Value, dataAttuale))
                ).FirstOrDefault();
                if (res != null) return true;
            }
            return false;
        }
        #endregion private methods
    }
}
