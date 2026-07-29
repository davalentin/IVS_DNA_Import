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
    public class GestioneVittimeTerrorismo
    {
        public static void GetVittimeTerrorismoByIdPensione(Int64 idPensione, out DatiVittimeTerrorismo datiVittimeTerrorismo)
        {
            VittimeTerrorismo vittimeTerrorismo = null;
            datiVittimeTerrorismo = null;
            DaGestioneVittimeTerrorismo.GetVittimeTerrorismoByIdPensione(idPensione, out vittimeTerrorismo);
            if (vittimeTerrorismo == null)
                return;
            datiVittimeTerrorismo = new DatiVittimeTerrorismo();
            Utility.ValorizzaOggetti(vittimeTerrorismo, datiVittimeTerrorismo);
        }

        public static void SalvaVittimeTerrorismo(long idPensione, DatiVittimeTerrorismo datiVittimeTerrorismo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                VittimeTerrorismo vittimeTerrorismo = new VittimeTerrorismo();
                Utility.ValorizzaOggetti(datiVittimeTerrorismo, vittimeTerrorismo);
                vittimeTerrorismo.IdPensione = idPensione;
                DaGestioneVittimeTerrorismo.SalvaVittimeTerrorismo(vittimeTerrorismo);
                transactionScope.Complete();
            }
        }

        public static void EliminaVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DaGestioneVittimeTerrorismo.EliminaVittimeTerrorismoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiVittimeTerrorismo
        {
            public DatiVittimeTerrorismo()
            { }
            public DatiVittimeTerrorismo(string tipoPrestazione,
                string codiceBeneficio, string tipoBeneficio, System.Nullable<System.DateTime> dataEvento, string beneficiario,
                System.Nullable<System.DateTime> decorrenzaBeneficiario, string codiceGestione1, string codiceGestione2,
                string codiceLiquidazione, System.Nullable<decimal> montanteContributivo, System.Nullable<decimal> importoContributivo,
                System.Nullable<int> nSettRideterminato)
            {
                this._TipoPrestazione = tipoPrestazione;
                this._CodiceBeneficio = codiceBeneficio;
                this._TipoBeneficio = tipoBeneficio;
                this._DataEvento = dataEvento;
                this._Beneficiario = beneficiario;
                this._DecorrenzaBeneficiario = decorrenzaBeneficiario;
                this._CodiceGestione1 = codiceGestione1;
                this._CodiceGestione2 = codiceGestione2;
                this._CodiceLiquidazione = codiceLiquidazione;
                this._MontanteContributivo = montanteContributivo;
                this._ImportoContributivo = importoContributivo;
                this._NSettRideterminato = nSettRideterminato;
            }

            #region private properties

            private string _TipoPrestazione;

            private string _CodiceBeneficio;

            private string _TipoBeneficio;

            private System.Nullable<System.DateTime> _DataEvento;

            private string _Beneficiario;

            private System.Nullable<System.DateTime> _DecorrenzaBeneficiario;

            private string _CodiceGestione1;

            private string _CodiceGestione2;

            private string _CodiceLiquidazione;

            private System.Nullable<decimal> _MontanteContributivo;

            private System.Nullable<decimal> _ImportoContributivo;

            private System.Nullable<int> _NSettRideterminato;
            #endregion private properties

            #region public properties

            public string TipoPrestazione { get { return _TipoPrestazione; } set { _TipoPrestazione = value; } }

            public string CodiceBeneficio { get { return _CodiceBeneficio; } set { _CodiceBeneficio = value; } }

            public string TipoBeneficio { get { return _TipoBeneficio; } set { _TipoBeneficio = value; } }

            public System.Nullable<System.DateTime> DataEvento { get { return _DataEvento; } set { _DataEvento = value; } }

            public string Beneficiario { get { return _Beneficiario; } set { _Beneficiario = value; } }

            public System.Nullable<System.DateTime> DecorrenzaBeneficiario { get { return _DecorrenzaBeneficiario; } set { _DecorrenzaBeneficiario = value; } }

            public string CodiceGestione1 { get { return _CodiceGestione1; } set { _CodiceGestione1 = value; } }

            public string CodiceGestione2 { get { return _CodiceGestione2; } set { _CodiceGestione2 = value; } }

            public string CodiceLiquidazione { get { return _CodiceLiquidazione; } set { _CodiceLiquidazione = value; } }

            public System.Nullable<decimal> MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }

            public System.Nullable<decimal> ImportoContributivo { get { return _ImportoContributivo; } set { _ImportoContributivo = value; } }

            public System.Nullable<int> NSettRideterminato { get { return _NSettRideterminato; } set { _NSettRideterminato = value; } }
            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiVittimeTerrorismo vittimeTerrorismo = (DatiVittimeTerrorismo)obj;
                try
                {
                    if ((this._TipoPrestazione != null ? this._TipoPrestazione.Trim() : null) != (vittimeTerrorismo._TipoPrestazione != null ? vittimeTerrorismo._TipoPrestazione.Trim() : null) ||
                        (this._CodiceBeneficio != null ? this._CodiceBeneficio.Trim() : null) != (vittimeTerrorismo._CodiceBeneficio != null ? vittimeTerrorismo._CodiceBeneficio.Trim() : null) ||
                        (this._TipoBeneficio != null ? this._TipoBeneficio.Trim() : null) != (vittimeTerrorismo._TipoBeneficio != null ? vittimeTerrorismo._TipoBeneficio.Trim() : null) ||
                        this._DataEvento != vittimeTerrorismo._DataEvento ||
                        this._Beneficiario != vittimeTerrorismo._Beneficiario ||
                        this._DecorrenzaBeneficiario != vittimeTerrorismo._DecorrenzaBeneficiario ||
                        (this._CodiceGestione1 != null ? this._CodiceGestione1.Trim() : null) != (vittimeTerrorismo._CodiceGestione1 != null ? vittimeTerrorismo._CodiceGestione1.Trim() : null) ||
                        (this._CodiceGestione2 != null ? this._CodiceGestione2.Trim() : null) != (vittimeTerrorismo._CodiceGestione2 != null ? vittimeTerrorismo._CodiceGestione2.Trim() : null) ||
                        (this._CodiceLiquidazione != null ? this._CodiceLiquidazione.Trim() : null) != (vittimeTerrorismo._CodiceLiquidazione != null ? vittimeTerrorismo._CodiceLiquidazione.Trim() : null) ||
                        this._MontanteContributivo != vittimeTerrorismo._MontanteContributivo ||
                        this._ImportoContributivo != vittimeTerrorismo._ImportoContributivo ||
                        this._NSettRideterminato != vittimeTerrorismo._NSettRideterminato)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._TipoPrestazione != null ? this._TipoPrestazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceBeneficio != null ? this._CodiceBeneficio.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._TipoBeneficio != null ? this._TipoBeneficio.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataEvento != null ? this._DataEvento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Beneficiario != null ? this._Beneficiario.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaBeneficiario != null ? this._DecorrenzaBeneficiario.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceGestione1 != null ? this._CodiceGestione1.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceGestione2 != null ? this._CodiceGestione2.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceLiquidazione != null ? this._CodiceLiquidazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MontanteContributivo != null ? this._MontanteContributivo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoContributivo != null ? this._ImportoContributivo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NSettRideterminato != null ? this._NSettRideterminato.GetHashCode() : 0);
            //    return hash;
            //}
            #endregion public members
        }
        #endregion nested class
    }
}
