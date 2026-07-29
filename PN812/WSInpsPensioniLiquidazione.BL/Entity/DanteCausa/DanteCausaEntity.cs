using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DanteCausaEntity
    {
        private long _IdDC;
        private AnagraficaDC _AnagraficaDC;
        private AltraPensioneDC _AltraPensioneDC;
        private DatiPensioneCI _DatiPensioneCI;
        private DatiPensioneDiretta _DatiPensioneDiretta;
        private DatiRedditiSentenza495_93 _DatiRedditiSentenza495_93;
        private List<DatiMaggiorazione781> _ElencoMaggiorazione781;

        public long IdDC { get { return _IdDC; } set { _IdDC = value; } }
        public AnagraficaDC AnagraficaDC { get { return _AnagraficaDC; } set { _AnagraficaDC = value; } }
        public AltraPensioneDC AltraPensioneDC { get { return _AltraPensioneDC; } set { _AltraPensioneDC = value; } }
        public DatiPensioneCI DatiPensioneCI { get { return _DatiPensioneCI; } set { _DatiPensioneCI = value; } }
        public DatiPensioneDiretta DatiPensioneDiretta { get { return _DatiPensioneDiretta; } set { _DatiPensioneDiretta = value; } }
        public DatiRedditiSentenza495_93 DatiRedditiSentenza495_93 { get { return _DatiRedditiSentenza495_93; } set { _DatiRedditiSentenza495_93 = value; } }
        public List<DatiMaggiorazione781> ElencoMaggiorazione781 { get { return _ElencoMaggiorazione781; } set { _ElencoMaggiorazione781 = value; } }


        public class DatiMaggiorazione781
        {
            public DatiMaggiorazione781()
            {}

            public DatiMaggiorazione781(string id, string descrizione)
            {
                this._Id = id;
                this._Descrizione = descrizione;
            }

            #region public properties

            public string Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private string _Id;
            private string _Descrizione;

            #endregion private properties

        }
    }
}
