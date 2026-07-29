using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneMessaggiHermes
    {
        public static void GetMessaggiHermesAttivi(string tipoApp, out List<DatiMessaggiHermes> elencoMessaggiHermesAttivi)
        {
            elencoMessaggiHermesAttivi = null;
            List<MessaggiHerme> elencoMessaggiHermes = null;
            DAGestioneMessaggiHermes.GetMessaggiHermesAttivi(tipoApp, out elencoMessaggiHermes);
            if (elencoMessaggiHermes != null && elencoMessaggiHermes.Count > 0)
            {
                elencoMessaggiHermesAttivi = new List<DatiMessaggiHermes>();
                foreach (MessaggiHerme mDB in elencoMessaggiHermes)
                {
                    DatiMessaggiHermes mBL = new DatiMessaggiHermes();
                    Utility.ValorizzaOggetti(mDB, mBL);
                    elencoMessaggiHermesAttivi.Add(mBL);
                }
            }
        }

        public static void GetAllMessaggiHermes(string tipoApp, out List<DatiMessaggiHermes> elencoMessaggiHermes)
        {
            elencoMessaggiHermes = null;
            List<MessaggiHerme> elencoMessaggiHermesDB = null;
            DAGestioneMessaggiHermes.GetAllMessaggiHermes(tipoApp, out elencoMessaggiHermesDB);
            if (elencoMessaggiHermesDB != null && elencoMessaggiHermesDB.Count > 0)
            {
                elencoMessaggiHermes = new List<DatiMessaggiHermes>();
                foreach (MessaggiHerme mDB in elencoMessaggiHermesDB)
                {
                    DatiMessaggiHermes mBL = new DatiMessaggiHermes();
                    Utility.ValorizzaOggetti(mDB, mBL);
                    elencoMessaggiHermes.Add(mBL);
                }
            }
        }

        public static void SalvaMessaggioHermes(DatiMessaggiHermes datiMessaggioHermes)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    MessaggiHerme messaggioHermes = new MessaggiHerme();
                    Utility.ValorizzaOggetti(datiMessaggioHermes, messaggioHermes);
                    DAGestioneMessaggiHermes.SalvaMessaggioHermes(messaggioHermes);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        public static void DeleteMessaggioHermes(long idMessaggioHermes)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    DAGestioneMessaggiHermes.DeleteMessaggioHermes(idMessaggioHermes);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiMessaggiHermes
        {
            #region private properties
            private long _Id;
            private string _Titolo;
            private string _Testo;
            private string _Url;
            private string _Categoria;
            private bool _Attivo;
            private string _Tipologia;
            private System.DateTime _TimeStamp;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public string Titolo { get { return _Titolo; } set { _Titolo = value; } }
            public string Testo { get { return _Testo; } set { _Testo = value; } }
            public string Url { get { return _Url; } set { _Url = value; } }
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public bool Attivo { get { return _Attivo; } set { _Attivo = value; } }
            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            public System.DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
