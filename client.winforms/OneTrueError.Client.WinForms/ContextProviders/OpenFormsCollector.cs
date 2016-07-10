using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.WinForms.ContextProviders
{
    /// <summary>
    ///     Serializes all open forms into the context collection named <c>"OpenForms"</c>
    /// </summary>
    public class OpenFormsCollector : IContextInfoProvider
    {
        /// <summary>
        ///     Returns <c>OpenForms</c>.
        /// </summary>
        public string Name
        {
            get { return "OpenForms"; }
        }

        /// <summary>
        ///     Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>
        ///     Collection. Items with multiple values are joined using <c>";;"</c>
        /// </returns>
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            var values = new Dictionary<string, string>();

            var invocationRequired = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form.InvokeRequired)
                    invocationRequired = true;
            }
            if (invocationRequired || !Application.MessageLoop)
                return new ContextCollectionDTO(Name,
                    new Dictionary<string, string> {{"Error", "Collection on non-ui thread"}});

            try
            {
                return Collect(values);
            }
            catch (Exception exception)
            {
                return new ContextCollectionDTO(Name,
                    new Dictionary<string, string>
                    {
                        {"Error", "Collection on non-ui thread"},
                        {"Exception", exception.ToString()}
                    });
            }
        }

        private ContextCollectionDTO Collect(Dictionary<string, string> values)
        {
            var variables = new StringBuilder();
            foreach (Form form in Application.OpenForms)
            {
                var fields =
                    form.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (typeof(Control).IsAssignableFrom(field.FieldType))
                    {
                        var control = (Control) field.GetValue(form);
                        if (control != null)
                        {
                            variables.AppendFormat("{1} = {2} [{0}];;", field.FieldType, field.Name, control.Text);
                        }
                    }
                    else
                    {
                        var value = field.GetValue(form);
                        variables.AppendFormat("{1} = {2} [{0}];;", field.FieldType, field.Name, value);
                    }
                }

                var properties =
                    form.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var property in properties)
                {
                    if (!property.CanRead || property.GetIndexParameters().Length > 0)
                        continue;

                    if (typeof(Control).IsAssignableFrom(property.PropertyType))
                    {
                        var control = (Control) property.GetValue(form, null);
                        if (control != null)
                        {
                            variables.AppendFormat("{1} = {2} [{0}];;", property.PropertyType, property.Name,
                                control.Text);
                        }
                    }
                    else
                    {
                        var value = property.GetValue(form, null);
                        variables.AppendFormat("{1} = {2} [{0}];;", property.PropertyType, property.Name, value);
                    }
                }


                if (values.ContainsKey(form.Name))
                {
                    for (var i = 0; i < 100; i++)
                    {
                        if (values.ContainsKey(form.Name + "_" + i))
                            continue;

                        values.Add(form.Name + "_" + i, variables.ToString());
                    }
                }
                else
                    values.Add(form.Name, variables.ToString());

                variables.Clear();
            }


            return new ContextCollectionDTO(Name, values);
        }
    }
}