/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Common.Logging;

public class Log : ILog
{
    public string Name { get; set; }
    public bool IsDebugEnabled { get; private set; }
    public bool IsInfoEnabled { get; private set; }
    public bool IsWarnEnabled { get; private set; }
    public bool IsErrorEnabled { get; private set; }
    public bool IsFatalEnabled { get; private set; }
    public bool IsTraceEnabled { get; private set; }
    public string LogDirectory => log4net.GlobalContext.Properties["LogDirectory"].ToString();

    private log4net.ILog _logger;

    public Log(string name)
    {
        Configure(name);
    }

    static Log()
    {
        XmlConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetCallingAssembly()));
    }

    public void Configure(string name)
    {
        _logger = log4net.LogManager.GetLogger(Assembly.GetCallingAssembly(), name);

        IsDebugEnabled = _logger.IsDebugEnabled;
        IsInfoEnabled = _logger.IsInfoEnabled;
        IsWarnEnabled = _logger.IsWarnEnabled;
        IsErrorEnabled = _logger.IsErrorEnabled;
        IsFatalEnabled = _logger.IsFatalEnabled;
        IsTraceEnabled = _logger.Logger.IsEnabledFor(Level.Trace);
    }

    public void Trace(object message)
    {
        if (IsTraceEnabled)
        {
            _logger.Logger.Log(GetType(), Level.Trace, message, null);
        }
    }

    public void TraceFormat(string message, object arg0)
    {
        if (IsTraceEnabled)
        {
            _logger.Logger.Log(GetType(), Level.Trace, string.Format(message, arg0), null);
        }
    }

    public void Debug(object message)
    {
        if (IsDebugEnabled)
        {
            _logger.Debug(message);
        }
    }

    public void Debug(object message, Exception exception)
    {
        if (IsDebugEnabled)
        {
            _logger.Debug(message, exception);
        }
    }

    public void DebugFormat(string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            _logger.DebugFormat(format, args);
        }
    }

    public void DebugFormat(string format, object arg0)
    {
        if (IsDebugEnabled)
        {
            _logger.DebugFormat(format, arg0);
        }
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
        if (IsDebugEnabled)
        {
            _logger.DebugFormat(format, arg0, arg1);
        }
    }

    public void DebugFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsDebugEnabled)
        {
            _logger.DebugFormat(format, arg0, arg1, arg2);
        }
    }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            _logger.DebugFormat(provider, format, args);
        }
    }

    public void DebugWithProps(string message, IEnumerable<KeyValuePair<string, object>> props)
    {
        if (!IsDebugEnabled)
        {
            return;
        }

        foreach (var p in props)
        {
            log4net.ThreadContext.Properties[p.Key] = p.Value;
        }

        _logger.Debug(message);
    }


    public void Info(object message)
    {
        if (IsInfoEnabled)
        {
            _logger.Info(message);
        }
    }

    public void Info(string message, Exception exception)
    {
        if (IsInfoEnabled)
        {
            _logger.Info(message, exception);
        }
    }

    public void InfoFormat(string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            _logger.InfoFormat(format, args);
        }
    }

    public void InfoFormat(string format, object arg0)
    {
        if (IsInfoEnabled)
        {
            _logger.InfoFormat(format, arg0);
        }
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
        if (IsInfoEnabled)
        {
            _logger.InfoFormat(format, arg0, arg1);
        }
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsInfoEnabled)
        {
            _logger.InfoFormat(format, arg0, arg1, arg2);
        }
    }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            _logger.InfoFormat(provider, format, args);
        }
    }


    public void Warn(object message)
    {
        if (IsWarnEnabled)
        {
            _logger.Warn(message);
        }
    }

    public void Warn(object message, Exception exception)
    {
        if (IsWarnEnabled)
        {
            _logger.Warn(message, exception);
        }
    }

    public void WarnFormat(string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            _logger.WarnFormat(format, args);
        }
    }

    public void WarnFormat(string format, object arg0)
    {
        if (IsWarnEnabled)
        {
            _logger.WarnFormat(format, arg0);
        }
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
        if (IsWarnEnabled)
        {
            _logger.WarnFormat(format, arg0, arg1);
        }
    }

    public void WarnFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsWarnEnabled)
        {
            _logger.WarnFormat(format, arg0, arg1, arg2);
        }
    }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            _logger.WarnFormat(provider, format, args);
        }
    }


    public void Error(object message)
    {
        if (IsErrorEnabled)
        {
            _logger.Error(message);
        }
    }

    public void Error(object message, Exception exception)
    {
        if (IsErrorEnabled)
        {
            _logger.Error(message, exception);
        }
    }

    public void ErrorFormat(string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            _logger.ErrorFormat(format, args);
        }
    }

    public void ErrorFormat(string format, object arg0)
    {
        if (IsErrorEnabled)
        {
            _logger.ErrorFormat(format, arg0);
        }
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
        if (IsErrorEnabled)
        {
            _logger.ErrorFormat(format, arg0, arg1);
        }
    }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsErrorEnabled)
        {
            _logger.ErrorFormat(format, arg0, arg1, arg2);
        }
    }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            _logger.ErrorFormat(provider, format, args);
        }
    }


    public void Fatal(object message)
    {
        if (IsFatalEnabled)
        {
            _logger.Fatal(message);
        }
    }

    public void Fatal(string message, Exception exception)
    {
        if (IsFatalEnabled)
        {
            _logger.Fatal(message, exception);
        }
    }

    public void FatalFormat(string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            _logger.FatalFormat(format, args);
        }
    }

    public void FatalFormat(string format, object arg0)
    {
        if (IsFatalEnabled)
        {
            _logger.FatalFormat(format, arg0);
        }
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
        if (IsFatalEnabled)
        {
            _logger.FatalFormat(format, arg0, arg1);
        }
    }

    public void FatalFormat(string format, object arg0, object arg1, object arg2)
    {
        if (IsFatalEnabled)
        {
            _logger.FatalFormat(format, arg0, arg1, arg2);
        }
    }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            _logger.FatalFormat(provider, format, args);
        }
    }

    public void DebugWithProps(string message, KeyValuePair<string, object> prop1, KeyValuePair<string, object> prop2, KeyValuePair<string, object> prop3)
    {
        if (!IsDebugEnabled)
        {
            return;
        }

        AddProp(prop1);
        AddProp(prop2);
        AddProp(prop3);

        _logger.Debug(message);

        static void AddProp(KeyValuePair<string, object> p)
        {
            log4net.ThreadContext.Properties[p.Key] = p.Value;
        }
    }
}