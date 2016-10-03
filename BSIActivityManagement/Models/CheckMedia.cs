using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace BSIActivityManagement.Models
{
    public static class CheckExtension
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public static bool IsVideo(this HttpPostedFileBase postedFile)
        {
            if (postedFile.ContentType.ToLower() != "video/mp4" &&
            postedFile.ContentType.ToLower() != "video/webgm" &&
            postedFile.ContentType.ToLower() != "video/ogg")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".mp4"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".webgm"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".ogv"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".m4v"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".ogg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------

            return true;
        }


        public static bool IsVoice(this HttpPostedFileBase postedFile)
        {

            if (postedFile.ContentType.ToLower() != "audio/mp3" &&
            postedFile.ContentType.ToLower() != "audio/aac" &&
            postedFile.ContentType.ToLower() != "audio/mpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".mp3"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".aac"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".mpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------

            return true;
        }


        public static bool IsDoc(this HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the file mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "application/pdf"
                //postedFile.ContentType.ToLower() != "application/msword" &&
                //postedFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                )
            {
                return false;
            }

            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".pdf"
                //Path.GetExtension(postedFile.FileName).ToLower() != ".doc" &&
                //Path.GetExtension(postedFile.FileName).ToLower() != ".docx"
                )
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
    public static class ImageProcessing
    {
        public static System.Drawing.Image ScaleImage(this System.Drawing.Image image, int maxHeight, int maxWidth)
        {
            var ratio = (double)maxHeight / image.Height;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            if (newWidth > maxWidth)
            {
                 ratio = (double)maxWidth / image.Width;
                 newWidth = (int)(image.Width * ratio);
                 newHeight = (int)(image.Height * ratio);
            }
            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
            using (var g = System.Drawing.Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static System.Drawing.Image cropImage(this System.Drawing.Image img, System.Drawing.Rectangle cropArea)
        {
            System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(img);
            try
            {
                return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }
            catch
            {
                return img;
            }            
        }
    }
}