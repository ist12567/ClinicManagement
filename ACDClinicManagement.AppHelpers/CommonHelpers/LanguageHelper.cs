using System.Linq;
using System.Windows.Forms;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class InputLanguageHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Objects

        static readonly InputLanguage PersianInput;
        static readonly InputLanguage EnglishInput;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region static InputLanguageHelper()

        static InputLanguageHelper()
        {
            PersianInput = GetInputLanguageByName("persian");
            EnglishInput = GetInputLanguageByName("english");
        }

        #endregion

        #region public static InputLanguage GetInputLanguageByName(string inputName)
        public static InputLanguage GetInputLanguageByName(string inputName)
        {
            return InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().FirstOrDefault(lang => lang.Culture.EnglishName.ToLower().StartsWith(inputName));
        }

        #endregion

        #region public static void LoadPersianKeyboardLayout()
        public static void LoadPersianKeyboardLayout()
        {
            InputLanguage.CurrentInputLanguage = PersianInput ?? InputLanguage.DefaultInputLanguage;
        }

        #endregion

        #region public static void LoadEnglishKeyboardLayout()
        public static void LoadEnglishKeyboardLayout()
        {
            InputLanguage.CurrentInputLanguage = EnglishInput ?? InputLanguage.DefaultInputLanguage;
        }

        #endregion
    }
}
