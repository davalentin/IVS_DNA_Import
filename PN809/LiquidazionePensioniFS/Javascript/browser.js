// Environment checks
/* -------------------------------------------------------------------------- */

function IsChrome() {
    // check if library is used as a Node.js module
    if (typeof window !== 'undefined') {
        // is current browser chrome?
        return /chrome|chromium/i.test(GetUserAgent()) && /google inc/.test(GetVendor());
    }
}

function IsFirefox() {
    if (typeof window !== 'undefined') {
        // is current browser firefox?
        return /firefox/i.test(GetUserAgent());
    }
}

function IsIe() {
    if (typeof window !== 'undefined') {
        // is current browser internet explorer?
        return /msie/i.test(GetUserAgent());
    }
}
function IsOpera() {
    if (typeof window !== 'undefined') {
        // is current browser opera?
        return /^Opera\//.test(userAgent) || // Opera 12 and older versions
                /\x20OPR\//.test(userAgent); // Opera 15+
    }
}
function IsSafari() {
    if (typeof window !== 'undefined') {
        // is current browser safari?
        return /safari/i.test(GetUserAgent()) && /apple computer/i.test(GetVendor());
    }
}

function GetUserAgent() {
    return 'navigator' in window && 'userAgent' in navigator && navigator.userAgent.toLowerCase() || '';
}

function GetVendor() {
    return 'navigator' in window && 'vendor' in navigator && navigator.vendor.toLowerCase() || '';
}

function GetAppVersion() {
    return 'navigator' in window && 'appVersion' in navigator && navigator.appVersion.toLowerCase() || '';
}

function CheckBrowser() {
    if (IsIe()) window.alert("Ie");
    if (IsChrome()) window.alert("Chrome");
    if (IsFirefox()) window.alert("Firefox");
    if (IsSafari()) window.alert("Safari");
}
