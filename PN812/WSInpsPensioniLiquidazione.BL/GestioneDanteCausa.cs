using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Entity;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneDanteCausa
    {
        public static void GetDanteCausaByIdPensione(long idPensione, out INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            datiDanteCausa = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(idPensione, out datiDanteCausa);
        }

        public static void GetDanteCausaEntityByDatiPensione(GestionePensione.DatiPensione datiPensione, ref Entity.DanteCausaEntity entityDanteCausa)
        {
            if (entityDanteCausa == null)
            {
                Utility.TipoAppartenenza? tipo = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                entityDanteCausa = new INPS.Pensioni.Liquidazione.Entity.DanteCausaEntity();

                BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                if (datiDanteCausa == null)
                    return;
                entityDanteCausa.AnagraficaDC = new Entity.AnagraficaDC();
                Utility.ValorizzaOggetti(datiDanteCausa, entityDanteCausa.AnagraficaDC);

                entityDanteCausa.DatiPensioneCI = new Entity.DatiPensioneCI();
                Utility.ValorizzaOggetti(datiDanteCausa, entityDanteCausa.DatiPensioneCI);
                entityDanteCausa.DatiPensioneCI.DecorrenzaOriginariaPrima = datiPensione.DecorrenzaOriginariaPrima;
                if (entityDanteCausa.DatiPensioneCI.IsDatiPensioneCINull())
                    entityDanteCausa.DatiPensioneCI = null;

                entityDanteCausa.DatiPensioneDiretta = new Entity.DatiPensioneDiretta();
                Utility.ValorizzaOggetti(datiDanteCausa, entityDanteCausa.DatiPensioneDiretta);
                if (entityDanteCausa.DatiPensioneDiretta.IsDatiPensioneDirettaNull())
                    entityDanteCausa.DatiPensioneDiretta = null;

                entityDanteCausa.AltraPensioneDC = new Entity.AltraPensioneDC();
                Utility.ValorizzaOggetti(datiDanteCausa, entityDanteCausa.AltraPensioneDC);
                if (entityDanteCausa.AltraPensioneDC.IsAllDatiAltraPensioneDCNull())
                    entityDanteCausa.AltraPensioneDC = null;

                if (tipo.HasValue && tipo.Value != Utility.TipoAppartenenza.FS)
                    GetDatiRedditiSentenza495_93ByIdPensione(datiPensione.Id, ref entityDanteCausa);

                entityDanteCausa.IdDC = datiDanteCausa.Id;
            }
        }

        public static void EliminaDanteCausaByIdPensione(long idPensione)
        {
            INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DeleteDanteCausaByIdPensione(idPensione);
        }

        private static bool ControlCrossIstituzioneEsteraCI(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoApp, out string errore)
        {
            errore = string.Empty;

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
            {
                List<BLCommon.GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
                BLCommon.GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out lDatiCalcoloRetrib);

                GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);

                GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                if (lDatiCalcoloRetrib == null)
                    lDatiCalcoloRetrib = new List<GestioneCalcolo.DatiCalcoloRetributivo>();

                if (nuoveLiquidate == null)
                    nuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();

                if (datiIstruttoria == null)
                    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                #region Controlli Contributi Danimarca CI non utilizzabile per mancanza di infrastruttura legata a CodeConvenzione e Cittadinanza

                //if (tipoDomanda == Utility.TipoDomanda.Reversibilita)
                //{
                //    string cittadinanzaDC = string.Empty;
                //    if (anagrafDC != null)
                //        cittadinanzaDC = anagrafDC.Cittadinanza;

                //    List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(numeroDomanda, out listaPrestazioniEstere);

                //    byte? codeConv = listaPrestazioniEstere[0].CodiceConvenzione;
                //    if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
                //    {
                //        List<StatoEstero> listaStatiEsteri = new List<StatoEstero>();
                //        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
                //        GestioneDatiContributiviCi.GetImportiEsteriByNumeroDomanda(numeroDomanda, out listaImportiEsteri);
                //        foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEstere)
                //        {
                //            StatoEstero statoEstero = new StatoEstero();
                //            statoEstero.PrestazioneEstera = new PrestazioneEstera();
                //            Utility.ValorizzaOggetti(prestazioneEE, statoEstero.PrestazioneEstera);
                //            statoEstero.ElencoImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEE.Id);

                //            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importiEE in statoEstero.ElencoImportiEsteri)
                //            {
                //                if (!GestioneCrossControls.VerificaContributiDanimarcaDanteCausa(codeConv, datiPensione.DecorrenzaOriginaria, statoEstero.PrestazioneEstera.CodiceStatoEE,
                //                importiEE.DecorrenzaPrestazioneEE, statoEstero.PrestazioneEstera.ContributiEEDiritto, cittadinanzaDC, datiPensione.Gruppo))
                //                {
                //                    errore = "Ctr. o Quota DANESI incompatibili con cittad.extraUE dante causa.";
                //                    return false;
                //                }
                //            }

                //        }
                //    }
                //}
                #endregion Controlli Contributi Danimarca CI non utilizzabile per mancanza di infrastruttura legata a CodeConvenzione e Cittadinanza

                #region Controlli R.M.S.

                //DA SPOSTARE SU CALCOLO
                //for (int i = 0; i < lDatiCalcoloRetrib.Count; i++)
                //{
                //    if (IsSingleTabSaved)
                //    {
                //        //DA SPOSTARE SU CALCOLO
                //        //if (!GestioneCrossControls.VerificaRMSDanteCausa(diretta.Certificato, diretta.DecorrenzaPensione, lDatiCalcoloRetrib[i].RMSQuotaA,
                //        //    datiPensione.InizioAssicurazione, datiPensione.SiglaCategoria, anagrafDC.DataMorte, datiIstruttoria.DecorrenzaOpzione, nuoveLiquidate.FlagContributiva, datiPensione.NaturaPensione, datiPensione.Gruppo))
                //        //{
                //        //    errore = "R.M.S. mancante.";
                //        //    return false;
                //        //}
                //    }
                //    else
                //    {

                //    }
                //}

                #endregion Controlli R.M.S.
            }

            return true;
        }

        #region DatiAnagrafica

        public static string StoreDatiAnagraficaDC(GestionePensione.DatiPensione datiPensione, Entity.AnagraficaDC datiAnagrafica, Entity.DatiPensioneDiretta datiPensioneDiretta,
            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, List<GestioneFamiliari.Familiare> listaFamiliari,
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari, ref GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausaDB,
            ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool isRiaperturaDomanda, bool IsSingleTabSaved)
        {
            string errore = string.Empty;

            if (datiAnagrafica != null)
            {
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                if (IsSingleTabSaved)
                {
                    if (datiDanteCausaDB == null)
                        datiDanteCausaDB = new BLCommon.GestioneDanteCausa.DatiDanteCausa();

                    datiPensioneDiretta = new DatiPensioneDiretta();
                    Utility.ValorizzaOggetti(datiDanteCausaDB, datiPensioneDiretta);
                }

                if (ControlsDatiAnagrafica(datiPensione, tipoApp, datiAnagrafica, datiPensioneDiretta, datiDanteCausaDB, datiMaggiorazioniBenefici, datiAnagraficiTitolare, listaFamiliari, listaAnagraficaFamiliari, isRiaperturaDomanda,
                    IsSingleTabSaved, datiPensione.SiglaCategoria, out errore))
                {
                    GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                    //ENG - MEMO 50/2023
                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        if (datiDanteCausaDB == null)
                            datiDanteCausaDB = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DatiDanteCausa();
                        Utility.ValorizzaOggetti(datiAnagrafica, datiDanteCausaDB);
                        datiDanteCausaDB.IdPensione = datiPensione.Id;

                        if (datiAnagraficiDC == null)
                            datiAnagraficiDC = new GestioneAnagrafica.DatiAnagrafici();
                        Utility.ValorizzaOggetti(datiAnagrafica, datiAnagraficiDC);

                        if (datiAnagrafica.SiglaFamiliare.HasValue && datiAnagrafica.SiglaFamiliare == 'R')
                            datiAnagrafica.DataMatrimonio = datiAnagraficiDC.DataMatrimonio;

                        //ENG - Per queste tipologie di domande bisogna salvare prima l'anagrafica perchè non è presente
                        if (tipoApp == Utility.TipoAppartenenza.FS
                            && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO)
                            && Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)
                            && (Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.FS || Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.PT))
                        {
                            //Potendo cambiare il codice fiscale del dante causa, vi è la necessità di cancellare e inserire nuovamente i dati
                            BLCommon.GestioneDanteCausa.DeleteDanteCausaByIdPensione(datiPensione.Id);
                            BLCommon.GestioneAnagrafica.DeleteAnagrafica(datiAnagraficiDC.Id);
                            BLCommon.GestioneAnagrafica.SalvaAnagrafica(datiAnagraficiDC);
                            datiDanteCausaDB.IdAnagrafica = datiAnagraficiDC.Id;
                            BLCommon.GestioneDanteCausa.SalvaDanteCausa(datiDanteCausaDB);
                        }
                        else
                        {
                            BLCommon.GestioneDanteCausa.SalvaDanteCausa(datiDanteCausaDB);
                            BLCommon.GestioneAnagrafica.SalvaAnagrafica(datiAnagraficiDC);
                        }

                        datiQuadroQuadroDanteCausa.TabAnagrafica = 2;
                        GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroQuadroDanteCausa, datiPensione);

                        if (tipoApp == Utility.TipoAppartenenza.FS)
                        {
                            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria);
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.GAS:
                                    if (!(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)) && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione))
                                    {
                                        if (Utility.IsDomandaReversibilita(datiPensione))
                                        {
                                            if (datiDanteCausaDB != null && datiDanteCausaDB.DecorrenzaPensione.HasValue)
                                            {
                                                if (Utility.DataStrettamenteSuccessivaA(datiDanteCausaDB.DecorrenzaPensione.Value, new DateTime(1998, 02, 01)))
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
                                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                                    break;
                            }
                        }

                        transactionScope.Complete();
                    }
                }
            }
            return errore;

        }

        public static void GetDatiAnagraficaDCByIdPensione(long idPensione, ref Entity.DanteCausaEntity entityDanteCausa)
        {
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;

            BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(idPensione, out datiAnagrafici);
            if (datiAnagrafici == null)
                return;

            // questa assegnazione è necessaria perchè anche datiAnagrafici contiene la proprietà DataMorte(!!!)
            datiAnagrafici.DataMorte = entityDanteCausa.AnagraficaDC.DataMorte;
            Utility.ValorizzaOggetti(datiAnagrafici, entityDanteCausa.AnagraficaDC);
            entityDanteCausa.AnagraficaDC.IdAnagrafica = datiAnagrafici.Id;
        }

        public static void GetCrossPropertiesAnagraficaDC(long idPensione, Utility.TipoAppartenenza? tipoAppartenenza, string categoria, ref Entity.AnagraficaDC entityAnagraficaDC)
        {
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(idPensione, out listaFamiliari, out listaAnagraficaFamiliari);
            if (tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
            {
                GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(idPensione, out anagrafica);

                if (string.IsNullOrEmpty(entityAnagraficaDC.StatoEEResidenza)) // Residente in ITALIA
                    entityAnagraficaDC.IsResidenzaEE_DalEnabled = false;
                else
                    entityAnagraficaDC.IsResidenzaEE_DalEnabled = true;

                if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsConiugeOrUnitoCivile()))
                {
                    entityAnagraficaDC.IsContitolareConiuge = true;
                    if (listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsConiuge()))
                        entityAnagraficaDC.ParentelaDC = 1;
                    else if (listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale && x.IsUnitoCivile()))
                        entityAnagraficaDC.ParentelaDC = 21;
                }
                else
                    entityAnagraficaDC.IsContitolareConiuge = false;

                if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.Exists(x => x.CodiceFiscale == anagrafica.CodiceFiscale))
                    entityAnagraficaDC.SiglaFamiliare = listaFamiliari.Find(x => x.CodiceFiscale == anagrafica.CodiceFiscale).SiglaFamiliare;
            }

            if (listaFamiliari != null && listaFamiliari.Count > 0 && listaFamiliari.Exists(x => x.IsConiugeOrUnitoCivile()) &&
                listaAnagraficaFamiliari != null && listaAnagraficaFamiliari.Count > 0 && listaAnagraficaFamiliari.Exists(x => x.Id == listaFamiliari.Find(y => y.IsConiugeOrUnitoCivile()).IdAnagrafica))
                entityAnagraficaDC.DataNascitaContitolareConiuge = listaAnagraficaFamiliari.Find(x => x.Id == listaFamiliari.Find(y => y.IsConiugeOrUnitoCivile()).IdAnagrafica).DataNascita;
        }

        public static bool ControlsDatiAnagrafica(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoApp, Entity.AnagraficaDC anagrafDC, Entity.DatiPensioneDiretta diretta,
            BLCommon.GestioneDanteCausa.DatiDanteCausa DanteCausaDB, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare,
            List<GestioneFamiliari.Familiare> listaFamiliari, List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari, bool isRiaperturaDomanda, bool IsSingleTabSaved, string categoria, out string errore)
        {
            errore = string.Empty;

            bool indirettaRicostituzione = false;

            if (DanteCausaDB != null && DanteCausaDB.Certificato == null && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                indirettaRicostituzione = true;
            }

            if (tipoApp == Utility.TipoAppartenenza.FS
                          && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO)
                          && Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)
                          && (Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.FS || Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.PT))
            {
                if (String.IsNullOrEmpty(anagrafDC.CodiceFiscale))
                {
                    errore = "Il Codice Fiscale è obbligatorio";
                    return false;
                }
            }

            if (tipoApp == Utility.TipoAppartenenza.CI && !String.IsNullOrEmpty(anagrafDC.CodiceFiscale) && anagrafDC.CodiceFiscale.Contains("DANTEC_") && GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_CI.NESSUN_DANTE_CAUSA))
                return true;

            if (DanteCausaDB != null && DanteCausaDB.DataMorteOrigine.HasValue && DanteCausaDB.DataMorteOrigine.Value.CompareTo(anagrafDC.DataMorte.Value) != 0)
            {
                errore = string.Format("E' stata modificata la Data Morte che originariamente era: {0:dd/MM/yyyy}.", DanteCausaDB.DataMorteOrigine.Value);
                return false;
            }

            if (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.DataSuccessivaA(anagrafDC.DataMorte.Value, new DateTime(1997, 12, 1)) && Utility.IsDomandaSOSPED(datiPensione.SiglaCategoria))
            {
                errore = string.Format("Per le SOSPED la data di morte del dante causa deve essere maggiore o uguale al 01/12/1997.");
                return false;
            }

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
            {
                if (!anagrafDC.ParentelaDC.HasValue)
                {
                    errore = "Attenzione! La 'Relazione di Parentela' è obbligatoria";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithSessoDanteCausa(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, anagrafDC.Sesso))
                {
                    errore = "Settimane Incremento 0.5% incompatibili con Sesso del Titolare Dante Causa";
                    return false;
                }
            }



            if (IsSingleTabSaved)
            {
                if (anagrafDC.DataMorte.HasValue && DanteCausaDB.DecorrenzaEliminazione.HasValue && !indirettaRicostituzione)
                {
                    DateTime dataMorte = anagrafDC.DataMorte.Value;
                    dataMorte = dataMorte.AddMonths(1);
                    if ((dataMorte.Month != DanteCausaDB.DecorrenzaEliminazione.Value.Month) || (dataMorte.Year != DanteCausaDB.DecorrenzaEliminazione.Value.Year))
                    {
                        errore = "Attenzione! La 'Decorrenza Eliminazione' deve essere obbligatoriamente: " + dataMorte.Month + "/" + dataMorte.Year + ". Verificare ed effettuare il salvataggio completo";
                        return false;
                    }
                }

                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAnagraficaDanteCausa(anagrafDC.CodiceFiscale, anagrafDC.Cognome, anagrafDC.Nome, anagrafDC.DataNascita, anagrafDC.DataMorte, anagrafDC.DecorrenzaResidenza,
                        anagrafDC.Cittadinanza, datiPensione.CausaCarico, anagrafDC.StatoEEResidenza, DanteCausaDB.DecorrenzaPensione, anagrafDC.ParentelaDC, anagrafDC.Sesso, datiAnagraficiTitolare.Sesso,
                        datiAnagraficiTitolare.CodiceStatoCivile, datiAnagraficiTitolare.CognomeAcquisito, anagrafDC.DataMatrimonio, datiPensione.DecorrenzaOriginaria, listaFamiliari, listaAnagraficaFamiliari, categoria,
                        out errore))
                        return false;
                }
            }
            else
            {
                if (anagrafDC.DataMorte.HasValue && diretta != null && diretta.DecorrenzaEliminazione.HasValue && !indirettaRicostituzione)
                {
                    DateTime dataMorte = anagrafDC.DataMorte.Value;
                    dataMorte = dataMorte.AddMonths(1);
                    if ((dataMorte.Month != diretta.DecorrenzaEliminazione.Value.Month) || (dataMorte.Year != diretta.DecorrenzaEliminazione.Value.Year))
                    {
                        errore = "Attenzione! La 'Decorrenza Eliminazione' deve essere obbligatoriamente: " + dataMorte.Month + "/" + dataMorte.Year;
                        return false;
                    }
                }


                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAnagraficaDanteCausa(anagrafDC.CodiceFiscale, anagrafDC.Cognome, anagrafDC.Nome, anagrafDC.DataNascita, anagrafDC.DataMorte, anagrafDC.DecorrenzaResidenza,
                        anagrafDC.Cittadinanza, datiPensione.CausaCarico, anagrafDC.StatoEEResidenza, diretta != null ? diretta.DecorrenzaPensione : null, anagrafDC.ParentelaDC, anagrafDC.Sesso, datiAnagraficiTitolare.Sesso,
                        datiAnagraficiTitolare.CodiceStatoCivile, datiAnagraficiTitolare.CognomeAcquisito, anagrafDC.DataMatrimonio, datiPensione.DecorrenzaOriginaria, listaFamiliari, listaAnagraficaFamiliari, datiPensione.SiglaCategoria,
                        out errore))
                        return false;
                }
            }


            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Superstiti && (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.AGO || tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.FS))
            {
                if (!GestioneCrossControls.AGO_FS_ControlsDataMatrimonioWithGradoParentelaAndDataMorte(datiPensione, anagrafDC.DataMatrimonio, anagrafDC.DataMorte, anagrafDC.DataNascita, listaFamiliari,
                    listaAnagraficaFamiliari, datiAnagraficiTitolare, tipoApp, isRiaperturaDomanda, DanteCausaDB, out errore))
                    return false;
            }

            if (tipoApp.HasValue && (tipoApp.Value == Utility.TipoAppartenenza.AGO || tipoApp.Value == Utility.TipoAppartenenza.CI))
            {
                if (!GestioneCrossControls.AGO_CI_ControlsDecorrenzaResidenzaDanteCausa(anagrafDC.StatoEEResidenza, anagrafDC.DecorrenzaResidenza, anagrafDC.DataMorte, anagrafDC.DataNascita, out errore))
                    return false;

                if (!(Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaSOSPED(datiPensione.SiglaCategoria)))
                {
                    if (string.IsNullOrEmpty(anagrafDC.Cittadinanza))
                    {
                        errore = "La Cittadinanza è obbligatoria";
                        return false;
                    }

                    if (!GestioneCrossControls.AGO_CI_ControlsProvenienzaPensione(anagrafDC.ProvenienzaPensione, out errore))
                        return false;
                }
            }

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.AGO)
            {
                if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria))
                {
                    if (!GestioneCrossControls.AGO_ControlsDataMortePerCumulo(anagrafDC.DataMorte, out errore))
                        return false;
                }

                if (Utility.IsDomandaAUT(datiPensione) && diretta != null)
                {
                    if (!GestioneCrossControls.AGO_ControlsCodNatura1WithDecPensForAut(datiPensione.NaturaPensione, diretta.DecorrenzaPensione, out errore))
                        return false;
                }
            }

            #region codice commentato
            //DA SPOSTARE SUL CALCOLO
            //if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
            //{                    
            //    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo);
            //    if (tipoDomanda == Utility.TipoDomanda.Reversibilita)
            //    {
            //        if (!GestioneCrossControls.VerificaFineAssicurazioneWithDataMorteDanteCausa(anagrafDC.DataMorte, datiPensione.FineAssicurazione, datiPensione.Gruppo))
            //        {
            //            errore = "Data Ultimo Contributo (dati assicurativi) posteriori a Data Morte Dante Causa.";
            //            return false;
            //        }
            //    }

            //    #region Controlli R.M.S.
            //    //DA SPOSTARE SU CALCOLO
            //    //List<BLCommon.GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
            //    //BLCommon.GestioneCalcolo.GetCalcoloRetributivoCI_AGOByPensione(datiPensione.Id, out lDatiCalcoloRetrib);

            //    //GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            //    //GestioneNuoveLiquidate.GetNuoveLiquidateByNumeroDomanda(numeroDomanda, out nuoveLiquidate);

            //    //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            //    //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

            //    //if (nuoveLiquidate == null)
            //    //    nuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();

            //    //if (datiIstruttoria == null)
            //    //    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
            //    //if (lDatiCalcoloRetrib != null)
            //    //{
            //    //    foreach (GestioneCalcolo.DatiCalcoloRetributivo retrib in lDatiCalcoloRetrib)
            //    //    {
            //    //        if (!GestioneCrossControls.VerificaRMSDanteCausa(DanteCausaDB.Certificato, DanteCausaDB.DecorrenzaPensione, retrib.RMSQuotaA, datiPensione.InizioAssicurazione,
            //    //             datiPensione.SiglaCategoria, anagrafDC.DataMorte, datiIstruttoria.DecorrenzaOpzione, nuoveLiquidate.FlagContributiva, datiPensione.NaturaPensione, datiPensione.Gruppo))
            //    //        {
            //    //            errore = "R.M.S. mancante.";
            //    //            return false;
            //    //        }
            //    //    }
            //    //}

            //    #endregion Controlli R.M.S.

            //}
            #endregion codice commentato

            #region Controlli Contributi Danimarca CI non utilizzabile per mancanza di infrastruttura legata a CodeConvenzione e Cittadinanza

            //if (tipoDomanda == Utility.TipoDomanda.Reversibilita)
            //{
            //    string cittadinanzaDC = string.Empty;
            //    if (anagrafDC != null)
            //        cittadinanzaDC = anagrafDC.Cittadinanza;

            //    List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(numeroDomanda, out listaPrestazioniEstere);

            //    byte? codeConv = listaPrestazioniEstere[0].CodiceConvenzione;
            //    if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            //    {
            //        List<StatoEstero> listaStatiEsteri = new List<StatoEstero>();
            //        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            //        GestioneDatiContributiviCi.GetImportiEsteriByNumeroDomanda(numeroDomanda, out listaImportiEsteri);
            //        foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEstere)
            //        {
            //            StatoEstero statoEstero = new StatoEstero();
            //            statoEstero.PrestazioneEstera = new PrestazioneEstera();
            //            Utility.ValorizzaOggetti(prestazioneEE, statoEstero.PrestazioneEstera);
            //            statoEstero.ElencoImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEE.Id);

            //            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importiEE in statoEstero.ElencoImportiEsteri)
            //            {
            //                if (!GestioneCrossControls.VerificaContributiDanimarcaDanteCausa(codeConv, datiPensione.DecorrenzaOriginaria, statoEstero.PrestazioneEstera.CodiceStatoEE,
            //                importiEE.DecorrenzaPrestazioneEE, statoEstero.PrestazioneEstera.ContributiEEDiritto, cittadinanzaDC, datiPensione.Gruppo))
            //                {
            //                    errore = "Ctr. o Quota DANESI incompatibili con cittad.extraUE dante causa.";
            //                    return false;
            //                }
            //            }

            //        }
            //    }
            //}
            #endregion Controlli Contributi Danimarca CI non utilizzabile per mancanza di infrastruttura legata a CodeConvenzione e Cittadinanza

            if (!GestioneCrossControls.ALL_VerificaDataMatrimonioDC(datiPensione, isRiaperturaDomanda, anagrafDC.DataMatrimonio, listaFamiliari, tipoApp, out errore))
                return false;

            return true;
        }

        #endregion DatiAnagrafica

        #region DatiAltraPensione

        public static string StoreDatiAltraPensioneByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.AltraPensioneDC datiAltraPensione, Entity.AnagraficaDC anagrafDC, Entity.DatiPensioneDiretta diretta,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved)
        {
            string errore = string.Empty;

            if (datiAltraPensione != null && !datiAltraPensione.IsAllDatiAltraPensioneDCNull())
            {
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                if (IsSingleTabSaved)
                {
                    if (datiDanteCausa == null)
                        datiDanteCausa = new BLCommon.GestioneDanteCausa.DatiDanteCausa();

                    anagrafDC = new AnagraficaDC();
                    Utility.ValorizzaOggetti(datiAnagraficiDC, anagrafDC);
                    Utility.ValorizzaOggetti(datiDanteCausa, anagrafDC);

                    diretta = new DatiPensioneDiretta();
                    Utility.ValorizzaOggetti(datiDanteCausa, diretta);
                }

                if (ControlsDatiAltraPensione(datiPensione, tipoApp, datiAltraPensione, anagrafDC, diretta, datiDanteCausa, datiAnagraficiDC, IsSingleTabSaved, out errore))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        if (datiDanteCausa == null)
                            datiDanteCausa = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DatiDanteCausa();
                        datiDanteCausa.IdPensione = datiPensione.Id;
                        Utility.ValorizzaOggetti(datiAltraPensione, datiDanteCausa);

                        INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.SalvaDanteCausa(datiDanteCausa);

                        datiQuadroQuadroDanteCausa.TabAltraPensione = 2;
                        GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroQuadroDanteCausa, datiPensione);

                        transactionScope.Complete();
                    }
                }
            }
            return errore;
        }

        public static void GetDatiAltraPensioneByDatiPensione(GestionePensione.DatiPensione datiPensione, ref Entity.DanteCausaEntity entityDanteCausa)
        {

        }

        public static bool ControlsDatiAltraPensione(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoApp, Entity.AltraPensioneDC altraPensione, Entity.AnagraficaDC anagrafDC, Entity.DatiPensioneDiretta diretta,
            BLCommon.GestioneDanteCausa.DatiDanteCausa DanteCausaDB, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;

            if (!(String.IsNullOrEmpty(DanteCausaDB.NaturaPensione)) && DanteCausaDB.NaturaPensione.Substring(0, 1).Trim() != string.Empty && Convert.ToInt32(DanteCausaDB.NaturaPensione.Substring(0, 1)) == 6)
            {
                if (altraPensione.IsDatiAltraPensioneDCObbligatoriNull())
                {
                    errore = "Attenzione! E' obbligatorio inserire i dati per Categoria, Sede, Codice U/C, Decorrenza e Codice Importo.";
                    return false;
                }
            }
            else
            {
                if (altraPensione.IsDatiAltraPensioneDCObbligatoriNull() && altraPensione.CessazioneAltraPensione.HasValue)
                {
                    errore = "Attenzione! Non è possibile salvare solo la data di Cessazione, completare l'inserimento dei dati e riprovare.";
                    return false;
                }

                if (altraPensione.IsDatiAltraPensioneDCObbligatoriNull() && !altraPensione.IsAllDatiAltraPensioneDCNull())
                {
                    errore = "Attenzione! I dati Categoria, Sede, Codice U/C, Decorrenza e Codice Importo devono essere o tutti valorizzati o nessuno.";
                    return false;
                }
            }

            if (IsSingleTabSaved)
            {
                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAltraPensioneDanteCausa(altraPensione.CodiceUCAltraPensione, altraPensione.DecorrenzaAltraPensione, datiAnagraficiDC.DataNascita, DanteCausaDB.DataMorte,
                        altraPensione.CessazioneAltraPensione, altraPensione.CategoriaAltraPensione, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(altraPensione.CategoriaAltraPensione, DanteCausaDB.NaturaPensione, DanteCausaDB.SiglaCategoria, out errore))
                        return false;
                }
            }
            else
            {
                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAltraPensioneDanteCausa(altraPensione.CodiceUCAltraPensione, altraPensione.DecorrenzaAltraPensione, anagrafDC != null ? anagrafDC.DataNascita : null,
                        anagrafDC != null ? anagrafDC.DataMorte : null, altraPensione.CessazioneAltraPensione, altraPensione.CategoriaAltraPensione, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(altraPensione.CategoriaAltraPensione, diretta != null ? diretta.NaturaPensione : null, diretta != null ? diretta.SiglaCategoria : null, out errore))
                        return false;
                }
            }

            return true;
        }

        #endregion DatiAltraPensione

        #region DatiPensioneCI

        public static string StoreDatiPensioneCI(ref GestionePensione.DatiPensione datiPensione, Entity.DatiPensioneCI datiPensioneCI, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi,
            ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroDanteCausa, bool IsSingleTabSaved)
        {
            string errore = string.Empty;

            if (datiPensioneCI != null)
            {
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                List<BLCommon.GestioneDanteCausa.PensioniEstereDcBL> lPensioniEstereDcBL = new List<BLCommon.GestioneDanteCausa.PensioniEstereDcBL>();
                byte? CodiceVario = 0;

                foreach (Entity.DatiPensioneCI.DatiPensioniEstereDc pensioniEstereDc in datiPensioneCI.lDatiPensioniEstereDc)
                {
                    BLCommon.GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDcBL = new BLCommon.GestioneDanteCausa.PensioniEstereDcBL();
                    Utility.ValorizzaOggetti(pensioniEstereDc, pensioniEstereDcBL);
                    lPensioniEstereDcBL.Add(pensioniEstereDcBL);

                    if (pensioniEstereDcBL.CodiciVari.HasValue && (pensioniEstereDcBL.CodiciVari == 3 || pensioniEstereDcBL.CodiciVari == 4 || pensioniEstereDcBL.CodiciVari == 5 || pensioniEstereDcBL.CodiciVari == 8))
                        CodiceVario = pensioniEstereDcBL.CodiciVari;
                }

                if (ControlsDatiPensioneCI(datiPensione, tipoApp, datiPensioneCI, datiDanteCausa, datiGenericiAgoCi, IsSingleTabSaved, out errore))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        // danteCausa
                        if (datiDanteCausa == null)
                        {
                            datiDanteCausa = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DatiDanteCausa();
                            datiDanteCausa.IdPensione = datiPensione.Id;
                        }
                        Utility.ValorizzaOggetti(datiPensioneCI, datiDanteCausa);
                        BLCommon.GestioneDanteCausa.SalvaDanteCausa(datiDanteCausa);
                        //pensione
                        datiPensione.DecorrenzaOriginariaPrima = datiPensioneCI.DecorrenzaOriginariaPrima;
                        GestionePensione.SalvaPensione(datiPensione);
                        // maggiorazioneBenefici
                        if (datiMaggiorazioniBenefici == null)
                        {
                            datiMaggiorazioniBenefici = new BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                            datiMaggiorazioniBenefici.IdPensione = datiPensione.Id;
                        }
                        Utility.ValorizzaOggetti(datiPensioneCI, datiMaggiorazioniBenefici);

                        datiMaggiorazioniBenefici.ImportoComplessivoArt1 = null;
                        if (CodiceVario == 3)
                        {
                            datiMaggiorazioniBenefici.ImportoComplessivoArt3 = datiPensioneCI.TotaleArticolo345Legge140;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt4 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt5 = null;
                        }
                        else if (CodiceVario == 4)
                        {
                            datiMaggiorazioniBenefici.ImportoComplessivoArt4 = datiPensioneCI.TotaleArticolo345Legge140;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt3 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt5 = null;
                        }
                        else if (CodiceVario == 5)
                        {
                            datiMaggiorazioniBenefici.ImportoComplessivoArt5 = datiPensioneCI.TotaleArticolo345Legge140;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt4 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt3 = null;
                        }
                        else if (CodiceVario == 8)
                        {
                            datiMaggiorazioniBenefici.ImportoComplessivoArt5 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt4 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt3 = null;
                            datiMaggiorazioniBenefici.ImportoComplessivoArt1 = datiPensioneCI.TotaleArticolo345Legge140;
                        }
                        BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
                        //PensioniEstereDc
                        foreach (BLCommon.GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDcBL in lPensioniEstereDcBL)
                            BLCommon.GestioneDanteCausa.SalvaPensioniEstereDC(pensioniEstereDcBL);

                        if (!Utility.IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa))
                        {
                            datiQuadroDanteCausa.TabDatiPensioneCI = 2;
                            GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroDanteCausa, datiPensione);
                        }

                        transactionScope.Complete();
                    }
                }
            }
            return errore;
        }

        public static void GetDatiPensioneCIByDatiPensione(GestionePensione.DatiPensione datiPensione, ref Entity.DanteCausaEntity entityDanteCausa)
        {
            if (datiPensione == null)
                return;
            entityDanteCausa.DatiPensioneCI.DecorrenzaOriginariaPrima = datiPensione.DecorrenzaOriginariaPrima;

            List<BLCommon.GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
            BLCommon.GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);

            entityDanteCausa.DatiPensioneCI.lDatiPensioniEstereDc = new List<Entity.DatiPensioneCI.DatiPensioniEstereDc>();
            byte? CodiceVario = 0;
            foreach (BLCommon.GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDcBL in LpensioniEstereDcBL)
            {
                Entity.DatiPensioneCI.DatiPensioniEstereDc PensioniEstereDc = new Entity.DatiPensioneCI.DatiPensioniEstereDc();
                PensioniEstereDc.Importo = pensioniEstereDcBL.Importo;
                PensioniEstereDc.CodiciVari = pensioniEstereDcBL.CodiciVari;
                PensioniEstereDc.IdDanteCausa = entityDanteCausa.IdDC;
                entityDanteCausa.DatiPensioneCI.lDatiPensioniEstereDc.Add(PensioniEstereDc);

                if (pensioniEstereDcBL.CodiciVari.HasValue && (pensioniEstereDcBL.CodiciVari == 3 || pensioniEstereDcBL.CodiciVari == 4 || pensioniEstereDcBL.CodiciVari == 5 || pensioniEstereDcBL.CodiciVari == 8))
                    CodiceVario = pensioniEstereDcBL.CodiciVari;
            }
            BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            BLCommon.GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out datiMaggiorazioniBenefici);

            if (datiMaggiorazioniBenefici != null)
            {
                Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, entityDanteCausa.DatiPensioneCI);

                if (CodiceVario == 3)
                    entityDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt3;
                else
                    if (CodiceVario == 4)
                    entityDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt4;
                else
                        if (CodiceVario == 5)
                    entityDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt5;
                else
                            if (CodiceVario == 8)
                    entityDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt1;

                entityDanteCausa.DatiPensioneCI.Articolo6140 = datiMaggiorazioniBenefici.Articolo6140;
            }
        }

        public static bool ControlsDatiPensioneCI(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoApp, Entity.DatiPensioneCI pensioneCI, BLCommon.GestioneDanteCausa.DatiDanteCausa DanteCausaDB,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
            {
                if (pensioneCI.lDatiPensioniEstereDc != null && pensioneCI.lDatiPensioniEstereDc.Count > 0)
                {
                    foreach (DatiPensioneCI.DatiPensioniEstereDc pensioneEsteraDC in pensioneCI.lDatiPensioniEstereDc)
                    {
                        //questi codici non sono contemplati nel controllo. Vengono settati staticamente nella web e corrispondono alla sezione Dati Pensione del Dante Causa 10/2013  Decorrenza "SO" e alla sezione Articolo 6 
                        //della tab Pensione CI
                        //Vengono salvati nella stessa tabella e al servizio arriva una lista di 3 record, di cui due contengono il CodiceVari 6 e 10
                        if (pensioneEsteraDC.CodiciVari.HasValue && pensioneEsteraDC.CodiciVari.Value != 6 && pensioneEsteraDC.CodiciVari.Value != 10)
                        {
                            if (!GestioneCrossControls.CI_VerificaCodiciVari(pensioneEsteraDC.CodiciVari, out errore))
                                return false;
                        }
                    }
                }

                if (!GestioneCrossControls.CI_VerificaLegge5991WithDecorrenzaPensione(datiPensione.DecorrenzaOriginaria, pensioneCI.AumentoMensileLegge5991Comma9, out errore))
                    return false;

                if (!GestioneCrossControls.CI_VerificaSentenza7290WithRms8888(pensioneCI.Aumento7290, datiGenericiAgoCi != null ? datiGenericiAgoCi.RMS8888 : null, out errore))
                    return false;

                if (!GestioneCrossControls.CI_VerificaAumentoLeggeArt2WithRms9090(pensioneCI.AumentoMensileLegge161289Art2, datiGenericiAgoCi != null ? datiGenericiAgoCi.RMS9090 : null, out errore))
                    return false;
            }

            return true;
        }

        #endregion DatiPensioneCI

        #region DatiPensioneDiretta

        public static string StoreDatiPensioneDirettaByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.DatiPensioneDiretta datiPensioneDiretta, Entity.AnagraficaDC anagraficaDC, Entity.DatiPensioneCI pensioneCI,
            Entity.AltraPensioneDC altraPensione, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere,
            GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, List<GestioneFamiliari.Familiare> listaFamiliari,
            ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved)
        {
            string errore = string.Empty;

            if (datiPensioneDiretta != null)
            {
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                if (IsSingleTabSaved)
                {
                    if (datiDanteCausa == null)
                        datiDanteCausa = new BLCommon.GestioneDanteCausa.DatiDanteCausa();

                    anagraficaDC = new AnagraficaDC();
                    Utility.ValorizzaOggetti(datiAnagraficiDC, anagraficaDC);
                    Utility.ValorizzaOggetti(datiDanteCausa, anagraficaDC);

                    pensioneCI = new DatiPensioneCI();
                    pensioneCI.DecorrenzaOriginariaPrima = datiPensione.DecorrenzaOriginariaPrima;

                    List<BLCommon.GestioneDanteCausa.PensioniEstereDcBL> LpensioniEstereDcBL = null;
                    BLCommon.GestioneDanteCausa.GetPensioniEstereDCByIdPensione(datiPensione.Id, out LpensioniEstereDcBL);

                    pensioneCI.lDatiPensioniEstereDc = new List<Entity.DatiPensioneCI.DatiPensioniEstereDc>();
                    byte? CodiceVario = 0;
                    foreach (BLCommon.GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDcBL in LpensioniEstereDcBL)
                    {
                        Entity.DatiPensioneCI.DatiPensioniEstereDc PensioniEstereDc = new Entity.DatiPensioneCI.DatiPensioniEstereDc();
                        PensioniEstereDc.Importo = pensioniEstereDcBL.Importo;
                        PensioniEstereDc.CodiciVari = pensioniEstereDcBL.CodiciVari;
                        PensioniEstereDc.IdDanteCausa = datiDanteCausa.Id;
                        pensioneCI.lDatiPensioniEstereDc.Add(PensioniEstereDc);

                        if (pensioniEstereDcBL.CodiciVari.HasValue && (pensioniEstereDcBL.CodiciVari == 3 || pensioniEstereDcBL.CodiciVari == 4 || pensioniEstereDcBL.CodiciVari == 5 || pensioniEstereDcBL.CodiciVari == 8))
                            CodiceVario = pensioniEstereDcBL.CodiciVari;
                    }

                    if (datiMaggiorazioniBenefici != null)
                    {
                        Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, pensioneCI);

                        if (CodiceVario == 3)
                            pensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt3;
                        else
                            if (CodiceVario == 4)
                            pensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt4;
                        else
                                if (CodiceVario == 5)
                            pensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt5;
                        else
                                    if (CodiceVario == 8)
                            pensioneCI.TotaleArticolo345Legge140 = datiMaggiorazioniBenefici.ImportoComplessivoArt1;

                        pensioneCI.Articolo6140 = datiMaggiorazioniBenefici.Articolo6140;
                    }
                    altraPensione = new AltraPensioneDC();
                    Utility.ValorizzaOggetti(datiDanteCausa, altraPensione);
                }

                if (ControlsDatiDiretta(datiPensione, tipoApp, anagraficaDC, datiPensioneDiretta, pensioneCI, altraPensione, datiDanteCausa, datiGenericiAgoCi, listaPrestazioniEstere, datiMaggiorazioniBenefici,
                    datiIstruttoria, datiAnagraficiDC, listaFamiliari, IsSingleTabSaved, out errore))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        if (datiDanteCausa == null)
                            datiDanteCausa = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DatiDanteCausa();

                        Utility.ValorizzaOggetti(datiPensioneDiretta, datiDanteCausa);
                        datiDanteCausa.IdPensione = datiPensione.Id;

                        BLCommon.GestioneDanteCausa.SalvaDanteCausa(datiDanteCausa);

                        datiQuadroQuadroDanteCausa.TabPensioneDiretta = 2;
                        GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroQuadroDanteCausa, datiPensione);

                        transactionScope.Complete();
                    }
                }
            }
            return errore;
        }

        public static void GetDatiPensioneDiretta(ref Entity.DanteCausaEntity entityDanteCausa)
        {
            List<GestioneDecodifica.Maggiorazione781> elencomaggcontr780 = null;
            entityDanteCausa.ElencoMaggiorazione781 = new List<Entity.DanteCausaEntity.DatiMaggiorazione781>();

            GestioneDecodifica.GetMaggiorazione781ContributiDC(out elencomaggcontr780);
            foreach (GestioneDecodifica.Maggiorazione781 m780 in elencomaggcontr780)
                entityDanteCausa.ElencoMaggiorazione781.Add(new Entity.DanteCausaEntity.DatiMaggiorazione781 { Id = m780.Id, Descrizione = m780.Descrizione });
        }

        public static bool ControlsDatiDiretta(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoApp, Entity.AnagraficaDC anagrafDC, Entity.DatiPensioneDiretta diretta, Entity.DatiPensioneCI pensioneCI,
            Entity.AltraPensioneDC altraPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa DanteCausaDB, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC, List<GestioneFamiliari.Familiare> listaFamiliari, bool IsSingleTabSaved, out string errore)
        {
            errore = string.Empty;

            if (diretta == null)
            {
                //Controllo nel quale non dovrebbe mai passare
                errore = "Attenzione! DatiPensioneDiretta non presenti";
                return false;
            }

            if (tipoApp == Utility.TipoAppartenenza.CI && datiAnagraficiDC.CodiceFiscale.Contains("DANTEC_") && GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_CI.NESSUN_DANTE_CAUSA))
                return true;

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, diretta.DecorrenzaPensione);

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoriaPensione = 0;
            int.TryParse(categoriaNumerica, out categoriaPensione);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.FS)
            {
                if (!diretta.DecorrenzaEliminazione.HasValue)
                {
                    errore = "Attenzione! La 'Decorrenza Eliminazione' deve essere inserita obbligatoriamente";
                    return false;
                }

                if (!diretta.DecorrenzaEliminazioneContabile.HasValue && !(Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, DanteCausaDB))
                    && !(controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))))
                {
                    errore = "Attenzione! La 'Decorrenza Eliminazione contabile' deve essere inserita obbligatoriamente";
                    return false;
                }
            }

            if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
            {
                if (!GestioneCrossControls.CI_VerificaCodNaturaWithCategoriaDC(datiPensione.NaturaPensione, diretta.SiglaCategoria))
                {
                    errore = "Natura pensione 'O' (reg.sperimentale donne) incompatibile con reversibilità da assicurato";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaCodNaturaTitolareWithDC(diretta.NaturaPensione, datiPensione.NaturaPensione))
                {
                    errore = "Natura Pensione errata";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaDecorrenzaArt2DPCMWithDanteCausa(datiGenericiAgoCi != null ? datiGenericiAgoCi.DecorrenzaArt2Dpcm : null, diretta.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria))
                {
                    errore = "Decorrenza D.P.C.M. (Liquidazione Pensione/Dati Opzione) incompatibile con Decorrenza della Pensione";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaCompatibilitaCategoriaDirettaWithCodNatura(datiPensione.NaturaPensione, diretta.SiglaCategoria, diretta.DecorrenzaPensione))
                {
                    errore = "Categoria Diretta incompatibile con Natura Pensione";
                    return false;
                }

                if (!GestioneCrossControls.CI_ControlsCodiceVirtualeWithCertificatoDiretta(datiGenericiAgoCi != null ? datiGenericiAgoCi.CodiceVirtuale : null, diretta.Certificato, listaPrestazioniEstere.Count() > 0 ? listaPrestazioniEstere[0].CodiceConvenzione : null,
                    datiPensione.CausaCarico, out errore))
                    return false;

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento1PercentoWithDanteCausa(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, diretta.SiglaCategoria,
                    diretta.DecorrenzaPensione))
                {
                    errore = "Settimane Incremento 1% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithDecorrenzaDiretta(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, diretta.SiglaCategoria,
                    diretta.DecorrenzaPensione))
                {
                    errore = "Settimane Incremento 0.5% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaAnniDifferimentoWithDanteCausa(datiGenericiAgoCi != null ? datiGenericiAgoCi.AnniDifferimento : null, diretta.SiglaCategoria, diretta.DecorrenzaPensione))
                {
                    errore = "Anni Differimento incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (pensioneCI != null && pensioneCI.lDatiPensioniEstereDc != null && pensioneCI.lDatiPensioniEstereDc.Count > 0)
                {
                    foreach (DatiPensioneCI.DatiPensioniEstereDc pensioneEsteraDC in pensioneCI.lDatiPensioniEstereDc)
                    {
                        //questi codici non sono contemplati nel controllo. Vengono settati staticamente nella web e corrispondono alla sezione Dati Pensione del Dante Causa 10/2013  Decorrenza "SO" e alla sezione Articolo 6 
                        //della tab Pensione CI
                        //Vengono salvati nella stessa tabella e al servizio arriva una lista di 3 record, di cui due contengono il CodiceVari 6 e 10
                        if (pensioneEsteraDC.CodiciVari.HasValue && pensioneEsteraDC.CodiciVari.Value != 6 && pensioneEsteraDC.CodiciVari.Value != 10)
                        {
                            if (!GestioneCrossControls.CI_VerificaCodiceArt4Legge140(diretta.DecorrenzaPensione, diretta.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaCodiceDCPM(diretta.DecorrenzaPensione, diretta.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaCodiceArt41(datiPensione.DecorrenzaOriginaria, diretta.Maggiorazione781Contributi, pensioneEsteraDC.CodiciVari, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaLegge140WithCategoria(datiPensione.SiglaCategoria, pensioneEsteraDC.CodiciVari.Value, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaImportoArt345(datiPensione.DecorrenzaOriginaria, pensioneCI.TotaleArticolo345Legge140, pensioneEsteraDC.CodiciVari, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaImportoArt345WithCodiciVari(pensioneCI.TotaleArticolo345Legge140, pensioneEsteraDC.CodiciVari, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaRangeImportoArt345(pensioneCI.TotaleArticolo345Legge140, out errore))
                                return false;

                            if (!GestioneCrossControls.CI_VerificaCodiciVariWithDecorrenzaPensione(datiPensione.DecorrenzaOriginaria, pensioneEsteraDC.CodiciVari, out errore))
                                return false;
                        }
                    }
                }

                if (!GestioneCrossControls.CI_VerificaDecorrenzaArt6WithDecorrenza(diretta.Certificato, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, datiPensione.DecorrenzaOriginaria))
                {
                    errore = "Decorrenza Art.6/140 anteriore a Decorrenza Originaria";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaDecorrenzaArt6WithDecorrenza(diretta.Certificato, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, diretta.DecorrenzaPensione))
                {
                    errore = "Decorrenza Art.6/140 anteriore a Decorrenza Diretta";
                    return false;
                }

                /////// Per maggiori informazioni sul commento fare riferimento al documento L1-PCIPL29.docx sotto tfs alla cartella Documentazione\ControlliCI
                //if (!GestioneCrossControls.CI_VerificaSiglaCategoria(diretta.SiglaCategoria, out errore))
                //    return false;

                /////// Per maggiori informazioni sul commento fare riferimento al documento L1-PCIPL29.docx sotto tfs alla cartella Documentazione\ControlliCI
                //if (!GestioneCrossControls.CI_VerificaDecorrenzaDiretta(diretta.DecorrenzaPensione, out errore))
                //    return false;

                if (!GestioneCrossControls.CI_VerificaImportoIVS(datiPensione.SiglaCategoria, datiGenericiAgoCi != null ? datiGenericiAgoCi.ImportoIVS : null, diretta.Certificato, DanteCausaDB.DataMorte, diretta.DecorrenzaPensione,
                    datiPensione.DecorrenzaOriginaria, out errore))
                {
                    errore = "Liquidazione Pensione / Dati Assicurativi: " + errore;
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaDataDomandaOpzioneWithDanteCausa(datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, decorrenza, diretta.SiglaCategoria, out errore))
                    return false;

                if (!GestioneCrossControls.CI_VerificaRequisitoParticolareDirittoWithDanteCausa(datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, categoriaPensione, diretta.Certificato, diretta.SiglaCategoria,
                    datiPensione.DecorrenzaOriginaria, out errore))
                    return false;
            }

            if (IsSingleTabSaved)
            {
                if (DanteCausaDB.DataMorte.HasValue && diretta.DecorrenzaEliminazione.HasValue)
                {
                    DateTime dataMorte = DanteCausaDB.DataMorte.Value;
                    dataMorte = dataMorte.AddMonths(1);
                    if ((dataMorte.Month != diretta.DecorrenzaEliminazione.Value.Month) || (dataMorte.Year != diretta.DecorrenzaEliminazione.Value.Year))
                    {
                        errore = "Attenzione! La 'Decorrenza Eliminazione' deve essere obbligatoriamente: " + dataMorte.Month + "/" + dataMorte.Year + ". Verificare ed effettuare il salvataggio completo";
                        return false;
                    }
                }

                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(DanteCausaDB.CategoriaAltraPensione, diretta.NaturaPensione, diretta.SiglaCategoria, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_ControlsPensioneDirettaDanteCausa(diretta.Certificato, diretta.SiglaCategoria, diretta.Sede, diretta.DecorrenzaPensione, diretta.Maggiorazione781Contributi, diretta.NaturaPensione,
                        datiAnagraficiDC.DataNascita, DanteCausaDB.DataMorte, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiPensione.CausaCarico, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDecorrenzaOriginaria(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null,
                        diretta.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria))
                    {
                        errore = "Decorrenza art.6 L.140/544 antecedente a Decorrenza Diretta o 01/85";
                        return false;
                    }

                    if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria(
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, diretta.DecorrenzaPensione, DanteCausaDB.DataMorte, datiPensione.DecorrenzaOriginaria))
                    {
                        errore = "Decorrenza art.6 L.140/544 incompatibile con Data Morte/Decorrenza Diretta";
                        return false;
                    }
                }
            }
            else
            {
                if (anagrafDC.DataMorte.HasValue && diretta.DecorrenzaEliminazione.HasValue)
                {
                    DateTime dataMorte = anagrafDC.DataMorte.Value;
                    dataMorte = dataMorte.AddMonths(1);
                    if ((dataMorte.Month != diretta.DecorrenzaEliminazione.Value.Month) || (dataMorte.Year != diretta.DecorrenzaEliminazione.Value.Year))
                    {
                        errore = "Attenzione! La 'Decorrenza Eliminazione' deve essere obbligatoriamente: " + dataMorte.Month + "/" + dataMorte.Year;
                        return false;
                    }
                }

                if (tipoApp.HasValue && tipoApp.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(altraPensione != null ? altraPensione.CategoriaAltraPensione : null, diretta.NaturaPensione, diretta.SiglaCategoria, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_ControlsPensioneDirettaDanteCausa(diretta.Certificato, diretta.SiglaCategoria, diretta.Sede, diretta.DecorrenzaPensione, diretta.Maggiorazione781Contributi, diretta.NaturaPensione,
                        datiAnagraficiDC.DataNascita, anagrafDC.DataMorte, datiPensione.SiglaCategoria, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, datiPensione.CausaCarico, out errore))
                        return false;

                    if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDecorrenzaOriginaria(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null,
                        diretta.DecorrenzaPensione, datiPensione.DecorrenzaOriginaria))
                    {
                        errore = "Decorrenza art.6 L.140/544 antecedente a Decorrenza Diretta o 01/85";
                        return false;
                    }

                    if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria(
                        datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 : null, diretta.DecorrenzaPensione, anagrafDC != null ? anagrafDC.DataMorte : null, datiPensione.DecorrenzaOriginaria))
                    {
                        errore = "Decorrenza art.6 L.140/544 incompatibile con Data Morte/Decorrenza Diretta";
                        return false;
                    }
                }
            }

            #region codice commentato
            //DA SPOSTARE SU CALCOLO
            //if (tipoApp.Value == Utility.TipoAppartenenza.CI)
            //{
            //    List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(numeroDomanda, out listaPrestazioniEstere);

            //    if (listaPrestazioniEstere != null)
            //    {
            //        List<StatoEstero> listaStatiEsteri = new List<StatoEstero>();

            //        foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEstere)
            //        {
            //            StatoEstero statoEstero = new StatoEstero();
            //            statoEstero.PrestazioneEstera = new PrestazioneEstera();
            //            Utility.ValorizzaOggetti(prestazioneEE, statoEstero.PrestazioneEstera);
            //            listaStatiEsteri.Add(statoEstero);
            //        }

            //        if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            //        {
            //            foreach (StatoEstero stato in listaStatiEsteri)
            //            {
            //                if (!GestioneCrossControls.VerificaSettimaneEstereTipoCalcDanteCausa(TipoCalcolo.GetHashCode(), stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, DecorrenzaOriginaria, diretta.DecorrenzaPensione))
            //                {
            //                    errore = "Settimane Estere maggiori di 2080";
            //                    return false;
            //                }
            //            }
            //        }
            //    }

            //    #region Controlli R.M.S.
            //    //DA SPOSTARE SU CALCOLO
            //    //List<BLCommon.GestioneCalcolo.DatiCalcoloRetributivo> lDatiCalcoloRetrib = null;
            //    //BLCommon.GestioneCalcolo.GetCalcoloRetributivoCI_AGOByPensione(datiPensione.Id, out lDatiCalcoloRetrib);

            //    //GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            //    //GestioneNuoveLiquidate.GetNuoveLiquidateByNumeroDomanda(numeroDomanda, out nuoveLiquidate);

            //    //GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            //    //GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

            //    //if (lDatiCalcoloRetrib == null)
            //    //    lDatiCalcoloRetrib = new List<GestioneCalcolo.DatiCalcoloRetributivo>();

            //    //if (nuoveLiquidate == null)
            //    //    nuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();

            //    //if (datiIstruttoria == null)
            //    //    datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

            //    //foreach (GestioneCalcolo.DatiCalcoloRetributivo retrib in lDatiCalcoloRetrib)
            //    //{
            //    //    if (!GestioneCrossControls.VerificaRMSDanteCausa(diretta.Certificato, diretta.DecorrenzaPensione, retrib.RMSQuotaA, datiPensione.InizioAssicurazione,
            //    //         datiPensione.SiglaCategoria, DanteCausaDB.DataMorte, datiIstruttoria.DecorrenzaOpzione, nuoveLiquidate.FlagContributiva, datiPensione.NaturaPensione, datiPensione.Gruppo))
            //    //    {
            //    //        errore = "R.M.S. mancante.";
            //    //        return false;
            //    //    }
            //    //}

            //    #endregion Controlli R.M.S.

            //}
            #endregion codice commentato

            return true;
        }

        #endregion DatiPensioneDiretta

        #region RedditiSentenza49593

        public static void GetDatiRedditiSentenza495_93ByIdPensione(long idPensione, ref Entity.DanteCausaEntity entityDanteCausa)
        {
            List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiRedditiSentenza495_93BL = null;

            BLCommon.GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(idPensione, out lDatiRedditiSentenza495_93BL);
            if (lDatiRedditiSentenza495_93BL == null)
                return;

            entityDanteCausa.DatiRedditiSentenza495_93 = new INPS.Pensioni.Liquidazione.Entity.DatiRedditiSentenza495_93();
            entityDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93 = new List<Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93>();
            foreach (BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redditiSentenza495_93BL in lDatiRedditiSentenza495_93BL)
            {
                Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93 redditoSentenza495_93 = new Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93();
                Utility.ValorizzaOggetti(redditiSentenza495_93BL, redditoSentenza495_93);
                entityDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93.Add(redditoSentenza495_93);
            }
        }

        public static string StoreDatiRedditiSentenza495_93ByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.DatiRedditiSentenza495_93 datiRedditiSentenza495_93,
            Entity.DatiPensioneDiretta datiPensioneDiretta, Entity.AnagraficaDC anagraficaDC, GestioneAnagrafica.DatiAnagrafici datiAnagraficaDC, ref BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            ref GestioneQuadri.DatiQuadroDanteCausa datiQuadroQuadroDanteCausa, bool IsSingleTabSaved, bool isRiaperturaDomanda, List<GestioneFamiliari.Familiare> listaFamiliari, decimal? importoMensilePensioneEstera, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici)
        {
            string errore = string.Empty;

            Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (IsSingleTabSaved)
            {
                datiPensioneDiretta = new DatiPensioneDiretta();
                Utility.ValorizzaOggetti(datiDanteCausa, datiPensioneDiretta);

                anagraficaDC = new AnagraficaDC();
                Utility.ValorizzaOggetti(datiAnagraficaDC, anagraficaDC);
                Utility.ValorizzaOggetti(datiDanteCausa, anagraficaDC);
            }

            if (tipoApp.Value != Utility.TipoAppartenenza.FS)
            {
                //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> lDatiSentenza495_93 = null;
                if (tipoApp.Value == Utility.TipoAppartenenza.CI && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                    BLCommon.GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(datiPensione.Id, out lDatiSentenza495_93);

                if ((datiRedditiSentenza495_93 != null && datiRedditiSentenza495_93.LredditiSentenza495_93 != null && datiRedditiSentenza495_93.LredditiSentenza495_93.Count > 0)
                    || (tipoApp.Value == Utility.TipoAppartenenza.CI && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)))
                {
                    if (ControlsDatiRedditiSentenza495_93(datiRedditiSentenza495_93 != null ? datiRedditiSentenza495_93.LredditiSentenza495_93 : null, datiPensione.DecorrenzaOriginaria, anagraficaDC != null ? anagraficaDC.DataMorte : null,
                        datiPensioneDiretta != null ? datiPensioneDiretta.DecorrenzaPensione : null, anagraficaDC != null ? anagraficaDC.ProvenienzaPensione : null, IsSingleTabSaved, datiPensione, listaFamiliari, tipoApp, datiDanteCausa, isRiaperturaDomanda, out errore))
                    {
                        using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                        {
                            BLCommon.GestioneDanteCausa.DeleteRedditiSentenza495_93ByIdPensione(datiPensione.Id);

                            if (datiRedditiSentenza495_93 != null)
                            {
                                foreach (Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93 redditiSentenza495_93 in datiRedditiSentenza495_93.LredditiSentenza495_93)
                                {
                                    BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redditoSentenza495_93BL = new BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93();
                                    Utility.ValorizzaOggetti(redditiSentenza495_93, redditoSentenza495_93BL);
                                    redditoSentenza495_93BL.IdPensione = datiPensione.Id;

                                    //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                                    if (tipoApp.Value == Utility.TipoAppartenenza.CI && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                                    {
                                        if (lDatiSentenza495_93 != null && lDatiSentenza495_93.Count() > 0)
                                        {
                                            foreach (BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redd in lDatiSentenza495_93)
                                            {
                                                if (redditiSentenza495_93.AnnoSentenza == redd.AnnoSentenza)
                                                {
                                                    if (redd.MeseSentenza.HasValue)
                                                        redditoSentenza495_93BL.MeseSentenza = redd.MeseSentenza;
                                                    if (redd.CodiceSentenza.HasValue)
                                                        redditoSentenza495_93BL.CodiceSentenza = redd.CodiceSentenza;
                                                }
                                            }
                                        }
                                    }
                                    BLCommon.GestioneDanteCausa.SalvaRedditiSentenza495_93(redditoSentenza495_93BL, datiPensione);
                                }
                            }

                            //ENG - Gestione Pensione Estera e redditi Sentenza 495
                            if (tipoApp.Value == Utility.TipoAppartenenza.CI)
                            {
                                if (datiGenerici == null)
                                    datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                                datiGenerici.ImportoMensilePensioneEstera = importoMensilePensioneEstera;
                                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);
                            }

                            datiQuadroQuadroDanteCausa.TabSentenza49593 = 2;

                            if (Utility.IsDomandaSOAUT_Supplementare(datiPensione, isRiaperturaDomanda) || (Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione) && (datiPensione.Contributivo == '8' || datiPensione.Contributivo == '5')))
                                datiQuadroQuadroDanteCausa.TabSentenza49593 = null;

                            GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroQuadroDanteCausa, datiPensione);

                            transactionScope.Complete();
                        }
                    }
                }
            }
            return errore;
        }

        public static bool ControlsDatiRedditiSentenza495_93(List<Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93> lRedditiSentenza495_93, DateTime? decorrenzaOriginaria, DateTime? dataMorteDC,
            DateTime? decorrenzaPensioneDiretta, byte? provenienzaPensione, bool IsSingleTabSaved, GestionePensione.DatiPensione datiPensione, List<GestioneFamiliari.Familiare> listaFamiliari, Utility.TipoAppartenenza? tipoAppartenenza,
             BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isRiaperturaDomanda, out string errore)
        {
            errore = string.Empty;

            if (lRedditiSentenza495_93 != null && lRedditiSentenza495_93.Count > 0)
            {
                List<DatiRedditiSentenza495_93.RedditoSentenza495_93> redditi = lRedditiSentenza495_93.Where(x => x.FlagSentenza == null).ToList();

                //conrolli sulle righe della tabella che sono redditi
                if (redditi.FindIndex(x => x.IsPre2009.GetValueOrDefault() && x.AnnoReddito.GetValueOrDefault() < 1983) > -1)
                {
                    errore = "Il campo 'Anno' per 'Sentenza 495/93' Dante Causa ante 2009 deve essere maggiore o uguale al 1983";
                    return false;
                }

                if (redditi.FindIndex(x => x.IsPre2009.GetValueOrDefault() && x.AnnoReddito.GetValueOrDefault() > 2008) > -1)
                {
                    errore = "Il campo 'Anno' per 'Sentenza 495/93' Dante Causa ante 2009 deve essere minore o uguale al 2008";
                    return false;
                }

                if (redditi.FindIndex(x => !x.IsPre2009.GetValueOrDefault() && x.AnnoReddito.GetValueOrDefault() < 2009 && x.AnnoSentenza == null) > -1)
                {
                    errore = "Il campo 'Anno' per 'Sentenza 495/93' Dante Causa post 2008 deve essere maggiore al 2008";
                    return false;
                }

                if (redditi.FindIndex(x => !x.IsPre2009.GetValueOrDefault() && x.AnnoReddito.GetValueOrDefault() > decorrenzaOriginaria.Value.Year) > -1)
                {
                    errore = "Il campo 'Anno' per 'Sentenza 495/93' Dante Causa post 2008 deve essere minore o uguale alla Decorrenza Originaria";
                    return false;
                }

                List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> lRedditiSentenza495_93BL = null;
                ConvertDatiRedditiSentenza495_93FromEntityToBL(lRedditiSentenza495_93, out lRedditiSentenza495_93BL);

                if (!GestioneCrossControls.AGO_CI_ControlsRedditiSentenza495_93(lRedditiSentenza495_93BL, decorrenzaOriginaria, dataMorteDC, decorrenzaPensioneDiretta, provenienzaPensione, datiPensione, listaFamiliari, tipoAppartenenza, datiDanteCausa, isRiaperturaDomanda, out errore))
                    return false;
            }

            //ENG - Aggiornamento Modifica Sentenza 495 - per decorrenza precedente al 2009 tale messaggio "E' obbligatorio acquisire l'anno" deve essere rimosso
            else
            {
                if (tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && !((dataMorteDC.HasValue && dataMorteDC.Value.Year < 2009) || (decorrenzaPensioneDiretta.HasValue && decorrenzaPensioneDiretta.Value.Year < 2009)))
                {
                    errore = "E' obbligatorio acquisire l'anno";
                    return false;
                }
            }

            return true;
        }

        public static void EliminaDatiRedditiSentenza495_93(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDanteCausa datiQuadroDanteCausa = null;
            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out datiQuadroDanteCausa);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                INPS.Pensioni.Liquidazione.BLCommon.GestioneDanteCausa.DeleteRedditiSentenza495_93ByIdPensione(datiPensione.Id);

                //ENG - Aggiornamento Modifica Sentenza 495 
                if (tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                    datiQuadroDanteCausa.TabSentenza49593 = 0;
                else

                    datiQuadroDanteCausa.TabSentenza49593 = 1;

                GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, datiQuadroDanteCausa, datiPensione);

                transactionScope.Complete();
            }
        }

        public static void ConvertDatiRedditiSentenza495_93FromEntityToBL(List<Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93> lRedditiSentenza495_93, out List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> lRedditiSentenza495_93BL)
        {
            lRedditiSentenza495_93BL = null;
            if (lRedditiSentenza495_93 != null && lRedditiSentenza495_93.Count > 0)
            {
                lRedditiSentenza495_93BL = new List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93>();
                foreach (Entity.DatiRedditiSentenza495_93.RedditoSentenza495_93 redditiSentenza495_93 in lRedditiSentenza495_93)
                {
                    BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redditiSentenza495_93BL = new BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93();
                    Utility.ValorizzaOggetti(redditiSentenza495_93, redditiSentenza495_93BL);
                    lRedditiSentenza495_93BL.Add(redditiSentenza495_93BL);
                }
            }
        }

        public static void GetCrossPropertiesDatiRedditiSentenza495_93(DateTime? dataMorteDC, DateTime? decorrenzaDiretta, ref DatiRedditiSentenza495_93 datiRedditiSentenza495_93)
        {
            DateTime dataPost2008 = new DateTime(2008, 12, 1);

            datiRedditiSentenza495_93.IsDCSentenza495_93Ante2009 = false;
            if ((dataMorteDC.HasValue && dataMorteDC.Value.Year < 2009) || (decorrenzaDiretta.HasValue && decorrenzaDiretta.Value.Year < 2009))
            {
                datiRedditiSentenza495_93.IsDCSentenza495_93Ante2009 = true;
            }

            datiRedditiSentenza495_93.IsDCSentenza495_93Post2008 = false;
            if ((dataMorteDC.HasValue && Utility.DataSuccessivaA(dataMorteDC.Value, dataPost2008)) || (decorrenzaDiretta.HasValue && decorrenzaDiretta.Value.Year > 2008))
            {
                datiRedditiSentenza495_93.IsDCSentenza495_93Post2008 = true;
            }
        }

        #endregion RedditiSentenza49593

        #region Get Liste Decodifica

        public static void GetListaCodiceEliminazione(out List<CodiceEliminazione> listaCodiceEliminazione, Utility.TipoAppartenenza? tipoApp)
        {
            listaCodiceEliminazione = new List<CodiceEliminazione>();
            List<GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazioneDB = null;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out elencoCodiceEliminazioneDB, tipoApp);

            if (elencoCodiceEliminazioneDB != null)
            {
                foreach (GestioneDecodifica.CodiceEliminazione codiceEliminazioneDB in elencoCodiceEliminazioneDB)
                {
                    CodiceEliminazione codiceEliminazione = new CodiceEliminazione();
                    Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                    listaCodiceEliminazione.Add(codiceEliminazione);
                }
            }
        }

        #endregion Get Liste Decodifica

        #region nestedclass

        public class StatoEstero
        {
            #region private properties
            private PrestazioneEstera _PrestazioneEstera;

            private List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> _ElencoImportiEsteri;
            #endregion private properties

            #region public properties
            public PrestazioneEstera PrestazioneEstera { get { return _PrestazioneEstera; } set { _PrestazioneEstera = value; } }

            public List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> ElencoImportiEsteri { get { return _ElencoImportiEsteri; } set { _ElencoImportiEsteri = value; } }

            #endregion public properties

        }

        public class PrestazioneEstera : GestioneDatiContributiviCi.PensioniCiPrestazioniEE
        {
            public PrestazioneEstera()
            { }

            public PrestazioneEstera(string codiceStatoIstituzione, string sigla, string citta,
                string nomeStato, string siglaStato, string codiceConvenzione, string matricolaIstituzione)
            {
                this.CodiceStatoEE = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(0, 2) : "";
                this.CodiceIstituzione = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(2, 4) : "";
                this._Sigla = sigla;
                this._Citta = citta;
                this._NomeStato = nomeStato;
                this._SiglaStato = siglaStato;
                this.CodiceConvenzione = Utility.StringToNullableByte(codiceConvenzione);
                this._MatricolaIstituzione = matricolaIstituzione;
            }
            #region private properties
            private string _Sigla;
            private string _Citta;
            private string _NomeStato;
            private string _SiglaStato;
            private string _MatricolaIstituzione;

            #endregion private properties

            #region public properties
            public string Sigla { get { return _Sigla; } set { _Sigla = value; } }
            public string Citta { get { return _Citta; } set { _Citta = value; } }
            public string NomeStato { get { return _NomeStato; } set { _NomeStato = value; } }
            public string SiglaStato { get { return _SiglaStato; } set { _SiglaStato = value; } }
            public string MatricolaIstituzione { get { return _MatricolaIstituzione; } set { _MatricolaIstituzione = value; } }

            #endregion public properties
        }

        #endregion nestedclass
    }
}
