using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneBeneficiParticolari
    {
        public static void GetBeneficiParticolariByIdPensione(long idPensione, GestionePensione.DatiPensione datiPensione, out List<DatiBeneficiParticolari> LbeneficiParticolari)
        {
            List<BeneficiParticolari> LbeneficiParticolariDB = null;
            LbeneficiParticolari = null;
            DAGestioneBeneficiParticolari.GetDatiBeneficiParticolariByIdPensione(idPensione, out LbeneficiParticolariDB);

            if (LbeneficiParticolariDB == null)
                return;

            LbeneficiParticolari = new List<DatiBeneficiParticolari>();
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            foreach (BeneficiParticolari beneficiParticolaridb in LbeneficiParticolariDB)
            {
                DatiBeneficiParticolari datiBeneficiParticolari = new DatiBeneficiParticolari();
                Utility.ValorizzaOggetti(beneficiParticolaridb, datiBeneficiParticolari);
               
                    LbeneficiParticolari.Add(datiBeneficiParticolari);
            }
        }

        public static void GetBeneficiParticolariStoricoByIdPensione(long idPensione, out List<DatiBeneficiParticolari> LbeneficiParticolari)
        {
            List<BeneficiParticolari> LbeneficiParticolariDB = null;
            LbeneficiParticolari = null;
            DAGestioneBeneficiParticolari.GetDatiBeneficiParticolariStoricoByIdPensione(idPensione, out LbeneficiParticolariDB);

            if (LbeneficiParticolariDB == null)
                return;

            LbeneficiParticolari = new List<DatiBeneficiParticolari>();
            foreach (BeneficiParticolari beneficiParticolaridb in LbeneficiParticolariDB)
            {
                DatiBeneficiParticolari datiBeneficiParticolari = new DatiBeneficiParticolari();
                Utility.ValorizzaOggetti(beneficiParticolaridb, datiBeneficiParticolari);
                LbeneficiParticolari.Add(datiBeneficiParticolari);
            }
        }

        public static void SalvaDatiBeneficiParticolari(DatiBeneficiParticolari beneficiParticolari)
        {
            BeneficiParticolari benToDB = new BeneficiParticolari();
            Utility.ValorizzaOggetti(beneficiParticolari, benToDB);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneBeneficiParticolari.SalvaDatiBeneficiParticolari(benToDB);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiBeneficiParticolariByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneBeneficiParticolari.CancellaDatiBeneficiParticolariNoStoricoByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #region Nested Class

        public class DatiBeneficiParticolari
        {
            #region Private Methods

            private long _Id;
            private long _IdPensione;
            private string _CodiceBenefici;
            private short? _Settimane;
            private bool _IsStorico;

            #endregion Private Methods

            #region Public Methods

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string CodiceBenefici { get { return _CodiceBenefici; } set { _CodiceBenefici = value; } }
            public short? Settimane { get { return _Settimane; } set { _Settimane = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion Public Methods

            public bool IsDatiBeneficiParticolariNull()
            {
                if (this.Settimane.HasValue || !string.IsNullOrEmpty(this.CodiceBenefici))
                    return false;
                else
                    return true;
            }
        }

        #endregion Nested Class
    }
}
