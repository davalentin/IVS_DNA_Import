using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class Utility
    {
        private static readonly Dictionary<string, Dictionary<string, PropertyInfo>> _cache = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        private static readonly object _cacheLock = new object();

        public static void ValorizzaOggettiNew(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();
            string cacheKey = sourceType.FullName + "->" + destinationType.FullName;

            Dictionary<string, PropertyInfo> propertyMap;

            // Usa cache senza lock e la aggiorna solo se necessario
            if (!_cache.TryGetValue(cacheKey, out propertyMap))
            {
                lock (_cacheLock)
                {
                    if (!_cache.TryGetValue(cacheKey, out propertyMap))
                    {
                        propertyMap = CreatePropertyMap(sourceType, destinationType);
                        _cache[cacheKey] = propertyMap;
                    }
                }
            }

            foreach (var kvp in propertyMap)
            {
                object sourceValue = kvp.Value.GetValue(source, null);
                PropertyInfo destinationProperty = destinationType.GetProperty(kvp.Key);

                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    destinationProperty.SetValue(destination, sourceValue, null);
                }
            }
        }

        private static Dictionary<string, PropertyInfo> CreatePropertyMap(Type sourceType, Type destinationType)
        {
            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();
            var map = new Dictionary<string, PropertyInfo>();

            foreach (var destProp in destinationProperties)
            {
                foreach (var sourceProp in sourceProperties)
                {
                    if (sourceProp.Name == destProp.Name && sourceProp.PropertyType == destProp.PropertyType)
                    {
                        map[destProp.Name] = sourceProp;
                        break;
                    }
                }
            }

            return map;
        }

        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cache.Clear();
            }
        }

        public static void ValorizzaOggetti(object source, object destination)
        {
            if (source == null || destination == null)
                return;
            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            Type destinationType = destination.GetType();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties)
                foreach (PropertyInfo destinationProperty in destinationProperties)
                    if (sourceProperty.Name == destinationProperty.Name)
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
        }

        public static void ValorizzaOggettiMaster(object source, object destination)
        {
            if (source == null || destination == null)
                return;
            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            Type destinationType = destination.GetType();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties)
                foreach (PropertyInfo destinationProperty in destinationProperties)
                    if (sourceProperty.Name == destinationProperty.Name)
                    {
                        //tale condizione permette di valorizzare la destinazione(oggetto master) solo se
                        //la proprietà in esame è nulla e la rispettiva proprietà del sorgente è non nulla
                        //in tal modo se il master ha proprietà valorizzate, queste non verranno sovrascritte
                        if (destinationProperty.GetValue(destination, null) == null && sourceProperty.GetValue(source, null) != null)
                            destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                    }
        }

        public static bool ConfrontaOggetti(object source, object destination)
        {
            if (source == null && destination == null)
                return true;
            if (source == null || destination == null)
                return false;
            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            Type destinationType = destination.GetType();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties)
                foreach (PropertyInfo destinationProperty in destinationProperties)
                    if (sourceProperty.Name == destinationProperty.Name)
                    {
                        object sourceValue = sourceProperty.GetValue(source, null);
                        object destinationValue = destinationProperty.GetValue(destination, null);
                        if (!(sourceValue == null && destinationValue == null) && (sourceValue == null || destinationValue == null ||
                            sourceProperty.GetValue(source, null).ToString() != destinationProperty.GetValue(destination, null).ToString()))
                            return false;

                        break;
                    }
            return true;
        }

        public static void ValorizzaOggettiBis(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            Type destinationType = destination.GetType();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties)
                foreach (PropertyInfo destinationProperty in destinationProperties)
                    if (sourceProperty.Name == destinationProperty.Name)
                    {
                        try
                        {
                            if (destinationProperty.PropertyType == typeof(Double) && sourceProperty.PropertyType == typeof(Decimal))
                            {
                                destinationProperty.SetValue(destination, Convert.ToDouble(sourceProperty.GetValue(source, null)), null);
                            }
                            else
                            {
                                destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
        }

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

        public static DateTime? DataFromInt(int anno, int mese, int giorno)
        {
            try
            {
                return new DateTime?(new DateTime(anno, mese, giorno));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int? StringToNullableInt(string value)
        {
            try
            {
                int output = 0;
                if (Int32.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Int64? StringToNullableInt64(string value)
        {
            try
            {
                Int64 output = 0;
                if (Int64.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static long StringToLong(string value)
        {
            long output;

            if (Int64.TryParse(value, out output) == false)
            {
                output = 0;
            }

            return output;
        }

        public static short? StringToNullableShort(string value)
        {
            try
            {
                short output = 0;
                if (short.TryParse(value, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static char? StringToNullableChar(string value)
        {
            try
            {
                if (value != null && value.Length >= 1)
                {
                    //Gestione LOW VALUE \0
                    if (((char?)((value.ToCharArray(0, 1)))[0]).Value != '\0')
                        return (char?)((value.ToCharArray(0, 1)))[0];
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte? StringToNullableByte(string value)
        {
            try
            {
                return byte.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool? StringToNullableBool(string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static decimal? StringToNullableDecimal(string value)
        {
            try
            {
                return decimal.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static decimal? StringToNullableDecimalPoint(string value)
        {
            try
            {
                decimal output = 0M;
                if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out output))
                    return output;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static short? ShortToNullableShort(short value)
        {
            //per value = 0 il valore ritornato è null
            if (value == 0)
                return (short?)null;
            else
                return value;
        }

        public static int? IntToNullableInt(int value)
        {
            //per value = 0 il valore ritornato è null
            if (value == 0)
                return (int?)null;
            else
                return value;
        }

        public static decimal? DecimalToNullableDecimal(decimal value)
        {
            //per value = 0 il valore ritornato è null
            if (value == 0)
                return (decimal?)null;
            else
                return value;
        }

        public static long? LongToNullableLong(long value)
        {
            //per value = 0 il valore ritornato è null
            if (value == 0)
                return (long?)null;
            else
                return value;
        }

        public static string BetweenStrings(string text, string start, string end)
        {
            if (text != null && text.Length > 0)
            {
                int p1 = text.IndexOf(start) + start.Length;
                int p2 = text.IndexOf(end, p1);

                if (end == "" || (p2 - p1 <= 0)) return (text.Substring(p1));
                else return text.Substring(p1, p2 - p1);
            }
            else return string.Empty;

        }

        public static bool? ControllaDataDecorrenzaInferiore(GestionePensione.DatiPensione datiPensione, bool isRevPL_Ric, DateTime? data, out DateTime? dataValidita)
        {
            string nomeControllo = "DataValidita";
            dataValidita = null;

            if ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) &&
                !isRevPL_Ric && !Utility.IsRicostituzione(datiPensione.Gruppo))
                nomeControllo += "Felpe";
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            nomeControllo += tipoAppartenenza.GetValueOrDefault().ToString();
            if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
            {
                string fondo = string.Empty;
                Utility.GetFondoBySiglaCategoria(datiPensione.SiglaCategoria, out fondo);
                if (!string.IsNullOrEmpty(fondo))
                    nomeControllo += "_" + fondo;
            }

            if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                nomeControllo += "_DAI";
            else if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                nomeControllo += "_ENPALS";

            if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
            {
                if (isRevPL_Ric)
                    nomeControllo += "-REV";
                else if (Utility.IsRicostituzione(datiPensione.Gruppo))
                    nomeControllo += "-RIC";
            }

            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;

            //aggiunta validità per sede
            BLCommon.GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SediValiditaAnte96", out sediDaControllare);
            BLCommon.GestioneControlliDinamici.ControlloDinamico sediDaControllare_2 = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SediValiditaAnte96_2", out sediDaControllare_2);
            BLCommon.GestioneControlliDinamici.ControlloDinamico sediDaControllare_3 = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SediValiditaAnte96_3", out sediDaControllare_3);
            BLCommon.GestioneControlliDinamici.ControlloDinamico sediDaControllare_4 = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SediValiditaAnte96_4", out sediDaControllare_4);
            if (sediDaControllare != null && sediDaControllare.ValoreControllo != null)
            {
                sediDaControllare.ValoreControllo = sediDaControllare.ValoreControllo + (sediDaControllare_2 != null && sediDaControllare_2.ValoreControllo != null ? ";" + sediDaControllare_2.ValoreControllo : "")
                    + (sediDaControllare_3 != null && sediDaControllare_3.ValoreControllo != null ? ";" + sediDaControllare_3.ValoreControllo : "")
                    + (sediDaControllare_4 != null && sediDaControllare_4.ValoreControllo != null ? ";" + sediDaControllare_4.ValoreControllo : "");
                if ((sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                         sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0')))))
                {
                    List<GestioneSediMatricola.DecSediMatricola> ctrlSediMatricola = null;
                    BLCommon.GestioneSediMatricola.GetDecodificaSediMatricole(Utility.GetCodiceSedeLavorazione(datiPensione, IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0'), out ctrlSediMatricola);
                    if ((ctrlSediMatricola != null && ctrlSediMatricola.Count > 0 && ctrlSediMatricola.Find(x => x.Matricola == datiPensione.MatricolaUtenteAcquisizione) != null) || (ctrlSediMatricola == null))
                    {
                        BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo + "Sedi", out controlloDinamico);
                        if (controlloDinamico != null && data != null)
                        {
                            dataValidita = Utility.DataFromString(controlloDinamico.ValoreControllo, FormatoData.AAAAmmGG);
                            if (Utility.DataSuccessivaA(data.Value.Date, dataValidita.Value.Date))
                                return true;
                            else
                                return false;
                        }

                    }
                }

            }

            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo, out controlloDinamico);
            if (controlloDinamico == null && tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS && nomeControllo.Contains('-'))
            {
                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo.Remove(nomeControllo.IndexOf('-')), out controlloDinamico);
            }
            if (controlloDinamico == null && tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
            {
                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo.Remove(nomeControllo.IndexOf('_')), out controlloDinamico);
            }

            if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO && Utility.IsDomandaBancari(datiPensione.SiglaCategoria))
                return true;

            if (controlloDinamico != null && data != null)
            {
                dataValidita = Utility.DataFromString(controlloDinamico.ValoreControllo, FormatoData.AAAAmmGG);
                if (Utility.DataSuccessivaA(data.Value.Date, dataValidita.Value.Date))
                    return true;
                else
                    return false;
            }
            else
                return null;
        }

        public static bool? ControllaDataDecorrenzaSuperiore(DateTime? data, Utility.TipoAppartenenza? tipoAppartenenza, out DateTime? dataValiditaSuperiore)
        {
            dataValiditaSuperiore = null;
            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataValiditaSuperiore" + tipoAppartenenza.GetValueOrDefault().ToString(), out controlloDinamico);
            if (controlloDinamico != null && data != null)
            {
                dataValiditaSuperiore = Utility.DataFromString(controlloDinamico.ValoreControllo, FormatoData.AAAAmmGG);
                int annoCompetenza;
                GestioneControlliDinamici.GetAnnoCompetenza(tipoAppartenenza, out annoCompetenza);
                if (dataValiditaSuperiore.HasValue)
                {
                    if (annoCompetenza != 0 && annoCompetenza != dataValiditaSuperiore.Value.Year)
                    {
                        controlloDinamico.ValoreControllo = annoCompetenza.ToString() + controlloDinamico.ValoreControllo.Substring(4);
                        GestioneControlliDinamici.SalvaControlloDinamico(controlloDinamico);
                        dataValiditaSuperiore = Utility.DataFromString(controlloDinamico.ValoreControllo, FormatoData.AAAAmmGG);
                    }
                }
                if (DateTime.Compare(data.Value.Date,
                    BLCommon.Utility.DataFromString(controlloDinamico.ValoreControllo, BLCommon.Utility.FormatoData.AAAAmmGG).Value.Date) <= 0)
                    return true;
                else
                    return false;
            }
            else
                return null;
        }

        public static TipoAppartenenza? GetTipoAppartenenza(bool? indconvint, string codgestione)
        {
            if (!indconvint.HasValue || String.IsNullOrEmpty(codgestione))
                return null;

            if (indconvint.Value)
            {
                if (IsDomandaENPALS(codgestione))
                    return TipoAppartenenza.AGO;

                return TipoAppartenenza.CI;
            }
            else
            {
                switch (codgestione)
                {
                    case "007":
                    case "019":
                        return TipoAppartenenza.FS;
                    default:
                        return TipoAppartenenza.AGO;
                }
            }
        }

        public static TipoDomanda GetTipoDomanda(string gruppo, string prodotto)
        {
            switch (gruppo)
            {
                case "0003":
                    return TipoDomanda.Superstiti;
                case "0031":
                    return TipoDomanda.Ricostituzione;
                case "0051":
                    switch (prodotto)
                    {
                        case "0421":
                            return TipoDomanda.RipristinoSuperstiti;
                        case "0121":
                        case "0321":
                            return TipoDomanda.Ripristino;
                        case "0422":
                            return TipoDomanda.RiliquidazioneSuperstiti;
                        case "0122":
                        case "0322":
                            return TipoDomanda.Riliquidazione;
                        default:
                            return TipoDomanda.Ripristino;
                    }
                default:
                    return TipoDomanda.Normale;
            }
        }

        public static char? GeTipoPensioneByCodeProdotto(string codeProdotto)
        {
            char? tipoPensione = null;
            List<BLCommon.GestioneDecodifica.TipoPensione> elencoTipoPensione = null;
            GestioneDecodifica.GetTipoPensione(out elencoTipoPensione);
            if (elencoTipoPensione != null)
            {
                switch (codeProdotto)
                {
                    case "0001":
                    case "0002":
                    case "0062":
                    case "0101":
                    case "0102":
                    case "0104":
                    case "0105":
                    case "0106":
                    case "0107":
                    case "0108":
                    case "0109":
                    case "0110":
                    case "0111":
                    case "0112":
                    case "0115":
                    case "0116":
                    case "0117":
                    case "0118":
                    case "0121":
                    case "0122":
                    case "0124":
                    case "0125":
                    case "0126":
                    case "0150":
                    case "0211":
                        tipoPensione = Convert.ToChar((elencoTipoPensione.Find(delegate(GestioneDecodifica.TipoPensione tipo) { return (tipo.Descrizione.ToLowerInvariant() == "vecchiaia"); })).Id.ToString());
                        break;
                    case "0011":
                    case "0012":
                    case "0013":
                    case "0014":
                    case "0301":
                    case "0302":
                    case "0303":
                    case "0304":
                    case "0305":
                    case "0306":
                    case "0307":
                    case "0308":
                    case "0309":
                    case "0310":
                    case "0311":
                    case "0312":
                    case "0315":
                    case "0316":
                    case "0317":
                    case "0318":
                    case "0319":
                    case "0321":
                    case "0322":
                    case "0325":
                    case "0326":
                    case "0330":
                    case "0350":
                    case "0391":
                        tipoPensione = Convert.ToChar((elencoTipoPensione.Find(delegate(GestioneDecodifica.TipoPensione tipo) { return (tipo.Descrizione.ToLowerInvariant() == "invalidità"); })).Id.ToString());
                        break;
                    case "0021":
                    case "0022":
                    case "0023":
                    case "0024":
                    case "0401":
                    case "0402":
                    case "0404":
                    case "0405":
                    case "0406":
                    case "0407":
                    case "0408":
                    case "0409":
                    case "0410":
                    case "0411":
                    case "0412":
                    case "0413":
                    case "0415":
                    case "0416":
                    case "0417":
                    case "0421":
                    case "0422":
                    case "0424":
                    case "0425":
                    case "0426":
                    case "0450":
                    case "0491":
                        tipoPensione = Convert.ToChar((elencoTipoPensione.Find(delegate(GestioneDecodifica.TipoPensione tipo) { return (tipo.Descrizione.ToLowerInvariant() == "indiretta"); })).Id.ToString());
                        break;
                }
            }
            return tipoPensione;
        }

        public static TipoFondo? GetTipoFondoByCategoria(bool? indConvInt, string gestione, string siglaCategoria)
        {
            TipoFondo? tipoFondo = null;
            if (GetTipoAppartenenza(indConvInt, gestione) == TipoAppartenenza.FS)
            {
                string fondo = string.Empty;
                GetFondoBySiglaCategoria(siglaCategoria, out fondo);
                tipoFondo = Utility.GetEnumTipoFondoByCategoria(fondo);
            }
            return tipoFondo;
        }

        public static TipoFondo? GetTipoFondoByCategoria(TipoAppartenenza? tipoAppartenenza, string siglaCategoria)
        {
            TipoFondo? tipoFondo = null;
            if (tipoAppartenenza == TipoAppartenenza.FS)
            {
                string fondo = string.Empty;
                GetFondoBySiglaCategoria(siglaCategoria, out fondo);
                tipoFondo = Utility.GetEnumTipoFondoByCategoria(fondo);
            }
            return tipoFondo;
        }

        public static bool IsTipologiaAbilitataByCategoria(Utility.TipoAppartenenza? tipologia, string gruppo, string prod, string tipo, string categoria, string filtro)
        {
            if (IsDomandaSPED(categoria))
            {
                if (gruppo == "0001" && prod == "0001")
                    return false;
                else if (gruppo == "0001" && prod == "0002" && (filtro == "BNX" || filtro == "BNS" || filtro == "SCO" || filtro == "RAL" || filtro == "ESO"))
                    return false;
                else if (gruppo == "0002" && prod == "0011" && filtro != "444" && filtro != "445")
                    return false;
                else if (gruppo == "0002" && prod == "0012" && filtro != "222" && filtro != "223" && filtro != "224" && filtro != "225")
                    return false;
                else if (gruppo == "0003" && prod == "0022" && (filtro == "BNX" || filtro == "BNS" || filtro == "SCO" || filtro == "RAL" || filtro == "CTT"))
                    return false;
            }
            return true;
        }
        public static void GetCodiciNatura(string naturaPensione, out char codNat1, out char codNat2, out char codNat3)
        {
            codNat1 = ' ';
            codNat2 = ' ';
            codNat3 = ' ';
            if (naturaPensione != null)
            {
                naturaPensione = naturaPensione.PadRight(3, ' ');
                codNat1 = char.Parse(naturaPensione.Substring(0, 1).ToUpperInvariant());
                codNat2 = char.Parse(naturaPensione.Substring(1, 1).ToUpperInvariant());
                codNat3 = char.Parse(naturaPensione.Substring(2, 1).ToUpperInvariant());
            }
        }

        public static List<INPS.DNA.Office> GetListaSediProvinciali()
        {
            //recupero elenco sedi escludendo quelle regionali (ZZCode == 80) e i centri operativi veri (ultime 2 cifre dell'AspnCode != 00)
            return (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                    where o.AspnCode.PadLeft(4, '0').PadRight(6, '0').Substring(4, 2) == "00" && o.ZZCode != "80"
                    select o).OrderBy(x => (x.ExtendedProperties != null ? x.ExtendedProperties["SEDE"].Trim() : x.Name.Trim())).ToList();
        }

        public static List<INPS.DNA.Office> GetListaSediECoProvinciali()
        {
            //recupero elenco sedi escludendo quelle regionali (ZZCode == 80)
            return (from o in INPS.DNA.Context.OfficeList.OfficeFullList
                    where o.ZZCode != "80"
                    select o).OrderBy(x => (x.ExtendedProperties != null ? x.ExtendedProperties["SEDE"].Trim() : x.Name.Trim())).ToList();
        }

        public static bool ExistSedeProvinciale(int codSede)
        {
            bool codSedeIsValid = false;
            if (codSede == 7005)
                return true;
            string codiceSede = codSede.ToString().PadLeft(4, '0');
            List<INPS.DNA.Office> listOffice = GetListaSediProvinciali();

            foreach (INPS.DNA.Office office in listOffice)
            {
                if (office.AspnCode.PadLeft(4, '0').Substring(0, 4) == codiceSede)
                    codSedeIsValid = true;
            }

            return codSedeIsValid;
        }

        public static INPS.DNA.Office GetOfficeByAspnCode(string aspnCode)
        {
            List<INPS.DNA.Office> listOffice = GetListaSediECoProvinciali();
            INPS.DNA.Office officeName = null;
            officeName = listOffice.Find(delegate(INPS.DNA.Office code)
            { return (code.AspnCode == aspnCode); });

            return officeName;
        }

        public static StatoPensione? GetStatoPensioneByCodice(byte codiceStatoPensione)
        {
            if (codiceStatoPensione == (int)Utility.StatoPensione.Calcolata)
                return StatoPensione.Calcolata;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcoloVerify)
                return StatoPensione.CalcoloVerify;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.DaAcquisire)
                return StatoPensione.DaAcquisire;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.DaCalcolare)
                return StatoPensione.DaCalcolare;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.InAcquisizione)
                return StatoPensione.InAcquisizione;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.NonLavorabile)
                return StatoPensione.NonLavorabile;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.ScartoDaCalcolo)
                return StatoPensione.ScartoDaCalcolo;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.ScartoVerify)
                return StatoPensione.ScartoVerify;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
                return StatoPensione.CalcolataNoWebDom;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoFelpe)
                return StatoPensione.CalcolataNoFelpe;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoOneri)
                return StatoPensione.CalcolataNoOneri;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoSAI)
                return StatoPensione.CalcolataNoSAI;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoStazLavoro)
                return StatoPensione.CalcolataNoStazLavoro;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoTotal)
                return StatoPensione.CalcolataNoTotal;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoSIN)
                return StatoPensione.CalcolataNoSIN;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoTot)
                return StatoPensione.CalcolataNoTot;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoNoteDebito)
                return StatoPensione.CalcolataNoNoteDebito;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNo6Scatti)
                return StatoPensione.CalcolataNo6Scatti;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoEquoInd)
                return StatoPensione.CalcolataNoEquoInd;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcoloNoIndeb)
                return StatoPensione.CalcoloNoIndeb;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcoloNoIndebWait)
                return StatoPensione.CalcoloNoIndebWait;
            else if (codiceStatoPensione == (int)Utility.StatoPensione.CalcolataNoIndennSpec)
                return StatoPensione.CalcolataNoIndennSpec;
            else
                return (StatoPensione?)null;
        }

        public static StatoPensione? GetStatoPensioneByDescrizione(string descStatoPensione)
        {
            if (descStatoPensione == GetDescription(Utility.StatoPensione.Calcolata))
                return StatoPensione.Calcolata;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcoloVerify))
                return StatoPensione.CalcoloVerify;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.DaAcquisire))
                return StatoPensione.DaAcquisire;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.DaCalcolare))
                return StatoPensione.DaCalcolare;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.InAcquisizione))
                return StatoPensione.InAcquisizione;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.NonLavorabile))
                return StatoPensione.NonLavorabile;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.ScartoDaCalcolo))
                return StatoPensione.ScartoDaCalcolo;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.ScartoVerify))
                return StatoPensione.ScartoVerify;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoWebDom))
                return StatoPensione.CalcolataNoWebDom;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoFelpe))
                return StatoPensione.CalcolataNoFelpe;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoOneri))
                return StatoPensione.CalcolataNoOneri;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoSAI))
                return StatoPensione.CalcolataNoSAI;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoStazLavoro))
                return StatoPensione.CalcolataNoStazLavoro;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoTotal))
                return StatoPensione.CalcolataNoTotal;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoTot))
                return StatoPensione.CalcolataNoTot;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoSIN))
                return StatoPensione.CalcolataNoSIN;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito))
                return StatoPensione.CalcolataNoNoteDebito;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNo6Scatti))
                return StatoPensione.CalcolataNo6Scatti;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoEquoInd))
                return StatoPensione.CalcolataNoEquoInd;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcoloNoIndeb))
                return StatoPensione.CalcoloNoIndeb;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcoloNoIndebWait))
                return StatoPensione.CalcoloNoIndebWait;
            else if (descStatoPensione == GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec))
                return StatoPensione.CalcolataNoIndennSpec;
            else
                return (StatoPensione?)null;
        }

        public static TipoCalcolo GetTipoCalcolo(GestionePensione.DatiPensione datiPensione)
        {
            TipoCalcolo tipoCalcolo = new TipoCalcolo();
            tipoCalcolo = TipoCalcolo.NonValido;

            if (!datiPensione.TipoCalcolo.HasValue)
                return tipoCalcolo;

            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
            GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
            GestioneDecodifica.TipoCalcolo tipoCalcoloDecodifica = null;

            switch (tipoAppartenenza)
            {
                case Utility.TipoAppartenenza.FS:
                    tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => x.Id == datiPensione.TipoCalcolo.Value.ToString());
                    break;
                case Utility.TipoAppartenenza.CI:
                    tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == datiPensione.TipoCalcolo.Value.ToString() && x.Tipo.Trim() == "Inps"));
                    break;
                case Utility.TipoAppartenenza.AGO:
                    if (IsDomandaENPALS(datiPensione.Gestione))
                        tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == datiPensione.TipoCalcolo.Value.ToString() && (x.Tipo.Trim() == "Inps" || x.Tipo.Trim() == "Enpals")));
                    else
                        tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == datiPensione.TipoCalcolo.Value.ToString() && x.Tipo.Trim() == "Inps"));
                    break;
            }

            if (tipoCalcoloDecodifica != null)
            {
                switch (tipoCalcoloDecodifica.Tipologia)
                {
                    case "FS":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                if (tipoCalcoloDecodifica.Id == "25")
                                    tipoCalcolo = TipoCalcolo.RetributivoMonti;
                                else
                                    tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 4:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 3:
                                tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                    case "AGO":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 2:
                                if (tipoCalcoloDecodifica.Tipo.Trim() == "Enpals")
                                    tipoCalcolo = TipoCalcolo.RetributivoComma707;
                                else
                                    tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 9:
                                if (tipoCalcoloDecodifica.Tipo.Trim() == "Enpals")
                                    tipoCalcolo = TipoCalcolo.MistoL214;
                                else
                                    tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                    case "CI":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 2:
                                tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 9:
                                tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                }
            }
            return tipoCalcolo;
        }

        public static TipoCalcolo GetTipoCalcoloById(byte? idTipoCalcolo, GestionePensione.DatiPensione datiPensione, TipoAppartenenza tipoAppartenenza)
        {
            TipoCalcolo tipoCalcolo = new TipoCalcolo();
            tipoCalcolo = TipoCalcolo.NonValido;

            if (!idTipoCalcolo.HasValue)
                return tipoCalcolo;


            List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
            GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
            GestioneDecodifica.TipoCalcolo tipoCalcoloDecodifica = null;

            switch (tipoAppartenenza)
            {
                case Utility.TipoAppartenenza.FS:
                    tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => x.Id == idTipoCalcolo.ToString());
                    break;
                case Utility.TipoAppartenenza.CI:
                    tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == idTipoCalcolo.ToString() && x.Tipo.Trim() == "Inps"));
                    break;
                case Utility.TipoAppartenenza.AGO:
                    if (IsDomandaENPALS(datiPensione.Gestione))
                        tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == idTipoCalcolo.ToString() && (x.Tipo.Trim() == "Inps" || x.Tipo.Trim() == "Enpals")));
                    else
                        tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => (x.Id == idTipoCalcolo.ToString() && x.Tipo.Trim() == "Inps"));
                    break;
            }

            if (tipoCalcoloDecodifica != null)
            {
                switch (tipoCalcoloDecodifica.Tipologia)
                {
                    case "FS":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                if (tipoCalcoloDecodifica.Id == "25")
                                    tipoCalcolo = TipoCalcolo.RetributivoMonti;
                                else
                                    tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 4:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 3:
                                tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                    case "AGO":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 2:
                                if (tipoCalcoloDecodifica.Tipo.Trim() == "Enpals")
                                    tipoCalcolo = TipoCalcolo.RetributivoComma707;
                                else
                                    tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 9:
                                if (tipoCalcoloDecodifica.Tipo.Trim() == "Enpals")
                                    tipoCalcolo = TipoCalcolo.MistoL214;
                                else
                                    tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                    case "CI":
                        switch (tipoCalcoloDecodifica.TraduzioneSuGP)
                        {
                            case 1:
                                tipoCalcolo = TipoCalcolo.Contributivo;
                                break;
                            case 2:
                                tipoCalcolo = TipoCalcolo.Retributivo;
                                break;
                            case 9:
                                tipoCalcolo = TipoCalcolo.Misto;
                                break;
                        }
                        break;
                }
            }
            return tipoCalcolo;
        }

        public static byte? GetTraduzioneSuGpTipoCalcolo(GestionePensione.DatiPensione datiPensione)
        {
            List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
            GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
            GestioneDecodifica.TipoCalcolo tipoCalcoloDecodifica = null;

            tipoCalcoloDecodifica = elencoTipoCalcolo.Find(x => x.Id == datiPensione.TipoCalcolo.Value.ToString());

            if (tipoCalcoloDecodifica == null)
                return null;

            List<GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondario = null;
            GestioneDecodifica.GetTipoCalcoloSecondario(out elencoTipoCalcoloSecondario);
            List<GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondarioDecodifica = null;

            elencoTipoCalcoloSecondarioDecodifica = elencoTipoCalcoloSecondario.FindAll(x => x.IdTipoCalcolo == datiPensione.TipoCalcolo &&
                x.Gruppo == datiPensione.Gruppo && x.Prodotto == datiPensione.Prodotto && x.Tipo == datiPensione.Tipo);

            if (elencoTipoCalcoloSecondarioDecodifica == null || elencoTipoCalcoloSecondarioDecodifica.Count == 0)
                return tipoCalcoloDecodifica.TraduzioneSuGP;
            else
                return elencoTipoCalcoloSecondarioDecodifica[0].TraduzioneSuGP;
        }

        public static byte? GetTipoCalcoloByTraduzioneSuGp(byte? traduzioneSuGp, GestionePensione.DatiPensione datiPensione, TipoAppartenenza? tipoAppartenenza)
        {
            List<GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondario = null;
            GestioneDecodifica.GetTipoCalcoloSecondario(out elencoTipoCalcoloSecondario);
            List<GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondarioDecodifica = null;

            elencoTipoCalcoloSecondarioDecodifica = elencoTipoCalcoloSecondario.FindAll(x => x.TraduzioneSuGP == traduzioneSuGp);

            List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
            GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
            GestioneDecodifica.TipoCalcolo tipoCalcolo = null;

            if (elencoTipoCalcoloSecondarioDecodifica == null || elencoTipoCalcoloSecondarioDecodifica.Count == 0)
            {
                if (tipoAppartenenza.HasValue)
                {
                    switch (tipoAppartenenza.Value)
                    {
                        case TipoAppartenenza.FS:
                            tipoCalcolo = elencoTipoCalcolo.Find(x => x.TraduzioneSuGP == traduzioneSuGp && x.Tipologia == "FS");
                            if (tipoCalcolo != null)
                                return Utility.StringToNullableByte(tipoCalcolo.Id);
                            break;
                        case TipoAppartenenza.AGO:
                            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                                tipoCalcolo = elencoTipoCalcolo.Find(x => x.TraduzioneSuGP == traduzioneSuGp && x.Tipologia == "AGO" && x.Tipo.Trim() == "Enpals");
                            if (tipoCalcolo == null)
                                tipoCalcolo = elencoTipoCalcolo.Find(x => x.TraduzioneSuGP == traduzioneSuGp && x.Tipologia == "AGO" && x.Tipo.Trim() == "Inps");
                            if (tipoCalcolo != null)
                                return Utility.StringToNullableByte(tipoCalcolo.Id);
                            break;
                        case TipoAppartenenza.CI:
                            tipoCalcolo = elencoTipoCalcolo.Find(x => x.TraduzioneSuGP == traduzioneSuGp && x.Tipologia == "CI");
                            if (tipoCalcolo != null)
                                return Utility.StringToNullableByte(tipoCalcolo.Id);
                            break;
                        default:
                            return null;
                    }
                }
                else
                    return null;
            }
            else
            {
                foreach (GestioneDecodifica.TipoCalcoloSecondario tcs in elencoTipoCalcoloSecondarioDecodifica)
                {
                    if (tipoAppartenenza.HasValue)
                    {
                        switch (tipoAppartenenza.Value)
                        {
                            case TipoAppartenenza.FS:
                                tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == (tcs.IdTipoCalcolo.HasValue ? tcs.IdTipoCalcolo.Value.ToString() : "") && x.Tipologia == "FS");
                                if (tipoCalcolo != null)
                                    return Utility.StringToNullableByte(tipoCalcolo.Id);
                                break;
                            case TipoAppartenenza.AGO:
                                if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                                    tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == (tcs.IdTipoCalcolo.HasValue ? tcs.IdTipoCalcolo.Value.ToString() : "") && x.Tipologia == "AGO" && x.Tipo.Trim() == "Enpals");
                                if (tipoCalcolo == null)
                                    tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == (tcs.IdTipoCalcolo.HasValue ? tcs.IdTipoCalcolo.Value.ToString() : "") && x.Tipologia == "AGO" && x.Tipo.Trim() == "Inps");
                                if (tipoCalcolo != null)
                                    return Utility.StringToNullableByte(tipoCalcolo.Id);
                                break;
                            case TipoAppartenenza.CI:
                                tipoCalcolo = elencoTipoCalcolo.Find(x => x.Id == (tcs.IdTipoCalcolo.HasValue ? tcs.IdTipoCalcolo.Value.ToString() : "") && x.Tipologia == "CI");
                                if (tipoCalcolo != null)
                                    return Utility.StringToNullableByte(tipoCalcolo.Id);
                                break;
                            default:
                                return null;
                        }
                    }
                    else
                        return null;
                }
            }
            return null;
        }

        public static string GetFiltroByCodTipoRichiesta(string codTipoRichiesta)
        {
            string filtro = string.Empty;
            GestioneDecodifica.GestioneCodiceTipoRichiesta gestioneCodTipoRichiesta = null;
            GestioneDecodifica.GetGestioneTipoRichiestaByCodTipoRichiesta(codTipoRichiesta, out gestioneCodTipoRichiesta);
            if (gestioneCodTipoRichiesta != null)
                filtro = gestioneCodTipoRichiesta.Filtro;

            if (!string.IsNullOrEmpty(filtro))
                filtro = filtro.Trim().ToUpperInvariant();
            return filtro;
        }

        public static bool? IsDecorrenzaSuccSett1989(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;
            return (datiPensione.DecorrenzaOriginaria >= new DateTime(1989, 10, 1));
        }

        public static bool IsDomandaAutomatica(GestionePensione.DatiPensione datiPensione)
        {
            return ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) || // FELPE
                    (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault()) || // CUMULO
                    (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault()) || // TOTALIZAZZIONE
                    (Utility.IsDomandaENPALS(datiPensione.Gestione))); // ENPALS
        }

        public static bool IsDomandaSalvaguardiaAutomatica(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                // Le domande ENPALS vengono considerate sempre manuali a prescindere dal filtro
                if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                    return false;

                switch (datiPensione.GetFiltro())
                {
                    case "UAA":
                    case "AAS":
                    case "VAS":
                    case "SAA":
                    case "SVA":
                    case "W3A":
                    case "W5A":
                    case "W1A":
                    case "V0A":
                    case "Y2A":
                    case "Y4A":
                    case "ZGA":
                    case "ZIA":
                    case "K2A":
                    case "K4A":
                    case "AAE":
                    case "VAE":
                    case "L6A":
                    case "L8A":
                    case "08A":
                    case "0AA":
                    case "0TA":
                    case "0VA":
                    case "S1A":
                    case "KOA":
                    case "KQA":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        public static bool IsDomandaUsuranti(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0140")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia214(GestionePensione.DatiPensione datiPensione)
        {

            //gestione per trasfomazioni AOI e sperimentale donna in regime di salvaguardia L214
            if (//sperimentale donna
                (Utility.IsDomandaSperimentaleDonna(datiPensione) && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("AAS") || datiPensione.GetFiltro().ToUpperInvariant().Equals("AMS"))
                //trasofmazioni AOI
                || (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("VAS") || datiPensione.GetFiltro().ToUpperInvariant().Equals("VMS")))
                return true;

            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002") && datiPensione.Tipo == "0144"
                && (string.IsNullOrEmpty(datiPensione.GetFiltro()) || (!datiPensione.GetFiltro().ToUpperInvariant().Equals("ZGA") && !datiPensione.GetFiltro().ToUpperInvariant().Equals("ZHM") && !datiPensione.GetFiltro().ToUpperInvariant().Equals("ZIA") && !datiPensione.GetFiltro().ToUpperInvariant().Equals("ZLM"))))
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia122(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0042")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia135(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0145")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia228(GestionePensione.DatiPensione datiPensione)
        {
            //gestione per trasfomazioni AOI regime di salvaguardia L228
            if (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && (datiPensione.GetFiltro().ToUpperInvariant().Equals("W5A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("W6M")))
                return true;

            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0146")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia124(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0151")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia124Art11Bis(GestionePensione.DatiPensione datiPensione)
        {
            string filtro = datiPensione.GetFiltro();

            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0144" && !string.IsNullOrEmpty(filtro)
                && (filtro.ToUpperInvariant().Equals("ZGA") || filtro.ToUpperInvariant().Equals("ZHM") || filtro.ToUpperInvariant().Equals("ZIA") || filtro.ToUpperInvariant().Equals("ZLM")))
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia147(GestionePensione.DatiPensione datiPensione)
        {
            //gestione per trasfomazioni AOI regime di salvaguardia L147
            if (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && (datiPensione.GetFiltro().ToUpperInvariant().Equals("K4A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("K5M")))
                return true;

            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0155")
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia147_2014(GestionePensione.DatiPensione datiPensione)
        {
            //gestione per trasfomazioni AOI regime di salvaguardia L147/2014
            if (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro())
                && (datiPensione.GetFiltro().ToUpperInvariant().Equals("L8A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("L9M")))
                return true;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0157" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) &&
                 (datiPensione.GetFiltro().ToUpperInvariant().Equals("L6A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("L7M"))) ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0157" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) &&
                 (datiPensione.GetFiltro().ToUpperInvariant().Equals("L8A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("L9M"))) ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0017" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) &&
                 (datiPensione.GetFiltro().ToUpperInvariant().Equals("L8A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("L9M")))
                )
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifica se la domanda appartiene alla Settima Salvaguardia (L. 208/2015)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaSalvaguardia208_2015(GestionePensione.DatiPensione datiPensione)
        {
            //gestione per trasfomazioni AOI regime di salvaguardia L208/2015
            if (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro())
                && (datiPensione.GetFiltro().ToUpperInvariant().Equals("08A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("0AA") || datiPensione.GetFiltro().ToUpperInvariant().Equals("09M") ||
                    datiPensione.GetFiltro().ToUpperInvariant().Equals("0BM") || datiPensione.GetFiltro().ToUpperInvariant().Equals("ZZA") || datiPensione.GetFiltro().ToUpperInvariant().Equals("ZJM")))
                return true;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0160" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) &&
                 (datiPensione.GetFiltro().ToUpperInvariant().Equals("08A") || datiPensione.GetFiltro().ToUpperInvariant().Equals("09M"))) ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0160" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) &&
                 (datiPensione.GetFiltro().ToUpperInvariant().Equals("0AA") || datiPensione.GetFiltro().ToUpperInvariant().Equals("0BM")))
                )
                return true;
            else
                return false;
        }

        public static bool IsDomandaSalvaguardia232_2016(GestionePensione.DatiPensione datiPensione)
        {
            //gestione per trasfomazioni AOI regime di salvaguardia L232_2016
            if (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() && !string.IsNullOrEmpty(datiPensione.GetFiltro())
                && (datiPensione.GetFiltro().ToUpperInvariant().Equals("0TA") || datiPensione.GetFiltro().ToUpperInvariant().Equals("0UM") || datiPensione.GetFiltro().ToUpperInvariant().Equals("0VA") ||
                    datiPensione.GetFiltro().ToUpperInvariant().Equals("0ZM")))
                return true;

            //gestione per PL
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0165" &&
                datiPensione.GetFiltro().ToUpperInvariant() == "0TA" || datiPensione.GetFiltro().ToUpperInvariant() == "0UM") //filtro per automatiche e manuali
                ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0165" &&
                datiPensione.GetFiltro().ToUpperInvariant() == "0VA" || datiPensione.GetFiltro().ToUpperInvariant() == "0ZM")) // filtro per automatiche e manuali
                return true;
            else
                return false;

        }

        public static bool IsDomandaSalvaguardia178_2020(GestionePensione.DatiPensione datiPensione)
        {
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0181" &&
                datiPensione.GetFiltro().ToUpperInvariant() == "KOA" || datiPensione.GetFiltro().ToUpperInvariant() == "KPM") //filtro per automatiche e manuali
                ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0181" &&
                datiPensione.GetFiltro().ToUpperInvariant() == "KQA" || datiPensione.GetFiltro().ToUpperInvariant() == "KRM")) // filtro per automatiche e manuali
                return true;
            else
                return false;
        }

        public static bool IsDomandaEsuberiPA(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002")
                && datiPensione.Tipo == "0147")
                return true;
            else
                return false;
        }

        public static bool IsDomandaVecchPerditaTitolo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002"
               && datiPensione.Tipo == "0043" && datiPensione.GetFiltro() == "PTA")
                return true;
            else
                return false;
        }

        public static bool IsGestioneSpeciale(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaUsuranti(datiPensione) || IsDomandaSalvaguardia122(datiPensione) ||
                IsDomandaSalvaguardia214(datiPensione) || IsDomandaSalvaguardia135(datiPensione) ||
                IsDomandaSalvaguardia228(datiPensione) || IsDomandaSalvaguardia124(datiPensione) ||
                IsDomandaSalvaguardia124Art11Bis(datiPensione) || IsDomandaSalvaguardia147(datiPensione) ||
                IsDomandaEsuberiPA(datiPensione) || IsDomandaSalvaguardia147_2014(datiPensione) || IsDomandaSalvaguardia208_2015(datiPensione) ||
                IsDomandaSalvaguardia178_2020(datiPensione) || IsDomandaAPEPrecoci(datiPensione))
                return true;
            else
                return false;
        }

        public static bool IsGestioneART_COM_CDCM(GestionePensione.DatiPensione datiPensione)
        {
            switch (datiPensione.Gestione)
            {
                case "002": //CD/CM
                case "003": //ART
                case "004": //COM
                case "005": //AUT
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRimpatriatiAlbania(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.GetFiltro() == "RAL" || datiPensione.GetFiltro() == "R44" || datiPensione.GetFiltro() == "R45")
                return true;
            return false;
        }

        public static bool IsDomandaLIPE(GestionePensione.DatiPensione datiPensione, bool isDiretta)
        {
            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria) || Utility.IsDomandaSOPGI(datiPensione.SiglaCategoria))
                return false;

            if ((datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021") ||
                (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0421" && datiPensione.Tipo == "0027") ||
                (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0422" && datiPensione.Tipo == "0027") ||
                (datiPensione.Gruppo != "0031" && isDiretta))
                return true;
            else
                return false;
        }

        public static bool IsDomandaAGOReversibile(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.NCertificato.HasValue)
            {
                //Reversibilità quando la sigla categoria inizia con S o (inizia con PMO con 3° carattere del certificato pari a 3, 6) o 
                //(inizia con PSO con 3° carattere del certificato pari a 3, 6, 9. Diretta negli altri casi
                if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant().StartsWith("S") ||
                    (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "PMO" &&
                    (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "3" ||
                    datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "6")) ||
                    (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "PSO" &&
                    (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "3" ||
                    datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "6" ||
                    datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "9")))
                    return true;
            }
            return false;
        }

        public static bool IsDomandaSuperstiti_PMO(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                //Reversibilità quando la sigla categoria inizia con S o (inizia con PMO con 3° carattere del certificato pari a 3, 6) o 
                //(inizia con PSO con 3° carattere del certificato pari a 3, 6, 9. Diretta negli altri casi
                if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "PMO" &&
                    ((datiPensione.NCertificato.HasValue && (datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "3" ||
                    datiPensione.NCertificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "6")) || datiPensione.Gruppo == "0003"))
                    return true;
            }
            return false;
        }

        public static TipoUnicarpe IsDomandaUnicarpe(GestionePensione.DatiPensione datiPensione, bool dettaglio)
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

        public static TipoUnicarpe IsDomandaUnicarpe(bool? falgUnicarpe, char? tipoLettura, bool dettaglio)
        {
            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            datiPensione.FlagUnicarpe = falgUnicarpe;
            datiPensione.TipoLetturaUnicarpe = tipoLettura;
            return IsDomandaUnicarpe(datiPensione, dettaglio);
        }

        /// <summary>
        /// Ritorna true se data1 è >= di data2
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static bool DataSuccessivaA(DateTime data1, DateTime data2)
        {
            if (DateTime.Compare(data1.Date, data2.Date) < 0)
                return false;
            return true;
        }

        /// <summary>
        /// Ritorna true se la prima data è strettamente maggiore della seconda senza considerare il giorno
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>

        public static bool DataStrettamenteSuccessivaSenzaGiorno(DateTime data1, DateTime data2)
        {

            int year1 = data1.Year;
            int year2 = data2.Year;
            int month1 = data1.Month;
            int month2 = data2.Month;

            if (year1 <= year2)
            {
                if ((month1 <= month2 && year1 == year2) || year1 < year2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Ritorna true se data è >= alla data costruita con anno/mese/giorno
        /// </summary>
        /// <param name="data"></param>
        /// <param name="anno"></param>
        /// <param name="mese"></param>
        /// <param name="giorno"></param>
        /// <returns></returns>
        public static bool DataSuccessivaA(DateTime data, int anno, int mese, int giorno)
        {
            return (data.Year * 10000) + (data.Month * 100) + data.Day >= (anno * 10000) + (mese * 100) + giorno;
        }

        /// <summary>
        /// Ritorna true se data1 è > di data2
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static bool DataStrettamenteSuccessivaA(DateTime data1, DateTime data2)
        {
            if (DateTime.Compare(data1.Date, data2.Date) <= 0)
                return false;
            return true;
        }

        /// <summary>
        /// Ritorna true se data è > alla data costruita con anno/mese/giorno
        /// </summary>
        /// <param name="data"></param>
        /// <param name="anno"></param>
        /// <param name="mese"></param>
        /// <param name="giorno"></param>
        /// <returns></returns>
        public static bool DataStrettamenteSuccessivaA(DateTime data, int anno, int mese, int giorno)
        {
            return (data.Year * 10000) + (data.Month * 100) + data.Day > (anno * 10000) + (mese * 100) + giorno;
        }

        /// <summary>
        /// Routine PCIPL94
        /// </summary>
        /// <param name="dataSuccessiva"></param>
        /// <param name="dataAntecedente"></param>
        /// <returns></returns>
        public static int NSettimaneBetweenDate(DateTime dataSuccessiva, DateTime dataAntecedente)
        {
            int sett = 0;
            TimeSpan ts = dataSuccessiva - dataAntecedente;

            if (ts.Days > 0)
                return sett = (int)Math.Ceiling(ts.Days / 7.0);
            else
                return 0;
        }

        public static int GetTipoPensioneForVolo(Utility.TipoFondo? tipoFondo, short? codArt22, char? codSpecifico)
        {
            int tipoPensione = 0;
            if (tipoFondo.HasValue && tipoFondo.Value == TipoFondo.VL)
            {
                if (codArt22.HasValue)
                {
                    switch (codArt22.Value)
                    {
                        case 1:
                            tipoPensione = 2;
                            break;
                        case 2:
                            tipoPensione = 1;
                            break;
                        case 3:
                        case 5:
                            tipoPensione = 4;
                            break;
                        case 4:
                            if (codSpecifico.HasValue)
                            {
                                switch (codSpecifico.Value)
                                {
                                    case 'P':
                                        tipoPensione = 5;
                                        break;
                                    case 'Q':
                                        tipoPensione = 6;
                                        break;
                                    default:
                                        tipoPensione = 4;
                                        break;
                                }
                            }
                            else
                                tipoPensione = 4;
                            break;
                        case 6:
                            tipoPensione = 7;
                            break;
                    }
                }
            }
            return tipoPensione;
        }

        public static byte? CalcolaArticolo22ForVolo(GestionePensione.DatiPensione datiPensione)
        {
            byte? codArt22 = null;

            if (datiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == TipoFondo.VL)
                {
                    switch (datiPensione.Gruppo)
                    {
                        case "0001":
                            switch (datiPensione.Prodotto)
                            {
                                case "0001":
                                    codArt22 = 1; //pensione anzianità
                                    break;
                                case "0002":
                                    codArt22 = 2; //pensione di vecchiaia
                                    break;
                            }
                            break;
                        case "0002":
                            switch (datiPensione.Prodotto)
                            {
                                case "0011":
                                case "0012":
                                    switch (datiPensione.Tipo)
                                    {
                                        case "0001":
                                            codArt22 = 4; //pensione di invalidità generica
                                            break;
                                    }
                                    break;
                                case "0013":
                                    switch (datiPensione.Tipo)
                                    {
                                        case "0001":
                                            codArt22 = 4; //pensione di invalidità generica
                                            break;
                                        case "0011":
                                            codArt22 = 3; //pensione di invalidità specifica
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "0003":
                            switch (datiPensione.Prodotto)
                            {
                                case "0022":
                                    switch (datiPensione.Tipo)
                                    {
                                        case "0001":
                                            codArt22 = 6; //pensione indiretta
                                            break;
                                        case "0005":
                                            codArt22 = 7; //pensione indiretta privilegiata
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return codArt22;
        }

        public static byte? CalcolaCodiceSpecificoForVolo(GestionePensione.DatiPensione datiPensione)
        {
            byte? codSpecifico = null;

            if (datiPensione != null)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue && tipoFondo.Value == TipoFondo.VL)
                {
                    switch (datiPensione.Gruppo)
                    {
                        case "0002":
                            switch (datiPensione.Prodotto)
                            {
                                case "0011":
                                case "0012":
                                    switch (datiPensione.Tipo)
                                    {
                                        case "0001":
                                            if (datiPensione.Prodotto == "0011")
                                                codSpecifico = 38; //assegno ordinario di invalidità
                                            else if (datiPensione.Prodotto == "0012")
                                                codSpecifico = 39; //pensione di inabilità
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return codSpecifico;
        }

        public static bool IsTipoAppartenenzaEquals(TipoAppartenenza? tipoAppDomanda, TipoAppartenenza? tipoAppRuolo)
        {
            if (!tipoAppDomanda.HasValue || !tipoAppRuolo.HasValue)
                return false;

            if (!(tipoAppDomanda.Value.Equals(tipoAppRuolo.Value)))
                return false;

            return true;
        }

        public static bool IsTelematica(string codiceProcedura)
        {
            if (string.IsNullOrEmpty(codiceProcedura))
                return false;
            switch (codiceProcedura)
            {
                case "NI":
                    return true;
                default:
                    return false;
            }
        }

        public static bool? IsDomandaTrasformazioneAOI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;

            if ((IsDomandaVecchiaiaTrasformazioneAOI(datiPensione).GetValueOrDefault() ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0002"))
                && !IsDomandaINPDAP(datiPensione.Gestione))
                return true;
            else
                return false;
        }

        public static bool? IsDomandaVecchiaiaTrasformazioneAOI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0002")
                return true;
            else
                return false;
        }

        public static bool? IsDomandaVecchiaiaTrasformazioneAOIPerINPDAP(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")
                return true;
            else
                return false;
        }
        /// <summary>
        /// Verifica se una domanda è una Pensione di vecchiaia a seguito di trasformazione Pensione di invalidita'
        /// </summary>
        public static bool IsDomandaTrasformazioneInvalidita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0003")
                return true;
            return false;
        }

        public static int? GetIdFromUfficioPagatore(string ufficioPagatore)
        {
            GestioneDecodifica.UfficiPagatoriEsteri ufficio = null;

            List<GestioneDecodifica.UfficiPagatoriEsteri> elencoUfficiPagatori = null;
            GestioneDecodifica.GetUfficiPagatoriEsteri(out elencoUfficiPagatori);
            if (elencoUfficiPagatori != null && elencoUfficiPagatori.Count > 0)
                ufficio = elencoUfficiPagatori.Find(delegate(GestioneDecodifica.UfficiPagatoriEsteri code)
                { return (code.Descrizione == ufficioPagatore); });

            return ufficio != null ? ufficio.Id : (int?)null;
        }

        public static string GetUfficioPagatoreFromId(int? idUfficioPagatore)
        {
            List<GestioneDecodifica.UfficiPagatoriEsteri> elencoUfficiPagatori = null;
            GestioneDecodifica.GetUfficiPagatoriEsteri(out elencoUfficiPagatori);
            GestioneDecodifica.UfficiPagatoriEsteri ufficio = null;
            if (elencoUfficiPagatori != null && elencoUfficiPagatori.Count > 0 && idUfficioPagatore.HasValue)
            {
                ufficio = elencoUfficiPagatori.Find(delegate(GestioneDecodifica.UfficiPagatoriEsteri code)
                { return (code.Id == idUfficioPagatore); });
            }

            return ufficio != null ? ufficio.Descrizione : string.Empty;
        }

        // Per i Fondi i mesi si considerano da 30 giorni, mentre per AGO e CI si considerano da 31 giorni
        /// <summary>
        /// Data1 - Data2
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="tipoAppartenenza"></param>
        /// <returns></returns>
        public static DifferenzaDateTime DifferenzaBetweenDate(DateTime? data1, DateTime? data2, TipoAppartenenza? tipoAppartenenza)
        {
            int app;
            DifferenzaDateTime diffDate = new DifferenzaDateTime();
            DifferenzaDateTime diffDateLimitInf = new DifferenzaDateTime();

            if (!data1.HasValue || !data2.HasValue)
                return diffDate;

            int giorni1 = 0;
            int giorni2 = 0;
            if (tipoAppartenenza.HasValue)
            {
                switch (tipoAppartenenza.Value)
                {
                    case TipoAppartenenza.AGO:
                    case TipoAppartenenza.CI:
                        giorni1 = data1.Value.Year * 372 + data1.Value.Month * 31 + data1.Value.Day;
                        giorni2 = data2.Value.Year * 372 + data2.Value.Month * 31 + data2.Value.Day;

                        // Aggiungo tre giorni per riportarmi al 31 del mese perchè per AGO e CI il mese è composto da 31 giorni
                        if (data1.Value.Month == 2 && data1.Value.Day == 28 && data1.Value.Year % 4 != 0)
                            giorni1 += 3;
                        if (data2.Value.Month == 2 && data2.Value.Day == 28 && data1.Value.Year % 4 != 0)
                            giorni2 += 3;
                        if (data1.Value.Month == 2 && data1.Value.Day == 29)
                            giorni1 += 2;
                        if (data2.Value.Month == 2 && data2.Value.Day == 29)
                            giorni2 += 2;


                        app = giorni1 - giorni2;

                        diffDate.Year = app / 372;
                        app = app % 372;
                        diffDate.Month = app / 31;
                        diffDate.Day = app % 31;

                        while (diffDate.Day > 30)
                        {
                            diffDate.Month += 1;
                            diffDate.Day -= 31;
                        }
                        while (diffDate.Month > 11)
                        {
                            diffDate.Year += 1;
                            diffDate.Month -= 12;
                        }
                        break;

                    case TipoAppartenenza.FS:
                        // Riporto l'ultimo giorno a 30 perchè per FS il mese è composto da 30 giorni
                        if (data1.Value.Day > 30)
                            data1 = data1.Value.AddDays(-1);
                        if (data2.Value.Day > 30)
                            data2 = data2.Value.AddDays(-1);

                        giorni1 = data1.Value.Year * 360 + data1.Value.Month * 30 + data1.Value.Day;
                        giorni2 = data2.Value.Year * 360 + data2.Value.Month * 30 + data2.Value.Day;

                        // Aggiungo due giorni per riportarmi al 30 del mese perchè per FS il mese è composto da 30 giorni
                        if (data1.Value.Month == 2 && data1.Value.Day == 28 && data1.Value.Year % 4 != 0)
                            giorni1 += 2;
                        if (data2.Value.Month == 2 && data2.Value.Day == 28 && data1.Value.Year % 4 != 0)
                            giorni2 += 2;
                        if (data1.Value.Month == 2 && data1.Value.Day == 29)
                            giorni1 += 1;
                        if (data2.Value.Month == 2 && data2.Value.Day == 29)
                            giorni2 += 1;

                        app = giorni1 - giorni2;

                        diffDate.Year = app / 360;
                        app = app % 360;
                        diffDate.Month = app / 30;
                        diffDate.Day = app % 30;

                        while (diffDate.Day > 29)
                        {
                            diffDate.Month += 1;
                            diffDate.Day -= 30;
                        }
                        while (diffDate.Month > 11)
                        {
                            diffDate.Year += 1;
                            diffDate.Month -= 12;
                        }
                        break;
                }
            }

            if (diffDate < diffDateLimitInf)
                return diffDateLimitInf;
            else
                return diffDate;
        }

        /// <summary>
        /// PCIPL93
        /// Verifica se la data ha un formato corretto:
        ///  - data presente;
        ///  - data non successiva alla data odierna;
        ///  - data non antecedente al 1880.
        ///  Per personalizzare il messaggio inserire in testa al messaggio di errore il nome della data che si sta verificando
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errore"></param>
        /// <returns>False se c'è stato errore</returns>
        public static bool VerificaData(DateTime? data, Utility.TipoAppartenenza? tipoApp, out string errore)
        {
            errore = string.Empty;
            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoApp);

            if (!data.HasValue)
            {
                errore = "Data mancante o errata";
                return false;
            }

            if (DataStrettamenteSuccessivaA(data.Value, dataSistema))
            {
                errore = "Data posteriore alla data odierna";
                return false;
            }

            if (!DataSuccessivaA(data.Value, new DateTime(1880, 01, 01)))
            {
                errore = "Data antecedente il 1880";
                return false;
            }

            return true;
        }

        public static bool IsDomandaVDAI(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim() == "VDAI")
                return true;
            return false;
        }

        public static bool IsDomandaIDAI(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "IDAI")
                return true;
            return false;
        }

        public static bool IsDomandaVO(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "VO")
                return true;
            return false;
        }

        /// <summary>
        /// ritorna la decorrenza dante casusa se presente, altrimenti la decorrenza pensione
        /// </summary>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <returns></returns>
        public static DateTime? GetDecorrenzaPensioneOrDecorrenzaDantecausa(DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta)
        {
            if (decorrenzaDiretta.HasValue)
                return decorrenzaDiretta;
            else
                return decorrenzaPensione;
        }

        public static bool? IsVecchiaiaInvaliditaSupplementare(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                //Vecchiaia
                //ENG - VOAUT 0001-0002-0192
                if (datiPensione.Gruppo.Equals("0001") && datiPensione.Prodotto.Equals("0002") && (datiPensione.Tipo.Equals("0009") || datiPensione.Tipo.Equals("0192")))
                    return true;
                //Invalidità
                if (datiPensione.Gruppo.Equals("0002") && datiPensione.Prodotto.Equals("0013") && datiPensione.Tipo.Equals("0009"))
                    return true;
            }
            return false;
        }

        public static bool IsVecchiaiaSupplementare(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                if (datiPensione.Gruppo.Equals("0001") && datiPensione.Prodotto.Equals("0002") && datiPensione.Tipo.Equals("0009"))
                    return true;
            }
            return false;
        }

        public static bool IsVecchiaiaNonSupplementare(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                if (datiPensione.Gruppo.Equals("0001") && datiPensione.Prodotto.Equals("0002") && !datiPensione.Tipo.Equals("0009"))
                    return true;
            }
            return false;
        }

        public static bool IsDomandaInvaliditaSupplementareOrRicostituzione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                if ((datiPensione.Gruppo.Equals("0002") && datiPensione.Prodotto.Equals("0013") && datiPensione.Tipo.Equals("0009")) ||
                    (IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("I") && !string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                     datiPensione.NaturaPensione.StartsWith("5")))
                    return true;
            }
            return false;
        }

        public static bool IsMaggiorazioniSceltaLavMadri(GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            if (datiMaggiorazioniBenefici == null)
                return false;

            if (datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "12" || datiMaggiorazioniBenefici.TipoSettimaneBeneficio == "15")
                return true;

            return false;
        }

        public static bool CheckAbbinamentoMaggiorazioniSceltaLavMadri(string TipoSettimaneBeneficio, byte? SceltaLavMadre)
        {
            if (TipoSettimaneBeneficio == null || SceltaLavMadre == null)
                return true;

            if ((TipoSettimaneBeneficio == "12" && SceltaLavMadre == 1) || (TipoSettimaneBeneficio == "15" && SceltaLavMadre == 2))
                return true;

            return false;
        }

        public static bool IsBititolaritaVisible(string NaturaPensione)
        {
            if (String.IsNullOrEmpty(NaturaPensione))
                return false;

            return (NaturaPensione.Substring(0, 1) == "2" || NaturaPensione.Substring(0, 1) == "4" || NaturaPensione.Substring(0, 1) == "5" || NaturaPensione.Substring(0, 1) == "6" || NaturaPensione.Substring(0, 1) == "9");
        }

        public static TipoQuadro? GetVisibilitaQuadroSupplementi(GestionePensione.DatiPensione datiPensione, string naturaPensione, bool isRiaperturaDomanda, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            //28-05-12: menu supplementi non più visibile per assegno di invalidità
            TipoQuadro? ret = null;
            Utility.TipoAppartenenza? tipoAppartenenza = null;
            if (datiPensione.IndConvInt.HasValue && !string.IsNullOrEmpty(datiPensione.Gestione))
            {
                tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoDomanda? tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                        switch (tipoFondo)
                        {
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.CL:
                            case Utility.TipoFondo.PL:
                                ret = TipoQuadro.NonVisibile;
                                break;
                            case Utility.TipoFondo.ET:
                                if ((!String.IsNullOrEmpty(datiPensione.Prodotto) && (datiPensione.Prodotto.Trim() == "0001" || datiPensione.Prodotto.Trim() == "0002") &&
                                     !String.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1).ToUpperInvariant() == "Y") ||
                                    Utility.IsDomandaPensioneIndiretta(datiPensione))
                                    ret = TipoQuadro.Facoltativo;
                                else
                                    ret = TipoQuadro.NonVisibile;
                                break;
                            default:
                                if (!String.IsNullOrEmpty(datiPensione.Prodotto) && (datiPensione.Prodotto.Trim() == "0001" || datiPensione.Prodotto.Trim() == "0002") &&
                                    !String.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1).ToUpperInvariant() == "Y")
                                    ret = TipoQuadro.Facoltativo;
                                else
                                    ret = TipoQuadro.NonVisibile;
                                break;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                    case Utility.TipoAppartenenza.AGO:
                        if (tipoDomanda != null && (tipoDomanda == Utility.TipoDomanda.Superstiti || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti))
                        {
                            if (tipoAppartenenza == Utility.TipoAppartenenza.CI && tipoDomanda == Utility.TipoDomanda.Superstiti)
                            {
                                ret = TipoQuadro.Facoltativo;
                            }
                            else if ((Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsDomandaReversibilita(datiPensione)) ||
                                (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim() == "SO" && IsDomandaPensioneIndiretta(datiPensione) && !IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                                || (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa) && Utility.IsDomandaReversibilita(datiPensione))
                                || ((Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)) && Utility.IsDomandaReversibilita(datiPensione)))
                                ret = TipoQuadro.Facoltativo;
                            else
                                ret = TipoQuadro.NonVisibile;
                        }
                        else if (isRiaperturaDomanda && Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                            ret = TipoQuadro.Facoltativo;
                        else
                        {
                            //20151029 G.Arru - Per le VESO33 e VESO92 i supplementi non sono previsti
                            if (Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) || Utility.IsDomandaSPED(datiPensione) || Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria)
                                || (Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) && !Utility.IsRicostituzione(datiPensione.Gruppo))
                                || (Utility.IsDomandaSOCUM(datiPensione.SiglaCategoria) && !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa)))
                                || (Utility.IsDomandaVOTOT(datiPensione.SiglaCategoria) && !Utility.IsRicostituzione(datiPensione.Gruppo))
                                || (Utility.IsDomandaSOTOT(datiPensione.SiglaCategoria) && !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa)))
                                || Utility.IsDomandaIOTOT(datiPensione.SiglaCategoria) || Utility.IsDomandaIOCUM(datiPensione.SiglaCategoria) || Utility.IsRenditaCasalinghe(datiPensione)
                                || Utility.IsRenditaFacoltativa(datiPensione) || Utility.IsDomandaVOST(datiPensione.SiglaCategoria) || Utility.IsDomandaPSO(datiPensione.SiglaCategoria)
                                )
                            {
                                ret = TipoQuadro.NonVisibile;
                            }
                            //20151023 - nuova logica 
                            else if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1).ToUpperInvariant() == "V")
                            {
                                if (Utility.IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                                    ret = TipoQuadro.Facoltativo;
                                else
                                    ret = TipoQuadro.Obbligatorio;
                            }
                            else
                                ret = TipoQuadro.Facoltativo;
                        }
                        break;
                }
            }

            if ((IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaPMO(datiPensione.SiglaCategoria)) && tipoAppartenenza != TipoAppartenenza.FS && (!ret.HasValue || ret.Value == TipoQuadro.NonVisibile))
                ret = TipoQuadro.Facoltativo;

            return ret;
        }

        public static bool IsDomandaENPALS(string codiceGestione)
        {
            if (codiceGestione == "018")
                return true;
            else
                return false;
        }

        public static bool IsDomandaINPDAP(string codiceGestione)
        {
            if (codiceGestione == "019")
                return true;
            else
                return false;
        }

        public static bool IsPensioneInabilitaPost2012(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)))
                return true;
            else return false;
        }

        public static bool IsRicostituzione(string gruppo)
        {
            if (gruppo == "0031")
                return true;
            return false;
        }

        public static bool IsRicostituzione_VariazionePerDecorrenza(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0110")
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0107/0307/0407
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_MotiviContributivi(GestionePensione.DatiPensione datiPensione)
        {
            if ((datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0107" || datiPensione.Prodotto == "0307" || datiPensione.Prodotto == "0407")) ||
                (IsDomandaRicPensioneOrdinariaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneInabilitaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneIndirettaInabilitaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(datiPensione)))
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0107/0307/0407
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_MotiviContributivi(string gruppo, string prodotto)
        {
            if (gruppo == "0031" &&
                (prodotto == "0107" || prodotto == "0307" || prodotto == "0407"))
                return true;
            return false;
        }

        //ENG - MEMO 50/2023
        public static bool IsRicostituzione_PerVariazioneDatiSupplemento(GestionePensione.DatiPensione datiPensione)
        {
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza.GetValueOrDefault() != TipoAppartenenza.CI)
            {
                if (datiPensione.Gruppo == "0031" &&
                    (datiPensione.Prodotto == "0107" || datiPensione.Prodotto == "0307" || datiPensione.Prodotto == "0407") && datiPensione.Tipo == "0193")
                    return true;
            }
            return false;
        }

        public static bool IsRicostituzione_PerVariazioneDatiSupplementoAll(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0107" || datiPensione.Prodotto == "0307" || datiPensione.Prodotto == "0407") && datiPensione.Tipo == "0193")
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0102/0302/0402
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_Supplemento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0102" || datiPensione.Prodotto == "0302" || datiPensione.Prodotto == "0402"))
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0102/0302
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_SupplementoAutomatico(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0102" || datiPensione.Prodotto == "0302"))
                return true;
            return false;
        }

        /// <summary>
        /// True se il gruppo è 0031 e il prodotto è 0101/0301/0401
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsRicostituzione_Reddituale(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0101" || datiPensione.Prodotto == "0301" || datiPensione.Prodotto == "0401"))
                return true;
            return false;
        }

        public static bool IsRicostituzione_TrattamentoDiFamiglia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0031" &&
                (datiPensione.Prodotto == "0104" || datiPensione.Prodotto == "0304" || datiPensione.Prodotto == "0404"))
                return true;
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una Pensione di Reversibilità (Gruppo = 0003 Prodotto = 0021)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>True se il gruppo è 0003 e il prodotto è 0021</returns>
        public static bool IsDomandaReversibilita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                return true;
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una Pensione di Reversibilità (Gruppo = 0003 Prodotto = 0021)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>True se il gruppo è 0003 e il prodotto è 0021</returns>
        public static bool IsDomandaReversibilitaOrRicostituzione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            return IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, null);
        }

        //ENG - RIC REVERSIBILITA 024: implementazione flusso anche per riconoscere le reversibilità "vecchie" 
        public static bool IsDomandaReversibilitaOrRicostituzione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione)
        {
            if ((datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021") ||
                 (IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && datiDanteCausa != null &&
                   !string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) && !string.IsNullOrEmpty(datiDanteCausa.Sede) && datiDanteCausa.Certificato.HasValue))
                return true;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza == Utility.TipoAppartenenza.FS && IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) && Utility.IsRicostituzione(datiPensione.Gruppo)
                && (tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS))
            {
                if (datiDanteCausa != null)
                {
                    if (string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) || string.IsNullOrEmpty(datiDanteCausa.Sede) || datiDanteCausa.Sede.PadLeft(4, '0') == "0000"
                        || !datiDanteCausa.Certificato.HasValue || datiDanteCausa.Certificato.Value == 0)
                    {
                        if (datiLavorazione != null && datiLavorazione.TipoReversibilita.HasValue && datiLavorazione.TipoReversibilita.Value.ToString().ToUpperInvariant() == "R")
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool IsRicostituzioneContributivaPerEsecuzioneSentenza(GestionePensione.DatiPensione datiPensione)
        {
            if ((datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0107" && datiPensione.Tipo == "0169") ||
               (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0307" && datiPensione.Tipo == "0169") ||
               (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0407" && datiPensione.Tipo == "0169"))
                return true;
            return false;
        }

        public static T? GetValueFromDescription<T>(string description) where T : struct
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return null;
        }

        private static Utility.TipoFondo? GetEnumTipoFondoByCategoria(string fondo)
        {
            Utility.TipoFondo? tipoFondo = null;
            while (!string.IsNullOrEmpty(fondo) && tipoFondo == null)
            {
                tipoFondo = GetValueFromDescription<Utility.TipoFondo>(fondo);
                fondo = fondo.Remove(fondo.Length - 1);
            }

            return tipoFondo;
        }

        public static bool IsTabPrepensionamentoVisible(GestionePensione.DatiPensione datiPensione, int? attivitaEconomica, int? professioneIndividuale, string naturaPensione)
        {
            if ((attivitaEconomica.GetValueOrDefault() == 92 && professioneIndividuale.GetValueOrDefault() == 257) ||
                (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 326) ||
                (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 350) ||
                (attivitaEconomica.GetValueOrDefault() == 4 && professioneIndividuale.GetValueOrDefault() == 350) ||
                (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(2, 1).Equals("O") && !IsPrepensionamentoEditoriaFiltroEAA(datiPensione) &&
                 !Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) && !Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) && !IsPrepensionamentoEditoriaFiltroEBA(datiPensione)))
                return true;

            return false;
        }

        public static bool IsTabPrepensionamentoVisible(GestionePensione.DatiPensione datiPensione, int? attivitaEconomica, int? professioneIndividuale, string naturaPensione, out int codiceLegge, out string tipoBeneficio)
        {
            codiceLegge = 0;
            tipoBeneficio = string.Empty;

            if (attivitaEconomica.GetValueOrDefault() == 92 && professioneIndividuale.GetValueOrDefault() == 257)
            {
                codiceLegge = 2001;
                tipoBeneficio = "04";
                return true;
            }

            if (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 326)
            {
                codiceLegge = 2005;
                tipoBeneficio = "04";
                return true;
            }

            if (attivitaEconomica.GetValueOrDefault() == 3 && professioneIndividuale.GetValueOrDefault() == 350)
            {
                codiceLegge = 2006;
                tipoBeneficio = "04";
                return true;
            }

            if (attivitaEconomica.GetValueOrDefault() == 4 && professioneIndividuale.GetValueOrDefault() == 350)
            {
                codiceLegge = 2007;
                tipoBeneficio = "09";
                return true;
            }

            if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(2, 1).Equals("O") && !IsPrepensionamentoEditoriaFiltroEAA(datiPensione) &&
                !Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) && !Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione))
            {
                codiceLegge = 903;
                return true;
            }

            return false;
        }

        public static string SetVersioni(List<GestioneVersioni.DatiVersioni> listaVersioni, Utility.TipoAppartenenza? tipoApp, Utility.ChiaviVersioni keyCD)
        {
            string messaggio = string.Empty;

            if (listaVersioni != null && listaVersioni.Count > 0)
            {
                GestioneVersioni.DatiVersioni versione = listaVersioni.Find(x => x.Applicativo == (keyCD.ToString() + (tipoApp.HasValue ? tipoApp.Value.ToString() : string.Empty)));
                if (versione != null)
                    return messaggio = versione.Applicativo + " v." + versione.NumVersione + " del " + string.Format("{0:dd/MM/yyyy}", versione.Data);
            }

            return messaggio;
        }

        public static short? GetTipoPensioneForFondi(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, char? codiceSpecifico)
        {
            List<GestioneDecodifica.TipoPensioneFondi> elencoTipoPensioneFondi = null;
            GestioneDecodifica.GetTipoPensioneFondi(out elencoTipoPensioneFondi);

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.CL:
                        elencoTipoPensioneFondi = elencoTipoPensioneFondi.FindAll(x => x.Fondo.Trim() == tipoFondo.ToString() && x.Gruppo.Equals(datiPensione.Gruppo));
                        break;
                    case Utility.TipoFondo.VL:
                        elencoTipoPensioneFondi = elencoTipoPensioneFondi.FindAll(x => x.Fondo.Trim() == tipoFondo.ToString() && x.Gruppo.Equals(datiPensione.Gruppo) && x.Prodotto.Equals(datiPensione.Prodotto) &&
                            (x.Tipo == datiPensione.Tipo || x.Tipo == null) && (x.CodiceSpecifico == codiceSpecifico || x.CodiceSpecifico == null));
                        break;
                }
            }

            if (elencoTipoPensioneFondi != null && elencoTipoPensioneFondi.Count == 1)
                return elencoTipoPensioneFondi.First().TipoPensione;

            return null;
        }

        public static bool IsResidenteEstero(string codCatastale)
        {
            if (string.IsNullOrEmpty(codCatastale))
                return false;

            if (codCatastale == "Z000") // ITALIA
                return false;

            if (codCatastale.StartsWith("Z"))
                return true;

            return false;
        }

        public static bool IsEsenzioneFiscaleEsteroINPDAP(string codiceComuneResidenza)
        {

            DatiCtrlStatiSenzaEsenzioneFiscaleEstera statoEsenzioneFiscaleEsteraINPDAP = null;
            GestioneCtrlStatiSenzaEsenzioneFiscaleEstera.GetStatoEsenzioneFiscaleEsteraINPDAP(codiceComuneResidenza, out statoEsenzioneFiscaleEsteraINPDAP);

            if (Utility.IsResidenteEstero(codiceComuneResidenza))
            {
                if (statoEsenzioneFiscaleEsteraINPDAP == null)
                    return false;
                else
                    return true;
            }
            else
                return false;


        }
        public static bool IsEsenzioneFiscaleEstero(GestionePensione.DatiPensione datiPensione, string codiceComuneResidenza, GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda)
        {
            bool? IsEccezioneEsenzione = IsEccezioneEsenzioneFiscaleEstero(datiPensione, isRiaperturaDomanda);
            if (IsEccezioneEsenzione.HasValue)
                return IsEccezioneEsenzione.GetValueOrDefault();

            bool? isEsenzioneEsteroFromGP = IsEsenzioneFiscaleEsteroFromDetrazioni(datiPensione, datiDetrazioni, isRiaperturaDomanda);
            if (isEsenzioneEsteroFromGP.HasValue)
                return isEsenzioneEsteroFromGP.GetValueOrDefault();

            //FUNZIONAMENTO COME PRIMA
            DatiCtrlStatiSenzaEsenzioneFiscaleEstera statoSenzaEsenzioneFiscaleEstera = null;
            GestioneCtrlStatiSenzaEsenzioneFiscaleEstera.GetStatoSenzaEsenzioneFiscaleEstera(codiceComuneResidenza, out statoSenzaEsenzioneFiscaleEstera);

            //se lo stato estero non è presente in tabella
            if (Utility.IsResidenteEstero(codiceComuneResidenza) && statoSenzaEsenzioneFiscaleEstera == null)
            {
                if (!Utility.IsEsenzioneFiscaleEsteroAutonomi(datiPensione, codiceComuneResidenza))
                    return false;
                else
                    return true;
            }
            else // se lo stato estero è presente in tabella
                return false;
        }

        public static bool IsEsenzioneFiscaleEsteroAutonomi(GestionePensione.DatiPensione datiPensione, string codiceComuneResidenza)
        {
            bool esito = true;
            if (datiPensione != null)
            {
                string siglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();
                switch (siglaCategoria)
                {
                    case "VR":
                    case "IR":
                    case "SR":
                    case "VOART":
                    case "IOART":
                    case "SOART":
                    case "VOCOM":
                    case "IOCOM":
                    case "SOCOM":
                    case "VRS":
                    case "IRS":
                    case "SRS":
                    case "VOARTS":
                    case "IOARTS":
                    case "SOARTS":
                    case "VOCOMS":
                    case "IOCOMS":
                    case "SOCOMS":
                        if (!string.IsNullOrEmpty(codiceComuneResidenza))
                        {
                            List<DatiCtrlStatiSenzaEsenzioneEsteraAutonomi> listaStati = null;
                            GestioneCtrlStatiSenzaEsenzioneEsteraAutonomi.GetListaStatiSenzaEsenzioneEsteraAutonomi(out listaStati);
                            if (listaStati.Any(x => x.CodCatastale.Trim() == codiceComuneResidenza.Trim()))
                                esito = false;
                        }
                        break;
                    default:
                        esito = true;
                        break;
                }
            }
            return esito;
        }

        public static bool? IsEsenzioneFiscaleEsteroFromDetrazioni(GestionePensione.DatiPensione datiPensione, GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda)
        {
            if (datiPensione != null && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (datiDetrazioni != null && datiDetrazioni.DetrazioniReddito != null && datiDetrazioni.DetrazioniReddito == 2)
                    return true;
                else if (datiDetrazioni != null && datiDetrazioni.DetrazioniReddito != null && datiDetrazioni.DetrazioniReddito == 3)
                    return false;
            }
            return (bool?)null;
        }

        public static bool? IsEccezioneEsenzioneFiscaleEstero(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (datiPensione != null)
            {
                if (IsDomandaBancari(datiPensione.SiglaCategoria) && !IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    return false;
            }
            return (bool?)null;
        }

        public static bool IsEsenzioneFiscaleVittima(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni, bool isRiaperturaDomanda)
        {
            if (datiPensione == null)
                return false;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
            {
                //AGO
                if (datiBeneficioVittimeTerrorismo != null)
                {
                    if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    {
                        //NEW AGO
                        if (datiPensione.SiglaCategoria.StartsWith("S"))
                        {
                            //tutte le domande di trasformazione e ricostituzione che siano domande ai superstiti
                            if ((new List<long> { 1, 2, 3, 4 }.Contains(datiBeneficioVittimeTerrorismo.TipologiaPrestazione.GetValueOrDefault())) ||
                                (datiDetrazioni != null && datiDetrazioni.DetrazioniReddito != null && datiDetrazioni.DetrazioniReddito == 3))
                                return true;
                        }
                        else
                        {
                            //tutte le domande di trasformazione e ricostituzione che NON siano domande ai superstiti
                            if ((new List<long> { 1, 2, 3, 4 }.Contains(datiBeneficioVittimeTerrorismo.TipologiaPrestazione.GetValueOrDefault())) &&
                                (datiDetrazioni != null && datiDetrazioni.DetrazioniReddito != null && datiDetrazioni.DetrazioniReddito == 3))
                                return true;
                        }
                    }
                    else
                    {
                        //OLD AGO
                        if (new List<long> { 1, 2, 3 }.Contains(datiBeneficioVittimeTerrorismo.TipologiaPrestazione.GetValueOrDefault()))
                            return true;
                    }
                }
                else
                    return false;
            }
            else
            {
                //NON AGO
                if (datiDetrazioni != null && datiDetrazioni.DetrazioniReddito != null && datiDetrazioni.DetrazioniReddito == 3)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public static void ManageSemaforoDetrazioniPerEsenzioneFiscale(GestionePensione.DatiPensione datiPensione, GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni, byte? esenzioneFiscale,
            bool isRiaperturaDomanda, bool isVariaDetrazioni, bool isBeneficioVittimeUnderOver80)
        {
            if (isRiaperturaDomanda || (Utility.IsRicostituzione(datiPensione.Gruppo) && !isVariaDetrazioni) ||
                Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria))
                return;

            if (esenzioneFiscale.HasValue)
            {
                GestioneDetrazioniImposta.EliminaDetrazioniByIdPensione(datiPensione.Id, false);
                datiQuadroDetrazioni.TabDetrazioni = null; // non visibile
                datiQuadroDetrazioni.Tipo = 0;
            }
            else
            {
                //si passano i datiBeneficioTerrorismo a null in quanto viene già verificato che si tratta di una ricostituzione
                if (!isBeneficioVittimeUnderOver80)
                {
                    if (isVariaDetrazioni)
                        GestioneDetrazioniImposta.EliminaDetrazioniByIdPensione(datiPensione.Id, false);

                    if (datiQuadroDetrazioni.Tipo == 0 && datiQuadroDetrazioni.TabDetrazioni == null)
                    {
                        datiQuadroDetrazioni.TabDetrazioni = 0; // rosso
                        datiQuadroDetrazioni.Tipo = 2;
                    }
                }
            }

            GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, datiQuadroDetrazioni);
        }

        public static TipoSalvaguardia GetTipoSalvaguardia(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaSalvaguardia214(datiPensione))
                return TipoSalvaguardia.L214;
            else if (Utility.IsDomandaSalvaguardia122(datiPensione))
                return TipoSalvaguardia.L122;
            else if (Utility.IsDomandaSalvaguardia135(datiPensione))
                return TipoSalvaguardia.L135;
            else if (Utility.IsDomandaSalvaguardia228(datiPensione))
                return TipoSalvaguardia.L228;
            else if (Utility.IsDomandaSalvaguardia124(datiPensione))
                return TipoSalvaguardia.L124;
            else if (Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione))
                return TipoSalvaguardia.L124Art11Bis;
            else if (Utility.IsDomandaSalvaguardia147(datiPensione))
                return TipoSalvaguardia.L147;
            else if (Utility.IsDomandaUsuranti(datiPensione))
                return TipoSalvaguardia.Usuranti;
            else if (Utility.IsDomandaEsuberiPA(datiPensione))
                return TipoSalvaguardia.EsuberiPA;
            else if (Utility.IsDomandaSalvaguardia147_2014(datiPensione))
                return TipoSalvaguardia.L147_2014;
            else if (Utility.IsDomandaSalvaguardia208_2015(datiPensione))
                return TipoSalvaguardia.L208_2015;
            else if (Utility.IsDomandaSalvaguardia232_2016(datiPensione))
                return TipoSalvaguardia.L232_2016;
            else if (Utility.IsDomandaAPEPrecoci(datiPensione))
                return TipoSalvaguardia.APE_Precoci;
            else if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) ||
                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                return TipoSalvaguardia.Contributivo_Optante;
            else if (Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                return TipoSalvaguardia.L178_2020;
            else if (Utility.IsDomandaBancariPLConBonus(datiPensione))
                return TipoSalvaguardia.Bancari_Bonus;
            else
                return TipoSalvaguardia.Nessuna;
        }

        public static void GetTipoCalcoloFromDatiHost(ref GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> elencoDatiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> elencoDatiContributivi, GestioneCalcolo.DatiCalcoloRetributivoENPAL elencoDatiRetributiviENPALS,
            GestioneCalcolo.DatiCalcoloContributivoENPAL elencoDatiContributiviENPALS, GestioneEnpals.DatiEnpals datiEnpals)
        {
            if (elencoDatiContributiviENPALS != null || elencoDatiRetributiviENPALS != null || (datiEnpals != null && !datiEnpals.IsDatiEnpalsNull()))
            {
                if (datiEnpals != null && datiEnpals.ImportoPensione707.HasValue)
                {
                    // Non è contemplata la casistica di valori uguali, perchè ci è stato detto che è una casistica che non si può verificare
                    if (datiEnpals.ImportoPensione.GetValueOrDefault() < datiEnpals.ImportoPensione707.GetValueOrDefault())
                        datiPensione.TipoCalcolo = 26;
                    else if (datiEnpals.ImportoPensione.GetValueOrDefault() > datiEnpals.ImportoPensione707.GetValueOrDefault())
                        datiPensione.TipoCalcolo = 27;
                }
                else if (elencoDatiContributiviENPALS != null && !elencoDatiContributiviENPALS.IsDatiCalcoloContributivoEnpalsNull() &&
                    (elencoDatiRetributiviENPALS == null || elencoDatiRetributiviENPALS.IsDatiCalcoloRetributivoEnpalsNull()))
                    datiPensione.TipoCalcolo = 1;
                else if ((elencoDatiContributiviENPALS == null || elencoDatiContributiviENPALS.IsDatiCalcoloContributivoEnpalsNull()) &&
                    elencoDatiRetributiviENPALS != null && !elencoDatiRetributiviENPALS.IsDatiCalcoloRetributivoEnpalsNull())
                    datiPensione.TipoCalcolo = 2;
                else if (elencoDatiContributiviENPALS != null && !elencoDatiContributiviENPALS.IsDatiCalcoloContributivoEnpalsNull() &&
                    elencoDatiRetributiviENPALS != null && !elencoDatiRetributiviENPALS.IsDatiCalcoloRetributivoEnpalsNull())
                    datiPensione.TipoCalcolo = 21;
            }
            else if ((elencoDatiRetributivi != null && elencoDatiRetributivi.Count > 0) || (elencoDatiContributivi != null && elencoDatiContributivi.Count > 0))
            {
                // Se esiste una quota retributiva 'A' o 'B' e non ci sono dati contributivi
                if ((elencoDatiRetributivi != null && elencoDatiRetributivi.FindIndex(x => (x.NSettimaneQuotaA.HasValue || x.RMSQuotaA.HasValue || x.NSettimaneQuotaB.HasValue || x.RMSQuotaB.HasValue)) > -1 &&
                    elencoDatiContributivi != null && elencoDatiContributivi.FindIndex(x => (x.Montante.HasValue || x.ImportoContributivoTotale.HasValue || x.NSettimane.HasValue ||
                         x.MontanteQuotaDL214.HasValue || x.ImportoContribTotaleQuotaDL214.HasValue || x.NSettimaneQuotaDL214.HasValue)) == -1)
                    ||
                    // oppure se esiste una quota retributiva 'A' o 'B', non esiste una quota contributiva 'C' ed esiste una quota contributiva 'D'
                    (elencoDatiRetributivi != null && elencoDatiRetributivi.FindIndex(x => (x.NSettimaneQuotaA.HasValue || x.RMSQuotaA.HasValue || x.NSettimaneQuotaB.HasValue || x.RMSQuotaB.HasValue)) > -1 &&
                    elencoDatiContributivi != null && elencoDatiContributivi.FindIndex(x => (x.Montante.HasValue || x.ImportoContributivoTotale.HasValue || x.NSettimane.HasValue)) == -1 &&
                    elencoDatiContributivi.FindIndex(x => (x.MontanteQuotaDL214.HasValue || x.ImportoContribTotaleQuotaDL214.HasValue || x.NSettimaneQuotaDL214.HasValue)) > -1))
                {
                    datiPensione.TipoCalcolo = 2;    //Retributivo or Retributivo Monti
                }
                // Se esiste una quota retributiva 'A' o 'B' ed una quota contributiva 'C'
                else if (elencoDatiRetributivi != null && elencoDatiRetributivi.FindIndex(x => (x.NSettimaneQuotaA.HasValue || x.RMSQuotaA.HasValue || x.NSettimaneQuotaB.HasValue || x.RMSQuotaB.HasValue)) > -1 &&
                    (elencoDatiContributivi != null && elencoDatiContributivi.FindIndex(x => (x.Montante.HasValue || x.ImportoContributivoTotale.HasValue || x.NSettimane.HasValue)) > -1))
                {
                    datiPensione.TipoCalcolo = 21;   //Misto
                }
                // Se non esiste una quota retributiva 'A' ed esiste una quota contributiva qualsiasi
                else if (elencoDatiRetributivi != null && elencoDatiRetributivi.FindIndex(x => (x.NSettimaneQuotaA.HasValue || x.RMSQuotaA.HasValue || x.NSettimaneQuotaB.HasValue || x.RMSQuotaB.HasValue)) == -1 &&
                    (elencoDatiContributivi != null && elencoDatiContributivi.FindIndex(x => (x.Montante.HasValue || x.ImportoContributivoTotale.HasValue || x.NSettimane.HasValue ||
                    x.MontanteQuotaDL214.HasValue || x.ImportoContribTotaleQuotaDL214.HasValue || x.NSettimaneQuotaDL214.HasValue)) > -1))
                {
                    datiPensione.TipoCalcolo = 1;    //Contributivo
                }
            }
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static char GetNextLetter(char letter)
        {
            try
            {
                if (letter == 'Z')
                    return 'A';
                else
                    return (char)(((int)letter) + 1);
            }
            catch (Exception)
            {
                return 'A';
            }
        }

        /// <summary>
        /// Verifica se tutte le Properties dell'oggetto passato come parametro sono Null.
        /// </summary>
        /// <returns> </returns>
        public static bool PropertiesAreAllNull(object value)
        {
            foreach (PropertyInfo objProp in value.GetType().GetProperties())
            {
                if (objProp.CanRead)
                {
                    object val = objProp.GetValue(value, null);
                    if (val != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica se la domanda è una pensione indiretta (gruppo = 0003, prodotto = 0022)
        /// </summary>
        /// <returns></returns>
        public static bool IsDomandaPensioneIndiretta(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneIndiretta(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0422" && datiPensione.Tipo == "0026")
                return true;

            return false;
        }

        public static bool IsDomandaPensioneIndirettaOrRicostituzione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaPensioneIndiretta(datiPensione) ||
                (Utility.IsRicostituzione(datiPensione.Gruppo) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S") &&
                 datiDanteCausa != null && datiDanteCausa.ProvenienzaPensione == 0))
                return true;

            return false;
        }

        public static bool IsDomandaPensioneReversibilitaOrRicostituzione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaReversibilita(datiPensione) ||
                (Utility.IsRicostituzione(datiPensione.Gruppo) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("S") &&
                 datiDanteCausa != null && (datiDanteCausa.ProvenienzaPensione == 1 || datiDanteCausa.ProvenienzaPensione == 2)))
                return true;

            return false;
        }


        /// <summary>
        /// Verifica se la domanda è una pensione ai superstiti  o una sua ricostituzione ( SiglaCategoria inizia per 'S')
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaPensioneSuperstitiOrRicostituzione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.SiglaCategoria == null)
                return false;
            if (datiPensione.SiglaCategoria.ToUpperInvariant().StartsWith("S") || IsDomandaSuperstiti_PMO(datiPensione))
                return true;
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una pensione indiretta supplementare (gruppo = 0003, prodotto = 0022, tipo = 0009) oppure
        /// una sua ricostituzione (gruppo = 0031, naturaPensione = "5**", siglaCategoria = "S*")
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsPensioneIndirettaSupplementareOrRicostituzione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0009") ||
                (datiPensione.Gruppo == "0031" && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.StartsWith("5") && !string.IsNullOrEmpty(datiPensione.SiglaCategoria) &&
                datiPensione.SiglaCategoria.StartsWith("S")))
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una pensione di inabilità
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaPensioneInabilita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una pensione di inabilità
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaPensioneOrdinariaDiInabilita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0001")
                return true;

            return false;
        }

        public static bool IsDomandaIobancInabilita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.SiglaCategoria == "IOBANC")
            {
                if (datiPensione.NaturaPensione != null)
                {
                    if (datiPensione.NaturaPensione.StartsWith("3") || datiPensione.NaturaPensione.StartsWith("4"))
                        return true;
                }
                else
                    return IsDomandaPensioneInabilita(datiPensione);
            }
            return false;
        }

        public static bool IsBloccoLavorazione(Utility.TipoAppartenenza? tipoAppartenenza, string gruppo, bool isRiaperturaDomanda)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamico);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamico);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            string nomeControlloDinamico = "BloccoLavorazione" + tipoAppartenenza.GetValueOrDefault().ToString();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            //Per le Ric e Trf rinnovate non effettuo il blocco
            if (!((Utility.IsRicostituzione(gruppo) || isRiaperturaDomanda) && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                 && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno)))
            {
                if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                    return true;
            }
            return false;
        }

        public static bool IsBloccoLavorazioneManuali_Opzione(GestionePensione.DatiPensione datiPensione)
        {
            if (!IsDomandaAutomatica(datiPensione) && IsDomandaTipoContributivo(datiPensione, null, true) && !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Acquisizione.ACQ_OPZ_CONTR))
            {
                var isRicOrTfr = IsRicostituzioneOrRiapertura(datiPensione, IsRiaperturaDomanda(datiPensione.Id));
                GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                string nomeControlloDinamico = isRicOrTfr ? "BloccoRICManuali_Opzione" :
                    (IsDomandaTipoContributivo(datiPensione, true, true) ? "BloccoPLManualiAnz_Opzione" : "BloccoPLManualiVec_Opzione");
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
                if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è una pensione di inabilità o una sua ricostituzione (utilizzabile solo per FS)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="codiceSpecifico"></param>
        /// <returns></returns>
        public static bool IsDomandaPensioneInabilitaOrRicostituzioneFS(GestionePensione.DatiPensione datiPensione, char? codiceSpecifico)
        {
            if ((datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012") || codiceSpecifico.GetValueOrDefault() == 'Q')
                return true;

            return false;
        }

        public static bool IsDomandaPensioneInabilitaOrRicostituzioneAGO_CI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012")
                return true;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue &&
                (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI) &&
                datiPensione.SiglaCategoria.StartsWith("I") && !string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                (datiPensione.NaturaPensione.StartsWith("3") || datiPensione.NaturaPensione.StartsWith("4")))
                return true;

            return false;
        }

        public static bool IsPensioneInabilitaGenericaPost2012(GestionePensione.DatiPensione datiPensione)
        {
            if ((datiPensione.NaturaPensione.StartsWith("3") || datiPensione.NaturaPensione.StartsWith("4")) &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)))
                return true;
            return false;
        }

        public static bool IsRiaperturaDomanda(string fase)
        {
            if (!string.IsNullOrEmpty(fase) && (fase == "0060" || fase == "0062" || fase == "0063"))
                return true;

            return false;
        }

        public static bool IsRiaperturaDomanda(long idPensione)
        {
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(idPensione, out datiLavorazione);

            if (datiLavorazione != null && (datiLavorazione.CodFase == "0060" || datiLavorazione.CodFase == "0062" || datiLavorazione.CodFase == "0063"))
                return true;

            return false;
        }


        public static bool? IsDomandaRipristino(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;

            TipoDomanda tipoDomanda = GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (tipoDomanda == TipoDomanda.Ripristino || tipoDomanda == TipoDomanda.RipristinoSuperstiti)
                return true;

            return false;
        }

        public static bool? IsDomandaRiliquidazione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return null;

            TipoDomanda tipoDomanda = GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (tipoDomanda == TipoDomanda.Riliquidazione || tipoDomanda == TipoDomanda.RiliquidazioneSuperstiti)
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoOrRiliquidazione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            TipoDomanda tipoDomanda = GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (IsDomandaRipristino(datiPensione).GetValueOrDefault() || IsDomandaRiliquidazione(datiPensione).GetValueOrDefault())
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoOrRiliquidazioneSuperstiti(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && (datiPensione.Prodotto == "0421" || datiPensione.Prodotto == "0422"))
                return true;
            return false;
        }

        public static bool IsDomandaRiliquidazioneAnzianitaAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0122" && datiPensione.Tipo == "0021")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneVecchiaiaOAnzianita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0122")
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0122" && datiPensione.Tipo == "0022")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoAnzianitaAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0121" && datiPensione.Tipo == "0021")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0121" && datiPensione.Tipo == "0022")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoAssegnoInvalidita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0321" && datiPensione.Tipo == "0023")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoInvalidita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0321" && datiPensione.Tipo == "0024")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoInabilita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0321" && datiPensione.Tipo == "0025")
                return true;

            return false;
        }

        public static bool IsDomandaRipristinoIndiretta(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0421" && datiPensione.Tipo == "0026")
                return true;

            return false;
        }

        public static bool IsDomandaSperimentaleDonna(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0050")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica che la domanda sia di tipo Sperimentale Donna o una sua Ricostituzione
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaSperimentaleDonnaOrRicostituzione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0050") ||
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "O"))
                return true;

            return false;
        }

        public static bool GestioneRiduzioneRetributivaEnabled(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi, List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.CI && !IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
                    return false;
                if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 2, 1)) &&
                    !Utility.DataStrettamenteSuccessivaA(new DateTime(2014, 12, 01), datiPensione.DecorrenzaOriginaria.Value))
                {
                    if (listaDatiContributivi != null && listaDatiContributivi.Count > 0 && (listaDatiRetributivi == null || listaDatiRetributivi.Count == 0))
                        return false;
                }
            }
            else if (IsDomandaTipoContributivo(datiPensione, null, null) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione)) //ENG - Memo 166/2023
                return false;
            else
            {
                // L'abilitazione viene gestita lato client (Javascript)
                if (Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria))
                    return true;
                if ((datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)) &&
                    datiPensione.DataPerfezionamentoRequisiti.HasValue && !Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2017, 12, 31))))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Il metodo restitusce false se codiceSindacato è diverso da 0. Codice 0 significa che non 
        /// è stato inserito nessun sindacato.
        /// </summary>
        public static bool IsSindacatoPresente(string CodiceSindacato)
        {
            if (CodiceSindacato != null && CodiceSindacato != string.Empty && CodiceSindacato.Trim() != "0")
                return true;
            return false;
        }

        public static bool IsDomandaINPDAI(string siglaCategoria)
        {
            if (siglaCategoria != null)
            {
                string siglaCategoriaNormalized = siglaCategoria.Trim().ToUpperInvariant();
                if (siglaCategoriaNormalized == "VDAI" || siglaCategoriaNormalized == "SDAI" || siglaCategoriaNormalized == "IDAI")
                    return true;
            }
            return false;
        }

        public static CategoriaFondoPI? GetCategoriaFondoPI(TipoAppartenenza? tipoAppartenenza, string siglaCategoria)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, siglaCategoria);
            if (tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.PI || tipoFondo.Value == Utility.TipoFondo.PL))
            {
                string categoria = string.Empty;
                categoria = !String.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().Length == 4 ? siglaCategoria.ToUpperInvariant().Substring(3, 1) : string.Empty;
                if (!String.IsNullOrEmpty(categoria))
                {
                    switch (categoria.ToUpperInvariant())
                    {
                        case "A":
                            return CategoriaFondoPI.A;
                        case "B":
                            return CategoriaFondoPI.B;
                        case "C":
                            return CategoriaFondoPI.C;
                        case "D":
                            return CategoriaFondoPI.D;
                        case "E":
                            return CategoriaFondoPI.E;
                        case "F":
                            return CategoriaFondoPI.F;
                        case "G":
                            return CategoriaFondoPI.G;
                        case "H":
                            return CategoriaFondoPI.H;
                        case "I":
                            return CategoriaFondoPI.I;
                        case "J":
                            return CategoriaFondoPI.J;
                        case "L":
                            return CategoriaFondoPI.L;
                        case "M":
                            return CategoriaFondoPI.M;
                        case "N":
                            return CategoriaFondoPI.N;
                        case "O":
                            return CategoriaFondoPI.O;
                        case "P":
                            return CategoriaFondoPI.P;
                        case "Q":
                            return CategoriaFondoPI.Q;
                        case "R":
                            return CategoriaFondoPI.R;
                        case "S":
                            return CategoriaFondoPI.S;
                        case "T":
                            return CategoriaFondoPI.T;
                        case "U":
                            return CategoriaFondoPI.U;
                        case "1":
                            return CategoriaFondoPI.Uno;
                        case "V":
                            return CategoriaFondoPI.V;
                        case "W":
                            return CategoriaFondoPI.W;
                        case "X":
                            return CategoriaFondoPI.X;
                        case "Y":
                            return CategoriaFondoPI.Y;
                        case "Z":
                            return CategoriaFondoPI.Z;
                        default:
                            if(tipoFondo.Value == Utility.TipoFondo.PL)
                                return CategoriaFondoPI.A;
                            else
                                return null;
                    }
                }
                if (tipoFondo.Value == Utility.TipoFondo.PL)
                    return CategoriaFondoPI.A;
            }
            return null;
        }

        public static char? GetCharCategoriaFondoPI(TipoAppartenenza? tipoAppartenenza, string siglaCategoria)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, siglaCategoria);
            if (tipoFondo.HasValue && tipoFondo.Value == Utility.TipoFondo.PI)
            {
                string categoria = string.Empty;
                categoria = !String.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().Length == 4 ? siglaCategoria.ToUpperInvariant().Substring(3, 1) : string.Empty;
                if (!String.IsNullOrEmpty(categoria))
                {
                    switch (categoria.ToUpperInvariant())
                    {
                        case "A":
                            return 'A';
                        case "B":
                            return 'B';
                        case "C":
                            return 'C';
                        case "D":
                            return 'D';
                        case "E":
                            return 'E';
                        case "F":
                            return 'F';
                        case "G":
                            return 'G';
                        case "H":
                            return 'H';
                        case "I":
                            return 'I';
                        case "J":
                            return 'J';
                        case "L":
                            return 'L';
                        case "M":
                            return 'M';
                        case "N":
                            return 'N';
                        case "O":
                            return 'O';
                        case "P":
                            return 'P';
                        case "Q":
                            return 'Q';
                        case "R":
                            return 'R';
                        case "S":
                            return 'S';
                        case "T":
                            return 'T';
                        case "U":
                            return 'U';
                        case "1":
                            return '1';
                        case "V":
                            return 'V';
                        case "W":
                            return 'W';
                        case "X":
                            return 'X';
                        case "Y":
                            return 'Y';
                        case "Z":
                            return 'Z';
                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        public static char? GetCharCategoriaFondoPI(CategoriaFondoPI? categoriaFondoPI)
        {
            if (categoriaFondoPI.HasValue)
            {
                switch (categoriaFondoPI.Value)
                {
                    case CategoriaFondoPI.A:
                        return 'A';
                    case CategoriaFondoPI.B:
                        return 'B';
                    case CategoriaFondoPI.C:
                        return 'C';
                    case CategoriaFondoPI.D:
                        return 'D';
                    case CategoriaFondoPI.E:
                        return 'E';
                    case CategoriaFondoPI.F:
                        return 'F';
                    case CategoriaFondoPI.G:
                        return 'G';
                    case CategoriaFondoPI.H:
                        return 'H';
                    case CategoriaFondoPI.I:
                        return 'I';
                    case CategoriaFondoPI.J:
                        return 'J';
                    case CategoriaFondoPI.L:
                        return 'L';
                    case CategoriaFondoPI.M:
                        return 'M';
                    case CategoriaFondoPI.N:
                        return 'N';
                    case CategoriaFondoPI.O:
                        return 'O';
                    case CategoriaFondoPI.P:
                        return 'P';
                    case CategoriaFondoPI.Q:
                        return 'Q';
                    case CategoriaFondoPI.R:
                        return 'R';
                    case CategoriaFondoPI.S:
                        return 'S';
                    case CategoriaFondoPI.T:
                        return 'T';
                    case CategoriaFondoPI.U:
                        return 'U';
                    case CategoriaFondoPI.Uno:
                        return '1';
                    case CategoriaFondoPI.V:
                        return 'V';
                    case CategoriaFondoPI.W:
                        return 'W';
                    case CategoriaFondoPI.X:
                        return 'X';
                    case CategoriaFondoPI.Y:
                        return 'Y';
                    case CategoriaFondoPI.Z:
                        return 'Z';
                    default:
                        return null;
                }
            }

            return null;
        }

        public static bool IsPIAPIBAnte99(TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            CategoriaFondoPI? categoriaFondoPI = GetCategoriaFondoPI(tipoAppartenenza, datiPensione.SiglaCategoria);
            if (categoriaFondoPI == CategoriaFondoPI.A || categoriaFondoPI == CategoriaFondoPI.B)
            {
                DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
                if (decorrenza.HasValue && DataSuccessivaA(new DateTime(1999, 01, 01), decorrenza.Value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Arrotonda per eccesso troncando value alla cifra decimale specificato come secondo parametro.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cifreDecimali"></param>
        /// <returns></returns>
        public static int CeilingAtDecimalDigit(decimal value, int cifreDecimali)
        {
            return (int)Math.Ceiling(Math.Truncate(value * (10 ^ cifreDecimali)) / (10 ^ cifreDecimali));
        }

        /// <summary>
        /// Verifica la regex su tutta l'oggetto passato
        /// </summary>
        /// <param name="obj">Oggetto da validare</param>
        /// <param name="pattern">Pattern per la validazione di stringhe</param>
        /// <param name="cifreIntere">Numero cifre della parte intera per un valore numerico</param>
        /// <param name="cifreDecimali">Numero cifre della parte decimale per un valore numerico</param>
        /// <returns>Ritorna true se l'oggetto passato è null oppure se valida la Regex</returns>
        public static bool VerificaRegex(object obj, string pattern = "^$", int? cifreIntere = null, int? cifreDecimali = null)
        {
            if (obj == null)
                return true;

            Type type = obj.GetType();
            Match m = null;
            string regex = "^$";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                    if (cifreIntere != null && cifreDecimali != null && cifreDecimali > 0)
                        regex = @"^\d{0," + cifreIntere + @"}(,\d{1," + cifreDecimali + "})?$";
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    if (cifreIntere != null)
                        regex = @"^\d{0," + cifreIntere + "}?$";
                    break;
                case TypeCode.String:
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        StringBuilder finalPattern = new StringBuilder();
                        string[] str = pattern.Split('|');
                        foreach (string s in str)
                        {
                            string app = s;
                            if (!s.StartsWith("^"))
                                app = "^" + app;
                            if (!s.EndsWith("$"))
                                app = app + "$";
                            finalPattern.Append(app + "|");
                        }
                        finalPattern.Remove(finalPattern.Length - 1, 1);

                        regex = finalPattern.ToString();
                    }
                    break;
            }

            m = Regex.Match(obj.ToString(), regex);
            if (m.Success)
                return true;
            else
                return false;
        }

        public static bool IsDomandaCumulo(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "VOCUM" || siglaCategoria.Trim().ToUpperInvariant() == "IOCUM" || siglaCategoria.Trim().ToUpperInvariant() == "SOCUM"))
                return true;

            return false;
        }

        public static bool IsRicostituzioneCumuloProgressiva(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaVOCUM(datiPensione.SiglaCategoria) && datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0107" && datiPensione.Tipo == "0177")
                return true;
            return false;
        }

        public static bool IsDomandaSupplementoCumulo(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaVOCUM(datiPensione.SiglaCategoria) && datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0102" && datiPensione.Tipo == "0001")
                return true;
            return false;
        }

        public static bool IsDomandaVOCUM(string siglaCategoria)
        {
            if (string.IsNullOrEmpty(siglaCategoria))
                return false;
            if (siglaCategoria.Trim().ToUpperInvariant() == "VOCUM")
                return true;
            return false;
        }

        public static bool IsDomandaIOCUM(string siglaCategoria)
        {
            if (string.IsNullOrEmpty(siglaCategoria))
                return false;
            if (siglaCategoria.Trim().ToUpperInvariant() == "IOCUM")
                return true;
            return false;
        }

        public static bool IsDomandaSOCUM(string siglaCategoria)
        {
            if (string.IsNullOrEmpty(siglaCategoria))
                return false;
            if (siglaCategoria.Trim().ToUpperInvariant() == "SOCUM")
                return true;
            return false;
        }

        public static bool IsDomandaVOCUM_SOCUM(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "VOCUM" || siglaCategoria.Trim().ToUpperInvariant() == "SOCUM"))
                return true;

            return false;
        }

        public static bool IsDomandaAnticipataCumuloL232(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")
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

        public static bool IsDomandaVOTOT_SOTOT(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim().ToUpperInvariant() == "VOTOT" || siglaCategoria.Trim().ToUpperInvariant() == "SOTOT"))
                return true;

            return false;
        }

        public static bool IsDomandaIOCUM_SOCUM_IOTOT_SOTOT(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (Utility.IsDomandaSOCUM(siglaCategoria) || Utility.IsDomandaIOCUM(siglaCategoria) || Utility.IsDomandaIOTOT(siglaCategoria) || Utility.IsDomandaSOTOT(siglaCategoria)))
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

        public static void GetListaSigleCategoriePerTipoApp(out List<string> elencoSigleCategoria, string tipoApp)
        {
            elencoSigleCategoria = null;

            List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione = null;
            GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensione);

            /////////////////////////////////////////////////////////////////////
            //Filtro la lista in base alla TipologiaAppartenenzaOperatore
            elencoCategoriePensione = elencoCategoriePensione.FindAll(x => x.AppartenenzaCatPensione == tipoApp);
            /////////////////////////////////////////////////////////////////////

            if (elencoCategoriePensione != null && elencoCategoriePensione.Count > 0)
            {
                List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensioneFS = elencoCategoriePensione.FindAll(x => x.TipoCatPensione == 'C');
                if (elencoCategoriePensioneFS != null && elencoCategoriePensioneFS.Count > 0)
                {
                    foreach (GestioneDecodifica.CategoriaPensione cpFS in elencoCategoriePensioneFS)
                    {
                        if (cpFS.SiglaCatPensione.Trim().ToUpperInvariant() == "PI")
                        {
                            List<char> categoriePI = new List<char> { 'A', '1', 'Y', 'U', 'V' };
                            foreach (char categoria in categoriePI)
                            {
                                string siglaCatPensione = cpFS.SiglaCatPensione.Trim().ToUpperInvariant() + categoria.ToString();

                                GestioneDecodifica.CategoriaPensione decCatPens = new GestioneDecodifica.CategoriaPensione();
                                decCatPens.TipoCatPensione = 'V';
                                decCatPens.SiglaCatPensione = "V" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                decCatPens.AppartenenzaCatPensione = tipoApp;
                                elencoCategoriePensione.Add(decCatPens);

                                decCatPens = new GestioneDecodifica.CategoriaPensione();
                                decCatPens.TipoCatPensione = 'I';
                                decCatPens.SiglaCatPensione = "I" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                decCatPens.AppartenenzaCatPensione = tipoApp;
                                elencoCategoriePensione.Add(decCatPens);

                                decCatPens = new GestioneDecodifica.CategoriaPensione();
                                decCatPens.TipoCatPensione = 'S';
                                decCatPens.SiglaCatPensione = "S" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                decCatPens.AppartenenzaCatPensione = tipoApp;
                                elencoCategoriePensione.Add(decCatPens);
                            }
                        }
                        else
                        {
                            GestioneDecodifica.CategoriaPensione decCatPens = new GestioneDecodifica.CategoriaPensione();
                            decCatPens.TipoCatPensione = 'V';
                            decCatPens.SiglaCatPensione = "V" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            decCatPens.AppartenenzaCatPensione = tipoApp;
                            elencoCategoriePensione.Add(decCatPens);

                            decCatPens = new GestioneDecodifica.CategoriaPensione();
                            decCatPens.TipoCatPensione = 'I';
                            decCatPens.SiglaCatPensione = "I" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            decCatPens.AppartenenzaCatPensione = tipoApp;
                            elencoCategoriePensione.Add(decCatPens);

                            decCatPens = new GestioneDecodifica.CategoriaPensione();
                            decCatPens.TipoCatPensione = 'S';
                            decCatPens.SiglaCatPensione = "S" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            decCatPens.AppartenenzaCatPensione = tipoApp;
                            elencoCategoriePensione.Add(decCatPens);
                        }
                    }

                    elencoCategoriePensione.RemoveAll(x => x.TipoCatPensione == 'C');
                }

                if (elencoCategoriePensione.Count > 0)
                {
                    elencoSigleCategoria = new List<string>();
                    foreach (GestioneDecodifica.CategoriaPensione cp in elencoCategoriePensione)
                    {
                        elencoSigleCategoria.Add(cp.SiglaCatPensione.Trim());
                    }

                    elencoSigleCategoria = elencoSigleCategoria.OrderBy(x => x).ToList<string>();
                }
            }
        }

        public static List<string> GetListaSigleCategorieINPDAP()
        {
            string catINPDAP = "VOCTPS;IOCTPS;SOCTPS;VOCPDEL;IOCPDEL;SOCPDEL;VOCPI;IOCPI;SOCPI;VOCPS;IOCPS;SOCPS;VOCPUG;IOCPUG;SOCPUG";
            return catINPDAP.Split(';').ToList();
        }

        public static bool IsDomandaConNuovaGestioneDatiFondoFSPT(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = null;
            GestioneQuadri.GetQuadroDatiFondoByDatiPensione(datiPensione, out datiQuadroDatiFondo);
            TipoFondo? TF = GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);

            if (datiQuadroDatiFondo != null && datiQuadroDatiFondo.Tipo != 0 && TF != Utility.TipoFondo.DZ)
                return true;

            return false;
        }

        public static bool IsDomandaAUT(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (string.IsNullOrEmpty(datiPensione.SiglaCategoria))
                return false;

            if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOAUT" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOAUT" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOAUT")
                return true;

            return false;
        }

        public static bool IsDomandaVOAUT_IOAUT(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOAUT" || categoria.Trim().ToUpperInvariant() == "IOAUT")
                return true;
            return false;
        }

        public static bool IsDomandaCTPS(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant().EndsWith("CTPS"))
                return true;
            return false;
        }


        public static bool IsDomandaVOAUT_SOAUT(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOAUT" || categoria.Trim().ToUpperInvariant() == "SOAUT")
                return true;
            return false;
        }

        public static bool IsDomandaVOAUT(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOAUT")
                return true;
            return false;
        }

        public static bool IsDomandaSOAUT(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "SOAUT")
                return true;
            return false;
        }

        public static bool IsDomandaVOAUT_IOAUT_SOAUT(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOAUT" || categoria.Trim().ToUpperInvariant() == "IOAUT" || categoria.Trim().ToUpperInvariant() == "SOAUT")
                return true;
            return false;
        }

        public static bool IsDomandaSOAUT_Supplementare(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (datiPensione == null)
                return false;
            if (IsDomandaSOAUT(datiPensione.SiglaCategoria))
            {
                if (IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                {
                    if (datiPensione.NaturaPensione.Substring(0, 1) == "5")
                        return true;
                }
                else
                {
                    if (datiPensione.Gruppo.Equals("0003") && datiPensione.Tipo.Equals("0009") && (datiPensione.Prodotto.Equals("0021") || datiPensione.Prodotto.Equals("0022")))
                        return true;
                }
            }
            return false;
        }

        public static bool IsDomandaAGOTipoContributivoFiltroERI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null || string.IsNullOrEmpty(datiPensione.SiglaCategoria))
                return false;

            if ((datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VO" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VR" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOART" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOCOM" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VDAI") &&
                datiPensione.GetFiltro() == "ERI" && IsDomandaTipoContributivo(datiPensione, null, null))
                return true;

            return false;
        }

        public static bool IsDomandaVOAUTContributivoOpzioneFiltroERI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null || string.IsNullOrEmpty(datiPensione.SiglaCategoria))
                return false;

            if (IsDomandaVOAUT(datiPensione.SiglaCategoria) && IsDomandaTipoContributivo(datiPensione, null, false) && datiPensione.GetFiltro() == "ERI")
                return true;
            return false;
        }

        public static bool IsDomandaIndennitaUnaTantum_AGO(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            TipoDomanda? tipoDomanda = GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza != null && tipoAppartenenza == TipoAppartenenza.AGO && tipoDomanda != null && tipoDomanda == TipoDomanda.Superstiti && datiPensione.Prodotto == "0025")
                return true;

            return false;
        }

        public static bool IsDomandaSPED(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (string.IsNullOrEmpty(datiPensione.SiglaCategoria))
                return false;

            if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOSPED" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOSPED" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOSPED")
                return true;

            return false;
        }

        public static bool IsDomandaSPED(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOSPED" || categoria.Trim().ToUpperInvariant() == "SOSPED" || categoria.Trim().ToUpperInvariant() == "IOSPED")
                return true;
            return false;
        }

        public static bool IsDomandaSOSPED(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "SOSPED")
                return true;
            return false;
        }

        public static bool IsDomandaVOSPED(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOSPED")
                return true;
            return false;
        }

        public static bool IsDomandaIOSPED(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "IOSPED")
                return true;
            return false;
        }

        public static bool IsDomandaAPESociale(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "APE")
                return true;
            return false;
        }

        public static bool IsDomandaAPEPrecoci(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0051") ||
                Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.APEPrecoci)
                return true;

            return false;
        }

        public static bool IsDomandaRequisitoAnticipatoArt1(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (IsDomandaAnzianitaRequisitoAnticipatoArt1(datiPensione) || IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                IsDomandaAnzianitaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione) || IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione))
                return true;
            return false;
        }

        public static bool IsDomandaAnzianitaRequisitoAnticipatoArt1(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0172")
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaRequisitoAnticipatoArt1(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0172")
                return true;
            return false;
        }

        public static bool IsDomandaAnzianitaRequisitoAnticipatoArt1OpzioneContributivo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0173")
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0173") ||
                datiPensione.IdTipoPLPerRIC.GetValueOrDefault() == (byte)Utility.TipoPLPerRIC.GravosiUsurantiConOpzione)
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaRequisitoAnticipatoArt1_NoCheck(GestionePensione.DatiPensione datiPensione, DateTime? dataPerfezionamentoRequisitiUI)
        {
            if (datiPensione == null)
                return false;
            if ((IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione)) && dataPerfezionamentoRequisitiUI.HasValue &&
                DataSuccessivaA(dataPerfezionamentoRequisitiUI.Value, new DateTime(2019, 1, 1)) && !DataStrettamenteSuccessivaA(dataPerfezionamentoRequisitiUI.Value, new DateTime(2022, 12, 31)))
                return true;
            return false;
        }

        public static bool IsForceCtrlRequisitoEtaPerAUT(GestionePensione.DatiPensione datiPensione, DateTime? dataAssunzioneCarico)
        {
            if (datiPensione == null)
                return false;
            //Se la domanda è una Vecchiaia o Ric vecchiaia AUT
            if (Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensione, null)
                && Utility.IsDomandaAUT(datiPensione) && !(dataAssunzioneCarico.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(2021, 03, 01), dataAssunzioneCarico.Value)))
                return true;
            //Se la domanda è una VOAUT con C1N = 5
            if (Utility.IsDomandaVOAUT(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                datiPensione.NaturaPensione.StartsWith("5") && !(dataAssunzioneCarico.HasValue && Utility.DataStrettamenteSuccessivaA(new DateTime(2021, 03, 01), dataAssunzioneCarico.Value)))
                return true;
            return false;
        }

        public static bool IsForceCtrlRequisitoEtaPerPescatori(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiPensione datiPensioneDB)
        {
            if (datiPensione == null)
                return false;
            //Se la domanda è una Vecchiaia o Ric vecchiaia VOP con filtro diverso da L80
            if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)) && Utility.IsPensioneVecchiaiaOrRicostituzione(datiPensioneDB, null)
                && !Utility.IsDomandaPescatoriFiltroL80(datiPensioneDB))
                return true;

            return false;
        }

        public static bool IsDomandaESOAMB(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "ESOAMB")
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla ESOAMB con il filtro L26 (Importo Extra-Calcolo).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaESOAMB_L26(GestionePensione.DatiPensione datiPensione)
        {
            if (!IsDomandaESOAMB(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "L26";
        }

        public static bool IsDomandaVecchiaiaESOAMB(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOAMB(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0038")
                return true;
            return false;
        }

        public static bool IsDomandaAnticipataESOAMB(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOAMB(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0039")
                return true;
            return false;
        }

        public static bool IsDomandaQuota100ESOAMB(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOAMB(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0054")
                return true;
            return false;
        }

        public static bool IsDomandaESOTEL(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "ESOTEL")
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla ESOTEL con il filtro L26 (Importo Extra-Calcolo).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaESOTEL_L26(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "L26";
        }

        public static bool IsDomandaVecchiaiaESOTEL(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOTEL(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0038")
                return true;
            return false;
        }

        public static bool IsDomandaAnticipataESOTEL(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOTEL(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0039")
                return true;
            return false;
        }

        public static bool IsDomandaQuota100ESOTEL(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESOTEL(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0054")
                return true;
            return false;
        }

        public static bool IsDomandaVESO33(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VESO33")
                return true;
            return false;
        }

        public static bool IsDomandaQuota100VESO33(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && IsDomandaVESO33(datiPensione.SiglaCategoria) &&
                (datiPensione.Tipo == "0054" || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.Quota100))
                return true;
            return false;
        }

        public static bool IsDomandaVESO92(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VESO92")
                return true;
            return false;
        }

        public static bool IsDomandaVESO29(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VESO29")
                return true;
            return false;
        }

        public static bool IsDomandaAnticipataVESO29(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0039")
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaVESO29(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0038")
                return true;
            return false;
        }

        public static bool IsDomandaQuota100VESO29(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && IsDomandaVESO29(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0054")
                return true;
            return false;
        }

        public static bool IsDomandaVOCOOP(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VOCOOP")
                return true;
            return false;
        }

        public static bool IsDomandaCOOP28(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "COOP28")
                return true;
            return false;
        }

        public static bool IsDomandaVOCOOP_COOP28(string categoria)
        {
            if (IsDomandaVOCOOP(categoria) || IsDomandaCOOP28(categoria))
                return true;
            return false;
        }

        public static bool IsDomandaQuota100COOP28(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && IsDomandaCOOP28(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0054")
                return true;
            return false;
        }

        public static bool IsDomandaVOESO(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VOESO")
                return true;
            return false;
        }

        public static bool IsDomandaVOCRED(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "VOCRED")
                return true;
            return false;
        }

        public static bool IsDomandaCRED27(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "CRED27")
                return true;
            return false;
        }

        public static bool IsDomandaVOCRED_CRED27(string categoria)
        {
            if (IsDomandaVOCRED(categoria) || IsDomandaCRED27(categoria))
                return true;
            return false;
        }

        public static bool IsDomandaQuota100CRED27(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && IsDomandaCRED27(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0054")
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla VESO92 con il filtro L92 (Importo Extra-Calcolo).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaVESO92_L92(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaVESO92(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "L92";
        }

        public static bool IsDomandaVESO92WithGP2BB05(string categoria, string gruppo, string GP2BB05)
        {
            if (IsRicostituzione(gruppo) && IsDomandaVESO92(categoria) && (GP2BB05 == "L1" || GP2BB05 == "E"))
                return true;
            return false;
        }
        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla VESO92 con il filtro AGO .
        /// </summary>
        public static bool IsDomandaVESO92_AGO(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaVESO92(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "AGO";
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla VOCOOP con il filtro L92 (Importo Extra-Calcolo).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaVOCOOP_L92(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "L92";
        }

        public static bool IsDomandaVESO29WithGP2BB05(string categoria, string gruppo, string GP2BB05)
        {
            if (IsRicostituzione(gruppo) && IsDomandaVESO29(categoria) && (GP2BB05 == "L1" || GP2BB05 == "E"))
                return true;
            return false;
        }

        //due fuzioni separate per VESO29/VOESO e VESO92 perchè per queste ultime bisogna abbinare la verifica della lista aziende
        public static bool IsDomandaIsoPensioneRicWithScadenzaAssegnoGGMMAAAA(string categoria, string gruppo, bool? IsScadenzaAssegnoConGiorno)
        {
            if (IsRicostituzione(gruppo) && (IsDomandaVESO29(categoria) || IsDomandaVOESO(categoria)) && IsScadenzaAssegnoConGiorno.GetValueOrDefault())
                return true;
            return false;
        }

        public static bool IsDomandaVESO92RicWithScadenzaAssegnoGGMMAAAA(string categoria, string gruppo, bool? IsScadenzaAssegnoConGiorno)
        {
            if (IsRicostituzione(gruppo) && IsDomandaVESO92(categoria) && IsScadenzaAssegnoConGiorno.GetValueOrDefault())
                return true;
            return false;
        }

        public static bool IsIsoPensioneRicWithGP2BB05(string categoria, string gruppo, string GP2BB05)
        {
            if (IsRicostituzione(gruppo) && (IsDomandaVESO29(categoria) || IsDomandaVOESO(categoria) || IsDomandaVESO92(categoria)) && (GP2BB05 == "L1" || GP2BB05 == "E"))
                return true;
            return false;
        }
        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla VOESO con il filtro L92 (Importo Extra-Calcolo).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaVOESO_L92(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaVOESO(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "L92";
        }

        public static bool IsDomandaVOESOWithGP2BB05(string categoria, string gruppo, string GP2BB05)
        {
            if (IsRicostituzione(gruppo) && IsDomandaVOESO(categoria) && (GP2BB05 == "L1" || GP2BB05 == "E"))
                return true;
            return false;
        }

        public static bool IsDomandaVOESOFerrovieDelloStatoRicConFiltro(GestionePensione.DatiPensione datiPensione, bool isRiapertura, string GP2BB05, string codiceBancaEsodati)
        {
            if (IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && IsDomandaVOESO(datiPensione.SiglaCategoria) &&
                !string.IsNullOrEmpty(codiceBancaEsodati) && Convert.ToInt32(codiceBancaEsodati) >= 601 && Convert.ToInt32(codiceBancaEsodati) <= 799 && GP2BB05 != "L1")
            {
                return true;
            }

            return false;
        }

        public static bool IsDomandaVOESOFerrovieDelloStatoRic(GestionePensione.DatiPensione datiPensione, bool isRiapertura, string codiceBancaEsodati)
        {
            if (IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && IsDomandaVOESO(datiPensione.SiglaCategoria) &&
                !string.IsNullOrEmpty(codiceBancaEsodati) && Convert.ToInt32(codiceBancaEsodati) >= 601 && Convert.ToInt32(codiceBancaEsodati) <= 799)
            {
                return true;
            }

            return false;
        }

        public static bool IsDomandaVOESOErarialiRic(GestionePensione.DatiPensione datiPensione, bool isRiapertura, string codiceBancaEsodati)
        {
            if (IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && IsDomandaVOESO(datiPensione.SiglaCategoria) &&
                !string.IsNullOrEmpty(codiceBancaEsodati) && Convert.ToInt32(codiceBancaEsodati) >= 900 && Convert.ToInt32(codiceBancaEsodati) <= 1000)
            {
                return true;
            }

            return false;
        }

        public static bool IsVOESORicErarialiOrFerrovie(GestionePensione.DatiPensione datiPensione, bool isRiapertura, string codiceBancaEsodati)
        {
            if (Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) && IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && !string.IsNullOrEmpty(codiceBancaEsodati) &&
                ((Convert.ToInt32(codiceBancaEsodati) >= 900 && Convert.ToInt32(codiceBancaEsodati) <= 1000) || (Convert.ToInt32(codiceBancaEsodati) >= 601 && Convert.ToInt32(codiceBancaEsodati) <= 799)))
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla VOCRED con il filtro DAP (Gestione pubblica).
        /// Al verificarsi di queste condizione dovrà essere visibile il pannello contenente importo lordo alla decorrenza su DatiCalcolo\DatiCalcolo
        /// </summary>
        public static bool IsDomandaVOCRED_CRED27__DAP(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "DAP";
        }

        public static bool IsDomandaEsodo(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaVESO29(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria))
                return true;
            return false;
        }

        public static bool IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria) == TipoFondo.ET)
            {
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0008")
                    return true;
            }

            return false;
        }

        public static bool IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;



            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0008" && datiPensione.SiglaCategoria.Trim() == "VOCPDEL")
                return true;

            if (datiPensione.SiglaCategoria.Trim() == "VOCPDEL" && datiPensione.NaturaPensione != null && datiPensione.NaturaPensione.Trim() == "K")
                return true;


            return false;
        }

        public static bool IsDomandaPensioneAnticipataTipo0008(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0008")
                return true;

            return false;
        }

        public static bool IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            string filtro = datiPensione.GetFiltro();

            if (GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria) == TipoFondo.ET)
            {
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0043" && filtro == "PTA")
                    return true;
            }



            return false;
        }

        public static bool IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;


            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0043" && datiPensione.SiglaCategoria.Trim() == "VOCPDEL")
                return true;

            if (datiPensione.NaturaPensione != null && datiPensione.SiglaCategoria.Trim() == "VOCPDEL" && datiPensione.NaturaPensione.Trim() == "W")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda ha un beneficio Amianto 181
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>Ritorna true se attività economica e professione individuale sono pari a 14, 190</returns>
        public static bool IsDomandaConBeneficioAmianto181(int? attivitaEconomica, int? professioneIndividuale)
        {
            if (attivitaEconomica.GetValueOrDefault() == 14 && professioneIndividuale.GetValueOrDefault() == 190)
                return true;

            return false;
        }

        public static bool IsDomandaESPA(string categoria)
        {
            if (!string.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant() == "ESPA")
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla ESPA con il filtro L26 (Importo Extra-Calcolo).
        /// </summary>
        public static bool IsDomandaESPA_L26(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && Utility.IsDomandaESPA(datiPensione.SiglaCategoria.Trim().ToUpperInvariant()))
            {
                if (datiPensione.GetFiltro() == "L26")
                    return true;

                if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicESPAFiltroL26)
                    return true;
            }

            return false;
        }

        public static bool IsDomandaVecchiaiaESPA(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESPA(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0038")
                return true;
            return false;
        }

        public static bool IsDomandaAnticipataESPA(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaESPA(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0039")
                return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la pensione appartiene alla ESPA con il filtro AGO .
        /// </summary>
        public static bool IsDomandaESPA_AGO(GestionePensione.DatiPensione datiPensione)
        {
            if (!Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                return false;
            return datiPensione.GetFiltro() == "AGO";
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione in favore dei soggetti invalidi in misura non inferiore all’80% 
        /// (art. 1, comma 8, del d. lgs. n. 503 del 1992; circ. 65del 1995)
        /// Gruppo = 0001, Prodotto = 0002, Tipo = 0001 oppure
        /// Gruppo = 0001, Prodotto = 0002, Tipo = 0002
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaManualeInvaliditaOver80(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not && ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001") ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0002")) &&
                datiPensione.CodiceTipoRichiesta == "C8")
                return true;
            return false;
        }

        public static bool IsDomandaManualeInvaliditaOver80_L80(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaManualeInvaliditaOver80(datiPensione) && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("L80"))
                return true;
            return false;
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione di vecchiaia VO-VR-VOART-VOCOM con tipologia ENAV
        /// Gruppo = 0001, Prodotto = 0002, Tipo = 0001
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaVecchiaiaENAV(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VO" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VR" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOART" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOCOM") &&
                datiPensione.CodiceTipoRichiesta == "EN" && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001")
                return true;

            return false;
        }

        public static bool IsRiaperturaRicTRF_Benefici16_17(GestionePensione.DatiPensione datiPensione, string beneficio)
        {
            bool retVal = false;
            if (datiPensione == null)
                return retVal;

            if (beneficio == "16" || beneficio == "17")
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneRIC_TRFMemo16_2020 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo16_2020", out ctrlAbilitazioneRIC_TRFMemo16_2020);

                if (ctrlAbilitazioneRIC_TRFMemo16_2020 != null && !String.IsNullOrEmpty(ctrlAbilitazioneRIC_TRFMemo16_2020.ValoreControllo) && !String.IsNullOrEmpty(ctrlAbilitazioneRIC_TRFMemo16_2020.ValoreControllo.Trim()) &&
                            ctrlAbilitazioneRIC_TRFMemo16_2020.ValoreControllo == "SI")
                {
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    if (tipoAppartenenza == Utility.TipoAppartenenza.AGO)
                    {
                        GestioneLavorazione.DatiLavorazione datiLavorazione;
                        GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
                        string codFase = datiLavorazione.CodFase;
                        bool isRiapertura = Utility.IsRiaperturaDomanda(codFase);
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) || isRiapertura)
                            retVal = true;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gruppo = 0001, Prodotto = 0001, Tipo = 0001 
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaAnticipataEsattoriali(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if ((datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VO" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOCOM" ||
                datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOART" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VR") &&
                datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0001" && datiPensione.CodiceTipoRichiesta == "ES")
                return true;
            return false;
        }

        public static bool IsDomandaINPGI(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOPGI" || categoria.Trim().ToUpperInvariant() == "IOPGI" || categoria.Trim().ToUpperInvariant() == "SOPGI")
                return true;
            return false;
        }

        public static bool IsDomandaVOPGI(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOPGI")
                return true;
            return false;
        }

        public static bool IsDomandaVOPGI_AGI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (!Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria))
                return false;
            if (datiPensione.GetFiltro() == "AGI" || (!string.IsNullOrEmpty(datiPensione.DirittoAutonomo) && !string.IsNullOrEmpty(datiPensione.DirittoAutonomo.Trim()) &&
                datiPensione.DirittoAutonomo.Trim().ToUpperInvariant() == "DA") ||
                (!String.IsNullOrEmpty(datiPensione.GP1AJ11) && datiPensione.GP1AJ11.Trim() == "1"))
                return true;
            return false;
        }

        public static bool IsDomandaVOMIN(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOMIN")
                return true;
            return false;
        }
        public static bool IsDomandaSOMIN(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "SOMIN")
                return true;
            return false;
        }

        public static bool IsDomandaMIN(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOMIN" || categoria.Trim().ToUpperInvariant() == "SOMIN" || categoria.Trim().ToUpperInvariant() == "IOMIN")
                return true;
            return false;
        }

        public static bool IsDomandaPescatori(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOP" || categoria.Trim().ToUpperInvariant() == "SOP" || categoria.Trim().ToUpperInvariant() == "IOP")
                return true;
            return false;
        }

        public static bool IsDomandaPescatoriFiltroL80(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVOP(datiPensione.SiglaCategoria) && datiPensione.CodiceTipoRichiesta == "C8")
                return true;

            return false;
        }

        public static bool IsDomandaVOP(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOP")
                return true;
            return false;
        }

        public static bool IsDomandaBancari(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim() == "VOBANC" || siglaCategoria.Trim() == "IOBANC" || siglaCategoria.Trim() == "SOBANC"))
                return true;
            return false;
        }

        public static bool IsDomandaVOBANC(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "VOBANC")
                return true;
            return false;
        }

        public static bool IsDomandaIOBANC(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "IOBANC")
                return true;
            return false;
        }

        public static bool IsDomandaSOBANC(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "SOBANC")
                return true;
            return false;
        }

        public static bool IsDomandaBancariPLConBonus(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVOBANC(datiPensione.SiglaCategoria) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                (datiPensione.CodiceTipoRichiesta == "51" || datiPensione.CodiceTipoRichiesta == "52" || datiPensione.CodiceTipoRichiesta == "55" || datiPensione.CodiceTipoRichiesta == "56"))
                return true;

            return false;
        }

        public static bool IsDomandaESOPMI(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "ESOPMI")
                return true;
            return false;
        }

        //ENG - Aggiornamento Memo 68/2022 IOPGI
        public static bool IsDomandaIOPGI(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "IOPGI")
                return true;
            return false;
        }

        //ENG - Aggiornamento Memo 68/2022 IOPGI
        public static bool IsDomandaIOPGI_AGI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && datiPensione.GetFiltro() == "AGI")
                return true;

            return false;
        }


        #region Data Sistema per tipo fondo

        public static DateTime DataSistemaAgo
        {
            get { return GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.AGO); }
        }

        public static DateTime DataSistemaFs
        {
            get { return GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.FS); }
        }

        public static DateTime DataSistemaCi
        {
            get { return GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.CI); }
        }

        #endregion

        #region Gestione Versioni

        public static void GetListaVersioni(ref List<GestioneVersioni.DatiVersioni> elencoVersioni, Utility.ChiaviVersioni chiaveVersione, long currentVersion)
        {
            if (elencoVersioni == null)
                elencoVersioni = new List<GestioneVersioni.DatiVersioni>();

            Dictionary<string, long> dctApp = new Dictionary<string, long>();
            GestioneVersioni.DatiVersioni versione = null;
            if (elencoVersioni.FindIndex(x => x.Applicativo == chiaveVersione.ToString()) == -1)
            {
                versione = new GestioneVersioni.DatiVersioni();
                versione.Applicativo = chiaveVersione.ToString();
                versione.NumVersione = currentVersion;
                versione.Data = DateTime.Now;
                elencoVersioni.Add(versione);
                dctApp.Add(versione.Applicativo, versione.NumVersione);
            }
            else
                versione = elencoVersioni.Find(x => x.Applicativo == chiaveVersione.ToString());

            if (versione != null)
            {
                if (versione.NumVersione < currentVersion)
                {
                    versione.NumVersione = currentVersion;
                    versione.Data = DateTime.Now;
                    dctApp.Add(versione.Applicativo, currentVersion);
                }
            }

            if (dctApp.Count > 0)
                GestioneVersioni.AggiornaVersioni(dctApp);
        }

        public static Dictionary<string, string> FormattaVersioni(List<GestioneVersioni.DatiVersioni> elencoVersioni)
        {
            Dictionary<string, string> dctApp = null;
            if (elencoVersioni != null && elencoVersioni.Count > 0)
            {
                dctApp = new Dictionary<string, string>();
                foreach (GestioneVersioni.DatiVersioni versioneAggiornata in elencoVersioni)
                {
                    dctApp.Add("Versione" + versioneAggiornata.Applicativo, versioneAggiornata.Applicativo + " v." + versioneAggiornata.NumVersione + " del " + string.Format("{0: dd/MM/yyyy}", versioneAggiornata.Data));
                }
            }
            return dctApp;
        }

        #endregion Gestione Versioni

        #region private methods
        private static void GetFondoBySiglaCategoria(string siglaCategoria, out string fondo)
        {
            fondo = string.Empty;
            if (string.IsNullOrEmpty(siglaCategoria))
                return;
            if (siglaCategoria.Trim().Length < 3)
                fondo = siglaCategoria.Trim();
            else if (siglaCategoria.Trim().Length == 3)
                fondo = siglaCategoria.Substring(1, 2).Trim();
            else
                fondo = siglaCategoria.Substring(1, 3).Trim();
            //if (fondo == "PL") fondo = "PI"; non si può fare perchè impatta anche altre cose
        }
        #endregion private methods

        #region Predicate Builder
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
        #endregion Predicate Builder

        public static Dictionary<string, char?> GetTipoPensione(GestionePensione.DatiPensione datiPensione)
        {
            Dictionary<string, char?> keyValuePair = new Dictionary<string, char?>();

            if (datiPensione.SiglaCategoria.StartsWith("V"))
            {
                keyValuePair.Add("VECCHIAIA", '1');
            }
            if (datiPensione.SiglaCategoria.StartsWith("I"))
            {
                keyValuePair.Add("INVALIDITA'", '2');

            }
            if (datiPensione.SiglaCategoria.StartsWith("S"))
            {
                keyValuePair.Add("INDIRETTA", '3');
            }
            return keyValuePair;
        }

        public static bool IsSameTipoPensione(string SiglaCat1, string SiglaCat2)
        {
            if (SiglaCat1 == null || SiglaCat2 == null)
                return false;
            if (SiglaCat1.StartsWith(SiglaCat2.Substring(0, 1)))
                return true;

            return false;
        }

        public static DateTime? GetDecorrenzaPerSindacatoANPPE(DateTime? decorrenzaSindacato, string CodiceSindacato)
        {
            if (IsSindacatoPresente(CodiceSindacato) && CodiceSindacato.Trim() == "MP")
            {
                if (decorrenzaSindacato.HasValue && !DataSuccessivaA(decorrenzaSindacato.Value, new DateTime(2015, 10, 1)))
                    return new DateTime(2015, 10, 1);
            }

            return decorrenzaSindacato;
        }

        /// <summary>
        /// Valorizza la proprietà con il nome nameProperty nell'area source con il valore value
        /// </summary>
        /// <param name="nameProperty"></param>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns>Torna true se viene valorizzata la proprietà, altrimenti false</returns>
        public static bool SetValueByNameProperty(string nameProperty, object source, object value)
        {
            try
            {
                if (source == null)
                    return false;
                Type sourceType = source.GetType();
                List<PropertyInfo> sourceProperties = sourceType.GetProperties().ToList();
                PropertyInfo p = sourceProperties.FirstOrDefault(x => x.Name == nameProperty);
                if (p != null)
                {
                    p.SetValue(source, value, null);
                    return true;
                }
                else
                {
                    if (sourceProperties != null)
                    {
                        foreach (var property in sourceProperties.FindAll(x => x.PropertyType.Assembly.ToString().StartsWith(sourceType.Assembly.ToString().Substring(0, sourceType.Assembly.ToString().IndexOf('.')))))
                        {
                            object propValue = property.GetValue(source, null);
                            if (propValue != null)
                            {
                                var elems = propValue.GetType().GetProperties().ToList();
                                if (elems != null && elems.Count > 0)
                                    if (SetValueByNameProperty(nameProperty, propValue, value))
                                        return true;
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                //Eccezione ignorata
            }

            return false;
        }

        /// <summary>
        /// Ritorna il valore della proprietà con nome nameProperty dell'area source
        /// </summary>
        /// <param name="nameProperty"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object GetValueByNameProperty(string nameProperty, object source)
        {
            object retValue = null;
            try
            {
                object value = null;
                if (source == null)
                    return retValue;
                Type sourceType = source.GetType();
                List<PropertyInfo> sourceProperties = sourceType.GetProperties().ToList();
                PropertyInfo p = sourceProperties.FirstOrDefault(x => x.Name == nameProperty);
                if (p != null)
                {
                    value = p.GetValue(source, null);
                    retValue = value;
                }
                else
                {
                    if (sourceProperties != null)
                    {
                        foreach (var property in sourceProperties.FindAll(x => x.PropertyType.Assembly.ToString().StartsWith(sourceType.Assembly.ToString().Substring(0, sourceType.Assembly.ToString().IndexOf('.')))))
                        {
                            object propValue = property.GetValue(source, null);
                            if (propValue != null)
                            {
                                var item = propValue.GetType().GetProperties().ToList();
                                if (item != null && item.Count > 0)
                                    retValue = GetValueByNameProperty(nameProperty, propValue);
                            }

                            if (retValue != null)
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione anticipata con benefici L. 206/2004 - vittime Invalidità => 80%
        /// Gruppo = 0001, Prodotto = 0001, Tipo = 0158
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaBeneficioTerrorismoOver80(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0158") ||
                ((IsRicostituzione(datiPensione.Gruppo) || IsDomandaRipristino(datiPensione).GetValueOrDefault()) && datiBeneficioVittimeTerrorismo != null &&
                 (datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 1 || datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 2 || datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 3) &&
                 datiBeneficioVittimeTerrorismo.SoggettoBeneficiario == 1))
                return true;

            return false;
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione anticipata con benefici L. 206/2004 - vittime Invalidità => 80%
        /// Gruppo = 0001, Prodotto = 0001, Tipo = 0158
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaBeneficioTerrorismoOver80(GestionePensione.DatiPensione datiPensione, long? soggettoBeneficiario, long? tipologiaPrestazione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0158") ||
                (IsRicostituzione(datiPensione.Gruppo) && (tipologiaPrestazione == 1 || tipologiaPrestazione == 2 || tipologiaPrestazione == 3) && soggettoBeneficiario == 1))
                return true;

            return false;
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione anticipata con benefici L. 206/2004 - vittime Invalidità &lt; 80% 
        /// o una Pensione di vecchiaia con benefici L. 206/2004 - vittime Invalidità &lt; 80%
        /// Gruppo = 0001, Prodotto = 0001, Tipo = 0159
        /// Gruppo = 0001, Prodotto = 0002, Tipo = 0159
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaBeneficioTerrorismoUnder80(GestionePensione.DatiPensione datiPensione, long? soggettoBeneficiario, long? tipologiaPrestazione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0159") ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0159")) ||
                (IsRicostituzione(datiPensione.Gruppo) && (tipologiaPrestazione == 1 || tipologiaPrestazione == 2 || tipologiaPrestazione == 3) && soggettoBeneficiario != 1))
                return true;

            return false;
        }

        /// <summary>
        /// Restituisce true se la domanda è una Pensione anticipata con benefici L. 206/2004 - vittime Invalidità &lt; 80% 
        /// o una Pensione di vecchiaia con benefici L. 206/2004 - vittime Invalidità &lt; 80%
        /// Gruppo = 0001, Prodotto = 0001, Tipo = 0159
        /// Gruppo = 0001, Prodotto = 0002, Tipo = 0159
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaBeneficioTerrorismoUnder80(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0159") ||
                (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0159")) ||
                ((IsRicostituzione(datiPensione.Gruppo) || IsDomandaRipristino(datiPensione).GetValueOrDefault()) && datiBeneficioVittimeTerrorismo != null &&
                 (datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 1 || datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 2 || datiBeneficioVittimeTerrorismo.TipologiaPrestazione == 3) &&
                 datiBeneficioVittimeTerrorismo.SoggettoBeneficiario != 1))
                return true;

            return false;
        }

        // ENG - Memo 49_2023 
        public static bool IsDomandaBeneficioTerrorismoLegge206_2004(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0105" && datiPensione.Tipo == "0112")
                return true;

            return false;
        }

        public static bool IsRicEsenzioneFiscaleVittimeDelDovere(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0108" && datiPensione.Tipo == "0166") ||
                (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0308" && datiPensione.Tipo == "0166") ||
                (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0408" && datiPensione.Tipo == "0166"))
                return true;

            return false;
        }

        public static bool IsRicostituzione_MotiviDocumentali(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0108" && datiPensione.Tipo == "0001") ||
                (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0308" && datiPensione.Tipo == "0001") ||
                (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0408" && datiPensione.Tipo == "0001"))
                return true;

            return false;
        }

        public static bool IsRicostituzione_VariazioneDatiContitolari(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0413" && datiPensione.Tipo == "0001")
                return true;

            return false;
        }

        /// <summary>
        /// True se il tipo calcolo è retributivo o misto
        /// Condizione di visibilità della griglia dei dati Retributivi Vittime (Terrorismo)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDatiRetributiviVittimeVisible(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            TipoCalcolo tipoCalcoloEnum)
        {
            if (IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo))
            {
                if (tipoCalcoloEnum == TipoCalcolo.Retributivo || tipoCalcoloEnum == TipoCalcolo.Misto)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True se il tipo calcolo è misto o contributivo
        /// Condizione di visibilità della griglia dei dati Contributivi Vittime (Terrorismo)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDatiContributiviVittimeVisible(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo,
            TipoCalcolo tipoCalcoloEnum, bool isQuotaDPresente)
        {
            if (IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) || IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo))
            {
                if (tipoCalcoloEnum == TipoCalcolo.Contributivo || tipoCalcoloEnum == TipoCalcolo.Misto ||
                    (tipoCalcoloEnum == TipoCalcolo.Retributivo && ((datiPensione.FineAssicurazione.HasValue && Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 1, 1))) || isQuotaDPresente)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// False se è una pensione di Vittime Invalidi in misura inferiore all’80% con Soggetto Beneficiario Coniuge / Genitore / Figlio e Tipologia Prestazione 2 e Tipologia Beneficio 2
        /// Condizione di visibilità della griglia dei dati Importo Pensione Vittime (Terrorismo)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="datiBeneficioVittimeTerrorisimo"></param>
        /// <returns></returns>
        public static bool IsDatiImportoPensioneVittimeVisible(GestionePensione.DatiPensione datiPensione, long? soggettoBeneficiario, long? tipologiaPrestazione, long? tipologiaBeneficio)
        {
            if (IsDomandaBeneficioTerrorismoOver80(datiPensione, soggettoBeneficiario, tipologiaPrestazione))
                return true;

            // Controllo specifico richiesto  dalla mail del 10/11/2016 con oggetto: LIQPENS RE: vittime, modifica della visibilità della sezione “Importo Pensione Vittime” nei “Dati Calcolo Terrorismo” 
            if (IsDomandaBeneficioTerrorismoUnder80(datiPensione, soggettoBeneficiario, tipologiaPrestazione)
                && (soggettoBeneficiario.GetValueOrDefault() == 2 || soggettoBeneficiario.GetValueOrDefault() == 3) /*corrisponde a valore 1 oppure 2 al 2° byte del GP1AC02*/
                && tipologiaPrestazione.GetValueOrDefault() == 2 /*corrisponde a valore 2 al 1° byte del GP1AC01 */
                && (tipologiaBeneficio.GetValueOrDefault() == 1 || tipologiaBeneficio.GetValueOrDefault() == 2)) /*corriponde a valore 1 oppure 2 al 3° byte del GP1AC01*/
                return false;

            // Requisito presente sulla mail del 18/04/2016 con oggetto: RE: LIQPENS - Dubbio precompilazione griglia ‘Importo Pensione Vittime’
            if (IsDomandaBeneficioTerrorismoUnder80(datiPensione, soggettoBeneficiario, tipologiaPrestazione) && soggettoBeneficiario.GetValueOrDefault() == 2)
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è nelle condizioni di Ante Armonizzazione
        /// Attenzione! E' presente una get all'interno del metodo
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaAnteArmonizzazione(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, DateTime? decorrenzaPensione, bool? dimissioniAnte97 = null,
            object datiFondoXX = null, char? codiceRequisiti2 = null, GestioneFondo.DatiFondo datiFondo = null, GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivo = null,
            List<GestioneDatiServizioUtile.ServizioUtile> datiServizioUtile = null)
        {
            bool ret = false;
            DateTime? dataLimite = null;
            switch (tipoFondo)
            {
                case Utility.TipoFondo.EL:
                    dataLimite = new DateTime(1997, 5, 1);
                    if (decorrenzaPensione.HasValue && !Utility.DataSuccessivaA(decorrenzaPensione.Value, dataLimite.Value))
                        ret = true;
                    else if (IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                    {
                        if (!codiceRequisiti2.HasValue)
                        {
                            if (datiFondo == null && datiPensione.Id != 0)
                                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);
                            if (datiFondo != null)
                                codiceRequisiti2 = datiFondo.CodiceRequisiti2;
                        }
                        if (codiceRequisiti2.GetValueOrDefault() != '0')
                        {
                            if (datiPensione.Id != 0)
                                GestioneCalcolo.GetCalcoloRetributivoByIdPensione(datiPensione.Id, out datiCalcoloRetributivo);
                            if (datiCalcoloRetributivo == null)
                            {
                                if (datiPensione.Id != 0)
                                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(datiPensione.Id, out datiServizioUtile);
                                if (datiServizioUtile != null && datiServizioUtile.Count(x => !x.IsNull()) > 0)
                                    ret = true;
                            }
                        }
                    }
                    break;
                case Utility.TipoFondo.VL:
                    dataLimite = new DateTime(1997, 8, 1);
                    if (decorrenzaPensione.HasValue && !Utility.DataSuccessivaA(decorrenzaPensione.Value, dataLimite.Value))
                        ret = true;
                    break;
                case Utility.TipoFondo.ET:
                    dataLimite = new DateTime(1996, 1, 1);
                    if (decorrenzaPensione.HasValue && !Utility.DataSuccessivaA(decorrenzaPensione.Value, dataLimite.Value))
                        ret = true;
                    break;
                case Utility.TipoFondo.TT:
                    if (!dimissioniAnte97.HasValue)
                    {
                        if (datiFondoXX != null)
                            dimissioniAnte97 = ((GestioneFondo.DatiFondoTT)datiFondoXX).DimissioniAnte97;
                        else
                        {
                            GestioneFondo.DatiFondoTT datiFondoTT = null;
                            if (datiPensione.Id != 0)
                                GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                            if (datiFondoTT != null)
                                dimissioniAnte97 = datiFondoTT.DimissioniAnte97;
                        }
                    }

                    dataLimite = new DateTime(1997, 8, 1);
                    if ((decorrenzaPensione.HasValue && !Utility.DataSuccessivaA(decorrenzaPensione.Value, dataLimite.Value)) || (dimissioniAnte97.GetValueOrDefault()))
                        ret = true;
                    break;
            }
            return ret;
        }

        public static bool IsVisibleTabAltraPensioneDatiAgo(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, DateTime? decorrenzaPensione, string naturaPensione)
        {
            bool ret = false;
            DateTime? decorrenzaPensioneOrDecorrenzaPensioneDC = GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza == TipoAppartenenza.FS)
            {
                TipoFondo? tipoFondo = GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                if (tipoFondo == TipoFondo.ET && IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaPensioneOrDecorrenzaPensioneDC)
                    && !string.IsNullOrEmpty(naturaPensione) && naturaPensione.FirstOrDefault() == '6' &&
                    decorrenzaPensione.HasValue && !DataStrettamenteSuccessivaA(decorrenzaPensione.Value, new DateTime(1996, 1, 1)))
                    ret = true;
            }
            return ret;
        }

        public static bool IsCategoriaAutonomi(string siglaCategoria)
        {
            switch (siglaCategoria.Trim().ToUpperInvariant())
            {
                case "VOCOM":
                case "VOART":
                case "VR":
                case "IOCOM":
                case "IOART":
                case "IR":
                case "SOCOM":
                case "SOART":
                case "SR":
                    return true;
            }
            return false;
        }

        public static bool IsPensioneVecchiaiaOrRicostituzione(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP)
        {
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002")
                return true;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue &&
                (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI) &&
                datiPensione.SiglaCategoria.StartsWith("V") && !string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                (datiPensione.NaturaPensione.StartsWith(" ") || datiPensione.NaturaPensione.StartsWith("6")))
                return true;

            if (GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS &&
                datiPensione.SiglaCategoria.StartsWith("V"))
            {
                TipoFondo? tipoFondo = GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                List<char> listaCodiciSpecifici = null;
                CategoriaFondoPI? categoriaFondoPI = GetCategoriaFondoPI(tipoAppartenenza, datiPensione.SiglaCategoria);

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case TipoFondo.EL:
                        case TipoFondo.TT:
                        case TipoFondo.VL:
                            listaCodiciSpecifici = new List<char> { 'A', 'B' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.ET:
                            listaCodiciSpecifici = new List<char> { 'A', 'B', 'C', 'L', 'Z' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.DZ:
                            listaCodiciSpecifici = new List<char> { 'A', 'F' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.ES:
                            listaCodiciSpecifici = new List<char> { 'A', 'B', 'C', 'D', 'E', 'L', 'M', 'N' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.FS:
                        case TipoFondo.PT:
                            listaCodiciSpecifici = new List<char> { 'B', 'I' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.GAS:
                            listaCodiciSpecifici = new List<char> { 'A', 'M', 'N' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.PI:
                        case TipoFondo.PL:
                            if (categoriaFondoPI.HasValue)
                            {
                                switch (categoriaFondoPI.Value)
                                {
                                    case CategoriaFondoPI.Uno:
                                    case CategoriaFondoPI.A:
                                    case CategoriaFondoPI.Y:
                                        listaCodiciSpecifici = new List<char> { 'B' };
                                        if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                            return true;
                                        break;
                                    case CategoriaFondoPI.U:
                                    case CategoriaFondoPI.V:
                                        listaCodiciSpecifici = new List<char> { 'A', 'B' };
                                        if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                            return true;
                                        break;
                                }
                            }
                            if(tipoFondo == TipoFondo.PL)
                            {
                                listaCodiciSpecifici = new List<char> { 'B' };
                                if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                    return true;
                                break;
                            }
                            break;
                        case TipoFondo.PM:
                            listaCodiciSpecifici = new List<char> { 'E' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                    }
                }
                else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    listaCodiciSpecifici = new List<char> { 'B', 'I' };
                    if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                        return true;
                }

                return false;
            }

            return false;
        }

        public static bool IsPensioneAnzianitaPL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001")
                return true;
            return false;
        }

        public static bool IsPensioneAnzianitaOrRicostituzione(GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP)
        {
            return IsPensioneAnzianitaOrRicostituzione(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Gestione, datiPensione.IndConvInt, datiPensione.SiglaCategoria, datiPensione.NaturaPensione, codiceSpecificoTraduzioneSuGP);
        }

        public static bool IsPensioneAnzianitaOrRicostituzione(string gruppo, string prodotto, string gestione, bool? indConvInt, string siglaCategoria, string naturaPensione, char? codiceSpecificoTraduzioneSuGP)
        {
            if (gruppo == "0001" && prodotto == "0001")
                return true;

            TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(indConvInt, gestione);

            if (GetTipoDomanda(gruppo, prodotto) == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue &&
                (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI) &&
                siglaCategoria.StartsWith("V") && !string.IsNullOrEmpty(naturaPensione) &&
                (naturaPensione.StartsWith("1") || naturaPensione.StartsWith("2")))
                return true;

            if (GetTipoDomanda(gruppo, prodotto) == TipoDomanda.Ricostituzione && tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS &&
                siglaCategoria.StartsWith("V"))
            {
                TipoFondo? tipoFondo = GetTipoFondoByCategoria(tipoAppartenenza, siglaCategoria);
                List<char> listaCodiciSpecifici = null;
                CategoriaFondoPI? categoriaFondoPI = GetCategoriaFondoPI(tipoAppartenenza, siglaCategoria);

                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case TipoFondo.EL:
                        case TipoFondo.FS:
                        case TipoFondo.PT:
                            listaCodiciSpecifici = new List<char> { 'C' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.ET:
                            listaCodiciSpecifici = new List<char> { 'D', 'W' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.GAS:
                            listaCodiciSpecifici = new List<char> { 'B' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.PI:
                        case TipoFondo.PL:
                            if (categoriaFondoPI.HasValue)
                            {
                                switch (categoriaFondoPI.Value)
                                {
                                    case CategoriaFondoPI.Uno:
                                    case CategoriaFondoPI.A:
                                    case CategoriaFondoPI.Y:
                                        listaCodiciSpecifici = new List<char> { 'C' };
                                        if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                            return true;
                                        break;
                                    case CategoriaFondoPI.U:
                                    case CategoriaFondoPI.V:
                                        listaCodiciSpecifici = new List<char> { 'C', 'D' };
                                        if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                            return true;
                                        break;
                                }
                            }
                            if (tipoFondo == TipoFondo.PL)
                            {
                                listaCodiciSpecifici = new List<char> { 'C' };
                                if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                    return true;
                            }
                            break;
                        case TipoFondo.TT:
                            listaCodiciSpecifici = new List<char> { 'E' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                        case TipoFondo.VL:
                            listaCodiciSpecifici = new List<char> { 'C', 'D' };
                            if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                                return true;
                            break;
                    }
                }
                else if (IsDomandaINPDAP(gestione))
                {
                    listaCodiciSpecifici = new List<char> { 'C' };
                    if (listaCodiciSpecifici.Contains(codiceSpecificoTraduzioneSuGP.GetValueOrDefault()))
                        return true;
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// memo 26/2020
        /// TRUE se la domanda è di tipo contributivo; contributivo con opzione; contributivo senza opzione
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="isAnzianita">Se TRUE controlla le domande di anzianità; se FALSE quelle di vecchiaia; se NULL entrambe</param>
        /// <param name="filtraOpzione">Se TRUE controlla le domande di pensione con opzione (tipo="0030"); se FALSE  quelle senza (tipo="0017"); se NULL entrambe</param>
        /// <returns></returns>
        public static bool IsDomandaTipoContributivo(GestionePensione.DatiPensione datiPensione, bool? isAnzianita, bool? filtraOpzione)
        {
            bool checkProdotto = false;
            bool checkOpzione = false;

            if (datiPensione == null)
                return false;

            if (datiPensione.Tipo.Trim() == "0017" && IsDomandaCumulo(datiPensione.SiglaCategoria))
                return false;

            if (datiPensione.Tipo.Trim() == "0030" && (IsDomandaCumulo(datiPensione.SiglaCategoria) || IsDomandaVOAUT_IOAUT(datiPensione.SiglaCategoria)))
                return false;

            if (Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) ||
                Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                return false;

            if (!isAnzianita.HasValue)
                checkProdotto = true;
            else if (isAnzianita.Value && datiPensione.Prodotto == "0001")
                checkProdotto = true;
            else if (!isAnzianita.Value && datiPensione.Prodotto == "0002")
                checkProdotto = true;

            if (!filtraOpzione.HasValue)
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0030") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0030") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "J") ||
                    (datiPensione.SceltaLavMadri.HasValue && datiPensione.SceltaLavMadri.Value > 0)) ||
                    (IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoConOpzione ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoPuro || datiPensione.IdTipoPLPerRIC == (byte?)TipoPLPerRIC.ContributivoPuro))
                {
                    checkOpzione = true;
                }
            }
            else if (filtraOpzione.Value)
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0030") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0030") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "J") ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoConOpzione)
                {
                    checkOpzione = true;
                }
            }
            else
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0017") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    datiPensione.SceltaLavMadri.HasValue && datiPensione.SceltaLavMadri.Value > 0 &&
                    (string.IsNullOrEmpty(datiPensione.NaturaPensione) || datiPensione.NaturaPensione.Substring(1, 1) != "J")) ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoPuro || datiPensione.IdTipoPLPerRIC == (byte?)TipoPLPerRIC.ContributivoPuro)
                {
                    checkOpzione = true;
                }
            }
            return (checkProdotto && checkOpzione);
        }

        public static bool IsDomandaAnticipataConOpzionePL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0030")
                return true;

            return false;
        }

        public static bool IsDomandaAnticipataConOpzionePLConFinestraDecorrenza(GestionePensione.DatiPensione datiPensione, DateTime? dataPerfezionamentoRequisiti = null)
        {
            if (datiPensione == null)
                return false;

            if (dataPerfezionamentoRequisiti == null)
                dataPerfezionamentoRequisiti = datiPensione.DataPerfezionamentoRequisiti;

            if (Utility.IsDomandaAnticipataConOpzionePL(datiPensione) &&
                dataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2019, 1, 1)) &&
                !Utility.DataStrettamenteSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2026, 12, 31)))
                return true;

            return false;
        }

        /// <summary>
        /// TRUE se la domanda è di tipo contributivo; contributivo con opzione; contributivo senza opzione
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="isAnzianita">Se TRUE controlla le domande di anzianità; se FALSE quelle di vecchiaia; se NULL entrambe</param>
        /// <param name="filtraOpzione">Se TRUE controlla le domande di pensione con opzione (tipo="0030"); se FALSE  quelle senza (tipo="0017"); se NULL entrambe</param>
        /// <returns></returns>
        public static bool IsDomandaTipoContributivoCumulo(GestionePensione.DatiPensione datiPensione, bool? isAnzianita, bool? filtraOpzione)
        {
            bool checkProdotto = false;
            bool checkOpzione = false;

            if (datiPensione == null)
                return false;

            if (!IsDomandaCumulo(datiPensione.SiglaCategoria))
                return false;

            if (!isAnzianita.HasValue)
                checkProdotto = true;
            else if (isAnzianita.Value && datiPensione.Prodotto == "0001")
                checkProdotto = true;
            else if (!isAnzianita.Value && datiPensione.Prodotto == "0002")
                checkProdotto = true;

            if (!filtraOpzione.HasValue)
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0030") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0030") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "J") ||
                    (datiPensione.SceltaLavMadri.HasValue && datiPensione.SceltaLavMadri.Value > 0)) ||
                    (IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoConOpzione ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoPuro))
                {
                    checkOpzione = true;
                }
            }
            else if (filtraOpzione.Value)
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0030") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0030") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) == "J") ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoConOpzione)
                {
                    checkOpzione = true;
                }
            }
            else
            {
                if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo.Trim() == "0017") ||
                    (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0017") ||
                    (IsRicostituzione(datiPensione.Gruppo) &&
                    datiPensione.SceltaLavMadri.HasValue && datiPensione.SceltaLavMadri.Value > 0 &&
                    (string.IsNullOrEmpty(datiPensione.NaturaPensione) || datiPensione.NaturaPensione.Substring(1, 1) != "J")) ||
                    IsDomandaTipoContributivoFromPrelievo(datiPensione) == TipoPLPerRIC.ContributivoPuro)
                {
                    checkOpzione = true;
                }
            }
            return (checkProdotto && checkOpzione);
        }

        public static bool IsDomandaRicAnticipataContributivaPura(GestionePensione.DatiPensione datiPensione, int? ContributiItalianiEdEsteriAl1295 = null)
        {
            if (IsRicostituzione(datiPensione.Gruppo) &&
                !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(1, 1) != "J" && datiPensione.NaturaPensione.Substring(1, 1) != "O" && (datiPensione.NaturaPensione.Substring(0, 1) == "1" || datiPensione.NaturaPensione.Substring(0, 1) != "2")
                && Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(1996, 1, 1)) && ContributiItalianiEdEsteriAl1295 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Utility.TipoPLPerRIC IsDomandaTipoContributivoFromPrelievo(GestionePensione.DatiPensione datiPensione, int? ContributiItalianiEdEsteriAl1295 = null)
        {
            if (IsRicostituzione(datiPensione.Gruppo) &&
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) == "J"))
                return TipoPLPerRIC.ContributivoConOpzione;
            if (IsRicostituzione(datiPensione.Gruppo) &&
                (!string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) != "O" &&
                datiPensione.NaturaPensione.PadRight(3, ' ').Substring(1, 1) != "I") &&
                Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(1996, 1, 1)) &&
                (Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault()) == TipoCalcolo.Contributivo
                || (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault() == Utility.TipoAppartenenza.CI && ContributiItalianiEdEsteriAl1295 == 0)))
                return TipoPLPerRIC.ContributivoPuro;
            return Utility.TipoPLPerRIC.Nessuno;
        }

        public static bool IsGestioneLavoratriciMadri(GestionePensione.DatiPensione datiPensione)
        {
            List<string> sigleCategorieNonAmmesse = new List<string> { "VOBIS", "VMP", "VOBANC", "INDCOM", "PSO", "PMO", "VOMIN", "VOSPED", "VOCRED", "VOCOOP", "CRED27", "COOP28", "VOESO", "VESO29", "ESPA", "ESOAMB", "ESOTEL", "VOST", "VESO33", "VESO92" };
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.StartsWith("V") &&
                (Utility.IsDomandaTipoContributivo(datiPensione, null, null) ||
                Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione) ||
                Utility.IsDomandaAnzianitaInComputo(datiPensione) ||
                Utility.IsDomandaVecchiaiaInComputo(datiPensione) ||
                Utility.IsDomandaTipoContributivoCumulo(datiPensione, null, null) ||
                Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
                && !IsDomandaAnticipataConOpzionePL(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && !(Utility.IsDomandaVOAUTContributivoOpzioneFiltroERI(datiPensione) || Utility.IsDomandaAGOTipoContributivoFiltroERI(datiPensione))
                && !(sigleCategorieNonAmmesse.Contains(datiPensione.SiglaCategoria.Trim()) && Utility.IsDomandaTipoContributivo(datiPensione, null, false))
                && !(sigleCategorieNonAmmesse.Contains(datiPensione.SiglaCategoria.Trim()) && Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
                && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
                return true;
            return false;
        }

        public static bool IsRicostituzioneTipoContributivo(GestionePensione.DatiPensione datiPensione)
        {
            if (IsRicostituzione(datiPensione.Gruppo) &&
                    !string.IsNullOrEmpty(datiPensione.NaturaPensione) &&
                    (datiPensione.NaturaPensione.StartsWith("1") || datiPensione.NaturaPensione.StartsWith("2") || datiPensione.NaturaPensione.StartsWith("6") || datiPensione.NaturaPensione == " "))
                return true;
            return false;
        }

        public static void GetDataRiferimento(DateTime? dataRaggiungimentoOpzione, DateTime? dataPerfezionamentoRequisiti, out DateTime dataRiferimento, out string nomeDataRiferimento)
        {
            if (!dataRaggiungimentoOpzione.HasValue)
            {
                nomeDataRiferimento = "perfezionamento del requisito";
                dataRiferimento = dataPerfezionamentoRequisiti.Value;
            }
            else
            {
                if (DateTime.Compare(dataPerfezionamentoRequisiti.Value, dataRaggiungimentoOpzione.Value) < 0)
                {
                    nomeDataRiferimento = "raggiungimento opzione";
                    dataRiferimento = dataRaggiungimentoOpzione.Value;
                }
                else
                {
                    nomeDataRiferimento = "perfezionamento del requisito";
                    dataRiferimento = dataPerfezionamentoRequisiti.Value;
                }
            }
        }

        public static bool IsDecPensioneValidaPerVecchOpzione(DateTime? decPensione, DateTime? dataRaggiungimentoOpzione, DateTime? dataOpzione, out string errore)
        {
            errore = string.Empty;

            if (dataOpzione.HasValue &&
                 !Utility.DataStrettamenteSuccessivaA(decPensione.GetValueOrDefault(), dataOpzione.Value))
            {
                errore = "La 'Decorrenza Pensione' deve essere strettamente maggiore della 'Data Opzione'";
                return false;
            }
            if (dataRaggiungimentoOpzione.HasValue &&
                !Utility.DataStrettamenteSuccessivaA(decPensione.GetValueOrDefault(), dataRaggiungimentoOpzione.Value))
            {
                errore = "La 'Decorrenza Pensione' deve essere strettamente maggiore della 'Data Raggiungimento Opzione'";
                return false;
            }
            return true;
        }

        //ENG - Prepensionamento Editoria art. 37, legge 416/1981, lettera a)
        public static bool IsPrepensionamentoEditoriaFiltroEAA(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO)
            {
                if ((Utility.IsDomandaVO(datiPensione.SiglaCategoria) || Utility.IsDomandaVDAI(datiPensione.SiglaCategoria) || Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria)) && (Utility.IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not) &&
                    datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0162" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("EAA"))
                    return true;

                if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicPrepensionamentoEditoriaArt37L416_1981_LetteraA)
                    return true;
            }
            return false;
        }

        //ENG - Prepensionamento Editoria art. 37 legge 416/1981, lettera b)
        public static bool IsPrepensionamentoEditoriaFiltroEBA(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO)
            {
                if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) && (Utility.IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not) &&
                    datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0162" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("EBA"))
                    return true;

                if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicPrepensionamentoEditoriaArt37L416_1981_LetteraB)
                    return true;
            }
            return false;
        }

        public static bool IsPrepensionamentoEditoriaArt1c154L205_2017(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((IsDomandaVO(datiPensione.SiglaCategoria) || IsDomandaVDAI(datiPensione.SiglaCategoria)) && (IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not))
            {
                if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO)
                {
                    if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0171")
                        return true;
                }
            }
            return false;
        }

        public static bool IsPrepensionamentoEditoriaArt1c500L160_2019(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO)
            {
                if ((IsDomandaVO(datiPensione.SiglaCategoria) || IsDomandaVDAI(datiPensione.SiglaCategoria)) && (IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Not))
                {
                    if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0179")
                        return true;
                }

                if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicPrepensionamentoEditoriaArt1c500L160_2019)
                    return true;
            }

            return false;
        }

        public static bool IsPrepensionamentoEditoriaTipo0162(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO)
            {
                if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0162")
                    return true;
            }
            return false;
        }

        public static string GetTypeOneriEditoria(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool isRiaperturaDomanda)
        {
            string oneriEditoria = null;

            if (datiIstruttoria != null && datiIstruttoria.CodiceAziendaEditoria != null)
            {
                short? codiceAziendaIstruttoria = datiIstruttoria.CodiceAziendaEditoria;

                List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> elencoAnagraficaAccordi = null;
                GestioneAnagraficaAccordi.GetDecAnagraficaAccordi(out elencoAnagraficaAccordi);

                GestioneAnagraficaAccordi.DecodAnagraficaAccordi accordo = elencoAnagraficaAccordi.Find(x => x.Codice == codiceAziendaIstruttoria);

                if (accordo != null && accordo.DenominazioneAzienda.HasValue)
                {
                    List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziende = null;
                    GestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoAnagraficaAziende);

                    GestioneAnagraficaAziende.DecodAnagraficaAziende azienda = elencoAnagraficaAziende.Find(x => x.Id == accordo.DenominazioneAzienda);

                    if (azienda != null && !string.IsNullOrEmpty(azienda.SottogruppoOnere))
                    {
                        oneriEditoria = azienda.SottogruppoOnere.ToString();
                    }
                }
                else if (IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    oneriEditoria = "0";
            }

            return oneriEditoria;
        }

        public static bool IsDomandaCasellario(string siglaCategoria, string sede)
        {
            if (siglaCategoria != null && siglaCategoria.Trim() == "DIR" && sede != null && sede.Trim() == "9933")
                return true;

            return false;
        }

        public static bool IsBonusBooking(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsRicostituzione(datiPensione.Gruppo) && (datiPensione.Prodotto == "0101" || datiPensione.Prodotto == "0301" || datiPensione.Prodotto == "0401"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// AGO - FS
        /// Se la domanda è in regime sperimentale lavoratrici e la Decorrenza Pensione è maggiore o uguale al 01/2016 e 
        /// il primo codice della natura pensione è pari a 1 allora gli Oneri sono obbligatori
        /// Solo per domande automatiche non devo effettuare i controlli
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="isRiaperturaDomanda"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="naturaPensione"></param>
        /// <returns></returns>
        public static bool IsOneriSperDonnaObbligatoriPerControlli(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, DateTime? decorrenzaOriginaria, string naturaPensione)
        {
            if (IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Automatica)
                return false;

            if ((GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO || GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.FS) &&
                !isRiaperturaDomanda && IsDomandaSperimentaleDonna(datiPensione))
            {
                if (decorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2016, 1, 1)) &&
                    (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.FS ||
                    (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO &&
                    !string.IsNullOrEmpty(naturaPensione) && (naturaPensione.StartsWith("1") || naturaPensione.StartsWith("2")))))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// AGO - FS
        /// Se la domanda è in regime sperimentale lavoratrici e la Decorrenza Pensione è maggiore o uguale al 01/2016 e 
        /// il primo codice della natura pensione è pari a 1 allora gli Oneri sono obbligatori
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="isRiaperturaDomanda"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="naturaPensione"></param>
        /// <returns></returns>
        public static bool IsOneriSperDonnaVisibili(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, DateTime? decorrenzaOriginaria, string naturaPensione, List<GestioneOneri.DatiOneri> lDatiOneri)
        {
            if ((IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Automatica) || IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (lDatiOneri != null && lDatiOneri.Count > 0)
                    return true;
                else
                    return false;
            }

            if ((GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO || GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.FS) &&
                !isRiaperturaDomanda && IsDomandaSperimentaleDonna(datiPensione))
            {
                if ((decorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2016, 1, 1))) &&
                    (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.FS ||
                    (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == TipoAppartenenza.AGO &&
                    !string.IsNullOrEmpty(naturaPensione) && (naturaPensione.StartsWith("1") || naturaPensione.StartsWith("2")))))
                    return true;
            }

            return false;
        }

        public static bool IsDomandaSalvaguardia122_FS_2011_2012(GestionePensione.DatiPensione datiPensione, char? derogaTraduzioneSuGP)
        {
            TipoFondo? tipoFondo = GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
            switch (tipoFondo)
            {
                case TipoFondo.EL:
                case TipoFondo.TT:
                case TipoFondo.ET:
                case TipoFondo.GAS:
                case TipoFondo.DZ:
                case TipoFondo.ES:
                case TipoFondo.PM:
                case TipoFondo.PI:
                case TipoFondo.VL:
                case TipoFondo.PL:
                    if ((IsDomandaSalvaguardia122(datiPensione) || derogaTraduzioneSuGP == '3') && datiPensione.DataPerfezionamentoRequisiti.HasValue &&
                        (datiPensione.DataPerfezionamentoRequisiti.Value.Year == 2011 || datiPensione.DataPerfezionamentoRequisiti.Value.Year == 2012))
                        return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per il Credito Cooperativo
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioCreditoCooperativo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0032")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per Poste Italiane
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioPosteItaliane(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0035")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per Dipendenti ex Monopoli
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioDipExMonopoli(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0033")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per Riscossione Tributi Erariali
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioRiscossioneTributiErariali(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0034")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per Ferrovie dello Stato
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioFerrovieDelloStato(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0036")
                return true;

            return false;
        }

        /// <summary>
        /// Verifica se la domanda è un Assegno straordinario a sostegno del reddito per Ferrovie dello Stato (solidaristico)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoStraordinarioFerrovieDelloStatoSolidaristico(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) != TipoAppartenenza.AGO)
                return false;

            if (datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0052" && datiPensione.Tipo == "0053")
                return true;

            return false;
        }

        public static bool IsDomandaSpacchettamentoENPALS(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaENPALS(datiPensione.Gestione) && IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                return true;

            return false;
        }

        public static bool IsDomandaSpacchettamentoINPDAP(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINPDAP(datiPensione.Gestione) && IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                return true;
            return false;
        }

        public static bool IsRicostituzioneOrRiaperturaAGOAbilitata(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            List<string> sigleCategorieAmmesse = new List<string> { "VO", "IO", "VR", "IR", "VOART", "IOART", "VOCOM", "IOCOM", "VOAUT", "IOAUT", "VDAI", "IDAI", "SO", "SR", "SOART", "SOCOM", "SOAUT", "SDAI" };

            if ((isRiaperturaDomanda || GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == TipoDomanda.Ricostituzione) &&
                !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && sigleCategorieAmmesse.Contains(datiPensione.SiglaCategoria.Trim()))
                return true;

            return false;
        }

        public static bool IsRicostituzioneOrRiaperturaAGOAutomaticaAbilitata(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (IsDomandaUnicarpe(datiPensione, true) == TipoUnicarpe.Automatica && IsRicostituzioneOrRiaperturaAGOAbilitata(datiPensione, isRiaperturaDomanda))
                return true;

            return false;
        }

        public static bool IsRicostituzioneOrRiapertura(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (isRiaperturaDomanda || GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == TipoDomanda.Ricostituzione)
                return true;

            return false;
        }

        public static bool IsDomandaINDCOM(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim() == "INDCOM")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM175(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINDCOM(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0175")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM175(string siglaCategoria, string tipo)
        {
            if (IsDomandaINDCOM(siglaCategoria) && tipo == "0175")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM156(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINDCOM(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0156")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM129(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINDCOM(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0129")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM125(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINDCOM(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0125")
                return true;
            return false;
        }

        public static bool IsDomandaINDCOM124(GestionePensione.DatiPensione datiPensione)
        {
            if (IsDomandaINDCOM(datiPensione.SiglaCategoria) && datiPensione.Tipo == "0124")
                return true;
            return false;
        }

        public static bool IsDomandaPL(GestionePensione.DatiPensione datiPensione)
        {
            return !(IsRicostituzioneOrRiapertura(datiPensione, IsRiaperturaDomanda(datiPensione.Id)) || IsDomandaRipristinoOrRiliquidazione(datiPensione));
        }


        public static bool IsRicostituzioneOrRiaperturaFSPTPerequata(GestionePensione.DatiPensione datiPensione, bool isRiapertura, DateTime? decorrenzaOriginaria)
        {
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza == TipoAppartenenza.FS && IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
            {
                TipoFondo? fondo = GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                if (fondo == TipoFondo.FS || fondo == TipoFondo.PT)
                {
                    DateTime? dataControlloPerequate = null;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloPerequateFS", out controlloDinamico);
                    if (controlloDinamico != null)
                    {
                        dataControlloPerequate = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG);
                        if (dataControlloPerequate.HasValue && decorrenzaOriginaria.HasValue &&
                            !DataSuccessivaA(decorrenzaOriginaria.Value, dataControlloPerequate.Value))
                            return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            int index = 0;
            while (index + maxLength < str.Length)
            {
                yield return str.Substring(index, maxLength);
                index += maxLength;
            }

            yield return str.Substring(index);
        }

        /// <summary>
        /// Verifica che due floating point number sono uguali
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true se i due floating point number sono uguali</returns>
        public static bool IsDoubleEquals(double a, double b)
        {
            //return Math.Abs(a - b) < (Math.Pow(10, -cifreDecimali));
            return a == b;
        }

        public static void CloseClient(System.ServiceModel.ICommunicationObject objWS)
        {
            try
            {
                if (objWS.State != System.ServiceModel.CommunicationState.Closed &&
                   objWS.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    objWS.Close(); // may throw exception while closing
                }
                else
                {
                    objWS.Abort();
                }
            }
            catch (System.ServiceModel.CommunicationException)
            {
                objWS.Abort();
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
        }

        public static List<TipoFondo> GetListaTipoFondo_PECO_Fondi_AMG()
        {
            return new List<TipoFondo>
            {
                TipoFondo.FS,
                TipoFondo.PT
            };
        }

        public static bool IsGestioneENPALSConSedeDestinazione(GestionePensione.DatiPensione datiPensione)
        {
            return IsPolarizzazionePerGestioneENPALSAttiva(datiPensione) && IsDomandaENPALS(datiPensione.Gestione) && IsPoloPALS(datiPensione);
        }

        public static bool IsPoloPALS(GestionePensione.DatiPensione datiPensione)
        {
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SedePoloENPALS", out ctrl);
            short codiceSedePoloEnpals = 0;
            if (ctrl != null && !String.IsNullOrEmpty(ctrl.ValoreControllo))
                short.TryParse(ctrl.ValoreControllo, out codiceSedePoloEnpals);

            return datiPensione.CodiceSede == codiceSedePoloEnpals && datiPensione.CentroOperativo == 0;
        }

        public static bool IsPolarizzazionePerGestioneENPALSAttiva(GestionePensione.DatiPensione datiPensione)
        {
            return IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) ? GestioneControlliDinamici.IsPolarizzazioneSuperstitiENPALSAttiva() : GestioneControlliDinamici.IsPolarizzazioneENPALSAttiva();
        }

        public static bool IsDomandaSupplementare(GestionePensione.DatiPensione datiPensione)
        {
            //ENG - VOAUT 0001-0002-0192
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0009" || datiPensione.Tipo == "0192")) ||
                (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0009") ||
                (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021" && datiPensione.Tipo == "0009") ||
                (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0009"))
                return true;
            else
                return false;
        }

        public static bool IsDomandaInabilitaAmianto(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0168" ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.InabilitaAmianto)
                return true;
            return false;
        }

        public static bool IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && (datiPensione.Tipo == "0170" || datiPensione.Tipo == "0161"))
                return true;
            return false;
        }

        public static bool IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0170" || datiPensione.Tipo == "0161"))
                return true;
            return false;
        }

        //ENG - IOAUT 0002-0012-0045 
        public static bool IsDomandaInabilitaOrdinaria(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && (datiPensione.Tipo == "0001" || datiPensione.Tipo == "0045");
        }

        public static bool IsDomandaInabilitaNavigazione(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0010";
        }

        public static bool IsDomandaInabilitaLegge335(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0052";
        }

        public static bool IsDomandaIndirettaInabilitaLegge335(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0052";
        }

        public static bool IsDomandaInabilitaProficuoLavoro(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0047";
        }

        public static bool IsDomandaInvaliditaOrdinaria(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0001";
        }

        public static bool IsDomandaInvaliditaSpecifica(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0011";
        }

        //ENG - IOAUT 0002-0011-0045
        public static bool IsDomandaAssegnoInvaliditaOrdinario(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011" && (datiPensione.Tipo == "0001" || datiPensione.Tipo == "0045");
        }

        public static bool IsNullOrWhiteSpace(string stringa)
        {
            return String.IsNullOrEmpty(stringa) || stringa.Trim().Length == 0;
        }

        public static bool IsEnpalsManualePL(bool isDomandaENPALS, bool isRicostituzioneOrRiapertura, bool? isDatiENPALSRecuperati)
        {
            if (isDomandaENPALS && !isRicostituzioneOrRiapertura && isDatiENPALSRecuperati.HasValue && !isDatiENPALSRecuperati.Value)
                return true;

            return false;
        }

        public static bool IsPensioneInvaliditaInabilitaENPALSOrCasellario(GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (datiDanteCausa != null && !string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) &&
                (datiDanteCausa.SiglaCategoria.Trim().ToUpperInvariant() == "IOSPETT" || datiDanteCausa.SiglaCategoria.Trim().ToUpperInvariant() == "IOSPORT" ||
                (datiDanteCausa.Sede == "9933" && datiDanteCausa.SiglaCategoria.ToUpperInvariant() == "INV")))
                return true;
            return false;
        }

        public static bool IsDomandaQuota100(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0174") ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.Quota100)
                return true;

            return false;
        }

        public static bool IsDomandaSperimentaleDonna_DL_4_2019(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0176")
                return true;

            return false;
        }

        public static bool IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaSperimentaleDonna_DL_4_2019(datiPensione) || Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.SperimentaleDonna_DL_4_2019)
                return true;

            return false;
        }

        public static bool IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(GestionePensione.DatiPensione datiPensione, DateTime? dataPerfezionamentoRequisiti = null)
        {
            if (datiPensione == null)
                return false;

            if (dataPerfezionamentoRequisiti == null)
                dataPerfezionamentoRequisiti = datiPensione.DataPerfezionamentoRequisiti;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0001" &&
                dataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2019, 1, 1)) &&
                !Utility.DataStrettamenteSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2026, 12, 31)))
                return true;

            if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.AnzianitaPerLeggeBilancio2019)
                return true;

            return false;
        }

        public static bool IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(GestionePensione.DatiPensione datiPensione, DateTime? dataPerfezionamentoRequisiti = null)
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

        public static bool IsDomandaAnticipataMilitariAutomaticaINPDAP(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0001" &&
                datiPensione.SiglaCategoria.Trim() == "VOCTPS" && datiPensione.TipoFelpe != null)
                return true;

            return false;
        }

        public static void GetAAMMGGFromSettimane(short settimane, out short anni, out short mesi, out short giorni)
        {
            anni = (short)Math.Floor((decimal)settimane / 52);
            int app = settimane % 52;
            mesi = (short)Math.Floor(app / 4.333M);
            decimal app2 = app % 4.333M;
            giorni = (short)Math.Floor(app2 * 6.923M);
        }

        public static double CalcolaSettimane(int anni, int mesi, int giorni)
        {
            // Conversione semplificata: 1 anno = 365 giorni, 1 mese = 30 giorni.
            int totaleGiorni = anni * 365 + mesi * 30 + giorni;
            double settimane = (double)totaleGiorni / 7.0;
            // Arrotonda con 2 decimali
            return Math.Round(settimane, 2);
        }

        public static bool IsDomandaAnzianitaInComputo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045")
                return true;

            return false;
        }

        public static bool IsDomandaVecchiaiaInComputo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0045")
                return true;

            return false;
        }

        public static bool IsDomandaAUTAnticipataInComputo(GestionePensione.DatiPensione datiPensione, bool filtroUgualeAV)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VOAUT" && datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0045" &&
                ((filtroUgualeAV && datiPensione.CodiceTipoRichiesta == "AV") || (!filtroUgualeAV && datiPensione.CodiceTipoRichiesta != "AV")))
                return true;

            return false;
        }

        public static string GetMessageFromException(Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
                message = string.Format("{0} | {1}", message, GetMessageFromException(ex.InnerException));

            return message;
        }

        public static TipoPLPerRIC GetEnumTipoPLPerRICbyId(byte? id)
        {
            TipoPLPerRIC tipoPL = TipoPLPerRIC.Nessuno;
            if (id != null)
            {
                foreach (TipoPLPerRIC value in Enum.GetValues(typeof(TipoPLPerRIC)))
                {
                    if (value.GetHashCode() == (int)id.GetValueOrDefault())
                    {
                        tipoPL = value;
                        break;
                    }
                }
            }
            return tipoPL;
        }

        public static bool IsDomandaSO(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "SO")
                return true;
            return false;
        }

        public static DateTime GetDataElaborazionePensione(GestionePensione.DatiPensione datiPensione)
        {
            DateTime dataElaborazione = DateTime.Now;
            StatoPensione? statoPensione = GetStatoPensioneByCodice(datiPensione.StatoPensione.Value);
            if (datiPensione != null && datiPensione.DataElaborazione.HasValue &&
                (statoPensione == StatoPensione.Calcolata || statoPensione == StatoPensione.CalcolataNoWebDom || statoPensione == StatoPensione.CalcolataNoStazLavoro))
            {
                dataElaborazione = datiPensione.DataElaborazione.Value;
            }
            return dataElaborazione;
        }

        /// <summary>
        /// Verifica se la domanda è una Ricostituzione per adeguamento pro quota Casse
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns>Restituisce true se il Gruppo è 0031, il Prodotto è (0102,0302,0402) e il Tipo 0184</returns>
        public static bool IsDomandaRicostituzioneAdeguamentoProQuotaCasse(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.Gruppo == "0031" && (datiPensione.Prodotto == "0102" || datiPensione.Prodotto == "0302" || datiPensione.Prodotto == "0402")
                && datiPensione.Tipo == "0184")
                return true;

            return false;
        }


        /// <summary>
        /// •	essere GDP (gestione = 019)
        ///•	avere il campo “Benefici L.336/70” valorizzato con un valore diverso da NULL e diverso da “0” (cifra zero)
        ///•	non essere una PL di REV(G/P/T = 3 / 21 / XX)
        ///•	può essere una ricostituzione per motivi contributivi G/P/T 
        ///•	non deve essere un gruppo 0051 (bloccato il gruppo su utility)
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="rMSSenzaLegge33670QA"></param>
        /// <returns></returns>
        public static bool IsFlussoNoteDiDebito(GestionePensione.DatiPensione datiPensione, decimal? rMSSenzaLegge33670QA)
        {
            if (datiPensione == null)
                return false;

            if ((IsDomandaINPDAP(datiPensione.Gestione) && rMSSenzaLegge33670QA.GetValueOrDefault() != 0 && datiPensione.Gruppo != "0051" &&
                (!IsDomandaReversibilita(datiPensione) || IsRicostituzione_MotiviContributivi(datiPensione))) || IsDomandaMiglioramentiContrattuali(datiPensione))
                return true;

            return false;
        }

        public static bool IsFlusso6Scatti(GestionePensione.DatiPensione datiPensione, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiPensione == null || recordDatiFondoINPDAP == null)
                return false;

            if ((datiPensione.Gestione == "019" && (datiPensione.Fondo == "001" || datiPensione.Fondo == "002" || datiPensione.Fondo == "003")) &&
                ((recordDatiFondoINPDAP.ImportoSingolaRata != null && recordDatiFondoINPDAP.ImportoSingolaRata != 0 && recordDatiFondoINPDAP.ImportoSingolaRata != 0.00m) &&
                (recordDatiFondoINPDAP.NumeroRate != null && recordDatiFondoINPDAP.NumeroRate != 0)) &&
                (IsDomandaPL(datiPensione) || ((IsRicostituzione_MotiviContributivi(datiPensione) || IsRiaperturaDomanda(datiPensione.Id)) && Utility.AbilitaFlussoSeiScatti())) &&
                (datiPensione.Gruppo != "0051"))
                return true;

            return false;
        }

        public static bool IsFlussoEquoIndennizzo(GestionePensione.DatiPensione datiPensione, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiPensione == null || recordDatiFondoINPDAP == null)
                return false;
            if (recordDatiFondoINPDAP.ImpEquoInd != null && recordDatiFondoINPDAP.ImpEquoInd != 0 && recordDatiFondoINPDAP.EnteEquoInd != null && recordDatiFondoINPDAP.EnteEquoInd != "")
                return true;

            return false;
        }

        public static bool IsFlussoIndennitaSpeciale(GestionePensione.DatiPensione datiPensione, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiPensione == null || recordDatiFondoINPDAP == null)
                return false;
            if (recordDatiFondoINPDAP.CodInd != null && recordDatiFondoINPDAP.ImpInd != null && recordDatiFondoINPDAP.ImpInd != 0)
                return true;

            return false;
        }

        public static bool IsSperimentazioneSediINPDAP()
        {
            return (ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP"] != null &&
                ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP"] == "SI");
        }

        public static bool IsRegimeSediINPDAP()
        {
            return (ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP_Final"] != null &&
                ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP_Final"] == "SI");
        }

        public static bool AbilitaQuadroDelegheTuteleINPDAP()
        {
            return (ConfigurationManager.AppSettings["AbilitaQuadroDelegheTuteleINPDAP"] != null &&
                ConfigurationManager.AppSettings["AbilitaQuadroDelegheTuteleINPDAP"] == "SI");
        }

        public static bool AbilitaFlussoNoteDiDebito()
        {
            return (ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] != null &&
                ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] == "SI");
        }

        public static bool AbilitaFlussoCalcoloQuote()
        {
            return (ConfigurationManager.AppSettings["AbilitaFlussoCalcoloQuote"] != null &&
                ConfigurationManager.AppSettings["AbilitaFlussoCalcoloQuote"] == "SI");
        }

        public static bool AbilitaFlussoSeiScatti()
        {
            return (ConfigurationManager.AppSettings["AbilitaFlussoSeiScatti"] != null &&
                ConfigurationManager.AppSettings["AbilitaFlussoSeiScatti"] == "SI");
        }

        public static bool DisabilitaSalvaAnagrafica()
        {
            return (ConfigurationManager.AppSettings["DisabilitaSalvaAnagrafica"] != null &&
                ConfigurationManager.AppSettings["DisabilitaSalvaAnagrafica"] == "SI");
        }

        public static bool AbilitaControlloLIPE()
        {
            return (ConfigurationManager.AppSettings["AbilitaControlloLIPE"] != null &&
                ConfigurationManager.AppSettings["AbilitaControlloLIPE"] == "SI");
        }

        public static bool IsDomandaQuota102(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0185") ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.Quota102)
                return true;

            return false;
        }

        public static bool IsDomandaRicostituzioneAdeguamentoProQuotaCasse(string gruppo, string prodotto, string tipo)
        {
            if (gruppo == "0031" && (prodotto == "0102" || prodotto == "0302" || prodotto == "0402") && tipo == "0184")
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

        public static bool IsRenditaFacoltativa(GestionePensione.DatiPensione datiPensione)
        {
            return !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && (datiPensione.SiglaCategoria.Trim() == "VOBIS" || datiPensione.SiglaCategoria.Trim() == "IOBIS");
        }

        public static bool IsRenditaCasalinghe(GestionePensione.DatiPensione datiPensione)
        {
            return !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && (datiPensione.SiglaCategoria.Trim() == "VMP" || datiPensione.SiglaCategoria.Trim() == "IMP");
        }

        public static bool IsDomandaRendita(string siglaCategoria)
        {
            if (!string.IsNullOrEmpty(siglaCategoria) && (siglaCategoria.Trim() == "VMP" || siglaCategoria.Trim() == "IMP" || siglaCategoria.Trim() == "VOBIS" || siglaCategoria.Trim() == "IOBIS"))
                return true;

            return false;
        }

        public static bool IsDomandaIMP(GestionePensione.DatiPensione datiPensione)
        {
            return !string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim() == "IMP";
        }

        public static bool IsDomandaVOST(string siglaCategoria)
        {
            return !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "VOST";
        }

        public static bool IsDomandaPSO(string siglaCategoria)
        {
            return !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "PSO";
        }

        public static bool IsDomandaPMO(string siglaCategoria)
        {
            return !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Trim().ToUpperInvariant() == "PMO";
        }

        public static bool IsDomandaSupplementarePLorRIC(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (IsDomandaSupplementare(datiPensione))
                return true;
            else if (IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                if (datiPensione.NaturaPensione.Substring(0, 1) == "5")
                    return true;
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la domanda è una VESO33 - Gestione pubblica     
        /// </summary>
        public static bool IsDomandaVESO33_DAP(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VESO33")
            {
                if (datiPensione.GetFiltro() == "DAP")
                    return true;

                if (Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicVESO33FiltroDAP)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Il metodo verifica se la domanda è una spacchettata 024 - PT  
        /// </summary>
        public static bool IsDomandaSpacchettamento024(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloSpacchettate024", out controlloDinamico);
            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamicoSedi = null;
            BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneCalcoloReingSediSpacchettatePT", out controlloDinamicoSedi);
            DateTime? controlloDinamicoDataControllo = Utility.DataFromString(controlloDinamico.ValoreControllo, FormatoData.AAAAmmGG);

            if (tipoAppartenenza == TipoAppartenenza.FS)
            {
                if (controlloDinamicoSedi != null && (String.IsNullOrEmpty(controlloDinamicoSedi.ValoreControllo) || controlloDinamicoSedi.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == GetCodiceSedeLavorazione(datiPensione, isRiaperturaDomanda).ToString().PadLeft(4, '0'))))
                {
                    //Una domanda deve rientrare nel flusso delle spacchettate024 se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico
                    TipoFondo? fondo = GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                    if (fondo.HasValue && fondo.Value == TipoFondo.PT && IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                        controlloDinamicoDataControllo.HasValue && Utility.DataSuccessivaA(datiPensione.DataAcquisizione.GetValueOrDefault(), controlloDinamicoDataControllo.GetValueOrDefault()))
                        return true;
                }
            }
            return false;
        }

        public static TipoAnte96? IsDomandaAnte96(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiPensione datiPensioneUI, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isRiaperturaDomanda, DateTime? dataDaConfrontare = null)
        {
            List<string> sigleCategorieAmmesse = new List<string>();
            sigleCategorieAmmesse.AddRange(getCategorieDipendenti());
            sigleCategorieAmmesse.AddRange(getCategorieGestioniSpeciali());
            DateTime dataEstremoSuperiore = new DateTime(1995, 12, 31);

            if (sigleCategorieAmmesse.Contains(datiPensione.SiglaCategoria.Trim()))
            {
                if (!dataDaConfrontare.HasValue)
                {
                    if (datiPensione.SiglaCategoria.Trim().StartsWith("V") || datiPensione.SiglaCategoria.Trim().StartsWith("I") || IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPMO(datiPensione.SiglaCategoria))
                        dataDaConfrontare = datiPensioneUI != null ? (datiPensioneUI.DecorrenzaOriginaria.HasValue ? datiPensioneUI.DecorrenzaOriginaria : null) : null;
                    else if (IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                        dataDaConfrontare = datiDanteCausa != null ? (datiDanteCausa.DecorrenzaPensione.HasValue ? datiDanteCausa.DecorrenzaPensione.Value : (DateTime?)null) : (DateTime?)null;
                    else if (datiPensione.SiglaCategoria.Trim().StartsWith("S") && datiPensione.MaxDecDatiCalcoloAnte96.HasValue)
                        dataDaConfrontare = datiPensione.MaxDecDatiCalcoloAnte96.Value;
                    else if (datiPensione.SiglaCategoria.Trim().StartsWith("S"))
                        dataDaConfrontare = datiPensioneUI != null ? (datiPensioneUI.DecorrenzaOriginaria.HasValue ? datiPensioneUI.DecorrenzaOriginaria : null) : null;
                }
                if (dataDaConfrontare.HasValue && DataSuccessivaA(dataEstremoSuperiore, dataDaConfrontare.Value))
                {
                    var isAnte96Prestabilita = Utility.isAnte96Prestabilita(datiPensione, datiDanteCausa);
                    if (isAnte96Prestabilita != null)
                        return isAnte96Prestabilita;

                    List<string> sigleCategorieDipendenti = getCategorieDipendenti();
                    List<string> sigleCategorieGestioniSpeciali = getCategorieGestioniSpeciali();
                    if (sigleCategorieDipendenti.Contains(datiPensione.SiglaCategoria.Trim()))
                    {
                        if (IsDomandaSupplementarePLorRIC(datiPensione, isRiaperturaDomanda))
                        {
                            if (DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1981, 05, 30)))
                                return TipoAnte96.Ante96Retributive;
                            else
                                return TipoAnte96.Ante96Contributive;
                        }
                        else
                        {
                            if (DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1976, 07, 31)))
                                return TipoAnte96.Ante96Retributive;
                            else if (DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1968, 04, 30)) && DataSuccessivaA(new DateTime(1976, 07, 31), dataDaConfrontare.Value))
                                return TipoAnte96.Ante96Miste;
                            else if (DataSuccessivaA(new DateTime(1968, 04, 30), dataDaConfrontare.Value))
                                return TipoAnte96.Ante96Contributive;
                        }
                    }
                    else if (sigleCategorieGestioniSpeciali.Contains(datiPensione.SiglaCategoria.Trim()))
                    {
                        if (DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1982, 01, 01)))
                            return TipoAnte96.Ante96Miste;
                        else
                            return TipoAnte96.Ante96Contributive;
                    }
                }
            }
            return null;
        }

        public static bool IsDomandaAnte96Generica(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isRiaperturaDomanda, DateTime? dataDaConfrontare = null)
        {
            DateTime dataEstremoSuperiore = new DateTime(1995, 12, 31);

            if (!dataDaConfrontare.HasValue)
            {
                if (datiPensione.SiglaCategoria.Trim().StartsWith("V") || datiPensione.SiglaCategoria.Trim().StartsWith("I") || IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPMO(datiPensione.SiglaCategoria))
                    dataDaConfrontare = datiPensione != null ? (datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : null) : null;
                else if (IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                    dataDaConfrontare = datiDanteCausa != null ? (datiDanteCausa.DecorrenzaPensione.HasValue ? datiDanteCausa.DecorrenzaPensione.Value : (DateTime?)null) : (DateTime?)null;
                else if (datiPensione.SiglaCategoria.Trim().StartsWith("S") && datiPensione.MaxDecDatiCalcoloAnte96.HasValue)
                    dataDaConfrontare = datiPensione.MaxDecDatiCalcoloAnte96.Value;
                else if (datiPensione.SiglaCategoria.Trim().StartsWith("S"))
                    dataDaConfrontare = datiPensione != null ? (datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : null) : null;
            }

            if (dataDaConfrontare.HasValue && DataSuccessivaA(dataEstremoSuperiore, dataDaConfrontare.Value))
            {
                return true;
            }

            return false;
        }
        public static bool mostraQuotaAnte96(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isRiaperturaDomanda, TipoAnte96? tipoAnte96)
        {
            if (tipoAnte96 != null)
            {
                DateTime? dataDaConfrontare = null;

                if (datiPensione.SiglaCategoria.Trim().StartsWith("V") || datiPensione.SiglaCategoria.Trim().StartsWith("I") || IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPMO(datiPensione.SiglaCategoria))
                    dataDaConfrontare = datiPensione != null ? (datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : null) : null;
                else if (IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                    dataDaConfrontare = datiDanteCausa != null ? (datiDanteCausa.DecorrenzaPensione.HasValue ? datiDanteCausa.DecorrenzaPensione.Value : (DateTime?)null) : (DateTime?)null;
                else if (datiPensione.SiglaCategoria.Trim().StartsWith("S") && datiPensione.MaxDecDatiCalcoloAnte96.HasValue)
                    dataDaConfrontare = datiPensione.MaxDecDatiCalcoloAnte96.Value;
                else if (datiPensione.SiglaCategoria.Trim().StartsWith("S"))
                    dataDaConfrontare = datiPensione != null ? (datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : null) : null;

                List<string> sigleCategorieDipendenti = getCategorieDipendenti();
                List<string> sigleCategorieGestioniSpeciali = getCategorieGestioniSpeciali();
                if (sigleCategorieDipendenti.Contains(datiPensione.SiglaCategoria.Trim()))
                {
                    if (tipoAnte96 == TipoAnte96.Ante96Retributive && DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1993, 02, 28)))
                    {
                        return true;
                    }
                }
                else if (sigleCategorieGestioniSpeciali.Contains(datiPensione.SiglaCategoria.Trim()))
                {
                    if (tipoAnte96 == TipoAnte96.Ante96Miste && DataStrettamenteSuccessivaA(dataDaConfrontare.Value, new DateTime(1993, 02, 28)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<string> getCategorieDipendenti()
        {
            List<string> listaCategorieDipendenti = new List<string> { "VO", "IO", "SO", "VOP", "IOP", "SOP", "VOMIN", "SOMIN", "PMO" };
            return listaCategorieDipendenti;
        }

        public static List<string> getCategorieGestioniSpeciali()
        {
            List<string> listaCategorieGestioniSpeciali = new List<string> { "VOART", "IOART", "SOART", "VOCOM", "IOCOM", "SOCOM", "VR", "IR", "SR" };
            return listaCategorieGestioniSpeciali;
        }


        public static bool IsPannelloSupplementiAnte96(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiPensione datiPensioneUI, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isRiaperturaDomanda)
        {
            List<string> sigleCategorieAmmesse = new List<string>();
            sigleCategorieAmmesse.AddRange(getCategorieDipendenti());
            sigleCategorieAmmesse.AddRange(getCategorieGestioniSpeciali());
            GestioneControlliDinamici.ControlloDinamico cdEstremoSuperioreSupplAnte96 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EstremoSuperioreSupplAnte96", out cdEstremoSuperioreSupplAnte96);
            DateTime dataEstremoSuperiore = cdEstremoSuperioreSupplAnte96 != null ? (DateTime)Utility.DataFromString(cdEstremoSuperioreSupplAnte96.ValoreControllo, Utility.FormatoData.AAAAmmGG) : new DateTime(1993, 12, 31);

            if (sigleCategorieAmmesse.Contains(datiPensione.SiglaCategoria.Trim()))
            {
                DateTime? dataDaConfrontare = null;
                if (datiPensione.SiglaCategoria.Trim().StartsWith("V") || datiPensione.SiglaCategoria.Trim().StartsWith("I") || IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPMO(datiPensione.SiglaCategoria))
                    dataDaConfrontare = datiPensioneUI != null ? (datiPensioneUI.DecorrenzaOriginaria.HasValue ? datiPensioneUI.DecorrenzaOriginaria : null) : null;
                else if (IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                {
                    dataDaConfrontare = datiDanteCausa != null ? (datiDanteCausa.DecorrenzaPensione.HasValue ? datiDanteCausa.DecorrenzaPensione : null) : null;
                    GestioneControlliDinamici.ControlloDinamico cdEstremoSuperioreSupplAnte96Reversibilita = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EstremoSuperioreSupplAnte96Reversibilita", out cdEstremoSuperioreSupplAnte96Reversibilita);
                    dataEstremoSuperiore = cdEstremoSuperioreSupplAnte96Reversibilita != null ? (DateTime)Utility.DataFromString(cdEstremoSuperioreSupplAnte96Reversibilita.ValoreControllo, Utility.FormatoData.AAAAmmGG) : new DateTime(1995, 12, 31);
                }
                else if (datiPensione.SiglaCategoria.Trim().StartsWith("S") && datiPensione.MaxDecDatiCalcoloAnte96.HasValue)
                {
                    dataDaConfrontare = datiPensione.MaxDecDatiCalcoloAnte96.Value;
                    GestioneControlliDinamici.ControlloDinamico cdEstremoSuperioreSupplAnte96Reversibilita = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EstremoSuperioreSupplAnte96Reversibilita", out cdEstremoSuperioreSupplAnte96Reversibilita);
                    dataEstremoSuperiore = cdEstremoSuperioreSupplAnte96Reversibilita != null ? (DateTime)Utility.DataFromString(cdEstremoSuperioreSupplAnte96Reversibilita.ValoreControllo, Utility.FormatoData.AAAAmmGG) : new DateTime(1995, 12, 31);

                }

                if (dataDaConfrontare.HasValue && DataSuccessivaA(dataEstremoSuperiore, dataDaConfrontare.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public static TipoAnte96? isAnte96Prestabilita(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (datiPensione.SiglaCategoria.Trim() == "SR")
            {
                DateTime? dataEstremoSuperiore = null;
                DateTime? dataDaConfrontare = null;
                if (IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                {
                    dataDaConfrontare = datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null;
                    dataEstremoSuperiore = new DateTime(1970, 01, 01);
                }
                else if (IsDomandaPensioneIndirettaOrRicostituzione(datiPensione, datiDanteCausa))
                {
                    dataDaConfrontare = datiDanteCausa != null ? datiDanteCausa.DataMorte : null;
                    dataEstremoSuperiore = new DateTime(1969, 05, 02);
                }
                else
                    return null;

                if (dataDaConfrontare.HasValue && DataSuccessivaA(dataEstremoSuperiore.Value, dataDaConfrontare.Value))
                {
                    return TipoAnte96.Ante96Contributive;
                }

            }
            return null;
        }

        public static bool IsDomandaAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (!String.IsNullOrEmpty(datiPensione.Prodotto) && datiPensione.Prodotto.Trim() == "0001")
                return true;

            return false;
        }

        public static bool IsDomandaVecchiaiaPL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001")
                return true;

            return false;
        }

        /// <summary>
        /// Ritorna null se il flusso non è abilitato da chiave o non è presente l'attività, in quel caso il flusso attuale di acquisizione non subisce variazioni. 
        /// False se è presente l'attività ma non è il tipo di domanda giusto, in quel caso si darà uno scarto all'acquisizione.
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="isAttivita168Aperta"></param>
        /// <returns></returns>
        public static bool? IsDomandaDaAutomatizzare(GestionePensione.DatiPensione datiPensione, bool isTrasformazioneDaAutomatizzare, out TipoAutomazione? tipoAutomazione, out string messaggio)
        {
            tipoAutomazione = null;
            messaggio = null;
            bool? esito = null;
            if (datiPensione == null)
                return null;

            //possibili messaggi di scarto
            string scarto1 = "Prodotto e gestione non rientranti fra quelle oggetto di automazione";
            string scarto2 = "Prodotto non rientrante fra quelli oggetto di automazione";
            string scarto3 = "Gestione non rientrante fra quelle oggetto di automazione";

            //imposto di default lo scarto più generico
            messaggio = scarto1;

            if (AbilitaFlussoCalcoloQuote())
            {
                //per prima cosa valuto se è una trasformazione così da non rischiare di ricadere negli altri casi 
                if (isTrasformazioneDaAutomatizzare)
                {
                    if ((datiPensione.Gestione == "001" || datiPensione.Gestione == "002" || datiPensione.Gestione == "003" || datiPensione.Gestione == "004") && datiPensione.Fondo == "001"
                    && datiPensione.IndConvInt != true)
                    {
                        tipoAutomazione = TipoAutomazione.Trasformazioni;
                        messaggio = string.Empty;
                        return true;
                    }
                    else
                        messaggio = scarto3;

                    esito = false;

                }

                if (IsRicostituzione_SupplementoAutomatico(datiPensione) && ((datiPensione.Gestione == "001" || datiPensione.Gestione == "002" || datiPensione.Gestione == "003" || datiPensione.Gestione == "004") && datiPensione.Fondo == "001"))
                {
                    tipoAutomazione = TipoAutomazione.Supplementi;
                    messaggio = string.Empty;
                    return true;
                }
                else
                {
                    //se è vera la prima, allora è falsa la seconda -> scarto di gestione
                    if (IsRicostituzione_SupplementoAutomatico(datiPensione))
                        messaggio = scarto3;
                    //se è vera la seconda, allora è falsa la prima -> scarto di prodotto
                    else if ((datiPensione.Gestione == "001" || datiPensione.Gestione == "002" || datiPensione.Gestione == "003" || datiPensione.Gestione == "004") && datiPensione.Fondo == "001")
                        messaggio = scarto2;

                    esito = false;
                }

                if (IsDomandaVecchiaiaPL(datiPensione) && ((datiPensione.Gestione == "001" || datiPensione.Gestione == "002" || datiPensione.Gestione == "003" || datiPensione.Gestione == "004") && datiPensione.Fondo == "001"))
                {
                    tipoAutomazione = TipoAutomazione.Vecchiaia;
                    messaggio = string.Empty;
                    return true;
                }
                else
                {
                    //se è vera la prima, allora è falsa la seconda -> scarto di gestione
                    if (IsDomandaVecchiaiaPL(datiPensione))
                        messaggio = scarto3;
                    //se è vera la seconda, allora è falsa la prima -> scarto di prodotto
                    else if ((datiPensione.Gestione == "001" || datiPensione.Gestione == "002" || datiPensione.Gestione == "003" || datiPensione.Gestione == "004") && datiPensione.Fondo == "001")
                        messaggio = scarto2;

                    esito = false;
                }


            }
            return esito;
        }

        public static bool IsRequisitiCalcoloQuote(GestionePensione.DatiPensione datiPensione, GestioneCalcolo.DatiCalcoloContributivo[] listaCalcoloContributivo, Entity.DatiSupplementi[] listaSupplementi)
        {
            if (datiPensione == null)
                return false;

            var listaSupplementiOrdered = listaSupplementi != null ? listaSupplementi.OrderByDescending(x => x.DecorrenzaSupplemento).ToList() : null;

            if (DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 1, 1)) || datiPensione.TipoCalcolo == 1 || (listaCalcoloContributivo != null && listaCalcoloContributivo.Length > 0) || (listaSupplementiOrdered != null && listaSupplementiOrdered.Count > 0 && listaSupplementiOrdered.First().DecorrenzaSupplemento.HasValue && DataSuccessivaA(listaSupplementiOrdered.First().DecorrenzaSupplemento.Value, new DateTime(2012, 1, 1))))
                return true;

            return false;
        }

        public static bool IsPensioniOvunqueAttiva(Utility.TipoAppartenenza? tipoAppartenenza)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("PensioniOvunque", out controlloDinamico);

            if (tipoAppartenenza.HasValue && controlloDinamico != null && !String.IsNullOrEmpty(controlloDinamico.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamico.ValoreControllo.Trim()))
            {
                DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza.Value);
                DateTime? dataInizioPensioniOvunque = Utility.DataFromString(controlloDinamico.ValoreControllo.Trim(), Utility.FormatoData.AAAAmmGG);
                if (Utility.DataSuccessivaA(dataSistema, dataInizioPensioniOvunque.GetValueOrDefault()))
                    return true;
            }

            return false;
        }

        public static bool isRicostituzioneOrRiaperturaPolarizzata(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && datiPensione.CodiceSedeDestinazione.HasValue)
                return true;

            return false;
        }

        //Lavoratrici che assistono persone con handicap in situazione di gravità ai sensi dell’articolo 3, comma 3, della legge 5 febbraio 1992, n. 104 
        //PL Automatiche(KWA) e Manuali(KXM)
        public static bool IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(GestionePensione.DatiPensione datiPensione, bool filtroUgualeKWA, bool filtroUgualeKXM)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0190" &&
                ((filtroUgualeKWA && datiPensione.CodiceTipoRichiesta == "KW") || (filtroUgualeKXM && datiPensione.CodiceTipoRichiesta == "KX")))
                return true;

            return false;
        }

        //Lavoratrici che assistono persone con handicap in situazione di gravità ai sensi dell’articolo 3, comma 3, della legge 5 febbraio 1992, n. 104 
        //RIC Automatiche(KWA) e Manuali(KXM)
        public static bool IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKWA) || Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKXM)
                return true;

            return false;
        }

        //Lavoratrici con riconoscimento invalidità civile di grado almeno pari al 74% 
        //PL Automatiche(KYA) e Manuali(KZM)
        public static bool IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(GestionePensione.DatiPensione datiPensione, bool filtroUgualeKYA, bool filtroUgualeKZM)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0190" &&
                ((filtroUgualeKYA && datiPensione.CodiceTipoRichiesta == "KY") || (filtroUgualeKZM && datiPensione.CodiceTipoRichiesta == "KZ")))
                return true;

            return false;
        }

        //Lavoratrici con riconoscimento invalidità civile di grado almeno pari al 74% 
        //RIC Automatiche(KYA) e Manuali(KZM)
        public static bool IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKYA) || Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKZM)
                return true;

            return false;
        }

        //Lavoratrici licenziate o dipendenti da imprese per le quali è attivo un tavolo di crisi aziendale ai sensi dell’art. 1 comma 852 l. 296/2006
        //PL Automatiche(KUA) e Manuali(KVM)
        public static bool IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(GestionePensione.DatiPensione datiPensione, bool filtroUgualeKUA, bool filtroUgualeKVM)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0190" &&
                ((filtroUgualeKUA && datiPensione.CodiceTipoRichiesta == "KU") || (filtroUgualeKVM && datiPensione.CodiceTipoRichiesta == "KV")))
                return true;

            return false;
        }

        //Lavoratrici licenziate o dipendenti da imprese per le quali è attivo un tavolo di crisi aziendale ai sensi dell’art. 1 comma 852 l. 296/2006
        //RIC Automatiche(KUA) e Manuali(KVM)
        public static bool IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKUA) || Utility.GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicOpzioneDonnaFiltroKVM)
                return true;

            return false;
        }

        public static short GetCodiceSedeLavorazione(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            short codiceSedeLavorazione = datiPensione.CodiceSede;
            if (Utility.IsPensioniOvunqueAttiva(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione)) && (Utility.IsRicostituzione(datiPensione.Gruppo) || isRiapertura) && datiPensione.CodiceSedeGP1ALZ6.HasValue
                && !Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione, isRiapertura))
            {
                codiceSedeLavorazione = datiPensione.CodiceSedeGP1ALZ6.GetValueOrDefault();

                //ENG - Implementazione Meta Processo        
                GestioneControlliDinamici.ControlloDinamico ctrlMetaProcesso = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrlMetaProcesso);
                if (ctrlMetaProcesso != null && !String.IsNullOrEmpty(ctrlMetaProcesso.ValoreControllo) && ctrlMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                {
                    if (datiPensione.CodiceSedeLavorazione.HasValue && datiPensione.CodiceSedeLavorazione.Value != datiPensione.CodiceSedeGP1ALZ6.Value)
                        codiceSedeLavorazione = datiPensione.CodiceSedeLavorazione.Value;
                }
            }

            return codiceSedeLavorazione;
        }

        public static byte GetCentroOperativoLavorazione(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            byte centroOperativoLavorazione = datiPensione.CentroOperativo.GetValueOrDefault();
            //if (Utility.IsPensioniOvunqueAttiva(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione)) && (Utility.IsRicostituzione(datiPensione.Gruppo) || isRiapertura) && datiPensione.CentroOperativoGP1ALZ6.HasValue
            //    && !Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione))
            //{
            //    centroOperativoLavorazione = datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault();
            //}

            return centroOperativoLavorazione;
        }

        public static bool IsDomandaBanc_91_95(GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaOriginaria, DateTime? decDanteCausa)
        {
            DateTime? date = null;
            DateTime ante = new DateTime(1995, 12, 31);
            DateTime post = new DateTime(1991, 01, 01);

            if (datiPensione != null && IsDomandaBancari(datiPensione.SiglaCategoria))
            {
                if (IsDomandaReversibilita(datiPensione) && decDanteCausa != null)
                    date = decDanteCausa.Value;
                else if (decorrenzaOriginaria != null)
                    date = decorrenzaOriginaria.Value;
            }

            if (date != null && Utility.DataSuccessivaA(date.Value, post) && Utility.DataSuccessivaA(ante, date.Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Il metodo indentifica le domande anticipate flessibili
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaAnticipataFlessibile(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0189") ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.AnticipataFlessibile)
                return true;

            return false;
        }

        //ENG - Memo 123/2024 
        public static bool IsDomandaAnticipataFlessibileLeggeBilancio2024(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0201") ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicAnticipataFlessibileLeggeBilancio2024)
                return true;

            return false;
        }

        //ENG - Memo 39/2023
        public static bool IsRicTrfAOI_Inabilita(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiEliminazione datiEliminazione)
        {
            if (datiPensione == null || datiEliminazione == null)
                return false;

            if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && !String.IsNullOrEmpty(datiPensione.SiglaCategoria.Trim())
                && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))
                && (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IO" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IR" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOART" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOCOM")
                && (datiEliminazione.CodiceMotivo == 1 || datiEliminazione.CodiceMotivo == 4))
                return true;

            return false;
        }

        //ENG - Memo 48_2023
        public static bool IsTitolareResidente_Cittadino_Bulgaria(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici)
        {
            if (datiPensione == null || datiAnagrafici == null)
                return false;

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoMemo48_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo48_2023", out controlloDinamicoMemo48_2023);

            if (controlloDinamicoMemo48_2023 != null && !String.IsNullOrEmpty(controlloDinamicoMemo48_2023.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamicoMemo48_2023.ValoreControllo.Trim())
                && controlloDinamicoMemo48_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsRicostituzione(datiPensione.Gruppo))
                {
                    string codiceStatoBulgaria = "Z104";
                    string cittadinanzaTitolare = !String.IsNullOrEmpty(datiAnagrafici.Cittadinanza) ? datiAnagrafici.Cittadinanza.Trim().ToUpperInvariant() : "";
                    string residenzaTitolare = !String.IsNullOrEmpty(datiAnagrafici.CodiceComuneResidenza) ? datiAnagrafici.CodiceComuneResidenza.Trim().ToUpperInvariant() : "";
                    if (cittadinanzaTitolare == codiceStatoBulgaria && residenzaTitolare == codiceStatoBulgaria)
                        return true;
                }
            }

            return false;
        }

        public static bool IsDomandaSOPGI(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return false;
            if (categoria.Trim().ToUpperInvariant() == "SOPGI")
                return true;
            return false;
        }

        //ENG - Spacchettate SOPGI Post 07/2022
        public static bool IsDomandaSpacchettamentoSOPGIPost072022(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            if (datiPensione == null || danteCausa == null || String.IsNullOrEmpty(datiPensione.SiglaCategoria) || !danteCausa.DataMorte.HasValue)
                return false;

            if (IsDomandaSOPGI(datiPensione.SiglaCategoria) && ((Utility.DataSuccessivaA(danteCausa.DataMorte.GetValueOrDefault(), new DateTime(2022, 7, 1))) ||
                (IsRicostituzione(datiPensione.Gruppo) && !string.IsNullOrEmpty(datiPensione.GP1AV91B) && datiPensione.GP1AV91B == "2")))
                return true;

            return false;
        }

        //ENG  - Spacchettate SOPGI
        public static int CalcolaCessazioneMassimaAventeDiritto(char? siglaFamiliare, DateTime? dataNascita)
        {
            int cessazioneMassima = 0;

            if (!siglaFamiliare.HasValue || !dataNascita.HasValue)
                return 999999;

            switch (siglaFamiliare)
            {
                case 'M':
                    Int32.TryParse(dataNascita.Value.AddYears(18).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
                case 'S':
                    Int32.TryParse(dataNascita.Value.AddYears(21).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
                case 'U':
                    Int32.TryParse(dataNascita.Value.AddYears(26).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
                case 'N':
                    Int32.TryParse(dataNascita.Value.AddYears(18).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
                case 'Z':
                    Int32.TryParse(dataNascita.Value.AddYears(21).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
                case 'W':
                    Int32.TryParse(dataNascita.Value.AddYears(26).AddMonths(1).ToString("yyyyMM"), out cessazioneMassima);
                    break;
            }

            return cessazioneMassima;

        }

        /////////////// <summary>
        /////////////// Controlla se è presente almeno una occorrenza duplicata all'interno della lista
        /////////////// </summary>
        /////////////// <param name="stringList">Lista di stringhe</param>
        /////////////// <returns>false se sono presenti dei duplicati, true altrimenti</returns>
        ////////////public static bool IsStringDuplicate(List<String> stringList)
        ////////////{
        ////////////    if (stringList == null || stringList.Count == 0)
        ////////////        return false;
        ////////////    else
        ////////////    {
        ////////////        List<string> ListaDuplicati = stringList.FindAll(delegate(string s1)
        ////////////        {
        ////////////            return stringList.FindAll(delegate(string s2)
        ////////////            {
        ////////////                return s1 == s2;
        ////////////            }).Count() > 1;
        ////////////        }).Distinct().ToList();

        ////////////        if (ListaDuplicati.Count > 0)
        ////////////            return true;
        ////////////        else
        ////////////            return false;
        ////////////    }
        ////////////}

        public static bool IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, DateTime? dataAssunzioneCarico)
        {
            if (IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (IsDomandaPSO(datiPensione.SiglaCategoria) || IsDomandaPMO(datiPensione.SiglaCategoria)))
                return true;

            if ((IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && dataAssunzioneCarico.HasValue && IsDomandaDAIAnte2003(datiPensione, dataAssunzioneCarico.Value)))
                return true;

            return false;

        }
        public static bool IsDomandaDAIAnte2003(GestionePensione.DatiPensione datiPensione, DateTime dataAssunzioneCarico)
        {
            if ((datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "VDAI" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SDAI" || datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IDAI") &&
                (dataAssunzioneCarico.Month < 12 && dataAssunzioneCarico.Year <= 2003))
                return true;
            else

                return false;
        }

        /// <summary>
        /// Il metodo indentifica le domande anticipate flessibili con opzione al contributivo
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsDomandaAnticipataFlessibileOpzioneContributivo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0195")
                || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicAnticipateFlessibileOpzioneContributivo)
                return true;

            return false;
        }

        //ENG - Memo 123/2024 
        public static bool IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0202")
                || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicAnticipataFlessibileOpzioneContributivoLeggeBilancio2024)
                return true;

            return false;
        }

        //ENG - Memo 32_a/2018
        public static bool IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) && Utility.IsRicostituzione_MotiviContributivi(datiPensione)
                && datiPensione.Tipo == "0193")
                return true;

            return false;
        }

        //ENG - RIC CONCESSIONE ALTRA PENSIONE
        public static bool IsRicostituzioneConcessioneAltraPensione(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && Utility.IsRicostituzione(datiPensione.Gruppo) && (datiPensione.Prodotto == "0109" || datiPensione.Prodotto == "0309" || datiPensione.Prodotto == "0409")
                && datiPensione.Tipo == "0130")
                return true;

            return false;
        }

        public static bool IsCTPSDocenti(string gestione, string fondo)
        {
            if (!String.IsNullOrEmpty(gestione) && !String.IsNullOrEmpty(fondo) && gestione == "019" && fondo == "002")
                return true;

            return false;
        }

        public static bool IsCTPSPrivilegio(GestionePensione.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && ((datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "IOCTPS" && datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0046") ||
               (datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOCTPS" && datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0046")))
                return true;

            return false;
        }

        public static bool IsSchedaPrivilegioVisible(GestionePensione.DatiPensione datiPensione)
        {
            if (IsCTPSPrivilegio(datiPensione) || IsDomandaRicPensioneOrdinariaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneInabilitaCambioPrivilegio(datiPensione) || IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(datiPensione)
                 || (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(0, 1) == "7"))
                return true;

            return false;
        }



        public static bool IsDomandaRiliquidazioneAOI(GestionePensione.DatiPensione datiPensione)
        {

            if (datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0322" && datiPensione.Tipo == "0023")
                return true;

            return false;
        }

        public static bool IsDomandaRicPensioneOrdinariaCambioPrivilegio(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0127" && datiPensione.Tipo == "0001";
        }

        public static bool IsDomandaRicPensioneInabilitaCambioPrivilegio(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0327" && datiPensione.Tipo == "0019";
        }

        public static bool IsDomandaRicPensioneIndirettaOrdinariaCambioPrivilegio(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0427" && datiPensione.Tipo == "0001";
        }

        public static bool IsDomandaRicPensioneIndirettaInabilitaCambioPrivilegio(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0427" && datiPensione.Tipo == "0019";
        }

        //ENG - Memo 116/2025
        public static bool IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVOAUT(datiPensione.SiglaCategoria) && ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0201" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("GSE"))
                || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE))
                return true;

            return false;
        }

        public static bool IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVOAUT(datiPensione.SiglaCategoria) && ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("GSE"))
                || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicVOAUTAnticipataTipoContributivoFiltroGSE))
                return true;

            return false;
        }

        public static bool IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVOAUT(datiPensione.SiglaCategoria) && ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0017" && !string.IsNullOrEmpty(datiPensione.GetFiltro()) && datiPensione.GetFiltro().ToUpperInvariant().Equals("GSE"))
                || GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicVOAUTVecchiaiaTipoContributivoFiltroGSE))
                return true;

            return false;
        }

        public static bool IsDomandaIndennitaAPESociale(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0057" && datiPensione.Tipo == "0048";
        }

        public static bool IsDomandaAssegnoEsodoArt4Legge922012PerVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0053" && datiPensione.Tipo == "0038";
        }

        public static bool IsDomandaIndennizzoCommerciantiLegge145(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0051" && datiPensione.Tipo == "0175";
        }

        public static bool IsDomandaAssegnoEsodoArt4Legge922012PerAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0053" && datiPensione.Tipo == "0039";
        }

        public static bool IsDomandaAssegnoStraordinarioCreditoLegge2322016PerVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0055" && datiPensione.Tipo == "0038";
        }

        public static bool IsDomandaAssegnoStraordinarioCreditoLegge2322016PerAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0055" && datiPensione.Tipo == "0039";
        }

        public static bool IsDomandaAssegnoStraordinarioCreditoCoopLegge2322016PerAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0056" && datiPensione.Tipo == "0039";
        }

        public static bool IsDomandaAssegnoStraordinarioCreditoCoopLegge2322016PerVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0056" && datiPensione.Tipo == "0038";
        }

        public static bool IsDomandaAssegnoStraordinarioFerrovieLegge2322016PerVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0058" && datiPensione.Tipo == "0038";
        }

        public static bool IsDomandaAssegnoStraordinarioFerrovieLegge2322016PerAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0058" && datiPensione.Tipo == "0039";
        }

        public static bool IsDomandaAssegnoStraordinarioAssicurativiPerAnticipata(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0054" && datiPensione.Tipo == "0039";
        }


        public static bool IsDomandaAssegnoStraordinarioAssicurativiPerVecchiaia(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;
            else
                return datiPensione.Gruppo == "0006" && datiPensione.Prodotto == "0054" && datiPensione.Tipo == "0038";
        }


        public static bool IsDomandaBloccoPrestazioniPL(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null || datiPensione.Gruppo == null || datiPensione.Prodotto == null || datiPensione.Tipo == null)
                return false;
            if (IsDomandaAssegnoStraordinarioAssicurativiPerVecchiaia(datiPensione) || IsDomandaAssegnoStraordinarioAssicurativiPerAnticipata(datiPensione) ||
                IsDomandaAssegnoStraordinarioFerrovieLegge2322016PerAnticipata(datiPensione) || IsDomandaAssegnoStraordinarioFerrovieLegge2322016PerVecchiaia(datiPensione) ||
                IsDomandaAssegnoStraordinarioCreditoCoopLegge2322016PerVecchiaia(datiPensione) || IsDomandaAssegnoStraordinarioCreditoCoopLegge2322016PerAnticipata(datiPensione) ||
                IsDomandaAssegnoStraordinarioCreditoLegge2322016PerAnticipata(datiPensione) || IsDomandaAssegnoStraordinarioCreditoLegge2322016PerVecchiaia(datiPensione) ||
                IsDomandaAssegnoEsodoArt4Legge922012PerAnticipata(datiPensione) || IsDomandaIndennizzoCommerciantiLegge145(datiPensione) ||
                IsDomandaAssegnoEsodoArt4Legge922012PerVecchiaia(datiPensione) || IsDomandaIndennitaAPESociale(datiPensione))
                return true;

            return false;
        }
        //

        public enum TipoUnicarpe
        {
            Not,
            Yes,
            Automatica,
            Manuale
        }

        public enum TipoFondo
        {
            FS,
            PM,
            PMS,
            VL,
            ES,
            ET,
            TT,
            DZ,
            GAS,
            EL,
            CL,
            PMO,
            PI,
            PL,
            PT
        };

        public enum TipoAppartenenza
        {
            FS,
            AGO,
            CI
        };

        public enum TipoDomanda
        {
            Normale,
            Ricostituzione,
            Superstiti,
            Ripristino,
            RipristinoSuperstiti,
            Riliquidazione,
            RiliquidazioneSuperstiti
        };

        public enum FormatoData
        {
            GGmmAAAA,
            AAAAmmGG,
            AAAAmm
        };

        public enum StatoCivile
        {
            NonDichiarato = 0,
            Celibe = 1,
            Coniugato = 2,
            Vedovo = 3,
            Separato = 4,
            Divorziato = 5,
            NonDisponibile = 9
        };

        public enum SiglaFamiliare
        {
            [StringValue("A")]
            Ascendente,
            [StringValue("C")]
            Coniuge,
            [StringValue("F")]
            FratelloInabile,
            [StringValue("G")]
            FratelloMinore,
            [StringValue("I")]
            Inabile,
            [StringValue("J")]
            NipotiMinoriInabili,
            [StringValue("K")]
            NipotiInabili,
            [StringValue("L")]
            Apprendista,
            [StringValue("M")]
            Minore,
            [StringValue("N")]
            Nubile,
            [StringValue("P")]
            NipotiMinoriCollateraliDiretti,
            [StringValue("S")]
            Studente,
            [StringValue("U")]
            Universitario,
            [StringValue("W")]
            NipotiUniversitari,
            [StringValue("Y")]
            NipotiMinoriCollateraliDirettiInabili,
            [StringValue("Z")]
            NipotiStudenti,
        };

        public class StringValue : System.Attribute
        {
            private string _value;
            public StringValue(string value)
            { _value = value; }
            public string Value
            { get { return _value; } }
        }

        public class DifferenzaDateTime
        {
            public DifferenzaDateTime()
            {
                Year = 0;
                Month = 0;
                Day = 0;
            }

            public DifferenzaDateTime(int year, int month, int day)
            {
                Year = year;
                Month = month;
                Day = day;
            }

            public DifferenzaDateTime(int year, int month, bool sottrai)
            {
                if (sottrai && month < 0)
                {
                    year -= 1;
                    month += 12;
                }
                Year = year;
                Month = month;
            }

            public DifferenzaDateTime(DateTime data)
            {
                Year = data.Year;
                Month = data.Month;
                Day = data.Day;
            }

            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }

            public static bool operator <(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                if (diff1.Year < diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month < diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day < diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator >(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                if (diff1.Year > diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month > diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day > diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator <=(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                if (diff1.Year < diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month < diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day <= diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator >=(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                if (diff1.Year > diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month > diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day >= diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator ==(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                // object.ReferenceEquals: true se objA è la stessa istanza di objB oppure se entrambe sono Null; in caso contrario, false.
                // Sono entrambi NULL, quindi uguali
                if (object.ReferenceEquals(diff1, null) && object.ReferenceEquals(diff2, null))
                    return true;
                // Sono entrambi diversi da NULL, quindi verifico direttamente sui dati
                else if (!object.ReferenceEquals(diff1, null) && !object.ReferenceEquals(diff2, null))
                {
                    if (diff1.Year == diff2.Year && diff1.Month == diff2.Month && diff1.Day == diff2.Day)
                        return true;
                    else
                        return false;
                }
                // Uno dei due è NULL, quindi sono diversi
                else
                    return false;
            }

            public static bool operator !=(DifferenzaDateTime diff1, DifferenzaDateTime diff2)
            {
                // object.ReferenceEquals: true se objA è la stessa istanza di objB oppure se entrambe sono Null; in caso contrario, false.
                // Sono entrambi NULL, quindi uguali
                if (object.ReferenceEquals(diff1, null) && object.ReferenceEquals(diff2, null))
                    return false;
                // Sono entrambi diversi da NULL, quindi verifico direttamente sui dati
                else if (!object.ReferenceEquals(diff1, null) && !object.ReferenceEquals(diff2, null))
                {
                    if (diff1.Year != diff2.Year || diff1.Month != diff2.Month || diff1.Day != diff2.Day)
                        return true;
                    else
                        return false;
                }
                // Uno dei due è NULL, quindi sono diversi
                else
                    return true;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (Year != 0)
                    sb.AppendFormat("{0} anni ", Year);
                if (Month != 0)
                    sb.AppendFormat("{0} mesi ", Month);
                if (Day != 0)
                    sb.AppendFormat("{0} giorni ", Day);
                return sb.ToString();
            }
        }

        public static string GetXmlFromObject(object obj)
        {
            if (obj == null)
                return null;

            PulisciOggetto(obj);

            string xmlOutput;

            DataContractSerializer xs = new DataContractSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb))
            {
                xs.WriteObject(writer, obj);
                writer.Flush();
                xmlOutput = sb.ToString();
            }

            return xmlOutput;
        }

        private static void PulisciOggetto(object source)
        {
            if (source == null)
                return;

            Type sourceType = source.GetType();

            if (sourceType.IsClass && sourceType.FullName.StartsWith("System.Collections"))
            {
                ICollection collSource = (ICollection)source;

                if (collSource != null && collSource.Count > 0)
                {
                    foreach (var item in collSource)
                        PulisciOggetto(item);
                }

                return;
            }

            PropertyInfo[] sourceProperties = sourceType.GetProperties();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                string ClassName = sourceProperty.PropertyType.FullName;

                if (sourceProperty.PropertyType.IsClass && !ClassName.StartsWith("System"))
                {
                    ParameterInfo[] parameters = sourceProperty.GetIndexParameters();

                    if (parameters.Length > 0)
                    {
                        foreach (ParameterInfo pInfo in parameters)
                        {
                            PulisciOggetto(pInfo);
                        }
                    }

                    else
                    {
                        object input = sourceProperty.GetValue(source, null);

                        if (input != null)
                            PulisciOggetto(sourceProperty.GetValue(source, null));
                    }
                }

                else if (sourceProperty.PropertyType.IsClass && ClassName.StartsWith("System.Collections"))
                {
                    object input = sourceProperty.GetValue(source, null);

                    if (input != null)
                    {
                        ICollection coll = (ICollection)input;
                        if (coll != null && coll.Count > 0)
                        {
                            foreach (var item in coll)
                            {
                                if (item == null)
                                    continue;
                                PulisciOggetto(item);
                            }
                        }
                    }
                }

                else if (sourceProperty.PropertyType == typeof(string))
                {
                    if (sourceProperty.CanWrite)
                    {
                        if (sourceType.Name == "ArrayOfString")
                        {
                            List<string> coll = (List<string>)source;
                            if (coll != null && coll.Count > 0)
                            {
                                for (int i = 0; i < coll.Count; i++)
                                    if (coll[i] != null)
                                        coll[i] = RemoveInvalidXmlChars(coll[i]);
                            }
                        }
                        else
                        {
                            object propertyValue = sourceProperty.GetValue(source, null);

                            if (propertyValue != null)
                            {
                                string stringPulito = RemoveInvalidXmlChars(propertyValue.ToString());
                                sourceProperty.SetValue(source, stringPulito, null);
                            }
                        }
                    }
                }
            }
        }

        private static string RemoveInvalidXmlChars(string input)
        {
            return new string(input.Where(value =>
                (value >= 0x0020 && value <= 0xD7FF) ||
                (value >= 0xE000 && value <= 0xFFFD) ||
                value == 0x0009 ||
                value == 0x000A ||
                value == 0x000D).ToArray());
        }

        public static T DeserializeXml<T>(string xml)
        {
            T obj;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
                obj = (T)serializer.Deserialize(reader);
            return obj;
        }

        public static string SerializeObject<T>(T obj)
        {
            string xml = string.Empty;
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (var sww = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xsSubmit.Serialize(writer, obj, ns);
                    xml = sww.ToString();
                }
            }
            return xml;
        }

        public static bool AreEqualLists<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            if (list1 == null && list2 == null)
                return true;
            if (list1 != null && list2 != null)
            {
                if (list1.Count() != list2.Count())
                    return false;
                if (list1.Any(x => !list2.Any(y => y.Equals(x))) || list2.Any(y => !list1.Any(x => x.Equals(y))))
                    return false;
                var list2App = list2.ToList();
                foreach (T el1 in list1)
                {
                    if (!list2App.Remove(el1))
                        return false;
                }
                return true;
            }
            return false;
        }

        public static bool isDomandaGiornalistiDipendentiConSistemaPrivato(GestionePensione.DatiPensione datiPensione)
        {
            if ((datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021" && datiPensione.Tipo == "0001" && (datiPensione.GP1AV91B == "0")) || (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.GP1AV91B == "3"))
                return true;
            return false;
        }

        public static bool isDomandaGiornalistiDipendentiConSistemaPrivato(GestionePensione.DatiPensione datiPensione, string GP1AV91B)
        {
            if ((datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021" && datiPensione.Tipo == "0001" && (GP1AV91B == "0")) || (Utility.IsRicostituzione(datiPensione.Gruppo) && GP1AV91B == "3"))
                return true;
            return false;
        }

        public static bool isDomandaReverisibilitaMemo187(GestionePensione.DatiPensione datiPensione, string GP1AV91B, string codiceSpecifico)
        {
            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021" && datiPensione.Tipo == "0001" && GP1AV91B == "0" && codiceSpecifico != "X")
                return true;
            return false;
        }

        public static bool isGestioneAutonoma(string codGestione)
        {
            if (codGestione == "2" || codGestione == "3" || codGestione == "4" || codGestione == "I" || codGestione == "M" || codGestione == "N")
                return true;

            return false;
        }

        public enum StatoPensione
        {
            [Description("DA ACQUISIRE")]
            DaAcquisire = 0,
            [Description("IN ACQUISIZIONE")]
            InAcquisizione = 1,
            [Description("NON LAVORABILE")]
            NonLavorabile = 2,
            [Description("DA CALCOLARE")]
            DaCalcolare = 3,
            [Description("CALCOLATA")]
            Calcolata = 4,
            [Description("SCARTO DA CALCOLO")]
            ScartoDaCalcolo = 5,
            [Description("CALCOLO VERIFY")]
            CalcoloVerify = 6,
            [Description("SCARTO VERIFY")]
            ScartoVerify = 7,
            [Description("CALCOLO NO WEBDOM")]
            CalcolataNoWebDom = 8,
            [Description("CALCOLO NO FELPE")]
            CalcolataNoFelpe = 9,
            [Description("CALCOLO NO ONERI")]
            CalcolataNoOneri = 10,
            [Description("CALCOLO NO SAI")]
            CalcolataNoSAI = 11,
            [Description("CALCOLO NO STAZ. LAVORO")]
            CalcolataNoStazLavoro = 12,
            [Description("CALCOLO NO TOTAL")]
            CalcolataNoTotal = 13,
            [Description("CALCOLO NO SIN")]
            CalcolataNoSIN = 14,
            [Description("CALCOLATA NO BOOKING")]
            CalcolataNoBooking = 15,
            [Description("CALCOLO NO TOT")]
            CalcolataNoTot = 16,
            [Description("CALCOLO NO NOTE DEBITO")]
            CalcolataNoNoteDebito = 17,
            [Description("CALCOLO NO SEI SCATTI")]
            CalcolataNo6Scatti = 18,
            [Description("CALCOLO NO EQUOIND")]
            CalcolataNoEquoInd = 19,
            [Description("CALCOLO NO INDEB")]
            CalcoloNoIndeb = 20,
            [Description("CALCOLO NO INDEB WAIT")]
            CalcoloNoIndebWait = 21,
            [Description("CALCOLO NO INDENN SPEC")]
            CalcolataNoIndennSpec = 22
        };

        public enum TipoCalcolo
        {
            NonValido,
            Contributivo,
            Retributivo,
            Misto,
            RetributivoMonti,
            MistoL214,
            RetributivoComma707,
        };

        public enum CategoriaFondoPI
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            Uno,
            V,
            W,
            X,
            Y,
            Z
        }

        public enum StatoSindacato
        {
            Iniziato,
            Attivo,
            Cessato
        };

        public enum TipoOperazione
        {
            RICERCA,
            UPDATE,
            INSERIMENTO,
            CANCELLAZIONE
        };

        public enum Ruolo
        {
            AMMINISTRATORE,
            DIRETTORE_RDP,
            UTENTE
        };

        public enum ChiaviVersioni
        {
            WA,
            WCF,
            WCFFS,
            WCFAGO,
            WCFCI
        }

        public enum TabAggArca
        {
            Anagrafica,
            ResidenzaEstero,
            Redditi,
            DatiGenerici,
            Detrazioni,
            EsenzioneFiscale,
            Eliminazione
        };

        public enum TipoSalvaguardia
        {
            Nessuna,
            [Description("Usuranti")]
            Usuranti,
            [Description("Salvaguardia L.214")]
            L214,
            [Description("Salvaguardia L.122")]
            L122,
            [Description("Salvaguardia L.135")]
            L135,
            [Description("Salvaguardia L.228")]
            L228,
            [Description("Salvaguardia L.124")]
            L124,
            [Description("Salvaguardia L.124 art.11 bis")]
            L124Art11Bis,
            [Description("Salvaguardia L.147")]
            L147,
            [Description("Esuberi PA")]
            EsuberiPA,
            [Description("Salvaguardia L.147/2014")]
            L147_2014,
            [Description("Salvaguardia L.208/2015")]
            L208_2015,
            [Description("Salvaguardia L.232/2016")]
            L232_2016,
            [Description("APE Precoci")]
            APE_Precoci,
            [Description("Opzione al contributivo")]
            Contributivo_Optante,
            [Description("Salvaguardia L.178/2020")]
            L178_2020,
            [Description("Bancari con bonus")]
            Bancari_Bonus
        }

        public enum TipoQuadro
        {
            NonVisibile = 0,
            Facoltativo = 1,
            Obbligatorio = 2
        }

        public enum SOAPLogDirection
        {
            [Description("I")]
            IN,

            [Description("O")]
            OUT
        }

        public enum Servizio
        {
            SrvWebDom,
            SrvLiquidazioneFs,
            SrvLiquidazioneAgo,
            SrvLiquidazioneCi,
            SrvSAI,
            SrvAggPec,
            SrvGeneraCertificati,
            SrvRedditi,
            SrvArcaMan,
            SrvAllegatiConvenzioni,
            SrvDatiPensioni,
            SrvSIN,
            SrvAssegnazioneCertificato,
            SrvDelegheSindacali,
            SrvUniDetra,
            SrvDetrazioni,
            SrvWSTotalIvs,
            SrvARCA,
            SrvANF,
            SrvVerTitolIBAN,
            SrvBooking,
            SrvNaci,
            SrvOrchServPens,
            SrvCalcoloQuote,
            SrvSistemaPensioni,
            SrvPianiDiPagamento,
            SrvNuovoCalcolo,
            SrvStampeWeb,
            SrvParallelRun
        }

        public enum MetodoServizio
        {
            GetDomanda,
            PrelevaDomanda,
            Aggiornamento_PECO_Fondi_Speciali,
            Aggiornamento_PECO_AGO,
            Aggiornamento_PECO_Convenzioni_Internazionali,
            GeneraFascicolo,
            GAPL,
            GARC,
            GAIN,
            insertDatiSQL,
            FSPL,
            FSRC,
            FSPR,
            RichiestaInserimentoGestione,
            CI05,
            CI02,
            CI01,
            GACI,
            letturaCI05,
            AggiornaFaseAttivita,
            GetDatiTGP1,
            GetDatiTGP2ByCodiceFascicolo,
            GetDatiTGP2ByChiavePensione,
            GetDatiTGP4ByCodiceFascicolo,
            GetDatiTGP4ByChiavePensione,
            GetDatiTGP5,
            GetDatiTGP6,
            GetDatiTGP7,
            GetDatiTGP8,
            AggiornaCI05,
            AssegnazioneCertificato,
            ElencoSindacatiPerCategoria,
            Ricerca,
            RicercaPerDatiPensionato_2,
            PAGSAI,
            PAGSAY,
            PAGSAR,
            PAGSAS,
            GETSAI,
            GETSAY,
            GETSAR,
            GETSAS,
            SBLSAI,
            SBLSAY,
            SBLSAR,
            SBLSAS,
            EstrazioneDatiCumulIVS,
            AggiornaKeyPensioneCUMUL,
            Aggiornamento_PECO_Fondi_AMG,
            Aggiornamento_PECO_Fondi_AMG_INPDAP,
            GeneraCertificato,
            CARC,
            GetSedeDestinazioneByResidenza,
            RicercaPerCodiceFiscale,
            RicercaPerCodiceIndividuale,
            RicercaPerDatiPersonaliParziali,
            getRedditiTemp,
            EstrazioneDatiCumulRicostituzioneIVS,
            AggiornaKeyRicostituzioneCUMUL,
            Approvazione_INPDAP,
            RicercaDomandeANF_Beneficiario_Asincrona,
            RichiediRispostaRicercaAsincrona,
            VerificaTitolarita,
            EseguiSprenotazione,
            VerificaAnniDirittoAlBonus,
            PrenotazioneElaborazioni,
            VerificaProceduraEESSI,
            GetDatiNaci,
            EstrazioneDatiTotalIVS,
            AggiornaKeyPensioneTOTAL,
            AcquisizioneEvento,
            CalcolaQuote,
            GetDatiECodiciVari,
            IvsInvocation,
            InsertOrUpdateDashboard,
            InsertCONG003,
            InsertOrUpdateNuovoCalcolo,
            GestioneCong_6scatti,
            GetAllFlowConf,
            Mainframe,
            Abaco,
            InsertCong005,
            RichiestaTE08DB2NewIVS,
            GetAnteprimaDebito,
            AggiornaCasuali,
            NotificaTE08,
            GestioneCong_Indennizzo,
            LeggiEsitiSanitario,
            IsNuovoCalcolo,
            TrasferimentoPensione
        }

        public enum TipoLogGenerico
        {
            Informativo,
            ErroreApplicativo,
        }

        public enum TipoLogDebug
        {
            Informativo,
            ErroreApplicativo,
        }

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
            [Description("Ricostituzione COOP28 con Filtro DAP")]
            RicCOOP28FiltroDAP,
            [Description("Ricostituzioni VOPGI con filtro L80")]
            RicVOPGIFiltroL80
        }

        public enum TipoFelpe
        {
            AMG = 1,
            SIN = 2,
            SPI = 3
        }

        public enum ErroreGeneraCertificato
        {
            NessunErrore,
            NumeroFascicoloNonGenerato,
            CodiceCategoriaNonValido,
            CodiceSedeNonValido,
            ApplicazioneNonAbilitata,
            RichiestaFascicoloVuota,
            AreaControlloVuota,
            FascicoloInputVuoto,
            CodiceFiscaleObbligatorio,
            CodiceFiscaleFormatoNonValido,
            CodiceFiscaleNonValido,
            NumFascicoloMaxRaggiunto,
            CodiceCategoriaNonValorizzata,
            SedeNonValorizzata,
            RichiestaCertificatoVuota,
            CertificatoInputVuoto,
            NumCertificatoMaxRaggiunto,
            NumeroCertificatoNonGenerato,
            CodiciSedeCategoriaNonValidi,
            IntervalloDateNonValido,
            ErroreGenerico = 999
        }

        public enum TipoAnte96
        {
            Ante96Contributive = 1,
            Ante96Miste = 2,
            Ante96Retributive = 3
        }

        public enum TipoAutomazione
        {
            Supplementi = 1,
            Vecchiaia = 2,
            Trasformazioni = 3
        }

        public static bool IsDomandaBancRicAnte1991(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (IsRicostituzione(datiPensione.Gruppo))
            {
                DateTime date = new DateTime(1990, 12, 31);
                switch (datiPensione.SiglaCategoria.Trim())
                {
                    case "SOBANC":
                        if (datiPensione.DecorrenzaOriginaria == null)
                            return false;

                        if (IsDomandaPensioneReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
                        {
                            if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione != null)
                                return !(DataSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, date));
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

        public static int GetEta(DateTime dataNascita, DateTime dataFineCalcolo)
        {
            int annoNascita = dataNascita.Year;
            int annoFineCalcolo = dataFineCalcolo.Year;

            return annoFineCalcolo - annoNascita;
        }

        public static bool IsPensioneInabilitaProficuoLavoroCumulo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.SiglaCategoria.Trim() == "IOCUM" && (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(0, 1) == "3" || datiPensione.NaturaPensione.Substring(0, 1) == "4")))
            {
                if ((datiPensione.IdTipoPLPerRIC == Utility.TipoPLPerRIC.RicInabilitaAProficuoLavoroMensioniInCumulo.GetHashCode()) || (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0012" && datiPensione.Tipo == "0047"))
                    return true;
            }

            return false;
        }

        public static bool ValorizzaTipologiaCumuloPerInabilita()
        {
            return (ConfigurationManager.AppSettings["TipologiaCumuloPerInabilita"] != null &&
                ConfigurationManager.AppSettings["TipologiaCumuloPerInabilita"] == "SI");
        }

        public static bool isPrepensionamentoEditoria(GestionePensione.DatiPensione datiPensione)
        {
            return (IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) ||
                IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || IsPrepensionamentoEditoriaFiltroEBA(datiPensione) || IsPrepensionamentoEditoriaTipo0162(datiPensione));
        }

        //ENG - Aggiornamento Memo86
        public static bool IsDomandaEccezioneMemo86(GestionePensione.DatiPensione datiPensione)
        {
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.SiglaCategoria.Trim()))
            {
                switch (datiPensione.SiglaCategoria.Trim().ToUpperInvariant())
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

                if (datiPensione.SiglaCategoria.ToUpperInvariant().StartsWith("S") ||
                    (datiPensione.SiglaCategoria.ToUpperInvariant().StartsWith("I") && !(datiPensione.Gestione == "019" || (datiPensione.Gestione == "007" && datiPensione.Fondo == "006") || (datiPensione.Gestione == "007" && datiPensione.Fondo == "014")) &&
                    !string.IsNullOrEmpty(datiPensione.NaturaPensione) && !(datiPensione.NaturaPensione.StartsWith("3") || datiPensione.NaturaPensione.StartsWith("4") || datiPensione.NaturaPensione.StartsWith(" "))))
                {
                    return true;
                }
            }

            return false;
        }

        //ENG - Memo 121_2023
        public static DateTime? CalcolaCessazioneIncumulabilita(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime? dataPerfezionamentoRequisiti)
        {
            DateTime? limiteEtaCessazioneIncumulabilita = new DateTime?();
            if (datiAnagraficiTitolare != null && datiAnagraficiTitolare.DataNascita.HasValue && dataPerfezionamentoRequisiti.HasValue)
            {
                int reqAA = 0;
                int reqMM = 0;
                string codCategoria = datiPensione.GetCodCategoria();
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

                if (tipoAppartenenza.HasValue)
                {
                    if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                    {
                        //primo accesso con DPR
                        GestioneDecodifica.GetCtrlRequisitoEta_Base(dataPerfezionamentoRequisiti.Value, codCategoria, datiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                        DateTime dataPrimoRequisitoAnagrafico = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);

                        reqAA = 0;
                        reqMM = 0;
                        //secondo accesso in tabella con la nuova data calcolata al punto precedente
                        GestioneDecodifica.GetCtrlRequisitoEta_Base(dataPrimoRequisitoAnagrafico, codCategoria, datiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                        limiteEtaCessazioneIncumulabilita = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);
                    }
                    else if (Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione))
                    {
                        List<GestioneDecodifica.CtrlRequisitoEta> elencoCtrlRequisitoEta = null;
                        GestioneDecodifica.GetCtrlRequisitoEta(out elencoCtrlRequisitoEta);
                        List<GestioneDecodifica.CtrlRequisitoEta> lelencoCtrlRequisitoEta = null;
                        lelencoCtrlRequisitoEta = elencoCtrlRequisitoEta.FindAll(x => x.Categoria == codCategoria.PadLeft(4, '0') && x.Sesso == datiAnagraficiTitolare.Sesso.GetValueOrDefault() && x.TipoAppartenenza == tipoAppartenenza.ToString());
                        DateTime? dataRiferimento = dataPerfezionamentoRequisiti.Value;
                        DateTime reqPrecedente = new DateTime();
                        if (lelencoCtrlRequisitoEta != null && lelencoCtrlRequisitoEta.Count() > 0)
                        {
                            foreach (GestioneDecodifica.CtrlRequisitoEta req in lelencoCtrlRequisitoEta)
                            {
                                if (req.InizioPeriodoPerfRequisiti <= dataRiferimento.Value && req.FinePeriodoPerfRequisiti >= dataRiferimento.Value)
                                {
                                    reqAA = req.RequisitoAA.GetValueOrDefault();
                                    reqMM = req.RequisitoMM.GetValueOrDefault();
                                    reqPrecedente = dataRiferimento.GetValueOrDefault();
                                    if (reqPrecedente != dataPerfezionamentoRequisiti)
                                        limiteEtaCessazioneIncumulabilita = reqPrecedente;
                                    dataRiferimento = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);
                                    limiteEtaCessazioneIncumulabilita = dataRiferimento;
                                    ////se viene fatto un solo accesso bisogna prendere la dataRiferimento ricavata la prima volta
                                    //if (reqPrecedente == dataPerfezionamentoRequisiti)
                                    //    limiteEtaCessazioneIncumulabilita = dataRiferimento;

                                    //if (reqPrecedente.Year == dataRiferimento.GetValueOrDefault().Year)
                                    //    limiteEtaCessazioneIncumulabilita = dataRiferimento;
                                }
                            }
                        }
                    }
                }
            }

            return limiteEtaCessazioneIncumulabilita;
        }

        //ENG - Memo 28_2024
        public static DateTime? CalcolaCessazioneIncumulabilita_memo_28(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime? dataPerfezionamentoRequisiti)
        {
            DateTime? limiteEtaCessazioneIncumulabilita = new DateTime?();

            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0017")
            {
                if (datiAnagraficiTitolare != null && datiAnagraficiTitolare.DataNascita.HasValue && datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    int reqAA = 0;
                    int reqMM = 0;
                    string codCategoria = datiPensione.GetCodCategoria();
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                    if (tipoAppartenenza.HasValue)
                    {
                        GestioneDecodifica.GetCtrlRequisitoEta_Base(datiPensione.DecorrenzaOriginaria.Value, codCategoria, datiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                        limiteEtaCessazioneIncumulabilita = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);
                    }
                }
            }
            else
            {
                if (datiAnagraficiTitolare != null && datiAnagraficiTitolare.DataNascita.HasValue && dataPerfezionamentoRequisiti.HasValue)
                {
                    int reqAA = 0;
                    int reqMM = 0;
                    string codCategoria = datiPensione.GetCodCategoria();
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

                    if (tipoAppartenenza.HasValue)
                    {
                        //primo accesso con DPR
                        GestioneDecodifica.GetCtrlRequisitoEta_Base(dataPerfezionamentoRequisiti.Value, codCategoria, datiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                        DateTime dataPrimoRequisitoAnagrafico = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);

                        reqAA = 0;
                        reqMM = 0;
                        //secondo accesso in tabella con la nuova data calcolata al punto precedente
                        GestioneDecodifica.GetCtrlRequisitoEta_Base(dataPrimoRequisitoAnagrafico, codCategoria, datiAnagraficiTitolare.Sesso.GetValueOrDefault(), tipoAppartenenza.ToString(), out reqAA, out reqMM);
                        limiteEtaCessazioneIncumulabilita = Utility.FirstDayOfMonth(datiAnagraficiTitolare.DataNascita.Value).AddYears(reqAA).AddMonths(reqMM);
                    }
                }
            }
            return limiteEtaCessazioneIncumulabilita;
        }


        //ENG - Memo 166/2023
        public static bool isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaVecchiaiaTrasformazioneAOI(datiPensione).GetValueOrDefault()
                && !String.IsNullOrEmpty(datiPensione.GetFiltro())
                && datiPensione.GetFiltro().ToUpperInvariant() == "ODM")
                return true;

            return false;
        }

        /// <summary>
        /// Il metodo riconosce gli assegni ordinari di invalidità
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <returns></returns>
        public static bool IsAssegnoInvalidita(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011")
                return true;

            return false;
        }

        public static bool IsNuovoCalcolo_old(GestionePensione.DatiPensione datiPensione, bool isVerify)
        {
            bool isNuovoCalcolo = false;
            List<string> listaCategorie = new List<string> { "VO", "VOP", "VR", "VOART", "VOCOM" };
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001"
                && listaCategorie.Contains(datiPensione.SiglaCategoria.Trim()) && !IsRiaperturaDomanda(datiPensione.Id))
            {
                if (isVerify)
                {
                    isNuovoCalcolo = true;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaNuovoCalcolo", out controlloDinamico);
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "NO")
                    {
                        NuovoCalcolo nuovoCalcolo;
                        GestioneCtrlNuovoCalcolo.GetCtrlNuovoCalcolo(datiPensione.NDomus, out nuovoCalcolo);
                        //se la domus non è presente in tabella o il valore di FlagVerifyDef non è V o B
                        if (nuovoCalcolo == null || string.IsNullOrEmpty(nuovoCalcolo.FlagVerifyDef) || nuovoCalcolo.FlagVerifyDef == "D")
                            isNuovoCalcolo = false;
                    }
                    else if (controlloDinamico == null)
                    {
                        isNuovoCalcolo = false;
                    }
                }
                else
                {
                    isNuovoCalcolo = true;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaNuovoCalcoloDefinitivo", out controlloDinamico);
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "NO")
                    {
                        NuovoCalcolo nuovoCalcolo;
                        GestioneCtrlNuovoCalcolo.GetCtrlNuovoCalcolo(datiPensione.NDomus, out nuovoCalcolo);
                        //se la domus non è presente in tabella o il valore di FlagVerifyDef non è D o B
                        if (nuovoCalcolo == null || string.IsNullOrEmpty(nuovoCalcolo.FlagVerifyDef) || nuovoCalcolo.FlagVerifyDef == "V")
                            isNuovoCalcolo = false;
                    }
                    else if (controlloDinamico == null)
                    {
                        isNuovoCalcolo = false;
                    }
                }
            }

            return isNuovoCalcolo;
        }

        public static bool IsPerimetroNuovoCalcolo(GestionePensione.DatiPensione datiPensione)
        {
            List<string> listaCategorie = new List<string> { "VO", "VOP", "VR", "VOART", "VOCOM" };
            if (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo == "0001"
                && listaCategorie.Contains(datiPensione.SiglaCategoria.Trim()) && !IsRiaperturaDomanda(datiPensione.Id))
            {
                return true;
            }
            return false;
        }

        public static bool VerificaSkipEccezioniNuovoCalcolo(GestionePensione.DatiPensione datiPensione, string modalitaCalcolo)
        {
            GestionePensione.DatiEliminazione datiEliminazione = null;
            GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
            GestioneControlliDinamici.ControlloDinamico abilitaEliminateNuovoCalcolo;
            string nomeControlloEliminate = "AbilitaEliminateNuovoCalcolo" + modalitaCalcolo;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloEliminate, out abilitaEliminateNuovoCalcolo);
            GestioneControlliDinamici.ControlloDinamico abilitaBititolariNuovoCalcolo;
            string nomeControlloBititolari = "AbilitaBititolariNuovoCalcolo" + modalitaCalcolo;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloBititolari, out abilitaBititolariNuovoCalcolo);
            bool noSkipEccezioni = true;
            if (!(abilitaEliminateNuovoCalcolo != null && abilitaEliminateNuovoCalcolo.ValoreControllo == "SI"))
            {
                if (datiEliminazione != null) noSkipEccezioni = false;
            }
            if (!(abilitaBititolariNuovoCalcolo != null && abilitaBititolariNuovoCalcolo.ValoreControllo == "SI"))
            {
                if (!string.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(0, 1) == "6" || datiPensione.NaturaPensione.Substring(0, 1) == "2" || datiPensione.NaturaPensione.Substring(0, 1) == "3" || datiPensione.NaturaPensione.Substring(0, 1) == "4")) noSkipEccezioni = false;
            }
            return noSkipEccezioni;
        }

        public static bool IsPerimetroNuovoCalcoloConfDinamica(GestionePensione.DatiPensione datiPensione, out List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata, bool? isVerify)
        {
            var lstConfigurazione = GestioneNuovoCalcolo.GetConfigurazioneDinamica(datiPensione.NDomus, datiPensione.MatricolaUtenteAcquisizione);

            lstConfFiltrata = lstConfigurazione.FindAll(x => x.CodGruppo == datiPensione.Gruppo && x.CodProdotto == datiPensione.Prodotto && x.CodTipo == datiPensione.Tipo
            && x.CodGestione == datiPensione.Gestione && x.CodFondo == datiPensione.Fondo && x.IndConvInt == (datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0") &&
            ((IsRiaperturaDomanda(datiPensione.Id) && x.Fase != "NORIAPERTURA") || (!IsRiaperturaDomanda(datiPensione.Id) && x.Fase == "NORIAPERTURA")) &&
            DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, x.DecOrig.Value) && (string.IsNullOrEmpty(datiPensione.CodiceTipoRichiesta) || (x.CodiciTipoRichiesta != null && x.CodiciTipoRichiesta.Contains(datiPensione.CodiceTipoRichiesta)) || x.CodiciTipoRichiesta == null));

            //Se sono una RIC ed è stata trovata una configurazione, devo fare un ulteriore check su una pensione PL collegata
            //con la combinazione proposta ad oggi, basta che sia presente una configurazione qualsiasi per la PL per far valere la configurazione della RIC (vince l'indicazione della RIC)
            //PL = new e ric = new --> ric new
            //PL = both e ric = new --> ric new
            //PL = both e ric = both --> ric both
            //PL = new e ric = both --> ric both
            if (Utility.IsRicostituzione(datiPensione.Gruppo) && lstConfFiltrata != null && lstConfFiltrata.Count > 0)
            {
                GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DoppioCheckRicostituzioniNCC", out controlloDinamico);
                if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                {
                    short sedeChiavePensioneDB = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;
                    var datiPensioneColl = CheckDomandaNonRicCalcolataSuStessaPensione(datiPensione, sedeChiavePensioneDB);
                    if (datiPensioneColl != null)
                    {
                        //è stato necessario assegnare le proprietà perchè per qualche motivo senza farlo andava in errore
                        var gruppo = datiPensioneColl.Gruppo;
                        var prodotto = datiPensioneColl.Prodotto;
                        var tipo = datiPensioneColl.Tipo;
                        var gestione = datiPensioneColl.Gestione;
                        var fondo = datiPensioneColl.Fondo;
                        var indConvInt = datiPensioneColl.IndConvInt.GetValueOrDefault() ? "1" : "0";
                        var id = datiPensioneColl.Id;
                        var decorrenzaOriginaria = datiPensioneColl.DecorrenzaOriginaria.Value;
                        var codiceTipoRichiesta = datiPensioneColl.CodiceTipoRichiesta;

                        //lstConfigurazione PL collegata
                        List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrataPensioneCollegata;
                        lstConfFiltrataPensioneCollegata = lstConfigurazione.FindAll(x =>
                        x.CodGruppo == gruppo &&
                        x.CodProdotto == prodotto &&
                        x.CodTipo == tipo &&
                        x.CodGestione == gestione &&
                        x.CodFondo == fondo &&
                        x.IndConvInt == indConvInt &&
                        ((IsRiaperturaDomanda(datiPensioneColl.Id) && x.Fase != "NORIAPERTURA") || (!IsRiaperturaDomanda(datiPensioneColl.Id) && x.Fase == "NORIAPERTURA")) &&
                        DataSuccessivaA(datiPensioneColl.DecorrenzaOriginaria.Value, x.DecOrig.Value) && (string.IsNullOrEmpty(datiPensioneColl.CodiceTipoRichiesta) || (x.CodiciTipoRichiesta != null && x.CodiciTipoRichiesta.Contains(datiPensioneColl.CodiceTipoRichiesta)) || x.CodiciTipoRichiesta == null)
                         );//&& (!isVerify.HasValue  || (isVerify.HasValue && ((isVerify.Value && x.TipoRichiesta == "1") || (!isVerify.Value && x.TipoRichiesta == "0")))));

                        //se la PL collegata non è in nessun modo in configurazione, la configurazione della RIC non è valida
                        //Atrimenti si può uscire con la configurazione trovata per la RIC
                        if (lstConfFiltrataPensioneCollegata == null || lstConfFiltrataPensioneCollegata.Count == 0)
                            return false;
                    }
                    else //se non c'è una PL collegata la configurazione non è valida
                        return false;
                }
            }

            if (lstConfFiltrata != null && lstConfFiltrata.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsNuovoCalcolo(GestionePensione.DatiPensione datiPensione, bool isVerify, out GestioneNuovoCalcolo.FlowConf confDomanda)
        {
            bool isNuovoCalcolo = false;
            List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
            confDomanda = null;

            //Controllo se c'è una domanda puntuale 
            NuovoCalcolo nuovoCalcolo;
            GestioneCtrlNuovoCalcolo.GetDomandePuntualiCtrlNuovoCalcolo(datiPensione.NDomus, out nuovoCalcolo);
            if (isVerify)
            {
                if (!(nuovoCalcolo == null || string.IsNullOrEmpty(nuovoCalcolo.FlagVerifyDef) || nuovoCalcolo.FlagVerifyDef == "D"))
                    return true;
            }
            else
            {
                if (!(nuovoCalcolo == null || string.IsNullOrEmpty(nuovoCalcolo.FlagVerifyDef) || nuovoCalcolo.FlagVerifyDef == "V"))
                    return true;
            }

            //Valuto la tabella delle sedi
            GestioneNuovoCalcolo.DatiCtrlSedeTransazioneNuovoCalcolo datiCtrlSedeTransazioneNuovoCalcolo;
            GestioneNuovoCalcolo.GetCtrlSedeTransazioneNuovoCalcoloBySede(datiPensione.CodiceSede.ToString().PadLeft(4, '0') + datiPensione.CentroOperativo.Value.ToString().PadLeft(2, '0'), out datiCtrlSedeTransazioneNuovoCalcolo);
            if (datiCtrlSedeTransazioneNuovoCalcolo != null && datiCtrlSedeTransazioneNuovoCalcolo.Attiva.GetValueOrDefault())
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO)
                {
                    if (!datiCtrlSedeTransazioneNuovoCalcolo.GARC.GetValueOrDefault() && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                    else if (!datiCtrlSedeTransazioneNuovoCalcolo.GAPL.GetValueOrDefault() && !(tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                }
                else if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
                {
                    //TODO
                    if (!datiCtrlSedeTransazioneNuovoCalcolo.FSPL.GetValueOrDefault() && !(tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                }
                else if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.CI)
                {
                    //TODO
                }
            }
            else
                return false;

            if (IsPerimetroNuovoCalcoloConfDinamica(datiPensione, out lstConfFiltrata, isVerify))
            {
                if (isVerify)
                {
                    isNuovoCalcolo = false;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaNuovoCalcolo", out controlloDinamico);
                    bool noSkipEccezioni = VerificaSkipEccezioniNuovoCalcolo(datiPensione, "NEW");
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" && noSkipEccezioni)
                    {
                        confDomanda = lstConfFiltrata.Find(x => x.TipoRichiesta == "1" && x.SistemiInvocati == "NEW");
                        if (confDomanda != null)
                            isNuovoCalcolo = true;
                    }
                    else
                    {
                        isNuovoCalcolo = false;
                    }
                }
                else
                {
                    isNuovoCalcolo = false;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaNuovoCalcoloDefinitivo", out controlloDinamico);
                    bool noSkipEccezioni = VerificaSkipEccezioniNuovoCalcolo(datiPensione, "NEW");
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" && noSkipEccezioni)
                    {
                        confDomanda = lstConfFiltrata.Find(x => x.TipoRichiesta == "0" && x.SistemiInvocati == "NEW");
                        if (confDomanda != null)
                            isNuovoCalcolo = true;
                    }
                    else
                    {
                        isNuovoCalcolo = false;
                    }
                }
            }

            return isNuovoCalcolo;
        }

        public static bool IsDoppiaChiamataConfDinamica(GestionePensione.DatiPensione datiPensione, bool isVerify, out GestioneNuovoCalcolo.FlowConf confFiltrata)
        {
            bool isDoppiaChiamata = false;
            List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
            confFiltrata = null;

            //Controllo se c'è una domanda puntuale 
            NuovoCalcolo nuovoCalcolo;
            GestioneCtrlNuovoCalcolo.GetDomandePuntualiCtrlNuovoCalcolo(datiPensione.NDomus, out nuovoCalcolo);
            if (isVerify)
            {
                if (nuovoCalcolo != null && (nuovoCalcolo.FlagDoppiaChiamata == "V" || nuovoCalcolo.FlagDoppiaChiamata == "B"))
                    return true;
            }
            else
            {
                if (nuovoCalcolo != null && (nuovoCalcolo.FlagDoppiaChiamata == "D" || nuovoCalcolo.FlagDoppiaChiamata == "B"))
                    return true;
            }

            //Valuto la tabella delle sedi
            GestioneNuovoCalcolo.DatiCtrlSedeTransazioneNuovoCalcolo datiCtrlSedeTransazioneNuovoCalcolo;
            GestioneNuovoCalcolo.GetCtrlSedeTransazioneNuovoCalcoloBySede(datiPensione.CodiceSede.ToString().PadLeft(4, '0') + datiPensione.CentroOperativo.Value.ToString().PadLeft(2, '0'), out datiCtrlSedeTransazioneNuovoCalcolo);
            if (datiCtrlSedeTransazioneNuovoCalcolo != null && datiCtrlSedeTransazioneNuovoCalcolo.Attiva.GetValueOrDefault())
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO)
                {
                    if (!datiCtrlSedeTransazioneNuovoCalcolo.GARC.GetValueOrDefault() && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                    else if (!datiCtrlSedeTransazioneNuovoCalcolo.GAPL.GetValueOrDefault() && !(tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                }
                else if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
                {
                    //TODO
                    if (!datiCtrlSedeTransazioneNuovoCalcolo.FSPL.GetValueOrDefault() && !(tipoDomanda == Utility.TipoDomanda.Ricostituzione || IsRiaperturaDomanda(datiPensione.Id)))
                        return false;
                }
                else if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.CI)
                {
                    //TODO
                }
            }
            else
                return false;

            if (IsPerimetroNuovoCalcoloConfDinamica(datiPensione, out lstConfFiltrata, isVerify))
            {

                if (isVerify)
                {
                    isDoppiaChiamata = false;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ChiamaIvsInvocation", out controlloDinamico);
                    bool noSkipEccezioni = VerificaSkipEccezioniNuovoCalcolo(datiPensione, "BOTH");
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" && noSkipEccezioni)
                    {
                        confFiltrata = lstConfFiltrata.Find(x => x.TipoRichiesta == "1" && x.SistemiInvocati == "BOTH");
                        if (confFiltrata != null)
                            isDoppiaChiamata = true;
                    }
                    else
                    {
                        isDoppiaChiamata = false;
                    }
                }
                else
                {
                    isDoppiaChiamata = false;
                    GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ChiamaIvsInvocationDefinitivo", out controlloDinamico);
                    bool noSkipEccezioni = VerificaSkipEccezioniNuovoCalcolo(datiPensione, "BOTH");
                    if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" && noSkipEccezioni)
                    {
                        confFiltrata = lstConfFiltrata.Find(x => x.TipoRichiesta == "0" && x.SistemiInvocati == "BOTH");
                        if (confFiltrata != null)
                            isDoppiaChiamata = true;
                    }
                    else
                    {
                        isDoppiaChiamata = false;
                    }
                }
            }

            return isDoppiaChiamata;
        }

        public static GestionePensione.DatiPensione CheckDomandaNonRicCalcolataSuStessaPensione(GestionePensione.DatiPensione datiDomanda, short sedeChiavePensioneDB)
        {
            GestionePensione.DatiPensione datiPensioneCollegata = null;
            try
            {
                if (datiDomanda != null)
                {
                    List<GestionePensione.DatiPensione> elencoDatiPensioni = null;
                    GestionePensione.GetPensioneByChiavePensione(datiDomanda.SiglaCategoria, sedeChiavePensioneDB, datiDomanda.NCertificato.GetValueOrDefault(), null, out elencoDatiPensioni);
                    if (elencoDatiPensioni != null && elencoDatiPensioni.Count > 0)
                    {
                        foreach (GestionePensione.DatiPensione datiPens in elencoDatiPensioni)
                        {
                            if (datiPens.NDomus != datiDomanda.NDomus && !Utility.IsRicostituzione(datiPens.Gruppo))
                            {
                                datiPensioneCollegata = new GestionePensione.DatiPensione();
                                ValorizzaOggettiNew(datiPens, datiPensioneCollegata);
                                return datiPensioneCollegata;
                            }
                        }
                    }
                    else
                        return datiPensioneCollegata;
                }
            }
            catch (Exception ex)
            {
                return datiPensioneCollegata;
            }
            return datiPensioneCollegata;
        }

        public static bool CopiaNuovoCalcoloPerDoppiaChamataRic(GestionePensione.DatiPensione datiPensione)
        {
            bool isDoppiaChiamata = false;
            List<GestioneNuovoCalcolo.FlowConf> lstConfFiltrata;
            GestioneNuovoCalcolo.FlowConf confFiltrata = null;

            if (IsPerimetroNuovoCalcoloConfDinamica(datiPensione, out lstConfFiltrata, null)) 
            {
                isDoppiaChiamata = false;
                GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ChiamaIvsInvocation", out controlloDinamico);
                bool noSkipEccezioni = VerificaSkipEccezioniNuovoCalcolo(datiPensione, "BOTH");
                if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI" && noSkipEccezioni)
                {
                    //deve esserci almeno una configurazione con BOTH
                    confFiltrata = lstConfFiltrata.Find(x => x.SistemiInvocati == "BOTH");
                    if (confFiltrata != null)
                        isDoppiaChiamata = true;
                }
                else
                {
                    isDoppiaChiamata = false;
                }
            }
            return isDoppiaChiamata;
        }

        public static bool IsDomandaINPGI(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.Gestione == "001" && datiPensione.Fondo == "008")
                return true;

            return false;
        }

        public static bool isRicostituzioneOrReversibileINPDAP(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            if (IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione != null)
                    return true;
            }
            return false;
        }

        public static bool IsDomandaMiglioramentiContrattuali(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione.SiglaCategoria.Trim() == "VOCUM" && datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0107" && datiPensione.Tipo == "0198")
                return true;
            return false;
        }

        public static string InserisciValoreCaratterizzazione(string caratterizzazioneDB, char valore, int posizione)
        {
            if (posizione < 1 || posizione > 8)
            {
                /// "La posizione deve essere compresa tra 1 e 8."
            }

            string caratterizzazioneResult = string.Empty;
            if (string.IsNullOrEmpty(caratterizzazioneDB))
            {
                caratterizzazioneResult = new string(' ', 8);
            }
            else
            {
                caratterizzazioneResult = caratterizzazioneDB.PadRight(posizione);
            }
            caratterizzazioneResult = caratterizzazioneResult.Substring(0, posizione - 1) + valore + caratterizzazioneResult.Substring(posizione);

            return caratterizzazioneResult;
        }

        public static string EliminaValoreCaratterizzazione(string caratterizzazioneDB, int posizione)
        {
            if (posizione < 1 || posizione > 8)
            {
                // "La posizione deve essere compresa tra 1 e 8.";
            }

            if (string.IsNullOrEmpty(caratterizzazioneDB) || caratterizzazioneDB.Length < posizione)
            {
                return caratterizzazioneDB;
            }

            string caratterizzazioneResult = caratterizzazioneDB.Substring(0, posizione - 1) + caratterizzazioneDB.Substring(posizione);

            return caratterizzazioneResult;
        }

        public static bool isDomandaRicperRiliquidazioneEtaPensionabile(GestionePensione.DatiPensione datiPensione)
        {
            return datiPensione.Gruppo == "0031" && datiPensione.Prodotto == "0114" && datiPensione.Tipo == "0001";
        }


        //codice specifica 187(per fondo FS) o 142(per fondo PT) = H
        public static bool IsRicPerMotiviContributiviDaIndiretteFsPt(string siglaCategoria, string gruppo, string prodotto, string tipo, byte? codiceSpecifico)
        {
            if ((siglaCategoria.Trim() == "SFS" || siglaCategoria.Trim() == "SPT") &&
                gruppo == "0031" && prodotto == "0407" && tipo == "0001" &&
                    (codiceSpecifico == 187 || codiceSpecifico == 142))
                return true;

            return false;
        }

        public static bool CheckMemo97(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            if (IsRicostituzione(datiPensione.Gruppo) && datiPensione.GP1AV91B == "2" &&
                (IsDomandaIOPGI(datiPensione.SiglaCategoria)
                || IsDomandaSOPGI(datiPensione.SiglaCategoria)
                || IsDomandaVOPGI(datiPensione.SiglaCategoria)))
                retVal = true;

            return retVal;
        }

        //ENG - Spacchettate SO
        public static bool IsDomandaSpacchettamentoSO(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            if (datiPensione != null)
            {
                TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                if (tipoAppartenenza == TipoAppartenenza.AGO)
                {
                    if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SO")
                    {
                        if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                        {
                            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate = null;
                            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSO", out controlloDinamicoSpacchettate);
                            else if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSO", out controlloDinamicoSpacchettate);

                            if (controlloDinamicoSpacchettate != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo.Trim()))
                            {
                                DateTime? controlloDinamicoDataControllo = Utility.DataFromString(controlloDinamicoSpacchettate.ValoreControllo.Trim(), FormatoData.AAAAmmGG);

                                //Una domanda deve rientrare nel flusso delle spacchettate SO se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (controlloDinamicoDataControllo.HasValue && Utility.DataSuccessivaA(datiPensione.DataAcquisizione.GetValueOrDefault(), controlloDinamicoDataControllo.GetValueOrDefault()))
                                    return true;
                            }
                        }
                        else
                        {
                            if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SOCOM
        public static bool IsDomandaSpacchettamentoSOCOM(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            if (datiPensione != null)
            {
                TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                if (tipoAppartenenza == TipoAppartenenza.AGO)
                {
                    if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOCOM")
                    {
                        if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                        {
                            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate = null;
                            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSOCOM", out controlloDinamicoSpacchettate);
                            else if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSOCOM", out controlloDinamicoSpacchettate);

                            if (controlloDinamicoSpacchettate != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo.Trim()))
                            {
                                DateTime? controlloDinamicoDataControllo = Utility.DataFromString(controlloDinamicoSpacchettate.ValoreControllo, FormatoData.AAAAmmGG);

                                //Una domanda deve rientrare nel flusso delle spacchettate SOCOM se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (controlloDinamicoDataControllo.HasValue && Utility.DataSuccessivaA(datiPensione.DataAcquisizione.GetValueOrDefault(), controlloDinamicoDataControllo.GetValueOrDefault()))
                                    return true;
                            }
                        }
                        else
                        {
                            if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SR
        public static bool IsDomandaSpacchettamentoSR(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            if (datiPensione != null)
            {
                TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                if (tipoAppartenenza == TipoAppartenenza.AGO)
                {
                    if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SR")
                    {
                        if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                        {
                            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate = null;
                            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSR", out controlloDinamicoSpacchettate);
                            else if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSR", out controlloDinamicoSpacchettate);

                            if (controlloDinamicoSpacchettate != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo.Trim()))
                            {
                                DateTime? controlloDinamicoDataControllo = Utility.DataFromString(controlloDinamicoSpacchettate.ValoreControllo, FormatoData.AAAAmmGG);

                                //Una domanda deve rientrare nel flusso delle spacchettate SR se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (controlloDinamicoDataControllo.HasValue && Utility.DataSuccessivaA(datiPensione.DataAcquisizione.GetValueOrDefault(), controlloDinamicoDataControllo.GetValueOrDefault()))
                                    return true;
                            }
                        }
                        else
                        {
                            if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        //ENG - Spacchettate SOART
        public static bool IsDomandaSpacchettamentoSOART(GestionePensione.DatiPensione datiPensione, bool isRiapertura)
        {
            if (datiPensione != null)
            {
                TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                if (tipoAppartenenza == TipoAppartenenza.AGO)
                {
                    if (!String.IsNullOrEmpty(datiPensione.SiglaCategoria) && datiPensione.SiglaCategoria.Trim().ToUpperInvariant() == "SOART")
                    {
                        if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
                        {
                            BLCommon.GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate = null;
                            if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0021")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloSpacchettateSOART", out controlloDinamicoSpacchettate);
                            else if (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022")
                                BLCommon.GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataControlloIndiretteSpacchettateSOART", out controlloDinamicoSpacchettate);

                            if (controlloDinamicoSpacchettate != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo) && !String.IsNullOrEmpty(controlloDinamicoSpacchettate.ValoreControllo.Trim()))
                            {
                                DateTime? controlloDinamicoDataControllo = Utility.DataFromString(controlloDinamicoSpacchettate.ValoreControllo, FormatoData.AAAAmmGG);

                                //Una domanda deve rientrare nel flusso delle spacchettate SOART se la Data Acquisizione è maggiore/uguale della data del controllo dinamico, altrimenti deve rientrare nel flusso classico                
                                if (controlloDinamicoDataControllo.HasValue && Utility.DataSuccessivaA(datiPensione.DataAcquisizione.GetValueOrDefault(), controlloDinamicoDataControllo.GetValueOrDefault()))
                                    return true;
                            }
                        }
                        else
                        {
                            if (datiPensione.GP1AJSP.HasValue && datiPensione.GP1AJSP.Value == '1')
                                return true;
                        }
                    }
                }
            }

            return false;
        }


        // Memo 79 2025 Check Memo
        public static bool IsDomandaOrganizzazioniInternazionali(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            if (IsDomandaOrganizzazioniInternazionali_Vecchiaia_Invialidita(datiPensione) ||
                IsDomandaOrganizzazioniInternazionali_Superstiti(datiPensione) ||
                IsDomandaOrganizzazioniInternazionali_Anticipate(datiPensione))
                retVal = true;

            return retVal;
        }

        // Memo 79 2025 Check Domande Organizzazioni Internazionali Vecchiaia ed Invialidità
        public static bool IsDomandaOrganizzazioniInternazionali_Vecchiaia_Invialidita(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO || tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
            {
                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Vecchiaia_Invialidita(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo))
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
        public static bool IsDomandaOrganizzazioniInternazionali_Superstiti(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO || tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS)
            {
                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Superstiti(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo))
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
        public static bool IsDomandaOrganizzazioniInternazionali_Anticipate(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            TipoAppartenenza? tipoAppartenenza = GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if ((tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.AGO || tipoAppartenenza.GetValueOrDefault() == TipoAppartenenza.FS))
            {
                if ((datiPensione.CodiceTipoRichiesta == "C9" && IsDomanda_Anticipate(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo))
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

        // Memo 79 2025 Check Domande di pensione anticipate Senza Contributivo
        public static bool IsDomanda_Anticipate_NoContributivo(GestionePensione.DatiPensione datiPensione)
        {
            bool retVal = false;
            if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0001" && datiPensione.Tipo == "0001")
                && IsDomandaOrganizzazioniInternazionali(datiPensione))
                retVal = true;
            return retVal;
        }


        // Memo 79 
        public static void SommaSettimaneAAMMGG(int AA, int MM, int GG, int AAToAdd, int MMToAdd, int GGToAdd, out short retAnni, out short retMesi, out short retGiorni)
        {
            // Somma valori singoli
            int giorniTotali = GG + GGToAdd;
            int mesiTotali = MM + MMToAdd;
            int anniTotali = AA + AAToAdd;

            // Riporta giorni in mesi (ogni 30 giorni = 1 mese)
            if (giorniTotali >= 30)
            {
                mesiTotali += giorniTotali / 30;
                giorniTotali = giorniTotali % 30;
            }
            // Riporta mesi in anni (ogni 12 mesi = 1 anno)
            if (mesiTotali >= 12)
            {
                anniTotali += mesiTotali / 12;
                mesiTotali = mesiTotali % 12;
            }

            // Output come short
            retAnni = (short)anniTotali;
            retMesi = (short)mesiTotali;
            retGiorni = (short)giorniTotali;
        }

        //ENG - Memo 91/2026 
        public static bool IsDomandaCOOP28_DAP(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                return false;

            if (IsDomandaCOOP28(datiPensione.SiglaCategoria) && (datiPensione.GetFiltro().ToUpperInvariant().Equals("DAP") ||
                GetEnumTipoPLPerRICbyId(datiPensione.IdTipoPLPerRIC) == TipoPLPerRIC.RicCOOP28FiltroDAP))
                return true;

            return false;
        }

        //ENG - RIC/TRF SUPERSTITI INDIRETTE 024 ANTE SETTEMBRE 1995
        public static bool isDomandaRicSuperstitiIndiretta024AnteSettembre1995(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione, bool isRiaperturaDomanda)
        {
            if (datiPensione == null)
                return false;

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) &&
                (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.DecorrenzaOriginaria.HasValue &&
                !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1995, 9, 1)))
            {
                if (string.IsNullOrEmpty(danteCausa.SiglaCategoria) || string.IsNullOrEmpty(danteCausa.Sede) || danteCausa.Sede.PadLeft(4, '0') == "0000"
                      || !danteCausa.Certificato.HasValue || danteCausa.Certificato.Value == 0)
                {
                    if (datiLavorazione != null && datiLavorazione.TipoReversibilita.HasValue && datiLavorazione.TipoReversibilita.Value.ToString().ToUpperInvariant() == "I")
                        return true;
                }
            }

            return false;

        }

        public static bool ControllaBloccoValidazioneCausaliByCensimentoSedi(GestionePensione.DatiPensione datiPensione)
        {
            // Null-check espliciti
            if (datiPensione == null)
                return true;

            short CodiceSede = datiPensione.CodiceSede;

            // Prepara DTO per la chiamata
            GestioneControlliDinamici.ControlloDinamico controllo;

            // WCF client
            try
            {
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ListaSediAbilitantiTe08Ind", out controllo);

                //Se il controllo dinamico non è stato trovato è accaduto qualcosa di anomalo a livello interazione con il db
                //Blocco a prescidnere
                if (controllo == null)
                {
                    return true;
                }

                if (controllo.ValoreControllo == null || controllo.ValoreControllo.Trim().Length == 0)
                    return true;

                if (controllo.ValoreControllo.Equals("Tutte", StringComparison.OrdinalIgnoreCase))
                    return false;

                return !controllo.ValoreControllo.Contains(CodiceSede.ToString());
            }
            catch
            {
                // In caso di errore remoto, fallback conservativo
                return false;
            }
        }
    }
}
