using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Roles;
using INPS.DNA.Context;
using INPS.DNA.Security.Idm;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for SindacatiTest
    /// </summary>
    [TestClass]
    public class SindacatiTest
    {
        public SindacatiTest()
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
        public void TestGetElencoSindacatiByCategoria()
        {
            string categoria = "063";//"VEL   ";
            string errori = string.Empty;
            List<Liquidazione.BLCommon.Entity.Sindacato> elencoSindacati = null;

            GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(categoria, out elencoSindacati, out errori);
            Assert.IsFalse(!String.IsNullOrEmpty(errori));

            elencoSindacati = Liquidazione.BLCommon.GestioneSindacati.GetElencoSindacatiAttivi(elencoSindacati, out errori);
            Assert.IsFalse(!String.IsNullOrEmpty(errori));



            Liquidazione.BLCommon.Entity.Sindacato Sindacato = null;
            if (elencoSindacati.Count > 0)
                GestioneDelegheSindacali.DecodificaCodiceSindacato(elencoSindacati.ToArray()[5].Id, out Sindacato, out errori);

            Assert.IsFalse(!String.IsNullOrEmpty(errori));

        }

        [TestMethod]
        public void TestDelegheSindacali()
        {
            INPS.Pensioni.Liquidazione.BLCommon.GestionePensione.DatiPensione datiPensione = null;
            BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(2008595200005, null, out datiPensione);

            string categoria = "200";
            string errori = string.Empty;
            List<Liquidazione.BLCommon.Entity.Sindacato> elencoSindacati = null;

            GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(categoria, out elencoSindacati, out errori);
            Assert.IsFalse(!String.IsNullOrEmpty(errori));

            elencoSindacati = Liquidazione.BLCommon.GestioneSindacati.GetElencoSindacatiAttivi(elencoSindacati, out errori);
            Assert.IsFalse(!String.IsNullOrEmpty(errori));

            Liquidazione.BLCommon.Entity.Sindacato Sindacato = null;
            if (elencoSindacati.Count > 0)
                GestioneDelegheSindacali.DecodificaCodiceSindacato(elencoSindacati.ToArray()[5].Id, out Sindacato, out errori);

            Assert.IsFalse(!String.IsNullOrEmpty(errori));

            INPS.Pensioni.Liquidazione.BLCommon.GestionePensione.DatiSindacato datiSindacato = null;
            BLCommon.GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out datiSindacato);
            string IdCategoria = BLCommon.GestioneSindacati.GetIdCategoriaForSindacato(datiPensione.SiglaCategoria, out errori);

            GestioneDelegheSindacali.VerificaCodiceSindacato(IdCategoria, datiPensione.NCertificato.GetValueOrDefault(), datiSindacato.CodiceSindacato, datiPensione.DecorrenzaOriginaria, out errori);
            Assert.IsFalse(!GestioneDelegheSindacali.VerificaCompatibilita(IdCategoria, datiPensione.NCertificato.GetValueOrDefault(), datiPensione.DecorrenzaOriginaria, datiSindacato.CodiceSindacato, out errori));
        }

        [TestMethod]
        public void TestRecuperoSindacati()
        {
            string categoria = "001";//"VEL   ";
            string errori = string.Empty;
            List<BLCommon.Entity.Sindacato> elencoSindacati = null;

            GestioneDelegheSindacali.GetElencoSindacatiPerCategoria(categoria, out elencoSindacati, out errori);
            Assert.IsFalse(!String.IsNullOrEmpty(errori));

            System.IO.File.Delete("C:\\Sindacati.xml");
            if (elencoSindacati != null && elencoSindacati.Count > 0)
            {
                string xmlTotale = BLCommon.Utility.GetXmlFromObject(elencoSindacati);
                System.IO.File.AppendAllText("C:\\Sindacati.xml", xmlTotale);
            }
        }
    }
}
