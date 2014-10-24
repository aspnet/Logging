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
#line 5 "LogPage.cshtml"
using Microsoft.Framework.Logging

#line default
#line hidden
    ;
    using System.Threading.Tasks;

    public class LogPage : Microsoft.AspNet.Diagnostics.Views.BaseView
    {
#line 8 "LogPage.cshtml"

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
    <script src=""//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js""></script>
    <style>
        body {
    font-family: 'Segoe UI', Tahoma, Arial, Helvetica, sans-serif;
    font-size: .813em;
    line-height: 1.4em;
    white-space: nowrap;
    margin: 10px;
}

col:nth-child(2) {
    background-color: #FAFAFA;
}

h1 {
    font-family: Arial, Helvetica, sans-serif;
    font-size: 2em;
}

table {
    margin: 0px auto;
    border-spacing: 0px;
    table-layout: fixed;
    width: 100%; 
    border-collapse: collapse;
}

td {
    text-overflow: ellipsis;
    overflow: hidden;
}

td, th {
    padding: 4px;
}

thead {
    font-size: 1em;
    font-family: Arial;
}

tr {
    height: 23px;
}

tr:nth-child(2n) {
    background-color: #F6F6F6;
}

#requestHeader {
    border-bottom: solid 1px gray;
    border-top: solid 1px gray;
    margin-bottom: 2px;
    font-size: 1em;
    line-height: 2em;
}

.date, .time {
    width: 70px; 
}

.logHeader {
    border-bottom: 1px solid lightgray;
    color: gray;
    text-align: left;
}

.logState {
    text-overflow: ellipsis;
    overflow: hidden;
}

.logTd {
    border-left: 1px solid gray;
    padding: 0px;
}

.logs {
    width: 80%;
}

.requestRow>td {
    border-bottom: solid 1px gray;
}

.severity {
    width: 80px;
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
    <form method=""get"">
        <select name=""level"">
");
#line 30 "LogPage.cshtml"
            

#line default
#line hidden

#line 30 "LogPage.cshtml"
             foreach (var severity in Enum.GetValues(typeof(TraceType)))
            {
                var s = (int)severity;
                if ((int)Model.Options.MinLevel == s)
                {

#line default
#line hidden

            WriteLiteral("                    <option");
            WriteAttribute("value", Tuple.Create(" value=\"", 848), Tuple.Create("\"", 858), 
            Tuple.Create(Tuple.Create("", 856), Tuple.Create<System.Object, System.Int32>(s, 856), false));
            WriteLiteral(" selected=\"selected\">");
#line 35 "LogPage.cshtml"
                                                      Write(severity);

#line default
#line hidden
            WriteLiteral("</option>\r\n");
#line 36 "LogPage.cshtml"
                }
                else
                {

#line default
#line hidden

            WriteLiteral("                    <option");
            WriteAttribute("value", Tuple.Create(" value=\"", 987), Tuple.Create("\"", 997), 
            Tuple.Create(Tuple.Create("", 995), Tuple.Create<System.Object, System.Int32>(s, 995), false));
            WriteLiteral(">");
#line 39 "LogPage.cshtml"
                                  Write(severity);

#line default
#line hidden
            WriteLiteral("</option>\r\n");
#line 40 "LogPage.cshtml"
                }
            }

#line default
#line hidden

            WriteLiteral("        </select>\r\n        <input type=\"text\" name=\"name\"");
            WriteAttribute("value", Tuple.Create(" value=\"", 1110), Tuple.Create("\"", 1143), 
            Tuple.Create(Tuple.Create("", 1118), Tuple.Create<System.Object, System.Int32>(Model.Options.NamePrefix, 1118), false));
            WriteLiteral(@"/>
        <input type=""submit"" value=""filter""/>
    </form>

    <table id=""requestTable"">  
        <thead id=""requestHeader"">
            <tr>
                <th class=""path"">Path</th>
                <th class=""host"">Host</th>
                <th class=""statusCode"">Status Code</th>
                <th class=""logs"">Logs</th>
            </tr>
        </thead>
        <colgroup>
            <col />
            <col />
            <col />
            <col />
        </colgroup>
");
#line 62 "LogPage.cshtml"
        

#line default
#line hidden

#line 62 "LogPage.cshtml"
         foreach (var logs in Model.Logs.GroupBy(g => g.Context))
        {

#line default
#line hidden

            WriteLiteral("            <tbody>\r\n                <tr class=\"requestRow\">\r\n");
#line 66 "LogPage.cshtml"
                    

#line default
#line hidden

#line 66 "LogPage.cshtml"
                      
                        var requestPath = Model.Options.Path.Value + "/" + logs.Key.RequestID;
                    

#line default
#line hidden

            WriteLiteral("\r\n                    <td><a");
            WriteAttribute("href", Tuple.Create(" href=\"", 1957), Tuple.Create("\"", 1976), 
            Tuple.Create(Tuple.Create("", 1964), Tuple.Create<System.Object, System.Int32>(requestPath, 1964), false));
            WriteAttribute("title", Tuple.Create(" title=\"", 1977), Tuple.Create("\"", 1999), 
            Tuple.Create(Tuple.Create("", 1985), Tuple.Create<System.Object, System.Int32>(logs.Key.Path, 1985), false));
            WriteLiteral(">");
#line 69 "LogPage.cshtml"
                                                                 Write(logs.Key.Path);

#line default
#line hidden
            WriteLiteral("</a></td>\r\n                    <td>");
#line 70 "LogPage.cshtml"
                   Write(logs.Key.Host);

#line default
#line hidden
            WriteLiteral("</td>\r\n                    <td>");
#line 71 "LogPage.cshtml"
                   Write(logs.Key.StatusCode);

#line default
#line hidden
            WriteLiteral(@"</td>
                    <td class=""logTd"">
                        <table class=""logTable"">
                            <thead class=""logHeader"">
                                <tr>
                                    <th class=""date"">Date</th>
                                    <th class=""time"">Time</th>
                                    <th class=""name"">Name</th>
                                    <th class=""severity"">Severity</th>
                                    <th class=""state"">State</th>
                                    <th class=""error"">Error</th>
                                </tr>
                            </thead>
                            <tbody>
                                
");
#line 86 "LogPage.cshtml"
                                

#line default
#line hidden

#line 86 "LogPage.cshtml"
                                 foreach (var log in logs.Reverse())
                                {
                                    var scopes = "";
                                    if (log.Scopes != null)
                                    {
                                        // classes cannot begin with a number, prepend an underscore
                                        scopes = string.Join(" _", log.Scopes);
                                    }
                                    var logState = string.Format("{0} _{1}", "logState", scopes);

#line default
#line hidden

            WriteLiteral("                                    <tr class=\"logRow\">\r\n                        " +
"                <td>");
#line 96 "LogPage.cshtml"
                                       Write(string.Format("{0:MM/dd/yy}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                                        <td>");
#line 97 "LogPage.cshtml"
                                       Write(string.Format("{0:H:mm:ss}", log.Time));

#line default
#line hidden
            WriteLiteral("</td>\r\n                                        <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 3768), Tuple.Create("\"", 3785), 
            Tuple.Create(Tuple.Create("", 3776), Tuple.Create<System.Object, System.Int32>(log.Name, 3776), false));
            WriteLiteral(">");
#line 98 "LogPage.cshtml"
                                                         Write(log.Name);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                        <td");
            WriteAttribute("class", Tuple.Create(" class=\"", 3846), Tuple.Create("\"", 3867), 
            Tuple.Create(Tuple.Create("", 3854), Tuple.Create<System.Object, System.Int32>(log.Severity, 3854), false));
            WriteLiteral(">");
#line 99 "LogPage.cshtml"
                                                             Write(log.Severity);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                        <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 3932), Tuple.Create("\"", 3950), 
            Tuple.Create(Tuple.Create("", 3940), Tuple.Create<System.Object, System.Int32>(log.State, 3940), false));
            WriteAttribute("class", Tuple.Create(" class=\"", 3951), Tuple.Create("\"", 3968), 
            Tuple.Create(Tuple.Create("", 3959), Tuple.Create<System.Object, System.Int32>(logState, 3959), false));
            WriteLiteral(">");
#line 100 "LogPage.cshtml"
                                                                            Write(log.State);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                        <td");
            WriteAttribute("title", Tuple.Create(" title=\"", 4030), Tuple.Create("\"", 4052), 
            Tuple.Create(Tuple.Create("", 4038), Tuple.Create<System.Object, System.Int32>(log.Exception, 4038), false));
            WriteLiteral(">");
#line 101 "LogPage.cshtml"
                                                              Write(log.Exception);

#line default
#line hidden
            WriteLiteral("</td>\r\n                                    </tr>\r\n");
#line 103 "LogPage.cshtml"
                                }

#line default
#line hidden

            WriteLiteral("                            </tbody>\r\n                        </table>\r\n         " +
"           </td>\r\n                </tr>\r\n            </tbody>\r\n");
#line 109 "LogPage.cshtml"
        }

#line default
#line hidden

            WriteLiteral(@"    </table>
    <script type=""text/javascript"">
        jQuery(document).ready(function () {
            $("".logState"").hover(
                function () {
                    var logs = $("".logState"");
                    var classes = this.classList;
                    classes.remove(""logState"");
                    for (i = 0; i < logs.length; i++) {
                        var shouldHighlight = true;
                        for (j = 0; j < classes.length; j++) {
                            if (!logs[i].classList.contains(classes[j])) {
                                shouldHighlight = false;
                                break;
                            }
                        }
                        if (!shouldHighlight) {
                            logs[i].style.color = ""gray"";
                            logs[i].parentNode.style.opacity = 0.6;
                        }
                    }
                    classes.add(""logState"");
                },
                function () {
                    $("".logState"").css(""color"", ""black"");
                    $("".logState"").parent().css(""opacity"", 1.0);
                }
            );
        });
    </script>
</body>
</html>");
        }
        #pragma warning restore 1998
    }
}
