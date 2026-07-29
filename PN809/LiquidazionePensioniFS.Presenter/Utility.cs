using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.ServiceModel;
using System.ComponentModel;
using System.Reflection;

using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Linq;
using System.Collections.Generic;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public static class Utility
    {
        public enum TipoRicerca { NDomus = 1, CodiceFiscale, Anagrafica, NDomusConRicercaCodiceFiscaleDA, NDomusConRicercaDatiParzialiDA };

        public enum CriterioRicercaStatoPratica
        {
            NumeroDomanda,
            CategoriaPensione,
            StatoPratica,
            PeriodoGiacenza,
            Fondo,
            Sede,
            Anagrafica,
            CodiceFiscale,
            DataPresentazione,
            DataElaborazione,
            Matricola,
            TipoDomandaInLavorazione,
            TipoDomandaLavorata,
            Gruppo,
            Prodotto,
            Tipo,
            Cassa
        };

        public enum TabMaggiorazioniAbilitate { Agevolazioni, ExCombattente, AgevolazioniEExCombattente };

        public enum Categoria
        {
            VO,
            VOP,
            VOMIN,
            VR,
            VOART,
            VOCOM,
            VDAI,
            IDAI,
            SDAI,
            SR,
            IR,
            VOCRED,
            CRED27,
            VOCOOP,
            COOP28,
            VOESO,
            IOART,
            SOART,
            //AUT 
            VOAUT,
            IOAUT,
            SOAUT,
            VESO33,
            VESO92,
            VOCUM,
            IOCUM,
            SOCUM,
            APE,
            VOSPETT,
            VESO29,
            ESOTEL,
            ESOAMB,
            VOSPED,
            IOSPED,
            SOSPED,
            INDCOM,
            ESPA,
            VOCTPS,
            IOCTPS,
            SOCTPS,
            VOCPDEL,
            IOCPDEL,
            SOCPDEL,
            SOMIN,
            VOTOT,
            SOTOT,
            IOTOT,
            IOP,
            SOP,
            VOST,
            VOBIS,
            IOBIS,
            VMP,
            IMP,
            VOBANC,
            VOPGI,
            SOPGI,
            IOPGI,
            ESOPMI
        };

        public enum TipoPLPerRIC
        {
            Nessuno = 0,
            [Description("Ricostituzione APE Precoci")]
            APEPrecoci,
            [Description("Ricostituzione Sperimentale Donna D.L. 4/2019")]
            SperimentaleDonna_DL_4_2019,
            [Description("Ricostituzione Anzianita Per Legge Bilancio 2019")]
            AnzianitaPerLeggeBilancio2019,
            [Description("Ricostituzione Quota 100")]
            Quota100,
            [Description("Ricostituzione Inabilità Amianto Legge 232/2016")]
            InabilitaAmianto,
            [Description("Ricostituzione Gravosi Usuranti con opzione al contributivo")]
            GravosiUsurantiConOpzione,
            [Description("Ricostituzione Contributivo Puro")]
            ContributivoPuro,
            [Description("Ricostituzione Contributivo con Opzione")]
            ContributivoConOpzione,
            [Description("Ricostituzione Prepensionamento Editoria art. 1 c. 500 L.160/2019")]
            RicPrepensionamentoEditoriaArt1c500L160_2019,
            [Description("Ricostituzione Quota 102")]
            Quota102,
            [Description("Ricostituzione Prepensionamento Editoria art. 37 legge 416/1981 lettera a)")]
            RicPrepensionamentoEditoriaArt37L416_1981_LetteraA,
            [Description("Ricostituzione ESPA con Filtro L26")]
            RicESPAFiltroL26,
            [Description("Ricostituzione VESO33 con Filtro DAP")]
            RicVESO33FiltroDAP,
            [Description("Ricostituzione Anticipata Flessibile")]
            AnticipataFlessibile,
            [Description("Ricostituzione Opzione Donna con Filtro KWA")]
            RicOpzioneDonnaFiltroKWA,
            [Description("Ricostituzione Opzione Donna con Filtro KXM")]
            RicOpzioneDonnaFiltroKXM,
            [Description("Ricostituzione Opzione Donna con Filtro KYA")]
            RicOpzioneDonnaFiltroKYA,
            [Description("Ricostituzione Opzione Donna con Filtro KZM")]
            RicOpzioneDonnaFiltroKZM,
            [Description("Ricostituzione Opzione Donna con Filtro KUA")]
            RicOpzioneDonnaFiltroKUA,
            [Description("Ricostituzione Opzione Donna con Filtro KVM")]
            RicOpzioneDonnaFiltroKVM,
            //ENG - Gestione RIC Anticipate Computo Senza Filtro PAV
            [Description("Ricostituzione Anticipate Computo Senza Filtro PAV")]
            RicAnticipateComputoSenzaFiltroPAV,
            //ENG - Gestione RIC Prepensionamento Editoria lettera b
            [Description("Ricostituzione Prepensionamento Editoria art. 37 legge 416/1981 lettera b)")]
            RicPrepensionamentoEditoriaArt37L416_1981_LetteraB,
            [Description("Ricostituzione inabilità ordinaria in cumulo")]
            RicInabilitaOrdinariaInCumulo,
            [Description("Ricostituzione inabilità art. 2 comma 12 legge 335/1995 in cumulo")]
            RicInabilitaArt2Comma12Legge3351995InCumulo,
            [Description("Ricostituzione inabilità a proficuo lavoro/mansioni in cumulo")]
            RicInabilitaAProficuoLavoroMensioniInCumulo,
            [Description("Ricostituzione Anticipate Computo Con Filtro PAV")]
            RicAnticipateComputoConFiltroPAV,
            [Description("Ricostituzione Anticipate Flessibile Opzione Contributivo")]
            RicAnticipateFlessibileOpzioneContributivo,
            [Description("Ricostituzione Vecchiaia Computo")]
            RicVecchiaiaComputo,
            [Description("Ricostituzione Vecchiaia Ordinario")]
            RicVecchiaiaOrdinario,
            [Description("Ricostituzione Anticipata Flessibile legge di bilancio 2024")]
            RicAnticipataFlessibileLeggeBilancio2024,
            [Description("Ricostituzione Anticipata Flessibile legge di bilancio 2024 con opzione al contributivo")]
            RicAnticipataFlessibileOpzioneContributivoLeggeBilancio2024,
            [Description("Ricostituzione Lavoratori Faticosi e Pesanti")]
            RicLavoratoriFaticosiEPesanti,
            Nessun = 33,
            [Description("Ricostituzione VOAUT Anticipata Flessibile legge bilancio 2024 con filtro GSE")]
            RicVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE,
            [Description("Ricostituzione VOAUT Anticipata tipo contributivo con filtro GSE")]
            RicVOAUTAnticipataTipoContributivoFiltroGSE,
            [Description("Ricostituzione VOAUT Vecchiaia tipo contributivo con filtro GSE")]
            RicVOAUTVecchiaiaTipoContributivoFiltroGSE,
            [Description("Ricostituzione Org. Int. Vecc/Inv Filtro C9A")]
            RicOIVecchiaiaInvaliditaFiltroC9A,
            [Description("Ricostituzione Org. Int. Superstiti Filtro C9A")]
            RicOISuperstitiFiltroC9A,
            [Description("Ricostituzione Org. Int. Anticipate Filtro C9A")]
            RicOIAnticipateFiltroC9A,
            Nessuno40 = 40,
            Nessuno41 = 41,
            Nessuno42 = 42,
            Nessuno43 = 43,
            Nessuno44 = 44,
            Nessuno45 = 45,
            Nessuno46 = 46,
            Nessuno47 = 47,
            Nessuno48 = 48,
            [Description("Ricostituzioni VOPGI con filtro L80")]
            RicVOPGIFiltroL80
        }

        public static string GetDescription(System.Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static Categoria? GetCategoria(string categoria)
        {
            if (String.IsNullOrEmpty(categoria))
                return null;

            switch (categoria)
            {
                case "VO":
                    return Categoria.VO;
                case "VOP":
                    return Categoria.VOP;
                case "VOMIN":
                    return Categoria.VOMIN;
                case "SOMIN":
                    return Categoria.SOMIN;
                case "VR":
                    return Categoria.VR;
                case "VOART":
                    return Categoria.VOART;
                case "VOCOM":
                    return Categoria.VOCOM;
                case "VOAUT":
                    return Categoria.VOAUT;
                case "VDAI":
                    return Categoria.VDAI;
                case "IDAI":
                    return Categoria.IDAI;
                case "SDAI":
                    return Categoria.SDAI;
                case "SR":
                    return Categoria.SR;
                case "IR":
                    return Categoria.IR;
                case "VOCRED":
                    return Categoria.VOCRED;
                case "CRED27":
                    return Categoria.CRED27;
                case "VOCOOP":
                    return Categoria.VOCOOP;
                case "COOP28":
                    return Categoria.COOP28;
                case "VOESO":
                    return Categoria.VOESO;
                case "IOART":
                    return Categoria.IOART;
                case "SOART":
                    return Categoria.SOART;
                case "SOAUT":
                    return Categoria.SOAUT;
                case "IOAUT":
                    return Categoria.IOAUT;
                case "VESO33":
                    return Categoria.VESO33;
                case "VESO92":
                    return Categoria.VESO92;
                case "VOCUM":
                    return Categoria.VOCUM;
                case "IOCUM":
                    return Categoria.IOCUM;
                case "SOCUM":
                    return Categoria.SOCUM;
                case "APE":
                    return Categoria.APE;
                case "VOSPETT":
                    return Categoria.VOSPETT;
                case "VESO29":
                    return Categoria.VESO29;
                case "ESOTEL":
                    return Categoria.ESOTEL;
                case "ESOAMB":
                    return Categoria.ESOAMB;
                case "VOSPED":
                    return Categoria.VOSPED;
                case "IOSPED":
                    return Categoria.IOSPED;
                case "SOSPED":
                    return Categoria.SOSPED;
                case "INDCOM":
                    return Categoria.INDCOM;
                case "ESPA":
                    return Categoria.ESPA;
                case "VOTOT":
                    return Categoria.VOTOT;
                case "SOTOT":
                    return Categoria.SOTOT;
                case "IOTOT":
                    return Categoria.IOTOT;
                case "IOP":
                    return Categoria.IOP;
                case "SOP":
                    return Categoria.SOP;
                case "VOST":
                    return Categoria.VOST;
                case "VOBIS":
                    return Categoria.VOBIS;
                case "IOBIS":
                    return Categoria.IOBIS;
                case "VMP":
                    return Categoria.VMP;
                case "IMP":
                    return Categoria.IMP;
                case "VOPGI":
                    return Categoria.VOPGI;
                case "IOPGI":
                    return Categoria.IOPGI;
                case "SOPGI":
                    return Categoria.SOPGI;
                case "ESOPMI":
                    return Categoria.ESOPMI;
                default:
                    return null;
            }
        }

        public enum TipoUnicarpe
        {
            Not,
            Yes,
            Automatica,
            Manuale
        }

        public static TipoUnicarpe IsDomandaUnicarpe(AreaTitolare.DatiPensione datiPensione, bool dettaglio)
        {
            TipoUnicarpe tipo = TipoUnicarpe.Not;
            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value)
            {
                if (dettaglio)
                {
                    if (datiPensione.TipoLetturaUnicarpe.HasValue &&
                        (datiPensione.TipoLetturaUnicarpe.Value == 'L' || datiPensione.TipoLetturaUnicarpe.Value == 'H' || datiPensione.TipoLetturaUnicarpe.Value == 'G' ||
                        datiPensione.TipoLetturaUnicarpe.Value == 'A' || datiPensione.TipoLetturaUnicarpe.Value == 'D'))
                        tipo = TipoUnicarpe.Automatica;
                    else
                        tipo = TipoUnicarpe.Manuale;
                }
                else
                    tipo = TipoUnicarpe.Yes;
            }
            return tipo;
        }

        public static TipoUnicarpe IsDomandaUnicarpe(bool? flagUnicarpe, char? tipoLettura, bool dettaglio)
        {

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione.FlagUnicarpe = flagUnicarpe;
            datiPensione.TipoLetturaUnicarpe = tipoLettura;
            return IsDomandaUnicarpe(datiPensione, dettaglio);
        }

        public static bool IsDomandaProvvisoria(bool? IsProvvisoria)
        {
            if (IsProvvisoria.HasValue && IsProvvisoria.Value)
                return true;
            else
                return false;
        }

        public static bool CheckDatiPeco_FunzioneC(string CodFase, string SiglaCategoria, string Gruppo, string Prodotto, string Tipo)
        {
            // memo 74-2025 88-2025
            bool retVal = false;
            bool Totalizzazione = Utility.IsDomandaTotalizzazione(SiglaCategoria); //&& datiPensione.IsTotAutomatica.GetValueOrDefault());
            bool PL_Vecchiaia = Gruppo == "0001";
            bool PL_InabilitaAssegniOrdinariInvalidita = Gruppo == "0002";
            bool PL_PensioniSuperstitiIndirette = Gruppo == "0003" && Prodotto == "0022";
            bool Trasformazione = false;
            if ((IsRiaperturaDomanda(CodFase) && PL_Vecchiaia) ||
                (IsRiaperturaDomanda(CodFase) && PL_InabilitaAssegniOrdinariInvalidita) ||
                (IsRiaperturaDomanda(CodFase) && PL_PensioniSuperstitiIndirette))
            {
                Trasformazione = true;
            }
            List<string> ListaExeptSigleRicostituzione = new List<string>() { "VOCRED", "VOCOOP", "VOESO", "CRED27", "COOP28", "VESO29", "ESOAMB", "ESOTEL", "VESO33", "VESO92", "ESPA", "VOTOT", "IOTOT", "SOTOT" };
            bool RicostituzioneContributiva = IsRicostituzione_MotiviContributivi(Gruppo, Prodotto) && ListaExeptSigleRicostituzione.Contains(SiglaCategoria) == false;
            bool RicostituzioniDiInabilitaSupplemento = (Gruppo == "0031" && Prodotto == "0302" && Tipo == "0001") && ListaExeptSigleRicostituzione.Contains(SiglaCategoria) == false;


            if (Totalizzazione == false &&
                (PL_Vecchiaia
                || PL_InabilitaAssegniOrdinariInvalidita
                || PL_PensioniSuperstitiIndirette
                || Trasformazione || RicostituzioneContributiva || RicostituzioniDiInabilitaSupplemento))
            {
                retVal = true;
            }

            return retVal;
        }

        public static bool checkMemo74_88(string CodFase, string Gruppo, string Prodotto, string Caratterizzazione, string TipoLetturaUnicarpe)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(TipoLetturaUnicarpe) || TipoLetturaUnicarpe.Trim() == "C")
            {
                bool Trasformazione = IsRiaperturaDomanda(CodFase);
                bool PL_Vecchiaia = Gruppo == "0001";
                bool PL_InabilitaAssegniOrdinariInvalidita = Gruppo == "0002";
                bool PL_PensioniSuperstitiIndirette = Gruppo == "0003" && Prodotto == "0022";
                bool CheckCaratterizzazione = string.IsNullOrEmpty(Caratterizzazione) ? false : Caratterizzazione.Substring(2, 1) == "1";  //AggPECO => TipCert = "POS"

                if (CheckCaratterizzazione && Trasformazione == false && (PL_Vecchiaia || PL_InabilitaAssegniOrdinariInvalidita || PL_PensioniSuperstitiIndirette))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public static bool IsRiaperturaDomanda(string fase)
        {
            if (!string.IsNullOrEmpty(fase) && (fase == "0060" || fase == "0062" || fase == "0063"))
                return true;

            return false;
        }

        public static bool IsRicostituzione_MotiviContributivi(string gruppo, string prodotto)
        {
            if (gruppo == "0031" &&
                (prodotto == "0107" || prodotto == "0307" || prodotto == "0407"))
                return true;
            return false;
        }

        internal static Boolean CheckNDomus(String ndomus, out String sErrore)
        {
            sErrore = string.Empty;
            if (String.IsNullOrEmpty(ndomus))
            { //numero domanda non inserito
                sErrore = "Inserire un numero di domanda";
                return false;
            }
            if (ndomus.Length != 13)//numero caratteri non valido
            {
                sErrore = "Numero Domanda: formato numero domanda non valido";
                return false;
            }
            if (!CheckCorrettezzaNumerico(ndomus, out sErrore))
            {
                sErrore = "Campo in formato non numerico";
                return false;
            }

            if (String.IsNullOrEmpty(sErrore))
            {
                Regex Rex = new Regex(@"^[1-9]{1}[0-9]{12}$");
                Match M = Rex.Match(ndomus);
                if (M.Success)
                    return true;
                else
                {
                    sErrore = "Il numero di domanda non può avere come prima cifra 0";
                    return false;
                }
            }
            return true;
        }

        internal static Boolean CheckCorrettezzaNumerico(String numerico, out string sErrore)
        {
            sErrore = string.Empty;
            try
            {
                Int32 interoEstratto = Int32.Parse(numerico);
            }
            catch (Exception)
            {
                try
                {
                    Int64 intero64Estratto = Int64.Parse(numerico);
                }
                catch (Exception)
                {
                    sErrore = "Campo in formato non numerico";
                    return false;
                }
            }
            return true;
        }

        internal static Boolean CheckCodiceFiscale(String sCodiceFiscale, out string sErrore)
        {
            try
            {
                sErrore = string.Empty;
                if (String.IsNullOrEmpty(sCodiceFiscale.Trim()))
                {
                    sErrore = "Inserire un codice fiscale";
                    return false;
                }



                Regex Rex = new Regex(@"^([A-Za-z]{6}[0-9lmnpqrstuvLMNPQRSTUV]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9lmnpqrstuvLMNPQRSTUV]{2}[A-Za-z]{1}[0-9lmnpqrstuvLMNPQRSTUV]{3}[A-Za-z]{1})$");
                Match M = Rex.Match(sCodiceFiscale);
                if (M.Success)
                    return true;
                else
                {
                    sErrore = "Inserire un codice fiscale in formato corretto";
                    return false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Utility, Errore nel metodo checkCodiceFiscale" + ex);
            }
        }

        internal static Boolean CheckAnagrafica(String nome, String cognome, String dataNascita, out String sErrore)
        {
            try
            {
                sErrore = string.Empty;
                if (String.IsNullOrEmpty(nome))
                {
                    sErrore = "Inserire un nome";
                    return false;
                }
                else
                {
                    if (!checkStringa((nome), out sErrore))
                    {
                        return false;
                    }
                    if (nome.Length < 3)
                    {
                        sErrore = "Nome: il campo deve essere lungo almeno tre caratteri";
                        return false;
                    }
                }
                if (String.IsNullOrEmpty(cognome))
                {
                    sErrore = "Inserire un cognome";
                    return false;
                }
                else
                {
                    if (!checkStringa((cognome), out sErrore))
                    {
                        return false;
                    }
                }
                if (String.IsNullOrEmpty(dataNascita))
                {
                    return true;
                }
                else
                {
                    if (checkDataMaxActual(dataNascita, out sErrore))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Utility, Errore nel metodo checkAnagrafica" + ex);
            }
        }

        internal static Boolean checkDataMaxActual(String sDataDaControllare, out String sErrore)
        {
            sErrore = string.Empty;
            DateTime dataDaControllare = ConvertString2Data_withMinValue(sDataDaControllare);
            if (dataDaControllare > DateTime.Now)
            {
                sErrore = "Data inserita posteriore a quella odierna";
                return false;
            }
            else
                return true;
        }

        private static Boolean confrontoData(DateTime? data1, DateTime? data2, out String sErrore)
        {
            sErrore = string.Empty;
            if (data1 >= data2)
                return true;
            else
                return false;

        }

        public static DateTime ConvertString2Data_withMinValue(String sData1)
        {
            DateTime? data1 = GetDateFromString(sData1);
            return data1.HasValue ? data1.Value : new DateTime();
        }

        public static DateTime? GetDateFromString(string sData)
        {
            DateTime? data = (DateTime?)null;
            try
            {
                if (sData.Length == 10)
                {
                    data = new DateTime(int.Parse(sData.Substring(6)), int.Parse(sData.Substring(3, 2)), int.Parse(sData.Substring(0, 2)));

                }
                else if (sData.Length == 9)
                {
                    Regex Rex = new Regex(@"^[0-9]{1}[/]{1}[0-9]{2}[/]{1}[0-9]{4}$");
                    Match M = Rex.Match(sData);
                    if (M.Success)
                    {
                        data = new DateTime(int.Parse(sData.Substring(5)), int.Parse(sData.Substring(2, 2)), int.Parse(sData.Substring(0, 1)));
                    }
                    else
                    {
                        Regex Rex2 = new Regex(@"^[0-9]{2}[/]{1}[0-9]{1}[/]{1}[0-9]{4}$");
                        Match M2 = Rex2.Match(sData);
                        if (M2.Success)
                            data = new DateTime(int.Parse(sData.Substring(5)), int.Parse(sData.Substring(3, 1)), int.Parse(sData.Substring(0, 2)));
                    }
                }
                else if (sData.Length == 8)
                {
                    Regex Rex = new Regex(@"^[0-9]{1}[/]{1}[0-9]{1}[/]{1}[0-9]{4}$");
                    Match M = Rex.Match(sData);
                    if (M.Success)
                    {
                        data = new DateTime(int.Parse(sData.Substring(4)), int.Parse(sData.Substring(2, 1)), int.Parse(sData.Substring(0, 1)));
                    }
                }
                else if (sData.Length == 7)
                {
                    Regex Rex = new Regex(@"^[0-9]{2}[/]{1}[0-9]{4}$");
                    Match M = Rex.Match(sData);
                    if (M.Success)
                    {
                        data = new DateTime(int.Parse(sData.Substring(3)), int.Parse(sData.Substring(0, 2)), 1);
                    }
                }
                else if (sData.Length == 6)
                {
                    Regex Rex = new Regex(@"^[1-9]{1}[/]{1}[0-9]{4}$");
                    Match M = Rex.Match(sData);
                    if (M.Success)
                    {
                        data = new DateTime(int.Parse(sData.Substring(2)), int.Parse(sData.Substring(0, 1)), 1);
                    }
                }
                //caso con orario
                else if (sData.Length == 19)
                {
                    data = new DateTime(int.Parse(sData.Substring(6, 4)), int.Parse(sData.Substring(3, 2)), int.Parse(sData.Substring(0, 2)), int.Parse(sData.Substring(11, 2)), int.Parse(sData.Substring(14, 2)), int.Parse(sData.Substring(17, 2)));
                }
            }
            catch (Exception)
            {
                return null;
            }

            return data;
        }

        private static Boolean checkStringa(String sStringaDaControllare, out String sErrore)
        {
            sErrore = string.Empty;
            Regex Rex = new Regex(@"^[A-Za-z ]+$");
            Match M = Rex.Match(sStringaDaControllare);
            if (M.Success)
            {
                return true;
            }
            else
            {
                sErrore = "Campo contenente caratteri non consentiti. ";
                return false;
            }
        }

        internal static Boolean CheckNTelefono(String sNumeroTelefono, out String sErrore)
        {
            sErrore = string.Empty;
            if (!String.IsNullOrEmpty(sNumeroTelefono))
            {
                Regex Rex = new Regex(@"^[0-9\+\ \/]+$");
                Match M = Rex.Match(sNumeroTelefono);
                if (M.Success)
                {
                    return true;
                }
                else
                {
                    sErrore = "Campo contenente caratteri non consentiti. ";
                    return false;
                }
            }
            else
            {
                return true;
            }


        }

        internal static Boolean CheckEmail(String sEmail, out String sErrore)
        {
            sErrore = string.Empty;
            if (!String.IsNullOrEmpty(sEmail))
            {

                Regex Rex = new Regex(@"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
                Match M = Rex.Match(sEmail);
                if (M.Success)
                {
                    return true;
                }
                else
                {
                    sErrore = "Campo contenente caratteri non consentiti. ";
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        internal static Boolean CheckDecorrenzaPensione(DateTime? Decorrenza, out String sErrore)
        {
            sErrore = string.Empty;
            if (!Decorrenza.HasValue || Decorrenza == DateTime.MinValue)
            {
                sErrore = "La decorrenza non può essere vuota";
                return false;
            }
            else
                return true;
        }

        public static bool AreAllColumnsEmpty(DataRow dr)
        {
            if (dr == null)
            {
                return true;
            }
            else
            {
                foreach (var value in dr.ItemArray)
                {
                    if (value != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static Presenter.SvrLiquidazione.AreaDecodifica ServizioGetDecodifica()
        {
            Presenter.SvrLiquidazione.AreaDecodifica rispostaDecodifica = new AreaDecodifica();
            Presenter.SvrLiquidazione.DecodificaClient objWS = new DecodificaClient();
            try
            {
                rispostaDecodifica = objWS.GetDecodifica();
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, "Utility, Errore nel metodo ServizioGetDecodifica");
            }
            finally
            {
                CloseClient(objWS);
            }

            return rispostaDecodifica;
        }

        public static short GetSedeOperatore()
        {
            string sedeStr = INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.PadLeft(4, '0').Substring(0, 4);
            return short.Parse(sedeStr);
        }

        public static short GetCentroOperativoOperatore()
        {
            string coStr = INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.PadLeft(4, '0').PadRight(6, '0').Substring(4, 2);
            return short.Parse(coStr);
        }

        internal static string GetMatricolaOperatore()
        {
            string matricola = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            return matricola;
        }

        public static string GetCFOperatore()
        {
            string CFOperatore = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).FiscalCode;
            return CFOperatore;
        }

        public static int GetSedeDiAppartenenzaOperatore()
        {
            int sede = 0;
            string sedeAppartenenza = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode.PadLeft(4, '0').PadRight(6, '0').Substring(0, 6);
            Int32.TryParse(sedeAppartenenza, out sede);
            return sede;
        }

        public static Enum.TipoAppartenenzaRuolo GetTipoAppartenenzaRuolo(object ruolo)
        {
            Enum.TipoAppartenenzaRuolo tipoRuolo = INPS.Pensioni.LiquidazionePensione.Presenter.Enum.TipoAppartenenzaRuolo.ASSENTE;

            if (ruolo != null)
            {
                switch ((Enum.Ruoli)ruolo)
                {
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P4677:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P4678:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8974:
                        tipoRuolo = INPS.Pensioni.LiquidazionePensione.Presenter.Enum.TipoAppartenenzaRuolo.FS;
                        break;
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8854:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8856:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8975:
                        tipoRuolo = INPS.Pensioni.LiquidazionePensione.Presenter.Enum.TipoAppartenenzaRuolo.AGO;
                        break;
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8855:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8857:
                    case INPS.Pensioni.LiquidazionePensione.Presenter.Enum.Ruoli.P8976:
                        tipoRuolo = INPS.Pensioni.LiquidazionePensione.Presenter.Enum.TipoAppartenenzaRuolo.CI;
                        break;
                }
            }

            return tipoRuolo;
        }

        public enum TipoFelpe
        {
            AMG = 1,
            SIN = 2,
            SPI = 3
        }

        public static bool DataSuccessivaA(DateTime data1, DateTime data2)
        {
            if (DateTime.Compare(data1.Date, data2.Date) < 0)
                return false;
            return true;
        }

        public static bool DataStrettamenteSuccessivaA(DateTime data1, DateTime data2)
        {
            if (DateTime.Compare(data1.Date, data2.Date) <= 0)
                return false;
            return true;
        }

        public static bool IsDomandaCumulo(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOCUM || GetCategoria(categoria.Trim()) == Categoria.IOCUM || GetCategoria(categoria.Trim()) == Categoria.SOCUM)
                    return true;

            return false;
        }

        public static bool IsDomandaCTPS(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (categoria.Trim() == Categoria.VOCTPS.ToString() || categoria.Trim() == Categoria.IOCTPS.ToString() || categoria.Trim() == Categoria.SOCTPS.ToString())
                    return true;

            return false;
        }

        public static bool IsCTPSPrivilegio(AreaTitolare.DatiPensione datiPensione, string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && ((categoria.Trim() == Categoria.IOCTPS.ToString() && datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0046") ||
               (categoria.Trim() == Categoria.SOCTPS.ToString() && datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022" && datiPensione.CodeTipo == "0046")))
                return true;

            return false;
        }
        public static bool IsDomandaCPDEL(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (categoria.Trim() == Categoria.VOCPDEL.ToString() || categoria.Trim() == Categoria.IOCPDEL.ToString() || categoria.Trim() == Categoria.SOCPDEL.ToString())
                    return true;

            return false;
        }

        public static bool IsDomandaVOCUM(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOCUM)
                    return true;

            return false;
        }

        public static bool IsDomandaIOCUM(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.IOCUM)
                    return true;

            return false;
        }

        public static bool IsDomandaSOCUM(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.SOCUM)
                    return true;

            return false;
        }

        public static bool IsDomandaINPGI(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOPGI || GetCategoria(categoria.Trim()) == Categoria.SOPGI || GetCategoria(categoria.Trim()) == Categoria.IOPGI)
                    return true;

            return false;
        }

        public static bool IsDomandaVOPGI(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOPGI)
                    return true;

            return false;
        }

        public static bool IsDomandaSupplementoCumulo(string categoria, AreaTitolare.DatiPensione datiPensione)
        {
            if (IsDomandaVOCUM(categoria) && datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0102" && datiPensione.CodeTipo == "0001")
                return true;
            return false;
        }

        public static bool IsDomandaAPESociale(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.APE)
                    return true;

            return false;
        }

        public static bool IsDomandaEsodo(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOCRED ||
                    GetCategoria(categoria.Trim()) == Categoria.VOCOOP ||
                    GetCategoria(categoria.Trim()) == Categoria.VOESO ||
                    GetCategoria(categoria.Trim()) == Categoria.CRED27 ||
                    GetCategoria(categoria.Trim()) == Categoria.COOP28 ||
                    GetCategoria(categoria.Trim()) == Categoria.VESO33 ||
                    GetCategoria(categoria.Trim()) == Categoria.VESO92 ||
                    GetCategoria(categoria.Trim()) == Categoria.VESO29 ||
                    GetCategoria(categoria.Trim()) == Categoria.ESOTEL ||
                    GetCategoria(categoria.Trim()) == Categoria.ESOAMB ||
                    GetCategoria(categoria.Trim()) == Categoria.ESPA ||
                    GetCategoria(categoria.Trim()) == Categoria.ESOPMI)
                    return true;
            return false;
        }

        public static bool IsDomandaESOAMB(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.ESOAMB)
                    return true;
            return false;
        }

        public static bool IsDomandaESOTEL(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.ESOTEL)
                    return true;
            return false;
        }

        public static bool IsDomandaVESO33(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VESO33)
                    return true;

            return false;
        }

        public static bool IsDomandaVESO29(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VESO29)
                    return true;

            return false;
        }

        public static bool IsDomandaVESO92(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VESO92)
                    return true;

            return false;
        }

        public static bool IsDomandaVESO92_L92(string categoria, string filtro)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && !string.IsNullOrEmpty(filtro) && !string.IsNullOrEmpty(filtro.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VESO92 && filtro == "L92")
                    return true;
            return false;
        }

        public static bool IsDomandaESPA(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.ESPA)
                    return true;

            return false;
        }

        public static bool IsDomandaESPA_L26(string categoria, string filtro)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && !string.IsNullOrEmpty(filtro) && !string.IsNullOrEmpty(filtro.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.ESPA && filtro == "L26")
                    return true;
            return false;
        }

        public static bool IsDomandaESOPMI(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.ESOPMI)
                    return true;

            return false;
        }

        public static bool IsIsoPensioneWithGP2BB05(string categoria, string GP2BB05)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if ((GetCategoria(categoria.Trim()) == Categoria.VESO92 || GetCategoria(categoria.Trim()) == Categoria.VOESO || GetCategoria(categoria.Trim()) == Categoria.VESO29)
                    && (GP2BB05 == "L1" || GP2BB05 == "E"))
                    return true;
            return false;
        }

        public static bool IsDomandaVESO92WithGP2BB05(string categoria, string GP2BB05)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VESO92 && (GP2BB05 == "L1" || GP2BB05 == "E"))
                    return true;
            return false;
        }

        //due fuzioni separate per VESO29/VOESO e VESO92 perchè per queste ultime bisogna abbinare la verifica della lista aziende
        public static bool IsDomandaIsoPensioneRicWithScadenzaAssegnoGGMMAAAA(string categoria, string gruppo, bool? IsScadenzaAssegnoConGiorno)
        {
            if (gruppo == "0031" && (IsDomandaVESO29(categoria) || IsDomandaVOESO(categoria)) && IsScadenzaAssegnoConGiorno.GetValueOrDefault())
                return true;
            return false;
        }

        public static bool IsDomandaVESO92RicWithScadenzaAssegnoGGMMAAAA(string categoria, string gruppo, bool? IsScadenzaAssegnoConGiorno)
        {
            if (gruppo == "0031" && IsDomandaVESO92(categoria) && IsScadenzaAssegnoConGiorno.GetValueOrDefault())
                return true;
            return false;
        }

        public static bool IsDomandaESPARicWithScadenzaAssegnoGGMMAAAA(string categoria, string gruppo, bool? IsScadenzaAssegnoConGiorno)
        {
            if (gruppo == "0031" && IsDomandaESPA(categoria) && IsScadenzaAssegnoConGiorno.GetValueOrDefault())
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaInComputo(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0045")
                return true;

            return false;
        }

        public static bool IsDomandaAUTAnticipataInComputo(AreaTitolare.DatiPensione datiPensione, string siglaCategoria, bool filtroUgualeAV)
        {
            if (datiPensione == null)
                return false;

            if (siglaCategoria.Trim().ToUpperInvariant() == "VOAUT" && datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0045" &&
                ((filtroUgualeAV && datiPensione.CodiceTipoRichiesta == "AV") || (!filtroUgualeAV && datiPensione.CodiceTipoRichiesta != "AV")))
                return true;

            return false;
        }

        public static bool IsDomandaAnzianitaInComputo(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0045")
                return true;

            return false;
        }

        public static bool IsRicEsenzioneFiscaleVittimeDelDovere(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0108" && datiPensione.CodeTipo == "0166") ||
                 (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0308" && datiPensione.CodeTipo == "0166") ||
                 (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0408" && datiPensione.CodeTipo == "0166"))
                return true;

            return false;
        }

        public static bool IsDomandaSpacchettamentoENPALS(bool isDomandaENPALS, string siglaCategoria)
        {
            if (isDomandaENPALS && siglaCategoria.ToUpperInvariant().StartsWith("S"))
                return true;

            return false;
        }

        public static bool IsDomandaSpacchettamentoINPDAP(bool isDomandaINPDAP, string siglaCategoria)
        {
            if (isDomandaINPDAP && siglaCategoria.ToUpperInvariant().StartsWith("S"))
                return true;

            return false;
        }

        public static bool IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && (datiPensione.CodeTipo == "0170" || datiPensione.CodeTipo == "0161"))
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && (datiPensione.CodeTipo == "0170" || datiPensione.CodeTipo == "0161"))
                return true;
            return false;
        }

        public static bool IsDomandaAnticipataConOpzionePL(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0030")
                return true;

            return false;
        }

        public static bool IsDomandaManualeInvaliditaOver80(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Enum.TipoAppartenenzaRuolo.AGO)
                return false;

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not && ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0001") ||
                (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0002")) &&
                datiPensione.CodiceTipoRichiesta == "C8")
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaENAV(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (datiPensione == null)
                return false;

            if ((siglaCategoria.Trim().ToUpperInvariant() == "VO" || siglaCategoria.Trim().ToUpperInvariant() == "VR" ||
                siglaCategoria.Trim().ToUpperInvariant() == "VOART" || siglaCategoria.Trim().ToUpperInvariant() == "VOCOM") &&
                datiPensione.CodiceTipoRichiesta == "EN" && datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0001")
                return true;

            return false;
        }

        public static bool IsRiaperturaRicTRF_Benefici16_17(AreaTitolare.DatiPensione datiPensione, string beneficio, string gruppo, string codFase)
        {
            bool retVal = false;
            if (datiPensione == null)
                return retVal;

            if (beneficio == "16" || beneficio == "17")
            {
                string ctrlAbilitazioneRIC_TRFMemo16_2020 = string.Empty;
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo16_2020", out ctrlAbilitazioneRIC_TRFMemo16_2020);

                if (ctrlAbilitazioneRIC_TRFMemo16_2020 != null && !String.IsNullOrEmpty(ctrlAbilitazioneRIC_TRFMemo16_2020) && !String.IsNullOrEmpty(ctrlAbilitazioneRIC_TRFMemo16_2020.Trim()) &&
                            ctrlAbilitazioneRIC_TRFMemo16_2020 == "SI")
                {
                    Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    if (tipoAppartenenza == Enum.TipoAppartenenzaRuolo.AGO)
                    {
                        bool isRiapertura = Utility.IsRiaperturaDomanda(codFase);
                        if (Utility.IsRicostituzione(gruppo) || isRiapertura)
                            retVal = true;
                    }
                }
            }

            return retVal;
        }

        public static bool IsDomandaAnticipataEsattoriali(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (datiPensione == null)
                return false;

            if ((siglaCategoria.Trim().ToUpperInvariant() == "VO" || siglaCategoria.Trim().ToUpperInvariant() == "VOCOM" ||
                siglaCategoria.Trim().ToUpperInvariant() == "VOART" || siglaCategoria.Trim().ToUpperInvariant() == "VR") &&
                datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0001" && datiPensione.CodiceTipoRichiesta == "ES")
                return true;
            return false;
        }

        #region ModalitaPagamento

        public static Boolean CheckAbiCabFrazionario(string abiCabFrazionario, out string sErrore)
        {
            Regex Rex = new Regex(@"^[0-9]{5}$");
            Match M = Rex.Match(abiCabFrazionario);
            if ((M.Success == false) || (String.Compare(abiCabFrazionario, "00000", false, System.Globalization.CultureInfo.CurrentUICulture) == 0))
            {
                sErrore = "Il codice  deve essere composto da 5 caratteri numerici diversi da 00000";
                return false;
            }
            else
            {
                sErrore = "";
                return true;
            }
        }

        public static Boolean CheckCassaSede(string abiCabFrazionario, out string sErrore)
        {
            Regex Rex = new Regex(@"^[0-9]{7}$");
            Match M = Rex.Match(abiCabFrazionario);
            if ((M.Success == false) || (String.Compare(abiCabFrazionario, "0000000", false, System.Globalization.CultureInfo.CurrentUICulture) == 0))
            {
                sErrore = "Il codice  deve essere composto da 7 caratteri numerici diversi da 0000000";
                return false;
            }
            else
            {
                sErrore = "";
                return true;
            }
        }

        public static Boolean CheckBic(string bic, out string sErrore)
        {
            Regex Rex = new Regex(@"^[A-Z a-z 0-9]{8,11}$");
            Match M = Rex.Match(bic);
            if (M.Success)
            {
                sErrore = "";
                return true;
            }
            else
            {
                sErrore = "Bic: codice bic in formato non valido";
                return false;
            }
        }

        public static String WriteAbiCab(int? number)
        {
            if (number.HasValue)
                return number.Value.ToString().PadLeft(5, '0');
            else
                return "";
        }
        #endregion ModalitaPagamento

        #region controlli Liquidazione pensione - Dati generici

        internal static Boolean checkVerify(string sVerify, out string sErrore)
        {
            sErrore = string.Empty;
            if (String.IsNullOrEmpty(sVerify))
            {
                sErrore = "Il campo è obbligatorio";
                return false;
            }
            return true;
        }

        internal static Boolean CheckDecorrenzaOriginaria(string sDecorrenzaOriginaria, out string sErrore)
        {
            sErrore = String.Empty;
            Regex Rex = new Regex(@"^[0-9]{2}[/]{1}[0-9]{4}$");
            Match M = Rex.Match(sDecorrenzaOriginaria);
            if (!(M.Success))
            {
                sErrore = "Decorrenza Originaria: Data in formato non valido";
                return false;
            }
            if (!CheckMese(sDecorrenzaOriginaria))
            {
                sErrore = "Decorrenza Originaria: Mese non valido";
                return false;
            }

            return true;
        }

        internal static Boolean CheckDecorrenzaArretrati(string sDecorrenzaArretrati, out string sErrore)
        {
            sErrore = string.Empty;
            if (!CheckDataMMAAAA(sDecorrenzaArretrati))
            {
                sErrore = "Decorrenza Arretrati: data in formato non valido";
                return false;
            }
            if (!(CheckMese(sDecorrenzaArretrati)))
            {
                sErrore = "Decorrenza Arretrati: mese non valido";
                return false;
            }
            return true;
        }

        internal static Boolean CheckScadenzaRevisioneSanitaria(string sData, out string sErrore)
        {
            sErrore = String.Empty;
            if (!CheckDataMMAAAA(sData))
            {
                sErrore = "Scadenza Revisione Sanitaria: data in formato non valido";
                return false;
            }
            if (!CheckMese(sData))
            {
                sErrore = "Scadenza Revisione Sanitaria: mese non valido";
            }
            return true;
        }

        internal static Boolean CheckDataCompletezza(string sData, out string sErrore)
        {
            sErrore = string.Empty;
            if (!CheckDataGGMMAAAA(sData))
            {
                sErrore = "Data completezza: data in formato non valido";
                return false;
            }
            if (!CheckMese(sData))
            {
                sErrore = "Data completezza: mese non valido";
                return false;
            }
            if (!CheckGiorno(sData))
            {
                sErrore = "Data completezza: giorno non valido";
                return false;
            }
            if (!checkDataMaxActual(sData, out sErrore))
            {
                sErrore = "Data completezza: la data inserita non può essere anteriore a quella attuale";
                return false;
            }
            return true;
        }

        internal static Boolean CheckDataInteressiLegali(string sData, out string sErrore)
        {
            sErrore = String.Empty;
            if (!CheckDataGGMMAAAA(sData))
            {
                sErrore = "Data interessi legali: formato data non valido";
                return false;
            }
            if (!CheckMese(sData))
            {
                sErrore = "Data interessi legali: mese non valido";
                return false;
            }
            if (!CheckGiorno(sData))
            {
                sErrore = "Data interessi legali: giorno non valido";
                return false;
            }
            return true;

        }

        internal static Boolean CheckTipoCalcolo(string sTipoCalcolo, out string sErrore)
        {
            sErrore = string.Empty;
            if (String.IsNullOrEmpty(sTipoCalcolo))
            {
                sErrore = "Tipo Calcolo: scegliere il tipo di calcolo";
                return false;
            }
            else
                return true;
        }

        internal static Boolean CheckCodiceNatura(string sCodice, out string sErrore)
        {
            sErrore = string.Empty;
            Regex Rex = new Regex(@"^[0-9][a-z][A-Z]{0,1}$");
            Match M = Rex.Match(sCodice);
            if (M.Success)
                return true;
            else
            {
                sErrore = "Codice natura: campo in formato non valido";
                return false;
            }
        }

        internal static Boolean CheckDecorrenzaEliminazioneContestuale(string sData, out string sErrore)
        {
            sErrore = String.Empty;
            if (!CheckDataMMAAAA(sData))
            {
                sErrore = "Decorrenza eliminazione contestuale: formato data non valido";
                return false;
            }
            if (!CheckMese(sData))
            {
                sErrore = "Decorrenza eliminazione contestuale: mese non valido";
                return false;
            }
            if (!checkDataMaxActual(sData, out sErrore))
            {
                sErrore = "Decorrenza eliminazione contestuale: la data inserita non può essere posteriore a quella odierna";
            }
            return true;
        }

        internal static Boolean CheckDataEventoEliminazioneContestuale(string sData, out string sErrore)
        {
            sErrore = string.Empty;
            if (!CheckDataGGMMAAAA(sData))
            {
                sErrore = "Data evento: formato data non valido";
                return false;
            }
            if (!CheckMese(sData))
            {
                sErrore = "Data evento: mese non valido";
                return false;
            }
            if (!CheckGiorno(sData))
            {
                sErrore = "Data evento: giorno non valido";
                return false;
            }
            if (!checkDataMaxActual(sData, out sErrore))
            {
                sErrore = "Data evento: la data inserita non può essere posteriore a quella odierna";

            }
            return true;

        }

        internal static Boolean CheckDataMMAAAA(string sData)
        {
            Regex Rex = new Regex(@"^[0-9]{2}[/]{1}[0-9]{4}$");
            Match M = Rex.Match(sData);
            if (M.Success)
                return true;
            else
                return false;
        }

        internal static Boolean CheckDataGGMMAAAA(string sData)
        {
            Regex Rex = new Regex(@"^[0-9]{2}[/]{1}[0-9]{2}[/]{1}[0-9]{4}$");
            Match M = Rex.Match(sData);
            if (M.Success)
                return true;
            else
                return false;
        }

        internal static Boolean CheckTrimestreRequisitiAnno(string sAnno, out string sErrore)
        {
            sErrore = String.Empty;
            Regex Rex = new Regex(@"^[0-9]{0,4}$");
            Match M = Rex.Match(sAnno);
            if (M.Success)
                return true;
            else
            {
                sErrore = "Trimestre Requisiti - Anno: formato anno non valido";
                return false;
            }
        }

        internal static Boolean CheckAnniAnzianita(string sNAnni, out string sErrore)
        {
            sErrore = string.Empty;
            Regex Rex = new Regex(@"^[0-9]{2}$");
            Match M = Rex.Match(sNAnni);
            if (M.Success)
            {
                if (Int32.Parse(sNAnni) > 35)
                {
                    return true;
                }
                else
                {
                    sErrore = "Anni Anzianità: il numero di anni di anzianità deve essere maggiore di 35";
                    return false;
                }
            }
            else
            {
                sErrore = "Anni Anzianità: formato anno non valido";
                return false;
            }
        }

        private static Boolean CheckMese(string sData)
        {
            bool result = true;
            if (sData.Length == 7)
            {
                string[] mese = sData.Split('/');
                result = CheckNMese(mese[0]);
            }
            else if (sData.Length == 10)
            {
                string[] mese = sData.Split('/');
                result = CheckNMese(mese[1]);
            }
            return result;
        }

        private static Boolean CheckNMese(string nMese)
        {
            if ((int.Parse(nMese) > 0) && (int.Parse(nMese) < 13))
                return true;
            else
                return false;
        }

        private static Boolean CheckGiorno(string sData)
        {
            Boolean result;
            string[] giorno = sData.Split('/');
            result = CheckNGiorno(giorno[0], giorno[1]);
            return result;


        }


        private static Boolean CheckNGiorno(string sGiorno, string sMese)
        {
            int mese = int.Parse(sMese);
            int giorno = int.Parse(sGiorno);
            int ngiorni;
            if (mese == 2)
            {
                ngiorni = 28;
            }
            else if (mese == 04 || mese == 4 || mese == 06 || mese == 6 || mese == 09 || mese == 9 ||
        mese == 11)
            {
                ngiorni = 30;
            }
            else
                ngiorni = 31;

            switch (ngiorni)
            {
                case 28:
                    if (giorno > 28)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 30:
                    if (giorno > 30)
                    {
                        return false;
                    }
                    else
                        return true;
                case 31:
                    if (giorno > 31)
                        return false;
                    else
                        return true;
            }
            return false;
        }
        #endregion controlli Liquidazione pensione - Dati generici

        #region Controllo Sede Operatore - Sede Domanda
        public static bool ControlloSedi(short sedeDomanda, short coDomanda)
        {
            if (ConfigurationManager.AppSettings["BypassControlloSedi"] != null &&
                ConfigurationManager.AppSettings["BypassControlloSedi"] == "SI")
                return true;

            short sedeOperatore = GetSedeOperatore();
            short coOperatore = GetCentroOperativoOperatore();
            if (sedeOperatore == sedeDomanda && coOperatore == coDomanda)
                return true;
            else
                return false;
        }
        #endregion Controllo Sede Operatore - Sede Domanda


        /// <summary>
        /// Verifica se la domanda è una Pensione di Reversibilità (Gruppo = 0003 Prodotto = 0021)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>True se il gruppo è 0003 e il prodotto è 0021</returns>
        public static bool IsDomandaReversibilita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021")
                return true;
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una Pensione indiretta (Gruppo = 0003 Prodotto = 0022)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>True se il gruppo è 0003 e il prodotto è 0022</returns>
        public static bool IsDomandaIndiretta(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022")
                return true;
            return false;
        }

        public static bool IsDomandaRiliquidazioneIndiretta(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0422" && datiPensione.CodeTipo == "0026")
                return true;
            return false;
        }

        public static bool IsDomandaSuperstiti(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0003")
                return true;
            return false;
        }

        public static bool IsDomandaSuperstiti(string CodeGruppo)
        {
            if (CodeGruppo == "0003")
                return true;
            return false;
        }

        //ENG - Aggiornamento Memo 68/2022 IOPGI
        public static bool IsDomandaIOPGI(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.IOPGI)
                    return true;

            return false;
        }

        //ENG - Aggiornamento Memo 68/2022 IOPGI
        public static bool IsDomandaIOPGI_AGI(string categoria, string filtro)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && !string.IsNullOrEmpty(filtro) && !string.IsNullOrEmpty(filtro.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && enumCategoria.Value == Categoria.IOPGI && filtro.ToUpperInvariant() == "AGI")
                    return true;
            }
            return false;
        }

        //ENG - Memo 32_a/2018
        public static bool IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(AreaTitolare.DatiPensione datiPensione, string categoria)
        {
            if (datiPensione == null)
                return false;

            if (Utility.IsDomandaVOCUM(categoria) && Utility.IsRicostituzione_MotiviContributivi(datiPensione)
                && datiPensione.CodeTipo == "0193")
                return true;

            return false;
        }

        //ENG - RIC VARIAZIONE DATI CONTITOLARI
        public static bool IsRicostituzioneVariazioneDatiContitolari(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0413" && datiPensione.CodeTipo == "0001")
                return true;

            return false;
        }

        //ENG - VOPGI AGI
        public static bool IsDomandaVOPGI_AGI(string categoria, string filtro, string dirittoAutonomo, string gp1aj11)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && enumCategoria.Value == Categoria.VOPGI)
                {
                    if (filtro.ToUpperInvariant() == "AGI" || (!string.IsNullOrEmpty(dirittoAutonomo) && !string.IsNullOrEmpty(dirittoAutonomo.Trim()) &&
                        dirittoAutonomo.Trim().ToUpperInvariant() == "DA") || (!String.IsNullOrEmpty(gp1aj11) && gp1aj11.Trim() == "1"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public delegate void CustomEventHandler(object sender, Utility.CustomEventArgs e);
        public class CustomEventArgs : EventArgs
        {
            public CustomEventArgs(object tipoApp, object tipoFondo)
            {
                this.TipoApp = tipoApp;
                this.TipoFondo = tipoFondo;
            }

            public object TipoApp { get; set; }
            public object TipoFondo { get; set; }
        }

        public delegate void EventHandlerMessage(object sender, Utility.EventMessageArgs e);
        public class EventMessageArgs : EventArgs
        {
            public EventMessageArgs()
            { }

            public EventMessageArgs(string message)
            {
                this.Message = message;
            }

            public string Message { get; set; }
        }


        #region Mapping tra le classi  DatiContribuzioneEnpals di Ago e Common
        public static Presenter.SvrLiquidazione.DatiContribuzioneEnpals GetDatiContribuzioneEnpalsSvrLiquidazione(Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals input)
        {
            Presenter.SvrLiquidazione.DatiContribuzioneEnpals output = null;
            if (input == null || (input.QuotaA == null && input.QuotaB == null && input.QuotaC == null))
                return output;

            output = new DatiContribuzioneEnpals();
            output.Tipologia = (input.Tipologia == SvrLiquidazioneAgo.TipologiaContribuzioneEnpals.SAI) ? SvrLiquidazione.TipologiaContribuzioneEnpals.SAI : SvrLiquidazione.TipologiaContribuzioneEnpals.SAS;

            if (input.QuotaA != null)
            {
                output.QuotaA = GetQuotaSvrLiquidazione(input.QuotaA);
            }
            if (input.QuotaB != null)
            {
                output.QuotaB = GetQuotaSvrLiquidazione(input.QuotaB);
            }
            if (input.QuotaC != null)
            {
                output.QuotaC = GetQuotaSvrLiquidazione(input.QuotaC);
            }
            return output;
        }

        private static Presenter.SvrLiquidazione.DatiContribuzioneEnpals.Quota GetQuotaSvrLiquidazione(Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals.Quota quotaInput)
        {
            Presenter.SvrLiquidazione.DatiContribuzioneEnpals.Quota quotaOutput = new DatiContribuzioneEnpals.Quota();
            quotaOutput.Enpals = quotaInput.Enpals;
            quotaOutput.Estera = quotaInput.Estera;
            quotaOutput.Figurativa = quotaInput.Figurativa;
            quotaOutput.Inps = quotaInput.Inps;
            quotaOutput.Ufficio = quotaInput.Ufficio;
            quotaOutput.Volontaria = quotaInput.Volontaria;
            quotaOutput.Enpals = quotaInput.Enpals;
            return quotaOutput;
        }

        public static Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals GetDatiContribuzioneEnpalsSvrLiquidazioneAgo(Presenter.SvrLiquidazione.DatiContribuzioneEnpals input)
        {
            Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals output = null;
            if (input == null || (input.QuotaA == null && input.QuotaB == null && input.QuotaC == null))
                return output;

            output = new Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals();
            output.Tipologia = (input.Tipologia == SvrLiquidazione.TipologiaContribuzioneEnpals.SAI) ? SvrLiquidazioneAgo.TipologiaContribuzioneEnpals.SAI : SvrLiquidazioneAgo.TipologiaContribuzioneEnpals.SAS;

            if (input.QuotaA != null)
            {
                output.QuotaA = GetQuotaSvrLiquidazioneAgo(input.QuotaA);
            }
            if (input.QuotaB != null)
            {
                output.QuotaB = GetQuotaSvrLiquidazioneAgo(input.QuotaB);
            }
            if (input.QuotaC != null)
            {
                output.QuotaC = GetQuotaSvrLiquidazioneAgo(input.QuotaC);
            }
            return output;
        }

        private static Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals.Quota GetQuotaSvrLiquidazioneAgo(Presenter.SvrLiquidazione.DatiContribuzioneEnpals.Quota quotaInput)
        {
            Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals.Quota quotaOutput = new Presenter.SvrLiquidazioneAgo.DatiContribuzioneEnpals.Quota();
            quotaOutput.Enpals = quotaInput.Enpals;
            quotaOutput.Estera = quotaInput.Estera;
            quotaOutput.Figurativa = quotaInput.Figurativa;
            quotaOutput.Inps = quotaInput.Inps;
            quotaOutput.Ufficio = quotaInput.Ufficio;
            quotaOutput.Volontaria = quotaInput.Volontaria;
            quotaOutput.Enpals = quotaInput.Enpals;
            return quotaOutput;
        }
        #endregion Mapping tra le classi  DatiContribuzioneEnpals di Ago e Common


        public static bool IsDomandaAUT(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOAUT || GetCategoria(categoria.Trim()) == Categoria.IOAUT || GetCategoria(categoria.Trim()) == Categoria.SOAUT)
                    return true;

            return false;
        }

        public static bool IsDomandaVOAUT(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOAUT)
                    return true;

            return false;
        }

        public static bool IsDomandaVOMIN_SOMIN(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOMIN || GetCategoria(categoria.Trim()) == Categoria.SOMIN)
                    return true;

            return false;
        }

        public static bool IsDomandaAnzianitaAnticipata(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001")
                return true;

            return false;
        }

        public static bool IsDomandaVOAUT_IOAUT(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOAUT || GetCategoria(categoria.Trim()) == Categoria.IOAUT)
                    return true;

            return false;
        }

        public static bool IsDomandaSOAUT(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.SOAUT)
                    return true;

            return false;
        }

        public static bool IsDomandaSOAUT_IOAUT(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.SOAUT || GetCategoria(categoria.Trim()) == Categoria.IOAUT)
                    return true;

            return false;
        }

        public static bool IsDomandaSPED(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOSPED || GetCategoria(categoria.Trim()) == Categoria.IOSPED || GetCategoria(categoria.Trim()) == Categoria.SOSPED)
                    return true;

            return false;
        }

        public static bool IsDomandaIndennitaUnaTantum_AGO(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0025")
                return true;

            return false;
        }

        public static bool IsDomandaVOMIN(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOMIN)
                    return true;

            return false;
        }

        public static bool IsDomandaSOMIN(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.SOMIN)
                    return true;

            return false;
        }

        public static bool IsDomandaRipristino(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if ((datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0121") ||
                            (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0321") ||
                            (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0421"))
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if ((datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0122") ||
                            (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0322") ||
                            (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0422"))
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoOrRiliquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (IsDomandaRiliquidazione(datiPensione) || IsDomandaRipristino(datiPensione))
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoOrRiliquidazioneSuperstiti(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && (datiPensione.CodeProdotto == "0421" || datiPensione.CodeProdotto == "0422"))
                return true;
            return false;
        }

        public static bool IsDomandaRiliquidazioneSuperstiti(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0422")
                return true;
            return false;
        }


        public static bool IsDomandaRipristinoSuperstiti(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0421")
                return true;
            return false;
        }
        public static bool IsDomandaRipristinoAnzianitaAnticipata(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0121" && datiPensione.CodeTipo == "0021")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoVecchiaia(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0121" && datiPensione.CodeTipo == "0022")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneVecchiaiaAnticipate(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0122")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoAssegnoInvalidita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0321" && datiPensione.CodeTipo == "0023")
                return true;

            return false;
        }

        public static bool IsDomandaRliquidazioneAssegnoInvalidita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0322" && datiPensione.CodeTipo == "0023")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoInvalidita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0321" && datiPensione.CodeTipo == "0024")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoInabilita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0321" && datiPensione.CodeTipo == "0025")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneAnzianitaAnticipata(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0051" && datiPensione.CodeProdotto == "0122" && datiPensione.CodeTipo == "0021")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(AreaTitolare.DatiPensione datiPensione, DateTime? dataPerfezionamentoRequisiti = null)
        {
            if (datiPensione == null)
                return false;

            if (dataPerfezionamentoRequisiti == null)
                dataPerfezionamentoRequisiti = datiPensione.DataPerfezionamentoRequisiti;

            if (Utility.IsDomandaRiliquidazioneAnzianitaAnticipata(datiPensione) &&
                dataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2019, 1, 1)) &&
                !Utility.DataStrettamenteSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2026, 12, 31)))
                return true;

            return false;
        }

        public static bool IsDomandaINDCOM(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim() == "INDCOM")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM(AreaTitolare.DatiPensione datiPensione)
        {
            if ((datiPensione.CodeGruppo == "0006" && datiPensione.CodeProdotto == "0051"))
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM175(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (IsDomandaINDCOM(siglaCategoria) && datiPensione.CodeTipo == "0175")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM175(string siglaCategoria, string tipo)
        {
            if (IsDomandaINDCOM(siglaCategoria) && tipo == "0175")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM156(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (IsDomandaINDCOM(siglaCategoria) && datiPensione.CodeTipo == "0156")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM129(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (IsDomandaINDCOM(siglaCategoria) && datiPensione.CodeTipo == "0129")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM125(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (IsDomandaINDCOM(siglaCategoria) && datiPensione.CodeTipo == "0125")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM124(AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            if (IsDomandaINDCOM(siglaCategoria) && datiPensione.CodeTipo == "0124")
                return true;
            return false;
        }

        public static bool IsDomandaTotalizzazione(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "VOTOT" || siglaCategoria.Trim().ToUpperInvariant() == "IOTOT" || siglaCategoria.Trim().ToUpperInvariant() == "SOTOT"))
                return true;

            return false;
        }

        public static bool IsDomandaVOTOT(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "VOTOT"))
                return true;

            return false;
        }

        public static bool IsDomandaSOTOT(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "SOTOT"))
                return true;

            return false;
        }

        public static bool IsDomandaIOTOT(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "IOTOT"))
                return true;

            return false;
        }

        public static bool IsDomandaSupplementare(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            //ENG - VOAUT 0001-0002-0192
            if ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && (datiPensione.CodeTipo == "0009" || datiPensione.CodeTipo == "0192")) ||
                            (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0013" && datiPensione.CodeTipo == "0009") ||
                            (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021" && datiPensione.CodeTipo == "0009") ||
                            (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022" && datiPensione.CodeTipo == "0009"))
                return true;

            return false;
        }

        public static bool IsDomandaUsuranti(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0140")
                return true;

            return false;
        }

        public static bool IsDomandaSOPED(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.SOSPED)
                    return true;

            return false;
        }

        public static bool IsDomandaDAI(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VDAI || GetCategoria(categoria.Trim()) == Categoria.IDAI || GetCategoria(categoria.Trim()) == Categoria.SDAI)
                    return true;

            return false;
        }

        public static bool IsDomandaVOCOOP_COOP28(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOCOOP || GetCategoria(categoria.Trim()) == Categoria.COOP28)
                    return true;

            return false;
        }

        public static bool IsDomandaVOCOOP(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                if (GetCategoria(categoria.Trim()) == Categoria.VOCOOP)
                    return true;
            }

            return false;
        }

        public static bool IsDomandaVOESO(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOESO)
                    return true;

            return false;
        }

        public static bool IsAssegnoStraordinarioRiscossioneTributiErariali(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Enum.TipoAppartenenzaRuolo.AGO)
                return false;

            if (datiPensione.CodeGruppo == "0006" && datiPensione.CodeProdotto == "0052" && datiPensione.CodeTipo == "0034")
                return true;

            return false;
        }

        public static bool IsAssegnoStraordinarioFerrovieDelloStato(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != Enum.TipoAppartenenzaRuolo.AGO)
                return false;

            if (datiPensione.CodeGruppo == "0006" && datiPensione.CodeProdotto == "0052" && datiPensione.CodeTipo == "0036")
                return true;

            return false;
        }

        public static bool IsDomandaVOESOFerrovieDelloStatoRicConFiltro(string categoria, string GP2BB05, string codiceBancaEsodati)
        {
            if (IsDomandaVOESO(categoria) &&
                !string.IsNullOrEmpty(codiceBancaEsodati) && Convert.ToInt32(codiceBancaEsodati) >= 601 && Convert.ToInt32(codiceBancaEsodati) <= 799 && GP2BB05 != "L1")
            {
                return true;
            }

            return false;
        }
        public static bool IsDomandaVOCRED_CRED27(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && (enumCategoria.Value == Categoria.VOCRED || enumCategoria.Value == Categoria.CRED27))
                    return true;
            }

            return false;
        }

        public static bool IsDomandaVOCRED(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                if (GetCategoria(categoria.Trim()) == Categoria.VOCRED)
                    return true;
            }

            return false;
        }

        public static bool IsDomandaVOCRED_CRED27_DAP(string categoria, string filtro)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && !string.IsNullOrEmpty(filtro) && !string.IsNullOrEmpty(filtro.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && (enumCategoria.Value == Categoria.VOCRED || enumCategoria.Value == Categoria.CRED27) && filtro == "DAP")
                    return true;
            }
            return false;
        }

        public static bool IsDomandaCRED27(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && enumCategoria.Value == Categoria.CRED27)
                    return true;
            }

            return false;
        }

        public static bool IsDomandaCOOP28(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim());
                if (enumCategoria.HasValue && enumCategoria.Value == Categoria.COOP28)
                    return true;
            }

            return false;
        }

        public static bool IsDomandaPescatori(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOP || GetCategoria(categoria.Trim()) == Categoria.IOP || GetCategoria(categoria.Trim()) == Categoria.SOP)
                    return true;

            return false;
        }

        public static bool IsDomandaVOP_PL_VecchiaiaAnzianita(string categoria, bool isRiaperturaDomandaOrRicostituzione, AreaTitolare.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && datiPensione != null)
                if (GetCategoria(categoria.Trim()) == Categoria.VOP && !isRiaperturaDomandaOrRicostituzione && datiPensione.CodeGruppo == "0001" &&
                    (datiPensione.CodeProdotto == "0001" || datiPensione.CodeProdotto == "0002") && datiPensione.CodeTipo == "0001")
                    return true;

            return false;
        }

        public static bool IsDomandaVOBANC_PL_VecchiaiaAnzianita(string categoria, bool isRiaperturaDomandaOrRicostituzione, AreaTitolare.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && datiPensione != null)
                if (GetCategoria(categoria.Trim()) == Categoria.VOBANC && !isRiaperturaDomandaOrRicostituzione && datiPensione.CodeGruppo == "0001" &&
                    (datiPensione.CodeProdotto == "0001" || datiPensione.CodeProdotto == "0002") && datiPensione.CodeTipo == "0001")
                    return true;

            return false;
        }

        public static bool IsDomandaPescatoriFiltroL80(AreaTitolare.DatiPensione datiPensione, string categoria)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaPescatori(categoria) && datiPensione.CodiceTipoRichiesta == "C8")
                return true;

            return false;
        }

        public static bool IsDomandaVOST(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
                if (GetCategoria(categoria.Trim()) == Categoria.VOST)
                    return true;

            return false;
        }

        public static bool IsDomandaBancari(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim() == "VOBANC" || siglaCategoria.Trim() == "IOBANC" || siglaCategoria.Trim() == "SOBANC"))
                return true;
            return false;
        }

        public static bool IsDomandaInabilitaLegge335(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0052";
        }

        public static bool IsDomandaInabilitaPrivilegioGestionePubblica(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0046";
        }

        public static bool IsDomandaInabilitaProficuoLavoro(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0047";
        }

        public static bool IsDomandaPSO(string siglaCategoria)
        {
            return !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "PSO";
        }
        public static bool IsDomandaPMO(string siglaCategoria)
        {
            return !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "PMO";
        }

        public static string GetCodiceEnteByCertificato(string certificato)
        {

            if (!string.IsNullOrEmpty(certificato))
            {
                var first3 = certificato.Substring(0, 3);
                switch (first3)
                {
                    case "091":
                    case "093":
                        return "1";
                    case "097":
                    case "099":
                        return "2";
                    case "094":
                    case "096":
                        return "3";
                }
            }
            return string.Empty;
        }

        public static bool IsTitolareExConiugeOrScioltoDallUnione(char? SiglaFamiliare)
        {
            if (SiglaFamiliare.HasValue && SiglaFamiliare == 'R')
                return true;

            return false;
        }

        public static DateTime? GetDataLimiteScadenzaIndenizzoINDCOM(string tipologia, string sesso, DateTime? dataNascitaTitolare,
            SvrLiquidazioneAgo.CtrlScadenzaIndennizzoINDCOM[] listaCtrlScadenzaIndennizzoINDCOM, AreaTitolare.DatiPensione datiPensione, string siglaCategoria)
        {
            DateTime? dataCompare = null;
            if (dataNascitaTitolare != null)
            {
                if (listaCtrlScadenzaIndennizzoINDCOM != null && listaCtrlScadenzaIndennizzoINDCOM.Length > 0 && !(IsDomandaINDCOM129(datiPensione, siglaCategoria) && datiPensione.DecorrenzaOriginaria.HasValue && !DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2010, 12, 31))))
                {
                    SvrLiquidazioneAgo.CtrlScadenzaIndennizzoINDCOM ctrlScadenzaIndennizzoINDCOM = listaCtrlScadenzaIndennizzoINDCOM
                    .FirstOrDefault(ctrl => (ctrl.Tipologia == tipologia) &&
                                            (ctrl.Sesso == "ALL" || ctrl.Sesso == sesso) &&
                                            (!ctrl.DataNascitaDal.HasValue || DataSuccessivaA(dataNascitaTitolare.Value, ctrl.DataNascitaDal.Value)) &&
                                            (!ctrl.DataNascitaAl.HasValue || DataSuccessivaA(ctrl.DataNascitaAl.Value, dataNascitaTitolare.Value)));

                    if (ctrlScadenzaIndennizzoINDCOM != null)
                    {
                        dataCompare = dataNascitaTitolare;

                        if (ctrlScadenzaIndennizzoINDCOM.PrepopolaAnni.HasValue)
                            dataCompare = dataCompare.Value.AddYears(ctrlScadenzaIndennizzoINDCOM.PrepopolaAnni.Value);
                        if (ctrlScadenzaIndennizzoINDCOM.PrepopolaMesi.HasValue)
                            dataCompare = dataCompare.Value.AddMonths(ctrlScadenzaIndennizzoINDCOM.PrepopolaMesi.Value);
                        if (ctrlScadenzaIndennizzoINDCOM.PrepopolaGiorni.HasValue)
                            dataCompare = dataCompare.Value.AddDays(ctrlScadenzaIndennizzoINDCOM.PrepopolaGiorni.Value);

                        if (dataCompare == dataNascitaTitolare)
                            dataCompare = null;
                    }
                }
                else if (IsDomandaINDCOM129(datiPensione, siglaCategoria) && datiPensione.DecorrenzaOriginaria.HasValue && !DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2010, 12, 31)))
                {
                    if (!string.IsNullOrEmpty(sesso) && sesso == "M")
                    {
                        dataCompare = new DateTime(dataNascitaTitolare.Value.AddYears(66).Year, dataNascitaTitolare.Value.AddYears(66).AddMonths(7).Month, 1);
                    }
                    else if (string.IsNullOrEmpty(sesso) && sesso == "F")
                    {
                        dataCompare = new DateTime(dataNascitaTitolare.Value.AddYears(61).Year, dataNascitaTitolare.Value.AddYears(61).AddMonths(7).Month, 1);
                    }
                }
                else if (IsDomandaINDCOM125(datiPensione, siglaCategoria))
                {
                    if (!string.IsNullOrEmpty(sesso) && sesso == "M")
                    {
                        dataCompare = new DateTime(dataNascitaTitolare.Value.AddYears(65).Year, dataNascitaTitolare.Value.AddYears(65).AddMonths(1).Month, 1);
                    }
                    else if (string.IsNullOrEmpty(sesso) && sesso == "F")
                    {
                        dataCompare = new DateTime(dataNascitaTitolare.Value.AddYears(60).Year, dataNascitaTitolare.Value.AddYears(60).AddMonths(1).Month, 1);
                    }
                }
            }

            return dataCompare;
        }

        public static bool IsRicostituzione_Reddituale(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0031" &&
                (datiPensione.CodeProdotto == "0101" || datiPensione.CodeProdotto == "0301" || datiPensione.CodeProdotto == "0401"))
                return true;
            return false;
        }

        public static bool IsRicostituzione_TrattamentoDiFamiglia(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0031" &&
                (datiPensione.CodeProdotto == "0104" || datiPensione.CodeProdotto == "0304" || datiPensione.CodeProdotto == "0404"))
                return true;
            return false;
        }

        public static bool IsRicostituzione_MotiviDocumentali(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0108" && datiPensione.CodeTipo == "0001") ||
                (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0308" && datiPensione.CodeTipo == "0001") ||
                (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0408" && datiPensione.CodeTipo == "0001"))
                return true;

            return false;
        }

        public static bool IsDomandaFPLD(string siglaCategoria)
        {
            if (siglaCategoria != null)
            {
                string siglaCategoriaNormalized = siglaCategoria.Trim().ToUpperInvariant();
                if (siglaCategoriaNormalized == "VO" || siglaCategoriaNormalized == "SO" || siglaCategoriaNormalized == "IO")
                    return true;
            }
            return false;
        }

        public static bool IsDomandaGestioneAutonomi(string siglaCategoria)
        {
            if (siglaCategoria != null)
            {
                string siglaCategoriaNormalized = siglaCategoria.Trim().ToUpperInvariant();
                if (siglaCategoriaNormalized == "VR" || siglaCategoriaNormalized == "SR" || siglaCategoriaNormalized == "IR" ||
                    siglaCategoriaNormalized == "VOART" || siglaCategoriaNormalized == "SOART" || siglaCategoriaNormalized == "IOART" ||
                    siglaCategoriaNormalized == "VOCOM" || siglaCategoriaNormalized == "SOCOM" || siglaCategoriaNormalized == "IOCOM")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una Ricostituzione per accredito periodi di maternità al di fuori del rapporto di lavoro
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>Restituisce true se il Gruppo è 0031, il Prodotto è 0117 e il Tipo 0001</returns>
        public static bool IsRicostituzione_AccreditoPeriodiMaternita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0031" && (datiPensione.CodeProdotto == "0117" || datiPensione.CodeProdotto == "0317") && datiPensione.CodeTipo == "0001")
                return true;

            return false;
        }

        public static void ExceptionHandler(Exception ex, string message)
        {
            try
            {
                throw ex;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                throw new DnaApplicationException(message, Ex);
            }
        }

        public static void CloseClient(System.ServiceModel.ICommunicationObject objWS)
        {
            try
            {
                if (objWS.State != CommunicationState.Closed &&
                   objWS.State != CommunicationState.Faulted)
                {
                    objWS.Close(); // may throw exception while closing
                }
                else
                {
                    objWS.Abort();
                }
            }
            catch (CommunicationException)
            {
                objWS.Abort();
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
        }

        public static Enum.TipoAppartenenzaRuolo? GetTipoAppartenenza(bool? indconvint, string codgestione)
        {
            if (!indconvint.HasValue || String.IsNullOrEmpty(codgestione))
                return null;

            if (indconvint.Value)
            {
                if (codgestione == "018")
                    return Enum.TipoAppartenenzaRuolo.AGO;

                return Enum.TipoAppartenenzaRuolo.CI;
            }
            else
            {
                switch (codgestione)
                {
                    case "007":
                    case "019":
                        return Enum.TipoAppartenenzaRuolo.FS;
                    default:
                        return Enum.TipoAppartenenzaRuolo.AGO;
                }
            }
        }


        /// <summary>
        /// Verifica se la domanda è una Ricostituzione per adeguamento pro quota Casse
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>Restituisce true se il Gruppo è 0031, il Prodotto è (0102,0302,0402) e il Tipo 0184</returns>
        public static bool IsDomandaRicostituzioneAdeguamentoProQuotaCasse(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.CodeGruppo == "0031" && (datiPensione.CodeProdotto == "0102" || datiPensione.CodeProdotto == "0302" || datiPensione.CodeProdotto == "0402")
                && datiPensione.CodeTipo == "0184")
                return true;

            return false;
        }

        public static bool IsDomandaSalvaguardia178_2020(AreaTitolare.DatiPensione datiPensione)
        {
            if ((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0181" &&
                datiPensione.Filtro.ToUpperInvariant() == "KOA" || datiPensione.Filtro.ToUpperInvariant() == "KPM") //filtro per automatiche e manuali
                ||
                (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0181" &&
                datiPensione.Filtro.ToUpperInvariant() == "KQA" || datiPensione.Filtro.ToUpperInvariant() == "KRM")) // filtro per automatiche e manuali
                return true;
            return false;
        }

        public static bool IsRicostituzione_MotiviContributivi(AreaTitolare.DatiPensione datiPensione)
        {
            if ((datiPensione.CodeGruppo == "0031" &&
                (datiPensione.CodeProdotto == "0107" || datiPensione.CodeProdotto == "0307" || datiPensione.CodeProdotto == "0407")) ||
                 (IsDomandaRicPensioneOrdinariaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneInabilitaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneIndirettaInabilitaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(datiPensione)))
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0120/0320/0420
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_ProvenienteDaListePensioniDaVerificare(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo == "0031" &&
                (datiPensione.CodeProdotto == "0120" || datiPensione.CodeProdotto == "0320" || datiPensione.CodeProdotto == "0420"))
                return true;
            return false;
        }

        /// <summary>
        /// Ritorna la data nel formato 01/MM/YYYY
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Ritorna la data nel formato 01/MM/YYYY</returns>
        public static DateTime FirstDayOfMonth(DateTime data)
        {
            return data.AddDays(1 - data.Day);
        }

        public static bool IsDomandaRenditaFacoltativa(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim() == "VOBIS" || siglaCategoria.Trim() == "IOBIS"))
                return true;
            return false;
        }

        public static bool IsDomandaRenditaCasalinghe(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim() == "VMP" || siglaCategoria.Trim() == "IMP"))
                return true;
            return false;
        }

        public static bool IsDomandaEccezioneMemo86(string categoria, string naturaPensione, AreaTitolare.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()))
            {
                switch (categoria.Trim().ToUpperInvariant())
                {
                    case "PS":
                    case "AS":
                    case "INVCIV":
                    case "VOBIS":
                    case "IOBIS":
                    case "VMP":
                    case "IMP":
                    case "VOSPED":
                    case "IOSPED":
                    case "SOSPED":
                    case "VOST":
                    case "INDCOM":
                    case "VOCRED":
                    case "VOCOOP":
                    case "VOESO":
                    case "CRED27":
                    case "COOP28":
                    case "VESO33":
                    case "VESO92":
                    case "VOSPETT":
                    case "IOSPETT":
                    case "SOSPETT":
                    case "VOSPORT":
                    case "IOSPORT":
                    case "SOSPORT":
                    case "VOBANC":
                    case "IOBANC":
                    case "SOBANC":
                    case "ESPA":
                        return true;
                }

                if (categoria.ToUpperInvariant().StartsWith("S") ||
                    (categoria.ToUpperInvariant().StartsWith("I") && !(datiPensione.CodeGestione == "019" || (datiPensione.CodeGestione == "007" && datiPensione.CodeFondo == "006") || (datiPensione.CodeGestione == "007" && datiPensione.CodeFondo == "014")) &&
                    !string.IsNullOrEmpty(naturaPensione) && !(naturaPensione.StartsWith("3") || naturaPensione.StartsWith("4") || naturaPensione.StartsWith(" "))))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Il metodo verifica se la domanda è una VESO33 - Gestione pubblica     
        /// </summary>
        public static bool IsDomandaVESO33_DAP(string categoria, string filtro)
        {
            if (!string.IsNullOrEmpty(categoria) && !string.IsNullOrEmpty(categoria.Trim()) && !string.IsNullOrEmpty(filtro) && !string.IsNullOrEmpty(filtro.Trim()))
            {
                Categoria? enumCategoria = GetCategoria(categoria.Trim().ToUpperInvariant());
                if (enumCategoria.HasValue && enumCategoria.Value == Categoria.VESO33 && filtro.Trim().ToUpperInvariant() == "DAP")
                    return true;
            }
            return false;
        }

        public enum FormatoData
        {
            GGmmAAAA,
            AAAAmmGG,
            AAAAmm
        };

        public static DateTime? DataFromString(string data, FormatoData formato)
        {
            try
            {
                data = data.Replace(".", "");
                data = data.Replace("/", "");
                data = data.Replace("-", "");

                switch (formato)
                {
                    case FormatoData.AAAAmmGG:
                        return new DateTime?(new DateTime(Int32.Parse(data.Substring(0, 4)), Int32.Parse(data.Substring(4, 2)), Int32.Parse(data.Substring(6, 2))));
                    case FormatoData.GGmmAAAA:
                        return new DateTime?(new DateTime(Int32.Parse(data.Substring(4, 4)), Int32.Parse(data.Substring(2, 2)), Int32.Parse(data.Substring(0, 2))));
                    case FormatoData.AAAAmm:
                        return new DateTime?(new DateTime(Int32.Parse(data.Substring(0, 4)), Int32.Parse(data.Substring(4, 2)), 1));
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Il metodo verifica se la domanda è una spacchettata 024 - PT
        /// </summary>
        public static bool IsDomandaSpacchettamento024(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, string siglaCategoria, DateTime? dataAcquisizione)
        {
            string controlloDinamicoAbilitazioneCalcoloReingSpacchettatePT = string.Empty;
            Presenter.PresenterControlliDinamici presenterSedi = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esitoCalcoloReingSpacchettatePT = presenterSedi.GetControlloDinamicoByNomeControllo("AbilitazioneCalcoloReingSediSpacchettatePT", out controlloDinamicoAbilitazioneCalcoloReingSpacchettatePT);
            string sedeLavorazione = GetSedeOperatore().ToString().PadLeft(4, '0');

            if (!tipoFondo.HasValue)
                return false;

            if (esitoCalcoloReingSpacchettatePT != null && esitoCalcoloReingSpacchettatePT.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                      && (String.IsNullOrEmpty(controlloDinamicoAbilitazioneCalcoloReingSpacchettatePT) || controlloDinamicoAbilitazioneCalcoloReingSpacchettatePT.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == sedeLavorazione)))
            {

                string controlloDinamico = string.Empty;
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("DataControlloSpacchettate024", out controlloDinamico);

                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    //Una domanda è classificata come spacchettata024 se la Data Acquisizione è maggiore/uguale alla data del controllo dinamico, altrimenti deve rientrare nel flusso classico
                    DateTime? controlloDinamicoDataAcquisizione = Utility.DataFromString(controlloDinamico, FormatoData.AAAAmmGG);
                    if (tipoFondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && siglaCategoria.ToUpperInvariant().StartsWith("S")
                        && controlloDinamicoDataAcquisizione.HasValue && DataSuccessivaA(dataAcquisizione.GetValueOrDefault(), controlloDinamicoDataAcquisizione.GetValueOrDefault()))
                        return true;
                }
            }

            return false;
        }

        public static bool IsDomandaBancRicAnte1991(string siglaCategoria, AreaTitolare.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa)
        {
            if (IsRicostituzione(datiPensione.CodeGruppo))
            {
                DateTime date = new DateTime(1990, 12, 31);
                switch (siglaCategoria.Trim())
                {
                    case "SOBANC":
                        if (datiPensione.DecorrenzaOriginaria == null)
                            return false;

                        if (IsDomandaPensioneReversibilitaOrRicostituzione(siglaCategoria, datiPensione, areaDanteCausa))
                        {
                            if (areaDanteCausa != null && areaDanteCausa.DatiPensioneDiretta != null && areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione != null)
                                return !(DataSuccessivaA(areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.Value, date));
                            else
                                return false;
                        }
                        else
                            return !(DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, date));

                    case "VOBANC":
                    case "IOBANC":
                        return !(DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, date)); ;

                    default:
                        return false;

                }
            }
            return false;
        }

        public static bool IsDomandaBanc_91_95(string siglaCategoria, AreaTitolare.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa)
        {
            DateTime? date = null;
            DateTime ante = new DateTime(1995, 12, 31);
            DateTime post = new DateTime(1991, 01, 01);

            if (IsDomandaBancari(siglaCategoria))
            {
                if (IsDomandaReversibilita(datiPensione) && areaDanteCausa != null && areaDanteCausa.DatiPensioneDiretta != null && areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione != null)
                    date = areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.Value;
                else if (datiPensione != null && datiPensione.DecorrenzaOriginaria != null)
                    date = datiPensione.DecorrenzaOriginaria.Value;
            }

            if (date != null && Utility.DataSuccessivaA(date.Value, post) && Utility.DataSuccessivaA(ante, date.Value))
            {
                return true;
            }

            return false;
        }

        public static bool IsDomandaPensioneReversibilitaOrRicostituzione(string siglaCategoria, AreaTitolare.DatiPensione datiPensione, AreaDanteCausa areaDanteCausa)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaReversibilita(datiPensione) ||
                (Utility.IsRicostituzione(datiPensione.CodeGruppo) && !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.StartsWith("S") &&
                 areaDanteCausa != null && (areaDanteCausa.AnagraficaDC.ProvenienzaPensione == 1 || areaDanteCausa.AnagraficaDC.ProvenienzaPensione == 2)
                 ))
                return true;

            return false;
        }

        public static bool IsRicostituzione(string gruppo)
        {
            if (gruppo == "0031")
                return true;
            return false;
        }

        public static bool IsRicostituzione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.CodeGruppo != null && datiPensione.CodeGruppo == "0031")
                return true;
            else return false;

        }

        public static bool IsDomandaPL(AreaTitolare.DatiPensione datiPensione, bool isRiapertura)
        {
            return !(IsRicostituzione(datiPensione) || isRiapertura || IsDomandaRipristinoOrRiliquidazione(datiPensione));
        }

        public static bool isPensioneOvunqueAttiva(UtilityTipoAppartenenza tipoAppartenenza)
        {
            string controlloDinamicoPensioniOvunque = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esitoControlloDinamicoPensioniOvunque = presenter.GetControlloDinamicoByNomeControllo("PensioniOvunque", out controlloDinamicoPensioniOvunque);

            if (esitoControlloDinamicoPensioniOvunque != null && esitoControlloDinamicoPensioniOvunque.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                && !String.IsNullOrEmpty(controlloDinamicoPensioniOvunque) && !String.IsNullOrEmpty(controlloDinamicoPensioniOvunque.Trim()))
            {
                DateTime? dataSistema = null;
                Presenter.PresenterControlliDinamici presenterData = new PresenterControlliDinamici();
                presenterData.GetDataSistema(tipoAppartenenza, out dataSistema);
                DateTime? dataInizioPensioniOvunque = Utility.DataFromString(controlloDinamicoPensioniOvunque.Trim(), Utility.FormatoData.AAAAmmGG);
                if (dataSistema.HasValue && Utility.DataSuccessivaA(dataSistema.Value, dataInizioPensioniOvunque.GetValueOrDefault()))
                    return true;
            }

            return false;
        }

        public static bool IsPensioneInabilitaProficuoLavoroCumulo(string categoria, AreaTitolare.DatiPensione datiPensione)
        {
            if (categoria.Trim().ToUpperInvariant() == "IOCUM")
            {
                if ((datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0012" && datiPensione.CodeTipo == "0047")
                    || (datiPensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicInabilitaOrdinariaInCumulo.GetHashCode())
                    )
                    return true;
            }
            return false;
        }


        //ENG - Spacchettate SOPGI
        public static bool IsDomandaSpacchettamentoSOPGIPost072022(string categoria, AreaTitolare.DatiPensione datiPensione, AreaDanteCausa danteCausa)
        {
            if (datiPensione == null || danteCausa == null || danteCausa.AnagraficaDC == null || String.IsNullOrEmpty(categoria) || !danteCausa.AnagraficaDC.DataMorte.HasValue)
                return false;

            if (categoria.Trim().ToUpperInvariant() == "SOPGI" && ((Utility.DataSuccessivaA(danteCausa.AnagraficaDC.DataMorte.GetValueOrDefault(), new DateTime(2022, 7, 1))) ||
                (IsRicostituzione(datiPensione.CodeGruppo) && !string.IsNullOrEmpty(datiPensione.GP1AV91B) && datiPensione.GP1AV91B == "2")))
                return true;

            return false;
        }

        public static bool IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(string siglaCategoria, string codeGruppo, DateTime? dataAssunzioneCarico, bool isRiaperturaDomanda)
        {
            if ((IsRicostituzione(codeGruppo) || isRiaperturaDomanda) && (IsDomandaPSO(siglaCategoria) || IsDomandaPMO(siglaCategoria)))
                return true;

            if (dataAssunzioneCarico.HasValue && IsDomandaDAIAnte2003(dataAssunzioneCarico.Value, siglaCategoria))
                return true;

            return false;
        }

        public static bool IsDomandaDAIAnte2003(DateTime dataAssunzioneCarico, string categoria)
        {
            if ((categoria.Trim().ToUpperInvariant() == "VDAI" || categoria.Trim().ToUpperInvariant() == "SDAI" || categoria.Trim().ToUpperInvariant() == "IDAI") &&
                (dataAssunzioneCarico.Month < 12 && dataAssunzioneCarico.Year <= 2003))
                return true;
            else

                return false;
        }

        //ENG - MEMO 50/2023
        public static bool IsRicostituzione_PerVariazioneDatiSupplemento(AreaTitolare.DatiPensione datiPensione)
        {
            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Enum.TipoAppartenenzaRuolo.CI)
                return false;

            if (datiPensione.CodeGruppo == "0031" &&
                (datiPensione.CodeProdotto == "0107" || datiPensione.CodeProdotto == "0307" || datiPensione.CodeProdotto == "0407") && datiPensione.CodeTipo == "0193")
                return true;

            return false;
        }

        public static bool IsRicostituzione_Supplemento(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.CodeGruppo == "0031" && (datiPensione.CodeProdotto == "0102" || datiPensione.CodeProdotto == "0302" || datiPensione.CodeProdotto == "0402"))
                return true;

            return false;
        }

        public static bool IsDomandaAdeguamentoRinnoviContrattualiGDP(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0107" && datiPensione.CodeTipo == "0198")
                return true;

            return false;
        }

        //ENG - RIC CONCESSIONE ALTRA PENSIONE
        public static bool IsRicostituzioneConcessioneAltraPensione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione != null && Utility.IsRicostituzione(datiPensione) && (datiPensione.CodeProdotto == "0109" || datiPensione.CodeProdotto == "0309" || datiPensione.CodeProdotto == "0409")
                && datiPensione.CodeTipo == "0130")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una Pensione di Reversibilità (Gruppo = 0003 Prodotto = 0021)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>True se il gruppo è 0003 e il prodotto è 0021</returns>
        public static bool IsDomandaReversibilitaOrRicostituzione(AreaTitolare.DatiPensione datiPensione, AreaDanteCausa danteCausa, string categoria)
        {
            return IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, categoria, null, null);
        }

        //ENG - RIC REVERSIBILITA 024: implementazione flusso anche per riconoscere le reversibilità "vecchie" 
        public static bool IsDomandaReversibilitaOrRicostituzione(AreaTitolare.DatiPensione datiPensione, AreaDanteCausa danteCausa, string categoria, char? tipoReversibilita, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if ((datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021") ||
                (Utility.IsRicostituzione(datiPensione.CodeGruppo) && !string.IsNullOrEmpty(categoria) && categoria.StartsWith("S") && danteCausa != null && danteCausa.DatiPensioneDiretta != null &&
                    !string.IsNullOrEmpty(danteCausa.DatiPensioneDiretta.SiglaCategoria) && !string.IsNullOrEmpty(danteCausa.DatiPensioneDiretta.Sede) && danteCausa.DatiPensioneDiretta.Certificato.HasValue))
                return true;

            if (!String.IsNullOrEmpty(categoria) && categoria.StartsWith("S") && Utility.IsRicostituzione(datiPensione)
                && (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (danteCausa != null && danteCausa.DatiPensioneDiretta != null)
                {
                    if (string.IsNullOrEmpty(danteCausa.DatiPensioneDiretta.SiglaCategoria) || string.IsNullOrEmpty(danteCausa.DatiPensioneDiretta.Sede)
                        || danteCausa.DatiPensioneDiretta.Sede.PadLeft(4, '0') == "0000" || !danteCausa.DatiPensioneDiretta.Certificato.HasValue
                        || danteCausa.DatiPensioneDiretta.Certificato.Value == 0)
                    {
                        if (tipoReversibilita.HasValue && tipoReversibilita.Value.ToString().ToUpperInvariant() == "R")
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool AbilitaFlussoSeiScatti()
        {
            return (ConfigurationManager.AppSettings["AbilitaFlussoSeiScatti"] != null &&
                ConfigurationManager.AppSettings["AbilitaFlussoSeiScatti"] == "SI");
        }

        public static bool Sblocco_supplementi_ante96()
        {
            return (ConfigurationManager.AppSettings["Sblocco_supplementi_ante96"] != null &&
                ConfigurationManager.AppSettings["Sblocco_supplementi_ante96"] == "SI");

        }

        public static bool IsDomandaBeneficioTerrorismoLegge206_2004(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0105" && datiPensione.CodeTipo == "0112")
                return true;

            return false;
        }

        public static bool isDomandaGiornalistiDipendentiConSistemaPrivato(AreaTitolare.DatiPensione datiPensione)
        {
            if ((datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021" && datiPensione.CodeTipo == "0001" && (datiPensione.GP1AV91B == "0")) || (Utility.IsRicostituzione(datiPensione.CodeGruppo) && datiPensione.GP1AV91B == "3"))
                return true;
            return false;
        }

        public static bool IsDomandaSperimentaleDonnaOrRicostituzione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.CodeGruppo == "0001" && (datiPensione.CodeProdotto == "0001" || datiPensione.CodeProdotto == "0002") && datiPensione.CodeTipo == "0050") ||
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O"))
                return true;

            return false;
        }

        public static bool IsDomandaCalcoloContributivoSperimentaleLavoratriciOrRicostrituzione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.CodeGruppo == "0001" && (datiPensione.CodeProdotto == "0001" || datiPensione.CodeProdotto == "0002") && datiPensione.CodeTipo == "0176") ||
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O"))
                return true;

            return false;
        }

        public static bool IsDomandaOpzioneDonnaOrRicostituzione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.CodeGruppo == "0001" && (datiPensione.CodeProdotto == "0001" || datiPensione.CodeProdotto == "0002") && datiPensione.CodeTipo == "0190") ||
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "J"))
                return true;

            return false;
        }

        public static bool IsDomandaRicPensioneOrdinariaCambioPrivilegio(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0127" && datiPensione.CodeTipo == "0001";
        }

        public static bool IsDomandaRicPensioneInabilitaCambioPrivilegio(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0327" && datiPensione.CodeTipo == "0019";
        }

        public static bool IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0427" && datiPensione.CodeTipo == "0001";
        }

        public static bool IsDomandaRicPensioneIndirettaInabilitaCambioPrivilegio(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0427" && datiPensione.CodeTipo == "0019";
        }

        public static bool isDomandaRicperRiliquidazioneEtaPensionabile(AreaTitolare.DatiPensione datiPensione)
        {
            return datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto == "0114" && datiPensione.CodeTipo == "0001";
        }

        //ENG - Assegno invalidità
        public static bool IsAssegnoInvalidita(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto == "0011")
                return true;

            return false;
        }

        //ENG - Spacchettate SO
        public static bool IsDomandaSpacchettamentoSO(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza == Enum.TipoAppartenenzaRuolo.AGO)
            {
                if (!String.IsNullOrEmpty(domanda.Categoria) && domanda.Categoria.Trim().ToUpperInvariant() == "SO")
                {
                    if (!Utility.IsRicostituzione(datiPensione) && !domanda.IsDomandaRiapertura)
                    {
                        Presenter.PresenterControlliDinamici presenterControlloDinamicoSpacchettate = new PresenterControlliDinamici();
                        string dataControlloSO = string.Empty;
                        Presenter.SvrLiquidazione.AreaEsito esito = null;

                        if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSO", out dataControlloSO);
                        else if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSO", out dataControlloSO);

                        if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        {
                            if (!String.IsNullOrEmpty(dataControlloSO) && !String.IsNullOrEmpty(dataControlloSO.Trim()))
                            {
                                DateTime? dataControlloConvertitaSO = Utility.DataFromString(dataControlloSO, FormatoData.AAAAmmGG);
                                //Una domanda deve rientrare nel flusso delle spacchettate SO se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (dataControlloConvertitaSO.HasValue && Utility.DataSuccessivaA(domanda.DataAcquisizione.GetValueOrDefault(), dataControlloConvertitaSO.GetValueOrDefault()))
                                    return true;
                            }
                        }
                    }
                    else
                    {
                        if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                            return true;
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SOCOM
        public static bool IsDomandaSpacchettamentoSOCOM(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza == Enum.TipoAppartenenzaRuolo.AGO)
            {
                if (!String.IsNullOrEmpty(domanda.Categoria) && domanda.Categoria.Trim().ToUpperInvariant() == "SOCOM")
                {
                    if (!Utility.IsRicostituzione(datiPensione) && !domanda.IsDomandaRiapertura)
                    {
                        Presenter.PresenterControlliDinamici presenterControlloDinamicoSpacchettate = new PresenterControlliDinamici();
                        string dataControlloSOCOM = string.Empty;
                        Presenter.SvrLiquidazione.AreaEsito esito = null;

                        if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSOCOM", out dataControlloSOCOM);
                        else if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSOCOM", out dataControlloSOCOM);

                        if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        {
                            if (!String.IsNullOrEmpty(dataControlloSOCOM) && !String.IsNullOrEmpty(dataControlloSOCOM.Trim()))
                            {
                                DateTime? dataControlloConvertitaSOCOM = Utility.DataFromString(dataControlloSOCOM, FormatoData.AAAAmmGG);
                                //Una domanda deve rientrare nel flusso delle spacchettate SOCOM se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (dataControlloConvertitaSOCOM.HasValue && Utility.DataSuccessivaA(domanda.DataAcquisizione.GetValueOrDefault(), dataControlloConvertitaSOCOM.GetValueOrDefault()))
                                    return true;
                            }
                        }
                    }
                    else
                    {
                        if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                            return true;
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SR
        public static bool IsDomandaSpacchettamentoSR(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza == Enum.TipoAppartenenzaRuolo.AGO)
            {
                if (!String.IsNullOrEmpty(domanda.Categoria) && domanda.Categoria.Trim().ToUpperInvariant() == "SR")
                {
                    if (!Utility.IsRicostituzione(datiPensione) && !domanda.IsDomandaRiapertura)
                    {
                        Presenter.PresenterControlliDinamici presenterControlloDinamicoSpacchettate = new PresenterControlliDinamici();
                        string dataControlloSR = string.Empty;
                        Presenter.SvrLiquidazione.AreaEsito esito = null;

                        if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSR", out dataControlloSR);
                        else if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSR", out dataControlloSR);

                        if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        {
                            if (!String.IsNullOrEmpty(dataControlloSR) && !String.IsNullOrEmpty(dataControlloSR.Trim()))
                            {
                                DateTime? dataControlloConvertitaSR = Utility.DataFromString(dataControlloSR, FormatoData.AAAAmmGG);
                                //Una domanda deve rientrare nel flusso delle spacchettate SR se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (dataControlloConvertitaSR.HasValue && Utility.DataSuccessivaA(domanda.DataAcquisizione.GetValueOrDefault(), dataControlloConvertitaSR.GetValueOrDefault()))
                                    return true;
                            }
                        }
                    }
                    else 
                    {
                        if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                            return true;
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SOART
        public static bool IsDomandaSpacchettamentoSOART(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza == Enum.TipoAppartenenzaRuolo.AGO)
            {
                if (!String.IsNullOrEmpty(domanda.Categoria) && domanda.Categoria.Trim().ToUpperInvariant() == "SOART")
                {
                    if (!Utility.IsRicostituzione(datiPensione) && !domanda.IsDomandaRiapertura)
                    {
                        Presenter.PresenterControlliDinamici presenterControlloDinamicoSpacchettate = new PresenterControlliDinamici();
                        string dataControlloSOART = string.Empty;
                        Presenter.SvrLiquidazione.AreaEsito esito = null;

                        if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0021")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSOART", out dataControlloSOART);
                        else if (datiPensione.CodeGruppo == "0003" && datiPensione.CodeProdotto == "0022")
                            esito = presenterControlloDinamicoSpacchettate.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSOART", out dataControlloSOART);

                        if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        {
                            if (!String.IsNullOrEmpty(dataControlloSOART) && !String.IsNullOrEmpty(dataControlloSOART.Trim()))
                            {
                                DateTime? dataControlloConvertitaSOART = Utility.DataFromString(dataControlloSOART, FormatoData.AAAAmmGG);
                                //Una domanda deve rientrare nel flusso delle spacchettate SOART se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (dataControlloConvertitaSOART.HasValue && Utility.DataSuccessivaA(domanda.DataAcquisizione.GetValueOrDefault(), dataControlloConvertitaSOART.GetValueOrDefault()))
                                    return true;
                            }
                        }
                    }
                    else 
                    {
                        if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                            return true;                    
                    }
                }
            }

            return false;
        }

        // Memo 79 2025 Check Memo
        public static bool IsDomandaOrganizzazioniInternazionali(AreaTitolare.DatiPensione datiPensione)
        {
            bool retVal = false;
            if (IsDomandaOrganizzazioniInternazionali_Vecchiaia_Invialidita(datiPensione) ||
                IsDomandaOrganizzazioniInternazionali_Superstiti(datiPensione) ||
                IsDomandaOrganizzazioniInternazionali_Anticipate(datiPensione))
                retVal = true;

            return retVal;
        }

        // Memo 79 2025 Check Domande Organizzazioni Internazionali Vecchiaia ed Invialidità
        public static bool IsDomandaOrganizzazioniInternazionali_Vecchiaia_Invialidita(AreaTitolare.DatiPensione datiPensione)
        {
            bool retVal = false;
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.AGO || tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.FS)
            {
                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Vecchiaia_Invialidita(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo))
                    || datiPensione.IdTipoPLPerRIC == (byte)Utility.TipoPLPerRIC.RicOIVecchiaiaInvaliditaFiltroC9A)
                    retVal = true;
            }

            return retVal;
        }

        // Memo 79 2025 Check Vecchiaia ed Invialidità
        public static bool IsDomanda_Vecchiaia_Invialidita(string Gruppo, string Prodotto, string Tipo)
        {
            bool retVal = false;
            if ((Gruppo == "0001" && Prodotto == "0002" && Tipo == "0017") ||
                (Gruppo == "0001" && Prodotto == "0002" && Tipo == "0030") ||
                (Gruppo == "0001" && Prodotto == "0002" && Tipo == "0001") ||
                (Gruppo == "0002" && Prodotto == "0012" && Tipo == "0001") ||
                (Gruppo == "0002" && Prodotto == "0012" && Tipo == "0047") ||
                (Gruppo == "0002" && Prodotto == "0012" && Tipo == "0052"))
                retVal = true;

            return retVal;

        }

        // Memo 79 2025 Check Domande Organizzazioni Internazionali Superstiti
        public static bool IsDomandaOrganizzazioniInternazionali_Superstiti(AreaTitolare.DatiPensione datiPensione)
        {
            bool retVal = false;
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.AGO || tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.FS)
            {

                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Superstiti(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo))
                    || datiPensione.IdTipoPLPerRIC == (byte)Utility.TipoPLPerRIC.RicOISuperstitiFiltroC9A)
                    retVal = true;
            }

            return retVal;
        }

        // Memo 79 2025 Check Superstiti
        public static bool IsDomanda_Superstiti(string Gruppo, string Prodotto, string Tipo)
        {
            bool retVal = false;
            if (Gruppo == "0003" && Prodotto == "0022" && Tipo == "0001")
                retVal = true;

            return retVal;

        }

        // Memo 79 2025 Check Domande di Organizzazioni Internazionali anticipate
        public static bool IsDomandaOrganizzazioniInternazionali_Anticipate(AreaTitolare.DatiPensione datiPensione)
        {
            bool retVal = false;
            Enum.TipoAppartenenzaRuolo? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.AGO || tipoAppartenenza.GetValueOrDefault() == Enum.TipoAppartenenzaRuolo.FS)
            {
                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Anticipate(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo))
                    || datiPensione.IdTipoPLPerRIC == (byte)Utility.TipoPLPerRIC.RicOIAnticipateFiltroC9A)
                    retVal = true;
            }

            return retVal;
        }

        // Memo 79 2025 Check Domande di pensione anticipate
        public static bool IsDomanda_Anticipate(string Gruppo, string Prodotto, string Tipo)
        {

            bool retVal = false;
            if ((Gruppo == "0001" && Prodotto == "0001" && Tipo == "0001") ||
                (Gruppo == "0001" && Prodotto == "0001" && Tipo == "0017") ||
                (Gruppo == "0001" && Prodotto == "0001" && Tipo == "0030"))
                retVal = true;

            return retVal;
        }

    }
}
