using p3ppc.hdsfx.Template.Configuration;
using System.ComponentModel;

namespace p3ppc.hdsfx.Configuration
{
    public class Config : Configurable<Config>
    {
        /*
            User Properties:
                - Please put all of your configurable properties here.

            By default, configuration saves as "Config.json" in mod user config folder.    
            Need more config files/classes? See Configuration.cs

            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */
        
        [DisplayName("Outline Flash Speed")]
        [Description("How fast the outline around the next skill to select flashes.")]
        [DefaultValue(2)]
        public int OutlineAlphaChangeMultipler { get; set; } = 2;

        [DisplayName("Outline Flash Minimum Alpha")]
        [Description("How low the alpha (transparency) gets when the outline for the next skill flashes." +
            "\nHigher means the flash will transition at a brighter colour, lower means a darker colour." +
            "\nMax of 255 (would be no flash)")]
        [DefaultValue((byte)80)]
        public byte MinOutlineAlpha { get; set; } = 80;

        [DisplayName("Use Text For Empty Skills")]
        [Description("If enabled text will be used for empty skills which gives the appearance of much thinner dashes.\nIf disabled empty skills will be indicated by thick dashes.")]
        [DefaultValue(false)]
        public bool EmptySkillsUseText { get; set; } = false;

        [DisplayName("Empty Skill Name")]
        [Description("What text is shown for empty skills")]
        [DefaultValue("-------")]
        public string EmptySkillName { get; set; } = "-------";

        [DisplayName("Alternate Choose Skills Text")]
        [Description("Use an alternate thinner style for the \"Choose which skills to inherit\" text")]
        [DefaultValue(false)]
        public bool AlternateChooseSkills { get; set; } = false;

        [DisplayName("Debug Mode")]
        [Description("Logs additional information to the console that is useful for debugging.")]
        [DefaultValue(false)]
        public bool DebugEnabled { get; set; } = false;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}