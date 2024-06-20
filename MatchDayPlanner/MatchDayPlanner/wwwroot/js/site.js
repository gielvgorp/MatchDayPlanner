function copyToClipboard(value) {
    navigator.clipboard.writeText(value);
    showSuccessPopup("Link gekopieerd!");   
}