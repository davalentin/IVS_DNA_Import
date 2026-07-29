using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.Aggiornamento;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Aggiornamento : CustomBasePage, IAggiornamento
    {
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        #region IAggiornamento
        public UtilityTipoAppartenenza? TipoApp { get; set; }
        public AreaAggiornamento areaAggiornamento { get; set; }
        #endregion IAggiornamento

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RecuperaInformazioni();
            }
        }

        private bool RecuperaInformazioni()
        {
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            //I pannelli relativi a AggiornamentoSAI e AggiornamentoCumulo saranno visibili solo per il tipo ruolo AGO
            if (this.TipoApp == UtilityTipoAppartenenza.AGO)
                pnlAgoVisible.Visible = true;
            //Il pannello relativo a AggiornamentoINPDAP sarà visibile solo per il tipo ruolo FS
            if (this.TipoApp == UtilityTipoAppartenenza.FS)
                pnlFsVisible.Visible = true;

            PresenterAggiornamento presenter = new PresenterAggiornamento();
            presenter.GetAggiornamento(this);

            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                return false;
            }

            if (this.areaAggiornamento != null)
            {
                if (this.areaAggiornamento.IsAggiornamentoInCorso)
                {
                    pnlInfo.Visible = false;
                    pnlElaborazioneInCorso.Visible = true;
                    if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.WebDom)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoWebDom.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Felpe)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoFelpe.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Oneri)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoOneri.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoOneri.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoOneri.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Cumulo)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoCumulo.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.Tot)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoTot.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoTot.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoTot.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoTot.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoTot.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoTot.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.SAI)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoSAI.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoSAI.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoSAI.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.INPDAP)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoINPDAP.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.NoteDiDebito)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }
                    else if (this.areaAggiornamento.TipoAggiornamentoInCorso == AreaAggiornamento.TipoAggiornamento.PianiDiPagamento)
                    {
                        if (this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborate.HasValue || this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborateConErrore.HasValue ||
                            this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeDaElaborare.HasValue)
                            pnlRiepilogo.Visible = true;
                        else
                            pnlRiepilogo.Visible = false;
                        lblDomandeElaborate.Text = (this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                        lblDomandeNonElaborate.Text = this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.DomandeDaElaborare.GetValueOrDefault().ToString();
                    }

                    return false;
                }
                else
                {
                    pnlElaborazioneInCorso.Visible = false;
                    pnlInfo.Visible = true;
                    ucAggiornamentoWebDom.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.WebDom);
                    ucAggiornamentoWebDom.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoWebDom);
                    ucAggiornamentoFelpe.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.Felpe);
                    ucAggiornamentoFelpe.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoFelpe);
                    ucAggiornamentoOneri.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.Oneri);
                    ucAggiornamentoOneri.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoOneri);
                    ucAggiornamentoCumulo.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.Cumulo);
                    ucAggiornamentoCumulo.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoCumulo);
                    ucAggiornamentoTot.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.Tot);
                    ucAggiornamentoTot.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoTot);
                    ucAggiornamentoSAI.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.SAI);
                    ucAggiornamentoSAI.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoSAI);
                    ucAggiornamentoINPDAP.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.INPDAP);
                    ucAggiornamentoINPDAP.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoINPDAP);
                    ucAggiornamentoNoteDebito.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.NoteDiDebito);
                    ucAggiornamentoNoteDebito.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoNoteDiDebito);
                    ucAggiornamentoPianiDiPagamento.SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento.PianiDiPagamento);
                    ucAggiornamentoPianiDiPagamento.ValorizzaEtichette(this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento);
                }
            }

            return true;
        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            event_ucHideAvviso(sender, e);
            RecuperaInformazioni();
        }

        protected void event_ucShowElaborazioneInCorso(object sender, EventArgs e)
        {
            pnlElaborazioneInCorso.Visible = true;
            pnlRiepilogo.Visible = false;
            pnlInfo.Visible = false;
        }

        protected void event_ucRecuperaInformazioni(object sender, TipoAggiornamentoToastArgs e)
        {
            if (RecuperaInformazioni())
            {
                ucAvviso.Visible = true;
                ucAvviso.Titolo = "Elaborazione completata";
                ucAvviso.Messaggio = string.Format("Le domande {0} sono state aggiornate", e.TipoAggiornamentoInCorso);
                ucAvviso.Tipo = TipoAvviso.Ok;
            }
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
        }
    }
}
