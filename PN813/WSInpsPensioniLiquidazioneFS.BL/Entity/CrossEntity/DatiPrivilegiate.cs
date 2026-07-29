using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiPrivilegiate
    {
        #region private

        private int? _IndennitaSpecialeAnnua;
        private int? _AssegnoCura;
        private int? _Categoria2aInfermita;
        private int? _CumuloInfermita;
        private int? _IndennitaAccompagnamentoAggiuntiva;
        private int? _IntegrazioneIndennitaAssistenza;
        private int? _AssegnoIntegrativo;
        private int? _PrivilegiataSuperinvaliditaIndennita;
        private byte? _Semaforo;
        #endregion private

        #region public

        public int? IndennitaSpecialeAnnua { get { return _IndennitaSpecialeAnnua; } set { _IndennitaSpecialeAnnua = value; } }
        public int? AssegnoCura { get { return _AssegnoCura; } set { _AssegnoCura = value; } }
        public int? Categoria2aInfermita { get { return _Categoria2aInfermita; } set { _Categoria2aInfermita = value; } }
        public int? CumuloInfermita { get { return _CumuloInfermita; } set { _CumuloInfermita = value; } }
        public int? IndennitaAccompagnamentoAggiuntiva { get { return _IndennitaAccompagnamentoAggiuntiva; } set { _IndennitaAccompagnamentoAggiuntiva = value; } }
        public int? IntegrazioneIndennitaAssistenza { get { return _IntegrazioneIndennitaAssistenza; } set { _IntegrazioneIndennitaAssistenza = value; } }
        public int? AssegnoIntegrativo { get { return _AssegnoIntegrativo; } set { _AssegnoIntegrativo = value; } }
        public int? PrivilegiataSuperinvaliditaIndennita { get { return _PrivilegiataSuperinvaliditaIndennita; } set { _PrivilegiataSuperinvaliditaIndennita = value; } }

        // INPDAP
        public bool? IndennitaAusiliaria { get; set; }
        public bool? IndennitaParaplegici { get; set; }
        public bool? IndennitaSpeciale { get; set; }
        public string EnteEquoInd { get; set; }
        public decimal? ImpEquoInd { get; set; }
        //-------------------

        public byte? Semaforo { get { return _Semaforo; } set { _Semaforo = value; } }
        #endregion public

        public bool IsDatiPrivilegiateNull()
        {
            if (!this._IndennitaSpecialeAnnua.HasValue &&
                !this._AssegnoCura.HasValue &&
                !this._Categoria2aInfermita.HasValue &&
                !this._CumuloInfermita.HasValue &&
                !this._IndennitaAccompagnamentoAggiuntiva.HasValue &&
                !this._IntegrazioneIndennitaAssistenza.HasValue &&
                !this._AssegnoIntegrativo.HasValue &&
                !this._PrivilegiataSuperinvaliditaIndennita.HasValue &&
                !this.IndennitaAusiliaria.HasValue &&
                !this.IndennitaParaplegici.HasValue &&
                !this.IndennitaSpeciale.HasValue &&
                string.IsNullOrEmpty(this.EnteEquoInd) &&
                !this.ImpEquoInd.HasValue
               )
               return true;
            else
               return false;
        }
    }
}
