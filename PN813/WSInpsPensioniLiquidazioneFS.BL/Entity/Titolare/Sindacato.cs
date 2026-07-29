using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class Sindacato
    {
        #region private properties

        private string _Id;          //elencoSindacati.sindacato.ToArray()[5].sCodice
        private string _Sigla;       //elencoSindacati.sindacato.ToArray()[5].sSigla
        private string _Descrizione; //elencoSindacati.sindacato.ToArray()[5].sDescrizione
        private string _Progressivo; //elencoSindacati.sindacato.ToArray()[5].sProgressivo
        private Liquidazione.BLCommon.Utility.StatoSindacato _Stato;   //elencoSindacati.sindacato.ToArray()[5].sStato

        #endregion private properties

        #region public properties

        public string Id { get { return _Id; } set { _Id = value; } }
        public string Sigla { get { return _Sigla; } set { _Sigla = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        public string Progressivo { get { return _Progressivo; } set { _Progressivo = value; } }
        public Liquidazione.BLCommon.Utility.StatoSindacato Stato { get { return _Stato; } set { _Stato = value; } }

        #endregion public properties
    }
}
