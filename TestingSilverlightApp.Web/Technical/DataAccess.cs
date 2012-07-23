using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Data;

namespace TestingSilverlightApp.Web.Technical
{
    public class DataAccess
    {
        public bool AddToDB(Image<Gray, byte> faceImg, String label)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);

            try
            {

                conn.Open();
                SqlCommand insertCommand = new SqlCommand("Insert into [pics] (label, pic) Values (@Label, @Pic)", conn);


                var bytes = faceImg.Bytes;


                insertCommand.Parameters.Add("Pic", SqlDbType.Image, 0).Value = bytes;
                insertCommand.Parameters.Add("Label", SqlDbType.VarChar, 0).Value = label;

                int queryResult = insertCommand.ExecuteNonQuery();
                if (queryResult == 1)
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return false;
        }

        public void LoadFromDB(int fixedFaceSize)
        {
            List<Image<Gray, byte>> trainedImages = new List<Image<Gray, byte>>();
            List<String> labels = new List<string>();


            var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
            var selcmd = new SqlCommand("select pic, label from pics", conn);
            try
            {
                conn.Open();
                var rdr = selcmd.ExecuteReader();

                while (rdr.Read())
                {
                    var binary = (byte[])rdr[0];
                    var label = (String)rdr[1];

                    //var ms = new MemoryStream(binary);

                    //var bitmap = new Bitmap(ms);
                    Image<Gray, byte> img = new Image<Gray, byte>(fixedFaceSize, fixedFaceSize);
                    img.Bytes = binary;

                    //check the bitmap from db
                    //img.Save(@"E:\test\bitmapcheck.jpg");


                    //Image<Gray,byte> img = new Image<Gray,byte>(bitmap);
                    trainedImages.Add(img);
                    labels.Add(label);
                }
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}