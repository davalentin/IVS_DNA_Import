using System;
using System.Collections.Generic;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class Segnalazione
    {
        #region public properties
        public string NomeMittente { get; set; }
        public string CognomeMittente { get; set; }
        public string RecapitoMittente { get; set; }
        public string Procedura { get; set; }
        public string Messaggio { get; set; }
        public List<string> Destinatari { get; set; }
        public string NDomus { get; set; }
        public string MatricolaOperatore { get; set; }
        public string Sede { get; set; }
        public string Tipologia { get; set; }
        public string CodiceFiscale { get; set; }
        public string Categoria { get; set; }
        public string Certificato { get; set; }
        public string CodiceErrore { get; set; }
        public string SedeOperatore { get; set; }
        public Utility.TipoAppartenenza? tipoApp { get; set; }
        public DateTime? DecorrenzaPensione { get; set; }
        #endregion public properties
    }
}
