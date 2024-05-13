var current_step = 0;
function nextStep() {
    if (current_step < $(".step-wizard").length - 1) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step++;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
        });
    }
}

function previousStep() {
    if (current_step > 0) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step--;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
        });
    }
}

function calculateBarValue() {
    const step_amount = 100 / $(".step-wizard").length;
    const new_value = step_amount * (current_step + 1);

    $(".prevention-value").width(new_value + "%");
    $("#value-txt").text(Math.round(new_value) + "%");
}