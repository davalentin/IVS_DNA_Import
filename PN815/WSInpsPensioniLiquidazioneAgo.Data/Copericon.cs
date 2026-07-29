using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Data
{
    public class Copericon
    {
        public HostRequest.CopericonRequest Request { get; set; }
        public HostResponse.CopericonResponse Response { get; set; }

        public void Invoke()
        {
            try
            {
                Response = new HostResponse.CopericonResponse();
                Response.Esito = true;
                SrvCopericon.ServiceVerificaClient proxy = new SrvCopericon.ServiceVerificaClient();
                SrvCopericon.DatiINPUT input = MapInput();
                SrvCopericon.CResult output = proxy.VerifyDataStruct(input);
                if(output == null || output.Esito != 1)
                {
                    Response.Esito = false;
                    Response.Messaggio = output.Descrizione;
                }

            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new INPS.DNA.DnaApplicationException("Puntamento errato al servizio Copericon", ex);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                throw new INPS.DNA.DnaApplicationException("Errore di comunicazione con il servizio Copericon", ex);
            }
            catch
            {
                throw;
            }
        }

        #region private methods
        private SrvCopericon.DatiINPUT MapInput()
        {
            SrvCopericon.DatiINPUT input = new SrvCopericon.DatiINPUT();
            input.TipoProcedura = Request.TipoProcedura;
            input.CodFonAnnoInCorso = Request.CodFondo;
            input.CodFonAnnoStorico = Request.CodFondoStorico;
            input.ImpTraErarialiAnniPre = Request.ImportoTrattenuteErarialiAP;
            input.CodCatPensione = Request.CodCategoria;
            input.CodSede = Request.CodSede;
            input.NumCertificato = Request.Certificato;
            input.CodEliminazione = Request.CodEliminazione;
            input.DataDecEliminazione = Request.DataEliminazione;
            input.AnnoDecorrenza = Request.AnnoDecorrenza;
            input.MeseDecorrenza = Request.MeseDecorrenza;
            input.MeseEstrazioneRata = Request.MeseEstrazioneRata;
            input.CodBeneficiLegge2062004 = Request.CodBeneficiLegge2062004;
            input.CodPartRinnovo = Request.CodParticolareRinnovo;
            input.CodMovimentazione = Request.CodMovimentazione;
            input.DataMovimentazione = Request.DataMovimentazione.GetValueOrDefault();
            input.SendMail = Request.InvioMail;
            input.DataPrelievo = Request.DataPrelievo.GetValueOrDefault();
            input.User = Request.MatricolaOperatore.ToString();
            return input;
        }
        #endregion private methods
    }
}
