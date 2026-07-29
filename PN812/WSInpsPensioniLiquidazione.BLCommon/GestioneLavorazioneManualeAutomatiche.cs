using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneLavorazioneManualeAutomatiche
    {
        #region LavorazioneManualeAutomatiche
        public static void GetAllPensioniLavorazioneManualeAutomatiche(string tipoApp, out List<DatiLavorazioneManualeAutomatiche> lstDatiLavorazioneManualeAutomatiche)
        {
            lstDatiLavorazioneManualeAutomatiche = new List<DatiLavorazioneManualeAutomatiche>();

            List<PensioniLavorazioneManualeAutomatiche> lstDataLayer = new List<PensioniLavorazioneManualeAutomatiche>();
            DAGestionePensioniLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomatiche(tipoApp, out lstDataLayer);
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (PensioniLavorazioneManualeAutomatiche objDb in lstDataLayer)
                {
                    DatiLavorazioneManualeAutomatiche objBl = new DatiLavorazioneManualeAutomatiche();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    lstDatiLavorazioneManualeAutomatiche.Add(objBl);
                }
            }
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(string utente, string tipoApp, List<Int16> codiceSede, out List<DatiLavorazioneManualeAutomatiche> lstDatiLavorazioneManualeAutomatiche)
        {
            lstDatiLavorazioneManualeAutomatiche = new List<DatiLavorazioneManualeAutomatiche>();

            List<PensioniLavorazioneManualeAutomatiche> lstDataLayer = new List<PensioniLavorazioneManualeAutomatiche>();
            DAGestionePensioniLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(utente, tipoApp, codiceSede, out lstDataLayer);
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (PensioniLavorazioneManualeAutomatiche objDb in lstDataLayer)
                {
                    DatiLavorazioneManualeAutomatiche objBl = new DatiLavorazioneManualeAutomatiche();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    lstDatiLavorazioneManualeAutomatiche.Add(objBl);
                }
            }
        }

        public static void GetAllPensioniLavorazioneManualeAutomaticheByNDomus(string gruppo, string prodotto, string tipo, long nDomus, out List<DatiLavorazioneManualeAutomatiche> lstDatiLavorazioneManualeAutomatiche)
        {
            lstDatiLavorazioneManualeAutomatiche = new List<DatiLavorazioneManualeAutomatiche>();

            List<PensioniLavorazioneManualeAutomatiche> lstDataLayer = new List<PensioniLavorazioneManualeAutomatiche>();
            DAGestionePensioniLavorazioneManualeAutomatiche.GetAllPensioniLavorazioneManualeAutomaticheByNDomus(gruppo, prodotto, tipo, nDomus, out lstDataLayer);
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (PensioniLavorazioneManualeAutomatiche objDb in lstDataLayer)
                {
                    DatiLavorazioneManualeAutomatiche objBl = new DatiLavorazioneManualeAutomatiche();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    lstDatiLavorazioneManualeAutomatiche.Add(objBl);
                }
            }
        }

        public static void SalvaLavorazioneManualeAutomatiche(DatiLavorazioneManualeAutomatiche datiLavorazioneManualeAutomatiche)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioniLavorazioneManualeAutomatiche objDl = new PensioniLavorazioneManualeAutomatiche();
                Utility.ValorizzaOggetti(datiLavorazioneManualeAutomatiche, objDl);
                DAGestionePensioniLavorazioneManualeAutomatiche.InsertPensioniLavorazioneManualeAutomatiche(objDl);
                transactionScope.Complete();
            }
        }
        #endregion LavorazioneManualeAutomatiche

        #region TipologieAutomaticheUnicarpe
        public static void GetAllTipologieAutomaticheUnicarpe(out List<TipologiaAutomaticaUnicarpe> lstDatiTipologieAutomaticheUnicarpe)
        {
            lstDatiTipologieAutomaticheUnicarpe = new List<TipologiaAutomaticaUnicarpe>();
            List<DAGestioneTipologieAutomaticheUnicarpe.TipologiaAutomaticaUnicarpe>  lstManualeAutomatiche = new List<DAGestioneTipologieAutomaticheUnicarpe.TipologiaAutomaticaUnicarpe>();
            DAGestioneTipologieAutomaticheUnicarpe.GetAllTipologieAutomaticheUnicarpe(out lstManualeAutomatiche);
            if (lstManualeAutomatiche != null && lstManualeAutomatiche.Count > 0)
            {
                foreach (DAGestioneTipologieAutomaticheUnicarpe.TipologiaAutomaticaUnicarpe objDb in lstManualeAutomatiche)
                {
                    if(objDb.DecorrenzaMinima.HasValue && !string.IsNullOrEmpty(objDb.SiglaCategoria) && !string.IsNullOrEmpty(objDb.Gruppo) && !string.IsNullOrEmpty(objDb.Prodotto) && !string.IsNullOrEmpty(objDb.Tipo))
                    {
                    TipologiaAutomaticaUnicarpe objBl = new TipologiaAutomaticaUnicarpe();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    lstDatiTipologieAutomaticheUnicarpe.Add(objBl);
                    }
                }
            }
        }
        #endregion TipologieAutomaticheUnicarpe

        #region Nested Class

        public class DatiLavorazioneManualeAutomatiche
        {
            public long? Id { get; set; }
            public long? NDomus { get; set; }
            public string SiglaCategoria { get; set; }
            public short CodiceSede { get; set; }
            public string Gruppo { get; set; }
            public string Prodotto { get; set; }
            public string Tipo { get; set; }
            public DateTime? DecorrenzaOriginaria { get; set; }
            public bool? AutorizzazioneManuale { get; set; }
            public string MatricolaUtente { get; set; }
            public string TipoApp { get; set; }
        }

        public class TipologiaAutomaticaUnicarpe
        {
            public string SiglaCategoria { get; set; }
            public string Gruppo { get; set; }
            public string Prodotto { get; set; }
            public string Tipo { get; set; }
            public string CodiceTipoRichiesta { get; set; }
            public DateTime? DecorrenzaMinima { get; set; }
        }

        #endregion Nested Class
    }
}
