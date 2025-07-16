function alertOnScreen(msg) {
    var alertItem = document.querySelector('#update-alert');
    var alertItemMessage = document.querySelector('#update-alert-message');
    alertItemMessage.innerHTML = msg;
    alertItem.className = 'fadein';
    return true;
}