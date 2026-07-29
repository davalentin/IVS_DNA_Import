using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for DetrazioniTest
    /// </summary>
    [TestClass]
    public class DetrazioniTest
    {
        public DetrazioniTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            RoleApplications roleApplications;
            RoleApplication roleApplication;
            IdmIdentity ident;

            ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            roleApplications = new RoleApplications();
            roleApplication = roleApplications.Add("Dominio1.Gruppo1.Processo1");
            roleApplication.Add("Ruolo1");

            System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, roleApplications);

            INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];
        }


        [TestMethod]
        public void TestGetDetrazioni()
        {
            string errori = "";
            GestioneDetrazioni.RichiestaDetrazioni richiesta = new GestioneDetrazioni.RichiestaDetrazioni();
            //presenza detrazioni
            richiesta.CodiceProcedura = 103;
            richiesta.AnnoFiscale = 2011;
            richiesta.CategoriaPensione = 63;
            richiesta.CodiceFiscale = "LTTDRV45B02H501E";
            richiesta.DataDecorrenza = new DateTime(2011, 03, 01);
            GestioneDetrazioni.RispostaDetrazioni risposta = null;
            if (!GestioneDetrazioni.GetDetrazioniUniDetra(richiesta, out risposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni == null, risposta.MessaggioRitorno);

            //nessuna detrazione per categoria bloccante
            richiesta.AnnoFiscale = 2010;
            if (!GestioneDetrazioni.GetDetrazioniUniDetra(richiesta, out risposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito != GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni != null, risposta.MessaggioRitorno);

            //nessuna detrazione per categoria non bloccante ma non compatibile
            richiesta.AnnoFiscale = 2011;
            richiesta.CategoriaPensione = 33;
            if (!GestioneDetrazioni.GetDetrazioniUniDetra(richiesta, out risposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito != GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni != null, risposta.MessaggioRitorno);

            //codice procedura errato
            richiesta.CodiceProcedura = 111;
            if (!GestioneDetrazioni.GetDetrazioniUniDetra(richiesta, out risposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito != GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni != null, risposta.MessaggioRitorno);
        }

        [TestMethod]
        public void TestVerificaDetrazioniCasiParticolari()
        {
            string errori = "";
            GestioneDetrazioni.RichiestaDetrazioni richiesta = new GestioneDetrazioni.RichiestaDetrazioni();
            //presenza detrazioni
            richiesta.CodiceProcedura = 103;
            richiesta.AnnoFiscale = 2011;
            richiesta.CategoriaPensione = 63;
            richiesta.CodiceFiscale = "LTTDRV45B02H501E";
            richiesta.DataDecorrenza = new DateTime(2011, 03, 01);
            GestioneDetrazioni.RispostaDetrazioni risposta = null;
            if (!GestioneDetrazioni.GetDetrazioniUniDetra(richiesta, out risposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni == null, risposta.MessaggioRitorno);

            //verifica ok
            GestioneDetrazioni.RispostaDetrazioni ultimaRisposta = null;
            if (!GestioneDetrazioni.VerificaDetrazioni(richiesta, risposta.Detrazioni, out ultimaRisposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(ultimaRisposta == null, "Risposta nulla");
            Assert.IsFalse(ultimaRisposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || ultimaRisposta.Detrazioni == null, ultimaRisposta.MessaggioRitorno);

            //verifica ko
            risposta.Detrazioni.AddizionaleLombardiaVeneto = 1;
            risposta.Detrazioni.AgevolazionePensionati = 1;
            if (!GestioneDetrazioni.VerificaDetrazioni(richiesta, risposta.Detrazioni, out ultimaRisposta, out errori, null))
                Assert.Fail(errori);
            Assert.IsFalse(ultimaRisposta == null, "Risposta nulla");
            Assert.IsFalse(ultimaRisposta.Esito != GestioneDetrazioni.TipoRitornoDetrazioni.Errore || ultimaRisposta.Detrazioni == null, ultimaRisposta.MessaggioRitorno);
        }
        [TestMethod]
        public void TestSaveDeleteDetrazioni()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2005507600002, null, out idPensione);

            BLCommon.GestioneDetrazioniImposta.DatiDetrazioni detrazioni = new BLCommon.GestioneDetrazioniImposta.DatiDetrazioni();
            detrazioni.AddizionaleLombardiaVeneto = 1;
            detrazioni.AgevolazionePensionati = 0;
            detrazioni.AltriFamiliari100 = 0;
            detrazioni.AltriFamiliari50 = 1;
            detrazioni.ConiugeOFiglio = 1;
            detrazioni.DetrazioniReddito = 0;
            detrazioni.FigliMaggiori3AnniHandicap100 = 1;
            detrazioni.FigliMaggiori3AnniHandicap50 = 0;
            detrazioni.FigliMaggiori3AnniNoHandicap100 = 0;
            detrazioni.FigliMaggiori3AnniNoHandicap50 = 1;
            detrazioni.FigliMinori3AnniHandicap100 = 1;
            detrazioni.FigliMinori3AnniHandicap50 = 0;
            detrazioni.FigliMinori3AnniNoHandicap100 = 1;
            detrazioni.FigliMinori3AnniNoHandicap50 = 0;
            detrazioni.DecorrenzaDetrazioneImposte = new DateTime(2010, 01, 01);
            BLCommon.GestioneDetrazioniImposta.SalvaDetrazioni(idPensione, detrazioni);

            BLCommon.GestioneDetrazioniImposta.GetDetrazioniByIdPensione(idPensione, out detrazioni);

            Assert.IsTrue(detrazioni != null && detrazioni.AddizionaleLombardiaVeneto == 1 &&
                detrazioni.AltriFamiliari50 == 1, "Detrazioni non valorizzate correttamente");

            BLCommon.GestioneDetrazioniImposta.EliminaDetrazioniByIdPensione(idPensione, false);

            BLCommon.GestioneDetrazioniImposta.GetDetrazioniByIdPensione(idPensione, out detrazioni);

            Assert.IsTrue(detrazioni == null, "Detrazioni non nulle");
        }

        [TestMethod]
        public void TestGetDetrazioniByDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2008612600015, null, out datiPensione);

            string errori = "";
            GestioneDetrazioni.RispostaDetrazioni risposta = null;
            if (!GestioneDetrazioni.GetDetrazioniByDatiPensione(datiPensione, "CSRPMR51M49C621B", false, 0, out risposta, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni == null || risposta.Url == "", risposta.MessaggioRitorno);

            //domanda non presente
            if (GestioneDetrazioni.GetDetrazioniByDatiPensione(datiPensione, "CSRPMR51M49C621B", false, 0, out risposta, out errori))
                Assert.Fail("Nessun errore sollevato");
        }

        [TestMethod]
        public void TestVerificaDetrazioniByDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            Assert.IsFalse(datiPensione == null, "Pensione non presente");

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            string errori = "";
            GestioneDetrazioni.RispostaDetrazioni risposta = null;
            if (!GestioneDetrazioni.GetDetrazioniByDatiPensione(datiPensione, datiAnagrafici.CodiceFiscale, false, datiAnagrafici.Id, out risposta, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(risposta == null, "Risposta nulla");
            Assert.IsFalse(risposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || risposta.Detrazioni == null || risposta.Url == "", risposta.MessaggioRitorno);
            // nessuna differenza
            GestioneDetrazioni.RispostaDetrazioni ultimaRisposta = null;
            if (!GestioneDetrazioni.VerificaDetrazioniByDatiPensione(datiPensione, datiAnagrafici.CodiceFiscale, datiAnagrafici.Id, false, risposta.Detrazioni, true, out ultimaRisposta, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ultimaRisposta == null, "Risposta nulla");
            Assert.IsFalse(ultimaRisposta.Esito == GestioneDetrazioni.TipoRitornoDetrazioni.Errore || ultimaRisposta.Detrazioni == null || ultimaRisposta.Url == "", ultimaRisposta.MessaggioRitorno);
            //differenza
            risposta.Detrazioni.AddizionaleLombardiaVeneto = 1;
            risposta.Detrazioni.AgevolazionePensionati = 1;
            risposta.Detrazioni.AltriFamiliari100 = 1;
            if (!GestioneDetrazioni.VerificaDetrazioniByDatiPensione(datiPensione, datiAnagrafici.CodiceFiscale, datiAnagrafici.Id, false, risposta.Detrazioni, true, out ultimaRisposta, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ultimaRisposta == null, "Risposta nulla");
            Assert.IsFalse(ultimaRisposta.Esito != GestioneDetrazioni.TipoRitornoDetrazioni.Errore || ultimaRisposta.Detrazioni == null || ultimaRisposta.Url == "", ultimaRisposta.MessaggioRitorno);

            //domanda non presente
            if (GestioneDetrazioni.VerificaDetrazioniByDatiPensione(datiPensione, datiAnagrafici.CodiceFiscale, datiAnagrafici.Id, false, risposta.Detrazioni, true, out ultimaRisposta, out errori))
                Assert.Fail("Nessun errore sollevato");
        }
    }
}

