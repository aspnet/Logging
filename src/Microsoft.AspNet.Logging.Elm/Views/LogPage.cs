namespace Microsoft.AspNet.Logging.Elm.Views
{
#line 1 "LogPage.cshtml"
using System

#line default
#line hidden
    ;
#line 2 "LogPage.cshtml"
using System.Globalization

#line default
#line hidden
    ;
#line 3 "LogPage.cshtml"
using System.Linq

#line default
#line hidden
    ;
#line 4 "LogPage.cshtml"
using Microsoft.AspNet.Logging.Elm.Views

#line default
#line hidden
    ;
    using System.Threading.Tasks;

    public class LogPage : Microsoft.AspNet.Diagnostics.Views.BaseView
    {
#line 7 "LogPage.cshtml"

    public LogPage(LogPageModel model)
    {
        Model = model;
    }

    public LogPageModel Model { get; set; }

#line default
#line hidden
        #line hidden
        public LogPage()
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
    width: 800px;
    font-family: monospace;
    white-space: nowrap;
}

table {
    margin: 0px auto;
    border-spacing: 1px;
}

td {
    padding: 4px;
}

thead {
    background-color: #B2F0E0;
}

tr {
    height: 23px;
}

tr:nth-child(2n) {
    background-color: #F5F5F5;
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
    <table>
        <thead>
            <tr>
                <th id=""date"">Date</th>
                <th id=""time"">Time</th>
                <th id=""severity"">Severity</th>
                <th id=""request"">Request</th>
                <th id=""host"">Host</th>
                <th id=""code"">Code</th>
                <th id=""state"">State</th>
                <th id=""error"">Error</th>
            </tr>
        </thead>
        <tbody>
");
#line 40 "LogPage.cshtml"
            

#line default
#line hidden

#line 40 "LogPage.cshtml"
             foreach (var log in Model.Logs)
            {

#line default
#line hidden

            WriteLiteral("                <tr>\r\n                    <td>");
#line 43 "LogPage.cshtml"
                   Write(string.Format("{0:MM/dd/yy}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 44 "LogPage.cshtml"
                   Write(string.Format("{0:H:mm:ss}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td");
            WriteAttribute("class", Tuple.Create(" class=\"", 1133), Tuple.Create("\"", 1154), 
            Tuple.Create(Tuple.Create("", 1141), Tuple.Create<System.Object, System.Int32>(log.Severity, 1141), false));
            WriteLiteral(">");
#line 45 "LogPage.cshtml"
                                         Write(log.Severity);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 46 "LogPage.cshtml"
                   Write(log.Context.Path);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 47 "LogPage.cshtml"
                   Write(log.Context.Host);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 48 "LogPage.cshtml"
                   Write(log.Context.StatusCode);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 49 "LogPage.cshtml"
                   Write(log.State);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 50 "LogPage.cshtml"
                   Write(log.Exception);

#line default
#line hidden
            WriteLiteral("</td>\r\n                </tr>\r\n");
#line 52 "LogPage.cshtml"
            }

#line default
#line hidden

            WriteLiteral("        </tbody>\r\n    </table>\r\n</body>\r\n</html>");
        }
        #pragma warning restore 1998
    }
}
