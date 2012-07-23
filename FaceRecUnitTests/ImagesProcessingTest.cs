using TestingSilverlightApp.Web.Technical;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace FaceRecUnitTests
{
    
    
    /// <summary>
    ///This is a test class for ImagesProcessingTest and is intended
    ///to contain all ImagesProcessingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImagesProcessingTest
    {


       
        [TestMethod()]
        public void CreateTrainingSetTest()
        {
            string mainDirectory = @"e:\data\photos\";
            int newSize = 80;
            ImagesProcessing.CreateTrainingSet(mainDirectory, newSize,true,0,true);
        }

        [TestMethod]
        public void CreateImageRecognizerTest()
        {
            ImagesProcessing.CreateRecognizerFromFotosInFolder(@"e:\data\photos\", "_100",0.001,500);

        }
    }
}
