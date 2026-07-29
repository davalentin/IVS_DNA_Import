using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DatiPensioneCI
    {        
        // pensione
        private DateTime? _DecorrenzaOriginariaPrima;
        
        // dantecausa
        private byte? _CodiceTipoPerequazione;
        private decimal? _VirtualeIntegrata;
        private decimal? _VirtualePura;
        private decimal? _Adeguata;
        private decimal? _TotaleQuoteFisse;

        // maggiorazioneBenefici
        private decimal? _TotaleArticolo345Legge140;        // a mano  
        private byte? _Articolo6140;                          // 1 o 9 calcolato dal codice cieco e non salvato
        private DateTime? _DecorrenzaMaggiorazioneArt6;       // DecorrenzaArt6140  
        private bool? _Articolo1Legge5991;                   //_Articolo1L5991;
        private decimal? _AumentoMensileLegge5991Comma2;   //_Totale90L5991;
        private decimal? _AumentoMensileLegge5991Comma9;     //_Totale9294L5991;
        private decimal? _MensileLegge5991;                  // _MensileLegge5991
        private decimal? _Aumento7290;                      //_AumentoSentenza7290;
        private decimal? _Aumento7290DC;                   // _AumentoSentenza7290Art2; 
        private decimal? _AumentoMensileLegge161289Art2;  //_AumentoTotaleArt2;  
        private decimal? _ImportoPagamentoDataMorte49593;  //_ImportoPagamentoDataMorte49593;
        //PensioniEstereDc
        private List<DatiPensioniEstereDc> _lDatiPensioniEstereDc;

       
        public DateTime? DecorrenzaOriginariaPrima { get { return _DecorrenzaOriginariaPrima; } set { _DecorrenzaOriginariaPrima = value; } }
        public byte? CodiceTipoPerequazione { get { return _CodiceTipoPerequazione; } set { _CodiceTipoPerequazione = value; } }
        public decimal? VirtualeIntegrata { get { return _VirtualeIntegrata; } set { _VirtualeIntegrata = value; } }
        public decimal? VirtualePura { get { return _VirtualePura; } set { _VirtualePura = value; } }
        public decimal? Adeguata { get { return _Adeguata; } set { _Adeguata = value; } }
        public decimal? TotaleQuoteFisse { get { return _TotaleQuoteFisse; } set { _TotaleQuoteFisse = value; } }
        public decimal? TotaleArticolo345Legge140 { get { return _TotaleArticolo345Legge140; } set { _TotaleArticolo345Legge140 = value; } }
        public byte? Articolo6140 { get { return _Articolo6140; } set { _Articolo6140 = value; } }
        public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }
        public bool? Articolo1Legge5991 { get { return _Articolo1Legge5991; } set { _Articolo1Legge5991 = value; } }
        public decimal? AumentoMensileLegge5991Comma2 { get { return _AumentoMensileLegge5991Comma2; } set { _AumentoMensileLegge5991Comma2 = value; } }
        public decimal? AumentoMensileLegge5991Comma9 { get { return _AumentoMensileLegge5991Comma9; } set { _AumentoMensileLegge5991Comma9 = value; } }
        public decimal? MensileLegge5991 { get { return _MensileLegge5991; } set { _MensileLegge5991 = value; } }
        public decimal? Aumento7290 { get { return _Aumento7290; } set { _Aumento7290 = value; } }
        public decimal? Aumento7290DC { get { return _Aumento7290DC; } set { _Aumento7290DC = value; } }
        public decimal? AumentoMensileLegge161289Art2 { get { return _AumentoMensileLegge161289Art2; } set { _AumentoMensileLegge161289Art2 = value; } }
        public decimal? ImportoPagamentoDataMorte49593 { get { return _ImportoPagamentoDataMorte49593; } set { _ImportoPagamentoDataMorte49593 = value; } }
        public List<DatiPensioniEstereDc> lDatiPensioniEstereDc { get { return _lDatiPensioniEstereDc; } set { _lDatiPensioniEstereDc = value; } }

        public bool IsDatiPensioneCINull()
        {
            if (DecorrenzaOriginariaPrima.HasValue || CodiceTipoPerequazione.HasValue || VirtualeIntegrata.HasValue || VirtualePura.HasValue ||
                Adeguata.HasValue || TotaleQuoteFisse.HasValue || TotaleArticolo345Legge140.HasValue || Articolo6140.HasValue || DecorrenzaMaggiorazioneArt6.HasValue ||
                Articolo1Legge5991.HasValue || AumentoMensileLegge5991Comma2.HasValue || AumentoMensileLegge5991Comma9.HasValue || MensileLegge5991.HasValue || Aumento7290.HasValue ||
                Aumento7290DC.HasValue || AumentoMensileLegge161289Art2.HasValue || (lDatiPensioniEstereDc != null && lDatiPensioniEstereDc.Count > 0))
                return false;
            else
                return true;
        }

        public class DatiPensioniEstereDc
        {
            public DatiPensioniEstereDc()
            {}

            public DatiPensioniEstereDc(byte? CodiciVari, decimal? _Importo, long? IdDanteCausa)
            {
                this._CodiciVari = CodiciVari;
                this._IdDanteCausa = IdDanteCausa;
                this._Importo = Importo;
            }

            #region public properties

            public byte? CodiciVari { get { return _CodiciVari; } set { _CodiciVari = value; } }
            public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
            public long? IdDanteCausa { get { return _IdDanteCausa; } set { _IdDanteCausa = value; } }

            #endregion public properties

            #region private properties

            private byte? _CodiciVari;
            private decimal? _Importo;
            private long? _IdDanteCausa;
            #endregion private properties

        }
    }
}
