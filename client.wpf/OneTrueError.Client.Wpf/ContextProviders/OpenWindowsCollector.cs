using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using OneTrueError.Client.ContextProviders;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.Wpf.ContextProviders
{
    /// <summary>
    ///     Serializes all open windows into the context collection named <c>"OpenWindows"</c>
    /// </summary>
    public class OpenWindowsCollector : IContextInfoProvider
    {
        /// <summary>
        ///     Returns <c>OpenWindows</c>.
        /// </summary>
        public string Name
        {
            get { return "OpenWindows"; }
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
            foreach (Window window in Application.Current.Windows)
            {
                if (!window.CheckAccess())
                {
                    invocationRequired = true;
                }
            }
            if (invocationRequired)
            {
                Application.Current.Dispatcher.VerifyAccess();
                return new ContextCollectionDTO(Name,
                    new Dictionary<string, string> {{"Error", "Collection on non-ui thread"}});
            }

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
            foreach (Window window in Application.Current.Windows)
            {
                var fields =
                    window.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (typeof(Control).IsAssignableFrom(field.FieldType))
                    {
                        var control = (Control)field.GetValue(window);
                        if (control != null)
                        {
                            variables.AppendFormat("{1} = {2} [{0}];;", field.FieldType, field.Name, control.Name);
                        }
                    }
                    else
                    {
                        var value = field.GetValue(window);
                        variables.AppendFormat("{1} = {2} [{0}];;", field.FieldType, field.Name, value);
                    }
                }

                var properties =
                    window.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var property in properties)
                {
                    if (!property.CanRead || property.GetIndexParameters().Length > 0)
                        continue;

                    if (typeof(Control).IsAssignableFrom(property.PropertyType))
                    {
                        var control = (Control)property.GetValue(window, null);
                        if (control != null)
                        {
                            variables.AppendFormat("{1} = {2} [{0}];;", property.PropertyType, property.Name,
                                control.Name);
                        }
                    }
                    else
                    {
                        var value = property.GetValue(window, null);
                        variables.AppendFormat("{1} = {2} [{0}];;", property.PropertyType, property.Name, value);
                    }
                }


                if (values.ContainsKey(window.Name))
                {
                    for (var i = 0; i < 100; i++)
                    {
                        if (values.ContainsKey(window.Name + "_" + i))
                            continue;

                        values.Add(window.Name + "_" + i, variables.ToString());
                    }
                }
                else
                    values.Add(window.Name, variables.ToString());

                variables.Clear();
            }


            return new ContextCollectionDTO(Name, values);
        }
    }
}
