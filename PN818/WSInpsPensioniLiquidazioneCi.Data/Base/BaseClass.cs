using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Data.HostResponse
{
    [Serializable]
    public class BaseClass
    {
        internal protected int RitornaOccorrenze(string occorrenze)
        {
            int numeroOccorrenze = 0;

            try
            {
                numeroOccorrenze = int.Parse(occorrenze);
            }
            catch (Exception)
            {
                numeroOccorrenze = 0;
            }

            return numeroOccorrenze;
        }

        internal protected byte[] Convert(byte[] areaCompressaNonConvertita, string areaCompressaConvertita)
        {
            #region RegoleConversione
            //*    ascii  ebcdic
            //*     60     76    <       un byte
            //*     62    110    >      due bytes
            //*     63    111    ?      tre bytes
            //*     61    126    =  quattro bytes
            //*    250    179    ·   cinque bytes
            #endregion RegoleConversione

            List<byte> datiDecompressi = new List<byte>();

            try
            {
                for (int i = 0; i < areaCompressaConvertita.Length; i++)
                {
                    switch (areaCompressaConvertita[i])
                    {
                        case '<':
                            EseguiDecompressione(ref datiDecompressi, areaCompressaNonConvertita, areaCompressaConvertita, ref i, 1);
                            break;
                        case '>':
                            EseguiDecompressione(ref datiDecompressi, areaCompressaNonConvertita, areaCompressaConvertita, ref i, 2);
                            break;
                        case '?':
                            EseguiDecompressione(ref datiDecompressi, areaCompressaNonConvertita, areaCompressaConvertita, ref i, 3);
                            break;
                        case '=':
                            EseguiDecompressione(ref datiDecompressi, areaCompressaNonConvertita, areaCompressaConvertita, ref i, 4);
                            break;
                        case '·':
                            EseguiDecompressione(ref datiDecompressi, areaCompressaNonConvertita, areaCompressaConvertita, ref i, 5);
                            break;
                        default:
                            datiDecompressi.Add(areaCompressaNonConvertita[i]);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return datiDecompressi.ToArray<byte>();
        }

        private void EseguiDecompressione(ref List<byte> datiDecompressi, byte[] areaCompressaNonConvertita, string areaCompressaConvertita, ref int indiceCiclo, int salto)
        {
            int occorrenze = RitornaOccorrenze(areaCompressaConvertita.Substring(indiceCiclo + 1, salto));
            if (occorrenze > 0)
            {
                for (int j = 0; j < occorrenze; j++)
                    datiDecompressi.Add(areaCompressaNonConvertita[indiceCiclo + salto + 1]);
                indiceCiclo = indiceCiclo + salto + 1;
            }
            else if (areaCompressaNonConvertita[indiceCiclo] == areaCompressaNonConvertita[indiceCiclo + 1])
            {
                datiDecompressi.Add(areaCompressaNonConvertita[indiceCiclo]);
                indiceCiclo++;
            }
            else
                datiDecompressi.Add(areaCompressaNonConvertita[indiceCiclo]);
        }
    }
}
