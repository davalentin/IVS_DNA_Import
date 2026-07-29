using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneFs.Entity;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneMaggiorazioniBenefici
    {
        #region MaggiorazioniBenefici

        //public static void EliminaMaggiorazioniBenefici(long numeroDomanda)
        //{
        //    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
        //    Liquidazione.BLCommon.GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

        //    if (datiIstruttoria != null)
        //        datiIstruttoria.CodiceMobilita = null;

        //    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
        //            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        //    {
        //        if (datiIstruttoria != null)
        //        {
        //            if (GestioneIstruttoria.IsIstruttoriaNull(datiIstruttoria))
        //                GestioneIstruttoria.EliminaIstruttoria(idPensione);
        //            else
        //                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);
        //        }
        //        GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBenefici(idPensione);
        //        GestioneQuadri.InizializzaQuadroMaggiorazioniBenefici(idPensione);

        //        transactionScope.Complete();


        //    }
        //}

        public static bool ControlsVisibleTabs(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, bool? IsExCombattente, bool? IsBenefici, bool? IsDL407, bool? IsArticolo2, bool? IsPrivilegiate, bool? IsBeneficioVittimeTerrorismo)
        {
            if (IsExCombattente.HasValue && IsExCombattente.Value && datiPensione.ExCombattente.HasValue && datiPensione.ExCombattente.Value)
                return true;
            if (IsBenefici.HasValue && IsBenefici.Value && datiPensione.Benefici.HasValue && datiPensione.Benefici.Value)
                return true;
            if (IsDL407.HasValue && IsDL407.Value && datiFondo != null && datiFondo.ChkDL407.HasValue && datiFondo.ChkDL407.Value)
                return true;
            if (IsArticolo2.HasValue && IsArticolo2.Value && datiFondo != null && datiFondo.Articolo2.HasValue && datiFondo.Articolo2.Value)
                return true;
            if (IsPrivilegiate.HasValue && IsPrivilegiate.Value && datiFondo != null && datiFondo.Privilegiate.HasValue && datiFondo.Privilegiate.Value)
                return true;
            if (IsBeneficioVittimeTerrorismo.HasValue && IsBeneficioVittimeTerrorismo.Value)
                return true;

            return false;
        }

        #endregion MaggiorazioniBenefici

        #region DatiExCombattente

        public static bool ControlDatiExCombattente(string siglaCategoria, List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, Entity.DatiExCombattente datiExCombattente, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, siglaCategoria);

            if (datiExCombattente == null || datiExCombattente.IsDatiExCombattenteNull())
            {
                messaggioVideo = "Inserire almeno un dato 'Ex Combattente' prima di procedere con il salvataggio";
                return false;
            }

            if ((datiExCombattente.CodiceCieco.HasValue || datiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue) &&
                (datiExCombattente.ExCombattente.HasValue || datiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.HasValue || datiExCombattente.RMSSenzaLegge33670QA.HasValue || datiExCombattente.RMSSenzaLegge33670QB.HasValue))
            {
                messaggioVideo = "Non è possibile salvare contemporaneamente i dati della 'Legge140' e quelli della 'Legge336'";
                return false;
            }

            if (datiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue && !datiExCombattente.CodiceCieco.HasValue)
            {
                messaggioVideo = "In presenza della 'Decorrenza' della Legge 140 è obbligatorio inserire il 'Codice ex Combattente'";
                return false;
            }

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                case Utility.TipoFondo.ET:
                case Utility.TipoFondo.TT:
                case Utility.TipoFondo.VL:
                case Utility.TipoFondo.DZ:
                case Utility.TipoFondo.PM:

                    if ((datiExCombattente.RMSSenzaLegge33670QA.HasValue || datiExCombattente.RMSSenzaLegge33670QB.HasValue) && !datiExCombattente.ExCombattente.HasValue)
                    {
                        messaggioVideo = "In presenza del 'RMS senza Legge336 quota A' e 'RMS senza Legge336 quota B' è obbligatorio inserire la 'Maggiorazione ex Combattente'";
                        return false;
                    }

                    if ((datiExCombattente.ExCombattente.HasValue && !datiExCombattente.RMSSenzaLegge33670QA.HasValue) || (datiExCombattente.ExCombattente.HasValue && !datiExCombattente.RMSSenzaLegge33670QB.HasValue && Utility.DataSuccessivaA(Utility.FirstDayOfMonth(decorrenzaOriginaria.GetValueOrDefault()), new DateTime(1993, 2, 1))))
                    {
                        messaggioVideo = "In presenza della 'Maggiorazione ex Combattente' è obbligatorio inserire la 'RMS senza Legge336 quota A' e 'RMS senza Legge336 quota B'";
                        return false;
                    }

                    if (datiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.HasValue && datiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.Value > 10)
                    {
                        messaggioVideo = "La percentuale Maggiorazione senza legge 336/70 non può essere superiore a 10";
                        return false;
                    }

                    break;

                case Utility.TipoFondo.FS:

                    if ((datiExCombattente.RMSSenzaLegge33670QA.HasValue || datiExCombattente.RMSSenzaLegge33670QB.HasValue) && !datiExCombattente.ExCombattente.HasValue)
                    {
                        messaggioVideo = "In presenza delle retribuzioni è obbligatorio inserire la 'Maggiorazione ex Combattente'";
                        return false;
                    }
                    break;
                case Utility.TipoFondo.ES:
                case Utility.TipoFondo.PI:
                case Utility.TipoFondo.PL:

                    if (datiExCombattente.RMSSenzaLegge33670QA.HasValue && !datiExCombattente.ExCombattente.HasValue)
                    {
                        messaggioVideo = "In presenza del 'RMS senza Legge336 quota A' è obbligatorio inserire la 'Maggiorazione ex Combattente'";
                        return false;
                    }

                    if (datiExCombattente.ExCombattente.HasValue && !datiExCombattente.RMSSenzaLegge33670QA.HasValue)
                    {
                        messaggioVideo = "In presenza della 'Maggiorazione ex Combattente' è obbligatorio inserire la 'RMS senza Legge336 quota A'";
                        return false;
                    }
                    break;
            }

            if (!GestioneControlli.VerificaExCombattentePerPIU(listaRecordFondo.Exists(x => x.CodiceNonCalcolo == 'N'), datiExCombattente.ExCombattente, datiExCombattente.RMSSenzaLegge33670QA,
                categoriaFondoPI, out messaggioVideo))
                return false;

            return true;
        }

        public static void ValorizzaDatiExCombattente(Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, out Entity.DatiExCombattente datiExCombattente)
        {
            datiExCombattente = new Entity.DatiExCombattente();
            Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiExCombattente);
            if (datiExCombattente.IsDatiExCombattenteNull())
                datiExCombattente = null;
        }

        public static void StoreDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Entity.DatiExCombattente datiExCombattente)
        {
            if (datiExCombattente == null)
                datiExCombattente = new Entity.DatiExCombattente();

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiExCombattentePrivate(datiPensione, datiExCombattente, ref datiMaggiorazioniBenefici);

                if (datiExCombattente.IsDatiExCombattenteNull())
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                else
                    datiQuadroMaggiorazioniBenefici.TabExCombattente = 2;

                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue && datiQuadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && datiQuadroMaggiorazioniBenefici.TabArticolo2.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && datiQuadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;

        }

        private static void StoreDatiExCombattentePrivate(GestionePensione.DatiPensione datiPensione, Entity.DatiExCombattente datiExCombattente, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
            {
                datiMaggiorazioniBenefici = new Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici();
                datiMaggiorazioniBenefici.IdPensione = datiPensione.Id;
            }

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoFondo == Utility.TipoFondo.FS)
                datiExCombattente.RMSSenzaLegge33670QA = datiMaggiorazioniBenefici.RMSSenzaLegge33670QA;

            Utility.ValorizzaOggetti(datiExCombattente, datiMaggiorazioniBenefici);
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
        }

        public static void EliminaDatiExCombattente(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (datiMaggiorazioniBenefici != null)
            {
                datiMaggiorazioniBenefici.CodiceCieco = null;
                datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneArt6 = null;
                datiMaggiorazioniBenefici.ExCombattente = null;
                if (tipoFondo != Utility.TipoFondo.FS)
                    datiMaggiorazioniBenefici.RMSSenzaLegge33670QA = null;
                datiMaggiorazioniBenefici.RMSSenzaLegge33670QB = null;
                datiMaggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670 = null;
                datiMaggiorazioniBenefici.DirittoScattiLegge336 = null;
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBenefici != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBenefici))
                    {
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                        datiMaggiorazioniBenefici = null;
                    }
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
                }
                datiQuadroMaggiorazioniBenefici.TabExCombattente = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;

        }

        #endregion DatiExCombattente

        #region DatiBenefici

        public static bool ControlDatiBeneficiForCancel(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, object datiFondoXX, GestioneFondo.DatiFondo datiFondo, char? codiceSpecifico,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Entity.DatiBenefici datiBenefici = new INPS.Pensioni.LiquidazioneFs.Entity.DatiBenefici();
            datiBenefici.TipoSettimaneBeneficio = string.Empty;
            ControlDatiBenefici(ref contenitore, ref contenitoreDecodifica, datiPensione, datiBenefici, datiAnagraficaTitolare, datiMaggiorazioniBenefici, datiFondoXX, datiFondo, codiceSpecifico, true, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, Entity.DatiBenefici datiBenefici, GestioneAnagrafica.DatiAnagrafici datiAnagraficaTitolare,
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, object datiFondoXX, GestioneFondo.DatiFondo datiFondo,
            char? codiceSpecificoTraduzioneSuGP, bool IsCancelOperation, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            int? maggiorazioneAmianto = null;
            int? maggiorazioneInv74 = null;
            if (datiMaggiorazioniBenefici != null)
            {
                maggiorazioneAmianto = datiMaggiorazioniBenefici.MaggiorazioneAmianto;
                maggiorazioneInv74 = datiMaggiorazioniBenefici.MaggiorazioneInv74;
            }

            if (datiBenefici != null)
            {
                if (!IsCancelOperation && datiBenefici.IsDatiBeneficiNull())
                {
                    messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                    return false;
                }

                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                    case Utility.TipoFondo.ET:
                    case Utility.TipoFondo.TT:
                    case Utility.TipoFondo.VL:
                    case Utility.TipoFondo.DZ:
                    case Utility.TipoFondo.ES:
                    case Utility.TipoFondo.PM:
                        if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull() && string.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio) && (datiBenefici.NSettimaneBeneficio.HasValue))
                        {
                            messaggioVideo = "In presenza delle 'Settimane Beneficio', inserire il 'Tipo beneficio'";
                            return false;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                    case Utility.TipoFondo.PT:
                        if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull() && (datiBenefici.SettimaneBeneficioAA.HasValue || datiBenefici.SettimaneBeneficioMM.HasValue || datiBenefici.SettimaneBeneficioGG.HasValue) && String.IsNullOrEmpty(datiBenefici.TipoSettimaneBeneficio))
                        {
                            messaggioVideo = "In presenza del 'Beneficio temporale', inserire il 'Tipo beneficio'";
                            return false;
                        }

                        if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull() && datiBenefici.SettimaneBeneficioAA.HasValue && datiBenefici.SettimaneBeneficioAA.Value > 99)
                        {
                            messaggioVideo = "Il 'Beneficio temporale AA' deve essere minore di 99";
                            return false;
                        }

                        if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull() && datiBenefici.SettimaneBeneficioMM.HasValue && datiBenefici.SettimaneBeneficioMM.Value > 11)
                        {
                            messaggioVideo = "Il 'Beneficio temporale MM' deve essere minore di 12";
                            return false;
                        }

                        if (datiBenefici != null && !datiBenefici.IsDatiBeneficiNull() && datiBenefici.SettimaneBeneficioGG.HasValue && datiBenefici.SettimaneBeneficioGG.Value > 30)
                        {
                            messaggioVideo = "Il 'Beneficio temporale GG' deve essere minore di 30";
                            return false;
                        }
                        break;
                }

                if (datiBenefici.CessazioneMaggiorazioneSociale.HasValue && !datiBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                {
                    messaggioVideo = "In presenza della data Cessazione è obbligatorio inserire la data Decorrenza";
                    return false;
                }

                if ((datiBenefici.DecorrenzaMaggiorazioneSociale.HasValue && datiBenefici.CessazioneMaggiorazioneSociale.HasValue &&
                    datiBenefici.CessazioneMaggiorazioneSociale.Value.CompareTo(datiBenefici.DecorrenzaMaggiorazioneSociale.Value) < 0))
                {
                    messaggioVideo = "La data Cessazione deve essere maggiore della data Decorrenza";
                    return false;
                }

                if (datiBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                {
                    GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = contenitore.DatiStoricoGP;
                    if (!GestioneControlli.ControlsDecMaggiorazioneSociale(ref contenitore, ref contenitoreDecodifica, datiBenefici.DecorrenzaMaggiorazioneSociale, datiPensione, datiAnagraficaTitolare, out messaggioVideo))
                        return false;

                    if (!GestioneCrossControls.ALL_ControlsDecorrenzaMaggiorazioneWithDataPresentazione(datiBenefici.DecorrenzaMaggiorazioneSociale, datiPensione, datiAnagraficaTitolare != null ? datiAnagraficaTitolare.DataNascita : null,
                         datiStoricoGP != null ? datiStoricoGP.DecorrenzaMaggiorazioneSociale.HasValue : false, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda, out messaggioVideo))
                        return false;
                }

                GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = contenitore.DatiRetributivi;
                List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = contenitore.ListaDatiCalcoloContributivoRecordFondo;
                List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile = contenitore.ListaDatiServizioUtile;
                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;

                List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    listaRecordDatiFondoINPDAP = contenitore.ListaRecordDatiFondoINPDAP;

                object objectFondoXX = contenitore.ObjectFondoXX;
                DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

                try
                {
                    if ((listaDatiContributivi == null || listaDatiContributivi.Count == 0) && datiRetributivi != null)  //controllo calcolo retributivo
                    {
                        bool result = true;
                        string messaggioCommon = "Prima di modificare il campo Non Vedente, Amianto o Invalidità 74% verificare il numero di settimane in 'Dati Calcolo'. ";
                        switch (tipoFondo)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                result = GestioneControlli.ControlsCalcoloRetributivoForMaggiorazioneBenefici(datiRetributivi, datiBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74,
                                    out messaggioVideo);
                                break;
                            case Utility.TipoFondo.ET:
                            case Utility.TipoFondo.DZ:
                                result = GestioneControlli.ControlsCalcoloRetributivoFondoETForMaggiorazioneBenefici(datiRetributivi, datiBenefici, datiPensione, datiDanteCausa, listaServizioUtile,
                                    codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, tipoFondo, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                result = GestioneControlli.ControlsCalcoloRetributivoFondoVLForMaggiorazioneBenefici(datiRetributivi, datiBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74,
                                    out messaggioVideo);
                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                //Controlli non previsti per i fondi FS e PT
                                break;
                        }

                        if (!result)
                        {
                            messaggioVideo = string.Concat(messaggioCommon, messaggioVideo);
                            return false;
                        }
                    }
                    //controllo calcolo retributivo Monti
                    else if ((listaDatiContributivi != null && listaDatiContributivi.Count > 0) && datiRetributivi != null && datiPensione.TipoCalcolo.HasValue && datiPensione.TipoCalcolo.Value == 25)
                    {
                        bool result = true;
                        string messaggioCommon = "Prima di modificare il campo Non Vedente, Amianto o Invalidità 74% verificare il numero di settimane in 'Dati Calcolo'. ";
                        switch (tipoFondo)
                        {
                            case Utility.TipoFondo.EL:
                            case Utility.TipoFondo.TT:
                                result = GestioneControlli.ControlsCalcoloRetributivoMontiForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), datiBenefici, datiPensione, codiceSpecificoTraduzioneSuGP,
                                    maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.ET:
                            case Utility.TipoFondo.DZ:
                                result = GestioneControlli.ControlsCalcoloRetributivoMontiFondoETForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), datiBenefici, datiPensione, listaServizioUtile,
                                    codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.VL:
                                result = GestioneControlli.ControlsCalcoloRetributivoMontiFondoVLForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), datiBenefici, datiPensione, codiceSpecificoTraduzioneSuGP,
                                    maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                //Controlli non previsti per i fondi FS e PT
                                break;
                        }

                        if (!result)
                        {
                            messaggioVideo = string.Concat(messaggioCommon, messaggioVideo);
                            return false;
                        }
                    }
                    else //controllo calcolo misto
                    {
                        if ((listaDatiContributivi != null && listaDatiContributivi.Count > 0) && datiRetributivi != null)
                        {
                            bool result = true;
                            string messaggioCommon = "Prima di modificare il campo Non Vedente, Amianto o Invalidità 74% verificare il numero di settimane in 'Dati Calcolo'. ";

                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.EL:
                                case Utility.TipoFondo.TT:
                                    result = GestioneControlli.ControlsCalcoloMistoForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), datiBenefici, datiPensione, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto,
                                        maggiorazioneInv74, out messaggioVideo);
                                    break;
                                case Utility.TipoFondo.ET:
                                case Utility.TipoFondo.DZ:
                                    result = GestioneControlli.ControlsCalcoloMistoFondoETForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), datiBenefici, datiPensione, listaServizioUtile,
                                        codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                    break;
                                case Utility.TipoFondo.VL:
                                    result = GestioneControlli.ControlsCalcoloMistoFondoVLForMaggiorazioneBenefici(datiRetributivi, listaDatiContributivi.FirstOrDefault(), codiceSpecificoTraduzioneSuGP, datiBenefici, datiPensione,
                                        maggiorazioneAmianto, maggiorazioneInv74, out messaggioVideo);
                                    break;
                                case Utility.TipoFondo.FS:
                                case Utility.TipoFondo.PT:
                                    //Controlli non previsti per i fondi FS e PT
                                    break;
                            }

                            if (!result)
                            {
                                messaggioVideo = string.Concat(messaggioCommon, messaggioVideo);
                                return false;
                            }
                        }
                    }

                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.ET:
                        case Utility.TipoFondo.DZ:
                            if (listaServizioUtile != null && listaServizioUtile.Count > 0)
                            {
                                if (!GestioneControlli.ControlsServizioUtileForMaggiorazioneBenefici(datiPensione, datiBenefici, listaServizioUtile, codiceSpecificoTraduzioneSuGP, maggiorazioneAmianto, maggiorazioneInv74,
                                    out messaggioVideo))
                                {
                                    messaggioVideo = "Controllo incrociato: " + messaggioVideo + ". E' necessario rivedere i Dati Calcolo";
                                    return false;
                                }
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                    messaggioVideo = "Controllo validità calcolo non riuscito. Controllare inserimento delle date di inizio e fine assicurazione";
                    return false;
                }

                if (listaDatiContributivi != null && listaDatiContributivi.Count > 0)
                {
                    foreach (var datiContributivi in listaDatiContributivi)
                    {
                        if (!GestioneControlli.ControlsSettimaneBeneficioNonVedenteWithDatiCalcolo(datiPensione, datiRetributivi, datiContributivi, listaServizioUtile, datiBenefici.TipoSettimaneBeneficio,
                            datiBenefici.NSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                            return false;
                    }
                }
                else
                {
                    if (!GestioneControlli.ControlsSettimaneBeneficioNonVedenteWithDatiCalcolo(datiPensione, datiRetributivi, null, listaServizioUtile, datiBenefici.TipoSettimaneBeneficio,
                        datiBenefici.NSettimaneBeneficio, codiceSpecificoTraduzioneSuGP, out messaggioVideo))
                        return false;
                }

                if (!GestioneCrossControls.FS_ControlsTipoBeneficioArt24Comma15Bis(datiPensione, datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio, datiPensione.DecorrenzaOriginaria,
                    datiPensione.NaturaPensione, datiAnagraficaTitolare.Sesso, datiAnagraficaTitolare.DataNascita, codiceSpecificoTraduzioneSuGP, datiRetributivi, listaDatiContributivi, listaServizioUtile,
                    listaRecordDatiFondoINPDAP, objectFondoXX, datiFondo != null ? datiFondo.SettimaneUtiliDiritto : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsBeneficioPrecoci(datiPensione, datiFondoXX, datiFondo, datiBenefici.TipoSettimaneBeneficio, decorrenzaPensioneOrDecorrenzaPensioneDC, tipoFondo, out messaggioVideo))
                    return false;

                if (tipoFondo != Utility.TipoFondo.CL &&
                    !GestioneCrossControls.ALL_ControlsLavoratoriNonVedenti(datiBenefici.TipoSettimaneBeneficio, datiBenefici.NSettimaneBeneficio, datiBenefici.SettAnzContribPost311295, datiPensione, datiDanteCausa, out messaggioVideo))
                    return false;
            }
            else
            {
                messaggioVideo = "Inserire almeno un dato di 'Benefici' prima di procedere con il salvataggio";
                return false;
            }
            return true;
        }

        public static void ValorizzaDatiBeneficiByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, out Entity.DatiBenefici datiBenefici)
        {
            datiBenefici = new Entity.DatiBenefici();
            Utility.ValorizzaOggetti(datiMaggiorazioniBenefici, datiBenefici);
            if (datiBenefici.IsDatiBeneficiNull())
                datiBenefici = null;

            List<GestioneRipartizioneFondi.DatiRipartizioneFondi> LdatiRipartizioneFondi = contenitore.ListaDatiRipartizioneFondi;
            if (LdatiRipartizioneFondi != null && LdatiRipartizioneFondi.Count > 0)
            {
                if (datiBenefici == null)
                    datiBenefici = new Entity.DatiBenefici();
                datiBenefici.ListOneriTerrorismo = new List<INPS.Pensioni.LiquidazioneFs.Entity.DatiBenefici.OneriTerrorismo>();
                foreach (GestioneRipartizioneFondi.DatiRipartizioneFondi rf in LdatiRipartizioneFondi)
                {
                    Entity.DatiBenefici.OneriTerrorismo ot = new Entity.DatiBenefici.OneriTerrorismo();
                    Liquidazione.BLCommon.Utility.ValorizzaOggetti(rf, ot);
                    datiBenefici.ListOneriTerrorismo.Add(ot);
                }
            }
        }

        public static void StoreDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, Entity.DatiBenefici datiBenefici)
        {
            if (datiBenefici != null)
            {
                GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
                GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
                List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariBlCommon = null;
                List<GestioneRipartizioneFondi.DatiRipartizioneFondi> listaDatiRipartizioneFondi = contenitore.ListaDatiRipartizioneFondi;
                List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaBeneficiParticolari = contenitore.ListaDatiBeneficiParticolari;

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
                        listaDatiRipartizioneFondi = null;
                        foreach (Entity.DatiBenefici.OneriTerrorismo ot in datiBenefici.ListOneriTerrorismo)
                        {
                            if (listaDatiRipartizioneFondi == null)
                                listaDatiRipartizioneFondi = new List<GestioneRipartizioneFondi.DatiRipartizioneFondi>();

                            ot.IdPensione = datiPensione.Id;
                            GestioneRipartizioneFondi.DatiRipartizioneFondi datiRipartizioneFondi = new GestioneRipartizioneFondi.DatiRipartizioneFondi();
                            Utility.ValorizzaOggetti(ot, datiRipartizioneFondi);
                            GestioneRipartizioneFondi.SalvaRipartizioneFondi(datiRipartizioneFondi);
                            listaDatiRipartizioneFondi.Add(datiRipartizioneFondi);
                        }
                    }

                    #endregion RipartizioneFondi

                    #region Benefici Particolari
                    if (datiMaggiorazioniBenefici != null && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01")
                    {
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                            StoreDatiBeneficiForBeneficiParticolari(lBeneficiParticolariBlCommon, datiPensione, ref listaBeneficiParticolari);

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
                   (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue && datiQuadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && datiQuadroMaggiorazioniBenefici.TabArticolo2.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && datiQuadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2))
                        datiQuadroMaggiorazioniBenefici.Tipo = 2;
                    else
                        datiQuadroMaggiorazioniBenefici.Tipo = 1;

                    GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                    transactionScope.Complete();
                }

                /* ---AGGIORNO I DATI SUL CONTENITORE --- */
                contenitore.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
                contenitore.ListaDatiRipartizioneFondi = listaDatiRipartizioneFondi;
                contenitore.ListaDatiBeneficiParticolari = listaBeneficiParticolari;
                contenitore.DatiQuadroOneri = datiQuadroOneri;
                contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
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

        public static void EliminaDatiBenefici(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, List<GestioneOneri.DatiOneri> listaDatiOneri)
        {
            Utility.TipoFondo? tipofondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            bool isClearQuadroOneri = false;
            bool datiMaggiorazioneBeneficiEliminati = false;

            if (datiMaggiorazioniBenefici != null)
            {
                if (!datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() &&
                    !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio = null;
                if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))
                {
                    if (listaDatiOneri == null || listaDatiOneri.Count == 0)
                        isClearQuadroOneri = true;
                    datiMaggiorazioniBenefici.NSettimaneBeneficio = null;
                }
                datiMaggiorazioniBenefici.CessazioneMaggiorazioneSociale = null;
                datiMaggiorazioniBenefici.DecorrenzaMaggiorazioneSociale = null;
                datiMaggiorazioniBenefici.SettimaneBeneficioAA = null;
                datiMaggiorazioniBenefici.SettimaneBeneficioMM = null;
                datiMaggiorazioniBenefici.SettimaneBeneficioGG = null;
            }

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroOneri datiQuadroOneri = contenitore.DatiQuadroOneri;
            List<GestioneRipartizioneFondi.DatiRipartizioneFondi> listaDatiRipartizioniFondi = contenitore.ListaDatiRipartizioneFondi;
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = contenitore.ListaDatiBeneficiParticolari;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiMaggiorazioniBenefici != null)
                {
                    if (Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.IsMaggiorazioniBeneficiNull(datiMaggiorazioniBenefici))
                    {
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(datiPensione.Id);
                        datiMaggiorazioneBeneficiEliminati = true;
                    }
                    else
                        Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBenefici);
                }

                if (tipofondo.HasValue)
                {
                    switch (tipofondo.Value)
                    {
                        case Utility.TipoFondo.EL:
                        case Utility.TipoFondo.ET:
                        case Utility.TipoFondo.TT:
                        case Utility.TipoFondo.VL:
                        case Utility.TipoFondo.GAS:
                        case Utility.TipoFondo.DZ:
                        case Utility.TipoFondo.ES:
                        case Utility.TipoFondo.PM:
                            GestioneRipartizioneFondi.EliminaRipartizioneFondiByIdPensione(datiPensione.Id);
                            listaDatiRipartizioniFondi = null;
                            break;
                    }
                }

                if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiMaggiorazioniBenefici != null &&
                    datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "01"))
                {
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);
                    listaDatiBeneficiParticolari = null;
                }

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

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiMaggiorazioniBenefici = (!datiMaggiorazioneBeneficiEliminati) ? datiMaggiorazioniBenefici : null;
            contenitore.ListaDatiRipartizioneFondi = listaDatiRipartizioniFondi;
            contenitore.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            contenitore.DatiQuadroOneri = datiQuadroOneri;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        #endregion DatiBenefici

        #region Dati Benefici Particolari

        private static void StoreDatiBeneficiForBeneficiParticolari(List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lBeneficiParticolariDB, GestionePensione.DatiPensione datiPensione, ref List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaBeneficiParticolari)
        {
            if (lBeneficiParticolariDB != null && lBeneficiParticolariDB.Count > 0)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    GestioneBeneficiParticolari.DeleteDatiBeneficiParticolariByIdPensione(datiPensione.Id);
                    listaBeneficiParticolari = null;
                    foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari beneficiParticolariCommon in lBeneficiParticolariDB)
                    {
                        if (!beneficiParticolariCommon.IsDatiBeneficiParticolariNull())
                        {
                            if (listaBeneficiParticolari == null)
                                listaBeneficiParticolari = new List<GestioneBeneficiParticolari.DatiBeneficiParticolari>();

                            beneficiParticolariCommon.IdPensione = datiPensione.Id;
                            GestioneBeneficiParticolari.SalvaDatiBeneficiParticolari(beneficiParticolariCommon);
                            listaBeneficiParticolari.Add(beneficiParticolariCommon);
                        }
                    }

                    transactionScope.Complete();
                }
            }
        }

        #endregion Dati Benefici Particolari

        #region DatiDL407

        public static void GetDatiDL407ByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiDL407 datiDL407)
        {
            datiDL407 = null;
            Liquidazione.BLCommon.GestioneDL407.DatiDL407 dl407 = contenitore.Dl407;
            if (dl407 == null)
                return;

            datiDL407 = new Entity.DatiDL407();
            Utility.ValorizzaOggetti(dl407, datiDL407);
            ValorizzaEntityDL407ForAnteArm(dl407, ref datiDL407);
        }



        public static void StoreDatiDL407(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, Entity.DatiDL407 datiDL407)
        {
            if (datiDL407 == null)
                datiDL407 = new Entity.DatiDL407();

            GestioneDL407.DatiDL407 dl407Common = new GestioneDL407.DatiDL407();

            dl407Common.IdPensione = datiPensione.Id;
            dl407Common.NSettimaneQuotaA = datiDL407.NSettimaneQuotaA;
            dl407Common.NSettimaneQuotaB = datiDL407.NSettimaneQuotaB;
            dl407Common.NSettimaneQuotaC = datiDL407.NSettimaneQuotaC;
            dl407Common.NSettimaneQuotaD = datiDL407.NSettimaneQuotaD;
            dl407Common.RMSQuotaA = datiDL407.RMSQuotaA;
            dl407Common.RMSQuotaB = datiDL407.RMSQuotaB;
            dl407Common.RMSQuotaD = datiDL407.RMSQuotaD;
            ValorizzaBLCommonDL407ForAnteArm(datiDL407.LstServizioUtileAnteArm, ref dl407Common);

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!dl407Common.IsDL407Null())
                {
                    GestioneDL407.SalvaDL407(datiPensione.Id, dl407Common);
                    datiQuadroMaggiorazioniBenefici.TabLegge407 = 2;
                }
                else
                {
                    if (datiFondo != null && datiFondo.ChkDL407.HasValue && datiFondo.ChkDL407.Value)
                    {
                        GestioneDL407.EliminaDL407ByIdPensione(datiPensione.Id);
                        dl407Common = null;
                        datiQuadroMaggiorazioniBenefici.TabLegge407 = 0;
                    }
                }

                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue && datiQuadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && datiQuadroMaggiorazioniBenefici.TabArticolo2.Value == 2) ||
                   (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && datiQuadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.Dl407 = dl407Common;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }



        public static void EliminaDL407(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneDL407.EliminaDL407ByIdPensione(contenitore.DatiPensione.Id);
                datiQuadroMaggiorazioniBenefici.TabLegge407 = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(contenitore.DatiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.Dl407 = null;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static bool ControlsDatiDL407(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Entity.DatiDL407 datiDL407, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isCalcoloValid = false;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = contenitore.DatiDanteCausa;

            if (GetVisibilitaTabDL407AnteArm(datiPensione, datiDanteCausa, datiPensione.SiglaCategoria))
            {
                if (datiDL407 != null)
                    GestioneControlli.ControlsDatiLegge407AnteArmonizzazione(datiDL407.LstServizioUtileAnteArm, datiPensione, out messaggioVideo);
            }
            else
            {
                if (datiDL407 != null && !datiDL407.IsDL407Null())
                {
                    int NsettA = datiDL407.NSettimaneQuotaA.HasValue ? datiDL407.NSettimaneQuotaA.Value : 0;
                    int NsettB = datiDL407.NSettimaneQuotaB.HasValue ? datiDL407.NSettimaneQuotaB.Value : 0;
                    int NsettC = datiDL407.NSettimaneQuotaC.HasValue ? datiDL407.NSettimaneQuotaC.Value : 0;
                    int NsettD = datiDL407.NSettimaneQuotaD.HasValue ? datiDL407.NSettimaneQuotaD.Value : 0;

                    GestioneControlli.ControlsNumeroSingoleSettimaneDL407(NsettA, NsettB, NsettC, NsettD, out messaggioVideo, out isCalcoloValid);
                }
                else
                {
                    messaggioVideo = "Inserire almeno un dato di 'D.L. 407' prima di effettuare il salvataggio";
                }
            }
            return isCalcoloValid;
        }

        #region Private methods
        private static void ValorizzaBLCommonDL407ForAnteArm(List<DatiServizioUtileDL407> lstServizioUtile, ref GestioneDL407.DatiDL407 dl407Common)
        {
            if (lstServizioUtile != null && lstServizioUtile.Count > 0)
            {
                foreach (var elemServUtil in lstServizioUtile)
                {
                    switch (elemServUtil.Quota)
                    {
                        case "A":
                            dl407Common.ServizioUtileAAQuotaA = Convert.ToByte(elemServUtil.ServizioUtileAA);
                            dl407Common.RetribPensQuotaA = elemServUtil.RetribuzionePensionabile;
                            dl407Common.RetribPensSL336QuotaA = elemServUtil.RetribPensSL336;
                            break;
                        case "B":
                            dl407Common.ServizioUtileAAQuotaB = Convert.ToByte(elemServUtil.ServizioUtileAA);
                            dl407Common.RetribPensQuotaB = elemServUtil.RetribuzionePensionabile;
                            dl407Common.RetribPensSL336QuotaB = elemServUtil.RetribPensSL336;
                            break;
                        case "C":
                            dl407Common.ServizioUtileAAQuotaC = Convert.ToByte(elemServUtil.ServizioUtileAA);
                            break;
                    }

                }
            }

        }

        private static void ValorizzaEntityDL407ForAnteArm(GestioneDL407.DatiDL407 dl407, ref Entity.DatiDL407 datiDL407)
        {
            if (dl407.ServizioUtileAAQuotaA.HasValue || dl407.RetribPensQuotaA.HasValue || dl407.RetribPensSL336QuotaA.HasValue)
            {
                datiDL407.LstServizioUtileAnteArm = new List<DatiServizioUtileDL407>();

                DatiServizioUtileDL407 elem = new DatiServizioUtileDL407();
                elem.Quota = "A";
                elem.ServizioUtileAA = dl407.ServizioUtileAAQuotaA;
                elem.RetribuzionePensionabile = dl407.RetribPensQuotaA;
                elem.RetribPensSL336 = dl407.RetribPensSL336QuotaA;
                datiDL407.LstServizioUtileAnteArm.Add(elem);

            }
            if (dl407.ServizioUtileAAQuotaB.HasValue || dl407.RetribPensQuotaB.HasValue || dl407.RetribPensSL336QuotaB.HasValue)
            {
                if (datiDL407.LstServizioUtileAnteArm == null)
                    datiDL407.LstServizioUtileAnteArm = new List<DatiServizioUtileDL407>();
                DatiServizioUtileDL407 elem = new DatiServizioUtileDL407();
                elem.Quota = "B";
                elem.ServizioUtileAA = dl407.ServizioUtileAAQuotaB;
                elem.RetribuzionePensionabile = dl407.RetribPensQuotaB;
                elem.RetribPensSL336 = dl407.RetribPensSL336QuotaB;
                datiDL407.LstServizioUtileAnteArm.Add(elem);
            }
            if (dl407.ServizioUtileAAQuotaC.HasValue)
            {
                if (datiDL407.LstServizioUtileAnteArm == null)
                    datiDL407.LstServizioUtileAnteArm = new List<DatiServizioUtileDL407>();
                DatiServizioUtileDL407 elem = new DatiServizioUtileDL407();
                elem.Quota = "C";
                elem.ServizioUtileAA = dl407.ServizioUtileAAQuotaC;
                datiDL407.LstServizioUtileAnteArm.Add(elem);
            }
        }
        #endregion Private methods
        #endregion DatiDL407

        #region DatiArticolo2

        public static void GetDatiArticolo2ByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiArticolo2 datiArticolo2)
        {
            datiArticolo2 = null;
            GestioneFondo.DatiFondoPT datiFondoPT = contenitore.DatiFondoPT;
            if (datiFondoPT != null)
            {
                datiArticolo2 = new Entity.DatiArticolo2();
                Utility.ValorizzaOggetti(datiFondoPT, datiArticolo2);
            }

            ////////Prevalorizziamo la Data Fine Beneficio Art 2 nel caso in cui in DatiGenerici è presente la ScedenzaRevisioneSanitaria e nel caso in cui non è stata precedentemente salvata alcnuna Data Fine Beneficio Art 2
            ////////Dati presenti SOLO per fondo PT. Riferimento mail del 10/07/2013 avente il seguente oggetto: ReEng - modifiche fondo PT
            //////GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            //////GestioneIstruttoria.GetIstruttoriaByNumeroDomanda(numeroDomanda, out datiIstruttoria);

            //////if (datiIstruttoria != null && datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
            //////{
            //////    if (datiArticolo2 == null)
            //////        datiArticolo2 = new Entity.DatiArticolo2();

            //////    if (!datiArticolo2.DataInzioBeneficioArt2.HasValue && !datiArticolo2.DataFineBeneficioArt2.HasValue)
            //////        datiArticolo2.DataFineBeneficioArt2 = datiIstruttoria.ScadenzaRevisioneSanitaria.Value;
            //////}
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (datiArticolo2 != null && datiArticolo2.IsDatiArticolo2Null())
                datiArticolo2 = null;
        }

        public static void StoreDatiArticolo2(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, Entity.DatiArticolo2 datiArticolo2)
        {
            if (datiArticolo2 == null)
                datiArticolo2 = new Entity.DatiArticolo2();

            GestioneFondo.DatiFondoPT datiFondoPT = contenitore.DatiFondoPT;
            long idFondo = contenitore.IdFondoPensione;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            Utility.ValorizzaOggetti(datiArticolo2, datiFondoPT);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (!datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                {
                    GestioneFondo.SalvaFondoPT(idFondo, datiFondoPT);
                    datiQuadroMaggiorazioniBenefici.TabArticolo2 = 2;
                }
                else
                {
                    GestioneFondo.EliminaFondoPT(datiPensione.Id);
                    datiFondoPT = null;
                    datiQuadroMaggiorazioniBenefici.TabArticolo2 = 0;
                }

                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue && datiQuadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && datiQuadroMaggiorazioniBenefici.TabArticolo2.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && datiQuadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiFondoPT = datiFondoPT;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static void EliminaDatiArticolo2(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            GestioneFondo.DatiFondoPT datiFondoPT = contenitore.DatiFondoPT;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            datiFondoPT.DataFineBeneficioArt2 = null;
            datiFondoPT.DataInzioBeneficioArt2 = null;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {

                if (datiFondoPT == null || datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                {
                    GestioneFondo.EliminaFondoPT(contenitore.DatiPensione.Id);
                    datiFondoPT = null;
                }
                else
                    GestioneFondo.SalvaFondoPT(datiFondoPT.IdFondo, datiFondoPT);

                datiQuadroMaggiorazioniBenefici.TabArticolo2 = 0;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(contenitore.DatiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* --- AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiFondoPT = datiFondoPT;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static bool ControlsDatiArticolo2(Entity.DatiArticolo2 datiArticolo2, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isCalcoloValid = true;

            // La data fine beneficio articolo 2 non è obbligatoria nel caso di fondi PT - Riferimento mail del 10/07/2013 - Oggetto: ReEng - modifiche fondo PT
            if (!datiArticolo2.DataInzioBeneficioArt2.HasValue)
            {
                isCalcoloValid = false;
                messaggioVideo = "La Data Inizio Beneficio Art2 è obbligatoria";
            }

            if (datiArticolo2.DataInzioBeneficioArt2.HasValue && datiArticolo2.DataFineBeneficioArt2.HasValue && datiArticolo2.DataInzioBeneficioArt2.Value.CompareTo(datiArticolo2.DataFineBeneficioArt2.Value) >= 0)
            {
                isCalcoloValid = false;
                messaggioVideo = "Data Fine Beneficio Art2 deve essere successiva alla Data Inzio Beneficio Art2";
            }

            return isCalcoloValid;
        }

        public static void PrevalorizzaArticolo2(GestioneIstruttoria.DatiIstruttoria datiIstruttoria, ref Entity.DatiArticolo2 datiArticolo2)
        {
            //Prevalorizziamo la Data Fine Beneficio Art 2 nel caso in cui in DatiGenerici è presente la ScedenzaRevisioneSanitaria e nel caso in cui non è stata precedentemente salvata alcnuna Data Fine Beneficio Art 2
            //Dati presenti SOLO per fondo PT. Riferimento mail del 10/07/2013 avente il seguente oggetto: ReEng - modifiche fondo PT
            if (datiIstruttoria != null && datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue)
            {
                if (datiArticolo2 == null)
                    datiArticolo2 = new Entity.DatiArticolo2();

                if (!datiArticolo2.DataInzioBeneficioArt2.HasValue && !datiArticolo2.DataFineBeneficioArt2.HasValue)
                    datiArticolo2.DataFineBeneficioArt2 = datiIstruttoria.ScadenzaRevisioneSanitaria.Value;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        #endregion DatiArticolo2

        #region DatiPrivilegiate

        public static void GetDatiPrivilegiateByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, string siglaCategoria, out Entity.DatiPrivilegiate datiPrivilegiate)
        {
            datiPrivilegiate = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST datiFondoFST = contenitore.DatiFondoFS;
                        if (datiFondoFST != null)
                        {
                            datiPrivilegiate = new Entity.DatiPrivilegiate();
                            Utility.ValorizzaOggetti(datiFondoFST, datiPrivilegiate);
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT datiFondoPT = contenitore.DatiFondoPT;
                        if (datiFondoPT != null)
                        {
                            datiPrivilegiate = new Entity.DatiPrivilegiate();
                            Utility.ValorizzaOggetti(datiFondoPT, datiPrivilegiate);
                        }
                        break;
                }
            }
            if (datiPrivilegiate != null && datiPrivilegiate.IsDatiPrivilegiateNull())
                datiPrivilegiate = null;
        }

        public static void StoreDatiPrivilegiate(ref EntityBLCommon.ContenitoreObject contenitore, GestionePensione.DatiPensione datiPensione, DatiPrivilegiate datiPrivilegiate)
        {
            if (datiPrivilegiate == null)
                datiPrivilegiate = new Entity.DatiPrivilegiate();

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            long idFondo = contenitore.IdFondoPensione;

            GestioneFondo.DatiFondoPT datiFondoPTCommon = null;
            GestioneFondo.DatiFondoFST datiFondoFSCommon = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        datiFondoPTCommon = contenitore.DatiFondoPT;
                        if (datiFondoPTCommon == null)
                            datiFondoPTCommon = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiPrivilegiate, datiFondoPTCommon);
                        break;
                    case Utility.TipoFondo.FS:
                        datiFondoFSCommon = contenitore.DatiFondoFS;
                        if (datiFondoFSCommon == null)
                            datiFondoFSCommon = new GestioneFondo.DatiFondoFST();
                        Utility.ValorizzaOggetti(datiPrivilegiate, datiFondoFSCommon);

                        break;
                }
            }

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoPTCommon != null && datiFondoPTCommon.Equals(new GestioneFondo.DatiFondoPT()))
                            {
                                GestioneFondo.EliminaFondoPT(datiPensione.Id);
                                datiFondoPTCommon = null;
                                datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 0;
                            }
                            else
                            {
                                GestioneFondo.SalvaFondoPT(idFondo, datiFondoPTCommon);
                                datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 2;
                            }
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoFSCommon != null && datiFondoFSCommon.Equals(new GestioneFondo.DatiFondoFST()))
                            {
                                GestioneFondo.EliminaFondoFST(datiPensione.Id);
                                datiFondoFSCommon = null;
                                datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 0;
                            }
                            else
                            {
                                GestioneFondo.SalvaFondoFST(idFondo, datiFondoFSCommon);
                                datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 2;
                            }
                            break;
                    }
                }
                if ((datiQuadroMaggiorazioniBenefici.TabBenefici.HasValue && datiQuadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabExCombattente.HasValue && datiQuadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabLegge407.HasValue && datiQuadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && datiQuadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2) ||
                    (datiQuadroMaggiorazioniBenefici.TabArticolo2.HasValue && datiQuadroMaggiorazioniBenefici.TabArticolo2.Value == 2))
                    datiQuadroMaggiorazioniBenefici.Tipo = 2;
                else
                    datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiFondoPT = datiFondoPTCommon;
            contenitore.DatiFondoFS = datiFondoFSCommon;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static void EliminaDatiPrivilegiate(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, contenitore.DatiPensione.SiglaCategoria);

            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoFST datiFondoFS = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        datiFondoPT = contenitore.DatiFondoPT;
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.AssegnoCura = null;
                            datiFondoPT.AssegnoIntegrativo = null;
                            datiFondoPT.Categoria2aInfermita = null;
                            datiFondoPT.CumuloInfermita = null;
                            datiFondoPT.IndennitaAccompagnamentoAggiuntiva = null;
                            datiFondoPT.PrivilegiataSuperinvaliditaIndennita = null;
                            datiFondoPT.IndennitaSpecialeAnnua = null;
                            datiFondoPT.IntegrazioneIndennitaAssistenza = null;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        datiFondoFS = contenitore.DatiFondoFS;
                        if (datiFondoFS != null)
                        {
                            datiFondoFS.AssegnoCura = null;
                            datiFondoFS.AssegnoIntegrativo = null;
                            datiFondoFS.Categoria2aInfermita = null;
                            datiFondoFS.CumuloInfermita = null;
                            datiFondoFS.IndennitaAccompagnamentoAggiuntiva = null;
                            datiFondoFS.PrivilegiataSuperinvaliditaIndennita = null;
                            datiFondoFS.IndennitaSpecialeAnnua = null;
                            datiFondoFS.IntegrazioneIndennitaAssistenza = null;
                        }
                        break;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoPT == null || datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                            {
                                GestioneFondo.EliminaFondoPT(contenitore.DatiPensione.Id);
                                datiFondoPT = null;
                            }
                            else
                                GestioneFondo.SalvaFondoPT(datiFondoPT.IdFondo, datiFondoPT);
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoFS == null || datiFondoFS.Equals(new GestioneFondo.DatiFondoFST()))
                            {
                                GestioneFondo.EliminaFondoFST(contenitore.DatiPensione.Id);
                                datiFondoFS = null;
                            }
                            else
                                GestioneFondo.SalvaFondoFST(datiFondoFS.IdFondo, datiFondoFS);
                            break;
                    }
                }
                datiQuadroMaggiorazioniBenefici.TabPrivilegiate = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;
                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(contenitore.DatiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE --- */
            contenitore.DatiFondoFS = datiFondoFS;
            contenitore.DatiFondoPT = datiFondoPT;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }

        public static bool ControlsDatiPrivilegiate(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, string siglaCategoria, Entity.DatiPrivilegiate datiPrivilegiate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region obbligatorietà

            if (datiPrivilegiate != null && !datiPrivilegiate.AssegnoCura.HasValue)
            {
                messaggioVideo = "Assegno di Cura dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.AssegnoIntegrativo.HasValue)
            {
                messaggioVideo = "Assegno Integrativo dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.Categoria2aInfermita.HasValue)
            {
                messaggioVideo = "Categoria 2° Infermità dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.CumuloInfermita.HasValue)
            {
                messaggioVideo = "cumulo Infermintà dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.HasValue)
            {
                messaggioVideo = "Indennità Accompagnamento Aggiuntiva dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IndennitaSpecialeAnnua.HasValue)
            {
                messaggioVideo = "Indennità Speciale Annua dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IntegrazioneIndennitaAssistenza.HasValue)
            {
                messaggioVideo = "Integrazione Indennità Assistenza dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.HasValue)
            {
                messaggioVideo = "Superinvalidità e Indennità Assistenza dato obbligatorio";
                return false;
            }

            #endregion obbligatorietà

            #region GetData

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            List<GestioneDecodifica.DecPensioniPrivilegiate> lPensioniPrivilegiate = contenitoreDecodifica.ElencoPensioniPrivilegiate;
            lPensioniPrivilegiate = lPensioniPrivilegiate.FindAll(x => x.Fondo == (tipoFondo.HasValue ? tipoFondo.Value.ToString() : string.Empty));

            GestioneDecodifica.DecPensioniPrivilegiate SuperInvalidita = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.Value && x.Posizione == 1);
            GestioneDecodifica.DecPensioniPrivilegiate AssegnoIntegrativo = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.AssegnoIntegrativo.Value && x.Posizione == 2);
            GestioneDecodifica.DecPensioniPrivilegiate IntegrIndennitàAssistenza = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.IntegrazioneIndennitaAssistenza.Value && x.Posizione == 3);
            GestioneDecodifica.DecPensioniPrivilegiate IndAccomAgg = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.Value && x.Posizione == 4);
            GestioneDecodifica.DecPensioniPrivilegiate CumuloInfermità = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.CumuloInfermita.Value && x.Posizione == 5);
            GestioneDecodifica.DecPensioniPrivilegiate Categoria2Infermità = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.Categoria2aInfermita.Value && x.Posizione == 6);
            GestioneDecodifica.DecPensioniPrivilegiate AssegnoCura = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.AssegnoCura.Value && x.Posizione == 7);

            #endregion GetData

            #region SuperInvalidita <--> AssegnoIntegrativo

            if (IndAccomAgg == null || (IndAccomAgg != null && IndAccomAgg.TraduzioneSuGP != '1'))
            {

                if (SuperInvalidita != null && AssegnoIntegrativo != null)
                {
                    if (AssegnoIntegrativo.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '0')
                    {
                        messaggioVideo = "'Super Invalidità' deve essere impostato a No se 'Assegno Integrativo' è valorizzato con SI";
                        return false;
                    }

                    if ((AssegnoIntegrativo.TraduzioneSuGP == '2' || AssegnoIntegrativo.TraduzioneSuGP == '3' || AssegnoIntegrativo.TraduzioneSuGP == '4') && SuperInvalidita.TraduzioneSuGP == '0')
                    {
                        messaggioVideo = "'Assegno Integrativo' deve essere impostato a SI o a NO se 'Super Invalidità' e 'Indennità Accompagnamento Aggiuntiva' sono valorizzate a No";
                        return false;
                    }

                    if (AssegnoIntegrativo.TraduzioneSuGP != '0' && SuperInvalidita.TraduzioneSuGP != '0')
                    {
                        messaggioVideo = "'Assegno Integrativo' deve essere impostato a No se 'Super Invalidità' è valorizzato diversamente da NO";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Dati Pensioni Privilegite mancanti";
                    return false;
                }
            }

            #endregion SuperInvalidita <--> AssegnoIntegrativo

            #region SuperInvalidita <--> Integrazione Indennità Assistenza

            if (SuperInvalidita != null && IntegrIndennitàAssistenza != null)
            {
                if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '1' || IntegrIndennitàAssistenza.TraduzioneSuGP == '2') && SuperInvalidita.TraduzioneSuGP != '2')
                {
                    messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Infermità ascrivibile lettera A/bis n. 1' o 'Infermità ascrivibile lettera A/bis n. 2', allora 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A bis'";
                    return false;
                }

                if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '3' || IntegrIndennitàAssistenza.TraduzioneSuGP == '4') && SuperInvalidita.TraduzioneSuGP != '1')
                {
                    messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Infermità ascrivibile lettera A n. 1-3-4' o 'Infermità ascrivibile lettera A n. 2', allora 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion SuperInvalidita <--> Integrazione Indennità Assistenza

            #region AssegnoIntegrativo <--> Integrazione Indennità Assistenza

            if (IndAccomAgg == null || (IndAccomAgg != null && IndAccomAgg.TraduzioneSuGP != '1'))
            {
                if (AssegnoIntegrativo != null && IntegrIndennitàAssistenza != null)
                {
                    if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '5' || IntegrIndennitàAssistenza.TraduzioneSuGP == '7' || IntegrIndennitàAssistenza.TraduzioneSuGP == '8') &&
                        (AssegnoIntegrativo.TraduzioneSuGP != '2' || AssegnoIntegrativo.TraduzioneSuGP != '3' || AssegnoIntegrativo.TraduzioneSuGP != '4'))
                    {
                        messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Ciechi con mancanza arti sup/inf o sordità bilaterale', 'Infermità ascrivibile lettera A n. 1 - mancanza arto', 'Infermità ascrivibile lettera A n. 1-3-4 e lettera A n. 1/arto' allora 'Assegno Integrativo deve essere valorizzato con 'Sì ind. ass. acc/no ulteriore integr. 2° e 3° accompagnatore' o 'No ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore' o 'Sì ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore'";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Dati Pensioni Privilegite mancanti";
                    return false;
                }
            }

            #endregion AssegnoIntegrativo <--> Integrazione Indennità Assistenza

            #region Indennità Accompagnamento Aggiuntiva <--> SuperInvalidita <--> AssegnoIntegrativo

            if (SuperInvalidita != null && AssegnoIntegrativo != null && IndAccomAgg != null)
            {
                if (IndAccomAgg.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '0' && AssegnoIntegrativo.TraduzioneSuGP != '2' && AssegnoIntegrativo.TraduzioneSuGP != '3' && AssegnoIntegrativo.TraduzioneSuGP != '4')
                {
                    messaggioVideo = "'Indennità Accompagnamento Aggiuntiva':se il valore selezionato è Si, 'Super Invalidità' deve essere valorizzato diversamente da No e 'Assegno Integrativo' deve essere valorizzato con 'Sì ind. ass. acc/no ulteriore integr. 2° e 3° accompagnatore', o 'No ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore' o 'Sì ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion Indennità Accompagnamento Aggiuntiva <--> SuperInvalidita <--> AssegnoIntegrativo

            #region CumuloInfermità <--> SuperInvalidità

            if (CumuloInfermità != null && SuperInvalidita != null)
            {
                if (CumuloInfermità.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '1' && SuperInvalidita.TraduzioneSuGP != '2' && SuperInvalidita.TraduzioneSuGP != '3')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere A-Abis e B', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A' o 'Infermità ascrivibile lettera A Bis' o 'Infermità ascrivibile lettera B'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '2' && SuperInvalidita.TraduzioneSuGP != '1' && SuperInvalidita.TraduzioneSuGP != '2' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere A-Abis e C-D-E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A' o 'Infermità ascrivibile lettera A Bis' o 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '3' && SuperInvalidita.TraduzioneSuGP != '3' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere B e C-D-E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera B' o 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '4' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6' && SuperInvalidita.TraduzioneSuGP != '7' && SuperInvalidita.TraduzioneSuGP != '8' && SuperInvalidita.TraduzioneSuGP != '9')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile Tab.E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E' o 'Infermità ascrivibile lettera F' o 'Infermità ascrivibile lettera G' o 'Infermità ascrivibile lettera H'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion CumuloInfermità <--> SuperInvalidità

            #region Categoria2Infermità <--> AssegnoIntegrativo <--> CumuloInfermità

            if (AssegnoIntegrativo != null && Categoria2Infermità != null && CumuloInfermità != null)
            {
                if (AssegnoIntegrativo.TraduzioneSuGP == '1' && Categoria2Infermità.TraduzioneSuGP != '0' && CumuloInfermità.TraduzioneSuGP != '0')
                {
                    messaggioVideo = "'Categoria 2a Infermità' deve essere impostato a No se 'Assegno Integrativo' è impostato a SI e 'Cumulo Infermità' è valorizzato diversavemte da No";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion Categoria2Infermità <--> AssegnoIntegrativo <--> CumuloInfermità

            #region AssegnoCura <--> All

            if (AssegnoCura != null && SuperInvalidita != null && CumuloInfermità != null && AssegnoIntegrativo != null && IndAccomAgg != null && IntegrIndennitàAssistenza != null && Categoria2Infermità != null)
            {
                if (AssegnoCura.TraduzioneSuGP != '0' && (SuperInvalidita.TraduzioneSuGP != '0' || AssegnoIntegrativo.TraduzioneSuGP != '0' || IndAccomAgg.TraduzioneSuGP != '0' ||
                                                          IntegrIndennitàAssistenza.TraduzioneSuGP != '0' || CumuloInfermità.TraduzioneSuGP != '0' || Categoria2Infermità.TraduzioneSuGP != '0'))
                {
                    messaggioVideo = "'Assegno di Cura' può essere valorizzato diversamente da No se 'Super Invalidità', 'Assegno Integrativo', 'Integrazione Indennità Assistenza', 'Indennità Accompagnamento Aggiuntiva', 'Cumulo Infermintà' e 'Categoria 2° Infermità' sono impostati a No";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion AssegnoCura <--> All

            return true;
        }

        #endregion DatiPrivilegiate

        #region Cross Properties
        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
             Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
             char? codiceSpecificoTraduzioneSuGP, int? settimaneUtiliDiritto, GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi,
             List<GestioneDatiServizioUtile.ServizioUtile> listaServizioUtile, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP, object objectFondoXX, out int? settimane)
        {
            bool? isAnteArmonizzazione;
            bool? isBeneficioArt24Comma15BisFromFELPE = null;
            bool? isBeneficioApePrecociFromFELPE = null;
            bool? isDomandaPensioneInabilita = null;
            bool? isBeneficioVittimeTerrorismo = null;
            bool? isMaggiorazioniForMemo72 = null;
            settimane = null;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();

            isAnteArmonizzazione = GetVisibilitaTabDL407AnteArm(datiPensione, datiDanteCausa, datiPensione.SiglaCategoria);
            isBeneficioArt24Comma15BisFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioArt24Comma15BisFromFELPE : null;
            isBeneficioApePrecociFromFELPE = datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.IsBeneficioApePrecociFromFELPE : null;
            isDomandaPensioneInabilita = Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecificoTraduzioneSuGP);
            if (datiBeneficioVittimeTerrorismo != null)
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione) ||
                                               Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione);
            else
                isBeneficioVittimeTerrorismo = Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) || Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            isMaggiorazioniForMemo72 = ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && codiceSpecificoTraduzioneSuGP != null && codiceSpecificoTraduzioneSuGP == 'F' &&
                                        ((Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.NCertificato.HasValue &&
                                        (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "2" || datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "5")) ||
                                        (!Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("I"))) && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1984, 7, 31)));

            if ((tipoFondo == Utility.TipoFondo.EL || tipoFondo == Utility.TipoFondo.ET || tipoFondo == Utility.TipoFondo.TT || tipoFondo == Utility.TipoFondo.VL) && settimaneUtiliDiritto.HasValue)
                settimane = settimaneUtiliDiritto.Value;
            else if (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || Utility.IsDomandaINPDAP(datiPensione.Gestione))
                settimane = GestioneCrossControls.FS_GetNumeroSettimane(datiPensione, datiRetributivi, listaDatiContributivi, listaServizioUtile, Utility.IsDomandaINPDAP(datiPensione.Gestione), listaRecordDatiFondoINPDAP,
                            tipoFondo, settimaneUtiliDiritto, objectFondoXX);

            lReturn.Add("IsNuovaGestioneDL407ForAnteArm", isAnteArmonizzazione);
            lReturn.Add("IsBeneficioArt24Comma15BisFromFELPE", isBeneficioArt24Comma15BisFromFELPE);
            lReturn.Add("IsBeneficioApePrecociFromFELPE", isBeneficioApePrecociFromFELPE);
            lReturn.Add("IsDomandaPensioneInabilita", isDomandaPensioneInabilita);
            lReturn.Add("IsBeneficioVittimeTerrorismo", isBeneficioVittimeTerrorismo);
            lReturn.Add("IsMaggiorazioniForMemo72", isMaggiorazioniForMemo72);
            return lReturn;
        }

        public static bool GetVisibilitaTabDL407AnteArm(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string siglaCategoria)
        {
            bool ret = false;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                case Utility.TipoFondo.ET:
                case Utility.TipoFondo.TT:
                    ret = Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC);
                    break;
            }
            return ret;
        }
        #endregion Cross Properties

        #region Dati Beneficio Vittime Terrorismo

        public static void GetDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, out Entity.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            datiBeneficioVittimeTerrorismo = new DatiBeneficioVittimeTerrorismo();

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL = contenitore.DatiBeneficioVittimeTerrorismo;
            Utility.ValorizzaOggetti(datiBeneficioVittimeTerrorismoBL, datiBeneficioVittimeTerrorismo);
        }

        public static bool ControlDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiCalcoloContributivo,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL, Utility.TipoCalcolo tipoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string soggettoBeneficiarioTraduzioneSuGP = string.Empty;

            List<GestioneDecodifica.SoggettoBeneficiario> decodificaSoggettoBeneficiario = contenitoreDecodifica.ElencoSoggettoBeneficiario;

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
                datiBeneficioVittimeTerrorismo != null ? datiBeneficioVittimeTerrorismo.TipologiaBeneficio : null, datiBeneficioVittimeTerrorismoBL, tipoCalcolo, out messaggioVideo))
                return false;

            return true;
        }

        public static void StoreDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore, Entity.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivo)
        {
            if (datiBeneficioVittimeTerrorismo == null)
                datiBeneficioVittimeTerrorismo = new Entity.DatiBeneficioVittimeTerrorismo();

            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismoBL = contenitore.DatiBeneficioVittimeTerrorismo;
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = contenitore.DatiQuadroDatiContributivi;

            long? soggettoBeneficiarioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.SoggettoBeneficiario : null;
            long? tipologiaPrestazioneOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaPrestazione : null;
            long? tipologiaBeneficioOld = datiBeneficioVittimeTerrorismoBL != null ? datiBeneficioVittimeTerrorismoBL.TipologiaBeneficio : null;

            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(contenitore.DatiPensione);

            // Verifico se è cambiata la condizione di visibilità di almeno una griglia
            bool isDatiCalcoloVittimeRosso =
                Utility.IsDatiImportoPensioneVittimeVisible(contenitore.DatiPensione, soggettoBeneficiarioOld, tipologiaPrestazioneOld, tipologiaBeneficioOld) != Utility.IsDatiImportoPensioneVittimeVisible(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio);

            bool isDatiCalcoloVittimeNonVisibile = !Utility.IsDatiRetributiviVittimeVisible(contenitore.DatiPensione, datiBeneficioVittimeTerrorismoBL, tipoCalcolo) &&
                                                           !Utility.IsDatiContributiviVittimeVisible(contenitore.DatiPensione, datiBeneficioVittimeTerrorismoBL, tipoCalcolo, datiCalcoloContributivo != null && datiCalcoloContributivo.Exists(x => x.IsQuotaDL214Presente())) &&
                                                           !Utility.IsDatiImportoPensioneVittimeVisible(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo.SoggettoBeneficiario, datiBeneficioVittimeTerrorismo.TipologiaPrestazione, datiBeneficioVittimeTerrorismo.TipologiaBeneficio);

            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiBeneficioVittimeTerrorismoBL == null)
                    datiBeneficioVittimeTerrorismoBL = new GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo();
                Utility.ValorizzaOggetti(datiBeneficioVittimeTerrorismo, datiBeneficioVittimeTerrorismoBL);
                GestioneBeneficioVittimeTerrorismo.SalvaBeneficioVittimeTerrorismo(contenitore.DatiPensione.Id, datiBeneficioVittimeTerrorismoBL);

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
                if (!(((ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(contenitore.DatiPensione)) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(contenitore.DatiPensione)) && !Utility.IsDomandaINPDAP(contenitore.DatiPensione.Gestione)))
                {
                    if (isDatiCalcoloVittimeRosso && !isDatiCalcoloVittimeNonVisibile)
                    {
                        datiQuadroDatiContributivi.TabVittime = 0;
                        GestioneQuadri.SalvaQuadroDatiContributivi(contenitore.DatiPensione.Id, datiQuadroDatiContributivi);
                    }
                    else if (isDatiCalcoloVittimeNonVisibile)
                    {
                        datiQuadroDatiContributivi.TabVittime = null;
                        GestioneQuadri.SalvaQuadroDatiContributivi(contenitore.DatiPensione.Id, datiQuadroDatiContributivi);
                    }
                }
                #endregion Gestione Semafori Dati Calcolo

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(contenitore.DatiPensione.Id, datiQuadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE ---*/
            contenitore.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismoBL;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
            contenitore.DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
        }

        public static void EliminaDatiBeneficioVittimeTerrorismo(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = contenitore.DatiQuadroMaggiorazioniBenefici;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneBeneficioVittimeTerrorismo.EliminaBeneficioVittimeTerrorismoByIdPensione(contenitore.DatiPensione.Id);

                datiQuadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo = 0;
                datiQuadroMaggiorazioniBenefici.Tipo = 1;

                GestioneQuadri.SalvaQuadroMaggiorazioniBenefici(contenitore.DatiPensione.Id, datiQuadroMaggiorazioniBenefici);

                transactionScope.Complete();
            }

            /* ---AGGIORNO I DATI SUL CONTENITORE ---*/
            contenitore.DatiBeneficioVittimeTerrorismo = null;
            contenitore.DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
        }


        #endregion Dati Beneficio Vittime Terrorismo

        #region Decodifica

        public static void GetListaTipoBenefici(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, GestionePensione.DatiPensione datiPensione, byte? codiceSpecifico, out List<Entity.TipoBenefici> listaTipoBenefici)
        {
            listaTipoBenefici = new List<Entity.TipoBenefici>();
            List<Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio> listaTipoBeneficiDB = contenitoreDecodifica.ElencoTipoBenefici;
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

                    // Il beneficio 19 è inseribile solo per le domande Anticipata Flessibile
                    if (!Utility.IsDomandaAnticipataFlessibile(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && beneficioDB.Id == "19")
                        continue;

                    // Il beneficio 24 è inseribile solo per le domande Anticipata Flessibile Legge Bilancio 2024
                    if (!Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && beneficioDB.Id == "24")
                        continue;

                    string descrizione = beneficioDB.Descrizione;

                    if (beneficioDB.Id == "14" && GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.MaggiorazioniBenefici_Benefici_FS.DOPPIO_BENEFICIO_CON_QUOTA100))
                        descrizione = "Pens. ant. quota 100 - art. 14, DL 4/2019 con benef. Amianto";

                    //ENG - Memo 123/2024 aggiornato al 27/03/2025
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

        public static void GetListaCodicePensioniPrivilegiate(out List<Entity.CodicePensioniPrivilegiate> listaCodicePensioniPrivilegiate)
        {
            listaCodicePensioniPrivilegiate = new List<Entity.CodicePensioniPrivilegiate>();
            List<GestioneDecodifica.DecPensioniPrivilegiate> listaDecPensioniPrivilegiateDB = null;
            GestioneDecodifica.GetElencoPensioniPrivilegiate(out listaDecPensioniPrivilegiateDB);
            if (listaDecPensioniPrivilegiateDB != null)
            {
                foreach (GestioneDecodifica.DecPensioniPrivilegiate DecPensioniPrivilegiateDB in listaDecPensioniPrivilegiateDB)
                {
                    Entity.CodicePensioniPrivilegiate codicePensioniPrivilegiate = new Entity.CodicePensioniPrivilegiate();
                    Utility.ValorizzaOggetti(DecPensioniPrivilegiateDB, codicePensioniPrivilegiate);
                    listaCodicePensioniPrivilegiate.Add(codicePensioniPrivilegiate);
                }
            }
        }

        public static void GetListaSoggettoBeneficiario(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<Entity.SoggettoBeneficiario> listaSoggettoBeneficiario)
        {
            listaSoggettoBeneficiario = new List<SoggettoBeneficiario>();
            List<GestioneDecodifica.SoggettoBeneficiario> listaSoggettoBeneficiarioDB = contenitoreDecodifica.ElencoSoggettoBeneficiario;
            if (listaSoggettoBeneficiarioDB != null && listaSoggettoBeneficiarioDB.Count > 0)
            {
                GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = contenitore.DatiBeneficioVittimeTerrorismo;
                foreach (GestioneDecodifica.SoggettoBeneficiario soggettoBeneficiarioDB in listaSoggettoBeneficiarioDB)
                {
                    if (Utility.IsDomandaBeneficioTerrorismoUnder80(contenitore.DatiPensione, datiBeneficioVittimeTerrorismo) && soggettoBeneficiarioDB.TraduzioneSuGP == "V3 ")
                        continue;

                    Entity.SoggettoBeneficiario soggettoBeneficiario = new SoggettoBeneficiario();
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

        #region nested class

        public class DatiMaggiorazioniBenefici
        {
            public DatiMaggiorazioniBenefici()
            { }
            public DatiMaggiorazioniBenefici(long id, long idPensione, byte? codiceCieco, DateTime? decorrenzaMaggiorazioneArt6, DateTime? decorrenzaMaggiorazioneSociale,
                                             string tipoSettimaneBeneficio, long? exCombattente, decimal _RMSSenzaLegge33670QA, decimal _RMSSenzaLegge33670QB,
                                             byte? percentualeMaggiorazioneSenzaLegge33670, DateTime? cessazioneMaggiorazioneSociale, int? nSettimaneBeneficio, DateTime? decorrenzaMaggiorazioneLegge140)
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
                this._DecorrenzaMaggiorazioneLegge140 = decorrenzaMaggiorazioneLegge140;
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
            private int? _DirittoScattiLegge336;
            private short? _SettimaneBeneficioAA;
            private short? _SettimaneBeneficioMM;
            private short? _SettimaneBeneficioGG;
            private DateTime? _DecorrenzaMaggiorazioneLegge140;

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
            public int? DirittoScattiLegge336 { get { return _DirittoScattiLegge336; } set { _DirittoScattiLegge336 = value; } }
            public short? SettimaneBeneficioAA { get { return _SettimaneBeneficioAA; } set { _SettimaneBeneficioAA = value; } }
            public short? SettimaneBeneficioMM { get { return _SettimaneBeneficioMM; } set { _SettimaneBeneficioMM = value; } }
            public short? SettimaneBeneficioGG { get { return _SettimaneBeneficioGG; } set { _SettimaneBeneficioGG = value; } }
            public DateTime? DecorrenzaMaggiorazioneLegge140 { get { return _DecorrenzaMaggiorazioneLegge140; } set { _DecorrenzaMaggiorazioneLegge140 = value; } }
            #endregion public properties

        }



        public static bool IsMaggiorazioniBeneficiNull(DatiMaggiorazioniBenefici maggiorazioniBenefici)
        {
            if (!maggiorazioniBenefici.CodiceCieco.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.TipoSettimaneBeneficio) &&
                !maggiorazioniBenefici.ExCombattente.HasValue && !maggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue &&
                !maggiorazioniBenefici.RMSSenzaLegge33670QB.HasValue && !maggiorazioniBenefici.PercentualeMaggiorazioneSenzaLegge33670.HasValue && !maggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140.HasValue)
            {
                return true;
            }
            else
                return false;
        }



        public class DatiServizioUtileDL407 : GestioneContrib.DatiServizioUtile
        {
            public decimal? RetribPensSL336 { get; set; }
        }

        #endregion nested class

    }
}
