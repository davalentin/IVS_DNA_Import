using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;
using INPS.Pensioni.Liquidazione;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;


namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaDanteCausa
    {
        private long _Id;
        private AltraPensioneDC _AltraPensioneDC;
        private AnagraficaDC _AnagraficaDC;
        private DatiPensioneCI _DatiPensioneCI;
        private DatiPensioneDiretta _DatiPensioneDiretta;
        private DatiRedditiSentenza495_93 _DatiRedditiSentenza495_93;
        private List<DatiMaggiorazione780> _ElencoMaggiorazione780;
        private List<CodiciNatura> _ElencoCodiciNatura;
        private List<CodiceEliminazione> _ElencoCodiceEliminazione;
        private char _SiglaFamiliare;
        private decimal? _ImportoMensilePensioneEstera;
        private bool? _IsFascicoloGenerato;
        private DateTime? _DataAssunzioneCarico;

        [DataMember]
        public long Id { get { return _Id; } set { _Id = value; } }
        [DataMember]
        public AltraPensioneDC AltraPensioneDC { get { return _AltraPensioneDC; } set { _AltraPensioneDC = value; } }
        [DataMember]
        public AnagraficaDC AnagraficaDC { get { return _AnagraficaDC; } set { _AnagraficaDC = value; } }
        [DataMember]
        public DatiPensioneCI DatiPensioneCI { get { return _DatiPensioneCI; } set { _DatiPensioneCI = value; } }
        [DataMember]
        public DatiPensioneDiretta DatiPensioneDiretta { get { return _DatiPensioneDiretta; } set { _DatiPensioneDiretta = value; } }
        [DataMember]
        public DatiRedditiSentenza495_93 DatiRedditiSentenza495_93 { get { return _DatiRedditiSentenza495_93; } set { _DatiRedditiSentenza495_93 = value; } }
        [DataMember]
        public List<DatiMaggiorazione780> ElencoMaggiorazione780 { get { return _ElencoMaggiorazione780; } set { _ElencoMaggiorazione780 = value; } }
        [DataMember]
        public List<CodiciNatura> ElencoCodiciNatura { get { return _ElencoCodiciNatura; } set { _ElencoCodiciNatura = value; } }
        [DataMember]
        public List<CodiceEliminazione> ElencoCodiceEliminazione { get { return _ElencoCodiceEliminazione; } set { _ElencoCodiceEliminazione = value; } }
        [DataMember]
        public char SiglaFamiliare { get { return _SiglaFamiliare; } set { _SiglaFamiliare = value; } }
        [DataMember]
        public bool? IsFascicoloGenerato { get { return _IsFascicoloGenerato; } set { _IsFascicoloGenerato = value; } }
        [DataMember]
        public Utility.TipoAnte96? IsAnte96 { get; set; }

        [DataMember]
        public DateTime? DataAssunzioneCarico { get { return _DataAssunzioneCarico; } set { _DataAssunzioneCarico = value; } }

        [DataMember]
        public bool IsPresenteBypassNessunDanteCausa { get; set; }

        [DataContract]
        public class DatiMaggiorazione780
        {
            public DatiMaggiorazione780()
            {
            }

            public DatiMaggiorazione780(string id, string descrizione)
            {
                this._Id = id;
                this._Descrizione = descrizione;
            }

            #region Public properties
            [DataMember]
            public string Id { get { return _Id; } set { _Id = value; } }
            [DataMember]
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion Public properties

            #region Private properties
            private string _Id;
            private string _Descrizione;
            #endregion Private properties

        }

        //ENG - Gestione Pensione Estera
        [DataMember]
        public decimal? ImportoMensilePensioneEstera { get { return _ImportoMensilePensioneEstera; } set { _ImportoMensilePensioneEstera = value; } }

    }
}


