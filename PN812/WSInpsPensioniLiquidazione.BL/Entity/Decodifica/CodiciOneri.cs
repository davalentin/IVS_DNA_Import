using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class CodiciOneri
    {
        #region public properties
        public GruppoOneri gruppoOneri { get { return _gruppoOneri; } set { _gruppoOneri = value; } }

        public SottoGruppoOneri sottoGruppoOneri { get { return _sottoGruppoOneri; } set { _sottoGruppoOneri = value; } }
        #endregion public properties

        #region private properties
        private GruppoOneri _gruppoOneri;

        private SottoGruppoOneri _sottoGruppoOneri;
        #endregion private properties


        public class GruppoOneri
        {
            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Code { get { return _Code; } set { _Code = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private string _Code;
            private string _Descrizione;

            #endregion private properties
        }

        public class SottoGruppoOneri
        {
            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdOnere { get { return _IdOnere; } set { _IdOnere = value; } }
            public string Code { get { return _Code; } set { _Code = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public bool? IsPubblica { get { return _IsPubblica; } set { _IsPubblica = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private long _IdOnere;
            private string _Code;
            private string _Descrizione;
            private bool? _IsPubblica;

            #endregion private properties
        }
    }
}
