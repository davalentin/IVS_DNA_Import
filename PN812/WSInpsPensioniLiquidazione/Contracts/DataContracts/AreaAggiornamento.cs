using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    public class AreaAggiornamento
    {
        public AreaAggiornamentoGeneric AreaAggiornamentoWebDom { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoFelpe { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoOneri { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoSAI { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoCumulo { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoTot { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoINPDAP { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoNoteDiDebito { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoPianiDiPagamento { get; set; }
        public AreaAggiornamentoGeneric AreaAggiornamentoEquoInd { get; set; }
        public bool IsAggiornamentoInCorso { get; set; }
        public TipoAggiornamento TipoAggiornamentoInCorso { get; set; }

        #region nested class
        public class AreaAggiornamentoGeneric
        {
            #region Private properties
            private int? _DomandeElaborate;
            private int? _DomandeDaElaborare;
            private int? _DomandeElaborateConErrore;
            private int? _DomandeDomandeTotali;
            private MemoryStream _PdfDoc;
            #endregion Private properties

            #region Public properties
            public int? DomandeElaborate { get { return _DomandeElaborate; } set { _DomandeElaborate = value; } }
            public int? DomandeDaElaborare { get { return _DomandeDaElaborare; } set { _DomandeDaElaborare = value; } }
            public int? DomandeElaborateConErrore { get { return _DomandeElaborateConErrore; } set { _DomandeElaborateConErrore = value; } }
            public int? DomandeDomandeTotali { get { return _DomandeDomandeTotali; } set { _DomandeDomandeTotali = value; } }
            public MemoryStream PdfDoc { get { return _PdfDoc; } set { _PdfDoc = value; } }
            #endregion Public properties
        }
        #endregion nested class

        #region enum
        public enum TipoAggiornamento
        {
            Nessuno,
            WebDom,
            Felpe,
            Oneri,
            SAI, 
            Cumulo,
            INPDAP,
            Tot,
            NoteDiDebito,
            PianiDiPagamento,
            EquoInd
        }
        #endregion enum
    }
}
