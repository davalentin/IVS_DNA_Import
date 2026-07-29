using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDelegatoTutore
    {
        public static void GetDelegatoByIdPensione(long idPensione, out GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            Anagrafica anagrafica = null;
            char codiceDelegato;
            datiAnagrafica = null;

            DAGestioneDelegato.GetAnagraficaDelegatoByIdPensione(idPensione, out anagrafica, out codiceDelegato);
            if (anagrafica == null)
                return;
            datiAnagrafica = new GestioneAnagrafica.DatiAnagrafici();
            Utility.ValorizzaOggetti(anagrafica, datiAnagrafica);
            datiAnagrafica.CodiceDelegato = codiceDelegato;
        }

        public static void GetTutoreByIdPensione(long idPensione, out GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            Anagrafica anagrafica = null;
            char codiceTutore;
            DateTime? cessValAmmSost;
            datiAnagrafica = null;

            DAGestioneTutore.GetAnagraficaTutoreByIdPensione(idPensione, out anagrafica, out codiceTutore, out cessValAmmSost);
            if (anagrafica == null)
                return;
            datiAnagrafica = new GestioneAnagrafica.DatiAnagrafici();
            Utility.ValorizzaOggetti(anagrafica, datiAnagrafica);
            datiAnagrafica.CodiceTutore = codiceTutore;
            datiAnagrafica.CessValAmmSost = cessValAmmSost;
        }

        public static void SalvaDelegatoByDatiPensione(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            GestioneQuadri.DatiQuadroDelegatoTutore datiQuadroDelegatoTutore;
            GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out datiQuadroDelegatoTutore);

            Anagrafica anagrafica = new Anagrafica();
            Utility.ValorizzaOggetti(datiAnagrafica, anagrafica);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDelegato.CancellaDelegatoByIdPensione(datiPensione.Id);

                if (datiAnagrafica.CodiceDelegato != null)
                    DAGestioneDelegato.SalvaDelegato(datiPensione.Id, anagrafica, datiAnagrafica.CodiceDelegato);
                else
                    throw new DnaValidationException("Valori non validi per le proprietà 'codice delegato'.");

                if (!String.IsNullOrEmpty(datiAnagrafica.CodiceFiscale) && datiAnagrafica.CodiceDelegato != null)
                {
                    if (datiAnagrafica.CodiceDelegato.ToString().Trim() != String.Empty)
                        datiQuadroDelegatoTutore.TabDelegato = 2;
                    else
                        datiQuadroDelegatoTutore.TabDelegato = 1;
                }
                else
                    datiQuadroDelegatoTutore.TabDelegato = 1;

                GestioneQuadri.SalvaQuadroDelegatoTutore(datiPensione.Id, datiQuadroDelegatoTutore);

                transactionScope.Complete();
            }
        }

        public static void SalvaDelegatoDaPrelievo(long idPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            Anagrafica anagrafica = new Anagrafica();
            Utility.ValorizzaOggetti(datiAnagrafica, anagrafica);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiAnagrafica.CodiceDelegato != null)
                    DAGestioneDelegato.SalvaDelegato(idPensione, anagrafica, datiAnagrafica.CodiceDelegato);
                transactionScope.Complete();
            }
        }

        public static void SalvaTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            GestioneQuadri.DatiQuadroDelegatoTutore datiQuadroDelegatoTutore;

            GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out datiQuadroDelegatoTutore);

            Anagrafica anagrafica = new Anagrafica();
            Utility.ValorizzaOggetti(datiAnagrafica, anagrafica);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneTutore.CancellaTutoreByIdPensione(datiPensione.Id);

                if (datiAnagrafica.CodiceTutore != null)
                    DAGestioneTutore.SalvaTutore(datiPensione.Id, anagrafica, datiAnagrafica.CodiceTutore, datiAnagrafica.CessValAmmSost);
                else
                    throw new DnaValidationException("Valori non validi per le proprietà 'codice tutore'.");

                if (!String.IsNullOrEmpty(datiAnagrafica.CodiceFiscale)
                    && datiAnagrafica.CodiceTutore != null
                    && datiAnagrafica.CodiceTutore.ToString().Trim() != String.Empty)
                    datiQuadroDelegatoTutore.TabTutore = 2;
                else
                    datiQuadroDelegatoTutore.TabTutore = 1;

                GestioneQuadri.SalvaQuadroDelegatoTutore(datiPensione.Id, datiQuadroDelegatoTutore);

                transactionScope.Complete();
            }
        }

        public static void SalvaTutoreDaPrelievo(long idPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafica)
        {
            Anagrafica anagrafica = new Anagrafica();
            Utility.ValorizzaOggetti(datiAnagrafica, anagrafica);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiAnagrafica.CodiceTutore != null)
                    DAGestioneTutore.SalvaTutore(idPensione, anagrafica, datiAnagrafica.CodiceTutore, datiAnagrafica.CessValAmmSost);

                transactionScope.Complete();
            }
        }

        public static void EliminaDelegatoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDelegatoTutore datiQuadroDelegatoTutore = null;
            GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out datiQuadroDelegatoTutore);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiPensione != null)
                {
                    DAGestioneDelegato.CancellaDelegatoByIdPensione(datiPensione.Id);
                    bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
                    {
                        //ENG - Reversibilita 024 
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

                        if ((Utility.IsDomandaINPDAP(datiPensione.Gestione) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                            || (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)))
                        {
                            datiQuadroDelegatoTutore.TabDelegato = 1;
                            //ENG - Reversibilita 024
                            if (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS))
                            {
                                if (!datiQuadroDelegatoTutore.TabTutore.HasValue || datiQuadroDelegatoTutore.TabTutore != 0)
                                    datiQuadroDelegatoTutore.Tipo = 1;
                                else
                                    datiQuadroDelegatoTutore.Tipo = 2;
                            }
                        }
                        else
                            datiQuadroDelegatoTutore.TabDelegato = null;
                    }
                    else
                        datiQuadroDelegatoTutore.TabDelegato = 1;
                    GestioneQuadri.SalvaQuadroDelegatoTutore(datiPensione.Id, datiQuadroDelegatoTutore);
                }

                transactionScope.Complete();
            }
        }

        public static void EliminaTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDelegatoTutore datiQuadroDelegatoTutore = null;
            GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out datiQuadroDelegatoTutore);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiPensione != null)
                {
                    DAGestioneTutore.CancellaTutoreByIdPensione(datiPensione.Id);
                    bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
                    {
                        //ENG - Reversibilita 024 
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

                        if ((Utility.IsDomandaINPDAP(datiPensione.Gestione) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                            || (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS)))
                        {
                            datiQuadroDelegatoTutore.TabTutore = 1;
                            //ENG - Reversibilita 024
                            if (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS))
                            {
                                if (!datiQuadroDelegatoTutore.TabDelegato.HasValue || datiQuadroDelegatoTutore.TabDelegato != 0)
                                    datiQuadroDelegatoTutore.Tipo = 1;
                                else
                                    datiQuadroDelegatoTutore.Tipo = 2;
                            }
                        }
                        else
                            datiQuadroDelegatoTutore.TabTutore = null;
                    }
                    else
                        datiQuadroDelegatoTutore.TabTutore = 1;
                    GestioneQuadri.SalvaQuadroDelegatoTutore(datiPensione.Id, datiQuadroDelegatoTutore);
                }

                transactionScope.Complete();
            }
        }

        //ENG - Reversibilita 024
        public static void ImpostaTabTuteleObbligatorio(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDelegatoTutore datiQuadroDelegatoTutore = null;
            GestioneQuadri.GetQuadroDelegatoTutoreByDatiPensione(datiPensione, out datiQuadroDelegatoTutore);

            if (datiQuadroDelegatoTutore != null)
            {
                datiQuadroDelegatoTutore.Tipo = 2;
                datiQuadroDelegatoTutore.TabTutore = 0;
                GestioneQuadri.SalvaQuadroDelegatoTutore(datiPensione.Id, datiQuadroDelegatoTutore);
            }
        }
    }
}
