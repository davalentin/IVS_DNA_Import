using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.Pensioni.LiquidazioneCi.Entity;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneControlli
    {
        #region Convenzioni Internazionali

        #region LiquidazionePensione

        /// <summary>
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz96CategDecorrenza(bool? bReqAnz96, string CodeNatura, string Gruppo, string prodotto, DateTime? Decorrenza, bool isRiaperturaDomanda)
        {
            if (String.IsNullOrEmpty(Gruppo) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
            List<string> lCodNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "U", "L", "Z", "O" };

            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda) //prime liquidate
            {
                if (bReqAnz96.HasValue)  // ReqAnz94 valorizzato
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") &&
                         lCodNatura.Contains(CodeNatura.Substring(2, 1).ToUpperInvariant()))
                    {
                        DateTime dtMin = new DateTime(1994, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 09/96 incompatibile con categoria o decorrenza
        /// </summary>
        /// <param name="bReqAnzVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz96_CatVOS_9596(bool? bReqAnz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            if (bReqAnz96.HasValue)  // ReqAnz94 valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2")) // codeNatura
                    {
                        DateTime dtMin = new DateTime(1995, 01, 01);
                        DateTime dtMax = new DateTime(1996, 10, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 09/96 incompatibile con categoria o decorrenza
        /// </summary>
        /// <param name="bReqAnz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz96_CatMix_9597(bool? bReqAnz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };

            if (bReqAnz96.HasValue)  // ReqAnz94 valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2"))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1995, 01, 01);
                        DateTime dtMax = new DateTime(1997, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94CategDecorrenza(bool? bReqVec94, string CodeNatura, string Gruppo, string prodotto, DateTime? Decorrenza, bool isRiaperturaDomanda)
        {
            if (String.IsNullOrEmpty(Gruppo) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
            List<string> lCodNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "U", "L", "Z", "O" };

            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda) //prime liquidate
            {
                if (!bReqVec94.HasValue)  // ReqAnz94 non valorizzato
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") &&
                         lCodNatura.Contains(CodeNatura.Substring(2, 1).ToUpperInvariant()))
                    {
                        DateTime dtMin = new DateTime(1994, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94_CatVOS_9709(bool? bReqVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            if (!bReqVec94.HasValue)  // ReqAnz94 non valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2")) // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94_CatVOS_97(bool? bReqVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCodNatura = new List<string> { "I", "U", "L" };

            if (!bReqVec94.HasValue)  // ReqAnz94 valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") &&
                        lCodNatura.Contains(CodeNatura.Substring(2, 1).ToUpperInvariant()))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        if (Decorrenza.Value.CompareTo(dtMin) > 0)   // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94_CatMix_9709(bool? bReqVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };

            if (!bReqVec94.HasValue)  // ReqAnz94 valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2"))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94_CatMix_97(bool? bReqVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };
            List<string> lCodNatura = new List<string> { "I", "U", "L" };

            if (!bReqVec94.HasValue)                                                                // ReqAnz94 valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))                                       // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") && lCodNatura.Contains(CodeNatura.Substring(2, 1)))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        if (Decorrenza.Value.CompareTo(dtMin) > 0)          // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqVec94_CodNatura_9509(bool? bReqVec94, string CodeNatura, string Gruppo, string prodotto, DateTime? Decorrenza, bool isRiaperturaDomanda)
        {
            if (String.IsNullOrEmpty(Gruppo) || !Decorrenza.HasValue)
                return false;

            // Il controllo deve scattare se il primo codice natura è blank o 6 ed è una pl con gruppo 0001 con 'Requisiti Vecchiaia al 12/94 non valorizzato
            // e la decorrenza pensione compresa tra 01/01/1995 e 01/12/2000
            if (CodeNatura == null || CodeNatura.Substring(0, 1) == " " || CodeNatura.Substring(0, 1) == "6")
            {
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
                {
                    if (Gruppo.Equals("0001"))
                    {
                        if (!bReqVec94.HasValue)
                        {
                            DateTime dtMin = new DateTime(1995, 01, 01);
                            DateTime dtMax = new DateTime(2000, 12, 01);

                            if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94CategDecorrenza(bool? bReqAnz94, string CodeNatura, string Gruppo, string prodotto, DateTime? Decorrenza, bool isRiaperturaDomanda)
        {
            if (String.IsNullOrEmpty(Gruppo) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
            List<string> lCodNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "U", "L", "Z", "O" };

            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda) //prime liquidate
            {
                if (bReqAnz94.HasValue)  // ReqAnz94 valorizzato
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") &&
                        lCodNatura.Contains(CodeNatura.Substring(2, 1).ToUpperInvariant()))
                    {
                        DateTime dtMin = new DateTime(1994, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);
                        DateTime dt = new DateTime(1995, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0 && Decorrenza.Value.CompareTo(dt) != 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 mancante (S/N) 
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqAnzVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnzVec94_CatVOS_9596(bool? bReqAnzVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            if (!bReqAnzVec94.HasValue)  // ReqAnz94 non valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2")) // codeNatura
                    {
                        DateTime dtMin = new DateTime(1995, 01, 01);
                        DateTime dtMax = new DateTime(1996, 10, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 mancante (S/N) 
        /// Requisito vecchiaia al 12/94 mancante (S/N) 
        /// Requisito anzianità al 09/96 mancante (S/N)
        /// </summary>
        /// <param name="bReqAnzVec94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnzVec94Anz96_CatVOS_9698(bool? bReqAnzVec94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            if (!bReqAnzVec94Anz96.HasValue)  // ReqAnzVec94Anz96 non valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2")) // codeNatura
                    {
                        DateTime dtMin = new DateTime(1996, 09, 01);
                        DateTime dtMax = new DateTime(1998, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza 
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94Anz96_CatVOS_9709(bool? bReqAnz94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            if (bReqAnz94Anz96.HasValue)  // ReqAnz94 valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2")) // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza ; 
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94Anz96_CatVOS_97(bool? bReqAnz94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCodNatura = new List<string> { "I", "U", "L" };

            if (bReqAnz94Anz96.HasValue)  // ReqAnz94 valorizzato
            {
                if (Categoria.Trim().ToUpperInvariant() == "VOS")  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") &&
                        lCodNatura.Contains(CodeNatura.Substring(2, 1).ToUpperInvariant()))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        if (Decorrenza.Value.CompareTo(dtMin) > 0)   // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 mancante (S/N) 
        /// Requisito vecchiaia al 12/94 mancante (S/N)
        /// </summary>
        /// <param name="bReqAnzVec94"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnzVec94_CatMix_9597(bool? bReqAnzVec94, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };

            if (!bReqAnzVec94.HasValue)  // ReqAnz94 non valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2"))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1995, 01, 01);
                        DateTime dtMax = new DateTime(1997, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 mancante (S/N) 
        /// Requisito vecchiaia al 12/94 mancante (S/N) 
        /// Requisito anzianità al 09/96 mancante (S/N)
        /// </summary>
        /// <param name="bReqAnzVec94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnzVec94Anz96_CatMix_9698(bool? bReqAnzVec94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };

            if (!bReqAnzVec94Anz96.HasValue)  // non valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2"))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1996, 12, 01);
                        DateTime dtMax = new DateTime(1998, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza 
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94Anz96_CatMix_9709(bool? bReqAnz94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };

            if (bReqAnz94Anz96.HasValue)  // valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))  // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2"))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza 
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza
        /// </summary>
        /// <param name="bReqAnz94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94Anz96_CatMix_97(bool? bReqAnz94Anz96, string CodeNatura, string Categoria, DateTime? Decorrenza)
        {
            if (String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCategoria = new List<string> { "VRS", "VOARTS", "VOCOMS" };
            List<string> lCodNatura = new List<string> { "I", "U", "L" };

            if (bReqAnz94Anz96.HasValue)           // ReqAnz94Anz96 valorizzato
            {
                if (lCategoria.Contains(Categoria.ToUpperInvariant()))                                       // categoria
                {
                    if ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") && lCodNatura.Contains(CodeNatura.Substring(2, 1)))   // codeNatura
                    {
                        DateTime dtMin = new DateTime(1997, 12, 01);
                        if (Decorrenza.Value.CompareTo(dtMin) > 0)          // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 errato (non deve essere acquisito) 
        /// Requisito anzianità al 09/96 errato (non deve essere acquisito)
        /// </summary>
        /// <param name="bReqAnz94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnz94Anz96_CodNatura_9509(bool? bReqAnz94Anz96, string CodeNatura, string Gruppo, string prodotto, DateTime? Decorrenza, bool isRiaperturaDomanda)
        {
            if (String.IsNullOrEmpty(Gruppo) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)                                     //prime liquidate
            {
                if (bReqAnz94Anz96.HasValue)        // ReqAnz94Anz96 valorizzato
                {
                    if ((CodeNatura.Substring(0, 1) != "1" && CodeNatura.Substring(0, 1) != "2"))      // codeNatura
                    {
                        DateTime dtMin = new DateTime(1995, 01, 01);
                        DateTime dtMax = new DateTime(2009, 01, 01);

                        if (Decorrenza.Value.CompareTo(dtMin) > 0 && Decorrenza.Value.CompareTo(dtMax) < 0)    // decorrenza
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza ; Requisito anzianità al 12/94 incompatibile con Categoria o Decorrenza (S)
        /// Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza ; Requisito vecchiaia al 12/94 incompatibile con Categoria o Decorrenza (S)
        /// Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza ; Requisito anzianità al 09/96 incompatibile con Categoria o Decorrenza (S)
        /// </summary>
        /// <param name="bReqAnzVec94Anz96"></param>
        /// <param name="CodeNatura"></param>
        /// <param name="Gruppo"></param>
        /// <param name="Categoria"></param>
        /// <param name="Decorrenza"></param>
        /// <returns></returns>
        public static bool VerificaReqAnzVec94Anz96_Fin95(bool? bReqAnzVec94Anz96, string CodeNatura, string Gruppo, string prodotto, string Categoria, DateTime? Decorrenza, bool? isAssicurativiAcquisito,
            out bool bFalse1, out bool bFalse2)
        {
            bFalse1 = true;
            bFalse2 = true;

            if (isAssicurativiAcquisito.HasValue && !isAssicurativiAcquisito.Value)
                return true;

            if (String.IsNullOrEmpty(Gruppo) || String.IsNullOrEmpty(Categoria) || !Decorrenza.HasValue)
                return false;

            if (CodeNatura == null || CodeNatura.Trim() == string.Empty)
                return true;

            List<string> lCodNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "U", "L", "Z", "O" };
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo, prodotto);
            //ENG - Modificato controllo come su Ex-Eap
            if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione && (CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") && lCodNatura.Contains(CodeNatura.Substring(2, 1))) ||
                ((CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2") && Categoria.Trim().ToUpperInvariant() == "VOS") ||
                ((CodeNatura.Substring(0, 1) != "1" && CodeNatura.Substring(0, 1) != "2") && (Categoria.Trim().ToUpperInvariant() == "VRS" || Categoria.Trim().ToUpperInvariant() == "VOARTS" || Categoria.Trim().ToUpperInvariant() == "VOCOMS")) ||
                (tipoDomanda != Utility.TipoDomanda.Ricostituzione && (CodeNatura.Substring(0, 1) != "1" && CodeNatura.Substring(0, 1) != "2")))
            {
                DateTime dtMin = new DateTime(1995, 01, 01);
                if ((Categoria.Trim().ToUpperInvariant() == "VOS" || Categoria.Trim().ToUpperInvariant() == "VRS" || Categoria.Trim().ToUpperInvariant() == "VOARTS" || Categoria.Trim().ToUpperInvariant() == "VOCOMS") &&
                   (CodeNatura.Substring(0, 1) == "1" || CodeNatura.Substring(0, 1) == "2" || CodeNatura.Substring(0, 1) == "3" || CodeNatura.Substring(0, 1) == "4") &&
                    lCodNatura.Contains(CodeNatura.Substring(2, 1)) &&
                    Decorrenza.Value.CompareTo(dtMin) == 0)
                {
                    if (!bReqAnzVec94Anz96.HasValue || !bReqAnzVec94Anz96.Value)
                    {
                        bFalse2 = false;
                        return false;
                    }
                }
                else
                {
                    //if (bReqAnzVec94Anz96.HasValue )
                    //{
                    //    bFalse1 = false;
                    //    return false;
                    //}

                    //controllo allentato in seguito alle segnalazioni sui Requisiti 12/94
                    if (bReqAnzVec94Anz96.HasValue && bReqAnzVec94Anz96.GetValueOrDefault() != false)
                    {
                        bFalse1 = false;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica se la decorrenzaArretrati non è valorizzata e, se valorizzata, che sia maggiore della data odierna + 1 mese
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <returns>False se decorrenzaArretrati è null o maggiore della data odierna, true altrimenti</returns>
        public static bool VerificaDecorrenzaArretrati(DateTime? decorrenzaArretrati)
        {
            DateTime dataOdierna = Utility.DataSistemaCi.AddMonths(1);
            if (!decorrenzaArretrati.HasValue || decorrenzaArretrati.Value.CompareTo(dataOdierna) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se la decorrenzaArretrati sia minore della decorrenzaCalcolo
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="decorrenzaCalcolo"></param>
        /// <returns>False se la decorrenzaArretrati è minore della decorrenzaCalcolo, true altrimenti</returns>
        public static bool VerificaDecorrenzaArretratiWithDataInizioCalcolo(DateTime? decorrenzaArretrati, DateTime? decorrenzaCalcolo)
        {
            DateTime? decorrenzaCalcoloCompare = null;
            if (decorrenzaCalcolo.HasValue)
                decorrenzaCalcoloCompare = new DateTime(decorrenzaCalcolo.Value.Year, decorrenzaCalcolo.Value.Month, 1);

            if (decorrenzaArretrati.HasValue && decorrenzaCalcoloCompare.HasValue && decorrenzaArretrati.Value.CompareTo(decorrenzaCalcoloCompare.Value) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se la decorrenzaArretrati è inferiore al 01/1983 e che causaCarico è pari a 2
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="causaCarico"></param>
        /// <returns>False se decorrenzaArretrati è inferiore al 01/1983 e che causaCarico è pari a 2, true altrimenti</returns>
        public static bool VerificaDecorrenzaArretratiWithGennaio1983(DateTime? decorrenzaArretrati, byte? causaCarico)
        {
            if (!decorrenzaArretrati.HasValue)
                return false;

            DateTime dateCompare = new DateTime(1983, 1, 1);
            if (causaCarico.HasValue && decorrenzaArretrati.Value.CompareTo(dateCompare) < 0 && causaCarico.Value == 2)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se la decorrenzaArretrati è inferiore a 10 anni prima della presentazione della domanda con causaCarico diversa da 2
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="causaCarico"></param>
        /// <param name="dataPresentazioneDomanda"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaArretratiWithDataPresentazione(DateTime? decorrenzaArretrati, byte? causaCarico, DateTime? dataPresentazioneDomanda)
        {
            if (causaCarico.HasValue && causaCarico.Value != 2 && dataPresentazioneDomanda.HasValue && decorrenzaArretrati.HasValue && dataPresentazioneDomanda.Value.AddYears(-10).Year > decorrenzaArretrati.Value.Year)
                return false;

            return true;
        }

        public static bool ControlsCodiceVirtuale(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, byte? codiceConvenzione, char? codiceVirtuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!VerificaCodiceVirtuale(codiceVirtuale))
            {
                messaggioVideo = "Codice Virtuale non ammesso ('2', '5', '6', ' ')";
                return false;
            }

            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                {
                    if (!new List<char> { ' ', '2' }.Contains(codiceVirtuale.Value))
                    {
                        messaggioVideo = string.Format("Codice Virtuale '{0}' non ammesso (codici ammessi: ' ', '2').", codiceVirtuale.Value);
                        return false;
                    }
                }
            }
            else
            {
                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                {
                    if (!new List<char> { '2', '5', '6' }.Contains(codiceVirtuale.Value))
                    {
                        messaggioVideo = string.Format("Codice Virtuale '{0}' non ammesso (codici ammessi: '2', '5', '6').", codiceVirtuale.Value);
                        return false;
                    }
                }
                else
                {
                    if (codiceConvenzione.HasValue)
                    {
                        char codiceVirtualeCompare = codiceConvenzione.Value == 17 ? ' ' : '2';
                        if (codiceVirtualeCompare != codiceVirtuale.Value)
                        {
                            messaggioVideo = string.Format("Codice Virtuale '{0}' non ammesso (codice ammesso: '{1}').", codiceVirtuale.Value, codiceVirtualeCompare);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica che il codiceVirtuale sia uguale ai valori 2, 5, 6 e spazio
        /// </summary>
        /// <param name="codiceVirtuale"></param>
        /// <returns>False se il codiceVirtuale è diverso da 2, 5, 6 e spazio, true altrimenti</returns>
        public static bool VerificaCodiceVirtuale(char? codiceVirtuale)
        {
            if (codiceVirtuale.HasValue && (codiceVirtuale.Value == ' ' || codiceVirtuale.Value == '2' || codiceVirtuale.Value == '5' || codiceVirtuale.Value == '6'))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verifica se codiceVirtuale è 6, allora causaCarico deve essere 9 o 2
        /// </summary>
        /// <param name="codiceVirtuale"></param>
        /// <param name="causaCarico"></param>
        /// <returns>False se codiceVirtuale è diverso da 6 e causaCarico è diverso da 9 o 2, true altrimenti</returns>
        public static bool VerificaCodiceVirtualeWithCausaCarico(char? codiceVirtuale, byte? causaCarico)
        {
            if (codiceVirtuale.HasValue && codiceVirtuale.Value == '6')
            {
                if (causaCarico.HasValue && (causaCarico.Value == 9 || causaCarico.Value == 2))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Verifica se codiceConvenzione è 17 e la cittadinanza è diversa tra quelle indicate e non è una reversibilità
        /// </summary>
        /// <param name="codiceVirtuale"></param>
        /// <param name="cittadinanza"></param>
        /// <param name="gruppo"></param>
        /// <returns>False se codiceConvenzione è 17 e la cittadinanza è diversa tra quelle indicate e non è una reversibilità, true altrimenti</returns>
        public static bool VerificaCodiceConvenzioneWithCittadinanza(byte? codiceConvenzione, string cittadinanza, string gruppo, string prodotto)
        {
            if (string.IsNullOrEmpty(gruppo))
                return false;

            List<string> cittadinanzeList = new List<string> { "I", "CH", "F", "B", "GB", "L", "NL", "A", "D", "E", "DK", "IR", "GR", "P", "IS", "SF" };

            bool isCittadinanzaPresent;
            if (string.IsNullOrEmpty(cittadinanza.Trim()))
                isCittadinanzaPresent = false;
            else
                isCittadinanzaPresent = cittadinanzeList.Contains(cittadinanza.Trim().ToUpperInvariant());

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);

            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 17 && !isCittadinanzaPresent && tipoDomanda != Utility.TipoDomanda.Superstiti)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se codiceConvenzione non è 17 e il codiceVirtuale non è pari a 2, 5 e 6 e se per codiceVirtuale pari a spazio o 6, 
        /// la causaCarico non è pari a 9 e 2, per reversibilità
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceVirtuale"></param>
        /// <param name="causaCarico"></param>
        /// <param name="gruppo"></param>
        /// <returns>
        /// False se codiceConvenzione non è 17 e il codiceVirtuale non è pari a 2, 5 e 6 e se per codiceVirtuale pari a spazio o 6, 
        /// la causaCarico non è pari a 9 e 2, per reversibilità, true altrimenti
        /// </returns>
        public static bool VerificaCodiceConvenzioneWithCodiceVirtualeReversibilita(byte? codiceConvenzione, char? codiceVirtuale, byte? causaCarico, string gruppo, string prodotto, out string msg)
        {
            msg = string.Empty;

            if (string.IsNullOrEmpty(gruppo))
                return false;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);

            if (codiceConvenzione.HasValue && codiceConvenzione.Value != 17)
                if (codiceVirtuale.HasValue && (codiceVirtuale.Value != '2' && codiceVirtuale.Value != '5' && codiceVirtuale.Value != '6'))
                    if (codiceVirtuale.Value == ' ' || codiceVirtuale.Value == '6' && causaCarico.HasValue && (causaCarico.Value == 9 || causaCarico.Value == 2))
                    {
                        if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                            msg = "Se convenzione diversa da 17, Codice Virtuale deve essere 2-5";
                        else
                            msg = "Se convenzione diversa da 17, Codice Virtuale deve essere 2";

                        return false;
                    }
            return true;
        }

        /// <summary>
        /// Verifica se causaCarico è 2 ma dataRicezione è spazio (non è una ricostituzione)
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="dataRicezione"></param>
        /// <returns>False se causaCarico è 2 ma dataRicezione è spazio, true altrimenti</returns>
        public static bool VerificaCausaCarico(byte? causaCarico, string gruppo, string prodotto, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = "Codice Causa Carico errato o mancante (1 - 3 - 9)";
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);

            if (!causaCarico.HasValue)
                return false;

            if ((causaCarico.Value != 1 && causaCarico.Value != 3 && causaCarico.Value != 9))
                if (causaCarico.Value == 2 && (tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda))
                    return false;

            //if ((!causaCarico.HasValue || (causaCarico.HasValue && (causaCarico.Value != 1 && causaCarico.Value != 3 && causaCarico.Value != 9))) && causaCarico.Value == 2 && tipoDomanda != Utility.TipoDomanda.Ricostituzione)
            //    return false;

            if (tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti)
                if (causaCarico.Value != 9)
                {
                    messaggioVideo = "Codice Causa Carico deve essere 9";
                    return false;
                }

            return true;
        }

        /// <summary>
        /// Verifica se dataInizioAssicurazione non è valorizzata o supera la data odierna
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <returns>False se dataInizioAssicurazione non è valorizzata o supera la data odierna, true altrimenti</returns>
        public static bool VerificaInizioAssicurazione(GestionePensione.DatiPensione datiPensione, DateTime? dataInizioAssicurazione, bool isRiaperturaDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!dataInizioAssicurazione.HasValue)
            {
                messaggioVideo = "Data Inizio Assicurazione illogica o mancante.";
                return false;
            }

            if (dataInizioAssicurazione.Value.CompareTo(Utility.DataSistemaCi) > 0)
            {
                messaggioVideo = "Data Inizio Assicurazione illogica o mancante.";
                return false;
            }

            //FG - Controlli tipo contributivo - data inizio assicurazione
            //ENG - Memo 166/2023
            if ((Utility.IsDomandaTipoContributivo(datiPensione, null, false) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione)) && !isRiaperturaDomanda)
            {
                if (!Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1996, 01, 01)))
                {
                    messaggioVideo = "La data di inizio assicurazione non può essere inferiore al 1996";
                    return false;
                }
            }
            //if ((Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)) &&
            //     !(datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && datiPensione.Tipo.Trim() == "0030"))
            //{
            //    if (Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1996, 01, 01)))
            //    {
            //        messaggioVideo = "La data di inizio assicurazione deve essere inferiore al 1996";
            //        return false;
            //    }
            //}

            return true;
        }

        /// <summary>
        /// Verifica se dataInizioAssicurazione è maggiore della dataNascitaTitolare + 100 anni
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <returns>False se dataInizioAssicurazione è maggiore della dataNascitaTitolare + 100 anni, true altrimenti</returns>
        public static bool VerificaInizioAssicurazioneWithDataNascitaTitolare(DateTime? dataInizioAssicurazione, DateTime? dataNascitaTitolare)
        {
            if (dataInizioAssicurazione.HasValue && dataNascitaTitolare.HasValue && dataInizioAssicurazione.Value.CompareTo(dataNascitaTitolare.Value.AddYears(100)) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se dataInizioAssicurazione non è minore della decorrenzaOriginaria
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns>False se dataInizioAssicurazione non è minore della decorrenzaOriginaria, true altrimenti</returns>
        public static bool VerificaInizioAssicurazioneWithDecorrenzaOriginaria(DateTime? dataInizioAssicurazione, DateTime? decorrenzaOriginaria)
        {
            if (dataInizioAssicurazione.HasValue && decorrenzaOriginaria.HasValue && dataInizioAssicurazione.Value.CompareTo(decorrenzaOriginaria.Value) >= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se dataFineAssicurazione non è valorizzata o supera la data odierna
        /// </summary>
        /// <param name="dataFineAssicurazione"></param>
        /// <returns>False se dataFineAssicurazione non è valorizzata o supera la data odierna, true altrimenti</returns>
        public static bool VerificaFineAssicurazione(DateTime? dataFineAssicurazione)
        {
            if (!dataFineAssicurazione.HasValue)
                return false;

            if (dataFineAssicurazione.Value.CompareTo(Utility.DataSistemaCi) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se dataFineAssicurazione non è maggiore di dataInizioAssicurazione
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <returns>False se dataFineAssicurazione non è maggiore di dataInizioAssicurazione, true altrimenti</returns>
        public static bool VerificaInizioAssicurazioneWithFineAssicurazione(DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione)
        {
            if (dataInizioAssicurazione.HasValue && dataFineAssicurazione.HasValue && dataInizioAssicurazione.Value.CompareTo(dataFineAssicurazione.Value) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se dataFineAssicurazione non è minore della decorrenzaOriginaria
        /// </summary>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns>False se dataFineAssicurazione non è minore della decorrenzaOriginaria, true altrimenti</returns>
        public static bool VerificaFineAssicurazioneWithDecorrenzaOriginaria(DateTime? dataFineAssicurazione, DateTime? decorrenzaOriginaria)
        {
            if (dataFineAssicurazione.HasValue && decorrenzaOriginaria.HasValue && dataFineAssicurazione.Value.CompareTo(decorrenzaOriginaria.Value) >= 0)
                return false;

            return true;
        }

        /// <summary>
        ///  Per le pensioni ai superstiti e trasormazioni AOI il numero di settimane OBG + VV Diritto + settimane diritto deve essere maggiore di 259
        /// </summary>
        /// <param name="nSettimaneOBG"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneOBGSettimaneDiritto(GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda, int? nSettimaneOBG, int? nContributiVolontari,
            int? settimaneEstere, int? settimaneItalianeDiritto, int? settimaneItalianeMisura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int? sommaSettimane = null;
            string descSettimane1 = "";
            string descSettimane2 = "";

            if (datiPensione.SiglaCategoria.Trim() == "VOS" || datiPensione.SiglaCategoria.Trim() == "IOS" || datiPensione.SiglaCategoria.Trim() == "SOS")
            {
                sommaSettimane = nSettimaneOBG.GetValueOrDefault() + nContributiVolontari.GetValueOrDefault();
                descSettimane1 = "OBG";
                descSettimane2 = "VV Diritto";
            }
            else
            {
                sommaSettimane = settimaneItalianeDiritto.GetValueOrDefault() + settimaneItalianeMisura.GetValueOrDefault();
                descSettimane1 = "Italiane Diritto";
                descSettimane2 = "Italiane Misura";
            }

            if (tipoDomanda == Utility.TipoDomanda.Superstiti || (Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault()))
            {
                if (sommaSettimane + settimaneEstere < 260)
                {
                    messaggioVideo = "Il numero di settimane " + descSettimane1 + " + settimane " + descSettimane2 + " + settimane Diritto deve essere maggiore di 259";
                    return false;
                }
            }
            return true;
        }

        public static bool ControlsDataInizioCalcolo(DateTime? dataInizioCalcolo, DateTime? dataInteressiLegali, byte? codiceDomandaRicorso, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataInizioCalcolo.HasValue)
            {
                if (dataInteressiLegali.HasValue)
                {
                    messaggioVideo = "La data interessi legali non deve essere acquisita in presenza della decorrenza ripristino";
                    return false;
                }

                if (!codiceDomandaRicorso.HasValue || codiceDomandaRicorso.Value != 9)
                {
                    messaggioVideo = "Il campo codice domanda ricorso deve essere uguale a 9";
                    return false;
                }

                if (!Utility.DataSuccessivaA(dataInizioCalcolo.Value, new DateTime(1965, 1, 1)))
                {
                    messaggioVideo = "La data ripristino non può essere inferiore a gennaio 1965";
                    return false;
                }

                if (Utility.DataStrettamenteSuccessivaA(dataInizioCalcolo.Value, new DateTime(Utility.DataSistemaCi.Year, 01, 31)))
                {
                    messaggioVideo = "La data ripristino non può essere superiore a gennaio dell'anno in corso";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaDataPerfezionamentoPerPensioneTipoContributivo(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime dataSistema, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //ENG - MEMO 166/2023
            if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
            {
                int numSettimaneTipoContibutivo = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPrestazioniEstere, datiPensione);
                if (!GestioneCrossControls.ALL_VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAnagraficiTitolare, numSettimaneTipoContibutivo, out messaggioVideo))
                    return false;
            }
            return true;
        }

        public static bool VerificaDataPerfezionamentoPerPensioneTipoContributivo(GestionePensione.DatiPensione datiPensione, int? nSettimaneOBG, int? nContributiVolontari, int? nSettimaneItalianeDiritto,
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare, DateTime dataSistema, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //ENG - MEMO 166/2023
            if (Utility.IsDomandaTipoContributivo(datiPensione, null, null) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione))
            {
                int numSettimaneTipoContibutivo = GestioneCrossControls.CI_GetNumeroSettimane(nSettimaneOBG, nContributiVolontari, nSettimaneItalianeDiritto, listaPrestazioniEstere, datiPensione);
                if (!GestioneCrossControls.ALL_VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAnagraficiTitolare, numSettimaneTipoContibutivo, out messaggioVideo))
                    return false;
            }
            return true;
        }

        public static bool ControlsNSettimanePerRequisitoAnticipatoArt1(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1(datiPensione) ||
                Utility.IsDomandaVecchiaiaRequisitoAnticipatoArt1OpzioneContributivo(datiPensione))
            {
                int nLimiteSettimane = 1560;
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);

                if (nSettimane < nLimiteSettimane)
                {
                    messaggioVideo = string.Format("Il numero delle settimane deve essere maggiore o uguale a {0}", nLimiteSettimane);
                    return false;
                }
            }
            return true;
        }


        #region PCIPL35
        /// <summary>
        /// Verifica in base al codice natura se la decorrenza originaria deve essere posteriore alla data presentazione domanda
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="causaCarico"></param>
        /// <param name="codNatura"></param>
        /// <param name="dataPresentazioneDomanda"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="attivitaEconomica"></param>
        /// <param name="professioneIndividuale"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaOriginariaWithCodNaturaAndDataPresentazione(GestionePensione.DatiPensione datiPensione, byte? causaCarico, string codNatura, int? attivitaEconomica,
            int? professioneIndividuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (causaCarico.HasValue && datiPensione.DecorrenzaOriginaria.HasValue && attivitaEconomica.HasValue && professioneIndividuale.HasValue)
            {
                if ((datiPensione.Gruppo == "0001" && causaCarico.Value == 1 && (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2")) ||
                    (datiPensione.Gruppo == "0002" && causaCarico.Value == 1))
                {
                    if (datiPensione.DecorrenzaOriginaria.Value.CompareTo(new DateTime(datiPensione.DataPresentazioneDomanda.Year, datiPensione.DataPresentazioneDomanda.Month, 01)) <= 0)
                    {
                        if (!((codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2") && (codNatura.Substring(2, 1) == "J" ||
                            codNatura.Substring(2, 1) == "K" || codNatura.Substring(2, 1) == "Q" || codNatura.Substring(2, 1) == "W" ||
                            codNatura.Substring(2, 1) == "X" || codNatura.Substring(2, 1) == "Y" || codNatura.Substring(2, 1) == "P" ||
                            codNatura.Substring(2, 1) == "U" || codNatura.Substring(2, 1) == "L" || codNatura.Substring(2, 1) == "Z" ||
                            codNatura.Substring(2, 1) == "O")) &&
                            !(datiPensione.DecorrenzaOriginaria.Value.CompareTo(new DateTime(1995, 01, 01)) == 0 && datiPensione.Gruppo == "0001" && codNatura.Substring(0, 1) == "1" &&
                            datiPensione.DataPresentazioneDomanda.CompareTo(new DateTime(1995, 05, 16)) < 0 && attivitaEconomica.Value == 94 && professioneIndividuale.Value == 724) &&
                            !Utility.IsDomandaAPEPrecoci(datiPensione))
                        {
                            messaggioVideo = "Decorrenza Originaria deve essere posteriore a Data Domanda";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Rif. PCIPL35
        /// </summary>
        /// <returns></returns>
        public static bool ControlsCodNaturaForDatiGenerici(GestionePensione.DatiPensione datiPensione, string codNatura, DateTime? decorrenzaOriginaria, string gruppo, string categoria, byte? codiceArretrati, string codiceComuneResidenza, byte? causaCarico, DateTime? dataPresentazioneDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            if (codNatura.Substring(0, 2) == "6T" && decorrenzaOriginaria.Value.CompareTo(new DateTime(1983, 10, 01)) > 0)
            {
                messaggioVideo = "Natura Pensione \"6T\" incompatibile con Decorrenza Originaria";
                return false;
            }

            if (codNatura.Substring(0, 2) == "6L" && decorrenzaOriginaria.Value.CompareTo(new DateTime(1982, 07, 01)) > 0)
            {
                messaggioVideo = "Natura Pensione \"6L\" incompatibile con Decorrenza Originaria";
                return false;
            }

            if (codNatura.Substring(0, 1) == " " && codNatura.Substring(2, 1) != "V" && codNatura.Substring(2, 1) != "Z" && codNatura.Substring(2, 1) != "G" &&
               (codNatura.Substring(1, 1) != " " || codNatura.Substring(2, 1) != " ") && codNatura.Substring(1, 1) != "X" && codNatura.Substring(1, 1) != "Y")
            {
                if (!(codNatura.Substring(0, 1) == " " && codNatura.Substring(1, 1) == " " && (codNatura.Substring(2, 1) == "L" || codNatura.Substring(2, 1) == "H") || ((Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)) && codNatura.Substring(1, 1) == "J") ||
                    (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && codNatura.Substring(1, 1) == "J") ||
                    (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && codNatura.Substring(1, 1) == "J"))
                    && !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Suppl_Inf_A_2Anni_Per_Sent_CI.SUPPL_INF_A_2ANNI_PER_SENT_CI))
                {
                    messaggioVideo = "Carattere in Natura non ammesso";
                    return false;
                }
            }

            if ((codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2") && !Utility.IsRicostituzione(gruppo) && !Utility.IsDomandaRiliquidazioneAnzianitaAnticipata(datiPensione) && !Utility.IsDomandaRipristinoAnzianitaAnticipata(datiPensione))
            {
                if (gruppo != "0001" && gruppo != "0003")
                {
                    messaggioVideo = "Natura Pensione '1/2' ammesso solo per pensione di anzianità";
                    return false;
                }
            }

            if ((codNatura.Substring(1, 1) == "A" || codNatura.Substring(1, 1) == "C") && (codNatura.Substring(0, 1) == " " ||
                codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "3" || codNatura.Substring(0, 1) == "5" ||
                codNatura.Substring(0, 1) == "8"))
            {
                messaggioVideo = "Incompatibilità tra i primi due codici della Natura Pensione";
                return false;
            }

            if (codNatura.Substring(1, 1) != " " && codNatura.Substring(1, 1) != "A" && codNatura.Substring(1, 1) != "C" &&
                codNatura.Substring(1, 1) != "F" && codNatura.Substring(1, 1) != "P" && codNatura.Substring(1, 1) != "R" &&
                codNatura.Substring(1, 1) != "T" && codNatura.Substring(1, 1) != "G" && codNatura.Substring(1, 1) != "L" &&
                codNatura.Substring(1, 1) != "Z" && codNatura.Substring(1, 1) != "X" && codNatura.Substring(1, 1) != "Y" &&
                codNatura.Substring(1, 1) != "O")
            {
                if (!(((Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione)) && codNatura.Substring(1, 1) == "J") ||
                    (!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && codNatura.Substring(1, 1) == "J") ||
                    (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && (Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && codNatura.Substring(1, 1) == "J"))
                    && !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Suppl_Inf_A_2Anni_Per_Sent_CI.SUPPL_INF_A_2ANNI_PER_SENT_CI))
                {
                    messaggioVideo = "2° codice Natura Pensione non ammesso";
                    return false;
                }
            }

            if (codNatura.Substring(2, 1) != " " && codNatura.Substring(2, 1) != "S" && codNatura.Substring(2, 1) != "E" &&
                codNatura.Substring(2, 1) != "M" && codNatura.Substring(2, 1) != "H" && codNatura.Substring(2, 1) != "Q" &&
                codNatura.Substring(2, 1) != "V" && codNatura.Substring(2, 1) != "X" && codNatura.Substring(2, 1) != "N" &&
                codNatura.Substring(2, 1) != "K" && codNatura.Substring(2, 1) != "W" && codNatura.Substring(2, 1) != "Y" &&
                codNatura.Substring(2, 1) != "J" && codNatura.Substring(2, 1) != "P" && codNatura.Substring(2, 1) != "U" &&
                codNatura.Substring(2, 1) != "L" && codNatura.Substring(2, 1) != "Z" && codNatura.Substring(2, 1) != "G" &&
                codNatura.Substring(2, 1) != "O")
            {
                messaggioVideo = "3° codice Natura Pensione non ammesso";
                return false;
            }

            if (decorrenzaOriginaria.Value.CompareTo(new DateTime(1981, 05, 01)) > 0 && (codNatura.Substring(2, 1) == "J" || codNatura.Substring(2, 1) == "K" ||
                codNatura.Substring(2, 1) == "Q" || codNatura.Substring(2, 1) == "W" || codNatura.Substring(2, 1) == "X" ||
                codNatura.Substring(2, 1) == "Y" || codNatura.Substring(2, 1) == "P" || codNatura.Substring(2, 1) == "U" ||
                codNatura.Substring(2, 1) == "L" || codNatura.Substring(2, 1) == "O"))
            {
                if (codNatura.Substring(0, 1) != "1" && codNatura.Substring(0, 1) != "2")
                {
                    if (!(codNatura.Substring(0, 1) == " " && decorrenzaOriginaria.Value.CompareTo(new DateTime(2003, 01, 01)) > 0 && decorrenzaOriginaria.Value.CompareTo(new DateTime(2004, 03, 01)) < 0))
                    {
                        messaggioVideo = "Incompatibilità tra 1° e 3° codice Natura Pensione";
                        return false;
                    }
                }
            }

            if (codNatura.Substring(0, 1) != " " && codNatura.Substring(0, 1) != "1" && codNatura.Substring(0, 1) != "2" &&
                codNatura.Substring(0, 1) != "3" && codNatura.Substring(0, 1) != "4" && codNatura.Substring(0, 1) != "6" &&
                codNatura.Substring(0, 1) != "8" && codNatura.Substring(0, 1) != "9" && codNatura.Substring(0, 1) != "X" &&
                codNatura.Substring(0, 1) != "Y")
            {
                messaggioVideo = "Codice Natura Pensione non ammesso";
                return false;
            }

            if (codNatura.Substring(0, 1) == "3" || codNatura.Substring(0, 1) == "4")
            {
                if (gruppo == "0001")
                {
                    messaggioVideo = "Natura Pensione 3/4 incompatibile con categoria Vecchiaia";
                    return false;
                }

                if (gruppo == "0002" && decorrenzaOriginaria.Value.CompareTo(new DateTime(1984, 08, 01)) < 0)
                {
                    messaggioVideo = "Natura Pensione 3/4 incompatibile con invalidità ante 08/1984";
                    return false;
                }
            }

            if (codNatura.Substring(2, 1) == "U")
            {
                if (categoria.Trim() != "VOS" || codiceArretrati.Value != 8 || !codiceComuneResidenza.StartsWith("Z"))
                {
                    messaggioVideo = "Incompatibilità tra Natura 'U' e Codice Arretrati/Categoria/Residenza";
                    return false;
                }
            }

            if (codNatura.Substring(2, 1) == "H" && causaCarico.HasValue && causaCarico.Value != 2)
            {
                if ((codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2") && Utility.DataStrettamenteSuccessivaA(dataPresentazioneDomanda.Value, new DateTime(2004, 09, 28)))
                {
                    messaggioVideo = "Trasf. in anzianità non possibile per domande post 28/09/2004";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica che la delibera 126/88 sia compatibile con il codice natura
        /// </summary>
        /// <param name="delibera12688"></param>
        /// <param name="codNatura"></param>
        /// <param name="gruppo"></param>
        /// <returns></returns>
        public static bool VerificaDelibera12688WithCodNatura(bool? delibera12688, string codNatura, string gruppo)
        {
            if (delibera12688.HasValue && delibera12688.Value)
                if ((codNatura.Substring(0, 1) != "1" && codNatura.Substring(0, 1) != "2") || (gruppo == "0002"))
                    return false;

            return true;
        }

        /// <summary>
        /// Calcola la decorrenza per le finestre 247. Rif. PCIPL35 - CALCOLA-DECO
        /// </summary>
        /// <returns>La decorrenza calcolata</returns>
        public static DateTime? CalcolaDecorrenza(DateTime? decCompare, DateTime? dataPerfReq, char? sessoTitolare, DateTime? dataNascitaTitolare, byte? legge44997, string categoria, byte? codiceCieco, char? codiceParticolareSoggettoDerogato, int? attivitaEconomica, int? professioneIndividuale)
        {
            DateTime? dataPerfReqCompare = null;
            string categoriaNumerica = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);
            int categoriaNum = 0;
            int.TryParse(categoriaNumerica, out categoriaNum);

            if (sessoTitolare.Value == 'M')
                decCompare = dataNascitaTitolare.Value.AddYears(65).AddDays(-dataNascitaTitolare.Value.Day + 1);
            else
                decCompare = dataNascitaTitolare.Value.AddYears(60).AddDays(-dataNascitaTitolare.Value.Day + 1);

            if (legge44997.GetValueOrDefault() == 6 && categoria.Trim() == "VOS")
                decCompare = decCompare.Value.AddYears(-5);

            if (codiceCieco.GetValueOrDefault() == 1)
            {
                if (categoria.Trim() == "VOS")
                    decCompare = decCompare.Value.AddYears(-10);

                if (categoria.Trim() == "VRS" || categoria.Trim() == "VOARTS" || categoria.Trim() == "VOCOMS")
                    decCompare = decCompare.Value.AddYears(-5);
            }

            if (decCompare.Value.Year < 2008 || dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2011, 12, 31)) > 0)
                return null;

            if (decCompare.Value.CompareTo(new DateTime(2012, 01, 31)) > 0 || dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2011, 01, 01)) >= 0)
            {
                if (dataPerfReq.HasValue && dataPerfReq.Value.CompareTo(new DateTime(2011, 01, 01)) < 0)
                {
                    decCompare = CalcolaFinestre247(decCompare, attivitaEconomica, professioneIndividuale, dataNascitaTitolare, categoria);
                    return decCompare;
                }

                if (dataPerfReq.HasValue && dataPerfReq.Value.CompareTo(new DateTime(2011, 01, 01)) >= 0 && dataPerfReq.Value.CompareTo(new DateTime(2011, 12, 31)) <= 0)
                {
                    if (codiceParticolareSoggettoDerogato.GetValueOrDefault() == '3')
                        return decCompare;

                    if (categoriaNum <= 6)
                    {
                        decCompare = dataPerfReq.Value.AddYears(1).AddMonths(1);
                        if (sessoTitolare == 'M')
                        {
                            dataPerfReqCompare = dataNascitaTitolare.Value.AddYears(66).AddMonths(1);

                            if (dataPerfReqCompare.Value.CompareTo(new DateTime(2012, 01, 01)) < 0)
                                dataPerfReqCompare = new DateTime(2012, 01, 01);

                            if (decCompare.Value.CompareTo(dataPerfReqCompare) > 0)
                                decCompare = dataPerfReqCompare;
                        }
                        if (sessoTitolare == 'F')
                        {
                            dataPerfReqCompare = dataNascitaTitolare.Value.AddYears(62).AddMonths(1);

                            if (dataPerfReqCompare.Value.CompareTo(new DateTime(2012, 01, 01)) < 0)
                                dataPerfReqCompare = new DateTime(2012, 01, 01);

                            if (decCompare.Value.CompareTo(dataPerfReqCompare) > 0)
                                decCompare = dataPerfReqCompare;
                        }
                    }
                    else
                    {
                        decCompare = dataPerfReq.Value.AddYears(1).AddMonths(7);
                        if (sessoTitolare == 'M')
                        {
                            dataPerfReqCompare = dataNascitaTitolare.Value.AddYears(66).AddMonths(1);

                            if (dataPerfReqCompare.Value.CompareTo(new DateTime(2012, 01, 01)) < 0)
                                dataPerfReqCompare = new DateTime(2012, 01, 01);

                            if (decCompare.Value.CompareTo(dataPerfReqCompare) > 0)
                                decCompare = dataPerfReqCompare;
                        }
                        if (sessoTitolare == 'F')
                        {
                            dataPerfReqCompare = dataNascitaTitolare.Value.AddYears(63).AddMonths(7);

                            if (dataPerfReqCompare.Value.CompareTo(new DateTime(2012, 01, 01)) < 0)
                                dataPerfReqCompare = new DateTime(2012, 01, 01);

                            if (decCompare.Value.CompareTo(dataPerfReqCompare) > 0)
                                decCompare = dataPerfReqCompare;
                        }
                    }
                    return decCompare;
                }
            }

            if (dataPerfReq.HasValue)
            {
                if (decCompare.Value.CompareTo(new DateTime(2011, 01, 01)) >= 0 && decCompare.Value.CompareTo(new DateTime(2012, 02, 01)) < 0)
                {
                    if (dataPerfReq.Value.CompareTo(new DateTime(2011, 01, 01)) < 0)
                    {
                        decCompare = CalcolaFinestre247(decCompare, attivitaEconomica, professioneIndividuale, dataNascitaTitolare, categoria);
                        return decCompare;
                    }

                    if (categoriaNum <= 6)
                        decCompare = dataPerfReq.Value.AddYears(1).AddMonths(1);
                    else
                        decCompare = dataPerfReq.Value.AddYears(1).AddMonths(7);

                    return decCompare;
                }
            }

            decCompare = CalcolaFinestre247(decCompare, attivitaEconomica, professioneIndividuale, dataNascitaTitolare, categoria);

            return decCompare;
        }

        /// <summary>
        /// CALCOLO DECORRENZE (LEGGE 214 DEL 22.12.11). Rif. PCIPL35 - CALCOLA-DECO-214
        /// </summary>
        /// <returns></returns>
        public static DateTime? CalcolaDecorrenza214(DateTime? decCompare, DateTime? dataNascitaTitolare, char? sessoTitolare, byte? codiceCieco, string categoria, byte? Legge44997, DateTime? dataPerfReq)
        {
            string categoriaNumerica = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);
            int categoriaNum = 0;
            int.TryParse(categoriaNumerica, out categoriaNum);

            decCompare = dataNascitaTitolare.Value.AddDays(-dataNascitaTitolare.Value.Day + 1);
            if (sessoTitolare == 'F')
                decCompare = decCompare.Value.AddYears(55);
            else
                decCompare = decCompare.Value.AddYears(60);

            if (codiceCieco.GetValueOrDefault() == 1)
                decCompare = decCompare.Value.AddYears(-5);

            if (int.Parse(categoriaNumerica) > 6)
                decCompare = decCompare.Value.AddYears(5);

            if (int.Parse(categoriaNumerica) <= 6 && Legge44997.HasValue && Legge44997.Value != 6)
                decCompare = decCompare.Value.AddYears(5);

            if (Legge44997.HasValue && Legge44997.Value != 6)
            {
                if (dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2012, 01, 01)) >= 0)
                {
                    if (sessoTitolare == 'F')
                    {
                        if (categoriaNum <= 6)
                            decCompare = decCompare.Value.AddYears(2);
                        else
                            decCompare = decCompare.Value.AddYears(3).AddMonths(6);
                    }
                    else
                        decCompare = decCompare.Value.AddYears(1);
                }

                if (dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2013, 01, 01)) >= 0)
                {
                    decCompare = decCompare.Value.AddMonths(3);
                }

                if (dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2014, 01, 01)) >= 0)
                {
                    if (sessoTitolare == 'F')
                    {
                        if (categoriaNum <= 6)
                            decCompare = decCompare.Value.AddYears(1).AddMonths(6);
                        else
                            decCompare = decCompare.Value.AddYears(1);
                    }
                    else
                        decCompare = decCompare.Value.AddYears(1);
                }
            }

            return decCompare;
        }

        /// <summary>
        /// Calcola la decorrenza per le Finestre 247. Rif. PCIPL35 - FINESTRE-247
        /// </summary>
        /// <param name="decCompare"></param>
        /// <param name="attivitaEconomica"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="categoria"></param>
        /// <returns></returns>
        public static DateTime? CalcolaFinestre247(DateTime? decCompare, int? attivitaEconomica, int? professioneIndividuale, DateTime? dataNascitaTitolare, string categoria)
        {
            if ((attivitaEconomica.GetValueOrDefault() == 99 && professioneIndividuale.GetValueOrDefault() == 247) ||
                (attivitaEconomica.GetValueOrDefault() == 65 && professioneIndividuale.GetValueOrDefault() == 999) ||
                (attivitaEconomica.GetValueOrDefault() == 52 && professioneIndividuale.GetValueOrDefault() == 888))
                return decCompare;

            if (dataNascitaTitolare.Value.Month == 1 || dataNascitaTitolare.Value.Month == 2 || dataNascitaTitolare.Value.Month == 3)
            {
                if (categoria.Trim().Equals("VOS"))
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 7);
                else
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 10);
            }
            if (dataNascitaTitolare.Value.Month == 4 || dataNascitaTitolare.Value.Month == 5 || dataNascitaTitolare.Value.Month == 6)
            {
                if (categoria.Trim().Equals("VOS"))
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 10);
                else
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 1).AddYears(1);
            }
            if (dataNascitaTitolare.Value.Month == 7 || dataNascitaTitolare.Value.Month == 8 || dataNascitaTitolare.Value.Month == 9)
            {
                if (categoria.Trim().Equals("VOS"))
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 1).AddYears(1);
                else
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 4).AddYears(1);
            }
            if (dataNascitaTitolare.Value.Month == 10 || dataNascitaTitolare.Value.Month == 11 || dataNascitaTitolare.Value.Month == 12)
            {
                if (categoria.Trim().Equals("VOS"))
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 4).AddYears(1);
                else
                    decCompare = decCompare.Value.AddMonths(-decCompare.Value.Month + 7).AddYears(1);
            }

            return decCompare;
        }

        /// <summary>
        /// Calcola la data in cui il titolare ha un'età pensionabile
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="categoria"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="codiceCieco"></param>
        /// <returns></returns>
        public static DateTime? CalcolaDataEtaPensionabile(string gruppo, string categoria, char? sessoTitolare, DateTime? dataNascitaTitolare, byte? codiceCieco, int codiceStato, byte? codiceConvenzione)
        {
            DateTime? dataCompare = null;
            string categoriaNumerica = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);

            if (gruppo == "0001")
            {
                if (int.Parse(categoriaNumerica) > 6)
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(60);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(65);
                }
                else
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(55);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(60);
                }
                if (codiceStato == 58 && codiceConvenzione.GetValueOrDefault() == 58)
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(62);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(67);
                }
                if (codiceCieco.GetValueOrDefault() == 1)
                    dataCompare = dataCompare.Value.AddYears(-5);

                dataCompare = dataCompare.Value.AddMonths(dataNascitaTitolare.Value.Month + 1);
            }

            return dataCompare;
        }

        //ENG - Per il controllo 'Incompatibilità tra Natura Pensione ed età pensionabile' bisogna aggiungere un mese e settare il primo giorno del mese alla data ricavata
        public static DateTime? CalcolaDecEtaPensionabile(string gruppo, string categoria, char? sessoTitolare, DateTime? dataNascitaTitolare, byte? codiceCieco, int codiceStato, byte? codiceConvenzione)
        {
            DateTime dataCompare = new DateTime();
            string categoriaNumerica = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);

            if (gruppo == "0001")
            {
                if (int.Parse(categoriaNumerica) > 6)
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(60);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(65);
                }
                else
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(55);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(60);
                }
                if (codiceStato == 58 && codiceConvenzione.GetValueOrDefault() == 58)
                {
                    if (sessoTitolare == 'F')
                        dataCompare = dataNascitaTitolare.Value.AddYears(62);
                    else
                        dataCompare = dataNascitaTitolare.Value.AddYears(67);
                }
                if (codiceCieco.GetValueOrDefault() == 1)
                    dataCompare = dataCompare.AddYears(-5);

                dataCompare = Utility.FirstDayOfMonth(dataCompare).AddMonths(1);
            }

            return dataCompare;
        }

        /// <summary>
        /// Se la causa carico è diversa da 2 il campo attività economica è obbligatorio
        /// </summary>
        /// <returns></returns>
        public static bool VerificaObbligatorietaAttivitaEconomicaWithCausaCarico(byte? causaCarico, int? attivitaEconomica)
        {
            if (!attivitaEconomica.HasValue && causaCarico.Value != 2)
                return false;
            return true;
        }

        /// <summary>
        /// Se la causa carico è diversa da 2 il campo professione individuale è obbligatorio
        /// </summary>
        /// <returns></returns>
        public static bool VerificaObbligatorietaProfessioneIndividualeWithCausaCarico(byte? causaCarico, int? professioneIndividuale)
        {
            if (!professioneIndividuale.HasValue && causaCarico.Value != 2)
                return false;
            return true;
        }

        public static bool ControlsNaturaPensioneWithEtaPensionabile(string gruppo, string categoria, char? sessoTitolare, DateTime? dataNascitaTitolare, byte? codiceCieco, string codNatura, DateTime? decorrenzaOriginaria, int codiceStato, byte? codiceConvenzione, out string messaggioVideo)
        {
            DateTime? dataCompare = new DateTime(01, 01, 01);
            messaggioVideo = string.Empty;

            if (gruppo == "0001")
            {
                if (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2")
                {
                    dataCompare = CalcolaDataEtaPensionabile(gruppo, categoria, sessoTitolare, dataNascitaTitolare, codiceCieco, codiceStato, codiceConvenzione);
                    if (dataCompare.Value.CompareTo(decorrenzaOriginaria) < 0 && decorrenzaOriginaria.Value.CompareTo(new DateTime(1994, 01, 01)) < 0)
                    {
                        messaggioVideo = "Se Natura Pensione = 1/2, non deve aver compiuto l'età pensionabile";
                        return false;
                    }
                }
                else
                {
                    dataCompare = CalcolaDecEtaPensionabile(gruppo, categoria, sessoTitolare, dataNascitaTitolare, codiceCieco, codiceStato, codiceConvenzione);
                    if (dataCompare.Value.CompareTo(decorrenzaOriginaria) > 0)
                    {
                        if (!((decorrenzaOriginaria.Value.CompareTo(new DateTime(2003, 01, 01)) > 0 && decorrenzaOriginaria.Value.CompareTo(new DateTime(2004, 03, 01)) < 0 &&
                            codNatura.Substring(2, 1) == "L") || decorrenzaOriginaria.Value.CompareTo(new DateTime(2001, 01, 01)) >= 0))
                        {
                            messaggioVideo = "Incompatibilità tra Natura Pensione ed età pensionabile";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se gli anni differimento sono valorizzati nel caso in cui la sigla categoria inizia per "I"
        /// </summary>
        /// <param name="anniDifferimento"></param>
        /// <param name="gruppo"></param>
        /// <returns></returns>
        public static bool VerificaAnniDifferimento(int? anniDifferimento, string gruppo)
        {
            if (anniDifferimento.HasValue && gruppo == "0002")
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se gli anni differimento sono valorizzati nel caso in cui la categoria è "VOS" e la decorrenza pensione è successiva al 07/1976
        /// </summary>
        /// <param name="anniDifferimento"></param>
        /// <param name="categoria"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns></returns>
        public static bool VerificaAnniDifferimentoWithVOS(int? anniDifferimento, string categoria, DateTime? decorrenzaOriginaria)
        {
            if (anniDifferimento.HasValue && categoria.Trim() == "VOS" && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1976, 07, 01)))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se gli anni differimento sono compatibili con l'età pensionabile
        /// </summary>
        /// <param name="anniDifferimento"></param>
        /// <param name="gruppo"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="categoria"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="codiceCieco"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsAnniDifferimentoWithEtaPensionabile(int? anniDifferimento, string gruppo, DateTime? decorrenzaOriginaria, string categoria, char? sessoTitolare, DateTime? dataNascitaTitolare, byte? codiceCieco, int codiceStato, byte? codiceConvenzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int anniCompare = 0;

            DateTime? dataCompare = CalcolaDataEtaPensionabile(gruppo, categoria, sessoTitolare, dataNascitaTitolare, codiceCieco, codiceStato, codiceConvenzione);

            if (dataCompare.HasValue)
            {
                if (anniDifferimento.HasValue)
                {
                    if (gruppo == "0001")
                    {
                        anniCompare = decorrenzaOriginaria.Value.Year - dataCompare.Value.Year;
                        if (decorrenzaOriginaria.Value.Month - dataCompare.Value.Month < 0)
                            anniCompare -= 1;

                        if (anniCompare < anniDifferimento)
                        {
                            messaggioVideo = "Anni di differimento superiori alla capienza";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se la trattenuta INPDAP è 'NO', la decorrenza trattenuta INPDAP è 10/2007 e la causa carico è 2
        /// </summary>
        /// <param name="trattenutaInpdap"></param>
        /// <param name="decorrenzaTrattenutaInpdap"></param>
        /// <param name="causaCarico"></param>
        /// <returns>False se la trattenuta INPDAP è 'NO', la decorrenza trattenuta INPDAP è 10/2007 e la causa carico è 2</returns>
        public static bool VerificaTrattenutaINPDAPWithCausaCarico(bool? trattenutaInpdap, DateTime? decorrenzaTrattenutaInpdap, byte? causaCarico, GestionePensione.DatiPensione datiPensione)
        {
            if (!trattenutaInpdap.HasValue || trattenutaInpdap.Value || (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda, new DateTime(2022, 02, 20))))
                return true;

            if (!(!trattenutaInpdap.Value && decorrenzaTrattenutaInpdap.HasValue && decorrenzaTrattenutaInpdap.Value.Equals(new DateTime(2007, 10, 01)) && causaCarico.Value == 2))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se è presente la decorrenza trattenuta INPDAP ed è assente la trattenuta inpdap
        /// </summary>
        /// <param name="trattenutaInpdap"></param>
        /// <param name="decorrenzaTrattenutaInpdap"></param>
        /// <returns>False se è presente la decorrenza trattenuta INPDAP ed è assente la trattenuta inpdap</returns>
        public static bool VerificaCoerenzaTrattenutaINPDAP(bool? trattenutaInpdap, DateTime? decorrenzaTrattenutaInpdap)
        {
            if (!trattenutaInpdap.HasValue && decorrenzaTrattenutaInpdap.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica la compatibilità tra la categoria pensione e la trattenuta INPDAP
        /// </summary>
        /// <param name="trattenutaInpdap"></param>
        /// <param name="gruppo"></param>
        /// <returns>False se la trattenuta INPDAP è SI e il gruppo è 0002 o 0003</returns>
        public static bool VerificaTrattenutaINPDAPWithCategoria(bool? trattenutaInpdap, string gruppo, GestionePensione.DatiPensione datiPensione)
        {
            if (trattenutaInpdap.HasValue && trattenutaInpdap.Value && (gruppo == "0002" || Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica la compatibilità della decorrenza Trattenuta INPDAP con la decorrenza pensione
        /// </summary>
        /// <param name="trattenutaInpdap"></param>
        /// <param name="decorrenaTrattenutaInpdap"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns>False se la decorrenza pensione è antecedente il 12/2007 e la decorrenza trattenuta è diversa da 11/2007 o 06/2008</returns>
        public static bool VerificaTrattenutaINPDAPWithDecorrenzaPensione(bool? trattenutaInpdap, DateTime? decorrenzaTrattenutaInpdap, DateTime? decorrenzaOriginaria, GestionePensione.DatiPensione datiPensione, DateTime? dataRinunciaTrattenutaInpdapStorico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo86", out ctrl);

            if (!(ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsRicostituzione(datiPensione.Gruppo) && decorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2022, 03, 01))))
            {
                if (!Utility.IsRicostituzione(datiPensione.Gruppo))
                {
                    if (trattenutaInpdap.HasValue && trattenutaInpdap.Value && decorrenzaTrattenutaInpdap.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2007, 12, 01)))
                        {
                            if (!decorrenzaTrattenutaInpdap.Value.Equals(new DateTime(2007, 11, 01)) && !decorrenzaTrattenutaInpdap.Value.Equals(new DateTime(2008, 06, 01)))
                            {
                                messaggioVideo = "Decorrenza Trattenuta Fondo Credito errata";
                                return false;
                            }
                        }

                        if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2007, 11, 01)) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2008, 07, 01)))
                        {
                            if (!decorrenzaTrattenutaInpdap.Value.Equals(new DateTime(2008, 06, 01)))
                            {
                                messaggioVideo = "Decorrenza Trattenuta Fondo Credito errata";
                                return false;
                            }
                        }

                        //ENG - TRF con Data Trattenuta valorizzata dal prelievo e decorrenza minore di 03/2022
                        if (Utility.IsRiaperturaDomanda(datiPensione.Id) && dataRinunciaTrattenutaInpdapStorico.HasValue && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2022, 03, 01)))
                        {
                            if (!Utility.DataSuccessivaA(decorrenzaTrattenutaInpdap.Value, decorrenzaOriginaria.Value))
                            {
                                messaggioVideo = "La decorrenza trattenuta Fondo Credito non deve essere minore della decorrenza originaria";
                                return false;
                            }
                        }
                        else
                        {
                            if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2008, 06, 01)))
                            {
                                if (!decorrenzaTrattenutaInpdap.Value.Equals(decorrenzaOriginaria))
                                {
                                    messaggioVideo = "Decorrenza Trattenuta Fondo Credito errata";
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (trattenutaInpdap.HasValue && trattenutaInpdap.Value && decorrenzaTrattenutaInpdap.HasValue)
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaTrattenutaInpdap.Value, decorrenzaOriginaria.Value))
                        {
                            messaggioVideo = "Non è consentito l’inserimento di una decorrenza trattenuta Fondo Credito precedente alla decorrenza della pensione";
                            return false;
                        }

                        if (Utility.DataSuccessivaA(decorrenzaTrattenutaInpdap.Value, new DateTime(2022, 03, 01)))
                        {
                            messaggioVideo = "Non è consentito l'inserimento di una decorrenza trattenuta maggiore di febbraio 2022";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Per le Ricostituzioni con Decorrenza Originaria >= 03/2022, la data trattenuta INPDAP deve essere uguale alla decorrenza originaria
        /// </summary>
        /// <param name="trattenutaInpdap"></param>
        /// <param name="decorrenaTrattenutaInpdap"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="dataPresentazioneDomanda"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaTrattenutaINPDAP(GestionePensione.DatiPensione datiPensione, bool? trattenutaInpdap, DateTime? decorrenzaTrattenutaInpdap, DateTime? dataRinunciaTrattenutaInpdapStorico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo86", out ctrl);

            if (ctrl != null && ctrl.ValoreControllo == "SI" && Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2022, 03, 01)))
            {
                if (trattenutaInpdap.HasValue && trattenutaInpdap.Value && decorrenzaTrattenutaInpdap.HasValue && !dataRinunciaTrattenutaInpdapStorico.HasValue)
                {
                    //Aggiornamento Memo86: Per le RIC con decorrenza originaria >= 03/2022, la data trattenuta INPDAP deve essere uguale alla decorrenza originaria
                    if (!Utility.DataSuccessivaA(decorrenzaTrattenutaInpdap.Value, datiPensione.DecorrenzaOriginaria.Value) || Utility.DataStrettamenteSuccessivaA(decorrenzaTrattenutaInpdap.Value, datiPensione.DecorrenzaOriginaria.Value))
                    {
                        messaggioVideo = "La decorrenza trattenuta Fondo Credito deve essere pari alla decorrenza originaria";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificaPresenzaTrattenutaINPDAP(bool? trattenutaInpdap, DateTime? decorrenzaTrattenutaInpdap, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((trattenutaInpdap.HasValue && trattenutaInpdap.Value && !decorrenzaTrattenutaInpdap.HasValue) ||
              (decorrenzaTrattenutaInpdap.HasValue && (!trattenutaInpdap.HasValue || !trattenutaInpdap.Value)))
            {
                messaggioVideo = "La decorrenza trattenuta Fondo Credito è necessaria in presenza del codice trattenuta Fondo Credito pari a SI";
                return false;
            }

            return true;
        }

        public static bool ControlsRequisitoRidotto(DateTime? decorrenzaOriginaria, string gruppo, string codNatura, byte? legge44997, string categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!gruppo.Equals("0003"))
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1998, 03, 01)) && !string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
                {
                    if (legge44997.GetValueOrDefault() > 6)
                    {
                        messaggioVideo = "Codice Requisito Ridotto errato (1-2-3-4-5-6)";
                        return false;
                    }
                }
                else
                {
                    if (legge44997.HasValue)
                    {
                        if (legge44997.Value != 6)
                        {
                            messaggioVideo = "Codice Requisito Ridotto incompatibile con la Decorrenza Originaria / Natura Pensione";
                            return false;
                        }
                        else
                        {
                            if (!categoria.Trim().Equals("VOS") || !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01)))
                            {
                                messaggioVideo = "Codice Requisito Ridotto incompatibile con la Decorrenza Originaria / Categoria";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static bool ControlsCodiceContrattoEquiparato(DateTime? decorrenza, string gruppo, string codNaturaTitolare, short? codiceContrattoEquiparato, string categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1998, 03, 01)) && !string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2")))
            {
                if (codiceContrattoEquiparato.HasValue)
                {
                    messaggioVideo = "Codice Contratto temporaneamente sospeso";
                    return false;
                }
            }
            else
            {
                if (codiceContrattoEquiparato.HasValue)
                {
                    messaggioVideo = "Codice Contratto incompatibile con Decorrenza Originaria / Natura Pensione";
                    return false;
                }
            }


            return true;
        }

        public static bool ControlsCodiceLivelloEquiparato(DateTime? decorrenza, string gruppo, string codNaturaTitolare, short? codiceLivelloEquiparato, string categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1998, 03, 01)) && !string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2")))
            {
                if (codiceLivelloEquiparato.HasValue)
                {
                    messaggioVideo = "Codice Livello temporaneamente sospeso";
                    return false;
                }
            }
            else
            {
                if (codiceLivelloEquiparato.HasValue)
                {
                    messaggioVideo = "Codice Livello incompatibile con Decorrenza Originaria / Natura Pensione";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsCodiceMobilita(DateTime? decorrenzaOriginaria, string gruppo, string codNaturaTitolare, byte? codiceMobilita, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!gruppo.Equals("0003"))
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1998, 03, 01)) && (!string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2"))))
                {
                    if (codiceMobilita.GetValueOrDefault() > 9)
                    {
                        messaggioVideo = "Codice Mobilità errato";
                        return false;
                    }
                }
                else
                {
                    if (codiceMobilita.HasValue)
                    {
                        messaggioVideo = "Codice Mobilità incompatibile con Decorrenza Originaria / Natura Pensione";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica che se la decorrenza pensione è posteriore al 03/1998 e il gruppo non è 0003 e il codice natura è "1**" o "2**" allora non devono essere presenti insieme il codice
        /// mobilità e il requisito ridotto
        /// </summary>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="gruppo"></param>
        /// <param name="codNatura"></param>
        /// <param name="codiceMobilita"></param>
        /// <param name="categoria"></param>
        /// <param name="legge44997"></param>
        /// <returns>False se il codice mobilità è uguale al requisito ridotto</returns>
        public static bool VerificaCodiceMobilitaWithRequisitoRidotto(DateTime? decorrenza, string gruppo, string codNatura, byte? codiceMobilita, string categoria, byte? legge44997)
        {
            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1998, 03, 31)) && !string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
                if (codiceMobilita.HasValue && legge44997.HasValue)
                    return false;

            return true;
        }

        /// <summary>
        /// Verifica se l'esenzione fiscale vittime terrorismo è diverso da NO
        /// </summary>
        /// <param name="comunicazioneCampo4"></param>
        /// <param name="detrazioniReddito"></param>
        /// <returns>False se è diverso da NO</returns>
        public static bool VerificaEsenzioneFiscaleTerrorismo(byte? comunicazioneCampo4, byte? detrazioniReddito)
        {
            if (comunicazioneCampo4.HasValue && comunicazioneCampo4.Value == 1)
                return false;

            if (detrazioniReddito.HasValue && detrazioniReddito.Value == 3)
                return false;

            return true;
        }

        public static bool ControlsEsenzioneFiscaleEstero(byte? comunicazioneCampo4, byte? detrazioniReddito, string provinciaResidenza, string codiceComuneResidenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool controlloRevocaEsenzioneEsteraAlRientroInItalia = !comunicazioneCampo4.HasValue && !(string.IsNullOrEmpty(codiceComuneResidenza) || codiceComuneResidenza.StartsWith("Z"));
            if ((comunicazioneCampo4.HasValue && comunicazioneCampo4.Value == 2) || (detrazioniReddito.HasValue && detrazioniReddito.Value == 2))
            {

                if ((string.IsNullOrEmpty(codiceComuneResidenza) || !codiceComuneResidenza.StartsWith("Z")) && !controlloRevocaEsenzioneEsteraAlRientroInItalia)
                {
                    messaggioVideo = "Esenzione fiscale 'Detass. Estera': soggetto residente in Italia";
                    if (controlloRevocaEsenzioneEsteraAlRientroInItalia)
                        messaggioVideo += "<br />Per i soggetti rientrati in Italia è necessario aggiornare le detrazioni.<br />" +
                                            "Accedere al quadro Detrazioni e collegarsi a Unidetra tramite il pulsante \"Acquisisci\".<br />" + 
                                            "Apportare le modifiche necessarie e salvare.<br />" +
                                            "Infine, prima di tornare al quadro Liquidazione Pensione, rientrare nel quadro Detrazioni, aggiornare e salvare le nuove detrazioni.";
                    return false;
                }

                if (provinciaResidenza.Trim().ToUpperInvariant() == "F" || provinciaResidenza.Trim().ToUpperInvariant() == "AND" || provinciaResidenza.Trim().ToUpperInvariant() == "AN" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "AG" || provinciaResidenza.Trim().ToUpperInvariant() == "MQ" || provinciaResidenza.Trim().ToUpperInvariant() == "AI" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "NA" || provinciaResidenza.Trim().ToUpperInvariant() == "BS" || provinciaResidenza.Trim().ToUpperInvariant() == "BER" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "BOL" || provinciaResidenza.Trim().ToUpperInvariant() == "BF" || provinciaResidenza.Trim().ToUpperInvariant() == "CAM" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "CV" || provinciaResidenza.Trim().ToUpperInvariant() == "RCH" || provinciaResidenza.Trim().ToUpperInvariant() == "CO" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "CR" || provinciaResidenza.Trim().ToUpperInvariant() == "C" || provinciaResidenza.Trim().ToUpperInvariant() == "ES" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "ER" || provinciaResidenza.Trim().ToUpperInvariant() == "FJN" || provinciaResidenza.Trim().ToUpperInvariant() == "G" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "WAG" || provinciaResidenza.Trim().ToUpperInvariant() == "GCA" || provinciaResidenza.Trim().ToUpperInvariant() == "HN" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "HK" || provinciaResidenza.Trim().ToUpperInvariant() == "EAK" || provinciaResidenza.Trim().ToUpperInvariant() == "RL" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "MW" || provinciaResidenza.Trim().ToUpperInvariant() == "RMM" || provinciaResidenza.Trim().ToUpperInvariant() == "MD" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "NAM" || provinciaResidenza.Trim().ToUpperInvariant() == "NIC" || provinciaResidenza.Trim().ToUpperInvariant() == "LAR" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "FL" || provinciaResidenza.Trim().ToUpperInvariant() == "MO" || provinciaResidenza.Trim().ToUpperInvariant() == "RM" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "MW" || provinciaResidenza.Trim().ToUpperInvariant() == "RMM" || provinciaResidenza.Trim().ToUpperInvariant() == "NAM" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "NIC" || provinciaResidenza.Trim().ToUpperInvariant() == "RN" || provinciaResidenza.Trim().ToUpperInvariant() == "NGR" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "NC" || provinciaResidenza.Trim().ToUpperInvariant() == "PA" || provinciaResidenza.Trim().ToUpperInvariant() == "PNG" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "PY" || provinciaResidenza.Trim().ToUpperInvariant() == "PE" || provinciaResidenza.Trim().ToUpperInvariant() == "PYF" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "PR" || provinciaResidenza.Trim().ToUpperInvariant() == "MC" || provinciaResidenza.Trim().ToUpperInvariant() == "DOM" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "REU" || provinciaResidenza.Trim().ToUpperInvariant() == "SY" || provinciaResidenza.Trim().ToUpperInvariant() == "WAL" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "SD" || provinciaResidenza.Trim().ToUpperInvariant() == "RC" || provinciaResidenza.Trim().ToUpperInvariant() == "ROU" ||
                    provinciaResidenza.Trim().ToUpperInvariant() == "ZW")
                {
                    messaggioVideo = "Esenzione fiscale 'Detass. Estera': stato di residenza incompatibile";
                    if (controlloRevocaEsenzioneEsteraAlRientroInItalia)
                        messaggioVideo += "<br />Per i soggetti rientrati in Italia è necessario aggiornare le detrazioni.<br />" +
                                        "Accedere al quadro Detrazioni e collegarsi a Unidetra tramite il pulsante \"Acquisisci\".<br />" +
                                        "Apportare le modifiche necessarie e salvare.<br />" +
                                        "Infine, prima di tornare al quadro Liquidazione Pensione, rientrare nel quadro Detrazioni, aggiornare e salvare le nuove detrazioni.";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsCodNaturaCrossTab(string codNatura, string gruppo, int? attivitaEconomica, int? professioneIndividuale, DateTime? decorrenzaOriginaria, byte? causaCarico, string codiceTipoRichiesta, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            string filtro = Utility.GetFiltroByCodTipoRichiesta(codiceTipoRichiesta);

            if (codNatura != null && codNatura.Substring(2, 1) == "L")
            {
                if (gruppo != "0001" || (attivitaEconomica.HasValue && attivitaEconomica.Value != 98) || (professioneIndividuale.HasValue && professioneIndividuale.Value != 176))
                {
                    messaggioVideo = "Incompatibilità tra Natura 'L' - Categoria - Attività Economica";
                    return false;
                }

                if ((Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1998, 07, 01)) && !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2001, 05, 01))) ||
                    (Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 02, 01)) && !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 02, 01))))
                {
                    if (causaCarico.GetValueOrDefault() == 1 && (Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 02, 01)) && !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 02, 01))) &&
                        filtro != "LS1" && filtro != "LS2")
                    {
                        messaggioVideo = "Pensione LSU (Natura = 'L') incompatibile con campo 36 dell'EAD75";
                        return false;
                    }

                    if (filtro == "LS1" && (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2"))
                    {
                        messaggioVideo = "Anzianita' LSU (Natura 1/2) incompatibile con campo 36 dell'EAD75";
                        return false;
                    }

                    if (filtro == "LS2" && (codNatura.Substring(0, 1) == "0" || codNatura.Substring(0, 1) == "6"))
                    {
                        messaggioVideo = "Vecchiaia LSU (Natura 0/6) incompatibile con campo 36 dell'EAD75";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Incompatibilita' tra Natura 'L' e Decorrenza Originaria";
                    return false;
                }
            }

            if (filtro == "LS1" || filtro == "LS2" && codNatura.Substring(2, 1) != "L")
            {
                messaggioVideo = "Campo 36 dell'EAD75 (LSU) incompatibile con pensione (non LSU)";
                return false;
            }

            if (filtro == "TRA" && codNatura.Substring(2, 1) != "H")
            {
                messaggioVideo = "Incompatibilita' tra campo 36/EAD75 (TRA) e Natura Pensione (3'cod.='H')";
                return false;
            }

            return true;
        }

        public static bool ControlsCodiceOpzioneRiliquidazione(byte? codiceOpzioneRiliquidazione, string cittadinanza, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, string gruppo, string codNatura, byte? legge44997, DateTime? decorrenzaOriginaria, DateTime? datNascitaTitolare, char? sessoTitolare, string categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceOpzioneRiliquidazione.GetValueOrDefault() != 0 && codiceOpzioneRiliquidazione.GetValueOrDefault() != 6 && codiceOpzioneRiliquidazione.GetValueOrDefault() != 7)
            {
                messaggioVideo = "Se presente, Codice Opzione deve essere  '7'";
                return false;
            }

            if (codiceOpzioneRiliquidazione.GetValueOrDefault() == 7)
            {
                if (string.IsNullOrEmpty(cittadinanza) || cittadinanza != "Z000") // Se non è ITALIA
                {
                    messaggioVideo = "Codice Opzione '7' incompatibile con Cittadinanza";
                    return false;
                }

                if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
                {
                    if (listaResidenzeEstere.First().CodCatastaleStatoEE != "Z000" && (listaResidenzeEstere.Count < 2 || !listaResidenzeEstere.ElementAt(1).Decorrenza.HasValue))
                    {
                        messaggioVideo = "Codice Opzione '7' incompatibile con Residenza Alla Decorrenza Originaria";
                        return false;
                    }
                }

                if (listaPrestazioniEstere.First().CodiceConvenzione != 12 || int.Parse(listaPrestazioniEstere.First().CodiceStatoEE) != 17 || listaPrestazioniEstere.Count > 1)
                {
                    messaggioVideo = "Codice Opzione '7' incompatibile con Convenzione / Stato";
                    return false;
                }

                if (gruppo != "0001")
                {
                    messaggioVideo = "Codice Opzione '7' incompatibile con Categoria di Pensione";
                    return false;
                }
                else
                {
                    if (!(legge44997.GetValueOrDefault() == 6 || (codNatura != string.Empty && (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2"))))
                    {
                        int mesi = decorrenzaOriginaria.Value.Year * 12 + decorrenzaOriginaria.Value.Month - datNascitaTitolare.Value.Year * 12 - datNascitaTitolare.Value.Month;
                        if (!(sessoTitolare.Value == 'F' && mesi > 720 && mesi < 756))
                        {
                            messaggioVideo = "Codice Opzione '7' incompatibile con Natura Pensione / Requisito Ridotto";
                            return false;
                        }
                    }
                }

                string categoriaNumerica = string.Empty;
                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);
                if (((codNatura != string.Empty && (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2")) && int.Parse(categoriaNumerica) < 7 && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 07, 01)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 04, 01))) ||
                    ((codNatura != string.Empty && (codNatura.Substring(0, 1) == "1" || codNatura.Substring(0, 1) == "2")) && int.Parse(categoriaNumerica) > 6 && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 07, 01)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 07, 01))) ||
                    (legge44997.GetValueOrDefault() == 6 && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 07, 01)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 12, 01))))
                {
                    messaggioVideo = "Codice Opzione '7' incompatibile con Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica che la decorrenza pensione sia compresa tra il 1930 e l'anno odierno
        /// </summary>
        /// <param name="decorrenzaOpzione"></param>
        /// <returns>False se non è compresa</returns>
        public static bool VerificaDecorrenzaOpzione(DateTime? decorrenzaOpzione)
        {
            if (decorrenzaOpzione.HasValue && (decorrenzaOpzione.Value.Year < 1930 || decorrenzaOpzione.Value.Year > Utility.DataSistemaCi.Year))
                return false;

            return true;
        }

        public static bool ControlsDecorrenzaOpzione(DateTime? decorrenzaOpzione, string gruppo, DateTime? decorrenzaOriginaria, DateTime? dataDomandaOpzione, byte? codiceConvenzione, string stato, string categoria, string codNatura, bool isRocOrRevCI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaOpzione.HasValue)
            {
                if (!gruppo.Equals("0003") && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, decorrenzaOriginaria.Value))
                {
                    messaggioVideo = "Decorrenza Opzione anteriore a Decorrenza Pensione";
                    return false;
                }

                if (dataDomandaOpzione.HasValue && (dataDomandaOpzione.Value.Year == 1994 || dataDomandaOpzione.Value.Year == 1995) && codiceConvenzione.GetValueOrDefault() != 17)
                {
                    if (!decorrenzaOpzione.Value.Equals(new DateTime(1994, 01, 01)))
                    {
                        if (codiceConvenzione.GetValueOrDefault() != 27 || !decorrenzaOpzione.Value.Equals(new DateTime(1995, 05, 01)))
                        {
                            messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda";
                            return false;
                        }
                    }
                }

                if (!isRocOrRevCI &&
                    dataDomandaOpzione.HasValue && dataDomandaOpzione.Value.Year > 1995 && codiceConvenzione != 17 && codiceConvenzione != 13 && codiceConvenzione != 38)
                {
                    if (!decorrenzaOpzione.Value.Equals(new DateTime(dataDomandaOpzione.Value.AddMonths(1).Year, dataDomandaOpzione.Value.AddMonths(1).Month, 01)) && !decorrenzaOpzione.Value.Equals(new DateTime(dataDomandaOpzione.Value.AddMonths(89).Year, dataDomandaOpzione.Value.AddMonths(89).Month, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda (Mese Success.)";
                        return false;
                    }
                }

                if (Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)))
                {
                    if (((codiceConvenzione == 13 && (!string.IsNullOrEmpty(stato) ? int.Parse(stato) : 0) == 38) || codiceConvenzione == 38) && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(2004, 05, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile: non puo' essere ante 5/2004";
                        return false;
                    }

                    if (codiceConvenzione == 17 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(2002, 06, 01)))
                    {
                        messaggioVideo = "Decorre Opzione incompatibile: non puo' essere ante 6/2002";
                        return false;
                    }

                    if ((codiceConvenzione == 9 || codiceConvenzione == 20) && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1994, 01, 01)))
                    {
                        messaggioVideo = "Decorre Opzione incompatibile: non puo' essere ante 1/1994";
                        return false;
                    }

                    if (codiceConvenzione == 27 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1995, 05, 01)))
                    {
                        messaggioVideo = "Decorre Opzione incompatibile non puo' essere ante 5/1995";
                        return false;
                    }
                }

                if (dataDomandaOpzione.HasValue && dataDomandaOpzione.Value.Year < 1977)
                {
                    if ((gruppo.Equals("0002") && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 07, 01))) || (gruppo.Equals("0001") && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1968, 05, 01))))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda";
                        return false;
                    }
                }

                if ((categoria.Trim().Equals("VOS") && (Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1965, 01, 01)) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)))) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")) && (codiceConvenzione != 9 && codiceConvenzione != 20 && codiceConvenzione != 29 && codiceConvenzione != 27 && codiceConvenzione != 17 && !Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1993, 12, 01))))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1968, 05, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                        return false;
                    }
                }

                if ((categoria.Trim().Equals("VOS") && (Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 04, 01)) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1969, 06, 01)))) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")) && (codiceConvenzione != 9 && codiceConvenzione != 20 && codiceConvenzione != 29 && codiceConvenzione != 27 && codiceConvenzione != 17 && !Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1993, 12, 01))))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1969, 05, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                        return false;
                    }
                }

                if (categoria.Trim().Equals("VOS") && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)) && (!codNatura.Substring(0, 1).Equals("1") || !codNatura.Substring(0, 1).Equals("2")))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1969, 05, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                        return false;
                    }
                }

                if (categoria.Trim().Equals("IOS") && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)) && (codiceConvenzione != 9 && codiceConvenzione != 20 && codiceConvenzione != 29 && codiceConvenzione != 27 && codiceConvenzione != 17 && !Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1993, 12, 01))))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 07, 01)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ControlsDecorrenzaDPCM(DateTime? decorrenzaArt2Dpcm, string categoria, DateTime? decorrenzaOriginaria, string gruppo, DateTime? decorrenzaOpzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;
            if (decorrenzaArt2Dpcm.HasValue)
            {
                if (decorrenzaArt2Dpcm.Value.Year < 1990 || decorrenzaArt2Dpcm.Value.Year > dataSistema.Year || (Utility.DataStrettamenteSuccessivaA(decorrenzaArt2Dpcm.Value, new DateTime(1990, 01, 01)) && !Utility.DataSuccessivaA(decorrenzaArt2Dpcm.Value, new DateTime(1990, 08, 01))))
                {
                    messaggioVideo = "Decorrenza D.P.C.M. illogica o errata";
                    return false;
                }

                string categoriaNumerica = string.Empty;
                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria, out categoriaNumerica);

                if (int.Parse(categoriaNumerica) > 6 && !Utility.DataSuccessivaA(decorrenzaArt2Dpcm.Value, decorrenzaOriginaria.Value))
                {
                    messaggioVideo = "Decorrenza D.P.C.M. anteriore Decorrenza Pensione";
                    return false;
                }

                if (!gruppo.Equals("0003"))
                {
                    if (((!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1971, 01, 01)) || !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1984, 12, 01))) && !decorrenzaOpzione.HasValue) ||
                        (decorrenzaOpzione.HasValue && (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1971, 01, 01)) || !Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1984, 12, 01)))))
                    {
                        messaggioVideo = "Decorr. D.P.C.M. incompatibile con Decorrenza Pensione";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 217.	Se il campo ICI2IMPCRIS34 (IMPORTO CRISTALLIZZAZIONE)  > 0 allora controlla il campo (IW1CARIC NOT =  9 AND 2) allora segnala errore "IMPORTO CRISTALL.34/81 
        /// INCOMPATIBILE CON CAUSA CARICO"      
        /// 218.	Se il campo IW1CATPEN NOT = 4 AND 5 AND 6  segnala  errore  "IMPORTO CRISTALL.34/81 INCOMPATIBILE CON  CATEGORIA DI PENSIONE"                                          
        /// 219.	Se i campi  IW1CATPEN  = 6 AND (ICODVIRT (codice virtuale) = "5"  OR = "6" ) allora segnala errore "IMPORTO CRISTALL.34/81 INCOMPATIBILE CON  COD.VIRTUALE 5/6"                         
        /// Se il campo IREQPARD = 6 segnala errore "IMPORTO CRISTALL.34/81 INCOMPATIBILE CON COD.REQ.PART.DIRITTO = 6"     
        /// 220.	Se il campo ICI2IMPCRIS34 > ( 2582284 / 10000) allora segnala errore "IMPORTO CRISTALL.34/81 SUPERIORE A 258,2284" 
        /// 221.	Se il campo IW1DEORIG minore 198101 allora controlla il campo ICI2IMPCRIS34 minore ( 355632 / 10000) allora segnala errore "IMPORTO CRISTALL.34/81 MINORE DI  
        /// 35,5332" altrimenti controlla il campo APPO-CAT1 = "S" AND IW1DEORIG minore 198104 allora controlla ICI2IMPCRIS34 minore ( 213297 / 10000)  allora segnala errore 
        /// "IMPORTO CRISTALL.34/81 MINORE DI 21.3297"     
        /// </summary>
        /// <param name="importoCristallizzazione"></param>
        /// <param name="causaCarico"></param>
        /// <param name="categoria"></param>
        /// <param name="codiceVirtuale"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="gruppo"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsImportoCristallizzazione(decimal? importoCristallizzazione, byte? causaCarico, string categoria, char? codiceVirtuale, DateTime? decorrenzaOriginaria, string gruppo, byte? codiceRequisitiParticolari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (importoCristallizzazione.GetValueOrDefault() > 0)
            {
                if (causaCarico.GetValueOrDefault() != 2 && causaCarico.GetValueOrDefault() != 9)
                {
                    messaggioVideo = "Importo Cristallizzazione 34/81 incompatibile con Causa Carico";
                    return false;
                }

                if (!categoria.Trim().Equals("VOS") && !categoria.Trim().Equals("IOS") && !categoria.Trim().Equals("SOS"))
                {
                    messaggioVideo = "Importo Cristallizzazione 34/81 incompatibile con Categoria Pensione";
                    return false;
                }

                if (categoria.Trim().Equals("SOS") && (codiceVirtuale.GetValueOrDefault() == '5' || codiceVirtuale.GetValueOrDefault() == '6'))
                {
                    messaggioVideo = "Importo Cristallizzazione 34/81 incompatibile con Codice Virtuale 5/6";
                    return false;
                }

                if (codiceRequisitiParticolari.GetValueOrDefault() == 6)
                {
                    messaggioVideo = "Importo Cristallizzazione 34/81 incompatibile con Codice Requisito Particolare Diritto = 6";
                    return false;
                }

                if (importoCristallizzazione.Value > 258.2284M)
                {
                    messaggioVideo = "Importo Cristallizzazione 34/81 superiore a 258,2284";
                    return false;
                }

                if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1981, 01, 01)))
                {
                    if (importoCristallizzazione.Value > 35.5332M)
                    {
                        messaggioVideo = "Importo Cristallizzazione 34/81 superiore a 35,5332";
                        return false;
                    }
                }
                else
                {
                    if (gruppo.Equals("0003") && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1981, 04, 01)))
                    {
                        if (importoCristallizzazione.Value > 21.3297M)
                        {
                            messaggioVideo = "Importo Cristallizzazione 34/81 superiore a 21,3297";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo COD-RIDUZIONE  = "S" controlla 
        /// *  che il campo  IABCONA2 NOT = "1" and "2" segnala errore “Incompatibilità  tra CODICE RIDUZIONE EX L.214  e natura pensione” 
        /// 
        /// </summary>
        /// <param name="codRiduzione"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodRiduzioneWithCodNatura(bool? codRiduzione, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codRiduzione.HasValue && codRiduzione.Value)
            {
                if (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("1") && !codNatura.Substring(0, 1).Equals("2")))
                {
                    messaggioVideo = "Incompatibilità tra CODICE RIDUZIONE EX L.214 e natura pensione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo COD-RIDUZIONE  = "S" controlla 
        /// *  Se il campo  (IW1NAT6 (data nascita titolare da 6)  + 6200) minore IW1DEORIG (dec. originaria) segnala errore "Incompatibilità  tra CODICE RIDUZIONE EX L.214  (più di 
        /// 62 anni alla decorrenza)" 
        /// </summary>
        /// <param name="codRiduzione"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodRiduzioneWithEtaTitolare(bool? codRiduzione, DateTime? dataNascitaTitolare, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codRiduzione.HasValue && codRiduzione.Value)
            {
                if (dataNascitaTitolare.HasValue && decorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(new DateTime(dataNascitaTitolare.Value.Year + 62, dataNascitaTitolare.Value.Month, 01), decorrenzaOriginaria.Value))
                {
                    messaggioVideo = "Incompatibilità  tra CODICE RIDUZIONE EX L.214  (più di 62 anni alla decorrenza)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo COD-RIDUZIONE = "S" e il campo PER-RIDUZIONE (% di riduzione) = zero segnala errore "Incompatibilità  tra CODICE RIDUZIONE EX L.214 ed PERCENTUALE RIDUZIONE" 
        /// </summary>
        /// <param name="codRiduzione"></param>
        /// <param name="percentualeRiduzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodRiduzioneWithPercentualeRiduzione(bool? codRiduzione, decimal? percentualeRiduzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codRiduzione.HasValue && codRiduzione.Value && percentualeRiduzione.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Incompatibilità tra CODICE RIDUZIONE EX L.214 ed PERCENTUALE RIDUZIONE";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1DOMOPZ (DATA DOMANDA DELL’ OPZIONE) > 0 allora chiama il programma PCIPL93 passando come parametro la stessa data e tipo-errore come codice di ritorno, se 
        /// il TIPO-ERRORE >  "00"  segnala errore "DATA DOMANDA OPZIONE ILLOGICA" .
        /// Se il campo W1DOMOPZ > 19940100 allora controlla  ((ICI2CONV NOT = "09" AND "20" AND "29") AND   IW1DEORIG > 199400  ) OR ( (ICI2CONV NOT = "27" ) OR ( (ICI2CONV  = "27" 
        /// )  AND   IW1DEORIG > 199504) OR ( (ICI2CONV  = "17" ) AND   IW1DEORIG > 200205  ) OR ( (ICI2CONV  = "13" AND STATO(1) = 38) AND IW1DEORIG > 200207  ) OR ( (ICI2CONV  = 
        /// "38" )  AND   IW1DEORIG > 200404 ) segnala errore "DATA DOMANDA OPZIONE INCOMPATIBILE CON  CODICE CONVENZIONE" 
        /// Altrimenti
        /// Se il campo IW1DOMOPZ > 19680500 AND  minore 19760132 allora controlla IW1CATPEN > 6 segnala errore "DATA DOMANDA OPZIONE INCOMPATIBILE CON  CATEGORIA / DECORRENZA ".
        /// Se il campo IF  (IW1CATPEN = 4 OR 5) AND  IW1DEORIG > 196805 segnala errore "DATA DOMANDA OPZIONE INCOMPATIBILE CON  CATEGORIA / DECORRENZA ".
        /// Se il campo IW1DOMOPZ minore 19720700 allora controlla APPO-CAT1 = "I"  segnala errore DATA DOMANDA OPZIONE INCOMPATIBILE CON CATEGORIA / DECORRENZA " altrimenti segnala 
        /// errore "DATA DOMANDA OPZIONE ERRATA" .
        /// Se il campo IW1DOMOPZ > 0 AND ICI2CONV = "39"  segnala errore "DATA DOMANDA OPZIONE INCOMPATIBILE CON CONVENZIONE CROATA  (39) " .
        /// </summary>
        /// <param name="dataDomandaOpzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataDomandaOpzione(DateTime? dataDomandaOpzione, byte? codiceConvenzione, DateTime? decorrenzaOriginaria, int? primoCodiceStatoEE, int categoria, string gruppo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataDomandaOpzione.HasValue)
            {
                if (!Utility.VerificaData(dataDomandaOpzione, Utility.TipoAppartenenza.CI, out messaggioVideo))
                {
                    messaggioVideo = "Data Domanda Opzione: " + messaggioVideo;
                    return false;
                }

                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(1994, 01, 01)))
                {
                    if (((codiceConvenzione.GetValueOrDefault() == 9 || codiceConvenzione.GetValueOrDefault() == 20 || codiceConvenzione.GetValueOrDefault() == 29) && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01))) ||
                        (codiceConvenzione.GetValueOrDefault() == 27 && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1995, 04, 30))) ||
                        (codiceConvenzione.GetValueOrDefault() == 17 && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 05, 31))) ||
                        (codiceConvenzione.GetValueOrDefault() == 13 && primoCodiceStatoEE.GetValueOrDefault() == 38 && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 07, 31))) ||
                        (codiceConvenzione.GetValueOrDefault() == 38 && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 04, 30))))
                    {
                        messaggioVideo = "Data Domanda Opzione incompatibile con Codice Convenzione";
                        return false;
                    }
                }
                else
                {
                    if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(1968, 05, 01)) && !Utility.DataStrettamenteSuccessivaA(dataDomandaOpzione.Value, new DateTime(1976, 01, 31)))
                    {
                        if (categoria > 6)
                        {
                            messaggioVideo = "Data Domanda Opzione incompatibile con Categoria / Decorrenza";
                            return false;
                        }

                        if (categoria == 4 || categoria == 5 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 31)))
                        {
                            messaggioVideo = "Data Domanda Opzione incompatibile con Categoria / Decorrenza";
                            return false;
                        }

                        if (!Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(1972, 07, 01)))
                        {
                            if (gruppo.Equals("0002"))
                            {
                                messaggioVideo = "Data Domanda Opzione incompatibile con Categoria / Decorrenza";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        messaggioVideo = "Data Domanda Opzione errata";
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 39)
                {
                    messaggioVideo = "Data Domanda Opzione incompatibile con Convenzione Croata (39)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1DEOP (DECORRENZA OPZIONE)  = 0    allora controlla che IW1DOMOPZ > 0 segnala errore "DECORRENZA OPZIONE INCOMPATIBILE CON DATA DOMANDA OPZIONE"  
        /// altrimenti  se il campo IW1DOMOPZ = 0 segnala errore "DECORRENZA OPZIONE INCOMPATIBILE CON DATA DOMANDA OPZIONE"     
        /// Se il campo ICI2CONV = "17"  AND  ( IW1DOMOPZ > 20020600  AND minore 20040600 ) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese di appoggio 
        /// W-DATA-MM = 12  allora somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo IW1DEOP minore 200206 OR > W-DATA (data di appoggio calcolata in precedenza)  segnala errore "DECORRENZA OPZIONE INCOMPATIBILE CON DATA DOMANDA (06-2002 / 
        /// W-DATA-MM  -  W-DATA-AA )"   
        /// Se  il campo  ICI2CONV = "17" AND ( IW1DOMOPZ > 20040600) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese di appoggio W-DATA-MM = 12  allora 
        /// somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo IW1DEOP NOT = W-DATA  (data di appoggio calcolata in precedenza)  "DECORR. OPZIONE INCOMPATIBILE CON MESE SUCC. DATA DOMANDA ( W-DATA-MM  "-"  W-DATA-AA ")" 
        /// Se il campo  (ICI2CONV = "13"  AND STATO(1) = 38) and  ( IW1DOMOPZ > 20040500  AND minore 20060500) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese 
        /// di appoggio W-DATA-MM = 12  allora somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo IW1DEOP minore 200405 OR > W-DATA   segnala errore "DECORRENZA OPZIONE INCOMPATIBILE CON DATA DOMANDA (05-2004 / W-DATA-MM  -  W-DATA-AA ")" 
        /// Se il campo (ICI2CONV = "13"  AND STATO(1) = 38) AND ( IW1DOMOPZ > 20060500) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese di appoggio 
        /// W-DATA-MM = 12  allora somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo  IW1DEOP NOT = W-DATA " segnala errore “DECORR.  OPZIONE  INCOMPATIBILE  CON MESE SUCC. DATA DOMANDA ( W-DATA-MM  "-"  W-DATA-AA ")"  .
        /// Se il campo (ICI2CONV = "38" ) AND  ( IW1DOMOPZ > 20040500  AND minore 20060500 ) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese di appoggio 
        /// W-DATA-MM = 12  allora somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo IW1DEOP minore 200405 OR > W-DATA segnala errore  "DECORRENZA OPZIONE INCOMPATIBILE CON DATA DOMANDA “(05-2004 /W-DATA-MM - W-DATA-AA ")"  
        /// Se il campo (ICI2CONV = "38"  ) AND ( IW1DOMOPZ > 20060500) muove il campo IW1OPZ6 su una data di comodo, allora controlla che il mese di appoggio W-DATA-MM = 12  allora 
        /// somma 1 nell’anno di appoggio e mette 1 nel mese di appoggio altrimenti somma solo 1 al mese di appoggio.
        /// Se il campo IW1DEOP NOT = W-DATA  segnala errore "DECORR. OPZIONE INCOMPATIBILE  CON MESE SUCC. DATA DOMANDA (" W-DATA-MM -  W-DATA-AA ")"  
        /// Se I campi   (IW1CODOPZ NOT = 7) AND  ( IW1DEOP > 0   OR   IW1DOMOPZ > 0 ) AND  ( ICI2CONV = "12" ) allora controlla i campi   IW1DEORIG > 200205 AND APPO-CAT1 NOT = "S" 
        /// segnala errore "OPZIONE INCOMPATIBILE CON DECORRENZA PENSIONE"
        /// Se i campi   (IW1DEOP > 0   OR   IW1DOMOPZ > 0 )  AND  (ICI2CONV = "12" AND STATO(1) = "17" ) allora controlla il campo  TP1CITT1 (CITTADINANZA)   = "IS "   OR   "FL "  
        /// OR "N  " segnala errore "OPZIONE INCOMPATIBILE CON SVIZZERA E CITTADINANZA 'IS'-'FL'-'N' "  
        /// </summary>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="dataDomandaOpzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaOpzioneWithDataDomandaOpzione(DateTime? decorrenzaOpzione, DateTime? dataDomandaOpzione, byte? codiceConvenzione, int primoCodiceStatoEE, byte? codiceOpzioneRiliquidazione, DateTime? decorrenzaOriginaria, Utility.TipoDomanda tipoDomanda, string cittadinanza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if ((!decorrenzaOpzione.HasValue || !dataDomandaOpzione.HasValue) && (decorrenzaOpzione.HasValue || dataDomandaOpzione.HasValue))
            {
                messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda Opzione";
                return false;
            }

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if (codiceConvenzione.GetValueOrDefault() == 17 && dataDomandaOpzione.HasValue)
            {
                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2002, 06, 01)) && !Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2004, 06, 01)))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(2002, 06, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda (06-2002 / " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }

                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2004, 06, 01)))
                {
                    if (!decorrenzaOpzione.Equals(dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con mese succ. Data Domanda ( " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }
            }

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if (codiceConvenzione.GetValueOrDefault() == 13 && primoCodiceStatoEE == 38 && dataDomandaOpzione.HasValue)
            {
                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2005, 05, 01)) && !Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2006, 05, 01)))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(2004, 05, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda (05-2004 / " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }

                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2006, 05, 01)))
                {
                    if (!decorrenzaOpzione.Equals(dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con mese succ. Data Domanda ( " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }
            }

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if (codiceConvenzione.GetValueOrDefault() == 38 && dataDomandaOpzione.HasValue)
            {
                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2004, 05, 01)) && !Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2006, 05, 01)))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(2004, 05, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda (05-2004 / " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }

                if (Utility.DataSuccessivaA(dataDomandaOpzione.Value, new DateTime(2006, 05, 01)))
                {
                    if (!decorrenzaOpzione.Equals(dataDomandaOpzione.Value.AddMonths(1)))
                    {
                        messaggioVideo = "Decorrenza Opzione incompatibile con mese succ. Data Domanda ( " + String.Format("{0:MM-yyyy}", dataDomandaOpzione) + ")";
                        return false;
                    }
                }
            }

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if (codiceOpzioneRiliquidazione.HasValue && codiceOpzioneRiliquidazione.Value != 7 && dataDomandaOpzione.HasValue && codiceConvenzione.GetValueOrDefault() == 12)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 05, 31)) && tipoDomanda != Utility.TipoDomanda.Superstiti)
                {
                    messaggioVideo = "Opzione incompatibile con Decorrenza Pensione";
                    return false;
                }
            }

            // decorrenzaOpzione e dataDomandaOpzione sono necessariamente valorizzate entrambe o non valorizzate entrambe;
            //quindi se una delle due è valorizzata lo è anche l'altra
            if (dataDomandaOpzione.HasValue && codiceConvenzione.GetValueOrDefault() == 12 && primoCodiceStatoEE == 17)
            {
                if (!string.IsNullOrEmpty(cittadinanza) && (new List<string> { "Z117", "Z119", "Z125" }).Contains(cittadinanza))
                {
                    messaggioVideo = "Opzione incompatibile con Svizzera e Cittadinanza 'ISLANDA'-'LIECHTENSTEIN'-'NORVEGIA'";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  (APPO-CAT1 = "I" AND IW1DEORIG > 198407) AND (IABCONA2 NOT = "3" AND "4") allora controlla:
        /// *  Se  il campo  ( NRICONOSC  = 1 AND (DATA-SYS-SSAA - IW1DEORA) > 3 )  OR  ( NRICONOSC  = 2 AND (DATA-SYS-SSAA - IW1DEORA) > 6 ) OR  ( NRICONOSC  > 1 AND (DATA-SYS-SSAA 
        /// - IW1DEORA) minore 3 ) OR  ( NRICONOSC  > 2 AND (DATA-SYS-SSAA - IW1DEORA) minore 6 )   allora segnala errore  "NUMERO RICONOSCIMENTI INCOMPATIBILE CON  DECORRENZA"
        /// * altrimenti NRICONOSC  > 0  allora segnala errore "NUMERO RICONOSCIMENTI INCOMPATIBILE CON CATEGORIA PENSIONE"   
        /// </summary>
        /// <param name="nRiconoscimentiInvalidita"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="gruppo"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaNRicoscimentiInvaliditaWithDecorrenza(byte? nRiconoscimentiInvalidita, DateTime? decorrenzaOriginaria, string gruppo, string codNatura, string categoria, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;
            if (!String.IsNullOrEmpty(categoria) && categoria.Trim().ToUpperInvariant().StartsWith("I") && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1984, 07, 31)) && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4"))))
            {
                if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Riconosc_Incompatibili_Dec.RICONOSC_INCOMPATIBILI_DEC))
                {
                    if ((nRiconoscimentiInvalidita.GetValueOrDefault() == 1 && dataSistema.Year - decorrenzaOriginaria.Value.Year > 3) ||
                        (nRiconoscimentiInvalidita.GetValueOrDefault() == 2 && dataSistema.Year - decorrenzaOriginaria.Value.Year > 6) ||
                        (nRiconoscimentiInvalidita.GetValueOrDefault() > 1 && dataSistema.Year - decorrenzaOriginaria.Value.Year < 3) ||
                        (nRiconoscimentiInvalidita.GetValueOrDefault() > 2 && dataSistema.Year - decorrenzaOriginaria.Value.Year < 6))
                    {
                        messaggioVideo = "Numero Di Riconoscimenti incompatibile con Decorrenza";
                        return false;
                    }
                }
            }
            else
            {
                if (nRiconoscimentiInvalidita.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Numero Di Riconoscimenti incompatibile con Categoria Pensione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IREQPARD (REQUISITO PARTICOLARE  DIRITTO) =  8 segnala errore "CODICE REQUISITO PARTICOLARE DIRITTO  ERRATO" 
        /// Se il campo IREQPARD = 6 allora controlla 
        /// *  se il campo IW1CATPEN NOT = 4 AND 5 AND 6 segnala errore "CODICE REQUISITO PARTICOLARE DIRITTO  INCOMPATIBILE CON CATEGORIA" 
        /// *  Se il campo IW1CATPEN NOT  = 6 AND IW1DEORIG > 198312  segnala errore "CODICE REQUISITO PARTICOLARE DIRITTO  INCOMPATIBILE CON DECORRENZA"
        /// </summary>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceRequisitiParticolari(byte? codiceRequisitiParticolari, int categoria, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceRequisitiParticolari.GetValueOrDefault() == 8)
            {
                messaggioVideo = "Codice Requisito Particolare Diritto errato";
                return false;
            }

            if (codiceRequisitiParticolari.GetValueOrDefault() == 6)
            {
                if (categoria != 4 && categoria != 5 && categoria != 6)
                {
                    messaggioVideo = "Codice Requisito Particolare Diritto incompatibile con Categoria";
                    return false;
                }

                if (categoria != 6 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1983, 12, 31)))
                {
                    messaggioVideo = "Codice Requisito Particolare Diritto incompatibile con Decorrenza";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IREQPARD = 7 allora controlla (APPO-CAT1 = "S" OR IW1DEORIG minore 198407 ) OR (APPO-CAT1 = "I" AND (IABCONA2 NOT = "3" AND "4") ) segnala  errore  "CODICE 
        /// REQUISITO PARTICOLARE DIRITTO INCOMPATIBILE CON CATEGORIA”
        /// Se i campi  TP1ATEC(attività economica)   = 98 AND TP1PRIN (professione individuale) = 176  allora contro i campi IF  IABCONA4 (PRIMA LETTERA NAT.PEN )= "L"  OR  
        /// IREQPARD(REQUISITO PARTICOLARE  DIRITTO)  = 7 continua altrimenti segnala errore "COD.NAT.PENS O COD.PART.DIRITTO MANCANTE:  L.S.U. (ATT.EC. = 98/176)"
        /// </summary>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codNatura"></param>
        /// <param name="gruppo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceRequisitiParticolariWithDatiGenerici(byte? codiceRequisitiParticolari, Utility.TipoDomanda tipoDomanda, DateTime? decorrenzaOriginaria, string codNatura, string gruppo, int? attivitaEconomica, int? professioneIndividuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceRequisitiParticolari.GetValueOrDefault() == 7)
            {
                if ((tipoDomanda == Utility.TipoDomanda.Superstiti || !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1984, 07, 01))) || (gruppo.Equals("0002") && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4")))))
                {
                    messaggioVideo = "Codice Requisito Particolare Diritto incompatibile con Categoria";
                    return false;
                }
            }

            if (attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176)
            {
                if (!((!string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("L")) || codiceRequisitiParticolari.GetValueOrDefault() == 7))
                {
                    messaggioVideo = "Codice Natura Pensione o Codice Particolare Diritto mancante: L.S.U. (Attività Economica = 98/176)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo (IW1CARIC = 3 OR 9) OR  (IW1CARIC = 1 AND IREQPARD = 7) OR  (IW1CARIC = 1 AND IABCONA4 = "H")  allora controlla 
        /// *  se i campi  (PRECCAT = "000" OR PRECCER =  "00000000" ) (PRECEDENTE CATEGORIA E CERTIFICATO (PER CAUSACARICO 3/5/9) allora segnala errore "DATI PRECEDENTE PENSIONE 
        /// MANCANTI O ERRATI"   
        /// *  Se i campi (PRECSEDE > "0000" OR PRECCAT > "000" OR PRECCER > "00000000" )   (PRECEDENTE SEDE CATEGORIA E CERTIFICATO (PER CAUSACARICO 3/5/9) allora controlla 
        ///    *  se il campo (PRECCER minore "00001001") allora segnala errore "CERTIFICATO PRECEDENTE PENSIONE MANCANTE O  ERRATO"                                                                 
        ///    *  Se il campo IF (PRECCAT > "000" AND minore "007")OR (PRECCAT > "014" AND minore "024")OR (PRECCAT > "084" AND minore "094") allora segnala errore "CATEGORIA 
        ///    PRECEDENTE PENSIONE MANCANTE O ERRATA"                       
        ///    *  Se il campo IREQPARD(REQUISITO PARTICOLARE  DIRITTO)  = 7  OR IABCONA4 = "H"  allora controlla IF (PRECCAT =  "002" OR  "005" OR "016" OR "019" OR "022" OR  "086" 
        ///    OR "089" OR "092") OR (PRECCAT =  "004" AND IW1CATPEN = 4  AND TP1ATEC  = 98 AND TP1PRIN = 176) OR (PRECCAT =  "085" AND IW1CATPEN = 85  AND TP1ATEC  = 98 AND TP1PRIN 
        ///    = 176) OR (PRECCAT =  "088" AND IW1CATPEN = 88    AND TP1ATEC  = 98 AND TP1PRIN = 176) OR (PRECCAT =  "091" AND IW1CATPEN = 91 AND TP1ATEC  = 98 AND TP1PRIN = 176) 
        ///    continua altrimenti segnala errore  "CATEGORIA PRECEDENTE PENSIONE MANCANTE O ERRATA"               
        ///    *  Se il campo PRECSEDE minore "0000" allora segnala errore "SEDE PRECEDENTE PENSIONE MANCANTE O ERRATA"  
        ///    *  altrimenti controlla i campi IREQPARD = 7 OR IABCONA4 = "H" allora segnala errore "CERTIFICATO PRECEDENTE PENSIONE MANCANTE" 
        /// *  altrimenti controlla (PRECSEDE > "0000" OR PRECCAT > "000" OR PRECCER >  "00000000" ) allora controlla IW1CARIC = 1AND IW1CATPEN = PRECCAT AND APPO-CAT1 = "V" AND 
        /// IW1DEORIG > 200301 continua altrimenti segnala errore  "DATI PRECEDENTE PENSIONE INCOMPATIBILI CON CAUSA CARICO" 
        /// Se il campo (IW1CARIC = 2 AND IREQPARD = 7) allora controlla Se il campo IDECASS (DECORRENZA ASSEGNO DI INVALIDITA')  > 0   allora controlla 
        /// *  Se il campo IDECASS  minore  198408 allora segnala errore "DATI A.O.I.: DECORRENZA ERRATA   (MINORE DI 08/1984)"   
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="codNatura"></param>
        /// <param name="codiceP18PrecedentePensione"></param>
        /// <param name="certificatoPrecedentePensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDatiPrecedentePensione(byte? causaCarico, byte? codiceRequisitiParticolari, string codNatura, short? codiceP18PrecedentePensione, int? certificatoPrecedentePensione, short? sedePrecedentePensione, int? categoria, int? attivitaEconomica, int? professioneIndividuale, string gruppo, DateTime? decorrenzaOriginaria, DateTime? decorrenzaOriginariaAltraPensione, bool? isTrasformazioneAOI, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((causaCarico.GetValueOrDefault() == 3 || causaCarico.GetValueOrDefault() == 9) || (causaCarico.GetValueOrDefault() == 1 && codiceRequisitiParticolari.GetValueOrDefault() == 7) ||
                (causaCarico.GetValueOrDefault() == 1 && !string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("H")))
            {
                if (codiceP18PrecedentePensione.GetValueOrDefault() == 0 || certificatoPrecedentePensione.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Dati Precedente Pensione mancanti o errati";
                    return false;
                }

                if (sedePrecedentePensione.GetValueOrDefault() > 0 || codiceP18PrecedentePensione.GetValueOrDefault() > 0 || certificatoPrecedentePensione.GetValueOrDefault() > 0)
                {
                    if (certificatoPrecedentePensione.GetValueOrDefault() < 1001)
                    {
                        messaggioVideo = "Certificato Precedente Pensione mancante o errato";
                        return false;
                    }

                    if (!((codiceP18PrecedentePensione.GetValueOrDefault() > 0 && codiceP18PrecedentePensione < 7) || (codiceP18PrecedentePensione.GetValueOrDefault() > 14 && codiceP18PrecedentePensione.GetValueOrDefault() < 24) || (codiceP18PrecedentePensione.GetValueOrDefault() > 84 && codiceP18PrecedentePensione.GetValueOrDefault() < 94)))
                    {
                        messaggioVideo = "Categoria Precedente Pensione mancante o errata";
                        return false;
                    }

                    if (codiceRequisitiParticolari.GetValueOrDefault() == 7 || (!string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("H")))
                    {
                        if (!((new List<int> { 2, 5, 16, 19, 22, 86, 89, 92 }).Contains(codiceP18PrecedentePensione.GetValueOrDefault()) ||
                            (codiceP18PrecedentePensione.GetValueOrDefault() == 4 && categoria.GetValueOrDefault() == 4 && attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176) ||
                            (codiceP18PrecedentePensione.GetValueOrDefault() == 85 && categoria.GetValueOrDefault() == 85 && attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176) ||
                            (codiceP18PrecedentePensione.GetValueOrDefault() == 88 && categoria.GetValueOrDefault() == 88 && attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176) ||
                            (codiceP18PrecedentePensione.GetValueOrDefault() == 91 && categoria.GetValueOrDefault() == 91 && attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176)))
                        {
                            messaggioVideo = "Categoria Precedente Pensione mancante o errata";
                            return false;
                        }
                    }

                    if (sedePrecedentePensione.GetValueOrDefault() < 0)
                    {
                        messaggioVideo = "Sede Precedente Pensione mancante o errata";
                        return false;
                    }
                }
                else
                {
                    if (codiceRequisitiParticolari.GetValueOrDefault() == 7 || (!string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("H")))
                    {
                        messaggioVideo = "Certificato Precedente Pensione mancante";
                        return false;
                    }
                }
            }
            else if (!isTrasformazioneAOI.GetValueOrDefault())
            {
                if (sedePrecedentePensione.GetValueOrDefault() > 0 || codiceP18PrecedentePensione.GetValueOrDefault() > 0 || certificatoPrecedentePensione.GetValueOrDefault() > 0)
                {
                    if (!(causaCarico.GetValueOrDefault() == 1 && categoria == codiceP18PrecedentePensione.GetValueOrDefault() && gruppo.Equals("0001") && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 01, 31))))
                    {
                        messaggioVideo = "Dati Precedente Pensione incompatibili con Causa Carico";
                        return false;
                    }
                }
            }

            if (causaCarico.GetValueOrDefault() == 2 && codiceRequisitiParticolari.GetValueOrDefault() == 7)
            {
                if (decorrenzaOriginariaAltraPensione.HasValue)
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOriginariaAltraPensione.Value, new DateTime(1984, 08, 01)))
                    {
                        messaggioVideo = "Dati A.O.I.: Decorrenza errata (minore di 08/1984)";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1CARIC (codice causa carico) NOT = 9 AND 2 e il campo  IDECNAT3 (DECORRENZA CODICE VIRTUALE = 2 (SOLO PER RICOSTITUZIONI)) >  zero segnala errore 
        /// "DECORR.CODICE VIRTUALE INCOMPATIBILE CON  CAUSA CARICO". 
        /// Se il campo IW1CARIC (codice causa carico) = 9 O  2 allora controllo che il campo IDECNAT3 = 0 allora controllo il campo ICODVIRT = "2" segnala errore "DECORR.CODICE 
        /// VIRTUALE MANCANTE ". 
        /// Se il campo (ICODVIRT = "2" OR "5" OR "6") allora controlla che il campo   (IDECNAT3M minore 1 OR > 12) OR  (IDECNAT3  minore IW1DEORIG)  OR  (IDECNAT3  > DATA-GIORNO-6) 
        /// segnala errore "DECORR.CODICE VIRTUALE ILLOGICA"  altrimenti segnala errore "DECORR.CODICE VIRTUALE NON DEVE ESSERE  ACQUISITA" 
        /// Se il campo (ICODVIRT = "2" ) AND (IDECNAT3 > IW1DEORIG  AND  IW1DEORIG > 199206) allora segnala errore "DECORR.CODICE VIRTUALE INCOMPATIBILE CON DEC.ORIGINARIA"
        /// Se IDECNAT3  > 0 allora controlla se ICI2CONV = 13 AND IDECNAT3 NOT =  IW1DEORIG allora segnala errore "DECORR.CODICE VIRTUALE INCOMPATIBILE CON CONVENZIONE 13   
        /// "  IW1DEORM  "/"  IW1DEORA 
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="decorrenzaCodiceVirtuale"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaCodiceVirtuale(byte? causaCarico, DateTime? decorrenzaCodiceVirtuale, char? codiceVirtuale, DateTime? decorrenzaOriginaria, byte? codiceConvenzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;
            //ENG - DecorrenzaCodiceVirtuale nel caso di Data di Sistema a Dicembre deve accettare anche il primo mese dell'anno successivo
            if (dataSistema.Month == 12)
                dataSistema = dataSistema.AddMonths(1);
            if (causaCarico.GetValueOrDefault() != 9 && causaCarico.GetValueOrDefault() != 2 && decorrenzaCodiceVirtuale.HasValue)
            {
                messaggioVideo = "Decorrenza Codice Virtuale incompatibile con Causa Carico";
                return false;
            }

            if (causaCarico.GetValueOrDefault() == 9 || causaCarico.GetValueOrDefault() == 2)
            {
                if (!decorrenzaCodiceVirtuale.HasValue)
                {
                    if (codiceVirtuale.GetValueOrDefault() == '2')
                    {
                        messaggioVideo = "Decorrenza Codice Virtuale mancante";
                        return false;
                    }
                }
                else
                {
                    if (codiceVirtuale.GetValueOrDefault() == '2' || codiceVirtuale.GetValueOrDefault() == '5' || codiceVirtuale.GetValueOrDefault() == '6')
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaCodiceVirtuale.Value, decorrenzaOriginaria.Value) || Utility.DataStrettamenteSuccessivaA(decorrenzaCodiceVirtuale.Value, new DateTime(dataSistema.Year, dataSistema.Month, 01)))
                        {
                            messaggioVideo = "Decorrenza Codice Virtuale illogica";
                            return false;
                        }
                    }
                    else
                    {
                        messaggioVideo = "Decorrenza Codice Virtuale non deve essere acquisita";
                        return false;
                    }
                }
            }

            if (codiceVirtuale.GetValueOrDefault() == '2' && decorrenzaCodiceVirtuale.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaCodiceVirtuale.Value, decorrenzaOriginaria.Value) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1992, 06, 30)))
            {
                messaggioVideo = "Decorrenza Codice Virtuale incompatibile con Decorrenza Originaria";
                return false;
            }

            if (decorrenzaCodiceVirtuale.HasValue)
            {
                if (codiceConvenzione.GetValueOrDefault() == 13 && !decorrenzaCodiceVirtuale.Equals(new DateTime(decorrenzaOriginaria.Value.Year, decorrenzaOriginaria.Value.Month, 01)))
                {
                    messaggioVideo = "Decorrenza Codice Virtuale incompatibile con Convenzione 13 " + String.Format("{0:MM/yyyy}", decorrenzaOriginaria);
                    return false;
                }
            }

            return true;
        }

        #endregion PCIPL35

        #region PCIPL25
        public static bool ControlsDecorrenzaOpzioneWithDanteCausa(DateTime? decorrenza, DateTime? decorrenzaOpzione, DateTime? decorrenzaPensioneDiretta, DateTime? decorrenzaOriginaria, DateTime? dataDomandaOpzione, string siglaCategoriaDiretta, string siglaCategoriaTitolare, string codNaturaTitolare, byte? convenzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaOpzione.HasValue)
            {
                if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, decorrenza.Value))
                {
                    messaggioVideo = "Decorrenza Opzione antecedente a Decorrenza Pensione (" + String.Format("{0:MM/aaaa}", decorrenza.Value) + ")";
                    return false;
                }

                if (dataDomandaOpzione.HasValue && dataDomandaOpzione.Value.Year < 1977)
                {
                    if (!string.IsNullOrEmpty(siglaCategoriaDiretta) && (siglaCategoriaDiretta.Equals("IO") || siglaCategoriaDiretta.Equals("IOS")))
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 07, 01)))
                        {
                            messaggioVideo = "Decorrenza Opzione incompatibile con Data Domanda/Categoria Diretta";
                            return false;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(siglaCategoriaTitolare) && siglaCategoriaTitolare.Trim().Equals("SOS") && decorrenzaPensioneDiretta.HasValue && Utility.DataSuccessivaA(decorrenzaPensioneDiretta.Value, new DateTime(1965, 01, 01)) && !Utility.DataSuccessivaA(decorrenzaPensioneDiretta.Value, new DateTime(1968, 05, 01)))
                {
                    if (!string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2")))
                    {
                        if (convenzione.GetValueOrDefault() != 9 && convenzione.GetValueOrDefault() != 20 && convenzione.GetValueOrDefault() != 29 && convenzione.GetValueOrDefault() != 27 && convenzione.GetValueOrDefault() != 17 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1968, 05, 01)))
                        {
                            messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                            return false;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(siglaCategoriaTitolare) && siglaCategoriaTitolare.Trim().Equals("SOS") && decorrenzaPensioneDiretta.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensioneDiretta.Value, new DateTime(1968, 04, 01)) && !Utility.DataSuccessivaA(decorrenzaPensioneDiretta.Value, new DateTime(1969, 06, 01)))
                {
                    if (!string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2")))
                    {
                        if (convenzione.GetValueOrDefault() != 9 && convenzione.GetValueOrDefault() != 20 && convenzione.GetValueOrDefault() != 29 && convenzione.GetValueOrDefault() != 27 && convenzione.GetValueOrDefault() != 17 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1969, 05, 01)))
                        {
                            messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                            return false;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(siglaCategoriaTitolare) && siglaCategoriaTitolare.Trim().Equals("SOS") && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)))
                {
                    if (string.IsNullOrEmpty(codNaturaTitolare) || (!codNaturaTitolare.Substring(0, 1).Equals("1") && !codNaturaTitolare.Substring(0, 1).Equals("2")))
                    {
                        if (!string.IsNullOrEmpty(siglaCategoriaDiretta) && (siglaCategoriaDiretta.Equals("VO") || siglaCategoriaDiretta.Equals("VOS"))) // IMPORTANTE: MANCA LA CATEGORIA 100 NELLA TABELLA A DB
                        {
                            if (convenzione.GetValueOrDefault() != 9 && convenzione.GetValueOrDefault() != 20 && convenzione.GetValueOrDefault() != 29 && convenzione.GetValueOrDefault() != 27 && convenzione.GetValueOrDefault() != 17 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1969, 05, 01)))
                            {
                                messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                                return false;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(siglaCategoriaTitolare) && siglaCategoriaTitolare.Trim().Equals("SOS") && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)))
                {
                    if (!string.IsNullOrEmpty(siglaCategoriaDiretta) && (siglaCategoriaDiretta.Equals("IO") || siglaCategoriaDiretta.Equals("IOS")))
                    {
                        if (convenzione.GetValueOrDefault() != 9 && convenzione.GetValueOrDefault() != 20 && convenzione.GetValueOrDefault() != 29 && convenzione.GetValueOrDefault() != 27 && convenzione.GetValueOrDefault() != 17 && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 07, 01)))
                        {
                            messaggioVideo = "Decorrenza Opzione incompatibile con Categoria / Decorrenza Pensione";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool ControlsRequisitoRidottoWithDanteCausa(DateTime? decorrenza, DateTime? decorrenzaOriginaria, string codNaturaTitolare, byte? legge44997, string siglaCategoriaTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1998, 03, 01)) && (!string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2"))))
            {
                if (legge44997.GetValueOrDefault() > 5)
                {
                    messaggioVideo = "Codice Requisito Ridotto errato (1-2-3-4-5)";
                    return false;
                }
            }
            else
            {
                if (legge44997.HasValue)
                {
                    if (legge44997.Value != 6)
                    {
                        messaggioVideo = "Codice Requisito Ridotto incompatibile con Decorrenza Originaria / Natura Pensione";
                        return false;
                    }
                    else
                    {
                        if ((!string.IsNullOrEmpty(siglaCategoriaTitolare) && !siglaCategoriaTitolare.Equals("VOS")) || !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01)))
                        {
                            messaggioVideo = "Codice Requisito Ridotto incompatibile con Decorrenza Originaria / Categoria";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool ControlsCodiceMobilitaWithDanteCausa(DateTime? decorrenza, string codNaturaTitolare, byte? codiceMobilita, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1998, 03, 01)) && (!string.IsNullOrEmpty(codNaturaTitolare) && (codNaturaTitolare.Substring(0, 1).Equals("1") || codNaturaTitolare.Substring(0, 1).Equals("2"))))
            {
                if (codiceMobilita.GetValueOrDefault() > 4)
                {
                    messaggioVideo = "Codice Mobilità errato (1-2-3-4)";
                    return false;
                }
            }
            else
            {
                if (codiceMobilita.HasValue)
                {
                    messaggioVideo = "Codice Mobilità incompatibile con Decorrenza Originaria / Natura Pensione";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL25

        #region PCIPL29

        //RiliquidazionePostCristallizzazione non mappato nella nostra applicazione
        public static bool ControlsRiliquidazionePostCristallizzazione(DateTime? decorrenzaOriginaria, char? riliquidazionePostCristallizzazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            DateTime dataCompare = new DateTime(1983, 11, 01);
            if (riliquidazionePostCristallizzazione.HasValue && decorrenzaOriginaria.HasValue && riliquidazionePostCristallizzazione.Value == '1' && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, dataCompare))
            {
                messaggioVideo = "CODICE PEREQUAZIONE ERRATO (NON AMMESSO 1 PER DEC. ANTE 11/83)";
                return false;
            }

            return true;
        }

        #endregion PCIPL29

        #region PCIPL40
        /// <summary>
        /// Se il campo APPO-CAT1 = "V"  AND IW1DEORIG > 200100 AND TP1REQRID (REQUISITO RIDOTTO)  NOT = 6 controlla se il campo CONTRIBUTIVA-8 = "S" allora COMPUTE W-APP-4 (campo 
        /// di appoggio) = IW1SECAN (anno data nascita del titolare)  + 57 altrimenti controlla se il campo IW1SESTIT(sesso titolare)  = "F" allora  COMPUTE W-APP-4 = IW1SECAN + 60 
        /// altrimenti  COMPUTE W-APP-4 = IW1SECAN + 65. Se il campo IW1CODC (CODICE CIECO)  = "1" allora  sottrai 5 dal campo W-APP-4 . COMPUTE W-APP-2 = IW1NATITM (mese di nascita 
        /// del titolare) + 1 Se il campo  W-APP-2   >  12 allora muovi 1 nal campo W-APP-2 e somma 1 nel campo W-APP-4 .Se il campo (IABCONA2(CODICE NATURA PENSIONE)  = "1" OR  "2" 
        /// ) allora controlla se  il campo   W-APP-6 minore IW1DEORIG e il campo  IW1DEORIG minore 199401   allora segnala errore "SE NAT-PEN. = 1/2, NON DEVE AVER COMPIUTO ETA' 
        /// PENSIONABILE (CNV01)" Se il campo  W-APP-6 > IW1DEORIG allora controlla ( (IW1DEORIG > 200301 AND  minore  200403) e (IABCONA4  = "L" ) )   oppure  (OPZIONE-CONTRIBUTIVA 
        /// = "S" OR IABCONA3 = "O") continua altrimenti segnala errore "INCOMPATIBILITA' TRA DATA NASCITA ED ETA' PENSIONABILE (CNV01)"     
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="legge44997"></param>
        /// <param name="dataNascita"></param>
        /// <param name="codiceCieco"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsCodNaturaWithEtaPensionabile(string gruppo, DateTime? decorrenzaOriginaria, byte? legge44997, DateTime? dataNascitaTitolare, char? sessoTitolare, byte? codiceCieco, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare;

            if (gruppo.Equals("0001") && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2001, 01, 01)) && legge44997.GetValueOrDefault() != 6)
            {
                if (sessoTitolare.GetValueOrDefault() == 'F')
                    dataCompare = dataNascitaTitolare.Value.AddYears(60);
                else
                    dataCompare = dataNascitaTitolare.Value.AddYears(65);

                if (codiceCieco.GetValueOrDefault() == 1)
                    dataCompare = dataCompare.AddYears(-5);

                if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
                {
                    dataCompare = dataCompare.AddMonths(1);
                    if (!Utility.DataSuccessivaA(dataCompare, decorrenzaOriginaria.Value) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01)))
                    {
                        messaggioVideo = "Se Natura Pensione = 1/2, non deve aver compiuto età pensionabile";
                        return false;
                    }
                }
                else
                {
                    dataCompare = Utility.FirstDayOfMonth(dataCompare).AddMonths(1);
                    if (Utility.DataStrettamenteSuccessivaA(dataCompare, decorrenzaOriginaria.Value))
                    {
                        if (!((Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 01, 31)) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 03, 01)) &&
                            !string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("L")) || (!string.IsNullOrEmpty(codNatura) && codNatura.Substring(1, 1).Equals("O"))))
                        {
                            messaggioVideo = "Incompatibilità tra Data Nascita ed età pensionabile";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// A questo punto il programma fa una chiamata al programma PCIPL93 (per controllo data) usando  il campo INIASS e come ritorno il campo TIPO-ERRORE; se il campo TIPO-ERRORE 
        /// non è = “00” segnala errore "DATA INIZIO ASSICURAZIONE ILLOGICA O MANCANTE". Se il campo IW1DNAS > 0 allora muove IW1DNAS nel campo di appoggio APP-DATA-1N altrimenti 
        /// muove il campo IW1NATIT  nel campo di appoggio APP-DATA-1N. Se il campo INIASS (DATA INIZIO ASSICURAZIONE)   NOT  >  (APP-DATA-1N +  10000) allora segnala errore "DATA 
        /// INIZIO ASSICURAZIONE INCOMPATIBILE CON DATA DI NASCITA " Se il campo INIASS(1:6)  NOT minore W-DEORIG allora segnala errore "DATA INIZIO ASSICURAZIONE POSTERIORE A 
        /// DECORRENZA"                          
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataNascitaDC"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsInizioAssicurazione(DateTime? dataInizioAssicurazione, DateTime? dataNascitaDC, DateTime? dataNascitaTitolare, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? dataCompare = null;

            if (!Utility.VerificaData(dataInizioAssicurazione, Utility.TipoAppartenenza.CI, out messaggioVideo))
            {
                messaggioVideo = "Inizio Assicurazione: " + messaggioVideo;
                return false;
            }

            if (dataNascitaDC.HasValue)
                dataCompare = dataNascitaDC;
            else
                dataCompare = dataNascitaTitolare;

            if (!Utility.DataStrettamenteSuccessivaA(dataInizioAssicurazione.Value, dataCompare.Value.AddYears(1)))
            {
                messaggioVideo = "Data Inizio Assicurazione incompatibile con Data di Nascita";
                return false;
            }

            if (Utility.DataSuccessivaA(dataInizioAssicurazione.Value, decorrenza.Value))
            {
                messaggioVideo = "Data Inizio Assicurazione posteriore a Decorrenza";
                return false;
            }

            return true;
        }

        /// <summary>
        /// A questo punto il programma fa una chiamata al programma PCIPL93 (per controllo data) usando  il campo FINASS e come ritorno il campo TIPO-ERRORE; se il campo 
        /// TIPO-ERRORE non è = “00” segnala errore " DATA ULTIMO CONTRIBUTO ILLOGICA O MANCANTE”. 
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsFineAssicurazione(DateTime? dataFineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.VerificaData(dataFineAssicurazione, Utility.TipoAppartenenza.CI, out messaggioVideo))
            {
                messaggioVideo = "Fine Assicurazione: " + messaggioVideo;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se il campo  IW1DEBON  (DECORRENZA BONUS 2004 PER ANZIANITA')  > 0  e il campo  
        /// (IABCONA3 = "X") allora controlla se il campo FINASS(1:6)  NOT  minore  IW1DEBON  segnala errore "DATA ULTIMO CONTRIBUTO INCOMPATIBILE  CON DEC.BONUS    (X)    (CNV01)"
        /// Se il campo  IW1DEBON  (DECORRENZA BONUS 2004 PER ANZIANITA')  > 0  e il campo  (IABCONA3 = "Y") allora controlla se il campo FINASSM = 12 allora controlla se il campo 
        /// ((FINASSA * 100) + 101)  NOT   = IW1DEBON    segnala  errore DATA ULTIMO CONTRIBUTO INCOMPATIBILE  CON DEC.BONUS    (Y) (CNV01)"  altrimenti  se il campo 
        /// ((FINASSA * 100) + FINASSM + 1)  NOT = IW1DEBON allora controlla che il campo TP1COFI (CODICE FISCALE)   = "FGNDNO43P11L934G" continua altrimenti segnala errore "DATA 
        /// ULTIMO CONTRIBUTO INCOMPATIBILE  "CON DEC.BONUS    (Y)    (CNV01)"  .
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="decorrenzaBonus"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaBonusWithFineAssicurazione(DateTime? dataFineAssicurazione, DateTime? decorrenzaBonus, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaBonus.HasValue && !string.IsNullOrEmpty(codNatura) && codNatura.Substring(1, 1).Equals("X"))
            {
                if (Utility.DataSuccessivaA(new DateTime(dataFineAssicurazione.Value.Year, dataFineAssicurazione.Value.Month, 01), decorrenzaBonus.Value))
                {
                    messaggioVideo = "Data Ultimo Contributo incompatibile con Decorrenza Bonus";
                    return false;
                }
            }

            if (decorrenzaBonus.HasValue && !string.IsNullOrEmpty(codNatura) && codNatura.Substring(1, 1).Equals("Y"))
            {
                if (!(new DateTime(dataFineAssicurazione.Value.AddMonths(1).Year, dataFineAssicurazione.Value.AddMonths(1).Month, 01)).Equals(decorrenzaBonus.Value))
                {
                    messaggioVideo = "Data Ultimo Contributo incompatibile con Decorrenza Bonus";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  ICI2CONV = 33 AND IABCONA2 = "1"  allora controlla se i campi  (TP1NUA + TP1NUB) minore 780 AND (IW1NSOBG + IW1STOBG) minore 780 AND IABCONA4 NOT = "Z"  
        /// allora segnala errore  "SETTIMANE INFERIORI A 780 (ANZIANITA' / CONVENZ.AUSTRALIA)"             
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codNatura"></param>
        /// <param name="settimane"></param>
        /// <param name="nContributiVolontati"></param>
        /// <param name="settimaneDatiRetributiviQuotaA"></param>
        /// <param name="settimaneDatiRetributiviQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneWithAnzianitaAndAustralia(byte? codiceConvenzione, string codNatura, int? settimane, int? nContributiVolontati, int? settimaneDatiRetributiviQuotaA,
            int? settimaneDatiRetributiviQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 33 && !string.IsNullOrEmpty(codNatura) && codNatura.Substring(0, 1).Equals("1"))
            {
                if ((settimane.HasValue || nContributiVolontati.HasValue || settimaneDatiRetributiviQuotaA.HasValue || settimaneDatiRetributiviQuotaB.HasValue) &&
                    (settimane.GetValueOrDefault() + nContributiVolontati.GetValueOrDefault()) < 780 &&
                    (settimaneDatiRetributiviQuotaA.GetValueOrDefault() + settimaneDatiRetributiviQuotaB.GetValueOrDefault()) < 780 && !codNatura.Substring(2, 1).Equals("Z"))
                {
                    messaggioVideo = "Settimane inferiori a 780 (Anzianita' / Convenzione Australia)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  ( (ICI2CONV NOT = 12) OR   (ICI2CONV = 12 AND W-DEORIG > 197209) ) allora controlla se i campi  (  (STATO(1) = 17) AND   (IW1DIRET > 0 AND IW1DIRET minore 200206) )  
        /// continua  altrimenti controlla se il campo F (TP1NUA + TP1NUB + IABNSASS)  minore  MIN-CTR   AND IW1CODOPZ NOT = "7" allora controlla che il campo APPO-CAT1 NOT = "S" 
        /// allora controlla se i campi  ( IABCONA2   =  "1" OR "2" ) AND (IW1NSOBG + IW1STOBG + ICISTOBG335 +  ICISTOBG012 + IABNSASS)   NOT minore MIN-CTR    continua altrimenti 
        /// segnala  STRING "SETTIMANE ITALIANE INFERIORI A  MIN-CTR  PER CONVENZ. “ altrimenti se il campo  (W-DEORIG > 197209 AND ICI2RESEST = "I  " ) AND (ICI2CONV NOT = 38 AND 
        /// 39)  continua  altrimenti controlla se i campi IW1DIRET > 0 AND IW3DESUP(1) > 0  continua altrimenti controlla se i campi  APPO-CAT1 = "S"  AND  ICI2CONV = 12   AND  
        /// STATO(1) = 38  AND  TP1CATD  > 0   continua altrimenti segnala errore   "SETTIMANE ITALIANE INFERIORI A "  MIN-CTR  " PER CONVENZ. "   ICI2CONV .        
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codiceStato"></param>
        /// <param name="settimane"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="nSettimaneGodimentoAssegno"></param>
        /// <param name="codiceOpzioneRiliquidazione"></param>
        /// <param name="gruppo"></param>
        /// <param name="codNatura"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="settimaneContributive"></param>
        /// <param name="settimaneContributiveDL214"></param>
        /// <param name="provinciaResidenza"></param>
        /// <param name="decorrenzaSupplemento"></param>
        /// <param name="siglaCategoriaDiretta"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneWithMinCTRAndConvenzione(byte? codiceConvenzione, DateTime? decorrenzaDiretta, DateTime? decorrenzaOriginaria, DateTime? decorrenza, int codiceStato,
            int? settimane, int? nContributiVolontari, int? nSettimaneGodimentoAssegno, byte? codiceOpzioneRiliquidazione, string gruppo, string codNatura, int? settimaneRetributiveQuotaA,
            int? settimaneRetributiveQuotaB, int? settimaneContributive, int? settimaneContributiveDL214, string comuneResidenza, DateTime? decorrenzaSupplemento, string siglaCategoriaDiretta,
            DateTime? dataMorteDC, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int min_CTR = CTR_Minimi(codiceConvenzione, decorrenzaDiretta, decorrenzaOriginaria, dataMorteDC, gruppo, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            if (codiceConvenzione.HasValue && (codiceConvenzione != 12 || (codiceConvenzione == 12 && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1972, 09, 30)))))
            {
                if (!(codiceStato == 17 && decorrenzaDiretta.HasValue && !Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(2002, 06, 01))))
                {
                    if ((settimane.GetValueOrDefault() + nContributiVolontari.GetValueOrDefault() + nSettimaneGodimentoAssegno.GetValueOrDefault()) < min_CTR && codiceOpzioneRiliquidazione.GetValueOrDefault() != 7)
                    {
                        if (!gruppo.Equals("0003"))
                        {
                            if (!(!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")) && (settimaneRetributiveQuotaA.GetValueOrDefault() + settimaneRetributiveQuotaB.GetValueOrDefault() + settimaneContributive.GetValueOrDefault() + settimaneContributiveDL214.GetValueOrDefault() + nSettimaneGodimentoAssegno.GetValueOrDefault()) >= min_CTR))
                            {
                                messaggioVideo = "Settimane OBG Diritto inferiori a " + min_CTR + " per convenzione " + codiceConvenzione;
                                return false;
                            }
                        }
                        else
                        {
                            if (!(Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1972, 09, 30)) && comuneResidenza != null && !comuneResidenza.StartsWith("Z") && codiceConvenzione != 38 && codiceConvenzione != 39))
                            {
                                if (!(decorrenzaDiretta.HasValue && decorrenzaSupplemento.HasValue))
                                {
                                    if (!(gruppo.Equals("0003") && codiceConvenzione.GetValueOrDefault() == 12 && codiceStato == 38 && !string.IsNullOrEmpty(siglaCategoriaDiretta)))
                                    {
                                        messaggioVideo = "Settimane OBG Diritto inferiori a " + min_CTR + " per convenzione " + codiceConvenzione;
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il  campo IF ICI2SETFIT (SETT.FITTIZIE)  = 0 AND (IABCONA4 = "J" OR "K" OR "Q" OR "W" OR "X" OR "Y" OR "P" OR "O" ) allora segnala errore "MANCANO SETTIMANE FITTIZIE 
        /// PER NATURA PENSIONE "'J-K-Q-W-X-Y-P-O'"             
        /// </summary>
        /// <param name="settimaneFittizie"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneFittizieWithCodNatura(int? settimaneFittizie, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<string> codiciNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "O" };

            if (settimaneFittizie.GetValueOrDefault() == 0 && !string.IsNullOrEmpty(codNatura) && codiciNatura.Contains(codNatura.Substring(2, 1)))
            {
                messaggioVideo = "Mancano Settimane Fittizie per Natura Pensione 'J-K-Q-W-X-Y-P-O'";
                return false;
            }

            return true;
        }

        /// <summary>
        /// A questo punto il programma fa una chiamata al programma PCIPL94 (calcola capienza) usando i campi APP-DATA-1  APP-DATA-2  APP-DATA-RC.
        /// Se il campo  (TP1NUA + TP1NUB) > APP-DATA-RC   AND TP1PRIN NOT = 257  AND TP1PRIN NOT = 350 allora controlla se il campo  IABCONA4  NOT  =  "G"   AND  IABCONA4  
        /// NOT  =  "Z"    allora segnala errore  "SETT. OBG + VV (DIRITTO) SUPERIORI  A CAPIENZA NEL PERIODO ASSICURATIVO"  
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nSettimaneOBG"></param>
        /// <param name="nContributiUtiliLavoratoriAutonomi"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCapienzaSettimaneWithAssicurazione(GestionePensione.DatiPensione datiPensione, DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione, int? settimane, int? professioneIndividuale, string codNatura,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Num_Sett_Periodo_Ass.NUM_SETT_PERIODO_ASS))
                return true;

            if (dataInizioAssicurazione.HasValue && dataFineAssicurazione.HasValue)
            {
                int? nSettimane = Utility.NSettimaneBetweenDate(dataFineAssicurazione.Value, dataInizioAssicurazione.Value);
                if (nSettimane < 0)
                    nSettimane = 0;
                if (settimane.GetValueOrDefault() > nSettimane && professioneIndividuale.HasValue && professioneIndividuale.Value != 257 && professioneIndividuale.Value != 350)
                {
                    if (!string.IsNullOrEmpty(codNatura) && !codNatura.Substring(2, 1).Equals("G") && !codNatura.Substring(2, 1).Equals("Z"))
                    {
                        messaggioVideo = "Settimane OBG + VV (Diritto) superiori a capienza nel periodo assicurativo";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo TP1NUA = 0 allora controlla se i campi  ( (ICI2CONV NOT = 17 AND 20) AND (STATO(1) NOT = 1)) AND ( ICI2CONV NOT = 27 )   allora segnala errore "SETTIMANE 
        /// ITALIANE DIRITTO MANCANTI"  
        /// Se i campi IW1NSOBG  > 0  OR IW1STOBG    > 0 allora controlla se il campo TP1NUA = 0 allora segnala errore "SETTIMANE OBG PER DIRITTO MANCANTI"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="settimane"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceStato"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaSettimaneOBG(int? settimane, byte? codiceConvenzione, int codiceStato, int? settimaneRetributiveQuotaA, int? settimaneRetributiveQuotaB, GestionePensione.DatiPensione datiPensione,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Settimane_OBG_Mancanti.SETTIMANE_OBG_MANCANTI))
                return true;

            if (settimane.GetValueOrDefault() == 0)
            {
                if (codiceConvenzione.HasValue && codiceConvenzione.Value != 17 && codiceConvenzione.Value != 20 && codiceConvenzione.Value != 27 && codiceStato != 1)
                {
                    messaggioVideo = "Settimane OBG Diritto mancanti";
                    return false;
                }
            }

            if (settimaneRetributiveQuotaA.GetValueOrDefault() > 0 || settimaneRetributiveQuotaB.GetValueOrDefault() > 0)
            {
                if (settimane.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane OBG per Diritto mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  (IABREMSVV = 0 AND  IW1IVSTOT = 0) AND (IW1CATPEN = 6 AND TP1CERTD = 0 AND IW1DMOR minore 19680501) allora segnala errore "IMPORTO I.V.S. MANCANTE".                              
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="importoIVS"></param>
        /// <param name="certificatoPensioneDiretta"></param>
        /// <param name="dataMorteDC"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaImportoIVS(int categoria, decimal? rmsQuotaA, decimal? importoIVS, int? certificatoPensioneDiretta, DateTime? dataMorteDC, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rmsQuotaA.GetValueOrDefault() == 0 && importoIVS.GetValueOrDefault() == 0 && categoria == 6 && certificatoPensioneDiretta.GetValueOrDefault() == 0 && dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(1968, 05, 01)))
            {
                messaggioVideo = "Importo I.V.S. mancante";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se I campi W-DEORIG > 196805 AND minore 197608 allora controlla Se il campo IW1IVSTOT = 0 segnala  errore  "SE DECORR. TRA 05/68 E 07/76 DEVE PRESENTE IVS ED RMS"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="importoIVS"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaImportoIVSWithDecorrenze(decimal? importoIVS, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1968, 05, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1976, 08, 01)))
            {
                if (importoIVS.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Se Decorrenza tra 05/68 e 07/76 devono essere presenti IVS ed RMS";
                    return false;
                }
            }

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1968, 05, 01)) && importoIVS.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Importo I.V.S. mancante (Decorrenza ante 05/1968)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1RMSS72 (RMS PER APPLICAZIONE DELLA SENTENZA N. 72/90) > 0 allora controlla  Se i campi  IABREMSVV = 0   OR  IW1RMSS72 minore IABREMSVV allora segnala 
        /// errore "INCOMPATIBILITA' TRA R.M.S.8888 E  R.M.S. AL 12/92"                                                                                                                                           
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rms8888"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMS8888WithRMSQuotaA(decimal? rms8888, decimal? rmsQuotaA, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rms8888.GetValueOrDefault() > 0)
            {
                if (rmsQuotaA.GetValueOrDefault() == 0 || rms8888.GetValueOrDefault() < rmsQuotaA.GetValueOrDefault())
                {
                    messaggioVideo = "Incompatibilita' tra R.M.S.8888 e R.M.S. al 12/92";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1RMSS72 (RMS PER APPLICAZIONE DELLA SENTENZA N. 72/90) > 0 allora controlla Se il campo W-DEORIG > 198712 allora segnala errore "INCOMPATIBILITA' TRA 
        /// R.M.S.8888 E DECORRENZA ORIGINARIA"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rms8888"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMS8888WithDecorrenza(decimal? rms8888, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rms8888.GetValueOrDefault() > 0)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1987, 12, 31)))
                {
                    messaggioVideo = "Incompatibilita' tra R.M.S.8888 e Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1RMSAR2 (RMS PER RILIQUIDAZIONE ART. 2 DPCM 16/12/89 ) > 0 allora controlla Se i campi  IABREMSVV = 0   OR  IW1RMSAR2 minore IABREMSVV segnala errore 
        /// "INCOMPATIBILITA' TRA R.M.S.9090 E  R.M.S. AL 12/92"  
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rms9090"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMS9090WithRMSQuotaA(decimal? rms9090, decimal? rmsQuotaA, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rms9090.GetValueOrDefault() > 0)
            {
                if (rmsQuotaA.GetValueOrDefault() == 0 || rms9090.GetValueOrDefault() < rmsQuotaA.GetValueOrDefault())
                {
                    messaggioVideo = "Incompatibilita' tra R.M.S.9090 e R.M.S. al 12/92";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1RMSAR2 (RMS PER RILIQUIDAZIONE ART. 2 DPCM 16/12/89 ) > 0 allora controlla Se il campo IW1DEOP > 0 allora controlla se i campi (IW1DEOP minore 197101 
        /// OR > 198412) allora segnala errore "INCOMPATIBILITA' TRA R.M.S.9090 E  DECORRENZA OPZIONE(CNV01)"    altrimenti controlla se i campi W-DEORIG minore 197101 OR > 198412 
        /// allora segnala errore "INCOMPATIBILITA' TRA R.M.S.9090 E  DECORRENZA ORIGINARIA"    
        /// Se il campo IW1DDPCM = 0 allora segnala errore "INCOMPATIBILITA' TRA R.M.S.9090 E DECORRENZA DPCM (CNV01)"     
        ///	Se i campi  W-DEORIG minore 196805 AND IW1DEOP > 0 allora muovi  IW1DEOPA (ANNO DECORRENZA OPZIONE) nel campo APP-AP  altrimenti  muovi  W-DEORIG-A (ANNO DI APPOGGIO 
        ///	DATA DEC. ORIG.) nel campo APP-AP  
        ///	Se i campi  (APP-APP = 1971 AND IW1RMSAR2 > 130,6740)
        ///	OR  (APP-APP = 1972 AND rms9090> 136,3152)
        ///	OR  (APP-APP = 1973 AND rms9090 > 143,2079)
        ///	OR  (APP-APP = 1974 AND rms9090 > 156,0893)
        ///	OR  (APP-APP = 1975 AND rms9090 > 174,7314)
        ///	OR  (APP-APP = 1976 AND rms9090 > 187,8912)
        ///	OR  (APP-APP = 1977 AND rms9090 > 199,2930)
        ///	OR  (APP-APP = 1978 AND rms9090 > 220,2689)
        ///	OR  (APP-APP = 1979 AND rms9090 > 229,5553)
        ///	OR  (APP-APP = 1980 AND rms9090 > 240,6989)
        ///	OR  (APP-APP = 1981 AND rms9090 > 258,6226)
        ///	OR  (APP-APP = 1982 AND rms9090 > 275,1626)
        ///	OR  (APP-APP = 1983 AND rms9090 > 292,5631)
        ///	OR  (APP-APP = 1984 AND rms9090 > 301,2445) allora segnala errore "IMPORTO R.M.S.9090 INCOMPATIBILE  CON DECORRENZA"  altrimenti  controlla se i campi IW3DESUP(1) 
        ///	(ANNO DECORRENZA DEL SUPPL ) = 0 AND IW1DDPCM((AAMM) DECORRENZA ART.2 DPCM 16/12/89)  > 0 allora segnala errore  "SE PRESENTE DEC.DPCM (CNV01) DEVE ESSERCI ANCHE R.M.S. 
        ///	9090"   
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rms9090"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMS9090WithDecorrenze(decimal? rms9090, DateTime? decorrenzaOpzione, DateTime? decorrenza, DateTime? decorrenzaDPCM, DateTime? decorrenzaSupplemento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rms9090.GetValueOrDefault() > 0)
            {
                if (decorrenzaOpzione.HasValue)
                {
                    if (!Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1971, 01, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1984, 12, 31)))
                    {
                        messaggioVideo = "Incompatibilita' tra R.M.S.9090 e Decorrenza Opzione";
                        return false;
                    }
                }
                else
                {
                    if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1971, 01, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1984, 12, 31)))
                    {
                        messaggioVideo = "Incompatibilita' tra R.M.S.9090 e Decorrenza Originaria";
                        return false;
                    }
                }

                if (!decorrenzaDPCM.HasValue)
                {
                    messaggioVideo = "Incompatibilita' tra R.M.S.9090 e Decorrenza DPCM";
                    return false;
                }

                int anno = 0;
                if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1968, 05, 01)) && decorrenzaOpzione.HasValue)
                    anno = decorrenzaOpzione.Value.Year;
                else
                    anno = decorrenza.Value.Year;

                if ((anno == 1971 && rms9090 > 130.6740M) ||
                    (anno == 1972 && rms9090 > 136.3152M) ||
                    (anno == 1973 && rms9090 > 143.2079M) ||
                    (anno == 1974 && rms9090 > 156.0893M) ||
                    (anno == 1975 && rms9090 > 174.7314M) ||
                    (anno == 1976 && rms9090 > 187.8912M) ||
                    (anno == 1977 && rms9090 > 199.2930M) ||
                    (anno == 1978 && rms9090 > 220.2689M) ||
                    (anno == 1979 && rms9090 > 229.5553M) ||
                    (anno == 1980 && rms9090 > 240.6989M) ||
                    (anno == 1981 && rms9090 > 258.6226M) ||
                    (anno == 1982 && rms9090 > 275.1626M) ||
                    (anno == 1983 && rms9090 > 292.5631M) ||
                    (anno == 1984 && rms9090 > 301.2445M))
                {
                    messaggioVideo = "Importo R.M.S.9090 incompatibile con Decorrenza";
                    return false;
                }
            }
            else
            {
                if (!decorrenzaSupplemento.HasValue && decorrenzaDPCM.HasValue)
                {
                    messaggioVideo = "Se presente Decorrenza DPCM deve esserci anche R.M.S. 9090";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi ICISET1X100 + ICISET05X100) > (IW1STOBG + ICISTOBG335 + icistobg012 )  allora segnala errore "SETTIMANE DAL 1993 INCOMPATIBILI CON SETT. INCR 1% - 0.5% (CNV01)"          
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="nSettimaneIncremento1Percento"></param>
        /// <param name="nSettimaneIncremento05Percento"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="settimaneContributiveQuotaD"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(int? nSettimaneIncremento1Percento, int? nSettimaneIncremento05Percento, int? settimaneRetributiveQuotaB, int? settimaneContributiveQuotaC, int? settimaneContributiveQuotaD, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((nSettimaneIncremento1Percento.GetValueOrDefault() + nSettimaneIncremento05Percento.GetValueOrDefault()) > (settimaneRetributiveQuotaB.GetValueOrDefault() + settimaneContributiveQuotaC.GetValueOrDefault() + settimaneContributiveQuotaD.GetValueOrDefault()))
            {
                messaggioVideo = "Settimane dal 1993 incompatibili con Settimane Incremento 1% - 0.5%";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se i campi (IW1VVMISURA + ICI1VVOBG (N. SETTIMANE VERS.VOL.OBG-503) ) > TP1NUB allora segnala errore "SETTIMANE VV MISURA SUPERIORI A VV DIRITTO"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCapienzaSettimaneVV(int? vvMisuraAl1292, int? vvMisuraDL50392, int? nContributiVolontari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((vvMisuraAl1292.GetValueOrDefault() + vvMisuraDL50392.GetValueOrDefault()) > nContributiVolontari.GetValueOrDefault())
            {
                messaggioVideo = "Settimane VV Misura superiori a VV Diritto";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se i campi (ICI2CONV  = 17 OR 20) AND IW1NSOBG = 0 AND TP1NUA = 0 allora controlla se i campi IW1VVMISURA = 0 AND ICI1VVOBG = 0 AND 
        /// IABAR11VV(IMP.IVS ART.11 DEI VV  )  = 0 allora segnala errore "SETTIMANE V.V. PER MISURA MANCANTI"
        /// Se il campo TP1NUB = 0  allora segnala errore "SETTIMANE V.V. PER DIRITTO MANCANTI"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="settimane"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="importoIVS"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaSettimaneVV(byte? codiceConvenzione, int? settimaneRetributiveQuotaA, int? settimane, int? vvMisuraAl1292, int? vvMisuraDL50392, decimal? importoIVS_art11,
            int? nContributiVolontari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((codiceConvenzione.GetValueOrDefault() == 17 || codiceConvenzione.GetValueOrDefault() == 20) && settimaneRetributiveQuotaA.GetValueOrDefault() == 0 && settimane.GetValueOrDefault() == 0)
            {
                if (vvMisuraAl1292.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0 && importoIVS_art11.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane V.V. per Misura mancanti";
                    return false;
                }

                if (nContributiVolontari.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane V.V. per Diritto mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IABCONA2 NOT = "3" AND "4" allora controlla se i campi  ICI2SETFIT > 0 AND (IABCONA4  NOT =  "J" AND "K" AND "Q" AND "W" AND   "X" AND "Y" AND "P" AND "O")  
        /// allora segnala errore "SETTIMANE FITTIZIE NON AMMESSE"    altrimenti controlla se il campo APPO-CAT1 = "S"  muove il campo  IW1DNAS nel campo APP-DATA-2 e muove  IW1DSES 
        /// nel campo APP-SESS  altrimenti muove IW1NATIT  nel campo  APP-DATA-2 e muove  IW1SESTIT  nel campo  APP-SESS 
        ///Se il campo ICIMMF (MONTANTE MEDIO FITTIZIE 335 )  > 0 somma 60 nel campo  APP-ANNO-2 altrimenti controlla se il campo IW1CATPEN > 6 allora controlla se il campo APP-SESS 
        ///= "F" somma 60 al campo APP-ANNO-2 altrimenti somma 65 nel campo APP-ANNO-2   altrimenti  APP-SESS  = "F" somma 55 al campo APP-ANNO-2 altrimenti somma 60 nel campo APP-ANNO-2
        ///Se il campo IW1CODC = 2 allora sottrai 5 al campo  APP-ANNO-2.
        ///Muovi  30 nel campoAPP-GIORNO-2 ,muovi il campo  W-DEORIG-A nel campo APP-ANNO-1  e muovi W-DEORIG-M nel campo  APP-MESE-1  e muove  1 nel campo APP-GIORNO-1 
        ///Se il campo APP-DATA-2(1:6) > APP-DATA-1(1:6) chiama il programma “PCIPL94” usando i campi APP-DATA-1   APP-DATA-2  APP-DATA-RC altrimenti muove zero al campo APP-DATA-RC.
        ///Se il campo ICI2SETFIT > APP-DATA-RC  allora segnala errore "SETT. FITT. SUPERIORI A CAPIENZA TRA "  
        ///"DECORR. ED ETA' PENSIONABILE" (" APP-DATA-RC ")"   
        ///Se i campi (IW1NSOBG + IW1STOBG + SETT1(1) + SETT1(2) + SETT1(3) + SETT1(4)) > 2079 allora muove zero al campo APP-DATA-RC
        ///Se i campi  ICI2SETFIT = 0  AND APP-DATA-RC > 0 allora segnala errore "SETT. FITTIZIE MANCANTI"
        ///Se i campi  (W-DEORIG  > 199512 AND ICI2SETFIT > 0) AND (IW1RETOBG(RETRIBUZIONE MEDIA SETTIMANALE OBG)  = 0 AND ICIMMF = 0) AND (OPZIONE-CONTRIBUTIVA NOT = "S" AND IABCONA3 
        ///NOT = "O") allora segnala errore "RMS 503/92 O CMSM MANCANTI O INCOMPATIBILI CON SETT.FITTIZIE"   
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="codNatura"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="gruppo"></param>
        /// <param name="dataNascitaDC"></param>
        /// <param name="sessoDC"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="cmsm"></param>
        /// <param name="codiceCieco"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="settimaneEstere"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneFittizie(string codNatura, int? settimaneFittizie, string gruppo, DateTime? dataNascitaDC, DateTime? dataNascitaTitolare, decimal? cmsm, byte? codiceCieco, int? settimaneRetributiveQuotaA, int? settimaneRetributiveQuotaB, int? settimaneEstere, DateTime? decorrenza, decimal? rmsQuotaB, int? nSettimaneOBG, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<string> codiciNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "O" };
            DateTime? dataNascitaCompare = null;
            DateTime? dataCompare = null;
            int nSettimane = 0;

            if (!string.IsNullOrEmpty(codNatura) && !codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4"))
            {
                if (settimaneFittizie.GetValueOrDefault() > 0 && !codiciNatura.Contains(codNatura.Substring(2, 1)))
                {
                    messaggioVideo = "Settimane Fittizie non ammesse";
                    return false;
                }
            }
            else
            {
                if (gruppo.Equals("0003"))
                    dataNascitaCompare = dataNascitaDC;
                else
                    dataNascitaCompare = dataNascitaTitolare;

                dataNascitaCompare = dataNascitaCompare.Value.AddYears(60);

                if (codiceCieco.GetValueOrDefault() == 2)
                    dataNascitaCompare = dataNascitaCompare.Value.AddYears(-5);

                dataNascitaCompare = dataNascitaCompare.Value.AddDays(-dataNascitaCompare.Value.Day + 30);

                dataCompare = new DateTime(decorrenza.Value.Year, decorrenza.Value.Month, 01);

                if (Utility.DataStrettamenteSuccessivaA(new DateTime(dataNascitaCompare.Value.Year, dataNascitaCompare.Value.Month, 01), new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 01)))
                    nSettimane = Utility.NSettimaneBetweenDate(dataNascitaCompare.Value, dataCompare.Value);

                if (settimaneFittizie.GetValueOrDefault() > nSettimane)
                {
                    messaggioVideo = "Settimane Fittizie superiori a capienza tra Decorrenza ed eta' pensionabile";
                    return false;
                }

                if ((settimaneRetributiveQuotaA.GetValueOrDefault() + settimaneRetributiveQuotaB.GetValueOrDefault() + settimaneEstere.GetValueOrDefault()) > 2079)
                    nSettimane = 0;

                if (!(Utility.IsDomandaPensioneInabilita(datiPensione) && nSettimaneOBG.GetValueOrDefault() + settimaneEstere.GetValueOrDefault() > 2080) &&
                    settimaneFittizie.GetValueOrDefault() == 0 && nSettimane > 0)
                {
                    messaggioVideo = "Settimane Fittizie mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1NSOBG  = 0 allora controlla se i campi IW1VVMISURA > 0 AND STATO(1) = 17 continua altrimenti controlla se i campi TP1NUA > 0  AND IABREMSVV > 0 segnala 
        /// errore  "SETTIMANE OBG AL 12/92 INCOMPATIBILI  CON OBG DIRITTO E R.M.S."  
        /// </summary>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="codiceStato"></param>
        /// <param name="settimane"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneOBG(int? settimaneRetributiveQuotaA, int? vvMisuraAl1292, int codiceStato, int? settimane, decimal? rmsQuotaA, DateTime? dataInizioAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0)
            {
                if (!(vvMisuraAl1292.GetValueOrDefault() > 0 && codiceStato == 17))
                {
                    if (settimane.GetValueOrDefault() > 0 && rmsQuotaA.GetValueOrDefault() > 0)
                    {
                        //ENG - aggiunta condizione se dataInizioAssicurazione > 31/12/1992
                        if (dataInizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1992, 12, 31)))
                        {
                            messaggioVideo = "Settimane OBG al 12/92 incompatibili con OBG Diritto e R.M.S.";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Se il campo IW1VVMISURA > 0  allora controlla se  i campi  (W-DEORIG minore 197208 AND IW1DEOP = 0) OR  (IW1DEOP minore 197208 AND  > 0)  allora controlla se il campo 
        /// (IW1DIRET > 0 AND minore 197208) continua altrimenti segnala errore  "SETTIMANE VV PER MISURA INCOMPATIBILI CON DECORR. ANTE 07/72"
        /// Se i campi TP1NUB > 0 AND IABAR11VV > 0 allora segnala errore  "SETTIMANE VV PER MISURA INCOMPATIBILI CON ART. 11/488"
        /// Se il campo TP1NUB = 0   allora segnala "SETTIMANE VV PER MISURA INCOMPATIBILI CON VV PER DIRITTO" altrimenti  se i campi W-DEORIG > 197207 OR   IW1DEOP > 197207 allora 
        /// controlla se i campi TP1NUB > 0 AND IABAR11VV = 0 AND ICI1VVOBG = 0 AND ICISTOBG335 = 0 allora segnala errore STRING "SETTIMANE VV PER MISURA MANCANTI O INCOMPATIBILI 
        /// CON VV DIRITTO"   
        /// </summary>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="importoIVS"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneVV(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiContributivi, int? vvMisuraAl1292, DateTime? decorrenza, DateTime? decorrenzaOpzione, DateTime? decorrenzaDiretta, int? nContributiVolontari, decimal? importoIVS_art11, int? vvMisuraDL50392, int? settimaneContributiveQuotaC, decimal? rmsQuotaA, bool isValidoAlCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (vvMisuraAl1292.GetValueOrDefault() > 0)
            {
                if ((!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1972, 08, 01)) && !decorrenzaOpzione.HasValue) ||
                    (decorrenzaOpzione.HasValue && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 08, 01))))
                {
                    if (!(decorrenzaDiretta.HasValue && !Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(1972, 08, 01))))
                    {
                        messaggioVideo = "Settimane VV per Misura incompatibili con Decorr. ante 07/72";
                        return false;
                    }
                }

                if (nContributiVolontari.GetValueOrDefault() > 0 && importoIVS_art11.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con art. 11/488";
                    return false;
                }

                if (nContributiVolontari.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane VV per Misura incompatibili con VV per Diritto";
                    return false;
                }
            }
            else
            {
                //if (isValidoAlCalcolo)
                //{
                //    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                //        (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1972, 07, 31)) ||
                //        (decorrenzaOpzione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1972, 07, 31)))))
                //    {
                //        if ((lDatiContributivi == null || lDatiContributivi.Count == 0) && nContributiVolontari.GetValueOrDefault() > 0 && importoIVS_art11.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0)
                //        {
                //            messaggioVideo = "Settimane VV per misura mancanti o incompatibili con VV Diritto";
                //            return false;
                //        }
                //    }
                //}

                if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && (lDatiContributivi == null || lDatiContributivi.Count == 0) && nContributiVolontari.GetValueOrDefault() > 0 && importoIVS_art11.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0 && rmsQuotaA.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Settimane V.V. Misura mancanti (presenza di Sett. V.V. Diritto)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo I1SETIVS (TOTALE NUMERO CONTRIBUTI X VECCHIO CALCOLO CONTRIBUTIVO) = 0 allora controlla se il campo IW1IVSTOT > 0 segnala errore "SETTIMANE PER CALCOLO 
        /// CONTRIBUTIVO MANCANTI" altrimenti   se il campo IW1IVSTOT = 0 segnala errore "SETTIMANE PER CALCOLO CONTRIBUTIVO INCOMPATIBILI CON I.V.S."  
        /// 
        /// ATTENZIONE: il metodo differisce dal testo qui sopra poichè è stato adattato all'applicazione reingegnerizzata
        /// </summary>
        /// <param name="settimanePerCalcoloContributivo"></param>
        /// <param name="importoIVS"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimanePerCalcoloContributivoWithImportoIVS(int? settimanePerCalcoloContributivo, decimal? importoIVS, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimanePerCalcoloContributivo.GetValueOrDefault() == 0)
            {
                if (importoIVS.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Settimane per calcolo contributivo mancanti";
                    return false;
                }
            }
            else
            {
                if (Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1976, 08, 01)))
                {
                    messaggioVideo = "Settimane per calcolo contributivo incompatibili con decorrenza successiva al 07/1976.";
                    return false;
                }

                if (importoIVS.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane per calcolo contributivo incompatibili con I.V.S.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1IVSTOT (MPORTO IVS TOTALE CONTR)   >  0  allora controlla se i campi IW1CATPEN = 6 AND TP1CERTD(CERT. PENSIONE DIRETTA)  = 0 AND IW1DMOR minore 19680501 
        /// continua altrimenti controlla se il campo  W-DEORIG  >  197607 segnala errore  "I.V.S. AMMESSA SOLO PER DECORRENZE  ANTE 08/1976"  
        /// </summary>
        /// <param name="importoIVS"></param>
        /// <param name="categoria"></param>
        /// <param name="certificatoPensioneDiretta"></param>
        /// <param name="dataMorteDC"></param>
        /// <param name="decorrenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportoIVSPost1976(decimal? importoIVS, int categoria, int? certificatoPensioneDiretta, DateTime? dataMorteDC, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (importoIVS.GetValueOrDefault() > 0)
            {
                if (!(categoria == 6 && certificatoPensioneDiretta.GetValueOrDefault() == 0 && dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(1968, 05, 01))))
                {
                    if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1976, 07, 31)))
                    {
                        messaggioVideo = "I.V.S. ammessa solo per decorrenza ante 08/1976";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IABAR11VV (IMP.IVS ART.11 DEI VV )  > 0 allora controlla 
        /// * Se il campo IABREMSVV = 0   allora segnala errore STRING "IVS ART.11/488 INCOMPATIBILE CON R.M.S. AL 12/92"   
        /// * Se il campo W-DEORIG > 197606 allora segnala errore "IVS ART.11/488 INCOMPATIBILE CON  DECORRENZA ORIGINARIA" 
        /// * Se il campo TP1NUB   = 0  allora segnala errore "IVS ART.11/488 INCOMPATIBILE CON SETTIMANE VV DIRITTO"      
        /// * Se il campo IW1NSOBG = 0 allora segnala errore  "IVS ART.11/488 INCOMPATIBILE CON  SETTIMANE OBG MISURA"  
        /// * Se il campo IW1VVMISURA > 0 allora segnala errore "IVS ART.11/488 INCOMPATIBILE CON SETTIMANE V.V. MISURA"    
        /// </summary>
        /// <param name="importoIVS_art11"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportoIVSArt11(decimal? importoIVS_art11, decimal? rmsQuotaA, DateTime? decorrenza, int? nContributiVolontari, int? settimaneRetributiveQuotaA, int? vvMisuraAl1292, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (importoIVS_art11.GetValueOrDefault() > 0)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1976, 06, 30)))
                {
                    messaggioVideo = "IVS art.11/488 incompatibile con Decorrenza Originaria";
                    return false;
                }

                if (rmsQuotaA.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "IVS art.11/488 incompatibili con R.M.S. al 12/92";
                    return false;
                }

                if (nContributiVolontari.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "IVS art.11/488 incompatibili con Settimane VV Diritto";
                    return false;
                }

                if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "IVS art.11/488 incompatibile con Settimane OBG Misura";
                    return false;
                }

                if (vvMisuraAl1292.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "IVS art.11/488 incompatibile con Settimane V.V. Misura";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo (FINASS(1:6)  NOT minore W-DEORIG)  allora controlla se il campo IW1DOMOPZ  (DATA DOMANDA DELLA OPZIONE ) > 0 allora controlla s e il campo FINASS  NOT 
        /// minore IW1DOMOPZ allora segnala errore "DATA ULTIMO CONTRIBUTO POSTERIORE  DATA DOMANDA OPZIONE "  altrimenti segnala errore "DATA ULTIMO CONTRIBUTO POSTERIORE  A 
        /// DECORRENZA"        
        /// Se il campo  IW1DOMOPZ  >  0 e il campo  FINASS  NOT  minore  IW1DOMOPZ  allora segnala errore "DATA ULTIMO CONTRIBUTO POSTERIORE  DATA DOMANDA OPZIONE "      
        /// </summary>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="decorrenza"></param>
        /// <param name="dataDomandaOpzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaFineAssicurazioneWithDataDomandaOpzione(DateTime? dataFineAssicurazione, DateTime? decorrenza, DateTime? dataDomandaOpzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataFineAssicurazione.HasValue && Utility.DataSuccessivaA(new DateTime(dataFineAssicurazione.Value.Year, dataFineAssicurazione.Value.Month, 01), new DateTime(decorrenza.Value.Year, decorrenza.Value.Month, 01)))
            {
                if (dataDomandaOpzione.HasValue)
                {
                    if (Utility.DataSuccessivaA(new DateTime(dataFineAssicurazione.Value.Year, dataFineAssicurazione.Value.Month, 01), new DateTime(dataDomandaOpzione.Value.Year, dataDomandaOpzione.Value.Month, 01)))
                    {
                        messaggioVideo = "Data Ultimo Contributo posteriore Data Domanda Opzione";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Data Ultimo Contributo posteriore a Decorrenza";
                    return false;
                }
            }

            if (dataFineAssicurazione.HasValue)
            {
                if (dataDomandaOpzione.HasValue)
                {
                    if (Utility.DataSuccessivaA(new DateTime(dataFineAssicurazione.Value.Year, dataFineAssicurazione.Value.Month, 01), new DateTime(dataDomandaOpzione.Value.Year, dataDomandaOpzione.Value.Month, 01)))
                    {
                        messaggioVideo = "Data Ultimo Contributo posteriore Data Domanda Opzione";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  W-DEORIG minore 196805 AND  (IW1DEOP = 0  OR  IW1DOMOPZ = 0)  allora segnala errore "INCOMPATIBILITA' TRA R.M.S.8888 E  DATA DOMANDA OPZIONE (CNV01)"         
        /// </summary>
        /// <param name="rms8888"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="dataDomandaOpzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMS8888WithOpzione(decimal? rms8888, DateTime? decorrenza, DateTime? decorrenzaOpzione, DateTime? dataDomandaOpzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rms8888.GetValueOrDefault() > 0)
            {
                if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1968, 05, 01)) && (!decorrenzaOpzione.HasValue || !dataDomandaOpzione.HasValue))
                {
                    messaggioVideo = "Incompatibilita' tra R.M.S.8888 e Data Domanda Opzione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  W-DEORIG > 195200 AND APPO-CAT1 = "I" AND APP-DIR < 260 AND IREQPARD NOT = 2  allora segnala errore   "SETTIMANE INFERIORI A 260 (CATEGORIA " APPO-CAT ")"
        /// </summary>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="gruppo"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="nSettGodimentoAssegno"></param>
        /// <param name="sommaSettimaneEstereDiritto"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneWithDecPensioneAndCodRequisitiParticolari(DateTime? decorrenzaPensione, string gruppo, string siglaCategoria, int? settimane, int? nContributiVolontari,
            int? nSettGodimentoAssegno, int? sommaSettimaneEstereDiritto, byte? codiceRequisitiParticolari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            DateTime dataCompare = new DateTime(1951, 12, 31);
            int? sommaSettimane = settimane.GetValueOrDefault() + nContributiVolontari.GetValueOrDefault() + nSettGodimentoAssegno.GetValueOrDefault() + sommaSettimaneEstereDiritto.GetValueOrDefault();

            if (decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, dataCompare) && gruppo.Equals("0002") &&
                sommaSettimane.GetValueOrDefault() < 260 && codiceRequisitiParticolari.GetValueOrDefault() != 2)
            {
                messaggioVideo = "Settimane inferiori a 260 (Categoria '" + siglaCategoria.Trim() + "')";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se i campi IABNSASS > 0 AND  ( (APPO-CAT1 NOT = "V" OR  IREQPARD NOT = 7) ) allora controlla se i  campi APPO-CAT1 = "S" continua altrimenti segnala errore  "SETT.GODIM.ASSEGNO INCOMPATIBILI CON  CATEGORIA / REQ.PART.DIRITTO)"              
        /// </summary>
        /// <param name="tipoDomanda"></param>
        /// <param name="gruppo"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="nSettGodimentoAssegno"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettGodimentoAssegnoAndCodReqParticolari(Utility.TipoDomanda tipoDomanda, string gruppo, byte? codiceRequisitiParticolari, int? nSettGodimentoAssegno, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (nSettGodimentoAssegno.GetValueOrDefault() > 0 && (!gruppo.Equals("0001") || codiceRequisitiParticolari.GetValueOrDefault() != 7) && tipoDomanda != Utility.TipoDomanda.Superstiti && !datiPensione.SiglaCategoria.StartsWith("I"))
            {
                messaggioVideo = "Settimane godimento assegno incompatibili con categoria / codice requisito particolare diritto";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se i campi  (IABCONA2 = "1" OR "2") AND (APPO-CAT1 = "V" OR "S")  AND APP-DIR < 1820 AND IREQPARD NOT = 2  AND IABCONA4 NOT = "Z" AND (IABCONA4 NOT = "J" AND "K" AND "Q" AND "W" AND "X" AND "Y" AND "P" AND "L" AND "O")  allora controlla se  il campo (APP-CAL-ITA + APP-DIR-EST) NOT = 2080  allora segnala  errrore "SETTIMANE INFERIORI A 1820 ("APPO-CAT "- ANZIANITA')"      
        /// </summary>
        /// <param name="tipoDomanda"></param>
        /// <param name="gruppo"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="sommaSettimaneEstereDiritto"></param>
        /// <param name="nSettGodimentoAssegno"></param>
        /// <param name="sommaSettimaneItaliane"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimane(Utility.TipoDomanda tipoDomanda, string gruppo, string siglaCategoria, string naturaPensione, byte? codiceRequisitiParticolari, int? settimane,
            int? settimaneVVDiritto, int? settimaneEstere, int? nSettGodimentoAssegno, int? settimaneItaliane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<string> codNaturaList = new List<string> { "Z", "J", "K", "Q", "W", "X", "Y", "P", "L", "O" };
            int? sommaSettimane = settimane.GetValueOrDefault() + settimaneVVDiritto.GetValueOrDefault() + nSettGodimentoAssegno.GetValueOrDefault() +
                settimaneEstere.GetValueOrDefault();

            if (!string.IsNullOrEmpty(naturaPensione) && (naturaPensione.Substring(0, 1).Equals("1") || naturaPensione.Substring(0, 1).Equals("2")) && (gruppo.Equals("0001") ||
                tipoDomanda == Utility.TipoDomanda.Superstiti) && sommaSettimane.GetValueOrDefault() < 1820 && !codNaturaList.Contains(naturaPensione.Substring(2, 1)))
            {
                if ((settimaneItaliane.GetValueOrDefault() + settimaneEstere.GetValueOrDefault()) != 2080)
                {
                    messaggioVideo = "Settimane inferiori a 1820 ('" + siglaCategoria.Trim() + " - Anzianità')";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi   APPO-CAT1 = "S" AND IABCONA2 NOT = "1"  AND APP-DIR < 260 AND IREQPARD NOT = 2  AND W-DEORIG > 195200  allora controlla se I campi   IW1SESEZ = 7700 AND IW1CERT  = 277744 AND IW1CATPEN = 6   (if con comment PIERA - 21.10.2003 PENSIONE 7700/006/00277744 MORTO PRIMA DEL 1952, MA DECORRENZA SUCCESSIVA PERCHE 'aSEGUITO DI LEGGE SUCCESSIVA: --   N O   E R R O R E   -) continua altrimenti segnala errore "SETTIMANE INFERIORI A 260 ("APPO-CAT ")"
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="sommaSettimaneEstereDiritto"></param>
        /// <param name="nSettGodimentoAssegno"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneWithCodiceSedeAndCertificato(GestionePensione.DatiPensione datiPensione, byte? codiceRequisitiParticolari, int? settimane, int? nContributiVolontari,
            int? sommaSettimaneEstereDiritto, int? nSettGodimentoAssegno, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1951, 12, 31);

            int? sommaSettimane = settimane.GetValueOrDefault() + nContributiVolontari.GetValueOrDefault() + nSettGodimentoAssegno.GetValueOrDefault() + sommaSettimaneEstereDiritto.GetValueOrDefault();

            if (datiPensione == null)
            {
                messaggioVideo = "Dati Pensione mancanti";
                return false;
            }

            string categoriaNumerica = datiPensione.GetCodCategoria();

            if (datiPensione.Gruppo.Equals("0001") && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && !datiPensione.NaturaPensione.Substring(0, 1).Equals("1") && sommaSettimane.GetValueOrDefault() < 260 &&
                codiceRequisitiParticolari.GetValueOrDefault() != 2 && datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare))
            {
                if (!(datiPensione.CodiceSede == 7700 && datiPensione.NCertificato.GetValueOrDefault() == 277744 && categoriaNumerica.Equals("6")))
                {
                    messaggioVideo = "Settimane inferiori a 260 ('" + datiPensione.SiglaCategoria.Trim() + "')";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi   IF  TP1NUB = 0 AND (IREQPARD = 7 AND TP1ATEC  = 98 AND TP1PRIN = 176) allora segnala errore "SE 'L.S.U' DEFINITIVA, DEBBONO ESSERE PRESENTI  CTR V.V. PER DIRITTO"  
        /// </summary>
        /// <param name="nContributiVolonatri"></param>
        /// <param name="attivitaEconomica"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsCodReqParticolareAndProfIndivAndAttEconAndNumContribVolontari(int? nContributiVolontari, int? attivitaEconomica, byte? codiceRequisitiParticolari, int? professioneIndividuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (nContributiVolontari.GetValueOrDefault() == 0 && codiceRequisitiParticolari.GetValueOrDefault() == 7 && attivitaEconomica.GetValueOrDefault() == 98 && professioneIndividuale.GetValueOrDefault() == 176)
            {
                messaggioVideo = "Se 'L.S.U' definitiva, debbono essere presenti CTR V.V. per diritto";
                return false;
            }

            return true;
        }

        #endregion PCIPL40

        #region PCIPL39

        /// <summary>
        /// Se IW1NSAUT è uguale a zero valorizzare con "06" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "SETTIMANE ITALIANE MISURA MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="settimaneItalianeMisura"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneDatiCalcolo(int? settimaneDatiCalcolo, DateTime? decorrenzaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((decorrenzaPensione.HasValue && Utility.DataSuccessivaA((DateTime)decorrenzaPensione, new DateTime(1993, 1, 1))) && (!settimaneDatiCalcolo.HasValue || settimaneDatiCalcolo.Value == 0))
            {
                messaggioVideo = "La somma delle settimane deve essere maggiore di zero";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se TP1NUA è uguale a zero effettuare le seguenti operazioni:  
        /// Se ICI2CONV non è uguale a 17 e 20  valorizzare con "07" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETTIMANE ITALIANE DIRITTO MANCANTI" il campo MESSAGGIO-ERRORE
        /// </summary>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneItalianeDiritto(int? settimaneItalianeDiritto, byte? codiceConvenzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!settimaneItalianeDiritto.HasValue || settimaneItalianeDiritto.Value == 0)
            {
                if (codiceConvenzione.HasValue && codiceConvenzione.Value != 17 && codiceConvenzione.Value != 20)
                {
                    messaggioVideo = "Settimane italiane diritto mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se  IW1DEOP è maggiore di zero e ((ICI2CONV è uguale a "13"  e STATO(1)  è uguale a 38)  oppure (ICI2CONV  è uguale a  "38") ) effettuare le seguenti operazioni : 
        /// Se TP1NUA  è inferiore a 52 valorizzare con "07" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "DOMANDA OPZIONE(CNV01) INCOMPATIBILE CON -52 CTR" il campo MESSAGGIO-ERRORE, con 1 il campo       FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="codiceStato"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaOpzioneWithCodiceStato(DateTime? decorrenzaOpzione, int codiceStato, byte? codiceConvenzione, int? settimane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaOpzione.HasValue && codiceConvenzione.HasValue && ((codiceConvenzione.Value == 13 && codiceStato == 38) || codiceConvenzione.Value == 38))
            {
                if (settimane.HasValue && settimane.Value < 52)
                {
                    messaggioVideo = "Domanda opzione incompatibilie con settimane";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se ICI2CONV non è uguale a 12 oppure  (ICI2CONV  è uguale a 12 e W-DEORIG è maggiore di 197209)  effettuare le seguenti operazioni : 
        /// Se (TP1NUA   +   IABNSASS) è inferiore a MIN-CTR e IW1CODOPZ non è uguale a "7"  effettuare le seguenti operazioni :                                                         
        /// Se APPO-CAT1 non è uguale a "S" valorizzare con "09" il campo TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETTIMANE ITALIANE INFERIORI A " + MIN-CTR + “ PER CONVENZ. " + ICI2CONV il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni :                                                  
        /// Se  (W-DEORIG è maggiore di 197209 e ICI2RESEST = "I       " )  e (ICI2CONV non è uguale a 38 e 39) continuare l’elaborazione al punto successivo (41); 
        ///	Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni:  
        ///	Se IW1DIRET è maggiore di zero e IW3DESUP(1) è maggiore di zero  continuare l’elaborazione al punto successivo (42);                            
        ///	Diversamente da quanto analizzato nel punto precedente valorizzare con "10" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETTIMANE ITALIANE INFERIORI A " + MIN-CTR + " PER CONVENZ. " +  ICI2CONV il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaSupplemento"></param>
        /// <param name="numeroSettimane"></param>
        /// <param name="numeroSettimaneGodimentoAssegno"></param>
        /// <param name="ctrMinimi"></param>
        /// <param name="codiceOpzione"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneItalianeWithCodiceConvenzione(string gruppo, byte? codiceConvenzione, DateTime? decorrenza, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta,
            DateTime? decorrenzaSupplemento, int numeroSettimane, int? numeroSettimaneGodimentoAssegno, int ctrMinimi, short? codiceOpzione, string codiceComuneResidenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1972, 09, 01);

            if (codiceConvenzione.HasValue && (codiceConvenzione.Value != 12 || (codiceConvenzione.Value == 12 && decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, dataCompare))))
            {
                if ((numeroSettimane + numeroSettimaneGodimentoAssegno.GetValueOrDefault()) < ctrMinimi && codiceOpzione.GetValueOrDefault() != 7)
                {
                    if (!string.IsNullOrEmpty(gruppo) && !gruppo.Equals("0003"))
                    {
                        messaggioVideo = "Settimane italiane inferiori a " + ctrMinimi + " per convenzione " + codiceConvenzione.Value;
                        return false;
                    }
                    else
                    {
                        if (!(Utility.DataStrettamenteSuccessivaA(decorrenza.Value, dataCompare) && !string.IsNullOrEmpty(codiceComuneResidenza) && codiceComuneResidenza.ToUpperInvariant().Trim() == "I" &&
                            codiceConvenzione.Value != 38 && codiceConvenzione.Value != 39))
                        {
                            if (!(decorrenzaDiretta.HasValue && decorrenzaSupplemento.HasValue))
                            {
                                messaggioVideo = "Settimane italiane inferiori a " + ctrMinimi + " per convenzione " + codiceConvenzione.Value;
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se   (ICI2CONV  è uguale a 33  e  (IABCONA2  è uguale a "1" oppure a "2") ) effettuare le seguenti operazioni :                                                              
        /// Se TP1NUA è inferiore a 780 e IW1NSAUT è inferiore a 780  e (IABCONA4 non è uguale a "Z" )  valorizzare con "11" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETT.ITALIANE INFERIORI A 780 (CONVEN è uguale a 33 E NATURA è uguale a 1)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimane"></param>
        /// <param name="settimaneItalianeMisura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneItaliane(byte? codiceConvenzione, string naturaPensione, int? settimane, int settimaneItalianeMisura, string tipoSettimaneBeneficio, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 33 && !string.IsNullOrEmpty(naturaPensione) && (naturaPensione.Substring(0, 1).Equals("1") || naturaPensione.Substring(0, 1).Equals("2")))
            {
                int limitValue = 780;
                if (!String.IsNullOrEmpty(tipoSettimaneBeneficio) && !String.IsNullOrEmpty(tipoSettimaneBeneficio.Trim()) && tipoSettimaneBeneficio == "01")
                    limitValue = 520;

                if (settimane.HasValue && settimane.Value < limitValue && settimaneItalianeMisura < 780 && !naturaPensione.Substring(2, 1).Equals("Z"))
                {
                    messaggioVideo = "Settimane italiane inferiori a " + limitValue;

                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Se IW1FFAA è uguale a zero valorizzare con "16" il campo  TIPO-ERRORE, con 05 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "NUMERO SETTIMANE EFFETTIVE MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="numeroContributiItalia"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneEffettive(int? numeroContributiItalia, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!numeroContributiItalia.HasValue || numeroContributiItalia.Value == 0)
            {
                messaggioVideo = "Numero settimane effettive mancanti";
                return false;
            }

            return true;
        }


        public static bool ControlsSettimaneFittizieWithEtaPensionabileAndDecorrenza(string gruppo, string naturaPensione, int? numSettFittiziePrepensionamento, DateTime? decorrenzaPensione, DateTime? decorrenza,
            DateTime? dataNascitaTitolare, DateTime? dataNascitaDC, decimal? montante, byte? codiceCieco, GestionePensione.DatiPensione datiPensione, int? settimaneEstere, int? settimaneItalianeDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime datacompare = new DateTime(1984, 06, 01);
            int settimaneFittizie = 0;

            if (decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, datacompare) &&
                (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(0, 1).Equals("3") || naturaPensione.Substring(0, 1).Equals("4")))
            {
                if (!string.IsNullOrEmpty(gruppo) && gruppo.Trim().Equals("0002") || gruppo.Trim().Equals("0003")) //I o S
                {
                    settimaneFittizie = CalcolaSettimaneFittizie(gruppo, decorrenza, dataNascitaDC, dataNascitaTitolare, montante, codiceCieco);

                    if (!datiPensione.SiglaCategoria.StartsWith("I"))
                    {
                        if (numSettFittiziePrepensionamento.HasValue && numSettFittiziePrepensionamento.Value > settimaneFittizie)
                        {
                            messaggioVideo = "Settimane fittizie superiori a capienza tra età pensionabile e decorrenza (" + settimaneFittizie + ")";
                            return false;
                        }
                    }

                    if (!(Utility.IsDomandaPensioneInabilita(datiPensione) && settimaneItalianeDiritto.GetValueOrDefault() + settimaneEstere.GetValueOrDefault() > 2080) &&
                        (!numSettFittiziePrepensionamento.HasValue || numSettFittiziePrepensionamento.Value == 0) && settimaneFittizie > 0)
                    {
                        messaggioVideo = "Settimane fittizie mancanti";
                        return false;
                    }
                }
            }
            else
            {
                if (!VerificaSettimaneFittizieWithCodNatura(naturaPensione, numSettFittiziePrepensionamento, out messaggioVideo))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Se (IABCONA4 è uguale a "J" oppure "K" oppure "Q" oppure "W" oppure "X" oppure "Y" oppure "P" oppure "O")  continuare l’elaborazione (54) al punto successivo diversamente se ICI2SETFIT è maggiore di zero valorizzare con "22" il campo TIPO-ERRORE , con 05 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETTIMANE FITTIZIE NON AMMESSE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195); 
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="numSettFittiziePrepensionamento"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool VerificaSettimaneFittizieWithCodNatura(string naturaPensione, int? numSettFittiziePrepensionamento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!(!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(2, 1).Equals("J") || naturaPensione.Substring(2, 1).Equals("K") || naturaPensione.Substring(2, 1).Equals("Q") ||
                naturaPensione.Substring(2, 1).Equals("W") || naturaPensione.Substring(2, 1).Equals("X") || naturaPensione.Substring(2, 1).Equals("Y") || naturaPensione.Substring(2, 1).Equals("P") ||
                naturaPensione.Substring(2, 1).Equals("O")))
            {
                if (numSettFittiziePrepensionamento.HasValue && numSettFittiziePrepensionamento.Value > 0)
                {
                    messaggioVideo = "Settimane fittizie non ammesse";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se W-DEORIG è inferiore a  199601 e  IW1IVSTOT è uguale a zero valorizzare con "13" il campo  TIPO-ERRORE, con 04 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "IMPORTO I.V.S. MANCANTE: DECORRENZA ANTE 1996  " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);

        /// Se W-DEORIG è maggiore di199512 e IW1IVSTOT è maggiore di zero valorizzare con "14" il campo  TIPO-ERRORE, con 04 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "IMPORTO I.V.S. ERRATO: NON VA ACQUISITO PER DECORRENZA POST 1995" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="importoIVS"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportoIVS(decimal? importoIVS, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1996, 01, 01);

            if (decorrenza.HasValue && !Utility.DataSuccessivaA(decorrenza.Value, dataCompare) && importoIVS.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Importo I.V.S. mancante: decorrenza ante 1996";
                return false;
            }

            if (decorrenza.HasValue && Utility.DataSuccessivaA(decorrenza.Value, dataCompare) && importoIVS.GetValueOrDefault() > 0)
            {
                messaggioVideo = "Importo I.V.S. errato: non va acquisito per decorrenza post 1995";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se IDEL126 è uguale a "S" e IREQPARD non è uguale a 2  effettuare le seguenti operazioni :  
        /// Se (IW1NSAUT è inferiore a 780 e TP1NUA è inferiore a 780)   e (IABCONA4 non è uguale a "Z" )  valorizzare con "12" il campo  TIPO-ERRORE, con 03 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "CONTRIBUTI ITALIANI INCOMPATIBILI CON DEL.126/88 (CNV01)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// </summary>
        /// <param name="deliberaCee126"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="settimaneItaliane"></param>
        /// <param name="settimane"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettWithCodReqPartAndNaturaPensione(bool? deliberaCee126, byte? codiceRequisitiParticolari, int? settimane, string naturaPensione, string tipoSettimaneBeneficio,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (deliberaCee126.GetValueOrDefault() && codiceRequisitiParticolari.GetValueOrDefault() != 2)
            {
                int limitValue = 780;
                if (!String.IsNullOrEmpty(tipoSettimaneBeneficio) && !String.IsNullOrEmpty(tipoSettimaneBeneficio.Trim()) && tipoSettimaneBeneficio == "01")
                    limitValue = 520;

                if (settimane.GetValueOrDefault() < limitValue && !string.IsNullOrEmpty(naturaPensione) && !naturaPensione.Substring(2, 1).Equals("Z"))
                {
                    messaggioVideo = "Contributi italiani incompatibili con delibera 126/88";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 46.	Se IABNSASS è maggiore di zero e (APPO-CAT1 non è uguale a "V" oppure  IREQPARD non è uguale a 7) effettuare le seguenti operazioni : 
        /// ******* inserito il 14.1.2009 (appunto 2/2009 Rosalia Mariani)    
        /// 46.1.	Se APPO-CAT1 è uguale a "S" 
        /// **** tolto controllo si indiretta  (rosalia mail del 6.7.2011     
        /// **       e TP1CATD = 0                                          
        /// continuare l’elaborazione al punto successivo  diversamente valorizzare con "15" il campo  TIPO-ERRORE, con 04 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETT.GODIM.ASSEGNO INCOMPATIBILI CON CATEGORIA / REQ.PART.DIRITTO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// </summary>
        /// <param name="settimaneGodimentoAssegno"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="gruppo"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettGodimentoAssegnoWithCodReqParticolari(int? settimaneGodimentoAssegno, byte? codiceRequisitiParticolari, string gruppo, Utility.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneGodimentoAssegno.GetValueOrDefault() > 0 && (!gruppo.Equals("0001") || codiceRequisitiParticolari.GetValueOrDefault() != 7))
            {
                if (tipoDomanda != Utility.TipoDomanda.Superstiti && !datiPensione.SiglaCategoria.StartsWith("I"))
                {
                    messaggioVideo = "Settimane godimento assegno incompatibili con categoria / req. part. diritto";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se APPO-CAT1 è uguale a "I" e APP-APP è inferiore a 260  e IREQPARD non è uguale a 2  valorizzare con "51" il campo  TIPO-ERRORE, con  4 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "INVALIDITA' : SETTIMANE INFERIORI A 260" il campo MESSAGGIO-ERRORE, 
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="sommaSettimaneDirittoEstere"></param>
        /// <param name="settimaneGodimentoAssegno"></param>
        /// <param name="settimane"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneWithCodReqParticolari(string gruppo, int? sommaSettimaneDirittoEstere, int? settimaneGodimentoAssegno, int? settimane, byte? codiceRequisitiParticolari, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int? settimaneTotali = sommaSettimaneDirittoEstere.GetValueOrDefault() + settimaneGodimentoAssegno.GetValueOrDefault() + settimane.GetValueOrDefault();

            if (gruppo.Equals("0002") && settimaneTotali.GetValueOrDefault() < 260 && codiceRequisitiParticolari.GetValueOrDefault() != 2)
            {
                messaggioVideo = "Invalidità: settimane inferiori a 260";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se (APPO-CAT1 è uguale a "S" e (APP-APP  +   ICI2SETFIT)  è inferiore a 260)  e (IABCONA2 non è uguale a "1" e "2")  e IREQPARD non è uguale a 2 valorizzare con "55" il campo      TIPO-ERRORE, con  4 il campo RIG-ERRORE, con 69 il campo COL-ERR1, con "SETTIMANE INFERIORI A 260" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// </summary>
        /// <param name="tipoDomanda"></param>
        /// <param name="sommaSettimaneDirittoEstere"></param>
        /// <param name="settimaneGodimentoAssegno"></param>
        /// <param name="settimane"></param>
        /// <param name="codiceRequisitiParticolari"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneWithCodReqParticolariAndTipoDomanda(Utility.TipoDomanda tipoDomanda, int? sommaSettimaneDirittoEstere, int? settimaneGodimentoAssegno, int? settimane, byte? codiceRequisitiParticolari, int? settimaneFittizie, string naturaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int? settimaneTotali = sommaSettimaneDirittoEstere.GetValueOrDefault() + settimaneGodimentoAssegno.GetValueOrDefault() + settimane.GetValueOrDefault() + settimaneFittizie.GetValueOrDefault();

            if (tipoDomanda == Utility.TipoDomanda.Superstiti && settimaneTotali.GetValueOrDefault() < 260 && !string.IsNullOrEmpty(naturaPensione) && !naturaPensione.Substring(0, 1).Equals("1") && !naturaPensione.Substring(0, 1).Equals("2") && codiceRequisitiParticolari.GetValueOrDefault() != 2)
            {
                messaggioVideo = "Settimane inferiori a 260";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 50.	Se IW1FFAA è maggiore di TP1NUA e (DAFELPE-DATA non NUMERIC oppure DAFELPE-DATA è inferiore a  '20110101' )   effettuare le seguenti operazioni :  
        /// 50.1.	Se APPO-CAT1 non è uguale a "S" valorizzare con "17" il campo  TIPO-ERRORE, con 05 il campo RIG-ERRORE, con 29 il campo COL-ERR1, con "NUMERO SETTIMANE EFFETTIVE 
        /// SUPERIORI A  SETTIMANE DIRITTO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 50.2.	Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni : 
        /// 50.2.1.	Se IW1DIRET è maggiore di zero e IW3DESUP(1) è maggiore di zero continuare l’elaborazione al punto successivo (51); 
        /// 50.2.2.	Diversamente valorizzare con "18" il campo  TIPO-ERRORE, con 05 il campo            RIG-ERRORE, con 29 il campo COL-ERR1, con "NUMERO SETTIMANE EFFETTIVE 
        /// SUPERIORI A  SETTIMANE DIRITTO"  il campo  MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// </summary>
        /// <param name="nContributiItalia"></param>
        /// <param name="settimane"></param>
        /// <param name="dataCalcolo"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="decorrenzaPensioneDiretta"></param>
        /// <param name="primaDecorrenzaSupplementi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMaggiori6(int? nContributiItalia, int? settimane, DateTime? dataCalcolo, Utility.TipoDomanda tipoDomanda,
            DateTime? decorrenzaPensioneDiretta, DateTime? primaDecorrenzaSupplementi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (nContributiItalia.GetValueOrDefault() > settimane.GetValueOrDefault() && dataCalcolo.HasValue && !Utility.DataSuccessivaA(dataCalcolo.Value, new DateTime(2011, 01, 01)))
            {
                if (tipoDomanda != Utility.TipoDomanda.Superstiti)
                {
                    messaggioVideo = "Numero Settimane Effettive superiori a Settimane Diritto";
                    return false;
                }
                else
                {
                    if (!(decorrenzaPensioneDiretta.HasValue && primaDecorrenzaSupplementi.HasValue))
                    {
                        messaggioVideo = "Numero Settimane Effettive superiori a Settimane Diritto";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo TP1NUA > IW1VVMISURA (CONTRIBUTI V.V. OBG PER MISURA) allora muovi il campo TP1NUA nel campo di appoggio  APP-APP   altrimenti  muove il campo IW1VVMISURA   nel campo di appoggio  APP-APP.
        /// Se il campo IW1FFAA  >  APP-APP  (N. SETT. EFF. DI CONTRIBUZIONE )  AND (DAFELPE-DATA (Dati prelievo da FELPE)  NOT  NUMERIC OR DAFELPE-DATA  <  '20110101' ) allora controlla se il campo  APPO-CAT1 NOT = "S"   AND (STATO(1) NOT = 17 AND 20)  allora segnala errore "SETTIMANE EFFETTIVE SUPERIORI A SETTIMANE OBG DIRITTO"              
        /// Se il campo  APPO-CAT1 = "S"  AND (TP1CERTD = 0 OR IW3DESUP(1) = 0) AND (STATO(1) NOT = 17 AND 20)    allora segnala errore "SETTIMANE EFFETTIVE SUPERIORI A SETTIMANE OBG DIRITTO" 
        /// </summary>
        /// <param name="nContributiItalia"></param>
        /// <param name="settimane"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="dataCalcolo"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="primaDecorrenzaSupplementi"></param>
        /// <param name="codicePrimoStatoEE"></param>
        /// <param name="certificatoPensioneDiretta"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneEffettiveWithSettimaneDirittoPerCategorieMinori7(int? nContributiItalia, int? settimane, int? vvMisuraAl1292, DateTime? dataCalcolo,
            Utility.TipoDomanda tipoDomanda, DateTime? primaDecorrenzaSupplementi, int? codicePrimoStatoEE, int? certificatoPensioneDiretta, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int appSettimane = 0;

            if (settimane.GetValueOrDefault() > vvMisuraAl1292.GetValueOrDefault())
                appSettimane = settimane.GetValueOrDefault();
            else
                appSettimane = vvMisuraAl1292.GetValueOrDefault();


            if (nContributiItalia.GetValueOrDefault() > appSettimane && dataCalcolo.HasValue && !Utility.DataSuccessivaA(dataCalcolo.Value, new DateTime(2011, 01, 01)))
            {
                if (tipoDomanda != Utility.TipoDomanda.Superstiti && codicePrimoStatoEE.GetValueOrDefault() != 17 && codicePrimoStatoEE.GetValueOrDefault() != 20)
                {
                    messaggioVideo = "Settimane Effettive superiori a Settimane OBG Diritto";
                    return false;
                }

                if (tipoDomanda == Utility.TipoDomanda.Superstiti && (certificatoPensioneDiretta.GetValueOrDefault() == 0 || !primaDecorrenzaSupplementi.HasValue) &&
                    codicePrimoStatoEE.GetValueOrDefault() != 17 && codicePrimoStatoEE.GetValueOrDefault() != 20)
                {
                    messaggioVideo = "Settimane Effettive superiori a Settimane OBG Diritto";
                    return false;
                }

            }

            return true;
        }

        #endregion PCIPL39

        #region PCIPL11
        /// <summary>
        /// 74.	Se (TP1NUA + TP1NUB + ICI2SETFIT + IABNSASS ) è minore di 52 e (APPO-CAT1 è uguale a "S" e TP1CERTD è uguale a 0 ) effettuare le seguenti operazioni : 
        /// 74.1.	Se ( ICI2CONV è uguale a 12 e IW1DEORIG è maggiore di 197209) effettuare le seguenti operazioni : 
        /// 74.1.1.	Se PRESENZA-ORFANO non è uguale a "S" valorizzare con "36" il campo TIPO-ERRORE, con "ERRORE PANN. CNV02/03: NUMERO CONTRIBUTI INFERIORI A 52" il campo 
        /// MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-2 (78);
        /// </summary>
        /// <param name="settimane"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="settimaneGodimentoAssegno"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="certificatoDC"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="presenzaOrfano"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiWithOrfano(int? settimane, int? settimaneFittizie, int? settimaneGodimentoAssegno, Utility.TipoDomanda tipoDomanda, int? certificatoDC, byte? codiceConvenzione,
            DateTime? decorrenzaOriginaria, bool presenzaOrfano, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimane.GetValueOrDefault() + settimaneFittizie.GetValueOrDefault() + settimaneGodimentoAssegno.GetValueOrDefault() < 52 && tipoDomanda == Utility.TipoDomanda.Superstiti &&
                certificatoDC.GetValueOrDefault() == 0)
            {
                if (codiceConvenzione.GetValueOrDefault() == 12 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1972, 09, 30)))
                {
                    if (!presenzaOrfano)
                    {
                        messaggioVideo = "Numero Contributi inferiori a 52";
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion PCIPL11

        /* ENG - 05/11/2024 Deprecata
        public static bool ControlsLimiteSettimaneReversibilitaSloveniaCroazia(GestionePensione.DatiPensione datiPensione, int? settimaneItalianeDiritto, int? nSettimaneOBG, int? nContributiUtiliLavoratoriAutonomi,
            int? nContributiVolontari, byte? codiceConvenzione, int codiceStatoEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            // 2.	Pensioni di reversibilità in convenzione
            //  a.	in convenzione 12 con stato 38 (Slovenia) o con stato 39 (Croazia)
            //  b.	in convenzione 38
            //  c.	in convenzione 39
            if (Utility.IsDomandaReversibilita(datiPensione) && (codiceConvenzione.GetValueOrDefault() == 38 || codiceConvenzione.GetValueOrDefault() == 39 ||
                (codiceConvenzione.GetValueOrDefault() == 12 && (codiceStatoEE == 38 || codiceStatoEE == 39))))
            {
                int settimaneToCompare = 0;
                if (settimaneItalianeDiritto.GetValueOrDefault() > 0)
                    settimaneToCompare = settimaneItalianeDiritto.GetValueOrDefault();
                else if (nSettimaneOBG.GetValueOrDefault() > 0)
                    settimaneToCompare = nSettimaneOBG.GetValueOrDefault();
                else if (nContributiUtiliLavoratoriAutonomi.GetValueOrDefault() > 0)
                    settimaneToCompare = nContributiUtiliLavoratoriAutonomi.GetValueOrDefault();

                if (settimaneToCompare + nContributiVolontari.GetValueOrDefault() >= 52)
                {
                    messaggioVideo = "La somma tra le Settimane OBG diritto/Settimane ital. Diritto e Settimane VV diritto deve essere inferiore strettamente a 52.";
                    return false;
                }
            }

            return true;
        }
*/

        //public static bool ControlsNSettimanePerAPEPrecoci(GestionePensione.DatiPensione datiPensione, int? nSettimaneOBG, int? nContributiVolontari,
        //    List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, out string messaggioVideo)
        //{
        //    messaggioVideo = string.Empty;
        //    int limiteSettimane = 2132;

        //    if (Utility.IsDomandaAPEPrecoci(datiPensione))
        //    {
        //        int numSettimaneTipoContibutivo = 0;
        //        numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + nSettimaneOBG.GetValueOrDefault(); //SETTIMANE OBG DIRITTO
        //        numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + nContributiVolontari.GetValueOrDefault(); //SETTIMANE VV DIRITTO
        //        numSettimaneTipoContibutivo = numSettimaneTipoContibutivo + listaPrestazioniEstere.Sum(x => x.ContributiEEDiritto.GetValueOrDefault());  //SETTIMANE DIRITTO

        //        if (numSettimaneTipoContibutivo < limiteSettimane)
        //        {
        //            messaggioVideo = string.Format("Il numero delle settimane deve essere maggiore o uguale a {0}", limiteSettimane);
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public static bool ControlsNSettimanePerQuota100(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaQuota100(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                if (nSettimane < 1976)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1976 (38 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerSperimentaleDonna_DL_4_2019(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
           GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                if (nSettimane < 1820)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1820 (35 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerOpzioneDonna_Legge197_2022_Art1_Comma292(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
          GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione, true, true) || Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione, true, true) ||
                Utility.IsOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione, true, true) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA(datiPensione)
                || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB(datiPensione) || Utility.IsRicOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraC(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                if (nSettimane < 1820)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1820 (35 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerAnzianitaPerLeggeBilancio2019(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
           GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, char? sessoTitolare,
           out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                switch (sessoTitolare)
                {
                    case 'M':
                        if (nSettimane < 2227)
                        {
                            messaggioVideo = "Il numero settimane non può essere inferiore a 2227 (42 anni e 10 mesi di contribuzione)";
                            return false;
                        }
                        break;
                    case 'F':
                        if (nSettimane < 2175)
                        {
                            messaggioVideo = "Il numero settimane non può essere inferiore a 2175 (41 anni e 10 mesi di contribuzione)";
                            return false;
                        }
                        break;
                    default:
                        messaggioVideo = "Sesso del titolare non presente nell'anagrafica";
                        return false;
                }
            }

            return true;
        }

        public static bool VerificaRiduzioneRetributiva(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi,
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiCi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && !Utility.GestioneRiduzioneRetributivaEnabled(datiPensione, isRiaperturaDomanda, listaDatiContributivi, listaDatiRetributivi))
            {
                if (datiGenericiCi != null && (datiGenericiCi.RiduzioneRetributiva || datiGenericiCi.RiduzioneRetributivaPercentuale.HasValue))
                {
                    messaggioVideo = "La Riduzione Retributiva non può essere acquisita. E' necessario eliminare i Dati Istruttoria.";
                    return false;
                }
            }
            return true;
        }

        public static bool ControlsCodiceConvenzioneUruguayArgentina(GestionePensione.DatiPensione datiPensione, byte? codiceConvenzione, int? codiceStatoEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);

            if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
            {
                string primoStato = listaPrestazioniEE.First().CodiceStatoEE;

                if (primoStato == "31" && listaPrestazioniEE.Exists(x => x.CodiceConvenzione == 14) && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1985, 10, 01)))
                {
                    messaggioVideo = "Non è possibile inserire il codice convenzione 14 con la decorrenza pensione precedente al 01/10/1985";
                    return false;
                }
                else if (primoStato == "14" && listaPrestazioniEE.Exists(x => x.CodiceConvenzione == 31) && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1985, 10, 01)))
                {
                    messaggioVideo = "Non è possibile inserire il codice convenzione 31 con la decorrenza pensione precedente al 01/10/1985";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerQuota102(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
         GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaQuota102(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                if (nSettimane < 1976)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 1976 (38 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimanePerAnticipateFlessibili(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPensioniCiPrestazioniEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
            {
                int nSettimane = GestioneCrossControls.CI_GetNumeroSettimane(datiIstruttoria, datiPensioniDatiGenerici, listaPensioniCiPrestazioniEE, datiPensione);
                if (nSettimane < 2132)
                {
                    messaggioVideo = "Il numero settimane non può essere inferiore a 2132 (41 anni di contribuzione)";
                    return false;
                }
            }

            return true;
        }

        //ENG - Gestione Nuovo Codice CI28
        public static bool VerificaCodiceCI28(GestionePensione.DatiPensione datiPensione, byte? codiceConvenzione, char? codiceCI28, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((codiceConvenzione == 13 || codiceConvenzione == 14 || codiceConvenzione == 26) && !codiceCI28.HasValue)
            {
                messaggioVideo = "Per le Convenzioni 13, 14 e 26 è obbligatorio inserire il Codice CI28";
                return false;
            }

            return true;
        }

        #endregion LiquidazionePensione

        #region DatiContributivi
        public static void ControlsTotaleNumeroSettimaneRetrib(int settimaneA, int settimaneB, int settimaneC, int settimaneD, out string messaggio, out bool IsCalcoloValid)
        {
            int totale = 0;
            messaggio = string.Empty;
            IsCalcoloValid = true;

            totale = settimaneA + settimaneB + settimaneC + settimaneD;
            if (totale > 2080)
            {
                IsCalcoloValid = false;
                messaggio = "La somma delle quote delle settimane non deve essere superiore a 2080";
            }
            else
            {
                IsCalcoloValid = true;
                messaggio = string.Empty;
            }
        }

        public static void ControlsNumeroSingoleSettimaneRetr(string prodotto, int NsettimanaA, int NsettimanaB, int NsettimanaC, int NsettimanaD, DateTime? InizioAssicurazione, DateTime? FineAssicurazione, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;

            //Quota A: il numero massimo di settimane ammesso è pari alla differenza dal 31/12/92 al primo versamento;
            DateTime dataLimite = new DateTime(1992, 12, 31);
            //aggiunta settimana di tolleranza come indicato da mail del 07-09-12
            //int settimaneAmmesse = (int)Math.Ceiling(((dataLimite.Year - InizioAssicurazione.Value.Year) * 12 +
            //        (dataLimite.Month - InizioAssicurazione.Value.Month) + 1) * 4.33333) + 1;
            int settimaneAmmesse = Utility.NSettimaneBetweenDate(dataLimite, InizioAssicurazione.Value) + 1;

            if (settimaneAmmesse > 0)
            {
                if (NsettimanaA <= settimaneAmmesse)
                {
                    //Quota B: il numero massimo di settimane ammesso è pari a 104;
                    if (NsettimanaB <= 104)
                    {
                        //Quota C: il numero massimo di settimane ammesso è pari a 104;
                        if (NsettimanaC <= 104)
                        {
                            //Quota D: il numero massimo di settimane ammesso è pari alla differenza fra l’ultimo versamento e il 01/01/97.
                            dataLimite = new DateTime(1997, 01, 01);
                            //settimaneAmmesse = (int)Math.Ceiling(((FineAssicurazione.Value.Year - dataLimite.Year) * 12 +
                            //    (FineAssicurazione.Value.Month - dataLimite.Month) + 1) * 4.33333);
                            settimaneAmmesse = Utility.NSettimaneBetweenDate(FineAssicurazione.Value, dataLimite);

                            //controllo su quota D va effettuato solo per Prodotto != 0012 quindi se pari a 0012 il calcolo è valido
                            if (settimaneAmmesse > 0)
                            {
                                if (prodotto == "0012" || NsettimanaD <= settimaneAmmesse)
                                    IsCalcoloValid = true;
                                else
                                {
                                    IsCalcoloValid = false;
                                    //if (settimaneAmmesse < 0)
                                    //    messaggio = "La data 'Ultimo versamento' inserita nella tab ‘Dati Assicurativi’  del menu 'Liquidazione Pensione' non è compatibile con la 'Quota D'";
                                    //else
                                    messaggio = "Settimane quota D superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                                }
                            }
                        }
                        else
                        {
                            IsCalcoloValid = false;
                            messaggio = "Settimane quota C superiori a 104";
                        }
                    }
                    else
                    {
                        IsCalcoloValid = false;
                        messaggio = "Settimane quota B superiori a 104";
                    }
                }
                else
                {
                    IsCalcoloValid = false;
                    //if (settimaneAmmesse < 0)
                    //    messaggio = "La data 'Primo versamento' inserita nella tab 'Dati Assicurativi'  del menu ‘Liquidazione Pensione’ non è compatibile con il sistema di calcolo Retributivo";
                    //else
                    messaggio = "Settimane quota A superiori al numero di settimane ammesse (" + settimaneAmmesse + ")";
                }
            }
        }

        public static void ControlsNumeroSingoleSettimaneDL407(int NsettimanaA, int NsettimanaB, int NsettimanaC, int NsettimanaD, out string messaggio, out bool IsCalcoloValid)
        {
            messaggio = string.Empty;
            IsCalcoloValid = true;
            int settimaneAmmesse = 260;
            if (NsettimanaA <= settimaneAmmesse)
            {
                if (NsettimanaB <= 104)
                {
                    if (NsettimanaC <= 104)
                    {
                        if (NsettimanaD <= settimaneAmmesse)
                            IsCalcoloValid = true;
                        else
                        {
                            IsCalcoloValid = false;
                            messaggio = "Settimane quota D superiori a 260";
                        }
                    }
                    else
                    {
                        IsCalcoloValid = false;
                        messaggio = "Settimane quota C superiori a 104";
                    }
                }
                else
                {
                    IsCalcoloValid = false;
                    messaggio = "Settimane quota B superiori a 104";
                }
            }
            else
            {
                IsCalcoloValid = false;
                messaggio = "Settimane quota A superiori a 260";
            }
        }

        public static void ControlsSettimaneRetibutiveCalcoloMisto(int? NSettimaneQuotaC, int? NSettimaneQuotaD, decimal? RMSQuotaD, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (NSettimaneQuotaC > 52)
                messaggioVideo = "Settimane quota C superiori a 52. ";
            if (NSettimaneQuotaD != 0)
                messaggioVideo += "Settimane quota D non pari a 0. ";
            if (RMSQuotaD != 0)
                messaggioVideo += "RMS quota D non pari a 0. ";
        }

        /// <summary>
        /// Settimane estere a ricalcolo maggiori di 2080
        /// </summary>
        /// <param name="tipoCalcolo"></param>
        /// <param name="ContributiEERicalcolo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneRicalcolo2080(int? ContributiEERicalcolo)
        {
            if (ContributiEERicalcolo.HasValue && ContributiEERicalcolo.Value > 2080)
                return false;
            return true;
        }

        /// <summary>
        /// Settimane estere a ricalcolo incompatibile con Stato / Convenzione (17)
        /// </summary>
        /// <param name="ContributiEERicalcolo"></param>
        /// <param name="codeConv"></param>
        /// <param name="codeStatoEE"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneRicalcoloCodeConv(int? ContributiEERicalcolo, byte? codeConv, string codeStatoEE)
        {
            if (ContributiEERicalcolo.HasValue && ContributiEERicalcolo.Value != 0 && codeConv == 17 && codeStatoEE.Trim() == "17")
                return false;
            return true;
        }

        /// <summary>
        /// Settimane estere a ricalcolo incompatibili con decorrenza estera
        /// </summary>
        /// <param name="ContributiEERicalcolo"></param>
        /// <param name="DecorrenzaPrestazioneEE"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneRicalcoloDecorrEstero(int? ContributiEERicalcolo, DateTime? DecorrenzaPrestazioneEE)
        {
            if (ContributiEERicalcolo.HasValue && ContributiEERicalcolo.Value > 0 && !DecorrenzaPrestazioneEE.HasValue)
                return false;
            return true;
        }

        /// <summary>
        /// Settimane estere a ricalcolo incompatibili con decorrenza estera
        /// </summary>
        /// <param name="ContributiEERicalcolo"></param>
        /// <param name="DecorrenzaPrestazioneEE"></param>
        /// <param name="DecorrOriginaria"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneRicalcoloDecEsteroDecOrig(int? ContributiEERicalcolo, DateTime? DecorrenzaPrestazioneEE, DateTime? DecorrOriginaria)
        {
            if (ContributiEERicalcolo.HasValue && ContributiEERicalcolo.Value > 0 && DecorrenzaPrestazioneEE.HasValue && DecorrOriginaria.HasValue && DecorrenzaPrestazioneEE.Value.CompareTo(DecorrOriginaria.Value) == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Settimane estere a ricalcolo incompatibili con decorrenza estera
        /// </summary>
        /// <param name="ContributiEERicalcolo"></param>
        /// <param name="ContributiEEDecOrig"></param>
        /// <returns></returns>
        public static bool VerificaContribRicalcoloContribDecOrig(int? ContributiEERicalcolo, int? ContributiEEDecOrig)
        {
            if (ContributiEERicalcolo.HasValue && ContributiEERicalcolo.Value > 0 && ContributiEEDecOrig.HasValue && ContributiEERicalcolo < ContributiEEDecOrig)
                return false;
            return true;
        }

        /// <summary>
        /// Settimane diritto mancanti o errate
        /// </summary>
        /// <param name="ContributiEEDiritto"></param>
        /// <param name="ContributiEEDecOrig"></param>
        /// <returns></returns>
        public static bool VerificaContribDirittoNullContribDecOrig(int? ContributiEEDiritto, int? ContributiEEDecOrig, int codiceStato, int istituzione)
        {
            if (!ContributiEEDiritto.HasValue && ContributiEEDecOrig.HasValue && ContributiEEDecOrig > 0 && !(codiceStato == 01 && (istituzione == 0509 || istituzione == 0510)))
                return false;
            if (ContributiEEDiritto.HasValue && ContributiEEDiritto.Value > 3000)
                return false;
            return true;
        }

        /// <summary>
        /// Settimane esteri mancanti
        /// </summary>
        /// <param name="indexEE"></param>
        /// <param name="CodeStatoEE"></param>
        /// <param name="ContributiEERicalcolo"></param>
        /// <param name="DecorrenzaPrestazioneEE"></param>
        /// <param name="codeConv1Stato"></param>
        /// <param name="ContributiEEDecOrig"></param>
        /// <param name="DecorrOriginaria"></param>
        /// <param name="DecorrOpzione"></param>
        /// <returns></returns>
        public static bool VerificaSettEstereCodeNot17(int indexEE, string CodeStatoEE, int? ContributiEERicalcolo, DateTime? DecorrenzaPrestazioneEE, byte? codeConv1Stato, int? ContributiEEDecOrig, DateTime? DecorrOriginaria, DateTime? DecorrOpzione)
        {
            if (!ContributiEEDecOrig.HasValue)
            {
                if (!string.IsNullOrEmpty(CodeStatoEE) && CodeStatoEE != "17" && codeConv1Stato.HasValue && codeConv1Stato.Value == 12)
                {
                    DateTime dtMin = new DateTime(2002, 06, 01);
                    if (DecorrOriginaria.HasValue && DecorrOriginaria.Value.CompareTo(dtMin) < 0 && !DecorrOpzione.HasValue)
                    {
                        if (indexEE > 0)
                        {
                            if (!ContributiEERicalcolo.HasValue && (!DecorrenzaPrestazioneEE.HasValue || (DecorrenzaPrestazioneEE.Value.CompareTo(DecorrOriginaria) == 0)))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        #region Controlli Duplicati
        /// / <summary>
        /// / Settimane italiane diritto mancanti
        /// / </summary>
        /// / <param name="nSettOBG"></param>
        /// / <param name="codeConv"></param>
        /// / <returns></returns>
        //public static bool VerificaSettItalianeCodeConv(int? nSettOBG, byte? codeConv)
        //{
        //    if (!nSettOBG.HasValue && (codeConv.HasValue && codeConv.Value != 17 && codeConv.Value != 20))
        //        return false;
        //    return true;
        //}

        /// / <summary>
        /// / Settimane italiane diritto mancanti
        /// / </summary>
        /// / <param name="indexEE"></param>
        /// / <param name="CodeStatoEE"></param>
        /// / <param name="nSettOBG"></param>
        /// / <param name="codeConv"></param>
        /// / <returns></returns>
        //public static bool VerificaSettItalianeCodeConvCodeStatoEE(int indexEE, string CodeStatoEE, int? nSettOBG, byte? codeConv)
        //{
        //    if (!nSettOBG.HasValue && (codeConv.HasValue && codeConv.Value != 17 && codeConv.Value != 20 && codeConv.Value != 27))
        //    {
        //        if (indexEE == 0 && CodeStatoEE != "17")
        //            return false;
        //    }

        //    return true;
        //}

        /// / <summary>
        /// / Settimane italiane inferiori a MIN-CTRL per convensione
        /// / </summary>
        /// / <param name="indexEE"></param>
        /// / <param name="CodeStatoEE"></param>
        /// / <param name="Gruppo"></param>
        /// / <param name="nSettOBG"></param>
        /// / <param name="nContrVol"></param>
        /// / <param name="nSettGod"></param>
        /// / <param name="minCTR"></param>
        /// / <param name="codeConv"></param>
        /// / <param name="DecorrOriginaria"></param>
        /// / <param name="DCDecorrPens"></param>
        /// / <param name="codeOpzRiliqGP"></param>
        /// / <param name="CodeNatura"></param>
        /// / <param name="nSettA"></param>
        /// / <param name="nSettB"></param>
        /// / <param name="nSett"></param>
        /// / <param name="nSettGodimentoAss"></param>
        /// / <param name="ProvResidenza"></param>
        /// / <param name="dec1Suppl"></param>
        /// / <param name="msg"></param>
        /// / <returns></returns>
        //public static bool VerificaSettItalianeCodeConvCodeStatoEE(int indexEE, string CodeStatoEE, string Gruppo, int nSettOBG, int nContrVol, int nSettGod, int minCTR,
        //                                                           byte? codeConv, DateTime? DecorrOriginaria, DateTime? DCDecorrPens, byte? codeOpzRiliqGP, string CodeNatura,
        //                                                           int nSettA, int nSettB, int nSett, int nSettGodimentoAss, string ProvResidenza, DateTime? dec1Suppl, out string msg)
        //{
        //    msg = string.Empty;
        //    DateTime dtMin = new DateTime(1972, 09, 01);
        //    DateTime dtMax = new DateTime(2002, 06, 01);

        //    if ((codeConv.HasValue && codeConv.Value != 12) || (codeConv.HasValue && codeConv.Value == 12 && DecorrOriginaria.HasValue && DecorrOriginaria.Value.CompareTo(dtMin) > 0))
        //    {
        //        if (indexEE == 0 && ((!String.IsNullOrEmpty(CodeStatoEE) && CodeStatoEE != "17") || (!String.IsNullOrEmpty(CodeStatoEE) && CodeStatoEE == "17" && DCDecorrPens.HasValue && DCDecorrPens.Value.CompareTo(dtMax) <= 0)))
        //        {
        //            if ((nSettOBG + nContrVol + nSettGod) < minCTR)
        //            {
        //                if (codeOpzRiliqGP == 7)
        //                {
        //                    Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(Gruppo);
        //                    if (tipoDomanda != Utility.TipoDomanda.Reversibilita)
        //                    {
        //                        if ((!string.IsNullOrEmpty(CodeNatura) && CodeNatura.Substring(0, 1) != "1" && CodeNatura.Substring(0, 1) != "2") ||
        //                           (nSettA + nSettB + nSett + nSettGodimentoAss) < minCTR)
        //                            return false;
        //                    }
        //                    else
        //                    {
        //                        if ((DecorrOriginaria.HasValue && DecorrOriginaria.Value.CompareTo(dtMin) <= 0) ||
        //                            (!string.IsNullOrEmpty(ProvResidenza) && ProvResidenza.Trim().ToUpper() == "I") ||
        //                            (codeConv.HasValue && (codeConv.Value == 38 || codeConv.Value == 39)))
        //                        {
        //                            if (!DCDecorrPens.HasValue || !dec1Suppl.HasValue)
        //                            {
        //                                if (codeConv.Value != 12 || (indexEE == 0 && CodeStatoEE != "38") /* || !TP1CATD.HasValue */)
        //                                    return false;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}
        #endregion Controlli Duplicati

        /// <summary>
        /// Verifica se nContributiItalia non è valorizzato e codiceStatoEE del primo stato è 17 o 20
        /// </summary>
        /// <param name="nContributiItalia"></param>
        /// <param name="codiceStatoEE"></param>
        /// <returns>False se nContributiItalia non è valorizzato e codiceStatoEE del primo stato è 17 o 20, true altrimenti</returns>
        public static bool VerificaSettimaneEffettiveCodiceStatoEE(int? nContributiItalia, string codiceStatoEE)
        {
            if (!nContributiItalia.HasValue && !string.IsNullOrEmpty(codiceStatoEE) && (codiceStatoEE.Trim().ToUpperInvariant() == "17" || codiceStatoEE.Trim().ToUpperInvariant() == "20"))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se nSettimaneOBG è valorizzato e nContributiItalia non è valorizzato
        /// </summary>
        /// <param name="nSettimaneOBG"></param>
        /// <param name="nContributiItalia"></param>
        /// <returns>False se nSettimaneOBG è valorizzato e nContributiItalia non è valorizzato, true altrimenti</returns>
        public static bool VerificaSettinaneEffettiveNSettimaneOBG(int? nSettimaneOBG, int? nContributiItalia)
        {
            if (nSettimaneOBG.HasValue && (!nContributiItalia.HasValue || nContributiItalia.Value == 0))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se FineAssicurazione è maggiore del 31/12/1995 e FlagContributiva non è S e CodiceNatura2 non è O e NSettimaneQuotaB non valorizzato e
        /// NSettimane non valorizzate e CodiceConvenzione non è 17 e NContributiVolontari non è valorizzato
        /// </summary>
        /// <param name="fineAssicurazione"></param>
        /// <param name="flagContributiva"></param>
        /// <param name="codiceNatura"></param> 
        /// <param name="nSettimaneQuotaB"></param>
        /// <param name="nSettimane"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="nContributiVolontari"></param>
        /// <returns>
        /// False se FineAssicurazione è maggiore del 31/12/1995 e FlagContributiva non è S e CodiceNatura2 non è O e NSettimaneQuotaB non valorizzato e
        /// NSettimane non valorizzate e CodiceConvenzione non è 17 e NContributiVolontari non è valorizzato, true altrimenti
        /// </returns>
        public static bool VerificaOBGMisura335Contributi335(DateTime? fineAssicurazione, bool? flagContributiva, string codiceNatura, int? nSettimaneQuotaB, int? nSettimane, byte? codiceConvenzione, int? nContributiVolontari)
        {
            DateTime dataCompare = new DateTime(1995, 12, 31);
            if (fineAssicurazione.HasValue && fineAssicurazione.Value.CompareTo(dataCompare) > 0 && (!flagContributiva.HasValue || !flagContributiva.Value)
                && !string.IsNullOrEmpty(codiceNatura) && codiceNatura.Substring(1, 1).ToUpperInvariant() != "O"
                && !nSettimaneQuotaB.HasValue && !nSettimane.HasValue && codiceConvenzione.HasValue && codiceConvenzione.Value != 17 && !nContributiVolontari.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se vvMisura192 è valorizzato e se decorrenzaOriginaria è minore del 08/1972 e decorrenzaOpzione è pari a zero
        /// </summary>
        /// <param name="vvMisura192"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <returns>False se vvMisura192 è valorizzato e se decorrenzaOriginaria è minore del 08/1972 e decorrenzaOpzione è pari a zero, true altrimenti</returns>
        public static bool VerificaSettVVMisuraWithDecOriginariaWithDecOpzione(int? vvMisura192, DateTime? decorrenzaOriginaria, DateTime? decorrenzaOpzione)
        {
            DateTime decorrenzaCompare = new DateTime(1972, 8, 1);

            if (vvMisura192.HasValue && decorrenzaOriginaria.HasValue && (decorrenzaOriginaria.Value.CompareTo(decorrenzaCompare) < 0 && !decorrenzaOpzione.HasValue))
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se vvMisura192 è valorizzato e se decorrenzaOpzione è minore del 08/1972
        /// </summary>
        /// <param name="vvMisura192"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns>False se vvMisura192 è valorizzato e se decorrenzaOpzione è minore del 08/1972, true altrimenti</returns>
        public static bool VerificaSettimaneVVMisuraWithDecorrenzaOriginaria(int? vvMisura192, DateTime? decorrenzaOpzione)
        {
            DateTime decorrenzaCompare = new DateTime(1972, 8, 1);
            if (vvMisura192.HasValue && decorrenzaOpzione.HasValue && decorrenzaOpzione.Value.CompareTo(decorrenzaCompare) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se vvMisura192, nContributiVolontari, importoIVS sono valorizzati e diversi da zero
        /// </summary>
        /// <param name="vvMisura192"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="importoIVS"></param>
        /// <returns>False se vvMisura192 è valorizzato e se nContributiVolontari è valorizzato e se importoIVS è valorizzato e se sono diversi da zero, true altrimenti</returns>
        public static bool VerificaSettVVMisuraWithNContribVolontariWithImportoIVS(int? vvMisura192, int? nContributiVolontari, decimal? importoIVS)
        {
            if (vvMisura192.HasValue && vvMisura192 != 0 && nContributiVolontari.HasValue && nContributiVolontari != 0 && importoIVS.HasValue && importoIVS != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se vvMisura192 è valorizzato e se nContributiVolontari non è valorizzato
        /// </summary>
        /// <param name="vvMisura192"></param>
        /// <param name="nContributiVolontari"></param>
        /// <returns>False se vvMisura192 è valorizzato e se nContributiVolontari non è valorizzato, true altrimenti</returns>
        public static bool VerificaSettimaneVVMisuraWithNContributiVolontari(int? vvMisura192, int? nContributiVolontari)
        {
            if (vvMisura192.HasValue && !nContributiVolontari.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se VVMisuraAl1292 non è valorizzato allora se DecorrenzaOriginaria è maggiore del 07/1972 e DecorrenzaOpzione è maggiore del 07/1972 
        /// e NContributiVolontari valorizzato e ImportoIVS non valorizzato e VVMisuraDL50392 non valorizzato e NSettimane non valorizzato
        /// </summary>
        /// <param name="vvMisura192"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="importoIVS"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="nSettimane"></param>
        /// <returns>
        /// False se VVMisuraAl1292 non è valorizzato allora se DecorrenzaOriginaria è maggiore del 07/1972 e DecorrenzaOpzione è maggiore del 07/1972 
        /// e NContributiVolontari valorizzato e ImportoIVS non valorizzato e VVMisuraDL50392 non valorizzato e NSettimane non valorizzato, true altrimenti
        /// </returns>
        public static bool VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(int? vvMisura192, DateTime? decorrenzaOriginaria, DateTime? decorrenzaOpzione,
            int? nContributiVolontari, decimal? importoIVS, int? vvMisuraDL50392, int? nSettimane)
        {
            DateTime decorrenzaCompare = new DateTime(1972, 7, 1);

            if (!vvMisura192.HasValue && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.CompareTo(decorrenzaCompare) > 0 && decorrenzaOpzione.HasValue && decorrenzaOpzione.Value.CompareTo(decorrenzaCompare) > 0
                && nContributiVolontari.HasValue && !importoIVS.HasValue && !vvMisuraDL50392.HasValue && !nSettimane.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Se CodiceConvenzione = 12 e DecorrenzaOriginaria maggiore del 05/2003 ed è presente almeno un CodiceStatoEE  = 18 (Danimarca) 
        /// con DecorrenzaPrestazioneEE valorizzata e ContributiEEDiritto maggiore di 0 allora se la Cittadinanza 
        /// non è compresa tra "A", "B", "BG", "CY", "DK", "EST", "FIN", "F", "D", "GR", "IRL", "I", "LV", "LT", "L", "M", 
        /// "NL", "PL", "P", "GB", "CZ", "RO", "SK", "SLO", "E", "S", "H"
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="cittadinanza"></param>
        /// <returns>
        /// False Se CodiceConvenzione = 12 e DecorrenzaOriginaria maggiore del 05/2003 ed è presente almeno un CodiceStatoEE  = 18 (Danimarca) 
        /// con DecorrenzaPrestazioneEE valorizzata e ContributiEEDiritto maggiore di 0 allora se la Cittadinanza 
        /// non è compresa tra "A", "B", "BG", "CY", "DK", "EST", "FIN", "F", "D", "GR", "IRL", "I", "LV", "LT", "L", "M", 
        /// "NL", "PL", "P", "GB", "CZ", "RO", "SK", "SLO", "E", "S", "H", true altrimenti
        /// </returns>
        public static bool VerificaContributiDanimarca(byte? codiceConvenzione, DateTime? decorrenzaOriginaria, string codiceStatoEE, DateTime? decorrenzaPrestazioneEE, int? contributiEEDiritto, string cittadinanza)
        {
            List<string> cittadinanzaList = new List<string>{"Z102", "Z103", "Z104", "Z211", "Z107", "Z144", "Z109", "Z110", "Z112", "Z115", "Z116", "Z000", "Z145", "Z146", "Z120",
                "Z121", "Z126", "Z127", "Z128", "Z114", "Z156", "Z129", "Z155", "Z150", "Z131", "Z132", "Z134"};

            bool? isCittadinanza = null;

            if (string.IsNullOrEmpty(cittadinanza) || string.IsNullOrEmpty(cittadinanza.Trim()))
                isCittadinanza = false;
            else
                isCittadinanza = cittadinanzaList.Contains(cittadinanza.Trim().ToUpperInvariant());

            DateTime decorrenzaCompare = new DateTime(2003, 5, 1);

            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 12 && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.CompareTo(decorrenzaCompare) > 0
                && !string.IsNullOrEmpty(codiceStatoEE) && codiceStatoEE.Trim() == "18"
                && decorrenzaPrestazioneEE.HasValue && contributiEEDiritto.GetValueOrDefault() > 0 && !isCittadinanza.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Se CodiceConvenzione = 12 e DecorrenzaOriginaria maggiore del 05/2003 ed è presente almeno un CodiceStatoEE  = 18 (Danimarca) 
        /// con DecorrenzaPrestazioneEE valorizzata e ContributiEEDiritto maggiore di 0 allora se è una reversibilità e la Cittadinanza del DanteCausa
        /// non è compresa tra "A", "B", "BG", "CY", "DK", "EST", "FIN", "F", "D", "GR", "IRL", "I", "LV", "LT", "L", "M", 
        /// "NL", "PL", "P", "GB", "CZ", "RO", "SK", "SLO", "E", "S", "H"
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="cittadinanza"></param>
        /// <param name="gruppo"></param>
        /// <returns>
        /// False Se CodiceConvenzione = 12 e DecorrenzaOriginaria maggiore del 05/2003 ed è presente almeno un CodiceStatoEE  = 18 (Danimarca) 
        /// con DecorrenzaPrestazioneEE valorizzata e ContributiEEDiritto maggiore di 0 allora se è una reversibilità e la Cittadinanza del DanteCausa
        /// non è compresa tra "A", "B", "BG", "CY", "DK", "EST", "FIN", "F", "D", "GR", "IRL", "I", "LV", "LT", "L", "M", 
        /// "NL", "PL", "P", "GB", "CZ", "RO", "SK", "SLO", "E", "S", "H", true altrimenti
        /// </returns>
        public static bool VerificaContributiDanimarcaDanteCausa(byte? codiceConvenzione, DateTime? decorrenzaOriginaria, string codiceStatoEE, DateTime? decorrenzaPrestazioneEE, int? contributiEEDiritto,
            string cittadinanza, string gruppo, string prodotto)
        {
            if (string.IsNullOrEmpty(gruppo))
                return false;

            List<string> cittadinanzaList = new List<string>{"A", "B", "BG", "CY", "DK", "EST", "FIN", "F", "D", "GR", "IRL", "I", "LV", "LT", "L",
                "M", "NL", "PL", "P", "GB", "CZ", "RO", "SK", "SLO", "E", "S", "H"};

            bool? isCittadinanza = null;

            if (string.IsNullOrEmpty(cittadinanza) || string.IsNullOrEmpty(cittadinanza.Trim()))
                isCittadinanza = false;
            else
                isCittadinanza = cittadinanzaList.Contains(cittadinanza.Trim().ToUpperInvariant());

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
            DateTime decorrenzaCompare = new DateTime(2003, 5, 1);

            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 12 && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.CompareTo(decorrenzaCompare) > 0
                && !string.IsNullOrEmpty(codiceStatoEE) && codiceStatoEE.Trim() == "18"
                && decorrenzaPrestazioneEE.HasValue && contributiEEDiritto.HasValue && contributiEEDiritto.Value > 0 && !isCittadinanza.Value && tipoDomanda == Utility.TipoDomanda.Superstiti)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se DecorrenzaOpzione non è valorizzato e DecorrenzaOriginaria è minore del 05/1968 e DanteCausa.Certificato è non valorizzato 
        /// e se RMSQuotaA è valorizzato
        /// </summary>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="certificato"></param>
        /// <param name="rmsQuotaA"></param>
        /// <returns>
        /// False se DecorrenzaOpzione non è valorizzato e DecorrenzaOriginaria è minore del 05/1968 e DanteCausa.Certificato è non valorizzato 
        /// e se RMSQuotaA è valorizzato, true altrimenti
        /// </returns>
        public static bool VerificaRMSWithDecOriginaria(DateTime? decorrenzaOpzione, DateTime? decorrenzaOriginaria, int? certificatoDanteCausa, decimal? rmsQuotaA)
        {
            DateTime decorrenzaCompare = new DateTime(1969, 5, 1);

            if (!decorrenzaOpzione.HasValue && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.CompareTo(decorrenzaCompare) < 0 && !certificatoDanteCausa.HasValue && rmsQuotaA.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se per una non reversibilità o per una reversibilità per la quale il certificato del DAntecausa è nullo o la DanteCausa.DecorrenzaPensione 
        /// è maggiore o pari del 05/1968 allora se RMSQuotaAnon è valorizzato e InizioAssicurazione è minore del 01/05/1968 allora se la CatPens è diverva  da 0006 
        /// o il certificato del DanteCausa è valorizzato o la DataMorte del DanteCausa è maggiore o uguale al 01/05/1968 
        /// allora se la DecorrenzaOpzione non è maggiore del 01/1979 allora se non siamo nel caso contributivo allora se CodiceNatura2 è diverso da O
        /// </summary>
        /// <param name="certificatoDanteCausa"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="categoriaPensione"></param>
        /// <param name="dataMorteDanteCausa"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="flagContributivo"></param>
        /// <param name="codiceNatura"></param>
        /// <param name="gruppo"></param>
        /// <returns>
        /// False se per una non reversibilità o per una reversibilità per la quale il certificato del DAntecausa è nullo o la DanteCausa.DecorrenzaPensione 
        /// è maggiore o pari del 05/1968 allora se RMSQuotaAnon è valorizzato e InizioAssicurazione è minore del 01/05/1968 allora se la CatPens è diverva  da 0006 
        /// o il certificato del DanteCausa è valorizzato o la DataMorte del DanteCausa è maggiore o uguale al 01/05/1968 
        /// allora se la DecorrenzaOpzione non è maggiore del 01/1979 allora se non siamo nel caso contributivo allora se CodiceNatura2 è diverso da O, true altrimenti
        /// </returns>
        public static bool VerificaRMSDanteCausa(int? certificatoDanteCausa, DateTime? decorrenzaPensioneDC, decimal? rmsQuotaA, DateTime? inizioAssicurazione, string categoriaPensione,
            DateTime? dataMorteDanteCausa, DateTime? decorrenzaOpzione, bool? flagContributivo, string codiceNatura, string gruppo, string prodotto)
        {
            if (string.IsNullOrEmpty(gruppo))
                return false;

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(gruppo, prodotto);
            DateTime decorrenzaCompare = new DateTime(1968, 05, 01);
            DateTime decorrenzaOpzioneCompare = new DateTime(1979, 01, 01);

            if (tipoDomanda != Utility.TipoDomanda.Superstiti || (!certificatoDanteCausa.HasValue || (decorrenzaPensioneDC.HasValue && decorrenzaPensioneDC.Value.CompareTo(decorrenzaCompare) >= 0)))
                if (!rmsQuotaA.HasValue && inizioAssicurazione.HasValue && inizioAssicurazione.Value.CompareTo(decorrenzaCompare) < 0)
                    if (!string.IsNullOrEmpty(categoriaPensione) && categoriaPensione.Trim().ToUpperInvariant() != "SOS" || certificatoDanteCausa.HasValue || (dataMorteDanteCausa.HasValue && dataMorteDanteCausa.Value.CompareTo(decorrenzaCompare) >= 0))
                        if (decorrenzaOpzione.HasValue && decorrenzaOpzione.Value.CompareTo(decorrenzaOpzioneCompare) <= 0)
                            if (flagContributivo.HasValue && !flagContributivo.Value)
                                if (!string.IsNullOrEmpty(codiceNatura) && codiceNatura.Substring(2, 1).ToUpperInvariant() != "O")
                                    return false;

            return true;
        }

        public static bool ControlsSettimane707(GestionePensione.DatiPensione datiPensione, List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi,
            List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetributivo,
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo, bool? flagContributiva, int? contributiItalianiEdEsteriAl1295, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneCalcolo.DatiCalcoloRetributivo> lRetributivi = GestioneContrib.MappingDatiRetributiviFromViewToBL(lDatiRetributivi);
            List<GestioneCalcolo.DatiCalcoloContributivo> lContributivi = GestioneContrib.MappingDatiContributiviFromViewToBL(lDatiContributivi);

            if (!ControlsSettimane707(datiPensione, lRetributivi, lContributivi, listaCodeGestioneCalcoloRetributivo, listaCodeGestioneCalcoloContributivo, flagContributiva, contributiItalianiEdEsteriAl1295, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsSettimane707(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> lDatiContributivi, List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetributivo,
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo, bool? flagContributiva, int? contributiItalianiEdEsteriAl1295, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!GestioneContrib.IsSettimane707Visible(datiPensione, lDatiRetributivi, lDatiContributivi, flagContributiva))
            {
                if (lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.NSettimane707.HasValue))
                {
                    messaggioVideo = "Settimane 707 non ammesse.";
                    return false;
                }

                return true;
            }

            // Per le domande manuali il campo “Contributi italiani ed esteri al 31/12/1995” (attualmente inibito se inizio assicurazione < 01.01.1996) dovrà essere obbligatoriamente valorizzato accettando anche il valore zero.
            // Il campo continuerà ad essere inibito solo quando il campo “Opzione Contributiva” = True (scheda “Generici” del quadro “Liquidazione Pensione”)
            if (!flagContributiva.GetValueOrDefault() ||
                (datiPensione.InizioAssicurazione.HasValue && !Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(1995, 12, 31))))
            {
                if (!contributiItalianiEdEsteriAl1295.HasValue)
                {
                    messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 obbligatori.";
                    return false;
                }

                // Per le domande automatiche “Unicarpe C.I.” dovrà valorizzare sempre il campo “Contributi italiani ed esteri al 31.12.1995” nonché le settimane 707 e il campo “Reddito/retr.media”
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                {
                    if (!contributiItalianiEdEsteriAl1295.HasValue)
                    {
                        messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 obbligatori per domande automatiche.";
                        return false;
                    }

                    if (lDatiRetributivi == null)
                    {
                        // Codice controllo CI: CI_0001
                        if (!(Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione)) &&
                            contributiItalianiEdEsteriAl1295 >= 936)
                        {
                            messaggioVideo = "Dati Retributivi obbligatori.";
                            return false;
                        }
                    }
                    else
                    {
                        if (lDatiRetributivi.Exists(x => !x.NSettimane707.HasValue) && contributiItalianiEdEsteriAl1295 >= 936)
                        {
                            messaggioVideo = "Sett. 707 obbligatorie.";
                            return false;
                        }

                        if (lDatiRetributivi.Exists(x => !x.RMSQuotaA.HasValue && !x.RMSQuotaB.HasValue))
                        {
                            messaggioVideo = "Reddito/retr.media obbligatorio.";
                            return false;
                        }
                    }
                }

                // Il campo sett.707 non può essere valorizzato se è presente una quota C di qualsiasi gestione
                if (lDatiContributivi != null && lDatiContributivi.Exists(x => x.IsQuotaL335Presente()) && lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.NSettimane707.HasValue))
                {
                    messaggioVideo = "Il campo Sett.707 non può essere valorizzato se è presente una quota C di qualsiasi gestione";
                    return false;
                }

                // Il campo sett.707 non può essere valorizzato se “Contributi italiani ed esteri al 31/12/1995” < 936 
                if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() < 936)
                {
                    if (lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.NSettimane707.HasValue))
                    {
                        messaggioVideo = "Il campo Sett.707 non può essere valorizzato se “Contributi italiani ed esteri al 31/12/1995” < 936.";
                        return false;
                    }
                }

                ////// Recupero l'elenco di codici gestione inseriti
                #region Recupero codici gestione
                List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> lCodeGestioneRetributivoInserito = null;
                if (lDatiRetributivi != null)
                    lCodeGestioneRetributivoInserito = listaCodeGestioneCalcoloRetributivo.FindAll(x => lDatiRetributivi.Select(y => y.CodiceGestione).Contains(x.Id));

                List<GestioneDecodifica.CodeGestioneCalcoloContributivo> lCodeGestioneContributivoInserito = null;
                if (lDatiContributivi != null)
                    lCodeGestioneContributivoInserito = listaCodeGestioneCalcoloContributivo.FindAll(x => lDatiContributivi.Select(y => y.CodiceGestione).Contains(x.Id));

                List<string> lCodiceGestione = new List<string>();
                if (lCodeGestioneRetributivoInserito != null && lCodeGestioneRetributivoInserito.Count > 0)
                    lCodiceGestione.AddRange(lCodeGestioneRetributivoInserito.Select(x => x.TraduzioneSuGP.Trim()).ToList());
                if (lCodeGestioneContributivoInserito != null && lCodeGestioneContributivoInserito.Count > 0)
                    lCodiceGestione.AddRange(lCodeGestioneContributivoInserito.Select(x => x.TraduzioneSuGP.Trim()).ToList());

                lCodiceGestione = lCodiceGestione.Distinct().ToList();
                #endregion Recupero codici gestione
                ///////

                // Le settimane 707 della quota B non può essere minore della somma delle settimane della quota B e delle settimane della quota D relative alla stessa gestione
                foreach (string codiceGestione in lCodiceGestione)
                {
                    if (!string.IsNullOrEmpty(codiceGestione))
                    {
                        long? gestione = null;
                        GestioneDecodifica.CodeGestioneCalcoloRetributivo gestioneCalcoloRetributivo = listaCodeGestioneCalcoloRetributivo.Find(x => x.TraduzioneSuGP.Trim() == codiceGestione.Trim());
                        if (gestioneCalcoloRetributivo != null)
                            gestione = gestioneCalcoloRetributivo.Id;

                        GestioneCalcolo.DatiCalcoloRetributivo retributiviB = null;
                        if (gestione.HasValue && lDatiRetributivi != null && lDatiRetributivi.Count > 0)
                            retributiviB = lDatiRetributivi.Find(x => x.CodiceGestione == gestione && x.QuotePrimeLiquidate == 'B');
                        int settimaneB = 0;
                        int settimane707 = 0;
                        if (retributiviB != null)
                        {
                            settimaneB = retributiviB.NSettimaneQuotaB.GetValueOrDefault();
                            settimane707 = retributiviB.NSettimane707.GetValueOrDefault();
                        }

                        gestione = null;
                        GestioneDecodifica.CodeGestioneCalcoloContributivo gestioneCalcoloContributivo = listaCodeGestioneCalcoloContributivo.Find(x => x.TraduzioneSuGP.Trim() == codiceGestione.Trim());
                        if (gestioneCalcoloContributivo != null)
                            gestione = gestioneCalcoloContributivo.Id;

                        GestioneCalcolo.DatiCalcoloContributivo contributiviD = null;
                        if (gestione.HasValue && lDatiContributivi != null && lDatiContributivi.Count > 0)
                            contributiviD = lDatiContributivi.Find(x => x.CodiceGestione == gestione && x.IsQuotaDL214Presente());
                        int settimaneD = 0;
                        if (contributiviD != null)
                            settimaneD = contributiviD.NSettimaneQuotaDL214.GetValueOrDefault();

                        // Il campo sett.707  può essere valorizzato solo se è presente una quota D nella stessa gestione
                        if ((Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && retributiviB != null && settimane707 > 0 && contributiviD == null) ||
                            (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && settimane707 > 0 && contributiviD != null && lDatiRetributivi != null && !lDatiRetributivi.Exists(x => x.CodiceGestione == contributiviD.CodiceGestione && x.QuotePrimeLiquidate == 'B')))
                        {
                            messaggioVideo = "Il campo Sett.707 può essere valorizzato solo se è presente una quota D nella stessa Gestione (" + gestioneCalcoloRetributivo.TraduzioneSuGP + " - " + gestioneCalcoloRetributivo.Descrizione + ")";
                            return false;
                        }

                        if (contributiviD != null && contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 935 && (retributiviB == null || settimane707 == 0))
                        {
                            messaggioVideo = "E' obbligatorio inserire le Sett.707 per la quota B della gestione " + codiceGestione;
                            return false;
                        }

                        if (settimane707 != 0 && settimane707 < settimaneB + settimaneD)
                        {
                            messaggioVideo = "Il campo Sett. 707 della quota B della gestione " + codiceGestione + " non può essere minore della somma delle settimane della quota B e delle settimane della quota D relative alla stessa gestione";
                            return false;
                        }
                    }
                }

                // Se viene valorizzato il campo settimane nella colonna “sett.707”, dovrà essere valorizzato anche la colonna Reddito/retr.media e nel campo “Settimane” dovrà essere accettato anche il valore “blank”, inserito con lo “0”
                if (lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.NSettimane707.HasValue && ((!x.RMSQuotaA.HasValue && !x.RMSQuotaB.HasValue) || (!x.NSettimaneQuotaA.HasValue && !x.NSettimaneQuotaB.HasValue))))
                {
                    messaggioVideo = "In presenza delle Sett.707 è necessario inserire Settimane e Reddito/retr.media.";
                    return false;
                }

                if (lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.QuotePrimeLiquidate == 'B' && (!x.NSettimane707.HasValue || x.NSettimane707 == 0) && x.NSettimaneQuotaB == 0))
                {
                    messaggioVideo = "Le settimane 0 in quota B sono accettabili solo se presenti le settimane 707";
                    return false;
                }
            }
            // Il campo aggiuntivo delle settimane 707 non dovrà essere mostrato per domande che abbiano “Opzione contributiva”= True
            else
            {
                if (lDatiRetributivi != null && lDatiRetributivi.Exists(x => x.NSettimane707.HasValue))
                {
                    messaggioVideo = "Settimane 707 non ammesse.";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsContributiItalianiEsteriAl1295WithQuotaC(GestionePensione.DatiPensione datiPensione, int? contributiItalianiEdEsteriAl1295, int settimaneQuotaCTotale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
                return true;

            if (Utility.IsDomandaTipoContributivo(datiPensione, null, true) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
            {
                if (!contributiItalianiEdEsteriAl1295.HasValue || (contributiItalianiEdEsteriAl1295.HasValue && contributiItalianiEdEsteriAl1295.Value == 0))
                {
                    messaggioVideo = "Per poter optare per il sistema contributivo è necessario aver versato almeno un contributo anteriormente al 1/1/1996";
                    return false;
                }

                if (contributiItalianiEdEsteriAl1295.HasValue && contributiItalianiEdEsteriAl1295.Value >= 936)
                {
                    messaggioVideo = "Per poter optare per il sistema contributivo è necessario aver versato meno di 936 contributi settimanali (pari a 18 anni) al 31/12/1995";
                    return false;
                }
            }

            if (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione))
            {
                if (!contributiItalianiEdEsteriAl1295.HasValue || (contributiItalianiEdEsteriAl1295.HasValue && contributiItalianiEdEsteriAl1295.Value == 0))
                {
                    messaggioVideo = "Contributi italiani ed esteri al 31/12/95 non ammesso. Per accedere alla pensione anticipata flessibile è necessario avere contribuzione ante 1996";
                    return false;
                }
            }

            if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 936 && !Utility.IsDomandaAnticipataFlessibile(datiPensione) && !Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && !Utility.IsDomandaTipoContributivo(datiPensione, true, null) && !Utility.IsDomandaTipoContributivo(datiPensione, true, true))
            {
                if (datiPensione.Gruppo == "0001" && settimaneQuotaCTotale > 0)
                {
                    messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 maggiori di 936. Non è possibile inserire la quota C";
                    return false;
                }
            }
            return true;
        }

        public static bool ControlsFineAssicurazioneWithQuotaD(GestionePensione.DatiPensione datiPensione, int settimaneQuotaDTotale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsRiaperturaDomanda(datiPensione.Id))
            {
                if (datiPensione.Gruppo == "0001" && GestioneContrib.IsFineAssicurazionePost2012(datiPensione.FineAssicurazione) && settimaneQuotaDTotale == 0 &&
                   !(Utility.IsDomandaVecchiaiaTrasformazioneAOI(datiPensione).GetValueOrDefault() && Utility.IsDomandaAutomatica(datiPensione)))
                {
                    messaggioVideo = "Per domande con data fine assicurazione pari o successiva al 01/01/2012 è necessario inserire la quota D";
                    return false;
                }
            }

            return true;
        }

        public static bool VerificaSettimaneDirittoConvenzioneCanada(byte? codiceConvenzione, string codiceStatoEE, int? settimane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (codiceStatoEE == "24" && codiceConvenzione == 59 && settimane.GetValueOrDefault() < 52)
            {
                messaggioVideo = "Le Settimane Diritto devono essere maggiori o uguali a 52.";
                return false;
            }
            return true;
        }

        public static bool VerificaSettimaneDirittoConvenzioneRegnoUnito(byte? codiceConvenzione, string codiceStatoEE, int? settimane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (codiceStatoEE == "04" && codiceConvenzione == 60 && settimane.GetValueOrDefault() < 52)
            {
                messaggioVideo = "Le Settimane Diritto devono essere maggiori o uguali a 52.";
                return false;
            }
            return true;
        }

        #region PCIPL35
        /// <summary>
        /// Verifica se il codice stato estero è in convenzione. Rif. PCIPL35
        /// </summary>
        /// <param name="codiceStato">Il codice dello stato</param>
        /// <returns>False se non è in convenzione</returns>
        public static bool VerificaStatoEsteroInConvenzione(int codiceStato)
        {
            if (codiceStato > 60)
                return false;
            return true;
        }

        /// <summary>
        /// Verifica se l'istituzione del Lussemburgo è uguale a 1 nel caso in cui la causa carico sia diversa da 2. Rif. PCIPL35
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceStato"></param>
        /// <param name="istituzione"></param>
        /// <returns>False se la causa carico non è due e l'istituzione del Lussemburgo non è 1</returns>
        /// ENG - False se la causa carico non è due e l'istituzione del Lussemburgo non è 1, 2, 3, 4, 5, 501, 502, 503
        public static bool VerificaIstituzioneLussemburgo(byte? causaCarico, int codiceStato, int istituzione)
        {
            if (causaCarico.HasValue && causaCarico.Value != 2)
                if (codiceStato == 6)
                {
                    switch (istituzione)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 501:
                        case 502:
                        case 503:
                            break;
                        default:
                            return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// Controlli sulla Turchia. Rif. PCIPL35
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decPensione"></param>
        /// <returns></returns>
        public static bool ControlliTurchia(List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione, byte? codiceConvenzione, DateTime? decPensione, string cittadinanza, int index, int codiceStato, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciTurchia = listaCodiciConvenzione != null && listaCodiciConvenzione.Count > 0 ? listaCodiciConvenzione.FindAll(x => x.CodiceStato == "36") : null;

            if (codiceConvenzione.HasValue && listaCodiciTurchia != null && listaCodiciTurchia.Count > 0 && listaCodiciTurchia.Exists(x => x.CodiceConvenzione == codiceConvenzione))
            {
                if (decPensione.HasValue && decPensione.Value.CompareTo(new DateTime(1990, 04, 01)) <= 0)
                {
                    messaggioVideo = string.Format("Convenzione {0} (TURCHIA) incompatibile con la decorrenza originaria (inserire una decorrenza successiva al 04/1990)", codiceConvenzione);
                    return false;
                }

                if (cittadinanza != "Z000" && cittadinanza != "Z243")
                {
                    messaggioVideo = string.Format("Convenzione {0} (TURCHIA) incompatibile con cittadinanza (ITALIA - TURCHIA)", codiceConvenzione);
                    return false;
                }

                if (index == 0 && codiceStato != 36)
                {
                    messaggioVideo = string.Format("Convenzione {0} (TURCHIA) incompatibile con il 1° Stato", codiceConvenzione);
                    return false;
                }
            }
            else
            {
                if (codiceConvenzione.HasValue && codiceConvenzione.Value != 12 && codiceStato == 36)
                {
                    messaggioVideo = "Convenzione " + codiceConvenzione + " incompatibile con Stato TURCHIA";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Rif. PCIPL35
        /// </summary>
        /// <param name="stato"></param>
        /// <param name="decPensione"></param>
        /// <param name="codiceConvenzione"></param>
        /// <returns></returns>
        public static bool VerificaSloveniaWithDecPensione(int codiceStato, DateTime? decPensione, byte? codiceConvenzione)
        {
            if (codiceStato == 38 && decPensione.HasValue && decPensione.Value.CompareTo(new DateTime(2004, 05, 01)) >= 0 && codiceConvenzione.HasValue && codiceConvenzione.Value != 12)
                return false;
            return true;
        }

        /// <summary>
        /// Rif. PCIPL35
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="gruppo"></param>
        /// <param name="cittadinanza"></param>
        /// <returns></returns>
        public static bool VerificaSloveniaWithCittadinanza(byte? codiceConvenzione, string cittadinanza, DateTime? decorrenzaDiretta)
        {
            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 38)
            {
                if (!decorrenzaDiretta.HasValue || Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(2002, 08, 01)))
                {
                    if (!string.IsNullOrEmpty(cittadinanza) && cittadinanza != "Z000" && cittadinanza != "Z150")
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Rif. PCIPL35
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="gruppo"></param>
        /// <param name="cittadinanza"></param>
        /// <returns></returns>
        public static bool VerificaCroaziaWithCittadinanza(byte? codiceConvenzione, string cittadinanza, DateTime? decorrenzaDiretta)
        {
            if (codiceConvenzione.HasValue && codiceConvenzione.Value == 39)
            {
                if (!decorrenzaDiretta.HasValue || Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(2003, 11, 01)))
                {
                    if (!string.IsNullOrEmpty(cittadinanza) && cittadinanza != "Z000" && cittadinanza != "Z149")
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Restituisce il codice convenzione corrispondente allo stato estero
        /// </summary>
        /// <param name="codiceStato"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codiceIstituzione"></param>
        /// <returns></returns>

        public static bool IsStatoConAltraConvenzionePresente(GestionePensione.DatiPensione datiPensione, string codiceStato, byte? codiceConvenzione)
        {
            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaStatiByConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaStatiByConvenzione(codiceConvenzione, out listaStatiByConvenzione);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

            bool isStatoPresente = listaStatiByConvenzione != null && listaPrestazioniEstere != null &&
                                   listaStatiByConvenzione.Exists(x => listaPrestazioniEstere.Exists(y => y.CodiceStatoEE == x.CodiceStato));

            if (!(listaCodiciConvenzione != null && listaCodiciConvenzione.FirstOrDefault(x => x.CodiceConvenzione == codiceConvenzione).IsConvenzioneConAltroStato && !isStatoPresente))
                return false;

            return true;

        }

        public static bool VerificaCodiceConvenzioneWithStatoEstero(GestionePensione.DatiPensione datiPensione, string codiceStato, byte? codiceConvenzione, string gruppo)
        {
            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

            if (listaCodiciConvenzione == null || !listaCodiciConvenzione.Exists(x => x.CodiceConvenzione == codiceConvenzione) ||
                IsStatoConAltraConvenzionePresente(datiPensione, codiceStato, codiceConvenzione))
            {
                if (!(gruppo == "0003" && (codiceStato == "09" || codiceStato == "20" || codiceStato == "27" || codiceStato == "29")))
                    return false;
            }

            return true;
        }

        public static string VerificaDecorrenzaCodiceConvenzioneWithStatoEstero(GestionePensione.DatiPensione datiPensione, DateTime? decorrenza, string codiceStato, byte? codiceConvenzione)
        {
            string maxDate = null;

            if (Utility.IsDomandaPL(datiPensione) && codiceConvenzione.HasValue && decorrenza.HasValue) 
            {
                if (codiceStato == "59" && codiceConvenzione.Value == 61 && Utility.DataStrettamenteSuccessivaA(new DateTime(2025, 07, 01), decorrenza.Value))
                    maxDate = "7/2025";

                if (codiceStato == "60" && codiceConvenzione.Value == 62 && Utility.DataStrettamenteSuccessivaA(new DateTime(2025, 09, 01), decorrenza.Value))
                    maxDate = "9/2025";

            }
            return maxDate;
        }
        #endregion PCIPL35

        #region PCIPL25
        public static bool VerificaStatiEsteriWithDanteCausa(DateTime? decorrenza, byte? convenzione, int codiceStato, int? contributiEEDecorrenzaOriginaria, decimal? importoPrestazioneEE)
        {
            if (convenzione.GetValueOrDefault() == 12 && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 12, 31)))
            {
                if (codiceStato > 0 && !contributiEEDecorrenzaOriginaria.HasValue && importoPrestazioneEE.HasValue)
                {
                    if (codiceStato == 9 || codiceStato == 20 || codiceStato == 29 || codiceStato == 40 || codiceStato == 41)
                        return false;
                }
            }

            return true;
        }

        public static bool VerificaConvenzioneWithDecorrenzaDiretta(byte? convenzione, DateTime? decorrenza, int codiceStato)
        {
            if (convenzione.GetValueOrDefault() != 12)
            {
                if ((codiceStato == 27 && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1995, 04, 30))) ||
                    (codiceStato == 9 && Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1994, 01, 01))) ||
                    (codiceStato == 20 && Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1994, 01, 01))) ||
                    (codiceStato == 29 && Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1994, 01, 01))) ||
                    (codiceStato == 17 && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(2002, 05, 31))))
                    return false;
            }

            return true;
        }
        #endregion PCIPL25

        #region PCIPL21
        /// <summary>
        /// Se IW1CARIC non è uguale a 2  e  non è uguale a 5  e  non è uguale a 9, eseguire i seguenti controlli: Se IMPESTL(1) è maggiore di 0 (zero)  oppure  DECESTL(1) è 
        /// maggiore di 0 (zero), impostare TIPO-ERRORE con “91”, MESSAGGIO-ERRORE con “PANNELLO CNV09-BIS NON DEVE ESSERE COMPILATO”, FLAG-ERR con 1 ed uscire da CONTROLLI-0;
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <returns></returns>
        public static bool VerificaImportiEsteriWithCausaCarico(byte? causaCarico, decimal? importoPrestazioneEE, DateTime? decorrenzaPrestazioneEE)
        {
            if (causaCarico.HasValue && causaCarico.Value != 2 && causaCarico.Value != 5 && causaCarico.Value != 9)
                if (importoPrestazioneEE.HasValue || decorrenzaPrestazioneEE.HasValue)
                    return false;

            return true;
        }

        /// <summary>
        /// Se DEC(1  1) è uguale a 0 (zero)  e  DEC(2  1) è uguale a 0 (zero)  e  DEC(3  1) è uguale a 0 (zero)  e  DEC(5  1) è uguale a 0 (zero), eseguire i seguenti controlli: 
        /// Se IMPESTL(1) è maggiore di 0 (zero)  oppure  DECESTL(1) è maggiore di 0 (zero), impostare TIPO-ERRORE con “92”, MESSAGGIO-ERRORE con “PANNELLO CNV09-BIS NON DEVE ESSERE 
        /// COMPILATO”, FLAG-ERR con 1 ed uscire da CONTROLLI-0;
        /// </summary>
        /// <param name="decorrenzaPrestazioneEEProrata"></param>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaPrestazioneEEValuta"></param>
        /// <returns></returns>
        public static bool VerificaImportiEsteriWithPrestazioniEE(DateTime? decorrenzaPrestazioneEEProrata, decimal? importoPrestazioneEE, DateTime? decorrenzaPrestazioneEEValuta)
        {
            if (!decorrenzaPrestazioneEEProrata.HasValue)
                if (importoPrestazioneEE.HasValue || decorrenzaPrestazioneEEValuta.HasValue)
                    return false;

            return true;
        }

        /// <summary>
        /// Se ( IW1CARIC è uguale a 5  oppure è uguale a 9  oppure è uguale a 2 )  e  ( IDAPLIQ(1) è uguale a 0 (zero)  e  IDAPLIQ(2) è uguale a 0 (zero)  e  IDAPLIQ(3) è uguale a 0 
        /// (zero)  e  IDAPLIQ(4) è uguale a 0 (zero) ), eseguire i seguenti controlli:
        /// Se IMPESTL(1) è maggiore di 0 (zero)  oppure  DECESTL(1) è maggiore di 0 (zero), impostare TIPO-ERRORE con “93”, MESSAGGIO-ERRORE con “PANNELLO CNV09-BIS NON DEVE ESSERE 
        /// COMPILATO”, FLAG-ERR con 1 ed uscire da CONTROLLI-0;
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="decorrenzaLiquidazioneStatoEE"></param>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <returns></returns>
        public static bool VerificaImportiEsteriWithDecPrecLiquidata(byte? causaCarico, DateTime? decorrenzaLiquidazioneStatoEE, decimal? importoPrestazioneEE, DateTime? decorrenzaPrestazioneEE)
        {
            if (causaCarico.GetValueOrDefault() == 5 || causaCarico.GetValueOrDefault() == 9 || causaCarico.GetValueOrDefault() == 2)
                if (!decorrenzaLiquidazioneStatoEE.HasValue)
                    if (importoPrestazioneEE.HasValue || decorrenzaPrestazioneEE.HasValue)
                        return false;

            return true;
        }

        /// <summary>
        /// Se ( IW1CARIC è uguale a 9  e  ICI2CONV è uguale a 12 )  oppure  ( IW1CARIC è uguale a 2  e  ICICONV2 è uguale a 12 ), eseguire i seguenti controlli:
        /// Se ICODRES(1) è uguale a “I  ”  e ICODRES(2) è uguale a SPAZI, eseguire i seguenti controlli:
        /// Se IMPESTL(1) è maggiore di 0 (zero)  oppure  DECESTL(1) è maggiore di 0 (zero), impostare TIPO-ERRORE con “94”, MESSAGGIO-ERRORE con “PANNELLO CNV09-BIS NON DEVE 
        /// ESSERE COMPILATO”, FLAG-ERR con 1 ed uscire da CONTROLLI-0;
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="listaResidenzeEstero"></param>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <returns></returns>
        public static bool VerificaImportiEsteriWithConvenzione(byte? causaCarico, byte? codiceConvenzione, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstero, decimal? importoPrestazioneEE, DateTime? decorrenzaPrestazioneEE)
        {
            if (codiceConvenzione.GetValueOrDefault() == 12 && (causaCarico.GetValueOrDefault() == 2 || causaCarico.GetValueOrDefault() == 9))
                if (listaResidenzeEstero != null && listaResidenzeEstero.Count > 0)
                    if (listaResidenzeEstero.First().CodCatastaleStatoEE == "Z000" && listaResidenzeEstero.Count == 1)
                        if (importoPrestazioneEE.HasValue || decorrenzaPrestazioneEE.HasValue)
                            return false;

            return true;
        }

        /// <summary>
        /// Verifica che siano presenti contemporaneamente i dati di Importi Esteri
        /// </summary>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaImportiEsteri(decimal? importoPrestazioneEE, DateTime? decorrenzaPrestazioneEE)
        {
            if ((!decorrenzaPrestazioneEE.HasValue || !importoPrestazioneEE.HasValue) && (decorrenzaPrestazioneEE.HasValue || importoPrestazioneEE.HasValue))
                return false;

            return true;
        }

        /// <summary>
        /// Se DECESTL(INDICE) è maggiore di 199212, impostare TIPO-ERRORE con “06”, COL-ERR1 con 1, MESSAGGIO-ERRORE con “DECORRENZA POSTERIORE AL 12.92”, FLAG-ERR con 1 ed 
        /// uscire da CONTROLLI-1;
        /// </summary>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataDecorrenzaImportiEsteri(DateTime? decorrenzaPrestazioneEE)
        {
            if (decorrenzaPrestazioneEE.HasValue)
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaPrestazioneEE.Value, new DateTime(1992, 12, 31)))
                    return false;

            return true;
        }

        /// <summary>
        /// Se DECESTL(1) non è uguale a DEC(1  1)  e non è uguale a DEC(2  1)  e  non è uguale a DEC(3  1)  e  non è uguale a DEC(4  1), impostare TIPO-ERRORE con “09”, 
        /// COL-ERR1 con 1, MESSAGGIO-ERRORE con “DECORRENZA DIVERSA DA DECORRENZA ESTERO (PANN.CNV01)”, FLAG-ERR con 1 ed uscire da CONTROLLI-1;
        /// </summary>
        /// <param name="decorrenzaPrestazioneEEValuta"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriWithDecorrenzaPrestazioniEE(DateTime? decorrenzaPrestazioneEEValuta, DateTime? decorrenzaPrestazioneEE)
        {
            if (decorrenzaPrestazioneEEValuta.HasValue)
            {
                if (decorrenzaPrestazioneEE.HasValue)
                {
                    if (!decorrenzaPrestazioneEEValuta.Value.Equals(decorrenzaPrestazioneEE.Value))
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Se DEC(INDICE  1) è maggiore di 0 (zero)  e  DEC(INDICE  1) è minore di DECESTL(1)  e  IDAPLIQ(INDICE) è maggiore di 0 (zero),  impostare TIPO-ERRORE con “10”, 
        /// COL-ERR1 con 1, MESSAGGIO-ERRORE con “DECORRENZA MAGGIORE DI DECORRENZA STATO ” + STATO(1) + “/” + ISTIT(1) + “   (CNV01)”, FLAG-ERR con 1 ed uscire da CONTROLLI-1;
        /// </summary>
        /// <param name="decorrenzaPrestazioneEEValuta"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="dataPrecedenteLiquidazione"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriWithPrestazioniEE(DateTime? decorrenzaPrestazioneEEValuta, DateTime? decorrenzaPrestazioneEE, DateTime? dataPrecedenteLiquidazione)
        {
            if (decorrenzaPrestazioneEEValuta.HasValue)
                if (decorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(decorrenzaPrestazioneEE.Value, decorrenzaPrestazioneEEValuta.Value) && dataPrecedenteLiquidazione.HasValue)
                    return false;

            return true;
        }

        /// <summary>
        /// CICLA-4.
        /// </summary>
        /// <param name="listaImportiEsteriValuta"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaImportiEsteri(List<GestioneContrib.PensioniCiImportiValuta> listaImportiEsteriValuta, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, DateTime? dataMin, bool decIsGreaterThan90, bool decIsGreaterThan91, int nStati, int stato, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int index = 0;
            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importoEstero in listaImportiEsteri)
            {
                if (!Utility.DataSuccessivaA(importoEstero.DecorrenzaPrestazioneEE.Value, new DateTime(1993, 01, 01)))
                {
                    bool uguale = false;

                    foreach (GestioneContrib.PensioniCiImportiValuta importoEsteroValuta in listaImportiEsteriValuta)
                    {
                        if (importoEstero.DecorrenzaPrestazioneEE.Equals(importoEsteroValuta.DecorrenzaPrestazioneEE))
                        {
                            if (Utility.DataStrettamenteSuccessivaA(importoEsteroValuta.DecorrenzaPrestazioneEE.Value, dataMin.Value))
                            {
                                messaggioVideo = "Decorrenza Importo Estero " + String.Format("{0:MM/yyyy}", importoEsteroValuta.DecorrenzaPrestazioneEE.Value) + " non deve essere acquisita";
                                return false;
                            }

                            if (!importoEsteroValuta.ImportoPrestazioneEE.HasValue)
                            {
                                messaggioVideo = "Importo Estero alla Decorrenza " + String.Format("{0:MM/yyyy}", importoEsteroValuta.DecorrenzaPrestazioneEE.Value) + " mancante";
                                return false;
                            }

                            uguale = true;
                            break;
                        }
                    }

                    if (uguale)
                        continue;

                    if (Utility.DataSuccessivaA(importoEstero.DecorrenzaPrestazioneEE.Value, new DateTime(1930, 01, 01)) && !Utility.DataSuccessivaA(importoEstero.DecorrenzaPrestazioneEE.Value, new DateTime(1993, 01, 01)))
                    {
                        if (stato == 0)
                        {
                            if (Utility.DataSuccessivaA(importoEstero.DecorrenzaPrestazioneEE.Value, new DateTime(1992, 01, 01)) && decIsGreaterThan91)
                                continue;

                            if (Utility.DataSuccessivaA(importoEstero.DecorrenzaPrestazioneEE.Value, new DateTime(1991, 01, 01)) && decIsGreaterThan90)
                                continue;

                            messaggioVideo = "Decorrenza Importo Estero (" + String.Format("{0:MM/yyyy}", importoEstero.DecorrenzaPrestazioneEE.Value) + ") Stato " + (index + 1) + " mancante";
                            return false;
                        }
                    }
                    else
                    {
                        messaggioVideo = "Decorrenza Importo Estero (" + String.Format("{0:MM/yyyy}", importoEstero.DecorrenzaPrestazioneEE.Value) + ") Primo Stato mancante";
                        return false;
                    }
                }

                index++;
            }

            return true;
        }

        #endregion PCIPL21

        #region PCIPL40

        /// <summary>
        /// Se il campo W-DEORIG > 196804 AND minore 198101 AND IABREMSVV  (R.M.S CON VV)  >  125,1582 allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"
        /// Se il campo W-DEORIG > 198012 AND minore 198301 AND IABREMSVV  (R.M.S CON VV)  >  183,7394 allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                                                                                 
        /// Se il campo W-DEORIG > 198212 AND minore 198401 AND IABREMSVV  (R.M.S CON VV)  >  201,3284 allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                                                                                 
        /// Se il campo W-DEORIG > 198312 AND minore 198501 AND IABREMSVV  (R.M.S CON VV)  >  211,2604 allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                                                                                 
        /// Se il campo W-DEORIG > 198412 AND minore 198601 AND IABREMSVV  (R.M.S CON VV)  >  317,8194  allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                                                                                 
        /// Se il campo W-DEORIG > 198512 AND minore 198701 AND IABREMSVV  (R.M.S CON VV)  >  345,6982  allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                      
        /// Se il campo W-DEORIG > 198612 AND minore 198801 AND IABREMSVV  (R.M.S CON VV)  >  365,3634  allora segnala errore  "R.M.S. SUPERIORE AL TETTO ALLA DECORRENZA  PENSIONE"                      
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMSQuotaAWithDecorrenze(DateTime? decorrenza, decimal? rmsQuotaA, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1968, 04, 30)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1981, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 125.1582M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1980, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1983, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 183.7394M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1982, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1984, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 201.3284M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1983, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1985, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 211.2604M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1984, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1986, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 317.8194M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1985, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1987, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 345.6982M) ||
                (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1986, 12, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1988, 01, 01)) && rmsQuotaA.HasValue && rmsQuotaA.Value > 365.3634M))
            {
                messaggioVideo = "R.M.S. Quota A superiore al tetto alla decorrenza pensione";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se il campo INIASS  > 19921231 allora controlla se il campo IW1NSOBG  > 0  allora segnala errore "OBG MISURA AL 12/92 INCOMPATIBILI CON INIZIO ASSICURAZIONE"        
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="nSettimaneQuotaA"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaNSettimaneQuotaAWithInizioAssicurazione(int? nSettimaneQuotaA, DateTime? dataInizioAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataInizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1992, 12, 31)))
            {
                if (nSettimaneQuotaA.HasValue)
                {
                    messaggioVideo = "OBG Misura al 12/92 incompatibili con Inizio Assicurazione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  (IW1DEOP > 0  AND IW1DEOP   minore 196805 AND TP1CERTD = 0) OR (IW1DEOP = 0  AND IW1DEORIG minore 196805 AND TP1CERTD = 0) OR (IW1DIRET minore 196805 AND 
        /// TP1CERTD > 0)  allora controlla se i campi  (IABREMSVV > 0 AND IW1DEOP = 0)  allora segnala errore "R.M.S. ERRATA PER DECORRENZA ANTE 05/1968" 
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="certificatoPensioneDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMSQuotaAWithOpzioneAndDanteCausa(int categoria, DateTime? decorrenzaOpzione, int? certificatoPensioneDiretta, DateTime? decorrenzaOriginaria, DateTime? decorrenzaDiretta, decimal? rmsQuotaA, DateTime? dataInizioAssicurazione, DateTime? dataMorteDC, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((decorrenzaOpzione.HasValue && !Utility.DataSuccessivaA(decorrenzaOpzione.Value, new DateTime(1968, 05, 01)) && certificatoPensioneDiretta.GetValueOrDefault() == 0) ||
                (!decorrenzaOpzione.HasValue && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1968, 05, 01)) && certificatoPensioneDiretta.GetValueOrDefault() == 0) ||
                (decorrenzaDiretta.HasValue && !Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(1968, 05, 01)) && certificatoPensioneDiretta.GetValueOrDefault() > 0))
            {
                if (rmsQuotaA.GetValueOrDefault() > 0 && !decorrenzaOpzione.HasValue)
                {
                    messaggioVideo = "R.M.S. errata per decorrenza ante 05/1968";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se I campi W-DEORIG > 196805 AND minore 197608 allora controlla se i campi  (IW1CATPEN = 6 AND TP1CERTD = 0   AND IW1DMOR minore 19680501) continua altrimenti segnala 
        /// errore   "SE DECORR. TRA 05/68 E 07/76 DEVE PRESENTE IVS ED RMS" 
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="certificatoPensioneDiretta"></param>
        /// <param name="dataMorteDC"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaRMSQuotaAWithDecorrenze(int categoria, decimal? rmsQuotaA, DateTime? decorrenza, int? certificatoPensioneDiretta, DateTime? dataMorteDC, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1968, 05, 31)) && !Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1976, 08, 01)))
            {
                if (rmsQuotaA.GetValueOrDefault() == 0)
                {
                    if (!(categoria == 6 && certificatoPensioneDiretta.GetValueOrDefault() == 0 && dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(1968, 05, 01))))
                    {
                        messaggioVideo = "Se Decorrenza tra 05/68 e 07/76 devono essere presenti IVS ed RMS";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IW1RETOBG > 0  allora controlla se i campi IW1STOBG = 0 AND ICI1VVOBG = 0 AND ICI2SETFIT = 0 allora segnala errore "R.M.S. D.L.503/92 PRESENTE, MA CONTRIBUTI  
        /// MANCANTI"               
        /// Se i campi  (IW1STOBG = 0 AND ICI1VVOBG = 0)  AND ICIMMF   > 0 allora segnala errore "INCOMPATIBILITA' TRA RMS 503/92 E RELATIVI  CONTRIBUTI"                                                     
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="cmsm"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMSQuotaBWithSettimane(int? contributiItalianiEdEsteriAl1295, int settimaneQuotaCTotale, int settimaneQuotaDTotale, decimal? rmsQuotaB, int? settimaneRetributiveQuotaB, int? vvMisuraDL50392, int? settimaneFittizie, decimal? cmsm, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (rmsQuotaB.GetValueOrDefault() > 0)
            {
                if (!(contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 935 && (settimaneQuotaCTotale > 0 || settimaneQuotaDTotale > 0)))
                {
                    if (settimaneRetributiveQuotaB.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0 && settimaneFittizie.GetValueOrDefault() == 0)
                    {
                        messaggioVideo = "R.M.S. D.L.503/92 presente, ma contributi mancanti";
                        return false;
                    }
                }
                if (settimaneRetributiveQuotaB.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0 && cmsm.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Incompatiblitta' tra RMS 503/92 e relativi contributi";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Se i campi  W-DEORIG > 199301 AND FINASS minore 19930101 allora controlla se il campo ( IABCONA2   =  "1" OR "2" allora controlla se (IABCONA4 = "J" OR "K" OR "Q" OR "W" 
        /// OR "X" OR "Y"  OR "P" OR "O" ) continua altrimenti controlla se i campi IW1RETOBG > 0  OR  IW1STOBG > 0  OR  ICI1VVOBG > 0   allora segnala errore    STRING "R.M.S., OBG 
        /// E VV D.L.503/92 NON  DEBBONO ESSERE ACQUISITI"  altrimenti controlla se ( IABCONA2   NOT =  "3" AND "4" ) allora controlla se i campi IW1RETOBG > 0 OR IW1STOBG > 0 OR 
        /// ICI1VVOBG > 0  segnala errore STRING "R.M.S., OBG E VV D.L.503/92 NON DEBBONO ESSERE ACQUISITI"   
        /// Muove spazio nel campo PREPEN (campi di comodo per prepensionamento).
        /// Se i campi  W-DEORIG > 199301 AND FINASS minore 19930101 allora controlla se il campoF IABCONA2 = "1" OR "2"   allora controlla se il campo  (IABCONA4 = "J" OR "K" OR "Q" 
        /// OR "W" OR "X" OR "Y" OR "P" OR "O")  allora muove “S” nel campo   PREPEN e controlla se il campo IABREMSVV = 0  allora segnala errore  "R.M.S AL 12/92 MANCANTE"  
        /// Se il campo IW1RETOBG = 0 allora segnala errore  "R.M.S D.L.503/92 MANCANTE"   
        /// Se i campi IW1NSOBG = 0 AND IW1VVMISURA = 0 allora segnala errore STRING "SETTIMANE MISURA (OBG O V.V.) AL  12/92 MANCANTI"            
        /// Se i campi IW1STOBG > 0 OR ICI1VVOBG > 0   allora segnala errore "SETTIMANE OBG E VV D.L.503/92 NON DEBBONO ESSERE ACQUISITI"  altrimenti se il campo IABCONA2 = "3" OR 
        /// "4"     allora muovi "S"  nel campo PREPEN  e controlla se il campo IABREMSVV = 0 allora segnala errore "R.M.S AL 12/92 MANCANTE"
        /// Se i campi  W-DEORIG minore 199601 AND IW1RETOBG = 0 AND ICI2SETFIT = 0 AND ETA-PENS NOT = "S" allora segnala errore "R.M.S D.L.503/92 MANCANTE"  
        /// Se i campi IW1NSOBG = 0 AND IW1VVMISURA = 0 allora segnala errore "SETTIMANE MISURA (OBG O V.V.) AL  12/92 MANCANTI"                                            
        /// Se i campi IW1STOBG > 0 OR ICI1VVOBG > 0 allora segnala errore "SETTIMANE OBG E VV D.L.503/92 NON DEBBONO ESSERE ACQUISITI"        
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="codNatura"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="gruppo"></param>
        /// <param name="dataNascitaDC"></param>
        /// <param name="sessoDC"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsRMSWithDecorrenzaAndAssicurazioneAndCodNatura(DateTime? decorrenza, DateTime? dataFineAssicurazione, string codNatura, decimal? rmsQuotaB, int? settimaneRetributiveQuotaB, int? vvMisuraDL50392, decimal? rmsQuotaA, int? settimaneRetributiveQuotaA, int? vvMisuraAl1292, int? settimaneFittizie, string gruppo, DateTime? dataNascitaDC, char? sessoDC, DateTime? dataNascitaTitolare, char? sessoTitolare, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<string> codiciNatura = new List<string> { "J", "K", "Q", "W", "X", "Y", "P", "O" };

            if (decorrenza.HasValue && dataFineAssicurazione.HasValue)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 31)) && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
                {
                    if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
                    {
                        if (!codiciNatura.Contains(codNatura.Substring(2, 1)))
                        {
                            if ((rmsQuotaB.GetValueOrDefault() > 0 || settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0) &&
                                 !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Rms_Obg_VV_Non_Acquisiti_CI.RMS_OBG_VV_NON_ACQUISITI))
                            {
                                messaggioVideo = "R.M.S., OBG e VV D.L.503/92 non devono essere acquisiti";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4"))) &&
                            !GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Rms_Obg_VV_Non_Acquisiti_CI.RMS_OBG_VV_NON_ACQUISITI))
                        {
                            if (rmsQuotaB.GetValueOrDefault() > 0 || settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0)
                            {
                                messaggioVideo = "R.M.S., OBG e VV D.L.503/92 non devono essere acquisiti";
                                return false;
                            }
                        }
                    }
                }

                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 31)) && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
                {
                    if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
                    {
                        if (codiciNatura.Contains(codNatura.Substring(2, 1)))
                        {
                            if (rmsQuotaA.GetValueOrDefault() == 0)
                            {
                                messaggioVideo = "R.M.S al 12/92 mancante";
                                return false;
                            }

                            if (rmsQuotaB.GetValueOrDefault() == 0)
                            {
                                messaggioVideo = "R.M.S D.L.503/92 mancante";
                                return false;
                            }

                            if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0 && vvMisuraAl1292.GetValueOrDefault() == 0)
                            {
                                messaggioVideo = "Settimane Misura (OBG o V.V.) al 12/92 mancanti";
                                return false;
                            }

                            if (settimaneRetributiveQuotaB.GetValueOrDefault() > 0 && vvMisuraDL50392.GetValueOrDefault() > 0)
                            {
                                messaggioVideo = "Settimane OBG e VV D.L.503/92 non devono essere acquisiti";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("3") || codNatura.Substring(0, 1).Equals("4")))
                        {
                            if (rmsQuotaA.GetValueOrDefault() == 0)
                            {
                                messaggioVideo = "R.M.S al 12/92 mancante";
                                return false;
                            }

                            bool eta_Pens = VerificaEtaPensionabileAllaDecorrenza(codNatura, gruppo, dataNascitaDC, sessoDC, dataNascitaTitolare, sessoTitolare, decorrenza);

                            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1996, 01, 01)) && rmsQuotaB.GetValueOrDefault() == 0 && settimaneFittizie.GetValueOrDefault() == 0 && !eta_Pens)
                            {
                                messaggioVideo = "R.M.S D.L.503/92 mancante";
                                return false;
                            }

                            if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0 && vvMisuraAl1292.GetValueOrDefault() == 0)
                            {
                                messaggioVideo = "Settimane Misura (OBG o V.V.) al 12/92 mancanti";
                                return false;
                            }

                            if (settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0)
                            {
                                messaggioVideo = "Settimane OBG e VV D.L.503/92 non devono essere acquisiti";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  APPO-CAT1 = "V" AND  ITOT-EST-95 = 0 AND INIASS    >   19953112 AND DECPERFREQ > 20111231   allora controlla se ( IW1DEORIG  -  IW1NAT6 ) NOT > 7000 allora 
        /// segnala errore "Pens. contributiva (inferiore 70 anni)  temporaneamente sospesa"                  
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="contributiItalianiEdEsteriAl1295"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataPerfezionamentoRequisiti"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>

        /// <summary>
        /// Se i campi  FINASS    > 19951231 AND (OPZIONE-CONTRIBUTIVA NOT = "S" AND IABCONA3 NOT  = "O" ) allora controlla  se i campi  ( IW1STOBG = 0   AND  ICISTOBG335 = 0 AND  
        /// ICISTOBG012 = 0 ) AND (ICI2CONV NOT = 17 AND TP1NUB =  0)  segnala errore  "OBG MISURA 503/92   O   CONTRIBUTI 335/95 MANCANTI"      
        /// Se il campo FINASS minore 19930101 allora controlla se IW1STOBG > 0 OR ICI1VVOBG > 0   segnala errore "OBG E VV D.L.503/92 NON  DEBBONO ESSERE ACQUISITI"    
        /// Se i campi  IW1RETOBG > 0  AND (IABCONA2 NOT = "2" AND "3")  AND ICI2SETFIT = 0 allora segnala errore "R.M.S., OBG E VV D.L.503/92 NON  DEBBONO ESSERE ACQUISITI"                                                       
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="codNatura"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="settimaneContributiveQuotaD"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiWithFineAssicurazione(DateTime? dataFineAssicurazione, string codNatura, int? settimaneRetributiveQuotaB, int? settimaneContributiveQuotaC, int? settimaneContributiveQuotaD, byte? codiceConvenzione, int? nContributiVolontari, int? vvMisuraDL50392, decimal? rmsQuotaB, int? settimaneFittizie, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Rms_Obg_VV_Non_Acquisiti_CI.RMS_OBG_VV_NON_ACQUISITI))
                return true;

            if (dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
            {
                if (settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "OBG e VV D.L.503/92 non devono essere acquisiti";
                    return false;
                }

                if (rmsQuotaB.GetValueOrDefault() > 0 && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("2") && !codNatura.Substring(0, 1).Equals("3"))) && settimaneFittizie.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "R.M.S., OBG e VV D.L.503/92 non devono essere acquisiti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo W-DEORIG minore 199601 allora controlla se i campi ICISTOBG335 > 0 OR ICICONOBG335 > 0 OR ICIRETOBG335 > 0 OR ICISTOBG012 > 0 OR ICICONOBG012 > 0 OR 
        /// ICIRETOBG012 > 0 allora segnala errore "DATI L.335 INCOMPATIBILI CON DECORRENZA"  
        /// Se il campo W-DEORIG > 199600 allora controlla se  ICISTOBG335 > 0 AND (ICICONOBG335  (AMMONTARE DEI CONTRIBUTI OBG ) = 0 OR ICIRETOBG335 (MONTANTE OBG) = 0)   allora 
        /// segnala  errore "DATI L.335 MANCANTI (IMP.CONTRIBUTI / MONTANTE )"      
        /// Se il campo (IABCONA2  = "3" OR "4") allora controlla se il campo FINASS minore 19960101 allora controlla se IF ICISTOBG335 > 0 OR ICICONOBG335 > 0 OR ICIRETOBG335 > 0   
        /// segnala errore "DATI L.335 INCOMPATIBILI CON DATA ULTIMO CONTRIBUTO" 
        /// Se i campi IF FINASS minore 19930101 AND (IW1RETOBG > 0 AND ICIMMF > 0 ) allora segnala errore "R.M.S. DAL 1993 INCOMPATIBILE CON  DATA ULTIMO CONTRIBUTO"    altrimenti  
        /// controlla se i campi  ICISTOBG335 = 0 AND (ICICONOBG335 > 0 OR ICIRETOBG335 > 0)  allora segnala errore "DATI L.335 MANCANTI (INABILITA')"  
        /// Se il campo IW1DEORIG minore 201201 allora controlla se IF (ICIMMF = 0 AND ICISTOBG335 > 0 AND ICI2SETFIT   > 0 ) OR (ICIMMF > 0 AND ICISTOBG335 = 0) allora segnala 
        /// errore  "CMSM INCOMPATIBILE CON SETT.335 O MANCANTE (INABILITA')" altrimenti controlla se il campo (ICIMMF = 0  AND ICI2SETFIT    > 0 ) segnala errore "CMSM  MANCANTE 
        /// (INABILITA')"
        /// Se i campi (ICIRETOBG335 minore ICICONOBG335) allora segnala errore "MONTANTE 335 MINORE DI IMP. CONTRIBUTI 335"
        /// Se il campo (IABCONA2 NOT = "3" AND "4") allora controlla se il campo ICIMMF > 0 segnala errore "CMSM  INCOMPATIBILE CON  CATEGORIA PENSIONE"  
        /// Se il campo ICISTOBG335 > 0   allora controlla se ICICONOBG335 = 0 allora segnala errore "IMPORTO CONTRIBUTI 335 MANCANTE"
        /// Se il campo ICIRETOBG335 = 0 allora segnala errore "MONTANTE 335 MANCANTE"
        /// Se il campo ICIRETOBG335 minore ICICONOBG335 allora segnala errore "MONTANTE 335 MINORE DI IMPORTO CONTRIBUTI 335" altrimenti se il campo ICICONOBG335 > 0 allora segnala 
        /// errore "IMPORTO CONTRIBUTI 335 INCOMPATIBILE CON SETTIMANE 335" altrimenti se il campo ICIRETOBG335 > 0 allora segnala errore "MONTANTE 335 INCOMPATIBILE CON SETTIMANE 
        /// 335"  se il campo ICIMMF > 0 allora segnala errore "CMSM 335 INCOMPATIBILE CON SETTIMANE 335"
        /// Se i campi  W-DEORIG minore 199301 AND FINASS  minore 19930101 AND TP1CERTD = 0  allora segnala errore "DATI D.L.503/92 INCOMPATIBILI CON DATA  ULTIMO CONTRIBUTO"    
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="importoContributivoQuotaC"></param>
        /// <param name="montanteContributivoQuotaC"></param>
        /// <param name="settimaneContributiveQuotaD"></param>
        /// <param name="importoContributivoQuotaD"></param>
        /// <param name="montanteContributivoQuotaD"></param>
        /// <param name="codNatura"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="cmsm"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiWithDecorrenza(DateTime? decorrenza, DateTime? decorrenzaOriginaria, int? settimaneContributiveQuotaC, decimal? importoContributivoQuotaC, decimal? montanteContributivoQuotaC, int? settimaneContributiveQuotaD, decimal? importoContributivoQuotaD, decimal? montanteContributivoQuotaD, string codNatura, DateTime? dataFineAssicurazione, decimal? rmsQuotaB, decimal? cmsm, int? settimaneFittizie, int? certificatoPensioneDiretta, int? settimaneRetributiveQuotaB, int? vvMisuraDL50392, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1996, 01, 01)))
            {
                if (settimaneContributiveQuotaC.GetValueOrDefault() > 0 || importoContributivoQuotaC.GetValueOrDefault() > 0 || montanteContributivoQuotaC.GetValueOrDefault() > 0 || settimaneContributiveQuotaD.GetValueOrDefault() > 0 || importoContributivoQuotaD.GetValueOrDefault() > 0 || montanteContributivoQuotaD.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Dati L.335 incompatibili con Decorrenza";
                    return false;
                }
            }

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1995, 12, 31)))
            {
                if (settimaneContributiveQuotaC.GetValueOrDefault() > 0 && (importoContributivoQuotaC.GetValueOrDefault() == 0 || montanteContributivoQuotaC.GetValueOrDefault() == 0))
                {
                    messaggioVideo = "Dati L.335 mancanti ( Importo Contributi / Montante )";
                    return false;
                }

                if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("3") || codNatura.Substring(0, 1).Equals("4")))
                {
                    if (dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1996, 01, 01)))
                    {
                        if (settimaneContributiveQuotaC.GetValueOrDefault() > 0 || importoContributivoQuotaC.GetValueOrDefault() > 0 || montanteContributivoQuotaC.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "Dati L.335 incompatibili con data Ultimo Contributo";
                            return false;
                        }

                        if (dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)) && rmsQuotaB.GetValueOrDefault() > 0 && cmsm.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "R.M.S. dal 1993 incompatibile con data Ultimo Contributo";
                            return false;
                        }
                    }
                    else
                    {
                        if (settimaneContributiveQuotaC.GetValueOrDefault() == 0 && (importoContributivoQuotaC.GetValueOrDefault() > 0 || montanteContributivoQuotaC.GetValueOrDefault() > 0))
                        {
                            messaggioVideo = "Dati L.335 mancanti (Inabilita')";
                            return false;
                        }

                        if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2012, 01, 01)))
                        {
                            if ((cmsm.GetValueOrDefault() == 0 && settimaneContributiveQuotaC.GetValueOrDefault() > 0 && settimaneFittizie.GetValueOrDefault() > 0) ||
                                (cmsm.GetValueOrDefault() > 0 && settimaneContributiveQuotaC.GetValueOrDefault() == 0))
                            {
                                messaggioVideo = "CMSM incompatibile con Sett.335 o mancante (Inabilita')";
                                return false;
                            }
                        }
                        else
                        {
                            if (cmsm.GetValueOrDefault() == 0 && settimaneFittizie.GetValueOrDefault() > 0)
                            {
                                messaggioVideo = "CMSM  mancante (Inabilita')";
                                return false;
                            }
                        }
                    }

                    if (!VerificaImportoContributivoTotWithMontante(decorrenzaOriginaria, montanteContributivoQuotaC, importoContributivoQuotaC, out messaggioVideo))
                        return false;
                }

                if (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4")))
                {
                    if (cmsm.GetValueOrDefault() > 0)
                    {
                        messaggioVideo = "CMSM incompatibile con Categoria Pensione";
                        return false;
                    }

                    if (settimaneContributiveQuotaC.GetValueOrDefault() > 0)
                    {
                        if (importoContributivoQuotaC.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Importo Contributi 335 mancante";
                            return false;
                        }

                        if (montanteContributivoQuotaC.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Montante 335 mancante";
                            return false;
                        }

                        if (!VerificaImportoContributivoTotWithMontante(decorrenzaOriginaria, montanteContributivoQuotaC, importoContributivoQuotaC, out messaggioVideo))
                            return false;
                    }
                    else
                    {
                        if (importoContributivoQuotaC.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "Importo Contributi 335 incompatibile con Settimane 335";
                            return false;
                        }

                        if (montanteContributivoQuotaC.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "Montante 335 incompatibile con Settimane 335";
                            return false;
                        }

                        if (cmsm.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "CMSM 335 incompatibile con Settimane 335";
                            return false;
                        }
                    }
                }
            }

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 01)) && dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)) && certificatoPensioneDiretta.GetValueOrDefault() == 0)
            {
                if (rmsQuotaB.GetValueOrDefault() > 0 || settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Dati D.L.503/92 incompatibili con data Ultimo Contributo";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se i campi  INIASS  > 19930100 AND GEST233 (1 1) (COD. GESTIONE  SETTIM. ESTERE LEGGE 233, 503, 335 ) = 0   allora controlla se i campi IW1RETOBG > 0 OR IW1STOBG > 0 OR 
        /// ICI1VVOBG > 0  segnala errore "MANCA REGISTRAZIONE A DECORRENZA ORIGINARIA"
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="codiceGestione"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRegistrazioneADecorrenza(DateTime? dataInizioAssicurazione, short? codiceGestione, int? vvMisuraDL50392, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataInizioAssicurazione.HasValue && Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)) && codiceGestione.GetValueOrDefault() == 0)
            {
                if (vvMisuraDL50392.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Manca Registrazione a Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo ICICONOBG012 = 0 allora segnala errore  "(quota D) IMP. CONTRIBUTI 335 MANCANTE"
        /// Se il campo ICIRETOBG012 = 0 allora segnala errore  "(quota D) MONTANTE 335 MANCANTE"
        /// Se il campo ICIRETOBG012 minore ICICONOBG012 allora segnala errore "(quota D) MONT. 335 MINORE DI IMPORTO CONTRIBUTI 335" altrimenti  se il campo ICICONOBG012 > 0 segnala 
        /// errore "(quota D) IMPORTO CONTRIBUTI 335  INCOMPATIBILE CON SETTIMANE 335"  
        /// Se il campo ICIRETOBG012 > 0 segnala errore "(quota D) MONTANTE 335  INCOMPATIBILE CON SETTIMANE 335"
        /// </summary>
        /// <param name="settimaneContributive"></param>
        /// <param name="importoContributivoTotale"></param>
        /// <param name="montanteContributivo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCompletezzaDatiContributiviQuotaD(int? settimaneContributive, decimal? importoContributivoTotale, decimal? montanteContributivo, DateTime? decorrenzaOriginaria,
            out string messaggioVideo)
        {
            messaggioVideo = "";

            if (settimaneContributive.GetValueOrDefault() > 0)
            {
                if (importoContributivoTotale.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "(quota D) Importo Contributi 335 mancante";
                    return false;
                }

                if (montanteContributivo.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "(quota D) Montante 335 mancante";
                    return false;
                }

                if (!VerificaImportoContributivoTotWithMontante(decorrenzaOriginaria, montanteContributivo, importoContributivoTotale, out messaggioVideo))
                {
                    messaggioVideo = "(quota D) " + messaggioVideo;
                    return false;
                }
            }
            else
            {
                if (importoContributivoTotale.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "(quota D) Importo Contributi 335 incompatibile con Settimane 335";
                    return false;
                }

                if (montanteContributivo.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "(quota D) Montante 335 incompatibile con Settimane 335";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// SETT-ESTE-2. IMPORTANTE: Questo metodo non è richiamato, perchè serve una maggiore analisi. Il metodo è incompleto.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="codiceGestioneTraduzioneSuGP"></param>
        /// <param name="decorrenzaContributoEstero"></param>
        /// <param name="settimaneContributoEstero"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="importoContributivoTotaleQuotaC"></param>
        /// <param name="importoContributivoTotaleQuotaD"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codNatura"></param>
        /// <param name="montanteContributivoQuotaC"></param>
        /// <param name="montanteContributivoQuotaD"></param>
        /// <param name="decorrenzaBonus"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsContributiEsteri(int index, short? codiceGestioneTraduzioneSuGP, DateTime? decorrenzaContributoEstero, int? settimaneContributoEstero, DateTime? dataInizioAssicurazione,
            decimal? rmsQuotaB, decimal? importoContributivoTotaleQuotaC, decimal? importoContributivoTotaleQuotaD, DateTime? dataFineAssicurazione, DateTime? decorrenzaDanteCausa, DateTime? decorrenzaOriginaria,
            string codNatura, decimal? montanteContributivoQuotaC, decimal? montanteContributivoQuotaD, DateTime? decorrenzaBonus, DateTime?[] primaDecorrenzaImportiEsteri,
            int? contributiItalianiEdEsteriAl1295, int? settimaneRetributiveQuotaBCodGestione1, int? settimaneVVMisuraAl1292,
            int? sommaGEST_EST_61, int? sommaSettimaneContributiItalianiEdEsteri, int sommaSettimaneDecUgualePrimaDec, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            DateTime? decorrenza = decorrenzaDanteCausa != null ? decorrenzaDanteCausa.Value : decorrenzaOriginaria.Value;
            DateTime dataSistema = Utility.DataSistemaCi;
            bool prepen = false;

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 01)) && dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)) && !string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("3") || codNatura.Substring(0, 1).Equals("4")))
                prepen = true;

            if ((dataInizioAssicurazione.HasValue && Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1996, 01, 01)) && codiceGestioneTraduzioneSuGP.GetValueOrDefault() == 61 && rmsQuotaB.GetValueOrDefault() == 0) ||
                (codiceGestioneTraduzioneSuGP.GetValueOrDefault() != 61 && codiceGestioneTraduzioneSuGP.GetValueOrDefault() != 1))
            {
                messaggioVideo = "Gestione errata o incompatibile con RMS D.L.503/92";
                return false;
            }

            if (codiceGestioneTraduzioneSuGP.GetValueOrDefault() == 1 && importoContributivoTotaleQuotaC.GetValueOrDefault() == 0 && importoContributivoTotaleQuotaD.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Gestione errata o incompatibile con Contributi 335";
                return false;
            }

            if ((codiceGestioneTraduzioneSuGP.GetValueOrDefault() == 61 || codiceGestioneTraduzioneSuGP.GetValueOrDefault() == 1) && dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)) &&
                !prepen && !Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)
                && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione))
            {
                messaggioVideo = "Gestione / Settimane estere non ammesse";
                return false;
            }

            if (codiceGestioneTraduzioneSuGP.GetValueOrDefault() != 1 && codiceGestioneTraduzioneSuGP.GetValueOrDefault() != 61 && codiceGestioneTraduzioneSuGP.GetValueOrDefault() != 71)
            {
                messaggioVideo = "Codice Gestione errato";
                return false;
            }

            short primaCifra = (short)(codiceGestioneTraduzioneSuGP.GetValueOrDefault() / 10);
            //short secondaCifra = (short)(codiceGestioneTraduzioneSuGP.GetValueOrDefault() - (primaCifra * 10));

            if (primaCifra == 6 && rmsQuotaB.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Codice Gestione incompatibile con RMS 503/92";
                return false;
            }

            if (primaCifra == 0 && montanteContributivoQuotaC.GetValueOrDefault() == 0 && montanteContributivoQuotaD.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Codice Gestione incompatibile con Montante 335/95";
                return false;
            }

            if (codiceGestioneTraduzioneSuGP.GetValueOrDefault() > 0 && (!decorrenzaContributoEstero.HasValue || settimaneContributoEstero.GetValueOrDefault() == 0))
            {
                messaggioVideo = "Riga incompleta (manca Decorrenza / Settimane)";
                return false;
            }

            if (decorrenzaContributoEstero.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaContributoEstero.Value, dataSistema.AddMonths(1)))
            {
                messaggioVideo = "Data posteriore alla data odierna più un mese";
                return false;
            }

            if (!decorrenzaContributoEstero.Equals(decorrenza) && !decorrenzaContributoEstero.Equals(decorrenzaOriginaria) && !decorrenzaContributoEstero.Equals(decorrenzaBonus) && dataInizioAssicurazione.HasValue && Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)) && index == 0)
            {
                messaggioVideo = "Decorrenza diversa da 02/93 o Dec.Originaria";
                return false;
            }

            if (!decorrenzaContributoEstero.Equals(decorrenzaOriginaria) && !decorrenzaContributoEstero.Equals(decorrenza) && !decorrenzaContributoEstero.Equals(decorrenzaBonus) &&
                 !primaDecorrenzaImportiEsteri.Contains(decorrenzaContributoEstero))
            {
                messaggioVideo = "Decorrenza diversa da decorrenza originaria o ricalcolo";
                return false;
            }

            if (sommaSettimaneDecUgualePrimaDec != sommaGEST_EST_61 && contributiItalianiEdEsteriAl1295.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Settimane estere diverse da settimane contributive italiane ed estere";
                return false;
            }

            if (decorrenzaContributoEstero.HasValue && (string.IsNullOrEmpty(codNatura) || !codNatura.Substring(1, 1).Equals("O")) && !(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione).GetValueOrDefault() == Utility.TipoAppartenenza.CI && Utility.IsDomandaTipoContributivo(datiPensione, null, true)))
            {
                if ((Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 01)) && dataInizioAssicurazione.HasValue && !Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)) &&
                    dataFineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01))) || prepen)
                {
                    int nSettimane = Utility.NSettimaneBetweenDate(decorrenzaContributoEstero.Value, new DateTime(1993, 01, 01));
                    if (nSettimane < 0)
                        nSettimane = 0;

                    if ((sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault() + settimaneVVMisuraAl1292.GetValueOrDefault()) > nSettimane)
                    {
                        if (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(2, 1).Equals("G") && !codNatura.Substring(2, 1).Equals("Z")) &&
                            !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (datiPensione.SiglaCategoria.Trim() == "VOS" || datiPensione.SiglaCategoria.Trim() == "IOS" || datiPensione.SiglaCategoria.Trim() == "SOS")))
                        {
                            messaggioVideo = "Settimane italiane  + estere D.L.503/92 superiori a capienza nel periodo";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo W-DEORIG >  199600 allora controlla
        /// * se i campi ICIMMF > 0 AND ICI2SETFIT = 0 allora segnala errore "CMSM 335 INCOMPATIBILE CON SETTIMANE  FITTIZIE"
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="cmsm"></param>
        /// <param name="nSettimaneFittiziePrepensionamento"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCMSM(DateTime? decorrenza, decimal? cmsm, int? nSettimaneFittiziePrepensionamento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1996, 01, 01)))
            {
                if (cmsm.GetValueOrDefault() > 0 && nSettimaneFittiziePrepensionamento.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "CMSM 335 incompatibile con Settimane Fittizie";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo W-DEORIG >  199600 allora controlla
        /// * Se il campo FINASS minore 19960101 allora controlla se ICIRETOBG335 > 0 allora controlla se il campo IABCONA3 NOT =  "O"  allora segnala errore "MONTANTE 335 
        /// INCOMPATIBILE  DATA ULTIMO CONTRIBUTO"  
        /// * Se il campo ICIMMF > 0 allora controlla se i campi  W-DEORIG > 199600 AND (IABCONA2 = "3" OR "4") allora continua altrimenti "MONTANTE 335 INCOMPATIBILE DATA ULTIMO 
        /// CONTRIBUTO"     
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaMontante335(DateTime? decorrenza, DateTime? dataFineAssicurazione, decimal? montante, string codNatura, decimal? cmsm, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1996, 01, 01)))
            {
                if (dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1996, 01, 01)))
                {
                    if (montante.GetValueOrDefault() > 0)
                    {
                        if (string.IsNullOrEmpty(codNatura) || !codNatura.Substring(1, 1).Equals("O"))
                        {
                            messaggioVideo = "Montante 335 incompatibile con Data Ultimo Contributo";
                            return false;
                        }
                    }

                    if (cmsm.GetValueOrDefault() > 0)
                    {
                        if (!(string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("3") || codNatura.Substring(0, 1).Equals("4"))))
                        {
                            messaggioVideo = "Montante 335 incompatibile con Data Ultimo Contributo";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo  ITOT-EST-95 > 0  allora controlla se il campo INIASS  NOT > 19951231 segnala errore "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON INIZIO ASSICURAZIONE"
        /// </summary>
        /// <param name="contributiItalianiEdEsteriAl1295"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaOpzioneContributiva(GestionePensione.DatiPensione datiPensione, bool? flagContributiva, int? contributiItalianiEdEsteriAl1295, int settimaneQuotaATotale, int settimaneQuotaBTotale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione))
                if (settimaneQuotaATotale > 0 || settimaneQuotaBTotale > 0)
                {
                    messaggioVideo = "Inserire solo quota contributiva";
                    return false;
                }

            if (flagContributiva.GetValueOrDefault())
            {
                if (settimaneQuotaATotale > 0 || settimaneQuotaBTotale > 0)
                {
                    messaggioVideo = "Inserire solo quota contributiva";
                    return false;
                }

                if (!Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) &&
                    !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                       ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))))
                {
                    if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 0)
                    {
                        messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 incompatibili con opzione contributiva";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo INIASS > 19951231 allora controlla 
        /// * se i campi ITOT-EST-95 > 935 AND (IABCONA2 not = "1" AND IABCONA2  NOT  = "2"  AND  IABCONA3 = "O")  allora controlla 
        ///    * Se il campo  ICISTOBG335  > 0 OR ICICONOBG335 > 0 OR ICIRETOBG335 > 0 allora segnala errore "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.335/95"
        ///    * Se i campi  ( (GEST233(1 1) > 0 AND < 61 ) OR (GEST233(1 2) > 0 AND < 61 ) OR (GEST233(2 1) > 0 AND < 61 )  OR (GEST233(2 2) > 0 AND < 61 )  OR (GEST233(3 1) > 0 
        ///    AND < 61 )  OR (GEST233(3 2) > 0 AND < 61 ) )  segnala  errore  "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON GESTIONE CTR ESTERI  (= 0x)"
        ///    * Se i campi  (IW1STOBG = 0 AND ICI1VVOBG = 0) OR IW1RETOBG = 0  allora segnala errore "CTR ITAL.ESTERI AL 31.12.95  INCOMPATIBILI CON DATI PER L.503/92" 
        ///    * Se i campi  (GEST233(1 1) > 0) AND  (GEST233(1 1) < 61 OR > 65 )  allora segnala errore "CTR ITAL.ESTERI AL 31.12.95  INCOMPATIBILI CON GESTIONE CTR ESTERI (<> 6X)"    
        /// *  Se il campo ITOT-EST-95 < 936 allora controlla 
        ///    *  se il campo  IW1STOBG > 0 OR IW1RETOBG > 0 OR ICI1VVOBG > 0  allora segnala errore "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.503/92"  
        ///    Se i campi ( (GEST233(1 1) > 61 AND < 65) OR (GEST233(1 2) > 61 AND < 65) OR (GEST233(2 1) > 61 AND < 65 ) OR (GEST233(2 2) > 61 AND < 65 ) OR (GEST233(3 1) > 61 AND < 65 ) OR (GEST233(3 2) > 61 AND < 65 ) allora segnala errore  "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON GESTIONE CTR ESTERI (= 6X)" 
        ///    *  Se il campo  ICISTOBG335  = 0 OR ICIRETOBG335 = 0  allora segnala errore "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.335/95" 
        ///    *  Se i campi (GEST233(1 1) NOT = 1) AND  ART48(1) NOT = "S"  allora segnala "CTR ITAL.ESTERI AL 31.12.95 INCOMP. CON  CTR ESTERI  (GEST 01 MANCANTE)"   
        /// *  Se il campo ITOT-EST-95 = 0 allora controlla se i campi  APPO-CAT1 = "V" AND  (IABCONA2 = "1" OR "2") allora segnala errore "CTR ITALIANI ED ESTERI AL 31.12.95 MANCANTI PER PENS. DI ANZIANITA'" 
        /// IMPORTANTE: Per testare questo controllo serve una domanda sperimentale donna
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="contributiItalianiEdEsteriAl1295"></param>
        /// <param name="codNatura"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="importoContributivoTotaleQuotaC"></param>
        /// <param name="montanteQuotaC"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>

        public static bool ControlsContributiItalianiEsteriAl1295(GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaOriginaria, DateTime? dataNascitaTitolare, DateTime? dataMorteDC, DateTime? decorrenzaOpzione, DateTime? dataPerfezionamentoRequisiti, bool? flagContributiva, int? contributiItalianiEdEsteriAl1295, int? settimane707QuotaBTotali, decimal rmsQuotaBTotale, decimal rmsQuotaATotale, int settimaneQuotaATotale, int settimaneQuotaBTotale, int settimaneQuotaCTotale, int settimaneQuotaDTotale, int? certificatoPensioneDiretta, int categoria, string codNatura, string gruppo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (Utility.IsDomandaVecchiaiaPL(datiPensione) && (categoria == 4 || categoria == 88 || categoria == 91 || categoria == 85))
            {
                if (!contributiItalianiEdEsteriAl1295.HasValue || contributiItalianiEdEsteriAl1295.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Contributi italiani ed esteri al 95 uguali a 0. Prodotto di domanda errato, utilizzare il prodotto specifico \"pensione di vecchiaia sistema calcolo contributivo\".";
                    return false;
                }
            }
            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione) || flagContributiva.GetValueOrDefault())
                return true;

            if ((settimaneQuotaCTotale > 0 || settimaneQuotaDTotale > 0) && !contributiItalianiEdEsteriAl1295.HasValue &&
                datiPensione.InizioAssicurazione.HasValue && !Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.GetValueOrDefault(), new DateTime(1995, 12, 31)))
            {
                messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 obbligatori";
                return false;
            }

            if (!Utility.IsDomandaTipoContributivo(datiPensione, null, false) && contributiItalianiEdEsteriAl1295.GetValueOrDefault() == 0 && (settimaneQuotaCTotale > 0 || settimaneQuotaDTotale > 0) && settimaneQuotaATotale == 0 && settimaneQuotaBTotale == 0)
            {
                if ((gruppo.Equals("0001") && dataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(dataPerfezionamentoRequisiti.Value, new DateTime(2011, 12, 31))) &&
                    (Utility.DifferenzaBetweenDate(decorrenzaOriginaria.Value, new DateTime(dataNascitaTitolare.Value.Year, dataNascitaTitolare.Value.Month, 01), Utility.TipoAppartenenza.CI)).Year <= 70)
                {
                    messaggioVideo = "Pensione contributiva (inferiore 70 anni) temporaneamente sospesa";
                    return false;
                }
            }


            if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 935)
            {
                if ((settimaneQuotaCTotale > 0 || settimaneQuotaDTotale > 0) && (rmsQuotaBTotale == 0 || settimane707QuotaBTotali == 0) && !Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) &&
                    !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                       ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))))
                {
                    if (!(categoria == 5 || categoria == 86 || categoria == 89 || categoria == 92))
                    {
                        messaggioVideo = "E' obbligatorio inserire le sett.707 per la quota B e Reddito/retr.media";
                        return false;
                    }
                }

                //if (rmsQuotaATotale == 0 && (!((categoria == 6 && certificatoPensioneDiretta.GetValueOrDefault() == 0 && dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(1968, 05, 01))) ||
                //   (decorrenzaOpzione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, new DateTime(1979, 01, 31))) || (!string.IsNullOrEmpty(codNatura) && codNatura.Substring(1, 1).Equals("O")))))
                //{
                //    messaggioVideo = "R.M.S. Quota A mancante";
                //    return false;
                //}
            }
            return true;
        }

        public static bool ControlsContributiItalianiEsteriAl1295PerAPEPrecoci(GestionePensione.DatiPensione datiPensione, int? contributiItalianiEdEsteriAl1295, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (Utility.IsDomandaAPEPrecoci(datiPensione))
            {
                if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 obbligatori.";
                    return false;
                }
            }

            return true;
        }

        //public static bool ControlsContributiItalianiEsteriAl1295(DateTime? dataInizioAssicurazione, int? contributiItalianiEdEsteriAl1295, string codNatura, int? settimaneContributiveQuotaC, decimal? importoContributivoTotaleQuotaC, decimal? montanteQuotaC, int? settimaneRetributiveQuotaB, int? vvMisuraDL50392, decimal? rmsQuotaB, string gruppo, bool isCodiceGestione0XPresenteContributiItalianiEdEsteri, short? primoCodiceGestioneTraduzioneSuGP, bool isCodiceGestione6XPresente, char? codiceArt48PrimoStato, out string messaggioVideo)
        //{
        //    messaggioVideo = string.Empty;

        //    //if (dataInizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1995, 12, 31)))
        //    {
        //        if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() > 935 && !string.IsNullOrEmpty(codNatura) && !codNatura.Substring(0, 1).Equals("1") && !codNatura.Substring(0, 1).Equals("2") && codNatura.Substring(1, 1).Equals("O"))
        //        {
        //            //if (settimaneContributiveQuotaC.GetValueOrDefault() > 0 || importoContributivoTotaleQuotaC.GetValueOrDefault() > 0 || montanteQuotaC.GetValueOrDefault() > 0)
        //            //{
        //            //    messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 incompatibili con dati per L.335/95";
        //            //    return false;
        //            //}

        //            if (isCodiceGestione0XPresenteContributiItalianiEdEsteri)
        //            {
        //                messaggioVideo = "CTR Ital.Esteri AL 31/12/95 incompatibili con Gestione CTR Esteri (= 0x)";
        //                return false;
        //            }

        //            if ((settimaneRetributiveQuotaB.GetValueOrDefault() == 0 && vvMisuraDL50392.GetValueOrDefault() == 0) || rmsQuotaB.GetValueOrDefault() == 0)
        //            {
        //                messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 incompatibili con dati per L.503/92";
        //                return false;
        //            }

        //            if (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 0 && (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() < 61 || primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 65))
        //            {
        //                messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con gestione CTR Esteri(<> 6X)";
        //                return false;
        //            }
        //        }

        //        if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() < 936)
        //        {
        //            if (settimaneRetributiveQuotaB.GetValueOrDefault() > 0 || rmsQuotaB.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0)
        //            {
        //                messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 incompatibili con dati per L.503/92";
        //                return false;
        //            }

        //            if (isCodiceGestione6XPresente)
        //            {
        //                messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con gestione CTR Esteri(= 6X)";
        //                return false;
        //            }

        //            if (settimaneContributiveQuotaC.GetValueOrDefault() == 0 || montanteQuotaC.GetValueOrDefault() == 0)
        //            {
        //                messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 incompatibili con dati per L.335/95";
        //                return false;
        //            }

        //            if (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 4 || (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() < 1 && codiceArt48PrimoStato.GetValueOrDefault() != 'S'))
        //            {
        //                messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con CTR Esteri (Gestione 01/4 mancante)";
        //                return false;
        //            }
        //        }

        //        //if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() == 0)
        //        //{
        //        //    if (gruppo.Equals("0001") && !string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2")))
        //        //    {
        //        //        messaggioVideo = "Contributi Italiani ed Esteri al 31.12.95 mancanti per Pensione di Anzianita'";
        //        //        return false;
        //        //    }
        //        //}

        //    }

        //    return true;
        //}

        /// <summary> 
        /// Muove il campo  INIASS nel campo di appoggio APP-DATA-1 
        /// Se il campo APP-DATA-1 minore 19930101 allora muove 19930101 nel campo APP-DATA-1. Muove  IW1DEORA  nel campo  APP-ANNO-2, muove IW1DEORM  nel campo APP-MESE-2 ,muove  1 
        /// nel campo APP-GIORNO-2 e chiama il programma PCIPL94 usando come ritorno  i campi APP-DATA-1 APP-DATA-2 APP-DATA-RC. Muove APP-DATA-RC  nel campo  APP-CTR-93-DEC, muove  
        /// FINASS nel campo APP-DATA-2; e chiama il programma PCIPL94 usando come ritorno  i campi APP-DATA-1 APP-DATA-2 APP-DATA-RC.  Muove il campo  APP-DATA-RC  nel campo  
        /// APP-CTR-93-FIN.
        /// Se i campi IF W-DEORIG > 199212 AND FINASS > 19930100 allora controlla se ( (IW1STOBG + ICI1VVOBG) >  APP-CTR-93-DEC  OR  APP-CTR-93-FIN ) AND  TP1PRIN NOT = 257 allora 
        /// controlla se i campi IABCONA4 NOT = "G"  AND IABCONA4 NOT = "Z"  allora controlla se i campi  (TP1ATEC = 4 AND TP1PRIN = 350 AND           W-DEORIG > 200401) continua 
        /// altrimenti segnala errore "SETT. OBG + V.V. D.L.503/92 SUPERIORI  A CAPIENZA NEL PERIODO"      
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nSettimaneOBG"></param>
        /// <param name="nContributiUtiliLavoratoriAutonomi"></param>
        /// <param name="nContributiVolontari"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCapienzaSettimaneDL50392WithAssicurazione(GestionePensione.DatiPensione datiPensione, DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione, int? professioneIndividuale, string codNatura, DateTime? decorrenzaOriginaria, DateTime? decorrenza, int? settimaneRetributiveQuotaB, int? vvMisuraDL50392, int? attivitaEconomica, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Num_Sett_Periodo_Ass.NUM_SETT_PERIODO_ASS))
                return true;

            if (dataInizioAssicurazione.HasValue && dataFineAssicurazione.HasValue)
            {
                DateTime? appInizioAssicurazione = null;

                if (!Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                    appInizioAssicurazione = new DateTime(1993, 01, 01);
                else
                    appInizioAssicurazione = dataInizioAssicurazione;

                int nSettimaneInizioAssWithDecorrenza = Utility.NSettimaneBetweenDate(new DateTime(decorrenzaOriginaria.Value.Year, decorrenzaOriginaria.Value.Month, 01), appInizioAssicurazione.Value);
                int nSettimaneAssicurative = Utility.NSettimaneBetweenDate(dataFineAssicurazione.Value, dataInizioAssicurazione.Value);

                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1992, 12, 31)) && Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
                {
                    if (((settimaneRetributiveQuotaB.GetValueOrDefault() + vvMisuraDL50392.GetValueOrDefault()) > nSettimaneInizioAssWithDecorrenza || (settimaneRetributiveQuotaB.GetValueOrDefault() + vvMisuraDL50392.GetValueOrDefault()) > nSettimaneAssicurative) && professioneIndividuale.GetValueOrDefault() != 257)
                    {
                        if (!string.IsNullOrEmpty(codNatura) && !codNatura.Substring(2, 1).Equals("G") && !codNatura.Substring(2, 1).Equals("Z"))
                        {
                            if (!(attivitaEconomica.GetValueOrDefault() == 4 && professioneIndividuale.GetValueOrDefault() == 350 && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(2004, 01, 31))))
                            {
                                messaggioVideo = "Sett. OBG + V.V. D.L.503/92 superiori a capienza nel periodo";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo INIASS > 19921231 allora controlla se il campo SETT2(1) > 0 allora controlla se (DEC(1  1) NOT =  DEC233(1 2) AND  DEC233(2 1) AND  DEC233(2 2) AND  DEC233(3 1) AND  DEC233(3 2)) 
        /// allora segnala errore  "MANCANO SETTIMANE ESTERE A DECORRENZA RICALCOLO  "  DECMM(1 1) "/" DECAA(1 1)
        /// </summary>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="datiContrEE"></param>
        /// <param name="prestEE"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsSettimaneEstereWithDecorrenzaRicalcolo(DateTime? dataInizioAssicurazione, DateTime? decorrenzaContrEE, int? contributiEERicalcolo,
             DateTime? primaDecorrenzaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1992, 12, 31);

            if (dataInizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(dataInizioAssicurazione.Value, dataCompare))
            {
                if (contributiEERicalcolo.GetValueOrDefault() > 0)
                {
                    if (primaDecorrenzaImportiEsteri.HasValue && !primaDecorrenzaImportiEsteri.Value.Equals(decorrenzaContrEE))
                    {
                        messaggioVideo = "Mancano settimane estere a decorrenza ricalcolo " + string.Format("{0: MM/yyyy}", primaDecorrenzaImportiEsteri);
                        return false;
                    }
                }
            }


            return true;
        }

        /// <summary>
        /// Calcola il numero delle settimane Contributi Italiani ed Esteri
        /// 
        /// Se il campo INIASS  <  19950101 allora controlla se i campi GEST233(1 1) = 1 AND  DEC233(1 1) NOT > IW1DEORIG allora somma  SETRI233(1 1) al campo APP-APP, se il campo GEST233(1 2) = 1 
        /// AND  DEC233(1 2) NOT > IW1DEORIG allora somma  SETRI233(1 2) al campo APP-APP, se il campo GEST233(2 1) = 1 AND  DEC233(2 1) NOT > IW1DEORIG allora somma  SETRI233(2 1) al campo APP-APP, 
        /// se il campo GEST233(2 2) = 1 AND  DEC233(2 2) NOT > IW1DEORIG allora somma  SETRI233(2 2) al campo APP-APP, se il campo GEST233(3 1) = 1  AND  DEC233(3 1) NOT > IW1DEORIG allora somma  
        /// SETRI233(3 1) al campo APP-APP, se il campo IF GEST233(3 2) = 1 AND  DEC233(3 2) NOT > IW1DEORIG  allora somma  SETRI233(3 2) al campo APP-APP
        /// </summary>
        /// <param name="sommmaSettimaneContrEE"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="codiceGestioneContrEE"></param>
        /// <param name="decorrenzaContrEE"></param>
        /// <param name="settimaneContrEE"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns></returns>
        public static int CalcolaSettimaneContrEE(int sommmaSettimaneContrEE, DateTime? dataInizioAssicurazione, short? codiceGestioneContrEE, DateTime? decorrenzaContrEE, int? settimaneContrEE, DateTime? decorrenzaOriginaria)
        {
            DateTime dataCompare = new DateTime(1995, 01, 01);

            if (dataInizioAssicurazione.HasValue && !Utility.DataSuccessivaA(dataInizioAssicurazione.Value, dataCompare))
            {
                if (codiceGestioneContrEE.GetValueOrDefault() == 1 && decorrenzaContrEE.HasValue && !Utility.DataStrettamenteSuccessivaA(decorrenzaContrEE.Value, decorrenzaOriginaria.Value))
                {
                    sommmaSettimaneContrEE = sommmaSettimaneContrEE + settimaneContrEE.GetValueOrDefault();
                }
            }
            return sommmaSettimaneContrEE;
        }

        /// <summary>
        /// Se il campo ICISTOBG335  >  0 allora muove il campo IW1STOBG nel campo  APP-APP se il campo INIASS < 19930101 AND GEST233(1 1) = 61 allora somma  SETRI233(1 1) al campo APP-APP, se il campo INIASS  < 19930101 AND GEST233(1 2) = 61 allora somma  SETRI233(1 2) al campo APP-APP, se il campo INIASS  < 19930101 AND GEST233(2 1) = 61 allora somma  SETRI233(2 1) al campo APP-APP, se il campo INIASS < 19930101 AND GEST233(2 2) = 61 allora somma  SETRI233(2 2) al campo APP-APP, se il campo INIASS < 19930101 AND GEST233(3 1) = 61 allora somma  SETRI233(3 1) al campo APP-APP, se il campo INIASS < 19930101 AND GEST233(3 2) = 61 allora somma  SETRI233(3 2) al campo APP-APP.
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="settimaneContributiveCodGestione1"></param>
        /// <param name="settimaneRetributiveQuotaBCodGestione1"></param>
        /// <param name="codiceGestioneContrEE"></param>
        /// <param name="settimaneContrEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static int CalcolaSettimane(DateTime? inizioAssicurazione, int? settimaneContributiveCodGestione1, int? settimaneRetributiveQuotaBCodGestione1,
            short? codiceGestioneContrEE, int? settimaneContrEE, int sommaSettimane)
        {
            if (settimaneContributiveCodGestione1.GetValueOrDefault() > 0)
            {
                sommaSettimane = sommaSettimane + settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault();
                if (!Utility.DataSuccessivaA(inizioAssicurazione.Value, new DateTime(1993, 01, 01)) && codiceGestioneContrEE.GetValueOrDefault() == 61)
                {
                    sommaSettimane = sommaSettimane + settimaneContrEE.GetValueOrDefault();
                }
            }

            return sommaSettimane;
        }

        /// <summary>
        /// Muove il campo INIASS   nel campo  APP-DATA-1. 
        /// Se il campo    APP-DATA-1 < 19930101 allora muove  19930101 nel campo APP-DATA-1
        /// Muove il campo FINIASS   nel campo  APP-DATA-2    
        /// Se  il campo    APP-DATA-2  >  19951231 allora muove  19951231 nel campo APP-DATA-2
        /// Chiamata al programma PCIPL94 usando i campi  APP-DATA-1  APP-DATA-2  APP-DATA-RC, muove il  campo APP-DATA-RC nel campo  APP-CTR-93-95
        /// Se  APP-APP  >  APP-CTR-93-95  AND  IW1DEORIG < 201202 allora segnala errore "SETTIMANE 1993/1995 SUPERIORI  AL PERIODO"
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="sommaSettimane"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimane1993_1995(DateTime? inizioAssicurazione, DateTime? fineAssicurazione, DateTime? decorrenzaOriginaria, int sommaSettimane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime data1 = inizioAssicurazione.Value;
            DateTime data2 = fineAssicurazione.Value;

            if (!Utility.DataSuccessivaA(data1, new DateTime(1993, 01, 01)))
                data1 = new DateTime(1993, 01, 01);

            if (Utility.DataStrettamenteSuccessivaA(data2, new DateTime(1995, 12, 31)))
                data2 = new DateTime(1995, 12, 31);

            int? nSettimane = Utility.NSettimaneBetweenDate(data2, data1);
            if (nSettimane < 0)
                nSettimane = 0;

            if (sommaSettimane > nSettimane.GetValueOrDefault() && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2012, 02, 01)))
            {
                messaggioVideo = "Settimane 1993/1995 superiori al periodo";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Muove zero nel campo   APP-APP
        /// Se il campo GEST233(1 1) = 1 OR 61 allora somma  SETRI233(1 1) al campo APP-APP , se il campo GEST233(1 2) = 1 OR 61 allora somma  il campo SETRI233(2 1)  al campo 
        /// APP-APP, se il campo EST233(2 2) = 1 OR 61 allora somma  il campo  SETRI233(2 2) al campo APP-APP, se il campo GEST233(3 1) = 1 OR 61 allora somma il campo SETRI233(3 1) 
        /// al campo APP-APP, se il campo GEST233(3 2) = 1 OR 61 allora somma il campo SETRI233(3 2) al campo APP-APP.
        /// Se il campo  APP-APP > 0  AND  APP-CAL-EST-R minore APP-APP allora segnala errore  "SETT.ESTERE 233/90 - 503/92 - 335/95 NON VANNO ACQUISITE"   se il campo APP-CAL-EST-R > 0 
        /// allora segnala errore "SETT.EST. 233/90 - 503/92 - 335/95 INCOMPATIBILI  CON SETTIMANE CNV09"    
        /// </summary>
        /// <param name="sommaSettimaneCodGestione1_61CTRItalianiEdEsteri"></param>
        /// <param name="settimaneRicalcoloMisura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimane23390_50392_33595(int? sommaSettimaneCodGestione1_61CTRItalianiEdEsteri, int? settimaneRicalcoloMisura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (sommaSettimaneCodGestione1_61CTRItalianiEdEsteri.GetValueOrDefault() > 0 && settimaneRicalcoloMisura.GetValueOrDefault() < sommaSettimaneCodGestione1_61CTRItalianiEdEsteri.GetValueOrDefault())
            {
                if (settimaneRicalcoloMisura.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Sett.Estere 233/90 - 503/92 - 335/95 incompatibili con Settimane delle Istituzioni Estere";
                    return false;
                }
                else
                {
                    messaggioVideo = "Sett.Estere 233/90 - 503/92 - 335/95 non vanno acquisite";
                    return false;
                }
            }

            return true;
        }

        #endregion PCIPL40

        #region PCIPL39

        /// <summary>
        /// Se IW1RMSCDM è maggiore di zero effettuare le seguenti operazioni : 
        /// Se FL-NO233 è uguale a "S" oppure FL-P93INP93 è uguale a "S" valorizzare con "23" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 24 il campo COL-ERR1, con "R.M.P. ANTE 1993 INCOMPATIBILE CON DECORRENZA E/O INIZIO ASSICUR."  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(GestionePensione.DatiPensione datiPensione, int categoria, decimal? rmsQuotaA, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta, DateTime? dataMorte, DateTime? inizioAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Rms_Ante_1993.RMS_ANTE_1993))
                return true;

            DateTime? decorrenza = null;
            if (decorrenzaDiretta.HasValue)
                decorrenza = decorrenzaDiretta;
            else
                decorrenza = decorrenzaPensione;

            bool flag1 = FlagNo233(decorrenzaPensione, decorrenzaDiretta, categoria, dataMorte);
            bool flag2 = FlagP93INP93(decorrenza, inizioAssicurazione);

            if (rmsQuotaA.HasValue && rmsQuotaA.Value > 0)
            {
                if (flag1 || flag2)
                {
                    messaggioVideo = "R.M.S. ante 1993 incompatibile con decorrenza e/o Inizio Assicurazione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se IW1RETCDM è maggiore di zero effettuare le seguenti operazioni :  
        /// Se FL-NO233 è uguale a "S" oppure (FL-P93FIA93-NOINA è uguale a "S" e FL-P93FIA93-INA è uguale a " ") valorizzare con "25" il campo  TIPO-ERRORE, valorizzare con 7 il campo RIG-ERRORE, valorizzare con 56 il campo COL-ERR1 , con "R.M.P. DAL 1993 INCOMPATIBILE CON DECORRENZA E/O ULTIMO CONTRIB." il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se IW1RETCOM è maggiore di zero effettuare le seguenti operazioni :                                                              
        /// Se FL-NO233 è uguale a "S" oppure (FL-P93FIA93-NOINA è uguale a "S" e FL-P93FIA93-INA è uguale a " ")  valorizzare con "25" il campo  TIPO-ERRORE, con 9 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "R.M.P. DAL 1993 INCOMPATIBILE CON DECORRENZA E/O ULTIMO CONTRIB."  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);                                     

        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="dataMorte"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(int categoria, string naturaPensione, DateTime? decorrenza, DateTime? decorrenzaDiretta, DateTime? fineAssicurazione, DateTime? dataMorte, decimal? rmsQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //richiamo routine
            bool flagP93FIA93NOINA = FlagP93FIA93NOINA(decorrenza, fineAssicurazione, naturaPensione);
            bool flagP93FIA93INA = FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione);
            bool flagNo233 = FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte);

            if (rmsQuotaB.HasValue && rmsQuotaB.Value > 0)
            {
                if (flagNo233 || (flagP93FIA93NOINA && !flagP93FIA93INA))
                {
                    messaggioVideo = "R.M.S. dal 1993 incompatibile con decorrenza e/o ultimo contributo";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se  IW1STCDM è uguale a zero effettuare le seguenti operazioni :  
        /// Se (IW1RETCDM è uguale a zero oppure ICI2SETFIT è maggiore di zero) continuare l’elaborazione al punto successivo (66)  diversamente valorizzare con "26" il campo    TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con  "RIGA INCOMPLETA " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Diversamente da quanto analizzato nel punto precedente (65) effettuare le seguenti operazioni : 
        /// Se IW1RETCDM è uguale a zero valorizzare con "26" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "RIGA INCOMPLETA "il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se FL-NO233 è uguale a "S" oppure (FL-P93FIA93-INA è uguale a "S" oppure FL-FIA93 è uguale a "S")  valorizzare con "27" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETT. DAL 1993 INCOMPATIBILI CON DECORRENZA E/O ULTIMO CONTRIB."  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se  FL-P93INA93FIP93 è uguale a "S" e IW1DEORIG è inferiore a 201202 effettuare le seguenti operazioni :
        /// Valorizzare con W-DEORIG i primi 6 caratteri del campo campo APP-DATA-2, con 1 il campo APP-GIORNO-2, con 19930101 il campo APP-DATA-1;
        /// Eseguire il programma PCIPL94 passandogli i parametri : APP-DATA-1      APP-DATA-2 e APP-DATA-RC;
        ///	Se dopo l’esecuzione del programma il valore contenuto nel campo IW1STCDM è uguale a quello di APP-DATA-RC valorizzare con "28" il campo     TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETTIMANE ITALIANE DAL 1993 SUPERIORI A CAPIENZA PERIODO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneQuotaB"></param>
        /// <param name="settimaneFittiziePrepensionamento"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneItaliane1993(int categoria, decimal? rmsQuotaB, int? settimaneQuotaB, int? settimaneFittiziePrepensionamento, DateTime? decorrenza, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta,
            DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione, int codCategoria, DateTime? inizioAssicurazione, string gruppo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(2012, 02, 01);
            DateTime dataCompare2 = new DateTime(1993, 01, 01);
            int settimane = 0;

            if (!settimaneQuotaB.HasValue || settimaneQuotaB.Value == 0)
            {
                if (!(categoria == 85 || categoria == 86 || categoria == 87 || categoria == 88 || categoria == 89 || categoria == 90 || categoria == 91 || categoria == 92 || categoria == 93 ||
                    categoria == 4 || categoria == 5 || categoria == 6))
                {
                    if (!((!rmsQuotaB.HasValue || rmsQuotaB.Value == 0) || (settimaneFittiziePrepensionamento.HasValue && settimaneFittiziePrepensionamento.Value > 0)))
                    {
                        messaggioVideo = "Riga incompleta";
                        return false;
                    }
                }
            }
            else
            {
                if (!rmsQuotaB.HasValue || rmsQuotaB.Value == 0)
                {
                    messaggioVideo = "Riga incompleta";
                    return false;
                }

                if (codCategoria != 1)  //bypasso il controllo per il codice categoria 1
                {
                    if (FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte) || FlagP93FIA93NOINA(decorrenza, fineAssicurazione, naturaPensione) ||
                        FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione))
                    {
                        messaggioVideo = "Settimane dal 1993 incompatibili con decorrenza e/o ultimo contributo";
                        return false;
                    }
                }

                if (Utility.IsRicostituzione(gruppo) && !(categoria >= 85 && categoria <= 93))
                {
                    if (FlagP93INA93FIP93(decorrenza, inizioAssicurazione, fineAssicurazione) && decorrenzaPensione.HasValue && !Utility.DataSuccessivaA(decorrenzaPensione.Value, dataCompare))
                    {
                        settimane = Utility.NSettimaneBetweenDate(decorrenzaDiretta.HasValue ? decorrenzaDiretta.Value : decorrenzaPensione.Value, dataCompare2);
                        if (settimaneQuotaB.Value == settimane)
                        {
                            messaggioVideo = "Settimane italiane dal 1993 superiori a capienza periodo";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se IW1STCDM è uguale a zero e FL-P93FIA93-INA è uguale a " " e IW1RETCDM è maggiore di zero  effettuare le seguenti operazioni :                                                             
        /// Se IW1RETCDM è maggiore di zero e W-DEORIG è maggiore di 199301 e ICI2SETFIT è maggiore di zero e IW1CATPEN è maggiore di 84 e IW1CATPEN è inferiore a 88  continuare l’elaborazione al punto successivo (68) diversamente valorizzare con "29" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "RIGA INCOMPLETA " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  ERRORE-CONTROLLI;                              

        /// Se  IW1START  è uguale a zero e FL-P93FIA93-INA è uguale a  " "  e IW1RETART è maggiore di zero  effettuare le seguenti operazioni :  
        /// Se  IW1RETART è maggiore di zero e W-DEORIG è maggiore di 199301 e ICI2SETFIT è maggiore di zero e IW1CATPEN è maggiore di 87  e IW1CATPEN è inferiore a 91 continuare l’elaborazione al punto successivo (78), diversamente valorizzare con "29" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 56 il campo COL-ERR1 , con "RIGA INCOMPLETA " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// 
        /// Se  IW1STCOM è uguale a zero e FL-P93FIA93-INA è uguale a " "  e IW1RETCOM è maggiore di zero effettuare le seguenti operazioni :  
        /// Se IW1RETCOM è maggiore di zero e W-DEORIG è maggiore di 199301 e ICI2SETFIT è maggiore di zero e IW1CATPEN è maggiore di 90 e IW1CATPEN è inferiore a 94 continuare l’elaborazione al punto successivo (88), diversamente valorizzare con "29" il campo  TIPO-ERRORE, con 9 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "RIGA INCOMPLETA " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneQuotaB"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimaneFittiziePrepensionamento"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsQuotaBWithcategoriaAndSettPrepensionamento(int categoria, decimal? rmsQuotaB, int? settimaneQuotaB, DateTime? decorrenza, DateTime? dataMorte, DateTime? fineAssicurazione,
            string naturaPensione, int? settimaneFittiziePrepensionamento, int categoriaMix, int categoriaMax, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1993, 01, 01);

            if ((!settimaneQuotaB.HasValue || settimaneQuotaB.Value == 0) && FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione) && rmsQuotaB.HasValue && rmsQuotaB.Value > 0)
            {
                if (!(Utility.DataStrettamenteSuccessivaA(decorrenza.Value, dataCompare) && settimaneFittiziePrepensionamento.HasValue && settimaneFittiziePrepensionamento.Value > 0 && categoria > categoriaMix && categoria < categoriaMax))
                {
                    messaggioVideo = "Riga incompleta";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se FL-NO233 è uguale a "S"  e (ICIRETCDM335 è maggiore di zero oppure ICISTCDM335 è maggiore  di zero oppure ICICONCDM335 è maggiore di zero) valorizzare con "30" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "DATI RELATIVI AI PERIODO DAL 01.01.1996 INCOMPATIBILI CON DECORRENZA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se FL-NO233 è uguale a"S" e (ICIRETOBG335 è maggiore di zero oppure ICISTOBG335 è maggiore di zero oppure ICICONOBG335 è maggiore di zero) valorizzare con "30" il campo  TIPO-ERRORE, con 15 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "DATI RELATIVI AI PERIODO DAL 01.01.1996 INCOMPATIBILI CON DECORRENZA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="montante"></param>
        /// <param name="settimane"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiviWithDecorrenza(int categoria, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta, DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione,
            decimal? montante, int? settimane, decimal? importoContributivoTotale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (FlagNo233(decorrenzaPensione, decorrenzaDiretta, categoria, dataMorte) &&
                ((montante.HasValue && montante.Value > 0) || (settimane.HasValue && settimane.Value > 0) || (importoContributivoTotale.HasValue && importoContributivoTotale.Value > 0)))
            {
                messaggioVideo = "Dati relativi al periodo dal 01/01/1996 incompatibili con decorrenza";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se ICICONCDM335 è maggiore di ICIRETCDM335 valorizzare con "31" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 12 il campo COL-ERR1 , con "IMPORTO CONTRIBUTI PER L.335 MAGGIORE DI MONTANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        ///
        /// Se ICICONART335 è maggiore di ICIRETART335  valorizzare con "31" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "IMPORTO CONTRIBUTI PER L.335 MAGGIORE DI MONTANTE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se ICICONCOM335 è maggiore di ICIRETCOM335  valorizzare con "31" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "IMPORTO CONTRIBUTI PER L.335 MAGGIORE DI MONTANTE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se ICICONOBG335 è maggiore di ICIRETOBG335, valorizzare con "31" il campo  TIPO-ERRORE, con 15 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "IMPORTO CONTRIBUTI PER L.335 MAGGIORE DI MONTANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="montante"></param>
        /// <param name="importoContributivoTotale"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportoContributivoTotWithMontante(DateTime? decorrenzaOriginaria, decimal? montante, decimal? importoContributivoTotale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            // Con decorrenza >= 01/2015 non dovrà essere verificato il controllo sotto. 
            // Rif. mail del 23/02/2015 con oggetto "LIQPENS - attività"
            if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                if (importoContributivoTotale.HasValue && montante.HasValue && importoContributivoTotale.Value > montante.Value)
                {
                    messaggioVideo = "Importo contributi per L.335 maggiore di montante";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se (ICICONCDM335 è maggiore di zero  e (ICIRETCDM335 è uguale a zero  oppure ICISTCDM335  è uguale a zero))  oppure (ICISTCDM335 è maggiore di zero e (ICIRETCDM335 è uguale a zero oppure ICICONCDM335 è uguale a zero)) valorizzare con "32" il campo   TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "IMPORTI / CONTRIBUTI PER L.335 INCOMPLETIO MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se (ICICONART335 è maggiore di zero e (ICIRETART335 è uguale a zero oppure ICISTART335 è uguale a zero ) ) oppure (ICISTART335 è maggiore di zero e (ICIRETART335 è uguale a zero oppure ICICONART335 è uguale a zero)) valorizzare con "32" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 12 il campo COL-ERR1 con "IMPORTI / CONTRIBUTI PER L.335 INCOMPLETI O MANCANTI"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se (ICICONCOM335 è maggiore di zero   e (ICIRETCOM335 è uguale a zero oppure ICISTCOM335 è uguale a zero) )  oppure (ICISTCOM335 è maggiore di zero  e (ICIRETCOM335 è uguale a zero  oppure ICICONCOM335 è uguale a zero) )  valorizzare con "32" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 12 il campo COL-ERR1 con "IMPORTI / CONTRIBUTI PER L.335 INCOMPLETI  O MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// 
        /// Se (ICICONOBG335 è maggiore di zero  e (ICIRETOBG335 è uguale a zero oppure ICISTOBG335 è uguale a zero ) ) oppure (ICISTOBG335 è maggiore di zero e (ICIRETOBG335 è uguale a zero oppure ICICONOBG335 uguale a zero) ) valorizzare con "32" il campo          TIPO-ERRORE, con 15 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "IMPORTI / CONTRIBUTI PER L.335 INCOMPLETIO MANCANTI"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="montante"></param>
        /// <param name="importoContributivoTotale"></param>
        /// <param name="settimane"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportiWithContributi(decimal? montante, decimal? importoContributivoTotale, int? settimane, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((importoContributivoTotale.HasValue && importoContributivoTotale.Value > 0 && (!montante.HasValue || montante.Value == 0 || !settimane.HasValue || settimane.Value == 0)) ||
                (settimane.HasValue && settimane.Value > 0 && (!montante.HasValue || montante.Value == 0 || !importoContributivoTotale.HasValue || importoContributivoTotale.Value == 0)))
            {
                messaggioVideo = "Importi / contributi per L.335 incompleti o mancanti";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se  FL-NO233 è uguale a " "  e (IW1CATPEN è uguale a 85 oppure 86 oppure 87) e (IW1SACDM è uguale a zero e IW1STCDM è uguale a zero e ICISTCDM335 è uguale a zero e ICISTCDM012 è uguale a zero) valorizzare con "33" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con  24 il campo COL-ERR1, con "CONTRIBUTI RELATIVI ALLA CATEGORIA IN LIQUIDAZIONE MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        ///
        /// Se  FL-NO233 è uguale a " "  e (IW1CATPEN è uguale a 88 oppure 89 oppure 90) e (IW1SAART è uguale a zero e IW1START è uguale a zero e ICISTART335 è uguale a zero e ICISTART012 è uguale a zero )  valorizzare con "33" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con  24 il campo COL-ERR1 , con "CONTRIBUTI RELATIVI ALLA CATEGORIA IN LIQUIDAZIONE MANCANTI"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);                         
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="settimaneQuotaA"></param>
        /// <param name="settimaneQuotaD"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="dataMorte"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsQuotaAWithSettimaneQuotaDAndCategoria(int categoria, int? settimaneQuotaA, int? settimaneQuotaB, int? settimaneQuotaC, int? settimaneQuotaD, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta, DateTime? fineAssicurazione, DateTime? dataMorte, int codGestione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int cat1 = 0;
            int cat2 = 0;
            int cat3 = 0;
            if (codGestione == 2)
            {
                cat1 = 85;
                cat2 = 86;
                cat3 = 87;
            }
            if (codGestione == 3)
            {
                cat1 = 88;
                cat2 = 89;
                cat3 = 90;
            }
            if (codGestione == 4)
            {
                cat1 = 91;
                cat2 = 92;
                cat3 = 93;
            }

            if (!FlagNo233(decorrenzaPensione, decorrenzaDiretta, categoria, dataMorte) && 
                (decorrenzaPensione.HasValue && Utility.DataSuccessivaA(decorrenzaPensione.Value, new DateTime(1993, 1, 1))) &&
                (categoria == cat1 || categoria == cat2 || categoria == cat3) &&
                ((!settimaneQuotaA.HasValue || settimaneQuotaA.Value == 0) && (!settimaneQuotaB.HasValue || settimaneQuotaB.Value == 0) && (!settimaneQuotaC.HasValue || settimaneQuotaC.Value == 0) && (!settimaneQuotaD.HasValue || settimaneQuotaD.Value == 0)))
            {
                messaggioVideo = "Contributi relativi alla categoria in liquidazione mancanti";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se FL-NO233 è uguale a "S" e (ICIRETART335 è maggiore di zero oppure ICISTART335 è maggiore di zero oppure ICISTART012 è maggiore di zero oppure ICICONART335 è maggiore di zero )  valorizzare con "30" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "DATI RELATIVI AI PERIODO DAL 01.01.1996 INCOMPATIBILI CON  DECORRENZA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);                                         
        /// 
        /// Se FL-NO233 è uguale a "S" e (ICIRETCOM335 è maggiore di zero oppure ICISTCOM335 è maggiore di zero oppure ICISTCOM012 è maggiore di zero oppure ICICONCOM335 è maggiore di zero ) valorizzare con "30" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "DATI RELATIVI AI PERIODO DAL 01.01.1996 INCOMPATIBILI CON DECORRENZA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="montante"></param>
        /// <param name="settimane"></param>
        /// <param name="importoContributivoTotale"></param>
        /// <param name="settimaneQuotaD"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsContributiviWithDecorrenzaWithSettQuotaD(int categoria, DateTime? decorrenzaPensione, DateTime? decorrenzaDiretta, DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione,
            decimal? montante, int? settimane, decimal? importoContributivoTotale, int? settimaneQuotaD, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (FlagNo233(decorrenzaPensione, decorrenzaDiretta, categoria, dataMorte) &&
                ((montante.HasValue && montante.Value > 0) || (settimane.HasValue && settimane.Value > 0) || (importoContributivoTotale.HasValue && importoContributivoTotale.Value > 0) || (settimaneQuotaD.HasValue && settimaneQuotaD.Value > 0)))
            {
                messaggioVideo = "Dati relativi al periodo dal 01/01/1996 incompatibili con decorrenza";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se  IW1STOBG  è uguale a zero  e FL-P93FIA93-INA è uguale a " " e IW1RETOBG è maggiore di zero valorizzare con "29" il campo  TIPO-ERRORE, con 10 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "RIGA INCOMPLETA "  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimaneQuotaB"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneQuotaBWithRsmQuotaB(DateTime? decorrenza, DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione, int? settimaneQuotaB,
            decimal? rmsQuotaB, int categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((!settimaneQuotaB.HasValue || settimaneQuotaB.Value == 0) && !FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione) && rmsQuotaB.HasValue && rmsQuotaB.Value > 0)
            {
                if (!(categoria == 85 || categoria == 86 || categoria == 87 || categoria == 88 || categoria == 89 || categoria == 90 || categoria == 91 || categoria == 92 || categoria == 93 ||
                    categoria == 4 || categoria == 5 || categoria == 6))
                {
                    messaggioVideo = "Riga incompleta";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se FINASS è maggiore di  20111231 e  (ICISTOBG012 è uguale a zero  e ICISTCDM012  è uguale a zero e ICISTART012 è uguale a zero e ICISTCOM012 è uguale a zero ) valorizzare con "37" il campo  
        /// TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "Contributi successivi al 31.12.2011: manca quota C" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire 
        /// da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="settimaneQuotaDCodGestione1"></param>
        /// <param name="settimaneQuotaDCodGestione2"></param>
        /// <param name="settimaneQuotaDCodGestione3"></param>
        /// <param name="settimaneQuotaDCodGestione4"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaFineAssicurazioneWithSettimaneQuotaD(DateTime? fineAssicurazione, int? settimaneQuotaDCodGestione1, int? settimaneQuotaDCodGestione2, int? settimaneQuotaDCodGestione3, int? settimaneQuotaDCodGestione4, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(2011, 12, 31);

            if (fineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, dataCompare) && settimaneQuotaDCodGestione1.GetValueOrDefault() == 0 &&
                settimaneQuotaDCodGestione2.GetValueOrDefault() == 0 && settimaneQuotaDCodGestione3.GetValueOrDefault() == 0 && settimaneQuotaDCodGestione4.GetValueOrDefault() == 0)
            {
                messaggioVideo = "Contributi successivi al 31/12/2011: manca quota D";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se  FINASS è maggiore di 19951231 effettuare le seguenti operazioni :                                                              
        /// Se  (IW1RETOBG è uguale a zero e IW1RETCDM è uguale a zero e IW1RETART è uguale a zero e IW1RETCOM è uguale a zero) e  (ICISTOBG335 è uguale a zero e ICISTCDM335 è uguale a zero e ICISTART335 è uguale a zero e ICISTCOM335 è uguale a zero) e  (ICISTOBG012 è uguale a zero e ICISTCDM012 è uguale a zero e ICISTART012 è uguale a zero e ICISTCOM012 è uguale a zero) valorizzare con "37" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "R.M.P MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="rmsQuotaBCodGestione1"></param>
        /// <param name="rmsQuotaBCodGestione2"></param>
        /// <param name="rmsQuotaBCodGestione3"></param>
        /// <param name="rmsQuotaBCodGestione4"></param>
        /// <param name="settimaneCodGestione1"></param>
        /// <param name="settimaneCodGestione2"></param>
        /// <param name="settimaneCodGestione3"></param>
        /// <param name="settimaneCodGestione4"></param>
        /// <param name="settimaneQuotaDCodGestione1"></param>
        /// <param name="settimaneQuotaDCodGestione2"></param>
        /// <param name="settimaneQuotaDCodGestione3"></param>
        /// <param name="settimaneQuotaDCodGestione4"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRmsQuotaBAndSettimaneWithFineAssicurazione(GestionePensione.DatiPensione datiPensione, DateTime? fineAssicurazione, decimal? rmsQuotaBCodGestione1, decimal? rmsQuotaBCodGestione2, decimal? rmsQuotaBCodGestione3, decimal? rmsQuotaBCodGestione4,
            int? settimaneCodGestione1, int? settimaneCodGestione2, int? settimaneCodGestione3, int? settimaneCodGestione4, int? settimaneQuotaDCodGestione1, int? settimaneQuotaDCodGestione2,
            int? settimaneQuotaDCodGestione3, int? settimaneQuotaDCodGestione4, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1995, 12, 31);

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Rms_Mancanti.RMS_MANCANTI))
                return true;

            if (fineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, dataCompare))
            {
                if (rmsQuotaBCodGestione1.GetValueOrDefault() == 0 && rmsQuotaBCodGestione2.GetValueOrDefault() == 0 && rmsQuotaBCodGestione3.GetValueOrDefault() == 0 && rmsQuotaBCodGestione4.GetValueOrDefault() == 0 &&
                    settimaneCodGestione1.GetValueOrDefault() == 0 && settimaneCodGestione2.GetValueOrDefault() == 0 && settimaneCodGestione3.GetValueOrDefault() == 0 && settimaneCodGestione4.GetValueOrDefault() == 0 &&
                    settimaneQuotaDCodGestione1.GetValueOrDefault() == 0 && settimaneQuotaDCodGestione2.GetValueOrDefault() == 0 && settimaneQuotaDCodGestione3.GetValueOrDefault() == 0 && settimaneQuotaDCodGestione4.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "R.M.S. mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se FL-P93FIA93-INA è uguale a "S" effettuare le seguenti operazioni :                                                              
        /// Se  (IW1RETCDM + IW1RETART + IW1RETCOM + IW1RETOBG) è uguale a zero e (INIASS è inferiore a 19960101 e FINASS è maggiore di 19930101) valorizzare con "37" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con  24 il campo COL-ERR1, con "DEVE ESSERE PRESENTE ALMENO UNA REGISTRAZIONE DAL 01.01.93" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimaneTotaliQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaInizioAndFineAssicurazioneWithSettimaneTotaliQuotaB(DateTime? decorrenza, DateTime? inizioAssicurazione, DateTime? fineAssicurazione, string naturaPensione, int? settimaneTotaliQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompareInizioAss = new DateTime(1996, 01, 01);
            DateTime dataCompareFineAss = new DateTime(1993, 01, 01);

            if (FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione))
            {
                if (settimaneTotaliQuotaB.GetValueOrDefault() == 0 && inizioAssicurazione.HasValue && !Utility.DataSuccessivaA(inizioAssicurazione.Value, dataCompareInizioAss) &&
                    fineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, dataCompareFineAss))
                {
                    messaggioVideo = "Deve essere presente almeno una registrazione dal 01/01/93";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se ICISTCDM012 è maggiore di zero effettuare le seguenti operazioni : 
        /// Se FINASS è inferiore a 20120101 valorizzare con "88" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 75 il campo COL-ERR1, con "(quota D) IMPORTO INCOMPATIBILE  CON DATA ULT.CONTRIBUTO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICICONCDM012 è uguale a zero valorizzare con "88" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 MANCANTE" il campo MESSAGGIO-ERRORE , con 1 il campo      FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICIRETCDM012 è uguale a zero valorizzare con "89" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICIRETCDM012 è inferiore a ICICONCDM012  valorizzare con "89" il campo       TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MINORE DI IMPORTO CONTRIBUTI 335" il campo         MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                           
        /// Diversamente da quanto analizzato nel punto precedente (108) effettuare le seguenti operazioni : 
        /// Se ICICONCDM012 è maggiore di zero valorizzare con "90" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 INCOMPATIBILE CON SETTIMANE 335" il campo              MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICIRETCDM012 è maggiore di zero valorizzare con "91" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 INCOMPATIBILE CON SETTIMANE 335" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);

        /// Se ICISTART012 è maggiore di zero  effettuare le seguenti operazioni :  
        /// Se FINASS è inferiore a 20120101 valorizzare con "13" il campo  TIPO-ERRORE, con 17 il campo RIG-ERRORE, con 75 il campo COL-ERR1, con "(quota D) IMPORTO INCOMPATIBILE CON DATA ULT.CONTRIBUTO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICICONART012 è uguale a zero valorizzare con "88" il campo TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 M      ANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1:                           
        /// Se ICIRETART012 è uguale a zero valorizzare con "89" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICIRETART012 è inferiore a ICICONART012 valorizzare con "89" il campo           TIPO-ERRORE valorizzare con 13 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MINORE DI IMPORTO CONTRIBUTI 335" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Diversamente da quanto analizzato nel punto precedente (110) effettuare le seguenti operazioni :                                                       
        /// Se ICICONART012 è maggiore di zero valorizzare con "90" il campo TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 INCOMPATIBILE CON SETTIMANE 335" il campo              MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                           
        /// Se ICIRETART012 è maggiore di zero valorizzare con "91" il campo TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 INCOMPATIBILE CON SETTIMANE 335" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);

        /// Se ICISTCOM012 è maggiore di zero  effettuare le seguenti operazioni : 
        /// Se FINASS è inferiore a 20120101 valorizzare con "88" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 75 il campo COL-ERR1, con "(quota D) IMPORTO INCOMPATIBILE CON DATA ULT.CONTRIBUTO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se ICICONCOM012 è uguale a zero valorizzare con "88" il campo TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo        FLAG-ERR e uscire da  CONTROLLI-1
        /// Se ICIRETCOM012 è uguale a zero effettuare le seguenti operazioni valorizzare con "89" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// Se ICIRETCOM012 è inferiore a ICICONCOM012 valorizzare con "89" il campo       TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 MINORE DI IMPORTO CONTRIBUTI 335" il campo        MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);                           
        /// Diversamente da quanto analizzato nel punto precedente (112) eseguire le seguenti operazioni :  
        /// Se ICICONCOM012 è maggiore di zero valorizzare con "90" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 49 il campo COL-ERR1, con "(quota D) IMPORTO CONTRIBUTI 335 INCOMPATIBILE CON SETTIMANE 335" il campo                 MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se ICIRETCOM012 è maggiore di zero valorizzare con "91" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "(quota D) MONTANTE 335 INCOMPATIBILE CON SETTIMANE 335" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// </summary>
        /// <param name="settimaneQuotaDCodGestione2"></param>
        /// <param name="importoQuotaDCodGestione2"></param>
        /// <param name="montanteQuotaDCodGestione2"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneAndImportoAndMontanteQuotaD(int? settimaneQuotaDCodGestione, decimal? importoQuotaDCodGestione, decimal? montanteQuotaDCodGestione, DateTime? fineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(2012, 01, 01);

            if (settimaneQuotaDCodGestione.HasValue && settimaneQuotaDCodGestione.Value > 0)
            {
                if (fineAssicurazione.HasValue && !Utility.DataSuccessivaA(fineAssicurazione.Value, dataCompare))
                {
                    messaggioVideo = "Quota D - Importo incompatibile con data ultimo contributo";
                    return false;
                }

                if (importoQuotaDCodGestione.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Quota D - Importo contributi 335 mancante";
                    return false;
                }

                if (montanteQuotaDCodGestione.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Quota D - Montante 335 mancante";
                    return false;
                }

                if (montanteQuotaDCodGestione.HasValue && importoQuotaDCodGestione.HasValue && montanteQuotaDCodGestione.Value < importoQuotaDCodGestione.Value)
                {
                    messaggioVideo = "Quota D - Montante 335 minore di Importo contributi 335";
                    return false;
                }
            }
            else
            {
                if (importoQuotaDCodGestione.HasValue && importoQuotaDCodGestione.Value > 0)
                {
                    messaggioVideo = "Quota D - Importo contributi 335 incompatibile con settimane 335";
                    return false;
                }

                if (montanteQuotaDCodGestione.HasValue && montanteQuotaDCodGestione.Value > 0)
                {
                    messaggioVideo = "Quota D - Montante 335 incompatibile con settimane 335";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se  (APPO-CAT1 è uguale a "V" e (APP-APP  + ICI2SETFIT)  è inferiore a 1820)  e  (IABCONA2 è uguale a  "1" oppure "2") e (IABCONA4 non è uguale a "L") e (IABCONA4  non è uguale a "Z" )  valorizzare con "52" il campo  TIPO-ERRORE, con  4 il campo  RIG-ERRORE, con 69 il campo COL-ERR1, con "ANZIANITA': SETTIMANE INFERIORI A 1820" il campo MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);       
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="settimaneEstere"></param>
        /// <param name="settimaneItaliane"></param>
        /// <param name="settimaneVVDiritto"></param>
        /// <param name="settGodimentoAssegno"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneWithNaturaPensione(string gruppo, int? settimaneEstere, int? settimaneItaliane, int? settimaneVVDiritto, int? settGodimentoAssegno, int? settimaneFittizie, string naturaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int somma = settimaneEstere.GetValueOrDefault() + settimaneItaliane.GetValueOrDefault() + settimaneVVDiritto.GetValueOrDefault() + settGodimentoAssegno.GetValueOrDefault() + settimaneFittizie.GetValueOrDefault();
            if (gruppo.Equals("0001") && somma < 1820 && !string.IsNullOrEmpty(naturaPensione) && (naturaPensione.Substring(0, 1).Equals("1") || naturaPensione.Substring(0, 1).Equals("2")) &&
                (!naturaPensione.Substring(2, 1).Equals("L") && !naturaPensione.Substring(2, 1).Equals("Z")))
            {
                messaggioVideo = "Anzianità: settimane inferiori a 1820";
                return false;
            }


            return true;
        }

        /// <summary>
        /// Se W-DEORIG è maggiore di 199212 e ICI2SETFIT è maggiore di zero e ICIMMF è uguale a zero effettuare le seguenti operazioni :                                                             
        /// Se IW1STCDM è uguale a zero valorizzare con "56" il campo TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con APPO-CAT + " CON SETT.FITTIZIE: MANCA CMSM O R.M.P" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                        
        /// Diversamente da quanto analizzato nel punto precedente valorizzare con "56" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con APPO-CAT  +  " CON SETT.FITTIZIE: MANCA R.M.P. NELLA GESTIONE 62"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                            
        /// Se  (IW1CATPEN è uguale a 88 oppure 89 oppure 90)  e IW1RETART è uguale a zero effettuare le seguenti operazioni :  
        /// Se IW1START è uguale a zero valorizzare con "56" il campo  TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con APPO-CAT +     " CON SETT.FITTIZIE: MANCA CMSM O R.M.P" il campo              MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);  
        /// Diversamente da quanto analizzato nel punto precedente valorizzare con "56" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con APPO-CAT   +  " CON SETT.FITTIZIE: MANCA R.M.P. NELLA GESTIONE 63" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se  (IW1CATPEN è uguale a 91 oppure 92 oppure 93)  e IW1RETCOM  è uguale a zero effettuare le seguenti operazioni : 
        /// Se IW1STCOM è uguale a zero valorizzare con "56" il campo  TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con APPO-CAT   +   " CON SETT.FITTIZIE: MANCA CMSM O R.M.P"  il campo             MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195); 
        /// Diversamente da quanto analizzato nel punto precedente valorizzare con "56" il campo  TIPO-ERRORE, con 9 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con APPO-CAT   +   " CON SETT.FITTIZIE: MANCA R.M.P. NELLA GESTIONE 64"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="cmsm"></param>
        /// <param name="categoria"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="settimaneQuotaB"></param>
        /// <param name="codiceGestione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneFittizieWithCmsmAndRMS(DateTime? decorrenza, int? settimaneFittizie, decimal? cmsm, int categoria, decimal? rmsQuotaB, int? settimaneQuotaB, short codiceGestione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(2012, 12, 01);

            int categoria1 = 0;
            int categoria2 = 0;
            int categoria3 = 0;
            if (codiceGestione == 2)
            {
                categoria1 = 85;
                categoria2 = 86;
                categoria3 = 87;
            }
            if (codiceGestione == 3)
            {
                categoria1 = 88;
                categoria2 = 89;
                categoria3 = 90;
            }
            if (codiceGestione == 4)
            {
                categoria1 = 91;
                categoria2 = 92;
                categoria3 = 93;
            }

            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, dataCompare) && settimaneFittizie.GetValueOrDefault() > 0 && settimaneFittizie.GetValueOrDefault() == 0)
            {
                if ((categoria == categoria1 || categoria == categoria2 || categoria == categoria3) && rmsQuotaB.GetValueOrDefault() == 0)
                {
                    if (settimaneQuotaB.GetValueOrDefault() == 0)
                    {
                        messaggioVideo = "Settimane Fittizie: CMSM o R.M.S. mancante";
                        return false;
                    }
                    else
                    {
                        messaggioVideo = "Settimane Fittizie: R.M.S. mancante per il codice gestione " + codiceGestione;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se ( ICIMMF è maggiore di zero e ICI2SETFIT è uguale a zero )  e IW1DEORIG è inferiore a 201201  valorizzare con "63" il campo  TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con "CMSM INCOMPATIBILE CON SETTIMANE FITTIZIE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);  
        /// </summary>
        /// <param name="cmsm"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="decorrenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCmsmWithSettimaneFittizie(decimal? cmsm, int? settimaneFittizie, DateTime? decorrenza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(2012, 01, 01);

            if (cmsm.GetValueOrDefault() > 0 && settimaneFittizie.GetValueOrDefault() == 0 && decorrenza.HasValue && !Utility.DataSuccessivaA(decorrenza.Value, dataCompare))
            {
                messaggioVideo = "CMSM incompatibile con settimane fittizie";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se (W-DEORIG è maggiore di 199600 e ICIMMF è uguale a zero e ICI2SETFIT è maggiore di zero e FINASS è maggiore di 19960100)  e (ICICONCDM335 è maggiore di zero oppure ICICONART335 è maggiore di zero oppure ICICONCOM335 è maggiore di zero oppure ICICONOBG335 è maggiore di zero) valorizzare con "64" il campo  TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con "CMSM MANCANTE O INCOMPATIBILE CON SETTIMANE FITTIZIE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                              
        /// </summary>
        /// <param name="cmsm"></param>
        /// <param name="decorrenza"></param>
        /// <param name="settimaneFittizie"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="importoContributivoTotaleCodGesione1"></param>
        /// <param name="importoContributivoTotaleCodGesione2"></param>
        /// <param name="importoContributivoTotaleCodGesione3"></param>
        /// <param name="importoContributivoTotaleCodGesione4"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCmsmWithSettimaneFittizieAndImportiContribTot(decimal? cmsm, DateTime? decorrenza, int? settimaneFittizie, DateTime? fineAssicurazione, decimal? importoContributivoTotaleCodGestione1,
            decimal? importoContributivoTotaleCodGestione2, decimal? importoContributivoTotaleCodGestione3, decimal? importoContributivoTotaleCodGestione4, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1996, 01, 01);

            if ((decorrenza.HasValue && Utility.DataSuccessivaA(decorrenza.Value, dataCompare) && cmsm.GetValueOrDefault() == 0 &&
                settimaneFittizie.GetValueOrDefault() > 0 && fineAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(fineAssicurazione.Value, dataCompare)) &&
                (importoContributivoTotaleCodGestione1.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione2.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione3 > 0 || importoContributivoTotaleCodGestione4.GetValueOrDefault() > 0))
            {
                messaggioVideo = "CMSM mancante o incompatibile con settimane fittizie e/o importo contributivo totale";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se  ( INIASS è maggiore di 19960100 oppure FINASS è inferiore a 19930101 )  e ( ITOT-EST-95 è inferiore a 936 )  effettuare le seguenti operazioni :  
        /// Se IW1STCDM è maggiore di zero valorizzare con "71" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "CONTR. CD/CM DAL 1993 INCOMPATIBILI CON PERIODO ASSICURATIVO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);  
        /// Se IW1START è maggiore di zero valorizzare con "71" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "CONTR. ART DAL 1993 INCOMPATIBILI CON PERIODO ASSICURATIVO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                              
        /// Se IW1STCOM è maggiore di zero valorizzare con "71" il campo TIPO-ERRORE, con 9 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "CONTR. COM DAL 1993 INCOMPATIBILI CON PERIODO ASSICURATIVO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);                              
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="contributiItalianiEsteri1295"></param>
        /// <param name="settimaneQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContribItalianiEsteri1295WithPeriodoAss(DateTime? inizioAssicurazione, DateTime? fineAssicurazione, int? contributiItalianiEsteri1295, int? settimaneQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompareInizioAss = new DateTime(1996, 01, 01);
            DateTime dataCompareFineAss = new DateTime(1993, 01, 01);

            if (((inizioAssicurazione.HasValue && Utility.DataSuccessivaA(inizioAssicurazione.Value, dataCompareInizioAss)) || (fineAssicurazione.HasValue && !Utility.DataSuccessivaA(fineAssicurazione.Value, dataCompareFineAss))) &&
                contributiItalianiEsteri1295.GetValueOrDefault() < 936)
            {
                if (settimaneQuotaB.GetValueOrDefault() > 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 181.	Se  ( INIASS è maggiore di 19960100 oppure FINASS è inferiore a 19930101 )  e ( ITOT-EST-95 è inferiore a 936 )  effettuare le seguenti operazioni :  
        /// 181.4.	Se IW1STOBG è maggiore di zero  effettuare le seguenti operazioni : 
        /// 181.4.1.	Se DAFELPE-DATA è maggiore di '20110101'  continuare l’elaborazione al punto successivo, diversamente , con "71" il campo  TIPO-ERRORE, con 10 il campo 
        /// RIG-ERRORE, con 76 il campo COL-ERR1, con "CONTR. AGO DAL 1993 INCOMPATIBILI CON PERIODO ASSICURATIVO" il campo      MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire 
        /// da  CONTROLLI-1 (195); 
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="contributiItalianiEsteri1295"></param>
        /// <param name="settimaneQuotaBCodGestione1"></param>
        /// <param name="dataCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneQuotaBWithPeriodoAssicurativo(DateTime? inizioAssicurazione, DateTime? fineAssicurazione, int? contributiItalianiEsteri1295, int? settimaneQuotaBCodGestione1,
            DateTime? dataCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompareInizioAss = new DateTime(1996, 01, 01);
            DateTime dataCompareFineAss = new DateTime(1993, 01, 01);

            if (((inizioAssicurazione.HasValue && Utility.DataSuccessivaA(inizioAssicurazione.Value, dataCompareInizioAss)) ||
                (fineAssicurazione.HasValue && !Utility.DataSuccessivaA(fineAssicurazione.Value, dataCompareFineAss))) &&
                contributiItalianiEsteri1295.GetValueOrDefault() < 936)
            {
                if (settimaneQuotaBCodGestione1.GetValueOrDefault() > 0)
                {
                    if (dataCalcolo.HasValue && !Utility.DataStrettamenteSuccessivaA(dataCalcolo.Value, new DateTime(2011, 01, 31)))
                    {
                        messaggioVideo = "Contr. AGO dal 1993 incompatibili con periodo assicurativo";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se ICIMMF è maggiore di zero effettuare le seguenti operazioni :  
        /// Se IABCONA2 non è uguale a 3 e 4  valorizzare con "73" il campo  TIPO-ERRORE, con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con "CMSM L.335 INCOMPATIBILE CON CATEGORIA DI PENSIONE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195); 
        /// Se W-DEORIG è inferiore a 199601 valorizzare con "74" il campo  TIPO-ERRORE , con 6 il campo RIG-ERRORE, con 63 il campo COL-ERR1, con "CMSM L.335 INCOMPATIBILE CON DECORRENZA PENSIONE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLi-1;                              
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="cmsm"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCmsmWithDecorrenza(DateTime? decorrenza, decimal? cmsm, string naturaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1996, 01, 01);

            if (cmsm.GetValueOrDefault() > 0)
            {
                if (!string.IsNullOrEmpty(naturaPensione) && !naturaPensione.Substring(0, 1).Equals("3") && !naturaPensione.Substring(0, 1).Equals("4"))
                {
                    messaggioVideo = "CMSM L.335 incompatibile con categoria di pensione";
                    return false;
                }

                if (decorrenza.HasValue && !Utility.DataSuccessivaA(decorrenza.Value, dataCompare))
                {
                    messaggioVideo = "CMSM L.335 incompatibile con decorrenza pensione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se INIASS è maggiore di 19951231  effettuare le seguenti operazioni :  
        /// Se ITOT-EST-95 è maggiore di 935  e IABCONA3 non è uguale a 'O'  e INIASS è inferiore a  20120101 effettuare le seguenti operazioni :  
        /// Se ICISTOBG335  è maggiore di zero oppure ICICONOBG335 è maggiore di zero oppure ICIRETOBG335 è maggiore di zero oppure ICISTART335 è maggiore di zero oppure ICICONART335 è maggiore di zero oppure ICIRETART335 è maggiore di zero oppure ICISTCOM335 è maggiore di zero oppure ICICONCOM335 è maggiore di zero oppure ICIRETCOM335 è maggiore di zero oppure ICISTCDM335 è maggiore di zero oppure ICICONCDM335 è maggiore di zero oppure ICIRETCDM335 è maggiore di zero valorizzare con "96" il campo  TIPO-ERRORE, con 19 il campo           RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.335/95"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// 193.1.2.	Se  ( (GEST233(1 1) è maggiore di zero ed è inferiore a 61 )  oppure (GEST233(1 2) è maggiore di zero ed è inferiore a       61 )   oppure (GEST233(2 1) è maggiore di zero ed è inferiore a       61 )  oppure (GEST233(2 2) è maggiore di zero ed è inferiore a       61 )  oppure (GEST233(3 1) è maggiore di zero ed è inferiore a       61 )  oppure (GEST233(3 2) è maggiore di zero ed è inferiore a       61 ) ) valorizzare con "96" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON GESTIONE CTR ESTERI(= 0X)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se (IW1STOBG è uguale a zero oppure IW1RETOBG è uguale a zero )  e  ( IW1STCDM è uguale a zero oppure IW1RETCDM è uguale a zero )  e  ( IW1START è uguale a zero  oppure IW1RETART è uguale a zero ) e  ( IW1STCOM è uguale a zero      oppure IW1RETCOM è uguale a zero )  valorizzare con "96" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.503/92" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// 193.1.4.	Se  (GEST233(1 1) è maggiore di zero)  e  (GEST233(1 1) è inferiore a 61 oppure è maggiore di 65 )  valorizzare con "96" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON GESTIONE CTR ESTERI(<> 6X)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// Se ITOT-EST-95 è inferiore a 936 effettuare le seguenti operazioni :  
        /// Se IW1STOBG è maggiore di zero oppure IW1RETOBG è maggiore di zero oppure ICI1VVOBG è maggiore di zero oppure IW1STCDM è maggiore di zero oppure IW1RETCDM è maggiore di zero oppure IW1START è maggiore di zero oppure IW1RETART è maggiore di zero oppure IW1STCOM è maggiore di zero oppure IW1RETCOM è maggiore di zero valorizzare con "97" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.503/92" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// 193.2.2.	Se  ( (GEST233(1 1) è maggiore di 61 ed è inferiore a 65) oppure (GEST233(1 2) è maggiore di 61 ed è inferiore a 65) oppure (GEST233(2 1) è maggiore di 61 ed è inferiore a 65) oppure (GEST233(2 2) è maggiore di 61 ed è inferiore a 65) oppure (GEST233(3 1) è maggiore di 61 ed è inferiore a 65) oppure (GEST233(3 2) è maggiore di 61 ed è inferiore a 65 ) ) valorizzare con "97" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON GESTIONE CTR ESTERI(= 6X)" il campo        MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// Se  ( ICISTOBG335 è uguale a zero oppure ICIRETOBG335 è uguale a zero) e ( ICISTCDM335 è uguale a zero oppure ICIRETCDM335 è uguale a zero) e     ( ICISTART335 è uguale a zero oppure ICIRETART335 è uguale a zero) e         ( ICISTCOM335 è uguale a zero oppure ICIRETCOM335 è uguale a zero) valorizzare con "97" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo  COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMPATIBILI CON DATI PER L.335/95" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);
        /// 193.2.4.	Se (GEST233(1 1) è maggiore di 4) oppure ( (GEST233(1 1) è inferiore a 1) e ART48(1) non è uguale a "S") valorizzare con "97" il campo  TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITAL.ESTERI AL 31.12.95 INCOMP. CON  CTR ESTERI (GEST 01/4 MANCANTE)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// Se ITOT-EST-95 è uguale a zero effettuare le seguenti operazioni :  
        /// Se APPO-CAT1 è uguale a "V"  e  (IABCONA2 è uguale a "1" oppure "2") valorizzare con "98" il campo TIPO-ERRORE, con 19 il campo RIG-ERRORE, con 62 il campo COL-ERR1, con "CTR ITALIANI ED ESTERI AL 31.12.95 MANCANTI PER PENS. DI ANZIANITA'" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                            
        /// </summary>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="contributiItalianiEsteri1295"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimaneQuotaC"></param>
        /// <param name="importoContributivoTotale"></param>
        /// <param name="montante"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContribItalianiEsteri1295WithLegge335(GestionePensione.DatiPensione datiPensione, int? contributiItalianiEsteri1295,
            int? settimaneQuotaCCodGestione1, decimal? montanteCodGestione1, decimal? importoContributivoTotaleCodGestione1, int? settimaneQuotaCCodGestione2, decimal? montanteCodGestione2,
            decimal? importoContributivoTotaleCodGestione2, int? settimaneQuotaCCodGestione3, decimal? montanteCodGestione3, decimal? importoContributivoTotaleCodGestione3,
            int? settimaneQuotaCCodGestione4, decimal? montanteCodGestione4, decimal? importoContributivoTotaleCodGestione4, int? settimaneQuotaBCodGestione1, decimal? rmsQuotaBCodGestione1,
            int? settimaneQuotaBCodGestione2, decimal? rmsQuotaBCodGestione2, int? settimaneQuotaBCodGestione3, decimal? rmsQuotaBCodGestione3, int? settimaneQuotaBCodGestione4,
            decimal? rmsQuotaBCodGestione4, int? vvMisuraDL50392, bool isCodiceGestione0XPresente, bool isCodiceGestione6XPresente, short? primoCodiceGestioneTraduzioneSuGP, char? codiceArt48PrimoStato, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare1 = new DateTime(1995, 12, 31);
            DateTime dataCompare2 = new DateTime(2012, 01, 30);
            DateTime dataCompare3 = new DateTime(2012, 01, 01);

            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Contributi_95_Incompatibili.CONTRIBUTI_95_INCOMPATIBILI))
            {
                if (datiPensione.InizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.Value, dataCompare1))
                {
                    if (contributiItalianiEsteri1295.GetValueOrDefault() > 935 && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && !datiPensione.NaturaPensione.Substring(1, 1).Equals("O") && !Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, dataCompare2))
                    {
                        if ((settimaneQuotaCCodGestione1.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione1.GetValueOrDefault() > 0 || montanteCodGestione1.GetValueOrDefault() > 0 ||
                            settimaneQuotaCCodGestione2.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione2.GetValueOrDefault() > 0 || montanteCodGestione2.GetValueOrDefault() > 0 ||
                            settimaneQuotaCCodGestione3.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione3.GetValueOrDefault() > 0 || montanteCodGestione3.GetValueOrDefault() > 0 ||
                            settimaneQuotaCCodGestione4.GetValueOrDefault() > 0 || importoContributivoTotaleCodGestione4.GetValueOrDefault() > 0 || montanteCodGestione4.GetValueOrDefault() > 0) &&
                            Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        {
                            messaggioVideo = "Contributi italiani ed esteri al 31/12/1995 incompatibili con dati per L.335/95";
                            return false;
                        }

                        if (isCodiceGestione0XPresente && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        {
                            messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con gestione CTR Esteri(= 0X)";
                            return false;
                        }

                        if ((rmsQuotaBCodGestione1.GetValueOrDefault() == 0 &&
                            (settimaneQuotaBCodGestione2.GetValueOrDefault() == 0 || rmsQuotaBCodGestione2.GetValueOrDefault() == 0) &&
                            (settimaneQuotaBCodGestione3.GetValueOrDefault() == 0 || rmsQuotaBCodGestione3.GetValueOrDefault() == 0) &&
                            (settimaneQuotaBCodGestione4.GetValueOrDefault() == 0 || rmsQuotaBCodGestione4.GetValueOrDefault() == 0)))
                        {
                            messaggioVideo = "Contributi italiani ed esteri al 31/12/1995 incompatibili con dati per L.503/92";
                            return false;
                        }

                        if (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 0 && (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() < 61 || primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 65))
                        {
                            messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con gestione CTR Esteri(<> 6X)";
                            return false;
                        }
                    }

                    if (contributiItalianiEsteri1295.GetValueOrDefault() < 936)
                    {
                        if ((settimaneQuotaBCodGestione1.GetValueOrDefault() > 0 || vvMisuraDL50392.GetValueOrDefault() > 0 || settimaneQuotaBCodGestione2.GetValueOrDefault() > 0 ||
                            settimaneQuotaBCodGestione3.GetValueOrDefault() > 0 || settimaneQuotaBCodGestione4.GetValueOrDefault() > 0))
                        {
                            messaggioVideo = "Contributi italiani ed esteri al 31/12/1995 incompatibili con dati per L.503/92";
                            return false;
                        }

                        if (isCodiceGestione6XPresente)
                        {
                            messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con gestione CTR Esteri(= 6X)";
                            return false;
                        }

                        if (!(Utility.DataSuccessivaA(datiPensione.InizioAssicurazione.Value, dataCompare3) && (datiPensione.SiglaCategoria.Trim() == "VOS" || datiPensione.SiglaCategoria.Trim() == "VOCOMS" ||
                            datiPensione.SiglaCategoria.Trim() == "VRS" || datiPensione.SiglaCategoria.Trim() == "VOARTS"
                            || datiPensione.SiglaCategoria.Trim() == "IOS" || datiPensione.SiglaCategoria.Trim() == "IRS" || datiPensione.SiglaCategoria.Trim() == "IOCOMS"
                            || datiPensione.SiglaCategoria.Trim() == "IOARTS")) &&
                            ((settimaneQuotaCCodGestione1.GetValueOrDefault() == 0 || montanteCodGestione1.GetValueOrDefault() == 0) &&
                            (settimaneQuotaCCodGestione2.GetValueOrDefault() == 0 || montanteCodGestione2.GetValueOrDefault() == 0) &&
                            (settimaneQuotaCCodGestione3.GetValueOrDefault() == 0 || montanteCodGestione3.GetValueOrDefault() == 0) &&
                            (settimaneQuotaCCodGestione4.GetValueOrDefault() == 0 || montanteCodGestione4.GetValueOrDefault() == 0)) &&
                            Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        {
                            messaggioVideo = "Contributi italiani ed esteri al 31/12/1995 incompatibili con dati per L.335/95";
                            return false;
                        }

                        if (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() > 4 || (primoCodiceGestioneTraduzioneSuGP.GetValueOrDefault() < 1 && codiceArt48PrimoStato.GetValueOrDefault() != 'S'))
                        {
                            messaggioVideo = "CTR Ital.Esteri al 31/12/95 incompatibili con CTR Esteri (Gestione 01/4 mancante)";
                            return false;
                        }
                    }

                    if (contributiItalianiEsteri1295.GetValueOrDefault() == 0)
                    {
                        if (datiPensione.Gruppo.Equals("0001") && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && (datiPensione.NaturaPensione.Substring(0, 1).Equals("1") || datiPensione.NaturaPensione.Substring(0, 1).Equals("2")))
                        {
                            //ENG memo_28 in accordo con Accenture(segnalazione 40482)
                            //Il controllo va saltato per le contributive pure, in quanto i contribiti italiani ed esteri al 95 devono essere 0
                            if (!((datiPensione.Gruppo.Equals("0001") && datiPensione.Prodotto.Equals("0001") && datiPensione.Tipo.Equals("0017")) ||
                                (datiPensione.Gruppo.Equals("0001") && datiPensione.Prodotto.Equals("0002") && datiPensione.Tipo.Equals("0017"))))
                            {
                                messaggioVideo = "Contributi Italiani ed Esteri al 31/12/95 mancanti per Pensione di Anzianita'";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se  (OPZIONE-CONTRIBUTIVA è uguale a "S" oppure IABCONA3 è uguale a "O") effettuare le seguenti operazioni :  
        /// Se (IW1CATPEN è uguale a 85 oppure 86 oppure 87) e ICISTCDM335 è uguale a zero valorizzare con "50" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI GESTIONE CD/CM MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se  (IW1CATPEN è uguale a 88 oppure 89 oppure 90) e ICISTART335 è uguale a zero valorizzare con "50" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA:DATI GESTIONE ART MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);  
        /// Se  (IW1CATPEN è uguale a 91 oppure 92 oppure 93) e ICISTCOM335 è uguale a zero valorizzare con "50" il campo TIPO-ERRORE, con 14 il campo RIG-ERRORE, con 12 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI GESTIONE COM MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="codGestione"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="settimaneQuotaC"></param>
        /// <param name="settimaneQuotaD"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneContributive(int categoria, int codGestione, string naturaPensione, int? settimaneQuotaC, int? settimaneQuotaD, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string msgApp = string.Empty;

            int cat1 = 0;
            int cat2 = 0;
            int cat3 = 0;
            if (codGestione == 2)
            {
                cat1 = 85;
                cat2 = 86;
                cat3 = 87;
                msgApp = "CD/CM";
            }
            if (codGestione == 3)
            {
                cat1 = 88;
                cat2 = 89;
                cat3 = 90;
                msgApp = "ART";
            }
            if (codGestione == 4)
            {
                cat1 = 91;
                cat2 = 92;
                cat3 = 93;
                msgApp = "COM";
            }

            if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1).Equals("O"))
            {
                if ((categoria == cat1 || categoria == cat2 || categoria == cat3) && settimaneQuotaC.GetValueOrDefault() == 0 && settimaneQuotaD.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Opzione Contributiva: dati gestione " + msgApp + " mancanti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se  (OPZIONE-CONTRIBUTIVA è uguale a "S" oppure IABCONA3 è uguale a "O") effettuare le seguenti operazioni :  
        /// Se IW1RMSCDM è maggiore di zero valorizzare con "50" il campo TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 24 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI CD/CM AL 31/12/92 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RMSART è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 24 il campo    COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI ART AL 31/12/92 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RMSCOM è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE valorizzare con 9 il campo RIG-ERRORE, con 24 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI COM   AL 31/12/92 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RMSOBG è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE, con 10 il campo RIG-ERRORE, con 24 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI AGO AL 31/12/92 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RETCDM è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI CD/CM DAL 1/91/93 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RETART è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI ART DAL 1/91/93 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// Se IW1RETCOM è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE valorizzare con 9 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI COM DAL 1/91/93 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195); 
        /// Se IW1RETOBG è maggiore di zero valorizzare con "50" il campo  TIPO-ERRORE, con 10 il campo RIG-ERRORE, con 56 il campo COL-ERR1, con "OPZ. CONTRIBUTIVA: DATI AGO DAL 1/91/93 NON VANNO ACQUISITI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="codGestione"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRmsQuotaAandQuotaB(int codGestione, string naturaPensione, decimal? rmsQuotaA, decimal? rmsQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string msgApp = string.Empty;

            if (codGestione == 1)
                msgApp = "AGO";
            if (codGestione == 2)
                msgApp = "CD/CM";
            if (codGestione == 3)
                msgApp = "ART";
            if (codGestione == 4)
                msgApp = "COM";

            if (!string.IsNullOrEmpty(naturaPensione) && naturaPensione.Substring(1, 1).Equals("O"))
            {
                if (rmsQuotaA.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Opzione Contributiva: dati " + msgApp + " al 31/12/92 non vanno acquisiti";
                    return false;
                }

                if (rmsQuotaB.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Opzione Contributiva: dati " + msgApp + " dal 1/91/93 non vanno acquisiti";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se ICISTCDM335 è maggiore di APP-DATA-RC e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" valorizzare con "68" il campo  TIPO-ERRORE, con 12 il campo RIG-ERRORE,  con 38 il campo COL-ERR1, con "SETT. CD-CM SUPERIORI A CAPIENZA NEL PERIODO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// Se ICISTART335 è maggiore di APP-DATA-RC e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" valorizzare con "68" il campo  TIPO-ERRORE, con 13 il campo RIG-ERRORE,  con 38 il campo COL-ERR1, con "SETT. ART SUPERIORI A CAPIENZA NEL PERIODO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// Se ICISTCOM335 è maggiore di APP-DATA-RC e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" valorizzare con "68" il campo  TIPO-ERRORE, con 14 il campo RIG-ERRORE,  con 38 il campo COL-ERR1, con "SETT. COM SUPERIORI A CAPIENZA NEL PERIODO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);                             
        /// Se ICISTOBG335 è maggiore di APP-DATA-RC  e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" effettuare le seguenti operazioni : valorizzare con "68" il campo  TIPO-ERRORE, con 15 il campo RIG-ERRORE,  con 38 il campo COL-ERR1, con "SETT. AGO SUPERIORI A CAPIENZA NEL PERIODO"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// </summary>
        /// <param name="codGestione"></param>
        /// <param name="inizioAssicurazione"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="settimaneQuotaC"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneQuotaCWithCapienzaSett(int codGestione, DateTime? inizioAssicurazione, DateTime? fineAssicurazione, int? settimaneQuotaC, string naturaPensione,
            GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string msgApp = string.Empty;

            if (codGestione == 1)
                msgApp = "AGO";
            if (codGestione == 2)
                msgApp = "CD/CM";
            if (codGestione == 3)
                msgApp = "ART";
            if (codGestione == 4)
                msgApp = "COM";

            if (inizioAssicurazione.HasValue && fineAssicurazione.HasValue)
            {
                if (!GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Sett_Superiori_Capienza.SETT_SUPERIORI_CAPIENZA))
                {
                    int settimane = Utility.NSettimaneBetweenDate(fineAssicurazione.Value, inizioAssicurazione.Value);

                    if (settimaneQuotaC.GetValueOrDefault() > settimane && (!string.IsNullOrEmpty(naturaPensione) && !naturaPensione.Substring(2, 1).Equals("G") && !naturaPensione.Substring(2, 1).Equals("Z")))
                    {
                        messaggioVideo = "Settimane " + msgApp + " superiori a capienza nel periodo";
                        return false;
                    }
                }
            }
            else
            {
                messaggioVideo = "Settimane non calcolabili perchè inizioAssicurazione e/o fineAssicurazione non valorizzate.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 218.	Se  GEST233(INDICE   INDBIS) è maggiore di zero e FL-NO233  è uguale a "S"  valorizzare con 99 il campo  INDBIS, con "38" il campo  TIPO-ERRORE, con 20 il campo 
        /// RIG-ERRORE,  con  8 il campo     COL-ERR1, con "CONTRIBUTI ESTERI 233/503/335 INCOMPATIBILI CON DECORRENZA PENSIONE"  il campo MESSAGGIO-ERRORE e uscire da SETT-ESTERE 
        /// (242);
        /// 219.	Valorizzare con GEST233(INDICE INDBIS) il campo W-COD-GE; 
        /// 220.	Se ( W-COD-GE è inferiore a  5 ) oppure ( W-COD-GE è maggiore di 60 ed è inferiore a  65 )  oppure ( W-COD-GE è maggiore di 70 ed è inferiore a  75 )  continuare 
        /// l’elaborazione al punto successivo (221), diversamente effettuare le seguenti operazioni : 
        /// 220.1.	Valorizzare con "39" il campo  TIPO-ERRORE;
        /// 220.2.	Calcolare RIG-ERRORE  =   INDICE + 19 ;
        /// 220.3.	Se INDBIS è uguale a 1 valorizzare con  8 il campo COL-ERR1, diversamente valorizzarlo con 48;
        /// 220.4.	Valorizzare con 99 il campo  INDBIS, con "CODICE GESTIONE ERRATA " il campo MESSAGGIO-ERRORE e uscire da SETT-ESTERE (242);
        /// 222.	Se W-COD-GE1 è uguale a 7  e FL-P93INP93 è uguale a "S" valorizzare con "40" il campo  TIPO-ERRORE, con 20 il campo RIG-ERRORE;  
        /// 222.1.	Se INDBIS è uguale a 1 valorizzare con 8 il campo COL-ERR1 diversamente valorizzarlo con 48;
        /// 222.2.	Valorizzare con 99 il campo  INDBIS, con "CODICI 71-72-73-74 INCOMPATIBILI CON DATA FINE ASSICURAZIONE" il campo MESSAGGIO-ERRORE, con 1 il campo        FLAG-ERR 
        /// e uscire da SETT-ESTERE;
        /// </summary>
        /// <param name="codiceGestione"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="categoria"></param>
        /// <param name="dataMorte"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiItalianiEdEsteri(short? codiceGestione, DateTime? decorrenza, DateTime? decorrenzaDiretta, int categoria, DateTime? dataMorte, DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione, string codNatura, int? settimaneRicalcoloMisura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool flagNo233 = FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte);
            bool flagP93INP93 = FlagP93INP93(decorrenza, dataInizioAssicurazione);
            bool flagFIA93 = FlagFIA93(dataFineAssicurazione);
            bool flagP93FIA93INA = FlagP93FIA93INA(decorrenza, dataFineAssicurazione, codNatura);

            if (codiceGestione.GetValueOrDefault() > 0 && flagNo233)
            {
                messaggioVideo = "Contributi Esteri 233/503/335 incompatibili con Decorrenza Pensione";
                return false;
            }

            if (!(codiceGestione.GetValueOrDefault() < 5 || (codiceGestione.GetValueOrDefault() > 60 && codiceGestione.GetValueOrDefault() < 65) || (codiceGestione.GetValueOrDefault() > 70 && codiceGestione.GetValueOrDefault() < 75)))
            {
                messaggioVideo = "Codice Gestione errata";
                return false;
            }

            if (codiceGestione.GetValueOrDefault() / 10 == 7 && flagP93INP93)
            {
                messaggioVideo = "Codici 71-72-73-74 incompatibile con data Inizio Assicurazione";
                return false;
            }
            else
            {
                if (codiceGestione.GetValueOrDefault() / 10 == 6 && flagFIA93 && !flagP93FIA93INA)
                {
                    messaggioVideo = "Codici 61-62-63-64 incompatibili con data Fine Assicurazione";
                    return false;
                }
            }

            if (!VerificaObbligatorietaContributiItalianiEdEsteri(codiceGestione, decorrenza, decorrenzaDiretta, categoria, dataMorte, settimaneRicalcoloMisura, out messaggioVideo))
                return false;

            return true;
        }

        /// <summary>
        /// 221.	Se (INDICE è uguale a 1 e INDBIS è uguale a 1)  e  (W-COD-GE non è maggiore di zero)  e       FL-NO233 non è uguale a "S"  e  APP-CAL-EST-R è maggiore di zero 
        /// valorizzare con 99 il campo INDBIS, con "39" il campo  TIPO-ERRORE , con 20 il campo RIG-ERRORE,  con  8 il campo COL-ERR1, con "REGISTRAZIONE AL 07.90 O DECORENZA ORIG. 
        /// MANCANTE"  il campo MESSAGGIO-ERRORE e uscire da SETT-ESTERE (242); 
        /// </summary>
        /// <param name="codiceGestione"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="categoria"></param>
        /// <param name="dataMorte"></param>
        /// <param name="settimaneRicalcoloMisura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaContributiItalianiEdEsteri(short? codiceGestione, DateTime? decorrenza, DateTime? decorrenzaDiretta, int categoria, DateTime? dataMorte, int? settimaneRicalcoloMisura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool flagNo233 = FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte);

            if (codiceGestione.GetValueOrDefault() == 0 && !flagNo233 && settimaneRicalcoloMisura.GetValueOrDefault() > 0)
            {
                messaggioVideo = "Registrazione al 07/1990 o Decorrenza Originaria mancante";
                return false;
            }

            return true;
        }

        /// <summary>
        /// GEST-ESTERA.                                                     
        /// 243.	Se W-COD-GE è uguale a 01 effettuare le seguenti operazioni : 
        /// 243.1.	Se ICIRETOBG335 è uguale a zero e ICIRETOBG012 è uguale a zero valorizzare con "99" il campo TIPO-ERRORE; 
        /// 243.2.	Uscire da GEST-ESTERA (255);     
        /// 244.	Se W-COD-GE è uguale a 02 effettuare le seguenti operazioni :                                                            
        /// 244.1.	Se ICIRETCDM335 è uguale a zero e ICIRETCDM012 è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;
        /// 244.2.	Uscire da GEST-ESTERA (255);     
        /// 245.	Se W-COD-GE è uguale a 03 effettuare le seguenti operazioni :                                                            
        /// 245.1.	Se ICIRETART335 è uguale a zero e ICIRETART012 è uguale a zero valorizzare con "99" il campo TIPO-ERRORE                   
        /// 245.2.	Uscire da GEST-ESTERA (255);     
        /// 246.	Se W-COD-GE è uguale a 04 effettuare le seguenti operazioni :                                                            
        /// 246.1.	Se ICIRETCOM335 è uguale a zero e ICIRETCOM012 è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;
        /// 246.2.	Uscire da GEST-ESTERA (255);     
        /// 247.	Se W-COD-GE è uguale a 61 effettuare le seguenti operazioni : 
        /// 247.1.	Se IW1RETOBG è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;
        /// 247.2.	Uscire da GEST-ESTERA (255); 
        /// 248.	Se W-COD-GE è uguale a 62 effettuare le seguenti operazioni : 
        /// 248.1.	Se IW1RETCDM è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;
        /// 248.2.	Uscire da GEST-ESTERA (255);     
        /// 249.	Se W-COD-GE è uguale a 63 effettuare le seguenti operazioni :                                                            
        /// 249.1.	Se IW1RETART è uguale a zerovalorizzare con "99" il campo TIPO-ERRORE;
        /// 249.2.	Uscire da GEST-ESTERA (255);
        /// 250.	Se W-COD-GE è uguale a 64 effettuare le seguenti operazioni :                                                            
        /// 250.1.	Se IW1RETCOM è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;                                            
        /// 250.2.	Uscire da GEST-ESTERA (255);
        /// 251.	Se W-COD-GE è uguale a 71 effettuare le seguenti operazioni :
        /// 251.1.	Se IW1RMSOBG è uguale a zero valorizzare con "99" il campo TIPO-ERRORE
        /// 251.2.	Uscire da GEST-ESTERA (255);     
        /// 252.	Se W-COD-GE è uguale a 72 effettuare le seguenti operazioni :
        /// 252.1.	Se IW1RMSCDM è uguale a zero valorizzare con "99" il campo TIPO-ERRORE 
        /// 252.2.	Uscire da GEST-ESTERA (255);
        /// 253.	Se W-COD-GE è uguale a 73 effettuare le seguenti operazioni :                                                            
        /// 253.1.	Se IW1RMSART è uguale a zero valorizzare con "99" il campo TIPO-ERRORE;
        /// 253.2.	Uscire da GEST-ESTERA (255);
        /// 254.	Se W-COD-GE è uguale a 74 effettuare le seguenti operazioni :                                                            
        /// 254.1.	Se IW1RMSCOM è uguale a zero valorizzare con "99" il campo TIPO-ERRORE; 
        /// 255.	Fine GEST-ESTERA. 
        /// </summary>
        /// <param name="codiceGestione"></param>
        /// <param name="montanteContributivoQuotaCCodGestione1"></param>
        /// <param name="montanteQuotaDL214CodGestione1"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMSWithContributiItalianiEdEsteri(short? codiceGestione, decimal? montanteContributivoQuotaCCodGestione1, decimal? montanteQuotaDL214CodGestione1, decimal? montanteContributivoQuotaCCodGestione2, decimal? montanteQuotaDL214CodGestione2, decimal? montanteContributivoQuotaCCodGestione3, decimal? montanteQuotaDL214CodGestione3, decimal? montanteContributivoQuotaCCodGestione4, decimal? montanteQuotaDL214CodGestione4, decimal? rmsQuotaBCodGestione1, decimal? rmsQuotaBCodGestione2, decimal? rmsQuotaBCodGestione3, decimal? rmsQuotaBCodGestione4, decimal? rmsQuotaACodGestione1, decimal? rmsQuotaACodGestione2, decimal? rmsQuotaACodGestione3, decimal? rmsQuotaACodGestione4, out string messaggioVideo)
        {
            messaggioVideo = "Manca la corrispondente Gestione in Italia";

            if (codiceGestione.HasValue)
            {
                switch (codiceGestione)
                {
                    case 1:
                        if (montanteContributivoQuotaCCodGestione1.GetValueOrDefault() == 0 && montanteQuotaDL214CodGestione1.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 2:
                        if (montanteContributivoQuotaCCodGestione2.GetValueOrDefault() == 0 && montanteQuotaDL214CodGestione2.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 3:
                        if (montanteContributivoQuotaCCodGestione3.GetValueOrDefault() == 0 && montanteQuotaDL214CodGestione3.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 4:
                        if (montanteContributivoQuotaCCodGestione4.GetValueOrDefault() == 0 && montanteQuotaDL214CodGestione4.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 61:
                        if (rmsQuotaBCodGestione1.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 62:
                        if (rmsQuotaBCodGestione2.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 63:
                        if (rmsQuotaBCodGestione3.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 64:
                        if (rmsQuotaBCodGestione4.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 71:
                        if (rmsQuotaACodGestione1.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 72:
                        if (rmsQuotaACodGestione2.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 73:
                        if (rmsQuotaACodGestione3.GetValueOrDefault() == 0)
                            return false;
                        break;
                    case 74:
                        if (rmsQuotaACodGestione4.GetValueOrDefault() == 0)
                            return false;
                        break;
                }
            }

            messaggioVideo = string.Empty;

            return true;
        }

        /// <summary>
        /// 229.11.	Se APP-DEC è inferiore a W-DEORIG effettuare le seguenti operazioni : 
        /// 229.11.1.	Se (IW1DEBON  è maggiore di zero e APP-DEC è uguale a IW1DEBON)  continuare l’elaborazione al punto successivo (229.12) diversamente effettuare le seguenti 
        /// operazioni :            
        /// 229.11.1.1.	Valorizzare con "45" il campo  TIPO-ERRORE;                                
        /// 229.11.1.2.	Calcolare RIG-ERRORE = 19  +  INDICE;
        /// 229.11.1.3.	Se INDBIS è uguale a 1 valorizzare con 16 il campo COL-ERR1 diversamente valorizzarlo con 56;
        /// 229.11.1.4.	Valorizzare con "DECORRENZA ANTERIORE A DECORRENZA P      ENSIONE"  il campo MESSAGGIO-ERRORE, con 99 il campo  INDBIS e uscire da SETT-ESTERE;
        /// 229.12.	Se IW1DEBON è maggiore di zero valorizzare con IW1DEBON il campo APP-DEC-2;
        /// 229.13.	Se APP-DEC non è uguale a APP-DEC-2 effettuare le seguenti operazioni :            
        /// 229.13.1.	Valorizzare con "46" il campo TIPO-ERRORE;
        /// 229.13.2.	Calcolare RIG-ERRORE = 19  +  INDICE;
        /// 229.13.3.	Valorizzare con 16 il campo COL-ERR1, con 99 il campo  INDBIS;                                      
        /// 229.13.4.	Se W-COD-GE1 è uguale a 7 valorizzare con "MANCA REGISTRAZIONE AL 07.90 O A DEC.ORIGIN. O DI RICALCOLO O DI BONUS" il campo MESSAGGIO-ERRORE, diversamente 
        /// valorizzare con "MANCA REGISTRAZIONE AL 02.93 O A DEC.ORIGIN. O DI RICALCOLO O DI BONUS" il campo MESSAGGIO-ERRORE;
        /// </summary>
        /// <param name="decorrenzaContributiItalianiEdEsteri"></param>
        /// <param name="decorrenzaBonus"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaContributiItalianiEdEsteri(DateTime? decorrenzaContributiItalianiEdEsteri, DateTime? decorrenza, DateTime? decorrenzaBonus, short? codiceGestione, DateTime?[] primaDecorrenzaImportiEsteri, DateTime? decorrenzaOriginaria, DateTime? decorrenzaPensioneDiretta, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? appDecorrenza = null;

            if (decorrenzaContributiItalianiEdEsteri.HasValue)
            {
                if (!Utility.DataSuccessivaA(decorrenzaContributiItalianiEdEsteri.Value, decorrenza.Value))
                {
                    if (!(decorrenzaBonus.HasValue && decorrenzaBonus.Equals(decorrenzaContributiItalianiEdEsteri)))
                    {
                        messaggioVideo = "Decorrenza anteriore a Decorrenza Pensione";
                        return false;
                    }
                }

                if (codiceGestione.GetValueOrDefault() / 10 == 7)
                    appDecorrenza = new DateTime(1990, 07, 01);
                else
                    appDecorrenza = new DateTime(1993, 02, 01);
                if (!Utility.DataSuccessivaA(appDecorrenza.Value, decorrenza.Value))
                    appDecorrenza = decorrenza;
                if (decorrenzaBonus.HasValue)
                    appDecorrenza = decorrenzaBonus;

                if (!decorrenzaContributiItalianiEdEsteri.Equals(appDecorrenza))
                {
                    if (codiceGestione.GetValueOrDefault() / 10 == 7)
                    {
                        messaggioVideo = "Manca Registrazione al 07/1990 o a Decorrenza Originaria o di Ricalcolo o di Bonus";
                        return false;
                    }
                    else
                    {
                        messaggioVideo = "Manca Registrazione al 02/1993 o a Decorrenza Originaria o di Ricalcolo o di Bonus";
                        return false;
                    }
                }

                if (codiceGestione.GetValueOrDefault() / 10 != 7)
                {
                    if (!(decorrenzaContributiItalianiEdEsteri.Equals(decorrenzaOriginaria) || decorrenzaContributiItalianiEdEsteri.Equals(decorrenzaPensioneDiretta) || primaDecorrenzaImportiEsteri.Contains(decorrenzaContributiItalianiEdEsteri)))
                    {
                        messaggioVideo = "Decorrenza diversa da Decorrenza Originaria o Decorrenza Ricalcolo";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 231.	Eeseguire la subroutine GEST-EST-61; 
        /// 232.	Calcolare APP-CAL-EST  =  ( SETRI233(1 1)  +  SETRI233(2 1)  +  SETRI233(3 1)  + SETRI233(1 2)  +  SETRI233(2 2)   +  SETRI233(3 2) ); 
        /// 233.	Se FL-P93INP93 è uguale a "S" effettuare le seguenti operazioni :                     
        /// 233.1.	Se APP-CAL-EST  non è uguale a APP-CAL5 effettuare le seguenti operazioni : 
        /// 233.1.1.	Se  (IW1SACDM + IW1STCDM + ICISTCDM335 + ICISTCDM012  + IW1SAART + IW1START + ICISTART335  + ICISTART012  +  IW1SACOM + IW1STCOM + ICISTCOM335 + ICISTCOM012  
        /// +  IW1SAOBG + IW1STOBG + ICISTOBG335 + ICISTOBG012  +  APP-CAL5)  è inferiore a 2080 effettuare le seguenti operazioni :
        /// 233.1.1.1.	Valorizzare con "48" il campo  TIPO-ERRORE; 
        /// 233.1.1.2.	Calcolare RIG-ERRORE = 19  +  INDICE;
        /// 233.1.1.3.	Se INDBIS è uguale a 1 valorizzare con 30 il campo COL-ERR1 diversamente valorizzarlo con 80;
        /// 233.1.1.4.	Valorizzare con "SETTIMANE ESTERE DIVERSE DAL PANN. C      NV09"  il campo MESSAGGIO-ERRORE, con 99 il campo  INDBIS e uscire da SETT-ESTERE (242);                              
        /// </summary>
        /// <param name="decorrenzaContributiItalianiEdEsteri"></param>
        /// <param name="numeroSettimanEstere"></param>
        /// <param name="sommaSettimaneContributiItalianiEdEsteri"></param>
        /// <param name="decorrenza"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="sommaSettimaneContributi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneEstereWithContributiItalianiEdEsteri(DateTime? decorrenzaContributiItalianiEdEsteri, int? numeroSettimanEstere /*GEST-EST-61*/, int? sommaSettimaneContributiItalianiEdEsteri, DateTime? decorrenza, DateTime? dataInizioAssicurazione, int? sommaSettimaneContributi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool flagP93INP93 = FlagP93INP93(decorrenza, dataInizioAssicurazione);

            if (flagP93INP93)
            {
                if (sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() != numeroSettimanEstere.GetValueOrDefault())
                {
                    if (sommaSettimaneContributi.GetValueOrDefault() + numeroSettimanEstere.GetValueOrDefault() < 2080)
                    {
                        messaggioVideo = "Settimane Dati Calcolo incompatibili con Settimane Istituzioni Estere";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 234.	Se W-COD-GE1  è uguale a 6 effettuare le seguenti operazioni : 
        /// 234.1.	Se FL-P93INA93FIP93 è uguale a "S" oppure FL-P93FIA93-INA è uguale a "S" effettuare le seguenti operazioni :
        /// 234.1.1.	Valorizzare con APP-DEC i primi 6 caratteri del campo APP-DATA-2, con 1 il campo APP-GIORNO-2, con 19930101 il campo APP-DATA-1;
        /// 234.1.2.	Eseguire il programma "PCIPL94" passandogli i parametri APP-DATA-1,  APP-DATA-2 e APP-DATA-RC;
        /// 234.1.3.	Valorizzare con SETRI233(INDICE  INDBIS) il campo APP-APP;              
        /// 234.1.4.	Se W-COD-GE2  è uguale a 1 aggiungere IW1STOBG al campo APP-APP;
        /// 234.1.5.	Se W-COD-GE2  è uguale a 2 aggiungere IW1STCDM il campo APP-APP;
        /// 234.1.6.	Se W-COD-GE2  è uguale a 3 aggiungere IW1START il campo APP-APP;
        /// 234.1.7.	Se W-COD-GE2  è uguale a 4 aggiungere IW1STCOM il campo APP-APP;
        /// 234.1.8.	Se APP-APP  è maggiore di APP-DATA-RC e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" effettuare le seguenti operazioni :  
        /// 234.1.9.	valorizzare con "49" il campo  TIPO-ERRORE, con 11 il campo RIG-ERRORE; 
        /// 234.1.9.1.	Se INDBIS è uguale a 1 valorizzare con 30 il campo COL-ERR1 diversamente valorizzarlo con 80;
        /// 234.1.9.2.	Valorizzare  con "SETT. ITALIANE + ESTERE DAL 01.93 SUPERIORI A CAPIENZA PERIODO"  il campo MESSAGGIO-ERRORE, con 99 il campo  INDBIS e uscire da SETT-ESTERE (242); 
        /// </summary>
        /// <param name="codiceGestione"></param>
        /// <param name="decorrenza"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="codNatura"></param>
        /// <param name="decorrenzaContributiItalianiEdEsteri"></param>
        /// <param name="settimaneRetributiveQuotaBCodGestione1"></param>
        /// <param name="settimaneRetributiveQuotaBCodGestione2"></param>
        /// <param name="settimaneRetributiveQuotaBCodGestione3"></param>
        /// <param name="settimaneRetributiveQuotaBCodGestione4"></param>
        /// <param name="settimaneContributiItalianiEdEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCapienzaSettimaneContributiItalianiEdEsteri(short? codiceGestione, DateTime? decorrenza, DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione, string codNatura, DateTime? decorrenzaContributiItalianiEdEsteri, int? settimaneRetributiveQuotaBCodGestione1, int? settimaneRetributiveQuotaBCodGestione2, int? settimaneRetributiveQuotaBCodGestione3, int? settimaneRetributiveQuotaBCodGestione4, int? settimaneContributiItalianiEdEsteri, int categoria, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool flagP93INA93FIP93 = FlagP93INA93FIP93(decorrenza, dataInizioAssicurazione, dataFineAssicurazione);
            bool flagP93FIA93INA = FlagP93FIA93INA(decorrenza, dataFineAssicurazione, codNatura);

            if (codiceGestione.GetValueOrDefault() / 10 == 6)
            {
                if (flagP93INA93FIP93 || flagP93FIA93INA)
                {
                    int nSettimane = Utility.NSettimaneBetweenDate(decorrenzaContributiItalianiEdEsteri.Value, new DateTime(1993, 01, 01));
                    int appSettimane = settimaneContributiItalianiEdEsteri.GetValueOrDefault();

                    switch (codiceGestione.GetValueOrDefault() % 10)
                    {
                        case 1:
                            appSettimane += settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault();
                            break;
                        case 2:
                            appSettimane += settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault();
                            break;
                        case 3:
                            appSettimane += settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault();
                            break;
                        case 4:
                            appSettimane += settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();
                            break;
                    }

                    if (appSettimane > nSettimane && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(2, 1).Equals("G") && !codNatura.Substring(2, 1).Equals("Z"))))
                    {
                        if (!(categoria == 85 || categoria == 86 || categoria == 87 || categoria == 88 || categoria == 89 || categoria == 90 || categoria == 91 || categoria == 92 || categoria == 93) &&
                            !(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (categoria == 4 || categoria == 5 || categoria == 6)))
                        {
                            messaggioVideo = "Settimane Italiane + Estere dal 01/93 superiori a capienza periodo";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 235.	Calcolare APP-CAL5  =  ( SETRI233(1 1)  +  SETRI233(2 1)   +  SETRI233(3 1)   +   SETRI233(1 2) +   SETRI233(2 2)  +  SETRI233(3 2) );
        /// 236.	Valorizzare con zero il campo APP-CAL-EST;
        /// 237.	Se  ART48(1)  non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 237.1.	Se SETT2(1) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(1), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(1);
        /// 238.	Se  ART48(2) non è uguale a "S" effettuare le seguenti operazioni : 
        /// 238.1.	Se SETT2(2) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(2), diversamente valorizzare APP-CAL-EST = APP-CAL-EST + SETT1(2);
        /// 239.	Se  ART48(3) non è uguale a "S" effettuare le seguenti operazioni :                                                              
        /// 239.1.	Se SETT2(3) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(3), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(3);
        /// 240.	Se  ART48(4) non è uguale a "S" effettuare le seguenti operazioni :  
        /// 240.1.	Se SETT2(4) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(4), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(4) ;
        /// **** MODIFICA DEL 14.3.2002 DA LUISA (ERR. 2700-48600006-VOCOMS)  
        /// **** Se APP-CAL5 è maggiore di zero ed è inferiore a APP-CAL-EST                                  
        /// 241.	Se APP-CAL5 è maggiore di zero ed è inferiore a APP-CAL-EST-R effettuare le seguenti operazioni : 
        /// 241.1.	Se (IW1SACDM + IW1STCDM + ICISTCDM335  + ICISTCDM012 + IW1SAART + IW1START + ICISTART335  + ICISTART012 + IW1SACOM + IW1STCOM +  ICISTCOM335  + ICISTCOM012  +  
        /// IW1SAOBG + IW1STOBG + ICISTOBG335  + ICISTOBG012 + APP-CAL5) è inferiore a 2080 valorizzare con "50" il campo  TIPO-ERRORE, con 20 il campo, con 30 il campo COL-ERR1, 
        /// con "SETTIMANE ESTERE DIVERSE DAL PANN. CNV09" il campo MESSAGGIO-ERRORE, con 99 il campo INDBIS;
        /// </summary>
        /// <param name="sommaSettimaneContributiItalianiEdEsteri"></param>
        /// <param name="settimaneEstere"></param>
        /// <param name="sommaSettimaneContributi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneWithContributiItalianiEdEsteri(int? sommaSettimaneContributiItalianiEdEsteri, int? settimaneEstere, int? sommaSettimaneContributi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() > 0 && sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() < settimaneEstere)
            {
                if (sommaSettimaneContributi.GetValueOrDefault() + sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() < 2080)
                {
                    messaggioVideo = "Settimane Dati Calcolo incompatibili con Settimane Istituzioni Estere";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 178.	Se FINASS è maggiore di 19930100 effettuare le seguenti operazioni :   
        /// 178.1.	Se INIASS è maggiore di 19930100 valorizzare con INIASS il campo APP-DATA-1, diversamente valorizzare con 19930101 il campo APP-DATA-1;
        /// 178.2.	Se FINASS è maggiore di 19960100 e (ICISTCDM335 è maggiore di zero oppure ICISTART335 è maggiore di zero oppure ICISTCOM335 è maggiore di zero oppure ICISTOBG335 
        /// è maggiore di zero ) valorizzare con 19951231 il campo APP-DATA-2, diversamente valorizzare con W-DEORIG i primi 6 caratteri del campo APP-DATA-2 e con 1 il campo 
        /// APP-GIORNO-2;
        /// 178.3.	Eseguire il programma "PCIPL94" passandogli i parametri APP-DATA-1 APP-DATA-2 e APP-DATA-RC;
        /// ********CDM 62                                                    
        /// 178.4.	Valorizzare con IW1STCDM il campo APP-APP;
        /// 178.5.	Se INIASS è inferiore a 19930101 e APP-APP è maggiore di zero effettuare le seguenti operazioni : 
        /// 178.5.1.	Se GEST233(1   1)  è uguale a 62 aggiungere SETRI233(1   1)  al campo     APP-APP; 
        /// 178.5.2.	Se GEST233(1   2)   è uguale a 62  aggiungere SETRI233(1   2) al campo     APP-APP;
        /// 178.5.3.	Se  GEST233(2   1)  è uguale a 62  aggiungere SETRI233(2   1) al campo     APP-APP;
        /// 178.5.4.	Se  GEST233(2   2)  è uguale a 62  aggiungere SETRI233(2   2)  al campo   APP-APP;
        /// 178.5.5.	Se  GEST233(3   1)  è uguale a 62 aggiungere SETRI233(3   1)   al campo   APP-APP;               
        /// 178.5.6.	Se  GEST233(3   2)  è uguale a 62 aggiungere SETRI233(3   2)   al campo   APP-APP;
        /// 178.6.	Se APP-APP è maggiore di APP-DATA-RC  e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" e IW1DEORIG è inferiore a 201202 valorizzare con "67" il campo 
        /// TIPO-ERRORE, con 7 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETT. CD-CM DAL 1993 + ESTERO SUPERIORI A CAPIENZA NEL PERIODO"  il campo MESSAGGIO-ERRORE, con 1 
        /// il campo FLAG-ERR e uscire da  CONTROLLI-1 (195); 
        /// ********ART 63                                                    
        /// 178.7.	Valorizzare con IW1START il campo APP-APP; 
        /// 178.8.	Se INIASS è inferiore a 19930101 e APP-APP è maggiore di zero effettuare le seguenti operazioni :                                                         
        /// 178.8.1.	Se GEST233(1   1) è uguale a 63 aggiungere SETRI233(1   1)   al campo     APP-APP;
        /// 178.8.2.	Se  GEST233(1   2) è uguale a 63 aggiungere SETRI233(1   2)  al campo         APP-APP;                
        /// 178.8.3.	Se  GEST233(2   1) è uguale a 63 aggiungere SETRI233(2   1)  al campo      APP-APP;
        /// 178.8.4.	Se  GEST233(2   2) è uguale a 63 aggiungere SETRI233(2   2) al campo     APP-APP;                 
        /// 178.8.5.	Se  GEST233(3   1) è uguale a 63 aggiungere SETRI233(3   1) al campo     APP-APP;                
        /// 178.8.6.	Se  GEST233(3   2) è uguale a 63 aggiungere SETRI233(3   2) al campo      APP-APP;
        /// 178.9.	Se APP-APP è maggiore di APP-DATA-RC  e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" e IW1DEORIG è inferiore a 201202 valorizzare con "67" il campo 
        /// TIPO-ERRORE, con 8 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETT. ART DAL 1993 + ESTERO SUPERIORI A CAPIENZA NEL PERIODO"  il campo         MESSAGGIO-ERRORE, 
        /// con 1 il campo FLAG-ERR e uscire da  CONTROLLI-1 (195);       
        /// ********COM 64                                                    
        /// 178.10.	Valorizzare con IW1STCOM il campo APP-APP;                                      
        /// 178.11.	Se INIASS è inferiore a 19930101 e APP-APP è maggiore di zero effettuare le seguenti operazioni :                                                         
        /// 178.11.1.	Se  GEST233(1   1)  è uguale a 64 aggiungere SETRI233(1   1)   al campo  APP-APP;                
        /// 178.11.2.	Se  GEST233(1   2)  è uguale a 64 aggiungere SETRI233(1   2)   al campo  APP-APP;                
        /// 178.11.3.	Se  GEST233(2   1)  è uguale a 64 aggiungere SETRI233(2   1)   al campo  APP-APP;                             
        /// 178.11.4.	Se  GEST233(2   2)  è uguale a 64 aggiungere SETRI233(2   2)   al campo   APP-APP;                             
        /// 178.11.5.	Se  GEST233(3   1)  è uguale a 64 aggiungere SETRI233(3   1)   al campo  APP-APP;                
        /// 178.11.6.	Se  GEST233(3   2)  è uguale a 64 aggiungere SETRI233(3   2)   al campo   APP-APP; 
        /// 178.12.	Se APP-APP è maggiore di APP-DATA-RC  e IABCONA4 non è uguale a "G" e IABCONA4 non è uguale a "Z" e IW1DEORIG è inferiore a 201202 valorizzare con "67" il campo 
        /// TIPO-ERRORE, con 9 il campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETT. COM DAL 1993 + ESTERO SUPERIORI A CAPIENZA NEL PERIODO"  il campo MESSAGGIO-ERRORE, con 1 il 
        /// campo FLAG-ERR e uscire da  CONTROLLI-1 (195);  
        /// </summary>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="settimaneContributiveQuotaCCodGestione1"></param>
        /// <param name="settimaneContributiveQuotaCCodGestione2"></param>
        /// <param name="settimaneContributiveQuotaCCodGestione3"></param>
        /// <param name="settimaneContributiveQuotaCCodGestione4"></param>
        /// <param name="decorrenza"></param>
        /// <param name="settimaneToCompare"></param>
        /// <param name="codNatura"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="msg"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneNelPeriodo9395(DateTime? dataFineAssicurazione, DateTime? dataInizioAssicurazione, int? settimaneContributiveQuotaCCodGestione1, int? settimaneContributiveQuotaCCodGestione2, int? settimaneContributiveQuotaCCodGestione3, int? settimaneContributiveQuotaCCodGestione4, DateTime? decorrenza, int? settimaneToCompare, string codNatura, DateTime? decorrenzaOriginaria, string msg, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? appData1 = null;
            DateTime? appData2 = null;

            if (dataFineAssicurazione.HasValue && Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
            {
                if (dataInizioAssicurazione.HasValue && Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                    appData1 = dataInizioAssicurazione;
                else
                    appData1 = new DateTime(1993, 01, 01);

                if (Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1996, 01, 01)) && (settimaneContributiveQuotaCCodGestione1.GetValueOrDefault() > 0 || settimaneContributiveQuotaCCodGestione2.GetValueOrDefault() > 0 || settimaneContributiveQuotaCCodGestione3.GetValueOrDefault() > 0 || settimaneContributiveQuotaCCodGestione4.GetValueOrDefault() > 0))
                    appData2 = new DateTime(1995, 12, 31);
                else
                    appData2 = new DateTime(decorrenza.Value.Year, decorrenza.Value.Month, 01);

                int nSettimane = Utility.NSettimaneBetweenDate(appData2.Value, appData1.Value);
                if (nSettimane < 0)
                    nSettimane = 0;

                if (settimaneToCompare.GetValueOrDefault() > nSettimane && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(2, 1).Equals("G") && !codNatura.Substring(2, 1).Equals("Z"))) && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2012, 02, 01)))
                {
                    messaggioVideo = "Settimane " + msg + " dal 1993 + Estero superiori a capienza nel periodo";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 184.	Valorizzare con zero il campo APP-APP;
        /// 185.	Se GEST233(1  1) è maggiore di zero aggiungere SETRI233(1   1)  a  APP-APP;
        /// 186.	Se GEST233(1  2) è maggiore di zero aggiungere SETRI233(1   2)  a  APP-APP;
        /// 187.	Se GEST233(2  1) è maggiore di zero aggiungere SETRI233(2   1)  a  APP-APP;
        /// 188.	Se GEST233(2  2) è maggiore di zero aggiungere SETRI233(2   2)  a  APP-APP;
        /// 189.	Se GEST233(3  1) è maggiore di zero aggiungere SETRI233(3   1)  a  APP-APP;
        /// 190.	Se GEST233(3  2) è maggiore di zero aggiungere SETRI233(3   2)  a  APP-APP;
        /// 191.	Se APP-APP è maggiore di zero e  APP-CAL-EST-R non è uguale a APP-APP effettuare le seguenti operazioni :  
        /// 191.1.	Se SET-RICAL  è uguale a "S"  e  (    (GEST233(1 1) è uguale a GEST233(1 2) oppure GEST233(2 1) oppure GEST233(2 2) oppure GEST233(3 1) oppure GEST233(3 2)) 
        /// oppure (GEST233(1 2) è uguale a GEST233(2 1) oppure GEST233(2 2) oppure GEST233(3 1)  oppure GEST233(3 2)) oppure  (GEST233(2 1) è uguale a GEST233(2 2) oppure 
        /// GEST233(3  1)  oppure GEST233(3 2))  oppure  (GEST233(2 2) è uguale a GEST233(3 1) oppure GEST233(3  2)) oppure  (GEST233(3 1) è uguale a GEST233(3 2))) continuare 
        /// l’elaborazione al punto successivo (192), diversamente effettuare le seguenti operazioni :                                                    
        /// 191.1.1.	Se APP-CAL-EST-R è maggiore di zero effettuare le seguenti operazioni :
        /// 191.1.1.1.	Se (IW1DEORIG è inferiore a 199007  e DEC233(1 1) è uguale a 199007 e SI2080 è uguale a "S" )  continuare l’elaborazione al punto successivo (192), 
        /// diversamente effettuare le seguenti operazioni :                                                 
        /// **********  ROSALIA 21.3.2007 PER PRATICA DI CHIETI               
        /// **********  CTR GIUSTI: QUELLI PER MISURA MAGGIORI DI DIRITTO     
        /// 191.1.1.1.1.	Se TP1COFI  = "SRGRSR46S48E243E" continuare l’elaborazione al punto successivo (190), diversamente valorizzare con "93" il campo  TIPO-ERRORE, con 20 il 
        /// campo RIG-ERRORE, con 08 il campo COL-ERR1, con "SETT.EST. 233/90 - 503/92 - 335/95 INCOMPATIBILI CON SETTIMANE CNV09" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR 
        /// e uscire da  CONTROLLI-1 (195);
        /// 191.1.2.	Diversamente da quanto analizzato nel punto precedente (191.1.1) valorizzare con "93" il campo TIPO-ERRORE, con 20 il campo RIG-ERRORE, con 08 il campo 
        /// COL-ERR1, con "SETT.ESTERE 233/90 - 503/92 - 335/95 N      ON VANNO ACQUISITE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="sommaSettimaneContributiItalianiEdEsteri"></param>
        /// <param name="settimaneRicalcoloMisura"></param>
        /// <param name="set_Rical"></param>
        /// <param name="isDecorrenzaDuplicata"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="primaDecorrenzaContributiItalianiEdEsteri"></param>
        /// <param name="isSommaSettimaneUgualeA2080"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiItalianiEdEsteriWithSettimaneProRata(int? sommaSettimaneContributiItalianiEdEsteri, int? settimaneRicalcoloMisura, bool set_Rical, bool isDecorrenzaContributiItalianiEdEsteriDuplicata, DateTime? decorrenzaOriginaria, DateTime? primaDecorrenzaContributiItalianiEdEsteri, bool isSommaSettimaneUgualeA2080, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() > 0 && settimaneRicalcoloMisura.GetValueOrDefault() != sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault())
            {
                if (!(set_Rical && isDecorrenzaContributiItalianiEdEsteriDuplicata))
                {
                    if (settimaneRicalcoloMisura.GetValueOrDefault() > 0)
                    {
                        if (!(!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1990, 07, 01)) && primaDecorrenzaContributiItalianiEdEsteri.Equals(new DateTime(1990, 07, 01)) && isSommaSettimaneUgualeA2080))
                        {
                            messaggioVideo = "Sett.Est. 233/90 - 503/92 - 335/95 incompatibili con Settimane Istituzione Estera";
                            return false;
                        }
                    }
                    else
                    {
                        messaggioVideo = "Sett.Estere 233/90 - 503/92 - 335/95 non vanno acquisite";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 94.	Se IW1RETOBG è maggiore di zero effettuare le seguenti operazioni :  
        /// 94.1.	Se FL-NO233 è uguale a "S" oppure (FL-P93FIA93-NOINA è uguale a "S" e FL-P93FIA93-INA è uguale a " ")  effettuare le seguenti operazioni :  
        /// 94.1.1.	Se DAFELPE-DATA è maggiore di '20110101'  continuare l’elaborazione al punto successivo (95) diversamente valorizzare con "25" il campo  TIPO-ERRORE, con 10 il 
        /// campo RIG-ERRORE, con 56 il campo COL-ERR1, con "R.M.P. DAL 1993 INCOMPATIBILE CON DECORRENZA E/O ULTIMO CONTRIB." il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e 
        /// uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="rmsQuotaBCodGestione1"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="categoria"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="dataCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaRMSQuotaBWithDecorrenzaAndUltimoContributo(decimal? rmsQuotaBCodGestione1, DateTime? decorrenza, DateTime? decorrenzaDiretta, int categoria, DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione, DateTime? dataCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            // Codice controllo CI: CI_0002
            //bool flagNo233 = FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte);
            //bool flagP93FIA93NOINA = FlagP93FIA93NOINA(decorrenza, fineAssicurazione, naturaPensione);
            //bool flagP93FIA93INA = FlagP93FIA93INA(decorrenza, fineAssicurazione, naturaPensione);

            //if (rmsQuotaBCodGestione1.GetValueOrDefault() > 0)
            //{
            //    if (flagNo233 || flagP93FIA93NOINA || !flagP93FIA93INA)
            //    {
            //        if (!(dataCalcolo.HasValue && Utility.DataStrettamenteSuccessivaA(dataCalcolo.Value, new DateTime(2011, 01, 31))))
            //        {
            //            messaggioVideo = "R.M.S. dal 1993 incompatibile con Decorrenza e/o Ultimo Contributo";
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        /// <summary>
        /// 95.	Se  IW1STOBG è maggiore a zero effettuare le seguenti operazioni :  
        /// 96.2.	Se FL-NO233 è uguale a "S" oppure (FL-P93FIA93-INA è uguale a "S" oppure FL-FIA93 è uguale a "S") effettuare le seguenti operazioni :                 
        /// 96.2.1.	Se DAFELPE-DATA è maggiore di '20110101'  continuare l’elaborazione al punto successivo (94.3) diversamente valorizzare con "27" il campo TIPO-ERRORE, con 10 il 
        /// campo RIG-ERRORE, con 76 il campo COL-ERR1, con "SETT. DAL 1993 INCOMPATIBILI CON DECORRENZA E/O ULTIMO CONTRIB." il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR e 
        /// uscire da CONTROLLI-1 (195);
        /// </summary>
        /// <param name="rmsQuotaBCodGestione1"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="categoria"></param>
        /// <param name="dataMorte"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <param name="dataCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneQuotaBWithDecorrenzaAndUltimoContributo(int? settimaneQuotaBCodGestione1, DateTime? decorrenza, DateTime? decorrenzaDiretta, int categoria, DateTime? dataMorte, DateTime? fineAssicurazione, string naturaPensione, DateTime? dataCalcolo, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Settimane_Dal_93_Incompatibili.SETTIMANE_DAL_93_INCOMPATIBILI))
                return true;

            bool flagNo233 = FlagNo233(decorrenza, decorrenzaDiretta, categoria, dataMorte);
            bool flagP93FIA93NOINA = FlagP93FIA93NOINA(decorrenza, fineAssicurazione, naturaPensione);
            bool flagFIA93 = FlagFIA93(fineAssicurazione);

            if (settimaneQuotaBCodGestione1.GetValueOrDefault() > 0)
            {
                if (flagNo233 || flagP93FIA93NOINA || flagFIA93)
                {
                    if (!(dataCalcolo.HasValue && Utility.DataStrettamenteSuccessivaA(dataCalcolo.Value, new DateTime(2011, 01, 31))))
                    {
                        messaggioVideo = "Settimane dal 1993 incompatibile con Decorrenza e/o Ultimo Contributo";
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion PCIPL39

        #region Maternità/Acna
        #region PCIPL70
        /// <summary>
        /// 10. Se ISETMAT1 è maggiore di 0 (zero) oppure ISETMAT2 è maggiore di 0 (zero) oppure ISETMAT3 è maggiore di 0 (zero), effettuare i seguenti controlli:
        /// * Se APP-SESS non è uguale a “F”, impostare TIPO-ERRORE con “01”, RIG-ERRORE con 06, COL-ERR1 con 28, MESSAGGIO-ERRORE con “MATERNITA' DL 151 DEL 24/3/01 INCOMPATIBILE CON 
        /// SESSO TITOLARE” e continuare l’elaborazione al punto successivo (24);
        /// * Se W-DEORIG è maggiore di 200104, impostare TIPO-ERRORE con “02”, RIG-ERRORE con 06, COL-ERR1 con 28, MESSAGGIO-ERRORE con “MATERNITA' DL 151 DEL 24/3/01 INCOMPATIBILE 
        /// CON DECORRENZA” e continuare l’elaborazione al punto successivo (24);
        /// 11. Diversamente da quanto analizzato al punto precedente (10), impostare TIPO-ERRORE con “04”, RIG-ERRORE con 06, COL-ERR1 con 28, MESSAGGIO-ERRORE con “MATERNITA' DL 
        /// 151 DEL 24/3/01 SETTIMANE NON ACQUISITE” e continuare l’elaborazione al punto successivo (24);
        /// 12.	Se W-DEORIG è minore di 197608 effettuare  seguenti controlli:
        /// 12.1.	Se ISETMAT1 è uguale a 0 (zero), impostare TIPO-ERRORE con “06”, RIG-ERRORE con 06, COL-ERR1 con 28, MESSAGGIO-ERRORE con “SETTIMANE AL 31/12/92 MANCANTI” e 
        /// continuare l’elaborazione al punto successivo (24);
        /// 12.2.	Se IIVSTMAT1 è uguale a 0 (zero), impostare TIPO-ERRORE con “07”, RIG-ERRORE con 08, COL-ERR1 con 28, MESSAGGIO-ERRORE con “IMPORTO IVS MATERNITA’ MANCANTE” e 
        /// continuare l’elaborazione al punto successivo (24);
        /// 13.	Diversamente da quanto indicato al punto precedente (12) effettuare i seguenti controlli:
        /// 13.1.	Se IIVSTMAT1 è maggiore di 0 (zero), impostare TIPO-ERRORE con “08”, RIG-ERRORE con 08, COL-ERR1 con 28, MESSAGGIO-ERRORE con “IMPORTO IVS MATERNITA' INCOMPATIBILE 
        /// CON DECORRENZA ORIGINARIA” e continuare l’elaborazione al punto successivo (24);
        /// </summary>
        /// <param name="settimaneAl1292"></param>
        /// <param name="settimaneDL50392"></param>
        /// <param name="sesso"></param>
        /// <param name="decorrenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsMaternita(int? settimaneAl1292, int? settimaneDL50392, char? sesso, DateTime? decorrenza, decimal? importoIVS, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0 || settimaneDL50392.GetValueOrDefault() > 0)
            {
                if (sesso != 'F')
                {
                    messaggioVideo = "Maternita' DL 151 del 26/3/01 incompatibile con Sesso Titolare";
                    return false;
                }

                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(2001, 04, 30)))
                {
                    messaggioVideo = "Maternita' DL 151 del 26/3/01 incompatibile con Decorrenza";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Richiesta Maternita' DL 151 del 26/3/01: settimane non acquisite";
                return false;
            }

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1976, 08, 01)))
            {
                if (settimaneAl1292.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane al 31/12/92 mancanti";
                    return false;
                }

                if (importoIVS.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Importo IVS Maternita' mancante";
                    return false;
                }
            }
            else
            {
                if (importoIVS.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Importo IVS Maternita' incompatibile con Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 10. Se ISETMAT1 è maggiore di 0 (zero) oppure ISETMAT2 è maggiore di 0 (zero) oppure ISETMAT3 è maggiore di 0 (zero), effettuare i seguenti controlli:
        /// 10.3.	Se IW1CATPEN è minore di 7 e la sommatoria dei campi (IW1NSOBG + IW1STOBG +  ICISTOBG335 + IW1VVMISURA + ICI1VVOBG) è minore di 240 
        /// Oppure 
        /// Se IW1CATPEN è maggiore di 6 e la sommatoria dei campi (IW1NSOBG + IW1STOBG +  ICISTOBG335 + IW1VVMISURA + ICI1VVOBG) è minore di 240
        /// effettuare i seguenti controlli:
        /// 10.3.1.	Se IW3DESUP(1) è uguale a 0 (zero) 
        /// 10.3.1.1.	impostare TIPO-ERRORE con “03”, RIG-ERRORE con 06, COL-ERR1 con 28, MESSAGGIO-ERRORE con “MATERNITA' DL 151 DEL 24/3/01 INCOMPATIBILE CON TOT.SETTIMANE 
        /// CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// </summary>
        /// <param name="settimaneAl1292"></param>
        /// <param name="settimaneDL50392"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="settimaneContributiveQuotaC"></param>
        /// <param name="vvMisuraAl1292"></param>
        /// <param name="vvMisuraDL50392"></param>
        /// <param name="decorrenzaSupplemento"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaMaternitaWithSettimane(int? settimaneAl1292, int? settimaneDL50392, int? settimaneRetributiveQuotaA, int? settimaneRetributiveQuotaB, int? settimaneContributiveQuotaC, int? vvMisuraAl1292, int? vvMisuraDL50392, DateTime? decorrenzaSupplemento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0 || settimaneDL50392.GetValueOrDefault() > 0)
            {
                if ((settimaneRetributiveQuotaA.GetValueOrDefault() + settimaneRetributiveQuotaB.GetValueOrDefault() + settimaneContributiveQuotaC.GetValueOrDefault() + vvMisuraAl1292.GetValueOrDefault() + vvMisuraDL50392.GetValueOrDefault()) < 260)
                {
                    if (!decorrenzaSupplemento.HasValue)
                    {
                        messaggioVideo = "Maternita' DL 151 del 26/3/01 incompatibile con Tot.Settimane";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 14.	Se ISEMAT1 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 14.1.	Se IW1NSOBG è uguale a 0 (zero) e IABREMSVV è uguale a 0 (zero) e IW1SAOBG è uguale a 0 (zero), impostare TIPO-ERRORE con “09”, RIG-ERRORE con 06, COL-ERR1 con 
        /// 28, MESSAGGIO-ERRORE con “SETTIMANE AL 12/92 INCOMPATIBILI CON SETTIM. DL 503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// 15.	Se ISETMAT2 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 15.1.	Se IW1STOBG è uguale a 0 (zero) oppure IW1RETOBG è uguale a 0 (zero), impostare TIPO-ERRORE con “10”, RIG-ERRORE con 06, COL-ERR1 con 68, MESSAGGIO-ERRORE con 
        /// “SETTIMANE D.L.503/92 INCOMPATIBILI CON SETTIM. DL 503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// </summary>
        /// <param name="settimane"></param>
        /// <param name="settimaneRetributive"></param>
        /// <param name="rms"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaMaternitaWithDatiCalcolo(int? settimaneAl1292, int? settimaneRetributiveQuotaA, decimal? rmsQuotaA, int? settimaneDL50392, int? settimaneRetributiveQuotaB, decimal? rmsQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0)
            {
                if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0 && rmsQuotaA.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane al 12/92 incompatibili con Settimane Retributive al 12/92";
                    return false;
                }
            }

            if (settimaneDL50392.GetValueOrDefault() > 0)
            {
                if (settimaneRetributiveQuotaB.GetValueOrDefault() == 0 || rmsQuotaB.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane D.L.503/92 incompatibili con Settimane Retributive DL 503/92";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 18.	Se ISETCEN1 è maggiore di 0 (zero) oppure ISETCEN2 è maggiore di 0 (zero) oppure ISETCEN3 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 18.1.	Se W-DEORIG è maggiore di 200401, impostare TIPO-ERRORE con “02”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con “CONTRIBUTI EX ACNA CENGIO 
        /// INCOMPATIBILE CON DECORRENZA” e continuare l’elaborazione al punto successivo (24);
        /// 18.2.	Se IW1DIRET è maggiore di 0 (zero) e IW1DMOR è minore di 20040000, impostare TIPO-ERRORE con “02”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con 
        /// “CONTRIBUTI EX ACNA INCOMPATIBILI CON DATA MORTE DANTE CAUSA (CNV14)” e continuare l’elaborazione al punto successivo (24);
        /// 18.4.	Diversamente da quanto analizzato al punto precedente (18.3), impostare TIPO-ERRORE con “02”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con 
        /// “CONTRIBUTI EX ACNA INCOMPATIBILI CON ATT.ECONOMICA/ PROF-IND. (CNV01)” e continuare l’elaborazione al punto successivo (24)
        /// 19.	Diversamente da quanto analizzato al punto precedente (18), impostare TIPO-ERRORE con “04”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con “RICHIESTA 
        /// CONTRIBUTI EX ACNA: SETTIMANE NON ACQUISITE” e continuare l’elaborazione al punto successivo (24);
        /// 20.	Se W-DEORIG è minore di  197608 effettuare i seguenti controlli:
        /// 20.1.	Se ISETCEN1 è uguale a 0 (zero), impostare TIPO-ERRORE con “06”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con “SETTIMANE AL 31/12/92 MANCANTI” e 
        /// continuare l’elaborazione al punto successivo (24);
        /// 20.2.	Se IIVSCEN1 è uguale a 0 (zero), impostare TIPO-ERRORE con “07”, RIG-ERRORE con 15, COL-ERR1 con 28, MESSAGGIO-ERRORE con “IMPORTO IVS EX ACNA MANCANTE” e 
        /// continuare l’elaborazione al punto successivo (24);
        /// 21.	Diversamente da quanto analizzato al punto precedente (20), se IIVSCEN1 è maggiore di 0 (zero), impostare TIPO-ERRORE con “08”, RIG-ERRORE con 15, COL-ERR1 con 28, 
        /// MESSAGGIO-ERRORE con “IMPORTO IVS EX ACNA INCOMPATIBILE CON DECORRENZA ORIGINARIA” e continuare l’elaborazione al punto successivo (24);
        /// </summary>
        /// <param name="settimaneAl1292"></param>
        /// <param name="settimaneDL50392"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaPensioneDiretta"></param>
        /// <param name="dataMorteDC"></param>
        /// <param name="attivitaEconomica"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="importoIVS"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsAcna(int? settimaneAl1292, int? settimaneDL50392, DateTime? decorrenza, DateTime? decorrenzaPensioneDiretta, DateTime? dataMorteDC, decimal? importoIVS, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0 || settimaneDL50392.GetValueOrDefault() > 0)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(2004, 01, 31)))
                {
                    messaggioVideo = "Contributi Ex Acna Cengio incompatibili con Decorrenza";
                    return false;
                }

                if (decorrenzaPensioneDiretta.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(2004, 01, 01)))
                {
                    messaggioVideo = "Contributi Ex Acna incompatibili con Data Morte Dante Causa";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Richiesta Contributi Ex Acna: Settimane non acquisite";
                return false;
            }

            if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1976, 08, 01)))
            {
                if (settimaneAl1292.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane al 31/12/92 mancanti";
                    return false;
                }

                if (importoIVS.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Importo IVS Ex Acna mancante";
                    return false;
                }
            }
            else
            {
                if (importoIVS.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Importo IVS Ex Acna incompatibile con Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 18.	Se ISETCEN1 è maggiore di 0 (zero) oppure ISETCEN2 è maggiore di 0 (zero) oppure ISETCEN3 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 18.3.	Se TP1ATEC è uguale a 4 e TP1PRIN = 350, continuare l’elaborazione al punto successivo (20);
        /// </summary>
        /// <param name="settimaneAl1292"></param>
        /// <param name="settimaneDL50392"></param>
        /// <param name="attivitaEconomica"></param>
        /// <param name="professioneIndividuale"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaAcnaWithDatiAssicurativi(int? settimaneAl1292, int? settimaneDL50392, int? attivitaEconomica, int? professioneIndividuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0 || settimaneDL50392.GetValueOrDefault() > 0)
            {
                if (attivitaEconomica.GetValueOrDefault() != 4 || professioneIndividuale.GetValueOrDefault() != 350)
                {
                    messaggioVideo = "Contributi Ex Acna incompatibili con Attività Economica/Professione Individuale";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 22.	Se ISETCEN1 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 22.1.	Se IW1NSOBG è uguale a 0 (zero) e IABREMSVV è uguale a 0 (zero) e IW1SAOBG è uguale a 0 (zero), impostare TIPO-ERRORE con “09”, RIG-ERRORE con 13, COL-ERR1 con 
        /// 28, MESSAGGIO-ERRORE con “SETTIMANE AL 12/92 INCOMPATIBILI CON SETTIM. DL 503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// 22.2.	Se IW1CATPEN è minore/uguale 6 effettuare i seguenti controlli:
        /// 22.2.1.	Se (ISETCEN1 * 2 ) è maggiore di IW1NSOBG, impostare TIPO-ERRORE con “09”, RIG-ERRORE con 13, COL-ERR1 con 28, MESSAGGIO-ERRORE con “SETTIMANE AL 12/92 SUPERIORI 
        /// AD 1/2 SETTIM. AL 12/92 PANN.CNV02” e continuare l’elaborazione al punto successivo (24);
        /// 22.3.	Diversamente da quanto analizzato al precedente punto (22.2) se (ISETCEN1 * 2 ) è maggiore di IW1SAOBG, impostare TIPO-ERRORE con “09”, RIG-ERRORE con 13, 
        /// COL-ERR1 con 28, MESSAGGIO-ERRORE con “SETTIMANE AL 12/92 SUPERIORI AD 1/2 SETTIM. AL 12/92  PANN.CNV03” e continuare l’elaborazione al punto successivo (24);
        /// 23.	Se ISETCEN2 è maggiore di 0 (zero) effettuare i seguenti controlli:
        /// 23.1.	Se IW1STOBG è uguale a 0 (zero) oppure IW1RETOBG è uguale a 0 (zero), impostare TIPO-ERRORE con “10”, RIG-ERRORE con 13, COL-ERR1 con 68, MESSAGGIO-ERRORE con 
        /// “SETTIMANE D.L.503/92 INCOMPATIBILI CON SETTIM. DL 503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24); 
        /// 23.2.	Se IW1CATPEN è minore/uguale a 6 effettuare i seguenti controlli:
        /// 23.2.1.	Se (ISETCEN2 * 2 ) è maggiore di IW1STOBG, impostare TIPO-ERRORE con “09”, RIG-ERRORE con 13, COL-ERR1 con 68, MESSAGGIO-ERRORE con “SETTIMANE D.L.503/92 
        /// SUPERIORI AD 1/2 SETTIM. D.L.503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// 23.3.	Diversamente da quanto analizzato al precedente punto (23.2) se (ISETCEN1 * 2 ) è maggiore di IW1STOBG, impostare TIPO-ERRORE con “09”, RIG-ERRORE con 13, 
        /// COL-ERR1 con 68, MESSAGGIO-ERRORE con “SETTIMANE D.L.503/92 SUPERIORI AD 1/2 SETTIM. D.L.503/92 PANN.CNV02/03” e continuare l’elaborazione al punto successivo (24);
        /// </summary>
        /// <param name="settimaneAl1292"></param>
        /// <param name="settimaneDL50392"></param>
        /// <param name="settimaneRetributiveQuotaA"></param>
        /// <param name="rmsQuotaA"></param>
        /// <param name="settimaneRetributiveQuotaB"></param>
        /// <param name="rmsQuotaB"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaAcnaWithDatiCalcolo(int? settimaneAl1292, int? settimaneDL50392, int? settimaneRetributiveQuotaA, decimal? rmsQuotaA, int? settimaneRetributiveQuotaB, decimal? rmsQuotaB, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (settimaneAl1292.GetValueOrDefault() > 0)
            {
                if (settimaneRetributiveQuotaA.GetValueOrDefault() == 0 && rmsQuotaA.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane al 12/92 incompatibili con Settimane Retributive al 12/92";
                    return false;
                }

                if (settimaneAl1292.GetValueOrDefault() * 2 > settimaneRetributiveQuotaA.GetValueOrDefault())
                {
                    messaggioVideo = "Settimane al 12/92 superiori ad 1/2 Settimane al 12/92";
                    return false;
                }
            }

            if (settimaneDL50392.GetValueOrDefault() > 0)
            {
                if (settimaneRetributiveQuotaB.GetValueOrDefault() == 0 || rmsQuotaB.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Settimane D.L.503/92 incompatibili con Settimane Retributive DL 503/92";
                    return false;
                }

                if (settimaneDL50392.GetValueOrDefault() * 2 > settimaneRetributiveQuotaB.GetValueOrDefault())
                {
                    messaggioVideo = "Settimane D.L.503/92 superiori ad 1/2 Settimane D.L.503/92";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL70
        #endregion Maternità/Acna

        #region ProRata
        #region PCIPL12
        /// <summary>
        /// 17.	Se  INDICE è uguale a 1 effettuare le seguenti operazioni  :                                         
        /// 17.1.	Se IDAPLIQ(IND-STA) maggiore di zero effettuare le seguenti operazioni :
        ///          17.1.2.	Se IW1CARIC è uguale a 2 o 5 o 9  effettuare le seguenti operazioni :                                  
        /// 17.1.2.1.	continuare l’elaborazione al punto successivo  (17.1.4);                                    
        /// 17.1.3.	Diversamente da quanto analizzato nel punto precedente (14) effettuare i seguenti controlli:
        /// 17.1.3.1.	Valorizzare con "02" il campo TIPO-ERRORE, con "DATA PRECEDENTE LIQUIDAZIONE ERRATA: INCOMPATIBILE CON TIPO CARICO"  il campo  MESSAGGIO-ERRORE, con 1 il 
        /// campo        FLAG-ERR, Uscire  da CONTROLLI-1, uscire  da CONTROLLI-1 (33);
        /// </summary>
        /// <param name="decorrenzaLiquidazioneStatoEE"></param>
        /// <param name="causaCarico"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataPrecedenteLiquidazioneWithCausaCarico(DateTime? decorrenzaLiquidazioneStatoEE, byte? causaCarico, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaLiquidazioneStatoEE.HasValue)
            {
                if (!(causaCarico.GetValueOrDefault() == 2 || causaCarico.GetValueOrDefault() == 5 || causaCarico.GetValueOrDefault() == 9))
                {
                    messaggioVideo = "Data Precedente Liquidazione errata: incompatibile con tipo carico";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.	Se  INDICE è uguale a 1 effettuare le seguenti operazioni  :                                         
        /// 17.1.	Se IDAPLIQ(IND-STA) maggiore di zero effettuare le seguenti operazioni :
        /// 17.1.4.	Se IDAPLIQ(IND-STA) è maggiore del 199106  effettuare le seguenti operazioni:                                          
        /// 17.1.4.1.	Valorizzare con "03"  il campo  TIPO-ERRORE, con "DATA PRECEDENTE LIQUIDAZIONE ERRATA: MAGGIORE DI 06/91" il campo MESSAGGIO-ERRORE; con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);                       
        /// </summary>
        /// <param name="decorrenzaLiquidazioneStatoEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataPrecedenteLiquidazione(DateTime? decorrenzaLiquidazioneStatoEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaLiquidazioneStatoEE.HasValue)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaLiquidazioneStatoEE.Value, new DateTime(1991, 06, 30)))
                {
                    messaggioVideo = "Data Precedente Liquidazione errata: maggiore di 06/1991";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.	Se  INDICE è uguale a 1 effettuare le seguenti operazioni  :                                         
        /// 17.1.	Se IDAPLIQ(IND-STA) maggiore di zero effettuare le seguenti operazioni :
        /// 17.1.5.	Se IDAPLIQ(IND-STA) è inferiore a DEC (IND-STA  1) effettuare le seguenti operazioni:
        /// 17.1.5.1.	Valorizzare con "04"  il campo  TIPO-ERRORE, con "DATA PRECEDENTE LIQUIDAZIONE ERRATA: ANTERIORE A DECORRENZA"  il campo MESSAGGIO-ERRORE,  con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                       
        /// </summary>
        /// <param name="decorrenzaLiquidazioneStatoEE"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataPrecedenteLiquidazioneWithDecImportiEsteri(DateTime? decorrenzaLiquidazioneStatoEE, DateTime? primaDecorrenzaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaLiquidazioneStatoEE.HasValue)
            {
                if (primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(decorrenzaLiquidazioneStatoEE.Value, primaDecorrenzaImportiEsteri.Value))
                {
                    messaggioVideo = "Data Precedente Liquidazione errata: anteriore a Decorrenza";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.	Se  INDICE è uguale a 1 effettuare le seguenti operazioni  :                                         
        /// 17.4.	Se RICALSTATO (IND-STA) maggiore di zero effettuare le seguenti operazioni :                                         
        /// 17.4.2.	Se APPO-CAT1 non è uguale a "V" effettuare le seguenti operazioni :
        /// 17.4.2.1.	Valorizzare con "13"  il campo  TIPO-ERRORE, con "DATA RICALCOLO INCOMPATIBILE CON CATEGORIA"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da 
        /// CONTROLLI-1 (33);                      
        /// 17.4.3.	 Se  SETT1(IND-STA) è uguale a zero e SETT2(IND-STA) è uguale a zero effettuare le seguenti operazioni :                                        
        /// 17.4.3.1.	Valorizzare con "14"  il campo  TIPO-ERRORE, con "DATA RICALCOLO INCOMPATIBILE CON SETTIMANE ESTERE"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);         
        /// </summary>
        /// <param name="dataRicalcolo"></param>
        /// <param name="gruppo"></param>
        /// <param name="contributEEDecorrenzaOriginaria"></param>
        /// <param name="contributiEERicalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataRicalcolo(DateTime? dataRicalcolo, string gruppo, int? contributEEDecorrenzaOriginaria, int? contributiEERicalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (dataRicalcolo.HasValue)
            {
                if (!gruppo.Equals("0001"))
                {
                    messaggioVideo = "Data Ricalcolo incompatibile con Categoria";
                    return false;
                }

                if (contributEEDecorrenzaOriginaria.GetValueOrDefault() == 0 && contributiEERicalcolo.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Data Ricalcolo incompatibile con Settimane Estere";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.8.	Se   SETT2(IND-STA)  è maggiore di zero effettuare le seguenti operazioni : 
        /// 17.8.1.	Se ICI2CONV è uguale a 17 e STATO(1)  è uguale a 17 effettuare le seguenti operazioni : 
        /// 17.8.1.1.	Valorizzare con "18"  il campo  TIPO-ERRORE, con "SETTIMANE ESTERE A RICALCOLO INCOMPATIBILI  CON STATO / CONVENZIONE (17)" il campo MESSAGGIO-ERRORE, con 1 
        /// il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.9.	Se   SETT2(IND-STA) è maggiore di zero e DEC(IND-STA 1) è uguale a zero effettuare le seguenti operazioni :                                                      
        /// 17.9.1.	Valorizzare con "18"  il campo  TIPO-ERRORE, con "SETTIMANE ESTERE A RICALCOLO INCOMPATIBILI CON DECORR. ESTERO" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.10.	Se SETT2(IND-STA) è maggiore di zero e DEC(IND-STA 1) è uguale a IW1DEORIG         effettuare le seguenti operazioni :                                                      
        /// 17.10.1.	Valorizzare con "19"  il campo  TIPO-ERRORE, con "SETTIMANE ESTERE A RICALCOLO INCOMPATIBILI CON DECORR. ESTERO" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.11.	Se SETT2(IND-STA) è maggiore di zero e inferiore a SETT1(IND-STA) effettuare le seguenti operazioni :                                                     
        /// 17.11.1.	Valorizzare con "18"  il campo  TIPO-ERRORE, con "SETTIMANE RICALCOLO INFERIORI A SETTIMANE A DEC.PENSIONE" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33); 
        /// </summary>
        /// <param name="contributiEERicalcolo"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="stato"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneARicalcolo(int? contributiEERicalcolo, byte? codiceConvenzione, int? stato, DateTime? primaDecorrenzaImportiEsteri, DateTime? decorrenzaOriginaria, int? contributiEEDecorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (contributiEERicalcolo.GetValueOrDefault() > 0)
            {
                if (codiceConvenzione.GetValueOrDefault() == 17 && stato.GetValueOrDefault() == 17)
                {
                    messaggioVideo = "Settimane Estere a Ricalcolo incompatibili con Stato / Convenzione (17)";
                    return false;
                }

                if (!primaDecorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Settimane Estere a Ricalcolo incompatibili con Decorrenza Estero";
                    return false;
                }

                if (primaDecorrenzaImportiEsteri.Equals(decorrenzaOriginaria))
                {
                    messaggioVideo = "Settimane Estere a Ricalcolo incompatibili con Decorrenza Estero";
                    return false;
                }

                if (contributiEERicalcolo.GetValueOrDefault() < contributiEEDecorrenzaOriginaria.GetValueOrDefault())
                {
                    messaggioVideo = "Settimane Ricalcolo inferiori a Settimane a Decorrenza Pensione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ///  IMPORTANTE: testare questo controllo con una domanda avente codice convenzione pari a 35
        /// 
        /// 17.13.	Se ICI2CONV è uguale a 35 effettuare le seguenti operazioni : 
        /// 17.13.1.	Se STATO(IND-STA) è uguale a 36 e SETTDIR(IND-STA) è uguale a zero effettuare le seguenti operazioni :                                                     
        /// 17.13.1.1.	Valorizzare con "87"  il campo  TIPO-ERRORE, con "CONTRIBUTI ESTERI MANCANTI PER STATO  TURCHIA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  
        /// da CONTROLLI-1 (33);                               
        /// 17.13.2.	Se STATO(IND-STA) non è uguale a 36 e SETTDIR(IND-STA) è maggiore di zero effettuare le seguenti operazioni :                                                     
        /// 17.13.2.1.	Valorizzare con "87"  il campo  TIPO-ERRORE, con "CONTRIBUTI ESTERI INCOMPATIBILI CON CONVENZIONE “ +  ICI2CONV  il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// 17.14.	Diversamente da quanto analizzato nel punto precedente (17.13) effettuare le seguenti operazioni : 
        /// 17.14.1.	Se STATO(IND-STA) è uguale a  36 e SETTDIR(IND-STA) è maggiore di zero effettuare le seguenti operazioni : 
        /// 17.14.2.	Valorizzare con "87"  il campo  TIPO-ERRORE, con "CONTRIBUTI STATO 36 (TURCHIA) INCOMPATIBILE CON CONV. " + ICI2CONV il campo MESSAGGIO-ERRORE, con 1 il 
        /// campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="stato"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaContributiTurchia(List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione, byte? codiceConvenzione, int? stato, int? contributiEEDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciTurchia = listaCodiciConvenzione != null && listaCodiciConvenzione.Count > 0 ? listaCodiciConvenzione.FindAll(x => x.CodiceStato == "36") : null;

            if (codiceConvenzione.HasValue && listaCodiciTurchia != null && listaCodiciTurchia.Count > 0 && listaCodiciTurchia.Exists(x => x.CodiceConvenzione == codiceConvenzione.Value))
            {
                if (stato.GetValueOrDefault() == 36 && contributiEEDiritto.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Contributi Esteri mancanti per stato Turchia";
                    return false;
                }

                if (stato.GetValueOrDefault() != 36 && contributiEEDiritto.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Contributi Esteri incompatibili con Convenzione " + codiceConvenzione;
                    return false;
                }
            }
            else
            {
                if (stato.GetValueOrDefault() == 36 && contributiEEDiritto.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Contributi Stato 36 (Turchia) incompatibile con Convenzione " + codiceConvenzione;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.17.	Se  COD-SOSP-ESTERO(IND-STA) è uguale a "S"  effettuare le seguenti operazioni :                                                         
        /// 17.17.1.	Se APPO-CAT1  è uguale a "S"  effettuare le seguenti operazioni :                                                       
        /// 17.17.1.1.	Valorizzare con "88"  il campo  TIPO-ERRORE, con "SOSPENSIONE INTEGRAZIONE INCOMPATIBILE CON CATEGORIA SO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);                               
        /// 17.17.2.	Se ART48(IND-STA) è uguale a "S" effettuare le seguenti operazioni : 
        /// 17.17.2.1.	Valorizzare con "88"  il campo  TIPO-ERRORE, con "SOSPENSIONE INTEGRAZIONE INCOMPATIBILE  CON CODICE ARTICOLO 48", il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// 17.17.3.	Se ETA-SOSP-ESTERO(IND-STA) è maggiore di "00" e ((ETA-SOSP-ESTERO(IND-STA) è minore di "56" oppure è maggiore di "88")   oppure  (ETA-SOSP-ESTERO(IND-STA) è 
        /// minore di "61"  e IW1SESTIT non è uguale a "F"))  effettuare le seguenti operazioni :                                                       
        /// 17.17.3.1.	Valorizzare con "89"  il campo  TIPO-ERRORE, con "ETA' SOSPENSIONE ERRATA O ILLOGICA" il campo               MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  
        /// da CONTROLLI-1 (33); 
        /// 17.17.4.	Valorizzare con ETA-SOSP-ESTERO(IND-STA) il campo W-APP-APP                  
        /// 17.17.5.	Se IW1CARIC  è uguale a 2  effettuare le seguenti operazioni : 
        /// 17.17.5.1.	Se (W-APP-APP + IW1SECAN - 2) è maggiore di DATA-SYS-SSAA effettuare le seguenti operazioni : 
        /// 17.17.5.1.1.	Valorizzare con "89"  il campo  TIPO-ERRORE, con "ETA' SOSPENSIONE INCOMPATIBILE  CON DATA DI NASCITA   (1)" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// 17.17.6.	Diversamente da quanto analizzato nel punto precedente (17.17.5) effettuare le seguenti operazioni : 
        /// 17.17.6.1.	Se (W-APP-APP + IW1SECAN - 1) è maggiore di DATA-SYS-SSAA effettuare le seguenti operazioni : 
        /// 17.17.6.1.1.	Valorizzare con "89"  il campo TIPO-ERRORE, con "ETA' SOSPENSIONE INCOMPATIBILE CON DATA DI NASCITA    (2)" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// 17.18.	Diversamente da quanto analizzato nel punto precedente (17.17)  effettuare le seguenti operazioni :                                                      
        /// 17.18.1.	Se ETA-SOSP-ESTERO(IND-STA) è maggiore di "00" effettuare le seguenti operazioni :                                                       
        /// 17.18.1.1.	Valorizzare con "89"  il campo  TIPO-ERRORE, con "ETA' (CODICE VIRTUALE) INCOMPATIBILE CON CODICE SOSPENSIONE" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                               
        /// </summary>
        /// <param name="sospensioneCautelativaIntegrazione"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="codiceArt48"></param>
        /// <param name="etaSospensione"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="causaCarico"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSospensioneEstero(char? sospensioneCautelativaIntegrazione, Utility.TipoDomanda tipoDomanda, char? codiceArt48, byte? etaSospensione, char? sessoTitolare, byte? causaCarico, DateTime? dataNascitaTitolare, byte? codiceConvenzione,  out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (sospensioneCautelativaIntegrazione.GetValueOrDefault() == 'S')
            {
                if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                {
                    messaggioVideo = "Sospensione Integrazione incompatibile con Categoria SO";
                    return false;
                }

                if (codiceArt48.GetValueOrDefault() == 'S' && codiceConvenzione.GetValueOrDefault() != 62) //moldavia
                {
                    messaggioVideo = "Sospensione Integrazione incompatibile con Codice Articolo 48";
                    return false;
                }

                if (etaSospensione.GetValueOrDefault() > 0 && ((etaSospensione.GetValueOrDefault() < 56 || etaSospensione.GetValueOrDefault() > 88) || (etaSospensione.GetValueOrDefault() < 61 && sessoTitolare.GetValueOrDefault() != 'F')))
                {
                    messaggioVideo = "Eta' Sospensione errata o illogica";
                    return false;
                }

                if (causaCarico.GetValueOrDefault() == 2)
                {
                    if (etaSospensione.GetValueOrDefault() + dataNascitaTitolare.Value.Year - 2 > dataSistema.Year)
                    {
                        messaggioVideo = "Eta' Sospensione incompatibile con Data di Nascita";
                        return false;
                    }
                }
                else
                {
                    if (etaSospensione.GetValueOrDefault() + dataNascitaTitolare.Value.Year - 1 > dataSistema.Year)
                    {
                        messaggioVideo = "Eta' Sospensione incompatibile con Data di Nascita";
                        return false;
                    }
                }
            }
            else
            {
                if (etaSospensione.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Eta' (Codice Virtuale) incompatibile con Codice Sospensione";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.20.	Se  ART48(IND-STA)  è uguale a "S" effettuare le seguenti operazioni :                                                         
        /// 17.20.1.	Se (ICI2CONV è uguale a 12, 14, 22, 34, 37, 23, 53 o 30)  oppure (ICI2CONV è uguale a 38 e (W-DEORIG  è maggiore o uguale al 200208 e minore o oguale al 
        /// 200404))  oppure (ICI2CONV è uguale a 39 e (W-DEORIG  è maggiore o uguale al 200311)) oppure (ICI2CONV è uguale a 17 e (W-DEORIG  entecedente al 200206)) oppure 
        /// (ICI2CONV è uguale a 35 )  oppure ( (ICI2CONV è uguale a 9 oppure 13 oppure 17 oppure 27)  e (IW1DEOP  è maggiore di zero)) continuare l’elaborazione al punto successivo 
        /// (17.20.3) 
        /// 17.20.2.	Diversamente da quanto analizzato nel punto precedente (17.20.1) :           Valorizzare con "91" il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE 
        /// CON CONVENZIONE" il campo  MESSAGGIO-ERRORE,con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.20.3.	Se DEC(IND-STA 1) è maggiore di zero effettuare le seguenti operazioni :                                                      
        /// 17.20.3.1.	Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON  PRESTAZIONE ESTERA"  il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.20.4.	Se  (SETT1(IND-STA) è maggiore di 52  e  STATO(IND-STA) non è uguale a 23)  oppure  (SETT1(IND-STA) è maggiore di 78)  effettuare le seguenti operazioni:                                                      
        /// 17.20.4.1.	Valorizzare con "91"  il campo TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON SETTIMANE MISURA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);                                
        /// 17.20.5.	Se  (SETTDIR(IND-STA) è maggiore di 52  e  STATO(IND-STA) non è uguale a 23)  oppure  (SETTDIR(IND-STA) è maggiore di 78 ) effettuare le seguenti operazioni :                                                      
        /// 17.20.5.1.	Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON SETTIMANE DIRITTO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);
        /// 17.20.6.	Se ICI2CONV è uguale a 12 effettuare le seguenti operazioni : 
        /// 17.20.6.1.	Se STATO(IND-STA) è uguale a  1, 2, 4, 6, 7,  9, 10, 11, 18, 19, 20 , 27, 28, 29, 32, 38, 40, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 17         continuare l’elaborazione al punto successivo (17.20.6.3) ;  
        /// 17.20.6.2.	Diversamente da quanto analizzato nel punto precedente (17.20.6.1) effettuare le seguenti operazioni :                                                    
        /// ************* CEE E SPAGNA           
        /// 17.20.6.2.1.	Se ((STATO(1) è uguale a 11 oppure STATO(2) è uguale a 11 oppure STATO(3) è uguale a 11 oppure STATO(4) è uguale a 11) e  (STATO(IND-STA) è uguale a 11, 14, 21, 23, 24, 25, 31 oppure 36) )                 
        /// ************* CEE E SVEZIA 
        /// oppure ( (STATO(1) è uguale a 20 oppure STATO(2) è uguale a 20 oppure STATO(3) è uguale a 20 oppure STATO(4) è uguale a 20)  e  (STATO(IND-STA) è uguale a 20, 13, 23, 24, 25, 36, 38, 39, 42, 43, 56 oppure 57 )  )                       
        /// ************* CEE E SVIZZERA    
        /// oppure ( (STATO(1) è uguale a 17 oppure STATO(2) è uguale a 17 oppure STATO(3) è uguale a 17 oppure STATO(4) è uguale a 17) e (STATO(IND-STA) è uguale a 17, 13, 23, 36, 38,  39, 42, 43, 56, 57, 24 oppure 25))  continuare l’elaborazione al punto successivo (17.20.7) ;                                    
        /// 17.20.6.2.2.	Diversamente da quanto analizzato nel punto precedente  (17.20.6.2.1) : Valorizzare con "91"  il campo  TIPO-ERRORE,  con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " + ICI2CONV  il campo    MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                         
        /// ************* ARGENTINA   -- TUTTI                                
        /// ************* SAN MARINO        
        /// 17.20.7.	Se ICI2CONV è uguale a 22 effettuare le seguenti operazioni :  
        /// 17.20.7.1.	Se STATO(IND-STA) è uguale a 22 continuare l’elaborazione al punto successivo (17.20.8) ;                                     
        /// 17.20.7.2.	Diversamente da quanto analizzato nel punto precedente (17.20.7.1) :  Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " + ICI2CONV  il campo MESSAGGIO-ERRORE, con 1 il campo       FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// ************* TUNISIA      
        /// 17.20.8.	Se ICI2CONV è uguale a 34 effettuare le seguenti operazioni : 
        /// 17.20.8.1.	Se (STATO(IND-STA) è uguale a 34, 1, 2, 6, 7 oppure a 10) continuare l’elaborazione al punto successivo (17.20.9) ;              
        /// 17.20.8.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :
        /// Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE "  +  ICI2CONV                         
        /// il campo MESSAGGIO-ERRORE, con 1 il  campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// ************* VENEZUELA                                           
        /// 17.20.9.	Se ICI2CONV è uguale a 37 effettuare le seguenti operazioni : 
        /// 17.20.9.1.	Se STATO(IND-STA) è uguale a 37 continuare l’elaborazione al punto successivo (17.20.10) ;                                      
        /// 17.20.9.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91" il campo TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " + ICI2CONV il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// ************* USA                                                 
        /// 17.20.10.	Se ICI2CONV è uguale a 23 effettuare le seguenti operazioni : 
        /// 17.20.10.1.	Se STATO(IND-STA) è uguale a 23  continuare l’elaborazione al punto successivo (17.20.11) ; 
        /// 17.20.10.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " + ICI2CONV 
        /// il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-1 (33);                                
        /// ************* SLOVENIA  
        /// 17.20.11.	Se ICI2CONV è uguale a 38 effettuare le seguenti operazioni :                                                       
        /// 17.20.11.1.	Se STATO(IND-STA) è uguale a 38, 1, 2, 4, 7, 9, 10, 17, 18, 20, 24, 25, 29, 39, oppure 43 continuare l’elaborazione al punto successivo (17.20.12) ; 
        /// 17.20.11.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " + ICI2CONV                         
        /// il campo MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// ************* CROAZIA                                             
        /// 17.20.12.	Se ICI2CONV è uguale a 39  effettuare le seguenti operazioni : 
        /// 17.20.12.1.	Se STATO(IND-STA) è uguale a 39, 1,  2, 4, 6, 7, 9, 10, 13, 17, 18, 19, 20, 24, 25, 29, 38, 42, 43, 56 oppure a 57
        /// continuare l’elaborazione al punto successivo (17.20.13) ; 
        /// 17.20.12.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91"  il campo TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZIONE " +  ICI2CONV  il campo MESSAGGIO-ERRORE, con 1 il campo   FLAG-ERR, uscire da CONTROLLI-1 (33); 
        /// ************* VATICANO    -- TUTTI                                
        /// ************* CAPO VERDE  
        /// 17.20.13.	Se ICI2CONV è uguale a 30  effettuare le seguenti operazioni : 
        /// 17.20.13.1.	Se STATO(IND-STA) è uguale a 30, 1, 6, 7, 20 oppure a 32 : continuare l’elaborazione al punto successivo (17.20.14); 
        /// 17.20.13.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 
        /// INCOMPATIBILE CON CONVENZIONE " +  ICI2CONV  il campo MESSAGGIO-ERRORE, con 1 il campo   FLAG-ERR, uscire da CONTROLLI-1 (33); 
        /// 17.20.14.	Se ICI2CONV è uguale a 17 effettuare le seguenti operazioni : 
        /// 17.20.14.1.	Se STATO(IND-STA) è uguale a 17, 13, 23, 36, 38, 39, 42, 43, 24, 25, 56, 57, 1,  2,  4,  6,  7,  9, 10, 11, 18,  19, 20, 27, 28, 29, 32, 38, 40, 41, 44, 45, 
        /// 46, 47, 48, 49, 50, 51 oppure 52           effettuare le seguenti operazioni : 
        /// 17.20.14.1.1.	Se IW1CARIC è uguale a 2 o 9 e IW1DOMOPZ è uguale a zero : Valorizzare con "91"  il campo      TIPO-ERRORE, con "COD.ARTICOLO 48 INCOMPATIBILE CON 
        /// OPZIONE CONVENZIONE SVIZZERA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                           
        /// 17.20.14.2.	Diversamente da quanto analizzato nel punto precedente (17.20.14.1) : Valorizzare con "91"  il campo TIPO-ERRORE,  con "CODICE ARTICOLO 48 INCOMPATIBILE CON 
        /// CONVENZIONE "  + ICI2CONV il campo MESSAGGIO-ERRORE, con 1 il campo    FLAG-ERR, uscire  da CONTROLLI-1 (33); 
        ///         17.20.15.	Se ICI2CONV è uguale a 53  effettuare le seguenti operazioni : 
        /// 17.20.15.1.	 Se TP1CITT1 è uguale a "I  ", "F  ", "D  ", "NL ", "B  ", "E  ", "P  ", "GR ", "GB ", "A  ", "S  ", "DK ", "L  ", "SF ", "FIN", "IRL", "PL ",   "H  ", "CZ ", "SK ", "LT ", "LV ", "EST", "M  ", "CY " oppure "SLO" continuare l’elaborazione al punto successivo (17.20.16);
        /// 17.20.15.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "91" il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZ. 53  E CITTADINANZA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// 17.20.16.	Se ICI2CONV è uguale a 17 effettuare le seguenti operazioni : 
        /// 17.20.16.1.	Se  (IW1CARIC è uguale a 2 oppure 9)  e IW1DEOP è uguale a zero : Valorizzare con "91"  il campo  TIPO-ERRORE, con "CODICE ARTICOLO 48 INCOMPATIBILE CON CONVENZ. 17  ED OPZIONE" il campo MESSAGGIO-ERRORE, con 1 il campo   FLAG-ERR, uscire  da CONTROLLI-1 (33);                              
        /// </summary>
        /// <param name="codiceArt48"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaArticolo48(GestionePensione.DatiPensione datiPensione, char? codiceArt48, byte? codiceConvenzione, DateTime? decorrenzaOpzione, DateTime? decorrenzaArt48, DateTime? primaDecorrenzaImportiEsteri, int? contributiEEDecorrenzaOriginaria, int? codiceStatoEE, int? contributiEEDiritto, bool stato11Presente, bool stato20Presente, bool stato17Presente, string cittadinanza, DateTime? dataDomandaOpzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 62) //moldavia
                return true;

            if (codiceArt48.GetValueOrDefault() == 'S')
            {
                if (!((new List<byte> { 12, 14, 22, 34, 37, 23, 53, 30, 60 }).Contains(codiceConvenzione.GetValueOrDefault()) ||
                    (codiceConvenzione.GetValueOrDefault() == 38 && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2002, 08, 01)) && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2004, 04, 01))) ||
                    (codiceConvenzione.GetValueOrDefault() == 39 && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2003, 11, 01))) ||
                    (codiceConvenzione.GetValueOrDefault() == 17 && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2002, 06, 01))) || codiceConvenzione.GetValueOrDefault() == 35 ||
                    ((new List<byte> { 9, 13, 17, 27 }).Contains(codiceConvenzione.GetValueOrDefault()) && decorrenzaOpzione.HasValue) || (codiceConvenzione.GetValueOrDefault() == 58 && Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 12, 01)))))
                {
                    messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione";
                    return false;
                }

                if (primaDecorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Codice Articolo 48 incompatibile con Prestazione Estera";
                    return false;
                }

                if ((contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 52 && codiceStatoEE.GetValueOrDefault() != 23) || contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 78)
                {
                    messaggioVideo = "Codice Articolo 48 incompatibile con Settimane Misura";
                    return false;
                }

                if ((contributiEEDiritto.GetValueOrDefault() > 52 && (codiceStatoEE.GetValueOrDefault() != 23 || codiceConvenzione.GetValueOrDefault() != 60)) || contributiEEDiritto.GetValueOrDefault() > 78)
                {
                    messaggioVideo = "Codice Articolo 48 incompatibile con Settimane Diritto";
                    return false;
                }

                if (codiceConvenzione.GetValueOrDefault() == 12)
                {
                    if (!(new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 27, 28, 29, 32, 38, 40, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 17, 54, 55 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        if (!((stato11Presente && (new List<int> { 11, 14, 21, 23, 24, 25, 31, 36 }).Contains(codiceStatoEE.GetValueOrDefault())) ||
                            (stato20Presente && (new List<int> { 20, 13, 23, 24, 25, 36, 38, 39, 42, 43, 56, 57 }).Contains(codiceStatoEE.GetValueOrDefault())) ||
                            (stato17Presente && (new List<int> { 17, 13, 23, 36, 38, 39, 42, 43, 56, 57, 24, 25 }).Contains(codiceStatoEE.GetValueOrDefault()))))
                        {
                            messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                            return false;
                        }
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 22)
                {
                    if (codiceStatoEE.GetValueOrDefault() != 22)
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 34)
                {
                    if (!(new List<int> { 34, 1, 2, 6, 7, 10 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 37)
                {
                    if (codiceStatoEE.GetValueOrDefault() != 37)
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 23)
                {
                    if (codiceStatoEE.GetValueOrDefault() != 23)
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 38)
                {
                    if (!(new List<int> { 38, 1, 2, 4, 7, 9, 10, 17, 18, 20, 24, 25, 29, 39, 43 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 39)
                {
                    if (!(new List<int> { 39, 1, 2, 4, 6, 7, 9, 10, 13, 17, 18, 19, 20, 24, 25, 29, 38, 42, 43, 56, 57 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 30)
                {
                    if (!(new List<int> { 30, 1, 6, 7, 20, 32 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione;
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 17)
                {
                    if ((new List<int> { 17, 13, 23, 36, 38, 39, 42, 43, 24, 25, 56, 57, 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 27, 28, 29, 32, 38, 40, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52 }).Contains(codiceStatoEE.GetValueOrDefault()))
                    {
                        if ((datiPensione.CausaCarico.GetValueOrDefault() == 2 || datiPensione.CausaCarico.GetValueOrDefault() == 9) && !dataDomandaOpzione.HasValue)
                        {
                            messaggioVideo = "Cod.Articolo 48 incompatibile con Opzione Convenzione Svizzera";
                            return false;
                        }
                    }
                    else
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione " + codiceConvenzione.GetValueOrDefault();
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 53)
                {
                    if (string.IsNullOrEmpty(cittadinanza) || !(new List<string> { "I  ", "F  ", "D  ", "NL ", "B  ", "E  ", "P  ", "GR ", "GB ", "A  ", "S  ", "DK ", "L  ", "SF ", "FIN", "IRL", "PL ", "H  ", "CZ ", "SK ", "LT ", "LV ", "EST", "M  ", "CY ", "SLO" }).Contains(cittadinanza))
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione 53 e Cittadinanza";
                        return false;
                    }
                }

                if (codiceConvenzione.GetValueOrDefault() == 17)
                {
                    if ((datiPensione.CausaCarico.GetValueOrDefault() == 2 || datiPensione.CausaCarico.GetValueOrDefault() == 9) && !decorrenzaOpzione.HasValue)
                    {
                        messaggioVideo = "Codice Articolo 48 incompatibile con Convenzione 17 ed Opzione";
                        return false;
                    }
                }

                if (decorrenzaArt48.HasValue && !Utility.DataSuccessivaA(decorrenzaArt48.GetValueOrDefault(), datiPensione.DecorrenzaOriginaria.GetValueOrDefault()))
                {
                    messaggioVideo = "La decorrenza art. 48 deve essere maggiore o uguale alla decorrenza della pensione.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.21.	Se APPO-CAT1 non è uguale a "S" e ICI2CONV è uguale a 12 e IW1DEORIG è maggiore del 199312 effettuare le seguenti operazioni :                                                         
        /// 17.21.1.	Se  (STATO(IND-STA) è maggiore di zero e SETT1(IND-STA) è uguale a zero e IMPEST(IND-STA 1) è maggiore di zero) e (STATO(IND-STA) è uguale a 9 oppure 20 
        /// oppure 29 oppure 40 oppure 41)  : Valorizzare con "17" il campo TIPO-ERRORE, con "SETTIMANE ESTERI MANCANTI (STATO CEE)" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire da CONTROLLI-1 (33);                                
        /// 17.22.	Se (ICI2CONV è uguale a 38 oppure 39) e SETT1(IND-STA) è maggiore di zero  effettuare le seguenti operazioni :  
        /// 17.22.1.	Se (STATO(IND-STA) è uguale a 38, 17, 39, 43, 7, 9, 2, 24, 25, 18, 1, 10, 4, 27, 29, 20, 13, 19, 20, 42, 56, 57) continuare l’elaborazione al punto successivo (17.23);
        /// 17.22.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "17"  il campo  TIPO-ERRORE, con "SETTIMANE ESTERI 
        /// INCOMPATIBILI CON CONVENZIONE 38/39 (SLOVENIA-CROAZIA)"  il campo MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);  
        /// 17.27.	Se ICI2CONV  è uguale a  W-APP-CON effettuare le seguenti operazioni :  
        /// 17.27.1.	Se  SETT1(IND-STA) è uguale a zero effettuare le seguenti operazioni : 
        /// 17.27.1.1.	Se (STATO(IND-STA) è uguale a STATO(1) e SETT1(1) è maggiore di zero) oppure (STATO(IND-STA) è uguale a STATO(2) e SETT1(2) è maggiore di zero) oppure 
        /// (STATO(IND-STA) è uguale a STATO(3) e SETT1(3) è maggiore di zero) oppure   (STATO(IND-STA) è uguale a STATO(4) e SETT1(4) è maggiore di zero) oppure (STATO(IND-STA) è 
        /// uguale a 17 e ICI2CONV è uguale a 12 e IW1DEORIG minore di 200206 e IW1DEOP è uguale a zero)  continuare l’elaborazione al punto successivo (17.28);
        /// 17.27.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni :  
        /// *Se NON 1'STATO, CTR POSSONO MANCARE Se CI SONO QUELLI A RICALC.  
        /// 17.27.1.2.1.	Se ( IND-STA è maggiore di 1 e SETT2(IND-STA) è maggiore di zero e DEC(IND-STA 1) è maggiore di zero e non è uguale a IW1DEORIG )  oppure (STATO(IND-STA) 
        /// è uguale a 1 e ISTIT(IND-STA)  è uguale a 300 )   continuare l’elaborazione al punto successivo (17.28);                                      
        /// 17.27.1.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "17"  il campo  TIPO-ERRORE, con "SETTIMANE 
        /// ESTERI MANCANTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                          
        /// </summary>
        /// <param name="tipoDomanda"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="primoImportoPrestazioneEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneEstere(Utility.TipoDomanda tipoDomanda, byte? codiceConvenzione, DateTime? decorrenzaOriginaria, int? codiceStatoEE, int? contributiEEDecorrenzaOriginaria, decimal? primoImportoPrestazioneEE, DateTime? decorrenzaOpzione, int index, int? contributiEERicalcolo, DateTime? primaDecorrenzaImportiEsteri, int? codiceIstituzione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (tipoDomanda != Utility.TipoDomanda.Superstiti && codiceConvenzione.GetValueOrDefault() == 12 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1993, 12, 31)))
            {
                if (contributiEEDecorrenzaOriginaria.GetValueOrDefault() == 0 && primoImportoPrestazioneEE.GetValueOrDefault() > 0 && (new List<int> { 9, 20, 29, 40, 41 }).Contains(codiceStatoEE.GetValueOrDefault()))
                {
                    messaggioVideo = "Settimane Esteri mancanti (Stato CEE)";
                    return false;
                }
            }

            if ((codiceConvenzione.GetValueOrDefault() == 38 || codiceConvenzione.GetValueOrDefault() == 39) && contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
            {
                if (!(new List<int> { 38, 17, 39, 43, 7, 9, 2, 24, 25, 18, 1, 10, 4, 27, 29, 20, 13, 19, 20, 42, 56, 57 }).Contains(codiceStatoEE.GetValueOrDefault()))
                {
                    messaggioVideo = "Settimane Esteri incompatibili con Convenzione 38/39 (Slovenia-Croazia)";
                    return false;
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == GetCodiceConvenzioneByCodiceStatoEE(codiceStatoEE, decorrenzaOriginaria))
            {
                if (contributiEEDecorrenzaOriginaria.GetValueOrDefault() == 0 && !(codiceStatoEE.GetValueOrDefault() == 01 && (codiceIstituzione.GetValueOrDefault() == 0509 || codiceIstituzione.GetValueOrDefault() == 0510 || codiceIstituzione.GetValueOrDefault() == 0511)))
                {
                    if (!(codiceStatoEE.GetValueOrDefault() == 17 && codiceConvenzione.GetValueOrDefault() == 12 && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 06, 01)) && !decorrenzaOpzione.HasValue))
                    {
                        if (!((index > 0 && contributiEERicalcolo.GetValueOrDefault() > 0 && primaDecorrenzaImportiEsteri.HasValue && !primaDecorrenzaImportiEsteri.Equals(decorrenzaOriginaria)) || (codiceStatoEE.GetValueOrDefault() == 1 && codiceIstituzione.GetValueOrDefault() == 300)))
                        {
                            messaggioVideo = "Settimane Esteri mancanti";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 17.28.	Se ICI2CONV è uguale a 53 e STATO(IND-STA) non è uguale a 53 e SETT1(IND-STA) è maggiore di zero effettuare le seguenti operazioni :                                                        
        /// 17.28.1.	Se (TP1CITT1  è uguale a "I  " oppure "F  " oppure "D  "  oppure "NL " oppure "L  " oppure "B  " oppure "E  " oppure "P  " oppure "GR " oppure "GB " oppure 
        /// "IRL" oppure "A  " oppure "S  " oppure "DK "                             
        /// *** PER LA FINLANDIA ERA 'SF ' E ORA E' 'FIN'                      
        /// oppure "SF " oppure "FIN"                                      
        /// *( E Se E' CITTADINO VATICANO)                                    
        /// oppure "V  "                                               
        /// *** PER I SEGUENTI NON SONO SICURO !!!! STATI EFTA O SEE ??        
        /// oppure "IS " oppure "N  " oppure "FL " oppure "CH ")                  
        /// *** NUOVI STATI UE DAL 05/2004                                     
        /// oppure ((TP1CITT1 è uguale a "PL " oppure "H  " oppure "CZ " oppure    "SK " oppure "LT " oppure "LV " oppure "EST" oppure "M  " oppure "CY " oppure "SLO")  e IW1DEORIG 
        /// non inferiore al 200405 )                      
        /// *** E' AMMESSA L'ACQUISIZIONE DI SETTIMANE                  
        /// *** PER STATI DIVERSI DA VATICANO                           
        /// continuare l’elaborazione al punto successivo (17.29);                                      
        /// 17.28.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :  Valorizzare con "17" il campo  TIPO-ERRORE, con "TOTALIZZAZIONE 
        /// MULTIPLA POSSIBILE SOLO PER I CITTADINI U.E." il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                                
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="cittadinanza"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaConvenzioneVaticano(byte? codiceConvenzione, int? codiceStatoEE, int? contributiEEDecorrenzaOriginaria, string cittadinanza, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 53 && codiceStatoEE.GetValueOrDefault() != 53 && contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
            {
                if (!(!string.IsNullOrEmpty(cittadinanza) && (new List<string> { "I  ", "F  ", "D  ", "NL ", "L  ", "B  ", "E  ", "P  ", "GR ", "GB ", "IRL", "A  ", "S  ", "DK ", "SF ", "FIN", "V  ", "IS ", "N  ", "FL ", "CH " }).Contains(cittadinanza) ||
                    (!string.IsNullOrEmpty(cittadinanza) && (new List<string> { "PL ", "H  ", "CZ ", "SK ", "LT ", "LV ", "EST", "M  ", "CY ", "SLO" }).Contains(cittadinanza) && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 05, 31)))))
                {
                    messaggioVideo = "Totalizzazione multipla possibile solo per i cittadini U.E.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.29.	Se STATO(IND-STA) è uguale a 1 e ISTIT(IND-STA) è uguale a 300  effettuare le seguenti operazioni :                                                        
        /// 17.29.1.	Se  SETT1(IND-STA)   è maggiore di zero oppure  SETT2(IND-STA)   è maggiore di zero oppure SETTDIR(IND-STA) è maggiore di zero oppure  DEC(IND-STA 1)   è 
        /// maggiore di zero effettuare le seguenti operazioni :                Valorizzare con "16"  il campo  TIPO-ERRORE, con "ISTIT. 01/300 (CAFAT - NUOVA CALEDONIA)NON AMMETTE 
        /// CONTRIBUTI O IMPORTI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33); 
        /// 17.29.2.	Se COD-SOSP-ESTERO(IND-STA) non è uguale a "N" : Valorizzare con "88"  il campo  TIPO-ERRORE, con "ISTIT. 01/300 (CAFAT - NUOVA CALEDONIA) COD.SOSP.T.M. 
        /// DIVERSO DA 'N’" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// </summary>
        /// <param name="codiceStatoEE"></param>
        /// <param name="codiceIstituzione"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="contributiEERicalcolo"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="sospensioneCautelativaIntegrazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaNuovaCaledonia(int? codiceStatoEE, int? codiceIstituzione, int? contributiEEDecorrenzaOriginaria, int? contributiEERicalcolo, int? contributiEEDiritto, DateTime? primaDecorrenzaImportiEsteri, char? sospensioneCautelativaIntegrazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceStatoEE.GetValueOrDefault() == 1 && codiceIstituzione.GetValueOrDefault() == 300)
            {
                if (contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0 || contributiEEDiritto.GetValueOrDefault() > 0 || contributiEERicalcolo.GetValueOrDefault() > 0 || primaDecorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Istit. 01/300 (CAFAT - NUOVA CALEDONIA) non ammette contributi o importi";
                    return false;
                }

                if (sospensioneCautelativaIntegrazione.GetValueOrDefault() != 'N')
                {
                    messaggioVideo = "Istit. 01/300 (CAFAT - NUOVA CALEDONIA) Sospensione Integrazione Trattamento Minimo diverso da 'N’";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 18.	Se  IW1CARIC è uguale a 2 e TP1CODELIM è maggiore di "0"  continuare l’elaborazione al punto successivo (20);         
        /// ***** CONTROLLO PRESENZA DELLA MATRICOLA (13.5.02)                     
        /// 19.	Se  (MATRIC(IND-STA) è uguale a spazi oppure a LOW-VALUE  e ( (STATO(IND-STA) è uguale a 10)  oppure (STATO(IND-STA) è uguale a 2 e ISTIT(IND-STA) è uguale a 1) 
        /// oppure  (STATO(IND-STA) è uguale a 1)) e (DEC (IND-STA INDICE)  è maggiore di zero) :           Valorizzare con "17"  il campo  TIPO-ERRORE, con "ISTITUZIONE  
        /// STATO(IND-STA) "/" ISTIT(IND-STA) CON MATRICOLA MANCANTE: VARIARE CI81" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceMotivo"></param>
        /// <param name="matricola"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="codiceIstituzione"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaPresenzaMatricola(byte? causaCarico, byte? codiceMotivo, string matricola, int? codiceStatoEE, int? codiceIstituzione, DateTime? primaDecorrenzaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!(causaCarico.GetValueOrDefault() == 2 && codiceMotivo.GetValueOrDefault() > 0))
            {
                if ((string.IsNullOrEmpty(matricola) || string.IsNullOrEmpty(matricola.Trim())) && (codiceStatoEE.GetValueOrDefault() == 10 || (codiceStatoEE.GetValueOrDefault() == 2 && codiceIstituzione.GetValueOrDefault() == 1)) && primaDecorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Istituzione " + codiceStatoEE.GetValueOrDefault() + " / " + codiceIstituzione.GetValueOrDefault().ToString().PadLeft(4, '0') + " con Matricola mancante: Variare CI81";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 29.3.	Se DEC(IND-STA INDICE) è maggiore di MESESUC effettuare le seguenti operazioni :                                                
        /// 29.3.1.	Se ((STATO(IND-STA) è uguale a  1, 2, 6, 7, 9, 10, 11, 16, 19, 22, 28, 32, 41 o 53) e DECAA(IND-STA INDICE) è uguale a  DATA-SYS-SSAA)                 continuare 
        /// l’elaborazione al punto successivo (29.4);                                        
        /// 29.3.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "35"  il campo  TIPO-ERRORE, con "DECORRENZA PRESTAZIONE 
        /// ESTERA  POSTERIORE A DATA ODIERNA" il campo MESSAGGIO-ERRORE, con 2 il campo COL-ERR1, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33); 
        /// </summary>
        /// <param name="decorrenzaImportoEstero"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriPosterioreADataOdierna(DateTime? decorrenzaImportoEstero, int? codiceStatoEE, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaImportoEstero.HasValue)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaImportoEstero.Value, Utility.DataSistemaCi.AddMonths(1)))
                {
                    if (!((new List<int> { 1, 2, 6, 7, 9, 10, 11, 16, 19, 22, 28, 32, 41, 53 }).Contains(codiceStatoEE.GetValueOrDefault()) && decorrenzaImportoEstero.Value.Year == Utility.DataSistemaCi.Year))
                    {
                        messaggioVideo = "Decorrenza Prestazione Estera posteriore a data odierna";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 29.4.	Se DEC(IND-STA INDICE) è minore di IW1DEORIG : Valorizzare con "36"  il campo  TIPO-ERRORE, con "DECORRENZA PRESTAZIONE ESTERA  ANTERIORE A DEC.ORIGINARIA"  il 
        /// campo MESSAGGIO-ERRORE, con 2 il campo COL-ERR1, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);                             
        /// </summary>
        /// <param name="decorrenzaImportoEstero"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriWithDecorrenzaOriginaria(DateTime? decorrenzaImportoEstero, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaImportoEstero.HasValue)
            {
                if (!Utility.DataSuccessivaA(decorrenzaImportoEstero.Value, decorrenzaOriginaria.Value))
                {
                    messaggioVideo = "Decorrenza Prestazione Estera anteriore a Decorrenza Originaria";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 29.5.	Se  DECMM(IND-STA INDICE) non è uguale a 1 e INDICE è maggiore di 1 effettuare le seguenti operazioni :                                                 
        /// ****  ci sono 2 controlli. uno per luX - SO ed uno per altri      
        /// 29.5.1.	Se (STATO(IND-STA) è uguale a 6 e  APPO-CAT1 è uguale a "S") e (CES(IND-STA INDICE-1) è uguale a zero) effettuare le seguenti operazioni:                                              
        /// ****  CONTR PER SO LUX (puo essere solo 3 mesi dopo dec pens ester
        /// 29.5.1.1.	Se   DECMM(IND-STA 1) è maggiore di 9,  CAMPO è uguale a  DEC(IND-STA 1)   +    91       
        /// 29.5.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :  CAMPO è uguale a DEC(IND-STA 1)  +  3        
        /// 29.5.1.3.	Se DEC(IND-STA INDICE) è uguale a CAMPO                         continuare l’elaborazione al punto successivo                                  
        /// 29.5.1.4.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "37"  il campo  TIPO-ERRORE, con "MESE DECORRENZA 
        /// ERRATO" il campo MESSAGGIO-ERRORE,  con 2 il campo COL-ERR1, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);  
        /// </summary>
        /// <param name="decorrenzaImportiEsteri"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="cessazioneImportiEsteriPrecedente"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="decorrenzaRicalcolo"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="listaResidenzeEstere"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaMeseDecorrenzaImportiEsteriPerLussemburgo(DateTime? decorrenzaImportiEsteri, Utility.TipoDomanda tipoDomanda, DateTime? cessazioneImportiEsteriPrecedente, int? codiceStatoEE, DateTime? primaDecorrenzaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaImportiEsteri.HasValue)
            {
                if (decorrenzaImportiEsteri.Value.Month != 1)
                {
                    if (codiceStatoEE.GetValueOrDefault() == 6 && tipoDomanda == Utility.TipoDomanda.Superstiti && !cessazioneImportiEsteriPrecedente.HasValue)
                    {
                        if (primaDecorrenzaImportiEsteri.Value.Month > 9)
                        {
                            if (!decorrenzaImportiEsteri.Equals(primaDecorrenzaImportiEsteri.Value.AddMonths(3)))
                            {
                                messaggioVideo = "Mese Decorrenza errato";
                                return false;
                            }
                        }
                    }
                    // IMPORTANTE: questa parte non la posso implementare perchè non so cosa è ICI2DADO

                    ////////////else
                    ////////////{
                    ////////////    if (!((codiceStatoEE.GetValueOrDefault() == 10 && tipoDomanda == Utility.TipoDomanda.Reversibilita) || cessazioneImportiEsteriPrecedente.HasValue || 
                    ////////////        decorrenzaRicalcolo.Equals(decorrenzaImportiEsteri) || (codiceConvenzione.GetValueOrDefault() == 9 && codiceStatoEE.GetValueOrDefault() == 9 && decorrenzaImportiEsteri.Value.Year == 1983 && decorrenzaImportiEsteri.Value.Month == 7) ||
                    ////////////        (codiceConvenzione.GetValueOrDefault() == 12 && GetTipologiaStato(codiceStatoEE.GetValueOrDefault()) == 'S' && decorrenzaImportiEsteri.Value.Year == 1972 && decorrenzaImportiEsteri.Value.Month == 10)))
                    ////////////    {
                    ////////////        DateTime? appDecorrenza = decorrenzaImportiEsteri;
                    ////////////        char? statoResidenza = GetStatoResidenzaByImportiEsteri(ref appDecorrenza, listaResidenzeEstere, codiceComuneResidenza);

                    ////////////    }
                    ////////////}
                }
            }

            return true;
        }

        /// <summary>
        /// 30.	Se IMPEST (IND-STA  INDICE) è maggiore di zero effettuare le seguenti operazioni :        
        /// 30.1.	Se DEC (IND-STA INDICE) è minore di 1 valorizzare con "41"  il campo  TIPO-ERRORE, con "INCOMPATIBILITA' TRA IMPORTO E DECORRENZA"  il campo MESSAGGIO-ERRORE, 
        /// con 2 il campo COL-ERR1, con 3 il campo  COL-ERR2 con 1 il campo        FLAG-ERR, uscire da CONTROLLI-1 (33);                          
        /// 30.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :                        
        /// 30.2.1.	Se DEC (IND-STA INDICE) maggiore di zero : Valorizzare, con "42"  il campo  TIPO-ERRORE, con "INCOMPATIBILITA' TRA IMPORTO E DECORRENZA" il campo 
        /// MESSAGGIO-ERRORE, con 3 il campo COL-ERR1, con 1 il campo   FLAG-ERR,  uscire da CONTROLLI-1 (33)                          
        /// </summary>
        /// <param name="importoPrestazioneEE"></param>
        /// <param name="decorrenzaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCompatibilitaImportoWithDecorrenza(decimal? importoPrestazioneEE, DateTime? decorrenzaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (importoPrestazioneEE.GetValueOrDefault() > 0)
            {
                if (!decorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Incompatibilita' tra Importo e Decorrenza";
                    return false;
                }
            }
            else
            {
                if (decorrenzaImportiEsteri.HasValue)
                {
                    messaggioVideo = "Incompatibilita' tra Importo e Decorrenza";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 31.	Se CES (IND-STA  INDICE) maggiore di zero effettuare le seguenti operazioni :        
        /// 31.2.	Se CES(IND-STA INDICE) non è maggiore di DEC(IND-STA INDICE)  : Valorizzare con "52"  il campo  TIPO-ERRORE, con "CESSAZIONE PRESTAZIONE ESTERA NON  POSTERIORE A 
        /// DECORRENZA" il campo MESSAGGIO-ERRORE, con 5 il campo         COL-ERR1, con 1 il campo FLAG-ERR, uscire da CONTROLLI-1 (33);
        /// 31.4.	Se  DECAA(IND-STA INDICE) non è uguale a CESAA(IND-STA INDICE) e CES(IND-STA INDICE) è maggiore del 199601  effettuare le seguenti operazioni :    
        /// 31.4.1.	Se  CESAA(IND-STA INDICE)  è uguale a   (DECAA(IND-STA INDICE)   +   1)                 e CESMM(IND-STA INDICE)  è uguale a  1 continuare l’elaborazione al punto 
        /// successivo (32);
        /// 31.4.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :  Valorizzare con "54"  il campo  TIPO-ERRORE, con "ANNO CESSAZIONE 
        /// DIVERSO DA ANNO DECORRENZA " il campo MESSAGGIO-ERRORE, con 5 il campo COL-ERR1, con 1 il campo FLAG-ERR, uscire da CONTROLLI-1 (33);
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="cessazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCoerenzaDecorrenzaCessazione(DateTime? decorrenza, DateTime? cessazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (cessazione.HasValue)
            {
                if (!Utility.DataSuccessivaA(cessazione.Value, decorrenza.Value))
                {
                    messaggioVideo = "Cessazione Prestazione Estera non posteriore a Decorrenza";
                    return false;
                }

                if (decorrenza.Value.Year != cessazione.Value.Year && Utility.DataStrettamenteSuccessivaA(cessazione.Value, new DateTime(1996, 01, 31)))
                {
                    if (!(cessazione.Value.Year == decorrenza.Value.Year + 1 && cessazione.Value.Month == 1))
                    {
                        messaggioVideo = "Anno Cessazione diverso da anno Decorrenza";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 34.	Se RICALSTATO (IND-STA) maggiore di zero effettuare le seguenti operazioni :       
        /// 34.1.	Se OK-RICALCOLO non è uguale a "S" : Valorizzare con 1 il campo INDICE, con "15"  il campo  TIPO-ERRORE, con "INCOMPATIBILITA' TRA DATA RICALCOLO E DECORRENZE" 
        /// il campo MESSAGGIO-ERRORE con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);                           
        /// 34.2.	Se RICALSTATO (IND-STA) non è maggiore di DEC(IND-STA 1) : Valorizzare con 1 il campo INDICE, con "15"  il campo  TIPO-ERRORE, con "DATA RICALCOLO NON POSTERIORE 
        /// AD ENTRATA STATO ESTERO" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// </summary>
        /// <param name="decorrenzaRicalcolo"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDataRicalcolo(DateTime? decorrenzaRicalcolo, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool ricalcoloOK = false;

            if (decorrenzaRicalcolo.HasValue)
            {
                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                {
                    foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importoEstero in listaImportiEsteri)
                    {
                        if (importoEstero.DecorrenzaPrestazioneEE.Equals(decorrenzaRicalcolo))
                        {
                            ricalcoloOK = true;
                            break;
                        }
                    }

                    if (!ricalcoloOK)
                    {
                        messaggioVideo = "Incompatibilita' tra Data Ricalcolo e Decorrenze";
                        return false;
                    }

                    if (!Utility.DataStrettamenteSuccessivaA(decorrenzaRicalcolo.Value, listaImportiEsteri[0].DecorrenzaPrestazioneEE.Value))
                    {
                        messaggioVideo = "Data Ricalcolo non posteriore ad entrata Stato Estero";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 35.	Se  (ICI2CONV  è uguale a 9  e STATO (IND-STA) è uguale a 9) e (DEC (IND-STA  1) è maggiore di zero e minore di 198307)effettuare le seguenti operazioni :       
        /// 35.1.	Se TP1NUA maggiore di 51 e DEC0783 non è uguale a "S" effettuare le seguenti operazioni : Valorizzare con 1 il campo INDICE, con "75"  il campo  TIPO-ERRORE, con 
        /// "CONVENZIONE 09 (AUSTRIA): MANCA DECORRENZA AL 07/83"  il campo MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 36.	Se  (ICI2CONV è uguale a 14 e STATO (IND-STA) è uguale a 14)  e (DEC (IND-STA  1) è maggiore di zero e minore di 198401)  effettuare le seguenti operazioni :       
        /// 36.1.	Se  TP1NUA maggiore di 51 e DEC0184 non è uguale a "S" effettuare le seguenti operazioni :     
        /// 36.1.1.	Se  IDAPLIQ (IND-STA) IS NUMERIC e IDAPLIQ (IND-STA) maggiore di zero e minore di 198401 continuare l’elaborazione al punto successivo (37);
        /// 36.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare, con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "CONVENZIONE 14 (ARGENTINA): MANCA DECORRENZA AL 01/84" il campo                  MESSAGGIO-ERRORE con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// **************  SVIZZERA - OPZIONE - PRIMA LIQUIDATE                                     
        /// 37.	Se  (ICI2CONV è uguale a 17) e (DEC (IND-STA  1) è maggiore di zero e minore del 200201)                        
        ///           effettuare le seguenti operazioni :       
        /// 37.1.	Valorizzare con ULTIMA-RIGA(IND-STA)  il campo INDX;
        /// 37.2.	Se (CES (IND-STA  INDX)  è uguale a  zero o maggiore del 200201)  e DEC0102 non è uguale a "S" effettuare le seguenti operazioni :     
        /// ******* SE RICOSTITUZIONE E NON OPZIONE, NON ERRORE                                      
        /// 37.2.1.	Se IW1CARIC è maggiore di 1 e IW1DEOP è uguale a zero continuare l’elaborazione al punto successivo (38);
        /// 37.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "75"  il campo  TIPO-ERRORE, con 
        /// "CONVENZIONE 17 (SVIZZERA): MANCA DECORRENZA AL 01/02" il campo MESSAGGIO-ERRORE                con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 39.	Se  DECRIAN è maggiore di zero e (DEC (IND-STA  1) è maggiore di zero e minore di DECRIAN) effettuare le seguenti operazioni :       
        /// 39.1.	Se FLGRIAN non è uguale a "S" : Valorizzare con 1 il campo INDICE, con "75"  il campo  TIPO-ERRORE, con "MANCA DECORRENZA AL "  +  DECRIAN-M  +  "/"  +  DECRIAN-A  
        /// + "  (RIENTRATO IN ITALIA IL " + DATRIEN(5:2)  +  "/"   +  DATRIEN(1:4)  +  ")" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 40.	Se  DATRIEN è maggiore di zero e ( (DEC (IND-STA  1) è maggiore di zero e minore di DATRIEN) )  e (ICI2CONV è uguale a 12)  effettuare le seguenti operazioni :       
        /// 40.1.	Se   FLGRIEN non è uguale a "S" : Valorizzare con 1 il campo INDICE, con "75"  il campo  TIPO-ERRORE, con "MANCA DECORRENZA ALLA DATA DI RIENTRO IN ITALIA ( "  + 
        /// DATRIEN(5:2)  +  "/"  + DATRIEN(1:4)  +  ")"  il campo MESSAGGIO-ERRORE con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);                             
        /// 42.	Se  (ICI2CONV è uguale a 14 e STATO (IND-STA)  è uguale a 14) e (DEC (IND-STA  1) è  maggiore di zero e minore di 198401) effettuare le seguenti operazioni :       
        /// 42.1.	Se TP1NUA maggiore di 51 e DEC0184 non è uguale a "S" effettuare le seguenti operazioni :     
        /// 42.1.1.	Se  IDAPLIQ (IND-STA) è numerico e IDAPLIQ (IND-STA) è maggiore di zero e minore di 198401 continuare l’elaborazione al punto successivo (43);
        /// 42.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con 
        /// "CONVENZIONE 14 (ARGENTINA): MANCA DECORRENZA AL 01/84"  il campo             MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);                       
        /// 43.	Se  (ICI2CONV  è uguale 17 e IW1DEOP è maggiore di zero)  e (DEC (IND-STA  1) è maggiore di zero e minore di IW1DEOP)                       
        /// **********  ULTIMO-IND     OCCURS 4 TIMES PIC 99.                                        
        ///           effettuare le seguenti operazioni :       
        /// 43.1.	Se  DEC-OPZ  non è uguale a "S" :  Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con "CONVENZIONE 17 (SVIZZERA): MANCA IMPORTO ALLA DATA 
        /// OPZIONE ("   +   IW1DEOPM  +  "/"   +   IW1DEOPA   +  ")"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 44.	Se ((ICI2CONV  è uguale a 13 e STATO(1)  è uguale a 38 e IW1DEOP è maggiore di zero) e (DEC (IND-STA  1) è maggiore di zero e minore di IW1DEOP))  oppure ((ICI2CONV 
        /// è uguale a  38  e IW1DEOP è maggiore di zero )   e (DEC (IND-STA  1) maggiore di zero e minore di IW1DEOP))                
        /// **********  ULTIMO-IND     OCCURS 4 TIMES PIC 99.                                        
        ///           effettuare le seguenti operazioni :       
        /// 44.1.	Se  DEC-OPZ  non è uguale a "S" : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con "CONVENZIONE 38 (SLOVENIA): MANCA IMPORTO ALLA DATA 
        /// OPZIONE ("   +   IW1DEOPM   +   "/"   +   IW1DEOPA   +   ")"  il campo MESSAGGIO-ERRORE,  con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 45.	Se  ( ICI2CONV = 33 e ICI2RESEST = "I  " )  e (DEC (IND-STA  1) è maggiore di zero e minore di 200001)  effettuare le seguenti operazioni :       
        /// 45.1.	Se  DEC2000 non è uguale a "S" : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con "CONV.33 (AUSTRALIA) / RESID. IN ITALIA: MANCA LA 
        /// DECORRENZA AL 01/2000" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);               
        /// 46.	Se  (ICI2CONV non è uguale a 38  e STATO (IND-STA) è uguale a 38) e (IW1DEORIG è maggire o uguale al 200208) e (SETT1(IND-STA) è uguale a zero e IND-STA è uguale a 1 
        /// ) : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con "COD.CONVENZIONE INCOMPATIBILE CON STATO SLOVENIA (CONV = 38)" il campo MESSAGGIO-ERRORE, con 
        /// 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 47.	Se  (ICI2CONV non è uguale a 39  e STATO (IND-STA) è uguale a 39) e (IW1DEORIG è maggiore o uguale a 200311)  e (SETT1(IND-STA) è uguale a zero e IND-STA è uguale a 
        /// 1) : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con "COD.CONVENZIONE INCOMPATIBILE CON STATO CROAZIA (CONV = 39)" il campo MESSAGGIO-ERRORE, con 
        /// 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);                               
        /// </summary>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="settimane"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaObbligatorietaDecorrenzaImportiEsteri(DateTime? primaDecorrenzaImportiEsteri, byte? codiceConvenzione, int? codiceStatoEE, int? settimane,
            DateTime? decorrenzaLiquidazioneStatoEE, DateTime? ultimaCessazioneImportiEsteri, byte? causaCarico, DateTime? decorrenzaOpzione, DateTime? ultimaDecorrenzaResidenzaItaliana,
            bool isDecorrenzaResidenzaItaliaOK, int? primoCodiceStatoEE, bool dec_Opz, string codiceComuneResidenza, bool dec2000, DateTime? decorrenzaOriginaria, int? contributiEEDecorrenzaOriginaria,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 9 && codiceStatoEE.GetValueOrDefault() == 9 && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(1983, 07, 01)))
            {
                if (settimane.GetValueOrDefault() > 51 && !primaDecorrenzaImportiEsteri.Equals(new DateTime(1983, 07, 01)))
                {
                    messaggioVideo = "Convenzione 09 (Austria): manca Decorrenza al 07/83";
                    return false;
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 14 && codiceStatoEE.GetValueOrDefault() == 14 && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(1984, 01, 01)))
            {
                if (settimane.GetValueOrDefault() > 51 && !primaDecorrenzaImportiEsteri.Equals(new DateTime(1984, 01, 01)))
                {
                    if (!decorrenzaLiquidazioneStatoEE.HasValue || Utility.DataSuccessivaA(decorrenzaLiquidazioneStatoEE.Value, new DateTime(1984, 01, 01)))
                    {
                        messaggioVideo = "Convenzione 14 (Argentina): manca Decorrenza al 01/84";
                        return false;
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 17 && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(2002, 01, 01)))
            {
                if ((!ultimaCessazioneImportiEsteri.HasValue || Utility.DataStrettamenteSuccessivaA(ultimaCessazioneImportiEsteri.Value, new DateTime(2002, 01, 01))) && !ultimaCessazioneImportiEsteri.Equals(new DateTime(2002, 01, 01)))
                {
                    if (!(causaCarico.GetValueOrDefault() > 1 && !decorrenzaOpzione.HasValue))
                    {
                        messaggioVideo = "Convenzione 17 (Svizzera): manca Cessazione al 01/2002";
                        return false;
                    }
                }
            }


            if (primaDecorrenzaImportiEsteri.HasValue &&
                ((codiceConvenzione.GetValueOrDefault() == 38 && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(2004, 01, 01))) ||
                (codiceConvenzione.GetValueOrDefault() == 13 && primoCodiceStatoEE.GetValueOrDefault() == 38) &&
                (!Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(2004, 01, 01)))))
            {
                if ((!ultimaCessazioneImportiEsteri.HasValue || Utility.DataStrettamenteSuccessivaA(ultimaCessazioneImportiEsteri.Value, new DateTime(2004, 01, 01))) && !ultimaCessazioneImportiEsteri.Equals(new DateTime(2004, 01, 01)))
                {
                    if (causaCarico.GetValueOrDefault() <= 1)
                    {
                        messaggioVideo = "Convenzione Slovenia: manca Decorrenza al 01/2004";
                        return false;
                    }
                }
            }

            if (ultimaDecorrenzaResidenzaItaliana.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, ultimaDecorrenzaResidenzaItaliana.Value))
            {
                if (!isDecorrenzaResidenzaItaliaOK)
                {
                    messaggioVideo = "Manca Decorrenza al " + String.Format("{0}:MM/yyyy", ultimaDecorrenzaResidenzaItaliana) + " (Rientrato in Italia il " + String.Format("{0}:MM/yyyy", ultimaDecorrenzaResidenzaItaliana) + ")";
                    return false;
                }
            }

            if (ultimaDecorrenzaResidenzaItaliana.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, ultimaDecorrenzaResidenzaItaliana.Value) && codiceConvenzione.GetValueOrDefault() == 12)
            {
                if (!isDecorrenzaResidenzaItaliaOK)
                {
                    messaggioVideo = "Manca Decorrenza alla data di rientro in Italia (" + String.Format("{0:MM/yyyy}", ultimaDecorrenzaResidenzaItaliana) + ")";
                    return false;
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 14 && codiceStatoEE.GetValueOrDefault() == 14 && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(1984, 01, 01)))
            {
                if (settimane.GetValueOrDefault() > 51 && !primaDecorrenzaImportiEsteri.Equals(new DateTime(1984, 01, 01)))
                {
                    if (!(decorrenzaLiquidazioneStatoEE.HasValue && !Utility.DataSuccessivaA(decorrenzaLiquidazioneStatoEE.Value, new DateTime(1984, 01, 01))))
                    {
                        messaggioVideo = "Convenzione 14 (Argentina): manca Decorrenza al 01/84";
                        return false;
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 17 && decorrenzaOpzione.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, decorrenzaOpzione.Value))
            {
                if (!dec_Opz)
                {
                    messaggioVideo = "Convenzione 17 (Svizzera): manca importo alla data opzione (" + String.Format("{0:MM/yyyy}", decorrenzaOpzione) + ")";
                    return false;
                }
            }

            if ((codiceConvenzione.GetValueOrDefault() == 13 && primoCodiceStatoEE.GetValueOrDefault() == 38 && decorrenzaOpzione.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, decorrenzaOpzione.Value)) || (codiceConvenzione.GetValueOrDefault() == 38 && decorrenzaOpzione.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, decorrenzaOpzione.Value)))
            {
                if (!dec_Opz)
                {
                    messaggioVideo = "Convenzione 38 (Slovenia): manca importo alla data opzione (" + String.Format("{0:MM/yyyy}", decorrenzaOpzione) + ")";
                    return false;
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 33 && !codiceComuneResidenza.StartsWith("Z") && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, new DateTime(2000, 01, 01)))
            {
                if (!dec2000)
                {
                    messaggioVideo = "Convenienza 33 (Australia) / Resid. in Italia: manca la Decorrenza al 01/2000";
                    return false;
                }
            }

            if (codiceConvenzione.GetValueOrDefault() != 38 && codiceStatoEE.GetValueOrDefault() == 38 && decorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 08, 01)) && contributiEEDecorrenzaOriginaria.GetValueOrDefault() == 0 && primoCodiceStatoEE == codiceStatoEE)
            {
                messaggioVideo = "Cod.Convenzione incompatibile con Stato Slovenia (Conv = 38)";
                return false;
            }

            if (codiceConvenzione.GetValueOrDefault() != 39 && codiceStatoEE.GetValueOrDefault() == 39 && decorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2003, 11, 01)) && contributiEEDecorrenzaOriginaria.GetValueOrDefault() == 0 && primoCodiceStatoEE == codiceStatoEE)
            {
                messaggioVideo = "Cod.Convenzione incompatibile con Stato Croazia (Conv = 39)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 41.	Se APPO-CAT1 non è uguale a "S" e  STATO (IND-STA) è uguale a 37 e                                 (COD-SOSP-ESTERO(IND-STA) è uguale a "N") e (SETTDIR(IND-STA) è 
        /// maggiore di 52 o SETT1(IND-STA) è maggiore di 52 )  e  (SETTDIR(IND-STA) è maggiore di 750 o SETT1(IND-STA) è maggiore di 750 ) effettuare le seguenti operazioni :       
        /// 41.1.	Se (DEC (IND-STA  1) ) è uguale a zero effettuare le seguenti operazioni :     
        /// 41.2.	Diversamente da quanto analizzato nel punto precedente (41.1) effettuare le seguenti operazioni : 
        /// 41.2.1.	Valorizzare con 1 il campo INDICE;
        /// 41.2.2.	Se (DEC (IND-STA  INDICE) ) è minore del 199602 effettuare le seguenti operazioni :  
        /// 41.2.2.1.	Ricercare nella tabella DEC (utilizzare un ciclo a conteggio) il primo valore maggiore di 199601 o uguale a zero, considerare come indice primario il valore 
        /// contenuto in IND-STA e come secondario il contatore INDICE impostandolo iniazialmente ad 1 con incremento progressivo di 1 fino ad un valore massimo di 51; se il valore 
        /// trovato è  uguale a zero valorizzare con 51 il contatore INDICE per uscire da ciclo di ricerca; 
        /// 41.2.3.	Se  INDICE è maggiore di 50 : Valorizzare  con "75"  il campo  TIPO-ERRORE                 , con "CONVENZIONE 37 (VENEZUELA): DECORRENZE POST-1995 MANCANTI" il 
        /// campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2                          
        /// 41.2.4.	Valorizzare con 2 il campo INDICE;
        /// 41.2.5.	Se DECAA(IND-STA INDICE) non è maggiore del 1996  e DECAA(IND-STA INDICE) è maggiore di zero effettuare le seguenti operazioni :  
        /// 41.2.5.1.	 Valorizzare con 3 il campo INDICE; 
        /// 41.2.5.2.	Se DECAA(IND-STA INDICE) non è maggiore del 1996 e   DECAA(IND-STA INDICE) è maggiore di zero: Valorizzare con 4 il campo INDICE;
        /// 41.2.6.	Effettuate un ciclo di lettura della tabella DEC  considerando come indice primario il valore contenuto in IND-STA e come secondario il contatore INDICE 
        /// (impostato nella fase precedente) incrementandolo progressivamente di 1, uscire dal ciclo quando INDICE diventa maggiore di 50 oppure il valore del’elemento delle tabella 
        /// è uguale a zero. Per ogni elemento letto effettuare le seguenti operazioni :
        /// 41.2.6.1.	Valorizzare INDICE-1  con INDICE e sottrarre 1;
        /// 41.2.6.2.	Valorizzare APPO-QUA con DECAA(IND-STA    INDICE-1)    e aggiungere   1                           
        /// 41.2.6.3.	Se DECAA(IND-STA INDICE) è uguale a DECAA(IND-STA   INDICE-1)  o DECAA(IND-STA INDICE)  è uguale a APPO-QUA effettuare le seguenti operazioni :
        /// 41.2.6.3.1.	Se DECMM(IND-STA INDICE) non è uguale a 1 valorizzare con DECAA(IND-STA   INDICE-1)  il  campo INDICE         
        /// 41.2.6.4.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con DECAA(IND-STA   INDICE-1)  il campo INDICE;               
        /// 41.2.7.	Se INDICE è maggiore del 1990 : Valorizzare con "75"  il campo  TIPO-ERRORE                 , con "CONVENZIONE 37 (VENEZUELA): DECORRENZE NON IN SEQUENZA - " +   INDICE(2:4) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// 41.3.	Valorizzare INDICE-1 con INDICE e sottrarre 1; 
        /// 41.4.	Se INDICE è minore di 51  e  ( DECAA(IND-STA INDICE-1)  è uguale a DATA-SYS-SSAA oppure CES(IND-STA INDICE-1) è maggiore di zero) continua l’elaborazione al 
        /// punto successivo (42);
        /// 41.5.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :  Valorizzare con "75"  il campo  TIPO-ERRORE, con "CONVENZIONE 37 
        /// (VENEZUELA): DECORRENZA ANNO IN CORSO MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// </summary>
        /// <param name="tipoDomanda"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="sospensioneCautelativaIntegrazione"></param>
        /// <param name="contributiEEDiritto"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlliVenezuela(Utility.TipoDomanda tipoDomanda, int? codiceStatoEE, char? sospensioneCautelativaIntegrazione, int? contributiEEDiritto, int? contributiEEDecorrenzaOriginaria, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (tipoDomanda != Utility.TipoDomanda.Superstiti && codiceStatoEE.GetValueOrDefault() == 37 && sospensioneCautelativaIntegrazione.GetValueOrDefault() == 'N' && (contributiEEDiritto.GetValueOrDefault() > 52 || contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 52) && (contributiEEDiritto.GetValueOrDefault() > 750 || contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 750))
            {
                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                {
                    if (listaImportiEsteri[0].DecorrenzaPrestazioneEE.HasValue)
                    {
                        int index = 0;
                        if (!Utility.DataSuccessivaA(listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value, new DateTime(1996, 02, 01)))
                        {
                            DateTime? decorrenzaPost1996 = listaImportiEsteri.FirstOrDefault(x => Utility.DataSuccessivaA(x.DecorrenzaPrestazioneEE.Value, new DateTime(1996, 02, 01))).DecorrenzaPrestazioneEE;
                            if (decorrenzaPost1996.Equals(DateTime.MinValue))
                            {
                                messaggioVideo = "Convenzione 37 (Venezuela): Decorrenza post-1995 mancanti";
                                return false;
                            }
                        }

                        if (listaImportiEsteri.Count > 1)
                        {
                            index = 1;
                            if (listaImportiEsteri[index].DecorrenzaPrestazioneEE.HasValue && listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Year <= 1996)
                            {
                                if (listaImportiEsteri.Count > 2)
                                {
                                    index = 2;
                                    if (listaImportiEsteri[index].DecorrenzaPrestazioneEE.HasValue && listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Year <= 1996)
                                    {
                                        index = 3;
                                    }
                                }
                            }
                        }

                        while (listaImportiEsteri.Count > index)
                        {
                            if (index > 0 && listaImportiEsteri[index].DecorrenzaPrestazioneEE.HasValue)
                            {
                                int annoDecorrenzaPrecedentePiu1 = listaImportiEsteri[index - 1].DecorrenzaPrestazioneEE.Value.Year + 1;
                                if (listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Year == listaImportiEsteri[index - 1].DecorrenzaPrestazioneEE.Value.Year ||
                                   listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Year == annoDecorrenzaPrecedentePiu1)
                                {
                                    if (listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Month != 1 &&
                                        !(listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Month == 3 && listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value.Year == 2018))
                                        index = listaImportiEsteri[index - 1].DecorrenzaPrestazioneEE.Value.Year;
                                }
                                else
                                {
                                    index = listaImportiEsteri[index - 1].DecorrenzaPrestazioneEE.Value.Year;
                                }
                            }
                            index++;
                        }

                        if (index > 1990)
                        {
                            messaggioVideo = "Convenzione 37 (Venezuela): Decorrenze non in sequenza";
                            return false;
                        }

                        if (!Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 12, 31)) &&
                           !listaImportiEsteri.Exists(x => x.DecorrenzaPrestazioneEE.Value.Month == 3 && x.DecorrenzaPrestazioneEE.Value.Year == 2018))
                        {
                            messaggioVideo = "Convenzione 37 (Venezuela): E' obbligatorio inserire un prorata con decorrenza 03/2018";
                            return false;
                        }

                        if (index < 51)
                        {
                            if (!(listaImportiEsteri[index - 1].DecorrenzaPrestazioneEE.Value.Year == dataSistema.Year || listaImportiEsteri[index - 1].CessazionePrestazioneEE.HasValue))
                            {
                                messaggioVideo = "Convenzione 37 (Venezuela): Decorrenza anno in corso mancante";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 48.	Ricercare nella tabella DEC (utilizzando un ciclo a conteggio) il primo valore maggiore di zero, considerare come indice primario il valore contenuto in IND-STA e 
        /// come secondario il contatore INDX impostandolo inizialmente a 50 con decremento progressivo di 1; se INDX è  inferiore a 2  terminare il ciclo di ricerca; 
        /// 49.	Se  DEC(IND-STA INDX) è maggiore di zero  e CES(IND-STA INDX) è uguale a zero effettuare le seguenti operazioni :       
        /// 49.1.	Se  APPO-CAT1 non è uguale a "S" e (ICODVIRT è uguale a "1" oppure "3" oppure "7") effettuare le seguenti operazioni :  Valorizzare con 1 il campo INDICE, con 
        /// "77" il campo  TIPO-ERRORE, con "PRESTAZIONE ESTERA INCOMPATIBILE CON COD.VIRT. ICODVIRT (PANN.CNV01)"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da 
        /// CONTROLLI-2 (55);
        /// </summary>
        /// <param name="decorrenzaImportiEsteri"></param>
        /// <param name="cessazioneImportiEsteri"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="codiceVirtuale"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriWithCodiceVirtuale(DateTime? decorrenzaImportiEsteri, DateTime? cessazioneImportiEsteri, Utility.TipoDomanda tipoDomanda, char? codiceVirtuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaImportiEsteri.HasValue && !cessazioneImportiEsteri.HasValue)
            {
                if (tipoDomanda != Utility.TipoDomanda.Superstiti && (codiceVirtuale.GetValueOrDefault() == 1 || codiceVirtuale.GetValueOrDefault() == 3 || codiceVirtuale.GetValueOrDefault() == 7))
                {
                    messaggioVideo = "Prestazione Estera incompatibile con Codice Virtuale " + codiceVirtuale.GetValueOrDefault();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 50.	Se  IW1CODOPZ è uguale a 7 e DEC(IND-STA  1)  è uguale a IW1DEORIG : Valorizzare con 1 il campo INDICE, con "75"  il campo  TIPO-ERRORE, con "PENSIONE CON COD.
        /// OPZIONE = 7: DEC.PRESTAZ.ESTERA ERRATA" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-2 (55);
        /// </summary>
        /// <param name="codiceOpzioneRiliquidazione"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaImportiEsteriWithCodiceOpzione(byte? codiceOpzioneRiliquidazione, DateTime? primaDecorrenzaImportiEsteri, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceOpzioneRiliquidazione.GetValueOrDefault() == 7 && primaDecorrenzaImportiEsteri.Equals(decorrenzaOriginaria))
            {
                messaggioVideo = "Pensione con Cod.Opzione = 7: Dec.Prestaz.Estera errata";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 51.	Valorizzare con  (TP1NUA + TP1NUB) il campo SET-DIR;
        /// 52.	Valorizzare con 999999 il campo DEC-108;                               
        /// 53.	Se IW1CODOPZ è uguale a 7 e SET-DIR è minore di 52 effettuare le seguenti operazioni : 
        /// 53.1.	Se STATO(1)  è uguale a  17 e STATO(2) è uguale a zero e ICODRES(1)  è uguale a 'I  ' e IW1DEORIG è maggiore di 200205 e ((IW1DEORIG minore di 200405 e IW1CATPEN 
        /// minore di 7) oppure (IW1DEORIG minore di 200408 e   IW1CATPEN maggiore di 6)) e APPO-CAT1  è uguale a 'V' effettuare le seguenti operazioni :    
        /// *** DEC-108 = DATA ACQUISITA 
        /// 53.1.1.	Se IW1DEOP maggiore di zero e IW1DEOP maggiore di IW1DEORIG : Valorizzare con IW1DEOP il campo DEC-108;
        /// *** DEC-108 = DATA DEC. QUOTA ESTERA 
        /// 53.1.2.	Se DEC(1, 1) è minore di DEC-108  e DEC(1, 1) è maggiore di zero : Valorizzare con DEC(1, 1)  il campo DEC-108;
        /// ***DEC-108 = DATA ETA' PENSIONABILE 
        /// 53.1.3.	Valorizzare con zero il campo CAMPO;                                   
        /// 53.1.4.	Se IW1SESTIT è uguale a 'F' valorizzare CAMPO-A con (IW1SECAN + 63) diversamente valorizzare CAMPO-A  con (IW1SECAN + 65);
        /// 53.1.5.	Valorizzare CAMPO-M con IW1NATITM;
        /// 53.1.6.	Se IW1SESTIT è uguale a 'F'  e CAMPO è maggiore di 200500  aggiungere 1 a CAMPO-A;
        /// 53.1.7.	Aggiungere 1 a CAMPO-M;
        /// 53.1.9.	Se CAMPO è minore di DEC-108  e CAMPO è maggiore di IW1DEORIG : Valorizzare con CAMPO il campo DEC-108:                
        /// *** DEC-108 = DATA TRASFERIMENTO RESIDENZA 
        /// 53.1.10.	Effettuate la lettura della tabella IDECRES (utilizzando un ciclo a conteggio), considerare come indice il contatore I1 impostandolo inizialmente a 1 con 
        /// incremento progressivo di 1; se I1  è maggiore di 5  oppure se IDECRES(I1)  è uguale a zero terminare il ciclo di ricerca; per ogni occorrenza letta effettuare i seguenti
        /// controlli :
        /// 53.1.10.1.	Se ICODRES(I1) non è uguale a 'I  ' e IDECRES(I1) è maggiore di IW1DEORIG e IDECRES(I1) è minore di DEC-108 valorizzare con IDECRES(I1) il campo  DEC-108
        /// 53.2.	Diversamente da quanto analizzato nel punto precedente (53.1) : Valorizzare con 999999 il campo DEC-108                                   
        /// 54.	Se DEC-108 non è maggiore del valore costituito dai primi 6 caratteri contenuti nel campo       DATA-GIORNO  effettuare le seguenti operazioni :      
        /// 54.1.	Se TP1ELIM è uguale a zero oppure se TP1ELIM è maggiore di DEC-108 : Valorizzare  con 1 il campo INDICE, con '75'  il campo  TIPO-ERRORE, con 'PENS. SVIZZERA - 
        /// COD.OPZIONE = 7  DA ELIMINARE'  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR;
        /// </summary>
        /// <param name="settimane"></param>
        /// <param name="codiceOpzioneRiliquidazione"></param>
        /// <param name="primoCodiceStatoEE"></param>
        /// <param name="secondoCodiceStatoEE"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="categoria"></param>
        /// <param name="gruppo"></param>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="listaResidenzeEstere"></param>
        /// <param name="decorrenzaEliminazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlliSvizzera(int? settimane, byte? codiceOpzioneRiliquidazione, int? primoCodiceStatoEE, int? secondoCodiceStatoEE, DateTime? decorrenzaOriginaria, int categoria,
            string gruppo, DateTime? decorrenzaOpzione, DateTime? primaDecorrenzaImportiEsteri, char? sessoTitolare, DateTime? dataNascitaTitolare,
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, DateTime? decorrenzaEliminazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? dec_108 = DateTime.MaxValue;
            DateTime? dataEtaPensionabile;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (codiceOpzioneRiliquidazione.GetValueOrDefault() == 7 && settimane.GetValueOrDefault() < 52)
            {
                if (primoCodiceStatoEE.GetValueOrDefault() == 17 && secondoCodiceStatoEE.GetValueOrDefault() == 0 && listaResidenzeEstere != null && listaResidenzeEstere.Count > 0 && listaResidenzeEstere.First().CodCatastaleStatoEE.Equals("Z000") && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 05, 31)) && ((!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 05, 01)) && categoria < 7) || (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 08, 01)) && categoria > 6)) && gruppo.Equals("0001"))
                {
                    if (decorrenzaOpzione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaOpzione.Value, decorrenzaOriginaria.Value))
                        dec_108 = decorrenzaOpzione;
                    if (primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(primaDecorrenzaImportiEsteri.Value, dec_108.Value))
                        dec_108 = primaDecorrenzaImportiEsteri;

                    if (sessoTitolare.GetValueOrDefault() == 'F')
                        dataEtaPensionabile = dataNascitaTitolare.Value.AddYears(63);
                    else
                        dataEtaPensionabile = dataNascitaTitolare.Value.AddYears(65);

                    if (sessoTitolare.GetValueOrDefault() == 'F' && Utility.DataSuccessivaA(dataEtaPensionabile.Value, new DateTime(2005, 01, 01)))
                        dataEtaPensionabile = dataEtaPensionabile.Value.AddYears(1);
                    dataEtaPensionabile = dataEtaPensionabile.Value.AddMonths(1);

                    if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
                    {
                        foreach (GestioneAnagrafica.DatiResidenzaEstero residenza in listaResidenzeEstere)
                        {
                            if (!residenza.CodCatastaleStatoEE.Equals("Z000") && Utility.DataSuccessivaA(residenza.Decorrenza.Value, decorrenzaOriginaria.Value) && !Utility.DataSuccessivaA(residenza.Decorrenza.Value, dec_108.Value))
                                dec_108 = residenza.Decorrenza;
                        }
                    }
                }
                else
                {
                    dec_108 = DateTime.MaxValue;
                }
            }

            if (!Utility.DataStrettamenteSuccessivaA(new DateTime(dec_108.Value.Year, dec_108.Value.Month, 01), new DateTime(dataSistema.Year, dataSistema.Month, 01)))
            {
                if (!decorrenzaEliminazione.HasValue || Utility.DataStrettamenteSuccessivaA(decorrenzaEliminazione.Value, dec_108.Value))
                {
                    messaggioVideo = "Pens. Svizzera - Cod.Opzione = 7 da eliminare";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 56.	Se  ICI2CONV è uguale a 12 e ( STATO(1) è uguale a 38  oppure STATO(2) è uguale a 38  oppure STATO(3) è uguale a 38  oppure STATO(4) è uguale a 38) effettuare le 
        /// seguenti operazioni :                                                       
        /// 56.1.	Se SETT1(2) è maggiore di zero effettuare le seguenti operazioni :                                                    
        /// 56.1.1.	Se (STATO(2)  è uguale a 1 o 2 o 4 o 6 o 7 o 9 o 10  o 11 o 18 o 19 o 20 o 28 o 32 o 38 o 41 o 44 o 45 o 46 o 47 o 48 o 49 o 50 o 51 o 52 o 54 o 55 o 56 o 57 o 
        /// 24 o 25 o 39 o 43 o 29 o 17 ) o (STATO(2) è uguale a 42 e  IW1DEORIG è maggiore di 200806)  o (STATO(2)  è uguale a 13 e  IW1DEORIG è maggiore di 201010)  continuare 
        /// l’elaborazione al punto successivo (56.2) ;                             
        /// 56.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, con 
        /// "STATO 38(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "  +  STATO(2) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);  
        /// 56.2.	Se SETT1(3) è maggiore di zero effettuare le seguenti operazioni :  
        /// 56.2.1.	Se (STATO(3)  è uguale a  1 o  2 o  4 o  6 o  7 o  9 o 10 o 11 o 18 o 19 o 20 o 28 o 32 o 38 o 41 o 44 o 45 o 46 o 47 o 48 o 49 o 50 o 51 o 52 o 54 o 55 o 56 o 
        /// 57 o 24 o 25 o 39 o 43 o 29 o 17)  o (STATO(3)  è uguale a 42 e  IW1DEORIG è maggiore di 200806) o (STATO(3) è uguale a 13 e IW1DEORIG è maggiore di 201010)  continuare 
        /// l’elaborazione al punto successivo  (56.3);
        /// 56.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE,  con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 38(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "  +  STATO(3) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);  
        /// 56.3.	Se SETT1(4) è maggiore di zero effettuare le seguenti operazioni;
        /// 56.3.1.	Se (STATO(4)  è uguale a 1 o 2 o 4 o 6 o 7 o 9 o 10 o 11 o 18 o 19 o 20 o 28 o 32 o 38  o 41 o 44 o 45 o 46 o 47 o 48 o 49  o 50 o 51 o 52 o 54 o 55 o 56 o 57 o 
        /// 24 o 25 o 39 o 43 o 29 o 17) o (STATO(4)  è uguale a 42 e  IW1DEORIG è maggiore di 200806) o (STATO(4) è uguale a 13 e  IW1DEORIG è maggiore di 201010) continuare 
        /// l’elaborazione al punto successivo  (57);
        /// 56.3.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 38(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "  +  STATO(4)  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);
        /// 57.	Se  ICI2CONV è uguale a 12 e ( (STATO(1)  è uguale a 29 oppure 40) oppure  (STATO(2) è uguale  29 oppure 40) oppure (STATO(3) è uguale  29 oppure 40) oppure (STATO(4)
        /// è uguale  29 oppure 40))  effettuare le seguenti operazioni :                                                       
        /// 57.1.	Se SETT1(2) è maggiore di zero effettuare le seguenti operazioni :  
        /// 57.1.1.	Se (STATO(2) è uguale a 1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10  oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38  
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49  oppure 50 oppure 51 oppure 52  oppure 29 oppure 40)  continuare l’elaborazione al punto successivo 
        /// (57.2);          
        /// 57.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// Valorizzare con "STATO 29/40  INCOMPATIBILE CON SETTIMANE STATO "  +  STATO(2)  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                             
        /// 57.2.	Se SETT1(3) è maggiore di zero effettuare le seguenti operazioni :  
        /// 57.2.1.	Se (STATO(3) è uguale a 1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52  oppure 29 oppure 40)            continuare l’elaborazione al punto 
        /// successivo  (57.3);        
        /// 57.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 29/40  INCOMPATIBILE CON SETTIMANE STATO "    +   STATO(3)  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                             
        /// 57.3.	Se   SETT1(4) è maggiore di zero effettuare le seguenti operazioni : 
        /// 57.3.1.	Se (STATO(4)  è uguale a 1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52 oppure 29 oppure 40)             continuare l’elaborazione al punto 
        /// successivo  (57.4);
        /// 57.3.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 29/40 INCOMPATIBILE CON SETTIMANE STATO "  +   STATO(4)  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);  
        /// 58.	Se  ICI2CONV  è uguale a 12 e  ( (STATO(1) è uguale a 27  ) oppure (STATO(2) è uguale a 27  ) oppure (STATO(3) è uguale a 27  ) oppure  (STATO(4) è uguale a 27  ))  
        /// effettuare le seguenti operazioni :                                                       
        /// 58.1.	Se   SETT1(2) è maggiore di zero effettuare le seguenti operazioni :                                                    
        /// 58.1.1.	Se (STATO(2) è uguale a  1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10  oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52  oppure 27 oppure 17 )           continuare l’elaborazione al punto 
        /// successivo  (58.2);       
        /// 58.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 27 INCOMPATIBILE CON SETTIMANE STATO "    +   STATO(2) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                             
        /// 58.2.	Se SETT1(3) è maggiore di zero effettuare le seguenti operazioni :                                                    
        /// 58.2.1.	Se (STATO(3)  è uguale a  1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10  oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52 oppure 27 oppure 17)             continuare l’elaborazione al punto 
        /// successivo (58.3);    
        /// 58.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 27 INCOMPATIBILE CON SETTIMANE STATO "    +   STATO(3) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                             
        /// 58.3.	Se SETT1(4) è maggiore di zero effettuare le seguenti operazioni : 
        /// 58.3.1.	Se (STATO(4) è uguale a 1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 
        /// oppure 41 oppure 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49  oppure 50 oppure 51 oppure 52 oppure 27 oppure 17) continuare l’elaborazione al punto successivo  
        /// (59);
        /// 58.3.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 27 INCOMPATIBILE CON SETTIMANE STATO "    +  STATO(4) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88); 
        /// 59.	Se  ICI2CONV  è uguale a 12  e  ( STATO(1) è uguale a 17 oppure STATO(2) è uguale a 17 oppure STATO(3) è uguale a 17 oppure STATO(4) è uguale a 17) effettuare le 
        /// seguenti operazioni :                                                       
        /// 59.1.	Se SETT1(2) è maggiore di zero effettuare le seguenti operazioni :  
        /// 59.1.1.	Se ((STATO(2)  è uguale a  1 oppure 2 oppure 4 oppure 6 oppure 7 oppure 9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 oppure 
        /// 41) e  IW1DEORIG è maggiore di 200205) oppure ((STATO(2) è uguale a 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52 ) e  IW1DEORIG è 
        /// maggiore di 200603)          oppure ((STATO(2) è uguale a 54 oppure 55 ) e  IW1DEORIG è maggiore di 200905) oppure ((STATO(2)  è uguale a 38 ) e  IW1DEORIG è maggiore di 
        /// 200405) oppure STATO(2) è uguale a 17 oppure 13 oppure 22 oppure 23 oppure 24 oppure 25 oppure 36 oppure 38 oppure 39 oppure 42 oppure 43 oppure 27 oppure 26 oppure 56 
        /// oppure 57  continuare l’elaborazione al punto successivo  (59.2);                                           
        /// 59.1.2.	Diversamente da quanto analizzato nel punto precedente effettuare i seguenti controlli;
        /// 59.1.2.1.	Se IW1DEORIG è minore di 200206  e  ( (STATO(1)  è uguale a 17 e  SETT1(1)  è uguale a zero) oppure  (STATO(2) è uguale a 17 e  SETT1(2) è uguale a zero) 
        /// oppure  (STATO(3) è uguale a 17 e  SETT1(3) è uguale a zero) oppure  (STATO(4) è uguale a 17 e  SETT1(4) è uguale a zero)) continuare l’elaborazione al punto successivo  
        /// (59.2);                
        /// 59.1.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo                      
        /// TIPO-ERRORE, con "STATO 17(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "    +     STATO(2) il campo                MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da 
        /// CONTROLLI-3 (88);                           
        /// 59.2.	Se SETT1(3) è maggiore di zero effettuare le seguenti operazioni : 
        /// 59.2.1.	Se ((STATO(3) è uguale a 1 oppure 2 oppure 4 oppure 6 oppure 7 oppure 9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38 oppure 41)
        /// e  IW1DEORIG è maggiore di 200205)  oppure ((STATO(3) è uguale a 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52) e IW1DEORIG è 
        /// maggiore di 200603) oppure ((STATO(3) è uguale 54 oppure 55 ) e  IW1DEORIG è maggiore di 200905) oppure ((STATO(3) è uguale a 38 ) e  IW1DEORIG è maggiore di 200405) 
        /// oppure STATO(3) è uguale a 17 oppure 13 oppure 22 oppure 23 oppure 24 oppure 25 oppure 36 oppure 38 oppure 39 oppure 42 oppure 43 oppure 27 oppure 26 oppure 56 oppure 57 
        /// continuare l’elaborazione al punto successivo  (59.3);
        /// 59.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni :
        /// 59.2.2.1.	Se IW1DEORIG è minore di 200206 e  ((STATO(1)  è uguale a 17 e  SETT1(1) è uguale a zero) oppure  (STATO(2)  è uguale a 17 e  SETT1(2) è uguale a zero) 
        /// oppure  (STATO(3) è uguale a 17 e  SETT1(3) è uguale a zero) oppure  (STATO(4) è uguale a  17 e  SETT1(4) è uguale a zero)) continuare l’elaborazione al punto successivo 
        /// (59.3);              
        /// 59.2.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo  TIPO-ERRORE, 
        /// con "STATO 17(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "    +    STATO(3)  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                           
        /// 59.3.	Se SETT1(4) è maggiore di zero effettuare le seguenti operazioni :  
        /// 59.3.1.	Se ((STATO(4)  è uguale a 1 oppure  2 oppure  4 oppure  6 oppure  7 oppure  9 oppure 10 oppure 11 oppure 18 oppure 19 oppure 20 oppure 28 oppure 32 oppure 38  
        /// oppure 41) e  IW1DEORIG è maggiore di 200205) oppure ((STATO(4)  è uguale a 44 oppure 45 oppure 46 oppure 47 oppure 48 oppure 49 oppure 50 oppure 51 oppure 52 ) e  
        /// IW1DEORIG è maggiore di 200603) oppure ((STATO(4) è uguale a 54 oppure 55 ) e  IW1DEORIG è maggiore di 200905)  oppure ((STATO(4) è uguale a 38 ) e  IW1DEORIG è maggiore 
        /// di 200405)  oppure STATO(4) è uguale a 17 oppure 13 oppure 22 oppure 23 oppure 24 oppure 25 oppure 36  oppure 38 oppure 39 oppure 42 oppure 43 oppure 27 oppure 26  
        /// oppure 56 oppure 57  continuare l’elaborazione al punto successivo (60);
        /// 59.3.2.	Diversamente da quanto analizzato nel punto precedente effettuare le seguenti operazioni :
        /// 59.3.2.1.	Se IW1DEORIG è minore di 200206 e  ((STATO(1)  è uguale a 17 e  SETT1(1) è uguale zero) oppure  (STATO(2) è uguale a 17 e  SETT1(2) è uguale a zero) oppure  
        /// (STATO(3) è uguale a 17 e  SETT1(3) è uguale a zero) oppure  (STATO(4) è uguale a 17 e  SETT1(4) è uguale a zero)) continuare l’elaborazione al punto successivo (60);              
        /// 59.3.2.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con 1 il campo INDICE, con "76"  il campo                
        /// TIPO-ERRORE, con "STATO 17(U.E.)  INCOMPATIBILE CON SETTIMANE STATO "    +   STATO(4) il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);  
        /// 64.	Se  (ICI2CONV è uguale a 17 e  STATO(1)  è uguale a 17 )  effettuare le seguenti operazioni :  
        /// 64.1.	Se SETT2(1) è maggiore di zero : Valorizzare con "83"  il campo  TIPO-ERRORE, con 1     il campo INDICE, con "SETTIMANE ESTERE A RICALCOLO INCOMPATIBILI CON 
        /// STATO / CONVENZIONE (17)" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);  
        /// 64.2.	Se SETT2(2) è maggiore di zero : Valorizzare con "83"  il campo  TIPO-ERRORE, con 1     il campo INDICE, con "SETT.ESTERE A RICALCOLO (IST "  +  STATO(2)   +   ”)
        /// INCOMPATIBILI CON CONVENZIONE 17" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                               
        /// 64.3.	Se SETT2(3) è maggiore di zero : Valorizzare con "83"  il campo  TIPO-ERRORE, con 1     il campo INDICE , con "SETT.ESTERE A RICALCOLO (IST”   +    STATO(3)   +  
        /// ") INCOMPATIBILI CON CONVENZIONE 17"  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                               
        /// 64.4.	Se   SETT2(4) è maggiore di zero : Valorizzare con "83"  il campo  TIPO-ERRORE, con 1     il campo INDICE, con "SETT.ESTERE A RICALCOLO (IST "   +   STATO(4)   +
        /// ") INCOMPATIBILI CON CONVENZIONE 17" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88); 
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="listaPrestazioniEstere"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCompatibilitaTraStati(byte? codiceConvenzione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, DateTime? decorrenzaOriginaria, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 12 && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 38) > -1)
            {
                for (int i = 1; i < listaPrestazioniEstere.Count; i++)
                {
                    if (listaPrestazioniEstere[i].ContributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
                    {
                        if (!((new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 28, 32, 38, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 54, 55, 56, 57, 24, 25, 39, 43, 29, 17 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)) ||
                            (int.Parse(listaPrestazioniEstere[i].CodiceStatoEE) == 42 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2008, 06, 30))) ||
                            (int.Parse(listaPrestazioniEstere[i].CodiceStatoEE) == 13 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2010, 10, 31)))))
                        {
                            messaggioVideo = "Stato 38(U.E.) incompatibile con Settimane stato " + listaPrestazioniEstere[i].CodiceStatoEE;
                            return false;
                        }
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 12 && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 29 || int.Parse(x.CodiceStatoEE) == 40) > -1)
            {
                for (int i = 1; i < listaPrestazioniEstere.Count; i++)
                {
                    if (listaPrestazioniEstere[i].ContributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
                    {
                        if (!(new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 28, 32, 38, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 29, 40, 17 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)))
                        {
                            messaggioVideo = "Stato 29/40 incompatibile con settimane stato " + listaPrestazioniEstere[i].CodiceStatoEE;
                            return false;
                        }
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 12 && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 27) > -1)
            {
                for (int i = 1; i < listaPrestazioniEstere.Count; i++)
                {
                    if (listaPrestazioniEstere[i].ContributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
                    {
                        if (!(new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 28, 32, 38, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 27, 17 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)))
                        {
                            messaggioVideo = "Stato 27 incompatibile con settimane stato " + listaPrestazioniEstere[i].CodiceStatoEE;
                            return false;
                        }
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 12 && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 17) > -1)
            {
                for (int i = 1; i < listaPrestazioniEstere.Count; i++)
                {
                    if (listaPrestazioniEstere[i].ContributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
                    {
                        if (!(((new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 28, 32, 38, 41 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 05, 31))) ||
                            ((new List<int> { 44, 45, 46, 47, 48, 49, 50, 51, 52 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2006, 03, 31))) ||
                            ((new List<int> { 54, 55 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)) && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2009, 05, 31))) ||
                            (int.Parse(listaPrestazioniEstere[i].CodiceStatoEE) == 38 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 05, 31))) ||
                            (int.Parse(listaPrestazioniEstere[i].CodiceStatoEE) == 29 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 05, 31)) && Utility.IsDomandaAutomatica(datiPensione)) ||
                            ((new List<int> { 17, 13, 22, 23, 24, 25, 36, 38, 39, 42, 43, 27, 26, 56, 57 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)))))
                        {
                            if (!(!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 06, 01)) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 17 && x.ContributiEEDecorrenzaOriginaria.GetValueOrDefault() == 0) > -1))
                            {
                                messaggioVideo = "Stato 17(U.E.) incompatibile con settimane stato " + listaPrestazioniEstere[i].CodiceStatoEE;
                                return false;
                            }
                        }
                    }
                }
            }

            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                if (codiceConvenzione.GetValueOrDefault() == 17 && int.Parse(listaPrestazioniEstere[0].CodiceStatoEE) == 17)
                {
                    foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
                    {
                        if (prestEE.ContributiEERicalcolo.GetValueOrDefault() > 0)
                        {
                            messaggioVideo = "Settimane Estere a Ricalcolo (" + prestEE.CodiceStatoEE + ") incompatibili con Stato / Convenzione (17)";
                            return false;
                        }
                    }
                }
            }

            if (codiceConvenzione.GetValueOrDefault() == 60 && listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0 && listaPrestazioniEstere.FindIndex(x => int.Parse(x.CodiceStatoEE) == 04) > -1)
            {
                for (int i = 1; i < listaPrestazioniEstere.Count; i++)
                {
                    if (listaPrestazioniEstere[i].ContributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
                    {
                        if (!(new List<int> { 1, 2, 4, 6, 7, 9, 10, 11, 18, 19, 20, 28, 32, 38, 39, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 54, 55 }).Contains(int.Parse(listaPrestazioniEstere[i].CodiceStatoEE)))
                        {
                            messaggioVideo = "Stato 04 incompatibile con settimane stato " + listaPrestazioniEstere[i].CodiceStatoEE;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 63.	Se  (ICI2CONV  è uguale a 12 e  STATO(1)  è uguale a 17) e (SETT1(1) è maggiore di zero) effettuare le seguenti operazioni :  
        /// 63.1.	Se TP1CITT1 è uguale a "IS "   oppure   "FL "  oppure "N  " : Valorizzare con "83"  il campo  TIPO-ERRORE, con 1 il campo INDICE, con "SETTIMANE SVIZZERE 
        /// INCOMPATIBILI CON CITTADINANZA 'IS'-'FL'-'N' " il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                               
        /// </summary>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="cittadinanza"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaSettimaneSvizzere(byte? codiceConvenzione, int? codiceStatoEE, int? contributiEEDecorrenzaOriginaria, string cittadinanza, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceConvenzione.GetValueOrDefault() == 12 && codiceStatoEE.GetValueOrDefault() == 17 && contributiEEDecorrenzaOriginaria.GetValueOrDefault() > 0)
            {
                if ((new List<string> { "Z117", "Z119", "Z125" }).Contains(cittadinanza))
                {
                    messaggioVideo = "Settimane Svizzere incompatibili con Cittadinanza 'ISLANDA'-'LIECHTENSTEIN'-'NORVEGIA'";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 81.	Valorizzare con spazi i campi SI9601, SI9701, SI9801, SI9901, SI0001, SI0101, SI0201, SI0301 e SI0401.                                        
        /// 82.	Valorizzare con zero i campi IND-BIS,  INDICE, ERR-335C, ERR-335S, PR-RI(1),  PR-RI(2),         PR-RI(3)  e  PR-RI(4).           
        /// 83.	Aggiungere 1 al campo IND-BIS.                                            
        /// 84.	Se  IND-BIS è minore di 5 eseguire la subroutine  PRESENZA-01XX variando INDICE a partire da 1 con incremento di 1 fino a quando INDICE  è maggiore di MAX-INDICE 
        /// oppure ERR-335C è maggiore di zero;  riprendere l’elaborazione dal punto 83;
        /// 85.	Se ERR-335C è maggiore di zero effettuare le seguenti operazioni : Valorizzare con 1 il campo INDICE,  con "74" il campo  TIPO-ERRORE, con "PRESTAZIONE MANCANTE AL 
        /// 01/"       +     ERR-335C    +    " PER LO STATO "  +  ERR-335S  il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire da CONTROLLI-3 (88);                                 
        /// </summary>
        /// <param name="listaPrestazioniEstere"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="listaResidenzeEstere"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaPresenzaDecorrenza01_XXper335(List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, byte? codiceConvenzione, string codiceComuneResidenza, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            string si9601 = string.Empty;
            string si9701 = string.Empty;
            string si9801 = string.Empty;
            string si9901 = string.Empty;
            string si0001 = string.Empty;
            string si0101 = string.Empty;
            string si0201 = string.Empty;
            string si0301 = string.Empty;
            int[] pr_ri = new int[6];
            string cod9601 = string.Empty;
            string cod9701 = string.Empty;
            string cod9801 = string.Empty;
            string cod9901 = string.Empty;
            string cod0001 = string.Empty;
            string cod0101 = string.Empty;
            string cod0201 = string.Empty;
            string cod0301 = string.Empty;
            int[] ultimaRiga = new int[6];
            int err335C = 0;
            string err335S = string.Empty;
            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>[] arrayImportiEsteriPerStato = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>[6];

            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                for (int i = 0; i < listaPrestazioniEstere.Count; i++)
                {
                    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> LImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == listaPrestazioniEstere[i].Id);
                    ultimaRiga[i] = LImportiEsteri != null ? LImportiEsteri.Count : 0;
                    arrayImportiEsteriPerStato[i] = LImportiEsteri;
                }

                for (int i = 0; i < listaPrestazioniEstere.Count; i++)
                {
                    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> LImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == listaPrestazioniEstere[i].Id);

                    if (LImportiEsteri != null && LImportiEsteri.Count > 0)
                    {
                        for (int j = 0; j < LImportiEsteri.Count; j++)
                        {
                            if (!LImportiEsteri[j].DecorrenzaPrestazioneEE.HasValue || ((new List<string> { "14", "34", "20", codiceConvenzione.GetValueOrDefault().ToString() }).Contains(listaPrestazioniEstere[i].CodiceStatoEE) && !codiceComuneResidenza.StartsWith("Z")))
                            {
                                j = 50;
                            }
                            else
                            {
                                pr_ri[i] = 1;
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(1996, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si9601, ref cod9601, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(1997, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si9701, ref cod9701, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(1998, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si9801, ref cod9801, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(1999, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si9901, ref cod9901, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(2000, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si0001, ref cod0001, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(2001, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si0101, ref cod0101, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(2002, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si0201, ref cod0201, listaPrestazioniEstere[0].CodiceStatoEE);
                                GetValuesForPresenzaDec01_XX(LImportiEsteri[j].DecorrenzaPrestazioneEE, j > 0 ? LImportiEsteri[j - 1].CessazionePrestazioneEE : null, new DateTime(2003, 01, 01), i, arrayImportiEsteriPerStato, ultimaRiga, pr_ri, ref si0301, ref cod0301, listaPrestazioniEstere[0].CodiceStatoEE);

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(1996, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si9601 = string.Empty;
                                    si9701 = string.Empty;
                                    si9801 = string.Empty;
                                    si9901 = string.Empty;
                                    si0001 = string.Empty;
                                    si0101 = string.Empty;
                                    si0201 = string.Empty;
                                    si0301 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(1997, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si9701 = string.Empty;
                                    si9801 = string.Empty;
                                    si9901 = string.Empty;
                                    si0001 = string.Empty;
                                    si0101 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(1998, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si9801 = string.Empty;
                                    si9901 = string.Empty;
                                    si0001 = string.Empty;
                                    si0101 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(1999, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si9901 = string.Empty;
                                    si0001 = string.Empty;
                                    si0101 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(2000, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si0001 = string.Empty;
                                    si0101 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(2001, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si0101 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(2002, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si0201 = string.Empty;
                                }

                                if (arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE.HasValue && arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE.Value, new DateTime(2003, 01, 01)) && (arrayImportiEsteriPerStato[i].Count <= j + 1 || !arrayImportiEsteriPerStato[i][j + 1].DecorrenzaPrestazioneEE.HasValue))
                                {
                                    si0301 = string.Empty;
                                }

                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si9601, cod9601, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), true, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si9701, cod9701, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si9801, cod9801, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si9901, cod9901, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si0001, cod0001, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si0101, cod0101, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si0201, cod0201, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                                GetValuesForERR335(arrayImportiEsteriPerStato[i][j].CessazionePrestazioneEE, codiceConvenzione, arrayImportiEsteriPerStato[i][j].DecorrenzaPrestazioneEE, j > 0 ? arrayImportiEsteriPerStato[i][j - 1].CessazionePrestazioneEE : null, si0301, cod0301, listaPrestazioniEstere[i].CodiceStatoEE, codiceComuneResidenza, listaResidenzeEstere, new DateTime(1996, 01, 01), false, ref err335C, ref err335S);
                            }
                        }
                    }

                    if (err335C > 0)
                    {
                        messaggioVideo = "Prestazione mancante al 01/" + err335C + " per lo Stato " + err335S;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 87.	Se  IABCONA4 è uguale a "V" effettuare le seguenti operazioni :                                                       
        /// 87.1.	Se  (STATO(1) è maggiore di zero e  IMPEST(1 1) è uguale a zero) oppure  (STATO(2) è maggiore di zero e  IMPEST(2 1) è uguale a zero) oppure  (STATO(3) è 
        /// maggiore di zero e  IMPEST(3 1) è uguale a zero) oppure  (STATO(4) è maggiore di zero e  IMPEST(4 1) è uguale a zero) continuare l’elaborazione al punto successivo; (88);
        /// 87.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : Valorizzare con "84"  il campo  TIPO-ERRORE, con 1 il campo INDICE , 
        /// con "COD.NAT.PENSIONE 'V' (CNV.01) INCOMPATIBILE CON IMPORTI ESTERI" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR;                             
        /// </summary>
        /// <param name="listaPrestazioniEstere"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaImportiEsteriWithCodNatura(List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isOK = false;
            if (!string.IsNullOrEmpty(codNatura) && codNatura.Substring(2, 1).Equals("V"))
            {
                foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestEE in listaPrestazioniEstere)
                {
                    GestioneDatiContributiviCi.PensioniCiImportiEsteri primoImportoEstero = listaImportiEsteri.FirstOrDefault(x => x.IDPrestazioneEE == prestEE.Id);
                    if (primoImportoEstero == null || primoImportoEstero.ImportoPrestazioneEE.GetValueOrDefault() == 0)
                    {
                        isOK = true;
                        break;
                    }
                }

                if (!isOK)
                {
                    messaggioVideo = "Cod.Nat.Pensione 'V' incompatibile con importi esteri";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 17.2.	Se RIGA-YU è uguale a "S" e ICI1DECYU  maggiore di zero effettuare le seguenti operazioni :
        /// 17.2.1.	Se ICI1DECYUM non è uguale a  12 effettuare le seguenti operazioni: 
        /// 17.2.1.1.	Valorizzare con "05"  il campo  TIPO-ERRORE, con "MESE DEC.INTEGRAZ. ESTERA DIVERSO DA '12’” il campo MESSAGGIO-ERRORE,  Valorizzare con 1 il campo FLAG-ERR, 
        /// uscire  da CONTROLLI-1 (33);   
        /// 17.2.2.	Se ICI1DECYU è maggiore del valore costituito dai primi 6 caratteri contenuto in DATA-GIORNO  effettuare le seguenti operazioni:
        /// 17.2.2.1.	Valorizzare con "08"  il campo TIPO-ERRORE, con "DEC. INTEGRAZIONE ESTERA ERRATA: POSTERIORE ALLA DATA ODIERNA"  il campo MESSAGGIO-ERRORE, con 1 il campo    
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// 17.2.3.	Se ICI1DECYU  è inferiore a  DEC (IND-STA  1)  effettuare le seguenti operazioni:
        /// 17.2.3.1.	Valorizzare con "09"  il campo  TIPO-ERRORE, con "DEC. INTEGRAZIONE ESTERA ERRATA: ANTERIORE A DEC. PRESTAZIONE ESTERA"  il campo MESSAGGIO-ERRORE, con 1 il 
        /// campo FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// 17.2.4.	Se IDAPLIQA (IND-STA) maggiore di zero e ICI1DECYUA  è inferiore a  IDAPLIQA(IND-STA) effettuare le seguenti operazioni:                                           
        /// 17.2.4.1.	Valorizzare con "09"  il campo  TIPO-ERRORE, con "ANNO DEC.INTEGRAZIONE ESTERA MINORE DA ANNO PRECEDENTE LIQUIDAZIONE” il campo MESSAGGIO-ERRORE, con 1 il 
        /// campo FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// 17.2.5.	Se ICI1INTRYU non é maggiore di zero  effettuare le seguenti operazioni:
        /// 17.2.5.1.	Valorizzare con "10"  il campo  TIPO-ERRORE,
        /// 17.2.5.2.	 con "IMPORTO INTEGRAZIONE ESTERA MANCANTE" il campo MESSAGGIO-ERRORE, con 1 il campo FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// 17.2.6.	Se IMPEST(IND-STA 1) è uguale a zero effettuare le seguenti operazioni:
        /// 17.2.6.1.	Valorizzare con "10"  il campo  TIPO-ERRORE, con "IMPORTO INTEGRAZIONE ESTERA INCOMPATIBILE CON IMPORTO ESTERO"  il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// 17.3.	Diversamente da quanto analizzato nel punto precedente (17.2) effettuare i seguenti controlli  :
        /// 17.3.1.	Se IND-STA è uguale a 1 e ICI1INTRYU è maggiore di zero effettuare le seguenti operazioni  :                                         
        /// 17.3.1.1.	Valorizzare con "11"  il campo  TIPO-ERRORE,  con "IMPORTO INTEGRAZIONE ESTERA INCOMPATIBILE CON DECORRENZA" il campo MESSAGGIO-ERRORE, con 1 il campo 
        /// FLAG-ERR, uscire  da CONTROLLI-1 (33);
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaIntegrazione"></param>
        /// <param name="quotaIntegrazioneEEeArgentinaResidentiItalia"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlliYugoslavia(byte? causaCarico, byte? codiceConvenzione, string codiceComuneResidenza, int? codiceStatoEE, DateTime? primaDecorrenzaImportiEsteri, DateTime? decorrenzaOriginaria, DateTime? decorrenzaIntegrazione, decimal? quotaIntegrazioneEEeArgentinaResidentiItalia, DateTime? decorrenzaLiquidazioneStatoEE, decimal? primoImportoPrestazioneEE, int index, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (GetRigaYugoslavia(causaCarico, codiceConvenzione, codiceComuneResidenza, codiceStatoEE, primaDecorrenzaImportiEsteri, decorrenzaOriginaria) && decorrenzaIntegrazione.HasValue)
            {
                if (decorrenzaIntegrazione.Value.Month != 12)
                {
                    messaggioVideo = "Mese Decorrenza Integrazione Estera diverso da '12’";
                    return false;
                }

                if (Utility.DataStrettamenteSuccessivaA(new DateTime(decorrenzaIntegrazione.Value.Year, decorrenzaIntegrazione.Value.Month, 01), new DateTime(dataSistema.Year, dataSistema.Month, 01)))
                {
                    messaggioVideo = "Decorrenza Integrazione Estera errata: posteriore alla data odierna";
                    return false;
                }

                if (primaDecorrenzaImportiEsteri.HasValue && !Utility.DataSuccessivaA(decorrenzaIntegrazione.Value, primaDecorrenzaImportiEsteri.Value))
                {
                    messaggioVideo = "Decorrenza Integrazione Estera errata: anteriore a Decorrenza Prestazione Estera";
                    return false;
                }

                if (decorrenzaLiquidazioneStatoEE.HasValue && decorrenzaIntegrazione.Value.Year < decorrenzaLiquidazioneStatoEE.Value.Year)
                {
                    messaggioVideo = "Anno Dec.Integrazione Estera minore da anno Precedente Liquidazione";
                    return false;
                }

                if (quotaIntegrazioneEEeArgentinaResidentiItalia.GetValueOrDefault() <= 0)
                {
                    messaggioVideo = "Importo Integrazione Estera mancante";
                    return false;
                }

                if (primoImportoPrestazioneEE.GetValueOrDefault() == 0)
                {
                    messaggioVideo = "Importo Integrazione Estera incompatibile con Importo Estero";
                    return false;
                }
            }
            else
            {
                if (index == 0 && quotaIntegrazioneEEeArgentinaResidentiItalia.GetValueOrDefault() > 0)
                {
                    messaggioVideo = "Importo Integrazione Estera incompatibile con Decorrenza";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL12

        public static bool VerificaSospensioneCautelativaIntegrazioneObbligatoria(char? sospensioneCautelativaIntegrazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!sospensioneCautelativaIntegrazione.HasValue)
            {
                messaggioVideo = "Sospensione Integrazione Trattamento minimo obbligatoria.";
                return false;
            }

            return true;
        }

        public static bool ControlsDomandeInabilità(GestionePensione.DatiPensione datiPensione, int? nSettimaneOBG, int? settimaneEstere, int? settimaneFittizie, int? contributiItalianiEdEsteriAl1295, int? settimaneItalianeDiritto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int? nSettimane = null;
            string tipoSettimane = string.Empty;

            if (nSettimaneOBG.HasValue)
            {
                nSettimane = nSettimaneOBG;
                tipoSettimane = "settimane OBG";
            }
            else if (settimaneItalianeDiritto.HasValue)
            {
                nSettimane = settimaneItalianeDiritto;
                tipoSettimane = "settimane italiane diritto";
            }

            if (Utility.IsDomandaPensioneInabilita(datiPensione))
            {
                if (nSettimane.HasValue && nSettimane.Value + settimaneEstere.GetValueOrDefault() < 260)
                {
                    messaggioVideo = "Il numero di " + tipoSettimane + " + Settimane Diritto(Dati calcolo - Istituzione estera) deve essere maggiore di 259";
                    return false;
                }

                if (nSettimane.HasValue && nSettimane.Value + settimaneEstere.GetValueOrDefault() > 2080 && settimaneFittizie.HasValue)
                {
                    messaggioVideo = "Il numero di " + tipoSettimane + " + Settimane Diritto(Dati calcolo - Istituzione estera) è maggiore di 2080. E' necessario non valorizzare le settimane fittizie";
                    return false;
                }

                if (contributiItalianiEdEsteriAl1295.GetValueOrDefault() >= 936 && settimaneFittizie.HasValue)
                {
                    messaggioVideo = "Il numero dei contributi Italiani ed Esteri al 31/12/95 maggiore di 935. E' necessario non valorizzare le settimane fittizie";
                    return false;
                }
            }

            return true;
        }
        #endregion ProRata

        #endregion DatiContributivi

        #region Titolare

        #region PCIPL35
        /// <summary>
        /// Verifica se il sindacato passato in input è presente nella lista dei sindacati attivi
        /// </summary>
        /// <param name="sindacato"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool VerificaSindacatoAttivo(GestionePensione.DatiSindacato sindacato, string siglaCategoria, out string msg)
        {
            msg = string.Empty;
            List<Liquidazione.BLCommon.Entity.Sindacato> elencoSindacato = null;
            if (sindacato != null)
            {
                string idCategoria = Liquidazione.BLCommon.GestioneSindacati.GetIdCategoriaForSindacato(siglaCategoria, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(idCategoria, out elencoSindacato, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                elencoSindacato = Liquidazione.BLCommon.GestioneSindacati.GetElencoSindacatiAttivi(elencoSindacato, out msg);
                if (!string.IsNullOrEmpty(msg))
                    return false;

                int index = elencoSindacato.FindIndex(x => x.Id == sindacato.CodiceSindacato.Trim());
                if (index < 0)
                {
                    msg = "Il Sindacato attualmente salvato non è più attivo.";
                    return false;
                }
            }
            return true;
        }
        #endregion PCIPL35

        /// <summary>
        /// Verifica se la decorrenza pensione è compatibile con l'età del titolare
        /// </summary>
        /// <param name="codeNatura"></param>
        /// <param name="dataPerfReq"></param>
        /// <param name="categoria"></param>
        /// <param name="tipoCalcolo"></param>
        /// <param name="annoNascitaTitolare"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="legge44997"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenza(string codeNatura, DateTime? dataPerfReq, string categoria, DateTime? dataNascitaTitolare, char? sessoTitolare, byte? legge44997, byte? codiceCieco, int? attivitaEconomica, int? professioneIndividuale, char? codiceParticolareSoggettoDerogato, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = null;
            DateTime? decCompare = null; // W-CAL-DECORR

            if ((categoria.Trim() == "VOS" || categoria.Trim() == "VRS" || categoria.Trim() == "VOARTS" || categoria.Trim() == "VOCOMS") &&
                codeNatura.Substring(0, 1) != "1" && codeNatura.Substring(0, 1) != "2" && dataPerfReq.GetValueOrDefault().CompareTo(new DateTime(2011, 12, 31)) > 0)
            {
                decCompare = CalcolaDecorrenza214(decCompare, dataNascitaTitolare, sessoTitolare, codiceCieco, categoria, legge44997, dataPerfReq);
            }

            if (decCompare.HasValue && decCompare.Value.CompareTo(decorrenzaOriginaria) >= 0)
            {
                messaggioVideo = "Decorrenza Originaria errata (Legge n.247 del 22/12/2011)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Controlli sulla decorrenzaArretrati per domande PL
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaArretratiPL(DateTime? decorrenzaArretrati, DateTime? decorrenzaOriginaria, GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaRipristino, DateTime dataSistema, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.CI, out annoCompetenza);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            if (decorrenzaArretrati.HasValue)
            {
                if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == Utility.DataSistemaCi.Year &&
                    (decorrenzaArretrati.Value.Year != decorrenzaOriginaria.Value.Year || decorrenzaArretrati.Value.Month != decorrenzaOriginaria.Value.Month))
                {
                    messaggioVideo = "La decorrenza arretrati deve essere uguale alla decorrenza pensione";
                    return false;
                }

                if (decorrenzaArretrati.Value.Year > annoCompetenza || (decorrenzaArretrati.Value.Year > Utility.DataSistemaCi.Year &&
                    Utility.DataSuccessivaA(decorrenzaArretrati.Value, new DateTime(annoCompetenza, 01, 31))))
                {
                    messaggioVideo = "La data decorrenza arretrati non può essere superiore all'anno di competenza";
                    return false;
                }

                if (decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year < annoCompetenza)
                {
                    if (Utility.DataStrettamenteSuccessivaA(decorrenzaArretrati.Value, new DateTime(annoCompetenza, 01, 01)))
                    {
                        messaggioVideo = "La data decorrenza arretrati non può essere superiore a gennaio dell'anno di competenza.";
                        return false;
                    }
                }

                if (decorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(decorrenzaArretrati.Value, decorrenzaOriginaria.Value))
                {
                    messaggioVideo = "La data decorrenza arretrati non può essere inferiore alla decorrenza pensione";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "La data decorrenza arretrati è obbligatoria.";
                return false;
            }

            if (decorrenzaRipristino.HasValue)
            {
                if (tipoDomanda == Utility.TipoDomanda.Ripristino)
                {
                    //La Data Ripristino (RAU105) non può essere superiore a gennaio dell'anno in
                    //corso
                    if (Utility.DataStrettamenteSuccessivaA(decorrenzaRipristino.Value, new DateTime(dataSistema.Year, 01, 31)))
                    {
                        messaggioVideo = "La data ripristino non può essere superiore a gennaio dell'anno in corso";
                        return false;
                    }

                    //La Data Ripristino (RAU105) deve essere posteriore alla Decorrenza
                    //Originaria (RAU104)
                    if (!Utility.DataStrettamenteSuccessivaA(decorrenzaRipristino.Value, decorrenzaOriginaria.Value))
                    {
                        messaggioVideo = "La data ripristino deve essere posteriore alla decorrenza originaria";
                        return false;
                    }

                    //La Data Ripristino (RAU105) non può essere inferiore a gennaio 1965
                    if (!Utility.DataSuccessivaA(decorrenzaRipristino.Value, new DateTime(1965, 01, 01)))
                    {
                        messaggioVideo = "La data ripristino non può essere inferiore a gennaio 1965";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Controlli sulla decorrenzaArretrati per domande RIC
        /// </summary>
        /// <param name="decorrenzaArretrati"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="causaCarico"></param>
        /// <param name="dataInizioCalcolo"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool ControlsDecorrenzaArretratiRIC(DateTime? decorrenzaArretrati, DateTime? decorrenzaOriginaria, byte? causaCarico, DateTime? dataInizioCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            int annoCompetenza = 0;
            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.CI, out annoCompetenza);

            if (decorrenzaArretrati.HasValue && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year == annoCompetenza &&
                (decorrenzaArretrati.Value.Year != decorrenzaOriginaria.Value.Year || decorrenzaArretrati.Value.Month != decorrenzaOriginaria.Value.Month))
            {
                messaggioVideo = "La decorrenza arretrati deve essere uguale alla decorrenza pensione";
                return false;
            }

            if (decorrenzaArretrati.HasValue && decorrenzaOriginaria.HasValue && decorrenzaOriginaria.Value.Year != annoCompetenza)
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaArretrati.Value, new DateTime(annoCompetenza, 01, 01)))
                {
                    messaggioVideo = "La data decorrenza arretrati non può essere superiore a gennaio dell'anno di competenza.";
                    return false;
                }

                if (Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, decorrenzaArretrati.Value))
                {
                    messaggioVideo = "La data decorrenza arretrati non può essere inferiore alla decorrenza pensione.";
                    return false;
                }
            }

            if (causaCarico == 3 || causaCarico == 9)
            {
                if (decorrenzaArretrati.HasValue && Utility.DataStrettamenteSuccessivaA(dataInizioCalcolo.Value, decorrenzaArretrati.Value))
                {
                    messaggioVideo = "La 'Decorrenza Arretrati' non può essere minore della 'Data di Inizio Calcolo'";
                    return false;
                }
            }

            return true;
        }


        #endregion Titolare

        #region Maggiorazioni e Benefici

        #region Benefici

        #region PCIPL35
        public static bool ControlsNSettimaneIncremento1Percento(int? nSettimaneIncremento1Percento, string gruppo, DateTime? decorrenzaOriginaria, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (nSettimaneIncremento1Percento.HasValue)
            {
                if (gruppo.Equals("0002") || !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1995, 01, 01)) || codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2"))
                {
                    messaggioVideo = "Settimane incremento 1% incompatibili con Categoria o Decorrenza Pensione";
                    return false;
                }

                if (nSettimaneIncremento1Percento.Value != 52 && nSettimaneIncremento1Percento.Value != 104 && nSettimaneIncremento1Percento.Value != 156 && nSettimaneIncremento1Percento.Value != 208)
                {
                    messaggioVideo = "Settimane incremento 1% errate. Valori consentiti (52 - 104 - 156 - 208)";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsNSettimaneIncremento05Percento(int? nSettimaneIncremento05Percento, string gruppo, DateTime? decorrenzaOriginaria, string codNatura, char? sessoTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (nSettimaneIncremento05Percento.HasValue)
            {
                if (gruppo.Equals("0002") || !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1995, 01, 01)) || codNatura.Substring(0, 1).Equals("1") || codNatura.Substring(0, 1).Equals("2"))
                {
                    messaggioVideo = "Settimane incremento 0.5% incompatibili con Categoria o Decorrenza Pensione";
                    return false;
                }

                if (gruppo.Equals("0001") && sessoTitolare.Value != 'F')
                {
                    messaggioVideo = "Settimane incremento 0.5% incompatibili con il sesso del Titolare della Pensione";
                    return false;
                }

                if (nSettimaneIncremento05Percento.Value != 52 && nSettimaneIncremento05Percento.Value != 104 && nSettimaneIncremento05Percento.Value != 156 && nSettimaneIncremento05Percento.Value != 208 && nSettimaneIncremento05Percento.Value != 260)
                {
                    messaggioVideo = "Settimane incremento 0.5% errate. Valori consentiti (52 - 104 - 156 - 208 - 260)";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL35

        #region PCIPL40
        /// <summary>
        /// Se i campi (ICISET1X100 + ICISET05X100) > 0 allora muovi  19940101 nel campo APP-DATA-1 ,muovi FINASS  nel campo  APP-DATA-2  e chiama il programma  "PCIPL94"   usando i 
        /// campi  APP-DATA-1 APP-DATA-2 APP-DATA-RC  dopo la chiamata controlla se I campi  (ICISET1X100 + ICISET05X100) > APP-DATA-RC   allora segnala errore "SETTIMANE INCR 1%  - 
        /// 0.5% (CNV01) SUPERIORI A CAPIENZA"    
        /// </summary>
        /// <param name="siglaCategoria"></param>
        /// <param name="nSettimaneIncremento1Percento"></param>
        /// <param name="nSettimaneIncremento05Percento"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCapienzaNSettimaneIncrementoPercentuale(int? nSettimaneIncremento1Percento, int? nSettimaneIncremento05Percento, DateTime? dataFineAssicurazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int sommaSettimaneIncrementoPercentuale = nSettimaneIncremento1Percento.GetValueOrDefault() + nSettimaneIncremento05Percento.GetValueOrDefault();
            if (sommaSettimaneIncrementoPercentuale > 0)
            {
                int capienzaSettimane = Utility.NSettimaneBetweenDate(dataFineAssicurazione.Value, new DateTime(1994, 01, 01));
                if (sommaSettimaneIncrementoPercentuale > capienzaSettimane)
                {
                    messaggioVideo = "Settimane Incremento 1%  - 0.5% superiori a capienza";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL40

        public static bool ControlsBeneficioPrecoci(GestionePensione.DatiPensione datiPensione, string tipoSettimaneBeneficio, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!Utility.IsDomandaAPEPrecoci(datiPensione) && tipoSettimaneBeneficio == "11")
            {
                messaggioVideo = "Non è possibile acquisire il beneficio \"LAVORATORE PRECOCE\" per una domanda non di tipologia APE Precoci";
                return false;
            }

            return true;
        }

        public static bool ControlsBeneficioMaggiorazioneAmiantoLegge208_2015(GestionePensione.DatiPensione datiPensione, int? nSettimaneBeneficio, short? settAnzContr311295, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if ((Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione)) &&
                 Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && nSettimaneBeneficio.HasValue && settAnzContr311295.HasValue)
            {
                if (nSettimaneBeneficio < settAnzContr311295)
                {
                    messaggioVideo = "Il Numero settimane beneficio deve essere maggiore o uguale del Numero settimane anz contrib successiva al 31/12/1995";
                    return false;
                }
            }

            return true;
        }

        #endregion Benefici

        #region Maggiorazioni
        #region PCIPL35
        /// <summary>
        /// Verifica che la decorrenza Maggiorazioni Sociali non sia antecedente alla decorrenza Pensione o al 07/1988
        /// </summary>
        /// <param name="decorrenzaMaggiorazioneSociale"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns>False se è antecedente</returns>
        public static bool VerificaDecorrenzaMaggiorazioneSocialeWithDecorrenzaOriginaria(DateTime? decorrenzaMaggiorazioneSociale, DateTime? decorrenzaOriginaria)
        {
            if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneSociale.Value, decorrenzaOriginaria.Value) || !Utility.DataSuccessivaA(decorrenzaMaggiorazioneSociale.Value, new DateTime(1988, 07, 01)))
                return false;

            return true;
        }

        public static bool ControlsMaggiorazioniWithEtaPensionabile(DateTime? decorrenzaMaggiorazioneSociale, DateTime? dataNascitaTitolare, string gruppo, byte? causaCarico, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = dataNascitaTitolare.Value.AddYears(65).AddMonths(1);

            string siglaCategoria = datiPensione.SiglaCategoria.Trim();
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            Utility.GetCodiciNatura(datiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneControlliMemo72", out ctrl);

            if (!(Utility.DataSuccessivaA(decorrenzaMaggiorazioneSociale.Value, dataCompare)) && !(ctrl != null && ctrl.ValoreControllo == "SI" && ((siglaCategoria == "IOS" || siglaCategoria == "IRS" || siglaCategoria == "IOARTS" || siglaCategoria == "IOCOMS") &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1984, 7, 31)) && (codNat1 == '1' || codNat1 == '2' || codNat1 == '3' || codNat1 == '4') &&
                Utility.DataStrettamenteSuccessivaA(decorrenzaMaggiorazioneSociale.Value, new DateTime(2020, 7, 31)) && !Utility.DataStrettamenteSuccessivaA(decorrenzaMaggiorazioneSociale.Value, dataNascitaTitolare.Value.AddYears(60)))))
            {
                if (!(gruppo.Equals("0003") && (causaCarico.GetValueOrDefault() == 2 || causaCarico.GetValueOrDefault() == 3 || causaCarico.GetValueOrDefault() == 9)))
                {
                    dataCompare = dataCompare.AddYears(-5);

                    if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneSociale.Value, dataCompare))
                    {
                        messaggioVideo = "Decorrenza L.544/1 anteriore ad eta' pensionabile";
                        return false;
                    }
                    else
                    {
                        if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneSociale.Value, new DateTime(1989, 01, 01)))
                        {
                            messaggioVideo = "Decorrenza L.544/1 per ultrasessantenni anteriore a 01/1989";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo (IW1DECMS1  minore IW1DEORIG (DEC. ORIGINARIA PENSIONE))  OR (IW1DECMS1  minore 198501)  OR (IW1DECMS1  > 198806) segnala  errore  "DECORR. L.140/1 ANTERIORE A 
        /// DEC. ORIG. O 01/1985 O POST 06/1988"  
        /// </summary>
        /// <param name="decorrenzaMaggiorazioneLegge140"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaMaggiorazioneLegge140(DateTime? decorrenzaMaggiorazioneLegge140, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaMaggiorazioneLegge140.HasValue)
            {
                if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneLegge140.Value, decorrenzaOriginaria.Value) || !Utility.DataSuccessivaA(decorrenzaMaggiorazioneLegge140.Value, new DateTime(1985, 01, 01)) || Utility.DataStrettamenteSuccessivaA(decorrenzaMaggiorazioneLegge140.Value, new DateTime(1988, 06, 30)))
                {
                    messaggioVideo = "Decorrenza L.140/1 anteriore a Decorrenza Originaria o 01/1985 o post 06/1988";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo (APPO-CAT1  = "S"  AND  (IW1CARIC  = 2 OR 3 OR 9) )  allora continua altrimenti che il campo IW1NATITM (MESE DATA NASCITA DEL TITOL.)  minore 12 allora  fa 
        /// un il controllo con i 65 anni compiuti (COMPUTE W-APP-APP = IW1NAT6 + 6501) altrimenti  COMPUTE W-APP-APP =((IW1SECAN + 66) * 100) + 1 una volta calcolato gli anni 
        /// controlla che se il campo IW1DECMS1 (DEC. MAG. SOC.ART.1/140)  minore W-APP-APP(anni calcolati) segnala errore  "DECORR. L.140/1 ANTERIORE AD ETA'  PENSIONABILE" 
        /// </summary>
        /// <param name="decorrenzaMaggiorazioneLegge140"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="causaCarico"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaMaggiorazioneLegg140WithEtaPensionabile(DateTime? decorrenzaMaggiorazioneLegge140, Utility.TipoDomanda tipoDomanda, byte? causaCarico, DateTime? dataNascitaTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (decorrenzaMaggiorazioneLegge140.HasValue)
            {
                if (!(tipoDomanda == Utility.TipoDomanda.Superstiti && (causaCarico.GetValueOrDefault() == 2 || causaCarico.GetValueOrDefault() == 3 || causaCarico.GetValueOrDefault() == 9)))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneLegge140.Value, new DateTime(dataNascitaTitolare.Value.AddMonths(1).Year + 65, dataNascitaTitolare.Value.AddMonths(1).Month, 01)))
                    {
                        messaggioVideo = "Decorrenza L.140/1 anteriore ad eta' pensionabile";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo ANNI-ANTICIPO-544 ( NUMERO ANNI ANTICIPO PER MAGGIORAZIONE SOCIALE) > 0  allora controlla 
        /// *  che il campo APPO-CAT1  NOT = "S" segnala errore "ANNI RIDUZIONE ETA' CONSENTITI SOLO PER REVERSIBILITA' "  
        /// *  Se il campo IW1DEC544 (DEC.AUMENTO SOC.LG.544)  = 0 allora segnala errore "ANNI RIDUZIONE ETA' INCOMPATIBILI CON  DEC.L. 544/1" 
        /// *  Se il campo ANNI-ANTICIPO-544 > 5 segnala errore "ANNI RIDUZIONE ETA' ERRATI (MAGGIORI DI 5)" 
        /// </summary>
        /// <param name="anniRiduzioneBeneficiArt38Legge02"></param>
        /// <param name="tipoDomanda"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaAnniRiduzioneBeneficiArt38Legge02(short? anniRiduzioneBeneficiArt38Legge02, Utility.TipoDomanda tipoDomanda, DateTime? decorrenzaMaggiorazioneSociale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (anniRiduzioneBeneficiArt38Legge02.GetValueOrDefault() > 0)
            {
                if (tipoDomanda != Utility.TipoDomanda.Superstiti)
                {
                    messaggioVideo = "Anni Riduzione Eta' consentiti solo per Reversibilita'";
                    return false;
                }

                if (!decorrenzaMaggiorazioneSociale.HasValue)
                {
                    messaggioVideo = "Anni Riduzione Eta' incompatibili con Decorrenza Legge 544/1";
                    return false;
                }

                if (anniRiduzioneBeneficiArt38Legge02.GetValueOrDefault() > 5)
                {
                    messaggioVideo = "Anni Riduzione Eta' errati (maggiori di 5)";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IREQA2C3-385 (REQUISITO ART.2,COMMA 3,LG.385/2000: 0/N=NO 1/S=SI 2/A=AUTOMATICO) NOT = SPACE  AND  "S"  segnala errore "REQ.ART.2 COMMA 3 D.L.503/92, SE 
        /// PRESENTE, DEVE ESSERE 'S' " 
        /// Se il campo IREQA2C3-385 = "S" allora controlla:
        /// *  Se il campo IW1DEORIG minore "199401" segnala errore "REQ.ART.2 COMMA 3 D.L.503/92 INCOMPATIBILE  CON DECORRENZA PENSIONE" 
        /// *  Se il campo APPO-CAT1 NOT  = "S"   allora controlla i campi ( IW1CATPEN minore 7 AND IW1SESTIT (SESSO TITOLARE)  = "M AND IW1NATIT  (DATA NASCITA TITOLARE) > 19360100) OR  
        ///    (  IW1CATPEN minore 7 AND IW1SESTIT = "F"   AND IW1NATIT > 19410100) OR  (IW1CATPEN > 6 AND IW1SESTIT = "M"  AND IW1NATIT > 19310100) OR  ( IW1CATPEN > 6 AND 
        ///    IW1SESTIT = "F"  AND IW1NATIT > 19360100) allora segnala errore "REQ.ART.2 COMMA 3 D.L.503/92 INCOMPATIBILE   CON DATA NASCITA"  
        /// </summary>
        /// <param name="codiceRequisitiLegge50392Art2"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceRequisitiLegge50392(char? codiceRequisitiLegge50392Art2, DateTime? decorrenzaOriginaria, Utility.TipoDomanda tipoDomanda, int categoria, DateTime? dataNascitaTitolare, char? sessoTitolare, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceRequisitiLegge50392Art2.HasValue && codiceRequisitiLegge50392Art2.Value != 'S')
            {
                messaggioVideo = "Req.Art.2 Comma 3 D.L.503/92, se presente, deve essere 'Si accertamento sede'";
                return false;
            }

            if (codiceRequisitiLegge50392Art2.HasValue && codiceRequisitiLegge50392Art2.Value == 'S')
            {
                if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01)))
                {
                    messaggioVideo = "Req.Art.2 Comma 3 D.L.503/92 incompatibile con Decorrenza Pensione";
                    return false;
                }

                if (tipoDomanda != Utility.TipoDomanda.Superstiti)
                {
                    if ((categoria < 7 && sessoTitolare.GetValueOrDefault() == 'M' && Utility.DataSuccessivaA(dataNascitaTitolare.Value, new DateTime(1936, 01, 01))) ||
                        (categoria < 7 && sessoTitolare.GetValueOrDefault() == 'F' && Utility.DataSuccessivaA(dataNascitaTitolare.Value, new DateTime(1941, 01, 01))) ||
                        (categoria > 6 && sessoTitolare.GetValueOrDefault() == 'M' && Utility.DataSuccessivaA(dataNascitaTitolare.Value, new DateTime(1931, 01, 01))) ||
                        (categoria > 6 && sessoTitolare.GetValueOrDefault() == 'F' && Utility.DataSuccessivaA(dataNascitaTitolare.Value, new DateTime(1936, 01, 01))))
                    {
                        messaggioVideo = "Req.Art.2 Comma 3 D.L.503/92 incompatibile con Data Nascita";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Se il campo IREQA2C3-385 = "S" allora controlla i campi ( APPO-CAT1 = "I" AND (IABCONA2 NOT = "3" AND "4")) segnala errore "REQ.ART.2 COMMA 3 D.L.503/92 INCOMPATIBILE  
        /// CON CATEGORIA DI PENSIONE"
        /// </summary>
        /// <param name="codiceRequisitiLegge50392Art2"></param>
        /// <param name="gruppo"></param>
        /// <param name="codNatura"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceRequisitiLegge50392WithInvalidita(char? codiceRequisitiLegge50392Art2, string gruppo, string codNatura, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceRequisitiLegge50392Art2.GetValueOrDefault() == 'S')
            {
                if (gruppo.Equals("0002") && (string.IsNullOrEmpty(codNatura) || (!codNatura.Substring(0, 1).Equals("3") && !codNatura.Substring(0, 1).Equals("4"))))
                {
                    messaggioVideo = "Req.Art.2 Comma 3 D.L.503/92 incompatibile con Categoria di Pensione";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL35
        #endregion Maggiorazioni

        #region Vittime terrorismo
        public static bool ControlsCoerenzaBeneficioVittimeTerrorismo(long? tipologiaPrestazione, long? tipologiaBeneficio, long? soggettoBeneficiario, string soggettoBeneficiarioTraduzioneSuGP,
           out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "V2")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2 && tipologiaPrestazione.GetValueOrDefault() != 3)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità => 25%\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\" o \"Art. 4 comma 2 bis L. 206/2004\"";
                    return false;
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 2)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 1 && tipologiaBeneficio.GetValueOrDefault() != 2)
                    {
                        messaggioVideo = "Per Tipologia di Prestazione \"Art. 2 e 3 L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° settembre 2004\" o \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 3)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 5)
                    {
                        messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità < 25%\" e Tipologia di Prestazione \"Art. 4 comma 2 bis L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 4 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "V1")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Vittima con invalidità < 25%\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaPrestazione.GetValueOrDefault() == 2)
                {
                    if (tipologiaBeneficio.GetValueOrDefault() != 1 && tipologiaBeneficio.GetValueOrDefault() != 2)
                    {
                        messaggioVideo = "Per Tipologia di Prestazione \"Art. 2 e 3 L. 206/2004\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° settembre 2004\" o \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(soggettoBeneficiarioTraduzioneSuGP) && soggettoBeneficiarioTraduzioneSuGP.Trim() == "G")
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Genitore\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Genitore\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                    return false;
                }
            }

            if (soggettoBeneficiario.GetValueOrDefault() == 4 || soggettoBeneficiario.GetValueOrDefault() == 7)
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge/Vedovo\" o \"Figlio/orfano\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2 && tipologiaBeneficio.GetValueOrDefault() != 3)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge/Vedovo\" o \"Figlio/orfano\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\" o \"Benefici art. 2-3 con decorrenza sia dal 1° settembre 2004 che dal 1° gennaio 2007\"";
                    return false;
                }
            }

            if (soggettoBeneficiario.GetValueOrDefault() == 5 || soggettoBeneficiario.GetValueOrDefault() == 8)
            {
                if (tipologiaPrestazione.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge\" o \"Figlio\" la Tipologia di Prestazione può essere solo \"Art. 2 e 3 L. 206/2004\"";
                    return false;
                }

                if (tipologiaBeneficio.GetValueOrDefault() != 2)
                {
                    messaggioVideo = "Per Soggetto Beneficiario \"Coniuge\" o \"Figlio\" la Tipologia di Beneficio può essere solo \"Benefici art. 2-3 con decorrenza dal 1° gennaio 2007\"";
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsDecorrenzaEventoTerroristico(DateTime? dataEventoTerroristico, DateTime dataPresentazioneDomanda, char? codiceEvento, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (dataEventoTerroristico.HasValue && codiceEvento == 'I' && (!Utility.DataSuccessivaA(dataEventoTerroristico.Value, new DateTime(1961, 1, 1)) ||
                Utility.DataSuccessivaA(dataEventoTerroristico.Value, dataPresentazioneDomanda)))
            {
                messaggioVideo = string.Format("Se il Codice Evento è ITALIA la Data Evento Terroristico deve essere successiva al 01/01/1961 e antecedente alla data di presentazione della domanda ({0:dd/MM/yyyy})", dataPresentazioneDomanda);
                return false;
            }

            return true;
        }

        public static bool ControlsDatiCalcoloVittimeTerrorismoWithVisibility(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloContributivo> lDatiCalcoloContributivo,
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo, long? soggettoBeneficiario, long? tipologiaPrestazione, long? tipologiaBeneficio,
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (listaDatiCalcoloVittimeTerrorismo != null && listaDatiCalcoloVittimeTerrorismo.Count > 0)
            {
                if (!Utility.IsDatiImportoPensioneVittimeVisible(datiPensione, soggettoBeneficiario, tipologiaPrestazione, tipologiaBeneficio) &&
                    listaDatiCalcoloVittimeTerrorismo.Exists(x => x.Tipo == 'I'))
                {
                    messaggioVideo = "Non è possibile acquisire i dati Importo Pensione Vittime se il Soggetto Beneficiario è diverso da Vittima.";
                    return false;
                }
            }

            return true;
        }
        #endregion Vittime terrorismo

        #region Cieco/Ex Combattente
        #region PCIPL35
        public static bool ControlsDecorrenzaMaggiorazioneArt6(DateTime? decorrenzaMaggiorazioneArt6, byte? codiceCieco, string gruppo, DateTime? decorrenzaOriginaria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;

            if (!decorrenzaMaggiorazioneArt6.HasValue)
            {
                if (codiceCieco.GetValueOrDefault() == 1 || codiceCieco.GetValueOrDefault() == 9)
                {
                    messaggioVideo = "Decorrenza Art.6 L.140/544 mancante per Cieco/Combattente = 8/9";
                    return false;
                }
            }
            else
            {
                if (Utility.DataStrettamenteSuccessivaA(decorrenzaMaggiorazioneArt6.Value, dataSistema))
                {
                    messaggioVideo = "Decorrenza art.6 L.140/544 illogica o posteriore a data odierna";
                    return false;
                }

                if (!gruppo.Equals("0003"))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneArt6.Value, decorrenzaOriginaria.Value) ||
                        !Utility.DataSuccessivaA(decorrenzaMaggiorazioneArt6.Value, new DateTime(1985, 01, 01)))
                    {
                        messaggioVideo = "Decorrenza art.6 L.140/544 anteriore a Decorrenza Originaria o 01/1985";
                        return false;
                    }
                }

                if (gruppo.Equals("0003"))
                {
                    if (!Utility.DataSuccessivaA(decorrenzaMaggiorazioneArt6.Value, new DateTime(1985, 01, 01)))
                    {
                        messaggioVideo = "Decorrenza art.6 L.140/544 anteriore a 01/1985";
                        return false;
                    }
                }

                if (codiceCieco.GetValueOrDefault() != 1 && codiceCieco.GetValueOrDefault() != 9)
                {
                    messaggioVideo = "Decorrenza art.6 L.140/544 incompatibile con Cieco/Combattente = 0";
                    return false;
                }
            }

            return true;
        }
        #endregion PCIPL35

        #region PCIPL29

        /// <summary>
        /// Se il campo (IW1CODEX (CODICE EX-COMBAT.) NOT = 0 AND NOT = 1 AND  NOT = 9) allora segnala errore  "CODICE ART.6/140 ERRATO (0 - 8 - 9)"
        /// </summary>
        /// <param name="codiceCieco"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaCodiceCiecoArt6(byte? codiceCieco, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceCieco.HasValue && codiceCieco.Value != 0 && codiceCieco.Value != 1 && codiceCieco.Value != 9)
            {
                messaggioVideo = "CODICE ART.6/140 ERRATO (0 - 8 - 9)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se il campo (IW1DECEX (DEC. ART.6,LG.140/85)  > 0 AND < 198501) segnala errore "DECORRENZA ART.6/140 ANTERIORE AL 1985" 
        /// </summary>
        /// <param name="decorrenzaArt6"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool VerificaDecorrenzaArt6(DateTime? decorrenzaArt6, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataCompare = new DateTime(1985, 01, 01);

            if (decorrenzaArt6.HasValue && !Utility.DataSuccessivaA(decorrenzaArt6.Value, dataCompare))
            {
                messaggioVideo = "DECORRENZA ART.6/140 ANTERIORE AL 1985";
                return false;
            }

            return true;
        }

        public static bool VerificaCodiceCiecoWithDecorrenza(byte? codiceCieco, DateTime? decorrenzaArt6, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (codiceCieco.HasValue && codiceCieco.Value != 0 && !decorrenzaArt6.HasValue)
            {
                messaggioVideo = "IMPORTO ART.6/140 ERRATO";
                return false;
            }

            return true;
        }

        #endregion PCIPL29

        #endregion Cieco/Ex Combattente
        #endregion Maggiorazioni e Benefici

        #region Bititolarità

        public static bool ControlsBititolarita(GestionePensione.DatiPensione datiPensione, List<Entity.AltraPensione> LaltraPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (LaltraPensione == null || LaltraPensione.Count == 0)
                return true;

            foreach (Entity.AltraPensione dati in LaltraPensione)
            {
                int categoriaNumerica = 0;
                int.TryParse(dati.Categoria, out categoriaNumerica);
                if (categoriaNumerica != 0)
                {
                    if (dati.Categoria.Length != 3)
                    {
                        messaggioVideo = "Il 'Codice Categoria' deve essere lungo 3";
                        return false;
                    }
                }
            }

            #region Recupero Dati
            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione mancanti";
                return false;
            }

            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            GetCodiciNatura(datiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);
            string filtro = string.Empty;
            string siglaCategoria = string.Empty;
            GetFiltro_SiglaCategoria(datiPensione, out filtro, out siglaCategoria);
            #endregion Recupero Dati

            foreach (Entity.AltraPensione aP in LaltraPensione)
            {
                int categoriaNumerica = 0;
                int.TryParse(aP.Categoria, out categoriaNumerica);

                //Se categoria = "070" OR "071" OR "072" il codice importo deve essere 0.
                if ((categoriaNumerica == 70 || categoriaNumerica == 71 || categoriaNumerica == 72) && aP.CodiceImporto.HasValue && aP.CodiceImporto.Value != '0')
                {
                    messaggioVideo = "Per la categoria " + aP.Categoria + " il codice importo deve essere 0";
                    return false;
                }
            }

            //Verifica se il 1° codice natura = 2, 4, 6 quando vengono 
            //segnalate solo pensioni 044 077 078 In tal caso dare errore con messaggio per tornare indietro cambiare il 1° codice natura
            if (LaltraPensione.Find(x => x.Categoria.Trim() != "044" && x.Categoria.Trim() != "077" && x.Categoria.Trim() != "078") == null &&
                (codNat1 == '2' || codNat1 == '4' || codNat1 == '6'))
            {
                messaggioVideo = "Volendo acquisire solo pensioni 044, 077, 078, è necessario modificare il primo codice natura dato che quello corrente (" +
                    codNat1 + ") non è ammesso";
                return false;
            }

            //I controlli seguenti scattano se la pensione non è: VOBIS, IOBIS, VMP, IMP, INVCIV, INDCOM, 
            //PS, AS, PSO, VOP, IOP, SOP, VOAUT, IOAUT, SOAUT, VOBANC, IOBANC, SOBANC.
            switch (siglaCategoria)
            {
                //case "VOBIS":
                //case "IOBIS":
                //case "VMP":
                //case "IMP":
                //case "INVCIV":
                //case "INDCOM":
                //case "PS":
                //case "AS":
                //case "PSO":
                //case "VOP":
                //case "IOP":
                //case "SOP":
                //case "VOAUT":
                //case "IOAUT":
                //case "SOAUT":
                //case "VOBANC":
                //case "IOBANC":
                //case "SOBANC":
                //    break;
                default:
                    if (!ControlsUlterioriBititolarita(datiPensione, LaltraPensione, out messaggioVideo))
                        return false;
                    break;
            }

            return true;
        }

        private static bool ControlsUlterioriBititolarita(GestionePensione.DatiPensione datiPensione, List<Entity.AltraPensione> LaltraPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (LaltraPensione == null || LaltraPensione.Count == 0)
                return true;

            #region Recupero Dati

            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione mancanti";
                return false;
            }

            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            GetCodiciNatura(datiPensione.NaturaPensione, out codNat1, out codNat2, out codNat3);
            string filtro = string.Empty;
            string siglaCategoria = string.Empty;
            GetFiltro_SiglaCategoria(datiPensione, out filtro, out siglaCategoria);
            int pdirsup = 0;
            int plavaut = 0;
            GetPdirusp_Plavaut(datiPensione, out pdirsup, out plavaut);
            string certificato = string.Empty;
            DateTime decorrenzaOriginaria = DateTime.MinValue;
            GetCertificato_DecorrenzaPensione(datiPensione, out decorrenzaOriginaria, out certificato);
            #endregion Recupero Dati
            bool nonAmmessa = false;
            bool? presente = null;
            bool? presenteG = null;
            bool? presenteL = null;
            bool? presenteR = null;
            bool? presenteA = null;
            bool? presenteT = null;
            //E’ possibile acquisire le bititolarità se il primo codice natura è pari a 2, 4, 5, 6, 9
            if (codNat1 != '2' && codNat1 != '4' && codNat1 != '5' && codNat1 != '6' && codNat1 != '9')
            {
                messaggioVideo = "Non è possibile acquisire le bititolarità se il primo codice natura è pari a " + codNat1;
                return false;
            }

            foreach (Entity.AltraPensione aP in LaltraPensione)
            {
                if (!GestioneAltrePensioni.VerifyCtrlBititolarita(aP.Categoria, aP.CodiceUC.GetValueOrDefault(), aP.CodiceImporto.GetValueOrDefault(), Utility.TipoAppartenenza.CI))
                {
                    messaggioVideo = "La combinazione categoria (" + (!string.IsNullOrEmpty(aP.Categoria) ? aP.Categoria.ToUpperInvariant() : " ") + ") codice U/C (" + aP.CodiceUC.GetValueOrDefault() +
                        ") e codice importo (" + aP.CodiceImporto.GetValueOrDefault() + ") non è ammessa";
                    return false;
                }

                //Categoria = "070"
                //Implica che importo = "0” con PDIRSUP = 0 e RAU1031 = 5
                if (aP.Categoria == "070" && codNat1 == '5' && pdirsup == 0)
                {
                    if (aP.CodiceImporto.HasValue && aP.CodiceImporto.Value != '0')
                    {
                        messaggioVideo = "Per la categoria " + aP.Categoria + " il codice importo deve essere pari a 0";
                        return false;
                    }
                }

                //Categoria = "070" OR "071"
                //Implica che importo = "0” con PDIRSUP = 1 e RAU1031 = 2,4,5,6,9
                if ((aP.Categoria == "070" || aP.Categoria == "071") &&
                    (codNat1 == '2' || codNat1 == '4' || codNat1 == '5' || codNat1 == '6' || codNat1 == '9') &&
                    pdirsup == 1)
                {
                    if (aP.CodiceImporto.HasValue && aP.CodiceImporto.Value != '0')
                    {
                        messaggioVideo = "Per la categoria " + aP.Categoria + " il codice importo deve essere pari a 0";
                        return false;
                    }
                }

                if (aP.Categoria == "024" &&
                    (certificato.Substring(0, 3) == "001" || certificato.Substring(0, 3) == "004" ||
                    certificato.Substring(0, 3) == "011" || certificato.Substring(0, 3) == "012"))
                    nonAmmessa = true;
                if (aP.Categoria == "037" &&
                   (certificato.Substring(0, 3) == "091" || certificato.Substring(0, 3) == "092"))
                    nonAmmessa = true;
                if (aP.Categoria == "040" &&
                   (certificato.Substring(0, 3) == "094" || certificato.Substring(0, 3) == "095"))
                    nonAmmessa = true;
                if ((aP.Categoria == "045" || aP.Categoria == "048" || aP.Categoria == "051" ||
                    aP.Categoria == "054" || aP.Categoria == "057" || aP.Categoria == "060" ||
                    aP.Categoria == "063" || aP.Categoria == "066" || aP.Categoria == "079" ||
                    aP.Categoria == "094" || aP.Categoria == "097") &&
                    (certificato.Substring(0, 3) == "001" || certificato.Substring(0, 3) == "002"))
                    nonAmmessa = true;
                if (aP.Categoria == "069" &&
                   (certificato.Substring(0, 3) == "091" || certificato.Substring(0, 3) == "094" ||
                    certificato.Substring(0, 3) == "097"))
                    nonAmmessa = true;

                //SE IL SECONDO SOTTOCAMPO DEL CODICE NATURA PENSIONE (RAU1032)
                //* E' UGUALE A "G" DEVE ESSERE PRESENTE UN'ALTRA PENSIONE CON CO-
                //* DICE CATEGORIA UGUALE A: " OA" " BA" " HA" " LA" " NA" E LA
                //* DECORRENZA (VNBI04) DEVE ESSERE MINORE O UGUALE A 8104, LA CES-
                //* SAZIONE (VNBI08) SE PRESENTE DEVE ESSERE MAGGIORE O UGUALE A
                //* 8104 E LA DECORRENZA ORIGINARIA DELLA PENSIONE (RAU104) DEVE
                //* ESSERE MINORE DI 8104.
                if (codNat2 == 'G')
                {
                    if (!presenteG.HasValue)
                        presenteG = false;
                    switch (aP.Categoria)
                    {
                        case "OA":
                        case "BA":
                        case "HA":
                        case "LA":
                        case "NA":
                            if (aP.Decorrenza.HasValue && !Utility.DataStrettamenteSuccessivaA(aP.Decorrenza.Value, new DateTime(1981, 4, 1)) &&
                                !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(1981, 4, 1)) &&
                                (!aP.Cessazione.HasValue || Utility.DataStrettamenteSuccessivaA(aP.Cessazione.Value, new DateTime(1981, 4, 1))))
                                presenteG = true;
                            break;
                    }
                }

                //YF: SE IL SECONDO SOTTOCAMPO DEL CODICE NATURA PENSIONE (RAU1032)
                //* E' UGUALE A "L" DEVE ESSERE PRESENTE UN'ALTRA PENSIONE CON
                //* CODICE CATEGORIA (VNBI01) UGUALE : " OA" " BA" " HA" " LA"
                //* E LA DECORRENZA (VNBI04) DEVE ESSERE MINORE O UGUALE A 8207,
                //* CESSAZIONE (VNBI08) SE PRESENTE, DEVE ESSERE MAGGIORE O UGUALE
                //* A 8207 E LA DECORRENZA ORIGINARIA DELLA PENSIONE (RAU104) DEVE
                //* ESSERE MINORE A 8207.
                if (codNat2 == 'L')
                {
                    if (!presenteL.HasValue)
                        presenteL = false;
                    switch (aP.Categoria)
                    {
                        case "OA":
                        case "BA":
                        case "HA":
                        case "LA":
                            if (aP.Decorrenza.HasValue && !Utility.DataStrettamenteSuccessivaA(aP.Decorrenza.Value, new DateTime(1982, 7, 1)) &&
                                !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(1982, 7, 1)) &&
                                (!aP.Cessazione.HasValue || Utility.DataStrettamenteSuccessivaA(aP.Cessazione.Value, new DateTime(1982, 7, 1))))
                                presenteL = true;
                            break;
                    }
                }

                //YG: SE IL SECONDO SOTTOCAMPO DEL CODICE NATURA PENSIONE (RAU1032)
                //* E' UGUALE A "R" DEVE ESSERE PRESENTE UN'ALTRA PENSIONE CON CO-
                //* DICE CATEGORIA (VNBI01) = "047" "056" "059" "065" " OR" " BR"
                //*                           " CR" " GR" " HR" " LR" " NR"    E
                //* LA DECORRENZA (VNBI04) DEVE ESSERE MINORE O UGUALE A 7406, LA
                //* CESSAZIONE (VNBI08) SE PRESENTE DEVE ESSERE MAGGIORE O UGUALE
                //* A 7406 E LA DECORRENZA ORIGINARIA DELLA PENSIONE DEVE ESSERE
                //* MINORE A 7406.
                if (codNat2 == 'R')
                {
                    if (!presenteR.HasValue)
                        presenteR = false;
                    switch (aP.Categoria)
                    {
                        case "047":
                        case "056":
                        case "059":
                        case "065":
                        case "OR":
                        case "BR":
                        case "CR":
                        case "GR":
                        case "HR":
                        case "LR":
                        case "NR":
                            if (aP.Decorrenza.HasValue && !Utility.DataStrettamenteSuccessivaA(aP.Decorrenza.Value, new DateTime(1974, 6, 1)) &&
                                !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(1974, 6, 1)) &&
                                (!aP.Cessazione.HasValue || Utility.DataStrettamenteSuccessivaA(aP.Cessazione.Value, new DateTime(1974, 6, 1))))
                                presenteR = true;
                            break;
                    }
                }

                //YJ: SE IL SECONDO CAMPO DEL CODICE NATURA PENSIONE RAU1032 = "A"
                //* LA DECORRENZA ORIGINARIA DELLA PENSIONE RAU104 DEVE ESSERE
                //* MINORE A 8310 E DEVE ESISTERE UN'ALTRA PENSIONE CON DECORRENZA
                //* VNBI08 MINORE 8310
                if (codNat2 == 'A')
                {
                    if (!presenteA.HasValue)
                        presenteA = false;
                    if (aP.Decorrenza.HasValue && !Utility.DataStrettamenteSuccessivaA(aP.Decorrenza.Value, new DateTime(1983, 10, 1)) &&
                                !Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria, new DateTime(1983, 10, 1)))
                        presenteA = true;
                }

                //YM: Se il 1° codice natura è uguale a 5 = Supplementare
                //* non può essere segnalata una categoria uguale
                //* a quella in liquidazione
                string catNum = string.Empty;
                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(siglaCategoria, out catNum);
                if (codNat1 == '5' && catNum == aP.Categoria)
                {
                    messaggioVideo = "Essendo il primo codice natura pari a 5 non può essere inserita una categoria altra pensione " + aP.Categoria +
                        " coincidente con la categoria pensione " + siglaCategoria;
                    return false;
                }

                if (LaltraPensione.Count(x => x.Categoria == aP.Categoria && x.Certificato == aP.Certificato) > 1)
                {
                    messaggioVideo = " Non sono ammesse altre pensioni con stessa categoria " + aP.Categoria +
                        " e certificato " + aP.Certificato;
                    return false;
                }

                if (LaltraPensione.Count(x => x.Categoria == aP.Categoria && x.CodiceUC == aP.CodiceUC && x.CodiceImporto == aP.CodiceImporto) > 1 ||
                    LaltraPensione.Count(x => x.Categoria == aP.Categoria && aP.Cessazione.HasValue && x.Decorrenza != aP.Cessazione) > 1)
                {
                    switch (aP.Categoria)
                    {
                        case "003":
                        case "009":
                        case "012":
                        case "014":
                        case "017":
                        case "020":
                        case "023":
                        case "034":
                        case "075":
                        case "084":
                            if (LaltraPensione.Count(x => x.Categoria == aP.Categoria && x.CodiceUC == aP.CodiceUC && x.CodiceImporto == aP.CodiceImporto) > 2 ||
                                LaltraPensione.Count(x => x.Categoria == aP.Categoria && aP.Cessazione.HasValue && x.Decorrenza != aP.Cessazione) > 2)
                            {
                                messaggioVideo = "Non sono ammesse più di due altre pensioni con stessa categoria " + aP.Categoria +
                                    " e codice U/C e codice importo oppure con stessa categoria e cessazione e decorrenza della precedente differenti";
                                return false;
                            }
                            break;
                        case "078":
                        case "079":
                            if (aP.Certificato.HasValue && aP.Certificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "3" ||
                                aP.Certificato.HasValue && aP.Certificato.Value.ToString().PadLeft(8, '0').Substring(2, 1) == "6")
                            {
                                if (LaltraPensione.Count(x => x.Categoria == aP.Categoria && x.CodiceUC == aP.CodiceUC && x.CodiceImporto == aP.CodiceImporto) > 2 ||
                                    LaltraPensione.Count(x => x.Categoria == aP.Categoria && aP.Cessazione.HasValue && x.Decorrenza != aP.Cessazione) > 2)
                                {
                                    messaggioVideo = "Non sono ammesse altre pensioni con stessa categoria " + aP.Categoria +
                                    " e codice U/C e codice importo oppure con stessa categoria e cessazione e decorrenza della precedente differenti";
                                    return false;
                                }
                            }
                            else
                            {
                                messaggioVideo = "Non sono ammesse altre pensioni con stessa categoria " + aP.Categoria +
                                " e codice U/C e codice importo oppure con stessa categoria e cessazione e decorrenza della precedente differenti";
                                return false;
                            }
                            break;
                        default:
                            messaggioVideo = "Non sono ammesse altre pensioni con stessa categoria " + aP.Categoria +
                                " e codice U/C e codice importo oppure con stessa categoria e cessazione e decorrenza della precedente differenti";
                            return false;
                    }
                }
            }

            if (presente.HasValue && !presente.Value)
            {
                messaggioVideo = "Essendo il primo codice natura pari a 5 e la categoria della pensione pari a " + siglaCategoria +
                    " mancano altre pensioni obbligatorie con decorrenza inferiore alla decorrenza originaria";
                return false;
            }

            if (presenteG.HasValue && !presenteG.Value)
            {
                messaggioVideo = "Essendo il secondo codice natura pari a G" +
                    " mancano altre pensioni obbligatorie (OA, BA, HA, LA, NA) con decorrenza  e/o decorrenza pensione inferiore ad Aprile 1981 e " +
                    "cessazione, se presente maggiore ad Aprile 1981";
                return false;
            }

            if (presenteL.HasValue && !presenteL.Value)
            {
                messaggioVideo = "Essendo il secondo codice natura pari a L" +
                    " mancano altre pensioni obbligatorie (OA, BA, HA, LA) con decorrenza  e/o decorrenza pensione inferiore a Luglio 1982 e " +
                    "cessazione, se presente maggiore a Luglio 1982";
                return false;
            }

            if (presenteR.HasValue && !presenteR.Value)
            {
                messaggioVideo = "Essendo il secondo codice natura pari a R" +
                    " mancano altre pensioni obbligatorie (047, 056, 059, 065, OR, BR, CR, GR, HR, LR, NR) con decorrenza  e/o decorrenza pensione inferiore a Giugno 1974 e " +
                    "cessazione, se presente maggiore a Giugno 1974";
                return false;
            }

            if (presenteA.HasValue && !presenteA.Value)
            {
                messaggioVideo = "Essendo il secondo codice natura pari ad A" +
                    " mancano altre pensioni obbligatorie con decorrenza  e/o decorrenza pensione inferiore ad Ottobre 1983";
                return false;
            }

            if (presenteT.HasValue && !presenteT.Value)
            {
                messaggioVideo = "Essendo il secondo codice natura pari ad T e la categoria della pensione pari a " + siglaCategoria +
                    " mancano altre pensioni obbligatorie (047, 056, 059, 065, OA, OR, , BA, BR, CA, CR, GA, GR, HA, HR, LA, LR, NA, NR)";
                return false;
            }

            List<Entity.AltraPensione> LaltraPensioneNonCessate = LaltraPensione.FindAll(x => !x.Cessazione.HasValue);
            if (LaltraPensioneNonCessate != null && LaltraPensioneNonCessate.Count > 1)
            {
                string altraPensione1 = string.Empty;
                string altraPensione2 = string.Empty;
                for (int i = 0; i < LaltraPensioneNonCessate.Count; i++)
                {
                    if (nonAmmessa)
                        break;
                    altraPensione1 = LaltraPensioneNonCessate[i].Categoria;
                    for (int j = 0; j < LaltraPensioneNonCessate.Count; j++)
                    {
                        if (nonAmmessa)
                            break;
                        if (i != j)
                        {
                            altraPensione2 = LaltraPensioneNonCessate[j].Categoria;
                            switch (LaltraPensioneNonCessate[i].Categoria)
                            {
                                case "001":
                                case "002":
                                case "004":
                                case "005":
                                case "077":
                                case "078":
                                case "007":
                                case "008":
                                case "013":
                                case "018":
                                case "019":
                                case "021":
                                case "022":
                                case "079":
                                case "080":
                                case "082":
                                case "083":
                                case "088":
                                case "089":
                                case "091":
                                case "092":
                                    switch (LaltraPensioneNonCessate[j].Categoria)
                                    {
                                        case "001":
                                        case "002":
                                        case "004":
                                        case "077":
                                        case "078":
                                        case "005":
                                        case "007":
                                        case "008":
                                        case "018":
                                        case "019":
                                        case "021":
                                        case "022":
                                        case "037":
                                        case "038":
                                        case "040":
                                        case "041":
                                        case "048":
                                        case "049":
                                        case "060":
                                        case "061":
                                        case "079":
                                        case "080":
                                        case "082":
                                        case "083":
                                        case "088":
                                        case "089":
                                        case "091":
                                        case "092":
                                        case "095":
                                        case "098":
                                            nonAmmessa = true;
                                            break;
                                        case "014":
                                        case "016":
                                        case "085":
                                        case "086":
                                            switch (LaltraPensioneNonCessate[i].Categoria)
                                            {
                                                case "018":
                                                case "019":
                                                case "021":
                                                case "022":
                                                case "088":
                                                case "089":
                                                case "091":
                                                case "092":
                                                    nonAmmessa = true;
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "010":
                                case "011":
                                    switch (LaltraPensioneNonCessate[j].Categoria)
                                    {
                                        case "010":
                                        case "011":
                                            nonAmmessa = true;
                                            break;
                                    }
                                    break;
                                case "015":
                                case "016":
                                case "085":
                                case "086":
                                    switch (LaltraPensioneNonCessate[j].Categoria)
                                    {
                                        case "037":
                                        case "038":
                                        case "040":
                                        case "041":
                                        case "048":
                                        case "049":
                                        case "060":
                                        case "061":
                                        case "095":
                                        case "098":
                                        case "014":
                                        case "016":
                                        case "085":
                                        case "086":
                                            nonAmmessa = true;
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (nonAmmessa)
                {
                    messaggioVideo = "Incompatibilità tra categoria " + altraPensione1 + " non cessata con categoria " + altraPensione2 + " non cessata";
                    return false;
                }
            }


            return true;
        }

        #region GetCommonData
        internal static void GetFiltro_SiglaCategoria(GestionePensione.DatiPensione datiPensione, out string filtro, out string siglaCategoria)
        {
            filtro = string.Empty;
            siglaCategoria = string.Empty;

            siglaCategoria = datiPensione.SiglaCategoria.Trim().ToUpperInvariant();

            filtro = datiPensione.GetFiltro();
        }

        internal static void GetCodiciNatura(string naturaPensione, out char codNat1, out char codNat2, out char codNat3)
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

        internal static void GetPdirusp_Plavaut(GestionePensione.DatiPensione datiPensione, out int pdirsup, out int plavaut)
        {
            pdirsup = 0;
            plavaut = 0;
            string filtro = string.Empty;
            string siglaCategoria = string.Empty;
            DateTime decorrenzaOriginaria = DateTime.MinValue;
            string certificato = string.Empty;

            GetCertificato_DecorrenzaPensione(datiPensione, out decorrenzaOriginaria, out certificato);
            GetFiltro_SiglaCategoria(datiPensione, out filtro, out siglaCategoria);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione) ||
              (siglaCategoria == "PMO" && (certificato.Substring(2, 1) == "3" || certificato.Substring(2, 1) == "6")))
                pdirsup = 1;

            switch (siglaCategoria)
            {
                case "VO":
                case "IO":
                case "SO":
                case "VOP":
                case "SOP":
                case "VOMIN":
                case "SOMIN":
                case "PMO":
                case "VOBANC":
                case "IOBANC":
                case "SOBANC":
                case "VOSPED":
                case "IOSPED":
                case "SOSPED":
                case "VDAI":
                case "IDAI":
                case "SDAI":
                case "VOTOT":
                case "IOTOT":
                case "SOTOT":
                    plavaut = 1;
                    break;
                default:
                    break;
            }
        }

        internal static void GetPanvein_Posticipo(string naturaPensione, GestionePensione.DatiPensione datiPensione, out int panvein, out bool posticipo)
        {
            char codNat1 = ' ';
            char codNat2 = ' ';
            char codNat3 = ' ';
            panvein = 0;
            posticipo = false;
            string filtro = string.Empty;
            string siglaCategoria = string.Empty;
            DateTime decorrenzaOriginaria = DateTime.MinValue;
            string certificato = string.Empty;

            GetCodiciNatura(naturaPensione, out codNat1, out codNat2, out codNat3);
            GetCertificato_DecorrenzaPensione(datiPensione, out decorrenzaOriginaria, out certificato);
            GetFiltro_SiglaCategoria(datiPensione, out filtro, out siglaCategoria);

            switch (siglaCategoria)
            {
                case "VO":
                case "VOTOT":
                case "VOP":
                case "VOMIN":
                case "VOBANC":
                case "VR":
                case "VOART":
                case "VOCOM":
                case "VODAI":
                    if (codNat1 != '1' && codNat1 != '2' && codNat1 != '3' && codNat1 != '4')
                        panvein = 1;
                    break;
                case "VOAUT":
                    panvein = 1;
                    break;
                default:
                    break;
            }

            if ((siglaCategoria.StartsWith("I") && siglaCategoria != "INDCOM") ||
            (siglaCategoria == "PMO" && (certificato.Substring(2, 1) == "2" || certificato.Substring(2, 1) == "5")))
            {
                panvein = 2;
            }

            switch (filtro)
            {
                case "SCO":
                case "BNS":
                case "BNX":
                    posticipo = true;
                    break;
                default:
                    break;
            }
        }

        internal static void GetCertificato_DecorrenzaPensione(GestionePensione.DatiPensione datiPensione, out DateTime decorrenzaOriginaria, out string certificato)
        {
            certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "00000000";
            decorrenzaOriginaria = datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria.Value : DateTime.MinValue;
        }

        internal static void GetDauCtrDai(GestionePensione.DatiPensione datiPensione, ref string dauCtrDai)
        {
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcContr = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out listaCalcContr);
            if (listaCalcContr != null && listaCalcContr.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo c in listaCalcContr)
                {
                    if (c.ImportoContributivoTotale.HasValue && c.ImportoContributivoTotale.Value > 0)
                    {
                        dauCtrDai = "S";
                        return;
                    }
                }
            }

            if (dauCtrDai == "N")
            {
                List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcRetr = null;
                GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaCalcRetr);
                if (listaCalcRetr != null && listaCalcRetr.Count > 0)
                {
                    foreach (GestioneCalcolo.DatiCalcoloRetributivo r in listaCalcRetr)
                    {
                        if ((r.NSettimaneQuotaA.HasValue && r.NSettimaneQuotaA.Value > 0) ||
                            (r.NSettimaneQuotaB.HasValue && r.NSettimaneQuotaB.Value > 0))
                        {
                            dauCtrDai = "S";
                            return;
                        }
                    }
                }
            }
        }

        internal static int GetAnniTrascorsi(DateTime dataIniziale, DateTime dataFinale)
        {
            int anni = 0;

            anni = dataIniziale.Year - dataFinale.Year;
            if (dataIniziale.Month < dataFinale.Month)
                anni = anni - 1;
            return anni;
        }
        #endregion GetCommonData

        #region PCIPL92

        /// <summary>
        /// Controllo tra la categoria e il codice ente
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="codiceEnte"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool CI_ControlsCategoriaWithCodiceEnteAltraPensione(string categoria, byte? codiceEnte, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                messaggioVideo = "Categorie mancante";
                return false;
            }

            if (!codiceEnte.HasValue)
            {
                messaggioVideo = "Codice Ente mancante";
                return false;
            }

            switch (categoria.Trim())
            {
                case "OA":
                case "OR":
                case "BA":
                case "BR":
                case "CA":
                case "CR":
                case "EA":
                case "ER":
                case "FA":
                case "FR":
                case "GA":
                case "GR":
                case "HA":
                case "HR":
                case "LA":
                case "LR":
                case "NA":
                case "NR":
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "30":
                case "31":
                case "32":
                case "34":
                case "35":
                case "36":
                case "43":
                case "44":
                case "69":
                case "76":
                case "77":
                case "78":
                    if (codiceEnte.Value < 1 || codiceEnte.Value > 9)
                    {
                        messaggioVideo = "Codice Ente errato (1/9).";
                        return false;
                    }
                    break;
                default:
                    messaggioVideo = "Categoria Altra Pensione errata.";
                    return false;
            }

            int cat = 0;
            int.TryParse(categoria.Trim(), out cat);

            if (codiceEnte.Value == 1)
            {
                if (!((cat > 0 && cat < 24) || (cat > 36 && cat < 43) || (cat > 47 && cat < 51) || (cat > 59 && cat < 63) || (cat > 72 && cat < 76) || (cat > 69 && cat < 76) || (cat > 78)))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 2)
            {
                if (!((cat > 44 && cat < 48) || (cat > 50 && cat < 60) || (cat > 62 && cat < 69)))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 3)
            {
                if (!categoria.Trim().Equals("OA") && !categoria.Trim().Equals("OR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 4)
            {
                if (!categoria.Trim().Equals("BA") && !categoria.Trim().Equals("BR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 5)
            {
                if (!categoria.Trim().Equals("CA") && !categoria.Trim().Equals("CR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 6)
            {
                if (!categoria.Trim().Equals("EA") && !categoria.Trim().Equals("ER"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 7)
            {
                if (!categoria.Trim().Equals("FA") && !categoria.Trim().Equals("FR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 8)
            {
                if (!categoria.Trim().Equals("HA") && !categoria.Trim().Equals("HR") && !categoria.Trim().Equals("LA") && !categoria.Trim().Equals("LR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            if (codiceEnte.Value == 9)
            {
                if (!categoria.Trim().Equals("NA") && !categoria.Trim().Equals("NR"))
                {
                    messaggioVideo = "Codice Ente errato rispetto alla categoria.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Controllo tra categorie e codice U/C
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="codiceUC"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool CI_ControlsCategoriaWithCodiceUCAltraPensione(string categoria, char? codiceUC, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                messaggioVideo = "Categorie mancante";
                return false;
            }

            if (!codiceUC.HasValue)
            {
                messaggioVideo = "Codice U/C mancante";
                return false;
            }

            if (codiceUC.Value == 'C')
            {
                switch (categoria.Trim())
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "10":
                    case "11":
                    case "13":
                    case "15":
                    case "16":
                    case "18":
                    case "19":
                    case "21":
                    case "22":
                    case "37":
                    case "38":
                    case "40":
                    case "41":
                    case "45":
                    case "46":
                    case "48":
                    case "49":
                    case "51":
                    case "52":
                    case "54":
                    case "55":
                    case "57":
                    case "58":
                    case "60":
                    case "61":
                    case "63":
                    case "64":
                    case "66":
                    case "67":
                    case "79":
                    case "70":
                    case "71":
                    case "73":
                    case "74":
                    case "80":
                    case "82":
                    case "83":
                    case "85":
                    case "86":
                    case "88":
                    case "89":
                    case "91":
                    case "92":
                    case "94":
                    case "95":
                    case "97":
                    case "98":
                    case "OA":
                    case "BA":
                    case "CA":
                    case "EA":
                    case "FA":
                    case "HA":
                    case "LA":
                    case "NA":
                    case "GA":
                        messaggioVideo = "Valore 'U/C' incompatibile con categoria.";
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Controllo tra categoria e codice importo
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="codiceImporto"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool CI_ControlsCategoriaWithCodiceImportoAltraPensione(string categoria, char? codiceImporto, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                messaggioVideo = "Categorie mancante";
                return false;
            }

            if (!codiceImporto.HasValue)
            {
                messaggioVideo = "Codice Importo mancante";
                return false;
            }

            if (codiceImporto.Value == '0')
            {
                if (!CategorieCompareCodiceImporto(categoria, out messaggioVideo))
                    return false;
            }

            if (codiceImporto.Value == '1' || codiceImporto.Value == '2' || codiceImporto.Value == '3')
            {
                if (!CategorieCompareCodiceImporto(categoria, out messaggioVideo))
                    return false;
            }


            if (categoria.Trim().Equals("73") || categoria.Trim().Equals("74") || categoria.Trim().Equals("75"))
            {
                if (codiceImporto.Value != '2' && codiceImporto.Value != '3')
                {
                    messaggioVideo = "Codice Importo incompatibile con categoria.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compara le categorie con il codice importo
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool CategorieCompareCodiceImporto(string categoria, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            switch (categoria.Trim())   //uguale a quello sopra. si può ottimizzare
            {
                case "66":
                case "67":
                case "68":
                case "94":
                case "95":
                case "96":
                case "97":
                case "98":
                case "99":
                case "OA":
                case "OR":
                case "BA":
                case "BR":
                case "CA":
                case "CR":
                case "EA":
                case "ER":
                case "FA":
                case "FR":
                case "HA":
                case "HR":
                case "LA":
                case "LR":
                case "GA":
                case "GR":
                case "NA":
                case "NR":
                case "70":
                case "71":
                case "72":
                    break;
                default:
                    messaggioVideo = "Codice Importo incompatibile con categoria.";
                    return false;
            }

            return true;
        }

        #endregion PCIPL92

        #region PCIPL15

        public static bool VerificaAltraPensioneWithCategoriaPensione(List<AltraPensione> LaltraPensione, string codiceCategoria, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime dataSistema = Utility.DataSistemaCi;
            if (LaltraPensione == null || LaltraPensione.Count == 0)
            {
                messaggioVideo = "Dati Altra Pensione mancanti";
                return false;
            }

            foreach (AltraPensione altraPensione in LaltraPensione)
            {
                if (string.IsNullOrEmpty(altraPensione.Categoria))
                {
                    if (altraPensione.Certificato.HasValue || altraPensione.Ente.HasValue || altraPensione.CodiceUC.HasValue || altraPensione.CodiceImporto.HasValue || altraPensione.Decorrenza.HasValue || altraPensione.Cessazione.HasValue)
                    {
                        messaggioVideo = "Riga incompleta";
                        return false;
                    }
                }
                else
                {
                    int categoriaNumerica = 0;
                    int.TryParse(altraPensione.Categoria, out categoriaNumerica);
                    switch (altraPensione.Categoria)
                    {
                        case "024":
                        case "037":
                        case "040":
                        case "045":
                        case "048":
                        case "051":
                        case "054":
                        case "057":
                        case "060":
                        case "063":
                        case "066":
                        case "094":
                        case "097":
                            messaggioVideo = "Per la bititolarità dei fondi speciali (024,037,040,045,048,051,054,057,060,063,066,094,097) devono essere utilizzate le lettere, come per le pensioni erogate da altri enti";
                            return false;
                        default:
                            break;
                    }
                    int categoriaPensione = 0;
                    int.TryParse(codiceCategoria, out categoriaPensione);

                    List<GestioneDecodifica.CatEnteAltraPensione> listaCatEnte = null;
                    GestioneDecodifica.GetCatEnteAltrePensioni(out listaCatEnte);

                    if (Utility.IsRicostituzione(datiPensione.Gruppo) && listaCatEnte != null && listaCatEnte.Count > 0)
                    {
                        if (listaCatEnte.Find(x => x.CodCategoria.Trim() == (categoriaNumerica != 0 ? categoriaNumerica.ToString() : altraPensione.Categoria.ToUpperInvariant().Trim()) && x.TipoApp == "CI") == null)
                        {
                            messaggioVideo = "Codice Categoria (" + altraPensione.Categoria.ToUpperInvariant() + ") non riconosciuto.";
                            return false;
                        }
                    }

                    if (categoriaNumerica > 99 && categoriaNumerica != 172 && categoriaNumerica != 170)
                    {
                        messaggioVideo = "Codice categoria errato";
                        return false;
                    }

                    if (categoriaNumerica == categoriaPensione && categoriaNumerica == 6 && categoriaNumerica == 87 && categoriaNumerica == 90 && categoriaNumerica == 93)
                    {
                        messaggioVideo = "Codice categoria uguale a pensione in aquisizione";
                        return false;
                    }

                    if ((altraPensione.Ente == 1 || altraPensione.Ente == 2) && altraPensione.Certificato.GetValueOrDefault() == 0)
                    {
                        messaggioVideo = "Numero certificato mancante";
                        return false;
                    }

                    if (!altraPensione.CodiceUC.HasValue || (!altraPensione.CodiceUC.Equals('C') && !altraPensione.CodiceUC.Equals('U')))
                    {
                        messaggioVideo = "Codice U/C errato o mancate";
                        return false;
                    }

                    if (!altraPensione.CodiceImporto.HasValue)
                    {
                        messaggioVideo = "Codice importo errato o mancante";
                        return false;
                    }

                    DateTime dataCompare = new DateTime(1940, 01, 01);
                    if (!altraPensione.Decorrenza.HasValue || (!Utility.DataSuccessivaA(altraPensione.Decorrenza.Value, dataCompare) ||
                        Utility.DataStrettamenteSuccessivaA(altraPensione.Decorrenza.Value, dataSistema.AddDays((-dataSistema.Day) + 1))))
                    {
                        messaggioVideo = "Decorrenza errata o mancante";
                        return false;
                    }

                    if (altraPensione.Cessazione.HasValue && (Utility.DataStrettamenteSuccessivaA(altraPensione.Cessazione.Value, dataSistema) || !Utility.DataSuccessivaA(altraPensione.Cessazione.Value, altraPensione.Decorrenza.Value)))
                    {
                        messaggioVideo = "Cessazione errata o minore della decorrenza";
                        return false;
                    }

                    if (categoriaNumerica > 0)
                    {
                        if ((categoriaNumerica > 24 && categoriaNumerica < 30) || (categoriaNumerica > 76 && categoriaNumerica < 79))
                        {
                            messaggioVideo = "Codice categoria errato";
                            return false;
                        }
                    }
                    else
                    {
                        switch (altraPensione.Categoria.Trim())
                        {
                            case "OA":
                            case "OR":
                            case "BA":
                            case "BR":
                            case "CA":
                            case "CR":
                            case "EA":
                            case "ER":
                            case "FA":
                            case "FR":
                            case "HA":
                            case "HR":
                            case "LA":
                            case "LR":
                            case "NA":
                            case "NR":
                            case "GA":
                            case "GR":
                                if (altraPensione.Ente < 2)
                                {
                                    messaggioVideo = "Codice ente incompatibile con codice categoria";
                                    return false;
                                }
                                if (altraPensione.CodiceImporto.GetValueOrDefault() == 0)
                                {
                                    messaggioVideo = "Codice importo incompatibile con codici categoria/ente";
                                    return false;
                                }
                                break;
                            default:
                                messaggioVideo = "Codice categoria errato";
                                return false;
                        }
                    }

                    if (altraPensione.Ente.HasValue)
                    {
                        if (altraPensione.Ente.Value == 1)
                        {
                            if (!((categoriaNumerica > 0 && categoriaNumerica < 25) || (categoriaNumerica > 29 && categoriaNumerica < 51) ||
                                (categoriaNumerica > 59 && categoriaNumerica < 63) || (categoriaNumerica > 69 && categoriaNumerica < 77) ||
                                (categoriaNumerica > 78 && categoriaNumerica < 100)))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 2)
                        {
                            if (!((categoriaNumerica > 44 && categoriaNumerica < 48) || (categoriaNumerica > 50 && categoriaNumerica < 60) ||
                                (categoriaNumerica > 62 && categoriaNumerica < 68)))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 3)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("OA") && !altraPensione.Categoria.Trim().Equals("OR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 4)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("BA") && !altraPensione.Categoria.Trim().Equals("BR") && !altraPensione.Categoria.Trim().Equals("GA") && !altraPensione.Categoria.Trim().Equals("GR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 5)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("CA") && !altraPensione.Categoria.Trim().Equals("CR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 6)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("EA") && !altraPensione.Categoria.Trim().Equals("ER"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 7)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("FA") && !altraPensione.Categoria.Trim().Equals("FR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 8)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("FA") && !altraPensione.Categoria.Trim().Equals("FR") && !altraPensione.Categoria.Trim().Equals("LA") && !altraPensione.Categoria.Trim().Equals("LR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }

                        if (altraPensione.Ente.Value == 9)
                        {
                            if (!altraPensione.Categoria.Trim().Equals("NA") && !altraPensione.Categoria.Trim().Equals("NR"))
                            {
                                messaggioVideo = "Codice ente incompatibile con codice categoria";
                                return false;
                            }
                        }
                    }

                    if (altraPensione.CodiceUC.Equals("C"))
                    {
                        List<string> elencoCategorie = new List<string> { "001", "002", "004", "005", "007", "008", "010", "011", "013", "015", "016", "018", "019", "021", "022",
                        "037", "038", "040", "041", "045", "046", "048", "049", "051", "052", "054", "055", "057", "058", "060", "061", "063", "064", "066", "067", "079", "080", "082",
                        "083", "085", "086", "088", "089", "091", "092", "094", "095", "097", "098", "OA", "BA", "CA", "EA", "FE", "HA", "LA", "NA", "GA", "030", "031", "073", "074", "044", "043", "070", "071"};


                        if (!elencoCategorie.Contains(categoriaNumerica != 0 ? categoriaNumerica.ToString() : altraPensione.Categoria))
                        {
                            messaggioVideo = "Codice U/C incompatibile con codice categoria";
                            return false;
                        }
                    }

                    if (altraPensione.CodiceImporto == '0')
                    {
                        List<string> elencoCategorie = new List<string> { "066", "067", "068", "094", "095", "096", "097", "098", "099", "OA", "OR", "BA", "BR", "CA", "CR", "EA", "ER", "FA", "FR", "HA",
                            "HR", "LA", "LR", "GA", "GR", "NA", "NR", "076", "044", "043", "070", "071", "072" };

                        if (!elencoCategorie.Contains(altraPensione.Categoria))
                        {
                            messaggioVideo = "Codice importo incompatibile con codice categoria (" + (altraPensione.Categoria) + ")";
                            return false;
                        }
                    }

                    if (altraPensione.CodiceImporto == '1' || altraPensione.CodiceImporto == '2' || altraPensione.CodiceImporto == '3')
                    {
                        List<string> elencoCategorie = new List<string> { "066", "067", "068", "094", "095", "096", "097", "098", "099", "OA", "OR", "BA", "BR", "CA", "CR", "EA", "ER", "FA", "FR", "HA",
                            "HR", "LA", "LR", "GA", "GR", "NA", "NR", "076", "044", "043", "070", "071", "072" };

                        if (elencoCategorie.Contains(altraPensione.Categoria))
                        {
                            messaggioVideo = "Codice importo incompatibile con codice categoria (" + (altraPensione.Categoria) + ")";
                            return false;
                        }
                    }

                    if ((altraPensione.CodiceImporto == '1' || altraPensione.CodiceImporto == '6' || altraPensione.CodiceImporto == '7') && (categoriaNumerica == 30 || categoriaNumerica == 31))
                    {
                        messaggioVideo = "Codice importo (1-6-7) incompatibile con codice categoria (030-031)";
                        return false;
                    }

                    if (!ConfrontaRecordAdiacenti(LaltraPensione, out messaggioVideo))
                        return false;
                }
            }


            return true;
        }

        public static bool VerificaAltraPensioneWithNaturaPensione(List<AltraPensione> LaltraPensione, string naturaPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (string.IsNullOrEmpty(naturaPensione))
            {
                messaggioVideo = "Natura pensione mancante";
                return false;
            }

            if (naturaPensione.Substring(0, 1).Equals("6") && naturaPensione.Substring(1, 1).Equals("L"))
            {
                foreach (AltraPensione altraPensione in LaltraPensione)
                {
                    if (!(altraPensione.Categoria.Equals("OA") || altraPensione.Categoria.Equals("BA") || altraPensione.Categoria.Equals("HA") || altraPensione.Categoria.Equals("LA")))
                    {
                        messaggioVideo = "Se natura pensione uguale a '6L' deve esserci pensione con categoria OA/BA/HA/LA";
                        return false;
                    }
                }
            }

            if (naturaPensione.Substring(0, 1).Equals("2") || naturaPensione.Substring(0, 1).Equals("4") || naturaPensione.Substring(0, 1).Equals("6") || naturaPensione.Substring(0, 1).Equals("9"))
            {
                foreach (AltraPensione altraPensione in LaltraPensione)
                {
                    if (!altraPensione.Decorrenza.HasValue || altraPensione.Cessazione.HasValue)
                    {
                        messaggioVideo = "Incompatibilità tra natura pensione e altre pensioni";
                        return false;
                    }
                }
            }
            else
            {
                foreach (AltraPensione altraPensione in LaltraPensione)
                {
                    if (altraPensione.Decorrenza.HasValue || !altraPensione.Cessazione.HasValue)
                    {
                        messaggioVideo = "Incompatibilità tra natura pensione e altre pensioni";
                        return false;
                    }
                }
            }


            return true;
        }

        #endregion PCIPL15

        #endregion Bititolarità

        #region Routine
        #region PCIPL40
        /// <summary>
        /// CTR-MINIMI
        /// </summary>
        public static int CTR_Minimi(byte? codiceConvenzione, DateTime? decorrenzaDiretta, DateTime? decorrenzaOriginaria, DateTime? dataMorteDC, string gruppo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            DateTime? decorrenza = null;
            if (decorrenzaDiretta.HasValue)
                decorrenza = decorrenzaDiretta;
            else
                decorrenza = decorrenzaOriginaria;

            if (codiceConvenzione.GetValueOrDefault() == 14)
            {
                if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1984, 01, 01)))
                    return 1;
                else
                    return 52;
            }

            if (codiceConvenzione.GetValueOrDefault() == 4)
                return 1;

            if (codiceConvenzione.GetValueOrDefault() == 9)
            {
                if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1983, 07, 01)))
                    return 13;
                else
                    return 52;
            }

            if (codiceConvenzione.GetValueOrDefault() == 16)
            {
                if (!Utility.DataSuccessivaA(decorrenza.Value, new DateTime(1985, 10, 01)))
                    return 1;
                else
                    return 53;
            }

            List<byte> codici = new List<byte> { 12, 11, 22, 34, 35, 36, 23, 37, 30, 33, 29, 53, 58, 60, 59, 61, 62 };
            if (codici.Contains(codiceConvenzione.GetValueOrDefault()))
                return 52;

            codici.Clear();
            codici = new List<byte> { 21, 27, 31, 13, 42, 43, 26, 56, 57 };
            if (codici.Contains(codiceConvenzione.GetValueOrDefault()))
                return 1;

            codici.Clear();
            codici = new List<byte> { 20, 17 };
            if (codici.Contains(codiceConvenzione.GetValueOrDefault()))
                return 0;

            codici.Clear();
            codici = new List<byte> { 24, 25 };
            if (codici.Contains(codiceConvenzione.GetValueOrDefault()))
                return 53;

            if (codiceConvenzione.GetValueOrDefault() == 38)
            {
                if (gruppo.Equals("0003") && (!decorrenzaDiretta.HasValue || !Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(2002, 08, 01))))
                {
                    if (decorrenzaDiretta.HasValue || (dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(2002, 07, 31))))
                        return 1;
                    else
                        return 52;
                }
                else
                    return 52;
            }

            if (codiceConvenzione.GetValueOrDefault() == 39)
            {
                if (gruppo.Equals("0003") && (!decorrenzaDiretta.HasValue || !Utility.DataSuccessivaA(decorrenzaDiretta.Value, new DateTime(2003, 11, 01))))
                {
                    if (decorrenzaDiretta.HasValue || (dataMorteDC.HasValue && !Utility.DataSuccessivaA(dataMorteDC.Value, new DateTime(2003, 10, 31))))
                        return 1;
                    else
                        return 52;
                }
                else
                    return 52;
            }

            messaggioVideo = "Codice Convenzione non contemplato in tabella CTR Minimi";

            return 99999;
        }

        /// <summary>
        /// Se il campo IABCONA2 (CODICE NATURA PENSIONE)  = "3" OR "4" allora controlla se il campo APPO-CAT1 = "S"  muovi il campo   IW1DNAS (DATA DI NASCITA D. Causa)  nel campo 
        /// di appoggio  APP-DATA-2 e muove il campo  IW1DSES  (SESSO DANTE CAUSA) nel campo di appoggio  APP-SESS  altrimenti muove il campo IW1NATIT (DATA NASCITA TITOLARE)  nel 
        /// campo di appoggio  APP-DATA-2  e muove il campo  IW1SESTIT  (SESSO DEL TITOLARE) nel campo di appoggio  APP-SESS.
        /// Se il campo IW1CATPEN(CAT.PENS.IN CODICE P18)  > 6 allora controlla se il campo APP-SESS  = "F"  somma 60 nel campo APP-ANNO-2  altrimenti somma 65 nel campo  APP-ANNO-2 
        /// altrimenti  se il campo APP-SESS  = "F"  somma 55 nel campo APP-ANNO-2  altrimenti somma 60 nel campo  APP-ANNO-2 
        /// Se il campo APP-DATA-2  (data di appoggio ) > W-DEORIG (APPOGGIO DATA DECORRENZA ORIGINARIA) muove  "N"   nel campo  ETA-PENS altrimenti  muove  "S"   nel campo  
        /// ETA-PENS.
        /// </summary>
        /// <param name="codNatura"></param>
        /// <param name="gruppo"></param>
        /// <param name="dataNascitaDC"></param>
        /// <param name="sessoDC"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns></returns>
        private static bool VerificaEtaPensionabileAllaDecorrenza(string codNatura, string gruppo, DateTime? dataNascitaDC, char? sessoDC, DateTime? dataNascitaTitolare, char? sessoTitolare, DateTime? decorrenza)
        {
            DateTime? dataNascitaCompare = null;
            char? sessoCompare = null;

            if (!string.IsNullOrEmpty(codNatura) && (codNatura.Substring(0, 1).Equals("3") || codNatura.Substring(0, 1).Equals("4")))
            {
                if (gruppo.Equals("0003"))
                {
                    dataNascitaCompare = dataNascitaDC;
                    sessoCompare = sessoDC;
                }
                else
                {
                    dataNascitaCompare = dataNascitaTitolare;
                    sessoCompare = sessoTitolare;
                }

                if (sessoCompare.GetValueOrDefault() == 'F')
                    dataNascitaCompare = dataNascitaCompare.Value.AddYears(55);
                else
                    dataNascitaCompare = dataNascitaCompare.Value.AddYears(60);

                if (Utility.DataStrettamenteSuccessivaA(new DateTime(dataNascitaCompare.Value.Year, dataNascitaCompare.Value.Month, 01), new DateTime(decorrenza.Value.Year, decorrenza.Value.Month, 01)))
                    return false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Se i campi ( APP-DEC  = IW1DEORIG  OR IW1DIRET) OR  ( APP-DEC  = IW1DEBON AND IW1DEBON > 0) allora muove il valore  0  nel campo  APP-CAL5 e controlla se i campi  ART48(1) = "S" AND DEC(1, 1) = 0  continua altrimenti somma il campo SETT1(1) al campo  APP-CAL5;
        /// se i campi  ART48(2) = "S"AND DEC(2, 1) = 0 continua altrimenti somma il campo SETT1(2) al campo  APP-CAL5
        /// se i campi  ART48(2) = "S"AND DEC(3, 1) = 0 continua altrimenti somma il campo SETT1(3) al campo  APP-CAL5
        /// se i campi  ART48(2) = "S"AND DEC(4, 1) = 0 continua altrimenti somma il campo SETT1(4) al campo  APP-CAL5
        /// altrimenti
        /// muove il valore  0  nel campo  APP-CAL5 e controlla se i campi  APP-DEC NOT > DEC(1 1) AND (ART48(1) = "N"   OR DEC(1, 1) > 0) allora controlla 
        /// se il campo SETT2(1) > 0 somma il campo SETT2(1) al campo APP-CAL5  altrimenti somma il campo SETT1(1) al campo  APP-CAL5 
        /// se i campi  APP-DEC NOT > DEC(2 1) AND (ART48(2) = "N"   OR DEC(2, 1) > 0) allora controlla se il campo SETT2(2) > 0   somma il campo SETT2(2) al campo APP-CAL5  altrimenti somma il campo SETT1(2) al campo  APP-CAL5 
        /// se i campi  APP-DEC NOT > DEC(3 1) AND (ART48(2) = "N"   OR DEC(3, 1) > 0) allora controlla se il campo SETT2(2) > 0   somma il campo SETT2(3) al campo APP-CAL5  altrimenti somma il campo SETT1(3) al campo  APP-CAL5 
        /// se i campi  APP-DEC NOT > DEC(4 1) AND (ART48(2) = "N"   OR DEC(4, 1) > 0) allora controlla se il campo SETT2(2) > 0   somma il campo SETT2(4) al campo APP-CAL5  altrimenti somma il campo SETT1(4) al campo  APP-CAL5 
        /// </summary>
        /// <param name="decorrenzaContributoEstero"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenza"></param>
        /// <param name="decorrenzaBonus"></param>
        /// <param name="art48"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="settimaneContributoEstero"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int? GEST_EST_61(int? sommaGEST_EST_61, DateTime? decorrenzaContributoEstero, DateTime? decorrenzaOriginaria, DateTime? decorrenzaDanteCausa, DateTime? decorrenzaBonus, char? art48, DateTime?[] primaDecorrenzaImportiEsteri,
            int? contributiEEDecorrenzaOriginaria, int? contributiEERicalcolo, int index)
        {
            if ((decorrenzaContributoEstero.Equals(decorrenzaDanteCausa) || decorrenzaContributoEstero.Equals(decorrenzaOriginaria)) || (decorrenzaBonus.HasValue && decorrenzaContributoEstero.Equals(decorrenzaBonus)))
            {
                if (!(art48.HasValue && art48.Value == 'S' && primaDecorrenzaImportiEsteri[index] == null))
                {
                    sommaGEST_EST_61 = sommaGEST_EST_61.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
                }
            }
            else
            {
                if ((primaDecorrenzaImportiEsteri[index].HasValue && !Utility.DataStrettamenteSuccessivaA(decorrenzaContributoEstero.Value, primaDecorrenzaImportiEsteri[index].Value)) &&
                    ((art48.HasValue && art48.Value == 'N') || primaDecorrenzaImportiEsteri[index].HasValue))
                {
                    if (contributiEERicalcolo.GetValueOrDefault() > 0)
                        sommaGEST_EST_61 = sommaGEST_EST_61.GetValueOrDefault() + contributiEERicalcolo.GetValueOrDefault();
                    else
                        sommaGEST_EST_61 = sommaGEST_EST_61.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
                }
            }

            return sommaGEST_EST_61;
        }


        #endregion PCIPL40

        #region PCIPL39
        /// <summary>
        /// Serve per valorizzare TP1NUA
        /// </summary>
        /// <param name="settimaneItalianeDiritto"></param>
        /// <param name="nSettimaneOBG"></param>
        /// <param name="nContributiUtiliLavoratoriAutonomi"></param>
        /// <returns></returns>
        public static int? NumeroSettimane(int? settimaneItalianeDiritto, int? nSettimaneOBG, int? nContributiUtiliLavoratoriAutonomi)
        {
            if (settimaneItalianeDiritto.HasValue)
                return settimaneItalianeDiritto.Value;
            else if (nSettimaneOBG.HasValue)
                return nSettimaneOBG.Value;
            else if (nContributiUtiliLavoratoriAutonomi.HasValue)
                return nContributiUtiliLavoratoriAutonomi.Value;
            else return null;
        }

        /// <summary>
        /// Se APPO-CAT1 è uguale a "S" valorizzare con IW1DNAS il campo APP-DATA-2, con  IW1DSES il campo APP-SESS diversamente valorizzare con  IW1NATIT il campo APP-DATA-2, con  IW1SESTIT il campo APP-SESS;                                                 
        /// Se (ICIRETCDM335 è maggiore di zero, oppure  ICIRETCOM335 è maggiore di zero, oppure  ICIRETART335 è maggiore di zero, oppure  ICIRETCDM012 è maggiore di zero, oppure  ICIRETCOM012 è maggiore di zero, oppure  ICIRETART012 è maggiore di zero) aggiungere 60 al campo  APP-ANNO-2 
        /// 210.	Se IW1CODC  è uguale a 1 sottrarre 5 ad APP-ANNO-2;                         
        /// Valorizzare con 30 il campo APP-GIORNO-2;
        /// Valorizzare con W-DEORIG  i primi 6 caratteri del campo APP-DATA-1 e con 1 il campo      APP-GIORNO-1; 
        /// Se il valore costituito dai primi 6 caratteri del campo APP-DATA-2 è maggiore del valore costituito dai primi 6 caratteri del campo APP-DATA-1 richiamare il programma "PCIPL94" passandogli i parametri APP-DATA-1,   APP-DATA-2  e APP-DATA-RC, diversamente valorizzare con zero il campo APP-DATA-RC
        /// </summary>
        /// <param name="gruppo"></param>
        /// <param name="siglaCategoria"></param>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="dataNascitaDC"></param>
        /// <param name="sessoDC"></param>
        /// <param name="dataNascitaTitolare"></param>
        /// <param name="sessoTitolare"></param>
        /// <param name="cmsm"></param>
        /// <param name="montante"></param>
        /// <param name="codiceCieco"></param>
        /// <returns></returns>
        public static int CalcolaSettimaneFittizie(string gruppo, DateTime? decorrenza, DateTime? dataNascitaDC, DateTime? dataNascitaTitolare,
            decimal? montante, byte? codiceCieco)
        {
            int settimaneFittizie = 0;
            DateTime? dataNasciaApp = null;

            if (!string.IsNullOrEmpty(gruppo) && gruppo.Trim().Equals("0003"))
                dataNasciaApp = dataNascitaDC;
            else
                dataNasciaApp = dataNascitaTitolare;

            if (montante.HasValue && montante.Value > 0)
                dataNasciaApp.Value.AddYears(60);

            if (codiceCieco.HasValue && codiceCieco.Value == 1)
                dataNasciaApp.Value.AddYears(-5);

            //setto il giorno a 30
            dataNasciaApp.Value.AddDays(-dataNasciaApp.Value.Day + 30);

            if (Utility.DataStrettamenteSuccessivaA(dataNasciaApp.Value, decorrenza.Value))
            {
                settimaneFittizie = Utility.NSettimaneBetweenDate(dataNasciaApp.Value, decorrenza.Value);
            }

            return settimaneFittizie;
        }

        /// <summary>
        /// Se (W-DEORIG è inferiore a 198201) oppure (IW1CATPEN è uguale a 87 e IW1DIRET è uguale a zero e IW1DMOR è inferiore a 19690502) valorizzare con "S" il campo FL-NO233;
        /// </summary>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="categoria"></param>
        /// <param name="dataMorte"></param>
        /// <returns></returns>
        public static bool FlagNo233(DateTime? decorrenza, DateTime? decorrenzaDiretta, int categoria, DateTime? dataMorte)
        {
            DateTime dataCompare = new DateTime(1982, 01, 01);
            DateTime dataMorteCompare = new DateTime(1969, 05, 02);

            if ((decorrenza.HasValue && !Utility.DataSuccessivaA(decorrenza.Value, dataCompare)) || (categoria == 87 && !decorrenzaDiretta.HasValue && dataMorte.HasValue && !Utility.DataSuccessivaA(dataMorte.Value, dataMorteCompare)))
                return true;

            return false;
        }

        /// <summary>
        /// Se (W-DEORIG è maggiore di 199300 e FINASS è inferiore a 19930101)  e (IABCONA2  è uguale a 3 oppure 4) valorizzare con "S" il campo FL-P93FIA93-INA;
        /// </summary>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <returns></returns>
        public static bool FlagP93FIA93INA(DateTime? decorrenza, DateTime? fineAssicurazione, string naturaPensione)
        {
            DateTime dataCompare = new DateTime(1993, 01, 01);

            if (decorrenza.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, dataCompare) && fineAssicurazione.HasValue && !Utility.DataSuccessivaA(fineAssicurazione.Value, dataCompare) &&
                !string.IsNullOrEmpty(naturaPensione) && (naturaPensione.Substring(0, 1).Equals("3") || naturaPensione.Substring(0, 1).Equals("4")))
                return true;

            return false;
        }

        /// <summary>
        /// Se (W-DEORIG è maggiore di 199301 e FINASS  è inferiore a 19930101) e (IABCONA2 non è uguale a 3 e 4)  valorizzare con "S" il campo FL-P93FIA93-NOINA;
        /// </summary>
        /// <param name="decorrenzaPensione"></param>
        /// <param name="decorrenzaDiretta"></param>
        /// <param name="fineAssicurazione"></param>
        /// <param name="naturaPensione"></param>
        /// <returns></returns>
        public static bool FlagP93FIA93NOINA(DateTime? decorrenza, DateTime? fineAssicurazione, string naturaPensione)
        {
            DateTime dataCompare = new DateTime(1993, 01, 01);

            if (decorrenza.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenza.Value, dataCompare) && fineAssicurazione.HasValue && !Utility.DataSuccessivaA(fineAssicurazione.Value, dataCompare) &&
                !string.IsNullOrEmpty(naturaPensione) && !naturaPensione.Substring(0, 1).Equals("3") && !naturaPensione.Substring(0, 1).Equals("4"))
                return true;

            return false;
        }

        /// <summary>
        /// 18.	Valorizzare con zero il campo APP-CAL-EST-R e con "N" il campo SET-RICAL;
        /// 19.	Eseguire un ciclo a conteggio variando IND1 da 1  con incrementi di 1 fino a quando questo è  maggiore di 4 oppure se STATO(IND1)  è uguale a zero, per ogni ciclo 
        /// effettuare i seguenti controlli :                                                 
        /// 19.1.	Se  ART48(IND1) è uguale a "S" e DEC(IND1, 1) è uguale a zero passare all’occorrenza succesiva ritornare al punto 20, diversamente effettuare le seguenti 
        /// operazioni : 
        /// 19.1.1.	Se SETT2(IND1) è maggiore di zero effettuare le seguenti operazioni :  
        /// 19.1.1.1.	Valorizzare con  "S" il campo SET-RICAL; 
        /// 19.1.1.2.	Calcolare APP-CAL-EST-R   =  APP-CAL-EST-R  +  SETT2(IND1      ) 
        /// 19.1.2.	Diversamente da quanto analizzato nel punto precedente (19.1.1)          Calcolare APP-CAL-EST-R    =  APP-CAL-EST-R   +    SETT1(IND1);
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="art48"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="contributiEERicalcolo"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneRicalcoloMisura(int? totSettimane, char? art48, DateTime? primaDecorrenzaImportiEsteri, int? contributiEERicalcolo, int? contributiEEDecorrenzaOriginaria, ref bool set_Rical)
        {
            if (!(art48.GetValueOrDefault() == 'S' && !primaDecorrenzaImportiEsteri.HasValue))
            {
                if (contributiEERicalcolo.GetValueOrDefault() > 0)
                {
                    totSettimane = totSettimane.GetValueOrDefault() + contributiEERicalcolo.GetValueOrDefault();
                    set_Rical = true;
                }
                else
                    totSettimane = totSettimane.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
            }

            return totSettimane;
        }

        /// <summary>
        /// 59.	Se (W-DEORIG è maggiore di 199301 e INIASS  è maggiore di 19930100) valorizzare con "S" il campo FL-P93INP93;
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <returns></returns>
        public static bool FlagP93INP93(DateTime? decorrenza, DateTime? dataInizioAssicurazione)
        {
            if (Utility.DataStrettamenteSuccessivaA(decorrenza.Value, new DateTime(1993, 01, 31)) && dataInizioAssicurazione.HasValue && Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                return true;

            return false;
        }

        /// <summary>
        /// 56.	Se  FINASS  è inferiore a 19930101 valorizzare con "S" il campo FL-FIA93;
        /// </summary>
        /// <param name="dataFineAssicurazione"></param>
        /// <returns></returns>
        public static bool FlagFIA93(DateTime? dataFineAssicurazione)
        {
            if (dataFineAssicurazione.HasValue && !Utility.DataSuccessivaA(dataFineAssicurazione.Value, new DateTime(1993, 01, 01)))
                return true;

            return false;
        }

        /// <summary>
        /// IMPORTANTE: Va richiamato all'interno di un foreach che scorre tutti gli stati
        /// GEST-EST-61.
        /// 256.	Se  ( APP-DEC  è uguale a IW1DEORIG  oppure IW1DIRET)  effettuare le seguenti operazioni : 
        /// 256.1.	Valorizzare con zero il campo  APP-CAL5;
        /// 256.2.	Se  ART48(1)  non  è uguale a "S" aggiungere SETT1(1) a APP-CAL5;
        /// 256.3.	Se  ART48(2)  non  è uguale a "S" aggiungere SETT1(2) a APP-CAL5;
        /// 256.4.	Se  ART48(3)  non  è uguale a "S" aggiungere SETT1(3) a APP-CAL5;
        /// 256.5.	Se  ART48(4)  non  è uguale a "S" aggiungere SETT1(4) a APP-CAL5;
        /// 257.	Diversamente da quanto analizzato nel punto precedente (256) effettuare le seguenti operazioni : 
        /// 257.1.	Valorizzare con zero il campo APP-CAL5;
        /// 257.2.	Se APP-DEC non è maggiore di DEC(1 1) e ART48(1) non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 257.2.1.	Se SETT2(1) è maggiore di zero aggiungere  SETT2(1) a APP-CAL5 diversamente aggiungere SETT1(1);
        /// 257.3.	Se APP-DEC non è maggiore di DEC(2 1) e ART48(2) non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 257.3.1.	Se SETT2(2) è maggiore di zero aggiungere  SETT2(2) a APP-CAL5 diversamente aggiungere SETT1(2);     
        /// 257.4.	Se APP-DEC non è maggiore di DEC(3 1) e ART48(3) non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 257.4.1.	Se SETT2(3) è maggiore di zero aggiungere  SETT2(3) a APP-CAL5 diversamente aggiungere SETT1(3);     
        /// 257.5.	Se APP-DEC non è maggiore di DEC(4 1) e ART48(4) non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 257.5.1.	Se SETT2(4) è maggiore di zero aggiungere  SETT2(4) a APP-CAL5 diversamente aggiungere SETT1(4);     
        /// 258.	Fine GEST-EST-61.                                                
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="decorrenzaContributiItalianiEdEsteri"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <param name="decorrenzaPensioneDiretta"></param>
        /// <param name="codiceArt48"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="contributiEERicalcolo"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneEstereWithDecorrenzaContributiItalianiEdEsteri(int? totSettimane, DateTime? decorrenzaContributiItalianiEdEsteri, DateTime? decorrenzaOriginaria, DateTime? decorrenzaPensioneDiretta, char? codiceArt48, int? contributiEEDecorrenzaOriginaria, DateTime? primaDecorrenzaImportiEsteri, int? contributiEERicalcolo)
        {
            if (decorrenzaContributiItalianiEdEsteri.Equals(decorrenzaOriginaria) || decorrenzaContributiItalianiEdEsteri.Equals(decorrenzaPensioneDiretta))
            {
                if (codiceArt48.GetValueOrDefault() != 'S')
                    totSettimane = totSettimane.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
            }
            else
            {
                if (decorrenzaContributiItalianiEdEsteri.HasValue && primaDecorrenzaImportiEsteri.HasValue && !Utility.DataStrettamenteSuccessivaA(decorrenzaContributiItalianiEdEsteri.Value, primaDecorrenzaImportiEsteri.Value) && codiceArt48.GetValueOrDefault() != 'S')
                {
                    if (contributiEERicalcolo.GetValueOrDefault() > 0)
                        totSettimane = totSettimane.GetValueOrDefault() + contributiEERicalcolo.GetValueOrDefault();
                    else
                        totSettimane = totSettimane.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
                }
            }

            return totSettimane;
        }

        /// <summary>
        /// 60.	Se (W-DEORIG è maggiore di 199300 e INIASS  è inferiore a 19930101)  e FINASS è maggiore di 19930100 valorizzare con "S" il campo FL-P93INA93FIP93;
        /// </summary>
        /// <param name="decorrenza"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="dataFineAssicurazione"></param>
        /// <returns></returns>
        public static bool FlagP93INA93FIP93(DateTime? decorrenza, DateTime? dataInizioAssicurazione, DateTime? dataFineAssicurazione)
        {
            DateTime dataCompare = new DateTime(1993, 01, 01);
            if (Utility.DataSuccessivaA(decorrenza.Value, dataCompare) && dataInizioAssicurazione.HasValue && !Utility.DataSuccessivaA(dataInizioAssicurazione.Value, dataCompare) && dataFineAssicurazione.HasValue && Utility.DataSuccessivaA(dataFineAssicurazione.Value, dataCompare))
                return true;

            return false;
        }

        /// <summary>
        /// IMPORTANTE: va richiamato dentro un ciclo che scorre gli stati esteri
        /// 236.	Valorizzare con zero il campo APP-CAL-EST;
        /// 237.	Se  ART48(1)  non  è uguale a "S" effettuare le seguenti operazioni : 
        /// 237.1.	Se SETT2(1) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(1), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(1);
        /// 238.	Se  ART48(2) non è uguale a "S" effettuare le seguenti operazioni : 
        /// 238.1.	Se SETT2(2) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(2), diversamente valorizzare APP-CAL-EST = APP-CAL-EST + SETT1(2);
        /// 239.	Se  ART48(3) non è uguale a "S" effettuare le seguenti operazioni :                                                              
        /// 239.1.	Se SETT2(3) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(3), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(3);
        /// 240.	Se  ART48(4) non è uguale a "S" effettuare le seguenti operazioni :  
        /// 240.1.	Se SETT2(4) è maggiore di zero calcolare APP-CAL-EST = APP-CAL-EST + SETT2(4), diversamente calcolare APP-CAL-EST = APP-CAL-EST + SETT1(4) ;
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="codiceArt48"></param>
        /// <param name="contributEERicalcolo"></param>
        /// <param name="contributiEEDecorrenzaOriginaria"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneEstereWithCodiceArt48(int? totSettimane, char? codiceArt48, int? contributEERicalcolo, int? contributiEEDecorrenzaOriginaria)
        {
            if (codiceArt48.GetValueOrDefault() != 'S')
            {
                if (contributEERicalcolo.GetValueOrDefault() > 0)
                    totSettimane = totSettimane.GetValueOrDefault() + contributEERicalcolo.GetValueOrDefault();
                else
                    totSettimane = totSettimane.GetValueOrDefault() + contributiEEDecorrenzaOriginaria.GetValueOrDefault();
            }

            return totSettimane;
        }

        /// <summary>
        /// 178.5.	Se INIASS è inferiore a 19930101 e APP-APP è maggiore di zero effettuare le seguenti operazioni : 
        /// 178.5.1.	Se GEST233(1   1)  è uguale a X aggiungere SETRI233(1   1)  al campo     APP-APP; 
        /// 178.5.2.	Se GEST233(1   2)   è uguale a X  aggiungere SETRI233(1   2) al campo     APP-APP;
        /// 178.5.3.	Se  GEST233(2   1)  è uguale a X  aggiungere SETRI233(2   1) al campo     APP-APP;
        /// 178.5.4.	Se  GEST233(2   2)  è uguale a X  aggiungere SETRI233(2   2)  al campo   APP-APP;
        /// 178.5.5.	Se  GEST233(3   1)  è uguale a X aggiungere SETRI233(3   1)   al campo   APP-APP;               
        /// 178.5.6.	Se  GEST233(3   2)  è uguale a X aggiungere SETRI233(3   2)   al campo   APP-APP;
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="codiceGestione"></param>
        /// <param name="settimaneContributiItalianiEdEsteri"></param>
        /// <param name="codiceGestioneToCompare"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneContributiItalianiEdEsteri9395(int? totSettimane, DateTime? dataInizioAssicurazione, short? codiceGestione, int? settimaneContributiItalianiEdEsteri, short codiceGestioneToCompare)
        {
            if (dataInizioAssicurazione.HasValue && !Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1993, 01, 01)) && totSettimane.GetValueOrDefault() > 0)
                if (codiceGestione.GetValueOrDefault() == codiceGestioneToCompare)
                    totSettimane = totSettimane.GetValueOrDefault() + settimaneContributiItalianiEdEsteri.GetValueOrDefault();

            return totSettimane;
        }

        /// <summary>
        /// 179.9.	Se INIASS è inferiore a 19960101 e APP-APP è maggiore di zero effettuare le seguenti operazioni :                                                        
        /// 179.9.1.	Se  GEST233(1   1)   è uguale a  X  aggiungere SETRI233(1   1)  al campo         APP-APP;               
        /// 179.9.2.	Se  GEST233(1   2)   è uguale a  X  aggiungere SETRI233(1   2)  al campo  APP-APP;
        /// 179.9.3.	Se  GEST233(2   1)   è uguale a  X  aggiungere SETRI233(2   1)  al campo  APP-APP;               
        /// 179.9.4.	Se  GEST233(2   2)   è uguale a  X  aggiungere SETRI233(2   2)  al campo      APP-APP;                
        /// 179.9.5.	Se  GEST233(3   1)   è uguale a  X  aggiungere SETRI233(3   1)  al campo  APP-APP;                            
        /// 179.9.6.	Se  GEST233(3 2)    è uguale a  X   aggiungere SETRI233(3   2)  al campo  APP-APP;                            
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="dataInizioAssicurazione"></param>
        /// <param name="codiceGestione"></param>
        /// <param name="settimaneContributiItalianiEdEsteri"></param>
        /// <param name="codiceGestioneToCompare"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneContributiItalianiEdEsteriPost95(int? totSettimane, DateTime? dataInizioAssicurazione, short? codiceGestione, int? settimaneContributiItalianiEdEsteri, short codiceGestioneToCompare)
        {
            if (dataInizioAssicurazione.HasValue && !Utility.DataSuccessivaA(dataInizioAssicurazione.Value, new DateTime(1996, 01, 01)) && totSettimane.GetValueOrDefault() > 0)
                if (codiceGestione.GetValueOrDefault() == codiceGestioneToCompare)
                    totSettimane = totSettimane.GetValueOrDefault() + settimaneContributiItalianiEdEsteri.GetValueOrDefault();

            return totSettimane;
        }

        /// <summary>
        /// 160.	Se  GEST233(1   1) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(1   1) al campo APP-APP; 
        /// 161.	Se  GEST233(1   2) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(1   2) al campo APP-APP; 
        /// 162.	Se  GEST233(2   1) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(2   1) al campo APP-APP; 
        /// 163.	Se  GEST233(2   2) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(2   2) al campo APP-APP;
        /// 164.	Se  GEST233(3   1) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(3   1) al campo APP-APP;
        /// 165.	Se  GEST233(3   2) è uguale a 4 oppure 64 oppure 74 aggiungere SETRI233(3   2) al campo APP-APP;
        /// </summary>
        /// <param name="totSettimane"></param>
        /// <param name="codiceGestione"></param>
        /// <param name="settimaneContributiItalianiEdEsteri"></param>
        /// <returns></returns>
        public static int? GetNumeroSettimaneContributiItalianiEdEsteriCodGestioneX4(int? totSettimane, short? codiceGestione, int? settimaneContributiItalianiEdEsteri)
        {
            if (codiceGestione.GetValueOrDefault() == 4 || codiceGestione.GetValueOrDefault() == 64 || codiceGestione.GetValueOrDefault() == 74)
                totSettimane = totSettimane.GetValueOrDefault() + settimaneContributiItalianiEdEsteri.GetValueOrDefault();

            return totSettimane;
        }

        #endregion PCIPL39

        #region PCIPL12
        /// <summary>
        /// VED-CONV. 
        /// 100.	Valorizzare con zero il campo W-APP-CON;
        /// 101.	Se  W-APP-STA è uguale a 13, 38, 39, 42, 43, 56 oppure a 57 valorizzare con 13 il campo                W-APP-CON                                      
        /// 102.	Se  W-APP-STA è uguale a 38 effettuare le seguenti operazioni :
        /// 102.1.	Se W-APP-DEC è maggiore di 200404 valorizzare con 12 il campo W-APP-CON;
        /// 103.	Se  W-APP-STA è uguale a 1,  2,  4,  6,  7, 10, 11, 18, 19, 28, 32, 40, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 54, 55 valorizzare con 12 il campo W-APP-CON;
        /// 104.	Se  W-APP-STA è uguale a 09, 20 oppure a 29 effettuare le seguenti operazioni :
        /// 104.1.	Se W-APP-DEC è minore di 199401 valorizzare con W-APP-STA il campo W-APP-CON; diversamente valorizzare con 12 il campo W-APP-CON;
        /// 105.	Se W-APP-STA è uguale a 27 effettuare le seguenti operazioni :
        /// 105.1.	Se W-APP-DEC è minore di 199505 valorizzare con W-APP-STA il campo W-APP-CON;                diversamente valorizzare con 12 il campo W-APP-CON;                         
        /// 106.	Se  W-APP-STA è uguale a 17 effettuare le seguenti operazioni :
        /// 106.1.	Se W-APP-DEC è minore di 200206 valorizzare con W-APP-STA il campo W-APP-CON diversamente valorizzare con 12 il campo W-APP-CON;                     
        /// 107.	Se  W-APP-STA è ugauel a 14, 16, 21, 22, 23, 24, 25, 26, 30, 31, 33, 34, 36, 37 oppure a 53 valorizzare con W-APP-STA il campo W-APP-CON;
        /// 108.	 Fine VED-CONV.             
        /// </summary>
        /// <param name="codiceStatoEE"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns></returns>
        public static byte? GetCodiceConvenzioneByCodiceStatoEE(int? codiceStatoEE, DateTime? decorrenzaOriginaria)
        {
            byte? codiceConvenzione = 0;

            if ((new List<int> { 13, 38, 39, 42, 43, 56, 57 }).Contains(codiceStatoEE.GetValueOrDefault()))
                codiceConvenzione = 13;

            if (codiceStatoEE.GetValueOrDefault() == 38 && Utility.DataStrettamenteSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2004, 04, 30)))
                codiceConvenzione = 12;

            if ((new List<int> { 1, 2, 4, 6, 7, 10, 11, 18, 19, 28, 32, 40, 41, 44, 45, 46, 47, 48, 49, 50, 51, 52, 54, 55 }).Contains(codiceStatoEE.GetValueOrDefault()))
                codiceConvenzione = 12;

            if ((new List<int> { 9, 20, 29 }).Contains(codiceStatoEE.GetValueOrDefault()))
                if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1994, 01, 01)))
                    codiceConvenzione = (byte)codiceStatoEE;
                else
                    codiceConvenzione = 12;

            if (codiceStatoEE == 27)
                if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1995, 05, 01)))
                    codiceConvenzione = (byte)codiceStatoEE;
                else
                    codiceConvenzione = 12;

            if (codiceStatoEE == 17)
            {
                if (!Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2002, 06, 01)))
                    codiceConvenzione = (byte)codiceStatoEE;
                else
                    codiceConvenzione = 12;
            }

            if ((new List<int> { 14, 16, 21, 22, 23, 24, 25, 26, 30, 31, 33, 34, 36, 37, 53 }).Contains(codiceStatoEE.GetValueOrDefault()))
                codiceConvenzione = (byte)codiceStatoEE;

            if (codiceStatoEE.GetValueOrDefault() == 58 && Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(2015, 12, 01)))
                codiceConvenzione = 58;

            if (codiceStatoEE.GetValueOrDefault() == 59)
                codiceConvenzione = 61;

            if (codiceStatoEE.GetValueOrDefault() == 60)
                codiceConvenzione = 62;

            return codiceConvenzione;
        }

        public static char? GetTipologiaStato(int? codiceStatoEE)
        {
            //* S= CEE
            //* X= CEE DA ALTRA DATA
            //* N= NO CEE
            //* M= NO CEE CON PARTICOLARITA'  
            // 12345678910111213141516171819202122232425262728293031323334353637383940414243444546474849505152535455
            // SS S SS X S S   N M   N X S S X N M N N N N X S X N N S N M   N N X N S S N N S S S S S S S S S N S S

            switch (codiceStatoEE.GetValueOrDefault())
            {
                case 1:
                case 2:
                case 4:
                case 6:
                case 7:
                case 10:
                case 11:
                case 18:
                case 19:
                case 28:
                case 32:
                case 40:
                case 41:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 54:
                case 55:
                    return 'S';
                case 9:
                case 17:
                case 20:
                case 27:
                case 29:
                case 38:
                    return 'X';
                case 13:
                case 16:
                case 21:
                case 23:
                case 24:
                case 25:
                case 26:
                case 30:
                case 31:
                case 33:
                case 36:
                case 37:
                case 39:
                case 42:
                case 43:
                case 53:
                    return 'N';
                case 14:
                case 22:
                case 34:
                    return 'M';
                default:
                    return ' ';
            }
        }

        /// <summary>
        /// VEDI-RESID. 
        /// 109.	Valorizzare con Spazi il campo W-RESOUT;
        /// 110.	Eseguire la subroutine VED-RES variando IND1 a partire da 1 con incremento di 1 fino a quando IND1  è maggiore di 4 oppure W-RESOUT non è uguale a spazi;
        /// 111.	Se W-RESOUT è uguale a spazi effettuare le seguenti operazioni :
        /// 111.1.	Se ICI2RESEST è uguale a "I  " valorizzare con "I" il campo W-RESOUT, diversamente valorizzare con "E" il campo W-RESOUT;
        /// 111.2.	Se IDECRES(5) è maggiore di zero valorizzare con IDECRES(5)  il campo W-DECRES diversamente valorizzare con IDECRES(1)  il campo W-DECRES;
        /// 112.	Fine VEDI-RESID.
        ///  VED-RES.
        /// 113.	Se (W-DECRES non è minore di IDECRES(IND1) ed è minore di IDECRES(IND1 + 1)) oppure    (W-DECRES NOT è minore di IDECRES(IND1) e IDECRES(IND1 + 1) è uguale a zero)
        /// effettuare le seguenti operazioni :                                                      
        /// 113.1.	Se ICODRES(IND1) è uguale a "I  " valorizzare con "I" il campo W-RESOUT, diversamente valorizzare con "E" il campo W-RESOUT;                                 
        /// 113.2.	Valorizzare con IDECRES(IND1)  il campo W-DECRES;
        /// 114.	Fine VED-RES.
        /// </summary>
        /// <param name="decorrenzaImportiEsteri"></param>
        /// <param name="listaResidenzeEstere"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="provinciaResidenza"></param>
        /// <returns></returns>
        public static char? GetStatoResidenzaByImportiEsteri(ref DateTime? decorrenzaImportiEsteri, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, string codiceComuneResidenza)
        {
            char? result = null;

            if (decorrenzaImportiEsteri.HasValue)
            {
                if (listaResidenzeEstere != null && listaResidenzeEstere.Count > 0)
                {
                    for (int i = 0; i < listaResidenzeEstere.Count - 1; i++)
                    {
                        if (Utility.DataSuccessivaA(decorrenzaImportiEsteri.Value, listaResidenzeEstere[i].Decorrenza.Value) && (!listaResidenzeEstere[i + 1].Decorrenza.HasValue || !Utility.DataSuccessivaA(decorrenzaImportiEsteri.Value, listaResidenzeEstere[i + 1].Decorrenza.Value)))
                        {
                            if (listaResidenzeEstere[i].CodCatastaleStatoEE == "Z000")
                                result = 'I';
                            else
                                result = 'E';
                            decorrenzaImportiEsteri = listaResidenzeEstere[i].Decorrenza;

                            break;
                        }
                    }

                    if (result != null)
                    {
                        if (!codiceComuneResidenza.StartsWith("Z"))
                            result = 'I';
                        else
                            result = 'E';
                    }

                    if (listaResidenzeEstere.Last().Decorrenza.HasValue)
                        decorrenzaImportiEsteri = listaResidenzeEstere.Last().Decorrenza;
                    else
                        decorrenzaImportiEsteri = listaResidenzeEstere.First().Decorrenza;
                }
            }

            return result;
        }

        public static int? GetStatoCEE(int? codiceStatoEE)
        {
            int? w_uno_stato = codiceStatoEE;

            if ((new List<int> { 1, 2, 4, 6, 7, 10, 18, 11, 19, 28, 40, 41, 32, 44, 45, 46, 47, 48, 49, 50, 51, 52 }).Contains(codiceStatoEE.GetValueOrDefault()))
                w_uno_stato = 12;


            return w_uno_stato;
        }

        /// <summary>
        /// 12.	Se  ICI2RESEST  è uguale a "I  " e ICODRES(2) è maggiore di "   " e ICI2CONV non è uguale a 12 eseguire i seguenti controlli :
        /// 12.1.	Se ICODRES(5) è uguale a "I  "  Valorizzare DECRIAN  e DATRIEN con IDECRES(5);
        /// 12.1.1.	Diversamente da quanto analizzato nel punto precedente (12.1) se ICODRES(4) è uguale a "I  "  Valorizzare DECRIAN  e DATRIEN con IDECRES(4);
        /// 12.1.1.1.	Diversamente da quanto analizzato nel punto precedente (12.1.1) se ICODRES(3) è uguale a "I  "  Valorizzare DECRIAN  e DATRIEN con IDECRES(3);
        /// 12.1.1.1.1.	Diversamente da quanto analizzato nel punto precedente (12.1.1.1) se ICODRES(2) è uguale a "I  "  Valorizzare DECRIAN  e DATRIEN con IDECRES(2);
        /// 12.2.	Se DECRIAN è inferiore al 199601 Valorizzare DECRIAN e DATRIEN con "000000" 
        /// 12.3.	diversamente Valorizzare DECRIAN-M con “01”;
        /// </summary>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="listaResidenzeEstero"></param>
        /// <param name="codiceConvenzione"></param>
        /// <returns></returns>
        public static DateTime? GetUltimaDecorrenzaResidenzaItaliana(string codiceComuneResidenza, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstero, byte? codiceConvenzione)
        {
            DateTime? result = null;

            if (!codiceComuneResidenza.StartsWith("Z") && listaResidenzeEstero != null && listaResidenzeEstero.Count > 1 && listaResidenzeEstero[1].Decorrenza.HasValue && codiceConvenzione.GetValueOrDefault() != 12)
            {
                for (int i = listaResidenzeEstero.Count - 1; i == 0; i--)
                {
                    if (listaResidenzeEstero[i].CodCatastaleStatoEE.Equals("Z000"))
                    {
                        result = listaResidenzeEstero[i].Decorrenza;
                        break;
                    }
                }

                if (result.HasValue && !Utility.DataSuccessivaA(result.Value, new DateTime(1996, 01, 01)))
                    result = null;
            }

            return result;
        }

        /// <summary>
        /// 29.13.	Se ( DECRIAN è maggiore di zero e DEC (IND-STA INDICE) è uguale a DECRIAN )            o ( (CES (IND-STA INDICE) è maggiore di zero e  è minore di DECRIAN)  e          
        /// (DEC (IND-STA INDICE + 1) è uguale a zero o è maggiore di DECRIAN) ) :               Valorizzare con "S"  il campo  FLGRIAN;  
        /// </summary>
        /// <param name="ultimaDecorrenzaResidenzaItaliana"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <returns></returns>
        public static bool IsDecorrenzaResidenzaItalianaOK(DateTime? ultimaDecorrenzaResidenzaItaliana, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri)
        {
            if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
            {
                for (int i = 0; i < listaImportiEsteri.Count; i++)
                {
                    if (ultimaDecorrenzaResidenzaItaliana.HasValue && ((listaImportiEsteri[i].DecorrenzaPrestazioneEE.Equals(ultimaDecorrenzaResidenzaItaliana)) ||
                        (listaImportiEsteri[i].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(listaImportiEsteri[i].CessazionePrestazioneEE.Value, ultimaDecorrenzaResidenzaItaliana.Value)) ||
                        (i < listaImportiEsteri.Count - 1 && listaImportiEsteri[i + 1].DecorrenzaPrestazioneEE.HasValue && Utility.DataStrettamenteSuccessivaA(listaImportiEsteri[i + 1].DecorrenzaPrestazioneEE.Value, ultimaDecorrenzaResidenzaItaliana.Value))))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 29.15.	Se IW1DEOP è maggiore di zero e  DEC-OPZ non è uguale a "S"  effettuare le seguenti operazioni :                                                 
        /// 29.15.1.	Se  DEC (IND-STA INDICE) è uguale a IW1DEOP : Valorizzare con "S"  il campo  DEC-OPZ;
        /// 29.15.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni : effettuare le seguenti operazioni : 
        /// 29.15.2.1.	Se INDICE è maggiore di 1 e ( DEC(IND-STA INDICE) è maggiore di IW1DEOP) e (CES(IND-STA INDICE-1)  è maggiore di zero e è minore di IW1DEOP)  :Valorizzare 
        /// con "S"  il campo  DEC-OPZ;                      
        /// 29.15.2.2.	INDX è uguale a INDICE  + 1;
        /// 29.15.2.3.	Se (CES(IND-STA INDICE) è maggiore di zero e uguale o inferiore a IW1DEOP) e (DEC(IND-STA INDX) è uguale a zero) :                       Valorizzare con "S"  
        /// il campo  DEC-OPZ;
        /// </summary>
        /// <param name="decorrenzaOpzione"></param>
        /// <param name="listaImportiEsteri"></param>
        /// <param name="index"></param>
        /// <param name="dec_Opz"></param>
        public static void GetDecOpz(DateTime? decorrenzaOpzione, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri, int index, ref bool dec_Opz)
        {
            if (decorrenzaOpzione.HasValue && !dec_Opz)
            {
                if (listaImportiEsteri[index].DecorrenzaPrestazioneEE.Equals(decorrenzaOpzione))
                    dec_Opz = true;
                else
                {
                    if (index > 1 && Utility.DataStrettamenteSuccessivaA(listaImportiEsteri[index].DecorrenzaPrestazioneEE.Value, decorrenzaOpzione.Value) && listaImportiEsteri[index - 1].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(listaImportiEsteri[index - 1].CessazionePrestazioneEE.Value, decorrenzaOpzione.Value))
                        dec_Opz = true;
                    if (listaImportiEsteri[index].CessazionePrestazioneEE.HasValue && !Utility.DataStrettamenteSuccessivaA(listaImportiEsteri[index].CessazionePrestazioneEE.Value, decorrenzaOpzione.Value) && listaImportiEsteri.Count > index + 1 && listaImportiEsteri[index + 1].DecorrenzaPrestazioneEE.HasValue)
                        dec_Opz = true;
                }
            }

        }

        /// <summary>
        /// 29.12.	Se (DEC (IND-STA INDICE) è maggiore di 200000 e è minore di 200100)  o ( (CES (IND-STA INDICE) è maggiore di zero e  è minore di 200100)  e  (DEC (IND-STA INDICE 
        /// + 1) è uguale a zero o è maggiore di 200100)) : Valorizzare con "S"  il campo  DEC2000;
        /// </summary>
        /// <param name="listaImportiEsteri"></param>
        /// <returns></returns>
        public static bool GetDec2000(List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri)
        {
            bool dec2000 = false;

            if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
            {
                for (int i = 0; i < listaImportiEsteri.Count; i++)
                {
                    if ((listaImportiEsteri[i].DecorrenzaPrestazioneEE.HasValue && Utility.DataSuccessivaA(listaImportiEsteri[i].DecorrenzaPrestazioneEE.Value, new DateTime(2000, 01, 01)) && !Utility.DataSuccessivaA(listaImportiEsteri[i].DecorrenzaPrestazioneEE.Value, new DateTime(2001, 01, 01))) ||
                        (listaImportiEsteri[i].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(listaImportiEsteri[i].CessazionePrestazioneEE.Value, new DateTime(2001, 01, 01)) && (listaImportiEsteri.Count <= i + 1 || !listaImportiEsteri[i + 1].DecorrenzaPrestazioneEE.HasValue || Utility.DataSuccessivaA(listaImportiEsteri[i + 1].DecorrenzaPrestazioneEE.Value, new DateTime(2001, 01, 01)))))
                        dec2000 = true;
                }
            }

            return dec2000;
        }

        /// <summary>
        /// 117.22.	Se DEC(IND-BIS INDICE) è uguale a 200301 effettuare le seguenti operazioni :
        /// 117.22.1.	Se SI0301 non è uguale a "S" e IND-BIS è maggiore di 1 effettuare le seguenti operazioni :
        /// 117.22.1.1.	Se ( DEC(1  1) è uguale a zero e IND-BIS è uguale a 2 )  oppure ( CES(1  1) è maggiore di zero e IND-BIS è uguale a 2 ) oppure ( DEC(2  1) è uguale a zero e 
        /// IND-BIS è uguale a 3 )  oppure ( CES(2  1) è maggiore di zero e IND-BIS è uguale a 3 ) oppure ( DEC(1  1) è maggiore di 200301 e IND-BIS è uguale a 2 ) oppure ( DEC(2  1) 
        /// è maggiore di 200301 e IND-BIS è uguale a 3 ) oppure ( PR-RI(1) è uguale a zero e IND-BIS è uguale a 2 ) oppure ( PR-RI(2) è uguale a zero e IND-BIS è uguale a 3 ) oppure 
        /// ( IND-BIS è uguale a 2   e   ULT-RIG-1 è maggiore di zero  e  CES(1  ULT-RIG-1) è maggiore di zero e CES(1  ULT-RIG-1)  è minore di 200302) oppure ( IND-BIS è uguale a 3 
        /// e   ULT-RIG-2 è maggiore di zero  e  CES(2  ULT-RIG-2) è maggiore di zero e CES(2  ULT-RIG-2)  è minore di 200302) oppure ( IND-BIS è uguale a 4   e   ULT-RIG-3 è 
        /// maggiore di zero  e CES(3  ULT-RIG-3) è maggiore di zero e CES(3  ULT-RIG-3)  è minore di 200302) valorizzare con "S" il campo SI0301 diversamente valorizzare con "N" il 
        /// campo SI0301;                              
        /// 117.22.2.	Diversamente da quanto analizzato nel punto precedente (116.22.1) valorizzare con "S" il campo SI0301;
        /// 117.23.	Se DEC(IND-BIS INDICE) è maggiore di 200301 e  INDICE è maggiore di 1 e SI0301 è uguale a "N" e (CES(IND-BIS IND-1) è maggiore di zero e è minore di 200302) 
        /// valorizzare con "S" il campo SI0301 e con "00" il campo COD0301;
        /// 117.24.	Se DEC(IND-BIS INDICE) è minore di 200301 e SI0301 è uguale a spazi valorizzare con "N" il campo SI0301 e con STATO (IND-BIS) il campo COD0301;
        /// </summary>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="precedenteCessazionePrestazioneEE"></param>
        /// <param name="dataCompare"></param>
        /// <param name="indexStato"></param>
        /// <param name="arrayImportiEsteriPerStato"></param>
        /// <param name="ultimaRiga"></param>
        /// <param name="pr_ri"></param>
        /// <param name="siXX01"></param>
        /// <param name="codXX01"></param>
        /// <param name="codicePrimoStato"></param>
        private static void GetValuesForPresenzaDec01_XX(DateTime? decorrenzaPrestazioneEE, DateTime? precedenteCessazionePrestazioneEE, DateTime? dataCompare, int indexStato, List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>[] arrayImportiEsteriPerStato, int[] ultimaRiga, int[] pr_ri, ref string siXX01, ref string codXX01, string codicePrimoStato)
        {
            if (decorrenzaPrestazioneEE.HasValue && decorrenzaPrestazioneEE.Equals(new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 01)))
            {
                if (!siXX01.Equals("S") && indexStato > 0)
                {
                    if ((!arrayImportiEsteriPerStato[0][0].DecorrenzaPrestazioneEE.HasValue && indexStato == 1) || (arrayImportiEsteriPerStato[0][0].CessazionePrestazioneEE.HasValue && indexStato == 1) || (!arrayImportiEsteriPerStato[1][0].DecorrenzaPrestazioneEE.HasValue && indexStato == 2) || (arrayImportiEsteriPerStato[1][0].CessazionePrestazioneEE.HasValue && indexStato == 2) || (arrayImportiEsteriPerStato[0][0].DecorrenzaPrestazioneEE.HasValue && Utility.DataStrettamenteSuccessivaA(arrayImportiEsteriPerStato[0][0].DecorrenzaPrestazioneEE.Value, new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 31)) && indexStato == 1) ||
                        (arrayImportiEsteriPerStato[1][0].DecorrenzaPrestazioneEE.HasValue && Utility.DataStrettamenteSuccessivaA(arrayImportiEsteriPerStato[1][0].DecorrenzaPrestazioneEE.Value, new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 31)) && indexStato == 2) || (pr_ri[0] == 0 && indexStato == 1) || (pr_ri[1] == 0 && indexStato == 2) || (indexStato == 1 && ultimaRiga[0] > 0 && arrayImportiEsteriPerStato[0][ultimaRiga[0] - 1].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[0][ultimaRiga[0] - 1].CessazionePrestazioneEE.Value, new DateTime(dataCompare.Value.AddMonths(1).Year, dataCompare.Value.AddMonths(1).Month, 01))) ||
                        (indexStato == 2 && ultimaRiga[1] > 0 && arrayImportiEsteriPerStato[1][ultimaRiga[1] - 1].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[1][ultimaRiga[1] - 1].CessazionePrestazioneEE.Value, new DateTime(dataCompare.Value.AddMonths(1).Year, dataCompare.Value.AddMonths(1).Month, 01))) || (indexStato == 3 && ultimaRiga[2] > 0 && arrayImportiEsteriPerStato[2][ultimaRiga[2] - 1].CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(arrayImportiEsteriPerStato[2][ultimaRiga[2] - 1].CessazionePrestazioneEE.Value, new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 01).AddMonths(1))))
                        siXX01 = "S";
                    else
                        siXX01 = "N";
                }
                else
                    siXX01 = "S";
            }

            if (decorrenzaPrestazioneEE.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPrestazioneEE.Value, new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 01)) && siXX01.Equals("N") && (precedenteCessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(precedenteCessazionePrestazioneEE.Value, new DateTime(dataCompare.Value.AddMonths(1).Year, dataCompare.Value.AddMonths(1).Month, 01))))
            {
                siXX01 = "S";
                codXX01 = "00";
            }

            if (decorrenzaPrestazioneEE.HasValue && !Utility.DataSuccessivaA(decorrenzaPrestazioneEE.Value, new DateTime(dataCompare.Value.Year, dataCompare.Value.Month, 01)) && siXX01.Equals(string.Empty))
            {
                siXX01 = "S";
                codXX01 = codicePrimoStato;
            }
        }

        /// <summary>
        /// 117.33.	Se  DEC(IND-BIS INDICE) è maggiore di 199600 e SI9601 è uguale a "N" effettuare le seguenti operazioni :
        /// 117.33.1.	Se ( (STATO (IND-BIS)  è uguale a 14, 34 oppure a 22)  e ICI2RESEST è uguale a "I  " e STATO (IND-BIS) è uguale a  ICI2CONV  )  oppure ( INDICE è maggiore di 
        /// 1  e CES(IND-BIS INDICE - 1) è maggiore di zero ed è minore di 199600 ) continuare l’elaborazione al punto successivo (116.34);             
        /// 117.33.2.	Diversamente da quanto analizzato nel punto precedente effettuare le segueti operazioni :
        /// 117.33.2.1.	Se  COD9601 è uguale a 14, 34 oppure a 22 valorizzare con  DEC(IND-BIS INDICE) il campo W-DECRES  e eseuguire la subroutine VEDI-RESID;
        /// 117.33.2.2.	Se  (COD9601 è uguale a 14, 34 oppure a 22) e STATO (IND-BIS) è uguale a ICI2CONV  e (W-RESOUT è uguale a "I  " oppure (W-DECRES è maggiore di 1996 ) ) 
        /// continuare l’elaborazione al punto successivo (116.34);                          
        /// 117.33.2.3.	Diversamente da quanto analizzato nel punto precedente valorizzare con 1996 il campo ERR-335C e con COD9601  il campo ERR-335S; 
        /// </summary>
        /// <param name="cessazionePrestazioneEE"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="decorrenzaPrestazioneEE"></param>
        /// <param name="precedenteCessazionePrestazioneEE"></param>
        /// <param name="siXX01"></param>
        /// <param name="codXX01"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="listaResidenzeEstere"></param>
        /// <param name="dataCompare"></param>
        /// <param name="err335C"></param>
        /// <param name="err335S"></param>
        private static void GetValuesForERR335(DateTime? cessazionePrestazioneEE, byte? codiceConvenzione, DateTime? decorrenzaPrestazioneEE, DateTime? precedenteCessazionePrestazioneEE, string siXX01, string codXX01, string codiceStatoEE, string codiceComuneResidenza, List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere, DateTime? dataCompare, bool is96, ref int err335C, ref string err335S)
        {
            if (cessazionePrestazioneEE.HasValue && Utility.DataSuccessivaA(decorrenzaPrestazioneEE.Value, dataCompare.Value) && siXX01.Equals("N"))
            {
                if (!is96)
                    dataCompare = dataCompare.Value.AddMonths(1);
                if (!(((new List<string> { "14", "34", "22", codiceConvenzione.GetValueOrDefault().ToString() }).Contains(codiceStatoEE) && !codiceComuneResidenza.StartsWith("Z")) || (precedenteCessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(precedenteCessazionePrestazioneEE.Value, dataCompare.Value))))
                {
                    char? result = ' ';
                    DateTime? appDecorrenza = decorrenzaPrestazioneEE;
                    if ((new List<string> { "14", "34", "22" }).Contains(codXX01))
                        result = GetStatoResidenzaByImportiEsteri(ref appDecorrenza, listaResidenzeEstere, codiceComuneResidenza);

                    if (!((new List<string> { "14", "34", "22" }).Contains(codXX01) && codiceStatoEE.Equals(codiceConvenzione.GetValueOrDefault().ToString()) && (result.GetValueOrDefault() == 'I' || Utility.DataSuccessivaA(appDecorrenza.Value, dataCompare.Value))))
                    {
                        err335C = dataCompare.Value.Year;
                        err335S = codXX01;
                    }
                }
            }
        }

        /// <summary>
        /// 14.3.	Valorizzare RIGA-YU con “N”
        /// 14.4.	Se IW1CARIC è uguale a 2, 5 o 9 effettuare I seguenti controlli :
        /// 14.4.1.	Se ICI2CONV è uguale a 13 o 14 : se ICI2RESEST uguale a "I  " effettuare i seguenti controlli :
        /// 14.4.1.1.	Se STATO (IND-STA) è uguale a 13, 38, 39, 42, 43, 56, o 57  oppure se : STATO (IND-STA) è uguale a 14 e  IW1DEORIG è inferiore al 198401 e DEC (IND-STA  1) è 
        /// maggiore di zero Valorizzare RIGA-YU con “S”;
        /// </summary>
        /// <param name="causaCarico"></param>
        /// <param name="codiceConvenzione"></param>
        /// <param name="codiceComuneResidenza"></param>
        /// <param name="codiceStatoEE"></param>
        /// <param name="primaDecorrenzaImportiEsteri"></param>
        /// <param name="decorrenzaOriginaria"></param>
        /// <returns></returns>
        private static bool GetRigaYugoslavia(byte? causaCarico, byte? codiceConvenzione, string codiceComuneResidenza, int? codiceStatoEE, DateTime? primaDecorrenzaImportiEsteri, DateTime? decorrenzaOriginaria)
        {
            bool riga_yu = false;

            if (causaCarico.GetValueOrDefault() == 2 || causaCarico.GetValueOrDefault() == 5 || causaCarico.GetValueOrDefault() == 9)
            {
                if (codiceConvenzione.GetValueOrDefault() == 13 || codiceConvenzione.GetValueOrDefault() == 14)
                {
                    if (string.IsNullOrEmpty(codiceComuneResidenza) || !codiceComuneResidenza.StartsWith("Z"))
                    {
                        if ((new List<int> { 13, 38, 39, 42, 43, 56, 57 }).Contains(codiceStatoEE.GetValueOrDefault()) || (codiceStatoEE.GetValueOrDefault() == 14 && !Utility.DataSuccessivaA(decorrenzaOriginaria.Value, new DateTime(1984, 01, 01)) && primaDecorrenzaImportiEsteri.HasValue))
                        {
                            riga_yu = true;
                        }
                    }
                }
            }

            return riga_yu;
        }
        #endregion PCIPL12

        #region PCIPL15

        public static bool ConfrontaRecordAdiacenti(List<AltraPensione> LaltraPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (LaltraPensione.Count > 1)
            {
                for (int i = LaltraPensione.Count - 1; i >= 2; i--)
                {
                    if (LaltraPensione[i].Certificato == LaltraPensione[i - 1].Certificato && LaltraPensione[i].Categoria == LaltraPensione[i - 1].Categoria)
                    {
                        if (LaltraPensione[i].Decorrenza != LaltraPensione[i - 1].Cessazione)
                        {
                            messaggioVideo = "Se pensione precendente cessata, la decorrenza deve essere consecutiva";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion PCIPL15

        #endregion Routine

        #endregion Convenzioni Internazionali
    }
}
