Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.method
Imports jp.co.systembase.report.operator
Imports jp.co.systembase.report.elementpreprocessor
Imports jp.co.systembase.report.search.searchobject
Imports jp.co.systembase.report.textformatter

Public Class ReportSetting
    Implements ICloneable

    Public MethodMap As New Dictionary(Of String, IMethod)
    Public OperatorMap As New Dictionary(Of String, IOperator)
    Public DefaultElementPreprocessor As IElementPreprocessor
    Public ElementPreprocessorMap As New Dictionary(Of String, IElementPreprocessor)
    Public DefaultSeachObject As ISearchObject
    Public SeacheObjectMap As New Dictionary(Of String, ISearchObject)
    Public DefaultTextFormatter As ITextFormatter
    Public TextFormatterMap As New Dictionary(Of String, ITextFormatter)

    Public Logger As IReportLogger = Nothing

    Public Sub New()
        Me.MethodMap.Add("field", New FieldMethod)
        Me.MethodMap.Add("global", New GlobalMethod)
        Me.MethodMap.Add("sum", New FieldSumMethod)
        Me.MethodMap.Add("sum_at", New FieldSumAtMethod)
        Me.MethodMap.Add("sum_page", New FieldSumPageMethod)
        Me.MethodMap.Add("avr", New AverageMethod)
        Me.MethodMap.Add("avr_at", New AverageAtMethod)
        Me.MethodMap.Add("avr_page", New AveragePageMethod)
        Me.MethodMap.Add("count", New RowCountMethod)
        Me.MethodMap.Add("count_at", New RowCountAtMethod)
        Me.MethodMap.Add("count_page", New RowCountPageMethod)
        Me.MethodMap.Add("page", New PageMethod)
        Me.MethodMap.Add("page_count", New PageCountMethod)
        Me.MethodMap.Add("toggle", New ToggleMethod)
        Me.MethodMap.Add("state", New StateMethod)
        Me.MethodMap.Add("prev", New PreviousMethod)
        Me.MethodMap.Add("next", New NextMethod)
        Me.MethodMap.Add("var", New VariableMethod)
        Me.MethodMap.Add("current_time", New CurrentTimeMethod)
        Me.MethodMap.Add("crosstab", New CrosstabMethod)
        Me.MethodMap.Add("page_group_count", New PageGroupCountMethod)
        Me.MethodMap.Add("debug", New DummyMethod)
        Me.OperatorMap.Add("+", New AddOperator)
        Me.OperatorMap.Add("-", New SubOperator)
        Me.OperatorMap.Add("*", New MulOperator)
        Me.OperatorMap.Add("/", New DivOperator)
        Me.OperatorMap.Add("&", New CatOperator)
        Me.OperatorMap.Add("mod", New ModOperator)
        Me.OperatorMap.Add("%", New ModOperator)
        Me.OperatorMap.Add("=", New EqOperator)
        Me.OperatorMap.Add("eq", New EqOperator)
        Me.OperatorMap.Add("in", New EqOperator)
        Me.OperatorMap.Add("!=", New NotEqOperator)
        Me.OperatorMap.Add("neq", New NotEqOperator)
        Me.OperatorMap.Add("nin", New NotEqOperator)
        Me.OperatorMap.Add(">", New GreaterOperator)
        Me.OperatorMap.Add(">=", New GreaterEqOperator)
        Me.OperatorMap.Add("<", New LessOperator)
        Me.OperatorMap.Add("<=", New LessEqOperator)
        Me.OperatorMap.Add("!", New NotOperator)
        Me.OperatorMap.Add("not", New NotOperator)
        Me.OperatorMap.Add("&&", New AndOperator)
        Me.OperatorMap.Add("and", New AndOperator)
        Me.OperatorMap.Add("||", New OrOperator)
        Me.OperatorMap.Add("or", New OrOperator)
        Me.OperatorMap.Add("if", New IfOperator)
        Me.OperatorMap.Add("nvl", New NvlOperator)
        Me.OperatorMap.Add("round5", New Round5Operator)
        Me.OperatorMap.Add("round5e", New Round5EOperator)
        Me.OperatorMap.Add("round6", New Round6Operator)
        Me.OperatorMap.Add("roundup", New RoundUpOperator)
        Me.OperatorMap.Add("roundup2", New RoundUp2Operator)
        Me.OperatorMap.Add("rounddown", New RoundDownOperator)
        Me.OperatorMap.Add("abs", New AbsOperator)
        Me.OperatorMap.Add("max", New MaxOperator)
        Me.OperatorMap.Add("min", New MinOperator)
        Me.OperatorMap.Add("year", New YearOperator)
        Me.OperatorMap.Add("month", New MonthOperator)
        Me.OperatorMap.Add("day", New DayOperator)
        Me.OperatorMap.Add("hour", New HourOperator)
        Me.OperatorMap.Add("minute", New MinuteOperator)
        Me.OperatorMap.Add("second", New SecondOperator)
        Me.OperatorMap.Add("dateadd", New DateAddOperator)
        Me.OperatorMap.Add("sub", New SubStringOperator)
        Me.OperatorMap.Add("wsub", New WSubStringOperator)
        Me.OperatorMap.Add("len", New StringLenOperator)
        Me.OperatorMap.Add("wlen", New WStringLenOperator)
        Me.OperatorMap.Add("digit", New DigitOperator)
        Me.OperatorMap.Add("wsplit", New WSplitOperator)
        Me.OperatorMap.Add("wlines", New WLinesOperator)
        Me.OperatorMap.Add("trim", New TrimOperator)
        Me.OperatorMap.Add("ybcode", New YbCodeOperator)
        Me.OperatorMap.Add("weekday", New WeekdayOperator)
        Me.DefaultElementPreprocessor = New DefaultPreprocessor
        Me.ElementPreprocessorMap.Add("subpage", New SubPagePreprocessor)
        Me.DefaultSeachObject = New DefaultSearchObject
        Me.SeacheObjectMap.Add("field", New FieldSearchObject)
        Me.SeacheObjectMap.Add("text", New TextSearchObject)
        Me.DefaultTextFormatter = New DefaultTextFormatter
        Me.TextFormatterMap.Add("default", Me.DefaultTextFormatter)
    End Sub

    Private Sub New(setting As ReportSetting)
        For Each k As String In setting.MethodMap.Keys
            Me.MethodMap.Add(k, setting.MethodMap(k))
        Next
        For Each k As String In setting.OperatorMap.Keys
            Me.OperatorMap.Add(k, setting.OperatorMap(k))
        Next
        Me.DefaultElementPreprocessor = setting.DefaultElementPreprocessor
        For Each k As String In setting.ElementPreprocessorMap.Keys
            Me.ElementPreprocessorMap.Add(k, setting.ElementPreprocessorMap(k))
        Next
        Me.DefaultSeachObject = setting.DefaultSeachObject
        For Each k As String In setting.SeacheObjectMap.Keys
            Me.SeacheObjectMap.Add(k, setting.SeacheObjectMap(k))
        Next
        Me.DefaultTextFormatter = setting.DefaultTextFormatter
        For Each k As String In setting.TextFormatterMap.Keys
            Me.TextFormatterMap.Add(k, setting.TextFormatterMap(k))
        Next
        Me.Logger = setting.Logger
    End Sub

    Public Function GetMethod(key As String) As IMethod
        If key Is Nothing Then
            Return Me.MethodMap("field")
        ElseIf Me.MethodMap.ContainsKey(key) Then
            Return Me.MethodMap(key)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetOperator(key As String) As IOperator
        If key IsNot Nothing AndAlso Me.OperatorMap.ContainsKey(key) Then
            Return Me.OperatorMap(key)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetElementPreprocessor(key As String) As IElementPreprocessor
        If key IsNot Nothing AndAlso Me.ElementPreprocessorMap.ContainsKey(key) Then
            Return Me.ElementPreprocessorMap(key)
        Else
            Return Me.DefaultElementPreprocessor
        End If
    End Function

    Public Function GetSearchObject(key As String) As ISearchObject
        If key IsNot Nothing AndAlso Me.SeacheObjectMap.ContainsKey(key) Then
            Return Me.SeacheObjectMap(key)
        Else
            Return Me.DefaultSeachObject
        End If
    End Function

    Public Function GetTextFormatter(key As String) As ITextFormatter
        If key IsNot Nothing AndAlso Me.TextFormatterMap.ContainsKey(key) Then
            Return Me.TextFormatterMap(key)
        Else
            Return Me.DefaultTextFormatter
        End If
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New ReportSetting(Me)
    End Function

End Class
