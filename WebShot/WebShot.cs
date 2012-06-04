//-----------------------------------------------------------------------
// <copyright file="WebShot.cs" company="Julian Information Technologies, L.L.C.">
//     Copyright Julian Information Technologies, L.L.C. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Julian.Imaging
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Enables the user to capture Web Site images.
	/// Testing git again
    /// </summary>
    public class WebShot : IDisposable
    {
        #region Private Members

        /// <summary>
        /// Holds the status of the Manual Reset Event.  This is used to stop the thread when necessary.
        /// </summary>
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// The URL to capture.
        /// </summary>
        private string url;

        /// <summary>
        /// Holds the capture image.
        /// </summary>
        private Bitmap image;

        /// <summary>
        /// The timeout value.
        /// </summary>
        private int timeout = 60; // Default

        /// <summary>
        /// The width of the captured image.
        /// </summary>
        private int webShotWidth = 1024;

        /// <summary>
        /// The height of the captured image.
        /// </summary>
        private int webShotHeight = 768;

        /// <summary>
        /// The width of the hidden browser window.
        /// </summary>
        private int width = 1024;

        /// <summary>
        /// The height of the hidden browser window.
        /// </summary>
        private int height = 768;

        /// <summary>
        /// Track the status of disposal
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The result of the most recent capture attempt.
        /// </summary>
        private WebShotStatus webShotResult = WebShotStatus.Uninitialized;

        /// <summary>
        /// Should the hidden browser window be autosized based on the size of the rendered web site?
        /// </summary>
        private bool autoSize;

        /// <summary>
        /// Should the capture image be autosized based on the size of the rendered web site?
        /// </summary>
        private bool autoSizeWebShot;

        /// <summary>
        /// Should the image height be reset in proportion to the size of the original web site?
        /// </summary>
        private bool keepWebShotProportional;

        /// <summary>
        /// The quality to save the image as.
        /// </summary>
        private int webShotImageQuality = 100;

        /// <summary>
        /// The image format to use when saving the image.
        /// </summary>
        private ImageFormat webShotImageFormat = ImageFormat.Jpeg;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WebShot class with default settings.
        /// </summary>
        public WebShot()
            : this(string.Empty, 1024, 768, 1024, 768, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebShot class with desired settings and set to autosize.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        public WebShot(string url)
            : this(url, 1024, 768, 1024, 768, true, true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebShot class with desired settings.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="webShotWidth">The width of the captured WebShot.  Default is 1024.</param>
        /// <param name="webShotHeight">The height of the captured WebShot.  Default is 768.</param>
        public WebShot(string url, int webShotWidth, int webShotHeight)
            : this(url, 1024, 768, webShotWidth, webShotHeight, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebShot class with desired parameters.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="width">The width of the hidden browser window.  Affects HTML rendering.  Default is 1024.</param>
        /// <param name="height">The height of the hidden browser window.  Affects HTML rendering.  Default is 768.</param>
        /// <param name="webShotWidth">The width of the captured WebShot.  Default is 1024.</param>
        /// <param name="webShotHeight">The height of the captured WebShot.  Default is 768.</param>
        public WebShot(string url, int width, int height, int webShotWidth, int webShotHeight)
            : this(url, width, height, webShotWidth, webShotHeight, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebShot class with desired parameters.B
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="width">The width of the hidden browser window.  Affects HTML rendering.  Default is 1024.</param>
        /// <param name="height">The height of the hidden browser window.  Affects HTML rendering.  Default is 768.</param>
        /// <param name="webShotWidth">The width of the captured WebShot.  Default is 1024.</param>
        /// <param name="webShotHeight">The height of the captured WebShot.  Default is 768.</param>
        /// <param name="autoSize">Autosizes the hidden browser window.  Overrides width and height.  Affects HTML Rendering.</param>
        /// <param name="autoSizeWebShot">Autosizes the WebShot image.  Overrides WebShotHeight, WebShotWidth and KeepWebShotProportional.</param>
        /// <param name="keepWebShotProportional">The WebShotHeight is kept in proportion with the WebShotWidth according to the hidden browser Width and Height proportion.</param>
        public WebShot(string url, int width, int height, int webShotWidth, int webShotHeight, bool autoSize, bool autoSizeWebShot, bool keepWebShotProportional)
        {
            this.url = url;
            this.width = width;
            this.height = height;
            this.webShotHeight = webShotHeight;
            this.webShotWidth = webShotWidth;
            this.autoSize = autoSize;
            this.autoSizeWebShot = autoSizeWebShot;
            this.keepWebShotProportional = keepWebShotProportional;
        }

        #endregion

        #region Destructors
        /// <summary>
        /// Finalizes an instance of the WebShot class.
        /// </summary>
        ~WebShot()
        {
            this.Dispose(false);
        }
        #endregion

        #region Public Enums

        /// <summary>
        /// Specifies the status of the WebShot image capture attempt.
        /// </summary>
        public enum WebShotStatus
        {
            /// <summary>
            /// The WebShot image was captured.
            /// </summary>
            Captured,

            /// <summary>
            /// The WebShot capture attempt failed.
            /// </summary>
            Failed,

            /// <summary>
            /// The WebShot capture attempt timed out.
            /// </summary>
            Timeout,

            /// <summary>
            /// No WebShot capture has been attempted.
            /// </summary>
            Uninitialized
        }

        /// <summary>
        /// Format of WebShot Image
        /// </summary>
        public enum ImageFormat
        {
            /// <summary>
            /// Bitmap Format
            /// </summary>
            Bmp,

            /// <summary>
            /// Jpeg Format
            /// </summary>
            Jpeg,

            /// <summary>
            /// Gif Format
            /// </summary>
            Gif,

            /// <summary>
            /// Tiff Format
            /// </summary>
            Tiff,

            /// <summary>
            /// Png Bitmap
            /// </summary>
            Png
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the captured WebShot image.
        /// </summary>
        public Bitmap Image
        {
            get { return this.image; }
        }

        /// <summary>
        /// Gets or sets the timeout value.  Default is 60 seconds.
        /// </summary>
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        /// <summary>
        /// Gets or sets the URL to capture.
        /// </summary>
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        /// <summary>
        /// Gets or sets the width of the captured Webshot image.  Default is 1024.
        /// </summary>
        public int WebShotWidth
        {
            get { return this.webShotWidth; }
            set { this.webShotWidth = value; }
        }

        /// <summary>
        /// Gets or sets the height of the captured Webshot image.  Default is 768.
        /// </summary>
        public int WebShotHeight
        {
            get { return this.webShotHeight; }
            set { this.webShotHeight = value; }
        }

        /// <summary>
        /// Gets or sets the width of the hidden browser window.  Affects HTML rendering.  Default is 1024.
        /// </summary>
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the height of the hidden browser window.  Affects HTML rendering.  Default is 768.
        /// </summary>
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets the result from the latest WebShot capture attempt.
        /// </summary>
        public WebShotStatus WebShotResult
        {
            get { return this.webShotResult; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Height and Width is set to match the rendered HTML.  This overrides Width and Height.  Default is false.
        /// </summary>
        public bool AutoSize
        {
            get { return this.autoSize; }
            set { this.autoSize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether WebShotHeight and WebShotWidth is set to match the rendered HTML.  This overrides WebShotWidth, WebShotHeight, and KeepWebShotProportional.  The default is false.
        /// </summary>
        public bool AutoSizeWebShot
        {
            get { return this.autoSizeWebShot; }
            set { this.autoSizeWebShot = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WebShotHeight is kept in proportion with the WebShotWidth according to the hidden browser Width and Height proportion.
        /// </summary>
        public bool KeepWebShotProportional
        {
            get { return this.keepWebShotProportional; }
            set { this.keepWebShotProportional = value; }
        }

        /// <summary>
        /// Gets or sets the quality for the WebShot Image. Values: 0 to 100.  Default is 100.
        /// </summary>
        public int WebShotImageQuality
        {
            get { return this.webShotImageQuality; }
            set { this.webShotImageQuality = value; }
        }

        /// <summary>
        /// Gets or sets the format to save the image as.  Formats: Jpg, Gif, Bmp, Tif, Png
        /// </summary>
        public ImageFormat WebShotImageFormat
        {
            get { return this.webShotImageFormat; }
            set { this.webShotImageFormat = value; }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a WebShot image for the given URL using autosizing.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <returns>Returns the captured WebShot image as a System.Drawing.Bitmap.</returns>
        public static Bitmap GetWebShot(string url)
        {
            return GetWebShot(url, 1024, 768, 1024, 768, true, true, false);
        }

        /// <summary>
        /// Returns a WebShot image of the provided size for the given URL.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="webShotWidth">The width of the returned WebShot Image.</param>
        /// <param name="webShotHeight">The height of the returned WebShot Image.</param>
        /// <returns>Returns the captured WebShot image as a System.Drawing.Bitmap.</returns>
        public static Bitmap GetWebShot(string url, int webShotWidth, int webShotHeight)
        {
            return GetWebShot(url, 1024, 768, webShotWidth, webShotHeight);
        }

        /// <summary>
        /// Returns a WebShot image for the given URL using full provided sizing.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="width">The width of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="height">The height of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="webShotWidth">The width of the returned WebShot Image.</param>
        /// <param name="webShotHeight">The height of the returned WebShot Image.</param>
        /// <returns>Returns the captured WebShot image as a System.Drawing.Bitmap.</returns>
        public static Bitmap GetWebShot(string url, int width, int height, int webShotWidth, int webShotHeight)
        {
            return GetWebShot(url, width, height, webShotWidth, webShotHeight, false, false, false);
        }

        /// <summary>
        /// Returns a WebShot image for the given URL using full provided sizing.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="width">The width of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="height">The height of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="webShotWidth">The width of the returned WebShot Image.</param>
        /// <param name="webShotHeight">The height of the returned WebShot Image.</param>
        /// <param name="autoSize">Autosizes the hidden browser window.  Overrides width and height.  Affects HTML Rendering.</param>
        /// <param name="autoSizeWebShot">Autosizes the WebShot image.  Overrides WebShotHeight and WebShotWidth.</param>
        /// <param name="keepWebShotProportional">The WebShotHeight is kept in proportion with the WebShotWidth according to the hidden browser Width and Height proportion.</param>
        /// <returns>Returns the captured WebShot image as a System.Drawing.Bitmap.</returns>
        public static Bitmap GetWebShot(string url, int width, int height, int webShotWidth, int webShotHeight, bool autoSize, bool autoSizeWebShot, bool keepWebShotProportional)
        {
            WebShot webShot = new WebShot(url, width, height, webShotWidth, webShotHeight, autoSize, autoSizeWebShot, keepWebShotProportional);
            Bitmap bitmap = webShot.GetWebShotImage();
            if (bitmap == null)
            {
                bitmap = (Bitmap)System.Drawing.Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Julian.Imaging.Notavailable.jpg"));
            }

            return bitmap;
        }

        /// <summary>
        /// Saves a WebShot image for the given URL using autosizing.
        /// </summary>
        /// <param name="fileName">The full path and file name to save to.  If the file exists, it will be overwritten.</param>
        /// <param name="webShotQuality">Quality for WebShot Image. Values: 0 to 100.</param>
        /// <param name="url">The URL to capture.</param>
        /// <param name="webShotImageFormat">The format to save the image as.</param>
        /// <returns>Returns one of the values from WebShotStatus.</returns>
        public static WebShotStatus SaveWebShot(string fileName, int webShotQuality, string url, ImageFormat webShotImageFormat)
        {
            return SaveWebShot(fileName, webShotQuality, url, webShotImageFormat, 1024, 768, 1024, 768, true, true, false);
        }

        /// <summary>
        /// Saves a WebShot image for the given URL using full provided sizing.
        /// </summary>
        /// <param name="fileName">The full path and file name to save to.  If the file exists, it will be overwritten.</param>
        /// <param name="webShotQuality">Quality for WebShot Image. Values: 0 to 100.</param>
        /// <param name="url">The URL to capture.</param>
        /// <param name="webShotImageFormat">The format to save the image as.</param>
        /// <param name="width">The width of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="height">The height of the hidden browser window.  Affects HTML Rendering.</param>
        /// <param name="webShotWidth">The width of the returned WebShot Image.</param>
        /// <param name="webShotHeight">The height of the returned WebShot Image.</param>
        /// <param name="autoSize">Autosizes the hidden browser window.  Overrides width and height.  Affects HTML Rendering.</param>
        /// <param name="autoSizeWebShot">Autosizes the WebShot image.  Overrides WebShotHeight and WebShotWidth.</param>
        /// <param name="keepWebShotProportional">The WebShotHeight is kept in proportion with the WebShotWidth according to the hidden browser Width and Height proportion.</param>
        /// <returns>Returns one of the values from WebShotStatus.</returns>
        public static WebShotStatus SaveWebShot(string fileName, int webShotQuality, string url, ImageFormat webShotImageFormat, int width, int height, int webShotWidth, int webShotHeight, bool autoSize, bool autoSizeWebShot, bool keepWebShotProportional)
        {
            WebShot webShot = new WebShot(url, width, height, webShotWidth, webShotHeight, autoSize, autoSizeWebShot, keepWebShotProportional);
            webShot.WebShotImageQuality = webShotQuality;
            webShot.WebShotImageFormat = webShotImageFormat;
            return webShot.SaveWebShot(fileName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Captures a WebShot image.
        /// </summary>
        /// <returns>Returns one of the values from WebShotStatus.</returns>
        public WebShotStatus CaptureWebShotImage()
        {
            this.webShotResult = WebShotStatus.Failed;
            Thread t = new Thread(this.RenderWebShotImage);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            this.manualResetEvent.WaitOne();
            t.Abort();

            return this.webShotResult;
        }

        /// <summary>
        /// Returns a WebShot image generated by the current instance.
        /// </summary>
        /// <returns>Returns the captured WebShot image as a System.Drawing.Bitmap.</returns>
        public Bitmap GetWebShotImage()
        {
            this.CaptureWebShotImage();
            return this.image;
        }

        /// <summary>
        /// Save a WebShot image generated by the current instance.
        /// </summary>
        /// <param name="fileName">The full path and file name to save to.  If the file exists, it will be overwritten.</param>
        /// <returns>Returns one of the values from WebShotStatus.</returns>
        public WebShotStatus SaveWebShot(string fileName)
        {
            Bitmap bitmap = this.GetWebShotImage();

            if (bitmap != null && this.webShotResult == WebShotStatus.Captured)
            {
                try
                {
                    ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders()[this.webShotImageFormat.GetHashCode()];
                    EncoderParameters parms = new EncoderParameters(1);

                    parms.Param[0] = new EncoderParameter(Encoder.Quality, this.webShotImageQuality);

                    bitmap.Save(fileName, encoder, parms);
                }
                catch (Exception)
                {
                    this.webShotResult = WebShotStatus.Failed;
                    throw;
                }
            }

            return this.webShotResult;
        }

        /// <summary>
        /// Releases all resources used by this Julian.Imaging.WebShot.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// This event fires when a new window is created in the hidden browser and cancels the new window.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The parameters sent along with this event.</param>
        private void WebBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// This event fires when the document is completely rendered.  It then takes a picture, formats it
        /// and stops the thread thereby returning control to the caller.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The parameters sent along with this event.</param>
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser webBrowser = (WebBrowser)sender;
            int tempHeight = this.Height;
            int tempWidth = this.Width;
            int tempWebShotHeight = this.webShotHeight;
            int tempWebShotWidth = this.webShotWidth;

            try
            {
                if (this.autoSize && webBrowser.Document != null && webBrowser.Document.Body != null)
                {
                    tempHeight = webBrowser.Document.Body.ScrollRectangle.Height;
                    tempWidth = webBrowser.Document.Body.ScrollRectangle.Width;
                }

                webBrowser.ClientSize = new Size(tempWidth, tempHeight);
                webBrowser.ScrollBarsEnabled = false;
                this.image = new Bitmap(webBrowser.Bounds.Width, webBrowser.Bounds.Height);
                webBrowser.BringToFront();
                webBrowser.DrawToBitmap(this.image, webBrowser.Bounds);

                if (this.autoSizeWebShot && webBrowser.Document != null && webBrowser.Document.Body != null)
                {
                    tempWebShotHeight = webBrowser.Document.Body.ScrollRectangle.Height;
                    tempWebShotWidth = webBrowser.Document.Body.ScrollRectangle.Width;
                }

                if (this.keepWebShotProportional && !this.autoSizeWebShot)
                {
                    double proportionalHeight = (Convert.ToDouble(this.webShotWidth) / Convert.ToDouble(tempWidth)) * Convert.ToDouble(tempHeight);
                    tempWebShotHeight = Convert.ToInt32(Math.Round(proportionalHeight, 0));
                }

                Image thumbnailImage = this.image.GetThumbnailImage(tempWebShotWidth, tempWebShotHeight, null, IntPtr.Zero);

                this.image = (Bitmap)thumbnailImage;
            }
            catch (Exception)
            {
                // Because of the threading model, we can't accurately capture errors.  They happen all the time, so I assumed failure
                // from the beginning and only set success flags if everything is ok at the end.
            }
            finally
            {
                if (this.manualResetEvent != null)
                {
                    this.manualResetEvent.Set();
                }

                this.webShotResult = this.image != null ? WebShotStatus.Captured : WebShotStatus.Failed;
                webBrowser.Dispose();
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// This method is used inside a thread.  It starts a hidden browser window, navigates to the provided URL, and 
        /// waits for the browser DocumentCompleted event to fire before ending the thread.  This method is responsible for 
        /// timing out if the timeout is reached.
        /// </summary>
        private void RenderWebShotImage()
        {
            try
            {
                WebBrowser webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.Visible = true;
                if (this.autoSize)
                {
                    webBrowser.Size = new Size(0, 0);
                }
                else
                {
                    webBrowser.Size = new Size(this.Width, this.Height);
                }

                webBrowser.ScriptErrorsSuppressed = true;

                DateTime time = DateTime.Now;

                webBrowser.DocumentCompleted += this.WebBrowser_DocumentCompleted;
                webBrowser.NewWindow += this.WebBrowser_NewWindow;

                webBrowser.Navigate(this.Url);

                while (true)
                {
                    Thread.Sleep(0);
                    TimeSpan elapsedTime = DateTime.Now - time;

                    if (elapsedTime.TotalSeconds >= this.timeout)
                    {
                        this.webShotResult = WebShotStatus.Timeout;
                        this.manualResetEvent.Set();
                    }

                    Application.DoEvents();
                }
            }
            catch (Exception)
            {
                // Because of the threading model, we can't accurately capture errors.  They happen all the time, so I assumed failure
                // from the beginning and only set success flags if everything is ok at the end.
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by this Julian.Imaging.WebShot.
        /// </summary>
        /// <param name="disposing">Was this item called by the Dispose Method?</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.image != null)
                    {
                        this.image.Dispose();
                    }

                    if (this.manualResetEvent != null)
                    {
                        this.manualResetEvent.Close();
                    }
                }
            }

            this.disposed = true;
        }
        #endregion
    }
}