using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class Delegati
    {

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoNumeroDomandaAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.NumeroDomanda) && String.IsNullOrEmpty(d2.NumeroDomanda))
                    return 0;
                if (String.IsNullOrEmpty(d1.NumeroDomanda))
                    return -1;
                if (String.IsNullOrEmpty(d2.NumeroDomanda))
                    return 1;
                return string.Compare(d1.NumeroDomanda, d2.NumeroDomanda, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoNumeroDomandaAsc" + ex);
            }
        };



        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoNumeroDomandaDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.NumeroDomanda) && String.IsNullOrEmpty(d2.NumeroDomanda))
                    return 0;
                if (String.IsNullOrEmpty(d1.NumeroDomanda))
                    return 1;
                if (String.IsNullOrEmpty(d2.NumeroDomanda))
                    return -1;
                return -string.Compare(d1.NumeroDomanda, d2.NumeroDomanda, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoNumeroDomandaDesc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoCategoriaAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Categoria) && String.IsNullOrEmpty(d2.Categoria))
                    return 0;
                if (String.IsNullOrEmpty(d1.Categoria))
                    return -1;
                if (String.IsNullOrEmpty(d2.Categoria))
                    return 1;
                return string.Compare(d1.Categoria, d2.Categoria, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCategoriaAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoCategoriaDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Categoria) && String.IsNullOrEmpty(d2.Categoria))
                    return 0;
                if (String.IsNullOrEmpty(d1.Categoria))
                    return 1;
                if (String.IsNullOrEmpty(d2.Categoria))
                    return -1;
                return -string.Compare(d1.Categoria, d2.Categoria, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCategoriaDesc" + ex);
            }
        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoSedeAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if ((String.IsNullOrEmpty(d1.CentroOperativo) || String.IsNullOrEmpty(d1.Sede)) && (String.IsNullOrEmpty(d2.CentroOperativo) || String.IsNullOrEmpty(d2.Sede)))
                    return 0;
                if (String.IsNullOrEmpty(d1.CentroOperativo) || String.IsNullOrEmpty(d1.Sede))
                    return -1;
                if (String.IsNullOrEmpty(d2.CentroOperativo) || String.IsNullOrEmpty(d2.Sede))
                    return 1;
                return string.Compare(d1.Sede + d1.CentroOperativo, d2.Sede + d2.CentroOperativo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoSedeAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoSedeDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if ((String.IsNullOrEmpty(d1.CentroOperativo) || String.IsNullOrEmpty(d1.Sede)) && (String.IsNullOrEmpty(d2.CentroOperativo) || String.IsNullOrEmpty(d2.Sede)))
                    return 0;
                if (String.IsNullOrEmpty(d1.CentroOperativo) || String.IsNullOrEmpty(d1.Sede))
                    return 1;
                if (String.IsNullOrEmpty(d2.CentroOperativo) || String.IsNullOrEmpty(d2.Sede))
                    return -1;
                return -string.Compare(d1.Sede + d1.CentroOperativo, d2.Sede + d2.CentroOperativo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoSedeDesc" + ex);
            }

        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoCertificatoAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Certificato) && String.IsNullOrEmpty(d2.Certificato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Certificato))
                    return -1;
                if (String.IsNullOrEmpty(d2.Certificato))
                    return 1;
                return string.Compare(d1.Certificato, d2.Certificato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCertificatoAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoCertificatoDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Certificato) && String.IsNullOrEmpty(d2.Certificato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Certificato))
                    return 1;
                if (String.IsNullOrEmpty(d2.Certificato))
                    return -1;
                return -string.Compare(d1.Certificato, d2.Certificato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCertificatoDesc" + ex);
            }

        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoTipoAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Tipo) && String.IsNullOrEmpty(d2.Tipo))
                    return 0;
                if (String.IsNullOrEmpty(d1.Tipo))
                    return -1;
                if (String.IsNullOrEmpty(d2.Tipo))
                    return 1;
                return string.Compare(d1.Tipo, d2.Tipo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoTipoAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoTipoDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Tipo) && String.IsNullOrEmpty(d2.Tipo))
                    return 0;
                if (String.IsNullOrEmpty(d1.Tipo))
                    return 1;
                if (String.IsNullOrEmpty(d2.Tipo))
                    return -1;
                return -string.Compare(d1.Tipo, d2.Tipo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoTipoDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoStatoAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Stato) && String.IsNullOrEmpty(d2.Stato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Stato))
                    return -1;
                if (String.IsNullOrEmpty(d2.Stato))
                    return 1;
                return string.Compare(d1.Stato, d2.Stato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoStatoAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ConfrontoStatoDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Stato) && String.IsNullOrEmpty(d2.Stato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Stato))
                    return 1;
                if (String.IsNullOrEmpty(d2.Stato))
                    return -1;
                return -string.Compare(d1.Stato, d2.Stato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoStatoDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoCategoriaPensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Categoria) && String.IsNullOrEmpty(d2.Categoria))
                    return 0;
                if (String.IsNullOrEmpty(d1.Categoria))
                    return -1;
                if (String.IsNullOrEmpty(d2.Categoria))
                    return 1;
                return string.Compare(d1.Categoria, d2.Categoria, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCategoriaPensioneAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoCategoriaPensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Categoria) && String.IsNullOrEmpty(d2.Categoria))
                    return 0;
                if (String.IsNullOrEmpty(d1.Categoria))
                    return 1;
                if (String.IsNullOrEmpty(d2.Categoria))
                    return -1;
                return -string.Compare(d1.Categoria, d2.Categoria, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCategoriaPensioneDesc" + ex);
            }
        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoSedePensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Sede) && String.IsNullOrEmpty(d2.Sede))
                    return 0;
                if (String.IsNullOrEmpty(d1.Sede))
                    return -1;
                if (String.IsNullOrEmpty(d2.Sede))
                    return 1;
                return string.Compare(d1.Sede, d2.Sede, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoSedePensioneAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoSedePensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Sede) && String.IsNullOrEmpty(d2.Sede))
                    return 0;
                if (String.IsNullOrEmpty(d1.Sede))
                    return 1;
                if (String.IsNullOrEmpty(d2.Sede))
                    return -1;
                return -string.Compare(d1.Sede, d2.Sede, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoSedePensioneDesc" + ex);
            }

        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoCertificatoPensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Certificato) && String.IsNullOrEmpty(d2.Certificato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Certificato))
                    return -1;
                if (String.IsNullOrEmpty(d2.Certificato))
                    return 1;
                return string.Compare(d1.Certificato, d2.Certificato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCertificatoPensioneAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoCertificatoPensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Certificato) && String.IsNullOrEmpty(d2.Certificato))
                    return 0;
                if (String.IsNullOrEmpty(d1.Certificato))
                    return 1;
                if (String.IsNullOrEmpty(d2.Certificato))
                    return -1;
                return -string.Compare(d1.Certificato, d2.Certificato, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCertificatoPensioneDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoDataCalcoloPensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.DataCalcolo) && String.IsNullOrEmpty(d2.DataCalcolo))
                    return 0;
                if (String.IsNullOrEmpty(d1.DataCalcolo))
                    return -1;
                if (String.IsNullOrEmpty(d2.DataCalcolo))
                    return 1;
                return string.Compare(d1.DataCalcolo, d2.DataCalcolo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoDataCalcoloPensioneAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoDataCalcoloPensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.DataCalcolo) && String.IsNullOrEmpty(d2.DataCalcolo))
                    return 0;
                if (String.IsNullOrEmpty(d1.DataCalcolo))
                    return 1;
                if (String.IsNullOrEmpty(d2.DataCalcolo))
                    return -1;
                return -string.Compare(d1.DataCalcolo, d2.DataCalcolo, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoDataCalcoloPensioneDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoTipoComponentePensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.TipoComponente) && String.IsNullOrEmpty(d2.TipoComponente))
                    return 0;
                if (String.IsNullOrEmpty(d1.TipoComponente))
                    return -1;
                if (String.IsNullOrEmpty(d2.TipoComponente))
                    return 1;
                return string.Compare(d1.TipoComponente, d2.TipoComponente, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoTipoComponentePensioneAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoTipoComponentePensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.TipoComponente) && String.IsNullOrEmpty(d2.TipoComponente))
                    return 0;
                if (String.IsNullOrEmpty(d1.TipoComponente))
                    return 1;
                if (String.IsNullOrEmpty(d2.TipoComponente))
                    return -1;
                return -string.Compare(d1.TipoComponente, d2.TipoComponente, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoTipoComponentePensioneDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoEliminazionePensioneAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Eliminazione) && String.IsNullOrEmpty(d2.Eliminazione))
                    return 0;
                if (String.IsNullOrEmpty(d1.Eliminazione))
                    return -1;
                if (String.IsNullOrEmpty(d2.Eliminazione))
                    return 1;
                return string.Compare(d1.Eliminazione, d2.Eliminazione, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoEliminazionePensioneAsc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ConfrontoEliminazionePensioneDesc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Eliminazione) && String.IsNullOrEmpty(d2.Eliminazione))
                    return 0;
                if (String.IsNullOrEmpty(d1.Eliminazione))
                    return 1;
                if (String.IsNullOrEmpty(d2.Eliminazione))
                    return -1;
                return -string.Compare(d1.Eliminazione, d2.Eliminazione, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoEliminazionePensioneDesc" + ex);
            }

        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoCodiceFiscaleAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.CodiceFiscale) && String.IsNullOrEmpty(d2.CodiceFiscale))
                    return 0;
                if (String.IsNullOrEmpty(d1.CodiceFiscale))
                    return -1;
                if (String.IsNullOrEmpty(d2.CodiceFiscale))
                    return 1;
                return string.Compare(d1.CodiceFiscale, d2.CodiceFiscale, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCodiceFiscaleAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoCodiceFiscaleDisc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.CodiceFiscale) && String.IsNullOrEmpty(d2.CodiceFiscale))
                    return 0;
                if (String.IsNullOrEmpty(d1.CodiceFiscale))
                    return 1;
                if (String.IsNullOrEmpty(d2.CodiceFiscale))
                    return -1; 
                return -string.Compare(d1.CodiceFiscale, d2.CodiceFiscale, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCodiceFiscaleDisc" + ex);
            }
        };


        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoCognomeAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Cognome) && String.IsNullOrEmpty(d2.Cognome))
                    return 0;
                if (String.IsNullOrEmpty(d1.Cognome))
                    return -1;
                if (String.IsNullOrEmpty(d2.Cognome))
                    return 1;
                return string.Compare(d1.Cognome, d2.Cognome, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCognomeAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoCognomeDisc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Cognome) && String.IsNullOrEmpty(d2.Cognome))
                    return 0;
                if (String.IsNullOrEmpty(d1.Cognome))
                    return 1;
                if (String.IsNullOrEmpty(d2.Cognome))
                    return -1; 
                return -string.Compare(d1.Cognome, d2.Cognome, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoCognomeDisc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoNomeAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Nome) && String.IsNullOrEmpty(d2.Nome))
                    return 0;
                if (String.IsNullOrEmpty(d1.Nome))
                    return -1;
                if (String.IsNullOrEmpty(d2.Nome))
                    return 1; 
                return string.Compare(d1.Nome, d2.Nome, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoNomeAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoNomeDisc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Nome) && String.IsNullOrEmpty(d2.Nome))
                    return 0;
                if (String.IsNullOrEmpty(d1.Nome))
                    return 1;
                if (String.IsNullOrEmpty(d2.Nome))
                    return -1; 
                return -string.Compare(d1.Nome, d2.Nome, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoNomeDisc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoDataNascitaAsc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (!d1.DataNascita.HasValue && !d2.DataNascita.HasValue)
                    return 0;
                if (!d1.DataNascita.HasValue)
                    return -1;
                if (!d2.DataNascita.HasValue)
                    return 1;
                return d1.DataNascita.Value.CompareTo(d2.DataNascita.Value);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoDataNascitaAsc" + ex);
            }
        };

        public static Comparison<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ConfrontoDataNascitaDisc = delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d1, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo d2)
        {
            try
            {
                if (!d1.DataNascita.HasValue && !d2.DataNascita.HasValue)
                    return 0;
                if (!d1.DataNascita.HasValue)
                    return 1;
                if (!d2.DataNascita.HasValue)
                    return -1;
                return -d1.DataNascita.Value.CompareTo(d2.DataNascita.Value);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoDataNascitaDisc" + ex);
            }
        };


        public static Comparison<Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero> ConfrontoAlfabeticoStatoEstero = delegate(Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero d1, Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero d2)
        {
            try
            {
                if (String.IsNullOrEmpty(d1.Descrizione) && String.IsNullOrEmpty(d2.Descrizione))
                    return 0;
                if (String.IsNullOrEmpty(d1.Descrizione))
                    return -1;
                if (String.IsNullOrEmpty(d2.Descrizione))
                    return 1;               
                return string.Compare(d1.Descrizione, d2.Descrizione, true, CultureInfo.InvariantCulture);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Delegati, Errore nel metodo ConfrontoAlfabeticoStatoEstero" + ex);
            }
        };

    }
}
