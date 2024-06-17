// Function to show a warning popup
function showWarningPopup(message) {
    Swal.fire({
        position: "top-end",
        icon: 'warning',
        title: message,
        showConfirmButton: false,
        timer: 1500
    });
}

// Function to show a success popup
function showSuccessPopup(message) {
    Swal.fire({
        position: "top-end",
        icon: 'success',
        title: message,
        showConfirmButton: false,
        timer: 1500
    });
}

// Function to show a failure popup
function showFailurePopup(message) {
    Swal.fire({
        position: "top-end",
        icon: 'error',
        title: message,
        showConfirmButton: false,
        timer: 1500
    });
}
