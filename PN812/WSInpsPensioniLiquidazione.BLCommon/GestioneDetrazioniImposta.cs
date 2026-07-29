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
    public class GestioneDetrazioniImposta
    {
        public static void GetDetrazioniByIdPensione(Int64 idPensione, out DatiDetrazioni datiDetrazioni)
        {
            DetrazioniImposta detrazioni = null;
            datiDetrazioni = null;
            DAGestioneDetrazioni.GetDetrazioniImpostaByIdPensione(idPensione, out detrazioni);
            if (detrazioni == null)
                return;
            datiDetrazioni = new DatiDetrazioni();
            Utility.ValorizzaOggetti(detrazioni, datiDetrazioni);
        }

        public static void GetDetrazioniStoricoByIdPensione(long idPensione, out DatiDetrazioni datiDetrazioni)
        {
            DetrazioniImposta detrazioni = null;
            datiDetrazioni = null;
            DAGestioneDetrazioni.GetDetrazioniImpostaStoricoByIdPensione(idPensione, out detrazioni);
            if (detrazioni == null)
                return;
            datiDetrazioni = new DatiDetrazioni();
            Utility.ValorizzaOggetti(detrazioni, datiDetrazioni);
        }

        public static void SalvaDetrazioni(long idPensione, DatiDetrazioni datiDetrazioni)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DetrazioniImposta detrazioni = new DetrazioniImposta();
                Utility.ValorizzaOggetti(datiDetrazioni, detrazioni);
                detrazioni.IdPensione = idPensione;
                DAGestioneDetrazioni.SalvaDetrazioniImposta(detrazioni);
                transactionScope.Complete();
            }
        }

        public static void EliminaDetrazioniByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneDetrazioni.EliminaDetrazioniImpostaByIdPensione(idPensione);
                else
                    DAGestioneDetrazioni.EliminaDetrazioniImpostaNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiDetrazioni
        {
            public DatiDetrazioni()
            { }
            public DatiDetrazioni(byte? detrazioniReddito, byte? agevolazionePensionati, byte? coniugeOFiglio,
                byte? figliMinori3AnniNoHandicap100, byte? figliMinori3AnniNoHandicap50,
                byte? figliMinori3AnniHandicap100, byte? figliMinori3AnniHandicap50, byte? figliMaggiori3AnniNoHandicap100,
                byte? figliMaggiori3AnniNoHandicap50, byte? figliMaggiori3AnniHandicap100,
                byte? figliMaggiori3AnniHandicap50, byte? altriFamiliari100, byte? altriFamiliari50, byte? addizionaleLombardiaVeneto,
                byte? nonResidenteSchumacker, byte? convDoppieImposizioni, DateTime? decorrenzaDetrazioneImposte)
            {
                this._DetrazioniReddito = detrazioniReddito;

                this._AgevolazionePensionati = agevolazionePensionati;

                this._ConiugeOFiglio = coniugeOFiglio;

                this._FigliMinori3AnniNoHandicap100 = figliMinori3AnniNoHandicap100;

                this._FigliMinori3AnniNoHandicap50 = figliMinori3AnniNoHandicap50;

                this._FigliMinori3AnniHandicap100 = figliMinori3AnniHandicap100;

                this._FigliMinori3AnniHandicap50 = figliMinori3AnniHandicap50;

                this._FigliMaggiori3AnniNoHandicap100 = figliMaggiori3AnniNoHandicap100;

                this._FigliMaggiori3AnniNoHandicap50 = figliMaggiori3AnniNoHandicap50;

                this._FigliMaggiori3AnniHandicap100 = figliMaggiori3AnniHandicap100;

                this._FigliMaggiori3AnniHandicap50 = figliMaggiori3AnniHandicap50;

                this._AltriFamiliari100 = altriFamiliari100;

                this._AltriFamiliari50 = altriFamiliari50;

                this._AddizionaleLombardiaVeneto = addizionaleLombardiaVeneto;

                this._NonResidenteSchumacker = nonResidenteSchumacker;

                this._ConvDoppieImposizioni = convDoppieImposizioni;

                this._DecorrenzaDetrazioneImposte = decorrenzaDetrazioneImposte;
            }

            public DatiDetrazioni(string detrazioni)
            {
                this._DetrazioniReddito = Utility.StringToNullableByte(detrazioni.Substring(0, 1));

                this._AgevolazionePensionati = Utility.StringToNullableByte(detrazioni.Substring(1, 1));

                this._ConiugeOFiglio = Utility.StringToNullableByte(detrazioni.Substring(2, 1));

                this._FigliMinori3AnniNoHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(3, 1));

                this._FigliMinori3AnniNoHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(4, 1));

                this._FigliMinori3AnniHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(5, 1));

                this._FigliMinori3AnniHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(6, 1));

                this._FigliMaggiori3AnniNoHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(7, 1));

                this._FigliMaggiori3AnniNoHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(8, 1));

                this._FigliMaggiori3AnniHandicap100 = Utility.StringToNullableByte(detrazioni.Substring(9, 1));

                this._FigliMaggiori3AnniHandicap50 = Utility.StringToNullableByte(detrazioni.Substring(10, 1));

                this._AltriFamiliari100 = Utility.StringToNullableByte(detrazioni.Substring(11, 1));

                this._AltriFamiliari50 = Utility.StringToNullableByte(detrazioni.Substring(12, 1));

                this._AddizionaleLombardiaVeneto = Utility.StringToNullableByte(detrazioni.Substring(13, 1));

                // Non vanno valorizzati i campi NonResidenteSchumacker e ConvDoppieImposizioni perché non sono presenti nella stringa detrazioni
            }

            #region private properties
            private System.Nullable<byte> _DetrazioniReddito;

            private System.Nullable<byte> _AgevolazionePensionati;

            private System.Nullable<byte> _ConiugeOFiglio;

            private System.Nullable<byte> _FigliMinori3AnniNoHandicap100;

            private System.Nullable<byte> _FigliMinori3AnniNoHandicap50;

            private System.Nullable<byte> _FigliMinori3AnniHandicap100;

            private System.Nullable<byte> _FigliMinori3AnniHandicap50;

            private System.Nullable<byte> _FigliMaggiori3AnniNoHandicap100;

            private System.Nullable<byte> _FigliMaggiori3AnniNoHandicap50;

            private System.Nullable<byte> _FigliMaggiori3AnniHandicap100;

            private System.Nullable<byte> _FigliMaggiori3AnniHandicap50;

            private System.Nullable<byte> _AltriFamiliari100;

            private System.Nullable<byte> _AltriFamiliari50;

            private System.Nullable<byte> _AddizionaleLombardiaVeneto;

            private System.Nullable<byte> _NonResidenteSchumacker;

            private System.Nullable<byte> _ConvDoppieImposizioni;

            private System.Nullable<System.DateTime> _DecorrenzaDetrazioneImposte;

            private bool _IsStorico;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> DetrazioniReddito { get { return _DetrazioniReddito; } set { _DetrazioniReddito = value; } }

            public System.Nullable<byte> AgevolazionePensionati { get { return _AgevolazionePensionati; } set { _AgevolazionePensionati = value; } }

            public System.Nullable<byte> ConiugeOFiglio { get { return _ConiugeOFiglio; } set { _ConiugeOFiglio = value; } }

            public System.Nullable<byte> FigliMinori3AnniNoHandicap100 { get { return _FigliMinori3AnniNoHandicap100; } set { _FigliMinori3AnniNoHandicap100 = value; } }

            public System.Nullable<byte> FigliMinori3AnniNoHandicap50 { get { return _FigliMinori3AnniNoHandicap50; } set { _FigliMinori3AnniNoHandicap50 = value; } }

            public System.Nullable<byte> FigliMinori3AnniHandicap100 { get { return _FigliMinori3AnniHandicap100; } set { _FigliMinori3AnniHandicap100 = value; } }

            public System.Nullable<byte> FigliMinori3AnniHandicap50 { get { return _FigliMinori3AnniHandicap50; } set { _FigliMinori3AnniHandicap50 = value; } }

            public System.Nullable<byte> FigliMaggiori3AnniNoHandicap100 { get { return _FigliMaggiori3AnniNoHandicap100; } set { _FigliMaggiori3AnniNoHandicap100 = value; } }

            public System.Nullable<byte> FigliMaggiori3AnniNoHandicap50 { get { return _FigliMaggiori3AnniNoHandicap50; } set { _FigliMaggiori3AnniNoHandicap50 = value; } }

            public System.Nullable<byte> FigliMaggiori3AnniHandicap100 { get { return _FigliMaggiori3AnniHandicap100; } set { _FigliMaggiori3AnniHandicap100 = value; } }

            public System.Nullable<byte> FigliMaggiori3AnniHandicap50 { get { return _FigliMaggiori3AnniHandicap50; } set { _FigliMaggiori3AnniHandicap50 = value; } }

            public System.Nullable<byte> AltriFamiliari100 { get { return _AltriFamiliari100; } set { _AltriFamiliari100 = value; } }

            public System.Nullable<byte> AltriFamiliari50 { get { return _AltriFamiliari50; } set { _AltriFamiliari50 = value; } }

            public System.Nullable<byte> AddizionaleLombardiaVeneto { get { return _AddizionaleLombardiaVeneto; } set { _AddizionaleLombardiaVeneto = value; } }

            public System.Nullable<byte> NonResidenteSchumacker { get { return _NonResidenteSchumacker; } set { _NonResidenteSchumacker = value; } }

            public System.Nullable<byte> ConvDoppieImposizioni { get { return _ConvDoppieImposizioni; } set { _ConvDoppieImposizioni = value; } }

            public System.Nullable<System.DateTime> DecorrenzaDetrazioneImposte { get { return _DecorrenzaDetrazioneImposte; } set { _DecorrenzaDetrazioneImposte = value; } }

            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiDetrazioni detr = (DatiDetrazioni)obj;
                try
                {
                    if (this.AddizionaleLombardiaVeneto != detr.AddizionaleLombardiaVeneto ||
                        this.AgevolazionePensionati != detr.AgevolazionePensionati ||
                        this.AltriFamiliari100 != detr.AltriFamiliari100 ||
                        this.AltriFamiliari50 != detr.AltriFamiliari50 ||
                        this.ConiugeOFiglio != detr.ConiugeOFiglio ||
                        this.DecorrenzaDetrazioneImposte != detr.DecorrenzaDetrazioneImposte ||
                        this.DetrazioniReddito != detr.DetrazioniReddito ||
                        this.FigliMaggiori3AnniHandicap100 != detr.FigliMaggiori3AnniHandicap100 ||
                        this.FigliMaggiori3AnniHandicap50 != detr.FigliMaggiori3AnniHandicap50 ||
                        this.FigliMaggiori3AnniNoHandicap100 != detr.FigliMaggiori3AnniNoHandicap100 ||
                        this.FigliMaggiori3AnniNoHandicap50 != detr.FigliMaggiori3AnniNoHandicap50 ||
                        this.FigliMinori3AnniHandicap100 != detr.FigliMinori3AnniHandicap100 ||
                        this.FigliMinori3AnniHandicap50 != detr.FigliMinori3AnniHandicap50 ||
                        this.FigliMinori3AnniNoHandicap100 != detr.FigliMinori3AnniNoHandicap100 ||
                        this.FigliMinori3AnniNoHandicap50 != detr.FigliMinori3AnniNoHandicap50 ||
                        this.NonResidenteSchumacker != detr.NonResidenteSchumacker ||
                        this.ConvDoppieImposizioni != detr.ConvDoppieImposizioni)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + (this.AddizionaleLombardiaVeneto != null ? this.AddizionaleLombardiaVeneto.GetHashCode() : 0);
                hash = (hash * 7) + (this.AgevolazionePensionati != null ? this.AgevolazionePensionati.GetHashCode() : 0);
                hash = (hash * 7) + (this.AltriFamiliari100 != null ? this.AltriFamiliari100.GetHashCode() : 0);
                hash = (hash * 7) + (this.AltriFamiliari50 != null ? this.AltriFamiliari50.GetHashCode() : 0);
                hash = (hash * 7) + (this.ConiugeOFiglio != null ? this.ConiugeOFiglio.GetHashCode() : 0);
                hash = (hash * 7) + (this.DecorrenzaDetrazioneImposte != null ? this.DecorrenzaDetrazioneImposte.GetHashCode() : 0);
                hash = (hash * 7) + (this.DetrazioniReddito != null ? this.DetrazioniReddito.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMaggiori3AnniHandicap100 != null ? this.FigliMaggiori3AnniHandicap100.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMaggiori3AnniHandicap50 != null ? this.FigliMaggiori3AnniHandicap50.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMaggiori3AnniNoHandicap100 != null ? this.FigliMaggiori3AnniNoHandicap100.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMaggiori3AnniNoHandicap50 != null ? this.FigliMaggiori3AnniNoHandicap50.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMinori3AnniHandicap100 != null ? this.FigliMinori3AnniHandicap100.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMinori3AnniHandicap50 != null ? this.FigliMinori3AnniHandicap50.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMinori3AnniNoHandicap100 != null ? this.FigliMinori3AnniNoHandicap100.GetHashCode() : 0);
                hash = (hash * 7) + (this.FigliMinori3AnniNoHandicap50 != null ? this.FigliMinori3AnniNoHandicap50.GetHashCode() : 0);
                hash = (hash * 7) + (this.NonResidenteSchumacker != null ? this.NonResidenteSchumacker.GetHashCode() : 0);
                hash = (hash * 7) + (this.ConvDoppieImposizioni != null ? this.ConvDoppieImposizioni.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion nested class
    }
}


