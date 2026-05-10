using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace PahlUnity
{
    /// <summary>
    /// 로그 출력 편의성 개선
    /// 간단히 LOG.trace()를 호출하면 기본적으로 파일명,함수명,라인번호를 콘솔창에 출력
    /// 사용법
    /// LOG.trace();
    /// LOG.trace("message");
    /// LOG.trace(value);
    /// </summary>
    public class LOG
    {
        static private readonly char PathSpliter = Path.DirectorySeparatorChar;
        static public Action<string> EventLogSimple { get; set; } = DefaultPrinter;
        static public Action<LogDetail> EventLogDetail { get; set; } = null;

        static private void DefaultPrinter(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        static public void trace(string val = "",
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOut(val, file, caller, lineNumber, "trace");
        }

        static public void trace<T>(T val,
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOut(val.ToString(), file, caller, lineNumber, "trace");
        }

        static public void warn(string val,
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOut(val, file, caller, lineNumber, "warn");
        }

        static public void warn<T>(T val,
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOut(val.ToString(), file, caller, lineNumber, "warn");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEBUG")]
        static public void errorif<T>(bool isError, T val,
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!isError)
                return;

            string message = LogOut(val.ToString(), file, caller, lineNumber, "error");
            throw new Exception(message);
        }

        static public void callStack(string val = "",
            [CallerFilePath] string file = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            LogDetail log = new LogDetail();
            string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "] [" + log.stackTrace + "]";
            EventLogSimple?.Invoke(message);
        }

        static private string LogOut(string val, string file, string caller, int lineNumber, string logLevel)
        {
            if (EventLogDetail != null)
            {
                LogDetail log = new LogDetail();
                log.logLevel = logLevel;
                log.fileName = ParseFilename(file);
                log.funcName = caller;
                log.lineNumber = lineNumber.ToString();
                log.message = val;
                EventLogDetail?.Invoke(log);
            }
            string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "]";
            EventLogSimple?.Invoke(message);
            return message;
        }

        static private string ParseFilename(string fullPath)
        {
            int idx = fullPath.LastIndexOf(PathSpliter);
            if (0 <= idx && idx < fullPath.Length)
            {
                return fullPath.Substring(idx);
            }
            return fullPath;
        }

        public class LogDetail
        {
            public string time; //로그 남기는 당시 시간
            public string threadID; //쓰레드 ID
            public string logLevel; //로그레벨
            public string fileName; //호출당시 파일이름
            public string funcName; //호출당시 함수이름
            public string lineNumber; //호출당시 코드 라인 번호
            public string message;  //사용자 로그 메시지
            public string stackTrace; //호출스택
            public override string ToString()
            {
                return time + "," + threadID + "," + logLevel + "," + fileName + "," + funcName + "," + lineNumber + "," + message + "," + stackTrace;
            }
            public LogDetail()
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                threadID = Thread.CurrentThread.ManagedThreadId.ToString();
                stackTrace = "";
                StackFrame[] frames = new StackTrace().GetFrames();
                int cnt = Mathf.Min(frames.Length, 7);
                for (int i = 0; i < cnt; ++i)
                {
                    stackTrace += (frames[i].GetMethod().Name + ">");
                }
            }
        }
    }
}