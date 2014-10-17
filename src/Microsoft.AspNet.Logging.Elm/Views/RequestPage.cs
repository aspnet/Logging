namespace Microsoft.AspNet.Logging.Elm.Views
{
#line 1 "RequestPage.cshtml"
using System

#line default
#line hidden
    ;
#line 2 "RequestPage.cshtml"
using System.Globalization

#line default
#line hidden
    ;
#line 3 "RequestPage.cshtml"
using System.Linq

#line default
#line hidden
    ;
#line 4 "RequestPage.cshtml"
using Microsoft.AspNet.Logging.Elm

#line default
#line hidden
    ;
#line 5 "RequestPage.cshtml"
using Microsoft.AspNet.Logging.Elm.Views

#line default
#line hidden
    ;
#line 6 "RequestPage.cshtml"
using Microsoft.Framework.Logging

#line default
#line hidden
    ;
    using System.Threading.Tasks;

    public class RequestPage : Microsoft.AspNet.Diagnostics.Views.BaseView
    {
#line 9 "RequestPage.cshtml"

    public RequestPage(RequestPageModel model)
    {
        Model = model;
    }

    public RequestPageModel Model { get; set; }

#line default
#line hidden
        #line hidden
        public RequestPage()
        {
        }

        #pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral(@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>ELM</title>
    <style>
        body {
    font-family: 'Segoe UI', Tahoma, Arial, Helvtica, sans-serif;
    font-size: 0.9em;
    line-height: 1.4em;
    width: 90%;
    margin: 0px auto;
}

h1, h2 {
    font-weight: normal;
}

table {
    border-spacing: 0px;
    width: 100%;
    border-collapse: collapse;
    border: 1px solid black;
    white-space: pre-wrap;
}

td {
    text-overflow: ellipsis;
    overflow: hidden;
}

th {
    font-family: Arial;
}

td, th {
    padding: 8px;
}

tr:nth-child(2n) {
    background-color: #F6F6F6;
}

#headerTable {
    border: none;
    height: 100%;
}

#headerTd {
    white-space: normal;
}

#label {
    width: 20%;
    border-right: 1px solid black;
}

#logs>tbody>tr>td {
    border-right: 1px dashed lightgray;
}

#logs>thead>tr>th {
    border: 1px solid black;
}

.Critical {
    background-color: red;
    color: white;
}

.Error {
    color: red;
}

.Information {
    color: blue;
}

.Verbose {
    color: black;
}

.Warning {
    color: orange;
}
    </style>
</head>
<body>
    <h1>ELM</h1>

    <h2>Request Details</h2>
    <table id=""requestDetails"">
        <colgroup><col id=""label"" /><col /></colgroup>
");
#line 32 "RequestPage.cshtml"
        

#line default
#line hidden

#line 32 "RequestPage.cshtml"
          
            var context = Model.Logs.First().Context;
        

#line default
#line hidden

            WriteLiteral("\r\n        <tr>\r\n            <th>Path</th>\r\n            <td>");
#line 37 "RequestPage.cshtml"
           Write(context.Path);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Host</th>\r\n            <td>");
#line 41 "RequestPage.cshtml"
           Write(context.Host);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Content Type</th>\r\n          " +
"  <td>");
#line 45 "RequestPage.cshtml"
           Write(context.ContentType);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Method</th>\r\n            <td>" +
"");
#line 49 "RequestPage.cshtml"
           Write(context.Method);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Protocol</th>\r\n            <t" +
"d>");
#line 53 "RequestPage.cshtml"
           Write(context.Protocol);

#line default
#line hidden
            WriteLiteral(@"</td>
        </tr>
        <tr>
            <th>Headers</th>
            <td id=""headerTd"">
                <table id=""headerTable"">
                    <thead>
                        <tr>
                            <th>Variable</th>
                            <th>Value</th>
                        </tr>
                    </thead>
                    <tbody>
");
#line 66 "RequestPage.cshtml"
                        

#line default
#line hidden

#line 66 "RequestPage.cshtml"
                         foreach (var header in context.Headers)
                        {

#line default
#line hidden

            WriteLiteral("                            <tr>\r\n                                <td>");
#line 69 "RequestPage.cshtml"
                               Write(header.Key);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                <td>");
#line 70 "RequestPage.cshtml"
                               Write(string.Join(";", header.Value));

#line default
#line hidden
            WriteLiteral("</td>\r\n                            </tr>\r\n");
#line 72 "RequestPage.cshtml"
                        }

#line default
#line hidden

            WriteLiteral("                    </tbody>\r\n                </table>\r\n            </td>\r\n      " +
"  </tr>\r\n        <tr>\r\n            <th>Status Code</th>\r\n            <td>");
#line 79 "RequestPage.cshtml"
           Write(context.StatusCode);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>User</th>\r\n            \r\n    " +
"        <td>");
#line 84 "RequestPage.cshtml"
           Write(context.User.Identity.Name);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Scheme</th>\r\n            <td>" +
"");
#line 88 "RequestPage.cshtml"
           Write(context.Scheme);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Query</th>\r\n            <td>");
#line 92 "RequestPage.cshtml"
           Write(context.Query.Value);

#line default
#line hidden
            WriteLiteral("</td>\r\n        </tr>\r\n        <tr>\r\n            <th>Cookies</th>\r\n            <td" +
">\r\n");
#line 97 "RequestPage.cshtml"
                

#line default
#line hidden

#line 97 "RequestPage.cshtml"
                 if (context.Cookies.Any())
                {

#line default
#line hidden

            WriteLiteral(@"                    <table id=""queryTable"">
                        <thead>
                            <tr>
                                <th>Variable</th>
                                <th>Value</th>
                            </tr>
                        </thead>
                        <tbody>
");
#line 107 "RequestPage.cshtml"
                            

#line default
#line hidden

#line 107 "RequestPage.cshtml"
                             foreach (var cookie in context.Cookies)
                            {

#line default
#line hidden

            WriteLiteral("                                <tr>\r\n                                    <td>");
#line 110 "RequestPage.cshtml"
                                   Write(cookie.Key);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                    <td>");
#line 111 "RequestPage.cshtml"
                                   Write(string.Join(";", cookie.Value));

#line default
#line hidden
            WriteLiteral("</td>\r\n                                </tr>\r\n");
#line 113 "RequestPage.cshtml"
                            }

#line default
#line hidden

            WriteLiteral("                        </tbody>\r\n                    </table>\r\n");
#line 116 "RequestPage.cshtml"
                }

#line default
#line hidden

            WriteLiteral("            </td>\r\n        </tr>\r\n    </table>\r\n\r\n    <h2>Logs</h2>\r\n    <form me" +
"thod=\"get\">\r\n        <select name=\"level\">\r\n");
#line 124 "RequestPage.cshtml"
            

#line default
#line hidden

#line 124 "RequestPage.cshtml"
             foreach (var severity in Enum.GetValues(typeof(TraceType)))
            {
                var s = (int)severity;
                if ((int)Model.Options.MinLevel == s)
                {

#line default
#line hidden

            WriteLiteral("                    <option");
            WriteAttribute("value", Tuple.Create(" value=\"", 3668), Tuple.Create("\"", 3678), 
            Tuple.Create(Tuple.Create("", 3676), Tuple.Create<System.Object, System.Int32>(s, 3676), false));
            WriteLiteral(" selected=\"selected\">");
#line 129 "RequestPage.cshtml"
                                                      Write(severity);

#line default
#line hidden
            WriteLiteral("</option>\r\n");
#line 130 "RequestPage.cshtml"
                }
                else
                {

#line default
#line hidden

            WriteLiteral("                    <option");
            WriteAttribute("value", Tuple.Create(" value=\"", 3807), Tuple.Create("\"", 3817), 
            Tuple.Create(Tuple.Create("", 3815), Tuple.Create<System.Object, System.Int32>(s, 3815), false));
            WriteLiteral(">");
#line 133 "RequestPage.cshtml"
                                  Write(severity);

#line default
#line hidden
            WriteLiteral("</option>\r\n");
#line 134 "RequestPage.cshtml"
                }
            }

#line default
#line hidden

            WriteLiteral("        </select>\r\n        <input type=\"text\" name=\"name\"");
            WriteAttribute("value", Tuple.Create(" value=\"", 3930), Tuple.Create("\"", 3963), 
            Tuple.Create(Tuple.Create("", 3938), Tuple.Create<System.Object, System.Int32>(Model.Options.NamePrefix, 3938), false));
            WriteLiteral(@" />
        <input type=""submit"" value=""filter"" />
    </form>
    <table id=""logs"">
        <thead>
            <tr>
                <th>Date</th>
                <th>Time</th>
                <th>Severity</th>
                <th>Name</th>
                <th>State</th>
                <th>Error</th>
            </tr>
        </thead>
");
#line 151 "RequestPage.cshtml"
        

#line default
#line hidden

#line 151 "RequestPage.cshtml"
         foreach (var log in Model.Logs)
        {

#line default
#line hidden

            WriteLiteral("            <tr>\r\n                <td>");
#line 154 "RequestPage.cshtml"
               Write(string.Format("{0:MM/dd/yy}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                <td>");
#line 155 "RequestPage.cshtml"
               Write(string.Format("{0:H:mm:ss}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                <td");
            WriteAttribute("class", Tuple.Create(" class=\"", 4540), Tuple.Create("\"", 4561), 
            Tuple.Create(Tuple.Create("", 4548), Tuple.Create<System.Object, System.Int32>(log.Severity, 4548), false));
            WriteLiteral(">");
#line 156 "RequestPage.cshtml"
                                     Write(log.Severity);

#line default
#line hidden
            WriteLiteral("</td>\r\n                <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 4602), Tuple.Create("\"", 4619), 
            Tuple.Create(Tuple.Create("", 4610), Tuple.Create<System.Object, System.Int32>(log.Name, 4610), false));
            WriteLiteral(">");
#line 157 "RequestPage.cshtml"
                                 Write(log.Name);

#line default
#line hidden
            WriteLiteral("</td>\r\n                <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 4656), Tuple.Create("\"", 4674), 
            Tuple.Create(Tuple.Create("", 4664), Tuple.Create<System.Object, System.Int32>(log.State, 4664), false));
            WriteLiteral(" class=\"logState\" width=\"100px\">");
#line 158 "RequestPage.cshtml"
                                                                 Write(log.State);

#line default
#line hidden
            WriteLiteral("</td>\r\n                <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 4743), Tuple.Create("\"", 4765), 
            Tuple.Create(Tuple.Create("", 4751), Tuple.Create<System.Object, System.Int32>(log.Exception, 4751), false));
            WriteLiteral(">");
#line 159 "RequestPage.cshtml"
                                      Write(log.Exception);

#line default
#line hidden
            WriteLiteral("</td>\r\n            </tr>\r\n");
#line 161 "RequestPage.cshtml"
        }

#line default
#line hidden

            WriteLiteral("    </table>\r\n</body>\r\n</html>");
        }
        #pragma warning restore 1998
    }
}
