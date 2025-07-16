function removeAlertOnScreen() {
    var alertItem = document.querySelector('#update-alert');
    if (alertItem != null && alertItem.className == 'fadein') {
        alertItem.className = 'fadeout';
        setTimeout(function () { alertItem.className = ''; }, 1000);
    }
    return true;
}