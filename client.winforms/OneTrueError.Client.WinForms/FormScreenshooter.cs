using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OneTrueError.Reporting.Contracts;
using OneTrueError.Reporting.WinForms.ContextProviders;

namespace OneTrueError.Reporting.WinForms
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
        ///     Capture all forms.
        /// </summary>
        /// <returns>A collection where all screenshots are BASE64 encoded.</returns>
        public ContextInfoDTO CaptureAllOpenForms()
        {
            var screenshots = new Dictionary<string, string>();

            var ms = new MemoryStream();

            if (Form.ActiveForm != null)
            {
                Capture(Form.ActiveForm, ms);
                var str = Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
                var name = GetFormName(Form.ActiveForm);
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
                var name = GetFormName(form);
                screenshots.Add(name, str);
            }

            return new ContextInfoDTO(ScreenshotProvider.NAME, screenshots);
        }

        private static string GetFormName(Form form)
        {
            var name = form.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = string.IsNullOrEmpty(form.Text)
                    ? "Noname"
                    : form.Text;
            }
            return name;
        }
    }
}