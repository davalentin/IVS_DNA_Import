using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for TestCrossControls
    /// </summary>
    [TestClass]
    public class CrossControlsTest
    {
        public CrossControlsTest()
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

        //Commentato in seguito alla mail: Reingegnerizzazione - controllo requisiti per vecchiaia del 07/05/2012
        //[TestMethod]
        //public void TestVerificaAnzianitaVecchiaiaNContributi() //ok
        //{
        //    GestioneCalcolo.DatiCalcoloContributivo datiContributivi = new GestioneCalcolo.DatiCalcoloContributivo();
        //    GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = new GestioneCalcolo.DatiCalcoloRetributivo();
        //    GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();

        //    datiPensione.Prodotto = "0001";
        //    datiContributivi.NSettimane      = 23;
        //    datiRetributivi.NSettimaneQuotaA = 23;
        //    datiRetributivi.NSettimaneQuotaB = 23;
        //    datiRetributivi.NSettimaneQuotaC = 23;
        //    datiRetributivi.NSettimaneQuotaD = 23;

        //    Assert.IsTrue(GestioneCrossControls.VerificaAnzianitaVecchiaiaNContributi(datiContributivi, datiRetributivi, datiPensione));
        //}

        [TestMethod]
        public void TestVerificaFamiliariGenitori() //ok
        {
            string messaggio = null;
            List<GestioneFamiliari.Familiare> LFamiliari = null;
            //List<GestioneFamiliari.Familiare> LFamiliari = new List<GestioneFamiliari.Familiare>();
            //GestioneFamiliari.Familiare fam1 = new GestioneFamiliari.Familiare();
            //fam1.SiglaFamiliare = 'C';
            //LFamiliari.Add(fam1);

            //GestioneFamiliari.Familiare fam2 = new GestioneFamiliari.Familiare();
            //fam2.SiglaFamiliare = 'G';
            //LFamiliari.Add(fam2);

            //GestioneFamiliari.Familiare fam3 = new GestioneFamiliari.Familiare();
            //fam3.SiglaFamiliare = 'G';
            //LFamiliari.Add(fam3);

            //GestioneFamiliari.Familiare fam4 = new GestioneFamiliari.Familiare();
            //fam4.SiglaFamiliare = 'G';
            //LFamiliari.Add(fam4);

            Assert.IsTrue(GestioneCrossControls.ALL_VerificaFamiliariGenitori(LFamiliari, out messaggio));
        }

        [TestMethod]
        public void TestVerificaFamiliariTitolareNoConiugato() // ok
        {
            BLCommon.Entity.AreaTitolare areaTitolare = new INPS.Pensioni.Liquidazione.BLCommon.Entity.AreaTitolare();
            List<GestioneFamiliari.Familiare> LFamiliari = new List<GestioneFamiliari.Familiare>();
            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = new BLCommon.GestioneDanteCausa.DatiDanteCausa();

            areaTitolare.ElencoStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>();

            GestioneAnagrafica.DatiStatoCivile sc1 = new GestioneAnagrafica.DatiStatoCivile();
            sc1.Codice = '3';
            sc1.Decorrenza = new DateTime(2011, 3, 1);

            areaTitolare.ElencoStatiCivili.Add(sc1);

            GestioneAnagrafica.DatiStatoCivile sc2 = new GestioneAnagrafica.DatiStatoCivile();
            sc2.Codice = '2';
            sc2.Decorrenza = new DateTime(2010, 4, 1);

            areaTitolare.ElencoStatiCivili.Add(sc2);

            GestioneAnagrafica.DatiStatoCivile sc3 = new GestioneAnagrafica.DatiStatoCivile();
            sc3.Codice = '1';
            sc3.Decorrenza = new DateTime(2011, 1, 1);

            areaTitolare.ElencoStatiCivili.Add(sc3);

            GestioneFamiliari.Familiare fam1 = new GestioneFamiliari.Familiare();
            fam1.SiglaFamiliare = 'U';
            LFamiliari.Add(fam1);

            GestioneFamiliari.Familiare fam2 = new GestioneFamiliari.Familiare();
            fam2.SiglaFamiliare = 'C';
            LFamiliari.Add(fam2);

            GestioneFamiliari.Familiare fam3 = new GestioneFamiliari.Familiare();
            fam3.SiglaFamiliare = 'G';
            LFamiliari.Add(fam3);

            GestioneFamiliari.Familiare fam4 = new GestioneFamiliari.Familiare();
            fam4.SiglaFamiliare = 'G';
            LFamiliari.Add(fam4);

            string msg = string.Empty;
            bool isRiaperturaDomanda = false;
            Assert.IsTrue(GestioneCrossControls.ALL_VerificaFamiliariConiugiTitolareConiugato(datiPensione, areaTitolare, LFamiliari, true, danteCausa, isRiaperturaDomanda, out msg));
        }

        [TestMethod]
        public void TestVerificaSupplementiWithBonus() //ok
        {
            List<BLCommon.Entity.DatiSupplementi> LSuppl = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            List<GestioneRecordFondo.DatiRecordFondo> LRecFondo = new List<GestioneRecordFondo.DatiRecordFondo>();


            BLCommon.Entity.DatiSupplementi suppl1 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            suppl1.DecorrenzaSupplemento = new DateTime(2011, 1, 1);
            LSuppl.Add(suppl1);

            BLCommon.Entity.DatiSupplementi suppl2 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            suppl2.DecorrenzaSupplemento = new DateTime(2011, 2, 1);
            LSuppl.Add(suppl2);

            GestioneRecordFondo.DatiRecordFondo fondo1 = new GestioneRecordFondo.DatiRecordFondo();
            fondo1.DecorrenzaValiditaDati = new DateTime(2011, 1, 1);
            LRecFondo.Add(fondo1);

            GestioneRecordFondo.DatiRecordFondo fondo2 = new GestioneRecordFondo.DatiRecordFondo();
            fondo2.DecorrenzaValiditaDati = new DateTime(2011, 1, 1);
            LRecFondo.Add(fondo2);

            GestioneRecordFondo.DatiRecordFondo fondo3 = new GestioneRecordFondo.DatiRecordFondo();
            fondo3.DecorrenzaValiditaDati = new DateTime(2011, 1, 1);
            LRecFondo.Add(fondo3);

            bool? AttribBonus = null;
            Assert.IsTrue(GestioneCrossControls.FS_VerificaSupplementiWithBonus(LSuppl, LRecFondo, AttribBonus, 'P', Utility.TipoFondo.EL));
        }

        [TestMethod]
        public void TestVerificaSupplementiCodiceSpecificoCodiceGestione() //ok
        {
            List<BLCommon.Entity.DatiSupplementi> LSuppl = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();

            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            datiPensione.Gruppo = "0003";
            datiPensione.Prodotto = "0022";
            datiPensione.SiglaCategoria = "SET";

            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo();
            datiFondo.CodiceSpecifico = 8;
            datiFondo.AttribuzioneBonus = true;

            BLCommon.Entity.DatiSupplementi suppl1 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            suppl1.CodGestioneSupplemento = "1";
            LSuppl.Add(suppl1);

            BLCommon.Entity.DatiSupplementi suppl2 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            suppl2.CodGestioneSupplemento = "1";
            LSuppl.Add(suppl2);

            string messaggioVideo = string.Empty;
            Assert.IsTrue(GestioneCrossControls.FS_VerificaSupplementiCodiceSpecificoCodiceGestione(datiPensione, LSuppl, datiFondo.AttribuzioneBonus, 'P', Utility.TipoFondo.EL, out messaggioVideo));
        }

        //[TestMethod]
        //public void TestVerificaConiugeInFamiliari()
        //{
        //    GestioneFamiliari.Familiare datiFam = new GestioneFamiliari.Familiare();
        //    List<GestioneFamiliari.Familiare> Lfamiliare = new List<GestioneFamiliari.Familiare>();
        //    //List<GestioneFamiliari.Familiare> Lfamiliare = null;
        //    datiFam.SiglaFamiliare = 'I';
        //    Lfamiliare.Add(datiFam);
        //    datiFam = new GestioneFamiliari.Familiare();
        //    datiFam.SiglaFamiliare = 'U';
        //    Lfamiliare.Add(datiFam);
        //    datiFam = new GestioneFamiliari.Familiare();
        //    datiFam.SiglaFamiliare = 'S';
        //    Lfamiliare.Add(datiFam);
        //    datiFam = new GestioneFamiliari.Familiare();
        //    datiFam.SiglaFamiliare = 'S';
        //    Lfamiliare.Add(datiFam);

        //    //BLCommon.Entity.AreaTitolare areaTitolare = null;
        //    BLCommon.Entity.AreaTitolare areaTitolare = new BLCommon.Entity.AreaTitolare();
        //    areaTitolare.Anagrafica.CodiceStatoCivile = 2;

        //    //Assert.IsTrue(GestioneCrossControls.VerificaConiugeInFamiliari(areaTitolare, Lfamiliare));
        //}

        [TestMethod]
        public void TestVerificaFamiliariTitolare()
        {
            GestioneFamiliari.Familiare datiFam = new GestioneFamiliari.Familiare();
            List<GestioneFamiliari.Familiare> Lfamiliare = new List<GestioneFamiliari.Familiare>();
            //List<GestioneFamiliari.Familiare> Lfamiliare = null;
            datiFam.CodiceFiscale = "MTALGN66M58F839G";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "BGGGRG54E29E488A";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "CSTSVT56T08C351U";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "BGONRC53H15D969I";
            Lfamiliare.Add(datiFam);

            //BLCommon.Entity.AreaTitolare areaTitolare = null;
            BLCommon.Entity.AreaTitolare areaTitolare = new BLCommon.Entity.AreaTitolare();
            areaTitolare.Anagrafica.CodiceFiscale = "MTALGN66M58F839G";

            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            datiPensione.Gruppo = "0003";
            datiPensione.Prodotto = "0022";
            datiPensione.SiglaCategoria = "SPT";
            datiPensione.Gestione = "007";

            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = new BLCommon.GestioneDanteCausa.DatiDanteCausa();

            Assert.IsTrue(GestioneCrossControls.ALL_VerificaFamiliariTitolare(Lfamiliare, areaTitolare, datiPensione, Utility.TipoAppartenenza.FS, false, danteCausa));
        }

        [TestMethod]
        public void TestVerificaFamiliariDuplicati()
        {
            string messaggio = null;
            GestioneFamiliari.Familiare datiFam = new GestioneFamiliari.Familiare();
            List<GestioneFamiliari.Familiare> Lfamiliare = new List<GestioneFamiliari.Familiare>();
            //List<GestioneFamiliari.Familiare> Lfamiliare = null;
            datiFam.CodiceFiscale = "MTALGN66M58F839G";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "BGGGRG54E29E488A";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "CSTSVT56T08C351U";
            Lfamiliare.Add(datiFam);
            datiFam = new GestioneFamiliari.Familiare();
            datiFam.CodiceFiscale = "MTALGN66M58F839G";
            Lfamiliare.Add(datiFam);

            Assert.IsTrue(GestioneCrossControls.ALL_VerificaFamiliariDuplicati(Lfamiliare, out messaggio));
        }

        [TestMethod]
        public void TestVerificaDecorrenzaCodMaggFamiliariConiugi()
        {
            string messaggio = null;

            List<GestioneFamiliari.Familiare> Lfamiliare = new List<GestioneFamiliari.Familiare>();

            GestioneFamiliari.Familiare datiFamA = new GestioneFamiliari.Familiare();
            datiFamA.IdAnagrafica = 3333;
            datiFamA.SiglaFamiliare = 'C';
            Lfamiliare.Add(datiFamA);

            GestioneFamiliari.Familiare datiFamB = new GestioneFamiliari.Familiare();
            datiFamB.IdAnagrafica = 1111;
            datiFamB.SiglaFamiliare = 'I';
            Lfamiliare.Add(datiFamB);

            GestioneFamiliari.Familiare datiFamC = new GestioneFamiliari.Familiare();
            datiFamC.IdAnagrafica = 2222;
            datiFamC.SiglaFamiliare = 'C';
            Lfamiliare.Add(datiFamC);
            GestioneFamiliari.Familiare datiFamD = new GestioneFamiliari.Familiare();
            datiFamD.IdAnagrafica = 4444;
            datiFamD.SiglaFamiliare = 'U';
            Lfamiliare.Add(datiFamD);

            List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = new List<GestioneFamiliari.CodMaggFamiliari>();

            GestioneFamiliari.CodMaggFamiliari codMaggFam = new GestioneFamiliari.CodMaggFamiliari();
            codMaggFam.Id = 4;
            codMaggFam.IdAnagrafica = 1111;
            codMaggFam.Decorrenza = new DateTime(2012, 04, 01);
            listaCodMaggFamiliari.Add(codMaggFam);

            codMaggFam = new GestioneFamiliari.CodMaggFamiliari();
            codMaggFam.Id = 2;
            codMaggFam.IdAnagrafica = 2222;
            codMaggFam.Decorrenza = new DateTime(2012, 03, 01);
            listaCodMaggFamiliari.Add(codMaggFam);

            codMaggFam = new GestioneFamiliari.CodMaggFamiliari();
            codMaggFam.Id = 1;
            codMaggFam.IdAnagrafica = 3333;
            codMaggFam.Decorrenza = new DateTime(2012, 03, 01);
            listaCodMaggFamiliari.Add(codMaggFam);

            codMaggFam = new GestioneFamiliari.CodMaggFamiliari();
            codMaggFam.Id = 3;
            codMaggFam.IdAnagrafica = 4444;
            codMaggFam.Decorrenza = new DateTime(2009, 01, 01);
            listaCodMaggFamiliari.Add(codMaggFam);

            Assert.IsTrue(GestioneCrossControls.ALL_VerificaDecorrenzaCodMaggFamiliariConiugi(Lfamiliare, listaCodMaggFamiliari, out messaggio));
        }

        [TestMethod]
        public void TestVerificaDecorrenzaSupplementoDecorrenzaPensione()
        {
            //List<BLCommon.Entity.DatiSupplementi> listDatiSupplementi = null;

            BLCommon.Entity.DatiSupplementi supp = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            List<BLCommon.Entity.DatiSupplementi> listDatiSupplementi = new List<BLCommon.Entity.DatiSupplementi>();
            supp.DecorrenzaSupplemento = new DateTime(2010, 10, 01);
            listDatiSupplementi.Add(supp);
            listDatiSupplementi = new List<BLCommon.Entity.DatiSupplementi>();
            supp.DecorrenzaSupplemento = new DateTime(2012, 10, 01);
            listDatiSupplementi.Add(supp);

            GestionePensione.DatiPensione datiPensione = null;

            //GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            //datiPensione.DecorrenzaOriginaria = new DateTime(2009, 12, 01);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            Assert.IsTrue(GestioneCrossControls.FS_VerificaDecorrenzaSupplementoDecorrenzaPensione(listDatiSupplementi, Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null)));
        }

        [TestMethod]
        public void TestVerificaDecPensioneProdottoForVecchiaia()
        {
            DateTime? DecPensione = new DateTime(2011, 01, 02);
            GestionePensione.DatiPensione datipensione = new GestionePensione.DatiPensione();

            datipensione.Prodotto = "0002";
            datipensione.Gruppo = "0001";
            BLCommon.GestioneCrossControls.TipoDecPensione? tipoDecPensione = BLCommon.GestioneCrossControls.ALL_VerificaDecPensioneProdottoForVecchiaiaOrAnzianitaSperDonna(DecPensione, datipensione.Gruppo, datipensione.Prodotto, datipensione.Tipo);
        }

        [TestMethod]
        public void TestVerificaEtaTitolareAnte2008()
        {
            string msg = string.Empty;
            GestioneAnagrafica.DatiAnagrafici areaTitolare = new GestioneAnagrafica.DatiAnagrafici();
            areaTitolare.DataNascita = new DateTime(1955, 1, 31);
            areaTitolare.Sesso = 'F';

            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            datiPensione.DecorrenzaOriginaria = new DateTime(2010, 1, 1);

            //char? codiceRequisito = 'A';

            //bool? isVerify = GestioneCrossControls.VerificaEtaTitolareAnte2008(areaTitolare, datiPensione, codiceRequisito, out msg);

        }

        [TestMethod]
        public void TestVerificaEtaTitolarePost2011()
        {
            string msg = string.Empty;

            GestioneAnagrafica.DatiAnagrafici areaTitolare = new GestioneAnagrafica.DatiAnagrafici();
            areaTitolare.DataNascita = new DateTime(1955, 3, 31);
            areaTitolare.Sesso = 'F';

            GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
            datiPensione.DataPerfezionamentoRequisiti = new DateTime(2010, 3, 30);

            //char? codiceRequisito = 'A';

            //bool? isVerify = GestioneCrossControls.VerificaEtaTitolarePost2011(areaTitolare, datiPensione, codiceRequisito, out msg);

        }

        [TestMethod]
        public void TestCI_ControlsPensioneDirettaDanteCausa()
        {
            string messaggioVideo = string.Empty;
            int? certificato = null;
            string categoria = null;
            string sede = string.Empty;
            DateTime? decorrenzaPensione = null;
            byte? maggiorazione780Contributivi = null;
            string naturaPensioneDC = "  V";
            DateTime? dataNascitaDC = DateTime.Now.AddYears(-50);
            DateTime? dataMorteDC = new DateTime(1968, 10, 27);
            string categoriaPensione = "SRS";
            DateTime? decorrenzaOriginaria = DateTime.Now.AddYears(-50);
            string naturaPensioneTitolare = "2  ";
            byte? causaCarico = 1;

            bool result = GestioneCrossControls.CI_ControlsPensioneDirettaDanteCausa(certificato, categoria, sede, decorrenzaPensione, maggiorazione780Contributivi, naturaPensioneDC, dataNascitaDC, dataMorteDC, categoriaPensione, decorrenzaOriginaria, naturaPensioneTitolare, causaCarico, out messaggioVideo);
        }

        [TestMethod]
        public void TestCI_VerificaCodNaturaTitolareWithDC()
        {
            string codNaturaDC = "2A ";
            string codNaturaTitolare = "1X ";

            bool result = GestioneCrossControls.CI_VerificaCodNaturaTitolareWithDC(codNaturaDC, codNaturaTitolare);
        }

        [TestMethod]
        public void TestCI_ControlsAltraPensioneWithPensioneDiretta()
        {
            string messaggioVideo = string.Empty;
            string categoriaAltraPensione = "24";
            string codNatura = "2  ";
            string categoriaPensioneDiretta = "FS";

            bool result = GestioneCrossControls.CI_ControlsAltraPensioneWithPensioneDiretta(categoriaAltraPensione, codNatura, categoriaPensioneDiretta, out messaggioVideo);
        }

        [TestMethod]
        public void TestCI_ControlsAltraPensioneDanteCausa()
        {
            string messaggioVideo = string.Empty;
            char? codiceUCAltraPensione = 'C';
            DateTime? decorrenzaAltraPensione = DateTime.Now.AddMonths(-1);
            DateTime? dataNascita = DateTime.Now.AddMonths(-1);
            DateTime? dataMorte = DateTime.Now.AddMonths(-1);
            DateTime? cessazioneAltraPensione = DateTime.Now;
            string categoriaAltraPensione = "24";

            bool result = GestioneCrossControls.CI_ControlsAltraPensioneDanteCausa(codiceUCAltraPensione, decorrenzaAltraPensione, dataNascita, dataMorte, cessazioneAltraPensione, categoriaAltraPensione, out messaggioVideo);
        }

        [TestMethod]
        public void TestCI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDiretta()
        {
            DateTime? decorrenzaMaggiorazioneArt6 = DateTime.Now.AddYears(-20);
            DateTime? decorrenzaPensioneDiretta = DateTime.Now;
            DateTime? decorrenzaOriginaria = DateTime.Now.AddYears(-20);

            bool result = GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDecorrenzaOriginaria(decorrenzaMaggiorazioneArt6, decorrenzaPensioneDiretta, decorrenzaOriginaria);
        }

        [TestMethod]
        public void TestCI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria()
        {
            DateTime? decorrenzaMaggiorazioneArt6 = DateTime.Now;
            DateTime? decorrenzaPensioneDiretta = DateTime.Now.AddMonths(2);
            DateTime? dataMorte = DateTime.Now.AddYears(-1);
            DateTime? decorrenzaOriginaria = DateTime.Now;

            bool result = GestioneCrossControls.CI_VerificaDecorrenzaMaggiorazioneArt6WithDecorrenzaPensioneDirettaAndDataMorteAndDecorrenzaOriginaria(decorrenzaMaggiorazioneArt6, decorrenzaPensioneDiretta, dataMorte, decorrenzaOriginaria);
        }

        [TestMethod]
        public void TestCI_VerificaDecorrenzaArt2DPCMWithDanteCausa()
        {
            DateTime? decorrenzaArt2DPCM = DateTime.Now;
            DateTime? decorrenzaPensioneDiretta = new DateTime(1973, 01, 01);
            DateTime? decorrenzaOriginaria = DateTime.Now;

            bool result = GestioneCrossControls.CI_VerificaDecorrenzaArt2DPCMWithDanteCausa(decorrenzaArt2DPCM, decorrenzaPensioneDiretta, decorrenzaOriginaria);
        }

        [TestMethod]
        public void TestCI_VerificaCompatibilitaCategoriaDirettaWithCodNatura()
        {
            string codNaturaTitolare = "4  ";
            string categoria = "IO";
            DateTime? decorrenzaPensioneDiretta = DateTime.Now.AddYears(-50);

            bool result = GestioneCrossControls.CI_VerificaCompatibilitaCategoriaDirettaWithCodNatura(codNaturaTitolare, categoria, decorrenzaPensioneDiretta);
        }

        [TestMethod]
        public void TestCI_ControlsCodiceVirtualeWithCertificatoDiretta()
        {
            char? codiceVirtuale = '4';
            int? certificato = 1;
            byte? convenzione = 17;
            byte? causaCarico = 9;
            string messaggioVideo = string.Empty;

            bool result = GestioneCrossControls.CI_ControlsCodiceVirtualeWithCertificatoDiretta(codiceVirtuale, certificato, convenzione, causaCarico, out messaggioVideo);
        }

        [TestMethod]
        public void TestCI_VerificaSettimaneIncremento1PercentoWithDanteCausa()
        {
            int? nSettimaneIncremento1Percento = 1;
            string categoria = "VO";
            DateTime? decorrenzaDiretta = new DateTime(1994, 01, 01);

            bool result = GestioneCrossControls.CI_VerificaSettimaneIncremento1PercentoWithDanteCausa(nSettimaneIncremento1Percento, categoria, decorrenzaDiretta);
        }

        [TestMethod]
        public void TestCI_VerificaSettimaneIncremento05PercentoWithDecorrenzaDiretta()
        {
            int? nSettimaneIncremento05Percento = 1;
            string categoria = "SO";
            DateTime? decorrenzaDiretta = null;// new DateTime(1994, 01, 01);

            bool result = GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithDecorrenzaDiretta(nSettimaneIncremento05Percento, categoria, decorrenzaDiretta);
        }

        [TestMethod]
        public void TestCI_VerificaSettimaneIncremento05PercentoWithSessoDanteCausa()
        {
            int? nSettimaneIncremento05Percento = 52;
            char? sessoDC = 'M';

            bool result = GestioneCrossControls.CI_VerificaSettimaneIncremento05PercentoWithSessoDanteCausa(nSettimaneIncremento05Percento, sessoDC);
        }

        [TestMethod]
        public void TestCI_VerificaAnniDifferimentoWithDanteCausa()
        {
            int? anniDifferimento = 1;
            string categoriaDiretta = "SOS";
            DateTime? decorrenzaDiretta = new DateTime(1975, 01, 01);

            bool result = GestioneCrossControls.CI_VerificaAnniDifferimentoWithDanteCausa(anniDifferimento, categoriaDiretta, decorrenzaDiretta);
        }

        [TestMethod]
        public void TestVerificaRegex()
        {
            decimal a = 32.3M;
            Assert.IsTrue(Utility.VerificaRegex(a, cifreIntere: 2, cifreDecimali: 1));

            int b = 230;
            Assert.IsTrue(Utility.VerificaRegex(b, cifreIntere: 3));

            decimal c = 33.3M;
            Assert.IsFalse(Utility.VerificaRegex(c, cifreIntere: 1));

            string d = "PIPPO";
            Assert.IsTrue(Utility.VerificaRegex(d, pattern: "[a-z]+|[A-Z]+"));
        }
    }
}
