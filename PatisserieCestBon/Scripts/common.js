//完全なリセットボタン
function clearFormAll() {
    for (var i = 0; i < document.forms.length; ++i) {
        clearForm(document.forms[i]);
    }
}
function clearForm(form) {
    for (var i = 0; i < form.elements.length; ++i) {
        clearElement(form.elements[i]);
    }
}
function clearElement(element) {
    switch (element.type) {
        case "hidden":
        case "submit":
        case "reset":
        case "button":
            return;
        case "text":
        case "password":
        case "textarea":
            element.value = "";
            return;
        default:
    }
}  
