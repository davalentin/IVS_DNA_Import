using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace INPS.Pensioni.Liquidazione.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for GestioneDatiContributiviCiTest and is intended
    ///to contain all GestioneDatiContributiviCiTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DatiContributiviCiTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion Additional test attributes

        [TestMethod]
        public void TestGetStoreDeleteDatiGenericiPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
            datiGenerici.NSettFittiziePrepensionamento = 10;
            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiGenerici);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiRitorno = null;
            //test un' eliminazione presente per numero domanda presente
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(idPensione, out datiGenericiRitorno);
            Assert.IsTrue(datiGenericiRitorno != null && datiGenericiRitorno.NSettFittiziePrepensionamento == datiGenerici.NSettFittiziePrepensionamento, "Errore nel recupero dei dati generici");

            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(idPensione);
        }

        //[TestMethod]
        //public void TestGetStoreDeletePrestazioniEEImportiEsteri()
        //{
        //    long idPensione = 0;
        //    GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, out idPensione);

        //    GestioneDatiContributiviCi.EliminaAllImportiEsteri(idPensione);
        //    GestioneDatiContributiviCi.EliminaAllPrestazioniEE(idPensione);

        //    List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
        //    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
        //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(2038517500007, out listaPrestazioniEE);
        //    Assert.IsTrue(listaPrestazioniEE == null || listaPrestazioniEE.Count == 0,"Controllo 1:PrestazioniEE non corrette");
        //    GestioneDatiContributiviCi.GetImportiEsteriByNumeroDomanda(2038517500007, out listaImportiEsteri);
        //    Assert.IsTrue(listaImportiEsteri == null || listaImportiEsteri.Count == 0, "Controllo 1:ImportiEsteri non corretti");
        //    listaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
        //    listaImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
        //    GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE = new GestioneDatiContributiviCi.PensioniCiPrestazioniEE();
        //    prestazioneEE.CodiceStatoEE = "01";
        //    prestazioneEE.CodiceIstituzione = "0001";
        //    listaPrestazioniEE.Add(prestazioneEE);
        //    prestazioneEE = new GestioneDatiContributiviCi.PensioniCiPrestazioniEE();
        //    prestazioneEE.CodiceStatoEE = "21";
        //    prestazioneEE.CodiceIstituzione = "0002";
        //    listaPrestazioniEE.Add(prestazioneEE);
        //    GestioneDatiContributiviCi.SalvaPrestazioniEE(idPensione, listaPrestazioniEE);
        //    listaPrestazioniEE = null;
        //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(2038517500007, out listaPrestazioniEE);
        //    Assert.IsTrue(listaPrestazioniEE != null && listaPrestazioniEE.Count == 2, "Controllo 2:PrestazioniEE non corrette");
        //    GestioneDatiContributiviCi.PensioniCiImportiEsteri importoEstero = new GestioneDatiContributiviCi.PensioniCiImportiEsteri();
        //    importoEstero.IDPrestazioneEE = listaPrestazioniEE[0].Id;
        //    importoEstero.ImportoPrestazioneEE = 100;
        //    listaImportiEsteri.Add(importoEstero);
        //    importoEstero = new GestioneDatiContributiviCi.PensioniCiImportiEsteri();
        //    importoEstero.IDPrestazioneEE = listaPrestazioniEE[1].Id;
        //    importoEstero.ImportoPrestazioneEE = 200;
        //    listaImportiEsteri.Add(importoEstero);
        //    importoEstero = new GestioneDatiContributiviCi.PensioniCiImportiEsteri();
        //    importoEstero.IDPrestazioneEE = listaPrestazioniEE[1].Id;
        //    importoEstero.ImportoPrestazioneEE = 300;
        //    listaImportiEsteri.Add(importoEstero);
        //    GestioneDatiContributiviCi.SalvaImportiEsteri(idPensione, listaImportiEsteri);
        //    listaImportiEsteri = null;
        //    GestioneDatiContributiviCi.GetImportiEsteriByNumeroDomanda(2038517500007, out listaImportiEsteri);
        //    Assert.IsTrue(listaImportiEsteri != null && listaImportiEsteri.Count == 3, "Controllo 2:ImportiEsteri non corretti");
        //    GestioneDatiContributiviCi.EliminaImportiEsteri(listaImportiEsteri[0].Id);
        //    GestioneDatiContributiviCi.EliminaPrestazioniEE(listaPrestazioniEE[0].Id);
        //    GestioneDatiContributiviCi.GetPrestazioniEEByNumeroDomanda(2038517500007, out listaPrestazioniEE);
        //    Assert.IsTrue(listaPrestazioniEE != null && listaPrestazioniEE.Count == 1, "Controllo 3:PrestazioniEE non corrette");
        //    GestioneDatiContributiviCi.GetImportiEsteriByNumeroDomanda(2038517500007, out listaImportiEsteri);
        //    Assert.IsTrue(listaImportiEsteri != null && listaImportiEsteri.Count == 2, "Controllo 3:ImportiEsteri non corretti");
        //    GestioneDatiContributiviCi.EliminaAllImportiEsteri(idPensione);
        //    GestioneDatiContributiviCi.EliminaAllPrestazioniEE(idPensione);
        //}
    }
}
