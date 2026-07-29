using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione;
using System.Text.RegularExpressions;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for ArcaTest
    /// </summary>
    [TestClass]
    public class ArcaTest
    {
        public ArcaTest()
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

        //Recupero anagrafica per codice fiscale
        [TestMethod]
        public void TestGetAreaARCAPerCodiceFiscale()
        {
            DataTable anagrafica = null;
            DataTable pensioniRiferimento = null;
            string errori = "";
            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
            richiestaArca.Applicazione = ConfigurationManager.AppSettings["SvrARCA.APP"].ToString();
            richiestaArca.Matricola = "99999998";
            richiestaArca.Provenienza = ConfigurationManager.AppSettings["SvrARCA.PROV"].ToString();
            richiestaArca.Ruolo = ConfigurationManager.AppSettings["SvrARCA.RUOLO"].ToString();
            richiestaArca.CodiceFiscale = "LNIMRC20B47L747D";
            bool bTest = GestioneARCA.GetAreaArcaByCodiceFiscale(richiestaArca, 0.ToString(), out anagrafica, out pensioniRiferimento, out errori);
            Assert.IsTrue(bTest && String.IsNullOrEmpty(errori) && anagrafica != null, errori);
        }

        //Recupero anagrafica per dati parziali
        [TestMethod]
        public void TestGetAreaARCAPerDatiParziali()
        {
            DataTable anagrafica = null;
            DataTable pensioniRiferimento = null;
            DataTable sinonimi = null;
            string errori = "";
            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
            richiestaArca.Applicazione = ConfigurationManager.AppSettings["SvrARCA.APP"].ToString();
            richiestaArca.Matricola = "E0004998";
            richiestaArca.Provenienza = ConfigurationManager.AppSettings["SvrARCA.PROV"].ToString();
            richiestaArca.Ruolo = ConfigurationManager.AppSettings["SvrARCA.RUOLO"].ToString();
            richiestaArca.Nome = "Pasquale";
            richiestaArca.Cognome = "Cozzolino";
            //richiestaArca.DataNascita = new DateTime(1983,10,2);
            //chiamata senza sinonimi
            bool bTest = GestioneARCA.GetAreaArcaByDatiPersonaliParziali(richiestaArca, 0.ToString(), out anagrafica, out pensioniRiferimento, out sinonimi, out errori);
            Assert.IsTrue(bTest && String.IsNullOrEmpty(errori) && (anagrafica != null || (anagrafica == null && sinonimi != null)), errori);

            //chiamata con sinonimi
            richiestaArca.DataNascita = null;
            bTest = GestioneARCA.GetAreaArcaByDatiPersonaliParziali(richiestaArca, 0.ToString(), out anagrafica, out pensioniRiferimento, out sinonimi, out errori);
            Assert.IsTrue(bTest && String.IsNullOrEmpty(errori) && (anagrafica != null || (anagrafica == null && sinonimi != null)), errori);
        }

        //Recupero anagrafica per codice fiscale
        [TestMethod]
        public void TestGetAnagraficaARCAPerCodiceFiscale()
        {
            Entity.Anagrafica anagrafica = null;
            string errori = "";
            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
            richiestaArca.Applicazione = ConfigurationManager.AppSettings["SvrARCA.APP"].ToString();
            richiestaArca.Matricola = "12345678";
            richiestaArca.Provenienza = ConfigurationManager.AppSettings["SvrARCA.PROV"].ToString();
            richiestaArca.Ruolo = ConfigurationManager.AppSettings["SvrARCA.RUOLO"].ToString();
            richiestaArca.CodiceFiscale = "RBRFNC40A01C983O";
            bool bTest = GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, 0.ToString(), out anagrafica, out errori);
            Assert.IsTrue(bTest && String.IsNullOrEmpty(errori) && anagrafica != null, errori);

            //codice fiscale non valido
            richiestaArca.CodiceFiscale = "CZZPQL28T28H243W";
            bTest = GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, 0.ToString(), out anagrafica, out errori);
            Assert.IsTrue(!bTest && !String.IsNullOrEmpty(errori) && anagrafica == null, errori);
        }

        [TestMethod]
        public void TestGetAnagraficaARCAPerCodiceSoggetto()
        {
            string sCodiceFiscale = "TTNMRB68H16I690H";
            Regex Rex = new Regex(@"^([A-Za-z]{6}[0-9lmnpqrstuvLMNPQRSTUV]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9lmnpqrstuvLMNPQRSTUV]{2}[A-Za-z]{1}[0-9lmnpqrstuvLMNPQRSTUV]{3}[A-Za-z]{1})$");
            Match M = Rex.Match(sCodiceFiscale);
            Assert.IsTrue(M.Success);



            DataTable anagrafica = null;
            DataTable pensioniRiferimento = null;
            string errori = "";
            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
            richiestaArca.Applicazione = ConfigurationManager.AppSettings["SvrARCA.APP"].ToString();
            richiestaArca.Matricola = "99999998";
            richiestaArca.Provenienza = ConfigurationManager.AppSettings["SvrARCA.PROV"].ToString();
            richiestaArca.Ruolo = ConfigurationManager.AppSettings["SvrARCA.RUOLO"].ToString();
            richiestaArca.CSog = 11105;
            bool bTest = GestioneARCA.GetAreaArcaByCodiceSoggetto(richiestaArca, 0.ToString(), out anagrafica, out pensioniRiferimento, out errori);
            Assert.IsTrue(bTest && String.IsNullOrEmpty(errori) && anagrafica != null, errori);
        }
    }
}
