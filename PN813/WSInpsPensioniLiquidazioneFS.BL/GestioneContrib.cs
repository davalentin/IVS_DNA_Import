using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.LiquidazioneFs.ServiceReferences.AggPec;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneContrib
    {
        #region public members


        #region Dati Calcolo
        public static void GetTipoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, out TipoCalcolo tipoCalcolo)
        {
            tipoCalcolo = TipoCalcolo.NonValido;
            if (datiPensione == null || !datiPensione.TipoCalcolo.HasValue)
                return;

            switch (Utility.GetTipoCalcolo(datiPensione))
            {
                case Utility.TipoCalcolo.Contributivo:
                    tipoCalcolo = TipoCalcolo.Contributivo;
                    break;
                case Utility.TipoCalcolo.Retributivo:
                    tipoCalcolo = TipoCalcolo.Retributivo;
                    break;
                case Utility.TipoCalcolo.Misto:
                    tipoCalcolo = TipoCalcolo.Misto;
                    break;
                case Utility.TipoCalcolo.RetributivoMonti:
                    tipoCalcolo = TipoCalcolo.RetributivoMonti;
                    break;
            }

        }

        public static void GetDatiCalcoloByDomandaFelpe(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            GestioneFondo.DatiFondo datiFondo, bool isRiaperturaDomanda, out DatiCalcolo datiCalcolo, out object datiFelpe, out string messaggioVideo)
        {
            datiFelpe = null;
            datiCalcolo = null;
            messaggioVideo = "";
            CrossDataRecipient crossDataRecipient = new CrossDataRecipient();  // contenitore di proprietà prelevabili e/o non prelevabili da AggPeco non appartenenti a dati Calcolo e /o Retributivi 
            GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco = new GestioneAggiornamentoPECO.DatiTotaliAggPeco();

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiContributivi);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
            GestioneCalcolo.GetCalcoloRetributivoRecordFondoByIdPensione(datiPensione.Id, out listaDatiRetributivi);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            List<Utility.TipoFondo> listaTipoFondo_PECO_Fondi_AMG = Utility.GetListaTipoFondo_PECO_Fondi_AMG();

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (listaDatiContributivi == null || listaDatiContributivi.Count == 0) && listaDatiRetributivi == null)
            {
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI))
                {
                    csAggiornamentoPECO_Fondi_AMG_INPDAP dati = null;
                    try
                    {
                        GestioneAggiornamentoPECO.GetDatiTotaliAMG_INPDAP(datiPensione, out dati, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            throw new INPS.DNA.DnaApplicationException(messaggioVideo);
                        else
                            GestioneAggiornamentoPECO.RecuperaDatiTotaliAMG_INPDAP(dati, datiPensione, datiDanteCausa, tipoFondo, datiFondo, datiMaggiorazioniBenefici, isRiaperturaDomanda, out datiAggPeco,
                                ref crossDataRecipient, out messaggioVideo);

                        RecuperaDatiFlatForDatiCalcolo(datiFondo, ref crossDataRecipient);
                    }
                    catch (Exception)
                    {
                        datiCalcolo = new DatiCalcolo();
                        datiCalcolo.IsCalcoloValido = false;
                        datiCalcolo.TipoCalcolo = TipoCalcolo.NonValido;
                    }
                    datiFelpe = dati;
                }
                else if ((tipoFondo.HasValue && listaTipoFondo_PECO_Fondi_AMG.Contains(tipoFondo.Value)) || (Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG))
                {
                    csAggiornamentoPECO_Fondi_AMG dati = null;
                    try
                    {
                        GestioneAggiornamentoPECO.GetDatiTotaliAMG(datiPensione, out dati, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            throw new INPS.DNA.DnaApplicationException(messaggioVideo);
                        else
                            GestioneAggiornamentoPECO.RecuperaDatiTotaliAMG(dati, datiPensione, datiDanteCausa, tipoFondo, datiFondo, datiMaggiorazioniBenefici, isRiaperturaDomanda, out datiAggPeco,
                                ref crossDataRecipient, out messaggioVideo);

                        RecuperaDatiFlatForDatiCalcolo(datiFondo, ref crossDataRecipient);
                    }
                    catch (Exception)
                    {
                        datiCalcolo = new DatiCalcolo();
                        datiCalcolo.IsCalcoloValido = false;
                        datiCalcolo.TipoCalcolo = TipoCalcolo.NonValido;
                    }
                    datiFelpe = dati;
                }
                else
                {
                    csAggiornamentoPECO_Fondi_Speciali dati = null;
                    try
                    {
                        GestioneAggiornamentoPECO.GetDatiTotali(datiPensione, out dati, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            throw new INPS.DNA.DnaApplicationException(messaggioVideo);
                        else
                            GestioneAggiornamentoPECO.RecuperaDatiTotaliAggPeco(dati, datiPensione, datiDanteCausa, tipoFondo, datiFondo, datiMaggiorazioniBenefici, isRiaperturaDomanda, out datiAggPeco,
                                ref crossDataRecipient, out messaggioVideo);

                        RecuperaDatiFlatForDatiCalcolo(datiFondo, ref crossDataRecipient);
                    }
                    catch (Exception)
                    {
                        datiCalcolo = new DatiCalcolo();
                        datiCalcolo.IsCalcoloValido = false;
                        datiCalcolo.TipoCalcolo = TipoCalcolo.NonValido;
                    }
                    datiFelpe = dati;
                }
            }
            else
            {
                datiAggPeco.DatiParziali = new GestioneAggiornamentoPECO.DatiParzialiAggPeco();
                datiAggPeco.DatiParziali.InizioAssicurazione = datiPensione.InizioAssicurazione;
                datiAggPeco.DatiParziali.FineAssicurazione = datiPensione.FineAssicurazione;
                datiAggPeco.DatiParziali.DecorrenzaPensione = datiPensione.DecorrenzaOriginaria;
                if (datiFondo != null && datiFondo.SettimaneUtiliDiritto.HasValue)
                    datiAggPeco.DatiParziali.SettimaneUtiliDiritto = datiFondo.SettimaneUtiliDiritto.Value;
                if (datiFondo != null && datiFondo.SettimaneUtiliDirittoOI.HasValue)
                    datiAggPeco.DatiParziali.SettimaneUtiliDirittoOI = datiFondo.SettimaneUtiliDirittoOI.Value;

                if (datiPensione.TipoCalcolo.HasValue)
                    datiAggPeco.DatiParziali.TipoCalcolo = (GestioneAggiornamentoPECO.TipoCalcolo)Liquidazione.BLCommon.Utility.GetTipoCalcolo(datiPensione);

                GestioneAggiornamentoPECO.DatiContributivi Contribuzione = null;
                GestioneAggiornamentoPECO.DatiRetributivi Retribuzione = null;
                RecuperaDatiCalcoloFromDB(listaDatiContributivi != null ? listaDatiContributivi.FirstOrDefault() : null, listaDatiRetributivi != null ? listaDatiRetributivi.FirstOrDefault() : null, out Retribuzione, out Contribuzione);

                datiAggPeco.Contribuzione = Contribuzione;
                datiAggPeco.Retribuzione = Retribuzione;

                RecuperaDatiCalcFromFondiFelpeByIdPensione(datiPensione.Id, tipoFondo, ref datiAggPeco);
                RecuperaDatiCustomFromFondi(datiPensione.Id, tipoFondo, datiPensione.Gruppo, datiPensione.IsPLUnicarpe, ref crossDataRecipient, datiPensione);
                RecuperaDatiFlatForDatiCalcolo(datiFondo, ref crossDataRecipient);
                RecuperaDati707ForDatiCalcolo(datiFondo, ref datiAggPeco);
                RecuperaDatiForAnteArmonizzazione(datiPensione, datiDanteCausa, datiFondo, ref crossDataRecipient);

                if (datiMaggiorazioniBenefici != null)
                    crossDataRecipient.RMSSenzaLegge33670QA = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA;

                List<GestioneContrib.DatiServizioUtile> listaDatiServizioUtile = null;
                if (crossDataRecipient != null)
                    listaDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;

                List<Entity.DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707 = null;
                if (crossDataRecipient != null)
                    listaDatiServizioUtile707 = crossDataRecipient.LDatiServizioUtile707;
                GestioneAggiornamentoPECO.ImpostaDatiControllo(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, listaDatiServizioUtile707, datiFondo, isRiaperturaDomanda, out messaggioVideo);
            }
            datiCalcolo = new DatiCalcolo(datiAggPeco, tipoFondo, crossDataRecipient, datiPensione.TipoCalcolo, datiPensione.Gruppo, datiPensione.IsPLUnicarpe, datiPensione);
        }

        private static void RecuperaDatiForAnteArmonizzazione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneFondo.DatiFondo datiFondo,
            ref CrossDataRecipient crossDataRecipient)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondo: datiFondo))
            {
                if (crossDataRecipient == null)
                    crossDataRecipient = new CrossDataRecipient();

                List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = null;
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                        GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out lServizioUtileCommon);
                        if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        {
                            crossDataRecipient.IdPensione = datiPensione.Id;
                            crossDataRecipient.LServizioUtileAnteArmonizzazione = lServizioUtileCommon.Select(x => { var y = new DatiServizioUtile(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
                        }
                        crossDataRecipient.RetrPondAnnuaAGOLimite = datiFondo != null ? datiFondo.RetrPondAnnuaAGOLimite : null;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out lServizioUtileCommon);
                        if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        {
                            crossDataRecipient.IdPensione = datiPensione.Id;
                            crossDataRecipient.LServizioUtileAnteArmonizzazione = lServizioUtileCommon.Select(x => { var y = new DatiServizioUtile(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
                        }
                        break;
                    case Utility.TipoFondo.TT:
                        GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out lServizioUtileCommon);
                        if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        {
                            crossDataRecipient.IdPensione = datiPensione.Id;
                            crossDataRecipient.LServizioUtileAnteArmonizzazione = lServizioUtileCommon.Select(x => { var y = new DatiServizioUtile(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
                        }
                        GestioneFondo.DatiFondoTT datiFondoTT = null;
                        GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                        if (datiFondoTT != null)
                        {
                            crossDataRecipient.PensioneMensileAl53 = datiFondoTT.PensioneMensileAl53;
                            crossDataRecipient.ElementiAccessori = datiFondoTT.ElementiAccessori;
                            crossDataRecipient.RetribuzioneSupplementi = datiFondoTT.RetribuzioneSupplementi;
                        }
                        crossDataRecipient.RetrPondAnnuaAGOLimite = datiFondo != null ? datiFondo.RetrPondAnnuaAGOLimite : null;

                        break;
                }
            }
        }

        private static void RecuperaDati707ForDatiCalcolo(GestioneFondo.DatiFondo datiFondo, ref GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco)
        {
            if (datiFondo != null && !datiFondo.IsDatiComma707Null())
            {
                if (datiAggPeco.Retribuzione == null)
                    datiAggPeco.Retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                datiAggPeco.Retribuzione.QuotaA707 = datiFondo.QuotaA707;
                datiAggPeco.Retribuzione.QuotaA2707 = datiFondo.QuotaA2707;
                datiAggPeco.Retribuzione.QuotaB707 = datiFondo.QuotaB707;
                datiAggPeco.Retribuzione.QuotaC707 = datiFondo.QuotaC707;
                datiAggPeco.Retribuzione.QuotaC2707 = datiFondo.QuotaC2707;
                datiAggPeco.Retribuzione.QuotaD707 = datiFondo.QuotaD707;
                datiAggPeco.Retribuzione.QuotaA707AA = datiFondo.QuotaA707AA;
                datiAggPeco.Retribuzione.QuotaA707MM = datiFondo.QuotaA707MM;
                datiAggPeco.Retribuzione.QuotaA707GG = datiFondo.QuotaA707GG;
                datiAggPeco.Retribuzione.QuotaB707AA = datiFondo.QuotaB707AA;
                datiAggPeco.Retribuzione.QuotaB707MM = datiFondo.QuotaB707MM;
                datiAggPeco.Retribuzione.QuotaB707GG = datiFondo.QuotaB707GG;
                datiAggPeco.Retribuzione.QuotaC707AA = datiFondo.QuotaC707AA;
                datiAggPeco.Retribuzione.QuotaC707MM = datiFondo.QuotaC707MM;
                datiAggPeco.Retribuzione.QuotaC707GG = datiFondo.QuotaC707GG;
                datiAggPeco.Retribuzione.RetribuzionePonderataAGO707 = datiFondo.RetribuzionePonderataAGO707;
                datiAggPeco.Retribuzione.QuotaAES707 = datiFondo.QuotaAES707;
                datiAggPeco.Retribuzione.QuotaBES707 = datiFondo.QuotaBES707;
            }
        }

        private static void RecuperaDati707ForDatiCalcoloStorico(GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, ref GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco)
        {
            if (datiStoricoGP != null && !datiStoricoGP.IsDatiComma707Null())
            {
                if (datiAggPeco.Retribuzione == null)
                    datiAggPeco.Retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                datiAggPeco.Retribuzione.QuotaA707 = datiStoricoGP.QuotaA707;
                datiAggPeco.Retribuzione.QuotaA2707 = datiStoricoGP.QuotaA2707;
                datiAggPeco.Retribuzione.QuotaB707 = datiStoricoGP.QuotaB707;
                datiAggPeco.Retribuzione.QuotaC707 = datiStoricoGP.QuotaC707;
                datiAggPeco.Retribuzione.QuotaC2707 = datiStoricoGP.QuotaC2707;
                datiAggPeco.Retribuzione.QuotaD707 = datiStoricoGP.QuotaD707;
                datiAggPeco.Retribuzione.RetribuzionePonderataAGO707 = datiStoricoGP.RetribuzionePonderataAGO707;
            }
        }

        public static void StoreDatiCalcoloByDomandaFelpe(GestionePensione.DatiPensione datiPensione, DatiCalcolo datiCalcolo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, char? codiceSpecificoTraduzioneSuGP, bool isRiaperturaDomanda,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            ref GestioneFondo.DatiFondo datiFondo, ref object datiFondoXX, DatiArt11e14 entityDatiArt11e14, DateTime dataSistema, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (datiCalcolo == null)
                return;

            GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco = new GestioneAggiornamentoPECO.DatiTotaliAggPeco();
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.FineAssicurazione.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(1992, 12, 31))) && datiPensione.TipoCalcolo.HasValue)
            {
                List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
                GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
                GestioneDecodifica.TipoCalcolo tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == datiPensione.TipoCalcolo.Value.ToString());

                if (tipoCalcolo != null && datiCalcolo.TipoCalcolo != TipoCalcolo.NonValido)
                {
                    if ((tipoCalcolo.TraduzioneSuGP == 1 && datiCalcolo.TipoCalcolo != TipoCalcolo.Retributivo && datiCalcolo.TipoCalcolo != TipoCalcolo.RetributivoMonti) ||
                        (tipoCalcolo.TraduzioneSuGP == 4 && datiCalcolo.TipoCalcolo != TipoCalcolo.Contributivo) ||
                        (tipoCalcolo.TraduzioneSuGP == 3 && datiCalcolo.TipoCalcolo != TipoCalcolo.Misto))
                    {
                        messaggioVideo = "Il tipo calcolo '" + tipoCalcolo.Descrizione + "' salvato sul quadro Liquidazione Pensione è differente dai dati calcolo che si sta tentando di salvare";
                        datiCalcolo.IsCalcoloValido = false;
                        return;
                    }
                }
            }

            GestioneAggiornamentoPECO.DatiContributivi contribuzione = null;
            GestioneAggiornamentoPECO.DatiRetributivi retribuzione = null;

            datiAggPeco.DatiParziali = new GestioneAggiornamentoPECO.DatiParzialiAggPeco();
            datiAggPeco.DatiParziali.InizioAssicurazione = datiPensione.InizioAssicurazione;
            datiAggPeco.DatiParziali.FineAssicurazione = datiPensione.FineAssicurazione;

            RecuperaDatiCalcoloFromWebForControls(datiCalcolo, tipoFondo, out retribuzione, out contribuzione, out messaggioVideo);
            datiAggPeco.Contribuzione = contribuzione;
            datiAggPeco.Retribuzione = retribuzione;

            List<DatiServizioUtile> listaDatiServizioUtile = null;
            if (datiCalcolo != null && datiCalcolo.fondoET != null)
                listaDatiServizioUtile = datiCalcolo.fondoET.lDatiServizioUtile;
            if (datiAggPeco.DatiParziali.FineAssicurazione.HasValue && datiAggPeco.DatiParziali.InizioAssicurazione.HasValue)
                GestioneAggiornamentoPECO.ImpostaDatiControlloToSave(tipoFondo, datiAggPeco, datiPensione, datiDanteCausa, datiMaggiorazioniBenefici, listaDatiServizioUtile, null, datiFondo, isRiaperturaDomanda, out messaggioVideo);
            else
            {
                datiAggPeco.DatiControllo = new INPS.Pensioni.LiquidazioneFs.GestioneAggiornamentoPECO.DatiControllo();
                datiAggPeco.DatiControllo.TipoCalcolo = INPS.Pensioni.LiquidazioneFs.GestioneAggiornamentoPECO.TipoCalcolo.NonValido;
                datiAggPeco.DatiControllo.IsCalcoloValido = false;
                messaggioVideo = "Controllo validità calcolo non riuscito. Controllare inserimento delle date di inizio e fine assicurazione";
            }

            if (!String.IsNullOrEmpty(messaggioVideo))
                return;

            if (!ControlsSettimaneUtiliDirittoFondi(datiCalcolo, datiPensione, tipoFondo, ref datiMaggiorazioniBenefici, out messaggioVideo))
                return;

            if (datiAggPeco.DatiControllo.IsCalcoloValido &&
                ((datiCalcolo.TipoCalcolo == TipoCalcolo.Contributivo && datiAggPeco.DatiControllo.TipoCalcolo == GestioneAggiornamentoPECO.TipoCalcolo.Contributivo) ||
                 (datiCalcolo.TipoCalcolo == TipoCalcolo.Retributivo && datiAggPeco.DatiControllo.TipoCalcolo == GestioneAggiornamentoPECO.TipoCalcolo.Retributivo) ||
                 (datiCalcolo.TipoCalcolo == TipoCalcolo.Misto && datiAggPeco.DatiControllo.TipoCalcolo == GestioneAggiornamentoPECO.TipoCalcolo.Misto) ||
                 (datiCalcolo.TipoCalcolo == TipoCalcolo.RetributivoMonti && datiAggPeco.DatiControllo.TipoCalcolo == GestioneAggiornamentoPECO.TipoCalcolo.RetributivoMonti)))
            {
                ControlsDatiCalcoloCross(datiCalcolo, tipoFondo, datiPensione, datiFondo, datiMaggiorazioniBenefici, datiFondoXX, datiDanteCausa, datiAnagraficiTitolare, entityDatiArt11e14,
                    codiceSpecificoTraduzioneSuGP, dataSistema, isRiaperturaDomanda, isSingleTab, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                {
                    datiCalcolo.IsCalcoloValido = false;
                    return;
                }

                GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneDoppioCalcolo707", out controlloDinamico);
                if ((tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.VL || tipoFondo.Value == Utility.TipoFondo.TT || tipoFondo.Value == Utility.TipoFondo.EL || tipoFondo.Value == Utility.TipoFondo.ES))
                    || (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI"))
                {
                    if (!GestioneControlli.ControlsDatiComma707(datiPensione, tipoFondo, datiCalcolo, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                        return;
                }

                GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;

                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                {
                    GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiContributivi);
                    GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiRetributivi);
                    Utility.ValorizzaOggetti(datiContributivi, datiCalcolo);
                    Utility.ValorizzaOggetti(datiRetributivi, datiCalcolo);
                }

                if (tipoFondo != Utility.TipoFondo.PI && tipoFondo != Utility.TipoFondo.PL)
                {
                    if (datiContributivi == null)
                        datiContributivi = new GestioneCalcolo.DatiCalcoloContributivo();
                    Utility.ValorizzaOggetti(datiCalcolo, datiContributivi);
                    datiContributivi.IdPensione = datiPensione.Id;

                    if (datiRetributivi == null)
                        datiRetributivi = new GestioneCalcolo.DatiCalcoloRetributivo();
                    Utility.ValorizzaOggetti(datiCalcolo, datiRetributivi);
                    datiRetributivi.IdPensione = datiPensione.Id;
                }





                #region EtichetteUnicarpe

                if (datiFondo == null)
                    datiFondo = new GestioneFondo.DatiFondo();
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                {
                    datiCalcolo.RiduzioneRetributiva = datiFondo.RiduzioneRetributiva;
                    datiCalcolo.RiduzioneRetributivaPercentuale = datiFondo.RiduzioneRetributivaPercentuale;
                    datiCalcolo.QuotaA707 = datiFondo.QuotaA707;
                    datiCalcolo.QuotaB707 = datiFondo.QuotaB707;
                    datiCalcolo.QuotaC707 = datiFondo.QuotaC707;
                    datiCalcolo.QuotaD707 = datiFondo.QuotaD707;
                    datiCalcolo.QuotaA707AA = datiFondo.QuotaA707AA;
                    datiCalcolo.QuotaA707MM = datiFondo.QuotaA707MM;
                    datiCalcolo.QuotaA707GG = datiFondo.QuotaA707GG;
                    datiCalcolo.QuotaB707AA = datiFondo.QuotaB707AA;
                    datiCalcolo.QuotaB707MM = datiFondo.QuotaB707MM;
                    datiCalcolo.QuotaB707GG = datiFondo.QuotaB707GG;
                    datiCalcolo.QuotaC707AA = datiFondo.QuotaC707AA;
                    datiCalcolo.QuotaC707MM = datiFondo.QuotaC707MM;
                    datiCalcolo.QuotaC707GG = datiFondo.QuotaC707GG;
                    datiCalcolo.RetribuzionePonderataAGO707 = datiFondo.RetribuzionePonderataAGO707;
                }

                Utility.ValorizzaOggetti(datiCalcolo, datiFondo);

                #endregion EtichetteUnicarpe

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    switch (datiCalcolo.TipoCalcolo)
                    {
                        case TipoCalcolo.Contributivo:
                            if (datiContributivi != null)
                                GestioneCalcolo.SalvaCalcoloContributivo(datiContributivi);
                            if (tipoFondo == Utility.TipoFondo.ES)
                            {
                                GestioneCalcolo.SalvaCalcoloRetributivo(datiRetributivi);
                            }
                            break;
                        case TipoCalcolo.Retributivo:
                            if (datiRetributivi != null)
                                GestioneCalcolo.SalvaCalcoloRetributivo(datiRetributivi);
                            break;
                        case TipoCalcolo.Misto:
                        case TipoCalcolo.RetributivoMonti:
                            if (datiContributivi != null)
                                GestioneCalcolo.SalvaCalcoloContributivo(datiContributivi);
                            if (datiRetributivi != null)
                                GestioneCalcolo.SalvaCalcoloRetributivo(datiRetributivi);
                            break;
                        default:
                            return;
                    }

                    SalvaDatiCalcoloWithFondi(datiCalcolo, datiFondoXX, ref datiFondo, datiPensione, ref datiMaggiorazioniBenefici, tipoFondo);


                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.GAS:
                            if ((datiCalcolo != null && datiCalcolo.fondoGAS != null && !datiCalcolo.fondoGAS.IsFondoNullForDatiAgo()) || !datiContributivi.IsDatiCalcoloContributivoNull() || !datiRetributivi.IsDatiCalcoloRetributivoNull())
                                quadroDatiContributivi.TabDatiAgo = 2;
                            else
                            {
                                if (Utility.IsDomandaReversibilita(datiPensione))
                                {
                                    if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                                    {
                                        if (Utility.DataStrettamenteSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                            quadroDatiContributivi.TabDatiAgo = 0;
                                        else
                                            quadroDatiContributivi.TabDatiAgo = 1;
                                    }
                                }
                                else
                                {
                                    if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1998, 02, 01)))
                                        quadroDatiContributivi.TabDatiAgo = 0;
                                    else
                                        quadroDatiContributivi.TabDatiAgo = 1;
                                }
                            }
                            break;
                        case Utility.TipoFondo.ES:
                            quadroDatiContributivi.TabDatiAgo = 2;
                            break;
                        case Utility.TipoFondo.PM:
                        case Utility.TipoFondo.EL:
                        case Utility.TipoFondo.ET:
                        case Utility.TipoFondo.TT:
                        case Utility.TipoFondo.VL:
                        case Utility.TipoFondo.PT:
                        case Utility.TipoFondo.FS:
                        case Utility.TipoFondo.DZ:
                        case Utility.TipoFondo.PI:
                        case Utility.TipoFondo.PL:
                            quadroDatiContributivi.TabDatiCalcolo = 2;
                            break;
                    }

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                    transactionScope.Complete();
                }
            }
            else
                datiCalcolo.IsCalcoloValido = datiAggPeco.DatiControllo.IsCalcoloValido;
        }

        public static void StoreDatiCalcolo707ByDomandaFelpe(GestionePensione.DatiPensione datiPensione, Entity.DatiCalcolo707 datiCalcolo707, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref GestioneFondo.DatiFondo datiFondo, ref object datiFondoXX, out string messaggioVideo)
        {
            messaggioVideo = "";
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (datiCalcolo707 == null)
                return;

            ControlsDatiCalcolo707Cross(datiCalcolo707, datiPensione, datiFondo, datiDanteCausa, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
            {
                return;
            }

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcolo707WithFondi(datiCalcolo707, datiFondoXX, ref datiFondo, datiPensione, tipoFondo);

                quadroDatiContributivi.TabDatiCalcolo707 = 2;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void DeleteDatiCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            if (Utility.IsDomandaReversibilita(datiPensione))
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            #region gestioneFondi

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            GestioneFondo.DatiFondoFST datiFondoFST = null;
            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            GestioneFondo.DatiFondoPI datiFondoPI = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.TT:
                        GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                        if (datiFondoTT != null)
                        {
                            datiFondoTT.RetribuzioneBiennio = null;
                            datiFondoTT.RetribuzioneUltimoAnnoQuotaA = null;
                            datiFondoTT.PensioneMensileAl53 = null;
                            datiFondoTT.ElementiAccessori = null;
                            datiFondoTT.RetribuzioneSupplementi = null;
                            IsFondoNull = datiFondoTT.Equals(new GestioneFondo.DatiFondoTT());
                        }
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);
                        if (datiFondoVL != null)
                        {
                            datiFondoVL.LavoratorePrecoce = null;
                            IsFondoNull = datiFondoVL.Equals(new GestioneFondo.DatiFondoVL());
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.GetFondoFSTByIdPensione(datiPensione.Id, out datiFondoFST);
                        if (datiFondoFST != null)
                        {
                            datiFondoFST.PensioneAnnuaLorda = null;
                            datiFondoFST.ServizioUtileDirittoAA = null;
                            datiFondoFST.ServizioUtileDirittoMM = null;
                            datiFondoFST.ServizioUtileDirittoGG = null;
                            datiFondoFST.CoefficienteTrasformazione = null;
                            IsFondoNull = datiFondoFST.Equals(new GestioneFondo.DatiFondoFST());
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.GetFondoPTByIdPensione(datiPensione.Id, out datiFondoPT);
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.PensioneAnnuaLorda = null;
                            datiFondoPT.ServizioUtileDirittoAA = null;
                            datiFondoPT.ServizioUtileDirittoMM = null;
                            datiFondoPT.ServizioUtileDirittoGG = null;
                            datiFondoPT.CoefficienteTrasformazione = null;
                            IsFondoNull = datiFondoPT.Equals(new GestioneFondo.DatiFondoPT());
                        }
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        if (datiFondoGAS != null)
                        {
                            datiFondoGAS.CodiceTipoLiquidazione = null;
                            datiFondoGAS.DecorrenzaDatiAgo = null;
                            datiFondoGAS.SospensioneAGO = null;
                            datiFondoGAS.SettimaneAnzianitaEsclusiva = null;
                            datiFondoGAS.AnniDifferimento = null;
                            datiFondoGAS.EtaMaturazioneRequisiti = null;
                            datiFondoGAS.CodiceSpecificoAgo = null;
                            datiFondoGAS.DecorrenzaTeorica = null;
                            IsFondoNull = datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS());
                        }
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.GetFondoDZByIdPensione(datiPensione.Id, out datiFondoDZ);
                        if (datiFondoDZ != null)
                        {
                            datiFondoDZ.PensioneBaseAnnua = null;
                            datiFondoDZ.Sospensione = null;

                            IsFondoNull = datiFondoDZ.Equals(new GestioneFondo.DatiFondoDZ());
                        }
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            FondoES_AGO fondoEsAgo = new FondoES_AGO();
                            Utility.ValorizzaOggetti(fondoEsAgo, datiFondoES);
                            IsFondoNull = datiFondoES.Equals(new GestioneFondo.DatiFondoES());
                        }
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.GetFondoPIByIdPensione(datiPensione.Id, out datiFondoPI);
                        if (datiFondoPI != null)
                        {
                            FondoPI fondoPI = new FondoPI();
                            Utility.ValorizzaOggetti(fondoPI, datiFondoPI);
                            IsFondoNull = datiFondoPI.Equals(new GestioneFondo.DatiFondoPI());
                        }
                        break;
                }
            }

            #endregion gestioneFondi

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (datiFondo != null)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.TT:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoTT, datiFondo.Id, IsFondoNull);
                                if (datiFondo != null)
                                    datiFondo.RetrPondAnnuaAGOLimite = null;
                                break;
                            case Utility.TipoFondo.ET:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                break;
                            case Utility.TipoFondo.VL:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoVL, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.FS:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoFST, datiFondo.Id, IsFondoNull);
                                if (datiMaggiorazioniBenefici != null)
                                    datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = null;

                                if (!Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBenefici))
                                    Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
                                else
                                    Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                                break;
                            case Utility.TipoFondo.PT:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoPT, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.GAS:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoGAS, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.DZ:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoDZ, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.ES:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoES, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.PM:
                                //DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoES, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoPI, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.EL:
                                if (datiFondo != null)
                                    datiFondo.RetrPondAnnuaAGOLimite = null;
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                break;
                        }
                    }
                    if (datiFondo.IsFondoNull() && IsFondoNull)
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                    else
                    {
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        {
                            datiFondo.RiduzioneRetributiva = false;
                            datiFondo.RiduzioneRetributivaPercentuale = null;
                            datiFondo.QuotaA707 = null;
                            datiFondo.QuotaA2707 = null;
                            datiFondo.QuotaB707 = null;
                            datiFondo.QuotaC707 = null;
                            datiFondo.QuotaC2707 = null;
                            datiFondo.QuotaD707 = null;
                            datiFondo.QuotaA707AA = null;
                            datiFondo.QuotaA707MM = null;
                            datiFondo.QuotaA707GG = null;
                            datiFondo.QuotaB707AA = null;
                            datiFondo.QuotaB707MM = null;
                            datiFondo.QuotaB707GG = null;
                            datiFondo.QuotaC707AA = null;
                            datiFondo.QuotaC707MM = null;
                            datiFondo.QuotaC707GG = null;
                            datiFondo.RetribuzionePonderataAGO707 = null;
                            datiFondo.SettimaneUtiliDiritto = null;
                            datiFondo.SettimaneUtiliDirittoOI = null;
                        }
                        GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                    }
                }

                GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);

                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.GAS:
                        if (Utility.IsDomandaReversibilita(datiPensione))
                        {
                            if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                            {
                                if (Utility.DataStrettamenteSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                {
                                    if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                        datiQuadroDatiContributivi.TabDatiAgo = 0;
                                    else
                                        datiQuadroDatiContributivi.TabDatiAgo = 1;
                                }
                                else
                                    datiQuadroDatiContributivi.TabDatiAgo = 1;
                            }
                        }
                        else
                        {
                            if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1998, 02, 01)))
                            {
                                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                    datiQuadroDatiContributivi.TabDatiAgo = 0;
                                else
                                    datiQuadroDatiContributivi.TabDatiAgo = 1;
                            }
                            else
                                datiQuadroDatiContributivi.TabDatiAgo = 1;
                        }
                        break;
                    case Utility.TipoFondo.PM:
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.PT:
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                        {
                            datiQuadroDatiContributivi.Tipo = 2;
                            datiQuadroDatiContributivi.TabDatiCalcolo = 0;
                        }
                        else
                            datiQuadroDatiContributivi.TabDatiCalcolo = 1;
                        break;


                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiCalcolo707ByDatiPensione(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            #region gestioneFondi

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;
            GestioneFondo.DatiFondoFST datiFondoFST = null;
            GestioneFondo.DatiFondoPT datiFondoPT = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.FS:

                        GestioneFondo.GetFondoFSTByIdPensione(datiPensione.Id, out datiFondoFST);
                        if (datiFondoFST != null)
                        {
                            datiFondoFST.PensioneAnnuaLorda707 = null;
                            IsFondoNull = datiFondoFST.Equals(new GestioneFondo.DatiFondoFST());
                        }
                        break;
                    case Utility.TipoFondo.PT:

                        GestioneFondo.GetFondoPTByIdPensione(datiPensione.Id, out datiFondoPT);
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.PensioneAnnuaLorda707 = null;
                            IsFondoNull = datiFondoPT.Equals(new GestioneFondo.DatiFondoPT());
                        }
                        break;
                }
            }

            #endregion gestioneFondi

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (datiFondo != null)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.FS:
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoFST, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.PT:
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoPT, datiFondo.Id, IsFondoNull);
                                break;
                        }
                    }
                }

                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                    case Utility.TipoFondo.FS:
                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                        {
                            datiQuadroDatiContributivi.Tipo = 2;
                            datiQuadroDatiContributivi.TabDatiCalcolo707 = 0;
                        }
                        else
                            datiQuadroDatiContributivi.TabDatiCalcolo707 = 1;
                        break;
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, DatiCalcolo datiCalcolo,
            GestioneFondo.DatiFondo datiFondoCommon, object datiFondoXX, char? codiceSpecificoTraduzioneSuGP, Utility.TipoFondo? tipoFondo, out GestioneLiquidazionePensione.TipoSalvaguardia? TipologiaSalvaguardia,
            out Dictionary<string, char?> TipoPensione, out Utility.CategoriaFondoPI? categoriaFondoPI)
        {
            bool? isRiduzioneRetribVisible;
            bool? isContribL214Visible;
            bool? isAnzianita;
            bool? isVecchiaiaSpecifica;
            bool? isInvaliditaSpecifica;
            TipologiaSalvaguardia = null;
            bool? isUsuranti;
            TipoPensione = null;
            bool? isAltraPensioneVisible;
            bool? isDecorrenzaSuccSett1989;
            bool? isRiduzioneRetributivaEnabled;
            bool? isSettimane707Visible;
            bool? isAnteArmonizzazione;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            //ENG - PL CONTRIBUZIONE POST 2011
            bool? isContribuzioneL335NonObbligatoria;
            bool? isPIAPIBAnte99;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            isRiduzioneRetribVisible = GestioneRiduzioneRetributiva(datiPensione, datiFondoCommon, datiFondoXX);
            isContribL214Visible = GestioneContributivoL214ForTipoFondo(datiPensione, datiFondoCommon, datiDanteCausa);
            isAnzianita = IsAnzianita(datiPensione);
            isVecchiaiaSpecifica = IsVecchiaiaSpecifica(datiPensione);
            isInvaliditaSpecifica = IsInvaliditaSpecifica(datiPensione);
            TipologiaSalvaguardia = GestioneLiquidazionePensione.GetTipoSalvaguardia(datiPensione);
            isUsuranti = Utility.IsDomandaUsuranti(datiPensione);
            TipoPensione = Utility.GetTipoPensione(datiPensione);
            isAltraPensioneVisible = IsAltraPensioneVisible(datiPensione);
            isDecorrenzaSuccSett1989 = decorrenzaPensioneOrDecorrenzaPensioneDC.HasValue && Utility.DataSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.Value, new DateTime(1989, 10, 1)) ? true : false;
            isRiduzioneRetributivaEnabled = Utility.GestioneRiduzioneRetributivaEnabled(datiPensione, isRiaperturaDomanda, null, null);
            isSettimane707Visible = GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, datiCalcolo != null ? !datiCalcolo.IsContribL214Null() : false);
            categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            isAnteArmonizzazione = Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC, datiFondoXX: datiFondoXX, datiFondo: datiFondoCommon);
            //ENG - PL CONTRIBUZIONE POST 2011
            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(2011, 12, 31)))
            {
                isContribuzioneL335NonObbligatoria = true;
            }
            else
                isContribuzioneL335NonObbligatoria = false;

            isPIAPIBAnte99 = Utility.IsPIAPIBAnte99(Utility.TipoAppartenenza.FS, datiPensione, datiDanteCausa);

            lReturn.Add("RiduzioneRetribVisible", isRiduzioneRetribVisible);
            lReturn.Add("ContribL214Visible", isContribL214Visible);
            lReturn.Add("Anzianita", isAnzianita);
            lReturn.Add("VecchiaiaSpecifica", isVecchiaiaSpecifica);
            lReturn.Add("InvaliditaSpecifica", isInvaliditaSpecifica);
            lReturn.Add("Usuranti", isUsuranti);
            lReturn.Add("IsAltraPensioneVisible", isAltraPensioneVisible);
            lReturn.Add("IsDecorrenzaSuccSett1989", isDecorrenzaSuccSett1989);
            lReturn.Add("IsRiduzioneRetributivaEnabled", isRiduzioneRetributivaEnabled);
            lReturn.Add("IsSettimane707Visible", isSettimane707Visible);
            lReturn.Add("IsAnteArmonizzazione", isAnteArmonizzazione);
            lReturn.Add("IsContribuzioneL335NonObbligatoria", isContribuzioneL335NonObbligatoria);
            lReturn.Add("IsPIAPIBAnte99", isPIAPIBAnte99);

            return lReturn;
        }


        public static bool? GestioneRiduzioneRetributiva(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondoCommon, object datiFondoXX)
        {
            if (datiPensione == null)
                return false;

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 02, 01)) &&
                    tipoCalcolo == Utility.TipoCalcolo.Retributivo || tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti || tipoCalcolo == Utility.TipoCalcolo.Misto)
                {
                    if (tipoFondo.HasValue)
                    {
                        List<GestioneDecodifica.CodiceSpecifico> listaCodiceSpecifico = null;
                        GestioneDecodifica.GetCodiceSpecifico(out listaCodiceSpecifico);
                        if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count > 0)
                        {
                            GestioneDecodifica.CodiceSpecifico codiceSpecifico = listaCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                            if (codiceSpecifico != null)
                            {
                                switch (tipoFondo.Value)
                                {
                                    case Utility.TipoFondo.EL:
                                        if (codiceSpecifico.TraduzioneGp == 'C')
                                            return true;
                                        break;
                                    case Utility.TipoFondo.TT:
                                        if (codiceSpecifico.TraduzioneGp == 'E')
                                            return true;
                                        break;
                                    case Utility.TipoFondo.ET:
                                        if (codiceSpecifico.TraduzioneGp == 'D' || codiceSpecifico.TraduzioneGp == 'W' || codiceSpecifico.TraduzioneGp == 'X')
                                            return true;
                                        break;
                                    case Utility.TipoFondo.GAS:
                                        if (codiceSpecifico.TraduzioneGp == 'B')
                                            return true;
                                        break;
                                    case Utility.TipoFondo.VL:
                                        if (((GestioneFondo.DatiFondoVL)datiFondoXX) != null && ((GestioneFondo.DatiFondoVL)datiFondoXX).CodiceArt22.GetValueOrDefault() == 1)
                                            return true;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            if (Utility.IsDomandaReversibilita(datiPensione))
                return true;
            if (string.IsNullOrEmpty(datiPensione.Gruppo) || datiPensione.Gruppo != "0001")
                return false;
            if (string.IsNullOrEmpty(datiPensione.Prodotto) || datiPensione.Prodotto != "0001")
                return false;

            if (tipoCalcolo == Utility.TipoCalcolo.Retributivo || tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti || tipoCalcolo == Utility.TipoCalcolo.Misto)
                if (datiPensione.DataPerfezionamentoRequisiti.HasValue && DateTime.Compare(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 12, 31).Date) > 0)
                    return true;

            return false;
        }

        public static bool? GestioneContributivoL214ForTipoFondo(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondoCommon, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (datiPensione == null)
                return false;

            char? codiceSpecifico = null;
            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                        codiceSpecifico = codice.TraduzioneGp;
                }
            }

            return GestioneContributivoL214(datiPensione, datiDanteCausa, codiceSpecifico);

        }

        public static bool? IsAnzianita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")
                    return true;
            }

            return false;
        }

        public static bool? IsAltraPensioneVisible(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                //date <= a 10/1983
                if (datiPensione.DecorrenzaOriginaria < new DateTime(1983, 11, 1))
                    return true;
            }
            return false;
        }

        public static bool? IsVecchiaiaSpecifica(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0011")
                    return true;
            }

            return false;
        }

        public static bool? IsInvaliditaSpecifica(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET && datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0011")
                    return true;
            }

            return false;
        }

        public static void GetDatiFondoAndDatiArt14e11(GestionePensione.DatiPensione datiPensione, out GestioneContrib.EntityDatiFondo entityDatifondoGAS_ES, out GestioneContrib.DatiArt11e14 entityDatiArt11e14)
        {
            entityDatifondoGAS_ES = null;
            entityDatiArt11e14 = null;
            //entityDatiES = null;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.DatiFondoGAS datiFondoGAS = null;
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        if (datiFondoGAS != null)
                        {
                            //Tab Dati Art 11 e 14
                            entityDatiArt11e14 = new DatiArt11e14();
                            entityDatiArt11e14.ContributiTotaliSupplementoDPR143271 = datiFondoGAS.ContributiTotaliSupplementoDPR143271;
                            entityDatiArt11e14.ContribuzioneEsclusivaDPR143271 = datiFondoGAS.ContribuzioneEsclusivaDPR143271;
                            entityDatiArt11e14.CCTotaliArt14 = datiFondoGAS.CCTotaliArt14;
                            entityDatiArt11e14.ContribuzioneEsclusiva = datiFondoGAS.ContribuzioneEsclusiva;
                            entityDatiArt11e14.DecDPCM = datiFondoGAS.DecDPCM;
                            entityDatiArt11e14.RMSArt14 = datiFondoGAS.RMSArt14;
                            entityDatiArt11e14.RMSSent72 = datiFondoGAS.RMSSent72;
                            entityDatiArt11e14.CCTotaliArt11 = datiFondoGAS.CCTotaliArt11;
                            entityDatiArt11e14.CCEsclusivaArt11 = datiFondoGAS.CCEsclusivaArt11;
                        }

                        List<GestioneDatiServizioUtile.ServizioUtile> listServizioUtile = null;
                        GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listServizioUtile);
                        if (listServizioUtile != null && listServizioUtile.Count > 0)
                        {
                            //Tab Dati Fondo
                            entityDatifondoGAS_ES = new EntityDatiFondo();
                            entityDatifondoGAS_ES.ServizioUtileAA = listServizioUtile.FirstOrDefault().ServizioUtileAA;
                            entityDatifondoGAS_ES.ServizioUtileMM = listServizioUtile.FirstOrDefault().ServizioUtileMM;
                            entityDatifondoGAS_ES.RetribuzionePensionabile = listServizioUtile.FirstOrDefault().RetribuzionePensionabile;
                        }
                        break;
                    case Utility.TipoFondo.ES:
                        entityDatifondoGAS_ES = new EntityDatiFondo();
                        entityDatifondoGAS_ES.fondoES = new FondoES();
                        //decodifica
                        List<GestioneDecodifica.DecPromiscui> decPromiscui;
                        GestioneDecodifica.GetDecodificaPromiscui(out decPromiscui, Utility.TipoFondo.ES.ToString());
                        entityDatifondoGAS_ES.fondoES.DecPromiscui = decPromiscui;
                        List<GestioneDecodifica.DecArt58> decArt58;
                        GestioneDecodifica.GetDecodificaArt58(out decArt58, Utility.TipoFondo.ES.ToString());
                        entityDatifondoGAS_ES.fondoES.DecArt58 = decArt58;

                        GestioneFondo.DatiFondoES datiFondoES = null;
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            //sezione - dati fondo
                            List<GestioneDatiServizioUtile.ServizioUtile> listServizioUtileES = null;
                            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listServizioUtileES);
                            if (listServizioUtileES != null && listServizioUtileES.Count > 0)
                            {
                                entityDatifondoGAS_ES.ServizioUtileAA = listServizioUtileES.FirstOrDefault().ServizioUtileAA;
                                entityDatifondoGAS_ES.ServizioUtileMM = listServizioUtileES.FirstOrDefault().ServizioUtileMM;
                                entityDatifondoGAS_ES.RetribuzionePensionabile = listServizioUtileES.FirstOrDefault().RetribuzionePensionabile;
                            }
                            Utility.ValorizzaOggetti(datiFondoES, entityDatifondoGAS_ES.fondoES);
                        }
                        else
                        {
                            //DatiFondo non presente - prevalorizzare i campi secondo specifiche
                            //sezione - dati fondo
                            List<GestioneDatiServizioUtile.ServizioUtile> listServizioUtileES = null;
                            GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listServizioUtileES);
                            if (listServizioUtileES != null && listServizioUtileES.Count > 0)
                            {
                                entityDatifondoGAS_ES.ServizioUtileAA = listServizioUtileES.FirstOrDefault().ServizioUtileAA;
                                entityDatifondoGAS_ES.ServizioUtileMM = listServizioUtileES.FirstOrDefault().ServizioUtileMM;
                                entityDatifondoGAS_ES.RetribuzionePensionabile = listServizioUtileES.FirstOrDefault().RetribuzionePensionabile;
                            }
                            //sezione - codici
                            entityDatifondoGAS_ES.fondoES.AnnoUtile = null;
                            entityDatifondoGAS_ES.fondoES.Articolo58 = null;
                            entityDatifondoGAS_ES.fondoES.Articolo59 = false;
                            entityDatifondoGAS_ES.fondoES.ClassePensioneAnte50 = null;
                            entityDatifondoGAS_ES.fondoES.CodiceDz = false;
                            entityDatifondoGAS_ES.fondoES.CodiceEsattoria = null;
                            entityDatifondoGAS_ES.fondoES.CodiciRetributivi = null;
                            entityDatifondoGAS_ES.fondoES.Optanti = (false);
                            entityDatifondoGAS_ES.fondoES.MaggiorazionePrivilegiata = false;
                            entityDatifondoGAS_ES.fondoES.Promiscui = 0;
                            entityDatifondoGAS_ES.fondoES.Saltuari = (false);

                        }
                        if (datiFondoES != null && (datiFondoES.DecDPCM.HasValue || datiFondoES.RmsDPCM.HasValue || datiFondoES.RMSSent72.HasValue))
                        {
                            //Tab Dati Art 11 e 14
                            entityDatiArt11e14 = new DatiArt11e14();
                            entityDatiArt11e14.DecDPCM = datiFondoES.DecDPCM;
                            entityDatiArt11e14.RMSArt14 = datiFondoES.RmsDPCM;
                            entityDatiArt11e14.RMSSent72 = datiFondoES.RMSSent72;

                        }
                        break;
                }
            }
        }

        public static void GetDatiAnte67AndSL336(GestionePensione.DatiPensione datiPensione, out GestioneContrib.DatiAnte67 entityDatiAnte67, out GestioneContrib.DatiSL33670 entityDatiSL336)
        {
            entityDatiAnte67 = null;
            entityDatiSL336 = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.ES:
                        entityDatiAnte67 = new DatiAnte67();

                        GestioneFondo.DatiFondoES datiFondoES = null;
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            //sezione - Ante 67
                            entityDatiAnte67 = new DatiAnte67();
                            Utility.ValorizzaOggetti(datiFondoES, entityDatiAnte67);
                            //sezione - SL336
                            entityDatiSL336 = new DatiSL33670();
                            Utility.ValorizzaOggetti(datiFondoES, entityDatiSL336);
                        }
                        break;
                }
            }
        }

        public static void GetAltraPensioneDatiAGO_ET(GestionePensione.DatiPensione datiPensione, object datiFondoXX, out DatiAgoAltraPensione entityDatiAgoAltraPensione)
        {
            entityDatiAgoAltraPensione = null;

            GestioneFondo.DatiFondoET datiFondoET = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET)
            {
                if (datiFondoXX != null)
                {
                    datiFondoET = (GestioneFondo.DatiFondoET)datiFondoXX;
                    entityDatiAgoAltraPensione = new DatiAgoAltraPensione();
                    Utility.ValorizzaOggetti(datiFondoET, entityDatiAgoAltraPensione);
                }
            }
        }

        public static void GetElencoDatiAgoPi(GestionePensione.DatiPensione datiPensione, out List<GestioneFondo.PretabellaDatiAgoFondoPI> elencoDatiAgo)
        {
            elencoDatiAgo = null;
            GestioneFondo.GetElencoDatiAgoPIByIdPensione_pretabella(datiPensione.Id, out elencoDatiAgo);
        }

        public static void GetElencoDatiFondoPi(GestionePensione.DatiPensione datiPensione, out List<GestioneFondo.PretabellaPensioneFondoPI> elencoDatiFondoPI)
        {
            elencoDatiFondoPI = null;
            GestioneFondo.GetElencoPensioneFondoPIByIdPensione_pretabella(datiPensione.Id, out elencoDatiFondoPI);
        }

        public static void GetStoricoGP(GestionePensione.DatiPensione datiPensione, out DatiCalcolo datiCalcoloStorico)
        {
            datiCalcoloStorico = null;
            CrossDataRecipient crossDataRecipient = new CrossDataRecipient();  // contenitore di proprietà prelevabili e/o non prelevabili da AggPeco non appartenenti a dati Calcolo e /o Retributivi 
            GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco = new GestioneAggiornamentoPECO.DatiTotaliAggPeco();

            GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoStoricoByIdPensione(datiPensione.Id, out datiContributivi);

            GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
            GestioneCalcolo.GetCalcoloRetributivoStoricoByIdPensione(datiPensione.Id, out datiRetributivi);

            GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
            GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            datiAggPeco.DatiParziali = new GestioneAggiornamentoPECO.DatiParzialiAggPeco();
            datiAggPeco.DatiParziali.InizioAssicurazione = datiPensione.InizioAssicurazione;
            datiAggPeco.DatiParziali.FineAssicurazione = datiPensione.FineAssicurazione;
            datiAggPeco.DatiParziali.DecorrenzaPensione = datiPensione.DecorrenzaOriginaria;

            if (datiPensione.TipoCalcolo.HasValue)
                datiAggPeco.DatiParziali.TipoCalcolo = (GestioneAggiornamentoPECO.TipoCalcolo)Liquidazione.BLCommon.Utility.GetTipoCalcolo(datiPensione);

            GestioneAggiornamentoPECO.DatiContributivi Contribuzione = null;
            GestioneAggiornamentoPECO.DatiRetributivi Retribuzione = null;
            RecuperaDatiCalcoloFromDB(datiContributivi, datiRetributivi, out Retribuzione, out Contribuzione);

            datiAggPeco.Contribuzione = Contribuzione;
            datiAggPeco.Retribuzione = Retribuzione;

            RecuperaDatiCalcFromFondiStoricoByIdPensione(datiStoricoGP, tipoFondo, ref datiAggPeco);
            RecuperaDatiServizioUtileStorico(datiPensione.Id, tipoFondo, ref crossDataRecipient);
            RecuperaDatiFlatForDatiCalcoloStorico(datiStoricoGP, ref crossDataRecipient);
            RecuperaDati707ForDatiCalcoloStorico(datiStoricoGP, ref datiAggPeco);

            datiCalcoloStorico = new DatiCalcolo(datiAggPeco, tipoFondo, crossDataRecipient, datiPensione.TipoCalcolo, datiPensione.Gruppo, datiPensione.IsPLUnicarpe, datiPensione);
        }

        public static void StoreDatiFondo(GestionePensione.DatiPensione datiPensione, EntityDatiFondo entityDatiFondo, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (entityDatiFondo == null)
                entityDatiFondo = new EntityDatiFondo();

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.GAS:

                    GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                    List<GestioneDatiServizioUtile.ServizioUtile> listDatiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listDatiServizioUtile);

                    //controlla se sulla tabella PensioneFondoDatiGenerici ci stanno dei record
                    bool eliminaFondoDatiGenerici = true;
                    long idFondo = 0;
                    if (datiFondo != null)
                    {
                        idFondo = datiFondo.Id;
                        eliminaFondoDatiGenerici = false;
                    }

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                        StoreDatiServizioUtilePerFondo(datiPensione.Id, idFondo, entityDatiFondo, listDatiServizioUtile.FirstOrDefault(), entityDatiFondo.IsNull(), eliminaFondoDatiGenerici);
                        //StoreDatiFondoPerFondoGas(datiPensione.Id, idFondo, entityDatiFondoGAS, datiFondoGAS, entityDatiFondoGAS.IsNull(), eliminaFondoDatiGenerici);

                        if (entityDatiFondo.IsNull())
                            quadroDatiContributivi.TabDatiFondo = 0;
                        else
                            quadroDatiContributivi.TabDatiFondo = 2;

                        GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                        transactionScope.Complete();
                    }
                    break;
                case Utility.TipoFondo.ES:
                    GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)fondoXX;

                    quadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                    listDatiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listDatiServizioUtile);

                    //GestioneFondo.DatiFondoES DatiFondoEs = null;
                    //GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out DatiFondoEs);

                    eliminaFondoDatiGenerici = true;
                    idFondo = 0;
                    if (datiFondo != null)
                    {
                        idFondo = datiFondo.Id;
                        eliminaFondoDatiGenerici = false;
                    }

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);

                        StoreDatiServizioUtilePerFondo(datiPensione.Id, idFondo, entityDatiFondo, listDatiServizioUtile.FirstOrDefault(), entityDatiFondo.IsNull(), eliminaFondoDatiGenerici);
                        //StoreDatiFondoPerFondoGas(datiPensione.Id, idFondo, entityDatiFondoGAS, datiFondoGAS, entityDatiFondoGAS.IsNull(), eliminaFondoDatiGenerici);

                        StoreDatiFondoPerFondoEs(datiPensione.Id, idFondo, entityDatiFondo.fondoES, ref datiFondoES, entityDatiFondo.fondoES == null || entityDatiFondo.fondoES.IsNull(), eliminaFondoDatiGenerici);

                        if (entityDatiFondo.IsNull())
                            quadroDatiContributivi.TabDatiFondo = 0;
                        else
                            quadroDatiContributivi.TabDatiFondo = 2;

                        GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                        transactionScope.Complete();
                    }
                    break;
            }
        }

        public static void StoreDatiArt14e11(GestionePensione.DatiPensione datiPensione, DatiArt11e14 datiArt11e14, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, DatiCalcolo datiCalcolo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            bool eliminaFondoDatiGenerici = true;
            long idFondo = 0;
            if (datiFondo != null)
            {
                idFondo = datiFondo.Id;
                eliminaFondoDatiGenerici = false;
            }
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            ControlsDatiArt11e14(datiArt11e14, datiPensione, tipoFondo, fondoXX, datiCalcolo, isSingleTab, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.GAS:
                            GestioneFondo.DatiFondoGAS datiFondoGAS = (GestioneFondo.DatiFondoGAS)fondoXX;
                            StoreDatiArt11e14PerFondoGas(datiPensione.Id, idFondo, datiArt11e14, datiFondoGAS, datiArt11e14 == null || datiArt11e14.IsNull(), eliminaFondoDatiGenerici);
                            if (datiArt11e14 == null || datiArt11e14.IsNull())
                                quadroDatiContributivi.TabArt11e14 = 1;
                            else
                                quadroDatiContributivi.TabArt11e14 = 2;
                            break;
                        case Utility.TipoFondo.ES:
                            GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)fondoXX;
                            StoreDatiArt11e14PerFondoEs(datiPensione.Id, idFondo, datiArt11e14, datiFondoES, datiArt11e14 == null || datiArt11e14.IsNull(), eliminaFondoDatiGenerici);
                            if (datiArt11e14 == null || datiArt11e14.IsNull())
                                quadroDatiContributivi.TabArt11e14 = 1;
                            else
                                quadroDatiContributivi.TabArt11e14 = 2;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void StoreDatiAnte67(GestionePensione.DatiPensione datiPensione, DatiAnte67 datiAnte67, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, DatiCalcolo datiCalcolo,
            bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            bool eliminaFondoDatiGenerici = true;
            long idFondo = 0;
            if (datiFondo != null)
            {
                idFondo = datiFondo.Id;
                eliminaFondoDatiGenerici = false;
            }
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            // ControlsDatiArt11e14(datiAnte67, datiPensione, tipoFondo, fondoXX, datiCalcolo, isSingleTab, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ES:
                            GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)fondoXX;
                            StoreDatiAnte67PerFondoEs(datiPensione.Id, idFondo, datiAnte67, datiFondoES, datiAnte67 == null || datiAnte67.IsNull(), eliminaFondoDatiGenerici);
                            if (datiAnte67 == null || datiAnte67.IsNull())
                                quadroDatiContributivi.TabAnte67 = 1;
                            else
                                quadroDatiContributivi.TabAnte67 = 2;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void StoreDatiSL336(GestionePensione.DatiPensione datiPensione, DatiSL33670 datiSL336, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX, DatiCalcolo datiCalcolo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            bool eliminaFondoDatiGenerici = true;
            long idFondo = 0;
            if (datiFondo != null)
            {
                idFondo = datiFondo.Id;
                eliminaFondoDatiGenerici = false;
            }
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            // ControlsDatiArt11e14(datiAnte67, datiPensione, tipoFondo, fondoXX, datiCalcolo, isSingleTab, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ES:
                            GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)fondoXX;
                            StoreDatiSL336PerFondoEs(datiPensione.Id, idFondo, datiSL336, datiFondoES, datiSL336 == null || datiSL336.IsNull(), eliminaFondoDatiGenerici);
                            if (datiSL336 == null || datiSL336.IsNull())
                                quadroDatiContributivi.TabSL33670 = 1;
                            else
                                quadroDatiContributivi.TabSL33670 = 2;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void StoreDatiAgoAltraPensione(GestionePensione.DatiPensione datiPensione, DatiAgoAltraPensione datiAgoAltraPensione, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX,
            DatiCalcolo datiCalcolo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            bool eliminaFondoDatiGenerici = true;
            long idFondo = 0;
            if (datiFondo != null)
            {
                idFondo = datiFondo.Id;
                eliminaFondoDatiGenerici = false;
            }

            // Non posso eliminare i dati dalla tabella PensioneFondoDatiGenerici se sono presenti dati nella tabella DatiServizioUtile
            if (eliminaFondoDatiGenerici)
            {
                if (isSingleTab)
                {
                    List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out listaDatiServizioUtile);
                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                        eliminaFondoDatiGenerici = false;
                }
                else
                    if (datiCalcolo != null && datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null && datiCalcolo.fondoET.lDatiServizioUtile.Count > 0)
                    eliminaFondoDatiGenerici = false;
            }

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ET:
                            GestioneFondo.DatiFondoET datiFondoET = (GestioneFondo.DatiFondoET)fondoXX;
                            StoreDatiAgoAltraPensionePerFondoET(datiPensione.Id, idFondo, datiAgoAltraPensione, datiFondoET, eliminaFondoDatiGenerici);
                            if (datiAgoAltraPensione == null || datiAgoAltraPensione.IsNull())
                                quadroDatiContributivi.TabDatiAgo = 0;
                            else
                                quadroDatiContributivi.TabDatiAgo = 2;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void DeleteDatiAnte67ByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            bool IsFondoGenericoNull = datiFondo != null ? datiFondo.IsFondoNull() : true;

            #region Gestione Fondi
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;

            GestioneFondo.DatiFondoES datiFondoES = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {

                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            DatiAnte67 entity = new DatiAnte67();
                            Utility.ValorizzaOggetti(entity, datiFondoES);

                            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
                                IsFondoNull = true;

                        }

                        break;
                }
            }
            #endregion Gestione Fondi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!IsFondoGenericoNull)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {



                            case Utility.TipoFondo.ES:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoES, datiFondo.Id, IsFondoNull);
                                break;
                        }
                    }

                    if (datiFondo.IsFondoNull() && IsFondoNull)
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                }

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ES:
                            datiQuadroDatiContributivi.TabAnte67 = 1;
                            break;

                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiSL336ByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            bool IsFondoGenericoNull = datiFondo != null ? datiFondo.IsFondoNull() : true;

            #region Gestione Fondi
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;

            GestioneFondo.DatiFondoES datiFondoES = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {

                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            DatiSL33670 entity = new DatiSL33670();
                            Utility.ValorizzaOggetti(entity, datiFondoES);

                            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
                                IsFondoNull = true;

                        }

                        break;
                }
            }
            #endregion Gestione Fondi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!IsFondoGenericoNull)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.ES:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoES, datiFondo.Id, IsFondoNull);
                                break;
                        }
                    }

                    if (datiFondo.IsFondoNull() && IsFondoNull)
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                }

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ES:
                            datiQuadroDatiContributivi.TabSL33670 = 1;
                            break;

                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiFondoByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo, ref object fondoXX)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            bool IsFondoGenericoNull = datiFondo != null ? datiFondo.IsFondoNull() : true;
            long idDatiFondo = (datiFondo != null) ? datiFondo.Id : 0;

            #region Gestione Fondi
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            //bool IsFondoNull = false;
            //GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            //if (tipoFondo.HasValue)
            //{
            //    switch (tipoFondo)
            //    {
            //        case Utility.TipoFondo.GAS:
            //            GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
            //            if (datiFondoGAS != null)
            //            {
            //                datiFondoGAS.ServizioUtileAA = null;
            //                datiFondoGAS.ServizioUtileMM = null;
            //                datiFondoGAS.RetribuzionePensionabile = null;
            //            }
            //            break;
            //    }
            //}
            #endregion Gestione Fondi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.GAS:
                            datiQuadroDatiContributivi.TabDatiFondo = 0;
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            //DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoGAS, datiFondo.Id, IsFondoNull);
                            if (datiFondo != null && datiFondo.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            break;
                        case Utility.TipoFondo.ES:
                            datiQuadroDatiContributivi.TabDatiFondo = 0;
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            GestioneFondo.DatiFondoES fondoES = (GestioneFondo.DatiFondoES)fondoXX; //tabella fondo specifico
                            Utility.ValorizzaOggetti(new GestioneContrib.FondoES(), fondoES);
                            bool isFondoSpecificoNull = true;
                            if (fondoES != null)
                                isFondoSpecificoNull = fondoES.isNull();
                            StoreDatiFondoPerFondoEs(datiPensione.Id, idDatiFondo, new GestioneContrib.FondoES(), ref fondoES, isFondoSpecificoNull, IsFondoGenericoNull);
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiArt14e11ByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            bool IsFondoGenericoNull = datiFondo != null ? datiFondo.IsFondoNull() : true;

            #region Gestione Fondi
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        if (datiFondoGAS != null)
                        {
                            datiFondoGAS.ContributiTotaliSupplementoDPR143271 = null;
                            datiFondoGAS.ContribuzioneEsclusivaDPR143271 = null;
                            datiFondoGAS.CCTotaliArt14 = null;
                            datiFondoGAS.ContribuzioneEsclusiva = null;
                            datiFondoGAS.DecDPCM = null;
                            datiFondoGAS.RMSArt14 = null;
                            datiFondoGAS.RMSSent72 = null;
                            datiFondoGAS.CCTotaliArt11 = null;
                            datiFondoGAS.CCEsclusivaArt11 = null;

                            if (datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))
                                IsFondoNull = true;

                        }
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            datiFondoES.DecDPCM = null;
                            datiFondoES.RmsDPCM = null;
                            datiFondoES.RMSSent72 = null;

                            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
                                IsFondoNull = true;

                        }

                        break;
                }
            }
            #endregion Gestione Fondi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!IsFondoGenericoNull)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.GAS:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoGAS, datiFondo.Id, IsFondoNull);
                                break;
                            case Utility.TipoFondo.ES:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoES, datiFondo.Id, IsFondoNull);
                                break;
                        }
                    }

                    if (datiFondo.IsFondoNull() && IsFondoNull)
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                }

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.GAS:
                        case Utility.TipoFondo.ES:
                            datiQuadroDatiContributivi.TabArt11e14 = 1;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiAgoAltraPensioneByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondo)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            bool IsFondoGenericoNull = datiFondo != null ? datiFondo.IsFondoNull() : true;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            #region Gestione Fondi
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool IsFondoNull = false;
            GestioneFondo.DatiFondoET datiFondoET = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.ET:
                        GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiFondoET);
                        if (datiFondoET != null)
                        {
                            Utility.ValorizzaOggetti(new DatiAgoAltraPensione(), datiFondoET);

                            if (datiFondoET.Equals(new GestioneFondo.DatiFondoET()))
                                IsFondoNull = true;
                        }

                        break;
                }
            }
            #endregion Gestione Fondi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!IsFondoGenericoNull)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.ET:
                                DeleteDatiContributiviWithFondi(datiPensione.Id, tipoFondo, datiFondoET, datiFondo.Id, IsFondoNull);
                                break;
                        }
                    }

                    if (datiFondo.IsFondoNull() && IsFondoNull)
                        GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                }

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.ET:
                            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                datiQuadroDatiContributivi.TabDatiAgo = 0;
                            else
                                datiQuadroDatiContributivi.TabDatiAgo = 1;
                            break;
                    }
                }

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiFondo(EntityDatiFondo datiFondo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.DifferenzaDateTime diff;

            switch (tipoFondo)
            {
                case Utility.TipoFondo.GAS:
                    if (!datiPensione.FineAssicurazione.HasValue || !datiPensione.InizioAssicurazione.HasValue)
                    {
                        messaggioVideo = "Inizio e Fine Assicurazione mancanti";
                        return false;
                    }

                    diff = Utility.DifferenzaBetweenDate(datiPensione.FineAssicurazione.Value.AddDays(1), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);

                    if (datiFondo != null)
                    {
                        if (datiFondo.ServizioUtileAA.HasValue && datiFondo.ServizioUtileAA.Value > diff.Year)
                        {
                            messaggioVideo = "Servizio utile AA deve essere minore o uguale a " + diff.Year;
                            return false;
                        }
                        if (datiFondo.ServizioUtileAA.HasValue && datiFondo.ServizioUtileAA.Value == diff.Year && datiFondo.ServizioUtileMM.HasValue && datiFondo.ServizioUtileMM.Value > diff.Month)
                        {
                            messaggioVideo = "Servizio utile MM deve essere minore o uguale a " + diff.Month;
                            return false;
                        }
                        if (!datiFondo.ControCodice.HasValue)
                        {
                            messaggioVideo = "Il Controcodice Retributivo è obbligatorio";
                            return false;
                        }

                        if (!GestioneControlli.CheckImportoWithControCodice(datiFondo.RetribuzionePensionabile, datiFondo.ControCodice, datiPensione, out messaggioVideo))
                            return false;
                    }
                    break;
                case Utility.TipoFondo.ES:
                    if (!datiPensione.FineAssicurazione.HasValue || !datiPensione.InizioAssicurazione.HasValue)
                    {
                        messaggioVideo = "Inizio e Fine Assicurazione mancanti";
                        return false;
                    }
                    diff = Utility.DifferenzaBetweenDate(datiPensione.FineAssicurazione.Value.AddDays(1), datiPensione.InizioAssicurazione, Utility.TipoAppartenenza.FS);
                    if (datiFondo != null)
                    {
                        if (datiFondo.ServizioUtileAA.HasValue && datiFondo.ServizioUtileAA.Value > diff.Year)
                        {
                            messaggioVideo = "Servizio utile AA deve essere minore o uguale a " + diff.Year;
                            return false;
                        }
                        if (datiFondo.ServizioUtileAA.HasValue && datiFondo.ServizioUtileAA.Value == diff.Year && datiFondo.ServizioUtileMM.HasValue && datiFondo.ServizioUtileMM.Value > diff.Month)
                        {
                            messaggioVideo = "Servizio utile MM deve essere minore o uguale a " + diff.Month;
                            return false;
                        }
                        if (!datiFondo.ControCodice.HasValue)
                        {
                            messaggioVideo = "Il Controcodice Retributivo è obbligatorio";
                            return false;
                        }

                        if (!GestioneControlli.CheckImportoWithControCodice(datiFondo.RetribuzionePensionabile, datiFondo.ControCodice, datiPensione, out messaggioVideo))
                            return false;

                        // Se presente codice ES/DZ il campo deve essere obbligatorimanente = a 0
                        if (datiFondo.fondoES != null && (datiFondo.fondoES.CodiceDz.HasValue && datiFondo.fondoES.CodiceDz.Value && datiFondo.fondoES.ClassePensioneAnte50 != 0))
                        {
                            messaggioVideo = "Classe Ante 50 : deve essere uguale a 0 se 'Codice ES/DZ' è presente";
                            return false;
                        }

                    }


                    break;

            }

            return true;
        }

        public static void GetListaTipoLiquidazioneGAS(out List<Entity.TipoLiquidazioneGAS> listaTipoLiquidazioneGAS)
        {
            listaTipoLiquidazioneGAS = new List<INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazioneGAS>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazioneGAS> listaTipoLiquidazioneGAS_DB = null;
            GestioneDecodifica.GetDecodificaTipoLiquidazioneGAS(out listaTipoLiquidazioneGAS_DB);
            if (listaTipoLiquidazioneGAS_DB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazioneGAS tipoLiquidazioneGAS_DB in listaTipoLiquidazioneGAS_DB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazioneGAS tipoLiquidazioneGAS = new INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazioneGAS();
                    Utility.ValorizzaOggetti(tipoLiquidazioneGAS_DB, tipoLiquidazioneGAS);
                    listaTipoLiquidazioneGAS.Add(tipoLiquidazioneGAS);
                }
            }
        }

        public static void GetListaTipoLiquidazionePI(out List<Entity.TipoLiquidazionePI> listaTipoLiquidazionePI)
        {
            listaTipoLiquidazionePI = new List<INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePI>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePI> listaTipoLiquidazionePI_DB = null;
            GestioneDecodifica.GetDecodificaTipoLiquidazionePI(out listaTipoLiquidazionePI_DB);
            if (listaTipoLiquidazionePI_DB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePI tipoLiquidazionePI_DB in listaTipoLiquidazionePI_DB)
                {
                    INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePI tipoLiquidazionePI = new INPS.Pensioni.LiquidazioneFs.Entity.TipoLiquidazionePI();
                    Utility.ValorizzaOggetti(tipoLiquidazionePI_DB, tipoLiquidazionePI);
                    listaTipoLiquidazionePI.Add(tipoLiquidazionePI);
                }
            }
        }

        public static void GetListaAttCon(char? codiceSpecificoTraduzioneSuGP, out List<Entity.AttCon> listaAttCon)
        {
            listaAttCon = new List<INPS.Pensioni.LiquidazioneFs.Entity.AttCon>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.AttCon> listaAttCon_DB = null;
            GestioneDecodifica.GetDecodificaAttCon(out listaAttCon_DB);
            if (listaAttCon_DB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.AttCon attCon_DB in listaAttCon_DB)
                {
                    if (attCon_DB.Id == '2' && codiceSpecificoTraduzioneSuGP != 'D')
                        continue;

                    INPS.Pensioni.LiquidazioneFs.Entity.AttCon attCon = new INPS.Pensioni.LiquidazioneFs.Entity.AttCon();
                    Utility.ValorizzaOggetti(attCon_DB, attCon);
                    listaAttCon.Add(attCon);
                }
            }
        }

        public static bool IsSettimane707Visible(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP, bool isQuotaDPresente)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            DateTime dataCompare;
            Utility.TipoCalcolo tipoCalcolo;

            switch (tipoFondo)
            {
                // Le settimane 707 sono visibili per tutti i fondi tranne PI (ex Dipendenti, Porto di Genova e Porto di Trieste) e CL
                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:
                case Utility.TipoFondo.CL:
                    break;
                default:
                    dataCompare = new DateTime(2012, 1, 1);
                    tipoCalcolo = Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
                    if (tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti && Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaIndirettaInabilitaLegge335(datiPensione))
                        return false;
                    if (tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti && datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, dataCompare) &&
                        codiceSpecificoTraduzioneSuGP != 'Q' && !(codiceSpecificoTraduzioneSuGP == 'F' && Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                    {
                        //Se non è presente la quota D, allora non si applica il comma 707
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && !isQuotaDPresente)
                            return false;

                        return true;
                    }
                    break;
                case Utility.TipoFondo.ET:
                    dataCompare = new DateTime(2012, 1, 2);
                    tipoCalcolo = Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
                    if (tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare) &&
                        codiceSpecificoTraduzioneSuGP != 'Q')
                    {
                        //Se non è presente la quota D, allora non si applica il comma 707
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && !isQuotaDPresente)
                            return false;

                        return true;
                    }
                    break;
            }

            return false;
        }

        public static bool ControlsDatiAgoAltraPensione(DatiAgoAltraPensione datiAgoAltraPensione, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiAgoAltraPensione == null)
            {
                messaggioVideo = "Valorizzare almeno un campo nel tab DatiCalcolo\\Altra Pensine - Dati AGO";
                return false;
            }
            if (!datiAgoAltraPensione.TipoLiquidazione.HasValue)
            {
                messaggioVideo = "Tipo Liquidazione è un dato obbligatorio.";
                return false;
            }
            if (!datiAgoAltraPensione.DecorrenzaAltraPensione.HasValue)
            {
                messaggioVideo = "Decorrenza Pensione è un dato obbligatorio.";
                return false;
            }
            if (datiAgoAltraPensione.CategoriaAltraPensione == null)
            {
                messaggioVideo = "Categoria è un dato obbligatorio.";
                return false;
            }
            if (!datiAgoAltraPensione.CertificatoAltraPensione.HasValue)
            {
                messaggioVideo = "Certificato è un dato obbligatorio.";
                return false;
            }

            switch (datiAgoAltraPensione.TipoLiquidazione)
            {
                case 1:
                    if (!datiAgoAltraPensione.BaseAltraPensione.HasValue)
                    {
                        messaggioVideo = string.Format("Per 'Tipo Liquidazione' {0} è obbligatorio il campo 'Base'.", datiAgoAltraPensione.TipoLiquidazione);
                        return false;
                    }
                    if (!datiAgoAltraPensione.RmsImpAltraPensione.HasValue)
                    {
                        messaggioVideo = string.Format("Per 'Tipo Liquidazione' {0} è obbligatorio il campo 'RMS/Imp'.", datiAgoAltraPensione.TipoLiquidazione);
                        return false;
                    }
                    break;
                case 2:
                    if (!datiAgoAltraPensione.BaseAltraPensione.HasValue)
                    {
                        messaggioVideo = string.Format("Per 'Tipo Liquidazione' {0} è obbligatorio il campo 'Base'.", datiAgoAltraPensione.TipoLiquidazione);
                        return false;
                    }
                    if (!datiAgoAltraPensione.RmsImpAltraPensione.HasValue)
                    {
                        messaggioVideo = string.Format("Per Tipo Liquidazione {0} è obbligatorio il campo 'Set. Anz. Tot'.", datiAgoAltraPensione.TipoLiquidazione);
                        return false;
                    }
                    break;
                case 5:
                case 7:
                    break;
                default:
                    messaggioVideo = "Il Tipo Liquidazione insertio non è ammesso.I valori ammessi sono 1, 2, 5, 7.";
                    return false;
            }
            if (datiAgoAltraPensione.DecorrenzaPrimoSupplemento.HasValue != datiAgoAltraPensione.ImpContribPrimoSupplemento.HasValue)
            {
                messaggioVideo = "La Decorrenza Primo Supplemento deve essere valorizzata se è stato inserito l'Importo del Primo Supplemento e viceversa.";
                return false;
            }
            if (datiAgoAltraPensione.DecorrenzaSecondoSupplemento.HasValue != datiAgoAltraPensione.ImpContribSecondoSupplemento.HasValue)
            {
                messaggioVideo = "La Decorrenza Secondo Supplemento deve essere valorizzata se è stato inserito l'Importo del Secondo Supplemento e viceversa.";
                return false;
            }
            if (datiAgoAltraPensione.RevAltraPensione.HasValue != datiAgoAltraPensione.RevAltraPensione > 100)
            {
                messaggioVideo = "La percentuale di reversibilità non può essere maggiore di 100";
                return false;
            }
            return true;
        }
        #endregion Dati Calcolo

        #region Dati Calcolo 707

        public static void GetDatiCalcolo707ByDomandaFelpe(GestionePensione.DatiPensione datiPensione, object dati, bool isCancelOperation,
            out Entity.DatiCalcolo707 datiCalcolo707ForDatiFondo, out string errori)
        {
            errori = string.Empty;
            datiCalcolo707ForDatiFondo = new Entity.DatiCalcolo707();
            CrossDataRecipient crossDataRecipient = null;
            List<GestioneCalcolo.ServizioUtile707> lServizioUtile707 = null;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            RecuperaDatiCustomFromFondi(datiPensione.Id, tipoFondo, datiPensione.Gruppo, datiPensione.IsPLUnicarpe, ref crossDataRecipient, datiPensione);

            //servizio utile         
            GestioneCalcolo.GetDatiServizioUtile707ByIdPensione(datiPensione.Id, out lServizioUtile707);

            if (lServizioUtile707 != null && lServizioUtile707.Count > 0)
            {
                datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<Entity.DatiCalcolo707.DatiServizioUtile707>();
                foreach (GestioneCalcolo.ServizioUtile707 servizioUtile707 in lServizioUtile707)
                {
                    Entity.DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new Entity.DatiCalcolo707.DatiServizioUtile707();
                    Utility.ValorizzaOggetti(servizioUtile707, datiServizioUtile707);
                    datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                }
            }

            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                case Utility.TipoFondo.PT:
                    if (crossDataRecipient != null)
                        datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = crossDataRecipient.PensioneAnnuaLorda707;
                    break;
            }

            csAggiornamentoPECO_Fondi_AMG datiAMG = null;
            if (dati != null && typeof(csAggiornamentoPECO_Fondi_AMG) == dati.GetType())
                datiAMG = (csAggiornamentoPECO_Fondi_AMG)dati;

            if (isCancelOperation && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                try
                {
                    GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref datiAMG, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        return;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }
            if (datiAMG != null)
            {
                try
                {
                    GestioneAggiornamentoPECO.RecuperaDatiTotaliAMG_FS_PT707(datiAMG, datiPensione, out lServizioUtile707, out crossDataRecipient);

                    if (lServizioUtile707 != null && lServizioUtile707.Count > 0)
                        datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<Entity.DatiCalcolo707.DatiServizioUtile707>();
                    foreach (GestioneCalcolo.ServizioUtile707 servizioUtile707 in lServizioUtile707)
                    {
                        Entity.DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new Entity.DatiCalcolo707.DatiServizioUtile707();
                        Utility.ValorizzaOggetti(servizioUtile707, datiServizioUtile707);
                        datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                    }

                    if (datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 == null && crossDataRecipient.PensioneAnnuaLorda707 != null)
                        datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = crossDataRecipient.PensioneAnnuaLorda707;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }
        }

        #endregion Dati Calcolo 707

        #endregion public members

        #region private members

        private static void RecuperaDatiCalcoloFromDB(GestioneCalcolo.DatiCalcoloContributivo datiContributivi, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, out GestioneAggiornamentoPECO.DatiRetributivi retribuzione, out GestioneAggiornamentoPECO.DatiContributivi contribuzione)
        {
            contribuzione = null;
            retribuzione = null;

            if (datiContributivi != null)
            {
                contribuzione = new GestioneAggiornamentoPECO.DatiContributivi();

                contribuzione.ImportoContributivoTotale = datiContributivi.ImportoContributivoTotale.HasValue ? datiContributivi.ImportoContributivoTotale.Value : 0M;
                contribuzione.Montante = datiContributivi.Montante.HasValue ? datiContributivi.Montante.Value : 0M;
                contribuzione.MontanteContributivo = datiContributivi.MontanteContributivo.HasValue ? datiContributivi.MontanteContributivo.Value : 0M;
                contribuzione.Settimane = datiContributivi.NSettimane.HasValue ? datiContributivi.NSettimane.Value : 0;
                contribuzione.MontanteQuotaDL214 = datiContributivi.MontanteQuotaDL214.HasValue ? datiContributivi.MontanteQuotaDL214.Value : 0M;
                contribuzione.ImportoContribTotaleQuotaDL214 = datiContributivi.ImportoContribTotaleQuotaDL214.HasValue ? datiContributivi.ImportoContribTotaleQuotaDL214.Value : 0M;
                contribuzione.NSettimaneQuotaDL214 = datiContributivi.NSettimaneQuotaDL214.HasValue ? datiContributivi.NSettimaneQuotaDL214.Value : 0;
                contribuzione.QuotaContributivaAnnua = datiContributivi.QuotaContributivaAnnua.HasValue ? datiContributivi.QuotaContributivaAnnua.Value : 0;
                contribuzione.AnzianitaPost0697AA = datiContributivi.AnzianitaPost0697AA.HasValue ? datiContributivi.AnzianitaPost0697AA.Value : (short)0;
                contribuzione.AnzianitaPost0697MM = datiContributivi.AnzianitaPost0697MM.HasValue ? datiContributivi.AnzianitaPost0697MM.Value : (short)0;
                contribuzione.AnzianitaPost0697GG = datiContributivi.AnzianitaPost0697GG.HasValue ? datiContributivi.AnzianitaPost0697GG.Value : (short)0;
                contribuzione.MontanteAnte0697 = datiContributivi.MontanteAnte0697.HasValue ? datiContributivi.MontanteAnte0697.Value : 0M;
                contribuzione.AnzianitaAnte0697AA = datiContributivi.AnzianitaAnte0697AA.HasValue ? datiContributivi.AnzianitaAnte0697AA.Value : (short)0;
                contribuzione.AnzianitaAnte0697MM = datiContributivi.AnzianitaAnte0697MM.HasValue ? datiContributivi.AnzianitaAnte0697MM.Value : (short)0;
                contribuzione.AnzianitaAnte0697GG = datiContributivi.AnzianitaAnte0697GG.HasValue ? datiContributivi.AnzianitaAnte0697GG.Value : (short)0;


                contribuzione.MontanteEsclusivo = datiContributivi.MontanteEsclusivo.HasValue ? datiContributivi.MontanteEsclusivo.Value : 0M;
                contribuzione.MontanteEsclusivoQuotaDL214 = datiContributivi.MontanteEsclusivoQuotaDL214.HasValue ? datiContributivi.MontanteEsclusivoQuotaDL214.Value : 0M;
            }
            if (datiRetributivi != null)
            {
                retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();

                retribuzione.RmsQuotaA = datiRetributivi.RMSQuotaA.HasValue ? datiRetributivi.RMSQuotaA.Value : 0M;
                retribuzione.RmsQuotaB = datiRetributivi.RMSQuotaB.HasValue ? datiRetributivi.RMSQuotaB.Value : 0M;
                retribuzione.RmsQuotaD = datiRetributivi.RMSQuotaD.HasValue ? datiRetributivi.RMSQuotaD.Value : 0M;
                retribuzione.SettimaneA = datiRetributivi.NSettimaneQuotaA;
                retribuzione.SettimaneB = datiRetributivi.NSettimaneQuotaB;
                retribuzione.SettimaneC = datiRetributivi.NSettimaneQuotaC;
                retribuzione.SettimaneD = datiRetributivi.NSettimaneQuotaD;
                retribuzione.RetribuzionePonderataAnnua = datiRetributivi.RetribuzionePonderataAnnua.HasValue ? datiRetributivi.RetribuzionePonderataAnnua.Value : 0M;
                retribuzione.SettimaneA2 = datiRetributivi.NSettimaneQuotaA2.HasValue ? datiRetributivi.NSettimaneQuotaA2.Value : 0;
                retribuzione.SettimaneC2 = datiRetributivi.NSettimaneQuotaC2.HasValue ? datiRetributivi.NSettimaneQuotaC2.Value : 0;

                retribuzione.NSettimaneEsclusiveQuotaA = datiRetributivi.NSettimaneEsclusiveQuotaA.HasValue ? datiRetributivi.NSettimaneEsclusiveQuotaA.Value : 0;
                retribuzione.NSettimaneEsclusiveQuotaB = datiRetributivi.NSettimaneEsclusiveQuotaB.HasValue ? datiRetributivi.NSettimaneEsclusiveQuotaB.Value : 0;
                retribuzione.NSettAnzianitaVV = datiRetributivi.NSettAnzianitaVV.HasValue ? datiRetributivi.NSettAnzianitaVV.Value : 0;
            }
        }

        private static void RecuperaDatiCalcoloFromWebForControls(DatiCalcolo datiCalcolo, Utility.TipoFondo? tipoFondo, out GestioneAggiornamentoPECO.DatiRetributivi retribuzione,
                                                                  out GestioneAggiornamentoPECO.DatiContributivi contribuzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            contribuzione = null;
            retribuzione = null;

            if (datiCalcolo != null)
            {
                switch (datiCalcolo.TipoCalcolo)
                {
                    case TipoCalcolo.Contributivo:
                        contribuzione = new GestioneAggiornamentoPECO.DatiContributivi();
                        contribuzione.ImportoContributivoTotale = datiCalcolo.ImportoContributivoTotale.HasValue ? datiCalcolo.ImportoContributivoTotale.Value : 0M;
                        contribuzione.Montante = datiCalcolo.Montante.HasValue ? datiCalcolo.Montante.Value : 0M;
                        contribuzione.Settimane = datiCalcolo.NSettimane.HasValue ? datiCalcolo.NSettimane.Value : 0;
                        contribuzione.MontanteContributivo = datiCalcolo.MontanteContributivo.HasValue ? datiCalcolo.MontanteContributivo.Value : 0M;
                        contribuzione.ImportoContribTotaleQuotaDL214 = datiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcolo.ImportoContribTotaleQuotaDL214.Value : 0M;
                        contribuzione.MontanteQuotaDL214 = datiCalcolo.MontanteQuotaDL214.HasValue ? datiCalcolo.MontanteQuotaDL214.Value : 0M;
                        contribuzione.NSettimaneQuotaDL214 = datiCalcolo.NSettimaneQuotaDL214.HasValue ? datiCalcolo.NSettimaneQuotaDL214.Value : 0;
                        contribuzione.QuotaContributivaAnnua = datiCalcolo.QuotaContributivaAnnua.HasValue ? datiCalcolo.QuotaContributivaAnnua.Value : 0M;

                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                        {
                            contribuzione.AnzianitaPost0697AA = datiCalcolo.AnzianitaPost0697AA.HasValue ? datiCalcolo.AnzianitaPost0697AA.Value : (short)0;
                            contribuzione.AnzianitaPost0697MM = datiCalcolo.AnzianitaPost0697MM.HasValue ? datiCalcolo.AnzianitaPost0697MM.Value : (short)0;
                            contribuzione.AnzianitaPost0697GG = datiCalcolo.AnzianitaPost0697GG.HasValue ? datiCalcolo.AnzianitaPost0697GG.Value : (short)0;
                            contribuzione.MontanteAnte0697 = datiCalcolo.MontanteAnte0697.HasValue ? datiCalcolo.MontanteAnte0697.Value : 0M;
                            contribuzione.AnzianitaAnte0697AA = datiCalcolo.AnzianitaAnte0697AA.HasValue ? datiCalcolo.AnzianitaAnte0697AA.Value : (short)0;
                            contribuzione.AnzianitaAnte0697MM = datiCalcolo.AnzianitaAnte0697MM.HasValue ? datiCalcolo.AnzianitaAnte0697MM.Value : (short)0;
                            contribuzione.AnzianitaAnte0697GG = datiCalcolo.AnzianitaAnte0697GG.HasValue ? datiCalcolo.AnzianitaAnte0697GG.Value : (short)0;
                        }

                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.GAS)
                        {
                            contribuzione.MontanteEsclusivo = datiCalcolo.MontanteEsclusivo.HasValue ? datiCalcolo.MontanteEsclusivo.Value : 0M;
                            contribuzione.MontanteEsclusivoQuotaDL214 = datiCalcolo.MontanteEsclusivoQuotaDL214.HasValue ? datiCalcolo.MontanteEsclusivoQuotaDL214.Value : 0M;
                        }
                        break;

                    case TipoCalcolo.Retributivo:
                        retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                        retribuzione.RmsQuotaA = datiCalcolo.RMSQuotaA.HasValue ? datiCalcolo.RMSQuotaA.Value : 0M;
                        retribuzione.RmsQuotaB = datiCalcolo.RMSQuotaB.HasValue ? datiCalcolo.RMSQuotaB.Value : 0M;
                        retribuzione.RmsQuotaD = datiCalcolo.RMSQuotaD.HasValue ? datiCalcolo.RMSQuotaD.Value : 0M;
                        retribuzione.SettimaneA = datiCalcolo.NSettimaneQuotaA;
                        retribuzione.SettimaneB = datiCalcolo.NSettimaneQuotaB;
                        retribuzione.SettimaneC = datiCalcolo.NSettimaneQuotaC;
                        retribuzione.SettimaneD = datiCalcolo.NSettimaneQuotaD;
                        retribuzione.RetribuzionePonderataAnnua = datiCalcolo.RetribuzionePonderataAnnua.HasValue ? datiCalcolo.RetribuzionePonderataAnnua.Value : 0M;

                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                        {
                            retribuzione.SettimaneA2 = datiCalcolo.NSettimaneQuotaA2.HasValue ? datiCalcolo.NSettimaneQuotaA2.Value : 0;
                            retribuzione.SettimaneC2 = datiCalcolo.NSettimaneQuotaC2.HasValue ? datiCalcolo.NSettimaneQuotaC2.Value : 0;
                        }
                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ES)
                        {
                            retribuzione.NSettAnzianitaVV = datiCalcolo.NSettAnzianitaVV ?? 0;
                        }

                        break;

                    case TipoCalcolo.Misto:
                    case TipoCalcolo.RetributivoMonti:
                        contribuzione = new GestioneAggiornamentoPECO.DatiContributivi();
                        contribuzione.ImportoContributivoTotale = datiCalcolo.ImportoContributivoTotale.HasValue ? datiCalcolo.ImportoContributivoTotale.Value : 0M;
                        contribuzione.Montante = datiCalcolo.Montante.HasValue ? datiCalcolo.Montante.Value : 0M;
                        contribuzione.MontanteContributivo = datiCalcolo.MontanteContributivo.HasValue ? datiCalcolo.MontanteContributivo.Value : 0M;
                        contribuzione.Settimane = datiCalcolo.NSettimane.HasValue ? datiCalcolo.NSettimane.Value : 0;
                        contribuzione.ImportoContribTotaleQuotaDL214 = datiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcolo.ImportoContribTotaleQuotaDL214.Value : 0M;
                        contribuzione.MontanteQuotaDL214 = datiCalcolo.MontanteQuotaDL214.HasValue ? datiCalcolo.MontanteQuotaDL214.Value : 0M;
                        contribuzione.NSettimaneQuotaDL214 = datiCalcolo.NSettimaneQuotaDL214.HasValue ? datiCalcolo.NSettimaneQuotaDL214.Value : 0;
                        contribuzione.QuotaContributivaAnnua = datiCalcolo.QuotaContributivaAnnua.HasValue ? datiCalcolo.QuotaContributivaAnnua.Value : 0M;

                        retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                        retribuzione.RmsQuotaA = datiCalcolo.RMSQuotaA.HasValue ? datiCalcolo.RMSQuotaA.Value : 0M;
                        retribuzione.RmsQuotaB = datiCalcolo.RMSQuotaB.HasValue ? datiCalcolo.RMSQuotaB.Value : 0M;
                        retribuzione.RmsQuotaD = datiCalcolo.RMSQuotaD.HasValue ? datiCalcolo.RMSQuotaD.Value : 0M;
                        retribuzione.SettimaneA = datiCalcolo.NSettimaneQuotaA;
                        retribuzione.SettimaneB = datiCalcolo.NSettimaneQuotaB;
                        retribuzione.SettimaneC = datiCalcolo.NSettimaneQuotaC;
                        retribuzione.SettimaneD = datiCalcolo.NSettimaneQuotaD;
                        retribuzione.RetribuzionePonderataAnnua = datiCalcolo.RetribuzionePonderataAnnua.HasValue ? datiCalcolo.RetribuzionePonderataAnnua.Value : 0M;

                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.VL)
                        {
                            contribuzione.AnzianitaPost0697AA = datiCalcolo.AnzianitaPost0697AA.HasValue ? datiCalcolo.AnzianitaPost0697AA.Value : (short)0;
                            contribuzione.AnzianitaPost0697MM = datiCalcolo.AnzianitaPost0697MM.HasValue ? datiCalcolo.AnzianitaPost0697MM.Value : (short)0;
                            contribuzione.AnzianitaPost0697GG = datiCalcolo.AnzianitaPost0697GG.HasValue ? datiCalcolo.AnzianitaPost0697GG.Value : (short)0;
                            contribuzione.MontanteAnte0697 = datiCalcolo.MontanteAnte0697.HasValue ? datiCalcolo.MontanteAnte0697.Value : 0M;
                            contribuzione.AnzianitaAnte0697AA = datiCalcolo.AnzianitaAnte0697AA.HasValue ? datiCalcolo.AnzianitaAnte0697AA.Value : (short)0;
                            contribuzione.AnzianitaAnte0697MM = datiCalcolo.AnzianitaAnte0697MM.HasValue ? datiCalcolo.AnzianitaAnte0697MM.Value : (short)0;
                            contribuzione.AnzianitaAnte0697GG = datiCalcolo.AnzianitaAnte0697GG.HasValue ? datiCalcolo.AnzianitaAnte0697GG.Value : (short)0;
                            retribuzione.SettimaneA2 = datiCalcolo.NSettimaneQuotaA2.HasValue ? datiCalcolo.NSettimaneQuotaA2.Value : 0;
                            retribuzione.SettimaneC2 = datiCalcolo.NSettimaneQuotaC2.HasValue ? datiCalcolo.NSettimaneQuotaC2.Value : 0;
                        }

                        if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.GAS)
                        {
                            contribuzione.MontanteEsclusivo = datiCalcolo.MontanteEsclusivo.HasValue ? datiCalcolo.MontanteEsclusivo.Value : 0M;
                            contribuzione.MontanteEsclusivoQuotaDL214 = datiCalcolo.MontanteEsclusivoQuotaDL214.HasValue ? datiCalcolo.MontanteEsclusivoQuotaDL214.Value : 0M;
                        }

                        break;

                    case TipoCalcolo.NonValido:
                        messaggioVideo = "E' necessario inserire almeno un dato contributivo e/o retributivo.";
                        return;
                    default:
                        return;
                }
            }
        }

        private static void RecuperaDatiCalcoloFromWebForStore(long IdPensione, DatiCalcolo datiCalcolo, out GestioneCalcolo.DatiCalcoloContributivo datiContributivi, out GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi)
        {
            datiContributivi = new GestioneCalcolo.DatiCalcoloContributivo();
            datiContributivi.ImportoContributivoTotale = datiCalcolo.ImportoContributivoTotale.HasValue ? datiCalcolo.ImportoContributivoTotale.Value : (decimal?)null;
            datiContributivi.Montante = datiCalcolo.Montante.HasValue ? datiCalcolo.Montante.Value : (decimal?)null;
            datiContributivi.IdPensione = IdPensione;
            datiContributivi.NSettimane = datiCalcolo.NSettimane.HasValue ? datiCalcolo.NSettimane.Value : (int?)null;
            datiContributivi.AnzianitaPost0697AA = datiCalcolo.AnzianitaPost0697AA.HasValue ? datiCalcolo.AnzianitaPost0697AA.Value : (short?)null;
            datiContributivi.AnzianitaPost0697MM = datiCalcolo.AnzianitaPost0697MM.HasValue ? datiCalcolo.AnzianitaPost0697MM.Value : (short?)null;
            datiContributivi.AnzianitaPost0697GG = datiCalcolo.AnzianitaPost0697GG.HasValue ? datiCalcolo.AnzianitaPost0697GG.Value : (short?)null;
            datiContributivi.MontanteAnte0697 = datiCalcolo.MontanteAnte0697.HasValue ? datiCalcolo.MontanteAnte0697.Value : (decimal?)null;
            datiContributivi.MontanteQuotaDL214 = datiCalcolo.MontanteQuotaDL214.HasValue ? datiCalcolo.MontanteQuotaDL214.Value : (decimal?)null;
            datiContributivi.ImportoContribTotaleQuotaDL214 = datiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? datiCalcolo.ImportoContribTotaleQuotaDL214.Value : (decimal?)null;
            datiContributivi.NSettimaneQuotaDL214 = datiCalcolo.NSettimaneQuotaDL214.HasValue ? datiCalcolo.NSettimaneQuotaDL214.Value : (int?)null;
            datiContributivi.MontanteContributivo = datiCalcolo.MontanteContributivo.HasValue ? datiCalcolo.MontanteContributivo.Value : (decimal?)null;


            datiRetributivi = new GestioneCalcolo.DatiCalcoloRetributivo();
            datiRetributivi.IdPensione = IdPensione;
            datiRetributivi.RMSQuotaA = datiCalcolo.RMSQuotaA.HasValue ? datiCalcolo.RMSQuotaA.Value : (decimal?)null;
            datiRetributivi.RMSQuotaB = datiCalcolo.RMSQuotaB.HasValue ? datiCalcolo.RMSQuotaB.Value : (decimal?)null;
            datiRetributivi.RMSQuotaD = datiCalcolo.RMSQuotaD.HasValue ? datiCalcolo.RMSQuotaD.Value : (decimal?)null;
            datiRetributivi.NSettimaneQuotaA = datiCalcolo.NSettimaneQuotaA.HasValue ? datiCalcolo.NSettimaneQuotaA.Value : (int?)null;
            datiRetributivi.NSettimaneQuotaB = datiCalcolo.NSettimaneQuotaB.HasValue ? datiCalcolo.NSettimaneQuotaB.Value : (int?)null;
            datiRetributivi.NSettimaneQuotaC = datiCalcolo.NSettimaneQuotaC.HasValue ? datiCalcolo.NSettimaneQuotaC.Value : (int?)null;
            datiRetributivi.NSettimaneQuotaD = datiCalcolo.NSettimaneQuotaD.HasValue ? datiCalcolo.NSettimaneQuotaD.Value : (int?)null;
            datiRetributivi.RetribuzionePonderataAnnua = datiCalcolo.RetribuzionePonderataAnnua.HasValue ? datiCalcolo.RetribuzionePonderataAnnua.Value : (decimal?)null;
            datiRetributivi.NSettimaneQuotaA2 = datiCalcolo.NSettimaneQuotaA2.HasValue ? datiCalcolo.NSettimaneQuotaA2.Value : (int?)null;
            datiRetributivi.NSettimaneQuotaC2 = datiCalcolo.NSettimaneQuotaC2.HasValue ? datiCalcolo.NSettimaneQuotaC2.Value : (int?)null;
        }

        private static void SalvaDatiCalcoloWithFondi(DatiCalcolo datiCalcolo, object Fondo, ref GestioneFondo.DatiFondo datiFondoBl, GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Utility.TipoFondo? tipoFondo)
        {
            List<DatiServizioUtile> lDatiServizioUtile = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    #region Fondo EL
                    case Utility.TipoFondo.EL:
                        List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileBl = null;
                        if (datiCalcolo.fondoEL != null && (datiCalcolo.fondoEL.LServizioUtile != null && datiCalcolo.fondoEL.LServizioUtile.Count > 0))
                        {
                            //gestione dati servizio utile
                            if (datiCalcolo.fondoEL.LServizioUtile != null && datiCalcolo.fondoEL.LServizioUtile.Count > 0)
                            {
                                lServizioUtileBl = datiCalcolo.fondoEL.LServizioUtile.Select(x =>
                                {
                                    var y = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(x, y);
                                    return y;
                                }).ToList();
                            }
                        }

                        if (datiCalcolo.fondoEL != null && datiCalcolo.fondoEL.RetrPondAnnuaAGOLimite.HasValue)
                        {
                            if (datiFondoBl == null)
                                datiFondoBl = new GestioneFondo.DatiFondo();
                            //Retribuzione ponderata annua AGO limite
                            datiFondoBl.RetrPondAnnuaAGOLimite = datiCalcolo.fondoEL.RetrPondAnnuaAGOLimite;
                        }

                        if (datiFondoBl.IsFondoNull() && lServizioUtileBl == null)
                        {
                            GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                        }
                        else
                        {
                            if (datiFondoBl == null)
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            long idFondo = datiFondoBl.Id;
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (lServizioUtileBl != null && lServizioUtileBl.Count > 0)
                            {
                                lServizioUtileBl.ForEach(x => GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, x));
                            }
                        }
                        break;

                    #endregion Fondo EL

                    #region Fondo TT

                    case Utility.TipoFondo.TT:

                        if (datiCalcolo.fondoTT == null)
                            datiCalcolo.fondoTT = new FondoTT();

                        GestioneFondo.DatiFondoTT DatiFondoTT = (GestioneFondo.DatiFondoTT)Fondo;
                        if (DatiFondoTT == null)
                            DatiFondoTT = new GestioneFondo.DatiFondoTT();
                        else if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                        {
                            datiCalcolo.fondoTT.RetribuzioneBiennio = DatiFondoTT.RetribuzioneBiennio;
                            datiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA = DatiFondoTT.RetribuzioneUltimoAnnoQuotaA;
                        }
                        Utility.ValorizzaOggetti(datiCalcolo.fondoTT, DatiFondoTT);

                        if (datiCalcolo.fondoTT != null && datiCalcolo.fondoTT.RetrPondAnnuaAGOLimite.HasValue)
                        {
                            if (datiFondoBl == null)
                                datiFondoBl = new GestioneFondo.DatiFondo();
                            //Retribuzione ponderata annua AGO limite
                            datiFondoBl.RetrPondAnnuaAGOLimite = datiCalcolo.fondoTT.RetrPondAnnuaAGOLimite;
                        }

                        if (!DatiFondoTT.Equals(new GestioneFondo.DatiFondoTT()) ||
                            (datiCalcolo.fondoTT != null && datiCalcolo.fondoTT.lDatiServizioUtile != null && datiCalcolo.fondoTT.lDatiServizioUtile.Count > 0) ||
                            datiFondoBl != null)    // fondo XX not null
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoTT(datiFondoBl.Id, DatiFondoTT);

                            if ((datiCalcolo.fondoTT.lDatiServizioUtile != null && datiCalcolo.fondoTT.lDatiServizioUtile.Count > 0))
                            {
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                lDatiServizioUtile = datiCalcolo.fondoTT.lDatiServizioUtile;
                                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoTT(datiPensione.Id);
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo TT

                    #region Fondo VL

                    case Utility.TipoFondo.VL:

                        if (datiCalcolo.fondoVL == null)
                            datiCalcolo.fondoVL = new FondoVL();

                        GestioneFondo.DatiFondoVL DatiFondoVL = (GestioneFondo.DatiFondoVL)Fondo;
                        if (DatiFondoVL == null)
                            DatiFondoVL = new GestioneFondo.DatiFondoVL();
                        Utility.ValorizzaOggetti(datiCalcolo.fondoVL, DatiFondoVL);

                        if (!DatiFondoVL.Equals(new GestioneFondo.DatiFondoVL()) || (datiCalcolo.fondoVL.LServizioUtile != null && datiCalcolo.fondoVL.LServizioUtile.Count > 0))
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoVL(datiFondoBl.Id, DatiFondoVL);

                            if ((datiCalcolo.fondoVL.LServizioUtile != null && datiCalcolo.fondoVL.LServizioUtile.Count > 0))
                            {
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                lDatiServizioUtile = datiCalcolo.fondoVL.LServizioUtile;
                                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoVL(datiPensione.Id);
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo VL

                    #region Fondo ET
                    case Utility.TipoFondo.ET:

                        if (datiFondoBl != null && !datiFondoBl.IsFondoNull())
                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);

                        if (datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null && datiCalcolo.fondoET.lDatiServizioUtile.Count > 0)
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                            {
                                datiFondoBl = new GestioneFondo.DatiFondo();
                                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            }
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            lDatiServizioUtile = datiCalcolo.fondoET.lDatiServizioUtile;
                            foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                            {
                                GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                            }
                        }
                        else
                        {
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo ET

                    #region Fondo PT
                    case Utility.TipoFondo.PT:

                        if (datiCalcolo.fondoPT == null)
                            datiCalcolo.fondoPT = new FondoPT();

                        GestioneFondo.DatiFondoPT DatiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                        if (DatiFondoPT == null)
                            DatiFondoPT = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiCalcolo.fondoPT, DatiFondoPT);

                        if (!DatiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoPT(datiFondoBl.Id, DatiFondoPT);

                            if (datiCalcolo.fondoPT.lDatiServizioUtile != null && datiCalcolo.fondoPT.lDatiServizioUtile.Count > 0)
                            {
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                lDatiServizioUtile = datiCalcolo.fondoPT.lDatiServizioUtile;
                                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoPT(datiPensione.Id);
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo PT

                    #region Fondo FS
                    case Utility.TipoFondo.FS:

                        if (datiCalcolo.fondoFST == null)
                            datiCalcolo.fondoFST = new FondoFST();

                        GestioneFondo.DatiFondoFST DatiFondoFST = (GestioneFondo.DatiFondoFST)Fondo;
                        if (DatiFondoFST == null)
                            DatiFondoFST = new GestioneFondo.DatiFondoFST();

                        Utility.ValorizzaOggetti(datiCalcolo.fondoFST, DatiFondoFST);

                        if (datiMaggiorazioniBenefici == null)
                        {
                            datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                            datiMaggiorazioniBenefici.IdPensione = datiPensione.Id;
                        }
                        datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = datiCalcolo.fondoFST.RMSSenzaLegge33670QA;
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiBLCommon = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                        Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiMaggiorazioniBeneficiBLCommon);

                        if (!DatiFondoFST.Equals(new GestioneFondo.DatiFondoFST()))
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoFST(datiFondoBl.Id, DatiFondoFST);
                            if (datiCalcolo.fondoFST.lDatiServizioUtile != null && datiCalcolo.fondoFST.lDatiServizioUtile.Count > 0)
                            {
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                lDatiServizioUtile = datiCalcolo.fondoFST.lDatiServizioUtile;
                                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }

                            if (!Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiBLCommon))
                                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiBLCommon);
                            else
                                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoFST(datiPensione.Id);
                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo FS

                    #region Fondo GAS
                    case Utility.TipoFondo.GAS:

                        if (datiCalcolo.fondoGAS != null)
                        {
                            GestioneFondo.DatiFondoGAS datiFondoGAS = (GestioneFondo.DatiFondoGAS)Fondo;
                            if (datiFondoGAS == null)
                                datiFondoGAS = new GestioneFondo.DatiFondoGAS();

                            Utility.ValorizzaOggetti(datiCalcolo.fondoGAS, datiFondoGAS);
                            if (!datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))    // fondo XX not null
                            {
                                if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                    datiFondoBl = new GestioneFondo.DatiFondo();

                                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                                GestioneFondo.SalvaFondoGAS(datiFondoBl.Id, datiFondoGAS);
                            }
                            else
                            {
                                GestioneFondo.EliminaFondoGAS(datiPensione.Id);
                                if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                    GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            }
                        }
                        break;
                    #endregion Fondo GAS

                    #region Fondo DZ
                    case Utility.TipoFondo.DZ:
                        if (datiCalcolo.fondoDZ != null)
                        {
                            GestioneFondo.DatiFondoDZ datiFondoDZ = (GestioneFondo.DatiFondoDZ)Fondo;
                            if (datiFondoDZ == null)
                                datiFondoDZ = new GestioneFondo.DatiFondoDZ();

                            Utility.ValorizzaOggetti(datiCalcolo.fondoDZ, datiFondoDZ);
                            if (!datiFondoDZ.Equals(new GestioneFondo.DatiFondoDZ()))    // fondo XX not null
                            {
                                if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                    datiFondoBl = new GestioneFondo.DatiFondo();

                                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                                GestioneFondo.SalvaFondoDZ(datiFondoBl.Id, datiFondoDZ);

                                if (datiCalcolo.fondoDZ.lDatiServizioUtile != null && datiCalcolo.fondoDZ.lDatiServizioUtile.Count > 0)
                                {
                                    GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                                    lDatiServizioUtile = datiCalcolo.fondoDZ.lDatiServizioUtile;
                                    foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                                    {
                                        GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon = new GestioneDatiServizioUtile.ServizioUtile();
                                        Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                        GestioneDatiServizioUtile.SalvaDatiServizioUtile(datiFondoBl.Id, servizioUtileCommon);
                                    }
                                }
                            }
                            else
                            {
                                GestioneFondo.EliminaFondoDZ(datiPensione.Id);
                                if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                    GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            }
                        }
                        break;
                    #endregion Fondo DZ

                    #region Fondo ES
                    case Utility.TipoFondo.ES:
                        if (datiCalcolo.fondoES != null)
                        {
                            GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)Fondo;
                            if (datiFondoES == null)
                                datiFondoES = new GestioneFondo.DatiFondoES();

                            Utility.ValorizzaOggetti(datiCalcolo.fondoES, datiFondoES);
                            if (!datiFondoES.Equals(new GestioneFondo.DatiFondoES()))    // fondo XX not null
                            {
                                if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                    datiFondoBl = new GestioneFondo.DatiFondo();

                                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                                GestioneFondo.SalvaFondoES(datiFondoBl.Id, datiFondoES);
                            }
                            else
                            {
                                GestioneFondo.EliminaFondoES(datiPensione.Id);
                                if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                    GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            }
                        }
                        break;

                    #endregion Fondo ES

                    #region Fondo PI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        if (datiCalcolo.fondoPI != null)
                        {
                            GestioneFondo.DatiFondoPI datiFondoPI = (GestioneFondo.DatiFondoPI)Fondo;
                            if (datiFondoPI == null)
                                datiFondoPI = new GestioneFondo.DatiFondoPI();

                            Utility.ValorizzaOggetti(datiCalcolo.fondoPI, datiFondoPI);
                            if (!datiFondoPI.Equals(new GestioneFondo.DatiFondoPI()))    // fondo XX not null
                            {
                                if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                    datiFondoBl = new GestioneFondo.DatiFondo();

                                GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                                //cambiato meotdo di salvataggio
                                GestioneFondo.SalvaFondoPIRecordFondo(datiFondoBl.Id, datiFondoPI.IdRecordFondo, datiFondoPI);
                            }
                            else
                            {
                                GestioneFondo.EliminaFondoPI(datiPensione.Id);
                                if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                    GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                            }
                        }
                        break;
                    #endregion Fondo PI
                    #region Fondo PM
                    case Utility.TipoFondo.PM:
                        if (datiFondoBl != null)
                        {
                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                        }
                        break;
                        #endregion Fondo PM
                }
            }
        }

        private static void SalvaDatiCalcolo707WithFondi(Entity.DatiCalcolo707 datiCalcolo707, object Fondo, ref GestioneFondo.DatiFondo datiFondoBl, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo)
        {
            List<Entity.DatiCalcolo707.DatiServizioUtile707> lDatiServizioUtile707 = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    #region Fondo PT
                    case Utility.TipoFondo.PT:

                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                        if (datiFondoPT == null)
                            datiFondoPT = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiCalcolo707, datiFondoPT);

                        if (!datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoPT(datiFondoBl.Id, datiFondoPT);

                            if (datiCalcolo707.LDatiServizioUtile707 != null && datiCalcolo707.LDatiServizioUtile707.Count > 0)
                            {
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                                lDatiServizioUtile707 = datiCalcolo707.LDatiServizioUtile707;
                                foreach (Entity.DatiCalcolo707.DatiServizioUtile707 servizioUtile in lDatiServizioUtile707)
                                {
                                    GestioneCalcolo.ServizioUtile707 servizioUtileCommon = new GestioneCalcolo.ServizioUtile707();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneCalcolo.SalvaDatiServizioUtile707(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoPT(datiPensione.Id);
                            GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                    #endregion Fondo PT

                    #region Fondo FS
                    case Utility.TipoFondo.FS:

                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)Fondo;
                        if (datiFondoFST == null)
                            datiFondoFST = new GestioneFondo.DatiFondoFST();
                        Utility.ValorizzaOggetti(datiCalcolo707, datiFondoFST);

                        if (!datiFondoFST.Equals(new GestioneFondo.DatiFondoFST()))
                        {
                            if (datiFondoBl == null || datiFondoBl.IsFondoNull())
                                datiFondoBl = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondoBl);
                            GestioneFondo.SalvaFondoFST(datiFondoBl.Id, datiFondoFST);
                            if (datiCalcolo707.LDatiServizioUtile707 != null && datiCalcolo707.LDatiServizioUtile707.Count > 0)
                            {
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                                lDatiServizioUtile707 = datiCalcolo707.LDatiServizioUtile707;
                                foreach (Entity.DatiCalcolo707.DatiServizioUtile707 servizioUtile in lDatiServizioUtile707)
                                {
                                    GestioneCalcolo.ServizioUtile707 servizioUtileCommon = new GestioneCalcolo.ServizioUtile707();
                                    Utility.ValorizzaOggetti(servizioUtile, servizioUtileCommon);
                                    GestioneCalcolo.SalvaDatiServizioUtile707(datiFondoBl.Id, servizioUtileCommon);
                                }
                            }
                        }
                        else
                        {
                            GestioneFondo.EliminaFondoFST(datiPensione.Id);
                            GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                            if (datiFondoBl != null && datiFondoBl.IsFondoNull())
                                GestioneFondo.EliminaFondoDatiGenerici(datiPensione.Id);
                        }
                        break;
                        #endregion Fondo FS
                }
            }
        }

        private static void ControlsDatiCalcoloCross(DatiCalcolo datiCalcolo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggBenefici, object datiFondoXX, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DatiArt11e14 entityDatiArt11e14, char? codiceSpecificoTraduzioneSuGP, DateTime dataSistema,
            bool isRiaperturaDomanda, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.ValorizzaOggetti(datiCalcolo, datiFondo);
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            DateTime? decorrenzaPensione = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (datiFondo == null)
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Dati Fondo Generici obbligatori.";
                return;
            }

            Utility.TipoCalcolo tipocalcolo = (Utility.TipoCalcolo)datiCalcolo.TipoCalcolo;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    #region Fondo EL
                    case Utility.TipoFondo.EL:
                        if (!ControlsSettimaneUtiliDiritto(datiCalcolo, out messaggioVideo, datiPensione))
                            return;

                        ControlsRMSWithSettimaneEL_TT(datiCalcolo, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;

                        if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensione, datiFondo: datiFondo))
                        {
                            if (datiCalcolo.fondoEL != null)
                            {
                                //controlli di correttezza sui dati
                                if (!ControlsDatiServizioUtile(datiCalcolo.fondoEL.LServizioUtile, out messaggioVideo))
                                    return;
                                //controlli specifici per settimane 
                                if (!ControlsDatiServizioUtileAnteArmonizzazioneWithAssicurazione(datiCalcolo, datiFondo, datiCalcolo.fondoEL.LServizioUtile, datiPensione, datiFondoXX, codiceSpecificoTraduzioneSuGP,
                                    datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null,
                                    datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                    return;
                            }
                        }
                        break;
                    #endregion Fondo EL

                    #region Fondo TT
                    case Utility.TipoFondo.TT:
                        if (!ControlsSettimaneUtiliDiritto(datiCalcolo, out messaggioVideo, datiPensione))
                            return;

                        ControlsRMSWithSettimaneEL_TT(datiCalcolo, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;

                        if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensione, datiFondoXX: datiFondoXX))
                        {
                            if (datiCalcolo.fondoTT != null)
                            {
                                Utility.ValorizzaOggetti(datiCalcolo.fondoTT, datiFondoXX);

                                //controlli di correttezza sui dati
                                if (!ControlsDatiServizioUtile(datiCalcolo.fondoTT.lDatiServizioUtile, out messaggioVideo))
                                {
                                    if (messaggioVideo.Contains("(Quota A)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota A)", "(Quota Ante 01/01/93)");
                                    else if (messaggioVideo.Contains("(Quota A2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota A2)", "(Quota Ante 01/01/93 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota B)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota B)", "(Quota Post 31/12/92)");
                                    else if (messaggioVideo.Contains("(Quota B2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota B2)", "(Quota Post 31/12/92 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota C)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota C)", "(Quota Post 31/12/94)");
                                    else if (messaggioVideo.Contains("(Quota C2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota C2)", "(Quota Post 31/12/94 Ridotto)");
                                    else if (messaggioVideo.Contains("(Quota D)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota D)", "(Quota Post 31/12/96)");
                                    else if (messaggioVideo.Contains("(Quota D2)"))
                                        messaggioVideo = messaggioVideo.Replace("(Quota D2)", "(Quota Post 31/12/96 Ridotto)");

                                    return;
                                }
                                //controlli specifici per settimane 
                                if (!ControlsDatiServizioUtileAnteArmonizzazioneWithAssicurazione(datiCalcolo, datiFondo, datiCalcolo.fondoTT.lDatiServizioUtile, datiPensione, datiFondoXX, codiceSpecificoTraduzioneSuGP,
                                    datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null,
                                    datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                    return;
                            }
                        }
                        break;
                    #endregion Fondo TT

                    #region Fondo VL

                    case Utility.TipoFondo.VL:
                        if (!ControlsSettimaneUtiliDiritto(datiCalcolo, out messaggioVideo, datiPensione))
                            return;

                        if (!GestioneControlli.IsValoreAAMMGGValido(datiCalcolo.AnzianitaAnte0697AA, null, null))//datiCalcolo.AnzianitaAnte0697AA.HasValue && datiCalcolo.AnzianitaAnte0697AA.Value.ToString().Length > 2)
                        {
                            messaggioVideo = "Anzianità a(da 01/96 a 06/97) deve essere compreso tra 0 e 99";
                            break;
                        }

                        if (!GestioneControlli.IsValoreAAMMGGValido(null, datiCalcolo.AnzianitaAnte0697MM, null))//datiCalcolo.AnzianitaAnte0697MM.HasValue && (datiCalcolo.AnzianitaAnte0697MM.Value.ToString().Length > 2 || datiCalcolo.AnzianitaAnte0697MM.Value > 11))
                        {
                            messaggioVideo = "Anzianità m(da 01/96 a 06/97) deve essere compreso tra 0 e 11";
                            break;
                        }

                        if (!GestioneControlli.IsValoreAAMMGGValido(null, null, datiCalcolo.AnzianitaAnte0697GG))//datiCalcolo.AnzianitaAnte0697GG.HasValue && (datiCalcolo.AnzianitaAnte0697GG.Value.ToString().Length > 2 || datiCalcolo.AnzianitaAnte0697GG.Value > 30))
                        {
                            messaggioVideo = "Anzianità g(da 01/96 a 06/97) deve essere compreso tra 0 e 29";
                            break;
                        }

                        if (!GestioneControlli.IsValoreAAMMGGValido(datiCalcolo.AnzianitaPost0697AA, null, null))//datiCalcolo.AnzianitaPost0697AA.HasValue && datiCalcolo.AnzianitaPost0697AA.Value.ToString().Length > 2)
                        {
                            messaggioVideo = "Anzianità a(da 07/97) deve essere compreso tra 0 e 99";
                            break;
                        }

                        if (!GestioneControlli.IsValoreAAMMGGValido(null, datiCalcolo.AnzianitaPost0697MM, null))//datiCalcolo.AnzianitaPost0697MM.HasValue && (datiCalcolo.AnzianitaPost0697MM.Value.ToString().Length > 2 || datiCalcolo.AnzianitaPost0697MM.Value > 11))
                        {
                            messaggioVideo = "Anzianità m(da 07/97) deve essere compreso tra 0 e 11";
                            break;
                        }

                        if (!GestioneControlli.IsValoreAAMMGGValido(null, null, datiCalcolo.AnzianitaPost0697GG))//datiCalcolo.AnzianitaPost0697GG.HasValue && (datiCalcolo.AnzianitaPost0697GG.Value.ToString().Length > 2 || datiCalcolo.AnzianitaPost0697GG.Value > 30))
                        {
                            messaggioVideo = "Anzianità g(da 07/97) deve essere compreso tra 0 e 29";
                            break;
                        }

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;

                        if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensione))
                        {
                            if (datiCalcolo.fondoVL != null)
                            {
                                //controlli di correttezza sui dati
                                if (!ControlsDatiServizioUtile(datiCalcolo.fondoVL.LServizioUtile, out messaggioVideo))
                                    return;

                                //controlli specifici per settimane 
                                if (!ControlsDatiServizioUtileAnteArmonizzazioneWithAssicurazione(datiCalcolo, datiFondo, datiCalcolo.fondoVL.LServizioUtile, datiPensione, datiFondoXX, codiceSpecificoTraduzioneSuGP,
                                    datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null,
                                    datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                    return;
                            }
                        }
                        break;
                    #endregion Fondo VL

                    #region Fondo ET
                    case Utility.TipoFondo.ET:
                        if (!ControlsSettimaneUtiliDiritto(datiCalcolo, out messaggioVideo, datiPensione))
                            return;
                        decimal? retribuzionePensionabileQuotaA = null;
                        GestioneFondo.DatiFondoET datiFondoET_DB = (GestioneFondo.DatiFondoET)datiFondoXX;
                        if (datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null && datiCalcolo.fondoET.lDatiServizioUtile.Count > 0)
                        {
                            DatiServizioUtile servizioUtileQuotaA = datiCalcolo.fondoET.lDatiServizioUtile.Find(x => x.Quota == "A");
                            if (servizioUtileQuotaA != null)
                                retribuzionePensionabileQuotaA = servizioUtileQuotaA.RetribuzionePensionabile;

                            if (!ControlsDatiServizioUtile(datiCalcolo.fondoET.lDatiServizioUtile, out messaggioVideo))
                                return;
                        }

                        if (!GestioneControlli.VerificaRetribuzionePensionabileQuotaA_ET(datiPensione, datiPensione.TipoCalcolo, retribuzionePensionabileQuotaA,
                            datiFondoET_DB != null ? datiFondoET_DB.Stipendio : null, datiFondoET_DB != null ? datiFondoET_DB.Importo13ma : null, datiFondoET_DB != null ? datiFondoET_DB.Importo14ma : null,
                            datiFondoET_DB != null ? datiFondoET_DB.ElementiAccessori : null, datiFondoET_DB != null ? datiFondoET_DB.Competenze40Percento : null, out messaggioVideo))
                            return;

                        if (datiCalcolo.fondoET != null)
                        {
                            if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensione, datiFondoXX: datiFondoXX))
                            {
                                if (!ControlsDatiServizioUtileAnteArmonizzazioneWithAssicurazione(datiCalcolo, datiFondo, datiCalcolo.fondoET.lDatiServizioUtile, datiPensione, datiFondoXX, codiceSpecificoTraduzioneSuGP,
                                    datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null,
                                    datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                    return;
                            }
                            else
                            {
                                if (!GestioneControlli.ControlsDatiCalcoloET(datiCalcolo, tipoFondo, datiPensione, datiMaggBenefici, false, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                                    return;
                            }
                        }

                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                            if (!ControlRMSWithControCodiceRetribuzioneET(datiCalcolo, datiPensione, out messaggioVideo))
                                return;

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;
                        break;

                    #endregion Fondo ET

                    #region Fondo FS

                    case Utility.TipoFondo.FS:

                        if (datiCalcolo.fondoFST != null && datiCalcolo.fondoFST.lDatiServizioUtile != null && datiCalcolo.fondoFST.lDatiServizioUtile.Count > 0)
                            if ((!ControlsDatiServizioUtile(datiCalcolo.fondoFST.lDatiServizioUtile, out messaggioVideo)) ||
                                 (!ControlsDatiServizioUtileWithFineAssicurazione(datiCalcolo.fondoFST.lDatiServizioUtile, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo)))
                                return;

                        if (datiCalcolo.fondoFST != null)
                            if (!GestioneControlli.ControlsDatiCalcoloFS_PT(datiCalcolo, tipoFondo, datiPensione, tipocalcolo, codiceSpecificoTraduzioneSuGP, datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null,
                                datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                return;
                        break;

                    #endregion Fondo FS

                    #region Fondo PT

                    case Utility.TipoFondo.PT:

                        if (datiCalcolo.fondoPT != null && datiCalcolo.fondoPT.lDatiServizioUtile != null && datiCalcolo.fondoPT.lDatiServizioUtile.Count > 0)
                            if ((!ControlsDatiServizioUtile(datiCalcolo.fondoPT.lDatiServizioUtile, out messaggioVideo)) ||
                                (!ControlsDatiServizioUtileWithFineAssicurazione(datiCalcolo.fondoPT.lDatiServizioUtile, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo)))
                                return;

                        if (datiCalcolo.fondoPT != null)
                            if (!GestioneControlli.ControlsDatiCalcoloFS_PT(datiCalcolo, tipoFondo, datiPensione, tipocalcolo, codiceSpecificoTraduzioneSuGP, datiMaggBenefici != null ? datiMaggBenefici.TipoSettimaneBeneficio : null,
                                datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneAmianto : null, datiMaggBenefici != null ? datiMaggBenefici.MaggiorazioneInv74 : null, out messaggioVideo))
                                return;
                        break;

                    #endregion Fondo PT

                    #region Fondo GAS
                    case Utility.TipoFondo.GAS:
                        if (isSingleTab)
                        {
                            if (entityDatiArt11e14 == null)
                                entityDatiArt11e14 = new DatiArt11e14();
                            Utility.ValorizzaOggetti((GestioneFondo.DatiFondoGAS)datiFondoXX, entityDatiArt11e14);
                        }

                        if (!ControlsRMSWithSettimaneGAS(datiCalcolo, out messaggioVideo))
                            return;

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!datiCalcolo.fondoGAS.CodiceTipoLiquidazione.HasValue)
                        {
                            messaggioVideo = "Tipo Liquidazione obbligatorio";
                            return;
                        }

                        if (!GestioneControlli.ControlsObbligatorietaForCodiceTipoLiquidazione(datiCalcolo.fondoGAS.CodiceTipoLiquidazione, datiCalcolo.RMSQuotaA, datiCalcolo.RMSQuotaB,
                            datiCalcolo.NSettimaneQuotaA, datiCalcolo.NSettimaneQuotaB, entityDatiArt11e14.ContributiTotaliSupplementoDPR143271, out messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;

                        if (!GestioneControlli.VerificaDecorrenzaTeorica(datiCalcolo.fondoGAS.DecorrenzaTeorica, datiFondo != null ? datiFondo.InizioBonus : null, datiPensione.DecorrenzaOriginaria,
                            out messaggioVideo))
                            return;
                        break;
                    #endregion Fondo GAS

                    #region Fondo DZ
                    case Utility.TipoFondo.DZ:

                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                            if (!ControlRetribuzioneWithControCodiceRetribuzioneDZ(datiCalcolo, datiPensione, out messaggioVideo))
                                return;

                        if (datiCalcolo.fondoDZ != null && datiCalcolo.fondoDZ.lDatiServizioUtile != null && datiCalcolo.fondoDZ.lDatiServizioUtile.Count > 0)
                            if ((!ControlsDatiServizioUtile(datiCalcolo.fondoDZ.lDatiServizioUtile, out messaggioVideo)) ||
                                (!ControlsDatiServizioUtileWithFineAssicurazione(datiCalcolo.fondoDZ.lDatiServizioUtile, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo)))
                                return;

                        ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(tipocalcolo, datiPensione, datiFondo, datiFondoXX, out messaggioVideo);
                        if (!String.IsNullOrEmpty(messaggioVideo))
                            return;

                        if (!GestioneCrossControls.ALL_VerificaRiduzioneRetributiva(datiPensione, datiPensione, isRiaperturaDomanda, datiFondo.RiduzioneRetributiva,
                            datiFondo.RiduzioneRetributivaPercentuale, out messaggioVideo))
                            return;
                        break;
                    #endregion Fondo DZ

                    #region Fondo ES
                    case Utility.TipoFondo.ES:

                        if (!datiCalcolo.fondoES.CodiceTipoLiquidazione.HasValue)
                        {
                            messaggioVideo = "Tipo Liquidazione obbligatorio";
                            return;
                        }
                        if (datiCalcolo.fondoES.CodiceTipoLiquidazione.Value != 1 && datiCalcolo.fondoES.CodiceTipoLiquidazione.Value != 3 && datiCalcolo.fondoES.CodiceTipoLiquidazione.Value != 4)
                        {
                            messaggioVideo = "Tipo Liquidazione può avere i valori : 1 per retributiva, 3 per mista, 4 per contributiva";
                            return;
                        }
                        if (datiCalcolo.fondoES.CodiceTipoLiquidazione.Value == 1 && datiCalcolo.TipoCalcolo == TipoCalcolo.Contributivo)
                        {
                            messaggioVideo = "Il Tipo Liquidazione 1 non è ammesso con il tipo di calcolo contributivo";
                            return;
                        }
                        if (datiCalcolo.fondoES.CodiceTipoLiquidazione.Value == 4 && datiCalcolo.TipoCalcolo != TipoCalcolo.Contributivo)
                        {
                            messaggioVideo = "Il Tipo Liquidazione 4 è ammesso solo per il tipo calcolo contributivo";
                            return;
                        }
                        // Per il tipo liquidazione 1 risultano obbligatori i campi RMS e Settimane Totali delle quote retributive
                        if (datiCalcolo.fondoES.CodiceTipoLiquidazione.Value == 1 &&
                            !((datiCalcolo.RMSQuotaA.HasValue && datiCalcolo.NSettimaneQuotaA.HasValue) || (datiCalcolo.RMSQuotaB.HasValue && datiCalcolo.NSettimaneQuotaB.HasValue)))
                        {
                            messaggioVideo = "Per il Tipo Liquidazione 1 sono obblicatori i campi RMS e Sett. Anz. Tot della quota A o B";
                            return;
                        }
                        // Per il tipo liquidazione 4 risulta essere obbligatorio solo il Montante Totale
                        if (datiCalcolo.fondoES.CodiceTipoLiquidazione.Value == 4 &&
                            (!datiCalcolo.Montante.HasValue))
                        {
                            messaggioVideo = "Per il Tipo Liquidazione 4 è obbligatorio il campo Montante Totale";
                            return;
                        }
                        break;
                    #endregion Fondo ES

                    #region Fondo PI
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        if (categoriaFondoPI.HasValue)
                        {
                            if (!datiCalcolo.fondoPI.StipendioAnnuo.HasValue)
                            {
                                messaggioVideo = "Elemento Retributivo obbligatorio.";
                                return;
                            }

                            //rimosso per tutte le PI il 22/01/2026
                            //if (!datiCalcolo.fondoPI.StipendioBase.HasValue)
                            //{
                            //    messaggioVideo = "Stipendio Base obbligatorio.";
                            //    return;
                            //}

                            switch (categoriaFondoPI.Value)
                            {
                                case Utility.CategoriaFondoPI.U:
                                    if (!GestioneControlli.ControlsControCodiceRetributivoPIU(datiCalcolo.fondoPI.StipendioAnnuo, datiCalcolo.fondoPI.StipendioBase, datiCalcolo.fondoPI.PensComplRiv1_95,
                                        datiCalcolo.fondoPI.ControCodiceRetribuzione, datiPensione, out messaggioVideo))
                                        return;

                                    if (!GestioneControlli.ControlsPensComplRiv195PIU(datiCalcolo.fondoPI.PensComplRiv1_95, decorrenzaPensione, out messaggioVideo))
                                        return;
                                    break;
                                case Utility.CategoriaFondoPI.V:
                                    if (!GestioneControlli.ControlsCapienzaSettimanePIV(datiCalcolo.fondoPI.NSettimaneQuotaA, datiCalcolo.fondoPI.NSettimaneQuotaB, datiPensione.InizioAssicurazione,
                                        datiPensione.FineAssicurazione, out messaggioVideo))
                                        return;
                                    break;
                            }
                        }
                        break;
                        #endregion Fondo PI
                }
            }

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.NUM_SETT_APEPRECOCI) &&
                !GestioneControlli.ControlsNSettimanePerAPEPrecoci(datiPensione, datiCalcolo, null, out messaggioVideo))
                return;
        }

        private static void ControlsDatiCalcolo707Cross(Entity.DatiCalcolo707 datiCalcolo707, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo,
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.ValorizzaOggetti(datiCalcolo707, datiFondo);

            if (datiFondo == null)
            {
                messaggioVideo = "Controlli Incrociati - Dati Liquidazione Pensione:<br/>Dati Fondo Generici obbligatori.";
                return;
            }
        }

        private static void ControlsDatiArt11e14(DatiArt11e14 entityDatiArt11e14, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, object datiFondoXX, DatiCalcolo datiCalcolo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (isSingleTab)
            {
                if (datiCalcolo == null)
                    datiCalcolo = new DatiCalcolo();

                GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivo = null;
                GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null;
                GestioneCalcolo.GetCalcoloContributivoByIdPensione(datiPensione.Id, out datiCalcoloContributivo);
                GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);

                Utility.ValorizzaOggetti(datiCalcoloContributivo, datiCalcolo);
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, datiCalcolo);
            }

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.GAS:
                        if (isSingleTab)
                        {
                            if (datiCalcolo.fondoGAS == null)
                                datiCalcolo.fondoGAS = new FondoGAS();
                            Utility.ValorizzaOggetti((GestioneFondo.DatiFondoGAS)datiFondoXX, datiCalcolo.fondoGAS);
                        }

                        if (!GestioneControlli.ControlsObbligatorietaForCodiceTipoLiquidazione(datiCalcolo.fondoGAS.CodiceTipoLiquidazione, datiCalcolo.RMSQuotaA, datiCalcolo.RMSQuotaB, datiCalcolo.NSettimaneQuotaA, datiCalcolo.NSettimaneQuotaB, entityDatiArt11e14.ContributiTotaliSupplementoDPR143271, out messaggioVideo))
                            return;
                        break;
                    case Utility.TipoFondo.ES:
                        //if (isSingleTab)
                        //{
                        //    if (datiCalcolo.fondoES == null)
                        //        datiCalcolo.fondoES = new FondoES();

                        //    Utility.ValorizzaOggetti((GestioneFondo.DatiFondoES)datiFondoXX, datiCalcolo.);
                        //}
                        //if (!GestioneControlli.ControlsObbligatorietaForCodiceTipoLiquidazione(datiCalcolo.fondoGAS.CodiceTipoLiquidazione, datiCalcolo.RMSQuotaA, datiCalcolo.RMSQuotaB, datiCalcolo.NSettimaneQuotaA, datiCalcolo.NSettimaneQuotaB, entityDatiArt11e14.ContributiTotaliSupplementoDPR143271, out messaggioVideo))
                        //    return;
                        break;
                }
            }
        }

        private static void ControlsRiduzioniRetributivaEL_TT_ET_VL_GAS_DZ(Utility.TipoCalcolo tipoCalcolo, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, object datiFondoXX,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!GestioneCrossControls.AGO_FS_VerificaDipendenzaPerfezRequisitiRiduzioneRetributiva(datiPensione, datiFondo.RiduzioneRetributiva, tipoCalcolo))
            {
                messaggioVideo = "La riduzione retributiva è incompatibile con la data perfezionamento requisiti.";
                return;
            }

            if (!GestioneControlli.ControlsRiduzioneRetributiva(tipoCalcolo, datiFondo, datiPensione, datiFondoXX, out messaggioVideo))
                return;
        }

        private static bool ControlsDatiServizioUtile(List<DatiServizioUtile> lDatiServizioUtile, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                    lDatiServizioUtileApp.Add(datiServizioUtile);
                }

                if (!GestioneControlli.ControlsDatiServizioUtile(lDatiServizioUtileApp, out messaggioVideo))
                    return false;
            }
            return true;
        }

        private static bool ControlsDatiServizioUtileWithFineAssicurazione(List<DatiServizioUtile> lDatiServizioUtile, DateTime? fineAssicurazione, Utility.TipoFondo? tipoFondo,
            GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
            {
                List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp = new List<GestioneDatiServizioUtile.ServizioUtile>();
                foreach (DatiServizioUtile servizioUtile in lDatiServizioUtile)
                {
                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                    Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                    lDatiServizioUtileApp.Add(datiServizioUtile);
                }

                if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(lDatiServizioUtileApp, fineAssicurazione, tipoFondo, datiPensione, out messaggioVideo))
                    return false;
            }

            return true;
        }

        private static bool ControlsDatiServizioUtileAnteArmonizzazioneWithAssicurazione(DatiCalcolo datiCalcolo, GestioneFondo.DatiFondo datiFondo, List<DatiServizioUtile> lDatiServizioUtile,
            GestionePensione.DatiPensione datiPensione, object datiFondoXX, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio, int? maggiorazioneAmianto, int? maggiorazioneInv74,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp = null;
            if (lDatiServizioUtile != null && lDatiServizioUtile.Count > 0)
            {
                lDatiServizioUtileApp = lDatiServizioUtile.Select(x =>
               {
                   var y = new GestioneDatiServizioUtile.ServizioUtile();
                   Utility.ValorizzaOggetti(x, y);
                   return y;
               }).ToList();
            }

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.EL:
                        if (!GestioneControlli.ControlsELDatiCalcoloAnteArmonizzazione(lDatiServizioUtileApp, datiFondo, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio,
                            maggiorazioneAmianto, maggiorazioneInv74, tipoFondo, false, out messaggioVideo))
                            return false;
                        break;
                    case Utility.TipoFondo.VL:
                        if (!GestioneControlli.ControlsVLDatiCalcoloAnteArmonizzazione(lDatiServizioUtileApp, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio,
                            maggiorazioneAmianto, maggiorazioneInv74, tipoFondo, false, out messaggioVideo))
                            return false;
                        break;
                    case Utility.TipoFondo.ET:
                        if (!GestioneControlli.ControlsETDatiCalcoloAnteArmonizzazione(lDatiServizioUtileApp, datiPensione, datiDanteCausa, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio,
                            maggiorazioneAmianto, maggiorazioneInv74, tipoFondo, false, out messaggioVideo))
                            return false;
                        break;
                    case Utility.TipoFondo.TT:
                        if (!GestioneControlli.ControlsTTDatiCalcoloAnteArmonizzazione(lDatiServizioUtileApp, datiPensione, datiDanteCausa, datiFondoXX,
                            datiCalcolo != null && datiCalcolo.fondoTT != null ? datiCalcolo.fondoTT.ControCodiceRetrQtaA : null, false, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto,
                            maggiorazioneInv74, tipoFondo, out messaggioVideo))
                            return false;
                        break;
                }
            }

            return true;
        }


        public static void ControlsDatiPensioneFondoPI(GestioneFondo.DatiFondoPI datiFondoPI, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, short? ControCodiceRetribuzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            DateTime? decorrenzaPensione = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            if (categoriaFondoPI.HasValue)
            {
                if (!datiFondoPI.StipendioAnnuo.HasValue)
                {
                    messaggioVideo = "Elemento Retributivo obbligatorio.";
                    return;
                }

                //rimosso per tutte le PI il 22/01/2026
                //if (!datiCalcolo.fondoPI.StipendioBase.HasValue)
                //{
                //    messaggioVideo = "Stipendio Base obbligatorio.";
                //    return;
                //}

                switch (categoriaFondoPI.Value)
                {
                    case Utility.CategoriaFondoPI.U:
                        if (!GestioneControlli.ControlsControCodiceRetributivoPIU(datiFondoPI.StipendioAnnuo, datiFondoPI.StipendioBase, datiFondoPI.PensComplRiv1_95,
                            ControCodiceRetribuzione, datiPensione, out messaggioVideo))
                            return;

                        if (!GestioneControlli.ControlsPensComplRiv195PIU(datiFondoPI.PensComplRiv1_95, decorrenzaPensione, out messaggioVideo))
                            return;
                        break;
                    case Utility.CategoriaFondoPI.V:
                        if (!GestioneControlli.ControlsCapienzaSettimanePIV(datiFondoPI.NSettimaneQuotaA, datiFondoPI.NSettimaneQuotaB, datiPensione.InizioAssicurazione,
                            datiPensione.FineAssicurazione, out messaggioVideo))
                            return;
                        break;
                }

                if (string.IsNullOrEmpty(datiFondoPI.Qualifica))
                {
                    messaggioVideo = "Qualifica obbligatoria";
                    return;
                }

                if (categoriaFondoPI != Utility.CategoriaFondoPI.U && categoriaFondoPI != Utility.CategoriaFondoPI.V)
                {
                    if (!ControCodiceRetribuzione.HasValue)
                    {
                        messaggioVideo = "Controcodice Retribuzione obbligatorio";
                        return;
                    }

                    if (!datiFondoPI.StipendioAnnuo.HasValue)
                    {
                        messaggioVideo = "Stipendio Annuo obbligatorio";
                        return;
                    }

                    //Per il momento la rendiamo non obbligatoria
                    //if (string.IsNullOrEmpty(datiAssicurativi.fondoPI.NumeroMatricola))
                    //{
                    //    messaggioVideo = "Matricola obbligatoria";
                    //    return false;
                    //}

                    //if (datiAssicurativi.fondoPI.NumeroMatricola.Length != 8)
                    //{
                    //    messaggioVideo = "Matricola deve avere lunghezza 8";
                    //    return false;
                    //}

                    if (!GestioneControlli.CheckImportoWithControCodice(datiFondoPI.StipendioAnnuo.HasValue ? datiFondoPI.StipendioAnnuo.Value : (decimal?)null,
                     ControCodiceRetribuzione.HasValue ? ControCodiceRetribuzione.Value : (int?)null, datiPensione, out messaggioVideo))
                        return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(datiFondoPI.Qualifica) && !(new List<string> { "10", "20", "30" }).Contains(datiFondoPI.Qualifica.Trim()))
                    {
                        messaggioVideo = "Inquadramento Professionale errato. (Valori ammessi: 10 - non quadri, 20 - quadri, 30 - dirigenti)";
                        return;
                    }

                    //if (!GestioneControlli.VerificaRequisitiEtaPIU_PIV(codiceSpecificoTraduzioneSuGP, attivitaSvoltaTraduzioneSuGP, datiPensione.DataPerfezionamentoRequisiti, dataNascita,
                    //    datiFondoPI.ServizioUtile, datiAssicurativi.InizioAssicurazione, datiAssicurativi.FineAssicurazione, datiPensione.DecorrenzaOriginaria, sesso, isDanteCausaPresente,
                    //    datiPensione.SiglaCategoria.Trim().ToUpper(), out messaggioVideo))
                    //    return false;
                }

                if (!GestioneControlli.IsValoreAAMMGGValido(datiFondoPI.RiscattiAA, null, null))
                {
                    messaggioVideo = "Riscatti AA deve essere compreso tra 0 e 99";
                    return;
                }

                if (!GestioneControlli.IsValoreAAMMGGValido(null, datiFondoPI.RiscattiMM, null))
                {
                    messaggioVideo = "Riscatti MM deve essere compreso tra 0 e 11";
                    return;
                }

                if (!GestioneControlli.IsValoreAAMMGGValido(null, null, datiFondoPI.RiscattiGG))
                {
                    messaggioVideo = "Riscatti GG deve essere compreso tra 0 e 29";
                    return;
                }
            }
        }

        /// <summary>
        /// Get dati contributivi o retributivi presenti nelle tabelle dei singoli fondi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="RetribNotNull"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiAggPeco"></param>
        private static void RecuperaDatiCalcFromFondiFelpeByIdPensione(long idPensione, Utility.TipoFondo? tipoFondo, ref GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.TT:
                        GestioneFondo.DatiFondoTT datiFondoTT = null;
                        GestioneFondo.GetFondoTTByIdPensione(idPensione, out datiFondoTT);
                        if (datiFondoTT != null)
                        {
                            if (datiFondoTT.RetribuzioneBiennio.HasValue || datiFondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue)
                            {
                                if (datiAggPeco == null)
                                    datiAggPeco = new GestioneAggiornamentoPECO.DatiTotaliAggPeco();
                                if (datiAggPeco.Retribuzione == null)
                                    datiAggPeco.Retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                            }

                            if (datiFondoTT.RetribuzioneBiennio.HasValue)
                                datiAggPeco.Retribuzione.RetribuzioneBiennio = datiFondoTT.RetribuzioneBiennio.Value;
                            if (datiFondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue)
                                datiAggPeco.Retribuzione.RetribuzioneUltimoAnnoQuotaA = datiFondoTT.RetribuzioneUltimoAnnoQuotaA.Value;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get dati contributivi o retributivi di storico presenti nelle tabelle dei singoli fondi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="RetribNotNull"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="datiAggPeco"></param>
        private static void RecuperaDatiCalcFromFondiStoricoByIdPensione(GestioneDatiStoricoGP.DatiStoricoGP datiStorico, Utility.TipoFondo? tipoFondo,
            ref GestioneAggiornamentoPECO.DatiTotaliAggPeco datiAggPeco)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.TT:
                        if (datiStorico != null)
                        {
                            if (datiStorico.RetribuzioneBiennio.HasValue || datiStorico.RetribuzioneUltimoAnnoQuotaA.HasValue)
                            {
                                if (datiAggPeco == null)
                                    datiAggPeco = new GestioneAggiornamentoPECO.DatiTotaliAggPeco();
                                if (datiAggPeco.Retribuzione == null)
                                    datiAggPeco.Retribuzione = new GestioneAggiornamentoPECO.DatiRetributivi();
                            }

                            if (datiStorico.RetribuzioneBiennio.HasValue)
                                datiAggPeco.Retribuzione.RetribuzioneBiennio = datiStorico.RetribuzioneBiennio.Value;
                            if (datiStorico.RetribuzioneUltimoAnnoQuotaA.HasValue)
                                datiAggPeco.Retribuzione.RetribuzioneUltimoAnnoQuotaA = datiStorico.RetribuzioneUltimoAnnoQuotaA.Value;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get dati collaterali ai dati contributivi e retributivi presenti nelle tabelle dei singoli fondi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="IdPensione"></param>
        /// <param name="RetribNotNull"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="crossDataRecipient"></param>
        private static void RecuperaDatiCustomFromFondi(long IdPensione, Utility.TipoFondo? tipoFondo, string gruppo, bool? isPlUnicarpe, ref CrossDataRecipient crossDataRecipient, GestionePensione.DatiPensione datiPensione)
        {
            crossDataRecipient = new CrossDataRecipient();

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                    case Utility.TipoFondo.DZ:
                        List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = null;
                        GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(IdPensione, out lServizioUtileCommon);
                        if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        {
                            crossDataRecipient.IdPensione = IdPensione;
                            crossDataRecipient.lDatiServizioUtile = new List<DatiServizioUtile>();
                            foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon in lServizioUtileCommon)
                            {
                                DatiServizioUtile datiServizioUtile = new DatiServizioUtile();
                                Utility.ValorizzaOggetti(servizioUtileCommon, datiServizioUtile);
                                crossDataRecipient.lDatiServizioUtile.Add(datiServizioUtile);
                            }
                        }
                        if (tipoFondo.Value == Utility.TipoFondo.FS)
                        {
                            GestioneFondo.DatiFondoFST datiFondoFST = null;
                            GestioneFondo.GetFondoFSTByIdPensione(IdPensione, out datiFondoFST);
                            if (datiFondoFST != null)
                            {
                                if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                                   !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    crossDataRecipient.PensioneAnnuaLorda214 = datiFondoFST.PensioneAnnuaLorda214;
                                else
                                    crossDataRecipient.PensioneAnnuaLorda = datiFondoFST.PensioneAnnuaLorda;
                                crossDataRecipient.ServizioUtileDirittoAA = datiFondoFST.ServizioUtileDirittoAA;
                                crossDataRecipient.ServizioUtileDirittoMM = datiFondoFST.ServizioUtileDirittoMM;
                                crossDataRecipient.ServizioUtileDirittoGG = datiFondoFST.ServizioUtileDirittoGG;
                                crossDataRecipient.PensioneAnnuaLorda707 = datiFondoFST.PensioneAnnuaLorda707;
                                crossDataRecipient.CoefficienteTrasformazione = datiFondoFST.CoefficienteTrasformazione;
                            }
                        }
                        if (tipoFondo.Value == Utility.TipoFondo.PT)
                        {
                            GestioneFondo.DatiFondoPT datiFondoPT = null;
                            GestioneFondo.GetFondoPTByIdPensione(IdPensione, out datiFondoPT);
                            if (datiFondoPT != null)
                            {
                                if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                                   !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    crossDataRecipient.PensioneAnnuaLorda214 = datiFondoPT.PensioneAnnuaLorda214;
                                else
                                    crossDataRecipient.PensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda;
                                crossDataRecipient.ServizioUtileDirittoAA = datiFondoPT.ServizioUtileDirittoAA;
                                crossDataRecipient.ServizioUtileDirittoMM = datiFondoPT.ServizioUtileDirittoMM;
                                crossDataRecipient.ServizioUtileDirittoGG = datiFondoPT.ServizioUtileDirittoGG;
                                crossDataRecipient.PensioneAnnuaLorda707 = datiFondoPT.PensioneAnnuaLorda707;
                                crossDataRecipient.CoefficienteTrasformazione = datiFondoPT.CoefficienteTrasformazione;
                            }
                        }
                        if (tipoFondo.Value == Utility.TipoFondo.DZ)
                        {
                            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
                            GestioneFondo.GetFondoDZByIdPensione(IdPensione, out datiFondoDZ);
                            if (datiFondoDZ != null)
                            {
                                crossDataRecipient.PensioneBaseAnnua = datiFondoDZ.PensioneBaseAnnua;
                                crossDataRecipient.Sospensione = datiFondoDZ.Sospensione;
                            }
                        }
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.DatiFondoVL datiFondoVL = null;
                        GestioneFondo.GetFondoVLByIdPensione(IdPensione, out datiFondoVL);
                        if (datiFondoVL != null)
                            crossDataRecipient.LavoratorePrecoce = datiFondoVL.LavoratorePrecoce;
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.DatiFondoGAS datiFondoGAS = null;
                        GestioneFondo.GetFondoGASByIdPensione(IdPensione, out datiFondoGAS);
                        if (datiFondoGAS != null)
                        {
                            crossDataRecipient.SospensioneAGO = datiFondoGAS.SospensioneAGO;
                            crossDataRecipient.AnniDifferimento = datiFondoGAS.AnniDifferimento;
                            crossDataRecipient.CodiceSpecificoAgo = datiFondoGAS.CodiceSpecificoAgo;
                            crossDataRecipient.CodiceTipoLiquidazione = datiFondoGAS.CodiceTipoLiquidazione;
                            crossDataRecipient.DecorrenzaDatiAgo = datiFondoGAS.DecorrenzaDatiAgo;
                            crossDataRecipient.SettimaneAnzianitaEsclusiva = datiFondoGAS.SettimaneAnzianitaEsclusiva;
                            crossDataRecipient.EtaMaturazioneRequisiti = datiFondoGAS.EtaMaturazioneRequisiti;
                            crossDataRecipient.DecorrenzaTeorica = datiFondoGAS.DecorrenzaTeorica;
                        }
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.DatiFondoES datiFondoES = null;
                        GestioneFondo.GetFondoESByIdPensione(IdPensione, out datiFondoES);
                        if (datiFondoES != null)
                        {
                            crossDataRecipient.IntegrazioneArticolo11 = datiFondoES.IntegrazioneArticolo11;
                            crossDataRecipient.AnniDifferimento = datiFondoES.AnniDifferimento;
                            crossDataRecipient.CodiceSpecificoAgo = datiFondoES.CodiceSpecificoAgo;
                            crossDataRecipient.CodiceTipoLiquidazione = datiFondoES.CodiceTipoLiquidazione;
                            crossDataRecipient.BaseAltraPensione = datiFondoES.BaseAltraPensione;
                            crossDataRecipient.CategoriaAltraPensione = datiFondoES.CategoriaAltraPensione;
                            crossDataRecipient.ImportoContributiLegge37758Art24 = datiFondoES.ImportoContributiLegge37758Art24;
                            crossDataRecipient.ImportoContributiLegge37758Art57 = datiFondoES.ImportoContributiLegge37758Art57;
                            crossDataRecipient.Decorrenza = datiFondoES.Decorrenza;
                            crossDataRecipient.ContributiDifferimentoQuota = datiFondoES.ContributiDifferimentoQuota;
                            crossDataRecipient.EtaMaturazioneRequisiti = datiFondoES.EtaMaturazioneRequisiti;
                            crossDataRecipient.SettimaneArt24QB = datiFondoES.SettimaneArt24QB;
                            crossDataRecipient.SettimaneArt24QA = datiFondoES.SettimaneArt24QA;
                            crossDataRecipient.NSettimaneLegge37758Art57 = datiFondoES.NSettimaneLegge37758Art57;
                            crossDataRecipient.Sospensione = datiFondoES.Sospensione;
                            crossDataRecipient.ImportoContributiLegge143271Art14 = datiFondoES.ImportoContributiLegge143271Art14;
                            crossDataRecipient.DecorrenzaTeorica = datiFondoES.DecorrenzaTeorica;
                        }
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.DatiFondoPI datiFondoPI = null;
                        GestioneFondo.GetFondoPIByIdPensione(IdPensione, out datiFondoPI);
                        if (datiFondoPI != null)
                        {
                            crossDataRecipient.RMSQuotaA = datiFondoPI.RMSQuotaA;
                            crossDataRecipient.RMSQuotaB = datiFondoPI.RMSQuotaB;
                            crossDataRecipient.NSettimaneQuotaA = datiFondoPI.NSettimaneQuotaA;
                            crossDataRecipient.NSettimaneQuotaB = datiFondoPI.NSettimaneQuotaB;
                            crossDataRecipient.StipendioAnnuo = datiFondoPI.StipendioAnnuo;
                            crossDataRecipient.StipendioBase = datiFondoPI.StipendioBase;
                            crossDataRecipient.ImportoIIS = datiFondoPI.ImportoIIS;
                            crossDataRecipient.PensioneFacoltativaMensile = datiFondoPI.PensioneFacoltativaMensile;
                            crossDataRecipient.AttCon = datiFondoPI.AttCon;
                            crossDataRecipient.PercentualeCapitalizzazione = datiFondoPI.PercentualeCapitalizzazione;
                            crossDataRecipient.CodiceMaggiorazione = datiFondoPI.CodiceMaggiorazione;
                            crossDataRecipient.PensComplRiv1_95 = datiFondoPI.PensComplRiv1_95;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get dati collaterali ai dati contributivi e retributivi di storico presenti nelle tabelle dei singoli fondi
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="IdPensione"></param>
        /// <param name="RetribNotNull"></param>
        /// <param name="tipoFondo"></param>
        /// <param name="crossDataRecipient"></param>
        private static void RecuperaDatiServizioUtileStorico(long IdPensione, Utility.TipoFondo? tipoFondo, ref CrossDataRecipient crossDataRecipient)
        {
            crossDataRecipient = new CrossDataRecipient();

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.ET:
                        List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon = null;
                        GestioneDatiServizioUtile.GetDatiServizioUtileStoricoByIdPensione(IdPensione, out lServizioUtileCommon);
                        if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        {
                            crossDataRecipient.IdPensione = IdPensione;
                            crossDataRecipient.lDatiServizioUtile = new List<DatiServizioUtile>();
                            foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtileCommon in lServizioUtileCommon)
                            {
                                DatiServizioUtile datiServizioUtile = new DatiServizioUtile();
                                Utility.ValorizzaOggetti(servizioUtileCommon, datiServizioUtile);
                                crossDataRecipient.lDatiServizioUtile.Add(datiServizioUtile);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get dati collaterali ai dati contributivi e retributivi presenti in varie tabelle
        /// </summary>
        /// <param name="datiFondo"></param>
        /// <param name="crossDataRecipient"></param>
        private static void RecuperaDatiFlatForDatiCalcolo(GestioneFondo.DatiFondo datiFondo, ref CrossDataRecipient crossDataRecipient)
        {
            if (datiFondo != null)
            {
                crossDataRecipient.RiduzioneRetributiva = datiFondo.RiduzioneRetributiva;
                crossDataRecipient.RiduzioneRetributivaPercentuale = datiFondo.RiduzioneRetributivaPercentuale;
            }
        }

        /// <summary>
        /// Get dati collaterali ai dati contributivi e retributivi presenti in varie tabelle
        /// </summary>
        /// <param name="datiFondo"></param>
        /// <param name="crossDataRecipient"></param>
        private static void RecuperaDatiFlatForDatiCalcoloStorico(GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, ref CrossDataRecipient crossDataRecipient)
        {
            if (datiStoricoGP != null)
            {
                crossDataRecipient.RiduzioneRetributiva = datiStoricoGP.RiduzioneRetributiva;
                crossDataRecipient.RiduzioneRetributivaPercentuale = datiStoricoGP.RiduzioneRetributivaPercentuale;
            }
        }

        private static void DeleteDatiContributiviWithFondi(long IdPensione, Utility.TipoFondo? tipoFondo, object Fondo, long idFondoGenerico, bool IsFondoNull)
        {
            if (tipoFondo.HasValue && Fondo != null)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.TT:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoTT(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoTT datiFondoTT = (GestioneFondo.DatiFondoTT)Fondo;
                            GestioneFondo.SalvaFondoTT(idFondoGenerico, datiFondoTT);
                        }
                        break;

                    case Utility.TipoFondo.VL:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoVL(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoVL datiFondoVL = (GestioneFondo.DatiFondoVL)Fondo;
                            GestioneFondo.SalvaFondoVL(idFondoGenerico, datiFondoVL);
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoFST(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)Fondo;
                            GestioneFondo.SalvaFondoFST(idFondoGenerico, datiFondoFST);
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoPT(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                            GestioneFondo.SalvaFondoPT(idFondoGenerico, datiFondoPT);
                        }
                        break;
                    case Utility.TipoFondo.GAS:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoGAS(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoGAS datiFondoGAS = (GestioneFondo.DatiFondoGAS)Fondo;
                            GestioneFondo.SalvaFondoGAS(idFondoGenerico, datiFondoGAS);
                        }
                        break;
                    case Utility.TipoFondo.DZ:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoDZ(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoDZ datiFondoDZ = (GestioneFondo.DatiFondoDZ)Fondo;
                            GestioneFondo.SalvaFondoDZ(idFondoGenerico, datiFondoDZ);
                        }
                        break;
                    case Utility.TipoFondo.ES:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoES(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoES datiFondoES = (GestioneFondo.DatiFondoES)Fondo;
                            GestioneFondo.SalvaFondoES(idFondoGenerico, datiFondoES);
                        }
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoPI(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoPI datiFondoPI = (GestioneFondo.DatiFondoPI)Fondo;
                            GestioneFondo.SalvaFondoPIRecordFondo(idFondoGenerico, datiFondoPI.IdRecordFondo, datiFondoPI);
                            // GestioneFondo.SalvaFondoPI(idFondoGenerico, datiFondoPI);
                        }
                        break;
                    case Utility.TipoFondo.ET:
                        if (IsFondoNull)
                            GestioneFondo.EliminaFondoET(IdPensione);
                        else
                        {
                            GestioneFondo.DatiFondoET datiFondoET = (GestioneFondo.DatiFondoET)Fondo;
                            GestioneFondo.SalvaFondoET(idFondoGenerico, datiFondoET);
                        }
                        break;
                }
            }
        }

        private static bool? GestioneContributivoL214(Liquidazione.BLCommon.GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, char? codiceSpecifico)
        {
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoCalcolo == Utility.TipoCalcolo.Contributivo || tipoCalcolo == Utility.TipoCalcolo.Misto)
                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                    return true;

            if (tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti)
                return true;

            // Per il fondo PM non è valida la condizione di pensione di inabilità post 2011
            if (tipoFondo == Utility.TipoFondo.PM)
                return false;

            if (Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecifico) && datiPensione.FineAssicurazione.HasValue &&
                !Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)) && Utility.DataSuccessivaA(decorrenzaPensioneOrDecorrenzaPensioneDC.Value, new DateTime(2012, 02, 01)))
                return true;

            return false;
        }

        private static bool ControlsRMSWithSettimaneEL_TT(DatiCalcolo datiCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcolo == null)
            {
                messaggioVideo = "Dati Calcolo non valorizzati";
                return false;
            }

            if ((datiCalcolo.RMSQuotaA.HasValue && !datiCalcolo.NSettimaneQuotaA.HasValue) || (!datiCalcolo.RMSQuotaA.HasValue && datiCalcolo.NSettimaneQuotaA.HasValue))
            {
                messaggioVideo = "In presenza della Retribuzione Media Settimanale Quota A sono previste le Settimane A e viceversa";
                return false;
            }
            if ((datiCalcolo.RMSQuotaB.HasValue && !datiCalcolo.NSettimaneQuotaB.HasValue && !datiCalcolo.NSettimaneQuotaC.HasValue) || (!datiCalcolo.RMSQuotaB.HasValue && (datiCalcolo.NSettimaneQuotaB.HasValue || datiCalcolo.NSettimaneQuotaC.HasValue)))
            {
                messaggioVideo = "In presenza della Retribuzione Media Settimanale Quota B sono previste le Settimane B o C e viceversa";
                return false;
            }
            if ((datiCalcolo.RMSQuotaD.HasValue && !datiCalcolo.NSettimaneQuotaD.HasValue) || (!datiCalcolo.RMSQuotaD.HasValue && datiCalcolo.NSettimaneQuotaD.HasValue))
            {
                messaggioVideo = "In presenza della Retribuzione Media Settimanale Quota D sono previste le Settimane D e viceversa";
                return false;
            }

            return true;
        }

        private static bool ControlsSettimaneUtiliDiritto(DatiCalcolo datiCalcolo, out string messaggioVideo, GestionePensione.DatiPensione datiPensione)
        {
            messaggioVideo = string.Empty;

            //Per le domande ai Superstiti NON Indirette il controllo non deve essere effettuato         
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !Utility.IsDomandaPensioneIndiretta(datiPensione))
                return true;

            if (datiCalcolo == null)
            {
                messaggioVideo = "Dati Calcolo non valorizzati";
                return false;
            }

            if (!datiCalcolo.SettimaneUtiliDiritto.HasValue || datiCalcolo.SettimaneUtiliDiritto.Value == 0)
            {
                messaggioVideo = "Il campo 'Settimane Utili Diritto' deve essere valorizzato e maggiore di 0.";
                return false;
            }

            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione)) // Memo 79
            {
                if (!datiCalcolo.SettimaneUtiliDirittoOI.HasValue || datiCalcolo.SettimaneUtiliDirittoOI.Value == 0)
                {
                    messaggioVideo = "Il campo 'Settimane Utili Diritto OI' deve essere valorizzato e maggiore di 0.";
                    return false;
                }
                if (!datiCalcolo.SettimaneUtiliDiritto.HasValue || datiCalcolo.SettimaneUtiliDiritto.Value < 52)
                {
                    messaggioVideo = "La differenza tra 'Settimane Utili al Diritto TOT' e le 'Settimane Utili al Diritto OI' deve essere maggiore o uguale a 52.";
                    return false;
                }
            }

            return true;
        }

        private static bool ControlRMSWithControCodiceRetribuzioneET(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcolo != null && datiCalcolo.fondoET != null && datiCalcolo.fondoET.lDatiServizioUtile != null)
            {
                var lDatiServizioUtile = datiCalcolo.fondoET.lDatiServizioUtile.ToList();

                foreach (var servUtile in lDatiServizioUtile)
                {
                    if (servUtile.RetribuzionePensionabile.HasValue && servUtile.ControCodiceRetributivo.HasValue)
                    {
                        if (!GestioneControlli.CheckImportoWithControCodice(servUtile.RetribuzionePensionabile, servUtile.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                        {
                            StringBuilder bld = new StringBuilder();
                            bld.Append(messaggioVideo);
                            bld.Append(" (Quota " + servUtile.Quota + ")");
                            messaggioVideo = bld.ToString();
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool ControlRetribuzioneWithControCodiceRetribuzioneDZ(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcolo != null && datiCalcolo.fondoDZ != null && datiCalcolo.fondoDZ.lDatiServizioUtile != null)
            {
                var lDatiServizioUtile = datiCalcolo.fondoDZ.lDatiServizioUtile.ToList();

                foreach (var servUtile in lDatiServizioUtile)
                {
                    if ((servUtile.RetribuzionePensionabile.HasValue || servUtile.ControCodiceRetributivo.HasValue) &&
                        (!servUtile.RetribuzionePensionabile.HasValue || !servUtile.ControCodiceRetributivo.HasValue))
                    {
                        messaggioVideo = "I campi Servizio Utile devono essere tutti acquisiti";
                        return false;
                    }

                    if (!GestioneControlli.CheckImportoWithControCodice(servUtile.RetribuzionePensionabile, servUtile.ControCodiceRetributivo, datiPensione, out messaggioVideo))
                    {
                        StringBuilder bld = new StringBuilder();
                        bld.Append(messaggioVideo);
                        bld.Append(" (Quota " + servUtile.Quota + ")");
                        messaggioVideo = bld.ToString();
                        return false;
                    }
                }
            }

            return true;
        }

        //private static void StoreDatiFondoPerFondoGas(long idPensione, long idFondo, DatiFondoGAS_ES entityDatiFondoGAS, GestioneFondo.DatiFondoGAS datiFondoGAS, bool isFondoGasNull, bool eliminaFondoDatiGenerici)
        //{
        //    if (datiFondoGAS == null)
        //    {
        //        if (entityDatiFondoGAS == null || isFondoGasNull)
        //        {
        //            if (eliminaFondoDatiGenerici)
        //                GestioneFondo.EliminaFondoDatiGenerici(idPensione);
        //            return;
        //        }
        //        else
        //            datiFondoGAS = new GestioneFondo.DatiFondoGAS();
        //    }
        //    Utility.ValorizzaOggetti(entityDatiFondoGAS, datiFondoGAS);
        //    if (datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))
        //    {
        //        GestioneFondo.EliminaFondoGAS(idPensione);
        //        if (eliminaFondoDatiGenerici)
        //            GestioneFondo.EliminaFondoDatiGenerici(idPensione);
        //    }
        //    else
        //    {
        //        if (idFondo == 0 || eliminaFondoDatiGenerici)
        //        {
        //            GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
        //            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
        //            idFondo = datiFondoNew.Id;
        //        }
        //        GestioneFondo.SalvaFondoGAS(idFondo, datiFondoGAS);
        //    }
        //}

        private static void StoreDatiServizioUtilePerFondo(long idPensione, long idFondo, EntityDatiFondo entityDatiFondoGAS, GestioneDatiServizioUtile.ServizioUtile datiServizioUtile, bool isServizioUtileNull, bool eliminaFondoDatiGenerici)
        {
            if (datiServizioUtile == null)
            {
                if (entityDatiFondoGAS == null || isServizioUtileNull)
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
            }
            Utility.ValorizzaOggetti(entityDatiFondoGAS, datiServizioUtile);
            if (datiServizioUtile.Equals(new GestioneDatiServizioUtile.ServizioUtile()))
            {
                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, datiServizioUtile);
            }
        }

        private static void StoreDatiFondoPerFondoEs(long idPensione, long idFondo, FondoES entityDatiFondoES, ref GestioneFondo.DatiFondoES datiFondoES, bool isFondoEsNull, bool eliminaFondoDatiGenerici)
        {

            if (datiFondoES == null)
            {
                if (entityDatiFondoES == null || isFondoEsNull)
                {
                    if (isFondoEsNull)
                        GestioneFondo.EliminaFondoES(idPensione);
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }

            Utility.ValorizzaOggetti(entityDatiFondoES, datiFondoES);
            // datiFondoES.IdFondo = 0;

            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
        }

        private static bool ControlsRMSWithSettimaneGAS(DatiCalcolo datiCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCalcolo == null)
            {
                messaggioVideo = "Dati Calcolo non valorizzati";
                return false;
            }

            if ((datiCalcolo.RMSQuotaA.HasValue && !datiCalcolo.NSettimaneQuotaA.HasValue) || (!datiCalcolo.RMSQuotaA.HasValue && datiCalcolo.NSettimaneQuotaA.HasValue))
            {
                messaggioVideo = "In presenza della Retribuzione Media Settimanale Quota A sono previste le Settimane A e viceversa";
                return false;
            }
            if ((datiCalcolo.RMSQuotaB.HasValue && !datiCalcolo.NSettimaneQuotaB.HasValue) || (!datiCalcolo.RMSQuotaB.HasValue && datiCalcolo.NSettimaneQuotaB.HasValue))
            {
                messaggioVideo = "In presenza della Retribuzione Media Settimanale Quota B sono previste le Settimane B e viceversa";
                return false;
            }

            return true;
        }

        private static void StoreDatiArt11e14PerFondoGas(long idPensione, long idFondo, DatiArt11e14 entityDatiArt11e14, GestioneFondo.DatiFondoGAS datiFondoGAS, bool isArt11e14Null, bool eliminaFondoDatiGenerici)
        {
            if (datiFondoGAS == null)
            {
                if (entityDatiArt11e14 == null || isArt11e14Null)
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiFondoGAS = new GestioneFondo.DatiFondoGAS();
            }
            Utility.ValorizzaOggetti(entityDatiArt11e14, datiFondoGAS);
            if (datiFondoGAS.Equals(new GestioneFondo.DatiFondoGAS()))
            {
                GestioneFondo.EliminaFondoGAS(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoGAS(idFondo, datiFondoGAS);
            }
        }

        private static void StoreDatiArt11e14PerFondoEs(long idPensione, long idFondo, DatiArt11e14 entityDatiArt11e14, GestioneFondo.DatiFondoES datiFondoES, bool isArt11e14Null, bool eliminaFondoDatiGenerici)
        {
            if (datiFondoES == null)
            {
                if (entityDatiArt11e14 == null || isArt11e14Null)
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }
            //Utility.ValorizzaOggetti(entityDatiArt11e14, datiFondoES);
            datiFondoES.RmsDPCM = entityDatiArt11e14.RMSArt14;
            datiFondoES.RMSSent72 = entityDatiArt11e14.RMSSent72;
            datiFondoES.DecDPCM = entityDatiArt11e14.DecDPCM;

            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
        }

        private static void StoreDatiAnte67PerFondoEs(long idPensione, long idFondo, DatiAnte67 entityDatiAnte67, GestioneFondo.DatiFondoES datiFondoES, bool isAnte67Null, bool eliminaFondoDatiGenerici)
        {
            if (datiFondoES == null)
            {
                if (entityDatiAnte67 == null || isAnte67Null)
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }
            Utility.ValorizzaOggetti(entityDatiAnte67, datiFondoES);

            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
        }

        private static void StoreDatiSL336PerFondoEs(long idPensione, long idFondo, DatiSL33670 entityDatiSL336, GestioneFondo.DatiFondoES datiFondoES, bool isAnte67Null, bool eliminaFondoDatiGenerici)
        {
            if (datiFondoES == null)
            {
                if (entityDatiSL336 == null || isAnte67Null)
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiFondoES = new GestioneFondo.DatiFondoES();
            }
            Utility.ValorizzaOggetti(entityDatiSL336, datiFondoES);

            if (datiFondoES.Equals(new GestioneFondo.DatiFondoES()))
            {
                GestioneFondo.EliminaFondoES(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoES(idFondo, datiFondoES);
            }
        }

        private static void StoreDatiAgoAltraPensionePerFondoET(long idPensione, long idFondo, DatiAgoAltraPensione entityDatiAgoAltraPensione, GestioneFondo.DatiFondoET datiFondoET,
            bool eliminaFondoDatiGenerici)
        {
            if (datiFondoET == null)
            {
                if (entityDatiAgoAltraPensione == null || entityDatiAgoAltraPensione.IsNull())
                {
                    if (eliminaFondoDatiGenerici)
                        GestioneFondo.EliminaFondoDatiGenerici(idPensione);
                    return;
                }
                else
                    datiFondoET = new GestioneFondo.DatiFondoET();
            }
            Utility.ValorizzaOggetti(entityDatiAgoAltraPensione, datiFondoET);

            if (datiFondoET.Equals(new GestioneFondo.DatiFondoET()))
            {
                GestioneFondo.EliminaFondoET(idPensione);
                if (eliminaFondoDatiGenerici)
                    GestioneFondo.EliminaFondoDatiGenerici(idPensione);
            }
            else
            {
                if (idFondo == 0 || eliminaFondoDatiGenerici)
                {
                    GestioneFondo.DatiFondo datiFondoNew = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondoNew);
                    idFondo = datiFondoNew.Id;
                }
                GestioneFondo.SalvaFondoET(idFondo, datiFondoET);
            }
        }

        public static bool ControlsSettimaneUtiliDirittoFondi(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo,
             ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.VL) &&
                 datiMaggiorazioniBenefici != null && !string.IsNullOrEmpty(datiMaggiorazioniBenefici.TipoSettimaneBeneficio) && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01" &&
                 Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
            {
                // Memo 79
                int? settimaneUtiliDiritto = datiCalcolo.SettimaneUtiliDiritto.HasValue ? datiCalcolo.SettimaneUtiliDiritto.Value : 0;
                string AddTotaliMessage = string.Empty;
                if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                {
                    settimaneUtiliDiritto += datiCalcolo.SettimaneUtiliDirittoOI.HasValue ? datiCalcolo.SettimaneUtiliDirittoOI.Value : 0;
                    AddTotaliMessage = "Tot";
                }


                if (settimaneUtiliDiritto < 520)
                {
                    messaggioVideo = "Le Settimane Utili al Diritto " + AddTotaliMessage + " devono essere maggiore o uguale a 520";
                    return false;
                }
            }
            return true;
        }

        #endregion private members

        #region nested class
        public class DatiCalcolo
        {
            public byte? Semaforo { get; set; }
            public DatiCalcolo()
            { }

            public DatiCalcolo(System.Nullable<decimal> rmsQuotaA, System.Nullable<decimal> rmsQuotaB, System.Nullable<decimal> rmsQuotaD,
                System.Nullable<int> nSettimaneQuotaA, System.Nullable<int> nSettimaneQuotaB, System.Nullable<int> nSettimaneQuotaC,
                System.Nullable<int> nSettimaneQuotaD, System.Nullable<decimal> retribuzionePonderataAnnua,
                System.Nullable<decimal> montante, System.Nullable<decimal> importoContributivoTotale, System.Nullable<int> nSettimane,
                System.Nullable<decimal> montanteEsclusivo, System.Nullable<int> nSettimaneEsclusiveQuotaA, System.Nullable<int> nSettimaneEsclusiveQuotaB,
                TipoCalcolo tipoCalcolo, bool isCalcoloValido, decimal? QuotaContributivaAnnua, int? settimaneUtiliDiritto, int? settimaneUtiliDirittoOI)
            {
                this._RMSQuotaA = rmsQuotaA;
                this._RMSQuotaB = rmsQuotaB;
                this._RMSQuotaD = rmsQuotaD;
                this._NSettimaneQuotaA = nSettimaneQuotaA;
                this._NSettimaneQuotaB = nSettimaneQuotaB;
                this._NSettimaneQuotaC = nSettimaneQuotaC;
                this._NSettimaneQuotaD = nSettimaneQuotaD;
                this._RetribuzionePonderataAnnua = retribuzionePonderataAnnua;
                this._Montante = montante;
                this._ImportoContributivoTotale = importoContributivoTotale;
                this._NSettimane = nSettimane;
                this._MontanteEsclusivo = montanteEsclusivo;
                this._NSettimaneEsclusiveQuotaA = nSettimaneEsclusiveQuotaA;
                this._NSettimaneEsclusiveQuotaB = nSettimaneEsclusiveQuotaB;
                this._TipoCalcolo = tipoCalcolo;
                this._IsCalcoloValido = isCalcoloValido;
                this._QuotaContributivaAnnua = QuotaContributivaAnnua;
                this._SettimaneUtiliDiritto = settimaneUtiliDiritto;
            }

            public DatiCalcolo(GestioneAggiornamentoPECO.DatiTotaliAggPeco datiTotaliAggPeco, Utility.TipoFondo? tipoFondo, CrossDataRecipient crossDataRecipient, byte? tipoCalcoloDatiPensione, string gruppo, bool? isPlUnicarpe, GestionePensione.DatiPensione datiPensione)
            {
                if (datiTotaliAggPeco == null)
                    return;

                if (datiTotaliAggPeco.Retribuzione != null)
                {
                    this._RiduzioneRetributiva = crossDataRecipient.RiduzioneRetributiva;
                    this._RiduzioneRetributivaPercentuale = crossDataRecipient.RiduzioneRetributivaPercentuale;
                    this._IdPensione = crossDataRecipient.IdPensione; // per gestire i fondi FS e PT che non contemplano dati retributivi salvati nella tabella DatiRetributivi ma solo in DatiServizioUtile
                    this._RMSQuotaA = datiTotaliAggPeco.Retribuzione.RmsQuotaA != 0M ? datiTotaliAggPeco.Retribuzione.RmsQuotaA : (decimal?)null;
                    this._RMSQuotaB = datiTotaliAggPeco.Retribuzione.RmsQuotaB != 0M ? datiTotaliAggPeco.Retribuzione.RmsQuotaB : (decimal?)null;
                    this._RMSQuotaD = datiTotaliAggPeco.Retribuzione.RmsQuotaD != 0M ? datiTotaliAggPeco.Retribuzione.RmsQuotaD : (decimal?)null;
                    this._NSettimaneQuotaA = datiTotaliAggPeco.Retribuzione.SettimaneA;
                    this._NSettimaneQuotaB = datiTotaliAggPeco.Retribuzione.SettimaneB;
                    this._NSettimaneQuotaC = datiTotaliAggPeco.Retribuzione.SettimaneC;
                    this._NSettimaneQuotaD = datiTotaliAggPeco.Retribuzione.SettimaneD;
                    this._RetribuzionePonderataAnnua = datiTotaliAggPeco.Retribuzione.RetribuzionePonderataAnnua != 0M ? datiTotaliAggPeco.Retribuzione.RetribuzionePonderataAnnua : (decimal?)null;
                    this._NSettimaneQuotaA2 = datiTotaliAggPeco.Retribuzione.SettimaneA2 != 0 ? datiTotaliAggPeco.Retribuzione.SettimaneA2 : (int?)null;
                    this._NSettimaneQuotaC2 = datiTotaliAggPeco.Retribuzione.SettimaneC2 != 0 ? datiTotaliAggPeco.Retribuzione.SettimaneC2 : (int?)null;

                    this._NSettimaneEsclusiveQuotaA = datiTotaliAggPeco.Retribuzione.NSettimaneEsclusiveQuotaA != 0 ? datiTotaliAggPeco.Retribuzione.NSettimaneEsclusiveQuotaA : (int?)null;
                    this._NSettimaneEsclusiveQuotaB = datiTotaliAggPeco.Retribuzione.NSettimaneEsclusiveQuotaB != 0 ? datiTotaliAggPeco.Retribuzione.NSettimaneEsclusiveQuotaB : (int?)null;
                    this._NSettAnzianitaVV = datiTotaliAggPeco.Retribuzione.NSettAnzianitaVV != 0 ? datiTotaliAggPeco.Retribuzione.NSettAnzianitaVV : (int?)null;
                    //comma 707
                    this._QuotaA707 = datiTotaliAggPeco.Retribuzione.QuotaA707;
                    this._QuotaA2707 = datiTotaliAggPeco.Retribuzione.QuotaA2707;
                    this._QuotaB707 = datiTotaliAggPeco.Retribuzione.QuotaB707;
                    this._QuotaC707 = datiTotaliAggPeco.Retribuzione.QuotaC707;
                    this._QuotaC2707 = datiTotaliAggPeco.Retribuzione.QuotaC2707;
                    this._QuotaD707 = datiTotaliAggPeco.Retribuzione.QuotaD707;
                    this._QuotaA707AA = datiTotaliAggPeco.Retribuzione.QuotaA707AA;
                    this._QuotaA707MM = datiTotaliAggPeco.Retribuzione.QuotaA707MM;
                    this._QuotaA707GG = datiTotaliAggPeco.Retribuzione.QuotaA707GG;
                    this._QuotaB707AA = datiTotaliAggPeco.Retribuzione.QuotaB707AA;
                    this._QuotaB707MM = datiTotaliAggPeco.Retribuzione.QuotaB707MM;
                    this._QuotaB707GG = datiTotaliAggPeco.Retribuzione.QuotaB707GG;
                    this._QuotaC707AA = datiTotaliAggPeco.Retribuzione.QuotaC707AA;
                    this._QuotaC707MM = datiTotaliAggPeco.Retribuzione.QuotaC707MM;
                    this._QuotaC707GG = datiTotaliAggPeco.Retribuzione.QuotaC707GG;
                    this._RetribuzionePonderataAGO707 = datiTotaliAggPeco.Retribuzione.RetribuzionePonderataAGO707;
                    this._QuotaAES707 = datiTotaliAggPeco.Retribuzione.QuotaAES707;
                    this._QuotaBES707 = datiTotaliAggPeco.Retribuzione.QuotaBES707;
                }
                if (datiTotaliAggPeco.Contribuzione != null)
                {
                    this._Montante = datiTotaliAggPeco.Contribuzione.Montante != 0M ? datiTotaliAggPeco.Contribuzione.Montante : (decimal?)null;
                    this._MontanteContributivo = datiTotaliAggPeco.Contribuzione.MontanteContributivo != 0M ? datiTotaliAggPeco.Contribuzione.MontanteContributivo : (decimal?)null;
                    //this._MontanteContributivo = datiTotaliAggPeco.Contribuzione.MontanteContributivo != 0M ? datiTotaliAggPeco.Contribuzione.MontanteContributivo : 0;
                    this._ImportoContributivoTotale = datiTotaliAggPeco.Contribuzione.ImportoContributivoTotale != 0M ? datiTotaliAggPeco.Contribuzione.ImportoContributivoTotale : (decimal?)null;
                    this._NSettimane = datiTotaliAggPeco.Contribuzione.Settimane != 0 ? datiTotaliAggPeco.Contribuzione.Settimane : (int?)null;
                    this._QuotaContributivaAnnua = datiTotaliAggPeco.Contribuzione.QuotaContributivaAnnua != 0M ? datiTotaliAggPeco.Contribuzione.QuotaContributivaAnnua : (decimal?)null;
                    //this._QuotaContributivaAnnua = datiTotaliAggPeco.Contribuzione.QuotaContributivaAnnua != 0M ? datiTotaliAggPeco.Contribuzione.QuotaContributivaAnnua : 0;
                    this._MontanteQuotaDL214 = datiTotaliAggPeco.Contribuzione.MontanteQuotaDL214 != 0M ? datiTotaliAggPeco.Contribuzione.MontanteQuotaDL214 : (decimal?)null;
                    this._ImportoContribTotaleQuotaDL214 = datiTotaliAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 != 0M ? datiTotaliAggPeco.Contribuzione.ImportoContribTotaleQuotaDL214 : (decimal?)null;
                    this._NSettimaneQuotaDL214 = datiTotaliAggPeco.Contribuzione.NSettimaneQuotaDL214 != 0 ? datiTotaliAggPeco.Contribuzione.NSettimaneQuotaDL214 : (int?)null;
                    this._MontanteAnte0697 = datiTotaliAggPeco.Contribuzione.MontanteAnte0697 != 0M ? datiTotaliAggPeco.Contribuzione.MontanteAnte0697 : (decimal?)null;
                    this._AnzianitaAnte0697AA = datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697AA != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697AA : (short?)null;
                    this._AnzianitaAnte0697MM = datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697MM != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697MM : (short?)null;
                    this._AnzianitaAnte0697GG = datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697GG != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaAnte0697GG : (short?)null;
                    this._AnzianitaPost0697AA = datiTotaliAggPeco.Contribuzione.AnzianitaPost0697AA != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaPost0697AA : (short?)null;
                    this._AnzianitaPost0697MM = datiTotaliAggPeco.Contribuzione.AnzianitaPost0697MM != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaPost0697MM : (short?)null;
                    this._AnzianitaPost0697GG = datiTotaliAggPeco.Contribuzione.AnzianitaPost0697GG != 0 ? datiTotaliAggPeco.Contribuzione.AnzianitaPost0697GG : (short?)null;


                    this._MontanteEsclusivo = datiTotaliAggPeco.Contribuzione.MontanteEsclusivo != 0M ? datiTotaliAggPeco.Contribuzione.MontanteEsclusivo : (decimal?)null;
                    this._MontanteEsclusivoQuotaDL214 = datiTotaliAggPeco.Contribuzione.MontanteEsclusivoQuotaDL214 != 0M ? datiTotaliAggPeco.Contribuzione.MontanteEsclusivoQuotaDL214 : (decimal?)null;
                }

                #region Settimane Utili
                if (datiTotaliAggPeco.DatiParziali != null && datiTotaliAggPeco.DatiParziali.SettimaneUtiliDiritto.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.EL:
                        case Utility.TipoFondo.ET:
                        case Utility.TipoFondo.TT:
                        case Utility.TipoFondo.VL:
                            this.SettimaneUtiliDiritto = datiTotaliAggPeco.DatiParziali.SettimaneUtiliDiritto;
                            this.SettimaneUtiliDirittoOI = datiTotaliAggPeco.DatiParziali.SettimaneUtiliDirittoOI;
                            break;
                    }
                }
                #endregion

                #region gestione fondi

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.TT:

                            if (datiTotaliAggPeco.Retribuzione != null)
                            {
                                if (this.fondoTT == null)
                                    this.fondoTT = new FondoTT();
                                this.fondoTT.RetribuzioneBiennio = datiTotaliAggPeco.Retribuzione.RetribuzioneBiennio;
                                this.fondoTT.RetribuzioneUltimoAnnoQuotaA = datiTotaliAggPeco.Retribuzione.RetribuzioneUltimoAnnoQuotaA;
                            }
                            break;
                        case Utility.TipoFondo.ET:
                            if (crossDataRecipient.lDatiServizioUtile != null && crossDataRecipient.lDatiServizioUtile.Count > 0)
                            {
                                if (this.fondoET == null)
                                    this.fondoET = new FondoET();
                                this.fondoET.lDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                            }
                            break;
                        case Utility.TipoFondo.VL:
                            if (crossDataRecipient.LavoratorePrecoce.HasValue)
                            {
                                if (this.fondoVL == null)
                                    this.fondoVL = new FondoVL();
                                this.fondoVL.LavoratorePrecoce = crossDataRecipient.LavoratorePrecoce;
                            }
                            break;

                        case Utility.TipoFondo.FS:
                            decimal? pensioneAnnuaLordaFS = null;
                            if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                               !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                pensioneAnnuaLordaFS = crossDataRecipient.PensioneAnnuaLorda214;
                            else
                                pensioneAnnuaLordaFS = crossDataRecipient.PensioneAnnuaLorda;

                            if ((crossDataRecipient.lDatiServizioUtile != null && crossDataRecipient.lDatiServizioUtile.Count > 0) ||
                                 pensioneAnnuaLordaFS.HasValue || crossDataRecipient.ServizioUtileDirittoAA.HasValue ||
                                 crossDataRecipient.ServizioUtileDirittoMM.HasValue || crossDataRecipient.ServizioUtileDirittoGG.HasValue ||
                                 crossDataRecipient.RMSSenzaLegge33670QA.HasValue)
                            {
                                if (this.fondoFST == null)
                                    this.fondoFST = new FondoFST();
                                this._IdPensione = crossDataRecipient.IdPensione;
                                this.fondoFST.lDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                                if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                                   !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    this.fondoFST.PensioneAnnuaLorda214 = pensioneAnnuaLordaFS;
                                else
                                    this.fondoFST.PensioneAnnuaLorda = pensioneAnnuaLordaFS;
                                this.fondoFST.ServizioUtileDirittoAA = crossDataRecipient.ServizioUtileDirittoAA;
                                this.fondoFST.ServizioUtileDirittoMM = crossDataRecipient.ServizioUtileDirittoMM;
                                this.fondoFST.ServizioUtileDirittoGG = crossDataRecipient.ServizioUtileDirittoGG;
                                this.fondoFST.RMSSenzaLegge33670QA = crossDataRecipient.RMSSenzaLegge33670QA;
                            }
                            break;
                        case Utility.TipoFondo.PT:
                            decimal? pensioneAnnuaLordaPT = null;
                            if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                               !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                pensioneAnnuaLordaPT = crossDataRecipient.PensioneAnnuaLorda214;
                            else
                                pensioneAnnuaLordaPT = crossDataRecipient.PensioneAnnuaLorda;

                            if ((crossDataRecipient.lDatiServizioUtile != null && crossDataRecipient.lDatiServizioUtile.Count > 0) ||
                                 pensioneAnnuaLordaPT.HasValue || crossDataRecipient.ServizioUtileDirittoAA.HasValue ||
                                 crossDataRecipient.ServizioUtileDirittoMM.HasValue || crossDataRecipient.ServizioUtileDirittoGG.HasValue)
                            {
                                if (this.fondoPT == null)
                                    this.fondoPT = new FondoPT();
                                this._IdPensione = crossDataRecipient.IdPensione;
                                this.fondoPT.lDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                                if (Utility.IsRicostituzione(gruppo) && isPlUnicarpe.GetValueOrDefault() &&
                                   !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                    this.fondoPT.PensioneAnnuaLorda214 = pensioneAnnuaLordaPT != 0M ? pensioneAnnuaLordaPT : (decimal?)null;
                                else
                                    this.fondoPT.PensioneAnnuaLorda = pensioneAnnuaLordaPT != 0M ? pensioneAnnuaLordaPT : (decimal?)null;
                                this.fondoPT.ServizioUtileDirittoAA = crossDataRecipient.ServizioUtileDirittoAA != 0 ? crossDataRecipient.ServizioUtileDirittoAA : (short?)null;
                                this.fondoPT.ServizioUtileDirittoMM = crossDataRecipient.ServizioUtileDirittoMM != 0 ? crossDataRecipient.ServizioUtileDirittoMM : (short?)null;
                                this.fondoPT.ServizioUtileDirittoGG = crossDataRecipient.ServizioUtileDirittoGG != 0 ? crossDataRecipient.ServizioUtileDirittoGG : (short?)null;
                            }
                            break;

                        case Utility.TipoFondo.GAS:
                            if (crossDataRecipient.SospensioneAGO.HasValue || crossDataRecipient.AnniDifferimento.HasValue || crossDataRecipient.CodiceSpecificoAgo.HasValue ||
                                crossDataRecipient.CodiceTipoLiquidazione.HasValue || crossDataRecipient.DecorrenzaDatiAgo.HasValue ||
                                crossDataRecipient.SettimaneAnzianitaEsclusiva.HasValue || crossDataRecipient.EtaMaturazioneRequisiti.HasValue || crossDataRecipient.DecorrenzaTeorica.HasValue)
                            {
                                if (this.fondoGAS == null)
                                    this.fondoGAS = new FondoGAS();
                                this.fondoGAS.SospensioneAGO = crossDataRecipient.SospensioneAGO.HasValue ? crossDataRecipient.SospensioneAGO.Value : (DateTime?)null;
                                this.fondoGAS.AnniDifferimento = crossDataRecipient.AnniDifferimento.HasValue ? crossDataRecipient.AnniDifferimento.Value : (int?)null;
                                this.fondoGAS.CodiceSpecificoAgo = crossDataRecipient.CodiceSpecificoAgo.HasValue ? crossDataRecipient.CodiceSpecificoAgo.Value : (char?)null;
                                this.fondoGAS.CodiceTipoLiquidazione = crossDataRecipient.CodiceTipoLiquidazione.HasValue ? crossDataRecipient.CodiceTipoLiquidazione.Value : (byte?)null;
                                this.fondoGAS.DecorrenzaDatiAgo = crossDataRecipient.DecorrenzaDatiAgo.HasValue ? crossDataRecipient.DecorrenzaDatiAgo.Value : (DateTime?)null;
                                this.fondoGAS.SettimaneAnzianitaEsclusiva = crossDataRecipient.SettimaneAnzianitaEsclusiva.HasValue ? crossDataRecipient.SettimaneAnzianitaEsclusiva.Value : (short?)null;
                                this.fondoGAS.EtaMaturazioneRequisiti = crossDataRecipient.EtaMaturazioneRequisiti.HasValue ? crossDataRecipient.EtaMaturazioneRequisiti.Value : (byte?)null;
                                this.fondoGAS.DecorrenzaTeorica = crossDataRecipient.DecorrenzaTeorica.HasValue ? crossDataRecipient.DecorrenzaTeorica.Value : (DateTime?)null;
                            }
                            break;
                        case Utility.TipoFondo.DZ:
                            if ((crossDataRecipient.lDatiServizioUtile != null && crossDataRecipient.lDatiServizioUtile.Count > 0) ||
                                crossDataRecipient.PensioneBaseAnnua.HasValue || crossDataRecipient.Sospensione.HasValue)
                            {
                                if (this.fondoDZ == null)
                                    this.fondoDZ = new FondoDZ();
                                this.fondoDZ.lDatiServizioUtile = crossDataRecipient.lDatiServizioUtile;
                                this.fondoDZ.PensioneBaseAnnua = crossDataRecipient.PensioneBaseAnnua;
                                this.fondoDZ.Sospensione = crossDataRecipient.Sospensione;
                            }
                            break;
                        case Utility.TipoFondo.ES:
                            if (crossDataRecipient.IntegrazioneArticolo11 != null || crossDataRecipient.AnniDifferimento != null ||
                            crossDataRecipient.CodiceSpecificoAgo != null || crossDataRecipient.CodiceTipoLiquidazione != null ||
                            crossDataRecipient.BaseAltraPensione != null || crossDataRecipient.CategoriaAltraPensione != null ||
                            crossDataRecipient.ImportoContributiLegge37758Art24 != null || crossDataRecipient.ImportoContributiLegge37758Art57 != null ||
                            crossDataRecipient.Decorrenza != null || crossDataRecipient.ContributiDifferimentoQuota != null ||
                            crossDataRecipient.EtaMaturazioneRequisiti != null || crossDataRecipient.SettimaneArt24QB != null ||
                            crossDataRecipient.SettimaneArt24QA != null || crossDataRecipient.NSettimaneLegge37758Art57 != null ||
                            crossDataRecipient.Sospensione != null || crossDataRecipient.ImportoContributiLegge143271Art14 != null ||
                            crossDataRecipient.DecorrenzaTeorica != null)
                            {

                                if (this.fondoES == null)
                                    this.fondoES = new FondoES_AGO();
                                Utility.ValorizzaOggetti(crossDataRecipient, this.fondoES);
                            }
                            break;
                        case Utility.TipoFondo.PI:
                        case Utility.TipoFondo.PL:
                            if (crossDataRecipient.RMSQuotaA != null || crossDataRecipient.RMSQuotaB != null || crossDataRecipient.NSettimaneQuotaA != null ||
                                crossDataRecipient.NSettimaneQuotaB != null || crossDataRecipient.StipendioAnnuo != null || crossDataRecipient.StipendioBase != null ||
                                crossDataRecipient.ImportoIIS != null || crossDataRecipient.PensioneFacoltativaMensile != null || crossDataRecipient.AttCon != null ||
                                crossDataRecipient.PercentualeCapitalizzazione != null || crossDataRecipient.CodiceMaggiorazione != null || crossDataRecipient.PensComplRiv1_95 != null)
                            {
                                if (this.fondoPI == null)
                                    this.fondoPI = new FondoPI();

                                Utility.ValorizzaOggetti(crossDataRecipient, this.fondoPI);
                            }
                            break;


                    }
                }
                #endregion gestione fondi

                #region Gestione Ante Armonizzazione
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.EL:
                            if ((crossDataRecipient.LServizioUtileAnteArmonizzazione != null && crossDataRecipient.LServizioUtileAnteArmonizzazione.Count > 0) ||
                                crossDataRecipient.RetrPondAnnuaAGOLimite.HasValue)
                            {
                                if (this._fondoEL == null)
                                    this._fondoEL = new FondoEL();

                                this._fondoEL.LServizioUtile = crossDataRecipient.LServizioUtileAnteArmonizzazione;
                                this._fondoEL.RetrPondAnnuaAGOLimite = crossDataRecipient.RetrPondAnnuaAGOLimite;
                            }
                            break;
                        case Utility.TipoFondo.VL:
                            if (crossDataRecipient.LServizioUtileAnteArmonizzazione != null && crossDataRecipient.LServizioUtileAnteArmonizzazione.Count > 0)
                            {
                                if (this._fondoVL == null)
                                    this._fondoVL = new FondoVL();

                                this._fondoVL.LServizioUtile = crossDataRecipient.LServizioUtileAnteArmonizzazione;
                            }
                            break;
                        case Utility.TipoFondo.TT:
                            if (this._fondoTT == null)
                                this._fondoTT = new FondoTT();

                            if (crossDataRecipient.LServizioUtileAnteArmonizzazione != null && crossDataRecipient.LServizioUtileAnteArmonizzazione.Count > 0)
                                this._fondoTT.lDatiServizioUtile = crossDataRecipient.LServizioUtileAnteArmonizzazione;
                            this._fondoTT.PensioneMensileAl53 = crossDataRecipient.PensioneMensileAl53;
                            this._fondoTT.ElementiAccessori = crossDataRecipient.ElementiAccessori;
                            this._fondoTT.RetribuzioneSupplementi = crossDataRecipient.RetribuzioneSupplementi;
                            this._fondoTT.RetrPondAnnuaAGOLimite = crossDataRecipient.RetrPondAnnuaAGOLimite;
                            break;
                    }
                }

                #endregion Gestione Ante Armonizzazione

                if (tipoCalcoloDatiPensione != null)
                {
                    switch (tipoCalcoloDatiPensione)
                    {
                        case 19:
                            this._TipoCalcolo = TipoCalcolo.Contributivo;
                            break;
                        case 20:
                            this._TipoCalcolo = TipoCalcolo.Misto;
                            break;
                        case 18:
                            this._TipoCalcolo = TipoCalcolo.Retributivo;
                            break;
                        case 25:
                            this._TipoCalcolo = TipoCalcolo.RetributivoMonti;
                            break;
                        default:
                            this._TipoCalcolo = TipoCalcolo.NonValido;
                            break;
                    }
                }
                else
                {
                    if (datiTotaliAggPeco.DatiControllo != null)
                    {
                        switch (datiTotaliAggPeco.DatiControllo.TipoCalcolo)
                        {
                            case GestioneAggiornamentoPECO.TipoCalcolo.Contributivo:
                                this._TipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case GestioneAggiornamentoPECO.TipoCalcolo.Misto:
                                this._TipoCalcolo = TipoCalcolo.Misto;
                                break;
                            case GestioneAggiornamentoPECO.TipoCalcolo.Retributivo:
                                this._TipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case GestioneAggiornamentoPECO.TipoCalcolo.RetributivoMonti:
                                this._TipoCalcolo = TipoCalcolo.RetributivoMonti;
                                break;
                            case GestioneAggiornamentoPECO.TipoCalcolo.NonValido:
                                this._TipoCalcolo = TipoCalcolo.NonValido;
                                break;
                        }
                        this._IsCalcoloValido = datiTotaliAggPeco.DatiControllo.IsCalcoloValido;
                    }
                }
            }

            #region private properties
            private System.Nullable<long> _IdPensione;
            private System.Nullable<decimal> _RMSQuotaA;
            private System.Nullable<decimal> _RMSQuotaB;
            private System.Nullable<decimal> _RMSQuotaD;
            private System.Nullable<int> _NSettimaneQuotaA;
            private System.Nullable<int> _NSettimaneQuotaA2;
            private System.Nullable<int> _NSettimaneQuotaB;
            private System.Nullable<int> _NSettimaneQuotaC;
            private System.Nullable<int> _NSettimaneQuotaC2;
            private System.Nullable<int> _NSettimaneQuotaD;
            private System.Nullable<decimal> _RetribuzionePonderataAnnua;
            private System.Nullable<int> _NSettimaneEsclusiveQuotaA;
            private System.Nullable<int> _NSettimaneEsclusiveQuotaB;

            private System.Nullable<decimal> _Montante;
            private System.Nullable<decimal> _ImportoContributivoTotale;
            private System.Nullable<int> _NSettimane;
            private decimal? _QuotaContributivaAnnua;
            private decimal? _MontanteAnte0697;
            private short? _AnzianitaAnte0697AA;
            private short? _AnzianitaAnte0697MM;
            private short? _AnzianitaAnte0697GG;
            private short? _AnzianitaPost0697AA;
            private short? _AnzianitaPost0697MM;
            private short? _AnzianitaPost0697GG;
            private System.Nullable<decimal> _MontanteContributivo;
            private bool _RiduzioneRetributiva;
            private System.Nullable<decimal> _RiduzioneRetributivaPercentuale;
            private System.Nullable<decimal> _MontanteQuotaDL214;
            private System.Nullable<decimal> _ImportoContribTotaleQuotaDL214;
            private System.Nullable<int> _NSettimaneQuotaDL214;
            private System.Nullable<decimal> _MontanteEsclusivo;
            private decimal? _MontanteEsclusivoQuotaDL214;
            private int? _NSettAnzianitaVV;
            private int? _SettimaneUtiliDiritto;
            private int? _SettimaneUtiliDirittoOI;

            private TipoCalcolo _TipoCalcolo;
            private bool _IsCalcoloValido;
            private FondoEL _fondoEL;
            private FondoTT _fondoTT;
            private FondoET _fondoET;
            private FondoVL _fondoVL;
            private FondoPT _fondoPT;
            private FondoFST _fondoFST;
            private FondoGAS _fondoGAS;
            //Comma 707
            private System.Nullable<short> _QuotaA707;
            private short? _QuotaA2707;
            private System.Nullable<short> _QuotaB707;
            private System.Nullable<short> _QuotaC707;
            private short? _QuotaC2707;
            private System.Nullable<short> _QuotaD707;
            private byte? _QuotaA707AA;
            private byte? _QuotaA707MM;
            private byte? _QuotaA707GG;
            private byte? _QuotaB707AA;
            private byte? _QuotaB707MM;
            private byte? _QuotaB707GG;
            private byte? _QuotaC707AA;
            private byte? _QuotaC707MM;
            private byte? _QuotaC707GG;
            private System.Nullable<decimal> _RetribuzionePonderataAGO707;
            private System.Nullable<short> _QuotaAES707;
            private System.Nullable<short> _QuotaBES707;

            #endregion private properties

            #region public properties
            public System.Nullable<long> IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public System.Nullable<decimal> RMSQuotaA { get { return _RMSQuotaA; } set { _RMSQuotaA = value; } }
            public System.Nullable<decimal> RMSQuotaB { get { return _RMSQuotaB; } set { _RMSQuotaB = value; } }
            public System.Nullable<decimal> RMSQuotaD { get { return _RMSQuotaD; } set { _RMSQuotaD = value; } }
            public System.Nullable<int> NSettimaneQuotaA { get { return _NSettimaneQuotaA; } set { _NSettimaneQuotaA = value; } }
            public System.Nullable<int> NSettimaneQuotaA2 { get { return _NSettimaneQuotaA2; } set { _NSettimaneQuotaA2 = value; } }
            public System.Nullable<int> NSettimaneQuotaB { get { return _NSettimaneQuotaB; } set { _NSettimaneQuotaB = value; } }
            public System.Nullable<int> NSettimaneQuotaC { get { return _NSettimaneQuotaC; } set { _NSettimaneQuotaC = value; } }
            public System.Nullable<int> NSettimaneQuotaC2 { get { return _NSettimaneQuotaC2; } set { _NSettimaneQuotaC2 = value; } }
            public System.Nullable<int> NSettimaneQuotaD { get { return _NSettimaneQuotaD; } set { _NSettimaneQuotaD = value; } }
            public System.Nullable<decimal> RetribuzionePonderataAnnua { get { return _RetribuzionePonderataAnnua; } set { _RetribuzionePonderataAnnua = value; } }
            public System.Nullable<int> NSettimaneEsclusiveQuotaA { get { return _NSettimaneEsclusiveQuotaA; } set { _NSettimaneEsclusiveQuotaA = value; } }
            public System.Nullable<int> NSettimaneEsclusiveQuotaB { get { return _NSettimaneEsclusiveQuotaB; } set { _NSettimaneEsclusiveQuotaB = value; } }

            public System.Nullable<decimal> Montante { get { return _Montante; } set { _Montante = value; } }
            public System.Nullable<decimal> ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
            public System.Nullable<int> NSettimane { get { return _NSettimane; } set { _NSettimane = value; } }
            public decimal? QuotaContributivaAnnua { get { return _QuotaContributivaAnnua; } set { _QuotaContributivaAnnua = value; } }
            public decimal? MontanteAnte0697 { get { return _MontanteAnte0697; } set { _MontanteAnte0697 = value; } }
            public short? AnzianitaAnte0697AA { get { return _AnzianitaAnte0697AA; } set { _AnzianitaAnte0697AA = value; } }
            public short? AnzianitaAnte0697MM { get { return _AnzianitaAnte0697MM; } set { _AnzianitaAnte0697MM = value; } }
            public short? AnzianitaAnte0697GG { get { return _AnzianitaAnte0697GG; } set { _AnzianitaAnte0697GG = value; } }
            public short? AnzianitaPost0697AA { get { return _AnzianitaPost0697AA; } set { _AnzianitaPost0697AA = value; } }
            public short? AnzianitaPost0697MM { get { return _AnzianitaPost0697MM; } set { _AnzianitaPost0697MM = value; } }
            public short? AnzianitaPost0697GG { get { return _AnzianitaPost0697GG; } set { _AnzianitaPost0697GG = value; } }
            public decimal? MontanteEsclusivo { get { return _MontanteEsclusivo; } set { _MontanteEsclusivo = value; } }

            public System.Nullable<decimal> MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }
            public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }
            public System.Nullable<decimal> RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }

            public System.Nullable<decimal> MontanteQuotaDL214 { get { return _MontanteQuotaDL214; } set { _MontanteQuotaDL214 = value; } }
            public System.Nullable<decimal> ImportoContribTotaleQuotaDL214 { get { return _ImportoContribTotaleQuotaDL214; } set { _ImportoContribTotaleQuotaDL214 = value; } }
            public System.Nullable<int> NSettimaneQuotaDL214 { get { return _NSettimaneQuotaDL214; } set { _NSettimaneQuotaDL214 = value; } }
            public decimal? MontanteEsclusivoQuotaDL214 { get { return _MontanteEsclusivoQuotaDL214; } set { _MontanteEsclusivoQuotaDL214 = value; } }
            public int? NSettAnzianitaVV { get { return this._NSettAnzianitaVV; } set { this._NSettAnzianitaVV = value; } }
            public int? SettimaneUtiliDiritto { get { return this._SettimaneUtiliDiritto; } set { this._SettimaneUtiliDiritto = value; } }
            public int? SettimaneUtiliDirittoOI { get { return this._SettimaneUtiliDirittoOI; } set { this._SettimaneUtiliDirittoOI = value; } }

            public TipoCalcolo TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }
            public bool IsCalcoloValido { get { return _IsCalcoloValido; } set { _IsCalcoloValido = value; } }
            public FondoEL fondoEL { get { return _fondoEL; } set { _fondoEL = value; } }
            public FondoTT fondoTT { get { return _fondoTT; } set { _fondoTT = value; } }
            public FondoET fondoET { get { return _fondoET; } set { _fondoET = value; } }
            public FondoVL fondoVL { get { return _fondoVL; } set { _fondoVL = value; } }
            public FondoPT fondoPT { get { return _fondoPT; } set { _fondoPT = value; } }
            public FondoFST fondoFST { get { return _fondoFST; } set { _fondoFST = value; } }
            public FondoGAS fondoGAS { get { return _fondoGAS; } set { _fondoGAS = value; } }
            public FondoDZ fondoDZ { get; set; }
            public FondoES_AGO fondoES { get; set; }
            public FondoPI fondoPI { get; set; }
            //Comma 707
            public System.Nullable<short> QuotaA707 { get { return _QuotaA707; } set { _QuotaA707 = value; } }
            public short? QuotaA2707 { get { return _QuotaA2707; } set { _QuotaA2707 = value; } }
            public System.Nullable<short> QuotaB707 { get { return _QuotaB707; } set { _QuotaB707 = value; } }
            public System.Nullable<short> QuotaC707 { get { return _QuotaC707; } set { _QuotaC707 = value; } }
            public short? QuotaC2707 { get { return _QuotaC2707; } set { _QuotaC2707 = value; } }
            public System.Nullable<short> QuotaD707 { get { return _QuotaD707; } set { _QuotaD707 = value; } }
            public byte? QuotaA707AA { get { return _QuotaA707AA; } set { _QuotaA707AA = value; } }
            public byte? QuotaA707MM { get { return _QuotaA707MM; } set { _QuotaA707MM = value; } }
            public byte? QuotaA707GG { get { return _QuotaA707GG; } set { _QuotaA707GG = value; } }
            public byte? QuotaB707AA { get { return _QuotaB707AA; } set { _QuotaB707AA = value; } }
            public byte? QuotaB707MM { get { return _QuotaB707MM; } set { _QuotaB707MM = value; } }
            public byte? QuotaB707GG { get { return _QuotaB707GG; } set { _QuotaB707GG = value; } }
            public byte? QuotaC707AA { get { return _QuotaC707AA; } set { _QuotaC707AA = value; } }
            public byte? QuotaC707MM { get { return _QuotaC707MM; } set { _QuotaC707MM = value; } }
            public byte? QuotaC707GG { get { return _QuotaC707GG; } set { _QuotaC707GG = value; } }
            public System.Nullable<decimal> RetribuzionePonderataAGO707 { get { return _RetribuzionePonderataAGO707; } set { _RetribuzionePonderataAGO707 = value; } }
            public System.Nullable<short> QuotaAES707 { get { return _QuotaAES707; } set { _QuotaAES707 = value; } }
            public System.Nullable<short> QuotaBES707 { get { return _QuotaBES707; } set { _QuotaBES707 = value; } }


            public bool IsContribL335Null()
            {
                if (!this._Montante.HasValue && !this._ImportoContributivoTotale.HasValue && !this._NSettimane.HasValue)
                    return true;
                else
                    return false;
            }

            public bool IsContribL214Null()
            {
                if (!this._MontanteQuotaDL214.HasValue && !this._ImportoContribTotaleQuotaDL214.HasValue && !this._MontanteQuotaDL214.HasValue)
                    return true;
                else
                    return false;
            }

            public bool IsComma707Null()
            {
                if (this._QuotaA2707.HasValue || this._QuotaA707.HasValue || this._QuotaB707.HasValue || this._QuotaC2707.HasValue || this._QuotaC707.HasValue || this._QuotaD707.HasValue ||
                    this._RetribuzionePonderataAGO707.HasValue || this._QuotaA707AA.HasValue || this._QuotaA707MM.HasValue || this._QuotaA707GG.HasValue || this._QuotaB707AA.HasValue || this._QuotaB707MM.HasValue ||
                    this._QuotaB707GG.HasValue || this._QuotaC707AA.HasValue || this._QuotaC707MM.HasValue || this._QuotaC707GG.HasValue || this._QuotaAES707.HasValue || this._QuotaBES707.HasValue)
                    return false;

                return true;
            }

            #endregion public properties
        }

        public class FondoEL
        {
            public List<DatiServizioUtile> LServizioUtile { get; set; }
            public decimal? RetrPondAnnuaAGOLimite { get; set; }
        }

        public class FondoTT
        {
            #region private properties
            private decimal? _RetribuzioneUltimoAnnoQuotaA;
            private decimal? _Retribuzionebiennio;
            private decimal? _PensioneMensileAl53;
            private decimal? _RetrPondAnnuaAGOLimite;
            private decimal? _ElementiAccessori;
            private decimal? _RetribuzioneSupplementi;
            private int? _ControCodiceRetrQtaA;
            private List<DatiServizioUtile> _lDatiServizioUtile;
            #endregion private properties

            #region public properties
            public decimal? RetribuzioneUltimoAnnoQuotaA { get { return _RetribuzioneUltimoAnnoQuotaA; } set { _RetribuzioneUltimoAnnoQuotaA = value; } }
            public decimal? RetribuzioneBiennio { get { return _Retribuzionebiennio; } set { _Retribuzionebiennio = value; } }
            public decimal? PensioneMensileAl53 { get { return _PensioneMensileAl53; } set { _PensioneMensileAl53 = value; } }
            public decimal? RetrPondAnnuaAGOLimite { get { return _RetrPondAnnuaAGOLimite; } set { _RetrPondAnnuaAGOLimite = value; } }
            public decimal? ElementiAccessori { get { return _ElementiAccessori; } set { _ElementiAccessori = value; } }
            public decimal? RetribuzioneSupplementi { get { return _RetribuzioneSupplementi; } set { _RetribuzioneSupplementi = value; } }
            public int? ControCodiceRetrQtaA { get { return _ControCodiceRetrQtaA; } set { _ControCodiceRetrQtaA = value; } }
            public List<DatiServizioUtile> lDatiServizioUtile { get { return _lDatiServizioUtile; } set { _lDatiServizioUtile = value; } }
            #endregion public properties
        }

        public class FondoET
        {
            public FondoET()
            {
                this._lDatiServizioUtile = new List<DatiServizioUtile>();
            }

            #region private properties
            private List<DatiServizioUtile> _lDatiServizioUtile;
            #endregion private properties

            #region public properties
            public List<DatiServizioUtile> lDatiServizioUtile { get { return _lDatiServizioUtile; } set { _lDatiServizioUtile = value; } }
            #endregion public properties
        }

        public class FondoVL
        {
            #region Private Properties

            private bool? _LavoratorePrecoce;
            public List<DatiServizioUtile> LServizioUtile { get; set; }

            #endregion Private Properties

            #region Public Properties

            public bool? LavoratorePrecoce { get { return _LavoratorePrecoce; } set { _LavoratorePrecoce = value; } }

            #endregion Public Properties

            public bool IsFondoNull()
            {
                if (!this._LavoratorePrecoce.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoPT
        {
            public FondoPT()
            {
                this._lDatiServizioUtile = new List<DatiServizioUtile>();
                this._lDatiServizioUtile707 = new List<Entity.DatiCalcolo707.DatiServizioUtile707>();
            }

            #region private properties
            private List<DatiServizioUtile> _lDatiServizioUtile;
            private List<Entity.DatiCalcolo707.DatiServizioUtile707> _lDatiServizioUtile707;
            private decimal? _PensioneAnnuaLorda;
            private decimal? _PensioneAnnuaLorda707;
            private short? _ServizioUtileDirittoAA;
            private short? _ServizioUtileDirittoMM;
            private short? _ServizioUtileDirittoGG;
            private short? _ServizioUtileDiritto707;
            private decimal? _PensioneAnnuaLorda214;
            #endregion private properties

            #region public properties
            public List<DatiServizioUtile> lDatiServizioUtile { get { return _lDatiServizioUtile; } set { _lDatiServizioUtile = value; } }
            public List<Entity.DatiCalcolo707.DatiServizioUtile707> lDatiServizioUtile707 { get { return _lDatiServizioUtile707; } set { _lDatiServizioUtile707 = value; } }
            public decimal? PensioneAnnuaLorda { get { return _PensioneAnnuaLorda; } set { _PensioneAnnuaLorda = value; } }
            public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
            public short? ServizioUtileDirittoAA { get { return _ServizioUtileDirittoAA; } set { _ServizioUtileDirittoAA = value; } }
            public short? ServizioUtileDirittoMM { get { return _ServizioUtileDirittoMM; } set { _ServizioUtileDirittoMM = value; } }
            public short? ServizioUtileDirittoGG { get { return _ServizioUtileDirittoGG; } set { _ServizioUtileDirittoGG = value; } }
            public short? ServizioUtileDiritto707 { get { return _ServizioUtileDiritto707; } set { _ServizioUtileDiritto707 = value; } }
            public decimal? PensioneAnnuaLorda214 { get { return _PensioneAnnuaLorda214; } set { _PensioneAnnuaLorda214 = value; } }
            #endregion public properties
        }

        public class FondoFST
        {
            public FondoFST()
            {
                this._lDatiServizioUtile = new List<DatiServizioUtile>();
                this._lDatiServizioUtile707 = new List<Entity.DatiCalcolo707.DatiServizioUtile707>();
            }

            #region private properties

            private List<DatiServizioUtile> _lDatiServizioUtile;
            private List<Entity.DatiCalcolo707.DatiServizioUtile707> _lDatiServizioUtile707;
            private decimal? _PensioneAnnuaLorda;
            private decimal? _PensioneAnnuaLorda707;
            private short? _ServizioUtileDirittoAA;
            private short? _ServizioUtileDirittoMM;
            private short? _ServizioUtileDirittoGG;
            private short? _ServizioUtileDiritto707;
            private decimal? _RMSSenzaLegge33670QA;
            private decimal? _PensioneAnnuaLorda214;
            #endregion private properties
            #region public properties

            public List<DatiServizioUtile> lDatiServizioUtile { get { return _lDatiServizioUtile; } set { _lDatiServizioUtile = value; } }
            public List<Entity.DatiCalcolo707.DatiServizioUtile707> lDatiServizioUtile707 { get { return _lDatiServizioUtile707; } set { _lDatiServizioUtile707 = value; } }
            public decimal? PensioneAnnuaLorda { get { return _PensioneAnnuaLorda; } set { _PensioneAnnuaLorda = value; } }
            public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
            public short? ServizioUtileDirittoAA { get { return _ServizioUtileDirittoAA; } set { _ServizioUtileDirittoAA = value; } }
            public short? ServizioUtileDirittoMM { get { return _ServizioUtileDirittoMM; } set { _ServizioUtileDirittoMM = value; } }
            public short? ServizioUtileDirittoGG { get { return _ServizioUtileDirittoGG; } set { _ServizioUtileDirittoGG = value; } }
            public short? ServizioUtileDiritto707 { get { return _ServizioUtileDiritto707; } set { _ServizioUtileDiritto707 = value; } }
            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
            public decimal? PensioneAnnuaLorda214 { get { return _PensioneAnnuaLorda214; } set { _PensioneAnnuaLorda214 = value; } }
            #endregion public properties
        }

        public class FondoGAS
        {
            #region private properties
            private DateTime? _SospensioneAGO;
            private int? _AnniDifferimento;
            private char? _CodiceSpecificoAgo;
            private byte? _CodiceTipoLiquidazione;
            private DateTime? _DecorrenzaDatiAgo;
            private short? _SettimaneAnzianitaEsclusiva;
            private byte? _EtaMaturazioneRequisiti;
            #endregion private properties

            #region public properties
            public DateTime? SospensioneAGO { get { return _SospensioneAGO; } set { _SospensioneAGO = value; } }
            public int? AnniDifferimento { get { return _AnniDifferimento; } set { _AnniDifferimento = value; } }
            public char? CodiceSpecificoAgo { get { return _CodiceSpecificoAgo; } set { _CodiceSpecificoAgo = value; } }
            public byte? CodiceTipoLiquidazione { get { return _CodiceTipoLiquidazione; } set { _CodiceTipoLiquidazione = value; } }
            public DateTime? DecorrenzaDatiAgo { get { return _DecorrenzaDatiAgo; } set { _DecorrenzaDatiAgo = value; } }
            public short? SettimaneAnzianitaEsclusiva { get { return _SettimaneAnzianitaEsclusiva; } set { _SettimaneAnzianitaEsclusiva = value; } }
            public byte? EtaMaturazioneRequisiti { get { return _EtaMaturazioneRequisiti; } set { _EtaMaturazioneRequisiti = value; } }
            public DateTime? DecorrenzaTeorica { get; set; }
            #endregion public properties

            public bool IsFondoNullForDatiAgo()
            {
                if (!this._CodiceTipoLiquidazione.HasValue && !this._DecorrenzaDatiAgo.HasValue &&
                    !this._SettimaneAnzianitaEsclusiva.HasValue && !this._EtaMaturazioneRequisiti.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoES
        {
            #region DATI FONDO
            public byte? ClassePensioneAnte50 { get; set; }
            public bool? AnnoUtile { get; set; }
            public byte? Articolo58 { get; set; }
            public bool? Articolo59 { get; set; }
            public byte? CodiciRetributivi { get; set; }
            public string CodiceEsattoria { get; set; }
            public bool? CodiceDz { get; set; }
            public bool? Optanti { get; set; }
            public bool? MaggiorazionePrivilegiata { get; set; }
            public byte? Promiscui { get; set; }
            public bool? Saltuari { get; set; }

            //Elementi di Calcolo
            public int? MMServizioUtile { get; set; }
            public decimal? Retribuzione { get; set; }
            public int? MMServizioUtile2 { get; set; }
            public decimal? Retribuzione2 { get; set; }
            public int? MMServizioUtile3 { get; set; }
            public decimal? Retribuzione3 { get; set; }
            public int? MMServizioUtile4 { get; set; }
            public decimal? Retribuzione4 { get; set; }

            //Decodifiche
            public List<GestioneDecodifica.DecArt58> DecArt58;
            public List<GestioneDecodifica.DecPromiscui> DecPromiscui;
            #endregion DATI FONDO

            public bool IsNull()
            {
                if (this.CodiceEsattoria == null && !this.AnnoUtile.HasValue && !this.Articolo58.HasValue && !this.Articolo59 != null && !this.ClassePensioneAnte50.HasValue &&
                    !this.CodiceDz.HasValue && this.CodiceEsattoria == null && !this.CodiciRetributivi.HasValue && !this.MaggiorazionePrivilegiata.HasValue && !this.Optanti.HasValue && !this.Promiscui.HasValue
                    && !this.Saltuari.HasValue && !this.MMServizioUtile.HasValue && !this.Retribuzione.HasValue && !this.MMServizioUtile2.HasValue && !this.Retribuzione2.HasValue
                    && !this.MMServizioUtile3.HasValue && !this.Retribuzione3.HasValue && !this.MMServizioUtile4.HasValue && !this.Retribuzione4.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class FondoES_AGO
        {

            public System.Nullable<System.DateTime> Decorrenza { get; set; }
            public System.Nullable<decimal> ContributiDifferimentoQuota { get; set; }
            public System.Nullable<decimal> ImportoContributiLegge37758Art24 { get; set; }
            public System.Nullable<decimal> ImportoContributiLegge143271Art14 { get; set; }
            public System.Nullable<decimal> ImportoContributiLegge37758Art57 { get; set; }
            public System.Nullable<decimal> BaseAltraPensione { get; set; }
            public System.Nullable<short> CategoriaAltraPensione { get; set; }
            public System.Nullable<decimal> IntegrazioneArticolo11 { get; set; }
            public System.Nullable<int> AnniDifferimento { get; set; }
            public System.Nullable<byte> EtaMaturazioneRequisiti { get; set; }
            public System.Nullable<int> SettimaneArt24QA { get; set; }
            public System.Nullable<int> SettimaneArt24QB { get; set; }
            public System.Nullable<int> NSettimaneLegge37758Art57 { get; set; }
            public System.Nullable<System.DateTime> Sospensione { get; set; }
            public System.Nullable<char> CodiceSpecificoAgo { get; set; }
            public System.Nullable<System.DateTime> DecorrenzaTeorica { get; set; }
            public System.Nullable<byte> CodiceTipoLiquidazione { get; set; }



            public bool IsNull()
            {
                if (this.AnniDifferimento != null && this.BaseAltraPensione != null && this.CategoriaAltraPensione != null &&
                this.CodiceSpecificoAgo != null && this.CodiceTipoLiquidazione != null && this.ContributiDifferimentoQuota != null &&
                this.Decorrenza != null && this.DecorrenzaTeorica != null && this.EtaMaturazioneRequisiti != null &&
                this.ImportoContributiLegge143271Art14 != null && this.ImportoContributiLegge37758Art24 != null && this.ImportoContributiLegge37758Art57 != null &&
                this.IntegrazioneArticolo11 != null && this.NSettimaneLegge37758Art57 != null && this.SettimaneArt24QA != null &&
                this.SettimaneArt24QB != null && this.Sospensione != null)
                    return true;
                else
                    return false;
            }
        }

        public class FondoDZ
        {
            #region public properties
            public List<DatiServizioUtile> lDatiServizioUtile { get; set; }
            public decimal? PensioneBaseAnnua { get; set; }
            public DateTime? DecorrenzaValidita { get; set; }
            public DateTime? Sospensione { get; set; }
            #endregion public properties

            public bool IsFondoNull()
            {
                if ((lDatiServizioUtile == null || lDatiServizioUtile.Count == 0) && !PensioneBaseAnnua.HasValue && !Sospensione.HasValue)
                    return true;

                return false;
            }
        }

        public class FondoPI
        {
            public decimal? RMSQuotaA { get; set; }
            public decimal? RMSQuotaB { get; set; }
            public short? NSettimaneQuotaA { get; set; }
            public short? NSettimaneQuotaB { get; set; }
            public decimal? StipendioAnnuo { get; set; }
            public decimal? StipendioBase { get; set; }
            public decimal? ImportoIIS { get; set; }
            public decimal? PensioneFacoltativaMensile { get; set; }
            public char? AttCon { get; set; }
            public decimal? PercentualeCapitalizzazione { get; set; }
            public char? CodiceMaggiorazione { get; set; }
            public decimal? PensComplRiv1_95 { get; set; }
            public short? ControCodiceRetribuzione { get; set; }
        }

        public class DatiServizioUtile
        {
            public DatiServizioUtile()
            {

            }

            public DatiServizioUtile(string quota, short? servizioUtileAA, short? servizioUtileMM, short? servizioUtileGG, decimal? retribuzionePensionabile, short? controCodiceRetributivo,
                decimal? retribuzione, decimal? quoteArt14, decimal? importoIndennitaIntegrativaSpeciale, short? servizioUtileCessazioneAA, short? servizioUtileCessazioneMM, short? servizioUtileCessazioneGG, decimal? QuotaPensioneRetributivaAnnua)
            {
                this._Quota = quota;
                this._ServizioUtileAA = servizioUtileAA;
                this._ServizioUtileMM = servizioUtileMM;
                this._ServizioUtileGG = servizioUtileGG;
                this._RetribuzionePensionabile = retribuzionePensionabile;
                this._ControCodiceRetributivo = controCodiceRetributivo;
                this._Retribuzione = retribuzione;
                this._QuoteArt14 = quoteArt14;
                this._ImportoIndennitaIntegrativaSpeciale = importoIndennitaIntegrativaSpeciale;
                this._ServizioUtileCessazioneAA = servizioUtileCessazioneAA;
                this._ServizioUtileCessazioneMM = servizioUtileCessazioneMM;
                this._ServizioUtileCessazioneGG = servizioUtileCessazioneGG;
                this._QuotaPensioneRetributivaAnnua = QuotaPensioneRetributivaAnnua;
            }

            #region private properties

            private System.Nullable<long> _IdFondo;

            private string _Quota;

            private System.Nullable<short> _ServizioUtileAA;

            private System.Nullable<short> _ServizioUtileMM;

            private System.Nullable<short> _ServizioUtileGG;

            private System.Nullable<decimal> _RetribuzionePensionabile;

            private System.Nullable<short> _ControCodiceRetributivo;

            private System.Nullable<decimal> _Retribuzione;

            private System.Nullable<decimal> _QuoteArt14;

            private System.Nullable<decimal> _ImportoIndennitaIntegrativaSpeciale;

            private System.Nullable<short> _ServizioUtileCessazioneAA;

            private System.Nullable<short> _ServizioUtileCessazioneMM;

            private System.Nullable<short> _ServizioUtileCessazioneGG;

            private System.Nullable<decimal> _QuotaPensioneRetributivaAnnua;


            #endregion private properties

            #region public properties

            public System.Nullable<long> IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }

            public string Quota { get { return _Quota; } set { _Quota = value; } }

            public System.Nullable<short> ServizioUtileAA { get { return _ServizioUtileAA; } set { _ServizioUtileAA = value; } }

            public System.Nullable<short> ServizioUtileMM { get { return _ServizioUtileMM; } set { _ServizioUtileMM = value; } }

            public System.Nullable<short> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<decimal> RetribuzionePensionabile { get { return _RetribuzionePensionabile; } set { _RetribuzionePensionabile = value; } }

            public System.Nullable<short> ControCodiceRetributivo { get { return _ControCodiceRetributivo; } set { _ControCodiceRetributivo = value; } }

            public System.Nullable<decimal> Retribuzione { get { return _Retribuzione; } set { _Retribuzione = value; } }

            public System.Nullable<decimal> QuoteArt14 { get { return _QuoteArt14; } set { _QuoteArt14 = value; } }

            public System.Nullable<decimal> ImportoIndennitaIntegrativaSpeciale { get { return _ImportoIndennitaIntegrativaSpeciale; } set { _ImportoIndennitaIntegrativaSpeciale = value; } }

            public System.Nullable<short> ServizioUtileCessazioneAA { get { return _ServizioUtileCessazioneAA; } set { _ServizioUtileCessazioneAA = value; } }

            public System.Nullable<short> ServizioUtileCessazioneMM { get { return _ServizioUtileCessazioneMM; } set { _ServizioUtileCessazioneMM = value; } }

            public System.Nullable<short> ServizioUtileCessazioneGG { get { return _ServizioUtileCessazioneGG; } set { _ServizioUtileCessazioneGG = value; } }

            public System.Nullable<decimal> QuotaPensioneRetributivaAnnua { get { return _QuotaPensioneRetributivaAnnua; } set { _QuotaPensioneRetributivaAnnua = value; } }

            #endregion public properties

        }

        public class EntityDatiFondo
        {
            #region public properties
            public short? ServizioUtileAA { get; set; }
            public short? ServizioUtileMM { get; set; }
            public decimal? RetribuzionePensionabile { get; set; }
            public int? ControCodice { get; set; }

            public FondoES fondoES { get; set; }

            #endregion public properties

            public bool IsNull()
            {
                if (!this.ServizioUtileAA.HasValue && !this.ServizioUtileMM.HasValue && !this.RetribuzionePensionabile.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class DatiArt11e14
        {
            #region private properties
            private decimal? _ContributiTotaliSupplementoDPR143271;
            private decimal? _ContribuzioneEsclusivaDPR143271;
            private decimal? _ContribuzioneEsclusiva;
            private decimal? _CCTotaliArt14;
            private DateTime? _DecDPCM;
            private decimal? _RMSArt14;
            private decimal? _RMSSent72;
            private decimal? _CCTotaliArt11;
            private decimal? _CCEsclusivaArt11;
            #endregion private properties

            #region public properties
            public decimal? ContributiTotaliSupplementoDPR143271 { get { return _ContributiTotaliSupplementoDPR143271; } set { _ContributiTotaliSupplementoDPR143271 = value; } }
            public decimal? ContribuzioneEsclusivaDPR143271 { get { return _ContribuzioneEsclusivaDPR143271; } set { _ContribuzioneEsclusivaDPR143271 = value; } }
            public decimal? ContribuzioneEsclusiva { get { return _ContribuzioneEsclusiva; } set { _ContribuzioneEsclusiva = value; } }
            public decimal? CCTotaliArt14 { get { return _CCTotaliArt14; } set { _CCTotaliArt14 = value; } }
            public DateTime? DecDPCM { get { return _DecDPCM; } set { _DecDPCM = value; } }
            public decimal? RMSArt14 { get { return _RMSArt14; } set { _RMSArt14 = value; } }
            public decimal? RMSSent72 { get { return _RMSSent72; } set { _RMSSent72 = value; } }
            public decimal? CCTotaliArt11 { get { return _CCTotaliArt11; } set { _CCTotaliArt11 = value; } }
            public decimal? CCEsclusivaArt11 { get { return _CCEsclusivaArt11; } set { _CCEsclusivaArt11 = value; } }
            #endregion public properties

            public bool IsNull()
            {
                if (!this._ContributiTotaliSupplementoDPR143271.HasValue && !this._ContribuzioneEsclusivaDPR143271.HasValue && !this._CCTotaliArt14.HasValue &&
                    !this._ContribuzioneEsclusiva.HasValue && !this._DecDPCM.HasValue && !this._RMSArt14.HasValue && !this._RMSSent72.HasValue &&
                    !this._CCTotaliArt11.HasValue && !this._CCEsclusivaArt11.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public class CrossDataRecipient
        {
            /// <summary>
            /// // Contenitore di proprietà prelevabili e/o non prelevabili da AggPeco non appartenenti a dati Contributivi e /o Retributivi
            /// </summary>

            #region private properties

            // fondo VL
            private bool? _LavoratorePrecoce;

            // fondo FS PT
            private decimal? _PensioneAnnuaLorda;
            private decimal? _PensioneAnnuaLorda707;
            private List<Entity.DatiCalcolo707.DatiServizioUtile707> _lDatiServizioUtile707;
            private short? _ServizioUtileDirittoAA;
            private short? _ServizioUtileDirittoMM;
            private short? _ServizioUtileDirittoGG;
            private decimal? _CoefficienteTrasformazione;
            private long? _IdPensione;
            private decimal? _PensioneAnnuaLorda214;

            // fondo FS
            private decimal? _RMSSenzaLegge33670QA;

            //fondo GAS
            private byte? _CodiceTipoLiquidazione;
            private DateTime? _DecorrenzaDatiAgo;
            private DateTime? _SospensioneAGO;
            private short? _SettimaneAnzianitaEsclusiva;
            private int? _AnniDifferimento;
            private byte? _EtaMaturazioneRequisiti;
            private char? _CodiceSpecificoAgo;

            //fondo ES


            // trasversale
            private List<DatiServizioUtile> _lDatiServizioUtile;
            private bool _RiduzioneRetributiva;
            private decimal? _RiduzioneRetributivaPercentuale;

            //Ante armonizzazione Common
            private List<DatiServizioUtile> _lServizioUtileAnteArmonizzazione;
            private decimal? _RetrPondAnnuaAGOLimite;

            //Ante armonizzazione TT
            private decimal? _PensioneMensileAl53;
            private decimal? _ElementiAccessori;
            private decimal? _RetribuzioneSupplementi;

            #endregion private properties

            #region public properties

            public bool? LavoratorePrecoce { get { return _LavoratorePrecoce; } set { _LavoratorePrecoce = value; } }
            public decimal? PensioneAnnuaLorda { get { return _PensioneAnnuaLorda; } set { _PensioneAnnuaLorda = value; } }
            public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
            public short? ServizioUtileDirittoAA { get { return _ServizioUtileDirittoAA; } set { _ServizioUtileDirittoAA = value; } }
            public short? ServizioUtileDirittoMM { get { return _ServizioUtileDirittoMM; } set { _ServizioUtileDirittoMM = value; } }
            public short? ServizioUtileDirittoGG { get { return _ServizioUtileDirittoGG; } set { _ServizioUtileDirittoGG = value; } }
            public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public List<DatiServizioUtile> lDatiServizioUtile { get { return _lDatiServizioUtile; } set { _lDatiServizioUtile = value; } }
            public List<Entity.DatiCalcolo707.DatiServizioUtile707> LDatiServizioUtile707 { get { return _lDatiServizioUtile707; } set { _lDatiServizioUtile707 = value; } }
            public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }
            public decimal? RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }
            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
            public byte? CodiceTipoLiquidazione { get { return _CodiceTipoLiquidazione; } set { _CodiceTipoLiquidazione = value; } }
            public DateTime? DecorrenzaDatiAgo { get { return _DecorrenzaDatiAgo; } set { _DecorrenzaDatiAgo = value; } }
            public DateTime? SospensioneAGO { get { return _SospensioneAGO; } set { _SospensioneAGO = value; } }
            public short? SettimaneAnzianitaEsclusiva { get { return _SettimaneAnzianitaEsclusiva; } set { _SettimaneAnzianitaEsclusiva = value; } }
            public int? AnniDifferimento { get { return _AnniDifferimento; } set { _AnniDifferimento = value; } }
            public byte? EtaMaturazioneRequisiti { get { return _EtaMaturazioneRequisiti; } set { _EtaMaturazioneRequisiti = value; } }
            public char? CodiceSpecificoAgo { get { return _CodiceSpecificoAgo; } set { _CodiceSpecificoAgo = value; } }
            public decimal? PensioneBaseAnnua { get; set; }
            public DateTime? Sospensione { get; set; }

            public decimal? IntegrazioneArticolo11 { get; set; }

            public decimal? BaseAltraPensione { get; set; }

            public short? CategoriaAltraPensione { get; set; }

            public decimal? ImportoContributiLegge37758Art24 { get; set; }

            public decimal? ImportoContributiLegge37758Art57 { get; set; }

            public DateTime? Decorrenza { get; set; }

            public decimal? ContributiDifferimentoQuota { get; set; }

            public int? SettimaneArt24QB { get; set; }

            public int? SettimaneArt24QA { get; set; }

            public int? NSettimaneLegge37758Art57 { get; set; }

            public decimal? ImportoContributiLegge143271Art14 { get; set; }

            public DateTime? DecorrenzaTeorica { get; set; }

            public decimal? RMSQuotaA { get; set; }
            public decimal? RMSQuotaB { get; set; }
            public short? NSettimaneQuotaA { get; set; }
            public short? NSettimaneQuotaB { get; set; }
            public decimal? StipendioAnnuo { get; set; }
            public decimal? StipendioBase { get; set; }
            public decimal? ImportoIIS { get; set; }
            public decimal? PensioneFacoltativaMensile { get; set; }
            public char? AttCon { get; set; }
            public decimal? PercentualeCapitalizzazione { get; set; }
            public char? CodiceMaggiorazione { get; set; }
            public decimal? PensComplRiv1_95 { get; set; }

            public bool IsQuotaDPresente { get; set; }

            #region Ante Armonizzazione
            public List<DatiServizioUtile> LServizioUtileAnteArmonizzazione { get { return _lServizioUtileAnteArmonizzazione; } set { _lServizioUtileAnteArmonizzazione = value; } }
            public decimal? RetrPondAnnuaAGOLimite { get { return _RetrPondAnnuaAGOLimite; } set { _RetrPondAnnuaAGOLimite = value; } }

            // Fondo TT
            public decimal? PensioneMensileAl53 { get { return _PensioneMensileAl53; } set { _PensioneMensileAl53 = value; } }
            public decimal? ElementiAccessori { get { return _ElementiAccessori; } set { _ElementiAccessori = value; } }
            public decimal? RetribuzioneSupplementi { get { return _RetribuzioneSupplementi; } set { _RetribuzioneSupplementi = value; } }
            #endregion Ante Armonizzazione

            public decimal? PensioneAnnuaLorda214 { get { return _PensioneAnnuaLorda214; } set { _PensioneAnnuaLorda214 = value; } }

            #endregion public properties
        }

        public enum TipoCalcolo
        {
            NonValido,
            Contributivo,
            Retributivo,
            Misto,
            RetributivoMonti
        };

        public class DatiAnte67
        {
            public decimal? ContributiLegge37758Art57Periodo1 { get; set; }
            public DateTime? DecorrenzaLegge37758Art57Pre67Periodo1 { get; set; }
            public decimal? ContributiLegge37758Art57Periodo2 { get; set; }
            public DateTime? DecorrenzaLegge37758Art57Pre67Periodo2 { get; set; }
            public decimal? ContributiLegge37758Art57Periodo3 { get; set; }
            public DateTime? DecorrenzaLegge37758Art57Pre67Periodo3 { get; set; }
            public decimal? ContributiLegge37758Art24 { get; set; }
            public DateTime? DecorrenzaArticolo24 { get; set; }
            public char? CodicePensioneInPagamentoPre67 { get; set; }
            public decimal? ImportoInPagamentoPre67 { get; set; }
            public decimal? PensioneFondoAl67 { get; set; }

            //public override bool Equals(object obj)
            //{
            //    DatiAnte67 datiAnte67 = (DatiAnte67)obj;
            //    if (this.CodicePensioneInPagamentoPre67 == datiAnte67.CodicePensioneInPagamentoPre67 &&
            //        this.ContributiLegge37758Art24 == datiAnte67.ContributiLegge37758Art24 &&
            //        this.ContributiLegge37758Art57Periodo1 == datiAnte67.ContributiLegge37758Art57Periodo1 &&
            //        this.ContributiLegge37758Art57Periodo2 == datiAnte67.ContributiLegge37758Art57Periodo2 &&
            //        this.ContributiLegge37758Art57Periodo3 == datiAnte67.ContributiLegge37758Art57Periodo3 &&
            //        this.DecorrenzaArticolo24 == datiAnte67.DecorrenzaArticolo24 &&
            //        this.DecorrenzaLegge37758Art57Pre67Periodo1 == datiAnte67.DecorrenzaLegge37758Art57Pre67Periodo1 &&
            //        this.DecorrenzaLegge37758Art57Pre67Periodo2 == datiAnte67.DecorrenzaLegge37758Art57Pre67Periodo2 &&
            //        this.DecorrenzaLegge37758Art57Pre67Periodo3 == datiAnte67.DecorrenzaLegge37758Art57Pre67Periodo3 &&
            //        this.ImportoInPagamentoPre67 == datiAnte67.ImportoInPagamentoPre67 &&
            //        this.PensioneFondoAl67 == datiAnte67.PensioneFondoAl67)
            //        return true;
            //    else
            //        return false;
            //}

            public bool IsNull()
            {
                return Utility.PropertiesAreAllNull(this);
            }



        }

        public class DatiSL33670
        {
            public decimal? CCArt14SenzaLegge33670 { get; set; }
            public int? NSettimaneAnzianitaTotaliSenzaLegge33670 { get; set; }
            public decimal? RMSSenzaLegge33670QB { get; set; }
            public decimal? RMSSenzaLegge33670QA { get; set; }
            public decimal? ContributiSupplementoAgo { get; set; }
            public decimal? ContributiSupplementoFondo { get; set; }
            public int? NSettimaneSenzaLegge33670Art24QuotaA { get; set; }
            public int? NSettimaneSenzaLegge33670Art57QuotaA { get; set; }
            public decimal? ContributiTotaliSenzaLegge33670 { get; set; }

            public bool IsNull()
            {
                return Utility.PropertiesAreAllNull(this);
            }

            public override bool Equals(object obj)
            {
                DatiSL33670 sl33670 = (DatiSL33670)obj;
                if (this.CCArt14SenzaLegge33670 == sl33670.CCArt14SenzaLegge33670 &&
                    this.ContributiSupplementoAgo == sl33670.ContributiSupplementoAgo &&
                    this.ContributiSupplementoFondo == sl33670.ContributiSupplementoFondo &&
                    this.ContributiTotaliSenzaLegge33670 == sl33670.ContributiTotaliSenzaLegge33670)
                    return true;
                else return false;
            }
        }

        public class DatiAgoAltraPensione
        {
            #region private properties

            private short? _SetAnzTotAltraPensione;
            private decimal? _BaseAltraPensione;
            private string _CategoriaAltraPensione;
            private int? _CertificatoAltraPensione;
            private decimal? _RmsImpAltraPensione;
            private DateTime? _DecorrenzaAltraPensione;
            private short? _RevAltraPensione;
            private byte? _TipoLiquidazione;
            private DateTime? _DecorrenzaPrimoSupplemento;
            private decimal? _ImpContribPrimoSupplemento;
            private DateTime? _DecorrenzaSecondoSupplemento;
            private decimal? _ImpContribSecondoSupplemento;

            #endregion private properties

            #region public properties
            public short? SetAnzTotAltraPensione
            {
                get { return _SetAnzTotAltraPensione; }
                set { _SetAnzTotAltraPensione = value; }
            }
            public decimal? BaseAltraPensione
            {
                get { return _BaseAltraPensione; }
                set { _BaseAltraPensione = value; }
            }
            public string CategoriaAltraPensione
            {
                get { return _CategoriaAltraPensione; }
                set { _CategoriaAltraPensione = value; }
            }
            public int? CertificatoAltraPensione
            {
                get { return _CertificatoAltraPensione; }
                set { _CertificatoAltraPensione = value; }
            }
            public decimal? RmsImpAltraPensione
            {
                get { return _RmsImpAltraPensione; }
                set { _RmsImpAltraPensione = value; }
            }
            public DateTime? DecorrenzaAltraPensione
            {
                get { return _DecorrenzaAltraPensione; }
                set { _DecorrenzaAltraPensione = value; }
            }
            public short? RevAltraPensione
            {
                get { return _RevAltraPensione; }
                set { _RevAltraPensione = value; }
            }
            public byte? TipoLiquidazione
            {
                get { return _TipoLiquidazione; }
                set { _TipoLiquidazione = value; }
            }
            public DateTime? DecorrenzaPrimoSupplemento
            {
                get { return _DecorrenzaPrimoSupplemento; }
                set { _DecorrenzaPrimoSupplemento = value; }
            }
            public decimal? ImpContribPrimoSupplemento
            {
                get { return _ImpContribPrimoSupplemento; }
                set { _ImpContribPrimoSupplemento = value; }
            }
            public DateTime? DecorrenzaSecondoSupplemento
            {
                get { return _DecorrenzaSecondoSupplemento; }
                set { _DecorrenzaSecondoSupplemento = value; }
            }
            public decimal? ImpContribSecondoSupplemento
            {
                get { return _ImpContribSecondoSupplemento; }
                set { _ImpContribSecondoSupplemento = value; }
            }
            #endregion public properties

            #region public methods

            public bool IsNull()
            {
                if (this._BaseAltraPensione.HasValue || !string.IsNullOrEmpty(this._CategoriaAltraPensione) || this._CertificatoAltraPensione.HasValue || this._DecorrenzaAltraPensione.HasValue ||
                    this._DecorrenzaPrimoSupplemento.HasValue || this._DecorrenzaSecondoSupplemento.HasValue || this._ImpContribPrimoSupplemento.HasValue || this._ImpContribSecondoSupplemento.HasValue ||
                    this._RevAltraPensione.HasValue || this._RmsImpAltraPensione.HasValue || this._SetAnzTotAltraPensione.HasValue || this._TipoLiquidazione.HasValue)
                    return false;

                return true;
            }

            #endregion public methods
        }


        #endregion nested class
    }
}

