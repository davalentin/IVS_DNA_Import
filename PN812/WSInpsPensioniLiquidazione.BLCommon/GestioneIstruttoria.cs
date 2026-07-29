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
    public class GestioneIstruttoria
    {
        public static void GetIstruttoriaByIdPensione(Int64 idPensione, out DatiIstruttoria datiIstruttoria)
        {
            Istruttoria istruttoria = null;
            datiIstruttoria = null;
            DAGestioneIstruttoria.GetIstruttoriaByIdPensione(idPensione, out istruttoria);
            if (istruttoria == null)
                return;
            datiIstruttoria = new DatiIstruttoria();
            Utility.ValorizzaOggetti(istruttoria, datiIstruttoria);
        }

        public static void SalvaIstruttoria(long idPensione, DatiIstruttoria datiIstruttoria)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Istruttoria istruttoria = new Istruttoria();
                Utility.ValorizzaOggetti(datiIstruttoria, istruttoria);
                istruttoria.IdPensione = idPensione;
                DAGestioneIstruttoria.SalvaIstruttoria(istruttoria);
                transactionScope.Complete();
            }
        }

        public static void EliminaIstruttoriaByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static bool IsIstruttoriaNull(DatiIstruttoria datiIstruttoria)
        {
            if (!datiIstruttoria.ScadenzaRevisioneSanitaria.HasValue &&
                !datiIstruttoria.Legge44997.HasValue &&
                !datiIstruttoria.CodiceMobilita.HasValue &&
                !datiIstruttoria.NRiconoscimentiInvalidita.HasValue &&
                !datiIstruttoria.NSettGodimentoAssegno.HasValue &&
                !datiIstruttoria.ClasseInvalidita1Codice.HasValue &&
                !datiIstruttoria.ClasseInvalidita2Codice.HasValue &&
                !datiIstruttoria.NSettimaneOBG.HasValue &&
                !datiIstruttoria.NContributiVolontari.HasValue &&
                !datiIstruttoria.NContributiVVAnzianita.HasValue &&
                !datiIstruttoria.NContributiUtiliLavoratoriAutonomi.HasValue &&
                !datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi.HasValue &&
                !datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi.HasValue &&
                !datiIstruttoria.Requisiti781Settimane.HasValue &&
                !datiIstruttoria.AccertamentoAutomatico.HasValue &&
                !datiIstruttoria.CodiceOpzioneRiliquidazione.HasValue &&
                !datiIstruttoria.DataDomandaOpzione.HasValue &&
                !datiIstruttoria.DecorrenzaOpzione.HasValue &&
                !datiIstruttoria.CodiceRequisitiParticolari.HasValue &&
                !datiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue &&
                !datiIstruttoria.CodiceP18PrecedentePensione.HasValue &&
                !datiIstruttoria.SedePrecedentePensione.HasValue &&
                !datiIstruttoria.CertificatoPrecedentePensione.HasValue &&
                !datiIstruttoria.DecorrenzaCaricoPrecedentePensione.HasValue &&
                (String.IsNullOrEmpty(datiIstruttoria.CodiceNaturaPrecedentePensione)) &&
                !datiIstruttoria.FacoltaComputoPrecedentePensione.HasValue &&
                !datiIstruttoria.CodiceComunicazioneCampo1.HasValue &&
                !datiIstruttoria.CodiceComunicazioneCampo2.HasValue &&
                !datiIstruttoria.CodiceComunicazioneCampo3.HasValue &&
                !datiIstruttoria.CodiceComunicazioneCampo4.HasValue &&
                !datiIstruttoria.CodiceDomandaRicorso.HasValue &&
                !datiIstruttoria.CodiceCdCmMr.HasValue &&
                !datiIstruttoria.CodiceContrattoEquiparato.HasValue &&
                !datiIstruttoria.CodiceLivelloEquip.HasValue &&
                (String.IsNullOrEmpty(datiIstruttoria.CodiceArt1Legge5990)) &&
                !datiIstruttoria.DecorrenzaOriginariaAltraPensione.HasValue &&
                !datiIstruttoria.ImportoAdeguataAoi.HasValue &&
                !datiIstruttoria.ImportoPagamentoAoi.HasValue &&
                (String.IsNullOrEmpty(datiIstruttoria.CodiceCentroOperativo)) &&
                (String.IsNullOrEmpty(datiIstruttoria.CodPosizioneLavoro)) &&
                !datiIstruttoria.ScadenzaRevisioneAssegno.HasValue &&
                !datiIstruttoria.PensioneSurroga.HasValue &&
                (String.IsNullOrEmpty(datiIstruttoria.CodiceASL)) &&
                !datiIstruttoria.TipoPensioneExInpdai.HasValue &&
                !datiIstruttoria.RiliquidazionePostCristallizzazione.HasValue &&
                !datiIstruttoria.CodiceImporto.HasValue &&
                !datiIstruttoria.CodiceLiquidazione.HasValue &&
                !datiIstruttoria.CodiceIsola.HasValue &&
                String.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione) &&
                !datiIstruttoria.NSettimaneOI.HasValue)
            {
                return true;
            }
            else
                return false;
        }

        #region nested class
        public class DatiIstruttoria
        {
            public DatiIstruttoria()
            { }
            public DatiIstruttoria(System.Nullable<System.DateTime> scadenzaRevisioneSanitaria,
                System.Nullable<byte> legge44997, System.Nullable<byte> codiceMobilita,
                System.Nullable<byte> nRiconoscimentiInvalidita, System.Nullable<int> nSettGodimentoAssegno,
                System.Nullable<short> classeInvalidita1Codice, System.Nullable<short> classeInvalidita2Codice,
                System.Nullable<int> nSettimaneOBG, System.Nullable<int> nContributiVolontari,
                System.Nullable<int> nContributiVVAnzianita, System.Nullable<int> nContributiUtiliLavoratoriAutonomi,
                System.Nullable<int> nSettimaneVVDirittoLavoratoriAutonomi,
                System.Nullable<int> nSettimaneVVMisuraLavoratoriAutonomi, System.Nullable<bool> requisiti781Settimane,
                System.Nullable<bool> accertamentoAutomatico, System.Nullable<byte> codiceOpzioneRiliquidazione,
                System.Nullable<System.DateTime> dataDomandaOpzione, System.Nullable<System.DateTime> decorrenzaOpzione,
                System.Nullable<byte> codiceRequisitiParticolari, System.Nullable<long> codiceParticolareSoggettoDerogato,
                System.Nullable<short> codiceP18PrecedentePensione, System.Nullable<short> sedePrecedentePensione,
                System.Nullable<int> certificatoPrecedentePensione,
                System.Nullable<System.DateTime> decorrenzaCaricoPrecedentePensione,
                string codiceNaturaPrecedentePEnsione,
                System.Nullable<char> facoltaComputoPrecedentePensione, 
                System.Nullable<byte> codiceComunicazioneCampo1,
                System.Nullable<char> codiceComunicazioneCampo2, System.Nullable<char> codiceComunicazioneCampo3,
                System.Nullable<byte> codiceComunicazioneCampo4, System.Nullable<byte> codiceDomandaRicorso,
                System.Nullable<byte> codiceCdCmMr, short? codiceContrattoEquiparato, short? codiceLivelloEquip,
                string codiceArt1Legge5990, System.Nullable<System.DateTime> decorrenzaOriginariaAltraPensione,
                System.Nullable<decimal> importoAdeguataAoi, System.Nullable<decimal> importoPagamentoAoi,
                string codiceCentroOperativo, string codPosizioneLavoro, System.Nullable<System.DateTime> scadenzaRevisioneAssegno,
                System.Nullable<char> pensioneSurroga, string codiceASL, System.Nullable<byte> tipoPensioneExInpdai,
                System.Nullable<char> riliquidazionePostCristallizzazione, System.Nullable<char> codiceImporto,
                System.Nullable<char> codiceLiquidazione, System.Nullable<byte> codiceIsola,
                string modalitaLiquidazione, bool? provvisoria, char? tipoCalcoloVincenteUnicarpe, decimal? riduzioneAssegno, short? codiceAziendaEditoria,
                short? codiceAziendaEditoriaPerTipo0171, short? codiceAziendaEditoriaPerTipo0179, short? codiceAziendaEditoriaLetteraB, bool? trattamentoDisagi, 
                short? codiceEnte, System.Nullable<char> I_AGGANCIO, System.Nullable<int> I_SETTEST, System.Nullable<byte> tipoCalcoloPrecedente, System.Nullable<byte> GP1AF08,
                System.Nullable<int> nSettimaneOI)
            {
                this._ScadenzaRevisioneSanitaria = scadenzaRevisioneSanitaria;

                this._Legge44997 = legge44997;

                this._CodiceMobilita = codiceMobilita;

                this._NRiconoscimentiInvalidita = nRiconoscimentiInvalidita;

                this._NSettGodimentoAssegno = nSettGodimentoAssegno;

                this._ClasseInvalidita1Codice = classeInvalidita1Codice;

                this._ClasseInvalidita2Codice = classeInvalidita2Codice;

                this._NSettimaneOBG = nSettimaneOBG;

                this._NContributiVolontari = nContributiVolontari;

                this._NContributiVVAnzianita = nContributiVVAnzianita;

                this._NContributiUtiliLavoratoriAutonomi = nContributiUtiliLavoratoriAutonomi;

                this._NSettimaneVVDirittoLavoratoriAutonomi = nSettimaneVVDirittoLavoratoriAutonomi;

                this._NSettimaneVVMisuraLavoratoriAutonomi = nSettimaneVVMisuraLavoratoriAutonomi;

                this._Requisiti781Settimane = requisiti781Settimane;

                this._AccertamentoAutomatico = accertamentoAutomatico;

                this._CodiceOpzioneRiliquidazione = codiceOpzioneRiliquidazione;

                this._DataDomandaOpzione = dataDomandaOpzione;

                this._DecorrenzaOpzione = decorrenzaOpzione;

                this._CodiceRequisitiParticolari = codiceRequisitiParticolari;

                this._CodiceParticolareSoggettoDerogato = codiceParticolareSoggettoDerogato;

                this._CodiceP18PrecedentePensione = codiceP18PrecedentePensione;

                this._SedePrecedentePensione = sedePrecedentePensione;

                this._CertificatoPrecedentePensione = certificatoPrecedentePensione;

                this._DecorrenzaCaricoPrecedentePensione = decorrenzaCaricoPrecedentePensione;

                this._CodiceNaturaPrecedentePensione = codiceNaturaPrecedentePEnsione;

                this._FacoltaComputoPrecedentePensione = facoltaComputoPrecedentePensione;

                this._CodiceComunicazioneCampo1 = codiceComunicazioneCampo1;

                this._CodiceComunicazioneCampo2 = codiceComunicazioneCampo2;

                this._CodiceComunicazioneCampo3 = codiceComunicazioneCampo3;

                this._CodiceComunicazioneCampo4 = codiceComunicazioneCampo4;

                this._CodiceDomandaRicorso = codiceDomandaRicorso;

                this._CodiceCdCmMr = codiceCdCmMr;

                this._CodiceContrattoEquiparato = codiceContrattoEquiparato;

                this._CodiceLivelloEquip = codiceLivelloEquip;

                this._CodiceArt1Legge5990 = codiceArt1Legge5990;

                this._DecorrenzaOriginariaAltraPensione = decorrenzaOriginariaAltraPensione;

                this._ImportoAdeguataAoi = importoAdeguataAoi;

                this._ImportoPagamentoAoi = importoPagamentoAoi;

                this._CodiceCentroOperativo = codiceCentroOperativo;

                this._CodPosizioneLavoro = codPosizioneLavoro;

                this._ScadenzaRevisioneAssegno = scadenzaRevisioneAssegno;

                this._PensioneSurroga = pensioneSurroga;

                this._CodiceASL = codiceASL;

                this._TipoPensioneExInpdai = tipoPensioneExInpdai;

                this._RiliquidazionePostCristallizzazione = riliquidazionePostCristallizzazione;

                this._CodiceImporto = codiceImporto;

                this._CodiceLiquidazione = codiceLiquidazione;

                this._CodiceIsola = codiceIsola;

                this._Provvisoria = provvisoria;

                this._TipoCalcoloVincenteUnicarpe = tipoCalcoloVincenteUnicarpe;

                this._RiduzioneAssegno = riduzioneAssegno;

                this._CodiceAziendaEditoria = codiceAziendaEditoria;

                this._CodiceAziendaEditoriaPerTipo0171 = codiceAziendaEditoriaPerTipo0171;

                this._TrattamentoDisagi = trattamentoDisagi;

                this._CodiceEnte = codiceEnte;

                this._CodiceAziendaEditoriaLetteraB = codiceAziendaEditoriaLetteraB;

                this._I_AGGANCIO = I_AGGANCIO;

                this.I_SETTEST = I_SETTEST;

                this._TipoCalcoloPrecedente = tipoCalcoloPrecedente;

                this._GP1AF08 = GP1AF08;

                this._NSettimaneOI = nSettimaneOI;
            }

            #region private properties
            private System.Nullable<System.DateTime> _ScadenzaRevisioneSanitaria;

            private System.Nullable<byte> _Legge44997;

            private System.Nullable<byte> _CodiceMobilita;

            private System.Nullable<byte> _NRiconoscimentiInvalidita;

            private System.Nullable<int> _NSettGodimentoAssegno;

            private System.Nullable<short> _ClasseInvalidita1Codice;

            private System.Nullable<short> _ClasseInvalidita2Codice;

            private System.Nullable<int> _NSettimaneOBG;

            private System.Nullable<int> _NContributiVolontari;

            private System.Nullable<int> _NContributiVVAnzianita;

            private System.Nullable<int> _NContributiUtiliLavoratoriAutonomi;

            private System.Nullable<int> _NSettimaneVVDirittoLavoratoriAutonomi;

            private System.Nullable<int> _NSettimaneVVMisuraLavoratoriAutonomi;

            private System.Nullable<bool> _Requisiti781Settimane;

            private System.Nullable<bool> _AccertamentoAutomatico;

            private System.Nullable<byte> _CodiceOpzioneRiliquidazione;

            private System.Nullable<System.DateTime> _DataDomandaOpzione;

            private System.Nullable<System.DateTime> _DecorrenzaOpzione;

            private System.Nullable<byte> _CodiceRequisitiParticolari;

            private System.Nullable<long> _CodiceParticolareSoggettoDerogato;

            private System.Nullable<short> _CodiceP18PrecedentePensione;

            private System.Nullable<short> _SedePrecedentePensione;

            private System.Nullable<int> _CertificatoPrecedentePensione;

            private System.Nullable<System.DateTime> _DecorrenzaCaricoPrecedentePensione;

            private string _CodiceNaturaPrecedentePensione;

            private System.Nullable<char> _FacoltaComputoPrecedentePensione;

            private System.Nullable<byte> _CodiceComunicazioneCampo1;

            private System.Nullable<char> _CodiceComunicazioneCampo2;

            private System.Nullable<char> _CodiceComunicazioneCampo3;

            private System.Nullable<byte> _CodiceComunicazioneCampo4;

            private System.Nullable<byte> _CodiceDomandaRicorso;

            private System.Nullable<byte> _CodiceCdCmMr;

            private short? _CodiceContrattoEquiparato;

            private short? _CodiceLivelloEquip;

            private string _CodiceArt1Legge5990;

            private System.Nullable<System.DateTime> _DecorrenzaOriginariaAltraPensione;

            private System.Nullable<decimal> _ImportoAdeguataAoi;

            private System.Nullable<decimal> _ImportoPagamentoAoi;

            private string _CodiceCentroOperativo;

            private string _CodPosizioneLavoro;

            private System.Nullable<System.DateTime> _ScadenzaRevisioneAssegno;

            private System.Nullable<char> _PensioneSurroga;

            private string _CodiceASL;

            private System.Nullable<byte> _TipoPensioneExInpdai;

            private System.Nullable<char> _RiliquidazionePostCristallizzazione;

            private System.Nullable<char> _CodiceImporto;

            private System.Nullable<char> _CodiceLiquidazione;

            private System.Nullable<byte> _CodiceIsola;

            private string _ModalitaLiquidazione;

            private bool? _Provvisoria;

            private System.Nullable<char> _TipoCalcoloVincenteUnicarpe;

            private decimal? _RiduzioneAssegno;

            private short? _CodiceAziendaEditoria;

            private short? _CodiceAziendaEditoriaPerTipo0171;

            private short? _CodiceAziendaEditoriaPerTipo0179;
            
            private bool? _TrattamentoDisagi;

            private short? _CodiceEnte;

            private short? _CodiceAziendaEditoriaLetteraB;

            private System.Nullable<char> _I_AGGANCIO;

            private System.Nullable<int> _I_SETTEST;

            private System.Nullable<byte> _TipoCalcoloPrecedente;

            private System.Nullable<byte> _GP1AF08;

            private System.Nullable<int> _NSettimaneOI;
            #endregion private properties

            #region public properties
            public System.Nullable<System.DateTime> ScadenzaRevisioneSanitaria { get { return _ScadenzaRevisioneSanitaria; } set { _ScadenzaRevisioneSanitaria = value; } }

            public System.Nullable<byte> Legge44997 { get { return _Legge44997; } set { _Legge44997 = value; } }

            public System.Nullable<byte> CodiceMobilita { get { return _CodiceMobilita; } set { _CodiceMobilita = value; } }

            public System.Nullable<byte> NRiconoscimentiInvalidita { get { return _NRiconoscimentiInvalidita; } set { _NRiconoscimentiInvalidita = value; } }

            public System.Nullable<int> NSettGodimentoAssegno { get { return _NSettGodimentoAssegno; } set { _NSettGodimentoAssegno = value; } }

            public System.Nullable<short> ClasseInvalidita1Codice { get { return _ClasseInvalidita1Codice; } set { _ClasseInvalidita1Codice = value; } }

            public System.Nullable<short> ClasseInvalidita2Codice { get { return _ClasseInvalidita2Codice; } set { _ClasseInvalidita2Codice = value; } }

            public System.Nullable<int> NSettimaneOBG { get { return _NSettimaneOBG; } set { _NSettimaneOBG = value; } }

            public System.Nullable<int> NContributiVolontari { get { return _NContributiVolontari; } set { _NContributiVolontari = value; } }

            public System.Nullable<int> NContributiVVAnzianita { get { return _NContributiVVAnzianita; } set { _NContributiVVAnzianita = value; } }

            public System.Nullable<int> NContributiUtiliLavoratoriAutonomi { get { return _NContributiUtiliLavoratoriAutonomi; } set { _NContributiUtiliLavoratoriAutonomi = value; } }

            public System.Nullable<int> NSettimaneVVDirittoLavoratoriAutonomi { get { return _NSettimaneVVDirittoLavoratoriAutonomi; } set { _NSettimaneVVDirittoLavoratoriAutonomi = value; } }

            public System.Nullable<int> NSettimaneVVMisuraLavoratoriAutonomi { get { return _NSettimaneVVMisuraLavoratoriAutonomi; } set { _NSettimaneVVMisuraLavoratoriAutonomi = value; } }

            public System.Nullable<bool> Requisiti781Settimane { get { return _Requisiti781Settimane; } set { _Requisiti781Settimane = value; } }

            public System.Nullable<bool> AccertamentoAutomatico { get { return _AccertamentoAutomatico; } set { _AccertamentoAutomatico = value; } }

            public System.Nullable<byte> CodiceOpzioneRiliquidazione { get { return _CodiceOpzioneRiliquidazione; } set { _CodiceOpzioneRiliquidazione = value; } }

            public System.Nullable<System.DateTime> DataDomandaOpzione { get { return _DataDomandaOpzione; } set { _DataDomandaOpzione = value; } }

            public System.Nullable<System.DateTime> DecorrenzaOpzione { get { return _DecorrenzaOpzione; } set { _DecorrenzaOpzione = value; } }

            public System.Nullable<byte> CodiceRequisitiParticolari { get { return _CodiceRequisitiParticolari; } set { _CodiceRequisitiParticolari = value; } }

            public System.Nullable<long> CodiceParticolareSoggettoDerogato { get { return _CodiceParticolareSoggettoDerogato; } set { _CodiceParticolareSoggettoDerogato = value; } }

            public System.Nullable<short> CodiceP18PrecedentePensione { get { return _CodiceP18PrecedentePensione; } set { _CodiceP18PrecedentePensione = value; } }

            public System.Nullable<short> SedePrecedentePensione { get { return _SedePrecedentePensione; } set { _SedePrecedentePensione = value; } }

            public System.Nullable<int> CertificatoPrecedentePensione { get { return _CertificatoPrecedentePensione; } set { _CertificatoPrecedentePensione = value; } }

            public System.Nullable<System.DateTime> DecorrenzaCaricoPrecedentePensione { get { return _DecorrenzaCaricoPrecedentePensione; } set { _DecorrenzaCaricoPrecedentePensione = value; } }
            
            public string CodiceNaturaPrecedentePensione { get { return _CodiceNaturaPrecedentePensione; } set { _CodiceNaturaPrecedentePensione = value; } }
            
            public System.Nullable<char> FacoltaComputoPrecedentePensione { get { return _FacoltaComputoPrecedentePensione; } set { _FacoltaComputoPrecedentePensione = value; } }

            public System.Nullable<byte> CodiceComunicazioneCampo1 { get { return _CodiceComunicazioneCampo1; } set { _CodiceComunicazioneCampo1 = value; } }

            public System.Nullable<char> CodiceComunicazioneCampo2 { get { return _CodiceComunicazioneCampo2; } set { _CodiceComunicazioneCampo2 = value; } }

            public System.Nullable<char> CodiceComunicazioneCampo3 { get { return _CodiceComunicazioneCampo3; } set { _CodiceComunicazioneCampo3 = value; } }

            public System.Nullable<byte> CodiceComunicazioneCampo4 { get { return _CodiceComunicazioneCampo4; } set { _CodiceComunicazioneCampo4 = value; } }

            public System.Nullable<byte> CodiceDomandaRicorso { get { return _CodiceDomandaRicorso; } set { _CodiceDomandaRicorso = value; } }

            public System.Nullable<byte> CodiceCdCmMr { get { return _CodiceCdCmMr; } set { _CodiceCdCmMr = value; } }

            public short? CodiceContrattoEquiparato { get { return _CodiceContrattoEquiparato; } set { _CodiceContrattoEquiparato = value; } }

            public short? CodiceLivelloEquip { get { return _CodiceLivelloEquip; } set { _CodiceLivelloEquip = value; } }

            public string CodiceArt1Legge5990 { get { return _CodiceArt1Legge5990; } set { _CodiceArt1Legge5990 = value; } }

            public System.Nullable<System.DateTime> DecorrenzaOriginariaAltraPensione { get { return _DecorrenzaOriginariaAltraPensione; } set { _DecorrenzaOriginariaAltraPensione = value; } }

            public System.Nullable<decimal> ImportoAdeguataAoi { get { return _ImportoAdeguataAoi; } set { _ImportoAdeguataAoi = value; } }

            public System.Nullable<decimal> ImportoPagamentoAoi { get { return _ImportoPagamentoAoi; } set { _ImportoPagamentoAoi = value; } }

            public string CodiceCentroOperativo { get { return _CodiceCentroOperativo; } set { _CodiceCentroOperativo = value; } }

            public string CodPosizioneLavoro { get { return _CodPosizioneLavoro; } set { _CodPosizioneLavoro = value; } }

            public System.Nullable<System.DateTime> ScadenzaRevisioneAssegno { get { return _ScadenzaRevisioneAssegno; } set { _ScadenzaRevisioneAssegno = value; } }

            public System.Nullable<char> PensioneSurroga { get { return _PensioneSurroga; } set { _PensioneSurroga = value; } }

            public string CodiceASL { get { return _CodiceASL; } set { _CodiceASL = value; } }

            public System.Nullable<byte> TipoPensioneExInpdai { get { return _TipoPensioneExInpdai; } set { _TipoPensioneExInpdai = value; } }

            public System.Nullable<char> RiliquidazionePostCristallizzazione { get { return _RiliquidazionePostCristallizzazione; } set { _RiliquidazionePostCristallizzazione = value; } }

            public System.Nullable<char> CodiceImporto { get { return _CodiceImporto; } set { _CodiceImporto = value; } }

            public System.Nullable<char> CodiceLiquidazione { get { return _CodiceLiquidazione; } set { _CodiceLiquidazione = value; } }

            public System.Nullable<byte> CodiceIsola { get { return _CodiceIsola; } set { _CodiceIsola = value; } }

            public string ModalitaLiquidazione { get { return _ModalitaLiquidazione; } set { _ModalitaLiquidazione = value; } }

            public bool? Provvisoria { get { return _Provvisoria; } set { _Provvisoria = value; } }

            public System.Nullable<char> TipoCalcoloVincenteUnicarpe { get { return _TipoCalcoloVincenteUnicarpe; } set { _TipoCalcoloVincenteUnicarpe = value; } }

            public decimal? RiduzioneAssegno { get { return _RiduzioneAssegno; } set { _RiduzioneAssegno = value; } }

            public short? CodiceAziendaEditoria { get { return _CodiceAziendaEditoria; } set { _CodiceAziendaEditoria = value; } }

            public short? CodiceAziendaEditoriaPerTipo0171 { get { return _CodiceAziendaEditoriaPerTipo0171; } set { _CodiceAziendaEditoriaPerTipo0171 = value; } }

            public short? CodiceAziendaEditoriaPerTipo0179 { get { return _CodiceAziendaEditoriaPerTipo0179; } set { _CodiceAziendaEditoriaPerTipo0179 = value; } }

            public bool? TrattamentoDisagi { get { return _TrattamentoDisagi; } set { _TrattamentoDisagi = value; } }

            public short? CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }

            public short? CodiceAziendaEditoriaLetteraB { get { return _CodiceAziendaEditoriaLetteraB; } set { _CodiceAziendaEditoriaLetteraB = value; } }

            public System.Nullable<char> I_AGGANCIO { get { return _I_AGGANCIO; } set { _I_AGGANCIO = value; } }

            public System.Nullable<int> I_SETTEST { get { return _I_SETTEST; } set { _I_SETTEST = value; } }

            public System.Nullable<byte> TipoCalcoloPrecedente { get { return _TipoCalcoloPrecedente; } set { _TipoCalcoloPrecedente = value; } }

            public System.Nullable<byte> GP1AF08 { get { return _GP1AF08; } set { _GP1AF08 = value; } }

            public System.Nullable<int> NSettimaneOI { get { return _NSettimaneOI; } set { _NSettimaneOI = value; } }
            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiIstruttoria istruttoria = (DatiIstruttoria)obj;
                try
                {
                    if (this._ScadenzaRevisioneSanitaria != istruttoria._ScadenzaRevisioneSanitaria ||
                        this._Legge44997 != istruttoria._Legge44997 ||
                        this._CodiceMobilita != istruttoria._CodiceMobilita ||
                        this._NRiconoscimentiInvalidita != istruttoria._NRiconoscimentiInvalidita ||
                        this._NSettGodimentoAssegno != istruttoria._NSettGodimentoAssegno ||
                        this._ClasseInvalidita1Codice != istruttoria._ClasseInvalidita1Codice ||
                        this._ClasseInvalidita2Codice != istruttoria._ClasseInvalidita2Codice ||
                        this._NSettimaneOBG != istruttoria._NSettimaneOBG ||
                        this._NContributiVolontari != istruttoria._NContributiVolontari ||
                        this._NContributiVVAnzianita != istruttoria._NContributiVVAnzianita ||
                        this._NContributiUtiliLavoratoriAutonomi != istruttoria._NContributiUtiliLavoratoriAutonomi ||
                        this._NSettimaneVVDirittoLavoratoriAutonomi != istruttoria._NSettimaneVVDirittoLavoratoriAutonomi ||
                        this._NSettimaneVVMisuraLavoratoriAutonomi != istruttoria._NSettimaneVVMisuraLavoratoriAutonomi ||
                        this._Requisiti781Settimane != istruttoria._Requisiti781Settimane ||
                        this._AccertamentoAutomatico != istruttoria._AccertamentoAutomatico ||
                        this._CodiceOpzioneRiliquidazione != istruttoria._CodiceOpzioneRiliquidazione ||
                        this._DataDomandaOpzione != istruttoria._DataDomandaOpzione ||
                        this._DecorrenzaOpzione != istruttoria._DecorrenzaOpzione ||
                        this._CodiceRequisitiParticolari != istruttoria._CodiceRequisitiParticolari ||
                        this._CodiceParticolareSoggettoDerogato != istruttoria._CodiceParticolareSoggettoDerogato ||
                        this._CodiceP18PrecedentePensione != istruttoria._CodiceP18PrecedentePensione ||
                        this._SedePrecedentePensione != istruttoria._SedePrecedentePensione ||
                        this._CertificatoPrecedentePensione != istruttoria._CertificatoPrecedentePensione ||
                        this._DecorrenzaCaricoPrecedentePensione != istruttoria._DecorrenzaCaricoPrecedentePensione ||
                        this._CodiceNaturaPrecedentePensione != istruttoria._CodiceNaturaPrecedentePensione ||
                        this._FacoltaComputoPrecedentePensione != istruttoria._FacoltaComputoPrecedentePensione ||
                        this._CodiceComunicazioneCampo1 != istruttoria._CodiceComunicazioneCampo1 ||
                        this._CodiceComunicazioneCampo2 != istruttoria._CodiceComunicazioneCampo2 ||
                        this._CodiceComunicazioneCampo3 != istruttoria._CodiceComunicazioneCampo3 ||
                        this._CodiceComunicazioneCampo4 != istruttoria._CodiceComunicazioneCampo4 ||
                        this._CodiceDomandaRicorso != istruttoria._CodiceDomandaRicorso ||
                        this._CodiceCdCmMr != istruttoria._CodiceCdCmMr ||
                        this._CodiceContrattoEquiparato != istruttoria._CodiceContrattoEquiparato ||
                        this._CodiceLivelloEquip != istruttoria._CodiceLivelloEquip ||
                        (this._CodiceArt1Legge5990 != null ? this._CodiceArt1Legge5990.Trim() : null) != (istruttoria._CodiceArt1Legge5990 != null ? istruttoria._CodiceArt1Legge5990.Trim() : null) ||
                        this._DecorrenzaOriginariaAltraPensione != istruttoria._DecorrenzaOriginariaAltraPensione ||
                        this._ImportoAdeguataAoi != istruttoria._ImportoAdeguataAoi ||
                        this._ImportoPagamentoAoi != istruttoria._ImportoPagamentoAoi ||
                        (this._CodiceCentroOperativo != null ? this._CodiceCentroOperativo.Trim() : null) != (istruttoria._CodiceCentroOperativo != null ? istruttoria._CodiceCentroOperativo.Trim() : null) ||
                        (this._CodPosizioneLavoro != null ? this._CodPosizioneLavoro.Trim() : null) != (istruttoria._CodPosizioneLavoro != null ? istruttoria._CodPosizioneLavoro.Trim() : null) ||
                        this._ScadenzaRevisioneAssegno != istruttoria._ScadenzaRevisioneAssegno ||
                        this._PensioneSurroga != istruttoria._PensioneSurroga ||
                        (this._CodiceASL != null ? this._CodiceASL.Trim() : null) != (istruttoria._CodiceASL != null ? istruttoria._CodiceASL.Trim() : null) ||
                        this._TipoPensioneExInpdai != istruttoria._TipoPensioneExInpdai ||
                        this._RiliquidazionePostCristallizzazione != istruttoria._RiliquidazionePostCristallizzazione ||
                        this._CodiceImporto != istruttoria._CodiceImporto ||
                        this._CodiceLiquidazione != istruttoria._CodiceLiquidazione ||
                        this._CodiceIsola != istruttoria._CodiceIsola ||
                        this._ModalitaLiquidazione != istruttoria._ModalitaLiquidazione ||
                        this._Provvisoria != istruttoria._Provvisoria ||
                        this._TipoCalcoloVincenteUnicarpe != istruttoria._TipoCalcoloVincenteUnicarpe ||
                        this._CodiceAziendaEditoria != istruttoria._CodiceAziendaEditoria ||
                        this._CodiceAziendaEditoriaPerTipo0171 != istruttoria._CodiceAziendaEditoriaPerTipo0171 ||
                        this._CodiceAziendaEditoriaPerTipo0179 != istruttoria._CodiceAziendaEditoriaPerTipo0179 ||
                        this._TrattamentoDisagi != istruttoria._TrattamentoDisagi ||
                        this._CodiceEnte != istruttoria._CodiceEnte ||
                        this._CodiceAziendaEditoriaLetteraB != istruttoria._CodiceAziendaEditoriaLetteraB ||
                        this._I_AGGANCIO != istruttoria._I_AGGANCIO ||
                        this._I_SETTEST != istruttoria._I_SETTEST ||
                        this._TipoCalcoloPrecedente != istruttoria._TipoCalcoloPrecedente ||
                        this._NSettimaneOI != istruttoria._NSettimaneOI)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            #endregion public members
        }
        #endregion nested class
    }
}

