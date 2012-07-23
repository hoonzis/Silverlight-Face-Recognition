using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing.Imaging;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using TestingSilverlightApp.Web.Technical;


namespace TestingSilverlightApp.Web
{

    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ImageProcessingService
    {
        private static int fixedFaceSize = 80;
        private static String OCTO_DIRECTORY = FileAccessUtil.GetFolder();

        [OperationContract]
        public String RecognizeFromOcto(int[] pixels, int size)
        {
            Logger.WriteMessage("Recognizing OCto");
            var imgFace = ImagesProcessing.DetectAndTrimFace(pixels, size, fixedFaceSize);
            
            if (imgFace == null)
            {
                return "No face detected on the image";
                //imgFace.Save(@"e:\data\phototest\trimed.jpg");
            }
            Logger.WriteMessage("Face detected");
            var equalized = ImagesProcessing.EqualizeHist(imgFace);
            Logger.WriteMessage("Face equalized");
            var folder = FileAccessUtil.GetFolder();

            Logger.WriteMessage("Folder obtained");
            var recognizer = ImagesProcessing.CreateRecognizerFromFotosInFolder(folder, "_" + fixedFaceSize, 0.001, 3000);
            Logger.WriteMessage("Recognizer created");
            String label = recognizer.Recognize(equalized);
            
            if(!String.IsNullOrEmpty(label))
            {
                return label;
            }

            return "Could not recognize";
        }

        [OperationContract]
        public bool AddToOctoSet(int[] pixels, int size, String label)
        {
            
            string[] fileEntries = Directory.GetDirectories(OCTO_DIRECTORY);
            var faceImg = ImagesProcessing.DetectAndTrimFace(pixels, size, fixedFaceSize);
            if (faceImg == null)
            {
                return false;
            }

            faceImg = ImagesProcessing.EqualizeHist(faceImg);
            var directoryNames = fileEntries.Select(x=>x.Substring(x.LastIndexOf("\\") + 1));
            if (!directoryNames.Contains(label))
            {
                Directory.CreateDirectory(OCTO_DIRECTORY + "\\" + label);
            }

            faceImg.Save(OCTO_DIRECTORY + "\\" + label + "\\" + Guid.NewGuid().ToString() + "_" + fixedFaceSize + ".jpg");
            return true;
        }

        [OperationContract]
        public String[] GetTrainedLabels()
        {
            string[] directoryEntries = Directory.GetDirectories(OCTO_DIRECTORY);
            var directoryNames = directoryEntries.Select(x => x.Substring(x.LastIndexOf("\\") + 1));
            return directoryNames.ToArray();
        }
    }
}
