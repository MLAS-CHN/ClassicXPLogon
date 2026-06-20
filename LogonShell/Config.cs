using System;

namespace LogonShell
{
    public static class Config
    {
        public static string[] Args;

        public static void Initialize(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Args = args;
        }

        public const string Theme = "Classic";
        public const string BackgroundColor = "#1d5c8c";
        public const bool UseKeySequence = false;
        public const string LoginType = "ListLast";
        public const bool IsCancelEnabled = true;
        public const string ButtonXStyle = "Enabled";
        public const string PasswordHint = "TryFirst";
        public const bool ShowOptions = true;
        public const string NetLogon = "DomainDialUp";
        public const bool NetLogonOptions = true;
        public const bool ShowHelpText = true;
        public const bool CloseOnUpdate = false;

        public const string BtnOK = "\u786E\u5B9A";
        public const string BtnCancel = "\u53D6\u6D88";
        public const string BtnOptions = "\u9009\u9879";
        public const string BtnShutdown = "\u5173\u673A...";
        public const string BtnHelp = "\u5E2E\u52A9";

        public const string LoginTitle = "\u767B\u5F55\u5230 Windows";
        public const string LblPassword = "\u5BC6\u7801:";
        public const string LblUsername = "\u7528\u6237\u540D:";
        public const string LblDomain = "\u767B\u5F55\u5230:";
        public const string ChkDialUp = "\u4F7F\u7528\u62E8\u53F7\u8FDE\u63A5\u767B\u5F55";
        public const string CapsLockNotice = "\u5927\u5199\u9501\u5B9A\u5DF2\u5F00\u542F\uFF0C\u5EFA\u8BAE\u5173\u95ED\u3002";

        public const string LoadingTitle = "\u8BF7\u7A0D\u5019...";
        public const string LoadingText = "\u6B63\u5728\u5E94\u7528\u4E2A\u4EBA\u8BBE\u7F6E...";
        public const string ShuttingDownText = "\u6B63\u5728\u5173\u95ED Windows...";

        public const string ShutdownTitle = "\u5173\u95ED Windows";
        public const string ShutdownQuestion = "\u60A8\u5E0C\u671B\u8BA1\u7B97\u673A\u505A\u4EC0\u4E48?";
        public const string ShutdownName = "\u5173\u673A";
        public const string RestartName = "\u91CD\u65B0\u542F\u52A8";
        public const string StandbyName = "\u5F85\u673A";
        public const string ShutdownDesc = "\u7ED3\u675F\u4F1A\u8BDD\u5E76\u5173\u95ED Windows\uFF0C\u4EE5\u4FBF\u5B89\u5168\u5173\u95ED\u7535\u6E90\u3002";
        public const string RestartDesc = "\u7ED3\u675F\u4F1A\u8BDD\uFF0C\u5173\u95ED Windows\uFF0C\u7136\u540E\u91CD\u65B0\u542F\u52A8 Windows\u3002";
        public const string StandbyDesc = "\u4FDD\u6301\u4F1A\u8BDD\uFF0C\u4EE5\u4F4E\u529F\u8017\u72B6\u6001\u8FD0\u884C\u8BA1\u7B97\u673A\uFF0C\u6570\u636E\u4ECD\u4FDD\u7559\u5728\u5185\u5B58\u4E2D\u3002";

        public const string LoginErrorTitle = "\u767B\u5F55\u6D88\u606F";
        public const string LoginErrorText = "\u7CFB\u7EDF\u65E0\u6CD5\u767B\u5F55\u3002\u8BF7\u786E\u4FDD\u7528\u6237\u540D\u548C\u57DF\u6B63\u786E\uFF0C\u7136\u540E\u91CD\u65B0\u8F93\u5165\u5BC6\u7801\u3002\u5BC6\u7801\u4E2D\u7684\u5B57\u6BCD\u5FC5\u987B\u4F7F\u7528\u6B63\u786E\u7684\u5927\u5C0F\u5199\u3002\u8BF7\u786E\u4FDD\u672A\u610F\u5916\u542F\u7528\u5927\u5199\u9501\u5B9A\u3002";
        public const string ErrorTitle = "\u9519\u8BEF";

        public const string KeySeqTitle = "\u6B22\u8FCE\u4F7F\u7528 Windows";
        public const string KeySeqText = "\u8BF7\u6309 Windows \u952E\u5F00\u59CB\u3002";
        public const string KeySeqHelp = "Windows \u5E2E\u52A9\u4FDD\u62A4\u60A8\u7684\u5BC6\u7801\u5B89\u5168\u3002\u5355\u51FB\u5E2E\u52A9\u83B7\u53D6\u66F4\u591A\u4FE1\u606F\u3002";
    }
}
