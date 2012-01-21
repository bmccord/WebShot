//-----------------------------------------------------------------------
// <copyright file="WebShotImageWebControlDesigner.cs" company="Julian Information Technologies, L.L.C.">
//     Copyright Julian Information Technologies, L.L.C. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Julian.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// The WebShotImageWebControlDesigner Class
    /// </summary>
    public class WebShotImageWebControlDesigner : System.Web.UI.Design.ControlDesigner
    {
        /// <summary>
        /// Initializes a new instance of the WebShotImageWebControlDesigner class.
        /// </summary>
        public WebShotImageWebControlDesigner()
        {
        }

        /// <summary>
        /// Displays design time HTML for designer.
        /// </summary>
        /// <returns>Design time HTML</returns>
        public override string GetDesignTimeHtml()
        {
            return this.GetEmptyDesignTimeHtml();
        }

        /// <summary>
        /// Displays design time HTML for designer.
        /// </summary>
        /// <returns>Design time HTML</returns>
        protected override string GetEmptyDesignTimeHtml()
        {
            return
                CreatePlaceHolderDesignTimeHtml(
                    "<div>[Image is set at runtime. Place control inside Table TD or DIV for absolute positioning.]</div>");
        }
    }
}
