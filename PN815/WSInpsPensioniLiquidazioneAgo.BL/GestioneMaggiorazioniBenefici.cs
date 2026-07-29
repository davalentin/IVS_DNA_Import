using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneMaggiorazioniBenefici
    {
        #region MaggiorazioniBenefici

        public static bool ControlsVisibleTabs(GestionePensione.DatiPensione datiPensione, bool? IsExCombattente, bool? IsBenefici, bool? IsMaggiorazioni, bool? IsBeneficioVittimeTerrorismo)
        {
            if (IsExCombattente.HasValue && IsExCombattente.Value && datiPensione.ExCombattente.HasValue && datiPensione.ExCombattente.Value)
                return true;
            if (IsBenefici.HasValue && IsBenefici.Value && datiPensione.Benefici.HasValue && datiPensione.Benefici.Value)
                return true;
            if (IsMaggiorazioni.HasValue && IsMaggiorazioni.Value && datiPensione.Maggiorazioni.HasValue && datiPensione.Maggiorazioni.Value)
                return true;
            if (IsBeneficioVittimeTerrorismo.HasValue && IsBeneficioVittimeTerrorismo.Value)
                return true;

            return false;
        }

        public static void GetMaggiorazioneBenefici(ref EntityBLCommon.ContenitoreObject contenitore, out DatiExCombattente datiExCombattente, out DatiBenefici datiBenefici,
            out DatiMaggiorazioni datiMaggiorazioni, out DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, out DatiMaggiorazioneBeneficiStorico datiMaggiorazioneBeneficiStorico)
        {
            datiExCombattente = null;
            ValorizzaDatiExCombattente(ref contenitore, out datiExCombattente);

            datiBenefici = null;
            ValorizzaDatiBeneficiByDatiPensione(ref contenitore, out datiBenefici);

            datiMaggiorazioni = null;
            ValorizzaDatiMaggiorazioni(ref contenitore, out datiMaggiorazioni);

            GetDatiBeneficioVittimeTerrorismo(ref contenitore, out datiBeneficioVittimeTerrorismo);

            GetDatiMaggiorazioneBeneficiStorico(ref contenitore, out datiMaggiorazioneBeneficiStorico);
        }

        #endregion MaggiorazioniBenefici

        #region DatiExCombattente

        public static bool ControlDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore, DatiExCombattente datiExCombattente, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaAgo;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRilascioRIC_31082020", out ctrl);
            DateTime? decorrenza = null;
            if (ctrl != null && ctrl.ValoreControllo == "NO")
                decorrenza = contenitore.DatiPensione != null ? contenitore.DatiPensione.DecorrenzaOriginaria : null;
            else
                decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(contenitore.DatiPensione != null ? contenitore.DatiPensione.DecorrenzaOriginaria : null, contenitore.DatiDanteCausa != null ? contenitore.DatiDanteCausa.DecorrenzaPensione : null);

            if (Utility.IsDomandaIOCUM(contenitore.DatiPensione != null ? contenitore.DatiPensione.SiglaCategoria : null) && Utility.IsDomandaPensioneInabilita(contenitore.DatiPensione) && decorrenza != null)
            {
                decorrenza = new DateTime(((DateTime)decorrenza).Year, ((DateTime)decorrenza).Month, 1);
            }

            if (datiExCombattente != null)
            {
                if (!GestioneControlli.ControlsExCombattente(datiExCombattente.CodiceCieco, datiExCombattente.DecorrenzaMaggiorazioneArt6, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (datiExCombattente.IsDatiExCombattenteNull())
                {
                    messaggioVideo = "Inserire almeno un dato 'Ex Combattente' prima di procedere con il salvataggio";
                    return false;
                }

                if (datiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue)
                {
                    if (!datiExCombattente.CodiceCieco.HasValue)
                    {
                        messaggioVideo = "In presenza della 'Decorrenza' della Legge 140 è obbligatorio inserire il 'Codice ex Combattente'";
                        return false;
                    }

                    if (!Utility.DataSuccessivaA(datiExCombattente.DecorrenzaMaggiorazioneArt6.Value, decorrenza.GetValueOrDefault()) ||
                        Utility.DataStrettamenteSuccessivaA(datiExCombattente.DecorrenzaMaggiorazioneArt6.Value, new DateTime(dataSistema.AddMonths(1).Year, dataSistema.AddMonths(1).Month, 1)))
                    {
                        messaggioVideo = string.Format("La 'Decorrenza' della Legge 140 deve essere maggiore o uguale alla 'Decorrenza Pensione' ({0}) e fino al {1}", decorrenza.Value.ToString("MM/yyyy"), dataSistema.AddMonths(1).ToString("MM/yyyy"));
                        return false;
                    }

                    if (!GestioneCrossControls.ALL_ControlsDecorrenzaExCombattenteWithDataPresentazione(datiExCombattente.DecorrenzaMaggiorazioneArt6.Value, contenitore.DatiPensione, out messaggioVideo))
                        return false;
                }
            }
            return true;
        }

        public static void ValorizzaDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore, out DatiExCombattente datiExCombattente)
        {
            datiExCombattente = new Entity.DatiExCombattente();
            if (contenitore.DatiMaggiorazioniBenefici != null)
                Utility.ValorizzaOggetti(contenitore.DatiMaggiorazioniBenefici, datiExCombattente);

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiEnpals != null)
                datiExCombattente.NumeroContributiNLNonVedenti = contenitore.DatiEnpals.NumeroContributiNLNonVedenti;

            if (datiExCombattente.IsDatiExCombattenteNull() && datiExCombattente.IsDatiExCombattenteENPALSNull())
                datiExCombattente = null;
        }

        public static void StoreDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore, DatiExCombattente datiExCombattente)
        {
            if (datiExCombattente == null)
                datiExCombattente = new DatiExCombattente();

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = contenitore.DatiMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiExCombattentePrivate(datiPensione.Id, datiExCombattente, ref datiMaggiorazioniBenefici);

                if (datiExCombattente.IsDatiExCombattenteNull())
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = 2;

                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                        (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            //--------------------------------------------------------------------
        }

        private static void StoreDatiExCombattentePrivate(long idPensione, DatiExCombattente datiExCombattente,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiExCombattente, datiMaggiorazioniBenefici);
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            //----------------------------------------------------------------

            if (datiMaggiorazioniBeneficiCommon != null)
            {
                datiMaggiorazioniBeneficiCommon.CodiceCieco = null;
                datiMaggiorazioniBeneficiCommon.DecorrenzaMaggiorazioneArt6 = null;
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBeneficiCommon != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiCommon))
                    {
                        datiMaggiorazioniBeneficiCommon = null;
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    }
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiCommon);
                }
                datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBeneficiCommon;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            //--------------------------------------------------------------------
        }

        #endregion DatiExCombattente

        #region Oneri Benefici Particolari
        #region Dati Benefici Particolari

        private static void StoreDatiBeneficiForBeneficiParticolari(List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariDB, GestionePensione.DatiPensione datiPensione)
        {
            if (lBeneficiParticolariDB != null && lBeneficiParticolariDB.Count > 0)
            {
                //Commentata a seguito della Mail del 10/04/2014 FW: Reeng Pensioni AGO - Prepensionamenti. Ad oggi non è prevista una gestione UNICARPE
                //// i dati provenienti da felpe sono non modificabili e non cancellabili 
                //if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Lettura_L)
                //{
                //    if (lBeneficiParticolariDB != null)
                //    {
                //        lDatiBeneficiParticolari = new List<Entity.DatiBeneficiParticolari>();
                //        foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari beneficiParticolariDB in lBeneficiParticolariDB)
                //        {
                //            Entity.DatiBeneficiParticolari beneficiParticolariBL = new Entity.DatiBeneficiParticolari();
                //            Utility.ValorizzaOggetti(beneficiParticolariDB, beneficiParticolariBL);
                //            lDatiBeneficiParticolari.Add(beneficiParticolariBL);
                //        }
                //    }
                //}

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                    foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari beneficiParticolariCommon in lBeneficiParticolariDB)
                    {
                        if (!beneficiParticolariCommon.IsDatiBeneficiParticolariNull())
                        {
                            beneficiParticolariCommon.IdPensione = datiPensione.Id;
                            GestioneBeneficiParticolari.SalvaDatiBeneficiParticolari(beneficiParticolariCommon);
                        }
                    }

                    transactionScope.Complete();
                }
            }
        }

        #endregion Dati Benefici Particolari

        #endregion Oneri Benefici Particolari

        #region Dati Benefici

        public static bool ControlDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore, DatiBenefici datiBenefici, bool IsCancelOperation, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData
            GestioneDanteCausa.DatiDanteCausa datiDA = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(contenitore.DatiPensione))
                datiDA = contenitore.DatiDanteCausa;
            GestioneEnpals.DatiEnpals datiENPALS = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                datiENPALS = contenitore.DatiEnpals;
            #endregion GetData

            if (datiBenefici != null)
            {
                if (!IsCancelOperation)
                {
                    //forzature per categorie
                    if (string.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio) && Utility.IsDomandaSPED(contenitore.DatiPensione))
                    {
                        messaggioVideo = "Inserire il tipo benificio prima di procedere con il salvataggio";
                        return false;
                    }
                    else
                    {
                        if (datiBenefici.IsDatiBeneficiNull())
                        {
                            messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                            return false;
                        }
                    }
                }

                if (!GestioneControlli.ControlsSettimaneIncremento(datiBenefici.NSettimaneIncremento1Percento, datiBenefici.NSettimaneIncremento05Percento,
                    contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBeneficio(datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.CodiceLiquidazione : (char?)null, datiDA != null ? datiDA.ProvenienzaPensione : (byte?)null,
                    datiDA != null ? datiDA.DecorrenzaPensione : (DateTime?)null, contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NSettimaneOBG : (int?)null,
                    contenitore.DatiIstruttoria != null ? contenitore.DatiIstruttoria.NContributiVolontari : null,
                    contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.Sesso : null,
                    contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null, datiENPALS, contenitore.IsRiaperturaDomanda, datiBenefici.SettAnzContribPost311295,
                    contenitore.DatiPensioniDatiGenerici != null ? contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico : (DateTime?)null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSentenze(datiBenefici.Sentenza495240, contenitore.DatiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsTipoBeneficiForPensioneInabilitaIndiretta(!string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione[0] : ' ',
                    !string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione[2] : ' ',
                    contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, datiBenefici.TipoSettimaneBeneficio, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsTipoBeneficiForCumulo(!string.IsNullOrEmpty(contenitore.DatiPensione.NaturaPensione) ? contenitore.DatiPensione.NaturaPensione[0] : ' ',
                    contenitore.DatiPensione, contenitore.IsRiaperturaDomanda, datiBenefici.TipoSettimaneBeneficio, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio, datiBenefici.SettAnzContribPost311295, contenitore.DatiPensione, contenitore.DatiDanteCausa, out messaggioVideo))
                    return false;
            }
            else
            {
                messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                return false;
            }
            return true;
        }

        public static void ValorizzaDatiBeneficiByDatiPensione(ref EntityBLCommon.ContenitoreObject contenitore, out DatiBenefici datiBenefici)
        {
            datiBenefici = new DatiBenefici();

            if (contenitore.DatiMaggiorazioniBenefici != null)
                Utility.ValorizzaOggetti(contenitore.DatiMaggiorazioniBenefici, datiBenefici);

            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione) && contenitore.DatiEnpals != null)
                datiBenefici.IndicatoreInvalidita80 = contenitore.DatiEnpals.IndicatoreInvalidita80;

            if (datiBenefici.IsDatiBeneficiNull() && datiBenefici.IsDatiBeneficiENPALSNull())
                datiBenefici = null;

            if (contenitore.ListaDatiRipartizioneFondi != null && contenitore.ListaDatiRipartizioneFondi.Count > 0)
            {
                if (datiBenefici == null)
                    datiBenefici = new DatiBenefici();
                datiBenefici.ListOneriTerrorismo = new List<DatiBenefici.OneriTerrorismo>();
                foreach (GestioneRipartizioneFondi.DatiRipartizioneFondi rf in contenitore.ListaDatiRipartizioneFondi)
                {
                    DatiBenefici.OneriTerrorismo ot = new DatiBenefici.OneriTerrorismo();
                    Utility.ValorizzaOggetti(rf, ot);
                    datiBenefici.ListOneriTerrorismo.Add(ot);
                }
            }
            //ENG - Prepensionamento Editoria EBA: Gestione Quadro Maggiorazione Benefici
            if (Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione) && (!Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) || Utility.IsRicostituzione_MotiviContributivi(contenitore.DatiPensione))
                && !(Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo) && Utility.IsDomandaVOPGI(contenitore.DatiPensione.SiglaCategoria) && !contenitore.DatiPensione.DataPerfezionamentoRequisiti.HasValue && !String.IsNullOrEmpty(contenitore.DatiPensione.GP1AV91B) && contenitore.DatiPensione.GP1AV91B.Trim() == "2"))
            {
                if (datiBenefici == null)
                    datiBenefici = new DatiBenefici();

                int settimaneLimite = 1560;
                int settimaneAmmesse1 = 0;
                if (settimaneLimite > contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault())
                    settimaneAmmesse1 = settimaneLimite - contenitore.DatiIstruttoria.NSettimaneOBG.GetValueOrDefault();
                else
                    settimaneAmmesse1 = 0;

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(contenitore.DatiPensione.IndConvInt, contenitore.DatiPensione.Gestione);
                int reqAA = 0;
                int reqMM = 0;
                GestioneDecodifica.GetCtrlRequisitoEta_Base(contenitore.DatiPensione.DataPerfezionamentoRequisiti.GetValueOrDefault(), contenitore.DatiPensione.GetCodCategoria(),
                  contenitore.DatiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                int anni = reqAA;
                int mesi = reqMM;
                int settimaneAmmesse2 = 0;
                int settimaneDiffPerfReqTitolareEDataNasc = Utility.NSettimaneBetweenDate(contenitore.DatiAnagraficiTitolare.DataNascita.GetValueOrDefault().AddYears(anni).AddMonths(mesi), contenitore.DatiAnagraficiTitolare.DataNascita.GetValueOrDefault());
                int settimaneDiffDecPensioneEDataNasc = Utility.NSettimaneBetweenDate(contenitore.DatiPensione.DecorrenzaOriginaria.GetValueOrDefault(), contenitore.DatiAnagraficiTitolare.DataNascita.GetValueOrDefault());

                if (settimaneDiffPerfReqTitolareEDataNasc < settimaneDiffDecPensioneEDataNasc)
                    settimaneAmmesse2 = 0;
                else
                    settimaneAmmesse2 = settimaneDiffPerfReqTitolareEDataNasc - settimaneDiffDecPensioneEDataNasc;

                int settimaneIntegrazioneRicavate = Math.Min(settimaneAmmesse1, settimaneAmmesse2);
                datiBenefici.NSettIntegrazioneContributivaConcessa = Math.Max(0, settimaneIntegrazioneRicavate);
            }
        }

        public static void StoreDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, DatiBenefici datiBenefici)
        {
            if (datiBenefici != null)
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = contenitore.DatiMaggiorazioniBenefici;
                GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
                GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
                //----------------------------------------------------------------

                //Gestione di salvataggio del TipoBeneficio e del NumeroSettimaneBeneficio nella tabella BeneficiParticolari. Mail del 10/04/2014 FW: Reeng Pensioni AGO - Prepensionamenti
                ////////////////////////////////////////////////////////////////////////////////////////////////
                List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariBlCommon = null;

                if ((datiBenefici.TipoSettimaneBeneficio == "02" || datiBenefici.TipoSettimaneBeneficio == "04" || datiBenefici.TipoSettimaneBeneficio == "09" ||
                    datiBenefici.TipoSettimaneBeneficio == "16" || datiBenefici.TipoSettimaneBeneficio == "17") &&
                    (Utility.IsDomandaSalvaguardia122(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione) ||
                    Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) ||
                    Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione) || Utility.IsDomandaEsuberiPA(datiPensione) ||
                    Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione) || Utility.IsDomandaSalvaguardia232_2016(datiPensione) ||
                    Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) ||
                    Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) || Utility.IsDomandaVecchiaiaENAV(datiPensione)) ||
                    (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && datiBenefici.TipoSettimaneBeneficio == "01"))
                {
                    GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                    datiBeneficiParticolari.CodiceBenefici = datiBenefici.TipoSettimaneBeneficio;
                    if (datiBenefici.TipoSettimaneBeneficio == "01")
                        datiBeneficiParticolari.Settimane = datiBenefici.SettAnzContribPost311295;
                    else
                        datiBeneficiParticolari.Settimane = (short?)datiBenefici.NSettimaneBeneficio;
                    lBeneficiParticolariBlCommon = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();
                    lBeneficiParticolariBlCommon.Add(datiBeneficiParticolari);
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////

                List<GestioneRipartizioneFondi.DatiRipartizioneFondi> listaDatiRipartizioneFondi = null;
                if (datiBenefici.ListOneriTerrorismo != null && datiBenefici.ListOneriTerrorismo.Count > 0)
                {
                    listaDatiRipartizioneFondi = new List<GestioneRipartizioneFondi.DatiRipartizioneFondi>();
                    GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);
                    foreach (DatiBenefici.OneriTerrorismo ot in datiBenefici.ListOneriTerrorismo)
                    {
                        ot.IdPensione = datiPensione.Id;
                        GestioneRipartizioneFondi.DatiRipartizioneFondi datiRipartizioneFondi = new GestioneRipartizioneFondi.DatiRipartizioneFondi();
                        Utility.ValorizzaOggetti(ot, datiRipartizioneFondi);
                        listaDatiRipartizioneFondi.Add(datiRipartizioneFondi);
                    }
                }

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    StoreDatiBeneficiPerMaggiorazioniBenefici(datiPensione.Id, datiBenefici, ref datiMaggiorazioniBenefici);
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    //Se nella tab Benefici si è salvato Prepensionamenti (2), Amianto (4) o Ex Acna Cengio (9) occorrerà:
                    //Salvare il campo Beneficio anche in BeneficiParticolari.CodiceBenefici
                    //Salvare il campo Numero settimane beneficio anche in BeneficiParticolari.Settimane
                    if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) ||
                        Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) ||
                        (datiMaggiorazioniBenefici != null &&
                        (datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() ||
                        datiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() ||
                        (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))) ||
                        Utility.IsDomandaVecchiaiaENAV(datiPensione))
                        StoreDatiBeneficiForBeneficiParticolari(lBeneficiParticolariBlCommon, datiPensione);
                    ////////////////////////////////////////////////////////////////////////////////////////////////

                    #region RipartizioneFondi

                    if (listaDatiRipartizioneFondi != null && listaDatiRipartizioneFondi.Count > 0)
                    {
                        GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);
                        foreach (GestioneRipartizioneFondi.DatiRipartizioneFondi datiRipartizioneFondi in listaDatiRipartizioneFondi)
                            GestioneRipartizioneFondi.SalvaRipartizioneFondi(datiRipartizioneFondi);
                    }

                    #endregion RipartizioneFondi

                    if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01" && !Utility.IsDomandaRiliquidazione(datiPensione).GetValueOrDefault())
                    {
                        datiQuadroOneri.Tipo = 2;
                        if (datiQuadroOneri.TabOneri == null)
                            datiQuadroOneri.TabOneri = 0;
                        GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                    }

                    if (datiBenefici.IsDatiBeneficiNull())
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                    else
                        datiQuadroMaggiorazioniBenefici.TabBenefici = 2;

                    if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                        (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2))
                        datiQuadroMaggiorazioniBenefici.Tipo = 2;
                    else
                        datiQuadroMaggiorazioniBenefici.Tipo = 1;

                    GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
                contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
                if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione) ||
                    Utility.IsDomandaConBeneficioAmianto181(datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale) ||
                    (datiMaggiorazioniBenefici != null && (datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() || datiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() ||
                     (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))) || Utility.IsDomandaVecchiaiaENAV(datiPensione))
                    contenitore.ListaDatiBeneficiParticolari = lBeneficiParticolariBlCommon;
                contenitore.ListaDatiRipartizioneFondi = listaDatiRipartizioneFondi;
                //--------------------------------------------------------------------
            }
        }

        private static void StoreDatiBeneficiPerMaggiorazioniBenefici(long idPensione, DatiBenefici datiBenefici,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiBenefici, datiMaggiorazioniBenefici);
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            if (Utility.IsDomandaENPALS(contenitore.DatiPensione.Gestione))
                datiEnpals = contenitore.DatiEnpals;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            List<GestioneOneri.DatiOneri> listaDatiOneriCommon = contenitore.ListaDatiOneri;
            bool isClearQuadroOneri = false;
            //----------------------------------------------------------------

            if (datiMaggiorazioniBeneficiCommon != null)
            {
                if (datiMaggiorazioniBeneficiCommon.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() ||
                    datiMaggiorazioniBeneficiCommon.IsBeneficioApePrecociFromFELPE.GetValueOrDefault())
                {
                    datiMaggiorazioniBeneficiCommon.NSettimaneBeneficio = null;
                }
                else if (!datiPensione.Amianto181Unicarpe.GetValueOrDefault() && (!Utility.IsDomandaENPALS(datiPensione.Gestione) || datiEnpals == null || !datiEnpals.NumeroContributiNLNonVedenti.HasValue) &&
                    !Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) && !Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) &&
                    !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01"))
                {
                    if (datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01" && (listaDatiOneriCommon == null || listaDatiOneriCommon.Count == 0))
                        isClearQuadroOneri = true;
                    datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio = null;
                    datiMaggiorazioniBeneficiCommon.NSettimaneBeneficio = null;
                }

                if (!Utility.IsDomandaAUT(datiPensione))
                {
                    datiMaggiorazioniBeneficiCommon.NSettimaneIncremento1Percento = null;
                    datiMaggiorazioniBeneficiCommon.NSettimaneIncremento05Percento = null;
                }

                datiMaggiorazioniBeneficiCommon.Sentenza495240 = null;
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBeneficiCommon != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiCommon))
                    {
                        datiMaggiorazioniBeneficiCommon = null;
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    }
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiCommon);
                }
                GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);
                if (!datiPensione.Amianto181Unicarpe.GetValueOrDefault() &&
                    !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon != null &&
                      datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01"))
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                if (isClearQuadroOneri)
                {
                    datiQuadroOneri.Tipo = 0;
                    datiQuadroOneri.TabOneri = null;
                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                }

                datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                if (Utility.IsRicostituzioneOrRiapertura(contenitore.DatiPensione, contenitore.IsRiaperturaDomanda) && (Utility.IsDomandaCOOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaCRED27(datiPensione.SiglaCategoria)))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBeneficiCommon;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.ListaDatiRipartizioneFondi = null;
            if (!datiPensione.Amianto181Unicarpe.GetValueOrDefault())
                contenitore.ListaDatiBeneficiParticolari = null;
            //--------------------------------------------------------------------
        }

        public static void ValorizzaDatiBeneficiForPrepensionamento(ref EntityBLCommon.ContenitoreObject contenitore, ref DatiBenefici datiBenefici)
        {
            int codiceLegge = 0;
            string tipoSettimaneBeneficio = string.Empty;

            if (Utility.IsTabPrepensionamentoVisible(contenitore.DatiPensione, contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale,
                contenitore.DatiPensione.NaturaPensione, out codiceLegge, out tipoSettimaneBeneficio))
            {
                if (datiBenefici == null)
                    datiBenefici = new DatiBenefici();

                if (string.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio))
                    datiBenefici.TipoSettimaneBeneficio = tipoSettimaneBeneficio;
            }
        }

        #endregion Dati Benefici

        #region Dati Maggiorazioni

        public static void ValorizzaDatiMaggiorazioni(ref EntityBLCommon.ContenitoreObject contenitore, out DatiMaggiorazioni datiMaggiorazioni)
        {
            datiMaggiorazioni = null;
            if (contenitore.DatiMaggiorazioniBenefici != null)
            {
                datiMaggiorazioni = new DatiMaggiorazioni();
                Utility.ValorizzaOggetti(contenitore.DatiMaggiorazioniBenefici, datiMaggiorazioni);
                if (datiMaggiorazioni.IsDatiMaggiorazioniNull())
                    datiMaggiorazioni = null;
            }
        }

        public static bool ControlDatiMaggiorazioni(ref EntityBLCommon.ContenitoreObject contenitore, DatiMaggiorazioni datiMaggiorazioni, bool IsCancelOperation, bool isRiaperturaDomanda, DateTime dataSistema,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiMaggiorazioni != null)
            {
                if (!GestioneControlli.ControlsDecMaggiorazioneSociale(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare, dataSistema,
                    contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DecorrenzaMaggiorazioneSociale : null, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsCessMaggiorazioneSociale(datiMaggiorazioni.CessazioneMaggiorazioneSociale, dataSistema, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsAnniRiduzioneBenefici(dataSistema, datiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02, datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione,
                    isRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (datiMaggiorazioni.CessazioneMaggiorazioneSociale.HasValue && !datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.HasValue)
                {
                    messaggioVideo = "In presenza della data Cessazione è obbligatorio inserire la data Decorrenza";
                    return false;
                }

                if ((datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.HasValue && datiMaggiorazioni.CessazioneMaggiorazioneSociale.HasValue &&
                    datiMaggiorazioni.CessazioneMaggiorazioneSociale.Value.CompareTo(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.Value) < 0))
                {
                    messaggioVideo = "La data Cessazione deve essere maggiore della data Decorrenza";
                    return false;
                }
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(contenitore.DatiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Maggiorazioni_AGO.MAGG_SOCIALE_DATA_PRESENT) &&
                    !GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, contenitore.DatiPensione, contenitore.DatiAnagraficiTitolare != null ? contenitore.DatiAnagraficiTitolare.DataNascita : null,
                    contenitore.DatiStoricoGP != null ? contenitore.DatiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsMaggiorazioneSocialeCUM(ref contenitore, out messaggioVideo))
                    return false;
            }
            return true;
        }

        public static void StoreDatiMaggiorazioni(ref EntityBLCommon.ContenitoreObject contenitore, DatiMaggiorazioni datiMaggiorazioni)
        {
            if (datiMaggiorazioni != null)
            {
                // Con queste istruzioni forzo la get dei dati
                //----------------------------------------------------------------
                GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
                Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = contenitore.DatiMaggiorazioniBenefici;
                GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
                //----------------------------------------------------------------

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    StoreDatiMaggiorazioniPerMaggiorazioniBenefici(datiPensione.Id, datiMaggiorazioni, ref datiMaggiorazioniBenefici);

                    if (datiMaggiorazioni.IsDatiMaggiorazioniNull())
                        datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 0;
                    else
                        datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 2;

                    if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                        (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                        (datiQuadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && datiQuadroMaggiorazioniBenefici.TabMaggiorazioni.Value == 2))
                        datiQuadroMaggiorazioniBenefici.Tipo = 2;
                    else
                        datiQuadroMaggiorazioniBenefici.Tipo = 1;

                    GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                    transactionScope.Complete();
                }

                // Aggiorno i dati sul contenitore
                //--------------------------------------------------------------------
                contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
                contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
                //--------------------------------------------------------------------
            }
        }

        private static void StoreDatiMaggiorazioniPerMaggiorazioniBenefici(long idPensione, DatiMaggiorazioni datiMaggiorazioni,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiMaggiorazioni, datiMaggiorazioniBenefici);

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiMaggiorazioni(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon = contenitore.DatiMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            //----------------------------------------------------------------

            if (datiMaggiorazioniBeneficiCommon != null)
            {
                datiMaggiorazioniBeneficiCommon.CessazioneMaggiorazioneSociale = null;
                datiMaggiorazioniBeneficiCommon.DecorrenzaMaggiorazioneSociale = null;
                datiMaggiorazioniBeneficiCommon.AnniRiduzioneBeneficiArt38Legge02 = null;
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBeneficiCommon != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiCommon))
                    {
                        datiMaggiorazioniBeneficiCommon = null;
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    }
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiCommon);
                }

                datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Maggiorazioni_AGO));

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBeneficiCommon;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            //--------------------------------------------------------------------
        }

        #endregion Dati Maggiorazioni

        #region Dati Beneficio Vittime Terrorismo

        public static void GetDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, out DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            datiBeneficioVittimeTerrorismo = new DatiBeneficioVittimeTerrorismo();

            Utility.ValorizzaOggetti(contenitore.DatiBeneficioVittimeTerrorismo, datiBeneficioVittimeTerrorismo);
        }

        public static bool ControlDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica,
            DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Utility.TipoCalcolo tipoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string soggettoBeneficiarioTraduzioneSuGP = string.Empty;

            if (!datiBeneficioVittimeTerrorismo.SoggettoBeneficiario.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione))
            {
                messaggioVideo = "Soggetto Beneficiario obbligatorio.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione))
            {
                messaggioVideo = "Data Evento Terroristico obbligatoria.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.CodiceEvento.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione))
            {
                messaggioVideo = "Codice Evento obbligatorio.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.TipologiaPrestazione.HasValue)
            {
                messaggioVideo = "Tipologia della Prestazione obbligatoria.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.TipologiaBeneficio.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(contenitore.DatiPensione))
            {
                messaggioVideo = "Tipologia del Beneficio obbligatoria.";
                return false;
            }

            if (contenitoreDecodifica.ElencoSoggettoBeneficiario != null && contenitoreDecodifica.ElencoSoggettoBeneficiario.Count > 0)
            {
                GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiario = contenitoreDecodifica.ElencoSoggettoBeneficiario.Find(x => x.Id == datiBeneficioVittimeTerrorismo.SoggettoBeneficiario);
                if (soggettoBeneficiario != null)
                    soggettoBeneficiarioTraduzioneSuGP = soggettoBeneficiario.TraduzioneSuGP;
            }

            if (!GestioneControlli.ControlsDecorrenzaEventoTerroristico(datiBeneficioVittimeTerrorismo.DataEventoTerroristico, contenitore.DatiPensione.DataPresentazioneDomanda,
                datiBeneficioVittimeTerrorismo.CodiceEvento, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCoerenzaBeneficioVittimeTerrorismo(datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio,
                datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, soggettoBeneficiarioTraduzioneSuGP, out messaggioVideo))
                return false;

            if (!Utility.IsRicostituzione(contenitore.DatiPensione.Gruppo))
            {
                if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoWithVisibility(contenitore.DatiPensione, contenitore.ListaDatiContributivi, contenitore.ListaDatiCalcoloVittimeTerrorismo,
                    contenitore.DatiBeneficioVittimeTerrorismo, tipoCalcolo, datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null,
                    datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaPrestazione : null,
                    datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null, out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static void StoreDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, Utility.TipoCalcolo tipoCalcolo)
        {
            if (datiBeneficioVittimeTerrorismo == null)
                datiBeneficioVittimeTerrorismo = new DatiBeneficioVittimeTerrorismo();

            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL = contenitore.DatiBeneficioVittimeTerrorismo;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;
            //----------------------------------------------------------------

            long? soggettoBeneficiarioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.SoggettoBeneficiario : null;
            long? tipologiaPrestazioneOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaPrestazione : null;
            long? tipologiaBeneficioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaBeneficio : null;

            // Verifico se è cambiata la condizione di visibilità di almeno una griglia
            bool isDatiCalcoloVittimeRosso =
                Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiarioOld, tipologiaPrestazioneOld, tipologiaBeneficioOld) != Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio);

            bool isDatiCalcoloVittimeNonVisibile = !Utility.IsDatiRetributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismoBL, tipoCalcolo) &&
                                                           !Utility.IsDatiContributiviVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismoBL, tipoCalcolo, contenitore.ListaDatiContributivi != null && contenitore.ListaDatiContributivi.Exists(x => x.IsQuotaDL214Presente())) &&
                                                           !Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiBeneficioVittimeTerrorismoBL == null)
                    datiBeneficioVittimeTerrorismoBL = new GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo();
                Utility.ValorizzaOggetti(datiBeneficioVittimeTerrorismo, datiBeneficioVittimeTerrorismoBL);
                GestioneBeneficioVittimeTerrorismo.SalvaBeneficioVittimeTerrorismo(datiPensione.Id, datiBeneficioVittimeTerrorismoBL);

                if (datiBeneficioVittimeTerrorismo.IsDatiBeneficioVittimeTerrorismoNull())
                    datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo = 0;
                else
                    datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo = 2;

                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue && datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                #region Gestione Semafori Dati Calcolo
                if (isDatiCalcoloVittimeRosso && !isDatiCalcoloVittimeNonVisibile)
                {
                    datiQuadroDatiContributivi.TabVittime = 0;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                }
                else if (isDatiCalcoloVittimeNonVisibile)
                {
                    datiQuadroDatiContributivi.TabVittime = null;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                }
                #endregion Gestione Semafori Dati Calcolo

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismoBL;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
            //--------------------------------------------------------------------
        }

        public static void EliminaDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            //----------------------------------------------------------------

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneBeneficioVittimeTerrorismo.EliminaBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id);

                datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }

            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiBeneficioVittimeTerrorismo = null;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            //--------------------------------------------------------------------
        }

        #endregion Dati Beneficio Vittime Terrorismo

        #region Dati Storico
        public static void GetDatiMaggiorazioneBeneficiStorico(ref EntityBLCommon.ContenitoreObject contenitore, out DatiMaggiorazioneBeneficiStorico datiMaggiorazioneBeneficiStorico)
        {
            datiMaggiorazioneBeneficiStorico = null;

            if (contenitore.DatiStoricoGP != null)
            {
                datiMaggiorazioneBeneficiStorico = new DatiMaggiorazioneBeneficiStorico();
                Utility.ValorizzaOggetti(contenitore.DatiStoricoGP, datiMaggiorazioneBeneficiStorico);
            }
        }
        #endregion Dati Storico

        #region Decodifica

        public static void GetListaTipoBenefici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<TipoBenefici> listaTipoBenefici)
        {
            listaTipoBenefici = new List<TipoBenefici>();
            List<GestioneDecodifica.SettimaneBeneficio> listaTipoBeneficiDB = contenitoreDecodifica.ElencoTipoSettimaneBeneficioAGO_CI;

            if (listaTipoBeneficiDB != null)
            {
                foreach (GestioneDecodifica.SettimaneBeneficio beneficioDB in listaTipoBeneficiDB)
                {
                    if (!Utility.IsDomandaRipristino(contenitore.DatiPensione).GetValueOrDefault())
                    {
                        // Il beneficio 11 è inseribile solo per le domande di APE Precoci
                        if (!Utility.IsDomandaAPEPrecoci(contenitore.DatiPensione) && beneficioDB.Id == "11")
                            continue;

                        // Il beneficio 13 è inseribile solo per le domande di Inabilità amianto
                        if (!Utility.IsDomandaInabilitaAmianto(contenitore.DatiPensione) && beneficioDB.Id == "13")
                            continue;

                        // Il beneficio 14 è inseribile solo per le domande di Quota 100
                        if (!Utility.IsDomandaQuota100(contenitore.DatiPensione) && beneficioDB.Id == "14")
                            continue;

                        // Il beneficio 18 è inseribile solo per le domande di Quota 102
                        if (!Utility.IsDomandaQuota102(contenitore.DatiPensione) && beneficioDB.Id == "18")
                            continue;

                        if (contenitore.DatiPensione.SceltaLavMadri.GetValueOrDefault() != 1 && !(Utility.IsDomandaVOAUT(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.IdTipoPLPerRIC.HasValue && contenitore.DatiPensione.IdTipoPLPerRIC.Value == 21 &&
                            contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.HasValue && !Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value, new DateTime(2024, 04, 01))) && beneficioDB.Id == "12")
                            continue;

                        if (contenitore.DatiPensione.SceltaLavMadri.GetValueOrDefault() != 2 && !(Utility.IsDomandaVOAUT(contenitore.DatiPensione.SiglaCategoria) && contenitore.DatiPensione.IdTipoPLPerRIC.HasValue && contenitore.DatiPensione.IdTipoPLPerRIC.Value == 21 &&
                            contenitore.DatiPensioniDatiGenerici != null && contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.HasValue && !Utility.DataStrettamenteSuccessivaA(contenitore.DatiPensioniDatiGenerici.DataAssunzioneCarico.Value, new DateTime(2024, 04, 01))) && beneficioDB.Id == "15")
                            continue;

                        if (Utility.IsDomandaSPED(contenitore.DatiPensione) && beneficioDB.Id != "01" && beneficioDB.Id != "08" && beneficioDB.Id != "06")
                            continue;

                        //I Benefici 16 e 17 sono inseribili sono nel caso di domande di vecchiaia vecchiaia ENAV (VO-VR-VOART-VOCOM)
                        if (
                            (!Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione) && (beneficioDB.Id == "16" || beneficioDB.Id == "17"))
                            && !Utility.IsRiaperturaRicTRF_Benefici16_17(contenitore.DatiPensione, beneficioDB.Id)) //ctrlAbilitazioneRIC_TRFMemo16_2020
                            continue;
                        //Le domande di vecchiaia ENAV (VO-VR-VOART-VOCOM) posso avere solo i benefici 16 e 17
                        if (Utility.IsDomandaVecchiaiaENAV(contenitore.DatiPensione) && beneficioDB.Id != "16" && beneficioDB.Id != "17")
                            continue;

                        //Per i bancari non è ammesso il codice beneficio 02 
                        if (Utility.IsDomandaBancari(contenitore.DatiPensione.SiglaCategoria) && beneficioDB.Id == "02")
                            continue;

                        // Il beneficio 19 è inseribile solo per le domande Anticipate Flessibili
                        if (!Utility.IsDomandaAnticipataFlessibile(contenitore.DatiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(contenitore.DatiPensione) && beneficioDB.Id == "19")
                            continue;

                        // Il beneficio 24 è inseribile solo per le domande Anticipata Flessibile Legge Bilancio 2024
                        if (!Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(contenitore.DatiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(contenitore.DatiPensione) &&
                            !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(contenitore.DatiPensione) && beneficioDB.Id == "24")
                            continue;
                    }

                    //ENG - Memo 123/2024 aggiornato al 27/03/2025
                    string descrizione = beneficioDB.Descrizione;
                    if (beneficioDB.Id == "24")
                    {
                        GestioneControlliDinamici.ControlloDinamico controlloDinamicoAnnoControlloMemo123_2024 = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AnnoControlloMemo123_2024", out controlloDinamicoAnnoControlloMemo123_2024);
                        if (controlloDinamicoAnnoControlloMemo123_2024 != null && !String.IsNullOrEmpty(controlloDinamicoAnnoControlloMemo123_2024.ValoreControllo) &&
                            !String.IsNullOrEmpty(controlloDinamicoAnnoControlloMemo123_2024.ValoreControllo.Trim()) && controlloDinamicoAnnoControlloMemo123_2024.ValoreControllo == "2025")
                            descrizione = "Pensione anticipata flessibile L.213/2023 e L. 207/2024";
                    }

                    TipoBenefici beneficio = new TipoBenefici();
                    beneficio.Id = beneficioDB.Id;
                    beneficio.Descrizione = descrizione;
                    listaTipoBenefici.Add(beneficio);
                }
            }
        }

        public static void GetListaCodiceMaggiorazioneExCombattente(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente)
        {
            listaCodiceMaggiorazioneExCombattente = new List<Entity.CodiceMaggiorazioneExCombattente>();
            List<Liquidazione.BLCommon.GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiceMaggiorazioneExCombattenteDB = contenitoreDecodifica.ElencoCodiceMaggiorazioneExCombattenti;
            if (listaCodiceMaggiorazioneExCombattenteDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.CodiceMaggiorazioneExCombattenti codiceMaggiorazioneExCombattentiDB in listaCodiceMaggiorazioneExCombattenteDB)
                {
                    Entity.CodiceMaggiorazioneExCombattente codiceMaggiorazioneExCombattente = new Entity.CodiceMaggiorazioneExCombattente();
                    codiceMaggiorazioneExCombattente.Id = codiceMaggiorazioneExCombattentiDB.Id;
                    codiceMaggiorazioneExCombattente.Descrizione = codiceMaggiorazioneExCombattentiDB.Descrizione;
                    codiceMaggiorazioneExCombattente.TraduzioneSuGP = codiceMaggiorazioneExCombattentiDB.TraduzioneSuGP;
                    listaCodiceMaggiorazioneExCombattente.Add(codiceMaggiorazioneExCombattente);
                }
            }
        }

        public static void GetListaCodiceCieco(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.CodiceCieco> listaCodicCieco)
        {
            listaCodicCieco = new List<Entity.CodiceCieco>();
            List<Liquidazione.BLCommon.GestioneDecodifica.Cieco> listaCodiceCiecoDB = contenitoreDecodifica.ElencoCodiceCieco;
            if (listaCodiceCiecoDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.Cieco ciecoDB in listaCodiceCiecoDB)
                {
                    Entity.CodiceCieco cieco = new Entity.CodiceCieco();
                    cieco.Id = ciecoDB.Id;
                    cieco.Descrizione = ciecoDB.Descrizione;
                    listaCodicCieco.Add(cieco);
                }
            }
        }

        public static void GetListaGruppoOneri(out List<Entity.CodiciOneri.GruppoOneri> listaGruppoOneri)
        {
            listaGruppoOneri = new List<Entity.CodiciOneri.GruppoOneri>();
            List<Liquidazione.BLCommon.GestioneDecodifica.GruppoOneri> listaGruppoOneriDB = null;
            GestioneDecodifica.GetGruppoOneri(out listaGruppoOneriDB);
            if (listaGruppoOneriDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.GruppoOneri gruppoOneriDB in listaGruppoOneriDB)
                {
                    Entity.CodiciOneri.GruppoOneri gruppoOneri = new Entity.CodiciOneri.GruppoOneri();
                    gruppoOneri.Id = gruppoOneriDB.Id;
                    gruppoOneri.Descrizione = gruppoOneriDB.Descrizione;
                    gruppoOneri.Code = gruppoOneriDB.Code;
                    listaGruppoOneri.Add(gruppoOneri);
                }
            }
        }

        public static void GetListaSottoGruppoOneri(out List<Entity.CodiciOneri.SottoGruppoOneri> listaSottoGruppoOneri)
        {
            listaSottoGruppoOneri = new List<Entity.CodiciOneri.SottoGruppoOneri>();
            List<Liquidazione.BLCommon.GestioneDecodifica.SottoGruppoOneri> listaSottoGruppoOneriDB = null;
            GestioneDecodifica.GetSottoGruppoOneri(out listaSottoGruppoOneriDB);
            if (listaSottoGruppoOneriDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.SottoGruppoOneri sottoGruppoOneriDB in listaSottoGruppoOneriDB)
                {
                    Entity.CodiciOneri.SottoGruppoOneri sottoGruppoOneri = new Entity.CodiciOneri.SottoGruppoOneri();
                    sottoGruppoOneri.Id = sottoGruppoOneriDB.Id;
                    sottoGruppoOneri.Descrizione = sottoGruppoOneriDB.Descrizione;
                    sottoGruppoOneri.Code = sottoGruppoOneriDB.Code;
                    sottoGruppoOneri.IdOnere = sottoGruppoOneriDB.IdOnere;
                    listaSottoGruppoOneri.Add(sottoGruppoOneri);
                }
            }
        }

        public static void GetListaSoggettoBeneficiario(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<SoggettoBeneficiario> listaSoggettoBeneficiario)
        {
            listaSoggettoBeneficiario = new List<SoggettoBeneficiario>();
            List<GestioneDecodifica.SoggettoBeneficiario> listaSoggettoBeneficiarioDB = contenitoreDecodifica.ElencoSoggettoBeneficiario;
            if (listaSoggettoBeneficiarioDB != null && listaSoggettoBeneficiarioDB.Count > 0)
            {
                foreach (GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiarioDB in listaSoggettoBeneficiarioDB)
                {
                    if (Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, contenitore.DatiBeneficioVittimeTerrorismo) && soggettoBeneficiarioDB.TraduzioneSuGP == "V3 ")
                        continue;

                    SoggettoBeneficiario soggettoBeneficiario = new SoggettoBeneficiario();
                    Utility.ValorizzaOggetti(soggettoBeneficiarioDB, soggettoBeneficiario);
                    listaSoggettoBeneficiario.Add(soggettoBeneficiario);
                }
            }
        }

        public static void GetListaTipologiaPrestazione(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.TipologiaPrestazione> listaTipologiaPrestazione)
        {
            listaTipologiaPrestazione = new List<TipologiaPrestazione>();
            List<GestioneDecodifica.TipologiaPrestazione> listaTipologiaPrestazioneDB = contenitoreDecodifica.ElencoTipologiaPrestazione;
            if (listaTipologiaPrestazioneDB != null && listaTipologiaPrestazioneDB.Count > 0)
            {
                foreach (GestioneDecodifica.TipologiaPrestazione tipologiaPrestazioneDB in listaTipologiaPrestazioneDB)
                {
                    Entity.TipologiaPrestazione tipologiaPrestazione = new TipologiaPrestazione();
                    Utility.ValorizzaOggetti(tipologiaPrestazioneDB, tipologiaPrestazione);
                    listaTipologiaPrestazione.Add(tipologiaPrestazione);
                }
            }
        }

        public static void GetListaTipologiaBeneficioTerrorismo(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo)
        {
            listaTipologiaBeneficioTerrorismo = new List<TipologiaBeneficioTerrorismo>();
            List<GestioneDecodifica.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismoDB = contenitoreDecodifica.ElencoTipologiaBeneficioTerrorismo;
            if (listaTipologiaBeneficioTerrorismoDB != null && listaTipologiaBeneficioTerrorismoDB.Count > 0)
            {
                foreach (GestioneDecodifica.TipologiaBeneficioTerrorismo tipologiaBeneficioTerrorismoDB in listaTipologiaBeneficioTerrorismoDB)
                {
                    Entity.TipologiaBeneficioTerrorismo tipologiaBeneficioTerrorismo = new TipologiaBeneficioTerrorismo();
                    Utility.ValorizzaOggetti(tipologiaBeneficioTerrorismoDB, tipologiaBeneficioTerrorismo);
                    listaTipologiaBeneficioTerrorismo.Add(tipologiaBeneficioTerrorismo);
                }
            }
        }

        #endregion Decodifica

        #region Cross Properties
        public static Dictionary<string, bool?> GetCrossProperties(ref EntityBLCommon.ContenitoreObject contenitore, DatiBenefici datiBenefici, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            bool isRiaperturaDomanda, out int? settimane)
        {
            bool? IsBeneficioBloccato = null;
            bool? isBeneficioExArt80 = null;
            bool? isDomandaInabilitaIndiretta = null;
            bool? isVisiblePerSuperstitiOrPMO = null;
            bool? isBeneficioAmianto181 = null;
            bool? isNumSettimaneBeneficioEnabled = null;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isPrepensionamentoEditoria = null;
            bool? isPrepensionamentoEditoriaArt1c154L205_2017 = null;
            bool? isPrepensionamentoEditoriaArt1c500L160_2019 = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isDomandaPensioneInabilita = null;
            bool? isBeneficioVittimeTerrorismo = null;
            bool? isBeneficioInabilitaByPrimoCodiceNatura = null;
            bool? isBeneficioUsuranti = null;
            bool? isBeneficioMaggiorazioneAmiantoLegge208_2015 = null;
            bool? isBeneficioNonVedenteByPrimoCodiceNatura = null;
            bool? isBeneficioMinatori = null;
            settimane = null;
            bool? isPrepensionamentoEditoriaFiltroEBA = null;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            IsBeneficioBloccato = Utility.IsTabPrepensionamentoVisible(contenitore.DatiPensione, contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale, null);
            isBeneficioExArt80 = IsBeneficioExArt80(contenitore.DatiPensione);
            isBeneficioMinatori = IsBeneficioMinatori(contenitore.DatiPensione);
            isBeneficioUsuranti = IsBeneficioUsuranti(contenitore.DatiPensione);
            isVisiblePerSuperstitiOrPMO = IsVisiblePerSuperstitiOrPMO(contenitore.DatiPensione);
            isBeneficioAmianto181 = Utility.IsDomandaConBeneficioAmianto181(contenitore.DatiPensione.AttivitaEconomica, contenitore.DatiPensione.ProfessioneIndividuale);
            isNumSettimaneBeneficioEnabled = IsNumSettimaneBeneficioEnabled(contenitore.DatiPensione, isRiaperturaDomanda);
            isBeneficioArt24Comma15BisFromFELPE = contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE : null;
            isPrepensionamentoEditoria = Utility.IsPrepensionamentoEditoriaFiltroEAA(contenitore.DatiPensione);
            isPrepensionamentoEditoriaArt1c154L205_2017 = Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(contenitore.DatiPensione);
            isPrepensionamentoEditoriaArt1c500L160_2019 = Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(contenitore.DatiPensione);
            isBeneficioApePrecociFromFELPE = contenitore.DatiMaggiorazioniBenefici != null ? contenitore.DatiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE : null;
            isDomandaPensioneInabilita = Utility.IsDomandaPensioneInabilitaOrRicostituzioneAGO_CI(contenitore.DatiPensione);
            if (datiBeneficioVittimeTerrorismo != null)
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione) ||
                                               Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione);
            else
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(contenitore.DatiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, null);
            char codNat1;
            char codNat2;
            char codNat3;
            Utility.GetCodiciNatura(contenitore.DatiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);
            isDomandaInabilitaIndiretta = GestioneControlli.IsDomandaInabilitaIndiretta(codNat1, contenitore.DatiPensione.SiglaCategoria);
            isBeneficioInabilitaByPrimoCodiceNatura = codNat1 == '3' || codNat1 == '4';
            //Per le SOTOT indirette, vince il beneficio 8
            if (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && Utility.IsDomandaSOTOT(contenitore.DatiPensione.SiglaCategoria) && isBeneficioExArt80.GetValueOrDefault())
            {
                isBeneficioInabilitaByPrimoCodiceNatura = false;
                isDomandaInabilitaIndiretta = false;
            }
            isBeneficioMaggiorazioneAmiantoLegge208_2015 = Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(contenitore.DatiPensione) || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(contenitore.DatiPensione);
            //l'assegnazione inline come i precedenti non funzionava
            if ((Utility.IsDomandaSOCUM(contenitore.DatiPensione.SiglaCategoria) && codNat1 == ' ') ||
                (Utility.IsDomandaPensioneIndiretta(contenitore.DatiPensione) && Utility.IsDomandaSOTOT(contenitore.DatiPensione.SiglaCategoria) && (codNat1 == ' ' || codNat1 == '1' || codNat1 == '2' || codNat1 == '6')))
                isBeneficioNonVedenteByPrimoCodiceNatura = true;


            if (contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.NSettimaneOBG.HasValue)
                settimane = contenitore.DatiIstruttoria.NSettimaneOBG.Value;
            if (contenitore.DatiIstruttoria != null && contenitore.DatiIstruttoria.NContributiVolontari.HasValue)
                settimane += contenitore.DatiIstruttoria.NContributiVolontari.Value;

            isPrepensionamentoEditoriaFiltroEBA = Utility.IsPrepensionamentoEditoriaFiltroEBA(contenitore.DatiPensione);

            lReturn.Add("IsBeneficioBloccato", IsBeneficioBloccato);
            lReturn.Add("IsBeneficioExArt80", isBeneficioExArt80);
            lReturn.Add("IsDomandaInabilitaIndiretta", isDomandaInabilitaIndiretta);
            lReturn.Add("IsVisiblePerSuperstitiOrPMO", isVisiblePerSuperstitiOrPMO);
            lReturn.Add("IsBeneficioAmianto181", isBeneficioAmianto181);
            lReturn.Add("IsNumSettimaneBeneficioEnabled", isNumSettimaneBeneficioEnabled);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            lReturn.Add("IsPrepensionamentoEditoria", isPrepensionamentoEditoria);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c154L205_2017", isPrepensionamentoEditoriaArt1c154L205_2017);
            lReturn.Add("IsPrepensionamentoEditoriaArt1c500L160_2019", isPrepensionamentoEditoriaArt1c500L160_2019);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsDomandaPensioneInabilita", isDomandaPensioneInabilita);
            lReturn.Add("IsBeneficioVittimeTerrorismo", isBeneficioVittimeTerrorismo);
            lReturn.Add("IsBeneficioInabilitaByPrimoCodiceNatura", isBeneficioInabilitaByPrimoCodiceNatura);
            lReturn.Add("IsBeneficioUsuranti", isBeneficioUsuranti);
            lReturn.Add("IsBeneficioMaggiorazioneAmiantoLegge208_2015", isBeneficioMaggiorazioneAmiantoLegge208_2015);
            lReturn.Add("IsBeneficioNonVedenteByPrimoCodiceNatura", isBeneficioNonVedenteByPrimoCodiceNatura);
            lReturn.Add("IsBeneficioMinatori", isBeneficioMinatori);
            lReturn.Add("IsPrepensionamentoEditoriaFiltroEBA", isPrepensionamentoEditoriaFiltroEBA);
            return lReturn;
        }

        private static bool IsBeneficioExArt80(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length == 3 && datiPensione.NaturaPensione[2] == 'G')
                return true;

            return false;
        }

        private static bool IsBeneficioMinatori(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length == 3 && datiPensione.NaturaPensione[2] == 'D')
                return true;

            return false;
        }

        private static bool IsBeneficioUsuranti(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length == 3 && datiPensione.NaturaPensione[2] == 'Z')
                return true;

            return false;
        }

        private static bool IsVisiblePerSuperstitiOrPMO(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                if (datiPensione.SiglaCategoria.StartsWith("S") ||
                    (datiPensione.SiglaCategoria.Trim().Equals("PMO") && (datiPensione.NCertificato.ToString().Substring(2, 1) == "3" || datiPensione.NCertificato.ToString().Substring(2, 1) == "6")) ||
                    datiPensione.SiglaCategoria.Trim().Equals("PSO"))
                    return true;
            }

            return false;
        }

        private static bool IsNumSettimaneBeneficioEnabled(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (datiPensione == null)
                return true;

            return !datiPensione.Amianto181Unicarpe.GetValueOrDefault() && !(Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                Utility.IsDomandaAPEPrecoci(datiPensione)) && !Utility.IsDomandaQuota100(datiPensione) && !Utility.IsDomandaQuota102(datiPensione) && !Utility.IsDomandaAnticipataFlessibile(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)
                && !Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) &&
                !Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) && !Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) &&
                !Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) && !Utility.IsDomandaESPA(datiPensione.SiglaCategoria)
                && !Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && !Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione);
        }

        #endregion Cross Properties

        #region nested class

        public class DatiMaggiorazioniBenefici
        {
            public DatiMaggiorazioniBenefici()
            { }
            public DatiMaggiorazioniBenefici(long id, long idPensione, byte? codiceCieco, DateTime? decorrenzaMaggiorazioneArt6, DateTime? decorrenzaMaggiorazioneSociale,
                                             string tipoSettimaneBeneficio, long? exCombattente, decimal _RMSSenzaLegge33670QA, decimal _RMSSenzaLegge33670QB,
                                             byte? percentualeMaggiorazioneSenzaLegge33670, DateTime? cessazioneMaggiorazioneSociale, int? nSettimaneBeneficio)
            {
                this._Id = id;
                this._IdPensione = idPensione;
                this._CodiceCieco = codiceCieco;
                this._DecorrenzaMaggiorazioneArt6 = decorrenzaMaggiorazioneArt6;
                this._DecorrenzaMaggiorazioneSociale = decorrenzaMaggiorazioneSociale;
                this._CessazioneMaggiorazioneSociale = cessazioneMaggiorazioneSociale;
                this._TipoSettimaneBeneficio = tipoSettimaneBeneficio;
                this._ExCombattente = exCombattente;
                this._RMSSenzaLegge33670QA = RMSSenzaLegge33670QA;
                this._RMSSenzaLegge33670QB = RMSSenzaLegge33670QB;
                this._PercentualeMaggiorazioneSenzaLegge33670 = percentualeMaggiorazioneSenzaLegge33670;
                this._NSettimaneBeneficio = nSettimaneBeneficio;
            }

            #region private properties

            private long _Id;
            private long _IdPensione;
            private byte? _CodiceCieco;
            private DateTime? _DecorrenzaMaggiorazioneArt6;
            private DateTime? _DecorrenzaMaggiorazioneSociale;
            private DateTime? _CessazioneMaggiorazioneSociale;
            private string _TipoSettimaneBeneficio;
            private long? _ExCombattente;
            private decimal? _RMSSenzaLegge33670QA;
            private decimal? _RMSSenzaLegge33670QB;
            private byte? _PercentualeMaggiorazioneSenzaLegge33670;
            private int? _NSettimaneBeneficio;
            private short? _AnniRiduzioneBeneficiArt38Legge02;
            private int? _NSettimaneIncremento1Percento;
            private int? _NSettimaneIncremento05Percento;
            private byte? _Sentenza495240;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }
            public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
            public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }
            public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }
            public string TipoSettimaneBeneficio { get { return _TipoSettimaneBeneficio; } set { _TipoSettimaneBeneficio = value; } }
            public long? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }
            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
            public decimal? RMSSenzaLegge33670QB { get { return _RMSSenzaLegge33670QB; } set { _RMSSenzaLegge33670QB = value; } }
            public byte? PercentualeMaggiorazioneSenzaLegge33670 { get { return _PercentualeMaggiorazioneSenzaLegge33670; } set { _PercentualeMaggiorazioneSenzaLegge33670 = value; } }
            public int? NSettimaneBeneficio { get { return _NSettimaneBeneficio; } set { _NSettimaneBeneficio = value; } }
            public short? AnniRiduzioneBeneficiArt38Legge02 { get { return _AnniRiduzioneBeneficiArt38Legge02; } set { _AnniRiduzioneBeneficiArt38Legge02 = value; } }
            public int? NSettimaneIncremento1Percento { get { return _NSettimaneIncremento1Percento; } set { _NSettimaneIncremento1Percento = value; } }
            public int? NSettimaneIncremento05Percento { get { return _NSettimaneIncremento05Percento; } set { _NSettimaneIncremento05Percento = value; } }
            public byte? Sentenza495240 { get { return _Sentenza495240; } set { _Sentenza495240 = value; } }

            #endregion public properties

        }

        public static bool IsMaggiorazioniBeneficiNull(DatiMaggiorazioniBenefici maggiorazioniBenefici)
        {
            if (!maggiorazioniBenefici.CodiceCieco.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.TipoSettimaneBeneficio) &&
                !maggiorazioniBenefici.ExCombattente.HasValue && !maggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue &&
                !maggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue && !maggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670.HasValue)
            {
                return true;
            }
            else
                return false;
        }

        #endregion nested class
    }
}
