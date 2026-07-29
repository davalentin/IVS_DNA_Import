using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon.Entity
{
    public class DatiContribuzioneEnpals
    {
        public long IdPensione { get; set; }

        #region public properties
        public TipologiaContribuzioneEnpals Tipologia { get; set; }

        public Quota QuotaA { get; set; }

        public Quota QuotaB { get; set; }

        public Quota QuotaC { get; set; }
        #endregion public properties

        public bool IsNull()
        {
            if (this.QuotaA != null || this.QuotaB != null || this.QuotaC != null)
            {
                if (this.QuotaA != null)
                {
                    if (this.QuotaA.Enpals != null ||
                        this.QuotaA.Estera != null ||
                        this.QuotaA.Figurativa != null ||
                        this.QuotaA.Inps != null ||
                        this.QuotaA.Ufficio != null ||
                        this.QuotaA.Volontaria != null)
                        return false;
                }

                if (this.QuotaB != null)
                {
                    if (this.QuotaB.Enpals != null ||
                        this.QuotaB.Estera != null ||
                        this.QuotaB.Figurativa != null ||
                        this.QuotaB.Inps != null ||
                        this.QuotaB.Ufficio != null ||
                        this.QuotaB.Volontaria != null)
                        return false;
                }

                if (this.QuotaC != null)
                {
                    if (this.QuotaC.Enpals != null ||
                        this.QuotaC.Estera != null ||
                        this.QuotaC.Figurativa != null ||
                        this.QuotaC.Inps != null ||
                        this.QuotaC.Ufficio != null ||
                        this.QuotaC.Volontaria != null)
                        return false;
                }
            }

            return true;
        }

        public class Quota
        {
           
            public System.Nullable<int> Enpals { get; set; }
            public System.Nullable<int> Figurativa { get; set; }
            public System.Nullable<int> Ufficio { get; set; }
            public System.Nullable<int> Inps { get; set; }
            public System.Nullable<int> Volontaria { get; set; }
            public System.Nullable<int> Estera { get; set; }

            public bool isNull()
            {
                return !Enpals.HasValue && !Figurativa.HasValue && !Ufficio.HasValue && !Inps.HasValue && !Volontaria.HasValue && !Estera.HasValue;
            }
        }
    }
}
