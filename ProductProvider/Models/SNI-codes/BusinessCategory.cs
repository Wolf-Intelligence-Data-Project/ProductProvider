using System.ComponentModel;

namespace ProductProvider.Models.SNI_codes
{
    public enum BusinessCategory
    {
        [Description("Jordbruk, skogsbruk och fiske")]
        A = 1,

        [Description("Utvinning av mineral")]
        B = 2,

        [Description("Tillverkning")]
        C = 3,

        [Description("Försörjning av el, gas, värme och kyla")]
        D = 4,

        [Description("Vattenförsörjning; avloppsrening, avfallshantering och sanering")]
        E = 5,

        [Description("Byggverksamhet")]
        F = 6,

        [Description("Handel; reparation av motorfordon och motorcyklar")]
        G = 7,

        [Description("Transport och magasinering")]
        H = 8,

        [Description("Hotell- och restaurangverksamhet")]
        I = 9,

        [Description("Informations- och kommunikationsverksamhet")]
        J = 10,

        [Description("Finans- och försäkringsverksamhet")]
        K = 11,

        [Description("Fastighetsverksamhet")]
        L = 12,

        [Description("Verksamhet inom juridik, ekonomi, vetenskap och teknik")]
        M = 13,

        [Description("Uthyrning, fastighetsservice, resetjänster och andra stödtjänster")]
        N = 14,

        [Description("Offentlig förvaltning och försvar; obligatorisk socialförsäkring")]
        O = 15,

        [Description("Utbildning")]
        P = 16,

        [Description("Vård och omsorg; sociala tjänster")]
        Q = 17,

        [Description("Kultur, nöje och fritid")]
        R = 18,

        [Description("Annan serviceverksamhet")]
        S = 19,

        [Description("Förvärvsarbete i hushåll; hushållens produktion av diverse varor och tjänster för eget bruk")]
        T = 20,

        [Description("Verksamhet vid internationella organisationer, utländska ambassader o.d.")]
        U = 21
    }

}
