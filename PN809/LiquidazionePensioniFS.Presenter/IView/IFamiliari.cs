using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IFamiliari : IViewUI
    {
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoAnagrafica areaRiepilogoAnagrafica { get; set; }
        List<PresenterFamiliari.FamiliareFull> elencoFamiliari { get; set; }
        List<GestioneAreaFamiliariAreaFamiliare> areaFamiliare { get; set; }
        List<Anagrafica> anagrafica { get; set; }
        GestioneFamiliariConsultazioneUnificataANF consultazioneANF { get; set; }
        List<string> familiariToDelete { get; set; }
        AreaEsito areaEsito { get; set; }
        string codiceFiscale { get; set; }
        GestioneAreaFamiliariAreaDecFam areaDecodifica { get; set; }
    }
}
