//-----------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Julian Information Technologies, L.L.C.">
//     Copyright Julian Information Technologies, L.L.C. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WebShotTestSite
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Web;
    using Julian.Imaging;
    
    /// <summary>
    /// Default Page
    /// </summary>
    public partial class _Default : System.Web.UI.Page
    {
        /// <summary>
        /// Adds a parameter to a querystring
        /// </summary>
        /// <param name="queryString">The existing querystring to add to</param>
        /// <param name="parameterName">The parameter name to add</param>
        /// <param name="textBox">The textbox to get the value from</param>
        /// <returns>Returns the updated querystring</returns>
        public string AddParameter(string queryString, string parameterName, System.Web.UI.WebControls.TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryString = queryString + "&";
                }

                queryString = queryString + parameterName + "=" + textBox.Text;
            }

            return queryString;
        }

        /// <summary>
        /// Adds a parameter to a querystring
        /// </summary>
        /// <param name="queryString">The existing querystring to add to</param>
        /// <param name="parameterName">The parameter name to add</param>
        /// <param name="checkBox">The checkbox that determines if this parameter gets added</param>
        /// <returns>Returns the updated querystring</returns>
        public string AddParameter(string queryString, string parameterName, System.Web.UI.WebControls.CheckBox checkBox)
        {
            if (checkBox.Checked)
            {
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryString = queryString + "&";
                }

                queryString = queryString + parameterName + "=true";
            }

            return queryString;
        }

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The Parameters</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.WebShotImageWebControl1.Bitmap =
                    new Bitmap(Server.MapPath(@"~\images\Notavailable.jpg"));
            }
        }

        /// <summary>
        /// Button Click Event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The parameters</param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            string queryString = string.Empty;

            queryString = this.AddParameter(queryString, "width", this.txtWidth);
            queryString = this.AddParameter(queryString, "height", this.txtHeight);
            queryString = this.AddParameter(queryString, "webshotwidth", this.txtWebShotWidth);
            queryString = this.AddParameter(queryString, "webshotHeight", this.txtWebShotHeight);
            queryString = this.AddParameter(queryString, "autosize", this.chkAutoSize);
            queryString = this.AddParameter(queryString, "autosizewebshot", this.chkAutoSizeWebShot);
            queryString = this.AddParameter(queryString, "keepwebshotproportional", this.chkKeepWebShotProportional);
            queryString = this.AddParameter(queryString, "encodedurl", this.txtUrl);

            this.Image1.ImageUrl = "RenderWebShot.aspx?" + queryString;

            this.WebShotImageWebControl1.Bitmap = WebShot.GetWebShot(this.txtUrl.Text, Convert.ToInt32(this.txtWidth.Text), Convert.ToInt32(this.txtHeight.Text), Convert.ToInt32(this.txtWebShotWidth.Text), Convert.ToInt32(this.txtWebShotHeight.Text), this.chkAutoSize.Checked, this.chkAutoSizeWebShot.Checked, this.chkKeepWebShotProportional.Checked);
        }

        /// <summary>
        /// An event stub to testing posting back
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments sent along with this event call</param>
        protected void Postback_Click(object sender, EventArgs e)
        {
        }
    }
}
