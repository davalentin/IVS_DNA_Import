using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for PrepensionamentoAGOTest
    /// </summary>
    [TestClass]
    public class PrepensionamentoAGOTest
    {
        public PrepensionamentoAGOTest()
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
        public void TestInsertTOPPL03()
        {
            long numeroDomanda = 2008627200009;
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione == null)
            {
                datiPensione = new GestionePensione.DatiPensione();
                datiPensione.CodiceSede = 0500;
                datiPensione.SiglaCategoria = "VO";
                datiPensione.NCertificato = 10047719;
                datiPensione.DecorrenzaOriginaria = new DateTime(2014, 04, 01);

            }

            if (datiPensione == null)
                Assert.Fail("Dati Pensione non presenti");

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione))
            {
                GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
                GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamento);

                if (datiPrepensionamento == null)
                {
                    datiPrepensionamento = new GestionePrepensionamento.DatiPrepensionamento();
                    datiPrepensionamento.CodiceLegge = 2109;
                    datiPrepensionamento.SettimaneUtiliMisura = 76;
                    datiPrepensionamento.SettimaneMaggioreAnzianita = 23;
                    datiPrepensionamento.CessazioneBeneficioPrepensionamento = new DateTime(2014, 04, 01);
                    datiPrepensionamento.CodiceAzienda = 1234567890;
                    datiPrepensionamento.OnereMancataContribuzione = 11M;
                    datiPrepensionamento.SettimaneUtiliDiritto = 99;
                    datiPrepensionamento.CessazioneAmianto = new DateTime(2014, 04, 01);
                    datiPrepensionamento.SettimaneAmianto = 56;
                }

                if (datiPrepensionamento != null)
                {
                    GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica);

                    if (anagrafica == null)
                    {
                        anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                        anagrafica.Nome = "Mario";
                        anagrafica.Cognome = "Rossi";
                        anagrafica.Sesso = 'M';
                        anagrafica.DataNascita = new DateTime(1950, 01, 14);
                        anagrafica.CodiceFiscale = "SRRSVT30S06Z602H";
                    }

                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                    if (datiIstruttoria == null)
                    {
                        datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                        datiIstruttoria.CodiceComunicazioneCampo3 = ' ';
                    }

                    GestionePrepensionamento.InsertTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);
                }
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
                Assert.Fail(messaggioVideo);
        }

        [TestMethod]
        public void TestUpdateTOPPL03()
        {
            long numeroDomanda = 2008597000017;
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione == null)
                Assert.Fail("Dati Pensione non presenti");

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione))
            {
                GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
                GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamento);

                if (datiPrepensionamento == null)
                {
                    datiPrepensionamento = new GestionePrepensionamento.DatiPrepensionamento();
                    datiPrepensionamento.CodiceLegge = 0903;
                    datiPrepensionamento.SettimaneUtiliMisura = 11;
                    datiPrepensionamento.SettimaneMaggioreAnzianita = 11;
                    datiPrepensionamento.CessazioneBeneficioPrepensionamento = new DateTime(2014, 04, 01);
                    datiPrepensionamento.CodiceAzienda = 12345678;//4960512272;
                    datiPrepensionamento.OnereMancataContribuzione = 11M;
                    datiPrepensionamento.SettimaneUtiliDiritto = 11;
                    datiPrepensionamento.CessazioneAmianto = new DateTime(2014, 04, 01);
                    datiPrepensionamento.SettimaneAmianto = 11;
                }

                if (datiPrepensionamento != null)
                {
                    GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica);

                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                    GestionePrepensionamento.UpdateTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);
                }
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
                Assert.Fail(messaggioVideo);
        }

        [TestMethod]
        public void TestSelectTOPPL03()
        {
            long numeroDomanda = 2008627200009;
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione == null)
                Assert.Fail("Dati Pensione non presenti");

            List<GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento = null;

            GestionePrepensionamento.SelectTOPPL03(datiPensione, out listaDatiPrepensionamento, ref messaggioVideo);

            if (!string.IsNullOrEmpty(messaggioVideo))
                Assert.Fail(messaggioVideo);
        }

        [TestMethod]
        public void TestDeleteTOPPL03()
        {
            long numeroDomanda = 2008597000017;
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione == null)
                Assert.Fail("Dati Pensione non presenti");

            GestionePrepensionamento.DeleteTOPPL03(datiPensione, ref messaggioVideo);

            if (!string.IsNullOrEmpty(messaggioVideo))
                Assert.Fail(messaggioVideo);
        }

        [TestMethod]
        public void TestInsertUpdateTOPPL03()
        {
            long numeroDomanda = 2008655800007;
            string messaggioVideo = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione == null)
            {
                datiPensione = new GestionePensione.DatiPensione();
                datiPensione.CodiceSede = 0500;
                datiPensione.SiglaCategoria = "VO";
                datiPensione.NCertificato = 10047719;
                datiPensione.DecorrenzaOriginaria = new DateTime(2014, 04, 01);
                datiPensione.AttivitaEconomica = 92;
                datiPensione.ProfessioneIndividuale = 257;
            }

            if (datiPensione == null)
                Assert.Fail("Dati Pensione non presenti");

            if (Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione))
            {
                GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
                GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamento);

                if (datiPrepensionamento == null)
                {
                    datiPrepensionamento = new GestionePrepensionamento.DatiPrepensionamento();
                    datiPrepensionamento.CodiceLegge = 2109;
                    datiPrepensionamento.SettimaneUtiliMisura = 76;
                    datiPrepensionamento.SettimaneMaggioreAnzianita = 23;
                    datiPrepensionamento.CessazioneBeneficioPrepensionamento = new DateTime(2014, 04, 01);
                    datiPrepensionamento.CodiceAzienda = 1234567890;
                    datiPrepensionamento.OnereMancataContribuzione = 11M;
                    datiPrepensionamento.SettimaneUtiliDiritto = 99;
                    datiPrepensionamento.CessazioneAmianto = new DateTime(2014, 04, 01);
                    datiPrepensionamento.SettimaneAmianto = 56;
                }

                if (datiPrepensionamento != null)
                {
                    GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica);

                    if (anagrafica == null)
                    {
                        anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                        anagrafica.Nome = "Mario";
                        anagrafica.Cognome = "Rossi";
                        anagrafica.Sesso = 'M';
                        anagrafica.DataNascita = new DateTime(1950, 01, 14);
                        anagrafica.CodiceFiscale = "SRRSVT30S06Z602H";
                    }

                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                    if (datiIstruttoria == null)
                    {
                        datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();
                        datiIstruttoria.CodiceComunicazioneCampo3 = ' ';
                    }

                    List<GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento = null;
                    GestionePrepensionamento.SelectTOPPL03(datiPensione, out listaDatiPrepensionamento, ref messaggioVideo);

                    if(listaDatiPrepensionamento != null && listaDatiPrepensionamento.Count > 0)
                        GestionePrepensionamento.UpdateTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);
                    else
                        GestionePrepensionamento.InsertTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);
                }
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
                Assert.Fail(messaggioVideo);
        }
    }
}
