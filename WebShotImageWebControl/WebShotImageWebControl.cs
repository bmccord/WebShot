//-----------------------------------------------------------------------
// <copyright file="WebShotImageWebControl.cs" company="Julian Information Technologies, L.L.C.">
//     Copyright Julian Information Technologies, L.L.C. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Julian.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    #region Public Enums
    /// <summary>
    /// Image types
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// Gif Format
        /// </summary>
        Gif,

        /// <summary>
        /// Jpeg Format
        /// </summary>
        Jpeg
    }

    /// <summary>
    /// Type of persistence to be used
    /// </summary>
    public enum Persistence
    {
        /// <summary>
        /// Cache Persistence
        /// </summary>
        Cache,

        /// <summary>
        /// Session Persistence
        /// </summary>
        Session
    } 
    #endregion

    /// <summary>
    /// The WebShotImageWebControl is a server control that exposes a Bitmap property and does not require a url.
    /// </summary>
    [Designer("Julian.WebControls.WebShotImageWebControlDesigner"),
     ToolboxDataAttribute("<{0}:WebShotImageWebControl Runat=\"server\"></{0}:WebShotImageWebControl>")]
    public class WebShotImageWebControl : Control
    {
        #region Private Fields
        /// <summary>
        /// ImageUrl is used as a placeholder
        /// </summary>
        private string imageUrl;

        /// <summary>
        /// The image type to display
        /// </summary>
        private ImageType imageType;

        /// <summary>
        /// The type of persistence to use
        /// </summary>
        private Persistence persistenceType; 
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the ImageType to be displayed
        /// </summary>
        [Description("Image Type")]
        [Category("Data")]
        [DefaultValue("Gif")]
        [Browsable(true)]
        public ImageType ImageType
        {
            get { return this.imageType; }

            set { this.imageType = value; }
        }

        /// <summary>
        /// Gets or sets the type of persistence to use
        /// </summary>
        [Description("Cache or Session Persistence")]
        [Category("Data")]
        [DefaultValue("Cache")]
        [Browsable(true)]
        public Persistence PersistenceType
        {
            get { return this.persistenceType; }

            set { this.persistenceType = value; }
        }

        /// <summary>
        /// Gets or sets the Bitmap to display
        /// </summary>
        [Browsable(false)]
        public Bitmap Bitmap
        {
            get
            {
                if (this.PersistenceType == Persistence.Session)
                {
                    return (Bitmap)Context.Session[String.Concat(this.CreateUniqueIDString(), "Bitmap")];
                }
                else
                {
                    return (Bitmap)Context.Cache[String.Concat(this.CreateUniqueIDString(), "Bitmap")];
                }
            }

            set
            {
                if (this.PersistenceType == Persistence.Session)
                {
                    Context.Session[String.Concat(this.CreateUniqueIDString(), "Bitmap")] = value;
                }
                else
                {
                    Context.Cache[String.Concat(this.CreateUniqueIDString(), "Bitmap")] = value;
                }
            }
        } 
        #endregion

        #region Protected Methods
        /// <summary>
        /// Initialization Event
        /// </summary>
        /// <param name="e">The event arguments passed to this event</param>
        protected override void OnInit(EventArgs e)
        {
            this.WebShotImageWebControl_Init(e);
        }

        /// <summary>
        /// Render Event
        /// </summary>
        /// <param name="output">The event arguments passed to this event</param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write("<img id={0} src={1}>", this.UniqueID, this.imageUrl);
        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a unique id string to use druing persistence
        /// </summary>
        /// <returns>A unique id string</returns>
        private string CreateUniqueIDString()
        {
            string idStr = String.Empty;
            string tmpId = String.Empty;
            if (this.PersistenceType == Persistence.Session)
            {
                idStr = "__" + Context.Session.SessionID.ToString() + "_";
            }
            else
            {
                if (Context.Cache["idStr"] == null)
                {
                    tmpId = Guid.NewGuid().ToString();
                    Context.Cache["idStr"] = tmpId;
                }

                idStr = "__" + Context.Cache["idStr"].ToString() + "_";
            }

            idStr = String.Concat(idStr, UniqueID);
            idStr = String.Concat(idStr, "_");
            idStr = String.Concat(idStr, Page.ToString());
            idStr = String.Concat(idStr, "_");
            return idStr;
        }

        /// <summary>
        /// Initializes the WebShotImageWebControl
        /// </summary>
        /// <param name="e">The event arguments passed to this event</param>
        private void WebShotImageWebControl_Init(EventArgs e)
        {
            HttpRequest httpRequest = Context.Request;
            HttpResponse httpResponse = Context.Response;
            if (httpRequest.Params[String.Concat("WebShotImageWebControl_", UniqueID)] != null)
            {
                httpResponse.Clear();
                if (this.ImageType == ImageType.Gif)
                {
                    httpResponse.ContentType = "Image/Gif";
                    Bitmap.Save(httpResponse.OutputStream, ImageFormat.Gif);
                }
                else
                {
                    httpResponse.ContentType = "Image/Jpeg";
                    Bitmap.Save(httpResponse.OutputStream, ImageFormat.Jpeg);
                }

                httpResponse.End();
            }

            string str = httpRequest.Url.ToString();
            if (str.IndexOf("?") == -1)
            {
                this.imageUrl = String.Concat(str, "?WebShotImageWebControl_", UniqueID, "=1");
            }
            else
            {
                this.imageUrl = String.Concat(str, "&WebShotImageWebControl_", UniqueID, "=1");
            }
        } 
        #endregion
    }
}