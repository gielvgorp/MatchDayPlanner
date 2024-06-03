// global.js

// Function to show a warning popup
function showWarningPopup(message) {
    Swal.fire({
        icon: 'warning',
        title: 'Let op!',
        text: message,
        confirmButtonText: 'OK'
    });
}

// Function to show a success popup
function showSuccessPopup(message) {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        confirmButtonText: 'OK'
    });
}

// Function to show a failure popup
function showFailurePopup(message) {
    Swal.fire({
        icon: 'error',
        title: 'Er is een fout opgetreden!',
        text: message,
        confirmButtonText: 'OK'
    });
}
