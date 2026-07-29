using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaFamiliari
    {
        #region public members
        public static void GetFamiliariByDatiPensione(ref ContenitoreObject contenitore, out List<AreaFamiliare> listaFamiliari, out List<Entity.Anagrafica> listaAnagrafiche)
        {
            listaFamiliari = new List<AreaFamiliare>();
            listaAnagrafiche = new List<INPS.Pensioni.Liquidazione.Entity.Anagrafica>();

            if (contenitore.ListaFamiliari != null && contenitore.ListaFamiliari.Count > 0)
            {
                foreach (GestioneFamiliari.Familiare familiareDB in contenitore.ListaFamiliari)
                {
                    AreaFamiliare familiare = new AreaFamiliare();
                    familiare.Familiare = new GestioneFamiliari.Familiare();
                    Utility.ValorizzaOggetti(familiareDB, familiare.Familiare);
                    if (contenitore.ListaCodMaggFamiliari != null)
                        familiare.ElencoCodMaggFamiliari = contenitore.ListaCodMaggFamiliari.FindAll(x =>
                            x.IdAnagrafica == familiareDB.IdAnagrafica &&
                            x.IdPensione == familiareDB.IdPensione);
                    listaFamiliari.Add(familiare);
                }
            }
            if (contenitore.ListaAnagraficaFamiliari != null)
            {
                foreach (GestioneAnagrafica.DatiAnagrafici anagraficaDB in contenitore.ListaAnagraficaFamiliari)
                {
                    Entity.Anagrafica anagrafica = new INPS.Pensioni.Liquidazione.Entity.Anagrafica();
                    Utility.ValorizzaOggetti(anagraficaDB, anagrafica);
                    listaAnagrafiche.Add(anagrafica);
                }
            }
            var tipologiaDomanda = Utility.GetTipoDomanda(contenitore.DatiPensione.Gruppo, contenitore.DatiPensione.Prodotto);
            if (tipologiaDomanda == Utility.TipoDomanda.Superstiti || tipologiaDomanda == Utility.TipoDomanda.RipristinoSuperstiti || tipologiaDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti)
            {
                string codFiscaleTitolare = contenitore.DatiAnagraficiTitolare.CodiceFiscale;
                if ((contenitore.ListaFamiliari != null && contenitore.ListaFamiliari.Count > 0 && !contenitore.ListaFamiliari.Any(x => x.CodiceFiscale == codFiscaleTitolare)) ||
                    contenitore.ListaFamiliari == null || contenitore.ListaFamiliari.Count == 0)
                {
                    AreaFamiliare familiare = new AreaFamiliare();
                    familiare.Familiare = new GestioneFamiliari.Familiare();
                    familiare.Familiare.CodiceFiscale = contenitore.DatiAnagraficiTitolare.CodiceFiscale;
                    familiare.Familiare.IdAnagrafica = contenitore.DatiAnagraficiTitolare.Id;
                    Entity.Anagrafica anagrafica = new INPS.Pensioni.Liquidazione.Entity.Anagrafica();
                    Utility.ValorizzaOggetti(contenitore.DatiAnagraficiTitolare, anagrafica);
                    listaFamiliari.Insert(0, familiare);
                    listaAnagrafiche.Insert(0, anagrafica);
                }
            }

            if (Utility.IsDomandaRipristinoOrRiliquidazione(contenitore.DatiPensione) && tipologiaDomanda != Utility.TipoDomanda.RipristinoSuperstiti && tipologiaDomanda != Utility.TipoDomanda.RiliquidazioneSuperstiti)
            {
                string codFiscaleTitolare = contenitore.DatiAnagraficiTitolare.CodiceFiscale;
                if ((contenitore.ListaFamiliari != null && contenitore.ListaFamiliari.Count > 0 && contenitore.ListaFamiliari.Any(x => x.CodiceFiscale == codFiscaleTitolare)))
                {
                    listaFamiliari.RemoveAll(x => x.Familiare.CodiceFiscale == codFiscaleTitolare);
                    listaAnagrafiche.RemoveAll(x => x.CodiceFiscale == codFiscaleTitolare);
                }
            }
        }

        public static void StoreFamiliari(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, string cfFamiliareAttuale, List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari,
            List<string> elencoFamiliariDaRimuovere, List<Entity.Anagrafica> elencoAnagrafiche, out string messaggioInfo)
        {
            messaggioInfo = string.Empty;
            List<long> IdAnagraficheDaRimuovere = new List<long>();
            List<GestioneFamiliari.Familiare> elencoFamiliariDB = null;
            List<GestioneFamiliari.CodMaggFamiliari> elencoCodMaggFamiliariDB = null;

            List<GestioneAnagrafica.DatiAnagrafici> elencoAnagraficheDB = null;
            if (elencoAnagrafiche != null)
            {
                elencoAnagraficheDB = new List<GestioneAnagrafica.DatiAnagrafici>();
                foreach (Entity.Anagrafica anagrafica in elencoAnagrafiche)
                {
                    GestioneAnagrafica.DatiAnagrafici anagraficaDB = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(anagrafica, anagraficaDB);
                    elencoAnagraficheDB.Add(anagraficaDB);
                }
            }
            if (elencoFamiliari != null)
            {
                if (elencoAnagraficheDB == null)
                    elencoAnagraficheDB = new List<GestioneAnagrafica.DatiAnagrafici>();

                elencoFamiliariDB = new List<GestioneFamiliari.Familiare>();
                elencoCodMaggFamiliariDB = new List<GestioneFamiliari.CodMaggFamiliari>();
                foreach (GestioneAreaFamiliari.AreaFamiliare f in elencoFamiliari)
                {
                    GestioneAnagrafica.DatiAnagrafici dati = new GestioneAnagrafica.DatiAnagrafici();
                    dati = elencoAnagraficheDB.Find(s => s.CodiceFiscale == f.Familiare.CodiceFiscale);
                    GestioneAnagrafica.SalvaAnagrafica(dati);
                    f.Familiare.IdAnagrafica = dati.Id;
                    f.Familiare.IdPensione = datiPensione.Id;
                    elencoFamiliariDB.Add(f.Familiare);
                    if (f.ElencoCodMaggFamiliari != null && f.ElencoCodMaggFamiliari.Count > 0)
                    {
                        foreach (GestioneFamiliari.CodMaggFamiliari codMagg in f.ElencoCodMaggFamiliari)
                        {
                            codMagg.IdAnagrafica = f.Familiare.IdAnagrafica;
                            codMagg.IdPensione = f.Familiare.IdPensione;
                            elencoCodMaggFamiliariDB.Add(codMagg);
                        }
                    }
                }
            }
            foreach (string cf in elencoFamiliariDaRimuovere)
            {
                long IdAnagrafica = 0;
                GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(cf, out IdAnagrafica);
                IdAnagraficheDaRimuovere.Add(IdAnagrafica);
            }

            GestioneFamiliari.StoreFamiliari(datiPensione, isRiaperturaDomanda, cfFamiliareAttuale, elencoFamiliariDB, elencoCodMaggFamiliariDB, IdAnagraficheDaRimuovere, elencoFamiliariDaRimuovere, out messaggioInfo);
        }

        public static void DeleteFamiliariByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = null;
            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out datiQuadroFamiliari);

            INPS.Pensioni.Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            List<GestioneFamiliari.Familiare> familiariAttualiDB = null;
            List<GestioneAnagrafica.DatiAnagrafici> anagraficheAttualiDB = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out familiariAttualiDB, out anagraficheAttualiDB);

            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
            bool aggiornaQuadroRedditi = ControlsAggiornamentoQuadroRedditiByIdPensione(datiPensione, familiariAttualiDB, anagraficheAttualiDB, out datiQuadroRedditi);

            List<GestioneComponenteFamiliare.ComponenteFamiliare> listaComponentiFamiliari = null;
            GestioneComponenteFamiliare.GetComponenteFamiliareByIdPensione(datiPensione.Id, out listaComponentiFamiliari);
            if (listaComponentiFamiliari != null && listaComponentiFamiliari.Count > 0)
                throw new DNA.DnaValidationException("Dati Familiari: Impossibile eliminare i familiari presenti tra i Componenti Familiari del record Dati No Calcolo");

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            //ENG - Spacchettate SOPGI
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneFamiliari.Familiare titolare = null;
            GestioneAnagrafica.DatiAnagrafici anagraficaTitolare = null;
            if (areaTitolare != null && areaTitolare.Anagrafica != null)
            {
                RecoveryFamiliareTitolare(areaTitolare.Anagrafica.CodiceFiscale, familiariAttualiDB, anagraficheAttualiDB, out titolare, out anagraficaTitolare);
                if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                    || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))
                    || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || Utility.IsDomandaSpacchettamentoSR(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                    familiariAttualiDB.FindAll(x => x.CodiceFiscale == areaTitolare.Anagrafica.CodiceFiscale).ForEach(x => x.Confermato = true);
            }

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDetrazioniContitolare.EliminaDetrazioniByIdPensione(datiPensione.Id);
                GestioneFamiliari.EliminaAllCodMaggiorazioneFamiliari(datiPensione.Id);
                GestioneFamiliari.DeleteAllRichiestaRicercaDomandeANF(datiPensione.Id);
                GestioneFamiliari.DeleteAllFamiliari(datiPensione.Id);

                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && titolare != null && anagraficaTitolare != null)
                    GestioneFamiliari.SalvaFamiliare(titolare, null, anagraficaTitolare, null, datiPensione.Id, datiPensione.SiglaCategoria);

                if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa)
                    || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda))
                    || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                    datiQuadroFamiliari.TabFamiliari = 2;
                else if (datiQuadroFamiliari.Tipo == 1)
                    datiQuadroFamiliari.TabFamiliari = 1;
                //modifica per prepopolamento familiari
                else if (datiQuadroFamiliari.Tipo == 2)
                {
                    if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                        datiQuadroFamiliari.TabFamiliari = 0;
                    else
                    {
                        if (areaTitolare != null && areaTitolare.ElencoStatiCivili != null && areaTitolare.ElencoStatiCivili.Count > 0)
                        {
                            //areaTitolare.ElencoStatiCivili = areaTitolare.ElencoStatiCivili.OrderBy(x => x.Decorrenza).ToList<GestioneAnagrafica.DatiStatoCivile>();
                            //if (areaTitolare.ElencoStatiCivili[areaTitolare.ElencoStatiCivili.Count - 1].Codice == 2)
                            if (areaTitolare.ElencoStatiCivili.FindAll(x => x.Codice == '2' || x.Codice == '7').Count != 0)
                                datiQuadroFamiliari.TabFamiliari = 0;
                            else
                            {
                                datiQuadroFamiliari.Tipo = 1;
                                datiQuadroFamiliari.TabFamiliari = 1;
                            }
                        }
                        else
                        {
                            datiQuadroFamiliari.Tipo = 1;
                            datiQuadroFamiliari.TabFamiliari = 1;
                        }
                    }
                }
                GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, datiQuadroFamiliari);

                if (!Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                {
                    if (aggiornaQuadroRedditi)
                    {
                        switch (datiQuadroRedditi.Tipo.Value)
                        {
                            case 1:
                                datiQuadroRedditi.Tipo = 2;
                                datiQuadroRedditi.TabRedditi = 1;
                                break;
                            case 2:
                                datiQuadroRedditi.TabRedditi = 0;
                                break;
                            default:
                                break;
                        }
                        GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                    }
                }

                transactionScope.Complete();
            }
        }

        public static void GetAreaDecodificaByDatiPensione(ref ContenitoreObject contenitore, ref ContenitoreDecodifica contenitoreDecodifica, out AreaDecFam areaDecodifica)
        {
            areaDecodifica = new AreaDecFam();

            List<BLCommon.GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliareBL = contenitoreDecodifica.ElencoSiglaFamiliare != null ? contenitoreDecodifica.ElencoSiglaFamiliare.ToList() : null;
            FiltraSigleFamiliari(contenitore.TipoAppartenenza, contenitore.DatiPensione, ref elencoSiglaFamiliareBL);

            if (elencoSiglaFamiliareBL != null && elencoSiglaFamiliareBL.Count > 0)
            {
                if (Utility.IsDomandaVESO92(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsDomandaESOAMB(contenitore.DatiPensione.SiglaCategoria) || Utility.IsDomandaESPA(contenitore.DatiPensione.SiglaCategoria) ||
                    Utility.IsRenditaFacoltativa(contenitore.DatiPensione) || Utility.IsRenditaCasalinghe(contenitore.DatiPensione))
                    areaDecodifica.ElencoSiglaFamiliare = elencoSiglaFamiliareBL.Where(x => x.Id == "C").Select(x => new AreaDecFam.DatiSiglaFamiliare(x)).ToList();
                else
                    areaDecodifica.ElencoSiglaFamiliare = elencoSiglaFamiliareBL.OrderBy(x => x.Descrizione).Select(x => new AreaDecFam.DatiSiglaFamiliare(x)).ToList();
            }
            if (contenitoreDecodifica.ElencoCodMaggiorazioneFamiliari != null && contenitoreDecodifica.ElencoCodMaggiorazioneFamiliari.Count > 0)
            {
                areaDecodifica.ElencoCodMaggFamiliari = new List<AreaDecFam.DatiCodMaggFamiliari>();
                foreach (BLCommon.GestioneDecodifica.CodMaggiorazioneFamiliari codMaggiorazioneFamiliariBL in contenitoreDecodifica.ElencoCodMaggiorazioneFamiliari)
                    areaDecodifica.ElencoCodMaggFamiliari.Add(new AreaDecFam.DatiCodMaggFamiliari(codMaggiorazioneFamiliariBL));
            }
        }

        public static void RicercaDomandeANF(ref ContenitoreObject contenitore, bool isRiaperturaDomanda, string matricolaOperatore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Utility.TipoAppartenenza? tipoAppartenenza = contenitore.TipoAppartenenza;
            AreaTitolare areaTitolare = contenitore.DatiAreaTitolare;
            List<GestioneFamiliari.Familiare> listaFamiliari = contenitore.ListaFamiliari;
            if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.Any(x => x.Confermato))
            {
                List<GestioneFamiliari.Familiare> listaFamiliariConfermati = listaFamiliari.FindAll(x => x.Confermato);
                List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggiorazione = contenitore.ListaCodMaggFamiliari;
                if (listaCodMaggiorazione != null)
                {
                    List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichiesteRicerca = contenitore.ListaRichiesteRicercaDomandeANF;
                    foreach (GestioneFamiliari.Familiare familiare in listaFamiliariConfermati)
                    {
                        byte? codMagg = null;
                        DateTime? dataSistema = null;
                        switch (tipoAppartenenza)
                        {
                            case Utility.TipoAppartenenza.AGO:
                                codMagg = 2;
                                dataSistema = Utility.DataSistemaAgo;
                                break;
                            case Utility.TipoAppartenenza.CI:
                                codMagg = 1;
                                dataSistema = Utility.DataSistemaCi;
                                break;
                        }
                        if (listaCodMaggiorazione.Where(x => x.IdAnagrafica == familiare.IdAnagrafica).Any(x => x.CodiceMaggiorazione == codMagg))
                        {
                            if (listaRichiesteRicerca != null && listaRichiesteRicerca.Count > 0 && listaRichiesteRicerca.Exists(x => x.IdAnagrafica == familiare.IdAnagrafica) &&
                                listaRichiesteRicerca.FirstOrDefault(x => x.IdAnagrafica == familiare.IdAnagrafica).DataRichiesta.Date == dataSistema.GetValueOrDefault().Date)
                                continue;

                            string guidANF;
                            if (!GestioneANF.RicercaDomandeANFByCodiceFiscale(contenitore.DatiPensione.NDomus.ToString(), familiare.CodiceFiscale, matricolaOperatore, out guidANF, out messaggioVideo))
                                throw new DNA.DnaValidationException(messaggioVideo);
                            if (!string.IsNullOrEmpty(guidANF))
                            {
                                GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta = new GestioneFamiliari.DatiRichiestaRicercaDomandeANF();
                                richiesta.IdPensione = contenitore.DatiPensione.Id;
                                richiesta.IdAnagrafica = familiare.IdAnagrafica;
                                richiesta.Guid = guidANF;
                                richiesta.DataRichiesta = dataSistema.GetValueOrDefault();
                                GestioneFamiliari.SalvaRichiestaRicercaDomandaANF(richiesta);
                            }
                        }
                        else
                            GestioneFamiliari.DeleteRichiestaRicercaDomandeANF(contenitore.DatiPensione.Id, familiare.IdAnagrafica);
                    }
                    contenitore.ListaRichiesteRicercaDomandeANF_GetEffettuata = false;
                }
            }
        }

        public static void RispostaRicercaDomandeANFSingola(ref ContenitoreObject contenitore, List<AreaFamiliare> listaFamiliari, string cfFamiliareAttuale,
            string matricolaOperatore, out GestioneFamiliari.ConsultazioneUnificataANF consultazioneANF, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            consultazioneANF = null;

            List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichiesteRicerca = contenitore.ListaRichiesteRicercaDomandeANF;
            if (listaRichiesteRicerca != null && listaRichiesteRicerca.Count > 0)
            {
                if (listaFamiliari != null && listaFamiliari.Count > 0)
                {
                    AreaFamiliare familiare = listaFamiliari.FirstOrDefault(x => x.Familiare.CodiceFiscale == cfFamiliareAttuale);
                    if (familiare != null)
                    {
                        GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiestaRicerca = listaRichiesteRicerca.FirstOrDefault(x => x.IdAnagrafica == familiare.Familiare.IdAnagrafica);
                        if (richiestaRicerca != null)
                        {
                            string rispostaConsultazione = string.Empty;
                            if (!GestioneANF.RichiediRispostaById(contenitore.DatiPensione.NDomus.ToString(), familiare.Familiare.CodiceFiscale, richiestaRicerca.Guid, matricolaOperatore, out rispostaConsultazione, out messaggioVideo))
                                return;
                            if (!GestioneFamiliari.ControllaRispostaANF(rispostaConsultazione, out consultazioneANF, out messaggioVideo))
                                return;
                        }
                    }
                }
            }
        }
        #endregion public members

        #region private members
        private static bool ControlsAggiornamentoQuadroRedditiByIdPensione(GestionePensione.DatiPensione datiPensione, List<GestioneFamiliari.Familiare> familiariAttualiDB, List<GestioneAnagrafica.DatiAnagrafici> anagraficheAttualiDB,
            out GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi)
        {
            datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);

            if (datiQuadroRedditi != null && datiQuadroRedditi.TabRedditi.HasValue && datiQuadroRedditi.TabRedditi.Value == 2)
            {
                if (familiariAttualiDB != null && familiariAttualiDB.Count > 0)
                    return true;
            }

            return false;
        }

        private static void FiltraSigleFamiliari(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione,
            ref List<BLCommon.GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliareBL)
        {
            if (tipoAppartenenza.HasValue && elencoSiglaFamiliareBL != null && elencoSiglaFamiliareBL.Count > 0)
            {
                GestioneControlliDinamici.ControlloDinamico abilitazioneMemo33 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo33" + tipoAppartenenza.Value, out abilitazioneMemo33);

                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.AGO:
                        string codCat = datiPensione.GetCodCategoria();
                        codCat = codCat.PadLeft(4, '0');


                        for (int i = 0; i < elencoSiglaFamiliareBL.Count; i++)
                        {
                            bool remove = false;
                            BLCommon.GestioneDecodifica.SiglaFamiliare s = elencoSiglaFamiliareBL[i];
                            if (s.Id == "R" ||
                                ((s.Id == "Z" || s.Id == "K" || s.Id == "W") && !Utility.IsDomandaAGOReversibile(datiPensione)) ||
                                ((s.Id == "G" || s.Id == "P" || s.Id == "Y") && Utility.IsDomandaAGOReversibile(datiPensione)) ||
                                ((s.Id == "X" || s.Id == "B" || s.Id == "D") && codCat != "0072"))
                                remove = true;
                            else if (s.Id == "N")
                            {
                                if (codCat == "0037" || codCat == "0040")
                                {
                                    try
                                    {
                                        s.Descrizione = s.Descrizione.Substring(s.Descrizione.IndexOf('/') + 1);
                                    }
                                    catch (Exception)
                                    {
                                        // Eccezione ignorata
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        s.Descrizione = s.Descrizione.Substring(0, s.Descrizione.IndexOf('/'));
                                    }
                                    catch (Exception)
                                    {
                                        // Eccezione ignorata
                                    }
                                }
                            }

                            if (remove)
                            {
                                elencoSiglaFamiliareBL.RemoveAt(i);
                                i--;
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.FS:
                        List<BLCommon.GestioneDecodifica.SiglaFamiliare> app = elencoSiglaFamiliareBL.ToList();
                        foreach (BLCommon.GestioneDecodifica.SiglaFamiliare siglaFamiliareBL in app)
                        {
                            switch (siglaFamiliareBL.Id)
                            {
                                case "K":
                                case "W":
                                case "Z":
                                    if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                                        elencoSiglaFamiliareBL.Remove(siglaFamiliareBL);
                                    break;
                                case "R":
                                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && !datiPensione.SiglaCategoria.Trim().ToUpperInvariant().StartsWith("S"))
                                        elencoSiglaFamiliareBL.Remove(siglaFamiliareBL);
                                    break;
                                default:
                                    // DO NOTHING
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private static void RecoveryFamiliareTitolare(string codiceFiscaleTitolare, List<GestioneFamiliari.Familiare> elencoFamiliari, List<GestioneAnagrafica.DatiAnagrafici> anagraficheFamiliari,
            out GestioneFamiliari.Familiare titolare, out GestioneAnagrafica.DatiAnagrafici anagraficaTitolare)
        {
            titolare = null;
            anagraficaTitolare = null;
            long idAnagrafica;
            if (elencoFamiliari != null && elencoFamiliari.Count > 0 && !string.IsNullOrEmpty(codiceFiscaleTitolare))
            {
                titolare = elencoFamiliari.Find(x => x.CodiceFiscale == codiceFiscaleTitolare);
                if (titolare != null)
                {
                    idAnagrafica = titolare.IdAnagrafica;
                    if (idAnagrafica != 0 && anagraficheFamiliari != null && anagraficheFamiliari.Count > 0)
                        anagraficaTitolare = anagraficheFamiliari.Find(x => x.Id == idAnagrafica);
                    titolare.Confermato = false;
                    titolare.ScadenzaRevisioneSanitaria = null;
                }
            }
        }
        #endregion private members

        #region nested classes
        public class AreaFamiliare
        {
            #region private properties
            private GestioneFamiliari.Familiare _Familiare;

            private List<GestioneFamiliari.CodMaggFamiliari> _ElencoCodMaggFamiliari;
            #endregion private properties

            #region public properties
            public GestioneFamiliari.Familiare Familiare { get { return _Familiare; } set { _Familiare = value; } }

            public List<GestioneFamiliari.CodMaggFamiliari> ElencoCodMaggFamiliari { get { return _ElencoCodMaggFamiliari; } set { _ElencoCodMaggFamiliari = value; } }
            #endregion public properties

        }

        public class AreaDecFam
        {
            #region private properties
            private List<DatiSiglaFamiliare> _ElencoSiglaFamiliare;
            private List<DatiCodMaggFamiliari> _ElencoCodMaggFamiliari;
            #endregion private properties

            #region public properties
            public List<DatiSiglaFamiliare> ElencoSiglaFamiliare { get { return _ElencoSiglaFamiliare; } set { _ElencoSiglaFamiliare = value; } }
            public List<DatiCodMaggFamiliari> ElencoCodMaggFamiliari { get { return _ElencoCodMaggFamiliari; } set { _ElencoCodMaggFamiliari = value; } }
            #endregion public properties

            #region nested classes
            public class DatiSiglaFamiliare
            {
                public DatiSiglaFamiliare()
                {
                }

                internal DatiSiglaFamiliare(BLCommon.GestioneDecodifica.SiglaFamiliare siglaFamiliare)
                {
                    this._Id = siglaFamiliare.Id;
                    this._Descrizione = siglaFamiliare.Descrizione;
                    this._TipoUnione = siglaFamiliare.TipoUnione;
                }

                #region private properties
                private string _Id;
                private string _Descrizione;
                private string _TipoUnione;
                #endregion private properties

                #region public data member
                public string Id { get { return _Id; } set { _Id = value; } }
                public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
                public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }
                #endregion public data member
            }

            public class DatiCodMaggFamiliari
            {
                public DatiCodMaggFamiliari()
                {
                }

                internal DatiCodMaggFamiliari(BLCommon.GestioneDecodifica.CodMaggiorazioneFamiliari codMagg)
                {
                    this._Id = codMagg.Id;
                    this._CampoVideo = codMagg.CampoVideo;
                    this._Descrizione = codMagg.Descrizione;
                    this._TipoAppartenenza = codMagg.TipoAppartenenza;
                }

                #region private properties
                private string _Id;
                private string _CampoVideo;
                private string _Descrizione;
                private string _TipoAppartenenza;
                #endregion private properties

                #region public data member
                public string Id { get { return _Id; } set { _Id = value; } }
                public string CampoVideo { get { return _CampoVideo; } set { _CampoVideo = value; } }
                public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
                public string TipoAppartenenza { get { return _TipoAppartenenza; } set { _TipoAppartenenza = value; } }
                #endregion public data member
            }
            #endregion nested classes
        }
        #endregion nested classes
    }
}
