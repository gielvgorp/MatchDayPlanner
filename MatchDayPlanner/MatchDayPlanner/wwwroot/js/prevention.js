var current_step = 0;
$(document).ready(function () {
    var step_containers = $(".step-wizard");
});

function nextStep() {
    $(".step-wizard").eq(current_step).fadeOut(500, function () {
        current_step++;
        $(".step-wizard").eq(current_step).fadeIn();
    });
}

function previousStep() {
    $(".step-wizard").eq(current_step).fadeOut(500, function () {
        current_step--;
        $(".step-wizard").eq(current_step).fadeIn();
    });
}