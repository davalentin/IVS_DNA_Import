using System;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Contract.AggiornaCalcoloNoInd
{
    [Serializable]
    public class CausaleDtoLite : IEquatable<CausaleDtoLite>
    {
        public int Analitica { get; set; }
        public string Descrizione { get; set; }
        public int Sintetica { get; set; }

        //I metodi di seguito sono inseriti per implementare i metodi inerenti le operazioni di uguaglianza fra oggetti complessi forniti nell'interfaccia IEquatable
        //Ciò semplifica operazioni di LINQ ove necessarie, come ad esempio utilizzo di Distinct() per estrapolare tutti i componenti diversi di una lista ed operazioni di uguaglianza fra oggetti.
        //Per ottenere l'effetto è necessario avere un metodo Equals ed eseguire l'implementazione completa dei metodi contenuti in IEquatable<T>.
        //Avere questi metodi permette di eseguire delle operazioni ottimizzate e leggibili in molti altri punti del codice semplificando lo stesso. Ottimizzando il coding e riducendo le complessità
        //Cognitiva e Computativa di IVS, attualmente piuttosto alte, non rare sono state segnalazioni di lentezza del codice e difficoltà nel interpretazione.

        // --- IEquatable<T>.Equals
        public bool Equals(CausaleDtoLite other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            // int: confronto diretto; string: normalizzo + case-insensitive
            return this.Sintetica == other.Sintetica
                && this.Analitica == other.Analitica
                && string.Equals(Norm(this.Descrizione), Norm(other.Descrizione),
                                 StringComparison.OrdinalIgnoreCase);
        }

        // --- override object.Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as CausaleDtoLite);
        }

        // --- override GetHashCode (coerente con Equals)
        public override int GetHashCode()
        {
            unchecked
            {
                int h1 = Sintetica.GetHashCode();
                int h2 = Analitica.GetHashCode();
                // Hash case-insensitive coerente con Equals
                int h3 = StringComparer.OrdinalIgnoreCase.GetHashCode(Norm(Descrizione));
                return ((h1 * 397) ^ h2) * 397 ^ h3;
            }
        }

        // Helper di normalizzazione per le stringhe
        private static string Norm(string s)
        {
            // Non c'è ToUpper o ToLower in quanto il confronto è già case-insensitive in Equals/GetHashCode
            return (s ?? string.Empty).Trim();
        }
    }
}

