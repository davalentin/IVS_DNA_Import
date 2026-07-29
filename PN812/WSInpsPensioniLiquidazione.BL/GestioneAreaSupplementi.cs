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

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaSupplementi
    {
        #region Supplementi

        public static void EliminaSupplementiByIdPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                //ENG - MEMO 50/2023
                if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" &&
                    tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001")
                    GestioneSupplementi.DeleteSupplementi(datiPensione.Id, false);
                else
                    GestioneSupplementi.DeleteSupplementi(datiPensione.Id, true);

                GestioneSupplementi.EliminaDatiSupplementiBase(datiPensione.Id);
                GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdPensione(datiPensione.Id);

                if ((Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.AGO &&
                    Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiPensione.NaturaPensione, isRiaperturaDomanda, null) == Utility.TipoQuadro.Obbligatorio) ||
                    (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Utility.TipoAppartenenza.CI && ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" &&
                    Utility.IsRicostituzione_Supplemento(datiPensione) && !Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                {
                    datiQuadroSupplementi.Tipo = 2;
                    datiQuadroSupplementi.TabSupplementi = 0;
                }
                else
                {
                    datiQuadroSupplementi.Tipo = 1;
                    datiQuadroSupplementi.TabSupplementi = 1;
                }
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                GestioneBypassControllo.SetUnlock(datiPensione.NDomus, typeof(GestioneBypassControllo.NomeBypass.Supplementi_Supplementi_AGO));

                transactionScope.Complete();
            }
        }

        public static bool ControlSupplementiPerRicContributivaPura(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            BLCommon.GestioneSupplementi.GetSupplementiNoStoricoByIdPensione(datiPensione.Id, out listaSupplementi);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            bool isTipoCalcoloModificato = false;
            if (datiPensione.TipoCalcolo.HasValue && datiIstruttoria != null && datiIstruttoria.TipoCalcoloPrecedente.HasValue && datiPensione.TipoCalcolo != datiIstruttoria.TipoCalcoloPrecedente)
                isTipoCalcoloModificato = true;

            if (listaSupplementi.Exists(x => x.IsFromPrelievo == true) && !isTipoCalcoloModificato)
            {
                messaggioVideo = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento.";
                return false;
            }

            return true;
        }

        #endregion Supplementi

        #region DatiSupplementi

        public static void GetDatiSupplementiByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> Lsupplemento)
        {
            Lsupplemento = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            BLCommon.GestioneSupplementi.GetSupplementiByIdPensione(idPensione, out Lsupplemento);
        }

        public static void GetDatiSupplementiNoStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> LsupplementoNoStorico)
        {
            LsupplementoNoStorico = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            BLCommon.GestioneSupplementi.GetSupplementiNoStoricoByIdPensione(idPensione, out LsupplementoNoStorico);
        }

        public static void GetDatiSupplementiStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> LsupplementoStorico)
        {
            LsupplementoStorico = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            BLCommon.GestioneSupplementi.GetSupplementiStoricoByIdPensione(idPensione, out LsupplementoStorico);
        }

        public static void StoreDatiSupplementiByDatiPensione(GestionePensione.DatiPensione datiPensione, List<BLCommon.Entity.DatiSupplementi> Listsupplemento, BLCommon.Entity.SupplementiBase supplementoBase,
            BLCommon.Entity.IntegrazioneArt11 integrazioneArt11)
        {
            if (supplementoBase == null)
                supplementoBase = new INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase();

            if (integrazioneArt11 == null)
                integrazioneArt11 = new BLCommon.Entity.IntegrazioneArt11();

            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //ENG - MEMO 50/2023
                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001")
                {
                    GestioneSupplementi.DeleteSupplementi(datiPensione.Id, false);
                    StoreDatiSupplementiStoricoPrivateByIdPensione(datiPensione.Id, Listsupplemento);
                }
                else
                    StoreDatiSupplementiPrivateByIdPensione(datiPensione.Id, Listsupplemento);

                StoreDatiSupplementoBasePrivateByIdPensione(datiPensione.Id, supplementoBase);
                StoreDatiIntegrazioneArt11PrivateByIdPensione(datiPensione.Id, integrazioneArt11);

                if (Listsupplemento.Count == 0 && supplementoBase.IsSupplementiBaseNull() && integrazioneArt11.IsIntegrazioneArt11Null())
                {
                    if (Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiPensione.NaturaPensione, isRiaperturaDomanda, null) == Utility.TipoQuadro.Facoltativo)
                    {
                        datiQuadroSupplementi.Tipo = 1;
                        datiQuadroSupplementi.TabSupplementi = 1;
                    }
                    else if (Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiPensione.NaturaPensione, isRiaperturaDomanda, null) == Utility.TipoQuadro.Obbligatorio)
                    {
                        datiQuadroSupplementi.Tipo = 2;
                        datiQuadroSupplementi.TabSupplementi = 0;
                    }
                }
                else
                    datiQuadroSupplementi.TabSupplementi = 2;

                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiSupplementiPrivateByIdPensione(long idPensione, List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> Listsupplemento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                BLCommon.GestioneSupplementi.SalvaDatiSupplementi(idPensione, Listsupplemento);
                transactionScope.Complete();
            }
        }

        private static void StoreDatiSupplementiStoricoPrivateByIdPensione(long idPensione, List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> Listsupplemento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                BLCommon.GestioneSupplementi.SalvaDatiSupplementiStorico(idPensione, Listsupplemento);
                transactionScope.Complete();
            }
        }

        public static bool ControlDatiSupplementiByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, BLCommon.GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            List<BLCommon.Entity.DatiSupplementi> datiSupplementi, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiSupplementi != null)
            {
                if (datiPensione == null)
                {
                    messaggioVideo = "Dati Pensione non valorizzati";
                    return false;
                }

                if (!datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    messaggioVideo = "Campo 'Decorrenza Pensione' obbligatorio in Titolare / Anagrafica";
                    return false;
                }

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                List<DatiSupplementi> datiSupplementiFiltrati = datiSupplementi.Where(x => !(x.TipoSupplemento.Equals('R') && x.QuotaSupplemento.Equals('A') && x.CodGestioneSupplemento.Equals("1"))).ToList();
                if (tipoAppartenenza.HasValue)
                {
                    switch (tipoAppartenenza.Value)
                    {
                        case Utility.TipoAppartenenza.FS:
                            return ControlDatiSupplementiFSByDatiPensione(datiPensione, datiSupplementiFiltrati, datiSupplementi, out messaggioVideo);

                        case Utility.TipoAppartenenza.AGO:
                            return ControlDatiSupplementiAgoByDatiPensione(datiPensione, datiDanteCausa, datiAnagrafici, datiSupplementi, isRiaperturaDomanda, out messaggioVideo);

                        case Utility.TipoAppartenenza.CI:
                            return ControlDatiSupplementiCIByDatiPensione(datiPensione, datiSupplementi, out messaggioVideo);

                        default:
                            messaggioVideo = "Tipo appartenenza mancante.";
                            return false;
                    }
                }
                else
                {
                    messaggioVideo = "Tipo appartenenza mancante.";
                    return false;
                }
            }
            else
                return true;
        }

        private static bool ControlDatiSupplementiFSByDatiPensione(GestionePensione.DatiPensione datiPensione, List<BLCommon.Entity.DatiSupplementi> datiSupplementi, List<BLCommon.Entity.DatiSupplementi> datiSupplementiNonFIltrati, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);

            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo = null;
            Liquidazione.BLCommon.GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out listaRecordFondo);

            GestioneFondo.DatiFondo datiFondo = null;
            GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            //ENG - MEMO 50/2023
            GestioneControlliDinamici.ControlloDinamico ctrl_Memo50 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrl_Memo50);

            char? codiceSpecificoTraduzioneSuGP = null;
            if (datiFondo != null && datiFondo.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondo.CodiceSpecifico.Value);
                    if (codice != null)
                        codiceSpecificoTraduzioneSuGP = codice.TraduzioneGp;
                }
            }

            if (datiSupplementi != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi s in datiSupplementi)
                {
                    if (!s.DecorrenzaSupplemento.HasValue)
                    {
                        messaggioVideo = "Campo 'Decorrenza' obbligatorio";
                        return false;
                    }

                    if (!s.NSettimaneSupplemento.HasValue)
                    {
                        messaggioVideo = "Campo 'Settimane' obbligatorio";
                        return false;
                    }

                    if (!s.QuotaSupplemento.HasValue)
                    {
                        messaggioVideo = "Campo 'Quota' obbligatorio";
                        return false;
                    }

                    if (!(tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.ET || tipoFondo.Value == Utility.TipoFondo.PM || tipoFondo.Value == Utility.TipoFondo.ES)))
                    {
                        if (s.CodGestioneSupplemento != string.Empty && s.CodGestioneSupplemento.Trim() != "1")
                        {
                            messaggioVideo = "Valore del Campo 'Tipo' errato";
                            return false;
                        }
                        if (s.QuotaSupplemento.Value.ToString().ToUpperInvariant() != "B")
                        {
                            messaggioVideo = "Valore del Campo 'Quota' errato";
                            return false;
                        }
                    }

                    if(tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ES)
                    {
                        if (s.CodGestioneSupplemento != string.Empty && s.CodGestioneSupplemento.Trim() != "1" && s.CodGestioneSupplemento.Trim() != "2" && s.CodGestioneSupplemento.Trim() != "3" && s.CodGestioneSupplemento.Trim() != "4" )
                        {
                            messaggioVideo = "Valore del Campo 'Tipo' errato";
                            return false;
                        }

                        if (s.CodGestioneSupplemento.Trim() == "1" && s.QuotaSupplemento.Value.ToString().ToUpperInvariant() != "B" && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1993, 1, 1)))
                        {
                            messaggioVideo = "Valore del Campo 'Quota' errato";
                            return false;
                        }
                        else if (s.CodGestioneSupplemento.Trim() != "1" && s.QuotaSupplemento.Value.ToString().ToUpperInvariant() != "A" && s.QuotaSupplemento.Value.ToString().ToUpperInvariant() != "B")
                        {
                            messaggioVideo = "Valore del Campo 'Quota' errato";
                            return false;
                        }
                    }

                    if (!s.TipoSupplemento.HasValue)
                    {
                        messaggioVideo = "Campo 'C/R' obbligatorio";
                        return false;
                    }

                    if (s.TipoSupplemento.HasValue && s.TipoSupplemento.Value == 'R' && (!s.RMSSupplemento.HasValue || s.RMSSupplemento.Value == 0M))
                    {
                        messaggioVideo = "Campo 'RMS' obbligatorio per supplementi retributivi";
                        return false;
                    }

                    if (s.TipoSupplemento.HasValue && s.TipoSupplemento.Value == 'R' && s.MontanteSupplemento.HasValue && s.MontanteSupplemento.Value > 0M)
                    {
                        messaggioVideo = "Campo 'Montante' non compatibile per supplementi retributivi";
                        return false;
                    }

                    if (s.TipoSupplemento.HasValue && (s.TipoSupplemento.Value == 'C' || s.TipoSupplemento.Value == 'D') && (!s.MontanteSupplemento.HasValue || s.MontanteSupplemento.Value == 0M))
                    {
                        messaggioVideo = "Campo 'Montante' obbligatorio per supplementi contributivi";
                        return false;
                    }

                    if (s.TipoSupplemento.HasValue && (s.TipoSupplemento.Value == 'C' || s.TipoSupplemento.Value == 'D') && s.RMSSupplemento.HasValue && s.RMSSupplemento.Value > 0M)
                    {
                        messaggioVideo = "Campo 'RMS' non compatibile per supplementi contributivi";
                        return false;
                    }

                    //controllo quota D
                    if (s.TipoSupplemento.HasValue && s.TipoSupplemento.Value == 'D' && s.DecorrenzaSupplemento.HasValue &&
                        DateTime.Compare(s.DecorrenzaSupplemento.Value.Date, new DateTime(2012, 01, 01).Date) < 0)
                    {
                        messaggioVideo = "In presenza di quota D e' possibile inserire una decorrenza non inferiore a Gennaio 2012";
                        return false;
                    }

                    if (datiSupplementi.Where(x => x.CodGestioneSupplemento == s.CodGestioneSupplemento && x.TipoSupplemento == s.TipoSupplemento && x.QuotaSupplemento == s.QuotaSupplemento &&
                        x.DecorrenzaSupplemento == s.DecorrenzaSupplemento).Count() > 1)
                    {
                        messaggioVideo = "Non possono esistere 2 registrazioni con la stessa gestione, decorrenza e stesso tipo";
                        return false;
                    }
                }

                //ENG - Per le RIC per supplemento (GPT 0031/0102-0302-0402/0001) deve esserci necessariamente un supplemento non proveniente dal prelievo
                if (ctrl_Memo50 != null && !String.IsNullOrEmpty(ctrl_Memo50.ValoreControllo) && ctrl_Memo50.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsRicostituzione_Supplemento(datiPensione) && datiPensione.Tipo == "0001")
                    {
                        int numeroSupplementiNoPrelievo = datiSupplementi.Count(x => !x.IsFromPrelievo);

                        if (numeroSupplementiNoPrelievo == 0)
                        {
                            messaggioVideo = "Per le Ricostituzioni per supplemento è necessario inserire almeno un supplemento";
                            return false;
                        }
                    }
                }

                if (Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datiPensione)) 
                {
                    if (datiSupplementiNonFIltrati.Where(x => (x.TipoSupplemento.Equals('R') && x.QuotaSupplemento.Equals('A') && x.CodGestioneSupplemento.Equals("1"))).Count() > 1) 
                    {
                        messaggioVideo = "Non è possibile inserire più di un supplemento con C/R = 'R', tipo = 1 e quota = 'A'";
                        return false;
                    }
                }

                if (!(tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.ET && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)))
                {
                    if (!GestioneCrossControls.FS_VerificaDecorrenzaSupplementoDecorrenzaPensione(datiSupplementi, Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null)))
                    {
                        messaggioVideo = "La data 'Decorrenza Supplementi' è anteriore alla data 'Decorrenza Pensione'";
                        return false;
                    }

                    //Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    //if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
                    //{
                    if (!GestioneCrossControls.FS_VerificaSupplementiWithBonus(datiSupplementi, listaRecordFondo, datiFondo.AttribuzioneBonus, codiceSpecificoTraduzioneSuGP, tipoFondo))
                    {
                        messaggioVideo = "Non è possibile inserire Supplementi in mancanza del Bonus.";
                        return false;
                    }

                    if (!GestioneCrossControls.FS_VerificaSupplementiDecorrenza(datiSupplementi, Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null),
                        datiPensione, datiDanteCausa, out messaggioVideo))
                        return false;
                }

                if (!GestioneCrossControls.FS_VerificaSupplementiCodiceSpecificoCodiceGestione(datiPensione, datiSupplementi, datiFondo.AttribuzioneBonus, codiceSpecificoTraduzioneSuGP, tipoFondo, out messaggioVideo))
                    return false;
                //}

                if (!GestioneCrossControls.FS_VerificaSupplementiDecorrenzaRicContributive(datiSupplementi, datiPensione, out messaggioVideo))
                    return false;
            }
            return true;
        }

        private static bool ControlDatiSupplementiAgoByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, BLCommon.GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            List<BLCommon.Entity.DatiSupplementi> datiSupplementi, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                return true;

            if (datiSupplementi != null && datiSupplementi.Count > 0)
            {
                if (Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, datiDanteCausa, isRiaperturaDomanda) == false)
                {
                    if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaVOAUT_IOAUT(datiPensione.SiglaCategoria))
                    {
                        if (datiSupplementi.Exists(x => x.TipoSupplemento == 'R'))
                        {
                            messaggioVideo = "Non è possibile acquisire Supplementi di tipo Retributivo.";
                            return false;
                        }
                    }

                    #region Controlli comuni Retributivo - Contributivo
                    if (!GestioneCrossControls.AGO_VerificaDecorrenzaSupplemento(datiSupplementi, datiPensione, out messaggioVideo))
                        return false;
                    if (!GestioneCrossControls.AGO_VerificaSupplementi(datiSupplementi, datiPensione, out messaggioVideo))
                        return false;

                    #endregion Controlli comuni Retributivo - Contributivo

                    if (!GestioneCrossControls.AGO_VerificaSupplementiDecorrenza(datiSupplementi, datiPensione, datiDanteCausa, datiAnagrafici,
                    Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null), isRiaperturaDomanda,
                    true, out messaggioVideo))
                        return false;
                }
                else
                {
                    if (!GestioneCrossControls.AGO_VerificaSupplementiAnte96(datiSupplementi, datiPensione, datiDanteCausa,
                    Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null), isRiaperturaDomanda,
                    true, out messaggioVideo))
                        return false;
                }
            }

            return true;
        }

        private static bool ControlDatiSupplementiCIByDatiPensione(GestionePensione.DatiPensione datiPensione, List<BLCommon.Entity.DatiSupplementi> datiSupplementi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiSupplementi != null && datiSupplementi.Count > 0)
            {
                if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione))
                {
                    if (datiSupplementi.Exists(x => x.TipoSupplemento == 'R'))
                    {
                        messaggioVideo = "Non è possibile acquisire Supplementi di tipo Retributivo.";
                        return false;
                    }
                }

                #region Controlli comuni Retributivo - Contributivo
                if (!GestioneCrossControls.CI_VerificaSupplementi(datiSupplementi, datiPensione, out messaggioVideo))
                    return false;
                #endregion Controlli comuni Retributivo - Contributivo
            }
            return true;
        }

        #endregion DatiSupplementi

        #region Supplemento Base

        public static void GetDatiSupplementiBaseByIdPensione(long idPensione, out BLCommon.Entity.SupplementiBase supplementoBase)
        {
            INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase supplementoBaseBL = new INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase();
            BLCommon.GestioneSupplementi.GetDatiSupplementiBaseByIdPensione(idPensione, out supplementoBaseBL);
            supplementoBase = new INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase();
            Utility.ValorizzaOggetti(supplementoBaseBL, supplementoBase);
        }

        private static void StoreDatiSupplementoBasePrivateByIdPensione(long idPensione, BLCommon.Entity.SupplementiBase supplementoBase)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (supplementoBase.Equals(new BLCommon.Entity.SupplementiBase()))
                    GestioneSupplementi.EliminaDatiSupplementiBase(idPensione);
                else
                    GestioneSupplementi.SalvaDatiSupplementiBase(idPensione, supplementoBase);

                transactionScope.Complete();
            }
        }

        public static bool ControlDatiSupplementiBaseByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.SupplementiBase datiSupplementiBase, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
            {
                if (datiSupplementiBase != null && !GestioneCrossControls.AGO_VerificaSupplementiBase(datiSupplementiBase.RenditaFacoltativaOrdinaria,
                    datiSupplementiBase.RenditaFacoltativaConvenzionale, datiPensione, out messaggioVideo))
                    return false;


            }

            return true;
        }

        #endregion Supplemento Base

        #region Integrazione art11

        public static void GetDatiIntegrazioneArt11ByIdPensione(long idPensione, out BLCommon.Entity.IntegrazioneArt11 integrazioneArt11)
        {
            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11BL = new GestioneIntegrazioneArt11.IntegrazioneArt11();
            BLCommon.GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(idPensione, out integrazioneArt11BL);
            integrazioneArt11 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11BL, integrazioneArt11);
        }

        public static void GetDatiIntegrazioneArt11ByIdSuppRecordEnpals(long idRecord, out BLCommon.Entity.IntegrazioneArt11 integrazioneArt11)
        {
            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11BL = new GestioneIntegrazioneArt11.IntegrazioneArt11();
            BLCommon.GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdRecord(idRecord, out integrazioneArt11BL);
            integrazioneArt11 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11BL, integrazioneArt11);
        }

        private static void StoreDatiIntegrazioneArt11PrivateByIdPensione(long idPensione, INPS.Pensioni.Liquidazione.BLCommon.Entity.IntegrazioneArt11 integrazioneArt11)
        {
            BLCommon.GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11BL = new GestioneIntegrazioneArt11.IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11, integrazioneArt11BL);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (integrazioneArt11.Equals(new BLCommon.Entity.IntegrazioneArt11()))
                    GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdPensione(idPensione);
                else
                    GestioneIntegrazioneArt11.SalvaIntegrazioneArt11(idPensione, integrazioneArt11BL);

                transactionScope.Complete();
            }
        }

        public static bool ControlDatiIntegrazioneArt11ByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.IntegrazioneArt11 datiIntegrazioneArt11,
            List<BLCommon.Entity.DatiSupplementi> listaSupplementi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiIntegrazioneArt11 != null && !Utility.IsDomandaRiliquidazioneIndiretta(datiPensione))
            {
                if (datiPensione == null)
                {
                    messaggioVideo = "Dati Pensione non valorizzati";
                    return false;
                }

                if (!datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    messaggioVideo = "Campo 'Decorrenza Pensione' obbligatorio in Titolare / Anagrafica";
                    return false;
                }

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                if (tipoAppartenenza.HasValue)
                {
                    switch (tipoAppartenenza.Value)
                    {
                        case Utility.TipoAppartenenza.AGO:
                            if (!GestioneCrossControls.AGO_VerificaIntArt11(datiIntegrazioneArt11, listaSupplementi, datiPensione, out messaggioVideo))
                                return false;
                            break;
                        default:
                            messaggioVideo = "Tipo appartenenza mancante.";
                            return false;
                    }
                }
                else
                {
                    messaggioVideo = "Tipo appartenenza mancante.";
                    return false;
                }
            }

            return true;
        }
        #endregion Integrazione art11

        #region Legge 407
        #endregion Legge 407

        #region Decodifica

        public static void GetListaTipoSupplementiByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out List<BLCommon.Entity.TipoSupplementi> listaTipoSupplementi)
        {
            listaTipoSupplementi = new List<BLCommon.Entity.TipoSupplementi>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoSupplementi> listaTipoSupplementiDB = null;
            GestioneDecodifica.GetTipoSupplementi(out listaTipoSupplementiDB);
            if (listaTipoSupplementiDB != null)
            {
                if (datiPensione != null)
                {
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    if (tipoAppartenenza != null)
                    {
                        switch (tipoAppartenenza)
                        {
                            case Utility.TipoAppartenenza.AGO:
                                listaTipoSupplementiDB = listaTipoSupplementiDB.FindAll(x => x.Tipologia == "AGO").ToList<GestioneDecodifica.TipoSupplementi>();
                                if (Utility.IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa) &&
                                    !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim() == "SO")
                                    //ENG - Pensioni di categoria SO e filtro BNS non deve essere rimosso il Codice Gestione 1
                                    if (!string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) && datiPensione.CodiceTipoRichiesta == "56")
                                        listaTipoSupplementiDB.RemoveAll(x => new List<char> { 'P', 'G', 'A' }.Contains(x.TraduzioneSuGP.GetValueOrDefault()));
                                    else
                                        listaTipoSupplementiDB.RemoveAll(x => new List<char> { '1', 'P', 'G', 'A' }.Contains(x.TraduzioneSuGP.GetValueOrDefault()));
                                else if (Utility.IsDomandaAUT(datiPensione))
                                {
                                    if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Titolare_Anagrafica_AGO.APPLICAZIONE_SENTENZA_VOAUT))
                                        listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP != 'G' && x.TraduzioneSuGP != '4');
                                    else
                                        listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP != 'G');
                                }
                                else if (Utility.IsDomandaBancari(datiPensione.SiglaCategoria))
                                    //listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP != '1');
                                    //revisione 1.5
                                    listaTipoSupplementiDB.RemoveAll(x => !(new List<char> { '1', '2', '3', '4', 'M', 'N', 'I', 'H' }.Contains(x.TraduzioneSuGP.GetValueOrDefault())));
                                else if (!Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == 'A');
                                else
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == 'G' || x.TraduzioneSuGP == 'P');

                                if (!Utility.IsDomandaBancari(datiPensione.SiglaCategoria))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == 'H');
                                //Rimozione codice 7 per domande non minatori
                                if (!Utility.IsDomandaMIN(datiPensione.SiglaCategoria))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == '7');
                                if (!Utility.IsDomandaAUT(datiPensione))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == 'G');
                                if (Utility.IsDomandaRiliquidazioneIndiretta(datiPensione) && Utility.IsDomandaSO(datiPensione.SiglaCategoria))
                                {
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP != '2' && x.TraduzioneSuGP != '3' && x.TraduzioneSuGP != '4');
                                } 
                                break;
                            case Utility.TipoAppartenenza.CI:
                                listaTipoSupplementiDB = listaTipoSupplementiDB.FindAll(x => x.Tipologia == "CI").ToList<GestioneDecodifica.TipoSupplementi>();
                                break;
                            case Utility.TipoAppartenenza.FS:
                                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                                listaTipoSupplementiDB = listaTipoSupplementiDB.FindAll(x => x.Tipologia == "FS" && x.Fondo == tipoFondo.ToString()).ToList<GestioneDecodifica.TipoSupplementi>();
                                if (tipoFondo == Utility.TipoFondo.ET && Utility.IsDomandaPensioneIndiretta(datiPensione))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == '1');
                                if (tipoFondo == Utility.TipoFondo.ET && !Utility.IsRicostituzione(datiPensione.Gruppo))
                                    listaTipoSupplementiDB.RemoveAll(x => x.TraduzioneSuGP == 'I' || x.TraduzioneSuGP == 'M' || x.TraduzioneSuGP == 'N');
                                break;
                        }
                    }
                }
                if (listaTipoSupplementiDB != null && listaTipoSupplementiDB.Count > 0)
                {
                    foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoSupplementi tipoSupplementiDB in listaTipoSupplementiDB)
                    {
                        BLCommon.Entity.TipoSupplementi tipoSupplementi = new BLCommon.Entity.TipoSupplementi();
                        tipoSupplementi.Id = tipoSupplementiDB.TraduzioneSuGP.HasValue ? tipoSupplementiDB.TraduzioneSuGP.Value.ToString() : "";
                        tipoSupplementi.Descrizione = tipoSupplementiDB.Descrizione;
                        listaTipoSupplementi.Add(tipoSupplementi);
                    }
                }
            }
        }
        #endregion Decodifica

        #region Supplementi Enpals

        public static void GetDatiSuppRecordEnpalsByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSuppRecordENPALS> lstSuppRecordEnpals)
        {
            BLCommon.GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(idPensione, out lstSuppRecordEnpals);
        }

        public static void StoreDatiSuppRecordEnpals(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordEnpals, List<BLCommon.Entity.DatiSuppRecordENPALS> lstSuppRecordEnpalsDb, out long? idRecord)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            if (dSuppRecordEnpals.IdSuppRecordEnpals == 0)
            {
                dSuppRecordEnpals.DettaglioSalvato = false;
                dSuppRecordEnpals.IsFromSas = false;
                dSuppRecordEnpals.IsFromGP = false;
            }

            //per convenzione quando stiamo inserendo un nuovo record verrà passato idRecord = 0           
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //operation
                GestioneSupplementi.SalvaDatiSuppRecordEnpals(datiPensione.Id, dSuppRecordEnpals, out idRecord);
                lstSuppRecordEnpalsDb.Add(dSuppRecordEnpals);
                //gestione semafori
                ManageSemaforiEnpals(lstSuppRecordEnpalsDb, ref datiQuadroSupplementi);
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                transactionScope.Complete();
            }
        }

        public static bool ControlsStoreDatiSuppRecordEnpals(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordEnpals,
            List<BLCommon.Entity.DatiSuppRecordENPALS> lstRecordSuppStored, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dSuppRecordEnpals != null)
            {
                if (!dSuppRecordEnpals.Decorrenza.HasValue)
                {
                    messaggioVideo = "La decorrenza è un dato obbligatorio.";
                    return false;
                }

                if (!dSuppRecordEnpals.InizioSupplemento.HasValue)
                {
                    messaggioVideo = "La inizio supplemento è un dato obbligatorio.";
                    return false;
                }
                else
                {
                    if (dSuppRecordEnpals.FineSupplemento.HasValue && dSuppRecordEnpals.InizioSupplemento > dSuppRecordEnpals.FineSupplemento)
                    {
                        messaggioVideo = string.Format("L'inizio supplemento ({0}) deve essere inferiore al fine supplemento ({1})", dSuppRecordEnpals.InizioSupplemento.Value.ToString("dd/MM/yyyy"),
                            dSuppRecordEnpals.FineSupplemento.Value.ToString("dd/MM/yyyy"));
                        return false;
                    }
                }

                if (!GestioneCrossControls.AGO_VerificaDecorrenzaSupplementoDecorrenzaPensioneENPALS(dSuppRecordEnpals, datiPensione))
                {
                    messaggioVideo = "La decorrenza deve essere pari alla decorrenza della pensione del titolare superstite.";
                    return false;
                }

                if (lstRecordSuppStored != null && lstRecordSuppStored.Count > 0)
                {
                    DateTime? decorrenzaRecord = dSuppRecordEnpals.Decorrenza;
                    long idRecord = dSuppRecordEnpals.IdSuppRecordEnpals;
                    List<BLCommon.Entity.DatiSuppRecordENPALS> lstApp = lstRecordSuppStored.Where(x => x.IdSuppRecordEnpals != idRecord).ToList();
                    //La decorrenza dei record inseriti deve essere sempre diversa.
                    if (lstApp.Exists(x => (x.Decorrenza == decorrenzaRecord)))
                    {
                        messaggioVideo = string.Format("Non è possibile inserire due record con la stessa decorrenza ({0:MM/yyyy})", decorrenzaRecord);
                        return false;
                    }
                    //Verifico che ogni supplemento abbia una distanza di 2 anni dalla decorrenza degli altri anni
                    foreach (var elem in lstApp)
                    {
                        int days = (decorrenzaRecord.Value - elem.Decorrenza.Value).Days;
                        decimal years = days / (decimal)365;
                        if (Math.Abs(years) < 2)
                        {
                            messaggioVideo = string.Format("La decorrenza del supplemento inserita ({0:MM/yyyy}) è incompatibile con la decorrenza ({1:MM/yyyy}) di un altro supplemento. E' possibile richiedere un supplemento ogni 2 anni.", decorrenzaRecord, elem.Decorrenza);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void EliminaDatiSuppRecordEnpalsByIdRecord(GestionePensione.DatiPensione datiPensione, long idRecord)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            List<BLCommon.Entity.DatiSuppRecordENPALS> lstStoredSuppRecordEnpals = null;
            GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out lstStoredSuppRecordEnpals);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                //eliminazione
                GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdSuppRecordENPALS(idRecord);
                GestioneSupplementi.EliminaDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord);
                GestioneSupplementi.EliminaDatiSuppRecordEnpalsByIdRecord(idRecord);
                //gestione semafori
                lstStoredSuppRecordEnpals = lstStoredSuppRecordEnpals.Where(x => x.IdSuppRecordEnpals != idRecord).ToList();
                ManageSemaforiEnpals(lstStoredSuppRecordEnpals, ref datiQuadroSupplementi);
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiSuppRecordEnpalsByIdPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            List<BLCommon.Entity.DatiSuppRecordENPALS> lstStoredSuppRecordEnpals = null;
            GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out lstStoredSuppRecordEnpals);

            List<BLCommon.Entity.DatiSupplementiENPALS> lstStoredSuppEnpals = null;
            GestioneSupplementi.GetDatiSupplementiEnpalsByIdPensione(datiPensione.Id, out lstStoredSuppEnpals);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                //eliminazione
                List<BLCommon.Entity.DatiSuppRecordENPALS> lstFromSas = new List<BLCommon.Entity.DatiSuppRecordENPALS>();
                foreach (var elem in lstStoredSuppRecordEnpals)
                {
                    long idRecord = elem.IdSuppRecordEnpals;

                    if (!elem.IsFromSas)
                    {
                        //eliminazione
                        GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdSuppRecordENPALS(idRecord);
                        GestioneSupplementi.EliminaDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord);
                        GestioneSupplementi.EliminaDatiSuppRecordEnpalsByIdRecord(idRecord);
                    }
                    else
                    {
                        long? app;
                        elem.DettaglioSalvato = false;
                        elem.RenditaFacoltativaConvenzionale = null;
                        elem.RenditaFacoltativaOrdinaria = null;
                        GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdSuppRecordENPALS(idRecord);
                        GestioneSupplementi.SalvaDatiSuppRecordEnpals(datiPensione.Id, elem, out app);

                        lstFromSas.Add(elem);
                    }
                }
                //gestione semafori
                ManageSemaforiEnpals(lstFromSas, ref datiQuadroSupplementi);
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                transactionScope.Complete();
            }
        }

        public static void GetDatiSupplementiDettaglioEnpalsByIdRecord(long idRecord, out BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordEnpals, out List<BLCommon.Entity.DatiSupplementiENPALS> lstSuppEnapls, out BLCommon.Entity.IntegrazioneArt11 dIntegrArt11)
        {
            BLCommon.GestioneSupplementi.GetDatiSuppRecordEnpalsyIdRecord(idRecord, out dSuppRecordEnpals);
            GetDatiIntegrazioneArt11ByIdSuppRecordEnpals(idRecord, out dIntegrArt11);
            BLCommon.GestioneSupplementi.GetDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord, out lstSuppEnapls);
            if (lstSuppEnapls != null && lstSuppEnapls.Count > 0)
            {
                //valorizzo la lista dei dati retributivi/contributivi con i dati del record 
                foreach (var elem in lstSuppEnapls)
                {
                    elem.IdSuppRecordENPALS = dSuppRecordEnpals.IdSuppRecordEnpals;
                    elem.IsFromSAS = dSuppRecordEnpals.IsFromSas;
                    elem.IsFromGP = dSuppRecordEnpals.IsFromGP;
                    elem.Decorrenza = dSuppRecordEnpals.Decorrenza;
                }
            }
        }

        public static void StoreDatiSupplementiDettaglioEnpals(GestionePensione.DatiPensione datiPensione, ref BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordEnpals, List<BLCommon.Entity.DatiSupplementiENPALS> lstSuppEnapls, BLCommon.Entity.IntegrazioneArt11 dIntegrArt11)
        {
            long idRecord = dSuppRecordEnpals.IdSuppRecordEnpals;

            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            List<BLCommon.Entity.DatiSuppRecordENPALS> lstStoredSuppRecordEnpals = null;
            GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out lstStoredSuppRecordEnpals);

            //Costruisco il record da salvare
            dSuppRecordEnpals.DettaglioSalvato = true;
            if (!dSuppRecordEnpals.IsFromSas && !dSuppRecordEnpals.IsFromGP && lstSuppEnapls != null && lstSuppEnapls.Count > 0)
            {
                decimal importo = 0;
                lstSuppEnapls.ForEach(x => importo += x.ImportoContributivoTotale.GetValueOrDefault() + x.Importo.GetValueOrDefault());
                dSuppRecordEnpals.Importo = importo;
            }
            //Articolo11
            BLCommon.GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11BL = new GestioneIntegrazioneArt11.IntegrazioneArt11();
            Utility.ValorizzaOggetti(dIntegrArt11, integrazioneArt11BL);


            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //salvataggio
                GestioneIntegrazioneArt11.SalvaIntegrazioneArt11ByIdSuppRecordENPALS(datiPensione.Id, idRecord, integrazioneArt11BL);

                GestioneSupplementi.EliminaDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord);
                if (lstSuppEnapls != null && lstSuppEnapls.Count > 0)
                {
                    foreach (var elem in lstSuppEnapls)
                        GestioneSupplementi.SalvaDatiSupplementiEnpalsByIdSuppRecordENPALS(datiPensione.Id, idRecord, elem);
                }

                long? idTemp;
                GestioneSupplementi.SalvaDatiSuppRecordEnpals(datiPensione.Id, dSuppRecordEnpals, out idTemp);

                //gestione semafori
                lstStoredSuppRecordEnpals = lstStoredSuppRecordEnpals.Where(x => x.IdSuppRecordEnpals != idRecord).ToList();
                lstStoredSuppRecordEnpals.Add(dSuppRecordEnpals);
                ManageSemaforiEnpals(lstStoredSuppRecordEnpals, ref datiQuadroSupplementi);
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                transactionScope.Complete();
            }
        }

        public static bool ControlsStoreDatiSupplementiDettaglioEnpals(GestionePensione.DatiPensione datiPensione, BLCommon.Entity.DatiSuppRecordENPALS dSuppRecordEnpals, List<BLCommon.Entity.DatiSupplementiENPALS> lstSuppEnpals,
            BLCommon.Entity.IntegrazioneArt11 integrazioneArticol11, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!Utility.IsDomandaRiliquidazioneIndiretta(datiPensione))
            {
                //vecchi controllo sulla tab SupplementiEnpals
                if (!GestioneCrossControls.AGO_ControlDatiSupplementiEnpalsByDatiPensione(datiPensione, lstSuppEnpals, out messaggioVideo))
                    return false;
                if (integrazioneArticol11 != null && !GestioneCrossControls.AGO_VerificaSupplementiBase(dSuppRecordEnpals.RenditaFacoltativaOrdinaria, dSuppRecordEnpals.RenditaFacoltativaConvenzionale, datiPensione, out messaggioVideo))
                    return false;

                //se esiste la Decorrenza Integrazione art. 11 DPR N. 488/68 (VDAR11) deve
                //essere presente anche l'Importo IVS Integrazione art. 11 DPR. N. 488/68
                //(VIAR11)
                if (integrazioneArticol11 != null && integrazioneArticol11.Decorrenza.HasValue && !integrazioneArticol11.ImportoIVS.HasValue)
                {
                    messaggioVideo = "Se esiste la Decorrenza Integrazione art. 11 DPR N. 488/68 deve essere presente anche l'Importo IVS Integrazione art. 11 DPR. N. 488/68";
                    return false;
                }

                //2015-02-18 - (Oggetto Mail:Modifiche Enpals) 
                //Inserire un nuovo controllo sul salvataggio dei supplementi verificare che si possono avere al massimo un record per ogni quota, quindi una solo quota A e una sola quota B (domanda di test 2146687300001)
                if (lstSuppEnpals != null && lstSuppEnpals.Count > 0)
                {
                    if (lstSuppEnpals.Count(x => x.Quota == 'A') > 1)
                    {
                        messaggioVideo = "E' possibile inserire un solo record per la quota A dei dati retributivi.";
                        return false;
                    }
                    if (lstSuppEnpals.Count(x => x.Quota == 'B') > 1)
                    {
                        messaggioVideo = "E' possibile inserire un solo record per la quota B dei dati retributivi.";
                        return false;
                    }
                    //2015-02-18 - (Oggetto Mail:Modifiche Enpals)
                    //Per i supplementi sezione contributiva aggiungere la colonna “Quota”, i valori ammessi sono C e D, e può essere inserita solo la C o solo la D, non è possibile inserirle tutte e due
                    if (lstSuppEnpals.Count(x => x.Quota == 'C' || x.Quota == 'D') > 1)
                    {
                        messaggioVideo = "E' possibile inserire un solo record per la quote C e D dei dati contributivi.";
                        return false;
                    }
                }
            }
            return true;
        }

        public static void EliminaDatiSupplementiDettagliEnpalsByIdRecord(GestionePensione.DatiPensione datiPensione, long idRecord)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            List<BLCommon.Entity.DatiSuppRecordENPALS> lstStoredSuppRecordEnpals = null;
            //getto tutta la lst perchè mi serve per i semafori
            GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(datiPensione.Id, out lstStoredSuppRecordEnpals);
            //recupero l'eleme dalla lista per evitarmi la get
            BLCommon.Entity.DatiSuppRecordENPALS suppRecordEnpals = lstStoredSuppRecordEnpals.Where(x => x.IdSuppRecordEnpals == idRecord).First();

            List<BLCommon.Entity.DatiSupplementiENPALS> lstSuppEnapls;
            BLCommon.GestioneSupplementi.GetDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord, out lstSuppEnapls);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneIntegrazioneArt11.EliminaIntegrazioneArt11ByIdSuppRecordENPALS(idRecord);

                if (!suppRecordEnpals.IsFromSas)
                {
                    GestioneSupplementi.EliminaDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord);
                }
                long? idRecordTmp;
                suppRecordEnpals.RenditaFacoltativaConvenzionale = null;
                suppRecordEnpals.RenditaFacoltativaOrdinaria = null;
                suppRecordEnpals.DettaglioSalvato = false;
                suppRecordEnpals.Importo = null;
                GestioneSupplementi.SalvaDatiSuppRecordEnpals(datiPensione.Id, suppRecordEnpals, out idRecordTmp);

                //gestione semafori
                lstStoredSuppRecordEnpals = lstStoredSuppRecordEnpals.Where(x => x.IdSuppRecordEnpals != idRecord).ToList();
                lstStoredSuppRecordEnpals.Add(suppRecordEnpals);
                ManageSemaforiEnpals(lstStoredSuppRecordEnpals, ref datiQuadroSupplementi);
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);

                transactionScope.Complete();
            }
        }

        private static void ManageSemaforiEnpals(List<BLCommon.Entity.DatiSuppRecordENPALS> lstRecord, ref GestioneQuadri.DatiQuadroSupplementi datiQuadro)
        {
            //Se è visibile il tab ContribuzioneEnpals oppure
            //Se esiste almeno un record con IsFromSaS = true i IsFromGP = true significa che questi dati sono arrivati dal prelievo/SAI 
            //quindi il Tipo sarà sempre 2 (obbligatorio)
            if (datiQuadro.TabContribuzioneEnpals != null || (lstRecord != null && lstRecord.Count > 0 && lstRecord.Exists(x => x.IsFromSas || x.IsFromGP)))
            {
                //poichè ci sta ContribuzioneEnapls il quadro sarà sempre obbligatorio
                datiQuadro.Tipo = 2;
                if (lstRecord == null || lstRecord.Count == 0)
                    datiQuadro.TabSupplementi = 1;// tab giallo
                else if (lstRecord.Exists(x => !x.DettaglioSalvato))
                    datiQuadro.TabSupplementi = 0; //tab rosso
                else
                    datiQuadro.TabSupplementi = 2; //tab verde
            }
            else
            {
                //tab contribuzione enpals non visibile e non ci sta nessun record con isFromSas == true o isFromGP == true
                if (lstRecord == null || lstRecord.Count == 0)
                {
                    datiQuadro.Tipo = 1; //semagoro quadro facoltativo
                    datiQuadro.TabSupplementi = 1; //semaforo tab giallo
                }
                else if (lstRecord.Exists(x => !x.DettaglioSalvato))
                {
                    datiQuadro.Tipo = 2; //Semaforo quadro obbligatorio
                    datiQuadro.TabSupplementi = 0;//semaforo tab rosso
                }
                else
                {
                    datiQuadro.Tipo = 2; //semaforo quadro obbligatorio
                    datiQuadro.TabSupplementi = 2; //semaforo tab verde
                }
            }
        }
        #endregion Supplementi Enpals

        #region Cumulo
        public static void GetDatiSupplementiCumuloByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementiCumulo> lSupplementiCumulo)
        {
            BLCommon.GestioneSupplementi.GetSupplementiCumuloByIdPensione(idPensione, out lSupplementiCumulo);
        }

        public static void EliminaSupplementiCumuloByIdPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneSupplementi.EliminaSupplementiCumuloByIdPensione(datiPensione.Id, false);

                if (datiQuadroSupplementi != null)
                {
                    datiQuadroSupplementi.Tipo = 2;
                    datiQuadroSupplementi.TabSupplementi = 0;
                }
                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                transactionScope.Complete();
            }
        }

        public static void StoreDatiSupplementiCumuloByDatiPensione(GestionePensione.DatiPensione datiPensione, List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiCumulo)
        {
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneSupplementi.EliminaSupplementiCumuloByIdPensione(datiPensione.Id, false);
                if (listaSupplementiCumulo != null && listaSupplementiCumulo.Count > 0)
                {
                    foreach (BLCommon.Entity.DatiSupplementiCumulo supp in listaSupplementiCumulo)
                        supp.IdPensione = datiPensione.Id;
                    GestioneSupplementi.SalvaDatiSupplementiCumulo(listaSupplementiCumulo);
                }

                if (listaSupplementiCumulo == null || listaSupplementiCumulo.Count == 0)
                {
                    datiQuadroSupplementi.Tipo = 2;
                    datiQuadroSupplementi.TabSupplementi = 0;
                }
                else
                    datiQuadroSupplementi.TabSupplementi = 2;

                GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiSupplementiCumulo(GestionePensione.DatiPensione datiPensione, List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiCumulo,
            List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo, List<GestioneCalcolo.QuotePensione> listaQuotePensione, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneCrossControls.AGO_ControlDatiSupplementiCumulo(datiPensione, listaSupplementiCumulo, listaDecEnteGestioneFondo, listaQuotePensione, isRiaperturaDomanda, datiDanteCausa, out messaggioVideo))
                return false;

            return true;
        }

        //ENG - Memo 32_a/2018
        public static void GetDatiSupplementiCumuloStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementiCumulo> lSupplementiCumulo)
        {
            BLCommon.GestioneSupplementi.GetSupplementiCumuloStoricoByIdPensione(idPensione, out lSupplementiCumulo);
        }

        #endregion Cumulo

        #region Cross Properties
        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            List<BLCommon.Entity.DatiSupplementiENPALS> listaDatiSupplementiENPALS, GestioneLavorazione.DatiLavorazione datiLavorazione, out DateTime? decorrenzaPensioneDC)
        {
            bool isDomandaSperimentaleDonna;
            bool isContribuzioneEnpalsRetributivaVisible = false;
            bool isContribuzioneEnpalsContributivaVisible = false;
            bool isReversibilitaOrRicostituzione;
            bool IsPannelloSupplementiAnte96 = false;
            bool? isTipoCalcoloModificato = null;


            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();
            isDomandaSperimentaleDonna = Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione);
            isContribuzioneEnpalsRetributivaVisible = listaDatiSupplementiENPALS != null && listaDatiSupplementiENPALS.Count(x => x.TipoSupplemento == 'R' && x.IsFromSAS) > 0;
            isContribuzioneEnpalsContributivaVisible = listaDatiSupplementiENPALS != null && listaDatiSupplementiENPALS.Count(x => x.TipoSupplemento == 'C' && x.IsFromSAS) > 0;
            isReversibilitaOrRicostituzione = Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione);
            decorrenzaPensioneDC = Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) ? datiDanteCausa.DecorrenzaPensione : (DateTime?)null;
            IsPannelloSupplementiAnte96 = Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, datiDanteCausa, Utility.IsRiaperturaDomanda(datiPensione.Id));

            //ENG - MEMO 50/2023
            if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.Tipo == "0001" && !Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

                if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI")
                {
                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                    if (datiPensione.TipoCalcolo.HasValue && datiIstruttoria != null && datiIstruttoria.TipoCalcoloPrecedente.HasValue && datiPensione.TipoCalcolo != datiIstruttoria.TipoCalcoloPrecedente)
                        isTipoCalcoloModificato = true;
                }
            }

            lReturn.Add("IsDomandaSperimentaleDonna", isDomandaSperimentaleDonna);
            lReturn.Add("IsContribuzioneEnpalsRetributivaVisible", isContribuzioneEnpalsRetributivaVisible);
            lReturn.Add("IsContribuzioneEnpalsContributivaVisible", isContribuzioneEnpalsContributivaVisible);
            lReturn.Add("IsReversibilitaOrRicostituzione", isReversibilitaOrRicostituzione);
            lReturn.Add("IsPannelloSupplementiAnte96", IsPannelloSupplementiAnte96);
            lReturn.Add("IsTipoCalcoloModificato", isTipoCalcoloModificato);

            return lReturn;
        }

        #endregion Cross Properties
    }
}
