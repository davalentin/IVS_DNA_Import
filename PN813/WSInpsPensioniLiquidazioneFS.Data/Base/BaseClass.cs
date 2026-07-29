using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Data.HostResponse
{
    public class BaseClass
    {
        internal protected byte[] RimuoviDelimiter(byte[] areaCompressaNonConvertita)
        {
            try
            {
                List<byte> areaSenzaDelimiter = new List<byte>();
                //con un offset iniziale di 134 occorre ogni 240 eliminare il 241esimo byte
                int offset = 134;
                int scarto = 241;
                int numblocchi = 0;

                for (int i = 0; i < areaCompressaNonConvertita.Length; i++)
                {
                    if (i != offset + scarto * numblocchi && i != areaCompressaNonConvertita.Length - 1)
                        areaSenzaDelimiter.Add(areaCompressaNonConvertita[i]);
                    else
                        numblocchi++;
                }
                return areaSenzaDelimiter.ToArray<byte>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

