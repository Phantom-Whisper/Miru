import ReactDOM from 'react-dom/client';
import './index.css'
import App from './App.tsx'
import I18nProvider from "./i18n/I18nProvider.tsx";

ReactDOM.createRoot(document.getElementById('root')!).render(
    <I18nProvider locale="en">
        <App />
    </I18nProvider>
)
