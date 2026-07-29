using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazioneFs.Data;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;

namespace INPS.Pensioni.LiquidazioneFs.Data.CMSGTRA
{
    public class AreaVariabile
    {
        #region Properties
        public List<Data.CMSGTRA.Anagrafica> ListaAnagrafica { get; set; }

        public List<Data.CMSGTRA.DelegatoNew> ListaDelegato { get; set; }

        public List<Data.CMSGTRA.Familiare> ListaFamiliare { get; set; }

        public List<Data.CMSGTRA.DanteCausa> ListaDanteCausa { get; set; }

        public List<Data.CMSGTRA.Supplementi> ListaSupplementi { get; set; }

        public List<Data.CMSGTRA.TrattamentiFamiglia> ListaTrattamentiFamiglia { get; set; }

        public List<Data.CMSGTRA.Minimo_PensInv> ListaMinimo_PensInv { get; set; }

        public List<Data.CMSGTRA.Residenza> ListaResidenza { get; set; }

        public List<Data.CMSGTRA.MaggiorazioneLegge> ListaMaggiorazioneLegge { get; set; }

        public List<Data.CMSGTRA.RenditaINAIL> ListaRenditaINAIL { get; set; }

        public List<Data.CMSGTRA.TrattenuteLavAutonomi> ListaTrattenuteLavAutonomi { get; set; }

        public List<Data.CMSGTRA.AgoTeorico> ListaAgoTeorico { get; set; }

        public List<Data.CMSGTRA.MaggiorazioneSociale> ListaMaggiorazioneSociale { get; set; }

        public List<Data.CMSGTRA.Redditi> ListaRedditi { get; set; }

        public List<Data.CMSGTRA.DatiNonCalcolo> ListaDatiNonCalcolo { get; set; }

        public List<Data.CMSGTRA.Gp4INPDAP> ListaGp4INPDAP { get; set; }

        public List<Data.CMSGTRA.Deleghe_Tutele> ListaDelegheTutele { get; set; }    

        public List<Data.CMSGTRA.Fondo.PI> ListaFondoPI { get; set; }

        public List<Data.CMSGTRA.Fondo.ES> ListaFondoES { get; set; }

        public List<Data.CMSGTRA.Fondo.GAS> ListaFondoGAS { get; set; }

        public List<Data.CMSGTRA.Fondo.ET> ListaFondoET { get; set; }

        public List<Data.CMSGTRA.Fondo.PM> ListaFondoPM { get; set; }

        public List<Data.CMSGTRA.Fondo.TT> ListaFondoTT { get; set; }

        public List<Data.CMSGTRA.Fondo.EL> ListaFondoEL { get; set; }

        public List<Data.CMSGTRA.Fondo.DZ> ListaFondoDZ { get; set; }

        public List<Data.CMSGTRA.Fondo.VL> ListaFondoVL { get; set; }

        public List<Data.CMSGTRA.Fondo.CL> ListaFondoCL { get; set; }

        public List<Data.CMSGTRA.Fondo.FS> ListaFondoFS { get; set; }

        public List<Data.CMSGTRA.Fondo.FS_New> ListaFondoFS_New { get; set; }

        public List<Data.CMSGTRA.Fondo.PT> ListaFondoPT { get; set; }

        public List<Data.CMSGTRA.Fondo.PT_New> ListaFondoPT_New { get; set; }

        public List<Data.CMSGTRA.Fondo.GDP> ListaFondoGDP { get; set; }

        public List<Data.CMSGTRA.Ago.PI> ListaAgoPI { get; set; }

        public List<Data.CMSGTRA.Ago.ES> ListaAgoES { get; set; }

        public List<Data.CMSGTRA.Ago.GAS> ListaAgoGAS { get; set; }

        public List<Data.CMSGTRA.Ago.ET> ListaAgoET { get; set; }

        public List<Data.CMSGTRA.Ago.PM> ListaAgoPM { get; set; }

        public List<Data.CMSGTRA.Ago.EL> ListaAgoEL { get; set; }

        public List<Data.CMSGTRA.Ago.TT> ListaAgoTT { get; set; }

        public List<Data.CMSGTRA.Ago.VL> ListaAgoVL { get; set; }

        public List<Data.CMSGTRA.Ago.DZ> ListaAgoDZ { get; set; }

        public List<Data.CMSGTRA.Ago.FS> ListaAgoFS { get; set; }

        public List<Data.CMSGTRA.Ago.PT> ListaAgoPT { get; set; }

        public List<Data.CMSGTRA.Ago.GDP> ListaAgoGDP { get; set; }

        public List<Data.CMSGTRA.Gp4IPOST> ListaGp4IPOST { get; set; }

        public List<Data.CMSGTRA.MiglioramentiContrattuali> ListaMiglioramentiContrattuali { get; set; }

        #endregion Properties
    }
}
