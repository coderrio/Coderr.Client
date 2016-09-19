using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.WinForms.ContextProviders;

namespace OneTrueError.Client.WinForms
{
    /// <summary>
    ///     Used to capture a screenshot from forms
    /// </summary>
    public class FormScreenshooter
    {
        static FormScreenshooter()
        {
            PixelFormat = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        ///     Decides the quality of the image (like amount of colors)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Default is <c>PixelFormat.Format8bppIndexed</c>
        ///     </para>
        /// </remarks>
        public static PixelFormat PixelFormat { get; set; }

        /// <summary>
        ///     Capture a single form
        /// </summary>
        /// <param name="form"></param>
        /// <param name="destination">Will be saved as JPEG.</param>
        public void Capture(Form form, Stream destination)
        {
            using (var g = form.CreateGraphics())
            {
                using (var bmp = new Bitmap(form.Width, form.Height, PixelFormat))
                {
                    form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
                    bmp.Save(destination, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        ///     Take a screenshot only of the active form
        /// </summary>
        /// <returns>collection where the screenshot it base64 encoded using the name of the form as property name</returns>
        public ContextCollectionDTO CaptureActiveForm()
        {
            var screenshots = new Dictionary<string, string>();

            var ms = new MemoryStream();

            var form = Form.ActiveForm ??
                       (Application.OpenForms.Count > 0
                           ? Application.OpenForms[Application.OpenForms.Count - 1]
                           : null);
            if (form == null)
                return null;

            Capture(form, ms);
            var str = Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
            var name = GetFormName(screenshots, form);
            screenshots.Add(name, str);
            return new ContextCollectionDTO(ScreenshotProvider.NAME, screenshots);
        }

        /// <summary>
        ///     Capture all forms.
        /// </summary>
        /// <returns>A collection where all screen shots are BASE64 encoded.</returns>
        public ContextCollectionDTO CaptureAllOpenForms()
        {
            var screenshots = new Dictionary<string, string>();

            var ms = new MemoryStream();

            if (Form.ActiveForm != null)
            {
                Capture(Form.ActiveForm, ms);
                var str = Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
                var name = GetFormName(screenshots, Form.ActiveForm);
                screenshots.Add(name, str);
            }
            
            foreach (Form form in Application.OpenForms)
            {
                if (form == Form.ActiveForm)
                    continue;

                ms.Position = 0;
                ms.SetLength(0);
                Capture(form, ms);
                var str = Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
                var name = GetFormName(screenshots, form);
                screenshots.Add(name, str);
            }

            return new ContextCollectionDTO(ScreenshotProvider.NAME, screenshots);
        }
        
        private static string GetFormName(IDictionary<string,string> screenshots, Form form)		
       {		
            var name = form.Name;		
            if (string.IsNullOrEmpty(name))		
                name = string.IsNullOrEmpty(form.Text) ? "Noname": form.Text;

             if (screenshots.ContainsKey(name))
                name = string.Format("{0}{1}",name,Guid.NewGuid().ToString());

            return name;		
        }
    }
}
