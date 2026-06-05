import { IntlProvider } from "react-intl";
import en from "../locales/en.json";
import fr from "../locales/fr.json";
import * as React from "react";

const messages = {
    en,
    fr
};

type Props = {
    locale: "en" | "fr";
    children: React.ReactNode;
};

export default function I18nProvider({
                                         locale,
                                         children
                                     }: Props) {
    return (
        <IntlProvider
            locale={locale}
            messages={messages[locale]}
        >
            {children}
        </IntlProvider>
    );
}