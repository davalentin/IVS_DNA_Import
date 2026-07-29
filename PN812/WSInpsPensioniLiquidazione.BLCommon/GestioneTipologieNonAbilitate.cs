using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneTipologieNonAbilitate
    {
        public static void SalvaTipologieNonAbilitate(DatiTipologieNonAbilitate datiTipologieNonAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                TipologieNonAbilitate tipologie = new TipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiTipologieNonAbilitate, tipologie);
                DAGestioneTipologieNonAbilitate.SalvaTipologieNonAbilitate(tipologie);
                transactionScope.Complete();
            }
        }

        public static void EliminaTipologieNonAbilitate(DatiTipologieNonAbilitate datiTipologieNonAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                TipologieNonAbilitate tipologie = new TipologieNonAbilitate();
                Utility.ValorizzaOggetti(datiTipologieNonAbilitate, tipologie);
                DAGestioneTipologieNonAbilitate.EliminaTipologieNonAbilitate(tipologie);
                transactionScope.Complete();
            }
        }

        public static void GetAllTipologieNonAbilitate(out List<DatiTipologieNonAbilitate> elencoTipologieNonAbilitate)
        {
            elencoTipologieNonAbilitate = null;

            List<TipologieNonAbilitate> elencoTipologieNonAbilitateDA = null;
            DAGestioneTipologieNonAbilitate.GetAllTipologieNonAbilitate(out elencoTipologieNonAbilitateDA);

            if (elencoTipologieNonAbilitateDA == null || elencoTipologieNonAbilitateDA.Count == 0)
                return;

            elencoTipologieNonAbilitate = new List<DatiTipologieNonAbilitate>();
            foreach (TipologieNonAbilitate tipNonAbil in elencoTipologieNonAbilitateDA)
            {
                DatiTipologieNonAbilitate tip = new DatiTipologieNonAbilitate();
                Utility.ValorizzaOggetti(tipNonAbil, tip);
                elencoTipologieNonAbilitate.Add(tip);
            }
        }

        #region Nested Class

        public class DatiTipologieNonAbilitate
        {
            #region private properties
            private string _TipoApp;
            private string _Fondo;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private string _Filtro;
            private string _SiglaCategoria;
            #endregion private properties

            #region public properties
            public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public string Filtro { get { return _Filtro; } set { _Filtro = value; } }
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            #endregion public properties
        }

        public class Gruppo
        {
            #region Private Properties
            private string _CodGruppo;
            private string _DescGruppo;
            #endregion Private Properties

            #region Public Properties
            public string CodGruppo { get { return _CodGruppo; } set { _CodGruppo = value; } }
            public string DescGruppo { get { return _DescGruppo; } set { _DescGruppo = value; } }
            #endregion Public Properties
        }

        public class Prodotto
        {
            #region Private Properties
            private string _CodProdotto;
            private string _DescProdotto;
            #endregion Private Properties

            #region Public Properties
            public string CodProdotto { get { return _CodProdotto; } set { _CodProdotto = value; } }
            public string DescProdotto { get { return _DescProdotto; } set { _DescProdotto = value; } }
            #endregion Public Properties
        }

        public class Tipo
        {
            #region Private Properties
            private string _CodTipo;
            private string _DescTipo;
            #endregion Private Properties

            #region Public Properties
            public string CodTipo { get { return _CodTipo; } set { _CodTipo = value; } }
            public string DescTipo { get { return _DescTipo; } set { _DescTipo = value; } }
            #endregion Public Properties
        }

        public class Filtro
        {
            #region Private Properties
            private string _Codice;
            private string _Descrizione;
            #endregion Private Properties

            #region Public Properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public Properties
        }

        #endregion Nested Class
    }
}
