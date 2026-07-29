using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    public class AreaAggiornamentoWebDom
    {
        #region Private properties
        private int? _DomandeElaborate;
        private int? _DomandeDaElaborare;
        private int? _DomandeElaborateConErrore;
        private int? _DomandeDomandeTotali;
        private bool _IsAggiornamentoInCorso;
        private MemoryStream _PdfDoc;
        #endregion

        #region Public properties
        public int? DomandeElaborate { get { return _DomandeElaborate; } set { _DomandeElaborate = value; }}
        public int? DomandeDaElaborare { get { return _DomandeDaElaborare; } set { _DomandeDaElaborare = value; }}
        public int? DomandeElaborateConErrore {get { return _DomandeElaborateConErrore; } set { _DomandeElaborateConErrore = value; }}
        public int? DomandeDomandeTotali { get { return _DomandeDomandeTotali; } set { _DomandeDomandeTotali = value; } }
        public bool IsAggiornamentoInCorso { get { return _IsAggiornamentoInCorso; } set { _IsAggiornamentoInCorso = value; } }
        public MemoryStream PdfDoc { get { return _PdfDoc; } set { _PdfDoc = value; }}
        #endregion



        
    }
}