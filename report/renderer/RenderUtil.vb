Imports System.Text
Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter

Namespace renderer

    Public Module RenderUtil

        Public COLOR_NAMES() As String = { _
          "black", "dimgray", "gray", "darkgray", "silver", _
          "lightgray", "gainsboro", "whitesmoke", "white", "snow", _
          "ghostwhite", "floralwhite", "linen", "antiquewhite", "papayawhip", _
          "blanchedalmond", "bisque", "moccasin", "navajowhite", "peachpuff", _
          "mistyrose", "lavenderblush", "seashell", "oldlace", "ivory", _
          "honeydew", "mintcream", "azure", "aliceblue", "lavender", _
          "lightsteelblue", "lightslategray", "slategray", "steelblue", "royalblue", _
          "midnightblue", "navy", "darkblue", "mediumblue", "blue", _
          "dodgerblue", "cornflowerblue", "deepskyblue", "lightskyblue", "skyblue", _
          "lightblue", "powderblue", "paleturquoise", "lightcyan", "cyan", _
          "aqua", "turquoise", "mediumturquoise", "darkturquoise", "lightseagreen", _
          "cadetblue", "darkcyan", "teal", "darkslategray", "darkgreen", _
          "green", "forestgreen", "seagreen", "mediumseagreen", "mediumaquamarine", _
          "darkseagreen", "aquamarine", "palegreen", "lightgreen", "springgreen", _
          "mediumspringgreen", "lawngreen", "chartreuse", "greenyellow", "lime", _
          "limegreen", "yellowgreen", "darkolivegreen", "olivedrab", "olive", _
          "darkkhaki", "palegoldenrod", "cornsilk", "beige", "lightyellow", _
          "lightgoldenrodyellow", "lemonchiffon", "wheat", "burlywood", "tan", _
          "khaki", "yellow", "gold", "orange", "sandybrown", _
          "darkorange", "goldenrod", "peru", "darkgoldenrod", "chocolate", _
          "sienna", "saddlebrown", "maroon", "darkred", "brown", _
          "firebrick", "indianred", "rosybrown", "darksalmon", "lightcoral", _
          "salmon", "lightsalmon", "coral", "tomato", "orangered", _
          "red", "crimson", "mediumvioletred", "deeppink", "hotpink", _
          "palevioletred", "pink", "lightpink", "thistle", "magenta", _
          "fuchsia", "violet", "plum", "orchid", "mediumorchid", _
          "darkorchid", "darkviolet", "darkmagenta", "purple", "indigo", _
          "darkslateblue", "blueviolet", "mediumpurple", "slateblue", "mediumslateblue"}

        Public Function Format(reportDesign As ReportDesign, formatterDesign As ElementDesign, value As Object) As String
            Return reportDesign.Setting.GetTextFormatter(formatterDesign.Get("type")).Format(value, formatterDesign)
        End Function

    End Module

End Namespace

