function copyToClipboard(msg) {
    navigator.clipboard.writeText(msg).catch(function (error) { alert(error); });
    return true;
}