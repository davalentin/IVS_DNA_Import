using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;


namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneMiglioramentiContrattuali
    {
        public static void GetDatiQuoteMiglioramentiContrattualiByIdPensione(long idPensione, out List<DatiQuoteMiglioramentiContrattuali> listaDatiQuoteMiglioramentiContrattuali)
        {
            List<QuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali = null;
            listaDatiQuoteMiglioramentiContrattuali = null;
            DAMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiByIdPensione(idPensione, out listaQuoteMiglioramentiContrattuali);
            if (listaQuoteMiglioramentiContrattuali == null || listaQuoteMiglioramentiContrattuali.Count == 0)
                return;
            listaDatiQuoteMiglioramentiContrattuali = new List<DatiQuoteMiglioramentiContrattuali>();
            foreach (var item in listaQuoteMiglioramentiContrattuali)
            {
                DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new DatiQuoteMiglioramentiContrattuali();
                Utility.ValorizzaOggetti(item, datiQuoteMiglioramentiContrattuali);
                listaDatiQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
            }
        }

        public static void GetDatiQuoteMiglioramentiContrattualiNoStoricoByIdPensione(long idPensione, out List<DatiQuoteMiglioramentiContrattuali> listaDatiQuoteMiglioramentiContrattuali)
        {
            List<QuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali = null;
            listaDatiQuoteMiglioramentiContrattuali = null;
            DAMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiNoStoricoByIdPensione(idPensione, out listaQuoteMiglioramentiContrattuali);
            if (listaQuoteMiglioramentiContrattuali == null || listaQuoteMiglioramentiContrattuali.Count == 0)
                return;
            listaDatiQuoteMiglioramentiContrattuali = new List<DatiQuoteMiglioramentiContrattuali>();
            foreach (var item in listaQuoteMiglioramentiContrattuali)
            {
                DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new DatiQuoteMiglioramentiContrattuali();
                Utility.ValorizzaOggetti(item, datiQuoteMiglioramentiContrattuali);
                listaDatiQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
            }
        }

        public static void GetDatiQuoteMiglioramentiContrattualiStoricoByIdPensione(long idPensione, out List<DatiQuoteMiglioramentiContrattuali> listaDatiQuoteMiglioramentiContrattuali)
        {
            List<QuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali = null;
            listaDatiQuoteMiglioramentiContrattuali = null;
            DAMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiStoricoByIdPensione(idPensione, out listaQuoteMiglioramentiContrattuali);
            if (listaQuoteMiglioramentiContrattuali == null || listaQuoteMiglioramentiContrattuali.Count == 0)
                return;
            listaDatiQuoteMiglioramentiContrattuali = new List<DatiQuoteMiglioramentiContrattuali>();
            foreach (var item in listaQuoteMiglioramentiContrattuali)
            {
                DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali = new DatiQuoteMiglioramentiContrattuali();
                Utility.ValorizzaOggetti(item, datiQuoteMiglioramentiContrattuali);
                listaDatiQuoteMiglioramentiContrattuali.Add(datiQuoteMiglioramentiContrattuali);
            }
        }

        public static void GetDatiMiglioramentiContrattualiByIdPensione(long idPensione, out DatiMiglioramentiContrattuali datiMiglioramentiContrattuali)
        {
            MiglioramentiContrattuali miglioramentiContrattuali = null;
            datiMiglioramentiContrattuali = null;
            DAMiglioramentiContrattuali.GetDatiMiglioramentiContrattualiByIdPensione(idPensione, out miglioramentiContrattuali);
            if (miglioramentiContrattuali == null)
                return;
            datiMiglioramentiContrattuali = new DatiMiglioramentiContrattuali();
            Utility.ValorizzaOggetti(miglioramentiContrattuali, datiMiglioramentiContrattuali);
        }

        public static void SalvaMiglioramentiContrattuali(DatiMiglioramentiContrattuali datiMiglioramentiContrattuali)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                MiglioramentiContrattuali miglioramentiContrattuali = new MiglioramentiContrattuali();
                Utility.ValorizzaOggetti(datiMiglioramentiContrattuali, miglioramentiContrattuali);
                DAMiglioramentiContrattuali.SalvaMiglioramentiContrattuali(miglioramentiContrattuali);
                transactionScope.Complete();
            }
        }

        public static void SalvaQuotaMiglioramentiContrattuali(DatiQuoteMiglioramentiContrattuali datiQuoteMiglioramentiContrattuali)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuoteMiglioramentiContrattuali quotaMiglioramentiContrattuali = new QuoteMiglioramentiContrattuali();
                Utility.ValorizzaOggetti(datiQuoteMiglioramentiContrattuali, quotaMiglioramentiContrattuali);
                DAMiglioramentiContrattuali.SalvaQuotaMiglioramentiContrattuali(quotaMiglioramentiContrattuali);
                transactionScope.Complete();
            }
        }

        public static void SalvaListaQuotaMiglioramentiContrattuali(List<DatiQuoteMiglioramentiContrattuali> lDatiQuoteMiglioramentiContrattuali)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiQuoteMiglioramentiContrattuali quote in lDatiQuoteMiglioramentiContrattuali)
                    SalvaQuotaMiglioramentiContrattuali(quote);
                transactionScope.Complete();
            }
        }

        public class DatiMiglioramentiContrattuali
        {
            private long _Id;

            private long? _IdPensione;

            private string _CodiceEnte;

            private string _CodiceCessazione;

            private string _MotivoCessazione;

            private bool? _IsStorico;

            public long Id { get { return _Id; } set { _Id = value; } }
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }
            public string CodiceCessazione { get { return _CodiceCessazione; } set { _CodiceCessazione = value; } }
            public string MotivoCessazione { get { return _MotivoCessazione; } set { _MotivoCessazione = value; } }
            public bool? IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
        }

        public class DatiQuoteMiglioramentiContrattuali
        {

            private long _Id;

            private long? _IdPensione;

            private string _Codice;

            private string _DataDecorrenza;

            private string _Quota;

            private bool _IsStorico;

            public long Id { get { return _Id; } set { _Id = value; } }
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string DataDecorrenza { get { return _DataDecorrenza; } set { _DataDecorrenza = value; } }
            public string Quota { get { return _Quota; } set { _Quota = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
        }
    }
}
