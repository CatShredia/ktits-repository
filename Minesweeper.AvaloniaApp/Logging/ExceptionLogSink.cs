using System;
using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Logging;

namespace Minesweeper.AvaloniaApp.Logging;

public static class AvaloniaExceptionLoggingExtensions
{
    public static AppBuilder LogExceptionsToFile(this AppBuilder builder, LogEventLevel minimumLevel = LogEventLevel.Warning)
    {
        Logger.Sink = new ExceptionLogSink(minimumLevel);
        return builder;
    }
}

public sealed class ExceptionLogSink : ILogSink
{
    private readonly LogEventLevel _minimumLevel;

    public ExceptionLogSink(LogEventLevel minimumLevel = LogEventLevel.Warning)
    {
        _minimumLevel = minimumLevel;
    }

    public bool IsEnabled(LogEventLevel level, string area) => level >= _minimumLevel;

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        if (!IsEnabled(level, area))
            return;

        Publish(level, Format(area, messageTemplate, source, null), null);
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
        if (!IsEnabled(level, area))
            return;

        Exception? exception = null;
        foreach (var value in propertyValues)
        {
            if (value is Exception ex)
            {
                exception = ex;
                break;
            }
        }

        Publish(level, Format(area, messageTemplate, source, propertyValues), exception);
    }

    private static void Publish(LogEventLevel level, string message, Exception? exception)
    {
        Trace.WriteLine(message);

        if (level < LogEventLevel.Error && exception is null)
            return;

        if (exception is null)
            ExceptionLog.Append($"{level} {message}");
        else
            ExceptionLog.Append($"{level} {message}{Environment.NewLine}{exception}");
    }

    private static string Format(string area, string template, object? source, object?[]? values)
    {
        var result = new StringBuilder(template.Length + 32);
        result.Append('[').Append(area).Append("] ");

        var valueIndex = 0;
        for (var i = 0; i < template.Length; i++)
        {
            var current = template[i];
            if (current != '{')
            {
                result.Append(current);
                continue;
            }

            if (i + 1 < template.Length && template[i + 1] == '{')
            {
                result.Append('{');
                i++;
                continue;
            }

            var end = template.IndexOf('}', i + 1);
            var value = values is not null && valueIndex < values.Length ? values[valueIndex++] : null;
            result.Append('\'');
            result.Append(value is Exception ex ? $"{ex.GetType().Name}: {ex.Message}" : value);
            result.Append('\'');
            if (end < 0)
                break;
            i = end;
        }

        if (source is not null)
        {
            result.Append(" (");
            result.Append(source.GetType().Name);
            result.Append(" #");
            result.Append(source.GetHashCode());
            result.Append(')');
        }

        return result.ToString();
    }
}
