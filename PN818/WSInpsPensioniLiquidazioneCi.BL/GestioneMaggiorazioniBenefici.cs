using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.LiquidazioneCi.Entity;

namespace INPS.Pensioni.LiquidazioneCi
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

        #endregion MaggiorazioniBenefici

        #region DatiExCombattente

        public static bool ControlDatiExCombattente(GestionePensione.DatiPensione datiPensione, Entity.DatiExCombattente datiExCombattente, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiExCombattente == null || datiExCombattente.IsDatiExCombattenteNull())
            {
                messaggioVideo = "Inserire almeno un dato 'Ex Combattente' prima di procedere con il salvataggio";
                return false;
            }

            if (datiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue && !datiExCombattente.CodiceCieco.HasValue)
            {
                messaggioVideo = "In presenza della 'Decorrenza' della Legge 140 è obbligatorio inserire il 'Codice ex Combattente'";
                return false;
            }

            if (!GestioneControlli.VerificaCodiceCiecoArt6(datiExCombattente.CodiceCieco, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaArt6(datiExCombattente.DecorrenzaMaggiorazioneArt6, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceCiecoWithDecorrenza(datiExCombattente.CodiceCieco, datiExCombattente.DecorrenzaMaggiorazioneArt6, out messaggioVideo))
                return false;

            if (!ControlCrossDatiExCombattente(datiPensione, datiExCombattente, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsDecorrenzaExCombattenteWithDataPresentazione(datiExCombattente.DecorrenzaMaggiorazioneArt6, datiPensione, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlCrossDatiExCombattente(GestionePensione.DatiPensione datiPensione, Entity.DatiExCombattente datiExCombattente, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            #endregion GetData

            if (!GestioneControlli.ControlsDecorrenzaMaggiorazioneArt6(datiExCombattente.DecorrenzaMaggiorazioneArt6, datiExCombattente.CodiceCieco, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDecorrenzaOriginaria(datiExCombattente.DecorrenzaMaggiorazioneArt6, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza art.6 L.140/544 antecedente a Decorrenza Diretta o 01/85";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria(datiExCombattente.DecorrenzaMaggiorazioneArt6, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza art.6 L.140/544 incompatibile con Data Morte/Decorrenza Diretta";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaDecorrenzaArt6WithDecorrenza(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiExCombattente.DecorrenzaMaggiorazioneArt6, datiPensione.DecorrenzaOriginaria))
            {
                messaggioVideo = "Decorrenza art.6/140 anteriore a Decorrenza Originaria";
                return false;
            }

            if (!GestioneCrossControls.CI_VerificaDecorrenzaArt6WithDecorrenza(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiExCombattente.DecorrenzaMaggiorazioneArt6, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null))
            {
                messaggioVideo = "Decorrenza art.6/140 anteriore a Decorrenza Diretta";
                return false;
            }

            return true;
        }

        public static void ValorizzaDatiExCombattente(Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, out Entity.DatiExCombattente datiExCombattente)
        {
            datiExCombattente = null;
            if (datiMaggiorazioniBenefici != null)
            {
                datiExCombattente = new Entity.DatiExCombattente();
                Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiExCombattente);
                if (datiExCombattente.IsDatiExCombattenteNull())
                    datiExCombattente = null;
            }
        }

        public static void StoreDatiExCombattente(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Entity.DatiExCombattente datiExCombattente)
        {
            if (datiExCombattente == null)
                datiExCombattente = new Entity.DatiExCombattente();

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

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
        }

        private static void StoreDatiExCombattentePrivate(long idPensione, Entity.DatiExCombattente datiExCombattente, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiExCombattente, datiMaggiorazioniBenefici);

            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiExCombattente(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

            if (datiMaggiorazioniBenefici != null)
            {
                datiMaggiorazioniBenefici.CodiceCieco = null;
                datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = null;
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBenefici != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBenefici))
                    {
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    }
                    else
                    {
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
                    }
                }
                datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }
        }

        #endregion DatiExCombattente

        #region Dati Benefici

        public static bool ControlDatiBenefici(GestionePensione.DatiPensione datiPensione, DatiBenefici datiBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, bool IsCancelOperation, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiBenefici != null)
            {
                if (!IsCancelOperation && datiBenefici.IsDatiBeneficiNull())
                {
                    messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                return false;
            }

            if (!ControlCrossDatiBenefici(datiPensione, datiBenefici, datiAnagraficaTitolare, datiIstruttoria, datiPensioniDatiGenerici, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlCrossDatiBenefici(GestionePensione.DatiPensione datiPensione, DatiBenefici datiBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa != null ? datiDanteCausa.IdAnagrafica : 0, out datiAnagraficiDC);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivo = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloContributivo);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPensioniCiPrestazioniEE);

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);

            int? settimaneRetributiveQuotaA = null;
            int? settimaneRetributiveQuotaB = null;
            int? settimaneContributive = null;
            int? settimaneContributiveDL214 = null;
            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in listaDatiCalcoloRetributivo)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaA = retr.NSettimaneQuotaA;
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaB = retr.NSettimaneQuotaB;
                        }
                    }
                }
            }
            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo contr in listaDatiCalcoloContributivo)
                {
                    if (contr.CodiceGestione == 1)
                    {
                        if (contr.NSettimane.HasValue)
                            settimaneContributive = contr.NSettimane;
                        if (contr.NSettimaneQuotaDL214.HasValue)
                            settimaneContributiveDL214 = contr.NSettimaneQuotaDL214;
                    }
                }
            }

            #endregion GetData

            if (!GestioneControlli.ControlsNSettimaneIncremento1Percento(datiBenefici.NSettimaneIncremento1Percento, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsNSettimaneIncremento05Percento(datiBenefici.NSettimaneIncremento05Percento, datiPensione.Gruppo, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione,
                datiAnagraficaTitolare.Sesso, out messaggioVideo))
                return false;

            if (datiDanteCausa != null)
            {
                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento1PercentoWithDanteCausa(datiBenefici.NSettimaneIncremento1Percento, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Settimane Incremento 1% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithDecorrenzaDiretta(datiBenefici.NSettimaneIncremento05Percento, datiDanteCausa.SiglaCategoria, datiDanteCausa.DecorrenzaPensione))
                {
                    messaggioVideo = "Settimane Incremento 0.5% incompatibili con Categoria o Decorrenza Diretta";
                    return false;
                }

                if (!GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithSessoDanteCausa(datiBenefici.NSettimaneIncremento05Percento, datiAnagraficiDC.Sesso))
                {
                    messaggioVideo = "Settimane Incremento 0.5% incompatibili con Sesso del Titolare Dante Causa";
                    return false;
                }
            }

            #region Categorie minori o uguali a 6
            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiBenefici.NSettimaneIncremento1Percento, datiBenefici.NSettimaneIncremento05Percento, settimaneRetributiveQuotaB, settimaneContributive, settimaneContributiveDL214, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCapienzaNSettimaneIncrementoPercentuale(datiBenefici.NSettimaneIncremento1Percento, datiBenefici.NSettimaneIncremento05Percento, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;
            }
            #endregion Categorie minori o uguali a 6

            #region Categorie maggiori o uguali a 7
            if (categoria >= 7)
            {
                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiBenefici.NSettimaneIncremento1Percento, datiBenefici.NSettimaneIncremento05Percento, settimaneRetributiveQuotaB, null, null, out messaggioVideo))
                    return false;
            }
            #endregion Categoria maggiori o uguali a 7

            if (!GestioneCrossControls.CI_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio, datiPensione.DecorrenzaOriginaria,
                datiPensione.NaturaPensione, datiAnagraficaTitolare.Sesso, datiAnagraficaTitolare.DataNascita, datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsBeneficioPrecoci(datiPensione, datiBenefici.TipoSettimaneBeneficio, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio, datiBenefici.SettAnzContribPost311295, datiPensione, datiDanteCausa, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsBeneficioMaggiorazioneAmiantoLegge208_2015(datiPensione, datiBenefici.NSettimaneBeneficio, datiBenefici.SettAnzContribPost311295, out messaggioVideo))
                return false;

            return true;
        }

        public static void GetDatiBeneficiByIdPensione(long idPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            out Entity.DatiBenefici datiBenefici)
        {
            datiBenefici = null;

            if (datiMaggiorazioniBenefici != null)
            {
                datiBenefici = new Entity.DatiBenefici();
                Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiBenefici);
                if (datiBenefici.IsDatiBeneficiNull())
                    datiBenefici = null;
            }

            List<GestioneRipartizioneFondi.DatiRipartizioneFondi> LdatiRipartizioneFondi = null;
            GestioneRipartizioneFondi.GetRipartizioneFondiByIdPensione(idPensione, out LdatiRipartizioneFondi);
            if (LdatiRipartizioneFondi != null && LdatiRipartizioneFondi.Count > 0)
            {
                if (datiBenefici == null)
                    datiBenefici = new Entity.DatiBenefici();
                datiBenefici.ListOneriTerrorismo = new List<INPS.Pensioni.LiquidazioneCi.Entity.DatiBenefici.OneriTerrorismo>();
                foreach (GestioneRipartizioneFondi.DatiRipartizioneFondi rf in LdatiRipartizioneFondi)
                {
                    Entity.DatiBenefici.OneriTerrorismo ot = new Entity.DatiBenefici.OneriTerrorismo();
                    Liquidazione.BLCommon.Utility.ValorizzaOggetti(rf, ot);
                    datiBenefici.ListOneriTerrorismo.Add(ot);
                }
            }
        }

        public static void StoreDatiBenefici(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            Entity.DatiBenefici datiBenefici)
        {
            if (datiBenefici != null)
            {
                GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
                GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);
                GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
                GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);
                List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariBlCommon = null;

                if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && datiBenefici.TipoSettimaneBeneficio == "01")
                {
                    GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                    datiBeneficiParticolari.CodiceBenefici = datiBenefici.TipoSettimaneBeneficio;
                    datiBeneficiParticolari.Settimane = datiBenefici.SettAnzContribPost311295;
                    lBeneficiParticolariBlCommon = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();
                    lBeneficiParticolariBlCommon.Add(datiBeneficiParticolari);
                }

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    StoreDatiBeneficiPerMaggiorazioniBenefici(datiPensione.Id, datiBenefici, ref datiMaggiorazioniBenefici);

                    #region RipartizioneFondi

                    if (datiBenefici.ListOneriTerrorismo != null && datiBenefici.ListOneriTerrorismo.Count > 0)
                    {
                        GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);
                        foreach (Entity.DatiBenefici.OneriTerrorismo ot in datiBenefici.ListOneriTerrorismo)
                        {
                            ot.IdPensione = datiPensione.Id;
                            GestioneRipartizioneFondi.DatiRipartizioneFondi datiRipartizioneFondi = new GestioneRipartizioneFondi.DatiRipartizioneFondi();
                            Utility.ValorizzaOggetti(ot, datiRipartizioneFondi);
                            GestioneRipartizioneFondi.SalvaRipartizioneFondi(datiRipartizioneFondi);
                        }
                    }

                    #endregion RipartizioneFondi

                    #region Benefici Particolari
                    if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                    {
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                            StoreDatiBeneficiForBeneficiParticolari(lBeneficiParticolariBlCommon, datiPensione);

                        datiQuadroOneri.Tipo = 2;
                        if (datiQuadroOneri.TabOneri == null)
                            datiQuadroOneri.TabOneri = 0;
                        GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                    }
                    #endregion Benefici Particolari

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
            }
        }

        private static void StoreDatiBeneficiPerMaggiorazioniBenefici(long idPensione, Entity.DatiBenefici datiBenefici, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiBenefici, datiMaggiorazioniBenefici);
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiBenefici(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, List<GestioneOneri.DatiOneri> listaDatiOneri)
        {
            bool isClearQuadroOneri = true;
            if (datiMaggiorazioniBeneficiCommon != null)
            {
                if (!datiMaggiorazioniBeneficiCommon.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() && !Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) &&
                    !Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) &&
                    !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01"))
                    datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio = null;

                if (!Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) && !Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) &&
                    !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01"))
                {
                    datiMaggiorazioniBeneficiCommon.NSettimaneBeneficio = null;
                }

                datiMaggiorazioniBeneficiCommon.NSettimaneIncremento1Percento = null;
                datiMaggiorazioniBeneficiCommon.NSettimaneIncremento05Percento = null;
                datiMaggiorazioniBeneficiCommon.Sentenza495240 = null;

                if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01") &&
                    (listaDatiOneri == null || listaDatiOneri.Count == 0))
                    isClearQuadroOneri = true;
            }

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
            GestioneQuadri.GetQuadroOneriByDatiPensione(datiPensione, out datiQuadroOneri);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBeneficiCommon != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiCommon))
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiCommon);
                }
                GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);

                if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBeneficiCommon != null &&
                    datiMaggiorazioniBeneficiCommon.TipoSettimaneBeneficio == "01"))
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);

                if (isClearQuadroOneri)
                {
                    datiQuadroOneri.Tipo = 0;
                    datiQuadroOneri.TabOneri = null;
                    GestioneQuadri.SalvaQuadroOneri(datiPensione.Id, datiQuadroOneri);
                }

                datiQuadroMaggiorazioniBenefici.TabBenefici = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }
        }

        #endregion Dati Benefici

        #region Dati Benefici Particolari

        private static void StoreDatiBeneficiForBeneficiParticolari(List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariDB, GestionePensione.DatiPensione datiPensione)
        {
            if (lBeneficiParticolariDB != null && lBeneficiParticolariDB.Count > 0)
            {
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

        #region Dati Maggiorazioni

        public static void ValorizzaDatiMaggiorazioni(Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, out Entity.DatiMaggiorazioni datiMaggiorazioni)
        {
            datiMaggiorazioni = null;
            if (datiMaggiorazioniBenefici != null)
            {
                datiMaggiorazioni = new Entity.DatiMaggiorazioni();
                Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiMaggiorazioni);
                if (datiMaggiorazioni.IsDatiMaggiorazioniNull())
                    datiMaggiorazioni = null;
            }
        }

        public static bool ControlDatiMaggiorazioni(GestionePensione.DatiPensione datiPensione, Entity.DatiMaggiorazioni datiMaggiorazioni, bool IsCancelOperation, DateTime? dataNascitaTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (datiMaggiorazioni != null)
            {
                GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);

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

                if (datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.Value.Year, datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.Value.Month, 1),
                    new DateTime(dataSistema.AddMonths(1).Year, dataSistema.AddMonths(1).Month, 1)))
                {
                    messaggioVideo = "La decorrenza maggiorazione sociale non può essere superiore di 1 mese dalla data odierna tenendo conto solo del mese ed anno";
                    return false;
                }

                if (!ControlCrossDatiMaggiorazioni(datiPensione, datiMaggiorazioni, out messaggioVideo))
                    return false;

                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
                if (!GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, datiPensione, dataNascitaTitolare,
                    datiStoricoGP != null ? datiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, datiDanteCausa, isRiaperturaDomanda, out messaggioVideo))
                    return false;
            }
            return true;
        }

        public static bool ControlCrossDatiMaggiorazioni(GestionePensione.DatiPensione datiPensione, Entity.DatiMaggiorazioni datiMaggiorazioni, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;
            #region GetData

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            List<GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392 = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392);

            INPS.Pensioni.Liquidazione.BLCommon.Entity.AreaTitolare areaTitolare = null;
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            char? codiceRequisitiLegge50392TraduzioneSuGP = null;
            if (datiMaggiorazioni.CodiceRequisitiLegge50392Art2.HasValue && listaCodiceRequisitiLegge50392 != null && listaCodiceRequisitiLegge50392.Count > 0)
            {
                GestioneDecodifica.CodiceRequisitiLegge50392 appCodiceRequisitiLegge50392 = listaCodiceRequisitiLegge50392.Find(x => x.Id == datiMaggiorazioni.CodiceRequisitiLegge50392Art2.ToString());
                codiceRequisitiLegge50392TraduzioneSuGP = appCodiceRequisitiLegge50392 != null ? appCodiceRequisitiLegge50392.TraduzioneSuGP : null;
            }

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);
            #endregion GetData

            if (datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.HasValue)
            {
                if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneSocialeWithDecorrenzaOriginaria(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, datiPensione.DecorrenzaOriginaria))
                {
                    messaggioVideo = "Decorrenza L.544/1 anteriore a Decorrenza Originaria o 07/1988";
                    return false;
                }

                if (datiMaggiorazioni.CessazioneMaggiorazioneSociale.HasValue)
                {
                    if (Utility.DataStrettamenteSuccessivaA(datiMaggiorazioni.CessazioneMaggiorazioneSociale.Value, dataSistema.AddMonths(1)))
                    {
                        messaggioVideo = "Cessazione L.544/1 illogica o posteriore data odierna";
                        return false;
                    }

                    if (!Utility.DataSuccessivaA(datiMaggiorazioni.CessazioneMaggiorazioneSociale.Value, datiMaggiorazioni.DecorrenzaMaggiorazioneSociale.Value))
                    {
                        messaggioVideo = "Cessazione L.544/1 anteriore a Decorrenza L.544/1";
                        return false;
                    }
                }

                if (!GestioneControlli.ControlsMaggiorazioniWithEtaPensionabile(datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, datiAnagrafici.DataNascita, datiPensione.Gruppo, datiPensione.CausaCarico, datiPensione, out messaggioVideo))
                    return false;
            }
            else
            {
                if (datiMaggiorazioni.CessazioneMaggiorazioneSociale.HasValue)
                {
                    messaggioVideo = "Cessazione L.544/1 illogica (Decorrenza mancante)";
                    return false;
                }
            }

            if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneLegge140(datiMaggiorazioni.DecorrenzaMaggiorazioneLegge140, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaDecorrenzaMaggiorazioneLegg140WithEtaPensionabile(datiMaggiorazioni.DecorrenzaMaggiorazioneLegge140, tipoDomanda, datiPensione.CausaCarico, datiAnagrafici.DataNascita, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaAnniRiduzioneBeneficiArt38Legge02(datiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02, tipoDomanda, datiMaggiorazioni.DecorrenzaMaggiorazioneSociale, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiLegge50392(codiceRequisitiLegge50392TraduzioneSuGP, datiPensione.DecorrenzaOriginaria, tipoDomanda, categoria, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCodiceRequisitiLegge50392WithInvalidita(codiceRequisitiLegge50392TraduzioneSuGP, datiPensione.Gruppo, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            if (!GestioneCrossControls.CI_VerificaCodiceRequisitiLegge50392WithStatoCivile(codiceRequisitiLegge50392TraduzioneSuGP, areaTitolare.ElencoStatiCivili, out messaggioVideo))
                return false;

            return true;
        }

        public static void StoreDatiMaggiorazioni(GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici,
            Entity.DatiMaggiorazioni datiMaggiorazioni)
        {
            if (datiMaggiorazioni != null)
            {
                GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
                GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

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
            }
        }

        private static void StoreDatiMaggiorazioniPerMaggiorazioniBenefici(long idPensione, Entity.DatiMaggiorazioni datiMaggiorazioni, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = idPensione;
            }

            Utility.ValorizzaOggetti(datiMaggiorazioni, datiMaggiorazioniBenefici);
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiMaggiorazioni(GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon)
        {
            if (datiMaggiorazioniBeneficiCommon != null)
            {
                datiMaggiorazioniBeneficiCommon.CessazioneMaggiorazioneSociale = null;
                datiMaggiorazioniBeneficiCommon.DecorrenzaMaggiorazioneSociale = null;
                datiMaggiorazioniBeneficiCommon.DecorrenzaMaggiorazioneLegge140 = null;
                datiMaggiorazioniBeneficiCommon.AnniRiduzioneBeneficiArt38Legge02 = null;
                datiMaggiorazioniBeneficiCommon.CodiceRequisitiLegge50392Art2 = null;
            }

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBeneficiCommon != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBeneficiCommon))
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiCommon);
                }

                datiQuadroMaggiorazioniBenefici.TabMaggiorazioni = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }
        }

        #endregion Dati Maggiorazioni

        #region Dati Beneficio Vittime Terrorismo

        public static void GetDatiBeneficioVittimeTerrorismo(long? idPensione, out Entity.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            datiBeneficioVittimeTerrorismo = new DatiBeneficioVittimeTerrorismo();

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(idPensione.GetValueOrDefault(), out datiBeneficioVittimeTerrorismoBL);
            Utility.ValorizzaOggetti(datiBeneficioVittimeTerrorismoBL, datiBeneficioVittimeTerrorismo);
        }

        public static bool ControlDatiBeneficioVittimeTerrorismo(GestionePensione.DatiPensione datiPensione, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiCalcoloContributivo,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string soggettoBeneficiarioTraduzioneSuGP = string.Empty;

            List<GestioneDecodifica.SoggettoBeneficiario> decodificaSoggettoBeneficiario = null;
            GestioneDecodifica.GetDecodificaSoggettoBeneficiario(out decodificaSoggettoBeneficiario);

            if (!datiBeneficioVittimeTerrorismo.SoggettoBeneficiario.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                messaggioVideo = "Soggetto Beneficiario obbligatorio.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                messaggioVideo = "Data Evento Terroristico obbligatoria.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.CodiceEvento.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                messaggioVideo = "Codice Evento obbligatorio.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.TipologiaPrestazione.HasValue)
            {
                messaggioVideo = "Tipologia della Prestazione obbligatoria.";
                return false;
            }

            if (!datiBeneficioVittimeTerrorismo.TipologiaBeneficio.HasValue && !Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                messaggioVideo = "Tipologia del Beneficio obbligatoria.";
                return false;
            }

            if (decodificaSoggettoBeneficiario != null && decodificaSoggettoBeneficiario.Count > 0)
            {
                GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiario = decodificaSoggettoBeneficiario.Find(x => x.Id == datiBeneficioVittimeTerrorismo.SoggettoBeneficiario);
                if (soggettoBeneficiario != null)
                    soggettoBeneficiarioTraduzioneSuGP = soggettoBeneficiario.TraduzioneSuGP;
            }

            if (!GestioneControlli.ControlsDecorrenzaEventoTerroristico(datiBeneficioVittimeTerrorismo.DataEventoTerroristico, datiPensione.DataPresentazioneDomanda, datiBeneficioVittimeTerrorismo.CodiceEvento, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsCoerenzaBeneficioVittimeTerrorismo(datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio,
                datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, soggettoBeneficiarioTraduzioneSuGP, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsDatiCalcoloVittimeTerrorismoWithVisibility(datiPensione, lDatiCalcoloContributivo, listaDatiCalcoloVittimeTerrorismo,
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.SoggettoBeneficiario : null,
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaPrestazione : null,
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null, datiBeneficioVittimeTerrorismoBL, out messaggioVideo))
                return false;

            return true;
        }

        public static void StoreDatiBeneficioVittimeTerrorismo(GestionePensione.DatiPensione datiPensione, Entity.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo)
        {
            if (datiBeneficioVittimeTerrorismo == null)
                datiBeneficioVittimeTerrorismo = new Entity.DatiBeneficioVittimeTerrorismo();

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL = null;
            GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismoBL);

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            long? soggettoBeneficiarioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.SoggettoBeneficiario : null;
            long? tipologiaPrestazioneOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaPrestazione : null;
            long? tipologiaBeneficioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaBeneficio : null;

            // Verifico se è cambiata la condizione di visibilità di almeno una griglia
            bool isDatiCalcoloVittimeRosso =
                Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiarioOld, tipologiaPrestazioneOld, tipologiaBeneficioOld) != Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio);

            bool isDatiCalcoloVittimeNonVisibile = !(Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismoBL) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismoBL));

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
        }

        public static void EliminaDatiBeneficioVittimeTerrorismo(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
            GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(datiPensione, out datiQuadroMaggiorazioniBenefici);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneBeneficioVittimeTerrorismo.EliminaBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id);

                datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }
        }



        #endregion Dati Beneficio Vittime Terrorismo

        #region Decodifica

        public static void GetListaTipoBenefici(GestionePensione.DatiPensione datiPensione, out List<Entity.TipoBenefici> listaTipoBenefici)
        {
            listaTipoBenefici = new List<Entity.TipoBenefici>();
            List<Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio> listaTipoBeneficiDB = null;
            GestioneDecodifica.GetTipoSettimaneBeneficioAGO_CI(out listaTipoBeneficiDB);

            if (listaTipoBeneficiDB != null)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio beneficioDB in listaTipoBeneficiDB)
                {
                    // Il beneficio 11 è inseribile solo per le domande di APE Precoci
                    if (!Utility.IsDomandaAPEPrecoci(datiPensione) && beneficioDB.Id == "11")
                        continue;

                    // Il beneficio 13 è inseribile solo per le domande di Inabilità amianto
                    if (!Utility.IsDomandaInabilitaAmianto(datiPensione) && beneficioDB.Id == "13")
                        continue;

                    // Il beneficio 14 è inseribile solo per le domande di Quota 100
                    if (!Utility.IsDomandaQuota100(datiPensione) && beneficioDB.Id == "14")
                        continue;

                    // Il beneficio 18 è inseribile solo per le domande di Quota 102
                    if (!Utility.IsDomandaQuota102(datiPensione) && beneficioDB.Id == "18")
                        continue;

                    if (datiPensione.SceltaLavMadri.GetValueOrDefault() != 1 && beneficioDB.Id == "12")
                        continue;

                    if (datiPensione.SceltaLavMadri.GetValueOrDefault() != 2 && beneficioDB.Id == "15")
                        continue;

                    // Il beneficio 19 è inseribile solo per le domande Anticipate Flessibili
                    if (!Utility.IsDomandaAnticipataFlessibile(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && beneficioDB.Id == "19")
                        continue;

                    // Il beneficio 24 è inseribile solo per le domande Anticipata Flessibile Legge Bilancio 2024
                    if (!Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && beneficioDB.Id == "24")
                        continue;

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

                    Entity.TipoBenefici beneficio = new Entity.TipoBenefici();
                    beneficio.Id = beneficioDB.Id;
                    beneficio.Descrizione = descrizione;
                    listaTipoBenefici.Add(beneficio);
                }
            }
        }

        public static void GetListaCodiceMaggiorazioneExCombattente(out List<Entity.CodiceMaggiorazioneExCombattente> listaCodiceMaggiorazioneExCombattente)
        {
            listaCodiceMaggiorazioneExCombattente = new List<Entity.CodiceMaggiorazioneExCombattente>();
            List<Liquidazione.BLCommon.GestioneDecodifica.CodiceMaggiorazioneExCombattenti> listaCodiceMaggiorazioneExCombattenteDB = null;
            GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out listaCodiceMaggiorazioneExCombattenteDB);
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

        public static void GetListaCodiceCieco(out List<Entity.CodiceCieco> listaCodicCieco)
        {
            listaCodicCieco = new List<Entity.CodiceCieco>();
            List<Liquidazione.BLCommon.GestioneDecodifica.Cieco> listaCodiceCiecoDB = null;
            GestioneDecodifica.GetCodiceCieco(out listaCodiceCiecoDB);
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

        public static void GetListaRequisitiLegge50392(out List<Entity.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392)
        {
            listaCodiceRequisitiLegge50392 = new List<INPS.Pensioni.LiquidazioneCi.Entity.CodiceRequisitiLegge50392>();
            List<Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitiLegge50392> listaCodiceRequisitiLegge50392DB = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out listaCodiceRequisitiLegge50392DB);
            if (listaCodiceRequisitiLegge50392DB != null && listaCodiceRequisitiLegge50392DB.Count > 0)
            {
                foreach (Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitiLegge50392 codiceRequisitiLegge50392DB in listaCodiceRequisitiLegge50392DB)
                {
                    Entity.CodiceRequisitiLegge50392 codiceRequisitiLegge50392 = new INPS.Pensioni.LiquidazioneCi.Entity.CodiceRequisitiLegge50392();
                    codiceRequisitiLegge50392.Id = codiceRequisitiLegge50392DB.Id;
                    codiceRequisitiLegge50392.Descrizione = codiceRequisitiLegge50392DB.Descrizione;
                    codiceRequisitiLegge50392.TraduzioneSuGP = codiceRequisitiLegge50392DB.TraduzioneSuGP;
                    listaCodiceRequisitiLegge50392.Add(codiceRequisitiLegge50392);
                }
            }
        }

        public static void GetListaSoggettoBeneficiario(GestionePensione.DatiPensione datiPensione, out List<Entity.SoggettoBeneficiario> listaSoggettoBeneficiario)
        {
            listaSoggettoBeneficiario = new List<SoggettoBeneficiario>();
            List<GestioneDecodifica.SoggettoBeneficiario> listaSoggettoBeneficiarioDB = null;
            GestioneDecodifica.GetDecodificaSoggettoBeneficiario(out listaSoggettoBeneficiarioDB);
            if (listaSoggettoBeneficiarioDB != null && listaSoggettoBeneficiarioDB.Count > 0)
            {
                GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
                GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
                foreach (GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiarioDB in listaSoggettoBeneficiarioDB)
                {
                    if (Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) && soggettoBeneficiarioDB.TraduzioneSuGP == "V3 ")
                        continue;

                    Entity.SoggettoBeneficiario soggettoBeneficiario = new SoggettoBeneficiario();
                    Utility.ValorizzaOggetti(soggettoBeneficiarioDB, soggettoBeneficiario);
                    listaSoggettoBeneficiario.Add(soggettoBeneficiario);
                }
            }
        }

        public static void GetListaTipologiaPrestazione(out List<Entity.TipologiaPrestazione> listaTipologiaPrestazione)
        {
            listaTipologiaPrestazione = new List<TipologiaPrestazione>();
            List<GestioneDecodifica.TipologiaPrestazione> listaTipologiaPrestazioneDB = null;
            GestioneDecodifica.GetDecodificaTipologiaPrestazione(out listaTipologiaPrestazioneDB);
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

        public static void GetListaTipologiaBeneficioTerrorismo(out List<Entity.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismo)
        {
            listaTipologiaBeneficioTerrorismo = new List<TipologiaBeneficioTerrorismo>();
            List<GestioneDecodifica.TipologiaBeneficioTerrorismo> listaTipologiaBeneficioTerrorismoDB = null;
            GestioneDecodifica.GetDecTipologiaBeneficioTerrorismo(out listaTipologiaBeneficioTerrorismoDB);
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
        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici, out int? settimane)
        {
            bool? isBeneficioExArt80 = null;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isDomandaPensioneInabilita = null;
            bool? isBeneficioVittimeTerrorismo = null;
            bool? isBeneficioMaggiorazioneAmiantoLegge208_2015 = null;
            settimane = null;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            isBeneficioExArt80 = IsBeneficioExArt80(datiPensione);
            isBeneficioArt24Comma15BisFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE : null;
            isBeneficioApePrecociFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE : null;
            isDomandaPensioneInabilita = Utility.IsDomandaPensioneInabilitaOrRicostituzioneAGO_CI(datiPensione);
            if (datiBeneficioVittimeTerrorismo != null)
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione);
            else
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null);
            isBeneficioMaggiorazioneAmiantoLegge208_2015 = Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione);



            if (datiGenerici != null && datiGenerici.SettimaneItalianeDiritto.HasValue && datiGenerici.SettimaneItalianeDiritto.Value > 0)
                settimane = datiGenerici.SettimaneItalianeDiritto.Value;

            lReturn.Add("IsBeneficioExArt80", isBeneficioExArt80);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsDomandaPensioneInabilita", isDomandaPensioneInabilita);
            lReturn.Add("IsBeneficioVittimeTerrorismo", isBeneficioVittimeTerrorismo);
            lReturn.Add("IsBeneficioMaggiorazioneAmiantoLegge208_2015", isBeneficioMaggiorazioneAmiantoLegge208_2015);
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

        #endregion Cross Properties

        #region nested class

        public class DatiMaggiorazioniBenefici
        {
            public DatiMaggiorazioniBenefici()
            { }
            public DatiMaggiorazioniBenefici(long id, long idPensione, byte? codiceCieco, DateTime? decorrenzaMaggiorazioneArt6, DateTime? decorrenzaMaggiorazioneSociale,
                                             string tipoSettimaneBeneficio, DateTime? cessazioneMaggiorazioneSociale, int? nSettimaneBeneficio, DateTime? decorrenzaMaggiorazioneLegge140,
                                             short? anniRiduzioneBeneficiArt38Legge02, byte? codiceRequisitiLegge50392Art2)
            {
                this._Id = id;
                this._IdPensione = idPensione;
                this._CodiceCieco = codiceCieco;
                this._DecorrenzaMaggiorazioneArt6 = decorrenzaMaggiorazioneArt6;
                this._DecorrenzaMaggiorazioneSociale = decorrenzaMaggiorazioneSociale;
                this._CessazioneMaggiorazioneSociale = cessazioneMaggiorazioneSociale;
                this._TipoSettimaneBeneficio = tipoSettimaneBeneficio;
                this._NSettimaneBeneficio = nSettimaneBeneficio;
                this._DecorrenzaMaggiorazioneLegge140 = decorrenzaMaggiorazioneLegge140;
                this._AnniRiduzioneBeneficiArt38Legge02 = anniRiduzioneBeneficiArt38Legge02;
                this._CodiceRequisitiLegge50392Art2 = codiceRequisitiLegge50392Art2;
            }

            #region private properties

            private long _Id;
            private long _IdPensione;
            private byte? _CodiceCieco;
            private DateTime? _DecorrenzaMaggiorazioneArt6;
            private DateTime? _DecorrenzaMaggiorazioneSociale;
            private DateTime? _CessazioneMaggiorazioneSociale;
            private string _TipoSettimaneBeneficio;
            private DateTime? _DecorrenzaMaggiorazioneLegge140;

            private int? _NSettimaneBeneficio;

            private int? _NSettimaneIncremento1Percento;
            private int? _NSettimaneIncremento05Percento;
            private byte? _Sentenza495240;

            private short? _AnniRiduzioneBeneficiArt38Legge02;
            private byte? _CodiceRequisitiLegge50392Art2;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }
            public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
            public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }
            public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }
            public string TipoSettimaneBeneficio { get { return _TipoSettimaneBeneficio; } set { _TipoSettimaneBeneficio = value; } }
            public int? NSettimaneBeneficio { get { return _NSettimaneBeneficio; } set { _NSettimaneBeneficio = value; } }
            public int? NSettimaneIncremento1Percento { get { return _NSettimaneIncremento1Percento; } set { _NSettimaneIncremento1Percento = value; } }
            public int? NSettimaneIncremento05Percento { get { return _NSettimaneIncremento05Percento; } set { _NSettimaneIncremento05Percento = value; } }
            public byte? Sentenza495240 { get { return _Sentenza495240; } set { _Sentenza495240 = value; } }
            public DateTime? DecorrenzaMaggiorazioneLegge140 { get { return _DecorrenzaMaggiorazioneLegge140; } set { _DecorrenzaMaggiorazioneLegge140 = value; } }
            public short? AnniRiduzioneBeneficiArt38Legge02 { get { return _AnniRiduzioneBeneficiArt38Legge02; } set { _AnniRiduzioneBeneficiArt38Legge02 = value; } }
            public byte? CodiceRequisitiLegge50392Art2 { get { return _CodiceRequisitiLegge50392Art2; } set { _CodiceRequisitiLegge50392Art2 = value; } }

            #endregion public properties
        }

        public static bool IsMaggiorazioniBeneficiNull(DatiMaggiorazioniBenefici maggiorazioniBenefici)
        {
            if (!maggiorazioniBenefici.CodiceCieco.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.TipoSettimaneBeneficio) &&
                !maggiorazioniBenefici.NSettimaneIncremento1Percento.HasValue &&
                !maggiorazioniBenefici.NSettimaneIncremento05Percento.HasValue &&
                !maggiorazioniBenefici.Sentenza495240.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140.HasValue &&
                !maggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.HasValue &&
                !maggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue)
            {
                return true;
            }
            else
                return false;
        }

        #endregion nested class
    }
}
