using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneFAQ
    {
        public static void GetFAQs(string tipoApp, out List<DatiFAQ> listaFAQ)
        {
            listaFAQ = null;
            List<FAQ> elencoFAQDB = null;
            DAGestioneFAQ.GetFAQs(tipoApp, out elencoFAQDB);
            if (elencoFAQDB != null && elencoFAQDB.Count > 0)
            {
                listaFAQ = new List<DatiFAQ>();
                foreach (FAQ faqDB in elencoFAQDB)
                {
                    DatiFAQ faq = new DatiFAQ();
                    Utility.ValorizzaOggetti(faqDB, faq);
                    listaFAQ.Add(faq);
                }
            }
        }

        public static void SalvaFAQ(DatiFAQ faq, bool cambiaCodice)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    FAQ faqDB = new FAQ();
                    Utility.ValorizzaOggetti(faq, faqDB);
                    DAGestioneFAQ.SalvaFAQ(faqDB);

                    if(cambiaCodice)
                        DAGestioneFAQ.UpdateContatoreFAQ(faqDB.Tipologia);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        public static void DeleteFAQ(long idFAQ)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    DAGestioneFAQ.DeleteFAQ(idFAQ);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        public static void UpdateContatoreFAQ(string codice)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    DAGestioneFAQ.UpdateContatoreFAQ(codice);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiFAQ
        {
            #region private properties
            private long _Id;
            private string _Domanda;
            private string _Risposta;
            private string _TipoApp;
            private string _Codice;
            private string _Tipologia;
            private bool? _Visibilita;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public string Domanda { get { return _Domanda; } set { _Domanda = value; } }
            public string Risposta { get { return _Risposta; } set { _Risposta = value; } }
            public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            public bool? Visibilita { get { return _Visibilita; } set { _Visibilita = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
