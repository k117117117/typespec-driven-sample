import polyglotI18nProvider from "ra-i18n-polyglot";
import ja from "./ja";

export const i18nProvider = polyglotI18nProvider(() => ja, "ja");
