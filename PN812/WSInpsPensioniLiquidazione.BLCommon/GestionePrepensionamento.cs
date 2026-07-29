using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestionePrepensionamento
    {
        public static void GetDatiPrepensionamentoByIdPensione(long idPensione, out DatiPrepensionamento datiPrepensionamento)
        {
            Prepensionamento prepensionamento = null;
            datiPrepensionamento = null;
            DAGestionePrepensionamento.GetPrepensionamentoByIdPensione(idPensione, out prepensionamento);
            if (prepensionamento == null)
                return;
            datiPrepensionamento = new DatiPrepensionamento();
            Utility.ValorizzaOggetti(prepensionamento, datiPrepensionamento);
        }

        public static void SalvaDatiPrepensionamento(DatiPrepensionamento datiPrepensionamento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Prepensionamento prepensionamento = new Prepensionamento();
                Utility.ValorizzaOggetti(datiPrepensionamento, prepensionamento);
                DAGestionePrepensionamento.SalvaPrepensionamento(prepensionamento);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiPrepensionamentoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePrepensionamento.EliminaPrepensionamentoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region TOPPL03
        public static void InsertTOPPL03(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafica,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento, ref string messaggioVideo)
        {

            Anagrafica anagrafica = null;
            Pensione pensione = null;
            Istruttoria istruttoria = null;
            Prepensionamento prepensionamento = null;

            if (datiPensione != null)
            {
                pensione = new Pensione();
                Utility.ValorizzaOggetti(datiPensione, pensione);
            }

            if (datiAnagrafica != null)
            {
                anagrafica = new Anagrafica();
                Utility.ValorizzaOggetti(datiAnagrafica, anagrafica);
            }

            if (datiIstruttoria != null)
            {
                istruttoria = new Istruttoria();
                Utility.ValorizzaOggetti(datiIstruttoria, istruttoria);
            }

            if (datiPrepensionamento != null)
            {
                prepensionamento = new Prepensionamento();
                Utility.ValorizzaOggetti(datiPrepensionamento, prepensionamento);
            }

            string categoria = datiPensione.GetCodCategoria();
            categoria = categoria.Substring(1, 3);

            // Chiamata al metodo sotto DataCommon
            try
            {
                DAGestionePrepensionamento.InsertTOPPL03(pensione, anagrafica, istruttoria, prepensionamento, categoria);
            }
            catch (Exception ex)
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? ex.Message : messaggioVideo + " - " + ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
            }
        }

        public static void UpdateTOPPL03(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici,
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria, GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento, ref string messaggioVideo)
        {
            Anagrafica anagrafica = null;
            Pensione pensione = null;
            Istruttoria istruttoria = null;
            Prepensionamento prepensionamento = null;

            if (datiPensione != null)
            {
                pensione = new Pensione();
                Utility.ValorizzaOggetti(datiPensione, pensione);
            }

            if (datiAnagrafici != null)
            {
                anagrafica = new Anagrafica();
                Utility.ValorizzaOggetti(datiAnagrafici, anagrafica);
            }

            if (datiIstruttoria != null)
            {
                istruttoria = new Istruttoria();
                Utility.ValorizzaOggetti(datiIstruttoria, istruttoria);
            }

            if (datiPrepensionamento != null)
            {
                prepensionamento = new Prepensionamento();
                Utility.ValorizzaOggetti(datiPrepensionamento, prepensionamento);
            }

            string categoria = datiPensione.GetCodCategoria();
            categoria = categoria.Substring(1, 3);

            // Chiamata al metodo sotto DataCommon
            try
            {
                DAGestionePrepensionamento.UpdateTOPPL03(pensione, anagrafica, istruttoria, prepensionamento, categoria);
            }
            catch (Exception ex)
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? ex.Message : messaggioVideo + " - " + ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
            }
        }

        public static void SelectTOPPL03(GestionePensione.DatiPensione datiPensione, out List<GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento, ref string messaggioVideo)
        {
            listaDatiPrepensionamento = null;

            string categoria = datiPensione.GetCodCategoria();
            categoria = categoria.Substring(1, 3);

            Pensione pensione = new Pensione();
            Utility.ValorizzaOggetti(datiPensione, pensione);

            List<Prepensionamento> listPrepensionamento = null;


            // Chiamata al metodo sotto DataCommon
            try
            {
                DAGestionePrepensionamento.SelectTOPPL03(pensione, categoria, out listPrepensionamento);
            }
            catch (Exception ex)
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? ex.Message : messaggioVideo + " - " + ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
            }

            if (listPrepensionamento != null && listPrepensionamento.Count > 0)
            {
                listaDatiPrepensionamento = new List<DatiPrepensionamento>();

                foreach (Prepensionamento prep in listPrepensionamento)
                {
                    GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = new DatiPrepensionamento();
                    Utility.ValorizzaOggetti(prep, datiPrepensionamento);

                    listaDatiPrepensionamento.Add(datiPrepensionamento);
                }
            }
        }

        public static void DeleteTOPPL03(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            Pensione pensione = null;

            if (datiPensione != null)
            {
                pensione = new Pensione();
                Utility.ValorizzaOggetti(datiPensione, pensione);
            }

            string categoria = datiPensione.GetCodCategoria();
            categoria = categoria.Substring(1, 3);

            // Chiamata al metodo sotto DataCommon
            try
            {
                DAGestionePrepensionamento.DeleteTOPPL03(pensione, categoria);
            }
            catch (Exception ex)
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? ex.Message : messaggioVideo + " - " + ex.Message;
                INPS.DNA.Logging.Logger.LogException(ex);
            }
        }
        #endregion TOPPL03

        #region nested class

        public class DatiPrepensionamento
        {
            #region private properties

            private long _IdPensione;
            private System.Nullable<int> _CodiceLegge;
            private System.Nullable<int> _SettimaneUtiliDiritto;
            private System.Nullable<int> _SettimaneUtiliMisura;
            private System.Nullable<int> _SettimaneMaggioreAnzianita;
            private System.Nullable<decimal> _OnereMancataContribuzione;
            private System.Nullable<long> _CodiceAzienda;
            private System.Nullable<System.DateTime> _CessazioneBeneficioPrepensionamento;
            private System.Nullable<int> _SettimaneAmianto;
            private System.Nullable<System.DateTime> _CessazioneAmianto;

            #endregion private properties

            #region public properties

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int? CodiceLegge { get { return _CodiceLegge; } set { _CodiceLegge = value; } }
            public int? SettimaneUtiliDiritto { get { return _SettimaneUtiliDiritto; } set { _SettimaneUtiliDiritto = value; } }
            public int? SettimaneUtiliMisura { get { return _SettimaneUtiliMisura; } set { _SettimaneUtiliMisura = value; } }
            public int? SettimaneMaggioreAnzianita { get { return _SettimaneMaggioreAnzianita; } set { _SettimaneMaggioreAnzianita = value; } }
            public decimal? OnereMancataContribuzione { get { return _OnereMancataContribuzione; } set { _OnereMancataContribuzione = value; } }
            public long? CodiceAzienda { get { return _CodiceAzienda; } set { _CodiceAzienda = value; } }
            public DateTime? CessazioneBeneficioPrepensionamento { get { return _CessazioneBeneficioPrepensionamento; } set { _CessazioneBeneficioPrepensionamento = value; } }
            public int? SettimaneAmianto { get { return _SettimaneAmianto; } set { _SettimaneAmianto = value; } }
            public DateTime? CessazioneAmianto { get { return _CessazioneAmianto; } set { _CessazioneAmianto = value; } }

            #endregion public properties

            public bool IsDatiPrepensionamentoNull()
            {
                if (!this._CodiceLegge.HasValue && !this._SettimaneUtiliDiritto.HasValue && !this._SettimaneUtiliMisura.HasValue &&
                    !this._SettimaneMaggioreAnzianita.HasValue && !this._OnereMancataContribuzione.HasValue && !this._CodiceAzienda.HasValue &&
                    !this._CessazioneBeneficioPrepensionamento.HasValue && !this._SettimaneAmianto.HasValue && !this._CessazioneAmianto.HasValue)
                    return true;
                else
                    return false;
            }

            public override bool Equals(object obj)
            {
                DatiPrepensionamento prepensionamento = obj as DatiPrepensionamento;
                try
                {
                    if (this._CessazioneAmianto != prepensionamento._CessazioneAmianto ||
                         this._CessazioneBeneficioPrepensionamento != prepensionamento._CessazioneBeneficioPrepensionamento ||
                         this._CodiceAzienda.GetValueOrDefault() != prepensionamento._CodiceAzienda.GetValueOrDefault() ||
                         this._CodiceLegge.GetValueOrDefault() != prepensionamento._CodiceLegge.GetValueOrDefault() ||
                         this._OnereMancataContribuzione.GetValueOrDefault() != prepensionamento._OnereMancataContribuzione.GetValueOrDefault() ||
                         this._SettimaneAmianto.GetValueOrDefault() != prepensionamento._SettimaneAmianto.GetValueOrDefault() ||
                         this._SettimaneMaggioreAnzianita.GetValueOrDefault() != prepensionamento._SettimaneMaggioreAnzianita.GetValueOrDefault() ||
                         this._SettimaneUtiliDiritto.GetValueOrDefault() != prepensionamento._SettimaneUtiliDiritto.GetValueOrDefault() ||
                         this._SettimaneUtiliMisura.GetValueOrDefault() != prepensionamento._SettimaneUtiliMisura.GetValueOrDefault())
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion nested class
    }
}
