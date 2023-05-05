using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TKE.CPQ.DocEngine.Generators;
using TKE.SC.DocModel.ApiContracts;
using TKE.CPQ.DocModel.Templates;
using static TKE.CPQ.DocModel.Templates.OrderFormTemplate;

namespace TKE.CPQ.DocEngine.Factories
{
    public class OrderFormFactory : IDocumentFactory<OrderFormModel>
    {
        private readonly IDocumentGenerator _documentGenerator;
        private readonly IConfiguration _configuration;

        private readonly Func<Section, bool> _isHeaderSection = section => section.Name.Equals(Constants.Header, StringComparison.OrdinalIgnoreCase);
        private readonly Func<KeyValuePair<string, object>, string, bool> _isValidVariable = (variable, variableKey) => variable.Key.Equals(variableKey, StringComparison.OrdinalIgnoreCase);
        private readonly Func<Variable, string> _formatToStubProperty = variable => variable.DisplayName.Replace(" ", "");
        private readonly Func<string, string> _convertToCamelCase = name => Char.ToLowerInvariant(name[0]) + name.Substring(1);
        private readonly Func<double, string> _currency = amount => amount.ToString("C2");
        public OrderFormFactory(IDocumentGenerator documentGenerator, IConfiguration configuration)
        {
            _documentGenerator = documentGenerator;
            _configuration = configuration;
        }

        public Stream CreatePdf(RequestModel<OrderFormModel> requestModel)
        {
            var template = GetTemplatePath(Constants.OrderFormTemplate);
            var jsonTemplate = BuildSectionData(requestModel.DocumentModel);
            var headerTemplate = GetHeaderTemplate(jsonTemplate);
            var documentContent = GetDocumentContent(jsonTemplate);

            var document = _documentGenerator.CreateDocument(template, documentContent, headerTemplate);
            return document;
        }
        private string GetTemplatePath(string variable, string type = "HtmlTemplates", string section = "GcPdf")
        {
            var ext = type == Constants.HtmlTemplates ? Constants.HtmlExtension : Constants.JsonExtension;
            return Path.Combine(Directory.GetCurrentDirectory(),
                                _configuration[$"{section}:{type}"],
                                $"{_configuration[$"{section}:{variable}"] ?? variable}{ ext}");
        }


        private OrderFormTemplate BuildSectionData(OrderFormModel orderFormModel)
        {
            var template = JsonConvert.DeserializeObject<OrderFormTemplate>(File.ReadAllText($"{GetTemplatePath(Constants.OrderFormTemplate, Constants.JsonTemplates)}"));
            foreach (var section in template.Sections)
            {

                section.Variables = TransformVariables(section, orderFormModel);
                section.Tables = TransformTables(section, orderFormModel);
                section.Subsections = BuildSubSectionData(section, orderFormModel).ToArray();
            }
            return template;
        }
        private IEnumerable<Subsection> BuildSubSectionData(Section section, OrderFormModel orderFormModel)
        {
            if (!(bool)(section?.Subsections?.Any()))
            {
                return section?.Subsections;
            }
            foreach (var subsection in section.Subsections)
            {
                subsection.Variables = TransformVariables(subsection, orderFormModel);
                subsection.Tables = TransformTables(subsection, orderFormModel);
                subsection.Subsections = BuildSubSectionData(subsection, orderFormModel).ToArray();
            }
            return section.Subsections;
        }
        private Variable[] TransformVariables(Section section, OrderFormModel orderFormModel)
        {
            if (section?.Variables?.Length == 0)
            {
                return section?.Variables;
            }
            dynamic value = null;
            return (from variable in section.Variables
                    where IsVariableAvailableInRequestModel(orderFormModel, variable.Key, out value)
                    select new Variable
                    {
                        //TODO: Auto mapper
                        Key = variable.Key,
                        DisplayName = variable.DisplayName,
                        Value = value,
                        Classes = variable.Classes,
                        ComponentType = variable.ComponentType,
                        DataType = value.GetType().ToString()
                    }).ToArray();
        }

        private Table[] TransformTables(Section section, OrderFormModel orderFormModel)
        {
            if (section?.Tables?.Length == 0)
            {
                return section?.Tables;
            }
            var tables = new List<Table>();
            foreach (var table in section?.Tables)
            {
                foreach (var priceSection in orderFormModel.PriceSections)
                {
                    if (!table.Name.Equals(priceSection.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var _table = new Table
                    {
                        Name = table.Name,
                        Template = table.Template,
                        Headers = table.Headers,
                        Rows = (from row in priceSection.PriceKeyInfo
                                select (from header in table.Headers
                                        select new Variable
                                        {
                                            DisplayName = header.DisplayName,
                                            Value = GetRowValueByReflection(header.Key, row, orderFormModel.PriceValue)
                                        }).ToArray()).ToArray()
                    };
                    if (table.Footer.Any())
                    {
                        _table.Footer = GenerateFooter(_table.Rows);
                    }
                    tables.Add(_table);
                    break;
                }
            }
            return CalculateTotalPrice(tables, section);
        }
        //Refection helper

        private Table[] CalculateTotalPrice(List<Table> tables, Section section)
        {
            var totalPrice = section.Tables.Where(x => x.Name.Equals("Pricing Summary")).ToList();
            if ((bool)totalPrice?.Any() && (bool)(totalPrice[0].Headers?.Any()))
            {
                var _table = new Table
                {
                    Name = totalPrice[0].Name,
                    Template = totalPrice[0].Template,
                    Headers = totalPrice.FirstOrDefault().Headers,
                    Rows = new Variable[tables.Count()][],
                    Footer = totalPrice[0].Footer
                };
                tables = GenerateRowsForTotalPriceTable(tables, section, _table);
            }
            return tables.ToArray();
        }
        public dynamic GenerateRowsForTotalPriceTable(List<Table> tables, Section section, Table totalPrice)
        {
            int rowCount = 0;

            foreach (var table in tables)
            {
                var counter = 0;
                var newRow = new Variable[totalPrice.Headers.Count()];
                foreach (var header in totalPrice.Headers)
                {
                    dynamic currentHeaderValue = null;
                    var agregatorColumn = header.Value.ToString();
                    var filterValue = header.Key.ToString();
                    var valueAction = header.DataType.ToString();
                    if (agregatorColumn.Equals("=rowsum", StringComparison.OrdinalIgnoreCase))
                    {
                        currentHeaderValue = _currency(RowSum(newRow));
                    }
                    else
                    {
                        var currentHeaderValueCollection = table.Rows.
                                                    Where(x => x.Any(y => y.DisplayName.Equals("Component") && y.Value.ToString().Contains(filterValue, StringComparison.CurrentCultureIgnoreCase))).
                                                    SelectMany(x => x.Where(y => y.DisplayName.Contains(agregatorColumn))).
                                                    Select(x => x.Value.ToString());

                        if (valueAction.Contains("positivesummation", StringComparison.OrdinalIgnoreCase))
                        {
                            currentHeaderValue = _currency(ConvertToDouble(currentHeaderValueCollection).Where(x => x > 0).Sum(x => x));
                        }
                        else if (valueAction.Contains("negativesummation", StringComparison.OrdinalIgnoreCase))
                        {
                            currentHeaderValue = _currency(ConvertToDouble(currentHeaderValueCollection).Where(x => x < 0).Sum(x => x));
                        }
                        else if (valueAction.Contains("summation", StringComparison.OrdinalIgnoreCase))
                        {
                            currentHeaderValue = _currency(ConvertToDouble(currentHeaderValueCollection).Sum(x => x));
                        }
                        else
                        {
                            currentHeaderValue = currentHeaderValueCollection.FirstOrDefault();
                        }
                    }

                    newRow[counter] = new Variable()
                    {
                        DisplayName = header.DisplayName,
                        Key = header.DisplayName,
                        ComponentType = header.ComponentType,
                        DataType = header.DataType,
                        Value = currentHeaderValue
                    };
                    counter++;
                }
                totalPrice.Rows[rowCount] = newRow;
                rowCount++;
            }
            totalPrice.Footer = GenerateFooterForTotalPrice(totalPrice.Rows, totalPrice);
            tables.Add(totalPrice);
            return tables;
        }
        private string GetRowValueByReflection(string propertyName, PriceValuesDetails priceValuesDetails, IDictionary<string, UnitPriceValues> priceInfo)
        {
            var value = GetPropertyValueByReflection(priceValuesDetails, propertyName);
            if (string.IsNullOrEmpty(value))
            {
                var key = string.Empty;
                if (!string.IsNullOrEmpty(priceValuesDetails.ItemNumber))
                {
                    key = _convertToCamelCase(priceValuesDetails.ItemNumber);
                }
                if (string.IsNullOrEmpty(priceInfo.Keys.FirstOrDefault(x => x.Equals(key))))
                {
                    return "";
                }

                value = propertyName.ToUpper().Contains("PRICE") ? $"{_currency(Convert.ToDouble(GetPropertyValueByReflection(priceInfo[key], propertyName)))}" : GetPropertyValueByReflection(priceInfo[key], propertyName);
            }
            return value;
        }

        private string GetPropertyValueByReflection(dynamic source, string propertyName)
        {
            try
            {
                return source.GetType().GetProperty(propertyName).GetValue(source, null)?.ToString();
            }
            catch (Exception)
            {
                return null;
            }

        }


        private bool IsVariableAvailableInRequestModel(OrderFormModel orderFormModel, string variableKey, out object inputValue)
        {
            inputValue = null;
            var orderFormData = orderFormModel.Variables.FirstOrDefault(x => _isValidVariable(x, variableKey));
            if (orderFormData.Value is null)
            {
                return false;
            }

            inputValue = orderFormData.Value.ToString();
            return true;
        }



        public string GetHeaderTemplate(OrderFormTemplate jsonTemplate)
        {
            var headerSection = jsonTemplate.Sections.FirstOrDefault(x => _isHeaderSection(x));
            return BuildVariableTemplate(headerSection);
        }
        public string GetDocumentContent(OrderFormTemplate jsonTemplate)
        {
            var templateBuilder = new StringBuilder();
            foreach (var section in jsonTemplate.Sections.Where(x => !_isHeaderSection(x)).ToList())
            {
                templateBuilder.AppendLine(BuildVariableTemplate(section));

                templateBuilder.AppendLine(BuildTableTemplate(section));
                if(section.Subsections!=null && section.Subsections.Any())
                {
                    templateBuilder.AppendLine(GetSubSectionDocumentContent(section.Subsections));
                }
            }
            return templateBuilder.ToString();
        }
        public string GetSubSectionDocumentContent(Subsection[] section)
        {
            var templateBuilder = new StringBuilder();
            foreach (var subSection in section)
            {
                templateBuilder.AppendLine(BuildTableTemplate(subSection));
                if (subSection.Subsections != null && subSection.Subsections.Any())
                {
                    templateBuilder.AppendLine(GetSubSectionDocumentContent(subSection.Subsections));
                }
            }
            return templateBuilder.ToString();
        }

        private IList<object> CreateDynamicObjectList(IEnumerable<Variable> collection)
        {
            var lstVariables = new List<object>();
            var exo = CreateDynamicObject(collection);
            lstVariables.Add(exo);
            return lstVariables;
        }

        private dynamic CreateDynamicObject(IEnumerable<Variable> collection)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            foreach (var variable in collection)
            {
                exo = UpsertProperty(exo, _formatToStubProperty(variable), variable.Value);
            }

            return exo;
        }

        private dynamic UpsertProperty(dynamic exo, string propertyName, object propertyValue)
        {
            if (((IDictionary<string, object>)exo).ContainsKey(propertyName))
            {
                ((IDictionary<string, object>)exo)[propertyName] = propertyValue;
                return exo;
            }
            ((IDictionary<string, object>)exo).Add(propertyName, propertyValue);
            return exo;
        }

        private string BindDataToHtmlTemplate(string template, object dataToBind)
        {
            var builder = new Stubble.Core.Builders.StubbleBuilder();
            var boundTemplate = builder.Build().Render(template, dataToBind);
            return boundTemplate;
        }


        private string BuildVariableTemplate(Section section)
        {
            var template = File.ReadAllText(GetTemplatePath(section.Template));
            var lstVariables = CreateDynamicObjectList(section.Variables);
            //TODO: update the name Header
            //Bind the template to data 
            template = AddOrRemoveFields(template, section);
            return BindDataToHtmlTemplate(template, new { Header = lstVariables });
        }
        public string AddOrRemoveFields(string template, Section section)
        {
            if (section.Variables.Any(x => x.Key.Contains("BLDGCODE")))
            {
                section.Subsections[0].Template = section.Variables.Where(x => x.Key.Contains("BLDGCODE")).First().Value.ToString().ToLower();
                template = template.Replace("subSection", BuildVariableTemplate(section.Subsections[0]));
            }
            else if (section.Name.Contains("Building"))
            {
                template = template.Replace("subSection", string.Empty);
            }
            else if ((bool)section?.Subsections?.Any())
            {
                if (section.Variables.Any(x => x.Key.Equals("commonName_SP")))
                {
                    section.Subsections[0].Template = section.Variables.Where(x => x.Key.Equals("commonName_SP")).ToList()[0].Value.ToString();
                }
                foreach (var subSection in section.Subsections)
                {
                    template = template.Replace("subSection", BuildVariableTemplate(subSection));
                    template = template.Replace(subSection.Template, BuildVariableTemplate(subSection));
                }
            }
            if (section.Name.Contains("Unit") && (section.Variables.Any(x => x.Key.Contains("DOORTYPR") && (x.Value.ToString()).Equals("NR"))))
            {
                template = template.Replace("Rear Door Width", string.Empty);
                template = template.Replace("Rear Door Height", string.Empty);
            }
            return template;
        }
        private string BuildTableTemplate(Section section)
        {
            var tableBuilder = new StringBuilder();
            if (section.Tables is null)
            {
                return "";
            }
            foreach (var table in section.Tables)
            {
                var template = File.ReadAllText(GetTemplatePath(table.Template));
                var rowData = (from row in table.Rows
                               select CreateDynamicObject(row)).ToList();
                if ((table.Name.Equals("Pricing Summary")))
                {
                    var dataToBindTotalPriceTable = new
                    {
                        SectionTable = new
                        {
                            TableName = table.Name,
                            Section = ((IDictionary<string, object>)rowData.FirstOrDefault())["Section"],
                            TRow = rowData,
                            ConfigurationListPriceTotal = table.Footer[1].Value,
                            CorporateAssistanceTotal = table.Footer[2].Value,
                            ProductSubsidiesTotal = table.Footer[3].Value,
                            StrategicDiscountTotal = table.Footer[4].Value,
                            TotalPrice = table.Footer[5].Value,
                        }
                    };
                    var boundTemplate = BindDataToHtmlTemplate(template, dataToBindTotalPriceTable);
                    tableBuilder.AppendLine(boundTemplate);
                }
                else if ((table.Template.Equals("sectionTable")) || (table.Template.Equals("SectionTableWithOpenings")))
                {
                    var dataToBindSectionTable = new
                    {
                        SectionTable = new
                        {
                            TableName = table.Name,
                            Section = ((IDictionary<string, object>)rowData.FirstOrDefault())["Section"],
                            TRow = rowData,
                            SubTotalSum = table.Footer[table.Footer.Length - 1].Value
                        }
                    };
                    var boundTemplate = BindDataToHtmlTemplate(template, dataToBindSectionTable);
                    tableBuilder.AppendLine(boundTemplate);
                }
                else
                {
                    var dataToBindSectionTable = new
                    {
                        SectionTable = new
                        {
                            TableName = table.Name,
                            TRow = rowData
                        }
                    };
                    var boundTemplate = BindDataToHtmlTemplate(template, dataToBindSectionTable);
                    tableBuilder.AppendLine(boundTemplate);
                }
            }
            return tableBuilder.ToString();
        }

        private Variable[] GenerateFooter(Variable[][] rows)
        {
            var subTotal = 0.0;
            var footer = new Variable[rows[0].Length];
            footer[rows[0].Length - 2] = new Variable()
            {
                Key = "SubTotal",
                Value = "SubTotal"
            };
            var valuesList = rows.Where(x => !string.IsNullOrEmpty(x[x.Length - 1].Value.ToString())).Select(x => x[x.Length-1].Value.ToString().Replace("$", string.Empty)).ToList();
            valuesList.ForEach(x => subTotal += Double.Parse(x, System.Globalization.NumberStyles.Currency));
            footer[rows[0].Length - 1] = new Variable()
            {
                Key = "Sum",
                Value = $"{_currency(subTotal)}"
            };
            return footer;
        }
        private Variable[] GenerateFooterForTotalPrice(Variable[][] rows, Table totalPrice)
        {
            var footerRow = new Variable[totalPrice.Footer.Length];
            var counter = 0;
            foreach (var col in totalPrice.Footer)
            {
                dynamic currentValue = col.Value;
                if (col.DataType.Equals("calculation", StringComparison.OrdinalIgnoreCase))
                {
                    if (col.Value.ToString().Equals("=colsum", StringComparison.OrdinalIgnoreCase))
                    {
                        currentValue = _currency(ColSum(rows, counter));
                    }
                    if (col.Value.ToString().Equals("=rowsum", StringComparison.OrdinalIgnoreCase))
                    {
                        currentValue = _currency(RowSum(footerRow));
                    }
                }
                footerRow[counter] = new Variable()
                {
                    Key = col.Key,
                    Value = currentValue
                };
                counter++;
            }
            return footerRow;
        }
        public double RowSum(Variable[] row)
        {
            var cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            return (row.Where(x => !(x is null) && x.DataType.Contains("summation", StringComparison.OrdinalIgnoreCase))
                             .Select(x => double.TryParse(x.Value.ToString(), System.Globalization.NumberStyles.Currency, cultureInfo, out double value) ? value : 0)
                             .Sum(x => x));
        }
        public double ColSum(Variable[][] rows, int colIndex)
        {
            return ConvertToDouble(rows.Select(x => x[colIndex].Value?.ToString())).Sum(x => x);
        }

        public IEnumerable<double> ConvertToDouble(IEnumerable<string> collection)
        {
            var cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            return collection.Where(x => !string.IsNullOrEmpty(x))
                                                .Select(x =>
                                                    double.TryParse(x, System.Globalization.NumberStyles.Currency, cultureInfo, out double value)
                                                    ? value
                                                    : 0);
        }
    }




    public static class Constants
    {
        public const string OrderFormTemplate = "OrderFormTemplate";
        public const string JsonTemplates = "JsonTemplates";
        public const string HtmlTemplates = "HtmlTemplates";
        public const string HtmlExtension = ".html";
        public const string JsonExtension = ".json";
        public const string Header = "Projet Details";

    }

    public static class TableReflectionHelper
    {
        public static dynamic GetTableValue(Table table, string propertyName)
        {
            try
            {
                string[] nameParts = propertyName.Split('.');
                dynamic currentValue = table;
                foreach (var property in nameParts)
                {
                    currentValue = GetValue(currentValue, property);
                }
                return currentValue;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static dynamic GetValue(dynamic table, String propName)
        {
            var indexCollection = propName.Where(Char.IsDigit).ToArray();
            var newPropName = propName.Split("[").FirstOrDefault();
            if (indexCollection.Any())
            {
                if (table.GetType().GetProperty(newPropName).GetValue(table, null) is Variable[][] rows)
                {
                    return rows[Convert.ToInt32(indexCollection[0].ToString())][Convert.ToInt32(indexCollection[1].ToString())];
                }
                if (table.GetType().GetProperty(newPropName).GetValue(table, null) is Variable[] footer)
                {
                    return footer[Convert.ToInt32(indexCollection[0].ToString())];
                }
            }
            return table.GetType().GetProperty(propName).GetValue(table, null);
        }
    }
}