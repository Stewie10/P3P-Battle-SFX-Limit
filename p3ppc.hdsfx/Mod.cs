using p3ppc.hdsfx.Configuration;
using p3ppc.hdsfx.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using CriFs.V2.Hook.Interfaces;
using Reloaded.Universal.Localisation.Framework.Interfaces;
using Reloaded.Memory;
using Reloaded.Memory.Structs;

namespace p3ppc.hdsfx
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public unsafe class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private Memory _memory;

        private IAsmHook _setSoundLimitHook;

        private IReverseWrapper<SoundLimitDelegate> _startSoundLimitReverseWrapper;

        private MemoryAllocation _inSoundLimit;

        private TimeSpan movementDelay = TimeSpan.FromMilliseconds(100);
        private TimeSpan movementInitialDelay = TimeSpan.FromMilliseconds(230);

        private Language _language;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            _memory = Memory.Instance;

            Utils.Initialise(_logger, _configuration);

            var startupScannerController = _modLoader.GetController<IStartupScanner>();
            if (startupScannerController == null || !startupScannerController.TryGetTarget(out var startupScanner))
            {
                Utils.LogError($"Unable to get controller for Reloaded SigScan Library, stuff won't work :(");
                return;
            }

            var criFsController = _modLoader.GetController<ICriFsRedirectorApi>();
            if (criFsController == null || !criFsController.TryGetTarget(out var criFsApi))
            {
                Utils.LogError($"Unable to get controller for CriFs Lib, things will not work :(");
                return;
            }

            var localisationFrameworkController = _modLoader.GetController<ILocalisationFramework>();
            if (localisationFrameworkController == null || !localisationFrameworkController.TryGetTarget(out var localisationFrameworkApi))
            {
                Utils.LogError($"Unable to get controller for Localisation Framework, things will not work :(");
                return;
            }

            if (!localisationFrameworkApi.TryGetLanguage(out _language))
            {
                Utils.LogError("Failed to get the language from localisation framework. Things might look funny...");
                _language = Language.English;
            }

            _inSoundLimit = _memory.Allocate(4);

            startupScanner.AddMainModuleScan("D1 EE 66 FF C7 48 83 C3 10 66 83 FF 10 7C 90 48 8B 4C 24 ?? 48 33 CC", result =>
                {

                    string[] function =
                    {
                        "use64",
                        "label Limit200",
                        "cmp [rbx], ebp",
                        "shr esi, 1",
                        "inc di",
                        "add rbx, 0x10",
                        "cmp di, 0xC8",
                        "jl Limit200",
                         $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(Utils.BaseAddress + result.Offset + 15, true)}",
                    };
                    _setSoundLimitHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
                });

        }

        private void SoundLimit(int param1)
        {
            for (int i = 0; i < 200; ++i)
            {
                _memory.WriteWithMarshalling(_inSoundLimit.Address, true);
            }
        }

        [Function(CallingConventions.Microsoft)]
        private delegate void SoundLimitDelegate(int param1);

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}