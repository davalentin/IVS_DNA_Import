using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaOneri
    {
        [DataMember]
        public Entity.Oneri.DatiPrepensionamento DatiPrepensionamento { get; set; }

        [DataMember]
        public Entity.Oneri.DatiOneriBenefParticolari DatiOneriBenefParticolari { get; set; }

        [DataMember]
        public Entity.Oneri.DatiOneriBenefParticolari DatiOneriBenefParticolariStorico { get; set; }

        [DataMember]
        public List<Entity.CodiciOneri.GruppoOneri> ListaGruppoOneri { get; set; }

        [DataMember]
        public List<Entity.CodiciOneri.SottoGruppoOneri> ListaSottoGruppoOneri { get; set; }


        [DataMember]
        public bool IsBeneficioAmianto { get; set; }
        [DataMember]
        public bool IsOneriSperDonnaObbligatori { get; set; }
        [DataMember]
        public bool IsBeneficioVittimeTerrorismo { get; set; }
        [DataMember]
        public bool IsPrepensionamentoEditoriaArt1c154L205_2017 { get; set; }
        [DataMember]
        public bool IsPrepensionamentoEditoriaArt1c500L160_2019 { get; set; }
        [DataMember]
        public bool IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione { get; set; }
        [DataMember]
        public bool IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione { get; set; }
        [DataMember]
        public bool IsPrepensionamentoEditoria { get; set; }
        [DataMember]
        public bool IsOpzioneDonna_Legge197_2022_Art1_Comma292 { get; set; }
        [DataMember]
        public bool IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione { get; set; }
        [DataMember]
        public bool IsPrepensionamentoEditoriaLetteraB { get; set; }
        [DataMember]
        public bool IsOneriPresentiDaAzienda { get; set; }
        [DataMember]
        public bool IsRicVOPGIMigrataFiltroEBA { get; set; }

    }
}