using System.Collections.Generic;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DatiRedditiSentenza495_93
    {
        private List<RedditoSentenza495_93> _LredditiSentenza495_93;

        private bool _isDCSentenza495_93Ante2009;
        private bool _isDCSentenza495_93Post2008;

        
        public List<RedditoSentenza495_93> LredditiSentenza495_93 
        { 
            get { return _LredditiSentenza495_93; } 
            set { _LredditiSentenza495_93 = value; } 
        }

        public bool IsDCSentenza495_93Ante2009
        {
            get { return _isDCSentenza495_93Ante2009; }
            set { _isDCSentenza495_93Ante2009 = value; }
        }

        public bool IsDCSentenza495_93Post2008
        {
            get { return _isDCSentenza495_93Post2008; }
            set { _isDCSentenza495_93Post2008 = value; }
        }

        #region nested class

        public class RedditoSentenza495_93
        {
            private long? _IdPensione;
            private short? _AnnoReddito;
            private decimal? _RedditoTitolare;
            private decimal? _RedditoConiuge;
            private decimal? _RedditoDaPensioneConiuge;
            private decimal? _RedditoDaPensioneDC;
            private bool? _IsPre2009;
            private string _CodiceDiReddito;
            private bool? _FlagSentenza;
            private short? _CodiceSentenza;
            private short? _MeseSentenza;
            private short? _AnnoSentenza;

            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public short? AnnoReddito { get { return _AnnoReddito; } set { _AnnoReddito = value; } }
            public decimal? RedditoTitolare { get { return _RedditoTitolare; } set { _RedditoTitolare = value; } }
            public decimal? RedditoConiuge { get { return _RedditoConiuge; } set { _RedditoConiuge = value; } }
            public decimal? RedditoDaPensioneConiuge { get { return _RedditoDaPensioneConiuge; } set { _RedditoDaPensioneConiuge = value; } }
            public decimal? RedditoDaPensioneDC { get { return _RedditoDaPensioneDC; } set { _RedditoDaPensioneDC = value; } }
            public bool? IsPre2009 { get { return _IsPre2009; } set { _IsPre2009 = value; } }
            public string CodiceDiReddito { get { return _CodiceDiReddito; } set { _CodiceDiReddito = value; } }
            public bool? FlagSentenza { get { return _FlagSentenza; } set { _FlagSentenza = value; } }
            public short? CodiceSentenza { get { return _CodiceSentenza; } set { _CodiceSentenza = value; } }
            public short? MeseSentenza { get { return _MeseSentenza; } set { _MeseSentenza = value; } }
            public short? AnnoSentenza { get { return _AnnoSentenza; } set { _AnnoSentenza = value; } }

            public bool IsAllDatiAltraPensioneDCNull()
            {
                if (!AnnoReddito.HasValue && !RedditoTitolare.HasValue && !RedditoConiuge.HasValue && !RedditoDaPensioneConiuge.HasValue && !RedditoDaPensioneDC.HasValue)
                    return true;
                else
                    return false;
            }
        }

        #endregion nested class
    }
}
