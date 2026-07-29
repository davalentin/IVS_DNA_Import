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
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for RedditiTest
    /// </summary>
    [TestClass]
    public class RedditiTest
    {
        public RedditiTest()
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
        public void TestGetStoreCancelRedditiDRedd()
        {
            
            
            GestionePensione.DatiPensione datiPensione;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2038517500007, null, out datiPensione);
            long idPensione = datiPensione.Id;

            BLCommon.GestioneRedditi.EliminaAllRedditiDRedd(idPensione);
            List<BLCommon.GestioneRedditi.RedditoDRedd> redditiDRedd = null;
            BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDRedd);
            Assert.IsTrue(redditiDRedd == null || redditiDRedd.Count == 0, "1:Redditi DRedd non nulli");

            redditiDRedd = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd>();
            redditiDRedd.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2011, "01"));
            redditiDRedd.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2011, "02"));
            redditiDRedd.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2010, "02"));
            BLCommon.GestioneRedditi.SalvaRedditiDRedd(datiPensione, redditiDRedd);

            redditiDRedd = null;
            BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDRedd);
            Assert.IsTrue(redditiDRedd != null && redditiDRedd.Count == 3, "1:Redditi DRedd non corretti");

            BLCommon.GestioneRedditi.EliminaRedditoDRedd(idPensione, new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2011, "02"));
            redditiDRedd = null;
            BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDRedd);
            Assert.IsTrue(redditiDRedd != null && redditiDRedd.Count == 2 && redditiDRedd[0].AnnoReddito == 2010 && redditiDRedd[1].AnnoReddito == 2011,
                "2:Redditi DRedd non corretti");

            redditiDRedd = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd>();
            redditiDRedd.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2011, "01"));
            redditiDRedd.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd(2010, "03"));
            BLCommon.GestioneRedditi.SalvaRedditiDRedd(datiPensione, redditiDRedd);
            redditiDRedd = null;
            BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDRedd);
            Assert.IsTrue(redditiDRedd != null && redditiDRedd.Count == 2 && redditiDRedd[0].Rilevanza == "03" && redditiDRedd[1].Rilevanza == "01",
                "3:Redditi DRedd non corretti");

            redditiDRedd = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd>();
            BLCommon.GestioneRedditi.SalvaRedditiDRedd(datiPensione, redditiDRedd);

            redditiDRedd = null;
            BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDRedd);
            Assert.IsTrue(redditiDRedd == null || redditiDRedd.Count == 0, "2:Redditi DRedd non nulli");

        }


        [TestMethod]
        public void TestGetVerifyRedditiByNumeroDomanda()
        {
            EntityBLCommon.ContenitoreObject contenitore = new EntityBLCommon.ContenitoreObject(2038700200001, null);
            EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica = new EntityBLCommon.ContenitoreDecodifica(contenitore);

            GestioneRedditi.AreaRedditi areaRedditi = null;
            GestioneRedditi.GetRedditiByDatiPensione(ref contenitore, ref contenitoreDecodifica, "12345678", 2100, out areaRedditi);
            Assert.IsTrue(areaRedditi.Esito == GestioneRedditi.TipoRitornoRedditi.Errore && areaRedditi.MessaggioVideo == "Non sono presenti redditi negli archivi centrali", "1:Risultato non congruo");

            GestioneRedditi.AreaRedditi areaRedditiLast = null;
            GestioneRedditi.VerifyRedditiByDatiPensione(ref contenitore, "99999998", 2100, false, areaRedditi, null, out areaRedditiLast);
            Assert.IsTrue(areaRedditiLast.Esito == GestioneRedditi.TipoRitornoRedditi.Errore && areaRedditiLast.MessaggioVideo == "Non sono stati acquisiti redditi. Redditi obbligatori", "2:Risultato non congruo");

            areaRedditiLast = null;
            GestioneRedditi.VerifyRedditiByDatiPensione(ref contenitore, "99999998", 2100, true, areaRedditi, null, out areaRedditiLast);
            Assert.IsTrue(areaRedditiLast.Esito == GestioneRedditi.TipoRitornoRedditi.Errore && areaRedditiLast.MessaggioVideo == "Dati incompleti. Redditi non acquisiti", "3:Risultato non congruo");
        }

        [TestMethod]
        public void TestEliminaRedditiFromSrvRedditi()
        {
            string errori = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2038569900008, null, out datiPensione);

            Assert.IsFalse(GestioneRedditi.ElimninaRedditiSrvRedditiByDatiPensione(datiPensione, out errori), errori);
        }

        [TestMethod]
        public void TestVerificaPresenzaRedditi()
        {
            string errori = string.Empty;
            long ndomus = 2038569900008;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(ndomus, null, out datiPensione);
            if (datiPensione == null)
                datiPensione = new GestionePensione.DatiPensione() { NDomus = ndomus };

            bool bTest = GestioneRedditi.VerificaPresenzaRedditi(datiPensione, out errori);
        }
    }
}

