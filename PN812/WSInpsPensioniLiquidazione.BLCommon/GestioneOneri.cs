using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneOneri
    {
        public static void GetOneriByIdPensione(long idPensione, out List<DatiOneri> lDatiOneri)
        {
            List<Oneri> lOneriDB = null;
            lDatiOneri = null;
            DAGestioneOneri.GetOneriByIdPensione(idPensione, out lOneriDB);
            if (lOneriDB == null || lOneriDB.Count == 0)
                return;
            lDatiOneri = new List<DatiOneri>();
            foreach (Oneri oneridb in lOneriDB)
            {
                DatiOneri datiOneri = new DatiOneri();
                Utility.ValorizzaOggetti(oneridb, datiOneri);
                lDatiOneri.Add(datiOneri);
            }
        }

        public static void GetOneriStoricoByIdPensione(long idPensione, out List<DatiOneri> lDatiOneri)
        {
            List<Oneri> lOneriDB = null;
            lDatiOneri = null;
            DAGestioneOneri.GetOneriStoricoByIdPensione(idPensione, out lOneriDB);
            if (lOneriDB == null || lOneriDB.Count == 0)
                return;
            lDatiOneri = new List<DatiOneri>();
            foreach (Oneri oneridb in lOneriDB)
            {
                DatiOneri datiOneri = new DatiOneri();
                Utility.ValorizzaOggetti(oneridb, datiOneri);
                lDatiOneri.Add(datiOneri);
            }
        }

        public static void SalvaOneriOnere(DatiOneri datiOneriOnere)
        {
            Oneri oneri = new Oneri();
            Utility.ValorizzaOggetti(datiOneriOnere, oneri);

            // questo lavoro viene fatto in precedenza
            //List<DecCodeSottoGruppoOneri> elencoSottoGruppoOneri = null;
            //List<DecCodeGruppoOneri> elencoGruppoOneri = null;
            //DAGestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);
            //DAGestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);

            //DecCodeGruppoOneri codeGruppoOneri = elencoGruppoOneri.Find(delegate (DecCodeGruppoOneri dec)
            //{ return (dec.Id == datiOneriOnere.IdCodeGruppo.Value); });

            //DecCodeSottoGruppoOneri codeSottoGruppoOneri = elencoSottoGruppoOneri.Find(delegate(DecCodeSottoGruppoOneri dec)
            //{ return (dec.Id == datiOneriOnere.IdCodeSottoGruppo.Value); });
            
            //oneri.IdCodeGruppo      = codeGruppoOneri.Id;
            //oneri.IdCodeSottoGruppo = codeSottoGruppoOneri.Id;


            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneOneri.SalvaOneriOnere(oneri);
                
                transactionScope.Complete();
            }
        }

        public static void EliminaOneriByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneOneri.EliminaOneriNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static bool IsOneriNull(DatiOneri Oneri)
        {
            if (!Oneri.Decorrenza.HasValue && !Oneri.Scadenza.HasValue && !Oneri.ScadenzaBeneficio.HasValue && !Oneri.IdCodeGruppo.HasValue && !Oneri.IdCodeSottoGruppo.HasValue && !Oneri.Settimane.HasValue && !Oneri.Onere.HasValue)
                return true;
            else
                return false;
        }

        #region nested class

        public class DatiOneri
        {
            public DatiOneri()
            { }
            public DatiOneri(long? id, long? idPensione, DateTime? decorrenza, DateTime? scadenza, DateTime? scadenzaBeneficio, long? idcodeGruppo, long? idcodeSottoGruppo, short? settiname, decimal? onere, bool isStorico)
            {
                this._Id = id;
                this._IdPensione = idPensione;
                this._Decorrenza = decorrenza;
                this._Scadenza = scadenza;
                this._ScadenzaBeneficio = scadenzaBeneficio;
                this._IdCodeGruppo = idcodeGruppo;
                this._IdCodeSottoGruppo = idcodeSottoGruppo;
                this._Settimane = settiname;
                this._Onere = onere;
                this._IsStorico = isStorico;
            }

            #region private properties

            private long? _Id;

            private long? _IdPensione;

            private DateTime? _Decorrenza;

            private DateTime? _Scadenza;

            private DateTime? _ScadenzaBeneficio;

            private long? _IdCodeGruppo;

            private long? _IdCodeSottoGruppo;

            private short? _Settimane;

            private decimal? _Onere;

            private bool _IsStorico;

            private short? _GP2PBB80;

            #endregion private properties

            #region public properties

            public long? Id{ get { return _Id; } set { _Id = value; } }

            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            public DateTime? Scadenza { get { return _Scadenza; } set { _Scadenza = value; } }

            public DateTime? ScadenzaBeneficio { get { return _ScadenzaBeneficio; } set { _ScadenzaBeneficio = value; } }

            public long? IdCodeGruppo { get { return _IdCodeGruppo; } set { _IdCodeGruppo = value; } }

            public long? IdCodeSottoGruppo { get { return _IdCodeSottoGruppo; } set { _IdCodeSottoGruppo = value; } }

            public short? Settimane { get { return _Settimane; } set { _Settimane = value; } }

            public decimal? Onere { get { return _Onere; } set { _Onere = value; } }

            public bool IsStorico { get { return _IsStorico;} set { _IsStorico = value;} }

            public short? GP2PBB80 { get { return _GP2PBB80; } set { _GP2PBB80 = value; } }

            #endregion public properties

        }

        #endregion nested class
    }
}
