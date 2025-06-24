export function localLanguage(){
    const cookies = document.cookie.split(';').map(c => c.trim());

    const rawlanguageCookie = cookies.find(c => c.startsWith('.AspNetCore.Culture'));
    if (!rawlanguageCookie) {
        console.error('Language cookie is not set');
        return "";
    }

    const languageCookie = decodeURIComponent(rawlanguageCookie);
    const localeString = languageCookie.split(/=|\|/)[2];
    return localeString;
}