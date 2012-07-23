using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestingSilverlightApp.Web.Technical
{
    public static class ImagesProcessing
    {
        /// <summary>
        /// Run over directory, get images, detech faces, resize and storeagain
        /// </summary>
        /// <param name="directory"></param>
        public static void CreateTrainingSet(String mainDirectory, int newSize, bool equalize, int rotation, bool flip)
        {
            string[] subdirEntries = Directory.GetDirectories(mainDirectory);
            foreach (var directory in subdirEntries)
            {
                string[] fileEntries = Directory.GetFiles(directory);
                foreach (var file in fileEntries.Where(x=>(!x.Contains("_"))))
                {
                    Image<Gray, byte> image = new Image<Gray, byte>(file);
                   

                    //the images are big - reduce the size to the half
                    image = image.Resize(0.5, INTER.CV_INTER_CUBIC);
                    
                    var haar = new HaarCascade(FileAccessUtil.GetHaarCascade());
                    var faces = haar.Detect(image);

                    if (faces.Count() == 1)
                    {
                        var face = faces[0];

                        //resize all images to 100
                        var faceImg = image.Copy(face.rect).Resize(newSize, newSize, INTER.CV_INTER_CUBIC);
                        String imgName = file.Insert(file.IndexOf("."), "_" + newSize.ToString());
                        if (equalize)
                        {
                            imgName = imgName.Insert(file.IndexOf("."), "_N");
                            var equalized = EqualizeHist(faceImg);
                            faceImg = equalized;

                        }

                        faceImg.Save(imgName);

                        //create rotated image if it was demanded
                        if (rotation != 0)
                        {
                            var rotated = faceImg.Rotate(rotation, new Gray(0.3));
                            var rotatedName = imgName.Insert(file.IndexOf("."), "_R");
                            rotated.Save(rotatedName);
                        }

                        if (flip)
                        {
                            var fliped = faceImg.Flip(FLIP.HORIZONTAL);
                            var flipedName = imgName.Insert(file.IndexOf("."), "_F");
                            fliped.Save(flipedName);
                        }
                    }
                }
            }
        }

        

        public static Image<Gray, byte> EqualizeHist(Image<Gray,byte> input)
        {
            Image<Gray, byte> output = new Image<Gray,byte>(input.Width, input.Height);

            CvInvoke.cvEqualizeHist(input.Ptr, output.Ptr);

            return output;
        }

        public static EigenObjectRecognizer CreateRecognizerFromFotosInFolder(String folder,  String pattern, double accuracy, int eigenDistanceThreshold)
        {

            List<Image<Gray, byte>> trainedImages = new List<Image<Gray, byte>>();
            List<String> labels = new List<string>();

             string[] subdirEntries = Directory.GetDirectories(folder);
             foreach (var directory in subdirEntries)
             {
                 string[] fileEntries = Directory.GetFiles(directory);
                 var label = directory.Remove(0, directory.LastIndexOf("\\")+1);
                 foreach (var file in fileEntries.Where(x=>x.Contains(pattern)))
                 {
                     Image<Gray, byte> image = new Image<Gray, byte>(file);
                     trainedImages.Add(image);
                     labels.Add(label);
                 }
             }

             MCvTermCriteria termCrit = new MCvTermCriteria(40, accuracy);


             EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                 trainedImages.ToArray(),
                 labels.ToArray(),
                 eigenDistanceThreshold,
                 ref termCrit);

            
             //int i = 1;



             //Image<Gray, float>[] eigenfaces;
             //Image<Gray, float> avg;
             //EigenObjectRecognizer.CalcEigenObjects(trainedImages.ToArray(),ref termCrit,out eigenfaces, out avg);
             
            

            //foreach(var eigenface in eigenfaces)
            // {
            //     eigenface.Bitmap.Save(@"e:\data\phototest\eigen" + i + ".bmp");
            //     i++;
            // }
             return recognizer;
        }

        public static Image<Gray, byte> DetectAndTrimFace(int[] pixels, int initialSize, int outputSize)
        {
            var inBitmap = ConvertToBitmap(pixels, initialSize);
            //inBitmap.Save(@"E:\data\phototest\received.bmp");
            var grayframe = new Image<Gray, byte>(inBitmap);
            
            var haar = new HaarCascade(FileAccessUtil.GetHaarCascade());
            var faces = haar.Detect(grayframe,
                1.2,
                3,
                HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(30,30));
            

            if (faces.Count() != 1)
            {
                return null;
            }
            var face = faces[0];

            var returnImage = grayframe.Copy(face.rect).Resize(outputSize, outputSize, INTER.CV_INTER_CUBIC);

            return returnImage;
        }

        private static byte[] ConvertBitmapToByteArray(Bitmap imageToConvert, ImageFormat formatOfImage)
        {
            byte[] Ret;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, formatOfImage);
                    Ret = ms.ToArray();
                }
            }
            catch (Exception) { throw; }
            return Ret;
        }

        private static Image<Gray, byte> ConvertToImage(byte[] pixels, int size)
        {
            MemoryStream stream = new MemoryStream(pixels, 0, pixels.Length);
            Bitmap bitmap = new Bitmap(stream);
            Image<Gray, byte> image = new Image<Gray, byte>(size, size);
            image.Bytes = pixels;

            return image;
        }

        private static Bitmap ConvertToBitmap(int[] pixels, int size)
        {
            Bitmap bitmap = new Bitmap(size, size);

            for (int i = 0; i < pixels.Length; i++)
            {
                int y = i / size;
                int x = i % size;
                bitmap.SetPixel(x, y, Color.FromArgb(pixels[i]));

            }

            //bitmap.Save(@"e:\test\bitmap.jpg");
            return bitmap;
        }
    }
}