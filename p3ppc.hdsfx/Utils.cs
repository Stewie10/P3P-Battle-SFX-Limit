using p3ppc.hdsfx.Configuration;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p3ppc.hdsfx
{
    internal class Utils
    {
        private static ILogger _logger;
        private static Config _config;
        internal static nint BaseAddress { get; private set; }

        internal static void Initialise(ILogger logger, Config config)
        {
            _logger = logger;
            _config = config;
            using var thisProcess = Process.GetCurrentProcess();
            BaseAddress = thisProcess.MainModule!.BaseAddress;
        }

        internal static void LogDebug(string message)
        {
            if (_config.DebugEnabled)
                _logger.WriteLine($"[Manual Skill Inheritance] {message}");
        }

        internal static void Log(string message)
        {
            _logger.WriteLine($"[Manual Skill Inheritance] {message}");
        }

        internal static void LogError(string message, Exception e)
        {
            _logger.WriteLine($"[Manual Skill Inheritance] {message}: {e.Message}", System.Drawing.Color.Red);
        }

        internal static void LogError(string message)
        {
            _logger.WriteLine($"[Manual Skill Inheritance] {message}", System.Drawing.Color.Red);
        }

        // Pushes the value of an xmm register to the stack, saving it so it can be restored with PopXmm
        public static string PushXmm(int xmmNum)
        {
            return // Save an xmm register 
                $"sub rsp, 16\n" + // allocate space on stack
                $"movdqu dqword [rsp], xmm{xmmNum}\n";
        }

        // Pushes all xmm registers (0-15) to the stack, saving them to be restored with PopXmm
        public static string PushXmm()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sb.Append(PushXmm(i));
            }
            return sb.ToString();
        }

        // Pops the value of an xmm register to the stack, restoring it after being saved with PushXmm
        public static string PopXmm(int xmmNum)
        {
            return                 //Pop back the value from stack to xmm
                $"movdqu xmm{xmmNum}, dqword [rsp]\n" +
                $"add rsp, 16\n"; // re-align the stack
        }

        // Pops all xmm registers (0-7) from the stack, restoring them after being saved with PushXmm
        public static string PopXmm()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 7; i >= 0; i--)
            {
                sb.Append(PopXmm(i));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the address of a global from something that references it
        /// </summary>
        /// <param name="ptrAddress">The address to the pointer to the global (like in a mov instruction or something)</param>
        /// <returns>The address of the global</returns>
        internal static unsafe nuint GetGlobalAddress(nint ptrAddress)
        {
            return (nuint)((*(int*)ptrAddress) + ptrAddress + 4);
        }

        /// <summary>
        /// Gets instructions to compare the value of a pointer
        /// </summary>
        /// <param name="pointer">The address of the pointer</param>
        /// <param name="cmpValue">The value to compare to</param>
        /// <param name="cmpSize">The size of the comparison (byte, word, dword, qword)</param>
        /// <returns>Assembly instructions that will compare the value being pointed to</returns>

        internal static string CmpValue(nint pointer, int cmpValue, string cmpSize)
        {
            return
            $"push rax\n" +
            $"mov rax, [qword {pointer}]\n" +
            $"cmp [rax], {cmpSize} {cmpValue}\n" +
            $"pop rax";
        }

        /// <summary>
        /// Gets instructions to set the value of a pointer
        /// </summary>
        /// <param name="pointer">The address of the pointer</param>
        /// <param name="cmpValue">The value to set to</param>
        /// <param name="cmpSize">The size of the value (byte, word, dword, qword)</param>
        /// <returns>Assembly instructions that will set the value being pointed to</returns>

        internal static string SetValue(nint pointer, int cmpValue, string cmpSize)
        {
            return
            $"push rax\n" +
            $"mov rax, [qword {pointer}]\n" +
            $"mov [rax], {cmpSize} {cmpValue}\n" +
            $"pop rax";
        }

    }
}
