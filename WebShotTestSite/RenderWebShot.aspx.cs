//-----------------------------------------------------------------------
// <copyright file="RenderWebShot.aspx.cs" company="Julian Information Technologies, L.L.C.">
//     Copyright Julian Information Technologies, L.L.C. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WebShotTestSite
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web;
    using Julian.Imaging;

    /// <summary>
    /// RenderWebShot Page
    /// </summary>
    public partial class RenderWebShot : System.Web.UI.Page
    {
        /// <summary>
        /// The Page Load Event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The parameters</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = string.Empty;
            int webShotWidth = 1024;
            int webShotHeight = 768;
            int width = 1024;
            int height = 768;
            bool autoSize = false;
            bool autoSizeWebShot = false;
            bool keepWebShotProportional = false;

            if (Request.QueryString["encodedurl"] != null)
            {
                url = HttpUtility.UrlDecode(Convert.ToString(Request.QueryString["encodedurl"]));
            }

            if (Request.QueryString["webShotWidth"] != null)
            {
                webShotWidth = Convert.ToInt32(Request.QueryString["webShotWidth"]);
            }

            if (Request.QueryString["webShotHeight"] != null)
            {
                webShotHeight = Convert.ToInt32(Request.QueryString["webShotHeight"]);
            }

            if (Request.QueryString["height"] != null)
            {
                height = Convert.ToInt32(Request.QueryString["height"]);
            }

            if (Request.QueryString["width"] != null)
            {
                width = Convert.ToInt32(Request.QueryString["width"]);
            }

            if (Request.QueryString["autoSize"] != null)
            {
                autoSize = Convert.ToBoolean(Request.QueryString["autoSize"]);
            }

            if (Request.QueryString["autoSizeWebShot"] != null)
            {
                autoSizeWebShot = Convert.ToBoolean(Request.QueryString["autoSizeWebShot"]);
            }

            if (Request.QueryString["keepWebShotProportional"] != null)
            {
                keepWebShotProportional = Convert.ToBoolean(Request.QueryString["keepWebShotProportional"]);
            }

            if (url == string.Empty)
            {
                return;
            }

            MemoryStream theStream = new MemoryStream();
            ////WebShot.SaveWebShot(@"c:\test.bmp", 100, url, WebShot.ImageFormat.Bmp);
            WebShot webShot = new WebShot();
            webShot.Url = url;
            webShot.Height = height;
            webShot.Width = width;
            webShot.WebShotHeight = webShotHeight;
            webShot.WebShotWidth = webShotWidth;
            webShot.AutoSize = autoSize;
            webShot.AutoSizeWebShot = autoSizeWebShot;
            webShot.KeepWebShotProportional = keepWebShotProportional;
            ////webShot.Timeout = 1;

            WebShot.WebShotStatus status = webShot.CaptureWebShotImage();
            
            Bitmap bitmap = new Bitmap(Server.MapPath(@"~\images\notavailable.jpg"));
            if (status == WebShot.WebShotStatus.Captured)
            {
                bitmap = webShot.Image;
            }

            if (bitmap != null)
            {
                bitmap.Save(theStream, ImageFormat.Jpeg);
                Response.ContentType = "image/jpeg";
                Response.BinaryWrite(theStream.ToArray());
            }

            theStream.Dispose();
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            webShot.Dispose();
        }
    }
}
